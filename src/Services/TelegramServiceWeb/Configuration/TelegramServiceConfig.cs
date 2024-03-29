﻿namespace TelegramServiceWeb.Configuration
{
    /// <summary>
    ///     Конфигурация сервиса для работы с пользователями в телеграмме
    /// </summary>
    internal class TelegramServiceConfig
    {
        /// <summary>
        ///     Название блока настроек
        /// </summary>
        public string Name => "TelegramService";

        /// <summary>
        ///     Id канала, на который необходимо пользователю подписаться
        /// </summary>
        public long ChannelId { get; set; }
    }
}
