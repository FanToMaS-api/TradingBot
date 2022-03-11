using BinanceExchange.Enums;

namespace BinanceExchange.Models
{
    /// <summary>
    ///     Представляет собой объект данных со стримов маркетдаты
    /// </summary>
    internal interface IMarketdataStreamModel
    {
        /// <summary>
        ///     Тип стрима с которого получаем данные
        /// </summary>
        public MarketdataStreamType StreamType { get; }
    }
}
