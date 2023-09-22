#region HEADER
/*
 * Project Id:

 * Project Name:OMMAS-II

 * File Name: DishaService.asmx..cs

 * Author : Pradip Patil

 * Creation Date :13-12-2017

 * Desc : This service exposed the data for Disha.  
*/

#endregion


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Newtonsoft.Json;
using PMGSY.Disha;
using PMGSY.Models;
using System.Web.Script.Services;
using System.Web.Script.Serialization;

namespace PMGSY.Disha
{
    /// <summary>
    /// Summary description for DishaService
    /// </summary>
    [WebService(Namespace = "https://online.omms.nic.in")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class DishaService : System.Web.Services.WebService
    {
        DishaServiceDAL DishaDAL = null;
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        [WebMethod]
        public void GetNSPPhaseProfileData(string level,string state,string district,string agency,string pmgsy="1")
        {
             DishaDAL = new DishaServiceDAL();
             Context.Response.ContentType = "application/json";
            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;

            HttpContext.Current.Response.Write(serializer.Serialize(DishaDAL.GetNSPPhaseProfileData(level, state, district, agency, pmgsy)));
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void PMGSY_ReportStatistic(string level, string lgdstate)
        {
            int levelCode = Convert.ToInt32(level);
            DishaDAL = new DishaServiceDAL();
            Context.Response.ContentType = "application/json";
            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;
            if (levelCode == 0)
            {
                HttpContext.Current.Response.Write(serializer.Serialize(new { message = "Invalid level code" }));
            }
            else
            {
                List<USP_PROJECT_STATISTICS_REPORT_DISHA_Result> result = DishaDAL.PMGSY_ReportStatisticDAL(level, lgdstate);

                if (levelCode == 1)
                {
                    var finalResult = result.Select(s => new
                    {
                        UROAD = s.UROAD,
                        ULEN = s.ULEN,
                        UC_COUNT = s.UC_COUNT,
                        NROAD = s.NROAD,
                        NLEN = s.NLEN,
                        NC_COUNT = s.NC_COUNT
                    }).ToList();
                    HttpContext.Current.Response.Write(serializer.Serialize(finalResult));
                }
                else
                {
                    HttpContext.Current.Response.Write(serializer.Serialize(result));
                }
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void PMGSY_AbstractGradingStatewise(int level, int lgdstate, int fromyear, int frommonth, int toyear, int tomonth, string qmtype)
        {
            DishaDAL = new DishaServiceDAL();
            var serializer = new JavaScriptSerializer();
            Context.Response.ContentType = "application/json";
            serializer.MaxJsonLength = Int32.MaxValue;
            HttpContext.Current.Response.Write(serializer.Serialize(DishaDAL.PMGSY_AbstractGradingStatewiseDAL(level, lgdstate,fromyear,frommonth,toyear,tomonth,qmtype)));
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void SanctionPendingWorksDisha(int level,int month,int year, int lgdstate)
        {
            DishaDAL = new DishaServiceDAL();
            var serializer = new JavaScriptSerializer();
            Context.Response.ContentType = "application/json";
            serializer.MaxJsonLength = Int32.MaxValue;
            HttpContext.Current.Response.Write(serializer.Serialize(DishaDAL.SanctionPendingWorksDishaDAL(level, month, year, lgdstate)));
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void TargetAchievementDisha(int year,int ldgstate)
        {
            DishaDAL = new DishaServiceDAL();
            var serializer = new JavaScriptSerializer();
            Context.Response.ContentType = "application/json";
            serializer.MaxJsonLength = Int32.MaxValue;
            if (year <= 2013)
            {
                HttpContext.Current.Response.Write(serializer.Serialize(new { message = "Invalid year" }));
            }
            else
            {
                HttpContext.Current.Response.Write(serializer.Serialize(DishaDAL.TargetAchievementDishaDAL(year,ldgstate))); 
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json,UseHttpGet=true)]
        public void SanctionedHabitaions(int ldgState=0)
        { 
          DishaDAL = new DishaServiceDAL();

            var serializer = new JavaScriptSerializer();
            Context.Response.ContentType="application/json";
            serializer.MaxJsonLength = Int32.MaxValue;
            HttpContext.Current.Response.Write(serializer.Serialize(DishaDAL.SanctionedHabitaionsDAL(ldgState)));
        }

    }
}
