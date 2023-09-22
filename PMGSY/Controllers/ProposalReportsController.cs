using PMGSY.BAL.ProposalReports;
using PMGSY.Common;
using PMGSY.DAL.ProposalReports;
using PMGSY.Extensions;
using PMGSY.Models;
using PMGSY.Models.ProposalReports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Configuration;

namespace PMGSY.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    [Audit]
    public class ProposalReportsController : Controller
    {

        IProposalReportsBAL proposalReportsBAL;
        CommonFunctions objCommonFunctions;
        #region MPR 1

        public ActionResult MPR1Details()
        {
            ProposalReportsModel ProposaleportsViewModel = new ProposalReportsModel();
            ProposalReportsDAL objDAL = new ProposalReportsDAL();
            objCommonFunctions = new CommonFunctions();

            try
            {
                ProposaleportsViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                ProposaleportsViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                List<SelectListItem> ddMonth = objCommonFunctions.PopulateMonths(false).ToList();
                List<SelectListItem> ddYear = objCommonFunctions.PopulateFinancialYear(true, true).ToList();
                List<SelectListItem> ddfundingAgency = objDAL.GetFundingAgencyList();
                SelectListItem all = new SelectListItem
                {
                    Selected = true,
                    Text = "All",
                    Value = "0"
                };
                // ddMonth.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));
                //ddMonth.Insert(0, all);
                //ddYear.Insert(0, all);
                ddYear.RemoveAt(0);
                ddMonth.Find(x => x.Value == DateTime.Now.Month.ToString()).Selected = true;
                ddYear.Find(x => x.Value == DateTime.Now.Year.ToString()).Selected = true;

                ViewData["YEAR"] = ddYear;
                ViewData["MONTH"] = ddMonth;
                ViewData["AGENCY"] = ddfundingAgency;


                return View(ProposaleportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult MPR1StateReportListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();
                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int year = Convert.ToInt32(formCollection["Year"]); ;
                int month = Convert.ToInt32(formCollection["Month"]);
                int collaboration = Convert.ToInt32(formCollection["Collaboration"]);
                int totalRecords;

                var jsonData = new
                {
                    rows = proposalReportsBAL.MPR1StateReportListingBAL(year, month, collaboration, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult MPR1DistrictReportListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();
                int stateCode = Convert.ToInt32(formCollection["StateCode"]);
                int year = Convert.ToInt32(formCollection["Year"]); ;
                int month = Convert.ToInt32(formCollection["Month"]);
                int collaboration = Convert.ToInt32(formCollection["Collaboration"]);
                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords = 0;

                var jsonData = new
                {
                    rows = proposalReportsBAL.MPR1DistrictReportListingBAL(stateCode, year, month, collaboration, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult MPR1BlockReportListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();
                int stateCode = Convert.ToInt32(formCollection["StateCode"]);
                int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
                int year = Convert.ToInt32(formCollection["Year"]); ;
                int month = Convert.ToInt32(formCollection["Month"]);
                int collaboration = Convert.ToInt32(formCollection["Collaboration"]);
                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords = 0;

                var jsonData = new
                {
                    rows = proposalReportsBAL.MPR1BlockReportListingBAL(stateCode, districtCode, year, month, collaboration, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult MPR1FinalReportListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();
                int blockCode = Convert.ToInt32(formCollection["BlockCode"]);
                int stateCode = Convert.ToInt32(formCollection["StateCode"]);
                int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
                int year = Convert.ToInt32(formCollection["Year"]); ;
                int month = Convert.ToInt32(formCollection["Month"]);
                int collaboration = Convert.ToInt32(formCollection["Collaboration"]);
                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords = 0;

                var jsonData = new
                {
                    rows = proposalReportsBAL.MPR1FinalListingBAL(blockCode, districtCode, stateCode, year, month, collaboration, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }
        #endregion

        #region MPR 2

        public ActionResult MPR2Details()
        {
            try
            {
                objCommonFunctions = new CommonFunctions();
                ProposalReportsDAL objDAL = new ProposalReportsDAL();
                ProposalReportsModel ProposaleportsViewModel = new ProposalReportsModel();
                ProposaleportsViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                ProposaleportsViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                List<SelectListItem> ddMonth = objCommonFunctions.PopulateMonths(false).ToList();
                List<SelectListItem> ddYear = objCommonFunctions.PopulateFinancialYear(true, true).ToList();
                List<SelectListItem> ddfundingAgency = objDAL.GetFundingAgencyList();
                SelectListItem all = new SelectListItem
                {
                    Selected = true,
                    Text = "All",
                    Value = "0"
                };
                // ddMonth.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));
                //ddMonth.Insert(0, all);
                //ddYear.Insert(0, all);
                ddYear.RemoveAt(0);
                ddMonth.Find(x => x.Value == DateTime.Now.Month.ToString()).Selected = true;
                ddYear.Find(x => x.Value == DateTime.Now.Year.ToString()).Selected = true;


                ViewData["YEAR"] = ddYear;
                ViewData["MONTH"] = ddMonth;
                ViewData["AGENCY"] = ddfundingAgency;
                return View(ProposaleportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult MPR2StateReportListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();
                int month = Convert.ToInt32(formCollection["Month"]);
                int year = Convert.ToInt32(formCollection["Year"]);
                int collaboration = Convert.ToInt32(formCollection["Collaboration"]);
                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords;

                var jsonData = new
                {
                    rows = proposalReportsBAL.MPR2StateReportListingBAL(month, year, collaboration, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult MPR2DistrictReportListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();
                int month = Convert.ToInt32(formCollection["Month"]);
                int year = Convert.ToInt32(formCollection["Year"]);
                int collaboration = Convert.ToInt32(formCollection["Collaboration"]);
                int stateCode = Convert.ToInt32(formCollection["StateCode"]);
                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords = 0;

                var jsonData = new
                {
                    rows = proposalReportsBAL.MPR2DistrictReportListingBAL(month, year, collaboration, stateCode, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult MPR2BlockReportListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();
                int month = Convert.ToInt32(formCollection["Month"]);
                int year = Convert.ToInt32(formCollection["Year"]);
                int collaboration = Convert.ToInt32(formCollection["Collaboration"]);
                int stateCode = Convert.ToInt32(formCollection["StateCode"]);
                int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords = 0;

                var jsonData = new
                {
                    rows = proposalReportsBAL.MPR2BlockReportListingBAL(month, year, collaboration, stateCode, districtCode, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult MPR2FinalReportListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();
                int month = Convert.ToInt32(formCollection["Month"]);
                int year = Convert.ToInt32(formCollection["Year"]);
                int collaboration = Convert.ToInt32(formCollection["Collaboration"]);
                int blockCode = Convert.ToInt32(formCollection["BlockCode"]);
                int stateCode = Convert.ToInt32(formCollection["StateCode"]);
                int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords = 0;

                var jsonData = new
                {
                    rows = proposalReportsBAL.MPR2FinalListingBAL(month, year, collaboration, blockCode, districtCode, stateCode, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }
        #endregion

        #region HY 1

        public ActionResult HY1Details()
        {
            ProposalReportsModel ProposaleportsViewModel = new ProposalReportsModel();
            try
            {
                ProposaleportsViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                ProposaleportsViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;

                return View(ProposaleportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult HY1StateReportListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();
                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords;

                var jsonData = new
                {
                    rows = proposalReportsBAL.HY1StateReportListingBAL(page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult HY1DistrictReportListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();
                int stateCode = Convert.ToInt32(formCollection["StateCode"]);
                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords = 0;

                var jsonData = new
                {
                    rows = proposalReportsBAL.HY1DistrictReportListingBAL(stateCode, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }
        #endregion

        #region HY 2


        public ActionResult HY2Details()
        {
            ProposalReportsModel ProposaleportsViewModel = new ProposalReportsModel();
            try
            {
                ProposaleportsViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                ProposaleportsViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;

                return View(ProposaleportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult HY2StateReportListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();
                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords;

                var jsonData = new
                {
                    rows = proposalReportsBAL.HY2StateReportListingBAL(page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult HY2DistrictReportListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();
                int stateCode = Convert.ToInt32(formCollection["StateCode"]);
                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords = 0;

                var jsonData = new
                {
                    rows = proposalReportsBAL.HY2DistrictReportListingBAL(stateCode, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }
        #endregion

        #region Prop List

        public ActionResult PropListDetails()
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            ProposalReportsModel ProposaleportsViewModel = new ProposalReportsModel();
            ProposalReportsDAL objDAL = new ProposalReportsDAL();
            try
            {
                int stateCode = PMGSYSession.Current.StateCode;

                List<SelectListItem> ddYear = objCommonFunctions.PopulateFinancialYear(true, true).ToList();
                List<SelectListItem> ddFundingAgency = objDAL.GetFundingAgencyList();
                List<SelectListItem> ddState = objCommonFunctions.PopulateStates(false);
                SelectListItem all = new SelectListItem
                {
                    Selected = true,
                    Text = "All",
                    Value = "0"
                };
                ddYear.RemoveAt(0);
                ddYear.Insert(0, all);
                ddState.Insert(0, all);
                if (stateCode > 0)  //if state login
                {
                    ddState.Find(x => x.Value == stateCode.ToString()).Selected = true;
                }


                List<SelectListItem> ddProposalStatus = new List<SelectListItem>();
                SelectListItem item = new SelectListItem();

                item = new SelectListItem();
                item.Text = "Sanction Pending";
                item.Value = "N";
                ddProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Dropped Proposal";
                item.Value = "D";
                ddProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Sanctioned Proposals";
                item.Value = "Y";
                ddProposalStatus.Add(item);

                item = new SelectListItem();
                // item.Text = "Un-Sanctioned Proposals";
                item.Text = "Proposal Not Sanctioned";
                item.Value = "U";
                ddProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Reconsider Proposals";
                item.Value = "R";
                ddProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "All";
                item.Value = "A";
                item.Selected = true;
                ddProposalStatus.Add(item);


                List<SelectListItem> ddBatch = objCommonFunctions.PopulateBatch();
                ddBatch.RemoveAt(0);
                ddBatch.Insert(0, all);



                ViewData["YEAR"] = ddYear;
                ViewData["BATCH"] = ddBatch;
                ViewData["AGENCY"] = ddFundingAgency;
                ViewData["STATUS"] = ddProposalStatus;
                //ViewData["STATE"] = ddState;
                ProposaleportsViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                ProposaleportsViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                return View(ProposaleportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult PropListStateReportListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();

                int year = Convert.ToInt32(formCollection["Year"]);
                int batch = Convert.ToInt32(formCollection["Batch"]);
                int collaboration = Convert.ToInt32(formCollection["Collaboration"]);
                string status = formCollection["Status"] == "A" ? "%" : formCollection["Status"];
                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords;

                var jsonData = new
                {
                    rows = proposalReportsBAL.PropListStateListingBAL(year, batch, collaboration, status, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult PropListDistrictReportListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();
                int stateCode = Convert.ToInt32(formCollection["StateCode"]);
                int year = Convert.ToInt32(formCollection["Year"]);
                int batch = Convert.ToInt32(formCollection["Batch"]);
                int collaboration = Convert.ToInt32(formCollection["Collaboration"]);
                string status = formCollection["Status"] == "A" ? "%" : formCollection["Status"];
                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords = 0;

                var jsonData = new
                {
                    rows = proposalReportsBAL.PropListDistrictListingBAL(stateCode, year, batch, collaboration, status, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult PropListBlockReportListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();
                int stateCode = Convert.ToInt32(formCollection["StateCode"]);
                int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
                int year = Convert.ToInt32(formCollection["Year"]);
                int batch = Convert.ToInt32(formCollection["Batch"]);
                int collaboration = Convert.ToInt32(formCollection["Collaboration"]);
                string status = formCollection["Status"] == "A" ? "%" : formCollection["Status"];
                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords = 0;

                var jsonData = new
                {
                    rows = proposalReportsBAL.PropListBlockListingBAL(districtCode, stateCode, year, batch, collaboration, status, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult PropListFinalReportListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();
                int blockCode = Convert.ToInt32(formCollection["BlockCode"]);
                int stateCode = Convert.ToInt32(formCollection["StateCode"]);
                int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
                int year = Convert.ToInt32(formCollection["Year"]);
                int batch = Convert.ToInt32(formCollection["Batch"]);
                int collaboration = Convert.ToInt32(formCollection["Collaboration"]);
                string status = formCollection["Status"] == "A" ? "%" : formCollection["Status"];
                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords = 0;

                var jsonData = new
                {
                    rows = proposalReportsBAL.PropListFinalListingBAL(blockCode, districtCode, stateCode, year, batch, collaboration, status, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }
        #endregion

        #region Proposal Sanction List

        public ActionResult PropSanctionDetails()
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            ProposalReportsModel ProposaleportsViewModel = new ProposalReportsModel();
            ProposalReportsDAL objDAL = new ProposalReportsDAL();
            try
            {
                int stateCode = PMGSYSession.Current.StateCode;

                List<SelectListItem> ddYear = objCommonFunctions.PopulateFinancialYear(true, true).ToList();
                List<SelectListItem> ddFundingAgency = objDAL.GetFundingAgencyList();
                List<SelectListItem> ddState = objCommonFunctions.PopulateStates(false);
                SelectListItem all = new SelectListItem
                {
                    Selected = true,
                    Text = "All",
                    Value = "0"
                };
                ddYear.RemoveAt(0);
                ddYear.Insert(0, all);
                ddState.Insert(0, all);
                if (stateCode > 0)  //if state login
                {
                    ddState.Find(x => x.Value == stateCode.ToString()).Selected = true;
                }


                List<SelectListItem> ddProposalStatus = new List<SelectListItem>();
                SelectListItem item = new SelectListItem();

                item = new SelectListItem();
                item.Text = "Pending Proposals";
                item.Value = "N";
                ddProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Sanctioned Proposals";
                item.Value = "Y";
                ddProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Un-Sanctioned Proposals";
                item.Value = "U";
                ddProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Recommended Proposals";
                item.Value = "R";
                ddProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Droped Propsoal";
                item.Value = "D";
                ddProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "All";
                item.Value = "A";
                item.Selected = true;
                ddProposalStatus.Add(item);


                List<SelectListItem> ddBatch = objCommonFunctions.PopulateBatch();
                ddBatch.RemoveAt(0);
                ddBatch.Insert(0, all);


                ViewData["YEAR"] = ddYear;
                ViewData["BATCH"] = ddBatch;
                ViewData["AGENCY"] = ddFundingAgency;
                ViewData["STATUS"] = ddProposalStatus;
                //ViewData["STATE"] = ddState;
                ProposaleportsViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                ProposaleportsViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                return View(ProposaleportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult PropSanctionStateReportListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();
                //  string status = "Y";
                int year = Convert.ToInt32(formCollection["Year"]);
                int batch = Convert.ToInt32(formCollection["Batch"]);
                int collaboration = Convert.ToInt32(formCollection["Collaboration"]);
                string status = formCollection["Status"] == "A" ? "%" : formCollection["Status"];
                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords;

                var jsonData = new
                {
                    rows = proposalReportsBAL.PropSanctionStateListingBAL(year, batch, collaboration, status, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult PropSanctionDistrictReportListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();
                int stateCode = Convert.ToInt32(formCollection["StateCode"]);
                //  string status = "Y";
                int year = Convert.ToInt32(formCollection["Year"]);
                int batch = Convert.ToInt32(formCollection["Batch"]);
                int collaboration = Convert.ToInt32(formCollection["Collaboration"]);
                string status = formCollection["Status"] == "A" ? "%" : formCollection["Status"];
                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords = 0;

                var jsonData = new
                {
                    rows = proposalReportsBAL.PropSanctionDistrictListingBAL(stateCode, year, batch, collaboration, status, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult PropSanctionBlockReportListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();
                int stateCode = Convert.ToInt32(formCollection["StateCode"]);
                int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
                //  string status = "Y";
                int year = Convert.ToInt32(formCollection["Year"]);
                int batch = Convert.ToInt32(formCollection["Batch"]);
                int collaboration = Convert.ToInt32(formCollection["Collaboration"]);
                string status = formCollection["Status"] == "A" ? "%" : formCollection["Status"];
                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords = 0;

                var jsonData = new
                {
                    rows = proposalReportsBAL.PropSanctionBlockListingBAL(districtCode, stateCode, year, batch, collaboration, status, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult PropSanctionFinalReportListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();
                int blockCode = Convert.ToInt32(formCollection["BlockCode"]);
                int stateCode = Convert.ToInt32(formCollection["StateCode"]);
                int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
                //  string status = "Y";
                int year = Convert.ToInt32(formCollection["Year"]);
                int batch = Convert.ToInt32(formCollection["Batch"]);
                int collaboration = Convert.ToInt32(formCollection["Collaboration"]);
                string status = formCollection["Status"] == "A" ? "%" : formCollection["Status"];
                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords = 0;

                var jsonData = new
                {
                    rows = proposalReportsBAL.PropSanctionFinalListingBAL(blockCode, districtCode, stateCode, year, batch, collaboration, status, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }
        #endregion

        #region Prop Sanction Length

        public ActionResult PropSanctionLengthDetails()
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            ProposalReportsModel ProposaleportsViewModel = new ProposalReportsModel();
            ProposalReportsDAL objDAL = new ProposalReportsDAL();
            try
            {
                int stateCode = PMGSYSession.Current.StateCode;

                List<SelectListItem> ddYear = objCommonFunctions.PopulateFinancialYear(true, true).ToList();
                List<SelectListItem> ddFundingAgency = objDAL.GetFundingAgencyList();
                List<SelectListItem> ddState = objCommonFunctions.PopulateStates(false);
                SelectListItem all = new SelectListItem
                {
                    Selected = true,
                    Text = "All",
                    Value = "0"
                };
                ddYear.RemoveAt(0);
                ddYear.Insert(0, all);
                ddState.Insert(0, all);
                if (stateCode > 0)  //if state login
                {
                    ddState.Find(x => x.Value == stateCode.ToString()).Selected = true;
                }


                List<SelectListItem> ddProposalStatus = new List<SelectListItem>();
                SelectListItem item = new SelectListItem();

                item = new SelectListItem();
                item.Text = "Pending Proposals";
                item.Value = "N";
                ddProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Sanctioned Proposals";
                item.Value = "Y";
                ddProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Un-Sanctioned Proposals";
                item.Value = "U";
                ddProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Recommended Proposals";
                item.Value = "R";
                ddProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Droped Propsoal";
                item.Value = "D";
                ddProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "All";
                item.Value = "A";
                item.Selected = true;
                ddProposalStatus.Add(item);



                List<SelectListItem> ddBatch = objCommonFunctions.PopulateBatch();
                ddBatch.RemoveAt(0);
                ddBatch.Insert(0, all);


                ViewData["YEAR"] = ddYear;
                ViewData["BATCH"] = ddBatch;
                ViewData["AGENCY"] = ddFundingAgency;
                ViewData["STATUS"] = ddProposalStatus;
                //ViewData["STATE"] = ddState;
                ProposaleportsViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                ProposaleportsViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                return View(ProposaleportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult PropSanctionLengthStateReportListing(FormCollection formCollection)
        {
            try
            {  //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();
                //  string status = "%";
                int year = Convert.ToInt32(formCollection["Year"]);
                int batch = Convert.ToInt32(formCollection["Batch"]);
                int collaboration = Convert.ToInt32(formCollection["Collaboration"]);
                string status = formCollection["Status"] == "A" ? "%" : formCollection["Status"];
                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords;

                var jsonData = new
                {
                    rows = proposalReportsBAL.PropSanctionLengthStateListingBAL(year, batch, collaboration, status, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult PropSanctionLengthDistrictReportListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();
                int stateCode = Convert.ToInt32(formCollection["StateCode"]);
                //  string status = "%";
                int year = Convert.ToInt32(formCollection["Year"]);
                int batch = Convert.ToInt32(formCollection["Batch"]);
                int collaboration = Convert.ToInt32(formCollection["Collaboration"]);
                string status = formCollection["Status"] == "A" ? "%" : formCollection["Status"];
                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords = 0;

                var jsonData = new
                {
                    rows = proposalReportsBAL.PropSanctionLengthDistrictListingBAL(stateCode, year, batch, collaboration, status, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult PropSanctionLengthBlockReportListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();
                int stateCode = Convert.ToInt32(formCollection["StateCode"]);
                int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
                //  string status = "%";
                int year = Convert.ToInt32(formCollection["Year"]);
                int batch = Convert.ToInt32(formCollection["Batch"]);
                int collaboration = Convert.ToInt32(formCollection["Collaboration"]);
                string status = formCollection["Status"] == "A" ? "%" : formCollection["Status"];
                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords = 0;

                var jsonData = new
                {
                    rows = proposalReportsBAL.PropSanctionLengthBlockListingBAL(districtCode, stateCode, year, batch, collaboration, status, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult PropSanctionLengthFinalReportListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();
                int blockCode = Convert.ToInt32(formCollection["BlockCode"]);
                int stateCode = Convert.ToInt32(formCollection["StateCode"]);
                int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
                //  string status = "%";
                int year = Convert.ToInt32(formCollection["Year"]);
                int batch = Convert.ToInt32(formCollection["Batch"]);
                int collaboration = Convert.ToInt32(formCollection["Collaboration"]);
                string status = formCollection["Status"] == "A" ? "%" : formCollection["Status"];
                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords = 0;

                var jsonData = new
                {
                    rows = proposalReportsBAL.PropSanctionLengthFinalListingBAL(blockCode, districtCode, stateCode, year, batch, collaboration, status, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }
        #endregion

        #region Prop Estimated Maintenance Cost

        public ActionResult PropEMCDetails()
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            ProposalReportsModel ProposaleportsViewModel = new ProposalReportsModel();
            ProposalReportsDAL objDAL = new ProposalReportsDAL();
            try
            {
                int stateCode = PMGSYSession.Current.StateCode;

                List<SelectListItem> ddYear = objCommonFunctions.PopulateFinancialYear(true, true).ToList();
                List<SelectListItem> ddFundingAgency = objDAL.GetFundingAgencyList();
                List<SelectListItem> ddState = objCommonFunctions.PopulateStates(false);
                SelectListItem all = new SelectListItem
                {
                    Selected = true,
                    Text = "All",
                    Value = "0"
                };
                ddYear.RemoveAt(0);
                ddYear.Insert(0, all);
                ddState.Insert(0, all);
                if (stateCode > 0)  //if state login
                {
                    ddState.Find(x => x.Value == stateCode.ToString()).Selected = true;
                }


                List<SelectListItem> ddProposalStatus = new List<SelectListItem>();
                SelectListItem item = new SelectListItem();

                item = new SelectListItem();
                item.Text = "Pending Proposals";
                item.Value = "N";
                ddProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Sanctioned Proposals";
                item.Value = "Y";
                ddProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Un-Sanctioned Proposals";
                item.Value = "U";
                ddProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Recommended Proposals";
                item.Value = "R";
                ddProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Droped Propsoal";
                item.Value = "D";
                ddProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "All";
                item.Value = "A";
                item.Selected = true;
                ddProposalStatus.Add(item);


                List<SelectListItem> ddBatch = objCommonFunctions.PopulateBatch();
                ddBatch.RemoveAt(0);
                ddBatch.Insert(0, all);


                ViewData["YEAR"] = ddYear;
                ViewData["BATCH"] = ddBatch;
                ViewData["AGENCY"] = ddFundingAgency;
                ViewData["STATUS"] = ddProposalStatus;
                //ViewData["STATE"] = ddState;
                ProposaleportsViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                ProposaleportsViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                return View(ProposaleportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult PropEMCStateReportListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();

                //  string status = "%";
                int year = Convert.ToInt32(formCollection["Year"]);
                int batch = Convert.ToInt32(formCollection["Batch"]);
                int collaboration = Convert.ToInt32(formCollection["Collaboration"]);
                string status = formCollection["Status"] == "A" ? "%" : formCollection["Status"];
                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords;

                var jsonData = new
                {
                    rows = proposalReportsBAL.PropEMCStateListingBAL(year, batch, collaboration, status, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult PropEMCDistrictReportListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();
                int stateCode = Convert.ToInt32(formCollection["StateCode"]) == 0 ? PMGSYSession.Current.StateCode : Convert.ToInt32(formCollection["StateCode"]);
                //  string status = "%";
                int year = Convert.ToInt32(formCollection["Year"]);
                int batch = Convert.ToInt32(formCollection["Batch"]);
                int collaboration = Convert.ToInt32(formCollection["Collaboration"]);
                string status = formCollection["Status"] == "A" ? "%" : formCollection["Status"];
                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords = 0;

                var jsonData = new
                {
                    rows = proposalReportsBAL.PropEMCDistrictListingBAL(stateCode, year, batch, collaboration, status, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult PropEMCBlockReportListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();
                int stateCode = Convert.ToInt32(formCollection["StateCode"]);
                int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
                //  string status = "%";
                int year = Convert.ToInt32(formCollection["Year"]);
                int batch = Convert.ToInt32(formCollection["Batch"]);
                int collaboration = Convert.ToInt32(formCollection["Collaboration"]);
                string status = formCollection["Status"] == "A" ? "%" : formCollection["Status"];
                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords = 0;

                var jsonData = new
                {
                    rows = proposalReportsBAL.PropEMCBlockListingBAL(districtCode, stateCode, year, batch, collaboration, status, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult PropEMCFinalReportListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();
                int blockCode = Convert.ToInt32(formCollection["BlockCode"]);
                int stateCode = Convert.ToInt32(formCollection["StateCode"]);
                int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
                //  string status = "%";
                int year = Convert.ToInt32(formCollection["Year"]);
                int batch = Convert.ToInt32(formCollection["Batch"]);
                int collaboration = Convert.ToInt32(formCollection["Collaboration"]);
                string status = formCollection["Status"] == "A" ? "%" : formCollection["Status"];
                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords = 0;

                var jsonData = new
                {
                    rows = proposalReportsBAL.PropEMCFinalListingBAL(blockCode, districtCode, stateCode, year, batch, collaboration, status, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }
        #endregion

        #region Proposed Scrutiny Details

        public ActionResult PropScrutinyDetails()
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            ProposalReportsModel ProposaleportsViewModel = new ProposalReportsModel();
            ProposalReportsDAL objDAL = new ProposalReportsDAL();
            try
            {
                int stateCode = PMGSYSession.Current.StateCode;

                List<SelectListItem> ddYear = objCommonFunctions.PopulateFinancialYear(true, true).ToList();
                List<SelectListItem> ddScheme = objDAL.GetFundingAgencyList();
                List<SelectListItem> ddState = objCommonFunctions.PopulateStates(false);
                SelectListItem all = new SelectListItem
                {
                    Selected = true,
                    Text = "All",
                    Value = "0"
                };
                ddYear.RemoveAt(0);
                ddYear.Insert(0, all);
                ddState.Insert(0, all);
                if (stateCode > 0)  //if state login
                {
                    ddState.Find(x => x.Value == stateCode.ToString()).Selected = true;
                }


                SelectListItem sta = new SelectListItem
                {
                    Selected = true,
                    Text = "STA",
                    Value = "S"
                };
                SelectListItem pta = new SelectListItem
                {

                    Text = "PTA",
                    Value = "P"
                };

                List<SelectListItem> ddtype = new List<SelectListItem>();
                ddtype.Add(sta);
                ddtype.Add(pta);
                List<SelectListItem> ddAgency = new List<SelectListItem>();
                ddAgency.Add(all);
                List<SelectListItem> ddBatch = objCommonFunctions.PopulateBatch();
                ddBatch.RemoveAt(0);
                ddBatch.Insert(0, all);

                ViewData["TYPE"] = ddtype;
                ViewData["AGENCY"] = ddAgency;
                ViewData["YEAR"] = ddYear;
                ViewData["BATCH"] = ddBatch;
                ViewData["SCHEME"] = ddScheme;
                ViewData["STATE"] = ddState;
                ProposaleportsViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                ProposaleportsViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                return View(ProposaleportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }


        [HttpPost]
        public ActionResult PropScrutinyReportListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();
                int stateCode = Convert.ToInt32(formCollection["StateCode"]) == 0 ? PMGSYSession.Current.StateCode : Convert.ToInt32(formCollection["StateCode"]);
                string type = formCollection["TYPE"];
                int agncyId = Convert.ToInt32(formCollection["AGENCY"]);
                int year = Convert.ToInt32(formCollection["YEAR"]);
                int batch = Convert.ToInt32(formCollection["BATCH"]);
                int scheme = Convert.ToInt32(formCollection["SCHEME"]);
                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords;

                var jsonData = new
                {
                    rows = proposalReportsBAL.PropScrutinyListingBAL(stateCode, type, agncyId, year, batch, scheme, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }

        }

        [HttpPost]
        public ActionResult PropScrutinyTASDReportListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();

                string type = formCollection["TYPE"];
                int agncyId = Convert.ToInt32(formCollection["AGENCY"]);
                int stateCode = Convert.ToInt32(formCollection["StateCode"]) == 0 ? PMGSYSession.Current.StateCode : Convert.ToInt32(formCollection["StateCode"]);
                int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
                string[] yearArr = formCollection["YEAR"].Split('-');
                int year = Convert.ToInt32(yearArr[0]);
                int batch = Convert.ToInt32(formCollection["BATCH"]);
                int scheme = Convert.ToInt32(formCollection["SCHEME"]);
                string taName = formCollection["TAName"];
                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords;

                var jsonData = new
                {
                    rows = proposalReportsBAL.PropScrutinyTASDListingBAL(stateCode, districtCode, type, agncyId, year, batch, scheme, taName, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public JsonResult GetTechAgencyName_ByAgencyType(string type, string stateCode)
        {
            try
            {
                List<SelectListItem> lstAgency = new List<SelectListItem>();
                ProposalReportsDAL objDAL = new ProposalReportsDAL();
                int StateCode = Convert.ToInt32(stateCode);
                lstAgency = objDAL.GetTechAgencyName_ByAgencyType(StateCode, type.Trim());
                //classTypeList.Insert(0, new SelectListItem { Value = "0", Text = "--All--" });
                return Json(new SelectList(lstAgency, "Value", "Text"));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(false);
            }
        }
        #endregion

        #region Pending Works Details

        public ActionResult PendingWorksDetails()
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            ProposalReportsModel ProposaleportsViewModel = new ProposalReportsModel();
            try
            {
                int stateCode = PMGSYSession.Current.StateCode;

                List<SelectListItem> ddState = objCommonFunctions.PopulateStates(false);
                SelectListItem allState = new SelectListItem
                {
                    Selected = true,
                    Text = "All",
                    Value = "0"
                };
                ddState.Insert(0, allState);
                if (stateCode > 0)  //if state login
                {
                    ddState.Find(x => x.Value == stateCode.ToString()).Selected = true;
                }

                SelectListItem all = new SelectListItem
                {
                    Selected = true,
                    Text = "All",
                    Value = "%"
                };

                SelectListItem landAcquisition = new SelectListItem
                {
                    Text = "Land Acquisition",
                    Value = "A"
                };
                SelectListItem legalCase = new SelectListItem
                {
                    Text = "Legal Case",
                    Value = "L"
                };
                SelectListItem forestClearence = new SelectListItem
                {
                    Text = "Forest Clearance",
                    Value = "F"
                };

                List<SelectListItem> ddreason = new List<SelectListItem>();
                ddreason.Add(all);
                ddreason.Add(landAcquisition);
                ddreason.Add(legalCase);
                ddreason.Add(forestClearence);
                ViewData["REASON"] = ddreason;
                ViewData["STATE"] = ddState;
                ProposaleportsViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                ProposaleportsViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                return View(ProposaleportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult PendingWorksReportListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();

                int stateCode = Convert.ToInt32(formCollection["StateCode"]) == 0 ? PMGSYSession.Current.StateCode : Convert.ToInt32(formCollection["StateCode"]);
                string reason = formCollection["Reason"];
                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords;

                var jsonData = new
                {
                    rows = proposalReportsBAL.PendingWorksListingBAL(stateCode, reason, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        #endregion

        #region Proposal Analysis Details

        public ActionResult PropAnalysisDetails()
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            ProposalReportsModel ProposaleportsViewModel = new ProposalReportsModel();
            try
            {
                int stateCode = PMGSYSession.Current.StateCode;

                List<SelectListItem> ddState = objCommonFunctions.PopulateStates(false);
                //SelectListItem allState = new SelectListItem
                //{
                //    Selected = true,
                //    Text = "All",
                //    Value = "0"
                //};
                ddState.Find(x => x.Value == 1.ToString()).Selected = true;
                if (stateCode > 0)  //if state login
                {
                    ddState.Find(x => x.Value == stateCode.ToString()).Selected = true;
                }

                SelectListItem all = new SelectListItem
                {
                    Selected = true,
                    Text = "All",
                    Value = "%"
                };

                SelectListItem pending = new SelectListItem
                {
                    Text = "Pending",
                    Value = "N"
                };
                SelectListItem complete = new SelectListItem
                {
                    Text = "Complete",
                    Value = "Y"
                };


                SelectListItem yes = new SelectListItem
                {
                    Text = "YES",
                    Value = "Y"
                };
                SelectListItem no = new SelectListItem
                {
                    Text = "NO",
                    Value = "N"
                };
                List<SelectListItem> ddscrutiny = new List<SelectListItem>();
                ddscrutiny.Add(all);
                ddscrutiny.Add(pending);
                ddscrutiny.Add(complete);

                List<SelectListItem> ddsanction = new List<SelectListItem>();
                ddsanction.Add(all);
                ddsanction.Add(yes);
                ddsanction.Add(no);
                ViewData["STATE"] = ddState;
                ViewData["SCRUTINY"] = ddscrutiny;
                ViewData["SANCTION"] = ddsanction;
                ProposaleportsViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                ProposaleportsViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                return View(ProposaleportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult PropAnalysisDataGapReportListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();
                int stateCode = Convert.ToInt32(formCollection["StateCode"]);
                string type = formCollection["Type"];
                string scrutiny = formCollection["Scrutiny"];
                string sanctioned = formCollection["Sanctioned"];
                string report = formCollection["Report"];

                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords;

                var jsonData = new
                {
                    rows = proposalReportsBAL.PropAnalysisDataGapListingBAL(stateCode, type, scrutiny, sanctioned, report, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult PropAnalysisDetailsReportListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();
                int stateCode = Convert.ToInt32(formCollection["StateCode"]);
                int year = Convert.ToInt32(formCollection["Year"]);
                int batch = Convert.ToInt32(formCollection["Batch"]);
                string type = formCollection["Type"];
                string scrutiny = formCollection["Scrutiny"];
                string sanctioned = formCollection["Sanctioned"];
                string report = formCollection["Report"];
                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords;

                var jsonData = new
                {
                    rows = proposalReportsBAL.PropAnalysisDetailListingBAL(stateCode, year, batch, type, scrutiny, sanctioned, report, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult PropAnalysisHabitationReportListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();
                int roadcode = Convert.ToInt32(formCollection["Roadcode"]);
                string type = formCollection["Type"];

                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords;
                var jsonData = new
                        {
                            rows = proposalReportsBAL.PropAnalysisHabitationListingBAL(roadcode, page, rows, sidx, sord, out totalRecords),
                            total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                            page = page + 1,
                            records = totalRecords
                        };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }

        }

        [HttpPost]
        public ActionResult PropAnalysisTraffReportListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();
                int roadcode = Convert.ToInt32(formCollection["Roadcode"]);
                string type = formCollection["Type"];

                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords;

                var jsonData = new
                    {
                        rows = proposalReportsBAL.PropAnalysisTrafficListingBAL(roadcode, page, rows, sidx, sord, out totalRecords),
                        total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                        page = page + 1,
                        records = totalRecords
                    };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }


        }


        [HttpPost]
        public ActionResult PropAnalysisCBRReportListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();
                int roadcode = Convert.ToInt32(formCollection["Roadcode"]);
                string type = formCollection["Type"];

                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords;

                var jsonData = new
                {
                    rows = proposalReportsBAL.PropAnalysisCBRListingBAL(roadcode, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }


        }


        [HttpPost]
        public ActionResult PropAnalysisHabTraffCBRReportListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();
                int roadcode = Convert.ToInt32(formCollection["Roadcode"]);
                string type = formCollection["Type"];

                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords;
                //var jsonData = String.Empty;

                switch (type)
                {
                    case "Traff":
                        var jsonData1 = new
                           {
                               rows = proposalReportsBAL.PropAnalysisHabitationListingBAL(roadcode, page, rows, sidx, sord, out totalRecords),
                               total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                               page = page + 1,
                               records = totalRecords

                           };
                        return Json(jsonData1, JsonRequestBehavior.AllowGet);
                    case "Hab":
                        var jsonData2 = new
                         {
                             rows = proposalReportsBAL.PropAnalysisHabitationListingBAL(roadcode, page, rows, sidx, sord, out totalRecords),
                             total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                             page = page + 1,
                             records = totalRecords
                         };
                        return Json(jsonData2, JsonRequestBehavior.AllowGet);

                }
                //var jsonData = new
                //{
                //    rows = proposalReportsBAL.PropAnalysisHabitationListingBAL(roadcode, page, rows, sidx, sord, out totalRecords),
                //    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                //    page = page + 1,
                //    records = totalRecords
                //};
                return Json(String.Empty, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }


        }



        #endregion

        #region Proposal DataGap Report

        public ActionResult PropDataGapDetails()
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            ProposalReportsModel ProposaleportsViewModel = new ProposalReportsModel();
            ProposalReportsDAL proposalReportsDAL = new ProposalReportsDAL();
            try
            {
                List<SelectListItem> ddYear = objCommonFunctions.PopulateFinancialYear(true, true).ToList();
                int stateCode = PMGSYSession.Current.StateCode == 0 ? 1 : PMGSYSession.Current.StateCode;
                int allStateCode = PMGSYSession.Current.StateCode == 0 ? 0 : PMGSYSession.Current.StateCode;                
                int districtCode = 0;
                List<SelectListItem> ddState = objCommonFunctions.PopulateStates(false);
                List<SelectListItem> ddCarriageWidth = proposalReportsDAL.GetCarriageWidthList();
                List<SelectListItem> ddAgency = objCommonFunctions.PopulateMasterAgency(true);
                SelectListItem allCollabor = new SelectListItem
                {
                    Selected = true,
                    Text = "All",
                    Value = "0"
                };
                List<SelectListItem> ddCollaboration = objCommonFunctions.PopulateFundingAgency(true);
                ddCollaboration.RemoveAt(0);
                ddCollaboration.Insert(0, allCollabor);               


                ddYear.RemoveAt(0);
                ddYear.Insert(0, allCollabor);

                
                ddState.Find(x => x.Value == 1.ToString()).Selected = true;
              
                    if (stateCode > 0)  //if state login
                    {
                        ddState.Find(x => x.Value == stateCode.ToString()).Selected = true;
                    }
                    List<SelectListItem> ddDistrict = new List<SelectListItem>();
                    List<SelectListItem> ddBlock = new List<SelectListItem>();
                    if (stateCode == 0)
                    {
                        ddDistrict.Insert(0, (new SelectListItem { Text = "All Districts", Value = "0", Selected = true }));
                    }
                    else
                    {
                        ddDistrict = objCommonFunctions.PopulateDistrict(stateCode, true);
                        districtCode = PMGSYSession.Current.DistrictCode == 0 ? 0 : PMGSYSession.Current.DistrictCode;
                        ddDistrict.Find(x => x.Value == "-1").Value = "0";
                        ddDistrict.Find(x => x.Value == districtCode.ToString()).Selected = true;
                    }

                    if (districtCode == 0)
                    {
                        ddBlock.Insert(0, (new SelectListItem { Text = "All Blocks", Value = "0", Selected = true }));
                    }
                    else
                    {
                        ddBlock = objCommonFunctions.PopulateBlocks(districtCode, true);
                        ddBlock.Find(x => x.Value == "-1").Value = "0";
                        //BlockCode = PMGSYSession.Current.BlockCode == 0 ? 0 : PMGSYSession.Current.BlockCode;
                        //BlockList.Find(x => x.Value == BlockCode.ToString()).Selected = true;
                    }
                //AllState
                    List<SelectListItem> ddAllState = new List<SelectListItem>();
                    ddAllState = objCommonFunctions.PopulateStates(false);
                    ddAllState.Insert(0, allCollabor);                  
                    ddAllState.Find(x => x.Value ==allStateCode.ToString()).Selected = true;
                    
                    List<SelectListItem> ddAllDistrict = new List<SelectListItem>();
                    List<SelectListItem> ddAllBlock = new List<SelectListItem>();
                    if (allStateCode == 0)
                    {
                        ddAllDistrict.Insert(0, (new SelectListItem { Text = "All Districts", Value = "0", Selected = true }));
                    }
                    else
                    {
                        ddAllDistrict = objCommonFunctions.PopulateDistrict(allStateCode, true);
                        districtCode = PMGSYSession.Current.DistrictCode == 0 ? 0 : PMGSYSession.Current.DistrictCode;
                        ddAllDistrict.Find(x => x.Value == "-1").Value = "0";
                        ddAllDistrict.Find(x => x.Value == districtCode.ToString()).Selected = true;
                    }

                    if (districtCode == 0)
                    {
                        ddAllBlock.Insert(0, (new SelectListItem { Text = "All Blocks", Value = "0", Selected = true }));
                    }
                    else
                    {
                        ddAllBlock = objCommonFunctions.PopulateBlocks(districtCode, true);
                        ddAllBlock.Find(x => x.Value == "-1").Value = "0";
                        //BlockCode = PMGSYSession.Current.BlockCode == 0 ? 0 : PMGSYSession.Current.BlockCode;
                        //BlockList.Find(x => x.Value == BlockCode.ToString()).Selected = true;
                    }



                    SelectListItem all = new SelectListItem
                    {
                        Selected = true,
                        Text = "All",
                        Value = "%"
                    };

                    SelectListItem pending = new SelectListItem
                    {
                        Text = "Pending",
                        Value = "N"
                    };

                    SelectListItem newConstType = new SelectListItem
                    {
                        Text = "New",
                        Value = "N"
                    };
                    SelectListItem UpgradeConstType = new SelectListItem
                    {
                        Text = "Upgradation",
                        Value = "U"
                    };
                    SelectListItem complete = new SelectListItem
                    {
                        Text = "Complete",
                        Value = "Y"
                    };


                    SelectListItem yes = new SelectListItem
                    {
                        Text = "YES",
                        Value = "Y"
                    };
                    SelectListItem no = new SelectListItem
                    {
                        Text = "NO",
                        Value = "N"
                    };


                    SelectListItem l1000 = new SelectListItem
                    {
                        Text = "1000+",
                        // Value = "1000",                   
                        Value = "1"

                    };

                    SelectListItem l999 = new SelectListItem
                    {
                        Text = "499-999",
                        // Value = "999"
                        Value = "2"

                    };
                    SelectListItem l499 = new SelectListItem
                    {
                        Text = "250-499",
                        // Value = "499"
                        Value = "3"

                    };
                    SelectListItem l250 = new SelectListItem
                    {
                        Text = "<250",
                        //  Value = "250"
                        Value = "4"
                    };
                    List<SelectListItem> ddPopulation = new List<SelectListItem>();
                    ddPopulation.Add(allCollabor);
                    ddPopulation.Add(l250);
                    ddPopulation.Add(l499);
                    ddPopulation.Add(l999);
                    ddPopulation.Add(l1000);

                    List<SelectListItem> ddscrutiny = new List<SelectListItem>();
                    ddscrutiny.Add(all);
                    ddscrutiny.Add(pending);
                    ddscrutiny.Add(complete);

                    List<SelectListItem> ddsanction = new List<SelectListItem>();
                    ddsanction.Add(all);
                    ddsanction.Add(yes);
                    ddsanction.Add(no);
                    List<SelectListItem> ddBatch = objCommonFunctions.PopulateBatch();
                    ddBatch.RemoveAt(0);
                    ddBatch.Insert(0, allCollabor);

                    List<SelectListItem> ddConstruction = new List<SelectListItem>();
                    ddConstruction.Add(all);
                    ddConstruction.Add(newConstType);
                    ddConstruction.Add(UpgradeConstType);

                    List<SelectListItem> ddSTAStatus = new List<SelectListItem>();
                    ddSTAStatus.Add(all);
                    ddSTAStatus.Add(yes);
                    ddSTAStatus.Add(no);

                    List<SelectListItem> ddMRDStatus = new List<SelectListItem>();
                    ddMRDStatus.Add(all);
                    ddMRDStatus.Add(yes);
                    ddMRDStatus.Add(no);

                  
                    ViewData["STATE"] = ddState;
                    ViewData["DISTRICT"] = ddDistrict;
                    ViewData["BLOCK"] = ddBlock;
                    ViewData["STATEALL"] = ddAllState;
                    ViewData["DISTRICTALL"] = ddAllDistrict;
                    ViewData["BLOCKALL"] = ddAllBlock;
                    ViewData["AGENCY"] = ddAgency;
                    ViewData["COLLABORATION"] = ddCollaboration;
                    ViewData["StaSTATUS"] = ddSTAStatus;
                    ViewData["MrdSTATUS"] = ddMRDStatus;
                    ViewData["BLOCK"] = ddBlock;
                    ViewData["SCRUTINY"] = ddscrutiny;
                    ViewData["SANCTION"] = ddsanction;
                    ViewData["YEAR"] = ddYear;
                    ViewData["BATCH"] = ddBatch;
                    ViewData["CONSTRUCTION"] = ddConstruction;
                    ViewData["CARRIAGEWIDTH"] = ddCarriageWidth;
                    ViewData["POPULATION"] = ddPopulation;
                    ProposaleportsViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                    ProposaleportsViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                


                return View(ProposaleportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }


        #region Tab1  Proposals Not Mapped
        /// <summary>
        /// Tab1 Proposal Not Mapped State Listing
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PropNotMappedStateListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();
                int stateCode = 0;
                int districtCode = 0;
                int blockCode = 0;
                int yearCode = 0;
                int batchCode = 0;
                int collaborationCode = 0;
                int agencyCode = 0;
                string staStatusCode = "%";
                string mrdStatusCode = "%";

                int year = 0;
                string sanctioned = formCollection["Sanctioned"];
                if (!string.IsNullOrEmpty(formCollection["StateCode"]))
                {
                    stateCode = Convert.ToInt32(formCollection["StateCode"]);
                }
                stateCode = stateCode == 0 ? PMGSYSession.Current.StateCode : stateCode;
                if (!string.IsNullOrEmpty(formCollection["DistrictCode"]))
                {
                    districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
                }
                districtCode = districtCode == 0 ? PMGSYSession.Current.DistrictCode : districtCode;

                if (!string.IsNullOrEmpty(formCollection["BlockCode"]))
                {
                    blockCode = Convert.ToInt32(formCollection["BlockCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["YearCode"]))
                {
                    yearCode = Convert.ToInt32(formCollection["YearCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["BatchCode"]))
                {
                    batchCode = Convert.ToInt32(formCollection["BatchCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["CollaborationCode"]))
                {
                    collaborationCode = Convert.ToInt32(formCollection["CollaborationCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["AgencyCode"]))
                {
                    agencyCode = Convert.ToInt32(formCollection["AgencyCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["StaStatusCode"]))
                {
                    staStatusCode = formCollection["StaStatusCode"];
                }
                if (!string.IsNullOrEmpty(formCollection["MrdStatusCode"]))
                {
                    mrdStatusCode = formCollection["MrdStatusCode"];
                }
                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords;

                var jsonData = new
                {
                    rows = proposalReportsBAL.PropNotMappedStateListingBAL(stateCode, districtCode, blockCode, yearCode, batchCode, collaborationCode, agencyCode, staStatusCode, mrdStatusCode, sanctioned, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        /// <summary>
        /// Tab1 Proposal Not Mapped District Listing
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PropNotMappedDistrictListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();
                int stateCode = 0;
                int districtCode = 0;
                int blockCode = 0;
                int yearCode = 0;
                int batchCode = 0;
                int collaborationCode = 0;
                int agencyCode = 0;
                string staStatusCode = "%";
                string mrdStatusCode = "%";

                int year = 0;
                string sanctioned = formCollection["Sanctioned"];
                if (!string.IsNullOrEmpty(formCollection["StateCode"]))
                {
                    stateCode = Convert.ToInt32(formCollection["StateCode"]);
                }
                stateCode = stateCode == 0 ? PMGSYSession.Current.StateCode : stateCode;
                if (!string.IsNullOrEmpty(formCollection["DistrictCode"]))
                {
                    districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
                }
                districtCode = districtCode == 0 ? PMGSYSession.Current.DistrictCode : districtCode;

                if (!string.IsNullOrEmpty(formCollection["BlockCode"]))
                {
                    blockCode = Convert.ToInt32(formCollection["BlockCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["YearCode"]))
                {
                    yearCode = Convert.ToInt32(formCollection["YearCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["BatchCode"]))
                {
                    batchCode = Convert.ToInt32(formCollection["BatchCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["CollaborationCode"]))
                {
                    collaborationCode = Convert.ToInt32(formCollection["CollaborationCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["AgencyCode"]))
                {
                    agencyCode = Convert.ToInt32(formCollection["AgencyCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["StaStatusCode"]))
                {
                    staStatusCode = formCollection["StaStatusCode"];
                }
                if (!string.IsNullOrEmpty(formCollection["MrdStatusCode"]))
                {
                    mrdStatusCode = formCollection["MrdStatusCode"];
                }
                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords;

                var jsonData = new
                {
                    rows = proposalReportsBAL.PropNotMappedDistrictListingBAL(stateCode, districtCode, blockCode, yearCode, batchCode, collaborationCode, agencyCode, staStatusCode, mrdStatusCode, sanctioned, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        /// <summary>
        /// Tab1 Proposal Not Mapped Details Listing
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PropNotMappedDetailsListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();
                int stateCode = 0;
                int districtCode = 0;
                int blockCode = 0;
                int yearCode = 0;
                int batchCode = 0;
                int collaborationCode = 0;
                int agencyCode = 0;
                string staStatusCode = "%";
                string mrdStatusCode = "%";

                int year = 0;
                string sanctioned = formCollection["Sanctioned"];
                if (!string.IsNullOrEmpty(formCollection["StateCode"]))
                {
                    stateCode = Convert.ToInt32(formCollection["StateCode"]);
                }
                stateCode = stateCode == 0 ? PMGSYSession.Current.StateCode : stateCode;
                if (!string.IsNullOrEmpty(formCollection["DistrictCode"]))
                {
                    districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
                }
                districtCode = districtCode == 0 ? PMGSYSession.Current.DistrictCode : districtCode;

                if (!string.IsNullOrEmpty(formCollection["BlockCode"]))
                {
                    blockCode = Convert.ToInt32(formCollection["BlockCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["YearCode"]))
                {
                    yearCode = Convert.ToInt32(formCollection["YearCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["BatchCode"]))
                {
                    batchCode = Convert.ToInt32(formCollection["BatchCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["CollaborationCode"]))
                {
                    collaborationCode = Convert.ToInt32(formCollection["CollaborationCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["AgencyCode"]))
                {
                    agencyCode = Convert.ToInt32(formCollection["AgencyCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["StaStatusCode"]))
                {
                    staStatusCode = formCollection["StaStatusCode"];
                }
                if (!string.IsNullOrEmpty(formCollection["MrdStatusCode"]))
                {
                    mrdStatusCode = formCollection["MrdStatusCode"];
                }
                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords;

                var jsonData = new
                {
                    rows = proposalReportsBAL.PropNotMappedDetailsListingBAL(stateCode, districtCode, blockCode, yearCode, batchCode, collaborationCode, agencyCode, staStatusCode, mrdStatusCode, sanctioned, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        /// <summary>
        /// Tab1 Proposal Not Mapped Phase View Listing
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PropNotMappedPhaseViewListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();
                int stateCode = 0;
                int districtCode = 0;
                int blockCode = 0;
                int yearCode = 0;
                int batchCode = 0;
                int collaborationCode = 0;
                int agencyCode = 0;
                string staStatusCode = "%";
                string mrdStatusCode = "%";

                int year = 0;
                string sanctioned = formCollection["Sanctioned"];
                if (!string.IsNullOrEmpty(formCollection["StateCode"]))
                {
                    stateCode = Convert.ToInt32(formCollection["StateCode"]);
                }
                stateCode = stateCode == 0 ? PMGSYSession.Current.StateCode : stateCode;
                if (!string.IsNullOrEmpty(formCollection["DistrictCode"]))
                {
                    districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
                }
                districtCode = districtCode == 0 ? PMGSYSession.Current.DistrictCode : districtCode;

                if (!string.IsNullOrEmpty(formCollection["BlockCode"]))
                {
                    blockCode = Convert.ToInt32(formCollection["BlockCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["YearCode"]))
                {
                    yearCode = Convert.ToInt32(formCollection["YearCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["BatchCode"]))
                {
                    batchCode = Convert.ToInt32(formCollection["BatchCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["CollaborationCode"]))
                {
                    collaborationCode = Convert.ToInt32(formCollection["CollaborationCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["AgencyCode"]))
                {
                    agencyCode = Convert.ToInt32(formCollection["AgencyCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["StaStatusCode"]))
                {
                    staStatusCode = formCollection["StaStatusCode"];
                }
                if (!string.IsNullOrEmpty(formCollection["MrdStatusCode"]))
                {
                    mrdStatusCode = formCollection["MrdStatusCode"];
                }
                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords;

                var jsonData = new
                {
                    rows = proposalReportsBAL.PropNotMappedPhaseViewListingBAL(stateCode, districtCode, blockCode, yearCode, batchCode, collaborationCode, agencyCode, staStatusCode, mrdStatusCode, sanctioned, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }


        #endregion

        #region Tab2 Road Number Based CN
        /// <summary>
        /// Tab2 Road Number Base DistrictWise Listing
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PropNumberBaseCNDistrictListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();
                int stateCode = 0;
                int districtCode = 0;
                int blockCode = 0;
                int yearCode = 0;
                int batchCode = 0;
                int collaborationCode = 0;
                int agencyCode = 0;
                string staStatusCode = "%";
                string mrdStatusCode = "%";

                int year = 0;
                if (!string.IsNullOrEmpty(formCollection["StateCode"]))
                {
                    stateCode = Convert.ToInt32(formCollection["StateCode"]);
                }
                stateCode = stateCode == 0 ? PMGSYSession.Current.StateCode : stateCode;
                if (!string.IsNullOrEmpty(formCollection["DistrictCode"]))
                {
                    districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
                }
                districtCode = districtCode == 0 ? PMGSYSession.Current.DistrictCode : districtCode;

                if (!string.IsNullOrEmpty(formCollection["BlockCode"]))
                {
                    blockCode = Convert.ToInt32(formCollection["BlockCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["YearCode"]))
                {
                    yearCode = Convert.ToInt32(formCollection["YearCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["BatchCode"]))
                {
                    batchCode = Convert.ToInt32(formCollection["BatchCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["CollaborationCode"]))
                {
                    collaborationCode = Convert.ToInt32(formCollection["CollaborationCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["AgencyCode"]))
                {
                    agencyCode = Convert.ToInt32(formCollection["AgencyCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["StaStatusCode"]))
                {
                    staStatusCode = formCollection["StaStatusCode"];
                }
                if (!string.IsNullOrEmpty(formCollection["MrdStatusCode"]))
                {
                    mrdStatusCode = formCollection["MrdStatusCode"];
                }
                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords;

                var jsonData = new
                {
                    rows = proposalReportsBAL.PropNumberBaseCNDistrictListingBAL(stateCode, districtCode, blockCode, yearCode, batchCode, collaborationCode, agencyCode, staStatusCode, mrdStatusCode, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        /// <summary>
        /// Tab2 Road Number Base Detail Listing
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PropNumberBaseCNRoadDetailsListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();
                int stateCode = 0;
                int districtCode = 0;
                int blockCode = 0;
                int yearCode = 0;
                int batchCode = 0;
                int collaborationCode = 0;
                int agencyCode = 0;
                string staStatusCode = "%";
                string mrdStatusCode = "%";

                int year = 0;
                if (!string.IsNullOrEmpty(formCollection["StateCode"]))
                {
                    stateCode = Convert.ToInt32(formCollection["StateCode"]);
                }
                stateCode = stateCode == 0 ? PMGSYSession.Current.StateCode : stateCode;
                if (!string.IsNullOrEmpty(formCollection["DistrictCode"]))
                {
                    districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
                }
                districtCode = districtCode == 0 ? PMGSYSession.Current.DistrictCode : districtCode;

                if (!string.IsNullOrEmpty(formCollection["BlockCode"]))
                {
                    blockCode = Convert.ToInt32(formCollection["BlockCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["YearCode"]))
                {
                    yearCode = Convert.ToInt32(formCollection["YearCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["BatchCode"]))
                {
                    batchCode = Convert.ToInt32(formCollection["BatchCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["CollaborationCode"]))
                {
                    collaborationCode = Convert.ToInt32(formCollection["CollaborationCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["AgencyCode"]))
                {
                    agencyCode = Convert.ToInt32(formCollection["AgencyCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["StaStatusCode"]))
                {
                    staStatusCode = formCollection["StaStatusCode"];
                }
                if (!string.IsNullOrEmpty(formCollection["MrdStatusCode"]))
                {
                    mrdStatusCode = formCollection["MrdStatusCode"];
                }
                string roadNumber = formCollection["RoadNumber"];
                string report = formCollection["Report"];
                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords;

                var jsonData = new
                {
                    rows = proposalReportsBAL.PropNumberBaseCNRoadDetailsListingBAL(stateCode, districtCode, blockCode, yearCode, batchCode, collaborationCode, agencyCode, staStatusCode, mrdStatusCode, roadNumber, report, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        #endregion

        #region Tab 3 Multiple Proposals mapped to CN Road
        /// <summary>
        /// Tab3 Multiple Proposals DistrictWise Listing
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PropMultipleDistrictListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();
                int stateCode = 0;
                int districtCode = 0;
                int blockCode = 0;
                int yearCode = 0;
                int batchCode = 0;
                int collaborationCode = 0;
                int agencyCode = 0;
                string staStatusCode = "%";
                string mrdStatusCode = "%";

                int year = 0;
                if (!string.IsNullOrEmpty(formCollection["StateCode"]))
                {
                    stateCode = Convert.ToInt32(formCollection["StateCode"]);
                }
                stateCode = stateCode == 0 ? PMGSYSession.Current.StateCode : stateCode;
                if (!string.IsNullOrEmpty(formCollection["DistrictCode"]))
                {
                    districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
                }
                districtCode = districtCode == 0 ? PMGSYSession.Current.DistrictCode : districtCode;

                if (!string.IsNullOrEmpty(formCollection["BlockCode"]))
                {
                    blockCode = Convert.ToInt32(formCollection["BlockCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["YearCode"]))
                {
                    yearCode = Convert.ToInt32(formCollection["YearCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["BatchCode"]))
                {
                    batchCode = Convert.ToInt32(formCollection["BatchCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["CollaborationCode"]))
                {
                    collaborationCode = Convert.ToInt32(formCollection["CollaborationCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["AgencyCode"]))
                {
                    agencyCode = Convert.ToInt32(formCollection["AgencyCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["StaStatusCode"]))
                {
                    staStatusCode = formCollection["StaStatusCode"];
                }
                if (!string.IsNullOrEmpty(formCollection["MrdStatusCode"]))
                {
                    mrdStatusCode = formCollection["MrdStatusCode"];
                }
                string sanctioned = formCollection["Sanctioned"];
                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords;

                var jsonData = new
                {
                    rows = proposalReportsBAL.PropMultipleDistrictListingBAL(stateCode, districtCode, blockCode, yearCode, batchCode, collaborationCode, agencyCode, staStatusCode, mrdStatusCode, sanctioned, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }


        /// <summary>
        /// Tab3 Multiple Proposals Details Listing
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PropMultipleDetailsListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();
                int stateCode = 0;
                int districtCode = 0;
                int blockCode = 0;
                int yearCode = 0;
                int batchCode = 0;
                int collaborationCode = 0;
                int agencyCode = 0;
                string staStatusCode = "%";
                string mrdStatusCode = "%";               
                if (!string.IsNullOrEmpty(formCollection["StateCode"]))
                {
                    stateCode = Convert.ToInt32(formCollection["StateCode"]);
                }
                stateCode = stateCode == 0 ? PMGSYSession.Current.StateCode : stateCode;
                if (!string.IsNullOrEmpty(formCollection["DistrictCode"]))
                {
                    districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
                }
                districtCode = districtCode == 0 ? PMGSYSession.Current.DistrictCode : districtCode;

                if (!string.IsNullOrEmpty(formCollection["BlockCode"]))
                {
                    blockCode = Convert.ToInt32(formCollection["BlockCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["YearCode"]))
                {
                    yearCode = Convert.ToInt32(formCollection["YearCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["BatchCode"]))
                {
                    batchCode = Convert.ToInt32(formCollection["BatchCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["CollaborationCode"]))
                {
                    collaborationCode = Convert.ToInt32(formCollection["CollaborationCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["AgencyCode"]))
                {
                    agencyCode = Convert.ToInt32(formCollection["AgencyCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["StaStatusCode"]))
                {
                    staStatusCode = formCollection["StaStatusCode"];
                }
                if (!string.IsNullOrEmpty(formCollection["MrdStatusCode"]))
                {
                    mrdStatusCode = formCollection["MrdStatusCode"];
                }
                string roadNumber = formCollection["RoadNumber"];               
                string sanctioned = formCollection["Sanctioned"];
                string report = formCollection["Report"];

                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords;

                var jsonData = new
                {
                    rows = proposalReportsBAL.PropMultipleDetailsListingBAL(stateCode, districtCode, blockCode, yearCode, batchCode, collaborationCode, agencyCode, staStatusCode, mrdStatusCode, roadNumber, sanctioned, report, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }

        }
        #endregion

        #region Tab 4 Single Habitation
        /// <summary>
        /// Tab4 Single Habitation Proposals StateWise Listing
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PropSingleHabStateListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();
                int stateCode = 0;
                int districtCode = 0;
                int blockCode = 0;
                int yearCode = 0;
                int batchCode = 0;
                int collaborationCode = 0;
                int agencyCode = 0;
                string staStatusCode = "%";
                string mrdStatusCode = "%";

                int year = 0;
                if (!string.IsNullOrEmpty(formCollection["StateCode"]))
                {
                    stateCode = Convert.ToInt32(formCollection["StateCode"]);
                }
                stateCode = stateCode == 0 ? PMGSYSession.Current.StateCode : stateCode;
                if (!string.IsNullOrEmpty(formCollection["DistrictCode"]))
                {
                    districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
                }
                districtCode = districtCode == 0 ? PMGSYSession.Current.DistrictCode : districtCode;

                if (!string.IsNullOrEmpty(formCollection["BlockCode"]))
                {
                    blockCode = Convert.ToInt32(formCollection["BlockCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["YearCode"]))
                {
                    yearCode = Convert.ToInt32(formCollection["YearCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["BatchCode"]))
                {
                    batchCode = Convert.ToInt32(formCollection["BatchCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["CollaborationCode"]))
                {
                    collaborationCode = Convert.ToInt32(formCollection["CollaborationCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["AgencyCode"]))
                {
                    agencyCode = Convert.ToInt32(formCollection["AgencyCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["StaStatusCode"]))
                {
                    staStatusCode = formCollection["StaStatusCode"];
                }
                if (!string.IsNullOrEmpty(formCollection["MrdStatusCode"]))
                {
                    mrdStatusCode = formCollection["MrdStatusCode"];
                }
                string sanctioned = formCollection["Sanctioned"];
                int population = Convert.ToInt32(formCollection["Population"]);
                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords;

                var jsonData = new
                {
                    rows = proposalReportsBAL.PropSingleHabStateListingBAL(stateCode, districtCode, blockCode, yearCode, batchCode, collaborationCode, agencyCode, staStatusCode, mrdStatusCode, population, sanctioned, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        /// <summary>
        /// Tab4 Single Habitation Proposals DistrictWise Listing
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PropSingleHabDistrictListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();
                int stateCode = 0;
                int districtCode = 0;
                int blockCode = 0;
                int yearCode = 0;
                int batchCode = 0;
                int collaborationCode = 0;
                int agencyCode = 0;
                string staStatusCode = "%";
                string mrdStatusCode = "%";

                int year = 0;
                if (!string.IsNullOrEmpty(formCollection["StateCode"]))
                {
                    stateCode = Convert.ToInt32(formCollection["StateCode"]);
                }
                stateCode = stateCode == 0 ? PMGSYSession.Current.StateCode : stateCode;
                if (!string.IsNullOrEmpty(formCollection["DistrictCode"]))
                {
                    districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
                }
                districtCode = districtCode == 0 ? PMGSYSession.Current.DistrictCode : districtCode;

                if (!string.IsNullOrEmpty(formCollection["BlockCode"]))
                {
                    blockCode = Convert.ToInt32(formCollection["BlockCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["YearCode"]))
                {
                    yearCode = Convert.ToInt32(formCollection["YearCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["BatchCode"]))
                {
                    batchCode = Convert.ToInt32(formCollection["BatchCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["CollaborationCode"]))
                {
                    collaborationCode = Convert.ToInt32(formCollection["CollaborationCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["AgencyCode"]))
                {
                    agencyCode = Convert.ToInt32(formCollection["AgencyCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["StaStatusCode"]))
                {
                    staStatusCode = formCollection["StaStatusCode"];
                }
                if (!string.IsNullOrEmpty(formCollection["MrdStatusCode"]))
                {
                    mrdStatusCode = formCollection["MrdStatusCode"];
                }
                string sanctioned = formCollection["Sanctioned"];
                int population = Convert.ToInt32(formCollection["Population"]);
                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords;

                var jsonData = new
                {
                    rows = proposalReportsBAL.PropSingleHabDistrictListingBAL(stateCode, districtCode, blockCode, yearCode, batchCode, collaborationCode, agencyCode, staStatusCode, mrdStatusCode, population, sanctioned, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }


        /// <summary>
        /// Tab4 Single Habitation Proposals Detail Listing
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PropSingleHabDetailsListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();
                int stateCode = 0;
                int districtCode = 0;
                int blockCode = 0;
                int yearCode = 0;
                int batchCode = 0;
                int collaborationCode = 0;
                int agencyCode = 0;
                string staStatusCode = "%";
                string mrdStatusCode = "%";

                int year = 0;
                if (!string.IsNullOrEmpty(formCollection["StateCode"]))
                {
                    stateCode = Convert.ToInt32(formCollection["StateCode"]);
                }
                stateCode = stateCode == 0 ? PMGSYSession.Current.StateCode : stateCode;
                if (!string.IsNullOrEmpty(formCollection["DistrictCode"]))
                {
                    districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
                }
                districtCode = districtCode == 0 ? PMGSYSession.Current.DistrictCode : districtCode;

                if (!string.IsNullOrEmpty(formCollection["BlockCode"]))
                {
                    blockCode = Convert.ToInt32(formCollection["BlockCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["YearCode"]))
                {
                    yearCode = Convert.ToInt32(formCollection["YearCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["BatchCode"]))
                {
                    batchCode = Convert.ToInt32(formCollection["BatchCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["CollaborationCode"]))
                {
                    collaborationCode = Convert.ToInt32(formCollection["CollaborationCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["AgencyCode"]))
                {
                    agencyCode = Convert.ToInt32(formCollection["AgencyCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["StaStatusCode"]))
                {
                    staStatusCode = formCollection["StaStatusCode"];
                }
                if (!string.IsNullOrEmpty(formCollection["MrdStatusCode"]))
                {
                    mrdStatusCode = formCollection["MrdStatusCode"];
                }
                string sanctioned = formCollection["Sanctioned"];
                int population = Convert.ToInt32(formCollection["Population"]);
                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords;

                var jsonData = new
                {
                    rows = proposalReportsBAL.PropSingleHabDetailsListingBAL(stateCode, districtCode, blockCode, yearCode, batchCode, collaborationCode, agencyCode, staStatusCode, mrdStatusCode, population, sanctioned, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        #endregion

        #region Tab 5 Zero Maintenance Cost
        /// <summary>
        /// Tab5 Zero Maintenance Proposals StateWise Listing
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PropZeroMaintStateListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();
                int stateCode = 0;
                int districtCode = 0;
                int blockCode = 0;
                int yearCode = 0;
                int batchCode = 0;
                int collaborationCode = 0;
                int agencyCode = 0;
                string staStatusCode = "%";
                string mrdStatusCode = "%";

                int year = 0;
                if (!string.IsNullOrEmpty(formCollection["StateCode"]))
                {
                    stateCode = Convert.ToInt32(formCollection["StateCode"]);
                }
                stateCode = stateCode == 0 ? PMGSYSession.Current.StateCode : stateCode;
                if (!string.IsNullOrEmpty(formCollection["DistrictCode"]))
                {
                    districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
                }
                districtCode = districtCode == 0 ? PMGSYSession.Current.DistrictCode : districtCode;

                if (!string.IsNullOrEmpty(formCollection["BlockCode"]))
                {
                    blockCode = Convert.ToInt32(formCollection["BlockCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["YearCode"]))
                {
                    yearCode = Convert.ToInt32(formCollection["YearCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["BatchCode"]))
                {
                    batchCode = Convert.ToInt32(formCollection["BatchCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["CollaborationCode"]))
                {
                    collaborationCode = Convert.ToInt32(formCollection["CollaborationCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["AgencyCode"]))
                {
                    agencyCode = Convert.ToInt32(formCollection["AgencyCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["StaStatusCode"]))
                {
                    staStatusCode = formCollection["StaStatusCode"];
                }
                if (!string.IsNullOrEmpty(formCollection["MrdStatusCode"]))
                {
                    mrdStatusCode = formCollection["MrdStatusCode"];
                }
                string sanctioned = formCollection["Sanctioned"];
                string constructionType = formCollection["ConstructionType"];

                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords;

                var jsonData = new
                {
                    rows = proposalReportsBAL.PropZeroMaintStateListingBAL(stateCode, districtCode, blockCode, yearCode, batchCode, collaborationCode, agencyCode, staStatusCode, mrdStatusCode, constructionType, sanctioned, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }


        /// <summary>
        /// Tab5 Zero Maintenance Proposals DistrictWise Listing
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PropZeroMaintDistrictListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();
                int stateCode = 0;
                int districtCode = 0;
                int blockCode = 0;
                int yearCode = 0;
                int batchCode = 0;
                int collaborationCode = 0;
                int agencyCode = 0;
                string staStatusCode = "%";
                string mrdStatusCode = "%";

                int year = 0;
                if (!string.IsNullOrEmpty(formCollection["StateCode"]))
                {
                    stateCode = Convert.ToInt32(formCollection["StateCode"]);
                }
                stateCode = stateCode == 0 ? PMGSYSession.Current.StateCode : stateCode;
                if (!string.IsNullOrEmpty(formCollection["DistrictCode"]))
                {
                    districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
                }
                districtCode = districtCode == 0 ? PMGSYSession.Current.DistrictCode : districtCode;

                if (!string.IsNullOrEmpty(formCollection["BlockCode"]))
                {
                    blockCode = Convert.ToInt32(formCollection["BlockCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["YearCode"]))
                {
                    yearCode = Convert.ToInt32(formCollection["YearCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["BatchCode"]))
                {
                    batchCode = Convert.ToInt32(formCollection["BatchCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["CollaborationCode"]))
                {
                    collaborationCode = Convert.ToInt32(formCollection["CollaborationCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["AgencyCode"]))
                {
                    agencyCode = Convert.ToInt32(formCollection["AgencyCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["StaStatusCode"]))
                {
                    staStatusCode = formCollection["StaStatusCode"];
                }
                if (!string.IsNullOrEmpty(formCollection["MrdStatusCode"]))
                {
                    mrdStatusCode = formCollection["MrdStatusCode"];
                }
                string sanctioned = formCollection["Sanctioned"];
                string constructionType = formCollection["ConstructionType"];

                //string scrutiny = formCollection["Scrutiny"];
                //string sanctioned = formCollection["Sanctioned"];
                //string report = formCollection["Report"];

                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords;

                var jsonData = new
                {
                    rows = proposalReportsBAL.PropZeroMaintDistrictListingBAL(stateCode, districtCode, blockCode, yearCode, batchCode, collaborationCode, agencyCode, staStatusCode, mrdStatusCode, constructionType, sanctioned, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }


        /// <summary>
        /// Tab5 Zero Maintenance Proposals Details Listing
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PropZeroMaintDetailsListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();

                int stateCode = 0;
                int districtCode = 0;
                int blockCode = 0;
                int yearCode = 0;
                int batchCode = 0;
                int collaborationCode = 0;
                int agencyCode = 0;
                string staStatusCode = "%";
                string mrdStatusCode = "%";

                int year = 0;
                if (!string.IsNullOrEmpty(formCollection["StateCode"]))
                {
                    stateCode = Convert.ToInt32(formCollection["StateCode"]);
                }
                stateCode = stateCode == 0 ? PMGSYSession.Current.StateCode : stateCode;
                if (!string.IsNullOrEmpty(formCollection["DistrictCode"]))
                {
                    districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
                }
                districtCode = districtCode == 0 ? PMGSYSession.Current.DistrictCode : districtCode;

                if (!string.IsNullOrEmpty(formCollection["BlockCode"]))
                {
                    blockCode = Convert.ToInt32(formCollection["BlockCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["YearCode"]))
                {
                    yearCode = Convert.ToInt32(formCollection["YearCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["BatchCode"]))
                {
                    batchCode = Convert.ToInt32(formCollection["BatchCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["CollaborationCode"]))
                {
                    collaborationCode = Convert.ToInt32(formCollection["CollaborationCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["AgencyCode"]))
                {
                    agencyCode = Convert.ToInt32(formCollection["AgencyCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["StaStatusCode"]))
                {
                    staStatusCode = formCollection["StaStatusCode"];
                }
                if (!string.IsNullOrEmpty(formCollection["MrdStatusCode"]))
                {
                    mrdStatusCode = formCollection["MrdStatusCode"];
                }
                string sanctioned = formCollection["Sanctioned"];
                string constructionType = formCollection["ConstructionType"];
                string report = formCollection["Report"];
                //string scrutiny = formCollection["Scrutiny"];
                //string sanctioned = formCollection["Sanctioned"];
                //string report = formCollection["Report"];

                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords;

                var jsonData = new
                {
                    rows = proposalReportsBAL.PropZeroMaintDetailsListingBAL(stateCode, districtCode, blockCode, yearCode, batchCode, collaborationCode, agencyCode, staStatusCode, mrdStatusCode, constructionType, sanctioned, report, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        #endregion

        #region Tab 6 Carriage way width
        /// <summary>
        /// Tab6 Carriage Width Proposals StateWise Listing
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PropCarriageWidthStateListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();
                int stateCode = 0;
                int districtCode = 0;
                int blockCode = 0;
                int yearCode = 0;
                int batchCode = 0;
                int collaborationCode = 0;
                int agencyCode = 0;
                string staStatusCode = "%";
                string mrdStatusCode = "%";

                int year = 0;
                if (!string.IsNullOrEmpty(formCollection["StateCode"]))
                {
                    stateCode = Convert.ToInt32(formCollection["StateCode"]);
                }
                stateCode = stateCode == 0 ? PMGSYSession.Current.StateCode : stateCode;
                if (!string.IsNullOrEmpty(formCollection["DistrictCode"]))
                {
                    districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
                }
                districtCode = districtCode == 0 ? PMGSYSession.Current.DistrictCode : districtCode;

                if (!string.IsNullOrEmpty(formCollection["BlockCode"]))
                {
                    blockCode = Convert.ToInt32(formCollection["BlockCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["YearCode"]))
                {
                    yearCode = Convert.ToInt32(formCollection["YearCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["BatchCode"]))
                {
                    batchCode = Convert.ToInt32(formCollection["BatchCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["CollaborationCode"]))
                {
                    collaborationCode = Convert.ToInt32(formCollection["CollaborationCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["AgencyCode"]))
                {
                    agencyCode = Convert.ToInt32(formCollection["AgencyCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["StaStatusCode"]))
                {
                    staStatusCode = formCollection["StaStatusCode"];
                }
                if (!string.IsNullOrEmpty(formCollection["MrdStatusCode"]))
                {
                    mrdStatusCode = formCollection["MrdStatusCode"];
                }
                string sanctioned = formCollection["Sanctioned"]; 
                int carriageWidth = Convert.ToInt32(formCollection["CarriageWidth"]);
                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords;

                var jsonData = new
                {
                    rows = proposalReportsBAL.PropCarriageWidthStateListingBAL(stateCode, districtCode, blockCode, yearCode, batchCode, collaborationCode, agencyCode, staStatusCode, mrdStatusCode, carriageWidth, sanctioned, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        /// <summary>
        /// Tab6 Carriage Width Proposals DistrictWise Listing
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PropCarriageWidthDistrictListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();
                int stateCode = 0;
                int districtCode = 0;
                int blockCode = 0;
                int yearCode = 0;
                int batchCode = 0;
                int collaborationCode = 0;
                int agencyCode = 0;
                string staStatusCode = "%";
                string mrdStatusCode = "%";

                int year = 0;
                if (!string.IsNullOrEmpty(formCollection["StateCode"]))
                {
                    stateCode = Convert.ToInt32(formCollection["StateCode"]);
                }
                stateCode = stateCode == 0 ? PMGSYSession.Current.StateCode : stateCode;
                if (!string.IsNullOrEmpty(formCollection["DistrictCode"]))
                {
                    districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
                }
                districtCode = districtCode == 0 ? PMGSYSession.Current.DistrictCode : districtCode;

                if (!string.IsNullOrEmpty(formCollection["BlockCode"]))
                {
                    blockCode = Convert.ToInt32(formCollection["BlockCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["YearCode"]))
                {
                    yearCode = Convert.ToInt32(formCollection["YearCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["BatchCode"]))
                {
                    batchCode = Convert.ToInt32(formCollection["BatchCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["CollaborationCode"]))
                {
                    collaborationCode = Convert.ToInt32(formCollection["CollaborationCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["AgencyCode"]))
                {
                    agencyCode = Convert.ToInt32(formCollection["AgencyCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["StaStatusCode"]))
                {
                    staStatusCode = formCollection["StaStatusCode"];
                }
                if (!string.IsNullOrEmpty(formCollection["MrdStatusCode"]))
                {
                    mrdStatusCode = formCollection["MrdStatusCode"];
                }
                string sanctioned = "%";
                int carriageWidth = Convert.ToInt32(formCollection["CarriageWidth"]);

                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords;

                var jsonData = new
                {
                    rows = proposalReportsBAL.PropCarriageWidthDistrictListingBAL(stateCode, districtCode, blockCode, yearCode, batchCode, collaborationCode, agencyCode, staStatusCode, mrdStatusCode, carriageWidth, sanctioned, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }


        /// <summary>
        /// Tab6 Carriage Width Proposals DistrictWise Listing
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PropCarriageWidthDetailsListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();
                int stateCode = 0;
                int districtCode = 0;
                int blockCode = 0;
                int yearCode = 0;
                int batchCode = 0;
                int collaborationCode = 0;
                int agencyCode = 0;
                string staStatusCode = "%";
                string mrdStatusCode = "%";

                int year = 0;
                if (!string.IsNullOrEmpty(formCollection["StateCode"]))
                {
                    stateCode = Convert.ToInt32(formCollection["StateCode"]);
                }
                stateCode = stateCode == 0 ? PMGSYSession.Current.StateCode : stateCode;
                if (!string.IsNullOrEmpty(formCollection["DistrictCode"]))
                {
                    districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
                }
                districtCode = districtCode == 0 ? PMGSYSession.Current.DistrictCode : districtCode;

                if (!string.IsNullOrEmpty(formCollection["BlockCode"]))
                {
                    blockCode = Convert.ToInt32(formCollection["BlockCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["YearCode"]))
                {
                    yearCode = Convert.ToInt32(formCollection["YearCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["BatchCode"]))
                {
                    batchCode = Convert.ToInt32(formCollection["BatchCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["CollaborationCode"]))
                {
                    collaborationCode = Convert.ToInt32(formCollection["CollaborationCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["AgencyCode"]))
                {
                    agencyCode = Convert.ToInt32(formCollection["AgencyCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["StaStatusCode"]))
                {
                    staStatusCode = formCollection["StaStatusCode"];
                }
                if (!string.IsNullOrEmpty(formCollection["MrdStatusCode"]))
                {
                    mrdStatusCode = formCollection["MrdStatusCode"];
                }
                string sanctioned = "%";
                int carriageWidth = Convert.ToInt32(formCollection["CarriageWidth"]);
                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords;

                var jsonData = new
                {
                    rows = proposalReportsBAL.PropCarriageWidthDetailsListingBAL(stateCode, districtCode, blockCode, yearCode, batchCode, collaborationCode, agencyCode, staStatusCode, mrdStatusCode, carriageWidth, sanctioned, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }


        #endregion

        #region Tab 7  Variation in Proposed
        /// <summary>
        /// Tab7 Variation Length Proposals  Listing
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PropVariationLengthListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();

                int stateCode = 0;
                int districtCode = 0;
                int blockCode = 0;
                int yearCode = 0;
                int batchCode = 0;
                int collaborationCode = 0;
                int agencyCode = 0;
                string staStatusCode = "%";
                string mrdStatusCode = "%";

                int year = 0;
                if (!string.IsNullOrEmpty(formCollection["StateCode"]))
                {
                    stateCode = Convert.ToInt32(formCollection["StateCode"]);
                }
                stateCode = stateCode == 0 ? PMGSYSession.Current.StateCode : stateCode;
                if (!string.IsNullOrEmpty(formCollection["DistrictCode"]))
                {
                    districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
                }
                districtCode = districtCode == 0 ? PMGSYSession.Current.DistrictCode : districtCode;

                if (!string.IsNullOrEmpty(formCollection["BlockCode"]))
                {
                    blockCode = Convert.ToInt32(formCollection["BlockCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["YearCode"]))
                {
                    yearCode = Convert.ToInt32(formCollection["YearCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["BatchCode"]))
                {
                    batchCode = Convert.ToInt32(formCollection["BatchCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["CollaborationCode"]))
                {
                    collaborationCode = Convert.ToInt32(formCollection["CollaborationCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["AgencyCode"]))
                {
                    agencyCode = Convert.ToInt32(formCollection["AgencyCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["StaStatusCode"]))
                {
                    staStatusCode = formCollection["StaStatusCode"];
                }
                if (!string.IsNullOrEmpty(formCollection["MrdStatusCode"]))
                {
                    mrdStatusCode = formCollection["MrdStatusCode"];
                }
                string sanctioned = formCollection["Sanctioned"];

                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords;

                var jsonData = new
                {
                    rows = proposalReportsBAL.PropVariationLengthListingBAL(stateCode, districtCode, blockCode, yearCode, batchCode, collaborationCode, agencyCode, staStatusCode, mrdStatusCode, sanctioned, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }
        #endregion

        #region Tab 8 Misclassification in Proposed
        /// <summary>
        /// Tab7 Variation Length Proposals  Listing
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PropMisclassificationListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();
                int stateCode = 0;
                int districtCode = 0;
                int blockCode = 0;
                int yearCode = 0;
                int batchCode = 0;
                int collaborationCode = 0;
                int agencyCode = 0;
                string staStatusCode = "%";
                string mrdStatusCode = "%";

                int year = 0;
                if (!string.IsNullOrEmpty(formCollection["StateCode"]))
                {
                    stateCode = Convert.ToInt32(formCollection["StateCode"]);
                }
                stateCode = stateCode == 0 ? PMGSYSession.Current.StateCode : stateCode;
                if (!string.IsNullOrEmpty(formCollection["DistrictCode"]))
                {
                    districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
                }
                districtCode = districtCode == 0 ? PMGSYSession.Current.DistrictCode : districtCode;

                if (!string.IsNullOrEmpty(formCollection["BlockCode"]))
                {
                    blockCode = Convert.ToInt32(formCollection["BlockCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["YearCode"]))
                {
                    yearCode = Convert.ToInt32(formCollection["YearCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["BatchCode"]))
                {
                    batchCode = Convert.ToInt32(formCollection["BatchCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["CollaborationCode"]))
                {
                    collaborationCode = Convert.ToInt32(formCollection["CollaborationCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["AgencyCode"]))
                {
                    agencyCode = Convert.ToInt32(formCollection["AgencyCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["StaStatusCode"]))
                {
                    staStatusCode = formCollection["StaStatusCode"];
                }
                if (!string.IsNullOrEmpty(formCollection["MrdStatusCode"]))
                {
                    mrdStatusCode = formCollection["MrdStatusCode"];
                } 
                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords;

                var jsonData = new
                {
                    rows = proposalReportsBAL.PropMisclassificationListingBAL(stateCode, districtCode, blockCode, yearCode, batchCode, collaborationCode, agencyCode, staStatusCode, mrdStatusCode, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }
        /// <summary>
        /// Tab7 Variation Length Proposals  Listing
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PropMisclassificationDetailsListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();
                int stateCode = 0;
                int districtCode = 0;
                int blockCode = 0;
                int yearCode = 0;
                int batchCode = 0;
                int collaborationCode = 0;
                int agencyCode = 0;
                string staStatusCode = "%";
                string mrdStatusCode = "%";

                int year = 0;
                if (!string.IsNullOrEmpty(formCollection["StateCode"]))
                {
                    stateCode = Convert.ToInt32(formCollection["StateCode"]);
                }
                stateCode = stateCode == 0 ? PMGSYSession.Current.StateCode : stateCode;
                if (!string.IsNullOrEmpty(formCollection["DistrictCode"]))
                {
                    districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
                }
                districtCode = districtCode == 0 ? PMGSYSession.Current.DistrictCode : districtCode;

                if (!string.IsNullOrEmpty(formCollection["BlockCode"]))
                {
                    blockCode = Convert.ToInt32(formCollection["BlockCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["YearCode"]))
                {
                    yearCode = Convert.ToInt32(formCollection["YearCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["BatchCode"]))
                {
                    batchCode = Convert.ToInt32(formCollection["BatchCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["CollaborationCode"]))
                {
                    collaborationCode = Convert.ToInt32(formCollection["CollaborationCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["AgencyCode"]))
                {
                    agencyCode = Convert.ToInt32(formCollection["AgencyCode"]);
                }
                if (!string.IsNullOrEmpty(formCollection["StaStatusCode"]))
                {
                    staStatusCode = formCollection["StaStatusCode"];
                }
                if (!string.IsNullOrEmpty(formCollection["MrdStatusCode"]))
                {
                    mrdStatusCode = formCollection["MrdStatusCode"];
                } 
                string type = formCollection["Type"];
                string report = formCollection["Report"];
                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords;

                var jsonData = new
                {
                    rows = proposalReportsBAL.PropMisclassificationDetailsListingBAL(stateCode,districtCode,blockCode,yearCode,batchCode,collaborationCode,agencyCode,staStatusCode,mrdStatusCode, type, report, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        #endregion

        #endregion

        #region PCI Abstract Analysis Details

        public ActionResult PCIAbstractAnalysisDetails()
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            ProposalReportsModel ProposaleportsViewModel = new ProposalReportsModel();
            try
            {

                ProposaleportsViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                ProposaleportsViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                SelectListItem all = new SelectListItem
                {
                    Selected = true,
                    Text = "All",
                    Value = "%"
                };

                SelectListItem link = new SelectListItem
                {
                    Text = "Link",
                    Value = "L"
                };

                SelectListItem through = new SelectListItem
                {
                    Text = "Through",
                    Value = "T"
                };

                List<SelectListItem> route = new List<SelectListItem>();
                route.Add(all);
                route.Add(link);
                route.Add(through);
                ViewData["Route"] = route;
                return View(ProposaleportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }


        /// <summary>
        /// PCI Abstract StateWise Listing
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PCIAbstractAnalysisListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();
                int stateCode = Convert.ToInt32(formCollection["StateCode"]) == 0 ? PMGSYSession.Current.StateCode : Convert.ToInt32(formCollection["StateCode"]);
                int districtCode = Convert.ToInt32(formCollection["DistrictCode"]) == 0 ? PMGSYSession.Current.DistrictCode : Convert.ToInt32(formCollection["DistrictCode"]);
                int blockCode = Convert.ToInt32(formCollection["BlockCode"]);
                string flag = formCollection["Flag"];
                string routType = formCollection["RouteType"];

                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords;

                var jsonData = new
                {
                    rows = proposalReportsBAL.PCIAbstractAnalysisListingBAL(stateCode, districtCode, blockCode, flag, routType, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }


        /// <summary>
        /// PCI Abstract StateWise Chart Listing
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult PCIAbstractAnalysisChartListing(FormCollection formCollection)
        {

            try
            {
                proposalReportsBAL = new ProposalReportsBAL();
                int stateCode = Convert.ToInt32(formCollection["StateCode"]) == 0 ? PMGSYSession.Current.StateCode : Convert.ToInt32(formCollection["StateCode"]);
                int districtCode = Convert.ToInt32(formCollection["DistrictCode"]) == 0 ? PMGSYSession.Current.DistrictCode : Convert.ToInt32(formCollection["DistrictCode"]);
                int blockCode = Convert.ToInt32(formCollection["BlockCode"]);
                string flag = formCollection["Flag"];
                string routType = formCollection["RouteType"];

                List<USP_CN_PCI_ANALYSIS_Result> list = proposalReportsBAL.GetPCIAnalysisChartBAL(stateCode, districtCode, blockCode, flag, routType);
                List<charModel> lstChart = new List<charModel>();

                decimal totalLen = 0;
                //decimal totalLen1 = 0;
                //decimal totalLen2 = 0; 
                //decimal totalAmt3 = 0;
                //decimal totalAmt4 = 0;
                //decimal totalAmt5 = 0;
                decimal pciNoTotal = 0;
                totalLen = list.Sum(m => (Decimal)m.PCI_TOTAL);
                string status = string.Empty;


                //totalLen1 = list.Sum(x => x.LEN_ONE.HasValue ? x.LEN_ONE.Value : 0);
                //totalLen2 = list.Sum(x => x.LEN_TWO.HasValue ? x.LEN_ONE.Value : 0);
                //totalAmt3 = list.Sum(x => x.LEN_THREE.HasValue ? x.LEN_ONE.Value : 0);
                //totalAmt4 = list.Sum(x => x.LEN_FOUR.HasValue ? x.LEN_ONE.Value : 0);
                //totalAmt5 = list.Sum(x => x.LEN_FIVE.HasValue ? x.LEN_ONE.Value : 0);

                if (totalLen > 0)
                {
                    for (int i = 1; i <= 5; ++i)
                    {
                        switch (i)
                        {
                            case 1:
                                pciNoTotal = list.Sum(x => x.LEN_ONE.HasValue ? x.LEN_ONE.Value : 0);
                                status = "Very Poor";
                                break;
                            case 2:
                                pciNoTotal = list.Sum(x => x.LEN_TWO.HasValue ? x.LEN_TWO.Value : 0);
                                status = "Poor";
                                break;
                            case 3:
                                pciNoTotal = list.Sum(x => x.LEN_THREE.HasValue ? x.LEN_THREE.Value : 0);
                                status = "Fair";
                                break;
                            case 4:
                                pciNoTotal = list.Sum(x => x.LEN_FOUR.HasValue ? x.LEN_FOUR.Value : 0);
                                status = "Good";
                                break;
                            case 5:
                                pciNoTotal = list.Sum(x => x.LEN_FIVE.HasValue ? x.LEN_FIVE.Value : 0);
                                status = "Very Good";
                                break;
                            default:
                                break;
                        }

                        var lenPercent = (pciNoTotal / totalLen) * 100;

                        charModel chart = new charModel();
                        //chart.x = (lenPercent < 10 ? "Very Poor: " + lenPercent : (lenPercent < 15 ? "Poor :" + lenPercent : (lenPercent < 20 ? "Good :" + lenPercent : "Very Good :" + lenPercent)));
                        chart.x = status.ToString() + ": " + lenPercent.ToString();
                        chart.y = pciNoTotal.ToString();
                        chart.z = i.ToString();
                        lstChart.Add(chart);
                    }
                }
                return new JsonResult
                {
                    Data = lstChart
                };

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        #endregion

        #region Proposal Asset Value

        public ActionResult PropAssetValueDetails()
        {
            ProposalReportsModel ProposaleportsViewModel = new ProposalReportsModel();
            try
            {
                ProposaleportsViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                ProposaleportsViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                return View(ProposaleportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult PropAssetValueReportListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();
                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords;

                var jsonData = new
                {
                    rows = proposalReportsBAL.PropAssetValueListingBAL(page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        #endregion



        #region Execution Financial Progress Details

        public ActionResult ExecutionFinancialProgressDetails()
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            ProposalReportsModel ProposaleportsViewModel = new ProposalReportsModel();
            ProposalReportsDAL objDAL = new ProposalReportsDAL();
            try
            {
                int stateCode = PMGSYSession.Current.StateCode;

                List<SelectListItem> ddState = objCommonFunctions.PopulateStates(false);
                List<SelectListItem> ddYear = objCommonFunctions.PopulateFinancialYear(true, true).ToList();
                SelectListItem all = new SelectListItem
               {
                   Selected = true,
                   Text = "All",
                   Value = "0"
               };
                ddYear.RemoveAt(0);
                ddYear.Insert(0, all);

                //SelectListItem selectState = new SelectListItem
                //{
                //    Selected = true,
                //    Text = "Select State",
                //    Value = "0"
                //};
                // ddState.Insert(0, selectState);
                ddState.Find(x => x.Value == "1").Selected = true;

                if (stateCode > 0)  //if state login
                {
                    ddState.Find(x => x.Value == stateCode.ToString()).Selected = true;
                }



                List<SelectListItem> ddcollaboration = objDAL.GetFundingAgencyList();
                // ddcollaboration.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));

                List<SelectListItem> ddDistrict = new List<SelectListItem>();
                ddDistrict.Insert(0, (new SelectListItem { Text = "All District", Value = "0", Selected = true }));

                List<SelectListItem> ddBlock = new List<SelectListItem>();
                ddBlock.Insert(0, (new SelectListItem { Text = "All Block", Value = "0", Selected = true }));

                List<SelectListItem> ddBatch = objCommonFunctions.PopulateBatch();
                ddBatch.RemoveAt(0);
                ddBatch.Insert(0, (new SelectListItem { Text = "All Batches", Value = "0", Selected = true }));

                List<SelectListItem> ddPropType = new List<SelectListItem>();
                ddPropType.Insert(0, (new SelectListItem { Text = "All", Value = "%", Selected = true }));
                ddPropType.Insert(1, (new SelectListItem { Text = "Road", Value = "P" }));
                ddPropType.Insert(2, (new SelectListItem { Text = "Bridge", Value = "L" }));


                ViewData["COLLABORATION"] = ddcollaboration;
                ViewData["YEAR"] = ddYear;
                ViewData["BATCH"] = ddBatch;
                ViewData["STATE"] = ddState;
                ViewData["DISTRICT"] = ddDistrict;
                ViewData["BLOCK"] = ddBlock;
                ViewData["TYPE"] = ddPropType;


                ProposaleportsViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                ProposaleportsViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                return View(ProposaleportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }
        [HttpPost]
        public ActionResult ExecutionFinancialProgressReportListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();
                int stateCode = Convert.ToInt32(formCollection["StateCode"]) == 0 ? PMGSYSession.Current.StateCode : Convert.ToInt32(formCollection["StateCode"]);
                int districtCode = Convert.ToInt32(formCollection["DistrictCode"]) == 0 ? PMGSYSession.Current.DistrictCode : Convert.ToInt32(formCollection["DistrictCode"]);
                int blockCode = Convert.ToInt32(formCollection["BlockCode"]);
                int year = Convert.ToInt32(formCollection["Year"]);
                int batch = Convert.ToInt32(formCollection["Batch"]);
                int collaboration = Convert.ToInt32(formCollection["Collaboration"]);
                string type = formCollection["Type"];
                string progress = formCollection["Progress"];
                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords;

                var jsonData = new
                {
                    rows = proposalReportsBAL.ExecutionFinancialProgressListingBAL(stateCode, districtCode, blockCode, year, batch, collaboration, type, progress, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        #endregion

        #region Maintenance Financial Progress Details

        public ActionResult MaintenanceFinancialProgressDetails()
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            ProposalReportsModel ProposaleportsViewModel = new ProposalReportsModel();
            ProposalReportsDAL objDAL = new ProposalReportsDAL();
            try
            {
                int stateCode = PMGSYSession.Current.StateCode;

                List<SelectListItem> ddState = objCommonFunctions.PopulateStates(false);
                List<SelectListItem> ddYear = objCommonFunctions.PopulateFinancialYear(true, true).ToList();
                SelectListItem all = new SelectListItem
                {
                    Selected = true,
                    Text = "All",
                    Value = "0"
                };
                ddYear.RemoveAt(0);
                ddYear.Insert(0, all);

                //SelectListItem selectState = new SelectListItem
                //{
                //    Selected = true,
                //    Text = "Select State",
                //    Value = "0"
                //};
                // ddState.Insert(0, selectState);
                ddState.Find(x => x.Value == "1").Selected = true;

                if (stateCode > 0)  //if state login
                {
                    ddState.Find(x => x.Value == stateCode.ToString()).Selected = true;
                }



                List<SelectListItem> ddcollaboration = objDAL.GetFundingAgencyList();
                // ddcollaboration.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));

                List<SelectListItem> ddDistrict = new List<SelectListItem>();
                ddDistrict.Insert(0, (new SelectListItem { Text = "All District", Value = "0", Selected = true }));

                List<SelectListItem> ddBlock = new List<SelectListItem>();
                ddBlock.Insert(0, (new SelectListItem { Text = "All Block", Value = "0", Selected = true }));

                List<SelectListItem> ddBatch = objCommonFunctions.PopulateBatch();
                ddBatch.RemoveAt(0);
                ddBatch.Insert(0, (new SelectListItem { Text = "All Batches", Value = "0", Selected = true }));

                List<SelectListItem> ddPropType = new List<SelectListItem>();
                ddPropType.Insert(0, (new SelectListItem { Text = "All", Value = "%", Selected = true }));
                ddPropType.Insert(1, (new SelectListItem { Text = "Road", Value = "P" }));
                ddPropType.Insert(2, (new SelectListItem { Text = "Bridge", Value = "L" }));


                ViewData["COLLABORATION"] = ddcollaboration;
                ViewData["YEAR"] = ddYear;
                ViewData["BATCH"] = ddBatch;
                ViewData["STATE"] = ddState;
                ViewData["DISTRICT"] = ddDistrict;
                ViewData["BLOCK"] = ddBlock;
                ViewData["TYPE"] = ddPropType;
                ProposaleportsViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                ProposaleportsViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                return View(ProposaleportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult MaintenanceFinancialProgressReportListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();

                int stateCode = Convert.ToInt32(formCollection["StateCode"]) == 0 ? PMGSYSession.Current.StateCode : Convert.ToInt32(formCollection["StateCode"]);
                int districtCode = Convert.ToInt32(formCollection["DistrictCode"]) == 0 ? PMGSYSession.Current.DistrictCode : Convert.ToInt32(formCollection["DistrictCode"]);
                int blockCode = Convert.ToInt32(formCollection["BlockCode"]);
                int year = Convert.ToInt32(formCollection["Year"]);
                int batch = Convert.ToInt32(formCollection["Batch"]);
                int collaboration = Convert.ToInt32(formCollection["Collaboration"]);
                string type = formCollection["Type"];
                string progress = formCollection["Progress"];
                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords;

                var jsonData = new
                {
                    rows = proposalReportsBAL.MaintenanceFinancialProgressListingBAL(stateCode, districtCode, blockCode, year, batch, collaboration, type, progress, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        #endregion

        #region Maintenance Agreement Details

        public ActionResult MaintenanceAgreementDetails()
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            ProposalReportsModel ProposaleportsViewModel = new ProposalReportsModel();
            ProposalReportsDAL objDAL = new ProposalReportsDAL();
            try
            {
                int stateCode = PMGSYSession.Current.StateCode;

                List<SelectListItem> ddState = objCommonFunctions.PopulateStates(false);
                List<SelectListItem> ddYear = objCommonFunctions.PopulateFinancialYear(true, true).ToList();
                SelectListItem all = new SelectListItem
                {
                    Selected = true,
                    Text = "All",
                    Value = "0"
                };
                ddYear.RemoveAt(0);
                ddYear.Insert(0, all);

                //SelectListItem selectState = new SelectListItem
                //{
                //    Selected = true,
                //    Text = "Select State",
                //    Value = "0"
                //};
                // ddState.Insert(0, selectState);
                ddState.Find(x => x.Value == "1").Selected = true;

                if (stateCode > 0)  //if state login
                {
                    ddState.Find(x => x.Value == stateCode.ToString()).Selected = true;
                }



                List<SelectListItem> ddcollaboration = objDAL.GetFundingAgencyList();
                //ddcollaboration.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));

                List<SelectListItem> ddDistrict = new List<SelectListItem>();
                ddDistrict.Insert(0, (new SelectListItem { Text = "All District", Value = "0", Selected = true }));

                List<SelectListItem> ddBlock = new List<SelectListItem>();
                ddBlock.Insert(0, (new SelectListItem { Text = "All Block", Value = "0", Selected = true }));

                List<SelectListItem> ddBatch = objCommonFunctions.PopulateBatch();
                ddBatch.RemoveAt(0);
                ddBatch.Insert(0, (new SelectListItem { Text = "All Batches", Value = "0", Selected = true }));

                List<SelectListItem> ddProposalStatus = new List<SelectListItem>();
                SelectListItem item = new SelectListItem();

                item = new SelectListItem();
                item.Text = "Sanction Pending";
                item.Value = "N";
                ddProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Dropped Proposal";
                item.Value = "D";
                ddProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Sanctioned Proposals";
                item.Value = "Y";
                ddProposalStatus.Add(item);

                item = new SelectListItem();
                // item.Text = "Un-Sanctioned Proposals";
                item.Text = "Proposal Not Sanctioned";
                item.Value = "U";
                ddProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Reconsider Proposals";
                item.Value = "R";
                ddProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "All";
                item.Value = "%";
                item.Selected = true;
                ddProposalStatus.Add(item);


                ViewData["COLLABORATION"] = ddcollaboration;
                ViewData["YEAR"] = ddYear;
                ViewData["BATCH"] = ddBatch;
                ViewData["STATE"] = ddState;
                ViewData["DISTRICT"] = ddDistrict;
                ViewData["BLOCK"] = ddBlock;
                ViewData["STATUS"] = ddProposalStatus;
                ProposaleportsViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                ProposaleportsViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                return View(ProposaleportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult MaintenanceAgreementReportListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();

                int stateCode = Convert.ToInt32(formCollection["StateCode"]) == 0 ? PMGSYSession.Current.StateCode : Convert.ToInt32(formCollection["StateCode"]);
                int districtCode = Convert.ToInt32(formCollection["DistrictCode"]) == 0 ? PMGSYSession.Current.DistrictCode : Convert.ToInt32(formCollection["DistrictCode"]);
                int blockCode = Convert.ToInt32(formCollection["BlockCode"]);
                int year = Convert.ToInt32(formCollection["Year"]);
                int batch = Convert.ToInt32(formCollection["Batch"]);
                int collaboration = Convert.ToInt32(formCollection["Collaboration"]);
                string status = formCollection["Status"];

                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords;

                var jsonData = new
                {
                    rows = proposalReportsBAL.MaintenanceAgreementListingBAL(stateCode, districtCode, blockCode, year, batch, collaboration, status, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        #endregion

        #region Maintenance Inspection Details

        public ActionResult MaintenanceInspectionDetails()
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            ProposalReportsModel ProposaleportsViewModel = new ProposalReportsModel();
            ProposalReportsDAL objDAL = new ProposalReportsDAL();
            try
            {
                int stateCode = PMGSYSession.Current.StateCode;

                List<SelectListItem> ddState = objCommonFunctions.PopulateStates(false);
                List<SelectListItem> ddYear = objCommonFunctions.PopulateFinancialYear(true, true).ToList();
                SelectListItem all = new SelectListItem
                {
                    Selected = true,
                    Text = "All",
                    Value = "0"
                };
                ddYear.RemoveAt(0);
                ddYear.Insert(0, all);

                //SelectListItem selectState = new SelectListItem
                //{
                //    Selected = true,
                //    Text = "Select State",
                //    Value = "0"
                //};
                // ddState.Insert(0, selectState);
                ddState.Find(x => x.Value == "1").Selected = true;

                if (stateCode > 0)  //if state login
                {
                    ddState.Find(x => x.Value == stateCode.ToString()).Selected = true;
                }



                List<SelectListItem> ddcollaboration = objDAL.GetFundingAgencyList();
                // ddcollaboration.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));

                List<SelectListItem> ddDistrict = new List<SelectListItem>();
                ddDistrict.Insert(0, (new SelectListItem { Text = "All District", Value = "0", Selected = true }));

                List<SelectListItem> ddBlock = new List<SelectListItem>();
                ddBlock.Insert(0, (new SelectListItem { Text = "All Block", Value = "0", Selected = true }));

                List<SelectListItem> ddBatch = objCommonFunctions.PopulateBatch();
                ddBatch.RemoveAt(0);
                ddBatch.Insert(0, (new SelectListItem { Text = "All Batches", Value = "0", Selected = true }));

                List<SelectListItem> ddPropType = new List<SelectListItem>();
                ddPropType.Insert(0, (new SelectListItem { Text = "All", Value = "%", Selected = true }));
                ddPropType.Insert(1, (new SelectListItem { Text = "Road", Value = "P" }));
                ddPropType.Insert(2, (new SelectListItem { Text = "Bridge", Value = "L" }));


                ViewData["COLLABORATION"] = ddcollaboration;
                ViewData["YEAR"] = ddYear;
                ViewData["BATCH"] = ddBatch;
                ViewData["STATE"] = ddState;
                ViewData["DISTRICT"] = ddDistrict;
                ViewData["BLOCK"] = ddBlock;
                ViewData["TYPE"] = ddPropType;
                ProposaleportsViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                ProposaleportsViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                return View(ProposaleportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult MaintenanceInspectionReportListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();
                int stateCode = Convert.ToInt32(formCollection["StateCode"]) == 0 ? PMGSYSession.Current.StateCode : Convert.ToInt32(formCollection["StateCode"]);
                int districtCode = Convert.ToInt32(formCollection["DistrictCode"]) == 0 ? PMGSYSession.Current.DistrictCode : Convert.ToInt32(formCollection["DistrictCode"]);
                int blockCode = Convert.ToInt32(formCollection["BlockCode"]);
                int year = Convert.ToInt32(formCollection["Year"]);
                int batch = Convert.ToInt32(formCollection["Batch"]);
                int collaboration = Convert.ToInt32(formCollection["Collaboration"]);
                string type = formCollection["Type"];
                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords;

                var jsonData = new
                {
                    rows = proposalReportsBAL.MaintenanceInspectionListingBAL(stateCode, districtCode, blockCode, year, batch, collaboration, type, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        #endregion


        #region Fund Sanction Release  Details

        public ActionResult FundSanctionReleaseDetails()
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            ProposalReportsModel ProposaleportsViewModel = new ProposalReportsModel();
            ProposalReportsDAL objDAL = new ProposalReportsDAL();
            try
            {
                int stateCode = PMGSYSession.Current.StateCode;

                List<SelectListItem> ddState = objCommonFunctions.PopulateStates(false);
                List<SelectListItem> ddYear = objCommonFunctions.PopulateFinancialYear(true, true).ToList();
                SelectListItem all = new SelectListItem
                {
                    Selected = true,
                    Text = "All",
                    Value = "0"
                };
                ddYear.RemoveAt(0);
                ddYear.Insert(0, all);

                //SelectListItem selectState = new SelectListItem
                //{
                //    Selected = true,
                //    Text = "Select State",
                //    Value = "0"
                //};
                // ddState.Insert(0, selectState);
                ddState.Find(x => x.Value == "1").Selected = true;

                if (stateCode > 0)  //if state login
                {
                    ddState.Find(x => x.Value == stateCode.ToString()).Selected = true;
                }



                List<SelectListItem> ddcollaboration = objDAL.GetFundingAgencyList();
                // ddcollaboration.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));


                List<SelectListItem> ddAgency = objDAL.GetAgencyList();
                ddAgency.RemoveAt(0);
                ddAgency.Insert(0, (new SelectListItem { Text = "All Agency", Value = "0", Selected = true }));

                List<SelectListItem> ddFUNDTYPE = new List<SelectListItem>();
                ddFUNDTYPE.Insert(0, (new SelectListItem { Text = "All", Value = "%", Selected = true }));
                ddFUNDTYPE.Insert(1, (new SelectListItem { Text = "Program Fund", Value = "P" }));
                ddFUNDTYPE.Insert(2, (new SelectListItem { Text = "Administrative Fund", Value = "A" }));
                ddFUNDTYPE.Insert(3, (new SelectListItem { Text = "Maintenance Fund", Value = "M" }));

                List<SelectListItem> ddReleaseTYPE = new List<SelectListItem>();
                ddReleaseTYPE.Insert(0, (new SelectListItem { Text = "All", Value = "%", Selected = true }));
                ddReleaseTYPE.Insert(1, (new SelectListItem { Text = "Central Government", Value = "C" }));
                ddReleaseTYPE.Insert(2, (new SelectListItem { Text = "State Government", Value = "S" }));


                ViewData["COLLABORATION"] = ddcollaboration;
                ViewData["YEAR"] = ddYear;
                ViewData["AGENCY"] = ddAgency;
                ViewData["STATE"] = ddState;
                ViewData["FUNDTYPE"] = ddFUNDTYPE;
                ViewData["RELEASETYPE"] = ddReleaseTYPE;

                ProposaleportsViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                ProposaleportsViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                return View(ProposaleportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult FundSanctionReleaseReportListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();

                int stateCode = Convert.ToInt32(formCollection["StateCode"]) == 0 ? PMGSYSession.Current.StateCode : Convert.ToInt32(formCollection["StateCode"]);
                int year = Convert.ToInt32(formCollection["Year"]);
                int collaboration = Convert.ToInt32(formCollection["Collaboration"]);
                int agency = Convert.ToInt32(formCollection["Agency"]);
                string fund = formCollection["FundType"];
                string releaseType = formCollection["ReleaseType"];
                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords;

                var jsonData = new
                {
                    rows = proposalReportsBAL.FundSanctionReleaseListingBAL(stateCode, year, collaboration, agency, fund, releaseType, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        #endregion

        #region Tendering Agreeement Details

        public ActionResult TenderingAgreementDetails()
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            ProposalReportsModel ProposaleportsViewModel = new ProposalReportsModel();
            ProposalReportsDAL objDAL = new ProposalReportsDAL();
            try
            {
                int stateCode = PMGSYSession.Current.StateCode;

                List<SelectListItem> ddState = objCommonFunctions.PopulateStates(false);
                List<SelectListItem> ddYear = objCommonFunctions.PopulateFinancialYear(true, true).ToList();
                SelectListItem all = new SelectListItem
                {
                    Selected = true,
                    Text = "All",
                    Value = "0"
                };
                ddYear.RemoveAt(0);
                ddYear.Insert(0, all);

                //SelectListItem selectState = new SelectListItem
                //{
                //    Selected = true,
                //    Text = "Select State",
                //    Value = "0"
                //};
                // ddState.Insert(0, selectState);
                ddState.Find(x => x.Value == "1").Selected = true;

                if (stateCode > 0)  //if state login
                {
                    ddState.Find(x => x.Value == stateCode.ToString()).Selected = true;
                }



                List<SelectListItem> ddcollaboration = objDAL.GetFundingAgencyList();
                // ddcollaboration.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));

                List<SelectListItem> ddDistrict = new List<SelectListItem>();
                ddDistrict.Insert(0, (new SelectListItem { Text = "All District", Value = "0", Selected = true }));

                List<SelectListItem> ddBlock = new List<SelectListItem>();
                ddBlock.Insert(0, (new SelectListItem { Text = "All Block", Value = "0", Selected = true }));

                List<SelectListItem> ddBatch = objCommonFunctions.PopulateBatch();
                ddBatch.RemoveAt(0);
                ddBatch.Insert(0, (new SelectListItem { Text = "All Batches", Value = "0", Selected = true }));

                List<SelectListItem> ddProposalStatus = new List<SelectListItem>();
                SelectListItem item = new SelectListItem();

                item = new SelectListItem();
                item.Text = "Sanction Pending";
                item.Value = "N";
                ddProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Dropped Proposal";
                item.Value = "D";
                ddProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Sanctioned Proposals";
                item.Value = "Y";
                ddProposalStatus.Add(item);

                item = new SelectListItem();
                // item.Text = "Un-Sanctioned Proposals";
                item.Text = "Proposal Not Sanctioned";
                item.Value = "U";
                ddProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Reconsider Proposals";
                item.Value = "R";
                ddProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "All";
                item.Value = "%";
                item.Selected = true;
                ddProposalStatus.Add(item);


                List<SelectListItem> ddAgreement = new List<SelectListItem>();
                ddAgreement.Insert(0, (new SelectListItem { Text = "All", Value = "%", Selected = true }));
                ddAgreement.Insert(1, (new SelectListItem { Text = "Contracto", Value = "C" }));
                ddAgreement.Insert(2, (new SelectListItem { Text = "DPR", Value = "D" }));
                ddAgreement.Insert(3, (new SelectListItem { Text = "Others", Value = "O" }));
                ddAgreement.Insert(4, (new SelectListItem { Text = "Supplier", Value = "S" }));

                List<SelectListItem> ddPackage = new List<SelectListItem>();
                ddPackage.Insert(0, (new SelectListItem { Text = "All", Value = "%", Selected = true }));

                List<SelectListItem> ddContractor = new List<SelectListItem>();
                ddContractor.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));

                ViewData["COLLABORATION"] = ddcollaboration;
                ViewData["YEAR"] = ddYear;
                ViewData["BATCH"] = ddBatch;
                ViewData["STATE"] = ddState;
                ViewData["DISTRICT"] = ddDistrict;
                ViewData["BLOCK"] = ddBlock;
                ViewData["STATUS"] = ddProposalStatus;
                ViewData["AGREEMENT"] = ddAgreement;
                ViewData["PACKAGE"] = ddPackage;
                ViewData["CONTRACTOR"] = ddContractor;
                ProposaleportsViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                ProposaleportsViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                return View(ProposaleportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult TenderingAgreementReportListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();

                int stateCode = Convert.ToInt32(formCollection["StateCode"]) == 0 ? PMGSYSession.Current.StateCode : Convert.ToInt32(formCollection["StateCode"]);
                int districtCode = Convert.ToInt32(formCollection["DistrictCode"]) == 0 ? PMGSYSession.Current.DistrictCode : Convert.ToInt32(formCollection["DistrictCode"]);
                int blockCode = Convert.ToInt32(formCollection["BlockCode"]);
                int year = Convert.ToInt32(formCollection["Year"]);
                int batch = Convert.ToInt32(formCollection["Batch"]);
                int collaboration = Convert.ToInt32(formCollection["Collaboration"]);
                int conId = Convert.ToInt32(formCollection["ConId"]);
                string package = formCollection["Package"];
                string status = formCollection["Status"];
                string agreement = formCollection["Agreement"];
                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords;

                var jsonData = new
                {
                    rows = proposalReportsBAL.TeanderAgreementListingBAL(stateCode, districtCode, blockCode, year, batch, collaboration, conId, package, status, agreement, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        #endregion


        #region Fund Allocation  Details

        public ActionResult FundAllocationDetails()
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            ProposalReportsModel ProposaleportsViewModel = new ProposalReportsModel();
            try
            {
                int stateCode = PMGSYSession.Current.StateCode;

                List<SelectListItem> ddState = objCommonFunctions.PopulateStates(false);
                SelectListItem allState = new SelectListItem
                {
                    Selected = true,
                    Text = "All",
                    Value = "0"
                };
                ddState.Insert(0, allState);
                if (stateCode > 0)  //if state login
                {
                    ddState.Find(x => x.Value == stateCode.ToString()).Selected = true;
                }

                SelectListItem all = new SelectListItem
                {
                    Selected = true,
                    Text = "All",
                    Value = "%"
                };

                SelectListItem landAcquisition = new SelectListItem
                {
                    Text = "Land Acquisition",
                    Value = "A"
                };
                SelectListItem legalCase = new SelectListItem
                {
                    Text = "Legal Case",
                    Value = "L"
                };
                SelectListItem forestClearence = new SelectListItem
                {
                    Text = "Forest Clearance",
                    Value = "F"
                };

                List<SelectListItem> ddreason = new List<SelectListItem>();
                ddreason.Add(all);
                ddreason.Add(landAcquisition);
                ddreason.Add(legalCase);
                ddreason.Add(forestClearence);
                ViewData["REASON"] = ddreason;
                ViewData["STATE"] = ddState;
                ProposaleportsViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                ProposaleportsViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                return View(ProposaleportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult FundAllocationReportListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                proposalReportsBAL = new ProposalReportsBAL();

                int stateCode = Convert.ToInt32(formCollection["StateCode"]) == 0 ? PMGSYSession.Current.StateCode : Convert.ToInt32(formCollection["StateCode"]);
                string reason = formCollection["Reason"];
                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords;

                var jsonData = new
                {
                    rows = proposalReportsBAL.PendingWorksListingBAL(stateCode, reason, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        #endregion

        #region Common Function
        public ActionResult DistrictDetails(FormCollection frmCollection)
        {
            try
            {
                objCommonFunctions = new CommonFunctions();
                List<SelectListItem> list = objCommonFunctions.PopulateDistrict(Convert.ToInt32(frmCollection["StateCode"]), false);
                // list.Find(x => x.Value == "-1").Value = "0";
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        public ActionResult BlockDetails(FormCollection frmCollection)
        {
            try
            {
                objCommonFunctions = new CommonFunctions();
                List<SelectListItem> list = objCommonFunctions.PopulateBlocks(Convert.ToInt32(frmCollection["DistrictCode"]), true);
                // list.Find(x => x.Value == "-1").Value = "0";
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }
        public ActionResult AllDistrictDetails(FormCollection frmCollection)
        {
            try
            {
                objCommonFunctions = new CommonFunctions();
                List<SelectListItem> list = objCommonFunctions.PopulateDistrict(Convert.ToInt32(frmCollection["StateCode"]), true);
                // list.Find(x => x.Value == "-1").Value = "0";
                SelectListItem all = new SelectListItem
                {
                    Selected = true,
                    Text = "All District",
                    Value = "0"
                };
                list.RemoveAt(0);
                list.Insert(0, all);
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        public ActionResult AllBlockDetails(FormCollection frmCollection)
        {
            try
            {
                objCommonFunctions = new CommonFunctions();
                List<SelectListItem> list = objCommonFunctions.PopulateBlocks(Convert.ToInt32(frmCollection["DistrictCode"]), true);
                // list.Find(x => x.Value == "-1").Value = "0";
                SelectListItem all = new SelectListItem
                {
                    Selected = true,
                    Text = "All Block",
                    Value = "0"
                };
                list.RemoveAt(0);
                list.Insert(0, all);
                return Json(list, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }
        public ActionResult GetContractoreNameByStateDetails(FormCollection frmCollection)
        {
            try
            {
                ProposalReportsDAL objDAL = new ProposalReportsDAL();
                List<SelectListItem> list = objDAL.GetContractoreNameByState(Convert.ToInt32(frmCollection["StateCode"]));
                // list.Find(x => x.Value == "-1").Value = "0";
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        public ActionResult GetPackageByStateDistrictBlockDetails(FormCollection frmCollection)
        {
            try
            {
                ProposalReportsDAL objDAL = new ProposalReportsDAL();
                List<SelectListItem> list = objDAL.GetPackageByStateDistrictBlock(Convert.ToInt32(frmCollection["StateCode"]), Convert.ToInt32(frmCollection["DistrictCode"]), Convert.ToInt32(frmCollection["BlockCode"]));
                // list.Find(x => x.Value == "-1").Value = "0";
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }
        #endregion

        #region MRDProposal REPORT
        public ActionResult MRDProposalLayout()
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            ProposalController p = new ProposalController();

            PMGSY.Models.ProposalReports.MRDProposalModel mrd = new PMGSY.Models.ProposalReports.MRDProposalModel();
            CommonFunctions common = new CommonFunctions();

            try
            {
                mrd.YearList = common.PopulateFinancialYear(true, true);
                //mrd.YearList.RemoveAt(0);
                //mrd.YearList.Insert(0, new SelectListItem { Value = "0", Text = "All Years" });
                mrd.StateList = common.PopulateStates(true);
                if (PMGSYSession.Current.StateCode > 0)
                {
                    mrd.StateCode = PMGSYSession.Current.StateCode;
                    mrd.StateName = PMGSYSession.Current.StateName;

                    mrd.DistrictList = common.PopulateDistrict(mrd.StateCode);
                    mrd.DistrictList.RemoveAt(0);
                    mrd.DistrictList.Add(new SelectListItem { Value = "0", Text = "All Districts" });

                    if (PMGSYSession.Current.DistrictCode > 0)
                    {
                        mrd.DistrictCode = PMGSYSession.Current.DistrictCode;
                        mrd.DistrictName = PMGSYSession.Current.DistrictName;

                        mrd.PackageList = PopulatePackages(Convert.ToString(mrd.StateCode).Trim() + "$" + Convert.ToString(mrd.DistrictCode).Trim());

                        //objCommonFunctions = new CommonFunctions();
                        //List<SelectListItem> lstBlock = new List<SelectListItem>();
                        mrd.BlockList = common.PopulateBlocks(mrd.DistrictCode, true);
                        mrd.BlockList.RemoveAt(0);
                        mrd.BlockList.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
                    }
                    else
                    {
                        string param = Convert.ToString(mrd.StateCode) + "$" + Convert.ToString(mrd.DistrictCode) + "$" + Convert.ToString(System.DateTime.Now.Year);
                        mrd.PackageList = PopulatePackages(param);

                        mrd.BlockList = new List<SelectListItem>();
                        mrd.BlockList.Add(new SelectListItem { Value = "0", Text = "All Blocks" });
                    }

                    //mrd.AgencyList = common.PopulateAgenciesByStateAndDepartmentwise(mrd.StateCode, PMGSYSession.Current.AdminNdCode, false);
                    //mrd.AgencyList.RemoveAt(0);
                    //string agencycd = mrd.AgencyList.Where();

                    mrd.Agency = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Select(m => m.MAST_AGENCY_CODE).FirstOrDefault();//PMGSYSession.Current.AdminNdCode;
                    mrd.AgencyName = PMGSYSession.Current.DepartmentName.Trim();
                    //mrd.AgencyList = common.PopulateAgencies(mrd.StateCode, true);
                }
                else
                {
                    mrd.DistrictList = common.PopulateDistrict(0);
                    mrd.DistrictList.RemoveAt(0);
                    mrd.DistrictList.Insert(0, new SelectListItem { Value = "-1", Text = "Select Districts" });

                    mrd.PackageList = common.GetPackages(mrd.Year, 0, true);//commonFunction.PopulatePackage(transactionParams);

                    mrd.AgencyList = new List<SelectListItem>();
                    mrd.AgencyList.Add(new SelectListItem { Value = "0", Text = "All Agencies" });

                    mrd.BlockList = new List<SelectListItem>();
                    mrd.BlockList.Add(new SelectListItem { Value = "0", Text = "All Blocks" });
                }





                mrd.CollabList = common.PopulateStreams("", false);
                mrd.BatchList = common.PopulateBatch();
                mrd.BatchList.RemoveAt(0);
                List<SelectListItem> sl = new List<SelectListItem>();
                mrd.BatchList.Add(new SelectListItem() { Text = "All Batches", Value = "0" });



                //mrd.StatusList = p.PopulateProposalStatus(25);
                List<SelectListItem> lstPropTypes = new List<SelectListItem>();

                lstPropTypes.Insert(0, new SelectListItem { Value = "A", Text = "All" });
                lstPropTypes.Insert(1, new SelectListItem { Value = "N", Text = "Pending Proposals" });
                lstPropTypes.Insert(2, new SelectListItem { Value = "Y", Text = "Sanctioned Proposals" });
                lstPropTypes.Insert(2, new SelectListItem { Value = "U", Text = "Un-Sanctioned Proposals" });
                lstPropTypes.Insert(2, new SelectListItem { Value = "R", Text = "Recommended Proposals" });
                lstPropTypes.Insert(2, new SelectListItem { Value = "D", Text = "Dropped Proposals" });

                mrd.StatusList = lstPropTypes;

                List<SelectListItem> lstTypes = new List<SelectListItem>();
                lstTypes.Insert(0, new SelectListItem { Value = "0", Text = "All" });
                lstTypes.Insert(1, new SelectListItem { Value = "N", Text = "New Connectivity" });
                lstTypes.Insert(2, new SelectListItem { Value = "U", Text = "Upgradation" });

                mrd.CategoryList = lstTypes;

                mrd.CollabList = common.PopulateFundingAgency(true);

                List<SelectListItem> lstSTAStatus = new List<SelectListItem>();
                lstSTAStatus.Insert(0, new SelectListItem { Value = "0", Text = "All" });
                lstSTAStatus.Insert(1, new SelectListItem { Value = "Y", Text = "Scrutinized" });
                lstSTAStatus.Insert(2, new SelectListItem { Value = "N", Text = "UnScrutinized" });

                mrd.STAStatusList = lstSTAStatus;

                List<SelectListItem> lstProposal = new List<SelectListItem>();
                lstProposal.Insert(0, new SelectListItem { Value = "0", Text = "Both" });
                lstProposal.Insert(1, new SelectListItem { Value = "P", Text = "Road" });
                lstProposal.Insert(2, new SelectListItem { Value = "L", Text = "Bridge" });

                mrd.ProposalList = lstProposal;


                mrd.Year = System.DateTime.Now.Year;

                mrd.PMGSY = PMGSY.Extensions.PMGSYSession.Current.PMGSYScheme;


                #region Filtering Logic
                //CN1.StateName = fetchCookie.StateCode == 0 ? "0" : fetchCookie.StateName.Trim();
                //CN1.DistName = fetchCookie.DistrictCode == 0 ? "0" : fetchCookie.DistrictName.Trim();
                //CN1.BlockName = fetchCookie.BlockCode == 0 ? "0" : fetchCookie.BlockName.Trim();
                //CN1.Mast_State_Code = fetchCookie.StateCode;
                //CN1.Mast_District_Code = fetchCookie.DistrictCode;
                //CN1.Mast_Block_Code = fetchCookie.BlockCode;
                //CN1.LevelCode = fetchCookie.BlockCode > 0 ? 3 : fetchCookie.DistrictCode > 0 ? 2 : 1;
                //CN1.StateList = commonFunctions.PopulateStates(true);
                //CN1.StateCode = fetchCookie.StateCode == 0 ? 0 : fetchCookie.StateCode;
                //CN1.StateList.Find(x => x.Value == CN1.StateCode.ToString()).Selected = true;

                //CN1.DistrictList = new List<SelectListItem>();
                //if (CN1.StateCode == 0)
                //{
                //    CN1.DistrictList.Insert(0, (new SelectListItem { Text = "All Districts", Value = "0", Selected = true }));
                //}
                //else
                //{
                //    CN1.DistrictList = commonFunctions.PopulateDistrict(CN1.StateCode, true);
                //    CN1.DistrictCode = fetchCookie.DistrictCode == 0 ? 0 : fetchCookie.DistrictCode;
                //    CN1.DistrictList.Find(x => x.Value == CN1.DistrictCode.ToString()).Selected = true;

                //}
                //CN1.BlockList = new List<SelectListItem>();
                //if (CN1.DistrictCode == 0)
                //{
                //    CN1.BlockList.Insert(0, (new SelectListItem { Text = "All Blocks", Value = "0", Selected = true }));
                //}
                //else
                //{
                //    CN1.BlockList = commonFunctions.PopulateBlocks(CN1.DistrictCode, true);
                //    CN1.BlockCode = fetchCookie.BlockCode == 0 ? 0 : fetchCookie.BlockCode;
                //    CN1.BlockList.Find(x => x.Value == CN1.BlockCode.ToString()).Selected = true;
                //}
                #endregion

                //if (fetchCookie.Language.Contains('-'))
                //{
                //    CN1.localizedValue = fetchCookie.Language.Substring(0, fetchCookie.Language.IndexOf('-'));
                //}
                //else
                //{
                //    CN1.localizedValue = fetchCookie.Language;
                //}

                return View(mrd);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
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


        public ActionResult MRDProposalReport(MRDProposalModel mrd)
        {
            //CN4Model CN4Model = new CN4Model();
            //FetchCookieData fetchCookie = new FetchCookieData();
            try
            {
                if (ModelState.IsValid)
                {
                    mrd.PackageCode = mrd.PackageCode == "0" ? "%" : mrd.PackageCode;
                    mrd.PTAStatus = "%";
                    mrd.STAStatusCode = mrd.STAStatusCode == "0" ? "%" : mrd.STAStatusCode;
                    mrd.MRDStatus = "%";
                    mrd.CollabCode = mrd.CollabCode == -1 ? 0 : mrd.CollabCode;
                    mrd.CategoryCode = mrd.CategoryCode == "0" ? "%" : mrd.CategoryCode;

                    mrd.Level = 1;
                    mrd.BlockCode = 0;
                    mrd.Agency = 0;
                    mrd.PTAStatus = "%";
                    mrd.MRDStatus = "%";
                    mrd.PMGSY = PMGSY.Extensions.PMGSYSession.Current.PMGSYScheme;

                    return View(mrd);
                }
                else
                {
                    #region
                    string message = "";
                    //    bool flag = false;
                    message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;

                    //PMGSY.Models.AverageConcurrencyCalculation.AverageConcurrencyCalculationModel acc = new Models.AverageConcurrencyCalculation.AverageConcurrencyCalculationModel();
                    //CommonFunctions common = new CommonFunctions();

                    ProposalController p = new ProposalController();
                    //PMGSY.Models.ProposalReports.MRDProposalModel mrd = new PMGSY.Models.ProposalReports.MRDProposalModel();
                    CommonFunctions common = new CommonFunctions();

                    mrd.YearList = common.PopulateFinancialYear(true, true);
                    //mrd.YearList.RemoveAt(0);
                    //mrd.YearList.Insert(0, new SelectListItem { Value = "0", Text = "All Years" });
                    mrd.StateList = common.PopulateStates(true);
                    mrd.DistrictList = common.PopulateDistrict(0);
                    mrd.CollabList = common.PopulateStreams("", false);
                    mrd.BatchList = common.PopulateBatch();
                    mrd.BatchList.RemoveAt(0);
                    mrd.BatchList.Add(new SelectListItem() { Text = "All Batches", Value = "0" });
                    mrd.PackageList = common.GetPackages(mrd.Year, 0, true);//commonFunction.PopulatePackage(transactionParams);

                    mrd.StatusList = p.PopulateProposalStatus(25);

                    List<SelectListItem> lstTypes = new List<SelectListItem>();
                    lstTypes.Insert(0, new SelectListItem { Value = "0", Text = "All" });
                    lstTypes.Insert(1, new SelectListItem { Value = "N", Text = "New Connectivity" });
                    lstTypes.Insert(2, new SelectListItem { Value = "U", Text = "Upgradation" });

                    mrd.CategoryList = lstTypes;

                    mrd.CollabList = common.PopulateFundingAgency(true);

                    List<SelectListItem> lstSTAStatus = new List<SelectListItem>();
                    lstSTAStatus.Insert(0, new SelectListItem { Value = "0", Text = "All" });
                    lstSTAStatus.Insert(1, new SelectListItem { Value = "Y", Text = "Sanctioned" });
                    lstSTAStatus.Insert(2, new SelectListItem { Value = "N", Text = "Unsanctioned" });

                    mrd.STAStatusList = lstSTAStatus;

                    List<SelectListItem> lstProposal = new List<SelectListItem>();
                    lstProposal.Insert(0, new SelectListItem { Value = "0", Text = "All" });
                    lstProposal.Insert(1, new SelectListItem { Value = "P", Text = "Road" });
                    lstProposal.Insert(2, new SelectListItem { Value = "L", Text = "Bridge" });

                    mrd.ProposalList = lstProposal;


                    mrd.Year = System.DateTime.Now.Year;


                    //CN1.RouteList = commonFunctions.PopulateRoute();
                    return PartialView("MRDProposalLayout", mrd);
                    #endregion
                }
            }
            catch
            {
                return View(mrd);
            }
            //return View();
        }

        public ActionResult MRDProposalDataReport(MRDProposalModel mrd)
        {
            PMGSY.BAL.ProposalReports.IProposalReportsBAL objReportBAL = new PMGSY.BAL.ProposalReports.ProposalReportsBAL();
            // PMGSY.Models.ProposalReports.MRDProposalReportFilter objParam = new MRDProposalReportFilter();
            try
            {
                if (ModelState.IsValid)
                {
                    mrd.ProposalCode = "L";
                    mrd = objReportBAL.GetMRDProposalBAL(mrd);


                }
                return View(mrd);
            }
            catch
            {
                return View(mrd);
            }
        }

        public ActionResult MRDProposalHabCoverage(MRDProposalModel mrd/*String parameter, String hash, String key*/)
        {
            PMGSY.BAL.ProposalReports.IProposalReportsBAL objReportBAL = new PMGSY.BAL.ProposalReports.ProposalReportsBAL();
            // PMGSY.Models.ProposalReports.MRDProposalReportFilter objParam = new MRDProposalReportFilter();
            try
            {
                if (ModelState.IsValid)
                {
                    mrd.ProposalCode = "%";
                    mrd = objReportBAL.GetMRDProposalHabCovgBAL(mrd);
                }
                return View(mrd);
            }
            catch
            {
                return View(mrd);
            }
        }

        public ActionResult MRDProposalBridgeTypeDetails(string param/*String parameter, String hash, String key*/)
        {
            PMGSY.BAL.ProposalReports.IProposalReportsBAL objReportBAL = new PMGSY.BAL.ProposalReports.ProposalReportsBAL();
            PMGSY.Models.ProposalReports.MRDProposalBridgeTypeDetailsModel mrd = new MRDProposalBridgeTypeDetailsModel();
            try
            {
                if (ModelState.IsValid)
                {
                    //if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                    //{
                    //    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    //    if (urlParams.Length >= 1)
                    //    {
                    //String[] urlSplitParams = urlParams[0].Split('$');

                    string[] urlSplitParams = param.Split(',');

                    mrd.PrRoadCode = Convert.ToInt32(urlSplitParams[0]);
                    mrd.StateName = urlSplitParams[1];
                    mrd.DistrictName = urlSplitParams[2];
                    mrd.BlockName = urlSplitParams[3];
                    mrd.Package = urlSplitParams[4];
                    mrd.SanctionYear = urlSplitParams[5];
                    mrd.RoadName = urlSplitParams[6];
                    mrd.BridgeName = urlSplitParams[7];
                    mrd.BridgeLength = urlSplitParams[8];
                    mrd.RoadLength = urlSplitParams[9];

                    mrd.BridgeDetails = objReportBAL.GetMRDProposalBridgeDetailsBAL(mrd.PrRoadCode);
                    //    }
                    //}
                }

                return View(mrd);
            }
            catch
            {
                return View(mrd);
            }
        }

        public ActionResult MRDProposalBridgeEstCostDetails(string param/*String parameter, String hash, String key*/)
        {
            PMGSY.BAL.ProposalReports.IProposalReportsBAL objReportBAL = new PMGSY.BAL.ProposalReports.ProposalReportsBAL();
            PMGSY.Models.ProposalReports.MRDProposalBridgeTypeDetailsModel mrd = new MRDProposalBridgeTypeDetailsModel();
            try
            {
                if (ModelState.IsValid)
                {
                    //if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                    //{
                    //    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    //    if (urlParams.Length >= 1)
                    //    {
                    //String[] urlSplitParams = urlParams[0].Split('$');

                    string[] urlSplitParams = param.Split(',');


                    mrd.PrRoadCode = Convert.ToInt32(urlSplitParams[0]);
                    mrd.StateName = urlSplitParams[1];
                    mrd.DistrictName = urlSplitParams[2];
                    mrd.BlockName = urlSplitParams[3];
                    mrd.Package = urlSplitParams[4];
                    mrd.SanctionYear = urlSplitParams[5];
                    mrd.RoadName = urlSplitParams[6];
                    mrd.BridgeName = urlSplitParams[7];
                    mrd.BridgeLength = urlSplitParams[8];
                    mrd.RoadLength = urlSplitParams[9];

                    mrd.BridgeEstCostDetails = objReportBAL.GetMRDProposalBridgeEstCostDetailsBAL(mrd.PrRoadCode);
                    //    }
                    //}
                }

                return View(mrd);
            }
            catch
            {
                return View(mrd);
            }
        }

        public ActionResult MRDProposalBridgeCostDetails(string param/*String parameter, String hash, String key*/)
        {
            PMGSY.BAL.ProposalReports.IProposalReportsBAL objReportBAL = new PMGSY.BAL.ProposalReports.ProposalReportsBAL();
            PMGSY.Models.ProposalReports.MRDProposalBridgeTypeDetailsModel mrd = new MRDProposalBridgeTypeDetailsModel();
            try
            {
                if (ModelState.IsValid)
                {
                    //if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                    //{
                    //    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    //    if (urlParams.Length >= 1)
                    //    {
                    //String[] urlSplitParams = urlParams[0].Split('$');

                    string[] urlSplitParams = param.Split(',');

                    mrd.PMGSY = PMGSY.Extensions.PMGSYSession.Current.PMGSYScheme;

                    mrd.PrRoadCode = Convert.ToInt32(urlSplitParams[0]);
                    mrd.StateName = urlSplitParams[1];
                    mrd.DistrictName = urlSplitParams[2];
                    mrd.BlockName = urlSplitParams[3];
                    mrd.Package = urlSplitParams[4];
                    mrd.SanctionYear = urlSplitParams[5];
                    mrd.RoadName = urlSplitParams[6];
                    mrd.BridgeName = urlSplitParams[7];
                    mrd.BridgeLength = urlSplitParams[8];
                    mrd.Proposal = urlSplitParams[10];
                    mrd.RoadLength = urlSplitParams[9];

                    mrd.BridgeCostDetails = objReportBAL.GetMRDProposalBridgeCostDetailsBAL(mrd.PrRoadCode);
                    //    }
                    //}
                }

                return View(mrd);
            }
            catch
            {
                return View(mrd);
            }
        }

        public ActionResult MRDProposalBridgeFileDetails(string param/*String parameter, String hash, String key*/)
        {
            PMGSY.Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            SelectListItem item;
            string FullfilePhysicalPath = string.Empty;

            PMGSY.BAL.ProposalReports.IProposalReportsBAL objReportBAL = new PMGSY.BAL.ProposalReports.ProposalReportsBAL();
            PMGSY.Models.ProposalReports.MRDProposalBridgeTypeDetailsModel mrd = new MRDProposalBridgeTypeDetailsModel();
            try
            {
                if (ModelState.IsValid)
                {
                    //if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                    //{
                    //    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    //    if (urlParams.Length >= 1)
                    //    {
                    //String[] urlSplitParams = urlParams[0].Split('$');

                    string[] urlSplitParams = param.Split(',');

                    mrd.PrRoadCode = Convert.ToInt32(urlSplitParams[0]);
                    mrd.StateName = urlSplitParams[1];
                    mrd.DistrictName = urlSplitParams[2];
                    mrd.BlockName = urlSplitParams[3];
                    mrd.Package = urlSplitParams[4];
                    mrd.SanctionYear = urlSplitParams[5];
                    mrd.RoadName = urlSplitParams[6];
                    mrd.RoadLength = urlSplitParams[7];
                    mrd.CollabName = urlSplitParams[8];
                    mrd.BatchName = "Batch " + urlSplitParams[9];
                    mrd.BridgeName = urlSplitParams[10];
                    mrd.BridgeLength = urlSplitParams[11];
                    mrd.Proposal = urlSplitParams[12];

                    int roadCode = Convert.ToInt32(urlSplitParams[0].Trim());

                    var s = dbContext.IMS_PROPOSAL_FILES.Where(a => a.IMS_PR_ROAD_CODE == roadCode).OrderBy(a => a.IMS_PR_ROAD_CODE).ToList();

                    if (s.Count > 0)
                    {

                        mrd.path = new List<SelectListItem>();
                        foreach (var a in s)
                        {

                            //dsplnewsfiles.IssuedBy = a.ADMIN_NEWS.UM_User_Master.UserName.Trim();//a.ADMIN_NEWS.NEWS_USER_ID.ToString();
                            //dsplnewsfiles.IssuedDate = a.ADMIN_NEWS.NEWS_UPLOAD_DATE.ToString("dd/MM/yyyy hh:mm tt");
                            //dsplnewsfiles.Title = a.ADMIN_NEWS.NEWS_TITLE.Trim();
                            //dsplnewsfiles.Description = a.ADMIN_NEWS.NEWS_DESCRIPTION;

                            item = new SelectListItem();
                            if (a.ISPF_TYPE == "P")
                            {
                                //FullFileLogicalPath = Path.Combine(ConfigurationManager.AppSettings["NEWS_PDF_FILE_UPLOAD_VIRTUAL_DIR_PATH"], FileName);
                                //FullfilePhysicalPath = Path.Combine(ConfigurationManager.AppSettings["NEWS_PDF_FILE_UPLOAD"], a.FILE_NAME.Trim());
                                FullfilePhysicalPath = Path.Combine(ConfigurationManager.AppSettings["PROPOSAL_PDF_FILE_UPLOAD_VIRTUAL_DIR_PATH_PMGSYI"], a.IMS_FILE_NAME.Trim());
                            }
                            else
                            {
                                //FullFileLogicalPath = Path.Combine(ConfigurationManager.AppSettings["NEWS_IMAGE_FILE_UPLOAD_VIRTUAL_DIR_PATH"], FileName);
                                //FullfilePhysicalPath = Path.Combine(ConfigurationManager.AppSettings["NEWS_IMAGE_FILE_UPLOAD"], a.FILE_NAME.Trim());
                                FullfilePhysicalPath = Path.Combine(ConfigurationManager.AppSettings["PROPOSAL_FILE_UPLOAD_VIRTUAL_DIR_PATH_PMGSYI"], a.IMS_FILE_NAME.Trim());
                            }
                            item.Text = FullfilePhysicalPath.Trim();
                            item.Value = a.ISPF_TYPE.Trim();
                            mrd.path.Add(item);
                        }
                    }

                    //mrd.BridgeDetails = objReportBAL.GetMRDProposalBridgeDetailsBAL(mrd.PrRoadCode);
                    //    }
                    //}
                }

                return View(mrd);
            }
            catch
            {
                return View(mrd);
            }
        }

        public ActionResult MRDRoadCBRDetails(string param/*String parameter, String hash, String key*/)
        {
            PMGSY.BAL.ProposalReports.IProposalReportsBAL objReportBAL = new PMGSY.BAL.ProposalReports.ProposalReportsBAL();
            PMGSY.Models.ProposalReports.MRDProposalBridgeTypeDetailsModel mrd = new MRDProposalBridgeTypeDetailsModel();
            try
            {
                if (ModelState.IsValid)
                {
                    //if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                    //{
                    //    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    //    if (urlParams.Length >= 1)
                    //    {
                    //String[] urlSplitParams = urlParams[0].Split('$');

                    string[] urlSplitParams = param.Split(',');

                    mrd.PrRoadCode = Convert.ToInt32(urlSplitParams[0]);
                    mrd.StateName = urlSplitParams[1];
                    mrd.DistrictName = urlSplitParams[2];
                    mrd.BlockName = urlSplitParams[3];
                    mrd.Package = urlSplitParams[4];
                    mrd.SanctionYear = urlSplitParams[5];
                    mrd.RoadName = urlSplitParams[6];
                    mrd.BridgeName = urlSplitParams[7];
                    mrd.BridgeLength = urlSplitParams[8];
                    mrd.RoadLength = urlSplitParams[9];

                    mrd.RoadCBRDetails = objReportBAL.GetMRDProposalRoadCBRDetailsBAL(mrd.PrRoadCode);
                    //    }
                    //}
                }

                return View(mrd);
            }
            catch
            {
                return View(mrd);
            }
        }

        public ActionResult MRDProposalDataReportRoad(MRDProposalModel mrd)
        {
            PMGSY.BAL.ProposalReports.IProposalReportsBAL objReportBAL = new PMGSY.BAL.ProposalReports.ProposalReportsBAL();
            try
            {
                if (ModelState.IsValid)
                {
                    mrd.ProposalCode = "P";
                    mrd = objReportBAL.GetMRDProposalBAL(mrd);


                }
                return View(mrd);
            }
            catch
            {
                return View(mrd);
            }


        }


        public JsonResult GetDistrictbyState(string stateCode)
        {
            int outParam = 0;
            PMGSY.DAL.Master.MasterDAL objDAL = new PMGSY.DAL.Master.MasterDAL();
            try
            {
                if (!int.TryParse(stateCode, out outParam))
                {
                    return Json(false);
                }

                List<PMGSY.Models.MASTER_DISTRICT> districtList = new List<PMGSY.Models.MASTER_DISTRICT>();

                //districtList = objDAL.GetAllDistricts(Convert.ToInt32(stateCode.Trim()));
                districtList = objDAL.GetAllDistrictByStateCode(Convert.ToInt32(stateCode));

                //if (districtList.Count == 0)
                //{
                districtList.Insert(0, new PMGSY.Models.MASTER_DISTRICT() { MAST_DISTRICT_CODE = 0, MAST_DISTRICT_NAME = "All Districts" });
                //}

                return Json(new SelectList(districtList, "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME"));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return Json(false);
            }
        }//end function GetDistrictsByStateCode

        public JsonResult GetBlocksbyDistrict(string distCode)
        {
            int outParam = 0;
            PMGSY.DAL.Master.MasterDAL objDAL = new PMGSY.DAL.Master.MasterDAL();
            try
            {
                if (!int.TryParse(distCode, out outParam))
                {
                    return Json(false);
                }
                objCommonFunctions = new CommonFunctions();
                List<SelectListItem> lstBlock = new List<SelectListItem>();
                lstBlock = objCommonFunctions.PopulateBlocks(Convert.ToInt32(distCode), true);
                lstBlock.RemoveAt(0);
                lstBlock.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });

                return Json(lstBlock);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return Json(false);
            }
        }//end function GetDistrictsByStateCode

        /// <summary>
        /// Populate Agencies based on selected state
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult PopulateAgencies(int stateCode)
        {
            try
            {
                CommonFunctions comm = new CommonFunctions();
                //int stateCode = Convert.ToInt32(Request.Params["stateCode"]);

                return Json(comm.PopulateAgencies(stateCode, true));
            }
            catch
            {
                return Json(new { string.Empty });
            }
        }

        public JsonResult GetPackages(string param)
        {
            int outParam = 0;
            PMGSY.DAL.Master.MasterDAL objDAL = new PMGSY.DAL.Master.MasterDAL();
            try
            {
                //if (!int.TryParse(stateCode, out outParam))
                //{
                //    return Json(false);
                //}

                List<PMGSY.Models.IMS_SANCTIONED_PROJECTS> packageList = new List<PMGSY.Models.IMS_SANCTIONED_PROJECTS>();

                //districtList = objDAL.GetAllDistricts(Convert.ToInt32(stateCode.Trim()));
                packageList = PopulatePackages(param);

                //if (districtList.Count == 0)
                //{
                //districtList.Insert(0, new PMGSY.Models.MASTER_DISTRICT() { MAST_DISTRICT_CODE = 0, MAST_DISTRICT_NAME = "All Districts" });
                //}

                return Json(new SelectList(packageList, "IMS_ROAD_NAME", "IMS_PACKAGE_ID"));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return Json(false);
            }
        }//end function GetDistrictsByStateCode

        public List<IMS_SANCTIONED_PROJECTS> PopulatePackages(string param)
        {
            int stateCode = 0;
            int distCode = 0;
            int year = 0;
            int batchCode = 0;
            int collabCode = 0;

            if (param.Contains('$'))
            {
                string[] code = param.Split('$');
                if (PMGSYSession.Current.StateCode > 0)
                {
                    stateCode = Convert.ToInt32(PMGSYSession.Current.StateCode);
                }
                else
                {
                    stateCode = Convert.ToInt32(code[0]);
                }
                if (code.Count() > 1)
                {
                    if (PMGSYSession.Current.DistrictCode > 0)
                    {
                        distCode = Convert.ToInt32(PMGSYSession.Current.DistrictCode);
                    }
                    else
                    {
                        distCode = Convert.ToInt32(code[1]);
                    }
                }
                if (code.Count() > 2)
                {
                    year = Convert.ToInt32(code[2]);
                }
                if (code.Count() > 3)
                {
                    batchCode = Convert.ToInt32(code[3]);
                }
                if (code.Count() > 4)
                {
                    collabCode = Convert.ToInt32(code[4]);
                }
            }
            else
            {
                if (param == "0")
                {
                    stateCode = PMGSYSession.Current.StateCode;
                }
            }
            //int outParam = 0;
            PMGSY.DAL.Master.MasterDAL objDAL = new PMGSY.DAL.Master.MasterDAL();
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {

                List<IMS_SANCTIONED_PROJECTS> packageList = (from sanctionProjects in dbContext.IMS_SANCTIONED_PROJECTS
                                                             where
                                                             (year == 0 ? 1 : sanctionProjects.IMS_YEAR) == (year == 0 ? 1 : year) &&
                                                                 //sanctionProjects.IMS_YEAR == year &&
                                                                 //(block == 0 ? 1 : sanctionProjects.MAST_BLOCK_CODE) == (block == 0 ? 1 : block) &&
                                                             sanctionProjects.IMS_SANCTIONED == "Y" &&
                                                             sanctionProjects.IMS_DPR_STATUS == "N" &&
                                                             (stateCode == 0 ? 1 : sanctionProjects.MAST_STATE_CODE) == (stateCode == 0 ? 1 : stateCode) &&
                                                             (distCode == 0 ? 1 : sanctionProjects.MAST_DISTRICT_CODE) == (distCode == 0 ? 1 : distCode) &&
                                                                 //sanctionProjects.MAST_DPIU_CODE == PMGSYSession.Current.AdminNdCode &&
                                                             sanctionProjects.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme && // new change done by Vikram as Packages should be populated based on the Scheme in session
                                                             (collabCode <= 0 ? 1 : sanctionProjects.IMS_STREAMS) == (collabCode <= 0 ? 1 : collabCode)
                                                             select sanctionProjects).Distinct().OrderBy(s => s.IMS_PACKAGE_ID).ToList<IMS_SANCTIONED_PROJECTS>();


                //packageList=packageList.GroupBy(

                packageList = packageList.GroupBy(pl => pl.IMS_PACKAGE_ID).Select(pl => pl.FirstOrDefault()).ToList<IMS_SANCTIONED_PROJECTS>();

                //if (isSearch)
                //{
                packageList.Insert(0, new IMS_SANCTIONED_PROJECTS() { IMS_ROAD_NAME = "0", IMS_PACKAGE_ID = "All Packages" });
                //}
                //else
                //{
                //    packageList.Insert(0, new IMS_SANCTIONED_PROJECTS() { IMS_ROAD_NAME = "0", IMS_PACKAGE_ID = "Select Package" });

                //}

                return packageList;

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

        }//end function GetDistrictsByStateCode

        #endregion
    }
}