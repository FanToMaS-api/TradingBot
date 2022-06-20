using System;

namespace Common.Models
{
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
