using System;
using System.Threading.Tasks;

namespace DataServiceLibrary
{
    /// <summary>
    ///     Сервис по приему и обработке данных
    /// </summary>
    public interface IDataService : IDisposable
    {
        /// <summary>
        ///     Обработчик данных
        /// </summary>
        public IDataHandler DataHandler { get; }

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
