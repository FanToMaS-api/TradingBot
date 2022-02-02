using System.Threading;
using System.Threading.Tasks;

namespace ExchangeLibrary
{
    /// <summary>
    ///     Общий интерфейс для всех бирж
    /// </summary>
    public interface IExchange
    {
        /// <summary>
        ///     Получить всю информацию о монетах
        /// </summary>
        Task<string> GetAllCoinsInformationAsync(long recvWindow, CancellationToken cancellationToken);
    }
}
