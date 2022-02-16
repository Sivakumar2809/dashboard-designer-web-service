// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TemplateController.cs" company="Syncfusion">
//   
// </copyright>
// <summary>
//   Controller class for handling the Template dashboard creation and updation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Syncfusion.Dashboard.Designer.Web.Service.Controllers
{
    using System;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Syncfusion.Dashboard.Core.Helpers;
    using Syncfusion.Dashboard.Designer.Web.Service.CustomAttributes;
    using Syncfusion.Dashboard.Service.Base;
    using Syncfusion.Dashboard.Service.Base.DashboardServerHelpers.Model;
    using Syncfusion.Dashboard.Service.Base.Implementation.Model;

    /// <summary>
    /// Controller class for handling the Template dashboard creation and updation
    /// </summary>
    [ApiVersion("1.0")]
    [Route("v{api-version:apiVersion}/template")]

    [CustomAuthorization]
    [ApiController]
    public class TemplateController : ControllerBase
    {
        public TemplateController(IOptions<AppSettings> appSettings)
        {
            AppSettingsInfo.AppSetting = appSettings.Value;
        }
        /// <summary>
        /// API for creating a template dashboard.
        /// </summary>
        /// <param name="request">Holds the details of the template dashboard along with the credentials to be used for processing the data</param>
        /// <returns>ApiResponse class object</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Reviewed")]
        [HttpPost]
        [Route("create-dashboard")]
        public ApiResponse CreateDashboard(TemplateRequest request)
        {
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request)
            {
                ClientId = Guid.NewGuid().ToString()
            };
            return helper.CreateDashboardFromTemplate(request);
        }

        /// <summary>
        /// Converts the sample template dashboard with the user data.
        /// </summary>
        /// <param name="request">Holds the details of the template dashboard along with the credentials to be used for processing the data</param>
        /// <returns>ApiResponse class object</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Reviewed")]
        [HttpPost]
        [Route("update-dashboard")]
        public ApiResponse UpdateDashboard(TemplateRequest request)
        {
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request)
            {
                ClientId = request.DashboardId ?? Guid.NewGuid().ToString()
            };
            return helper.UpdateTemplateDashboard(request);
        }
    }
}