namespace Syncfusion.Dashboard.Designer.Web.Service.Controllers
{
    using System;
    using System.IO;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Syncfusion.Dashboard.Core.Helpers;
    using Syncfusion.Dashboard.Service.Base;

    [ApiVersion("1.0")]
    [Route("v{api-version:apiVersion}/design")]

    public class JsonShapeController : ControllerBase
    {
        public JsonShapeController(IOptions<AppSettings> appSettings)
        {
            AppSettingsInfo.AppSetting = appSettings.Value;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Reviewed")]
        [HttpGet]
        [Route("mapshape/{shapename}")]
        public FileStreamResult GetMapJsonShapeData([System.Web.Http.FromUri] string shapeName)
        {
            try
            {
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"wwwroot", "shapefiles", shapeName.ToLowerInvariant() + ".js");
                if (string.IsNullOrEmpty(shapeName) || !System.IO.File.Exists(filePath))
                {
                    return null;
                }
                MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(filePath));
                return new FileStreamResult(stream, "text/javascript")
                {
                    FileDownloadName = string.Format("{0}.{1}", shapeName, "js"),
                };
            }
            catch (Exception ex)
            {
                BaseLogHandler.LogError(ex.Message, true, ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
            return null;
        }
    }
}
