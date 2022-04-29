using Telegram.Bot.Types;

namespace Telegram.Models
{
    /// <summary>
    ///     Модель сообщения для телеграмма
    /// </summary>
    public class TelegramMessageModel
    {
        /// <summary>
        ///     Тип сообщения
        /// </summary>
        public MessageType Type { get; set; }
        
        /// <summary>
        ///     Id чата для отправки
        /// </summary>
        public long ChatId { get; set; }

        /// <summary>
        ///     Тект сообщения
        /// </summary>
        public string MessageText { get; set; }

        /// <summary>
        ///     Картинки
        /// </summary>
        public InputMediaPhoto Image { get; set; }

        /// <summary>
        ///     Кнопка для перехода по ссылке внизу сообщения
        /// </summary>
        public InlineKeyboardButtonModel InlineKeyboardButton { get; set; }
    }
}
