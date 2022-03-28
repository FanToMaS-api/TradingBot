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
        public static string Secondly()
            => "* * * * * ? *";

        /// <summary>
        ///     Каждые N секунд
        /// </summary>
        public static string Secondly(int N)
            => $"0/{N} * * * * ? *";

        /// <summary>
        ///     Ежеминутно
        /// </summary>
        public static string Minutely(int second = 0)
            => $"{second} * * * * ? *";

        /// <summary>
        ///     В определенные минуты
        /// </summary>
        public static string AtMinutes(int[] minutes)
            => $"0 {string.Join(',', minutes)} * ? * * *";

        /// <summary>
        ///     Каждые сколько-то минут
        /// </summary>
        public static string EveryNthMinute(int every)
            => $"0 0/{every} * * * ? *";

        /// <summary>
        ///     Ежечасно
        /// </summary>
        public static string Hourly(int minute = 0, int second = 0)
            => $"{second} {minute} * * * ? *";

        /// <summary>
        ///     Ежедневно
        /// </summary>
        public static string Daily(int hour = 0, int minute = 0, int second = 0)
            => $"{second} {minute} {hour} * * ? *";

        /// <summary>
        ///     Еженедельно
        /// </summary>
        public static string Weekly(DayOfWeek dayOfWeek = DayOfWeek.Monday, int hour = 0, int minute = 0, int second = 0)
            => $"{second} {minute} {hour} * * {(int)dayOfWeek} *";

        /// <summary>
        ///     Ежемесячно
        /// </summary>
        public static string Monthly(int day = 1, int hour = 0, int minute = 0, int second = 0)
            => $"{second} {minute} {hour} {day} * ? *";

        /// <summary>
        ///     Ежегодно
        /// </summary>
        public static Cron Yearly(int month = 1, int day = 1, int hour = 0, int minute = 0, int second = 0)
            => $"{second} {minute} {hour} {day} {month} ? *";

        /// <summary>
        ///     Никогда (31 февраля каждого года)
        /// </summary>
        public static Cron Never() => Yearly(2, 31);

        #endregion

        /// <inheritdoc cref="Cron"/>
        public Cron(string value)
        {
            Value = value;
        }

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

        public static bool operator ==(Cron left, Cron right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Cron left, Cron right)
        {
            return !(left == right);
        }
    }
}
