using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Syncfusion.Dashboard.Service.Base;
using Syncfusion.Dashboard.Service.Base.DashboardServerHelpers.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Syncfusion.Dashboard.Designer.Web.Service.Helpers
{
    public class DataExtractionHelper
    {
        private RequestHelper requestHelper;
        private Task task;
        private IHubContext<DesignerHub> _hubContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DataExtractionHelper(IHttpContextAccessor httpContextAccessor,IHubContext<DesignerHub> hubContext)
        {
            requestHelper = new RequestHelper();
            _httpContextAccessor = httpContextAccessor;
            _hubContext = hubContext;
        }
        public ApiResponse GetSourceTableInfo(Dictionary<string, object> jsonData)
        {
            task = new Task(() => GetSchema(jsonData, task, _hubContext));
            task.Start();
            return new ApiResponse
            {
                Message = "success",
                Status = true
            };
        }

        public ApiResponse createTableInIntermediateDb(Dictionary<string, object> jsondata)
        {
            task = new Task(() => ProcessData(jsondata, task, _hubContext, _httpContextAccessor.HttpContext.Request));
            task.Start();
            return new ApiResponse
            {
                Message = "success",
                Status = true
            };
        }

        private void GetSchema(Dictionary<string, object> jsonData, Task threadObj, IHubContext<DesignerHub> context)
        {
            requestHelper.request = _httpContextAccessor.HttpContext.Request;
            DashboardDesignerHelper helper = new DashboardDesignerHelper(_httpContextAccessor.HttpContext.Request);
            string clientId = jsonData["clientid"].ToString();
            string datasetId = jsonData.ContainsKey("dataSetId") ? jsonData["dataSetId"].ToString() : string.Empty;
            var response = helper.GetSourceSchemaDetails(jsonData.ContainsKey("datasource") ? jsonData["datasource"] : null, requestHelper.GetClientId(), datasetId);
            context.Clients.Client(clientId).SendAsync("GetStatusFromServer", response);
        }

        private void ProcessData(Dictionary<string, object> jsonData, Task threadObj, IHubContext<DesignerHub> context, HttpRequest request)
        {
            requestHelper.request = _httpContextAccessor.HttpContext.Request;
            DashboardDesignerHelper helper = new DashboardDesignerHelper(request);
            string clientId = jsonData["clientid"].ToString();
            var response = helper.CreateTablesInRemoteServer(jsonData, requestHelper.GetClientId());
            context.Clients.Client(clientId).SendAsync("GetStatusFromServer", response);
        }
    }
}
