using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Extensions
{
    public class PMGSYSession
    {
        // private constructor
        private PMGSYSession()
        {
            UserName = null;
            StateCode = 0;
        }

        // Gets the current session.
        public static PMGSYSession Current
        {
            get
            {
                try
                {
                    PMGSYSession session = null;
                    if ((PMGSYSession)HttpContext.Current.Session["__MySession__"] == null)
                    {
                        session = new PMGSYSession();
                        HttpContext.Current.Session["__MySession__"] = session;
                    }
                    else
                    {
                        session = (PMGSYSession)HttpContext.Current.Session["__MySession__"];
                    }

                    HttpContext.Current.Session.Timeout = 20;
                    return session;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public void EndSession()
        {
            HttpContext.Current.Session.Abandon();
            HttpContext.Current.Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));
        }

        public bool IsSessionExpired()
        {
            return PMGSYSession.Current.UserId != 0 ? false : true;
        }
        
        // **** add your session properties here, e.g like this:
        //public Int32 SessionSalt { get; set; }
        public Int64 SessionSalt { get; set; }
        public String ModuleName { get; set; }
        public Int32 ParentUserId { get; set; }
        public Int32 ParentUserName { get; set; }
        public Int32 UserId { get; set; }
        public String UserName { get; set; }
        public Int16 LevelId { get; set; }
        public long LogId { get; set; }
        public Int16 StateCode { get; set; }
        public String StateName { get; set; }
        public String StateShortCode { get; set; }
        public Int32 RoleCode { get; set; }
        public String RoleName { get; set; }
        public Int32 DistrictCode { get; set; }
        public String DistrictName { get; set; }
        public Int16 CssID { get; set; }
        public String CssName { get; set; }
        public Int16 LanguageID { get; set; }
        public String Language { get; set; }
        public String DepartmentName { get; set; }
        public Int32? ParentNDCode { get; set; }
        // For Account Module ------------------
        public String FundType { get; set; }
        public Int32 AdminNdCode { get; set; }
        public Int16 AccMonth { get; set; }
        public Int16 AccYear { get; set; }
        // -------------------------------------
        public Byte PMGSYScheme { get; set; }
        public int MastConCode { get; set; }
        public string HomePageURL { get; set; }

        public string AppletErrMessage { get; set; }
        public int MastAgencyCode { get; set; }
        public string MastAgencyName { get; set; }
    }
}