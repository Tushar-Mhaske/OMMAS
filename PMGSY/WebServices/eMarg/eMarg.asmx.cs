using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;

using PMGSY.Models;

using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Data.Entity;
using Newtonsoft.Json;
using System.ComponentModel;
using PMGSY.WebServices.eMarg.Model;
using System.Net;

using System.Web.Script.Serialization;
using PMGSY.Common;
using System.Web.Http;

namespace PMGSY.WebServices.eMarg
{
    /// <summary>
    /// Summary description for eMarg
    /// </summary>
    [WebService(Namespace = "https://online.omms.nic.in")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class eMarg : System.Web.Services.WebService
    {

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public List<RoadDetails> EmargRoadDetails()
        {
            try
            {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            //string URL = "https://emarg.gov.in/emargrest/rest/roaddetail/getdetail?User_ID=cdcweb&Password=gis@cdc098";
            //Uri uri = new Uri(URL);
            //WebClient wc = new WebClient();

            //string URL = "https://emarg.gov.in/emargrest/rest/roaddetail/getdetail/";
            string URL = "https://emarg.gov.in/emargrest/rest/datadetail/getdatacorrectionrecord";
            Uri uri = new Uri(URL);
            WebClient wc = new WebClient();

            //string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("cdcweb" + ":" + "gis@cdc098"));
            //wc.Headers[HttpRequestHeader.Authorization] = "Basic" + credentials;
            string token = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes("cdcweb" + ":" + "gis@cdc098"));
            // var token = (ASCIIEncoding.ASCII.GetBytes("cdcweb" + ":" + "gis@cdc098"));
            wc.Headers.Add("Authorization", "Basic " + token);
            
            string results;
            results = wc.DownloadString(uri);

           JavaScriptSerializer ser = new JavaScriptSerializer();

           List<RoadDetails> lst = ser.Deserialize<List<RoadDetails>>(results);//str is JSON string.

            eMargDAL abc = new eMargDAL();
            abc.saveData(lst);


            return lst;

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EmargRoadDetails()");
                throw ex;
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public string GetEmargPaymentDetails(string data)
        {
            try
            {
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ////string URL = "https://emarg.gov.in/emargrest/rest/expenditure/getroadwiseexpenditure?User_ID=cdcweb&Password=gis@cdc098";
                ////Uri uri = new Uri(URL);
                ////WebClient wc = new WebClient();

                ////string URL = "https://emarg.gov.in/emargrest/rest/emargpayment/GetEmargPaymentDetails/";
                //string URL = "https://emarg.gov.in/emargrest/rest/emargpayment/GetEmargPaymentDetails";
                //Uri uri = new Uri(URL);
                //WebClient wc = new WebClient();

                ////string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("cdcweb" + ":" + "gis@cdc098"));
                ////wc.Headers[HttpRequestHeader.Authorization] = "Basic" + credentials;
                //string token = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes("cdcweb" + ":" + "gis@cdc098"));
                //// var token = (ASCIIEncoding.ASCII.GetBytes("cdcweb" + ":" + "gis@cdc098"));
                //wc.Headers.Add("Authorization", "Basic " + token);
                List<EmargPaymentModel> lst = new List<EmargPaymentModel>();
                string results;
               // results = wc.DownloadString(uri);
                results = data;


                JavaScriptSerializer ser = new JavaScriptSerializer();
                try
                {
                     lst = ser.Deserialize<List<EmargPaymentModel>>(results);//str is JSON string.
                }
                catch (Exception ex)
                {
                    throw new Exception("Invalid JSON String");
                }

                eMargDAL dalobj = new eMargDAL();
                int updateddata = dalobj.getemargpaymentdetailsDAL(lst);


                if (updateddata > 0)
                {
                    return "Success";
                }
                return "Failure";

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EmargRoadDetails()");
                throw ex;
            }
        }

       
    }



    //public class RootObject
    //{
    //    public List<EMARG_ROAD_DETAILS> plans { get; set; }

    //}


   
}
