using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;

namespace Common
{
    /// <summary>
    ///     Компонент настраивающий сервисы и службы
    /// </summary>
    public sealed class InitializationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _services;

        private readonly object _syncRoot = new();
        private Task _initializationTask;

        [UsedImplicitly]
        public InitializationMiddleware(RequestDelegate next, IServiceProvider services)
        {
            _next = next;
            _services = services;
        }

        [UsedImplicitly]
        public async Task Invoke(HttpContext context)
        {
            Task task;
            lock (_syncRoot)
            {
                if (_initializationTask == null)
                {
                    _initializationTask = Task.Run(InitializeAsync);
                }

                task = _initializationTask;
            }

            await task;

            await _next.Invoke(context);
        }

        private async Task InitializeAsync()
        {
            await AppInitializer.InitializeAsync(_services);
        }
    }
}
