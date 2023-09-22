using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Reflection;
using PMGSY.Extensions;
using PMGSY.Common;
using PMGSY.Models;
using System.Transactions;
using System.Configuration;


namespace PMGSY.Controllers
{
    public class RequiredAuthentication : AuthorizeAttribute
    {

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            HttpContext ctx = HttpContext.Current;
            try
            {
                if (PMGSYSession.Current.IsSessionExpired())
                {
                    ctx.Response.Redirect("/Login/SessionExpire");
                    return false;
                }

            //    regenerateId();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        void regenerateId()
        {

            System.Web.SessionState.SessionIDManager manager = new System.Web.SessionState.SessionIDManager();

            string oldId = manager.GetSessionID(System.Web.HttpContext.Current);

            string newId = manager.CreateSessionID(System.Web.HttpContext.Current);

            bool isAdd = false, isRedir = false;

            manager.SaveSessionID(System.Web.HttpContext.Current, newId, out isRedir, out isAdd);

            HttpApplication ctx = (HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;

            HttpModuleCollection mods = ctx.Modules;

            System.Web.SessionState.SessionStateModule ssm = (System.Web.SessionState.SessionStateModule)mods.Get("Session");

            System.Reflection.FieldInfo[] fields = ssm.GetType().GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            System.Web.SessionState.SessionStateStoreProviderBase store = null;

            System.Reflection.FieldInfo rqIdField = null, rqLockIdField = null, rqStateNotFoundField = null;

            foreach (System.Reflection.FieldInfo field in fields)
            {

                if (field.Name.Equals("_store")) store = (System.Web.SessionState.SessionStateStoreProviderBase)field.GetValue(ssm);

                if (field.Name.Equals("_rqId")) rqIdField = field;

                if (field.Name.Equals("_rqLockId")) rqLockIdField = field;

                if (field.Name.Equals("_rqSessionStateNotFound")) rqStateNotFoundField = field;

            }

            object lockId = rqLockIdField.GetValue(ssm);

            if ((lockId != null) && (oldId != null)) store.ReleaseItemExclusive(System.Web.HttpContext.Current, oldId, lockId);

            rqStateNotFoundField.SetValue(ssm, true);

            rqIdField.SetValue(ssm, newId);

            HttpContext.Current.Session["ASP.NET_SessionId"] = newId;



        }


        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            HttpContext httpContext = HttpContext.Current;
            try
            {

                httpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
                httpContext.Response.Cache.SetValidUntilExpires(false);
                httpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
                httpContext.Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache);
                httpContext.Response.Cache.SetNoStore();

                if (!HttpContext.Current.Session["ASP.NET_SessionId"].ToString().Equals(HttpContext.Current.Request.Cookies["ASP.NET_SessionId"].Value))
                {
                    //throw new Exception("invalid session");
                    httpContext.Response.Redirect("/Login/SessionExpire");
                }


                if (PMGSYSession.Current.IsSessionExpired())
                {
                    httpContext.Response.Redirect("/Login/SessionExpire");
                }
            }
            catch (Exception)
            {
                httpContext.Response.Redirect("/Login/SessionExpire");
            }
        }
    }


    //  Check for Authorized actions only.
    //  authorize attribute is being used instead 
    //  Used to identify each user has his authorized role to access authorized actions
    public class RequiredAuthorization : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext ctx = HttpContext.Current;
            Boolean blnResult = false;
            try
            {
                PMGSY.DAL.Login loginDAL = new PMGSY.DAL.Login();
                PMGSY.Models.AuthorizationModel objAuthorizationModel = new PMGSY.Models.AuthorizationModel();

                string[] actionControllerList = ctx.Request.Url.AbsolutePath.ToString().Trim().Split('/');
                objAuthorizationModel.ActionURL = ctx.Request.Url.AbsolutePath;

                objAuthorizationModel.ControllerName = actionControllerList[1].ToString().Trim();
                objAuthorizationModel.ActionName = actionControllerList[2].ToString().Trim();

                objAuthorizationModel.RoleCode = Convert.ToInt16(PMGSYSession.Current.RoleCode);

                blnResult = loginDAL.Authorize(objAuthorizationModel);

                if (!blnResult)     //if false
                {
                    ctx.Response.Redirect("/Login/UnAuthorized");
                }

                ///Check to enable menus for PMGSY3 for PIU
                #region
                //if (PMGSYSession.Current.PMGSYScheme == 4 && PMGSYSession.Current.RoleCode == 22)
                //{
                //    string[] lstController = ConfigurationManager.AppSettings["PMGSY3EnabledMenu"].Split(',');
                //    List<string> checkaction = new List<string>();
                //    foreach (var itm in lstController)
                //    {
                //        checkaction.Add(itm.Trim());
                //    }
                //    if (objAuthorizationModel.ControllerName.Trim() == "LocationMasterDataEntry" && !(objAuthorizationModel.ActionName.Trim().Contains("facility") || objAuthorizationModel.ActionName.Trim() != "GetHabitationBlockCode"
                //        || objAuthorizationModel.ActionName.Trim() != "GetHabitationBlockCodeSearch"))
                //    {
                //        ctx.Response.Redirect("/ExistingRoads/PMGSY3EnabledMessageLayout");
                //    }
                //    else
                //        if (!checkaction.Contains(objAuthorizationModel.ControllerName.Trim()))
                //        {
                //            ctx.Response.Redirect("/ExistingRoads/PMGSY3EnabledMessageLayout");
                //        }
                //}
                #endregion
            }
            catch (Exception)
            {
                if (PMGSYSession.Current.IsSessionExpired())
                {
                    ctx.Response.Redirect("/Login/SessionExpire");
                }
                else
                {
                    ctx.Response.Redirect("/Login/UnAuthorized");
                }
            }
        }
    }


    //Class to keep Audit Records
    public class AuditAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //using (var scope = new TransactionScope())
            //{
            //    try
            //    {
            //        if (PMGSYSession.Current.UserId != 0)
            //        {
            //            //Stores the Request in an Accessible object
            //            //var request = filterContext.HttpContext.Request;
            //            ////Stores the Audit in the Database
            //            //using (var dbContext = new PMGSYEntities())
            //            //{
            //            //    ADMIN_AUDIT_LOG adminAuditDetails = new ADMIN_AUDIT_LOG();
            //            //    Int64 maxAuditId = 0;
            //            //    if (!dbContext.ADMIN_AUDIT_LOG.Any())
            //            //    {
            //            //        maxAuditId = 0;
            //            //    }
            //            //    else
            //            //    {
            //            //        maxAuditId = (from c in dbContext.ADMIN_AUDIT_LOG select c.AuditId).Max();
            //            //    }
            //            //    adminAuditDetails.AuditId = maxAuditId + 1;
            //            //    adminAuditDetails.UserId = PMGSYSession.Current.UserId;
            //            //    adminAuditDetails.IPAddress = request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? request.UserHostAddress;
            //            //    adminAuditDetails.ModuleName = PMGSYSession.Current.ModuleName == null ? "-" : PMGSYSession.Current.ModuleName;
            //            //    adminAuditDetails.RequestType = request.HttpMethod == null ? "-" : request.HttpMethod;
            //            //    adminAuditDetails.URLAccessed = request.RawUrl;
            //            //    adminAuditDetails.TimeStamp = DateTime.Now;
            //            //    dbContext.ADMIN_AUDIT_LOG.Add(adminAuditDetails);
            //            //    dbContext.SaveChanges();
            //            //    scope.Complete();
            //            //    //Finishes executing the Action as normal 
            //            //    base.OnActionExecuting(filterContext);
            //            //}
            //        }
            //    }
            //    catch (Exception)
            //    {
            //        //Finishes executing the Action as normal 
            //        base.OnActionExecuting(filterContext);
            //    }
            //}
        }
    }




}
