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

        /// <summary>
        ///     С картинкой (инлайн-кнопкой присутствует) TODO: подумать как разнести, чтобы не было связки
        /// </summary>
        WithImage,
    }
}
