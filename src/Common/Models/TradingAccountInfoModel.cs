﻿namespace Common.Models
{
    /// <summary>
    ///    Модель трейдинг информации об аккаунте
    /// </summary>
    public class TradingAccountInfoModel
    {
        /// <summary>
        ///     Заблокирован ли трейдер
        /// </summary>
        public bool IsLocked { get; internal set; }

        /// <summary>
        ///     Если торговля запрещена, указывает время до ее восстановления
        /// </summary>
        public long PlannedRecoverTimeUnix { get; internal set; }

        /// <summary>
        ///     Время обновления
        /// </summary>
        public long UpdateTimeUnix { get; internal set; }
    }
}

