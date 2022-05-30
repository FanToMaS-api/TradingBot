using Common.JsonConvertWrapper;
using System.Globalization;
using System.Text.Json;

namespace BinanceExchange.Models
{
    /// <summary>
    ///     Модель ответа на запрос состояния ордера
    /// </summary>
    internal class CheckOrderResponseModel : OrderResponseModelBase, IHaveMyOwnJsonConverter
    {
        /// <summary>
        ///     Стоп цена
        /// </summary>
        public double StopPrice { get; set; }

        /// <summary>
        ///     Кол-во для ордера-айсберга
        /// </summary>
        public double IcebergQty { get; set; }

        /// <summary>
        ///     Время
        /// </summary>
        public long TimeUnix { get; set; }

        /// <summary>
        ///     Время обновления
        /// </summary>
        public long UpdateTimeUnix { get; set; }

        /// <summary>
        ///     Открыт ли сейчас ордер
        /// </summary>
        public bool IsWorking { get; set; }

        /// <summary>
        ///     Кол-во для квотируемого ордера
        /// </summary>
        public double OrigQuoteOrderQty { get; set; }

        /// <summary>
        ///     Установить свойства
        /// </summary>
        public void SetProperties(ref Utf8JsonReader reader)
        {
            var propertyName = "";
            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    propertyName = reader.GetString();
                    reader.Read();
                }

                switch (propertyName)
                {
                    case "stopPrice":
                        StopPrice = double.Parse(reader.GetString(), CultureInfo.InvariantCulture);
                        continue;
                    case "icebergQty":
                        IcebergQty = double.Parse(reader.GetString(), CultureInfo.InvariantCulture);
                        continue;
                    case "time":
                        TimeUnix = reader.GetInt64();
                        continue;
                    case "updateTime":
                        UpdateTimeUnix = reader.GetInt64();
                        continue;
                    case "isWorking":
                        IsWorking = reader.GetBoolean();
                        continue;
                    case "origQuoteOrderQty":
                        OrigQuoteOrderQty = double.Parse(reader.GetString(), CultureInfo.InvariantCulture);
                        continue;
                    default:
                        {
                            SetProperty(propertyName, ref reader);
                            continue;
                        }
                }
            }
        }
    }
}
