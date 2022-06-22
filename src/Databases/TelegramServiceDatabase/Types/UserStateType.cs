namespace TelegramServiceDatabase.Types
{
    /// <summary>
    ///     Тип состояния пользователя
    /// </summary>
    public enum UserStatusType
    {
        /// <summary>
        ///     Активный
        /// </summary>
        Active,

        /// <summary>
        ///     Заблокировал бота
        /// </summary>
        BlockedBot,

        /// <summary>
        ///     Неактивный
        /// </summary>
        Inactive,

        /// <summary>
        ///     Забанен
        /// </summary>
        Banned,

        /// <summary>
        ///     Оплачена подписка
        /// </summary>
        PaidForSubscription,

        /// <summary>
        ///     Не оплачена подписка
        /// </summary>
        NotPaidForSubscription,
    }
}
