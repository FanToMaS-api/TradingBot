using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BinanceDatabase.Entities
{
    /// <summary>
    ///     Таблица предсказаний цен
    /// </summary>
    [Table("predictions")]
    public class PredictionEntity
    {
        #region Properties

        /// <summary>
        ///     Уникальный идентификатор записи
        /// </summary>
        [Key]
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        ///     Короткое название пары
        /// </summary>
        [Column("short_name")]
        public string ShortName { get; set; }

        /// <summary>
        ///     Время начала предсказания
        /// </summary>
        [Column("prediction_time")]
        public DateTime PredictionTime { get; set; }

        /// <summary>
        ///     Значения предсказанной цены
        /// </summary>
        [Column("price_values")]
        public double[] PriceValues { get; set; }

        #endregion

        #region Setup

        /// <summary>
        ///     Настройка
        /// </summary>
        public static void Setup(EntityTypeBuilder<PredictionEntity> builder)
        {
            // Индексы
            builder.HasIndex(_ => _.ShortName).HasDatabaseName("IX_predictions_short_name");
            builder.HasIndex(_ => _.PredictionTime).HasDatabaseName("IX_predictions_prediction_time");
        }

        #endregion
    }
}
