using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json.Bson;
using TelegramServiceDatabase.Types;

namespace TelegramServiceDatabase.Entities
{
    /// <summary>
    ///     Таблица состояний пользователей бота
    /// </summary>
    [Table("users_state")]
    public class UserStateEntity
    {
        #region Fields

        private int _warningNumber;

        #endregion

        #region Properties

        /// <summary>
        ///     Уникальное id состояния пользователя
        /// </summary>
        [Column("id")]
        public long Id { get; set; }

        /// <summary>
        ///     Id пользователя
        /// </summary>
        [Column("user_id")]
        public long UserId { get; set; }

        /// <summary>
        ///     Сам пользователь для связи
        /// </summary>
        public UserEntity User { get; set; }

        /// <summary>
        ///     Текущее состояние пользователя
        /// </summary>
        [Column("state_type")]
        public UserStateType UserStateType { get; set; }

        /// <summary>
        ///     Причина бана
        /// </summary>
        [Column("ban_reason")]
        public BanReasonType BanReason { get; set; }

        /// <summary>
        ///     Кол-во предупреждений полученных пользователем
        /// </summary>
        [Column("warning_number")]
        public int WarningNumber { get => _warningNumber; internal set => _warningNumber = value; }

        #endregion

        #region Public methods

        /// <summary>
        ///     Потокобезопасно увеличить кол-во предупреждений
        /// </summary>
        public void IncrementWarningNumbers()
        {
            Interlocked.Increment(ref _warningNumber);
        }

        /// <summary>
        ///     Потокобезопасно сбросить кол-во предупреждений
        /// </summary>
        public void ResetWarningNumbers()
        {
            // вычитаем его самого
            _warningNumber = Interlocked.Add(ref _warningNumber, -_warningNumber);
        }

        /// <summary>
        ///     Банит пользователя
        /// </summary>
        /// <param name="banReason"> Причина бана </param>
        internal void Ban(BanReasonType banReason)
        {
            UserStateType = UserStateType.Banned;
            BanReason = banReason;
        }

        /// <summary>
        ///     Убирает все предупреждения с пользователя
        /// </summary>
        internal void Unban()
        {
            UserStateType = UserStateType.Active;
            BanReason = BanReasonType.NotBanned;
        }

        #endregion

        #region Setup

        /// <summary>
        ///     Настройка
        /// </summary>
        public static void Setup(EntityTypeBuilder<UserStateEntity> builder)
        {
            // Ключ
            builder.HasKey(_ => _.Id);

            // Связи
            builder
                .HasOne(_ => _.User)
                .WithOne(_ => _.UserState)
                .HasForeignKey<UserStateEntity>(_ => _.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Индексы
            builder.HasIndex(_ => _.UserId).IsUnique().HasDatabaseName("IX_users_state_user_id");
            builder.HasIndex(_ => _.Id).IsUnique().HasDatabaseName("IX_users_state_id");
            builder.HasIndex(_ => _.UserStateType).HasDatabaseName("IX_users_state_state_type");
            builder.HasIndex(_ => _.BanReason).HasDatabaseName("IX_users_state_ban_reason");
            builder.HasIndex(_ => _.WarningNumber).HasDatabaseName("IX_users_state_warning_number");

            // конвертеры
            builder
                .Property(_ => _.UserStateType)
                .HasConversion(
                    v => v.ToString(),
                    v => string.IsNullOrEmpty(v) ? UserStateType.Active : (UserStateType)Enum.Parse(typeof(UserStateType), v));

            builder
                .Property(_ => _.BanReason)
                .HasConversion(
                    v => v.ToString(),
                    v => string.IsNullOrEmpty(v) ? BanReasonType.NotBanned : (BanReasonType)Enum.Parse(typeof(BanReasonType), v));
        }

        #endregion
    }
}
