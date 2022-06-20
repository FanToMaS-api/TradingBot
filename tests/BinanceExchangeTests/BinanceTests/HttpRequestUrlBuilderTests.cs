using BinanceExchange.Client.Helpers;
using BinanceExchange.Client.Http.Request.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using Xunit;

namespace BinanceExchangeTests.BinanceTests
{
    /// <summary>
    ///     Класс, тестирующий <see cref="HttpRequestUrlBuilder"/>
    /// </summary>
    public class HttpRequestUrlBuilderTests
    {
        #region Fields

        private static readonly string BaseUrl = "https://api.binance.com";

        #endregion

        #region Tests

        /// <summary>
        ///     Тест установки конечной точки запроса и урла
        /// </summary>
        [Fact(DisplayName = "RequestUrlBuilder: Set endpoint Test")]
        public void SetEndpoint_Test()
        {
            var builder = new HttpRequestUrlBuilder();
            var endpoint = "/endpoint";
            var expectedUrl = $"{BaseUrl}{endpoint}?";
            var expectedEndpoint = $"{BaseUrl}{endpoint}";

            var requestModel = builder.SetEndpoint(endpoint)
                .GetResult();

            Assert.Equal(expectedUrl, requestModel.Url);
            Assert.Equal(expectedEndpoint, requestModel.Endpoint);
        }

        /// <summary>
        ///     Тест выброса ошибки при попытке установить пустой или неинициализированной конечной точки
        /// </summary>
        [Fact(DisplayName = "RequestUrlBuilder: Set empty or null endpoint Test")]
        public void SetEmptyOrNullEndpoint_Test()
        {
            var builder = new HttpRequestUrlBuilder();
            var emptyEndpoint = "";
            string nullEndpoint = null;

            Assert.Throws<ArgumentNullException>(() => builder.SetEndpoint(emptyEndpoint));
            Assert.Throws<ArgumentNullException>(() => builder.SetEndpoint(nullEndpoint));
        }

        /// <summary>
        ///     Тест установки урла
        /// </summary>
        [Fact(DisplayName = "RequestUrlBuilder: Set url Test")]
        public void SetUrl_Test()
        {
            var builder = new HttpRequestUrlBuilder();
            var url = "/endpoint/23/address";
            var expectedUrl = $"{BaseUrl}{url}?";

            var requestModel = builder.SetUrl(url)
                .GetResult();

            Assert.Equal(expectedUrl, requestModel.Url);
        }

        /// <summary>
        ///     Тест выброса ошибки при попытке установить пустой или неинициализированный урл
        /// </summary>
        [Fact(DisplayName = "RequestUrlBuilder: Set empty or null url Test")]
        public void SetEmptyOrNullUrl_Test()
        {
            var builder = new HttpRequestUrlBuilder();
            var emptyUrl = "";
            string nullUrl = null;

            Assert.Throws<ArgumentNullException>(() => builder.SetUrl(emptyUrl));
            Assert.Throws<ArgumentNullException>(() => builder.SetUrl(nullUrl));
            Assert.Throws<ArgumentNullException>(() => builder.GetResult());
        }

        /// <summary>
        ///     Тест установки http-метода
        /// </summary>
        [Fact(DisplayName = "RequestUrlBuilder: Set http method Test")]
        public void SetHttpMethod_Test()
        {
            var builder = new HttpRequestUrlBuilder();
            var get = HttpMethod.Get;
            var getRequestModel = builder
                .SetHttpMethod(get)
                .SetUrl("asd")
                .GetResult();
            Assert.Equal(get, getRequestModel.HttpMethod);

            var post = HttpMethod.Post;
            var postRequestModel = builder
                .SetHttpMethod(post)
                .SetUrl("asd")
                .GetResult();
            Assert.Equal(post, postRequestModel.HttpMethod);
        }

        /// <summary>
        ///     Тест выброса ошибки при попытке установить неинициализированный http-метод
        /// </summary>
        [Fact(DisplayName = "RequestUrlBuilder: Set null http method Test")]
        public void SetNullHttpMethod_Test()
        {
            var builder = new HttpRequestUrlBuilder();
            Assert.Throws<ArgumentNullException>(() => builder.SetHttpMethod(null));
        }

        /// <summary>
        ///     Тест установки типа контента
        /// </summary>
        [Fact(DisplayName = "RequestUrlBuilder: Set content type Test")]
        public void SetContentType_Test()
        {
            var builder = new HttpRequestUrlBuilder();
            var contentType = "application/json";

            var requestModel = builder
                .SetContentType(contentType)
                .SetUrl("asd")
                .GetResult();

            Assert.Equal(contentType, requestModel.ContentType);
        }

        /// <summary>
        ///     Тест выброса ошибки при попытке установить пустой или неинициализированный тип контента
        /// </summary>
        [Fact(DisplayName = "RequestUrlBuilder: Set empty or null content type Test")]
        public void SetEmptyOrNullContentType_Test()
        {
            var builder = new HttpRequestUrlBuilder();
            var emptycontentType = "";
            string nullcontentType = null;

            Assert.Throws<ArgumentNullException>(() => builder.SetContentType(emptycontentType));
            Assert.Throws<ArgumentNullException>(() => builder.SetContentType(nullcontentType));
        }

        /// <summary>
        ///     Тест установки тела запроса
        /// </summary>
        [Fact(DisplayName = "RequestUrlBuilder: Set body Test")]
        public void SetBody_Test()
        {
            var builder = new HttpRequestUrlBuilder();
            var parameters = new Dictionary<string, string>();
            parameters["1"] = "1";
            parameters["2"] = "2";
            parameters["3"] = "3";
            parameters["4"] = "4";

            var parametersStr = parameters
                .Select(pair => string.Format("{0}={1}", pair.Key, HttpUtility.UrlEncode(pair.Value)))
                .JoinToString("&");
            var expectedBody = Encoding.ASCII.GetBytes(parametersStr);

            var requestModel = builder
                .SetBody(parameters)
                .SetUrl("asd")
                .GetResult();

            Assert.Equal(expectedBody, requestModel.Body);
        }

        /// <summary>
        ///     Тест выброса ошибки при попытке установить неинициализированное словарь в тело запроса
        /// </summary>
        [Fact(DisplayName = "RequestUrlBuilder: Set null body Test")]
        public void SetNullBody_Test()
        {
            var builder = new HttpRequestUrlBuilder();
            Assert.Throws<ArgumentNullException>(() => builder.SetBody(null));
        }

        /// <summary>
        ///     Тест установки параметров запроса
        /// </summary>
        [Fact(DisplayName = "RequestUrlBuilder: Set parameters Test")]
        public void SetParameters_Test()
        {
            var builder = new HttpRequestUrlBuilder();
            var parameters = new Dictionary<string, string>();
            parameters["1"] = "1";
            parameters["2"] = "2";
            parameters["3"] = "3";
            parameters["4"] = "4";

            var expectedParams = new Dictionary<string, string>();
            expectedParams["1"] = "1";
            expectedParams["2"] = "2";
            expectedParams["3"] = "3";
            expectedParams["4"] = "4";

            var requestModel = builder
                .SetParameters(parameters)
                .SetUrl("asd")
                .GetResult();

            Assert.Equal(expectedParams, requestModel.Parameters);
        }

        /// <summary>
        ///     Тест выброса ошибки при попытке установить неинициализированное параметры
        /// </summary>
        [Fact(DisplayName = "RequestUrlBuilder: Set null parameters Test")]
        public void SetNullParameters_Test()
        {
            var builder = new HttpRequestUrlBuilder();
            Assert.Throws<ArgumentNullException>(() => builder.SetParameters(null));
        }

        /// <summary>
        ///     Тест добавления и удаления параметра запроса
        /// </summary>
        [Fact(DisplayName = "RequestUrlBuilder: Add and remove parameter Test")]
        public void AddRemoveParameter_Test()
        {
            var builder = new HttpRequestUrlBuilder()
                .SetUrl("asd")
                .AddParameter("1", "1")
                .AddParameter("2", "2");

            Assert.Throws<ArgumentException>(() => builder.AddParameter("1", "1"));
            var expectedParams = new Dictionary<string, string>();
            expectedParams["1"] = "1";
            expectedParams["2"] = "2";

            var requestModel = builder.GetResult();
            Assert.Equal(expectedParams, requestModel.Parameters);

            builder.RemoveParameter("1")
                .RemoveParameter("2")
                .RemoveParameter("3")
                .RemoveParameter("2"); // не должно быть ошибок

            expectedParams.Remove("1");
            expectedParams.Remove("2");

            var requestModelWihoutParams = builder.GetResult();
            Assert.Equal(expectedParams, requestModelWihoutParams.Parameters);
        }

        /// <summary>
        ///     Тест выброса ошибки при попытке добавить или удалить неинициализированное параметр
        /// </summary>
        [Fact(DisplayName = "RequestUrlBuilder: Add and remove null parameters Test")]
        public void AddRemoveNullParameter_Test()
        {
            var builder = new HttpRequestUrlBuilder();
            Assert.Throws<ArgumentNullException>(() => builder.AddParameter("", "1"));
            Assert.Throws<ArgumentNullException>(() => builder.AddParameter("1", ""));
            Assert.Throws<ArgumentNullException>(() => builder.AddParameter("", ""));
            Assert.Throws<ArgumentNullException>(() => builder.AddParameter(null, "1"));
            Assert.Throws<ArgumentNullException>(() => builder.AddParameter("1", null));
            Assert.Throws<ArgumentNullException>(() => builder.AddParameter(null, null));

            Assert.Throws<ArgumentNullException>(() => builder.RemoveParameter(""));
            Assert.Throws<ArgumentNullException>(() => builder.RemoveParameter(null));
        }

        /// <summary>
        ///     Тест сброса строителя
        /// </summary>
        [Fact(DisplayName = "RequestUrlBuilder: reset builder Test")]
        public void Reset_Test()
        {
            var parameters = new Dictionary<string, string>();
            var endpoint = "endpoint";
            var endpointWithInlineParams = "endpoint/32/followers";

            parameters["1"] = "1";
            parameters["2"] = "2";
            var builder = new HttpRequestUrlBuilder()
                .SetEndpoint(endpoint)
                .SetUrl(endpointWithInlineParams)
                .SetHttpMethod(HttpMethod.Post)
                .SetBody(parameters)
                .SetContentType("application/json")
                .SetParameters(parameters)
                .AddParameter("3", "3");
            
            var requestModel = builder.GetResult();
            
            var expectedFullEndpoint = $"{BaseUrl}{endpoint}";
            var expectedUrl = "https://api.binance.comendpoint/32/followers?1=1&2=2&3=3";
            Assert.Equal(expectedFullEndpoint, requestModel.Endpoint);
            Assert.Equal(expectedUrl, requestModel.Url);
            Assert.Equal(HttpMethod.Post, requestModel.HttpMethod);
            Assert.Equal("application/json", requestModel.ContentType);

            var parametersStr = parameters
               .Select(pair => string.Format("{0}={1}", pair.Key, HttpUtility.UrlEncode(pair.Value)))
               .JoinToString("&");
            var expectedBody = Encoding.ASCII.GetBytes(parametersStr);
            Assert.Equal(expectedBody, requestModel.Body);
            
            parameters.Add("3", "3");
            Assert.Equal(parameters, requestModel.Parameters);

            // Act
            var emptyRequestModel = builder
                .Reset()
                .SetUrl("/test")
                .GetResult();
            Assert.Null(emptyRequestModel.Endpoint);
            Assert.Equal($"{BaseUrl}/test?", emptyRequestModel.Url);
            Assert.Null(emptyRequestModel.HttpMethod);
            Assert.Empty(emptyRequestModel.Parameters);
            Assert.Null(emptyRequestModel.ContentType);
            Assert.Null(emptyRequestModel.Body);
        }

        #endregion
    }
}
