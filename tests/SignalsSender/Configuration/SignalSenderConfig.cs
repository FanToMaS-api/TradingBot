using Common.Models;

namespace SignalsSender.Configuration
{
    /// <summary>
    ///     Настройки сервиса уведомлений
    /// </summary>
    public class SignalSenderConfig
    {
        /// <inheritdoc />
        public static string Name => "Settings";

        /// <summary>
        ///     Id канала в телеграмме
        /// </summary>
        public string ChannelId { get; set; } = string.Empty;
    }
}
