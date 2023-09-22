#region HEADER
/*
* Project Id:

 * Project Name:OMMAS-II

 * File Name: ExecutionController.cs

 * Author : Vikram Nandanwar

 * Creation Date :19/June/2013

 * Desc : This class is used as controller  to perform Save,Edit,Update,Delete and listing of Execution  screens.  
 */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Models.Execution;
using PMGSY.Common;
using PMGSY.Extensions;
using PMGSY.BAL.Execution;
using PMGSY.Models;
using PMGSY.DAL.Execution;
using System.Data.Entity.Validation;
using PMGSY.Models.Common;
using System.IO;
using System.Configuration;
using System.Web.Script.Serialization;
using System.Text.RegularExpressions;
using ExifLib;
using System.Text;

namespace PMGSY.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class ExecutionController : Controller
    {

        public ExecutionController()
        {
            PMGSYSession.Current.ModuleName = "Execution";
        }

        PMGSYEntities dbContext = null;
        string message = String.Empty;

        #region Work/Payment Shedule Proposal List

        /// <summary>
        /// ListProposal() is used to display filter search and Work/Payment Shedule Grid
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult ListProposal()
        {
            //view model obj
            ProposalFilterViewModel proposalViewModel = new ProposalFilterViewModel();

            //common function object
            CommonFunctions objCommonFunctions = new CommonFunctions();
            try
            {
                //populate Blobk,Batch,Year,Stream,Proposal Type
                proposalViewModel.BLOCKS = objCommonFunctions.PopulateBlocks(PMGSYSession.Current.DistrictCode, true);
                List<SelectListItem> lstBatch = objCommonFunctions.PopulateBatch();
                TransactionParams transactionParams = new TransactionParams();
                transactionParams.STATE_CODE = PMGSYSession.Current.StateCode;
                transactionParams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                transactionParams.ISSearch = true;
                transactionParams.SANC_YEAR = (Int16)DateTime.Now.Year;
                List<SelectListItem> lstPackages = objCommonFunctions.PopulatePackage(transactionParams);
                lstPackages.RemoveAt(0);
                lstPackages.Insert(0, new SelectListItem { Value = "All", Text = "All Packages" });
                proposalViewModel.PACKAGES = lstPackages;
                lstBatch.RemoveAt(0);
                lstBatch.Insert(0, new SelectListItem() { Value = "-1", Text = "All Batch" });
                proposalViewModel.BATCHS = lstBatch;
                //proposalViewModel.BATCHS = objCommonFunctions.PopulateBatch();
                proposalViewModel.PROPOSAL_TYPES = objCommonFunctions.PopulateProposalTypes();
                proposalViewModel.STREAMS = objCommonFunctions.PopulateStreams("", true);
                proposalViewModel.IMS_YEAR = System.DateTime.Now.Year;
                proposalViewModel.Years = PopulateYear(0, true, true);

                //new fitlers added by Vikram 
                proposalViewModel.lstBatchs = objCommonFunctions.PopulateBatch(true);
                proposalViewModel.lstCollaborations = objCommonFunctions.PopulateFundingAgency(true);
                proposalViewModel.lstUpgradations = objCommonFunctions.PopulateNewUpgradeList(true);
                //end of change

                return View("ListProposal", proposalViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        /// <summary>
        /// GetProposalList() Action is used to show Proposal List on grid for Execution Work/Payment shedule
        /// </summary>
        /// <returns>
        /// Proposal list is filtered by Year,State and Batch,Stream, Proposal Type wise. and json data is return to jqGrid
        /// </returns>
        [Audit]
        public ActionResult GetProposalList(int? page, int? rows, string sidx, string sord)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }

            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            IExecutionBAL objExecutionBAL = new ExecutionBAL();

            ProposalFilterViewModel proposalfilterViewModel = new ProposalFilterViewModel();
            proposalfilterViewModel.IMS_YEAR = Convert.ToInt32(Request.Params["IMS_YEAR"]);
            proposalfilterViewModel.MAST_BLOCK_CODE = Convert.ToInt32(Request.Params["MAST_BLOCK_CODE"]);
            //proposalfilterViewModel.IMS_BATCH = Convert.ToInt32(Request.Params["IMS_BATCH"]);
            //proposalfilterViewModel.IMS_STREAMS = Convert.ToInt32(Request.Params["IMS_STREAM"]);
            proposalfilterViewModel.IMS_PACKAGE = Request.Params["IMS_PACKAGE"];
            proposalfilterViewModel.IMS_PROPOSAL_TYPE = Request.Params["IMS_PROPOSAL_TYPE"];
            proposalfilterViewModel.filterParameters = Request.Params["filters"];

            //new filters added by Vikram 
            if (!string.IsNullOrEmpty(Request.Params["batch"]))
            {
                proposalfilterViewModel.Batch = Convert.ToInt32(Request.Params["batch"].Trim());
            }

            if (!string.IsNullOrEmpty(Request.Params["collaboration"]))
            {
                proposalfilterViewModel.Collaboration = Convert.ToInt32(Request.Params["collaboration"].Trim());
            }

            if (!string.IsNullOrEmpty(Request.Params["upgradationType"]))
            {
                proposalfilterViewModel.UpgradeConnect = Request.Params["upgradationType"].Trim();
            }


            int totalRecords;
            var jsonData = new
            {
                rows = objExecutionBAL.GetProposalsBAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, proposalfilterViewModel),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords,
            };
            return Json(jsonData);
        }

        [Audit]
        public List<SelectListItem> PopulateYear(int SelectedYear = 0, bool populateFirstItem = true, bool isAllYearsSelected = false)
        {
            List<SelectListItem> lstYears = new List<SelectListItem>();
            SelectListItem item = new SelectListItem();
            if (populateFirstItem && isAllYearsSelected == false)
            {
                item.Text = "Select Year";
                item.Value = "0";
                item.Selected = true;
                lstYears.Add(item);
            }
            if (populateFirstItem && isAllYearsSelected)
            {
                item.Text = "All Years";
                item.Value = "-1";
                item.Selected = true;
                lstYears.Add(item);
            }
            for (int i = 2000; i < DateTime.Now.Year + 1; i++)
            {
                item = new SelectListItem();
                item.Text = i + " - " + (i + 1);
                item.Value = i.ToString();
                lstYears.Add(item);
            }

            return lstYears;
        }

        #endregion

        #region Work Program Acctions

        /// <summary>
        /// Display Work Program list and Add/Edit Form
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult WorkProgramAddEdit(String parameter, String hash, String key)
        {
            IExecutionDAL objExecutionDAL = new ExecutionDAL();
            IExecutionBAL objExecutionBAL = new ExecutionBAL();

            Dictionary<string, string> decryptedParameters = null;
            WorkProgramViewModel workProgramViewModel = new WorkProgramViewModel();

            try
            {
                dbContext = new PMGSYEntities();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                int IMS_PR_ROAD_CODE = Convert.ToInt32(decryptedParameters["IMS_PR_ROAD_CODE"]);
                if (decryptedParameters.Count() > 0)
                {
                    workProgramViewModel = objExecutionBAL.GetWorkProgramInformation(IMS_PR_ROAD_CODE);

                    if (workProgramViewModel.ProposalType == "P")
                    {
                        ViewData["MAST_HEAD_CODE"] = new SelectList(objExecutionDAL.PopulateHeadItemForRoad(IMS_PR_ROAD_CODE).AsEnumerable<SelectListItem>(), "Value", "Text");
                    }
                    else
                    {
                        ViewData["MAST_HEAD_CODE"] = new SelectList(objExecutionDAL.PopulateHeadItemForLSB(IMS_PR_ROAD_CODE).AsEnumerable<SelectListItem>(), "Value", "Text");
                    }
                    return PartialView("WorkProgramAddEdit", workProgramViewModel);
                }
                return PartialView("WorkProgramAddEdit", new WorkProgramViewModel());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "Work Program Details not present.");
                return PartialView("WorkProgramAddEdit", new WorkProgramViewModel());
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// AddWorkProgramDetails() Action is used to Save Work program Data into database
        /// </summary>
        /// <param name="WorkProgramViewModel"> AddWorkProgramDetails Contains All Entered Work program Form Data</param>
        /// <returns>If data is successfully Saved then return success message else return error message </returns>
        [HttpPost]
        [Audit]
        public ActionResult AddWorkProgramDetails(WorkProgramViewModel workProgramViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    IExecutionBAL objExecutionBAL = new ExecutionBAL();
                    if (objExecutionBAL.AddWorkProgramDetails(workProgramViewModel, ref message))
                    {
                        return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (DbEntityValidationException e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e, HttpContext.ApplicationInstance.Context);
                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        ModelState.AddModelError("", eve.ValidationErrors.ToString());
                        message = message + eve.ValidationErrors.ToString();
                    }
                }
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "An Error Occured While proccessing your request";
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// EditWorkProgramDetails() is used to Display Work Program Data entry Form
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult EditWorkProgramDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            dbContext = new PMGSYEntities();
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                int imsPrRoadCode = Convert.ToInt32(decryptedParameters["IMS_PR_ROAD_CODE"]);
                int headCode = Convert.ToInt32(decryptedParameters["MAST_HEAD_CODE"]);

                if (decryptedParameters.Count() > 0)
                {
                    IExecutionBAL objExecutionBAL = new ExecutionBAL();
                    WorkProgramViewModel workProgramViewModel = objExecutionBAL.GetWorkProgramDetails(imsPrRoadCode, headCode);
                    if (workProgramViewModel == null)
                    {
                        ModelState.AddModelError(String.Empty, "Work Program details not exist");
                        return PartialView("WorkProgramAddEdit", new WorkProgramViewModel());
                    }

                    //set Head Item Drop down
                    List<SelectListItem> HeadItem = new List<SelectListItem>();
                    SelectListItem item = new SelectListItem();
                    item.Text = dbContext.MASTER_EXECUTION_ITEM.Where(m => m.MAST_HEAD_CODE == workProgramViewModel.MAST_HEAD_CODE).Select(m => m.MAST_HEAD_DESC).FirstOrDefault();
                    item.Value = workProgramViewModel.MAST_HEAD_CODE.ToString();
                    HeadItem.Add(item);
                    ViewData["MAST_HEAD_CODE"] = new SelectList(HeadItem.AsEnumerable<SelectListItem>(), "Value", "Text", workProgramViewModel.MAST_HEAD_CODE);

                    return PartialView("WorkProgramAddEdit", workProgramViewModel);
                }
                return PartialView("WorkProgramAddEdit", new WorkProgramViewModel());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "Work Program details not Exist.");
                return PartialView("WorkProgramAddEdit", new WorkProgramViewModel());
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        /// <summary>
        /// EditWorkProgramDetails() Action is used to Update Work program Data into database
        /// </summary>
        /// <param name="WorkProgramViewModel"> workProgramViewModel Contains All Entered Work program Form Data</param>
        /// <returns>If data is successfully Saved then return success message else return error message </returns>
        [HttpPost]
        [Audit]
        public ActionResult EditWorkProgramDetails(WorkProgramViewModel workProgramViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    IExecutionBAL objExecutionBAL = new ExecutionBAL();
                    if (objExecutionBAL.EditWorkProgramDetails(workProgramViewModel, ref message))
                    {
                        return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (DbEntityValidationException e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e, HttpContext.ApplicationInstance.Context);
                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        ModelState.AddModelError("", eve.ValidationErrors.ToString());
                        message = message + eve.ValidationErrors.ToString();
                    }
                }
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "An Error Occured While proccessing your request";
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        ///DeleteWorkProgramDetails() this Action is used to delete Perticular Work Program Details From database  
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns>this function return success message if Work Program Details successfully Deleted else shows error message</returns>
        [HttpPost]
        [Audit]
        public ActionResult DeleteWorkProgramDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                int imsPrRoadCode = Convert.ToInt32(decryptedParameters["IMS_PR_ROAD_CODE"]);
                int headCode = Convert.ToInt32(decryptedParameters["MAST_HEAD_CODE"]);

                if (decryptedParameters.Count() > 0)
                {
                    IExecutionBAL objExecutionBAL = new ExecutionBAL();

                    if (objExecutionBAL.DeleteWorkProgramDetails(imsPrRoadCode, headCode, ref message))
                    {
                        return Json(new { success = true, message = "Work Program details deleted successfully." }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Work Program details can not be deleted because other details for work details are entered." }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { success = false, message = "There is an error occured while processing your request." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "There is an error occured while processing your request." }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// GetWorkProgramList() this Action is used to Display Work Program Details On Grid
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns>Return json data for Work Program Jqgrid</returns>
        public JsonResult GetWorkProgramList(int? page, int? rows, string sidx, string sord)
        {
            int IMS_PR_ROAD_CODE = Convert.ToInt32(Request["IMS_PR_ROAD_CODE"]);
            int totalRecords;
            ExecutionBAL objExecutionBAL = new ExecutionBAL();

            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }

            var jsonData = new
            {
                rows = objExecutionBAL.GetWorkProgramList(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, IMS_PR_ROAD_CODE),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };
            return Json(jsonData);
        }

        /// <summary>
        /// PopulateHeadItemForRoad() this action is used to Populate Head Item for Road to update head item through Ajax Call
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult PopulateHeadItemForRoad()
        {
            IExecutionDAL objExecutionDAL = new ExecutionDAL();
            int IMS_PR_ROAD_CODE = Convert.ToInt32(Request.Params["IMS_PR_ROAD_CODE"]);
            return Json(objExecutionDAL.PopulateHeadItemForRoad(IMS_PR_ROAD_CODE), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// PopulateHeadItemForLSB() this action is used to Populate Head Item for LSB to update head item through Ajax Call
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult PopulateHeadItemForLSB()
        {
            IExecutionDAL objExecutionDAL = new ExecutionDAL();
            int IMS_PR_ROAD_CODE = Convert.ToInt32(Request.Params["IMS_PR_ROAD_CODE"]);
            return Json(objExecutionDAL.PopulateHeadItemForLSB(IMS_PR_ROAD_CODE), JsonRequestBehavior.AllowGet);
        }

        #endregion Work Program Actions


        #region Payment Schedule Actions
        /// <summary>
        /// PaymentScheduleAddEdit() Action is used to Display Payment Shedule details on grid and Add/Edit Form
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult PaymentScheduleAddEdit(String parameter, String hash, String key)
        {
            IExecutionDAL objExecutionDAL = new ExecutionDAL();
            Dictionary<string, string> decryptedParameters = null;

            PaymentScheduleViewModel paymentScheduleViewModel = new PaymentScheduleViewModel();


            try
            {
                dbContext = new PMGSYEntities();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                int IMS_PR_ROAD_CODE = Convert.ToInt32(decryptedParameters["IMS_PR_ROAD_CODE"]);
                if (decryptedParameters.Count() > 0)
                {
                    CommonFunctions objCommonFunction = new CommonFunctions();

                    IExecutionDAL objExeptionDAL = new ExecutionDAL();
                    paymentScheduleViewModel = objExecutionDAL.getInformation(IMS_PR_ROAD_CODE);
                    paymentScheduleViewModel.Operation = "A";

                    if (paymentScheduleViewModel == null)
                    {
                        ModelState.AddModelError(string.Empty, "Payment Schedule Details not present.");
                        return PartialView("PaymentScheduleAddEdit", new PaymentScheduleViewModel());
                    }

                    //set here ddl
                    ViewData["EXEC_MPS_MONTH"] = new SelectList(objCommonFunction.PopulateMonths().AsEnumerable<SelectListItem>(), "Value", "Text");

                    //ViewData["EXEC_MPS_YEAR"] = new SelectList(objExecutionDAL.GetYears(IMS_PR_ROAD_CODE).AsEnumerable<SelectListItem>(), "Value", "Text");
                    ViewData["EXEC_MPS_YEAR"] = objCommonFunction.PopulateYears(true);
                    return PartialView("PaymentScheduleAddEdit", paymentScheduleViewModel);
                }
                return PartialView("PaymentScheduleAddEdit", new PaymentScheduleViewModel());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "Payment Schedule Details not present.");
                return PartialView("PaymentScheduleAddEdit", new PaymentScheduleViewModel());
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// AddPaymentScheduleDetails() Action is used to Save Payment Schedule Data into database
        /// </summary>
        /// <param name="paymentScheduleViewModel"> paymentScheduleViewModel Contains All Entered Payment Schedule Form Data</param>
        /// <returns>If data is successfully Saved then return success message else return error message </returns>
        [HttpPost]
        [Audit]
        public ActionResult AddPaymentScheduleDetails(PaymentScheduleViewModel paymentScheduleViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    IExecutionBAL objExecutionBAL = new ExecutionBAL();
                    if (objExecutionBAL.AddPaymentScheduleDetails(paymentScheduleViewModel, ref message))
                    {
                        return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (DbEntityValidationException e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e, HttpContext.ApplicationInstance.Context);
                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        ModelState.AddModelError("", eve.ValidationErrors.ToString());
                        message = message + eve.ValidationErrors.ToString();
                    }
                }
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "An Error Occured While proccessing your request";
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// EditPaymentScheduleDetails() is used to Display Payment Shedule Form in Edit Mode
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult EditPaymentScheduleDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            dbContext = new PMGSYEntities();
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                int imsPrRoadCode = Convert.ToInt32(decryptedParameters["IMS_PR_ROAD_CODE"]);
                int month = Convert.ToInt32(decryptedParameters["MONTH"]);
                int year = Convert.ToInt32(decryptedParameters["YEAR"]);


                if (decryptedParameters.Count() > 0)
                {
                    IExecutionBAL objExecutionBAL = new ExecutionBAL();
                    //ViewData["EXEC_MPS_YEAR"] = new CommonFunctions().PopulateYears(true);
                    //ViewData["EXEC_MPS_MONTH"] = new CommonFunctions().PopulateMonths();
                    PaymentScheduleViewModel paymentScheduleViewModel = objExecutionBAL.GetPaymentScheduleDetails(imsPrRoadCode, month, year);
                    if (paymentScheduleViewModel == null)
                    {
                        ModelState.AddModelError(String.Empty, "Payment Schedule details not exist");
                        return PartialView("PaymentScheduleAddEdit", new PaymentScheduleViewModel());
                    }

                    //set Month Drop down                    
                    List<SelectListItem> itemMonth = new List<SelectListItem>();
                    SelectListItem item = new SelectListItem();
                    item.Text = dbContext.MASTER_MONTH.Where(m => m.MAST_MONTH_CODE == paymentScheduleViewModel.EXEC_MPS_MONTH).Select(s => s.MAST_MONTH_FULL_NAME).SingleOrDefault();
                    item.Value = paymentScheduleViewModel.EXEC_MPS_MONTH.ToString();
                    itemMonth.Add(item);
                    ViewData["EXEC_MPS_MONTH"] = new SelectList(itemMonth.AsEnumerable<SelectListItem>(), "Value", "Text", paymentScheduleViewModel.EXEC_MPS_MONTH);

                    //set Year Drop down                    
                    List<SelectListItem> itemYear = new List<SelectListItem>();
                    SelectListItem itemYr = new SelectListItem();
                    itemYr.Text = paymentScheduleViewModel.EXEC_MPS_YEAR.ToString(); //+ "-" + (paymentScheduleViewModel.EXEC_MPS_YEAR + 1);
                    itemYr.Value = paymentScheduleViewModel.EXEC_MPS_YEAR.ToString();
                    itemYear.Add(itemYr);
                    ViewData["EXEC_MPS_YEAR"] = new SelectList(itemYear.AsEnumerable<SelectListItem>(), "Value", "Text", paymentScheduleViewModel.EXEC_MPS_YEAR);

                    return PartialView("PaymentScheduleAddEdit", paymentScheduleViewModel);
                }
                return PartialView("PaymentScheduleAddEdit", new PaymentScheduleViewModel());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "Payment Schedule details not Exist.");
                return PartialView("PaymentScheduleAddEdit", new PaymentScheduleViewModel());
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        /// <summary>
        /// EditPaymentScheduleDetails() Action is used to Update Payment Schedule Data into database
        /// </summary>
        /// <param name="paymentScheduleViewModel"> paymentScheduleViewModel Contains All Entered Payment Schedule Form Data</param>
        /// <returns>If data is successfully Saved then return success message else return error message </returns>
        [HttpPost]
        [Audit]
        public ActionResult EditPaymentScheduleDetails(PaymentScheduleViewModel paymentScheduleViewModel)
        {
            //enc logic
            try
            {
                if (ModelState.IsValid)
                {
                    IExecutionBAL objExecutionBAL = new ExecutionBAL();
                    if (objExecutionBAL.EditPaymentScheduleDetails(paymentScheduleViewModel, ref message))
                    {
                        return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (DbEntityValidationException e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e, HttpContext.ApplicationInstance.Context);
                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        ModelState.AddModelError("", eve.ValidationErrors.ToString());
                        message = message + eve.ValidationErrors.ToString();
                    }
                }
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "An Error Occured While proccessing your request";
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        ///DeletePaymentScheduleDetails() this Action is used to delete Perticular Payment Schedule Details From database  
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns>this function return success message if Payment Schedule Details successfully Deleted else shows error message</returns>
        [HttpPost]
        [Audit]
        public ActionResult DeletePaymentScheduleDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                int imsPrRoadCode = Convert.ToInt32(decryptedParameters["IMS_PR_ROAD_CODE"]);
                int month = Convert.ToInt32(decryptedParameters["MONTH"]);
                int year = Convert.ToInt32(decryptedParameters["YEAR"]);

                if (decryptedParameters.Count() > 0)
                {
                    IExecutionBAL objExecutionBAL = new ExecutionBAL();

                    if (objExecutionBAL.DeletePaymentScheduleDetails(imsPrRoadCode, month, year, ref message))
                    {
                        return Json(new { success = true, message = "Payment Schedule details deleted successfully." }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Payment Schedule details can not be deleted because other details forPayment Schedule are entered." }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { success = false, message = "There is an error occured while processing your request." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "There is an error occured while processing your request." }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// GetPaymentScheduleList() this Action is used to Display Payment Schedule Details On Grid
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns>Return json data for Payment Shedule Jqgrid</returns>
        [Audit]
        public JsonResult GetPaymentScheduleList(int? page, int? rows, string sidx, string sord)
        {
            int IMS_PR_ROAD_CODE = Convert.ToInt32(Request["IMS_PR_ROAD_CODE"]);
            int totalRecords;
            ExecutionBAL objExecutionBAL = new ExecutionBAL();

            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }

            var jsonData = new
            {
                rows = objExecutionBAL.GetPaymentScheduleList(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, IMS_PR_ROAD_CODE),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };
            return Json(jsonData);
        }

        /// <summary>
        /// PopulateMonth() this action is used to Populate month using Ajax Call
        /// </summary>
        /// <returns>Return Month List</returns>
        [HttpPost]
        [Audit]
        public JsonResult PopulateMonth()
        {
            try
            {
                CommonFunctions objCommonFunction = new CommonFunctions();
                return Json(objCommonFunction.PopulateMonths());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        /// <summary>
        /// PopulateYear() this action is used to Populate year using Ajax Call
        /// </summary>
        /// <returns>Return Year List</returns>
        [HttpPost]
        [Audit]
        public JsonResult PopulateYear()
        {
            try
            {
                CommonFunctions objCommonFunction = new CommonFunctions();
                IExecutionDAL objExecutionDAL = new ExecutionDAL();
                int imsPrRoadCode = Convert.ToInt32(Request.Params["IMS_PR_ROAD_CODE"]);
                return Json(objCommonFunction.PopulateYears(true));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }


        #endregion Payment Schedule Actions

        #region PHYSICAL_ROAD

        /// <summary>
        /// returns the list view of execution progress details
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult ListExecutionProgress()
        {
            try
            {
                if (PMGSYSession.Current.RoleCode == 36 || PMGSYSession.Current.RoleCode == 47)
                {
                    CommonFunctions objCommon = new CommonFunctions();
                    ProposalFilterForITNOViewModel proposalModel = new ProposalFilterForITNOViewModel();
                    List<SelectListItem> lstBatches = new List<SelectListItem>();
                    TransactionParams transactionParams = new TransactionParams();
                    transactionParams.STATE_CODE = PMGSYSession.Current.StateCode;
                    transactionParams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                    transactionParams.ISSearch = true;
                    transactionParams.SANC_YEAR = (Int16)DateTime.Now.Year;
                    lstBatches = objCommon.PopulateBatch();
                    lstBatches.RemoveAt(0);
                    lstBatches.Add(new SelectListItem { Value = "0", Text = "All Batches" });
                    proposalModel.BATCHS = lstBatches;
                    proposalModel.lstDistricts = objCommon.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                    proposalModel.BLOCKS = objCommon.PopulateBlocks(PMGSYSession.Current.DistrictCode, true);
                    proposalModel.Years = PopulateYear(0, true, true);
                    proposalModel.STREAMS = objCommon.PopulateStreams("", true);
                    proposalModel.PROPOSAL_TYPES = objCommon.PopulateProposalTypes();
                    proposalModel.lstBatchs = objCommon.PopulateBatch(true);
                    proposalModel.lstCollaborations = objCommon.PopulateFundingAgency(true);
                    proposalModel.lstUpgradations = objCommon.PopulateNewUpgradeList(true);
                    List<SelectListItem> lstPackages = objCommon.PopulatePackage(transactionParams);
                    lstPackages.RemoveAt(0);
                    lstPackages.Insert(0, new SelectListItem { Value = "All", Text = "All Packages" });
                    proposalModel.PACKAGES = lstPackages;
                    return View("ListProposalsForITNO", proposalModel);
                }
                else
                {
                    CommonFunctions objCommon = new CommonFunctions();
                    ProposalFilterViewModel proposalModel = new ProposalFilterViewModel();
                    List<SelectListItem> lstBatches = new List<SelectListItem>();
                    TransactionParams transactionParams = new TransactionParams();
                    transactionParams.STATE_CODE = PMGSYSession.Current.StateCode;
                    transactionParams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                    transactionParams.ISSearch = true;
                    transactionParams.SANC_YEAR = (Int16)DateTime.Now.Year;
                    lstBatches = objCommon.PopulateBatch();
                    lstBatches.RemoveAt(0);
                    lstBatches.Add(new SelectListItem { Value = "0", Text = "All Batches" });
                    proposalModel.BATCHS = lstBatches;
                    proposalModel.BLOCKS = objCommon.PopulateBlocks(PMGSYSession.Current.DistrictCode, true);
                    proposalModel.Years = PopulateYear(0, true, true);
                    proposalModel.STREAMS = objCommon.PopulateStreams("", true);
                    proposalModel.PROPOSAL_TYPES = objCommon.PopulateProposalTypes();

                    //new filters added by Vikram 
                    proposalModel.lstBatchs = objCommon.PopulateBatch(true);
                    proposalModel.lstCollaborations = objCommon.PopulateFundingAgency(true);
                    proposalModel.lstUpgradations = objCommon.PopulateNewUpgradeList(true);
                    //end of change

                    List<SelectListItem> lstPackages = objCommon.PopulatePackage(transactionParams);
                    lstPackages.RemoveAt(0);
                    lstPackages.Insert(0, new SelectListItem { Value = "All", Text = "All Packages" });
                    proposalModel.PACKAGES = lstPackages;
                    return View("ListExecutionProgress", proposalModel);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ListExecutionProgress()");
                return null;
            }
        }

        /// <summary>
        /// returns the list of execution progress details 
        /// </summary>
        /// <param name="executionCollection">contains the filters and grid parameters</param>
        /// <returns></returns>
        [Audit]
        public ActionResult GetExecutionProgressList(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                int yearCode = 0;
                int blockCode = 0;
                int streamCode = 0;
                int batchCode = 0;
                string proposalCode = string.Empty;
                string packageCode = string.Empty;
                string upgradationType = string.Empty;
                long totalRecords = 0;
                IExecutionBAL objBAL = new ExecutionBAL();

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["yearCode"]))
                {
                    yearCode = Convert.ToInt32(Request.Params["yearCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["blockCode"]))
                {
                    blockCode = Convert.ToInt32(Request.Params["blockCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["streamCode"]))
                {
                    streamCode = Convert.ToInt32(Request.Params["streamCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["batchCode"]))
                {
                    batchCode = Convert.ToInt32(Request.Params["batchCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["proposalCode"]))
                {
                    proposalCode = Request.Params["proposalCode"];
                }

                if (!string.IsNullOrEmpty(Request.Params["packageCode"]))
                {
                    packageCode = Request.Params["packageCode"];
                }

                if (!string.IsNullOrEmpty(Request.Params["upgradationType"]))
                {
                    upgradationType = Request.Params["upgradationType"];
                }

                var jsonData = new
                {
                    rows = objBAL.GetExecutionList(yearCode, blockCode, packageCode, proposalCode, upgradationType, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetExecutionProgressList()");
                return null;
            }
        }

        /// <summary>
        /// returns Physiacal Progress of road 
        /// </summary>
        /// <param name="urlparameter">encrypted proposal code</param>
        /// <returns></returns>
        [Audit]
        public ActionResult ListPhysicalDetails(string urlparameter)
        {
            int proposalCode = 0;

            String[] encryptedParameters = null;
            Dictionary<string, string> decryptedParameters = null;
            ExecutionDAL objDAL = new ExecutionDAL();
            try
            {
                if (urlparameter != string.Empty)
                {
                    encryptedParameters = urlparameter.Split('/');
                    if (encryptedParameters.Length == 3)
                    {
                        decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    }
                }

                proposalCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);

                ExecutionRoadStatusViewModel roadModel = objDAL.GetRoadDetails(proposalCode);
                if (roadModel == null)
                {
                    roadModel = new ExecutionRoadStatusViewModel();
                }

                roadModel.IMS_PR_ROAD_CODE = proposalCode;
                return View("ListPhysicalDetails", roadModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ListPhysicalDetails()");
                return null;
            }
        }

        /// <summary>
        /// returns the list of physical road details
        /// </summary>
        /// <param name="physicalRoadCollection">formcollection containing grid parameters</param>
        /// <returns></returns>
        [Audit]
        public ActionResult GetRoadPhysicalProgressList(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                IExecutionBAL objBAL = new ExecutionBAL();
                long totalRecords = 0;
                int imsRoadCode = 0;

                if (!(string.IsNullOrEmpty(Request.Params["roadCode"])))
                {
                    imsRoadCode = Convert.ToInt32(Request.Params["roadCode"]);
                }

                var jsonData = new
                {
                    rows = objBAL.GetRoadPhysicalProgressList(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, imsRoadCode),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetRoadPhysicalProgressList()");
                return null;
            }
        }

        /// <summary>
        /// returns add view of physical road progress 
        /// </summary>
        /// <param name="id">encrypted id key</param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult AddPhysicalRoadProgress(string id)
        {
            try
            {
                int proposalCode = Convert.ToInt32(id);
                ExecutionDAL objDAL = new ExecutionDAL();
                ExecutionRoadStatusViewModel roadModel = new ExecutionRoadStatusViewModel();
                roadModel = objDAL.GetRoadDetails(proposalCode);
                if (roadModel == null)
                {
                    roadModel = new ExecutionRoadStatusViewModel();
                }
                CommonFunctions objCommon = new CommonFunctions();
                roadModel.IMS_PR_ROAD_CODE = proposalCode;
                roadModel.Operation = "A";
                //ViewData["Year"] = objCommon.PopulateYears(true);
                ViewData["Month"] = objCommon.PopulateMonthsforCurrentFinancialYear(true);//objCommon.PopulateMonths();

                #region Populate Month and Year
                //DateTime currDate = DateTime.Now;
                //roadModel.crYear = DateTime.Now.Year;

                //List<SelectListItem> lstMonth = new List<SelectListItem>();
                //List<SelectListItem> lstYear = new List<SelectListItem>();

                //if (currDate.Month == 1 && currDate.Day <= 14)
                //{
                //    lstYear.Insert(0, (new SelectListItem { Text = (currDate.Year - 1).ToString(), Value = (currDate.Year - 1).ToString() }));
                //    lstYear.Insert(0, (new SelectListItem { Text = (currDate.Year).ToString(), Value = (currDate.Year).ToString(), Selected = true }));
                //}
                //else
                //{
                //    lstYear.Insert(0, (new SelectListItem { Text = (currDate.Year).ToString(), Value = (currDate.Year).ToString(), Selected = true }));
                //}
                //if (currDate.Day <= 14)
                //{
                //    lstMonth.Insert(0, (new SelectListItem { Text = (currDate.AddMonths(-1)).ToString("MMMM"), Value = currDate.AddMonths(-1).Month.ToString() }));
                //    lstMonth.Insert(0, (new SelectListItem { Text = currDate.ToString("MMMM"), Value = currDate.Month.ToString(), Selected = true }));
                //}
                //else
                //{
                //    lstMonth.Insert(0, (new SelectListItem { Text = currDate.ToString("MMMM"), Value = currDate.Month.ToString(), Selected = true }));
                //}
                //ViewData["Year"] = lstYear;
                //ViewData["Month"] = lstMonth;

                //roadModel.currmonthName = currDate.ToString("MMMM");
                //roadModel.prevmonthName = currDate.AddMonths(-1).ToString("MMMM");
                #endregion

                /*List<SelectListItem> lstYear = objCommon.PopulateYears(true);
                int count = lstYear.IndexOf(lstYear.Find(c => c.Value == "2014"));
                lstYear.RemoveRange(lstYear.IndexOf(lstYear.Find(c => c.Value == "2014")), lstYear.Count - count);
                */
                List<SelectListItem> lstYear = new List<SelectListItem>();
                #region Old Logic
                //if (DateTime.Now.Month <= 3)
                //{
                //    lstYear.Insert(0, new SelectListItem { Text = (DateTime.Now.Year - 1).ToString(), Value = (DateTime.Now.Year - 1).ToString() });
                //}
                //else
                //{
                //    lstYear.Insert(0, new SelectListItem { Text = DateTime.Now.Year.ToString(), Value = DateTime.Now.Year.ToString() });
                //}
                #endregion
                if (DateTime.Now.Day <= 10 && DateTime.Now.Month == 1)
                {
                    lstYear.Insert(0, new SelectListItem { Text = (DateTime.Now.Year - 1).ToString(), Value = (DateTime.Now.Year - 1).ToString() });
                    lstYear.Insert(1, new SelectListItem { Text = (DateTime.Now.Year).ToString(), Value = (DateTime.Now.Year).ToString() });
                }
                else
                {
                    lstYear.Insert(0, new SelectListItem { Text = (DateTime.Now.Year).ToString(), Value = (DateTime.Now.Year).ToString() });
                }
                ViewData["Year"] = lstYear;
                ViewData["WorkStatus"] = objDAL.GetWorkStatus();

                return PartialView("AddPhysicalRoadProgress", roadModel);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "AddPhysicalRoadDetails(string id)");
                return Json(new { success = false, message = "Error Occurred while processing your request." });
            }
        }

        /// <summary>
        /// save the Physical Road Status Details
        /// </summary>
        /// <param name="progressModel">contains the form data</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Audit]
        public ActionResult AddPhysicalRoadDetails(ExecutionRoadStatusViewModel progressModel)
        {
            string message = string.Empty;
            IExecutionBAL objBAL = new ExecutionBAL();

            string AprilMonthDay = ConfigurationManager.AppSettings["PHYSICAL_ROAD_PROGRESS_APRIL_MONTH_DAY"];   //10
            int AprilMonthDayValue = Convert.ToInt16(AprilMonthDay);

            string AprilMonth = ConfigurationManager.AppSettings["PHYSICAL_ROAD_PROGRESS_APRIL_MONTH"];  //4
            int AprilMonthValue = Convert.ToInt16(AprilMonth);

            int allowedMonth = (DateTime.Now.Month == AprilMonthValue && DateTime.Now.Day <= AprilMonthDayValue) ? (DateTime.Now.Month - 1) : DateTime.Now.Month;
            int allowedYear = DateTime.Now.Year;

            try
            {
                if (ModelState.IsValid)
                {
                    #region Old Logic
                    //int allowedMonth = DateTime.Now.Month == 1 ? (10) : (DateTime.Now.Month == 2 ? 11 : (DateTime.Now.Month - 3));
                    //int allowedYear = DateTime.Now.Month == 1 ? (DateTime.Now.Year - 1) : (DateTime.Now.Month == 2 ? (DateTime.Now.Year - 1) : (DateTime.Now.Year));

                    //if (progressModel.EXEC_PROG_MONTH < allowedMonth || progressModel.EXEC_PROG_YEAR < allowedYear)
                    //{
                    //    return Json(new { success = false, message = "Month and Year must be equal to current month and year or three months less than current date." }, JsonRequestBehavior.AllowGet);
                    //}
                    #endregion


                    //if (progressModel.EXEC_PROG_MONTH > allowedMonth && progressModel.EXEC_PROG_YEAR <= allowedYear)
                    //{
                    //    return Json(new { success = false, message = "Month and Year must be equal to current month and year or last month." }, JsonRequestBehavior.AllowGet);
                    //}

                    if (DateTime.Now.Month == AprilMonthValue)
                    {
                        if (DateTime.Now.Day <= AprilMonthDayValue)
                        {
                            if (!((progressModel.EXEC_PROG_MONTH == allowedMonth) && progressModel.EXEC_PROG_YEAR == allowedYear))
                            {
                                return Json(new { success = false, message = "Progress Month and Year must be equal to March and " + DateTime.Now.Year }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            if (!((progressModel.EXEC_PROG_MONTH == allowedMonth) && progressModel.EXEC_PROG_YEAR == allowedYear))
                            {
                                return Json(new { success = false, message = "Progress Month and Year must be equal to April and " + DateTime.Now.Year }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                    else if (DateTime.Now.Day <= 5)
                    {
                        if (DateTime.Now.Month == 1)
                        {
                            if (!((progressModel.EXEC_PROG_MONTH == 12 && progressModel.EXEC_PROG_YEAR == (allowedYear - 1)) || ((progressModel.EXEC_PROG_MONTH == allowedMonth) && (progressModel.EXEC_PROG_YEAR == allowedYear)))
                               )
                            {
                                return Json(new { success = false, message = "Progress Month and Year must be equal to current month and year." }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            if (!(((progressModel.EXEC_PROG_MONTH == allowedMonth) || (progressModel.EXEC_PROG_MONTH == (allowedMonth - 1)))
                                   && progressModel.EXEC_PROG_YEAR == allowedYear)
                               )
                            {
                                return Json(new { success = false, message = "Progress Month and Year must be equal to current month and year." }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                    else
                    {
                        if (!(progressModel.EXEC_PROG_MONTH == allowedMonth && progressModel.EXEC_PROG_YEAR == allowedYear))
                        {
                            return Json(new { success = false, message = "Progress Month and Year must be equal to current month and year." }, JsonRequestBehavior.AllowGet);
                        }
                    }

                    if (!(objBAL.AddPhysicalProgressDetails(progressModel, ref message)))
                    {
                        if (message != string.Empty)
                        {
                            return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { success = false, message = "Physical Road Progress Details Not Added Successfully" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "AddPhysicalRoadDetails(ExecutionRoadStatusViewModel progressModel)");
                return Json(new { success = false, message = "Error occurred while processing the request." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Audit]
        public JsonResult GetRoadWeeklyDetails()
        {

            ExecutionDAL objDAL = new ExecutionDAL();
            ExecutionRoadStatusViewModel roadModel = new ExecutionRoadStatusViewModel();
            try
            {
                roadModel.IMS_PR_ROAD_CODE = Convert.ToInt32(Request.Params["roadCode"]);
                roadModel.Year = Convert.ToInt32(Request.Params["year"]);
                roadModel.AgreementMonth = Convert.ToInt32(Request.Params["month"]);

                objDAL.GetPhysicalRoadWeeklyDetails(ref roadModel);
                return Json(roadModel, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "GetRoadWeeklyDetails()");
                return null;
            }
        }

        /// <summary>
        /// return the details for updation
        /// </summary>
        /// <param name="hash">encrypted key</param>
        /// <param name="parameter">encrypted key</param>
        /// <param name="key">encrypted key</param>
        /// <returns></returns>
        [Audit]
        public ActionResult EditPhysicalRoadProgress(String hash, String parameter, String key)
        {
            try
            {
                Dictionary<string, string> decryptedParameters = null;
                CommonFunctions objCommon = new CommonFunctions();
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                IExecutionBAL objBAL = new ExecutionBAL();
                ExecutionDAL objDAL = new ExecutionDAL();
                int proposalCode = 0;   //for storing decrypted proposal code
                int monthCode = 0;  //for storing decrypted month code
                int yearCode = 0;   //for storing decrypted year code
                if (decryptedParameters != null)
                {
                    proposalCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);
                    monthCode = Convert.ToInt32(decryptedParameters["Month"]);
                    yearCode = Convert.ToInt32(decryptedParameters["Year"]);
                }
                ViewData["Year"] = objCommon.PopulateYears(true);
                ViewData["Month"] = objCommon.PopulateMonths();
                ViewData["WorkStatus"] = objDAL.GetWorkStatus();
                ExecutionRoadStatusViewModel model = objBAL.GetPhysicalRoadDetails(proposalCode, monthCode, yearCode);
                model.Operation = "E";
                if (model != null)
                {
                    return PartialView("AddPhysicalRoadProgress", model);
                }
                return PartialView("AddPhysicalRoadProgress", new ExecutionRoadStatusViewModel());
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EditPhysicalRoadProgress(String hash, String parameter, String key)");
                return null;
            }
        }

        /// <summary>
        /// updates the physical road details
        /// </summary>
        /// <param name="progressModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Audit]
        public ActionResult EditPhysicalRoadDetails(ExecutionRoadStatusViewModel progressModel)
        {
            IExecutionBAL objBAL = new ExecutionBAL();
            string message = string.Empty;

            string AprilMonthDay = ConfigurationManager.AppSettings["PHYSICAL_ROAD_PROGRESS_APRIL_MONTH_DAY"];   //10
            int AprilMonthDayValue = Convert.ToInt16(AprilMonthDay);

            string AprilMonth = ConfigurationManager.AppSettings["PHYSICAL_ROAD_PROGRESS_APRIL_MONTH"];  //4
            int AprilMonthValue = Convert.ToInt16(AprilMonth);

            int allowedMonth = (DateTime.Now.Month == AprilMonthValue && DateTime.Now.Day <= AprilMonthDayValue) ? (DateTime.Now.Month - 1) : DateTime.Now.Month;
            int allowedYear = DateTime.Now.Year;
            try
            {
                //if (!(progressModel.EXEC_PROG_MONTH == allowedMonth && progressModel.EXEC_PROG_YEAR == allowedYear))
                //{
                //    return Json(new { success = false, message = "Month and Year must be equal to current month and year." }, JsonRequestBehavior.AllowGet);
                //}




                if (DateTime.Now.Month == AprilMonthValue)
                {
                    if (DateTime.Now.Day <= AprilMonthDayValue)
                    {
                        if (!((progressModel.EXEC_PROG_MONTH == allowedMonth) && progressModel.EXEC_PROG_YEAR == allowedYear))
                        {
                            return Json(new { success = false, message = "Progress Month and Year must be equal to March and " + DateTime.Now.Year }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        if (!((progressModel.EXEC_PROG_MONTH == allowedMonth) && progressModel.EXEC_PROG_YEAR == allowedYear))
                        {
                            return Json(new { success = false, message = "Progress Month and Year must be equal to April and " + DateTime.Now.Year }, JsonRequestBehavior.AllowGet);
                        }
                    }


                }

                else if (DateTime.Now.Day <= 5)
                {


                    if (DateTime.Now.Month == 1)
                    {
                        if (!((progressModel.EXEC_PROG_MONTH == 12 && progressModel.EXEC_PROG_YEAR == (allowedYear - 1)) || ((progressModel.EXEC_PROG_MONTH == allowedMonth) && (progressModel.EXEC_PROG_YEAR == allowedYear)))
                           )
                        {
                            return Json(new { success = false, message = "Progress Month and Year must be equal to current month and year." }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        if (!(((progressModel.EXEC_PROG_MONTH == allowedMonth) || (progressModel.EXEC_PROG_MONTH == (allowedMonth - 1)))
                               && progressModel.EXEC_PROG_YEAR == allowedYear)
                           )
                        {
                            return Json(new { success = false, message = "Progress Month and Year must be equal to current month and year." }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                else
                {
                    if (!(progressModel.EXEC_PROG_MONTH == allowedMonth && progressModel.EXEC_PROG_YEAR == allowedYear))
                    {
                        return Json(new { success = false, message = "Progress Month and Year must be equal to current month and year." }, JsonRequestBehavior.AllowGet);
                    }
                }

                if (ModelState.IsValid)
                {
                    if (!objBAL.EditPhysicalRoadDetails(progressModel, ref message))
                    {
                        return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "EditPhysicalRoadProgress(ExecutionRoadStatusViewModel progressModel)");
                return null;
            }
        }

        /// <summary>
        /// for performing delete operation on physical road details 
        /// </summary>
        /// <param name="parameter">encrypted id key</param>
        /// <param name="hash">encrypted id key</param>
        /// <param name="key">encrypted id key</param>
        /// <returns>returns json along with status</returns>
        [HttpPost]
        [Audit]
        public ActionResult DeletePhysicalRoadDetails(String parameter, String hash, String key)
        {
            IExecutionBAL objBAL = new ExecutionBAL();
            string message = string.Empty;
            Dictionary<string, string> decryptedParameters = null;
            int proposalCode = 0;
            int monthCode = 0;
            int yearCode = 0;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                proposalCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);
                yearCode = Convert.ToInt32(decryptedParameters["Year"]);
                monthCode = Convert.ToInt32(decryptedParameters["Month"]);
                if ((objBAL.DeletePhysicalRoadDetails(proposalCode, yearCode, monthCode, ref message)))
                {
                    if (message == string.Empty)
                    {
                        return Json(new { success = true, message = "Physical road progress details deleted successfully." });
                    }
                    else
                    {
                        return Json(new { success = true, message = message });
                    }
                }
                else
                {
                    return Json(new { success = false, message = message });
                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "DeletePhysicalRoadDetails()");
                return Json(new { success = false, message = "Error Occurred while processing your request." });
            }
        }

        public ActionResult CheckAgreementStatus(int proposalCode)
        {
            ExecutionDAL objDAL = new ExecutionDAL();
            try
            {
                bool status = objDAL.CheckAgreementStatus(proposalCode, ref message);
                return Json(new { success = status, message = message });
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "CheckAgreementStatus()");
                return Json(new { success = false, message = "Request can not be processed at this time.Please try after some time." });
            }
        }

        [HttpPost]
        [Audit]
        public JsonResult GetPackagesByYearandBlock(string sanctionYear, string blockCode)
        {
            try
            {
                TransactionParams transactionParams = new TransactionParams();

                transactionParams.STATE_CODE = PMGSYSession.Current.StateCode;
                transactionParams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                transactionParams.ISSearch = true;
                transactionParams.SANC_YEAR = Convert.ToInt16(sanctionYear.Trim());
                transactionParams.BlockCode = Convert.ToInt16(blockCode.Trim());
                List<SelectListItem> lstPackages = (new SelectList(new CommonFunctions().GetPackages(Convert.ToInt32(sanctionYear), Convert.ToInt32(blockCode), true).ToList(), "IMS_PACKAGE_ID", "IMS_PACKAGE_ID")).ToList<SelectListItem>();
                return Json(lstPackages);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "GetPackagesByYearandBlock()");
                return Json(false);
            }
        }

        [Audit]
        public ActionResult CheckSanctionCost(string id)
        {
            ExecutionBAL objBAL = new ExecutionBAL();
            try
            {
                string[] parameters = id.Split(',');
                int proposalCode = Convert.ToInt32(parameters[0]);
                decimal valueofWork = Convert.ToDecimal(parameters[1]);
                decimal valueofPayment = Convert.ToDecimal(parameters[2]);
                string operation = parameters[3];
                bool status = objBAL.CheckSanctionValue(proposalCode, valueofWork, valueofPayment, operation);
                if (status == true)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false });
                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "CheckSanctionCost()");
                return Json(new { success = false });
            }
        }

        #endregion

        #region PHYSICAL_LSB

        /// <summary>
        /// returns the list and add view of LSB Details
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult ListLSBPhysicalDetails(string urlparameter)
        {
            try
            {
                int proposalCode = 0;
                String[] encryptedParameters = null;
                Dictionary<string, string> decryptedParameters = null;
                if (urlparameter != string.Empty)
                {
                    encryptedParameters = urlparameter.Split('/');
                    if (encryptedParameters.Length == 3)
                    {
                        decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    }
                }
                ExecutionDAL objDAL = new ExecutionDAL();
                proposalCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);
                ExecutionLSBStatusViewModel roadModel = new ExecutionLSBStatusViewModel();
                roadModel = objDAL.GetLSBValues(proposalCode);
                if (roadModel == null)
                {
                    roadModel = new ExecutionLSBStatusViewModel();
                }
                roadModel.IMS_PR_ROAD_CODE = proposalCode;

                CommonFunctions objCommon = new CommonFunctions();
                roadModel.Operation = "A";
                ViewData["Year"] = objCommon.PopulateYears(false);
                ViewData["Month"] = objCommon.PopulateMonths();
                ViewData["WorkStatus"] = objDAL.GetWorkStatus();
                return View("ListLSBPhysicalDetails", roadModel);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "ListLSBPhysicalDetails()");
                return Json(new { success = false, message = "Request can not be processed at this time." });
            }
        }

        /// <summary>
        /// returns the LSB Details
        /// </summary>
        /// <param name="lsbCollection"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult GetLSBPhysicalProgressList(int? page, int? rows, string sidx, string sord)
        {
            IExecutionBAL objBAL = new ExecutionBAL();
            int imsRoadCode = 0;
            long totalRecords = 0;

            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }

            try
            {
                if (!(string.IsNullOrEmpty(Request.Params["roadCode"])))
                {
                    imsRoadCode = Convert.ToInt32(Request.Params["roadCode"]);
                }

                var jsonData = new
                {
                    rows = objBAL.GetLSBPhysicalProgressList(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, imsRoadCode),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "GetLSBPhysicalProgressList()");
                return null;
            }
        }

        /// <summary>
        /// returns the add view of physical road progress
        /// </summary>
        /// <param name="id">encrypted id key</param>
        /// <returns></returns>
        [Audit]
        public ActionResult AddPhysicalLSBProgress(string id)
        {
            try
            {
                ExecutionDAL objDAL = new ExecutionDAL();
                int proposalCode = Convert.ToInt32(id);
                ExecutionLSBStatusViewModel roadModel = new ExecutionLSBStatusViewModel();
                roadModel = objDAL.GetLSBValues(proposalCode);
                if (roadModel == null)
                {
                    roadModel = new ExecutionLSBStatusViewModel();
                }
                roadModel.IMS_PR_ROAD_CODE = proposalCode;
                CommonFunctions objCommon = new CommonFunctions();
                roadModel.Operation = "A";
                //ViewData["Year"] = objCommon.PopulateYears(true);
                //ViewData["Month"] = objCommon.PopulateMonths();
                ViewData["WorkStatus"] = objDAL.GetWorkStatus("L");

                #region Populate Month and Year
                //DateTime currDate = DateTime.Now;
                //roadModel.crYear = DateTime.Now.Year;

                //List<SelectListItem> lstMonth = new List<SelectListItem>();
                //List<SelectListItem> lstYear = new List<SelectListItem>();

                //if (currDate.Month == 1 && currDate.Day <= 14)
                //{
                //    lstYear.Insert(0, (new SelectListItem { Text = (currDate.Year - 1).ToString(), Value = (currDate.Year - 1).ToString() }));
                //    lstYear.Insert(0, (new SelectListItem { Text = (currDate.Year).ToString(), Value = (currDate.Year).ToString(), Selected = true }));
                //}
                //else
                //{
                //    lstYear.Insert(0, (new SelectListItem { Text = (currDate.Year).ToString(), Value = (currDate.Year).ToString(), Selected = true }));
                //}
                //if (currDate.Day <= 14)
                //{
                //    lstMonth.Insert(0, (new SelectListItem { Text = (currDate.AddMonths(-1)).ToString("MMMM"), Value = currDate.AddMonths(-1).Month.ToString() }));
                //    lstMonth.Insert(0, (new SelectListItem { Text = currDate.ToString("MMMM"), Value = currDate.Month.ToString(), Selected = true }));
                //}
                //else
                //{
                //    lstMonth.Insert(0, (new SelectListItem { Text = currDate.ToString("MMMM"), Value = currDate.Month.ToString(), Selected = true }));
                //}
                //ViewData["Year"] = lstYear;
                //ViewData["Month"] = lstMonth;

                //roadModel.currmonthName = currDate.ToString("MMMM");
                //roadModel.prevmonthName = currDate.AddMonths(-1).ToString("MMMM");
                #endregion

                ///Changes by SAMMED A. PATIL on 21JULY to restrict backdated progress entry for bridges
                ViewData["Month"] = objCommon.PopulateMonthsforCurrentFinancialYear(true);//objCommon.PopulateMonths();
                List<SelectListItem> lstYear = new List<SelectListItem>();
                #region Old Code
                //if (DateTime.Now.Month <= 3)
                //{
                //    lstYear.Insert(0, new SelectListItem { Text = (DateTime.Now.Year - 1).ToString(), Value = (DateTime.Now.Year - 1).ToString() });
                //}
                //else
                //{
                //    lstYear.Insert(0, new SelectListItem { Text = DateTime.Now.Year.ToString(), Value = DateTime.Now.Year.ToString() });
                //}
                #endregion
                if (DateTime.Now.Month == 1)
                {
                    lstYear.Insert(0, new SelectListItem { Text = (DateTime.Now.Year - 1).ToString(), Value = (DateTime.Now.Year - 1).ToString() });
                    lstYear.Insert(1, new SelectListItem { Text = (DateTime.Now.Year).ToString(), Value = (DateTime.Now.Year).ToString() });
                }
                else
                {
                    lstYear.Insert(0, new SelectListItem { Text = (DateTime.Now.Year).ToString(), Value = (DateTime.Now.Year).ToString() });
                }
                ViewData["Year"] = lstYear;

                return View("AddPhysicalLSBProgress", roadModel);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "AddPhysicalLSBProgress(string id)");
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        /// <summary>
        /// save the LSB Physical details
        /// </summary>
        /// <param name="lsbMmodel">model containing the form data</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Audit]
        public ActionResult AddPhysicalLSBDetails(ExecutionLSBStatusViewModel lsbMmodel)
        {
            string message = string.Empty;
            IExecutionBAL objBAL = new ExecutionBAL();
            try
            {
                //int allowedMonth = DateTime.Now.Month;
                //int allowedYear = DateTime.Now.Year;

                string AprilMonthDay = ConfigurationManager.AppSettings["PHYSICAL_ROAD_PROGRESS_APRIL_MONTH_DAY"];   //10
                int AprilMonthDayValue = Convert.ToInt16(AprilMonthDay);

                string AprilMonth = ConfigurationManager.AppSettings["PHYSICAL_ROAD_PROGRESS_APRIL_MONTH"];  //4
                int AprilMonthValue = Convert.ToInt16(AprilMonth);

                int allowedMonth = (DateTime.Now.Month == AprilMonthValue && DateTime.Now.Day <= AprilMonthDayValue) ? (DateTime.Now.Month - 1) : DateTime.Now.Month;
                int allowedYear = DateTime.Now.Year;

                #region Old Logic
                //int allowedMonth = DateTime.Now.Month == 1 ? (10) : (DateTime.Now.Month == 2 ? 11 : (DateTime.Now.Month - 3));
                //int allowedYear = DateTime.Now.Month == 1 ? (DateTime.Now.Year - 1) : (DateTime.Now.Month == 2 ? (DateTime.Now.Year - 1) : (DateTime.Now.Year));

                //if (lsbMmodel.EXEC_PROG_MONTH < allowedMonth || lsbMmodel.EXEC_PROG_YEAR < allowedYear)
                //{
                //    return Json(new { success = false, message = "Month and Year must be equal to current month and year or three months less than current date." }, JsonRequestBehavior.AllowGet);
                //}

                //if (lsbMmodel.EXEC_PROG_MONTH > allowedMonth && lsbMmodel.EXEC_PROG_YEAR <= allowedYear)
                //{
                //    return Json(new { success = false, message = "Month and Year must be equal to current month and year or last month." }, JsonRequestBehavior.AllowGet);
                //}
                #endregion



                if (DateTime.Now.Month == AprilMonthValue  &&  DateTime.Now.Day <= AprilMonthDayValue)
                {
                    if (ModelState.IsValid)
                    {
                        if (!(lsbMmodel.EXEC_PROG_MONTH == allowedMonth && lsbMmodel.EXEC_PROG_YEAR == allowedYear))
                        {
                            return Json(new { success = false, message = "Progress Month and Year must be equal to current month and year." }, JsonRequestBehavior.AllowGet);
                        }

                        if (!(objBAL.AddLSBPhysicalProgressDetails(lsbMmodel, ref message)))
                        {
                            if (message != string.Empty)
                            {
                                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                return Json(new { success = false, message = "Physical LSB Progress Details Not Added Successfully" }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                    }
                }
                else
                {
                    if (DateTime.Now.Day <= 10)  
                    {
                        if (DateTime.Now.Month == 1)
                        {
                            if (!((lsbMmodel.EXEC_PROG_MONTH == 12 && lsbMmodel.EXEC_PROG_YEAR == (allowedYear - 1)) || ((lsbMmodel.EXEC_PROG_MONTH == allowedMonth) && (lsbMmodel.EXEC_PROG_YEAR == allowedYear)))
                               )
                            {
                                return Json(new { success = false, message = "Progress Month and Year must be equal to current month and year." }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            if (!(((lsbMmodel.EXEC_PROG_MONTH == allowedMonth) || (lsbMmodel.EXEC_PROG_MONTH == (allowedMonth - 1)))
                                   && lsbMmodel.EXEC_PROG_YEAR == allowedYear)
                               )
                            {
                                return Json(new { success = false, message = "Progress Month and Year must be equal to current month and year." }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                    else
                    {
                        #region Added 
                        if (allowedMonth == 4 && allowedYear == 2020)
                        {
                            // Allow Progress Entry of March 2020 till End of April 2020
                        }
                        else
                        {

                            if (!(lsbMmodel.EXEC_PROG_MONTH == allowedMonth && lsbMmodel.EXEC_PROG_YEAR == allowedYear))
                            {
                                return Json(new { success = false, message = "Progress Month and Year must be equal to current month and year." }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        #endregion
                        // Commented
                        //if (!(lsbMmodel.EXEC_PROG_MONTH == allowedMonth && lsbMmodel.EXEC_PROG_YEAR == allowedYear))
                        //{
                        //    return Json(new { success = false, message = "Progress Month and Year must be equal to current month and year." }, JsonRequestBehavior.AllowGet);
                        //}
                    }

                    if (ModelState.IsValid)
                    {
                        if (!(objBAL.AddLSBPhysicalProgressDetails(lsbMmodel, ref message)))
                        {
                            if (message != string.Empty)
                            {
                                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                return Json(new { success = false, message = "Physical LSB Progress Details Not Added Successfully" }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                    }
                }
                #region   old code
                ///Changes by SAMMED A. PATIL on 21JULY to restrict backdated progress entry for bridges
                //if (DateTime.Now.Day <= 10)  
                //{
                //    if (DateTime.Now.Month == 1)
                //    {
                //        if (!((lsbMmodel.EXEC_PROG_MONTH == 12 && lsbMmodel.EXEC_PROG_YEAR == (allowedYear - 1)) || ((lsbMmodel.EXEC_PROG_MONTH == allowedMonth) && (lsbMmodel.EXEC_PROG_YEAR == allowedYear)))
                //           )
                //        {
                //            return Json(new { success = false, message = "Progress Month and Year must be equal to current month and year." }, JsonRequestBehavior.AllowGet);
                //        }
                //    }
                //    else
                //    {
                //        if (!(((lsbMmodel.EXEC_PROG_MONTH == allowedMonth) || (lsbMmodel.EXEC_PROG_MONTH == (allowedMonth - 1)))
                //               && lsbMmodel.EXEC_PROG_YEAR == allowedYear)
                //           )
                //        {
                //            return Json(new { success = false, message = "Progress Month and Year must be equal to current month and year." }, JsonRequestBehavior.AllowGet);
                //        }
                //    }
                //}
                //else
                //{

                //    #region Added 
                //    if (allowedMonth == 4 && allowedYear == 2020)
                //    { 
                //        // Allow Progress Entry of March 2020 till End of April 2020
                //    }
                //    else
                //    {

                //        if (!(lsbMmodel.EXEC_PROG_MONTH == allowedMonth && lsbMmodel.EXEC_PROG_YEAR == allowedYear))
                //        {
                //            return Json(new { success = false, message = "Progress Month and Year must be equal to current month and year." }, JsonRequestBehavior.AllowGet);
                //        }
                //    }
                //    #endregion

                //    // Commented
                //    //if (!(lsbMmodel.EXEC_PROG_MONTH == allowedMonth && lsbMmodel.EXEC_PROG_YEAR == allowedYear))
                //    //{
                //    //    return Json(new { success = false, message = "Progress Month and Year must be equal to current month and year." }, JsonRequestBehavior.AllowGet);
                //    //}
                //}

                #endregion 
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "AddPhysicalLSBProgress(ExecutionLSBStatusViewModel lsbMmodel)");
                return Json(new { success = false, message = "Physical LSB Progress Details Not Added Successfully" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// returns the LSB Physical details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult EditLSBPhysicalProgress(String parameter, String hash, String key)
        {
            try
            {
                Dictionary<string, string> decryptedParameters = null;
                CommonFunctions objCommon = new CommonFunctions();
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                IExecutionBAL objBAL = new ExecutionBAL();
                ExecutionDAL objDAL = new ExecutionDAL();
                int proposalCode = 0;   //for storing decrypted proposal code
                int monthCode = 0;  //for storing decrypted month code
                int yearCode = 0;   //for storing decrypted year code
                if (decryptedParameters != null)
                {
                    proposalCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);
                    monthCode = Convert.ToInt32(decryptedParameters["Month"]);
                    yearCode = Convert.ToInt32(decryptedParameters["Year"]);
                }
                ViewData["Year"] = objCommon.PopulateYears(true);
                ViewData["Month"] = objCommon.PopulateMonths();
                ViewData["WorkStatus"] = objDAL.GetWorkStatus("L");
                ExecutionLSBStatusViewModel model = objBAL.GetPhysicalLSBDetails(proposalCode, yearCode, monthCode);

                if (model != null)
                {
                    return PartialView("AddPhysicalLSBProgress", model);
                }
                return PartialView("AddPhysicalLSBProgress", new ExecutionLSBStatusViewModel());
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "EditPhysicalLSBDetails(String parameter, String hash, String key)");
                return Json(new { success = false, message = "Request can not be processed at this time." });
            }
        }

        /// <summary>
        /// updates the LSB Details
        /// </summary>
        /// <param name="lsbModel">model containing the updated data</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Audit]
        public ActionResult EditPhysicalLSBDetails(ExecutionLSBStatusViewModel lsbModel)
        {
            IExecutionBAL objBAL = new ExecutionBAL();
            string message = string.Empty;
            try
            {
                ///Changes by SAMMED A. PATIL on 21JULY to restrict backdated progress entry for bridges
                //int allowedMonth = DateTime.Now.Month;
                //int allowedYear = DateTime.Now.Year;

                string AprilMonthDay = ConfigurationManager.AppSettings["PHYSICAL_ROAD_PROGRESS_APRIL_MONTH_DAY"];   //10
                int AprilMonthDayValue = Convert.ToInt16(AprilMonthDay);

                string AprilMonth = ConfigurationManager.AppSettings["PHYSICAL_ROAD_PROGRESS_APRIL_MONTH"];  //4
                int AprilMonthValue = Convert.ToInt16(AprilMonth);

                int allowedMonth = (DateTime.Now.Month == AprilMonthValue && DateTime.Now.Day <= AprilMonthDayValue) ? (DateTime.Now.Month - 1) : DateTime.Now.Month;
                int allowedYear = DateTime.Now.Year;

                if (DateTime.Now.Month == AprilMonthValue && DateTime.Now.Day <= AprilMonthDayValue)
                {
                    if (ModelState.IsValid)
                    {
                        if (!(lsbModel.EXEC_PROG_MONTH == allowedMonth && lsbModel.EXEC_PROG_YEAR == allowedYear))
                        {
                            return Json(new { success = false, message = "Progress Month and Year must be equal to current month and year." }, JsonRequestBehavior.AllowGet);
                        }

                        if (!objBAL.EditLSBPhysicalRoadDetails(lsbModel, ref message))
                        {
                            return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                    }
                }
                else
                {
                    if (DateTime.Now.Day <= 10)
                    {
                        if (DateTime.Now.Month == 1)
                        {
                            if (!((lsbModel.EXEC_PROG_MONTH == 12 && lsbModel.EXEC_PROG_YEAR == (allowedYear - 1)) || ((lsbModel.EXEC_PROG_MONTH == allowedMonth) && (lsbModel.EXEC_PROG_YEAR == allowedYear)))
                                   )
                            {
                                return Json(new { success = false, message = "Progress Month and Year must be equal to current month and year." }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            if (!(((lsbModel.EXEC_PROG_MONTH == allowedMonth) || (lsbModel.EXEC_PROG_MONTH == (allowedMonth - 1)))
                                   && lsbModel.EXEC_PROG_YEAR == allowedYear)
                               )
                            {
                                return Json(new { success = false, message = "Progress Month and Year must be equal to current month and year." }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                    else
                    {

                        #region Added
                        if (allowedMonth == 4 && allowedYear == 2020)
                        {
                            // Allow Progress Entry of March 2020 till End of April 2020
                        }
                        else
                        {
                            if (!(lsbModel.EXEC_PROG_MONTH == allowedMonth && lsbModel.EXEC_PROG_YEAR == allowedYear))
                            {
                                return Json(new { success = false, message = "Progress Month and Year must be equal to current month and year." }, JsonRequestBehavior.AllowGet);
                            }
                        }

                        #endregion


                        // Commented
                        //if (!(lsbModel.EXEC_PROG_MONTH == allowedMonth && lsbModel.EXEC_PROG_YEAR == allowedYear))
                        //{
                        //    return Json(new { success = false, message = "Progress Month and Year must be equal to current month and year." }, JsonRequestBehavior.AllowGet);
                        //}
                    }

                    if (ModelState.IsValid)
                    {
                        if (!objBAL.EditLSBPhysicalRoadDetails(lsbModel, ref message))
                        {
                            return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                    }
                }
               
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "EditPhysicalLSBDetails(ExecutionLSBStatusViewModel lsbModel)");
                return null;
            }
        }

        /// <summary>
        /// delete operation for LSB details 
        /// </summary>
        /// <param name="parameter">encrypted id key</param>
        /// <param name="hash">encrypted id key</param>
        /// <param name="key">encrypted id key</param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult DeletePhysicalLSBDetails(String parameter, String hash, String key)
        {
            IExecutionBAL objBAL = new ExecutionBAL();
            string message = string.Empty;
            Dictionary<string, string> decryptedParameters = null;
            int proposalCode = 0;
            int monthCode = 0;
            int yearCode = 0;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                proposalCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);
                yearCode = Convert.ToInt32(decryptedParameters["Year"]);
                monthCode = Convert.ToInt32(decryptedParameters["Month"]);
                if ((objBAL.DeletePhysicalLSBDetails(proposalCode, yearCode, monthCode, ref message)))
                {
                    if (message == string.Empty)
                    {
                        return Json(new { success = true, message = "LSB progress details deleted successfully." });
                    }
                    else
                    {
                        return Json(new { success = true, message = message });
                    }
                }
                else
                {
                    return Json(new { success = false, message = message });
                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "DeletePhysicalLSBDetails()");
                return Json(new { success = false, message = "Error Occurred while processing your request." });
            }
        }

        #endregion

        #region FINANCIAL_PROGRESS


        /// <summary>
        /// returns the Add View of financial details
        /// </summary>
        /// <param name="id">Encrypted id indicating proposal</param>
        /// <returns></returns>
        [Audit]
        public ActionResult ListFinancialDetails(string urlparameter)
        {
            try
            {
                ExecutionDAL objDAL = new ExecutionDAL();
                int proposalCode = 0;
                string progressType = string.Empty;
                String[] encryptedParameters = null;
                Dictionary<string, string> decryptedParameters = null;
                if (urlparameter != string.Empty)
                {
                    encryptedParameters = urlparameter.Split('/');
                    if (encryptedParameters.Length == 3)
                    {
                        decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    }
                }
                proposalCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);
                progressType = decryptedParameters["Operation"];
                ExecutionProgressViewModel progressModel = new ExecutionProgressViewModel();
                progressModel = objDAL.GetFinancialDetails(proposalCode, progressType);

                progressModel.IMS_PR_ROAD_CODE = proposalCode;
                CommonFunctions objCommon = new CommonFunctions();
                progressModel.Operation = "A";
                if (progressType == "M")
                {
                    progressModel.EXEC_PROGRESS_TYPE = "M";
                }
                else
                {
                    progressModel.EXEC_PROGRESS_TYPE = "P";
                }
                ViewData["Year"] = objCommon.PopulateYears(false);
                ViewData["Month"] = objCommon.PopulateMonths();
                return View("ListFinancialDetails", progressModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Request can not be processed at this time." });
            }
        }

        /// <summary>
        /// returns the list of Financial Progress
        /// </summary>
        /// <param name="progressCollection">grid parameters along with filter parameters</param>
        /// <returns>list for populating grid</returns>
        [HttpPost]
        [Audit]
        public ActionResult GetFinancialProgressList(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                IExecutionBAL objBAL = new ExecutionBAL();
                long totalRecords = 0;
                int proposalCode = 0;
                string progressType = string.Empty;
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!(string.IsNullOrEmpty(Request.Params["roadCode"])))
                {
                    proposalCode = Convert.ToInt32(Request.Params["roadCode"]);
                }

                if (!(string.IsNullOrEmpty(Request.Params["progressType"])))
                {
                    progressType = Request.Params["progressType"];
                }


                var jsonData = new
                {
                    rows = objBAL.GetFinancialProgressList(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, proposalCode, progressType),
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

        /// <summary>
        /// returns the add view of Financial road details 
        /// </summary>
        /// <param name="id">encrypted id key</param>
        /// <returns></returns>
        [Audit]
        public ActionResult AddFinancialProgress(string id)
        {
            try
            {
                string[] str = id.Split(',');
                ExecutionDAL objDAL = new ExecutionDAL();
                int proposalCode = Convert.ToInt32(str[0]);
                string progressType = str[1];
                ExecutionProgressViewModel progressModel = new ExecutionProgressViewModel();
                progressModel = objDAL.GetFinancialDetails(proposalCode, progressType);
                progressModel.IMS_PR_ROAD_CODE = proposalCode;
                CommonFunctions objCommon = new CommonFunctions();
                progressModel.Operation = "A";
                progressModel.EXEC_PROGRESS_TYPE = str[1];
                ViewData["Year"] = objCommon.PopulateYears(true);
                ViewData["Month"] = objCommon.PopulateMonths();
                return View("AddFinancialProgress", progressModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        /// <summary>
        /// save the data of Financial details
        /// </summary>
        /// <param name="progreeModel">model containing the financial details</param>
        /// <returns>status along with message</returns>
        [HttpPost]
        [Audit]
        public ActionResult AddFinancialProgress(ExecutionProgressViewModel progressModel)
        {
            IExecutionBAL objBAL = new ExecutionBAL();
            string message = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    if (!(objBAL.AddFinancialProgress(progressModel, ref message)))
                    {
                        if (message == string.Empty)
                        {
                            return Json(new { success = false, message = "Financial details not added successfully." });
                        }
                        else
                        {
                            return Json(new { success = false, message = message });
                        }
                    }
                    else
                    {
                        return Json(new { success = true, message = message });
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Financial details not added successfully." });
            }
        }

        /// <summary>
        /// return the financial details for updation
        /// </summary>
        /// <param name="parameter">encrypted id for updation</param>
        /// <param name="hash">encrypted id for updation</param>
        /// <param name="key">encrypted id for updation</param>
        /// <returns>return view along with model containing data</returns>
        [Audit]
        public ActionResult EditFinancialDetails(String parameter, String hash, String key)
        {
            try
            {
                Dictionary<string, string> decryptedParameters = null;
                CommonFunctions objCommon = new CommonFunctions();
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                IExecutionBAL objBAL = new ExecutionBAL();
                ExecutionDAL objDAL = new ExecutionDAL();
                int proposalCode = 0;   //for storing decrypted proposal code
                int monthCode = 0;  //for storing decrypted month code
                int yearCode = 0;   //for storing decrypted year code
                string progressType = string.Empty; //for storing type of progress
                if (decryptedParameters != null)
                {
                    proposalCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);
                    monthCode = Convert.ToInt32(decryptedParameters["Month"]);
                    yearCode = Convert.ToInt32(decryptedParameters["Year"]);
                    //progressType = decryptedParameters["Type"];
                }
                ViewData["Year"] = objCommon.PopulateYears(true);
                ViewData["Month"] = objCommon.PopulateMonths();
                ExecutionProgressViewModel progressModel = objBAL.GetFinancialDetails(proposalCode, yearCode, monthCode);
                progressModel.PreviousYear = 0;
                if (progressModel != null)
                {
                    return PartialView("AddFinancialProgress", progressModel);
                }
                else
                {
                    return Json(new { success = false, message = "Request can not be processed at this time." });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Request can not be processed at this time." });
            }
        }

        /// <summary>
        /// updates the financial details
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult EditFinancialProgress(ExecutionProgressViewModel progressModel)
        {
            IExecutionBAL objBAL = new ExecutionBAL();
            string message = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    if (!(objBAL.EditFinancialProgress(progressModel, ref message)))
                    {
                        if (message == string.Empty)
                        {
                            return Json(new { success = false, message = "Financial details not updated successfully." });
                        }
                        else
                        {
                            return Json(new { success = false, message = message });
                        }
                    }
                    else
                    {
                        return Json(new { success = true, message = message });
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Financial details not updated successfully." });
            }
        }

        /// <summary>
        /// delete operation for Financial details
        /// </summary>
        /// <param name="parameter">encrypted id key</param>
        /// <param name="hash">encrypted id key</param>
        /// <param name="key">encrypted id key</param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult DeleteFinancialDetails(String parameter, String hash, String key)
        {
            IExecutionBAL objBAL = new ExecutionBAL();
            string message = string.Empty;
            Dictionary<string, string> decryptedParameters = null;
            int proposalCode = 0;
            int monthCode = 0;
            int yearCode = 0;
            string progressType = string.Empty;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                proposalCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);
                yearCode = Convert.ToInt32(decryptedParameters["Year"]);
                monthCode = Convert.ToInt32(decryptedParameters["Month"]);
                //progressType = decryptedParameters["Type"];
                if ((objBAL.DeleteFinancialRoadDetails(proposalCode, yearCode, monthCode, ref message)))
                {
                    if (message == string.Empty)
                    {
                        return Json(new { success = true, message = "Financial details deleted successfully." });
                    }
                    else
                    {
                        return Json(new { success = true, message = message });
                    }
                }
                else
                {
                    return Json(new { success = false, message = message });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error Occurred while processing your request." });
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult GetRoadAgreementDetailsList(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                IExecutionBAL objBAL = new ExecutionBAL();
                long totalRecords = 0;
                int proposalCode = 0;
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!(string.IsNullOrEmpty(Request.Params["roadCode"])))
                {
                    proposalCode = Convert.ToInt32(Request.Params["roadCode"]);
                }

                var jsonData = new
                {
                    rows = objBAL.GetRoadAgreementDetailsList(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, proposalCode),
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

        #endregion

        #region CD_WORKS

        /// <summary>
        /// returns the cd works list
        /// </summary>
        /// <param name="urlparameter">encrypted road code</param>
        /// <returns></returns>
        [Audit]
        public ActionResult ListCDWorksDetails(string urlparameter)
        {
            ExecutionDAL objDAL = new ExecutionDAL();
            try
            {
                int proposalCode = 0;
                String[] encryptedParameters = null;
                Dictionary<string, string> decryptedParameters = null;
                if (urlparameter != string.Empty)
                {
                    encryptedParameters = urlparameter.Split('/');
                    if (encryptedParameters.Length == 3)
                    {
                        decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    }
                }
                proposalCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);
                ExecutionCDWorksViewModel model = new ExecutionCDWorksViewModel();
                model = objDAL.GetOldValues(proposalCode);
                model.CD_WORKS = objDAL.GetCdWorksValues();
                model.IMS_PR_ROAD_CODE = proposalCode;
                model.Operation = "A";
                return View("ListCDWorksDetails", model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        /// <summary>
        /// returns the list of CDWorks details
        /// </summary>
        /// <param name="cdWorksCollection">contains </param>
        /// <returns></returns>
        [Audit]
        public ActionResult GetCDWorksList(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                IExecutionBAL objBAL = new ExecutionBAL();
                long totalRecords = 0;
                int proposalCode = 0;

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!(string.IsNullOrEmpty(Request.Params["roadCode"])))
                {
                    proposalCode = Convert.ToInt32(Request.Params["roadCode"]);
                }
                var jsonData = new
                {
                    rows = objBAL.GetCDWorksList(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, proposalCode),
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

        /// <summary>
        /// returns the add view of CDWorks progress
        /// </summary>
        /// <param name="id">encrypted id key</param>
        /// <returns></returns>
        [Audit]
        public ActionResult AddCDWorksProgress(string id)
        {
            try
            {
                ExecutionDAL objDAL = new ExecutionDAL();
                ExecutionCDWorksViewModel model = new ExecutionCDWorksViewModel();
                model = objDAL.GetOldValues(Convert.ToInt32(id));
                model.CD_WORKS = objDAL.GetCdWorksValues();
                model.IMS_PR_ROAD_CODE = Convert.ToInt32(id);
                model.Operation = "A";
                return PartialView("AddCDWorksProgress", model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occurred while processing your request." }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// for saving the CDWorks details
        /// </summary>
        /// <param name="cdWorksModel">data containing CDWorks details</param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult AddCdWorksDetails(ExecutionCDWorksViewModel cdWorksModel)
        {
            IExecutionBAL objBAL = new ExecutionBAL();
            string message = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    if (!(objBAL.AddCDWorksDetails(cdWorksModel, ref message)))
                    {
                        if (message == string.Empty)
                        {
                            return Json(new { success = false, message = "CDWorks details not added successfully." });
                        }
                        else
                        {
                            return Json(new { success = false, message = message });
                        }
                    }
                    else
                    {
                        return Json(new { success = true, message = message });
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error Occurred while processing your request." });
            }
        }

        /// <summary>
        /// returns the CDWorks details for updation
        /// </summary>
        /// <param name="parameter">encrypted id key</param>
        /// <param name="hash">encrypted id key</param>
        /// <param name="key">encrypted id key</param>
        /// <returns></returns>
        [Audit]
        public ActionResult EditCDWorksDetails(String parameter, String hash, String key)
        {
            try
            {
                Dictionary<string, string> decryptedParameters = null;
                CommonFunctions objCommon = new CommonFunctions();
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                IExecutionBAL objBAL = new ExecutionBAL();
                ExecutionDAL objDAL = new ExecutionDAL();
                int proposalCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);
                int cdWorksCode = Convert.ToInt32(decryptedParameters["CDWorksCode"]);
                ExecutionCDWorksViewModel model = objBAL.GetCdWorksDetails(proposalCode, cdWorksCode);
                model.CD_WORKS = objDAL.GetCdWorksValues();
                model.PreviousCDWorksCount = model.PreviousCDWorksCount - 1;
                if (model != null)
                {
                    return View("AddCDWorksProgress", model);
                }
                else
                {
                    return Json(new { success = false, message = "Error Occurred while processing your request." });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error Occurred while processing your request." });
            }
        }

        /// <summary>
        /// for upadation of CDWorks details
        /// </summary>
        /// <param name="cdWorksModel">CDWorks details data for updation</param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult EditCdWorksDetails(ExecutionCDWorksViewModel cdWorksModel)
        {
            IExecutionBAL objBAL = new ExecutionBAL();
            string message = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    if (!(objBAL.EditCDWorksDetails(cdWorksModel, ref message)))
                    {
                        if (message == string.Empty)
                        {
                            return Json(new { success = false, message = "CDWorks details not added successfully." });
                        }
                        else
                        {
                            return Json(new { success = false, message = message });
                        }
                    }
                    else
                    {
                        return Json(new { success = true, message = message });
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error Occurred while processing your request." });
            }
        }

        /// <summary>
        /// delete CDWorks details 
        /// </summary>
        /// <param name="parameter">Encrypted id</param>
        /// <param name="hash">Encrypted id</param>
        /// <param name="key">Encrypted id</param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult DeleteCdWorksDetails(String parameter, String hash, String key)
        {
            IExecutionBAL objBAL = new ExecutionBAL();
            string message = string.Empty;
            Dictionary<string, string> decryptedParameters = null;
            String[] encryptedParameters = null;
            int proposalCode = 0;
            int cdWorksCode = 0;
            try
            {
                //encryptedParameters = id.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                proposalCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);
                cdWorksCode = Convert.ToInt32(decryptedParameters["CDWorksCode"]);
                if (!(objBAL.DeleteCDWorksDetails(proposalCode, cdWorksCode, ref message)))
                {
                    if (message == string.Empty)
                    {
                        return Json(new { success = false, message = "CDWorks details deleted successfully." });
                    }
                    else
                    {
                        return Json(new { success = false, message = message });
                    }
                }
                else
                {
                    return Json(new { success = true, message = message });
                }

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error Occurred while processing your request." });
            }
        }

        [Audit]
        public ActionResult CheckCDWorksCount(string id)
        {
            ExecutionDAL objDAL = new ExecutionDAL();
            try
            {
                if (objDAL.CheckCDWorksCount(Convert.ToInt32(id), "A"))
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Total no. of CDWorks are already entered." });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occcurred while processing your request." });
            }
        }

        [Audit]
        public ActionResult CheckPhysicalRoadDetails(string id)
        {
            ExecutionDAL objDAL = new ExecutionDAL();
            try
            {
                if (objDAL.CheckPhysicalRoadDetails(Convert.ToInt32(id)))
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Work status is already completed so further entries are not allowed." });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occcurred while processing your request." });
            }
        }

        [Audit]
        public ActionResult CheckPhysicalLSBDetails(string id)
        {
            ExecutionDAL objDAL = new ExecutionDAL();
            try
            {
                if (objDAL.CheckPhysicalLSBDetails(Convert.ToInt32(id)))
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Work status is already completed so further entries are not allowed." });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occcurred while processing your request." });
            }
        }


        [Audit]
        public ActionResult CheckFinancialDetails(string id)
        {
            ExecutionDAL objDAL = new ExecutionDAL();
            try
            {
                String[] parameters = id.Split(',');
                int proposalCode = Convert.ToInt32(parameters[0]);
                if (objDAL.CheckFinancialDetails(proposalCode, parameters[1]))
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Final payment is already made so further entries are not allowed." });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occcurred while processing your request." });
            }
        }


        #endregion

        #region REMARKS

        [Audit]
        public ActionResult ListRemarkDetails(string urlparameter)
        {
            Dictionary<string, string> decryptedParameters = null;
            ExecutionBAL objBAL = new ExecutionBAL();
            ExecutionDAL objDAL = new ExecutionDAL();
            String[] encryptedParameters = null;
            int proposalCode = 0;
            try
            {
                if (urlparameter != null)
                {
                    encryptedParameters = urlparameter.Split('/');
                    decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    proposalCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);
                    ProposalRemarksViewModel model = new ProposalRemarksViewModel();
                    model = objDAL.GetProgressValues(proposalCode);
                    model.IMS_PR_ROAD_CODE = proposalCode;
                    return PartialView("ListRemarkDetails", model);
                }
                else
                {
                    return Json(new { success = false, message = "Error occurred while processing your request." });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }



        /// <summary>
        /// returns the add view of Progress remarks 
        /// </summary>
        /// <param name="urlparameter">encrypted id key</param>
        /// <returns></returns>
        [Audit]
        public ActionResult AddProgressRemarks(string id)
        {
            ExecutionDAL objDAL = new ExecutionDAL();
            int proposalCode = 0;
            try
            {
                if (id != null)
                {
                    proposalCode = Convert.ToInt32(id);
                    ProposalRemarksViewModel model = new ProposalRemarksViewModel();
                    model = objDAL.GetProgressValues(proposalCode);
                    model.IMS_PR_ROAD_CODE = proposalCode;
                    return PartialView("AddProgressRemarks", model);
                }
                else
                {
                    return Json(new { success = false, message = "Error occurred while processing your request." });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        /// <summary>
        /// save the Progress remarks details 
        /// </summary>
        /// <param name="remarkModel">model containing progress remarks details </param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult AddProgressRemarks(ProposalRemarksViewModel remarkModel)
        {
            string message = string.Empty;
            ExecutionBAL objBAL = new ExecutionBAL();
            try
            {
                if (ModelState.IsValid)
                {
                    if (objBAL.AddProgressRemarks(remarkModel, ref message))
                    {
                        if (message == string.Empty)
                        {
                            return Json(new { success = true, message = "Remarks added successfully." });
                        }
                        else
                        {
                            return Json(new { success = true, message = message });
                        }
                    }
                    else
                    {
                        return Json(new { success = false, message = message });
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error Occurred while processing your request." });
            }
        }

        /// <summary>
        /// returns the remarks of particular proposal
        /// </summary>
        /// <param name="proposalCode">id of proposal</param>
        /// <returns></returns>
        [Audit]
        public ActionResult GetRemarksList(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                IExecutionBAL objBAL = new ExecutionBAL();
                long totalRecords = 0;
                int proposalCode = 0;

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!(string.IsNullOrEmpty(Request.Params["proposalCode"])))
                {
                    proposalCode = Convert.ToInt32(Request.Params["proposalCode"]);
                }
                var jsonData = new
                {
                    rows = objBAL.GetRemarksList(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, proposalCode),
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

        [HttpPost]
        [Audit]
        public ActionResult DeleteRemark(string IMS_PR_ROAD_CODE)
        {
            Dictionary<string, string> decryptedParameters = null;
            String[] encryptedParameters = null;
            ExecutionBAL objBAL = new ExecutionBAL();
            int proposalCode = 0;
            try
            {
                encryptedParameters = IMS_PR_ROAD_CODE.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                proposalCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);
                bool status = objBAL.DeleteRemark(proposalCode, ref message);
                if (status == true)
                {
                    return Json(new { success = true, message = message });
                }
                else
                {
                    return Json(new { success = false, message = message });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = message });
            }
        }


        [HttpPost]
        [Audit]
        public ActionResult EditRemark(ProposalRemarksViewModel model)
        {
            ExecutionBAL objBAL = new ExecutionBAL();
            try
            {
                bool status = objBAL.EditRemark(model, ref message);
                if (status == true)
                {
                    return Json(new { success = true, message = message });
                }
                else
                {
                    return Json(new { success = false, message = message });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = message });
            }
        }

        [Audit]
        public ActionResult EditRemark(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            ExecutionDAL objDAL = new ExecutionDAL();
            int proposalCode = 0;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                proposalCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);
                ProposalRemarksViewModel model = objDAL.GetProgressValues(proposalCode, true);
                if (model != null)
                {
                    return PartialView("AddProgressRemarks", model);
                }
                else
                {
                    return Json(new { success = false, message = "Error occurred while processing your request." });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }


        #endregion

        #region OTHER

        /// <summary>
        /// returns districts according to the state
        /// </summary>
        /// <param name="stateCode">indicates the state id</param>
        /// <returns></returns>
        [Audit]
        public JsonResult GetDistrictByState(int stateCode)
        {
            CommonFunctions objCommon = new CommonFunctions();
            try
            {
                return Json(new SelectList(objCommon.PopulateDistrict(stateCode, true), "Value", "Text"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        /// <summary>
        /// returns the package code by year and district code
        /// </summary>
        /// <param name="yearCode">indicates the year code</param>
        /// <param name="districtCode">indicates the district code</param>
        /// <returns></returns>
        [Audit]
        public JsonResult GetPackageByState(int yearCode, int districtCode)
        {
            CommonFunctions objCommon = new CommonFunctions();
            try
            {
                TransactionParams objParam = new TransactionParams();
                objParam.SANC_YEAR = Convert.ToInt16(yearCode);
                objParam.DISTRICT_CODE = districtCode;
                return Json(new SelectList(objCommon.PopulatePackage(objParam), "Value", "Text"), JsonRequestBehavior.AllowGet);
                //List<IMS_SANCTIONED_PROJECTS> lstProposal = db.IMS_SANCTIONED_PROJECTS.Where(m=>m.MAST_STATE_CODE==stateCode && m.MAST_DISTRICT_CODE == districtCode).ToList<IMS_SANCTIONED_PROJECTS>();
                //return Json(new SelectList(lstProposal, "IMS_PACKAGE_ID", "IMS_PACKAGE_ID"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [Audit]
        public ActionResult ViewExecutionProgressDetails(string urlparameter)
        {
            String[] encryptedParameters = null;
            Dictionary<string, string> decryptedParameters = null;
            int proposalCode = 0;
            try
            {
                ExecutionProgressViewModel execModel = new ExecutionProgressViewModel();
                encryptedParameters = urlparameter.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                proposalCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);

                #region
                ExecutionDAL objDAL = new ExecutionDAL();

                execModel = objDAL.GetFinancialDetails(proposalCode, "P");
                execModel.Operation = "A";
                #endregion

                execModel.IMS_PR_ROAD_CODE = proposalCode;
                execModel.EXEC_PROGRESS_TYPE = "P";
                return View("ViewExecutionProgressDetails", execModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Request can not be processed at this time." });
            }
        }

        [Audit]
        public ActionResult GetProposalType(string id)
        {
            bool status;
            ExecutionBAL objBAL = new ExecutionBAL();
            try
            {
                status = objBAL.CheckProposalType(Convert.ToInt32(id));
                if (status == true)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = "", message = "Error occurred while processing your request." });
            }
        }

        #endregion

        #region TECHNOLOGY

        /// <summary>
        /// returns the cd works list
        /// </summary>
        /// <param name="urlparameter">encrypted road code</param>
        /// <returns></returns>
        [Audit]
        public ActionResult ListTechnologyDetails(string urlparameter)
        {
            ExecutionDAL objDAL = new ExecutionDAL();
            try
            {
                int proposalCode = 0;
                String[] encryptedParameters = null;
                Dictionary<string, string> decryptedParameters = null;
                if (urlparameter != string.Empty)
                {
                    encryptedParameters = urlparameter.Split('/');
                    if (encryptedParameters.Length == 3)
                    {
                        decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    }
                }
                proposalCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);
                //ExecutionCDWorksViewModel model = new ExecutionCDWorksViewModel();
                ExecutionRoadStatusViewModel model = new ExecutionRoadStatusViewModel();
                //model = objDAL.GetOldValues(proposalCode);
                // model.CD_WORKS = objDAL.GetCdWorksValues();
                model = objDAL.GetRoadDetails(proposalCode);
                model.IMS_PR_ROAD_CODE = proposalCode;
                model.Operation = "A";

                ViewBag.EncryptedProposalCode = URLEncrypt.EncryptParameters1(new string[] { "ProposalCode=" + proposalCode.ToString().Trim() });
                return View("ListTechnologyDetails", model);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "ListTechnologyDetails()");
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        [Audit]
        public ActionResult ListTechnologyProgressDetails(String hash, String parameter, String key)
        {
            ExecutionDAL objDAL = new ExecutionDAL();
            TechnologyDetailsViewModel model = new TechnologyDetailsViewModel();
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptedParameters != null)
                {
                    int proposalCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);
                    int technologyCode = Convert.ToInt32(decryptedParameters["TechnologyCode"]);
                    decimal stChainage = Convert.ToDecimal(decryptedParameters["StChainage"]);
                    decimal endChainage = Convert.ToDecimal(decryptedParameters["EndChainage"]);
                    int layerCode = Convert.ToInt32(decryptedParameters["LayerCode"]);

                    model.EncryptedRoadCode = URLEncrypt.EncryptParameters1(new string[] { "ProposalCode=" + proposalCode.ToString().Trim() });

                    model.EncryptedTechCode = URLEncrypt.EncryptParameters1(new string[] { "TechnologyCode=" + technologyCode.ToString().Trim() });
                    model.EncryptedStChainage = URLEncrypt.EncryptParameters1(new string[] { "StChainage=" + stChainage.ToString().Trim() });
                    model.EncryptedEndChainage = URLEncrypt.EncryptParameters1(new string[] { "EndChainage=" + endChainage.ToString().Trim() });
                    model.EncryptedLayerCode = URLEncrypt.EncryptParameters1(new string[] { "LayerCode=" + layerCode.ToString().Trim() });

                    dbContext = new PMGSYEntities();
                    ///Changes by SAMMED A. PATIL on 20JULY2017 
                    model.previousStatus = dbContext.EXEC_TECH_MONTHLY_STATUS.Where(a => a.IMS_PR_ROAD_CODE == proposalCode && a.MAST_TECH_CODE == technologyCode && a.MAST_LAYER_CODE == layerCode).OrderByDescending(x => x.TECH_MONTHLY_CODE).Select(x => x.EXEC_ISCOMPLETED).FirstOrDefault();
                }

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ListTechnologyProgressDetails()");
                //return Json(new { success = false, message = "Error occurred while processing your request.", JsonRequestBehavior.AllowGet });
                return null;
            }
            finally
            {

            }
        }

        [HttpGet]
        [Audit]
        public ActionResult AddEditTechnologyProgressDetails(String hash, String parameter, String key)
        {
            ExecutionDAL objDAL = new ExecutionDAL();
            TechnologyDetailsViewModel model = null;
            int techMonthlyCode = 0;
            int techCode = 0;
            int layerCode = 0;
            Dictionary<string, string> decryptedParameters, decryptedParameters1 = null, decryptedParameters2 = null;
            try
            {
                dbContext = new PMGSYEntities();

                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptedParameters != null)
                {
                    int proposalCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);
                    if (decryptedParameters.Count > 2)
                    {
                        techMonthlyCode = Convert.ToInt32(decryptedParameters["TechMonthlyCode"]);
                        techCode = Convert.ToInt32(decryptedParameters["TechnologyCode"]);
                        layerCode = Convert.ToInt32(decryptedParameters["LayerCode"]);
                    }

                    string opr = Convert.ToString(Request.Params["Operation"]).Trim();

                    if (!string.IsNullOrEmpty(Request.Params["EncrTechCode"]))
                    {

                        string[] arr = Request.Params["EncrTechCode"].Split('/');
                        decryptedParameters1 = URLEncrypt.DecryptParameters1(new string[] { arr[0], arr[1], arr[2] });
                        techCode = Convert.ToInt32(decryptedParameters1["TechnologyCode"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Params["EncrLayerCode"]))
                    {

                        string[] arr = Request.Params["EncrLayerCode"].Split('/');
                        decryptedParameters2 = URLEncrypt.DecryptParameters1(new string[] { arr[0], arr[1], arr[2] });
                        layerCode = Convert.ToInt32(decryptedParameters2["LayerCode"]);
                    }

                    model = objDAL.GetTechProgressDetails(proposalCode, techCode, techMonthlyCode, layerCode, opr);
                    if (model == null)
                    {
                        model = new TechnologyDetailsViewModel();
                    }

                    if (!string.IsNullOrEmpty(Request.Params["EncrTechCode"]))
                    {
                        model.EncryptedTechCode = Request.Params["EncrTechCode"].Trim();
                        model.TechnologyCode = Convert.ToInt32(decryptedParameters1["TechnologyCode"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Params["EncrLayerCode"]))
                    {
                        model.EncryptedLayerCode = Request.Params["EncrLayerCode"].Trim();
                        model.hdnLayerCode = Convert.ToInt32(decryptedParameters2["LayerCode"]);
                    }

                    CommonFunctions objCommon = new CommonFunctions();
                    model.IMS_PR_ROAD_CODE = proposalCode;

                    model.EncryptedRoadCode = URLEncrypt.EncryptParameters1(new string[] { "ProposalCode=" + proposalCode.ToString().Trim() });

                    model.flg = dbContext.EXEC_TECH_MONTHLY_STATUS.Where(x => x.IMS_PR_ROAD_CODE == proposalCode && x.MAST_TECH_CODE == model.TechnologyCode).Any();

                    model.sanctionLength = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == proposalCode).Select(x => x.IMS_PAV_LENGTH).FirstOrDefault();

                    var qery = dbContext.IMS_PROPOSAL_TECH.Where(x => x.IMS_PR_ROAD_CODE == proposalCode && x.MAST_TECH_CODE == techCode && x.MAST_LAYER_CODE == layerCode).FirstOrDefault();
                    if (qery != null)
                    {
                        //foreach (var item in qery)
                        {
                            model.startChainage = qery.IMS_START_CHAINAGE;
                            model.endChainage = qery.IMS_END_CHAINAGE;
                            model.technologyName = qery.MASTER_TECHNOLOGY.MAST_TECH_DESC.Trim();
                            model.layerName = qery.MASTER_EXECUTION_ITEM.MAST_HEAD_DESC.Trim();
                        }
                    }

                    model.Operation = "A";

                    model.MonthList = objCommon.PopulateMonthsforCurrentFinancialYear(true);//objCommon.PopulateMonths();

                    List<SelectListItem> lstYear = new List<SelectListItem>();
                    //lstYear = objCommon.PopulateYears(true);

                    //int year = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == proposalCode).Max(m => m.IMS_YEAR);
                    //year = year - 1;
                    //int count = lstYear.IndexOf(lstYear.Find(c => c.Value == year.ToString()));
                    //lstYear.RemoveRange(lstYear.IndexOf(lstYear.Find(c => c.Value == year.ToString())), lstYear.Count - count);
                    //model.YearList = lstYear;

                    ///Changed for back dated Technology Prgress entry upto 5th of current Month
                    #region Old Logic
                    //if (DateTime.Now.Month <= 3)
                    //{
                    //    lstYear.Insert(0, new SelectListItem { Text = (DateTime.Now.Year - 1).ToString(), Value = (DateTime.Now.Year - 1).ToString() });
                    //}
                    //else
                    //{
                    //    lstYear.Insert(0, new SelectListItem { Text = DateTime.Now.Year.ToString(), Value = DateTime.Now.Year.ToString() });
                    //}
                    #endregion
                    if (DateTime.Now.Day <= 10 && DateTime.Now.Month == 1)
                    {
                        lstYear.Insert(0, new SelectListItem { Text = (DateTime.Now.Year - 1).ToString(), Value = (DateTime.Now.Year - 1).ToString() });
                        lstYear.Insert(1, new SelectListItem { Text = (DateTime.Now.Year).ToString(), Value = (DateTime.Now.Year).ToString() });
                    }
                    else
                    {
                        lstYear.Insert(0, new SelectListItem { Text = (DateTime.Now.Year).ToString(), Value = (DateTime.Now.Year).ToString() });
                    }
                    model.YearList = lstYear;

                    int agreementCode = 0;

                    if (dbContext.TEND_AGREEMENT_DETAIL.Any(m => m.IMS_PR_ROAD_CODE == proposalCode && m.TEND_AGREEMENT_STATUS == "P"))
                    {
                        agreementCode = dbContext.TEND_AGREEMENT_DETAIL.Where(m => m.IMS_PR_ROAD_CODE == proposalCode && m.TEND_AGREEMENT_STATUS == "P").OrderBy(m => m.TEND_AGREEMENT_ID).Select(m => m.TEND_AGREEMENT_CODE).FirstOrDefault();
                    }
                    else
                    {
                        agreementCode = dbContext.TEND_AGREEMENT_DETAIL.Where(m => m.IMS_PR_ROAD_CODE == proposalCode).OrderBy(m => m.TEND_AGREEMENT_ID).Select(m => m.TEND_AGREEMENT_CODE).FirstOrDefault();
                    }

                    model.agreementDate = new CommonFunctions().GetDateTimeToString(dbContext.TEND_AGREEMENT_MASTER.Where(x => x.TEND_AGREEMENT_CODE == agreementCode).Select(x => x.TEND_DATE_OF_AGREEMENT).FirstOrDefault());


                    model.StatusList = new List<SelectListItem>();
                    model.StatusList.Insert(0, new SelectListItem { Text = "In Progress", Value = "P" });
                    model.StatusList.Insert(1, new SelectListItem { Text = "Completed", Value = "C" });
                    if (Request.Params["Operation"] != null)
                    {
                        if (Convert.ToString(Request.Params["Operation"]).Trim() == "E")
                        {
                            if (techCode > 0 && model.TechnologyCode == 0)
                            {
                                model.TechnologyCode = techCode;
                            }
                            CommonFunctions comm = new CommonFunctions();
                            model.techMonthlyCode = Convert.ToInt32(decryptedParameters["TechMonthlyCode"]);
                            var query = dbContext.EXEC_TECH_MONTHLY_STATUS.Where(x => x.IMS_PR_ROAD_CODE == proposalCode && x.MAST_TECH_CODE == model.TechnologyCode && x.TECH_MONTHLY_CODE == model.techMonthlyCode).FirstOrDefault();
                            if (query != null)
                            {
                                model.Operation = "E";
                                model.Month = query.EXEC_PROG_MONTH;
                                model.Year = query.EXEC_PROG_YEAR;
                                model.Status = query.EXEC_ISCOMPLETED;
                                model.Date = comm.GetDateTimeToString(query.EXEC_PROGRESS_DATE.Value);
                                model.completedLength = query.EXEC_COMPLETED.Value;
                                model.previousStatus = query.EXEC_ISCOMPLETED;
                                model.totalTechQuantity = dbContext.EXEC_TECH_MONTHLY_STATUS.Where(x => x.IMS_PR_ROAD_CODE == proposalCode && x.MAST_TECH_CODE == techCode && x.MAST_LAYER_CODE == layerCode).Select(x => x.EXEC_TECH_PROGRESS_DETAILS.TECH_QUANTITY).Sum();

                                //Added by pradip patil [08/02/2017]
                                EXEC_TECH_PROGRESS_DETAILS TechoprogressModel = dbContext.EXEC_TECH_PROGRESS_DETAILS.Where(x => x.TECH_MONTHLY_CODE == techMonthlyCode).FirstOrDefault();

                                if (TechoprogressModel != null)
                                {
                                    model.TechQuantity = Convert.ToDecimal(TechoprogressModel.TECH_QUANTITY.ToString("0.00"));
                                    model.TechUnit = TechoprogressModel.TECH_UNIT;
                                    model.TechSupplier = TechoprogressModel.TECH_SUPPLIER;
                                    model.RatePerunit = Convert.ToDecimal(TechoprogressModel.TECH_RATE_PER_UNIT.ToString("0.00")); ;
                                }
                                //ends
                            }
                        }
                    }


                    model.UnitList = new List<SelectListItem>();
                    model.UnitList.Add(new SelectListItem { Text = "Select unit", Value = "0", Selected = true });
                    model.UnitList.Add(new SelectListItem { Text = "Tons", Value = "1", Selected = true });
                    model.UnitList.Add(new SelectListItem { Text = "Square Metre", Value = "2", Selected = true });
                    model.UnitList.Add(new SelectListItem { Text = "Cubic Metre", Value = "3", Selected = true });
                    // Added by Pradip Patil [08/05/2017] ends
                }
                return View(model);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "AddEditTechnologyProgressDetails()");
                //return Json(new { success = false, message = "Error Occurred while processing your request.", JsonRequestBehavior.AllowGet });
                return null;
            }
        }

        /// <summary>
        /// returns the list of Technology details
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetTechnologyProgressDetailsList(int? page, int? rows, string sidx, string sord)
        {
            //Added By Abhishek kamble 30-Apr-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Added By SAMMED PATIL 3-NOV-2016 end
            ExecutionBAL objBAL = new ExecutionBAL();
            long totalRecords = 0;
            int imsRoadCode = 0;
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!(string.IsNullOrEmpty(Request.Params["ProposalCode"])))
                {
                    imsRoadCode = Convert.ToInt32(Request.Params["ProposalCode"]);
                }

                var jsonData = new
                {
                    rows = objBAL.GetTechnologyProgressDetailsListBAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, imsRoadCode),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "GetTechnologyProgressDetailsList()");
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        /// <summary>
        /// returns the list of Technology details
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetExecTechnologyProgressDetailsList(int? page, int? rows, string sidx, string sord)
        {
            //Added By Abhishek kamble 30-Apr-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Added By SAMMED PATIL 3-NOV-2016 end
            ExecutionBAL objBAL = new ExecutionBAL();
            long totalRecords = 0;
            int imsRoadCode = 0;
            int technologyCode = 0, layerCode = 0;
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!(string.IsNullOrEmpty(Request.Params["ProposalCode"])))
                {
                    imsRoadCode = Convert.ToInt32(Request.Params["ProposalCode"]);
                }
                if (!(string.IsNullOrEmpty(Request.Params["TechnologyCode"])))
                {
                    string[] arr = (Request.Params["TechnologyCode"]).Split('/');
                    decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { arr[0], arr[1], arr[2] });
                    technologyCode = Convert.ToInt32(decryptedParameters["TechnologyCode"]);
                }
                if (!(string.IsNullOrEmpty(Request.Params["LayerCode"])))
                {
                    string[] arr = (Request.Params["LayerCode"]).Split('/');
                    decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { arr[0], arr[1], arr[2] });
                    layerCode = Convert.ToInt32(decryptedParameters["LayerCode"]);
                }

                var jsonData = new
                {
                    rows = objBAL.GetExecTechnologyProgressDetailsListBAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, imsRoadCode, technologyCode, layerCode),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "GetExecTechnologyProgressDetailsList()");
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        /// <summary>
        /// save the Physical Road Status Details
        /// </summary>
        /// <param name="progressModel">contains the form data</param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult AddTechnologyProgressDetails(TechnologyDetailsViewModel model)
        {
            string message = string.Empty;
            IExecutionBAL objBAL = new ExecutionBAL();
            int month = 0, year = 0;

            //Change by Avinash15 on 11/04/2019 Relaxation for April Month to 15..Prev it was 10.
            string AprilMonthDay = ConfigurationManager.AppSettings["PHYSICAL_ROAD_PROGRESS_APRIL_MONTH_DAY"];   //15
            int AprilMonthDayValue = Convert.ToInt16(AprilMonthDay);

            string AprilMonth = ConfigurationManager.AppSettings["PHYSICAL_ROAD_PROGRESS_APRIL_MONTH"];  //4
            int AprilMonthValue = Convert.ToInt16(AprilMonth);

            int allowedMonth = (DateTime.Now.Month == AprilMonthValue && DateTime.Now.Day <= AprilMonthDayValue) ? (DateTime.Now.Month - 1) : DateTime.Now.Month;
            int allowedYear = DateTime.Now.Year;

            //int allowedMonth = DateTime.Now.Month, allowedYear = DateTime.Now.Year;

            // Changes START here 
            DateTime FinanDate = DateTime.Now; // change
            int FinancialYear = FinanDate.Month >= 4 ? FinanDate.Year : FinanDate.Year - 1; // change
            DateTime Conditional_Date_Value = new DateTime(FinancialYear, 3, 31, 00, 00, 00);  // change
            int CondFinanYear = FinancialYear;
            //DateTime TestDate = new DateTime(2023, 4, 6, 00, 00, 00);  // change commentable
            //model.Date = TestDate.ToString();  // change commentable
            Nullable<DateTime> ProgressDate = new DateTime();
            if (model.Date != null)
            {
                ProgressDate = Convert.ToDateTime(model.Date);
            }

            Nullable<DateTime> Entry_Date = new DateTime();
            if (ProgressDate != null)
            {
                Entry_Date = (ProgressDate.Value.Day <= AprilMonthDayValue && ProgressDate.Value.Month == AprilMonthValue)
                        ? Conditional_Date_Value : ProgressDate;
            }

            if (FinanDate.Month == AprilMonthValue && FinanDate.Day > AprilMonthDayValue)   // CHANGE
            {
                if (Entry_Date.Value.Year * 12 + Entry_Date.Value.Month <= CondFinanYear * 12 + 3)
                {
                    return Json(new { success = false, message = "Technology Details can be entered in Current Date of Current Financial Year." }, JsonRequestBehavior.AllowGet);
                }
            }
            try
            {
                #region Validation

                if (!string.IsNullOrEmpty(model.Date))
                {
                    year = new CommonFunctions().GetStringToDateTime(model.Date).Year;
                    month = new CommonFunctions().GetStringToDateTime(model.Date).Month;
                }
                ///Changed by SAMMED A. PATIL on 12APR2017 to restrict Technology progress entry for current financial year
                //if (!(model.Year == currYear && model.Month == currMonth))
                //{
                //    return Json(new { success = false, message = "Month and Year must be equal to current month and year." }, JsonRequestBehavior.AllowGet);
                //}

                //Avinash15 April Month Relaxation
                if (DateTime.Now.Month == AprilMonthValue)
                {
                    if (DateTime.Now.Day <= AprilMonthDayValue)
                    {
                        if (!((model.Month == allowedMonth) && model.Year == allowedYear))
                        {
                            return Json(new { success = false, message = "Progress Month and Year must be equal to March and " + DateTime.Now.Year }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        if (!((model.Month == allowedMonth) && model.Year == allowedYear))
                        {
                            return Json(new { success = false, message = "Progress Month and Year must be equal to April and " + DateTime.Now.Year }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                //if (DateTime.Now.Day <= 10)
                else if (DateTime.Now.Day <= 5)
                {
                    if (DateTime.Now.Month == 1)
                    {
                        if (!((model.Month == 12 && model.Year == (allowedYear - 1)) || ((model.Month == allowedMonth) && (model.Year == allowedYear)))
                           )
                        {
                            return Json(new { success = false, message = "Progress Month and Year must be equal to current month and year." }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        if (!(((model.Month == allowedMonth) || (model.Month == (allowedMonth - 1)))
                               && model.Year == allowedYear)
                           )
                        {
                            return Json(new { success = false, message = "Progress Month and Year must be equal to current month and year." }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                else
                {
                    if (!(model.Month == allowedMonth && model.Year == allowedYear))
                    {
                        return Json(new { success = false, message = "Progress Month and Year must be equal to current month and year." }, JsonRequestBehavior.AllowGet);
                    }
                }

                if (!(model.Year == year && model.Month == month))
                {
                    return Json(new { success = false, message = "Please select date for Selected Year and Month" });
                }
                if (model.previousYear > 0)
                {
                    if (model.previousYear > model.Year)
                    {
                        return Json(new { success = false, message = "Selected Year should be greater than the previous Year" });
                    }
                }

                if (model.previousMonth > 0)
                {
                    if (model.previousYear == model.Year)
                    {
                        if (model.previousMonth >= model.Month)
                        {
                            return Json(new { success = false, message = "Selected Month should be greater than the previous month" });
                        }
                    }
                }

                if (!string.IsNullOrEmpty(model.previousDate))
                {
                    if (new CommonFunctions().GetStringToDateTime(model.previousDate) > new CommonFunctions().GetStringToDateTime(model.Date))
                    {
                        return Json(new { success = false, message = "Selected Date should be greater than the previous Date" });
                    }
                }

                if (model.previousCompletedLength > 0)
                {
                    if (model.previousCompletedLength > model.completedLength)
                    {
                        if (model.Status == "C")
                        {
                            return Json(new { success = false, message = "Please enter Completed Length greater than the previous length" });
                        }
                        if (model.Status == "P")
                        {
                            return Json(new { success = false, message = "Please enter In-Progress Length greater than the previous length" });
                        }
                    }
                }

                if (model.completedLength > model.sanctionLength)
                {
                    if (model.Status == "C")
                    {
                        return Json(new { success = false, message = "Completed Length cannot be greater than sanction length" });
                    }
                    if (model.Status == "P")
                    {
                        return Json(new { success = false, message = "Progress Length cannot be greater than sanction length" });
                    }
                }

                //if ((model.previousCompletedLength + model.completedLength) > model.sanctionLength)
                //{
                //    if (model.Status == "P")
                //    {
                //        return Json(new { success = false, message = "Total Progress Length exceeding the sanction length" });
                //    }
                //    if (model.Status == "C")
                //    {
                //        return Json(new { success = false, message = "Total Completed Length exceeding the sanction length" });
                //    }
                //}
                #endregion
                if (ModelState.IsValid)
                {
                    message = objBAL.AddExecTechnologyProgressDetailsBAL(model);
                    //if (objBAL.AddExecTechnologyProgressDetailsBAL(model) == "")
                    //{
                    if (message == string.Empty)
                    {
                        return Json(new { success = true, message = "Execution Technology Progress Details Added Successfully" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        message = message == "" ? "Execution Technology Progress Details Not Added" : message;
                        return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    //}
                    //else
                    //{
                    //    return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
                    //}
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "AddTechnologyProgressDetails()");
                return Json(new { success = false, message = "Error occurred while processing the request." }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// updates the physical road details
        /// </summary>
        /// <param name="progressModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult EditTechnologyProgressDetails(TechnologyDetailsViewModel model)
        {
            IExecutionBAL objBAL = new ExecutionBAL();
            string message = string.Empty;
            int year = 0, month = 0;

            //Change by Avinash15 on 11/04/2019 Relaxation for April Month to 15..Prev it was 10.
            string AprilMonthDay = ConfigurationManager.AppSettings["PHYSICAL_ROAD_PROGRESS_APRIL_MONTH_DAY"];   //15
            int AprilMonthDayValue = Convert.ToInt16(AprilMonthDay);

            string AprilMonth = ConfigurationManager.AppSettings["PHYSICAL_ROAD_PROGRESS_APRIL_MONTH"];  //4
            int AprilMonthValue = Convert.ToInt16(AprilMonth);

            int allowedMonth = (DateTime.Now.Month == AprilMonthValue && DateTime.Now.Day <= AprilMonthDayValue) ? (DateTime.Now.Month - 1) : DateTime.Now.Month;
            int allowedYear = DateTime.Now.Year;

            //int allowedYear = DateTime.Now.Year, allowedMonth = DateTime.Now.Month;
            try
            {
                #region Validation

                if (!string.IsNullOrEmpty(model.Date))
                {
                    year = new CommonFunctions().GetStringToDateTime(model.Date).Year;
                    month = new CommonFunctions().GetStringToDateTime(model.Date).Month;
                }
                ///Changed by SAMMED A. PATIL on 12APR2017 to restrict Technology progress entry for current financial year
                //if (!(model.Year == currYear && model.Month == currMonth))
                //{
                //    return Json(new { success = false, message = "Month and Year must be equal to current month and year." }, JsonRequestBehavior.AllowGet);
                //}

                //Change by Avinash15 on 11/04/2019 Relaxation for April Month to 15..Prev it was 10.
                if (DateTime.Now.Month == AprilMonthValue)
                {
                    if (DateTime.Now.Day <= AprilMonthDayValue)
                    {
                        if (!((model.Month == allowedMonth) && model.Year == allowedYear))
                        {
                            return Json(new { success = false, message = "Progress Month and Year must be equal to March and " + DateTime.Now.Year }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        if (!((model.Month == allowedMonth) && model.Year == allowedYear))
                        {
                            return Json(new { success = false, message = "Progress Month and Year must be equal to April and " + DateTime.Now.Year }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                //if (DateTime.Now.Day <= 10)
                else if (DateTime.Now.Day <= 5)
                {
                    if (DateTime.Now.Month == 1)
                    {
                        if (!((model.Month == 12 && model.Year == (allowedYear - 1)) || ((model.Month == allowedMonth) && (model.Year == allowedYear)))
                           )
                        {
                            return Json(new { success = false, message = "Progress Month and Year must be equal to current month and year." }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        if (!(((model.Month == allowedMonth) || (model.Month == (allowedMonth - 1)))
                               && model.Year == allowedYear)
                           )
                        {
                            return Json(new { success = false, message = "Progress Month and Year must be equal to current month and year." }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                else
                {
                    if (!(model.Month == allowedMonth && model.Year == allowedYear))
                    {
                        return Json(new { success = false, message = "Progress Month and Year must be equal to current month and year." }, JsonRequestBehavior.AllowGet);
                    }
                }

                if (!(model.Year == year && model.Month == month))
                {
                    return Json(new { success = false, message = "Please select date for Selected Year and Month" });
                }

                if (model.previousYear > 0)
                {
                    if (model.previousYear > model.Year)
                    {
                        return Json(new { success = false, message = "Selected Year should be greater than the previous Year" });
                    }
                }

                if (model.previousMonth > 0)
                {
                    if (model.previousYear == model.Year)
                    {
                        if (model.previousMonth > model.Month)
                        {
                            return Json(new { success = false, message = "Selected Month should be greater than the previous month" });
                        }
                    }
                }

                if (!string.IsNullOrEmpty(model.previousDate))
                {
                    if (new CommonFunctions().GetStringToDateTime(model.previousDate) > new CommonFunctions().GetStringToDateTime(model.Date))
                    {
                        return Json(new { success = false, message = "Selected Date should be greater than the previous Date" });
                    }
                }

                if (model.previousCompletedLength > 0)
                {
                    if (model.previousCompletedLength > model.completedLength)
                    {
                        if (model.Status == "C")
                        {
                            return Json(new { success = false, message = "Please enter Completed Length greater than the previous length" });
                        }
                        if (model.Status == "P")
                        {
                            return Json(new { success = false, message = "Please enter In-Progress Length greater than the previous length" });
                        }
                    }
                }

                if (model.completedLength > model.sanctionLength)
                {
                    if (model.Status == "C")
                    {
                        return Json(new { success = false, message = "Completed Length cannot be greater than sanction length" });
                    }
                    if (model.Status == "P")
                    {
                        return Json(new { success = false, message = "Progress Length cannot be greater than sanction length" });
                    }
                }

                //if ((model.previousCompletedLength + model.completedLength) > model.sanctionLength)
                //{
                //    if (model.Status == "P")
                //    {
                //        return Json(new { success = false, message = "Total Progress Length exceeding the sanction length" });
                //    }
                //    if (model.Status == "C")
                //    {
                //        return Json(new { success = false, message = "Total Completed Length exceeding the sanction length" });
                //    }
                //}
                #endregion
                if (ModelState.IsValid)
                {
                    if (!objBAL.EditExecTechnologyProgressDetailsBAL(model, ref message))
                    {
                        return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "EditTechnologyProgressDetails()");
                return null;
            }
        }

        /// <summary>
        /// delete CDWorks details 
        /// </summary>
        /// <param name="parameter">Encrypted id</param>
        /// <param name="hash">Encrypted id</param>
        /// <param name="key">Encrypted id</param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult DeleteTechnologyDetailsViewModel(String parameter, String hash, String key)
        {
            IExecutionBAL objBAL = new ExecutionBAL();
            string message = string.Empty;
            Dictionary<string, string> decryptedParameters = null;
            String[] encryptedParameters = null;
            int proposalCode = 0;
            int techMonhtlyCode = 0;
            try
            {
                //encryptedParameters = id.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                proposalCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);
                techMonhtlyCode = Convert.ToInt32(decryptedParameters["TechMonthlyCode"]);
                if (!(objBAL.DeleteExecTechnologyProgressDetailsBAL(techMonhtlyCode, ref message)))
                {
                    if (message == string.Empty)
                    {
                        return Json(new { success = false, message = "CDWorks details deleted successfully." });
                    }
                    else
                    {
                        return Json(new { success = false, message = message });
                    }
                }
                else
                {
                    return Json(new { success = true, message = message });
                }

            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "EditTechnologyProgressDetails()");
                return Json(new { success = false, message = "Error Occurred while processing your request." });
            }
        }
        #endregion

        #region File Upload

        //image upload View
        [HttpGet]
        [Audit]
        public ActionResult ImageUpload(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                dbContext = new PMGSYEntities();

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                int imsPrRoadCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);

                if (decryptedParameters.Count() > 0)
                {
                    PMGSY.Models.Execution.FileUploadViewModel fileUploadViewModel = new PMGSY.Models.Execution.FileUploadViewModel();
                    fileUploadViewModel.IMS_PR_ROAD_CODE = imsPrRoadCode;
                    fileUploadViewModel.lstHeadItems = new List<SelectListItem>();

                    fileUploadViewModel.lstHeadItems = new SelectList(dbContext.MASTER_EXECUTION_ITEM.Where(m => m.MAST_HEAD_TYPE == "I").Select(m => new { Value = m.MAST_HEAD_CODE, Text = m.MAST_HEAD_DESC }).ToList(), "Value", "Text").ToList();
                    fileUploadViewModel.lstHeadItems.Insert(0, new SelectListItem { Value = "0", Text = "Select Stage" });

                    if (dbContext.EXEC_FILES.Where(a => a.IMS_PR_ROAD_CODE == fileUploadViewModel.IMS_PR_ROAD_CODE).Any())
                    {
                        fileUploadViewModel.NumberofFiles = dbContext.EXEC_FILES.Where(a => a.IMS_PR_ROAD_CODE == fileUploadViewModel.IMS_PR_ROAD_CODE && a.EXEC_FILE_TYPE == 0).Count();
                    }
                    else
                    {
                        fileUploadViewModel.NumberofFiles = 0;
                    }
                    return View("ImageUpload", fileUploadViewModel);
                }
                return View("ImageUpload", new PMGSY.Models.Execution.FileUploadViewModel());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return View("ImageUpload", new PMGSY.Models.Execution.FileUploadViewModel());
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        //list uploaded Images
        [Audit]
        public JsonResult ListFiles(FormCollection formCollection)
        {
            //Added By Abhishek kamble 29-Apr-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Added By Abhishek kamble 29-Apr-2014 start

            IExecutionBAL objExecutionBAL = new ExecutionBAL();

            int IMS_PR_ROAD_CODE = Convert.ToInt32(Request["IMS_PR_ROAD_CODE"]);
            int totalRecords;
            var jsonData = new
            {
                rows = objExecutionBAL.GetFilesListBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, IMS_PR_ROAD_CODE),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                page = Convert.ToInt32(formCollection["page"]),
                records = totalRecords
            };
            return Json(jsonData);
        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        [Audit]
        public ActionResult Uploads(PMGSY.Models.Execution.FileUploadViewModel fileUploadViewModel)
        {
            //Added By Abhishek kamble 26-Apr-2014 to validate File Type
            CommonFunctions objCommonFunc = new CommonFunctions();
            //Array of File Types to Validate             
            String[] fileTypes = new String[] { "jpeg", String.Empty, String.Empty };
            if (!(objCommonFunc.IsValidImageFile(ConfigurationManager.AppSettings["EXECUTION_PRGRESS_FILE_UPLOAD"], Request, fileTypes)))
            {
                fileUploadViewModel.ErrorMessage = "File Type is Not Allowed.";
                return View("ImageUpload", fileUploadViewModel.ErrorMessage);
            }

            var fileData = new List<PMGSY.Models.Execution.FileUploadViewModel>();
            int IMS_PR_ROAD_CODE = 0;
            if (fileUploadViewModel.IMS_PR_ROAD_CODE != 0)
            {
                IMS_PR_ROAD_CODE = fileUploadViewModel.IMS_PR_ROAD_CODE;
            }
            else
            {
                try
                {
                    IMS_PR_ROAD_CODE = Convert.ToInt32(Request["IMS_PR_ROAD_CODE"]);
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                    if (Request["IMS_PR_ROAD_CODE"].Contains(','))
                    {
                        IMS_PR_ROAD_CODE = Convert.ToInt32(Request["IMS_PR_ROAD_CODE"].Split(',')[0]);
                    }
                }
            }

            string errorMessage = string.Empty;
            string latitude = string.Empty;
            string longitude = string.Empty;
            HttpPostedFileBase file1 = Request.Files[0];
            foreach (string file in Request.Files)
            {

                UploadImageFile(Request, fileData, IMS_PR_ROAD_CODE);

                //bool check = ReadGeoPositions(file1, out errorMessage, out latitude, out longitude);
                //if (check)
                //{
                //    decimal lati = Convert.ToDecimal(latitude);
                //    decimal longi = Convert.ToDecimal(longitude);
                //    HttpPostedFileBase file2 = Request.Files[0];

                //}
                //else
                //{
                //    fileUploadViewModel.ErrorMessage = errorMessage;
                //    return View("ImageUpload", fileUploadViewModel.ErrorMessage);
                //}
            }

            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;

            var result = new ContentResult
            {
                Content = "{\"files\":" + serializer.Serialize(fileData) + "}",
            };
            return result;
        }

        [Audit]
        public void UploadImageFile(HttpRequestBase request, List<PMGSY.Models.Execution.FileUploadViewModel> statuses, int IMS_PR_ROAD_CODE)
        {
            try
            {
                dbContext = new PMGSYEntities();
                IExecutionBAL objExecutionBAL = new ExecutionBAL();
                HttpRequestBase newRequest = request;
                String StorageRoot = ConfigurationManager.AppSettings["EXECUTION_PRGRESS_FILE_UPLOAD"];
                int MaxCount = 0;
                //decimal latitude = 0;decimal longitude= 0;
                string errorMessage = string.Empty;
                string lati = string.Empty;
                string longi = string.Empty;
                for (int i = 0; i < request.Files.Count; i++)
                {
                    HttpPostedFileBase file = request.Files[i];

                    //bool check = ReadGeoPositions(file, out errorMessage, out lati, out longi);

                    //if (check)
                    //{

                    //}

                    #region LAT_LONG_CALCULATION

                    Double[] latitudeRef = null;
                    Double[] longitudeRef = null;
                    StringBuilder strLat = new StringBuilder();
                    StringBuilder strLong = new StringBuilder();

                    using (ExifReader reader = new ExifReader(file.InputStream, true))
                    {
                        reader.GetTagValue<Double[]>(ExifTags.GPSLatitude, out latitudeRef);
                        reader.GetTagValue<Double[]>(ExifTags.GPSLongitude, out longitudeRef);

                        if (latitudeRef != null && longitudeRef != null)
                        {
                            //return Json(new { success = true, latitude = latitude, longitude = longitude });

                            for (int value = 0; value < latitudeRef.Count(); value++)
                            {
                                if (value == 0)
                                {
                                    strLat.Append(latitudeRef[value].ToString() + ".");
                                }
                                else
                                {
                                    if (latitudeRef[value].ToString().Contains("."))
                                    {
                                        strLat.Append(latitudeRef[value].ToString().Replace(".", string.Empty).ToString());
                                    }
                                    else
                                    {
                                        strLat.Append(latitudeRef[value].ToString());
                                    }

                                }
                            }


                            for (int value = 0; value < longitudeRef.Count(); value++)
                            {
                                if (value == 0)
                                {
                                    strLong.Append(longitudeRef[value].ToString() + ".");
                                }
                                else
                                {
                                    if (longitudeRef[value].ToString().Contains("."))
                                    {
                                        strLong.Append(longitudeRef[value].ToString().Replace(".", string.Empty).ToString());
                                    }
                                    else
                                    {
                                        strLong.Append(longitudeRef[value].ToString());
                                    }
                                }
                            }
                        }
                    }

                    #endregion



                    int contentLength = file.ContentLength;
                    var fileId = IMS_PR_ROAD_CODE;
                    if (dbContext.EXEC_FILES.Where(a => a.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Any())
                    {
                        //new change done on 26-08-2013
                        //MaxCount = dbContext.EXEC_FILES.Where(a => a.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Count();
                        MaxCount = dbContext.EXEC_FILES.Where(m => m.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).OrderByDescending(m => m.EXEC_FILE_ID).Select(m => m.EXEC_FILE_ID).FirstOrDefault();
                    }
                    MaxCount++;

                    var fileName = IMS_PR_ROAD_CODE + "-" + MaxCount + Path.GetExtension(request.Files[i].FileName).ToString();
                    var fullPath = Path.Combine(StorageRoot, fileName);

                    var ThumbnailPath = Path.Combine(StorageRoot, "thumbnails");
                    var FullThumbnailPath = Path.Combine(ThumbnailPath, fileName);

                    statuses.Add(new PMGSY.Models.Execution.FileUploadViewModel()
                    {
                        url = fullPath,
                        thumbnail_url = fullPath,
                        name = fileName,
                        type = file.ContentType,
                        size = file.ContentLength,
                        delete_url = "",
                        delete_type = "DELETE",
                        file_type = 0,
                        Image_Description = request.Params["remark[]"],
                        IMS_PR_ROAD_CODE = IMS_PR_ROAD_CODE,
                        status = "P",
                        HeadItem = Convert.ToInt32(request.Params["stage[]"]),
                        //Latitude = latitude,
                        //Longitude = longitude
                        Latitude = Convert.ToDecimal(strLat.ToString()),
                        Longitude = Convert.ToDecimal(strLong.ToString())
                    });
                    string status = objExecutionBAL.AddFileUploadDetailsBAL(statuses);
                    if (status == string.Empty)
                    {
                        objExecutionBAL.CompressImage(newRequest.Files[0], fullPath, FullThumbnailPath);
                        //file.SaveAs(fullPath);
                    }
                    else
                    {
                        // show an error over here
                    }
                }
                //return Json(new { status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                throw ex;
                //return Json(new { status=false},JsonRequestBehavior.AllowGet);
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        [HttpGet]
        [Audit]
        public ActionResult DownloadFile(String parameter, String hash, String key)
        {
            try
            {
                string FileName = string.Empty;
                string FullFileLogicalPath = string.Empty;
                string FullfilePhysicalPath = string.Empty;
                string FileExtension = string.Empty;

                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        FileName = (urlSplitParams[0]);

                    }
                }
                FileExtension = Path.GetExtension(FileName).ToLower();

                if (FileExtension == ".jpg" || FileExtension == ".jpeg" || FileExtension == ".bmp" || FileExtension == ".png" || FileExtension == ".gif" || FileExtension == ".tiff" || FileExtension == ".wmv")
                {
                    FullFileLogicalPath = Path.Combine(ConfigurationManager.AppSettings["EXECUTION_PRGRESS_FILE_UPLOAD_VIRTUAL_DIR_PATH"], FileName);
                    FullfilePhysicalPath = Path.Combine(ConfigurationManager.AppSettings["EXECUTION_PRGRESS_FILE_UPLOAD"], FileName);
                }

                string name = Path.GetFileName(FileName);
                string ext = Path.GetExtension(FileName);

                string type = string.Empty;

                if (ext != null)
                {
                    switch (ext.ToLower())
                    {
                        case ".jpg":
                        case ".bmp":
                        case ".tiff":
                        case ".png":
                        case ".gif":
                        case ".jpeg":
                            type = "image/png";
                            break;
                        default:
                            type = "Application";
                            break;
                    }
                }

                if (System.IO.File.Exists(FullfilePhysicalPath))
                {
                    return File(FullfilePhysicalPath, type, DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + FileExtension);
                }
                else
                {
                    return Json(new { message = "No File Found" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DownloadFile()");
                return Json(new { Success = "false", message = "Error occured on File Download" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Audit]
        public JsonResult UpdateImageDetails(FormCollection formCollection)
        {
            IExecutionBAL objExecutionBAL = new ExecutionBAL();
            try
            {
                string[] arrKey = formCollection["id"].Split('$');
                PMGSY.Models.Execution.FileUploadViewModel fileuploadViewModel = new PMGSY.Models.Execution.FileUploadViewModel();
                fileuploadViewModel.IMS_PR_ROAD_CODE = Convert.ToInt32(arrKey[1]);
                fileuploadViewModel.EXEC_FILE_ID = Convert.ToInt32(arrKey[0]);

                Regex regex = new Regex(@"^[a-zA-Z0-9 ]+$");
                if (regex.IsMatch(formCollection["Description"]))
                {
                    fileuploadViewModel.Image_Description = formCollection["Description"];
                }
                else
                {
                    return Json("Invalid Image Description, Only Alphabets and Numbers are allowed");
                }

                string status = objExecutionBAL.UpdateImageDetailsBAL(fileuploadViewModel);

                if (status == string.Empty)
                    return Json(true);
                else
                    return Json("There is an error occured while processing your request.");
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "UpdateImageDetails()");
                return Json(new { Success = "false", message = "Error occured on Update Image Details" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Audit]
        public JsonResult DeleteFileDetails(string id)
        {
            IExecutionBAL objExecutionBAL = new ExecutionBAL();
            try
            {
                String PhysicalPath = string.Empty;
                String ThumbnailPath = string.Empty;
                string EXEC_FILE_NAME = Request.Params["IMS_FILE_NAME"];
                PhysicalPath = ConfigurationManager.AppSettings["EXECUTION_PRGRESS_FILE_UPLOAD"];
                ThumbnailPath = Path.Combine(Path.Combine(PhysicalPath, "thumbnails"), EXEC_FILE_NAME);

                string[] arrParam = Request.Params["IMS_PR_ROAD_CODE"].Split('$');

                int EXEC_FILE_ID = Convert.ToInt32(arrParam[0]);
                int IMS_PR_ROAD_CODE = Convert.ToInt32(arrParam[1]);

                PhysicalPath = Path.Combine(PhysicalPath, EXEC_FILE_NAME);

                if (!System.IO.File.Exists(PhysicalPath) && !System.IO.File.Exists(ThumbnailPath))
                {
                    //return Json(new { Success = false, ErrorMessage = "file Not Found." });
                }

                string status = objExecutionBAL.DeleteFileDetails(EXEC_FILE_ID, IMS_PR_ROAD_CODE, EXEC_FILE_NAME);

                if (status == string.Empty)
                {
                    try
                    {
                        System.IO.File.Delete(PhysicalPath);
                        System.IO.File.Delete(ThumbnailPath);
                    }
                    catch (Exception ex)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                        return Json(new { Success = true, ErrorMessage = ex.Message });
                    }
                    return Json(new { Success = true, ErrorMessage = "There is an error while processing your request." });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = status });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DeleteFileDetails()");
                return Json(new { Success = "false", message = "Error occured on Delete File Details" }, JsonRequestBehavior.AllowGet);
            }
        }

        //Video Upload


        [HttpGet]
        [Audit]
        public ActionResult FileUpload(String hash, String parameter, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            int coreNetworkCode = 0;
            FileUploadViewModel fileUploadViewModel = new FileUploadViewModel();
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptedParameters.Count() > 0)
                {
                    coreNetworkCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);
                    fileUploadViewModel.IMS_PR_ROAD_CODE = coreNetworkCode;
                    fileUploadViewModel.Urlparameter = URLEncrypt.EncryptParameters1(new string[] { "ProposalCode=" + fileUploadViewModel.IMS_PR_ROAD_CODE.ToString().Trim() });
                    return PartialView("FileUpload", fileUploadViewModel);
                }
                else
                {
                    return PartialView("FileUpload", new FileUploadViewModel());
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return PartialView("FileUpload", new FileUploadViewModel());
            }

        }


        [HttpGet]
        [Audit]
        public ActionResult VideoUpload(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                dbContext = new PMGSYEntities();

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                int imsPrRoadCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);

                if (decryptedParameters.Count() > 0)
                {
                    PMGSY.Models.Execution.FileUploadViewModel fileUploadViewModel = new PMGSY.Models.Execution.FileUploadViewModel();
                    fileUploadViewModel.IMS_PR_ROAD_CODE = imsPrRoadCode;

                    if (dbContext.EXEC_FILES.Where(a => a.IMS_PR_ROAD_CODE == fileUploadViewModel.IMS_PR_ROAD_CODE).Any())
                    {
                        fileUploadViewModel.NumberofFiles = dbContext.EXEC_FILES.Where(a => a.IMS_PR_ROAD_CODE == fileUploadViewModel.IMS_PR_ROAD_CODE && a.EXEC_FILE_TYPE == 1).Count();
                    }
                    else
                    {
                        fileUploadViewModel.NumberofFiles = 0;
                    }
                    return View("VideoUpload", fileUploadViewModel);
                }
                return View("VideoUpload", new PMGSY.Models.Execution.FileUploadViewModel());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return View("VideoUpload", new PMGSY.Models.Execution.FileUploadViewModel());
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult KmlFileUpload(FileUploadViewModel fileUploadViewModel)
        {
            try
            {
                var fileData = new List<FileUploadViewModel>();

                int roadCode = 0;
                if (fileUploadViewModel.IMS_PR_ROAD_CODE != 0)
                {
                    roadCode = fileUploadViewModel.IMS_PR_ROAD_CODE;
                }
                else
                {
                    try
                    {
                        roadCode = Convert.ToInt32(Request["IMS_PR_ROAD_CODE"]);
                    }
                    catch (Exception ex)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                        if (Request["IMS_PR_ROAD_CODE"].Contains(','))
                        {
                            roadCode = Convert.ToInt32(Request["IMS_PR_ROAD_CODE"].Split(',')[0]);
                        }
                    }
                }

                foreach (string file in Request.Files)
                {
                    UploadPDFFile(Request, fileData, roadCode);
                }

                var serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;
                var result = new ContentResult
                {
                    Content = "{\"files\":" + serializer.Serialize(fileData) + "}",
                };
                return result;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occurred while processing the request." });
            }

        }

        /// <summary>
        /// method for uploading the file
        /// </summary>
        /// <param name="request"></param>
        /// <param name="statuses"></param>
        /// <param name="roadCode"></param>
        public void UploadPDFFile(HttpRequestBase request, List<FileUploadViewModel> statuses, int roadCode)
        {
            dbContext = new PMGSYEntities();
            try
            {
                ExecutionBAL objBAL = new ExecutionBAL();
                String StorageRoot = ConfigurationManager.AppSettings["EXECUTION_PRGRESS_FILE_UPLOAD"];
                int MaxCount = 0;

                for (int i = 0; i < request.Files.Count; i++)
                {
                    HttpPostedFileBase file = request.Files[i];
                    var fileId = roadCode;
                    if (dbContext.EXEC_FILES.Where(a => a.IMS_PR_ROAD_CODE == roadCode).Any())
                    {
                        MaxCount = dbContext.EXEC_FILES.Where(a => a.IMS_PR_ROAD_CODE == roadCode).Count();
                    }
                    MaxCount++;

                    var fileName = roadCode + "-" + MaxCount + Path.GetExtension(request.Files[i].FileName).ToString();
                    var fullPath = Path.Combine(StorageRoot, fileName);

                    statuses.Add(new FileUploadViewModel()
                    {
                        url = fullPath,
                        thumbnail_url = fullPath,
                        name = fileName,
                        type = file.ContentType,
                        size = file.ContentLength,
                        delete_url = "",
                        file_type = 1,
                        delete_type = "DELETE",
                        Image_Description = request.Params["remark[]"],
                        IMS_PR_ROAD_CODE = roadCode
                    });

                    string status = objBAL.AddVideoUploadDetailsBAL(statuses);
                    if (status == string.Empty)
                    {

                        //file.SaveAs(fullPath);
                        file.SaveAs(Path.Combine(ConfigurationManager.AppSettings["EXECUTION_PRGRESS_FILE_UPLOAD"], fileName));
                    }
                    else
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

            }

        }

        [Audit]
        public JsonResult ListVideoFiles(FormCollection formCollection)
        {
            IExecutionBAL objExecutionBAL = new ExecutionBAL();

            //Added By Abhishek kamble 29-Apr-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Added By Abhishek kamble 29-Apr-2014 start

            int IMS_PR_ROAD_CODE = Convert.ToInt32(Request["IMS_PR_ROAD_CODE"]);
            int totalRecords;
            var jsonData = new
            {
                rows = objExecutionBAL.GetVideoFilesListBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, IMS_PR_ROAD_CODE),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                page = Convert.ToInt32(formCollection["page"]),
                records = totalRecords
            };
            return Json(jsonData);
        }

        #endregion File Upload

        #region Executing Officers

        /// <summary>
        /// ListExecutingOfficerDetails() action is used to display Executing Officer List add Executing Officer Add/Edit Form
        /// </summary>
        /// <param name="urlparameter">
        /// urlparameter contails IMS_PR_ROAD_CODE
        /// </param>
        /// <returns></returns>
        [Audit]
        public ActionResult ListExecutingOfficerDetails(string urlparameter)
        {
            int proposalCode = 0;
            String[] encryptedParameters = null;
            Dictionary<string, string> decryptedParameters = null;
            ExecutionDAL objDAL = new ExecutionDAL();

            if (urlparameter != string.Empty)
            {
                encryptedParameters = urlparameter.Split('/');
                if (encryptedParameters.Length == 3)
                {
                    decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                }
            }

            proposalCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);
            ExecutingOfficerViewModel executingOfficerViewModel = objDAL.GetExecutingOfficerRoadDetails(proposalCode);
            if (executingOfficerViewModel == null)
            {
                executingOfficerViewModel = new ExecutingOfficerViewModel();
            }
            executingOfficerViewModel.IMS_PR_ROAD_CODE = proposalCode;
            return View("ListExecutingOfficerDetails", executingOfficerViewModel);
        }

        /// <summary>
        /// list executing Officer Details
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult GetExecutingOfficerList(int? page, int? rows, string sidx, string sord)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }

            //Added By Abhishek kamble 29-Apr-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Added By Abhishek kamble 29-Apr-2014 start

            IExecutionBAL objBAL = new ExecutionBAL();
            long totalRecords = 0;
            int imsRoadCode = 0;

            if (!(string.IsNullOrEmpty(Request.Params["roadCode"])))
            {
                imsRoadCode = Convert.ToInt32(Request.Params["roadCode"]);
            }

            var jsonData = new
            {
                rows = objBAL.GetExecutingOfficerListBAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, imsRoadCode),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords,
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// list Technology Progress Details
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult GetTechnologyProgressList(int? page, int? rows, string sidx, string sord)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }

            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }

            IExecutionBAL objBAL = new ExecutionBAL();
            long totalRecords = 0;
            int imsRoadCode = 0;

            if (!(string.IsNullOrEmpty(Request.Params["roadCode"])))
            {
                imsRoadCode = Convert.ToInt32(Request.Params["roadCode"]);
            }

            var jsonData = new
            {
                rows = objBAL.GetTechnologyProgressListBAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, imsRoadCode),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords,
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// AddExecutingOfficerDetails() action is used to Display Executing Officer Data Entery Form
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult AddExecutingOfficerDetails(string id)
        {
            try
            {
                ExecutionDAL objDAL = new ExecutionDAL();
                int proposalCode = Convert.ToInt32(id);
                ExecutingOfficerViewModel executingOfficerViewModel = new ExecutingOfficerViewModel();
                executingOfficerViewModel = objDAL.GetAgreementDetails(proposalCode);
                executingOfficerViewModel.IMS_PR_ROAD_CODE = proposalCode;
                CommonFunctions objCommon = new CommonFunctions();
                executingOfficerViewModel.Operation = "A";
                //executingOfficerViewModel.EXEC_PROGRESS_TYPE = "P";
                ViewData["Year"] = objCommon.PopulateYears(true);
                ViewData["Month"] = objCommon.PopulateMonths();
                ViewData["Designation"] = objDAL.PopulateDesignation();
                ViewData["ExecutingOfficer"] = objDAL.PopulateExecutingOfficer(String.Empty);
                return View("AddExecutingOfficerDetails", executingOfficerViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        /// <summary>
        ///  AddExecutingOfficerDetails() action is used to Save Executing Officer Details
        /// </summary>
        /// <param name="executingOfficerViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult AddExecutingOfficerDetails(ExecutingOfficerViewModel executingOfficerViewModel)
        {
            IExecutionBAL objBAL = new ExecutionBAL();
            string message = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    if (!(objBAL.AddExecutingOfficerDetails(executingOfficerViewModel, ref message)))
                    {
                        if (message == string.Empty)
                        {
                            return Json(new { success = false, message = "Executing Officer details not added." });
                        }
                        else
                        {
                            return Json(new { success = false, message = message });
                        }
                    }
                    else
                    {
                        return Json(new { success = true, message = message });
                    }
                }
                else
                {
                    string messages = string.Join("<br/>", ModelState.Values
                                            .SelectMany(x => x.Errors)
                                            .Select(x => x.ErrorMessage));
                    return Json(new { success = false, message = messages }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Executing Officer details not added." });
            }
        }

        /// <summary>
        /// AddExecutingOfficerDetails() action is used to display Executing Officer Form in Edit Mode
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult EditExecutingOfficerDetails(String parameter, String hash, String key)
        {
            try
            {
                Dictionary<string, string> decryptedParameters = null;
                CommonFunctions objCommon = new CommonFunctions();
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });

                IExecutionBAL objBAL = new ExecutionBAL();
                ExecutionDAL objDAL = new ExecutionDAL();

                int proposalCode = 0;   //for storing decrypted proposal code
                int ExecutingOfficerCode = 0;  //for storing decrypted month code

                if (decryptedParameters != null)
                {
                    proposalCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);
                    ExecutingOfficerCode = Convert.ToInt32(decryptedParameters["OfficerCode"]);
                }
                ViewData["Year"] = objCommon.PopulateYears(true);
                ViewData["Month"] = objCommon.PopulateMonths();
                ViewData["Designation"] = objDAL.PopulateDesignation();
                ViewData["ExecutingOfficer"] = objDAL.PopulateExecutingOfficer(String.Empty);

                ExecutingOfficerViewModel executingOfficerViewModel = objBAL.GetExecutingOfficerDetails(proposalCode, ExecutingOfficerCode);

                if (executingOfficerViewModel != null)
                {
                    return PartialView("AddExecutingOfficerDetails", executingOfficerViewModel);
                }
                else
                {
                    return Json(new { success = false, message = "Request can not be processed at this time." });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Request can not be processed at this time." });
            }
        }

        /// <summary>
        ///  AddExecutingOfficerDetails() action is used to Update Executing Officer Details
        /// </summary>
        /// <param name="executingOfficerViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult EditExecutingOfficerDetails(ExecutingOfficerViewModel executingOfficerViewModel)
        {
            IExecutionBAL objBAL = new ExecutionBAL();
            string message = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    if (!(objBAL.EditExecutingOfficerDetails(executingOfficerViewModel, ref message)))
                    {
                        if (message == string.Empty)
                        {
                            return Json(new { success = false, message = "Executing Officer details not updated." });
                        }
                        else
                        {
                            return Json(new { success = false, message = message });
                        }
                    }
                    else
                    {
                        return Json(new { success = true, message = message });
                    }
                }
                else
                {
                    string messages = string.Join("<br/>", ModelState.Values
                                            .SelectMany(x => x.Errors)
                                            .Select(x => x.ErrorMessage));

                    return Json(new { success = false, message = messages }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Executing Officer details not updated successfully." });
            }
        }

        /// <summary>
        ///  AddExecutingOfficerDetails() action is used to delete Executing Officer Details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult DeleteExecutingOfficerDetails(String parameter, String hash, String key)
        {
            IExecutionBAL objBAL = new ExecutionBAL();
            string message = string.Empty;
            Dictionary<string, string> decryptedParameters = null;
            int proposalCode = 0;
            int ExecutingOfficerCode = 0;
            string progressType = string.Empty;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                proposalCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);
                ExecutingOfficerCode = Convert.ToInt32(decryptedParameters["OfficerCode"]);

                if ((objBAL.DeleteExecutingOfficerDetails(proposalCode, ExecutingOfficerCode, ref message)))
                {
                    if (message == string.Empty)
                    {
                        return Json(new { success = true, message = "Executing Officer  details deleted successfully." });
                    }
                    else
                    {
                        return Json(new { success = true, message = message });
                    }
                }
                else
                {
                    return Json(new { success = false, message = message });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error Occurred while processing your request." });
            }
        }

        /// <summary>
        /// Method to get Executing Officer By Designation Code
        /// </summary>
        /// <param name="stateCode"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult GetExecutingOfficerByDesig(string imsPrRoadCode_DesignationCode)
        {
            try
            {
                IExecutionDAL objDAL = new ExecutionDAL();
                return Json(new SelectList(objDAL.PopulateExecutingOfficer(imsPrRoadCode_DesignationCode), "Value", "Text"));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(false);
            }
        }//end function GetDistrictsByStateCode

        #endregion

        #region PROPOSAL_RELATED_DETAILS

        [Audit]
        public ActionResult ViewExecutionDetails(string id)
        {
            dbContext = new PMGSYEntities();
            int proposalCode = 0;
            if (int.TryParse(id, out proposalCode))
            {
                ViewBag.ProposalCode = proposalCode;
            }
            string proposalType = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == proposalCode).Select(m => m.IMS_PROPOSAL_TYPE).FirstOrDefault();
            ViewBag.ProposalType = proposalType;
            return PartialView();
        }

        [Audit]
        public ActionResult GetRoadProposalExecutionList(int? page, int? rows, string sidx, string sord)
        {
            //Added By Abhishek kamble 29-Apr-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Added By Abhishek kamble 29-Apr-2014 end

            IExecutionBAL objBAL = new ExecutionBAL();
            long totalRecords = 0;
            int imsRoadCode = 0;

            if (!(string.IsNullOrEmpty(Request.Params["ProposalCode"])))
            {
                imsRoadCode = Convert.ToInt32(Request.Params["ProposalCode"]);
            }

            var jsonData = new
            {
                rows = objBAL.GetRoadProposalExecutionList(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, imsRoadCode),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords,
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [Audit]
        public ActionResult GetLSBProposalExecutionList(int? page, int? rows, string sidx, string sord)
        {
            //Added By Abhishek kamble 29-Apr-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Added By Abhishek kamble 29-Apr-2014 end

            IExecutionBAL objBAL = new ExecutionBAL();
            long totalRecords = 0;
            int imsRoadCode = 0;

            if (!(string.IsNullOrEmpty(Request.Params["ProposalCode"])))
            {
                imsRoadCode = Convert.ToInt32(Request.Params["ProposalCode"]);
            }

            var jsonData = new
            {
                rows = objBAL.GetLSBProposalExecutionList(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, imsRoadCode),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords,
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [Audit]
        public ActionResult GetProposalFinancialList(int? page, int? rows, string sidx, string sord)
        {
            //Added By Abhishek kamble 29-Apr-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Added By Abhishek kamble 29-Apr-2014 end

            IExecutionBAL objBAL = new ExecutionBAL();
            long totalRecords = 0;
            int imsRoadCode = 0;

            if (!(string.IsNullOrEmpty(Request.Params["ProposalCode"])))
            {
                imsRoadCode = Convert.ToInt32(Request.Params["ProposalCode"]);
            }

            var jsonData = new
            {
                rows = objBAL.GetProposalFinancialList(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, imsRoadCode),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords,
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region PHYSICAL_PROGRESS_FOR_ITNO

        /// <summary>
        /// returns the view for listing the proposals at ITNO
        /// </summary>
        /// <returns></returns>
        public ActionResult ListProposalsForITNO()
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                ProposalFilterForITNOViewModel proposalModel = new ProposalFilterForITNOViewModel();
                List<SelectListItem> lstBatches = new List<SelectListItem>();
                TransactionParams transactionParams = new TransactionParams();
                transactionParams.STATE_CODE = PMGSYSession.Current.StateCode;
                transactionParams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                transactionParams.ISSearch = true;
                transactionParams.SANC_YEAR = (Int16)DateTime.Now.Year;
                lstBatches = objCommon.PopulateBatch();
                lstBatches.RemoveAt(0);
                lstBatches.Add(new SelectListItem { Value = "0", Text = "All Batches" });
                proposalModel.BATCHS = lstBatches;
                proposalModel.lstDistricts = objCommon.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                proposalModel.BLOCKS = objCommon.PopulateBlocks(PMGSYSession.Current.DistrictCode, true);
                proposalModel.Years = PopulateYear(0, true, true);
                proposalModel.STREAMS = objCommon.PopulateStreams("", true);
                proposalModel.PROPOSAL_TYPES = objCommon.PopulateProposalTypes();
                proposalModel.lstBatchs = objCommon.PopulateBatch(true);
                proposalModel.lstCollaborations = objCommon.PopulateFundingAgency(true);
                proposalModel.lstUpgradations = objCommon.PopulateNewUpgradeList(true);
                List<SelectListItem> lstPackages = objCommon.PopulatePackage(transactionParams);
                lstPackages.RemoveAt(0);
                lstPackages.Insert(0, new SelectListItem { Value = "All", Text = "All Packages" });
                proposalModel.PACKAGES = lstPackages;
                return View(proposalModel);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// returns the list of execution progress details 
        /// </summary>
        /// <param name="executionCollection">contains the filters and grid parameters</param>
        /// <returns></returns>
        [Audit]
        public ActionResult GetExecutionProgressListForITNO(int? page, int? rows, string sidx, string sord)
        {
            int districtCode = 0;
            int yearCode = 0;
            int blockCode = 0;
            int streamCode = 0;
            int batchCode = 0;
            string proposalCode = string.Empty;
            string packageCode = string.Empty;
            long totalRecords = 0;
            IExecutionBAL objBAL = new ExecutionBAL();

            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }

            if (!string.IsNullOrEmpty(Request.Params["yearCode"]))
            {
                yearCode = Convert.ToInt32(Request.Params["yearCode"]);
            }

            if (!string.IsNullOrEmpty(Request.Params["blockCode"]))
            {
                blockCode = Convert.ToInt32(Request.Params["blockCode"]);
            }

            if (!string.IsNullOrEmpty(Request.Params["districtCode"]))
            {
                districtCode = Convert.ToInt32(Request.Params["districtCode"]);
            }

            if (!string.IsNullOrEmpty(Request.Params["streamCode"]))
            {
                streamCode = Convert.ToInt32(Request.Params["streamCode"]);
            }

            if (!string.IsNullOrEmpty(Request.Params["batchCode"]))
            {
                batchCode = Convert.ToInt32(Request.Params["batchCode"]);
            }

            if (!string.IsNullOrEmpty(Request.Params["proposalCode"]))
            {
                proposalCode = Request.Params["proposalCode"];
            }

            if (!string.IsNullOrEmpty(Request.Params["packageCode"]))
            {
                packageCode = Request.Params["packageCode"];
            }

            var jsonData = new
            {
                rows = objBAL.GetExecutionListForITNO(districtCode, yearCode, blockCode, packageCode, proposalCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords,
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// updates the physical road details
        /// </summary>
        /// <param name="progressModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult UpdateRoadProgressDetails(ProposalFilterForITNOViewModel progressModel)
        {
            IExecutionBAL objBAL = new ExecutionBAL();
            string message = string.Empty;
            try
            {
                Regex regex = new Regex(@"^[\d.]+$");
                //Regex regex = new Regex(@"/^[0-9]+(\.[0-9][0-9]?)?$/");
                //Regex regex = new Regex(@"^(-?[1-9]+\\d*([.]\\d+)?)$|^(-?0[.]\\d*[1-9]+)$|^0$");

                if (!(regex.IsMatch(progressModel.EXEC_PREPARATORY_WORK.ToString().Trim()) /*&& progressModel.EXEC_PREPARATORY_WORK != 0*/))
                {
                    return Json("Preparatory works length should be decimal value upto 2 digits after decimal");
                }
                if (!(regex.IsMatch(progressModel.EXEC_EARTHWORK_SUBGRADE.ToString().Trim()) /*&& progressModel.EXEC_PREPARATORY_WORK != 0*/))
                {
                    return Json("Subgrade Stage length should be decimal value upto 2 digits after decimal");
                }
                if (!(regex.IsMatch(progressModel.EXEC_SUBBASE_PREPRATION.ToString().Trim()) /*&& progressModel.EXEC_PREPARATORY_WORK != 0*/))
                {
                    return Json("Sub base length should be decimal value upto 2 digits after decimal");
                }
                if (!(regex.IsMatch(progressModel.EXEC_BASE_COURSE.ToString().Trim()) /*&& progressModel.EXEC_PREPARATORY_WORK != 0*/))
                {
                    return Json("Base course length should be decimal value upto 2 digits after decimal");
                }
                if (!(regex.IsMatch(progressModel.EXEC_SURFACE_COURSE.ToString().Trim()) /*&& progressModel.EXEC_PREPARATORY_WORK != 0*/))
                {
                    return Json("Surface course length should be decimal value upto 2 digits after decimal");
                }
                if (!(regex.IsMatch(progressModel.EXEC_MISCELANEOUS.ToString().Trim()) /*&& progressModel.EXEC_PREPARATORY_WORK != 0*/))
                {
                    return Json("Misscelaneous length should be decimal value upto 2 digits after decimal");
                }
                if (!(regex.IsMatch(progressModel.EXEC_COMPLETED.ToString().Trim()) /*&& progressModel.EXEC_PREPARATORY_WORK != 0*/))
                {
                    return Json("Completed length should be decimal value upto 2 digits after decimal");
                }

                Regex regex1 = new Regex(@"^[\d]+$");
                if (!(regex.IsMatch(progressModel.EXEC_SIGNS_STONES.ToString().Trim()) /*&& progressModel.EXEC_PREPARATORY_WORK != 0*/))
                {
                    return Json("Road signs stones should be a number");
                }
                if (!(regex.IsMatch(progressModel.EXEC_CD_WORKS.ToString().Trim()) /*&& progressModel.EXEC_PREPARATORY_WORK != 0*/))
                {
                    return Json("CD Works should be a number");
                }
                if (!(regex.IsMatch(progressModel.EXEC_LSB_WORKS.ToString().Trim()) /*&& progressModel.EXEC_PREPARATORY_WORK != 0*/))
                {
                    return Json("LSB Works should be a number");
                }

                if (ModelState.IsValid)
                {
                    if (Request.Params["id"] != null)
                    {
                        string[] id = Request.Params["id"].Split('$');
                        progressModel.ProposalCode = Convert.ToInt32(id[0]);
                        progressModel.EXEC_PROG_YEAR = progressModel.YEAR;
                        progressModel.EXEC_PROG_MONTH = progressModel.MONTH;
                    }
                    if (!objBAL.UpdateRoadProgressDetailsITNO(progressModel, ref message))
                    {
                        //return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                        return Json(message, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(true);
                        //return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    foreach (var modelStateValue in ModelState.Values)
                    {
                        int count = 0;
                        foreach (var error in modelStateValue.Errors)
                        {
                            //if (count == 0)
                            //{
                            //    strErrorMessage.Append("<ul>");
                            //}
                            //count++;
                            //strErrorMessage.Append("<li>");
                            message = error.ErrorMessage;
                            //strErrorMessage.Append("</li>");
                        }
                    }
                    //return Json(new { success = false, message = message });
                    return Json(message);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        /// <summary>
        /// returns the list of physical road details
        /// </summary>
        /// <param name="physicalRoadCollection">formcollection containing grid parameters</param>
        /// <returns></returns>
        [Audit]
        public ActionResult GetRoadPhysicalProgressListForITNO(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                IExecutionBAL objBAL = new ExecutionBAL();
                long totalRecords = 0;
                string encryptedRoad = string.Empty;
                int imsRoadCode = 0;
                Dictionary<string, string> decryptedParameters = new Dictionary<string, string>();

                if (!(string.IsNullOrEmpty(Request.Params["roadCode"])))
                {
                    encryptedRoad = Request.Params["roadCode"];
                    decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedRoad.Split('/')[0], encryptedRoad.Split('/')[1], encryptedRoad.Split('/')[2], });
                    imsRoadCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);
                }

                var jsonData = new
                {
                    rows = objBAL.GetRoadPhysicalProgressListForITNO(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, imsRoadCode),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// returns the list of physical road details
        /// </summary>
        /// <param name="physicalRoadCollection">formcollection containing grid parameters</param>
        /// <returns></returns>
        [Audit]
        public ActionResult GetFinancialProgressListForITNO(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                IExecutionBAL objBAL = new ExecutionBAL();
                long totalRecords = 0;
                string encryptedRoad = string.Empty;
                int imsRoadCode = 0;
                Dictionary<string, string> decryptedParameters = new Dictionary<string, string>();

                if (!(string.IsNullOrEmpty(Request.Params["roadCode"])))
                {
                    encryptedRoad = Request.Params["roadCode"];
                    decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedRoad.Split('/')[0], encryptedRoad.Split('/')[1], encryptedRoad.Split('/')[2], });
                    imsRoadCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);
                }

                var jsonData = new
                {
                    rows = objBAL.GetFinancialProgressListForITNO(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, imsRoadCode),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return null;
            }
        }


        /// <summary>
        /// returns the LSB Details
        /// </summary>
        /// <param name="lsbCollection"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult GetLSBPhysicalProgressListForITNO(int? page, int? rows, string sidx, string sord)
        {
            IExecutionBAL objBAL = new ExecutionBAL();
            int imsRoadCode = 0;
            string encryptedRoad = string.Empty;
            long totalRecords = 0;
            Dictionary<string, string> decryptedParameters = new Dictionary<string, string>();

            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }

            try
            {
                if (!(string.IsNullOrEmpty(Request.Params["roadCode"])))
                {
                    encryptedRoad = Request.Params["roadCode"];
                    decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedRoad.Split('/')[0], encryptedRoad.Split('/')[1], encryptedRoad.Split('/')[2], });
                    imsRoadCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);
                }

                var jsonData = new
                {
                    rows = objBAL.GetLSBPhysicalProgressListForITNO(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, imsRoadCode),
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

        /// <summary>
        /// returns the blocks according to the district
        /// </summary>
        /// <param name="districtCode"></param>
        /// <returns></returns>
        public JsonResult GetBlockByDistrict(int districtCode)
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                return Json(objCommon.PopulateBlocks(districtCode, true));
            }
            catch (Exception)
            {
                return null;
            }
        }


        #endregion

        #region READ_GEOPOSITIONS_IMAGE


        /// <summary>
        /// reads the image to find the Geo locations (lattitude and longitude)
        /// </summary>
        /// <returns></returns>
        //[HttpPost]
        private bool ReadGeoPositions(HttpPostedFileBase file, out string errorMessage, out string latitude, out string longitude)
        {
            try
            {
                Double[] latitudeRef = null;
                Double[] longitudeRef = null;
                //HttpPostedFileBase file = formCollection["files"];
                using (ExifReader reader = new ExifReader(file.InputStream))
                {
                    reader.GetTagValue<Double[]>(ExifTags.GPSLatitude, out latitudeRef);
                    reader.GetTagValue<Double[]>(ExifTags.GPSLongitude, out longitudeRef);

                    if (latitudeRef != null && longitudeRef != null)
                    {
                        //return Json(new { success = true, latitude = latitude, longitude = longitude });
                        StringBuilder strLat = new StringBuilder();
                        for (int value = 0; value < latitudeRef.Count(); value++)
                        {
                            if (value == 0)
                            {
                                strLat.Append(latitudeRef[value].ToString() + ".");
                            }
                            else
                            {
                                if (latitudeRef[value].ToString().Contains("."))
                                {
                                    strLat.Append(latitudeRef[value].ToString().Replace(".", string.Empty).ToString());
                                }
                                else
                                {
                                    strLat.Append(latitudeRef[value].ToString());
                                }

                            }
                        }

                        StringBuilder strLong = new StringBuilder();
                        for (int value = 0; value < longitudeRef.Count(); value++)
                        {
                            if (value == 0)
                            {
                                strLong.Append(longitudeRef[value].ToString() + ".");
                            }
                            else
                            {
                                if (longitudeRef[value].ToString().Contains("."))
                                {
                                    strLong.Append(longitudeRef[value].ToString().Replace(".", string.Empty).ToString());
                                }
                                else
                                {
                                    strLong.Append(longitudeRef[value].ToString());
                                }
                            }
                        }

                        errorMessage = string.Empty;
                        latitude = strLat.ToString();
                        longitude = strLong.ToString();
                        return true;
                    }
                    else
                    {
                        errorMessage = "Image does not contain the Geo location information.";
                        //return Json(new { success = false, message = "Image does not contain the Geo location information." });
                        latitude = latitudeRef.ToString();
                        longitude = longitudeRef.ToString();
                        return false;
                    }

                }
            }
            catch (Exception)
            {
                errorMessage = "Please select file to upload";
                latitude = string.Empty;
                longitude = string.Empty;
                return false;
                //return Json(new { success = false , message = "Error occurred while processing your request."});
            }
        }


        [HttpPost]
        public ActionResult ReadGeoPositions(FormCollection formCollection)
        {
            try
            {
                Double[] latitudeRef = null;
                Double[] longitudeRef = null;

                Stream stream = Request.InputStream;

                if (stream != null)
                {
                    using (ExifReader reader = new ExifReader(stream))
                    {
                        reader.GetTagValue<Double[]>(ExifTags.GPSLatitude, out latitudeRef);
                        reader.GetTagValue<Double[]>(ExifTags.GPSLongitude, out longitudeRef);

                        if (latitudeRef != null && longitudeRef != null)
                        {
                            StringBuilder strLatitude = new StringBuilder();
                            for (int item = 0; item < latitudeRef.Count(); item++)
                            {
                                if (item == 0)
                                {
                                    strLatitude.Append(latitudeRef[item].ToString() + (char)176 + " ");
                                }
                                if (item == 1)
                                {
                                    strLatitude.Append(latitudeRef[item].ToString() + "' ");
                                }
                                if (item == 2)
                                {
                                    strLatitude.Append(Math.Round(latitudeRef[item], 1).ToString());
                                }
                            }

                            StringBuilder strLongitude = new StringBuilder();
                            for (int item = 0; item < longitudeRef.Count(); item++)
                            {
                                if (item == 0)
                                {
                                    strLongitude.Append(longitudeRef[item].ToString() + (char)176 + " ");
                                }
                                if (item == 1)
                                {
                                    strLongitude.Append(longitudeRef[item].ToString() + "' ");
                                }
                                if (item == 2)
                                {
                                    strLongitude.Append(Math.Round(longitudeRef[item], 1).ToString());
                                }
                                //strLongitude.Append(item.ToString());
                            }

                            return Json(new { success = true, latitude = strLatitude.ToString(), longitude = strLongitude.ToString() });
                        }
                        else
                        {
                            return Json(new { success = false, message = "Image does not contain the Geo location information." });
                        }

                    }
                }
                else
                {
                    return Json(new { success = false, message = "Please select image to upload." });
                }

                //return Json(new { success = true, latitude = latitudeRef.ToString(), longitude = longitudeRef.ToString() });
            }
            catch (Exception ex)
            {
                if (ex.Message == "Unable to locate EXIF content")
                {
                    return Json(new { success = false, message = "Image does not contain Longitude and Latitude." });
                }
                else if (ex.Message == "File is not a valid JPEG")
                {
                    return Json(new { success = false, message = "Please select a valid Image File." });
                }
                else
                {
                    return Json(new { success = false, message = "Error occurred while processing your request." });
                }
            }
        }

        #endregion

        #region Habitation Details

        /// <summary>
        /// returns Physiacal Progress of road 
        /// </summary>
        /// <param name="urlparameter">encrypted proposal code</param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult ListHabitationDetails(string urlparameter)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }
            int proposalCode = 0;

            String[] encryptedParameters = null;
            Dictionary<string, string> decryptedParameters = null;
            ExecutionDAL objDAL = new ExecutionDAL();
            PMGSY.DAL.Proposal.ProposalDAL objProp = new DAL.Proposal.ProposalDAL();
            PMGSY.Controllers.ProposalController prop = new PMGSY.Controllers.ProposalController();
            try
            {
                if (urlparameter != string.Empty)
                {
                    encryptedParameters = urlparameter.Split('/');
                    if (encryptedParameters.Length == 3)
                    {
                        decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    }
                }

                proposalCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);

                ExecutionRoadStatusViewModel roadModel = objDAL.GetRoadDetails(proposalCode);
                if (roadModel == null)
                {
                    roadModel = new ExecutionRoadStatusViewModel();
                }

                roadModel.IMS_PR_ROAD_CODE = proposalCode;

                roadModel.PLAN_CN_ROAD_CODE = objProp.getCNRoadCode(proposalCode);

                roadModel.Cluster_Habitation = "H";
                roadModel.HABITATIONS = objDAL.GetHabitationsForMappingDAL(proposalCode);

                List<SelectListItem> lst = objDAL.PopulateClustersDAL(Convert.ToInt32(roadModel.PLAN_CN_ROAD_CODE), proposalCode);
                lst.Insert(0, new SelectListItem() { Text = "Select Cluster", Value = "-1", Selected = true });

                roadModel.clusterList = lst;

                roadModel.EncryptedRoadCode = URLEncrypt.EncryptParameters1(new string[] { "ProposalCode=" + proposalCode.ToString().Trim() });// urlparameter;
                return View(roadModel);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPost]
        public JsonResult PopulateClusters()
        {
            ExecutionDAL objDAL = new ExecutionDAL();
            try
            {
                //CommonFunctions objCommonFunctions = new CommonFunctions();
                int roadCode = Convert.ToInt32(Request.Params["roadCode"]);
                int proposalCode = Convert.ToInt32(Request.Params["proposalCode"]);

                return Json(objDAL.PopulateClustersDAL(roadCode, proposalCode));
            }
            catch (Exception ex)
            {
                return Json(new { string.Empty });
            }
        }

        /// <summary>
        /// returns the list of habitations mapped with the road
        /// </summary>
        /// <param name="mapCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetHabitationListToMap(int? page, int? rows, string sidx, string sord)
        {
            ExecutionBAL objBAL = new ExecutionBAL();
            int roadCode = 0;
            int blockCode = 0;
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

                if (!string.IsNullOrEmpty(Request.Params["prRoadCode"]))
                {
                    roadCode = Convert.ToInt32(Request.Params["prRoadCode"]);
                }

                var jsonData = new
                {
                    rows = objBAL.GetHabitationListToMap(roadCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// populating the list of Habitations
        /// </summary>
        /// <param name="page">no. of pages</param>
        /// <param name="rows">no. of rows</param>
        /// <param name="sidx">sort column name</param>
        /// <param name="sord">sort order</param>
        /// <returns>retuns the list of habitations</returns>
        [HttpPost]
        [Audit]
        public ActionResult GetHabitationList(int? page, int? rows, string sidx, string sord)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }

            ExecutionBAL objBAL = new ExecutionBAL();
            int roadCode = 0;
            long totalRecords = 0;
            string flag = string.Empty;
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["prRoadCode"]))
                {
                    roadCode = Convert.ToInt32(Request.Params["prRoadCode"]);
                }

                var jsonData = new
                {
                    rows = objBAL.GetHabitationList(roadCode, flag, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// maps the habitation to the particular road
        /// </summary>
        /// <param name="mappedCollection">form collection containing the Habitation details</param>
        /// <returns>response message along with status</returns>
        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult MapHabitations(FormCollection mappedCollection)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }

            ExecutionBAL objBAL = new ExecutionBAL();
            bool status = false;
            string encryptedHabCodes = string.Empty;
            string roadName = string.Empty;
            String MappingDate = String.Empty;
            string[] strArray;

            string AprilMonthDay = ConfigurationManager.AppSettings["PHYSICAL_ROAD_PROGRESS_APRIL_MONTH_DAY"];   //10
            int AprilMonthDayValue = Convert.ToInt16(AprilMonthDay);

            string AprilMonth = ConfigurationManager.AppSettings["PHYSICAL_ROAD_PROGRESS_APRIL_MONTH"];  //4
            int AprilMonthValue = Convert.ToInt16(AprilMonth);
           
            try
            {
                encryptedHabCodes = mappedCollection["EncryptedHabCodes"];
                roadName = mappedCollection["IMS_PR_ROAD_CODE"];
                MappingDate = mappedCollection["mappedHabitaionDate"];
                // change
                int allowedMonth = (DateTime.Now.Month == AprilMonthValue && DateTime.Now.Day <= AprilMonthDayValue) ? (DateTime.Now.Month - 1) : DateTime.Now.Month;
                int allowedYear = DateTime.Now.Year;
                // Changes START here 
                DateTime FinanDate = DateTime.Now; // change
                int FinancialYear = FinanDate.Month >= 4 ? FinanDate.Year : FinanDate.Year - 1; // change
                DateTime Conditional_Date_Value = new DateTime(FinancialYear, 3, 31, 00, 00, 00);  // change
                int CondFinanYear = FinancialYear;
                //DateTime TestDate = new DateTime(2023, 4, 6, 00, 00, 00);  // change commentable
                //MappingDate = TestDate.ToString();  // change commentable
                Nullable<DateTime> ProgressDate = new DateTime();
                if (MappingDate != null)
                {
                    ProgressDate = Convert.ToDateTime(MappingDate);
                }

                Nullable<DateTime> Entry_Date = new DateTime();
                if (ProgressDate != null)
                {
                    Entry_Date = (ProgressDate.Value.Day <= AprilMonthDayValue && ProgressDate.Value.Month == AprilMonthValue)
                            ? Conditional_Date_Value : ProgressDate;
                }
                if (FinanDate.Month == AprilMonthValue && FinanDate.Day > AprilMonthDayValue)   // CHANGE
                {
                    if (Entry_Date.Value.Year * 12 + Entry_Date.Value.Month <= CondFinanYear * 12 + 3)
                    {
                        return Json(new { success = false, message = "Habitation can be mapped in Current Date of Current Financial Year " }, JsonRequestBehavior.AllowGet);
                    }
                }

                ///Changed by SAMMED A. PATIL on 05APR2017 to restrict entry before APRIL 2017
                strArray = MappingDate.Split('/');

                if (DateTime.Now.Month == AprilMonthValue)
                {
                    if (DateTime.Now.Day <= AprilMonthDayValue)
                    {
                        if (!((Convert.ToInt32(strArray[1]) == DateTime.Now.Month - 1) && Convert.ToInt32(strArray[2]) == DateTime.Now.Year))
                        {
                            return Json(new { success = false, message = "Progress Month and Year must be equal to March and " + DateTime.Now.Year }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        if (!((Convert.ToInt32(strArray[1]) == DateTime.Now.Month) && Convert.ToInt32(strArray[2]) == DateTime.Now.Year))
                        {
                            return Json(new { success = false, message = "Progress Month and Year must be equal to April and " + DateTime.Now.Year }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                else if (DateTime.Now.Day <= 5)
                {
                    if (Convert.ToInt32(strArray[1]) == 12)
                    {
                        if (!((Convert.ToInt32(strArray[1]) == 12) && Convert.ToInt32(strArray[2]) == DateTime.Now.Year - 1))
                        {
                            //return Json(new { success = false, message = "Please select date for current month and year" }, JsonRequestBehavior.AllowGet);
                            return Json(new { success = false, message = "Please select date for last month" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        if (!((/*Convert.ToInt32(strArray[1]) == DateTime.Now.Month || */Convert.ToInt32(strArray[1]) == DateTime.Now.Month - 1) && Convert.ToInt32(strArray[2]) == DateTime.Now.Year))
                        {
                            //return Json(new { success = false, message = "Please select date for current month and year" }, JsonRequestBehavior.AllowGet);
                            return Json(new { success = false, message = "Please select date for last month" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                else
                {
                    if (!((Convert.ToInt32(strArray[1]) == DateTime.Now.Month) && Convert.ToInt32(strArray[2]) == DateTime.Now.Year))
                    {
                        return Json(new { success = false, message = "Progress Month and Year must be equal to current month and year." }, JsonRequestBehavior.AllowGet);
                    }
                }

                //if (!(Convert.ToInt32(strArray[1]) == DateTime.Now.Month && Convert.ToInt32(strArray[2]) == DateTime.Now.Year))
                //{
                //    return Json(new { success = false, message = "Please select date for current month and year" }, JsonRequestBehavior.AllowGet);
                //}

                if (objBAL.MapHabitationToRoad(encryptedHabCodes, roadName, MappingDate))
                {
                    message = "Habitations added successfully.";
                    status = true;
                }
                else
                {
                    status = false;
                    message = "Habitations not added successfully.";
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                message = "Error occurred while processing the request.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult PopulateHabitationList()
        {
            ExecutionDAL objDAL = new ExecutionDAL();
            try
            {
                int roadCode = Convert.ToInt32(Request.Params["prRoadCode"]);
                return Json(objDAL.GetHabitationsForMappingDAL(roadCode));
            }
            catch
            {
                return Json(new { string.Empty }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// populating the list of Habitations
        /// </summary>
        /// <param name="page">no. of pages</param>
        /// <param name="rows">no. of rows</param>
        /// <param name="sidx">sort column name</param>
        /// <param name="sord">sort order</param>
        /// <returns>retuns the list of habitations</returns>
        [HttpPost]
        [Audit]
        public ActionResult GetMappedHabitationList(int? page, int? rows, string sidx, string sord)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }

            ExecutionBAL objBAL = new ExecutionBAL();
            int roadCode = 0;
            long totalRecords = 0;
            string flag = string.Empty;
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["prRoadCode"]))
                {
                    roadCode = Convert.ToInt32(Request.Params["prRoadCode"]);
                }

                var jsonData = new
                {
                    rows = objBAL.GetMappedHabitationList(roadCode, flag, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// maps the habitation to the particular road
        /// </summary>
        /// <param name="mappedCollection">form collection containing the Habitation details</param>
        /// <returns>response message along with status</returns>
        [HttpPost]
        [Audit]
        //[ValidateAntiForgeryToken]
        public ActionResult MapClusterHabitations()
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }

            ExecutionBAL objBAL = new ExecutionBAL();
            bool status = false;
            int roadCode = 0;
            int clusterCode = 0;
            try
            {
                roadCode = Convert.ToInt32(Request.Params["roadCode"]);
                clusterCode = Convert.ToInt32(Request.Params["clusterCode"]);

                if (clusterCode <= 0)
                {
                    return Json(new { success = false, message = "Please select a Cluster to map" }, JsonRequestBehavior.AllowGet);
                }

                if (objBAL.MapClusterToRoad(roadCode, clusterCode))
                {
                    message = "Cluster added successfully.";
                    status = true;
                }
                else
                {
                    status = false;
                    message = "Cluster not added successfully.";
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                message = "Error occurred while processing the request.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        //added by Pradip Patil 09/03/2017

        public JsonResult DeleteHabitaion(string urlparameter)
        {
            SelectListItem deleted = null;
            String[] encryptedParameters = null;
            Dictionary<string, string> decryptedParameters = null;
            ExecutionBAL objBAL = new ExecutionBAL();
            try
            {
                if (urlparameter != string.Empty)
                {
                    encryptedParameters = urlparameter.Split('/');
                    if (encryptedParameters.Length == 3)
                    {
                        decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    }
                }

                string HabCode = decryptedParameters["HabCode"];
                return Json(new { success = objBAL.DeleteHabitaion(Convert.ToInt32(HabCode), out deleted), Deleted = deleted });
            }
            catch (Exception ex)
            {
                return Json(new { success = false });
            }

        }

        #endregion Habitation Details Ends

        #region Road Safety
        /// <summary>
        /// returns Road Safety Filter view
        /// </summary>
        /// <param name="urlparameter">encrypted proposal code</param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult RoadSafetyLayout(string urlparameter)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }
            int proposalCode = 0;

            String[] encryptedParameters = null;
            Dictionary<string, string> decryptedParameters = null;
            ExecutionDAL objDAL = new ExecutionDAL();
            PMGSY.DAL.Proposal.ProposalDAL objProp = new DAL.Proposal.ProposalDAL();
            try
            {
                if (urlparameter != string.Empty)
                {
                    encryptedParameters = urlparameter.Split('/');
                    if (encryptedParameters.Length == 3)
                    {
                        decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    }
                }

                proposalCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);

                RoadSafetyViewModel roadModel = new RoadSafetyViewModel();

                roadModel.prRoadCode = proposalCode;

                roadModel.stageList = new List<SelectListItem>();
                roadModel.stageList.Insert(0, new SelectListItem() { Text = "Select Stage", Value = "-1", Selected = true });
                roadModel.stageList.Insert(1, new SelectListItem() { Text = "Design Stage", Value = "D" });
                roadModel.stageList.Insert(1, new SelectListItem() { Text = "During Construction Stage", Value = "P" });
                roadModel.stageList.Insert(2, new SelectListItem() { Text = "Completion Stage", Value = "C" });

                //roadModel.EncryptedRoadCode = URLEncrypt.EncryptParameters1(new string[] { "ProposalCode=" + proposalCode.ToString().Trim() });// urlparameter;

                ExecutionRoadStatusViewModel execRoadModel = objDAL.GetRoadDetails(proposalCode);
                if (execRoadModel != null)
                {
                    roadModel.BlockName = execRoadModel.BlockName;
                    roadModel.Package = execRoadModel.Package;
                    roadModel.RoadName = execRoadModel.RoadName;
                    roadModel.AgreementDate = execRoadModel.AgreementDate;
                    roadModel.Sanction_Cost = execRoadModel.Sanction_Cost;
                    roadModel.Sanction_length = execRoadModel.Sanction_length;
                    roadModel.AgreementCost = execRoadModel.AgreementCost;
                    roadModel.SanctionYear = execRoadModel.SanctionYear;
                    roadModel.changedLength = execRoadModel.changedLength;
                }

                return View(roadModel);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddRoadSafety(RoadSafetyViewModel model)
        {
            message = "Error occured while processing your request.";
            try
            {
                ExecutionBAL execBal = new ExecutionBAL();
                Boolean Status = execBal.AddRoadSafetyBAL(model, ref message);
                return Json(new { success = Status, message = message });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AddRoadSafety()");
                return Json(new { success = false, message = message });
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult GetRoadSafetyList(int? page, int? rows, string sidx, string sord)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }

            ExecutionBAL objBAL = new ExecutionBAL();
            int roadCode = 0;
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

                if (!string.IsNullOrEmpty(Request.Params["prRoadCode"]))
                {
                    roadCode = Convert.ToInt32(Request.Params["prRoadCode"]);
                }

                var jsonData = new
                {
                    rows = objBAL.GetRoadSafetyListBAL(roadCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetMappedHabitationList()");
                return null;
            }
        }

        #endregion Road Safety ends

        #region Exec Tech File Upload

        //image upload View
        [HttpGet]
        [Audit]
        public ActionResult ExecTechImageUpload(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                dbContext = new PMGSYEntities();

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                int imsPrRoadCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);

                if (decryptedParameters.Count() > 0)
                {
                    PMGSY.Models.Execution.FileUploadViewModel fileUploadViewModel = new PMGSY.Models.Execution.FileUploadViewModel();
                    fileUploadViewModel.IMS_PR_ROAD_CODE = imsPrRoadCode;
                    fileUploadViewModel.lstHeadItems = new List<SelectListItem>();

                    fileUploadViewModel.lstHeadItems = new SelectList(dbContext.MASTER_EXECUTION_ITEM.Where(m => m.MAST_HEAD_TYPE == "I").Select(m => new { Value = m.MAST_HEAD_CODE, Text = m.MAST_HEAD_DESC }).ToList(), "Value", "Text").ToList();
                    fileUploadViewModel.lstHeadItems.Insert(0, new SelectListItem { Value = "0", Text = "Select Stage" });

                    if (dbContext.EXEC_TECH_FILES.Where(a => a.IMS_PR_ROAD_CODE == fileUploadViewModel.IMS_PR_ROAD_CODE).Any())
                    {
                        fileUploadViewModel.NumberofFiles = dbContext.EXEC_TECH_FILES.Where(a => a.IMS_PR_ROAD_CODE == fileUploadViewModel.IMS_PR_ROAD_CODE /*&& a.EXEC_FILE_TYPE == 0*/).Count();
                    }
                    else
                    {
                        fileUploadViewModel.NumberofFiles = 0;
                    }
                    return View("ExecTechImageUpload", fileUploadViewModel);
                }
                return View("ExecTechImageUpload", new PMGSY.Models.Execution.FileUploadViewModel());
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "ExecTechImageUpload");
                //return View("ExecTechImageUpload", new PMGSY.Models.Execution.FileUploadViewModel());
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        //list uploaded Images
        [Audit]
        public JsonResult ListExecTechFiles(FormCollection formCollection)
        {
            //Added By Abhishek kamble 29-Apr-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Added By Abhishek kamble 29-Apr-2014 start

            IExecutionBAL objExecutionBAL = new ExecutionBAL();

            int IMS_PR_ROAD_CODE = Convert.ToInt32(Request["IMS_PR_ROAD_CODE"]);
            int totalRecords;
            var jsonData = new
            {
                rows = objExecutionBAL.GetExecTechFilesListBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, IMS_PR_ROAD_CODE),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                page = Convert.ToInt32(formCollection["page"]),
                records = totalRecords
            };
            return Json(jsonData);
        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        [Audit]
        public ActionResult UploadsExecTech(PMGSY.Models.Execution.FileUploadViewModel fileUploadViewModel)
        {
            Regex regex = new Regex(@"^[a-zA-Z0-9 ]*$");
            if (regex.IsMatch(Request.Params["remark[]"]) && Request.Params["remark[]"].Trim() != string.Empty)
            {
                fileUploadViewModel.Image_Description = Request.Params["remark[]"];
            }
            else
            {
                return Json("Invalid Image Description, Only Alphabets and Numbers are allowed");
            }

            if (regex.IsMatch(Request.Params["remarks[]"]))
            {
                fileUploadViewModel.Remarks = Request.Params["remarks[]"];
            }
            else
            {
                return Json("Invalid Remarks, Only Alphabets and Numbers are allowed");
            }

            //Added By Abhishek kamble 26-Apr-2014 to validate File Type
            CommonFunctions objCommonFunc = new CommonFunctions();
            //Array of File Types to Validate             
            String[] fileTypes = new String[] { "jpeg", String.Empty, String.Empty };
            if (!(objCommonFunc.IsValidImageFile(ConfigurationManager.AppSettings["EXEC_TECH_PROGRESS_FILE_UPLOAD"], Request, fileTypes)))
            {
                fileUploadViewModel.ErrorMessage = "File Type is Not Allowed.";
                return View("ExecTechImageUpload", fileUploadViewModel.ErrorMessage);
            }

            var fileData = new List<PMGSY.Models.Execution.FileUploadViewModel>();
            int IMS_PR_ROAD_CODE = 0;
            if (fileUploadViewModel.IMS_PR_ROAD_CODE != 0)
            {
                IMS_PR_ROAD_CODE = fileUploadViewModel.IMS_PR_ROAD_CODE;
            }
            else
            {
                try
                {
                    IMS_PR_ROAD_CODE = Convert.ToInt32(Request["IMS_PR_ROAD_CODE"]);
                }
                catch (Exception ex)
                {
                    //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                    ErrorLog.LogError(ex, "UploadsExecTech(PMGSY.Models.Execution.FileUploadViewModel fileUploadViewModel)");
                    if (Request["IMS_PR_ROAD_CODE"].Contains(','))
                    {
                        IMS_PR_ROAD_CODE = Convert.ToInt32(Request["IMS_PR_ROAD_CODE"].Split(',')[0]);
                    }
                }
            }

            string errorMessage = string.Empty;
            string latitude = string.Empty;
            string longitude = string.Empty;
            HttpPostedFileBase file1 = Request.Files[0];
            foreach (string file in Request.Files)
            {
                ExecTechUploadImageFile(Request, fileData, IMS_PR_ROAD_CODE);
            }

            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;

            var result = new ContentResult
            {
                Content = "{\"files\":" + serializer.Serialize(fileData) + "}",
            };
            return result;
        }

        [Audit]
        public void ExecTechUploadImageFile(HttpRequestBase request, List<PMGSY.Models.Execution.FileUploadViewModel> statuses, int IMS_PR_ROAD_CODE)
        {
            try
            {
                dbContext = new PMGSYEntities();
                IExecutionBAL objExecutionBAL = new ExecutionBAL();
                HttpRequestBase newRequest = request;
                String StorageRoot = ConfigurationManager.AppSettings["EXEC_TECH_PROGRESS_FILE_UPLOAD"];
                int MaxCount = 0;

                string errorMessage = string.Empty;
                string lati = string.Empty;
                string longi = string.Empty;
                for (int i = 0; i < request.Files.Count; i++)
                {
                    HttpPostedFileBase file = request.Files[i];

                    #region LAT_LONG_CALCULATION

                    Double[] latitudeRef = null;
                    Double[] longitudeRef = null;
                    StringBuilder strLat = new StringBuilder();
                    StringBuilder strLong = new StringBuilder();

                    using (ExifReader reader = new ExifReader(file.InputStream, true))
                    {
                        reader.GetTagValue<Double[]>(ExifTags.GPSLatitude, out latitudeRef);
                        reader.GetTagValue<Double[]>(ExifTags.GPSLongitude, out longitudeRef);

                        if (latitudeRef != null && longitudeRef != null)
                        {
                            //return Json(new { success = true, latitude = latitude, longitude = longitude });

                            for (int value = 0; value < latitudeRef.Count(); value++)
                            {
                                if (value == 0)
                                {
                                    strLat.Append(latitudeRef[value].ToString() + ".");
                                }
                                else
                                {
                                    if (latitudeRef[value].ToString().Contains("."))
                                    {
                                        strLat.Append(latitudeRef[value].ToString().Replace(".", string.Empty).ToString());
                                    }
                                    else
                                    {
                                        strLat.Append(latitudeRef[value].ToString());
                                    }
                                }
                            }

                            for (int value = 0; value < longitudeRef.Count(); value++)
                            {
                                if (value == 0)
                                {
                                    strLong.Append(longitudeRef[value].ToString() + ".");
                                }
                                else
                                {
                                    if (longitudeRef[value].ToString().Contains("."))
                                    {
                                        strLong.Append(longitudeRef[value].ToString().Replace(".", string.Empty).ToString());
                                    }
                                    else
                                    {
                                        strLong.Append(longitudeRef[value].ToString());
                                    }
                                }
                            }
                        }
                        else
                        {
                            errorMessage = "Image does not contain the Geo location information.";
                            //return Json(new { success = false, message = "Image does not contain the Geo location information." });
                        }
                    }
                    #endregion

                    int contentLength = file.ContentLength;
                    var fileId = IMS_PR_ROAD_CODE;
                    if (dbContext.EXEC_TECH_FILES.Where(a => a.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Any())
                    {
                        //new change done on 26-08-2013
                        //MaxCount = dbContext.EXEC_TECH_FILES.Where(a => a.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Count();
                        MaxCount = dbContext.EXEC_TECH_FILES.Where(m => m.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).OrderByDescending(m => m.EXEC_FILE_ID).Select(m => m.EXEC_FILE_ID).FirstOrDefault();
                    }
                    MaxCount++;

                    var fileName = IMS_PR_ROAD_CODE + "-" + MaxCount + Path.GetExtension(request.Files[i].FileName).ToString();
                    var fullPath = Path.Combine(StorageRoot, fileName);

                    var ThumbnailPath = Path.Combine(StorageRoot, "thumbnails");
                    var FullThumbnailPath = Path.Combine(ThumbnailPath, fileName);

                    statuses.Add(new PMGSY.Models.Execution.FileUploadViewModel()
                    {
                        url = fullPath,
                        thumbnail_url = fullPath,
                        name = fileName,
                        type = file.ContentType,
                        size = file.ContentLength,
                        delete_url = "",
                        delete_type = "DELETE",
                        file_type = 0,
                        Image_Description = request.Params["remark[]"],
                        Remarks = request.Params["remarks[]"],
                        IMS_PR_ROAD_CODE = IMS_PR_ROAD_CODE,
                        status = "P",
                        HeadItem = Convert.ToInt32(request.Params["stage[]"]),
                        //Latitude = latitude,
                        //Longitude = longitude
                        Latitude = Convert.ToDecimal(strLat.ToString()),
                        Longitude = Convert.ToDecimal(strLong.ToString())
                    });
                    string status = objExecutionBAL.AddExecTechFileUploadDetailsBAL(statuses);
                    if (status == string.Empty)
                    {
                        objExecutionBAL.CompressImage(newRequest.Files[0], fullPath, FullThumbnailPath);
                    }
                    else
                    {
                        // show an error over here
                    }
                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "ExecTechUploadImageFile()");
                throw ex;
                //return Json(new { status=false},JsonRequestBehavior.AllowGet);
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        [HttpPost]
        [Audit]
        public JsonResult UpdateExecTechImageDetails(FormCollection formCollection)
        {
            IExecutionBAL objExecutionBAL = new ExecutionBAL();
            try
            {
                string[] arrKey = formCollection["id"].Split('$');
                PMGSY.Models.Execution.FileUploadViewModel fileuploadViewModel = new PMGSY.Models.Execution.FileUploadViewModel();
                fileuploadViewModel.IMS_PR_ROAD_CODE = Convert.ToInt32(arrKey[1]);
                fileuploadViewModel.EXEC_FILE_ID = Convert.ToInt32(arrKey[0]);

                Regex regex = new Regex(@"^[a-zA-Z0-9 ]*$");
                if (regex.IsMatch(formCollection["Description"]) && formCollection["Description"].Trim() != string.Empty)
                {
                    fileuploadViewModel.Image_Description = formCollection["Description"];
                }
                else
                {
                    return Json("Invalid Image Description, Only Alphabets and Numbers are allowed");
                }

                if (regex.IsMatch(formCollection["Remarks"]))
                {
                    fileuploadViewModel.Remarks = formCollection["Remarks"];
                }
                else
                {
                    return Json("Invalid Remarks, Only Alphabets and Numbers are allowed");
                }

                string status = objExecutionBAL.UpdateExecTechImageDetailsBAL(fileuploadViewModel);

                if (status == string.Empty)
                    return Json(true);
                else
                    return Json("There is an error occured while processing your request.");
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "UpdateImageDetails()");
                return Json(new { Success = "false", message = "Error occured on Update Image Details" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Audit]
        public JsonResult DeleteExecTechFileDetails(string id)
        {
            IExecutionBAL objExecutionBAL = new ExecutionBAL();
            try
            {
                String PhysicalPath = string.Empty;
                String ThumbnailPath = string.Empty;
                string EXEC_FILE_NAME = Request.Params["IMS_FILE_NAME"];
                PhysicalPath = ConfigurationManager.AppSettings["EXEC_TECH_PROGRESS_FILE_UPLOAD"];
                ThumbnailPath = Path.Combine(Path.Combine(PhysicalPath, "thumbnails"), EXEC_FILE_NAME);

                string[] arrParam = Request.Params["IMS_PR_ROAD_CODE"].Split('$');

                int EXEC_FILE_ID = Convert.ToInt32(arrParam[0]);
                int IMS_PR_ROAD_CODE = Convert.ToInt32(arrParam[1]);

                PhysicalPath = Path.Combine(PhysicalPath, EXEC_FILE_NAME);

                if (!System.IO.File.Exists(PhysicalPath) && !System.IO.File.Exists(ThumbnailPath))
                {
                    //return Json(new { Success = false, ErrorMessage = "file Not Found." });
                }

                string status = objExecutionBAL.DeleteExecTechFileDetails(EXEC_FILE_ID, IMS_PR_ROAD_CODE, EXEC_FILE_NAME);

                if (status == string.Empty)
                {
                    try
                    {
                        System.IO.File.Delete(PhysicalPath);
                        System.IO.File.Delete(ThumbnailPath);
                    }
                    catch (Exception ex)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                        return Json(new { Success = true, ErrorMessage = ex.Message });
                    }
                    return Json(new { Success = true, ErrorMessage = "There is an error while processing your request." });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = status });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DeleteExecTechFileDetails()");
                return Json(new { Success = "false", message = "Error occured on Delete File Details" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [Audit]
        public ActionResult DownloadExecTechFile(String parameter, String hash, String key)
        {
            try
            {
                string FileName = string.Empty;
                string FullFileLogicalPath = string.Empty;
                string FullfilePhysicalPath = string.Empty;
                string FileExtension = string.Empty;

                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        FileName = (urlSplitParams[0]);
                    }
                }
                FileExtension = Path.GetExtension(FileName).ToLower();

                if (FileExtension == ".jpg" || FileExtension == ".jpeg" || FileExtension == ".bmp" || FileExtension == ".png" || FileExtension == ".gif" || FileExtension == ".tiff" || FileExtension == ".wmv")
                {
                    FullFileLogicalPath = Path.Combine(ConfigurationManager.AppSettings["EXEC_TECH_PRORESS_FILE_UPLOAD_VIRTUAL_DIR_PATH"], FileName);
                    FullfilePhysicalPath = Path.Combine(ConfigurationManager.AppSettings["EXEC_TECH_PROGRESS_FILE_UPLOAD"], FileName);
                }

                string name = Path.GetFileName(FileName);
                string ext = Path.GetExtension(FileName);

                string type = string.Empty;

                if (ext != null)
                {
                    switch (ext.ToLower())
                    {
                        case ".jpg":
                        case ".bmp":
                        case ".tiff":
                        case ".png":
                        case ".gif":
                        case ".jpeg":
                            type = "image/png";
                            break;
                        default:
                            type = "Application";
                            break;
                    }
                }

                if (System.IO.File.Exists(FullfilePhysicalPath))
                {
                    return File(FullfilePhysicalPath, type, DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + FileExtension);
                }
                else
                {
                    return Json(new { message = "No File Found" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DownloadExecTechFile()");
                return Json(new { Success = "false", message = "Error occured on File Download" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Execution Progress MRD

        /// <summary>
        /// returns districts according to the state
        /// </summary>
        /// <param name="stateCode">indicates the state id</param>
        /// <returns></returns>
        [Audit]
        public JsonResult GetDistrictByStateCode(int stateCode)
        {
            CommonFunctions objCommon = new CommonFunctions();
            try
            {
                List<SelectListItem> list = objCommon.PopulateDistrict(stateCode, true);
                list.Find(x => x.Value == "-1").Text = "Select District";
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        /// <summary>
        /// returns the list view of execution progress details
        /// </summary>
        /// <returns></returns>
        [Audit]
        [HttpGet]
        public ActionResult ListExecutionProgressMRD()
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                ExecutionProgressMRD progressModel = new ExecutionProgressMRD();
                List<SelectListItem> lstBatches = new List<SelectListItem>();
                TransactionParams transactionParams = new TransactionParams();
                transactionParams.STATE_CODE = PMGSYSession.Current.StateCode;
                transactionParams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                transactionParams.ISSearch = true;
                transactionParams.SANC_YEAR = (Int16)DateTime.Now.Year;
                lstBatches = objCommon.PopulateBatch();
                lstBatches.RemoveAt(0);
                lstBatches.Add(new SelectListItem { Value = "0", Text = "All Batches" });
                progressModel.BATCHS = lstBatches;

                progressModel.STATES = objCommon.PopulateStates(true);
                progressModel.STATES.Find(x => x.Value == "0").Value = "-1";

                progressModel.DISTRICTS.Insert(0, new SelectListItem() { Text = "Select District", Value = "-1" });

                progressModel.BLOCKS = objCommon.PopulateBlocks(PMGSYSession.Current.DistrictCode, true);
                progressModel.Years = PopulateYear(0, true, true);
                progressModel.STREAMS = objCommon.PopulateStreams("", true);
                progressModel.PROPOSAL_TYPES = objCommon.PopulateProposalTypes();

                //new filters added by Vikram 
                progressModel.lstBatchs = objCommon.PopulateBatch(true);
                progressModel.lstCollaborations = objCommon.PopulateFundingAgency(true);
                progressModel.lstUpgradations = objCommon.PopulateNewUpgradeList(true);
                //end of change

                List<SelectListItem> lstPackages = objCommon.PopulatePackage(transactionParams);
                lstPackages.RemoveAt(0);
                lstPackages.Insert(0, new SelectListItem { Value = "All", Text = "All Packages" });
                progressModel.PACKAGES = lstPackages;
                return View("ListExecutionProgressMRD", progressModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ListExecutionProgressMRD()");
                return null;
            }
        }

        /// <summary>
        /// returns the list of execution progress details 
        /// </summary>
        /// <param name="executionCollection">contains the filters and grid parameters</param>
        /// <returns></returns>
        [Audit]
        [HttpGet]
        public ActionResult GetExecutionProgressListMRD(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                int yearCode = 0;
                int districtCode = 0;
                int blockCode = 0;
                int streamCode = 0;
                int batchCode = 0;
                string proposalCode = string.Empty;
                string packageCode = string.Empty;
                string upgradationType = string.Empty;
                long totalRecords = 0;
                IExecutionBAL objBAL = new ExecutionBAL();

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["yearCode"]))
                {
                    yearCode = Convert.ToInt32(Request.Params["yearCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["districtCode"]))
                {
                    districtCode = Convert.ToInt32(Request.Params["districtCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["blockCode"]))
                {
                    blockCode = Convert.ToInt32(Request.Params["blockCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["streamCode"]))
                {
                    streamCode = Convert.ToInt32(Request.Params["streamCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["batchCode"]))
                {
                    batchCode = Convert.ToInt32(Request.Params["batchCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["proposalCode"]))
                {
                    proposalCode = Request.Params["proposalCode"];
                }

                if (!string.IsNullOrEmpty(Request.Params["packageCode"]))
                {
                    packageCode = Request.Params["packageCode"];
                }

                if (!string.IsNullOrEmpty(Request.Params["upgradationType"]))
                {
                    upgradationType = Request.Params["upgradationType"];
                }

                var jsonData = new
                {
                    rows = objBAL.GetExecutionListMRDBAL(yearCode, districtCode, blockCode, packageCode, proposalCode, upgradationType, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetExecutionProgressList()");
                return null;
            }
        }

        /// <summary>
        /// returns Physiacal Progress of road 
        /// </summary>
        /// <param name="urlparameter">encrypted proposal code</param>
        /// <returns></returns>
        [Audit]
        [HttpGet]
        public ActionResult ListPhysicalDetailsMRD(string urlparameter)
        {
            int proposalCode = 0;

            String[] encryptedParameters = null;
            Dictionary<string, string> decryptedParameters = null;
            ExecutionDAL objDAL = new ExecutionDAL();
            try
            {
                if (urlparameter != string.Empty)
                {
                    encryptedParameters = urlparameter.Split('/');
                    if (encryptedParameters.Length == 3)
                    {
                        decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    }
                }

                proposalCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);

                ExecutionRoadStatusViewModel roadModel = objDAL.GetRoadDetails(proposalCode);
                if (roadModel == null)
                {
                    roadModel = new ExecutionRoadStatusViewModel();
                }

                roadModel.IMS_PR_ROAD_CODE = proposalCode;
                return View("ListPhysicalDetailsMRD", roadModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ListPhysicalDetailsMRD()");
                return null;
            }
        }

        /// <summary>
        /// returns add view of physical road progress 
        /// </summary>
        /// <param name="id">encrypted id key</param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult AddPhysicalRoadProgressMRD(string id)
        {
            try
            {
                int proposalCode = Convert.ToInt32(id);
                ExecutionDAL objDAL = new ExecutionDAL();
                ExecutionRoadStatusViewModel roadModel = new ExecutionRoadStatusViewModel();
                roadModel = objDAL.GetRoadDetails(proposalCode);
                if (roadModel == null)
                {
                    roadModel = new ExecutionRoadStatusViewModel();
                }
                CommonFunctions objCommon = new CommonFunctions();
                roadModel.IMS_PR_ROAD_CODE = proposalCode;
                roadModel.Operation = "A";
                //ViewData["Year"] = objCommon.PopulateYears(true);
                //ViewData["Month"] = objCommon.PopulateMonths();

                List<SelectListItem> lstMonth = new List<SelectListItem>();
                lstMonth.Insert(0, new SelectListItem { Text = DateTime.Now.ToString("MMMM"), Value = DateTime.Now.Month.ToString() });
                ViewData["Month"] = lstMonth;

                #region Populate Month and Year
                //DateTime currDate = DateTime.Now;
                //roadModel.crYear = DateTime.Now.Year;

                //List<SelectListItem> lstMonth = new List<SelectListItem>();
                //List<SelectListItem> lstYear = new List<SelectListItem>();

                //if (currDate.Month == 1 && currDate.Day <= 14)
                //{
                //    lstYear.Insert(0, (new SelectListItem { Text = (currDate.Year - 1).ToString(), Value = (currDate.Year - 1).ToString() }));
                //    lstYear.Insert(0, (new SelectListItem { Text = (currDate.Year).ToString(), Value = (currDate.Year).ToString(), Selected = true }));
                //}
                //else
                //{
                //    lstYear.Insert(0, (new SelectListItem { Text = (currDate.Year).ToString(), Value = (currDate.Year).ToString(), Selected = true }));
                //}
                //if (currDate.Day <= 14)
                //{
                //    lstMonth.Insert(0, (new SelectListItem { Text = (currDate.AddMonths(-1)).ToString("MMMM"), Value = currDate.AddMonths(-1).Month.ToString() }));
                //    lstMonth.Insert(0, (new SelectListItem { Text = currDate.ToString("MMMM"), Value = currDate.Month.ToString(), Selected = true }));
                //}
                //else
                //{
                //    lstMonth.Insert(0, (new SelectListItem { Text = currDate.ToString("MMMM"), Value = currDate.Month.ToString(), Selected = true }));
                //}
                //ViewData["Year"] = lstYear;
                //ViewData["Month"] = lstMonth;

                //roadModel.currmonthName = currDate.ToString("MMMM");
                //roadModel.prevmonthName = currDate.AddMonths(-1).ToString("MMMM");
                #endregion

                List<SelectListItem> lstYear = objCommon.PopulateYears(true);
                int count = lstYear.IndexOf(lstYear.Find(c => c.Value == "2017"));
                lstYear.RemoveRange(lstYear.IndexOf(lstYear.Find(c => c.Value == "2017")), lstYear.Count - count);

                /*List<SelectListItem> lstYear = new List<SelectListItem>();
                if (DateTime.Now.Month <= 3)
                {
                    lstYear.Insert(0, new SelectListItem { Text = (DateTime.Now.Year - 1).ToString(), Value = (DateTime.Now.Year - 1).ToString() });
                }
                else
                {
                    lstYear.Insert(0, new SelectListItem { Text = DateTime.Now.Year.ToString(), Value = DateTime.Now.Year.ToString() });
                }*/
                ViewData["Year"] = lstYear;
                ViewData["WorkStatus"] = objDAL.GetWorkStatus();

                #region Clone Model
                ExecutionRoadStatusViewModelMRD model = new ExecutionRoadStatusViewModelMRD();
                model.IMS_PR_ROAD_CODE = roadModel.IMS_PR_ROAD_CODE;
                model.EncryptedPhysicalRoadCode = roadModel.EncryptedPhysicalRoadCode;
                model.OldCompleted = roadModel.OldCompleted;
                model.Sanction_length = roadModel.Sanction_length;
                model.CompleteStatus = roadModel.CompleteStatus;
                model.PreviousMonth = roadModel.PreviousMonth;
                model.PreviousYear = roadModel.PreviousYear;
                model.Operation = roadModel.Operation;
                model.PreviousBaseCourse = roadModel.PreviousBaseCourse;
                model.PreviousEarthWork = roadModel.PreviousEarthWork;
                model.PreviousMiscellaneous = roadModel.PreviousMiscellaneous;
                model.PreviousPreparatoryWork = roadModel.PreviousPreparatoryWork;
                model.PreviousSubbase = roadModel.PreviousSubbase;
                model.PreviousSurfaceCourse = roadModel.PreviousSurfaceCourse;
                model.PreviousCDWorks = roadModel.PreviousCDWorks;
                model.PreviousLSB = roadModel.PreviousLSB;
                model.PreviousRoadSigns = roadModel.PreviousRoadSigns;
                model.IsStage = roadModel.IsStage;
                model.AgreementMonth = roadModel.AgreementMonth;
                model.AgreementYear = roadModel.AgreementYear;
                model.AgreementDate = roadModel.AgreementDate;
                model.PreviousCompletedLength = roadModel.PreviousCompletedLength;
                model.changed_SanctionedLength = roadModel.changed_SanctionedLength;
                model.crYear = roadModel.crYear;
                model.currmonthName = roadModel.currmonthName;
                model.prevmonthName = roadModel.prevmonthName;
                model.EXEC_PROG_YEAR = roadModel.EXEC_PROG_YEAR;
                model.Operation = roadModel.Operation;
                model.EXEC_PROG_MONTH = roadModel.EXEC_PROG_MONTH;
                model.EXEC_ISCOMPLETED = roadModel.EXEC_ISCOMPLETED;
                model.EXEC_PREPARATORY_WORK = roadModel.EXEC_PREPARATORY_WORK;
                model.EXEC_EARTHWORK_SUBGRADE = roadModel.EXEC_EARTHWORK_SUBGRADE;
                model.EXEC_SUBBASE_PREPRATION = roadModel.EXEC_SUBBASE_PREPRATION;
                model.EXEC_BASE_COURSE = roadModel.EXEC_BASE_COURSE;
                model.EXEC_SURFACE_COURSE = roadModel.EXEC_SURFACE_COURSE;
                model.EXEC_SIGNS_STONES = roadModel.EXEC_SIGNS_STONES;
                model.EXEC_CD_WORKS = roadModel.EXEC_CD_WORKS;
                model.EXEC_LSB_WORKS = roadModel.EXEC_LSB_WORKS;
                model.EXEC_MISCELANEOUS = roadModel.EXEC_MISCELANEOUS;
                model.EXEC_COMPLETED = roadModel.EXEC_COMPLETED;
                model.ExecutionCompleteDate = roadModel.ExecutionCompleteDate;
                #endregion

                return PartialView("AddPhysicalRoadProgressMRD", model);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "AddPhysicalRoadProgressMRD(string id)");
                return Json(new { success = false, message = "Error Occurred while processing your request." });
            }
        }

        /// <summary>
        /// save the Physical Road Status Details
        /// </summary>
        /// <param name="progressModel">contains the form data</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Audit]
        public ActionResult AddPhysicalRoadDetailsMRD(ExecutionRoadStatusViewModelMRD progressModel)
        {
            string message = string.Empty;
            IExecutionBAL objBAL = new ExecutionBAL();
            try
            {
                if (ModelState.IsValid)
                {
                    int allowedMonth = DateTime.Now.Month;
                    int allowedYear = DateTime.Now.Year;

                    //if (DateTime.Now.Day <= 5)
                    //{
                    //    if (!(((progressModel.EXEC_PROG_MONTH == allowedMonth) || (progressModel.EXEC_PROG_MONTH == (allowedMonth - 1)))
                    //           && progressModel.EXEC_PROG_YEAR == allowedYear)
                    //       )
                    //    {
                    //        return Json(new { success = false, message = "Progress Month and Year must be equal to current month and year." }, JsonRequestBehavior.AllowGet);
                    //    }
                    //}
                    //else
                    //{
                    //    if (!(progressModel.EXEC_PROG_MONTH == allowedMonth && progressModel.EXEC_PROG_YEAR == allowedYear))
                    //    {
                    //        return Json(new { success = false, message = "Progress Month and Year must be equal to current month and year." }, JsonRequestBehavior.AllowGet);
                    //    }
                    //}


                    if (!(objBAL.AddPhysicalProgressDetailsMRDBAL(progressModel, ref message)))
                    {
                        if (message != string.Empty)
                        {
                            return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { success = false, message = "Physical Road Progress Details Not Added Successfully" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "AddPhysicalRoadDetails(ExecutionRoadStatusViewModel progressModel)");
                return Json(new { success = false, message = "Error occurred while processing the request." }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// returns the list of physical road details
        /// </summary>
        /// <param name="physicalRoadCollection">formcollection containing grid parameters</param>
        /// <returns></returns>
        [Audit]
        [HttpGet]
        public ActionResult GetRoadPhysicalProgressListMRD(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                IExecutionBAL objBAL = new ExecutionBAL();
                long totalRecords = 0;
                int imsRoadCode = 0;

                if (!(string.IsNullOrEmpty(Request.Params["roadCode"])))
                {
                    imsRoadCode = Convert.ToInt32(Request.Params["roadCode"]);
                }

                var jsonData = new
                {
                    rows = objBAL.GetRoadPhysicalProgressListMRDBAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, imsRoadCode),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetRoadPhysicalProgressList()");
                return null;
            }
        }

        /// <summary>
        /// return the details for updation
        /// </summary>
        /// <param name="hash">encrypted key</param>
        /// <param name="parameter">encrypted key</param>
        /// <param name="key">encrypted key</param>
        /// <returns></returns>
        [Audit]
        [HttpGet]
        public ActionResult EditPhysicalRoadProgressMRD(String hash, String parameter, String key)
        {
            try
            {
                Dictionary<string, string> decryptedParameters = null;
                CommonFunctions objCommon = new CommonFunctions();
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                IExecutionBAL objBAL = new ExecutionBAL();
                ExecutionDAL objDAL = new ExecutionDAL();
                int proposalCode = 0;   //for storing decrypted proposal code
                int monthCode = 0;  //for storing decrypted month code
                int yearCode = 0;   //for storing decrypted year code
                if (decryptedParameters != null)
                {
                    proposalCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);
                    monthCode = Convert.ToInt32(decryptedParameters["Month"]);
                    yearCode = Convert.ToInt32(decryptedParameters["Year"]);
                }
                ViewData["Year"] = objCommon.PopulateYears(true);
                ViewData["Month"] = objCommon.PopulateMonths();
                ViewData["WorkStatus"] = objDAL.GetWorkStatus();
                ExecutionRoadStatusViewModel roadModel = objBAL.GetPhysicalRoadDetails(proposalCode, monthCode, yearCode);
                roadModel.Operation = "E";

                #region Clone Model
                ExecutionRoadStatusViewModelMRD model = new ExecutionRoadStatusViewModelMRD();
                model.IMS_PR_ROAD_CODE = roadModel.IMS_PR_ROAD_CODE;
                model.EncryptedPhysicalRoadCode = roadModel.EncryptedPhysicalRoadCode;
                model.OldCompleted = roadModel.OldCompleted;
                model.Sanction_length = roadModel.Sanction_length;
                model.CompleteStatus = roadModel.CompleteStatus;
                model.PreviousMonth = roadModel.PreviousMonth;
                model.PreviousYear = roadModel.PreviousYear;
                model.Operation = roadModel.Operation;
                model.PreviousBaseCourse = roadModel.PreviousBaseCourse;
                model.PreviousEarthWork = roadModel.PreviousEarthWork;
                model.PreviousMiscellaneous = roadModel.PreviousMiscellaneous;
                model.PreviousPreparatoryWork = roadModel.PreviousPreparatoryWork;
                model.PreviousSubbase = roadModel.PreviousSubbase;
                model.PreviousSurfaceCourse = roadModel.PreviousSurfaceCourse;
                model.PreviousCDWorks = roadModel.PreviousCDWorks;
                model.PreviousLSB = roadModel.PreviousLSB;
                model.PreviousRoadSigns = roadModel.PreviousRoadSigns;
                model.IsStage = roadModel.IsStage;
                model.AgreementMonth = roadModel.AgreementMonth;
                model.AgreementYear = roadModel.AgreementYear;
                model.AgreementDate = roadModel.AgreementDate;
                model.PreviousCompletedLength = roadModel.PreviousCompletedLength;
                model.changed_SanctionedLength = roadModel.changed_SanctionedLength;
                model.crYear = roadModel.crYear;
                model.currmonthName = roadModel.currmonthName;
                model.prevmonthName = roadModel.prevmonthName;
                model.EXEC_PROG_YEAR = roadModel.EXEC_PROG_YEAR;
                model.Operation = roadModel.Operation;
                model.EXEC_PROG_MONTH = roadModel.EXEC_PROG_MONTH;
                model.EXEC_ISCOMPLETED = roadModel.EXEC_ISCOMPLETED;
                model.EXEC_PREPARATORY_WORK = roadModel.EXEC_PREPARATORY_WORK;
                model.EXEC_EARTHWORK_SUBGRADE = roadModel.EXEC_EARTHWORK_SUBGRADE;
                model.EXEC_SUBBASE_PREPRATION = roadModel.EXEC_SUBBASE_PREPRATION;
                model.EXEC_BASE_COURSE = roadModel.EXEC_BASE_COURSE;
                model.EXEC_SURFACE_COURSE = roadModel.EXEC_SURFACE_COURSE;
                model.EXEC_SIGNS_STONES = roadModel.EXEC_SIGNS_STONES;
                model.EXEC_CD_WORKS = roadModel.EXEC_CD_WORKS;
                model.EXEC_LSB_WORKS = roadModel.EXEC_LSB_WORKS;
                model.EXEC_MISCELANEOUS = roadModel.EXEC_MISCELANEOUS;
                model.EXEC_COMPLETED = roadModel.EXEC_COMPLETED;
                model.ExecutionCompleteDate = roadModel.ExecutionCompleteDate;
                #endregion

                if (model != null)
                {
                    return PartialView("EditPhysicalRoadProgressMRD", model);
                }
                return PartialView("EditPhysicalRoadProgressMRD", new ExecutionRoadStatusViewModelMRD());
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EditPhysicalRoadProgressMRD(String hash, String parameter, String key)");
                return null;
            }
        }


        /// <summary>
        /// updates the physical road details
        /// </summary>
        /// <param name="progressModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Audit]
        public ActionResult EditPhysicalRoadDetailsMRD(ExecutionRoadStatusViewModelMRD progressModel)
        {
            IExecutionBAL objBAL = new ExecutionBAL();
            string message = string.Empty;

            int allowedMonth = DateTime.Now.Month;
            int allowedYear = DateTime.Now.Year;
            try
            {
                //if (DateTime.Now.Day <= 5)
                //{
                //    if (!(((progressModel.EXEC_PROG_MONTH == allowedMonth) || (progressModel.EXEC_PROG_MONTH == (allowedMonth - 1)))
                //           && progressModel.EXEC_PROG_YEAR == allowedYear)
                //       )
                //    {
                //        return Json(new { success = false, message = "Progress Month and Year must be equal to current month and year." }, JsonRequestBehavior.AllowGet);
                //    }
                //}
                //else
                //{
                //    if (!(progressModel.EXEC_PROG_MONTH == allowedMonth && progressModel.EXEC_PROG_YEAR == allowedYear))
                //    {
                //        return Json(new { success = false, message = "Progress Month and Year must be equal to current month and year." }, JsonRequestBehavior.AllowGet);
                //    }
                //}
                if (ModelState.IsValid)
                {
                    if (!objBAL.EditPhysicalRoadDetailsMRDBAL(progressModel, ref message))
                    {
                        return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "EditPhysicalRoadDetailsMRD(ExecutionRoadStatusViewModel progressModel)");
                return null;
            }
        }
        #endregion

        #region Execution Change Work Status
        /// <summary>
        /// returns the list view of Road List details 
        /// </summary>
        /// <returns></returns>

        [Audit]
        [HttpGet]
        public ActionResult ListNewRoadList()
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                NewRoadList RoadListModel = new NewRoadList();
                List<SelectListItem> lstBatches = new List<SelectListItem>();
                TransactionParams transactionParams = new TransactionParams();
                transactionParams.STATE_CODE = PMGSYSession.Current.StateCode;
                transactionParams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                transactionParams.ISSearch = true;
                transactionParams.SANC_YEAR = (Int16)DateTime.Now.Year;
                lstBatches = objCommon.PopulateBatch();
                lstBatches.RemoveAt(0);
                lstBatches.Add(new SelectListItem { Value = "0", Text = "All Batches" });
                RoadListModel.BATCHS = lstBatches;

                RoadListModel.STATES = objCommon.PopulateStates(true);
                RoadListModel.STATES.Find(x => x.Value == "0").Value = "-1";

                RoadListModel.DISTRICTS.Insert(0, new SelectListItem() { Text = "Select District", Value = "-1", Selected = true });

                RoadListModel.BLOCKS = objCommon.PopulateBlocks(PMGSYSession.Current.DistrictCode, true);
                RoadListModel.Years = PopulateYear(0, true, true);
                RoadListModel.STREAMS = objCommon.PopulateStreams("", true);
                RoadListModel.PROPOSAL_TYPES = objCommon.PopulateProposalTypes();

                //new filters added by Vikram 
                RoadListModel.lstBatchs = objCommon.PopulateBatch(true);
                RoadListModel.lstCollaborations = objCommon.PopulateFundingAgency(true);
                RoadListModel.lstUpgradations = objCommon.PopulateNewUpgradeList(true);
                //end of change

                List<SelectListItem> lstPackages = objCommon.PopulatePackage(transactionParams);
                lstPackages.RemoveAt(0);
                lstPackages.Insert(0, new SelectListItem { Value = "All", Text = "All Packages" });
                RoadListModel.PACKAGES = lstPackages;
                return View("ListNewRoadList", RoadListModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ListNewRoadList()");
                return null;
            }
        }
        /// <summary>
        /// returns the list of Road 
        /// </summary>
        /// <param name="executionCollection">contains the filters and grid parameters</param>
        /// <returns></returns>
        [Audit]
        [HttpGet]
        public ActionResult GetRoadList(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                int yearCode = 0;
                int districtCode = 0;
                int blockCode = 0;
                int streamCode = 0;
                int batchCode = 0;
                string proposalCode = string.Empty;
                string packageCode = string.Empty;
                string upgradationType = string.Empty;
                long totalRecords = 0;
                IExecutionBAL objBAL = new ExecutionBAL();

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["yearCode"]))
                {
                    yearCode = Convert.ToInt32(Request.Params["yearCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["districtCode"]))
                {
                    districtCode = Convert.ToInt32(Request.Params["districtCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["blockCode"]))
                {
                    blockCode = Convert.ToInt32(Request.Params["blockCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["streamCode"]))
                {
                    streamCode = Convert.ToInt32(Request.Params["streamCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["batchCode"]))
                {
                    batchCode = Convert.ToInt32(Request.Params["batchCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["proposalCode"]))
                {
                    proposalCode = Request.Params["proposalCode"];
                }

                if (!string.IsNullOrEmpty(Request.Params["packageCode"]))
                {
                    packageCode = Request.Params["packageCode"];
                }

                if (!string.IsNullOrEmpty(Request.Params["upgradationType"]))
                {
                    upgradationType = Request.Params["upgradationType"];
                }

                var jsonData = new
                {
                    rows = objBAL.GetRoadListBAL(yearCode, districtCode, blockCode, batchCode, streamCode, packageCode, proposalCode, upgradationType, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetRoadList()");
                return null;
            }
        }

        /// <summary>
        /// returns Additional Road Detail 
        /// </summary>

        [Audit]
        public ActionResult AdditionalRoadDetails()
        {
            string urlparameter = Request.Params["urlparameter"];
            try
            {
                AdditionalRoadDetailsViewModel ims_sanctioned_projects = null;

                IExecutionBAL objExecutionBAL = new ExecutionBAL();

                ExecutionAdditionalRoadDetails executionAdditionalRoadDetails = new ExecutionAdditionalRoadDetails();
                Dictionary<string, string> decryptedParameters = null;
                string[] encryptedParams = urlparameter.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedParams[0], encryptedParams[1], encryptedParams[2] });
                int proposalCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);
                ims_sanctioned_projects = objExecutionBAL.GetRoadDetails(proposalCode);

                executionAdditionalRoadDetails.WORK_COMPLETION_DATE = ims_sanctioned_projects.WORK_COMPLETION_DATE;
                executionAdditionalRoadDetails.IMS_PR_ROAD_CODE = proposalCode;
                executionAdditionalRoadDetails.EncryptedRoadCode = urlparameter;
                //set Road Details                                                            
                executionAdditionalRoadDetails.IMS_YEAR = ims_sanctioned_projects.IMS_YEAR;
                executionAdditionalRoadDetails.IMS_BATCH = ims_sanctioned_projects.IMS_BATCH;
                executionAdditionalRoadDetails.IMS_PACKAGE_ID = ims_sanctioned_projects.IMS_PACKAGE_ID;
                executionAdditionalRoadDetails.IMS_ROAD_NAME = ims_sanctioned_projects.IMS_ROAD_NAME;
                executionAdditionalRoadDetails.IMS_PAV_LENGTH = ims_sanctioned_projects.IMS_PAV_LENGTH;
                executionAdditionalRoadDetails.IMS_PR_ROAD_CODE = proposalCode;
                executionAdditionalRoadDetails.IMS_STATE_AMOUNT_TEXT = Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_BS_AMT) + Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_RS_AMT);
                executionAdditionalRoadDetails.IMS_MORD_AMOUNT_TEXT = Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_PAV_AMT) + Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_CD_AMT) + Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_OW_AMT) + Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_HS_AMT) + Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_FC_AMT) + Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_BW_AMT);
                return View(executionAdditionalRoadDetails);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AdditionalCostDetails()");
                //return View(new TestResultViewModel());
                return null;
            }
        }

        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult AddAdditionalRoadDetails(FormCollection formCollection)
        {
            string message = string.Empty;
            IExecutionBAL objExecutionBAL = new ExecutionBAL();

            try
            {
                HttpPostedFileBase ClearancePdfFile = Request.Files["fileLetter"];
                string fileTypes = string.Empty;
                string[] arrfiletype = new string[5];
                bool fileExt = false;
                string filename = string.Empty;
                string filePdfSaveExt = string.Empty;
                string filePathPdf = string.Empty;
                bool status = false;

                ExecutionAdditionalRoadDetails executionAdditionalRoadDetails = new ExecutionAdditionalRoadDetails();

                if (ClearancePdfFile != null)
                {
                    fileTypes = ConfigurationManager.AppSettings["Road_Status_Change_pdf_format"];


                    //if (fileTypes == ClearancePdfFile.FileName.Split('.')[1])
                    if (fileTypes == Path.GetExtension(ClearancePdfFile.FileName.Trim()).Split('.')[1])
                    {
                        fileExt = true;
                        filePdfSaveExt = fileTypes;
                        filePathPdf = ConfigurationManager.AppSettings["Road_Status_Change"];
                        executionAdditionalRoadDetails.FILE_TYPE = "P";
                    }

                    if (fileExt == false)
                    {
                        message = "selected File type is not allowed.";
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    message = "Please upload file.";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
                DateTime Exec_Approved_date = DateTime.Now;
                var proposalCode = Convert.ToInt32(formCollection["IMS_PR_ROAD_CODE"]);

                executionAdditionalRoadDetails.IMS_PR_ROAD_CODE = proposalCode;
                executionAdditionalRoadDetails.WORK_COMPLETION_DATE = Convert.ToDateTime(formCollection["WORK_COMPLETION_DATE"]);
                executionAdditionalRoadDetails.EXEC_APPROVED_DATE = Exec_Approved_date;

                if (ClearancePdfFile != null)
                {
                    executionAdditionalRoadDetails.FILE_NAME = ClearancePdfFile.FileName;
                    Request.Files["fileLetter"].SaveAs(Path.Combine(filePathPdf, ClearancePdfFile.FileName));
                }

                if (ModelState.IsValid)
                {

                    if (objExecutionBAL.AddAdditionalRoadDetailsBAL(executionAdditionalRoadDetails, ref message) && objExecutionBAL.EditAdditionalRoadDetailsBAL(proposalCode))
                    {
                        message = message == string.Empty ? "Work status has been Modified successfully." : message;
                        return Json(new { success = true, message = message });
                    }
                    else
                    {
                        message = message == string.Empty ? "Work status has not been Modified" : message;
                        return Json(new { success = false, message = message });
                    }
                }
                else
                {

                    string messages = string.Join("; ", ModelState.Values
                                           .SelectMany(x => x.Errors)
                                           .Select(x => x.ErrorMessage));

                    return View("AdditionalRoadDetails", executionAdditionalRoadDetails);
                }
            }
            catch (DbEntityValidationException ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "AddAdditionalRoadDetails(DbEntityValidationException ex)");
                foreach (var eve in ex.EntityValidationErrors)
                {
                    ModelState.AddModelError("", eve.ValidationErrors.ToString());
                    message += eve.ValidationErrors.ToString();
                }
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "AddAdditionalRoadDetails()");
                message = "Work Status not changed because " + ex.Message;
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        //Get Additional Road Details List

        //[Audit]
        //public ActionResult GetAdditionalRoadList(FormCollection formCollection)
        //{

        //    using (CommonFunctions commonFunction = new CommonFunctions())
        //    {
        //        if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
        //        {
        //            return null;
        //        }
        //    }
        //    long totalRecords = 0;
        //    int IMS_PR_ROAD_CODE = 0;

        //    IExecutionBAL objProposalBAL = new ExecutionBAL();

        //    try
        //    {


        //        if (!string.IsNullOrEmpty(formCollection["IMS_PR_ROAD_CODE"]))
        //        {
        //            IMS_PR_ROAD_CODE = Convert.ToInt32(formCollection["IMS_PR_ROAD_CODE"]);
        //        }


        //        var jsonData = new
        //        {
        //            rows = objProposalBAL.GetAdditionalRoadListBAL(IMS_PR_ROAD_CODE, Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords),
        //            total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
        //            page = Convert.ToInt32(formCollection["page"]),
        //            records = totalRecords
        //        };
        //        return Json(jsonData, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "GetAdditionalCostList()");
        //        return null;
        //    }
        //}

        //Download file of Additional Road Details
        //[Audit]
        //public ActionResult DownloadAdditionalRoadDetailsFile(string parameter, string hash, string key)
        //{
        //    try
        //    {
        //        string FileName = string.Empty;
        //        string FullFileLogicalPath = string.Empty;
        //        string FullfilePhysicalPath = string.Empty;
        //        string FileExtension = string.Empty;

        //        if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
        //        {
        //            String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
        //            if (urlParams.Length >= 1)
        //            {
        //                String[] urlSplitParams = urlParams[0].Split('$');
        //                FileName = (urlSplitParams[0]);
        //            }
        //        }
        //        FileExtension = Path.GetExtension(FileName).ToLower();

        //        if (FileExtension == ".pdf")
        //        {
        //            FullfilePhysicalPath = Path.Combine(ConfigurationManager.AppSettings["ADDITIONAL_ROAD_DETAILS_PDF_UPLOAD"], FileName);
        //        }

        //        string name = Path.GetFileName(FileName);
        //        string ext = Path.GetExtension(FileName);

        //        string type = string.Empty;

        //        if (ext != null)
        //        {
        //            switch (ext.ToLower())
        //            {
        //                case ".pdf":
        //                    type = "Application/pdf";
        //                    break;
        //                case ".doc":
        //                case ".docx":
        //                    type = "Application/msword";
        //                    break;
        //                case ".jpg":
        //                case ".bmp":
        //                case ".tiff":
        //                case ".png":
        //                case ".gif":
        //                case ".jpeg":
        //                    type = "image/png";
        //                    break;
        //                default:
        //                    type = "Application";
        //                    break;
        //            }
        //        }

        //        if (System.IO.File.Exists(FullfilePhysicalPath))
        //        {
        //            return File(FullfilePhysicalPath, type, DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + FileExtension);
        //        }
        //        else
        //        {
        //            return Json(new { Success = "false", ErrorMessage = "File Not Found." }, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "DownloadAdditionalRoadDetailsFile()");
        //        return Json(new { Success = "false", ErrorMessage = "Error While Processing Your Request." }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        #endregion

        #region Location Details Added By Rohit J.

        [HttpPost]
        public JsonResult GetTrackingForExecution(FormCollection formCollection)
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                long totalRecords = 0;
                ExecutionDAL objDAL = new ExecutionDAL();
                if (true)
                {//objCommon.ValidateDataTableParameters(formCollection)
                    var jsonData = new
                    {
                        data = objDAL.GetTrackingForExecutionDAL(formCollection, out totalRecords),
                        draw = formCollection["draw"],
                        recordsTotal = totalRecords,
                        recordsFiltered = totalRecords
                    };

                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }

        }


        [Audit]
        public ActionResult ViewLocationDetails(string urlparameter)
        {
            String[] encryptedParameters = null;
            Dictionary<string, string> decryptedParameters = null;
            int proposalCode = 0;
            try
            {
                ProposalDetailsForLocationTracking execModel = new ProposalDetailsForLocationTracking();
                encryptedParameters = urlparameter.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                proposalCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);


                ExecutionDAL objDAL = new ExecutionDAL();
                execModel = objDAL.GetProposalDetailsForLocationDAL(proposalCode);
                execModel.Operation = "A";


                execModel.ProposalCode = proposalCode;

                return View("ViewLocationDetails", execModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Request can not be processed at this time." });
            }
        }

        #endregion

        #region Downloading Exec Location Details Added By Rohit J. 30 Nov 2018


        [HttpGet]
        public ActionResult DownloadExecLocationDetails(string ProposalCode)
        {

            ProposalDetailsForLocationTracking reportmodel = new ProposalDetailsForLocationTracking();

            string file;

            try
            {
                reportmodel.UserName = ConfigurationManager.AppSettings["MvcReportViewer.Username"]; //UserName for MVC Report Viewer
                reportmodel.Password = ConfigurationManager.AppSettings["MvcReportViewer.Password"]; //Password for MVC Report Viewer
                reportmodel.ReportServerUrl = ConfigurationManager.AppSettings["MvcReportViewer.ReportServerUrl"]; //MVC Report Viewer URL
                reportmodel.FolderNameAndReportName = "/PMGSYCitizen/DownloadLatLongDetails";  //e-Authorization Report URL


                Int32 ProposalCode1 = Convert.ToInt32(ProposalCode);
                reportmodel.ReportParameter = new { RoadCode = ProposalCode1 };

                //  reportmodel.ReportParameter = reportmodel.ProposalCode;


                if (reportmodel.ReportParameter != null)
                    reportmodel.QueryString = CreateQueryStringForSSRSReport(reportmodel.ReportParameter);


                reportmodel.FileBytes = ConvertSSRReportToBytes(reportmodel);
                var filename = "ExecutionLocationDetails" + reportmodel.ProposalCode + ".pdf";
                string ExecutionLocationDetails = ConfigurationManager.AppSettings["ExecutionLocationDetails"];  //Directory Name
                if (!Directory.Exists(ExecutionLocationDetails))
                {
                    Directory.CreateDirectory(ExecutionLocationDetails);
                }

                file = ExecutionLocationDetails + filename;   //File Path AND File Name


                if (System.IO.File.Exists(file))
                {
                    System.IO.File.Delete(file);
                    //   System.IO.File.WriteAllBytes(file, reportmodel.FileBytes);  //Saving File


                    return File(reportmodel.FileBytes, System.Net.Mime.MediaTypeNames.Application.Pdf, "LocationDetails" + DateTime.Now + ".xls");

                }
                else
                {
                    // System.IO.File.WriteAllBytes(file, reportmodel.FileBytes);  //Saving File

                    return File(reportmodel.FileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "LocationDetails" + DateTime.Now + ".xls");

                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Execution.DownloadExecLocationDetails");
                return null;
            }
        }

        public string CreateQueryStringForSSRSReport(object parameter)
        {

            try
            {
                string queryString = "&";
                System.Reflection.PropertyInfo[] paramProperties = parameter.GetType().GetProperties();
                if (paramProperties.Length == 0)
                    return string.Empty;

                for (int i = 0; i < paramProperties.Length; i++)
                {
                    queryString += paramProperties[i].Name + "=" + paramProperties[i].GetValue(parameter) + "&";
                }

                queryString = queryString.TrimEnd('&');
                return queryString;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Execution.DownloadExecLocationDetails");
                return null;
            }
        }


        public byte[] ConvertSSRReportToBytes(ProposalDetailsForLocationTracking model)
        {

            try
            {
                string sub = model.ReportParameter.ToString();

                string queryString = model.QueryString;
                string strReportUser = model.UserName;
                string strReportUserPW = model.Password;


                // string sTargetURL = model.ReportServerUrl + "?" + model.FolderNameAndReportName + "&rs:Command=Render&rs:format=PDF" + queryString;
                string sTargetURL = model.ReportServerUrl + "?" + model.FolderNameAndReportName + "&rs:Command=Render&rs:format=Excel" + queryString;


                System.Net.HttpWebRequest req =
                      (System.Net.HttpWebRequest)System.Net.WebRequest.Create(sTargetURL);
                req.PreAuthenticate = true;
                req.Credentials = new System.Net.NetworkCredential(
                    strReportUser,
                    strReportUserPW);

                System.Net.HttpWebResponse HttpWResp = (System.Net.HttpWebResponse)req.GetResponse();

                Stream fStream = HttpWResp.GetResponseStream();
                //Now turn around and send this as the response..


                byte[] fileBytes = ReadFully(fStream);
                return fileBytes;
                //return fStream;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Execution.DownloadExecLocationDetails");
                return null;
            }
        }

        public static byte[] ReadFully(Stream input)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    input.CopyTo(ms);
                    return ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Execution.DownloadExecLocationDetails");
                return null;
            }
        }

        #endregion

        #region Get Latest Month Physical Progress Details from EXEC_ROADS_MONTHLY_STATUS

        /// <summary>
        /// Populate Agreement based on Selected Contractor
        /// </summary>
        /// <param name="MastConID"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult PopulateLatestPhysicalRoadProgress(int RoadCode)
        {
            PMGSYEntities dbContext = null;
            try
            {
                dbContext = new PMGSYEntities();
                decimal EXEC_PREPARATORY_WORK = 0;
                decimal EXEC_EARTHWORK_SUBGRADE = 0;
                decimal EXEC_SUBBASE_PREPRATION = 0;
                decimal EXEC_BASE_COURSE = 0;
                decimal EXEC_SURFACE_COURSE = 0;
                decimal EXEC_SIGNS_STONES = 0;
                decimal EXEC_CD_WORKS = 0;
                decimal EXEC_LSB_WORKS = 0;
                decimal EXEC_MISCELANEOUS = 0;
                decimal EXEC_COMPLETED = 0;
                string ReadonlyFieldFlag = string.Empty;

                if (dbContext.EXEC_ROADS_MONTHLY_STATUS.Any(m => m.IMS_PR_ROAD_CODE == RoadCode))
                {
                    EXEC_ROADS_MONTHLY_STATUS objPhysicalProgress = dbContext.EXEC_ROADS_MONTHLY_STATUS.Where(m => m.IMS_PR_ROAD_CODE == RoadCode).OrderByDescending(m => m.EXEC_PROG_YEAR).ThenByDescending(m => m.EXEC_PROG_MONTH).FirstOrDefault();
                    EXEC_PREPARATORY_WORK = Convert.ToDecimal(objPhysicalProgress.EXEC_PREPARATORY_WORK);
                    EXEC_EARTHWORK_SUBGRADE = Convert.ToDecimal(objPhysicalProgress.EXEC_EARTHWORK_SUBGRADE);
                    EXEC_SUBBASE_PREPRATION = Convert.ToDecimal(objPhysicalProgress.EXEC_SUBBASE_PREPRATION);
                    EXEC_BASE_COURSE = Convert.ToDecimal(objPhysicalProgress.EXEC_BASE_COURSE);
                    EXEC_SURFACE_COURSE = Convert.ToDecimal(objPhysicalProgress.EXEC_SURFACE_COURSE);
                    EXEC_SIGNS_STONES = Convert.ToDecimal(objPhysicalProgress.EXEC_SIGNS_STONES);
                    EXEC_CD_WORKS = Convert.ToDecimal(objPhysicalProgress.EXEC_CD_WORKS);
                    EXEC_LSB_WORKS = Convert.ToDecimal(objPhysicalProgress.EXEC_LSB_WORKS);
                    EXEC_MISCELANEOUS = Convert.ToDecimal(objPhysicalProgress.EXEC_MISCELANEOUS);
                    EXEC_COMPLETED = Convert.ToDecimal(objPhysicalProgress.EXEC_COMPLETED);
                    return Json(new { success = true, EXEC_PREPARATORY_WORK = EXEC_PREPARATORY_WORK, EXEC_EARTHWORK_SUBGRADE = EXEC_EARTHWORK_SUBGRADE, EXEC_SUBBASE_PREPRATION = EXEC_SUBBASE_PREPRATION, EXEC_BASE_COURSE = EXEC_BASE_COURSE, EXEC_SURFACE_COURSE = EXEC_SURFACE_COURSE, EXEC_SIGNS_STONES = EXEC_SIGNS_STONES, EXEC_CD_WORKS = EXEC_CD_WORKS, EXEC_LSB_WORKS = EXEC_LSB_WORKS, EXEC_MISCELANEOUS = EXEC_MISCELANEOUS, EXEC_COMPLETED = EXEC_COMPLETED, ReadonlyFieldFlag = "Y" });

                }
                else
                {
                    EXEC_PREPARATORY_WORK = 0;
                    EXEC_EARTHWORK_SUBGRADE = 0;
                    EXEC_SUBBASE_PREPRATION = 0;
                    EXEC_BASE_COURSE = 0;
                    EXEC_SURFACE_COURSE = 0;
                    EXEC_SIGNS_STONES = 0;
                    EXEC_CD_WORKS = 0;
                    EXEC_LSB_WORKS = 0;
                    EXEC_MISCELANEOUS = 0;
                    EXEC_COMPLETED = 0;
                    return Json(new { success = true, EXEC_PREPARATORY_WORK = EXEC_PREPARATORY_WORK, EXEC_EARTHWORK_SUBGRADE = EXEC_EARTHWORK_SUBGRADE, EXEC_SUBBASE_PREPRATION = EXEC_SUBBASE_PREPRATION, EXEC_BASE_COURSE = EXEC_BASE_COURSE, EXEC_SURFACE_COURSE = EXEC_SURFACE_COURSE, EXEC_SIGNS_STONES = EXEC_SIGNS_STONES, EXEC_CD_WORKS = EXEC_CD_WORKS, EXEC_LSB_WORKS = EXEC_LSB_WORKS, EXEC_MISCELANEOUS = EXEC_MISCELANEOUS, EXEC_COMPLETED = EXEC_COMPLETED, ReadonlyFieldFlag = "N" });

                }
            }
            catch (Exception ex)
            {

                ErrorLog.LogError(ex, "ExecutionController.PopulateLatestPhysicalRoadProgress(int RoadCode)");
                return null;
            }
        }

        #endregion

        #region Building Proposal

        ///Building Progress Layout
        [Audit]
        [HttpGet]
        public ActionResult ListExecBuildingProgress()
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                ProposalFilterViewModel proposalModel = new ProposalFilterViewModel();
                List<SelectListItem> lstBatches = new List<SelectListItem>();
                TransactionParams transactionParams = new TransactionParams();
                transactionParams.STATE_CODE = PMGSYSession.Current.StateCode;
                transactionParams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                transactionParams.ISSearch = true;
                transactionParams.SANC_YEAR = (Int16)DateTime.Now.Year;
                lstBatches = objCommon.PopulateBatch();
                lstBatches.RemoveAt(0);
                lstBatches.Add(new SelectListItem { Value = "0", Text = "All Batches" });
                proposalModel.BATCHS = lstBatches;
                proposalModel.BLOCKS = objCommon.PopulateBlocks(PMGSYSession.Current.DistrictCode, true);
                proposalModel.Years = PopulateYear(0, true, true);
                proposalModel.STREAMS = objCommon.PopulateStreams("", true);
                //proposalModel.PROPOSAL_TYPES = objCommon.PopulateProposalTypes();

                proposalModel.PROPOSAL_TYPES = new List<SelectListItem>();
                proposalModel.PROPOSAL_TYPES.Insert(0, new SelectListItem() { Text = "Building", Value = "B" });


                //new filters added by Vikram 
                proposalModel.lstBatchs = objCommon.PopulateBatch(true);
                proposalModel.lstCollaborations = objCommon.PopulateFundingAgency(true);
                proposalModel.lstUpgradations = objCommon.PopulateNewUpgradeList(true);
                //end of change

                List<SelectListItem> lstPackages = objCommon.PopulatePackage(transactionParams);
                lstPackages.RemoveAt(0);
                lstPackages.Insert(0, new SelectListItem { Value = "All", Text = "All Packages" });
                proposalModel.PACKAGES = lstPackages;
                return View("ListExecBuildingProgress", proposalModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ListExecBuildingProgress()");
                return null;
            }
        }

        [HttpGet]
        public ActionResult GetExecBuildingProposalList(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                int yearCode = 0;
                int blockCode = 0;
                int streamCode = 0;
                int batchCode = 0;
                string proposalCode = string.Empty;
                string packageCode = string.Empty;
                string upgradationType = string.Empty;
                long totalRecords = 0;
                ExecutionDAL objDAL = new ExecutionDAL();

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["yearCode"]))
                {
                    yearCode = Convert.ToInt32(Request.Params["yearCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["blockCode"]))
                {
                    blockCode = Convert.ToInt32(Request.Params["blockCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["streamCode"]))
                {
                    streamCode = Convert.ToInt32(Request.Params["streamCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["batchCode"]))
                {
                    batchCode = Convert.ToInt32(Request.Params["batchCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["proposalCode"]))
                {
                    proposalCode = Request.Params["proposalCode"];
                }

                if (!string.IsNullOrEmpty(Request.Params["packageCode"]))
                {
                    packageCode = Request.Params["packageCode"];
                }

                if (!string.IsNullOrEmpty(Request.Params["upgradationType"]))
                {
                    upgradationType = Request.Params["upgradationType"];
                }

                var jsonData = new
                {
                    rows = objDAL.GetExecBuildingProposalListDAL(yearCode, blockCode, packageCode, proposalCode, upgradationType, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetExecBuildingProposalList()");
                return null;
            }
        }

        ///Excavation Layout
        [HttpGet]
        public ActionResult ListEarthWorkExcavation(String parameter, String hash, String key)
        {
            EarthWorkExcavationViewModel model = new EarthWorkExcavationViewModel();
            try
            {
                model.EncrProposalCode = parameter + "/" + hash + "/" + key;//URLEncrypt.EncryptParameters1(new string[] { "ProposalCode=" + imsPrRoadCode.ToString().Trim() });
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ListEarthWorkExcavation()");
                return null;
            }
        }

        [HttpGet]
        public ActionResult EarthWorkExcavationLayout(String parameter, String hash, String key)
        {
            EarthWorkExcavationViewModel model = new EarthWorkExcavationViewModel();
            CommonFunctions comm = new CommonFunctions();
            Dictionary<string, string> decryptedParameters = null;
            int imsPrRoadCode = 0, progressCode = 0;
            ExecutionDAL objDAL = new ExecutionDAL();

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                //Add
                if (decryptedParameters.Count() == 1)
                {
                    imsPrRoadCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);

                    model.lstYear = comm.PopulateYears(true);
                    model.lstMonth = comm.PopulateMonths(true);

                    //model.lstEarthWorkExcavationPCC = new List<SelectListItem>();
                    //model.lstEarthWorkExcavationPCC.Insert(0, new SelectListItem() { Text = "Yes", Value = "Y" });
                    //model.lstEarthWorkExcavationPCC.Insert(1, new SelectListItem() { Text = "No", Value = "N" });
                    //model.lstEarthWorkExcavationPCC.Insert(2, new SelectListItem() { Text = "Not Applicable", Value = "NA" });

                    model.lstEarthWorkExcavationPCC = comm.PopulateItemProgress();

                    model.EncrProposalCode = URLEncrypt.EncryptParameters1(new string[] { "ProposalCode=" + imsPrRoadCode.ToString().Trim() });
                }
                else
                {
                    //Edit
                    progressCode = Convert.ToInt32(decryptedParameters["ProgressCode"]);
                    imsPrRoadCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);

                    model = objDAL.GetEarthworkExcavationProgressDetailsDAL(progressCode);

                    //model.lstYear = comm.PopulateYears(true);
                    //model.lstMonth = comm.PopulateMonths(true);
                    //model.lstEarthWorkExcavationPCC = new List<SelectListItem>();
                    //model.lstEarthWorkExcavationPCC.Insert(0, new SelectListItem() { Text = "Yes", Value = "Y" });
                    //model.lstEarthWorkExcavationPCC.Insert(1, new SelectListItem() { Text = "No", Value = "N" });
                    //model.lstEarthWorkExcavationPCC.Insert(2, new SelectListItem() { Text = "Not Applicable", Value = "NA" });


                    model.lstEarthWorkExcavationPCC = comm.PopulateItemProgress();

                    model.lstYear = new List<SelectListItem>();
                    model.lstYear.Insert(0, new SelectListItem() { Text = model.Year.ToString().Trim(), Value = model.Year.ToString() });

                    model.lstMonth = new List<SelectListItem>();
                    model.lstMonth.Insert(0, new SelectListItem() { Text = new System.Globalization.DateTimeFormatInfo().GetMonthName(model.Month).Trim(), Value = model.Month.ToString() });

                    model.EncrProgressCode = URLEncrypt.EncryptParameters1(new string[] { "ProgressCode=" + progressCode.ToString().Trim() });

                    model.EncrProposalCode = URLEncrypt.EncryptParameters1(new string[] { "ProposalCode=" + imsPrRoadCode.ToString().Trim() });
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ListExecBuildingProgress()");
                return null;
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Audit]
        public ActionResult AddEarthworkExcavationDetails(EarthWorkExcavationViewModel model)
        {
            ExecutionDAL objDAL = new ExecutionDAL();
            string message = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    if (!(objDAL.AddEarthworkExcavationDetailsDAL(model, ref message)))
                    {
                        if (message == string.Empty)
                        {
                            return Json(new { success = false, message = "Earthwork Excavation PCC details not added." });
                        }
                        else
                        {
                            return Json(new { success = false, message = message });
                        }
                    }
                    else
                    {
                        return Json(new { success = true, message = message });
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AddEarthworkExcavationDetails()");
                return Json(new { success = false, message = "Error Occurred while processing your request." });
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Audit]
        public ActionResult EditEarthworkExcavationDetails(EarthWorkExcavationViewModel model)
        {
            ExecutionDAL objDAL = new ExecutionDAL();
            string message = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    if (!(objDAL.EditEarthworkExcavationDetailsDAL(model, ref message)))
                    {
                        if (message == string.Empty)
                        {
                            return Json(new { success = false, message = "Earthwork Excavation PCC details not added." });
                        }
                        else
                        {
                            return Json(new { success = false, message = message });
                        }
                    }
                    else
                    {
                        return Json(new { success = true, message = message });
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EditEarthworkExcavationDetails()");
                return Json(new { success = false, message = "Error Occurred on Edit Earthwork Excavation Details." });
            }
        }

        [HttpGet]
        public ActionResult GetEarthworkExcavationDetailsList(int? page, int? rows, string sidx, string sord)
        {
            Dictionary<string, string> decryptedParameters = null;
            string[] param = null;
            int proposalCode = 0;

            try
            {
                long totalRecords = 0;
                ExecutionDAL objDAL = new ExecutionDAL();

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["proposalCode"]))
                {
                    param = Request.Params["proposalCode"].Split('/');

                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { param[0], param[1], param[2] });

                    if (decryptedParameters.Count() > 0)
                    {
                        proposalCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);
                    }
                }
                var jsonData = new
                {
                    rows = objDAL.GetEarthworkExcavationDetailsListDAL(proposalCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetEarthworkExcavationDetailsList()");
                return null;
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult DeleteEarthworkExcavationDetails(String parameter, String hash, String key)
        {
            ExecutionDAL objDAL = new ExecutionDAL();
            string message = string.Empty;
            Dictionary<string, string> decryptedParameters = null;
            String[] encryptedParameters = null;
            int progressCode = 0;

            try
            {
                //encryptedParameters = id.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                progressCode = Convert.ToInt32(decryptedParameters["ProgressCode"]);
                if (!(objDAL.DeleteEarthworkExcavationDetailsDAL(progressCode, ref message)))
                {
                    if (message == string.Empty)
                    {
                        return Json(new { success = false, message = "Earthwork Excavation details deleted successfully." });
                    }
                    else
                    {
                        return Json(new { success = false, message = message });
                    }
                }
                else
                {
                    return Json(new { success = true, message = message });
                }

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DeleteEarthworkExcavationDetails()");
                return Json(new { success = false, message = "Error Occurred on Delete Earthwork Excavation Details." });
            }
        }

        ///Foundation Layout
        [HttpGet]
        public ActionResult ListFoundationDetails(String parameter, String hash, String key)
        {
            FoundationViewModel model = new FoundationViewModel();
            try
            {
                model.EncrProposalCode = parameter + "/" + hash + "/" + key;
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ListFoundationDetails()");
                return null;
            }
        }

        [HttpGet]
        public ActionResult GetFoundationDetailsList(int? page, int? rows, string sidx, string sord)
        {
            Dictionary<string, string> decryptedParameters = null;
            string[] param = null;
            int proposalCode = 0;

            try
            {
                long totalRecords = 0;
                ExecutionDAL objDAL = new ExecutionDAL();

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["proposalCode"]))
                {
                    param = Request.Params["proposalCode"].Split('/');

                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { param[0], param[1], param[2] });

                    if (decryptedParameters.Count() > 0)
                    {
                        proposalCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);
                    }
                }
                var jsonData = new
                {
                    rows = objDAL.GetFoundationDetailsListDAL(proposalCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetFoundationDetailsList()");
                return null;
            }
        }

        [HttpGet]
        public ActionResult FoundationLayout(String parameter, String hash, String key)
        {
            FoundationViewModel model = new FoundationViewModel();
            CommonFunctions comm = new CommonFunctions();
            Dictionary<string, string> decryptedParameters = null;
            int imsPrRoadCode = 0, progressCode = 0;
            ExecutionDAL objDAL = new ExecutionDAL();

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                //Add
                if (decryptedParameters.Count() == 1)
                {
                    imsPrRoadCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);

                    model.lstYear = comm.PopulateYears(true);
                    model.lstMonth = comm.PopulateMonths(true);

                    model.lstSubComponent = comm.PopulateFoundationSubcomponent();
                    model.lstFoundation = comm.PopulateItemProgress();

                    model.EncrProposalCode = URLEncrypt.EncryptParameters1(new string[] { "ProposalCode=" + imsPrRoadCode.ToString().Trim() });
                }
                else
                {
                    //Edit
                    progressCode = Convert.ToInt32(decryptedParameters["ProgressCode"]);
                    imsPrRoadCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);

                    model = objDAL.GetFoundationProgressDetailsDAL(progressCode);

                    model.lstSubComponent = comm.PopulateFoundationSubcomponent();
                    model.lstSubComponent.Remove(model.lstSubComponent.Find(c => c.Value != model.SubComponent.ToString()));

                    model.lstFoundation = comm.PopulateItemProgress();


                    model.lstYear = new List<SelectListItem>();
                    model.lstYear.Insert(0, new SelectListItem() { Text = model.Year.ToString().Trim(), Value = model.Year.ToString() });

                    model.lstMonth = new List<SelectListItem>();
                    model.lstMonth.Insert(0, new SelectListItem() { Text = new System.Globalization.DateTimeFormatInfo().GetMonthName(model.Month).Trim(), Value = model.Month.ToString() });

                    model.EncrProgressCode = URLEncrypt.EncryptParameters1(new string[] { "ProgressCode=" + progressCode.ToString().Trim() });

                    model.EncrProposalCode = URLEncrypt.EncryptParameters1(new string[] { "ProposalCode=" + imsPrRoadCode.ToString().Trim() });
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ListExecBuildingProgress()");
                return null;
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Audit]
        public ActionResult AddFoundationDetails(FoundationViewModel model)
        {
            ExecutionDAL objDAL = new ExecutionDAL();
            string message = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    if (!(objDAL.AddFoundationDetailsDAL(model, ref message)))
                    {
                        if (message == string.Empty)
                        {
                            return Json(new { success = false, message = "Foundation details not added." });
                        }
                        else
                        {
                            return Json(new { success = false, message = message });
                        }
                    }
                    else
                    {
                        return Json(new { success = true, message = message });
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AddFoundationDetails()");
                return Json(new { success = false, message = "Error Occurred while on Add Foundation Details." });
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Audit]
        public ActionResult EditFoundationDetails(FoundationViewModel model)
        {
            ExecutionDAL objDAL = new ExecutionDAL();
            string message = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    if (!(objDAL.EditFoundationDetailsDAL(model, ref message)))
                    {
                        if (message == string.Empty)
                        {
                            return Json(new { success = false, message = "Foundation details not added." });
                        }
                        else
                        {
                            return Json(new { success = false, message = message });
                        }
                    }
                    else
                    {
                        return Json(new { success = true, message = message });
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EditFoundationDetails()");
                return Json(new { success = false, message = "Error Occurred while on Edit Foundation Details." });
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Audit]
        public ActionResult DeleteFoundationDetails(String parameter, String hash, String key)
        {
            ExecutionDAL objDAL = new ExecutionDAL();
            string message = string.Empty;
            Dictionary<string, string> decryptedParameters = null;
            String[] encryptedParameters = null;
            int progressCode = 0;

            try
            {
                //encryptedParameters = id.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                progressCode = Convert.ToInt32(decryptedParameters["ProgressCode"]);
                if (!(objDAL.DeleteFoundationDetailsDAL(progressCode, ref message)))
                {
                    if (message == string.Empty)
                    {
                        return Json(new { success = false, message = "Foundation details deleted successfully." });
                    }
                    else
                    {
                        return Json(new { success = false, message = message });
                    }
                }
                else
                {
                    return Json(new { success = true, message = message });
                }

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DeleteFoundationDetails()");
                return Json(new { success = false, message = "Error Occurred on Delete Foundation Details." });
            }
        }

        ///Superstructure Layout
        [HttpGet]
        public ActionResult ListSuperstructureDetails(String parameter, String hash, String key)
        {
            SuperstructureViewModel model = new SuperstructureViewModel();
            try
            {
                model.EncrProposalCode = parameter + "/" + hash + "/" + key;
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ListSuperstructureDetails()");
                return null;
            }
        }

        [HttpGet]
        public ActionResult GetSuperstructureDetailsList(int? page, int? rows, string sidx, string sord)
        {
            Dictionary<string, string> decryptedParameters = null;
            string[] param = null;
            int proposalCode = 0;

            try
            {
                long totalRecords = 0;
                ExecutionDAL objDAL = new ExecutionDAL();

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["proposalCode"]))
                {
                    param = Request.Params["proposalCode"].Split('/');

                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { param[0], param[1], param[2] });

                    if (decryptedParameters.Count() > 0)
                    {
                        proposalCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);
                    }
                }
                var jsonData = new
                {
                    rows = objDAL.GetSuperstructureDetailsListDAL(proposalCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetSuperstructureDetailsList()");
                return null;
            }
        }

        [HttpGet]
        public ActionResult SuperstructureLayout(String parameter, String hash, String key)
        {
            SuperstructureViewModel model = new SuperstructureViewModel();
            CommonFunctions comm = new CommonFunctions();
            Dictionary<string, string> decryptedParameters = null;
            int imsPrRoadCode = 0, progressCode = 0;
            ExecutionDAL objDAL = new ExecutionDAL();

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                //Add
                if (decryptedParameters.Count() == 1)
                {
                    imsPrRoadCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);

                    model.lstYear = comm.PopulateYears(true);
                    model.lstMonth = comm.PopulateMonths(true);

                    model.lstSubComponent = comm.PopulateSuperstructureSubcomponent();
                    //model.lstSuperstructure = comm.PopulateItemProgress();

                    model.lstGroundFloor = comm.PopulateItemProgress();
                    model.lstfirstFloor = comm.PopulateItemProgress();
                    model.lstSecondFloor = comm.PopulateItemProgress();
                    model.lstThirdFloor = comm.PopulateItemProgress();
                    model.lstCoveredParking = comm.PopulateItemProgress();
                    model.lstApproachRoad = comm.PopulateItemProgress();

                    //model.lstFloor = comm.PopulateSuperstructureFloor();

                    model.EncrProposalCode = URLEncrypt.EncryptParameters1(new string[] { "ProposalCode=" + imsPrRoadCode.ToString().Trim() });
                }
                else
                {
                    ////Edit
                    progressCode = Convert.ToInt32(decryptedParameters["ProgressCode"]);
                    imsPrRoadCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);

                    model = objDAL.GetSuperstructureProgressDetailsDAL(progressCode);

                    model.lstSubComponent = comm.PopulateSuperstructureSubcomponent();
                    //model.lstFloor = comm.PopulateSuperstructureFloor();

                    model.lstGroundFloor = comm.PopulateItemProgress();
                    model.lstfirstFloor = comm.PopulateItemProgress();
                    model.lstSecondFloor = comm.PopulateItemProgress();
                    model.lstThirdFloor = comm.PopulateItemProgress();
                    model.lstCoveredParking = comm.PopulateItemProgress();
                    model.lstApproachRoad = comm.PopulateItemProgress();

                    model.lstYear = new List<SelectListItem>();
                    model.lstYear.Insert(0, new SelectListItem() { Text = model.Year.ToString().Trim(), Value = model.Year.ToString() });

                    model.lstMonth = new List<SelectListItem>();
                    model.lstMonth.Insert(0, new SelectListItem() { Text = new System.Globalization.DateTimeFormatInfo().GetMonthName(model.Month).Trim(), Value = model.Month.ToString() });

                    model.EncrProgressCode = URLEncrypt.EncryptParameters1(new string[] { "ProgressCode=" + progressCode.ToString().Trim() });

                    model.EncrProposalCode = URLEncrypt.EncryptParameters1(new string[] { "ProposalCode=" + imsPrRoadCode.ToString().Trim() });
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "SuperstructureLayout()");
                return null;
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Audit]
        public ActionResult AddSuperstructureDetails(SuperstructureViewModel model)
        {
            ExecutionDAL objDAL = new ExecutionDAL();
            string message = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    #region Item Progress Validation
                    if (model.SubComponent != 12)
                    {
                        if (model.firstFloor == "Y" && model.groundFloor != "Y")
                        {
                            return Json(new { success = false, message = "Ground floor progress should be Yes." });
                        }
                        if (model.secondFloor == "Y" && model.firstFloor != "Y")
                        {
                            return Json(new { success = false, message = "First floor progress should be Yes." });
                        }
                        if (model.thirdFloor == "Y" && model.secondFloor != "Y")
                        {
                            return Json(new { success = false, message = "Second floor progress should be Yes." });
                        }
                    }
                    else
                    {
                        ///Courtyard
                    }
                    #endregion
                    if (!(objDAL.AddSuperstructureDetailsDAL(model, ref message)))
                    {
                        if (message == string.Empty)
                        {
                            return Json(new { success = false, message = "Superstructure details not added." });
                        }
                        else
                        {
                            return Json(new { success = false, message = message });
                        }
                    }
                    else
                    {
                        return Json(new { success = true, message = message });
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AddSuperstructureDetails()");
                return Json(new { success = false, message = "Error Occurred while on Add Superstructure Details." });
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Audit]
        public ActionResult EditSuperstructureDetails(SuperstructureViewModel model)
        {
            ExecutionDAL objDAL = new ExecutionDAL();
            string message = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    #region Item Progress Validation
                    if (model.SubComponent != 12)
                    {
                        if (model.firstFloor == "Y" && model.groundFloor == "N")
                        {
                            return Json(new { success = false, message = "Ground floor progress should be Yes." });
                        }
                        if (model.secondFloor == "Y" && model.firstFloor == "N")
                        {
                            return Json(new { success = false, message = "First floor progress should be Yes." });
                        }
                        if (model.thirdFloor == "Y" && model.secondFloor == "N")
                        {
                            return Json(new { success = false, message = "Second floor progress should be Yes." });
                        }
                    }
                    else
                    {
                        ///Courtyard
                    }
                    #endregion
                    if (!(objDAL.EditSuperstructureDetailsDAL(model, ref message)))
                    {
                        if (message == string.Empty)
                        {
                            return Json(new { success = false, message = "Superstructure details not added." });
                        }
                        else
                        {
                            return Json(new { success = false, message = message });
                        }
                    }
                    else
                    {
                        return Json(new { success = true, message = message });
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EditSuperstructureDetails()");
                return Json(new { success = false, message = "Error Occurred while on Edit Superstructure Details." });
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Audit]
        public ActionResult DeleteSuperstructureDetails(String parameter, String hash, String key)
        {
            ExecutionDAL objDAL = new ExecutionDAL();
            string message = string.Empty;
            Dictionary<string, string> decryptedParameters = null;
            String[] encryptedParameters = null;
            int progressCode = 0;

            try
            {
                //encryptedParameters = id.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                progressCode = Convert.ToInt32(decryptedParameters["ProgressCode"]);
                if (!(objDAL.DeleteSuperstructureDetailsDAL(progressCode, ref message)))
                {
                    if (message == string.Empty)
                    {
                        return Json(new { success = false, message = "Superstructure details deleted successfully." });
                    }
                    else
                    {
                        return Json(new { success = false, message = message });
                    }
                }
                else
                {
                    return Json(new { success = true, message = message });
                }

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DeleteSuperstructureDetails()");
                return Json(new { success = false, message = "Error Occurred on Delete Superstructure Details." });
            }
        }
        #endregion

        #region RSA ATR

        #region Master List of Roads

        [HttpGet]
        public ActionResult RoadSafetyATRProgress()
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                ProposalFilterViewModel proposalModel = new ProposalFilterViewModel();
                List<SelectListItem> lstBatches = new List<SelectListItem>();
                TransactionParams transactionParams = new TransactionParams();

                transactionParams.STATE_CODE = PMGSYSession.Current.StateCode;
                transactionParams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;

                string RoleName = PMGSYSession.Current.RoleName;

                proposalModel.RoleName = RoleName;

                //if (RoleName.Equals("SQC") || RoleName.Equals("RSAuditor"))
                //{
                //    proposalModel.DISTRICTS = objCommon.PopulateDistrictForSRRDA(PMGSYSession.Current.StateCode, false, 0, false, false);
                //}

                if (RoleName.Equals("SQC"))
                {
                    proposalModel.DISTRICTS = objCommon.PopulateDistrictForSRRDA(PMGSYSession.Current.StateCode, false, 0, false, false);
                }

                if (RoleName.Equals("RSAuditor"))
                {
                    if (PMGSYSession.Current.StateCode > 0)
                    {
                        proposalModel.STATES = new List<SelectListItem>();
                        proposalModel.STATES.Insert(0, (new SelectListItem { Text = PMGSYSession.Current.StateName, Value = Convert.ToString(PMGSYSession.Current.StateCode), Selected = true }));
                        proposalModel.DISTRICTS = objCommon.PopulateDistrictForSRRDA(PMGSYSession.Current.StateCode, false, 0, false, false);
                    }
                    else
                    {
                        proposalModel.STATES = objCommon.PopulateStates(true);

                        proposalModel.DISTRICTS.Add(new SelectListItem { Value = "-1", Text = "Select District" });
                    }
                }

                if (RoleName.Equals("RSAAuditorDistrict"))
                {

                    if (PMGSYSession.Current.StateCode > 0 || PMGSYSession.Current.DistrictCode > 0)
                    {
                        proposalModel.STATES = new List<SelectListItem>();
                        proposalModel.STATES.Insert(0, (new SelectListItem { Text = PMGSYSession.Current.StateName, Value = Convert.ToString(PMGSYSession.Current.StateCode), Selected = true }));
                        proposalModel.DISTRICTS = new List<SelectListItem>();
                        proposalModel.DISTRICTS.Insert(0, (new SelectListItem { Text = PMGSYSession.Current.DistrictName, Value = Convert.ToString(PMGSYSession.Current.DistrictCode), Selected = true }));
                    }
                    else
                    {
                        proposalModel.STATES = objCommon.PopulateStates(true);

                        proposalModel.DISTRICTS.Add(new SelectListItem { Value = "-1", Text = "Select District" });
                    }
                }



                transactionParams.ISSearch = true;
                transactionParams.SANC_YEAR = (Int16)DateTime.Now.Year;

                lstBatches = objCommon.PopulateBatch();
                lstBatches.RemoveAt(0);
                lstBatches.Add(new SelectListItem { Value = "0", Text = "All Batches" });
                proposalModel.BATCHS = lstBatches;
                proposalModel.BLOCKS = objCommon.PopulateBlocks(PMGSYSession.Current.DistrictCode, true);
                proposalModel.Years = PopulateYear(0, true, true);
                proposalModel.STREAMS = objCommon.PopulateStreams("", true);
                proposalModel.PROPOSAL_TYPES = objCommon.PopulateProposalTypes();

                proposalModel.lstBatchs = objCommon.PopulateBatch(true);
                proposalModel.lstCollaborations = objCommon.PopulateFundingAgency(true);
                proposalModel.lstUpgradations = objCommon.PopulateNewUpgradeList(true);

                List<SelectListItem> lstPackages = objCommon.PopulatePackage(transactionParams);
                lstPackages.RemoveAt(0);
                lstPackages.Insert(0, new SelectListItem { Value = "All", Text = "All Packages" });
                proposalModel.PACKAGES = lstPackages;
                return View("RoadSafetyATRProgress", proposalModel);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Execution().RoadSafetyATRProgress()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult GetRoadSafetyATRProgressList(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                int yearCode = 0;
                int blockCode = 0;
                int streamCode = 0;
                int batchCode = 0;
                int districtCode = 0;
                string proposalCode = string.Empty;
                string packageCode = string.Empty;
                string upgradationType = string.Empty;
                long totalRecords = 0;
                IExecutionBAL objBAL = new ExecutionBAL();

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["districtCode"]))
                {
                    districtCode = Convert.ToInt32(Request.Params["districtCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["yearCode"]))
                {
                    yearCode = Convert.ToInt32(Request.Params["yearCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["blockCode"]))
                {
                    blockCode = Convert.ToInt32(Request.Params["blockCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["streamCode"]))
                {
                    streamCode = Convert.ToInt32(Request.Params["streamCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["batchCode"]))
                {
                    batchCode = Convert.ToInt32(Request.Params["batchCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["proposalCode"]))
                {
                    proposalCode = Request.Params["proposalCode"];
                }

                if (!string.IsNullOrEmpty(Request.Params["packageCode"]))
                {
                    packageCode = Request.Params["packageCode"];
                }

                if (!string.IsNullOrEmpty(Request.Params["upgradationType"]))
                {
                    upgradationType = Request.Params["upgradationType"];
                }

                var jsonData = new
                {
                    rows = objBAL.RSABALList(districtCode, yearCode, blockCode, packageCode, proposalCode, upgradationType, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Exceution().GetRoadSafetyATRProgressList");
                return null;
            }
        }

        #endregion Master List of Roads

        #region Submitted Records by Auditor

        [HttpGet]
        public ActionResult RoadSafetyATRProgressSubmitted()
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                ProposalFilterViewModel proposalModel = new ProposalFilterViewModel();
                List<SelectListItem> lstBatches = new List<SelectListItem>();
                TransactionParams transactionParams = new TransactionParams();

                transactionParams.STATE_CODE = PMGSYSession.Current.StateCode;
                transactionParams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;

                string RoleName = PMGSYSession.Current.RoleName;

                proposalModel.RoleName = RoleName;

                if (RoleName.Equals("SQC"))
                {
                    proposalModel.DISTRICTS = objCommon.PopulateDistrictForSRRDA(PMGSYSession.Current.StateCode, false, 0, false, false);
                }

                if (RoleName.Equals("RSAuditor"))
                {
                    proposalModel.STATES = objCommon.PopulateStates(true);

                    proposalModel.DISTRICTS.Add(new SelectListItem { Value = "-1", Text = "Select District" });
                }

                if (RoleName.Equals("RSAAuditorDistrict"))
                {

                    if (PMGSYSession.Current.StateCode > 0 || PMGSYSession.Current.DistrictCode > 0)
                    {
                        proposalModel.STATES = new List<SelectListItem>();
                        proposalModel.STATES.Insert(0, (new SelectListItem { Text = PMGSYSession.Current.StateName, Value = Convert.ToString(PMGSYSession.Current.StateCode), Selected = true }));
                        proposalModel.DISTRICTS = new List<SelectListItem>();
                        proposalModel.DISTRICTS.Insert(0, (new SelectListItem { Text = PMGSYSession.Current.DistrictName, Value = Convert.ToString(PMGSYSession.Current.DistrictCode), Selected = true }));
                    }
                    else
                    {
                        proposalModel.STATES = objCommon.PopulateStates(true);

                        proposalModel.DISTRICTS.Add(new SelectListItem { Value = "-1", Text = "Select District" });
                    }
                }




                transactionParams.ISSearch = true;
                transactionParams.SANC_YEAR = (Int16)DateTime.Now.Year;
                lstBatches = objCommon.PopulateBatch();
                lstBatches.RemoveAt(0);
                lstBatches.Add(new SelectListItem { Value = "0", Text = "All Batches" });
                proposalModel.BATCHS = lstBatches;
                proposalModel.BLOCKS = objCommon.PopulateBlocks(PMGSYSession.Current.DistrictCode, true);
                proposalModel.Years = PopulateYear(0, true, true);
                proposalModel.STREAMS = objCommon.PopulateStreams("", true);
                proposalModel.PROPOSAL_TYPES = objCommon.PopulateProposalTypes();

                proposalModel.lstBatchs = objCommon.PopulateBatch(true);
                proposalModel.lstCollaborations = objCommon.PopulateFundingAgency(true);
                proposalModel.lstUpgradations = objCommon.PopulateNewUpgradeList(true);

                List<SelectListItem> lstPackages = objCommon.PopulatePackage(transactionParams);
                lstPackages.RemoveAt(0);
                lstPackages.Insert(0, new SelectListItem { Value = "All", Text = "All Packages" });
                proposalModel.PACKAGES = lstPackages;
                return View("RoadSafetyATRProgressSubmitted", proposalModel);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Execution().RoadSafetyATRProgressSubmitted()");
                return null;
            }
        }


        [HttpPost]
        public ActionResult GetRoadSafetyATRProgressListSubmitted(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                int yearCode = 0;
                int blockCode = 0;
                int streamCode = 0;
                int batchCode = 0;
                int districtCode = 0;
                string proposalCode = string.Empty;
                string packageCode = string.Empty;
                string upgradationType = string.Empty;
                long totalRecords = 0;
                IExecutionBAL objBAL = new ExecutionBAL();

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["districtCode"]))
                {
                    districtCode = Convert.ToInt32(Request.Params["districtCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["yearCode"]))
                {
                    yearCode = Convert.ToInt32(Request.Params["yearCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["blockCode"]))
                {
                    blockCode = Convert.ToInt32(Request.Params["blockCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["streamCode"]))
                {
                    streamCode = Convert.ToInt32(Request.Params["streamCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["batchCode"]))
                {
                    batchCode = Convert.ToInt32(Request.Params["batchCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["proposalCode"]))
                {
                    proposalCode = Request.Params["proposalCode"];
                }

                if (!string.IsNullOrEmpty(Request.Params["packageCode"]))
                {
                    packageCode = Request.Params["packageCode"];
                }

                if (!string.IsNullOrEmpty(Request.Params["upgradationType"]))
                {
                    upgradationType = Request.Params["upgradationType"];
                }

                var jsonData = new
                {
                    rows = objBAL.RSABALListSubmitted(districtCode, yearCode, blockCode, packageCode, proposalCode, upgradationType, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Exceution().GetRoadSafetyATRProgressListSubmitted");
                return null;
            }
        }
        #endregion

        #region Auditor Methods

        [HttpGet]
        public ActionResult GetDetailsForATR(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            String[] encryptedParameters = null;
            var en = parameter + "/" + hash + "/" + key;
            int proposalCode = 0;
            RSAInspectionDetails execModel = new RSAInspectionDetails();
            decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
            proposalCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);

            dbContext = new PMGSYEntities();
            try
            {

                #region
                ExecutionDAL objDAL = new ExecutionDAL();

                execModel = GetFinancialDetails(proposalCode, "P");
                execModel.Operation = "A";
                execModel.IMS_PR_ROAD_CODE = proposalCode;
                execModel.EXEC_PROGRESS_TYPE = "P";
                execModel.IMS_PR_ROAD_CODE = proposalCode;
                #endregion

                int recordCount = 0;
                recordCount = dbContext.EXEC_RSA_INSPECTION.Where(m => m.IMS_PR_ROAD_CODE == proposalCode).Count();

                if (recordCount >= 1)
                {
                    string RsaCode = dbContext.EXEC_RSA_INSPECTION.Where(m => m.IMS_PR_ROAD_CODE == proposalCode).Select(m => m.EXEC_RSA_CODE).FirstOrDefault().ToString();
                    execModel.EXEC_RSA_CODE = Convert.ToInt32(RsaCode);
                    execModel.InspectionDate = dbContext.EXEC_RSA_INSPECTION.Where(m => m.IMS_PR_ROAD_CODE == proposalCode).Select(m => m.EXEC_RSA_INSP_DATE).FirstOrDefault().ToString().Substring(0, 10);
                    string Status = dbContext.EXEC_RSA_INSPECTION.Where(m => m.IMS_PR_ROAD_CODE == proposalCode).Select(m => m.IMS_ROAD_STATUS).FirstOrDefault().ToString();
                    string IsFinalizedByAuditor = dbContext.EXEC_RSA_INSPECTION.Where(m => m.IMS_PR_ROAD_CODE == proposalCode).Select(m => m.EXEC_RSA_AUDIT_SUB).FirstOrDefault().ToString();
                    execModel.IsFinalizedByAuditor = IsFinalizedByAuditor.Equals("Y") ? "Yes" : "No";

                    if (Status.Equals("P"))
                    {
                        execModel.RoadStatus = "Progress";
                    }
                    else if (Status.Equals("A"))
                    {
                        execModel.RoadStatus = "Land Acquisition";
                    }
                    else if (Status.Equals("L"))
                    {
                        execModel.RoadStatus = "Legal";
                    }
                    else if (Status.Equals("F"))
                    {
                        execModel.RoadStatus = "Forest";
                    }
                    else if (Status.Equals("X"))
                    {
                        execModel.RoadStatus = "Maintenance";
                    }
                    else if (Status.Equals("C"))
                    {
                        execModel.RoadStatus = "Completed";
                    }
                    else if (Status.Equals("D"))
                    {
                        execModel.RoadStatus = "Design Stage";
                    }
                    else if (Status.Equals("N"))
                    {
                        execModel.RoadStatus = "Construction";
                    }
                    else if (Status.Equals("G"))
                    {
                        execModel.RoadStatus = "Pre opening";
                    }
                    else if (Status.Equals("O"))
                    {
                        execModel.RoadStatus = "Operation";
                    }
                    else
                    {
                        execModel.RoadStatus = "NA";
                    }
                    // execModel.StartChainage = Convert.ToDecimal(dbContext.EXEC_RSA_INSPECTION_DETAILS.Where(m => m.EXEC_RSA_CODE == execModel.EXEC_RSA_CODE).Max(m => m.EXEC_RSA_END_CHAINAGE));
                    execModel.TotalSegmentEntered = Convert.ToDecimal(dbContext.EXEC_RSA_INSPECTION_DETAILS.Where(m => m.EXEC_RSA_CODE == execModel.EXEC_RSA_CODE).Sum(m => m.EXEC_SEGMENT_LENGTH));
                    execModel.RemainingSegmentLength = execModel.Sanction_length - execModel.TotalSegmentEntered;
                }
                else
                {
                    // execModel.Operation = "Create";
                    execModel.RoadStatus = "NA";
                    execModel.InspectionDate = "NA";
                    execModel.IsFinalizedByAuditor = "NA";
                    //execModel.StartChainage = 0;
                    execModel.TotalSegmentEntered = 0;
                    execModel.RemainingSegmentLength = 0;
                }
                CommonFunctions comm = new CommonFunctions();
                execModel.IssueList = comm.PopulateIssueInRSA(); //new List<SelectListItem>();




                execModel.GradeList = new List<SelectListItem>();
                execModel.GradeList.Insert(0, new SelectListItem() { Text = "Select Severity", Value = "-1", Selected = true });
                execModel.GradeList.Insert(1, new SelectListItem() { Text = "Very High", Value = "U" });
                execModel.GradeList.Insert(2, new SelectListItem() { Text = "High", Value = "R" });
                execModel.GradeList.Insert(3, new SelectListItem() { Text = "Medium", Value = "S" });
                execModel.GradeCode = "-1";


                //execModel.GradeList.Insert(0, new SelectListItem() { Text = "Select Grade", Value = "-1", Selected = true });
                //execModel.GradeList.Insert(1, new SelectListItem() { Text = "Satisfactory", Value = "S" });
                //execModel.GradeList.Insert(2, new SelectListItem() { Text = "Required Improvement ", Value = "R" });
                //execModel.GradeList.Insert(2, new SelectListItem() { Text = "Unsatisfactory", Value = "U" });
                //execModel.GradeCode = "-1";



                // Added on 24 Dec 2019
                execModel.PriorityList = new List<SelectListItem>();
                // execModel.GradeList.Insert(0, new SelectListItem() { Text = "Select Priority", Value = "-1", Selected = true });
                execModel.PriorityList.Insert(0, new SelectListItem() { Text = "Essential", Value = "E", Selected = true });
                execModel.PriorityList.Insert(1, new SelectListItem() { Text = "Highly Desirable", Value = "H" });
                execModel.PriorityList.Insert(2, new SelectListItem() { Text = "Desirable", Value = "D" });
                execModel.PriorityCode = "E";




                execModel.LikelihoodList = new List<SelectListItem>();
                execModel.LikelihoodList.Insert(0, new SelectListItem() { Text = "Select Frequency of Occurrence", Value = "-1", Selected = true });
                execModel.LikelihoodList.Insert(1, new SelectListItem() { Text = "High", Value = "H" });
                execModel.LikelihoodList.Insert(2, new SelectListItem() { Text = "Medium", Value = "M" });
                execModel.LikelihoodList.Insert(3, new SelectListItem() { Text = "Low", Value = "L" });
                execModel.LikelihoodCode = "-1";


                // Inspection Master Fields

                execModel.stageList = new List<SelectListItem>();
                execModel.stageList.Insert(0, new SelectListItem() { Text = "Select RSA Stage", Value = "-1", Selected = true });

                execModel.stageList.Insert(1, new SelectListItem() { Text = "Design Stage", Value = "D" });
                execModel.stageList.Insert(2, new SelectListItem() { Text = "Construction", Value = "N" });
                execModel.stageList.Insert(3, new SelectListItem() { Text = "Pre opening", Value = "G" });
                execModel.stageList.Insert(4, new SelectListItem() { Text = "Operation ", Value = "O" });

                //execModel.stageList.Insert(1, new SelectListItem() { Text = "Progress", Value = "P" });
                //execModel.stageList.Insert(2, new SelectListItem() { Text = "Land Acquisition", Value = "A" });
                //execModel.stageList.Insert(3, new SelectListItem() { Text = "Legal", Value = "L" });
                //execModel.stageList.Insert(4, new SelectListItem() { Text = "Forest ", Value = "F" });
                //execModel.stageList.Insert(5, new SelectListItem() { Text = "Maintenance", Value = "X" });
                //execModel.stageList.Insert(6, new SelectListItem() { Text = "Completed", Value = "C" });


                ExecutionRoadStatusViewModel execRoadModel = objDAL.GetRoadDetails(proposalCode);
                if (execRoadModel != null)
                {
                    execModel.BlockName = execRoadModel.BlockName;
                    execModel.Package = execRoadModel.Package;
                    execModel.RoadName = execRoadModel.RoadName;
                    execModel.AgreementDate = execRoadModel.AgreementDate;
                    execModel.Sanction_Cost = execRoadModel.Sanction_Cost;
                    execModel.Sanction_length = execRoadModel.Sanction_length;
                    execModel.AgreementCost = execRoadModel.AgreementCost;
                    execModel.SanctionYear = execRoadModel.SanctionYear;
                    execModel.changedLength = execRoadModel.changedLength;
                }

                // Inspection Master Fields


                execModel.encryptedURL = en;
                return View("GetDetailsForATR", execModel);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Exceution().GetDetailsForATR");
                return Json(new { success = false, message = "Request can not be processed at this time." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddRSA(RSAInspectionDetails model)
        {
            try
            {
                message = "Error occured while processing your request.";
                // Remove  Inspection Details Validation
                ModelState.Remove("StartChainage");
                ModelState.Remove("EndChainage");
                ModelState.Remove("Safety_Issue");
                ModelState.Remove("RSA_Recommendation");
                ModelState.Remove("GradeCode");
                ModelState.Remove("LikelihoodCode");


                // Remove Remarks By PIU Validations
                ModelState.Remove("AccpetCode");
                ModelState.Remove("ATRUploadDate");
                ModelState.Remove("ATR_By_PIU");
                ModelState.Remove("IssueCode"); // Added on 24 Feb 2020

                if (ModelState.IsValid)
                {
                    ExecutionBAL execBal = new ExecutionBAL();
                    Boolean Status = execBal.AddRSABAL(model, ref message);
                    return Json(new { success = Status, message = message, encryptedRoadCodeID = model.EncryptedRoadCode, encryptedURLID = model.encryptedURL });
                }
                else
                {
                    return Json(new { success = false, message = message });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Execution().AddRSA()");
                return Json(new { success = false, message = message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddRSAdetails(RSAInspectionDetails model)
        {
            try
            {


                message = "Error occured while processing your request.";
                // Remove Inspection Master Validations
                ModelState.Remove("stageCode");
                ModelState.Remove("auditDate");

                // Remove Remarks By PIU Validations
                ModelState.Remove("AccpetCode");
                ModelState.Remove("ATRUploadDate");
                ModelState.Remove("ATR_By_PIU");

                if (model.EndChainage <= model.StartChainage)
                {
                    message = "End Chainage cannot be greater than Start Chainage.";
                    return Json(new { success = false, message = message });

                }
                //else
                //{
                //    decimal difference = model.EndChainage - model.StartChainage;
                //    if (difference > model.Sanction_length)
                //    {
                //        message = "Chainage entred is exceeding total maximum Segment Length i.e. " + model.Sanction_length + " Kms";
                //        return Json(new { success = false, message = message });

                //    }

                //}


                   else
                   {

                    if (model.EndChainage <= model.Sanction_length)
                    {
                        //message = "Chainage entred is exceeding total maximum Segment Length i.e. " + model.Sanction_length + " Kms";
                        //return Json(new { success = false, message = message });

                    }
                    else
                    {
                        message = "End chainage cannot be greater than the segment length i.e. " + model.Sanction_length + " Kms";
                        return Json(new { success = false, message = message });
                    }

                }


                if (ModelState.IsValid)
                {
                    ExecutionBAL execBal = new ExecutionBAL();
                    Boolean Status = execBal.AddRSAdetailsBAL(model, ref message);
                    return Json(new { success = Status, message = message, encryptedURLID = model.encryptedURL, StartChainage = model.EndChainage });
                }
                else
                {
                    return Json(new { success = false, message = message });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Execution().AddRSAdetails()");
                return Json(new { success = false, message = message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FinalizeByAuditor(String parameter, String hash, String key)
        {
            IExecutionBAL objExecutionBAL = new ExecutionBAL();
            Dictionary<string, string> decryptedParameters = null;
            var en = parameter + "/" + hash + "/" + key;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                int ProposalCode = Convert.ToInt32(decryptedParameters["ProposalCode"].ToString());

                string result = objExecutionBAL.FinalizeDetailsByAuditor(ProposalCode); //FinalizeRSAATRBAL
                if (result.Equals(string.Empty))
                {
                    return Json(new { Success = true, Code = en }, JsonRequestBehavior.DenyGet);
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = result, Code = en }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Execution.FinalizeByAuditor()");
                return null;
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteByAuditor(String parameter, String hash, String key)
        {
            IExecutionBAL objExecutionBAL = new ExecutionBAL();
            Dictionary<string, string> decryptedParameters = null;

            var en = parameter + "/" + hash + "/" + key;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                int RSAId = Convert.ToInt32(decryptedParameters["RSAId"].ToString());

                string result = objExecutionBAL.DeleteByAuditorBAL(RSAId); //FinalizeRSAATRBAL
                if (result.Equals(string.Empty))
                {
                    return Json(new { Success = true, Code = en }, JsonRequestBehavior.DenyGet);
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = result, Code = en, encryptedURLID = en }, JsonRequestBehavior.DenyGet);
                }

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Execution().DeleteByAuditor()");
                return null;
            }

        }

        #endregion

        #region  PIU Methods

        [HttpGet]
        public ActionResult PIUATR(string urlparameter)
        {
            String[] encryptedParameters = null;
            Dictionary<string, string> decryptedParameters = null;
            int proposalCode = 0;
            RSAInspectionDetails execModel = new RSAInspectionDetails();
            encryptedParameters = urlparameter.Split('/');
            decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
            proposalCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);

            dbContext = new PMGSYEntities();
            try
            {

                #region
                ExecutionDAL objDAL = new ExecutionDAL();

                execModel = GetFinancialDetails(proposalCode, "P");
                execModel.Operation = "A";
                execModel.IMS_PR_ROAD_CODE = proposalCode;
                execModel.EXEC_PROGRESS_TYPE = "P";
                execModel.IMS_PR_ROAD_CODE = proposalCode;
                #endregion


                int recordCount = 0;
                recordCount = dbContext.EXEC_RSA_INSPECTION.Where(m => m.IMS_PR_ROAD_CODE == proposalCode).Count();

                if (recordCount >= 1)
                {
                    string RsaCode = dbContext.EXEC_RSA_INSPECTION.Where(m => m.IMS_PR_ROAD_CODE == proposalCode).Select(m => m.EXEC_RSA_CODE).FirstOrDefault().ToString();

                    execModel.EXEC_RSA_CODE = Convert.ToInt32(RsaCode);

                    execModel.InspectionDate = dbContext.EXEC_RSA_INSPECTION.Where(m => m.IMS_PR_ROAD_CODE == proposalCode).Select(m => m.EXEC_RSA_INSP_DATE).FirstOrDefault().ToString().Substring(0, 10); ;

                    string Status = dbContext.EXEC_RSA_INSPECTION.Where(m => m.IMS_PR_ROAD_CODE == proposalCode).Select(m => m.IMS_ROAD_STATUS).FirstOrDefault().ToString();

                    if (Status.Equals("P"))
                    {
                        execModel.RoadStatus = "Progress";
                    }
                    else if (Status.Equals("A"))
                    {
                        execModel.RoadStatus = "Land Acquisition";
                    }
                    else if (Status.Equals("L"))
                    {
                        execModel.RoadStatus = "Legal";
                    }
                    else if (Status.Equals("F"))
                    {
                        execModel.RoadStatus = "Forest";
                    }
                    else if (Status.Equals("X"))
                    {
                        execModel.RoadStatus = "Maintenance";
                    }
                    else if (Status.Equals("C"))
                    {
                        execModel.RoadStatus = "Completed";
                    }
                    else if (Status.Equals("D"))
                    {
                        execModel.RoadStatus = "Design Stage";
                    }
                    else if (Status.Equals("N"))
                    {
                        execModel.RoadStatus = "Construction";
                    }
                    else if (Status.Equals("G"))
                    {
                        execModel.RoadStatus = "Pre opening";
                    }
                    else if (Status.Equals("O"))
                    {
                        execModel.RoadStatus = "Operation";
                    }
                    else
                    {
                        execModel.RoadStatus = "NA";
                    }

                }
                else
                {
                    // execModel.Operation = "Create";
                    execModel.RoadStatus = "NA";
                    execModel.InspectionDate = "NA";
                }

                execModel.GradeList = new List<SelectListItem>();
                execModel.GradeList.Insert(0, new SelectListItem() { Text = "Select Grade", Value = "-1", Selected = true });
                execModel.GradeList.Insert(1, new SelectListItem() { Text = "Satisfactory", Value = "S" });
                execModel.GradeList.Insert(2, new SelectListItem() { Text = "Required Improvement ", Value = "R" });
                execModel.GradeList.Insert(2, new SelectListItem() { Text = "Unsatisfactory", Value = "U" });
                execModel.GradeCode = "-1";

                // Inspection Master Fields

                execModel.stageList = new List<SelectListItem>();
                execModel.stageList.Insert(0, new SelectListItem() { Text = "Select RSA Stage", Value = "-1", Selected = true });

                execModel.stageList.Insert(1, new SelectListItem() { Text = "Design Stage", Value = "D" });
                execModel.stageList.Insert(2, new SelectListItem() { Text = "Construction", Value = "N" });
                execModel.stageList.Insert(3, new SelectListItem() { Text = "Pre opening", Value = "G" });
                execModel.stageList.Insert(4, new SelectListItem() { Text = "Operation ", Value = "O" });

                //execModel.stageList.Insert(1, new SelectListItem() { Text = "Progress", Value = "P" });
                //execModel.stageList.Insert(2, new SelectListItem() { Text = "Land Acquisition", Value = "A" });
                //execModel.stageList.Insert(3, new SelectListItem() { Text = "Legal", Value = "L" });
                //execModel.stageList.Insert(4, new SelectListItem() { Text = "Forest ", Value = "F" });
                //execModel.stageList.Insert(5, new SelectListItem() { Text = "Maintenance", Value = "X" });
                //execModel.stageList.Insert(6, new SelectListItem() { Text = "Completed", Value = "C" });


                ExecutionRoadStatusViewModel execRoadModel = objDAL.GetRoadDetails(proposalCode);
                if (execRoadModel != null)
                {
                    execModel.BlockName = execRoadModel.BlockName;
                    execModel.Package = execRoadModel.Package;
                    execModel.RoadName = execRoadModel.RoadName;
                    execModel.AgreementDate = execRoadModel.AgreementDate;
                    execModel.Sanction_Cost = execRoadModel.Sanction_Cost;
                    execModel.Sanction_length = execRoadModel.Sanction_length;
                    execModel.AgreementCost = execRoadModel.AgreementCost;
                    execModel.SanctionYear = execRoadModel.SanctionYear;
                    execModel.changedLength = execRoadModel.changedLength;
                }
                // Inspection Master Fields
                // PIU
                execModel.AccpetList = new List<SelectListItem>();
                execModel.AccpetList.Insert(0, new SelectListItem() { Text = "Select ", Value = "-1", Selected = true });
                execModel.AccpetList.Insert(1, new SelectListItem() { Text = "Yes", Value = "Y" });
                execModel.AccpetList.Insert(2, new SelectListItem() { Text = "No", Value = "N" });
                //
                execModel.EncryptedRoadCode = urlparameter;
                return View("PIUATR", execModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Execution().PIUATR()");
                return Json(new { success = false, message = "Request can not be processed at this time." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddATRByPIU(RSAInspectionDetails model)
        {
            try
            {
                message = "Error occured while processing your request.";

                Dictionary<string, string> decryptedParameters = null;
                String[] encryptedParameters = null;
                string urlParams = model.EncryptedRSAId;
                encryptedParameters = urlParams.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                int RSAId = Convert.ToInt32(decryptedParameters["RSAId"]);
                model.RSAId = RSAId;
                ModelState.Remove("StartChainage");
                ModelState.Remove("EndChainage");
                ModelState.Remove("Safety_Issue");
                ModelState.Remove("RSA_Recommendation");
                ModelState.Remove("GradeCode");
                ModelState.Remove("auditDate");
                ModelState.Remove("stageCode");

                ModelState.Remove("IssueCode"); // Added on 03 March 2020



                ModelState.Remove("LikelihoodCode"); // Inspection Details Validation

                if (ModelState.IsValid)
                {
                    ExecutionBAL execBal = new ExecutionBAL();
                    Boolean Status = execBal.AddATRByPIUBAL(model, ref message);
                    return Json(new { success = Status, message = message });
                }
                else
                {
                    return Json(new { success = false, message = message });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AddATRByPIU()");
                return Json(new { success = false, message = message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FinalizeByPIU(string urlparameter)
        {
            IExecutionBAL objExecutionBAL = new ExecutionBAL();

            String[] encryptedParameters = null;
            Dictionary<string, string> decryptedParameters = null;
            int proposalCode = 0;

            try
            {
                encryptedParameters = urlparameter.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                proposalCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);

                string result = objExecutionBAL.FinalizeDetailsByPIU(proposalCode); //FinalizeRSAATRBAL
                if (result.Equals(string.Empty))
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = result }, JsonRequestBehavior.DenyGet);
                }

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExecutionController.FinalizeByPIU()");
                return null;
            }

        }

        #endregion

        #region SQC Methods
        [HttpGet]
        public ActionResult SQCATR(string urlparameter)
        {
            String[] encryptedParameters = null;
            Dictionary<string, string> decryptedParameters = null;
            int proposalCode = 0;
            RSAInspectionDetails execModel = new RSAInspectionDetails();
            encryptedParameters = urlparameter.Split('/');
            decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
            proposalCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);


            dbContext = new PMGSYEntities();
            try
            {
                #region
                ExecutionDAL objDAL = new ExecutionDAL();

                execModel = GetFinancialDetails(proposalCode, "P");
                execModel.Operation = "A";
                execModel.IMS_PR_ROAD_CODE = proposalCode;
                execModel.EXEC_PROGRESS_TYPE = "P";
                execModel.IMS_PR_ROAD_CODE = proposalCode;
                #endregion

                int recordCount = 0;
                recordCount = dbContext.EXEC_RSA_INSPECTION.Where(m => m.IMS_PR_ROAD_CODE == proposalCode).Count();

                if (recordCount >= 1)
                {
                    string RsaCode = dbContext.EXEC_RSA_INSPECTION.Where(m => m.IMS_PR_ROAD_CODE == proposalCode).Select(m => m.EXEC_RSA_CODE).FirstOrDefault().ToString();
                    execModel.EXEC_RSA_CODE = Convert.ToInt32(RsaCode);
                    execModel.InspectionDate = dbContext.EXEC_RSA_INSPECTION.Where(m => m.IMS_PR_ROAD_CODE == proposalCode).Select(m => m.EXEC_RSA_INSP_DATE).FirstOrDefault().ToString().Substring(0, 10); ;
                    string Status = dbContext.EXEC_RSA_INSPECTION.Where(m => m.IMS_PR_ROAD_CODE == proposalCode).Select(m => m.IMS_ROAD_STATUS).FirstOrDefault().ToString();
                    execModel.IsFinalized = dbContext.EXEC_RSA_INSPECTION_ATR.Where(m => m.EXEC_RSA_CODE == execModel.EXEC_RSA_CODE).Select(m => m.IS_FINALIZED).FirstOrDefault().ToString();

                    if (Status.Equals("P"))
                    {
                        execModel.RoadStatus = "Progress";
                    }
                    else if (Status.Equals("A"))
                    {
                        execModel.RoadStatus = "Land Acquisition";
                    }
                    else if (Status.Equals("L"))
                    {
                        execModel.RoadStatus = "Legal";
                    }
                    else if (Status.Equals("F"))
                    {
                        execModel.RoadStatus = "Forest";
                    }
                    else if (Status.Equals("X"))
                    {
                        execModel.RoadStatus = "Maintenance";
                    }
                    else if (Status.Equals("C"))
                    {
                        execModel.RoadStatus = "Completed";
                    }
                    else if (Status.Equals("D"))
                    {
                        execModel.RoadStatus = "Design Stage";
                    }
                    else if (Status.Equals("N"))
                    {
                        execModel.RoadStatus = "Construction";
                    }
                    else if (Status.Equals("G"))
                    {
                        execModel.RoadStatus = "Pre opening";
                    }
                    else if (Status.Equals("O"))
                    {
                        execModel.RoadStatus = "Operation";
                    }
                    else
                    {
                        execModel.RoadStatus = "NA";
                    }

                }
                else
                {
                    // execModel.Operation = "Create";
                    execModel.RoadStatus = "NA";
                    execModel.InspectionDate = "NA";
                }

                execModel.GradeList = new List<SelectListItem>();
                execModel.GradeList.Insert(0, new SelectListItem() { Text = "Select Grade", Value = "-1", Selected = true });
                execModel.GradeList.Insert(1, new SelectListItem() { Text = "Satisfactory", Value = "S" });
                execModel.GradeList.Insert(2, new SelectListItem() { Text = "Required Improvement ", Value = "R" });
                execModel.GradeList.Insert(2, new SelectListItem() { Text = "Unsatisfactory", Value = "U" });
                execModel.GradeCode = "-1";

                // Inspection Master Fields

                execModel.stageList = new List<SelectListItem>();
                execModel.stageList.Insert(0, new SelectListItem() { Text = "Select RSA Stage", Value = "-1", Selected = true });


                execModel.stageList.Insert(1, new SelectListItem() { Text = "Design Stage", Value = "D" });
                execModel.stageList.Insert(2, new SelectListItem() { Text = "Construction", Value = "N" });
                execModel.stageList.Insert(3, new SelectListItem() { Text = "Pre opening", Value = "G" });
                execModel.stageList.Insert(4, new SelectListItem() { Text = "Operation ", Value = "O" });

                //execModel.stageList.Insert(1, new SelectListItem() { Text = "Progress", Value = "P" });
                //execModel.stageList.Insert(2, new SelectListItem() { Text = "Land Acquisition", Value = "A" });
                //execModel.stageList.Insert(3, new SelectListItem() { Text = "Legal", Value = "L" });
                //execModel.stageList.Insert(4, new SelectListItem() { Text = "Forest ", Value = "F" });
                //execModel.stageList.Insert(5, new SelectListItem() { Text = "Maintenance", Value = "X" });
                //execModel.stageList.Insert(6, new SelectListItem() { Text = "Completed", Value = "C" });


                ExecutionRoadStatusViewModel execRoadModel = objDAL.GetRoadDetails(proposalCode);
                if (execRoadModel != null)
                {
                    execModel.BlockName = execRoadModel.BlockName;
                    execModel.Package = execRoadModel.Package;
                    execModel.RoadName = execRoadModel.RoadName;
                    execModel.AgreementDate = execRoadModel.AgreementDate;
                    execModel.Sanction_Cost = execRoadModel.Sanction_Cost;
                    execModel.Sanction_length = execRoadModel.Sanction_length;
                    execModel.AgreementCost = execRoadModel.AgreementCost;
                    execModel.SanctionYear = execRoadModel.SanctionYear;
                    execModel.changedLength = execRoadModel.changedLength;
                }


                // PIU
                execModel.AccpetList = new List<SelectListItem>();
                execModel.AccpetList.Insert(0, new SelectListItem() { Text = "Select ", Value = "-1", Selected = true });
                execModel.AccpetList.Insert(1, new SelectListItem() { Text = "Yes", Value = "Y" });
                execModel.AccpetList.Insert(2, new SelectListItem() { Text = "No", Value = "N" });
                execModel.EncryptedRoadCode = urlparameter;

                return View("SQCATR", execModel);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExecutionController.SQCATR()");
                return Json(new { success = false, message = "Request can not be processed at this time." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddATRBySQC(RSAInspectionDetails model)
        {
            try
            {
                Dictionary<string, string> decryptedParameters = null;
                String[] encryptedParameters = null;
                string urlParams = model.EncryptedATRId;
                encryptedParameters = urlParams.Split('/');

                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                int ATRId = Convert.ToInt32(decryptedParameters["ATRId"]);
                model.ATRId = ATRId;

                message = "Invalid Request.";
                ModelState.Remove("StartChainage");
                ModelState.Remove("EndChainage");
                ModelState.Remove("Safety_Issue");
                ModelState.Remove("RSA_Recommendation");
                ModelState.Remove("GradeCode");
                ModelState.Remove("auditDate");
                ModelState.Remove("stageCode");
                ModelState.Remove("ATR_By_PIU");
                ModelState.Remove("LikelihoodCode"); // Inspection Details Validation
                ModelState.Remove("IssueCode"); // Added on 04 March 2020


                if (ModelState.IsValid)
                {
                    ExecutionBAL execBal = new ExecutionBAL();
                    Boolean Status = execBal.AddATRBySQCBAL(model, ref message);
                    return Json(new { success = Status, message = message });
                }
                else
                {
                    return Json(new { success = false, message = message });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Execution.AddATRBySQC()");
                return Json(new { success = false, message = message });
            }
        }

        #endregion

        #region List
        [HttpPost]
        public ActionResult GetInspectionDetailsList(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                IExecutionBAL objBAL = new ExecutionBAL();
                long totalRecords = 0;
                int RSACode = 0;

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!(string.IsNullOrEmpty(Request.Params["RSACode"])))
                {
                    RSACode = Convert.ToInt32(Request.Params["RSACode"]);
                }
                var jsonData = new
                {
                    rows = objBAL.GetInspectionDetailsBALList(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, RSACode),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };
                return Json(jsonData, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Execution.GetInspectionDetailsList()");
                return null;
            }
        }
        #endregion List

        #region Image Upload ( Auditor )

        [HttpGet]
        public ActionResult ImageUploadByAuditor(String parameter, String hash, String key)
        {
            try
            {
                dbContext = new PMGSYEntities();
                PMGSY.Models.Execution.FileUploadViewModelATR fileUploadViewModel = new PMGSY.Models.Execution.FileUploadViewModelATR();

                Dictionary<string, string> decryptedParameters = null;
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                int RSAId = Convert.ToInt32(decryptedParameters["RSAId"].ToString());

                int RSACode = dbContext.EXEC_RSA_INSPECTION_DETAILS.Where(m => m.EXEC_RSA_ID == RSAId).Select(m => m.EXEC_RSA_CODE).FirstOrDefault();
                string FinalizedByAuditor = dbContext.EXEC_RSA_INSPECTION.Where(m => m.EXEC_RSA_CODE == RSACode).Select(m => m.EXEC_RSA_AUDIT_SUB).FirstOrDefault().ToString();
                fileUploadViewModel.FinalizedByAuditor = FinalizedByAuditor;

                Int32 PRRoadCode = Convert.ToInt32(RSAId);

                fileUploadViewModel.IMS_PR_ROAD_CODE = PRRoadCode;

                if (dbContext.EXEC_RSA_INSPECTION_DETAILS.Where(a => a.EXEC_RSA_ID == PRRoadCode).Any())
                {
                    string FileName = dbContext.EXEC_RSA_INSPECTION_DETAILS.Where(a => a.EXEC_RSA_ID == PRRoadCode).Select(m => m.EXEC_RSA_FILE_NAME).FirstOrDefault().ToString();

                    if (FileName.Equals("NA"))
                    {
                        fileUploadViewModel.NumberofFiles = 0;
                    }
                    else
                    {
                        fileUploadViewModel.NumberofFiles = 1;
                    }
                }
                else
                {
                    fileUploadViewModel.NumberofFiles = 0;
                }
                return View("ImageUploadByAuditor", fileUploadViewModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Execution.ImageUploadByAuditor()");
                return null;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        [HttpPost]
        public JsonResult ListFilesByAuditor(FormCollection formCollection)
        {
            IExecutionBAL objExecutionBAL = new ExecutionBAL();
            try
            {

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                Int32 IMS_PR_ROAD_CODE = Convert.ToInt32(Request.Params["IMS_PR_ROAD_CODE"]);

                int totalRecords;
                var jsonData = new
                {
                    rows = objExecutionBAL.GetFilesListBALByAuditor(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, IMS_PR_ROAD_CODE),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Execution.ListFilesByAuditor()");
                return null;
            }

        }

        [HttpPost]
        public ActionResult UploadsByAuditor(PMGSY.Models.Execution.FileUploadViewModelATR fileUploadViewModel)
        {
            try
            {
                ModelState.Remove("PdfDescription");
                CommonFunctions objCommonFunc = new CommonFunctions();

                String[] fileTypes = new String[] { "jpeg", String.Empty, String.Empty };
                String StorageRoot = string.Empty;
                StorageRoot = System.Configuration.ConfigurationManager.AppSettings["RSA_INSPECTION_DETAILS_FILE_UPLOAD"];

                if (!(objCommonFunc.IsValidImageFile(StorageRoot, Request, fileTypes)))
                {
                    fileUploadViewModel.ErrorMessage = "File Type is Not Allowed.";
                    return View("ImageUploadByAuditor", fileUploadViewModel.ErrorMessage);
                }


                foreach (string file in Request.Files)
                {
                    string status = ValidateImageFileofProgress(Request.Files[0].ContentLength, System.IO.Path.GetExtension(Request.Files[0].FileName));
                    if (status != string.Empty)
                    {
                        fileUploadViewModel.ErrorMessage = status;
                        return View("ImageUpload", fileUploadViewModel.ErrorMessage);
                    }
                }

                var fileData = new List<PMGSY.Models.Execution.FileUploadViewModelATR>();

                //
                foreach (string file in Request.Files)
                {
                    UploadImageFile(Request, fileData, fileUploadViewModel.IMS_PR_ROAD_CODE);
                }
                var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;

                var result = new ContentResult
                {
                    Content = "{\"files\":" + serializer.Serialize(fileData) + "}",
                };
                return result;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Execution.UploadsByAuditor()");
                return null;
            }
        }

        public string ValidateImageFileofProgress(int FileSize, string FileExtension)
        {
            string ValidExtensions = System.Configuration.ConfigurationManager.AppSettings["RSA_INSPECTION_DETAILS_VALID_FORMAT"];
            string[] arrValidFormats = ValidExtensions.Split('$');


            if (!arrValidFormats.Contains(FileExtension.ToLower()))
            {
                return "File is not Valid Image File";
            }
            if (FileSize > Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["RSA_INSPECTION_DETAILS_FILE_MAX_SIZE"]))
            {
                return "File Size Exceed the Maximum File Limit";
            }

            return string.Empty;
        }

        [HttpPost]
        public void UploadImageFile(HttpRequestBase request, List<PMGSY.Models.Execution.FileUploadViewModelATR> statuses, int IMS_PR_ROAD_CODE)
        {
            dbContext = new PMGSYEntities();
            IExecutionBAL objExecutionBAL = new ExecutionBAL();
            String StorageRoot = string.Empty;
            ModelState.Remove("PdfDescription");
            try
            {
                for (int i = 0; i < request.Files.Count; i++)
                {
                    HttpPostedFileBase file = request.Files[i];
                    // var fileName = IMS_PR_ROAD_CODE + System.IO.Path.GetExtension(request.Files[i].FileName).ToString();


                    var timestampArray = System.DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff").Replace('.', '_').Split(' ');
                    string timestamp = timestampArray[0].ToString() + "_" + timestampArray[1].ToString();
                    var fileName = "IMG_" + timestamp.Replace('/', '_').Replace(':', '_') + "_" + IMS_PR_ROAD_CODE.ToString() + System.IO.Path.GetExtension(request.Files[i].FileName).ToString();

                    StorageRoot = System.Configuration.ConfigurationManager.AppSettings["RSA_INSPECTION_DETAILS_FILE_UPLOAD"];
                    var fullPath = System.IO.Path.Combine(StorageRoot, fileName);

                    var ThumbnailPath = System.IO.Path.Combine(StorageRoot, "thumbnails");
                    var FullThumbnailPath = System.IO.Path.Combine(ThumbnailPath, fileName);

                    if (!(System.IO.Directory.Exists(ThumbnailPath)))
                    {
                        System.IO.Directory.CreateDirectory(ThumbnailPath);
                    }

                    statuses.Add(new PMGSY.Models.Execution.FileUploadViewModelATR()
                    {
                        url = fullPath,
                        thumbnail_url = fullPath,
                        name = fileName,
                        type = file.ContentType,
                        size = file.ContentLength,
                        delete_url = "",
                        delete_type = "DELETE",
                        Image_Description = request.Params["remark[]"],  //.Split(',')[i]
                        IMS_PR_ROAD_CODE = IMS_PR_ROAD_CODE

                    });
                    string status = objExecutionBAL.AddFileUploadDetailsBALByAuditorBAL(IMS_PR_ROAD_CODE, fileName, request.Params["remark[]"]);

                    if (status == string.Empty)
                    {
                        new PMGSY.BAL.Proposal.ProposalBAL().CompressImage(request.Files[0], fullPath, FullThumbnailPath);
                    }
                    else
                    {
                        // show an error over here
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Execution.UploadImageFile()");
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        [HttpGet]
        public ActionResult DownloadFileUploadedByAuditor(String parameter, String hash, String key)
        {
            string FileName = string.Empty;
            string FullFileLogicalPathName = string.Empty;
            string FullfilePhysicalPathName = string.Empty;
            string FullFileLogicalPathId = string.Empty;
            string FullfilePhysicalPathId = string.Empty;
            string FileExtension = string.Empty;
            Int32 ObsOrAtrId = 0;
            dbContext = new PMGSYEntities();
            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        FileName = (urlSplitParams[0]);
                        ObsOrAtrId = Convert.ToInt32(urlSplitParams[1]);
                    }
                }
                FileExtension = System.IO.Path.GetExtension(FileName).ToLower();

                if (FileExtension == ".jpg" || FileExtension == ".jpeg" || FileExtension == ".bmp" || FileExtension == ".png" || FileExtension == ".gif" || FileExtension == ".tiff")
                {
                    FullFileLogicalPathName = System.IO.Path.Combine(System.Configuration.ConfigurationManager.AppSettings["RSA_INSPECTION_DETAILS_FILE_UPLOAD_VIRTUAL_DIR_PATH"], FileName);
                    FullfilePhysicalPathName = System.IO.Path.Combine(System.Configuration.ConfigurationManager.AppSettings["RSA_INSPECTION_DETAILS_FILE_UPLOAD"], FileName);

                }

                string name = System.IO.Path.GetFileName(FileName);
                string ext = System.IO.Path.GetExtension(FileName);

                string type = string.Empty;

                if (ext != null)
                {
                    switch (ext.ToLower())
                    {
                        case ".pdf":
                            type = "Application/pdf";
                            break;
                        case ".doc":
                        case ".docx":
                            type = "Application/msword";
                            break;
                        case ".jpg":
                        case ".bmp":
                        case ".tiff":
                        case ".png":
                        case ".gif":
                        case ".jpeg":
                            type = "image/png";
                            break;
                        default:
                            type = "Application";
                            break;
                    }
                }

                if (System.IO.File.Exists(FullfilePhysicalPathName))
                {
                    return File(FullfilePhysicalPathName, type, DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + FileExtension);
                }
                else if (System.IO.File.Exists(FullfilePhysicalPathId))
                {
                    return File(FullfilePhysicalPathId, type, DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + FileExtension);
                }
                else
                {
                    return Json(new { Error = "File Not Exist" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Execution.DownloadFileUploadedByAuditor()");
                return Json(new { Success = "false" }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        [HttpPost]
        public JsonResult DeleteFileDetailsByAuditor(String parameter, String hash, String key)
        {
            String PhysicalPath = string.Empty;
            String ThumbnailPath = string.Empty;
            dbContext = new PMGSYEntities();
            IExecutionBAL objExecutionBAL = new ExecutionBAL();

            try
            {
                String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });

                String[] urlSplitParams = urlParams[0].Split('$');
                string QM_FILE_NAME = (urlSplitParams[0]);
                int RSAId = Convert.ToInt32(urlSplitParams[1]);

                PhysicalPath = System.Configuration.ConfigurationManager.AppSettings["RSA_INSPECTION_DETAILS_FILE_UPLOAD"];
                ThumbnailPath = System.IO.Path.Combine(System.IO.Path.Combine(PhysicalPath, "thumbnails"), QM_FILE_NAME);
                PhysicalPath = System.IO.Path.Combine(PhysicalPath, QM_FILE_NAME);
                if (!System.IO.File.Exists(PhysicalPath) && !System.IO.File.Exists(ThumbnailPath))
                {
                    return Json(new { Success = false, ErrorMessage = "File Not Found." });
                }
                string status = objExecutionBAL.DeleteFileDetailsByAuditor(RSAId);
                if (status == string.Empty)
                {
                    try
                    {
                        System.IO.File.Delete(PhysicalPath);
                        System.IO.File.Delete(ThumbnailPath);
                    }
                    catch (Exception ex)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                        return Json(new { Success = true, ErrorMessage = ex.Message });
                    }
                    return Json(new { Success = true, ErrorMessage = "Error Ocurred While Processing Your Request." });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = status });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Execution.DeleteFileDetailsByAuditor()");
                return Json(new { Success = false, ErrorMessage = "Error Ocurred While Processing Your Request." });
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        #endregion

        #region PDF ( Auditor )

        //1
        [HttpGet]
        public ActionResult PDFUploadByAuditorLayout(String parameter, String hash, String key)
        {
            dbContext = new PMGSYEntities();
            PMGSY.Models.Execution.FileUploadViewModelATR fileUploadViewModel = new PMGSY.Models.Execution.FileUploadViewModelATR();

            Dictionary<string, string> decryptedParameters = null;
            var en = parameter + "/" + hash + "/" + key;
            decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
            int ProposalCode = Convert.ToInt32(decryptedParameters["ProposalCode"].ToString());


            try
            {
                Int32 PRRoadCode = Convert.ToInt32(ProposalCode);
                string ATRId = dbContext.EXEC_RSA_INSPECTION.Where(m => m.IMS_PR_ROAD_CODE == PRRoadCode).Select(m => m.EXEC_RSA_CODE).FirstOrDefault().ToString();
                fileUploadViewModel.ATRId = ATRId;

                fileUploadViewModel.IMS_PR_ROAD_CODE = PRRoadCode;
                fileUploadViewModel.DocumentName = dbContext.EXEC_RSA_INSPECTION.Where(m => m.IMS_PR_ROAD_CODE == PRRoadCode).Select(m => m.EXEC_FILE_NAME).FirstOrDefault().ToString();

                int? RSA_CODE = dbContext.EXEC_RSA_INSPECTION.Where(m => m.IMS_PR_ROAD_CODE == PRRoadCode).Select(m => m.EXEC_RSA_CODE).FirstOrDefault();

                fileUploadViewModel.IsFinalized = dbContext.EXEC_RSA_INSPECTION.Where(m => m.EXEC_RSA_CODE == RSA_CODE).Select(m => m.EXEC_RSA_AUDIT_SUB).FirstOrDefault();

                if (dbContext.EXEC_RSA_INSPECTION.Where(a => a.IMS_PR_ROAD_CODE == PRRoadCode).Any())
                {
                    string FileName = dbContext.EXEC_RSA_INSPECTION.Where(a => a.IMS_PR_ROAD_CODE == PRRoadCode).Select(m => m.EXEC_FILE_NAME).FirstOrDefault().ToString();

                    if (FileName.Equals("NA"))
                    {
                        fileUploadViewModel.NumberofFiles = 0;
                    }
                    else
                    {
                        fileUploadViewModel.NumberofFiles = 1;
                    }
                }
                else
                {
                    fileUploadViewModel.NumberofFiles = 0;
                }


                return View("PDFUploadByAuditorLayout", fileUploadViewModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExecutionController.PDFUploadByAuditorLayout()");
                return null;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        //3
        [HttpPost]
        public JsonResult ListPDFByAuditor(FormCollection formCollection)
        {
            dbContext = new PMGSYEntities();
            IExecutionBAL objExecutionBAL = new ExecutionBAL();
            try
            {

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                Int32 IMS_PR_ROAD_CODE = Convert.ToInt32(Request.Params["IMS_PR_ROAD_CODE"]);

                int totalRecords;
                var jsonData = new
                {
                    rows = objExecutionBAL.GetPDFListBALByAuditor(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, IMS_PR_ROAD_CODE),

                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExecutionController.ListPDFByAuditor()");
                return Json(string.Empty);
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        //4
        [HttpPost]
        public ActionResult PDFUploadByAuditor(PMGSY.Models.Execution.FileUploadViewModelATR fileUploadViewModel)
        {
            try
            {
                CommonFunctions objCommonFunc = new CommonFunctions();
                String[] fileTypes = new String[] { "pdf" };
                String StorageRoot = string.Empty;
                StorageRoot = System.Configuration.ConfigurationManager.AppSettings["RSA_INSPECTION_MASTER_PDF_UPLOAD_BY_AUDITOR"];
                if (!(objCommonFunc.IsValidImageFileForContractor(ConfigurationManager.AppSettings["RSA_INSPECTION_MASTER_PDF_UPLOAD_BY_AUDITOR"], Request, fileTypes)))
                {
                    fileUploadViewModel.ErrorMessage = "File Type is Not Allowed.";
                    return View("PDFUploadByAuditorLayout", fileUploadViewModel.ErrorMessage);
                }

                var fileData = new List<PMGSY.Models.Execution.FileUploadViewModelATR>();
                foreach (string file in Request.Files)
                {
                    UploadPDFFileAuditor(Request, fileData, fileUploadViewModel.IMS_PR_ROAD_CODE, fileUploadViewModel.ErrorMessage);
                }
                if (!String.IsNullOrEmpty(fileUploadViewModel.ErrorMessage))
                {
                    return Json(new { Success = false, ErrorMessage = "File Not Found." });
                }
                else
                { // File Uploaded Successfully.
                    var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                    serializer.MaxJsonLength = Int32.MaxValue;

                    var result = new ContentResult
                    {
                        Content = "{\"files\":" + serializer.Serialize(fileData) + "}",
                    };
                    return result;

                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExecutionController.PDFUploadByAuditor()");
                return null;
            }
        }
        //5
        [HttpPost]
        public void UploadPDFFileAuditor(HttpRequestBase request, List<PMGSY.Models.Execution.FileUploadViewModelATR> statuses, int IMS_PR_ROAD_CODE, string ErrorMessage)
        {
            dbContext = new PMGSYEntities();
            IExecutionBAL objExecutionBAL = new ExecutionBAL();
            String StorageRoot = string.Empty;

            try
            {
                for (int i = 0; i < request.Files.Count; i++)
                {
                    HttpPostedFileBase file = request.Files[i];

                    var timestampArray = System.DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff").Replace('.', '_').Split(' ');
                    string timestamp = timestampArray[0].ToString() + "_" + timestampArray[1].ToString();
                    var fileName = "PDF_" + timestamp.Replace('/', '_').Replace(':', '_') + "_" + IMS_PR_ROAD_CODE.ToString() + System.IO.Path.GetExtension(request.Files[i].FileName).ToString();
                    StorageRoot = System.Configuration.ConfigurationManager.AppSettings["RSA_INSPECTION_MASTER_PDF_UPLOAD_BY_AUDITOR"];
                    var fullPath = System.IO.Path.Combine(StorageRoot, fileName);

                    var ThumbnailPath = System.IO.Path.Combine(StorageRoot, "thumbnails");
                    var FullThumbnailPath = System.IO.Path.Combine(ThumbnailPath, fileName);

                    if (!(System.IO.Directory.Exists(ThumbnailPath)))
                    {
                        System.IO.Directory.CreateDirectory(ThumbnailPath);
                    }

                    statuses.Add(new PMGSY.Models.Execution.FileUploadViewModelATR()
                    {
                        url = fullPath,
                        thumbnail_url = fullPath,
                        name = fileName,
                        type = file.ContentType,
                        size = file.ContentLength,
                        delete_url = "",
                        delete_type = "DELETE",
                        Image_Description = request.Params["remark[]"],  //.Split(',')[i]
                        IMS_PR_ROAD_CODE = IMS_PR_ROAD_CODE

                    });
                    string status = objExecutionBAL.AddPADFByAuditorBAL(IMS_PR_ROAD_CODE, fileName, request.Params["remark[]"]);

                    if (status == string.Empty)
                    {
                        if (fileName.Contains("pdf") || fileName.Contains("xls"))
                        {
                            HttpPostedFileBase postBasefile = request.Files[0];
                            postBasefile.SaveAs(Path.Combine(StorageRoot, fileName));
                            ErrorMessage = "Details Uploaded Successfully.";

                        }
                        else
                        {
                            CompressImage(request.Files[0], fullPath, FullThumbnailPath);
                        }
                    }
                    else
                    {
                        ErrorMessage = "Details are not Uploaded.";
                        // show an error over here
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExecutionController.UploadPDFFileAuditor()");
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        //6 Compress


        //7
        [HttpGet]
        public ActionResult DownloadPDFUploadedByAuditor(String parameter, String hash, String key)
        {
            string FileName = string.Empty;
            string FullFileLogicalPathName = string.Empty;
            string FullfilePhysicalPathName = string.Empty;
            string FullFileLogicalPathId = string.Empty;
            string FullfilePhysicalPathId = string.Empty;
            string FileExtension = string.Empty;
            Int32 ObsOrAtrId = 0;
            dbContext = new PMGSYEntities();
            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        FileName = (urlSplitParams[0]);
                        ObsOrAtrId = Convert.ToInt32(urlSplitParams[1]);
                    }
                }
                FileExtension = System.IO.Path.GetExtension(FileName).ToLower();

                if (FileExtension == ".jpg" || FileExtension == ".jpeg" || FileExtension == ".bmp" || FileExtension == ".png" || FileExtension == ".gif" || FileExtension == ".tiff")
                {
                    FullFileLogicalPathName = System.IO.Path.Combine(System.Configuration.ConfigurationManager.AppSettings["RSA_INSPECTION_MASTER_PDF_UPLOAD_VIRTUAL_DIR_PATH_BY_AUDITOR"], FileName);
                    FullfilePhysicalPathName = System.IO.Path.Combine(System.Configuration.ConfigurationManager.AppSettings["RSA_INSPECTION_MASTER_PDF_UPLOAD_BY_AUDITOR"], FileName);

                }

                string name = System.IO.Path.GetFileName(FileName);
                string ext = System.IO.Path.GetExtension(FileName);

                string type = string.Empty;

                if (ext != null)
                {
                    switch (ext.ToLower())
                    {
                        case ".pdf":
                            type = "Application/pdf";
                            break;
                        case ".doc":
                        case ".docx":
                            type = "Application/msword";
                            break;
                        case ".jpg":
                        case ".bmp":
                        case ".tiff":
                        case ".png":
                        case ".gif":
                        case ".jpeg":
                            type = "image/png";
                            break;
                        default:
                            type = "Application";
                            break;
                    }
                }

                if (System.IO.File.Exists(FullfilePhysicalPathName))
                {
                    return File(FullfilePhysicalPathName, type, DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + FileExtension);
                }
                else if (System.IO.File.Exists(FullfilePhysicalPathId))
                {
                    return File(FullfilePhysicalPathId, type, DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + FileExtension);
                }
                else
                {
                    return Json(new { Error = "File Not Exist" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {

                ErrorLog.LogError(ex, "ExcecutionController.DownloadPDFUploadedByAuditor()");
                return Json(new { Success = "false" }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        //8
        [HttpPost]
        public JsonResult DeletePDFDetailsByAuditor(String parameter, String hash, String key)
        {
            String PhysicalPath = string.Empty;
            String ThumbnailPath = string.Empty;
            dbContext = new PMGSYEntities();
            IExecutionBAL objExecutionBAL = new ExecutionBAL();

            try
            {
                String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });

                String[] urlSplitParams = urlParams[0].Split('$');
                string QM_FILE_NAME = (urlSplitParams[0]);
                int RSAId = Convert.ToInt32(urlSplitParams[1]);

                PhysicalPath = System.Configuration.ConfigurationManager.AppSettings["RSA_INSPECTION_MASTER_PDF_UPLOAD_BY_AUDITOR"];

                ThumbnailPath = System.IO.Path.Combine(System.IO.Path.Combine(PhysicalPath, "thumbnails"), QM_FILE_NAME);

                PhysicalPath = System.IO.Path.Combine(PhysicalPath, QM_FILE_NAME);

                if (!System.IO.File.Exists(PhysicalPath) && !System.IO.File.Exists(ThumbnailPath))
                {
                    return Json(new { Success = false, ErrorMessage = "File Not Found." });
                }

                string status = objExecutionBAL.DeletePDFDetailsByAuditor(RSAId);

                if (status == string.Empty)
                {
                    try
                    {
                        System.IO.File.Delete(PhysicalPath);
                        System.IO.File.Delete(ThumbnailPath);
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.LogError(ex, "ExecutionController.DeletePDFDetailsByAuditor()");
                        return Json(new { Success = true, ErrorMessage = ex.Message });
                    }
                    return Json(new { Success = true, ErrorMessage = "Error Ocurred While Processing Your Request." });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = status });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExecutionController.DeletePDFDetailsByAuditor()");
                return Json(new { Success = false, ErrorMessage = "Error Ocurred While Processing Your Request." });
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        #endregion

        #region PDF Upload ( PIU )

        //1
        [HttpGet]
        public ActionResult ImageUploadByPIU(String parameter, String hash, String key)
        {
            dbContext = new PMGSYEntities();
            PMGSY.Models.Execution.FileUploadViewModelATR fileUploadViewModel = new PMGSY.Models.Execution.FileUploadViewModelATR();

            Dictionary<string, string> decryptedParameters = null;
            decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
            int ATRId = Convert.ToInt32(decryptedParameters["ATRId"].ToString());


            try
            {
                Int32 PRRoadCode = Convert.ToInt32(ATRId);
                fileUploadViewModel.ATRId = Convert.ToString(ATRId);
                fileUploadViewModel.IMS_PR_ROAD_CODE = PRRoadCode;
                fileUploadViewModel.DocumentName = dbContext.EXEC_RSA_INSPECTION_ATR.Where(m => m.EXEC_RSA_ATR_ID == ATRId).Select(m => m.EXEC_RSA_ATR_FILE_NAME).FirstOrDefault().ToString();

                int? RSA_CODE = dbContext.EXEC_RSA_INSPECTION_ATR.Where(m => m.EXEC_RSA_ATR_ID == PRRoadCode).Select(m => m.EXEC_RSA_CODE).FirstOrDefault();

                fileUploadViewModel.IsFinalized = dbContext.EXEC_RSA_INSPECTION.Where(m => m.EXEC_RSA_CODE == RSA_CODE).Select(m => m.EXEC_RSA_PIU_SUB).FirstOrDefault();

                if (dbContext.EXEC_RSA_INSPECTION_ATR.Where(a => a.EXEC_RSA_ATR_ID == PRRoadCode).Any())
                {
                    string FileName = dbContext.EXEC_RSA_INSPECTION_ATR.Where(a => a.EXEC_RSA_ATR_ID == PRRoadCode).Select(m => m.EXEC_RSA_ATR_FILE_NAME).FirstOrDefault().ToString();

                    if (FileName.Equals("NA"))
                    {
                        fileUploadViewModel.NumberofFiles = 0;
                    }
                    else
                    {
                        fileUploadViewModel.NumberofFiles = 1;
                    }
                }
                else
                {
                    fileUploadViewModel.NumberofFiles = 0;
                }


                return View("ImageUploadByPIU", fileUploadViewModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Execution.ImageUploadByPIU()");
                return null;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }


        //2
        [HttpGet]
        public ActionResult GetImageUploadByPIU(string id)
        {
            dbContext = new PMGSYEntities();
            PMGSY.Models.Execution.FileUploadViewModelATR fileUploadViewModel = new PMGSY.Models.Execution.FileUploadViewModelATR();

            //Dictionary<string, string> decryptedParameters = null;
            //decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
            int ATRId = Convert.ToInt32(id);


            try
            {
                Int32 PRRoadCode = Convert.ToInt32(ATRId);
                fileUploadViewModel.ATRId = Convert.ToString(ATRId);
                fileUploadViewModel.IMS_PR_ROAD_CODE = PRRoadCode;
                fileUploadViewModel.DocumentName = dbContext.EXEC_RSA_INSPECTION_ATR.Where(m => m.EXEC_RSA_ATR_ID == ATRId).Select(m => m.EXEC_RSA_ATR_FILE_NAME).FirstOrDefault().ToString();

                int? RSA_CODE = dbContext.EXEC_RSA_INSPECTION_ATR.Where(m => m.EXEC_RSA_ATR_ID == PRRoadCode).Select(m => m.EXEC_RSA_CODE).FirstOrDefault();

                fileUploadViewModel.IsFinalized = dbContext.EXEC_RSA_INSPECTION.Where(m => m.EXEC_RSA_CODE == RSA_CODE).Select(m => m.EXEC_RSA_PIU_SUB).FirstOrDefault();

                if (dbContext.EXEC_RSA_INSPECTION_ATR.Where(a => a.EXEC_RSA_ATR_ID == PRRoadCode).Any())
                {
                    string FileName = dbContext.EXEC_RSA_INSPECTION_ATR.Where(a => a.EXEC_RSA_ATR_ID == PRRoadCode).Select(m => m.EXEC_RSA_ATR_FILE_NAME).FirstOrDefault().ToString();

                    if (FileName.Equals("NA"))
                    {
                        fileUploadViewModel.NumberofFiles = 0;
                    }
                    else
                    {
                        fileUploadViewModel.NumberofFiles = 1;
                    }
                }
                else
                {
                    fileUploadViewModel.NumberofFiles = 0;
                }


                return View("ImageUploadByPIU", fileUploadViewModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Execution.ImageUploadByPIU()");
                return null;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        //3
        [HttpPost]
        public JsonResult ListFilesByPIU(FormCollection formCollection)
        {
            dbContext = new PMGSYEntities();
            IExecutionBAL objExecutionBAL = new ExecutionBAL();
            try
            {

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                Int32 IMS_PR_ROAD_CODE = Convert.ToInt32(Request.Params["IMS_PR_ROAD_CODE"]);

                int totalRecords;
                var jsonData = new
                {
                    rows = objExecutionBAL.GetFilesListBALByPIU(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, IMS_PR_ROAD_CODE),

                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Execution.ListFilesByPIU()");
                return Json(string.Empty);
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        //4
        [HttpPost]
        public ActionResult UploadsByPIU(PMGSY.Models.Execution.FileUploadViewModelATR fileUploadViewModel)
        {
            try
            {
                CommonFunctions objCommonFunc = new CommonFunctions();
                String[] fileTypes = new String[] { "pdf" };
                String StorageRoot = string.Empty;
                StorageRoot = System.Configuration.ConfigurationManager.AppSettings["RSA_INSPECTION_DETAILS_FILE_UPLOAD_PIU"];
                if (!(objCommonFunc.IsValidImageFileForContractor(ConfigurationManager.AppSettings["RSA_INSPECTION_DETAILS_FILE_UPLOAD_PIU"], Request, fileTypes)))
                {
                    fileUploadViewModel.ErrorMessage = "File Type is Not Allowed.";
                    return View("ImageUploadByPIU", fileUploadViewModel.ErrorMessage);
                }

                var fileData = new List<PMGSY.Models.Execution.FileUploadViewModelATR>();
                foreach (string file in Request.Files)
                {
                    UploadImageFilePIU(Request, fileData, fileUploadViewModel.IMS_PR_ROAD_CODE);
                }
                var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;

                var result = new ContentResult
                {
                    Content = "{\"files\":" + serializer.Serialize(fileData) + "}",
                };
                return result;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Execution.UploadsByPIU()");
                return null;
            }
        }

        //5
        [HttpPost]
        public void UploadImageFilePIU(HttpRequestBase request, List<PMGSY.Models.Execution.FileUploadViewModelATR> statuses, int IMS_PR_ROAD_CODE)
        {
            dbContext = new PMGSYEntities();
            IExecutionBAL objExecutionBAL = new ExecutionBAL();
            String StorageRoot = string.Empty;

            try
            {
                for (int i = 0; i < request.Files.Count; i++)
                {
                    HttpPostedFileBase file = request.Files[i];

                    var timestampArray = System.DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff").Replace('.', '_').Split(' ');
                    string timestamp = timestampArray[0].ToString() + "_" + timestampArray[1].ToString();
                    var fileName = "PDF_" + timestamp.Replace('/', '_').Replace(':', '_') + "_" + IMS_PR_ROAD_CODE.ToString() + System.IO.Path.GetExtension(request.Files[i].FileName).ToString();
                    StorageRoot = System.Configuration.ConfigurationManager.AppSettings["RSA_INSPECTION_DETAILS_FILE_UPLOAD_PIU"];
                    var fullPath = System.IO.Path.Combine(StorageRoot, fileName);

                    var ThumbnailPath = System.IO.Path.Combine(StorageRoot, "thumbnails");
                    var FullThumbnailPath = System.IO.Path.Combine(ThumbnailPath, fileName);

                    if (!(System.IO.Directory.Exists(ThumbnailPath)))
                    {
                        System.IO.Directory.CreateDirectory(ThumbnailPath);
                    }

                    statuses.Add(new PMGSY.Models.Execution.FileUploadViewModelATR()
                    {
                        url = fullPath,
                        thumbnail_url = fullPath,
                        name = fileName,
                        type = file.ContentType,
                        size = file.ContentLength,
                        delete_url = "",
                        delete_type = "DELETE",
                        Image_Description = request.Params["remark[]"],  //.Split(',')[i]
                        IMS_PR_ROAD_CODE = IMS_PR_ROAD_CODE

                    });
                    string status = objExecutionBAL.AddFileUploadDetailsBALByPIUBAL(IMS_PR_ROAD_CODE, fileName, request.Params["remark[]"]);

                    if (status == string.Empty)
                    {
                        if (fileName.Contains("pdf") || fileName.Contains("xls"))
                        {
                            HttpPostedFileBase postBasefile = request.Files[0];
                            postBasefile.SaveAs(Path.Combine(StorageRoot, fileName));

                        }
                        else
                        {
                            CompressImage(request.Files[0], fullPath, FullThumbnailPath);
                        }
                    }
                    else
                    {
                        // show an error over here
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Execution.UploadImageFilePIU()");
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }


        //6
        public void CompressImage(HttpPostedFileBase httpPostedFileBase, string DestinitionPath, string ThumbnailPath)
        {
            try
            {
                // For Thumbnail Image    
                ImageResizer.ImageJob ThumbnailJob = new ImageResizer.ImageJob(httpPostedFileBase, ThumbnailPath,
                    new ImageResizer.ResizeSettings("width=100;height=100;format=jpg;mode=max"));

                ThumbnailJob.Build();

                // For Original Image
                ImageResizer.ImageJob job = new ImageResizer.ImageJob(httpPostedFileBase, DestinitionPath,
                    new ImageResizer.ResizeSettings("width=1024;height=768;format=jpg;mode=max"));

                job.Build();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Execution.CompressImage()");
            }
        }

        //7
        [HttpGet]
        public ActionResult DownloadFileUploadedByPIU(String parameter, String hash, String key)
        {
            string FileName = string.Empty;
            string FullFileLogicalPathName = string.Empty;
            string FullfilePhysicalPathName = string.Empty;
            string FullFileLogicalPathId = string.Empty;
            string FullfilePhysicalPathId = string.Empty;
            string FileExtension = string.Empty;
            Int32 ObsOrAtrId = 0;
            dbContext = new PMGSYEntities();
            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        FileName = (urlSplitParams[0]);
                        ObsOrAtrId = Convert.ToInt32(urlSplitParams[1]);
                    }
                }
                FileExtension = System.IO.Path.GetExtension(FileName).ToLower();

                if (FileExtension == ".jpg" || FileExtension == ".jpeg" || FileExtension == ".bmp" || FileExtension == ".png" || FileExtension == ".gif" || FileExtension == ".tiff")
                {
                    FullFileLogicalPathName = System.IO.Path.Combine(System.Configuration.ConfigurationManager.AppSettings["RSA_INSPECTION_DETAILS_FILE_UPLOAD_VIRTUAL_DIR_PATH_PIU"], FileName);
                    FullfilePhysicalPathName = System.IO.Path.Combine(System.Configuration.ConfigurationManager.AppSettings["RSA_INSPECTION_DETAILS_FILE_UPLOAD_PIU"], FileName);

                }

                string name = System.IO.Path.GetFileName(FileName);
                string ext = System.IO.Path.GetExtension(FileName);

                string type = string.Empty;

                if (ext != null)
                {
                    switch (ext.ToLower())
                    {
                        case ".pdf":
                            type = "Application/pdf";
                            break;
                        case ".doc":
                        case ".docx":
                            type = "Application/msword";
                            break;
                        case ".jpg":
                        case ".bmp":
                        case ".tiff":
                        case ".png":
                        case ".gif":
                        case ".jpeg":
                            type = "image/png";
                            break;
                        default:
                            type = "Application";
                            break;
                    }
                }

                if (System.IO.File.Exists(FullfilePhysicalPathName))
                {
                    return File(FullfilePhysicalPathName, type, DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + FileExtension);
                }
                else if (System.IO.File.Exists(FullfilePhysicalPathId))
                {
                    return File(FullfilePhysicalPathId, type, DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + FileExtension);
                }
                else
                {
                    return Json(new { Error = "File Not Exist" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {

                ErrorLog.LogError(ex, "Excecution.DownloadFileUploadedByPIU()");
                return Json(new { Success = "false" }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        //8
        [HttpPost]
        public JsonResult DeleteFileDetailsByPIU(String parameter, String hash, String key)
        {
            String PhysicalPath = string.Empty;
            String ThumbnailPath = string.Empty;
            dbContext = new PMGSYEntities();
            IExecutionBAL objExecutionBAL = new ExecutionBAL();

            try
            {
                String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });

                String[] urlSplitParams = urlParams[0].Split('$');
                string QM_FILE_NAME = (urlSplitParams[0]);
                int RSAId = Convert.ToInt32(urlSplitParams[1]);

                PhysicalPath = System.Configuration.ConfigurationManager.AppSettings["RSA_INSPECTION_DETAILS_FILE_UPLOAD_PIU"];

                ThumbnailPath = System.IO.Path.Combine(System.IO.Path.Combine(PhysicalPath, "thumbnails"), QM_FILE_NAME);

                PhysicalPath = System.IO.Path.Combine(PhysicalPath, QM_FILE_NAME);

                if (!System.IO.File.Exists(PhysicalPath) && !System.IO.File.Exists(ThumbnailPath))
                {
                    return Json(new { Success = false, ErrorMessage = "File Not Found." });
                }

                string status = objExecutionBAL.DeleteFileDetailsByPIU(RSAId);

                if (status == string.Empty)
                {
                    try
                    {
                        System.IO.File.Delete(PhysicalPath);
                        System.IO.File.Delete(ThumbnailPath);
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.LogError(ex, "Execution.DeleteFileDetailsByPIU()");
                        return Json(new { Success = true, ErrorMessage = ex.Message });
                    }
                    return Json(new { Success = true, ErrorMessage = "Error Ocurred While Processing Your Request." });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = status });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Execution.DeleteFileDetailsByPIU()");
                return Json(new { Success = false, ErrorMessage = "Error Ocurred While Processing Your Request." });
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        #endregion

        #region Finalize By SQC
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FinalizeBySQCDetails(String parameter, String hash, String key)
        {
            IExecutionBAL objExecutionBAL = new ExecutionBAL();
            Dictionary<string, string> decryptedParameters = null;

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                int ATRId = Convert.ToInt32(decryptedParameters["ATRId"].ToString());

                string result = objExecutionBAL.FinalizeRSAATRBAL(ATRId);
                if (result.Equals(string.Empty))
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = result }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExecutionController.FinalizeBySQCDetails()");
                return null;
            }

        }
        #endregion

        #region Common
        [HttpGet]
        public RSAInspectionDetails GetFinancialDetails(int proposalCode, string progressType)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                RSAInspectionDetails model = new RSAInspectionDetails();
                if (dbContext.EXEC_PROGRESS.Any(m => m.IMS_PR_ROAD_CODE == proposalCode))
                {
                    int? year = (dbContext.EXEC_PROGRESS.Where(m => m.IMS_PR_ROAD_CODE == proposalCode).OrderByDescending(y => y.EXEC_PROG_YEAR).Select(y => y.EXEC_PROG_YEAR).FirstOrDefault());
                    int? month = (dbContext.EXEC_PROGRESS.Where(m => m.IMS_PR_ROAD_CODE == proposalCode && m.EXEC_PROG_YEAR == year).OrderByDescending(y => y.EXEC_PROG_MONTH).Select(y => y.EXEC_PROG_MONTH).FirstOrDefault());
                    EXEC_PROGRESS masterRoad = (dbContext.EXEC_PROGRESS.Where(m => m.EXEC_PROG_YEAR == year && m.IMS_PR_ROAD_CODE == proposalCode).OrderByDescending(m => m.EXEC_PROG_MONTH).FirstOrDefault());

                    if (masterRoad != null)
                    {
                        model.PreviousMonth = masterRoad.EXEC_PROG_MONTH;
                        model.PreviousYear = masterRoad.EXEC_PROG_YEAR;
                        model.TotalValueofwork = (dbContext.EXEC_PROGRESS.Where(m => m.EXEC_PROG_MONTH == month && m.EXEC_PROG_YEAR == year && m.IMS_PR_ROAD_CODE == proposalCode).Select(m => m.EXEC_VALUEOFWORK_LASTMONTH).FirstOrDefault()) + (dbContext.EXEC_PROGRESS.Where(m => m.EXEC_PROG_MONTH == month && m.EXEC_PROG_YEAR == year && m.IMS_PR_ROAD_CODE == proposalCode).Select(m => m.EXEC_VALUEOFWORK_THISMONTH).FirstOrDefault());
                        model.TotalPayment = (dbContext.EXEC_PROGRESS.Where(m => m.EXEC_PROG_MONTH == month && m.EXEC_PROG_YEAR == year && m.IMS_PR_ROAD_CODE == proposalCode).Select(m => m.EXEC_PAYMENT_LASTMONTH).FirstOrDefault()) + (dbContext.EXEC_PROGRESS.Where(m => m.EXEC_PROG_MONTH == month && m.EXEC_PROG_YEAR == year && m.IMS_PR_ROAD_CODE == proposalCode).Select(m => m.EXEC_PAYMENT_THISMONTH).FirstOrDefault());
                        model.LastPaymentValue = (dbContext.EXEC_PROGRESS.Where(m => m.EXEC_PROG_MONTH == month && m.EXEC_PROG_YEAR == year && m.IMS_PR_ROAD_CODE == proposalCode).Select(m => m.EXEC_PAYMENT_LASTMONTH).FirstOrDefault());
                        model.LastMonthValue = (dbContext.EXEC_PROGRESS.Where(m => m.EXEC_PROG_MONTH == month && m.EXEC_PROG_YEAR == year && m.IMS_PR_ROAD_CODE == proposalCode).Select(m => m.EXEC_VALUEOFWORK_LASTMONTH).FirstOrDefault());
                        model.EXEC_PAYMENT_LASTMONTH = model.LastPaymentValue;
                        model.EXEC_VALUEOFWORK_LASTMONTH = model.LastMonthValue;
                        string status = (from item in dbContext.EXEC_PROGRESS
                                         where item.IMS_PR_ROAD_CODE == proposalCode &&
                                         item.EXEC_FINAL_PAYMENT_FLAG == "Y"
                                         select item.EXEC_FINAL_PAYMENT_FLAG).FirstOrDefault();

                        if (status == "Y")
                        {
                            model.CompleteStatus = "C";
                        }
                        else
                        {
                            model.CompleteStatus = "N";
                        }
                    }
                    else
                    {
                        model.TotalValueofwork = 0;
                        model.TotalPayment = 0;
                        model.LastPaymentValue = 0;
                        model.LastMonthValue = 0;
                        model.EXEC_PAYMENT_LASTMONTH = 0;
                        model.EXEC_VALUEOFWORK_LASTMONTH = 0;
                    }
                }
                else
                {
                    model.TotalValueofwork = 0;
                    model.TotalPayment = 0;
                    model.LastPaymentValue = 0;
                    model.LastMonthValue = 0;
                    model.EXEC_PAYMENT_LASTMONTH = 0;
                    model.EXEC_VALUEOFWORK_LASTMONTH = 0;
                }
                if (dbContext.EXEC_PROGRESS.Any(m => m.IMS_PR_ROAD_CODE == proposalCode && m.EXEC_FINAL_PAYMENT_FLAG == "Y"))
                {
                    model.IsFinalPaymentBefore = "Y";
                }
                int agreementId = dbContext.TEND_AGREEMENT_DETAIL.Where(m => m.IMS_PR_ROAD_CODE == proposalCode).OrderBy(m => m.TEND_AGREEMENT_ID).Select(m => m.TEND_AGREEMENT_CODE).FirstOrDefault();

                TEND_AGREEMENT_MASTER tendMaster = dbContext.TEND_AGREEMENT_MASTER.Find(agreementId);
                if (tendMaster != null)
                {
                    model.AgreementDate = new CommonFunctions().GetDateTimeToString(tendMaster.TEND_AGREEMENT_START_DATE);
                    model.AgreementMonth = tendMaster.TEND_DATE_OF_AGREEMENT.Month;
                    model.AgreementYear = tendMaster.TEND_DATE_OF_AGREEMENT.Year;
                    model.AgreementCost = tendMaster.TEND_AGREEMENT_AMOUNT;
                    decimal? valAgreement = null;
                    if (dbContext.TEND_AGREEMENT_DETAIL.Any(m => m.IMS_PR_ROAD_CODE == proposalCode && m.TEND_AGREEMENT_STATUS == "W"))
                    {
                        valAgreement = dbContext.TEND_AGREEMENT_DETAIL.Where(m => m.IMS_PR_ROAD_CODE == proposalCode && m.TEND_AGREEMENT_STATUS == "W").Sum(m => m.TEND_VALUE_WORK_DONE);
                    }

                    if (valAgreement == null)
                    {
                        valAgreement = 0;
                        model.AgreementCost = valAgreement + tendMaster.TEND_AGREEMENT_AMOUNT;
                    }
                    else
                    {
                        model.AgreementCost = valAgreement + tendMaster.TEND_AGREEMENT_AMOUNT;
                    }
                    model.AgreementCost = valAgreement + (dbContext.TEND_AGREEMENT_DETAIL.Where(m => m.IMS_PR_ROAD_CODE == proposalCode && m.TEND_AGREEMENT_STATUS != "W").Sum(m => (Decimal?)m.TEND_AGREEMENT_AMOUNT) == null ? 0 : dbContext.TEND_AGREEMENT_DETAIL.Where(m => m.IMS_PR_ROAD_CODE == proposalCode && m.TEND_AGREEMENT_STATUS != "W").Sum(m => (Decimal?)m.TEND_AGREEMENT_AMOUNT));

                }
                else {

                    model.AgreementDate = new CommonFunctions().GetDateTimeToString(System.DateTime.Now);
                    model.AgreementMonth = System.DateTime.Now.Month;
                    model.AgreementYear = System.DateTime.Now.Year;
                    model.AgreementCost = null;
                
                }


                IMS_SANCTIONED_PROJECTS master = dbContext.IMS_SANCTIONED_PROJECTS.Find(proposalCode);
                if (master != null)
                {
                    model.BlockName = (dbContext.MASTER_BLOCK.Where(m => m.MAST_BLOCK_CODE == master.MAST_BLOCK_CODE).Select(m => m.MAST_BLOCK_NAME).FirstOrDefault());
                    model.Package = master.IMS_PACKAGE_ID.ToString();
                    model.RoadName = master.IMS_ROAD_NAME;
                    model.Sanction_Cost = Convert.ToDouble(master.IMS_SANCTIONED_BS_AMT + master.IMS_SANCTIONED_BW_AMT + master.IMS_SANCTIONED_CD_AMT + master.IMS_SANCTIONED_OW_AMT + master.IMS_SANCTIONED_PAV_AMT + master.IMS_SANCTIONED_PW_AMT + master.IMS_SANCTIONED_RS_AMT);
                    model.Sanction_length = master.IMS_PAV_LENGTH;
                }

                //new change done for validating additional cost if done on proposal.
                if (dbContext.IMS_PROPOSAL_COST_ADD.Any(m => m.IMS_PR_ROAD_CODE == proposalCode))
                {
                    decimal? stateCost = dbContext.IMS_PROPOSAL_COST_ADD.Where(m => m.IMS_PR_ROAD_CODE == proposalCode).Sum(m => m.IMS_STATE_AMOUNT);
                    decimal? mordCost = dbContext.IMS_PROPOSAL_COST_ADD.Where(m => m.IMS_PR_ROAD_CODE == proposalCode).Sum(m => m.IMS_MORD_AMOUNT);

                    model.AdditionalCost = ((stateCost.HasValue ? stateCost.Value : 0) + (mordCost.HasValue ? mordCost.Value : 0));
                }

                return model;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExecutionController.GetFinancialDetails()");
                return null;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        [HttpGet]
        public ActionResult RSALayout(string urlparameter)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }
            int proposalCode = 0;

            String[] encryptedParameters = null;
            Dictionary<string, string> decryptedParameters = null;
            ExecutionDAL objDAL = new ExecutionDAL();
            PMGSY.DAL.Proposal.ProposalDAL objProp = new DAL.Proposal.ProposalDAL();
            try
            {
                if (urlparameter != string.Empty)
                {
                    encryptedParameters = urlparameter.Split('/');
                    if (encryptedParameters.Length == 3)
                    {
                        decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    }
                }

                proposalCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);

                RSAlayout roadModel = new RSAlayout();

                dbContext = new PMGSYEntities();
                roadModel.prRoadCode = proposalCode;

                int recordCount = 0;
                recordCount = dbContext.EXEC_RSA_INSPECTION.Where(m => m.IMS_PR_ROAD_CODE == proposalCode).Count();

                if (recordCount >= 1)
                {
                    roadModel.Operation = "Update";
                    roadModel.InspectionDate = dbContext.EXEC_RSA_INSPECTION.Where(m => m.IMS_PR_ROAD_CODE == proposalCode).Select(m => m.EXEC_RSA_INSP_DATE).FirstOrDefault().ToString().Substring(0, 10); ;

                    roadModel.auditDate = roadModel.InspectionDate.ToString();
                    roadModel.stageCode = dbContext.EXEC_RSA_INSPECTION.Where(m => m.IMS_PR_ROAD_CODE == proposalCode).Select(m => m.IMS_ROAD_STATUS).FirstOrDefault().ToString();

                    string Status = dbContext.EXEC_RSA_INSPECTION.Where(m => m.IMS_PR_ROAD_CODE == proposalCode).Select(m => m.IMS_ROAD_STATUS).FirstOrDefault().ToString();

                    if (Status.Equals("P"))
                    {
                        roadModel.RoadStatus = "Progress";
                    }
                    else if (Status.Equals("A"))
                    {
                        roadModel.RoadStatus = "Land Acquisition";
                    }
                    else if (Status.Equals("L"))
                    {
                        roadModel.RoadStatus = "Legal";
                    }
                    else if (Status.Equals("F"))
                    {
                        roadModel.RoadStatus = "Forest";
                    }
                    else if (Status.Equals("X"))
                    {
                        roadModel.RoadStatus = "Maintenance";
                    }
                    else if (Status.Equals("C"))
                    {
                        roadModel.RoadStatus = "Completed";
                    }
                    else if (Status.Equals("D"))
                    {
                        roadModel.RoadStatus = "Design Stage";
                    }
                    else if (Status.Equals("N"))
                    {
                        roadModel.RoadStatus = "Construction";
                    }
                    else if (Status.Equals("G"))
                    {
                        roadModel.RoadStatus = "Pre opening";
                    }
                    else if (Status.Equals("O"))
                    {
                        roadModel.RoadStatus = "Operation";
                    }
                    else
                    {
                        roadModel.RoadStatus = "NA";
                    }

                }
                else
                {
                    roadModel.Operation = "Create";
                    roadModel.RoadStatus = "NA";
                    roadModel.InspectionDate = "NA";
                }

                roadModel.stageList = new List<SelectListItem>();
                roadModel.stageList.Insert(0, new SelectListItem() { Text = "Select RSA Stage", Value = "-1", Selected = true });


                roadModel.stageList.Insert(1, new SelectListItem() { Text = "Design Stage", Value = "D" });
                roadModel.stageList.Insert(2, new SelectListItem() { Text = "Construction", Value = "N" });
                roadModel.stageList.Insert(3, new SelectListItem() { Text = "Pre opening", Value = "G" });
                roadModel.stageList.Insert(4, new SelectListItem() { Text = "Operation ", Value = "O" });

                //roadModel.stageList.Insert(1, new SelectListItem() { Text = "Progress", Value = "P" });
                //roadModel.stageList.Insert(2, new SelectListItem() { Text = "Land Acquisition", Value = "A" });
                //roadModel.stageList.Insert(3, new SelectListItem() { Text = "Legal", Value = "L" });
                //roadModel.stageList.Insert(4, new SelectListItem() { Text = "Forest ", Value = "F" });
                //roadModel.stageList.Insert(5, new SelectListItem() { Text = "Maintenance", Value = "X" });
                //roadModel.stageList.Insert(6, new SelectListItem() { Text = "Completed", Value = "C" });


                ExecutionRoadStatusViewModel execRoadModel = objDAL.GetRoadDetails(proposalCode);
                if (execRoadModel != null)
                {
                    roadModel.BlockName = execRoadModel.BlockName;
                    roadModel.Package = execRoadModel.Package;
                    roadModel.RoadName = execRoadModel.RoadName;
                    roadModel.AgreementDate = execRoadModel.AgreementDate;
                    roadModel.Sanction_Cost = execRoadModel.Sanction_Cost;
                    roadModel.Sanction_length = execRoadModel.Sanction_length;
                    roadModel.AgreementCost = execRoadModel.AgreementCost;
                    roadModel.SanctionYear = execRoadModel.SanctionYear;
                    roadModel.changedLength = execRoadModel.changedLength;
                }

                return View(roadModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExecutionController.RSALayout()");
                return null;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        #endregion



        [HttpGet]
        public JsonResult GetIssueSubDetailsForTextBox(int id)
        {
            try
            {
                PMGSYEntities dbContext = new PMGSYEntities();

                string IssueDesc = dbContext.RSA_MASTER_ISSUE.Where(m => m.ISSUE_CODE == id).Select(m => m.ISSUE_LONG_DESC).FirstOrDefault();
                string IssueReccomendation = dbContext.RSA_MASTER_ISSUE.Where(m => m.ISSUE_CODE == id).Select(m => m.ISSUE_RECOMMENDATION).FirstOrDefault();

                return Json(new { success = true, message = "Success", IssueDescDetails = IssueDesc, IssueReccomendationDetails = IssueReccomendation }, JsonRequestBehavior.AllowGet);

                //   return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExecutionController().GetIssueSubDetailsForTextBox()");
                return Json(new { success = false, message = "Error" });
            }
        }

        [Audit]
        public JsonResult GetDistrictByStateForRSA(int stateCode)
        {
            CommonFunctions objCommon = new CommonFunctions();
            try
            {
                return Json(new SelectList(objCommon.PopulateDistrict(stateCode, false), "Value", "Text"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }



        [HttpGet]
        public ActionResult GetDetailsEnteredByAuditor(String parameter, String hash, String key)
        {
            dbContext = new PMGSYEntities();
            PMGSY.Models.Execution.FileUploadViewModelATR fileUploadViewModel = new PMGSY.Models.Execution.FileUploadViewModelATR();

            Dictionary<string, string> decryptedParameters = null;
            decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
            int RSAId = Convert.ToInt32(decryptedParameters["RSAId"].ToString());


            try
            {


                EXEC_RSA_INSPECTION_DETAILS details = dbContext.EXEC_RSA_INSPECTION_DETAILS.Where(m => m.EXEC_RSA_ID == RSAId).FirstOrDefault();

                string Severity = details.EXEC_RSA_GRADE.Equals("S") ? "Medium" : (details.EXEC_RSA_GRADE.Equals("U") ? "Very High" : "High");

                return Json(new { success = true, startChainage = details.EXEC_RSA_START_CHAINAGE, endChainage = details.EXEC_RSA_END_CHAINAGE, Issue = details.EXEC_RSA_SAFETY_ISSUE, Recommondation = details.EXEC_RSA_RECOMMENDATION, SeverityDetails = Severity }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Execution.GetDetailsEnteredByAuditor()");
                return null;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }
        #endregion
    }

}
