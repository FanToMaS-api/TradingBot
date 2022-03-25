namespace Common.Enums
{
    /// <summary>
    ///     Расширяет enum'ы
    /// </summary>
    public static class EnumHelper
    {
        /// <summary>
        ///     Переводит тип ордера в формате бинанса в удобный для работы <see cref="OrderSideType"/>
        /// </summary>
        public static OrderSideType ConvertToOrderSideType(this string type) =>
            type switch
            {
                "BUY" => OrderSideType.Buy,
                "SELL" => OrderSideType.Sell,
                _ => throw new System.Exception($"Failed to convert '{type}' to {nameof(OrderSideType)}"),
            };

        /// <summary>
        ///     Получить строку из типа для отправки запроса бинансу
        /// </summary>
        public static string ToUrl(this OrderSideType type)
        {
            return type switch
            {
                OrderSideType.Sell => "SELL",
                OrderSideType.Buy => "BUY",
                _ => type.ToString(),
            };
        }
    }
}
