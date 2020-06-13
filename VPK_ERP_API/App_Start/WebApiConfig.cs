using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using VPK_ERP_API.Utilities;

namespace VPK_ERP_API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {

            //Toolkits t = new Toolkits();
            //t.Select_All_Menu2_Huu_2(4);


            // New code
            config.EnableCors();
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
