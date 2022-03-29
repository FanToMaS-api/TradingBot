namespace Common.Models
{
    /// <summary>
    ///     Базовый класс моделей получаемых со стримов маркетдаты
    /// </summary>
    public class MarketdataStreamModelBase
    {
        /// <summary>
        ///     Время события
        /// </summary>
        public long EventTimeUnix { get; internal set; }

        /// <summary>
        ///     Краткое наименование объекта торговли
        /// </summary>
        public string ShortName { get; internal set; }
    }
}
