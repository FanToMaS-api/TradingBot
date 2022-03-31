using BinanceExchange.Models;

namespace BinanceExchange.RequestWeights
{
    /// <summary>
    ///     Веса запросов к конечным точкам спот торговли
    /// </summary>
    internal class SpotTradeWeightStorage
    {
        /// <summary>
        ///     Вес запроса на создание нового ордера (включая тестовый)
        /// </summary>
        public RequestWeightModel NewOrderWeight { get; } = new RequestWeightModel(
            Enums.ApiType.Api,
            new()
            {
                { RequestWeightModel.GetDefaultKey(), 1 }
            });

        /// <summary>
        ///     Вес запроса на отмену активного ордера по паре (включая отмену всех оредров по паре)
        /// </summary>
        public RequestWeightModel CancelOrderWeight { get; } = new RequestWeightModel(
            Enums.ApiType.Api,
            new()
            {
                { RequestWeightModel.GetDefaultKey(), 1 }
            });

        /// <summary>
        ///     Вес запроса на проверку состояния ордера по паре
        /// </summary>
        public RequestWeightModel CheckOrderWeight { get; } = new RequestWeightModel(
            Enums.ApiType.Api,
            new()
            {
                { RequestWeightModel.GetDefaultKey(), 2 }
            });

        /// <summary>
        ///     Вес запроса на проверку состояния всех открытых ордеров или только по паре
        /// </summary>
        public RequestWeightModel CheckAllOpenOrdersWeight { get; } = new RequestWeightModel(
            Enums.ApiType.Api,
            new()
            {
                { RequestWeightModel.GetDefaultKey(), 3 },
                { "null", 40 }
            });

        /// <summary>
        ///     Вес запроса на получение всех ордеров аккаунта по паре
        /// </summary>
        public RequestWeightModel GetAllOrdersWeight { get; } = new RequestWeightModel(
            Enums.ApiType.Api,
            new()
            {
                { RequestWeightModel.GetDefaultKey(), 10 }
            });

        /// <summary>
        ///     Вес запроса на получение информации об аккаунте
        /// </summary>
        public RequestWeightModel AccountInformationWeight { get; } = new RequestWeightModel(
            Enums.ApiType.Api,
            new()
            {
                { RequestWeightModel.GetDefaultKey(), 10 }
            });
    }
}
