using TelegramServiceDatabase.Repositories;

namespace TelegramServiceDatabase
{
    /// <summary>
    ///     Фабрика по созданию контекста базы данных с заданной областью видимости
    /// </summary>
    public interface ITelegramDatabaseFactory
    {
        /// <summary>
        ///     Создать подключение к БД
        /// </summary>
        ITelegramDbUnitOfWork CreateScopeDatabase();
    }
}
