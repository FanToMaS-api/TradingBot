using System;
using System.Collections.Generic;
using System.Collections.Immutable;

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

    /// <summary>
    ///     Баланс определенной монеты на кошельке
    /// </summary>
    public class BalanceModel : IEquatable<BalanceModel>
    {
        /// <summary>
        ///     Название актива
        /// </summary>
        public string Asset { get; internal set; }

        /// <summary>
        ///     Кол-во
        /// </summary>
        public double Free { get; internal set; }

        /// <summary>
        ///     Кол-во в ордерах
        /// </summary>
        public double Locked { get; internal set; }

        /// <inheritdoc />
        public bool Equals(BalanceModel other) => Asset == other.Asset && Free == other.Free && Locked == other.Locked;
    }
}
