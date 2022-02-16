// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OAuthController.cs" company="Syncfusion">
//   
// </copyright>
// <summary>
//   Controller class for handling OAuth login and token generation 
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Syncfusion.Dashboard.Designer.Web.Service.Controllers
{
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Net;
    using System.Text;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Syncfusion.Dashboard.Core.Helpers;
    using Syncfusion.Dashboard.Designer.Web.Service.CustomAttributes;
    using Syncfusion.Dashboard.Service.Base;
    using Syncfusion.Dashboard.Web.OAuth;
    using Syncfusion.Dashboard.Web.OAuth.Model;

    [ApiVersion("1.0")]
    [Route("v{api-version:apiVersion}/oauth")]
    [CustomAuthorization]
    [ApiController]
    public class OAuthController : ControllerBase
    {
        public OAuthController(IOptions<AppSettings> appSettings)
        {
            AppSettingsInfo.AppSetting = appSettings.Value;
        }
        [Route("authenticate")]
        [HttpGet]
        public IActionResult Authenticate()
        {
            OAuth2RequestParams requestModel = new OAuth2RequestParams();
            ContentResult contentResult = new ContentResult();
            string errorMsg = "";
            try
            {
                if (Utility.ValidateQueryStringForRequestModel(ref requestModel, HttpContext.Request.QueryString.ToString()))
                {
                    string api = "/v1.0/oauth/agent";
                    StringBuilder segmentBuilder = new StringBuilder();
                    int port = Int32.Parse((HttpContext.Request.Host.Port == null ? -1 : HttpContext.Request.Host.Port).ToString());
                    string path = string.Format("{0}{1}", HttpContext.Request.PathBase.Value, HttpContext.Request.Path.Value);
                    Uri url = new Uri(new UriBuilder(HttpContext.Request.Scheme, HttpContext.Request.Host.Host, port, path).ToString());
                    foreach (string segment in url.Segments)
                    {
                        if (segment.ToLowerInvariant() == "v1.0/")
                        {
                            break;
                        }
                        if (segment.ToLowerInvariant() != "/")
                        {
                            segmentBuilder.Append(segment);
                        }
                    }
                    string token = Utility.GetValueFromQueryString(HttpContext.Request.QueryString.ToString(), "token");
                    string origin = Utility.GetValueFromQueryString(HttpContext.Request.QueryString.ToString(), "origin");
                    string server = Utility.GetValueFromQueryString(HttpContext.Request.QueryString.ToString(), "server");
                    string accid = Utility.GetValueFromQueryString(HttpContext.Request.QueryString.ToString(), "accid");
                    requestModel.LoginAs = Utility.GetValueFromQueryString(HttpContext.Request.QueryString.ToString(), "accname");
                    string pageRedirect = Utility.GetValueFromQueryString(HttpContext.Request.QueryString.ToString(), "pageredirect");
                    string caller = new UriBuilder(HttpContext.Request.Scheme, HttpContext.Request.Host.Host, port, segmentBuilder.ToString()).ToString();
                    string redirectUrl = ConfigurationManager.AppSettings.Get("designer_oauth_redirect:url");
                    requestModel.OrganizationUrl = Utility.GetValueFromQueryString(HttpContext.Request.QueryString.ToString(), "resourceurl");
                    if (string.IsNullOrWhiteSpace(redirectUrl))
                    {
                        segmentBuilder.Append("/");
                        segmentBuilder.Replace(segmentBuilder.ToString(), segmentBuilder.ToString().TrimEnd(new[] { '/' }));
                        segmentBuilder.Append(api);
                        requestModel.RedirectUrl = new UriBuilder(HttpContext.Request.Scheme, HttpContext.Request.Host.Host, port, segmentBuilder.ToString()).ToString();
                    }
                    else
                    {
                        var uriBuilder = new UriBuilder(redirectUrl.TrimEnd(new[] { '/' }));
                        uriBuilder.Path += api;
                        uriBuilder.Path = uriBuilder.Path.Replace("//", "/");
                        requestModel.RedirectUrl = uriBuilder.ToString();
                    }
                    BaseLogHandler.LogInfo("Get values from query string", System.Reflection.MethodBase.GetCurrentMethod(), $"Service : {requestModel?.Service} Provider : {requestModel?.Provider} Redirect Url : {requestModel?.RedirectUrl}");
                    var stateParameterKey = Guid.NewGuid().ToString();
                    var stateParameterValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("provider={0}&service={1}&origin={2}&token={3}&caller={4}&server={5}&accid={6}&pageredirect={7}&resourceurl={8}", requestModel.Provider.ToString(), requestModel.Service, origin, token, caller, server, accid, pageRedirect, requestModel.OrganizationUrl)));
                    HttpContext.Session.SetString(stateParameterKey, stateParameterValue);
                    requestModel.State = stateParameterKey;
                    OAuth2Helper helper = new OAuth2Helper(requestModel, DashboardDesignerHelper.GetOAuthConfigurationModel(HttpContext.Request));
                    BaseLogHandler.LogInfo("Authenticate method Called", System.Reflection.MethodBase.GetCurrentMethod(), $"Status code : {contentResult?.StatusCode}");
                    return Redirect(helper.GetAuthenticationUrl());
                }
            }
            catch (Exception ex)
            {
                BaseLogHandler.LogError(ex.ToString(), true, ex, System.Reflection.MethodBase.GetCurrentMethod());
                errorMsg = ex.Message;
            }
            contentResult.Content = "<Error>" + (!string.IsNullOrEmpty(errorMsg) ? errorMsg : "The Request is not valid.Required parameters are not passed") + ".</Error>";
            contentResult.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/xml").ToString();
            return contentResult;
        }

        [Route("token")]
        [HttpGet]
        public IActionResult Token()
        {
            var contentResult = new ContentResult();
            string queryString = Utility.GetValueFromQueryString(HttpContext.Request.QueryString.ToString(), "state");
            queryString = GetValidBase64String(queryString);
            var stateParameterBasedOnGUID = "";
            ////check whether session exists & session with guid key value is not null.
            try
            {

                if (HttpContext.Session != null && HttpContext.Session.GetString(queryString) != null)
                {
                    stateParameterBasedOnGUID = HttpContext.Session.GetString(queryString).ToString();
                }
                else
                {
                    BaseLogHandler.LogError("Session expired", true, new WebException("Session expired"), System.Reflection.MethodBase.GetCurrentMethod());
                    contentResult.Content = "<p>Session expired</p>";
                    contentResult.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html").ToString();
                    return contentResult;
                }
                string stateString = Encoding.UTF8.GetString(Convert.FromBase64String(stateParameterBasedOnGUID));
                string token = Utility.GetValueFromQueryString(stateString, "token");
                string provider = Utility.GetValueFromQueryString(stateString, "provider");
                string service = Utility.GetValueFromQueryString(stateString, "service");
                string origin = Utility.GetValueFromQueryString(stateString, "origin");
                string accountId = Utility.GetValueFromQueryString(stateString, "accid");
                string server = Utility.GetValueFromQueryString(stateString, "server");
                string pageRedirect = Utility.GetValueFromQueryString(stateString, "pageredirect");
                DashboardDesignerHelper designerHelper = new DashboardDesignerHelper(token, HttpContext.Request);
                string api = "/v1.0/oauth/agent";
                string redirectUrl = ConfigurationManager.AppSettings.Get("designer_oauth_redirect:url");
                int port = Int32.Parse((HttpContext.Request.Host.Port == null ? -1 : HttpContext.Request.Host.Port).ToString());
                string path = string.Format("{0}{1}", HttpContext.Request.PathBase.Value, HttpContext.Request.Path.Value);
                if (string.IsNullOrWhiteSpace(redirectUrl))
                {
                    StringBuilder segmentBuilder = new StringBuilder();
                    Uri url = new Uri(new UriBuilder(HttpContext.Request.Scheme, HttpContext.Request.Host.Host, port, path).ToString());
                    foreach (string segment in url.Segments)
                    {
                        if (segment.ToLowerInvariant() == "v1.0/")
                        {
                            break;
                        }
                        if (segment.ToLowerInvariant() != "/")
                        {
                            segmentBuilder.Append(segment);
                        }
                    }
                    segmentBuilder.Append("/");
                    segmentBuilder.Replace(segmentBuilder.ToString(), segmentBuilder.ToString().TrimEnd(new[] { '/' }));
                    segmentBuilder.Append(api);
                    redirectUrl = new UriBuilder(HttpContext.Request.Scheme, HttpContext.Request.Host.Host, port, segmentBuilder.ToString()).ToString();
                }
                else
                {
                    var uriBuilder = new UriBuilder(redirectUrl);
                    uriBuilder.Path += api;
                    uriBuilder.Path = uriBuilder.Path.Replace("//", "/");
                    redirectUrl = uriBuilder.ToString();
                }
                BaseLogHandler.LogInfo("Token method called. Get values from query string", System.Reflection.MethodBase.GetCurrentMethod(), $"Service : {service} Provider : {provider} Token : {token} origin : {origin} server : {server} Account id : {accountId} pageRedirect : {pageRedirect} Redirect Url : {redirectUrl}");
                var response = designerHelper.ProcessOAuthToken(stateString, redirectUrl, accountId);
                if (response.Status)
                {
                    if (string.IsNullOrEmpty(pageRedirect))
                    {
                        contentResult.Content = "<script>if(window.opener){window.opener.postMessage('" + Newtonsoft.Json.JsonConvert.SerializeObject(new { Status = true, UpdatedOn = DateTime.UtcNow.ToString("o"), serviceId = response.Data, provider = Utility.GetValueFromQueryString(stateString, "provider"), service = Utility.GetValueFromQueryString(stateString, "service"), name = response.Name }) + "','" + origin + "');}window.close();</script>";
                        contentResult.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html").ToString();
                    }
                    else
                    {
                        StringBuilder segmentBuilder = new StringBuilder();
                        Uri url = new Uri(new UriBuilder(HttpContext.Request.Scheme, HttpContext.Request.Host.Host, port, path).ToString());
                        foreach (string segment in url.Segments)
                        {
                            if (segment.ToLowerInvariant() == "v1.0/")
                            {
                                break;
                            }
                            if (segment.ToLowerInvariant() != "/")
                            {
                                segmentBuilder.Append(segment);
                            }
                        }
                        return Redirect(pageRedirect + "?payload=" + Newtonsoft.Json.JsonConvert.SerializeObject(new
                        {
                            provider,
                            service,
                            token,
                            origin,
                            server,
                            serviceId = response.Data,
                            serviceUrl = new UriBuilder(HttpContext.Request.Scheme, HttpContext.Request.Host.Host, port, segmentBuilder.ToString()).ToString()
                        })); 
                    }
                }
                else
                {
                    BaseLogHandler.LogError(response.Data?.ToString(), true, new Exception(response.Data?.ToString()), System.Reflection.MethodBase.GetCurrentMethod());
                    contentResult.Content = string.IsNullOrEmpty(response.Data?.ToString()) ? "<p>An error occured while trying to validate the response from the provider</p>" : "<p>" + response.Data.ToString() + "</p>";
                    contentResult.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html").ToString();
                }
                return contentResult;
            }

            catch (Exception ex)
            {
                BaseLogHandler.LogError(ex.ToString(), true, ex, System.Reflection.MethodBase.GetCurrentMethod());
                contentResult.Content = "<Error>" + ex.Message + ".</Error>";
                contentResult.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/xml").ToString();
            }
            return contentResult;
        }


        [HttpGet]
        [Route("agent")]
        [AllowAnonymous]
        public IActionResult Proxy()
        {
            var contentResult = new ContentResult();
            string api = "v1.0/oauth/token";
            string error = Utility.GetValueFromQueryString(HttpContext.Request.QueryString.ToString(), "error");
            BaseLogHandler.LogInfo("Proxy method called", System.Reflection.MethodBase.GetCurrentMethod(), $"Status Code : {contentResult?.StatusCode} Error :{error}");
            try
            {
                if (!string.IsNullOrEmpty(error))
                {
                    contentResult.Content = error;
                    contentResult.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html").ToString();
                }
                else
                {
                    string queryString = Utility.GetValueFromQueryString(HttpContext.Request.QueryString.ToString(),
                        "state");
                    queryString = GetValidBase64String(queryString);
                    var sessionStateValue = HttpContext.Session.GetString(queryString).ToString();
                    string stateString = Encoding.UTF8.GetString(Convert.FromBase64String(sessionStateValue));
                    string caller = Utility.GetValueFromQueryString(stateString, "caller");
                    if (string.IsNullOrEmpty(caller))
                    {
                        contentResult.Content = "<p>An error occured while trying to validate the response from the provider.The response is tampered.</p>";
                        contentResult.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html").ToString();
                    }
                    else
                    {
                        UriBuilder builder = new UriBuilder(caller) { Query = HttpContext.Request.QueryString.ToString() };
                        builder.Path += api;
                        return Redirect(builder.ToString());
                    }
                }
                return contentResult;
            }
            catch (Exception ex)
            {
                BaseLogHandler.LogError(ex.ToString(), true, ex, System.Reflection.MethodBase.GetCurrentMethod());
                contentResult.Content = "<Error>" + ex.Message + ".</Error>";
                contentResult.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/xml").ToString();
            }
            return contentResult;
        }

        [HttpGet]
        [Route("re-authorize")]
        public IActionResult ReAuthorize()
        {
            var contentResult = new ContentResult();
            string queryString = HttpContext.Request.QueryString.ToString();
            string token = Utility.GetValueFromQueryString(queryString, "token");
            string accid = Utility.GetValueFromQueryString(queryString, "id");
            string server = Utility.GetValueFromQueryString(queryString, "server");
            string origin = Utility.GetValueFromQueryString(queryString, "origin");
            string service = Utility.GetValueFromQueryString(queryString, "service");
            string organizationUri = Utility.GetValueFromQueryString(queryString, "resourceurl");
            try
            {
                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(accid))
                {
                    contentResult.Content = "<p>The request parameters are not valid.</p>";
                    contentResult.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html").ToString();
                }
                else
                {
                    BaseLogHandler.LogInfo("Reauthorize method called. Get values from query string", System.Reflection.MethodBase.GetCurrentMethod(), $"Service : {service} Token : {token} origin : {origin} server : {server}");
                    DashboardDesignerHelper designerHelper = new DashboardDesignerHelper(token, HttpContext.Request);
                    var credential = designerHelper.GetOAuthAccount(accid);
                    if (credential != null)
                    {
                        BaseLogHandler.LogInfo("Credentials retrieved for reauthorize", System.Reflection.MethodBase.GetCurrentMethod());
                        string api = "/v1.0/oauth/authenticate";
                        StringBuilder segmentBuilder = new StringBuilder();
                        int port = Int32.Parse((HttpContext.Request.Host.Port == null ? -1 : HttpContext.Request.Host.Port).ToString());
                        string path = string.Format("{0}{1}", HttpContext.Request.PathBase.Value, HttpContext.Request.Path.Value);
                        Uri url = new Uri(new UriBuilder(HttpContext.Request.Scheme, HttpContext.Request.Host.Host, port, path).ToString());
                        foreach (string segment in url.Segments)
                        {
                            if (segment.ToLowerInvariant() == "v1.0/")
                            {
                                break;
                            }
                            if (segment.ToLowerInvariant() != "/")
                            {
                                segmentBuilder.Append(segment);
                            }
                        }
                        segmentBuilder.Append("/");
                        segmentBuilder.Replace(segmentBuilder.ToString(), segmentBuilder.ToString().TrimEnd(new[] { '/' }));
                        segmentBuilder.Append(api);
                        string query = string.Format("?token={0}&server={1}&origin={2}&accid={3}&accname={4}&provider={5}&resourceurl={6}", token, server, origin, accid, credential.Email, credential.Provider.ToString(), organizationUri);
                        if (string.IsNullOrEmpty(service))
                        {
                            query = string.Format("{0}&services={1}", query, credential.Services.Aggregate((i, j) => string.Format("{0},{1}", i, j)));
                        }
                        else
                        {
                            query = string.Format("{0}&service={1}", query, service);
                        }
                        return Redirect(new UriBuilder(HttpContext.Request.Scheme, HttpContext.Request.Host.Host, port, segmentBuilder.ToString(), query).ToString());
                    }
                    else
                    {
                        contentResult.Content = "<p>The selected account cannot be re authorized. There was an error reading the account information.</p>";
                        contentResult.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html").ToString();
                    }
                }
                return contentResult;
            }
            catch (Exception ex)
            {
                BaseLogHandler.LogError(ex.ToString(), true, ex, System.Reflection.MethodBase.GetCurrentMethod());
                contentResult.Content =  "</Error>" + ex.Message + ".</Error>";
                contentResult.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/xml").ToString();
            }
            return contentResult;
        }

        private static string GetValidBase64String(string queryString)
        {
            int validateBase64 = queryString.Length % 4;
            if (validateBase64 > 0)
            {
                queryString += new string('=', 4 - validateBase64);
            }

            return queryString;
        }

    }
}