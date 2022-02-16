namespace Syncfusion.Dashboard.Designer.Web.Service.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Syncfusion.Dashboard.Core.Helpers;
    using Syncfusion.Dashboard.Designer.Web.Service.CustomAttributes;
    using Syncfusion.Dashboard.Service.Base;
    using Syncfusion.Dashboard.Service.Base.DashboardServerHelpers.Model;
    using Syncfusion.Dashboard.Service.Base.Implementation.ApiController;
    using Syncfusion.Dashboard.Service.Base.Implementation.Model;

    /// <summary>
    /// Controller for Oauth Updation.
    /// </summary>
    [ApiVersion("1.0")]
    [Route("v{api-version:apiVersion}/configuration")]
    [CustomAuthorization]
    [ApiController]

    public class ConfigurationController : ControllerBase
    {

        public ConfigurationController(IOptions<AppSettings> appSettings)
        {
            AppSettingsInfo.AppSetting = appSettings.Value;
        }

        [HttpGet]
        [Route("oauth-config-list")]
        public ApiResponse GetOAuthConfigList()
        {
            return new OAuthConfigFileHelper(Request).GetOAuthConfigList();
        }

        [HttpPatch]
        [Route("oauth-config")]
        public ApiResponse UpdateOAuthList(OAuthConfigModel arguments)
        {
            return new OAuthConfigFileHelper(Request).OAuthConfigListUpdater(arguments);
        }

        [HttpPost]
        [Route("migrate-metadata")]
        [AllowAnonymous]
        public ApiResponse MigrateMetadataToTargetServer(ItemRequest connectionInfo)
        {
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request);
            return helper.MigrateMetadataToTargetServer(connectionInfo);
        }

        [HttpGet]
        [Route("export")]
        [AllowAnonymous]
        public ApiResponse GetExportSettings()
        {
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request);
            return helper.GetExportSettings();
        }

        [HttpGet]
        [Route("connectionsetting")]
        public ApiResponse GetConnectionSettings()
        {
            return new ConnectionConfigurationHelper(Request).GetConnections();
        }
        [HttpPost]
        [Route("connectionsetting")]
        public ApiResponse UpdateConnectionSettings(Connection arguments)
        {
            return new ConnectionConfigurationHelper(Request).UpdateConnections(arguments);
        }
    }
}
