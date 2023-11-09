using RebacExperiments.Blazor.Shared.Extensions;

namespace RebacExperiments.Blazor.Shared.Models
{
    public class ODataQueryParameters
    {
        /// <summary>
        /// Gets or sets the number of elements to skip.
        /// </summary>
        public int? Skip { get; set; } = null;

        /// <summary>
        /// Gets or sets the number of elements to take.
        /// </summary>
        public int? Top { get; set; } = null;

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
        private int? _skip;
        private int? _top;
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
