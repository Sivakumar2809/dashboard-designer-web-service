using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Syncfusion.Dashboard.Core.Helpers;
using Syncfusion.Dashboard.Service.Base;
using Syncfusion.Dashboard.Service.Base.Cache;
using Syncfusion.Dashboard.Service.Base.DashboardServerHelpers;
using Syncfusion.Dashboard.Web.OAuth;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Http;
using System.Text;
using Syncfusion.Dashboard.Service.Base.DashboardServerHelpers.Model;

namespace Syncfusion.Dashboard.Designer.Web.Service.CustomAttributes
{
    [AttributeUsage(AttributeTargets.All)]
    public class CustomAuthorizationAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (SkipAuthorization(context))
            {
                return;
            }
            var settings = AppConfiguration.GetFromAppSettings("hosting_env:server");
            if (settings == null || settings.ToUpperInvariant() == "TRUE")
            {
                string authToken = ExtractToken(context);
                if (!string.IsNullOrWhiteSpace(authToken))
                {
                    if (string.IsNullOrEmpty(context.HttpContext.Request.Headers["Caller"]) && string.IsNullOrEmpty(context.HttpContext.Request.Headers["caller"]))
                    {
                        string serverUrl = Utility.GetValueFromQueryString(context.HttpContext.Request.QueryString.ToString(), "server");
                        if (!string.IsNullOrEmpty(serverUrl))
                        {
                            context.HttpContext.Request.Headers.Add("Caller", serverUrl);
                        }
                    }
                    CacheModel cachedData = new CacheManager().GetFromCache(authToken) as CacheModel;
                    if (cachedData == null || cachedData.ExpiresIn <= DateTime.Now || ForceValidateRequest(context))
                    {
                        Dictionary<string, string> HeaderKeys = new Dictionary<string, string>();
                        if (context.HttpContext.Request.Headers != null)
                        {
                            foreach (var hd in context.HttpContext.Request.Headers)
                            {
                                HeaderKeys.Add(hd.Key, hd.Value);
                            }
                        }
                        ServerRequestHandler handler = new ServerRequestHandler(authToken, HeaderKeys);
                        try
                        {
                            var response = new TokenResponse();
                            if (context.HttpContext.Request.Headers.ContainsKey("DashboardPath"))
                            {
                                response = handler.ValidateToken(context.HttpContext.Request.Headers["DashboardPath"], context.HttpContext.Connection.RemoteIpAddress.ToString());
                            }
                            else if (context.HttpContext.Request.Headers.ContainsKey("dashboardpath"))
                            {
                                response = handler.ValidateToken(context.HttpContext.Request.Headers["dashboardpath"], context.HttpContext.Connection.RemoteIpAddress.ToString());
                            }
                            else
                            {
                                response = handler.ValidateToken("", context.HttpContext.Connection.RemoteIpAddress.ToString());
                            }
                            if (!response.Status)
                            {
                                BaseLogHandler.LogError(response.StatusMessage, true, null, System.Reflection.MethodBase.GetCurrentMethod());
                                context.Result = new UnauthorizedResult();
                                if (response.ErrorCode == "10000")
                                {
                                    context.HttpContext.Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "/10000/" + response.StatusMessage;
                                }
                                return;
                            }
                            new CacheManager().AddToCache(authToken, new CacheModel
                            {
                                Email = response.Email,
                                FullName = response.UserDisplayName,
                                UserId = response.UserId,
                                Token = authToken,
                                ExpiresIn = DateTime.Now.AddMinutes(20),
                                Filter = response.Filter,
                                Domain = response.UserDomain
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
                            context.Result = new UnauthorizedResult();
                            return;
                        }
                    }
                }
                else if ((!string.IsNullOrEmpty(context.HttpContext.Request.Headers["IsPublic"]) && context.HttpContext.Request.Headers["IsPublic"].ToString().ToUpperInvariant().Equals("TRUE")) ||
                    (!string.IsNullOrEmpty(context.HttpContext.Request.Headers["ispublic"]) && context.HttpContext.Request.Headers["ispublic"].ToString().ToUpperInvariant().Equals("TRUE")))
                {
                    try
                    {
                        Dictionary<string, string> HeaderKeys = new Dictionary<string, string>();
                        if (context.HttpContext.Request.Headers != null)
                        {
                            foreach (var hd in context.HttpContext.Request.Headers)
                            {
                                HeaderKeys.Add(hd.Key, hd.Value);
                            }
                        }
                        ServerRequestHandler handler = new ServerRequestHandler(string.Empty, HeaderKeys);
                        string clientId = context.HttpContext.Request.Headers.ContainsKey("ClientID") ? context.HttpContext.Request.Headers["ClientID"] : context.HttpContext.Request.Headers.ContainsKey("clientid") ? context.HttpContext.Request.Headers["clientid"] : Microsoft.Extensions.Primitives.StringValues.Empty;
                        var response = handler.ValidateIsPublicDashboard(clientId);
                        if (response != null && !response.Status)
                        {
                            BaseLogHandler.LogError(response.StatusMessage, true, null, System.Reflection.MethodBase.GetCurrentMethod());
                            context.Result = new UnauthorizedResult();
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        string errorMsg = ex.Message;
                        if (ex is WebException)
                        {
                            if ((ex as WebException).Response is HttpWebResponse)
                            {
                                errorMsg = new StreamReader((ex as WebException).Response.GetResponseStream()).ReadToEnd();
                            }
                        }
                        BaseLogHandler.LogError(errorMsg, true, ex, System.Reflection.MethodBase.GetCurrentMethod());
                        context.Result = new UnauthorizedResult();
                        return;
                    }
                }
                else
                {
                    BaseLogHandler.LogError("Authotization Token Not set", true, null, System.Reflection.MethodBase.GetCurrentMethod());
                    context.Result = new UnauthorizedResult();
                    return;
                }
            }
        }

        #region Force Token Validation Block
        // Holds the list of actions that need to force the token validation request. The name should be given in Upper case for proper comparison
        private static readonly string[] actions = { "OPEN" };

        public bool AllowMultiple => throw new NotImplementedException();

        /// <summary>
        /// Method to verify whether the request needs to force validate the token. 
        /// This is added to ensure that the dashboard load action revalidated the token to avoid unauthorized access to the dashboards in mobile application
        /// </summary>
        /// <param name="actionContext"> The request context object</param>
        /// <returns>Boolean that represents whether to validate the token or not</returns>
        private static bool ForceValidateRequest(AuthorizationFilterContext actionContext)
        {
            var requestedAction = actionContext.RouteData.Values.ContainsKey("action") ? actionContext.RouteData.Values.GetValueOrDefault("action")?.ToString().ToUpperInvariant() : string.Empty; // The action name is converted to upper case for easy comparison
            if (!string.IsNullOrEmpty(requestedAction))
            {
                return actions.Contains(requestedAction);
            }
            return false;
        }
        #endregion

        private static string ExtractToken(AuthorizationFilterContext actionContext)
        {
            var authorization = actionContext.HttpContext.Request.Headers.ContainsKey("Authorization") ? actionContext.HttpContext.Request.Headers["Authorization"] : actionContext.HttpContext.Request.Headers.ContainsKey("authorization") ? actionContext.HttpContext.Request.Headers["authorization"] : Microsoft.Extensions.Primitives.StringValues.Empty;
            if (!string.IsNullOrEmpty(authorization))
            {
                return authorization.ToString().Replace("bearer", string.Empty).Replace("Bearer", string.Empty).Trim();

            }
            string token = Utility.GetValueFromQueryString(actionContext.HttpContext.Request.QueryString.ToString(), "token");
            if (string.IsNullOrEmpty(token))
            {
                string state = Utility.GetValueFromQueryString(actionContext.HttpContext.Request.QueryString.ToString(), "state");
                if (!string.IsNullOrEmpty(state))
                {
                    int validateBase64 = state.Length % 4;
                    if (validateBase64 > 0)
                    {
                        state += new string('=', 4 - validateBase64);
                    }

                    //check whether session exists
                    if ((actionContext.HttpContext.Session != null) && actionContext.HttpContext.Session.Keys.Contains(state))
                    {
                        var sessionStateValue = actionContext.HttpContext.Session.GetString(state);
                        string stateString = Encoding.UTF8.GetString(Convert.FromBase64String(sessionStateValue));
                        token = Utility.GetValueFromQueryString(stateString, "token");
                    }
                    else
                    {
                        token = string.Empty;
                    }

                }
                return token;
            }

            return token;
        }
        private static bool SkipAuthorization(AuthorizationFilterContext context)
        {
            var filters = context.ActionDescriptor.EndpointMetadata;
            for (var i = 0; i < filters.Count; i++)
            {
                if (filters[i] is AllowAnonymousAttribute)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
