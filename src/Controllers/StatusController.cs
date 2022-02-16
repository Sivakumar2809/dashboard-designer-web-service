using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Syncfusion.Dashboard.Designer.Web.Service.Controllers
{
    
    [ApiController]
    public class StatusController : ControllerBase
    {
        [HttpGet]
        [Route("/status")]
        public string ServerStatus()
        {
            return "Service is up and running.";
        }

    }
}
