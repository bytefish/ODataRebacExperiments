// <auto-generated/>
using ApiSdk.Odata.SignInUser;
using ApiSdk.Odata.SignOutUser;
using ApiSdk.Odata.UserTasks;
using Microsoft.Kiota.Abstractions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;
namespace ApiSdk.Odata {
    /// <summary>
    /// Builds and executes requests for operations under \odata
    /// </summary>
    public class OdataRequestBuilder : BaseRequestBuilder {
        /// <summary>Provides operations to call the SignInUser method.</summary>
        public SignInUserRequestBuilder SignInUser { get =>
            new SignInUserRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>Provides operations to call the SignOutUser method.</summary>
        public SignOutUserRequestBuilder SignOutUser { get =>
            new SignOutUserRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>Provides operations to manage the collection of UserTask entities.</summary>
        public UserTasksRequestBuilder UserTasks { get =>
            new UserTasksRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>
        /// Instantiates a new OdataRequestBuilder and sets the default values.
        /// </summary>
        /// <param name="pathParameters">Path parameters for the request</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public OdataRequestBuilder(Dictionary<string, object> pathParameters, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/odata", pathParameters) {
        }
        /// <summary>
        /// Instantiates a new OdataRequestBuilder and sets the default values.
        /// </summary>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public OdataRequestBuilder(string rawUrl, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/odata", rawUrl) {
        }
    }
}
