using Analytic.Models;
using System.Linq;

namespace Analytic.Filters
{
    /// <summary>
    ///     
    /// </summary>
    public class NameFilter : IFilter
    {
        /// <inheritdoc cref="NameFilter"/>
        public NameFilter(string filterName, string[] neededNames)
        {
            FilterName = filterName;
            NeededNames = neededNames;
        }

        /// <inheritdoc />
        public string FilterName { get; }

        /// <summary>
        ///     Нужные наименования моделей
        /// </summary>
        public string[] NeededNames { get; }

        /// <inheritdoc />
        public InfoModel[] Filter(InfoModel[] models) =>
            models.Where(m => NeededNames.Any(_ => m.TradeObjectName.Contains(_))).ToArray();
    }
}
