using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    /// <summary>
    ///     Модель сделки
    /// </summary>
    public class TradeModel
    {
        /// <summary>
        ///     Уникальный идентификатор
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        ///     Цена сделки
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        ///     Кол-во
        /// </summary>
        public double Quantity { get; set; }

        /// <summary>
        ///     Кол-во квотируемой
        /// </summary>
        public double QuoteQty { get; set; }

        /// <summary>
        ///     Время сделки
        /// </summary>
        public long TimeUnix { get; set; }

        /// <summary>
        ///     Была ли покупка по указанной покупателем цене
        /// </summary>
        public bool IsBuyerMaker { get; set; }

        /// <summary>
        ///     Была ли встречная сделка
        /// </summary>
        public bool IsBestMatch { get; set; }
    }
}
