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

        /// <summary>
        ///     Проверяет состоит ли пользователь в указанном канале
        /// </summary>
        /// <param name="channelId"> Id канала </param>
        /// <param name="userId"> Id пользователя </param>
        /// <param name="cancellationToken"> Токен отмены </param>
        /// <returns>
        ///     <see langword="true"/> если пользователь подписан на канал
        /// </returns>
        Task<bool> IsInChannelAsync(long channelId, long userId, CancellationToken cancellationToken);
    }
}
