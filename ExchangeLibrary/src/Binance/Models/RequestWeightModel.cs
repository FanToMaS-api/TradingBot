using ExchangeLibrary.Binance.Enums;
using System.Collections.Generic;

namespace ExchangeLibrary.Binance.Models
{
    /// <summary>
    ///     Модель веса запроса
    /// </summary>
    internal class RequestWeightModel
    {
        private static string _defaultKey = "default";

        #region .ctor

        /// <inheritdoc cref="RequestWeightModel"/>
        public RequestWeightModel(ApiType type, Dictionary<string, long> weights)
        {
            Type = type;
            Weights = weights;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Тип ограничения скорости
        /// </summary>
        public ApiType Type { get; set; }

        /// <summary>
        ///     Словарь с весами запросов в зависимости от string параметров
        /// </summary>
        /// <remarks>
        ///     Если у запроса фиксированный вес, то в <code>string = "default"</code> 
        /// </remarks>
        public Dictionary<string, long> Weights { get; set; }

        #endregion
        
        /// <summary>
        ///     Возвращает дефолтный ключ
        /// </summary>
        public static string GetDefaultKey() => _defaultKey;
    }
}
