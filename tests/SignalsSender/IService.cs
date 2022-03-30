using System.Threading.Tasks;

namespace SignalsSender
{
    /// <summary>
    ///     Сервис для предварительного тестирования логики
    /// </summary>
    public interface IService
    {
        /// <summary>
        ///     Запускает сервис
        /// </summary>
        Task RunAsync();
    }
}