using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Syncfusion.Dashboard.Designer.Web.Service.Helpers;
using Syncfusion.Dashboard.Core.Helpers;
using Microsoft.Extensions.Options;

namespace Syncfusion.Dashboard.Designer.Web.Service
{
    public class DesignerHub : Hub
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IHubContext<DesignerHub> _hubContext;
        public DesignerHub(IHttpContextAccessor httpContextAccessor, IOptions<AppSettings> appSettings, IHubContext<DesignerHub> hubContext)
        {
            _httpContextAccessor = httpContextAccessor;
            AppSettingsInfo.AppSetting = appSettings.Value;
            _hubContext = hubContext;
        }

        /// <summary>
        /// Hub implementation for getting schema for extract mode connetions.
        /// </summary>
        /// <param name="jsonData"></param>
        public void ValidateConnection(Dictionary<string, object> jsonData)
        {
            DataExtractionHelper helper = new DataExtractionHelper(_httpContextAccessor, _hubContext);
            this.ProcessHeaders(jsonData);
            helper.GetSourceTableInfo(jsonData);

        }
        /// <summary>
        /// Hub implementation for Creating table in remote server
        /// </summary>
        /// <param name="jsonData"></param>
        public void ProcessFileData(Dictionary<string, object> jsonData)
        {
            DataExtractionHelper helper = new DataExtractionHelper(_httpContextAccessor, _hubContext);
            this.ProcessHeaders(jsonData);
            helper.createTableInIntermediateDb(jsonData);
        }

        /// <summary>
        /// Extracts the header information from the request data and assigns to the request object
        /// </summary>
        /// <param name="jsonData"></param>
        private void ProcessHeaders(Dictionary<string, object> jsonData)
        {
            if (jsonData.ContainsKey("Headers"))
            {
                
                var headers = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>((string)jsonData["Headers"].ToString());
                foreach (var key in headers.Keys)
                {
                   if (!string.IsNullOrEmpty(headers[key]))
                   {
                        _httpContextAccessor.HttpContext.Request.Headers.Add(key, headers[key]);
                   }
                }
                jsonData.Remove("Headers");
            }
        }
    }
}
