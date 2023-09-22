using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Web.Mvc;
using System.Resources;
using System.Globalization;
//using FAMS.DTO.Common;
using System.Security.Cryptography; 
using System.Collections.Specialized;
using PMGSY.Common;
using PMGSY.Extensions;

using PMGSY.Controllers;
using System.Web.Routing;

namespace MVCHtmlHelper.Helpers
{
    
    public static class MVCMenuHelper 
    {
        public static List<Menu> m_accessCollection = null;
        public static StringBuilder m_sbBuilder = new StringBuilder();
        public static StringBuilder p_Builder = new StringBuilder();
        public static StringBuilder v_Builder = new StringBuilder();
        public static UrlHelper url = null;
        public static CultureInfo Culture;

        #region Code for Drop Line Menus old menubar
      /*  public static string Menu(this HtmlHelper helper, string SessionCulture)
        {

            Culture = new System.Globalization.CultureInfo(SessionCulture);

            url = new UrlHelper(helper.ViewContext.RequestContext);
          
       * // SessionDTO obj = (SessionDTO)HttpContext.Current.Session["User"];
           
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();

            MenuControl mnc = new MenuControl();

            List<Menu> m_accessCollection = mnc.MenuReturn(objUser);

            m_sbBuilder = new StringBuilder();

            var results = m_accessCollection.Where(roleAccess => roleAccess.ParentModuleID == 0);
            m_sbBuilder.Append("<div id=\"Div-Menu\" style=\"display:none\">");
           
            //m_sbBuilder.Append("<ul>");
            
            int cntMenu = 0;
            if (results.Count() > 0)
            {

                foreach (Menu accessByRole in results)
                {
                    //m_sbBuilder.Append("<ul>");
                    cntMenu++;

                    string strUrl = string.Empty;
                    strUrl = url.Action(accessByRole.ActionName + "/" + (accessByRole.PayableID.ToString() == "0" ? "" : accessByRole.PayableID.ToString()), accessByRole.ControllerName);
                    if (accessByRole.ActionName == "" && accessByRole.ControllerName == "")
                    {
                        strUrl = "#";
                    }

                    m_sbBuilder.Append("<a href=\"" + strUrl + "\" rel=\"rel" + accessByRole.ModuleID + "\">" + HttpContext.GetLocalResourceObject(@"~\Views\Shared\Menu", accessByRole.ModuleID.ToString(), Culture) + "</a>");
                    var cntResult = m_accessCollection.Where(roleAccess => roleAccess.StrParentModuleID == accessByRole.ModuleID.ToString());
                    if (cntResult.Count() > 0)
                    {
                        BuildMenuRecursively(accessByRole.ModuleID.ToString());
                    }
                }
                m_sbBuilder.Append("</div>");
            }
            return m_sbBuilder.ToString();
        }

        public static void BuildMenuRecursively(string moduleID)
        {
            var results = m_accessCollection.Where(roleAccess => roleAccess.StrParentModuleID == moduleID);
           // SessionDTO obj = (SessionDTO)HttpContext.Current.Session["User"];

            if (results.Count() > 0)
            {
                m_sbBuilder.Append("<ul>");
                foreach (Menu accessByRole in results)
                {
                    string strUrl = string.Empty;
                    strUrl = url.Action(accessByRole.ActionName + "/" + (accessByRole.PayableID == 0 ? "" : accessByRole.PayableID.ToString()), accessByRole.ControllerName);

                    if ((accessByRole.ActionName == null && accessByRole.ControllerName == null) || (accessByRole.ActionName.ToString() == "" && accessByRole.ControllerName.ToString() == ""))
                    {
                        strUrl = "#";
                    }
                    if (accessByRole.ModuleID != 184)
                    {
                        m_sbBuilder.Append("<li>");
                        m_sbBuilder.Append("<a href=\"" + strUrl + "\" onClick=\"return LoadPage('" + strUrl + "')\">" + HttpContext.GetLocalResourceObject(@"~\Views\Shared\Menu", accessByRole.ModuleID.ToString(), Culture) + "</a>");

                        BuildMenuRecursively(accessByRole.ModuleID.ToString());

                        m_sbBuilder.Append("</li>");
                    }
                    else
                    {
                        m_sbBuilder.Append("<li>");
                        m_sbBuilder.Append("<a href=\"#\" onclick=\"LoadMis()\">" + HttpContext.GetLocalResourceObject(@"~\Views\Shared\Menu", accessByRole.ModuleID.ToString(), Culture) + "</a>");

                        m_sbBuilder.Append("</li>");
                    }
                }
                m_sbBuilder.Append("</ul>");
            }
            
        }
       * 
       */
        #endregion     



        #region Code for Drop Line Menus as per new mwnubar
        public static string Menu(this HtmlHelper helper) 
        {

            try
            {
                if (PMGSYSession.Current.Language != null)
                {
                    Culture = new System.Globalization.CultureInfo(PMGSYSession.Current.Language);
                }
                else {
                    HttpContext.Current.Response.Redirect("/Login/SessionExpire", true);
                }
                url = new UrlHelper(helper.ViewContext.RequestContext);

                DESCryptoServiceProvider des = new DESCryptoServiceProvider();

                MenuControl mnc = new MenuControl();

                m_accessCollection = mnc.MenuReturn(PMGSYSession.Current.RoleCode, PMGSYSession.Current.LevelId);

                m_sbBuilder = new StringBuilder();

                var results = m_accessCollection.Where(roleAccess => roleAccess.ParentModuleID == 0);
                m_sbBuilder.Append("<ul id=\"bar2\"  class=\"menubar-icons\" style=\"display:none\" >");
                //m_sbBuilder.Append("<ul>");
                int cntMenu = 0;
                if (results.Count() > 0)
                {

                    foreach (Menu accessByRole in results)
                    {
                        //m_sbBuilder.Append("<ul>");
                        cntMenu++;

                        string strUrl = string.Empty;
                        strUrl = url.Action(accessByRole.ActionName + "/" + (accessByRole.PayableID.ToString() == "0" ? "" : accessByRole.PayableID.ToString()), accessByRole.ControllerName);
                        if (accessByRole.ActionName == null || accessByRole.ActionName == null || accessByRole.ActionName == "" || accessByRole.ControllerName == "")
                        {
                            strUrl = "#";
                        }

                        //m_sbBuilder.Append("<li><a href=\"" + strUrl + "\" rel=\"rel" + accessByRole.ModuleID + "\">" + HttpContext.GetLocalResourceObject(@"~\Views\Shared\Menu", accessByRole.ModuleID.ToString(), Culture) + "</a>");

                        m_sbBuilder.Append("<li style=" + "margin-top:0.5px;" + "><a href=\"" + strUrl + "\" rel=\"rel" + accessByRole.ModuleID + "\">" + accessByRole.Name + "</a>");
                       
                        var cntResult = m_accessCollection.Where(roleAccess => roleAccess.StrParentModuleID == accessByRole.ModuleID.ToString());
                        if (cntResult.Count() > 0)
                        {
                            BuildMenuRecursively(accessByRole.ModuleID.ToString(), accessByRole.ModuleName);
                        }
                        m_sbBuilder.Append("</li>");
                    }
                    m_sbBuilder.Append("</ul>");
                }
              //  m_sbBuilder.Append("<br/><center><label id='headerFundType'>Progremme Fund</label> </center>");
                return m_sbBuilder.ToString();
            }
            catch(Exception ex)
            {
                throw ex;
               
            }
        }


        

        public static void BuildMenuRecursively(string moduleID, string moduleName)
        {
            var results = m_accessCollection.Where(roleAccess => roleAccess.StrParentModuleID == moduleID);
           

            if (results.Count() > 0)
            {
                m_sbBuilder.Append("<ul>");
                foreach (Menu accessByRole in results)
                {
                    string strUrl = string.Empty;

                    if (accessByRole.ControllerName != null && accessByRole.ControllerName.Contains('/'))
                    {
                        String contName = accessByRole.ControllerName.Split('/')[1];
                        String areaName = accessByRole.ControllerName.Split('/')[0];
                        strUrl = url.Action(accessByRole.ActionName + "/" + (accessByRole.PayableID == 0 ? "" : accessByRole.PayableID.ToString()), contName, new RouteValueDictionary(new { area = areaName }));
                    }
                    else
                    {
                        //strUrl = url.Action(accessByRole.ActionName + "/" + (accessByRole.PayableID == 0 ? "" : accessByRole.PayableID.ToString()), accessByRole.ControllerName, accessByRole.ModuleName);
                        strUrl = url.Action(accessByRole.ActionName + "/" + (accessByRole.PayableID == 0 ? "" : accessByRole.PayableID.ToString()), accessByRole.ControllerName, new RouteValueDictionary(new { area = "" }));
                    }
                    if ((accessByRole.ActionName == null && accessByRole.ControllerName == null) || (accessByRole.ActionName.ToString() == "" && accessByRole.ControllerName.ToString() == ""))
                    {
                        strUrl = "#";
                    }
                    //if (accessByRole.ModuleID != 184)
                    //{
                        m_sbBuilder.Append("<li>");
                       // m_sbBuilder.Append("<a href=\"" + strUrl + "\" onClick=\"return LoadPage('" + strUrl + "$" + moduleName + "')\">" + HttpContext.GetLocalResourceObject(@"~\Views\Shared\Menu", accessByRole.ModuleID.ToString(), Culture) + "</a>");
                        m_sbBuilder.Append("<a href=\"" + strUrl + "\" onClick=\"return LoadPage('" + strUrl + "$" + moduleName + "')\">" + accessByRole.Name + "</a>");
                        BuildMenuRecursively(accessByRole.ModuleID.ToString(), moduleName);

                        m_sbBuilder.Append("</li>");
                    //}
                    //else
                    //{
                    //    m_sbBuilder.Append("<li>");
                    //    m_sbBuilder.Append("<a href=\"#\" onclick=\"LoadMis()\">" + HttpContext.GetLocalResourceObject(@"~\Views\Shared\Menu", accessByRole.ModuleID.ToString(), Culture) + "</a>");

                    //    m_sbBuilder.Append("</li>");
                    //}
                }
                m_sbBuilder.Append("</ul>");
            }
         
        }
        #endregion
    }

    

    #region Code for Horizontal Menu Class 
        public class Menu
        {
            public int ModuleID { get; set; }

            public int? ParentModuleID { get; set; }

            public string ActionName { get; set; }

            public string ControllerName { get; set; }

            public string Name { get; set; }

            public string Title { get; set; }

            public string StrParentModuleID { get; set; }
            
            public int? PayableID { get; set; }

            public string ModuleName { get; set; }
        }
    #endregion


}
