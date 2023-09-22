using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Script.Serialization;
using PMGSY.Common;
using PMGSY.Extensions;
using PMGSY.Areas.PRAYASNEW.Models;



using Newtonsoft.Json;
using System.Data;

namespace PMGSY.Areas.PRAYASNEW.Controllers
{
    public class PrayasNewController : Controller
    {

        //Pleaseo import the following Classes of CS Code.
        //================================================
        CSComClassNew cc = new CSComClassNew();
        CSUtilityNew fcs = new CSUtilityNew();
        // ================================================

        // Please set the required values of the desired variables.
        // ================================================

        DataSet ds;
        Hashtable param;

        // Set the values of State Code, Sector Code, Department Code, and Project Code to the respective Variables.

        private string Instance_Code = "1"; // As provided by DARPAN Dashboard.
        private string Sec_Code = "25"; // As provided by DARPAN Dashboard.
        private string Ministry_Code = "40"; // As provided by DARPAN Dashboard.
        private string Dept_Code = "47"; // As provided by DARPAN Dashboard.
        private string Project_Code = "100136";//"70048"; // As provided by DARPAN Dashboard.


        // Set the values of APIs URL to the respective Variables.
        // Step 1 API URL

        //  private string GetDateRangeAPIUrl = "http://localhost:34287/getdate";
        private string GetDateRangeAPIUrl = "http://prayasapi.darpan.nic.in/getdate";


        // Step 2 API URL

        // private string PushDataAPIUrl = "http://localhost:34287/pushdata";
        private string PushDataAPIUrl = "http://prayasapi.darpan.nic.in/pushdata";


        // Test_DATA is the name of stored procedure and will be created at user end database to fetch the dataset of desired duration.
        // The input parameter will be Instance_Code,Sec_Code,Ministry_Code,Dept_Code,Project_Code and datefrom.
        private string SQLDBLogStoredProc = "omms.Prayas_Maintain_Log";
        private string SQLDBDataStoredProc = "omms.Prayas_Retrieve_Data"; // Test_DATA




        //Variable to assign concatinated Group Id(s) string
        private string remGroupId = "";


        [HttpGet]
        public ActionResult PrayasNewLayout()
        {
            //QualityReportsViewModel QM = new QualityReportsViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {

                //QM.State = PMGSYSession.Current.StateCode;
                //QM.StateList = comm.PopulateStates(true);
                //QM.StateList.RemoveAt(0);
                //QM.StateList.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));

                return View();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PrayasNew.PrayasNewLayout()");
                return null;
            }
            finally
            {
                if (comm != null)
                {
                    comm.Dispose();
                }
            }
        }


        // ================================================
        //  protected void generateJson_Click(object sender, EventArgs e)
        public ActionResult generateJsonClickNew()
        {
            #region
            // Make the Object of InputParam Class Defined in Record Class.
            var g = new PMGSY.Areas.PRAYASNEW.Models.CSRecordNew.InputParamNew();
            g.Instance_Code = Convert.ToInt32(Instance_Code);
            g.Sec_Code = Convert.ToInt32(Sec_Code);
            g.Ministry_Code = Convert.ToInt32(Ministry_Code);
            g.Dept_Code = Convert.ToInt32(Dept_Code);
            g.Project_Code = Convert.ToInt32(Project_Code);

            string datefrom = "01/01/1900";
            string dateto = "01/01/1900";

            // Insert a new record to maintain an audit trail of the usage of Web API.
            LogEntry();

            #endregion

            try
            {
                // Step 1: Call Method to Get Date Range "ApiConnectWithDaterange"
                string ResultMessage = "";
                DataSet ds2 = new DataSet();
                int cnt = 0;

                ds2 = cc.ConvertJsonToDatatable(ApiConnectWithDaterange(g));

                cnt = ds2.Tables[0].Rows.Count;
                if (cnt > 0)
                {
                    #region
                    string op = "";
                    op = ds2.Tables[0].Rows[0].ItemArray[0].ToString();

                    if (op == "0" | op == "99" | op.Length > 10)
                    {
                        datefrom = DateTime.Now.ToString("MM'/'dd'/'yyyy");
                        dateto = DateTime.Now.ToString("MM'/'dd'/'yyyy");
                        if (ds2.Tables[0].Rows[0].ItemArray[1].ToString().Split('#').Count() > 0)
                        {
                            // lblResponse.Text = ds2.Tables[0].Rows[0].ItemArray[1].ToString().Split('#')[1];
                            LogEntry(datefrom, dateto, ds2.Tables[0].Rows[0].ItemArray[1].ToString().Split('#')[1], 0, Convert.ToInt32(ds2.Tables[0].Rows[0].ItemArray[1].ToString().Split('#')[0]), 0);
                        }
                        else
                        {
                            // lblResponse.Text = ds2.Tables[0].Rows[0].ItemArray[1].ToString().Split('#')[0];
                            LogEntry(datefrom, dateto, ds2.Tables[0].Rows[0].ItemArray[1].ToString().Split('#')[0], 0, 0, 0);
                        }
                        //    return;
                        return Json(new { success = false, message = "No Date Found for Date Range" });
                    }
                    else


                        //Loop start for multiple dates
                        for (var i = 0; i <= ds2.Tables[0].Rows.Count - 1; i++)
                        {
                            datefrom = ds2.Tables[0].Rows[i].ItemArray[0].ToString();
                            dateto = ds2.Tables[0].Rows[i].ItemArray[0].ToString();
                            // ###############################################
                            remGroupId = ds2.Tables[0].Rows[i].ItemArray[1].ToString();
                            string[] Group_Id = remGroupId.Split('#');
                            int dateExp = 0;


                            // ###############################################
                            // 'Loop start for Group Id(s)
                            for (var j = 0; j <= Group_Id.Count() - 1; j++)
                            {


                                LogEntry(datefrom, dateto, "", Convert.ToInt32(Group_Id[j]), 0);


                                // If Step 1 executed successfully, Then Step 2 executes.
                                // Step 2: Call Method to PUSH DATA TO DARPAN DASHBOARD "ApiConnect"

                                // string ResultMessage = "";
                                ds = new DataSet();
                                param = new Hashtable();
                                param.Clear();

                                param.Add("@Instance_code", Instance_Code);
                                param.Add("@sec_code", Sec_Code);
                                param.Add("@Ministry_Code", Ministry_Code);
                                param.Add("@dept_code", Dept_Code);
                                param.Add("@project_code", Project_Code);
                                param.Add("@datadt_from", datefrom);
                                param.Add("@datadt_to", dateto);
                                param.Add("@Group_Id", Group_Id[j]);

                                ds = cc.getDatasetParametersp(SQLDBDataStoredProc, "", param);



                                if (ds.Tables.Count > 0)
                                {
                                    if (ds.Tables[0].Rows.Count > 0)
                                        ResultMessage = ApiConnectNew(fcs.ConvertToList(ds), g);
                                    else
                                    {
                                        dateExp = 1;
                                        ResultMessage = "Status: 0, Message:0#No Record found for the given date range and group id in client table.";
                                    }
                                }
                                else
                                {
                                    dateExp = 1;
                                    ResultMessage = "Status: 0,  Message:0#No Record found for the given date range and group id in client table.";
                                }

                                // If Step 2 executed successfully, you got the message from API in "ResultMessage" Variable.

                                DataSet ds1 = new DataSet();

                                ds1 = cc.ConvertJsonToDatatable(ResultMessage.Replace("=", ":"));

                                if (!ds1.Tables[0].Rows[0].ItemArray[1].ToString().Contains("Success"))
                                {
                                    dateExp = 1;

                                }


                                // Update the existing record with the values, return from Web Api as a response.


                                LogEntry(datefrom, dateto, ds1.Tables[0].Rows[0].ItemArray[1].ToString().Split('#')[1], Convert.ToInt32(Group_Id[j]), Convert.ToInt32(ds1.Tables[0].Rows[0].ItemArray[1].ToString().Split('#')[0]), Convert.ToInt32(ds1.Tables[0].Rows[0].ItemArray[0].ToString()));
                                //        LogEntry(datefrom, dateto, ds1.Tables[0].Rows[0].ItemArray[1].ToString());

                            }


                            if (dateExp == 1)
                                break;
                        }

                    #endregion
                }

                return Json(new { success = true, message = ResultMessage });
            }
            catch (Exception ex)
            {
                object response = null;
                var r = new { Status = "0", Message = ex.Message };
                response = r;

                // Inset/Update the existing record with the exception message.
                LogEntry(datefrom, dateto, ex.Message);
                return Json(new { success = false, message = "Error occurred" });
            }
        }

        // Method to Maintain Log Records
        protected void LogEntry(string datadt_from = null, string datadt_to = null, string Message = null, int Group_Id = 0, int errcode = 0, int status = 0)
        {
            try
            {
                ds = new DataSet();
                param = new Hashtable();
                param.Clear();

                param.Add("@Instance_Code", Instance_Code);
                param.Add("@sec_code", Sec_Code);
                param.Add("@ministry_Code", Ministry_Code);
                param.Add("@dept_code", Dept_Code);
                param.Add("@project_code", Project_Code);
                param.Add("@datadt_from", datadt_from);
                param.Add("@datadt_to", datadt_to);
                param.Add("@msg", Message);
                param.Add("@Group_Id", Group_Id);
                param.Add("@error_code", errcode);
                param.Add("@Status", status);
                ds = cc.getDatasetParametersp(SQLDBLogStoredProc, "", param);
            }
            catch (Exception ex)
            {
                object response = null;
                var r = new { Status = "0", Message = ex.Message };
                response = r;
            }
        }

        // Step 1: Method to Get Date Range
        protected string ApiConnectWithDaterange(PMGSY.Areas.PRAYASNEW.Models.CSRecordNew.InputParamNew api)
        {
            try
            {

                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.Timeout = TimeSpan.FromMinutes(60.0);

                var f = new PMGSY.Areas.PRAYASNEW.Models.CSRecordNew.InputParamNew();
                f = api;


                var inputContent = (new JavaScriptSerializer() { MaxJsonLength = 2147483644 }).Serialize(f);
                var buffer = Encoding.UTF8.GetBytes(inputContent);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = httpClient.PostAsync(GetDateRangeAPIUrl, byteContent).Result;
                string xml = null;

                if (response.IsSuccessStatusCode)
                {
                    xml = response.Content.ReadAsStringAsync().Result;

                    if (xml.Equals("null"))
                        xml = null;
                }
                else
                    xml = response.Content.ReadAsStringAsync().Result;

                return xml;
            }
            catch (Exception ex)
            {
                object response = null;
                var r = new { Status = "0", Message = ex.Message };
                response = r;
                return response.ToString();
            }
            finally
            {
            }
        }




        protected string ApiConnectNew(List<PMGSY.Areas.PRAYASNEW.Models.CSRecordNew.MasterdataNew> Records, PMGSY.Areas.PRAYASNEW.Models.CSRecordNew.InputParamNew api)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.Timeout = TimeSpan.FromMinutes(60.0);

                //Serialize the list of KPIData to json string
                var inputContent = (new JavaScriptSerializer() { MaxJsonLength = 2147483644 }).Serialize(Records);

                //Compress the json string
                string compressData = fcs.Compress(inputContent);

                //Encrypt the compressed string and assign to ProjrctKpiDetails Object
                List<PMGSY.Areas.PRAYASNEW.Models.CSRecordNew.ProjrctKpiDetailsNew> encrypted = new List<PMGSY.Areas.PRAYASNEW.Models.CSRecordNew.ProjrctKpiDetailsNew>()
            {
                new PMGSY.Areas.PRAYASNEW.Models.CSRecordNew.ProjrctKpiDetailsNew()
                {
                    EncyptedData = fcs.Encrypt(compressData),
                    IP = api
                }
            };

                //ProjrctKpiDetails is Serialized and converted to json string
                string data = JsonConvert.SerializeObject(encrypted, new JsonSerializerSettings()
                {
                    Error = (se, ev) =>
                    {
                        ev.ErrorContext.Handled = true;
                    }
                });

                StringContent sc = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage response = httpClient.PostAsync(PushDataAPIUrl, sc).Result;

                // =======================================NEW=================================================




                string xml = null;

                if (response.IsSuccessStatusCode)
                {
                    xml = response.Content.ReadAsStringAsync().Result;

                    if (xml.Equals("null"))
                        xml = null;
                }
                else
                    xml = response.Content.ReadAsStringAsync().Result;

                return xml;
            }
            catch (Exception ex)
            {
                object response = null;
                var r = new { Status = "0", Message = ex.Message };
                response = r;
                return response.ToString();
            }
            finally
            {
            }
        }





    }
}
