using System;
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
        public async Task SendMessageAsync(TelegramMessageModel messageModel, CancellationToken cancellationToken)
        {
            switch (messageModel.Type)
            {
                case Models.MessageType.WithImage:
                    await SendMessageWithImage(messageModel, cancellationToken);
                    break;
                case Models.MessageType.WithInlineButton:
                    await SendMessageWithInlineButton(messageModel, cancellationToken);
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
        ///     Отправить сообщение с изображением
        /// </summary>
        private async Task SendMessageWithImage(TelegramMessageModel messageModel, CancellationToken cancellationToken)
        {
            messageModel.Image.Caption = messageModel.MessageText;
            messageModel.Image.ParseMode = ParseMode.MarkdownV2;

            await _client.SendMediaGroupAsync(
                messageModel.ChatId,
                new IAlbumInputMedia[] { messageModel.Image },
                cancellationToken: cancellationToken);
        }

        /// <summary>
        ///     Отправить сообщение с встроенной кнопкой
        /// </summary>
        private async Task SendMessageWithInlineButton(TelegramMessageModel messageModel, CancellationToken cancellationToken)
        {
            await _client.SendTextMessageAsync(
                messageModel.ChatId,
                messageModel.MessageText,
                parseMode: ParseMode.MarkdownV2,
                replyMarkup: messageModel.Type == Models.MessageType.WithInlineButton
                    ? new InlineKeyboardMarkup(
                        InlineKeyboardButton.WithUrl(
                            messageModel.InlineKeyboardButton.ButtonText,
                            messageModel.InlineKeyboardButton.Url))
                    : null,
                cancellationToken: cancellationToken);
        }

        #endregion
    }
}
