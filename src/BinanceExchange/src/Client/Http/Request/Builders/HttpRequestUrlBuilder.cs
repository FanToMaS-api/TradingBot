using BinanceExchange.Client.Helpers;
using BinanceExchange.Client.Http.Request.Models;
using BinanceExchange.Client.Http.Request.Models.Impl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;

namespace BinanceExchange.Client.Http.Request.Builders
{
    /// <summary>
    ///     Строитель запросов
    /// </summary>
    internal class HttpRequestUrlBuilder
    {
        #region Fields

        /// <summary>
        ///     Базовый адрес биржи
        /// </summary>
        private static readonly string BaseUrl = "https://api.binance.com";
        private RequestModel _requestModel = new();

        /// <summary>
        ///     Параметры запроса
        /// </summary>
        private IDictionary<string, string> _parameters = new Dictionary<string, string>();

        #endregion

        #region Public methods

        /// <summary>
        ///     Установить конечную точку запроса
        /// </summary>
        /// <param name="endpoint"> Конечная точка запроса </param>
        /// <remarks>
        ///     Также сразу устанавливает <see cref="RequestModel.Url"/>
        /// </remarks>
        public HttpRequestUrlBuilder SetEndpoint(string endpoint)
        {
            ThrowIsEmpty(nameof(endpoint), endpoint);

            var endpointUrl = $"{BaseUrl}{endpoint}";
            _requestModel.Endpoint = endpointUrl;
            _requestModel.Url = endpointUrl;
            
            return this;
        }

        /// <summary>
        ///     Установить Http метод
        /// </summary>
        /// <param name="httpMethod"> Http метод </param>
        public HttpRequestUrlBuilder SetHttpMethod(HttpMethod httpMethod)
        {
            _requestModel.HttpMethod = httpMethod;

            return this;
        }

        /// <summary>
        ///     Установить тип контента
        /// </summary>
        /// <param name="contentType"> Тип контента </param>
        public HttpRequestUrlBuilder SetContentType(string contentType)
        {
            ThrowIsEmpty(nameof(contentType), contentType);

            _requestModel.ContentType = contentType;

            return this;
        }

        /// <summary>
        ///     Установить тело запроса
        /// </summary>
        /// <param name="body"> Тело запроса </param>
        /// <remarks>
        ///     Также сразу устанавливает тип контента на "application/json"
        /// </remarks>
        public HttpRequestUrlBuilder SetBody(IDictionary<string, string> body)
        {
            var parametersStr = GetCustomParametersString(body);
            _requestModel.Body = Encoding.ASCII.GetBytes(parametersStr);
            SetContentType("application/json");

            return this;
        }

        /// <summary>
        ///     Установить конечную точку запроса с инлайн параметрами, если нужны
        /// </summary>
        /// <param name="endpointWithInlineParams"> Конечная точка с инлайн параметрами </param>
        public HttpRequestUrlBuilder SetUrl(string endpointWithInlineParams)
        {
            ThrowIsEmpty(nameof(endpointWithInlineParams), endpointWithInlineParams);

            _requestModel.Url = $"{BaseUrl}{endpointWithInlineParams}";

            return this;
        }

        /// <summary>
        ///     Установить параметры запроса
        /// </summary>
        public HttpRequestUrlBuilder SetParameters(IDictionary<string, string> parameters)
        {
            _parameters = parameters;

            return this;
        }

        /// <summary>
        ///     Добавить параметр к запросу
        /// </summary>
        /// <param name="key"> Название параметра </param>
        /// <param name="value"> Значение параметра </param>
        /// <remarks>
        ///     Значение параметра не кодируется
        /// </remarks>
        public HttpRequestUrlBuilder AddParameter(string key, string value)
        {
            ThrowIsEmpty(nameof(key), key);
            ThrowIsEmpty(nameof(value), value);

            _parameters.Add(key, value);

            return this;
        }

        /// <summary>
        ///     Удалить параметр из запроса
        /// </summary>
        /// <param name="key"> Название параметра </param>
        public HttpRequestUrlBuilder RemoveParameter(string key)
        {
            ThrowIsEmpty(nameof(key), key);

            if (_parameters.ContainsKey(key))
            {
                _parameters.Remove(key);
            }

            return this;
        }

        /// <summary>
        ///     Сбросить все параметры строителя
        /// </summary>
        public HttpRequestUrlBuilder Reset()
        {
            _requestModel = new();
            _parameters.Clear();

            return this;
        }

        /// <summary>
        ///     Получить модель запроса
        /// </summary>
        public IRequestModel GetResult()
        {
            _requestModel.Url = GetRequestUrl(_requestModel.Url, _parameters);
            
            _requestModel.Parameters = new ReadOnlyDictionary<string, string>(_parameters);

            return _requestModel;
        }

        #endregion

        #region Private methods

        /// <summary>
        ///     Проверят переданный параметр на пустоту, выбрасывает исключение в случае если он неинициализирован
        /// </summary>
        /// <param name="paramName"> Название параметра </param>
        /// <param name="value"> Значение </param>
        /// <exception cref="ArgumentNullException"> Если параметр пуст или не инициализирован </exception>
        private static void ThrowIsEmpty(string paramName, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException($"{paramName} can not be empty");
            }
        }

        /// <summary>
        ///     Возвращает полный урл запроса со всеми параметрами
        /// </summary>
        /// <param name="requestUri"> Адрес запроса (с инлайн параметрами) </param>
        /// <param name="customParameters"> Параметры запроса </param>
        private static string GetRequestUrl(string requestUri, IDictionary<string, string> customParameters)
            => string.Format(
                "{0}?{1}",
                requestUri,
                GetCustomParametersString(customParameters));

        /// <summary>
        ///     Получить строковое представление тела запроса <br />
        ///     Кодирует параметры запроса
        /// </summary>
        private static string GetCustomParametersString(IDictionary<string, string> body)
            => body
                .Select(pair => string.Format("{0}={1}", pair.Key, HttpUtility.UrlEncode(pair.Value)))
                .JoinToString("&");

        #endregion
    }
}
