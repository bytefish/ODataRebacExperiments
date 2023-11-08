// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.RateLimiting;
using RebacExperiments.Server.Api.Infrastructure.Authentication;
using RebacExperiments.Server.Api.Infrastructure.Constants;
using RebacExperiments.Server.Api.Infrastructure.Database;
using RebacExperiments.Server.Api.Infrastructure.Errors;
using RebacExperiments.Server.Api.Infrastructure.Logging;
using RebacExperiments.Shared.Models;
using RebacExperiments.Server.Api.Services;

namespace RebacExperiments.Server.Api.Controllers
{
    public class UserTasksController : ODataController
    {
        private readonly ILogger<UserTasksController> _logger;
        private readonly ApplicationErrorHandler _applicationErrorHandler;

        public UserTasksController(ILogger<UserTasksController> logger, ApplicationErrorHandler applicationErrorHandler)
        {
            _logger = logger;
            _applicationErrorHandler = applicationErrorHandler;
        }

        [Authorize(Policy = Policies.RequireUserRole)]
        [EnableRateLimiting(Policies.PerUserRatelimit)]
        public async Task<IActionResult> GetUserTask([FromServices] ApplicationDbContext context, [FromServices] IUserTaskService userTaskService, [FromODataUri(Name = "key")] int key, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            if (!ModelState.IsValid)
            {
                return _applicationErrorHandler.HandleInvalidModelState(HttpContext, ModelState);
            }

            try
            {
                var userTask = await userTaskService.GetUserTaskByIdAsync(context, key, User.GetUserId(), cancellationToken);

                return Ok(userTask);
            }
            catch (Exception ex)
            {
                return _applicationErrorHandler.HandleException(HttpContext, ex);
            }
        }

        [HttpGet]
        [Authorize(Policy = Policies.RequireUserRole)]
        [EnableRateLimiting(Policies.PerUserRatelimit)]
        public ActionResult GetUserTasks([FromServices] ApplicationDbContext context, [FromServices] IUserTaskService userTaskService, ODataQueryOptions<UserTask> queryOptions)
        {
            _logger.TraceMethodEntry();

            if (!ModelState.IsValid)
            {
                return _applicationErrorHandler.HandleInvalidModelState(HttpContext, ModelState);
            }

            try
            {
                var queryable = userTaskService.QueryUserTasks(context, User.GetUserId());

                return Ok(queryOptions.ApplyTo(queryable));
            }
            catch (Exception ex)
            {
                return _applicationErrorHandler.HandleException(HttpContext, ex);
            }
        }

        [HttpPost]
        [Authorize(Policy = Policies.RequireUserRole)]
        [EnableRateLimiting(Policies.PerUserRatelimit)]
        public async Task<IActionResult> PostUserTask([FromServices] ApplicationDbContext context, [FromServices] IUserTaskService userTaskService, [FromBody] UserTask userTask, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            if (!ModelState.IsValid)
            {
                return _applicationErrorHandler.HandleInvalidModelState(HttpContext, ModelState);
            }

            try
            {
                await userTaskService.CreateUserTaskAsync(context, userTask, User.GetUserId(), cancellationToken);

                return Created(userTask);
            }
            catch (Exception ex)
            {
                return _applicationErrorHandler.HandleException(HttpContext, ex);
            }
        }

        [HttpPut]
        [HttpPatch]
        [Authorize(Policy = Policies.RequireUserRole)]
        [EnableRateLimiting(Policies.PerUserRatelimit)]
        public async Task<IActionResult> PatchUserTask([FromServices] ApplicationDbContext context, [FromServices] IUserTaskService userTaskService, [FromODataUri] int key, [FromBody] Delta<UserTask> delta, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            if (!ModelState.IsValid)
            {
                return _applicationErrorHandler.HandleInvalidModelState(HttpContext, ModelState);
            }

            try
            {
                // Get the UserTask with the current values:
                var userTask = await userTaskService.GetUserTaskByIdAsync(context, key, User.GetUserId(), cancellationToken);

                // Patch the Values to it:
                delta.Patch(userTask);

                // Update the Values:
                await userTaskService.UpdateUserTaskAsync(context, userTask, User.GetUserId(), cancellationToken);

                return Updated(userTask);
            }
            catch (Exception ex)
            {
                return _applicationErrorHandler.HandleException(HttpContext, ex);
            }
        }

        
        [HttpDelete]
        [Authorize(Policy = Policies.RequireUserRole)]
        [EnableRateLimiting(Policies.PerUserRatelimit)]
        public async Task<IActionResult> DeleteUserTask([FromServices] ApplicationDbContext context, [FromServices] IUserTaskService userTaskService, [FromODataUri(Name = "key")] int key, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            if (!ModelState.IsValid)
            {
                return _applicationErrorHandler.HandleInvalidModelState(HttpContext, ModelState);
            }

            try
            {
                await userTaskService.DeleteUserTaskAsync(context, key, User.GetUserId(), cancellationToken);

                return StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception ex)
            {
                return _applicationErrorHandler.HandleException(HttpContext, ex);
            }
        }
    }
}