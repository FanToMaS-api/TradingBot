using Jose;
using System;

namespace ExchangeLibrary.Binance.Models
{
    /// <summary>
    ///     Содержит базовые параметры запроса
    /// </summary>
    public abstract class RequestModelBase
    {
        #region Properties

        /// <summary>
        ///     Отметка времени в миллисекундах, когда запрос был создан и отправлен
        /// </summary>
        /// <remarks>
        ///     Необходим только для SIGNED запросов
        /// </remarks>
        protected virtual TimeSpan TimeStamp { get; set; }

        /// <summary>
        ///     Кол-во миллисекунд, в течении которых запрос действителен
        /// </summary>
        protected virtual TimeSpan RecvWindow { get; set; } = TimeSpan.FromMilliseconds(7000);

        /// <summary>
        ///     Ключ 
        /// </summary>
        protected virtual string ApiKey { get; set; }

        /// <summary>
        ///     Секретный ключ
        /// </summary>
        protected virtual string SecretKey { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Возвращает тело запроса с необходимыми параметрами
        /// </summary>
        protected virtual string CreateUrlBodyWithParams()
        {
            return "";
        }

        /// <summary>
        ///     Подписывает токен
        /// </summary>
        public string SignToken() => JWT.Encode(ApiKey, SecretKey, JwsAlgorithm.HS256);

        #endregion
    }
}
