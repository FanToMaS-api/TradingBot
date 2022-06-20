using System.Collections.Generic;

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
}
