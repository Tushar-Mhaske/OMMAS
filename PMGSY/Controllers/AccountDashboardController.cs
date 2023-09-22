using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Models;
using PMGSY.Models.AccountDashboard;
using PMGSY.Extensions;
using PMGSY.Common;


namespace PMGSY.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class AccountDashboardController : Controller
    {
        //
        // GET: /AccountDashboad/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Account Dashboard Layout Page.
        /// </summary>
        /// <returns></returns>
        public ActionResult AccountDashboardLayout()
        {
            try
            {
                CommonFunctions objCommFunction = new CommonFunctions();
                AccountDashboardModel dashboardModel = new AccountDashboardModel();

                //Populate SRRDA
                List<SelectListItem> lstSRRDA = new List<SelectListItem>();
                lstSRRDA = PopulateSRRDA();
                lstSRRDA.RemoveAt(0);
                lstSRRDA.Insert(0, new SelectListItem { Text = "All State", Value = "0" });

                if (PMGSYSession.Current.LevelId == 4)
                {
                    var selected = lstSRRDA.Where(x => x.Value == PMGSYSession.Current.AdminNdCode.ToString()).ToList<SelectListItem>();
                    //selected.Selected = true;                    
                    lstSRRDA = selected;
                }
                dashboardModel.lstState = lstSRRDA;
                //Populate DPIU
                List<SelectListItem> lstDpiu = objCommFunction.PopulateDPIUOfSRRDA(0);
                lstDpiu.RemoveAt(0);
                lstDpiu.Insert(0, new SelectListItem { Text = "All DPIU", Value = "0", Selected = true });
                dashboardModel.lstDPIU = lstDpiu;

                //populate Fund Type
                dashboardModel.lstFundType = PopulateFundType();
                dashboardModel.FundType = PMGSYSession.Current.FundType == null ? "P" : PMGSYSession.Current.FundType;

                //populate Month
                //dashboardModel.lstMonth = objCommFunction.PopulateMonths(true);

                //Populate Year
                //dashboardModel.lstYear = objCommFunction.PopulateYears(true);
                dashboardModel.EncryptedProgramme = URLEncrypt.EncryptParameters(new string[] { "P" });
                dashboardModel.EncryptedAdminFund = URLEncrypt.EncryptParameters(new String[] { "A" });
                dashboardModel.EncryptedMaintenance = URLEncrypt.EncryptParameters(new String[] { "M" });
                return View(dashboardModel);
            }
            catch (Exception)
            {
                return View();
            }
        }


        /// <summary>
        /// Populate DPIu function
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult PopulateDPIU(string id)
        {
            List<SelectListItem> lstDPIU = new List<SelectListItem>();
            try
            {
                if (!(String.IsNullOrEmpty(id)))
                {
                    CommonFunctions objComFunc = new CommonFunctions();
                    lstDPIU = objComFunc.PopulateDPIUOfSRRDA(Convert.ToInt32(id));
                    lstDPIU.Insert(0, new SelectListItem { Text = "All DPIU", Value = "0" });
                }
                else
                {
                    lstDPIU.Insert(0, new SelectListItem { Text = "All DPIU", Value = "0" });
                }
                return Json(lstDPIU);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Fund Vs Expenditure Column chart
        /// </summary>
        /// <param name="dashboardModel"></param>
        /// <returns></returns>
        public ActionResult FundExpenditureColumnChart(AccountDashboardModel dashboardModel)
        {
            try
            {
                return new JsonResult { Data = FundExpenditureData(dashboardModel) };
            }
            catch (Exception)
            {
                return null;
            }
        }


        public ActionResult FundExpenditureGrid(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                AccountDashboardModel dashboardModel = new AccountDashboardModel();
                dashboardModel.AgencyCode = Convert.ToInt32(Request.Params["AgencyCode"]);
                dashboardModel.DPIU = Convert.ToInt32(Request.Params["DPIU"]);
                dashboardModel.FundType = Request.Params["FundType"];

                List<USP_ACC_MIS_FUND_EXPENDITURE_Result> lstExpenditureList = FundExpenditureData(dashboardModel);

                int totalRecords = lstExpenditureList.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "YEAR_ID":
                                lstExpenditureList = lstExpenditureList.OrderBy(x => x.YEAR_ID).ToList();
                                break;
                            case "YEARLY_INCOME":
                                lstExpenditureList = lstExpenditureList.OrderBy(x => x.YEARLY_INCOME).ToList();
                                break;
                            case "YEARLY_EXPN":
                                lstExpenditureList = lstExpenditureList.OrderBy(x => x.YEARLY_EXPN).ToList();
                                break;
                            default:
                                lstExpenditureList = lstExpenditureList.OrderBy(x => x.YEAR_ID).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "YEAR_ID":
                                lstExpenditureList = lstExpenditureList.OrderByDescending(x => x.YEAR_ID).ToList();
                                break;
                            case "YEARLY_INCOME":
                                lstExpenditureList = lstExpenditureList.OrderByDescending(x => x.YEARLY_INCOME).ToList();
                                break;
                            case "YEARLY_EXPN":
                                lstExpenditureList = lstExpenditureList.OrderByDescending(x => x.YEARLY_EXPN).ToList();
                                break;
                            default:
                                lstExpenditureList = lstExpenditureList.OrderByDescending(x => x.YEAR_ID).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstExpenditureList = lstExpenditureList.OrderByDescending(x => x.YEAR_ID).ToList();
                }

                var data = lstExpenditureList.Select(item => new
                {

                    cell = new[] 
                    {                                                   
                            item.YEAR_TEXT,
                            item.YEARLY_INCOME.ToString(),
                            item.YEARLY_EXPN.ToString()
                    }
                }).ToArray();

                var jsonData = new
                {
                    rows = data,
                    //total = 0,
                    total = totalRecords <= rows ? 1 : totalRecords / ((rows) + 1),
                    page = page,
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<USP_ACC_MIS_FUND_EXPENDITURE_Result> FundExpenditureData(AccountDashboardModel dashboardModel)
        {
            PMGSYEntities dbContext = new PMGSYEntities();

            try
            {
                int AdminNdCode = PMGSYSession.Current.LevelId == 5 ? PMGSYSession.Current.AdminNdCode : (dashboardModel.DPIU == 0 ? dashboardModel.AgencyCode : dashboardModel.DPIU);
                int levelID = PMGSYSession.Current.LevelId == 5 ? PMGSYSession.Current.LevelId : (dashboardModel.AgencyCode == 0 ? 6 : (dashboardModel.DPIU == 0 ? 4 : 5));
                List<USP_ACC_MIS_FUND_EXPENDITURE_Result> lstExpenditureList = dbContext.USP_ACC_MIS_FUND_EXPENDITURE(dashboardModel.FundType, levelID, AdminNdCode).OrderBy(o => o.YEAR_ID).ToList<USP_ACC_MIS_FUND_EXPENDITURE_Result>();
                return lstExpenditureList;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        [HttpPost]
        public ActionResult DeductionRemttancesColumnStackChart(AccountDashboardModel dashboardModel, String id)
        {
            try
            {
                List<String> lstYearText = new List<string>();
                return Json(new
                {
                    DeductionRemittanceData = DeductionRemttancesData(dashboardModel, id, ref lstYearText),
                    lstYearText = lstYearText
                });
            }
            catch (Exception)
            {
                return null;
            }

        }

        public ActionResult DeductionRemittancesGrid(int? page, int? rows, string sidx, string sord)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                AccountDashboardModel dashboardModel = new AccountDashboardModel();
                dashboardModel.AgencyCode = Convert.ToInt32(Request.Params["AgencyCode"]);
                dashboardModel.DPIU = Convert.ToInt32(Request.Params["DPIU"]);
                dashboardModel.FundType = Request.Params["FundType"];
                String id = Request.Params["Year"];

                int AdminNdCode = PMGSYSession.Current.LevelId == 5 ? PMGSYSession.Current.AdminNdCode : (dashboardModel.DPIU == 0 ? dashboardModel.AgencyCode : dashboardModel.DPIU);
                int levelID = PMGSYSession.Current.LevelId == 5 ? PMGSYSession.Current.LevelId : (dashboardModel.AgencyCode == 0 ? 6 : (dashboardModel.DPIU == 0 ? 4 : 5));

                int Year = 0;

                if (!(String.IsNullOrEmpty(id)))
                {
                    Year = Convert.ToInt32(id.Split('-')[0]);
                }

                var lstDeductionRemittanceList = dbContext.USP_ACC_MIS_DEDUCTION_REMITTANCES(dashboardModel.FundType, levelID, AdminNdCode, Year).ToList();

                int totalRecords = lstDeductionRemittanceList.Count();

                var data = lstDeductionRemittanceList.Select(item => new
                {
                    cell = new[] 
                    {                                                   
                        item.YEAR_ID.ToString(),
                        item.DRType.Trim().ToUpper()=="D"?"Deduction":"Remittance",
                        item.YEAR_TEXT,
                        item.HEAD_NAME,
                        item.DED_AMOUNT.ToString()
                    }
                }).ToArray();

                var jsonData = new
                {
                    rows = data,
                    //total = 0,
                    total = totalRecords <= rows ? 1 : totalRecords / ((rows) + 1),
                    page = page,
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }

        public List<DeductionVsRemittanceChartModel> DeductionRemttancesData(AccountDashboardModel dashboardModel, String id, ref List<String> lstYearText)
        {
            PMGSYEntities dbContext = new PMGSYEntities();

            try
            {
                int AdminNdCode = PMGSYSession.Current.LevelId == 5 ? PMGSYSession.Current.AdminNdCode : (dashboardModel.DPIU == 0 ? dashboardModel.AgencyCode : dashboardModel.DPIU);
                int levelID = PMGSYSession.Current.LevelId == 5 ? PMGSYSession.Current.LevelId : (dashboardModel.AgencyCode == 0 ? 6 : (dashboardModel.DPIU == 0 ? 4 : 5));

                int Year = 0;

                if (!(String.IsNullOrEmpty(id)))
                {
                    Year = Convert.ToInt32(id.Split('-')[0]);
                }

                var lstDeductionRemittanceList = dbContext.USP_ACC_MIS_DEDUCTION_REMITTANCES(dashboardModel.FundType, levelID, AdminNdCode, Year).ToList();

                List<short?> lstDeductionHeadId = lstDeductionRemittanceList.Where(m => m.DRType == "D").Select(s => s.HEAD_ID).Distinct().OrderBy(o => o.Value).ToList<short?>();
                List<short?> lstRemittanceHeadID = lstDeductionRemittanceList.Where(m => m.DRType == "R").Select(s => s.HEAD_ID).Distinct().ToList<short?>();
                List<int> lstYearID = lstDeductionRemittanceList.Select(s => s.YEAR_ID).Distinct().ToList<int>();

                lstYearText = lstDeductionRemittanceList.Select(s => s.YEAR_ID.ToString()).Distinct().ToList<String>();

                List<DeductionVsRemittanceChartModel> DeductionRemitanceList = new List<DeductionVsRemittanceChartModel>();

                //Deduction Details
                foreach (var item in lstDeductionHeadId)
                {
                    DeductionVsRemittanceChartModel DRModel = new DeductionVsRemittanceChartModel();

                    var DedDetailsYearly = lstDeductionRemittanceList.Where(m => m.HEAD_ID == item.Value).ToList();
                    DRModel.HeadArrayDeductionsRemiAmount = new List<decimal?>();

                    foreach (var year in lstYearID)
                    {
                        if (DedDetailsYearly.Where(m => m.YEAR_ID == year).Any())
                        {
                            decimal? dedAmt = DedDetailsYearly.Where(m => m.YEAR_ID == year).Select(s => s.DED_AMOUNT).FirstOrDefault();
                            DRModel.HeadArrayDeductionsRemiAmount.Add((dedAmt == 0 ? null : dedAmt));
                        }
                        else
                        {
                            DRModel.HeadArrayDeductionsRemiAmount.Add(0);
                        }
                    }

                    DRModel.HeadName = lstDeductionRemittanceList.Where(m => m.HEAD_ID == item.Value).Select(s => s.HEAD_NAME).FirstOrDefault();
                    DRModel.StatckName = "Deduction";
                    DeductionRemitanceList.Add(DRModel);
                }
                //Remittance Details
                foreach (var item in lstRemittanceHeadID)
                {
                    DeductionVsRemittanceChartModel DRModel = new DeductionVsRemittanceChartModel();

                    DRModel.HeadArrayDeductionsRemiAmount = new List<decimal?>();

                    var RemDetailsYearly = lstDeductionRemittanceList.Where(m => m.HEAD_ID == item.Value).ToList();

                    foreach (var year in lstYearID)
                    {
                        if (RemDetailsYearly.Where(m => m.YEAR_ID == year).Any())
                        {
                            decimal? remAmt = RemDetailsYearly.Where(m => m.YEAR_ID == year).Select(s => s.DED_AMOUNT).FirstOrDefault();
                            DRModel.HeadArrayDeductionsRemiAmount.Add(remAmt == 0 ? null : remAmt);
                        }
                        else
                        {
                            DRModel.HeadArrayDeductionsRemiAmount.Add(0);
                        }
                    }

                    DRModel.HeadName = lstDeductionRemittanceList.Where(m => m.HEAD_ID == item.Value).Select(s => s.HEAD_NAME).FirstOrDefault();
                    DRModel.StatckName = "Remittance";
                    DeductionRemitanceList.Add(DRModel);
                }
                return DeductionRemitanceList;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        [HttpPost]
        public ActionResult ExpenditureDetailsPieChart(AccountDashboardModel dashboardModel, String id)
        {
            try
            {
                List<USP_ACC_MIS_EXPENDITURE_DETAILS_Result> lstExpenditureDetails = ExpenditureDetailsData(dashboardModel, id);

                return new JsonResult
                {
                    Data = lstExpenditureDetails
                };

            }
            catch (Exception)
            {
                return null;
            }
        }

        public ActionResult ExpenditureDetailsGrid(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                AccountDashboardModel dashboardModel = new AccountDashboardModel();
                dashboardModel.AgencyCode = Convert.ToInt32(Request.Params["AgencyCode"]);
                dashboardModel.DPIU = Convert.ToInt32(Request.Params["DPIU"]);
                dashboardModel.FundType = Request.Params["FundType"];
                String year = Request.Params["Year"];

                List<USP_ACC_MIS_EXPENDITURE_DETAILS_Result> lstExpenditureDetailsList = ExpenditureDetailsData(dashboardModel, year);

                int totalRecords = lstExpenditureDetailsList.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "HEAD_CODE":
                                lstExpenditureDetailsList = lstExpenditureDetailsList.OrderBy(x => x.HEAD_CODE).ToList();
                                break;
                            case "Expn":
                                lstExpenditureDetailsList = lstExpenditureDetailsList.OrderBy(x => x.Expn).ToList();
                                break;
                            default:
                                lstExpenditureDetailsList = lstExpenditureDetailsList.OrderBy(x => x.HEAD_CODE).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "HEAD_CODE":
                                lstExpenditureDetailsList = lstExpenditureDetailsList.OrderByDescending(x => x.HEAD_CODE).ToList();
                                break;
                            case "Expn":
                                lstExpenditureDetailsList = lstExpenditureDetailsList.OrderByDescending(x => x.Expn).ToList();
                                break;
                            default:
                                lstExpenditureDetailsList = lstExpenditureDetailsList.OrderByDescending(x => x.HEAD_CODE).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstExpenditureDetailsList = lstExpenditureDetailsList.OrderBy(x => x.HEAD_CODE).ToList();
                }


                var data = lstExpenditureDetailsList.Select(item => new
                {

                    cell = new[] 
                    {                                                   
                            item.HEAD_CODE.ToString(),
                            item.Expn.ToString(),                            
                    }
                }).ToArray();

                var jsonData = new
                {
                    rows = data,
                    //total = 0,
                    total = totalRecords <= rows ? 1 : totalRecords / ((rows) + 1),
                    page = page,
                    records = totalRecords
                };
                return Json(jsonData);


            }
            catch (Exception)
            {
                return null;
            }

        }


        public List<USP_ACC_MIS_EXPENDITURE_DETAILS_Result> ExpenditureDetailsData(AccountDashboardModel dashboardModel, String id)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            int Year = 0;

            if (!(String.IsNullOrEmpty(id)))
            {
                Year = Convert.ToInt32(id.Split('-')[0]);
            }

            try
            {
                int AdminNdCode = PMGSYSession.Current.LevelId == 5 ? PMGSYSession.Current.AdminNdCode : (dashboardModel.DPIU == 0 ? dashboardModel.AgencyCode : dashboardModel.DPIU);
                int levelID = PMGSYSession.Current.LevelId == 5 ? PMGSYSession.Current.LevelId : (dashboardModel.AgencyCode == 0 ? 6 : (dashboardModel.DPIU == 0 ? 4 : 5));

                List<USP_ACC_MIS_EXPENDITURE_DETAILS_Result> lstExpenditureDetails = dbContext.USP_ACC_MIS_EXPENDITURE_DETAILS(dashboardModel.FundType, levelID, AdminNdCode, Year).ToList<USP_ACC_MIS_EXPENDITURE_DETAILS_Result>();

                return lstExpenditureDetails;

            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// Action to get the list of summary for dashbord
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetAccountSummaryList(int? page, int? rows, string sidx, string sord)
        {
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            PMGSY.Models.Accounts.AccountsFilterModel objFilter = new PMGSY.Models.Accounts.AccountsFilterModel();
            // int Year = 0;  
            long totalRecords;
            objFilter.page = Convert.ToInt32(page) - 1;
            objFilter.rows = Convert.ToInt32(rows);
            objFilter.sidx = sidx.ToString();
            objFilter.sord = sord.ToString();

            if (PMGSYSession.Current.LevelId == 6 || PMGSYSession.Current.LevelId == 4)
            {
                objFilter.AdminNdCode = Convert.ToInt32(Request.Params["adminNdCode"]);
            }
            else
            {
                objFilter.AdminNdCode = PMGSYSession.Current.ParentNDCode.Value;
            }

            objFilter.FundType = Request.Params["fundType"];
            objFilter.month = Convert.ToInt16(System.DateTime.Now.Month);
            if (!(String.IsNullOrEmpty(Request.Params["year"])))
            {
                objFilter.year = Convert.ToInt16(Request.Params["year"].ToString().Split('-')[0]);
            }
            else
            {
                objFilter.year = Convert.ToInt16(System.DateTime.Now.Year);
            }
            if (((PMGSYSession.Current.LevelId == 6) || (PMGSYSession.Current.LevelId == 4)) && Convert.ToInt16(Request.Params["lowercode"]) == 0)
            {
                objFilter.LevelId = 4;//DPIU
            }
            else
            {
                objFilter.LevelId = 5;
            }

            if (PMGSYSession.Current.LevelId == 6 || PMGSYSession.Current.LevelId == 4)
            {
                objFilter.lowercode = Convert.ToInt16(Request.Params["lowercode"]);
            }
            else
            {
                objFilter.lowercode = (short)PMGSYSession.Current.AdminNdCode;
            }
            objFilter.ownLower = Request.Params["ownLower"];

            var jsonData = new
            {
                rows = GetSummaryList(objFilter, out totalRecords),
                total = totalRecords <= objFilter.rows ? 1 : totalRecords / objFilter.rows + 1,
                page = objFilter.page + 1,
                records = totalRecords
            };
            return Json(jsonData);

        }


        /// <summary>
        /// function to get the Account summary for dashbord
        /// </summary>
        /// <param name="objFilter"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array GetSummaryList(PMGSY.Models.Accounts.AccountsFilterModel objFilter, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                CommonFunctions objCommonFunc = new CommonFunctions();
                List<SP_ACC_DB_DISPLAY_SUMMARY_Result> resultlist =
                dbContext.SP_ACC_DB_DISPLAY_SUMMARY(

                objFilter.FundType,
                objFilter.AdminNdCode,
                objFilter.month,
                objFilter.year,
                objFilter.LevelId,
                objFilter.lowercode,
                objFilter.ownLower
                ).ToList<SP_ACC_DB_DISPLAY_SUMMARY_Result>();

                totalRecords = resultlist.Count();

                if (objFilter.sidx.Trim() != string.Empty)
                {
                    if (objFilter.sord.ToString() == "asc")
                    {
                        switch (objFilter.sidx)
                        {

                            default:
                                resultlist = resultlist.OrderBy(x => x.BILL_TYPE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<SP_ACC_DB_DISPLAY_SUMMARY_Result>();
                                break;
                        }

                    }
                    else
                    {
                        switch (objFilter.sidx)
                        {

                            default:
                                resultlist = resultlist.OrderByDescending(x => x.BILL_TYPE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<SP_ACC_DB_DISPLAY_SUMMARY_Result>();
                                break;
                        }
                    }
                }
                else
                {
                    resultlist = resultlist.OrderByDescending(x => x.BILL_TYPE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<SP_ACC_DB_DISPLAY_SUMMARY_Result>();
                }


                return resultlist.Select(item => new
                {

                    cell = new[] 
                    {                                                   
                                   item.BILL_TYPE.ToString(),
                                   item.Desc,
                                   item.Upto_count.HasValue ? item.Upto_count.Value.ToString():string.Empty,
                                   item.Upto_amount.HasValue ? item.Upto_amount.Value.ToString():string.Empty,
                                   item.month_count.HasValue ? item.month_count.Value.ToString() : String.Empty,
                                   item.month_amount.HasValue ? item.month_amount.Value.ToString():string.Empty

                                  
                    }
                }).ToArray();


            }
            catch (Exception ex)
            {
                totalRecords = 0;
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }


        }


        /// <summary>
        /// Populate Agency
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> PopulateSRRDA()
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                List<SelectListItem> lstDetails = new List<SelectListItem>();
                var list = (from item in dbContext.ADMIN_DEPARTMENT
                            join state in dbContext.MASTER_STATE
                            on item.MAST_STATE_CODE equals state.MAST_STATE_CODE
                            where item.MAST_ND_TYPE == "S"
                            orderby state.MAST_STATE_NAME
                            select new
                            {
                                ADMIN_NAME = state.MAST_STATE_NAME + "(" + item.ADMIN_ND_NAME + ")",
                                ADMIN_CODE = item.ADMIN_ND_CODE
                            });

                foreach (var item in list)
                {
                    lstDetails.Add(new SelectListItem { Value = item.ADMIN_CODE.ToString(), Text = item.ADMIN_NAME.ToString().Trim() });

                }
                lstDetails.Insert(0, (new SelectListItem { Text = "Select State", Value = "" }));
                return lstDetails;
            }
            catch (Exception ex)
            {
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

        /// <summary>
        /// Populate Fund Types
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> PopulateFundType()
        {
            try
            {
                List<SelectListItem> lstFundType = new List<SelectListItem>();
                lstFundType.Add(new SelectListItem { Text = "Select Fund Type", Value = "0" });
                lstFundType.Add(new SelectListItem { Text = "Programme Fund", Value = "P", Selected = true });
                lstFundType.Add(new SelectListItem { Text = "Administrative Fund", Value = "A" });
                lstFundType.Add(new SelectListItem { Text = "Maintenance Fund", Value = "M" });
                return lstFundType;
            }
            catch (Exception)
            {
                return null;
            }

        }
    }
}

