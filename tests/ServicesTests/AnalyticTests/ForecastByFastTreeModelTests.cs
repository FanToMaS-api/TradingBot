using Analytic.AnalyticUnits.Profiles.ML.MapperProfiles;
using Analytic.AnalyticUnits.Profiles.ML.Models;
using Analytic.AnalyticUnits.Profiles.ML.Models.Impl;
using AutoMapper;
using Logger;
using Microsoft.ML;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AnalyticTests
{
    /// <summary>
    ///     Тестирует <see cref="ForecastByFastTreeModel"/>
    /// </summary>
    public class ForecastByFastTreeModelTests
    {
        private readonly ILoggerDecorator _logger = LoggerManager.CreateDefaultLogger();
        private readonly IMapper _mapper = new MapperConfiguration
            (mc => mc.AddProfile(new MlMapperProfile())).CreateMapper();

        #region Tests

        /// <summary>
        ///     Тестирует получение имен фич
        /// </summary>
        [Fact(DisplayName = "Get feature names Test")]
        public void GetFeatureNames_Test()
        {
            IEnumerable<IObjectForMachineLearning> data = new List<IObjectForMachineLearning>
            {
                new TradeObjectModel
                {
                    ClosePrice = 0.2F,
                },
                new TradeObjectModel
                {
                    ClosePrice = 0.15F,
                }
            };

            var context = new MLContext(1);
            var trainingDataView = context.Data.LoadFromEnumerable(data.Cast<TradeObjectModel>());
            var actual = ForecastByFastTreeModel.GetFeatureNames(trainingDataView);

            var expected = new[]
            {
                "ClosePrice",
                "OpenPrice",
                "MinPrice",
                "MaxPrice",
                "BasePurchaseVolume",
                "QuotePurchaseVolume"
            };

            for (var i = 0; i < expected.Length; i++)
            {
                Assert.Equal(expected[i], actual[i]);
            }
        }

        /// <summary>
        ///     Тестирует прогнозирование цены
        /// </summary>
        [Fact(DisplayName = "Forecast Test")]
        public void Forecast_Test()
        {
            var enitites = MockHelper.GetMiniTickerEntities("Files/EntitiesForTest_2000.txt");
            var data = _mapper.Map<IEnumerable<TradeObjectModel>>(enitites);
            var model = new ForecastByFastTreeModel(_logger, 1);
            var predictions = model.Forecast(data);
            Assert.Equal(80, predictions.Length);
        }

        #endregion
    }
}
