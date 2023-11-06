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
using System.ComponentModel;
using System.Threading;

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
            
            // Extract all Sort Columns
            var sortColumns = DataGridUtils.GetSortColumns(request);

            // Extract all Grid Filters
            var filters = FilterState.Filters.Values.ToList();

            var parameters = new ODataQueryParametersBuilder()
                .Page(Pagination.CurrentPageIndex + 1, Pagination.ItemsPerPage)
                .Filter(filters)
                .OrderBy(sortColumns)
                .Build();

            // Get the Data:
            var entities = await ODataService.GetEntitiesAsync<UserTask>("", parameters, default);


        }

        private DataServiceQuery<UserTask> GetDataServiceQuery(List<SortColumn> sortColumns, List<FilterDescriptor> filters,  int pageNumber, int pageSize)
        {
            var httpRequestMessage = new HttpRequestMessageBuilder("UserTasks", HttpMethod.Get);

            return (DataServiceQuery<UserTask>)query;
        }
    }
}
