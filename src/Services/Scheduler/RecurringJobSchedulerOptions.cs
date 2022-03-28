using System.Collections.Generic;

namespace Scheduler
{
    /// <summary>
    ///     Параметры планировщика повторяющихся заданий
    /// </summary>
    internal sealed class RecurringJobSchedulerOptions
    {
        /// <summary>
        ///     Список повторяющихся задач
        /// </summary>
        public List<RecurringJobDefinition> RecurringJobs { get; } = new List<RecurringJobDefinition>();
    }
}
