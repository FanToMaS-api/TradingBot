using System.Text.Json;

namespace Common.JsonConvertWrapper
{
    /// <summary>
    ///     Показывает, что класс сам отвечает за свою десериализацию
    /// </summary>
    public interface IHaveMyOwnJsonConverter
    {
        /// <summary>
        ///     Наполняет объект св-ми из json readr'a
        /// </summary>
        /// <param name="reader"> Reader с указателем на начало объекта </param>
        public void SetProperties(ref Utf8JsonReader reader, IHaveMyOwnJsonConverter result);
    }
}
