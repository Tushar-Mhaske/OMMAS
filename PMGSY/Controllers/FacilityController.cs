using PMGSY.Common;
using PMGSY.DAL.Facility;
using PMGSY.Extensions;
using PMGSY.Models.Facility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Controllers
{
    public class FacilityController : Controller
    {
        //
        // GET: /Facility/

        FacilityDAL objDAL = new FacilityDAL();

        #region Finalize Facility PMGSY3 BLOCK/DISTRICT
        [Audit]
        public ActionResult FinalizeFacilityPMGSY3Layout()
        {
            FinalizeFacilityPMGSY3ViewModel model = new FinalizeFacilityPMGSY3ViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                if (PMGSYSession.Current.StateCode > 0)
                {
                    model.lstDistricts = comm.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                    model.lstDistricts.Find(x => x.Value == "-1").Text = "Select District";
                }
                else
                {
                    model.lstState = comm.PopulateStates(true);
                    model.lstDistricts = new List<SelectListItem>();
                    model.lstDistricts.Insert(0, new SelectListItem() { Text = "Select District", Value = "-1" });
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Facility.FinalizeFacilityPMGSY3Layout()");
                return null;
            }
        }
        [HttpGet]
        [Audit]
        public ActionResult GetBlockListPMGSY3(int? page, int? rows, string sidx, string sord)
        {
            String searchParameters = String.Empty;
            long totalRecords;

            int districtCode = 0;
            bool isAllBlockFinalized = false;
            bool isDistrictDefinalized = false;
            try
            {
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

                var jsonData = new
                {
                    rows = objDAL.GetBlockListPMGSY3DAL(districtCode, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords, Request.Params["filters"]
                            , ref isAllBlockFinalized, ref isDistrictDefinalized),
                    total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                    page = Convert.ToInt32(Request.Params["page"]),
                    records = totalRecords,
                    isAllBlockFinalized = isAllBlockFinalized,
                    isDistrictDefinalized = isDistrictDefinalized
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Facility.GetExistingRoadsPMGSY3List()");
                return null;
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Audit]
        public JsonResult FinalizeFacilityBlock(String parameter, String hash, String key)
        {
            string message = string.Empty;
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                int blockCode = Convert.ToInt32(decryptedParameters["BlockCode"]);

                if (decryptedParameters.Count() > 0)
                {
                    bool status = objDAL.FinalizeFacilityBlockPMGSY3DAL(blockCode, ref message);
                    return Json(new { success = status, message = message.Trim() });
                }
                else
                {
                    return Json(new { success = false, message = "Invalid request" });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Facility.FinalizeFacilityBlock()");
                return Json(new { success = false, message = "Error occurred while processing the request." });
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Audit]
        public JsonResult FinalizeFacilityDistrict(int districtCode)
        {
            string message = string.Empty;
            try
            {
                bool status = objDAL.FinalizeFacilityDistrictPMGSY3DAL(districtCode, ref message);
                return Json(new { success = status, message = message.Trim() });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Facility.FinalizeFacilityBlock()");
                return Json(new { success = false, message = "Error occurred while processing the request." });
            }
        }
        #endregion



        #region Definalize Facility PMGSY3 BLOCK/DISTRICT

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Audit]
        public JsonResult DefinalizeFacilityDistrict(int districtCode)
        {
            string message = string.Empty;
            try
            {
                bool status = objDAL.DefinalizeFacilityDistrictPMGSY3DAL(districtCode, ref message);
                return Json(new { success = status, message = message.Trim() });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Facility.DefinalizeFacilityDistrict()");
                return Json(new { success = false, message = "Error occurred while processing the request." });
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Audit]
        public JsonResult DefinalizeFacilityBlock(String parameter, String hash, String key)
        {
            string message = string.Empty;
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                int blockCode = Convert.ToInt32(decryptedParameters["BlockCode"]);

                if (decryptedParameters.Count() > 0)
                {
                    bool status = objDAL.DefinalizeFacilityBlockPMGSY3DAL(blockCode, ref message);
                    return Json(new { success = status, message = message.Trim() });
                }
                else
                {
                    return Json(new { success = false, message = "Invalid request" });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Facility.DefinalizeFacilityBlock()");
                return Json(new { success = false, message = "Error occurred while processing the request." });
            }
        }

        #endregion

    }
}
