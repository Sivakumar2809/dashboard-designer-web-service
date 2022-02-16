using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Syncfusion.Dashboard.Service.Base;
using Syncfusion.Dashboard.Service.Base.CoreHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Syncfusion.Dashboard.Designer.Web.Service.Helpers
{
	public class DashboardServiceMiddleWare
	{

        private readonly RequestDelegate next;

        public DashboardServiceMiddleWare(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            this.BeginInvoke(context);
            await this.next.Invoke(context);
            this.EndInvoke(context);
        }

        private void BeginInvoke(HttpContext context)
        {
            //testInjector = new TestInjector();
           // DashboardHeaders = new HeadersInjector() { DashboardHeaders = context};
        }

        private void EndInvoke(HttpContext context)
        {
            
        }
    }
}
