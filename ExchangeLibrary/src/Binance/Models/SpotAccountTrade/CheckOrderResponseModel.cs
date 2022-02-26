using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExchangeLibrary.Binance.Models
{
    /// <summary>
    ///     Модель ответа на запрос состояния ордера
    /// </summary>
    internal class CheckOrderResponseModel : OrderResponseModelBase
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
                        StopPrice = double.Parse(reader.GetString());
                        continue;
                    case "icebergQty":
                        IcebergQty = double.Parse(reader.GetString());
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
                        OrigQuoteOrderQty = double.Parse(reader.GetString());
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
    internal class CheckOrderResponseModelConverter : JsonConverter<CheckOrderResponseModel>
    {
        /// <inheritdoc />
        public override CheckOrderResponseModel Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var model = new CheckOrderResponseModel();
            model.SetProperties(ref reader);

            return model;
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, CheckOrderResponseModel value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
