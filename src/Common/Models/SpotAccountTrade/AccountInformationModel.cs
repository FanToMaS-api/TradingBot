using System.Collections.Generic;

namespace Common.Models
{
    /// <summary>
    ///     Информация об аккаунте пользователя
    /// </summary>
    public class AccountInformationModel
    {
        /// <summary>
        ///     Комиссия мейкера
        /// </summary>
        public int MakerCommission { get; internal set; }

        /// <summary>
        ///     Комиссия тейкера
        /// </summary>
        public int TakerCommission { get; internal set; }

        /// <summary>
        ///     Комиссия при покупке
        /// </summary>
        public int BuyerCommission { get; internal set; }

        /// <summary>
        ///     Комиссия при продаже
        /// </summary>
        public int SellerCommission { get; internal set; }

        /// <summary>
        ///     Разрешена ли торговля
        /// </summary>
        public bool CanTrade { get; internal set; }

        /// <summary>
        ///     Можно ли снять средства
        /// </summary>
        public bool CanWithdraw { get; internal set; }

        /// <summary>
        ///     Можно ли внести средства
        /// </summary>
        public bool CanDeposit { get; internal set; }

        /// <summary>
        ///     Время обновления
        /// </summary>
        public long UpdateTimeUnix { get; internal set; }

        /// <summary>
        ///     Тип аккаунта (SPOT, FEATURES)
        /// </summary>
        public string AccountType { get; internal set; }

        /// <summary>
        ///     Активы кошелька
        /// </summary>
        public List<BalanceModel> Balances { get; internal set; }
    }
}
