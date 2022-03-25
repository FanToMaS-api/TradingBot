using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BinanceExchange.Models
{
    /// <summary>
    ///     Информация об аккаунте пользователя
    /// </summary>
    internal class AccountInformationModel
    {
        /// <summary>
        ///     Комиссия мейкера
        /// </summary>
        public int MakerCommission { get; set; }

        /// <summary>
        ///     Комиссия тейкера
        /// </summary>
        public int TakerCommission { get; set; }

        /// <summary>
        ///     Комиссия при покупке
        /// </summary>
        public int BuyerCommission { get; set; }

        /// <summary>
        ///     Комиссия при продаже
        /// </summary>
        public int SellerCommission { get; set; }

        /// <summary>
        ///     Разрешена ли торговля
        /// </summary>
        public bool CanTrade { get; set; }

        /// <summary>
        ///     Можно ли снять средства
        /// </summary>
        public bool CanWithdraw { get; set; }

        /// <summary>
        ///     Можно ли внести средства
        /// </summary>
        public bool CanDeposit { get; set; }

        /// <summary>
        ///     Время обновления
        /// </summary>
        public long UpdateTimeUnix { get; set; }

        /// <summary>
        ///     Тип аккаунта (SPOT, FEATURES)
        /// </summary>
        public string AccountType { get; set; }

        /// <summary>
        ///     Активы кошелька
        /// </summary>
        public List<BalanceModel> Balances { get; set; } = new();
    }

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

    #region AccountInformationModelConverter

    /// <summary>
    ///     Нормально конвертирует полученные данные
    /// </summary>
    internal class AccountInformationModelConverter : JsonConverter<AccountInformationModel>
    {
        /// <inheritdoc />
        public override AccountInformationModel Read(ref Utf8JsonReader reader, System.Type typeToConvert, JsonSerializerOptions options)
        {
            var result = new AccountInformationModel();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var lastPropertyName = reader.GetString();
                    reader.Read();

                    switch (lastPropertyName)
                    {
                        case "makerCommission":
                            result.MakerCommission = reader.GetInt32();
                            continue;
                        case "takerCommission":
                            result.TakerCommission = reader.GetInt32();
                            continue;
                        case "buyerCommission":
                            result.BuyerCommission = reader.GetInt32();
                            continue;
                        case "sellerCommission":
                            result.SellerCommission = reader.GetInt32();
                            continue;
                        case "canTrade":
                            result.CanTrade = reader.GetBoolean();
                            continue;
                        case "canWithdraw":
                            result.CanWithdraw = reader.GetBoolean();
                            continue;
                        case "canDeposit":
                            result.CanDeposit = reader.GetBoolean();
                            continue;
                        case "updateTime":
                            result.UpdateTimeUnix = reader.GetInt64();
                            continue;
                        case "accountType":
                            result.AccountType = reader.GetString();
                            continue;
                        case "balances":
                            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                            {
                                var balanceModel = new BalanceModel();
                                balanceModel.SetProperties(ref reader);

                                result.Balances.Add(balanceModel);
                            }
                            continue;
                    }
                }
            }

            return result;
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, AccountInformationModel value, JsonSerializerOptions options)
        {
            throw new System.NotImplementedException();
        }
    }

    #endregion
}
