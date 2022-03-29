namespace Common.Models
{
    /// <summary>
    ///     Модель ответа на запрос отмены ордера
    /// </summary>
    public class CancelOrderResponseModel : OrderResponseModelBase
    {
        /// <summary>
        ///     ID ордера, назначенный пользователем или сгенерированный
        /// </summary>
        public string OrigClientOrderId { get; internal set; }
    }
}
