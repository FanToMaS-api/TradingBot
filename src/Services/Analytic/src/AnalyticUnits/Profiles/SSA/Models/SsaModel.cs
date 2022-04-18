using MathNet.Numerics.LinearAlgebra;
using System.Numerics;

namespace Analytic.AnalyticUnits.Profiles.SSA.Models
{
    /// <summary>
    ///     Модель для удобной работы алгоритма SSA
    /// </summary>
    internal class SsaModel
    {
        #region Fields

        private Matrix<double> _covariationMatrix;
        private Matrix<double> _mainComponentsMatrix;
        private Matrix<double> _eigenVectors;
        private MathNet.Numerics.LinearAlgebra.Vector<Complex> _eigenValues;

        #endregion

        #region .ctor

        /// <inheritdoc cref="SsaModel"/>
        private SsaModel(int tauDelayNumber, int delayVectorsNumber, Matrix<double> delayMatrix)
        {
            TauDelayNumber = tauDelayNumber;
            DelayVectorsNumber = delayVectorsNumber;
            DelayMatrix = delayMatrix;
        }

        /// <inheritdoc cref="SsaModel"/>
        public static SsaModel Create(double[] prices)
        {
            // преобразование одномерного ряда в многомерный
            var tauDelayNumber = (prices.Length + 1) / 2;
            var delayVectorsNumber = prices.Length - tauDelayNumber + 1;
            var rectangleMatrix = Matrix<double>.Build.Dense(tauDelayNumber, delayVectorsNumber);
            for (var i = 0; i < tauDelayNumber; i++)
            {
                for (var j = 0; j < delayVectorsNumber; j++)
                {
                    rectangleMatrix[i, j] = prices[i + j];
                }
            }

            return new SsaModel(tauDelayNumber, delayVectorsNumber, rectangleMatrix);
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
        ///     Ковариационная матрица
        /// </summary>
        public Matrix<double> CovariationMatrix => _covariationMatrix ??= GetCovariationMatrix();

        /// <summary>
        ///     Матрица главных компонент
        /// </summary>
        public Matrix<double> MainComponentsMatrix => _mainComponentsMatrix ??= GetMainComponentsMatrix();

        #endregion

        #region Public methods

        /// <summary>
        ///     Возвращает матрицу собственных векторов и вектор собственных значений
        /// </summary>
        /// <remarks>
        ///     При повторном вызове возвращает уже посчитанные значения
        /// </remarks>
        public (Matrix<double> eigenVectors, MathNet.Numerics.LinearAlgebra.Vector<Complex> eigenValues)
            GetEigenVectorsAndValues()
        {
            if (_eigenVectors is null || _eigenValues is null)
            {
                // Определение собственных значений и собственных векторов матрицы
                var evd = CovariationMatrix.Evd();
                _eigenVectors = evd.EigenVectors;
                _eigenValues = evd.EigenValues;
            }

            return (_eigenVectors, _eigenValues);
        }

        /// <summary>
        ///     Возвращает матрицу главных компонент
        /// </summary>
        public Matrix<double> GetMainComponentsMatrix()
        {
            // Переход к главным компонентам
            // Представление матрицы собственных векторов как матрицу перехода к главным компонентам
            var (matrixEigenvectors, matrixEigenvalues) = GetEigenVectorsAndValues();
            var matrixEigenvectorsTranspose = matrixEigenvectors.Transpose();
            var mainComponents = matrixEigenvectorsTranspose * DelayMatrix;

            return mainComponents;
        }

        #endregion

        #region Private methods

        /// <summary>
        ///     Возвращает ковариационную матрицу
        /// </summary>
        private Matrix<double> GetCovariationMatrix()
        {
            var multiplicationMatrix = DelayMatrix.TransposeAndMultiply(DelayMatrix);
            var covariationMatrix = 1 / (double)DelayVectorsNumber * multiplicationMatrix;

            return covariationMatrix;
        }

        #endregion
    }
}
