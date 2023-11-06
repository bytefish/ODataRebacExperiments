// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using RebacExperiments.Blazor.Shared.Models;
using Microsoft.OData.Client;
using RebacExperiments.Blazor.Shared.Http;
using System.Globalization;

namespace RebacExperiments.Blazor.Shared.Extensions
{
    /// <summary>
    /// OData Extensions to simplify working with a Grid in a WinUI 3 application.
    /// </summary>
    public static class ODataExtensions
    {
        /// <summary>
        /// Adds the $top and $skip clauses to the <see cref="DataServiceQuery"/> to add pagination.
        /// </summary>
        /// <remarks>
        /// The <paramref name="pageNumber"/> starts with 1.
        /// </remarks>
        /// <typeparam name="TElement">Entity to query for</typeparam>
        /// <param name="httpRequestMessageBuilder">The <see cref="HttpRequestMessageBuilder"/> to modify</param>
        /// <param name="pageNumber">Page Number (starting with 1)</param>
        /// <param name="pageSize">Page size</param>
        /// <returns><see cref="DataServiceQuery"/> with Pagination</returns>
        public static void SetPage<TElement>(HttpRequestMessageBuilder httpRequestMessageBuilder, int pageNumber, int pageSize)
        {
            var skip = (pageNumber - 1) * pageSize;
            var top = pageSize;

            httpRequestMessageBuilder.SetQueryString("$skip", skip.ToString(CultureInfo.InvariantCulture));
            httpRequestMessageBuilder.SetQueryString("$top", top.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Adds a $filter clause to a <see cref="HttpRequestMessageBuilder"/>.
        /// </summary>
        /// <typeparam name="TElement">Entity to Filter</typeparam>
        /// <param name="httpRequestMessageBuilder">DataServiceQuery to add the $filter clause to</param>
        /// <param name="filters">Filters to apply</param>
        /// <returns><see cref="DataServiceQuery"/> with filtering</returns>
        public static void Filter<TElement>(HttpRequestMessageBuilder httpRequestMessageBuilder, List<FilterDescriptor> filters)
        {
            if (filters.Count == 0)
            {
                return;
            }

            httpRequestMessageBuilder.SetQueryString("$filter", ODataUtils.Translate(filters));
        }

        /// <summary>
        /// Adds the $orderby clause to a <see cref="DataServiceQuery"/>.
        /// </summary>
        /// <typeparam name="TElement">Entity to Query for</typeparam>
        /// <param name="dataServiceQuery">DataServiceQuery to add the $orderby clause to</param>
        /// <param name="columns">Columns to sort</param>
        /// <returns><see cref="DataServiceQuery"/> with sorting</returns>
        public static void SortBy<TElement>(HttpRequestMessageBuilder httpRequestMessageBuilder, List<SortColumn> columns)
        {
            var sortColumns = GetOrderByColumns(columns);

            if (!string.IsNullOrWhiteSpace(sortColumns))
            {
                httpRequestMessageBuilder.SetQueryString("$orderby", sortColumns);
            }
        }

        /// <summary>
        /// Sorts the DataGrid by the specified column, updating the column header to reflect the current sort direction.
        /// </summary>
        /// <param name="columns">The Columns to sort.</param>
        public static string GetOrderByColumns(List<SortColumn> columns)
        {
            var sortColumns = columns
                // We need a Tag with the OData Path:
                .Where(column => column.PropertyName != null)
                // Turn into OData string:
                .Select(column =>
                {
                    var sortDirection = column.SortDirection == SortDirectionEnum.Descending ? "desc" : "asc";

                    return $"{column.PropertyName} {sortDirection}";
                });

            return string.Join(",", sortColumns);
        }
    }
}
