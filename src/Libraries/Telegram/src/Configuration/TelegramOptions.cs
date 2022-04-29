namespace Telegram.Configuration
{
    /// <summary>
    ///     Конфигурация для телеграмма
    /// </summary>
    internal class TelegramOptions
    {
        /// <inheritdoc />
        public static string Name => "Telegram";

        /// <summary>
        ///     Токен бота
        /// </summary>
        public string Token { get; set; }
    }
}
