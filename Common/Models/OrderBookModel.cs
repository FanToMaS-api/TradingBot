using System.Collections.Generic;

namespace Common.Models
{
    /// <summary>
    ///     Модель книги ордеров
    /// </summary>
    public class OrderBookModel
    {
        /// <summary>
        ///    Идентификатор последнего обновления 
        /// </summary>
        public long LastUpdateId { get; set; }

        /// <summary>
        ///     Ордера на покупку
        /// </summary>
        public List<OrderModel> Bids { get; set; } = new();

        /// <summary>
        ///     Ордера на продажу
        /// </summary>
        public List<OrderModel> Asks { get; set; } = new();
    }
}
