﻿using System;
using System.Threading.Tasks;

namespace Scraper.Common.Schedule
{
    /// <summary>
    ///     Определение повторяющейся работы
    /// </summary>
    internal sealed class RecurringJobDefinition
    {
        #region Properties

        /// <summary>
        ///     Cron-выражение
        /// </summary>
        public string CronExpression { get; }

        /// <summary>
        ///     Задача на повторение
        /// </summary>
        public Func<IServiceProvider, Task> Action { get; }

        #endregion

        #region .ctor

        /// <summary>
        ///     Определение повторяющейся работы
        /// </summary>
        /// <param name="cronExpression"> Cron-выражение </param>
        /// <param name="action"> Задача на повторение </param>
        public RecurringJobDefinition(string cronExpression, Func<IServiceProvider, Task> action)
        {
            CronExpression = cronExpression;
            Action = action;
        }

        #endregion
    }
}
