using System;
using System.Globalization;
using System.Text.Json;

namespace BinanceExchange.Models
{
    /// <summary>
    ///     Баланс определенной монеты на кошельке
    /// </summary>
    internal class BalanceModel : IEquatable<BalanceModel>
    {
        /// <summary>
        ///     Название актива
        /// </summary>
        public string Asset { get; set; }

        /// <summary>
        ///     Кол-во
        /// </summary>
        public double Free { get; set; }

        /// <summary>
        ///     Кол-во в ордерах
        /// </summary>
        public double Locked { get; set; }

        /// <inheritdoc />
        public bool Equals(BalanceModel other) => Asset == other.Asset && Free == other.Free && Locked == other.Locked;

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
                    case "locked":
                        Locked = double.Parse(reader.GetString(), CultureInfo.InvariantCulture);
                        continue;
                    case "free":
                        Free = double.Parse(reader.GetString(), CultureInfo.InvariantCulture);
                        continue;
                    case "asset":
                        Asset = reader.GetString();
                        continue;
                }
            }
        }
    }

    #endregion
}
