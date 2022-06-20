using Common.JsonConvertWrapper;
using System.Text.Json;

namespace BinanceExchange.Models
{
    /// <summary>
    ///     Модель ответа на запрос отмены ордера
    /// </summary>
    internal class CancelOrderResponseModel : OrderResponseModelBase, IHaveMyOwnJsonConverter
    {
        /// <summary>
        ///     ID ордера, назначенный пользователем или сгенерированный
        /// </summary>
        public string OrigClientOrderId { get; set; }

        /// <inheritdoc />
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
                    case "origClientOrderId":
                        OrigClientOrderId = reader.GetString();
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
