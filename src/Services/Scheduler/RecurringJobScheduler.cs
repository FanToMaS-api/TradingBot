using System;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Scheduler;

namespace Scraper.Common.Schedule
{
    /// <summary>
    ///     Инициализатор повторяющихся задач
    /// </summary>
    public static class RecurringJobScheduler
    {
        /// <summary>
        ///     Добавляет сервис повторяющихся задач по расписанию
        /// </summary>
        public static void AddRecurringJobScheduler(this IServiceCollection services)
        {
            if (services.Any(_ => _.ServiceType == typeof(IRecurringJobScheduler)))
            {
                return;
            }

            services.AddSingleton<RecurringJobSchedulerImpl>();
            services.AddSingleton<IRecurringJobScheduler>(_ => _.GetRequiredService<RecurringJobSchedulerImpl>());

            services.ConfigureForInitialization<RecurringJobSchedulerImpl>(_ => _.InitializeAsync());
        }

        /// <summary>
        ///     Настраивает повторяющуюся задачу
        /// </summary>
        public static void ConfigureRecurringJob(
            this IServiceCollection services,
            Cron cronExpression,
            Func<IServiceProvider, Task> action)
        {
            CronExpression.ValidateExpression(cronExpression);

            services.AddRecurringJobScheduler();

            services.Configure<RecurringJobSchedulerOptions>(
                options =>
                {
                    options.RecurringJobs.Add(
                        new RecurringJobDefinition(cronExpression, action)
                    );
                }
            );
        }

        /// <summary>
        ///     Настраивает повторяющуюся задачу
        /// </summary>
        public static void ConfigureRecurringJob<T>(
            this IServiceCollection services,
            Cron cronExpression,
            Func<T, IServiceProvider, Task> action)
        {
            ConfigureRecurringJob(services, cronExpression, sp => action(sp.GetRequiredService<T>(), sp));
        }

        /// <summary>
        ///     Настраивает повторяющуюся задачу
        /// </summary>
        public static void ConfigureRecurringJob<T>(
            this IServiceCollection services,
            Cron cronExpression,
            Func<T, Task> action)
        {
            ConfigureRecurringJob<T>(services, cronExpression, (service, _) => action(service));
        }

        /// <summary>
        ///     Запланировать задачу
        /// </summary>
        public static Task<TriggerKey> ScheduleAsync<T>(
            this IRecurringJobScheduler scheduler,
            Cron cronExpression,
            Func<T, IServiceProvider, Task> action,
            string name = default)
        {
            return scheduler.ScheduleAsync(cronExpression, sp => action(sp.GetRequiredService<T>(), sp), name);
        }

        /// <summary>
        ///     Запланировать задачу
        /// </summary>
        public static Task<TriggerKey> ScheduleAsync<T>(
            this IRecurringJobScheduler scheduler,
            Cron cronExpression,
            Func<T, Task> action,
            string name = default)
        {
            return scheduler.ScheduleAsync<T>(cronExpression, (service, _) => action(service), name);
        }
    }
}
