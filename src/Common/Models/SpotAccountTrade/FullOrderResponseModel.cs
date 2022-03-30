using System;
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
        public long TransactTimeUnix { get; internal set; }

        /// <summary>
        ///     Части заполнения ордера
        /// </summary>
        public List<FillModel> Fills { get; internal set; } = new();
    }

    /// <summary>
    ///     Содержит информацию о частях заполнения ордера
    /// </summary>
    public class FillModel : IEquatable<FillModel>
    {
        /// <summary>
        ///     Цена
        /// </summary>
        public double Price { get; internal set; }

        /// <summary>
        ///     Кол-во
        /// </summary>
        public double Quantity { get; internal set; }

        /// <summary>
        ///     Коммиссия
        /// </summary>
        public double Commission { get; internal set; }

        /// <summary>
        ///     Актив комиссии
        /// </summary>
        public string CommissionAsset { get; internal set; }

        /// <summary>
        ///     Id сделки
        /// </summary>
        public long TradeId { get; internal set; }

        /// <inheritdoc />
        public bool Equals(FillModel other)
        {
            return Price == other.Price
                && Quantity == other.Quantity
                && CommissionAsset == other.CommissionAsset
                && TradeId == other.TradeId
                && Commission == other.Commission;
        }
    }
}
