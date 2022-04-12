using System;

namespace Scheduler
{
    /// <summary>
    ///     CRON-выражение
    /// </summary>
    public readonly struct Cron
    {
        #region Factory

        /// <summary>
        ///     Ежесекундно
        /// </summary>
        public static string Secondly() => "* * * ? * *";

        /// <summary>
        ///     Каждые N секунд
        /// </summary>
        public static string Secondly(int N) => $"0/{N} 0 0 ? * * *";

        /// <summary>
        ///     Ежеминутно
        /// </summary>
        public static string Minutely() => $"0 * * ? * *";

        /// <summary>
        ///     Ежеминутно в определенную секунду
        /// </summary>
        public static string MinutelyOnSecond(int second) => $"{second} * * ? * *";

        /// <summary>
        ///     Ежечасно
        /// </summary>
        public static string Hourly() => $"0 0 * ? * *";

        /// <summary>
        ///     Ежедневно в 12 ночи
        /// </summary>
        public static string Daily() => $"0 0 0 * * ?";

        #endregion

        /// <inheritdoc cref="Cron"/>
        public Cron(string value) => Value = value;

        /// <summary>
        ///     Выражение
        /// </summary>
        public readonly string Value;

        public static implicit operator Cron(string str) => new(str);

        public static implicit operator string(Cron cron) => cron.Value;

        /// <inheritdoc />
        public override string ToString() => Value;

        /// <summary>
        ///     Сравнить
        /// </summary>
        public bool Equals(Cron other) => Value == other.Value;

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is Cron other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(Cron left, Cron right) => left.Equals(right);

        public static bool operator !=(Cron left, Cron right) => !(left == right);
    }
}
