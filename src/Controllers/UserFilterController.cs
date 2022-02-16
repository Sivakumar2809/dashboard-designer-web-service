namespace Syncfusion.Dashboard.Designer.Web.Service.Controllers
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Syncfusion.Dashboard.Core.Helpers;
    using Syncfusion.Dashboard.Designer.Web.Service.CustomAttributes;
    using Syncfusion.Dashboard.Designer.Web.Service.Helpers;
    using Syncfusion.Dashboard.Service.Base.DashboardServerHelpers.Model;
    using Syncfusion.Dashboard.Service.Base.Implementation.ApiController;

    [ApiVersion("1.0")]
    [Route("v{api-version:apiVersion}/userfilter")]
    [CustomAuthorization]
    [ApiController]
    public class UserFilterController : ControllerBase
    {
        /// <summary>
        /// Helper class object for the request
        /// </summary>
        private RequestHelper requestHelper;
        public UserFilterController(IOptions<AppSettings> appSettings)
        {
            requestHelper = new RequestHelper();
            AppSettingsInfo.AppSetting = appSettings.Value;
        }
        /// <summary>
        /// Gets the list of data with user and group details 
        /// </summary>
        /// <returns>returns Api response data with filter details</returns>
        [HttpPost]
        [Route("usergroups")]
        public ApiResponse GetUserFilterDetails()
        {       
            UserFilterHelper UserHelper = new UserFilterHelper(Request);
            return UserHelper.GetUserGroupDetails();
        }

        /// <summary>
        /// Gets the list of user data in the groups
        /// </summary>
        /// <param name="groupid">Group id from uri to get the users in group</param>
        /// <returns>Returns all users data in the group</returns>
        [HttpGet]
        [Route("{groupid}/users")]
        public ApiResponse GetUsersInGroup([System.Web.Http.FromUri] string groupid)
        {
            UserFilterHelper UserHelper = new UserFilterHelper(Request);
            return UserHelper.GetUsersInGroup(groupid);
        }

        [HttpPost]
        [Route("save-filter")]
        public ApiResponse SaveUserInfo(IDictionary<string, object> jsonResult)
        {
            requestHelper.request = Request;
            string clientId = requestHelper.GetClientId();
            UserFilterHelper helper = new UserFilterHelper(Request);
            return helper.SaveUserBasedInfo(jsonResult, clientId);
        }

        [HttpPost]
        [Route("advanced/get-mapping-columns")]
        public ApiResponse GetColumnDetails(IDictionary<string, object> jsonResult)
        {
            requestHelper.request = Request;
            UserFilterHelper helper = new UserFilterHelper(Request);
            string clientId = requestHelper.GetClientId();
            return helper.GetColumnDetails(jsonResult, clientId);
        }

        [HttpPost]
        [Route("advanced/validate")]
        public ApiResponse GetGridData(IDictionary<string, object> jsonResult)
        {
            requestHelper.request = Request;
            string clientId = requestHelper.GetClientId();
            UserFilterHelper helper = new UserFilterHelper(Request);
            return helper.GetGridData(jsonResult, clientId);
        }
    }
}
