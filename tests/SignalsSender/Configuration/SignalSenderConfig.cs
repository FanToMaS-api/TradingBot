namespace SignalsSender.Configuration
{
    public class SignalSenderConfig
    {
        /// <summary>
        ///     Название блока настроек
        /// </summary>
        public const string Name = "Settings";

        /// <summary>
        ///     Токен
        /// </summary>
        public string ApiKey { get; set; } = string.Empty;

        /// <summary>
        ///     Секретный токен
        /// </summary>
        public string SecretKey { get; set; } = string.Empty;

        /// <summary>
        ///     Токен для телеграмма
        /// </summary>
        public string TelegramToken { get; set; } = string.Empty;

        /// <summary>
        ///     Id канала в телеграмме
        /// </summary>
        public string ChannelId { get; set; } = string.Empty;
    }
}
