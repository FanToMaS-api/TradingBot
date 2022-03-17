﻿namespace Common.Models
{
    /// <summary>
    ///     Базовый класс моделей получаемых со стримов маркетдаты
    /// </summary>
    public class MarketdataStreamModelBase
    {
        /// <summary>
        ///     Время события
        /// </summary>
        public long EventTimeUnix { get; set; }

        /// <summary>
        ///     Пара тикеров
        /// </summary>
        public string Symbol { get; set; }
    }
}