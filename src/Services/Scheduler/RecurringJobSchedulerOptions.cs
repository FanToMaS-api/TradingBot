using System.Collections.Generic;

namespace Scraper.Common.Schedule
{
    /// <summary>
    /// Параметры планировщика повторяющихся заданий
    /// </summary>
    internal sealed class RecurringJobSchedulerOptions
    {
        /// <summary>
        ///     Список повторяющихся задач
        /// </summary>
        public List<RecurringJobDefinition> RecurringJobs { get; } = new List<RecurringJobDefinition>();
    }
}
