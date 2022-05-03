using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TelegramServiceDatabase
{
    /// <summary>
    ///     Design-time фабрика для <see cref="TelegramDbContext"/>
    /// </summary>
    public class TelegramDbContextFactory : IDesignTimeDbContextFactory<TelegramDbContext>
    {
        /// <inheritdoc />
        public TelegramDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TelegramDbContext>();
            optionsBuilder.UseNpgsql("Server=localhost;UserID=postgres;Port=5432;Database=Telegram_TB_Database;Password=0000;Include Error Detail=true");

            return new TelegramDbContext(optionsBuilder.Options);
        }
    }
}
