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
        public string Symbol { get; internal set; }

        /// <summary>
        ///     Статус пары
        /// </summary>
        public string Status { get; internal set; }

        /// <summary>
        ///     Базовая валюта
        /// </summary>
        public string BaseAsset { get; internal set; }

        /// <summary>
        ///     Требуемое количество символов базовой валюты после запятой при создании ордера (для цены и количества)
        /// </summary>
        public int BaseAssetPrecision { get; internal set; }

        /// <summary>
        ///     Квотируемая валюта
        /// </summary>
        public string QuoteAsset { get; internal set; }

        /// <summary>
        ///     Требуемое количество символов квотируемой валюты после запятой при создании ордера (для цены и количества)
        /// </summary>
        public int QuotePrecision { get; internal set; }

        /// <summary>
        ///     Разрешено ли создание айсбергов
        /// </summary>
        public bool IsIcebergAllowed { get; internal set; }

        /// <summary>
        ///     Разрешено ли создание OCO-ордеров
        /// </summary>
        public bool IsOcoAllowed { get; internal set; }
    }
}
