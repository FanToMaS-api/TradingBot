using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
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
        public async Task SendMessageAsync(TelegramMessageModel messageModel, CancellationToken cancellationToken)
        {
            if (messageModel.Type == Models.MessageType.WithImage)
            {
                messageModel.Image.Caption = messageModel.MessageText;
                messageModel.Image.ParseMode = ParseMode.MarkdownV2;

                await _client.SendMediaGroupAsync(
                    messageModel.ChatId,
                    new IAlbumInputMedia[] { messageModel.Image },
                    cancellationToken: cancellationToken);

                return;
            }

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

        /// <inheritdoc />
        public async Task<bool> IsInChannelAsync(long channelId, long userId, CancellationToken cancellationToken)
            => await _client.GetChatMemberAsync(channelId, userId, cancellationToken) is not null;

        #endregion
    }
}
