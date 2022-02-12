using System.Text.Json;

namespace Common.JsonConvertWrapper
{
    /// <summary>
    ///     Расширение для <see cref="Utf8JsonReader"/>
    /// </summary>
    public static class Utf8JsonReaderExtension
    {
        /// <summary>
        ///     Считывает занчение и сдвигает позицию reader'a
        /// </summary>
        public static double ReadDoubleAndNext(this ref Utf8JsonReader reader)
        {
            var res = double.Parse(reader.GetString());
            reader.Read();

            return res;
        }

        /// <summary>
        ///     Считывает занчение и сдвигает позицию reader'a
        /// </summary>
        public static long ReadLongAndNext(this ref Utf8JsonReader reader)
        {
            var res = reader.GetInt64();
            reader.Read();

            return res;
        }

        /// <summary>
        ///     Считывает занчение и сдвигает позицию reader'a
        /// </summary>
        public static int ReadIntAndNext(this ref Utf8JsonReader reader)
        {
            var res = reader.GetInt32();
            reader.Read();

            return res;
        }
    }
}
