using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Helpers
{
    /// <summary>
    ///     Хелпер для коллекций
    /// </summary>
    public static class CollectionsHelper
    {
        /// <summary>
        ///     Асинхронно проверяет заданное условие
        /// </summary>
        public static async Task<bool> TrueForAllAsync<T>(this IEnumerable<T> collection, Func<T, Task<bool>> predicate)
        {
            foreach (var item in collection)
            {
                if (!await predicate(item))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
