using System;

namespace Scheduler
{
    /// <summary>
    ///     Контекст задач
    /// </summary>
    internal interface IRecurringJobContext
    {
        /// <summary>
        ///     Отслеживает состояние выполнения задачи
        /// </summary>
        IDisposable TrackJobExecution();
    }
}
