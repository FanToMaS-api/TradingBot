using System.Collections.Generic;
using System.Net.Http;

namespace BinanceExchange.Client.Http.Request.Models
{
    /// <summary>
    ///     Модель запроса
    /// </summary>
    internal interface IRequestModel
    {
        #region Properties

        /// <summary>
        ///     Полный путь до конечной точки без параметров
        /// </summary>
        string Endpoint { get; }

        /// <summary>
        ///     Урл запроса с параметрами
        /// </summary>
        string Url { get; }

        /// <summary>
        ///     Метод http запроса
        /// </summary>
        HttpMethod HttpMethod { get; }

        /// <summary>
        ///     Тип контента
        /// </summary>
        string ContentType { get; }

        /// <summary>
        ///     Тело запроса
        /// </summary>
        byte[] Body { get; }

        /// <summary>
        ///     Параметры запроса
        /// </summary>
        IReadOnlyDictionary<string, string> Parameters { get; }

        #endregion
    }
}
