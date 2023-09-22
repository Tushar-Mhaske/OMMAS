using PMGSY.Areas.QMSSRSReports.Models;
using PMGSY.BAL.TourClaim;
using PMGSY.Common;
using PMGSY.Extensions;
using PMGSY.Models;
using PMGSY.Models.NQMTourClaimModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Controllers
{
    public class TourClaimController : Controller
    {
        PMGSYEntities dbContext = new PMGSYEntities();
        CommonFunctions objCommon = new CommonFunctions();
        Dictionary<string, string> decryptedParameters = null;

        public ActionResult Index()
        {
            return View();
        }

        #region NQM Tour Claim 

        #region Main Form

        public ActionResult ScheduleFiltersNQM()
        {
            try
            {
                NQM_TOUR_CLAIM_MASTER_MODEL qmFilterModel = new NQM_TOUR_CLAIM_MASTER_MODEL();
                CommonFunctions objCommonFunctions = new CommonFunctions();

                qmFilterModel.FROM_MONTH = DateTime.Now.Month;
                qmFilterModel.FROM_YEAR = DateTime.Now.Year;
                qmFilterModel.FROM_MONTHS_LIST = objCommonFunctions.PopulateMonths(false);
                qmFilterModel.FROM_YEARS_LIST = objCommonFunctions.PopulateYears(false).Where(a => a.Value != "0").ToList();
                if (DateTime.Now.Month == 12)
                {
                    string nextYear = (DateTime.Now.Year + 1).ToString();
                    qmFilterModel.FROM_YEARS_LIST.Add(new SelectListItem { Text = nextYear, Value = nextYear });
                    qmFilterModel.FROM_YEARS_LIST = qmFilterModel.FROM_YEARS_LIST.OrderByDescending(c => c.Value).ToList();
                }
                return View(qmFilterModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.ScheduleFiltersNQM()");
                return null;
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult GetNQMCurrScheduleList(FormCollection formCollection)
        {
            ITourClaimBAL objBAL = new TourClaimBAL();
            int totalRecords;
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.GetNQMCurrScheduleListBAL(Convert.ToInt32(formCollection["month"]), Convert.ToInt32(formCollection["year"]), Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords), 
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.GetNQMCurrScheduleList()");
                return Json(String.Empty);
            }
        }

        public ActionResult ViewSummaryDetailsReport(int scheduleCode)
        {
            try
            {
                NQM_TOUR_CLAIM_MASTER masterModel = new NQM_TOUR_CLAIM_MASTER();
                NQM_TOUR_CLAIM_MASTER_MODEL model = new NQM_TOUR_CLAIM_MASTER_MODEL();

                masterModel = dbContext.NQM_TOUR_CLAIM_MASTER.Where(x => x.ADMIN_SCHEDULE_CODE == scheduleCode).FirstOrDefault();
                ViewBag.tourId = masterModel.TOUR_CLAIM_ID;

                model.ADMIN_SCHEDULE_CODE = masterModel.ADMIN_SCHEDULE_CODE;
                model.MONTH_OF_INSPECTION = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(masterModel.MONTH_OF_INSPECTION);
                model.YEAR_OF_INSPECTION = masterModel.YEAR_OF_INSPECTION;
                model.finalizeFlag = masterModel.FINALIZE_FLAG;
                model.MONTH_NUMBER = masterModel.MONTH_OF_INSPECTION;

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.ViewSummaryDetailsReport()");
                return null;
            }
        }

        public ActionResult AddNewTourDetails(string id)
        {
            try
            {
                NQM_TOUR_CLAIM_MASTER_MODEL modelView = new NQM_TOUR_CLAIM_MASTER_MODEL();
                NQM_TOUR_CLAIM_MASTER model = new NQM_TOUR_CLAIM_MASTER();
                QUALITY_QM_SCHEDULE scheduleModel = new QUALITY_QM_SCHEDULE();
                ADMIN_QUALITY_MONITORS adminMonitor = new ADMIN_QUALITY_MONITORS();
                NQM_TOUR_MASTER_LETTER_NUMBER letter = new NQM_TOUR_MASTER_LETTER_NUMBER();


                int scheduleId = Convert.ToInt32(id);

                scheduleModel = dbContext.QUALITY_QM_SCHEDULE.Where(x => x.ADMIN_SCHEDULE_CODE == scheduleId).FirstOrDefault();
                adminMonitor = dbContext.ADMIN_QUALITY_MONITORS.Where(x => x.ADMIN_QM_CODE == scheduleModel.ADMIN_QM_CODE).FirstOrDefault();

                model.TOUR_CLAIM_ID = dbContext.NQM_TOUR_CLAIM_MASTER.Max(cp => (Int32?)cp.TOUR_CLAIM_ID) == null ? 1 : (Int32)dbContext.NQM_TOUR_CLAIM_MASTER.Max(cp => (Int32?)cp.TOUR_CLAIM_ID) + 1;
                model.ADMIN_QM_CODE = adminMonitor.ADMIN_QM_CODE;
                model.ADMIN_SCHEDULE_CODE = scheduleId;
                model.DATE_OF_CLAIM = DateTime.Now;
                model.MONTH_OF_INSPECTION = scheduleModel.ADMIN_IM_MONTH;
                model.YEAR_OF_INSPECTION = scheduleModel.ADMIN_IM_YEAR;
                model.FINALIZE_FLAG = 0;

                var financialYear = (DateTime.Now.Month > 3) ? (DateTime.Now.Year + "-" + (DateTime.Now.Year + 1)) : ((DateTime.Now.Year - 1) + "-" + (DateTime.Now.Year));
                var runningNum = dbContext.NQM_TOUR_MASTER_LETTER_NUMBER.Where(x => x.FINANCIAL_YEAR == financialYear).Any() ? dbContext.NQM_TOUR_MASTER_LETTER_NUMBER.Where(x => x.FINANCIAL_YEAR == financialYear).Max(y => y.RUNNING_NUM) + 1 : 0;
                model.NRRDA_LETTER_NUMBER = "NRRDA/" + financialYear + "/N/" + runningNum;

                model.USERID = PMGSYSession.Current.UserId;
                model.IPADD = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                dbContext.NQM_TOUR_CLAIM_MASTER.Add(model);
                dbContext.SaveChanges();

                string letterNumber = model.NRRDA_LETTER_NUMBER;
                string[] letterDetails = letterNumber.Split('/');
                letter.LETTER_ID = dbContext.NQM_TOUR_MASTER_LETTER_NUMBER.Max(x => (Int32?)x.LETTER_ID) == null ? 1 : (Int32)dbContext.NQM_TOUR_MASTER_LETTER_NUMBER.Max(x => (Int32?)x.LETTER_ID) + 1;
                letter.QM_TYPE = letterDetails[2];
                letter.FINANCIAL_YEAR = letterDetails[1];
                letter.RUNNING_NUM = Convert.ToInt32(letterDetails[3]);

                dbContext.NQM_TOUR_MASTER_LETTER_NUMBER.Add(letter);
                dbContext.SaveChanges();

                var list = dbContext.USP_QM_QUICK_RESPONSE_SHEET("I", scheduleModel.ADMIN_QM_CODE, scheduleModel.ADMIN_IM_YEAR, scheduleModel.ADMIN_IM_MONTH, 0, scheduleModel.MAST_STATE_CODE, 0).ToList();
                decimal amt = dbContext.NQM_TOUR_ALLOWANCE_MASTER.Where(x => x.ALLOWANCE_TYPE.Equals("Inspection") && x.IS_ACTIVE == "Y").Select(y => y.AMOUNT).FirstOrDefault();
                DateTime inspDate;

                foreach (var item in list)
                {
                    NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS roadInspec = new NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS();
                    inspDate = Convert.ToDateTime(item.QM_INSPECTION_DATE);

                    roadInspec.HONORARIUM_INSPECTION_ID = dbContext.NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS.Max(cp => (Int32?)cp.HONORARIUM_INSPECTION_ID) == null ? 1 : (Int32)dbContext.NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS.Max(cp => (Int32?)cp.HONORARIUM_INSPECTION_ID) + 1;
                    roadInspec.TOUR_CLAIM_ID = model.TOUR_CLAIM_ID;
                    roadInspec.DATE_OF_CLAIM = DateTime.Now;
                    roadInspec.DATE_OF_INSPECTION = Convert.ToDateTime(item.QM_INSPECTION_DATE);
                    roadInspec.TYPE_OF_WORK = item.ADMIN_IS_ENQUIRY == "Y" ? "E" : item.ADMIN_IS_ENQUIRY;

                    if (dbContext.NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS.Where(x => x.TOUR_CLAIM_ID == model.TOUR_CLAIM_ID && x.DATE_OF_INSPECTION == inspDate).Any())
                    {
                        roadInspec.AMOUNT_CLAIMED = 0;
                        roadInspec.AMOUNT_PASSED_CQC = 0;
                    }
                    else
                    {
                        roadInspec.AMOUNT_CLAIMED = amt;
                        roadInspec.AMOUNT_PASSED_CQC = amt;
                    }


                    roadInspec.QRS_FLAG = 1;
                    roadInspec.USERID = PMGSYSession.Current.UserId;
                    roadInspec.IPADD = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    dbContext.NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS.Add(roadInspec);
                    dbContext.SaveChanges();
                }

                modelView.TOUR_CLAIM_ID = model.TOUR_CLAIM_ID;
                modelView.ADMIN_SCHEDULE_CODE = model.ADMIN_SCHEDULE_CODE;
                modelView.ADMIN_QM_CODE = model.ADMIN_QM_CODE;
                DateTime date = new DateTime(2020, scheduleModel.ADMIN_IM_MONTH, 1);
                var month = date.ToString("MMMM");
                modelView.MONTH_OF_INSPECTION = month;
                modelView.YEAR_OF_INSPECTION = model.YEAR_OF_INSPECTION;

                return View("ViewNQMDEtails", modelView);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.AddNewTourDetails()");
                return null;
            }
        }

        public ActionResult ViewNQMDEtails(string id)
        {
            try
            {
                ADMIN_QUALITY_MONITORS adminMonitor = new ADMIN_QUALITY_MONITORS();
                NQM_TOUR_CLAIM_MASTER_MODEL model = new NQM_TOUR_CLAIM_MASTER_MODEL();
                QUALITY_QM_SCHEDULE scheduleModel = new QUALITY_QM_SCHEDULE();

                int scheduleId = Convert.ToInt32(id);

                scheduleModel = dbContext.QUALITY_QM_SCHEDULE.Where(x => x.ADMIN_SCHEDULE_CODE == scheduleId).FirstOrDefault();
                adminMonitor = dbContext.ADMIN_QUALITY_MONITORS.Where(x => x.ADMIN_QM_CODE == scheduleModel.ADMIN_QM_CODE).FirstOrDefault();

                model.ADMIN_QM_CODE = adminMonitor.ADMIN_QM_CODE;
                model.ADMIN_SCHEDULE_CODE = scheduleModel.ADMIN_SCHEDULE_CODE;
                model.ADMIN_QM_NAME = (adminMonitor.ADMIN_QM_FNAME == null ? "" : adminMonitor.ADMIN_QM_FNAME) + " " + (adminMonitor.ADMIN_QM_MNAME == null ? "" : adminMonitor.ADMIN_QM_MNAME) + " " + (adminMonitor.ADMIN_QM_LNAME == null ? "" : adminMonitor.ADMIN_QM_LNAME);
                model.ADMIN_QM_ADDRESS = (adminMonitor.ADMIN_QM_ADDRESS1 == null ? "" : adminMonitor.ADMIN_QM_ADDRESS1) + " " + (adminMonitor.ADMIN_QM_ADDRESS2 == null ? "" : adminMonitor.ADMIN_QM_ADDRESS2);
                model.ADMIN_QM_PAN = adminMonitor.ADMIN_QM_PAN == null ? "--" : adminMonitor.ADMIN_QM_PAN;
                model.ADMIN_QM_MOBILE = (adminMonitor.ADMIN_QM_MOBILE1 == null ? (adminMonitor.ADMIN_QM_MOBILE2 == null ? "--" : adminMonitor.ADMIN_QM_MOBILE2) : adminMonitor.ADMIN_QM_MOBILE1);
                model.ADMIN_QM_EMAIL = adminMonitor.ADMIN_QM_EMAIL == null ? "--" : adminMonitor.ADMIN_QM_EMAIL;
                model.DATE_OF_CLAIM = DateTime.Now.ToString("dd/MM/yyyy");
                DateTime date = new DateTime(2020, scheduleModel.ADMIN_IM_MONTH, 1);
                var month = date.ToString("MMMM");
                model.MONTH_OF_INSPECTION = month;
                model.YEAR_OF_INSPECTION = scheduleModel.ADMIN_IM_YEAR;
                model.LST_BANK_NAME = objCommon.PopulatePFMSBankNames();
                model.addEditCheck = 1;  // 0 indicates edit, 1 indicates adding, 2 indicates view 

                var financialYear = (DateTime.Now.Month > 3) ? (DateTime.Now.Year + "-" + (DateTime.Now.Year + 1)) : ((DateTime.Now.Year - 1) + "-" + (DateTime.Now.Year));
                var runningNum = dbContext.NQM_TOUR_MASTER_LETTER_NUMBER.Where(x => x.FINANCIAL_YEAR == financialYear).Any() ? dbContext.NQM_TOUR_MASTER_LETTER_NUMBER.Where(x => x.FINANCIAL_YEAR == financialYear).Max(y => y.RUNNING_NUM) + 1 : 0;
                model.NRRDA_LETTER_NUMBER = "NRRDA/" + financialYear + "/N/" + runningNum;

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.ViewNQMDEtails()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult InsertTourClaimDetails(FormCollection formCollection)
        {
            try
            {
                ITourClaimBAL objBAL = new TourClaimBAL();
                bool status = false;
                string isValidMsg = String.Empty;
                String message = "Tour claim details not added";

                if (ModelState.IsValid)
                {
                    status = objBAL.InsertTourClaimDetailsBAL(formCollection, out isValidMsg);

                    return Json(new { success = status, message = isValidMsg });
                }

                return Json(new { success = status, message = message });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.InsertTourClaimDetails()");
                return null;
            }
        }

        public ActionResult EditTourDetails(string tourClaimId)
        {
            try
            {
                ADMIN_QUALITY_MONITORS adminMonitor = new ADMIN_QUALITY_MONITORS();
                NQM_TOUR_CLAIM_MASTER_MODEL model = new NQM_TOUR_CLAIM_MASTER_MODEL();
                NQM_TOUR_CLAIM_MASTER tourModel = new NQM_TOUR_CLAIM_MASTER();

                int tourId = Convert.ToInt32(tourClaimId);

                tourModel = dbContext.NQM_TOUR_CLAIM_MASTER.Where(x => x.TOUR_CLAIM_ID == tourId).FirstOrDefault();
                adminMonitor = dbContext.ADMIN_QUALITY_MONITORS.Where(x => x.ADMIN_QM_CODE == tourModel.ADMIN_QM_CODE).FirstOrDefault();

                model.TOUR_CLAIM_ID = tourId;
                model.ADMIN_QM_CODE = adminMonitor.ADMIN_QM_CODE;
                model.ADMIN_SCHEDULE_CODE = tourModel.ADMIN_SCHEDULE_CODE;
                model.ADMIN_QM_NAME = (adminMonitor.ADMIN_QM_FNAME == null ? "" : adminMonitor.ADMIN_QM_FNAME) + " " + (adminMonitor.ADMIN_QM_MNAME == null ? "" : adminMonitor.ADMIN_QM_MNAME) + " " + (adminMonitor.ADMIN_QM_LNAME == null ? "" : adminMonitor.ADMIN_QM_LNAME);
                model.ADMIN_QM_ADDRESS = (adminMonitor.ADMIN_QM_ADDRESS1 == null ? "" : adminMonitor.ADMIN_QM_ADDRESS1) + " " + (adminMonitor.ADMIN_QM_ADDRESS2 == null ? "" : adminMonitor.ADMIN_QM_ADDRESS2);
                model.ADMIN_QM_PAN = adminMonitor.ADMIN_QM_PAN == null ? "--" : adminMonitor.ADMIN_QM_PAN;
                model.ADMIN_QM_MOBILE = (adminMonitor.ADMIN_QM_MOBILE1 == null ? (adminMonitor.ADMIN_QM_MOBILE2 == null ? "--" : adminMonitor.ADMIN_QM_MOBILE2) : adminMonitor.ADMIN_QM_MOBILE1);
                model.ADMIN_QM_EMAIL = adminMonitor.ADMIN_QM_EMAIL == null ? "--" : adminMonitor.ADMIN_QM_EMAIL;
                model.DATE_OF_CLAIM = tourModel.DATE_OF_CLAIM.ToShortDateString();
                model.NRRDA_LETTER_NUMBER = tourModel.NRRDA_LETTER_NUMBER;
                DateTime date = new DateTime(2020, tourModel.MONTH_OF_INSPECTION, 1);
                var month = date.ToString("MMMM");
                model.MONTH_OF_INSPECTION = month;
                model.YEAR_OF_INSPECTION = tourModel.YEAR_OF_INSPECTION;

                model.LST_BANK_NAME = objCommon.PopulatePFMSBankNames();
                model.addEditCheck = 0;  // 1 indicates adding and 0 indicates edit

                return View("ViewNQMDEtails", model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.EditTourDetails()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult FinalizeTourDetail(int scheduleCode)
        {
            try
            {
                ITourClaimBAL objBAL = new TourClaimBAL();
                bool status = false;
                string isValidMsg = String.Empty;
                String message = "Tour claim details not finalized";

                if (ModelState.IsValid)
                {
                    status = objBAL.FinalizeTourDetailBAL(scheduleCode, out isValidMsg);

                    return Json(new { success = status, message = isValidMsg });
                }

                return Json(new { success = status, message = message });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.FinalizeTourDetail()");
                return null;
            }
        }

        public ActionResult ViewAllDetails(int scheduleCode)
        {
            try
            {
                NQM_TOUR_CLAIM_MASTER_MODEL model = new NQM_TOUR_CLAIM_MASTER_MODEL();
                ADMIN_QUALITY_MONITORS adminMonitor = new ADMIN_QUALITY_MONITORS();
                NQM_TOUR_CLAIM_MASTER tourModel = new NQM_TOUR_CLAIM_MASTER();
                ADMIN_QUALITY_MONITORS_BANK bankModel = new ADMIN_QUALITY_MONITORS_BANK();

                tourModel = dbContext.NQM_TOUR_CLAIM_MASTER.Where(x => x.ADMIN_SCHEDULE_CODE == scheduleCode).FirstOrDefault();
                adminMonitor = dbContext.ADMIN_QUALITY_MONITORS.Where(x => x.ADMIN_QM_CODE == tourModel.ADMIN_QM_CODE).FirstOrDefault();
                bankModel = dbContext.ADMIN_QUALITY_MONITORS_BANK.Where(x => x.ADMIN_QM_CODE == tourModel.ADMIN_QM_CODE).FirstOrDefault();
                int tourId = tourModel.TOUR_CLAIM_ID;

                List<TOUR_CLAIM_CALCULATION_Result> lstExecution = new List<TOUR_CLAIM_CALCULATION_Result>();

                var item = dbContext.TOUR_CLAIM_CALCULATION(tourId, null, null).FirstOrDefault();

                model.TOUR_CLAIM_ID = tourId;
                model.ADMIN_SCHEDULE_CODE = tourModel.ADMIN_SCHEDULE_CODE;

                model.ADMIN_QM_NAME = (adminMonitor.ADMIN_QM_FNAME == null ? "" : adminMonitor.ADMIN_QM_FNAME) + " " + (adminMonitor.ADMIN_QM_MNAME == null ? "" : adminMonitor.ADMIN_QM_MNAME) + " " + (adminMonitor.ADMIN_QM_LNAME == null ? "" : adminMonitor.ADMIN_QM_LNAME);
                model.NRRDA_LETTER_NUMBER = tourModel.NRRDA_LETTER_NUMBER;
                model.ADMIN_QM_PAN = adminMonitor.ADMIN_QM_PAN == null ? "--" : adminMonitor.ADMIN_QM_PAN;

                model.BANK_NAME = bankModel.MAST_BANK_NAME;
                model.ACCOUNT_NUMBER = bankModel.MAST_ACCOUNT_NUMBER;
                model.IFSC_CODE = bankModel.MAST_IFSC_CODE;

                model.DISTRICT_VISITED_ALLOWANCE = item.DISTRICT_VISITED_ALLOWANCE;
                model.TOTAL_AMOUNT_CLAIMED = item.DISTRICT_VISITED_ALLOWANCE + 200 + item.TOTAL_TRAVEL_CLAIM_AMOUNT + item.TOTAL_LODGE_CLAIM_AMOUNT + item.TOTAL_DAILY_CLAIM_AMOUNT + item.TOTAL_INSPECTION_CLAIM_AMOUNT + item.TOTAL_MEETING_CLAIM_AMOUNT + item.TOTAL_MISCELLANEOUS_CLAIM_AMOUNT;
                model.TOTAL_AMOUNT_SANCTIONED = tourModel.FINALIZE_FLAG == 5 ? ((item.DISTRICT_VISITED_ALLOWANCE + item.TOTAL_TRAVEL_SANCTIONED_AMOUNT + item.TOTAL_LODGE_SANCTIONED_AMOUNT + item.TOTAL_DAILY_SANCTIONED_AMOUNT + item.TOTAL_INSPECTION_SANCTIONED_AMOUNT + item.TOTAL_MEETING_SANCTIONED_AMOUNT + item.TOTAL_MISCELLANEOUS_SANCTIONED_AMOUNT + 200).ToString() == null ? "--" : (item.DISTRICT_VISITED_ALLOWANCE + item.TOTAL_TRAVEL_SANCTIONED_AMOUNT + item.TOTAL_LODGE_SANCTIONED_AMOUNT + item.TOTAL_DAILY_SANCTIONED_AMOUNT + item.TOTAL_INSPECTION_SANCTIONED_AMOUNT + item.TOTAL_MEETING_SANCTIONED_AMOUNT + item.TOTAL_MISCELLANEOUS_SANCTIONED_AMOUNT + 200).ToString()) : "Not yet sanctioned";

                model.addEditCheck = 2;
                model.finalizeFlag = tourModel.FINALIZE_FLAG;

                return View("ViewNQMDEtails", model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.ViewAllDetails()");
                return null;
            }
        }

        #endregion


        #region District

        public ActionResult ViewDistrictDetails(int scheduleCode)
        {
            try
            {
                NQM_TOUR_CLAIM_MASTER masterModel = new NQM_TOUR_CLAIM_MASTER();
                NQM_TOUR_DISTRICT_DETAILS_MODEL model = new NQM_TOUR_DISTRICT_DETAILS_MODEL();
                QUALITY_QM_SCHEDULE scheduleModel = new QUALITY_QM_SCHEDULE();

                masterModel = dbContext.NQM_TOUR_CLAIM_MASTER.Where(x => x.ADMIN_SCHEDULE_CODE == scheduleCode).FirstOrDefault();
                scheduleModel = dbContext.QUALITY_QM_SCHEDULE.Where(x => x.ADMIN_SCHEDULE_CODE == scheduleCode).FirstOrDefault();

                model.TOUR_CLAIM_ID = masterModel.TOUR_CLAIM_ID;
                model.ADMIN_SCHEDULE_CODE = scheduleCode;
                model.DISTRICT_CODE = -1;

                model.lstDistricts = new List<SelectListItem>();
                model.lstDistricts.Insert(0, new SelectListItem() { Text = "Select District", Value = "-1" });
                model.ADD_EDIT = 1; // 1 for Add and 2 for Edit

                var stateName = dbContext.MASTER_STATE.Where(x => x.MAST_STATE_CODE == scheduleModel.MAST_STATE_CODE).Select(y => y.MAST_STATE_NAME).FirstOrDefault();

                if (scheduleModel.MAST_DISTRICT_CODE != null)
                {
                    var districtName = dbContext.MASTER_DISTRICT.Where(x => x.MAST_DISTRICT_CODE == scheduleModel.MAST_DISTRICT_CODE).Select(y => y.MAST_DISTRICT_NAME).FirstOrDefault();
                    model.lstDistricts.Insert(1, new SelectListItem() { Text = districtName.ToString() + " (" + stateName.ToString() + ")", Value = scheduleModel.MAST_DISTRICT_CODE.ToString() });
                }
                if (scheduleModel.MAST_DISTRICT_CODE2 != null)
                {
                    var districtName2 = dbContext.MASTER_DISTRICT.Where(x => x.MAST_DISTRICT_CODE == scheduleModel.MAST_DISTRICT_CODE2).Select(y => y.MAST_DISTRICT_NAME).FirstOrDefault();
                    model.lstDistricts.Insert(1, new SelectListItem() { Text = districtName2.ToString() + " (" + stateName.ToString() + ")", Value = scheduleModel.MAST_DISTRICT_CODE2.ToString() });
                }
                if (scheduleModel.MAST_DISTRICT_CODE3 != null)
                {
                    var districtName3 = dbContext.MASTER_DISTRICT.Where(x => x.MAST_DISTRICT_CODE == scheduleModel.MAST_DISTRICT_CODE3).Select(y => y.MAST_DISTRICT_NAME).FirstOrDefault();
                    model.lstDistricts.Insert(1, new SelectListItem() { Text = districtName3.ToString() + " (" + stateName.ToString() + ")", Value = scheduleModel.MAST_DISTRICT_CODE3.ToString() });
                }

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.ViewDistrictDetails()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult InsertDistrictDetails(FormCollection formCollection)
        {
            try
            {
                ITourClaimBAL objBAL = new TourClaimBAL();
                bool status = false;
                string isValidMsg = String.Empty;
                String message = "District details not saved";
                int tourClaimId = Convert.ToInt32(formCollection["TOUR_CLAIM_ID"]);
                int district = Convert.ToInt32(formCollection["DISTRICT_CODE"]);

                if (dbContext.NQM_TOUR_DISTRICT_DETAILS.Where(x => x.TOUR_CLAIM_ID == tourClaimId && x.DISTRICT == district).Any())
                    return Json(new { success = false, file = false, message = "District already added !!" });

                if (ModelState.IsValid)
                {
                    status = objBAL.InsertDistrictDetailsBAL(formCollection, out isValidMsg);

                    return Json(new { success = status, message = isValidMsg });
                }

                return Json(new { success = status, message = message });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.InsertDistrictDetails()");
                return null;
            }
        }

        [Audit]
        public ActionResult GetTourDistrictList(int scheduleCode, int? page, int? rows, string sidx, string sord)

        {
            try
            {
                long totalRecords = 0;
                ITourClaimBAL objBAL = new TourClaimBAL();

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.GetTourDistrictListBAL(scheduleCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.GetTourClaimList()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult DeleteDistrictDetail(int districtId)
        {
            int districtCode, tourClaimId;
            NQM_TOUR_HONORARIUM_MEETING_WITH_PIU meetingModel = new NQM_TOUR_HONORARIUM_MEETING_WITH_PIU();

            try
            {
                using (var scope = new TransactionScope())
                {
                    bool status = false;
                    String message = "Details not deleted";

                    districtCode = dbContext.NQM_TOUR_DISTRICT_DETAILS.Where(x => x.DISTRICT_DETAILS_ID == districtId).Select(y => y.DISTRICT).FirstOrDefault();
                    tourClaimId = dbContext.NQM_TOUR_DISTRICT_DETAILS.Where(x => x.DISTRICT_DETAILS_ID == districtId).Select(y => y.TOUR_CLAIM_ID).FirstOrDefault();
                    if (dbContext.NQM_TOUR_HONORARIUM_MEETING_WITH_PIU.Where(x => x.TOUR_CLAIM_ID == tourClaimId && x.DISTRICT == districtCode).Any())
                    {
                        status = false;
                        message = "District cannot be deleted as details for this district already exists in Meeting with PIU.";

                        return Json(new { success = status, message = message });
                    }

                    dbContext.NQM_TOUR_DISTRICT_DETAILS.Remove(dbContext.NQM_TOUR_DISTRICT_DETAILS.Where(x => x.DISTRICT_DETAILS_ID == districtId).FirstOrDefault());
                    dbContext.SaveChanges();

                    status = true;
                    message = "Details deleted successfully";

                    scope.Complete();

                    return Json(new { success = status, message = message });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.DeleteDistrictDetail()");
                return Json(new { success = false, message = "Details not deleted." });
            }
        }

        public ActionResult EditDistrictDetail(int districtId)
        {
            try
            {
                NQM_TOUR_CLAIM_MASTER masterModel = new NQM_TOUR_CLAIM_MASTER();
                NQM_TOUR_DISTRICT_DETAILS disModel = new NQM_TOUR_DISTRICT_DETAILS();
                NQM_TOUR_DISTRICT_DETAILS_MODEL model = new NQM_TOUR_DISTRICT_DETAILS_MODEL();
                QUALITY_QM_SCHEDULE scheduleModel = new QUALITY_QM_SCHEDULE();

                disModel = dbContext.NQM_TOUR_DISTRICT_DETAILS.Where(x => x.DISTRICT_DETAILS_ID == districtId).FirstOrDefault();
                masterModel = dbContext.NQM_TOUR_CLAIM_MASTER.Where(x => x.TOUR_CLAIM_ID == disModel.TOUR_CLAIM_ID).FirstOrDefault();
                scheduleModel = dbContext.QUALITY_QM_SCHEDULE.Where(x => x.ADMIN_SCHEDULE_CODE == masterModel.ADMIN_SCHEDULE_CODE).FirstOrDefault();

                model.DISTRICT_DETAILS_ID = districtId;
                model.TOUR_CLAIM_ID = masterModel.TOUR_CLAIM_ID;
                model.ADMIN_SCHEDULE_CODE = masterModel.ADMIN_SCHEDULE_CODE;
                model.DISTRICT_CODE = disModel.DISTRICT;
                model.DATE_FROM = disModel.DATE_FROM;
                model.DATE_TO = disModel.DATE_TO;
                model.ADD_EDIT = 2;

                model.lstDistricts = new List<SelectListItem>();
                model.lstDistricts.Insert(0, new SelectListItem() { Text = "Select District", Value = "-1" });

                var stateName = dbContext.MASTER_STATE.Where(x => x.MAST_STATE_CODE == scheduleModel.MAST_STATE_CODE).Select(y => y.MAST_STATE_NAME).FirstOrDefault();

                if (scheduleModel.MAST_DISTRICT_CODE != null)
                {
                    var districtName = dbContext.MASTER_DISTRICT.Where(x => x.MAST_DISTRICT_CODE == scheduleModel.MAST_DISTRICT_CODE).Select(y => y.MAST_DISTRICT_NAME).FirstOrDefault();
                    model.lstDistricts.Insert(1, new SelectListItem() { Text = districtName.ToString() + " (" + stateName.ToString() + ")", Value = scheduleModel.MAST_DISTRICT_CODE.ToString() });
                }
                if (scheduleModel.MAST_DISTRICT_CODE2 != null)
                {
                    var districtName2 = dbContext.MASTER_DISTRICT.Where(x => x.MAST_DISTRICT_CODE == scheduleModel.MAST_DISTRICT_CODE2).Select(y => y.MAST_DISTRICT_NAME).FirstOrDefault();
                    model.lstDistricts.Insert(1, new SelectListItem() { Text = districtName2.ToString() + " (" + stateName.ToString() + ")", Value = scheduleModel.MAST_DISTRICT_CODE2.ToString() });
                }
                if (scheduleModel.MAST_DISTRICT_CODE3 != null)
                {
                    var districtName3 = dbContext.MASTER_DISTRICT.Where(x => x.MAST_DISTRICT_CODE == scheduleModel.MAST_DISTRICT_CODE3).Select(y => y.MAST_DISTRICT_NAME).FirstOrDefault();
                    model.lstDistricts.Insert(1, new SelectListItem() { Text = districtName3.ToString() + " (" + stateName.ToString() + ")", Value = scheduleModel.MAST_DISTRICT_CODE3.ToString() });
                }

                return View("ViewDistrictDetails", model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.EditDistrictDetail()");
                return null;
            }
        }

        #endregion


        #region Travel

        public ActionResult AddTravelClaim(int scheduleCode)
        {
            try
            {
                NQM_TRAVEL_CLAIM_DETAILS_MODEL travelClaimModel = new NQM_TRAVEL_CLAIM_DETAILS_MODEL();
                NQM_TOUR_CLAIM_MASTER masterModel = new NQM_TOUR_CLAIM_MASTER();

                masterModel = dbContext.NQM_TOUR_CLAIM_MASTER.Where(x => x.ADMIN_SCHEDULE_CODE == scheduleCode).FirstOrDefault();

                travelClaimModel.TOUR_CLAIM_ID = masterModel.TOUR_CLAIM_ID;
                travelClaimModel.ADMIN_SCHEDULE_CODE = masterModel.ADMIN_SCHEDULE_CODE;
                travelClaimModel.lstModes = new List<SelectListItem>();

                List<NQM_TOUR_CLAIM_MODE_OF_TRAVEL> modeModel = new List<NQM_TOUR_CLAIM_MODE_OF_TRAVEL>();
                modeModel = dbContext.NQM_TOUR_CLAIM_MODE_OF_TRAVEL.ToList();

                foreach (var item in modeModel)
                {
                    travelClaimModel.lstModes.Add(new SelectListItem() { Text = item.MODE_OF_TRAVEL, Value = item.ID.ToString() });
                }

                List<SelectListItem> lstClass = new List<SelectListItem>();
                lstClass.Insert(0, new SelectListItem { Text = "Select Class", Value = "-1" });
                travelClaimModel.lstClass = lstClass;

                if (!dbContext.NQM_TRAVEL_CLAIM_DETAILS.Where(x => x.TOUR_CLAIM_ID == masterModel.TOUR_CLAIM_ID).Any())
                {
                    int? districtCode = dbContext.ADMIN_QUALITY_MONITORS.Where(x => x.ADMIN_QM_CODE == masterModel.ADMIN_QM_CODE).Select(y => y.MAST_DISTRICT_CODE).FirstOrDefault();
                    travelClaimModel.DEPARTURE_FROM = dbContext.MASTER_DISTRICT.Where(x => x.MAST_DISTRICT_CODE == districtCode).Select(y => y.MAST_DISTRICT_NAME).FirstOrDefault();
                }
                travelClaimModel.ADD_EDIT = 1;

                return PartialView("AddTravelClaim", travelClaimModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.AddTravelClaim()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult InsertTravelClaimDetails(FormCollection formCollection)
        {
            try
            {
                ITourClaimBAL objBAL = new TourClaimBAL();
                bool status = false;
                string isValidMsg = String.Empty;
                String message = "Travel Claim details not saved";
                HttpPostedFileBase boardingPass = Request.Files[0];
                HttpPostedFileBase travelTicket = Request.Files[1];
                int maxSize = 1024 * 1024 * 4;
                int tourClaimId = Convert.ToInt32(formCollection["TOUR_CLAIM_ID"]);
                string departureFrom = formCollection["DEPARTURE_FROM"];

                DateTime startDate = Convert.ToDateTime(formCollection["START_DATE_OF_TRAVEL"]);
                DateTime endDate = Convert.ToDateTime(formCollection["END_DATE_OF_TRAVEL"]);
                int startHour = Convert.ToInt32(formCollection["START_HOURS"]);
                int startMinute = Convert.ToInt32(formCollection["START_MINUTES"]);
                int endHour = Convert.ToInt32(formCollection["END_HOURS"]);
                int endMinute = Convert.ToInt32(formCollection["END_MINUTES"]);
                List<NQM_TRAVEL_CLAIM_DETAILS> travelModelList = new List<NQM_TRAVEL_CLAIM_DETAILS>();

                TimeSpan startTime = new TimeSpan(startHour, startMinute, 0);
                TimeSpan endTime = new TimeSpan(endHour, endMinute, 0);

                DateTime startDateTime = startDate + startTime;
                DateTime endDateTime = endDate + endTime;

                if ((startDate == endDate) && (startHour > endHour))
                    return Json(new { success = false, message = "Start Time should be less than End Time." });
                if ((startDate == endDate) && (startHour == endHour) && (startMinute >= endMinute))
                    return Json(new { success = false, message = "Start Time should be less than End Time." });

                travelModelList = dbContext.NQM_TRAVEL_CLAIM_DETAILS.Where(x => x.TOUR_CLAIM_ID == tourClaimId).ToList();

                foreach (var item in travelModelList)
                {
                    TimeSpan itemStartTime = new TimeSpan(item.START_HOURS, item.START_MINUTES, 0);
                    TimeSpan itemEndTime = new TimeSpan(item.END_HOURS, item.END_MINUTES, 0);

                    DateTime itemStartDateTime = item.START_DATE_OF_TRAVEL + itemStartTime;
                    DateTime itemEndDateTime = item.END_DATE_OF_TRAVEL + itemEndTime;

                    if ((itemStartDateTime == startDateTime && itemEndDateTime == endDateTime) ||
                        (itemStartDateTime > startDateTime && itemStartDateTime < endDateTime) ||
                        (itemStartDateTime < startDateTime && itemEndDateTime > endDateTime) ||
                        (itemEndDateTime > startDateTime && itemEndDateTime < endDateTime))
                    {
                        return Json(new { success = false, message = "Start Date Time and End Date Time cannot be between " + itemStartDateTime + " and " + itemEndDateTime + "." });
                    }
                }

                if (!dbContext.NQM_TRAVEL_CLAIM_DETAILS.Where(x => x.TOUR_CLAIM_ID == tourClaimId).Any())
                {
                    int adminQmCode = dbContext.NQM_TOUR_CLAIM_MASTER.Where(x => x.TOUR_CLAIM_ID == tourClaimId).Select(y => y.ADMIN_QM_CODE).FirstOrDefault();
                    int? districtCode = dbContext.ADMIN_QUALITY_MONITORS.Where(x => x.ADMIN_QM_CODE == adminQmCode).Select(y => y.MAST_DISTRICT_CODE).FirstOrDefault();
                    string districtName = dbContext.MASTER_DISTRICT.Where(x => x.MAST_DISTRICT_CODE == districtCode).Select(y => y.MAST_DISTRICT_NAME).FirstOrDefault();

                    if (departureFrom.ToLower() != districtName.ToLower())
                        return Json(new { success = false, message = "Travel should start from " + districtName + " ." });
                }

                if ((Convert.ToInt32(formCollection["MODE_OF_TRAVEL"]) == 2 || Convert.ToInt32(formCollection["MODE_OF_TRAVEL"]) == 3) && (formCollection["TRAVEL_CLASS"].Equals("-1")))
                {
                    return Json(new { success = false, file = false, message = "Please select travel class" });
                }

                if (Request.Files.Count == 0)
                {
                    return Json(new { success = false, file = false, message = "No file selected. Please select file" });
                }
                else if (Convert.ToInt32(formCollection["MODE_OF_TRAVEL"]) == 2)
                {
                    if (boardingPass.ContentLength == 0)
                        return Json(new { success = false, file = false, message = "Boarding pass is required." });
                    else if (boardingPass.FileName.Substring(boardingPass.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "pdf"
                            && boardingPass.FileName.Substring(boardingPass.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "jpg"
                            && boardingPass.FileName.Substring(boardingPass.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "jpeg"
                            && boardingPass.FileName.Substring(boardingPass.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "png")
                        return Json(new { success = false, file = false, message = "Invalid boarding pass. Please upload pdf, jpg, jpeg or png files." });
                    else if (boardingPass.ContentLength > maxSize)
                        return Json(new { success = false, file = false, message = "Invalid boarding pass file size. Please upload file upto 4 MB." });

                    if (travelTicket.ContentLength == 0)
                        return Json(new { success = false, file = false, message = "Ticket is required." });
                    else if (travelTicket.FileName.Substring(travelTicket.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "pdf"
                            && travelTicket.FileName.Substring(travelTicket.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "jpg"
                            && travelTicket.FileName.Substring(travelTicket.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "jpeg"
                            && travelTicket.FileName.Substring(travelTicket.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "png")
                        return Json(new { success = false, file = false, message = "Invalid ticket file. Please upload pdf, jpg, jpeg or png files." });
                    if (travelTicket.ContentLength > maxSize)
                        return Json(new { success = false, file = false, message = "Invalid ticket file size. Please upload file upto 4 MB." });
                }
                else if (Convert.ToInt32(formCollection["MODE_OF_TRAVEL"]) != 5)
                {
                    if (travelTicket.ContentLength == 0)
                        return Json(new { success = false, file = false, message = "Ticket is required." });
                    else if (travelTicket.FileName.Substring(travelTicket.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "pdf"
                            && travelTicket.FileName.Substring(travelTicket.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "jpg"
                            && travelTicket.FileName.Substring(travelTicket.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "jpeg"
                            && travelTicket.FileName.Substring(travelTicket.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "png")
                        return Json(new { success = false, file = false, message = "Invalid ticket file. Please upload pdf, jpg, jpeg or png files." });
                    else if (travelTicket.ContentLength > maxSize)
                        return Json(new { success = false, file = false, message = "Invalid ticket file size. Please upload file upto 4 MB." });
                }

                if (ModelState.IsValid)
                {
                    status = objBAL.InsertTravelClaimDetailsBAL(formCollection, boardingPass, travelTicket, out isValidMsg);

                    return Json(new { success = status, message = isValidMsg });
                }

                return Json(new { success = status, message = message });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.InsertTravelClaimDetails()");
                return null;
            }
        }

        [Audit]
        public ActionResult GetTravelClaimList(int scheduleCode, int? page, int? rows, string sidx, string sord)
        {
            try
            {
                long totalRecords = 0;
                ITourClaimBAL objBAL = new TourClaimBAL();

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.GetTravelClaimListBAL(scheduleCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.GetTravelClaimList()");
                return null;
            }
        }

        public FileResult ViewUploadedTravelTicket(string id1)
        {
            try
            {
                String FileName = String.Empty;
                string[] encParam = id1.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encParam[0], encParam[1], encParam[2] });
                if (decryptedParameters.Count > 0)
                {
                    FileName = decryptedParameters["FileName"];
                }

                string path = ConfigurationManager.AppSettings["NQM_TOUR_CLAIM_PATH"].ToString();
                var fullPath = Path.Combine(path, "TravelClaim", FileName.Split('_')[0], FileName);
                byte[] FileBytes = System.IO.File.ReadAllBytes(fullPath);

                string type = string.Empty;
                type = FileName.Substring(FileName.LastIndexOf('.') + 1).Trim().ToLower();
                if (type == "pdf")
                {
                    return File(FileBytes, "application/pdf");
                }
                else
                {
                    return File(FileBytes, "image/png");
                }

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.ViewUploadedTravelTicket()");
                return null;
            }
        }

        public FileResult ViewUploadedBoardingPass(string id1)
        {
            try
            {
                String FileName = String.Empty;
                string[] encParam = id1.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encParam[0], encParam[1], encParam[2] });
                if (decryptedParameters.Count > 0)
                {
                    FileName = decryptedParameters["FileName"];
                }

                string path = ConfigurationManager.AppSettings["NQM_TOUR_CLAIM_PATH"].ToString();
                var fullPath = Path.Combine(path, "TravelClaim", FileName.Split('_')[0], FileName);

                byte[] FileBytes = System.IO.File.ReadAllBytes(fullPath);
                string type = string.Empty;
                type = FileName.Substring(FileName.LastIndexOf('.') + 1).Trim().ToLower();
                if (type == "pdf")
                {
                    return File(FileBytes, "application/pdf");
                }
                else
                {
                    return File(FileBytes, "image/png");
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.ViewUploadedBoardingPass()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult DeleteTravelDetails(int travelId)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    bool status = false;
                    String message = "Details not deleted";
                    String FileNameTicket = String.Empty;
                    String FileNameBP = String.Empty;
                    String ticketPath = String.Empty;
                    String boardingPassPath = String.Empty;

                    NQM_TRAVEL_CLAIM_DETAILS model = new NQM_TRAVEL_CLAIM_DETAILS();
                    model = dbContext.NQM_TRAVEL_CLAIM_DETAILS.Where(x => x.TRAVEL_CLAIM_ID == travelId).FirstOrDefault();

                    FileNameTicket = model.UPLOADED_TICKET_NAME;
                    ticketPath = model.UPLOADED_TICKET_PATH;
                    FileNameBP = model.BOARDING_PASS_NAME;
                    boardingPassPath = model.BOARDING_PASS_PATH;

                    dbContext.NQM_TRAVEL_CLAIM_DETAILS.Remove(dbContext.NQM_TRAVEL_CLAIM_DETAILS.Where(x => x.TRAVEL_CLAIM_ID == travelId).FirstOrDefault());
                    dbContext.SaveChanges();

                    //string path = ConfigurationManager.AppSettings["NQM_TOUR_CLAIM_PATH"].ToString();

                    if (model.MODE_OF_TRAVEL == 2)
                    {
                        //var fullPathBP = Path.Combine(path, "TravelClaim", FileNameBP.Split('_')[0], FileNameBP);
                        var fullPathBP = Path.Combine(boardingPassPath, FileNameBP);
                        FileInfo fileBP = new FileInfo(fullPathBP);

                        if (fileBP.Exists)
                        {
                            fileBP.Delete();
                        }
                    }

                    if (model.MODE_OF_TRAVEL != 5)
                    {
                        //var fullPathTicket = Path.Combine(path, "TravelClaim", FileNameTicket);
                        var fullPathTicket = Path.Combine(ticketPath, FileNameTicket);
                        FileInfo fileTicket = new FileInfo(fullPathTicket);

                        if (fileTicket.Exists)
                        {
                            fileTicket.Delete();
                        }
                    } 

                    status = true;
                    message = "Details deleted successfully";

                    scope.Complete();

                    return Json(new { success = status, message = message });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.DeleteTravelDetails()");
                return Json(new { success = false, message = "Details not deleted." });
            }
        }

        [HttpPost]
        public ActionResult PopulateTravelClass(int mode)
        {
            try
            {
                List<SelectListItem> lstClass = new List<SelectListItem>();

                lstClass.Insert(0, new SelectListItem { Text = "Select Class", Value = "-1" });

                if (mode == 2)
                {
                    lstClass.Add(new SelectListItem() { Text = "Business", Value = "Business" });
                    lstClass.Add(new SelectListItem() { Text = "Economy", Value = "Economy" });
                }
                else if (mode == 3)
                {
                    lstClass.Add(new SelectListItem() { Text = "First AC", Value = "First AC" });
                    lstClass.Add(new SelectListItem() { Text = "Second AC", Value = "Second AC" });
                    lstClass.Add(new SelectListItem() { Text = "Third AC", Value = "Third AC" });
                    lstClass.Add(new SelectListItem() { Text = "AC Chair Car", Value = "AC Chair Car" });
                    lstClass.Add(new SelectListItem() { Text = "Sleeper", Value = "Sleeper" });
                }

                return Json(lstClass);
            }
            catch(Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.PopulateTravelClass()");
                return Json(new { string.Empty });
            }
        }

        public ActionResult EditTravelDetail(int travelId)
        {
            try
            {
                NQM_TRAVEL_CLAIM_DETAILS_MODEL model = new NQM_TRAVEL_CLAIM_DETAILS_MODEL();
                NQM_TOUR_CLAIM_MASTER masterModel = new NQM_TOUR_CLAIM_MASTER();
                QUALITY_QM_SCHEDULE scheduleModel = new QUALITY_QM_SCHEDULE();
                NQM_TRAVEL_CLAIM_DETAILS travelModel = new NQM_TRAVEL_CLAIM_DETAILS();
                List<NQM_TOUR_CLAIM_MODE_OF_TRAVEL> modeModel = new List<NQM_TOUR_CLAIM_MODE_OF_TRAVEL>();

                travelModel = dbContext.NQM_TRAVEL_CLAIM_DETAILS.Where(x => x.TRAVEL_CLAIM_ID == travelId).FirstOrDefault();
                masterModel = dbContext.NQM_TOUR_CLAIM_MASTER.Where(x => x.TOUR_CLAIM_ID == travelModel.TOUR_CLAIM_ID).FirstOrDefault();
                scheduleModel = dbContext.QUALITY_QM_SCHEDULE.Where(x => x.ADMIN_SCHEDULE_CODE == masterModel.ADMIN_SCHEDULE_CODE).FirstOrDefault();

                model.TRAVEL_CLAIM_ID = travelId;
                model.TOUR_CLAIM_ID = masterModel.TOUR_CLAIM_ID;
                model.ADMIN_SCHEDULE_CODE = masterModel.ADMIN_SCHEDULE_CODE;
                model.START_DATE_OF_TRAVEL = travelModel.START_DATE_OF_TRAVEL;
                model.START_HOURS = travelModel.START_HOURS;
                model.START_MINUTES = travelModel.START_MINUTES;
                model.END_DATE_OF_TRAVEL = travelModel.END_DATE_OF_TRAVEL;
                model.END_HOURS = travelModel.END_HOURS;
                model.END_MINUTES = travelModel.END_MINUTES;
                model.DEPARTURE_FROM = travelModel.DEPARTURE_FROM;
                model.ARRIVAL_AT = travelModel.ARRIVAL_AT;
                model.MODE_OF_TRAVEL = travelModel.MODE_OF_TRAVEL.ToString();
                model.TRAVEL_CLASS = travelModel.TRAVEL_CLASS;
                model.AMOUNT_CLAIMED = travelModel.AMOUNT_CLAIMED;
                model.UPLOADED_TICKET_NAME = travelModel.UPLOADED_TICKET_NAME == null ? "--" : URLEncrypt.EncryptParameters1(new String[] { "FileName =" + travelModel.UPLOADED_TICKET_NAME });
                model.BOARDING_PASS_NAME = travelModel.BOARDING_PASS_NAME == null ? "--" : URLEncrypt.EncryptParameters1(new String[] { "FileName =" + travelModel.UPLOADED_TICKET_NAME });
                model.DEC_UPLOADED_TICKET_NAME = travelModel.UPLOADED_TICKET_NAME == null ? "--" : travelModel.UPLOADED_TICKET_NAME;
                model.DEC_BOARDING_PASS_NAME = travelModel.BOARDING_PASS_NAME == null ? "--" : travelModel.BOARDING_PASS_NAME;

                model.lstModes = new List<SelectListItem>();
                modeModel = dbContext.NQM_TOUR_CLAIM_MODE_OF_TRAVEL.ToList();

                foreach (var item in modeModel)
                {
                    model.lstModes.Add(new SelectListItem() { Text = item.MODE_OF_TRAVEL, Value = item.ID.ToString() });
                }

                List<SelectListItem> lstClass = new List<SelectListItem>();
                PopulateTravelClass(travelModel.MODE_OF_TRAVEL);
                lstClass.Insert(0, new SelectListItem { Text = "Select Class", Value = "-1" });

                if (travelModel.MODE_OF_TRAVEL == 2)
                {
                    lstClass.Add(new SelectListItem() { Text = "Business", Value = "Business" });
                    lstClass.Add(new SelectListItem() { Text = "Economy", Value = "Economy" });
                }
                else if (travelModel.MODE_OF_TRAVEL == 3)
                {
                    lstClass.Add(new SelectListItem() { Text = "First AC", Value = "First AC" });
                    lstClass.Add(new SelectListItem() { Text = "Second AC", Value = "Second AC" });
                    lstClass.Add(new SelectListItem() { Text = "Third AC", Value = "Third AC" });
                    lstClass.Add(new SelectListItem() { Text = "AC Chair Car", Value = "AC Chair Car" });
                    lstClass.Add(new SelectListItem() { Text = "Sleeper", Value = "Sleeper" });
                }

                model.lstClass = lstClass;

                model.ADD_EDIT = 2;

                return PartialView("AddTravelClaim", model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.EditTravelDetail()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult UpdateTravelClaimDetails(FormCollection formCollection)
        {
            try
            {
                ITourClaimBAL objBAL = new TourClaimBAL();
                bool status = false;
                string isValidMsg = String.Empty;
                String message = "Travel Claim details not updated";
                HttpPostedFileBase boardingPass = Request.Files[0];
                HttpPostedFileBase travelTicket = Request.Files[1];
                int maxSize = 1024 * 1024 * 4;
                int travelId = Convert.ToInt32(formCollection["TRAVEL_CLAIM_ID"]);
                NQM_TRAVEL_CLAIM_DETAILS travelModel = new NQM_TRAVEL_CLAIM_DETAILS();
                int modeOfTravel;

                travelModel = dbContext.NQM_TRAVEL_CLAIM_DETAILS.Where(x => x.TRAVEL_CLAIM_ID == travelId).FirstOrDefault();

                DateTime startDate = Convert.ToDateTime(formCollection["START_DATE_OF_TRAVEL"]);
                DateTime endDate = Convert.ToDateTime(formCollection["END_DATE_OF_TRAVEL"]);
                int startHour = Convert.ToInt32(formCollection["START_HOURS"]);
                int startMinute = Convert.ToInt32(formCollection["START_MINUTES"]);
                int endHour = Convert.ToInt32(formCollection["END_HOURS"]);
                int endMinute = Convert.ToInt32(formCollection["END_MINUTES"]);
                List<NQM_TRAVEL_CLAIM_DETAILS> travelModelList = new List<NQM_TRAVEL_CLAIM_DETAILS>();

                TimeSpan startTime = new TimeSpan(startHour, startMinute, 0);
                TimeSpan endTime = new TimeSpan(endHour, endMinute, 0);

                DateTime startDateTime = startDate + startTime;
                DateTime endDateTime = endDate + endTime;

                if ((startDate == endDate) && (startHour > endHour))
                    return Json(new { success = false, message = "Start Time should be less than End Time." });
                if ((startDate == endDate) && (startHour == endHour) && (startMinute >= endMinute))
                    return Json(new { success = false, message = "Start Time should be less than End Time." });

                travelModelList = dbContext.NQM_TRAVEL_CLAIM_DETAILS.Where(x => x.TOUR_CLAIM_ID == travelModel.TOUR_CLAIM_ID && x.TRAVEL_CLAIM_ID != travelId).ToList();

                foreach (var item in travelModelList)
                {
                    TimeSpan itemStartTime = new TimeSpan(item.START_HOURS, item.START_MINUTES, 0);
                    TimeSpan itemEndTime = new TimeSpan(item.END_HOURS, item.END_MINUTES, 0);

                    DateTime itemStartDateTime = item.START_DATE_OF_TRAVEL + itemStartTime;
                    DateTime itemEndDateTime = item.END_DATE_OF_TRAVEL + itemEndTime;

                    if ((itemStartDateTime == startDateTime && itemEndDateTime == endDateTime) ||
                        (itemStartDateTime > startDateTime && itemStartDateTime < endDateTime) ||
                        (itemStartDateTime < startDateTime && itemEndDateTime > endDateTime) ||
                        (itemEndDateTime > startDateTime && itemEndDateTime < endDateTime))
                    {
                        return Json(new { success = false, message = "Start Date Time and End Date Time cannot be between " + itemStartDateTime + " and " + itemEndDateTime + "." });
                    }
                }

                modeOfTravel = travelModel.MODE_OF_TRAVEL;

                if ((modeOfTravel == 2 || modeOfTravel == 3) && (formCollection["TRAVEL_CLASS"].Equals("-1")))
                {
                    return Json(new { success = false, file = false, message = "Please select travel class" });
                }

                if (modeOfTravel == 2)
                {
                    if (boardingPass.ContentLength != 0)
                    {
                        if (boardingPass.FileName.Substring(boardingPass.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "pdf"
                            && boardingPass.FileName.Substring(boardingPass.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "jpg"
                            && boardingPass.FileName.Substring(boardingPass.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "jpeg"
                            && boardingPass.FileName.Substring(boardingPass.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "png")
                            return Json(new { success = false, file = false, message = "Invalid boarding pass. Please upload pdf, jpg, jpeg or png files." });
                        if (boardingPass.ContentLength > maxSize)
                            return Json(new { success = false, file = false, message = "Invalid boarding pass file size. Please upload file upto 4 MB." });
                    }

                    if (travelTicket.ContentLength != 0)
                    {
                        if (travelTicket.FileName.Substring(travelTicket.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "pdf"
                            && travelTicket.FileName.Substring(travelTicket.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "jpg"
                            && travelTicket.FileName.Substring(travelTicket.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "jpeg"
                            && travelTicket.FileName.Substring(travelTicket.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "png")
                            return Json(new { success = false, file = false, message = "Invalid ticket file. Please upload pdf, jpg, jpeg or png files." });
                        if (travelTicket.ContentLength > maxSize)
                            return Json(new { success = false, file = false, message = "Invalid ticket file size. Please upload file upto 4 MB." });
                    }
                }
                else if (modeOfTravel != 5)
                {
                    if (travelTicket.ContentLength != 0)
                    {
                        if (travelTicket.FileName.Substring(travelTicket.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "pdf"
                            && travelTicket.FileName.Substring(travelTicket.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "jpg"
                            && travelTicket.FileName.Substring(travelTicket.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "jpeg"
                            && travelTicket.FileName.Substring(travelTicket.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "png")
                            return Json(new { success = false, file = false, message = "Invalid ticket file. Please upload pdf, jpg, jpeg or png files." });
                        if (travelTicket.ContentLength > maxSize)
                            return Json(new { success = false, file = false, message = "Invalid ticket file size. Please upload file upto 4 MB." });
                    }
                }

                ModelState.Remove("BOARDING_PASS");
                ModelState.Remove("TRAVEL_TICKET");

                if (ModelState.IsValid)
                {
                    status = objBAL.UpdateTravelClaimDetailsBAL(formCollection, boardingPass, travelTicket, out isValidMsg);

                    return Json(new { success = status, message = isValidMsg });
                }

                return Json(new { success = status, message = message });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.UpdateTravelClaimDetails()");
                return null;
            }
        }

        #endregion


        #region Lodge and daily allowance

        public ActionResult AddLodgeClaim(int scheduleCode)
        {
            try
            {
                NQM_LODGE_CLAIM_DETAILS_MODEL lodgeClaimModel = new NQM_LODGE_CLAIM_DETAILS_MODEL();

                NQM_TOUR_CLAIM_MASTER masterModel = new NQM_TOUR_CLAIM_MASTER();

                masterModel = dbContext.NQM_TOUR_CLAIM_MASTER.Where(x => x.ADMIN_SCHEDULE_CODE == scheduleCode).FirstOrDefault();

                lodgeClaimModel.TOUR_CLAIM_ID = masterModel.TOUR_CLAIM_ID;
                lodgeClaimModel.ADMIN_SCHEDULE_CODE = masterModel.ADMIN_SCHEDULE_CODE;

                List<SelectListItem> lstClaim = new List<SelectListItem>();
                lstClaim.Insert(0, new SelectListItem { Text = "Select Claim Type", Value = "-1" });
                lstClaim.Add(new SelectListItem() { Text = "Hotel", Value = "H" });
                lstClaim.Add(new SelectListItem() { Text = "Govt. Guest House", Value = "G" });
                lstClaim.Add(new SelectListItem() { Text = "Self Accomodation", Value = "S" });

                lodgeClaimModel.LST_TYPE_OF_CLAIM = lstClaim;
                lodgeClaimModel.ADD_EDIT = 1;

                return PartialView("AddLodgeClaim", lodgeClaimModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.AddLodgeClaim()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult InsertLodgeClaimDetails(FormCollection formCollection)
        {
            try
            {
                ITourClaimBAL objBAL = new TourClaimBAL();
                bool status = false;
                string isValidMsg = String.Empty;
                String message = "Lodge claim details not saved";
                HttpPostedFileBase gBill = Request.Files[0];
                HttpPostedFileBase bill = Request.Files[1];
                HttpPostedFileBase receipt = Request.Files[2];
                int maxSize = 1024 * 1024 * 4;

                List<NQM_LODGE_AND_DAILY_CLAIM_DETAILS> model = new List<NQM_LODGE_AND_DAILY_CLAIM_DETAILS>();
                int tourId = Convert.ToInt32(formCollection["TOUR_CLAIM_ID"]);
                model = dbContext.NQM_LODGE_AND_DAILY_CLAIM_DETAILS.Where(x => x.TOUR_CLAIM_ID == tourId).ToList();

                foreach (var item in model)
                {
                    if (item.DATE_FROM <= Convert.ToDateTime(formCollection["DATE_FROM"]) && item.DATE_TO > Convert.ToDateTime(formCollection["DATE_FROM"]))
                    {
                        message = "Entry from date " + Convert.ToDateTime(formCollection["DATE_FROM"]).ToShortDateString() + " to date " + item.DATE_TO.ToShortDateString() + " already exist.";
                        return Json(new { success = false, file = false, message });
                    }
                    if (item.DATE_FROM <= Convert.ToDateTime(formCollection["DATE_TO"]) && item.DATE_TO > Convert.ToDateTime(formCollection["DATE_TO"]))
                    {
                        message = "Entry from date " + item.DATE_FROM.ToShortDateString() + " to date " + Convert.ToDateTime(formCollection["DATE_TO"]).ToShortDateString() + " already exist.";
                        return Json(new { success = false, file = false, message });
                    }
                }

                if (formCollection["TYPE_OF_CLAIM"].Equals("H"))
                {
                    if (bill.ContentLength == 0)
                        return Json(new { success = false, file = false, message = "No file selected for bill. Please select file" });
                    if (receipt.ContentLength == 0)
                        return Json(new { success = false, file = false, message = "No file selected for e-Receipt. Please select file" });

                    if (bill.FileName.Substring(bill.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "pdf"
                        && bill.FileName.Substring(bill.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "jpg"
                        && bill.FileName.Substring(bill.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "jpeg"
                        && bill.FileName.Substring(bill.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "png")
                    {
                        return Json(new { success = false, file = false, message = "Invalid bill file. Please upload pdf, jpg, jpeg or png files." });
                    }

                    if (bill.ContentLength > maxSize)
                    {
                        return Json(new { success = false, file = false, message = "Invalid bill file size. Please upload file upto 4 MB." });
                    }

                    if (receipt.FileName.Substring(receipt.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "pdf"
                        && receipt.FileName.Substring(receipt.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "jpg"
                        && receipt.FileName.Substring(receipt.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "jpeg"
                        && receipt.FileName.Substring(receipt.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "png")
                    {
                        return Json(new { success = false, file = false, message = "Invalid e-Receipt file. Please upload pdf, jpg, jpeg or png files." });
                    }

                    if (receipt.ContentLength > maxSize)
                    {
                        return Json(new { success = false, file = false, message = "Invalid e-Receipt file size. Please upload file upto 4 MB." });
                    }
                }
                else if (formCollection["TYPE_OF_CLAIM"].Equals("G"))
                {
                    if (gBill.ContentLength == 0)
                        return Json(new { success = false, file = false, message = "No file selected for bill. Please select file" });
                    if (gBill.FileName.Substring(gBill.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "pdf"
                        && gBill.FileName.Substring(gBill.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "jpg"
                        && gBill.FileName.Substring(gBill.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "jpeg"
                        && gBill.FileName.Substring(gBill.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "png")
                        return Json(new { success = false, file = false, message = "Invalid bill file. Please upload pdf, jpg, jpeg or png files." });
                    if (gBill.ContentLength > maxSize)
                        return Json(new { success = false, file = false, message = "Invalid bill file size. Please upload file upto 4 MB." });
                }

                if (ModelState.IsValid)
                {
                    status = objBAL.InsertLodgeClaimDetailsBAL(formCollection, bill, receipt, gBill, out isValidMsg);

                    return Json(new { success = status, message = isValidMsg });
                }

                return Json(new { success = status, message = message });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.InsertLodgeClaimDetails()");
                return null;
            }
        }

        [Audit]
        public ActionResult GetLodgeClaimList(int scheduleCode, int? page, int? rows, string sidx, string sord)
        {
            try
            {
                long totalRecords = 0;
                ITourClaimBAL objBAL = new TourClaimBAL();

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.GetLodgeClaimListBAL(scheduleCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.GetLodgeClaimList()");
                return null;
            }
        }

        public FileResult ViewUploadedLodgeBill(string id1)
        {
            try
            {
                String FileName = String.Empty;
                string[] encParam = id1.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encParam[0], encParam[1], encParam[2] });
                if (decryptedParameters.Count > 0)
                {
                    FileName = decryptedParameters["FileName"];
                }

                string path = ConfigurationManager.AppSettings["NQM_TOUR_CLAIM_PATH"].ToString();
                var fullPath = Path.Combine(path, "LodgeClaim", FileName.Split('_')[0], FileName);

                byte[] FileBytes = System.IO.File.ReadAllBytes(fullPath);

                string type = string.Empty;
                type = FileName.Substring(FileName.LastIndexOf('.') + 1).Trim().ToLower();
                if (type == "pdf")
                {
                    return File(FileBytes, "application/pdf");
                }
                else
                {
                    return File(FileBytes, "image/png");
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.ViewUploadedLodgeBill()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult DeleteLodgeDetails(int lodgeId)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    bool status = false;
                    String message = "Details not deleted";
                    String FileNameBill = String.Empty;
                    String FileNameReceipt = String.Empty;
                    String FilePathBill = String.Empty;
                    String FilePathReceipt = String.Empty;

                    NQM_LODGE_AND_DAILY_CLAIM_DETAILS model = new NQM_LODGE_AND_DAILY_CLAIM_DETAILS();
                    model = dbContext.NQM_LODGE_AND_DAILY_CLAIM_DETAILS.Where(x => x.LODGE_CLAIM_ID == lodgeId).FirstOrDefault();

                    FileNameBill = model.UPLOADED_BILL_NAME;
                    FileNameReceipt = model.UPLOADED_RECEIPT_NAME;
                    FilePathBill = model.UPLOADED_BILL_PATH;
                    FilePathReceipt = model.UPLOADED_RECEIPT_PATH;
                    int tourId = model.TOUR_CLAIM_ID;
                    DateTime dt = model.DATE_TO;

                    dbContext.NQM_LODGE_AND_DAILY_CLAIM_DETAILS.Remove(dbContext.NQM_LODGE_AND_DAILY_CLAIM_DETAILS.Where(x => x.LODGE_CLAIM_ID == lodgeId).FirstOrDefault());
                    dbContext.SaveChanges();

                    List<NQM_LODGE_AND_DAILY_CLAIM_DETAILS> modelList = new List<NQM_LODGE_AND_DAILY_CLAIM_DETAILS>();

                    modelList = dbContext.NQM_LODGE_AND_DAILY_CLAIM_DETAILS.Where(x => x.TOUR_CLAIM_ID == tourId).ToList();

                    foreach (var item in modelList)
                    {
                        if (dt == item.DATE_FROM)
                        {
                            if (item.TYPE_OF_CLAIM == "H" || item.TYPE_OF_CLAIM == "G")
                            {
                                item.AMOUNT_CLAIMED_DAILY = item.AMOUNT_CLAIMED_DAILY + dbContext.NQM_TOUR_ALLOWANCE_MASTER.Where(x => x.ALLOWANCE_TYPE.Equals("Daily for Hotel") && x.IS_ACTIVE == "Y").Select(y => y.AMOUNT).FirstOrDefault();
                                dbContext.Entry(item).State = System.Data.Entity.EntityState.Modified;
                                dbContext.SaveChanges();
                            }
                            else
                            {
                                item.AMOUNT_CLAIMED_DAILY = item.AMOUNT_CLAIMED_DAILY + dbContext.NQM_TOUR_ALLOWANCE_MASTER.Where(x => x.ALLOWANCE_TYPE.Equals("Daily for Self") && x.IS_ACTIVE == "Y").Select(y => y.AMOUNT).FirstOrDefault();
                                dbContext.Entry(item).State = System.Data.Entity.EntityState.Modified;
                                dbContext.SaveChanges();
                            }
                        }
                    }

                    //string path = ConfigurationManager.AppSettings["NQM_TOUR_CLAIM_PATH"].ToString();

                    if (model.TYPE_OF_CLAIM.Equals("H"))
                    {
                        //var fullPathBill = Path.Combine(path, "LodgeClaim", FileNameBill);
                        //var fullPathReceipt = Path.Combine(path, "LodgeClaim", FileNameReceipt);

                        var fullPathBill = Path.Combine(FilePathBill, FileNameBill);
                        var fullPathReceipt = Path.Combine(FilePathReceipt, FileNameReceipt);

                        FileInfo fileBill = new FileInfo(fullPathBill);
                        FileInfo fileReceipt = new FileInfo(fullPathReceipt);

                        if (fileBill.Exists)
                        {
                            fileBill.Delete();
                        }
                        if (fileReceipt.Exists)
                        {
                            fileReceipt.Delete();
                        }
                    }

                    status = true;
                    message = "Details deleted successfully";

                    scope.Complete();

                    return Json(new { success = status, message = message });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.DeleteLodgeDetails()");
                return Json(new { success = false, message = "Details not deleted." });
            }
        }

        public ActionResult EditLodgeDetail(int lodgeId)
        {
            try
            {
                NQM_LODGE_CLAIM_DETAILS_MODEL model = new NQM_LODGE_CLAIM_DETAILS_MODEL();
                NQM_TOUR_CLAIM_MASTER masterModel = new NQM_TOUR_CLAIM_MASTER();
                QUALITY_QM_SCHEDULE scheduleModel = new QUALITY_QM_SCHEDULE();
                NQM_LODGE_AND_DAILY_CLAIM_DETAILS lodgeModel = new NQM_LODGE_AND_DAILY_CLAIM_DETAILS();

                lodgeModel = dbContext.NQM_LODGE_AND_DAILY_CLAIM_DETAILS.Where(x => x.LODGE_CLAIM_ID == lodgeId).FirstOrDefault();
                masterModel = dbContext.NQM_TOUR_CLAIM_MASTER.Where(x => x.TOUR_CLAIM_ID == lodgeModel.TOUR_CLAIM_ID).FirstOrDefault();
                scheduleModel = dbContext.QUALITY_QM_SCHEDULE.Where(x => x.ADMIN_SCHEDULE_CODE == masterModel.ADMIN_SCHEDULE_CODE).FirstOrDefault();

                model.LODGE_CLAIM_ID = lodgeId;
                model.TOUR_CLAIM_ID = masterModel.TOUR_CLAIM_ID;
                model.ADMIN_SCHEDULE_CODE = masterModel.ADMIN_SCHEDULE_CODE;
                model.DATE_FROM = lodgeModel.DATE_FROM;
                model.DATE_TO = lodgeModel.DATE_TO;
                model.TYPE_OF_CLAIM = lodgeModel.TYPE_OF_CLAIM;
                model.HOTEL_NAME = lodgeModel.HOTEL_NAME;
                model.AMOUNT_CLAIMED_HOTEL = lodgeModel.AMOUNT_CLAIMED_HOTEL;
                model.GUEST_HOUSE_NAME = lodgeModel.HOTEL_NAME;
                model.AMOUNT_CLAIMED_GUEST = lodgeModel.AMOUNT_CLAIMED_HOTEL;
                model.UPLOADED_BILL_NAME = lodgeModel.UPLOADED_BILL_NAME == null ? "--" : URLEncrypt.EncryptParameters1(new String[] { "FileName =" + lodgeModel.UPLOADED_BILL_NAME });
                model.UPLOADED_RECEIPT_NAME = lodgeModel.UPLOADED_RECEIPT_NAME == null ? "--" : URLEncrypt.EncryptParameters1(new String[] { "FileName =" + lodgeModel.UPLOADED_RECEIPT_NAME });
                model.DEC_UPLOADED_BILL_NAME = lodgeModel.UPLOADED_BILL_NAME == null ? "--" : lodgeModel.UPLOADED_BILL_NAME;
                model.DEC_UPLOADED_RECEIPT_NAME = lodgeModel.UPLOADED_RECEIPT_NAME == null ? "--" : lodgeModel.UPLOADED_RECEIPT_NAME;

                List<SelectListItem> lstClaim = new List<SelectListItem>();
                lstClaim.Insert(0, new SelectListItem { Text = "Select Claim Type", Value = "-1" });
                lstClaim.Add(new SelectListItem() { Text = "Hotel", Value = "H" });
                lstClaim.Add(new SelectListItem() { Text = "Govt. Guest House", Value = "G" });
                lstClaim.Add(new SelectListItem() { Text = "Self Accomodation", Value = "S" });

                model.LST_TYPE_OF_CLAIM = lstClaim;
                model.ADD_EDIT = 2;

                return PartialView("AddLodgeClaim", model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.EditLodgeDetail()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult UpdateLodgeClaimDetails(FormCollection formCollection)
        {
            try
            {
                ITourClaimBAL objBAL = new TourClaimBAL();
                bool status = false;
                string isValidMsg = String.Empty;
                String message = "Lodge claim details not updated";
                HttpPostedFileBase gBill = Request.Files[0];
                HttpPostedFileBase bill = Request.Files[1];
                HttpPostedFileBase receipt = Request.Files[2];
                int maxSize = 1024 * 1024 * 4;
                int lodgeId = Convert.ToInt32(formCollection["LODGE_CLAIM_ID"]);
                int tourId = Convert.ToInt32(formCollection["TOUR_CLAIM_ID"]);
                string typeOfClaim;

                List<NQM_LODGE_AND_DAILY_CLAIM_DETAILS> modelList = new List<NQM_LODGE_AND_DAILY_CLAIM_DETAILS>();
                NQM_LODGE_AND_DAILY_CLAIM_DETAILS model = new NQM_LODGE_AND_DAILY_CLAIM_DETAILS();

                modelList = dbContext.NQM_LODGE_AND_DAILY_CLAIM_DETAILS.Where(x => x.TOUR_CLAIM_ID == tourId && x.LODGE_CLAIM_ID != lodgeId).ToList();
                model = dbContext.NQM_LODGE_AND_DAILY_CLAIM_DETAILS.Where(x => x.LODGE_CLAIM_ID == lodgeId).FirstOrDefault();
                typeOfClaim = model.TYPE_OF_CLAIM;

                foreach (var item in modelList)
                {
                    if (item.DATE_FROM <= Convert.ToDateTime(formCollection["DATE_FROM"]) && item.DATE_TO > Convert.ToDateTime(formCollection["DATE_FROM"]))
                    {
                        message = "Entry from date " + Convert.ToDateTime(formCollection["DATE_FROM"]).ToShortDateString() + " to date " + item.DATE_TO.ToShortDateString() + " already exist.";
                        return Json(new { success = false, file = false, message });
                    }
                    if (item.DATE_FROM <= Convert.ToDateTime(formCollection["DATE_TO"]) && item.DATE_TO > Convert.ToDateTime(formCollection["DATE_TO"]))
                    {
                        message = "Entry from date " + item.DATE_FROM.ToShortDateString() + " to date " + Convert.ToDateTime(formCollection["DATE_TO"]).ToShortDateString() + " already exist.";
                        return Json(new { success = false, file = false, message });
                    }
                }

                if (typeOfClaim.Equals("H"))
                {
                    if (bill.ContentLength != 0)
                    {
                        if (bill.FileName.Substring(bill.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "pdf"
                            && bill.FileName.Substring(bill.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "jpg"
                            && bill.FileName.Substring(bill.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "jpeg"
                            && bill.FileName.Substring(bill.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "png")
                        {
                            return Json(new { success = false, file = false, message = "Invalid bill file. Please upload pdf, jpg, jpeg or png files." });
                        }

                        if (bill.ContentLength > maxSize)
                        {
                            return Json(new { success = false, file = false, message = "Invalid bill file size. Please upload file upto 4 MB." });
                        }
                    }

                    if (receipt.ContentLength != 0)
                    {
                        if (receipt.FileName.Substring(receipt.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "pdf"
                            && receipt.FileName.Substring(receipt.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "jpg"
                            && receipt.FileName.Substring(receipt.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "jpeg"
                            && receipt.FileName.Substring(receipt.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "png")
                        {
                            return Json(new { success = false, file = false, message = "Invalid e-Receipt file. Please upload pdf, jpg, jpeg or png files." });
                        }

                        if (receipt.ContentLength > maxSize)
                        {
                            return Json(new { success = false, file = false, message = "Invalid e-Receipt file size. Please upload file upto 4 MB." });
                        }
                    }
                }
                else if (typeOfClaim.Equals("G"))
                {
                    if (gBill.ContentLength != 0)
                    {
                        if (gBill.FileName.Substring(gBill.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "pdf"
                            && gBill.FileName.Substring(gBill.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "jpg"
                            && gBill.FileName.Substring(gBill.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "jpeg"
                            && gBill.FileName.Substring(gBill.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "png")
                            return Json(new { success = false, file = false, message = "Invalid bill file. Please upload pdf, jpg, jpeg or png files." });
                        if (gBill.ContentLength > maxSize)
                            return Json(new { success = false, file = false, message = "Invalid bill file size. Please upload file upto 4 MB." });
                    }
                }

                ModelState.Remove("HOTEL_BILL");
                ModelState.Remove("GUEST_HOUSE_BILL");
                ModelState.Remove("HOTEL_RECEIPT");

                if (ModelState.IsValid)
                {
                    status = objBAL.UpdateLodgeClaimDetailsBAL(formCollection, bill, receipt, gBill, out isValidMsg);

                    return Json(new { success = status, message = isValidMsg });
                }

                return Json(new { success = status, message = message });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.UpdateLodgeClaimDetails()");
                return null;
            }
        }

        #endregion


        #region Inspection Of Roads

        public ActionResult InspectionOfRoadsHonorarium(int scheduleCode)
        {
            try
            {
                NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS_DETAILS_MODEL roadInspec = new NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS_DETAILS_MODEL();

                NQM_TOUR_CLAIM_MASTER masterModel = new NQM_TOUR_CLAIM_MASTER();

                masterModel = dbContext.NQM_TOUR_CLAIM_MASTER.Where(x => x.ADMIN_SCHEDULE_CODE == scheduleCode).FirstOrDefault();

                roadInspec.TOUR_CLAIM_ID = masterModel.TOUR_CLAIM_ID;
                roadInspec.ADMIN_SCHEDULE_CODE = masterModel.ADMIN_SCHEDULE_CODE;
                List<SelectListItem> lstWorks = new List<SelectListItem>();
                lstWorks.Insert(0, new SelectListItem { Text = "Select Work", Value = "-1" });
                lstWorks.Add(new SelectListItem() { Text = "Inspection", Value = "I" });
                lstWorks.Add(new SelectListItem() { Text = "Enquiry", Value = "E" });
                lstWorks.Add(new SelectListItem() { Text = "ATR Verification", Value = "A" });
                lstWorks.Add(new SelectListItem() { Text = "Others", Value = "O" });
                roadInspec.LST_WORK = lstWorks;

                roadInspec.lstDateOfInsp = new List<SelectListItem>();
                roadInspec.lstDateOfInsp.Insert(0, new SelectListItem() { Text = "Select Inspection Date", Value = "1" });
                roadInspec.ADD_EDIT = 1;

                List<DateTime> lstDates = new List<DateTime>();
                List<string> newLstDates = new List<string>();

                lstDates = dbContext.QUALITY_QM_OBSERVATION_MASTER.Where(x => x.ADMIN_SCHEDULE_CODE == scheduleCode).Select(y => y.QM_INSPECTION_DATE).ToList();

                foreach (DateTime item in lstDates)
                {
                    string date1 = null;
                    date1 = item.Date.ToShortDateString();
                    newLstDates.Add(date1);
                }

                newLstDates = newLstDates.Distinct().ToList();

                foreach (string item in newLstDates)
                {
                    roadInspec.lstDateOfInsp.Insert(1, new SelectListItem() { Text = item, Value = item });
                }

                decimal amt = dbContext.NQM_TOUR_ALLOWANCE_MASTER.Where(x => x.ALLOWANCE_TYPE.Equals("Inspection") && x.IS_ACTIVE == "Y").Select(y => y.AMOUNT).FirstOrDefault();
                roadInspec.AMOUNT_CLAIMED = amt;

                return PartialView("InspectionOfRoadsHonorarium", roadInspec);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.InspectionOfRoadsHonorarium()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult InsertInspectionHonorarium(FormCollection formCollection)
        {
            try
            {
                ITourClaimBAL objBAL = new TourClaimBAL();
                bool status = false;
                string isValidMsg = String.Empty;
                String message = "Honorarium for inspection of roads not saved";

                decimal amt = dbContext.NQM_TOUR_ALLOWANCE_MASTER.Where(x => x.ALLOWANCE_TYPE.Equals("Inspection") && x.IS_ACTIVE == "Y").Select(y => y.AMOUNT).FirstOrDefault();

                if (Convert.ToDecimal(formCollection["AMOUNT_CLAIMED"]) > amt)
                    return Json(new { success = false, message = "Amount claimed cannot be greater than " + amt + "." });

                if (ModelState.IsValid)
                {
                    status = objBAL.InsertInspectionHonorariumBAL(formCollection, out isValidMsg);

                    return Json(new { success = status, message = isValidMsg });
                }

                return Json(new { success = status, message = message });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.InsertInspectionHonorarium()");
                return null;
            }
        }

        [Audit]
        public ActionResult GetInspectionHonorariumList(int scheduleCode, int? page, int? rows, string sidx, string sord)
        {
            try
            {
                long totalRecords = 0;
                ITourClaimBAL objBAL = new TourClaimBAL();

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.GetInspectionHonorariumListBAL(scheduleCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.GetInspectionHonorariumList()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult DeleteInspectionDetails(int inspectionId)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    bool status = false;
                    String message = "Details not deleted";
                    DateTime dateOfInspec;
                    NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS model = new NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS();
                    NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS inspModel = new NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS();
                    NQM_TOUR_HONORARIUM_MEETING_WITH_PIU meetingModel = new NQM_TOUR_HONORARIUM_MEETING_WITH_PIU();

                    model = dbContext.NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS.Where(x => x.HONORARIUM_INSPECTION_ID == inspectionId).FirstOrDefault();
                    dateOfInspec = model.DATE_OF_INSPECTION;
                    decimal amt = dbContext.NQM_TOUR_ALLOWANCE_MASTER.Where(x => x.ALLOWANCE_TYPE.Equals("Meeting") && x.IS_ACTIVE == "Y").Select(y => y.AMOUNT).FirstOrDefault();
                    decimal inspAmt = dbContext.NQM_TOUR_ALLOWANCE_MASTER.Where(x => x.ALLOWANCE_TYPE.Equals("Inspection") && x.IS_ACTIVE == "Y").Select(y => y.AMOUNT).FirstOrDefault();

                    if (dbContext.NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS.Where(x => x.TOUR_CLAIM_ID == model.TOUR_CLAIM_ID && x.DATE_OF_INSPECTION == dateOfInspec && x.HONORARIUM_INSPECTION_ID != inspectionId).Any())
                    {
                        inspModel = dbContext.NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS.Where(x => x.TOUR_CLAIM_ID == model.TOUR_CLAIM_ID && x.DATE_OF_INSPECTION == dateOfInspec && x.HONORARIUM_INSPECTION_ID != inspectionId).FirstOrDefault();
                        inspModel.AMOUNT_CLAIMED = inspAmt;
                        dbContext.Entry(inspModel).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }

                    if (!(dbContext.NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS.Where(x => x.TOUR_CLAIM_ID == model.TOUR_CLAIM_ID && x.DATE_OF_INSPECTION == dateOfInspec && x.HONORARIUM_INSPECTION_ID != inspectionId).Any()) && dbContext.NQM_TOUR_HONORARIUM_MEETING_WITH_PIU.Where(x => x.TOUR_CLAIM_ID == model.TOUR_CLAIM_ID && x.DATE_OF_MEETING == dateOfInspec).Any())
                    {
                        meetingModel = dbContext.NQM_TOUR_HONORARIUM_MEETING_WITH_PIU.Where(x => x.TOUR_CLAIM_ID == model.TOUR_CLAIM_ID && x.DATE_OF_MEETING == dateOfInspec).FirstOrDefault();
                        meetingModel.AMOUNT_CLAIMED = amt;
                        dbContext.Entry(meetingModel).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }

                    dbContext.NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS.Remove(dbContext.NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS.Where(x => x.HONORARIUM_INSPECTION_ID == inspectionId).FirstOrDefault());
                    dbContext.SaveChanges();

                    status = true;
                    message = "Details deleted successfully";

                    scope.Complete();

                    return Json(new { success = status, message = message });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.DeleteInspectionDetails()");
                return Json(new { success = false, message = "Details not deleted." });
            }
        }

        public ActionResult EditInspectionDetails(int inspectionId)
        {
            try
            {
                NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS_DETAILS_MODEL roadInspec = new NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS_DETAILS_MODEL();
                NQM_TOUR_CLAIM_MASTER masterModel = new NQM_TOUR_CLAIM_MASTER();
                NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS inspModel = new NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS();

                inspModel = dbContext.NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS.Where(x => x.HONORARIUM_INSPECTION_ID == inspectionId).FirstOrDefault();
                masterModel = dbContext.NQM_TOUR_CLAIM_MASTER.Where(x => x.TOUR_CLAIM_ID == inspModel.TOUR_CLAIM_ID).FirstOrDefault();

                List<SelectListItem> lstWorks = new List<SelectListItem>();
                lstWorks.Insert(0, new SelectListItem { Text = "Select Work", Value = "-1" });
                lstWorks.Add(new SelectListItem() { Text = "Inspection", Value = "I" });
                lstWorks.Add(new SelectListItem() { Text = "Enquiry", Value = "E" });
                lstWorks.Add(new SelectListItem() { Text = "ATR Verification", Value = "A" });
                lstWorks.Add(new SelectListItem() { Text = "Others", Value = "O" });

                roadInspec.HONORARIUM_INSPECTION_ID = inspModel.HONORARIUM_INSPECTION_ID;
                roadInspec.TOUR_CLAIM_ID = masterModel.TOUR_CLAIM_ID;
                roadInspec.ADMIN_SCHEDULE_CODE = masterModel.ADMIN_SCHEDULE_CODE;
                roadInspec.DATE_OF_INSPECTION = inspModel.DATE_OF_INSPECTION.ToShortDateString();
                roadInspec.TYPE_OF_WORK = inspModel.TYPE_OF_WORK;
                roadInspec.TYPE_OF_WORK_OTHER = inspModel.TYPE_OF_WORK == "O" ? inspModel.TYPE_OF_WORK_OTHER : "";
                roadInspec.ADD_EDIT = 2;
                roadInspec.LST_WORK = lstWorks;

                decimal amt = dbContext.NQM_TOUR_ALLOWANCE_MASTER.Where(x => x.ALLOWANCE_TYPE.Equals("Inspection") && x.IS_ACTIVE == "Y").Select(y => y.AMOUNT).FirstOrDefault();
                roadInspec.AMOUNT_CLAIMED = amt;

                return View("InspectionOfRoadsHonorarium", roadInspec);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.EditInspectionDetails()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult UpdateInspectionHonorarium(FormCollection formCollection)
        {
            try
            {
                ITourClaimBAL objBAL = new TourClaimBAL();
                bool status = false;
                string isValidMsg = String.Empty;
                String message = "Honorarium for inspection of roads not updated";

                if (ModelState.IsValid)
                {
                    status = objBAL.UpdateInspectionHonorariumBAL(formCollection, out isValidMsg);

                    return Json(new { success = status, message = isValidMsg });
                }

                return Json(new { success = status, message = message });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.UpdateInspectionHonorarium()");
                return null;
            }
        }

        #endregion


        #region Meeting Wih PIU

        public ActionResult MeetingWihPIUHonorarium(int scheduleCode)
        {
            try
            {
                NQM_TOUR_HONORARIUM_MEETING_WITH_PIU_DETAILS_MODEL meetingModel = new NQM_TOUR_HONORARIUM_MEETING_WITH_PIU_DETAILS_MODEL();
                NQM_TOUR_CLAIM_MASTER masterModel = new NQM_TOUR_CLAIM_MASTER();
                QUALITY_QM_SCHEDULE scheduleModel = new QUALITY_QM_SCHEDULE();
                NQM_TOUR_DISTRICT_DETAILS_MODEL model = new NQM_TOUR_DISTRICT_DETAILS_MODEL();

                masterModel = dbContext.NQM_TOUR_CLAIM_MASTER.Where(x => x.ADMIN_SCHEDULE_CODE == scheduleCode).FirstOrDefault();
                scheduleModel = dbContext.QUALITY_QM_SCHEDULE.Where(x => x.ADMIN_SCHEDULE_CODE == scheduleCode).FirstOrDefault();

                int tourClaimId = masterModel.TOUR_CLAIM_ID;
                string distName = string.Empty;

                List<int> lst = dbContext.NQM_TOUR_DISTRICT_DETAILS.Where(x => x.TOUR_CLAIM_ID == tourClaimId).Select(y => y.DISTRICT).ToList();
                meetingModel.lstDistricts = new List<SelectListItem>();
                meetingModel.lstDistricts.Insert(0, (new SelectListItem { Text = "Select District", Value = "0", Selected = true }));

                foreach (var item in lst.Distinct())
                {
                    distName = dbContext.MASTER_DISTRICT.Where(x => x.MAST_DISTRICT_CODE == item).Select(y => y.MAST_DISTRICT_NAME).FirstOrDefault();
                    meetingModel.lstDistricts.Insert(1, (new SelectListItem { Text = distName, Value = item.ToString() }));
                }

                meetingModel.TOUR_CLAIM_ID = masterModel.TOUR_CLAIM_ID;
                meetingModel.ADMIN_SCHEDULE_CODE = masterModel.ADMIN_SCHEDULE_CODE;

                meetingModel.STATE_CODE = scheduleModel.MAST_STATE_CODE;

                decimal amt = dbContext.NQM_TOUR_ALLOWANCE_MASTER.Where(x => x.ALLOWANCE_TYPE.Equals("Meeting") && x.IS_ACTIVE == "Y").Select(y => y.AMOUNT).FirstOrDefault();
                meetingModel.AMOUNT_CLAIMED = amt;
                meetingModel.ADD_EDIT = 1;

                return PartialView("MeetingWihPIUHonorarium", meetingModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.MeetingWihPIUHonorarium()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult InsertMeetingHonorarium(FormCollection formCollection)
        {
            try
            {
                ITourClaimBAL objBAL = new TourClaimBAL();
                bool status = false;
                string isValidMsg = String.Empty;
                String message = "Honorarium for Meeting with PIU not saved";
                HttpPostedFileBase postedBgFile = Request.Files[1];     // Uploaded file
                HttpPostedFileBase postedBgFile1 = Request.Files[2];    // Photograph
                HttpPostedFileBase postedBgFile2 = Request.Files[0];    // attendance Sheet
                int maxSize = 1024 * 1024 * 4;

                int tourClaimId = Convert.ToInt32(formCollection["TOUR_CLAIM_ID"]);
                int placeOfmeeting = Convert.ToInt32(formCollection["DISTRICT_CODE"]);

                if (dbContext.NQM_TOUR_HONORARIUM_MEETING_WITH_PIU.Where(x => x.TOUR_CLAIM_ID == tourClaimId && x.DISTRICT == placeOfmeeting).Any())
                    return Json(new { success = false, file = false, message = "District already added !!" });

                if (postedBgFile.ContentLength == 0)
                {
                    return Json(new { success = false, file = false, message = "No file selected for meeting details. Please select file" });
                }
                else
                {
                    if (postedBgFile.FileName.Substring(postedBgFile.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "pdf"
                        && postedBgFile.FileName.Substring(postedBgFile.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "jpg"
                        && postedBgFile.FileName.Substring(postedBgFile.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "jpeg"
                        && postedBgFile.FileName.Substring(postedBgFile.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "png")
                    {
                        return Json(new { success = false, file = false, message = "Invalid file for meeting details. Please upload pdf, jpg, jpeg or png files." });
                    }

                    if (postedBgFile.ContentLength > maxSize)
                    {
                        return Json(new { success = false, file = false, message = "Invalid file size for meeting details. Please upload file upto 4 MB." });
                    }
                }

                if (postedBgFile1.ContentLength == 0)
                {
                    return Json(new { success = false, file = false, message = "No photograph selected. Please select photograph" });
                }
                else
                {
                    if (postedBgFile1.FileName.Substring(postedBgFile1.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "jpg"
                        && postedBgFile1.FileName.Substring(postedBgFile1.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "jpeg"
                        && postedBgFile1.FileName.Substring(postedBgFile1.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "png")
                    {
                        return Json(new { success = false, file = false, message = "Invalid photograph. Please upload jpg, jpeg or png files." });
                    }

                    if (postedBgFile1.ContentLength > maxSize)
                    {
                        return Json(new { success = false, file = false, message = "Invalid photograph size. Please upload file upto 4 MB." });
                    }
                }

                if (postedBgFile2.ContentLength == 0)
                {
                    return Json(new { success = false, file = false, message = "No attendance sheet selected. Please select attendance sheet" });
                }
                else
                {
                    if (postedBgFile2.FileName.Substring(postedBgFile2.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "pdf"
                        && postedBgFile2.FileName.Substring(postedBgFile2.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "jpg"
                        && postedBgFile2.FileName.Substring(postedBgFile2.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "jpeg"
                        && postedBgFile2.FileName.Substring(postedBgFile2.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "png")
                    {
                        return Json(new { success = false, file = false, message = "Invalid attendance sheet. Please upload pdf, jpg, jpeg or png files." });
                    }

                    if (postedBgFile2.ContentLength > maxSize)
                    {
                        return Json(new { success = false, file = false, message = "Invalid attendance sheet size. Please upload file upto 4 MB." });
                    }
                }

                if (ModelState.IsValid)
                {
                    status = objBAL.InsertMeetingHonorariumBAL(formCollection, postedBgFile, postedBgFile1, postedBgFile2, out isValidMsg);

                    return Json(new { success = status, message = isValidMsg });
                }

                return Json(new { success = status, message = message });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.InsertMeetingHonorarium()");
                return null;
            }
        }

        [Audit]
        public ActionResult GetMeetingHonorariumList(int scheduleCode, int? page, int? rows, string sidx, string sord)
        {
            try
            {
                long totalRecords = 0;
                ITourClaimBAL objBAL = new TourClaimBAL();

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.GetMeetingHonorariumListBAL(scheduleCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.GetMeetingHonorariumList()");
                return null;
            }
        }

        public FileResult ViewUploadedMeetingDetails(string id1)
        {
            try
            {
                String FileName = String.Empty;
                string[] encParam = id1.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encParam[0], encParam[1], encParam[2] });
                if (decryptedParameters.Count > 0)
                {
                    FileName = decryptedParameters["FileName"];
                }

                string path = ConfigurationManager.AppSettings["NQM_TOUR_CLAIM_PATH"].ToString();
                var fullPath = Path.Combine(path, "MeetingWithPIU", FileName.Split('_')[0], FileName);

                byte[] FileBytes = System.IO.File.ReadAllBytes(fullPath);

                string type = string.Empty;
                type = FileName.Substring(FileName.LastIndexOf('.') + 1).Trim().ToLower();
                if (type == "pdf")
                {
                    return File(FileBytes, "application/pdf");
                }
                else
                {
                    return File(FileBytes, "image/png");
                }

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.ViewUploadedMeetingDetails()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult DeleteMeetingDetails(int meetingId)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    bool status = false;
                    String message = "Details not deleted";
                    String FileName = String.Empty;
                    String PhotoName = String.Empty;
                    String AttendanceSheetName = String.Empty;
                    String FilePath = String.Empty;
                    String PhotoPath = String.Empty;
                    String AttendanceSheetPath = String.Empty;

                    NQM_TOUR_HONORARIUM_MEETING_WITH_PIU model = new NQM_TOUR_HONORARIUM_MEETING_WITH_PIU();
                    List<NQM_TOUR_HONORARIUM_MEETING_WITH_PIU> meetingList = new List<NQM_TOUR_HONORARIUM_MEETING_WITH_PIU>();

                    model = dbContext.NQM_TOUR_HONORARIUM_MEETING_WITH_PIU.Where(x => x.HONORARIUM_MEETING_ID == meetingId).FirstOrDefault();
                    meetingList = dbContext.NQM_TOUR_HONORARIUM_MEETING_WITH_PIU.Where(x => x.TOUR_CLAIM_ID == model.TOUR_CLAIM_ID).ToList();
                    decimal amt = dbContext.NQM_TOUR_ALLOWANCE_MASTER.Where(x => x.ALLOWANCE_TYPE.Equals("Meeting") && x.IS_ACTIVE == "Y").Select(y => y.AMOUNT).FirstOrDefault();

                    foreach (var item in meetingList)
                    {
                        if (model.DATE_OF_MEETING == item.DATE_OF_MEETING)
                        {
                            if (!dbContext.NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS.Where(x => x.DATE_OF_INSPECTION == item.DATE_OF_MEETING).Any())
                            {
                                if (!dbContext.NQM_TOUR_HONORARIUM_MEETING_WITH_PIU.Where(x => x.DATE_OF_MEETING == model.DATE_OF_MEETING && x.HONORARIUM_MEETING_ID != model.HONORARIUM_MEETING_ID && x.AMOUNT_CLAIMED == amt).Any())
                                {
                                    item.AMOUNT_CLAIMED = amt;
                                    dbContext.Entry(item).State = System.Data.Entity.EntityState.Modified;
                                    dbContext.SaveChanges();
                                }
                            }
                        }
                    }

                    FileName = model.UPLOADED_FILE_NAME;
                    PhotoName = model.PHOTO_NAME;
                    AttendanceSheetName = model.ATTENDANCE_SHEET_NAME;
                    FilePath = model.UPLOADED_FILE_PATH;
                    PhotoPath = model.PHOTO_PATH;
                    AttendanceSheetPath = model.ATTENDANCE_SHEET_PATH;

                    dbContext.NQM_TOUR_HONORARIUM_MEETING_WITH_PIU.Remove(dbContext.NQM_TOUR_HONORARIUM_MEETING_WITH_PIU.Where(x => x.HONORARIUM_MEETING_ID == meetingId).FirstOrDefault());
                    dbContext.SaveChanges();

                    //string path = ConfigurationManager.AppSettings["NQM_TOUR_CLAIM_PATH"].ToString();
                    var fullPath1 = Path.Combine(FilePath, FileName);
                    var fullPath2 = Path.Combine(PhotoPath, PhotoName);
                    var fullPath3 = Path.Combine(AttendanceSheetPath, AttendanceSheetName);

                    FileInfo file1 = new FileInfo(fullPath1);
                    FileInfo file2 = new FileInfo(fullPath2);
                    FileInfo file3 = new FileInfo(fullPath3);

                    if (file1.Exists)
                    {
                        file1.Delete();
                    }
                    if (file2.Exists)
                    {
                        file2.Delete();
                    }
                    if (file3.Exists)
                    {
                        file3.Delete();
                    }

                    status = true;
                    message = "Details deleted successfully";

                    scope.Complete();

                    return Json(new { success = status, message = message });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.DeleteMeetingDetails()");
                return Json(new { success = false, message = "Details not deleted." });
            }
        }

        public ActionResult EditMeetingDetails(int meetingId)
        {
            try
            {
                NQM_TOUR_HONORARIUM_MEETING_WITH_PIU_DETAILS_MODEL model = new NQM_TOUR_HONORARIUM_MEETING_WITH_PIU_DETAILS_MODEL();
                NQM_TOUR_CLAIM_MASTER masterModel = new NQM_TOUR_CLAIM_MASTER();
                NQM_TOUR_HONORARIUM_MEETING_WITH_PIU meetingModel = new NQM_TOUR_HONORARIUM_MEETING_WITH_PIU();
                string distName = string.Empty;

                meetingModel = dbContext.NQM_TOUR_HONORARIUM_MEETING_WITH_PIU.Where(x => x.HONORARIUM_MEETING_ID == meetingId).FirstOrDefault();
                masterModel = dbContext.NQM_TOUR_CLAIM_MASTER.Where(x => x.TOUR_CLAIM_ID == meetingModel.TOUR_CLAIM_ID).FirstOrDefault();

                List<int> lst = dbContext.NQM_TOUR_DISTRICT_DETAILS.Where(x => x.TOUR_CLAIM_ID == meetingModel.TOUR_CLAIM_ID).Select(y => y.DISTRICT).ToList();
                model.lstDistricts = new List<SelectListItem>();
                model.lstDistricts.Insert(0, (new SelectListItem { Text = "Select District", Value = "0", Selected = true }));

                foreach (var item in lst.Distinct())
                {
                    distName = dbContext.MASTER_DISTRICT.Where(x => x.MAST_DISTRICT_CODE == item).Select(y => y.MAST_DISTRICT_NAME).FirstOrDefault();
                    model.lstDistricts.Insert(1, (new SelectListItem { Text = distName, Value = item.ToString() }));
                }

                model.HONORARIUM_MEETING_ID = meetingId;
                model.TOUR_CLAIM_ID = meetingModel.TOUR_CLAIM_ID;
                model.ADMIN_SCHEDULE_CODE = masterModel.ADMIN_SCHEDULE_CODE;
                model.DATE_OF_MEETING = meetingModel.DATE_OF_MEETING;
                model.DISTRICT_CODE = meetingModel.DISTRICT;
                model.UPLOADED_ATTENDANCE_SHEET_NAME = meetingModel.ATTENDANCE_SHEET_NAME == null ? "--" : URLEncrypt.EncryptParameters1(new String[] { "FileName =" + meetingModel.ATTENDANCE_SHEET_NAME });
                model.UPLOADED_FILE_NAME = meetingModel.UPLOADED_FILE_NAME == null ? "--" : URLEncrypt.EncryptParameters1(new String[] { "FileName =" + meetingModel.UPLOADED_FILE_NAME });
                model.UPLOADED_PHOTO_NAME = meetingModel.PHOTO_NAME == null ? "--" : URLEncrypt.EncryptParameters1(new String[] { "FileName =" + meetingModel.PHOTO_NAME });
                model.DEC_UPLOADED_ATTENDANCE_SHEET_NAME = meetingModel.ATTENDANCE_SHEET_NAME == null ? "--" : meetingModel.ATTENDANCE_SHEET_NAME;
                model.DEC_UPLOADED_FILE_NAME = meetingModel.UPLOADED_FILE_NAME == null ? "--" : meetingModel.UPLOADED_FILE_NAME;
                model.DEC_UPLOADED_PHOTO_NAME = meetingModel.PHOTO_NAME == null ? "--" : meetingModel.PHOTO_NAME;

                model.ADD_EDIT = 2;

                decimal amt = dbContext.NQM_TOUR_ALLOWANCE_MASTER.Where(x => x.ALLOWANCE_TYPE.Equals("Meeting") && x.IS_ACTIVE == "Y").Select(y => y.AMOUNT).FirstOrDefault();
                model.AMOUNT_CLAIMED = amt;

                return View("MeetingWihPIUHonorarium", model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.EditMeetingDetails()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult UpdateMeetingHonorarium(FormCollection formCollection)
        {
            try
            {
                ITourClaimBAL objBAL = new TourClaimBAL();
                bool status = false;
                string isValidMsg = String.Empty;
                String message = "Honorarium for Meeting with PIU not added";
                HttpPostedFileBase postedBgFile = Request.Files[1];   // Meeting Details
                HttpPostedFileBase postedBgFile1 = Request.Files[2];  // Photograph
                HttpPostedFileBase postedBgFile2 = Request.Files[0];  // Attendance Sheet
                int maxSize = 1024 * 1024 * 4;

                int tourClaimId = Convert.ToInt32(formCollection["TOUR_CLAIM_ID"]);
                int placeOfmeeting = Convert.ToInt32(formCollection["DISTRICT_CODE"]);
                int meetingId = Convert.ToInt32(formCollection["HONORARIUM_MEETING_ID"]);

                if (dbContext.NQM_TOUR_HONORARIUM_MEETING_WITH_PIU.Where(x => x.TOUR_CLAIM_ID == tourClaimId && x.DISTRICT == placeOfmeeting && x.HONORARIUM_MEETING_ID != meetingId).Any())
                    return Json(new { success = false, file = false, message = "District already added !!" });

                if (postedBgFile.ContentLength != 0)
                {
                    if (postedBgFile.FileName.Substring(postedBgFile.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "pdf"
                        && postedBgFile.FileName.Substring(postedBgFile.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "jpg"
                        && postedBgFile.FileName.Substring(postedBgFile.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "jpeg"
                        && postedBgFile.FileName.Substring(postedBgFile.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "png")
                    {
                        return Json(new { success = false, file = false, message = "Invalid file for meeting details. Please upload pdf, jpg, jpeg or png files." });
                    }

                    if (postedBgFile.ContentLength > maxSize)
                    {
                        return Json(new { success = false, file = false, message = "Invalid file size for meeting details. Please upload file upto 4 MB." });
                    }
                }

                if (postedBgFile1.ContentLength != 0)
                {
                    if (postedBgFile1.FileName.Substring(postedBgFile1.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "jpg"
                        && postedBgFile1.FileName.Substring(postedBgFile1.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "jpeg"
                        && postedBgFile1.FileName.Substring(postedBgFile1.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "png")
                    {
                        return Json(new { success = false, file = false, message = "Invalid file for photograph. Please upload jpg, jpeg or png files." });
                    }

                    if (postedBgFile1.ContentLength > maxSize)
                    {
                        return Json(new { success = false, file = false, message = "Invalid file size photograph. Please upload file upto 4 MB." });
                    }
                }

                if (postedBgFile2.ContentLength != 0)
                {
                    if (postedBgFile2.FileName.Substring(postedBgFile2.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "pdf"
                        && postedBgFile2.FileName.Substring(postedBgFile2.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "jpg"
                        && postedBgFile2.FileName.Substring(postedBgFile2.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "jpeg"
                        && postedBgFile2.FileName.Substring(postedBgFile2.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "png")
                    {
                        return Json(new { success = false, file = false, message = "Invalid attendance sheet. Please upload pdf, jpg, jpeg or png files." });
                    }

                    if (postedBgFile2.ContentLength > maxSize)
                    {
                        return Json(new { success = false, file = false, message = "Invalid attendance sheet size. Please upload file upto 4 MB." });
                    }
                }

                ModelState.Remove("ATTENDANCE_SHEET");
                ModelState.Remove("MEETING_FILE");
                ModelState.Remove("PHOTO_FILE");

                if (ModelState.IsValid)
                {
                    status = objBAL.UpdateMeetingHonorariumBAL(formCollection, postedBgFile, postedBgFile1, postedBgFile2, out isValidMsg);

                    return Json(new { success = status, message = isValidMsg });
                }

                return Json(new { success = status, message = message });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.UpdateMeetingHonorarium()");
                return null;
            }
        }

        #endregion


        #region Miscellaneous

        public ActionResult AddMiscellaneousClaim(int scheduleCode)
        {
            try
            {
                NQM_TOUR_MISCELLANEOUS_MODEL misModel = new NQM_TOUR_MISCELLANEOUS_MODEL();
                NQM_TOUR_CLAIM_MASTER masterModel = new NQM_TOUR_CLAIM_MASTER();
                QUALITY_QM_SCHEDULE scheduleModel = new QUALITY_QM_SCHEDULE();

                masterModel = dbContext.NQM_TOUR_CLAIM_MASTER.Where(x => x.ADMIN_SCHEDULE_CODE == scheduleCode).FirstOrDefault();
                scheduleModel = dbContext.QUALITY_QM_SCHEDULE.Where(x => x.ADMIN_SCHEDULE_CODE == scheduleCode).FirstOrDefault();

                misModel.TOUR_CLAIM_ID = masterModel.TOUR_CLAIM_ID;
                misModel.ADMIN_SCHEDULE_CODE = masterModel.ADMIN_SCHEDULE_CODE;
                misModel.ADD_EDIT = 1;

                return PartialView("AddMiscellaneousClaim", misModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.AddMiscellaneousClaim()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult InsertMiscellaneousClaimDetails(FormCollection formCollection)
        {
            try
            {
                ITourClaimBAL objBAL = new TourClaimBAL();
                bool status = false;
                string isValidMsg = String.Empty;
                String message = "Miscellaneous Claim Details not saved";
                HttpPostedFileBase postedBgFile = Request.Files[0];

                if (postedBgFile.ContentLength > 0)
                {
                    int maxSize = 1024 * 1024 * 4;

                    if (postedBgFile.FileName.Substring(postedBgFile.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "pdf"
                        && postedBgFile.FileName.Substring(postedBgFile.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "jpg"
                        && postedBgFile.FileName.Substring(postedBgFile.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "jpeg"
                        && postedBgFile.FileName.Substring(postedBgFile.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "png")
                    {
                        return Json(new { success = false, file = false, message = "Invalid file. Please upload pdf, jpg, jpeg or png files." });
                    }

                    if (postedBgFile.ContentLength > maxSize)
                    {
                        return Json(new { success = false, file = false, message = "Invalid file size. Please upload file upto 4 MB." });
                    }
                }

                if (ModelState.IsValid)
                {
                    status = objBAL.InsertMiscellaneousClaimDetailsBAL(formCollection, postedBgFile, out isValidMsg);

                    return Json(new { success = status, message = isValidMsg });
                }

                return Json(new { success = status, message = message });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.InsertMiscellaneousClaimDetails()");
                return null;
            }
        }

        [Audit]
        public ActionResult GetMiscellaneousClaimList(int scheduleCode, int? page, int? rows, string sidx, string sord)
        {
            try
            {
                long totalRecords = 0;
                ITourClaimBAL objBAL = new TourClaimBAL();

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.GetMiscellaneousClaimListBAL(scheduleCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.GetMiscellaneousClaimList()");
                return null;
            }
        }

        public FileResult ViewUploadedMisDetails(string id1)
        {
            try
            {
                String FileName = String.Empty;
                string[] encParam = id1.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encParam[0], encParam[1], encParam[2] });
                if (decryptedParameters.Count > 0)
                {
                    FileName = decryptedParameters["FileName"];
                }

                string path = ConfigurationManager.AppSettings["NQM_TOUR_CLAIM_PATH"].ToString();
                var fullPath = Path.Combine(path, "MisClaim", FileName.Split('_')[0], FileName);
                byte[] FileBytes = System.IO.File.ReadAllBytes(fullPath);

                string type = string.Empty;
                type = FileName.Substring(FileName.LastIndexOf('.') + 1).Trim().ToLower();
                if (type == "pdf")
                {
                    return File(FileBytes, "application/pdf");
                }
                else
                {
                    return File(FileBytes, "image/png");
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.ViewUploadedMisDetails()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult DeleteMiscellaneousDetails(int miscellaneousId)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    bool status = false;
                    String message = "Details not deleted";
                    String FileName = String.Empty;
                    String FilePath = String.Empty;

                    NQM_TOUR_MISCELLANEOUS model = new NQM_TOUR_MISCELLANEOUS();
                    model = dbContext.NQM_TOUR_MISCELLANEOUS.Where(x => x.MISCELLANEOUS_ID == miscellaneousId).FirstOrDefault();

                    FileName = model.UPLOADED_FILE_NAME;
                    FilePath = model.UPLOADED_FILE_PATH;

                    dbContext.NQM_TOUR_MISCELLANEOUS.Remove(dbContext.NQM_TOUR_MISCELLANEOUS.Where(x => x.MISCELLANEOUS_ID == miscellaneousId).FirstOrDefault());
                    dbContext.SaveChanges();

                    if (FileName != null)
                    {
                        //string path = ConfigurationManager.AppSettings["NQM_TOUR_CLAIM_PATH"].ToString();
                        var fullPath = Path.Combine(FilePath, FileName);

                        FileInfo file = new FileInfo(fullPath);
                        if (file.Exists)
                        {
                            file.Delete();
                        }
                    }

                    status = true;
                    message = "Details deleted successfully";

                    scope.Complete();

                    return Json(new { success = status, message = message });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.DeleteMiscellaneousDetails()");
                return Json(new { success = false, message = "Details not deleted." });
            }
        }

        public ActionResult EditMiscellaneousDetails(int miscellaneousId)
        {
            try
            {
                NQM_TOUR_MISCELLANEOUS_MODEL model = new NQM_TOUR_MISCELLANEOUS_MODEL();
                NQM_TOUR_CLAIM_MASTER masterModel = new NQM_TOUR_CLAIM_MASTER();
                QUALITY_QM_SCHEDULE scheduleModel = new QUALITY_QM_SCHEDULE();
                NQM_TOUR_MISCELLANEOUS misModel = new NQM_TOUR_MISCELLANEOUS();

                misModel = dbContext.NQM_TOUR_MISCELLANEOUS.Where(x => x.MISCELLANEOUS_ID == miscellaneousId).FirstOrDefault();
                masterModel = dbContext.NQM_TOUR_CLAIM_MASTER.Where(x => x.TOUR_CLAIM_ID == misModel.TOUR_CLAIM_ID).FirstOrDefault();
                scheduleModel = dbContext.QUALITY_QM_SCHEDULE.Where(x => x.ADMIN_SCHEDULE_CODE == masterModel.ADMIN_SCHEDULE_CODE).FirstOrDefault();

                model.MISCELLANEOUS_ID = miscellaneousId;
                model.TOUR_CLAIM_ID = masterModel.TOUR_CLAIM_ID;
                model.ADMIN_SCHEDULE_CODE = masterModel.ADMIN_SCHEDULE_CODE;
                model.DATE = misModel.DATE.ToShortDateString();
                model.DESCRIPTION = misModel.DESCRIPTION;
                model.AMOUNT_CLAIMED = misModel.AMOUNT_CLAIMED;
                model.UPLOADED_FILE_NAME = misModel.UPLOADED_FILE_NAME == null ? "--" : URLEncrypt.EncryptParameters1(new String[] { "FileName =" + misModel.UPLOADED_FILE_NAME });
                model.DEC_UPLOADED_FILE_NAME = misModel.UPLOADED_FILE_NAME == null ? "" : misModel.UPLOADED_FILE_NAME;
                model.ADD_EDIT = 2;

                return View("AddMiscellaneousClaim", model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.EditMiscellaneousDetails()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult DeleteUploadedMisFile(int miscellaneousId)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    bool status = false;
                    String message = "File not deleted";
                    String FileName = String.Empty;
                    String FilePath = String.Empty;

                    NQM_TOUR_MISCELLANEOUS model = new NQM_TOUR_MISCELLANEOUS();
                    model = dbContext.NQM_TOUR_MISCELLANEOUS.Where(x => x.MISCELLANEOUS_ID == miscellaneousId).FirstOrDefault();

                    FileName = model.UPLOADED_FILE_NAME;
                    FilePath = model.UPLOADED_FILE_PATH;

                    model.UPLOADED_FILE_NAME = null;
                    model.UPLOADED_FILE_PATH = null;

                    dbContext.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    if (FileName != null)
                    {
                        //string path = ConfigurationManager.AppSettings["NQM_TOUR_CLAIM_PATH"].ToString();
                        var fullPath = Path.Combine(FilePath, FileName);

                        FileInfo file = new FileInfo(fullPath);
                        if (file.Exists)
                        {
                            file.Delete();
                        }
                    }

                    status = true;
                    message = "File deleted successfully";

                    scope.Complete();

                    return Json(new { success = status, message = message });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.DeleteUploadedMisFile()");
                return Json(new { success = false, message = "File not deleted." });
            }
        }

        [HttpPost]
        public ActionResult UpdateMiscellaneousClaimDetails(FormCollection formCollection)
        {
            try
            {
                ITourClaimBAL objBAL = new TourClaimBAL();
                bool status = false;
                string isValidMsg = String.Empty;
                String message = "Miscellaneous Claim Details not updated";
                HttpPostedFileBase postedBgFile = Request.Files[0];

                if (postedBgFile.ContentLength > 0)
                {
                    int maxSize = 1024 * 1024 * 4;

                    if (postedBgFile.FileName.Substring(postedBgFile.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "pdf"
                        && postedBgFile.FileName.Substring(postedBgFile.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "jpg"
                        && postedBgFile.FileName.Substring(postedBgFile.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "jpeg"
                        && postedBgFile.FileName.Substring(postedBgFile.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "png")
                    {
                        return Json(new { success = false, file = false, message = "Invalid file. Please upload pdf, jpg, jpeg or png files." });
                    }

                    if (postedBgFile.ContentLength > maxSize)
                    {
                        return Json(new { success = false, file = false, message = "Invalid file size. Please upload file upto 4 MB." });
                    }
                }

                if (ModelState.IsValid)
                {
                    status = objBAL.UpdateMiscellaneousClaimDetailsBAL(formCollection, postedBgFile, out isValidMsg);

                    return Json(new { success = status, message = isValidMsg });
                }

                return Json(new { success = status, message = message });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.UpdateMiscellaneousClaimDetails()");
                return null;
            }
        }

        #endregion


        #region Permissions

        public ActionResult AddPermissionClaim(int scheduleCode)
        {
            try
            {
                NQM_TOUR_PERMISSION_MODEL perModel = new NQM_TOUR_PERMISSION_MODEL();
                NQM_TOUR_CLAIM_MASTER masterModel = new NQM_TOUR_CLAIM_MASTER();
                QUALITY_QM_SCHEDULE scheduleModel = new QUALITY_QM_SCHEDULE();

                masterModel = dbContext.NQM_TOUR_CLAIM_MASTER.Where(x => x.ADMIN_SCHEDULE_CODE == scheduleCode).FirstOrDefault();
                scheduleModel = dbContext.QUALITY_QM_SCHEDULE.Where(x => x.ADMIN_SCHEDULE_CODE == scheduleCode).FirstOrDefault();

                perModel.TOUR_CLAIM_ID = masterModel.TOUR_CLAIM_ID;
                perModel.ADMIN_SCHEDULE_CODE = masterModel.ADMIN_SCHEDULE_CODE;
                perModel.ADD_EDIT = 1;

                return PartialView("AddPermissionClaim", perModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.AddPermissionClaim()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult InsertPermissionClaimDetails(FormCollection formCollection)
        {
            try
            {
                ITourClaimBAL objBAL = new TourClaimBAL();
                bool status = false;
                string isValidMsg = String.Empty;
                String message = "Permission Details not saved";
                HttpPostedFileBase postedBgFile = Request.Files[0];

                if (postedBgFile.ContentLength > 0)
                {
                    int maxSize = 1024 * 1024 * 4;

                    if (postedBgFile.FileName.Substring(postedBgFile.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "pdf"
                        && postedBgFile.FileName.Substring(postedBgFile.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "jpg"
                        && postedBgFile.FileName.Substring(postedBgFile.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "jpeg"
                        && postedBgFile.FileName.Substring(postedBgFile.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "png")
                    {
                        return Json(new { success = false, file = false, message = "Invalid file. Please upload pdf, jpg, jpeg or png files." });
                    }

                    if (postedBgFile.ContentLength > maxSize)
                    {
                        return Json(new { success = false, file = false, message = "Invalid file size. Please upload file upto 4 MB." });
                    }
                }

                if (ModelState.IsValid)
                {
                    status = objBAL.InsertPermissionClaimDetailsBAL(formCollection, postedBgFile, out isValidMsg);

                    return Json(new { success = status, message = isValidMsg });
                }

                return Json(new { success = status, message = message });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.InsertPermissionClaimDetails()");
                return null;
            }
        }

        [Audit]
        public ActionResult GetPermissionClaimList(int scheduleCode, int? page, int? rows, string sidx, string sord)
        {
            try
            {
                long totalRecords = 0;
                ITourClaimBAL objBAL = new TourClaimBAL();

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.GetPermissionClaimListBAL(scheduleCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.GetPermissionClaimList()");
                return null;
            }
        }

        public ActionResult EditPermissionDetails(int permissionId)
        {
            try
            {
                NQM_TOUR_PERMISSION_MODEL model = new NQM_TOUR_PERMISSION_MODEL();
                NQM_TOUR_CLAIM_MASTER masterModel = new NQM_TOUR_CLAIM_MASTER();
                QUALITY_QM_SCHEDULE scheduleModel = new QUALITY_QM_SCHEDULE();
                NQM_TOUR_PERMISSION_DETAILS perModel = new NQM_TOUR_PERMISSION_DETAILS();

                perModel = dbContext.NQM_TOUR_PERMISSION_DETAILS.Where(x => x.ID == permissionId).FirstOrDefault();
                masterModel = dbContext.NQM_TOUR_CLAIM_MASTER.Where(x => x.TOUR_CLAIM_ID == perModel.TOUR_CLAIM_ID).FirstOrDefault();
                scheduleModel = dbContext.QUALITY_QM_SCHEDULE.Where(x => x.ADMIN_SCHEDULE_CODE == masterModel.ADMIN_SCHEDULE_CODE).FirstOrDefault();

                model.PERMISSION_ID = permissionId;
                model.TOUR_CLAIM_ID = masterModel.TOUR_CLAIM_ID;
                model.ADMIN_SCHEDULE_CODE = masterModel.ADMIN_SCHEDULE_CODE;
                model.DATE = perModel.DATE.ToShortDateString();
                model.DESCRIPTION = perModel.REMARK;
                model.UPLOADED_FILE_NAME = perModel.PERMISSION_DOCUMENT_NAME == null ? "--" : URLEncrypt.EncryptParameters1(new String[] { "FileName =" + perModel.PERMISSION_DOCUMENT_NAME });
                model.DEC_UPLOADED_FILE_NAME = perModel.PERMISSION_DOCUMENT_NAME == null ? "" : perModel.PERMISSION_DOCUMENT_NAME;
                model.ADD_EDIT = 2;

                return View("AddPermissionClaim", model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.EditPermissionDetails()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult UpdatePermissionClaimDetails(FormCollection formCollection)
        {
            try
            {
                ITourClaimBAL objBAL = new TourClaimBAL();
                bool status = false;
                string isValidMsg = String.Empty;
                String message = "Permission Details not updated";
                HttpPostedFileBase postedBgFile = Request.Files[0];

                if (postedBgFile.ContentLength > 0)
                {
                    int maxSize = 1024 * 1024 * 4;

                    if (postedBgFile.FileName.Substring(postedBgFile.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "pdf"
                        && postedBgFile.FileName.Substring(postedBgFile.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "jpg"
                        && postedBgFile.FileName.Substring(postedBgFile.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "jpeg"
                        && postedBgFile.FileName.Substring(postedBgFile.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "png")
                    {
                        return Json(new { success = false, file = false, message = "Invalid file. Please upload pdf, jpg, jpeg or png files." });
                    }

                    if (postedBgFile.ContentLength > maxSize)
                    {
                        return Json(new { success = false, file = false, message = "Invalid file size. Please upload file upto 4 MB." });
                    }
                }

                if (ModelState.IsValid)
                {
                    status = objBAL.UpdatePermissionClaimDetailsBAL(formCollection, postedBgFile, out isValidMsg);

                    return Json(new { success = status, message = isValidMsg });
                }

                return Json(new { success = status, message = message });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.UpdatePermissionClaimDetails()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult DeletePermissionDetails(int permissionId)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    bool status = false;
                    String message = "Details not deleted";
                    String FileName = String.Empty;
                    String FilePath = String.Empty;

                    NQM_TOUR_PERMISSION_DETAILS model = new NQM_TOUR_PERMISSION_DETAILS();
                    model = dbContext.NQM_TOUR_PERMISSION_DETAILS.Where(x => x.ID == permissionId).FirstOrDefault();

                    FileName = model.PERMISSION_DOCUMENT_NAME;
                    FilePath = model.PERMISSION_DOCUMENT_PATH;

                    dbContext.NQM_TOUR_PERMISSION_DETAILS.Remove(dbContext.NQM_TOUR_PERMISSION_DETAILS.Where(x => x.ID == permissionId).FirstOrDefault());
                    dbContext.SaveChanges();

                    if (FileName != null)
                    {
                        //string path = ConfigurationManager.AppSettings["NQM_TOUR_CLAIM_PATH"].ToString();
                        var fullPath = Path.Combine(FilePath, FileName);

                        FileInfo file = new FileInfo(fullPath);
                        if (file.Exists)
                        {
                            file.Delete();
                        }
                    }

                    status = true;
                    message = "Details deleted successfully";

                    scope.Complete();

                    return Json(new { success = status, message = message });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.DeletePermissionDetails()");
                return Json(new { success = false, message = "Details not deleted." });
            }

        }

        public FileResult ViewUploadedPermissionDetails(string id1)
        {
            try
            {
                String FileName = String.Empty;
                string[] encParam = id1.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encParam[0], encParam[1], encParam[2] });
                if (decryptedParameters.Count > 0)
                {
                    FileName = decryptedParameters["FileName"];
                }

                string path = ConfigurationManager.AppSettings["NQM_TOUR_CLAIM_PATH"].ToString();
                var fullPath = Path.Combine(path, "Permissions", FileName.Split('_')[0], FileName);
                byte[] FileBytes = System.IO.File.ReadAllBytes(fullPath);

                string type = string.Empty;
                type = FileName.Substring(FileName.LastIndexOf('.') + 1).Trim().ToLower();
                if (type == "pdf")
                {
                    return File(FileBytes, "application/pdf");
                }
                else
                {
                    return File(FileBytes, "image/png");
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.ViewUploadedPermissionDetails()");
                return null;
            }
        }

        #endregion

        #endregion

        #region CQC Tour Claim 


        #region Main Form

        public ActionResult AddSanctionedAmount(int scheduleCode)
        {
            try
            {
                NQM_TOUR_CLAIM_MASTER_MODEL model = new NQM_TOUR_CLAIM_MASTER_MODEL();
                ADMIN_QUALITY_MONITORS adminMonitor = new ADMIN_QUALITY_MONITORS();
                NQM_TOUR_CLAIM_MASTER tourModel = new NQM_TOUR_CLAIM_MASTER();
                ADMIN_QUALITY_MONITORS_BANK bankModel = new ADMIN_QUALITY_MONITORS_BANK();

                tourModel = dbContext.NQM_TOUR_CLAIM_MASTER.Where(x => x.ADMIN_SCHEDULE_CODE == scheduleCode).FirstOrDefault();
                adminMonitor = dbContext.ADMIN_QUALITY_MONITORS.Where(x => x.ADMIN_QM_CODE == tourModel.ADMIN_QM_CODE).FirstOrDefault();
                bankModel = dbContext.ADMIN_QUALITY_MONITORS_BANK.Where(x => x.ADMIN_QM_CODE == tourModel.ADMIN_QM_CODE).FirstOrDefault();
                int tourId = tourModel.TOUR_CLAIM_ID;

                List<TOUR_CLAIM_CALCULATION_Result> lstExecution = new List<TOUR_CLAIM_CALCULATION_Result>();

                var item = dbContext.TOUR_CLAIM_CALCULATION(tourId, null, null).FirstOrDefault();

                model.TOUR_CLAIM_ID = tourId;
                model.ADMIN_SCHEDULE_CODE = tourModel.ADMIN_SCHEDULE_CODE;

                model.ADMIN_QM_NAME = (adminMonitor.ADMIN_QM_FNAME == null ? "" : adminMonitor.ADMIN_QM_FNAME) + " " + (adminMonitor.ADMIN_QM_MNAME == null ? "" : adminMonitor.ADMIN_QM_MNAME) + " " + (adminMonitor.ADMIN_QM_LNAME == null ? "" : adminMonitor.ADMIN_QM_LNAME);
                model.NRRDA_LETTER_NUMBER = tourModel.NRRDA_LETTER_NUMBER;
                model.ADMIN_QM_PAN = adminMonitor.ADMIN_QM_PAN == null ? "--" : adminMonitor.ADMIN_QM_PAN;

                model.BANK_NAME = bankModel.MAST_BANK_NAME;
                model.ACCOUNT_NUMBER = bankModel.MAST_ACCOUNT_NUMBER;
                model.IFSC_CODE = bankModel.MAST_IFSC_CODE;
                model.finalizeFlag = tourModel.FINALIZE_FLAG;
                model.ROUND_SEQUENCE = tourModel.ROUNDS_SEQUENCE;

                model.TOTAL_AMOUNT_CLAIMED = item.DISTRICT_VISITED_ALLOWANCE + 200 + item.TOTAL_TRAVEL_CLAIM_AMOUNT + item.TOTAL_LODGE_CLAIM_AMOUNT + item.TOTAL_DAILY_CLAIM_AMOUNT + item.TOTAL_INSPECTION_CLAIM_AMOUNT + item.TOTAL_MEETING_CLAIM_AMOUNT + item.TOTAL_MISCELLANEOUS_CLAIM_AMOUNT;

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.AddSanctionedAmount()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult FinalizeSanctionTourDetails(string adminScheduleCode)
        {
            try
            {
                ITourClaimBAL objBAL = new TourClaimBAL();
                bool status = false;
                string isValidMsg = String.Empty;
                String message = "Tour claim details not finalized";


                if (ModelState.IsValid)
                {
                    status = objBAL.FinalizeSanctionTourDetailsBAL(adminScheduleCode, out isValidMsg);

                    return Json(new { success = status, message = isValidMsg });
                }

                return Json(new { success = status, message = message });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.FinalizeSanctionTourDetails()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult ApproveTourDetails(string adminScheduleCode)
        {
            try
            {
                ITourClaimBAL objBAL = new TourClaimBAL();
                bool status = false;
                string isValidMsg = String.Empty;
                String message = "Tour claim details not approved";

                if (ModelState.IsValid)
                {
                    status = objBAL.ApproveTourDetailsBAL(adminScheduleCode, out isValidMsg);

                    return Json(new { success = status, message = isValidMsg });
                }

                return Json(new { success = status, message = message });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.ApproveTourDetails()");
                return null;
            }
        }

        public ActionResult ViewSummaryDetailsReportCQC(int scheduleCode)
        {
            try
            {
                NQM_TOUR_CLAIM_MASTER masterModel = new NQM_TOUR_CLAIM_MASTER();
                NQM_TOUR_CLAIM_MASTER_MODEL model = new NQM_TOUR_CLAIM_MASTER_MODEL();

                masterModel = dbContext.NQM_TOUR_CLAIM_MASTER.Where(x => x.ADMIN_SCHEDULE_CODE == scheduleCode).FirstOrDefault();
                ViewBag.tourId = masterModel.TOUR_CLAIM_ID;

                model.ADMIN_SCHEDULE_CODE = masterModel.ADMIN_SCHEDULE_CODE;
                model.MONTH_OF_INSPECTION = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(masterModel.MONTH_OF_INSPECTION);
                model.YEAR_OF_INSPECTION = masterModel.YEAR_OF_INSPECTION;
                model.finalizeFlag = masterModel.FINALIZE_FLAG;
                model.ROUND_SEQUENCE = masterModel.ROUNDS_SEQUENCE;
                model.DIRECTOR = string.IsNullOrEmpty(masterModel.CQC_APPROVE_DIRECTOR) ? "Dr. I.K.Pateriya, Director (P-III)" : masterModel.CQC_APPROVE_DIRECTOR;
                if (model.finalizeFlag == 1 && model.ROUND_SEQUENCE == 1)
                    model.OFFICE_ASSISTANT_P_III = masterModel.CQC_FINALIZE_OFFICE_ASSISTANT_P_III;

                List<NQM_TRAVEL_CLAIM_DETAILS> travelList = new List<NQM_TRAVEL_CLAIM_DETAILS>();
                List<NQM_LODGE_AND_DAILY_CLAIM_DETAILS> lodgeList = new List<NQM_LODGE_AND_DAILY_CLAIM_DETAILS>();
                List<NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS> inspList = new List<NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS>();
                List<NQM_TOUR_HONORARIUM_MEETING_WITH_PIU> meetingList = new List<NQM_TOUR_HONORARIUM_MEETING_WITH_PIU>();
                List<NQM_TOUR_MISCELLANEOUS> misList = new List<NQM_TOUR_MISCELLANEOUS>();

                List<string> remarks = new List<string>();

                travelList = dbContext.NQM_TRAVEL_CLAIM_DETAILS.Where(x => x.TOUR_CLAIM_ID == masterModel.TOUR_CLAIM_ID).ToList();

                if (travelList != null)
                {
                    foreach (var item in travelList)
                    {
                        if (!string.IsNullOrEmpty(item.REMARK_CQC))
                            remarks.Add(item.REMARK_CQC);
                    }
                }

                ViewBag.travelList = remarks;
                remarks = new List<string>();

                lodgeList = dbContext.NQM_LODGE_AND_DAILY_CLAIM_DETAILS.Where(x => x.TOUR_CLAIM_ID == masterModel.TOUR_CLAIM_ID).ToList();

                if (lodgeList != null)
                {
                    foreach (var item in lodgeList)
                    {
                        if (!string.IsNullOrEmpty(item.REMARK_CQC))
                            remarks.Add(item.REMARK_CQC);
                    }
                }

                ViewBag.lodgeList = remarks;
                remarks = new List<string>();

                inspList = dbContext.NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS.Where(x => x.TOUR_CLAIM_ID == masterModel.TOUR_CLAIM_ID).ToList();

                if (inspList != null)
                {
                    foreach (var item in inspList)
                    {
                        if (!string.IsNullOrEmpty(item.REMARK_CQC))
                            remarks.Add(item.REMARK_CQC);
                    }
                }

                ViewBag.inspList = remarks;
                remarks = new List<string>();

                meetingList = dbContext.NQM_TOUR_HONORARIUM_MEETING_WITH_PIU.Where(x => x.TOUR_CLAIM_ID == masterModel.TOUR_CLAIM_ID).ToList();

                if (meetingList != null)
                {
                    foreach (var item in meetingList)
                    {
                        if (!string.IsNullOrEmpty(item.REMARK_CQC))
                            remarks.Add(item.REMARK_CQC);
                    }
                }

                ViewBag.meetingList = remarks;
                remarks = new List<string>();

                misList = dbContext.NQM_TOUR_MISCELLANEOUS.Where(x => x.TOUR_CLAIM_ID == masterModel.TOUR_CLAIM_ID).ToList();

                if (misList != null)
                {
                    foreach (var item in misList)
                    {
                        if (!string.IsNullOrEmpty(item.REMARK_CQC))
                            remarks.Add(item.REMARK_CQC);
                    }
                }

                ViewBag.misList = remarks;
                remarks = new List<string>();

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.ViewSummaryDetailsReportCQC()");
                return null;
            }
        }

        public ActionResult GetNotificationCQC(string currMonthYear)
        {
            try
            {
                List<NQM_TOUR_CLAIM_MASTER> revertModel = new List<NQM_TOUR_CLAIM_MASTER>();
                List<NQM_TOUR_CLAIM_MASTER> approveModel = new List<NQM_TOUR_CLAIM_MASTER>();
                List<NQM_TOUR_CLAIM_MASTER> finalizedByNqmModel = new List<NQM_TOUR_CLAIM_MASTER>();
                List<NQM_TOUR_CLAIM_MASTER_MODEL> model = new List<NQM_TOUR_CLAIM_MASTER_MODEL>();
                ADMIN_QUALITY_MONITORS monitor = new ADMIN_QUALITY_MONITORS();
                CommonFunctions objCommonFunctions = new CommonFunctions();
                string month;
                string name;
                int currMonth = Convert.ToInt32(currMonthYear.Split('$')[0]);
                int currYear = Convert.ToInt32(currMonthYear.Split('$')[1]);

                List<SelectListItem> monthList = new List<SelectListItem>();
                List<SelectListItem> yearList = new List<SelectListItem>();
                monthList = objCommonFunctions.PopulateMonths(false);
                yearList = objCommonFunctions.PopulateYears(false).Where(a => a.Value != "0").ToList();
                if (DateTime.Now.Month == 12)
                {
                    string nextYear = (DateTime.Now.Year + 1).ToString();
                    yearList.Add(new SelectListItem { Text = nextYear, Value = nextYear });
                    yearList = yearList.OrderByDescending(c => c.Value).ToList();
                }

                ViewBag.MonthList = monthList;
                ViewBag.YearList = yearList;
                ViewBag.Month = currMonth;
                ViewBag.Year = currYear;

                revertModel = dbContext.NQM_TOUR_CLAIM_MASTER.Where(x => x.FINALIZE_FLAG == 1 && x.ROUNDS_SEQUENCE == 1).OrderByDescending(y => y.FIN2_REVERTE_DATE).ToList();
                approveModel = dbContext.NQM_TOUR_CLAIM_MASTER.Where(x => x.FINALIZE_FLAG == 4).OrderByDescending(y => y.FIN2_APPROVE_DATE).ToList();
                finalizedByNqmModel = PopulatePendingBillsAtCqc(currMonth, currYear);

                foreach (var item in revertModel)
                {
                    DateTime date = new DateTime(2020, item.MONTH_OF_INSPECTION, 1);
                    month = date.ToString("MMM");
                    monitor = dbContext.ADMIN_QUALITY_MONITORS.Where(x => x.ADMIN_QM_CODE == item.ADMIN_QM_CODE).FirstOrDefault();
                    name = ((monitor.ADMIN_QM_FNAME == null || monitor.ADMIN_QM_FNAME == string.Empty) ? " " : monitor.ADMIN_QM_FNAME) + " " + ((monitor.ADMIN_QM_MNAME == null || monitor.ADMIN_QM_MNAME == string.Empty) ? " " : monitor.ADMIN_QM_MNAME) + " " + ((monitor.ADMIN_QM_LNAME == null || monitor.ADMIN_QM_LNAME == string.Empty) ? " " : monitor.ADMIN_QM_LNAME);

                    model.Add(new NQM_TOUR_CLAIM_MASTER_MODEL
                    {
                        MONTH_OF_INSPECTION = month,
                        YEAR_OF_INSPECTION = item.YEAR_OF_INSPECTION,
                        ADMIN_QM_NAME = name,
                        DATE_OF_CLAIM = Convert.ToDateTime(item.FIN2_REVERTE_DATE).ToString("dd/MM/yyyy"),
                        REMARK = item.FIN2_REVERTE_REMARK,
                        addEditCheck = 1,
                    });
                }

                foreach (var item in approveModel)
                {
                    DateTime date = new DateTime(2020, item.MONTH_OF_INSPECTION, 1);
                    month = date.ToString("MMM");
                    monitor = dbContext.ADMIN_QUALITY_MONITORS.Where(x => x.ADMIN_QM_CODE == item.ADMIN_QM_CODE).FirstOrDefault();
                    name = ((monitor.ADMIN_QM_FNAME == null || monitor.ADMIN_QM_FNAME == string.Empty) ? " " : monitor.ADMIN_QM_FNAME) + " " + ((monitor.ADMIN_QM_MNAME == null || monitor.ADMIN_QM_MNAME == string.Empty) ? " " : monitor.ADMIN_QM_MNAME) + " " + ((monitor.ADMIN_QM_LNAME == null || monitor.ADMIN_QM_LNAME == string.Empty) ? " " : monitor.ADMIN_QM_LNAME);

                    model.Add(new NQM_TOUR_CLAIM_MASTER_MODEL
                    {
                        MONTH_OF_INSPECTION = month,
                        YEAR_OF_INSPECTION = item.YEAR_OF_INSPECTION,
                        ADMIN_QM_NAME = name,
                        DATE_OF_CLAIM = Convert.ToDateTime(item.FIN2_APPROVE_DATE).ToString("dd/MM/yyyy"),
                        REMARK = (item.FIN2_APPROVE_REMARK == null || item.FIN2_APPROVE_REMARK == string.Empty) ? "--" : item.FIN2_APPROVE_REMARK,
                        addEditCheck = 2
                    });
                }

                foreach (var item in finalizedByNqmModel)
                {
                    //DateTime date = new DateTime(2020, item.MONTH_OF_INSPECTION, 1);
                    //month = date.ToString("MMM");
                    monitor = dbContext.ADMIN_QUALITY_MONITORS.Where(x => x.ADMIN_QM_CODE == item.ADMIN_QM_CODE).FirstOrDefault();
                    name = ((monitor.ADMIN_QM_FNAME == null || monitor.ADMIN_QM_FNAME == string.Empty) ? " " : monitor.ADMIN_QM_FNAME) + " " + ((monitor.ADMIN_QM_MNAME == null || monitor.ADMIN_QM_MNAME == string.Empty) ? " " : monitor.ADMIN_QM_MNAME) + " " + ((monitor.ADMIN_QM_LNAME == null || monitor.ADMIN_QM_LNAME == string.Empty) ? " " : monitor.ADMIN_QM_LNAME);

                    CultureInfo cultureInfo = CultureInfo.CurrentCulture;
                    string monthName = cultureInfo.DateTimeFormat.GetMonthName(item.MONTH_OF_INSPECTION);

                    model.Add(new NQM_TOUR_CLAIM_MASTER_MODEL
                    {
                        MONTH_OF_INSPECTION = monthName,
                        YEAR_OF_INSPECTION = item.YEAR_OF_INSPECTION,
                        ADMIN_QM_NAME = name,
                        DATE_OF_CLAIM = Convert.ToDateTime(item.NQM_FINALIZATION_DATE).ToString("dd/MM/yyyy"),
                        REMARK = item.FIN2_REVERTE_REMARK,
                        addEditCheck = 3,
                    });
                }

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.GetNotificationCQC()");
                return null;
            }
        }

        public List<NQM_TOUR_CLAIM_MASTER> PopulatePendingBillsAtCqc(int month, int year)
        {
            try
            {
                List<NQM_TOUR_CLAIM_MASTER> modelList = new List<NQM_TOUR_CLAIM_MASTER>();

                modelList = (List<NQM_TOUR_CLAIM_MASTER>)(from master in dbContext.NQM_TOUR_CLAIM_MASTER
                                                          where
                                                          master.FINALIZE_FLAG == 1
                                                          && master.ROUNDS_SEQUENCE == 0
                                                          && master.MONTH_OF_INSPECTION <= month
                                                          && master.YEAR_OF_INSPECTION <= year
                                                          select master).Distinct().ToList();

                return modelList;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.PopulatePendingBillsAtCqc()");
                return null;
            }
        }

        #endregion


        #region District

        [Audit]
        public ActionResult GetTourDistrictListCqc(int scheduleCode, int? page, int? rows, string sidx, string sord)

        {
            try
            {
                long totalRecords = 0;
                ITourClaimBAL objBAL = new TourClaimBAL();

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.GetTourDistrictListCqcBAL(scheduleCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.GetTourDistrictListCqc()");
                return null;
            }
        }

        public ActionResult ViewQuickResponseSheet()
        {
            dbContext = new PMGSYEntities();
            try
            {
                int districtDetailsId = Convert.ToInt32(Request.Params["districtDetailsId"]);
                ResponseSheet responseSheetModel = new ResponseSheet();
                NQM_TOUR_DISTRICT_DETAILS districtModel = new NQM_TOUR_DISTRICT_DETAILS();
                NQM_TOUR_CLAIM_MASTER tourModel = new NQM_TOUR_CLAIM_MASTER();

                districtModel = dbContext.NQM_TOUR_DISTRICT_DETAILS.Where(x => x.DISTRICT_DETAILS_ID == districtDetailsId).FirstOrDefault();
                tourModel = dbContext.NQM_TOUR_CLAIM_MASTER.Where(x => x.TOUR_CLAIM_ID == districtModel.TOUR_CLAIM_ID).FirstOrDefault();

                responseSheetModel.Monitor = tourModel.ADMIN_QM_CODE;
                responseSheetModel.FromYear = tourModel.YEAR_OF_INSPECTION;
                responseSheetModel.FromMonth = tourModel.MONTH_OF_INSPECTION;
                responseSheetModel.SchemeCode = 0;
                responseSheetModel.State = dbContext.MASTER_DISTRICT.Where(x => x.MAST_DISTRICT_CODE == districtModel.DISTRICT).Select(y => y.MAST_STATE_CODE).FirstOrDefault();
                responseSheetModel.District = districtModel.DISTRICT;

                return View(responseSheetModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.ViewQuickResponseSheet()");
                return null;
            }
        }

        #endregion


        #region Travel Claim

        [Audit]
        public ActionResult GetTravelClaimListCqc(int scheduleCode, int? page, int? rows, string sidx, string sord)
        {
            try
            {
                long totalRecords = 0;
                ITourClaimBAL objBAL = new TourClaimBAL();

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.GetTravelClaimListCqcBAL(scheduleCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.GetTravelClaimListCqc()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult AddSanctionAmtTravel(int travelId, decimal amount, string remark)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    bool status = false;
                    String message = "Details not added";
                    String FileName = String.Empty;

                    NQM_TRAVEL_CLAIM_DETAILS travelModel = new NQM_TRAVEL_CLAIM_DETAILS();

                    travelModel = dbContext.NQM_TRAVEL_CLAIM_DETAILS.Where(x => x.TRAVEL_CLAIM_ID == travelId).FirstOrDefault();

                    travelModel.AMOUNT_PASSED_CQC = amount;
                    travelModel.REMARK_CQC = (remark == "" ? null : remark);
                    travelModel.DATE_OF_MODIFICATION_CQC = DateTime.Now;

                    dbContext.Entry(travelModel).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    status = true;
                    message = "Details added successfully";

                    scope.Complete();

                    return Json(new { success = status, message = message });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.AddSanctionAmtTravel()");
                return Json(new { success = false, message = "Details not uploaded." });
            }

        }

        #endregion


        #region Lodge and daily Claim

        [Audit]
        public ActionResult GetLodgeClaimListCqc(int scheduleCode, int? page, int? rows, string sidx, string sord)
        {
            try
            {
                long totalRecords = 0;
                ITourClaimBAL objBAL = new TourClaimBAL();

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.GetLodgeClaimListCqcBAL(scheduleCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.GetLodgeClaimListCqc()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult AddSanctionAmtLodge(int lodgeId, decimal amount, string remark)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    bool status = false;
                    String message = "Details not added";
                    String FileName = String.Empty;

                    NQM_LODGE_AND_DAILY_CLAIM_DETAILS lodgeModel = new NQM_LODGE_AND_DAILY_CLAIM_DETAILS();

                    lodgeModel = dbContext.NQM_LODGE_AND_DAILY_CLAIM_DETAILS.Where(x => x.LODGE_CLAIM_ID == lodgeId).FirstOrDefault();

                    if (lodgeModel.TYPE_OF_CLAIM.Equals("H") || lodgeModel.TYPE_OF_CLAIM.Equals("G"))
                        lodgeModel.AMOUNT_PASSED_HOTEL_CQC = amount;

                    lodgeModel.REMARK_CQC = (remark == "" ? null : remark);
                    lodgeModel.DATE_OF_MODIFICATION_CQC = DateTime.Now;

                    dbContext.Entry(lodgeModel).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    status = true;
                    message = "Details added successfully";

                    scope.Complete();

                    return Json(new { success = status, message = message });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.AddSanctionAmtLodge()");
                return Json(new { success = false, message = "Details not uploaded." });
            }

        }

        #endregion


        #region Inspection of Road Claim

        [Audit]
        public ActionResult GetInspectionHonorariumListCqc(int scheduleCode, int? page, int? rows, string sidx, string sord)
        {
            try
            {
                long totalRecords = 0;
                ITourClaimBAL objBAL = new TourClaimBAL();

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.GetInspectionHonorariumListCqcBAL(scheduleCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.GetInspectionHonorariumListCqc()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult AddSanctionAmtInsp(int inspId, decimal amount, string remark)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    bool status = false;
                    String message = "Details not added";
                    String FileName = String.Empty;

                    NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS inspModel = new NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS();

                    inspModel = dbContext.NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS.Where(x => x.HONORARIUM_INSPECTION_ID == inspId).FirstOrDefault();

                    inspModel.AMOUNT_PASSED_CQC = amount;
                    inspModel.REMARK_CQC = (remark == "" ? null : remark);
                    inspModel.DATE_OF_MODIFICATION_CQC = DateTime.Now;

                    dbContext.Entry(inspModel).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    status = true;
                    message = "Details added successfully";

                    scope.Complete();

                    return Json(new { success = status, message = message });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.AddSanctionAmtInsp()");
                return Json(new { success = false, message = "Details not uploaded." });
            }

        }

        #endregion


        #region Meeting with PIU Claim

        [Audit]
        public ActionResult GetMeetingHonorariumListCqc(int scheduleCode, int? page, int? rows, string sidx, string sord)
        {
            try
            {
                long totalRecords = 0;
                ITourClaimBAL objBAL = new TourClaimBAL();

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.GetMeetingHonorariumListCqcBAL(scheduleCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.GetInspectionHonorariumListCqc()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult AddSanctionAmtMeeting(int meetingId, decimal amount, string remark)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    bool status = false;
                    String message = "Details not added";
                    String FileName = String.Empty;

                    NQM_TOUR_HONORARIUM_MEETING_WITH_PIU meetingModel = new NQM_TOUR_HONORARIUM_MEETING_WITH_PIU();

                    meetingModel = dbContext.NQM_TOUR_HONORARIUM_MEETING_WITH_PIU.Where(x => x.HONORARIUM_MEETING_ID == meetingId).FirstOrDefault();

                    meetingModel.AMOUNT_PASSED_CQC = amount;
                    meetingModel.REMARK_CQC = (remark == "" ? null : remark);
                    meetingModel.DATE_OF_MODIFICATION_CQC = DateTime.Now;

                    dbContext.Entry(meetingModel).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    status = true;
                    message = "Details added successfully";

                    scope.Complete();

                    return Json(new { success = status, message = message });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.AddSanctionAmtMeeting()");
                return Json(new { success = false, message = "Details not uploaded." });
            }

        }

        #endregion


        #region Miscellaneous

        [Audit]
        public ActionResult LoadMiscellaneousClaimListCqc(int scheduleCode, int? page, int? rows, string sidx, string sord)
        {
            try
            {
                long totalRecords = 0;
                ITourClaimBAL objBAL = new TourClaimBAL();

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.LoadMiscellaneousClaimListCqcBAL(scheduleCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.LoadMiscellaneousClaimListCqc()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult AddSanctionAmtMiscellaneous(int miscellaneousId, decimal amount, string remark)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    bool status = false;
                    String message = "Details not added";
                    String FileName = String.Empty;

                    NQM_TOUR_MISCELLANEOUS misModel = new NQM_TOUR_MISCELLANEOUS();

                    misModel = dbContext.NQM_TOUR_MISCELLANEOUS.Where(x => x.MISCELLANEOUS_ID == miscellaneousId).FirstOrDefault();

                    misModel.AMOUNT_PASSED_CQC = amount;
                    misModel.REMARK_CQC = (remark == "" ? null : remark);
                    misModel.DATE_OF_MODIFICATION_CQC = DateTime.Now;

                    dbContext.Entry(misModel).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    status = true;
                    message = "Details added successfully";

                    scope.Complete();

                    return Json(new { success = status, message = message });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.AddSanctionAmtMiscellaneous()");
                return Json(new { success = false, message = "Details not uploaded." });
            }

        }

        #endregion


        #region Permissions

        [Audit]
        public ActionResult LoadPermissionClaimListCqc(int scheduleCode, int? page, int? rows, string sidx, string sord)
        {
            try
            {
                long totalRecords = 0;
                ITourClaimBAL objBAL = new TourClaimBAL();

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.LoadPermissionClaimListCqcBAL(scheduleCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.LoadPermissionClaimListCqc()");
                return null;
            }
        }

        #endregion

        #endregion

        #region Finance 1 Tour Claim

        #region  Main Form

        public ActionResult ViewFinanceFilter()
        {
            try
            {
                if (PMGSYSession.Current.RoleCode == 14)
                {
                    return RedirectToAction("ViewFinance2Filter");
                }

                NQM_TOUR_CLAIM_MASTER_MODEL qmFilterModel = new NQM_TOUR_CLAIM_MASTER_MODEL();
                CommonFunctions objCommonFunctions = new CommonFunctions();

                qmFilterModel.FROM_MONTH = DateTime.Now.Month;
                qmFilterModel.FROM_YEAR = DateTime.Now.Year;
                qmFilterModel.FROM_MONTHS_LIST = objCommonFunctions.PopulateMonths(false);
                qmFilterModel.FROM_YEARS_LIST = objCommonFunctions.PopulateYears(false).Where(a => a.Value != "0").ToList();
                if (DateTime.Now.Month == 12)
                {
                    string nextYear = (DateTime.Now.Year + 1).ToString();
                    qmFilterModel.FROM_YEARS_LIST.Add(new SelectListItem { Text = nextYear, Value = nextYear });
                    qmFilterModel.FROM_YEARS_LIST = qmFilterModel.FROM_YEARS_LIST.OrderByDescending(c => c.Value).ToList();
                }
                return View(qmFilterModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.ViewFinanceFilter()");
                return null;
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult GetFinanceMonitorList(FormCollection formCollection)
        {
            ITourClaimBAL objBAL = new TourClaimBAL();
            int totalRecords;
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.GetFinanceMonitorListBAL(Convert.ToInt32(formCollection["month"]), Convert.ToInt32(formCollection["year"]), Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.GetFinanceMonitorList()");
                return Json(String.Empty);
            }
        }

        public ActionResult ViewEditTourFinance(int tourClaimId)
        {
            try
            {
                NQM_TOUR_CLAIM_MASTER_MODEL model = new NQM_TOUR_CLAIM_MASTER_MODEL();
                NQM_TOUR_CLAIM_MASTER masterModel = new NQM_TOUR_CLAIM_MASTER();
                ADMIN_QUALITY_MONITORS_BANK bankModel = new ADMIN_QUALITY_MONITORS_BANK();
                ADMIN_QUALITY_MONITORS adminMonitor = new ADMIN_QUALITY_MONITORS();

                masterModel = dbContext.NQM_TOUR_CLAIM_MASTER.Where(x => x.TOUR_CLAIM_ID == tourClaimId).FirstOrDefault();
                adminMonitor = dbContext.ADMIN_QUALITY_MONITORS.Where(x => x.ADMIN_QM_CODE == masterModel.ADMIN_QM_CODE).FirstOrDefault();
                bankModel = dbContext.ADMIN_QUALITY_MONITORS_BANK.Where(x => x.ADMIN_QM_CODE == masterModel.ADMIN_QM_CODE).FirstOrDefault();

                List<TOUR_CLAIM_CALCULATION_Result> lstExecution = new List<TOUR_CLAIM_CALCULATION_Result>();

                var item = dbContext.TOUR_CLAIM_CALCULATION(tourClaimId, null, null).FirstOrDefault();

                model.TOUR_CLAIM_ID = tourClaimId;
                model.ADMIN_SCHEDULE_CODE = masterModel.ADMIN_SCHEDULE_CODE;

                model.ADMIN_QM_NAME = (adminMonitor.ADMIN_QM_FNAME == null ? "" : adminMonitor.ADMIN_QM_FNAME) + " " + (adminMonitor.ADMIN_QM_MNAME == null ? "" : adminMonitor.ADMIN_QM_MNAME) + " " + (adminMonitor.ADMIN_QM_LNAME == null ? "" : adminMonitor.ADMIN_QM_LNAME);
                model.NRRDA_LETTER_NUMBER = masterModel.NRRDA_LETTER_NUMBER;
                model.ADMIN_QM_PAN = adminMonitor.ADMIN_QM_PAN == null ? "--" : adminMonitor.ADMIN_QM_PAN;

                model.BANK_NAME = bankModel.MAST_BANK_NAME;
                model.ACCOUNT_NUMBER = bankModel.MAST_ACCOUNT_NUMBER;
                model.IFSC_CODE = bankModel.MAST_IFSC_CODE;
                model.finalizeFlag = masterModel.FINALIZE_FLAG;

                model.TOTAL_AMOUNT_CLAIMED = item.DISTRICT_VISITED_ALLOWANCE + 200 + item.TOTAL_TRAVEL_CLAIM_AMOUNT + item.TOTAL_LODGE_CLAIM_AMOUNT + item.TOTAL_DAILY_CLAIM_AMOUNT + item.TOTAL_INSPECTION_CLAIM_AMOUNT + item.TOTAL_MEETING_CLAIM_AMOUNT + item.TOTAL_MISCELLANEOUS_CLAIM_AMOUNT;
                model.TOTAL_AMOUNT_SANCTIONED = (item.DISTRICT_VISITED_ALLOWANCE + item.TOTAL_TRAVEL_SANCTIONED_AMOUNT + item.TOTAL_LODGE_SANCTIONED_AMOUNT + item.TOTAL_DAILY_SANCTIONED_AMOUNT + item.TOTAL_INSPECTION_SANCTIONED_AMOUNT + item.TOTAL_MEETING_SANCTIONED_AMOUNT + item.TOTAL_MISCELLANEOUS_SANCTIONED_AMOUNT + 200).ToString();

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.ViewEditTourFinance()");
                return null;
            }
        }

        public ActionResult ViewSummaryDetailsReportFin1(int scheduleCode)
        {
            try
            {
                NQM_TOUR_CLAIM_MASTER masterModel = new NQM_TOUR_CLAIM_MASTER();
                NQM_TOUR_CLAIM_MASTER_MODEL model = new NQM_TOUR_CLAIM_MASTER_MODEL();

                masterModel = dbContext.NQM_TOUR_CLAIM_MASTER.Where(x => x.ADMIN_SCHEDULE_CODE == scheduleCode).FirstOrDefault();
                //ViewBag.tourId = masterModel.TOUR_CLAIM_ID;

                model.TOUR_CLAIM_ID = masterModel.TOUR_CLAIM_ID;
                model.ADMIN_SCHEDULE_CODE = masterModel.ADMIN_SCHEDULE_CODE;
                model.MONTH_OF_INSPECTION = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(masterModel.MONTH_OF_INSPECTION);
                model.YEAR_OF_INSPECTION = masterModel.YEAR_OF_INSPECTION;
                model.finalizeFlag = masterModel.FINALIZE_FLAG;
                model.ROUND_SEQUENCE = masterModel.ROUNDS_SEQUENCE;
                if (model.finalizeFlag == 2 && model.ROUND_SEQUENCE == 1)
                    model.OFFICE_ASSISTANT_FINANCE = masterModel.FIN1_FINALIZE_OFFICE_ASSISTANT_FINANCE;

                List<NQM_TRAVEL_CLAIM_DETAILS> travelList = new List<NQM_TRAVEL_CLAIM_DETAILS>();
                List<NQM_LODGE_AND_DAILY_CLAIM_DETAILS> lodgeList = new List<NQM_LODGE_AND_DAILY_CLAIM_DETAILS>();
                List<NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS> inspList = new List<NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS>();
                List<NQM_TOUR_HONORARIUM_MEETING_WITH_PIU> meetingList = new List<NQM_TOUR_HONORARIUM_MEETING_WITH_PIU>();
                List<NQM_TOUR_MISCELLANEOUS> misList = new List<NQM_TOUR_MISCELLANEOUS>();

                List<string> remarks = new List<string>();

                travelList = dbContext.NQM_TRAVEL_CLAIM_DETAILS.Where(x => x.TOUR_CLAIM_ID == masterModel.TOUR_CLAIM_ID).ToList();

                if (travelList != null)
                {
                    foreach (var item in travelList)
                    {
                        if (!string.IsNullOrEmpty(item.REMARK_FIN1))
                            remarks.Add(item.REMARK_FIN1);
                    }
                }

                ViewBag.travelList = remarks;
                remarks = new List<string>();

                lodgeList = dbContext.NQM_LODGE_AND_DAILY_CLAIM_DETAILS.Where(x => x.TOUR_CLAIM_ID == masterModel.TOUR_CLAIM_ID).ToList();

                if (lodgeList != null)
                {
                    foreach (var item in lodgeList)
                    {
                        if (!string.IsNullOrEmpty(item.REMARK_FIN1))
                            remarks.Add(item.REMARK_FIN1);
                    }
                }

                ViewBag.lodgeList = remarks;
                remarks = new List<string>();

                inspList = dbContext.NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS.Where(x => x.TOUR_CLAIM_ID == masterModel.TOUR_CLAIM_ID).ToList();

                if (inspList != null)
                {
                    foreach (var item in inspList)
                    {
                        if (!string.IsNullOrEmpty(item.REMARK_FIN1))
                            remarks.Add(item.REMARK_FIN1);
                    }
                }

                ViewBag.inspList = remarks;
                remarks = new List<string>();

                meetingList = dbContext.NQM_TOUR_HONORARIUM_MEETING_WITH_PIU.Where(x => x.TOUR_CLAIM_ID == masterModel.TOUR_CLAIM_ID).ToList();

                if (meetingList != null)
                {
                    foreach (var item in meetingList)
                    {
                        if (!string.IsNullOrEmpty(item.REMARK_FIN1))
                            remarks.Add(item.REMARK_FIN1);
                    }
                }

                ViewBag.meetingList = remarks;
                remarks = new List<string>();

                misList = dbContext.NQM_TOUR_MISCELLANEOUS.Where(x => x.TOUR_CLAIM_ID == masterModel.TOUR_CLAIM_ID).ToList();

                if (misList != null)
                {
                    foreach (var item in misList)
                    {
                        if (!string.IsNullOrEmpty(item.REMARK_FIN1))
                            remarks.Add(item.REMARK_FIN1);
                    }
                }

                ViewBag.misList = remarks;
                remarks = new List<string>();

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.ViewSummaryDetailsReportFin1()");
                return null;
            }
        }

        public ActionResult ViewNotificationFin(int currMonth = 0, int currYear = 0)
        {
            try
            {
                if (PMGSYSession.Current.RoleCode == 14)
                {
                    return RedirectToAction("ViewNotificationFin2");
                }

                List<NQM_TOUR_CLAIM_MASTER> masterModel = new List<NQM_TOUR_CLAIM_MASTER>();
                List<NQM_TOUR_CLAIM_MASTER_MODEL> model = new List<NQM_TOUR_CLAIM_MASTER_MODEL>();
                ADMIN_QUALITY_MONITORS monitor = new ADMIN_QUALITY_MONITORS();
                string month;
                string name;

                CommonFunctions objCommonFunctions = new CommonFunctions();

                if (currMonth == 0)
                    currMonth = DateTime.Now.Month;
                if (currYear == 0)
                    currYear = DateTime.Now.Year;

                List<SelectListItem> monthList = new List<SelectListItem>();
                List<SelectListItem> yearList = new List<SelectListItem>();
                monthList = objCommonFunctions.PopulateMonths(false);
                yearList = objCommonFunctions.PopulateYears(false).Where(a => a.Value != "0").ToList();
                if (DateTime.Now.Month == 12)
                {
                    string nextYear = (DateTime.Now.Year + 1).ToString();
                    yearList.Add(new SelectListItem { Text = nextYear, Value = nextYear });
                    yearList = yearList.OrderByDescending(c => c.Value).ToList();
                }

                ViewBag.MonthList = monthList;
                ViewBag.YearList = yearList;
                ViewBag.Month = currMonth;
                ViewBag.Year = currYear;

                masterModel = dbContext.NQM_TOUR_CLAIM_MASTER.Where(x => x.FINALIZE_FLAG == 2 && x.MONTH_OF_INSPECTION <= currMonth && x.YEAR_OF_INSPECTION <= currYear).ToList();

                foreach (var item in masterModel)
                {
                    DateTime date = new DateTime(2020, item.MONTH_OF_INSPECTION, 1);
                    month = date.ToString("MMM");
                    monitor = dbContext.ADMIN_QUALITY_MONITORS.Where(x => x.ADMIN_QM_CODE == item.ADMIN_QM_CODE).FirstOrDefault();
                    name = ((monitor.ADMIN_QM_FNAME == null || monitor.ADMIN_QM_FNAME == string.Empty) ? " " : monitor.ADMIN_QM_FNAME) + " " + ((monitor.ADMIN_QM_MNAME == null || monitor.ADMIN_QM_MNAME == string.Empty) ? " " : monitor.ADMIN_QM_MNAME) + " " + ((monitor.ADMIN_QM_LNAME == null || monitor.ADMIN_QM_LNAME == string.Empty) ? " " : monitor.ADMIN_QM_LNAME);

                    model.Add(new NQM_TOUR_CLAIM_MASTER_MODEL
                    {
                        MONTH_OF_INSPECTION = month,
                        YEAR_OF_INSPECTION = item.YEAR_OF_INSPECTION,
                        ADMIN_QM_NAME = name,
                        REMARK = string.IsNullOrEmpty(item.CQC_FORWARDED_REMARK) ? "--" : item.CQC_FORWARDED_REMARK
                    });

                }

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.ViewNotificationFin()");
                return null;
            }
        }

        #endregion


        #region District

        [Audit]
        public ActionResult GetTourDistrictListFin1(int scheduleCode, int? page, int? rows, string sidx, string sord)

        {
            try
            {
                long totalRecords = 0;
                ITourClaimBAL objBAL = new TourClaimBAL();

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.GetTourDistrictListFin1BAL(scheduleCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.GetTourDistrictListFin1()");
                return null;
            }
        }

        #endregion


        #region Travel Claim

        [Audit]
        public ActionResult GetTravelClaimListFin1(int scheduleCode, int? page, int? rows, string sidx, string sord)
        {
            try
            {
                long totalRecords = 0;
                ITourClaimBAL objBAL = new TourClaimBAL();

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.GetTravelClaimListFin1BAL(scheduleCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.GetTravelClaimListFin1()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult AddSanctionAmtTravelFin1(int travelId, decimal amount, string remark)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    bool status = false;
                    String message = "Details not updated";
                    String FileName = String.Empty;

                    NQM_TRAVEL_CLAIM_DETAILS travelModel = new NQM_TRAVEL_CLAIM_DETAILS();

                    travelModel = dbContext.NQM_TRAVEL_CLAIM_DETAILS.Where(x => x.TRAVEL_CLAIM_ID == travelId).FirstOrDefault();

                    travelModel.AMOUNT_PASSED_FIN1 = amount;
                    travelModel.REMARK_FIN1 = (remark == "" ? null : remark);
                    travelModel.DATE_OF_MODIFICATION_FIN1 = DateTime.Now;

                    dbContext.Entry(travelModel).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    status = true;
                    message = "Details updated successfully";

                    scope.Complete();

                    return Json(new { success = status, message = message });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.AddSanctionAmtTravelFin1()");
                return Json(new { success = false, message = "Details not updated." });
            }

        }

        #endregion


        #region Lodge and daily Claim

        [Audit]
        public ActionResult GetLodgeClaimListFin1(int scheduleCode, int? page, int? rows, string sidx, string sord)
        {
            try
            {
                long totalRecords = 0;
                ITourClaimBAL objBAL = new TourClaimBAL();

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.GetLodgeClaimListFin1BAL(scheduleCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.GetLodgeClaimListFin1()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult AddSanctionAmtLodgeFin1(int lodgeId, decimal amount, string remark)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    bool status = false;
                    String message = "Details not updated";
                    String FileName = String.Empty;

                    NQM_LODGE_AND_DAILY_CLAIM_DETAILS lodgeModel = new NQM_LODGE_AND_DAILY_CLAIM_DETAILS();

                    lodgeModel = dbContext.NQM_LODGE_AND_DAILY_CLAIM_DETAILS.Where(x => x.LODGE_CLAIM_ID == lodgeId).FirstOrDefault();

                    if (lodgeModel.TYPE_OF_CLAIM.Equals("H") || lodgeModel.TYPE_OF_CLAIM.Equals("G"))
                        lodgeModel.AMOUNT_PASSED_HOTEL_FIN1 = amount;

                    lodgeModel.REMARK_FIN1 = (remark == "" ? null : remark);
                    lodgeModel.DATE_OF_MODIFICATION_FIN1 = DateTime.Now;

                    dbContext.Entry(lodgeModel).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    status = true;
                    message = "Details updated successfully";

                    scope.Complete();

                    return Json(new { success = status, message = message });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.AddSanctionAmtLodgeFin1()");
                return Json(new { success = false, message = "Details not updated." });
            }

        }

        #endregion


        #region Inspection of Road Claim

        [Audit]
        public ActionResult GetInspectionHonorariumListFin1(int scheduleCode, int? page, int? rows, string sidx, string sord)
        {
            try
            {
                long totalRecords = 0;
                ITourClaimBAL objBAL = new TourClaimBAL();

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.GetInspectionHonorariumListFin1BAL(scheduleCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.GetInspectionHonorariumListFin1()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult AddSanctionAmtInspFin1(int inspId, decimal amount, string remark)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    bool status = false;
                    String message = "Details not updated";
                    String FileName = String.Empty;

                    NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS inspModel = new NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS();

                    inspModel = dbContext.NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS.Where(x => x.HONORARIUM_INSPECTION_ID == inspId).FirstOrDefault();

                    inspModel.AMOUNT_PASSED_FIN1 = amount;
                    inspModel.REMARK_FIN1 = (remark == "" ? null : remark);
                    inspModel.DATE_OF_MODIFICATION_FIN1 = DateTime.Now;

                    dbContext.Entry(inspModel).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    status = true;
                    message = "Details updated successfully";

                    scope.Complete();

                    return Json(new { success = status, message = message });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.AddSanctionAmtInspFin1()");
                return Json(new { success = false, message = "Details not updated." });
            }

        }

        #endregion


        #region Meeting with PIU Claim

        [Audit]
        public ActionResult GetMeetingHonorariumListFin1(int scheduleCode, int? page, int? rows, string sidx, string sord)
        {
            try
            {
                long totalRecords = 0;
                ITourClaimBAL objBAL = new TourClaimBAL();

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.GetMeetingHonorariumListFin1BAL(scheduleCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.GetMeetingHonorariumListFin1()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult AddSanctionAmtMeetingFin1(int meetingId, decimal amount, string remark)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    bool status = false;
                    String message = "Details not updated";
                    String FileName = String.Empty;

                    NQM_TOUR_HONORARIUM_MEETING_WITH_PIU meetingModel = new NQM_TOUR_HONORARIUM_MEETING_WITH_PIU();

                    meetingModel = dbContext.NQM_TOUR_HONORARIUM_MEETING_WITH_PIU.Where(x => x.HONORARIUM_MEETING_ID == meetingId).FirstOrDefault();

                    meetingModel.AMOUNT_PASSED_FIN1 = amount;
                    meetingModel.REMARK_FIN1 = (remark == "" ? null : remark);
                    meetingModel.DATE_OF_MODIFICATION_FIN1 = DateTime.Now;

                    dbContext.Entry(meetingModel).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    status = true;
                    message = "Details updated successfully";

                    scope.Complete();

                    return Json(new { success = status, message = message });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.AddSanctionAmtMeetingFin1()");
                return Json(new { success = false, message = "Details not updated." });
            }

        }

        #endregion


        #region Miscellaneous

        [Audit]
        public ActionResult LoadMiscellaneousClaimListFin1(int scheduleCode, int? page, int? rows, string sidx, string sord)
        {
            try
            {
                long totalRecords = 0;
                ITourClaimBAL objBAL = new TourClaimBAL();

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.LoadMiscellaneousClaimListFin1BAL(scheduleCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.LoadMiscellaneousClaimListFin1()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult AddSanctionAmtMiscellaneousFin1(int miscellaneousId, decimal amount, string remark)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    bool status = false;
                    String message = "Details not updated";
                    String FileName = String.Empty;

                    NQM_TOUR_MISCELLANEOUS misModel = new NQM_TOUR_MISCELLANEOUS();

                    misModel = dbContext.NQM_TOUR_MISCELLANEOUS.Where(x => x.MISCELLANEOUS_ID == miscellaneousId).FirstOrDefault();

                    misModel.AMOUNT_PASSED_FIN1 = amount;
                    misModel.REMARK_FIN1 = (remark == "" ? null : remark);
                    misModel.DATE_OF_MODIFICATION_FIN1 = DateTime.Now;

                    dbContext.Entry(misModel).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    status = true;
                    message = "Details updated successfully";

                    scope.Complete();

                    return Json(new { success = status, message = message });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.AddSanctionAmtMiscellaneousFin1()");
                return Json(new { success = false, message = "Details not updated." });
            }

        }

        #endregion


        #region Forward to Finance Approver

        [HttpPost]
        public ActionResult ForwardToFin2(string id)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    bool status = false;
                    String message = "Cannot forward !!";
                    String FileName = String.Empty;
                    int scheduleCode = Convert.ToInt32(id.Split('$')[0]);
                    string officerName = id.Split('$')[1];

                    NQM_TOUR_CLAIM_MASTER model = new NQM_TOUR_CLAIM_MASTER();
                    List<NQM_TRAVEL_CLAIM_DETAILS> travelModel = new List<NQM_TRAVEL_CLAIM_DETAILS>();
                    List<NQM_LODGE_AND_DAILY_CLAIM_DETAILS> lodgeModel = new List<NQM_LODGE_AND_DAILY_CLAIM_DETAILS>();
                    List<NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS> inspModel = new List<NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS>();
                    List<NQM_TOUR_HONORARIUM_MEETING_WITH_PIU> meetingModel = new List<NQM_TOUR_HONORARIUM_MEETING_WITH_PIU>();
                    List<NQM_TOUR_MISCELLANEOUS> misModel = new List<NQM_TOUR_MISCELLANEOUS>();

                    model = dbContext.NQM_TOUR_CLAIM_MASTER.Where(x => x.ADMIN_SCHEDULE_CODE == scheduleCode).FirstOrDefault();

                    model.FIN1_FINALIZE_OFFICE_ASSISTANT_FINANCE = officerName;
                    model.FINALIZE_FLAG = 3; // finalized from Fin1

                    dbContext.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    if(model.ROUNDS_SEQUENCE == 0)
                    {
                        travelModel = dbContext.NQM_TRAVEL_CLAIM_DETAILS.Where(x => x.TOUR_CLAIM_ID == model.TOUR_CLAIM_ID).ToList();
                        foreach (var item in travelModel)
                        {
                            item.AMOUNT_PASSED_FIN2 = item.AMOUNT_PASSED_FIN1;

                            dbContext.Entry(item).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                        }

                        lodgeModel = dbContext.NQM_LODGE_AND_DAILY_CLAIM_DETAILS.Where(x => x.TOUR_CLAIM_ID == model.TOUR_CLAIM_ID).ToList();
                        foreach (var item in lodgeModel)
                        {
                            item.AMOUNT_PASSED_HOTEL_FIN2 = item.AMOUNT_PASSED_HOTEL_FIN1;

                            dbContext.Entry(item).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                        }

                        inspModel = dbContext.NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS.Where(x => x.TOUR_CLAIM_ID == model.TOUR_CLAIM_ID).ToList();
                        foreach (var item in inspModel)
                        {
                            item.AMOUNT_PASSED_FIN2 = item.AMOUNT_PASSED_FIN1;

                            dbContext.Entry(item).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                        }

                        meetingModel = dbContext.NQM_TOUR_HONORARIUM_MEETING_WITH_PIU.Where(x => x.TOUR_CLAIM_ID == model.TOUR_CLAIM_ID).ToList();
                        foreach (var item in meetingModel)
                        {
                            item.AMOUNT_PASSED_FIN2 = item.AMOUNT_PASSED_FIN1;

                            dbContext.Entry(item).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                        }

                        misModel = dbContext.NQM_TOUR_MISCELLANEOUS.Where(x => x.TOUR_CLAIM_ID == model.TOUR_CLAIM_ID).ToList();
                        foreach (var item in misModel)
                        {
                            item.AMOUNT_PASSED_FIN2 = item.AMOUNT_PASSED_FIN1;

                            dbContext.Entry(item).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                        }
                    }

                    status = true;
                    message = "Details forwarded to approver.";

                    scope.Complete();

                    return Json(new { success = status, message = message, month = model.MONTH_OF_INSPECTION, year = model.YEAR_OF_INSPECTION });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.ForwardToFin2()");
                return Json(new { success = false, message = "Cannot forward !!", month = DateTime.Now.Month, year = DateTime.Now.Year });
            }

        }

        #endregion

        #endregion

        #region  Finance 2 Tour Claim

        #region Main Form

        public ActionResult ViewFinance2Filter()
        {
            try
            {
                NQM_TOUR_CLAIM_MASTER_MODEL qmFilterModel = new NQM_TOUR_CLAIM_MASTER_MODEL();
                CommonFunctions objCommonFunctions = new CommonFunctions();

                qmFilterModel.FROM_MONTH = DateTime.Now.Month;
                qmFilterModel.FROM_YEAR = DateTime.Now.Year;
                qmFilterModel.FROM_MONTHS_LIST = objCommonFunctions.PopulateMonths(false);
                qmFilterModel.FROM_YEARS_LIST = objCommonFunctions.PopulateYears(false).Where(a => a.Value != "0").ToList();
                if (DateTime.Now.Month == 12)
                {
                    string nextYear = (DateTime.Now.Year + 1).ToString();
                    qmFilterModel.FROM_YEARS_LIST.Add(new SelectListItem { Text = nextYear, Value = nextYear });
                    qmFilterModel.FROM_YEARS_LIST = qmFilterModel.FROM_YEARS_LIST.OrderByDescending(c => c.Value).ToList();
                }
                return View(qmFilterModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.ViewFinance2Filter()");
                return null;
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult GetFinance2MonitorList(FormCollection formCollection)
        {
            ITourClaimBAL objBAL = new TourClaimBAL();
            int totalRecords;
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.GetFinance2MonitorListBAL(Convert.ToInt32(formCollection["month"]), Convert.ToInt32(formCollection["year"]), Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.GetFinance2MonitorList()");
                return Json(String.Empty);
            }
        }

        public ActionResult ViewEditTourFinance2(int tourClaimId)
        {
            try
            {
                NQM_TOUR_CLAIM_MASTER_MODEL model = new NQM_TOUR_CLAIM_MASTER_MODEL();
                NQM_TOUR_CLAIM_MASTER masterModel = new NQM_TOUR_CLAIM_MASTER();
                ADMIN_QUALITY_MONITORS_BANK bankModel = new ADMIN_QUALITY_MONITORS_BANK();
                ADMIN_QUALITY_MONITORS adminMonitor = new ADMIN_QUALITY_MONITORS();

                masterModel = dbContext.NQM_TOUR_CLAIM_MASTER.Where(x => x.TOUR_CLAIM_ID == tourClaimId).FirstOrDefault();
                adminMonitor = dbContext.ADMIN_QUALITY_MONITORS.Where(x => x.ADMIN_QM_CODE == masterModel.ADMIN_QM_CODE).FirstOrDefault();
                bankModel = dbContext.ADMIN_QUALITY_MONITORS_BANK.Where(x => x.ADMIN_QM_CODE == masterModel.ADMIN_QM_CODE).FirstOrDefault();

                List<TOUR_CLAIM_CALCULATION_Result> lstExecution = new List<TOUR_CLAIM_CALCULATION_Result>();

                var item = dbContext.TOUR_CLAIM_CALCULATION(tourClaimId, null, null).FirstOrDefault();

                model.TOUR_CLAIM_ID = tourClaimId;
                model.ADMIN_SCHEDULE_CODE = masterModel.ADMIN_SCHEDULE_CODE;

                model.ADMIN_QM_NAME = (adminMonitor.ADMIN_QM_FNAME == null ? "" : adminMonitor.ADMIN_QM_FNAME) + " " + (adminMonitor.ADMIN_QM_MNAME == null ? "" : adminMonitor.ADMIN_QM_MNAME) + " " + (adminMonitor.ADMIN_QM_LNAME == null ? "" : adminMonitor.ADMIN_QM_LNAME);
                model.NRRDA_LETTER_NUMBER = masterModel.NRRDA_LETTER_NUMBER;
                model.ADMIN_QM_PAN = adminMonitor.ADMIN_QM_PAN == null ? "--" : adminMonitor.ADMIN_QM_PAN;

                model.BANK_NAME = bankModel.MAST_BANK_NAME;
                model.ACCOUNT_NUMBER = bankModel.MAST_ACCOUNT_NUMBER;
                model.IFSC_CODE = bankModel.MAST_IFSC_CODE;
                model.finalizeFlag = masterModel.FINALIZE_FLAG;
                model.ROUND_SEQUENCE = masterModel.ROUNDS_SEQUENCE;

                model.TOTAL_AMOUNT_CLAIMED = item.DISTRICT_VISITED_ALLOWANCE + 200 + item.TOTAL_TRAVEL_CLAIM_AMOUNT + item.TOTAL_LODGE_CLAIM_AMOUNT + item.TOTAL_DAILY_CLAIM_AMOUNT + item.TOTAL_INSPECTION_CLAIM_AMOUNT + item.TOTAL_MEETING_CLAIM_AMOUNT + item.TOTAL_MISCELLANEOUS_CLAIM_AMOUNT;
                model.TOTAL_AMOUNT_SANCTIONED = (item.DISTRICT_VISITED_ALLOWANCE + item.TOTAL_TRAVEL_SANCTIONED_AMOUNT + item.TOTAL_LODGE_SANCTIONED_AMOUNT + item.TOTAL_DAILY_SANCTIONED_AMOUNT + item.TOTAL_INSPECTION_SANCTIONED_AMOUNT + item.TOTAL_MEETING_SANCTIONED_AMOUNT + item.TOTAL_MISCELLANEOUS_SANCTIONED_AMOUNT + 200).ToString();
                model.TOTAL_AMOUNT_PASSED_FIN1 = (item.DISTRICT_VISITED_ALLOWANCE + item.TOTAL_TRAVEL_PASSED_AMOUNT_FIN1 + item.TOTAL_LODGE_PASSED_AMOUNT_FIN1 + item.TOTAL_DAILY_SANCTIONED_AMOUNT + item.TOTAL_INSPECTION_PASSED_AMOUNT_FIN1 + item.TOTAL_MEETING_PASSED_AMOUNT_FIN1 + item.TOTAL_MISCELLANEOUS_PASSED_AMOUNT_FIN1 + 200).ToString();

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.ViewEditTourFinance2()");
                return null;
            }
        }

        public ActionResult ViewNotificationFin2(int currMonth = 0, int currYear = 0)
        {
            try
            {
                List<NQM_TOUR_CLAIM_MASTER> masterModel = new List<NQM_TOUR_CLAIM_MASTER>();
                List<NQM_TOUR_CLAIM_MASTER_MODEL> model = new List<NQM_TOUR_CLAIM_MASTER_MODEL>();
                ADMIN_QUALITY_MONITORS monitor = new ADMIN_QUALITY_MONITORS();
                string month;
                string name;

                CommonFunctions objCommonFunctions = new CommonFunctions();

                if (currMonth == 0)
                    currMonth = DateTime.Now.Month;
                if (currYear == 0)
                    currYear = DateTime.Now.Year;

                List<SelectListItem> monthList = new List<SelectListItem>();
                List<SelectListItem> yearList = new List<SelectListItem>();
                monthList = objCommonFunctions.PopulateMonths(false);
                yearList = objCommonFunctions.PopulateYears(false).Where(a => a.Value != "0").ToList();
                if (DateTime.Now.Month == 12)
                {
                    string nextYear = (DateTime.Now.Year + 1).ToString();
                    yearList.Add(new SelectListItem { Text = nextYear, Value = nextYear });
                    yearList = yearList.OrderByDescending(c => c.Value).ToList();
                }

                ViewBag.MonthList = monthList;
                ViewBag.YearList = yearList;
                ViewBag.Month = currMonth;
                ViewBag.Year = currYear;

                masterModel = dbContext.NQM_TOUR_CLAIM_MASTER.Where(x => x.FINALIZE_FLAG == 3 && x.MONTH_OF_INSPECTION <= currMonth && x.YEAR_OF_INSPECTION <= currYear).ToList();

                foreach (var item in masterModel)
                {
                    DateTime date = new DateTime(2020, item.MONTH_OF_INSPECTION, 1);
                    month = date.ToString("MMM");
                    monitor = dbContext.ADMIN_QUALITY_MONITORS.Where(x => x.ADMIN_QM_CODE == item.ADMIN_QM_CODE).FirstOrDefault();
                    name = ((monitor.ADMIN_QM_FNAME == null || monitor.ADMIN_QM_FNAME == string.Empty) ? " " : monitor.ADMIN_QM_FNAME) + " " + ((monitor.ADMIN_QM_MNAME == null || monitor.ADMIN_QM_MNAME == string.Empty) ? " " : monitor.ADMIN_QM_MNAME) + " " + ((monitor.ADMIN_QM_LNAME == null || monitor.ADMIN_QM_LNAME == string.Empty) ? " " : monitor.ADMIN_QM_LNAME);

                    model.Add(new NQM_TOUR_CLAIM_MASTER_MODEL
                    {
                        MONTH_OF_INSPECTION = month,
                        YEAR_OF_INSPECTION = item.YEAR_OF_INSPECTION,
                        ADMIN_QM_NAME = name
                    });

                }

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.ViewNotificationFin2()");
                return null;
            }
        }

        #endregion 


        #region District

        [Audit]
        public ActionResult GetTourDistrictListFin2(int scheduleCode, int? page, int? rows, string sidx, string sord)

        {
            try
            {
                long totalRecords = 0;
                ITourClaimBAL objBAL = new TourClaimBAL();

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.GetTourDistrictListFin2BAL(scheduleCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "tourClaim.GetTourDistrictListFin2()");
                return null;
            }
        }

        #endregion


        #region Travel Claim

        [Audit]
        public ActionResult GetTravelClaimListFin2(int scheduleCode, int? page, int? rows, string sidx, string sord)
        {
            try
            {
                long totalRecords = 0;
                ITourClaimBAL objBAL = new TourClaimBAL();

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.GetTravelClaimListFin2BAL(scheduleCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.GetTravelClaimListFin2()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult AddSanctionAmtTravelFin2(int travelId, decimal amount, string remark)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    bool status = false;
                    String message = "Details not updated";
                    String FileName = String.Empty;

                    NQM_TRAVEL_CLAIM_DETAILS travelModel = new NQM_TRAVEL_CLAIM_DETAILS();

                    travelModel = dbContext.NQM_TRAVEL_CLAIM_DETAILS.Where(x => x.TRAVEL_CLAIM_ID == travelId).FirstOrDefault();

                    travelModel.AMOUNT_PASSED_FIN2 = amount;
                    travelModel.REMARK_FIN2 = (remark == "" ? null : remark);
                    travelModel.DATE_OF_MODIFICATION_FIN2 = DateTime.Now;

                    dbContext.Entry(travelModel).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    status = true;
                    message = "Details updated successfully";

                    scope.Complete();

                    return Json(new { success = status, message = message });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.AddSanctionAmtTravelFin2()");
                return Json(new { success = false, message = "Details not updated." });
            }

        }

        #endregion


        #region Lodge and daily Claim

        [Audit]
        public ActionResult GetLodgeClaimListFin2(int scheduleCode, int? page, int? rows, string sidx, string sord)
        {
            try
            {
                long totalRecords = 0;
                ITourClaimBAL objBAL = new TourClaimBAL();

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.GetLodgeClaimListFin2BAL(scheduleCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.GetLodgeClaimListFin2()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult AddSanctionAmtLodgeFin2(int lodgeId, decimal amount, string remark)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    bool status = false;
                    String message = "Details not updated";
                    String FileName = String.Empty;

                    NQM_LODGE_AND_DAILY_CLAIM_DETAILS lodgeModel = new NQM_LODGE_AND_DAILY_CLAIM_DETAILS();

                    lodgeModel = dbContext.NQM_LODGE_AND_DAILY_CLAIM_DETAILS.Where(x => x.LODGE_CLAIM_ID == lodgeId).FirstOrDefault();

                    if (lodgeModel.TYPE_OF_CLAIM.Equals("H") || lodgeModel.TYPE_OF_CLAIM.Equals("G"))
                        lodgeModel.AMOUNT_PASSED_HOTEL_FIN2 = amount;

                    lodgeModel.REMARK_FIN2 = (remark == "" ? null : remark);
                    lodgeModel.DATE_OF_MODIFICATION_FIN2 = DateTime.Now;

                    dbContext.Entry(lodgeModel).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    status = true;
                    message = "Details updated successfully";

                    scope.Complete();

                    return Json(new { success = status, message = message });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.AddSanctionAmtLodgeFin2()");
                return Json(new { success = false, message = "Details not updated." });
            }

        }

        #endregion


        #region Inspection of Road Claim

        [Audit]
        public ActionResult GetInspectionHonorariumListFin2(int scheduleCode, int? page, int? rows, string sidx, string sord)
        {
            try
            {
                long totalRecords = 0;
                ITourClaimBAL objBAL = new TourClaimBAL();

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.GetInspectionHonorariumListFin2BAL(scheduleCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.GetInspectionHonorariumListFin2()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult AddSanctionAmtInspFin2(int inspId, decimal amount, string remark)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    bool status = false;
                    String message = "Details not updated";
                    String FileName = String.Empty;

                    NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS inspModel = new NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS();

                    inspModel = dbContext.NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS.Where(x => x.HONORARIUM_INSPECTION_ID == inspId).FirstOrDefault();

                    inspModel.AMOUNT_PASSED_FIN2 = amount;
                    inspModel.REMARK_FIN2 = (remark == "" ? null : remark);
                    inspModel.DATE_OF_MODIFICATION_FIN2 = DateTime.Now;

                    dbContext.Entry(inspModel).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    status = true;
                    message = "Details updated successfully";

                    scope.Complete();

                    return Json(new { success = status, message = message });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.AddSanctionAmtInspFin2()");
                return Json(new { success = false, message = "Details not updated." });
            }

        }

        #endregion


        #region Meeting with PIU Claim

        [Audit]
        public ActionResult GetMeetingHonorariumListFin2(int scheduleCode, int? page, int? rows, string sidx, string sord)
        {
            try
            {
                long totalRecords = 0;
                ITourClaimBAL objBAL = new TourClaimBAL();

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.GetMeetingHonorariumListFin2BAL(scheduleCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.GetMeetingHonorariumListFin2()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult AddSanctionAmtMeetingFin2(int meetingId, decimal amount, string remark)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    bool status = false;
                    String message = "Details not updated";
                    String FileName = String.Empty;

                    NQM_TOUR_HONORARIUM_MEETING_WITH_PIU meetingModel = new NQM_TOUR_HONORARIUM_MEETING_WITH_PIU();

                    meetingModel = dbContext.NQM_TOUR_HONORARIUM_MEETING_WITH_PIU.Where(x => x.HONORARIUM_MEETING_ID == meetingId).FirstOrDefault();

                    meetingModel.AMOUNT_PASSED_FIN2 = amount;
                    meetingModel.REMARK_FIN2 = (remark == "" ? null : remark);
                    meetingModel.DATE_OF_MODIFICATION_FIN2 = DateTime.Now;

                    dbContext.Entry(meetingModel).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    status = true;
                    message = "Details updated successfully";

                    scope.Complete();

                    return Json(new { success = status, message = message });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.AddSanctionAmtMeetingFin2()");
                return Json(new { success = false, message = "Details not updated." });
            }
        }

        #endregion


        #region Miscellaneous

        [Audit]
        public ActionResult LoadMiscellaneousClaimListFin2(int scheduleCode, int? page, int? rows, string sidx, string sord)
        {
            try
            {
                long totalRecords = 0;
                ITourClaimBAL objBAL = new TourClaimBAL();

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.LoadMiscellaneousClaimListFin2BAL(scheduleCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.LoadMiscellaneousClaimListFin2()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult AddSanctionAmtMiscellaneousFin2(int miscellaneousId, decimal amount, string remark)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    bool status = false;
                    String message = "Details not updated";
                    String FileName = String.Empty;

                    NQM_TOUR_MISCELLANEOUS misModel = new NQM_TOUR_MISCELLANEOUS();

                    misModel = dbContext.NQM_TOUR_MISCELLANEOUS.Where(x => x.MISCELLANEOUS_ID == miscellaneousId).FirstOrDefault();

                    misModel.AMOUNT_PASSED_FIN2 = amount;
                    misModel.REMARK_FIN2 = (remark == "" ? null : remark);
                    misModel.DATE_OF_MODIFICATION_FIN2 = DateTime.Now;

                    dbContext.Entry(misModel).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    status = true;
                    message = "Details updated successfully";

                    scope.Complete();

                    return Json(new { success = status, message = message });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.AddSanctionAmtMiscellaneousFin2()");
                return Json(new { success = false, message = "Details not updated." });
            }
        }

        #endregion


        #region Approve or Revert to CQC

        public ActionResult ApproveForwardFilter(int scheduleCode)
        {
            try
            {
                NQM_TOUR_CLAIM_MASTER_MODEL model = new NQM_TOUR_CLAIM_MASTER_MODEL();
                NQM_TOUR_CLAIM_MASTER masterModel = new NQM_TOUR_CLAIM_MASTER();

                masterModel = dbContext.NQM_TOUR_CLAIM_MASTER.Where(x => x.ADMIN_SCHEDULE_CODE == scheduleCode).FirstOrDefault();

                model.TOUR_CLAIM_ID = masterModel.TOUR_CLAIM_ID;
                model.ADMIN_SCHEDULE_CODE = masterModel.ADMIN_SCHEDULE_CODE;
                model.finalizeFlag = masterModel.FINALIZE_FLAG;
                model.ROUND_SEQUENCE = masterModel.ROUNDS_SEQUENCE;
                model.ASSISTANT_DIRECTOR = string.IsNullOrEmpty(masterModel.FIN2_FINALIZE_ASSISTANT_DIRECTOR) ? "Jitender Jha, Assistant Director (F&A)" : masterModel.FIN2_FINALIZE_ASSISTANT_DIRECTOR;

                List<NQM_TRAVEL_CLAIM_DETAILS> travelList = new List<NQM_TRAVEL_CLAIM_DETAILS>();
                List<NQM_LODGE_AND_DAILY_CLAIM_DETAILS> lodgeList = new List<NQM_LODGE_AND_DAILY_CLAIM_DETAILS>();
                List<NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS> inspList = new List<NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS>();
                List<NQM_TOUR_HONORARIUM_MEETING_WITH_PIU> meetingList = new List<NQM_TOUR_HONORARIUM_MEETING_WITH_PIU>();
                List<NQM_TOUR_MISCELLANEOUS> misList = new List<NQM_TOUR_MISCELLANEOUS>();

                List<string> remarks = new List<string>();

                travelList = dbContext.NQM_TRAVEL_CLAIM_DETAILS.Where(x => x.TOUR_CLAIM_ID == masterModel.TOUR_CLAIM_ID).ToList();

                if (travelList != null)
                {
                    foreach (var item in travelList)
                    {
                        if (!string.IsNullOrEmpty(item.REMARK_FIN2))
                            remarks.Add(item.REMARK_FIN2);
                    }
                }

                ViewBag.travelList = remarks;
                remarks = new List<string>();

                lodgeList = dbContext.NQM_LODGE_AND_DAILY_CLAIM_DETAILS.Where(x => x.TOUR_CLAIM_ID == masterModel.TOUR_CLAIM_ID).ToList();

                if (lodgeList != null)
                {
                    foreach (var item in lodgeList)
                    {
                        if (!string.IsNullOrEmpty(item.REMARK_FIN2))
                            remarks.Add(item.REMARK_FIN2);
                    }
                }

                ViewBag.lodgeList = remarks;
                remarks = new List<string>();

                inspList = dbContext.NQM_TOUR_HONORARIUM_INSPECTION_OF_ROADS.Where(x => x.TOUR_CLAIM_ID == masterModel.TOUR_CLAIM_ID).ToList();

                if (inspList != null)
                {
                    foreach (var item in inspList)
                    {
                        if (!string.IsNullOrEmpty(item.REMARK_FIN2))
                            remarks.Add(item.REMARK_FIN2);
                    }
                }

                ViewBag.inspList = remarks;
                remarks = new List<string>();

                meetingList = dbContext.NQM_TOUR_HONORARIUM_MEETING_WITH_PIU.Where(x => x.TOUR_CLAIM_ID == masterModel.TOUR_CLAIM_ID).ToList();

                if (meetingList != null)
                {
                    foreach (var item in meetingList)
                    {
                        if (!string.IsNullOrEmpty(item.REMARK_FIN2))
                            remarks.Add(item.REMARK_FIN2);
                    }
                }

                ViewBag.meetingList = remarks;
                remarks = new List<string>();

                misList = dbContext.NQM_TOUR_MISCELLANEOUS.Where(x => x.TOUR_CLAIM_ID == masterModel.TOUR_CLAIM_ID).ToList();

                if (misList != null)
                {
                    foreach (var item in misList)
                    {
                        if (!string.IsNullOrEmpty(item.REMARK_FIN2))
                            remarks.Add(item.REMARK_FIN2);
                    }
                }

                ViewBag.misList = remarks;
                remarks = new List<string>();

                return PartialView(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.ApproveForwardFilter()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult ApproveByFinance2(FormCollection formCollection)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    bool status = false;
                    String message = "Cannot approve !!";
                    String FileName = String.Empty;
                    int tourClaimId = Convert.ToInt32(formCollection["TOUR_CLAIM_ID"]);

                    NQM_TOUR_CLAIM_MASTER model = new NQM_TOUR_CLAIM_MASTER();

                    model = dbContext.NQM_TOUR_CLAIM_MASTER.Where(x => x.TOUR_CLAIM_ID == tourClaimId).FirstOrDefault();

                    model.FINALIZE_FLAG = 4; // finalized from Fin2
                    model.ROUNDS_SEQUENCE = 1;
                    model.FIN2_APPROVE_DATE = DateTime.Now;
                    model.FIN2_APPROVE_REMARK = formCollection["REMARK"] == "" ? null : formCollection["REMARK"];
                    model.FIN2_FINALIZE_ASSISTANT_DIRECTOR = formCollection["ASSISTANT_DIRECTOR"];

                    dbContext.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    status = true;
                    message = "Details approved.";

                    scope.Complete();

                    return Json(new { success = status, message = message, month = model.MONTH_OF_INSPECTION, year = model.YEAR_OF_INSPECTION });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.ApproveByFinance2()");
                return Json(new { success = false, message = "Cannot approve !!", month = DateTime.Now.Month, year = DateTime.Now.Year });
            }

        }

        [HttpPost]
        public ActionResult RevertToCQC(FormCollection formCollection)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    bool status = false;
                    String message = "Cannot revert !!";
                    String FileName = String.Empty;
                    int tourClaimId = Convert.ToInt32(formCollection["TOUR_CLAIM_ID"]);
                    string revertRemark = formCollection["REMARK"];

                    NQM_TOUR_CLAIM_MASTER model = new NQM_TOUR_CLAIM_MASTER();

                    model = dbContext.NQM_TOUR_CLAIM_MASTER.Where(x => x.TOUR_CLAIM_ID == tourClaimId).FirstOrDefault();

                    if (string.IsNullOrWhiteSpace(revertRemark))
                    {
                        status = false;
                        message = "Remark is required.";
                    }
                    else
                    {
                        model.FINALIZE_FLAG = 1;
                        model.ROUNDS_SEQUENCE = 1;
                        model.FIN2_REVERTE_DATE = DateTime.Now;
                        model.FIN2_REVERTE_REMARK = formCollection["REMARK"];
                        model.FIN2_FINALIZE_ASSISTANT_DIRECTOR = formCollection["ASSISTANT_DIRECTOR"];

                        dbContext.Entry(model).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();

                        status = true;
                        message = "Details reverted.";

                        scope.Complete();
                    }

                    return Json(new { success = status, message = message, month = model.MONTH_OF_INSPECTION, year = model.YEAR_OF_INSPECTION });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TourClaim.RevertToCQC()");
                return Json(new { success = false, message = "Cannot revert !!", month = DateTime.Now.Month, year = DateTime.Now.Year });
            }

        }

        #endregion

        #endregion
    }
}