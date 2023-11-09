// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using RebacExperiments.Blazor.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.Fast.Components.FluentUI;
using RebacExperiments.Blazor.Infrastructure;
using RebacExperiments.Shared.ApiSdk.Models;
using RebacExperiments.Blazor.Extensions;
using RebacExperiments.Blazor.Shared.Models;

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

                if(response == null)
                {
                    return GridItemsProviderResult.From(items: new List<UserTask>(), totalItemCount: 0);
                }

                var entities = response.Value;

                if (entities == null)
                {
                    return GridItemsProviderResult.From(items: new List<UserTask>(), totalItemCount: 0);
                }

                int count = response.GetODataCount();

                return GridItemsProviderResult.From(items: entities, totalItemCount: count);
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

        private async Task<UserTaskCollectionResponse?> GetUserTasks(GridItemsProviderRequest<UserTask> request)
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

            return await ApiClient.Odata.UserTasks.GetAsync(request =>
            {
                request.QueryParameters.Count = true;
                request.QueryParameters.Top = parameters.Top;
                request.QueryParameters.Skip = parameters.Skip;

                if(!string.IsNullOrWhiteSpace(parameters.Filter))
                {
                    request.QueryParameters.Filter = parameters.Filter;
                }

                if (!string.IsNullOrWhiteSpace(parameters.OrderBy))
                {
                    request.QueryParameters.Orderby = new[] { parameters.OrderBy };
                }
            }).ConfigureAwait(false);    
        }
    }
}
