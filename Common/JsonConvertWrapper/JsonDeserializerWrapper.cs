using Common.JsonConvertWrapper.Converters;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Common.JsonConvertWrapper
{
    /// <summary>
    ///     Оболочка для десериализации объектов
    /// </summary>
    public class JsonDeserializerWrapper
    {
        #region Fields

        private readonly JsonSerializerOptions _jsonSerializerOptions;

        #endregion

        #region .ctor

        /// <inheritdoc cref="JsonDeserializerWrapper"/>
        public JsonDeserializerWrapper()
        {
            _jsonSerializerOptions = new JsonSerializerOptions();
            _jsonSerializerOptions.Converters.Add(item: new AutoStringToNumberConverter());
        }

        #endregion

        #region Public methods

        /// <summary>
        ///     Добавляет конвертер
        /// </summary>
        public void AddConverter(JsonConverter item) => _jsonSerializerOptions.Converters.Add(item);

        /// <summary>
        ///     Десериализует json в требуемый объект
        /// </summary>
        public T Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, _jsonSerializerOptions);

        #endregion
    }
}
