using Analytic.Models;
using ExchangeLibrary;
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
        ///     Уникальное название элемента анализа
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Анализирует объект торговли
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns> 
        ///     <see langword="true, model"/> Если объект стоит купить <br/>
        ///     <see langword="false"/> Если объект не стоит покупать
        /// </returns>
        Task<(bool isSuccessfulAnalyze, AnalyticResultModel resultModel)> TryAnalyzeAsync(
            IExchange exchange,
            InfoModel model,
            CancellationToken cancellationToken);

        /// <summary>
        ///     Удаляет анализатор или профиль анализаторов
        /// </summary>
        /// <returns> True, если удаление прошло успешно </returns>
        bool Remove(string name);
    }
}
