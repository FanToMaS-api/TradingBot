namespace Telegram.Models
{
    /// <summary>
    ///     Тип сообщения
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        ///     Простое текстовое сообщение
        /// </summary>
        Default,

        /// <summary>
        ///     Сообщение с инлайн-кнопкой
        /// </summary>
        WithInlineButton,
    }
}
