// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using RebacExperiments.Blazor.Components;
using RebacExperiments.Blazor.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Fast.Components.FluentUI;
using Microsoft.OData.Client;
using RebacExperiments.Shared.Models;
using RebacExperiments.Blazor.Shared.Http;
using RebacExperiments.Blazor.Infrastructure;
using RebacExperiments.Blazor.Shared.Extensions;

namespace RebacExperiments.Blazor.Pages
{
    public partial class UserTasksDataGrid
    {
        /// <summary>
        /// Provides the Data Items.
        /// </summary>
        private GridItemsProvider<UserTask> UserTasksProvider = default!;

        /// <summary>
        /// DataGrid.
        /// </summary>
        private FluentDataGrid<UserTask> DataGrid = default!;

        /// <summary>
        /// The current Pagination State.
        /// </summary>
        private readonly PaginationState Pagination = new() { ItemsPerPage = 10 };

        /// <summary>
        /// The current Filter State.
        /// </summary>
        private readonly FilterState FilterState = new();

        /// <summary>
        /// Reacts on Paginator Changes.
        /// </summary>
        private readonly EventCallbackSubscriber<FilterState> CurrentFiltersChanged;

        public UserTasksDataGrid()
        {
            CurrentFiltersChanged = new(EventCallback.Factory.Create<FilterState>(this, RefreshData));
        }

        protected override Task OnInitializedAsync()
        {
            UserTasksProvider = async request =>
            {
                var response = await GetUserTasks(request);

                return GridItemsProviderResult.From(items: response.ToList(), totalItemCount: (int)response.Count);
            };
            
            return base.OnInitializedAsync();
        }

        /// <inheritdoc />
        protected override Task OnParametersSetAsync()
        {
            // The associated filter state may have been added/removed/replaced
            CurrentFiltersChanged.SubscribeOrMove(FilterState.CurrentFiltersChanged);

            return Task.CompletedTask;
        }

        private Task RefreshData()
        {
            return DataGrid.RefreshDataAsync();
        }

        private async Task<QueryOperationResponse<UserTask>> GetUserTasks(GridItemsProviderRequest<UserTask> request)
        {
            var sorts = DataGridUtils.GetSortColumns(request);
            var filters = FilterState.Filters.Values.ToList();

            var dataServiceQuery = GetDataServiceQuery(sorts, filters, Pagination.CurrentPageIndex, Pagination.ItemsPerPage);

            var result = await dataServiceQuery.ExecuteAsync(request.CancellationToken);

            return (QueryOperationResponse<UserTask>)result;
        }

        private DataServiceQuery<UserTask> GetDataServiceQuery(List<SortColumn> sortColumns, List<FilterDescriptor> filters,  int pageNumber, int pageSize)
        {
            var httpRequestMessage = new HttpRequestMessageBuilder("UserTasks", HttpMethod.Get);

            ODataService.GetEntitiesAsync<UserTask>("",)

            var query = Container.UserTasks
                .Page(pageNumber + 1, pageSize)
                .Filter(filters)
                .SortBy(sortColumns)
                .IncludeCount(true);

            return (DataServiceQuery<UserTask>)query;
        }
    }
}
