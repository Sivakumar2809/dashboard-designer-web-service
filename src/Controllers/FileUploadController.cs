// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileUploadController.cs" company="Syncfusion">
//   
// </copyright>
// <summary>
//   Controller class for handling the physical file processing 
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Syncfusion.Dashboard.Designer.Web.Service.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Syncfusion.Dashboard.Core;
    using Syncfusion.Dashboard.Core.Connection;
    using Syncfusion.Dashboard.Core.DataSources;
    using Syncfusion.Dashboard.Core.Helpers;
    using Syncfusion.Dashboard.CoreHelpers;
    using Syncfusion.Dashboard.Designer.Web.Service.CustomAttributes;
    using Syncfusion.Dashboard.Service.Base;
    using Syncfusion.Dashboard.Service.Base.DashboardServerHelpers;
    using Syncfusion.Dashboard.Service.Base.Implementation.DataService;

    [ApiVersion("1.0")]
    [Route("v{api-version:apiVersion}/datahandler")]
    [CustomAuthorization]
    public class FileUploadController : ControllerBase
    {
        public FileUploadController(IOptions<AppSettings> appSettings)
        {
            AppSettingsInfo.AppSetting = appSettings.Value;
        }
        public DashboardContainer DashboardContainer { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        [HttpPost]
        [Route("upload")]
        [RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        public DataServiceResponse UploadFileInServer()
        {

            if (HttpContext.Request.Form.Files != null && HttpContext.Request.Form.Files.Count() > 0)
            {
                var fileUploadInfo = HttpContext.Request.Form.Files.First();
                var streamFile = fileUploadInfo?.OpenReadStream();
                string clientId = GetClientId();
                try
                {
                    DataServiceHelper helper = new DataServiceHelper(clientId, null, null, Request);
                    return helper.SaveFile(streamFile, fileUploadInfo?.FileName);
                }
                catch (Exception ex)
                {
                    BaseLogHandler.LogError(ex.Message, true, ex, System.Reflection.MethodBase.GetCurrentMethod());
                    return new DataServiceResponse
                    {
                        Message = "Upload Failed. Reason:" + ex.Message
                    };
                }
            }
            return new DataServiceResponse
            {
                Message = "Invalid or corrupted file"
            };
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Reviewed")]
        [HttpPost]
        [Route("certificateUpload")]
        //[ServerAuthenticationFilter]
        public DataServiceResponse UploadCertificateFileInServer()
        {
            DashboardDesignerHelper helper = new DashboardDesignerHelper(Request);
            if (HttpContext.Request.Form.Files != null && HttpContext.Request.Form.Files.Count() > 0)
            {
                var fileUploadInfo = HttpContext.Request.Form.Files.First();
                var streamFile = fileUploadInfo?.OpenReadStream();

                try
                {
                    return helper.SaveCertificateFile(streamFile, fileUploadInfo?.FileName);
                }
                catch (Exception ex)
                {
                    BaseLogHandler.LogError(ex.Message, true, ex, System.Reflection.MethodBase.GetCurrentMethod());
                    return new DataServiceResponse
                    {
                        Message = "Upload Failed. Reason:" + ex.Message
                    };
                }
            }
            return new DataServiceResponse
            {
                Message = "Invalid or corrupted file"
            };
        }

        [HttpPost]
        [Route("update")]
        //[FileServiceAuthorizationFilter]
        public UpdateResult UpdateDataInServer(DataSourceAndDashboardContainer container)
        {
            var dataSource = container.DataSource;
            DashboardContainer = CoreConfiguration.Instance.CoreRepository.LoadDashboardFromString(container.DashboardContainer);
            UpdateResult result = new UpdateResult();

            var ClientId = GetClientId();
            var dataProvider = dataSource.Provider;

            ConnectParameters connectParameters = GetConnectParameters(dataSource, ClientId);
            var fileConnection = CreateNewConnection(dataProvider, connectParameters);
            DashboardContainer.New();
            DashboardContainer.Connections.Add(fileConnection);
            var schemaCollection = fileConnection?.GetSchemaAsync();

            var sqlConnectionBase = CreateNewConnection("sql", connectParameters);

            Core.DataSources.DataSource dataSet = new Core.DataSources.DataSource(DashboardContainer) { Id = new Guid().ToString() };
            var updateResult = dataSet?.GetDataTableAsync(dataSet?.GetVisibleSourceFields().Union(dataSet.Expressions));

            result.UpdatedTableInfo = UpdateDataSetInfoInLookupTable(dataSet.Tables, connectParameters);

            result.Result = (updateResult?.Exception == null && updateResult?.Result.HasError == false) ? true : false;
            result.ExceptionMessage = updateResult?.Exception == null ? updateResult?.Result?.Exception?.Message : updateResult?.Exception?.Message;
            result.connectParameters = connectParameters;
            result.ConnectionString = GetSqlConnectionString(connectParameters);
            return result;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Reviewed")]
        [HttpPost]
        [Route("droptable")]
        //[FileServiceAuthorizationFilter]
        public object DropTable(object tableConnectionInformation)
        {
            return new DataServiceHelper(GetClientId(), null, null, Request).DropTable(tableConnectionInformation);
        }
        #region Private Methods
        private static List<string> UpdateDataSetInfoInLookupTable(List<DataSourceTable> tables, ConnectParameters parameters)
        {
            var tableInfo = new List<string>();
            var sqlConnection = new SqlConnection(GetSqlConnectionString(parameters));
            SqlCommand command = new SqlCommand();
            if (!DoesTempTableExists(sqlConnection))
            {
                if (sqlConnection.State != System.Data.ConnectionState.Open)
                {
                    sqlConnection.Open();
                }
                try
                {
                    command.CommandText = "CREATE TABLE LOOKUPTABLEINFO(FILENAME varchar(max), TABLENAME varchar(max));";
                    command.Connection = sqlConnection;
                    command.ExecuteScalar();
                }
                finally
                {
                    sqlConnection.Close();
                }
            }

            if (sqlConnection.State != System.Data.ConnectionState.Open)
            {
                sqlConnection.Open();
            }
            try
            {
                command.Connection = sqlConnection;
                tables.ForEach(table =>
                {
                    command.CommandText = "INSERT INTO LOOKUPTABLEINFO VALUES(@FILENAME,@TABLENAME);";
                    command.Parameters.AddWithValue("@FILENAME", Path.GetFileNameWithoutExtension(parameters.FileName));
                    command.Parameters.AddWithValue("@TABLENAME", table.Name);
                    command.ExecuteScalar();
                    command.Parameters.Clear();
                    tableInfo.Add(table.Name);
                });
            }
            finally
            {
                sqlConnection.Close();
            }
            return tableInfo;
        }

        private static string GetSqlConnectionString(ConnectParameters parameters)
        {
            return new SqlConnectionStringBuilder()
            {
                DataSource = parameters?.DataSource,
                IntegratedSecurity = parameters.IntegratedSecurity,
                InitialCatalog = parameters?.InitialCatalog,
                UserID = !parameters.IntegratedSecurity ? parameters?.UserName : string.Empty,
                Password = !parameters.IntegratedSecurity ? parameters?.Password : string.Empty,
            }.ToString();
        }

        private static bool DoesTempTableExists(SqlConnection sqlConnection)
        {
            bool result = false;
            if (sqlConnection.State != System.Data.ConnectionState.Open)
            {
                sqlConnection.Open();
            }
            try
            {
                SqlCommand command = new SqlCommand() { Connection = sqlConnection, CommandText = "IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'LOOKUPTABLEINFO') SELECT 1 ELSE SELECT 0" };
                int x = Convert.ToInt32(command.ExecuteScalar(), CultureInfo.InvariantCulture);
                result = x == 1;
            }
            finally
            {
                sqlConnection.Close();
            }
            return result;
        }

        private ConnectionBase CreateNewConnection(string dataProvider, ConnectParameters connectParameters)
        {
            ConnectionBase fileConnectionBase = GetNewConnectionBase(dataProvider);
            ConnectionPorter porter = new ConnectionPorter(fileConnectionBase);
            return porter.PortConnection(ConnectionParameters(connectParameters, dataProvider));
        }

        private ConnectionBase GetNewConnectionBase(string dataProvider)
        {
            switch (dataProvider?.ToLower())
            {
                case "file":
                    return new CoreConfigurationHelper(DashboardContainer).GetConnectionPlugin(new Guid("95461dca-86c3-4a40-8e02-45d4230b2514")).BuildNewConnection(DashboardContainer);
                case "sql":
                    return new CoreConfigurationHelper(DashboardContainer).GetConnectionPlugin(new Guid("dc8b01ad-9970-4dab-a066-499ec13a6e21")).BuildNewConnection(DashboardContainer);
                case "web":
                    break;
                default: return null;
            }
            return null;
        }

        private static ConnectParameters GetConnectParameters(Syncfusion.Dashboard.Web.Serialization.Model.DataSources.DataSource dataSource, string clientId)
        {
            return new ConnectParameters()
            {
                FilePath = Path.ChangeExtension(Path.Combine(!string.IsNullOrEmpty(AppSettingsInfo.AppSetting.AppDataPath) ? Path.GetFullPath(AppSettingsInfo.AppSetting.AppDataPath, AppDomain.CurrentDomain.BaseDirectory) : AppDomain.CurrentDomain.BaseDirectory, "Syncfusion Dashboard Designer(web)", clientId, Path.GetFileNameWithoutExtension(dataSource?.FileName)), Path.GetExtension(dataSource?.FileName)),
                FileName = dataSource?.FileName,
                OriginalFileName = dataSource?.OriginalFileName,
                FileType = (Core.Helpers.FileType)Enum.Parse(typeof(Syncfusion.Dashboard.Web.Serialization.Model.DataSources.Common.FileType), dataSource.FileType.ToString(), true),
                SeparatorType = (SeparatorType)Enum.Parse(typeof(Syncfusion.Dashboard.Web.Serialization.Model.DataSources.Common.SeparatorType), dataSource.SeparatorType.ToString(), true),
                ElementSeparator = dataSource.ElementSeparator,
                EolSeparator = dataSource.EolSeparator,
                DataSource = dataSource?.ServerName,
                IntegratedSecurity = dataSource.IsIntegrated,
                InitialCatalog = dataSource?.Database,
                UserName = dataSource?.Username,
                Password = dataSource?.Password,
                IsCSVFirstRowHeader = dataSource != null ? dataSource.IsCSVFirstRowHeader : true,
                ExcelImportType = Enum.TryParse(dataSource.ExcelImportType?.ToString(), true, out ExcelImportType importType) ? importType : ExcelImportType.Worksheets
            };
        }

        private static Dictionary<string, string> ConnectionParameters(ConnectParameters ConnectParameters, string dataProvider)
        {
            Dictionary<string, string> connectionInfo = new Dictionary<string, string>();
            if (ConnectParameters != null)
            {
                switch (dataProvider?.ToLower())
                {
                    case "file":
                        connectionInfo.Add("FileName", ConnectParameters.FileName);
                        connectionInfo.Add("FilePath", ConnectParameters.FilePath);
                        connectionInfo.Add("FileType", ConnectParameters.FileType.ToString());
                        connectionInfo.Add("SeparatorType", ConnectParameters.SeparatorType.ToString());
                        connectionInfo.Add("ElementSeparator", ConnectParameters.ElementSeparator.ToString());
                        connectionInfo.Add("EolSeparator", ConnectParameters.EolSeparator.ToString());
                        connectionInfo.Add("ExcelImportType", ConnectParameters.ExcelImportType?.ToString());
                        break;

                    case "sql":
                        connectionInfo.Add("Service", ConnectParameters.DataSource);
                        connectionInfo.Add("IntegratedSecurity", ConnectParameters.IntegratedSecurity.ToString());
                        connectionInfo.Add("Database", ConnectParameters.InitialCatalog);
                        connectionInfo.Add("Username", ConnectParameters.UserName);
                        connectionInfo.Add("Password", ConnectParameters.Password);
                        var builder = new SqlConnectionStringBuilder()
                        {
                            DataSource = ConnectParameters.DataSource,
                            IntegratedSecurity = ConnectParameters.IntegratedSecurity,
                            InitialCatalog = ConnectParameters.InitialCatalog,
                        };
                        if (!builder.IntegratedSecurity)
                        {
                            builder.UserID = ConnectParameters.UserName;
                            builder.Password = ConnectParameters.Password;
                        }
                        connectionInfo.Add("ConnectionString", builder?.ConnectionString);
                        break;

                    case "web":
                        break;
                }
            }

            return connectionInfo;
        }

        private string GetClientId()
        {
            return HttpContext.Request.Headers["ClientID"].ToString();
        }
        #endregion
    }

    public class WebTargetDataSourceConnection
    {
        public SqlConnectionStringBuilder TargetConnectionString { get; set; }

        public List<string> TableNameList { get; set; }
    }
}