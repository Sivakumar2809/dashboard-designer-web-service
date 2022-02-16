using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
//using Syncfusion.Dashboard.Connection.ClientMode;
using Syncfusion.Dashboard.Core;
using Syncfusion.Dashboard.Core.Configuration;
using Syncfusion.Dashboard.Core.Plugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Syncfusion.Dashboard.Designer.Web.Service.Helpers
{
    public class PluginLoaderHelper
    {

        [Obsolete]
        public PluginLoaderHelper()
        {
            var connectionPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins", CoreConfiguration.ConnectionsFolder);
            var widgetPath = Path.Combine(ConfigFileResolver.ConfigInfo.PlugInFolderPath, CoreConfiguration.WidgetsFolder);
            CoreConfiguration.Instance = new RuntimeConfiguration<IConnectionPlugin, IWidgetPlugin>(connectionPath, widgetPath);
            CoreConfiguration.Instance.ConnectionFactory.GetPlugins().Cast<IConnectionPlugin>();
            //CoreConfiguration.Instance.ConnectionFactory.AddClientMode(new ClientModeConnectionPlugin());
        }  
    }
}
