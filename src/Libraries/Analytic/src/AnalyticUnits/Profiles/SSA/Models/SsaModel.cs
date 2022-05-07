using Logger;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Analytic.AnalyticUnits.Profiles.SSA.Models
{
    /// <summary>
    ///     Модель SSA
    /// </summary>
    internal class SsaModel
    {
        #region Fields

        private static readonly ILoggerDecorator Log = LoggerManager.CreateDefaultLogger();
        private readonly int _pricesLength;
        private Matrix<double> _covariationMatrix;
        private Matrix<double> _eigenVectors;
        private IEnumerable<Complex> _neededLambdas = new List<Complex>();
        private MathNet.Numerics.LinearAlgebra.Vector<Complex> _eigenValues;
        private double[,] _restoredOriginalMatrix = new double[0, 0];
        private const int _neededLambdaNumber = 7; // Кол-во значимых собственных значений, которое будет отбираться
        private const double _minMagnitudeForSsaSmoothing = 0.0002; // Минимальная магнитуда
                                                                    // для собственного значения при сглаживании

        #endregion

        #region .ctor

        /// <inheritdoc cref="SsaModel"/>
        private SsaModel(int tauDelayNumber, int pricesLength, int delayVectorsNumber, Matrix<double> delayMatrix)
        {
            TauDelayNumber = tauDelayNumber;
            _pricesLength = pricesLength;
            DelayVectorsNumber = delayVectorsNumber;
            DelayMatrix = delayMatrix;
        }

        /// <inheritdoc cref="SsaModel"/>
        public static SsaModel Create(double[] prices)
        {
            // преобразование одномерного ряда в многомерный
            var pricesLength = prices.Length;
            var tauDelayNumber = (pricesLength + 1) / 2;
            var delayVectorsNumber = pricesLength - tauDelayNumber + 1;
            var delayMatrix = Matrix<double>.Build.Dense(tauDelayNumber, delayVectorsNumber);
            for (var i = 0; i < tauDelayNumber; i++)
            {
                for (var j = 0; j < delayVectorsNumber; j++)
                {
                    delayMatrix[i, j] = prices[i + j];
                }
            }

            return new SsaModel(tauDelayNumber, pricesLength, delayVectorsNumber, delayMatrix);
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Число задержек тау
        /// </summary>
        public int TauDelayNumber { get; }

        /// <summary>
        ///     Кол-во векторов задержек
        /// </summary>
        public int DelayVectorsNumber { get; }

        /// <summary>
        ///     Матрица задержек (<see cref="TauDelayNumber"/>, <see cref="DelayVectorsNumber"/>)
        /// </summary>
        public Matrix<double> DelayMatrix { get; }

        /// <summary>
        ///     Матрица главных компонент (для восстановления сигнала)
        /// </summary>
        public Matrix<double> MainComponentsMatrix { get; private set; }

        #endregion

        #region Public methods

        /// <summary>
        ///     Вычисляет ковариационную матрицу
        /// </summary>
        internal void ComputeCovariationMatrix()
        {
            // Защита от лишних вычислений
            if (_covariationMatrix is not null)
            {
                return;
            }

            var multiplicationMatrix = DelayMatrix.TransposeAndMultiply(DelayMatrix);
            _covariationMatrix = 1 / (double)DelayVectorsNumber * multiplicationMatrix;
        }

        /// <summary>
        ///     Вычисляет матрицу собственных векторов и вектор собственных значений
        /// </summary>
        internal void ComputeEigenVectorsAndValues()
        {
            // Валидация исходных данных перед вычислением
            if (IsCovariationMatrixIsNull())
            {
                return;
            }

            // Защита от лишних вычислений
            if (_eigenVectors is not null && _eigenValues is not null)
            {
                return;
            }

            // Определение собственных значений и собственных векторов матрицы
            var evd = _covariationMatrix.Evd();
            _eigenVectors = evd.EigenVectors;
            _eigenValues = evd.EigenValues;
        }

        #region Signal recovery

        /// <summary>
        ///     Вычисляет матрицу главных компонент
        /// </summary>
        internal void ComputeMainComponentsMatrix()
        {
            // Валидация исходных данных перед вычислением
            if (IsEigenVectorsAndValuesIsNull())
            {
                return;
            }

            // Защита от лишних вычислений
            if (MainComponentsMatrix is not null)
            {
                return;
            }

            // Переход к главным компонентам
            // Представление матрицы собственных векторов как матрицу перехода к главным компонентам
            var matrixEigenvectorsTranspose = _eigenVectors.Transpose();
            MainComponentsMatrix = matrixEigenvectorsTranspose * DelayMatrix;
        }

        /// <summary>
        ///     Вычисляет восстановленную оригинальную матрицу
        /// </summary>
        internal void ComputeRestoredOriginalMatrix()
        {
            // Валидация исходных данных перед вычислением
            if (IsEigenVectorsAndValuesIsNull() | IsMainComponentsIsNull())
            {
                return;
            }

            // Защита от лишних вычислений
            if (_restoredOriginalMatrix is not null && _restoredOriginalMatrix.Length != 0)
            {
                return;
            }

            _restoredOriginalMatrix = (_eigenVectors * MainComponentsMatrix).ToArray();
        }

        /// <summary>
        ///     Возвращает восстанавленный сигнал
        /// </summary>
        internal double[] GetRestoredSignal()
        {
            // Валидация исходных данных перед вычислением
            if (IsRestoredOriginalMatrixIsNull())
            {
                return Array.Empty<double>();
            }

            var result = new List<double>();
            var n = _pricesLength - TauDelayNumber + 1;
            for (var s = 1; s <= _pricesLength; s++)
            {
                if (s < TauDelayNumber)
                {
                    var item = 0d;
                    for (var i = 0; i < s; i++)
                    {
                        item += _restoredOriginalMatrix[i, s - i - 1];
                    }

                    result.Add(item / s);
                    continue;
                }

                if (s < n)
                {
                    var item = 0d;
                    for (var i = 0; i < TauDelayNumber; i++)
                    {
                        item += _restoredOriginalMatrix[i, s - i - 1];
                    }

                    result.Add(item / TauDelayNumber);
                    continue;
                }

                var lastItem = 0d;
                for (var i = 0; i < (_pricesLength - s + 1); i++)
                {
                    lastItem += _restoredOriginalMatrix[i + s - n, n - i - 1];
                }

                result.Add(lastItem / (_pricesLength - s + 1));
            }

            return result.ToArray();
        }

        #endregion

        #region Make Smoothing

        /// <summary>
        ///     Вычисляет значимые значения собственных чисел
        /// </summary>
        internal IEnumerable<Complex> ComputeNeededLambdas()
        {
            // Валидация исходных данных перед вычислением
            if (IsEigenVectorsAndValuesIsNull())
            {
                return new List<Complex>();
            }

            var eigenvaluesArray = _eigenValues.ToList();
            eigenvaluesArray.Sort((x, y) =>
            {
                return x.Magnitude < y.Magnitude ? 1 : x.Magnitude == y.Magnitude ? 0 : -1;
            });

            var neededLambdas = eigenvaluesArray.Where(_ => _.Magnitude > _minMagnitudeForSsaSmoothing);
            _neededLambdas = neededLambdas.Count() < _neededLambdaNumber ? eigenvaluesArray.Take(_neededLambdaNumber) : neededLambdas;

            return _neededLambdas;
        }

        /// <summary>
        ///     Возвращает список значимых векторов, для предсказаний <br/>
        ///     Зануляет столбцы собственных векторов, которые незначимы
        /// </summary>
        internal List<MathNet.Numerics.LinearAlgebra.Vector<double>> GetSubListEigenvectors()
        {
            // Валидация исходных данных перед вычислением
            var subEigenvectors = new List<MathNet.Numerics.LinearAlgebra.Vector<double>>(); // понадобится для предсказаний
            if (IsEigenVectorsAndValuesIsNull())
            {
                return subEigenvectors;
            }

            for (var i = 0; i < _eigenValues.Count; i++)
            {
                if (_neededLambdas.Contains(_eigenValues[i]))
                {
                    subEigenvectors.Add(_eigenVectors.Column(i));
                    continue;
                }

                // зануляем столбцы собственных векторов, которые незначимы (сглаживаем)
                _eigenVectors.ClearColumn(i);
            }

            return subEigenvectors;
        }

        #endregion

        #endregion

        #region Private methods

        /// <summary>
        ///     Проверяет значение <see cref="_covariationMatrix"/> на <see langword="null"/><br />
        ///     Логгирует сообщение об ошибке
        /// </summary>
        /// <returns>
        ///     <see langword="true" /> если <see cref="_covariationMatrix"/> не инициализирован
        /// </returns>
        private bool IsCovariationMatrixIsNull()
        {
            var isNull = _covariationMatrix is null;
            if (isNull)
            {
                Log.ErrorAsync($"Covariation Matrix is null. Need to call {nameof(ComputeCovariationMatrix)}");
            }

            return isNull;
        }

        /// <summary>
        ///     Проверяет значение <see cref="_eigenVectors"/> и <see cref="_eigenValues"/> на <see langword="null"/><br />
        ///     Логгирует сообщение об ошибке
        /// </summary>
        /// <returns>
        ///     <see langword="true" />, если <see cref="_eigenVectors"/> или <see cref="_eigenValues"/> не инициализирован
        /// </returns>
        private bool IsEigenVectorsAndValuesIsNull()
        {
            var isNull = _eigenVectors is null || _eigenValues is null;
            if (isNull)
            {
                Log.ErrorAsync($"Eigen vectors or values is null. Need to call {nameof(ComputeEigenVectorsAndValues)}");
            }

            return isNull;
        }

        /// <summary>
        ///     Проверяет значение <see cref="MainComponentsMatrix"/> на <see langword="null"/><br />
        ///     Логгирует сообщение об ошибке
        /// </summary>
        /// <returns>
        ///     <see langword="true" /> если <see cref="MainComponentsMatrix"/> не инициализирован
        /// </returns>
        private bool IsMainComponentsIsNull()
        {
            var isNull = MainComponentsMatrix is null;
            if (isNull)
            {
                Log.ErrorAsync($"Main Components Matrix is null. Need to call {nameof(ComputeMainComponentsMatrix)}");
            }

            return isNull;
        }

        /// <summary>
        ///     Проверяет значение <see cref="_restoredOriginalMatrix"/> на <see langword="null"/><br />
        ///     Логгирует сообщение об ошибке
        /// </summary>
        /// <returns>
        ///     <see langword="true" /> если <see cref="_restoredOriginalMatrix"/> не инициализирован
        /// </returns>
        private bool IsRestoredOriginalMatrixIsNull()
        {
            var isNull = _restoredOriginalMatrix is null || _restoredOriginalMatrix.Length == 0;
            if (isNull)
            {
                Log.ErrorAsync($"restored Original Matrix is null. Need to call {nameof(ComputeRestoredOriginalMatrix)}");
            }

            return isNull;
        }

        #endregion
    }
}
