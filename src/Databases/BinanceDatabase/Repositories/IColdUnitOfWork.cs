using BinanceDatabase.Repositories.ColdRepositories;

namespace BinanceDatabase.Repositories
{
    /// <summary>
    ///     Хранилище репозиториев для доступа к  таблицам с "холодными" данными
    /// </summary>
    public interface IColdUnitOfWork
    {
        /// <summary>
        ///     Репозиторий таблицы усеченных данных о торговом объекте
        /// </summary>
        IMiniTickerRepository MiniTickers { get; }
    }
}
