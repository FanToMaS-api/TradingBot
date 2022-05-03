using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TelegramServiceDatabase.Entities
{
    /// <summary>
    ///     Таблица пользователей бота
    /// </summary>
    [Table("users")]
    public class UserEntity
    {
        #region Properties

        /// <summary>
        ///     Уникальный id
        /// </summary>
        [Column("id")]
        public long Id { get; set; }

        /// <summary>
        ///     Уникальный id в телеграмме
        /// </summary>
        [Column("telegram_id")]
        public long TelegramId { get; set; }

        /// <summary>
        ///     id чата в телеграмме
        /// </summary>
        [Column("chat_id")]
        public long ChatId { get; set; }

        /// <summary>
        ///     Ник пользователя
        /// </summary>
        [Column("nickname")]
        public string Nickname { get; set; }

        /// <summary>
        ///     Фамилия пользователя
        /// </summary>
        [Column("lastname")]
        public string LastName { get; set; }

        /// <summary>
        ///     Имя пользователя
        /// </summary>
        [Column("firstname")]
        public string FirstName { get; set; }

        /// <summary>
        ///     Отражает дату последнего действия пользователя
        /// </summary>
        [Column("last_action")]
        public DateTime LastAction { get; set; }

        /// <summary>
        ///     Св-во для навигации
        /// </summary>
        public UserStateEntity UserState { get; set; }

        #endregion

        #region Setup

        /// <summary>
        ///     Настройка
        /// </summary>
        public static void Setup(EntityTypeBuilder<UserEntity> builder)
        {
            // Ключ
            builder.HasKey(_ => _.Id);

            // Индексы
            builder.HasIndex(_ => _.Id).IsUnique().HasDatabaseName("IX_users_id");
            builder.HasIndex(_ => _.Nickname).HasDatabaseName("IX_users_nickname");
            builder.HasIndex(_ => _.TelegramId).IsUnique().HasDatabaseName("IX_users_telegram_id");
            builder.HasIndex(_ => _.ChatId).IsUnique().HasDatabaseName("IX_users_chat_id");
            builder.HasIndex(_ => _.FirstName).HasDatabaseName("IX_users_firstname");
            builder.HasIndex(_ => _.LastName).HasDatabaseName("IX_users_lastname");
            builder.HasIndex(_ => _.LastAction).HasDatabaseName("IX_users_last_action");
        }

        #endregion
    }
}
