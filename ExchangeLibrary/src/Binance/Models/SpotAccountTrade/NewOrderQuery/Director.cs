using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeLibrary.Binance.Models.SpotAccountTrade.NewOrderQuery
{
    /// <summary>
    ///     Управляет строителями по созданию запросов на новый ордер
    /// </summary>
    internal class Director
    {
        #region Fields

        private IBuilder _builder;

        #endregion

        #region .ctor

        /// <inheritdoc cref="Director"/>
        public Director(IBuilder builder)
        {
            _builder = builder;
        }

        #endregion

        #region Public methods

        /// <summary>
        ///     Сменить строителя
        /// </summary>
        public void ChangeBuilder(IBuilder builder) => _builder = builder;

        /// <summary>
        ///     Выполнить шаги строителя
        /// </summary>
        public void Make(NewOrderType newOrderType)
        {

        }

        #endregion
    }
}
