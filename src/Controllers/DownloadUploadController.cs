namespace Syncfusion.Dashboard.Designer.Web.Service.Controllers
{
    using System;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Syncfusion.Dashboard.Core.Helpers;
    using Syncfusion.Dashboard.Designer.Web.Service.Helpers;
    using Syncfusion.Dashboard.Service.Base.DashboardServerHelpers.Model;
    using Syncfusion.Dashboard.Service.Base.DownloadUpload;
    using Syncfusion.Dashboard.Designer.Web.Service.CustomAttributes;
    using Newtonsoft.Json;

    [ApiVersion("1.0")]
    [Route("v{api-version:apiVersion}/migrate")]
    [CustomAuthorization]
    [ApiController]
    public class DownloadUploadController : ControllerBase
    {
        /// <summary>
        /// Helper class object for the request
        /// </summary>
        private RequestHelper requestHelper;
        public DownloadUploadController(IOptions<AppSettings> appSettings)
        {
            requestHelper = new RequestHelper();
            AppSettingsInfo.AppSetting = appSettings.Value;
        }

        [HttpPost]
        [Route("download/dashboard")]
        public ApiResponse DownloadDashboard(object requestObj)
        {
            PublishItemDetail request = JsonConvert.DeserializeObject<PublishItemDetail>(requestObj.ToString());
            DownloadUploadHelper helper = new DownloadUploadHelper(Request);
            IHeaderDictionary headers = Request.Headers;
            return helper.ProcessDataForDownloadDashboard(request, headers);
        }

        [HttpPost]
        [Route("download/datasource")]
        public ApiResponse DownloadDataSource(PublishItemDetail request)
        {
            DownloadUploadHelper helper = new DownloadUploadHelper(Request);
            IHeaderDictionary headers = Request.Headers;
            return helper.ProcessDataForDownloadDataSource(request, headers);
        }

        [HttpPost]
        [Route("upload/dashboard")]
        public ApiResponse UploadDashboard(object requestObj)
        {
            SlaveSiteRequest request = JsonConvert.DeserializeObject<SlaveSiteRequest>(requestObj.ToString());
            DownloadUploadHelper helper = new DownloadUploadHelper(Request);
            IHeaderDictionary headers = Request.Headers;
            return helper.ProcessDataForUploadDashboard(request, headers);
        }

        [HttpPost]
        [Route("upload/datasource")]
        public ApiResponse UploadDataSource(PublishItemDetail request)
        {
            DownloadUploadHelper helper = new DownloadUploadHelper(Request);
            IHeaderDictionary headers = Request.Headers;
            return helper.ProcessDataForUploadDataSource(request, headers);
        }

        [HttpGet]
        [Route("validate-item/{itemid}/{version}")]
        public ApiResponse ValidateOAuthDataSource([System.Web.Http.FromUri] Guid itemId, [System.Web.Http.FromUri] int version)
        {
            DownloadUploadHelper helper = new DownloadUploadHelper(Request);
            return helper.ValidateIsOAuthDataSource(itemId, version);
        }
    }
}
