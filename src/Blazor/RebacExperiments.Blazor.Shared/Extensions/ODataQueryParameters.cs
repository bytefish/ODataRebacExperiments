using RebacExperiments.Blazor.Shared.Http;
using RebacExperiments.Blazor.Shared.Models;

namespace RebacExperiments.Blazor.Shared.Extensions
{
    public class ODataQueryParameters
    {
        /// <summary>
        /// Gets or sets the number of elements to skip.
        /// </summary>
        public long? Skip { get; set; } = null;

        /// <summary>
        /// Gets or sets the number of elements to take.
        /// </summary>
        public long? Top { get; set; } = null;

        /// <summary>
        /// Gets or sets the filter clause.
        /// </summary>
        public string? Filter { get; set; } = null;

        /// <summary>
        /// Gets or sets the order by clause.
        /// </summary>
        public string? OrderBy { get; set; } = null;

        /// <summary>
        /// Gets or sets the option to include the count (default: <see cref="true"/>).
        /// </summary>
        public bool IncludeCount { get; set; } = true;
    }

    public class ODataQueryParametersBuilder
    {
        private long? _skip;
        private long? _top;
        private string? _orderby;
        private string? _filter;

        public ODataQueryParametersBuilder Page(int pageNumber, int pageSize)
        { 
            _skip = (pageNumber - 1) * pageSize;
            _top = pageSize;
       
            return this;
        }

        public ODataQueryParametersBuilder Filter(List<FilterDescriptor> filterDescriptors)
        {
            _filter = ODataUtils.Translate(filterDescriptors);
            
            return this;
        }

        public ODataQueryParametersBuilder OrderBy(List<SortColumn> columns)
        {
            _orderby = GetOrderByColumns(columns);

            return this;
        }

        private string GetOrderByColumns(List<SortColumn> columns)
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

        public ODataQueryParameters Build()
        {
            return new ODataQueryParameters
            {
                Skip = _skip,
                Top = _top,
                OrderBy = _orderby,
                Filter = _filter,
            };
        }
    }
}
