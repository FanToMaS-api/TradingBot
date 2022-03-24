using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BinanceExchange.Models
{
    /// <summary>
    ///     Возвращает информацию об аккаунте пользователя
    /// </summary>
    internal class AccountInformation
    {
        /// <summary>
        ///     Комиссия мейкера
        /// </summary>
        [JsonPropertyName("makerCommission")]
        public int MakerCommission { get; set; }

        /// <summary>
        ///     Комиссия тейкера
        /// </summary>
        [JsonPropertyName("takerCommission")]
        public int TakerCommission { get; set; }

        /// <summary>
        ///     Комиссия при покупке
        /// </summary>
        [JsonPropertyName("buyerCommission")]
        public int BuyerCommission { get; set; }

        /// <summary>
        ///     Комиссия при продаже
        /// </summary>
        [JsonPropertyName("sellerCommission")]
        public int SellerCommission { get; set; }

        /// <summary>
        ///     Разрешена ли торговля
        /// </summary>
        [JsonPropertyName("canTrade")]
        public bool CanTrade { get; set; }

        /// <summary>
        ///     Можно ли снять средства
        /// </summary>
        [JsonPropertyName("canWithdraw")]
        public bool CanWithdraw { get; set; }

        /// <summary>
        ///     Можно ли внести средства
        /// </summary>
        [JsonPropertyName("canDeposit")]
        public bool CanDeposit { get; set; }

        /// <summary>
        ///     Время обновления
        /// </summary>
        [JsonPropertyName("updateTime")]
        public long UpdateTimeUnix { get; set; }

        /// <summary>
        ///     Тип аккаунта (SPOT, FEATURES)
        /// </summary>
        [JsonPropertyName("accountType")]
        public string AccountType { get; set; }

        /// <summary>
        ///     Активы кошелька
        /// </summary>
        [JsonPropertyName("balances")]
        public IEnumerable<Balance> Balances { get; set; }

        /// <summary>
        ///     Разрешения на торговлю
        /// </summary>
        [JsonPropertyName("permissions")]
        public IEnumerable<string> Permissions { get; set; }
    }

    /// <summary>
    ///     Баланс определенной монеты на кошельке
    /// </summary>
    internal class Balance
    {
        /// <summary>
        ///     Название актива
        /// </summary>
        [JsonPropertyName("asset")]
        public string Asset { get; set; }

        /// <summary>
        ///     Кол-во
        /// </summary>
        [JsonPropertyName("free")]
        public string Free { get; set; }

        /// <summary>
        ///     Кол-во в ордерах
        /// </summary>
        [JsonPropertyName("locked")]
        public string Locked { get; set; }

    }
}
