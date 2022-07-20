using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Quartz;

namespace Scheduler
{
    /// <summary>
    ///     Сервис задач по расписанию
    /// </summary>
    public interface IRecurringJobScheduler
    {
        /// <summary>
        ///     Запланировать задачу
        /// </summary>
        [NotNull]
        Task<TriggerKey> ScheduleAsync([NotNull] string cronExpression, [NotNull] Func<IServiceProvider, Task> func, string name = default);

        /// <summary>
        ///     Перезапланировать задачу на другое время, не изменяя триггер
        /// </summary>
        [NotNull]
        Task RescheduleAsync(string cronExpression, string triggerName);

        /// <summary>
        ///     Удалить планирование задачи
        /// </summary>
        [NotNull]
        Task UnscheduleAsync(TriggerKey triggerKey);

        /// <summary>
        ///     Останавливает все задачи
        /// </summary>
        Task UnscheduleAsync(List<TriggerKey> triggerKeys);

        /// <summary>
        ///     Создаёт задачу на метод с заданным интервалом повторения
        /// </summary>
        /// <remarks>
        ///     Исключается возможность выполнения одной задачи дважды одновременно
        /// </remarks>
        Task<TriggerKey> GeneralScheduleAsync(
             string cronExpress,
             Func<IServiceProvider, Task> func);
    }
}
