#region HEADER
/*
* Project Id:

 * Project Name:OMMAS-II

 * File Name: ExistingRoadsController.cs

 * Author : Abhishek Kamble (changes done by Vikram Nandanwar)

 * Creation Date :30/May/2013

 * Desc : This class is used as controller  to perform Save,Edit,Update,Delete and listing of Existing Roads screens.  
 */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using PMGSY.BAL.ExistingRoads;
using PMGSY.Models;
using PMGSY.Models.ExistingRoads;
using System.Data.Entity;
using PMGSY.Common;
using PMGSY.Extensions;
using PMGSY.DAL.ExistingRoads;
using System.Data.Entity.Validation;
using System.Configuration;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;

namespace PMGSY.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class ExistingRoadsController : Controller
    {

        public ExistingRoadsController()
        {
            PMGSYSession.Current.ModuleName = "Existing Roads";
        }

        private PMGSYEntities db;

        private IExistingRoadasBAL objBAL = new ExistingRoadsBAL();
        ExistingRoadsDAL objDAL = new ExistingRoadsDAL();
        string message = String.Empty;

        #region Existing Roads Action
        [Audit]
        public ActionResult Index()
        {
            return View();
        }


        /// <summary>
        /// ListExistingRoads() Actions Shows Filter Bar and Existing Road List Grid
        /// </summary> 
        /// <returns></returns>
        [Audit]
        public ActionResult ListExistingRoads()
        {
            try
            {
                IExistingRoadsDAL objDAL = new ExistingRoadsDAL();
                List<SelectListItem> lstDefault = new List<SelectListItem>();
                CommonFunctions objCommon = new CommonFunctions();
                lstDefault.Insert(0, new SelectListItem { Value = "0", Text = "All" });
                List<MASTER_BLOCK> blockList = objDAL.GetAllBlockNames(PMGSYSession.Current.DistrictCode);
                if (PMGSYSession.Current.RoleCode == 25 || PMGSYSession.Current.RoleCode == 53 || PMGSYSession.Current.RoleCode == 65)//Changes by SAMMED A. PATIL for mordviewuser
                {
                    ViewData["States"] = objCommon.PopulateStates(true);
                    ViewData["Districts"] = lstDefault;
                    ViewData["blockList"] = lstDefault;
                }
                else if (PMGSYSession.Current.RoleCode == 2)
                {
                    ViewData["Districts"] = objCommon.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                    ViewData["blockList"] = lstDefault;
                }
                else
                {
                    ViewData["blockList"] = new SelectList(blockList, "MAST_BLOCK_CODE", "MAST_BLOCK_NAME");
                }

                List<MASTER_ROAD_CATEGORY> roadCategoryList = objDAL.GetAllRoadCategory();
                roadCategoryList.Insert(0, new MASTER_ROAD_CATEGORY() { MAST_ROAD_CAT_CODE = 0, MAST_ROAD_CAT_NAME = "-- All --" });

                ViewData["roadCategoryList"] = new SelectList(roadCategoryList, "MAST_ROAD_CAT_CODE", "MAST_ROAD_CAT_NAME");

                if (blockList.Count > 0)
                {
                    ViewData["isUnlocked"] = objDAL.CheckUnlockedDAL(blockList.Select(x => x.MAST_BLOCK_CODE).First());

                    ///PMGSY3
                    ExistingRoadsDAL objDRRPDAL = new ExistingRoadsDAL();
                    ViewData["isPMGSY3"] = objDRRPDAL.CheckPMGSY3DAL(PMGSYSession.Current.StateCode);
                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "ListExistingRoads()");
                ViewData["blockList"] = null;
                ViewData["roadCategoryList"] = null;
            }
            return View();
        }

        /// <summary>
        /// GetRoadShortName() Action is used to display Road Discription on text box through Ajax Call
        /// </summary>
        /// <param name="roadCategoryCode"></param>
        /// <returns>return Exisiting Road Short Discription Name </returns>
        [HttpPost]
        [Audit]
        public ActionResult CheckLockStatus()
        {
            try
            {
                int blockCode = Convert.ToInt32(Request.Params["blockCode"]);
                if (blockCode > 0)
                {
                    IExistingRoadsDAL objDAL = new ExistingRoadsDAL();

                    bool flag = objDAL.CheckUnlockedDAL(blockCode);

                    return Json(new { status = flag }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = false }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "CheckUnlocked()");
                return null;
            }
        }

        /// <summary>
        /// GetExistingRoadsList() this actions displays Existing Road
        /// </summary>
        /// <param name="homeFormCollection"> Parameters are 1)Block Code 2)Category Code</param>
        /// <returns>return json data to display on grid</returns>
        /// 
        [HttpPost]
        [Audit]
        public ActionResult GetExistingRoadsList(int? page, int? rows, string sidx, string sord)
        {
            String searchParameters = String.Empty;
            long totalRecords;

            int blockCode = 0;
            int categoryCode = 0;
            int ownerCode = 0;
            string roadName = string.Empty;
            int stateCode = 0;
            int districtCode = 0;


            try
            {

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["stateCode"]))
                {
                    stateCode = Convert.ToInt32(Request.Params["stateCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["districtCode"]))
                {
                    districtCode = Convert.ToInt32(Request.Params["districtCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["MAST_BLOCK_CODE"]))
                {
                    blockCode = Convert.ToInt32(Request.Params["MAST_BLOCK_CODE"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["MAST_ROAD_CAT_CODE"]))
                {
                    categoryCode = Convert.ToInt32(Request.Params["MAST_ROAD_CAT_CODE"]);
                }

                var jsonData = new
                {
                    rows = objBAL.ListExistingRoads(stateCode, districtCode, blockCode, categoryCode, ownerCode, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords, Request.Params["filters"]),
                    total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                    page = Convert.ToInt32(Request.Params["page"]),
                    records = totalRecords
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
        /// AddEditExistingRoads() Actions shows Existing Road Data entry form
        /// </summary>
        /// <param name="id"> Id parameter contains BlockCode used to get Block name</param>
        /// <returns> Returns partial view Of Existing Road Data Entry Form </returns>
        [Audit]
        public ActionResult AddEditExistingRoads(string id)
        {
            //int MAST_BLOCK_CODE = Convert.ToInt32(id);
            int stateCode = 0, districtCode = 0, MAST_BLOCK_CODE = 0;

            string[] locationcodes = id.Split('$');

            stateCode = locationcodes[0] == "undefined" ? 0 : Convert.ToInt32(locationcodes[0]);
            districtCode = locationcodes[1] == "undefined" ? 0 : Convert.ToInt32(locationcodes[1]);
            MAST_BLOCK_CODE = Convert.ToInt32(locationcodes[2]);

            ExistingRoadsViewModel ExistingRoadViewModel = new ExistingRoadsViewModel();
            try
            {

                ExistingRoadViewModel.MAST_STATE_CODE = stateCode > 0 ? stateCode : PMGSYSession.Current.StateCode;
                ExistingRoadViewModel.MAST_DISTRICT_CODE = stateCode > 0 ? districtCode : PMGSYSession.Current.DistrictCode;
                ExistingRoadViewModel.BlockName = objDAL.GetBlockName(MAST_BLOCK_CODE);
                ExistingRoadViewModel.MAST_BLOCK_CODE = MAST_BLOCK_CODE;

                List<MASTER_ROAD_CATEGORY> roadCategoriList = new List<Models.MASTER_ROAD_CATEGORY>();
                ///Changed by SAMMED A. PATIL for provision to update Road Category upwards
                IExistingRoadsDAL objERDAL = new ExistingRoadsDAL();
                roadCategoriList = objERDAL.GetAllRoadCategory();
                roadCategoriList.Insert(0, new MASTER_ROAD_CATEGORY() { MAST_ROAD_CAT_CODE = 0, MAST_ROAD_CAT_NAME = "-- Select Road Category --" });
                ExistingRoadViewModel.RoadCategory = new SelectList(roadCategoriList, "MAST_ROAD_CAT_CODE", "MAST_ROAD_CAT_NAME");

                ///PMGSY3
                if (PMGSYSession.Current.PMGSYScheme == 4)
                {
                    ExistingRoadViewModel.MAST_CORE_NETWORK = "N";
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                ModelState.AddModelError("", ex.Message);
                return PartialView("AddEditExistingRoads", new ExistingRoadsViewModel());
            }
            return PartialView("AddEditExistingRoads", ExistingRoadViewModel);
        }

        /// <summary>
        /// AddExistingRoads() Action is used to Save Existing Road Data into database
        /// </summary>
        /// <param name="ExistingRoadsViewModel"> ExistingRoadsViewModel Contains All Entered Existing Road Form Data</param>
        /// <returns>If data is successfully Saved then return success message else return error message </returns>
        [HttpPost]
        [Audit]
        public ActionResult AddExistingRoads(ExistingRoadsViewModel ExistingRoadsViewModel)
        {
            bool status = false;
            try
            {
                if (ModelState.IsValid)
                {

                    ExistingRoadsViewModel.MAST_CD_WORKS_NUM = null;
                    ExistingRoadsViewModel.MAST_LOCK_STATUS = "N";

                    if (objBAL.AddExistingRoads(ExistingRoadsViewModel, ref message))
                    {
                        message = message == string.Empty ? "Existing Road details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Existing Road details not saved successfully." : message;
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
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
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Existing Road details not saved successfully because ";
                message = message + ex.Message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// EditExistingRoads() this Action is used to fill Existing Road Form for editing
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"> Parameters,hash and key contails Encrypted Existing Road Code</param>
        /// <returns> returns Existing Road view model</returns>
        [HttpGet]
        [Audit]
        public ActionResult EditExistingRoads(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;

            try
            {
                db = new PMGSYEntities();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                int roadCode = Convert.ToInt32(decryptedParameters["RoadCode"]);

                if (decryptedParameters.Count() > 0)
                {
                    ExistingRoadsViewModel existingRoadsViewMode = objBAL.GetExistingRoads_ByRoadCode(roadCode);

                    if (decryptedParameters.Count() > 1)
                    {
                        existingRoadsViewMode.LockUnlockFlag = decryptedParameters["UnlockFlag"];
                    }
                    ///Changed by SAMMED A. PATIL for provision to update Road Category upwards
                    existingRoadsViewMode.RoadCategory = objDAL.PopulateRoadCategoriesforEditDAL(existingRoadsViewMode.hdnRoadCategoryCode);

                    //surface and CBR details exist or not check for Start and end chainage

                    //if (db.MASTER_ER_SURFACE_TYPES.Where(m => m.MAST_ER_ROAD_CODE == roadCode).Any() || db.MASTER_ER_CBR_VALUE.Where(m => m.MAST_ER_ROAD_CODE == roadCode).Any())
                    //{
                    //    existingRoadsViewMode.isSurfaceCbrDetails = 1;
                    //}
                    //else
                    //{
                    //    existingRoadsViewMode.isSurfaceCbrDetails = 0;
                    //}

                    if (existingRoadsViewMode == null)
                    {
                        ModelState.AddModelError(string.Empty, "Existing Road details not Exist.");
                        return PartialView("AddEditExistingRoads", new ExistingRoadsViewModel());
                    }

                    return PartialView("AddEditExistingRoads", existingRoadsViewMode);

                }
                return PartialView("AddEditExistingRoads", new ExistingRoadsViewModel());

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "Existing Road details not Exist.");
                return PartialView("AddEditExistingRoads", new ExistingRoadsViewModel());
            }
            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// EditExistingRoads() Action is used to Save Existing Road Data into database
        /// </summary>
        /// <param name="ExistingRoadsViewModel"> ExistingRoadsViewModel Contains All Entered Existing Road Form Data</param>
        /// <returns>If data is successfully Saved then return success message else return error message </returns>
        [HttpPost]
        [Audit]
        public ActionResult EditExistingRoads(ExistingRoadsViewModel existingRoadViewModel)
        {
            db = new PMGSYEntities();
            //for benifited hab start            
            string[] encryptedParameters = null;
            Dictionary<string, string> decryptedParameters = null;
            encryptedParameters = existingRoadViewModel.EncryptedRoadCode.Split('/');

            decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

            int ExistingRoadCode = Convert.ToInt32(decryptedParameters["RoadCode"].ToString());

            //Added By Abhishek kamble 7-feb-2014 start
            decimal? CN_CRRoadLength = 0;
            decimal TotalERRoadLength = 0;

            //Added By Abhishke kamble 12-feb-2014
            //surface and CBR details exist or not check for Start and end chainage
            if (db.MASTER_ER_SURFACE_TYPES.Where(m => m.MAST_ER_ROAD_CODE == ExistingRoadCode).Any() || db.MASTER_ER_CBR_VALUE.Where(m => m.MAST_ER_ROAD_CODE == ExistingRoadCode).Any())
            {
                MASTER_EXISTING_ROADS existingRoadModel = db.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == ExistingRoadCode).FirstOrDefault();
                if ((existingRoadModel.MAST_ER_ROAD_STR_CHAIN != existingRoadViewModel.MAST_ER_ROAD_STR_CHAIN) || (existingRoadViewModel.MAST_ER_ROAD_END_CHAIN != existingRoadViewModel.MAST_ER_ROAD_END_CHAIN))
                {
                    return Json(new { success = false, message = "Surface or CBR Details are entered so you can't change Start or End Chainage." }, JsonRequestBehavior.AllowGet);
                }
            }


            if (!(objDAL.IsExistingRoadIsMappedWithCN_CR(existingRoadViewModel, ExistingRoadCode, out CN_CRRoadLength, out TotalERRoadLength)))
            {
                if (PMGSYSession.Current.PMGSYScheme == 1)
                {
                    return Json(new { success = false, message = "Total Existing Road Length ('" + TotalERRoadLength + "') must be greater than or equal to Core Network Total Road Length. ('" + CN_CRRoadLength + "')" });
                }
                else
                {
                    return Json(new { success = false, message = "Total Existing Road Length ('" + TotalERRoadLength + "') must be greater than or equal to Total Candidate Road Length. ('" + CN_CRRoadLength + "')" });
                }
            }

            //Added By Abhishek kamble 7-feb-2014 end

            bool status = false;
            try
            {
                if (PMGSYSession.Current.PMGSYScheme == 2)
                {
                    ModelState.Remove("MAST_RENEW_YEAR");
                    ModelState.Remove("MAST_CONS_YEAR");
                }
                if (ModelState.IsValid)
                {
                    if (objBAL.EditExistingRoads(existingRoadViewModel, ref message))
                    {
                        message = message == string.Empty ? "Existing Road details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Existing Road details not updated successfully." : message;
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
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
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Existing Road details not updated successfully." : message;
                message = message + ex.Message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// ViewExistingRoads() this function displays details information of Perticular Existiong Road
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns>Return Existing Road View Model which contains Existing Road Information</returns>
        [HttpGet]
        [Audit]
        public ActionResult ViewExistingRoads(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                int roadCode = Convert.ToInt32(decryptedParameters["RoadCode"]);

                IExistingRoadsDAL objDAL = new ExistingRoadsDAL();

                if (decryptedParameters.Count() > 0)
                {
                    ExistingRoadsViewModel existingRoadsViewMode = objDAL.GetExistingRoads_ForViewDetails(roadCode);

                    if (existingRoadsViewMode == null)
                    {
                        ModelState.AddModelError(string.Empty, "Existing Road details not Exist.");
                        return PartialView("ViewExistingRoadsDetails", new ExistingRoadsViewModel());
                    }
                    return PartialView("ViewExistingRoadsDetails", existingRoadsViewMode);
                }
                return PartialView("ViewExistingRoadsDetails", new ExistingRoadsViewModel());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "Existing Road details not Exist.");
                return PartialView("ViewExistingRoadsDetails", new ExistingRoadsViewModel());
            }
        }

        /// <summary>
        /// GetCBRList() this Action is used to Display CBR Details On Grid
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns>Return json data for CBR Value Jqgrid</returns>
        [Audit]
        public JsonResult GetCBRList(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 29-Apr-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 29-Apr-2014 end

                int roadCode = Convert.ToInt32(Request["MAST_ER_ROAD_CODE"]);
                int totalRecords;
                var jsonData = new
                {
                    rows = objBAL.GetCBRListBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, roadCode),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        /// <summary>
        /// GetSurfaceTypeList() this Action is used to Display CBR Details On Grid
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns>Return json data for Surface Type Jqgrid</returns>
        [Audit]
        public JsonResult GetSurfaceTypeList(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                int roadCode = Convert.ToInt32(Request["MAST_ER_ROAD_CODE"]);
                int totalRecords;

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.GetSurfaceList(Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords, roadCode),
                    total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                    page = Convert.ToInt32(Request.Params["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        /// <summary>
        /// GetTrafficIntensityList() this Action is used to Display Traffic Intensity Details On Grid
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns>Return json data for Traffic Intensity Jqgrid</returns>
        [Audit]
        public JsonResult GetTrafficIntensityList(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                int roadCode = Convert.ToInt32(Request["MAST_ER_ROAD_CODE"]);
                int totalRecords;

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.GetTrafficListBAL(Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords, roadCode),
                    total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                    page = Convert.ToInt32(Request.Params["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }

        }

        /// <summary>
        /// GetCdWorksList() this Action is used to Display CdWorks Details On Grid
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns>Return json data for CdWorks Jqgrid</returns>
        [Audit]
        public JsonResult GetCdWorksList(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                int roadCode = Convert.ToInt32(Request["MAST_ER_ROAD_CODE"]);
                int totalRecords;

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.GetCdWorkList(Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords, roadCode),
                    total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                    page = Convert.ToInt32(Request.Params["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return null;
            }
        }

        /// <summary>
        /// GetHabitationList() this Action is used to Display Habitation Details On Grid
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns>Return json data for Habitation Jqgrid</returns>
        [Audit]
        public JsonResult GetHabitationList(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                int roadCode = Convert.ToInt32(Request["MAST_ER_ROAD_CODE"]);
                int totalRecords;

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.GetHabitationList(Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords, roadCode),
                    total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                    page = Convert.ToInt32(Request.Params["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return null;
            }
        }

        /// <summary>
        ///DeleteExistingRoads() this Action is used to delete Perticular Existing Road Details From database  
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns>this function return success message if ER Details successfully Deleted else shows error message</returns>
        [HttpPost]
        [Audit]
        public ActionResult DeleteExistingRoads(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            bool status = false;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                int roadCode = Convert.ToInt32(decryptedParameters["RoadCode"]);

                if (decryptedParameters.Count() > 0)
                {
                    if (objBAL.DeleteExistingRoads(roadCode, ref message))
                    {
                        message = "Existing Road details deleted successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Existing Road details can not be deleted because other details for road are entered." : message;
                    }
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

                }
                message = "There is an error occured while processing your request.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "There is an error occured while processing your request.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// GetRoadShortName() Action is used to display Road Discription on text box through Ajax Call
        /// </summary>
        /// <param name="roadCategoryCode"></param>
        /// <returns>return Exisiting Road Short Discription Name </returns>
        [HttpPost]
        [Audit]
        public ActionResult GetRoadShortName(string roadCategoryCode)
        {
            try
            {
                if (roadCategoryCode != "0")
                {
                    string roadShortName = "";

                    IExistingRoadsDAL objDAL = new ExistingRoadsDAL();

                    roadShortName = objDAL.GetRoadShortName(Convert.ToInt32(roadCategoryCode));

                    return Json(new { RoadShortName = roadShortName }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { RoadShortName = "" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        /// <summary>
        /// PopulateBroRoadOwner() this Action is used to Populate Dropdown of BRO Road Owner 
        /// </summary>
        /// <returns>Return BRO Road Owner</returns>
        [HttpPost]
        [Audit]
        public JsonResult PopulateBroRoadOwner()
        {
            try
            {
                ExistingRoadsDAL objDAL = new ExistingRoadsDAL();
                return Json(objDAL.PopulateBroRoadOwner(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        /// <summary>
        /// PopulateRoadOwner() this function is used to populate dropdown of All road owners except BRO Road Owner
        /// </summary>
        /// <returns></returns>
        [Audit]
        public JsonResult PopulateRoadOwner()
        {
            try
            {
                ExistingRoadsDAL objDAL = new ExistingRoadsDAL();
                return Json(objDAL.PopulateRoadOwners(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return null;
            }
        }

        [HttpPost]
        [Audit]
        public JsonResult GetExistingRoadChecks()
        {
            try
            {
                int MAST_ER_ROAD_CODE = Convert.ToInt32(Request.Params["MAST_ER_ROAD_CODE"]);
                string status = objBAL.GetExistingRoadCheckBAL(MAST_ER_ROAD_CODE);

                if (status == String.Empty)
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = status }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpPost]
        [Audit]
        public JsonResult FinalizeExistingRoad()
        {
            try
            {
                int MAST_ER_ROAD_CODE = Convert.ToInt32(Request.Params["MAST_ER_ROAD_CODE"]);
                string status = objBAL.FinalizeExistingRoad(MAST_ER_ROAD_CODE);

                if (status == string.Empty)
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = status });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        /// <summary>
        /// ViewExistingRoadDetails() this function is used to display AddEdit Existing Road Form
        /// On Cancel button action
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult ViewExistingRoadDetails(string id)
        {
            try
            {

                int MAST_ER_ROAD_CODE = Convert.ToInt32(id);

                IExistingRoadsDAL objDAL = new ExistingRoadsDAL();

                ExistingRoadsViewModel existingRoadsViewMode = objDAL.GetExistingRoads_ForViewDetails(MAST_ER_ROAD_CODE);

                if (existingRoadsViewMode == null)
                {
                    ModelState.AddModelError(string.Empty, "Existing Road details not Exist.");
                    return PartialView("ViewExistingRoadsDetails", new ExistingRoadsViewModel());
                }
                return PartialView("ViewExistingRoadsDetails", existingRoadsViewMode);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Existing Road details not Exist.");
                return PartialView("ViewExistingRoadsDetails", new ExistingRoadsViewModel());
            }
        }

        [Audit]
        public ActionResult ValidateCoreNetwork(string id)
        {
            String[] urlParams = id.Split('/');
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { urlParams[0], urlParams[1], urlParams[2] });
                bool status = objBAL.ValidateCoreNetwork(Convert.ToInt32(decryptedParameters["RoadCode"]));
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
                return Json(new { success = false });
            }
        }

        /// <summary>
        /// lists the core network associated with the DRRP
        /// </summary>
        /// <returns></returns>
        public ActionResult ListCoreNetworkByDRRP(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                int roadCode = Convert.ToInt32(Request["MAST_ER_ROAD_CODE"]);
                int totalRecords;

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objBAL.GetCoreNetworkList(Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords, roadCode),
                    total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                    page = Convert.ToInt32(Request.Params["page"]),
                    records = totalRecords
                };

                return Json(jsonData);
            }
            catch (Exception)
            {
                return null;
            }
        }



        #endregion Existing Roads Action

        #region Traffic Intensity Actions

        /// <summary>
        /// TrafficIntensity() this Action is used to Display Traffic Intensity Grid And Data Entry Form
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult TrafficIntensity(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            ExistingRoadsDAL objDAL = new ExistingRoadsDAL();

            TrafficViewModel trafficViewModel = new TrafficViewModel();

            trafficViewModel.Operation = "A";

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                int MAST_ER_ROAD_CODE = Convert.ToInt32(decryptedParameters["RoadCode"]);

                ViewData["MAST_IT_YEAR"] = new SelectList(objDAL.PopulateTrafficIntensityYears(MAST_ER_ROAD_CODE).AsEnumerable<SelectListItem>(), "Value", "Text", DateTime.Now.Year.ToString());

                if (decryptedParameters.Count() > 0)
                {
                    MASTER_EXISTING_ROADS masterExistingRoads = objDAL.GetRoadDetails(MAST_ER_ROAD_CODE);//db.MASTER_EXISTING_ROADS.Find(MAST_ER_ROAD_CODE);

                    if (masterExistingRoads == null)
                    {
                        ModelState.AddModelError(string.Empty, "Existing Roads details not Exist.");
                        return PartialView("TrafficIntensity", new TrafficViewModel());
                    }

                    trafficViewModel.MAST_ER_ROAD_CODE = masterExistingRoads.MAST_ER_ROAD_CODE;
                    trafficViewModel.RoadNumber = masterExistingRoads.MAST_ER_ROAD_NUMBER;
                    trafficViewModel.RoadName = masterExistingRoads.MAST_ER_ROAD_NAME;

                    return PartialView("TrafficIntensity", trafficViewModel);
                }
                return PartialView("TrafficIntensity", new TrafficViewModel());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "Existing Roads details not Exist.");
                return PartialView("TrafficIntensity", new TrafficViewModel());
            }
        }


        /// <summary>
        /// TrafficIntensityCancel() this Action shows Traffic intensity data entry form on Cancel button click
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult TrafficIntensityCancel(string id)
        {
            ExistingRoadsDAL objDAL = new ExistingRoadsDAL();

            TrafficViewModel trafficViewModel = new TrafficViewModel();

            trafficViewModel.Operation = "A";

            try
            {
                int MAST_ER_ROAD_CODE = Convert.ToInt32(id);

                ViewData["MAST_IT_YEAR"] = new SelectList(objDAL.PopulateTrafficIntensityYears(MAST_ER_ROAD_CODE).AsEnumerable<SelectListItem>(), "Value", "Text", DateTime.Now.Year.ToString());

                MASTER_EXISTING_ROADS masterExistingRoads = objDAL.GetRoadDetails(MAST_ER_ROAD_CODE);//db.MASTER_EXISTING_ROADS.Find(MAST_ER_ROAD_CODE);

                if (masterExistingRoads == null)
                {
                    ModelState.AddModelError(string.Empty, "Existing Roads details not Exist.");
                    return PartialView("TrafficIntensity", new TrafficViewModel());
                }

                trafficViewModel.MAST_ER_ROAD_CODE = masterExistingRoads.MAST_ER_ROAD_CODE;
                trafficViewModel.RoadNumber = masterExistingRoads.MAST_ER_ROAD_NUMBER;
                trafficViewModel.RoadName = masterExistingRoads.MAST_ER_ROAD_NAME;

                return PartialView("TrafficIntensity", trafficViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "Traffic Intensity details not Exist.");
                return PartialView("TrafficIntensity", new TrafficViewModel());
            }
        }

        /// <summary>
        /// AddTrafficIntensity() Action Save traffic intensity entered valid data into data base
        /// </summary>
        /// <param name="TrafficIntensityViewModel"> TrafficIntensityViewModel contatains traffic intensity form data</param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult AddTrafficIntensity(TrafficViewModel TrafficIntensityViewModel)
        {
            bool status = false;
            try
            {
                if (ModelState.IsValid)
                {
                    if (objBAL.AddTrafficIntensity(TrafficIntensityViewModel, ref message))
                    {
                        message = message == string.Empty ? "Traffic Intensity details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Traffic Intensity details not saved successfully." : message;
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Traffic Intensity details not saved successfully";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// EditTrafficIntensity() action open Traffic Intensity Data entry form in edit Mode
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult EditTrafficIntensity(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                int MAST_ER_ROAD_CODE = Convert.ToInt32(decryptedParameters["MAST_ER_ROAD_CODE"]);
                int MAST_TI_YEAR = Convert.ToInt32(decryptedParameters["MAST_IT_YEAR"]);

                if (decryptedParameters.Count() > 0)
                {
                    TrafficViewModel trafficIntensityViewMode = objBAL.GetTrafficIntensity_ByRoadCode(MAST_ER_ROAD_CODE, MAST_TI_YEAR);
                    trafficIntensityViewMode.Operation = "U";


                    if (trafficIntensityViewMode == null)
                    {
                        ModelState.AddModelError(string.Empty, "Traffic Intensity Dettails details not Exist.");

                        return PartialView("TrafficIntensity", new TrafficViewModel());
                    }

                    List<SelectListItem> SpecificYear = new List<SelectListItem>();
                    SelectListItem year = new SelectListItem();
                    year.Text = MAST_TI_YEAR.ToString() + "-" + (MAST_TI_YEAR + 1).ToString();
                    year.Value = MAST_TI_YEAR.ToString();
                    SpecificYear.Add(year);

                    ViewData["MAST_IT_YEAR"] = new SelectList(SpecificYear.AsEnumerable<SelectListItem>(), "Value", "Text", trafficIntensityViewMode.MAST_TI_YEAR);

                    return PartialView("TrafficIntensity", trafficIntensityViewMode);

                }
                return PartialView("TrafficIntensity", new TrafficViewModel());

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "Traffic Intensity details not Exist.");
                return PartialView("TrafficIntensity", new TrafficViewModel());
            }
        }

        /// <summary>
        /// EditTrafficIntensity() action is used to update traffic intensity details 
        /// </summary>
        /// <param name="TrafficViewModel"></param>
        /// <returns>Return success message if recored is successfully updated return success message else error message</returns>
        [HttpPost]
        [Audit]
        public ActionResult EditTrafficIntensity(TrafficViewModel TrafficViewModel)
        {
            bool status = false;
            try
            {
                if (ModelState.IsValid)
                {

                    if (objBAL.EditTrafficIntensity(TrafficViewModel, ref message))
                    {
                        message = message == string.Empty ? "Traffic Intensity details updated successfully." : message;
                        status = true;
                        ModelState.Clear();
                    }
                    else
                    {
                        message = message == string.Empty ? "Traffic Intensity details not updated successfully." : message;
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Traffic Intensity details not updated successfully." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// DeleteTrafficIntensity() action is used to delete Traffic intensity Details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns>Return success message if recored is successfully updated return success message else error message</returns>
        [HttpPost]
        [Audit]
        public ActionResult DeleteTrafficIntensity(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            bool status = false;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                int MAST_ER_ROAD_CODE = Convert.ToInt32(decryptedParameters["MAST_ER_ROAD_CODE"]);
                int MAST_TI_YEAR = Convert.ToInt32(decryptedParameters["MAST_IT_YEAR"]);

                if (decryptedParameters.Count() > 0)
                {
                    if (objBAL.DeleteTrafficIntensity(MAST_ER_ROAD_CODE, MAST_TI_YEAR, ref message))
                    {
                        message = "Trafffic Intensity details deleted successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Trafffic Intensity details not deleted successfully." : message;
                    }
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

                }
                message = "Trafffic Intensity details not deleted successfully.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Trafffic Intensity details not deleted successfully.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// PopulateTrafficIntensityYears() this action is used to Populate traffic intensity Year using Ajax Call
        /// </summary>
        /// <returns>Return Traffic intensity Year List</returns>
        [HttpPost]
        [Audit]
        public JsonResult PopulateTrafficIntensityYears()
        {
            try
            {
                ExistingRoadsDAL objDAL = new ExistingRoadsDAL();

                int MAST_ER_ROAD_CODE = Convert.ToInt32(Request.Params["MAST_ER_ROAD_CODE"]);

                return Json(objDAL.PopulateTrafficIntensityYears(MAST_ER_ROAD_CODE));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }


        #endregion Traffic Intensity Actions

        #region CBR Value Actions

        /// <summary>
        /// CBRlengthCalculation() actions is used to calculate Start Chainage, Total Segment length, Total Road Length
        /// </summary>
        /// <param name="MAST_ER_ROAD_CODE"></param>
        /// <returns> Return Start chainage,Total segment length,Total Road Length</returns>
        [Audit]
        public CBRViewModel CBRlengthCalculation(int MAST_ER_ROAD_CODE)
        {
            try
            {
                db = new PMGSYEntities();
                CBRViewModel CbrViewModel = new CBRViewModel();

                MASTER_EXISTING_ROADS existingRoadModel = objDAL.GetRoadDetails(MAST_ER_ROAD_CODE);//db.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).FirstOrDefault();


                //calculate start chainage
                MASTER_ER_CBR_VALUE CbrModel = objDAL.GetCBRDetails(MAST_ER_ROAD_CODE);//db.MASTER_ER_CBR_VALUE.Where(a => a.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE && a.MAST_SEGMENT_NO == (db.MASTER_ER_CBR_VALUE.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).Max(m => m.MAST_SEGMENT_NO))).FirstOrDefault();

                if (CbrModel == null)
                {
                    CbrViewModel.MAST_STR_CHAIN = existingRoadModel.MAST_ER_ROAD_STR_CHAIN;
                }
                else
                {
                    CbrViewModel.MAST_STR_CHAIN = CbrModel.MAST_END_CHAIN;
                }

                //cal total segment length
                CbrViewModel.TotalAvailableRoadLength = db.MASTER_ER_CBR_VALUE.Where(a => a.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).Count() > 0 ? db.MASTER_ER_CBR_VALUE.Where(a => a.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).Sum((m => m.MAST_END_CHAIN - m.MAST_STR_CHAIN)) : 0;

                //cal road length
                if (existingRoadModel != null)
                {
                    CbrViewModel.RoadLength = existingRoadModel.MAST_ER_ROAD_END_CHAIN - existingRoadModel.MAST_ER_ROAD_STR_CHAIN;
                }
                else
                {
                    CbrViewModel.RoadLength = 0;
                }

                if (CbrViewModel.TotalAvailableRoadLength == null)
                {
                    CbrViewModel.TotalAvailableRoadLength = CbrViewModel.RoadLength;
                }
                else
                {
                    CbrViewModel.TotalAvailableRoadLength = CbrViewModel.RoadLength - CbrViewModel.TotalAvailableRoadLength;
                }

                //total entered segment length

                if (db.MASTER_ER_CBR_VALUE.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).Any())
                {
                    CbrViewModel.EnteredSegmentLength = db.MASTER_ER_CBR_VALUE.Where(c => c.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).AsEnumerable().Sum(c => c.MAST_END_CHAIN - c.MAST_STR_CHAIN);
                }

                return CbrViewModel;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
            finally
            {

                db.Dispose();
            }
        }

        //ajax CBRLength Update
        /// <summary>
        /// CBRlengthCalculation() action is used to calculate Remainning Segment length
        /// </summary>
        /// <returns>Return Remaining Segment length</returns>
        [HttpPost]
        [Audit]
        public JsonResult CBRlengthCalculation()
        {
            try
            {
                int MAST_ER_ROAD_CODE = Convert.ToInt32(Request.Params["MAST_ER_ROAD_CODE"]);

                CBRViewModel lengthCalulation = CBRlengthCalculation(MAST_ER_ROAD_CODE);

                return Json(new { TotalAvailableLength = lengthCalulation.TotalAvailableRoadLength, startChainnage = lengthCalulation.MAST_STR_CHAIN, EnteredSegLength = lengthCalulation.EnteredSegmentLength }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        /// <summary>
        /// CBRAddEdit() action is used to display  CBR Details data entry form
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult CBRAddEdit(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;

            ExistingRoadsDAL objDAL = new ExistingRoadsDAL();

            CBRViewModel CbrViewModel = new CBRViewModel();

            CbrViewModel.Operation = "A";

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                //existing road grid value
                int MAST_ER_ROAD_CODE = Convert.ToInt32(decryptedParameters["RoadCode"]);

                if (decryptedParameters.Count() > 0)
                {
                    MASTER_EXISTING_ROADS masterExistingRoads = objDAL.GetRoadDetails(MAST_ER_ROAD_CODE);//db.MASTER_EXISTING_ROADS.Find(MAST_ER_ROAD_CODE);

                    if (masterExistingRoads == null)
                    {
                        ModelState.AddModelError(string.Empty, "Existing Roads details not Exist.");
                        return PartialView("CBRAddEdit", new CBRViewModel());
                    }
                    CbrViewModel.MAST_ER_ROAD_CODE = masterExistingRoads.MAST_ER_ROAD_CODE;
                    CbrViewModel.RoadID = masterExistingRoads.MAST_ER_ROAD_NUMBER;
                    CbrViewModel.RoadName = masterExistingRoads.MAST_ER_ROAD_NAME;

                    CBRViewModel lengthCalulation = CBRlengthCalculation(MAST_ER_ROAD_CODE);

                    CbrViewModel.RoadLength = lengthCalulation.RoadLength;
                    CbrViewModel.TotalAvailableRoadLength = lengthCalulation.TotalAvailableRoadLength;
                    CbrViewModel.EnteredSegmentLength = lengthCalulation.EnteredSegmentLength;
                    return PartialView("CBRAddEdit", CbrViewModel);
                }
                return PartialView("CBRAddEdit", new CBRViewModel());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "CBR details not Exist.");
                return PartialView("CBRAddEdit", new CBRViewModel());
            }
        }


        /// <summary>
        /// CBRCancel() this action is used to display CBR Data Entery form on cancel button click
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult CBRCancel(String id)
        {
            ExistingRoadsDAL objDAL = new ExistingRoadsDAL();

            CBRViewModel CbrViewModel = new CBRViewModel();

            CbrViewModel.Operation = "A";

            try
            {
                //existing road grid value
                int MAST_ER_ROAD_CODE = Convert.ToInt32(id);

                MASTER_EXISTING_ROADS masterExistingRoads = objDAL.GetRoadDetails(MAST_ER_ROAD_CODE);//db.MASTER_EXISTING_ROADS.Find(MAST_ER_ROAD_CODE);

                if (masterExistingRoads == null)
                {
                    ModelState.AddModelError(string.Empty, "Existing Roads details not Exist.");
                    return PartialView("CBRAddEdit", new CBRViewModel());
                }
                CbrViewModel.MAST_ER_ROAD_CODE = masterExistingRoads.MAST_ER_ROAD_CODE;
                CbrViewModel.RoadID = masterExistingRoads.MAST_ER_ROAD_NUMBER;
                CbrViewModel.RoadName = masterExistingRoads.MAST_ER_ROAD_NAME;

                CBRViewModel lengthCalulation = CBRlengthCalculation(MAST_ER_ROAD_CODE);

                CbrViewModel.RoadLength = lengthCalulation.RoadLength;
                CbrViewModel.TotalAvailableRoadLength = lengthCalulation.TotalAvailableRoadLength;

                return PartialView("CBRAddEdit", CbrViewModel);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "CBR details not Exist.");
                return PartialView("CBRAddEdit", new CBRViewModel());
            }
        }

        /// <summary>
        /// AddCBRDetails() action is used to Save CBR Details into the database
        /// </summary>
        /// <param name="CbrViewModel"></param>
        /// <returns>Return success message if recored is successfully saved else returns error message</returns>
        [HttpPost]
        [Audit]
        public ActionResult AddCBRDetails(CBRViewModel CbrViewModel)
        {
            bool status = false;
            try
            {

                if (ModelState.IsValid)
                {
                    if (objBAL.AddCbrValue(CbrViewModel, ref message))
                    {
                        message = message == string.Empty ? "CBR details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "CBR details not saved successfully." : message;
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "CBR details not saved successfully";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// EditCBRDetails() this action is used to display CBR Details Form in Edit Mode
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns>CBR View model which is used to fill CBR data entry form</returns>
        [HttpGet]
        [Audit]
        public ActionResult EditCBRDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                int MAST_ER_ROAD_CODE = Convert.ToInt32(decryptedParameters["MAST_ER_ROAD_CODE"]);
                int MAST_SEGMENT_NO = Convert.ToInt32(decryptedParameters["MAST_SEGMENT_NO"]);

                if (decryptedParameters.Count() > 0)
                {
                    CBRViewModel CbrViewMode = objBAL.GetCBRDetails(MAST_ER_ROAD_CODE, MAST_SEGMENT_NO);
                    CbrViewMode.Operation = "U";

                    if (CbrViewMode == null)
                    {
                        ModelState.AddModelError(string.Empty, "CBR Dettails details not Exist.");

                        return PartialView("CBRAddEdit", new CBRViewModel());
                    }

                    //cal total length

                    //plan model
                    MASTER_EXISTING_ROADS planRoadModel = objDAL.GetRoadDetails(MAST_ER_ROAD_CODE);//db.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).FirstOrDefault();

                    //cal road length
                    if (planRoadModel != null)
                    {
                        CbrViewMode.RoadLength = planRoadModel.MAST_ER_ROAD_END_CHAIN - planRoadModel.MAST_ER_ROAD_STR_CHAIN;
                    }
                    else
                    {
                        CbrViewMode.RoadLength = 0;
                    }

                    return PartialView("CBRAddEdit", CbrViewMode);

                }
                return PartialView("CBRAddEdit", new CBRViewModel());

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "CBR details not Exist.");
                return PartialView("CBRAddEdit", new CBRViewModel());
            }
        }

        /// <summary>
        /// ShowAddCBRDetails() action is used to display CBR Data entry form
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult ShowAddCBRDetails(String id)
        {
            try
            {
                string[] parameter = id.Split('$');

                int MAST_ER_ROAD_CODE = Convert.ToInt32(parameter[0]);
                int MAST_SEGMENT_NO = Convert.ToInt32(parameter[1]);

                CBRViewModel CbrViewMode = objBAL.GetCBRDetails(MAST_ER_ROAD_CODE, MAST_SEGMENT_NO);
                CbrViewMode.Operation = "A";

                if (CbrViewMode == null)
                {
                    ModelState.AddModelError(string.Empty, "CBR Dettails details not Exist.");

                    return PartialView("CBRAddEdit", new CBRViewModel());
                }

                //cal total length

                //plan model
                MASTER_EXISTING_ROADS planRoadModel = objDAL.GetRoadDetails(MAST_ER_ROAD_CODE);//db.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).FirstOrDefault();

                //cal road length
                if (planRoadModel != null)
                {
                    CbrViewMode.RoadLength = planRoadModel.MAST_ER_ROAD_END_CHAIN - planRoadModel.MAST_ER_ROAD_STR_CHAIN;
                }
                else
                {
                    CbrViewMode.RoadLength = 0;
                }

                CbrViewMode.MAST_END_CHAIN = 0;
                CbrViewMode.MAST_CBR_VALUE = 0;

                return PartialView("CBRAddEdit", CbrViewMode);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "CBR details not Exist.");
                return PartialView("CBRAddEdit", new CBRViewModel());
            }
        }


        /// <summary>
        /// EditCBRDetails() this action is used to update CBR Details
        /// </summary>
        /// <param name="CbrViewModel"></param>
        /// <returns>Return success message if recored is successfully updated else returns error message</returns>
        [HttpPost]
        [Audit]
        public ActionResult EditCBRDetails(CBRViewModel CbrViewModel)
        {
            bool status = false;
            try
            {
                if (ModelState.IsValid)
                {

                    if (objBAL.EditCbrValue(CbrViewModel, ref message))
                    {
                        message = message == string.Empty ? "CBR details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "CBR details not updated successfully." : message;
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "CBR details not updated successfully." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// DeleteCBRDetails() action is used to Delete CBR details 
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns>Return success message if recored is successfully deleted else returns error message</returns>
        [HttpPost]
        [Audit]
        public ActionResult DeleteCBRDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            bool status = false;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                int MAST_ER_ROAD_CODE = Convert.ToInt32(decryptedParameters["MAST_ER_ROAD_CODE"]);
                int MAST_SEGMENT_NO = Convert.ToInt32(decryptedParameters["MAST_SEGMENT_NO"]);

                if (decryptedParameters.Count() > 0)
                {
                    if (objBAL.DeleteCbrValue(MAST_ER_ROAD_CODE, MAST_SEGMENT_NO, ref message))
                    {
                        message = "CBR details deleted successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "CBR details not deleted successfully." : message;
                    }
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

                }
                message = "CBR details not deleted successfully.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "CBR details not deleted successfully.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion CBR Value Actions Actions

        #region CdWorks Actions

        [Audit]
        public ActionResult ListCdWorks(string id)
        {
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                FilterViewModel model = new FilterViewModel();
                String[] urlparams = id.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { urlparams[0], urlparams[1], urlparams[2] });
                ViewBag.MAST_ER_ROAD_CODE = decryptedParameters["RoadCode"];
                MASTER_EXISTING_ROADS masterExistingRoads = objDAL.GetRoadDetails(Convert.ToInt32(decryptedParameters["RoadCode"]));
                ViewBag.RoadNo = masterExistingRoads.MAST_ER_ROAD_NUMBER;
                ViewBag.RoadName = masterExistingRoads.MAST_ER_ROAD_NAME;
                ViewData["RoadCode"] = id;
                model.EncryptedRoadCode = id;
                model.ExistingRoadCode = Convert.ToInt32(decryptedParameters["RoadCode"]);
                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }


        /// <summary>
        /// CdWorkAddEdit() this action is used to display CDWork Data entry form and CDwork Grid
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        //[HttpGet]
        [Audit]
        public ActionResult CdWorkAddEdit(string id)
        {
            Dictionary<string, string> decryptedParameters = null;

            ExistingRoadsDAL objDAL = new ExistingRoadsDAL();

            CdWorksViewModel CdWorksViewModel = new CdWorksViewModel();

            CdWorksViewModel.Operation = "A";

            try
            {
                String[] urlparams = id.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { urlparams[0], urlparams[1], urlparams[2] });

                //existing road grid value
                int MAST_ER_ROAD_CODE = Convert.ToInt32(decryptedParameters["RoadCode"]);

                if (decryptedParameters.Count() > 0)
                {
                    MASTER_EXISTING_ROADS masterExistingRoads = objDAL.GetRoadDetails(MAST_ER_ROAD_CODE);//db.MASTER_EXISTING_ROADS.Find(MAST_ER_ROAD_CODE);

                    if (masterExistingRoads == null)
                    {
                        ModelState.AddModelError(string.Empty, "Existing Roads details not Exist.");
                        return PartialView("CdWorkAddEdit", new CdWorksViewModel());
                    }

                    CdWorksViewModel.MAST_ER_ROAD_CODE = masterExistingRoads.MAST_ER_ROAD_CODE;
                    CdWorksViewModel.RoadNumber = masterExistingRoads.MAST_ER_ROAD_NUMBER;
                    CdWorksViewModel.RoadName = masterExistingRoads.MAST_ER_ROAD_NAME;
                    CdWorksViewModel.EncryptedCdWorksCode = URLEncrypt.EncryptParameters1(new string[] { "RoadCode=" + MAST_ER_ROAD_CODE.ToString().Trim() });
                    return PartialView("CdWorkAddEdit", CdWorksViewModel);
                }
                return PartialView("CdWorkAddEdit", new CdWorksViewModel());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "Cd Work details not Exist.");
                return PartialView("CdWorkAddEdit", new CdWorksViewModel());
            }
        }

        /// <summary>
        /// AddCdWorkDetails() action is used to save Cd Work Details into database
        /// </summary>
        /// <param name="CdWorksViewModel"></param>
        /// <returns>Return success message if recored is successfully saved else returns error message</returns>
        [HttpPost]
        [Audit]
        public ActionResult AddCdWorkDetails(CdWorksViewModel CdWorksViewModel)
        {
            bool status = false;
            try
            {

                if (ModelState.IsValid)
                {
                    if (objBAL.AddCDWorksDetails(CdWorksViewModel, ref message))
                    {
                        message = message == string.Empty ? "CD Works details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "CD Works  details not saved successfully." : message;
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "CD Works details not saved successfully";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// EditCdWorkDetails() action is used to open CdWorks data entry form in edit mode
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns>CdWorks View Model which is used to fill Cdworks data entry form</returns>
        [HttpGet]
        [Audit]
        public ActionResult EditCdWorkDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                int MAST_ER_ROAD_CODE = Convert.ToInt32(decryptedParameters["MAST_ER_ROAD_CODE"]);
                int MAST_CD_NUMBER = Convert.ToInt32(decryptedParameters["MAST_CD_NUMBER"]);

                if (decryptedParameters.Count() > 0)
                {
                    CdWorksViewModel CdWorksViewModel = objBAL.GetCDWorksDetails(MAST_ER_ROAD_CODE, MAST_CD_NUMBER);
                    CdWorksViewModel.Operation = "U";

                    if (CdWorksViewModel == null)
                    {
                        ModelState.AddModelError(string.Empty, "CD Works Details details not Exist.");

                        return PartialView("CdWorkAddEdit", new CdWorksViewModel());
                    }

                    return PartialView("CdWorkAddEdit", CdWorksViewModel);

                }
                return PartialView("CdWorkAddEdit", new CdWorksViewModel());

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "CBR details not Exist.");
                return PartialView("CBRAddEdit", new CdWorksViewModel());
            }
        }

        /// <summary>
        /// EditCdWorkDetails() action is used to update CdWorks Details
        /// </summary>
        /// <param name="CdWorksViewModel"></param>
        /// <returns>Return success message if recored is successfully updated else returns error message</returns>
        [HttpPost]
        [Audit]
        public ActionResult EditCdWorkDetails(CdWorksViewModel CdWorksViewModel)
        {
            bool status = false;
            try
            {
                if (ModelState.IsValid)
                {

                    if (objBAL.EditCDWorksDetails(CdWorksViewModel, ref message))
                    {
                        message = message == string.Empty ? "CD Works details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "CD Works details not updated successfully." : message;
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "CD Works details not updated successfully." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// DeleteCDWorksDetails() action is used to delete CdWork Details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns>Return success message if recored is successfully deleted else returns error message</returns>
        [HttpPost]
        [Audit]
        public ActionResult DeleteCDWorksDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            bool status = false;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                int MAST_ER_ROAD_CODE = Convert.ToInt32(decryptedParameters["MAST_ER_ROAD_CODE"]);
                int MAST_CD_NUMBER = Convert.ToInt32(decryptedParameters["MAST_CD_NUMBER"]);

                if (decryptedParameters.Count() > 0)
                {
                    if (objBAL.DeleteCDWorksDetails(MAST_ER_ROAD_CODE, MAST_CD_NUMBER, ref message))
                    {
                        message = "CD Works details deleted successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "CD Works details not deleted successfully." : message;
                    }
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

                }
                message = "CD Works details not deleted successfully.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "CD Works details not deleted successfully.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion CdWorks Actions

        #region Surface Actions

        [Audit]
        public ActionResult ListSurfaceTypes(string id)
        {
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                String[] urlparams = id.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { urlparams[0], urlparams[1], urlparams[2] });
                FilterViewModel model = new FilterViewModel();
                model.EncryptedRoadCode = id.ToString().Trim();
                model.ExistingRoadCode = Convert.ToInt32(decryptedParameters["RoadCode"]);
                MASTER_EXISTING_ROADS masterExistingRoads = objDAL.GetRoadDetails(Convert.ToInt32(decryptedParameters["RoadCode"]));
                ViewBag.RoadNo = masterExistingRoads.MAST_ER_ROAD_NUMBER;
                ViewBag.RoadName = masterExistingRoads.MAST_ER_ROAD_NAME;
                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }


        ///<summary>
        ///SurfaceAddEdit() action is used to display Surface Data entry form and Surface Type list Grid
        ///</summary>
        ///<returns></returns>
        [HttpGet]
        //public ActionResult SurfaceAddEdit(String parameter, String hash, String key)
        [Audit]
        public ActionResult SurfaceAddEdit(string id)
        {
            Dictionary<string, string> decryptedParameters = null;

            ExistingRoadsDAL objDAL = new ExistingRoadsDAL();

            SurfaceTypeViewModel SurfaceViewModel = new SurfaceTypeViewModel();

            SurfaceViewModel.Operation = "A";

            try
            {
                db = new PMGSYEntities();
                //decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                String[] urlparams = id.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { urlparams[0], urlparams[1], urlparams[2] });
                //existing road grid value
                int MAST_ER_ROAD_CODE = Convert.ToInt32(decryptedParameters["RoadCode"]);

                if (decryptedParameters.Count() > 0)
                {
                    MASTER_EXISTING_ROADS masterExistingRoads = objDAL.GetRoadDetails(MAST_ER_ROAD_CODE);//db.MASTER_EXISTING_ROADS.Find(MAST_ER_ROAD_CODE);

                    //set last entered start chainage as first to start chainage
                    MASTER_ER_SURFACE_TYPES SurfaceModel = db.MASTER_ER_SURFACE_TYPES.Where(a => a.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE && a.MAST_SURFACE_SEG_NO == (db.MASTER_ER_SURFACE_TYPES.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).Max(m => m.MAST_SURFACE_SEG_NO))).FirstOrDefault();

                    //cal total segment length

                    //set start chainage
                    if (SurfaceModel == null)
                    {
                        //double startChannage = 0.000;
                        SurfaceViewModel.MAST_ER_STR_CHAIN = masterExistingRoads.MAST_ER_ROAD_STR_CHAIN;
                    }
                    else
                    {
                        SurfaceViewModel.MAST_ER_STR_CHAIN = SurfaceModel.MAST_ER_END_CHAIN == null ? 0 : SurfaceModel.MAST_ER_END_CHAIN;
                    }

                    if (masterExistingRoads == null)
                    {
                        ModelState.AddModelError(string.Empty, "Existing Roads details not Exist.");
                        return PartialView("SurfaceAddEdit", new SurfaceTypeViewModel());
                    }
                    SurfaceViewModel.MAST_ER_ROAD_CODE = masterExistingRoads.MAST_ER_ROAD_CODE;
                    SurfaceViewModel.RoadNumber = masterExistingRoads.MAST_ER_ROAD_NUMBER;
                    SurfaceViewModel.RoadName = masterExistingRoads.MAST_ER_ROAD_NAME;

                    SurfaceViewModel.StartChainageOfRoad = masterExistingRoads.MAST_ER_ROAD_STR_CHAIN;
                    SurfaceViewModel.EndChainageOfRoad = masterExistingRoads.MAST_ER_ROAD_END_CHAIN;
                    SurfaceViewModel.SumOfAllSurfaceLength = masterExistingRoads.MAST_ER_ROAD_END_CHAIN - masterExistingRoads.MAST_ER_ROAD_STR_CHAIN;

                    //remaining length
                    if (db.MASTER_ER_SURFACE_TYPES.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).Any())
                    {
                        SurfaceViewModel.SurfaceLenghEntered = db.MASTER_ER_SURFACE_TYPES.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).Sum(m => m.MAST_ER_SURFACE_LENGTH);
                    }
                    SurfaceViewModel.Remaining_Length = SurfaceViewModel.SumOfAllSurfaceLength - SurfaceViewModel.SurfaceLenghEntered;

                    return PartialView("SurfaceAddEdit", SurfaceViewModel);

                }
                return PartialView("SurfaceAddEdit", new SurfaceTypeViewModel());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "Surface details not Exist.");
                return PartialView("SurfaceAddEdit", new SurfaceTypeViewModel());
            }
            finally
            {
                db.Dispose();
            }
        }

        ///<summary>
        ///SurfaceAddEdit() action is used to display Surface Data entry form after successfully recored get updated
        ///</summary>
        ///<returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult ShowAddSurfaceDetails(String id)
        {
            ExistingRoadsDAL objDAL = new ExistingRoadsDAL();

            SurfaceTypeViewModel SurfaceViewModel = new SurfaceTypeViewModel();

            SurfaceViewModel.Operation = "A";

            try
            {
                db = new PMGSYEntities();
                string[] parameter = id.Split('$');

                int MAST_ER_ROAD_CODE = Convert.ToInt32(parameter[0]);
                int MAST_SEGMENT_NO = Convert.ToInt32(parameter[1]);

                MASTER_EXISTING_ROADS masterExistingRoads = objDAL.GetRoadDetails(MAST_ER_ROAD_CODE);//db.MASTER_EXISTING_ROADS.Find(MAST_ER_ROAD_CODE);

                //set last entered start chainage as first to start chainage
                MASTER_ER_SURFACE_TYPES SurfaceModel = db.MASTER_ER_SURFACE_TYPES.Where(a => a.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE && a.MAST_SURFACE_SEG_NO == (db.MASTER_ER_SURFACE_TYPES.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).Max(m => m.MAST_SURFACE_SEG_NO))).FirstOrDefault();

                //cal total segment length

                //set start chainage
                if (SurfaceModel == null)
                {
                    double startChannage = 0.000;
                    SurfaceViewModel.MAST_ER_STR_CHAIN = (decimal)startChannage;
                }
                else
                {
                    SurfaceViewModel.MAST_ER_STR_CHAIN = SurfaceModel.MAST_ER_END_CHAIN == null ? 0 : SurfaceModel.MAST_ER_END_CHAIN;
                }

                if (masterExistingRoads == null)
                {
                    ModelState.AddModelError(string.Empty, "Existing Roads details not Exist.");
                    return PartialView("SurfaceAddEdit", new SurfaceTypeViewModel());
                }

                SurfaceViewModel.MAST_ER_ROAD_CODE = masterExistingRoads.MAST_ER_ROAD_CODE;
                SurfaceViewModel.RoadNumber = masterExistingRoads.MAST_ER_ROAD_NUMBER;
                SurfaceViewModel.RoadName = masterExistingRoads.MAST_ER_ROAD_NAME;

                SurfaceViewModel.StartChainageOfRoad = masterExistingRoads.MAST_ER_ROAD_STR_CHAIN;
                SurfaceViewModel.EndChainageOfRoad = masterExistingRoads.MAST_ER_ROAD_END_CHAIN;
                SurfaceViewModel.SumOfAllSurfaceLength = masterExistingRoads.MAST_ER_ROAD_END_CHAIN - masterExistingRoads.MAST_ER_ROAD_STR_CHAIN;

                //remaining length
                if (db.MASTER_ER_SURFACE_TYPES.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).Any())
                {
                    SurfaceViewModel.SurfaceLenghEntered = db.MASTER_ER_SURFACE_TYPES.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).Sum(m => m.MAST_ER_SURFACE_LENGTH);
                }
                SurfaceViewModel.Remaining_Length = SurfaceViewModel.SumOfAllSurfaceLength - SurfaceViewModel.SurfaceLenghEntered;


                return PartialView("SurfaceAddEdit", SurfaceViewModel);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "Surface details not Exist.");
                return PartialView("SurfaceAddEdit", new SurfaceTypeViewModel());
            }
            finally
            {
                db.Dispose();
            }
        }

        ///<summary>
        ///SurfaceCancel() action is used to display Surface Data entry form on cancel button click
        ///</summary>
        ///<returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult SurfaceCancel(string id)
        {
            ExistingRoadsDAL objDAL = new ExistingRoadsDAL();

            SurfaceTypeViewModel SurfaceViewModel = new SurfaceTypeViewModel();

            SurfaceViewModel.Operation = "A";

            try
            {
                db = new PMGSYEntities();
                //existing road grid value
                int MAST_ER_ROAD_CODE = Convert.ToInt32(id);

                MASTER_EXISTING_ROADS masterExistingRoads = objDAL.GetRoadDetails(MAST_ER_ROAD_CODE);//db.MASTER_EXISTING_ROADS.Find(MAST_ER_ROAD_CODE);

                //set last entered start chainage as first to start chainage
                MASTER_ER_SURFACE_TYPES SurfaceModel = db.MASTER_ER_SURFACE_TYPES.Where(a => a.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE && a.MAST_SURFACE_SEG_NO == (db.MASTER_ER_SURFACE_TYPES.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).Max(m => m.MAST_SURFACE_SEG_NO))).FirstOrDefault();

                //set start chainage
                if (SurfaceModel == null)
                {
                    double startChannage = 0.000;
                    SurfaceViewModel.MAST_ER_STR_CHAIN = (decimal)startChannage;
                }
                else
                {
                    SurfaceViewModel.MAST_ER_STR_CHAIN = SurfaceModel.MAST_ER_END_CHAIN == null ? 0 : SurfaceModel.MAST_ER_END_CHAIN;
                }

                if (masterExistingRoads == null)
                {
                    ModelState.AddModelError(string.Empty, "Existing Roads details not Exist.");
                    return PartialView("SurfaceAddEdit", new SurfaceTypeViewModel());
                }

                SurfaceViewModel.MAST_ER_ROAD_CODE = masterExistingRoads.MAST_ER_ROAD_CODE;
                SurfaceViewModel.RoadNumber = masterExistingRoads.MAST_ER_ROAD_NUMBER;
                SurfaceViewModel.RoadName = masterExistingRoads.MAST_ER_ROAD_NAME;

                SurfaceViewModel.StartChainageOfRoad = masterExistingRoads.MAST_ER_ROAD_STR_CHAIN;
                SurfaceViewModel.EndChainageOfRoad = masterExistingRoads.MAST_ER_ROAD_END_CHAIN;
                SurfaceViewModel.SumOfAllSurfaceLength = masterExistingRoads.MAST_ER_ROAD_END_CHAIN - masterExistingRoads.MAST_ER_ROAD_STR_CHAIN;

                //remaining length
                SurfaceViewModel.SurfaceLenghEntered = db.MASTER_ER_SURFACE_TYPES.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).Sum(m => m.MAST_ER_SURFACE_LENGTH);
                SurfaceViewModel.Remaining_Length = SurfaceViewModel.SumOfAllSurfaceLength - SurfaceViewModel.SurfaceLenghEntered;

                return PartialView("SurfaceAddEdit", SurfaceViewModel);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "Surface details not Exist.");
                return PartialView("SurfaceAddEdit", new SurfaceTypeViewModel());
            }
            finally
            {
                db.Dispose();
            }
        }


        ///<summary>
        ///AddSurfaceDetails() action is used to save Surface Details into the database
        ///</summary>
        ///<returns>Return success message if recored is successfully saved else returns error message</returns>
        [HttpPost]
        [Audit]
        public ActionResult AddSurfaceDetails(SurfaceTypeViewModel SurfaceViewModel)
        {
            bool status = false;

            try
            {
                db = new PMGSYEntities();
                if (ModelState.IsValid)
                {
                    if (objBAL.AddSurfaceDetails(SurfaceViewModel, ref message))
                    {
                        message = message == string.Empty ? "Surface details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Surface details not saved successfully." : message;
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }

                //  remaining length
                SurfaceViewModel.SurfaceLenghEntered = db.MASTER_ER_SURFACE_TYPES.Where(m => m.MAST_ER_ROAD_CODE == SurfaceViewModel.MAST_ER_ROAD_CODE).AsEnumerable().Sum(m => m.MAST_ER_SURFACE_LENGTH);
                SurfaceViewModel.Remaining_Length = db.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == SurfaceViewModel.MAST_ER_ROAD_CODE).Select(m => m.MAST_ER_ROAD_END_CHAIN - m.MAST_ER_ROAD_STR_CHAIN).First() - SurfaceViewModel.SurfaceLenghEntered;

                return Json(new { Success = true, RemainingLength = SurfaceViewModel.Remaining_Length, SurfaceLengthEntered = SurfaceViewModel.SurfaceLenghEntered, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Surface details not saved successfully";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                db.Dispose();
            }
        }

        ///<summary>
        ///EditSurfaceDetails() action is used to open Surface details form in edit mode
        ///</summary>
        ///<returns>Return Surface View Model which is used to fill Surface Details Form</returns>  
        [HttpGet]
        [Audit]
        public ActionResult EditSurfaceDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                db = new PMGSYEntities();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                int MAST_ER_ROAD_CODE = Convert.ToInt32(decryptedParameters["MAST_ER_ROAD_CODE"]);
                int MAST_SURFACE_SEG_NO = Convert.ToInt32(decryptedParameters["MAST_SURFACE_SEG_NO"]);

                if (decryptedParameters.Count() > 0)
                {
                    SurfaceTypeViewModel SurfaceViewModel = objBAL.GetSurfaceDetails(MAST_ER_ROAD_CODE, MAST_SURFACE_SEG_NO);
                    SurfaceViewModel.Operation = "U";


                    if (SurfaceViewModel == null)
                    {
                        ModelState.AddModelError(string.Empty, "Surface Dettails details not Exist.");

                        return PartialView("SurfaceAddEdit", new SurfaceTypeViewModel());
                    }

                    //remaining length
                    SurfaceViewModel.SurfaceLenghEntered = db.MASTER_ER_SURFACE_TYPES.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).Sum(m => m.MAST_ER_SURFACE_LENGTH);
                    SurfaceViewModel.Remaining_Length = SurfaceViewModel.SumOfAllSurfaceLength - SurfaceViewModel.SurfaceLenghEntered;

                    return PartialView("SurfaceAddEdit", SurfaceViewModel);

                }
                return PartialView("SurfaceAddEdit", new SurfaceTypeViewModel());

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "Surface details not Exist.");
                return PartialView("SurfaceAddEdit", new SurfaceTypeViewModel());
            }
            finally
            {
                db.Dispose();
            }
        }


        ///<summary>
        ///EditSurfaceDetails() action is used to Update Surface Details
        ///</summary>
        ///<returns>Return success message if recored is successfully updated else returns error message</returns>
        [HttpPost]
        [Audit]
        public ActionResult EditSurfaceDetails(SurfaceTypeViewModel SurfaceViewModel)
        {
            SurfaceViewModel.MAST_ER_END_CHAIN = SurfaceViewModel.EditModeEndChainage;

            bool status = false;
            try
            {
                if (ModelState.IsValid)
                {
                    if (objBAL.EditSurfaceDetails(SurfaceViewModel, ref message))
                    {
                        message = message == string.Empty ? "Surface details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Surface details not updated successfully." : message;
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Surface details not updated successfully." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        ///<summary>
        ///DeleteSurfaceDetails() action is used to Delete Surface Details
        ///</summary>
        ///<returns>Return success message if recored is successfully deleted else returns error message</returns>
        [HttpPost]
        [Audit]
        public ActionResult DeleteSurfaceDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            bool status = false;
            try
            {
                db = new PMGSYEntities();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                int MAST_ER_ROAD_CODE = Convert.ToInt32(decryptedParameters["MAST_ER_ROAD_CODE"]);
                int MAST_SURFACE_SEG_NO = Convert.ToInt32(decryptedParameters["MAST_SURFACE_SEG_NO"]);

                if (decryptedParameters.Count() > 0)
                {
                    if (objBAL.DeleteSurfaceDetails(MAST_ER_ROAD_CODE, MAST_SURFACE_SEG_NO, ref message))
                    {
                        message = "Surface details deleted successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Surface details not deleted successfully." : message;
                    }

                    //remaining length
                    SurfaceTypeViewModel SurfaceViewModel = new SurfaceTypeViewModel();

                    if (db.MASTER_ER_SURFACE_TYPES.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).Any())
                    {
                        SurfaceViewModel.SurfaceLenghEntered = db.MASTER_ER_SURFACE_TYPES.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).Sum(m => m.MAST_ER_SURFACE_LENGTH);
                    }
                    SurfaceViewModel.Remaining_Length = db.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).Select(m => m.MAST_ER_ROAD_END_CHAIN - m.MAST_ER_ROAD_STR_CHAIN).First() - SurfaceViewModel.SurfaceLenghEntered;

                    return Json(new { success = status, message = message, RemainingLength = SurfaceViewModel.Remaining_Length, SurfaceLengthEntered = SurfaceViewModel.SurfaceLenghEntered, }, JsonRequestBehavior.AllowGet);

                }
                message = "Surface details not deleted successfully.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Surface details not deleted successfully.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                db.Dispose();
            }
        }


        //ajax start chainage update
        /// <summary>
        /// SurfaceStartChainageUpdate () this action is used to set start chainage and update start chainage after                 recored is inserted 
        /// </summary>
        /// <returns>Return Road Start chainage </returns>
        [HttpPost]
        [Audit]
        public JsonResult SurfaceStartChainageUpdate()
        {
            try
            {
                int MAST_ER_ROAD_CODE = Convert.ToInt32(Request.Params["MAST_ER_ROAD_CODE"]);

                SurfaceTypeViewModel surfaceViewModel = SurfaceStartChainageCalculation(MAST_ER_ROAD_CODE);

                return Json(new { startChainage = surfaceViewModel.MAST_ER_STR_CHAIN }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return null;
            }
        }

        /// <summary>
        /// SurfaceStartChainageCalculation() this action is used to calcutate start chainage of Surface
        /// </summary>
        /// <param name="MAST_ER_ROAD_CODE"></param>
        /// <returns>Returns start chainage of the surface</returns>
        [Audit]
        public SurfaceTypeViewModel SurfaceStartChainageCalculation(int MAST_ER_ROAD_CODE)
        {
            try
            {
                db = new PMGSYEntities();

                MASTER_EXISTING_ROADS masterExistingRoads = db.MASTER_EXISTING_ROADS.Find(MAST_ER_ROAD_CODE);


                SurfaceTypeViewModel SurfaceViewModel = new SurfaceTypeViewModel();
                //set last entered start chainage as first to start chainage
                MASTER_ER_SURFACE_TYPES SurfaceModel = db.MASTER_ER_SURFACE_TYPES.Where(a => a.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE && a.MAST_SURFACE_SEG_NO == (db.MASTER_ER_SURFACE_TYPES.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).Max(m => m.MAST_SURFACE_SEG_NO))).FirstOrDefault();



                //set start chainage
                if (SurfaceModel == null)
                {
                    SurfaceViewModel.MAST_ER_STR_CHAIN = masterExistingRoads.MAST_ER_ROAD_STR_CHAIN;
                }
                else
                {
                    SurfaceViewModel.MAST_ER_STR_CHAIN = SurfaceModel.MAST_ER_END_CHAIN == null ? 0 : SurfaceModel.MAST_ER_END_CHAIN;
                }

                return SurfaceViewModel;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
            finally
            {
                db.Dispose();
            }
        }

        #endregion Surface Actions

        #region Mapped Habitation


        /// <summary>
        /// ListHabitations () actions is used to show Grids of Habitation List and Mapped Habitation list
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult ListHabitations(String parameter, String hash, String key)
        {
            try
            {
                db = new PMGSYEntities();
                CommonFunctions objCommon = new CommonFunctions();
                HabitationDetailsViewModel model = new HabitationDetailsViewModel();
                ExistingRoadsDAL objDAL = new ExistingRoadsDAL();
                Dictionary<string, string> decryptedParameters = null;
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                int RoadCode = Convert.ToInt32(decryptedParameters["RoadCode"]);
                
                model.EncryptedRoadCode = RoadCode.ToString();

                MASTER_EXISTING_ROADS existingRoadModel = objDAL.GetRoadDetails(RoadCode);//db.MASTER_EXISTING_ROADS.FirstOrDefault(m => m.MAST_ER_ROAD_CODE == RoadCode);

                if (existingRoadModel != null)
                {
                    model.RoadName = existingRoadModel.MAST_ER_ROAD_NAME;
                    model.RoadNumber = existingRoadModel.MAST_ER_ROAD_NUMBER;
                    model.IsBenifitedHabitation = existingRoadModel.MAST_IS_BENEFITTED_HAB;
                }

                ViewBag.Blocks = objCommon.PopulateBlocks(PMGSYSession.Current.DistrictCode == 0 ? existingRoadModel.MAST_DISTRICT_CODE : PMGSYSession.Current.DistrictCode, false);

                return PartialView("ListHabitations", model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// GetHabitationListToMap() actions is used to display grid Habitations to map
        /// </summary>
        /// <param name="mapCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetHabitationListToMap(FormCollection mapCollection)
        {
            try
            {
                int roadCode = 0;
                int blockCode = 0;
                long totalRecords = 0;

                if (!string.IsNullOrEmpty(mapCollection["habCode"]))
                {
                    roadCode = Convert.ToInt32(mapCollection["habCode"]);
                }

                if (!string.IsNullOrEmpty(mapCollection["blockCode"]))
                {
                    blockCode = Convert.ToInt32(mapCollection["blockCode"]);
                }

                var jsonData = new
                {
                    rows = objBAL.GetHabitationListToMap(roadCode, blockCode, Convert.ToInt32(mapCollection["page"]) - 1, Convert.ToInt32(mapCollection["rows"]), mapCollection["sidx"], mapCollection["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(mapCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(mapCollection["rows"]) + 1,
                    page = Convert.ToInt32(mapCollection["page"]),
                    records = totalRecords
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
        /// GetAllHabitationList() action is used to view all mapped habitations into view mode
        /// </summary>
        /// <param name="habitationCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetAllHabitationList(FormCollection habitationCollection)
        {
            try
            {
                int habCode = 0;
                long totalRecords = 0;

                if (!string.IsNullOrEmpty(habitationCollection["habCode"]))
                {
                    habCode = Convert.ToInt32(habitationCollection["habCode"]);
                }

                var jsonData = new
                {
                    rows = objBAL.GetAllHabitationList(habCode, Convert.ToInt32(habitationCollection["page"]) - 1, Convert.ToInt32(habitationCollection["rows"]), habitationCollection["sidx"], habitationCollection["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(habitationCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(habitationCollection["rows"]) + 1,
                    page = Convert.ToInt32(habitationCollection["page"]),
                    records = totalRecords
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
        /// MapHabitationsToExistingRoads() action is used to save mapped habitation into MAST_ER_Habitation_Road table
        /// </summary>
        /// <param name="mappedCollection"></param>
        /// <returns>Return success message if habitation is successfully mapped else returns error message</returns>
        [HttpPost]
        [Audit]
        public ActionResult MapHabitationsToExistingRoads(FormCollection mappedCollection)
        {
            bool status = false;
            string encryptedHabCodes = string.Empty;
            string roadName = string.Empty;

            try
            {
                encryptedHabCodes = mappedCollection["EncryptedHabCodes"];
                roadName = mappedCollection["EncryptedRoadCode"];


                if (objBAL.MapHabitationToRoad(encryptedHabCodes, roadName))
                {

                    message = "Habitations mapped successfully.";
                    status = true;
                }
                else
                {
                    message = "Habitations not mapped.";
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Habitations not mapped.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// DeleteMapHabitation() action is used to deleted mapped habitation
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult DeleteMapHabitation(String hash, String parameter, String key, string roadCode)
        {
            Dictionary<string, string> decryptedParameters = null;
            string message = string.Empty;

            int coreNetworkCode = Convert.ToInt32(roadCode);
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptedParameters.Count() > 0)
                {
                    if (!objBAL.DeleteMapHabitation(Convert.ToInt32(decryptedParameters["HabCode"].ToString()), coreNetworkCode, out message))
                    {
                        ModelState.AddModelError(String.Empty, "Habitation not deleted successfully.");

                        message = message == String.Empty ? "Habitation can not be deleted" : message;

                        return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { success = true, message = "Habitation deleted successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Habitation can not be deleted" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Audit]
        public ActionResult CheckMapHabitation(String parameter, String hash, String key)//string encryptedCode)
        {
            ExistingRoadsDAL objDAL = new ExistingRoadsDAL();
            string message = string.Empty;
            Dictionary<string, string> decryptedParameters = null;
            int roadCode = 0;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                roadCode = Convert.ToInt32(decryptedParameters["RoadCode"]);
                bool status = objDAL.CheckMapHabitation(roadCode, ref message);
                if (status)
                {
                    return Json(new { success = true, message = message });
                }
                else
                {
                    return Json(new { success = false });
                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = message });
            }
        }




        #endregion Mapped Habitation

        #region Map DRRP PMGSY 1 Roads
        /// <summary>
        /// MapDRRPPMGSY1() this function is used to display Existing DRRP Roads Form
        /// On Cancel button action
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[HttpGet]
        [Audit]
        public ActionResult MapDRRPLayout(String parameter, String hash, String key)
        {
            //String[] urlParams = id.Split('/');
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                //decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { urlParams[0], urlParams[1], urlParams[2] });
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                int blockCode = Convert.ToInt32(decryptedParameters["BlockCode"]);
                int drrpCode = Convert.ToInt32(decryptedParameters["RoadCode"]);
                int roadCategory = Convert.ToInt32(decryptedParameters["RoadCategory"]);
                ViewData["blockCode"] = blockCode;
                ViewData["RoadCode"] = drrpCode;
                ViewData["Roads"] = objDAL.GetDRRPPMGSY1RoadsToMap(blockCode, roadCategory);
                ViewData["isMapped"] = objDAL.checkIsRoadMapped(drrpCode);

                ViewData["EncryptedDRRPRoadCode"] = URLEncrypt.EncryptParameters1(new string[] { "BlockCode =" + blockCode.ToString().Trim(), "RoadCode =" + drrpCode.ToString().Trim(), "RoadCategory=" + roadCategory.ToString().Trim() });

                return PartialView("MapDRRPLayout");
            }
            catch (Exception ex)
            {
                //ModelState.AddModelError(string.Empty, "Existing Road details not Exist.");
                ErrorLog.LogError(ex, "MapDRRPLayout()");
                return PartialView("MapDRRPLayout");
            }
        }

        [Audit]
        public JsonResult GetRoadsbyBlockCode(int blockCode, int roadCategory)
        {
            try
            {
                //int blockCode = Convert.ToInt32(Request.Params["blockCode"]);
                return Json(objDAL.GetDRRPPMGSY1RoadsToMap(blockCode, roadCategory), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "GetRoadsbyBlockCode()");
                return null;
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Audit]
        public ActionResult MapDRRPPMGSY1Roads(string id)
        {
            int erRoadCode = 0;
            int erRoadCode1 = 0;
            string message = string.Empty;
            //bool status = false;
            //String[] urlParams = id.Split('/');
            //Dictionary<string, string> decryptedParameters = null;
            try
            {
                //decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { urlParams[0], urlParams[1], urlParams[2] });
                erRoadCode1 = Convert.ToInt32(id);
                erRoadCode = Convert.ToInt32(Request.Params["roadCode"]);
                if (erRoadCode > 0)
                {
                    if (objBAL.MapDRRPPMGSY1RoadsBAL(erRoadCode, erRoadCode1, ref message))
                    {
                        message = message == string.Empty ? "DRRP Road for PMGSY-I mapped successfully." : message;
                        return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { success = false, message = "Error occured on DRRP Mapping" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = "Please select a DRRP Road to Map" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                //ModelState.AddModelError(string.Empty, "Existing Road details not Exist.");
                ErrorLog.LogError(ex, "MapDRRPPMGSY1Roads()");
                return Json(new { success = false, message = "DRRP Road for PMGSY-I cannot be Mapped" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// GetHabitationListToMap() actions is used to display grid Habitations to map
        /// </summary>
        /// <param name="mapCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetMappedDRRPPmgsy1List(FormCollection mapCollection)
        {
            try
            {
                if (PMGSYSession.Current == null)
                {
                    Response.Redirect("/Login/Login");
                }

                int blockCode = 0;
                int erRoadCode = 0;
                long totalRecords = 0;
                if (!string.IsNullOrEmpty(mapCollection["blockCode"]))
                {
                    blockCode = Convert.ToInt32(mapCollection["blockCode"]);
                }
                if (!string.IsNullOrEmpty(mapCollection["blockCode"]))
                {
                    erRoadCode = Convert.ToInt32(mapCollection["RoadCode"]);
                }
                var jsonData = new
                {
                    rows = objBAL.GetMappedDRRPPmgsy1ListBAL(blockCode, erRoadCode, Convert.ToInt32(mapCollection["page"]) - 1, Convert.ToInt32(mapCollection["rows"]), mapCollection["sidx"], mapCollection["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(mapCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(mapCollection["rows"]) + 1,
                    page = Convert.ToInt32(mapCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "GetMappedDRRPPmgsy1List()");
                return null;
            }
        }

        /// <summary>
        ///DeleteExistingRoads() this Action is used to delete Perticular Existing Road Details From database  
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns>this function return success message if ER Details successfully Deleted else shows error message</returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Audit]
        public ActionResult UnMapExistingRoadPMGSY1(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            bool status = false;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                int roadCode = Convert.ToInt32(decryptedParameters["RoadCode"]);
                if (decryptedParameters.Count() > 0)
                {
                    if (objBAL.UnMapDRRPPMGSY1RoadsBAL(roadCode, ref message))
                    {
                        message = "Existing Road details unmapped successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Existing Road details can not be unmapped." : message;
                    }
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
                message = "There is an error occured while processing your request.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                message = "Error occured while processing your request.";
                ErrorLog.LogError(ex, "UnMapExistingRoadPMGSY1()");
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion Map DRRP PMGSY 1 Roads Ends

        #region PMGSY 3
        ///DRRP 
        [Audit]
        public ActionResult ListExistingRoadsPMGSY3()
        {
            try
            {
                ListDRRPPmgsy3ViewModel model = new ListDRRPPmgsy3ViewModel();
                ExistingRoadsDAL objDAL = new ExistingRoadsDAL();
                List<SelectListItem> lstDefault = new List<SelectListItem>();
                CommonFunctions objCommon = new CommonFunctions();

                lstDefault.Insert(0, new SelectListItem { Value = "0", Text = "All" });
                List<MASTER_BLOCK> blockList = objDAL.GetAllBlockNames(PMGSYSession.Current.DistrictCode);
                //if (PMGSYSession.Current.RoleCode == 25 || PMGSYSession.Current.RoleCode == 53 || PMGSYSession.Current.RoleCode == 65)//Changes by SAMMED A. PATIL for mordviewuser
                if (PMGSYSession.Current.StateCode == 0)
                {
                    model.lstStateCode = objCommon.PopulateStates(true);
                    model.lstDistrictCode = lstDefault;
                    model.lstBlockCode = lstDefault;
                }
                //else if (PMGSYSession.Current.RoleCode == 2)
                else if (PMGSYSession.Current.StateCode > 0 && PMGSYSession.Current.DistrictCode == 0)
                {
                    model.lstStateCode = new List<SelectListItem>();
                    model.lstStateCode.Insert(0, new SelectListItem() { Text = PMGSYSession.Current.StateName.Trim(), Value = PMGSYSession.Current.StateCode.ToString().Trim() });

                    model.lstDistrictCode = objCommon.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                    model.lstBlockCode = lstDefault;
                }
                else if (PMGSYSession.Current.DistrictCode > 0)
                {
                    model.lstStateCode = new List<SelectListItem>();
                    model.lstStateCode.Insert(0, new SelectListItem() { Text = PMGSYSession.Current.StateName.Trim(), Value = PMGSYSession.Current.StateCode.ToString().Trim() });

                    model.lstDistrictCode = new List<SelectListItem>();
                    model.lstDistrictCode.Insert(0, new SelectListItem() { Text = PMGSYSession.Current.DistrictName.Trim(), Value = PMGSYSession.Current.DistrictCode.ToString().Trim() });

                    model.lstBlockCode = new SelectList(blockList, "MAST_BLOCK_CODE", "MAST_BLOCK_NAME").ToList();
                }

                List<MASTER_ROAD_CATEGORY> CategoryList = objDAL.GetAllRoadCategory();
                CategoryList.Insert(0, new MASTER_ROAD_CATEGORY() { MAST_ROAD_CAT_CODE = 0, MAST_ROAD_CAT_NAME = "-- All --" });

                model.lstCategory = new SelectList(CategoryList, "MAST_ROAD_CAT_CODE", "MAST_ROAD_CAT_NAME").ToList();

                var selected = model.lstCategory.Where(x => x.Text == "-- All --").First();
                selected.Selected = true;

                //ViewData["roadCategoryList"] = new SelectList(roadCategoryList, "MAST_ROAD_CAT_CODE", "MAST_ROAD_CAT_NAME");

                if (blockList.Count > 0)
                {
                    model.isUnlocked = objDAL.CheckUnlockedPMGSY3DAL(blockList.Select(x => x.MAST_BLOCK_CODE).FirstOrDefault());
                    //Temp
                    //model.isUnlocked = true;
                }
                model.isPMGSY3 = objDAL.CheckPMGSY3DAL(PMGSYSession.Current.StateCode);

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ListExistingRoadsPMGSY3()");
                //ViewData["blockList"] = null;
                //ViewData["roadCategoryList"] = null;
                return null;
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult CheckLockStatusPMGSY3()
        {
            try
            {
                int blockCode = Convert.ToInt32(Request.Params["blockCode"]);
                if (blockCode > 0)
                {
                    ExistingRoadsDAL objDAL = new ExistingRoadsDAL();

                    bool flag = objDAL.CheckUnlockedPMGSY3DAL(blockCode);

                    return Json(new { status = flag }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = false }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "CheckUnlocked()");
                return null;
            }
        }


        [HttpGet]
        [Audit]
        public ActionResult GetExistingRoadsPMGSY3List(int? page, int? rows, string sidx, string sord)
        {
            String searchParameters = String.Empty;
            long totalRecords;

            int blockCode = 0;
            int categoryCode = 0;
            int ownerCode = 0;
            string roadName = string.Empty;
            int stateCode = 0;
            int districtCode = 0;

            try
            {

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["stateCode"]))
                {
                    stateCode = Convert.ToInt32(Request.Params["stateCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["districtCode"]))
                {
                    districtCode = Convert.ToInt32(Request.Params["districtCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["MAST_BLOCK_CODE"]))
                {
                    blockCode = Convert.ToInt32(Request.Params["MAST_BLOCK_CODE"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["MAST_ROAD_CAT_CODE"]))
                {
                    categoryCode = Convert.ToInt32(Request.Params["MAST_ROAD_CAT_CODE"]);
                }

                var jsonData = new
                {
                    rows = objDAL.ListExistingRoadsPMGSY3DAL(stateCode, districtCode, blockCode, categoryCode, ownerCode, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords, Request.Params["filters"]),
                    total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                    page = Convert.ToInt32(Request.Params["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetExistingRoadsPMGSY3List()");
                return null;
            }
        }

        [HttpGet]
        [Audit]
        public ActionResult ViewExistingRoadsPMGSY3(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                int roadCode = Convert.ToInt32(decryptedParameters["RoadCode"]);

                IExistingRoadsDAL objDAL = new ExistingRoadsDAL();

                if (decryptedParameters.Count() > 0)
                {
                    ExistingRoadsViewModel existingRoadsViewMode = objDAL.GetExistingRoads_ForViewDetails(roadCode);

                    if (existingRoadsViewMode == null)
                    {
                        ModelState.AddModelError(string.Empty, "Existing Road details not Exist.");
                        return PartialView(new ExistingRoadsViewModel());
                    }
                    return PartialView(existingRoadsViewMode);
                }
                return PartialView(new ExistingRoadsViewModel());
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ViewExistingRoadsPMGSY3()");
                ModelState.AddModelError(string.Empty, "Existing Road details not Exist.");
                return PartialView(new ExistingRoadsViewModel());
            }
        }

        [HttpGet]
        [Audit]
        public ActionResult FinalizeDRRPPMGSY3(string id)
        {
            IExistingRoadsDAL objDAL = new ExistingRoadsDAL();
            try
            {
                int roadCode = Convert.ToInt32(id.Trim());

                ExistingRoadsViewModel existingRoadsViewMode = objDAL.GetExistingRoads_ForViewDetails(roadCode);

                if (existingRoadsViewMode == null)
                {
                    ModelState.AddModelError(string.Empty, "Existing Road details not Exist.");
                    return PartialView(new ExistingRoadsViewModel());
                }
                return PartialView("ViewExistingRoadsPMGSY3", existingRoadsViewMode);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ViewExistingRoadsPMGSY3()");
                ModelState.AddModelError(string.Empty, "Existing Road details not Exist.");
                return PartialView(new ExistingRoadsViewModel());
            }
        }

        [HttpPost]
        [Audit]
        public JsonResult GetExistingRoadChecksPMGSY3()
        {
            try
            {
                int MAST_ER_ROAD_CODE = Convert.ToInt32(Request.Params["MAST_ER_ROAD_CODE"]);
                string status = objDAL.GetExistingRoadCheckPMGSY3DAL(MAST_ER_ROAD_CODE);

                if (status == String.Empty)
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = status }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetExistingRoadChecksPMGSY3()");
                return null;
            }
        }

        ///CD Works
        [Audit]
        public ActionResult ListCdWorksPMGSY3(string id)
        {
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                FilterViewModel model = new FilterViewModel();
                String[] urlparams = id.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { urlparams[0], urlparams[1], urlparams[2] });
                ViewBag.MAST_ER_ROAD_CODE = decryptedParameters["RoadCode"];
                MASTER_EXISTING_ROADS masterExistingRoads = objDAL.GetRoadDetails(Convert.ToInt32(decryptedParameters["RoadCode"]));
                ViewBag.RoadNo = masterExistingRoads.MAST_ER_ROAD_NUMBER;
                ViewBag.RoadName = masterExistingRoads.MAST_ER_ROAD_NAME;
                ViewData["RoadCode"] = id;
                model.EncryptedRoadCode = id;
                model.ExistingRoadCode = Convert.ToInt32(decryptedParameters["RoadCode"]);
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ListCdWorksPMGSY3()");
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        /// <summary>
        /// GetCdWorksList() this Action is used to Display CdWorks Details On Grid
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns>Return json data for CdWorks Jqgrid</returns>
        [Audit]
        public JsonResult GetCdWorksListPMGSY3(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                int roadCode = Convert.ToInt32(Request["MAST_ER_ROAD_CODE"]);
                int totalRecords;

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objDAL.GetCdWorkListPMGSY3DAL(Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords, roadCode),
                    total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                    page = Convert.ToInt32(Request.Params["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "GetCdWorksListPMGSY3()");
                return null;
            }
        }

        [Audit]
        public ActionResult CdWorkPMGSY3Layout(string id)
        {
            Dictionary<string, string> decryptedParameters = null;

            ExistingRoadsDAL objDAL = new ExistingRoadsDAL();

            CdWorksViewModel CdWorksViewModel = new CdWorksViewModel();

            CdWorksViewModel.Operation = "A";

            try
            {
                String[] urlparams = id.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { urlparams[0], urlparams[1], urlparams[2] });

                if (decryptedParameters.Count() == 1)
                {
                    //existing road grid value
                    int MAST_ER_ROAD_CODE = Convert.ToInt32(decryptedParameters["RoadCode"]);

                    MASTER_EXISTING_ROADS masterExistingRoads = objDAL.GetRoadDetails(MAST_ER_ROAD_CODE);//db.MASTER_EXISTING_ROADS.Find(MAST_ER_ROAD_CODE);

                    if (masterExistingRoads == null)
                    {
                        ModelState.AddModelError(string.Empty, "Existing Roads details not Exist.");
                        return PartialView(new CdWorksViewModel());
                    }

                    CdWorksViewModel.MAST_ER_ROAD_CODE = masterExistingRoads.MAST_ER_ROAD_CODE;
                    CdWorksViewModel.RoadNumber = masterExistingRoads.MAST_ER_ROAD_NUMBER;
                    CdWorksViewModel.RoadName = masterExistingRoads.MAST_ER_ROAD_NAME;
                    CdWorksViewModel.EncryptedCdWorksCode = URLEncrypt.EncryptParameters1(new string[] { "RoadCode=" + MAST_ER_ROAD_CODE.ToString().Trim() });
                    return PartialView(CdWorksViewModel);
                }
                else if (decryptedParameters.Count() == 2)
                {
                    //    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                    int MAST_ER_ROAD_CODE = Convert.ToInt32(decryptedParameters["MAST_ER_ROAD_CODE"]);
                    int MAST_CD_NUMBER = Convert.ToInt32(decryptedParameters["MAST_CD_NUMBER"]);

                    CdWorksViewModel = objDAL.GetCDWorksDetailsPMGSY3DAL(MAST_ER_ROAD_CODE, MAST_CD_NUMBER);
                    CdWorksViewModel.Operation = "U";

                    //CdWorksViewModel.MAST_ER_ROAD_CODE = masterExistingRoads.MAST_ER_ROAD_CODE;
                    //CdWorksViewModel.RoadNumber = masterExistingRoads.MAST_ER_ROAD_NUMBER;
                    //CdWorksViewModel.RoadName = masterExistingRoads.MAST_ER_ROAD_NAME;
                    CdWorksViewModel.EncryptedCdWorksCode = URLEncrypt.EncryptParameters1(new string[] { "RoadCode=" + MAST_ER_ROAD_CODE.ToString().Trim() });

                    if (CdWorksViewModel == null)
                    {
                        ModelState.AddModelError(string.Empty, "CD Works Details details not Exist.");

                        return PartialView(new CdWorksViewModel());
                    }

                    return PartialView(CdWorksViewModel);

                }

                return PartialView(new CdWorksViewModel());
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "CdWorkPMGSY3Layout()");
                ModelState.AddModelError(string.Empty, "Cd Work details not Exist.");
                return PartialView(new CdWorksViewModel());
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Audit]
        public ActionResult AddCdWorkDetailsPMGSY3(CdWorksViewModel CdWorksViewModel)
        {
            bool status = false;
            try
            {

                if (ModelState.IsValid)
                {
                    if (objDAL.AddCDWorksDetailsPMGSY3DAL(CdWorksViewModel, ref message))
                    {
                        message = message == string.Empty ? "CD Works details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "CD Works  details not saved successfully." : message;
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "AddCdWorkDetailsPMGSY3()");
                message = "CD Works details not saved successfully";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Audit]
        public ActionResult EditCdWorkDetailsPMGSY3(CdWorksViewModel CdWorksViewModel)
        {
            bool status = false;
            try
            {
                if (ModelState.IsValid)
                {
                    objDAL = new ExistingRoadsDAL();
                    if (objDAL.EditCDWorksDetailsPMGSY3DAL(CdWorksViewModel, ref message))
                    {
                        message = message == string.Empty ? "CD Works details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "CD Works details not updated successfully." : message;
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "EditCdWorkDetailsPMGSY3()");
                message = message == string.Empty ? "CD Works details not updated successfully." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// DeleteCDWorksDetails() action is used to delete CdWork Details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns>Return success message if recored is successfully deleted else returns error message</returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Audit]
        public ActionResult DeleteCDWorksDetailsPMGSY3(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            bool status = false;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                int MAST_ER_ROAD_CODE = Convert.ToInt32(decryptedParameters["MAST_ER_ROAD_CODE"]);
                int MAST_CD_NUMBER = Convert.ToInt32(decryptedParameters["MAST_CD_NUMBER"]);

                if (decryptedParameters.Count() > 0)
                {
                    objDAL = new ExistingRoadsDAL();
                    if (objDAL.DeleteCDWorksDetailsPMGSY3DAL(MAST_ER_ROAD_CODE, MAST_CD_NUMBER, ref message))
                    {
                        message = "CD Works details deleted successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "CD Works details not deleted successfully." : message;
                    }
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

                }
                message = "CD Works details not deleted successfully.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "DeleteCDWorksDetails()");
                message = "CD Works details not deleted successfully.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        ///Surface Types
        [Audit]
        public ActionResult ListSurfaceTypesPMGSY3(string id)
        {
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                String[] urlparams = id.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { urlparams[0], urlparams[1], urlparams[2] });
                FilterViewModel model = new FilterViewModel();
                model.EncryptedRoadCode = id.ToString().Trim();
                model.ExistingRoadCode = Convert.ToInt32(decryptedParameters["RoadCode"]);
                MASTER_EXISTING_ROADS masterExistingRoads = objDAL.GetRoadDetails(Convert.ToInt32(decryptedParameters["RoadCode"]));
                ViewBag.RoadNo = masterExistingRoads.MAST_ER_ROAD_NUMBER;
                ViewBag.RoadName = masterExistingRoads.MAST_ER_ROAD_NAME;
                return View(model);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "ListSurfaceTypes()");
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        [HttpGet]
        [Audit]
        public JsonResult GetSurfaceTypeListPMGSY3(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                int roadCode = Convert.ToInt32(Request["MAST_ER_ROAD_CODE"]);
                int totalRecords;

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objDAL.GetSurfaceListPMGSY3DAL(Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords, roadCode),
                    total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                    page = Convert.ToInt32(Request.Params["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "GetSurfaceTypeListPMGSY3()");
                return null;
            }
        }

        [HttpGet]
        [Audit]
        public ActionResult SurfacePMGSY3Layout(string id)
        {
            Dictionary<string, string> decryptedParameters = null;

            ExistingRoadsDAL objDAL = new ExistingRoadsDAL();

            SurfaceTypeViewModel SurfaceViewModel = new SurfaceTypeViewModel();

            SurfaceViewModel.Operation = "A";

            try
            {
                db = new PMGSYEntities();
                //decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                String[] urlparams = id.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { urlparams[0], urlparams[1], urlparams[2] });

                if (decryptedParameters.Count() == 1)
                {
                    //existing road grid value
                    int MAST_ER_ROAD_CODE = Convert.ToInt32(decryptedParameters["RoadCode"]);

                    MASTER_EXISTING_ROADS masterExistingRoads = objDAL.GetRoadDetails(MAST_ER_ROAD_CODE);//db.MASTER_EXISTING_ROADS.Find(MAST_ER_ROAD_CODE);

                    //set last entered start chainage as first to start chainage
                    MASTER_ER_SURFACE_TYPES_PMGSY3 SurfaceModel = db.MASTER_ER_SURFACE_TYPES_PMGSY3.Where(a => a.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE && a.MAST_SURFACE_SEG_NO == (db.MASTER_ER_SURFACE_TYPES_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).Max(m => m.MAST_SURFACE_SEG_NO))).FirstOrDefault();

                    //cal total segment length

                    //set start chainage
                    if (SurfaceModel == null)
                    {
                        //double startChannage = 0.000;
                        SurfaceViewModel.MAST_ER_STR_CHAIN = masterExistingRoads.MAST_ER_ROAD_STR_CHAIN;
                    }
                    else
                    {
                        SurfaceViewModel.MAST_ER_STR_CHAIN = SurfaceModel.MAST_ER_END_CHAIN == null ? 0 : SurfaceModel.MAST_ER_END_CHAIN;
                    }

                    if (masterExistingRoads == null)
                    {
                        ModelState.AddModelError(string.Empty, "Existing Roads details not Exist.");
                        return PartialView(new SurfaceTypeViewModel());
                    }
                    SurfaceViewModel.MAST_ER_ROAD_CODE = masterExistingRoads.MAST_ER_ROAD_CODE;
                    SurfaceViewModel.RoadNumber = masterExistingRoads.MAST_ER_ROAD_NUMBER;
                    SurfaceViewModel.RoadName = masterExistingRoads.MAST_ER_ROAD_NAME;

                    SurfaceViewModel.StartChainageOfRoad = masterExistingRoads.MAST_ER_ROAD_STR_CHAIN;
                    SurfaceViewModel.EndChainageOfRoad = masterExistingRoads.MAST_ER_ROAD_END_CHAIN;
                    SurfaceViewModel.SumOfAllSurfaceLength = masterExistingRoads.MAST_ER_ROAD_END_CHAIN - masterExistingRoads.MAST_ER_ROAD_STR_CHAIN;

                    //remaining length
                    if (db.MASTER_ER_SURFACE_TYPES_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).Any())
                    {
                        SurfaceViewModel.SurfaceLenghEntered = Convert.ToDecimal(db.MASTER_ER_SURFACE_TYPES_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).Sum(m => m.MAST_ER_SURFACE_LENGTH));
                    }
                    SurfaceViewModel.Remaining_Length = SurfaceViewModel.SumOfAllSurfaceLength - SurfaceViewModel.SurfaceLenghEntered;

                    return PartialView(SurfaceViewModel);

                }

                else if (decryptedParameters.Count() > 0)
                {
                    int MAST_ER_ROAD_CODE = Convert.ToInt32(decryptedParameters["MAST_ER_ROAD_CODE"]);
                    int MAST_SURFACE_SEG_NO = Convert.ToInt32(decryptedParameters["MAST_SURFACE_SEG_NO"]);

                    SurfaceViewModel = objDAL.GetSurfaceDetailsPMGSY3(MAST_ER_ROAD_CODE, MAST_SURFACE_SEG_NO);
                    SurfaceViewModel.Operation = "U";


                    if (SurfaceViewModel == null)
                    {
                        ModelState.AddModelError(string.Empty, "Surface Dettails details not Exist.");

                        return PartialView(new SurfaceTypeViewModel());
                    }



                    return PartialView(SurfaceViewModel);

                }
                return PartialView(new SurfaceTypeViewModel());
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "SurfacePMGSY3Layout()");
                ModelState.AddModelError(string.Empty, "Surface details not Exist.");
                return PartialView(new SurfaceTypeViewModel());
            }
            finally
            {
                db.Dispose();
            }
        }

        //ajax start chainage update
        /// <summary>
        /// SurfaceStartChainageUpdate () this action is used to set start chainage and update start chainage after                 recored is inserted 
        /// </summary>
        /// <returns>Return Road Start chainage </returns>
        [HttpPost]
        [Audit]
        public JsonResult SurfaceStartChainageUpdatePMGSY3()
        {
            try
            {
                int MAST_ER_ROAD_CODE = Convert.ToInt32(Request.Params["MAST_ER_ROAD_CODE"]);

                SurfaceTypeViewModel surfaceViewModel = objDAL.SurfaceStartChainageCalculationPMGSY3DAL(MAST_ER_ROAD_CODE);

                return Json(new { startChainage = surfaceViewModel.MAST_ER_STR_CHAIN }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "SurfaceStartChainageUpdatePMGSY3()");
                return null;
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Audit]
        public ActionResult AddSurfaceDetailsPMGSY3(SurfaceTypeViewModel SurfaceViewModel)
        {
            bool status = false;

            try
            {
                db = new PMGSYEntities();
                if (ModelState.IsValid)
                {
                    if (objDAL.AddSurfaceDetailsPMGSY3DAL(SurfaceViewModel, ref message))
                    {
                        message = message == string.Empty ? "Surface details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Surface details not saved successfully." : message;
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }

                //  remaining length
                SurfaceViewModel.SurfaceLenghEntered = Convert.ToDecimal(db.MASTER_ER_SURFACE_TYPES_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == SurfaceViewModel.MAST_ER_ROAD_CODE).AsEnumerable().Sum(m => m.MAST_ER_SURFACE_LENGTH));
                SurfaceViewModel.Remaining_Length = db.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == SurfaceViewModel.MAST_ER_ROAD_CODE).Select(m => m.MAST_ER_ROAD_END_CHAIN - m.MAST_ER_ROAD_STR_CHAIN).First() - SurfaceViewModel.SurfaceLenghEntered;

                return Json(new { Success = true, RemainingLength = SurfaceViewModel.Remaining_Length, SurfaceLengthEntered = SurfaceViewModel.SurfaceLenghEntered, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "AddSurfaceDetailsPMGSY3()");
                message = "Surface details not saved successfully";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                db.Dispose();
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Audit]
        public ActionResult EditSurfaceDetailsPMGSY3(SurfaceTypeViewModel SurfaceViewModel)
        {
            SurfaceViewModel.MAST_ER_END_CHAIN = SurfaceViewModel.EditModeEndChainage;

            bool status = false;
            try
            {
                if (ModelState.IsValid)
                {
                    if (objDAL.EditSurfaceDetailsPMGSY3DAL(SurfaceViewModel, ref message))
                    {
                        message = message == string.Empty ? "Surface details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Surface details not updated successfully." : message;
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "EditSurfaceDetailsPMGSY3()");
                message = message == string.Empty ? "Surface details not updated successfully." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Audit]
        public ActionResult DeleteSurfaceDetailsPMGSY3(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            bool status = false;
            try
            {
                db = new PMGSYEntities();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                int MAST_ER_ROAD_CODE = Convert.ToInt32(decryptedParameters["MAST_ER_ROAD_CODE"]);
                int MAST_SURFACE_SEG_NO = Convert.ToInt32(decryptedParameters["MAST_SURFACE_SEG_NO"]);

                if (decryptedParameters.Count() > 0)
                {
                    if (objDAL.DeleteSurfaceDetailsPMGSY3DAL(MAST_ER_ROAD_CODE, MAST_SURFACE_SEG_NO, ref message))
                    {
                        message = "Surface details deleted successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Surface details not deleted successfully." : message;
                    }

                    //remaining length
                    SurfaceTypeViewModel SurfaceViewModel = new SurfaceTypeViewModel();

                    if (db.MASTER_ER_SURFACE_TYPES_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).Any())
                    {
                        SurfaceViewModel.SurfaceLenghEntered = Convert.ToDecimal(db.MASTER_ER_SURFACE_TYPES_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).Sum(m => m.MAST_ER_SURFACE_LENGTH));
                    }
                    SurfaceViewModel.Remaining_Length = db.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).Select(m => m.MAST_ER_ROAD_END_CHAIN - m.MAST_ER_ROAD_STR_CHAIN).First() - SurfaceViewModel.SurfaceLenghEntered;

                    return Json(new { success = status, message = message, RemainingLength = SurfaceViewModel.Remaining_Length, SurfaceLengthEntered = SurfaceViewModel.SurfaceLenghEntered, }, JsonRequestBehavior.AllowGet);

                }
                message = "Surface details not deleted successfully.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "DeleteSurfaceDetailsPMGSY3()");
                message = "Surface details not deleted successfully.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                db.Dispose();
            }
        }

        ///Habitations
        [Audit]
        public ActionResult ListHabitationsPMGSY3(String parameter, String hash, String key)
        {
            try
            {
                //db = new PMGSYEntities();
                CommonFunctions objCommon = new CommonFunctions();
                HabitationDetailsPMGSY3ViewModel model = new HabitationDetailsPMGSY3ViewModel();
                ExistingRoadsDAL objDAL = new ExistingRoadsDAL();
                Dictionary<string, string> decryptedParameters = null;
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                int RoadCode = Convert.ToInt32(decryptedParameters["RoadCode"]);
                ViewBag.Blocks = objCommon.PopulateBlocks(PMGSYSession.Current.DistrictCode, false);
                model.EncryptedRoadCode = RoadCode.ToString();

                MASTER_EXISTING_ROADS existingRoadModel = objDAL.GetRoadDetails(RoadCode);//db.MASTER_EXISTING_ROADS.FirstOrDefault(m => m.MAST_ER_ROAD_CODE == RoadCode);

                if (existingRoadModel != null)
                {
                    model.RoadName = existingRoadModel.MAST_ER_ROAD_NAME;
                    model.RoadNumber = existingRoadModel.MAST_ER_ROAD_NUMBER;
                    model.IsBenifitedHabitation = existingRoadModel.MAST_IS_BENEFITTED_HAB;
                }
                //model.habDirect = "Y";
                return PartialView(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ListHabitationsPMGSY3()");
                return null;
            }
            finally
            {
                //db.Dispose();
            }
        }

        //[HttpGet]
        [Audit]
        public ActionResult GetHabitationListToMapPMGSY3(FormCollection mapCollection)
        {
            try
            {
                int roadCode = 0;
                int blockCode = 0;
                long totalRecords = 0;

                if (!string.IsNullOrEmpty(mapCollection["habCode"]))
                {
                    roadCode = Convert.ToInt32(mapCollection["habCode"]);
                }

                if (!string.IsNullOrEmpty(mapCollection["blockCode"]))
                {
                    blockCode = Convert.ToInt32(mapCollection["blockCode"]);
                }

                var jsonData = new
                {
                    rows = objDAL.GetHabitationListToMapPMGSY3DAL(roadCode, blockCode, Convert.ToInt32(mapCollection["page"]) - 1, Convert.ToInt32(mapCollection["rows"]), mapCollection["sidx"], mapCollection["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(mapCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(mapCollection["rows"]) + 1,
                    page = Convert.ToInt32(mapCollection["page"]),
                    records = totalRecords
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetHabitationListToMapPMGSY3()");
                return null;
            }
        }

        //[HttpGet]
        [Audit]
        public ActionResult GetAllHabitationListPMGSY3(FormCollection habitationCollection)
        {
            try
            {
                int habCode = 0;
                long totalRecords = 0;

                if (!string.IsNullOrEmpty(habitationCollection["habCode"]))
                {
                    habCode = Convert.ToInt32(habitationCollection["habCode"]);
                }

                var jsonData = new
                {
                    rows = objDAL.GetAllHabitationListPMGSY3DAL(habCode, Convert.ToInt32(habitationCollection["page"]) - 1, Convert.ToInt32(habitationCollection["rows"]), habitationCollection["sidx"], habitationCollection["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(habitationCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(habitationCollection["rows"]) + 1,
                    page = Convert.ToInt32(habitationCollection["page"]),
                    records = totalRecords
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetAllHabitationListPMGSY3()");
                return null;
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Audit]
        public ActionResult MapHabitationsToExistingRoadsPMGSY3(FormCollection mappedCollection)
        {
            bool status = false;
            string encryptedHabCodes = string.Empty;
            string roadName = string.Empty, habDirect = string.Empty;
            try
            {
                encryptedHabCodes = mappedCollection["EncryptedHabCodes"];
                roadName = mappedCollection["EncryptedRoadCode"];

                if (mappedCollection.AllKeys.Contains("habDirect"))
                {
                    habDirect = mappedCollection["habDirect"].Trim();
                }
                else
                {
                    return Json(new { success = false, message = "Please select Habitation Direct (Yes/No)" }, JsonRequestBehavior.AllowGet);
                }

                if (objDAL.MapHabitationToRoadPMGSY3DAL(encryptedHabCodes, roadName, habDirect))
                {

                    message = "Habitations mapped successfully.";
                    status = true;
                }
                else
                {
                    message = "Habitations not mapped.";
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MapHabitationsToExistingRoadsPMGSY3()");
                message = "Habitations not mapped.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Audit]
        public ActionResult DeleteMapHabitationPMGSY3(String hash, String parameter, String key, string roadCode)
        {
            Dictionary<string, string> decryptedParameters = null;
            string message = string.Empty;

            int coreNetworkCode = Convert.ToInt32(roadCode);
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptedParameters.Count() > 0)
                {
                    if (!objDAL.DeleteMapHabitationPMGSY3DAL(Convert.ToInt32(decryptedParameters["HabCode"].ToString()), coreNetworkCode, out message))
                    {
                        ModelState.AddModelError(String.Empty, "Habitation not deleted successfully.");

                        message = message == String.Empty ? "Habitation can not be deleted" : message;

                        return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { success = true, message = "Habitation deleted successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DeleteMapHabitationPMGSY3()");
                return Json(new { success = false, message = "Habitation can not be deleted" }, JsonRequestBehavior.AllowGet);
            }
        }

        ///Traffic Intensity
        [HttpGet]
        [Audit]
        public ActionResult ListTrafficIntensityPMGSY3(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            ExistingRoadsDAL objDAL = new ExistingRoadsDAL();

            TrafficViewModel trafficViewModel = new TrafficViewModel();
            trafficViewModel.Operation = "A";
            int MAST_ER_ROAD_CODE = 0, MAST_TI_YEAR = 0;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() == 1)
                {
                    MAST_ER_ROAD_CODE = Convert.ToInt32(decryptedParameters["RoadCode"]);
                    ViewData["MAST_IT_YEAR"] = new SelectList(objDAL.PopulateTrafficIntensityYearsPMGSY3DAL(MAST_ER_ROAD_CODE).AsEnumerable<SelectListItem>(), "Value", "Text", DateTime.Now.Year.ToString());

                    MASTER_EXISTING_ROADS masterExistingRoads = objDAL.GetRoadDetails(MAST_ER_ROAD_CODE);//db.MASTER_EXISTING_ROADS.Find(MAST_ER_ROAD_CODE);
                    if (masterExistingRoads == null)
                    {
                        ModelState.AddModelError(string.Empty, "Existing Roads details not Exist.");
                        return PartialView(new TrafficViewModel());
                    }

                    trafficViewModel.MAST_ER_ROAD_CODE = masterExistingRoads.MAST_ER_ROAD_CODE;
                    trafficViewModel.RoadNumber = masterExistingRoads.MAST_ER_ROAD_NUMBER;
                    trafficViewModel.RoadName = masterExistingRoads.MAST_ER_ROAD_NAME;

                    trafficViewModel.EncryptedErRoadCode = URLEncrypt.EncryptParameters1(new string[] { "RoadCode=" + MAST_ER_ROAD_CODE.ToString().Trim() });
                    return PartialView(trafficViewModel);
                }
                else
                {
                    //decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                    MAST_ER_ROAD_CODE = Convert.ToInt32(decryptedParameters["MAST_ER_ROAD_CODE"]);
                    MAST_TI_YEAR = Convert.ToInt32(decryptedParameters["MAST_IT_YEAR"]);

                    if (decryptedParameters.Count() > 0)
                    {
                        TrafficViewModel trafficIntensityViewMode = objDAL.GetTrafficIntensity_ByRoadCodePMGSY3DAL(MAST_ER_ROAD_CODE, MAST_TI_YEAR);
                        trafficIntensityViewMode.Operation = "U";

                        if (trafficIntensityViewMode == null)
                        {
                            ModelState.AddModelError(string.Empty, "Traffic Intensity Dettails details not Exist.");

                            return PartialView(new TrafficViewModel());
                        }

                        List<SelectListItem> SpecificYear = new List<SelectListItem>();
                        SelectListItem year = new SelectListItem();
                        year.Text = MAST_TI_YEAR.ToString() + "-" + (MAST_TI_YEAR + 1).ToString();
                        year.Value = MAST_TI_YEAR.ToString();
                        SpecificYear.Add(year);

                        ViewData["MAST_IT_YEAR"] = new SelectList(SpecificYear.AsEnumerable<SelectListItem>(), "Value", "Text", trafficIntensityViewMode.MAST_TI_YEAR);

                        trafficIntensityViewMode.EncryptedErRoadCode = URLEncrypt.EncryptParameters1(new string[] { "RoadCode=" + MAST_ER_ROAD_CODE.ToString().Trim() });
                        return PartialView(trafficIntensityViewMode);
                    }
                }
                return PartialView(new TrafficViewModel());
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ListTrafficIntensityPMGSY3()");
                ModelState.AddModelError(string.Empty, "Existing Roads details not Exist.");
                return PartialView(new TrafficViewModel());
            }
        }

        [Audit]
        public JsonResult GetTrafficIntensityListPMGSY3(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                int roadCode = Convert.ToInt32(Request["MAST_ER_ROAD_CODE"]);
                int totalRecords;

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                var jsonData = new
                {
                    rows = objDAL.GetTrafficListPMGSY3DAL(Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords, roadCode),
                    total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                    page = Convert.ToInt32(Request.Params["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetTrafficIntensityListPMGSY3");
                return null;
            }
        }

        [HttpPost]
        [Audit]
        public JsonResult PopulateTrafficIntensityYearsPMGSY3()
        {
            try
            {
                ExistingRoadsDAL objDAL = new ExistingRoadsDAL();

                int MAST_ER_ROAD_CODE = Convert.ToInt32(Request.Params["MAST_ER_ROAD_CODE"]);

                return Json(objDAL.PopulateTrafficIntensityYearsPMGSY3DAL(MAST_ER_ROAD_CODE));
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PopulateTrafficIntensityYearsPMGSY3()");
                return null;
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Audit]
        public ActionResult AddTrafficIntensityPMGSY3(TrafficViewModel TrafficIntensityViewModel)
        {
            bool status = false;
            try
            {
                if (ModelState.IsValid)
                {
                    if (objDAL.AddTrafficIntensityPMGSY3DAL(TrafficIntensityViewModel, ref message))
                    {
                        message = message == string.Empty ? "Traffic Intensity details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Traffic Intensity details not saved successfully." : message;
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AddTrafficIntensityPMGSY3");
                message = "Traffic Intensity details not saved successfully";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Audit]
        public ActionResult EditTrafficIntensityPMGSY3(TrafficViewModel TrafficViewModel)
        {
            bool status = false;
            try
            {
                if (ModelState.IsValid)
                {
                    if (objDAL.EditTrafficIntensityPMGSY3DAL(TrafficViewModel, ref message))
                    {
                        message = message == string.Empty ? "Traffic Intensity details updated successfully." : message;
                        status = true;
                        ModelState.Clear();
                    }
                    else
                    {
                        message = message == string.Empty ? "Traffic Intensity details not updated successfully." : message;
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EditTrafficIntensityPMGSY3()");
                message = message == string.Empty ? "Traffic Intensity details not updated successfully." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// DeleteTrafficIntensity() action is used to delete Traffic intensity Details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns>Return success message if recored is successfully updated return success message else error message</returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Audit]
        public ActionResult DeleteTrafficIntensityPMGSY3(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            bool status = false;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                int MAST_ER_ROAD_CODE = Convert.ToInt32(decryptedParameters["MAST_ER_ROAD_CODE"]);
                int MAST_TI_YEAR = Convert.ToInt32(decryptedParameters["MAST_IT_YEAR"]);

                if (decryptedParameters.Count() > 0)
                {
                    if (objDAL.DeleteTrafficIntensityPMGSY3DAL(MAST_ER_ROAD_CODE, MAST_TI_YEAR, ref message))
                    {
                        message = "Trafffic Intensity details deleted successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Trafffic Intensity details not deleted successfully." : message;
                    }
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

                }
                message = "Trafffic Intensity details not deleted successfully.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DeleteTrafficIntensityPMGSY3()");
                message = "Trafffic Intensity details not deleted successfully.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        ///CBR
        [Audit]
        public CBRViewModel CBRlengthCalculationPMGSY3(int MAST_ER_ROAD_CODE)
        {
            try
            {
                db = new PMGSYEntities();
                CBRViewModel CbrViewModel = new CBRViewModel();

                MASTER_EXISTING_ROADS existingRoadModel = objDAL.GetRoadDetails(MAST_ER_ROAD_CODE);//db.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).FirstOrDefault();


                //calculate start chainage
                MASTER_ER_CBR_VALUE_PMGSY3 CbrModel = objDAL.GetCBRDetailsPMGSY3DAL(MAST_ER_ROAD_CODE);

                if (CbrModel == null)
                {
                    CbrViewModel.MAST_STR_CHAIN = existingRoadModel.MAST_ER_ROAD_STR_CHAIN;
                }
                else
                {
                    CbrViewModel.MAST_STR_CHAIN = CbrModel.MAST_END_CHAIN;
                }

                //cal total segment length
                CbrViewModel.TotalAvailableRoadLength = db.MASTER_ER_CBR_VALUE_PMGSY3.Where(a => a.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).Count() > 0 ? db.MASTER_ER_CBR_VALUE_PMGSY3.Where(a => a.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).Sum((m => m.MAST_END_CHAIN - m.MAST_STR_CHAIN)) : 0;

                //cal road length
                if (existingRoadModel != null)
                {
                    CbrViewModel.RoadLength = existingRoadModel.MAST_ER_ROAD_END_CHAIN - existingRoadModel.MAST_ER_ROAD_STR_CHAIN;
                }
                else
                {
                    CbrViewModel.RoadLength = 0;
                }

                if (CbrViewModel.TotalAvailableRoadLength == null)
                {
                    CbrViewModel.TotalAvailableRoadLength = CbrViewModel.RoadLength;
                }
                else
                {
                    CbrViewModel.TotalAvailableRoadLength = CbrViewModel.RoadLength - CbrViewModel.TotalAvailableRoadLength;
                }

                //total entered segment length

                if (db.MASTER_ER_CBR_VALUE_PMGSY3.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).Any())
                {
                    CbrViewModel.EnteredSegmentLength = db.MASTER_ER_CBR_VALUE_PMGSY3.Where(c => c.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).AsEnumerable().Sum(c => c.MAST_END_CHAIN - c.MAST_STR_CHAIN);
                }

                return CbrViewModel;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CBRlengthCalculationPMGSY3()");
                return null;
            }
            finally
            {

                db.Dispose();
            }
        }

        [HttpPost]
        [Audit]
        public JsonResult CBRlengthCalculationPMGSY3()
        {
            try
            {
                int MAST_ER_ROAD_CODE = Convert.ToInt32(Request.Params["MAST_ER_ROAD_CODE"]);

                CBRViewModel lengthCalulation = CBRlengthCalculationPMGSY3(MAST_ER_ROAD_CODE);

                return Json(new { TotalAvailableLength = lengthCalulation.TotalAvailableRoadLength, startChainnage = lengthCalulation.MAST_STR_CHAIN, EnteredSegLength = lengthCalulation.EnteredSegmentLength }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CBRlengthCalculationPMGSY3()");
                return null;
            }
        }

        [HttpGet]
        [Audit]
        public ActionResult ListCBRPMGSY3(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;

            ExistingRoadsDAL objDAL = new ExistingRoadsDAL();

            CBRViewModel CbrViewModel = new CBRViewModel();

            CbrViewModel.Operation = "A";
            int MAST_ER_ROAD_CODE = 0, MAST_SEGMENT_NO = 0;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() == 1)
                {
                    //existing road grid value
                    MAST_ER_ROAD_CODE = Convert.ToInt32(decryptedParameters["RoadCode"]);
                    MASTER_EXISTING_ROADS masterExistingRoads = objDAL.GetRoadDetails(MAST_ER_ROAD_CODE);//db.MASTER_EXISTING_ROADS.Find(MAST_ER_ROAD_CODE);

                    if (masterExistingRoads == null)
                    {
                        ModelState.AddModelError(string.Empty, "Existing Roads details not Exist.");
                        return PartialView(new CBRViewModel());
                    }
                    CbrViewModel.MAST_ER_ROAD_CODE = masterExistingRoads.MAST_ER_ROAD_CODE;
                    CbrViewModel.RoadID = masterExistingRoads.MAST_ER_ROAD_NUMBER;
                    CbrViewModel.RoadName = masterExistingRoads.MAST_ER_ROAD_NAME;

                    CBRViewModel lengthCalulation = CBRlengthCalculationPMGSY3(MAST_ER_ROAD_CODE);

                    CbrViewModel.RoadLength = lengthCalulation.RoadLength;
                    CbrViewModel.TotalAvailableRoadLength = lengthCalulation.TotalAvailableRoadLength;
                    CbrViewModel.EnteredSegmentLength = lengthCalulation.EnteredSegmentLength;

                    CbrViewModel.EncryptedCBRCode = URLEncrypt.EncryptParameters1(new string[] { "RoadCode=" + MAST_ER_ROAD_CODE.ToString().Trim() });
                    return PartialView(CbrViewModel);
                }
                else
                {
                    MAST_ER_ROAD_CODE = Convert.ToInt32(decryptedParameters["MAST_ER_ROAD_CODE"]);
                    MAST_SEGMENT_NO = Convert.ToInt32(decryptedParameters["MAST_SEGMENT_NO"]);

                    if (decryptedParameters.Count() > 0)
                    {
                        CBRViewModel CbrViewMode = objDAL.GetCBRDetailsPMGSY3DAL(MAST_ER_ROAD_CODE, MAST_SEGMENT_NO);
                        CbrViewMode.Operation = "U";

                        if (CbrViewMode == null)
                        {
                            ModelState.AddModelError(string.Empty, "CBR Dettails details not Exist.");

                            return PartialView(new CBRViewModel());
                        }

                        //cal total length

                        //plan model
                        MASTER_EXISTING_ROADS planRoadModel = objDAL.GetRoadDetails(MAST_ER_ROAD_CODE);//db.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == MAST_ER_ROAD_CODE).FirstOrDefault();

                        //cal road length
                        if (planRoadModel != null)
                        {
                            CbrViewMode.RoadLength = planRoadModel.MAST_ER_ROAD_END_CHAIN - planRoadModel.MAST_ER_ROAD_STR_CHAIN;
                        }
                        else
                        {
                            CbrViewMode.RoadLength = 0;
                        }

                        CbrViewMode.EncryptedCBRCode = URLEncrypt.EncryptParameters1(new string[] { "RoadCode=" + MAST_ER_ROAD_CODE.ToString().Trim() });
                        return PartialView(CbrViewMode);

                    }
                }
                return PartialView(new CBRViewModel());
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ListCBRPMGSY3()");
                ModelState.AddModelError(string.Empty, "CBR details not Exist.");
                return PartialView(new CBRViewModel());
            }
        }

        /// <summary>
        /// GetCBRList() this Action is used to Display CBR Details On Grid
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns>Return json data for CBR Value Jqgrid</returns>
        [Audit]
        public JsonResult GetCBRListPMGSY3(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 29-Apr-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 29-Apr-2014 end

                int roadCode = Convert.ToInt32(Request["MAST_ER_ROAD_CODE"]);
                int totalRecords;
                var jsonData = new
                {
                    rows = objDAL.GetCBRListPMGSY3DAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, roadCode),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Audit]
        public ActionResult AddCBRDetailsPmgsy3(CBRViewModel CbrViewModel)
        {
            bool status = false;
            try
            {
                if (ModelState.IsValid)
                {
                    if (objDAL.AddCbrValuePMGSY3DAL(CbrViewModel, ref message))
                    {
                        message = message == string.Empty ? "CBR details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "CBR details not saved successfully." : message;
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AddCBRDetailsPmgsy3()");
                message = "CBR details not saved successfully";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Audit]
        public ActionResult EditCBRDetailsPMGSY3(CBRViewModel CbrViewModel)
        {
            bool status = false;
            try
            {
                if (ModelState.IsValid)
                {
                    if (objDAL.EditCbrValuePMGSY3(CbrViewModel, ref message))
                    {
                        message = message == string.Empty ? "CBR details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "CBR details not updated successfully." : message;
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EditCBRDetailsPMGSY3()");
                message = message == string.Empty ? "CBR details not updated successfully." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// DeleteCBRDetails() action is used to Delete CBR details 
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns>Return success message if recored is successfully deleted else returns error message</returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Audit]
        public ActionResult DeleteCBRDetailsPMGSY3(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            bool status = false;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                int MAST_ER_ROAD_CODE = Convert.ToInt32(decryptedParameters["MAST_ER_ROAD_CODE"]);
                int MAST_SEGMENT_NO = Convert.ToInt32(decryptedParameters["MAST_SEGMENT_NO"]);

                if (decryptedParameters.Count() > 0)
                {
                    if (objDAL.DeleteCbrValuePMGSY3(MAST_ER_ROAD_CODE, MAST_SEGMENT_NO, ref message))
                    {
                        message = "CBR details deleted successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "CBR details not deleted successfully." : message;
                    }
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

                }
                message = "CBR details not deleted successfully.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DeleteCBRDetailsPMGSY3()");
                message = "CBR details not deleted successfully.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult PMGSY3EnabledMessageLayout()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PMGSY3EnabledMessage()");
                return null;
            }

        }
        #endregion

        #region DRRP - II and PMGSY-I Mapping

        public ActionResult ListProposalsForDRRPMappingUnderPMGSY3()
        {
            try
            {
                DRRPMappingUnderPMGSY3 proposalViewModel = new DRRPMappingUnderPMGSY3();
                CommonFunctions objCommonFuntion = new CommonFunctions();
                ExistingRoadsDAL objDAL = new ExistingRoadsDAL();

                List<SelectListItem> lstTypes = new List<SelectListItem>();
                lstTypes.Insert(0, new SelectListItem { Value = "0", Text = "All" });
                lstTypes.Insert(1, new SelectListItem { Value = "N", Text = "New Connectivity" });
                lstTypes.Insert(2, new SelectListItem { Value = "U", Text = "Upgradation" });
                proposalViewModel.UserLevelID = PMGSYSession.Current.LevelId;
                proposalViewModel.COLLABORATIONS = objCommonFuntion.PopulateFundingAgency(true);
                proposalViewModel.DISTRICTS = objCommonFuntion.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                if (PMGSYSession.Current.RoleCode == (int)PMGSY.Common.CommonFunctions.PMGSYSessionRoleDetails.PIU)
                {
                    proposalViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                }
                proposalViewModel.BLOCKS = objCommonFuntion.PopulateBlocks(PMGSYSession.Current.DistrictCode, true);
                proposalViewModel.PROPOSAL_STATUS = PopulateProposalStatus(PMGSYSession.Current.RoleCode);
                proposalViewModel.CONNECTIVITYLIST = lstTypes;
                proposalViewModel.BATCHS = objCommonFuntion.PopulateBatch();
                proposalViewModel.BATCHS.RemoveAt(0);
                proposalViewModel.BATCHS.Insert(0, (new SelectListItem { Text = "All Batches", Value = "0", Selected = true }));
                proposalViewModel.PROPOSAL_TYPES = objCommonFuntion.PopulateProposalTypes();
                proposalViewModel.Years = PopulateYear(0, true, true);
                proposalViewModel.IMS_YEAR = DateTime.Now.Year;
                proposalViewModel.RoleID = PMGSYSession.Current.RoleCode;
                proposalViewModel.isPMGSY3 = objDAL.CheckPMGSY3DAL(PMGSYSession.Current.StateCode);
                return View(proposalViewModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoads.ListProposalsForDRRPMappingUnderPMGSY3");
                return null;
            }
        }

        public ActionResult GetProposalsForDRRPMappingUnderPMGSY3(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, Convert.ToInt32(rows), sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                int IMS_YEAR = Convert.ToInt32(Request.Params["IMS_YEAR"]);
                int MAST_BLOCK_ID = Convert.ToInt32(Request.Params["MAST_BLOCK_ID"]);
                int MAST_DISTRICT_ID = Convert.ToInt32(Request.Params["MAST_DISTRICT_ID"]);
                int IMS_BATCH = Convert.ToInt32(Request.Params["IMS_BATCH"]);
                int IMS_STREAMS = Convert.ToInt32(Request.Params["IMS_STREAM"]);
                string IMS_PROPOSAL_TYPE = Request.Params["IMS_PROPOSAL_TYPE"];
                string IMS_PROPOSAL_STATUS = Request.Params["IMS_PROPOSAL_STATUS"];
                string IMS_UPGRADE_CONNECT = Request.Params["IMS_UPGRADE_CONNECT"];
                long totalRecords;

                var jsonData = new
                {
                    rows = objBAL.GetProposalsForDRRPMappingUnderPMGSY3BAL(page.Value - 1, rows, sidx, sord, out totalRecords, PMGSYSession.Current.StateCode, MAST_DISTRICT_ID, IMS_YEAR, MAST_BLOCK_ID, IMS_BATCH, IMS_STREAMS, IMS_PROPOSAL_TYPE, PMGSYSession.Current.AdminNdCode, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoads.GetProposalsForDRRPMappingUnderPMGSY3()");
                return null;
            }
        }



        public List<SelectListItem> PopulateProposalStatus(int RollID)
        {
            // STA & PTALogin Status
            if (RollID == 3 || RollID == 15)
            {
                List<SelectListItem> lstProposalStatus = new List<SelectListItem>();
                SelectListItem item = new SelectListItem();
                item.Text = "Pending Proposals";
                item.Value = "N";
                item.Selected = true;
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Scrutinized Proposals";
                item.Value = "Y";
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Un-Scrutinized Proposals";
                item.Value = "U";
                item.Selected = true;
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "All";
                item.Value = "A";
                lstProposalStatus.Add(item);

                return lstProposalStatus;
            }
            /// MoRD Login Status
            else if (RollID == 25 || RollID == 65)
            {
                List<SelectListItem> lstProposalStatus = new List<SelectListItem>();
                SelectListItem item = new SelectListItem();

                item = new SelectListItem();
                item.Text = "Pending Proposals";
                item.Value = "N";
                item.Selected = true;
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Sanctioned Proposals";
                item.Value = "Y";
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Un-Sanctioned Proposals";
                item.Value = "U";
                item.Selected = true;
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Recommended Proposals";
                item.Value = "R";
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Dropped Propsoal";
                item.Value = "D";
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "STA Pending";
                item.Value = "S";
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "All";
                item.Value = "A";
                lstProposalStatus.Add(item);

                return lstProposalStatus;
            }
            else if (RollID == 2 || RollID == 37 || RollID == 55)
            {
                List<SelectListItem> lstProposalStatus = new List<SelectListItem>();
                SelectListItem item = new SelectListItem();

                item = new SelectListItem();
                item.Text = "All";
                item.Value = "00";
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "PTA Scrutinized";
                item.Value = "PY";
                item.Selected = true;
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "PTA Pending";
                item.Value = "PN";
                item.Selected = true;
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "PTA UnScrutinized";
                item.Value = "PU";
                item.Selected = true;
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "STA Scrutinized";
                item.Value = "SY";
                item.Selected = true;
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "STA Pending";
                item.Value = "SN";
                item.Selected = true;
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "STA UnScrutinized";
                item.Value = "SU";
                item.Selected = true;
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "MoRD Pending";
                item.Value = "MN";
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "MoRD Sanctioned Proposals";
                item.Value = "MY";
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "MoRD Un-Sanctioned Proposals";
                item.Value = "MU";
                item.Selected = true;
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Recommended Proposals";
                item.Value = "MR";
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Dropped Proposals";
                item.Value = "MD";
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "PIU Pending";
                item.Value = "DE";
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Habitation Finalized";
                item.Value = "DH";
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "PIU Finalize";
                item.Value = "DD";
                lstProposalStatus.Add(item);

                return lstProposalStatus;
            }
            else if (RollID == 22 || RollID == 38 || RollID == 54)
            {
                List<SelectListItem> lstProposalStatus = new List<SelectListItem>();
                SelectListItem item = new SelectListItem();

                item = new SelectListItem();
                item.Text = "All";
                item.Value = "00";
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "PTA Srutinized";
                item.Value = "PY";
                item.Selected = true;
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "PTA Pending";
                item.Value = "PN";
                item.Selected = true;
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "PTA UnScrutinized";
                item.Value = "PU";
                item.Selected = true;
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "STA Srutinized";
                item.Value = "SY";
                item.Selected = true;
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "STA Pending";
                item.Value = "SN";
                item.Selected = true;
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "STA UnScrutinized";
                item.Value = "SU";
                item.Selected = true;
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "MoRD Pending";
                item.Value = "MN";
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "MoRD Sanctioned Proposals";
                item.Value = "MY";
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "MoRD Un-Sanctioned Proposals";
                item.Value = "MU";
                item.Selected = true;
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Recommended Proposals";
                item.Value = "MR";
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Dropped Proposals";
                item.Value = "MD";
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "PIU Entry";
                item.Value = "DE";
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Habitation Finalized";
                item.Value = "DH";
                lstProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "PIU Finalized";
                item.Value = "DD";
                lstProposalStatus.Add(item);

                return lstProposalStatus;
            }
            else
            {
                List<SelectListItem> lstProposalStatus = new List<SelectListItem>();
                SelectListItem item = new SelectListItem();

                item.Text = "Select Status";
                item.Value = "0";
                item.Selected = true;
                lstProposalStatus.Add(item);

                return lstProposalStatus;
            }
        }

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

        #region Map Drrp

        public ActionResult MapDRRPToProposalsUnderPMGSY3(String parameter, String hash, String key)
        {
            string[] decryptedParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                ExistingRoadsDAL objDAL = new ExistingRoadsDAL();
                MapDRRPUnderPMGSY3 model = new MapDRRPUnderPMGSY3();
                PMGSYEntities dbContext = new PMGSYEntities();
                model.ProposalCode = Convert.ToInt32(decryptedParameters[0]);
                model = objDAL.GetProposalDetails(model.ProposalCode);
                //model.lstBlocks = objCommon.PopulateBlocks(PMGSYSession.Current.DistrictCode);

                //added by abhinav
                model.lstDistrict = objCommon.PopulateDistrict(PMGSYSession.Current.StateCode);
                model.lstBlocks.Insert(0, new SelectListItem { Text = "Select Block", Value = "0", Selected = true });

                int ERCode = dbContext.MASTER_ER_MAPROAD_PMGSY3.Where(m => m.IMS_PR_ROAD_CODE == model.ProposalCode).Select(m => m.MAST_ER_ROAD_CODE).FirstOrDefault();

                model.lstCoreNetworks = objCommon.PopulateDRRPToMapUnderPMGSY3(model.Block, model.UpgradeConnect, model.ProposalType);
                model.CnCode = ERCode;


                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoads.MapDRRPToProposalsUnderPMGSY3()");
                return null;
            }
        }

        [HttpGet]
        public ActionResult PopulateBlockListOnDistrictSelect()
        {
            try
            {
                CommonFunctions comm = new CommonFunctions();
                var BlockList = comm.PopulateBlocks(Convert.ToInt32(Request.Params["DistrictCode"]));
                BlockList.RemoveAll(x => x.Value == "0");
                BlockList.Insert(0, new SelectListItem { Text = "All Blocks", Value = "0", Selected = true });
                return Json(BlockList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoads/PopulateBlockList");
                return null;
            }
        }

        [HttpGet]
        public ActionResult PopulateDRRPonBlockChange()
        {
            try
            {
                List<SelectListItem> DRRPList = new List<SelectListItem>();
                CommonFunctions comm = new CommonFunctions();
                string upconnect = Request.Params["upgardeconnect"];
                string protype = Request.Params["proposaltype"];
                int blockcode = Convert.ToInt32(Request.Params["DistrictCode"]);
                DRRPList = comm.PopulateDRRPToMapUnderPMGSY3(blockcode, upconnect, protype);

                return Json(DRRPList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoads/PopulateBlockList");
                return null;
            }
        }


        [HttpPost]
        public ActionResult MapDRRPDetailsUnderPMGSY3(MapDRRPUnderPMGSY3 model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool status = objBAL.MapDRRPDetailsBAL(model);
                    if (status == true)
                    {
                        return Json(new { success = true, message = "Proposal Updated Successfully." });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Proposal Details not Updated" });
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoads.MapDRRPDetailsUnderPMGSY3()");
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }


        [HttpPost]
        [Audit]
        public ActionResult FinalizeDRRPDetails(string id)
        {
            try
            {
                int prRoadCode = Convert.ToInt32(id);
                string result = objDAL.FinalizeProposalDAL(prRoadCode);
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
                ErrorLog.LogError(ex, "ExistingRoads.FinalizeDRRPDetails()");
                return null;
            }

        }
        [HttpPost]
        [Audit]
        public ActionResult DeFinalizeDRRPDetails(string id)
        {
            try
            {
                int prRoadCode = Convert.ToInt32(id);
                string result = objDAL.DeFinalizeProposalDAL(prRoadCode);
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
                ErrorLog.LogError(ex, "ExistingRoads.DeFinalizeDRRPDetails()");
                return null;
            }
        }

        [HttpPost]
        [Audit]
        public JsonResult GetLinkThroughList()
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                int BlockID = Convert.ToInt32(Request.Params["BlockID"]);
                string ims_upgrade_connect = Request.Params["IMS_UPGRADE_CONNECT"].ToString();
                string proposalType = Request.Params["PROPOSAL_TYPE"].ToString();
                return Json(objCommon.PopulateDRRPToMapUnderPMGSY3(BlockID, ims_upgrade_connect, proposalType));
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoads.GetLinkThroughList()");
                return null;
            }
        }
        #endregion

        #region Finalize DRRP PMGSY3 BLOCK/DISTRICT
        [Audit]
        public ActionResult FinalizeDRRPPMGSY3Layout()
        {
            FinalizeDRRPPMGSY3ViewModel model = new FinalizeDRRPPMGSY3ViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                model.lstDistricts = comm.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                model.lstDistricts.Find(x => x.Value == "-1").Text = "Select District";

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoads.ListExistingRoadsPMGSY3()");
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
                            , ref isAllBlockFinalized),
                    total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                    page = Convert.ToInt32(Request.Params["page"]),
                    records = totalRecords,
                    isAllBlockFinalized = isAllBlockFinalized
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoads.GetExistingRoadsPMGSY3List()");
                return null;
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Audit]
        public JsonResult FinalizeDRRPBlock(String parameter, String hash, String key)
        {
            string message = string.Empty;
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                int blockCode = Convert.ToInt32(decryptedParameters["BlockCode"]);

                if (decryptedParameters.Count() > 0)
                {
                    bool status = objDAL.FinalizeDRRPBlockPMGSY3DAL(blockCode, ref message);
                    return Json(new { success = status, message = message.Trim() });
                }
                else
                {
                    return Json(new { success = false, message = "Invalid request" });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoads.FinalizeDRRPBlock()");
                return Json(new { success = false, message = "Error occurred while processing the request." });
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Audit]
        public JsonResult FinalizeDRRPDistrict(int districtCode)
        {
            string message = string.Empty;
            try
            {
                bool status = objDAL.FinalizeDRRPDistrictPMGSY3DAL(districtCode, ref message);
                return Json(new { success = status, message = message.Trim() });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoads.FinalizeDRRPBlock()");
                return Json(new { success = false, message = "Error occurred while processing the request." });
            }
        }
        #endregion

        #region Trace Maps
        [HttpGet]
        public ActionResult GetTraceMaps()
        {
            try
            {
                TraceMapsModel model = new TraceMapsModel();
                CommonFunctions common = new CommonFunctions();
                model.DistrictList = new List<SelectListItem>();
                model.StateList = new List<SelectListItem>();

                if (PMGSYSession.Current.RoleCode == 25)
                {
                    model.StateList = common.PopulateStates();
                    model.DistrictList.Add(new SelectListItem { Text = "Select District", Value = "0", Selected = true });
                    //model.BlockList.Add(new SelectListItem { Text = PMGSYSession.Current.DistrictName, Value = PMGSYSession.Current.DistrictCode.ToString(), Selected = true });
                }

                if (PMGSYSession.Current.DistrictCode > 0)
                {
                    model.DistrictList.Add(new SelectListItem { Text = PMGSYSession.Current.DistrictName, Value = PMGSYSession.Current.DistrictCode.ToString(), Selected = true });
                    return View(model);
                }

                if (PMGSYSession.Current.StateCode > 0)
                {
                    model.DistrictList = common.PopulateDistrict(PMGSYSession.Current.StateCode);
                    return View(model);
                }

                return View(model);
            }

            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoads/GetTraceMaps");
                return null;
            }
        }

        [HttpPost]
        public ActionResult GetDistrictList(FormCollection formdata)
        {

            try
            {
                int totalRecords;

                var formData = Request.Params["formdata"];
                var FormArrayKeys = formdata[0].Split('&');

                List<string> ModelValues = new List<string>();

                foreach (var item in FormArrayKeys)
                {
                    ModelValues.Add(item.Split('=')[1]);
                }
                var DistrictCode = Convert.ToInt32(Request.Params["DistrictCodeDD"]);

                #region For MORD

                if (PMGSYSession.Current.RoleCode == 25)
                {
                    ExistingRoadsDAL objDAL = new ExistingRoadsDAL();
                    var Statecode = Convert.ToInt32(Request.Params["statecode"]);
                    var jsonDataMORD = new
                    {
                        rows = objDAL.GetDistrictListMORDDAL(Convert.ToInt32(formdata["page"]) - 1, Convert.ToInt32(formdata["rows"]), formdata["sidx"], formdata["sord"], out totalRecords, Statecode, DistrictCode),
                        total = totalRecords <= Convert.ToInt32(formdata["rows"]) ? 1 : totalRecords / Convert.ToInt32(formdata["rows"]) + 1,
                        page = Convert.ToInt32(formdata["page"]),
                        records = totalRecords
                    };
                    return Json(jsonDataMORD);

                }


                #endregion
                var jsonData = new
                {

                    rows = objBAL.GetBlockListTM(Convert.ToInt32(formdata["page"]) - 1, Convert.ToInt32(formdata["rows"]), formdata["sidx"], formdata["sord"], out totalRecords, DistrictCode),
                    total = totalRecords <= Convert.ToInt32(formdata["rows"]) ? 1 : totalRecords / Convert.ToInt32(formdata["rows"]) + 1,
                    page = Convert.ToInt32(formdata["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "LocationMasterDataEntryController.GetFacilityDetailsList()");
                return null;
            }

        }

        [HttpGet]
        public ActionResult PdfFileUploadView(string id)
        {
            try
            {
                PMGSYEntities dbcontext = new PMGSYEntities();
                FileUploadModel model = new FileUploadModel();
                model.BlockCode = Convert.ToInt32(id);
                model.ErrorMessage = string.Empty;
                if (PMGSYSession.Current.RoleCode == 22)
                {
                    model.NumberofPdfs = 1;
                }
                else
                {
                    bool isRecordPresent = dbcontext.MAST_TRACEFILE_PMGSY3.Where(x => x.MAST_BLOCK_CODE == model.BlockCode && x.TRACEFILE_NAME_PDF != null).Any();
                    model.NumberofPdfs = isRecordPresent == true ? 1 : 0;
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoads/PdfFileUploadView");
                return null;
            }
        }

        [HttpGet]
        public ActionResult CsvFileUploadView(string id)
        {
            try
            {
                PMGSYEntities dbcontext = new PMGSYEntities();
                FileUploadModel model = new FileUploadModel();
                model.BlockCode = Convert.ToInt32(id);
                model.ErrorMessage = string.Empty;
                if (PMGSYSession.Current.RoleCode == 22)
                {
                    model.NumberofPdfs = 1;
                }
                else
                {

                    bool isRecordPresent = dbcontext.MAST_TRACEFILE_PMGSY3.Where(x => x.MAST_BLOCK_CODE == model.BlockCode && x.TRACEFILE_NAME_CSV != null).Any();
                    model.NumberofPdfs = isRecordPresent == true ? 1 : 0;
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoads/CsvFileUploadView");
                return null;
            }
        }

        [HttpPost]
        public ActionResult TraceMapsPDFFileUpload(FileUploadModel filemodel)
        {
            try
            {
                CommonFunctions objCommonFunc = new CommonFunctions();

                foreach (string file in Request.Files)
                {
                    string status = ValidatePDFFile(Request.Files[0].ContentLength, Path.GetExtension(Request.Files[0].FileName));
                    if (status != string.Empty)
                    {
                        filemodel.ErrorMessage = status;
                        return Json(new { message = "Invalid File, Please upload valid file for the selected block", success = true });
                    }
                }

                var fileData = new List<FileUploadModel>();

                foreach (string file in Request.Files)
                {
                    UploadMultipleInspPDFFile(Request, fileData, filemodel.BlockCode);
                }


                filemodel.NumberofPdfs = 0;

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
                ErrorLog.LogError(ex, "ExistingRoads/TraceMapsPDFFileUpload");
                filemodel.ErrorMessage = "An Error Occurred While Processing Your Request.";
                return View("PdfFileUploadView", filemodel.ErrorMessage);
            }
            finally
            {
            }
        }

        //Save pdf file.
        public void UploadMultipleInspPDFFile(HttpRequestBase request, List<FileUploadModel> statuses, int blockcode)
        {
            int MaxID = 0;
            try
            {
                PMGSYEntities dbcontext = new PMGSYEntities();
                for (int i = 0; i < request.Files.Count; i++)
                {
                    HttpPostedFileBase file = request.Files[i];
                    if (dbcontext.MAST_TRACEFILE_PMGSY3.Count() == 0)
                    {
                        MaxID = 1;
                    }
                    else
                    {
                        MaxID = (from c in dbcontext.MAST_TRACEFILE_PMGSY3 select c.MAST_TRACEFILE_ID).Max();
                        ++MaxID;
                    }

                    var fileName = blockcode + "_" + MaxID + Path.GetExtension(request.Files[i].FileName).ToString();

                    string PhysicalPath = ConfigurationManager.AppSettings["TRACE_MAPS_FILE_UPLOAD_PDF"];

                    var fullPath = Path.Combine(PhysicalPath, fileName);

                    statuses.Add(new FileUploadModel()
                    {
                        url = fullPath,
                        thumbnail_url = fullPath,
                        name = fileName,
                        type = file.ContentType,
                        size = file.ContentLength,
                        delete_url = "",
                        delete_type = "DELETE",
                        BlockCode = blockcode
                    });

                    string status = objBAL.TraceMapsSaveFileDetails(statuses, ref fileName);
                    if (status == string.Empty)
                    {
                        file.SaveAs(Path.Combine(PhysicalPath, fileName));
                    }
                    else
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoad/UploadMultipleInspPDFFile");
                throw;
            }
        }

        public string ValidatePDFFile(int FileSize, string FileExtension)
        {
            if (FileExtension.ToUpper() != ".PDF")
            {
                return "File is not PDF File";
            }
            if (FileSize > Convert.ToInt32(ConfigurationManager.AppSettings["PROPOSAL_PDF_FILE_MAX_SIZE"]))
            {
                return "File Size Exceed the Maximum File Limit";
            }

            return string.Empty;
        }

        [HttpPost]
        public JsonResult ListInspMultipleCSVFiles(FormCollection formCollection)
        {
            try
            {
                ExistingRoadsDAL objDAL = new ExistingRoadsDAL();
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                int blokcode = Convert.ToInt32(Request["blockcode"]);
                int totalRecords;
                var jsonData = new
                {
                    rows = objDAL.GetTraceCSVFilesListDAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, blokcode),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoads/ListInspMultipleCSVFiles");
                return null;
            }
        }

        //Download uploaded pdf
        [HttpGet]
        public ActionResult DownloadMultipleCSVFile(String parameter, String hash, String key)
        {
            try
            {
                PMGSYEntities dbContext = new PMGSYEntities();
                string FileName = string.Empty;
                int obsId = 0;
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
                        obsId = Convert.ToInt32(urlSplitParams[1]);
                    }
                }
                FileExtension = Path.GetExtension(FileName).ToLower();

                string VirtualDirectoryUrl = string.Empty;
                string PhysicalPath = string.Empty;

                VirtualDirectoryUrl = ConfigurationManager.AppSettings["TRACE_MAPS_FILE_UPLOAD_PDF_VIRTUAL_DIR_PATH"];
                PhysicalPath = ConfigurationManager.AppSettings["DRRP_TRACE_CSV_PATH"];


                FullFileLogicalPath = Path.Combine(VirtualDirectoryUrl, FileName);
                FullfilePhysicalPath = Path.Combine(PhysicalPath, FileName);


                string name = Path.GetFileName(FileName);
                string ext = Path.GetExtension(FileName);

                if (System.IO.File.Exists(FullfilePhysicalPath))
                {
                    return File(FullfilePhysicalPath, "Application/csv", DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + FileExtension);
                }
                else
                {
                    return Json(new { Success = "false", ErrorMessage = "File Not Found." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoads/DownloadMultipleCSVFile");
                return Json(new { Success = "false", ErrorMessage = "Error While Processing Your Request." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult ListInspMultiplePDFFiles(FormCollection formCollection)
        {
            try
            {
                ExistingRoadsDAL objDAL = new ExistingRoadsDAL();
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                int blokcode = Convert.ToInt32(Request["blockcode"]);
                int totalRecords;
                var jsonData = new
                {
                    rows = objDAL.GetTraceMapsFilesListDAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, blokcode),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoads/ListInspMultiplePDFFiles");
                return null;
            }
        }

        //Download uploaded pdf
        [HttpGet]
        public ActionResult DownloadMultipleInspFile(String parameter, String hash, String key)
        {
            try
            {
                PMGSYEntities dbContext = new PMGSYEntities();
                string FileName = string.Empty;
                int obsId = 0;
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
                        obsId = Convert.ToInt32(urlSplitParams[1]);
                    }
                }
                FileExtension = Path.GetExtension(FileName).ToLower();

                string VirtualDirectoryUrl = string.Empty;
                string PhysicalPath = string.Empty;

                VirtualDirectoryUrl = ConfigurationManager.AppSettings["TRACE_MAPS_FILE_UPLOAD_PDF_VIRTUAL_DIR_PATH"];
                PhysicalPath = ConfigurationManager.AppSettings["TRACE_MAPS_FILE_UPLOAD_PDF"];


                FullFileLogicalPath = Path.Combine(VirtualDirectoryUrl, FileName);
                FullfilePhysicalPath = Path.Combine(PhysicalPath, FileName);


                string name = Path.GetFileName(FileName);
                string ext = Path.GetExtension(FileName);

                if (System.IO.File.Exists(FullfilePhysicalPath))
                {
                    return File(FullfilePhysicalPath, "Application/pdf", DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + FileExtension);
                }
                else
                {
                    return Json(new { Success = "false", ErrorMessage = "File Not Found." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoads/DownloadMultipleInspFile");
                return Json(new { Success = "false", ErrorMessage = "Error While Processing Your Request." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult DeleteMultipleInspFileDetails(string id)
        {
            PMGSYEntities dbContext = new PMGSYEntities();

            try
            {
                bool isUpdated = false;
                string FILE_NAME = Request.Params["FILE_NAME"];

                string[] arrParam = Request.Params["FileID"].Split('$');

                int FILE_ID = Convert.ToInt32(arrParam[0]);
                int blockcode = Convert.ToInt32(arrParam[1]);
                string filename = string.Empty;
                var filedetails = dbContext.MAST_TRACEFILE_PMGSY3.Where(obj => obj.MAST_TRACEFILE_ID == FILE_ID && obj.MAST_BLOCK_CODE == blockcode).FirstOrDefault();

                string PhysicalPath = string.Empty;
                if (filedetails != null)
                {
                    filename = filedetails.TRACEFILE_NAME_PDF;
                    PhysicalPath = ConfigurationManager.AppSettings["TRACE_MAPS_FILE_UPLOAD_PDF"];
                    filedetails.TRACEFILE_NAME_PDF = null;
                    filedetails.TRACEFILE_PDF_UPLOAD_DATE = null;
                    dbContext.Entry(filedetails).State = System.Data.Entity.EntityState.Modified;
                    isUpdated = true;
                }


                PhysicalPath = Path.Combine(PhysicalPath, filename);
                if (isUpdated && filedetails != null)
                {
                    try
                    {
                        System.IO.File.Delete(PhysicalPath);
                        dbContext.SaveChanges();

                        return Json(new { Success = true, ErrorMessage = "" });
                    }
                    catch (Exception ex)
                    {
                        return Json(new { Success = true, ErrorMessage = ex.Message });
                    }
                    return Json(new { Success = true, ErrorMessage = "There is an error while processing your request." });
                }
                else
                {
                    return Json(new { Success = false });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoads/DeleteMultipleInspFileDetails");
                return Json(new { Success = false, ErrorMessage = "There is an error while processing your request." });
            }
        }

        [HttpPost]
        public JsonResult DeleteCSVFileDetails(string id)
        {
            PMGSYEntities dbContext = new PMGSYEntities();

            try
            {
                bool isUpdated = false;
                string FILE_NAME = Request.Params["FILE_NAME"];

                string[] arrParam = Request.Params["FileID"].Split('$');

                int FILE_ID = Convert.ToInt32(arrParam[0].Split('&')[0]);
                int blockcode = Convert.ToInt32(arrParam[0].Split('&')[1]);
                string filename = string.Empty;

                var filedetails = dbContext.MAST_TRACEFILE_PMGSY3.Where(obj => obj.MAST_TRACEFILE_ID == FILE_ID && obj.MAST_BLOCK_CODE == blockcode).FirstOrDefault();

                var fileDetailsFromScoreTable = dbContext.MAST_TRACE_DRRP_SCORE_PMGSY3.Where(obj => obj.MAST_TRACEFILE_ID == FILE_ID && obj.MAST_BLOCK_CODE == blockcode).ToList();

                string PhysicalPath = string.Empty;
                if (filedetails != null)
                {
                    filename = filedetails.TRACEFILE_NAME_CSV;
                    PhysicalPath = ConfigurationManager.AppSettings["DRRP_TRACE_CSV_PATH"];
                    filedetails.TRACEFILE_NAME_CSV = null;
                    filedetails.TRACEFILE_CSV_UPLOAD_DATE1 = null;
                    dbContext.Entry(filedetails).State = System.Data.Entity.EntityState.Modified;
                    isUpdated = true;
                }

                if (fileDetailsFromScoreTable != null)
                {
                    dbContext.MAST_TRACE_DRRP_SCORE_PMGSY3.DeleteMany(fileDetailsFromScoreTable);
                }

                PhysicalPath = Path.Combine(PhysicalPath, filename);
                if (isUpdated && filedetails != null)
                {
                    try
                    {
                        System.IO.File.Delete(PhysicalPath);
                        dbContext.SaveChanges();

                        return Json(new { Success = true, ErrorMessage = "" });
                    }
                    catch (Exception ex)
                    {
                        return Json(new { Success = true, ErrorMessage = ex.Message });
                    }
                    return Json(new { Success = true, ErrorMessage = "There is an error while processing your request." });
                }
                else
                {
                    return Json(new { Success = false });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoads/DeleteCSVFileDetails");
                return Json(new { Success = false, ErrorMessage = "There is an error while processing your request." });
            }
        }

        [HttpPost]
        public ActionResult FinaliseFileDetails()
        {
            try
            {
                PMGSYEntities dbcontext = new PMGSYEntities();
                var blockcode = Convert.ToInt32(Request.Params["BlockCode"]);

                if (dbcontext.MAST_HAB_CSV_PMGSY3.Where(obj => obj.MAST_BLOCK_CODE == blockcode).Any())
                {
                    if (dbcontext.MAST_HAB_CSV_PMGSY3.Where(obj => obj.MAST_BLOCK_CODE == blockcode && obj.MAST_HAB_CSV_FILE_FINALIZED == "Y").Any())
                    {
                        // Continue to Update FLAG
                    }
                    else
                    {
                        return Json(new { success = false }, JsonRequestBehavior.DenyGet);
                    }
                }
                else
                {
                    return Json(new { success = false }, JsonRequestBehavior.DenyGet);
                }

                var entry = dbcontext.MAST_TRACEFILE_PMGSY3.Where(obj => obj.MAST_BLOCK_CODE == blockcode).FirstOrDefault();
                if (entry != null)
                {
                    entry.TRACEFILE_FINALIZE = "Y";
                    dbcontext.Entry(entry).State = System.Data.Entity.EntityState.Modified;
                    dbcontext.SaveChanges();
                    return Json(new { success = true }, JsonRequestBehavior.DenyGet);
                }

                return Json(new { success = false }, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistinRoads/FinaliseFileDetails");
                return Json(new { success = false }, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpGet]
        public ActionResult DownloadFacilityList(string id)
        {
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            try
            {
                int blockcode = Convert.ToInt32(id);
                db = new PMGSYEntities();
                var gridview = new GridView();
                gridview.DataSource = db.USP_HAB_FACILITY_LIST(blockcode).ToList();
                gridview.DataBind();

                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=" + "Facility_List.xls");
                Response.ContentType = "application/ms-excel";

                Response.Charset = "";
                gridview.RenderControl(objHtmlTextWriter);

                Response.Output.Write(objStringWriter.ToString());
                Response.Flush();
                Response.End();
                return null;

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoads/DownloadFacilityList");
                return null;
            }
            finally
            {
                objStringWriter.Flush();
                objStringWriter.Dispose();
                objHtmlTextWriter.Flush();
                objHtmlTextWriter.Dispose();
            }
        }

        [HttpGet]
        public ActionResult PopulateDistrictList()
        {
            try
            {
                CommonFunctions comm = new CommonFunctions();
                var DistrictList = comm.PopulateDistrict(Convert.ToInt32(Request.Params["StateCode"]));
                DistrictList.RemoveAll(x => x.Value == "0");
                DistrictList.Insert(0, new SelectListItem { Text = "All District", Value = "0", Selected = true });
                return Json(DistrictList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoads/PopulateDistrictList");
                return null;
            }
        }

        [HttpPost]
        public ActionResult DefinalizeTraceMaps()
        {
            try
            {
                PMGSYEntities dbcontext = new PMGSYEntities();
                var blockcode = Convert.ToInt32(Request.Params["BlockCode"]);

                var entry = dbcontext.MAST_TRACEFILE_PMGSY3.Where(obj => obj.MAST_BLOCK_CODE == blockcode).FirstOrDefault();

                var entry1 = dbcontext.MAST_HAB_CSV_PMGSY3.Where(obj => obj.MAST_BLOCK_CODE == blockcode).FirstOrDefault();

                if (entry != null)
                {
                    if (entry1 != null)
                    {
                        entry1.MAST_HAB_CSV_FILE_FINALIZED = "N";
                        dbcontext.Entry(entry1).State = System.Data.Entity.EntityState.Modified;
                        dbcontext.SaveChanges();
                    }
                }

                if (entry != null)
                {
                    entry.TRACEFILE_FINALIZE = "N";
                    dbcontext.Entry(entry).State = System.Data.Entity.EntityState.Modified;
                    dbcontext.SaveChanges();
                    return Json(new { success = true }, JsonRequestBehavior.DenyGet);
                }

                return Json(new { success = false }, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistinRoads/DefinalizeTraceMaps");
                return Json(new { success = false }, JsonRequestBehavior.DenyGet);
            }
        }

        #endregion


        /// ///CSV
        #region  
        public ActionResult CSVLayout()
        {
            try
            {
                FileUploadModel model = new FileUploadModel();
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoads.CSVLayout()");
                return null;
            }
        }

        public ActionResult GetCSVValue()
        {
            bool success = false;
            bool flg = false;
            string filePath = string.Empty;
            string fileSaveExt = string.Empty;
            int fileValidSize = 0;
            string message = string.Empty;
            HttpPostedFileBase file = null;
            var blockcode = Convert.ToInt32(Request.Form["BlockCode"]);
            try
            {
                if (Request.Files.AllKeys.Count() <= 0)
                {
                    return Json(new { success = false, message = "File not selected" });
                }

                filePath = ConfigurationManager.AppSettings["DRRP_TRACE_CSV_PATH"];
                objDAL = new ExistingRoadsDAL();

                if (Request.Files.Count > 0)
                {
                    file = Request.Files[0];

                    fileSaveExt = Path.GetExtension(file.FileName);//file.ContentType;
                    fileSaveExt = fileSaveExt.Contains('.') ? fileSaveExt.Trim().Remove(0, 1) : fileSaveExt;
                    fileValidSize = file.ContentLength;

                    if (fileSaveExt.ToLower() != "csv")
                    {
                        return Json(new { success = false, message = "Please select a valid csv file" });
                    }
                    flg = objDAL.UploadCSVDAL(file, ",", ref message, blockcode);
                    if (!flg)
                    {
                        return Json(new { message = "Invalid File, Please upload valid file for the selected block", success = false }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoads.GetCSVValue");
            }
            return Json(new { success = flg, message = message }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region List Existing Roads ITNO

        public ActionResult ListExistingRoadsITNO()
        {
            try
            {
                IExistingRoadsDAL objDAL = new ExistingRoadsDAL();
                List<SelectListItem> lstDefault = new List<SelectListItem>();
                CommonFunctions objCommon = new CommonFunctions();
                //lstDefault.Insert(0, new SelectListItem { Value = "0", Text = "All" });

                lstDefault = objCommon.PopulateDistrict(PMGSYSession.Current.StateCode, false);

                List<MASTER_BLOCK> blockList = objDAL.GetAllBlockNames(PMGSYSession.Current.DistrictCode);

                List<SelectListItem> StateListITNO = new List<SelectListItem>();
                List<SelectListItem> BlockListITNO = new List<SelectListItem>();
                BlockListITNO.Insert(0, new SelectListItem { Value = "0", Text = "Select Block", Selected = true });
                StateListITNO.Insert(0, new SelectListItem { Value = PMGSYSession.Current.StateCode.ToString(), Text = PMGSYSession.Current.StateName, Selected = true });

                if (PMGSYSession.Current.RoleCode == 25 || PMGSYSession.Current.RoleCode == 53 || PMGSYSession.Current.RoleCode == 65 || PMGSYSession.Current.RoleCode == 36)//Changes by SAMMED A. PATIL for mordviewuser
                {
                    ViewData["States"] = StateListITNO;//objCommon.PopulateStates(true);
                    ViewData["Districts"] = lstDefault;
                    ViewData["blockList"] = BlockListITNO;
                }
                else if (PMGSYSession.Current.RoleCode == 2)
                {
                    ViewData["Districts"] = objCommon.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                    ViewData["blockList"] = lstDefault;
                }
                else
                {
                    ViewData["blockList"] = new SelectList(blockList, "MAST_BLOCK_CODE", "MAST_BLOCK_NAME");
                }

                List<MASTER_ROAD_CATEGORY> roadCategoryList = objDAL.GetAllRoadCategory();
                roadCategoryList.Insert(0, new MASTER_ROAD_CATEGORY() { MAST_ROAD_CAT_CODE = 0, MAST_ROAD_CAT_NAME = "-- All --" });

                ViewData["roadCategoryList"] = new SelectList(roadCategoryList, "MAST_ROAD_CAT_CODE", "MAST_ROAD_CAT_NAME");

                if (blockList.Count > 0)
                {
                    ViewData["isUnlocked"] = objDAL.CheckUnlockedDAL(blockList.Select(x => x.MAST_BLOCK_CODE).First());

                    ///PMGSY3
                    ExistingRoadsDAL objDRRPDAL = new ExistingRoadsDAL();
                    ViewData["isPMGSY3"] = objDRRPDAL.CheckPMGSY3DAL(PMGSYSession.Current.StateCode);
                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "ListExistingRoads()");
                ViewData["blockList"] = null;
                ViewData["roadCategoryList"] = null;
            }
            return View();
        }

        [HttpPost]
        public ActionResult GetExistingRoadsListITNO(int? page, int? rows, string sidx, string sord)
        {
            String searchParameters = String.Empty;
            long totalRecords;

            int blockCode = 0;
            int categoryCode = 0;
            int ownerCode = 0;
            string roadName = string.Empty;
            int stateCode = 0;
            int districtCode = 0;


            try
            {

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["stateCode"]))
                {
                    stateCode = Convert.ToInt32(Request.Params["stateCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["districtCode"]))
                {
                    districtCode = Convert.ToInt32(Request.Params["districtCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["MAST_BLOCK_CODE"]))
                {
                    blockCode = Convert.ToInt32(Request.Params["MAST_BLOCK_CODE"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["MAST_ROAD_CAT_CODE"]))
                {
                    categoryCode = Convert.ToInt32(Request.Params["MAST_ROAD_CAT_CODE"]);
                }

                var jsonData = new
                {
                    rows = objBAL.ListExistingRoadsITNO(stateCode, districtCode, blockCode, categoryCode, ownerCode, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords, Request.Params["filters"]),
                    total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                    page = Convert.ToInt32(Request.Params["page"]),
                    records = totalRecords
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
        public ActionResult DeleteExistingCoreNetworkRoad(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            bool status = false;
            try
            {
                ExistingRoadsDAL objDAL = new ExistingRoadsDAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                int roadCode = Convert.ToInt32(decryptedParameters["RoadCode"]);

                if (decryptedParameters.Count() > 0)
                {
                    if (objDAL.DeleteExistingRoadsITNO(roadCode, ref message))
                    {
                        message = "Candidate Road details deleted successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Candidate Road Details has not been deleted." : message;
                    }
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

                }
                message = "There is an error occured while processing your request.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoads/DeleteExistingCoreNetworkRoad");
                message = "There is an error occured while processing your request.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [Audit]
        public ActionResult ViewExistingRoadsITNO(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                int roadCode = Convert.ToInt32(decryptedParameters["RoadCode"]);

                IExistingRoadsDAL objDAL = new ExistingRoadsDAL();

                if (decryptedParameters.Count() > 0)
                {
                    ExistingRoadsViewModel existingRoadsViewMode = objDAL.GetExistingRoads_ForViewDetails(roadCode);

                    if (existingRoadsViewMode == null)
                    {
                        ModelState.AddModelError(string.Empty, "Existing Road details not Exist.");
                        return PartialView("ViewExistingRoadsDetailsITNO", new ExistingRoadsViewModel());
                    }
                    return PartialView("ViewExistingRoadsDetailsITNO", existingRoadsViewMode);
                }
                return PartialView("ViewExistingRoadsDetailsITNO", new ExistingRoadsViewModel());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "Existing Road details not Exist.");
                return PartialView("ViewExistingRoadsDetailsITNO", new ExistingRoadsViewModel());
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult DeleteExistingRoadsMainITNO(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            bool status = false;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                int roadCode = Convert.ToInt32(decryptedParameters["RoadCode"]);
                IExistingRoadsDAL objDAL = new ExistingRoadsDAL();
                if (decryptedParameters.Count() > 0)
                {
                    if (objDAL.DeleteExistingRoadsMainITNO(roadCode, ref message))
                    {
                        message = "Existing Road details deleted successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Existing Road details can not be deleted because other details for road are entered." : message;
                    }
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

                }
                message = "There is an error occured while processing your request.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "There is an error occured while processing your request.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ListCoreNetworkByDRRPITNO(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                int roadCode = Convert.ToInt32(Request["MAST_ER_ROAD_CODE"]);
                int totalRecords;
                IExistingRoadsDAL objDAL = new ExistingRoadsDAL();
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var jsonData = new
                {
                    rows = objDAL.GetCoreNetworkListITNO(Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords, roadCode),
                    total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                    page = Convert.ToInt32(Request.Params["page"]),
                    records = totalRecords
                };

                return Json(jsonData);
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion


        #region List Existing Roads ITNO for Inactive Blocks

        //public ActionResult ListExistingRoadsITNOForInactiveBlocks()
        //{
        //    try
        //    {
        //        IExistingRoadsDAL objDAL = new ExistingRoadsDAL();
        //        List<SelectListItem> lstDefault = new List<SelectListItem>();
        //        CommonFunctions objCommon = new CommonFunctions();
        //        //lstDefault.Insert(0, new SelectListItem { Value = "0", Text = "All" });

        //        lstDefault = objCommon.PopulateDistrict(PMGSYSession.Current.StateCode, false);

        //        List<MASTER_BLOCK> blockList = objDAL.GetAllBlockNames(PMGSYSession.Current.DistrictCode);

        //        List<SelectListItem> StateListITNO = new List<SelectListItem>();
        //        List<SelectListItem> BlockListITNO = new List<SelectListItem>();
        //        BlockListITNO.Insert(0, new SelectListItem { Value = "0", Text = "Select Block", Selected = true });
        //        StateListITNO.Insert(0, new SelectListItem { Value = PMGSYSession.Current.StateCode.ToString(), Text = PMGSYSession.Current.StateName, Selected = true });

        //        if (PMGSYSession.Current.RoleCode == 25 || PMGSYSession.Current.RoleCode == 53 || PMGSYSession.Current.RoleCode == 65 || PMGSYSession.Current.RoleCode == 36)//Changes by SAMMED A. PATIL for mordviewuser
        //        {
        //            ViewData["States"] = StateListITNO;//objCommon.PopulateStates(true);
        //            ViewData["Districts"] = lstDefault;
        //            ViewData["blockList"] = BlockListITNO;
        //        }
        //        else if (PMGSYSession.Current.RoleCode == 2)
        //        {
        //            ViewData["Districts"] = objCommon.PopulateDistrict(PMGSYSession.Current.StateCode, true);
        //            ViewData["blockList"] = lstDefault;
        //        }
        //        else
        //        {
        //            ViewData["blockList"] = new SelectList(blockList, "MAST_BLOCK_CODE", "MAST_BLOCK_NAME");
        //        }

        //        List<MASTER_ROAD_CATEGORY> roadCategoryList = objDAL.GetAllRoadCategory();
        //        roadCategoryList.Insert(0, new MASTER_ROAD_CATEGORY() { MAST_ROAD_CAT_CODE = 0, MAST_ROAD_CAT_NAME = "-- All --" });

        //        ViewData["roadCategoryList"] = new SelectList(roadCategoryList, "MAST_ROAD_CAT_CODE", "MAST_ROAD_CAT_NAME");

        //        if (blockList.Count > 0)
        //        {
        //            ViewData["isUnlocked"] = objDAL.CheckUnlockedDAL(blockList.Select(x => x.MAST_BLOCK_CODE).First());

        //            ///PMGSY3
        //            ExistingRoadsDAL objDRRPDAL = new ExistingRoadsDAL();
        //            ViewData["isPMGSY3"] = objDRRPDAL.CheckPMGSY3DAL(PMGSYSession.Current.StateCode);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
        //        ErrorLog.LogError(ex, "ListExistingRoads()");
        //        ViewData["blockList"] = null;
        //        ViewData["roadCategoryList"] = null;
        //    }
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult GetExistingRoadsListITNOForInactiveBlocks(int? page, int? rows, string sidx, string sord)
        //{
        //    String searchParameters = String.Empty;
        //    long totalRecords;

        //    int blockCode = 0;
        //    int categoryCode = 0;
        //    int ownerCode = 0;
        //    string roadName = string.Empty;
        //    int stateCode = 0;
        //    int districtCode = 0;

        //    try
        //    {

        //        using (CommonFunctions commonFunction = new CommonFunctions())
        //        {
        //            if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
        //            {
        //                return null;
        //            }
        //        }

        //        if (!string.IsNullOrEmpty(Request.Params["stateCode"]))
        //        {
        //            stateCode = Convert.ToInt32(Request.Params["stateCode"]);
        //        }

        //        if (!string.IsNullOrEmpty(Request.Params["districtCode"]))
        //        {
        //            districtCode = Convert.ToInt32(Request.Params["districtCode"]);
        //        }

        //        if (!string.IsNullOrEmpty(Request.Params["MAST_BLOCK_CODE"]))
        //        {
        //            blockCode = Convert.ToInt32(Request.Params["MAST_BLOCK_CODE"]);
        //        }

        //        if (!string.IsNullOrEmpty(Request.Params["MAST_ROAD_CAT_CODE"]))
        //        {
        //            categoryCode = Convert.ToInt32(Request.Params["MAST_ROAD_CAT_CODE"]);
        //        }

        //        var jsonData = new
        //        {
        //            rows = objBAL.ListExistingRoadsITNOForInactiveBlocks(stateCode, districtCode, blockCode, categoryCode, ownerCode, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords, Request.Params["filters"]),
        //            total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
        //            page = Convert.ToInt32(Request.Params["page"]),
        //            records = totalRecords
        //        };
        //        return Json(jsonData, JsonRequestBehavior.AllowGet);

        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "existingRoads().GetExistingRoadsListITNOForInactiveBlocks()");
        //        return null;
        //    }
        //}

        //[Audit]
        //public JsonResult GetInactiveBlocksByDistrict(int districtCode)
        //{
        //    CommonFunctions objCommon = new CommonFunctions();
        //    try
        //    {
        //        return Json(new SelectList(objCommon.PopulateInactiveBlocks(districtCode, true), "Value", "Text"), JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
        //        return null;
        //    }
        //}


        //public ActionResult ShiftDetails(String parameter, String hash, String key)
        //{
        //    PMGSYEntities dbContext = new PMGSY.Models.PMGSYEntities();

        //    // IMasterDataEntryBAL masterDataEntryBAL = new MasterDataEntryBAL();

        //    PMGSY.DAL.MasterDataEntryDAL masterDataEntryDAL = new PMGSY.DAL.MasterDataEntryDAL();

        //    Dictionary<string, string> decryptedParameters = null;
        //    String[] encryptedParameters = null;

        //    string message = string.Empty;

        //    int districtCode = 0;
        //    int stateCode = 0;
        //    int blockCode = 0;
        //    int villageCode = 0;
        //    var en = parameter + "/" + hash + "/" + key;
        //    try
        //    {
        //        if (en != string.Empty)
        //        {
        //            // PMGSY.DAL.MasterDataEntryDAL masterDataEntryDAL = new PMGSY.DAL.MasterDataEntryDAL();

        //            ViewBag.EncryptedVillageCode = en;


        //            // encryptedParameters = id.Split('/');

        //            //if (encryptedParameters.Length == 3)
        //            //{
        //            //    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
        //            //    blockCode = Convert.ToInt32(decryptedParameters["BlockCode"].ToString());
        //            //    villageCode = Convert.ToInt32(decryptedParameters["VillageCode"].ToString());
        //            //}

        //            decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
        //            int ERCode = Convert.ToInt32(decryptedParameters["ERCode"].ToString());

        //            ViewBag.ERCode = ERCode;

        //            blockCode = dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == ERCode).Select(m => m.MAST_BLOCK_CODE).FirstOrDefault();
        //            districtCode = dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == ERCode).Select(m => m.MAST_DISTRICT_CODE).FirstOrDefault(); // dbContext.MASTER_BLOCK.Where(b => b.MAST_BLOCK_CODE == blockCode).Select(b => b.MAST_DISTRICT_CODE).FirstOrDefault();
        //            stateCode = dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == ERCode).Select(m => m.MAST_STATE_CODE).FirstOrDefault();// dbContext.MASTER_DISTRICT.Where(d => d.MAST_DISTRICT_CODE == districtCode).Select(d => d.MAST_STATE_CODE).FirstOrDefault();


        //            ViewData["StateList"] = new SelectList(masterDataEntryDAL.GetAllStates(false), "MAST_STATE_CODE", "MAST_STATE_NAME", stateCode); //GetAllStates(false);

        //            List<Models.MASTER_DISTRICT> districtList = new List<Models.MASTER_DISTRICT>();
        //            districtList = masterDataEntryDAL.GetAllDistrictsByStateCode(stateCode, false);
        //            //  districtList.RemoveAt(0);
        //            ViewData["DistrictList"] = new SelectList(districtList, "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME", districtCode);

        //            //Models.MASTER_DISTRICT master_district=districtList.ElementAt(0);
        //            List<Models.MASTER_BLOCK> blockList = new List<Models.MASTER_BLOCK>();
        //            blockList = masterDataEntryDAL.GetAllBlocksByDistrictCode(districtCode, false);
        //            //blockList.RemoveAt(0);

        //            blockList = blockList.Where(b => b.MAST_BLOCK_CODE != blockCode).ToList<Models.MASTER_BLOCK>();

        //            ViewData["BlockList"] = new SelectList(blockList, "MAST_BLOCK_CODE", "MAST_BLOCK_NAME");



        //            ViewBag.ExistingStateName = dbContext.MASTER_STATE.Where(s => s.MAST_STATE_CODE == stateCode).Select(s => s.MAST_STATE_NAME).FirstOrDefault();
        //            ViewBag.ExistingDistrictName = dbContext.MASTER_DISTRICT.Where(d => d.MAST_DISTRICT_CODE == districtCode).Select(d => d.MAST_DISTRICT_NAME).FirstOrDefault();
        //            ViewBag.ExistingBlockName = dbContext.MASTER_BLOCK.Where(b => b.MAST_BLOCK_CODE == blockCode).Select(b => b.MAST_BLOCK_NAME).FirstOrDefault();
        //            ViewBag.ExistingVillageName = "Test";// dbContext.MASTER_VILLAGE.Where(v => v.MAST_VILLAGE_CODE == villageCode).Select(v => v.MAST_VILLAGE_NAME).FirstOrDefault();

        //            return PartialView("ShiftDetails");
        //        }
        //        return PartialView("ShiftDetails");
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "ExistingRoads().ShiftDetails()");
        //        return PartialView("ShiftDetails");
        //    }
        //}


        //[HttpPost]
        //public ActionResult ShiftDetailsPostMethod(FormCollection frmCollection)
        //{
        //    bool status = false;
        //    string encryptedVillageCode = string.Empty;
        //    string newBlockCode = string.Empty;
        //    string newDistrictCode = string.Empty;
        //    string ERCode = string.Empty;
        //    IExistingRoadsDAL objDAL = new ExistingRoadsDAL();
        //    try
        //    {

        //        if (string.IsNullOrEmpty(frmCollection["EncryptedVillageCode"]) || string.IsNullOrEmpty(frmCollection["ddlSearchBlocks_ShiftVillage"]))
        //        {
        //            message = "Details not shifted successfully.";
        //            return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
        //        }

        //        encryptedVillageCode = frmCollection["EncryptedVillageCode"];

        //        newBlockCode = frmCollection["ddlSearchBlocks_ShiftVillage"];

        //        newDistrictCode = frmCollection["ddlSearchDistricts_ShiftVillage"];


        //        ERCode = frmCollection["ERCode"];

        //        if (objDAL.ShiftDetailsDAL(encryptedVillageCode, newBlockCode, newDistrictCode, ERCode))
        //        {

        //            message = "Details shifted successfully.";
        //            status = true;
        //        }
        //        else
        //        {
        //            message = "Details not shifted..";
        //        }

        //        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "ExistingRoads().ShiftDetailsPostMethod()");
        //        message = "Details not shifted.";
        //        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        #endregion 

        #region Not Feasible Roads under PMGSY III

        [HttpGet]
        public ActionResult NotFeasibleRoadsLayout()
        {
            try
            {
                DRRPMappingUnderPMGSY3 proposalViewModel = new DRRPMappingUnderPMGSY3();
                CommonFunctions objCommonFuntion = new CommonFunctions();
                ExistingRoadsDAL objDAL = new ExistingRoadsDAL();

                List<SelectListItem> lstTypes = new List<SelectListItem>();
                lstTypes.Insert(0, new SelectListItem { Value = "0", Text = "All" });
                lstTypes.Insert(1, new SelectListItem { Value = "N", Text = "New Connectivity" });
                lstTypes.Insert(2, new SelectListItem { Value = "U", Text = "Upgradation" });
                proposalViewModel.UserLevelID = PMGSYSession.Current.LevelId;
                proposalViewModel.COLLABORATIONS = objCommonFuntion.PopulateFundingAgency(true);
                proposalViewModel.DISTRICTS = objCommonFuntion.PopulateDistrict(PMGSYSession.Current.StateCode, false);
                if (PMGSYSession.Current.RoleCode == (int)PMGSY.Common.CommonFunctions.PMGSYSessionRoleDetails.PIU)
                {
                    proposalViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                }
                proposalViewModel.BLOCKS = objCommonFuntion.PopulateBlocks(PMGSYSession.Current.DistrictCode, false);
                proposalViewModel.PROPOSAL_STATUS = PopulateProposalStatus(PMGSYSession.Current.RoleCode);
                proposalViewModel.CONNECTIVITYLIST = lstTypes;
                proposalViewModel.BATCHS = objCommonFuntion.PopulateBatch(true);
                //  proposalViewModel.BATCHS.RemoveAt(0);
                // proposalViewModel.BATCHS.Insert(0, (new SelectListItem { Text = "All Batches", Value = "0", Selected = true }));
                proposalViewModel.PROPOSAL_TYPES = objCommonFuntion.PopulateProposalTypes();
                proposalViewModel.Years = PopulateYear(0, false, false);
                proposalViewModel.IMS_YEAR = DateTime.Now.Year;
                proposalViewModel.RoleID = PMGSYSession.Current.RoleCode;
                proposalViewModel.isPMGSY3 = objDAL.CheckPMGSY3DAL(PMGSYSession.Current.StateCode);
                return View(proposalViewModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoads.NotFeasibleRoadsLayout");
                return null;
            }
        }

        [HttpPost]
        public ActionResult GetNotFeasibleRoadsList(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, Convert.ToInt32(rows), sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                int IMS_YEAR = Convert.ToInt32(Request.Params["IMS_YEAR"]);
                int MAST_BLOCK_ID = Convert.ToInt32(Request.Params["MAST_BLOCK_ID"]);
                int MAST_DISTRICT_ID = Convert.ToInt32(Request.Params["MAST_DISTRICT_ID"]);
                int IMS_BATCH = Convert.ToInt32(Request.Params["IMS_BATCH"]);
                int IMS_STREAMS = Convert.ToInt32(Request.Params["IMS_STREAM"]);
                string IMS_PROPOSAL_TYPE = Request.Params["IMS_PROPOSAL_TYPE"];
                string IMS_PROPOSAL_STATUS = Request.Params["IMS_PROPOSAL_STATUS"];
                string IMS_UPGRADE_CONNECT = Request.Params["IMS_UPGRADE_CONNECT"];
                long totalRecords;

                var jsonData = new
                {
                    rows = objBAL.GetNotFeasibleRoadsListBAL(page.Value, rows, sidx, sord, out totalRecords, PMGSYSession.Current.StateCode, MAST_DISTRICT_ID, IMS_YEAR, MAST_BLOCK_ID, IMS_BATCH, IMS_STREAMS, IMS_PROPOSAL_TYPE, PMGSYSession.Current.AdminNdCode, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoads.GetNotFeasibleRoadsList()");
                return null;
            }
        }

        [HttpGet]
        public ActionResult AddReason(String parameter, String hash, String key)
        {
           
            try
            {
                string[] decryptedParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });

                if (parameter == null || hash == null || key == null) 
                {
                    return null;
                }

                CommonFunctions objCommon = new CommonFunctions();
                ExistingRoadsDAL objDAL = new ExistingRoadsDAL();
                MapNotFeasibleRoads model = new MapNotFeasibleRoads();
                PMGSYEntities dbContext = new PMGSYEntities();
                model.PLAN_CN_ROAD_CODE = Convert.ToInt32(decryptedParameters[0].Split('$')[0]);

                model = objCommon.GetDetails(model.PLAN_CN_ROAD_CODE);
                model.lstBlocks = objCommon.PopulateBlocks(model.District);
                model.ReasonCode = 0;
                model.ReasonList = objCommon.PopulateReaonsPMGSY3();

                model.Year = Convert.ToInt32(decryptedParameters[0].Split('$')[1]);
                model.Batch = Convert.ToInt32(decryptedParameters[0].Split('$')[2]);

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoads.AddReason()");
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MapRoadDetails(MapNotFeasibleRoads model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool status = objBAL.MapRoadDetailsBAL(model);
                    if (status == true)
                    {
                        return Json(new { success = true, message = "Details Saved Successfully." });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Request Remarks are already mapped against this Work." });
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoads.MapRoadDetails()");
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        [HttpPost]
        public ActionResult BlockDetailsForNotFeasibleRoads(FormCollection frmCollection)
        {
            try
            {
                CommonFunctions objCommonFunctions = new CommonFunctions();
                List<SelectListItem> list = objCommonFunctions.PopulateBlocks(Convert.ToInt32(frmCollection["DistrictCode"]), false);
                //   list.Find(x => x.Value == "-1").Value = "0";
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoads.BlockDetailsForNotFeasibleRoads()");
                return null;
            }
        }
        #endregion


        #region  Existing Road Shift

        public ActionResult ListExistingRoadsITNOForInactiveBlocks()
        {
            try
            {
                IExistingRoadsDAL objDAL = new ExistingRoadsDAL();
                List<SelectListItem> lstDefault = new List<SelectListItem>();
                CommonFunctions objCommon = new CommonFunctions();
                //lstDefault.Insert(0, new SelectListItem { Value = "0", Text = "All" });

                lstDefault = objCommon.PopulateDistrict(PMGSYSession.Current.StateCode, false);

                List<MASTER_BLOCK> blockList = objDAL.GetAllBlockNames(PMGSYSession.Current.DistrictCode);

                List<SelectListItem> StateListITNO = new List<SelectListItem>();
                List<SelectListItem> BlockListITNO = new List<SelectListItem>();
                BlockListITNO.Insert(0, new SelectListItem { Value = "0", Text = "Select Block", Selected = true });
                StateListITNO.Insert(0, new SelectListItem { Value = PMGSYSession.Current.StateCode.ToString(), Text = PMGSYSession.Current.StateName, Selected = true });

                if (PMGSYSession.Current.RoleCode == 25 || PMGSYSession.Current.RoleCode == 53 || PMGSYSession.Current.RoleCode == 65 || PMGSYSession.Current.RoleCode == 36)//Changes by SAMMED A. PATIL for mordviewuser
                {
                    ViewData["States"] = StateListITNO;//objCommon.PopulateStates(true);
                    ViewData["Districts"] = lstDefault;
                    ViewData["blockList"] = BlockListITNO;
                }
                else if (PMGSYSession.Current.RoleCode == 2)
                {
                    ViewData["Districts"] = objCommon.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                    ViewData["blockList"] = lstDefault;
                }
                else
                {
                    ViewData["blockList"] = new SelectList(blockList, "MAST_BLOCK_CODE", "MAST_BLOCK_NAME");
                }

                List<SelectListItem> SchemeListITNO = new List<SelectListItem>();
                SchemeListITNO.Insert(0, new SelectListItem { Value = "0", Text = "Select Scheme", Selected = true });
                SchemeListITNO.Insert(1, new SelectListItem { Value = "1", Text = "PMGSY I" });
                SchemeListITNO.Insert(2, new SelectListItem { Value = "2", Text = "PMGSY II" });
                ViewData["SchemeListViewBag"] = SchemeListITNO;


                List<MASTER_ROAD_CATEGORY> roadCategoryList = objDAL.GetAllRoadCategory();
                roadCategoryList.Insert(0, new MASTER_ROAD_CATEGORY() { MAST_ROAD_CAT_CODE = 0, MAST_ROAD_CAT_NAME = "-- All --" });

                ViewData["roadCategoryList"] = new SelectList(roadCategoryList, "MAST_ROAD_CAT_CODE", "MAST_ROAD_CAT_NAME");

                if (blockList.Count > 0)
                {
                    ViewData["isUnlocked"] = objDAL.CheckUnlockedDAL(blockList.Select(x => x.MAST_BLOCK_CODE).First());

                    ///PMGSY3
                    ExistingRoadsDAL objDRRPDAL = new ExistingRoadsDAL();
                    ViewData["isPMGSY3"] = objDRRPDAL.CheckPMGSY3DAL(PMGSYSession.Current.StateCode);
                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "ExistingRoad().ListExistingRoadsITNOForInactiveBlocks()");
                ViewData["blockList"] = null;
                ViewData["roadCategoryList"] = null;
            }
            return View();
        }

        [HttpPost]
        public ActionResult GetERShiftList(int? page, int? rows, string sidx, string sord)
        {
            String searchParameters = String.Empty;
            long totalRecords;

            int blockCode = 0;
            int SchemeCode = 0;
            int ownerCode = 0;
            string roadName = string.Empty;
            int stateCode = 0;
            int districtCode = 0;

            try
            {

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["stateCode"]))
                {
                    stateCode = Convert.ToInt32(Request.Params["stateCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["districtCode"]))
                {
                    districtCode = Convert.ToInt32(Request.Params["districtCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["MAST_BLOCK_CODE"]))
                {
                    blockCode = Convert.ToInt32(Request.Params["MAST_BLOCK_CODE"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["SCHEME_CODE_DETAILS"]))
                {
                    SchemeCode = Convert.ToInt32(Request.Params["SCHEME_CODE_DETAILS"]);
                }

                var jsonData = new
                {
                    rows = objBAL.ListExistingRoadsForShiftBAL(stateCode, districtCode, blockCode, SchemeCode, ownerCode, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords, Request.Params["filters"]),
                    total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                    page = Convert.ToInt32(Request.Params["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoads().GetERShiftList()");
                return null;
            }
        }


        public ActionResult ERShiftDetailsGet(String parameter, String hash, String key)
        {
            PMGSYEntities dbContext = new PMGSY.Models.PMGSYEntities();

            // IMasterDataEntryBAL masterDataEntryBAL = new MasterDataEntryBAL();

            PMGSY.DAL.MasterDataEntryDAL masterDataEntryDAL = new PMGSY.DAL.MasterDataEntryDAL();

            Dictionary<string, string> decryptedParameters = null;
            String[] encryptedParameters = null;

            string message = string.Empty;

            int districtCode = 0;
            int stateCode = 0;
            int blockCode = 0;
            int villageCode = 0;
            var en = parameter + "/" + hash + "/" + key;
            try
            {
                if (en != string.Empty)
                {
                    // PMGSY.DAL.MasterDataEntryDAL masterDataEntryDAL = new PMGSY.DAL.MasterDataEntryDAL();

                    ViewBag.EncryptedVillageCode = en;


                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                    int ERCode = Convert.ToInt32(decryptedParameters["ERCode"].ToString());

                    ViewBag.ERCode = ERCode;

                    blockCode = dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == ERCode).Select(m => m.MAST_BLOCK_CODE).FirstOrDefault();
                    districtCode = dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == ERCode).Select(m => m.MAST_DISTRICT_CODE).FirstOrDefault(); // dbContext.MASTER_BLOCK.Where(b => b.MAST_BLOCK_CODE == blockCode).Select(b => b.MAST_DISTRICT_CODE).FirstOrDefault();
                    stateCode = dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == ERCode).Select(m => m.MAST_STATE_CODE).FirstOrDefault();// dbContext.MASTER_DISTRICT.Where(d => d.MAST_DISTRICT_CODE == districtCode).Select(d => d.MAST_STATE_CODE).FirstOrDefault();


                    ViewData["StateList"] = new SelectList(masterDataEntryDAL.GetAllStates(false), "MAST_STATE_CODE", "MAST_STATE_NAME", stateCode); //GetAllStates(false);

                    List<Models.MASTER_DISTRICT> districtList = new List<Models.MASTER_DISTRICT>();
                    districtList = masterDataEntryDAL.GetAllDistrictsByStateCode(stateCode, false);
                    //  districtList.RemoveAt(0);
                    ViewData["DistrictList"] = new SelectList(districtList, "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME", districtCode);

                    //Models.MASTER_DISTRICT master_district=districtList.ElementAt(0);
                    List<Models.MASTER_BLOCK> blockList = new List<Models.MASTER_BLOCK>();
                    blockList = masterDataEntryDAL.GetAllBlocksByDistrictCode(districtCode, false);
                    //blockList.RemoveAt(0);

                    blockList = blockList.Where(b => b.MAST_BLOCK_CODE != blockCode).ToList<Models.MASTER_BLOCK>();

                    ViewData["BlockList"] = new SelectList(blockList, "MAST_BLOCK_CODE", "MAST_BLOCK_NAME");



                    ViewBag.ExistingStateName = dbContext.MASTER_STATE.Where(s => s.MAST_STATE_CODE == stateCode).Select(s => s.MAST_STATE_NAME).FirstOrDefault();
                    ViewBag.ExistingDistrictName = dbContext.MASTER_DISTRICT.Where(d => d.MAST_DISTRICT_CODE == districtCode).Select(d => d.MAST_DISTRICT_NAME).FirstOrDefault();
                    ViewBag.ExistingBlockName = dbContext.MASTER_BLOCK.Where(b => b.MAST_BLOCK_CODE == blockCode).Select(b => b.MAST_BLOCK_NAME).FirstOrDefault();
                    ViewBag.ExistingVillageName = "Test";// dbContext.MASTER_VILLAGE.Where(v => v.MAST_VILLAGE_CODE == villageCode).Select(v => v.MAST_VILLAGE_NAME).FirstOrDefault();

                    return PartialView("ERShiftDetailsGet");
                }
                return PartialView("ERShiftDetailsGet");
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoads().ERShiftDetailsGet()");
                return PartialView("ShiftDetailsPMGSYII");
            }
        }


        [HttpPost]
        public ActionResult ERShiftDetailsPost(FormCollection frmCollection)
        {
            bool status = false;
            string encryptedVillageCode = string.Empty;
            string newBlockCode = string.Empty;
            string newDistrictCode = string.Empty;
            string ERCode = string.Empty;
            IExistingRoadsDAL objDAL = new ExistingRoadsDAL();
            try
            {

                if (string.IsNullOrEmpty(frmCollection["EncryptedVillageCode"]) || string.IsNullOrEmpty(frmCollection["ddlSearchBlocks_ShiftVillage"]))
                {
                    message = "Details not shifted successfully.";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                encryptedVillageCode = frmCollection["EncryptedVillageCode"];

                newBlockCode = frmCollection["ddlSearchBlocks_ShiftVillage"];

                newDistrictCode = frmCollection["ddlSearchDistricts_ShiftVillage"];


                ERCode = frmCollection["ERCode"];

                if (objDAL.ShiftERDetailsDAL(encryptedVillageCode, newBlockCode, newDistrictCode, ERCode))
                {

                    message = "Details shifted successfully.";
                    status = true;
                }
                else
                {
                    message = "Details not shifted..";
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoads().ERShiftDetailsPost()");
                message = "Details not shifted.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [Audit]
        public JsonResult GetInactiveBlocksByDistrictForShifting(int districtCode)
        {
            CommonFunctions objCommon = new CommonFunctions();
            try
            {
                return Json(new SelectList(objCommon.PopulateInactiveBlocks(districtCode, true), "Value", "Text"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoads().GetInactiveBlocksByDistrictForShifting()");
                return null;
            }
        }


        [Audit]
        public JsonResult GetDistrictByStateForShifting(int stateCode)
        {
            CommonFunctions objCommon = new CommonFunctions();
            try
            {
                return Json(new SelectList(objCommon.PopulateDistrict(stateCode, true), "Value", "Text"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoads().GetDistrictByStateForShifting()");
                return null;
            }
        }
        #endregion


        #region Delete / Unlock Exempted Roads

        [HttpGet]
        public ActionResult DeleteNotFeasibleRoadsLayout()
        {
            try
            {
                DRRPMappingUnderPMGSY3 proposalViewModel = new DRRPMappingUnderPMGSY3();
                CommonFunctions objCommonFuntion = new CommonFunctions();
                ExistingRoadsDAL objDAL = new ExistingRoadsDAL();

                List<SelectListItem> lstTypes = new List<SelectListItem>();
                lstTypes.Insert(0, new SelectListItem { Value = "0", Text = "All" });
                lstTypes.Insert(1, new SelectListItem { Value = "N", Text = "New Connectivity" });
                lstTypes.Insert(2, new SelectListItem { Value = "U", Text = "Upgradation" });
                proposalViewModel.UserLevelID = PMGSYSession.Current.LevelId;
                proposalViewModel.COLLABORATIONS = objCommonFuntion.PopulateFundingAgency(true);


                proposalViewModel.STATES = objCommonFuntion.PopulateStates(true);
                proposalViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;

                if (proposalViewModel.MAST_STATE_CODE == 0 && PMGSYSession.Current.RoleCode != 2)
                {
                    proposalViewModel.DISTRICTS.Insert(0, (new SelectListItem { Text = "All Districts", Value = "0", Selected = true }));
                }
                proposalViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;

                proposalViewModel.BLOCKS = objCommonFuntion.PopulateBlocks(PMGSYSession.Current.DistrictCode, false);
                proposalViewModel.PROPOSAL_STATUS = PopulateProposalStatus(PMGSYSession.Current.RoleCode);
                proposalViewModel.CONNECTIVITYLIST = lstTypes;
                proposalViewModel.BATCHS = objCommonFuntion.PopulateBatch(false);
                //  proposalViewModel.BATCHS.RemoveAt(0);
                // proposalViewModel.BATCHS.Insert(0, (new SelectListItem { Text = "All Batches", Value = "0", Selected = true }));
                proposalViewModel.PROPOSAL_TYPES = objCommonFuntion.PopulateProposalTypes();
                proposalViewModel.Years = PopulateYear(0, false, false);
                proposalViewModel.IMS_YEAR = DateTime.Now.Year;
                proposalViewModel.RoleID = PMGSYSession.Current.RoleCode;
                proposalViewModel.isPMGSY3 = objDAL.CheckPMGSY3DAL(PMGSYSession.Current.StateCode);
                return View(proposalViewModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoads.DeleteNotFeasibleRoadsLayout");
                return null;
            }
        }

        [HttpPost]
        public ActionResult GetNotFeasibleRoadsListForDeletion(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, Convert.ToInt32(rows), sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                int IMS_YEAR = Convert.ToInt32(Request.Params["IMS_YEAR"]);
                int MAST_BLOCK_ID = Convert.ToInt32(Request.Params["MAST_BLOCK_ID"]);
                int MAST_DISTRICT_ID = Convert.ToInt32(Request.Params["MAST_DISTRICT_ID"]);
                int IMS_BATCH = Convert.ToInt32(Request.Params["IMS_BATCH"]);
                int IMS_STREAMS = Convert.ToInt32(Request.Params["IMS_STREAM"]);
                string IMS_PROPOSAL_TYPE = Request.Params["IMS_PROPOSAL_TYPE"];
                string IMS_PROPOSAL_STATUS = Request.Params["IMS_PROPOSAL_STATUS"];
                string IMS_UPGRADE_CONNECT = Request.Params["IMS_UPGRADE_CONNECT"];
                long totalRecords;

                ExistingRoadsDAL objDAL = new ExistingRoadsDAL();

                var jsonData = new
                {
                    rows = objDAL.GetNotFeasibleRoadsListDALforDeletion(page.Value, rows, sidx, sord, out totalRecords, PMGSYSession.Current.StateCode, MAST_DISTRICT_ID, IMS_YEAR, MAST_BLOCK_ID, IMS_BATCH, IMS_STREAMS, IMS_PROPOSAL_TYPE, PMGSYSession.Current.AdminNdCode, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoads.GetNotFeasibleRoadsListForDeletion()");
                return null;
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult DeleteTRMRLDetails(String hash, String parameter, String key, string roadCode)
        {
            Dictionary<string, string> decryptedParameters = null;
            string message = string.Empty;
            ExistingRoadsDAL objDAL = new ExistingRoadsDAL();
            int coreNetworkCode = Convert.ToInt32(roadCode);
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptedParameters.Count() > 0)
                {
                    if (!objDAL.DeleteTRMRLDetailsDAL(Convert.ToInt32(decryptedParameters["CNCode"].ToString()), out message))
                    {
                        ModelState.AddModelError(String.Empty, "Details not deleted.");
                        message = message == String.Empty ? "Details can not be deleted" : message;
                        return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { success = true, message = "Details deleted successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoadsController.DeleteTRMRLDetails()");
                return Json(new { success = false, message = "Details can not be deleted" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        [Audit]
        public ActionResult DeleteTRMRLExemptedDetails(String hash, String parameter, String key, string roadCode)
        {
            Dictionary<string, string> decryptedParameters = null;
            string message = string.Empty;
            ExistingRoadsDAL objDAL = new ExistingRoadsDAL();
            //int coreNetworkCode = Convert.ToInt32(roadCode);
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptedParameters.Count() > 0)
                {
                    int TrMrlExemptionId = Convert.ToInt32(decryptedParameters["TRMRLCode"].ToString());
                    if (!objDAL.DeleteTRMRLExemptedDetailsDAL(Convert.ToInt32(decryptedParameters["TRMRLCode"].ToString()), out message))
                    {
                        ModelState.AddModelError(String.Empty, "Details not deleted.");
                        message = message == String.Empty ? "Details can not be deleted" : message;
                        return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { success = true, message = "Details deleted successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoadsController.DeleteTRMRLExemptedDetails()");
                return Json(new { success = false, message = "Details can not be deleted" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Delete CUPL Roads
        [HttpGet]
        public ActionResult UnlockCUPLLayout()
        {
            try
            {
                DRRPMappingUnderPMGSY3 proposalViewModel = new DRRPMappingUnderPMGSY3();
                CommonFunctions objCommonFuntion = new CommonFunctions();
                ExistingRoadsDAL objDAL = new ExistingRoadsDAL();

                List<SelectListItem> lstTypes = new List<SelectListItem>();
                lstTypes.Insert(0, new SelectListItem { Value = "0", Text = "All" });
                lstTypes.Insert(1, new SelectListItem { Value = "N", Text = "New Connectivity" });
                lstTypes.Insert(2, new SelectListItem { Value = "U", Text = "Upgradation" });
                proposalViewModel.UserLevelID = PMGSYSession.Current.LevelId;
                proposalViewModel.COLLABORATIONS = objCommonFuntion.PopulateFundingAgency(true);


                proposalViewModel.STATES = objCommonFuntion.PopulateStates(true);
                proposalViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;

                if (proposalViewModel.MAST_STATE_CODE == 0 && PMGSYSession.Current.RoleCode != 2)
                {
                    proposalViewModel.DISTRICTS.Insert(0, (new SelectListItem { Text = "All Districts", Value = "0", Selected = true }));
                }
                proposalViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;

                proposalViewModel.BLOCKS = objCommonFuntion.PopulateBlocks(PMGSYSession.Current.DistrictCode, false);
                proposalViewModel.PROPOSAL_STATUS = PopulateProposalStatus(PMGSYSession.Current.RoleCode);
                proposalViewModel.CONNECTIVITYLIST = lstTypes;
                proposalViewModel.BATCHS = objCommonFuntion.PopulateBatch(false);
                //  proposalViewModel.BATCHS.RemoveAt(0);
                // proposalViewModel.BATCHS.Insert(0, (new SelectListItem { Text = "All Batches", Value = "0", Selected = true }));
                proposalViewModel.PROPOSAL_TYPES = objCommonFuntion.PopulateProposalTypes();
                proposalViewModel.Years = PopulateYear(0, false, false);
                proposalViewModel.IMS_YEAR = DateTime.Now.Year;
                proposalViewModel.RoleID = PMGSYSession.Current.RoleCode;
                proposalViewModel.isPMGSY3 = objDAL.CheckPMGSY3DAL(PMGSYSession.Current.StateCode);
                return View(proposalViewModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoads.UnlockCUPLLayout");
                return null;
            }
        }

        [HttpPost]
        public ActionResult UnlockCUPLList(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, Convert.ToInt32(rows), sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                int IMS_YEAR = Convert.ToInt32(Request.Params["IMS_YEAR"]);
                int MAST_BLOCK_ID = Convert.ToInt32(Request.Params["MAST_BLOCK_ID"]);
                int MAST_DISTRICT_ID = Convert.ToInt32(Request.Params["MAST_DISTRICT_ID"]);
                int IMS_BATCH = Convert.ToInt32(Request.Params["IMS_BATCH"]);
                int IMS_STREAMS = Convert.ToInt32(Request.Params["IMS_STREAM"]);
                string IMS_PROPOSAL_TYPE = Request.Params["IMS_PROPOSAL_TYPE"];
                string IMS_PROPOSAL_STATUS = Request.Params["IMS_PROPOSAL_STATUS"];
                string IMS_UPGRADE_CONNECT = Request.Params["IMS_UPGRADE_CONNECT"];
                long totalRecords;

                ExistingRoadsDAL objDAL = new ExistingRoadsDAL();

                var jsonData = new
                {
                    rows = objDAL.UnlockCUPLListDAL(page.Value, rows, sidx, sord, out totalRecords, PMGSYSession.Current.StateCode, MAST_DISTRICT_ID, IMS_YEAR, MAST_BLOCK_ID, IMS_BATCH, IMS_STREAMS, IMS_PROPOSAL_TYPE, PMGSYSession.Current.AdminNdCode, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoads.UnlockCUPLList()");
                return null;
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult DeleteCUPLDetails(String id)
        {
            ExistingRoadsDAL objDAL = new ExistingRoadsDAL();

            int BlockCodeID = 0;
            int YearCodeID = 0;
            int BatchCodeID = 0;
            string message = string.Empty;

            try
            {

                if (!String.IsNullOrEmpty(id))
                {
                    if (id.Split('$').Count() == 3)
                    {
                        BlockCodeID = Convert.ToInt32(id.Split('$')[0]);
                        YearCodeID = Convert.ToInt32(id.Split('$')[1]);
                        BatchCodeID = Convert.ToInt32(id.Split('$')[2]);
                    }
                }

                if (id.Split('$').Count() > 0)
                {
                    if (!objDAL.DeleteCUPLDetailsDAL(BlockCodeID, YearCodeID, BatchCodeID, out message))
                    {
                        ModelState.AddModelError(String.Empty, "Details not deleted.");
                        message = message == String.Empty ? "Details can not be deleted" : message;
                        return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = true, message = "Details deleted Successfully.." }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Unable to delete details." }, JsonRequestBehavior.AllowGet);
                }
               
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoadsController.DeleteCUPLDetails()");
                return Json(new { success = false, message = "Details can't be deleted" }, JsonRequestBehavior.AllowGet);
            }
        }

        //[HttpPost]
        //[Audit]
        //public ActionResult DeleteCUPLDetails(String hash, String parameter, String key, string roadCode)
        //{
        //    Dictionary<string, string> decryptedParameters = null;
        //    string message = string.Empty;
        //    ExistingRoadsDAL objDAL = new ExistingRoadsDAL();
        //    int coreNetworkCode = Convert.ToInt32(roadCode);
        //    try
        //    {
        //        decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
        //        if (decryptedParameters.Count() > 0)
        //        {
        //            if (!objDAL.DeleteCUPLDetailsDAL(Convert.ToInt32(decryptedParameters["CNCode"].ToString()), out message))
        //            {
        //                ModelState.AddModelError(String.Empty, "Details not deleted.");
        //                message = message == String.Empty ? "Details can not be deleted" : message;
        //                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
        //            }
        //        }
        //        return Json(new { success = true, message = "Details deleted successfully" }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "ExistingRoadsController.DeleteCUPLDetails()");
        //        return Json(new { success = false, message = "Details can not be deleted" }, JsonRequestBehavior.AllowGet);
        //    }
        //}
        #endregion

        #region Common
        public ActionResult DistrictDetailsForDefinalizationPCI(FormCollection frmCollection)
        {
            try
            {
                CommonFunctions objCommonFunctions = new CommonFunctions();
                List<SelectListItem> list = objCommonFunctions.PopulateDistrict(Convert.ToInt32(frmCollection["StateCode"]), false);
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoadsController.DistrictDetailsForDefinalizationPCI()");
                return null;
            }
        }
        #endregion


        #region PCI Block and District Definalization at MORD

        [HttpGet]
        public ActionResult DeFinalizePCIMGSY3atMORDLayout()
        {
            try
            {
                DeFinalizeDRRPPMGSY3atMORDModel packageAgreement = new DeFinalizeDRRPPMGSY3atMORDModel();
                CommonFunctions commonFunctions = new CommonFunctions();
                packageAgreement.StateName = PMGSYSession.Current.StateCode == 0 ? "0" : PMGSYSession.Current.StateName.Trim();
                packageAgreement.DistName = PMGSYSession.Current.DistrictCode == 0 ? "0" : PMGSYSession.Current.DistrictName.Trim();
                packageAgreement.Mast_State_Code = PMGSYSession.Current.StateCode;
                packageAgreement.Mast_District_Code = PMGSYSession.Current.DistrictCode;
                packageAgreement.DistrictCode = PMGSYSession.Current.DistrictCode;
                packageAgreement.StateList = commonFunctions.PopulateStates(true);
                packageAgreement.StateList.Find(x => x.Value == packageAgreement.StateCode.ToString()).Selected = true;
                packageAgreement.DistrictList = new List<SelectListItem>();
                if (packageAgreement.StateCode == 0 && PMGSYSession.Current.RoleCode != 2)
                {
                    packageAgreement.DistrictList.Insert(0, (new SelectListItem { Text = "All Districts", Value = "0", Selected = true }));
                }
                else
                {
                    packageAgreement.DistrictList = commonFunctions.PopulateDistrictForSRRDA(PMGSYSession.Current.StateCode, true);
                }
                return View(packageAgreement);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoadsController.DeFinalizePCIMGSY3atMORDLayout()");
                return null;
            }
        }

        [HttpGet]
        [Audit]
        public ActionResult GetBlockListForPCIunderPMGSYIII(int? page, int? rows, string sidx, string sord)
        {
            ExistingRoadsDAL objDAL = new ExistingRoadsDAL();
            String searchParameters = String.Empty;
            long totalRecords;

            int districtCode = 0;
            bool isAllBlockFinalized = false;
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
                    rows = objDAL.GetListForDefinalizePCIatMORDunderPMGSYIII(districtCode, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords, Request.Params["filters"], ref isAllBlockFinalized, Convert.ToInt32(Request.Params["statecode"])),

                    total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,

                    page = Convert.ToInt32(Request.Params["page"]),

                    records = totalRecords,

                    isAllBlockFinalized = isAllBlockFinalized
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoadsController.GetBlockListForPCIunderPMGSYIII()");
                return null;
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult DeFinalizePCIBlockatMORD(String parameter, String hash, String key)
        {
            ExistingRoadsDAL objDAL = new ExistingRoadsDAL();
            string message = string.Empty;
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                int blockCode = Convert.ToInt32(decryptedParameters["BlockCode"]);
                if (decryptedParameters.Count() > 0)
                {
                    bool status = objDAL.DeFinalizePCIPMGSY3DAL(blockCode, ref message);
                    return Json(new { success = status, message = message.Trim() });
                }
                else
                {
                    return Json(new { success = false, message = "Invalid request" });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoadsController.DeFinalizePCIBlockatMORD()");
                return Json(new { success = false, message = "Error occurred while processing the request." });
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult DeFinalizePCIDistrict(DeFinalizeDRRPPMGSY3atMORDModel model)
        {
            ExistingRoadsDAL objDAL = new ExistingRoadsDAL();
            string message = string.Empty;
            try
            {
                bool status = objDAL.DeFinalizePCIDistrictPMGSY3DAL(model.DistrictCode, ref message);
                return Json(new { success = status, message = message.Trim() });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoadsController.DeFinalizePCIDistrict()");
                return Json(new { success = false, message = "Error occurred while processing the request." });
            }
        }
        // Following Function is already in this file. Thats why its commented.

        //public ActionResult DistrictDetailsForDefinalizationPCI(FormCollection frmCollection)
        //{
        //    try
        //    {
        //        CommonFunctions objCommonFunctions = new CommonFunctions();
        //        List<SelectListItem> list = objCommonFunctions.PopulateDistrict(Convert.ToInt32(frmCollection["StateCode"]), false);
        //        return Json(list, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "ExistingRoadsController.DistrictDetailsForDefinalizationPCI()");
        //        return null;
        //    }
        //}

        #endregion

        #region Hab CSV Upload

        public ActionResult GetHabCSVValue()
        {
            bool success = false;
            bool flg = false;
            string filePath = string.Empty;
            string fileSaveExt = string.Empty;
            int fileValidSize = 0;
            string message = string.Empty;
            HttpPostedFileBase file = null;
            var blockcode = Convert.ToInt32(Request.Form["BlockCode"]);
            try
            {
                if (Request.Files.AllKeys.Count() <= 0)
                {
                    return Json(new { success = false, message = "File not selected" });
                }

                filePath = ConfigurationManager.AppSettings["DRRP_HAB_TRACE_CSV_PATH"];
                objDAL = new ExistingRoadsDAL();

                if (Request.Files.Count > 0)
                {
                    file = Request.Files[0];

                    fileSaveExt = Path.GetExtension(file.FileName);//file.ContentType;
                    fileSaveExt = fileSaveExt.Contains('.') ? fileSaveExt.Trim().Remove(0, 1) : fileSaveExt;
                    fileValidSize = file.ContentLength;

                    if (fileSaveExt.ToLower() != "csv")
                    {
                        return Json(new { success = false, message = "Please select a valid csv file" });
                    }
                    flg = objDAL.UploadHabCSVDAL(file, ",", ref message, blockcode);
                    if (!flg)
                    {
                        return Json(new { message = "Invalid File, Please upload valid file for the selected block", success = false }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoads.GetHabCSVValue");
            }
            return Json(new { success = flg, message = message }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult HabCsvFileUploadView(string id)
        {
            try
            {
                PMGSYEntities dbcontext = new PMGSYEntities();
                FileUploadModel model = new FileUploadModel();
                model.BlockCode = Convert.ToInt32(id);
                model.ErrorMessage = string.Empty;
                if (PMGSYSession.Current.RoleCode == 22)
                {
                    model.NumberofPdfs = 1;
                }
                else
                {

                    bool isRecordPresent = dbcontext.MAST_HAB_CSV_PMGSY3.Where(x => x.MAST_BLOCK_CODE == model.BlockCode && x.MAST_HAB_CSV_FILE_NAME != null).Any();
                    model.NumberofPdfs = isRecordPresent == true ? 1 : 0;
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoads/HabCsvFileUploadView");
                return null;
            }
        }

        [HttpPost]
        public JsonResult ListHabMultipleCSVFiles(FormCollection formCollection)
        {
            try
            {
                ExistingRoadsDAL objDAL = new ExistingRoadsDAL();
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                int blokcode = Convert.ToInt32(Request["blockcode"]);
                int totalRecords;
                var jsonData = new
                {
                    rows = objDAL.GetTraceHabCSVFilesListDAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, blokcode),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoads/ListHabMultipleCSVFiles");
                return null;
            }
        }


        [HttpPost]
        public JsonResult DeleteHabCSVFileDetails(string id)
        {
            PMGSYEntities dbContext = new PMGSYEntities();

            try
            {

                bool isUpdated = false;
                string FILE_NAME = Request.Params["FILE_NAME"];

                string[] arrParam = Request.Params["FileID"].Split('$');

                int FILE_ID = Convert.ToInt32(arrParam[0].Split('&')[0]);
                int blockcode = Convert.ToInt32(arrParam[0].Split('&')[1]);
                string filename = string.Empty;
                var filedetails = dbContext.MAST_HAB_CSV_PMGSY3.Where(obj => obj.MAST_HAB_CSV_FILE_ID == FILE_ID && obj.MAST_BLOCK_CODE == blockcode).FirstOrDefault();

                var fileDetailsFromScoreTable = dbContext.MAST_HAB_DETAILS_CSV_PMGSY3.Where(obj => obj.MAST_HAB_CSV_FILE_ID == FILE_ID).ToList();



                // If the record is unlocked then this habitation csv can be deleted even if TR / MRL is created . As per suggestion by Pankaj Sir.  04 Nov 2020 
                //In table omms.MAST_TRACEFILE_PMGSY3 ,  TRACEFILE_FINALIZE must be N




                //// Block Code
                //if (dbContext.PLAN_ROAD.Any(m => m.MAST_BLOCK_CODE == blockcode && m.MAST_PMGSY_SCHEME==4))
                //{
                //    return Json(new { Success = false, ErrorMessage = "TR / MRL road is created against this Block. Habitation CSV can not be deleted." });
                //}




                string PhysicalPath = string.Empty;
                if (filedetails != null)
                {
                    filename = filedetails.MAST_HAB_CSV_FILE_NAME;
                    PhysicalPath = ConfigurationManager.AppSettings["DRRP_HAB_TRACE_CSV_PATH"];
                    filedetails.MAST_HAB_CSV_FILE_NAME = null;
                    filedetails.MAST_HAB_CSV_FILE_UPLOAD_DATE = null;
                    filedetails.MAST_HAB_CSV_FILE_FINALIZED = "N";
                    filedetails.MAST_HAB_CSV_FILE_FINALIZED_DATE = null;
                    filedetails.MAST_HAB_CSV_HAB_FINALIZED_BY = null;
                    dbContext.Entry(filedetails).State = System.Data.Entity.EntityState.Modified;
                    isUpdated = true;
                }

                //   using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope())
                // {
                if (fileDetailsFromScoreTable != null)
                {
                    //string constr = ConfigurationManager.AppSettings["HAB_CSV_UPLOAD_CONNECTION_STRING"];
                    //System.Data.SqlClient.SqlConnection destinationConnection = new System.Data.SqlClient.SqlConnection(constr);

                    //System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("delete from omms.MAST_HAB_DETAILS_CSV_PMGSY3 where MAST_HAB_CSV_FILE_ID='" + Convert.ToInt32(FILE_ID) + "'", destinationConnection);
                    //destinationConnection.Open();
                    //cmd.ExecuteNonQuery();
                    //destinationConnection.Close();
                    dbContext.Configuration.AutoDetectChangesEnabled = false;
                    dbContext.MAST_HAB_DETAILS_CSV_PMGSY3.DeleteMany(fileDetailsFromScoreTable);
                    dbContext.Configuration.AutoDetectChangesEnabled = true;
                }

                PhysicalPath = Path.Combine(PhysicalPath, filename);
                if (isUpdated && filedetails != null)
                {
                    try
                    {
                        System.IO.File.Delete(PhysicalPath);
                        dbContext.SaveChanges();

                        return Json(new { Success = true, ErrorMessage = "" });
                    }
                    catch (Exception ex)
                    {
                        return Json(new { Success = false, ErrorMessage = ex.Message });
                    }
                    return Json(new { Success = false, ErrorMessage = "There is an error while processing your request." });
                }
                else
                {
                    return Json(new { Success = false });
                }

                //   ts.Complete();

                // //   }

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoads/DeleteHabCSVFileDetails");
                return Json(new { Success = false, ErrorMessage = "There is an error while processing your request." });
            }
            finally
            {
                dbContext.Configuration.AutoDetectChangesEnabled = true;
            }
        }





        [HttpPost]
        public JsonResult FinaliseHabCSV(string id)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            ExistingRoadsDAL objDAL = new ExistingRoadsDAL();

            try
            {
                Int32 FileId = Convert.ToInt32(id);

                string status = objDAL.FinaliseHabCSVDAL(FileId);

                if (status == string.Empty)
                {
                    return Json(new { Success = true, ErrorMessage = "CSV File is finalized Successfully.." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = status });
                }


            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoads/FinaliseHabCSV");
                return Json(new { Success = false, ErrorMessage = "There is an error while processing your request." });
            }
        }

        [HttpGet]
        public ActionResult DownloadHabMultipleCSVFile(String parameter, String hash, String key)
        {
            try
            {
                PMGSYEntities dbContext = new PMGSYEntities();
                string FileName = string.Empty;
                int obsId = 0;
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
                        obsId = Convert.ToInt32(urlSplitParams[1]);
                    }
                }
                FileExtension = Path.GetExtension(FileName).ToLower();

                string VirtualDirectoryUrl = string.Empty;
                string PhysicalPath = string.Empty;

                VirtualDirectoryUrl = ConfigurationManager.AppSettings["DRRP_HAB_TRACE_CSV_PATH_VIRTUAL_DIR_PATH"];
                PhysicalPath = ConfigurationManager.AppSettings["DRRP_HAB_TRACE_CSV_PATH"];

                FullFileLogicalPath = Path.Combine(VirtualDirectoryUrl, FileName);
                FullfilePhysicalPath = Path.Combine(PhysicalPath, FileName);

                string name = Path.GetFileName(FileName);
                string ext = Path.GetExtension(FileName);

                if (System.IO.File.Exists(FullfilePhysicalPath))
                {
                    return File(FullfilePhysicalPath, "Application/csv", DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + FileExtension);
                }
                else
                {
                    return Json(new { Success = "false", ErrorMessage = "File Not Found." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoads/DownloadHabMultipleCSVFile");
                return Json(new { Success = "false", ErrorMessage = "Error While Processing Your Request." }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Map Unmap Hab

        [HttpGet]
        public ActionResult MapUnmapHabGetTraceMaps()
        {
            try
            {
                TraceMapsModel model = new TraceMapsModel();
                CommonFunctions common = new CommonFunctions();
                model.DistrictList = new List<SelectListItem>();
                model.StateList = new List<SelectListItem>();

                if (PMGSYSession.Current.RoleCode == 25)
                {
                    model.StateList = common.PopulateStates();
                    model.DistrictList.Add(new SelectListItem { Text = "Select District", Value = "0", Selected = true });
                    //model.BlockList.Add(new SelectListItem { Text = PMGSYSession.Current.DistrictName, Value = PMGSYSession.Current.DistrictCode.ToString(), Selected = true });
                }

                if (PMGSYSession.Current.DistrictCode > 0)
                {
                    model.DistrictList.Add(new SelectListItem { Text = PMGSYSession.Current.DistrictName, Value = PMGSYSession.Current.DistrictCode.ToString(), Selected = true });
                    return View(model);
                }

                if (PMGSYSession.Current.StateCode > 0)
                {
                    model.DistrictList = common.PopulateDistrict(PMGSYSession.Current.StateCode);
                    return View(model);
                }

                return View(model);
            }

            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoads/MapUnmapHabGetTraceMaps");
                return null;
            }
        }

        [HttpPost]
        public ActionResult MapUnmapHabGetDistrictList(FormCollection formdata)
        {

            try
            {
                int totalRecords;

                var formData = Request.Params["formdata"];
                var FormArrayKeys = formdata[0].Split('&');

                List<string> ModelValues = new List<string>();

                foreach (var item in FormArrayKeys)
                {
                    ModelValues.Add(item.Split('=')[1]);
                }
                var DistrictCode = Convert.ToInt32(Request.Params["DistrictCodeDD"]);

                #region For MORD

                if (PMGSYSession.Current.RoleCode == 25)
                {
                    ExistingRoadsDAL objDAL = new ExistingRoadsDAL();
                    var Statecode = Convert.ToInt32(Request.Params["statecode"]);
                    var jsonDataMORD = new
                    {
                        rows = objDAL.MapUnmapHabGetDistrictListMORDDAL(Convert.ToInt32(formdata["page"]) - 1, Convert.ToInt32(formdata["rows"]), formdata["sidx"], formdata["sord"], out totalRecords, Statecode, DistrictCode),
                        total = totalRecords <= Convert.ToInt32(formdata["rows"]) ? 1 : totalRecords / Convert.ToInt32(formdata["rows"]) + 1,
                        page = Convert.ToInt32(formdata["page"]),
                        records = totalRecords
                    };
                    return Json(jsonDataMORD);

                }


                #endregion
                ExistingRoadsDAL objDAL1 = new ExistingRoadsDAL();
                var jsonData = new
                {

                    rows = objDAL1.MapUnmapHabGetDistrictListDAL(Convert.ToInt32(formdata["page"]) - 1, Convert.ToInt32(formdata["rows"]), formdata["sidx"], formdata["sord"], out totalRecords, DistrictCode),
                    total = totalRecords <= Convert.ToInt32(formdata["rows"]) ? 1 : totalRecords / Convert.ToInt32(formdata["rows"]) + 1,
                    page = Convert.ToInt32(formdata["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistringRoadsController.MapUnmapHabGetDistrictList()");
                return null;
            }

        }

        [HttpPost]
        public ActionResult MapUnmapHabitationDetails()
        {
            try
            {
                PMGSYEntities dbcontext = new PMGSYEntities();
                var blockcode = Convert.ToInt32(Request.Params["BlockCode"]);
                int DistrictCode=dbcontext.MASTER_BLOCK.Where(m=>m.MAST_BLOCK_CODE==blockcode).Select(m=>m.MAST_DISTRICT_CODE).FirstOrDefault();
                int StateCode = dbcontext.MASTER_DISTRICT.Where(m => m.MAST_DISTRICT_CODE == DistrictCode).Select(m => m.MAST_STATE_CODE).FirstOrDefault();

                if (dbcontext.MAST_HAB_CSV_PMGSY3.Where(obj => obj.MAST_BLOCK_CODE == blockcode).Any())
                {
                    if (dbcontext.MAST_HAB_CSV_PMGSY3.Where(obj => obj.MAST_BLOCK_CODE == blockcode && obj.MAST_HAB_CSV_FILE_FINALIZED == "Y").Any())
                    {
                        // Continue to Update FLAG
                    }
                    else
                    {
                        return Json(new { success = false, message = "CSV File is not Finalized by PIU." }, JsonRequestBehavior.DenyGet);
                       // return Json(new { success = false }, JsonRequestBehavior.DenyGet);
                    }
                }
                else
                {
                    return Json(new { success = false, message = "CSV File is not Uploaded by ITNO." }, JsonRequestBehavior.DenyGet);
                  //  return Json(new { success = false }, JsonRequestBehavior.DenyGet);
                }



                var entry = dbcontext.USP_MAP_UNMAP_HABS_FROM_CSV(StateCode, blockcode);

                var entry1 = dbcontext.MAST_HAB_CSV_PMGSY3.Where(obj => obj.MAST_BLOCK_CODE == blockcode && obj.MAST_HAB_CSV_FILE_FINALIZED == "Y").FirstOrDefault();

                if (entry1 != null)
                {
                    entry1.MAST_MAP_UNMAP = "Y";
                    entry1.MAST_MAP_UNMAP_DATE = System.DateTime.Now;
                    dbcontext.Entry(entry1).State = System.Data.Entity.EntityState.Modified;
                    dbcontext.SaveChanges();
                   // return Json(new { success = true }, JsonRequestBehavior.DenyGet);
                }

                if (entry != null)
                {
                    
                    return Json(new { success = true }, JsonRequestBehavior.DenyGet);
                }
                return Json(new { success = false, message = "Error occured while processing your request." }, JsonRequestBehavior.DenyGet);
               // return Json(new { success = false }, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {

                ErrorLog.LogError(ex, "ExistinRoads/MapUnmapHabitationDetails");
                return Json(new { success = false, message = "Block Code, ER Code or Habitation Code is not valid in the CSV File." }, JsonRequestBehavior.DenyGet);

               // return Json(new { success = false }, JsonRequestBehavior.DenyGet);
            }
        }

        #endregion

        #region Final DRRP Deletion
        [Audit]
        public ActionResult DeleteListExistingRoadsPMGSY3()
        {
            FinalizeDRRPPMGSY3ViewModel model = new FinalizeDRRPPMGSY3ViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                model.lstDistricts = comm.PopulateDistrict(PMGSYSession.Current.StateCode, true);

                if (PMGSYSession.Current.RoleCode == 22)
                {
                    model.districtCode = PMGSYSession.Current.DistrictCode;
                }
                else
                {
                    model.lstDistricts.Find(x => x.Value == "-1").Text = "Select District";
                }

                List<SelectListItem> lstDefault = new List<SelectListItem>();

                
                lstDefault.Insert(0, new SelectListItem { Value = "0", Text = "All" });

                if (PMGSYSession.Current.RoleCode == 22)
                {
                    lstDefault = comm.PopulateBlocks(PMGSYSession.Current.DistrictCode);
                }


                ViewData["blockList"] = lstDefault;


                List<MASTER_ROAD_CATEGORY> roadCategoryList = objDAL.GetAllRoadCategory();
                roadCategoryList.Insert(0, new MASTER_ROAD_CATEGORY() { MAST_ROAD_CAT_CODE = 0, MAST_ROAD_CAT_NAME = "-- All --" });

                ViewData["roadCategoryList"] = new SelectList(roadCategoryList, "MAST_ROAD_CAT_CODE", "MAST_ROAD_CAT_NAME");

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoads.DeleteListExistingRoadsPMGSY3()");
                return null;
            }
        }


        [HttpGet]
        [Audit]
        public ActionResult DeleteGetExistingRoadsPMGSY3List(int? page, int? rows, string sidx, string sord)
        {
            String searchParameters = String.Empty;
            long totalRecords;

            int blockCode = 0;
            int categoryCode = 0;
            int ownerCode = 0;
            string roadName = string.Empty;
            int stateCode = 0;
            int districtCode = 0;
            ExistingRoadsDAL objDAL = new ExistingRoadsDAL();
            try
            {

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }


                stateCode = PMGSYSession.Current.StateCode;


                if (!string.IsNullOrEmpty(Request.Params["districtCode"]))
                {
                    districtCode = Convert.ToInt32(Request.Params["districtCode"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["catCode"]))
                {
                    categoryCode = Convert.ToInt32(Request.Params["catCode"]);
                }



                if (!string.IsNullOrEmpty(Request.Params["blockCode"]))
                {
                    blockCode = Convert.ToInt32(Request.Params["blockCode"]);
                }


                // categoryCode = 0;


                var jsonData = new
                {
                    rows = objDAL.DeleteListExistingRoadsPMGSY3DAL(stateCode, districtCode, blockCode, categoryCode, ownerCode, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords, Request.Params["filters"]),
                    total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                    page = Convert.ToInt32(Request.Params["page"]),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetExistingRoadsPMGSY3List()");
                return null;
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult DeletePCI(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            bool status = false;
            ExistingRoadsDAL objDAL = new ExistingRoadsDAL();
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                int roadCode = Convert.ToInt32(decryptedParameters["RoadCode"]);

                if (decryptedParameters.Count() > 0)
                {
                    if (objDAL.DeletePCIDAL(roadCode, ref message))
                    {
                        message = "PCI details deleted successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "PCI details can not be deleted because other details for road are entered." : message;
                    }
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

                }
                message = "There is an error occured while processing your request.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "There is an error occured while processing your request.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult DeleteCBR(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            bool status = false;
            ExistingRoadsDAL objDAL = new ExistingRoadsDAL();
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                int roadCode = Convert.ToInt32(decryptedParameters["RoadCode"]);

                if (decryptedParameters.Count() > 0)
                {
                    if (objDAL.DeleteCBRDAL(roadCode, ref message))
                    {
                        message = "CBR details deleted successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "CBR details can not be deleted because other details for road are entered." : message;
                    }
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

                }
                message = "There is an error occured while processing your request.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "There is an error occured while processing your request.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult DeleteCDWorks(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            bool status = false;
            ExistingRoadsDAL objDAL = new ExistingRoadsDAL();
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                int roadCode = Convert.ToInt32(decryptedParameters["RoadCode"]);

                if (decryptedParameters.Count() > 0)
                {
                    if (objDAL.DeleteCDWorksDAL(roadCode, ref message))
                    {
                        message = "CD Works details deleted successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "CD Works details can not be deleted because other details for road are entered." : message;
                    }
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

                }
                message = "There is an error occured while processing your request.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "There is an error occured while processing your request.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        [Audit]
        public ActionResult DeleteHabitations(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            bool status = false;
            ExistingRoadsDAL objDAL = new ExistingRoadsDAL();
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                int roadCode = Convert.ToInt32(decryptedParameters["RoadCode"]);

                if (decryptedParameters.Count() > 0)
                {
                    if (objDAL.DeleteHabitationsDAL(roadCode, ref message))
                    {
                        message = "Habitation details deleted successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Habitation details can not be deleted because other details for road are entered." : message;
                    }
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

                }
                message = "There is an error occured while processing your request.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "There is an error occured while processing your request.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        [Audit]
        public ActionResult DeleteSurfaceTypes(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            bool status = false;
            ExistingRoadsDAL objDAL = new ExistingRoadsDAL();
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                int roadCode = Convert.ToInt32(decryptedParameters["RoadCode"]);

                if (decryptedParameters.Count() > 0)
                {
                    if (objDAL.DeleteSurfaceTypesDAL(roadCode, ref message))
                    {
                        message = "Surface Types details deleted successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Surface Type details can not be deleted because other details for road are entered." : message;
                    }
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

                }
                message = "There is an error occured while processing your request.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "There is an error occured while processing your request.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult DeleteTrafficeIntensity(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            bool status = false;
            ExistingRoadsDAL objDAL = new ExistingRoadsDAL();
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                int roadCode = Convert.ToInt32(decryptedParameters["RoadCode"]);

                if (decryptedParameters.Count() > 0)
                {
                    if (objDAL.DeleteTrafficeIntensityDAL(roadCode, ref message))
                    {
                        message = "Traffice Intensity details deleted successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Traffice Intensity details can not be deleted because other details for road are entered." : message;
                    }
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

                }
                message = "There is an error occured while processing your request.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "There is an error occured while processing your request.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        [Audit]
        public ActionResult DeleteDRRPandMRL(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            bool status = false;
            ExistingRoadsDAL objDAL = new ExistingRoadsDAL();
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                int roadCode = Convert.ToInt32(decryptedParameters["RoadCode"]);

                if (decryptedParameters.Count() > 0)
                {
                    if (objDAL.DeleteDRRPandMRLDAL(roadCode, ref message))
                    {
                        message = "DRRP / MRL details deleted successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "DRRP / MRL details can not be deleted because other details for road are entered." : message;
                    }
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

                }
                message = "There is an error occured while processing your request.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "There is an error occured while processing your request.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult DeleteExistingRoadsPMGSY3DRRP(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            bool status = false;
            ExistingRoadsDAL objDAL = new ExistingRoadsDAL();
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                int roadCode = Convert.ToInt32(decryptedParameters["RoadCode"]);

                if (decryptedParameters.Count() > 0)
                {
                    if (objDAL.DeleteExistingRoadsDALDRRPPMGSY3(roadCode, ref message))
                    {
                        message = "Existing Road details deleted successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Existing Road details can not be deleted because other details for road are entered." : message;
                    }
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

                }
                message = "There is an error occured while processing your request.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "There is an error occured while processing your request.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult BlockDetailsITNO(FormCollection frmCollection)
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            List<SelectListItem> list = objCommonFunctions.PopulateBlocks(Convert.ToInt32(frmCollection["DistrictCode"]), true);
            list.Find(x => x.Value == "-1").Value = "0";
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Shift DRRP to New District and Block 
        // 27 Jan 2021

        public ActionResult ShiftDRRPBlock(String id)
        {
            int districtCode = 0;
            int stateCode = 0;
            int drrpCode = 0;
            int villageCode = 0;
            int blockCode = 0;
            Dictionary<string, string> decryptedParameters = null;
            String[] encryptedParameters = null;
            PMGSYEntities dbContext = new PMGSYEntities(); 
            PMGSY.DAL.MasterDataEntryDAL masterDataEntryDAL = new PMGSY.DAL.MasterDataEntryDAL();

            try
            {
                if (id != string.Empty)
                {
                    ViewBag.EncryptedDRRPCode = id;
                    encryptedParameters = id.Split('/');

                    if (encryptedParameters.Length == 3)
                    {
                        decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                        drrpCode = Convert.ToInt32(decryptedParameters["RoadCode"].ToString());
                     //   villageCode = Convert.ToInt32(decryptedParameters["VillageCode"].ToString());
                    }


                    districtCode = dbContext.MASTER_EXISTING_ROADS.Where(b => b.MAST_ER_ROAD_CODE == drrpCode).Select(b => b.MAST_DISTRICT_CODE).FirstOrDefault();
                    stateCode = dbContext.MASTER_EXISTING_ROADS.Where(d => d.MAST_ER_ROAD_CODE == drrpCode).Select(d => d.MAST_STATE_CODE).FirstOrDefault();
                    blockCode = dbContext.MASTER_EXISTING_ROADS.Where(d => d.MAST_ER_ROAD_CODE == drrpCode).Select(d => d.MAST_BLOCK_CODE).FirstOrDefault();

                    if (PMGSYSession.Current.StateCode == 0)
                    {
                        ViewData["StateList"] = new SelectList(masterDataEntryDAL.GetAllStates(false), "MAST_STATE_CODE", "MAST_STATE_NAME", stateCode); //GetAllStates(false);

                        List<Models.MASTER_DISTRICT> districtList = new List<Models.MASTER_DISTRICT>();
                        districtList = masterDataEntryDAL.GetAllDistrictsByStateCode(stateCode, false);
                        //  districtList.RemoveAt(0);
                        ViewData["DistrictList"] = new SelectList(districtList, "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME", districtCode);
                    }
                    else if (PMGSYSession.Current.StateCode > 0)
                    {
                        ViewData["StateList"] = new SelectList(masterDataEntryDAL.GetAllStates(false).Where(z => z.MAST_STATE_CODE == PMGSYSession.Current.StateCode), "MAST_STATE_CODE", "MAST_STATE_NAME", stateCode); //GetAllStates(false);
                        ViewData["DistrictList"] = new SelectList(masterDataEntryDAL.GetAllDistrictsByStateCode(stateCode, false), "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME", districtCode);
                        //ViewData["DistrictList"] = new SelectList(masterDataEntryDAL.GetAllDistrictsByStateCode(stateCode, false).Where(z => z.MAST_DISTRICT_CODE == districtCode), "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME", districtCode);
                    }
                    //Models.MASTER_DISTRICT master_district=districtList.ElementAt(0);
                    List<Models.MASTER_BLOCK> blockList = new List<Models.MASTER_BLOCK>();
                    blockList = masterDataEntryDAL.GetAllBlocksByDistrictCode(districtCode, false);
                    //blockList.RemoveAt(0);

                    blockList = blockList.Where(b => b.MAST_BLOCK_CODE != blockCode).ToList<Models.MASTER_BLOCK>();

                    ViewData["BlockList"] = new SelectList(blockList, "MAST_BLOCK_CODE", "MAST_BLOCK_NAME");
                    ViewBag.ExistingStateName = dbContext.MASTER_STATE.Where(s => s.MAST_STATE_CODE == stateCode).Select(s => s.MAST_STATE_NAME).FirstOrDefault();
                    ViewBag.ExistingDistrictName = dbContext.MASTER_DISTRICT.Where(d => d.MAST_DISTRICT_CODE == districtCode).Select(d => d.MAST_DISTRICT_NAME).FirstOrDefault();
                    ViewBag.ExistingBlockName = dbContext.MASTER_BLOCK.Where(b => b.MAST_BLOCK_CODE == blockCode).Select(b => b.MAST_BLOCK_NAME).FirstOrDefault();
                    ViewBag.ExistingVillageName = dbContext.MASTER_VILLAGE.Where(v => v.MAST_VILLAGE_CODE == villageCode).Select(v => v.MAST_VILLAGE_NAME).FirstOrDefault();
                    return PartialView("ShiftDRRPBlock");
                }
                return PartialView("ShiftDRRPBlock");
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoadsController.ShiftDRRPBlock() [Get Method]");
             //   Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return PartialView("ShiftDRRPBlock");
            }
        }

        [HttpPost]
        public ActionResult ShiftDRRPBlock(FormCollection frmCollection)
        {
            bool status = false;
            string EncryptedDRRPCode = string.Empty;
            string newBlockCode = string.Empty;
      //      PMGSY.DAL.ExistingRoads masterDataEntryDAL = new PMGSY.DAL.ExistingRoads();
            try
            {
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "ShiftDRRPBlock()");
                    sw.Close();
                }

                if (string.IsNullOrEmpty(frmCollection["EncryptedDRRPCode"]) || string.IsNullOrEmpty(frmCollection["ddlSearchBlocks_ShiftVillage1"])) // ddlSearchBlocks_ShiftVillage
                {
                    message = "DRRP is not shifted in new Block..";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                EncryptedDRRPCode = frmCollection["EncryptedDRRPCode"];

                newBlockCode = frmCollection["ddlSearchBlocks_ShiftVillage1"];

                if (objDAL.ShiftDRRPToNewBlock(EncryptedDRRPCode, newBlockCode))
                {

                    message = "DRRP shifted successfully.";
                    status = true;
                }
                else
                {
                    message = "DRRP is not shifted in new Block as it is used in Sanctioned or Dropped Proposal.";
                }

                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "ShiftDRRPBlock()");
                    sw.WriteLine("status : " + status.ToString());
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoadsController.ShiftDRRPBlock() [Post Method]");
                message = "DRRP is not shifted in new Block";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }




        public ActionResult ShiftDRRPBlockPMGSY3(String id)
        {
            int districtCode = 0;
            int stateCode = 0;
            int drrpCode = 0;
            int villageCode = 0;
            int blockCode = 0;
            Dictionary<string, string> decryptedParameters = null;
            String[] encryptedParameters = null;
            PMGSYEntities dbContext = new PMGSYEntities();
            PMGSY.DAL.MasterDataEntryDAL masterDataEntryDAL = new PMGSY.DAL.MasterDataEntryDAL();

            try
            {
                if (id != string.Empty)
                {
                    ViewBag.EncryptedDRRPCode = id;
                    encryptedParameters = id.Split('/');

                    if (encryptedParameters.Length == 3)
                    {
                        decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                        drrpCode = Convert.ToInt32(decryptedParameters["RoadCode"].ToString());
                        //   villageCode = Convert.ToInt32(decryptedParameters["VillageCode"].ToString());
                    }


                    districtCode = dbContext.MASTER_EXISTING_ROADS.Where(b => b.MAST_ER_ROAD_CODE == drrpCode).Select(b => b.MAST_DISTRICT_CODE).FirstOrDefault();
                    stateCode = dbContext.MASTER_EXISTING_ROADS.Where(d => d.MAST_ER_ROAD_CODE == drrpCode).Select(d => d.MAST_STATE_CODE).FirstOrDefault();
                    blockCode = dbContext.MASTER_EXISTING_ROADS.Where(d => d.MAST_ER_ROAD_CODE == drrpCode).Select(d => d.MAST_BLOCK_CODE).FirstOrDefault();

                    if (PMGSYSession.Current.StateCode == 0)
                    {
                        ViewData["StateList"] = new SelectList(masterDataEntryDAL.GetAllStates(false), "MAST_STATE_CODE", "MAST_STATE_NAME", stateCode); //GetAllStates(false);

                        List<Models.MASTER_DISTRICT> districtList = new List<Models.MASTER_DISTRICT>();
                        districtList = masterDataEntryDAL.GetAllDistrictsByStateCode(stateCode, false);
                        //  districtList.RemoveAt(0);
                        ViewData["DistrictList"] = new SelectList(districtList, "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME", districtCode);
                    }
                    else if (PMGSYSession.Current.StateCode > 0)
                    {
                        ViewData["StateList"] = new SelectList(masterDataEntryDAL.GetAllStates(false).Where(z => z.MAST_STATE_CODE == PMGSYSession.Current.StateCode), "MAST_STATE_CODE", "MAST_STATE_NAME", stateCode); //GetAllStates(false);
                        ViewData["DistrictList"] = new SelectList(masterDataEntryDAL.GetAllDistrictsByStateCode(stateCode, false), "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME", districtCode);
                        //ViewData["DistrictList"] = new SelectList(masterDataEntryDAL.GetAllDistrictsByStateCode(stateCode, false).Where(z => z.MAST_DISTRICT_CODE == districtCode), "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME", districtCode);
                    }
                    //Models.MASTER_DISTRICT master_district=districtList.ElementAt(0);
                    List<Models.MASTER_BLOCK> blockList = new List<Models.MASTER_BLOCK>();
                    blockList = masterDataEntryDAL.GetAllBlocksByDistrictCode(districtCode, false);
                    //blockList.RemoveAt(0);

                    blockList = blockList.Where(b => b.MAST_BLOCK_CODE != blockCode).ToList<Models.MASTER_BLOCK>();

                    ViewData["BlockList"] = new SelectList(blockList, "MAST_BLOCK_CODE", "MAST_BLOCK_NAME");
                    ViewBag.ExistingStateName = dbContext.MASTER_STATE.Where(s => s.MAST_STATE_CODE == stateCode).Select(s => s.MAST_STATE_NAME).FirstOrDefault();
                    ViewBag.ExistingDistrictName = dbContext.MASTER_DISTRICT.Where(d => d.MAST_DISTRICT_CODE == districtCode).Select(d => d.MAST_DISTRICT_NAME).FirstOrDefault();
                    ViewBag.ExistingBlockName = dbContext.MASTER_BLOCK.Where(b => b.MAST_BLOCK_CODE == blockCode).Select(b => b.MAST_BLOCK_NAME).FirstOrDefault();
                    ViewBag.ExistingVillageName = dbContext.MASTER_VILLAGE.Where(v => v.MAST_VILLAGE_CODE == villageCode).Select(v => v.MAST_VILLAGE_NAME).FirstOrDefault();
                    return PartialView("ShiftDRRPBlockPMGSY3");
                }
                return PartialView("ShiftDRRPBlockPMGSY3");
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoadsController.ShiftDRRPBlockPMGSY3() [Get Method]");
                //   Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return PartialView("ShiftDRRPBlockPMGSY3");
            }
        }


        [HttpPost]
        public ActionResult ShiftDRRPBlockPMGSY3(FormCollection frmCollection)
        {
            bool status = false;
            string EncryptedDRRPCode = string.Empty;
            string newBlockCode = string.Empty;
            //      PMGSY.DAL.ExistingRoads masterDataEntryDAL = new PMGSY.DAL.ExistingRoads();
            try
            {
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "ShiftDRRPBlockPMGSY3()");
                    sw.Close();
                }

                if (string.IsNullOrEmpty(frmCollection["EncryptedDRRPCode"]) || string.IsNullOrEmpty(frmCollection["ddlSearchBlocks_ShiftVillage1"])) // ddlSearchBlocks_ShiftVillage
                {
                    message = "DRRP is not shifted in new Block..";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                EncryptedDRRPCode = frmCollection["EncryptedDRRPCode"];

                newBlockCode = frmCollection["ddlSearchBlocks_ShiftVillage1"];

                if (objDAL.ShiftDRRPToNewBlockPMGSY3(EncryptedDRRPCode, newBlockCode))
                {

                    message = "DRRP shifted successfully.";
                    status = true;
                }
                else
                {
                    message = "DRRP is not shifted in new Block as it is used in Sanctioned or Dropped Proposal.";
                }

                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "ShiftDRRPBlockPMGSY3()");
                    sw.WriteLine("status : " + status.ToString());
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoadsController.ShiftDRRPBlockPMGSY3() [Post Method]");
                message = "DRRP is not shifted in new Block";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion


    }

    public class CdWorksRoadDetails
    {
        public int MAST_CD_NUMBER { get; set; }
        public string MAST_CDWORKS_NAME { get; set; }
        public decimal MAST_CD_LENGTH { get; set; }
        public decimal MAST_CD_DISCHARGE { get; set; }
        public int MAST_CONSTRUCT_YEAR { get; set; }
        public int MAST_REHAB_YEAR { get; set; }
        public decimal MAST_ER_SPAN { get; set; }
        public decimal MAST_CARRIAGE_WAY { get; set; }
        public string MAST_IS_FOOTPATH { get; set; }
    }
}
