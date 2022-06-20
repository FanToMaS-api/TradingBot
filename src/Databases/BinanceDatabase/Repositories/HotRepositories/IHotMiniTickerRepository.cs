﻿using BinanceDatabase.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceDatabase.Repositories.HotRepositories
{
    /// <summary>
    ///     Репозиторий таблицы "горячих" усеченных данных о торговом объекте
    /// </summary>
    public interface IHotMiniTickerRepository : IRepositoryBase<HotMiniTickerEntity>
    {
        /// <summary>
        ///     Множественное добавление сущностей
        /// </summary>
        Task AddRangeAsync(IEnumerable<HotMiniTickerEntity> miniTickerEntities, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Получить необходимое кол-во самых актуальных объектов, отсортированные в порядке получения
        /// </summary>
        Task<HotMiniTickerEntity[]> GetArrayAsync(string pair, CancellationToken cancellationToken, int neededCount = 2000);

        /// <summary>
        ///     Удаляет все записи до указанной даты
        /// </summary>
        /// <remarks>
        ///     Контекст сохраняется автоматически внутри функции
        /// </remarks>
        Task<int> RemoveUntilAsync(DateTime until, CancellationToken cancellationToken);
    }
}
