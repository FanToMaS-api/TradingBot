using System.Text.Json.Serialization;

namespace ExchangeLibrary.Binance.DTOs.Wallet
{
    /// <summary>
    ///    Модель трейдинг статуса аккаунта 
    /// </summary>
    public class AccountTraidingStatusDto
    {
        #region Properties

        /// <summary>
        ///     Содержит инфу об аккаунте
        /// </summary>
        [JsonPropertyName("data")]
        public DataDto Data { get; set; }

        #endregion
    }

    /// <summary>
    ///     API trading status detail
    /// </summary>
    public class DataDto
    {
        #region Properties

        /// <summary>
        ///     Заблокирован ли трейдер
        /// </summary>
        [JsonPropertyName("isLocked")]
        public bool IsLocked { get; set; }

        /// <summary>
        ///     Если торговля запрещена, указывает время до ее восстановления
        /// </summary>
        [JsonPropertyName("plannedRecoverTime")]
        public long PlannedRecoverTimeUnix { get; set; }

        /// <summary>
        ///     Содержит инфу об ордерах 
        /// </summary>
        [JsonPropertyName("triggerCondition")]
        public TriggerConditionDto TriggerCondition { get; set; }

        /// <summary>
        ///     Время обновления
        /// </summary>
        [JsonPropertyName("updateTime")]
        public long UpdateTimeUnix { get; set; }

        #endregion
    }

    /// <summary>
    ///     Содержит инфу об ордерах 
    /// </summary>
    public class TriggerConditionDto
    {
        /// <summary>
        ///     Количество ордеров GCR
        /// </summary>
        [JsonPropertyName("GCR")]
        public int GCR { get; set; }

        /// <summary>
        ///     Количество ордеров FOK/IOC
        /// </summary>
        [JsonPropertyName("IFER")]
        public int IFER { get; set; }

        /// <summary>
        ///     Количество ордеров
        /// </summary>
        [JsonPropertyName("UFR")]
        public int UFR { get; set; }
    }
}
