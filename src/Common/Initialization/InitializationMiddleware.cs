using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;

namespace Common.Initialization
{
    /// <summary>
    ///     Компонент, настраивающий сервисы и службы
    /// </summary>
    public sealed class InitializationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _services;
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
            if (_initializationTask == null)
            {
                // TODO: ПРОТЕСТИРОВАТЬ!
                /*await ??*/
                Interlocked.CompareExchange(ref _initializationTask, Task.Run(InitializeAsync), null);
            }

            // TODO: ПРОТЕСТИРОВАТЬ!
            await _initializationTask;

            await _next.Invoke(context);
        }

        private async Task InitializeAsync()
        {
            await AppInitializer.InitializeAsync(_services);
        }
    }
}
