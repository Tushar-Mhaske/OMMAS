#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   CoreNetworkReportsController.cs        
        * Description   :   Listing of Records for Core Networks.
        * Author        :   Deepak Madane
        * Creation Date :   16/December/2013
 **/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.BAL.ExistingRoadsReports;
using PMGSY.Models.ExistingRoadsReports;
using PMGSY.Extensions;
using PMGSY.DAL.ExistingRoadsReports;
using PMGSY.Common;

namespace PMGSY.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    [Audit]
    public class ExistingRoadsReportsController : Controller
    {
        IExistingRoadsReportsBAL existingRoadsReportsBAL;

        #region ERR1 Reports
       
        public ActionResult ERR1ReportDetails()
        {
            ExistingRoadsReportsViewModel existingRoadsReportsViewModel = new ExistingRoadsReportsViewModel();
            try
            {
                existingRoadsReportsViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                existingRoadsReportsViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;

                return View(existingRoadsReportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
       
        public ActionResult ERR1StateReportListing(FormCollection formCollection)
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
            existingRoadsReportsBAL = new ExistingRoadsReportsBAL();
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int totalRecords;

            var jsonData = new
            {
                rows = existingRoadsReportsBAL.ERR1StateReportListingBAL(page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]       
        public ActionResult ERR1DistrictReportListing(FormCollection formCollection)
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
            existingRoadsReportsBAL = new ExistingRoadsReportsBAL();
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int totalRecords = 0;

            var jsonData = new
            {
                rows = existingRoadsReportsBAL.ERR1DistrictReportListingBAL(stateCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]       
        public ActionResult ERR1BlockReportListing(FormCollection formCollection)
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
            existingRoadsReportsBAL = new ExistingRoadsReportsBAL();
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];

            int totalRecords;

            var jsonData = new  
            {
                rows = existingRoadsReportsBAL.ERR1BlockReportListingBAL(stateCode, districtCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult; 
        }

        [HttpPost]       
        public ActionResult ERR1FinalReportListing(FormCollection formCollection)
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
            existingRoadsReportsBAL = new ExistingRoadsReportsBAL();
            int blockCode = Convert.ToInt32(formCollection["BlockCode"]);
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];

            int totalRecords;

            var jsonData = new
            {
                rows = existingRoadsReportsBAL.ERR1FinalListingBAL(blockCode, districtCode, stateCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        #endregion

        #region ERR2 Reports
       
        public ActionResult ERR2ReportDetails()
        {
            ExistingRoadsReportsViewModel existingRoadsReportsViewModel = new ExistingRoadsReportsViewModel();
            try
            {
                existingRoadsReportsViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                existingRoadsReportsViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;

                return View(existingRoadsReportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]       
        public ActionResult ERR2StateReportListing(FormCollection formCollection)
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
            existingRoadsReportsBAL = new ExistingRoadsReportsBAL();
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int totalRecords;

            var jsonData = new
            {
                rows = existingRoadsReportsBAL.ERR2StateReportListingBAL(page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]       
        public ActionResult ERR2DistrictReportListing(FormCollection formCollection)
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
            existingRoadsReportsBAL = new ExistingRoadsReportsBAL();
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int totalRecords = 0;

            var jsonData = new
            {
                rows = existingRoadsReportsBAL.ERR2DistrictReportListingBAL(stateCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]        
        public ActionResult ERR2BlockReportListing(FormCollection formCollection)
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
            existingRoadsReportsBAL = new ExistingRoadsReportsBAL();
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];

            int totalRecords;

            var jsonData = new
            {
                rows = existingRoadsReportsBAL.ERR2BlockReportListingBAL(stateCode, districtCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]       
        public ActionResult ERR2FinalReportListing(FormCollection formCollection)
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
            existingRoadsReportsBAL = new ExistingRoadsReportsBAL();
            int blockCode = Convert.ToInt32(formCollection["BlockCode"]);
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];

            int totalRecords;

            var jsonData = new
            {
                rows = existingRoadsReportsBAL.ERR2FinalListingBAL(blockCode, districtCode, stateCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        #endregion


        #region ERR3 Reports       
        public ActionResult ERR3ReportDetails()
        {
            ExistingRoadsReportsViewModel existingRoadsReportsViewModel = new ExistingRoadsReportsViewModel();
            try
            {
                existingRoadsReportsViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                existingRoadsReportsViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;

                return View(existingRoadsReportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]      
        public ActionResult ERR3StateReportListing(FormCollection formCollection)
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
            existingRoadsReportsBAL = new ExistingRoadsReportsBAL();
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int totalRecords;

            var jsonData = new
            {
                rows = existingRoadsReportsBAL.ERR3StateReportListingBAL(page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]       
        public ActionResult ERR3DistrictReportListing(FormCollection formCollection)
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
            existingRoadsReportsBAL = new ExistingRoadsReportsBAL();
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int totalRecords = 0;

            var jsonData = new
            {
                rows = existingRoadsReportsBAL.ERR3DistrictReportListingBAL(stateCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]      
        public ActionResult ERR3BlockReportListing(FormCollection formCollection)
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
            existingRoadsReportsBAL = new ExistingRoadsReportsBAL();
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];

            int totalRecords;

            var jsonData = new
            {
                rows = existingRoadsReportsBAL.ERR3BlockReportListingBAL(stateCode, districtCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]       
        public ActionResult ERR3FinalReportListing(FormCollection formCollection)
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
            existingRoadsReportsBAL = new ExistingRoadsReportsBAL();
            int blockCode = Convert.ToInt32(formCollection["BlockCode"]);
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];

            int totalRecords;

            var jsonData = new
            {
                rows = existingRoadsReportsBAL.ERR3FinalListingBAL(blockCode, districtCode, stateCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        #endregion


        #region ERR4 Reports        
        public ActionResult ERR4ReportDetails()
        {
            ExistingRoadsReportsViewModel existingRoadsReportsViewModel = new ExistingRoadsReportsViewModel();
            try
            {
                existingRoadsReportsViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                existingRoadsReportsViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;

                return View(existingRoadsReportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]        
        public ActionResult ERR4StateReportListing(FormCollection formCollection)
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
            existingRoadsReportsBAL = new ExistingRoadsReportsBAL();
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int totalRecords;

            var jsonData = new
            {
                rows = existingRoadsReportsBAL.ERR4StateReportListingBAL(page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]       
        public ActionResult ERR4DistrictReportListing(FormCollection formCollection)
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
            existingRoadsReportsBAL = new ExistingRoadsReportsBAL();
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int totalRecords = 0;

            var jsonData = new
            {
                rows = existingRoadsReportsBAL.ERR4DistrictReportListingBAL(stateCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]        
        public ActionResult ERR4BlockReportListing(FormCollection formCollection)
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
            existingRoadsReportsBAL = new ExistingRoadsReportsBAL();
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];

            int totalRecords;

            var jsonData = new
            {
                rows = existingRoadsReportsBAL.ERR4BlockReportListingBAL(stateCode, districtCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]      
        public ActionResult ERR4FinalReportListing(FormCollection formCollection)
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
            existingRoadsReportsBAL = new ExistingRoadsReportsBAL();
            int blockCode = Convert.ToInt32(formCollection["BlockCode"]);
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];

            int totalRecords;

            var jsonData = new
            {
                rows = existingRoadsReportsBAL.ERR4FinalListingBAL(blockCode, districtCode, stateCode, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        #endregion


        #region ERR5 Reports       
        public ActionResult ERR5ReportDetails()
        {
            ExistingRoadsReportsViewModel existingRoadsReportsViewModel = new ExistingRoadsReportsViewModel();
            try
            {
                existingRoadsReportsViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                existingRoadsReportsViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                SelectListItem all = new SelectListItem
                {
                    Selected = true,
                    Text = "All",
                    Value = "0"
                };
                SelectListItem cbr1 = new SelectListItem
                {
                    Text = "CBR Less than 3",
                    Value = "1"
                };

                SelectListItem cbr2 = new SelectListItem
                {
                    Text = "CBR 3 to 4.99",
                    Value = "2"
                };
                SelectListItem cbr3 = new SelectListItem
                {
                    Text = "CBR 5 to 9.99",
                    Value = "3"
                };
                SelectListItem cbr4 = new SelectListItem
                {
                    Text = "CBR with 10 and more",
                    Value = "4"
                };

                List<SelectListItem> cbr = new List<SelectListItem>();
                cbr.Add(all);
                cbr.Add(cbr1);
                cbr.Add(cbr2);
                cbr.Add(cbr3);
                cbr.Add(cbr4);
                ViewData["CBR"] = cbr;
                return View(existingRoadsReportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]        
        public ActionResult ERR5StateReportListing(FormCollection formCollection)
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
            existingRoadsReportsBAL = new ExistingRoadsReportsBAL();
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int cbrValue = Convert.ToInt32(formCollection["CBRValue"]);
            int totalRecords;

            var jsonData = new
            {
                rows = existingRoadsReportsBAL.ERR5StateReportListingBAL(page, rows, sidx, sord, out totalRecords,cbrValue),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]       
        public ActionResult ERR5DistrictReportListing(FormCollection formCollection)
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
            existingRoadsReportsBAL = new ExistingRoadsReportsBAL();
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int totalRecords = 0;
            int cbrValue = Convert.ToInt32(formCollection["CBRValue"]);
            var jsonData = new
            {
                rows = existingRoadsReportsBAL.ERR5DistrictReportListingBAL(stateCode, page, rows, sidx, sord, out totalRecords,cbrValue),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]       
        public ActionResult ERR5BlockReportListing(FormCollection formCollection)
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
            existingRoadsReportsBAL = new ExistingRoadsReportsBAL();
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int cbrValue = Convert.ToInt32(formCollection["CBRValue"]);
            int totalRecords;

            var jsonData = new
            {
                rows = existingRoadsReportsBAL.ERR5BlockReportListingBAL(stateCode, districtCode, page, rows, sidx, sord, out totalRecords,cbrValue),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]       
        public ActionResult ERR5FinalReportListing(FormCollection formCollection)
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
            existingRoadsReportsBAL = new ExistingRoadsReportsBAL();
            int blockCode = Convert.ToInt32(formCollection["BlockCode"]);
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int cbrValue = Convert.ToInt32(formCollection["CBRValue"]);
            int totalRecords;

            var jsonData = new
            {
                rows = existingRoadsReportsBAL.ERR5FinalListingBAL(blockCode, districtCode, stateCode, page, rows, sidx, sord, out totalRecords,cbrValue),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        #endregion

        #region ERR6 Reports
       
        public ActionResult ERR6ReportDetails()
        {
            ExistingRoadsReportsViewModel existingRoadsReportsViewModel = new ExistingRoadsReportsViewModel();
            try
            {
                existingRoadsReportsViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                existingRoadsReportsViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                SelectListItem all = new SelectListItem
                {
                    Text = "All",
                    Value = "%"
                };
                SelectListItem allweather = new SelectListItem
                {
                    Text = "All Weather",
                    Value = "A"
                };
                SelectListItem fairweather = new SelectListItem
                {
                    Text = "Fair Weather",
                    Value = "F"
                };
                List<SelectListItem> ddroadtype = new List<SelectListItem>();
                ddroadtype.Add(all);
                ddroadtype.Add(allweather);
                ddroadtype.Add(fairweather);

                ViewData["RoadType"] = ddroadtype;
          
                return View(existingRoadsReportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]       
        public ActionResult ERR6StateReportListing(FormCollection formCollection)
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
            existingRoadsReportsBAL = new ExistingRoadsReportsBAL();
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            string roadType = formCollection["RoadType"];
            int totalRecords;

            var jsonData = new
            {
                rows = existingRoadsReportsBAL.ERR6StateReportListingBAL(page, rows, sidx, sord, out totalRecords,roadType),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]      
        public ActionResult ERR6DistrictReportListing(FormCollection formCollection)
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
            existingRoadsReportsBAL = new ExistingRoadsReportsBAL();
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            string roadType = formCollection["RoadType"];
            int totalRecords = 0;

            var jsonData = new
            {
                rows = existingRoadsReportsBAL.ERR6DistrictReportListingBAL(stateCode, page, rows, sidx, sord, out totalRecords,roadType),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]       
        public ActionResult ERR6BlockReportListing(FormCollection formCollection)
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
            existingRoadsReportsBAL = new ExistingRoadsReportsBAL();
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            string roadType = formCollection["RoadType"];

            int totalRecords;

            var jsonData = new
            {
                rows = existingRoadsReportsBAL.ERR6BlockReportListingBAL(stateCode, districtCode, page, rows, sidx, sord, out totalRecords,roadType),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]      
        public ActionResult ERR6FinalReportListing(FormCollection formCollection)
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
            existingRoadsReportsBAL = new ExistingRoadsReportsBAL();
            int blockCode = Convert.ToInt32(formCollection["BlockCode"]);
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            string roadType = formCollection["RoadType"];

            int totalRecords;

            var jsonData = new
            {
                rows = existingRoadsReportsBAL.ERR6FinalListingBAL(blockCode, districtCode, stateCode, page, rows, sidx, sord, out totalRecords,roadType),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        #endregion

        #region ERR7 Reports      
        public ActionResult ERR7ReportDetails()
        {
            ExistingRoadsReportsViewModel existingRoadsReportsViewModel = new ExistingRoadsReportsViewModel();
            ExistingRoadsReportsDAL objDAL = new ExistingRoadsReportsDAL();
     
            try
            {
                existingRoadsReportsViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                existingRoadsReportsViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                List<SelectListItem> ddTerrainType = objDAL.GetTerrainTypeList();
                ViewData["TerrainType"] = ddTerrainType;
          
                return View(existingRoadsReportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]       
        public ActionResult ERR7StateReportListing(FormCollection formCollection)
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
            existingRoadsReportsBAL = new ExistingRoadsReportsBAL();
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int totalRecords;
            int terrainType = Convert.ToInt32(formCollection["TerrainType"]);
            var jsonData = new
            {
                rows = existingRoadsReportsBAL.ERR7StateReportListingBAL(terrainType, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]       
        public ActionResult ERR7DistrictReportListing(FormCollection formCollection)
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
            existingRoadsReportsBAL = new ExistingRoadsReportsBAL();
            int terrainType = Convert.ToInt32(formCollection["TerrainType"]);            
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int totalRecords = 0;

            var jsonData = new
            {
                rows = existingRoadsReportsBAL.ERR7DistrictReportListingBAL(stateCode,terrainType, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]        
        public ActionResult ERR7BlockReportListing(FormCollection formCollection)
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
            existingRoadsReportsBAL = new ExistingRoadsReportsBAL();
            int terrainType = Convert.ToInt32(formCollection["TerrainType"]);
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];

            int totalRecords;

            var jsonData = new
            {
                rows = existingRoadsReportsBAL.ERR7BlockReportListingBAL(stateCode, districtCode,terrainType, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]       
        public ActionResult ERR7FinalReportListing(FormCollection formCollection)
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
            existingRoadsReportsBAL = new ExistingRoadsReportsBAL();
            int terrainType = Convert.ToInt32(formCollection["TerrainType"]);
            int blockCode = Convert.ToInt32(formCollection["BlockCode"]);
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];

            int totalRecords;

            var jsonData = new
            {
                rows = existingRoadsReportsBAL.ERR7FinalListingBAL(blockCode, districtCode, stateCode, terrainType, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        #endregion


        #region ERR8 Reports      
        public ActionResult ERR8ReportDetails()
        {
            ExistingRoadsReportsViewModel existingRoadsReportsViewModel = new ExistingRoadsReportsViewModel();
            ExistingRoadsReportsDAL objDAL = new ExistingRoadsReportsDAL();
            try
            {
                existingRoadsReportsViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                existingRoadsReportsViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                List<SelectListItem> ddSoilType = objDAL.GetSoilTypeList();
                ViewData["SoilType"] =ddSoilType;
        
                return View(existingRoadsReportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]       
        public ActionResult ERR8StateReportListing(FormCollection formCollection)
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
            existingRoadsReportsBAL = new ExistingRoadsReportsBAL();
            int soilType = Convert.ToInt32(formCollection["SoilType"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int totalRecords;

            var jsonData = new
            {
                rows = existingRoadsReportsBAL.ERR8StateReportListingBAL(soilType,page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]      
        public ActionResult ERR8DistrictReportListing(FormCollection formCollection)
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
            existingRoadsReportsBAL = new ExistingRoadsReportsBAL();
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int soilType = Convert.ToInt32(formCollection["SoilType"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int totalRecords = 0;

            var jsonData = new
            {
                rows = existingRoadsReportsBAL.ERR8DistrictReportListingBAL(stateCode,soilType, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]       
        public ActionResult ERR8BlockReportListing(FormCollection formCollection)
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
            existingRoadsReportsBAL = new ExistingRoadsReportsBAL();
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
            int soilType = Convert.ToInt32(formCollection["SoilType"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];

            int totalRecords;

            var jsonData = new
            {
                rows = existingRoadsReportsBAL.ERR8BlockReportListingBAL(stateCode, districtCode,soilType, page, rows, sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]      
        public ActionResult ERR8FinalReportListing(FormCollection formCollection)
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
            existingRoadsReportsBAL = new ExistingRoadsReportsBAL();
            int blockCode = Convert.ToInt32(formCollection["BlockCode"]);
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int districtCode = Convert.ToInt32(formCollection["DistrictCode"]);
            int soilType = Convert.ToInt32(formCollection["SoilType"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];

            int totalRecords;

            var jsonData = new
            {
                rows = existingRoadsReportsBAL.ERR8FinalListingBAL(blockCode, districtCode, stateCode,soilType, page, rows, sidx, sord, out totalRecords),
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
