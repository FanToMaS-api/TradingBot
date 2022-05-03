namespace TelegramServiceDatabase.Types
{
    /// <summary>
    ///     Причины бана
    /// </summary>
    public enum BanReasonType
    {
        /// <summary>
        ///     Не забанен
        /// </summary>
        NotBanned,

        /// <summary>
        ///     Спам
        /// </summary>
        Spam,

        /// <summary>
        ///     Поведение, нарушающее правила бота
        /// </summary>
        BadBehavior,

        /// <summary>
        ///     Общая причина бана
        /// </summary>
        Common
    }
}
