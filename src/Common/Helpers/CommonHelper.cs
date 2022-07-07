using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Helpers
{
    /// <summary>
    ///     Класс, содерщажий общие расширения для разных типов
    /// </summary>
    public static class CommonHelper
    {
        /// <summary>
        ///     Возвращает процентное отклонение новой цены от старой
        /// </summary>
        public static double GetPercentDeviation(double oldPrice, double newPrice)
            => (newPrice / (double)oldPrice - 1) * 100;

        /// <summary>
        ///     Проверят объект на пустоту, выбрасывает исключение в случае если он неинициализирован
        /// </summary>
        /// <param name="value"> Значение </param>
        /// <exception cref="ArgumentNullException"> Если параметр пуст или не инициализирован </exception>
        public static void ThrowIsEmptyOrNull(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException($"{nameof(value)} can not be null or empty");
            }
        }

        /// <summary>
        ///     Выполнеяет указанное действие для каждого элемента из коллекции
        /// </summary>
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection)
            {
                action(item);
            }
        }

        /// <summary>
        ///     Проверят объекты на пустоту,
        ///     выбрасывает исключение в случае если какой-либо из объектов списка пуст или неинициализирован
        /// </summary>
        /// <param name="collection"> Значение </param>
        /// <exception cref="ArgumentNullException"> Если параметр пуст или не инициализирован </exception>
        public static void ThrowIsAnyEmptyOrNull<T>(this IEnumerable<T> collection)
        {
            collection.ThrowIsNull();

            foreach (var item in collection)
            {
                if (item is null)
                {
                    throw new ArgumentNullException($"{nameof(item)} can not be null");
                }

                if (item is string && string.IsNullOrEmpty(item as string))
                {
                    throw new ArgumentNullException($"{nameof(item)} is a string can not be null or empty");
                }
            }
        }

        /// <summary>
        ///     Проверят объект на пустоту, выбрасывает исключение в случае если он неинициализирован
        /// </summary>
        /// <param name="collection"> Значение </param>
        /// <exception cref="ArgumentNullException"> Если параметр пуст или не инициализирован </exception>
        public static void ThrowIsNull<T>(this IEnumerable<T> collection)
        {
            if (collection is null)
            {
                throw new ArgumentNullException($"{nameof(collection)} can not be null");
            }
        }
    }
}
