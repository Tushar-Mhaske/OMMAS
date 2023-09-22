using PMGSY.DAL;
using PMGSY.Extensions;
using PMGSY.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace PMGSY
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            var application = sender as HttpApplication;
            if (application != null && application.Context != null)
            {
                Response.Headers.Set("Server", "");
                application.Context.Response.Headers.Remove("Server");
                HttpContext.Current.Response.Headers.Remove("X-AspNet-Version");
                HttpContext.Current.Response.Headers.Remove("X-AspNetMvc-Version");
                HttpContext.Current.Response.AddHeader("x-frame-options", "SAMEORIGIN");
                MvcHandler.DisableMvcResponseHeader = true;
            }

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            
            ///To disable TLs 1 and below
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        protected void Application_Start()
        {
            //addition by Koustubh Nakate to remove X-AspNetMvc-Version from response header on 30/08/2013
            MvcHandler.DisableMvcResponseHeader = true;

            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            // Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mgo+DSMBPh8sVXJ0S0d+XE9Bd1RDX3xKf0x/TGpQb19xflBPallYVBYiSV9jS3xTc0VhWXtcdnRcQGBcWA==");
            // Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mgo+DSMBPh8sVXJ0S0J+XE9BdlRDX3xKf0x/TGpQb19xflBPallYVBYiSV9jS31TdERkWX9acHZURGRfUg==");
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mgo+DSMBPh8sVXJ0S0d+XE9Bd1RDX3xKf0x/TGpQb19xflBPallYVBYiSV9jS31TdERkWX9bdXZQRGheVA==");

            // Mgo+DSMBPh8sVXJ0S0J+XE9BdlRDX3xKf0x/TGpQb19xflBPallYVBYiSV9jS31TdERkWX9acHZURGRfUg==
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            BundleTable.EnableOptimizations = true;
            AuthConfig.RegisterAuth();
            GlobalConfiguration.Configuration.Filters.Add(new System.Web.Http.AuthorizeAttribute());

            int onlineUserCnt = 0;
            Application["OnlineUsers"] = onlineUserCnt;
        }

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new System.Web.Mvc.AuthorizeAttribute()); //new
        }

        protected void Application_Error() 
        {
            Exception unhandledException = Server.GetLastError();
            HttpException httpException = unhandledException as HttpException;
            Exception innerException = unhandledException.InnerException;

            using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
            {
                sw.WriteLine("Date :" + DateTime.Now.ToString());
                sw.WriteLine("Method : " + "Application_Error()");
                if (unhandledException != null)
                    sw.WriteLine("Exception : " + unhandledException.ToString());
                //if (innerException != null)
                //    sw.WriteLine("innerException : " + innerException.Message.ToString());
                //if (httpException != null)
                //    sw.WriteLine("httpException : " + httpException.Message.ToString());
                sw.WriteLine("---------------------------------------------------------------------------------------");
                sw.Close();
            }
            String strURL, strIP;
            String strErrorCode = "0";
            String strBrowser;
            strURL = Request.Url.ToString();
            strBrowser = Request.Browser.Type + Request.Browser.Version;
            strIP = Request.ServerVariables["HTTP_X_FORWORDED_FOR"];
            if (String.IsNullOrEmpty(strIP))
            {
                strIP = Request.ServerVariables["REMOTE_ADDR"];
            }
            if (strIP.Equals("::1"))
            {
                strIP = "127.0.0.1";
            }
            try
            {
                if (httpException != null || innerException != null)
                {
                    //if the exception is an http exception get the http code
                    if (httpException != null)
                    {
                        strErrorCode = httpException.GetHttpCode().ToString();
                    }
                }
               
                Server.ClearError();

                if (!Response.IsRequestBeingRedirected)
                    Response.Redirect("/Login/Error/" + strErrorCode);
            }
            catch (Exception ex)
            {
                //clear the respone, log the details and redirect the user to the ErrorHandler page
                Server.ClearError();
                Response.Clear();
                if (!Response.IsRequestBeingRedirected)
                    Response.Redirect("/Login/Error/" + strErrorCode);
            }           
        }


        protected void Session_Start(Object sender, EventArgs E)
        {
            try
            {   
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.MinValue;
                Response.Cookies[".ASPXAUTH"].Expires = DateTime.MinValue;
            }
            catch 
            {
                Server.ClearError();
                Response.Clear();
                if (!Response.IsRequestBeingRedirected)                    
                    Response.Redirect("/Login/Error");
            }
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        protected void Session_End(Object sender, EventArgs E)
        {
            Application.Lock();
            Application["OnlineUsers"] = Convert.ToInt32(Application["OnlineUsers"]) - 1;
            Application.UnLock();
            Session.Abandon();
        }

        void Application_PreSendRequestHeaders(Object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
        }
    }
}