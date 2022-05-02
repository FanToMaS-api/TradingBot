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

        public static string WelcomeText = "Привет!\nЯ могу присылать графики прогнозов.\n" +
            $"1) Чтобы получить прогноз надо быть подписанным на канал {CannelLink}\n" +
            "2) Прислать название пары. (BTC/ETH, btc usdt, btcBUSD)";

        /// <summary>
        ///     Сообщение о неудачном анализе пары
        /// </summary>
        public static string UnsuccessfulAnalyze = "Анализ оказался неудачным, проверь правильность названия пары";
    }
}
