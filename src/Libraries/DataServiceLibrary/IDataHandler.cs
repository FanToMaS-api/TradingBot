using System;
using System.Threading;
using System.Threading.Tasks;

namespace DataServiceLibrary
{
    /// <summary>
    ///     Занимается получением и обработкой данных
    /// </summary>
    public interface IDataHandler : IDisposable
    {
        /// <summary>
        ///     Запускает прием и обработку данных
        /// </summary>
        Task StartAsync(CancellationToken cancellationToken);

        /// <summary>
        ///     Останавливает прием данных
        /// </summary>
        Task StopAsync();
    }
}
