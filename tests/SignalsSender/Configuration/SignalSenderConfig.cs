using Common.Models;

namespace SignalsSender.Configuration
{
    /// <summary>
    ///     Настройки сервиса уведомлений
    /// </summary>
    public class SignalSenderConfig : OptionsBase
    {
        /// <inheritdoc />
        public override string Name => "Settings";

        /// <summary>
        ///     Id канала в телеграмме
        /// </summary>
        public string ChannelId { get; set; } = string.Empty;
    }
}
