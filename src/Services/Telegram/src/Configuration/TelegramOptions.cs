using Common.Models;

namespace Telegram.Configuration
{
    /// <summary>
    ///     Конфигурация для телеграмма
    /// </summary>
    internal class TelegramOptions : OptionsBase
    {
        /// <inheritdoc />
        public override string Name => "Telegram";

        /// <summary>
        ///     Токен бота
        /// </summary>
        public string Token { get; set; }
    }
}
