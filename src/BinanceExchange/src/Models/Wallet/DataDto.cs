using System.Text.Json.Serialization;

namespace BinanceExchange.Models
{
    /// <summary>
    ///     API trading status detail
    /// </summary>
    internal class DataDto
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
}
