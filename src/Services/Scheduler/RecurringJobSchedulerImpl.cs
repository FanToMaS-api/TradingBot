using Logger;
using Microsoft.Extensions.Options;
using NLog;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Scheduler
{
    /// <inheritdoc cref="IRecurringJobScheduler"/>
    internal sealed class RecurringJobSchedulerImpl : IRecurringJobScheduler, IRecurringJobContext, IAsyncDisposable
    {
        #region Fields

        private static readonly ILoggerDecorator Log = LoggerManager.CreateDefaultLogger();
        private readonly RecurringJobSchedulerOptions _options;
        private readonly IServiceProvider _serviceProvider;
        private IScheduler _scheduler;
        private readonly List<bool> _busyScheduled = new();
        private readonly object _runningJobCounterLock = new();
        private int _runningJobCounter;
        private bool _isDisposed;

        #endregion

        #region .ctor

        /// <inheritdoc cref="RecurringJobSchedulerImpl"/>
        public RecurringJobSchedulerImpl(
            IOptions<RecurringJobSchedulerOptions> options,
            IServiceProvider serviceProvider)
        {
            _options = options.Value;
            _serviceProvider = serviceProvider;
        }

        #endregion

        /// <summary>
        ///     Инициализация сервиса задач по расписанию
        /// </summary>
        public async Task InitializeAsync()
        {
            var factory = new StdSchedulerFactory();
            _scheduler = await factory.GetScheduler();

            await _scheduler.Start();

            foreach (var jobDefinition in _options.RecurringJobs)
            {
                await ScheduleAsync(jobDefinition.CronExpression, jobDefinition.Action);
            }
        }

        /// <inheritdoc />
        public async Task<TriggerKey> GeneralScheduleAsync(
            Func<CancellationToken, Task> action,
            string cronExpress,
            CancellationToken cancellationToken = default)
        {
            var id = _busyScheduled.Count;
            _busyScheduled.Insert(id, false);
            var triggerKey = await ScheduleAsync(
                cronExpress,
                _ => Task.Run(
                    async () =>
                    {
                        if (_busyScheduled[id])
                        {
                            await Log.WarnAsync($"The task {action.Method.Name} is in progress");
                            return;
                        }

                        _busyScheduled[id] = true;

                        try
                        {
                            await action(cancellationToken);
                        }
                        catch (Exception ex)
                        {
                            await Log.ErrorAsync(ex, $"Failed to complete the task of {action.Method.Name}");
                        }
                        finally
                        {
                            _busyScheduled[id] = false;
                        }
                    },
                    cancellationToken
                ));

            return triggerKey;
        }

        /// <inheritdoc />
        public async Task<TriggerKey> ScheduleAsync(string cronExpression, Func<IServiceProvider, Task> func, string name = default)
        {
            CronExpression.ValidateExpression(cronExpression);

            var job = JobBuilder.Create<RecurringJob>()
                .SetJobData(
                    new JobDataMap
                    {
                        [RecurringJob.ServiceProviderKey] = _serviceProvider,
                        [RecurringJob.ContextKey] = this,
                        [RecurringJob.ActionKey] = func,
                    })
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity(name ?? Guid.NewGuid().ToString())
                .StartNow()
                .WithCronSchedule(cronExpression)
                .Build();

            await _scheduler.ScheduleJob(job, trigger);

            return trigger.Key;
        }

        /// <inheritdoc />
        public async Task RescheduleAsync(string cronExpression, string triggerName)
        {
            CronExpression.ValidateExpression(cronExpression);

            var oldTrigger = await _scheduler.GetTrigger(new TriggerKey(triggerName));

            var newTrigger = TriggerBuilder.Create()
                .WithIdentity(triggerName)
                .StartNow()
                .WithCronSchedule(cronExpression)
                .Build();

            await _scheduler.RescheduleJob(oldTrigger.Key, newTrigger);
        }

        /// <inheritdoc />
        public Task UnscheduleAsync(TriggerKey triggerKey) => _scheduler.UnscheduleJob(triggerKey);

        /// <inheritdoc />
        public async Task UnscheduleAsync(List<TriggerKey> triggerKeys)
        {
            foreach (var scheduleKey in triggerKeys)
            {
                await UnscheduleAsync(scheduleKey);
                await Log.TraceAsync($"Schedule task stopped '{scheduleKey.Name}'");
            }

            triggerKeys.Clear();
        }

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            lock (_runningJobCounterLock)
            {
                _isDisposed = true;
            }

            if (_scheduler != null)
            {
                await _scheduler.Shutdown();
                _scheduler = null;
            }

            if (_busyScheduled is not null)
            {
                _busyScheduled.Clear();
            }

            lock (_runningJobCounterLock)
            {
                while (_runningJobCounter > 0)
                {
                    Monitor.Wait(_runningJobCounterLock);
                }
            }
        }

        /// <inheritdoc />
        IDisposable IRecurringJobContext.TrackJobExecution()
        {
            lock (_runningJobCounterLock)
            {
                if (_isDisposed)
                {
                    return null;
                }

                _runningJobCounter++;

                Monitor.PulseAll(_runningJobCounterLock);
            }

            return new JobExecutionTracker(this);
        }

        /// <summary>
        ///     Уменьшает кол-во текущих вполняемых задач при завершении одной
        /// </summary>
        private void OnJobExecuted()
        {
            lock (_runningJobCounterLock)
            {
                _runningJobCounter--;

                Monitor.PulseAll(_runningJobCounterLock);
            }
        }

        /// <summary>
        ///     Отслеживание выполнения задач
        /// </summary>
        private sealed class JobExecutionTracker : IDisposable
        {
            private readonly RecurringJobSchedulerImpl _scheduler;

            public JobExecutionTracker(RecurringJobSchedulerImpl scheduler)
            {
                _scheduler = scheduler;
            }

            public void Dispose()
            {
                _scheduler.OnJobExecuted();
            }
        }
    }
}
