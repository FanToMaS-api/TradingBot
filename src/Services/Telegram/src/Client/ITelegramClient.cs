using System.Threading;
using System.Threading.Tasks;
using Telegram.Models;

namespace Telegram.Client
{
    /// <summary>
    ///     Клиент для работы с Telegram
    /// </summary>
    public interface ITelegramClient
    {
        /// <summary>
        ///     Отправляет сообщение в указанный чат
        /// </summary>
        Task SendMessageAsync(TelegramMessageModel messageModel, CancellationToken cancellationToken);
    }
}
