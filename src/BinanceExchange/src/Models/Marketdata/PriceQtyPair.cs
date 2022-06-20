using System.Text.Json;
using System.Globalization;

namespace BinanceExchange.Models
{
    /// <summary>
    ///     Модель цены и объема ордера
    /// </summary>
    public class PriceQtyPair
    {
        /// <summary>
        ///     Цена
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        ///     Объем
        /// </summary>
        public double Quantity { get; set; }

        /// <summary>
        ///     Создает пару и добавляет в нужный массив пар
        /// </summary>
        internal static void CreatePair(ref Utf8JsonReader reader, OrderBookModel result, string lastPropertyName)
        {
            var workItem = new PriceQtyPair
            {
                Price = double.Parse(reader.GetString(), CultureInfo.InvariantCulture)
            };
            reader.Read();
            workItem.Quantity = double.Parse(reader.GetString(), CultureInfo.InvariantCulture);
            switch (lastPropertyName)
            {
                case "bids":
                    result.Bids.Add(workItem);
                    break;
                case "asks":
                    result.Asks.Add(workItem);
                    break;
            }
        }
    }
}
