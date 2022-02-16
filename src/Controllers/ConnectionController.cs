// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionController.cs" company="Syncfusion">
//   
// </copyright>
// <summary>
//   Controller class for handling the Template dashboard connection validation and processing.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Syncfusion.Dashboard.Designer.Web.Service.Controllers
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Xml.Linq;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Syncfusion.Dashboard.Core.Helpers;
    using Syncfusion.Dashboard.Core.Helpers.Security;
    using Syncfusion.Dashboard.Service.Base;
    using Syncfusion.Dashboard.Service.Base.DashboardServerHelpers.Model;
    using Syncfusion.Dashboard.Service.Base.OAuth;
    using Syncfusion.Dashboard.Web.Data.Handler.Model;
    using Syncfusion.Dashboard.Web.OAuth;
    using Syncfusion.Dashboard.Web.OAuth.Model;

    /// <summary>
    ///  Controller class for handling the Template dashboard connection validation and processing.
    /// </summary>
    [ApiVersion("1.0")]
    [Route("v{api-version:apiVersion}/connection")]
    [ApiController]
    public class ConnectionController : ControllerBase
    {
        private IHostingEnvironment Environment;
        public ConnectionController(IOptions<AppSettings> appSettings, IHostingEnvironment environment)
        {
            AppSettingsInfo.AppSetting = appSettings.Value;
            Environment = environment;
        }

#if BOLDBIDOMAIN
        public const string Domain = "Bold BI";
#else
        public const string Domain = "Syncfusion Dashboards";
#endif
        /// <summary>
        /// API for navigating the user to the login page for template dashboards own data connection 
        /// </summary>
        /// <param name="model">Holds the details about the current provider and the service required for creating the template dashbard</param>
        /// <returns>The HTML page where the user can input the connection details.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Reviewed")]
        [HttpGet]
        [Route("authorize")]
        public ContentResult Authorize([FromQuery] ConnectionModel model)
        {
            StringBuilder segmentBuilder = new StringBuilder();

            var absoluteUri = string.Concat(
                        HttpContext.Request.Scheme,
                        "://",
                        HttpContext.Request.Host.ToUriComponent(),
                        HttpContext.Request.PathBase.ToUriComponent(),
                        HttpContext.Request.Path.ToUriComponent(),
                        HttpContext.Request.QueryString.ToUriComponent());

            Uri url = new Uri(absoluteUri);
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

            var response = new ContentResult();
            model.DesignerService = new UriBuilder(url.Scheme, url.Host, url.Port, segmentBuilder.ToString()).ToString();
            string fileContent = System.IO.File.ReadAllText(Environment.ContentRootPath + "/Connection/Authorize.html");
            XElement ele = XElement.Parse(fileContent);
            var titleTag = (from e in ele.Elements("head").Elements()
                            where (e.Name == "title")
                            select e).FirstOrDefault();
            if (titleTag != null)
            {
                titleTag.Value = Domain;
            }
            var scriptTag = (from e in ele.Elements("head").Elements() where (e.Name == "script" && e.Attribute("id")?.Value == "target") select e).FirstOrDefault();
            scriptTag.Value = string.Format("initialize({0})", string.IsNullOrEmpty(model?.Provider) ? null : Newtonsoft.Json.JsonConvert.SerializeObject(model));
            var timeStampUtc = DateTime.UtcNow.ToString("ddMMyyyymmhhss", System.Globalization.CultureInfo.InvariantCulture);
            var authorizeScript = (from e in ele.Elements("head").Elements() where (e.Name == "script" && e.Attribute("id")?.Value == "authorize_script_tag") select e).FirstOrDefault();
            var styleTag = (from e in ele.Elements("head").Elements() where (e.Name == "link" && e.Attribute("id")?.Value == "style_link_tag") select e).FirstOrDefault();
            authorizeScript?.SetAttributeValue("src", string.Format(authorizeScript.FirstAttribute.Value.ToString() + "?v={0}", timeStampUtc));
            styleTag?.SetAttributeValue("href", string.Format(styleTag.FirstAttribute.Value.ToString() + "?v={0}", timeStampUtc));
            response.Content = "<!DOCTYPE html>\r\n" + ele.ToString();
            response.ContentType = "text/html";
            return response;
        }

        /// <summary>
        /// API to post the  information entered by the user to the parent window.
        /// </summary>
        /// <param name="model">Holds the authentication details given by the user</param>
        /// <returns>HTML page for passing the data to the parent window</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Reviewed")]
        [HttpGet]
        [Route("validate")]
        public ContentResult EncryptAuthorizationData([FromQuery] ConnectionModel model)
        {
            var response = new ContentResult();
            if (model != null)
            {
                response.Content = "<script>if(window.opener){window.opener.postMessage('" + Newtonsoft.Json.JsonConvert.SerializeObject(new { Status = true, Code = TokenCryptoHelper.DoEncryption(Newtonsoft.Json.JsonConvert.SerializeObject(model)) }) + "','" + model.Origin + "');}window.close();</script>";
                response.ContentType = "text/html";
            }
            else
            {
                response.Content = "<error>There was an error reading the authentication information</error>";
                response.ContentType = "text/html";
            }

            return response;
        }

        /// <summary>
        /// API to validate the credentials entered by the user.
        /// </summary>
        /// <param name="model">Holds the credentials entered by the user </param>
        /// <returns>ApiResponse class object</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Reviewed")]
        [HttpGet]
        [Route("validate-credentials")]
        public ApiResponse ValidateAuthorizationData([FromQuery] ConnectionModel model)
        {
            ApiResponse response = new ApiResponse() { ApiStatus = true };
            try
            {
                switch (model?.Provider.ToLower())
                {
                    case "jira":
                    case "zendesk":
                        var data = Newtonsoft.Json.JsonConvert.DeserializeObject<Syncfusion.Dashboard.Service.Base.Implementation.Model.ConnectionData>(model?.Data);
                        string api = data.Url + (model?.Provider.ToLower() == "jira" ? "/rest/api/2/search" : "/api/v2/users.json");
                        var httpWebResponse = (HttpWebRequest)HttpWebRequest.Create(api);

                        if (!string.IsNullOrEmpty(data.UserName) && !string.IsNullOrEmpty(data.Password))
                        {
                            string authenticatedString = string.Empty;
                            byte[] userCredential = Encoding.UTF8.GetBytes(data.UserName + ":" + data.Password);
                            authenticatedString = Convert.ToBase64String(userCredential);
                            httpWebResponse.Headers.Add("Authorization", "Basic " + authenticatedString);
                        }

                        Syncfusion.Dashboard.Service.Base.DashboardServerHelpers.CustomWebClient webClient = new Dashboard.Service.Base.DashboardServerHelpers.CustomWebClient();
                        HttpWebResponse result = (HttpWebResponse)httpWebResponse.GetResponse();
                        break;
                    case "salesforce":
                        DashboardDesignerHelper helper = new DashboardDesignerHelper(Request)
                        {
                            ClientId = Guid.NewGuid().ToString()
                        };
                        var oAuthData = Newtonsoft.Json.JsonConvert.DeserializeObject<Syncfusion.Dashboard.Service.Base.Implementation.Model.ConnectionData>(model?.Data);
                        OAuth2Helper auth2Helper = new OAuth2Helper(new OAuth2RequestParams { Provider = (OAuthProviderType)Enum.Parse(typeof(OAuthProviderType), model.Provider, true), Service = model.Service }, DashboardDesignerHelper.GetOAuthConfigurationModel(Request));

                        ConnectionStringBuilder connectionStringBuilder = helper.GetLookupTableConnection();

                        var oAuthCredentials = new OAuthSettingsWriter().GetOAuthUserCredentials(helper.UserData, helper.UserData?.Token, connectionStringBuilder, Request);
                        var cred = auth2Helper.GetOAuthUserCredential(oAuthCredentials, oAuthData.ServiceId);
                        if (cred != null)
                        {
                            auth2Helper.RefreshAccessToken(cred.RefreshToken, cred);
                        }

                        break;
                    case "github":
                        var githubdata = Newtonsoft.Json.JsonConvert.DeserializeObject<Syncfusion.Dashboard.Service.Base.Implementation.Model.ConnectionData>(model?.Data);
                        string githubapi = "https://api.github.com/user";
                        var githubHttpWebResponse = (HttpWebRequest)HttpWebRequest.Create(githubapi);

                        if (!string.IsNullOrEmpty(githubdata.UserName) && !string.IsNullOrEmpty(githubdata.Password))
                        {
                            string authenticatedString = string.Empty;
                            byte[] userCredential = Encoding.UTF8.GetBytes(githubdata.UserName + ":" + githubdata.Password);
                            authenticatedString = Convert.ToBase64String(userCredential);
                            githubHttpWebResponse.Headers.Add("Authorization", "Basic " + authenticatedString);
                            githubHttpWebResponse.UserAgent = "Syncfusion Dashboards";
                        }

                        Syncfusion.Dashboard.Service.Base.DashboardServerHelpers.CustomWebClient githubWebClient = new Dashboard.Service.Base.DashboardServerHelpers.CustomWebClient();
                        HttpWebResponse githubResult = (HttpWebResponse)githubHttpWebResponse.GetResponse();
                        break;
                    case "twilio":
                        var twilioData = Newtonsoft.Json.JsonConvert.DeserializeObject<Syncfusion.Dashboard.Service.Base.Implementation.Model.ConnectionData>(model?.Data);
                        string twilioApi = "https://api.twilio.com/2010-04-01/Accounts/" + twilioData.UserName + "/Messages.json";
                        var twilioHttpWebResponse = (HttpWebRequest)HttpWebRequest.Create(twilioApi);

                        if (!string.IsNullOrEmpty(twilioData.UserName) && !string.IsNullOrEmpty(twilioData.Password))
                        {
                            string authenticatedString = string.Empty;
                            byte[] userCredential = Encoding.UTF8.GetBytes(twilioData.UserName + ":" + twilioData.Password);
                            authenticatedString = Convert.ToBase64String(userCredential);
                            twilioHttpWebResponse.Headers.Add("Authorization", "Basic " + authenticatedString);
                        }

                        Syncfusion.Dashboard.Service.Base.DashboardServerHelpers.CustomWebClient twilioWebClient = new Dashboard.Service.Base.DashboardServerHelpers.CustomWebClient();
                        HttpWebResponse twilioResult = (HttpWebResponse)twilioHttpWebResponse.GetResponse();
                        break;
                    case "sendgrid":
                        var sendgridData = Newtonsoft.Json.JsonConvert.DeserializeObject<Syncfusion.Dashboard.Service.Base.Implementation.Model.ConnectionData>(model?.Data);
                        string sendgridApi = "https://api.sendgrid.com/v3/campaigns?limit=100";
                        var sendgridHttpWebResponse = (HttpWebRequest)HttpWebRequest.Create(sendgridApi);

                        if (!string.IsNullOrEmpty(sendgridData.UserName) && !string.IsNullOrEmpty(sendgridData.Password))
                        {
                            string authenticatedString = string.Empty;
                            byte[] userCredential = Encoding.UTF8.GetBytes(sendgridData.UserName + ":" + sendgridData.Password);
                            authenticatedString = Convert.ToBase64String(userCredential);
                            sendgridHttpWebResponse.Headers.Add("Authorization", "Basic " + authenticatedString);
                        }

                        Syncfusion.Dashboard.Service.Base.DashboardServerHelpers.CustomWebClient sendgridWebClient = new Dashboard.Service.Base.DashboardServerHelpers.CustomWebClient();
                        HttpWebResponse sendgridResult = (HttpWebResponse)sendgridHttpWebResponse.GetResponse();
                        break;
                    case "servicenow":
                        var servicenowData = Newtonsoft.Json.JsonConvert.DeserializeObject<Syncfusion.Dashboard.Service.Base.Implementation.Model.ConnectionData>(model?.Data);
                        string servicenowApi = servicenowData.Url;
                        var servicenowHttpWebResponse = (HttpWebRequest)HttpWebRequest.Create(servicenowApi);

                        if (!string.IsNullOrEmpty(servicenowData.UserName) && !string.IsNullOrEmpty(servicenowData.Password))
                        {
                            byte[] userCredential = Encoding.UTF8.GetBytes(servicenowData.UserName + ":" + servicenowData.Password);
                            string authenticatedString = Convert.ToBase64String(userCredential);
                            servicenowHttpWebResponse.Headers.Add("Authorization", "Basic " + authenticatedString);
                        }

                        Syncfusion.Dashboard.Service.Base.DashboardServerHelpers.CustomWebClient servicenowWebClient = new Dashboard.Service.Base.DashboardServerHelpers.CustomWebClient();
                        HttpWebResponse servicenowResult = (HttpWebResponse)servicenowHttpWebResponse.GetResponse();
                        break;
                    default:
                        response.Status = true;
                        break;
                }
            }
            catch (Exception ex)
            {
                response.ApiStatus = false;
                response.Message = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// API to gather the list of accounts under the Google Analytics account.
        /// </summary>
        /// <param name="request">Holds the AnalyticsAccountSettings object that has the service ID for the account.</param>
        /// <returns>ApiResponse class object</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Reviewed")]
        [HttpPost]
        [Route("analytics/list-accounts")]
        public ApiResponse GetGAAccounts(AnalyticsAccountSettings request)
        {
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request);
            return helper.GetAnalyticsAccounts(request);
        }

        /// <summary>
        /// API to gather the list of repositories under the GitHub account.
        /// </summary>
        /// <param name="request">Holds the GitHubRepositoriesSettings object that has Username and Access Token for the account.</param>
        /// <returns>ApiResponse class object</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Reviewed")]
        [HttpPost]
        [Route("github/list-repositories")]
        public ApiResponse GetGitHubRepositories(GitHubRepositoriesSettings request)
        {
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request);
            return helper.GetGitHubRepositories(request);
        }

        /// <summary>
        /// API to gather the list of accounts under the Google Analytics account.
        /// </summary>
        /// <param name="request">Holds the AnalyticsAccountSettings object that has the service ID for the account.</param>
        /// <returns>ApiResponse class object</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Reviewed")]
        [HttpPost]
        [Route("xero/list-organizations")]
        public ApiResponse GetXeroOrganizations(AnalyticsAccountSettings request)
        {
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request);
            return helper.GetXeroOrganizationsList(request);
        }
        /// <summary>
        /// API to return the list of properties under the given GA sub account.
        /// </summary>
        /// <param name="request">Holds the AnalyticsAccountSettings object that has the sub account id for the account</param>
        /// <returns>ApiResponse class object</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Reviewed")]
        [HttpPost]
        [Route("analytics/list-properties")]
        public ApiResponse GetGAProperties(AnalyticsAccountSettings request)
        {
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request);
            return helper.GetAnalyticsProperties(request);
        }

        /// <summary>
        /// API to return the list of views for the given GA property ID
        /// </summary>
        /// <param name="request">Holds the AnalyticsAccountSettings object that has the property id for the account</param>
        /// <returns>ApiResponse class object</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Reviewed")]
        [HttpPost]
        [Route("analytics/list-views")]
        public ApiResponse GetGAViews(AnalyticsAccountSettings request)
        {
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request);
            return helper.GetAnalyticsViews(request);
        }
    }
}