using PMGSY.Common;
using PMGSY.Models.Gepnic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Extensions;
using PMGSY.DAL.Gepnic;
using WebMatrix.WebData;
using System.Web.Security;
using System.Data.Entity;
using PMGSY.DAL;
using System.Data.Entity.Validation;
using System.Diagnostics;
using PMGSY.BAL.Menu;
using PMGSY.Models.Login;
using System.Text;
using System.Security.Cryptography;
using System.Xml;
using PMGSY.Models;
using System.Configuration;
using System.Web.Script.Serialization;

namespace PMGSY.Controllers
{
    public class GepnicController : Controller
    {
        GepnicDAL gepnicDAL = null;
        string message = String.Empty;
        PMGSYEntities dbContext = null;


        private static string ErrorLogBasePath = System.Configuration.ConfigurationSettings.AppSettings["GepnicNregaErrorLogPath"];
        private static string ErrorLogDirectory = ErrorLogBasePath + "\\" + "ErrorLog_" + DateTime.Now.Month + "_" + DateTime.Now.Year;
        private string ErrorLogFilePath = ErrorLogDirectory + "//ErrorLog_" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + ".txt";	

        /// <summary>
        /// To call, Gepnic or Nrega Data push/pull service
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            try
            {
                CommonFunctions objCommonFuntion = new CommonFunctions();
                GepnicNregaModel model = new GepnicNregaModel();
                model.STATES = objCommonFuntion.PopulateStates();
                model.Years = objCommonFuntion.PopulateYears(true);
                List<SelectListItem> lstType = new List<SelectListItem>();
                lstType.Insert(0, (new SelectListItem { Text = "GePNIC", Value = "G", Selected = true }));
                lstType.Insert(0, (new SelectListItem { Text = "NREGA", Value = "N" }));
                model.IntegrationTypeList = lstType;
                return View(model);
            }
            catch (Exception ex)
            {
                if (!Directory.Exists(ErrorLogDirectory))
                {
                    Directory.CreateDirectory(ErrorLogDirectory);
                }
                using (StreamWriter sw = System.IO.File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "GepnicController.cs - Index()");
                    sw.WriteLine("Exception : " + ex.Message);
                    sw.WriteLine("Exception : " + ex.StackTrace);
                    if (ex.InnerException != null)
                        sw.WriteLine("Inner Exception : " + ex.InnerException.StackTrace);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return null;
            }
            
        }


        /// <summary>
        /// post method for pushing data to Gepnic & Nrega Service
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(GepnicNregaModel model)
        {
            try
            {
                string status = string.Empty; 
                if (ModelState.IsValid)
                {
                    if (model.IntegrationType.Equals("G"))
                    {
                       status =  PMGSY.GepnicAndNrega.OMMASGePNICIntegration.PostSanctionDataToGepnic(model.MAST_STATE_CODE, model.IMS_YEAR, 1, 0, "P", 1);
                    }
                    else if (model.IntegrationType.Equals("N"))
                    {
                        status = PMGSY.GepnicAndNrega.OMMASNregaIntegration.PostSanctionDataToNrega(model.MAST_STATE_CODE, model.IMS_YEAR, 0, 0, "P", 
                            PMGSYSession.Current.PMGSYScheme);
                    }
                    return Json(new { Success = true, Status = status });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                if (!Directory.Exists(ErrorLogDirectory))
                {
                    Directory.CreateDirectory(ErrorLogDirectory);
                }
                using (StreamWriter sw = System.IO.File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "GepnicController.cs - Index post()");
                    sw.WriteLine("Exception : " + ex.Message);
                    sw.WriteLine("Exception : " + ex.StackTrace);
                    if (ex.InnerException != null)
                        sw.WriteLine("Inner Exception : " + ex.InnerException.StackTrace);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();

                }
                return Json(new { Success = false, ErrorMessage = "Exception occurred" });
            }
        }

        #region  Gepnic Work Item Details
        [HttpGet]
        public ActionResult GetGepnicTenderDetailsLayout()
        {
            try
            {
                CommonFunctions objCommonFuntion = new CommonFunctions();
                GepnicTenderDetailsViewModel model = new GepnicTenderDetailsViewModel();
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Gepnic.GetGepnicTenderDetailsLayout()");
                return null;
            }
        }

        [HttpGet]
        public ActionResult PopulatePackageRefNo()
        {
            try
            {
                gepnicDAL = new GepnicDAL();
                List<SelectListItem> list = gepnicDAL.PopulatePackageRefNoDAL();

                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Master.PopulateIfscByBankName()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult GetDataFromGepenic(GepnicTenderDetailsViewModel model)
        {
            bool status = false;
            try
            {
                string packageRefNo = model.packageRefNo;

                var gep = new GePNICWebReferencePush.PushTenderService();

                string strXML = gep.getTenderInfoByTenderRefNo(packageRefNo);



                //      string strXML1 = gep.getTenderInfoFromPreTenderbyPublishDate("20180811");
                //      string strXML2 = gep.getCorrigendumInfoFromPreTenderbyPublishDate("20180811"); 

                // Added on 5 Oct 2018

                string b = "\\GetTenderInfoByTenderRefNo.xml";

                StreamWriter file = new StreamWriter(ConfigurationManager.AppSettings["GepnicXMLs"].ToString() + b);

              
                file.Write(strXML);
                file.Close();   
                       


                XmlDocument xmlDoc = new XmlDocument();

                xmlDoc.LoadXml(strXML);

                XmlNodeList workItemNodes = xmlDoc.GetElementsByTagName("WORKITEM_DETAILS");
                XmlNodeList CorrigendumNodes = xmlDoc.GetElementsByTagName("CORRIGENDUM");
                XmlNodeList biddersNodes = xmlDoc.GetElementsByTagName("BIDDERS");
                XmlNodeList techOpenNodes = xmlDoc.GetElementsByTagName("TECH_OPEN");
                XmlNodeList techEvalNodes = xmlDoc.GetElementsByTagName("TECH_EVAL");
                XmlNodeList finOpenNodes = xmlDoc.GetElementsByTagName("FIN_OPEN");
                XmlNodeList finEvalNodes = xmlDoc.GetElementsByTagName("FIN_EVAL");
                XmlNodeList aocNodes = xmlDoc.GetElementsByTagName("AOC");



                gepnicDAL = new GepnicDAL();


                if (ModelState.IsValid)
                {
                    if (gepnicDAL.AddDetails(model, ref message, xmlDoc, workItemNodes, CorrigendumNodes, biddersNodes, techOpenNodes, techEvalNodes, finOpenNodes, finEvalNodes, aocNodes))
                    {
                        message = message == string.Empty ? "Details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Details not saved." : message;
                    }
                }
                else
                {
                    return PartialView("GetDataFromGepenic", model);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? ex.Message : ex.Message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
        }


        public ActionResult TenderList(int? page, int? rows, string sidx, string sord)
        {

            gepnicDAL = new GepnicDAL();
            long totalRecords = 0;
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = gepnicDAL.GetTenderList(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DPRProposalList()");
                return null;
            }
        }

        public ActionResult ViewTenderDetails(string id)
        {
            String[] urlSplitParams = id.Split('$');
            Int32 tenderCode = Convert.ToInt32(urlSplitParams[0]);
            dbContext = new PMGSYEntities();

            OMMAS_GEPNIC_TENDER_WORKITEM_DETAILS master = dbContext.OMMAS_GEPNIC_TENDER_WORKITEM_DETAILS.Find(tenderCode);
            WORKITEMDETAILS workItems = new WORKITEMDETAILS();


            try
            {
                workItems.WORKITEM_REF_NO = master.WORKITEM_REF_NO;
                workItems.ORG_CHAIN = master.ORG_CHAIN;
                workItems.T_REF_NO = master.T_REF_NO;
                workItems.T_TENDER_TYPE = master.T_TENDER_TYPE;
                workItems.T_FORM_OF_CONTRACT = master.T_FORM_OF_CONTRACT;
                workItems.T_TENDER_CATEGORY = master.T_TENDER_CATEGORY;
                workItems.W_TITLE = master.W_TITLE;
                workItems.W_DESC = master.W_DESC;
                workItems.W_PRE_QUAL = master.W_PRE_QUAL;

                workItems.W_PROD_CAT = master.W_PROD_CAT;
                workItems.W_PROD_SUB_CAT = master.W_PROD_SUB_CAT;
                workItems.W_VALUE = (master.W_VALUE == (decimal?)null ? string.Empty : Convert.ToString(master.W_VALUE));
                workItems.W_BIDVALIDITY = (master.W_BIDVALIDITY == (int?)null ? string.Empty : Convert.ToString(master.W_BIDVALIDITY));
                workItems.W_LOCATION = master.W_LOCATION;
                workItems.W_PINCODE = Convert.ToInt32(master.W_PINCODE);
                workItems.W_PREBID_MEET_PLACE = master.W_PREBID_MEET_PLACE;
                workItems.W_PREBID_ADDRESS = master.W_PREBID_ADDRESS;
                workItems.W_BID_OPENING_PLACE = master.W_BID_OPENING_PLACE;
                workItems.W_INVITING_OFFICER = master.W_INVITING_OFFICER;
                workItems.W_INVITING_OFF_ADDRESS = master.W_INVITING_OFF_ADDRESS;
                workItems.W_TENDER_FEE = master.W_TENDER_FEE;
                workItems.W_TF_PAYABLE_TO = master.W_TF_PAYABLE_TO;

                workItems.W_TF_PAYABLE_AT = master.W_TF_PAYABLE_AT;
                workItems.W_EMD_FEE = master.W_EMD_FEE;
                workItems.W_EMD_PAYABLE_TO = master.W_EMD_PAYABLE_TO;
                workItems.W_EMD_PAYABLE_AT = master.W_EMD_PAYABLE_AT;
                workItems.W_PUB_DATE = (master.W_PUB_DATE == (DateTime?)null ? String.Empty : Convert.ToString(master.W_PUB_DATE));
                workItems.W_DOC_START_DATE = (master.W_DOC_START_DATE == (DateTime?)null ? String.Empty : Convert.ToString(master.W_DOC_START_DATE));
                workItems.W_DOC_END_DATE = (master.W_DOC_END_DATE == (DateTime?)null ? String.Empty : Convert.ToString(master.W_DOC_END_DATE));
                workItems.W_SEEK_CLAR_START_DATE = (master.W_SEEK_CLAR_START_DATE == (DateTime?)null ? String.Empty : Convert.ToString(master.W_SEEK_CLAR_START_DATE));
                workItems.W_SEEK_CLAR_END_DATE = (master.W_SEEK_CLAR_END_DATE == (DateTime?)null ? String.Empty : Convert.ToString(master.W_SEEK_CLAR_END_DATE));
                workItems.W_PREBID_DATE = (master.W_PREBID_DATE == (DateTime?)null ? String.Empty : Convert.ToString(master.W_PREBID_DATE));

                workItems.W_BIDSUB_START_DATE = (master.W_BIDSUB_START_DATE == (DateTime?)null ? String.Empty : Convert.ToString(master.W_BIDSUB_START_DATE));
                workItems.W_BIDSUB_END_DATE = (master.W_BIDSUB_END_DATE == (DateTime?)null ? String.Empty : Convert.ToString(master.W_BIDSUB_END_DATE));
                workItems.W_BID_OPEN_DATE = (master.W_BID_OPEN_DATE == (DateTime?)null ? String.Empty : Convert.ToString(master.W_BID_OPEN_DATE));
                workItems.W_FIN_BID_OPEN_DATE = (master.W_FIN_BID_OPEN_DATE == (DateTime?)null ? String.Empty : Convert.ToString(master.W_FIN_BID_OPEN_DATE));
                workItems.W_BID_OPENERS = master.W_BID_OPENERS;
                workItems.W_RETURN_URL = master.W_RETURN_URL;
                workItems.W_NO_OF_BIDS = (master.W_NO_OF_BIDS == (int?)null ? String.Empty : Convert.ToString(master.W_NO_OF_BIDS));



            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ViewTenderDetails().DAL");
                //return false;
            }
            return View(workItems);
        }



        public ActionResult CorrigendumList(int? page, int? rows, string sidx, string sord)
        {
            int TENDER_CODE = Convert.ToInt32(Request["TENDER_ID"]);
            gepnicDAL = new GepnicDAL();
            long totalRecords = 0;
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = gepnicDAL.GetCorrigendumList(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, TENDER_CODE),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CorrigendumList()");
                return null;
            }
        }



        public ActionResult BidderList(int? page, int? rows, string sidx, string sord)
        {
            int TENDER_CODE = Convert.ToInt32(Request["TENDER_ID"]);
            gepnicDAL = new GepnicDAL();
            long totalRecords = 0;
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = gepnicDAL.GetBidderList(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, TENDER_CODE),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "BidderList()");
                return null;
            }
        }


        public ActionResult BidOpenEvalList(int? page, int? rows, string sidx, string sord)
        {
            int TENDER_CODE = Convert.ToInt32(Request["TENDER_ID"]);
            gepnicDAL = new GepnicDAL();
            long totalRecords = 0;
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = gepnicDAL.GetOpenEvalDAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, TENDER_CODE),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "BidderList()");
                return null;
            }
        }


        public ActionResult AOCList(int? page, int? rows, string sidx, string sord)
        {
            int TENDER_CODE = Convert.ToInt32(Request["TENDER_ID"]);
            gepnicDAL = new GepnicDAL();
            long totalRecords = 0;
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = gepnicDAL.AOCListDAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, TENDER_CODE),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AOCList()");
                return null;
            }
        }

        #endregion

        #region Tender Info By Publish Date

        [HttpGet]
        public ActionResult GetGepnicServices()
        {
            try
            {
                CommonFunctions objCommonFuntion = new CommonFunctions();
                GepnicTenderDetailsViewModel model = new GepnicTenderDetailsViewModel();
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Gepnic.GetGepnicServices()");
                return null;
            }
        }


        [HttpGet]
        public ActionResult GetTenderXMLFromGepenicLayout()
        {
            try
            {
                CommonFunctions objCommonFuntion = new CommonFunctions();
                XMLCreation model = new XMLCreation();

            

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Gepnic.GetTenderXMLFromGepenicLayout()");
                return null;
            }
        }


        [HttpPost]
        public ActionResult GetTenderXMLFromGepenic(XMLCreation model)
        {
            bool status = false;
            try
            {
                string publishedDate = model.DateStringNumber; // "20180811"
                var gep = new GePNICWebReferencePush.PushTenderService();
                string strXML1 = gep.getTenderInfoFromPreTenderbyPublishDate(publishedDate);


                // Added on 5 Oct 2018
             

                string c = "\\GetTenderInfoFromPreTenderbyPublishDate.xml";
                StreamWriter file = new StreamWriter(ConfigurationManager.AppSettings["GepnicXMLs"].ToString() + c);

                file.Write(strXML1);
                file.Close();   


                XmlDocument xmlDoc = new XmlDocument();

                xmlDoc.LoadXml(strXML1);

                XmlNodeList workItemNodes = xmlDoc.GetElementsByTagName("TENDERDATA");

                gepnicDAL = new GepnicDAL();


                if (ModelState.IsValid)
                {
                    if (gepnicDAL.AddDetailsTender(model, ref message, xmlDoc, workItemNodes))
                    {
                        message = message == string.Empty ? "Tender Information Details By Published Date saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Details are not saved." : message;
                    }
                }
                else
                {
                    return PartialView("GetTenderXMLFromGepenic", model);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? ex.Message : ex.Message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
        }

        // TenderListByPublishedDate

        public ActionResult TenderListByPublishedDate(int? page, int? rows, string sidx, string sord)
        {

            gepnicDAL = new GepnicDAL();
            long totalRecords = 0;
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = gepnicDAL.GetTenderListByPublishedDate(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };


               

                //var result = jsonData;
                //var jsonResult = Json(result);
                //jsonResult.MaxJsonLength = int.MaxValue;


                //return Json(jsonResult, JsonRequestBehavior.AllowGet);



                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return jsonResult;


             //   return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DPRProposalList()");
                return null;
            }
        }

        #endregion

        #region Corr Info By Publish Date
        [HttpGet]
        public ActionResult GetCorrInfoByPublishDateLayout()
        {
            try
            {
                CommonFunctions objCommonFuntion = new CommonFunctions();
                XMLCreation model = new XMLCreation();
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Gepnic.GetCorrInfoByPublishDateLayout()");
                return null;
            }
        }
        // GetCorrinfoDetailsBYPublishedDate

        [HttpPost]
        public ActionResult GetCorrinfoDetailsBYPublishedDate(XMLCreation model)
        {
            bool status = false;
            try
            {
                string publishedDate = model.DateStringNumber; // "20180811"

                var gep = new GePNICWebReferencePush.PushTenderService();

                string strXML = gep.getCorrigendumInfoFromPreTenderbyPublishDate(publishedDate);


                // Added on 5 Oct 2018
            

                string d = "\\GetCorrigendumInfoFromPreTenderbyPublishDate.xml";
                StreamWriter file = new StreamWriter(ConfigurationManager.AppSettings["GepnicXMLs"].ToString() + d);

                file.Write(strXML);
                file.Close();   


                XmlDocument xmlDoc = new XmlDocument();

                xmlDoc.LoadXml(strXML);
                XmlNodeList CorrigendumNodes = xmlDoc.GetElementsByTagName("TENDERDATA");
                gepnicDAL = new GepnicDAL();

                if (ModelState.IsValid)
                {
                    if (gepnicDAL.AddDetailsCorrInfoByPublishedDate(model, ref message, xmlDoc, CorrigendumNodes))
                    {
                        message = message == string.Empty ? "Details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Details not saved." : message;
                    }
                }
                else
                {
                    return PartialView("GetCorrInfoByPublishDateLayout", model);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? ex.Message : ex.Message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
        }

        public ActionResult ListCorrinfoDetailsBYPublishedDate(int? page, int? rows, string sidx, string sord)
        {
            gepnicDAL = new GepnicDAL();
            long totalRecords = 0;
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = gepnicDAL.GetCorrListByPublishedDate(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };

                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return jsonResult;

               // return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ListCorrinfoDetailsBYPublishedDate()");
                return null;
            }
        }


        #endregion

        #region AOC Info By Publish Date

        [HttpGet]
        public ActionResult GetAOCInfoByPublishDateLayout()
        {
            try
            {
                CommonFunctions objCommonFuntion = new CommonFunctions();
                XMLCreation model = new XMLCreation();
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Gepnic.GetAOCInfoByPublishDateLayout()");
                return null;
            }
        }
        // GetCorrinfoDetailsBYPublishedDate

        [HttpPost]
        public ActionResult GetAOCinfoDetailsBYPublishedDate(XMLCreation model)
        {
            bool status = false;
            try
            {
                string publishedDate = model.DateStringNumber; // "20180811"

                var gep = new GePNICWebReferencePush.PushTenderService();

                string strXML = gep.getAocInfoFromPreTenderbyCreatedDate(publishedDate);


                // Added on 5 Oct 2018

                string a = "\\GetAocInfoFromPreTenderbyCreatedDate.xml";
                StreamWriter file = new StreamWriter(ConfigurationManager.AppSettings["GepnicXMLs"].ToString() + a);
                
               
               
                file.Write(strXML);
                file.Close();   

                XmlDocument xmlDoc = new XmlDocument();

                xmlDoc.LoadXml(strXML);
                XmlNodeList CorrigendumNodes = xmlDoc.GetElementsByTagName("TENDERDATA");
                gepnicDAL = new GepnicDAL();

                if (ModelState.IsValid)
                {
                    if (gepnicDAL.AddDetailsAOCInfoByPublishedDate(model, ref message, xmlDoc, CorrigendumNodes))
                    {
                        message = message == string.Empty ? "Details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Details not saved." : message;
                    }
                }
                else
                {
                    return PartialView("GetAOCInfoByPublishDateLayout", model);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? ex.Message : ex.Message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
        }

        public ActionResult ListAOCinfoDetailsBYPublishedDate(int? page, int? rows, string sidx, string sord)
        {
            gepnicDAL = new GepnicDAL();
            long totalRecords = 0;
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = gepnicDAL.GetAOCListByPublishedDate(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ListAOCinfoDetailsBYPublishedDate()");
                return null;
            }
        }

        #endregion

    }
}


