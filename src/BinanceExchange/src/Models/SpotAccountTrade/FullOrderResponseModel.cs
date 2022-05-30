using System.Collections.Generic;
using System.Text.Json;

namespace BinanceExchange.Models
{
    /// <summary>
    ///     Модель ответа на отправку нового ордера (содержит полную информацию)
    /// </summary>
    internal class FullOrderResponseModel : OrderResponseModelBase
    {
        /// <summary>
        ///     Время исполнения транзакции
        /// </summary>
        public long TransactTimeUnix { get; set; }

        /// <summary>
        ///     Части заполнения ордера
        /// </summary>
        public List<FillModel> Fills { get; set; } = new();

        /// <summary>
        ///     Установить свойства
        /// </summary>
        public void SetProperties(ref Utf8JsonReader reader)
        {
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    FillModel.CreateFillModel(ref reader, this);
                }

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var propertyName = reader.GetString();
                    reader.Read();

                    switch (propertyName)
                    {
                        case "transactTime":
                            TransactTimeUnix = reader.GetInt64();
                            continue;
                        case "fills":
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
}
