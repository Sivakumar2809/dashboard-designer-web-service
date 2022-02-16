using Microsoft.AspNetCore.Mvc.Filters;
using Syncfusion.Dashboard.Core.Properties;
using Syncfusion.Dashboard.Designer.Web.Service.Helpers;
using Syncfusion.Dashboard.Service;
using Syncfusion.Dashboard.Service.Base;
using Syncfusion.Dashboard.Service.Base.CoreHelpers;
using Syncfusion.Dashboard.Service.Base.Implementation.Model;
using System;
using System.IO;

namespace Syncfusion.Dashboard.Designer.Web.Service.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field)]
    public class ValidateAndLoadDashboardAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                DashboardDesignerHelper helper = new DashboardDesignerHelper(context.HttpContext.Request);
                string fileName = Path.ChangeExtension(DashboardConstants.TempDashboardFileName, DashboardConstants.JsonFileExtension);
                var jsonPath = Path.Combine(FilePathHelper.GetDashboardDesigerClientPath(FilePathHelper.GetDashboardDesignerViewerModeTempPath(helper.CallerUrl, helper.DashboardPath)), fileName);
                if (helper.ModeType.ToUpperInvariant() == "VIEW" && !File.Exists(jsonPath))
                {
                    RequestHelper requestHelper = new RequestHelper() { request = context.HttpContext.Request };
                    LocalizationSettings currentLocaleSetting = requestHelper.GetCulture();
                    OpenDashboardRequestModel requestModel = new OpenDashboardRequestModel
                    {
                        Path = helper.DashboardPath
                    };
                    helper.OpenDashboard(requestModel, requestHelper.GetClientId(), true, null, currentLocaleSetting);
                }
            }
            catch (Exception ex)
            {
                BaseLogHandler.LogError(ex.Message, true, ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
            base.OnActionExecuting(context);
        }
    }
}
