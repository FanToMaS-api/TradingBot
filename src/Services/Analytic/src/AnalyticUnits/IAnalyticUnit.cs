using Common.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Analytic.AnalyticUnits
{
    /// <summary>
    ///     Обозначает элемент занимающийся анализом торгового актива
    /// </summary>
    public interface IAnalyticUnit
    {
        /// <summary>
        ///     Анализирует торговый актив
        /// </summary>
        Task<AnalyticResultModel> AnalyzeAsync(string name, CancellationToken cancellationToken);
    }
}
