namespace Common.Models
{
    /// <summary>
    ///     Модель ответа на запрос состояния ордера
    /// </summary>
    public class CheckOrderResponseModel : OrderResponseModelBase
    {
        /// <summary>
        ///     Стоп цена
        /// </summary>
        public double StopPrice { get; internal set; }

        /// <summary>
        ///     Кол-во для ордера-айсберга
        /// </summary>
        public double IcebergQty { get; internal set; }

        /// <summary>
        ///     Время
        /// </summary>
        public long TimeUnix { get; internal set; }

        /// <summary>
        ///     Время обновления
        /// </summary>
        public long UpdateTimeUnix { get; internal set; }

        /// <summary>
        ///     Открыт ли сейчас ордер
        /// </summary>
        public bool IsWorking { get; internal set; }

        /// <summary>
        ///     Кол-во для квотируемого ордера
        /// </summary>
        public double OrigQuoteOrderQty { get; internal set; }
    }
}
