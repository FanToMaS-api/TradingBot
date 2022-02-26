using Common.JsonConvertWrapper;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExchangeLibrary.Binance.Models
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

    /// <summary>
    ///     Нормально конвертирует полученные данные
    /// </summary>
    internal class CancelOrderResponseModelConverter : JsonConverter<CancelOrderResponseModel>
    {
        /// <inheritdoc />
        public override CancelOrderResponseModel Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var model = new CancelOrderResponseModel();
            model.SetProperties(ref reader);

            return model;
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, CancelOrderResponseModel value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
