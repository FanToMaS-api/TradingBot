namespace Common.Models
{
    /// <summary>
    ///     Модель цены и объема ордера
    /// </summary>
    public class OrderModel
    {
        /// <summary>
        ///     Цена
        /// </summary>
        public double Price { get; internal set; }

        /// <summary>
        ///     Объем
        /// </summary>
        public double Quantity { get; internal set; }
    }
}
