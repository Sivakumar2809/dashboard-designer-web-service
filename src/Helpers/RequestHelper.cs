namespace Syncfusion.Dashboard.Designer.Web.Service.Helpers
{
    using Newtonsoft.Json;
    using Syncfusion.Dashboard.Core.Properties;
    using Syncfusion.Dashboard.Web.Serialization.DOM.Model.DataSources.Common;
    using System;
    using System.Globalization;
    using System.Linq;
    using Microsoft.AspNetCore.Http;

    public class RequestHelper
    {

        public HttpRequest request { get; set; }
        /// <summary>
        /// Returns the Client id sent by the request or generates a new one.
        /// </summary>
        /// <returns></returns>
        internal string GetClientId()
        {
            string clientId = request.Headers["ClientID"].FirstOrDefault();
			if (string.IsNullOrEmpty(clientId))
			{
				clientId = Initialize();
                request.Headers.Add("ClientID", clientId);
			}
			return clientId;
        }

        /// <summary>
        /// Generates a new GUID for the client
        /// </summary>
        /// <returns></returns>
        internal string Initialize()
        {
            return Guid.NewGuid().ToString("N");
        }

        /// <summary>
        /// Generates culture info with request header.
        /// </summary>
        /// <returns>Returns the current culture info.</returns>
        internal LocalizationSettings GetCulture()
        {
            string cultureSetting = request?.Headers["LocaleSettings"].FirstOrDefault();
            if (!string.IsNullOrEmpty(cultureSetting))
            {
                var culturedata = JsonConvert.DeserializeObject<LocalizationInfo>(cultureSetting);
                return GetCultureInfo(culturedata.culture);
            }
            return GetDefaultCulture();
        }

        /// <summary>
        /// Generates culture info.
        /// </summary>
        /// <returns>Returns the current culture info.</returns>
        internal LocalizationSettings GetCultureInfo(string culture)
        {
            if (!string.IsNullOrEmpty(culture))
            {
                CultureInfo us = new CultureInfo(culture);
                string shortDateFormatString = us.DateTimeFormat.ShortDatePattern;
                string shortTimeFormatString = us.DateTimeFormat.ShortTimePattern;
                return new LocalizationSettings { DateSettings = new DateFormatSettings() { DateFormat = shortDateFormatString, TimeFormat = shortTimeFormatString, Is24Hours = true }, DisplayCulture = CultureInfo.GetCultureInfo(culture), culture = culture};
            }
            return GetDefaultCulture();
        }



        /// <summary>
        /// Generates default culture info.
        /// </summary>
        /// <returns>Returns the default current culture info.</returns>
        internal LocalizationSettings GetDefaultCulture()
        {
            return new LocalizationSettings { DateSettings = new DateFormatSettings() { DateFormat = "M/d/yyyy", TimeFormat = "hh:mm tt", Is24Hours = false }, DisplayCulture = CultureInfo.GetCultureInfo("en-US"), culture = "en-US" };
        }

    }
}