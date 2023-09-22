using PMGSY.Areas.PFMSReports.Models;
using PMGSY.Common;
using PMGSY.Controllers;
using PMGSY.Extensions;
using PMGSY.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.PFMSReports.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class PFMSReportsController : Controller
    {
        //
        // GET: /PFMSReports/PFMSReports/

        #region Beneficiary Details
        [HttpGet]
        public ActionResult BeneficiaryDetailsLayout()
        {
            BeneficiaryDetailsViewModel model = new BeneficiaryDetailsViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                if (PMGSYSession.Current.StateCode == 0)
                {
                    model.StateList = comm.PopulateStates(false);
                    model.DistrictList = new List<SelectListItem>();
                    model.DistrictList.Insert(0, new SelectListItem { Text = "All Districts", Value = "0" });
                }
                if (PMGSYSession.Current.StateCode > 0)
                {
                    model.StateList = new List<SelectListItem>();
                    model.StateList.Insert(0, new SelectListItem { Text = PMGSYSession.Current.StateName, Value = PMGSYSession.Current.StateCode.ToString() });
                    model.DistrictList = comm.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                    model.DistrictList.Find(x => x.Value == "-1").Value = "0";

                    model.DistrictCode = (PMGSYSession.Current.DistrictCode > 0) ? PMGSYSession.Current.DistrictCode : 0;
                }
                model.lstAgency = comm.PopulateAgencies(PMGSYSession.Current.StateCode, true);

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PFMSReports.BeneficiaryDetailsLayout()");
                return Json(new { message = "Error occured while processing your request" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult BeneficiaryDetailsReport(BeneficiaryDetailsViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return View(model);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(new { message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PFMSReports.BeneficiaryDetailsReport()");
                return Json(new { message = "Error occured while processing your request" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult PopulateAgency(string id)
        {
            CommonFunctions comm = new CommonFunctions();
            try
            {
                int stateCode = Convert.ToInt32(id);
                return Json(comm.PopulateAgencies(stateCode, true), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PFMSReports.PopulateAgency()");
                return Json(new { message = "Error occured while processing your request" }, JsonRequestBehavior.AllowGet);
            }
        }


        #endregion

        #region Report
        [HttpGet]
        public ActionResult DscReportLayout()
        {
            DSCReportModel model = new DSCReportModel();
            CommonFunctions cf = new CommonFunctions();

            if (PMGSYSession.Current.StateCode == 0)
            {
                model.StateList = cf.PopulateStates(isPopulateFirstItem: false);
                model.DistrictList = new List<SelectListItem> { new SelectListItem { Selected = true, Text = "All District", Value = "0" } };
            }
            if (PMGSYSession.Current.StateCode > 0)
            {
                model.StateList = new List<SelectListItem>();
                model.StateList.Insert(0, new SelectListItem { Value = PMGSYSession.Current.StateCode.ToString().Trim(), Text = PMGSYSession.Current.StateName.Trim() });

                if (PMGSYSession.Current.DistrictCode == 0)
                {
                    model.DistrictList = cf.PopulateDistrict(StateCode: PMGSYSession.Current.StateCode, isAllSelected: true);
                    model.DistrictList.SingleOrDefault(s => s.Value == "-1").Value = "0";
                }
                else
                {
                    model.DistrictList = new List<SelectListItem>();
                    model.DistrictList.Insert(0, new SelectListItem { Value = PMGSYSession.Current.DistrictCode.ToString().Trim(), Text = PMGSYSession.Current.DistrictName.Trim() });
                }

            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DscReport(DSCReportModel model)
        {
            if (ModelState.IsValid)
            {
                return View(model);
            }
            return null;
        }

        [HttpPost]
        public JsonResult PopulateDistrictList(int StateCode)
        {
            try
            {
                CommonFunctions cf = new CommonFunctions();
                List<SelectListItem> districtList = cf.PopulateDistrict(StateCode: StateCode, isAllSelected: true);
                districtList.SingleOrDefault(s => s.Value == "-1").Value = "0";
                return Json(districtList);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PFMS1.PopulateDistrictList()");
                return Json(new { status = false, error = "error occured while processing your request" });
            }
        }
        #endregion

        #region PFMS Payments
        [HttpGet]
        public ActionResult PfmsPaymentLayout()
        {
            PfmsPaymentsViewModel model = new PfmsPaymentsViewModel();
            CommonFunctions comm = new CommonFunctions();
            TransactionParams objParam = new TransactionParams();
            try
            {
                model.MonthList = comm.PopulateMonths(DateTime.Now.Month);
                model.YearList = comm.PopulateYears(DateTime.Now.Year);

                model.lstSrrda = comm.PopulateNodalAgencies();
                if (PMGSYSession.Current.LevelId == 4)
                {
                    model.SRRDA_DPIU = "S";
                    model.SRRDA = PMGSYSession.Current.AdminNdCode;

                    //Populate DPIU
                    model.lstDpiu = new List<SelectListItem>();
                    model.lstDpiu.Insert(0, new SelectListItem { Text = "Select DPIU", Value = "-1" });
                    
                }
                else
                {
                    model.SRRDA_DPIU = "D";
                    model.SRRDA = PMGSYSession.Current.ParentNDCode.Value;
                    objParam.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                    objParam.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                    model.lstDpiu = comm.PopulateDPIU(objParam);
                    //model.lstDpiu.RemoveAt(0);
                    //model.lstDpiu.Insert(0, new SelectListItem { Text = "Select DPIU", Value = "-1" });
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PFMSReports.BeneficiaryDetailsLayout()");
                return Json(new { message = "Error occured while processing your request" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult PfmsPaymentReport(PfmsPaymentsViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return View(model);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(new { message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PFMSReports.BeneficiaryDetailsLayout()");
                return Json(new { message = "Error occured while processing your request" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        //added by abhinav pathak
        #region Pfms MIS Report DashBoard
        [HttpGet]
        public ActionResult PfmsMISDasboardLayout()
        {
            try
            {
                PfmsMISDashboardModel model = new PfmsMISDashboardModel();
                return View(model);
            }
            catch (Exception excep)
            {
                ErrorLog.LogError(excep, "PFMSReportsController/PfmsMISDasboardLayout");
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PfmsMisDashboardView(PfmsMISDashboardModel model)
        {
            try
            {
                PMGSY.Models.PMGSYEntities dbcontext = new PMGSY.Models.PMGSYEntities();
                DashBoardRecordModel DashBoardRecordModel = new Models.DashBoardRecordModel();
                var result = dbcontext.USP_ACC_PFMS_MIS_PAYMENTS_ABSTRACT("P", model.StartDate, model.EndDate).FirstOrDefault<PMGSY.Models.USP_ACC_PFMS_MIS_PAYMENTS_ABSTRACT_Result>();
                DashBoardRecordModel.ACK_BANK = result.ACK_BANK;
                DashBoardRecordModel.ACK_BANK_AMT = result.ACK_BANK_AMT;
                DashBoardRecordModel.ACK_PFMS = Convert.ToInt32(result.ACK_PFMS);
                DashBoardRecordModel.ACK_PFMS_AMT = Convert.ToDecimal(result.ACK_PFMS_AMT);
                DashBoardRecordModel.BANK_RESPONSE_PENDING = result.BANK_RESPONSE_PENDING;
                DashBoardRecordModel.BANK_RESPONSE_PENDING_AMT = result.BANK_RESPONSE_PENDING_AMT;
                DashBoardRecordModel.PAYMENT_MADE_BY_BANK = result.PAYMENT_MADE_BY_BANK;
                DashBoardRecordModel.PAYMENT_MADE_BY_BANK_AMT = result.PAYMENT_MADE_BY_BANK_AMT;
                DashBoardRecordModel.PFMS_RESPONSE_PENDING = Convert.ToInt32(result.PFMS_RESPONSE_PENDING);
                DashBoardRecordModel.PFMS_RESPONSE_PENDING_AMT = Convert.ToDecimal(result.PFMS_RESPONSE_PENDING_AMT);
                DashBoardRecordModel.REJECTED_BANK = result.REJECTED_BANK;
                DashBoardRecordModel.REJECTED_BANK_AMT = result.REJECTED_BANK_AMT;
                DashBoardRecordModel.REJECTED_PFMS = result.REJECTED_PFMS;
                DashBoardRecordModel.REJECTED_PFMS_AMT = result.REJECTED_PFMS_AMT;
                DashBoardRecordModel.Total_Payment = Convert.ToInt32(result.Total_Payment);
                DashBoardRecordModel.StartDate = model.StartDate;
                DashBoardRecordModel.EndDate = model.EndDate;
                return View(DashBoardRecordModel);
            }
            catch (Exception Ex)
            {
                ErrorLog.LogError(Ex, "PFMSReportsController/PfmsMISDasboardLayout");
                return null;
            }
        }
        #endregion

        #region REAT Beneficiary Report
        [HttpGet]
        public ActionResult ReatBeneficiaryDetailsLayout()
        {
            BeneficiaryDetailsViewModel model = new BeneficiaryDetailsViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                if (PMGSYSession.Current.StateCode == 0)
                {
                    model.StateList = comm.PopulateStates(false);
                    model.DistrictList = new List<SelectListItem>();
                    model.DistrictList.Insert(0, new SelectListItem { Text = "All Districts", Value = "0" });
                }
                if (PMGSYSession.Current.StateCode > 0)
                {
                    model.StateList = new List<SelectListItem>();
                    model.StateList.Insert(0, new SelectListItem { Text = PMGSYSession.Current.StateName, Value = PMGSYSession.Current.StateCode.ToString() });
                    model.DistrictList = comm.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                    model.DistrictList.Find(x => x.Value == "-1").Value = "0";

                    model.DistrictCode = (PMGSYSession.Current.DistrictCode > 0) ? PMGSYSession.Current.DistrictCode : 0;
                }
                model.lstAgency = comm.PopulateAgencies(PMGSYSession.Current.StateCode, true);

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PFMSReports.ReatBeneficiaryDetailsLayout()");
                return Json(new { message = "Error occured while processing your request" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult ReatBeneficiaryDetailsReport(BeneficiaryDetailsViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return View(model);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(new { message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PFMSReports.ReatBeneficiaryDetailsReport()");
                return Json(new { message = "Error occured while processing your request" }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion


        #region REAT DSC Status 
        [HttpGet]
        public ActionResult ReatDscStatusLayout()
        {
            ReatDscStatus model = new ReatDscStatus();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                if (PMGSYSession.Current.StateCode == 0)
                {
                    model.StateList = comm.PopulateStates(false);
                    model.DistrictList = new List<SelectListItem>();
                    model.DistrictList.Insert(0, new SelectListItem { Text = "All Districts", Value = "0" });
                }
                if (PMGSYSession.Current.StateCode > 0)
                {
                    model.StateList = new List<SelectListItem>();
                    model.StateList.Insert(0, new SelectListItem { Text = PMGSYSession.Current.StateName, Value = PMGSYSession.Current.StateCode.ToString() });

                    if (PMGSYSession.Current.DistrictCode > 0)
                    {
                        model.DistrictList = new List<SelectListItem>();
                        model.DistrictList.Insert(0, new SelectListItem { Text = PMGSYSession.Current.DistrictName, Value = PMGSYSession.Current.DistrictCode.ToString() });
                    }
                    else 
                    {
                        model.DistrictList = comm.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                        model.DistrictList.Find(x => x.Value == "-1").Value = "0";
                    }

                   // model.DistrictCode = (PMGSYSession.Current.DistrictCode > 0) ? PMGSYSession.Current.DistrictCode : 0;
                }
               // model.lstAgency = comm.PopulateAgencies(PMGSYSession.Current.StateCode, true);

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PFMSReports.ReatDscStatusLayout()");
                return Json(new { message = "Error occured while processing your request" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult ReatDscStatusReport(ReatDscStatus model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return View(model);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(new { message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PFMSReports.ReatDscStatusReport()");
                return Json(new { message = "Error occured while processing your request" }, JsonRequestBehavior.AllowGet);
            }
        }

       

        #endregion

    }
}
