// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerController.cs" company="Syncfusion">
//   
// </copyright>
// <summary>
//   Controller class for handling the requests from Dashboard Server
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Syncfusion.Dashboard.Designer.Web.Service.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http.Headers;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using Syncfusion.Dashboard.Core.Helpers;
    using Syncfusion.Dashboard.Core.Helpers.Configuration;
    using Syncfusion.Dashboard.Core.Helpers.Security;
    using Syncfusion.Dashboard.Core.Properties;
    using Syncfusion.Dashboard.Designer.Web.Service.CustomAttributes;
    using Syncfusion.Dashboard.Designer.Web.Service.Helpers;
    using Syncfusion.Dashboard.Service.Base;
    using Syncfusion.Dashboard.Service.Base.Cache;
    using Syncfusion.Dashboard.Service.Base.DashboardServerHelpers;
    using Syncfusion.Dashboard.Service.Base.DashboardServerHelpers.Model;
    using Syncfusion.Dashboard.Service.Base.Export.Model;

    [ApiVersion("1.0")]
    [Route("v{api-version:apiVersion}")]
    [CustomAuthorization]
    [ApiController]
    public class ServerController : ControllerBase
    {
        private RequestHelper requestHelper;
        private HttpRequest httpRequest;

        internal ServerController()
        {
            requestHelper = new RequestHelper();
            requestHelper.request = Request;
        }

        public ServerController(HttpRequest request = null)
        {
            requestHelper = new RequestHelper();
            requestHelper.request = Request;
            this.httpRequest = request != null ? request : null;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Reviewed")]
        [HttpGet]
        [Route("dashboards/collection/{itemid}/{version}")]
        public ApiResponse GetDashboardCollection([System.Web.Http.FromUri] Guid itemId, [System.Web.Http.FromUri] int version)
        {
            AddDashboardPathtoHeader(itemId.ToString(), version.ToString());
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request)
            {
                ClientId = itemId.ToString(),
            };
            return helper.GetDashboardCollection(itemId, version);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Reviewed")]
        [HttpGet]
        [Route("dashboards/data-sources/{itemid}/{version}")]
        public ApiResponse GetDataSourceNames([System.Web.Http.FromUri] Guid itemId, [System.Web.Http.FromUri] int version)
        {
            AddDashboardPathtoHeader(itemId.ToString(), version.ToString());
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request)
            {
                ClientId = itemId.ToString(),
            };
            return helper.GetDataSourceNames(itemId, version);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Reviewed")]
        [HttpGet]
        [Route("dashboards/exportuserinfo/{itemid}/{version}")]
        public ApiResponse GetScheduleExportUserInfo([System.Web.Http.FromUri] Guid itemId, [System.Web.Http.FromUri] int version)
        {
            AddDashboardPathtoHeader(itemId.ToString(), version.ToString());
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request)
            {
                ClientId = itemId.ToString()
            };
            return helper.GetScheduleExportUserInfo();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Reviewed")]
        [HttpPost]
        [AllowAnonymous]
        [Route("dashboards/export/excel")]
        public ApiResponse ExportDashboardExcel(ExportExcelArguments arguments)
        {
            try
            {
                if (arguments != null)
                {
                    AddDashboardPathtoHeader(arguments.Itemid, arguments.Version);
                    requestHelper.request = Request;
                    ApiResponse response = ValidateEncryptionStringForScheduleExport(arguments.EncryptedString, arguments.CurrentUserData);
                    if (response.Status)
                    {
                        DashboardDesignerHelper helper = new DashboardDesignerHelper(Request);
                        LocalizationSettings currentLocalSetting = requestHelper.GetCultureInfo(arguments?.CurrentUserData?.LanguageSettings?.ModelLanguage);
                        return helper.GetExportedExcelForScheduling(arguments, currentLocalSetting);
                    }
                    else
                    {
                        return response;
                    }
                }
                return new ApiResponse { Status = false, Message = "Passed arguments is null" };
            }
            catch (Exception ex)
            {
                Dashboard.Service.Base.BaseLogHandler.LogError(ex.Message, true, ex, System.Reflection.MethodBase.GetCurrentMethod());
                return new ApiResponse { Status = false, Message = $"An error occurred in exporting dashboard. - {ex.Message}" };
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Reviewed")]
        [HttpPost]
        [AllowAnonymous]
        [Route("dashboards/export/image")]
        public ApiResponse ExportDashboardImage(ExportImageArguments arguments)
        {
            try
            {
                if (arguments != null)
                {
                    AddDashboardPathtoHeader(arguments.Itemid, arguments.Version);
                    requestHelper.request = Request;
                    ApiResponse response = ValidateEncryptionStringForScheduleExport(arguments.EncryptedString, arguments.CurrentUserData);
                    if (response.Status)
                    {
                        DashboardDesignerHelper helper = new DashboardDesignerHelper(Request);
                        LocalizationSettings currentLocalSetting = requestHelper.GetCultureInfo(arguments?.CurrentUserData?.LanguageSettings?.ModelLanguage);
                        return helper.GetExportImage(arguments, currentLocalSetting);
                    }
                    else
                    {
                        return response;
                    }
                }
                return new ApiResponse { Status = false, Message = "Passed arguments is null" };
            }
            catch (Exception ex)
            {
                Dashboard.Service.Base.BaseLogHandler.LogError(ex.Message, true, ex, System.Reflection.MethodBase.GetCurrentMethod());
                return new ApiResponse { Status = false, Message = $"An error occurred in exporting dashboard. - {ex.Message}" };
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Reviewed")]
        [HttpPost]
        [AllowAnonymous]
        [Route("dashboards/export/pdf")]
        public ApiResponse ExportDashboardPdf(ExportPdfArguments arguments)
        {
            try
            {
                if (arguments != null)
                {
                    AddDashboardPathtoHeader(arguments.Itemid, arguments.Version);
                    requestHelper.request = Request;
                    ApiResponse response = ValidateEncryptionStringForScheduleExport(arguments.EncryptedString, arguments.CurrentUserData);
                    if (response.Status)
                    {
                        DashboardDesignerHelper helper = new DashboardDesignerHelper(Request);
                        LocalizationSettings currentLocalSetting = requestHelper.GetCultureInfo(arguments?.CurrentUserData?.LanguageSettings?.ModelLanguage);
                        return helper.GetExportPdf(arguments, currentLocalSetting);
                    }
                    else
                    {
                        return response;
                    }
                }
                return new ApiResponse { Status = false, Message = "Passed arguments is null" };
            }
            catch (Exception ex)
            {
                Dashboard.Service.Base.BaseLogHandler.LogError(ex.Message, true, ex, System.Reflection.MethodBase.GetCurrentMethod());
                return new ApiResponse { Status = false, Message = $"An error occurred in exporting dashboard. - {ex.Message}" };
            }
        }

        /// <summary>
        /// Check the valid database or not while install the build
        /// </summary>
        /// <param name="connectionString">dashboard id</param>
        /// <param name="providerType">version</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Reviewed")]
        [HttpPost]
        [Route("datasource/validate-lookuptable")]
        public ApiResponse ValidateLookupTable([FromBody] Dictionary<string, object> jsonResult)
        {
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request)
            {
                ClientId = Guid.NewGuid().ToString()
            };
            return helper.ValidateLookupTable(jsonResult);
        }

        /// <summary>
        /// Added dashboard path into header, since this will be used for local folder creation in server mode
        /// </summary>
        /// <param name="itemid">dashboard id</param>
        /// <param name="version">version</param>
        private void AddDashboardPathtoHeader(string itemid, string version)
        {
            if (Request != null)
            {
                Request.Headers.Add("DashboardPath", string.Format("{0}/{1}", itemid, version));
                Request.Headers.Add("Mode", "View");
            }
        }

        /// <summary>
        /// Get access token from request headers.
        /// </summary>
        /// <returns>Returns access token.</returns>
        private string GetAccessToken(HttpRequest httpRequest)
        {
            var authorization = httpRequest.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer", string.Empty).Replace("bearer", string.Empty).Trim();
            if (authorization != null)
            {
                var authorizationHeaderValue = AuthenticationHeaderValue.Parse(authorization);
                if (authorizationHeaderValue != null)
                {
                    return authorizationHeaderValue.Parameter ?? authorizationHeaderValue.Scheme;
                }
            }
            return authorization;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="encryptString"></param>
        public ApiResponse ValidateEncryptionStringForScheduleExport(string encryptString, CurrentUser userData)
        {
            try
            {
                this.httpRequest = this.httpRequest ?? Request;
                if (this.httpRequest != null && !this.httpRequest.Headers.ContainsKey("Mode"))
                {
                    this.httpRequest.Headers.Add("Mode", "View");
                }
                var decryptedString = TokenCryptoHelper.DoDecryption(encryptString);
                var decryptedObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(decryptedString);
                SystemSettings settings = AppConfiguration.SystemSettings;
                if (settings.MachineKey.ValidationKey == decryptedObject["validationKey"] && settings.MachineKey.DecryptionKey == decryptedObject["decryptionKey"])
                {
                    string accessToken = GetAccessToken(this.httpRequest);
                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        CacheModel cachedData = new Dashboard.Service.Base.Cache.CacheManager().GetFromCache(accessToken) as CacheModel;
                        if (cachedData == null || cachedData.ExpiresIn <= DateTime.Now)
                        {
                            try
                            {
                                IHeaderDictionary requestHeaders = this.httpRequest.Headers;
                                Dictionary<string, string> HeaderKeys = new Dictionary<string, string>();
                                if (requestHeaders != null)
                                {
                                    foreach (var hd in requestHeaders)
                                    {
                                        HeaderKeys.Add(hd.Key, hd.Value);
                                    }
                                }
                                ServerRequestHandler handler = new ServerRequestHandler(accessToken, HeaderKeys);
                                var response = this.httpRequest.Headers.ContainsKey("DashboardPath") ? handler.ValidateToken(this.httpRequest.Headers["DashboardPath"].ToString(), this.httpRequest.HttpContext.Connection.RemoteIpAddress.ToString()) : handler.ValidateToken("", this.httpRequest.HttpContext.Connection.RemoteIpAddress.ToString());
                                new Dashboard.Service.Base.Cache.CacheManager().AddToCache(accessToken, new CacheModel
                                {
                                    Email = userData == null ? response.Email : userData.Email,
                                    FullName = userData == null ? response.UserDisplayName : userData.FullName,
                                    UserId = userData == null ? response.UserId : userData.Id,
                                    Token = accessToken,
                                    ExpiresIn = DateTime.Now.AddMinutes(20)
                                });

                            }
                            catch (Exception ex)
                            {
                                string errorMsg = ex.ToString();
                                if (ex is WebException)
                                {
                                    if ((ex as WebException).Response is HttpWebResponse)
                                    {
                                        errorMsg = new StreamReader((ex as WebException).Response.GetResponseStream()).ReadToEnd();
                                    }
                                }
                                BaseLogHandler.LogError(errorMsg, true, ex, System.Reflection.MethodBase.GetCurrentMethod());
                                return new ApiResponse() { Status = false, Message = "Authotization Token is invalid." };
                            }
                        }
                    }
                    return new ApiResponse() { Status = true };
                }
                else
                {
                    return new ApiResponse() { Status = false, Message = "Authentication failed." };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse() { Status = false, Message = "Authentication failed." };
            }
        }
    }
}
