using PMGSY.Common;
using PMGSY.Models.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.Services.Protocols;

namespace PMGSY.OverallStatus
{
    /// <summary>
    /// Summary description for OverallStatus
    /// </summary>
    [WebService(Namespace = "https://omms.nic.in/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class OverallStatus : System.Web.Services.WebService
    {
        public UserDetails User;
        [SoapHeader("User", Required = true)]
        [WebMethod]
        public string GetOverallStatusData()
        {
            OverallStatusDAL objDAL = new OverallStatusDAL();
            OverallStatusViewModel model = new OverallStatusViewModel();
            try
            {
                if (User != null)
                {
                    if (User.IsValid())
                    {
                        model = objDAL.GetOverallStatusDataDAL();
                        var serializer = new JavaScriptSerializer();
                        serializer.MaxJsonLength = Int32.MaxValue;

                        //HttpContext.Current.Response.Write(serializer.Serialize(model));
                        return (serializer.Serialize(model));
                    }
                    else
                        //HttpContext.Current.Response.Write("Error in authentication"); 
                        return "Error in authentication";
                }
                else
                {
                    return "Error in authentication";
                }   
                //return model;
                //HttpContext.Current.Response.Write("[" + new JavaScriptSerializer().Serialize(model) + "]");
                //HttpContext.Current.Response.Write(new JavaScriptSerializer().Serialize(model));
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "OmmasDoRDIntegration.GetLGDHabitations()");
                //return null;
                //HttpContext.Current.Response.Write(new JavaScriptSerializer().Serialize("Something went wrong, please try again"));
                return "Error occured";
            }
        }
    }
}
