using BinanceDatabase.Repositories.HotRepositories;

namespace BinanceDatabase.Repositories
{
    /// <summary>
    ///     Хранилище репозиториев для доступа к таблицам необходимым в первую очередь
    /// </summary>
    public interface IHotUnitOfWork
    {
        /// <summary>
        ///     Самые свежие данные по ценах тикеров
        /// </summary>
        IHotMiniTickerRepository HotMiniTickers { get; }
    }
}
