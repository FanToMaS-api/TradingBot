using ExchangeLibrary.Binance.Enums;

namespace ExchangeLibrary.Binance.Models
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
