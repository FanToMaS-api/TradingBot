using System;
using System.Threading.Tasks;

namespace TelegramService
{
    /// <summary>
    ///     Сервис для работы с пользователями в телеграмме
    /// </summary>
    public interface ITelegramService : IDisposable
    {
        /// <summary>
        ///     Запускает прием и обработку сообщений от пользователей
        /// </summary>
        Task StartAsync();
    }
}
