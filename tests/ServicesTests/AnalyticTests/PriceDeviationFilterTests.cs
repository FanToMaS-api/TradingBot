using Analytic.Filters;
using Analytic.Models;
using System.Collections.Generic;
using Xunit;

namespace AnalyticTests
{
    /// <summary>
    ///     Тестирует <see cref="PriceDeviationFilter"/>
    /// </summary>
    public class PriceDeviationFilterTests
    {
        #region Fields

        public static readonly IEnumerable<object[]> InfoModelsMemberData = new List<object[]>
        {
            new object[] 
            {
                CreateModelWithNeededDeviation(60), CreatePriceDeviationFilter(ComparisonType.GreaterThan, 10, 60), 18.299999999999997, true
            },
            new object[] 
            {
                CreateModelWithNeededDeviation(60), CreatePriceDeviationFilter(ComparisonType.GreaterThan, 20, 60), 18.299999999999997, false
            },
            new object[] 
            {
                CreateModelWithNeededDeviation(5), CreatePriceDeviationFilter(ComparisonType.LessThan, 10, 6), 0.15000000000000002, true
            },
            new object[] 
            {
                CreateModelWithNeededDeviation(10), CreatePriceDeviationFilter(ComparisonType.LessThan, 0.2, 6), 0.44999999999999996, false
            },
            new object[] 
            {
                CreateModelWithNeededDeviation(10), CreatePriceDeviationFilter(ComparisonType.Equal, 0.44999999999999996, 6), 0.44999999999999996, true
            },
            new object[] 
            {
                CreateModelWithNeededDeviation(10), CreatePriceDeviationFilter(ComparisonType.Equal, 0.44999999999999996, 7), 0.49, false
            },
            new object[] 
            {
                CreateModelWithNeededDeviation(5), CreatePriceDeviationFilter(ComparisonType.Equal, 0.44999999999999996, 7), 0.15000000000000002, false
            },
        };

        #endregion

        /// <summary>
        ///     Тест фильтрации по отклонению цены за определенных промежуток времени
        /// </summary>
        [Theory(DisplayName = "Filter by price deviation Test")]
        [MemberData(nameof(InfoModelsMemberData))]
        public void FilterByPriceDeviation_Test(InfoModel infoModel, PriceDeviationFilter filter, double expectedSum, bool expectedFilterResult)
        {
            var actualFilterResult = filter.CheckConditions(infoModel);

            Assert.Equal(expectedFilterResult, actualFilterResult);
            Assert.Equal(expectedSum, infoModel.DeviationsSum);
        }

        #region Private methods

        /// <summary>
        ///     Создает модель с требуемым кол-вом отклонений цены и суммарным отклонением цены
        /// </summary>
        /// <param name="deviationsNumber"> Нужное кол-во отклонений цены в модели </param>
        private static InfoModel CreateModelWithNeededDeviation(int deviationsNumber)
        {
            var infoModel = new InfoModel("_", 0);
            for (var i = 0; i < deviationsNumber; i++)
            {
                infoModel.PricePercentDeviations.Add((i + 1) / (double)100);
            }

            return infoModel;
        }

        /// <summary>
        ///     Создает модель фильтра с заданными параметрами
        /// </summary>
        /// <param name="comparisonType"> Тип сравнения </param>
        /// <param name="limit"> Ограничение </param>
        /// <param name="timeframeNumber"> Кол-во таймфреймов участвующих в анализе </param>
        private static PriceDeviationFilter CreatePriceDeviationFilter(
            ComparisonType comparisonType,
            double limit,
            int timeframeNumber)
            => new(
                "PriceDeviationFilter",
                comparisonType,
                limit: limit,
                timeframeNumber: timeframeNumber);

        #endregion
    }
}
