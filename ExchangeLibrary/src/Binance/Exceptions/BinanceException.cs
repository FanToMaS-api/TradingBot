using ExchangeLibrary.Binance.Enums;
using System;

namespace ExchangeLibrary.Binance.Exceptions
{
    /// <summary>
    ///     Определяет исключения возникающие при работе с Binance
    /// </summary>
    internal class BinanceException : Exception
    {
        #region .ctor

        /// <inheritdoc cref="BinanceException"/>
        public BinanceException(Exception exception, BinanceExceptionType exceptionType) : base(exception.Message, exception) 
        {
            ExceptionType = exceptionType;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Тип исключения
        /// </summary>
        public BinanceExceptionType ExceptionType { get; private set; }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public override string ToString() => $"Binance exception type: {ExceptionType.Display()} | " + base.ToString();

        #endregion
    }
}
