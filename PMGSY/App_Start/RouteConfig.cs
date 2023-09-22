using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PMGSY
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{resource}.aspx/{*pathInfo}");
            routes.IgnoreRoute("{*allaspx}", new { allaspx = @".*\.aspx(/.*)?" });
            routes.IgnoreRoute("elmah.axd");
            routes.IgnoreRoute("{*allasmx}", new { allasmx = @".*\.asmx(/.*)?" });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Login", action = "Login", id = UrlParameter.Optional }
            );

            routes.MapRoute(
               name: "EncryptParameter",
               url: "{controller}/{action}/{parameter}/{hash}/{key}"
            );

            routes.MapRoute(
                 null,//"EncryptParameter",
                 "{controller}/{action}/{parameter}/{hash}/{key}/{id}"
            );

            routes.MapRoute(
                name: "Edit",                                              // Route name
                url: "{controller}/{action}/{id1}/{hash}/{key}"
            );

            routes.MapRoute(
            "newRoute",                                              // Route name
            "{controller}/{action}/{id1}/{id2}"
            );


            routes.MapRoute(
                "Admin_elmah",
                "Admin/elmah/{type}",
                new { action = "Index", controller = "Elmah", type = UrlParameter.Optional }
            );
        }
    }
}