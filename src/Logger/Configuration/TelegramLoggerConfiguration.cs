namespace Logger.Configuration
{
    /// <summary>
    ///     Класс конфигурации логгера с функций отправки логов в телеграмм
    /// </summary>
    internal class TelegramLoggerConfiguration
    {
        /// <summary>
        ///     Название блока настроек
        /// </summary>
        public static string Name => "TelegramLoggerConfiguration";

        /// <summary>
        ///     Id чата, в который будут отправляться логи
        /// </summary>
        public string ChatId { get; set; }
    }
}
