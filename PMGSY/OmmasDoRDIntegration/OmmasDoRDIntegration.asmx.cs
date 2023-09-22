using PMGSY.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;

namespace PMGSY.OmmasDoRDIntegration
{
    /// <summary>
    /// Summary description for OmmasDoRDIntegration
    /// </summary>
    [WebService(Namespace = "https://online.omms.nic.in/")]
    //[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class OmmasDoRDIntegration : System.Web.Services.WebService
    {

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        /// <summary>
        /// Get Local Government Directories Habitations 
        /// </summary>
        /// <returns></returns>
        public void GetLGDHabitations(int lgdStateCode, int lgdDistrictCode, int lgdBlockCode)
        {
            OmmasDoRDIntegrationDAL objDAL = new OmmasDoRDIntegrationDAL();
            List<OmmasDoRDIntegrationViewModel> model = new List<OmmasDoRDIntegrationViewModel>();
            try
            {
                model = objDAL.GetLGDHabitationsDAL(lgdStateCode, lgdDistrictCode, lgdBlockCode);

                //return model;
                //HttpContext.Current.Response.Write("[" + new JavaScriptSerializer().Serialize(model) + "]");
                //HttpContext.Current.Response.Write(new JavaScriptSerializer().Serialize(model));

                var serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;

                HttpContext.Current.Response.Write(serializer.Serialize(model));
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "OmmasDoRDIntegration.GetLGDHabitations()");
                //return null;
                //HttpContext.Current.Response.Write(new JavaScriptSerializer().Serialize("Something went wrong, please try again"));
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        /// <summary>
        /// Get Local Government Directories Habitation Details
        /// </summary>
        /// <returns></returns>
        public void GetLGDHabitationDetails(int lgdStateCode, int lgdDistrictCode, int lgdBlockCode, int habCode, byte scheme)
        {
            OmmasDoRDIntegrationDAL objDAL = new OmmasDoRDIntegrationDAL();
            List<OmmasDoRDHabitationDetailsViewModel> model = new List<OmmasDoRDHabitationDetailsViewModel>();
            try
            {
                model = objDAL.GetLGDHabitationDetailsDAL(lgdStateCode, lgdDistrictCode, lgdBlockCode, habCode, scheme);

                var serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;

                HttpContext.Current.Response.Write(serializer.Serialize(model));
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "OmmasDoRDIntegration.GetLGDHabitationDetails()");
                //return null;
                //HttpContext.Current.Response.Write(new JavaScriptSerializer().Serialize("Something went wrong, please try again"));
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        /// <summary>
        /// Get Local Government Directories SQC Details
        /// </summary>
        /// <returns></returns>
        public void GetSQCDetails(int lgdstateCode)
        {
            OmmasDoRDIntegrationDAL objDAL = new OmmasDoRDIntegrationDAL();
            List<OmmasDoRDSQCDetailsViewModel> model = new List<OmmasDoRDSQCDetailsViewModel>();
            try
            {
                model = objDAL.GetSQCDetailsDAL(lgdstateCode);

                var serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;

                HttpContext.Current.Response.Write(serializer.Serialize(model));
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "OmmasDoRDIntegration.GetSQCDetails()");
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        /// <summary>
        /// Get Local Government Directories Habitations Statewise 
        /// </summary>
        /// <returns></returns>
        public void GetHabitationsStatewise(int lgdstateCode, int lgdDistrictCode, int lgdBlockCode, byte scheme)
        {
            OmmasDoRDIntegrationDAL objDAL = new OmmasDoRDIntegrationDAL();
            List<OmmasDoRDHabStatewiseViewModel> model = new List<OmmasDoRDHabStatewiseViewModel>();
            try
            {
                model = objDAL.GetHabitationsStatewiseDAL(lgdstateCode, lgdDistrictCode, lgdBlockCode, scheme);

                var serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;

                HttpContext.Current.Response.Write(serializer.Serialize(model));
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "OmmasDoRDIntegration.GetHabitationsStatewise()");
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        /// <summary>
        /// Get Local Government Directories Habitations State
        /// </summary>
        /// <returns></returns>
        public void GetHabitationsState(int lgdstateCode, int lgdDistrictCode, byte scheme)
        {
            OmmasDoRDIntegrationDAL objDAL = new OmmasDoRDIntegrationDAL();
            List<OmmasDoRDHabDetailsStateViewModel> model = new List<OmmasDoRDHabDetailsStateViewModel>();
            try
            {
                model = objDAL.GetHabitationsStateDAL(lgdstateCode, lgdDistrictCode, scheme);

                var serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;

                HttpContext.Current.Response.Write(serializer.Serialize(model));
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "OmmasDoRDIntegration.GetHabitationsState()");
            }
        }

    }
}
