using Telegram.Models;

namespace Telegram.Builder
{
    /// <summary>
    ///     Строитель сообщений для отправки в телеграм
    /// </summary>
    public class TelegramMessageBuilder
    {
        #region Fields

        private TelegramMessageModel _messageModel = new();

        #endregion

        #region Public methods

        /// <summary>
        ///     Сбрасывает настройки сообщения
        /// </summary>
        public void Reset() => _messageModel = new();

        /// <summary>
        ///     Установить id чата
        /// </summary>
        public void SetChatId(long chatId) => _messageModel.ChatId = chatId;

        /// <summary>
        ///     Установить id чата
        /// </summary>
        public void SetChatId(string chatId) => _messageModel.ChatId = long.Parse(chatId);

        /// <summary>
        ///     Установить текст сообщения
        /// </summary>
        public void SetMessageText(string message) => _messageModel.MessageText = message;

        /// <summary>
        ///     Установить inline кнопку
        /// </summary>
        public void SetInlineButton(string buttonText, string url)
        {
            _messageModel.Type = MessageType.WithInlineButton;
            _messageModel.InlineKeyboardButton = new InlineKeyboardButtonModel(buttonText, url);
        }

        /// <summary>
        ///     Удалить inline конпку
        /// </summary>
        public void DeleteInlineButton()
        {
            _messageModel.Type = MessageType.Default;
            _messageModel.InlineKeyboardButton = null;
        }

        /// <summary>
        ///     Возвращает софрмированную модель сообщения
        /// </summary>
        public TelegramMessageModel GetResult() => _messageModel;

        #endregion
    }
}
