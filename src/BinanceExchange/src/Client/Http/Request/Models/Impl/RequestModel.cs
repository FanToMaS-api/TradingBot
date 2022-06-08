using System.Collections.Generic;
using System.Net.Http;

namespace BinanceExchange.Client.Http.Request.Models.Impl
{
    /// <inheritdoc cref="IRequestModel"/>
    internal class RequestModel : IRequestModel
    {
        #region Properties

        /// <inheritdoc />
        public string Endpoint { get; internal set; }
        
        /// <inheritdoc />
        public string ParametersStr { get; internal set; }

        /// <inheritdoc />
        public string Url { get; internal set; }

        /// <inheritdoc />
        public HttpMethod HttpMethod { get; internal set; }

        /// <inheritdoc />
        public string ContentType { get; internal set; }

        /// <inheritdoc />
        public byte[] Body { get; internal set; }

        /// <inheritdoc />
        public IReadOnlyDictionary<string, string> Parameters { get; internal set; }

        #endregion
    }
}
