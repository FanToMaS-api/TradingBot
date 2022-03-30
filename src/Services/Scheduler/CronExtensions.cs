using System;
using Quartz;

namespace Scheduler
{
    /// <summary>
    ///     Расширяющий класс для <see cref="Cron"/>
    /// </summary>
    public static class CronExtensions
    {
        /// <summary>
        ///     Возвращает временной интервал между следующими датами выгрузки
        /// </summary>
        public static TimeSpan GetNextInterval(this Cron cron, DateTime nowDateTime)
        {
            var expression = new CronExpression(cron.Value);
            var next = expression.GetNextValidTimeAfter(nowDateTime);

            return expression.GetNextValidTimeAfter(next.Value).Value - next.Value;
        }
    }
}
