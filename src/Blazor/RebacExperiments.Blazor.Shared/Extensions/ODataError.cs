using System.Text.Json;
using System.Text.Json.Serialization;

namespace RebacExperiments.Blazor.Shared.Extensions
{
    /// <summary>
    /// Error sent by the OData service. All properties are marked as optional, 
    /// because we shouldn't trust the implementations.
    /// </summary>
    public class ODataError
    {
        /// <summary>Gets or sets the error code to be used in payloads.</summary>
        [JsonPropertyName("code")]
        public string? ErrorCode { get; set; }

        /// <summary>Gets or sets the error message.</summary>
        [JsonPropertyName("message")]
        public string? Message { get; set; }

        /// <summary>Gets or sets the target of the particular error.</summary>
        [JsonPropertyName("target")]
        public string? Target { get; set; }

        /// <summary>
        /// A collection of JSON objects that MUST contain name/value pairs for code and message, and MAY contain a name/value pair for target, as described above.
        /// </summary>
        [JsonPropertyName("details")]
        public List<ODataErrorDetail>? Details { get; set; }

        /// <summary>Gets or sets the implementation specific debugging information to help determine the cause of the error.</summary>
        [JsonPropertyName("innerError")]
        public ODataInnerError? InnerError { get; set; }

        [JsonPropertyName("instanceAnnotations")]
        public List<ODataInstanceAnnotation>? InstanceAnnotations { get; set; }

        [JsonPropertyName("typeAnnotation")]
        public ODataTypeAnnotation? TypeAnnotation { get; set; }

        /// <summary>
        /// Gets or sets extension data sent by the service.
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, JsonElement>? ExtensionData { get; set; }
    }
}
