using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TelegramServiceDatabase.Entities;

namespace TelegramServiceDatabase.Repositories
{
    /// <summary>
    ///     Репозиторий пользователей бота
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        ///     Создать запрос к таблице пользователей
        /// </summary>
        IQueryable<UserEntity> CreateQuery();

        /// <summary>
        ///     Удалить пользователя
        /// </summary>
        void Remove(UserEntity entity);

        /// <summary>
        ///     Получить пользователя по его id
        /// </summary>
        Task<UserEntity> GetAsync(long id, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Создать пользователя
        /// </summary>
        Task<UserEntity> CreateAsync(Action<UserEntity> action, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Изменить пользователя
        /// </summary>
        Task<UserEntity> UpdateAsync(long id, Action<UserEntity> action, CancellationToken cancellationToken = default);
    }
}
