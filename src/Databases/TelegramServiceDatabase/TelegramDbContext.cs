using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Npgsql.NameTranslation;
using TelegramServiceDatabase.Entities;

namespace TelegramServiceDatabase
{
    /// <summary>
    ///     Контекст базы данных
    /// </summary>
    public class TelegramDbContext : DbContext
    {
        #region .ctor

        /// <summary>
        ///     Инициализация контекста базы данных
        /// </summary>
        public TelegramDbContext(DbContextOptions<TelegramDbContext> options) : base(options)
        { }

        #endregion

        #region Properties

        /// <summary>
        ///     Пользователи бота
        /// </summary>
        public DbSet<UserEntity> Users { get; set; }

        /// <summary>
        ///     Состояния пользователей
        /// </summary>
        public DbSet<UserStateEntity> UsersStates { get; set; }

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
            UserEntity.Setup(modelBuilder.Entity<UserEntity>());
            UserStateEntity.Setup(modelBuilder.Entity<UserStateEntity>());
        }

        #endregion
    }
}
