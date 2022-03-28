using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    ///     Настройки инициализации приложения
    /// </summary>
    internal sealed class AppInitializerOptions
    {
        /// <summary>
        ///     Действия при инициализации
        /// </summary>
        public List<Func<IServiceProvider, Task>> InitActions { get; } = new();
    }
}
