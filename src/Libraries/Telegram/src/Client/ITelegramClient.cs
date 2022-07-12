using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Models;

namespace Telegram.Client
{
    /// <summary>
    ///     Клиент для работы с Telegram
    /// </summary>
    public interface ITelegramClient
    {
        /// <summary>
        ///     Получить получатель обновлений в очереди
        /// </summary>
        /// <remarks>
        ///     Если получатель обновлений был уже создан, возвращает существующий экземпляр
        /// </remarks>
        QueuedUpdateReceiver GetUpdateReceiver(UpdateType[] allowedUpdates);

        /// <summary>
        ///     Удаляет сообщение из чата
        /// </summary>
        /// <param name="channelId"> Id канала </param>
        /// <param name="messageId"> Id сообщения, которое хотим удалить </param>
        /// <param name="cancellationToken"> Токен отмены </param>
        Task DeleteMessageAsync(long channelId, int messageId, CancellationToken cancellationToken);

        /// <summary>
        ///     Отправляет блок сообщений за раз
        /// </summary>
        /// <param name="models"> Модели сообщений для отправки </param>
        Task SendManyMessagesAsync(IEnumerable<TelegramMessageModel> models, CancellationToken cancellationToken);

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
