// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebDesignerController.cs" company="Syncfusion">
//   
// </copyright>
// <summary>
//   Controller class for handling the requests for creation and viewing of dashboards
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Syncfusion.Dashboard.Designer.Web.Service.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using Syncfusion.Dashboard.Core.Helpers;
    using Syncfusion.Dashboard.Core.Properties;
    using Syncfusion.Dashboard.Designer.Web.Service.CustomAttributes;
    using Syncfusion.Dashboard.Designer.Web.Service.Helpers;
    using Syncfusion.Dashboard.Service;
    using Syncfusion.Dashboard.Service.Base;
    using Syncfusion.Dashboard.Service.Base.DashboardServerHelpers.Model;
    using Syncfusion.Dashboard.Service.Base.Export.Model;
    using Syncfusion.Dashboard.Service.Base.Implementation.ApiController;
    using Syncfusion.Dashboard.Service.Base.Implementation.DataService;
    using Syncfusion.Dashboard.Service.Base.Implementation.Model;
    using Syncfusion.Dashboard.Service.Base.Models;
    using Syncfusion.Dashboard.Service.Base.RefreshDataSource;
    using Syncfusion.Dashboard.Web.Serialization.Model.DOM.Data;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    [ApiVersion("1.0")]
    [Route("v{api-version:apiVersion}/design")]
    [CustomAuthorization]
    [ApiController]
    public class WebDesignerController : ControllerBase
    {
        private RequestHelper requestHelper;
        private DesignHelper designHelper;

        internal WebDesignerController()
        {
            designHelper = new DesignHelper();
            requestHelper = new RequestHelper();
        }

        private readonly ILogger<WebDesignerController> _logger;


        // private readonly IOptions<AppSettings> _appSettings;

        public WebDesignerController(ILogger<WebDesignerController> logger, IOptions<AppSettings> appSettings)
        {

            _logger = logger;
            designHelper = new DesignHelper();
            requestHelper = new RequestHelper();
            AppSettingsInfo.AppSetting = appSettings.Value;
        }

        [HttpHead]
        [HttpPost]
        [Route("validate-dynamic-connection-api")]
        public Task<ApiResponse> validateDynamicConnectionString(Dictionary<string, object> jsonResult)
        {
            return new DashboardDesignerHelper(Request).ValidateDynamicConnection(jsonResult);
        }

        [HttpGet]
        [Route("test")]
        public ActionResult<string> TestAPI()
        {
            return "Designer Service Running Properly.";
        }

        [HttpPost]
        [Route("getnewguid")]
        public ApiResponse GenerateNewGuids()
        {
            // Removed 1st 2 char to shorten the id with 30 char length. 
            // Since oracle connection does not support alias name more than 30 char. 
            return new ApiResponse
            {
                Data = Guid.NewGuid().ToString("N").Remove(0, 2),
                Status = true
            };
        }

        [HttpGet]
        [HttpHead]
        [Route("getdashboard/{id}")]
        public HttpResponseMessage GetDashboardForPreview(string id)
        {
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request);
            HttpResponseMessage responseMessage = new HttpResponseMessage();
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.InternalServerError,
                        Content = new StringContent("Invalid Id")
                    };
                }
                Stream stream = helper.GetPreviewDashboardAsStream(id);
                stream.Position = 0;
                responseMessage.Content = new StreamContent(stream);
                responseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = string.Format("{0}.{1}", id, DashboardConstants.DashboardExtn.ToLowerInvariant())
                };
                responseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                responseMessage.StatusCode = HttpStatusCode.OK;

            }
            catch (Exception ex)
            {
                BaseLogHandler.LogError(ex.ToString(), true, ex, System.Reflection.MethodBase.GetCurrentMethod());
                responseMessage.StatusCode = HttpStatusCode.InternalServerError;
                responseMessage.Content = new StringContent(ex.Message);
            }
            return responseMessage;
        }
        [HttpPost]
        [Route("postdesigneraction")]
        [ValidateAndLoadDashboard]
        public async Task<object> PostDesignerAction(Dictionary<string, object> jsonResult)
        {
          
            requestHelper.request = Request;
            if (jsonResult != null && jsonResult.Count > 0 && (jsonResult["designerAction"].ToString().ToLower() == "openserverreport" || jsonResult["designerAction"].ToString().ToLower() == "saveserverreport" || jsonResult["designerAction"].ToString().ToLower() == "createserverreport")
                && jsonResult.ContainsKey("customData"))
            {
                var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonResult["customData"].ToString());
            }
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request)
            {
                ClientId = requestHelper.GetClientId()
            };
            return await helper.ProcessDesigner(jsonResult, designHelper, GetUrlParameterString(), requestHelper.GetCulture());
        }

        #region SalesForceReportsList
        [HttpGet]
        [Route("salesforce-reports-list/{serviceId}")]
        public ApiResponse GetSalesForceReportsList(string serviceId)
        {
            requestHelper.request = Request;
            DashboardDesignerHelper dashboardHelper = new DashboardDesignerHelper(Request)
            {
                ClientId = requestHelper.GetClientId(),
            };

            return dashboardHelper.GetReportsList(serviceId);

        }
        #endregion

        #region Google Analytics Custom UI
        [HttpGet]
        [Route("ga-accounts-list/{serviceId}")]
        public ApiResponse GetGoogleAnalyticsAccountsList(string serviceId)
        {
            return new DataSourceControllerHelper(Request).GetAnalyticsAccountsList(serviceId);
        }

        [HttpGet]
        [Route("ga-metrics-dimensions/{serviceId}/{accountId}/{webPropertyId}/{connector}")]
        public ApiResponse GetGoogleAnalyticsCustomMetricsDimensionsList(string serviceId, string accountId, string webPropertyId, string connector)
        {
            return new DataSourceControllerHelper(Request).GetCustomMetricsAndDimensions(serviceId, accountId, webPropertyId, connector);
        }
        #endregion

        #region ZohoCRM Modules List
        [HttpGet]
        [Route("zohocrm-modules/{serviceId}")]
        public ApiResponse GetModulesListForZohoCRM(string serviceId)
        {
            return new DataSourceControllerHelper(Request).GetModulesListForZohoCRM(serviceId);
        }
        #endregion

        #region LinkedIn Organizations List
        [HttpGet]
        [Route("linkedin-organizations/{serviceId}")]
        public ApiResponse GetLinkedInOrganizations(string serviceId)
        {
            return new DataSourceControllerHelper(Request).GetOrganizationsListForLinkedIn(serviceId);
        }
        #endregion


        #region Zoho Invoice Domain list
        [HttpGet]
        [Route("zohoinvoice-domainlist/{serviceId}")]
        public ApiResponse GetDomainNameForZohoInvoice(string serviceId)
        {
            return new DataSourceControllerHelper(Request).GetDomainNamesForZohoInvoice(serviceId);
        }
        #endregion
        #region AzureTablesList
        /// <summary>
        /// returns list of tables available in Storage account.
        /// </summary>
        /// <param name="jsonData">Json input has info about storage account name & its access key.</param>
        /// <returns>returns list of tables available in Storage account.</returns>
        [HttpPost]
        [Route("azure-storage-tables")]
        public ApiResponse GetAzureStorageTables(Dictionary<string, object> jsonData)
        {
            requestHelper.request = Request; 
            DashboardDesignerHelper dashboardHelper = new DashboardDesignerHelper(Request)
            {
                ClientId = this.requestHelper.GetClientId(),
            };

            //Validate the inputs
            if (!string.IsNullOrEmpty(jsonData["storageAccountName"].ToString()) && !string.IsNullOrEmpty(jsonData["storageAccountKey"].ToString()))
            {
                return dashboardHelper.GetAzureStorageTables(jsonData["storageAccountName"].ToString(), jsonData["storageAccountKey"].ToString());
            }
            else
            {
                return new ApiResponse
                {
                    Status = false,
                    Message = "Either Storage Account Name or Storage Account Key was empty."
                };
            }
        }
        #endregion

        #region RavenDBDatabasesList
        /// <summary>
        /// returns list of databases available in raven db base URL
        /// </summary>
        /// <param name="jsonData">Json input has info about storage ServerName.</param>
        /// <returns>returns list of databases available in base URL.</returns>
        [HttpPost]
        [Route("ravendb-databases")]
        public ApiResponse GetRavenDBDatabases(Dictionary<string, object> jsonData)
        {
            requestHelper.request = Request; 
            DashboardDesignerHelper dashboardHelper = new DashboardDesignerHelper(Request)
            {
                ClientId = this.requestHelper.GetClientId(),
            };

            //Validate the inputs
            if (jsonData["ServerName"] != null && !string.IsNullOrWhiteSpace(jsonData["ServerName"].ToString()))
            {
                return dashboardHelper.GetRavenDBDatabases(jsonData);
            }
            else
            {
                return new ApiResponse
                {
                    Status = false,
                    Message = "Base URL was empty."
                };
            }
        }
        #endregion

        [HttpGet]
        [Route("facebook-page-list/{serviceId}")]
        public ApiResponse GetFacebookPageList(string serviceId)
        {
            return new DashboardDesignerHelper(Request).GetPageList("facebook", serviceId);
        }

        [HttpGet]
        [Route("facebook-ads-list/{serviceId}")]
        public ApiResponse GetFacebookAdIds(string serviceId)
        {
            return new DashboardDesignerHelper(Request).GetFacebookAdIds("facebookads", serviceId);
        }

        [HttpGet]
        [Route("instagram-account-list/{serviceId}")]
        public ApiResponse GetInstagramAccountIds(string serviceId)
        {
            return new DashboardDesignerHelper(Request).GetInstagramAccountData("instagram", serviceId);
        }

        [HttpPost]
        [Route("instagram-media-list")]
        public ApiResponse GetInstagramMediaIds(Dictionary<string, string> jsonResult)
        {
            return new DashboardDesignerHelper(Request).GetInstagramMediaIds(jsonResult);
        }

        [HttpGet]
        [Route("xero-organization-list/{serviceId}")]
        public ApiResponse GetXeroOrganizationList(string serviceId)
        {
            return new DashboardDesignerHelper(Request).GetXeroOrganizationList("xero", serviceId);
        }

        [HttpGet]
        [Route("get-salesforce-hostname/{serviceId}")]
        public ApiResponse GetSaleforceHostName(string serviceId)
        {
            return new DashboardDesignerHelper(Request).GetSalesforceHostName("salesforce", serviceId);
        }

        [HttpPost]
        [Route("quickbase-table-list")]
        public ApiResponse GetQuickbaseTables(Dictionary<string, string> jsonResult)
        {
            return new DashboardDesignerHelper(Request).GetQuickbaseTables(jsonResult);
        }

        #region Export As CSV      
        [HttpPost]
        [Route("export/csv")]
        [ValidateAndLoadDashboard]
        public ApiResponse ExportAsCsv(Dictionary<string, object> arguments)
        {
            requestHelper.request = Request;
            DashboardDesignerHelper dashboardHelper = new DashboardDesignerHelper(Request)
            {
                ClientId = this.requestHelper.GetClientId()
            };
            LocalizationSettings currentLocalSetting = requestHelper.GetCulture();
            return dashboardHelper.GetExportedCsv(arguments, currentLocalSetting);

        }

        [HttpGet]
        [Route("download/csv")]
        public FileStreamResult DownloadCsvFile([System.Web.Http.FromUri] string arguments)
        {
            try
            {
                DownloadArguments param = JsonConvert.DeserializeObject<DownloadArguments>(arguments);
                var baseLocation = AppDomain.CurrentDomain.BaseDirectory;
                string fileName = Path.GetFileName(param.FilePath) + DashboardConstants.CSVExtension;
#if onpremiseboldbi
                baseLocation = Path.GetFullPath("../../app_data", AppDomain.CurrentDomain.BaseDirectory);
#endif
                bool isPublic = !string.IsNullOrWhiteSpace(this.HttpContext.Request.Headers["IsPublic"]) && this.HttpContext.Request.Headers["IsPublic"].ToString().ToUpperInvariant().Equals("TRUE");
                string fileLocation = isPublic ? Path.Combine(baseLocation, DashboardConstants.TempDir, DashboardConstants.PublicDir, fileName) : Path.Combine(baseLocation, DashboardConstants.TempDir, fileName);
                MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(fileLocation));
                return new FileStreamResult(stream, "application/csv")
                {
                    FileDownloadName = param.FileName + DashboardConstants.CSVExtension
                };
            }
            catch (Exception ex)
            {
                BaseLogHandler.LogError(ex.Message, true, ex, System.Reflection.MethodBase.GetCurrentMethod());
            }

            return null;
        }
        #endregion

        #region View Data
        [HttpPost]
        [Route("viewdata")]
        [ValidateAndLoadDashboard]
        public ApiResponse GetViewData(Dictionary<string, object> arguments)
        {
            requestHelper.request = Request;
            DashboardDesignerHelper dashboardHelper = new DashboardDesignerHelper(Request)
            {
                ClientId = this.requestHelper.GetClientId(),
            };
            LocalizationSettings currentLocalSetting = requestHelper.GetCulture();
            return dashboardHelper.GetViewData(arguments, currentLocalSetting, GetUrlParameterString());
        }
        #endregion

        #region Export As Excel       
        [HttpPost]
        [Route("export/excel")]
        [ValidateAndLoadDashboard]
        public ApiResponse ExportAsExcel(Dictionary<string, object> arguments)
        {
            requestHelper.request = Request;
            DashboardDesignerHelper dashboardHelper = new DashboardDesignerHelper(Request)
            {
                ClientId = this.requestHelper.GetClientId(),
            };
            LocalizationSettings currentLocalSetting = requestHelper.GetCulture();
            return dashboardHelper.GetExportedExcel(arguments, currentLocalSetting);
        }

        [HttpGet]
        [Route("download/excel")]
        public FileStreamResult DownloadExcelFile([System.Web.Http.FromUri] string arguments)
        {
            try
            {
                DownloadArguments param = JsonConvert.DeserializeObject<DownloadArguments>(arguments);
                var baseLocation = AppDomain.CurrentDomain.BaseDirectory;
#if onpremiseboldbi
                baseLocation = Path.GetFullPath("../../app_data", AppDomain.CurrentDomain.BaseDirectory);
#endif
                bool isPublic = !string.IsNullOrWhiteSpace(this.HttpContext.Request.Headers["IsPublic"]) && this.HttpContext.Request.Headers["IsPublic"].ToString().ToUpperInvariant().Equals("TRUE");
                string fileLocation = isPublic ? Path.Combine(baseLocation, DashboardConstants.TempDir, DashboardConstants.PublicDir, Path.GetFileName(param.FilePath)) : Path.Combine(baseLocation, DashboardConstants.TempDir, Path.GetFileName(param.FilePath));
                MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(fileLocation));
                return new FileStreamResult(stream, "application/vnd.ms-excel")
                {
                    FileDownloadName = param.FileName
                };
            }           
            catch (Exception ex)
            {
                BaseLogHandler.LogError(ex.Message, true, ex, System.Reflection.MethodBase.GetCurrentMethod());
            }

            return null;
        }

        #endregion

        [HttpPost]
        [Route("postextractdesigneraction")]
        public async Task<object> PostExtractDesignerAction(Dictionary<string, object> jsonResult)
        {
            requestHelper.request = Request;
            string clientId = requestHelper.GetClientId();
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request);
            return await helper.ProcessExtractDesigner(jsonResult, designHelper, clientId);
        }

        [HttpPost]
        [Route("getcolorparameters")]
        [ValidateAndLoadDashboard]
        public async Task<object> GetColorParameters(Dictionary<string, object> jsonResult)
        {
            requestHelper.request = Request;
            string clientId = requestHelper.GetClientId();
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request);
            return await helper.FetchDataTable(jsonResult, clientId);
        }

        [HttpPost]
        [Route("uploadimage")]
        public DataServiceResponse UploadFileInServer()
        {
            if (HttpContext.Request.Form.Files != null && HttpContext.Request.Form.Files.Count() > 0)
            {
                var fileUploadInfo = HttpContext.Request.Form.Files.First();
                var dd = fileUploadInfo?.OpenReadStream();
                try
                {
                    requestHelper.request = Request;
                    DashboardDesignerHelper designerHelper = new DashboardDesignerHelper(Request);
                    string userid = designerHelper.UserData == null ? "-1" : designerHelper.UserData.UserId;
                    AzureBlobResponse response = designerHelper.response();
                    DataServiceHelper helper = new DataServiceHelper(requestHelper.GetClientId(), null, null, Request);
                    return helper.SaveImage(dd, fileUploadInfo?.FileName, userid, response);
                }
                catch (Exception ex)
                {
                    BaseLogHandler.LogError(ex.Message, true, ex, System.Reflection.MethodBase.GetCurrentMethod());
                    return new DataServiceResponse
                    {
                        Message = DashboardConstants.updateFailed + ". " + DashboardConstants.reasonText + ":" + ex.Message
                    };
                }
            }
            return new DataServiceResponse
            {
                Message = "Invalid or corrupted file"
            };
        }

        [HttpPost]
        [Route("deleteimage")]
        public DataServiceResponse DeleteFileInServer(Dictionary<string, string> jsonData)
        {
            requestHelper.request = Request;
            try
            {
                DashboardDesignerHelper designerHelper = new DashboardDesignerHelper(Request);
                string userid = designerHelper.UserData == null ? "-1" : designerHelper.UserData.UserId;
                AzureBlobResponse response = designerHelper.response();
                DataServiceHelper helper = new DataServiceHelper(requestHelper.GetClientId(), null, null, Request);
                return helper.DeleteImage(Path.GetFileName(jsonData["fileName"]), userid, response);
            }
            catch (Exception ex)
            {
                BaseLogHandler.LogError(ex.Message, true, ex, System.Reflection.MethodBase.GetCurrentMethod());
                return new DataServiceResponse
                {
                    Message = DashboardConstants.deleteFailed + ". " + DashboardConstants.reasonText + ":" + ex.Message
                };
            }
        }
        [HttpPost]
        [Route("assignimage")]
        public DataServiceResponse ReadImagefromSydx(Dictionary<string, string> jsonData)
        {
            requestHelper.request = Request;
            try
            {
                DashboardDesignerHelper designerHelper = new DashboardDesignerHelper(Request);
                string userid = designerHelper.UserData == null ? "-1" : designerHelper.UserData.UserId;
                AzureBlobResponse response = designerHelper.response();
                DataServiceHelper helper = new DataServiceHelper(requestHelper.GetClientId(), null, null, Request);
                return helper.GetImage(Path.GetFileName(jsonData["fileName"]), userid, response, designerHelper.ModeType, designerHelper.CallerUrl, designerHelper.DashboardPath);
            }
            catch (Exception ex)
            {
                BaseLogHandler.LogError(ex.Message, true, ex, System.Reflection.MethodBase.GetCurrentMethod());
                return new DataServiceResponse
                {
                    Message = DashboardConstants.readFailed + ". " + DashboardConstants.reasonText + ":" + ex.Message
                };
            }
        }

        [HttpPost]
        [Route("getdashboardresources")]
        [ValidateAndLoadDashboard]
        public DataServiceResponse GetDashboardResources()
        {
            requestHelper.request = Request;
            try
            {
                DashboardDesignerHelper designerHelper = new DashboardDesignerHelper(Request);
                string userid = designerHelper.UserData == null ? "-1" : designerHelper.UserData.UserId;
                AzureBlobResponse response = designerHelper.response();
                DataServiceHelper helper = new DataServiceHelper(requestHelper.GetClientId(), null, null, Request);
                string imageDataSource = helper.GetImages(userid, response, designerHelper.ModeType, designerHelper.CallerUrl, designerHelper.DashboardPath);
                return new DataServiceResponse
                {
                    Status = true,
                    Data = new
                    {
                        source = imageDataSource
                    }
                };
            }
            catch (Exception ex)
            {
                BaseLogHandler.LogError(ex.Message, true, ex, System.Reflection.MethodBase.GetCurrentMethod());
                return new DataServiceResponse
                {
                    Message = DashboardConstants.readFailed + ". " + DashboardConstants.reasonText + ":" + ex.Message
                };
            }
        }

        [HttpPost]
        [Route("compressimagesize")]
        public DataServiceResponse CompressImageSize()
        {
            try
            {
                requestHelper.request = Request;
                DashboardDesignerHelper designerHelper = new DashboardDesignerHelper(Request);
                string userid = designerHelper.UserData == null ? "-1" : designerHelper.UserData.UserId;
                AzureBlobResponse response = designerHelper.response();
                LocalizationSettings currentLocalSetting = requestHelper.GetCulture();
                string imageProcessResponse = designerHelper.ProcessImageCompression(userid, response, designerHelper.ModeType, designerHelper.CallerUrl, designerHelper.DashboardPath, requestHelper.GetClientId(), currentLocalSetting);
                return new DataServiceResponse
                {
                    Status = true,
                    Message = imageProcessResponse
                };
            }
            catch (Exception ex)
            {
                BaseLogHandler.LogError(ex.Message, true, ex, System.Reflection.MethodBase.GetCurrentMethod());
                return new DataServiceResponse
                {
                    Message = DashboardConstants.processFailed + ". " + DashboardConstants.reasonText + ":" + ex.Message
                };
            }
        }

        [HttpPost]
        [Route("formUrlpattern")]
        public DataServiceResponse FormURLPattern(Dictionary<string, object> jsonData)
        {
            string patternText = null;
            try
            {
                Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonData["Colums"]?.ToString());
                string[] keyArray = new string[dictionary.Count];
                dictionary.Values.CopyTo(keyArray, 0);
                patternText = string.Format(jsonData["UrlText"].ToString(), keyArray);
                return new DataServiceResponse
                {
                    Status = true,
                    Data = new
                    {
                        Status = true,
                        patternText = patternText
                    }
                };

            }
            catch (FormatException ex)
            {
                return new DataServiceResponse
                {
                    Message = DashboardConstants.invalidPattern
                };
            }
            catch (Exception ex)
            {
                BaseLogHandler.LogError(ex.ToString(), true, ex, System.Reflection.MethodBase.GetCurrentMethod());
                return new DataServiceResponse
                {
                    Message = DashboardConstants.readFailed + ". " + DashboardConstants.reasonText + ":" + ex.Message
                };
            }

        }

        [HttpGet]
        [Route("getuserdetails")]
        public ApiResponse GetUserDetails()
        {
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request);
            if (helper.UserData != null)
            {
                return new ApiResponse
                {
                    Status = true,
                    ApiStatus = true,
                    Data = JsonConvert.SerializeObject(helper.UserData)
                };
            }
            else
            {
                return new ApiResponse
                {
                    Status = false,
                    ApiStatus = false,
                    Data = null
                };
            }
        }

        [HttpGet]
        [Route("getrowscount")]
        public ApiResponse GetRowsCount()
        {
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request);
            ////var usedRowsCountResponse = helper.GetUsedRowsCount();
            var maximumRowsCountResponse = helper.GetTotalRowsCount();
            if (maximumRowsCountResponse.Status && maximumRowsCountResponse.Data != null)
            {
                Dictionary<string, object> rowCount = new Dictionary<string, object>();
                var maximumRowCount = Dashboard.Web.Serialization.Utility.JsonConverter.Deserialize<Dictionary<string, object>>(maximumRowsCountResponse.Data.ToString());
                if (maximumRowCount.ContainsKey("Max") && maximumRowCount.ContainsKey("Used"))
                {
                    rowCount.Add("MaxRowSize", maximumRowCount["Max"]);
                    rowCount.Add("UsedRowSize", maximumRowCount["Used"]);
                }
                else
                {
                    return new ApiResponse
                    {
                        Status = false,
                        ApiStatus = false,
                        Message = "Failed to fetch the maximum records count.",
                        Data = null,
                    };
                }
                return new ApiResponse
                {
                    Status = true,
                    ApiStatus = true,
                    Message = "Records count has been fetched Successfully.",
                    Data = rowCount,
                };
            }
            else
            {
                return new ApiResponse
                {
                    Status = false,
                    ApiStatus = false,
                    Message = "Failed to fetch the records count.",
                    Data = null,
                };
            }
        }

        /// <summary>
        /// API to save the design progress.
        /// </summary>
        /// <param name="jsonResult"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("notifypropertychange")]
        public async Task<ResultModel> NotifyPropertyChange(IDictionary<string, object> jsonResult)
        {
            requestHelper.request = Request;
            string clientId = requestHelper.GetClientId();
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request);
            LocalizationSettings currentLocalSetting = requestHelper.GetCulture();
            return await helper.SerializeData(jsonResult, clientId, currentLocalSetting);
        }

        [HttpPost]
        [Route("rowcount-total")]
        [Route("rowcount")]
        [AllowAnonymous]
        public ApiResponse GetRowCount(ItemRequest connectionParameter)
        {
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request);
            return helper.GetRowCount(connectionParameter);
        }

        [HttpPost]
        [Route("linked-accounts")]
        [AllowAnonymous]
        public ApiResponse GetOAuthAccount(object connectionObj)
        {
            ItemRequest connectionParameter = JsonConvert.DeserializeObject<ItemRequest>(connectionObj.ToString());
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request);
            return helper.GetOAuthDetails(connectionParameter);
        }

        [HttpPost]
        [Route("rowcount-perdatasource")]
        [AllowAnonymous]
        public ApiResponse GetRowCountPerDatasource(object connectionObj)
        {
            ItemRequest connectionParameter = JsonConvert.DeserializeObject<ItemRequest>(connectionObj.ToString());
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request);
            return helper.GetRowCountPerDatasource(connectionParameter);
        }

        [HttpPost]
        [Route("publish-mergeds-schema")]
        public ApiResponse PublishMergeDataSourceInfo(IDictionary<string, object> jsonResult)
        {
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request);
            return helper.publishMergeDataSourceInfo(jsonResult);
        }

        [HttpPost]
        [Route("rowcount-perdatasource-guid")]
        [AllowAnonymous]
        public ApiResponse GetRowCountPerDatasource(IDictionary<string, object> jsonResult)
        {
            requestHelper.request = Request;
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request);
            helper.OpenDashboard(new OpenDashboardRequestModel { Path = helper.DashboardPath }, requestHelper.GetClientId());
            if (jsonResult != null && jsonResult.Count > 0)
            {
                if (jsonResult["datasourceList"] != null)
                {
                    var datasourceIdList = JsonConvert.DeserializeObject<List<string>>(jsonResult["datasourceList"].ToString());
                    Dictionary<string, int> rowCountList = new Dictionary<string, int>();
                    try
                    {
                        for (int i = 0; i < datasourceIdList.Count; i++)
                        {
                            Dashboard.Core.DataSources.DataSource Ds = helper.DashboardObject.DataSources.FirstOrDefault(k => k.Id == datasourceIdList[i]);
                            int rowCount = Ds.GetDataSourceTotalRowCount().Result;
                            rowCountList.Add(datasourceIdList[i], (rowCount));

                        }
                        return new ApiResponse
                        {
                            Status = true,
                            ApiStatus = true,
                            Message = "Records count has been fetched Successfully.",
                            Data = rowCountList
                        };
                    }
                    catch (Exception ex)
                    {
                        return new ApiResponse
                        {
                            Status = false,
                            ApiStatus = false,
                            Message = "Exception in fetching records count.",
                            Data = null,
                        };
                    }
                }
            }
            return new ApiResponse
            {
                Status = false,
                ApiStatus = false,
                Message = "Failed to fetch the maximum records count.",
                Data = null,
            };
        }

        [HttpDelete]
        [Route("deletedatasource")]
        [AllowAnonymous]
        public ApiResponse DropTableFromIntermediateDB(object connectionObj)
        {
            ItemRequest connectionParameter = JsonConvert.DeserializeObject<ItemRequest>(connectionObj.ToString());
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request);
            return helper.DropTableFromIntermediateDB(connectionParameter);
        }

        /// <summary>
        /// API to get the designer Instance Unique ID
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("initialize")]
        public object Initialize()
        {
            return new
            {
                Status = true,
                Data = this.requestHelper.Initialize()
            };
        }

        /// <summary>
        /// API to get the preview Dashboard path
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getpreviewdashboard")]
        public object GetPreviewDashboard(Dictionary<string, object> jsonData)
        {
            requestHelper.request = Request;
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request);
            return helper.GetSerializedDashboardPath(requestHelper.GetClientId(), jsonData.ContainsKey("widgetData") ? jsonData["widgetData"].ToString() : null, jsonData.ContainsKey("filterInfo") ? Convert.ToString(jsonData["filterInfo"]) : null);
        }

        /// <summary>
        /// API to log the script errors while working with the web designer
        /// </summary>
        /// <param name="jsonData">Script error list</param>
        /// <returns></returns>
        [HttpPost]
        [Route("trackerrors")]
        public int TrackErrors(object jsonData)
        {
            requestHelper.request = Request;
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request);
            return helper.WriteScriptErrors(jsonData.ToString(), requestHelper.GetClientId());
        }

        /// <summary>
        /// API to download datasource from server.
        /// </summary>
        /// <param name="datasourceId">Selected datasourcce id.</param>
        /// <param name="version">Selected datasource version.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("importconnection/{dataSourceId}")]
        public ApiResponse GetImportDatasource([System.Web.Http.FromUri] string dataSourceId)
        {
            requestHelper.request = Request;
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request)
            {
                ClientId = requestHelper.GetClientId()
            };
            return helper.ImportDatasource(null, new List<string> { dataSourceId }, "");
        }

        [HttpPost]
        [Route("shareddatasources")]
        public ApiResponse GetSharedDataSources(SharedDataSourcePage page)
        {
            requestHelper.request = Request;
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request);
            return helper.GetSharedDatasources(page);
        }

        [HttpPost]
        [Route("importdatasource")]
        public ApiResponse ImportDatasource(SharedDatasource datasource)
        {
            requestHelper.request = Request;
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request)
            {
                ClientId = requestHelper.GetClientId()
            };
            return helper.ImportDatasource(datasource, new List <string> { datasource.Id });
        }

        [HttpPost]
        [Route("importdatasources")]
        public ApiResponse ImportDataSources(List<string> datasource)
        {
            requestHelper.request = Request;
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request)
            {
                ClientId = requestHelper.GetClientId()
            };
            return helper.ImportDatasource(null, datasource);
        }

        [HttpPost]
        [Route("savetoserver")]
        public object SaveDashboard(Dictionary<string, object> jsonData)
        {
            requestHelper.request = Request;
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request);
            try
            {
                return helper.SaveDashboardToServer((string)jsonData["id"]?.ToString(), (string)jsonData["name"]?.ToString(), (string)jsonData["description"]?.ToString(), (string)jsonData["categoryId"]?.ToString(), Convert.ToString(jsonData["widgetData"]?.ToString()), jsonData.ContainsKey("filterInfo") ? Convert.ToString(jsonData["filterInfo"]?.ToString()) : null, requestHelper.GetClientId(), requestHelper.GetCulture(), jsonData.ContainsKey("isPublic") ? Convert.ToBoolean(jsonData["isPublic"]?.ToString()) : false, jsonData.ContainsKey("colorSetJson") ? Convert.ToString(jsonData["colorSetJson"]?.ToString()) : null);
            }
            catch (Exception ex)
            {
                BaseLogHandler.LogError(ex.ToString(), true, ex, System.Reflection.MethodBase.GetCurrentMethod());
                if (IsValidServerException(ex.Message) && Newtonsoft.Json.JsonConvert.DeserializeObject<ErrorMessage>(ex.Message).StatusCode == "2008")
                {
                    return SaveDeletedDashboard(jsonData);
                }
                else
                {
                    return new
                    {
                        Status = false,
                        Message = ex.Message
                    };
                }
            }

        }
        private bool IsValidServerException(string data)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(data))
                {
                    Dashboard.Web.Serialization.Utility.JsonConverter.Deserialize<ErrorMessage>(data);
                    return true;
                }
            }
            catch
            {
                // ignored
            }
            return false;
        }

        public object SaveDeletedDashboard(Dictionary<string, object> jsonData)
        {
            requestHelper.request = Request;
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request);
            try
            {
                //return helper.SaveDashboardToServer("", (string)jsonData["name"], (string)jsonData["description"], (string)jsonData["categoryId"], Convert.ToString(jsonData["widgetData"]), jsonData.ContainsKey("filterInfo") ? Convert.ToString(jsonData["filterInfo"]) : null, requestHelper.GetClientId(), requestHelper.GetCulture(), jsonData.ContainsKey("isPublic") ? Convert.ToBoolean(jsonData["isPublic"]) : false, jsonData.ContainsKey("colorSetJson") ? Convert.ToString(jsonData["colorSetJson"]) : null);
                return helper.SaveDashboardToServer(null, (string)jsonData["name"]?.ToString(), (string)jsonData["description"]?.ToString(), (string)jsonData["categoryId"]?.ToString(), Convert.ToString(jsonData["widgetData"]?.ToString()), jsonData.ContainsKey("filterInfo") ? Convert.ToString(jsonData["filterInfo"]?.ToString()) : null, requestHelper.GetClientId(), requestHelper.GetCulture(), jsonData.ContainsKey("isPublic") ? Convert.ToBoolean(jsonData["isPublic"]?.ToString()) : false, jsonData.ContainsKey("colorSetJson") ? Convert.ToString(jsonData["colorSetJson"]?.ToString()) : null);

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
                return new
                {
                    Status = false,
                    Message = errorMsg
                };
            }
        }

        [HttpPost]
        [Route("getcategories")]
        public ApiResponse GetDashboardCategories([FromBody] string categoryName)
        {
            requestHelper.request = Request;
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request);
            categoryName = categoryName.Equals("test") ? "" : categoryName;
            return helper.GetCategories(requestHelper.GetClientId(), (string)categoryName);
        }

        [HttpPost]
        [Route("createcategory")]
        public object CreateDashboardCategory(Dictionary<string, string> categoryDetails)
        {
            requestHelper.request = Request;
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request);
            return helper.CreateCategory(requestHelper.GetClientId(), (string)categoryDetails["name"], (string)categoryDetails["description"]);
        }

        [HttpGet]
        [Route("getculturescript")]
        public HttpResponseMessage GetCultureScript([System.Web.Http.FromUri] string culture)
        {
            //string filePath = !string.IsNullOrEmpty(AppSettingsValue.AppSetting.AppDataPath) ? AppSettingsValue.AppSetting.AppDataPath : AppDomain.CurrentDomain.BaseDirectory + FileAndDirectoryConstants.CultureDirectory;
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FileAndDirectoryConstants.CultureDirectory);
            if (!string.IsNullOrEmpty(culture))
            {
                string fileName = FileAndDirectoryConstants.CultureFileName + culture + FileAndDirectoryConstants.CultureFileExtension;
                if (System.IO.File.Exists(filePath + fileName))
                {
                    var result = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ByteArrayContent(System.IO.File.ReadAllBytes(Path.Combine(filePath, fileName)))
                    };
                    result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = fileName
                    };
                    result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/javascript");
                    return result;
                }
            }

            return new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent("Culture file not found")

            };
        }


        [HttpPost]
        [Route("loaddashboard")]
        public object Open(OpenDashboardRequestModel requestModel)
        {
            requestHelper.request = Request;
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request);
            LocalizationSettings currentLocalSetting = requestHelper.GetCulture();
            return helper.OpenDashboard(requestModel, requestHelper.GetClientId(), true, null, currentLocalSetting);
        }

        [HttpPost]
        [Route("loaddraft")]
        public object OpenDraft(object ItemId)
        {
            requestHelper.request = Request;
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request);
            LocalizationSettings currentLocalSetting = requestHelper.GetCulture();
            return helper.OpenDashboardFromDraft(ItemId.ToString(), currentLocalSetting);
        }

        [HttpPost]
        [Route("refreshdatasources")]
        [AllowAnonymous]
        public ApiResponse RefreshDataSource(object request)
        {
            try
            {
                RefreshDataSourceHelper helper = new RefreshDataSourceHelper(Request);
                RefreshDataSourceRequest refreshDataSourceRequest = JsonConvert.DeserializeObject<RefreshDataSourceRequest>(request.ToString());
                IHeaderDictionary headers = Request.Headers;
                ApiResponse response = helper.RefreshDatasources(refreshDataSourceRequest, headers);
                response.Message = response.StatusMessage;
                return response;
            }
            catch (Exception exception)
            {
                Dashboard.Service.Base.BaseLogHandler.LogError(exception.Message, true, exception, System.Reflection.MethodBase.GetCurrentMethod());
                return new ApiResponse
                {
                    ApiStatus = true,
                    Status = false,
                    Message = $"An error occurred in refreshing datasource. - {exception.Message}"
                };
            }
        }

        [HttpPost]
        [Route("checkdraft")]
        public bool IsDraftExist(Dictionary<string, object> jsonData)
        {
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request);
            return helper.CheckDraft(Convert.ToString(jsonData["itemId"]));
        }

        [HttpPost]
        [Route("savedatasource")]
        public async Task<object> SaveDataSource([FromBody]Dictionary<string, object> jsonData)
        {
            requestHelper.request = Request;
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request)
            {
                ClientId = requestHelper.GetClientId()
            };
            return helper.SaveDatasourceToServer(jsonData);
        }

        #region Salesforce objects
        [HttpPost]
        [Route("salesforce-objects-list")]
        public object LoadSalesForceObjects(Dictionary<string, object> jsonObj)
        {
            requestHelper.request = Request;
            DashboardDesignerHelper dashboardHelper = new DashboardDesignerHelper(Request)
            {
                ClientId = this.requestHelper.GetClientId(),
            };
            return dashboardHelper.LoadSalesForceObjects(jsonObj);
        }

        [HttpPost]
        [Route("salesforce-fields-list")]
        public object LoadSalesForceFieldsFromObjects(Dictionary<string, object> jsonObj)
        {
            requestHelper.request = Request;
            DashboardDesignerHelper dashboardHelper = new DashboardDesignerHelper(Request)
            {
                ClientId = this.requestHelper.GetClientId(),
            };
            return dashboardHelper.LoadSalesForceFieldsFromObjects(jsonObj);
        }
        #endregion

        [HttpPost]
        [Route("sharedtables")]
        public ApiResponse GetSharedTables(Dictionary<string, object> jsonData)
        {
            requestHelper.request = Request;
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request)
            {
                ClientId = requestHelper.GetClientId()
            };
            return helper.GetSharedTableSchema(jsonData);
        }

        [HttpDelete]
        [Route("linked-accounts")]
        [AllowAnonymous]
        public ApiResponse DeleteOAuthAccount(object requestObj)
        {
            ItemRequest request = JsonConvert.DeserializeObject<ItemRequest>(requestObj.ToString());
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request);
            return helper.DeleteOAuthAccount(request);
        }

        [HttpPut]
        [Route("linked-accounts/{id}")]
        [AllowAnonymous]
        public ApiResponse UpdateOAuthAccountName([System.Web.Http.FromUri] string id, object requestObj)
        {
            ItemRequest request = JsonConvert.DeserializeObject<ItemRequest>(requestObj.ToString());
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request);
            return helper.UpdateOAuthAccountName(id, request);
        }

        [HttpGet]
        [Route("linked-accounts/{provider}")]
        public ApiResponse GetOAuthAccountsForProvider([System.Web.Http.FromUri] string provider)
        {
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request);
            return helper.GetOAuthDetailsForProvider(provider);
        }

        [HttpGet]
        [Route("oauthconf-status/{provider}")]
        public ApiResponse GetOAuthClientStatusForProvider([System.Web.Http.FromUri] string provider)
        {
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request);
            return helper.GetOAuthClientStatusForProvider(provider);
        }

        [HttpPost]
        [Route("previewfeatures")]
        public ApiResponse GetPreviewFeatures(object environment)
        {
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request);
            return helper.GetPreviewFeatures(environment.ToString());
        }

        [HttpPost]
        [Route("getlocalizationformat")]
        public object GetLocalizationFormat()
        {
            requestHelper.request = Request;
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request);
            LocalizationSettings currentLocalSetting = requestHelper.GetCulture();
            ApiResponse response = new ApiResponse();
            response.Data = currentLocalSetting;
            response.ApiStatus = true;
            response.Status = true;
            return JsonConvert.SerializeObject(response);            
        }       

        [HttpPost]
        [Route("getdashboardtheme")]
        public ApiResponse GetDashboardThemeContent(object themeFileName)
        {
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request);
            return helper.GetDashboardThemeContent(themeFileName?.ToString());
        }


        [HttpPost]
        [Route("getdashboardthemenames")]
        public ApiResponse GetAllDashboardThemeNames()
        {
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request);
            return helper.GetAllDashboardThemeNames();
        }


        [HttpPost]
        [Route("customwidget/get-widgets")]
        public ApiResponse GetCustomWidgets(Dictionary<string, string> data)
        {
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request);
            return helper.GetCustomWidgets(data);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("customwidget/download-dependency")]
        public FileStreamResult DownloadCustomWidgetFile(string path)
        {
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request);
            return helper.DownLoadCustomWidgetDependency(path.ToString());
        }

        [HttpPatch]
        [Route("linked-accounts/{id}/{name}")]
        public ApiResponse UpdateOAuthAccount([System.Web.Http.FromUri] string id, [System.Web.Http.FromUri] string name)
        {
            requestHelper.request = Request;
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request)
            {
                ClientId = requestHelper.GetClientId()
            };
            return helper.UpdateOAuthAccountName(id, name);
        }

        [HttpPost]
        [Route("addactivitylog")]
        public ApiResponse ActivityLog(Dictionary<string, object> jsonData)
        {
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request);
            ActivityLog activityInfo = JsonConvert.DeserializeObject<ActivityLog>(jsonData["activityLog"].ToString());
            ApiResponse response = helper.AddActivityLogToServer(activityInfo.Action, activityInfo.ItemId, activityInfo.ItemType, activityInfo.ItemSubType);
            return response;
        }

        [HttpPost]
        [Route("sampledatasource")]
        public ApiResponse GetSampleDataSources()
        {
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request);
            return helper.GetSampleDatasources();
        }

        [HttpPost]
        [Route("getpreviewdata")]
        public ApiResponse GetSamplePreviewData(Dictionary<string, object> jsonData)
        {
            requestHelper.request = Request;
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request);
            return helper.GetPreviewDataforSchemaChange(jsonData, requestHelper.GetClientId());
        }

        [HttpPost]
        [Route("list-drive-files")]
        public ApiResponse GetDriveFiles(DriveFileRequest fileRequest)
        {
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request);
            return helper.GetFiles(fileRequest);
        }

        [HttpPost]
        [Route("drive-file")]
        public ApiResponse DownloadDriveFile(DriveFileRequest fileRequest)
        {
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request);
            return helper.DownloadFile(fileRequest);
        }
        [HttpPost]
        [Route("get-file-size")]
        public ApiResponse GetDriveFileSize(DriveFileRequest fileRequest)
        {
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request);
            return helper.GetFileSize(fileRequest);
        }

        #region Smartsheet File Helper codes

        [HttpPost]
        [Route("list-smartsheet-files")]
        public ApiResponse GetSmartsheetFiles(Dictionary<string, object> jsonData)
        {
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request);
            return helper.GetSmartsheetFiles(jsonData);
        }

        [HttpGet]
        [Route("get-home-items")]
        public ApiResponse GetHomeItems()
        {
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request);
            return helper.GetHomeItems();
        }

        #endregion

        [HttpPost]
        [Route("list-sites")]
        public ApiResponse GetSitesList(DriveFileRequest fileRequest)
        {
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request);
            return helper.GetSitesList(fileRequest);
        }

        [HttpPost]
        [Route("deletedatasource")]
        public object DeleteDataSource(Dictionary<string, object> jsonData)
        {
            requestHelper.request = Request;
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request)
            {
                ClientId = requestHelper.GetClientId()
            };
            return helper.DeleteDataSource(jsonData);
        }
        [HttpPost]
        [Route("datasource/validate-name")]
        public ApiResponse DataSourceNameValidation(object dsName)
        {
            requestHelper.request = Request;
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request)
            {
                ClientId = requestHelper.GetClientId()
            };
            string test = dsName.ToString();
            return helper.DatasourceNameValidation(test);
        }
        #region Export AS Pdf
        [HttpPost]
        [Route("export/pdf")]
        [ValidateAndLoadDashboard]
        public ApiResponse ExportAsPdf(Dictionary<string, object> jsonResult)
        {
            requestHelper.request = Request;
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request)
            {
                ClientId = requestHelper.GetClientId()
            };
            LocalizationSettings currentLocalSetting = requestHelper.GetCulture();
            DashboardInfo data = null;
            if (jsonResult != null && jsonResult.Count > 0)
            {
                if (jsonResult["itemId"] != null && jsonResult["hasWidgetItems"].ToString().ToLower() == "false")
                {
                    data = helper.ReadDrafts(jsonResult["itemId"]?.ToString(), currentLocalSetting);
                }
                return helper.ExportToPdf(jsonResult, data, currentLocalSetting);
            }

            return null;
        }

        [HttpGet]
        [Route("download/pdf")]
        public FileStreamResult DownloadPdfFile([System.Web.Http.FromUri] string arguments)
        {
            try
            {
                DownloadArguments param = JsonConvert.DeserializeObject<DownloadArguments>(arguments);
                var baseLocation = AppDomain.CurrentDomain.BaseDirectory;
#if onpremiseboldbi
                baseLocation = Path.GetFullPath("../../app_data", AppDomain.CurrentDomain.BaseDirectory);
#endif
                bool isPublic = !string.IsNullOrWhiteSpace(this.HttpContext.Request.Headers["IsPublic"]) && this.HttpContext.Request.Headers["IsPublic"].ToString().ToUpperInvariant().Equals("TRUE");
                string fileLocation = isPublic ? Path.Combine(baseLocation, DashboardConstants.TempDir, DashboardConstants.PublicDir, Path.GetFileName(param.FilePath)) : Path.Combine(baseLocation, DashboardConstants.TempDir, Path.GetFileName(param.FilePath));
                MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(fileLocation));
                return new FileStreamResult(stream, "application/pdf")
                {
                    FileDownloadName = param.FileName
                };
            }
            catch (Exception ex)
            {
                BaseLogHandler.LogError(ex.Message, true, ex, System.Reflection.MethodBase.GetCurrentMethod());
            }

            return null;
        }

        #endregion

        #region Export AS Image
        [HttpPost]
        [Route("export/image")]
        [ValidateAndLoadDashboard]
        public ApiResponse ExportAsImage(Dictionary<string, object> jsonResult)
        {
            requestHelper.request = Request;
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request)
            {
                ClientId = requestHelper.GetClientId()
            };
            LocalizationSettings currentLocalSetting = requestHelper.GetCulture();
            DashboardInfo data = null;
            if (jsonResult != null && jsonResult.Count > 0)
            {
                if (jsonResult["itemId"] != null && jsonResult["hasWidgetItems"].ToString().ToLower() == "false") 
                {
                    data = helper.ReadDrafts(jsonResult["itemId"]?.ToString(), currentLocalSetting);
                }
                return helper.ExportToImage(jsonResult, data, currentLocalSetting);
            }

            return null;
        }

        [HttpGet]
        [Route("download/image")]
        public FileStreamResult DownloadImageFile([System.Web.Http.FromUri] string arguments)
        {
            try
            {
                DownloadArguments param = JsonConvert.DeserializeObject<DownloadArguments>(arguments);
                var baseLocation = AppDomain.CurrentDomain.BaseDirectory;
#if onpremiseboldbi
                baseLocation = Path.GetFullPath("../../app_data", AppDomain.CurrentDomain.BaseDirectory);
#endif
                bool isPublic = !string.IsNullOrWhiteSpace(this.HttpContext.Request.Headers["IsPublic"]) && this.HttpContext.Request.Headers["IsPublic"].ToString().ToUpperInvariant().Equals("TRUE");
                string fileLocation = isPublic ? Path.Combine(baseLocation, DashboardConstants.TempDir, DashboardConstants.PublicDir, Path.GetFileName(param.FilePath)) : Path.Combine(baseLocation, DashboardConstants.TempDir, Path.GetFileName(param.FilePath));
                MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(fileLocation));
                return new FileStreamResult(stream, "application/" + param.ImageType)
                {
                    FileDownloadName = param.FileName
                };
            }
            catch (Exception ex)
            {
                BaseLogHandler.LogError(ex.Message, true, ex, System.Reflection.MethodBase.GetCurrentMethod());
            }

            return null;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Get Url parameter string from request headers.
        /// </summary>
        /// <returns></returns>
        private string GetUrlParameterString()
        {
            return System.Web.HttpUtility.UrlDecode(requestHelper.request.Headers["UrlParameter"]);
        }
        #endregion
    }

    public class FileAndDirectoryConstants
    {

        /// <summary>
        /// Culture file name
        /// </summary>
        internal const string CultureFileName = "ej.culture.";

        /// <summary>
        /// Culture file extension
        /// </summary>
        internal const string CultureFileExtension = ".min.js";

        /// <summary>
        /// Culture files directory name
        /// </summary>
        internal const string CultureDirectory = "cultures\\";
    }

}