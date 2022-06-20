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
        private static readonly string[] SymbolsToReplace =
            { ".", "-", "(", ")", "!", "#", "_", "|", "{", "}", "[", "]", "~", "=", "+", ">" };
        private TelegramMessageModel _messageModel = new();

        #endregion

        #region Public methods

        /// <summary>
        ///     Сбрасывает настройки сообщения
        /// </summary>
        public TelegramMessageBuilder Reset()
        {
            _messageModel = new();

            return this;
        }

        /// <summary>
        ///     Установить id чата
        /// </summary>
        public TelegramMessageBuilder SetChatId(long chatId)
        {
            _messageModel.ChatId = chatId;

            return this;
        }

        /// <summary>
        ///     Установить id чата
        /// </summary>
        public TelegramMessageBuilder SetChatId(string chatId)
        {
            _messageModel.ChatId = long.Parse(chatId);
            
            return this;
        }

        /// <summary>
        ///     Установить текст сообщения
        /// </summary>
        public TelegramMessageBuilder SetMessageText(string message)
        {
            // экранирую специальные символы
            foreach (var symbol in SymbolsToReplace)
            {
                message = message.Replace(symbol, $"\\{symbol}");
            }

            _messageModel.MessageText = message;

            return this;
        }

        /// <summary>
        ///     Установить inline кнопку
        /// </summary>
        public TelegramMessageBuilder SetInlineButton(string buttonText, string url)
        {
            _messageModel.Type = MessageType.WithInlineButton;
            _messageModel.InlineKeyboardButton = new InlineKeyboardButtonModel(buttonText, url);

            return this;
        }

        /// <summary>
        ///     Удалить inline кнопку
        /// </summary>
        public TelegramMessageBuilder DeleteInlineButton()
        {
            _messageModel.Type = MessageType.Default;
            _messageModel.InlineKeyboardButton = null;
            
            return this;
        }

        /// <summary>
        ///     Установить изображение
        /// </summary>
        /// <param name="pathToImage"> Путь к нужному изображению </param>
        public TelegramMessageBuilder SetImage(string pathToImage)
        {
            _messageModel.Type = MessageType.WithImage;
            var imageStream = System.IO.File.OpenRead(pathToImage);
            var mediaPhoto = new InputMediaPhoto(new InputMedia(imageStream, Path.GetFileNameWithoutExtension(pathToImage)));
            _messageModel.Image = mediaPhoto;
            
            return this;
        }

        /// <summary>
        ///     Удалить изображение
        /// </summary>
        public TelegramMessageBuilder DeleteImage()
        {
            _messageModel.Type = MessageType.Default;
            _messageModel.Image = null;
            
            return this;
        }

        /// <summary>
        ///     Возвращает софрмированную модель сообщения
        /// </summary>
        public TelegramMessageModel GetResult() => _messageModel;

        #endregion
    }
}
