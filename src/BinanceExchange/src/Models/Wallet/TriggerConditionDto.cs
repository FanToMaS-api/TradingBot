using System.Text.Json.Serialization;

namespace BinanceExchange.Models
{
    /// <summary>
    ///     Содержит инфу об ордерах 
    /// </summary>
    internal class TriggerConditionDto
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
