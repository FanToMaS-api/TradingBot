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
        public RequestWeight SistemStatusWeight { get; set; } = new RequestWeight(
            Enums.ApiType.Sapi,
            new() 
            {
                { RequestWeight.GetDefaultKey(), 1 } 
            });

        /// <summary>
        ///     Вес запроса статуса аккаунта
        /// </summary>
        public RequestWeight AccountStatusWeight { get; set; } = new RequestWeight(
            Enums.ApiType.Sapi,
            new()
            {
                { RequestWeight.GetDefaultKey(), 1 }
            });

        /// <summary>
        ///     Вес запроса информации обо всех монетах
        /// </summary>
        public RequestWeight AllCoinsInfoWeight { get; set; } = new RequestWeight(
            Enums.ApiType.Sapi,
            new()
            {
                { RequestWeight.GetDefaultKey(), 10 }
            });

        /// <summary>
        ///     Вес запроса таксы за торговлю определенными монетами
        /// </summary>
        public RequestWeight TradeFeeWeight { get; set; } = new RequestWeight(
            Enums.ApiType.Sapi,
            new()
            {
                { RequestWeight.GetDefaultKey(), 1 }
            });

        #endregion

        #region Marketdata requests weight



        #endregion
    }
}
