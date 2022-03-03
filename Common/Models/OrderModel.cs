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
        public double Price { get; set; }

        /// <summary>
        ///     Объем
        /// </summary>
        public double Quantity { get; set; }
    }
}
