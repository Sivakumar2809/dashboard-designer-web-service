using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Syncfusion.Dashboard.Designer.Web.Service.Helpers
{
    public class ServiceHelper
    {
        public ServiceHelper(string tt = null)
        {
            Test = tt;
        }

        [Inject]
        public static string Test
        {
            get;
            set;
        }
    }
}
