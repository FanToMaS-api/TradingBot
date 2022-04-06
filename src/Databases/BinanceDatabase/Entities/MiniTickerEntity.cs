using BinanceDatabase.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BinanceDatabase.Entities
{
    /// <summary>
    ///     Усеченная модель 24-часового скользящего окна по символу
    /// </summary>
    [Table("mini_tickers")]
    public class MiniTickerEntity
    {
        /// <summary>
        ///     Уникальный идентификатор записи
        /// </summary>
        [Key]
        [Column("id")]
        public int Id { get; set; }

        /// <inheritdoc />
        [Column("short_name")]
        public string ShortName { get; set; }

        /// <inheritdoc />
        [Column("event_time")]
        public DateTime EventTime { get; set; }

        /// <inheritdoc />
        [Column("interval")]
        public AggregateDataIntervalType IntervalType { get; set; }

        /// <inheritdoc />
        [Column("close_price")]
        public double ClosePrice { get; set; }

        /// <inheritdoc />
        [Column("open_price")]
        public double OpenPrice { get; set; }

        /// <inheritdoc />
        [Column("min_price")]
        public double MinPrice { get; set; }

        /// <inheritdoc />
        [Column("max_price")]
        public double MaxPrice { get; set; }

        /// <inheritdoc />
        [Column("base_purchase_volume")]
        public double BasePurchaseVolume { get; set; }

        /// <inheritdoc />
        [Column("quote_purchase_volume")]
        public double QuotePurchaseVolume { get; set; }

        #region Setup

        /// <summary>
        ///     Настройка
        /// </summary>
        public static void Setup(EntityTypeBuilder<MiniTickerEntity> builder)
        {
            // Индексы
            builder.HasIndex(_ => _.ShortName).HasDatabaseName("IX_mini_tickers_short_name");
            builder.HasIndex(_ => _.EventTime).HasDatabaseName("IX_mini_tickers_event_time");
            builder.HasIndex(_ => _.IntervalType).HasDatabaseName("IX_mini_tickers_interval");
            builder.HasIndex(_ => _.ClosePrice).HasDatabaseName("IX_mini_tickers_close_price");
            builder.HasIndex(_ => _.OpenPrice).HasDatabaseName("IX_mini_tickers_open_price");
            builder.HasIndex(_ => _.MinPrice).HasDatabaseName("IX_mini_tickers_min_price");
            builder.HasIndex(_ => _.MaxPrice).HasDatabaseName("IX_mini_tickers_max_price");
        }

        #endregion
    }
}
