using System;

namespace ExchangeLibrary.Binance.Models
{
    /// <summary>
    ///     Оболочка для параметров запроса
    /// </summary>
    internal class OrderParamWrapper
    {
        /// <summary>
        ///     Обозначение св-ва в запросе
        /// </summary>
        public string Url { get; }

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
        public string ValueStr { get; set; }

        /// <<inheritdoc cref="OrderParamWrapper"/>
        /// <param name="url"> Обозначение св-ва в запросе </param>
        /// <param name="isUse"> Показывает нужен ли параметр в текущем запросе </param>
        /// <param name="canSet"> Показывает можем ли создавать этот параметр </param>
        public OrderParamWrapper(string url, bool isUse, bool canSet)
        {
            Url = url;
            IsUse = isUse;
            CanSet = canSet;
        }
    }
}
