namespace Syncfusion.Dashboard.Designer.Web.Service.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.Json;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Syncfusion.Dashboard.Core.Helpers;
    using Syncfusion.Dashboard.Designer.Web.Service.CustomAttributes;
    using Syncfusion.Dashboard.Service.Base;
    using Syncfusion.Dashboard.Service.Base.DashboardServerHelpers.Model;
    using Syncfusion.Dashboard.Service.Base.DataAlerts;
    using Syncfusion.Dashboard.Service.Base.DataAlerts.Helpers;

    [ApiVersion("1.0")]
    [Route("v{api-version:apiVersion}/dataalert")]
    [CustomAuthorization]
    [ApiController]
    public class DataAlertController : ControllerBase
    {
        public DataAlertController(IOptions<AppSettings> appSettings)
        {
            AppSettingsInfo.AppSetting = appSettings.Value;
        }
        [HttpPost]
        [Route("getallwidgets")]
        public ApiResponse GetAllWidgetInformation()
        {
            ApiResponse response;

            try
            {
                DataAlerts dataAlerts = new DataAlerts(Request);
                response = dataAlerts.GetAllWidgetInformation();
            }
            catch (Exception exception)
            {
                response = new ApiResponse()
                {
                    ApiStatus = true,
                    Message = exception.Message,
                    Status = false,
                };
                Dashboard.Service.Base.BaseLogHandler.LogError(exception.Message, true, exception, System.Reflection.MethodBase.GetCurrentMethod());
            }
            return response;
        }

        [HttpPost]
        [Route("getmeasurefields")]
        public ApiResponse GetAllMeasureFieldsInSelectedWidget([FromBody] JsonElement jsonData)
        {
            ApiResponse response;

            try
            {
                JObject data = JsonConvert.DeserializeObject<JObject>(jsonData.GetRawText());
                string widgetId = data["widgetId"]?.ToObject<string>();
                string datasourceId = data["datasourceId"]?.ToObject<string>();

                DataAlerts dataAlerts = new DataAlerts(Request);
                response = dataAlerts.GetAllMeasureFieldsInSelectedWidget(widgetId, datasourceId);
            }
            catch (Exception exception)
            {
                response = new ApiResponse()
                {
                    ApiStatus = true,
                    Message = exception.Message,
                    Status = false,
                };
                Dashboard.Service.Base.BaseLogHandler.LogError(exception.Message, true, exception, System.Reflection.MethodBase.GetCurrentMethod());
            }
            return response;
        }

        [HttpPost]
        [Route("getdatasourcefields")]
        public ApiResponse GetDatasourceFieldCollection([FromBody] JsonElement jsonData)
        {
            ApiResponse response;

            try
            {
                JObject data = JsonConvert.DeserializeObject<JObject>(jsonData.GetRawText());
                string datasourceId = data["datasourceId"]?.ToObject<string>();
                string widgetId = data["widgetId"]?.ToObject<string>();
                DataAlerts dataAlerts = new DataAlerts(Request);
                if (string.IsNullOrEmpty(widgetId))
                    response = dataAlerts.GetDatasourceFieldCollection(datasourceId);
                else
                    response = dataAlerts.GetDimensionFields(widgetId, datasourceId);
            }
            catch (Exception exception)
            {
                response = new ApiResponse()
                {
                    ApiStatus = true,
                    Message = exception.Message,
                    Status = false,
                };
                Dashboard.Service.Base.BaseLogHandler.LogError(exception.Message, true, exception, System.Reflection.MethodBase.GetCurrentMethod());
            }
            return response;
        }

        [HttpPost]
        [Route("getexpressionfunctions")]
        public ApiResponse GetExpressionFunctions()
        {
            ApiResponse response;

            try
            {
                DataAlerts dataAlerts = new DataAlerts(Request);
                response = dataAlerts.GetExpressionFunctions();
            }
            catch (Exception exception)
            {
                response = new ApiResponse()
                {
                    ApiStatus = true,
                    Message = exception.Message,
                    Status = false,
                };
                Dashboard.Service.Base.BaseLogHandler.LogError(exception.Message, true, exception, System.Reflection.MethodBase.GetCurrentMethod());
            }
            return response;
        }

        [HttpPost]
        [Route("getexpressioncolumninfo")]
        public ApiResponse GetExpressionColumnInfo([FromBody] JsonElement jsonData)
        {
            ApiResponse response;

            try
            {
                JObject data = JsonConvert.DeserializeObject<JObject>(jsonData.GetRawText());
                string datasourceId = data["datasourceId"]?.ToObject<string>();
                string expressionName = data["expressionName"]?.ToObject<string>();
                string expressionText = data["expressionText"]?.ToObject<string>();

                DataAlerts dataAlerts = new DataAlerts(Request);
                response = dataAlerts.GetExpressionColumnInfo(datasourceId, expressionName, expressionText);
            }
            catch (Exception exception)
            {
                response = new ApiResponse()
                {
                    ApiStatus = true,
                    Message = exception.Message,
                    Status = false,
                };
                Dashboard.Service.Base.BaseLogHandler.LogError(exception.Message, true, exception, System.Reflection.MethodBase.GetCurrentMethod());
            }
            return response;
        }

        [HttpPost]
        [Route("getvaluesforselectedcolumn")]
        public ApiResponse GetValuesForSelectedColumn([FromBody] JsonElement jsonData)
        {
            ApiResponse response;

            try
            {
                JObject data = JsonConvert.DeserializeObject<JObject>(jsonData.GetRawText());
                string datasourceId = data["datasourceId"]?.ToObject<string>();
                DaDatasourceFieldInfo datasourceFieldInfo = data["datasourceFieldInfo"]?.ToObject<DaDatasourceFieldInfo>();

                DataAlerts dataAlerts = new DataAlerts(Request);
                response = dataAlerts.GetValuesForSelectedColumn(datasourceId, datasourceFieldInfo);
            }
            catch (Exception exception)
            {
                response = new ApiResponse()
                {
                    ApiStatus = true,
                    Message = exception.Message,
                    Status = false,
                };
                Dashboard.Service.Base.BaseLogHandler.LogError(exception.Message, true, exception, System.Reflection.MethodBase.GetCurrentMethod());
            }
            return response;
        }

        [HttpPost]
        [Route("evaluatedataalertmodel")]
        public ApiResponse EvaluateDataAlert([FromBody] JsonElement jsonData)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                JObject data = JsonConvert.DeserializeObject<JObject>(jsonData.GetRawText());
                response = EvaluateDataAlertModel(data);
            }
            catch (Exception exception)
            {
                response = new ApiResponse()
                {
                    ApiStatus = true,
                    Message = exception.Message,
                    Status = false,
                };
                Dashboard.Service.Base.BaseLogHandler.LogError(exception.Message, true, exception, System.Reflection.MethodBase.GetCurrentMethod());
            }
            return response;
        }

        [HttpPost]
        [Route("evaluatecondition")]
        [AllowAnonymous]
        public ApiResponse EvaluateCondition([FromBody] JsonElement jsonData)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                JObject data = JsonConvert.DeserializeObject<JObject>(jsonData.GetRawText());
                if (data == null)
                {
                    return new ApiResponse()
                    {
                        ApiStatus = true,
                        Message = "Parameter passed is empty",
                        Status = false,
                    };
                }
                AddDashboardPathtoHeader(data["ItemId"]?.ToString(), data["Version"]?.ToString());
                HttpRequest request = Request;
                response = new ServerController(request).ValidateEncryptionStringForScheduleExport(data["EncryptedString"]?.ToString(), null);
                response = response.Status ? EvaluateDataAlertModel(data) : response;
            }
            catch (Exception exception)
            {
                response = new ApiResponse()
                {
                    ApiStatus = true,
                    Message = $"An error occurred in evaluating data alert. - {exception.Message}",
                    Status = false,
                };
                Dashboard.Service.Base.BaseLogHandler.LogError(exception.Message, true, exception, System.Reflection.MethodBase.GetCurrentMethod());
            }
            return response;
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

        [HttpPost]
        [Route("updatedataalertmodel")]
        [AllowAnonymous]
        public ApiResponse UpdateDataAlertModel([FromBody] JsonElement jsonData)
        {
            ApiResponse response;
            try
            {
                JObject data = JsonConvert.DeserializeObject<JObject>(jsonData.GetRawText());
                DashboardDesignerHelper helper = new DashboardDesignerHelper(Request);
                DataAlerts dataAlerts = new DataAlerts(Request);
                DaConditionalResult conditionalResult = data["daConditionalResult"]?.ToObject<DaConditionalResult>();
                var locale = new Helpers.RequestHelper().GetCulture();
                response = dataAlerts.UpdateDataAlertModel(conditionalResult, helper, locale);
            }
            catch (Exception exception)
            {
                response = new ApiResponse()
                {
                    ApiStatus = true,
                    Message = exception.Message,
                    Status = false,
                };
                Dashboard.Service.Base.BaseLogHandler.LogError(exception.Message, true, exception, System.Reflection.MethodBase.GetCurrentMethod());
            }
            return response;
        }

        private ApiResponse EvaluateDataAlertModel(JObject data)
        {
            ApiResponse response;
            try
            {
                DataAlerts dataAlerts = new DataAlerts(Request);
                DaInfo daInfo = new DaInfo();
                DashboardDesignerHelper helper;
                if (data["ItemId"] != null && data["Version"] != null)
                {
                    string version = data["Version"]?.ToObject<string>();
                    string itemId = data["ItemId"]?.ToObject<string>();
                    string path = itemId + "/" + version;
                    helper = new DashboardDesignerHelper(path, itemId, Request);
                    DaJsonModel DaJsonArgs = JObject.Parse(data["DaJsonString"].ToString())?.ToObject<DaJsonModel>();
                    List<FilterModel> filters = data["Filters"]?.ToObject<List<FilterModel>>();
                    daInfo.Category = DaJsonArgs.Category;
                    daInfo.DaConditionalResult = DaJsonArgs.DaConditionalResult;
                    daInfo.WidgetInfo = new SelectedWidgetInfo()
                    {
                        WidgetId = DaJsonArgs.WidgetId,
                        DatasourceId = DaJsonArgs.DatasourceId
                    };
                    daInfo.Filters = filters;
                }
                else
                {
                    string widgetId = data["widgetId"]?.ToObject<string>();
                    string datasourceId = data["datasourceId"]?.ToObject<string>();
                    DaConditionalResult conditionalResult = data["conditionalResult"]?.ToObject<DaConditionalResult>();
                    string conditionCategory = data["conditionCategory"]?.ToObject<string>();
                    daInfo.Category = conditionCategory;
                    daInfo.DaConditionalResult = conditionalResult;
                    daInfo.WidgetInfo = new SelectedWidgetInfo()
                    {
                        WidgetId = widgetId,
                        DatasourceId = datasourceId
                    };
                    daInfo.Filters = null;
                    helper = new DashboardDesignerHelper(Request);
                }
                var locale = new Helpers.RequestHelper().GetCulture();
                response = dataAlerts.EvaluateCondition(daInfo, helper, locale);
            }
            catch (Exception exception)
            {
                response = new ApiResponse()
                {
                    ApiStatus = true,
                    Message = $"An error occurred in evaluating data alert. - {exception.Message}",
                    Status = false,
                };
                Dashboard.Service.Base.BaseLogHandler.LogError(exception.Message, true, exception, System.Reflection.MethodBase.GetCurrentMethod());
            }
            return response;
        }
    }
}