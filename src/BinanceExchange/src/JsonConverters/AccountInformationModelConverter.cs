using BinanceExchange.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BinanceExchange.JsonConverters
{
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
}
