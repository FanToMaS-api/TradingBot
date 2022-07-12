using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Configuration;
using Telegram.Models;

namespace Telegram.Client.Impl
{
    /// <inheritdoc cref="ITelegramClient"/>
    internal class TelegramClient : ITelegramClient
    {
        #region Fields

        private const int MessagesNumberInOneSending = 4;
        private readonly ITelegramBotClient _client;
        private QueuedUpdateReceiver _updateReceiver;

        #endregion

        #region .ctor

        /// <inheritdoc cref="ITelegramClient"/>
        public TelegramClient(TelegramOptions telegramOptions)
        {
            _client = new TelegramBotClient(telegramOptions.Token);
        }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public QueuedUpdateReceiver GetUpdateReceiver(UpdateType[] allowedUpdates)
        {
            if (_updateReceiver is not null)
            {
                return _updateReceiver;
            }

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = allowedUpdates
            };

            _updateReceiver = new QueuedUpdateReceiver(_client, receiverOptions);

            return _updateReceiver;
        }

        /// <inheritdoc />
        public async Task DeleteMessageAsync(long channelId, int messageId, CancellationToken cancellationToken)
        {
            await _client.DeleteMessageAsync(channelId, messageId, cancellationToken);
        }

        /// <inheritdoc />
        public async Task SendManyMessagesAsync(IEnumerable<TelegramMessageModel> models, CancellationToken cancellationToken)
        {
            var pagesCount = (int)Math.Ceiling(models.Count() / (double)MessagesNumberInOneSending);
            var tasks = new List<Task>();
            for (var page = 0; page < pagesCount; page++)
            {
                var modelsToSend = models
                    .Skip(MessagesNumberInOneSending * page)
                    .Take(MessagesNumberInOneSending)
                    .ToArray();

                for (var i = 0; i < modelsToSend.Length; i++)
                {
                    tasks.Add(SendMessageAsync(modelsToSend[i], cancellationToken));

                }

                await Task.WhenAll(tasks);
                tasks.Clear();

                // принудительная задержка чтобы телеграм не отбивал запросы
                await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
            }
        }

        /// <inheritdoc />
        public async Task SendMessageAsync(TelegramMessageModel messageModel, CancellationToken cancellationToken)
        {
            switch (messageModel.Type)
            {
                case Models.MessageType.Default:
                    await SendTextMessageAsync(messageModel, cancellationToken);
                    break;
                case Models.MessageType.WithInlineButton:
                    await SendMessageWithInlineButtonAsync(messageModel, cancellationToken);
                    break;
                case Models.MessageType.WithImage:
                    await SendMessageWithImageAsync(messageModel, cancellationToken);
                    break;
                default:
                    throw new NotImplementedException($"{nameof(messageModel.Type)} = {messageModel.Type}");
            };
        }

        /// <inheritdoc />
        public async Task<bool> IsInChannelAsync(long channelId, long userId, CancellationToken cancellationToken)
        {
            var user = (await _client.GetChatMemberAsync(channelId, userId, cancellationToken)).User;

            return user is not null;
        }

        #endregion

        #region Private methods

        /// <summary>
        ///     Отправить обычное текстовое сообщение
        /// </summary>
        private async Task SendTextMessageAsync(TelegramMessageModel messageModel, CancellationToken cancellationToken)
        {
            await _client.SendTextMessageAsync(
                messageModel.ChatId,
                messageModel.MessageText,
                parseMode: ParseMode.MarkdownV2,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        ///     Отправить сообщение с встроенной кнопкой
        /// </summary>
        private async Task SendMessageWithInlineButtonAsync(TelegramMessageModel messageModel, CancellationToken cancellationToken)
        {
            await _client.SendTextMessageAsync(
                messageModel.ChatId,
                messageModel.MessageText,
                parseMode: ParseMode.MarkdownV2,
                replyMarkup: new InlineKeyboardMarkup(
                    InlineKeyboardButton.WithUrl(
                            messageModel.InlineKeyboardButton.ButtonText,
                            messageModel.InlineKeyboardButton.Url)),
                cancellationToken: cancellationToken);
        }

        /// <summary>
        ///     Отправить сообщение с изображением
        /// </summary>
        private async Task SendMessageWithImageAsync(TelegramMessageModel messageModel, CancellationToken cancellationToken)
        {
            messageModel.Image.Caption = messageModel.MessageText;
            messageModel.Image.ParseMode = ParseMode.MarkdownV2;

            await _client.SendMediaGroupAsync(
                messageModel.ChatId,
                new IAlbumInputMedia[] { messageModel.Image },
                cancellationToken: cancellationToken);
        }

        #endregion
    }
}
