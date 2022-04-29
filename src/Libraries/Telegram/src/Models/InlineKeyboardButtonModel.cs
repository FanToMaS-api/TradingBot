namespace Telegram.Models
{
    /// <summary>
    ///     Модель кнопки внизу сообщения для перехода по ссылке
    /// </summary>
    public class InlineKeyboardButtonModel
    {
        /// <inheritdoc cref="InlineKeyboardButtonModel"/>
        /// <param name="text"> Текст кнопки </param>
        /// <param name="url"> Url, на который ведет кнопка </param>
        public InlineKeyboardButtonModel(string text, string url)
        {
            ButtonText = text;
            Url = url;
        }

        /// <summary>
        ///     Текст кнопки
        /// </summary>
        public string ButtonText { get; set; }

        /// <summary>
        ///     Url, на который ведет кнопка
        /// </summary>
        public string Url { get; set; }
    }
}
