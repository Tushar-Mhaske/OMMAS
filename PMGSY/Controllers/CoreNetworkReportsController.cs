#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   CoreNetworkReportsController.cs        
        * Description   :   Listing of Records for Core Networks.
        * Author        :   Pranav Nerkar 
        * Creation Date :   7/October/2013
 **/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.BAL.CoreNetworkReports;
using PMGSY.Models.CoreNetworkReports;
using PMGSY.Extensions;
using PMGSY.DAL.CoreNetworkReports;
using PMGSY.Common;
namespace PMGSY.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    [Audit]
    public class CoreNetworkReportsController : Controller
    {
        ICoreNetworkReportsBAL coreNetworkReportsBAL;

        #region Core Network 1 Reports

        public ActionResult CN1ReportDetails()
        {
            //return View();
            CoreNetworkReportViewModel CoreNetworkReportsViewModel = new CoreNetworkReportViewModel();
            try
            {
                CoreNetworkReportsViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                CoreNetworkReportsViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                SelectListItem all = new SelectListItem
                {
                    Selected = true,
                    Text = "All",
                    Value = "0"
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
                ViewData["ROUTE"] = route;
                return View(CoreNetworkReportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult CN1ReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            string route = formCollection["Route"];
            int totalRecords;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.CN1ReportListingBAL(route, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public ActionResult CN1DistrictReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();

            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            string route = formCollection["Route"];

            int totalRecords;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.CN1DistrictReportListingBAL(route, stateCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public ActionResult CN1BlockReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
            //int stateCode = 2;
            //int districtCode = 1;
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            string route = formCollection["Route"];

            int totalRecords;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.CN1BlockReportListingBAL(route, stateCode, districtCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public ActionResult CN1FinalBlockReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int blockCode = Convert.ToInt32(formCollection["BlockCode"]);
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
            //int stateCode = 2;
            //int blockCode = 22;
            //int districtCode = 1;
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int population = 0;
            int roadcategory = 0;
            string route = formCollection["Route"];

            int totalRecords;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.CN1FinalBlockReportListingBAL(population, roadcategory, route, blockCode, stateCode, districtCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        #endregion

        #region Core Network 2 Reports


        public ActionResult CN2ReportDetails()
        {
            CoreNetworkReportViewModel CoreNetworkReportsViewModel = new CoreNetworkReportViewModel();
            try
            {
                CoreNetworkReportsViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                CoreNetworkReportsViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                SelectListItem all = new SelectListItem
                {
                    Selected = true,
                    Text = "All",
                    Value = "0"
                };

                SelectListItem l1000 = new SelectListItem
                {
                    Text = "1000+",
                    Value = "4"
                };

                SelectListItem l999 = new SelectListItem
                {
                    Text = "499-999",
                    Value = "3"
                };
                SelectListItem l499 = new SelectListItem
                {
                    Text = "250-499",
                    Value = "2"
                };
                SelectListItem l250 = new SelectListItem
                {
                    Text = "<250",
                    Value = "1"
                };
                List<SelectListItem> pop = new List<SelectListItem>();
                pop.Add(all);
                pop.Add(l250);
                pop.Add(l499);
                pop.Add(l999);
                pop.Add(l1000);


                ViewData["Population"] = pop;
                return View(CoreNetworkReportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult CN2StateReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int population = Convert.ToInt32(formCollection["Population"]);

            int totalRecords;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.CN2StateReportListingBAL(population, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public ActionResult CN2DistrictReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int population = Convert.ToInt32(formCollection["Population"]);
            int totalRecords = 0;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.CN2DistrictReportListingBAL(population, stateCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]       
        public ActionResult CN2BlockReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int population = Convert.ToInt32(formCollection["Population"]);

            int totalRecords;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.CN2BlockReportListingBAL(population, stateCode, districtCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]       
        public ActionResult CN2FinalReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int blockCode = Convert.ToInt32(formCollection["BlockCode"]);
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int population = Convert.ToInt32(formCollection["Population"]);
            int roadcategory = 0;
            string route = "0";
            int totalRecords;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.CN2FinalListingBAL(population, roadcategory, route, blockCode, districtCode, stateCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        #endregion

        #region Core Network 3 Reports


       
        public ActionResult CN3ReportDetails()
        {
            //return View();
            CoreNetworkReportsDAL objDAL = new CoreNetworkReportsDAL();
            CoreNetworkReportViewModel CoreNetworkReportsViewModel = new CoreNetworkReportViewModel();
            try
            {
                CoreNetworkReportsViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                CoreNetworkReportsViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                SelectListItem all = new SelectListItem
                {
                    Selected = true,
                    Text = "All",
                    Value = "%"
                };
                SelectListItem allCategory = new SelectListItem
                {
                    Selected = true,
                    Text = "All",
                    Value = "0"
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
                List<SelectListItem> roadCategory = objDAL.PopulateRoadCategoryList();
                ViewData["Route"] = route;
                ViewData["RoadCategory"] = roadCategory;



                return View(CoreNetworkReportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]       
        public ActionResult CN3StateReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int roadCategory = Convert.ToInt32(formCollection["RoadCategory"]);
            string route = formCollection["Route"];
            int totalRecords;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.CN3StateReportListingBAL(roadCategory, route, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]       
        public ActionResult CN3DistrictReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int roadCategory = Convert.ToInt32(formCollection["RoadCategory"]);
            string route = formCollection["Route"];

            int totalRecords = 0;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.CN3DistrictReportListingBAL(roadCategory, route, stateCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public ActionResult CN3BlockReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int roadCategory = Convert.ToInt32(formCollection["RoadCategory"]);
            string route = formCollection["Route"];
            int totalRecords = 0;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.CN3BlockReportListingBAL(roadCategory, route, stateCode, districtCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]    
        public ActionResult CN3FinalReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int blockCode = Convert.ToInt32(formCollection["BlockCode"]);
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int poulation = 0;
            int roadCategory = Convert.ToInt32(formCollection["RoadCategory"]);
            string route = formCollection["Route"];
            int totalRecords = 0;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.CN3FinalListingBAL(poulation, roadCategory, route, blockCode, districtCode, stateCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        #endregion

        #region Core Network 4 Reports


     
        public ActionResult CN4ReportDetails()
        {
            //return View();
            CoreNetworkReportViewModel CoreNetworkReportsViewModel = new CoreNetworkReportViewModel();
            try
            {
                CoreNetworkReportsViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                CoreNetworkReportsViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;

                return View(CoreNetworkReportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]       
        public ActionResult CN4StateReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int totalRecords;
            int roadCategory = Convert.ToInt32(formCollection["RoadCategory"]);
            string route = formCollection["Route"];

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.CN4StateReportListingBAL(roadCategory, route, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]       
        public ActionResult CN4DistrictReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int roadCategory = Convert.ToInt32(formCollection["RoadCategory"]);
            string route = formCollection["Route"];

            int totalRecords = 0;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.CN4DistrictReportListingBAL(roadCategory, route, stateCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public ActionResult CN4BlockReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int roadCategory = Convert.ToInt32(formCollection["RoadCategory"]);
            string route = formCollection["Route"];

            int totalRecords = 0;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.CN4BlockReportListingBAL(roadCategory, route, stateCode, districtCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public ActionResult CN4FinalReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int blockCode = Convert.ToInt32(formCollection["BlockCode"]);
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int population = 0;
            int roadCategory = Convert.ToInt32(formCollection["RoadCategory"]);
            string route = formCollection["Route"];

            int totalRecords = 0;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.CN4FinalListingBAL(population, roadCategory, route, blockCode, districtCode, stateCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        #endregion Core Network 4 Reports

        #region Core Network 5 Reports


        [Audit]
        public ActionResult CN5ReportDetails()
        {
            //return View();
            CoreNetworkReportViewModel CoreNetworkReportsViewModel = new CoreNetworkReportViewModel();
            CoreNetworkReportsDAL objDal = new CoreNetworkReportsDAL();
            try
            {
                CoreNetworkReportsViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                CoreNetworkReportsViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                return View(CoreNetworkReportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]    
        public ActionResult CN5StateReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int totalRecords;
            int roadCategory = Convert.ToInt32(formCollection["RoadCategory"]);
            string route = formCollection["Route"];

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.CN5StateReportListingBAL(roadCategory, route, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]      
        public ActionResult CN5DistrictReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int roadCategory = Convert.ToInt32(formCollection["RoadCategory"]);
            string route = formCollection["Route"];

            int totalRecords = 0;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.CN5DistrictReportListingBAL(roadCategory, route, stateCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]      
        public ActionResult CN5BlockReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int roadCategory = Convert.ToInt32(formCollection["RoadCategory"]);
            string route = formCollection["Route"];

            int totalRecords = 0;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.CN5BlockReportListingBAL(roadCategory, route, stateCode, districtCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]       
        public ActionResult CN5FinalReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int blockCode = Convert.ToInt32(formCollection["BlockCode"]);
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int population = 0;
            int roadCategory = Convert.ToInt32(formCollection["RoadCategory"]);
            string route = formCollection["Route"];

            int totalRecords = 0;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.CN5FinalListingBAL(population, roadCategory, route, blockCode, districtCode, stateCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        #endregion Core Network 5 Reports

        #region Core Network 6 Reports


       
        public ActionResult CN6ReportDetails()
        {
            //return View();
            CoreNetworkReportViewModel CoreNetworkReportsViewModel = new CoreNetworkReportViewModel();
            try
            {
                CoreNetworkReportsViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                CoreNetworkReportsViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
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

                return View(CoreNetworkReportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]      
        public ActionResult CN6StateReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            string route = formCollection["Route"];
            int totalRecords;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.CN6StateReportListingBAL(route, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public ActionResult CN6DistrictReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            string route = formCollection["Route"];
            int totalRecords = 0;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.CN6DistrictReportListingBAL(route, stateCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]     
        public ActionResult CN6BlockReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            string route = formCollection["Route"];

            int totalRecords = 0;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.CN6BlockReportListingBAL(route, stateCode, districtCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public ActionResult CN6FinalReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int blockCode = Convert.ToInt32(formCollection["BlockCode"]);
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            string route = formCollection["Route"];

            int totalRecords = 0;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.CN6FinalListingBAL(route, blockCode, districtCode, stateCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        #endregion

        #region CNCPL Reports


     
        public ActionResult CNCPLDetails()
        {
            //return View();
            CoreNetworkReportViewModel CoreNetworkReportsViewModel = new CoreNetworkReportViewModel();
            try
            {
                CoreNetworkReportsViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                CoreNetworkReportsViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
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
                return View(CoreNetworkReportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]      
        public ActionResult CNCPLStateReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            string route = formCollection["Route"];

            int totalRecords;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.CNCPLStateListingBAL(route, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public ActionResult CNCPLDistrictReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            string route = formCollection["Route"];
            int totalRecords = 0;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.CNCPLDistrictListingBAL(route, stateCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public ActionResult CNCPLBlockReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            string route = formCollection["Route"];
            int totalRecords = 0;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.CNCPLBlockListingBAL(route, districtCode, stateCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public ActionResult CNCPLFinalReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int blockCode = Convert.ToInt32(formCollection["BlockCode"]);
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            string route = formCollection["Route"];
            int totalRecords = 0;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.CNCPLFinalListingBAL(route, blockCode, districtCode, stateCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        #endregion

        #region Habitation Wise Core Network(HWCN)

        [Audit]
        public ActionResult HWCNDetails()
        {

            CoreNetworkReportViewModel CoreNetworkReportsViewModel = new CoreNetworkReportViewModel();
            try
            {
                CoreNetworkReportsViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                CoreNetworkReportsViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
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
                return View(CoreNetworkReportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult HWCNStateReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            string route = formCollection["Route"];
            int totalRecords;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.HWCNStateListingBAL(route, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public ActionResult HWCNDistrictReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            string route = formCollection["Route"];
            int totalRecords = 0;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.HWCNDistrictListingBAL(route, stateCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public ActionResult HWCNBlockReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            string route = formCollection["Route"];
            int totalRecords = 0;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.HWCNBlockListingBAL(route, districtCode, stateCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public ActionResult HWCNFinalReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int blockCode = Convert.ToInt32(formCollection["BlockCode"]);
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            string route = formCollection["Route"];
            int totalRecords = 0;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.HWCNFinalListingBAL(route, blockCode, districtCode, stateCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        #endregion

        #region Road Wise Core Network(RWCN)

        [Audit]
        public ActionResult RWCNDetails()
        {
            CoreNetworkReportViewModel CoreNetworkReportsViewModel = new CoreNetworkReportViewModel();
            CoreNetworkReportsDAL objDAL = new CoreNetworkReportsDAL();
            try
            {
                CoreNetworkReportsViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                CoreNetworkReportsViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
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
                return View(CoreNetworkReportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]

        public ActionResult RWCNStateReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int roadCategory = Convert.ToInt32(formCollection["RoadCategory"]);
            string route = formCollection["Route"];
            //string route = "%";

            int totalRecords;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.RWCNStateListingBAL(route, roadCategory, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public ActionResult RWCNDistrictReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int roadCategory = Convert.ToInt32(formCollection["RoadCategory"]);
            string route = formCollection["Route"];

            int totalRecords = 0;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.RWCNDistrictListingBAL(route, roadCategory, stateCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public ActionResult RWCNBlockReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int roadCategory = Convert.ToInt32(formCollection["RoadCategory"]);
            string route = formCollection["Route"];

            int totalRecords = 0;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.RWCNBlockListingBAL(route, roadCategory, districtCode, stateCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public ActionResult RWCNFinalReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int blockCode = Convert.ToInt32(formCollection["BlockCode"]);
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int population = 0;
            int roadCategory = Convert.ToInt32(formCollection["RoadCategory"]);
            string route = formCollection["Route"];
            int totalRecords = 0;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.RWCNFinalListingBAL(population, route, roadCategory, blockCode, districtCode, stateCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        #endregion

        #region PCI Abstract

        [Audit]
        public ActionResult PCIAbstractDetails()
        {
            CoreNetworkReportViewModel CoreNetworkReportsViewModel = new CoreNetworkReportViewModel();
            try
            {
                CoreNetworkReportsViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                CoreNetworkReportsViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
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
                return View(CoreNetworkReportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult PCIAbstractStateReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            string route = formCollection["Route"];
            int totalRecords;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.PCIAbstractStateListingBAL(route, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public ActionResult PCIAbstractDistrictReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            string route = formCollection["Route"];
            int totalRecords = 0;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.PCIAbstractDistrictListingBAL(route, stateCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public ActionResult PCIAbstractBlockReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            string route = formCollection["Route"];
            int totalRecords = 0;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.PCIAbstractBlockListingBAL(route, districtCode, stateCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public ActionResult PCIAbstractFinalReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int blockCode = Convert.ToInt32(formCollection["BlockCode"]);
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            string route = formCollection["Route"];
            int totalRecords = 0;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.PCIAbstractFinalListingBAL(route, blockCode, districtCode, stateCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        #endregion

        #region CUCPL

        [Audit]
        public ActionResult CUCPLDetails()
        {
            //return View();
            CoreNetworkReportViewModel CoreNetworkReportsViewModel = new CoreNetworkReportViewModel();
            try
            {
                CoreNetworkReportsViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                CoreNetworkReportsViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
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
                return View(CoreNetworkReportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult CUCPLStateReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            string route = formCollection["Route"];

            int totalRecords;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.CUCPLStateListingBAL(route, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
       
        public ActionResult CUCPLDistrictReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            string route = formCollection["Route"];
            int totalRecords = 0;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.CUCPLDistrictListingBAL(route, stateCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]      
        public ActionResult CUCPLBlockReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            string route = formCollection["Route"];

            int totalRecords = 0;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.CUCPLBlockListingBAL(route, districtCode, stateCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]      
        public ActionResult CUCPLFinalReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int blockCode = Convert.ToInt32(formCollection["BlockCode"]);
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            string route = formCollection["Route"];

            int totalRecords = 0;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.CUCPLFinalListingBAL(route, blockCode, districtCode, stateCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        #endregion


        #region Core Network R1 Reports

        public ActionResult CNR1ReportDetails()
        {
            //return View();
            //Link / Through/All
            CoreNetworkReportViewModel CoreNetworkReportsViewModel = new CoreNetworkReportViewModel();
            CoreNetworkReportsDAL objDAL = new CoreNetworkReportsDAL();
            try
            {

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
                List<SelectListItem> roadCategory = objDAL.PopulateRoadCategoryList();

                ViewData["Route"] = route;
                ViewData["RoadCategory"] = roadCategory;

                CoreNetworkReportsViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                CoreNetworkReportsViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;


                return View(CoreNetworkReportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]      
        public ActionResult CNR1StateReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            string route = formCollection["Route"];
            int roadCategory = Convert.ToInt32(formCollection["RoadCategory"]);

            int totalRecords;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.CNR1StateReportListingBAL(roadCategory, route, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]       
        public ActionResult CNR1DistrictReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();

            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            string route = formCollection["Route"];
            int roadCategory = Convert.ToInt32(formCollection["RoadCategory"]);

            int totalRecords;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.CNR1DistrictReportListingBAL(roadCategory, route, stateCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]    
        public ActionResult CNR1BlockReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);

            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int roadCategory = Convert.ToInt32(formCollection["RoadCategory"]);
            string route = formCollection["Route"];

            int totalRecords;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.CNR1BlockReportListingBAL(roadCategory, route, stateCode, districtCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]    
        public ActionResult CNR1FinalBlockReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int blockCode = Convert.ToInt32(formCollection["BlockCode"]);
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
            string route = formCollection["Route"];
            int roadCategory = Convert.ToInt32(formCollection["RoadCategory"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int population = 0;


            int totalRecords;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.CNR1FinalBlockReportListingBAL(population, roadCategory, route, blockCode, stateCode, districtCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


        #endregion


        #region Core Network R2 Reports

    
        public ActionResult CNR2ReportDetails()
        {
            CoreNetworkReportViewModel CoreNetworkReportsViewModel = new CoreNetworkReportViewModel();
            CoreNetworkReportsDAL objDAL = new CoreNetworkReportsDAL();
            try
            {
                CoreNetworkReportsViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                CoreNetworkReportsViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
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
                List<SelectListItem> roadCategory = objDAL.PopulateRoadCategoryList();


                ViewData["Route"] = route;
                ViewData["RoadCategory"] = roadCategory;
                return View(CoreNetworkReportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult CNR2StateReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int roadCategory = Convert.ToInt32(formCollection["RoadCategory"]);
            string route = formCollection["Route"];
            int totalRecords;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.CNR2StateReportListingBAL(roadCategory, route, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public ActionResult CNR2DistrictReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int roadCategory = Convert.ToInt32(formCollection["RoadCategory"]);
            string route = formCollection["Route"];
            int totalRecords = 0;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.CNR2DistrictReportListingBAL(roadCategory, route, stateCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public ActionResult CNR2BlockReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int roadCategory = Convert.ToInt32(formCollection["RoadCategory"]);
            string route = formCollection["Route"];
            int totalRecords;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.CNR2BlockReportListingBAL(roadCategory, route, stateCode, districtCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public ActionResult CNR2FinalReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int blockCode = Convert.ToInt32(formCollection["BlockCode"]);
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int roadCategory = Convert.ToInt32(formCollection["RoadCategory"]);
            string route = formCollection["Route"];

            int totalRecords;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.CNR2FinalBlockReportListingBAL(roadCategory, route, blockCode, stateCode, districtCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        #endregion

        #region Core Network R3 Reports

        [Audit]
        public ActionResult CNR3ReportDetails()
        {
            CoreNetworkReportViewModel CoreNetworkReportsViewModel = new CoreNetworkReportViewModel();
            CoreNetworkReportsDAL objDAL = new CoreNetworkReportsDAL();
            try
            {
                CoreNetworkReportsViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                CoreNetworkReportsViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
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
                List<SelectListItem> roadCategory = objDAL.PopulateRoadCategoryList();



                ViewData["Route"] = route;
                ViewData["RoadCategory"] = roadCategory;
                return View(CoreNetworkReportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult CNR3StateReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int roadCategory = Convert.ToInt32(formCollection["RoadCategory"]);
            string route = formCollection["Route"];
            int totalRecords;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.CNR3StateReportListingBAL(roadCategory, route, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public ActionResult CNR3DistrictReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int roadCategory = Convert.ToInt32(formCollection["RoadCategory"]);
            string route = formCollection["Route"];

            int totalRecords = 0;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.CNR3DistrictReportListingBAL(roadCategory, route, stateCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public ActionResult CNR3BlockReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int roadCategory = Convert.ToInt32(formCollection["RoadCategory"]);
            string route = formCollection["Route"];

            int totalRecords;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.CNR3BlockReportListingBAL(roadCategory, route, stateCode, districtCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public ActionResult CNR3FinalReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int blockCode = Convert.ToInt32(formCollection["BlockCode"]);
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int roadCategory = Convert.ToInt32(formCollection["RoadCategory"]);
            string route = formCollection["Route"];

            int totalRecords;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.CNR3FinalBlockReportListingBAL(roadCategory, route, blockCode, stateCode, districtCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        #endregion

        #region Core Network R4 Reports

      
        public ActionResult CNR4ReportDetails()
        {
            CoreNetworkReportViewModel CoreNetworkReportsViewModel = new CoreNetworkReportViewModel();
            CoreNetworkReportsDAL objDAL = new CoreNetworkReportsDAL();
            try
            {
                CoreNetworkReportsViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                CoreNetworkReportsViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
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
                List<SelectListItem> roadCategory = objDAL.PopulateRoadCategoryList();


                ViewData["Route"] = route;
                ViewData["RoadCategory"] = roadCategory;
                return View(CoreNetworkReportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult CNR4StateReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int roadCategory = Convert.ToInt32(formCollection["RoadCategory"]);
            string route = formCollection["Route"];
            string length = "%";
            int totalRecords;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.CNR4StateReportListingBAL(roadCategory, route, length, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public ActionResult CNR4DistrictReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int roadCategory = Convert.ToInt32(formCollection["RoadCategory"]);
            string route = formCollection["Route"];
            string length = "%";
            int totalRecords = 0;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.CNR4DistrictReportListingBAL(roadCategory, route, length, stateCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public ActionResult CNR4BlockReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int roadCategory = Convert.ToInt32(formCollection["RoadCategory"]);
            string route = formCollection["Route"];
            string length = "%";
            int totalRecords;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.CNR4BlockReportListingBAL(roadCategory, route, length, stateCode, districtCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public ActionResult CNR4FinalReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int blockCode = Convert.ToInt32(formCollection["BlockCode"]);
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            // int roadCategory = Convert.ToInt32(formCollection["RoadCategory"]);
            int roadCategory = 0;
            string route = formCollection["Route"];
            string length = "%";
            int totalRecords;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.CNR4FinalBlockReportListingBAL(roadCategory, route, length, blockCode, stateCode, districtCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        #endregion

        #region Core Network Priority Reports

    
        public ActionResult CNPriorityReportDetails()
        {
            CoreNetworkReportViewModel CoreNetworkReportsViewModel = new CoreNetworkReportViewModel();
            try
            {
                SelectListItem one = new SelectListItem
                {
                    Selected = true,
                    Text = "Unconnected Habitation having Population >= 1000 ",
                    Value = "1"
                };

                SelectListItem two = new SelectListItem
                {
                    Text = "Unconnected Habitation having Population between 500 and 1000 ",
                    Value = "2"
                };

                SelectListItem three = new SelectListItem
                {
                    Text = "Habitation having Population >= 1000 and Road Surface Type is GRAVEL ",
                    Value = "3"
                };

                SelectListItem four = new SelectListItem
                {
                    Text = "Habitation having Population between 500 and 1000 and Road Surface Type is GRAVEL ",
                    Value = "4"
                };

                SelectListItem five = new SelectListItem
                {
                    Text = "Habitation having Population >= 1000 and Road Surface Type is WBM",
                    Value = "5"
                };

                SelectListItem six = new SelectListItem
                {
                    Text = "Habitation having Population between 500 and 1000 and Road Surface Type is WBM",
                    Value = "6"
                };
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

                List<SelectListItem> prority = new List<SelectListItem>();
                prority.Add(one);
                prority.Add(two);
                prority.Add(three);
                prority.Add(four);
                prority.Add(five);
                prority.Add(six);
                ViewData["Priority"] = prority;
                ViewData["Route"] = route;

                CoreNetworkReportsViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                CoreNetworkReportsViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;

                return View(CoreNetworkReportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]      
        public ActionResult CNPriorityStateReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int priority = Convert.ToInt32(formCollection["Priority"]);
            string route = formCollection["Route"];
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int totalRecords;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.CNPriorityStateReportListingBAL(priority, route, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]      
        public ActionResult CNPriorityDistrictReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int priority = Convert.ToInt32(formCollection["Priority"]);
            string route = formCollection["Route"];
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int totalRecords = 0;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.CNPriorityDistrictReportListingBAL(stateCode, priority, route, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]      
        public ActionResult CNPriorityBlockReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
            int priority = Convert.ToInt32(formCollection["Priority"]);
            string route = formCollection["Route"];
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int totalRecords;
            var jsonData = new
            {
                rows = coreNetworkReportsBAL.CNPriorityBlockReportListingBAL(stateCode, districtCode, priority, route, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]       
        public ActionResult CNPriorityFinalReportListing(FormCollection formCollection)
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
            coreNetworkReportsBAL = new CoreNetworkReportsBAL();
            int blockCode = Convert.ToInt32(formCollection["BlockCode"]);
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
            int priority = Convert.ToInt32(formCollection["Priority"]);
            string route = formCollection["Route"];
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];

            int totalRecords;

            var jsonData = new
            {
                rows = coreNetworkReportsBAL.CNPriorityFinalBlockReportListingBAL(blockCode, districtCode, stateCode, priority, route, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        #endregion
    }
}
