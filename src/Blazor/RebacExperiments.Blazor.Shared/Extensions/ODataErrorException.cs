// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace RebacExperiments.Blazor.Shared.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public class ODataException : Exception
    {
        public ODataException()
        {
        }

        public ODataException(string? message) : base(message)
        {
        }

        public ODataException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }

    public class ODataErrorException : Exception
    {
        /// <summary>
        /// Gets or sets the <see cref="ODataError"/> for the invalid request.
        /// </summary>
        public required ODataError ODataError { get; set; }

        public ODataErrorException()
        {
        }

        public ODataErrorException(string? message) : base(message)
        {
        }

        public ODataErrorException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
