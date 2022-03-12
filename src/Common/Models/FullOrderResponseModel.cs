using System.Collections.Generic;

namespace Common.Models
{
    /// <summary>
    ///     Модель ответа на отправку нового ордера (содержит полную информацию)
    /// </summary>
    public class FullOrderResponseModel : OrderResponseModelBase
    {
        /// <summary>
        ///     Время исполнения транзакции
        /// </summary>
        public long TransactTimeUnix { get; set; }

        /// <summary>
        ///     Части заполнения ордера
        /// </summary>
        public List<FillModel> Fills { get; set; } = new();
    }

    /// <summary>
    ///     Содержит информацию о частях заполнения ордера
    /// </summary>
    public class FillModel
    {
        /// <summary>
        ///     Цена
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        ///     Кол-во
        /// </summary>
        public double Quantity { get; set; }

        /// <summary>
        ///     Коммиссия
        /// </summary>
        public double Commission { get; set; }

        /// <summary>
        ///     Актив комиссии
        /// </summary>
        public string CommissionAsset { get; set; }

        /// <summary>
        ///     Id сделки
        /// </summary>
        public long TradeId { get; set; }
    }
}
