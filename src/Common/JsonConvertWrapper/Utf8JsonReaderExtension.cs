using System.Globalization;
using System.Text.Json;

namespace Common.JsonConvertWrapper
{
    /// <summary>
    ///     Расширение для <see cref="Utf8JsonReader"/>
    /// </summary>
    public static class Utf8JsonReaderExtension
    {
        /// <summary>
        ///     Пропускает все, пока не дойдет до требуемой секции
        /// </summary>
        public static void SkipToSection(this ref Utf8JsonReader reader, string sectionName)
        {
            var lastPropertyName = "";
            var isSkipNeeded = true;
            while (reader.Read() && isSkipNeeded)
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    lastPropertyName = reader.GetString();
                    reader.Read();

                    isSkipNeeded = lastPropertyName != sectionName;
                }
            }
        }

        /// <summary>
        ///     Считывает значение и сдвигает позицию reader'a
        /// </summary>
        public static double ReadDoubleAndNext(this ref Utf8JsonReader reader)
        {
            var res = double.Parse(reader.GetString(), CultureInfo.InvariantCulture);
            reader.Read();

            return res;
        }

        /// <summary>
        ///     Считывает значение и сдвигает позицию reader'a
        /// </summary>
        public static long ReadLongAndNext(this ref Utf8JsonReader reader)
        {
            var res = reader.GetInt64();
            reader.Read();

            return res;
        }

        /// <summary>
        ///     Считывает значение и сдвигает позицию reader'a
        /// </summary>
        public static string ReadStringAndNext(this ref Utf8JsonReader reader)
        {
            var res = reader.GetString();
            reader.Read();

            return res;
        }

        /// <summary>
        ///     Считывает значение и сдвигает позицию reader'a
        /// </summary>
        public static int ReadIntAndNext(this ref Utf8JsonReader reader)
        {
            var res = reader.GetInt32();
            reader.Read();

            return res;
        }
    }
}
