﻿using Analytic.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Analytic.Filters
{
    /// <summary>
    ///     Группа фильтров
    /// </summary>
    public interface IFilterGroup
    {
        /// <summary>
        ///     Название группы фильтров
        /// </summary>
        public string FilterGroupName { get; }

        /// <summary>
        ///     Фильтры
        /// </summary>
        List<IFilter> Filters { get; }

        /// <summary>
        ///     Тип фильтруемой группы
        /// </summary>
        FilterGroupType Type { get; }

        /// <summary>
        ///     Проверяет модель на соответствие условиям
        /// </summary>
        Task<bool> CheckConditionsAsync(IServiceScopeFactory serviceScopeFactory, InfoModel model, CancellationToken cancellationToken);

        /// <summary>
        ///     Добавить фильтр
        /// </summary>
        void AddFilter(IFilter filter);

        /// <summary>
        ///     Удалить фильтр
        /// </summary>
        /// <returns> True - если произошло успешное удаление </returns>
        bool RemoveFilter(IFilter filter);

        /// <summary>
        ///     Показывает фильтрует ли указанный фильтр этот объект торговли
        /// </summary>
        bool IsFilter(string tradeObjectName);

        /// <summary>
        ///     Проверяет содержит ли данная группа с таким фильтром
        /// </summary>
        /// <returns> True - если группа содержит фильтр </returns>
        bool ContainsFilter(string filterName);
    }
}
