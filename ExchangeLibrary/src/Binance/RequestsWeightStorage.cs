using ExchangeLibrary.Binance.Models;

namespace ExchangeLibrary.Binance
{
    /// <summary>
    ///     Хранилище весов запросов для Binance
    /// </summary>
    internal class RequestsWeightStorage
    {
        #region Wallet's requests weight

        /// <summary>
        ///     Вес запроса статуса системы
        /// </summary>
        public RequestWeightModel SistemStatusWeight { get; set; } = new RequestWeightModel(
            Enums.ApiType.Sapi,
            new()
            {
                { RequestWeightModel.GetDefaultKey(), 1 }
            });

        /// <summary>
        ///     Вес запроса статуса аккаунта
        /// </summary>
        public RequestWeightModel AccountStatusWeight { get; set; } = new RequestWeightModel(
            Enums.ApiType.Sapi,
            new()
            {
                { RequestWeightModel.GetDefaultKey(), 1 }
            });

        /// <summary>
        ///     Вес запроса информации обо всех монетах
        /// </summary>
        public RequestWeightModel AllCoinsInfoWeight { get; set; } = new RequestWeightModel(
            Enums.ApiType.Sapi,
            new()
            {
                { RequestWeightModel.GetDefaultKey(), 10 }
            });

        /// <summary>
        ///     Вес запроса таксы за торговлю определенными монетами
        /// </summary>
        public RequestWeightModel TradeFeeWeight { get; set; } = new RequestWeightModel(
            Enums.ApiType.Sapi,
            new()
            {
                { RequestWeightModel.GetDefaultKey(), 1 }
            });

        #endregion

        #region Marketdata requests weight

        /// <summary>
        ///     Вес запроса книги ордеров
        /// </summary>
        public RequestWeightModel OrderBookWeight { get; set; } = new RequestWeightModel(
            Enums.ApiType.Api,
            new()
            {
                { "5", 1 },
                { "10", 1 },
                { "20", 1 },
                { "50", 1 },
                { "100", 1 },
                { "500", 5 },
                { "1000", 10 },
                { "5000", 50 },
            });

        /// <summary>
        ///     Вес запроса списка недавних сделок
        /// </summary>
        public RequestWeightModel RecentTradesWeight { get; set; } = new RequestWeightModel(
            Enums.ApiType.Api,
            new()
            {
                { RequestWeightModel.GetDefaultKey(), 1 }
            });

        #endregion
    }
}
