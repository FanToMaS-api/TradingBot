using System;
using System.Threading.Tasks;
using Quartz;

namespace Scheduler
{
    /// <summary>
    ///     Повторяющаяся задача
    /// </summary>
    internal sealed class RecurringJob : IJob
    {
        public const string ServiceProviderKey = nameof(ServiceProviderKey);
        public const string ActionKey = nameof(ActionKey);
        public const string ContextKey = nameof(ContextKey);

        /// <summary>
        ///     Выполняет указанную задачу
        /// </summary>
        public async Task Execute(IJobExecutionContext context)
        {
            if (!context.MergedJobDataMap.TryGetValue(ServiceProviderKey, out var obj) ||
                obj is not IServiceProvider serviceProvider)
            {
                return;
            }

            if (!context.MergedJobDataMap.TryGetValue(ActionKey, out obj) ||
                obj is not Func<IServiceProvider, Task> func)
            {
                return;
            }

            if (!context.MergedJobDataMap.TryGetValue(ContextKey, out obj) ||
                obj is not IRecurringJobContext ctx)
            {
                return;
            }

            using var tracker = ctx.TrackJobExecution();
            if (tracker is null)
            {
                return;
            }

            await func(serviceProvider);
        }
    }
}
