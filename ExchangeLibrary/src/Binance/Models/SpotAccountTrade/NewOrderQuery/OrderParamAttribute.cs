using System;

namespace ExchangeLibrary.Binance.Models.SpotAccountTrade.NewOrderQuery
{
    /// <summary>
    ///     Атрибут, для параметров запроса
    /// </summary>
    internal class OrderParamAttribute : Attribute
    {
        /// <summary>
        ///     Обозначение св-ва в запросе
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        ///     Показывает нужен ли параметр в текущем запросе
        /// </summary>
        public bool IsUse { get; set; }

        /// <summary>
        ///     Показывает можем ли создавать этот параметр
        ///     (Для определенных типов запросов некоторые параметры - лишние)
        /// </summary>
        public bool CanSet { get; set; }

        /// <summary>
        ///     Значение в запросе
        /// </summary>
        public string Value { get; set; }

        /// <<inheritdoc cref="OrderParamAttribute"/>
        /// <param name="url"> Обозначение св-ва в запросе </param>
        /// <param name="isUse"> Показывает нужен ли параметр в текущем запросе </param>
        /// <param name="canSet"> Показывает можем ли создавать этот параметр </param>
        public OrderParamAttribute(string url, bool isUse, bool canSet)
        {
            Url = url;
            IsUse = isUse;
            CanSet = canSet;
        }
    }
}
