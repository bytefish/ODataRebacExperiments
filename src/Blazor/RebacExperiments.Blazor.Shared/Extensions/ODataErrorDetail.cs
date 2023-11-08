using System.Text.Json;
using System.Text.Json.Serialization;

namespace RebacExperiments.Blazor.Shared.Extensions
{
    /// <summary>
    /// Class representing an error detail.
    /// </summary>
    public class ODataErrorDetail
    {
        /// <summary>
        /// Gets or sets the error code to be used in payloads.
        /// </summary>
        [JsonPropertyName("errorCode")]
        public string? ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        [JsonPropertyName("message")]
        public string? Message { get; set; }

        /// <summary>
        /// Gets or sets the target of the particular error. For example, the name of the property in error.
        /// </summary>
        [JsonPropertyName("target")]
        public string? Target { get; set; }

        /// <summary>
        /// Gets or sets extension data sent by the service.
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, JsonElement>? ExtensionData { get; set; }
    }
}
