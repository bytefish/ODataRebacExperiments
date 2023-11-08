// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace RebacExperiments.Blazor.Shared.Extensions
{
    public class ODataResponse
    {
        /// <summary>
        /// Gets or sets the Status, such as a 200 (OK) or 400 (Bad Request).
        /// </summary>
        public required int Status { get; set; }

        /// <summary>
        /// Gets or sets the Metadata, such as @odata.count.
        /// </summary>
        public Dictionary<string, object?> Metadata { get; set; } = new();

        /// <summary>
        /// Gets or sets the Headers.
        /// </summary>
        public Dictionary<string, List<string>> Headers { get; set; } = new();
    }

    public class ODataEntityResponse<TEntityType> : ODataResponse
    {
        public required TEntityType? Entity { get; set; }
    }

    public class ODataEntitiesResponse<TEntityType> : ODataResponse
    {
        public required List<TEntityType>? Entities { get; set; }
    }

    public class ODataResponseParserOptions
    {
        /// <summary>
        /// Gets or sets the option for using case-insensitive property name comparisms.
        /// </summary>
        public bool CaseInsensitivePropertyNames { get; set; } = true;
    }
}
