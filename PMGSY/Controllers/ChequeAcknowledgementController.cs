#region File Header
/* 
     *  Name : ChequeAcknowledgementController.cs
     *  Path : ~Controller\ChequeAcknowledgementController.cs
     *  Description : ChequeAcknowledgementController.cs is Controller used for Cheque acknowledgment module.

                      
     *  Functions / Procedures Called : 
            
         * GetChequeAckMasterList()
                                               

     * classes used 
        
        * ChequeAcknowledgementController
                                                   
 
     *  Author : Amol Jadhav (PE, e-gov)
     *  Company : C-DAC,E-GOV
     *  Dates of creation : 15/06/2013  
    
*/
#endregion


using PMGSY.BAL.ChequeAcknowledgement;
using PMGSY.Common;
using PMGSY.DAL.ChequeAcknowledgement;
using PMGSY.Extensions;
using PMGSY.Models;
using PMGSY.Models.ChequeAcknowledgement;
using PMGSY.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class ChequeAcknowledgementController : Controller
    {
        IChequeAcknowledgementBAL objChqAckBAL = null;
        CommonFunctions common = null;

        public ChequeAcknowledgementController()
        {
            objChqAckBAL = new ChequeAcknowledgementBAL();
        }

        /// <summary>
        /// action to return the master list page for cheque acknowledgment
        /// </summary>
        /// <returns> view of master page </returns>
        [Audit]
        public ActionResult GetChequeAckMasterList()
        {

            TransactionParams objparams = new TransactionParams();
            common = new CommonFunctions();
            Dictionary<string, string> decryptedParameters = null;
            String[] encryptedParameters = null;
            //Added Abhishek kamble 8-June-2014
            string hdBillID = String.Empty;

            try
            {
                CheckAckSelectionModel model = new CheckAckSelectionModel();
                if ((Request.Params["DPIU"] != null) && (Request.Params["Month"] != null) && (Request.Params["Year"] != null) && (Request.Params["BillNo"] != null))
                {
                    model.DPIU = Convert.ToInt16(Request.Params["DPIU"]);
                    model.BILL_MONTH = Convert.ToInt16(Request.Params["Month"]);
                    model.BILL_YEAR = Convert.ToInt16(Request.Params["Year"]);

                    string billNo = Request.Params["BillNo"];
                    encryptedParameters = billNo.Split('/');
                    decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    int billID = Convert.ToInt32(decryptedParameters["BillId"]);

                    //Added Abhishek kamble 8-June-2014
                    //ViewBag.hdBillId = hdBillID;                      

                    // hdBillID = billNo;
                    hdBillID = URLEncrypt.EncryptParameters(new string[] { billID.ToString() });

                    model = objChqAckBAL.GetSelectionDetails(billID);
                    // Added by Srishti on 03-03-2023
                    if (PMGSYSession.Current.FundType == "P")
                        model.ACCOUNT_TYPE = Request.Params["ACC_TYPE"];
                    else
                        model.ACCOUNT_TYPE = "1";

                    //If Month,Year, PIU Details Not Present In ACC_CHEQUE_ISSUED then set using Request Params new Change star by abhisehk kamble 8-July-2014 for avoid dublicate entry
                    //if (model.DPIU == 0)
                    //{
                    //    model.DPIU = Convert.ToInt16(Request.Params["DPIU"]);
                    //    model.BILL_MONTH = Convert.ToInt16(Request.Params["Month"]);
                    //    model.BILL_YEAR = Convert.ToInt16(Request.Params["Year"]);    
                    //}
                    //If Month,Year, PIU Details Not Present In ACC_CHEQUE_ISSUED then set using Request Params new Change End by abhisehk kamble 8-July-2014 for avoid dublicate entry


                }
                model.Mode = Request.Params["Mode"];
                objparams.BILL_TYPE = "P";
                objparams.FUND_TYPE = PMGSYSession.Current.FundType;
                objparams.LVL_ID = PMGSYSession.Current.LevelId;
                objparams.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                objparams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;

                //List<SelectListItem> monthList = common.PopulateMonths(DateTime.Now.Month);
                List<SelectListItem> monthList = common.PopulateMonths(model.BILL_MONTH);
                model.BILL_MONTH_LIST = monthList;

                //List<SelectListItem> yearList = common.PopulateYears(DateTime.Now.Year);
                List<SelectListItem> yearList = common.PopulateYears(model.BILL_YEAR);
                model.BILL_YEAR_LIST = yearList;

                List<SelectListItem> DPIUList = common.PopulateDPIU(objparams);
                model.DPIU_LIST = DPIUList;

                //Added Abhishek kamble 8-June-2014  
                model.hdnBillID = hdBillID;

                //Added Abhishek kamble 26-Aug-2014 
                if (Request.Params["AckUnackFlag"] != null)
                {
                    model.AckUnackFlag = Request.Params["AckUnackFlag"];
                }

                // Added by Srishti on 03-03-2023
                List<SelectListItem> lstAccType = new List<SelectListItem>();
                lstAccType.Insert(0, new SelectListItem { Text = "Select Account Type", Value = "0" });
                lstAccType.Insert(1, new SelectListItem { Text = "SNA", Value = "1" });
                lstAccType.Insert(2, new SelectListItem { Text = "Holding", Value = "2" });
                lstAccType.Insert(3, new SelectListItem { Text = "Security Deposit Account", Value = "3" });
                model.ACCOUNT_TYPE_LIST = lstAccType;

                return View("GetChequeAckMasterList", model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                string errorMsg = string.Format("Errors: {0}", ex.Message);
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return Json(string.Empty);
            }

        }

        /// <summary>
        /// Action to return the cheque list for acknowledgemnt
        /// </summary>
        /// <param name="page">page number</param>
        /// <param name="rows"> number of rows to display</param>
        /// <param name="sidx"> </param>
        /// <param name="sord"> </param>
        /// <returns></returns>
        [Audit]
        public JsonResult ListChequeForAcknowledgment(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                //Adde By Abhishek kamble 30-Apr-2014 start  
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 30-Apr-2014 end

                //Added By Abhishek kamble 16-July-2014 Get SRRDA BILL_ID From billMaster and Bill_Details Table
                string SrrdaBillId = "";

                PaymentFilterModel objFilter = new PaymentFilterModel();

                short month = 0;
                short year = 0;
                short DPIU = 0;
                string ErrorMessage = "invalid input";

                try
                {
                    month = Convert.ToInt16(Request.Params["months"]);
                    year = Convert.ToInt16(Request.Params["year"]);
                    DPIU = Convert.ToInt16(Request.Params["DPIU"]);
                    objFilter.AckUnackFlag = Request.Params["AckUnackFlag"];

                    if (month <= 0)
                    {
                        ErrorMessage = "Invalid Month";
                    }
                    else if (year <= 0)
                    {
                        ErrorMessage = "Invalid year";
                    }
                    else if (DPIU <= 0)
                    {
                        ErrorMessage = "Invalid DPIU";
                    }

                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                    string errorMsg = ErrorMessage;
                    Response.TrySkipIisCustomErrors = true;
                    Response.StatusCode = 500;
                    Response.Write(errorMsg);
                    return Json(string.Empty);
                }

                long totalRecords;
                objFilter.Month = month;
                objFilter.Year = year;


                objFilter.page = Convert.ToInt32(page) - 1;
                objFilter.rows = Convert.ToInt32(rows);
                objFilter.sidx = sidx.ToString();
                objFilter.sord = sord.ToString();

                objFilter.Bill_type = "P";
                objFilter.AdminNdCode = DPIU; ;
                objFilter.FundType = PMGSYSession.Current.FundType;
                objFilter.LevelId = PMGSYSession.Current.LevelId;

                //Added by Srishti on 03-03-2023
                if (PMGSYSession.Current.FundType == "P")
                    objFilter.Account_Type = Request.Params["ACC_TYPE"];
                else
                    objFilter.Account_Type = "1";

                List<String> SelectedIdList = new List<String>();



                var jsonData = new
                {
                    rows = objChqAckBAL.ListChequeForAcknowledgment(objFilter, out totalRecords, out SelectedIdList, ref SrrdaBillId),
                    total = totalRecords <= objFilter.rows ? 1 : totalRecords / objFilter.rows + 1,
                    page = objFilter.page,
                    records = totalRecords,
                    userdata = new { ids = SelectedIdList.ToArray<string>(), billID = SrrdaBillId }

                    //billID = URLEncrypt.EncryptParameters(new string[] { "1831319" })
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                string errorMsg = string.Format("Errors: {0}", ex.Message);
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return Json(string.Empty);
            }

        }

        /// <summary>
        /// function to get voucher ack form
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult GetVoucherAckForm()
        {
            return View("ChequeAckVoucherDetailsView");
        }

        /// <summary>
        /// action to acknowledge the selected cheques for acknowledgement 
        /// </summary>
        /// <param name="model"> model with selected cheque id acknowledge & save </param>
        /// <returns> 1 on success  else -111 to already finalized ack voucher</returns>
        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitChequesForAcknowledgement(CheckAckModel model, string id)
        {

            long bill_id = 0;

            int i = 0;
            try
            {

                //check the DPIU Code
                short dpiu;
                if (model.DPIU_CODE <= 0)
                {
                    throw new Exception("Invalid DPIU");
                }
                else
                {

                    bool isShort = short.TryParse(model.DPIU_CODE.ToString(), out dpiu);

                    if (!isShort)
                    {

                        throw new Exception("Invalid DPIU");
                    }
                }

                //if voucher number already entered do not check for unique constraint.
                if (!String.IsNullOrEmpty(model.STR_NA_BILL_ID))
                {

                    string[] paramAckBillValues = model.STR_NA_BILL_ID.Split('/');

                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { paramAckBillValues[0], paramAckBillValues[1], paramAckBillValues[2] });
                    if (urlParams.Length >= 1)
                    {

                        model.NA_BILL_ID = Convert.ToInt64(urlParams[0]);

                    }

                    string result = objChqAckBAL.GetAcknowledgedCheuesDetails(model.NA_BILL_ID);
                    if (result.Split('$')[1] == model.BILL_NO)
                    {

                        if (ModelState.ContainsKey("BILL_NO"))
                            ModelState["BILL_NO"].Errors.Clear();
                    }

                }
                ChequeAcknowledgementDAL objChqAckDAL = new ChequeAcknowledgementDAL();
                string message = string.Empty;
                bool flag = objChqAckDAL.checkSRRDAMonthCloseDAL(model.BILL_MONTH_VOUCHER, model.BILL_YEAR_VOUCHER, ref message);
                if (!flag)
                {
                    ModelState.AddModelError("BILL_DATE", message);
                }
                if (ModelState.IsValid)
                {

                    short month = Convert.ToInt16(model.BILL_DATE.Split('/')[1]);
                    short year = Convert.ToInt16(model.BILL_DATE.Split('/')[2]);

                    if (model.BILL_MONTH_VOUCHER == month && model.BILL_YEAR_VOUCHER == year)
                    {
                        long[] array_bill_id=null;

                        if (model.SelectedIDArray != null)
                        {
                            string[] selectedIDs = model.SelectedIDArray.Split(',');

                             array_bill_id = new long[selectedIDs.Length];


                            foreach (var item in selectedIDs)
                            {
                                //decrypt
                                string[] paramValues = item.Split('/');

                                if (!String.IsNullOrEmpty(paramValues[0]) && !String.IsNullOrEmpty(paramValues[1]) && !String.IsNullOrEmpty(paramValues[2]))
                                {
                                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { paramValues[0], paramValues[1], paramValues[2] });
                                    if (urlParams.Length >= 1)
                                    {

                                        bill_id = Convert.ToInt64(urlParams[0]);
                                        array_bill_id[i] = bill_id;
                                        i++;
                                    }
                                }
                                else
                                {
                                    throw new Exception("Error while getting selected Cheque details");

                                }


                            }
                        }
                        //Added By Abhishek 23-Aug-2014  to get Acknowledged BIll Ids and select only new chq bill Ids using except start
                        if (model.AckBillIDArray != null && id != "Unacknowledge")
                        {
                            String[] ackBillIds = model.AckBillIDArray.Split(',');
                            long[] ack_bill_id_array = ackBillIds.Select(s => Convert.ToInt64(s)).ToArray();
                            //remove the common cheques they are allready acknowledged
                            //        AckchequesBillId.RemoveAll(c => array_bill_id.Contains(c));   
                            if (array_bill_id != null)
                            {
                                array_bill_id = array_bill_id.Except(ack_bill_id_array).ToArray<long>();
                            }
                        }
                        else if (model.AckBillIDArray != null && id == "Unacknowledge")
                        {    
                            String[] ackBillIds = model.AckBillIDArray.Split(',');
                            long[] ack_bill_id_array = ackBillIds.Select(s => Convert.ToInt64(s)).ToArray();                            
                            //array_bill_id = array_bill_id.Except(ack_bill_id_array ).ToArray<long>();                           
                            if (array_bill_id != null)
                            {
                                array_bill_id = array_bill_id.Intersect(ack_bill_id_array).ToArray<long>();
                            }
                        }
                       
                        //Added By Abhishek 23-Aug-2014 end

                        string result = string.Empty;
                        if (id == "Submit" || id == "SubmitNFinalise")
                        {
                            result = objChqAckBAL.AcknowledgeCheues(model, array_bill_id, model.Finalize);
                            // ViewBag.VoucherNo = result;
                        }
                        else
                        {
                            ChequeAcknowledgementDAL objDAL = new ChequeAcknowledgementDAL();
                            result = objDAL.UnauthrizeVoucher(array_bill_id, model);

                        }

                        //sucess
                        if (result == "1")
                        {
                            return Json(new
                            {
                                Success = true
                            });
                        }

                        //added by ashish markande on 12/10/2013
                        else if (result == "2")
                        {
                            return Json(new
                            {
                                Success = true,
                                statusCode = "111"//Unacknowledge successfully
                            });
                        }//end.

                        //already finalized
                        else if (result == "-111")
                        {
                            return Json(new
                            {
                                Success = false,
                                statusCode = "-111"
                            });
                        }

                        //month is not closed
                        else if (result == "-222")
                        {
                            return Json(new
                            {
                                Success = false,
                                statusCode = "-222"
                            });
                        }

                        //date is not of the last date of the month
                        else if (result == "-333")
                        {
                            return Json(new
                            {
                                Success = false,
                                statusCode = "-333"
                            });
                        }
                        else if (result == "-444")
                        {
                            return Json(new
                            {
                                Success = false,
                                statusCode = "-444"
                            });
                        }
                        else if (result == "-555")
                        {
                            return Json(new
                            {
                                Success = false,
                                statusCode = "-555"
                            });
                        }
                        else if (result == "-999")
                        {
                            return Json(new
                            {
                                Success = false,
                                statusCode = "-999"
                            });
                        }
                        else if(result=="-777"){
                            return Json(new
                            {
                                Success = false,
                                statusCode = "-777"
                            });
                        }
                        else
                        {
                            return Json(new
                            {
                                Success = true,
                                //voucher = result
                            });
                        }
                    }
                    else
                    {

                        ModelState.AddModelError("BILL_DATE", "Voucher Date must be in selected month and year");

                        //create the view 

                        return View("ChequeAckVoucherDetailsView", model);
                    }

                }
                else
                {

                    //create the view 

                    return View("ChequeAckVoucherDetailsView", model);

                }

            }

            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                string errorMsg = string.Format("Errors: {0}", ex.Message);
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return Json(string.Empty);
            }
        }

        /// <summary>
        /// action to get the ack details of already ack cheques
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult GetAcknowledgedChequeDetails(String parameter, String hash, String key)
        {
            try
            {

                long bill_Id = 0;

                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {

                        bill_Id = Convert.ToInt64(urlParams[0]);

                        string result = objChqAckBAL.GetAcknowledgedCheuesDetails(bill_Id);

                        return Json(new
                        {
                            Success = result

                        });

                    }
                    else
                    {
                        throw new Exception("Error while getting acknowledgement details..");
                    }
                }
                else
                {
                    throw new Exception("Error while getting acknowledgement details..");
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                string errorMsg = string.Format("Errors: {0}", ex.Message);
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return Json(string.Empty);
            }

        }

        [Audit]
        public ActionResult ViewVoucherDetails()
        {
            common = new CommonFunctions();
            List<SelectListItem> lstMonths = common.PopulateMonths(DateTime.Now.Month);
            lstMonths.RemoveAt(0);
            lstMonths.Insert(0, new SelectListItem { Text = "All Months", Value = "0" });
            ViewBag.ddlMonth = lstMonths;
            List<SelectListItem> lstYears = common.PopulateYears(DateTime.Now.Year);
            lstYears.RemoveAt(0);
            lstYears.Insert(0, new SelectListItem { Text = "All Years", Value = "0" });
            ViewBag.ddlYear = lstYears;
            TransactionParams objMaster = new TransactionParams();
            objMaster.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
            objMaster.DISTRICT_CODE = 0;
            List<SelectListItem> lstDPIU = common.PopulateDPIU(objMaster);
            lstDPIU.RemoveAt(0);
            lstDPIU.Insert(0, new SelectListItem { Text = "All DPIUs", Value = "0" });
            ViewBag.ddlDPIU = lstDPIU;
            // Added by Srishti on 03-03-2023
            List<SelectListItem> lstAccType = new List<SelectListItem>();
            lstAccType.Insert(0, new SelectListItem { Text = "Select Account Type", Value = "0" });
            lstAccType.Insert(1, new SelectListItem { Text = "SNA", Value = "1" });
            lstAccType.Insert(2, new SelectListItem { Text = "Holding", Value = "2" });
            lstAccType.Insert(3, new SelectListItem { Text = "Security Deposit Account", Value = "3" });
            ViewBag.ddlAccType = lstAccType;

            // Added by Srishti on 06-03-2023 
            ViewBag.fundType = PMGSYSession.Current.FundType.ToUpper();

            return View("ViewVoucherDetails");
        }
        [Audit]
        public ActionResult ListVoucherDetails(int? page, int? rows, string sidx, string sord)
        {
            ChequeAcknowledgementBAL objBAL = new ChequeAcknowledgementBAL();
            long totalRecords = 0;
            CheckAckSelectionModel model = new CheckAckSelectionModel();
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            try
            {
                if (!(string.IsNullOrEmpty(Request.Params["BILL_MONTH"])))
                {
                    model.BILL_MONTH = Convert.ToInt16(Request.Params["BILL_MONTH"]);
                }
                if (!(string.IsNullOrEmpty(Request.Params["BILL_YEAR"])))
                {
                    model.BILL_YEAR = Convert.ToInt16(Request.Params["BILL_YEAR"]);
                }
                if (!(string.IsNullOrEmpty(Request.Params["DPIU"])))
                {
                    model.DPIU = Convert.ToInt16(Request.Params["DPIU"]);
                }
                //Added by Srishti on 03-03-2023
                if (!(string.IsNullOrEmpty(Request.Params["ACC_TYPE"])))
                {
                    model.ACCOUNT_TYPE = Request.Params["ACC_TYPE"];
                }
                if (PMGSYSession.Current.FundType != "P")
                    model.ACCOUNT_TYPE = "1";

                var jsonData = new
                {
                    rows = objBAL.ListVoucherDetailsBAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, model),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);



            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        //Added by Ashish Markande on 21/08/2013
        /// <summary>
        /// Method is to check month is already closed or not.
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult CheckMonthClose()
        {
            ChequeAcknowledgementDAL objDAL = new ChequeAcknowledgementDAL();
            CheckAckSelectionModel model = new CheckAckSelectionModel();
            string message = string.Empty;
            model.fundType = PMGSYSession.Current.FundType;
            try
            {
                if (!(string.IsNullOrEmpty(Request.Params["BILL_MONTH"])))
                {
                    model.BILL_MONTH = Convert.ToInt16(Request.Params["BILL_MONTH"]);
                }
                if (!(string.IsNullOrEmpty(Request.Params["BILL_YEAR"])))
                {
                    model.BILL_YEAR = Convert.ToInt16(Request.Params["BILL_YEAR"]);
                }
                if (!(string.IsNullOrEmpty(Request.Params["DPIU"])))
                {
                    model.DPIU = Convert.ToInt16(Request.Params["DPIU"]);
                }
                bool check = objDAL.checkMonthClose(model, ref message);
                if (check == false)
                {
                    //message = message;
                    return Json(new { success = check, message = message });
                }
                else
                {
                    return Json(new { success = check, message = message });
                }

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult ValidatePreviousCheques(CheckAckSelectionModel model)
        {
            bool status = false;
            string statusCode = string.Empty;
            string message = string.Empty;
            try
            {
                objChqAckBAL = new ChequeAcknowledgementBAL();

                if (ModelState.IsValid)
                {
                    statusCode = objChqAckBAL.ValidatePreviousCheques(model, ref message);
                }

                if (statusCode == "111")
                {
                    return Json(new { status = true, statusCode = statusCode });
                }
                else
                {
                    return Json(new { status = status, statusCode = statusCode });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { status = status, statusCode = statusCode });
            }
        }

        /// <summary>
        /// Added By Abhishek kamble to delete Cheque Ack Details 4-july-2014
        /// </summary>
        /// <returns></returns>
        [HttpPost]  
        public ActionResult DeleteChequeAckVocherDetails()
        {
            //bool status = false;            
            string message = string.Empty;
            try
            {       
                objChqAckBAL = new ChequeAcknowledgementBAL();

                if (!String.IsNullOrEmpty(Request.Params["Month"]) && !String.IsNullOrEmpty(Request.Params["Year"]) && !String.IsNullOrEmpty(Request.Params["DPIU"]) && !String.IsNullOrEmpty(Request.Params["BillNo"]))
                {

                    Dictionary<string, string> decryptedParameters = null;

                    int BIllMonth = Convert.ToInt32(Request.Params["Month"]);
                    int BIllYear = Convert.ToInt32(Request.Params["Year"]);
                    int AdminNdCode = Convert.ToInt32(Request.Params["DPIU"]);
                    string encBillId = Request.Params["BillNo"];
                    Int64 BillID;

                    string[] paramAckBillID = encBillId.Split('/');         
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { paramAckBillID[0], paramAckBillID[1], paramAckBillID[2] });
                    BillID = Convert.ToInt64(decryptedParameters["BillId"]);

                    
                    if (objChqAckBAL.DeleteChequeAckVocherDetails(BIllMonth,BIllYear,AdminNdCode,BillID,ref message))
                    {
                        return Json(new { status = true, message = "Cheque Acknoledgement details deleted successfully." });
                    }
                    else
                    {
                        return Json(new { status = false, message = "An error occured while processing your request." });
                    }                    
                }
                else {
                    return Json(new { status = false, message = "An error occured while processing your request." });                
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { status = false, message = "An error occured while processing your request."});                
            }
        }

        //public ActionResult UnauthorizeVoucher(CheckAckModel model)
        //{
        //    ChequeAcknowledgementDAL objDAL = new ChequeAcknowledgementDAL();
        //    long bill_id = 0;

        //    int i = 0;

        //    if (model.STR_NA_BILL_ID != null)
        //    {
        //        string[] paramAckBillValues = model.STR_NA_BILL_ID.Split('/');

        //        String[] urlParams = URLEncrypt.DecryptParameters(new String[] { paramAckBillValues[0], paramAckBillValues[1], paramAckBillValues[2] });
        //        if (urlParams.Length >= 1)
        //        {

        //            model.NA_BILL_ID = Convert.ToInt64(urlParams[0]);
        //            if (model.NA_BILL_ID == null)
        //            {
        //                return Json(new { 
        //                    status=false                      
        //                });
        //            }

        //        }
        //    }

        //    string[] selectedIDs = model.SelectedIDArray.Split(',');

        //    long[] BillIdArray = new long[selectedIDs.Length];


        //    foreach (var item in selectedIDs)
        //    {
        //        string[] paramValues = item.Split('/');

        //        if (!String.IsNullOrEmpty(paramValues[0]) && !String.IsNullOrEmpty(paramValues[1]) && !String.IsNullOrEmpty(paramValues[2]))
        //        {
        //            String[] urlParams = URLEncrypt.DecryptParameters(new String[] { paramValues[0], paramValues[1], paramValues[2] });
        //            if (urlParams.Length >= 1)
        //            {

        //                bill_id = Convert.ToInt64(urlParams[0]);
        //                BillIdArray[i] = bill_id;
        //                i++;
        //            }
        //        }
        //        else
        //        {
        //            throw new Exception("Error while getting selected Cheque details");

        //        }

        //    }

        //    bool result = objDAL.UnauthrizeVoucher(BillIdArray,model);
        //    return Json(new { success = result,status=true });

        //}
    }
}
