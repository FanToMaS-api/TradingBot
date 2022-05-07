using Analytic.Filters;
using Analytic.Models;
using Xunit;

namespace AnalyticTests
{
    /// <summary>
    ///     Тестирует класс <see cref="VolumeFilter"/>
    /// </summary>
    public class VolumeFilterTests
    {
        #region Fields

        private readonly InfoModel _bidGreaterAskModel = new("BidGreaterAsk", 15)
        {
            AskVolume = 4,
            BidVolume = 10,
        };

        private readonly InfoModel _askGreaterBidModel = new("AskGreaterBid", 15)
        {
            AskVolume = 10,
            BidVolume = 5,
        };

        #endregion

        /// <summary>
        ///     Тест фильтрации: спрос больше предложения
        /// </summary>
        [Fact(DisplayName = "Bid greater than ask Test")]
        public void BidGreaterThanAsk_Test()
        {
            var filter = new VolumeFilter(
                "BidGreaterThanAsk",
                VolumeType.Bid,
                VolumeComparisonType.GreaterThan,
                0.5);

            Assert.True(filter.CheckConditions(_bidGreaterAskModel));
            Assert.False(filter.CheckConditions(_askGreaterBidModel));
        }

        /// <summary>
        ///     Тест фильтрации: спрос меньше предложения
        /// </summary>
        [Fact(DisplayName = "Bid less than ask Test")]
        public void BidLessThanAsk_Test()
        {
            var filter = new VolumeFilter(
                "BidLessThanAsk",
                VolumeType.Bid,
                VolumeComparisonType.LessThan,
                0.5);

            Assert.False(filter.CheckConditions(_bidGreaterAskModel));
            Assert.True(filter.CheckConditions(_askGreaterBidModel));
        }

        /// <summary>
        ///     Тест фильтрации: предложение больше спроса
        /// </summary>
        [Fact(DisplayName = "Ask greater than bid Test")]
        public void AskGreaterThanBid_Test()
        {
            var filter = new VolumeFilter(
                "AskGreaterThanBid",
                VolumeType.Ask,
                VolumeComparisonType.GreaterThan,
                0.5);

            Assert.False(filter.CheckConditions(_bidGreaterAskModel));
            Assert.True(filter.CheckConditions(_askGreaterBidModel));
        }

        /// <summary>
        ///     Тест фильтрации: предложение меньше спроса
        /// </summary>
        [Fact(DisplayName = "Ask less than bid Test")]
        public void AskLessThanBid_Test()
        {
            var filter = new VolumeFilter(
                "AskLessThanBid",
                VolumeType.Ask,
                VolumeComparisonType.LessThan,
                0.5);

            Assert.True(filter.CheckConditions(_bidGreaterAskModel));
            Assert.False(filter.CheckConditions(_askGreaterBidModel));
        }
    }
}
