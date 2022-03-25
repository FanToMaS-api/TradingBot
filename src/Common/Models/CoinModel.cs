namespace Common.Models
{
    /// <summary>
    ///     Модель объекта торговли
    /// </summary>
    public class CoinModel
    {
        /// <summary>
        ///     Обозначение монеты
        /// </summary>
        public string Coin { get; internal set; }

        /// <summary>
        ///     Название валюты
        /// </summary>
        public string Name { get; internal set; }
    }
}
