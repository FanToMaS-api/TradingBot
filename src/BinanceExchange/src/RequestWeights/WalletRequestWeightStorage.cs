using BinanceExchange.Models;

namespace BinanceExchange.RequestWeights
{
    /// <summary>
    ///     Веса запросов к конечным точкам кошелька
    /// </summary>
    internal sealed class WalletRequestWeightStorage
    {
        /// <summary>
        ///     Вес запроса статуса системы
        /// </summary>
        public RequestWeightModel SystemStatusWeight { get; } = new RequestWeightModel(
            Enums.ApiType.Sapi,
            new()
            {
                { RequestWeightModel.GetDefaultKey(), 1 }
            });

        /// <summary>
        ///     Вес запроса статуса аккаунта
        /// </summary>
        public RequestWeightModel AccountStatusWeight { get; } = new RequestWeightModel(
            Enums.ApiType.Sapi,
            new()
            {
                { RequestWeightModel.GetDefaultKey(), 1 }
            });

        /// <summary>
        ///     Вес запроса информации обо всех монетах
        /// </summary>
        public RequestWeightModel AllCoinsInfoWeight { get; } = new RequestWeightModel(
            Enums.ApiType.Sapi,
            new()
            {
                { RequestWeightModel.GetDefaultKey(), 10 }
            });

        /// <summary>
        ///     Вес запроса таксы за торговлю определенными монетами
        /// </summary>
        public RequestWeightModel TradeFeeWeight { get; } = new RequestWeightModel(
            Enums.ApiType.Sapi,
            new()
            {
                { RequestWeightModel.GetDefaultKey(), 1 }
            });
    }
}
