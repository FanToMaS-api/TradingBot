using System.Collections.Generic;

namespace BinanceExchange.Models
{
    /// <summary>
    ///     Информация о правилах текущей торговли парами
    /// </summary>
    internal class ExchangeInfoModel
    {
        /// <summary>
        ///     Правила торговли для символа
        /// </summary>
        public List<SymbolInfoModel> Symbols { get; set; } = new();
    }
}
