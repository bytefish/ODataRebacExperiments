// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Mvc;
using RebacExperiments.Server.Api.Infrastructure.Resources;
using System.ComponentModel.DataAnnotations;

namespace RebacExperiments.Server.Api.Models
{
    /// <summary>
    /// A User Task.
    /// </summary>
    [ModelMetadataType(typeof(UserTaskMetadata))]
    public class UserTask : Entity
    {
        /// <summary>
        /// Gets or sets the Title.
        /// </summary>
        public required string Title { get; set; }

        /// <summary>
        /// Gets or sets the Description.
        /// </summary>
        public required string Description { get; set; }

        /// <summary>
        /// Gets or sets the date a task is due.
        /// </summary>
        public DateTime? DueDateTime { get; set; }

        /// <summary>
        /// Gets or sets the date a task should be reminded of.
        /// </summary>        
        public DateTime? ReminderDateTime { get; set; }

        /// <summary>
        /// Gets or sets the date a task has been completed.
        /// </summary>
        public DateTime? CompletedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the user the task is assigned to.
        /// </summary>
        public int? AssignedTo { get; set; }

        /// <summary>
        /// Gets or sets the user the task priority.
        /// </summary>
        public UserTaskPriorityEnum UserTaskPriority { get; set; }

        /// <summary>
        /// Gets or sets the user the task status.
        /// </summary>
        public UserTaskStatusEnum UserTaskStatus { get; set; }
    }

    public class UserTaskMetadata
    {
        [Required(ErrorMessageResourceName = nameof(ErrorMessages.Validation_Required), ErrorMessageResourceType = typeof(ErrorMessages))]
        [StringLength(255, ErrorMessageResourceName = nameof(ErrorMessages.Validation_StringLength), ErrorMessageResourceType = typeof(ErrorMessages))]
        public required string Title { get; set; }

        [Required(ErrorMessageResourceName = nameof(ErrorMessages.Validation_Required), ErrorMessageResourceType = typeof(ErrorMessages))]
        [StringLength(2000, ErrorMessageResourceName = nameof(ErrorMessages.Validation_StringLength), ErrorMessageResourceType = typeof(ErrorMessages))]
        public required string Description { get; set; }
    }
}