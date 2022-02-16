namespace Syncfusion.Dashboard.Designer.Web.Service.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Syncfusion.Dashboard.Core.Helpers;
    using Syncfusion.Dashboard.Designer.Web.Service.CustomAttributes;
    using Syncfusion.Dashboard.Designer.Web.Service.Helpers;
    using Syncfusion.Dashboard.Service.Base;
    using Syncfusion.Dashboard.Service.Base.DashboardServerHelpers.Model;
    using Syncfusion.Dashboard.Service.Base.Implementation.ApiController;

    [ApiVersion("1.0")]
    [Route("v{api-version:apiVersion}/dashboard")]
    [CustomAuthorization]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private RequestHelper requestHelper;
        public DashboardController(IOptions<AppSettings> appSettings)
        {
            requestHelper = new RequestHelper();
            AppSettingsInfo.AppSetting = appSettings.Value;

        }

        /// <summary>
        /// Method to get list of dashboards configured by the user.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("get-shared-dashboards")]
        public ApiResponse GetSharedDashboardDetails(SharedDashboardModel dashboardModel)
        {
            requestHelper.request = Request;
            DashboardHelper helper = new DashboardHelper(Request)
            {
                ClientId = requestHelper.GetClientId()
            };
            return helper.GetSharedDashboard(dashboardModel);
        }
        /// <summary>
        /// Method to get list of widgets present in current selected widgets.
        /// </summary>
        /// <param name="dashboardId">Dashboard Id to get widgets in it.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-shared-dashboard-widgets/{dashboardId}")]
        public ApiResponse GetSharedDashboardWidgetDetails([System.Web.Http.FromUri] string dashboardId)
        {
            requestHelper.request = Request;
            DashboardHelper helper = new DashboardHelper(Request)
            {
                ClientId = requestHelper.GetClientId()
            };
            return helper.GetSharedDashboardWidget(dashboardId);
        }
        /// <summary>
        /// Method to add widget from existing dashboards
        /// </summary>
        /// <param name="dashboardid"></param>
        /// <param name="widgetid"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("add-widget/{dashboardid}/{version}/{widgetid}")]
        public object GetExistingWidgetInfo([System.Web.Http.FromUri] string dashboardid, [System.Web.Http.FromUri] string version, [System.Web.Http.FromUri] string widgetid)
        {
            requestHelper.request = Request;
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request)
            {
                ClientId = requestHelper.GetClientId()
            };
            return helper.GetExistingWidgetInfo(dashboardid, version, widgetid);
        }

    }
}
