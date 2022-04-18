using BinanceDatabase;
using Microsoft.EntityFrameworkCore;
using System;

namespace SharedForTest
{
    /// <summary>
    ///     Локальная база для тестов
    /// </summary>
    public class DatabaseFixture : IDisposable
    {
        #region Fields

        private const string ConnectionString = "Server=localhost;UserID=postgres;Port=5432;Database=Test_Database;" +
            "Password=0000;Include Error Detail=true";

        #endregion

        #region .ctor

        /// <inheritdoc cref="DatabaseFixture"/>
        public DatabaseFixture()
        {
            using var db = CreateDbContext();
            db.Database.Migrate();
        }

        #endregion

        public AppDbContext CreateDbContext()
        {
            var optionBuilder = new DbContextOptionsBuilder();
            optionBuilder.UseNpgsql(ConnectionString);
            var db = new AppDbContext(optionBuilder.Options);

            return db;
        }

        public void Dispose()
        {
            using var db = CreateDbContext();
            if (!db.Database.EnsureDeleted())
            {
                throw new Exception("Failed to delete database");
            }
        }
    }
}
