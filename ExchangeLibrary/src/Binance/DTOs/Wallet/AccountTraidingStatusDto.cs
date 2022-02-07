using Newtonsoft.Json;

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
        [JsonProperty("data")]
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
        [JsonProperty("isLocked")]
        public bool IsLocked { get; set; }

        /// <summary>
        ///     Если торговля запрещена, указывает время до ее восстановления
        /// </summary>
        [JsonProperty("plannedRecoverTime")]
        public long PlannedRecoverTimeUnix { get; set; }

        /// <summary>
        ///     Содержит инфу об ордерах 
        /// </summary>
        [JsonProperty("triggerCondition")]
        public TriggerConditionDto TriggerCondition { get; set; }

        /// <summary>
        ///     Время обновления
        /// </summary>
        [JsonProperty("updateTime")]
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
        [JsonProperty("GCR")]
        public int GCR { get; set; }

        /// <summary>
        ///     Количество ордеров FOK/IOC
        /// </summary>
        [JsonProperty("IFER")]
        public int IFER { get; set; }

        /// <summary>
        ///     Количество ордеров
        /// </summary>
        [JsonProperty("UFR")]
        public int UFR { get; set; }
    }
}
