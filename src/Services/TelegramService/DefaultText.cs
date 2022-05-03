namespace TelegramService
{
    /// <summary>
    ///     Стандартные тексты для ответов пользователям
    /// </summary>
    internal class DefaultText
    {
        /// <summary>
        ///     Телеграм-аккаунт для вопросов и предложений
        /// </summary>
        private readonly static string HelpAccount = "@Misha_Dulov";

        /// <summary>
        ///     Ссылка на канал в телеграмме
        /// </summary>
        private readonly static string CannelLink = "t.me/binance_signals_M";

        /// <summary>
        ///     Сообщение о забаненном аккаунте пользователя
        /// </summary>
        public static string BannedAccountText = $"Ваш аккаунт был забанен. По всем вопросам пишите {HelpAccount}";

        /// <summary>
        ///     Приветственный текст
        /// </summary>
        public static string WelcomeText = "Привет!\nЯ могу присылать графики прогнозов.\n" +
            $"Чтобы получить прогноз нужно:\n" +
            $"1) Быть подписанным на канал {CannelLink}\n" +
            "2) Прислать название пары. (BTC/ETH, btc usdt, btcBUSD)";

        /// <summary>
        ///     Сообщение о необходимости подписки на канал
        /// </summary>
        public static string RequiredSubscriptionText = $"Чтобы получать личные прогнозы необходимо подписаться на канал {CannelLink}";

        /// <summary>
        ///     Сообщение о неудачном анализе пары
        /// </summary>
        public static string UnsuccessfulAnalyzeText = "Анализ оказался неудачным. Возможно, ошибка в названии пары";
    }
}
