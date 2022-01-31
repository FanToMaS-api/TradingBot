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
        ///     Получить цену определенной монеты
        /// </summary>
        Task<double> GetCryptoPrice(string cryptoName, CancellationToken cancellationToken);
    }
}
