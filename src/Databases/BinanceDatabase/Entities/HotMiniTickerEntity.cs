using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BinanceDatabase.Entities
{
    /// <summary>
    ///     Содержит только нужную информацию для расчетов о тикера
    /// </summary>
    [Table("hot_mini_tickers")]
    public class HotMiniTickerEntity
    {
        /// <summary>
        ///     Уникальный идентификатор записи
        /// </summary>
        [Key]
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        ///     Название пары
        /// </summary>
        [Required]
        [Column("pair")]
        public string Pair { get; set; }

        /// <summary>
        ///     Цена
        /// </summary>
        [Required]
        [Column("price")]
        public double Price { get; set; }

        /// <summary>
        ///     Время получения
        /// </summary>
        [Required]
        [Column("received_time")]
        public DateTime ReceivedTime { get; set; }

        #region Setup

        /// <summary>
        ///     Настройка
        /// </summary>
        public static void Setup(EntityTypeBuilder<HotMiniTickerEntity> builder)
        {
            // Индексы
            builder.HasIndex(_ => _.Id).IsUnique().HasDatabaseName("IX_hot_mini_tickers_id");
            builder.HasIndex(_ => _.Pair).HasDatabaseName("IX_hot_mini_tickers_pair");
            builder.HasIndex(_ => _.ReceivedTime).HasDatabaseName("IX_hot_mini_tickers_received_time");
        }

        #endregion
    }
}
