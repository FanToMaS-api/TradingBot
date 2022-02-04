using ExchangeLibrary.Binance.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ExchangeLibrary.Binance.Exceptions
{
    /// <summary>
    ///     Определяет исключения возникающие при работе с Binance
    /// </summary>
    internal class BinanceException : Exception
    {
        #region .ctor

        /// <inheritdoc cref="BinanceException"/>
        public BinanceException(BinanceExceptionType exceptionType) : base() 
        {
            ExceptionType = exceptionType;
        }

        /// <inheritdoc cref="BinanceException"/>
        public BinanceException(string message) : base(message)
        { }

        /// <inheritdoc cref="BinanceException"/>
        public BinanceException(BinanceExceptionType exceptionType, string message, Exception innerException) : base(message, innerException)
        {
            ExceptionType = exceptionType;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Код ошибки
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        ///     Сообщения в заголовке ошибки
        /// </summary>
        public Dictionary<string, IEnumerable<string>> Headers { get; set; }

        /// <summary>
        ///     Тип исключения
        /// </summary>
        public BinanceExceptionType? ExceptionType { get; private set; }

        /// <inheritdoc />
        public override string Message => $"Code: {StatusCode}\n" +
            $"Message: {ExceptionType.Display()}\n" +
            $"Server message: {base.Message}";

        #endregion

        #region Public methods

        /// <inheritdoc />
        public override string ToString() => $"Binance exception type: {ExceptionType.Display()} | " + base.ToString();

        #endregion
    }
}
