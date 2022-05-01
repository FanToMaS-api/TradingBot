using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TelegramServiceDatabase.Entities;

namespace TelegramServiceDatabase.Repositories
{
    /// <summary>
    ///     Репозиторий состояний пользователя
    /// </summary>
    public interface IUserStateRepository
    {
        /// <summary>
        ///     Создать запрос к таблице состояний пользователей
        /// </summary>
        IQueryable<UserStateEntity> CreateQuery();

        /// <summary>
        ///     Получить состояние пользователя по его Id
        /// </summary>
        Task<UserStateEntity> GetAsync(long userId, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Создать состояние пользователя
        /// </summary>
        Task<UserStateEntity> CreateAsync(Action<UserStateEntity> action, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Изменить состояние пользователя
        /// </summary>
        Task<UserStateEntity> UpdateAsync(long id, Action<UserStateEntity> action, CancellationToken cancellationToken = default);
    }
}
