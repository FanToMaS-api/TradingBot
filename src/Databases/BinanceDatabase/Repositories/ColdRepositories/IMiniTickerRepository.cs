using BinanceDatabase.Entities;

namespace BinanceDatabase.Repositories.ColdRepositories
{
    /// <summary>
    ///     Репозиторий таблицы усеченных данных о торговом объекте
    /// </summary>
    public interface IMiniTickerRepository : IRepositoryBase<MiniTickerEntity>
    {
    }
}
