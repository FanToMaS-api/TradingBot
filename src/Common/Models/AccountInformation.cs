using System.Collections.Generic;

namespace Common.Models
{
    /// <summary>
    ///     Возвращает информацию об аккаунте пользователя
    /// </summary>
    public class AccountInformation
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
        public IEnumerable<Balance> Balances { get; set; }
    }

    /// <summary>
    ///     Баланс определенной монеты на кошельке
    /// </summary>
    public class Balance
    {
        /// <summary>
        ///     Название актива
        /// </summary>
        public string Asset { get; set; }

        /// <summary>
        ///     Кол-во
        /// </summary>
        public string Free { get; set; }

        /// <summary>
        ///     Кол-во в ордерах
        /// </summary>
        public string Locked { get; set; }

    }
}
