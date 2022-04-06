using System;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace DataServiceLibrary
{
    /// <summary>
    ///     Сервис по приему и обработке данных
    /// </summary>
    public interface IDataService : IDisposable
    {
        /// <summary>
        ///     Обработчики данных
        /// </summary>
        public ImmutableArray<IDataHandler> DataHandlers { get; }

        /// <summary>
        ///     Запускает сервис по приему и обработке данных
        /// </summary>
        Task StartAsync();

        /// <summary>
        ///     Останавливает сервис по приему и обработке данных
        /// </summary>
        Task StopAsync();
    }
}
