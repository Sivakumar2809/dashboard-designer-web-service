using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Syncfusion.Dashboard.Core.Helpers.Security;
using Syncfusion.Dashboard.Service.Base;
using Syncfusion.Dashboard.Service.Base.Cache;
using Syncfusion.Dashboard.Service.Base.DashboardServerHelpers;
using Syncfusion.Dashboard.Web.OAuth;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Syncfusion.Dashboard.Designer.Web.Service.CustomAttributes
{
    public sealed class FileServiceAuthorizationFilter : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
			//if (HttpContext.Current.Request.Headers["X-ServiceAuthorization"] != null)
			//{
			//    var decryptedString = TokenCryptoHelper.DoDecryption(HttpContext.Current.Request.Headers["X-ServiceAuthorization"]);
			//    var decryptedObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(decryptedString);

			//    var configSection = (MachineKeySection)ConfigurationManager.GetSection("system.web/machineKey");
			//    if (configSection.ValidationKey == decryptedObject["validationKey"] && configSection.DecryptionKey == decryptedObject["decryptionKey"])
			//    {
			//        return;
			//    }
			//    else
			//    {
			//        BaseLogHandler.LogError("Authotization Token Not Valid", true, null, System.Reflection.MethodBase.GetCurrentMethod());
			//        actionContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
			//        return;
			//    }
			//}
			//else
			//{
			//    BaseLogHandler.LogError("Authotization Token Not Set", true, null, System.Reflection.MethodBase.GetCurrentMethod());
			//    actionContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
			//    return;
			//}
		}
	}
}