// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Diagnostics.CodeAnalysis;

namespace RebacExperiments.Blazor.Shared.Extensions
{
    /// <summary>
    /// OData Extensions to simplify working with a Grid in a WinUI 3 application.
    /// </summary>
    public static class ODataExtensions
    {
        /// <summary>
        /// Gets the @odata.count value or <see cref="0"/>, if none was found.
        /// </summary>
        /// <param name="response">OData Response with Metadata included</param>
        /// <returns>Value of the "@odata.count" Metadata</returns>
        public static long Count(this ODataResponse response)
        {
            if(!TryGetCount(response.Metadata, out var count))
            {
                return 0;
            }

            return count.Value;
        }

        private static bool TryGetCount(IDictionary<string, object?> metadata, [NotNullWhen(true)] out long? count)
        {
            count = null;

            if (!metadata.TryGetValue("@odata.count", out object? value))
            {
                return false;
            }

            if(value == null)
            {
                return false;
            }

            count = value as long?;

            if(count == null)
            {
                return false;
            }

            return true;
        }
    }
}
