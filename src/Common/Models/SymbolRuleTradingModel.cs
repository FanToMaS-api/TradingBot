namespace Common.Models
{
    /// <summary>
    ///     Содержит правила торговли для пары
    /// </summary>
    public class SymbolRuleTradingModel
    {
        /// <summary>
        ///     Пара
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        ///     Базовая валюта
        /// </summary>
        public string BaseAsset { get; set; }

        /// <summary>
        ///     Требуемое количество символов базовой валюты после запятой при создании ордера (для цены и количества)
        /// </summary>
        public int BaseAssetPrecision { get; set; }

        /// <summary>
        ///     Квотируемая валюта
        /// </summary>
        public string QuoteAsset { get; set; }

        /// <summary>
        ///     Требуемое количество символов квотируемой валюты после запятой при создании ордера (для цены и количества)
        /// </summary>
        public int QuotePrecision { get; set; }

        /// <summary>
        ///     Разрешено ли создание айсбергов
        /// </summary>
        public bool IsIcebergAllowed { get; set; }

        /// <summary>
        ///     Разрешено ли создание OCO-ордеров
        /// </summary>
        public bool IsOcoAllowed { get; set; }
    }
}
