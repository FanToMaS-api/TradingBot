using BinanceDatabase.Repositories;

namespace BinanceDatabase
{
    /// <summary>
    ///     Фабрика по созданию контекста базы данных с заданной областью видимости
    /// </summary>
    public interface IBinanceDbContextFactory
    {
        /// <summary>
        ///     Создать подключение к БД
        /// </summary>
        IUnitOfWork CreateScopeDatabase();
    }
}
