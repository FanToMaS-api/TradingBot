using BinanceDatabase.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Npgsql.NameTranslation;
using System;

namespace BinanceDatabase
{
    /// <summary>
    ///     База данных приложения
    /// </summary>
    public class AppDbContext : DbContext
    {
        #region .ctor

        /// <inheritdoc cref="AppDbContext" />
        public AppDbContext(DbContextOptions options) : base(options) { }

        #endregion

        #region Tables

        /// <summary>
        ///     Таблица "горячих" данных для алгоритмов
        /// </summary>
        /// <remarks>
        ///     Содержит большое кол-во данных за короткий промежуток времени
        /// </remarks>
        public DbSet<HotMiniTickerEntity> HotMiniTickers { get; set; }

        /// <summary>
        ///     Таблица данных полученных со стрима минитикеров
        /// </summary>
        public DbSet<MiniTickerEntity> MiniTickers { get; set; }

        #endregion

        #region Public methods

        /// <inheritdoc />
        override protected void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Проставляем имя поля по умолчанию (snake_case)
            var mapper = new NpgsqlSnakeCaseNameTranslator();
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                var storeObjectId = StoreObjectIdentifier.Table(entity.GetTableName(), entity.GetSchema());
                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(mapper.TranslateMemberName(property.GetColumnName(storeObjectId)));
                }
            }

            // Setup
            HotMiniTickerEntity.Setup(modelBuilder.Entity<HotMiniTickerEntity>());
            MiniTickerEntity.Setup(modelBuilder.Entity<MiniTickerEntity>());
        }

        #endregion
    }
}
