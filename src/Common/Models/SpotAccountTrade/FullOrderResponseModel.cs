using System.Collections.Generic;

namespace Common.Models
{
    /// <summary>
    ///     Модель ответа на отправку нового ордера (содержит полную информацию)
    /// </summary>
    public class FullOrderResponseModel : OrderResponseModelBase
    {
        /// <summary>
        ///     Время исполнения транзакции
        /// </summary>
        public long TransactTimeUnix { get; internal set; }

        /// <summary>
        ///     Части заполнения ордера
        /// </summary>
        public List<FillModel> Fills { get; internal set; } = new();
    }
}
