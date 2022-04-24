using System.IO;
using Telegram.Bot.Types;
using Telegram.Models;

namespace Telegram.Builder
{
    /// <summary>
    ///     Строитель сообщений для отправки в телеграм
    /// </summary>
    public class TelegramMessageBuilder
    {
        #region Fields

        // оставлены для Markdown разметки только '*' и '`'
        private static readonly string[] SymbolsToReplace = { ".", "-", "(", ")", "!", "#", "_", "|", "{", "}", "[", "]", "~", "=", "+", ">" };
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
        public void SetMessageText(string message)
        {
            // экранирую специальные символы
            foreach (var symbol in SymbolsToReplace)
            {
                message = message.Replace(symbol, $"\\{symbol}");
            }

            _messageModel.MessageText = message;
        }

        /// <summary>
        ///     Установить inline кнопку
        /// </summary>
        public void SetInlineButton(string buttonText, string url)
        {
            _messageModel.Type = MessageType.WithInlineButton;
            _messageModel.InlineKeyboardButton = new InlineKeyboardButtonModel(buttonText, url);
        }

        /// <summary>
        ///     Удалить inline кнопку
        /// </summary>
        public void DeleteInlineButton()
        {
            _messageModel.Type = MessageType.Default;
            _messageModel.InlineKeyboardButton = null;
        }

        /// <summary>
        ///     Установить изображение
        /// </summary>
        /// <param name="pathToImage"> Путь к нужному изображению </param>
        public void SetImage(string pathToImage)
        {
            _messageModel.Type = MessageType.WithImage;
            var imageStream = System.IO.File.OpenRead(pathToImage);
            var mediaPhoto = new InputMediaPhoto(new InputMedia(imageStream, Path.GetFileNameWithoutExtension(pathToImage)));
            _messageModel.Image = mediaPhoto;
        }

        /// <summary>
        ///     Удалить изображение
        /// </summary>
        public void DeleteImage()
        {
            _messageModel.Type = MessageType.Default;
            _messageModel.Image = null;
        }

        /// <summary>
        ///     Возвращает софрмированную модель сообщения
        /// </summary>
        public TelegramMessageModel GetResult() => _messageModel;

        #endregion
    }
}
