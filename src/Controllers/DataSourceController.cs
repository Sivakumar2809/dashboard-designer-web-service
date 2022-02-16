namespace Syncfusion.Dashboard.Designer.Web.Service.Controllers
{
    using System;
    using Newtonsoft.Json;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Syncfusion.Dashboard.Core.Helpers;
    using Syncfusion.Dashboard.Service.Base;
    using Syncfusion.Dashboard.Service.Base.DashboardServerHelpers.Model;
    using Syncfusion.Dashboard.Service.Base.Implementation.ApiController;
    using Syncfusion.Dashboard.Service.Base.Implementation.Model;

    [ApiVersion("1.0")]
    [Route("v{api-version:apiVersion}/datasource")]
    [ApiController]
    public class DataSourceController : ControllerBase
    {
        public DataSourceController(IOptions<AppSettings> appSettings)
        {
            AppSettingsInfo.AppSetting = appSettings.Value;
        }
        /// <summary>
        /// Gets the list of data with user and group details 
        /// </summary>
        /// <returns>returns Api response data with filter details</returns>
        [HttpPost]
        [Route("copy")]
        public ApiResponse CopyDataSource(object requestInfo)
        {
            try
            {
                CopyDataSourceRequest request = JsonConvert.DeserializeObject<CopyDataSourceRequest>(requestInfo.ToString());
                DataSourceControllerHelper helper = new DataSourceControllerHelper(Request);
                return helper.CopyDataSource(request);
            }
            catch (Exception ex)
            {
                ApiResponse response = new ApiResponse()
                {
                    ApiStatus = true,
                    Status = false,
                    Message = ex.Message
                };
                BaseLogHandler.LogError(ex.Message, true, ex, System.Reflection.MethodBase.GetCurrentMethod());
                return response;
            }
        }

        [HttpPost]
        [Route("create")]
        public ApiResponse CreateDataSource(CreateDataSourceModel model)
        {
            DataSourceControllerHelper helper = new DataSourceControllerHelper(Request);
            return helper.CreateDataSource(model);
        }
    }
}
