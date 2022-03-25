using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Models;

namespace Telegram.Client.Impl
{

    /// <inheritdoc cref="ITelegramClient"/>
    public class TelegramClient : ITelegramClient
    {
        #region Fields

        private readonly ITelegramBotClient _client;

        #endregion

        #region .ctor

        /// <inheritdoc cref="ITelegramClient"/>
        public TelegramClient(string token)
        {
            _client = new TelegramBotClient(token);
        }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public async Task SendMessageAsync(TelegramMessageModel messageModel, CancellationToken cancellationToken)
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
