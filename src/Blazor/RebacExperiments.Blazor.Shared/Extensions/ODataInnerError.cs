using System.Text.Json;
using System.Text.Json.Serialization;

namespace RebacExperiments.Blazor.Shared.Extensions
{
    public class ODataInnerError
    {
        /// <summary>
        /// Properties to be written for the inner error.
        /// </summary>
        [JsonPropertyName("properties")]
        public IDictionary<string, ODataValue> Properties { get; private set; }

        /// <summary>Gets or sets the error message.</summary>
        [JsonPropertyName("message")]
        public string? Message { get; set; }

        /// <summary>Gets or sets the type name of this error, for example, the type name of an exception.</summary>

        [JsonPropertyName("typeName")]
        public string? TypeName { get; set; }

        /// <summary>Gets or sets the stack trace for this error.</summary>
        [JsonPropertyName("stackTrace")]
        public string? StackTrace { get; set; }

        /// <summary>Gets or sets the nested implementation specific debugging information. </summary>
        [JsonPropertyName("innerError")]
        public ODataInnerError? InnerError { get; set; }

        /// <summary>
        /// Gets or sets extension data sent by the service.
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, JsonElement>? ExtensionData { get; set; }

    }
}
