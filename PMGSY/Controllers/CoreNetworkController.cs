#region HEADER
/*
* Project Id:

 * Project Name:OMMAS-II

 * File Name: CoreNetworkController.cs

 * Author : Vikram Nandanwar

 * Creation Date :30/May/2013

 * Desc : This class is used as controller  to perform Save,Edit,Update,Delete and listing of Core Network  screens.  
 */
#endregion


using PMGSY.BAL.Core_Network;
using PMGSY.Common;
using PMGSY.DAL.Core_Network;
using PMGSY.Extensions;
using PMGSY.Models;
using PMGSY.Models.CoreNetwork;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace PMGSY.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class CoreNetworkController : Controller
    {

        public CoreNetworkController()
        {
            PMGSYSession.Current.ModuleName = "Core Network";
        }

        private PMGSYEntities db = new PMGSYEntities();
        private ICoreNetworkBAL objBAL = new CoreNetworkBAL();
        string message = String.Empty;


        #region CORE_NETWORK

        /// <summary>
        /// Returns the list of core network details
        /// </summary>
        /// <returns>List View of Core Network details</returns>
        [Audit]
        public ActionResult ListCoreNetWorks()
        {
            List<MASTER_BLOCK> lst = new List<MASTER_BLOCK>();
            int block = 0;
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                int districtCode = PMGSYSession.Current.DistrictCode;
                CoreNetworkDAL objDAL = new CoreNetworkDAL();
                ViewBag.Scheme = PMGSYSession.Current.PMGSYScheme;
                ViewData["Route"] = objDAL.GetAllRoutes();
                ViewData["Category"] = objDAL.GetCategoryForSearch();

                List<SelectListItem> lstDefault = new List<SelectListItem>();
                lstDefault.Insert(0, new SelectListItem { Value = "0", Text = "All" });
                if (PMGSYSession.Current.RoleCode == 25 || PMGSY.Extensions.PMGSYSession.Current.RoleCode == 53 || PMGSYSession.Current.RoleCode == 65)//Changes by SAMMED A. PATIL for mordviewuser
                {
                    ViewData["States"] = objCommon.PopulateStates(true);
                    ViewData["Districts"] = lstDefault;
                    ViewData["Blocks"] = lstDefault;
                }
                else if (PMGSYSession.Current.RoleCode == 2)
                {
                    ViewData["Districts"] = objCommon.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                    ViewData["Blocks"] = lstDefault;
                }
                else
                {
                    ViewData["Blocks"] = new SelectList(objDAL.GetBlocksByDistrictCode(districtCode), "MAST_BLOCK_CODE", "MAST_BLOCK_NAME");
                    lst = objDAL.GetBlocksByDistrictCode(districtCode);

                    if (lst.Count > 0)
                    {
                        PMGSY.DAL.ExistingRoads.IExistingRoadsDAL objDRRPDAL = new PMGSY.DAL.ExistingRoads.ExistingRoadsDAL();
                        ViewData["isUnlocked"] = objDRRPDAL.CheckUnlockedDAL(lst.Select(x => x.MAST_BLOCK_CODE).First());
                    }
                }
                //for (int i = 0; i < lst.Count; i++)
                //{
                //    if (i == 0)
                //    { 
                //        block = lst.
                //    }
                //}
                //block = lst.ElementAt(0);
                ViewData["IsLocked"] = objDAL.checkIsLocked(block);

                return View();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return View();
            }

        }

        /// <summary>
        /// returns the Search view of core network
        /// </summary>
        /// <returns>returns the search view of Core networks</returns>
        [Audit]
        public ActionResult SearchNetworks()
        {
            return PartialView("SearchNetworks");
        }

        /// <summary>
        /// returns the Details view of Core Network
        /// </summary>
        /// <param name="parameter">Encrypted id</param>
        /// <param name="hash">Encrypted id</param>
        /// <param name="key">Encrypted id</param>
        /// <returns>Details view of Core Network</returns>
        [Audit]
        public ActionResult DetailsCoreNetwork(String parameter, String hash, String key)
        {
            Dictionary<string, string> parameters = null;
            CoreNetworkViewModel model = new CoreNetworkViewModel();
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            try
            {
                parameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (parameters != null)
                {
                    model = objBAL.GetCoreNetworkDetails(Convert.ToInt32(parameters["NetworkCode"]));
                    model.ROAD_CODE = db.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == model.MAST_ER_ROAD_CODE).Select(r => r.MAST_ER_ROAD_NUMBER).FirstOrDefault();
                    return PartialView("DetailsCoreNetwork", model);
                }
                else
                {
                    return PartialView("DetailsCoreNetwork", new CoreNetworkViewModel());
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return Json(new { success = false, message = "Error occurred while processing the request." });
                //return PartialView("DetailsCoreNetwork", new CoreNetworkViewModel());
            }
            finally
            {
                db.Dispose();
            }

        }

        /// <summary>
        /// returns the list of core network details for populating the grid
        /// </summary>
        /// <param name="networkCollection">form collection containing Core Network Grid Parameters</param>
        /// <returns>Json data containing list to populate grid</returns>
        [HttpPost]
        [Audit]
        public ActionResult GetCoreNetWorksList(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                int blockCode = 0;
                int districtCode = 0;
                int stateCode = 0;
                int categoryCode = 0;
                long totalRecords = 0;
                string roadType = string.Empty;
                String searchParameters = String.Empty;
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                string roadName = string.Empty;
                int CNCode = 0;

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

                if (!string.IsNullOrEmpty(Request.Params["blockCode"]))
                {
                    blockCode = Convert.ToInt32(Request.Params["blockCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["categoryCode"]))
                {
                    categoryCode = Convert.ToInt32(Request.Params["categoryCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["roadType"]))
                {
                    roadType = Request.Params["roadType"];
                }

                if (!string.IsNullOrEmpty(Request.Params["PLAN_RD_NAME"]))
                {
                    roadName = Request.Params["PLAN_RD_NAME"];
                }

                if (!string.IsNullOrEmpty(Request.Params["CNCode"]))
                {
                    CNCode = Convert.ToInt32(Request.Params["CNCode"]);
                }

                var jsonData = new
                {
                    rows = objBAL.GetCoreNetWorksList(stateCode, districtCode, blockCode, roadType, categoryCode, roadName, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, CNCode),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
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
        /// returns the core network add view
        /// </summary>
        /// <param name="blockCode">id of block</param>
        /// <returns>returns the add view of Core Network</returns>
        [Audit]
        public ActionResult AddEditCoreNetworks(/*int blockCode*/ string id)
        {
            CoreNetworkViewModel model = new CoreNetworkViewModel();
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            bool flag = false;
            int stateCode = 0, districtCode = 0, blockCode = 0;

            string[] locationcodes = id.Split('$');
            try
            {
                stateCode = locationcodes[0] == "undefined" ? 0 : Convert.ToInt32(locationcodes[0]);
                districtCode = locationcodes[1] == "undefined" ? 0 : Convert.ToInt32(locationcodes[1]);

                model.MAST_STATE_CODE = stateCode > 0 ? stateCode : PMGSYSession.Current.StateCode;
                model.MAST_DISTRICT_CODE = stateCode > 0 ? districtCode : PMGSYSession.Current.DistrictCode;
                blockCode = Convert.ToInt32(locationcodes[2]);
                model.BLOCK_NAME = objDAL.GetBlockName(blockCode);
                model.MAST_BLOCK_CODE = blockCode;
                List<SelectListItem> lstRoadNumber = new List<SelectListItem>();
                lstRoadNumber.Add(new SelectListItem { Value = "A", Text = "--Select Road--", Selected = true });
                //ViewData["RoadFrom"] = model.Road;
                ViewData["RoadFrom"] = new SelectList(lstRoadNumber, "Value", "Text");
                ViewData["RoadTo"] = new SelectList(lstRoadNumber, "Value", "Text");
                //ViewData["RoadTo"] = model.RoadTo;
                ViewData["RoadNumFrom"] = model.RoadNumFromList;
                ViewData["RoadNumTo"] = model.RoadNumToList;
                ViewData["Category"] = model.RoadCategory;
                ViewData["PreviousBlock"] = new SelectList(objDAL.GetBlocksByDistrictCode(PMGSYSession.Current.DistrictCode, blockCode), "MAST_BLOCK_CODE", "MAST_BLOCK_NAME");
                ViewData["PreviousBlockRoadNo"] = new SelectList(lstRoadNumber, "Value", "Text");
                ViewData["RoadNumber"] = new SelectList(lstRoadNumber, "Value", "Text");
                ViewData["HabitationFrom"] = new SelectList(objDAL.GetHabitationsByBlockCode(blockCode), "Value", "Text");
                ViewData["HabitationTo"] = new SelectList(objDAL.GetHabitationsByBlockCode(blockCode), "Value", "Text");
                ViewData["NextBlock"] = new SelectList(objDAL.GetBlocksByDistrictCode(PMGSYSession.Current.DistrictCode, blockCode), "MAST_BLOCK_CODE", "MAST_BLOCK_NAME");
                ViewData["NextBlockRoadNo"] = new SelectList(lstRoadNumber, "Value", "Text");


                List<SelectListItem> lstRoute = new List<SelectListItem>();
                lstRoute.Add(new SelectListItem { Value = "0", Text = "--Select Route--", Selected = true });
                lstRoute.Add(new SelectListItem { Value = "T", Text = "Through Route" });
                if (PMGSYSession.Current.PMGSYScheme == 1)
                {
                    lstRoute.Add(new SelectListItem { Value = "L", Text = "Link Route" });
                }
                // added by rohit for vibrant village prog on 20-07-2023
                else if (PMGSYSession.Current.PMGSYScheme == 2 || PMGSYSession.Current.PMGSYScheme == 5)
                {
                    ///Changes by SAMMED A. PATIL on 30AUG2017 to allow Link Route in PMGSY2
                    lstRoute.Add(new SelectListItem { Value = "L", Text = "Link Route" });
                    lstRoute.Add(new SelectListItem { Value = "M", Text = "Major Rural Link" });
                }
                //return new SelectList(lstRoute,"Value","Text");
                if (PMGSYSession.Current.PMGSYScheme == 1)
                {
                    flag = checkSchedule5(blockCode);
                    if (flag)
                    {
                        lstRoute.Add(new SelectListItem { Value = "N", Text = "Missing Link" });
                    }
                }
                model.RouteType = lstRoute;
                return PartialView("AddEditCoreNetworks", model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return PartialView();
            }
        }

        public bool checkSchedule5(int blockCode)
        {
            bool flg = false;
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            try
            {
                return objDAL.checkSchedule5DAL(blockCode);
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {

            }
        }

        /// <summary>
        /// saves the inserted details of core network
        /// </summary>
        /// <param name="model">contains the data of Core Network details</param>
        /// <returns>response message along with status</returns>
        [HttpPost]
        [Audit]
        public ActionResult AddCoreNetworks(CoreNetworkViewModel model)
        {
            bool status = false;
            try
            {
                if (ModelState.IsValid)
                {
                    model.MAST_DISTRICT_CODE = model.MAST_DISTRICT_CODE > 0 ? model.MAST_DISTRICT_CODE : PMGSYSession.Current.DistrictCode;
                    model.MAST_STATE_CODE = model.MAST_STATE_CODE > 0 ? model.MAST_STATE_CODE : PMGSYSession.Current.StateCode;

                    if (objBAL.AddCoreNetworks(model, ref message))
                    {
                        if (PMGSYSession.Current.PMGSYScheme == 1)
                        {
                            message = message == string.Empty ? "Core network details added successfully." : message;
                        }
                        // added by rohit for vibrant village prog on 20-07-2023
                        else if (PMGSYSession.Current.PMGSYScheme == 2)
                        {
                            message = message == string.Empty ? "Candidate road details added successfully." : message;
                        }
                        status = true;
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        message = message == string.Empty ? "Error occurred while processing the request." : message;
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
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

                return Json(new { success = false, message = "Error occurred while processing the request." });
                //return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// returns the core network details for updation
        /// </summary>
        /// <param name="parameter">Encrypted id</param>
        /// <param name="hash">Encrypted id</param>
        /// <param name="key">Encrypted id</param>
        /// <returns>data of Core Network road for updation</returns>
        [Audit]
        public ActionResult EditCoreNetworks(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptParameters = null;
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            List<SelectListItem> lstRoadNumber = new List<SelectListItem>();
            bool flag = false;
            try
            {
                decryptParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptParameters.Count() > 0)
                {
                    CoreNetworkViewModel model = objBAL.GetCoreNetworkDetails(Convert.ToInt32(decryptParameters["NetworkCode"]));

                    if (decryptParameters.Count > 1)
                    {
                        model.LockUnlockFlag = decryptParameters["UnlockFlag"];
                    }

                    List<SelectListItem> lstRoute = new List<SelectListItem>();
                    lstRoute.Add(new SelectListItem { Value = "0", Text = "--Select Route--", Selected = true });
                    lstRoute.Add(new SelectListItem { Value = "T", Text = "Through Route" });
                    if (PMGSYSession.Current.PMGSYScheme == 1)
                    {
                        lstRoute.Add(new SelectListItem { Value = "L", Text = "Link Route" });
                    }
                    // added by rohit for vibrant village prog on 20-07-2023
                    else if (PMGSYSession.Current.PMGSYScheme == 2 || PMGSYSession.Current.PMGSYScheme == 5)
                    {
                        ///Changes by SAMMED A. PATIL on 30AUG2017 to allow Link Route in PMGSY2
                        lstRoute.Add(new SelectListItem { Value = "L", Text = "Link Route" });
                        lstRoute.Add(new SelectListItem { Value = "M", Text = "Major Rural Link" });
                    }
                    //return new SelectList(lstRoute,"Value","Text");
                    if (PMGSYSession.Current.PMGSYScheme == 1)
                    {
                        flag = checkSchedule5(model.MAST_BLOCK_CODE);
                        if (flag)
                        {
                            lstRoute.Add(new SelectListItem { Value = "N", Text = "Missing Link" });
                        }
                    }
                    model.RouteType = lstRoute;

                    model.FROM_TYPE = model.PLAN_RD_FROM_TYPE;
                    model.TO_TYPE = model.PLAN_RD_TO_TYPE;
                    model.RD_FROM = model.PLAN_RD_FROM;
                    model.RD_TO = model.PLAN_RD_TO;
                    model.ROAD_CODE = db.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == model.MAST_ER_ROAD_CODE).Select(r => r.MAST_ER_ROAD_NUMBER).FirstOrDefault();
                    model.BLOCK_NAME = objDAL.GetBlockName(model.MAST_BLOCK_CODE);//db.MASTER_BLOCK.Where(m => m.MAST_BLOCK_CODE == model.MAST_BLOCK_CODE).Select(b => b.MAST_BLOCK_NAME).FirstOrDefault();
                    if (model.PLAN_RD_NUM_FROM == null)
                    {
                        model.NUM_FROM = null;
                    }
                    else
                    {
                        model.NUM_FROM = model.PLAN_RD_NUM_FROM;
                    }

                    if (model.PLAN_RD_NUM_TO == null)
                    {
                        model.NUM_TO = null;
                    }
                    else
                    {
                        model.NUM_TO = model.PLAN_RD_NUM_TO;
                    }

                    if (model == null)
                    {
                        ModelState.AddModelError(String.Empty, "Core network details not exist.");
                        return PartialView("AddEditCoreNetworks", new CoreNetworkViewModel());
                    }

                    ViewData["RoadFrom"] = model.Road;
                    ViewData["RoadTo"] = model.RoadTo;

                    switch (model.PLAN_RD_FROM_TYPE)
                    {
                        case "T":
                            ViewData["RoadNumFrom"] = new SelectList(objDAL.GetRoadNumFromThroughEditList(model.PLAN_RD_FROM_TYPE, model.MAST_BLOCK_CODE, model.PLAN_CN_ROAD_CODE), "Value", "Text", model.PLAN_RD_NUM_FROM);
                            break;
                        case "L":
                            ViewData["RoadNumFrom"] = new SelectList(objDAL.GetRoadNumFromThroughEditList(model.PLAN_RD_FROM_TYPE, model.MAST_BLOCK_CODE, model.PLAN_CN_ROAD_CODE), "Value", "Text", model.PLAN_RD_NUM_FROM);
                            break;
                        case "M":
                            ViewData["RoadNumFrom"] = new SelectList(objDAL.GetRoadNumFromThroughEditList(model.PLAN_RD_FROM_TYPE, model.MAST_BLOCK_CODE, model.PLAN_CN_ROAD_CODE), "Value", "Text", model.PLAN_RD_NUM_FROM);
                            break;
                        case "H":
                            ViewData["RoadNumFrom"] = model.RoadNumFromList;
                            break;
                        case "B":
                            ViewData["RoadNumFrom"] = model.RoadNumFromList;
                            break;
                        default:
                            ViewData["RoadNumFrom"] = new SelectList(objDAL.GetRoadNumFromByRoadFrom(model.PLAN_RD_FROM_TYPE, model.MAST_BLOCK_CODE), "MAST_ER_ROAD_CODE", "MAST_ER_ROAD_NAME", model.PLAN_RD_NUM_FROM);
                            break;
                    }

                    switch (model.PLAN_RD_TO_TYPE)
                    {
                        case "T":
                            ViewData["RoadNumTo"] = new SelectList(objDAL.GetRoadNumFromThroughList(model.PLAN_RD_TO_TYPE, model.MAST_BLOCK_CODE), "PLAN_CN_ROAD_CODE", "PLAN_CN_ROAD_NUMBER", model.PLAN_RD_NUM_TO);
                            break;
                        case "L":
                            ViewData["RoadNumTo"] = new SelectList(objDAL.GetRoadNumFromThroughList(model.PLAN_RD_TO_TYPE, model.MAST_BLOCK_CODE), "PLAN_CN_ROAD_CODE", "PLAN_CN_ROAD_NUMBER", model.PLAN_RD_NUM_TO);
                            break;
                        case "M":
                            ViewData["RoadNumTo"] = new SelectList(objDAL.GetRoadNumFromThroughList(model.PLAN_RD_TO_TYPE, model.MAST_BLOCK_CODE), "PLAN_CN_ROAD_CODE", "PLAN_CN_ROAD_NUMBER", model.PLAN_RD_NUM_TO);
                            break;
                        case "H":
                            ViewData["RoadNumTo"] = model.RoadNumToList;
                            break;
                        case "B":
                            ViewData["RoadNumTo"] = model.RoadNumToList;
                            break;
                        default:
                            ViewData["RoadNumTo"] = new SelectList(objDAL.GetRoadNumFromByRoadFrom(model.PLAN_RD_TO_TYPE, model.MAST_BLOCK_CODE), "MAST_ER_ROAD_CODE", "MAST_ER_ROAD_NAME", model.PLAN_RD_NUM_TO);
                            break;
                    }
                    if (model.PLAN_RD_ROUTE == "N")
                    {
                        List<string> lst = objDAL.MLRoadList(model.MAST_BLOCK_CODE);
                        List<SelectListItem> lstRoadNumberML = new List<SelectListItem>();
                        lstRoadNumberML.Insert(0, new SelectListItem { Value = "A", Text = "--Select Road No--" });
                        for (int i = 1; i <= 30; i++)
                        {
                            if (i < 10)
                            {
                                lstRoadNumberML.Add(new SelectListItem { Value = "ML0" + i.ToString(), Text = "ML0" + i.ToString() });
                            }
                            else
                            {
                                lstRoadNumberML.Add(new SelectListItem { Value = "ML" + i.ToString(), Text = "ML" + i.ToString() });
                            }
                        }

                        var list = (from item in lstRoadNumberML
                                    where !lst.Contains(item.Value)
                                    select new
                                    {
                                        item.Value,
                                        item.Text
                                    }).Distinct().ToList().Select(x => new SelectListItem
                                    {
                                        Text = x.Text,
                                        Value = x.Value
                                    }).ToList();

                        list.Add(new SelectListItem { Value = model.PLAN_CN_ROAD_NUMBER, Text = model.PLAN_CN_ROAD_NUMBER });
                        //list.Insert(list.Count, SelectListItem { Value = model.PLAN_CN_ROAD_NUMBER, Text = model.PLAN_CN_ROAD_NUMBER });
                        //new SelectList(list, "Value", "Text")

                        ViewData["RoadNumber"] = list;
                    }
                    else
                    {
                        JsonResult a = GetRoadNumberByRoadRoute(model.PLAN_RD_ROUTE, model.MAST_BLOCK_CODE, model.PLAN_CN_ROAD_CODE);
                        ViewData["RoadNumber"] = a.Data;
                    }
                    ViewData["RoadNumThroughFrom"] = model.RoadFrom;
                    ViewData["RoadNumThroughTo"] = model.RoadFrom;
                    //ViewData["RoadNumThroughTo"] = new SelectList(lstRoadNumber, "Value", "Text");
                    ViewBag.RoadCategory = objDAL.GetRoadCategory(model.MAST_ER_ROAD_CODE);
                    ViewData["PreviousBlock"] = new SelectList(objDAL.GetBlocksByDistrictCode(PMGSYSession.Current.DistrictCode), "MAST_BLOCK_CODE", "MAST_BLOCK_NAME");
                    ViewData["PreviousBlockRoadNo"] = new SelectList(objDAL.GetPreviousBlockByBlockCode(model.MAST_BLOCK_CODE), "PLAN_CN_ROAD_CODE", "PLAN_CN_ROAD_NUMBER");
                    //ViewData["RoadNumber"] = new SelectList(lstRoadNumber, "Value", "Text");
                    ViewData["HabitationFrom"] = new SelectList(objDAL.GetHabitationsByBlockCode(model.MAST_BLOCK_CODE), "Value", "Text");
                    ViewData["HabitationTo"] = new SelectList(objDAL.GetHabitationsByBlockCode(model.MAST_BLOCK_CODE), "Value", "Text");
                    ViewData["NextBlock"] = new SelectList(objDAL.GetBlocksByDistrictCode(PMGSYSession.Current.DistrictCode), "MAST_BLOCK_CODE", "MAST_BLOCK_NAME");
                    ViewData["NextBlockRoadNo"] = new SelectList(objDAL.GetPreviousBlockByBlockCode(model.MAST_BLOCK_CODE), "PLAN_CN_ROAD_CODE", "PLAN_CN_ROAD_NUMBER");
                    return PartialView("AddEditCoreNetworks", model);
                }
                return PartialView("AddEditCoreNetworks", new CoreNetworkViewModel());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return Json(new { success = false });
            }
        }

        /// <summary>
        /// save the updated details of core network
        /// </summary>
        /// <param name="model">contains the updated Core Network details</param>
        /// <returns>response message along with status of operation</returns>
        [HttpPost]
        [Audit]
        public ActionResult EditCoreNetworks(CoreNetworkViewModel model)
        {
            bool status = false;
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            try
            {
                if (ModelState.IsValid)
                {
                    if (objBAL.EditCoreNetworks(model, ref message))
                    {
                        if (PMGSYSession.Current.PMGSYScheme == 1)
                        {
                            message = message == string.Empty ? "Core network details updated successfully." : message;
                        }
                        else if (PMGSYSession.Current.PMGSYScheme == 2)
                        {
                            message = message == string.Empty ? "Candidate road details updated successfully." : message;
                        }
                        status = true;
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        message = message == string.Empty ? "Error occurred while processing your request." : message;
                        ModelState.AddModelError("", message);
                        return Json(new { success = status, message = new CommonFunctions().FormatErrorMessage(ModelState) }, JsonRequestBehavior.AllowGet);
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

                message = message == string.Empty ? "Error occurred while processing your request." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// deletes the particular core network details
        /// </summary>
        /// <param name="parameter">Encrypted id</param>
        /// <param name="hash">Encrypted id</param>
        /// <param name="key">Encrypted id</param>
        /// <returns>status of delete operation </returns>
        [HttpPost]
        [Audit]
        public ActionResult DeleteCoreNetwork(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptedParameters.Count() > 0)
                {
                    if (!objBAL.DeleteCoreNetworks(Convert.ToInt32(decryptedParameters["NetworkCode"].ToString())))
                    {
                        if (PMGSYSession.Current.PMGSYScheme == 1)
                        {
                            ModelState.AddModelError(String.Empty, "Core network details are in use and can not be deleted.");
                            message = "Core network details are in use and can not be deleted.";
                        }
                        // added by rohit for vibrant village prog on 20-07-2023
                        //else if (PMGSYSession.Current.PMGSYScheme == 2)
                        else if (PMGSYSession.Current.PMGSYScheme == 2 || PMGSYSession.Current.PMGSYScheme == 5)
                        {
                            ModelState.AddModelError(String.Empty, "Candidate road details are in use and can not be deleted.");
                            message = "Candidate road details are in use and can not be deleted.";
                        }
                        return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                if (PMGSYSession.Current.PMGSYScheme == 1)
                {
                    message = "Core network details deleted successfully.";
                }
                // added by rohit for vibrant village prog on 20-07-2023
                else if (PMGSYSession.Current.PMGSYScheme == 2 || PMGSYSession.Current.PMGSYScheme == 5)
                {
                    message = "Candidate road details deleted successfully.";
                }
                return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return Json(new { success = false, message = "Core network details can not be deleted" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// for getting Road Name of particular Road Code
        /// </summary>
        /// <param name="roadName">road name associated with road code</param>
        /// <param name="blockName">block name</param>
        /// <returns>Road name associated with the particular block</returns>
        [HttpPost]
        [Audit]
        public JsonResult GetRoadNameByRoadCode(string roadName, string blockName)
        {
            try
            {
                int roadCode = Convert.ToInt32(roadName);
                //int blockCode = PMGSYSession.Current.blockCode;
                int blockCode = Convert.ToInt32(blockName);
                CoreNetworkDAL objDAL = new CoreNetworkDAL();
                List<SelectListItem> lstRoads = objDAL.GetRoadNamesByRoadCode(roadCode, blockCode);
                lstRoads.Insert(0, new SelectListItem { Value = "0", Text = "-Select Road Name-" });
                return Json(lstRoads, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return null;
            }

        }

        /// <summary>
        /// populating the Road From Dropdown
        /// </summary>
        /// <param name="roadRoute">represents the route type</param>
        /// <returns>Road From dropdown list</returns>
        [HttpPost]
        [Audit]
        public JsonResult GetRoadFromByRoadRoute(string roadRoute)
        {
            try
            {
                CoreNetworkViewModel model = new CoreNetworkViewModel();
                if (roadRoute == "T")
                {
                    SelectList lstRoadFromTo = model.RoadFrom;
                    return Json(lstRoadFromTo);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        /// <summary>
        /// populating the Road To Dropdown
        /// </summary>
        /// <param name="roadRoute">type of route(through)</param>
        /// <returns>list of roads of type through route</returns>
        [HttpPost]
        [Audit]
        public JsonResult GetRoadToByRoadRoute(string roadRoute)
        {
            try
            {
                CoreNetworkViewModel model = new CoreNetworkViewModel();
                if (roadRoute == "L")
                {
                    SelectList lstRoadTo = model.RoadTo;
                    return Json(lstRoadTo);
                }
                else if (roadRoute == "M")
                {
                    SelectList lstRoadTo = model.RoadTo;
                    return Json(lstRoadTo);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return null;
            }
        }

        /// <summary>
        /// populates Road To Dropdown
        /// </summary>
        /// <param name="roadRoute">represent the type of route</param>
        /// <returns>list to populate Road To dropdown</returns>
        [HttpPost]
        [Audit]
        public JsonResult GetRoadFromByRoadRouteLink(string roadRoute)
        {
            try
            {
                List<SelectListItem> lstRoadFrom = new List<SelectListItem>();
                lstRoadFrom.Add(new SelectListItem { Value = "0", Text = "-Select Road-", Selected = true });
                lstRoadFrom.Add(new SelectListItem { Value = "H", Text = "Habitation" });
                if (PMGSYSession.Current.PMGSYScheme == 1)
                {
                    lstRoadFrom.Add(new SelectListItem { Value = "L", Text = "Link Route" });
                }
                else if (PMGSYSession.Current.PMGSYScheme == 2 || PMGSYSession.Current.PMGSYScheme == 4)///PMGSY3
                {
                    //lstRoadFrom.Add(new SelectListItem { Value = "M", Text = "Major Rural Link" });
                    lstRoadFrom.Add(new SelectListItem { Value = "D", Text = "Major District Road" });
                    lstRoadFrom.Add(new SelectListItem { Value = "N", Text = "National Highway" });
                    lstRoadFrom.Add(new SelectListItem { Value = "O", Text = "Others" });
                    lstRoadFrom.Add(new SelectListItem { Value = "R", Text = "Rural Road(Other District Roads)" });
                    lstRoadFrom.Add(new SelectListItem { Value = "Z", Text = "Rural Road(Track)" });
                    lstRoadFrom.Add(new SelectListItem { Value = "V", Text = "Rural Road(Village Roads)" });
                    lstRoadFrom.Add(new SelectListItem { Value = "S", Text = "State Highway" });
                }
                if (!(PMGSYSession.Current.PMGSYScheme == 4))
                {
                    lstRoadFrom.Add(new SelectListItem { Value = "T", Text = "Through Route" });
                }
                return Json(new SelectList(lstRoadFrom, "Value", "Text"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        /// <summary>
        /// populates the Road Number dropdown
        /// </summary>
        /// <param name="roadRoute">type of Route </param>
        /// <param name="blockCode">block code of Road Number</param>
        /// <returns>list of Road Number</returns>
        [HttpPost]
        [Audit]
        public JsonResult GetRoadNumberByRoadRoute(string roadRoute, int blockCode, int cnCode)
        {
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            try
            {
                if (roadRoute == "T")
                {
                    List<SelectListItem> lstRoadNumberThrough = new List<SelectListItem>();
                    if (PMGSYSession.Current.PMGSYScheme == 1)
                    {
                        for (int i = 1; i <= 20; i++)
                        {
                            if (i < 10)
                            {
                                lstRoadNumberThrough.Add(new SelectListItem { Value = "T0" + i.ToString(), Text = "T0" + i.ToString() });
                            }
                            else
                            {
                                lstRoadNumberThrough.Add(new SelectListItem { Value = "T" + i.ToString(), Text = "T" + i.ToString() });
                            }
                            lstRoadNumberThrough.Insert(0, new SelectListItem { Value = "A", Text = "--Select Road No--" });
                        }
                    }
                    // added by rohit for vibrant village prog on 20-07-2023
                    else if (PMGSYSession.Current.PMGSYScheme == 2 || PMGSYSession.Current.PMGSYScheme == 4 || PMGSYSession.Current.PMGSYScheme == 5)///PMGSY3
                    {
                        for (int i = 1; i <= 50; i++)
                        {
                            if (i < 10)
                            {
                                lstRoadNumberThrough.Add(new SelectListItem { Value = "T0" + i.ToString(), Text = "T0" + i.ToString() });
                            }
                            else
                            {
                                lstRoadNumberThrough.Add(new SelectListItem { Value = "T" + i.ToString(), Text = "T" + i.ToString() });
                            }
                            lstRoadNumberThrough.Insert(0, new SelectListItem { Value = "A", Text = "--Select Road No--" });
                        }
                    }

                    List<string> filter = objDAL.GetThroughRoutes(blockCode, cnCode);

                    var list = (from item in lstRoadNumberThrough
                                where !filter.Contains(item.Value)
                                select new
                                {
                                    item.Value,
                                    item.Text
                                }).Distinct().ToList();

                    int recordCount = list.Count();
                    if (recordCount == 0)
                    {
                        List<SelectListItem> lst = new List<SelectListItem>();
                        lst.Add(new SelectListItem { Value = "A", Text = "No Record Found" });
                        return Json(new SelectList(lst, "Value", "Text"), JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new SelectList(list, "Value", "Text"), JsonRequestBehavior.AllowGet);
                    }
                }
                else if (roadRoute == "L" || roadRoute == "M")///Changed by SAMMED A. PATIL on 21APRIL2017 to correct the entry of Missing Rural Link as MRL
                {
                    List<string> filter = null;
                    List<SelectListItem> lstRoadNumberLink = new List<SelectListItem>();
                    if (PMGSYSession.Current.PMGSYScheme == 1)
                    {
                        for (int i = 21; i < 1000; i++)
                        {
                            if (i < 100)
                            {
                                lstRoadNumberLink.Add(new SelectListItem { Value = "L0" + i.ToString(), Text = "L0" + i.ToString() });
                            }
                            else
                            {
                                lstRoadNumberLink.Add(new SelectListItem { Value = "L" + i.ToString(), Text = "L" + i.ToString() });
                            }
                        }
                    }
                    // added by rohit for vibrant village prog on 20-07-2023
                    else if (PMGSYSession.Current.PMGSYScheme == 2 || PMGSYSession.Current.PMGSYScheme == 4 || PMGSYSession.Current.PMGSYScheme == 5)///PMGSY3
                    {
                        ///Changes by SAMMED A. PATIL on 30AUG2017 to allow Link Route in PMGSY2
                        if (roadRoute == "L")
                        {
                            for (int i = 21; i < 1000; i++)
                            {
                                if (i < 100)
                                {
                                    lstRoadNumberLink.Add(new SelectListItem { Value = "L0" + i.ToString(), Text = "L0" + i.ToString() });
                                }
                                else
                                {
                                    lstRoadNumberLink.Add(new SelectListItem { Value = "L" + i.ToString(), Text = "L" + i.ToString() });
                                }
                            }
                        }
                        else if (roadRoute == "M")
                        {
                            //for (int i = 1; i <= 50; i++)
                            for (int i = 1; i <= 99; i++)///Changed by SAMMED A. PATIL on 21APRIL2017 to allow MRL entry upto 99
                            {
                                if (i < 10)
                                {
                                    lstRoadNumberLink.Add(new SelectListItem { Value = "MRL0" + i.ToString(), Text = "MRL0" + i.ToString() });
                                }
                                else
                                {
                                    lstRoadNumberLink.Add(new SelectListItem { Value = "MRL" + i.ToString(), Text = "MRL" + i.ToString() });
                                }
                            }
                        }
                    }

                    lstRoadNumberLink.Insert(0, new SelectListItem { Value = "A", Text = "--Select Road No--" });

                    if (PMGSYSession.Current.PMGSYScheme == 1)
                    {
                        filter = objDAL.GetLinkRoutes(blockCode, cnCode);
                    }
                    else
                    {
                        ///Changes by SAMMED A. PATIL on 30AUG2017 to allow Link Route in PMGSY2
                        if (roadRoute == "L")
                        {
                            filter = objDAL.GetLinkRoutes(blockCode, cnCode);
                        }
                        if (roadRoute == "M")
                        {
                            filter = objDAL.GetMRLRoutes(blockCode, cnCode);
                        }
                    }

                    var list = (from item in lstRoadNumberLink
                                where !filter.Contains(item.Value)
                                select new
                                {
                                    item.Value,
                                    item.Text
                                }).Distinct().ToList();

                    int recordCount = list.Count();
                    if (recordCount == 0)
                    {
                        List<SelectListItem> lst = new List<SelectListItem>();
                        lst.Add(new SelectListItem { Value = "A", Text = "No Record Found" });
                        return Json(new SelectList(lst, "Value", "Text"), JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new SelectList(list, "Value", "Text"), JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    List<string> lst = objDAL.MLRoadList(blockCode);
                    List<SelectListItem> lstRoadNumberML = new List<SelectListItem>();
                    lstRoadNumberML.Insert(0, new SelectListItem { Value = "A", Text = "--Select Road No--" });
                    for (int i = 1; i <= 30; i++)
                    {
                        if (i < 10)
                        {
                            lstRoadNumberML.Add(new SelectListItem { Value = "ML0" + i.ToString(), Text = "ML0" + i.ToString() });
                        }
                        else
                        {
                            lstRoadNumberML.Add(new SelectListItem { Value = "ML" + i.ToString(), Text = "ML" + i.ToString() });
                        }
                    }

                    var list = (from item in lstRoadNumberML
                                where !lst.Contains(item.Value)
                                select new
                                {
                                    item.Value,
                                    item.Text
                                }).Distinct().ToList();

                    return Json(new SelectList(list, "Value", "Text"), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        /// <summary>
        /// populates the dropdown associated with the Road From type
        /// </summary>
        /// <param name="roadFrom">type of road</param>
        /// <param name="blockCode">block of roads</param>
        /// <param name="selectedCode">already selected id (in case of edit)</param>
        /// <returns>list to populate the dropdown associated with the Road From</returns>
        [HttpPost]
        [Audit]
        public JsonResult GetRoadNumFromByRoadFrom(string roadFrom, int blockCode, string selectedCode)
        {
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            List<PLAN_ROAD> lstPlanRoad = new List<PLAN_ROAD>();
            try
            {
                switch (roadFrom)
                {
                    case "T":
                        lstPlanRoad = objDAL.GetRoadNumFromThroughList(roadFrom, blockCode);

                        if (selectedCode != null && selectedCode != "undefined")
                        {
                            lstPlanRoad.Insert(0, new PLAN_ROAD { PLAN_CN_ROAD_CODE = 0, PLAN_CN_ROAD_NUMBER = "--Select--" });
                            return Json(new SelectList(lstPlanRoad, "PLAN_CN_ROAD_CODE", "PLAN_CN_ROAD_NUMBER", selectedCode));
                        }
                        lstPlanRoad.Insert(0, new PLAN_ROAD { PLAN_CN_ROAD_CODE = 0, PLAN_CN_ROAD_NUMBER = "--Select--" });
                        return Json(new SelectList(lstPlanRoad, "PLAN_CN_ROAD_CODE", "PLAN_CN_ROAD_NUMBER"));

                    case "L":
                        lstPlanRoad = objDAL.GetRoadNumFromThroughList(roadFrom, blockCode);
                        if (selectedCode != null && selectedCode != "undefined")
                        {
                            lstPlanRoad.Insert(0, new PLAN_ROAD { PLAN_CN_ROAD_CODE = 0, PLAN_CN_ROAD_NUMBER = "--Select--" });
                            return Json(new SelectList(lstPlanRoad, "PLAN_CN_ROAD_CODE", "PLAN_CN_ROAD_NUMBER", selectedCode));
                        }
                        else
                        {
                            lstPlanRoad.Insert(0, new PLAN_ROAD { PLAN_CN_ROAD_CODE = 0, PLAN_CN_ROAD_NUMBER = "--Select--" });
                            return Json(new SelectList(lstPlanRoad, "PLAN_CN_ROAD_CODE", "PLAN_CN_ROAD_NUMBER", 0));
                        }
                    case "M":
                        lstPlanRoad = objDAL.GetRoadNumFromThroughList(roadFrom, blockCode);
                        if (selectedCode != null && selectedCode != "undefined")
                        {
                            lstPlanRoad.Insert(0, new PLAN_ROAD { PLAN_CN_ROAD_CODE = 0, PLAN_CN_ROAD_NUMBER = "--Select--" });
                            return Json(new SelectList(lstPlanRoad, "PLAN_CN_ROAD_CODE", "PLAN_CN_ROAD_NUMBER", selectedCode));
                        }
                        else
                        {
                            lstPlanRoad.Insert(0, new PLAN_ROAD { PLAN_CN_ROAD_CODE = 0, PLAN_CN_ROAD_NUMBER = "--Select--" });
                            return Json(new SelectList(lstPlanRoad, "PLAN_CN_ROAD_CODE", "PLAN_CN_ROAD_NUMBER", 0));
                        }
                    default:
                        List<MASTER_EXISTING_ROADS> list = objDAL.GetRoadNumFromByRoadFrom(roadFrom, blockCode);

                        if (list.Count() == 0)
                        {
                            list.Insert(0, new MASTER_EXISTING_ROADS { MAST_ER_ROAD_CODE = 0, MAST_ER_ROAD_NAME = "-No Roads Found-" });
                            return Json(new SelectList(list, "MAST_ER_ROAD_CODE", "MAST_ER_ROAD_NAME"), JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            list.Insert(0, new MASTER_EXISTING_ROADS { MAST_ER_ROAD_CODE = 0, MAST_ER_ROAD_NAME = "--Select--" });
                            return Json(new SelectList(list, "MAST_ER_ROAD_CODE", "MAST_ER_ROAD_NAME", selectedCode), JsonRequestBehavior.AllowGet);
                        }
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        /// <summary>
        /// populates the dropdown corresponding to the Road To Option
        /// </summary>
        /// <param name="roadFrom">type of road </param>
        /// <param name="blockCode">block of road</param>
        /// <param name="selectedCode">selected id of road</param>
        /// <returns>list to populate the road of particular Road Type</returns>
        [HttpPost]
        [Audit]
        public JsonResult GetRoadNumFromByRoadTo(string roadFrom, int blockCode, string selectedCode)
        {
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            List<PLAN_ROAD> lstPlanRoad = new List<PLAN_ROAD>();
            try
            {
                switch (roadFrom)
                {
                    case "T":
                        lstPlanRoad = objDAL.GetRoadNumFromThroughList(roadFrom, blockCode);

                        if (selectedCode != null && selectedCode != "undefined")
                        {
                            lstPlanRoad.Insert(0, new PLAN_ROAD { PLAN_CN_ROAD_CODE = 0, PLAN_CN_ROAD_NUMBER = "--Select--" });
                            return Json(new SelectList(lstPlanRoad, "PLAN_CN_ROAD_CODE", "PLAN_CN_ROAD_NUMBER", selectedCode));
                        }
                        else
                        {
                            lstPlanRoad.Insert(0, new PLAN_ROAD { PLAN_CN_ROAD_CODE = 0, PLAN_CN_ROAD_NUMBER = "--Select--" });
                            return Json(new SelectList(lstPlanRoad, "PLAN_CN_ROAD_CODE", "PLAN_CN_ROAD_NUMBER"));
                        }
                    case "L":
                        lstPlanRoad = objDAL.GetRoadNumFromThroughList(roadFrom, blockCode);

                        if (selectedCode != null && selectedCode != "undefined")
                        {
                            lstPlanRoad.Insert(0, new PLAN_ROAD { PLAN_CN_ROAD_CODE = 0, PLAN_CN_ROAD_NUMBER = "--Select--" });
                            return Json(new SelectList(lstPlanRoad, "PLAN_CN_ROAD_CODE", "PLAN_CN_ROAD_NUMBER", selectedCode));
                        }
                        else
                        {
                            lstPlanRoad.Insert(0, new PLAN_ROAD { PLAN_CN_ROAD_CODE = 0, PLAN_CN_ROAD_NUMBER = "--Select--" });
                            return Json(new SelectList(lstPlanRoad, "PLAN_CN_ROAD_CODE", "PLAN_CN_ROAD_NUMBER", 0));
                        }
                    case "M":
                        lstPlanRoad = objDAL.GetRoadNumFromThroughList(roadFrom, blockCode);

                        if (selectedCode != null && selectedCode != "undefined")
                        {
                            lstPlanRoad.Insert(0, new PLAN_ROAD { PLAN_CN_ROAD_CODE = 0, PLAN_CN_ROAD_NUMBER = "--Select--" });
                            return Json(new SelectList(lstPlanRoad, "PLAN_CN_ROAD_CODE", "PLAN_CN_ROAD_NUMBER", selectedCode));
                        }
                        else
                        {
                            lstPlanRoad.Insert(0, new PLAN_ROAD { PLAN_CN_ROAD_CODE = 0, PLAN_CN_ROAD_NUMBER = "--Select--" });
                            return Json(new SelectList(lstPlanRoad, "PLAN_CN_ROAD_CODE", "PLAN_CN_ROAD_NUMBER", 0));
                        }
                    default:
                        List<MASTER_EXISTING_ROADS> list = objDAL.GetRoadNumFromByRoadFrom(roadFrom, blockCode);

                        if (list.Count() == 0)
                        {
                            list.Insert(0, new MASTER_EXISTING_ROADS { MAST_ER_ROAD_CODE = 0, MAST_ER_ROAD_NAME = "--No Roads Found--" });
                            return Json(new SelectList(list, "MAST_ER_ROAD_CODE", "MAST_ER_ROAD_NAME"), JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            list.Insert(0, new MASTER_EXISTING_ROADS { MAST_ER_ROAD_CODE = 0, MAST_ER_ROAD_NAME = "--Select--" });
                            return Json(new SelectList(list, "MAST_ER_ROAD_CODE", "MAST_ER_ROAD_NAME", selectedCode), JsonRequestBehavior.AllowGet);
                        }
                }

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        /// <summary>
        /// populates the Previous blocks dropdown
        /// </summary>
        /// <param name="blockName">represent the blockCode</param>
        /// <returns>list of Roads associated with the Previous Block</returns>
        [HttpPost]
        [Audit]
        public JsonResult GetPreviousBlockByBlockCode(string blockName)
        {
            try
            {
                int blockCode = Convert.ToInt32(blockName);
                CoreNetworkDAL objDAL = new CoreNetworkDAL();
                List<PLAN_ROAD> lstPreviousBlock = new List<PLAN_ROAD>();
                lstPreviousBlock = objDAL.GetPreviousBlockByBlockCode(blockCode);
                return Json(new SelectList(lstPreviousBlock, "PLAN_CN_ROAD_CODE", "PLAN_CN_ROAD_NUMBER"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }

        }

        /// <summary>
        /// list view of Habitations
        /// </summary>
        /// <param name="parameter">Encrypted id</param>
        /// <param name="hash">Encrypted id</param>
        /// <param name="key">Encrypted id</param>
        /// <returns>list view of Habitation details</returns>
        [Audit]
        public ActionResult ListHabitations(String parameter, String hash, String key)
        {
            try
            {
                HabitationDetailsViewModel model = new HabitationDetailsViewModel();
                CoreNetworkDAL objDAL = new CoreNetworkDAL();
                CommonFunctions objCommon = new CommonFunctions();
                Dictionary<string, string> decryptedParameters = null;
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                int networkCode = Convert.ToInt32(decryptedParameters["NetworkCode"]);
                model.UnlockFlag = decryptedParameters["UnlockFlag"];
                model.EncryptedRoadCode = networkCode.ToString();
                ViewBag.RoadNumber = db.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == networkCode).Select(m => m.PLAN_CN_ROAD_NUMBER).FirstOrDefault();

                ViewData["Habitations"] = objDAL.GetHabitationCodeList(networkCode);

                if (PMGSYSession.Current.PMGSYScheme == 1)
                {
                    ViewBag.Blocks = objCommon.PopulateBlocks(PMGSYSession.Current.DistrictCode, false);
                }
                if (PMGSYSession.Current.PMGSYScheme == 2)
                {
                    ViewBag.Roads = objDAL.GetRoadsByCNCodeCandidate(networkCode);
                }
                return PartialView("ListHabitations", model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occurred while processing the request" });
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
            int habCode = 0;

            long totalRecords = 0;
            string flag = string.Empty;
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }

            if (!string.IsNullOrEmpty(Request.Params["habCode"]))
            {
                habCode = Convert.ToInt32(Request.Params["habCode"]);
            }

            if (!string.IsNullOrEmpty(Request.Params["Flag"]))
            {
                flag = Request.Params["Flag"];
            }

            var jsonData = new
            {
                rows = objBAL.GetHabitationList(habCode, flag, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// add habitation to core network
        /// </summary>
        /// <param name="habModel">contains the details of Habitation</param>
        /// <returns>response message along with status</returns>
        [HttpPost]
        [Audit]
        public ActionResult MapHabitationToPlan(FormCollection habModel)
        {
            int roadCode = Convert.ToInt32(habModel["EncryptedHabCode"]);
            int habCode = Convert.ToInt32(habModel["Habitations"]);
            try
            {
                if (ModelState.IsValid)
                {
                    if (objBAL.AddHabitation(habCode, roadCode, ref message))
                    {
                        message = (message == string.Empty ? "Habitation added successfully." : message);
                        return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        message = message == string.Empty ? "Habitation not added successfully." : message;
                        return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Habitation not added successfully." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Error occurred while processing the request." : message;
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// deletes the Habitation details
        /// </summary>
        /// <param name="hash">Encrypted id</param>
        /// <param name="parameter">Encrypted id</param>
        /// <param name="key">Encrypted id</param>
        /// <param name="roadCode">road id</param>
        /// <returns>response message along with status</returns>
        [HttpPost]
        [Audit]
        public ActionResult DeleteMapHabitation(String hash, String parameter, String key, string roadCode)
        {
            Dictionary<string, string> decryptedParameters = null;
            int coreNetworkCode = Convert.ToInt32(roadCode);
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptedParameters.Count() > 0)
                {
                    if (!objBAL.DeleteMapHabitation(Convert.ToInt32(decryptedParameters["HabCode"].ToString()), decryptedParameters["Flag"].ToString(), coreNetworkCode))
                    {
                        ModelState.AddModelError(String.Empty, "Habitation not deleted successfully.");
                        return Json(new { success = false, message = "Habitation can not be deleted" }, JsonRequestBehavior.AllowGet);
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

        /// <summary>
        /// for chainage length of associated road 
        /// </summary>
        /// <param name="roadCode">represents the core network code</param>
        /// <returns>chainage length associated with the road </returns>
        [HttpPost]
        [Audit]
        public JsonResult GetChainageLength(string roadCode)
        {
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            try
            {
                MASTER_EXISTING_ROADS master = objDAL.GetRoadDetails(Convert.ToInt32(roadCode));//db.MASTER_EXISTING_ROADS.Find(Convert.ToInt32(roadCode));
                string startChainage = master.MAST_ER_ROAD_STR_CHAIN.ToString();
                string endChainage = master.MAST_ER_ROAD_END_CHAIN.ToString();

                double startLength = Convert.ToDouble(startChainage);
                double endLength = Convert.ToDouble(endChainage);
                double roadLength = endLength - startLength;
                return Json(new { startChainage = startChainage, endChainage = endChainage, roadLength = roadLength.ToString() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        /// <summary>
        /// returns the total population of a particular habitation
        /// </summary>
        /// <param name="habCode">habitation id representing habitation</param>
        /// <returns>total population of habitation id</returns>
        [HttpPost]
        [Audit]
        public JsonResult GetTotalPopulationByHabCode(string habCode)
        {
            int habitationCode = Convert.ToInt32(habCode);
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            string population = string.Empty;
            try
            {
                MASTER_HABITATIONS_DETAILS master = objDAL.GetHabitationDetails(habitationCode);//db.MASTER_HABITATIONS_DETAILS.Where(m => m.MAST_HAB_CODE == habitationCode).FirstOrDefault();
                population = master.MAST_HAB_TOT_POP.ToString();
                return Json(new { population = population }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        /// <summary>
        /// returns the list of habitations for dropdown
        /// </summary>
        /// <param name="roadCode">indicates the core network code</param>
        /// <returns>returns the list of habitations according to the core network</returns>
        [HttpPost]
        [Audit]
        public JsonResult PopulateHabitations(int roadCode)
        {
            try
            {
                CoreNetworkDAL objDAL = new CoreNetworkDAL();
                List<SelectListItem> lstHabitations = objDAL.GetHabitationCodeList(roadCode);
                return Json(lstHabitations);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }

        }

        /// <summary>
        /// returns road name associated with the road code
        /// </summary>
        /// <param name="roadCode">core network code</param>
        /// <returns>returns road name of particular road code</returns>
        [HttpPost]
        [Audit]
        public ActionResult GetRoadNameForHabitation(string roadCode)
        {
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            try
            {
                int roadId = Convert.ToInt32(roadCode);
                string roadName = objDAL.GetRoadName(roadId);
                return Content(roadName);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        /// <summary>
        /// Returns Road number from MASTER_EXISTING_ROAD by MAST_ER_ROAD_CODE(roadCode)
        /// in Add Core Network View 
        /// </summary>
        /// <param name="roadCode">MAST_ER_ROAD_CODE</param>
        /// <returns>MAST_ER_ROAD_NUMBER</returns>
        [HttpPost]
        [Audit]
        public ActionResult GetRoadNumberByExistRoadCode(int roadCode)
        {
            try
            {
                string roadNumber = (from item in db.MASTER_EXISTING_ROADS where item.MAST_ER_ROAD_CODE == roadCode select item.MAST_ER_ROAD_NUMBER).FirstOrDefault().ToString();
                return Content(roadNumber);
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
        /// returns the list of habitations mapped with the road
        /// </summary>
        /// <param name="mapCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetHabitationListToMap(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                int roadCode = 0, erRoadCode = 0;
                int blockCode = 0;
                long totalRecords = 0;

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["habCode"]))
                {
                    roadCode = Convert.ToInt32(Request.Params["habCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["erRoadCode"]))
                {
                    erRoadCode = Convert.ToInt32(Request.Params["erRoadCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["blockCode"]))
                {
                    blockCode = Convert.ToInt32(Request.Params["blockCode"]);
                }

                var jsonData = new
                {
                    rows = objBAL.GetHabitationListToMap(roadCode, blockCode, erRoadCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
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
        /// maps the habitation to the particular road
        /// </summary>
        /// <param name="mappedCollection">form collection containing the Habitation details</param>
        /// <returns>response message along with status</returns>
        [HttpPost]
        [Audit]
        public ActionResult MapHabitationsToNetwork(FormCollection mappedCollection)
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
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Error occurred while processing the request.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// checks the habitation and file uploaded details of particular road
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetCoreNetworkChecks()
        {
            try
            {
                int PLAN_CN_ROAD_CODE = Convert.ToInt32(Request.Params["PLAN_CN_ROAD_CODE"]);
                string status = objBAL.GetCoreNetworkChecksBAL(PLAN_CN_ROAD_CODE);
                if (status == string.Empty)
                    return Json(new { Success = true });
                else
                    return Json(new { Success = false, ErrorMessage = status }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { Success = false, message = "Error occurred while processing the request." });
            }

        }

        /// <summary>
        /// finalize the core network details
        /// </summary>
        /// <returns></returns>
        [Audit]
        public JsonResult FinalizeCoreNetwork()
        {
            try
            {
                int PLAN_CN_ROAD_CODE = Convert.ToInt32(Request.Params["PLAN_CN_ROAD_CODE"]);
                string status = objBAL.FinalizeCoreNetworkBAL(PLAN_CN_ROAD_CODE);
                if (status == string.Empty)
                    return Json(new { Success = true });
                else
                    return Json(new { Success = false, ErrorMessage = status });
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { Success = false, message = "Error occurred while processing the request." });
            }
        }

        [Audit]
        public ActionResult CheckPavementLength(string id)
        {
            try
            {
                String[] parameters = id.Split(',');
                bool status = objBAL.CheckPavementLength(Convert.ToInt32(parameters[0]), Convert.ToDecimal(parameters[1]));
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

                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        /// <summary>
        /// returns the Proposal list associated with this core network
        /// </summary>
        /// <param name="roadCode"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public ActionResult ListProposalByCoreNetwork(int? page, int? rows, string sidx, string sord)
        {
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

                if (!string.IsNullOrEmpty(Request.Params["RoadCode"]))
                {
                    roadCode = Convert.ToInt32(Request.Params["RoadCode"]);
                }

                var jsonData = new
                {
                    rows = objBAL.ListProposals(roadCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return null;
            }
        }

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
        /// returns blocks according to the state
        /// </summary>
        /// <param name="stateCode">indicates the state id</param>
        /// <returns></returns>
        [Audit]
        public JsonResult GetBlocksByDistrict(int districtCode)
        {
            CommonFunctions objCommon = new CommonFunctions();
            try
            {
                return Json(new SelectList(objCommon.PopulateBlocks(districtCode, true), "Value", "Text"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        #endregion

        #region PMGSY SCHEME 2

        /// <summary>
        /// returns the view for mapping the other candidate road
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public ActionResult MapOtherCandidateRoadView(String parameter, String hash, String key)
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                CoreNetworkDAL objDAL = new CoreNetworkDAL();
                CandidateRoadViewModel model = new CandidateRoadViewModel();
                if (!(String.IsNullOrEmpty(parameter)) && !(String.IsNullOrEmpty(hash)) && !(String.IsNullOrEmpty(key)))
                {
                    Dictionary<string, string> decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                    if (decryptedParameters != null)
                    {
                        model.CNCode = Convert.ToInt32(decryptedParameters["NetworkCode"]);
                    }
                }
                model.LockStatus = objDAL.GetLockStatusOfCandidateRoad(model.CNCode);
                //model.lstBlocks = objCommon.PopulateBlocks(PMGSYSession.Current.DistrictCode);
                model.lstBlocks = new SelectList(objDAL.GetBlocksByDistrictCode(PMGSYSession.Current.DistrictCode), "MAST_BLOCK_CODE", "MAST_BLOCK_NAME").ToList();
                model.lstBlocks.Insert(0, new SelectListItem { Value = "0", Text = "Select Block" });
                model.lstRoadCategory = new SelectList(objDAL.GetAllRoadCategories().Where(m => m.MAST_ROAD_SHORT_DESC == "RR(ODR)" || m.MAST_ROAD_SHORT_DESC == "RR(VR)" || m.MAST_ROAD_SHORT_DESC == "RR(TRACK)").ToList(), "MAST_ROAD_CAT_CODE", "MAST_ROAD_CAT_NAME").ToList();
                model.lstRoadCategory.Insert(0, new SelectListItem { Value = "0", Text = "- Select Road Category -" });
                model.lstDRRP.Insert(0, new SelectListItem { Value = "0", Text = "- Select Road Name -" });
                return PartialView(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MapOtherCandidateRoadView()");
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        /// <summary>
        /// returns the list of mapped candidate roads
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        public JsonResult GetMappedCandidateRoadList(int? page, int? rows, string sidx, string sord)
        {
            long totalRecords = 0;
            int candidateRoadCode = 0;
            string IsFinalized = String.Empty;
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["RoadCode"]))
                {
                    candidateRoadCode = Convert.ToInt32(Request.Params["RoadCode"]);
                }

                var jsonData = new
                {
                    rows = objBAL.ListCandidateRoads(candidateRoadCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, out IsFinalized),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                    IsFinalized = IsFinalized
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetMappedCandidateRoadList()");
                return null;
            }
        }

        /// <summary>
        /// Post method for adding the mapping details of candidate road
        /// </summary>
        /// <param name="model">model containing the mapping details</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MapCandidateRoad(CandidateRoadViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (objBAL.MapCandidateRoad(model, ref message))
                    {
                        message = message == string.Empty ? "DRRP details mapped successfully." : message;
                        return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        message = message == string.Empty ? "Error occurred while processing your request." : message;
                        ModelState.AddModelError("", message);
                        return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MapCandidateRoad()");
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        /// <summary>
        /// deletes the mapped DRRP Details from the Candidate Road
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteMappedDRRPDetails(String parameter, String hash, String key)
        {
            int DRRPCode = 0;
            int CNCode = 0;
            try
            {
                if (!(String.IsNullOrEmpty(parameter)) && !(String.IsNullOrEmpty(hash)) && !(String.IsNullOrEmpty(key)))
                {
                    Dictionary<string, string> decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                    if (decryptedParameters != null)
                    {
                        DRRPCode = Convert.ToInt32(decryptedParameters["DRRPCode"]);
                        CNCode = Convert.ToInt32(decryptedParameters["CNCode"]);
                    }
                }

                if (!(objBAL.DeleteMappedDRRPDetails(DRRPCode, CNCode)))
                {
                    return Json(new { success = false });
                }
                else
                {
                    return Json(new { success = true });
                }
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }
        }

        /// <summary>
        /// finalizes the DRRP Road Details for particular Candidate Road
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult FinalizeMappedDRRPDetails(String id)
        {
            int CNCode = 0;
            try
            {
                if (!String.IsNullOrEmpty(id))
                {
                    CNCode = Convert.ToInt32(id);
                    if (objBAL.FinalizeMappedDRRPDetails(CNCode))
                    {
                        return Json(new { success = true });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Error occurred while processing your request." });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Candidate Road details are not present." });
                }
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        /// <summary>
        /// finalizes the DRRP Road Details for particular Candidate Road
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeFinalizeMappedDRRPDetails(String id)
        {
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            int CNCode = 0;
            try
            {
                if (!String.IsNullOrEmpty(id))
                {
                    CNCode = Convert.ToInt32(id);
                    if (objDAL.DeFinalizeMappedDRRPDetails(CNCode))
                    {
                        return Json(new { success = true });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Error occurred while processing your request." });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Candidate Road details are not present." });
                }
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        /// <summary>
        /// returns the list of DRRP Mapped habitation list
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        public ActionResult GetDRRPMappedHabitationList(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                Dictionary<string, string> decryptedParameters = null;
                int DRRPCode = 0;
                int CNCode = 0;
                long totalRecords = 0;
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!String.IsNullOrEmpty(Request.Params["RoadCode"]))
                {
                    String[] urlparameters = Request.Params["RoadCode"].Split('/');
                    if (urlparameters != null)
                    {
                        decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { urlparameters[0], urlparameters[1], urlparameters[2] });
                        if (decryptedParameters != null)
                        {
                            DRRPCode = Convert.ToInt32(decryptedParameters["DRRPCode"]);
                            CNCode = Convert.ToInt32(decryptedParameters["CNCode"]);
                        }
                    }
                }

                var json = new
                {
                    rows = objBAL.ListDRRPMappedHabitations(DRRPCode, CNCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };

                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// deletes the habitation mapping details associated with the 
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public ActionResult DeleteMappedDRRPHabitation(String parameter, String hash, String key)
        {
            int DRRPCode = 0;
            int CNCode = 0;
            int habCode = 0;
            try
            {
                if (!(String.IsNullOrEmpty(parameter)) && !(String.IsNullOrEmpty(hash)) && !(String.IsNullOrEmpty(key)))
                {
                    Dictionary<string, string> decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                    if (decryptedParameters != null)
                    {
                        DRRPCode = Convert.ToInt32(decryptedParameters["DRRPCode"]);
                        CNCode = Convert.ToInt32(decryptedParameters["CNCode"]);
                        habCode = Convert.ToInt32(decryptedParameters["HabitationCode"]);
                    }
                }

                if (!(objBAL.DeleteMappedDRRPHabitation(DRRPCode, CNCode, habCode)))
                {
                    return Json(new { success = false });
                }
                else
                {
                    return Json(new { success = true });
                }
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        public ActionResult AddMappedDRRPDetails(String parameter, String hash, String key)
        {
            try
            {
                CoreNetworkDAL objDAL = new CoreNetworkDAL();
                CandidateRoadViewModel model = new CandidateRoadViewModel();
                if (!(String.IsNullOrEmpty(parameter)) && !(String.IsNullOrEmpty(hash)) && !(String.IsNullOrEmpty(key)))
                {
                    Dictionary<string, string> decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                    if (decryptedParameters != null)
                    {
                        model.CNCode = Convert.ToInt32(decryptedParameters["NetworkCode"]);
                    }
                }

                model.LockStatus = objDAL.GetLockStatusOfCandidateRoad(model.CNCode);
                //model.lstBlocks = objCommon.PopulateBlocks(PMGSYSession.Current.DistrictCode);
                model.lstBlocks = new SelectList(objDAL.GetBlocksByDistrictCode(PMGSYSession.Current.DistrictCode), "MAST_BLOCK_CODE", "MAST_BLOCK_NAME").ToList();
                model.lstBlocks.Insert(0, new SelectListItem { Value = "0", Text = "Select Block" });
                model.lstRoadCategory = new SelectList(objDAL.GetAllRoadCategories().Where(m => m.MAST_ROAD_SHORT_DESC == "RR(ODR)" || m.MAST_ROAD_SHORT_DESC == "RR(VR)" || m.MAST_ROAD_SHORT_DESC == "RR(TRACK)").ToList(), "MAST_ROAD_CAT_CODE", "MAST_ROAD_CAT_NAME").ToList();
                model.lstRoadCategory.Insert(0, new SelectListItem { Value = "0", Text = "- Select Road Category -" });
                model.lstDRRP.Insert(0, new SelectListItem { Value = "0", Text = "- Select Road Name -" });
                return PartialView(model);
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }

        }


        /// <summary>
        /// returns the DRRP details mapped with the Candidate Road for updation
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public ActionResult EditMappedDRRPDetails(String parameter, String hash, String key)
        {
            CandidateRoadViewModel model = new CandidateRoadViewModel();
            int DRRPCode = 0;
            int CNCode = 0;
            try
            {
                if (!(String.IsNullOrEmpty(parameter)) && !(String.IsNullOrEmpty(hash)) && !(String.IsNullOrEmpty(key)))
                {
                    Dictionary<string, string> decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                    if (decryptedParameters != null)
                    {
                        DRRPCode = Convert.ToInt32(decryptedParameters["DRRPCode"]);
                        CNCode = Convert.ToInt32(decryptedParameters["CNCode"]);
                    }
                }
                model = new CoreNetworkDAL().GetDRRPDetails(CNCode, DRRPCode);
                return PartialView("MapOtherCandidateRoadView", model);
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        #endregion

        #region FILE_UPLOAD

        /// <summary>
        /// returns the File Upload view
        /// </summary>
        /// <param name="hash">Encrypted id key</param>
        /// <param name="parameter">Encrypted id key</param>
        /// <param name="key">Encrypted id key</param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult FileUpload(String hash, String parameter, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            int coreNetworkCode = 0;
            CoreNetworkUploadFileViewModel fileUploadViewModel = new CoreNetworkUploadFileViewModel();
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptedParameters.Count() > 0)
                {
                    coreNetworkCode = Convert.ToInt32(decryptedParameters["NetworkCode"]);
                    fileUploadViewModel.PLAN_CN_ROAD_CODE = coreNetworkCode;

                    return PartialView("FileUpload", fileUploadViewModel);
                }
                else
                {
                    return PartialView("FileUpload", new CoreNetworkUploadFileViewModel());
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return PartialView("FileUpload", new CoreNetworkUploadFileViewModel());
            }

        }

        /// <summary>
        /// returns the file upload view with road details
        /// </summary>
        /// <param name="id">represents the core network code</param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult CoreNetworkFileUpload(string id)
        {
            try
            {
                CoreNetworkUploadFileViewModel fileUploadViewModel = new CoreNetworkUploadFileViewModel();
                fileUploadViewModel.PLAN_CN_ROAD_CODE = Convert.ToInt32(id);

                if (db.PLAN_ROAD_UPLOAD_FILE.Where(a => a.PLAN_CN_ROAD_CODE == fileUploadViewModel.PLAN_CN_ROAD_CODE).Any())
                {
                    fileUploadViewModel.NumberOfFiles = db.PLAN_ROAD_UPLOAD_FILE.Where(a => a.PLAN_CN_ROAD_CODE == fileUploadViewModel.PLAN_CN_ROAD_CODE).Count();
                }
                else
                {
                    fileUploadViewModel.NumberOfFiles = 0;
                }
                return View("CoreNetworkFileUpload", fileUploadViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return Json(new { success = false, message = "Error occurred while processing the request." });
            }

        }

        /// <summary>
        /// KML file upload
        /// </summary>
        /// <param name="fileUploadViewModel">contains the details associated with file</param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult KmlFileUpload(CoreNetworkUploadFileViewModel fileUploadViewModel)
        {
            try
            {    //Added By Abhishek kamble 27-Apr-2014
                CommonFunctions objCommonFunc = new CommonFunctions();
                if (!(objCommonFunc.ValidateIsKml(ConfigurationManager.AppSettings["CORE_NETWORK_FILE_UPLOAD"], Request)))
                {
                    fileUploadViewModel.ErrorMessage = "File Type is Not Allowed.";
                    return View("FileUpload", fileUploadViewModel.ErrorMessage);
                }

                var fileData = new List<CoreNetworkUploadFileViewModel>();

                int roadCode = 0;
                if (fileUploadViewModel.PLAN_CN_ROAD_CODE != 0)
                {
                    roadCode = fileUploadViewModel.PLAN_CN_ROAD_CODE;
                }
                else
                {
                    try
                    {
                        roadCode = Convert.ToInt32(Request["PLAN_CN_ROAD_CODE"]);
                    }
                    catch (Exception ex)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                        if (Request["PLAN_CN_ROAD_CODE"].Contains(','))
                        {
                            roadCode = Convert.ToInt32(Request["PLAN_CN_ROAD_CODE"].Split(',')[0]);
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
        [Audit]
        public void UploadPDFFile(HttpRequestBase request, List<CoreNetworkUploadFileViewModel> statuses, int roadCode)
        {
            try
            {
                String StorageRoot = ConfigurationManager.AppSettings["CORE_NETWORK_FILE_UPLOAD"];
                int MaxCount = 0;
                String message = String.Empty;
                for (int i = 0; i < request.Files.Count; i++)
                {
                    HttpPostedFileBase file = request.Files[i];
                    var fileId = roadCode;
                    if (db.PLAN_ROAD_UPLOAD_FILE.Where(a => a.PLAN_CN_ROAD_CODE == roadCode).Any())
                    {
                        MaxCount = db.PLAN_ROAD_UPLOAD_FILE.Where(a => a.PLAN_CN_ROAD_CODE == roadCode).Count();
                    }
                    MaxCount++;

                    var fileName = roadCode + "-" + MaxCount + Path.GetExtension(request.Files[i].FileName).ToString();
                    var fullPath = Path.Combine(StorageRoot, fileName);

                    statuses.Add(new CoreNetworkUploadFileViewModel()
                    {
                        url = fullPath,
                        thumbnail_url = fullPath,
                        name = fileName,
                        type = file.ContentType,
                        size = file.ContentLength,
                        delete_url = "",
                        delete_type = "DELETE",
                        PLAN_START_CHAINAGE = Convert.ToDecimal(request.Params["StartChainage[]"]),
                        PLAN_END_CHAINAGE = Convert.ToDecimal(request.Params["EndChainage[]"]),
                        PLAN_CN_ROAD_CODE = roadCode
                    });

                    string status = objBAL.AddFileUploadDetailsBAL(statuses, ref message);
                    if (status == string.Empty)
                    {

                        //file.SaveAs(fullPath);
                        file.SaveAs(Path.Combine(ConfigurationManager.AppSettings["CORE_NETWORK_FILE_UPLOAD"], fileName));
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

        /// <summary>
        /// returns the list of files 
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult ListFiles(FormCollection formCollection)
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

                int PLAN_CN_ROAD_CODE = Convert.ToInt32(Request["PLAN_CN_ROAD_CODE"]);
                long totalRecords;
                var jsonData = new
                {
                    rows = objBAL.GetFilesListBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, PLAN_CN_ROAD_CODE),
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
        /// method for downloading the files
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
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

                if (FileExtension == ".kml")
                {
                    FullFileLogicalPath = Path.Combine(ConfigurationManager.AppSettings["CORE_NETWORK_FILE_UPLOAD_VIRTUAL_DIR_PATH"], FileName);
                    FullfilePhysicalPath = Path.Combine(ConfigurationManager.AppSettings["CORE_NETWORK_FILE_UPLOAD"], FileName);
                }


                string name = Path.GetFileName(FileName);
                string ext = Path.GetExtension(FileName);

                string type = string.Empty;

                if (ext != null)
                {
                    switch (ext.ToLower())
                    {
                        case ".kml":
                            type = "Application/kml";
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

                if (System.IO.File.Exists(FullfilePhysicalPath))
                {
                    return File(FullfilePhysicalPath, type, DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + FileExtension);
                }
                else
                {
                    return Json(new { message = "File not Found" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.DownloadFile()");
                return Json(new { Success = "false" }, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// Updates the file details
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult UpdatePDFDetails(FormCollection formCollection)
        {
            try
            {
                string[] arrKey = formCollection["id"].Split('$');
                CoreNetworkUploadFileViewModel fileuploadViewModel = new CoreNetworkUploadFileViewModel();
                fileuploadViewModel.PLAN_CN_ROAD_CODE = Convert.ToInt32(arrKey[1]);
                fileuploadViewModel.PLAN_FILE_ID = Convert.ToInt32(arrKey[0]);
                String message = String.Empty;


                Regex regex = new Regex(@"^\d{1,16}(\.\d{1,2})?$");
                if (regex.IsMatch(formCollection["PLAN_START_CHAINAGE"]))
                {
                    fileuploadViewModel.PLAN_START_CHAINAGE = Convert.ToDecimal(formCollection["PLAN_START_CHAINAGE"]);
                }
                else
                {
                    return Json("Invalid Start Chainage, Only Numbers and Decimals upto two digits are allowed");
                }

                if (regex.IsMatch(formCollection["PLAN_END_CHAINAGE"]))
                {
                    fileuploadViewModel.PLAN_END_CHAINAGE = Convert.ToDecimal(formCollection["PLAN_END_CHAINAGE"]);
                }
                else
                {
                    return Json("Invalid End Chainage, Only Numbers and Decimals upto two digits are allowed");
                }

                if (Convert.ToDecimal(formCollection["PLAN_END_CHAINAGE"]) <= Convert.ToDecimal(formCollection["PLAN_START_CHAINAGE"]))
                {
                    return Json("Start Chainage must be less than End Chainage.");
                }

                string status = objBAL.UpdateFileDetailsBAL(fileuploadViewModel, ref message);

                if (status == string.Empty)
                    return Json(true);
                else if (!String.IsNullOrEmpty(message))
                {
                    return Json(message);
                }
                else
                    return Json("There is an error occured while processing your request.");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json("There is an error occured while processing your request.");
            }

        }

        /// <summary>
        /// deletes the particular file with details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult DeleteFileDetails(string id)
        {
            try
            {
                String PhysicalPath = string.Empty;
                PhysicalPath = ConfigurationManager.AppSettings["CORE_NETWORK_FILE_UPLOAD"];

                string[] arrParam = Request.Params["PLAN_CN_ROAD_CODE"].Split('$');

                int PLAN_FILE_ID = Convert.ToInt32(arrParam[0]);
                int PLAN_CN_ROAD_CODE = Convert.ToInt32(arrParam[1]);
                string PLAN_FILE_NAME = Request.Params["PLAN_FILE_NAME"];
                PhysicalPath = Path.Combine(PhysicalPath, PLAN_FILE_NAME);

                if (!System.IO.File.Exists(PhysicalPath))
                {
                    return Json(new { Success = false, ErrorMessage = "File Not Found." });
                }

                string status = objBAL.DeleteFileDetails(PLAN_FILE_ID, PLAN_CN_ROAD_CODE, PLAN_FILE_NAME);

                if (status == string.Empty)
                {
                    try
                    {
                        System.IO.File.Delete(PhysicalPath);
                        return Json(new { Success = true, ErrorMessage = "File Deleted Successfully." });
                    }
                    catch (Exception ex)
                    {
                        return Json(new { Success = true, ErrorMessage = ex.Message });
                    }
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = status });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { Success = false, ErrorMessage = "There is an error while processing your request." });
            }
        }




        #endregion

        #region RCPLWE

        /// <summary>
        /// Returns the list of RCPLWE details
        /// </summary>
        /// <returns>List View of Core Network details</returns>
        [HttpGet]
        [Audit]
        public ActionResult ListRCPLWE()
        {
            List<MASTER_BLOCK> lst = new List<MASTER_BLOCK>();
            int block = 0;
            try
            {
                if (PMGSYSession.Current == null)
                {
                    Response.Redirect("/Login/Login");
                }

                CommonFunctions objCommon = new CommonFunctions();
                int districtCode = PMGSYSession.Current.DistrictCode;
                CoreNetworkDAL objDAL = new CoreNetworkDAL();
                ViewBag.Scheme = PMGSYSession.Current.PMGSYScheme;
                ViewData["Route"] = objDAL.GetAllRoutes();
                ViewData["Category"] = objDAL.GetCategoryForSearch();

                List<SelectListItem> lstDefault = new List<SelectListItem>();
                lstDefault.Insert(0, new SelectListItem { Value = "0", Text = "All" });
                if (PMGSYSession.Current.RoleCode == 25 || PMGSY.Extensions.PMGSYSession.Current.RoleCode == 53 || PMGSYSession.Current.RoleCode == 65)//Changes by SAMMED A. PATIL for mordviewuser
                {
                    ViewData["States"] = objCommon.PopulateStates(true);
                    ViewData["Districts"] = lstDefault;
                    ViewData["Blocks"] = lstDefault;
                }
                else if (PMGSYSession.Current.RoleCode == 2)
                {
                    ViewData["Districts"] = objCommon.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                    ViewData["Blocks"] = lstDefault;
                }
                else
                {
                    SelectList blockList = new SelectList(objDAL.GetBlocksByDistrictCodeRCPLWE(districtCode), "MAST_BLOCK_CODE", "MAST_BLOCK_NAME");
                    if (blockList.Count() > 0)
                    {
                        ViewData["Blocks"] = blockList;
                    }
                    lst = objDAL.GetBlocksByDistrictCode(districtCode);
                }

                ViewData["IsLocked"] = objDAL.checkIsLocked(block);

                return View();
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return View();
            }
        }

        /// <summary>
        /// returns blocks according to the state
        /// </summary>
        /// <param name="stateCode">indicates the state id</param>
        /// <returns></returns>
        [Audit]
        public JsonResult GetBlocksByDistrictRCPLWE(int districtCode)
        {
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            CommonFunctions objCommon = new CommonFunctions();
            try
            {
                SelectList blockList = new SelectList(objDAL.GetBlocksByDistrictCodeRCPLWE(districtCode), "MAST_BLOCK_CODE", "MAST_BLOCK_NAME");
                if (blockList.Count() <= 0)
                {
                    List<SelectListItem> lst = new List<SelectListItem>();
                    lst.Insert(0, new SelectListItem { Text = "Select Block", Value = "-1" });
                    blockList = new SelectList(lst, "Value", "Text");
                }
                return Json(blockList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        /// <summary>
        /// returns the list of RCPLWE details for populating the grid
        /// </summary>
        /// <param name="networkCollection">form collection containing Core Network Grid Parameters</param>
        /// <returns>Json data containing list to populate grid</returns>
        [HttpPost]
        [Audit]
        public ActionResult GetRCPLWEList(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                if (PMGSYSession.Current == null)
                {
                    Response.Redirect("/Login/Login");
                }

                int blockCode = 0;
                int districtCode = 0;
                int stateCode = 0;
                int categoryCode = 0;
                long totalRecords = 0;
                string roadType = string.Empty;
                String searchParameters = String.Empty;
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                string roadName = string.Empty;
                int CNCode = 0;

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

                if (!string.IsNullOrEmpty(Request.Params["blockCode"]))
                {
                    blockCode = Convert.ToInt32(Request.Params["blockCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["categoryCode"]))
                {
                    categoryCode = Convert.ToInt32(Request.Params["categoryCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["roadType"]))
                {
                    roadType = Request.Params["roadType"];
                }

                if (!string.IsNullOrEmpty(Request.Params["PLAN_RD_NAME"]))
                {
                    roadName = Request.Params["PLAN_RD_NAME"];
                }

                if (!string.IsNullOrEmpty(Request.Params["CNCode"]))
                {
                    CNCode = Convert.ToInt32(Request.Params["CNCode"]);
                }

                var jsonData = new
                {
                    rows = objBAL.GetRCPLWEList(stateCode, districtCode, blockCode, roadType, categoryCode, roadName, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, CNCode),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        /// <summary>
        /// returns the RCPLWE add view
        /// </summary>
        /// <param name="blockCode">id of block</param>
        /// <returns>returns the add view of Core Network</returns>
        [Audit]
        public ActionResult AddEditRCPLWE(int blockCode)
        {
            CoreNetworkViewModel model = new CoreNetworkViewModel();
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            bool flag = false;
            try
            {
                model.BLOCK_NAME = objDAL.GetBlockName(blockCode);
                model.MAST_BLOCK_CODE = blockCode;
                List<SelectListItem> lstRoadNumber = new List<SelectListItem>();
                lstRoadNumber.Add(new SelectListItem { Value = "0", Text = "--Select Road--", Selected = true });
                lstRoadNumber.Add(new SelectListItem { Value = "H", Text = "Habitation" });
                lstRoadNumber.Add(new SelectListItem { Value = "D", Text = "Major District Road" });
                lstRoadNumber.Add(new SelectListItem { Value = "N", Text = "National Highway" });
                lstRoadNumber.Add(new SelectListItem { Value = "R", Text = "Rural Road(Other District Roads)" });
                lstRoadNumber.Add(new SelectListItem { Value = "Z", Text = "Rural Road(Track)" });
                lstRoadNumber.Add(new SelectListItem { Value = "V", Text = "Rural Road(Village Roads)" });
                lstRoadNumber.Add(new SelectListItem { Value = "S", Text = "State Highway" });

                //ViewData["RoadFrom"] = model.Road;
                ViewData["RoadFrom"] = new SelectList(lstRoadNumber, "Value", "Text");
                ViewData["RoadTo"] = new SelectList(lstRoadNumber, "Value", "Text");
                //ViewData["RoadTo"] = model.RoadTo;
                ViewData["RoadNumFrom"] = model.RoadNumFromList;
                ViewData["RoadNumTo"] = model.RoadNumToList;
                ViewData["Category"] = model.RoadCategory;
                ViewData["PreviousBlock"] = new SelectList(objDAL.GetBlocksByDistrictCode(PMGSYSession.Current.DistrictCode, blockCode), "MAST_BLOCK_CODE", "MAST_BLOCK_NAME");
                ViewData["PreviousBlockRoadNo"] = new SelectList(lstRoadNumber, "Value", "Text");
                ViewData["RoadNumber"] = new SelectList(lstRoadNumber, "Value", "Text");
                ViewData["HabitationFrom"] = new SelectList(objDAL.GetHabitationsByBlockCode(blockCode), "Value", "Text");
                ViewData["HabitationTo"] = new SelectList(objDAL.GetHabitationsByBlockCode(blockCode), "Value", "Text");
                ViewData["NextBlock"] = new SelectList(objDAL.GetBlocksByDistrictCode(PMGSYSession.Current.DistrictCode, blockCode), "MAST_BLOCK_CODE", "MAST_BLOCK_NAME");
                ViewData["NextBlockRoadNo"] = new SelectList(lstRoadNumber, "Value", "Text");


                List<SelectListItem> lstRoute = new List<SelectListItem>();
                lstRoute.Add(new SelectListItem { Value = "0", Text = "--Select Route--", Selected = true });
                lstRoute.Add(new SelectListItem { Value = "T", Text = "Through Route" });
                if (PMGSYSession.Current.PMGSYScheme == 3)
                {
                    lstRoute.Add(new SelectListItem { Value = "L", Text = "Link Route" });
                }
                else if (PMGSYSession.Current.PMGSYScheme == 2)
                {
                    lstRoute.Add(new SelectListItem { Value = "M", Text = "Major Rural Link" });
                }

                if (PMGSYSession.Current.PMGSYScheme == 1)
                {
                    flag = checkSchedule5(blockCode);
                    if (flag)
                    {
                        lstRoute.Add(new SelectListItem { Value = "N", Text = "Missing Link" });
                    }
                }
                model.RouteType = lstRoute;
                return PartialView("AddEditRCPLWE", model);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return PartialView();
            }
        }

        /// <summary>
        /// saves the inserted details of RCPLWE
        /// </summary>
        /// <param name="model">contains the data of Core Network details</param>
        /// <returns>response message along with status</returns>
        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult AddRCPLWE(CoreNetworkViewModel model)
        {
            bool status = false;
            try
            {
                ModelState.Remove("PLAN_RD_ROUTE");
                ModelState.Remove("PLAN_CN_ROAD_NUMBER");

                if (ModelState.IsValid)
                {
                    model.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                    model.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                    if (string.IsNullOrEmpty(Convert.ToString(model.TotalLengthOfCandidate)))
                    {
                        message = message == string.Empty ? "Please enter Total length of RCPLWE road" : message;
                        return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                    }

                    if (objBAL.AddRCPLWE(model, ref message))
                    {
                        if (PMGSYSession.Current.PMGSYScheme == 3)
                        {
                            message = message == string.Empty ? "RCPLWE details added successfully." : message;
                        }
                        ///PMGSY 2 not required for RCPLWE
                        //else if (PMGSYSession.Current.PMGSYScheme == 2)
                        //{
                        //    message = message == string.Empty ? "RCPLWE details added successfully." : message;
                        //}
                        status = true;
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        message = message == string.Empty ? "Error occurred while processing the request." : message;
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
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

                return Json(new { success = false, message = "Error occurred while processing the request." });
                //return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// returns the RCPLWE details for updation
        /// </summary>
        /// <param name="parameter">Encrypted id</param>
        /// <param name="hash">Encrypted id</param>
        /// <param name="key">Encrypted id</param>
        /// <returns>data of Core Network road for updation</returns>
        [Audit]
        public ActionResult EditRCPLWE(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptParameters = null;
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            List<SelectListItem> lstRoadNumber = new List<SelectListItem>();
            bool flag = false;
            try
            {
                decryptParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptParameters.Count() > 0)
                {
                    CoreNetworkViewModel model = objBAL.GetCoreNetworkDetails(Convert.ToInt32(decryptParameters["NetworkCode"]));

                    if (decryptParameters.Count > 1)
                    {
                        model.LockUnlockFlag = decryptParameters["UnlockFlag"];
                    }

                    List<SelectListItem> lstRoute = new List<SelectListItem>();
                    lstRoute.Add(new SelectListItem { Value = "0", Text = "--Select Route--", Selected = true });
                    lstRoute.Add(new SelectListItem { Value = "T", Text = "Through Route" });
                    if (PMGSYSession.Current.PMGSYScheme == 3)
                    {
                        lstRoute.Add(new SelectListItem { Value = "L", Text = "Link Route" });
                    }
                    else if (PMGSYSession.Current.PMGSYScheme == 2)
                    {
                        lstRoute.Add(new SelectListItem { Value = "M", Text = "Major Rural Link" });
                    }
                    //return new SelectList(lstRoute,"Value","Text");
                    if (PMGSYSession.Current.PMGSYScheme == 1)
                    {
                        flag = checkSchedule5(model.MAST_BLOCK_CODE);
                        if (flag)
                        {
                            lstRoute.Add(new SelectListItem { Value = "N", Text = "Missing Link" });
                        }
                    }
                    model.RouteType = lstRoute;

                    model.FROM_TYPE = model.PLAN_RD_FROM_TYPE;
                    model.TO_TYPE = model.PLAN_RD_TO_TYPE;
                    model.RD_FROM = model.PLAN_RD_FROM;
                    model.RD_TO = model.PLAN_RD_TO;
                    model.ROAD_CODE = db.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == model.MAST_ER_ROAD_CODE).Select(r => r.MAST_ER_ROAD_NUMBER).FirstOrDefault();
                    model.BLOCK_NAME = objDAL.GetBlockName(model.MAST_BLOCK_CODE);//db.MASTER_BLOCK.Where(m => m.MAST_BLOCK_CODE == model.MAST_BLOCK_CODE).Select(b => b.MAST_BLOCK_NAME).FirstOrDefault();
                    if (model.PLAN_RD_NUM_FROM == null)
                    {
                        model.NUM_FROM = null;
                    }
                    else
                    {
                        model.NUM_FROM = model.PLAN_RD_NUM_FROM;
                    }

                    if (model.PLAN_RD_NUM_TO == null)
                    {
                        model.NUM_TO = null;
                    }
                    else
                    {
                        model.NUM_TO = model.PLAN_RD_NUM_TO;
                    }

                    if (model == null)
                    {
                        ModelState.AddModelError(String.Empty, "RCPLWE details not exist.");
                        return PartialView("AddEditRCPLWE", new CoreNetworkViewModel());
                    }

                    //ViewData["RoadFrom"] = model.Road;
                    //ViewData["RoadTo"] = model.RoadTo;

                    lstRoadNumber = new List<SelectListItem>();
                    lstRoadNumber.Add(new SelectListItem { Value = "0", Text = "--Select Road--", Selected = true });
                    lstRoadNumber.Add(new SelectListItem { Value = "H", Text = "Habitation" });
                    lstRoadNumber.Add(new SelectListItem { Value = "N", Text = "National Highway" });
                    lstRoadNumber.Add(new SelectListItem { Value = "R", Text = "Rural Road(Other District Roads)" });
                    lstRoadNumber.Add(new SelectListItem { Value = "Z", Text = "Rural Road(Track)" });
                    lstRoadNumber.Add(new SelectListItem { Value = "V", Text = "Rural Road(Village Roads)" });
                    lstRoadNumber.Add(new SelectListItem { Value = "S", Text = "State Highway" });

                    //ViewData["RoadFrom"] = model.Road;
                    ViewData["RoadFrom"] = new SelectList(lstRoadNumber, "Value", "Text");
                    ViewData["RoadTo"] = new SelectList(lstRoadNumber, "Value", "Text");

                    switch (model.PLAN_RD_FROM_TYPE)
                    {
                        case "T":
                            ViewData["RoadNumFrom"] = new SelectList(objDAL.GetRoadNumFromThroughEditList(model.PLAN_RD_FROM_TYPE, model.MAST_BLOCK_CODE, model.PLAN_CN_ROAD_CODE), "Value", "Text", model.PLAN_RD_NUM_FROM);
                            break;
                        case "L":
                            ViewData["RoadNumFrom"] = new SelectList(objDAL.GetRoadNumFromThroughEditList(model.PLAN_RD_FROM_TYPE, model.MAST_BLOCK_CODE, model.PLAN_CN_ROAD_CODE), "Value", "Text", model.PLAN_RD_NUM_FROM);
                            break;
                        case "M":
                            ViewData["RoadNumFrom"] = new SelectList(objDAL.GetRoadNumFromThroughEditList(model.PLAN_RD_FROM_TYPE, model.MAST_BLOCK_CODE, model.PLAN_CN_ROAD_CODE), "Value", "Text", model.PLAN_RD_NUM_FROM);
                            break;
                        case "H":
                            ViewData["RoadNumFrom"] = model.RoadNumFromList;
                            break;
                        case "B":
                            ViewData["RoadNumFrom"] = model.RoadNumFromList;
                            break;
                        default:
                            ViewData["RoadNumFrom"] = new SelectList(objDAL.GetRoadNumFromByRoadFrom(model.PLAN_RD_FROM_TYPE, model.MAST_BLOCK_CODE), "MAST_ER_ROAD_CODE", "MAST_ER_ROAD_NAME", model.PLAN_RD_NUM_FROM);
                            break;
                    }

                    switch (model.PLAN_RD_TO_TYPE)
                    {
                        case "T":
                            ViewData["RoadNumTo"] = new SelectList(objDAL.GetRoadNumFromThroughList(model.PLAN_RD_TO_TYPE, model.MAST_BLOCK_CODE), "PLAN_CN_ROAD_CODE", "PLAN_CN_ROAD_NUMBER", model.PLAN_RD_NUM_TO);
                            break;
                        case "L":
                            ViewData["RoadNumTo"] = new SelectList(objDAL.GetRoadNumFromThroughList(model.PLAN_RD_TO_TYPE, model.MAST_BLOCK_CODE), "PLAN_CN_ROAD_CODE", "PLAN_CN_ROAD_NUMBER", model.PLAN_RD_NUM_TO);
                            break;
                        case "M":
                            ViewData["RoadNumTo"] = new SelectList(objDAL.GetRoadNumFromThroughList(model.PLAN_RD_TO_TYPE, model.MAST_BLOCK_CODE), "PLAN_CN_ROAD_CODE", "PLAN_CN_ROAD_NUMBER", model.PLAN_RD_NUM_TO);
                            break;
                        case "H":
                            ViewData["RoadNumTo"] = model.RoadNumToList;
                            break;
                        case "B":
                            ViewData["RoadNumTo"] = model.RoadNumToList;
                            break;
                        default:
                            ViewData["RoadNumTo"] = new SelectList(objDAL.GetRoadNumFromByRoadFrom(model.PLAN_RD_TO_TYPE, model.MAST_BLOCK_CODE), "MAST_ER_ROAD_CODE", "MAST_ER_ROAD_NAME", model.PLAN_RD_NUM_TO);
                            break;
                    }
                    if (model.PLAN_RD_ROUTE == "N")
                    {
                        List<string> lst = objDAL.MLRoadList(model.MAST_BLOCK_CODE);
                        List<SelectListItem> lstRoadNumberML = new List<SelectListItem>();
                        lstRoadNumberML.Insert(0, new SelectListItem { Value = "A", Text = "--Select Road No--" });
                        for (int i = 1; i <= 30; i++)
                        {
                            if (i < 10)
                            {
                                lstRoadNumberML.Add(new SelectListItem { Value = "ML0" + i.ToString(), Text = "ML0" + i.ToString() });
                            }
                            else
                            {
                                lstRoadNumberML.Add(new SelectListItem { Value = "ML" + i.ToString(), Text = "ML" + i.ToString() });
                            }
                        }

                        var list = (from item in lstRoadNumberML
                                    where !lst.Contains(item.Value)
                                    select new
                                    {
                                        item.Value,
                                        item.Text
                                    }).Distinct().ToList().Select(x => new SelectListItem
                                    {
                                        Text = x.Text,
                                        Value = x.Value
                                    }).ToList();

                        list.Add(new SelectListItem { Value = model.PLAN_CN_ROAD_NUMBER, Text = model.PLAN_CN_ROAD_NUMBER });
                        //list.Insert(list.Count, SelectListItem { Value = model.PLAN_CN_ROAD_NUMBER, Text = model.PLAN_CN_ROAD_NUMBER });
                        //new SelectList(list, "Value", "Text")

                        ViewData["RoadNumber"] = list;
                    }
                    else
                    {
                        JsonResult a = GetRoadNumberByRoadRoute(model.PLAN_RD_ROUTE, model.MAST_BLOCK_CODE, model.PLAN_CN_ROAD_CODE);
                        ViewData["RoadNumber"] = a.Data;
                    }
                    ViewData["RoadNumThroughFrom"] = model.RoadFrom;
                    ViewData["RoadNumThroughTo"] = model.RoadFrom;
                    //ViewData["RoadNumThroughTo"] = new SelectList(lstRoadNumber, "Value", "Text");
                    ViewBag.RoadCategory = objDAL.GetRoadCategory(model.MAST_ER_ROAD_CODE);
                    ViewData["PreviousBlock"] = new SelectList(objDAL.GetBlocksByDistrictCode(PMGSYSession.Current.DistrictCode), "MAST_BLOCK_CODE", "MAST_BLOCK_NAME");
                    ViewData["PreviousBlockRoadNo"] = new SelectList(objDAL.GetPreviousBlockByBlockCode(model.MAST_BLOCK_CODE), "PLAN_CN_ROAD_CODE", "PLAN_CN_ROAD_NUMBER");
                    //ViewData["RoadNumber"] = new SelectList(lstRoadNumber, "Value", "Text");
                    ViewData["HabitationFrom"] = new SelectList(objDAL.GetHabitationsByBlockCode(model.MAST_BLOCK_CODE), "Value", "Text");
                    ViewData["HabitationTo"] = new SelectList(objDAL.GetHabitationsByBlockCode(model.MAST_BLOCK_CODE), "Value", "Text");
                    ViewData["NextBlock"] = new SelectList(objDAL.GetBlocksByDistrictCode(PMGSYSession.Current.DistrictCode), "MAST_BLOCK_CODE", "MAST_BLOCK_NAME");
                    ViewData["NextBlockRoadNo"] = new SelectList(objDAL.GetPreviousBlockByBlockCode(model.MAST_BLOCK_CODE), "PLAN_CN_ROAD_CODE", "PLAN_CN_ROAD_NUMBER");
                    return PartialView("AddEditRCPLWE", model);
                }
                return PartialView("AddEditRCPLWE", new CoreNetworkViewModel());
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return Json(new { success = false });
            }
        }

        /// <summary>
        /// save the updated details of RCPLWE
        /// </summary>
        /// <param name="model">contains the updated Core Network details</param>
        /// <returns>response message along with status of operation</returns>
        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult EditRCPLWE(CoreNetworkViewModel model)
        {
            bool status = false;
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            try
            {
                ModelState.Remove("PLAN_RD_ROUTE");
                ModelState.Remove("PLAN_CN_ROAD_NUMBER");

                if (ModelState.IsValid)
                {
                    if (objBAL.EditRCPLWE(model, ref message))
                    {
                        if (PMGSYSession.Current.PMGSYScheme == 3)
                        {
                            message = message == string.Empty ? "RCPLWE details updated successfully." : message;
                        }
                        ///PMGSY 2 not required for RCPLWE
                        //else if (PMGSYSession.Current.PMGSYScheme == 2)
                        //{
                        //    message = message == string.Empty ? "RCPLWE details updated successfully." : message;
                        //}
                        status = true;
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        message = message == string.Empty ? "Error occurred while processing your request." : message;
                        ModelState.AddModelError("", message);
                        return Json(new { success = status, message = new CommonFunctions().FormatErrorMessage(ModelState) }, JsonRequestBehavior.AllowGet);
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

                message = message == string.Empty ? "Error occurred while processing your request." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// deletes the particular RCPLWE details
        /// </summary>
        /// <param name="parameter">Encrypted id</param>
        /// <param name="hash">Encrypted id</param>
        /// <param name="key">Encrypted id</param>
        /// <returns>status of delete operation </returns>
        [HttpPost]
        [Audit]
        public ActionResult DeleteRCPLWE(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptedParameters.Count() > 0)
                {
                    if (!objBAL.DeleteRCPLWE(Convert.ToInt32(decryptedParameters["NetworkCode"].ToString())))
                    {
                        if (PMGSYSession.Current.PMGSYScheme == 3)
                        {
                            ModelState.AddModelError(String.Empty, "RCPLWE details are in use and can not be deleted.");
                            message = "RCPLWE details are in use and can not be deleted.";
                        }
                        ///PMGSY 2 not required for RCPLWE
                        //else if (PMGSYSession.Current.PMGSYScheme == 2)
                        //{
                        //    ModelState.AddModelError(String.Empty, "RCPLWE details are in use and can not be deleted.");
                        //    message = "RCPLWE details are in use and can not be deleted.";
                        //}
                        return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                if (PMGSYSession.Current.PMGSYScheme == 3)
                {
                    message = "RCPLWE details deleted successfully.";
                }
                //else if (PMGSYSession.Current.PMGSYScheme == 2)
                //{
                //    message = "RCPLWE details deleted successfully.";
                //}
                return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return Json(new { success = false, message = "RCPLWE details can not be deleted" }, JsonRequestBehavior.AllowGet);
            }
        }

        #region Habitation
        /// <summary>
        /// list view of Habitations for RCPLWE
        /// </summary>
        /// <param name="parameter">Encrypted id</param>
        /// <param name="hash">Encrypted id</param>
        /// <param name="key">Encrypted id</param>
        /// <returns>list view of Habitation details</returns>
        [Audit]
        public ActionResult ListHabitationsRCPLWE(String parameter, String hash, String key)
        {
            try
            {
                HabitationDetailsViewModel model = new HabitationDetailsViewModel();
                CoreNetworkDAL objDAL = new CoreNetworkDAL();
                CommonFunctions objCommon = new CommonFunctions();
                Dictionary<string, string> decryptedParameters = null;
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                int networkCode = Convert.ToInt32(decryptedParameters["NetworkCode"]);
                model.UnlockFlag = decryptedParameters["UnlockFlag"];
                model.EncryptedRoadCode = networkCode.ToString();
                ViewBag.RoadNumber = db.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == networkCode).Select(m => m.PLAN_CN_ROAD_NUMBER).FirstOrDefault();
                ViewData["Habitations"] = objDAL.GetHabitationCodeList(networkCode);
                ViewBag.Roads = objDAL.GetRoadsByCNCodeRCPLWE(networkCode);
                return PartialView("ListHabitationsRCPLWE", model);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occurred while processing the request" });
            }

        }

        /// <summary>
        /// returns the list of habitations mapped with the road
        /// </summary>
        /// <param name="mapCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetHabitationListToMapRCPLWE(int? page, int? rows, string sidx, string sord)
        {
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            try
            {
                if (PMGSYSession.Current == null)
                {
                    Response.Redirect("/Login/Login");
                }

                int roadCode = 0;
                int erRoadCode = 0;
                long totalRecords = 0;

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["habCode"]))
                {
                    roadCode = Convert.ToInt32(Request.Params["habCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["erRoadCode"]))
                {
                    erRoadCode = Convert.ToInt32(Request.Params["erRoadCode"]);
                }

                var jsonData = new
                {
                    rows = objBAL.GetHabitationListToMapRCPLWE(roadCode, erRoadCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        /// <summary>
        /// populating the list of Habitations RCPLWE
        /// </summary>
        /// <param name="page">no. of pages</param>
        /// <param name="rows">no. of rows</param>
        /// <param name="sidx">sort column name</param>
        /// <param name="sord">sort order</param>
        /// <returns>retuns the list of habitations</returns>
        [HttpPost]
        [Audit]
        public ActionResult GetHabitationListRCPLWE(int? page, int? rows, string sidx, string sord)
        {
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            int habCode = 0;

            long totalRecords = 0;
            string flag = string.Empty;
            try
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

                if (!string.IsNullOrEmpty(Request.Params["habCode"]))
                {
                    habCode = Convert.ToInt32(Request.Params["habCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["Flag"]))
                {
                    flag = Request.Params["Flag"];
                }

                var jsonData = new
                {
                    rows = objBAL.GetHabitationListRCPLWE(habCode, flag, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
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
        public ActionResult MapHabitationsToNetworkRCPLWE(FormCollection mappedCollection)
        {
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            bool status = false;
            string encryptedHabCodes = string.Empty;
            string roadName = string.Empty;
            try
            {
                encryptedHabCodes = mappedCollection["EncryptedHabCodes"];
                roadName = mappedCollection["EncryptedRoadCode"];
                //roadName = mappedCollection["hdnERRoadCode"];
                if (objBAL.MapHabitationToRoadRCPLWE(encryptedHabCodes, roadName))
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
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Error occurred while processing the request.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion


        #region MapDRRP RCPLWE
        /// <summary>
        /// returns the view for mapping the other candidate road
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MapOtherCandidateRoadViewRCPLWE(String parameter, String hash, String key)
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                CoreNetworkDAL objDAL = new CoreNetworkDAL();
                CandidateRoadViewModel model = new CandidateRoadViewModel();
                if (!(String.IsNullOrEmpty(parameter)) && !(String.IsNullOrEmpty(hash)) && !(String.IsNullOrEmpty(key)))
                {
                    Dictionary<string, string> decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                    if (decryptedParameters != null)
                    {
                        model.CNCode = Convert.ToInt32(decryptedParameters["NetworkCode"]);
                    }
                }
                model.LockStatus = objDAL.GetLockStatusOfCandidateRoad(model.CNCode);
                //model.lstBlocks = objCommon.PopulateBlocks(PMGSYSession.Current.DistrictCode);
                model.lstBlocks = new SelectList(objDAL.GetBlocksByDistrictCodeRCPLWE(PMGSYSession.Current.DistrictCode), "MAST_BLOCK_CODE", "MAST_BLOCK_NAME").ToList();
                model.lstBlocks.Insert(0, new SelectListItem { Value = "0", Text = "Select Block" });
                //model.lstRoadCategory = new SelectList(objDAL.GetAllRoadCategories().Where(m => m.MAST_ROAD_SHORT_DESC == "RR(ODR)" || m.MAST_ROAD_SHORT_DESC == "RR(VR)" || m.MAST_ROAD_SHORT_DESC == "RR(TRACK)").ToList(), "MAST_ROAD_CAT_CODE", "MAST_ROAD_CAT_NAME").ToList();
                model.lstRoadCategory = new SelectList(objDAL.GetAllRoadCategories(), "MAST_ROAD_CAT_CODE", "MAST_ROAD_CAT_NAME").ToList();
                model.lstRoadCategory.Find(x => x.Text == "--Select Category--").Value = "-1";
                model.RoadCatCode = -1;
                model.lstDRRP.Insert(0, new SelectListItem { Value = "0", Text = "- Select Road Name -" });
                return PartialView(model);
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }
        #endregion MapDRRP RCPLWE


        #region View RCPLWE Details
        /// <summary>
        /// returns the Details view of Core Network
        /// </summary>
        /// <param name="parameter">Encrypted id</param>
        /// <param name="hash">Encrypted id</param>
        /// <param name="key">Encrypted id</param>
        /// <returns>Details view of Core Network</returns>
        [Audit]
        public ActionResult DetailsCoreNetworkRCPLWE(String parameter, String hash, String key)
        {
            Dictionary<string, string> parameters = null;
            CoreNetworkViewModel model = new CoreNetworkViewModel();
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            try
            {
                parameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (parameters != null)
                {
                    model = objBAL.GetCoreNetworkDetails(Convert.ToInt32(parameters["NetworkCode"]));
                    model.ROAD_CODE = db.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == model.MAST_ER_ROAD_CODE).Select(r => r.MAST_ER_ROAD_NUMBER).FirstOrDefault();
                    return PartialView("DetailsCoreNetworkRCPLWE", model);
                }
                else
                {
                    return PartialView("DetailsCoreNetworkRCPLWE", new CoreNetworkViewModel());
                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return Json(new { success = false, message = "Error occurred while processing the request." });
                //return PartialView("DetailsCoreNetwork", new CoreNetworkViewModel());
            }
            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// finalize the core network details RCPLWE
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult FinalizeCoreNetworkRCPLWE()
        {
            try
            {
                int PLAN_CN_ROAD_CODE = Convert.ToInt32(Request.Params["PLAN_CN_ROAD_CODE"]);
                string status = objBAL.FinalizeCoreNetworkRCPLWEBAL(PLAN_CN_ROAD_CODE);
                if (status == string.Empty)
                    return Json(new { Success = true });
                else
                    return Json(new { Success = false, ErrorMessage = status });
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { Success = false, message = "Error occurred while processing the request." });
            }
        }
        #endregion View RCPLWE Details Ends

        #endregion RCPLWE Ends

        #region PMGSY3
        ///CN
        [HttpGet]
        public ActionResult ListCoreNetWorksPMGSY3()
        {
            List<MASTER_BLOCK> lst = new List<MASTER_BLOCK>();
            int block = 0;
            CommonFunctions objCommon = new CommonFunctions();
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            CNPMGSY3ViewModel model = new CNPMGSY3ViewModel();
            try
            {

                model.DistrictCode = PMGSYSession.Current.DistrictCode;

                model.PmgsyScheme = PMGSYSession.Current.PMGSYScheme;
                model.lstRouteType = objDAL.GetAllRoutes().ToList();

                model.lstRouteType.Remove(model.lstRouteType.Find(c => c.Value.Trim() == "L"));

                model.lstCategory = objDAL.GetCategoryForSearch().ToList();
                model.lstCategory.Insert(0, new SelectListItem() { Value = "0", Text = "-- All --" });

                List<SelectListItem> lstDefault = new List<SelectListItem>();
                lstDefault.Insert(0, new SelectListItem { Value = "0", Text = "All" });
                if (PMGSYSession.Current.RoleCode == 25 || PMGSY.Extensions.PMGSYSession.Current.RoleCode == 53 || PMGSYSession.Current.RoleCode == 65)//Changes by SAMMED A. PATIL for mordviewuser
                {
                    model.lstStateCode = objCommon.PopulateStates(true);
                    model.lstDistrictCode = lstDefault;
                    model.lstBlockCode = lstDefault;
                }
                else if (PMGSYSession.Current.RoleCode == 2)
                {
                    model.lstDistrictCode = objCommon.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                    model.lstBlockCode = lstDefault;
                }
                else
                {
                    model.lstBlockCode = new SelectList(objDAL.GetBlocksByDistrictCode(model.DistrictCode), "MAST_BLOCK_CODE", "MAST_BLOCK_NAME").ToList();
                    lst = objDAL.GetBlocksByDistrictCode(model.DistrictCode);

                    if (lst.Count > 0)
                    {
                        PMGSY.DAL.ExistingRoads.IExistingRoadsDAL objDRRPDAL = new PMGSY.DAL.ExistingRoads.ExistingRoadsDAL();
                        ViewData["isUnlocked"] = objDRRPDAL.CheckUnlockedDAL(lst.Select(x => x.MAST_BLOCK_CODE).First());
                    }
                }
                ViewData["IsLocked"] = objDAL.checkIsLockedPMGSY3(lst.Select(x => x.MAST_BLOCK_CODE).First());


                ViewData["IsTraceFileFinalized"] = objDAL.CheckLockofTraceMapDAL(lst.Select(x => x.MAST_BLOCK_CODE).First());


                model.isPMGSY3Finalized = objDAL.CheckPMGSY3FinalizedDAL(model.DistrictCode);
                //model.isPMGSY3Finalized = objDAL.CheckPMGSY3FinalizedDAL(lst.Select(x => x.MAST_BLOCK_CODE).First());
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.ListCoreNetWorksPMGSY3");
                return View();
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult CheckLockStatus()
        {
            try
            {
                int blockCode = Convert.ToInt32(Request.Params["blockCode"]);
                if (blockCode > 0)
                {
                    CoreNetworkDAL objDAL = new CoreNetworkDAL();

                    bool flag = objDAL.checkIsLockedPMGSY3(blockCode);

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
                ErrorLog.LogError(ex, "CoreNetwork.CheckUnlocked()");
                return null;
            }
        }


        [HttpPost]
        [Audit]
        public ActionResult CheckTraceMapIsFinalizedOrNot()
        {
            try
            {
                int blockCode = Convert.ToInt32(Request.Params["blockCode"]);
                if (blockCode > 0)
                {
                    CoreNetworkDAL objDAL = new CoreNetworkDAL();

                    bool flag = objDAL.CheckLockofTraceMapDAL(blockCode);

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
                ErrorLog.LogError(ex, "CoreNetwork.CheckTraceMapIsFinalizedOrNot()");
                return null;
            }
        }

        [HttpGet]
        public ActionResult DetailsCoreNetworkPMGSY3(String parameter, String hash, String key)
        {
            Dictionary<string, string> parameters = null;
            CoreNetworkViewModel model = new CoreNetworkViewModel();
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            try
            {
                parameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (parameters != null)
                {
                    model = objBAL.GetCoreNetworkDetails(Convert.ToInt32(parameters["NetworkCode"]));
                    model.ROAD_CODE = db.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == model.MAST_ER_ROAD_CODE).Select(r => r.MAST_ER_ROAD_NUMBER).FirstOrDefault();
                    return PartialView("DetailsCoreNetworkPMGSY3", model);
                }
                else
                {
                    return PartialView("DetailsCoreNetworkPMGSY3", new CoreNetworkViewModel());
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ListCoreNetWorksPMGSY3()");
                return Json(new { success = false, message = "Error occurred while processing the request." });
            }
            finally
            {
                db.Dispose();
            }
        }

        [HttpGet]
        [Audit]
        public ActionResult GetCoreNetWorksListPMGSY3(int? page, int? rows, string sidx, string sord)
        {
            int blockCode = 0;
            int districtCode = 0;
            int stateCode = 0;
            int categoryCode = 0;
            long totalRecords = 0;
            string roadType = string.Empty;
            String searchParameters = String.Empty;
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string roadName = string.Empty;
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            int CNCode = 0;
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

                if (!string.IsNullOrEmpty(Request.Params["blockCode"]))
                {
                    blockCode = Convert.ToInt32(Request.Params["blockCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["categoryCode"]))
                {
                    categoryCode = Convert.ToInt32(Request.Params["categoryCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["roadType"]))
                {
                    roadType = Request.Params["roadType"];
                }

                if (!string.IsNullOrEmpty(Request.Params["PLAN_RD_NAME"]))
                {
                    roadName = Request.Params["PLAN_RD_NAME"];
                }

                if (!string.IsNullOrEmpty(Request.Params["CNCode"]))
                {
                    CNCode = Convert.ToInt32(Request.Params["CNCode"]);
                }

                var jsonData = new
                {
                    rows = objDAL.GetCoreNetWorksListPMGSY3DAL(stateCode, districtCode, blockCode, roadType, categoryCode, roadName, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, CNCode),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.GetCoreNetWorksListPMGSY3()");
                return null;
            }
        }

        [Audit]
        public ActionResult CoreNetworksPMGSY3Layout(/*int blockCode*/ string id)
        {
            CoreNetworkViewModelPMGSY3 model = new CoreNetworkViewModelPMGSY3();
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            bool flag = false;
            int stateCode = 0, districtCode = 0, blockCode = 0;

            string[] locationcodes = id.Split('$');
            try
            {
                stateCode = locationcodes[0] == "undefined" ? 0 : Convert.ToInt32(locationcodes[0]);
                districtCode = locationcodes[1] == "undefined" ? 0 : Convert.ToInt32(locationcodes[1]);

                model.MAST_STATE_CODE = stateCode > 0 ? stateCode : PMGSYSession.Current.StateCode;
                model.MAST_DISTRICT_CODE = stateCode > 0 ? districtCode : PMGSYSession.Current.DistrictCode;
                blockCode = Convert.ToInt32(locationcodes[2]);
                model.BLOCK_NAME = objDAL.GetBlockName(blockCode);
                model.MAST_BLOCK_CODE = blockCode;
                List<SelectListItem> lstRoadNumber = new List<SelectListItem>();
                lstRoadNumber.Add(new SelectListItem { Value = "A", Text = "--Select Road--", Selected = true });
                //ViewData["RoadFrom"] = model.Road;
                model.lstRoadFrom = new SelectList(lstRoadNumber, "Value", "Text").ToList();
                model.lstRoadTo = new SelectList(lstRoadNumber, "Value", "Text").ToList();

                model.lstRoadNumFrom = model.RoadNumFromList.ToList();
                model.lstRoadNumTo = model.RoadNumToList.ToList();

                model.lstCategory = model.RoadCategory.ToList();
                model.lstCategory.Remove(model.lstCategory.Find(x => x.Text.Equals("X")));
                model.lstCategory.Remove(model.lstCategory.Find(x => x.Text.Equals("Major District Road")));
                model.lstCategory.Remove(model.lstCategory.Find(x => x.Text.Equals("National Highway")));
                model.lstCategory.Remove(model.lstCategory.Find(x => x.Text.Equals("State Highway")));

                model.lstPreviousBlock = new SelectList(objDAL.GetBlocksByDistrictCode(PMGSYSession.Current.DistrictCode, blockCode), "MAST_BLOCK_CODE", "MAST_BLOCK_NAME").ToList();
                model.lstPreviousBlockRoadNo = new SelectList(lstRoadNumber, "Value", "Text").ToList();
                model.lstRoadNumber = new SelectList(lstRoadNumber, "Value", "Text").ToList();
                model.lstHabitationFrom = new SelectList(objDAL.GetHabitationsByBlockCode(blockCode), "Value", "Text").ToList();
                model.lstHabitationTo = new SelectList(objDAL.GetHabitationsByBlockCode(blockCode), "Value", "Text").ToList();
                model.lstNextBlock = new SelectList(objDAL.GetBlocksByDistrictCode(PMGSYSession.Current.DistrictCode, blockCode), "MAST_BLOCK_CODE", "MAST_BLOCK_NAME").ToList();
                model.lstNextBlockRoadNo = new SelectList(lstRoadNumber, "Value", "Text").ToList();


                List<SelectListItem> lstRoute = new List<SelectListItem>();
                lstRoute.Add(new SelectListItem { Value = "0", Text = "--Select Route--", Selected = true });
                lstRoute.Add(new SelectListItem { Value = "T", Text = "Through Route" });

                ///Changes by SAMMED A. PATIL on 30AUG2017 to allow Link Route in PMGSY2
                //lstRoute.Add(new SelectListItem { Value = "L", Text = "Link Route" });
                lstRoute.Add(new SelectListItem { Value = "M", Text = "Major Rural Link" });

                //return new SelectList(lstRoute,"Value","Text");
                //if (PMGSYSession.Current.PMGSYScheme == 1)
                //{
                //    flag = checkSchedule5(blockCode);
                //    if (flag)
                //    {
                //        lstRoute.Add(new SelectListItem { Value = "N", Text = "Missing Link" });
                //    }
                //}
                model.RouteType = lstRoute;

                model.isPMGSY3Finalized = objDAL.CheckPMGSY3FinalizedTRMRLDAL(blockCode);

                return PartialView(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.CoreNetworksPMGSY3Layout()");
                return null;
            }
        }

        [HttpPost]
        [Audit]
        public JsonResult GetRoadNameByRoadCodePMGSY3(string roadName, string blockName)
        {
            try
            {
                int roadCode = Convert.ToInt32(roadName);
                //int blockCode = PMGSYSession.Current.blockCode;
                int blockCode = Convert.ToInt32(blockName);
                CoreNetworkDAL objDAL = new CoreNetworkDAL();
                List<SelectListItem> lstRoads = objDAL.GetRoadNamesByRoadCodePMGSY3DAL(roadCode, blockCode);
                lstRoads.Insert(0, new SelectListItem { Value = "0", Text = "-Select Road Name-" });
                return Json(lstRoads, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.GetRoadNameByRoadCodePMGSY3()");
                return null;
            }

        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Audit]
        public ActionResult AddCoreNetworksPMGSY3(CoreNetworkViewModelPMGSY3 model)
        {
            CoreNetworkDAL objDAL = new CoreNetworkDAL();

            bool status = false;
            try
            {
                if (ModelState.IsValid)
                {
                    model.MAST_DISTRICT_CODE = model.MAST_DISTRICT_CODE > 0 ? model.MAST_DISTRICT_CODE : PMGSYSession.Current.DistrictCode;
                    model.MAST_STATE_CODE = model.MAST_STATE_CODE > 0 ? model.MAST_STATE_CODE : PMGSYSession.Current.StateCode;

                    if (objDAL.AddCoreNetworksPMGSY3DAL(model, ref message))
                    {
                        message = message == string.Empty ? "TR/MRL road details added successfully." : message;

                        status = true;
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        message = message == string.Empty ? "Error occurred while processing the request." : message;
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.AddCoreNetworksPMGSY3()");
                return Json(new { success = false, message = "Error occurred while processing the request." });
            }
        }

        [HttpGet]

        public ActionResult EditCoreNetworksPMGSY3(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptParameters = null;
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            List<SelectListItem> lstRoadNumber = new List<SelectListItem>();
            bool flag = false;
            try
            {
                decryptParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptParameters.Count() > 0)
                {
                    CoreNetworkViewModelPMGSY3 model = objDAL.GetCoreNetworkDetailsPMGSY3(Convert.ToInt32(decryptParameters["NetworkCode"]));

                    if (decryptParameters.Count > 1)
                    {
                        model.LockUnlockFlag = decryptParameters["UnlockFlag"];
                    }

                    List<SelectListItem> lstRoute = new List<SelectListItem>();
                    lstRoute.Add(new SelectListItem { Value = "0", Text = "--Select Route--", Selected = true });
                    lstRoute.Add(new SelectListItem { Value = "T", Text = "Through Route" });

                    ///Changes by SAMMED A. PATIL on 30AUG2017 to allow Link Route in PMGSY2
                    ///changes made by abhinav pathak on 31oct2019
                    if (!(PMGSYSession.Current.PMGSYScheme == 4))
                        lstRoute.Add(new SelectListItem { Value = "L", Text = "Link Route" });

                    lstRoute.Add(new SelectListItem { Value = "M", Text = "Major Rural Link" });

                    model.RouteType = lstRoute;

                    model.FROM_TYPE = model.PLAN_RD_FROM_TYPE;
                    model.TO_TYPE = model.PLAN_RD_TO_TYPE;
                    model.RD_FROM = model.PLAN_RD_FROM;
                    model.RD_TO = model.PLAN_RD_TO;
                    model.ROAD_CODE = db.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == model.MAST_ER_ROAD_CODE).Select(r => r.MAST_ER_ROAD_NUMBER).FirstOrDefault();
                    model.BLOCK_NAME = objDAL.GetBlockName(model.MAST_BLOCK_CODE);//db.MASTER_BLOCK.Where(m => m.MAST_BLOCK_CODE == model.MAST_BLOCK_CODE).Select(b => b.MAST_BLOCK_NAME).FirstOrDefault();
                    if (model.PLAN_RD_NUM_FROM == null)
                    {
                        model.NUM_FROM = null;
                    }
                    else
                    {
                        model.NUM_FROM = model.PLAN_RD_NUM_FROM;
                    }

                    if (model.PLAN_RD_NUM_TO == null)
                    {
                        model.NUM_TO = null;
                    }
                    else
                    {
                        model.NUM_TO = model.PLAN_RD_NUM_TO;
                    }

                    if (model == null)
                    {
                        ModelState.AddModelError(String.Empty, "Core network details not exist.");
                        return PartialView("CoreNetworksPmgsy3Layout", new CoreNetworkViewModelPMGSY3());
                    }

                    model.lstRoadFrom = model.Road.ToList();
                    model.lstRoadTo = model.RoadTo.ToList();

                    switch (model.PLAN_RD_FROM_TYPE)
                    {
                        case "T":
                            model.lstRoadNumFrom = new SelectList(objDAL.GetRoadNumFromThroughEditList(model.PLAN_RD_FROM_TYPE, model.MAST_BLOCK_CODE, model.PLAN_CN_ROAD_CODE), "Value", "Text", model.PLAN_RD_NUM_FROM).ToList();
                            break;
                        case "L":
                            model.lstRoadNumFrom = new SelectList(objDAL.GetRoadNumFromThroughEditList(model.PLAN_RD_FROM_TYPE, model.MAST_BLOCK_CODE, model.PLAN_CN_ROAD_CODE), "Value", "Text", model.PLAN_RD_NUM_FROM).ToList();
                            break;
                        case "M":
                            model.lstRoadNumFrom = new SelectList(objDAL.GetRoadNumFromThroughEditList(model.PLAN_RD_FROM_TYPE, model.MAST_BLOCK_CODE, model.PLAN_CN_ROAD_CODE), "Value", "Text", model.PLAN_RD_NUM_FROM).ToList();
                            break;
                        case "H":
                            model.lstRoadNumFrom = model.RoadNumFromList.ToList();
                            break;
                        case "B":
                            model.lstRoadNumFrom = model.RoadNumFromList.ToList();
                            break;
                        default:
                            model.lstRoadNumFrom = new SelectList(objDAL.GetRoadNumFromByRoadFrom(model.PLAN_RD_FROM_TYPE, model.MAST_BLOCK_CODE), "MAST_ER_ROAD_CODE", "MAST_ER_ROAD_NAME", model.PLAN_RD_NUM_FROM).ToList();
                            break;
                    }

                    switch (model.PLAN_RD_TO_TYPE)
                    {
                        case "T":
                            model.lstRoadNumTo = new SelectList(objDAL.GetRoadNumFromThroughList(model.PLAN_RD_TO_TYPE, model.MAST_BLOCK_CODE), "PLAN_CN_ROAD_CODE", "PLAN_CN_ROAD_NUMBER", model.PLAN_RD_NUM_TO).ToList();
                            break;
                        case "L":
                            model.lstRoadNumTo = new SelectList(objDAL.GetRoadNumFromThroughList(model.PLAN_RD_TO_TYPE, model.MAST_BLOCK_CODE), "PLAN_CN_ROAD_CODE", "PLAN_CN_ROAD_NUMBER", model.PLAN_RD_NUM_TO).ToList();
                            break;
                        case "M":
                            model.lstRoadNumTo = new SelectList(objDAL.GetRoadNumFromThroughList(model.PLAN_RD_TO_TYPE, model.MAST_BLOCK_CODE), "PLAN_CN_ROAD_CODE", "PLAN_CN_ROAD_NUMBER", model.PLAN_RD_NUM_TO).ToList();
                            break;
                        case "H":
                            model.lstRoadNumTo = model.RoadNumToList.ToList();
                            break;
                        case "B":
                            model.lstRoadNumTo = model.RoadNumToList.ToList();
                            break;
                        default:
                            model.lstRoadNumTo = new SelectList(objDAL.GetRoadNumFromByRoadFrom(model.PLAN_RD_TO_TYPE, model.MAST_BLOCK_CODE), "MAST_ER_ROAD_CODE", "MAST_ER_ROAD_NAME", model.PLAN_RD_NUM_TO).ToList();
                            break;
                    }
                    if (model.PLAN_RD_ROUTE == "N")
                    {
                        List<string> lst = objDAL.MLRoadList(model.MAST_BLOCK_CODE);
                        List<SelectListItem> lstRoadNumberML = new List<SelectListItem>();
                        lstRoadNumberML.Insert(0, new SelectListItem { Value = "A", Text = "--Select Road No--" });
                        for (int i = 1; i <= 30; i++)
                        {
                            if (i < 10)
                            {
                                lstRoadNumberML.Add(new SelectListItem { Value = "ML0" + i.ToString(), Text = "ML0" + i.ToString() });
                            }
                            else
                            {
                                lstRoadNumberML.Add(new SelectListItem { Value = "ML" + i.ToString(), Text = "ML" + i.ToString() });
                            }
                        }

                        var list = (from item in lstRoadNumberML
                                    where !lst.Contains(item.Value)
                                    select new
                                    {
                                        item.Value,
                                        item.Text
                                    }).Distinct().ToList().Select(x => new SelectListItem
                                    {
                                        Text = x.Text,
                                        Value = x.Value
                                    }).ToList();

                        list.Add(new SelectListItem { Value = model.PLAN_CN_ROAD_NUMBER, Text = model.PLAN_CN_ROAD_NUMBER });
                        //list.Insert(list.Count, SelectListItem { Value = model.PLAN_CN_ROAD_NUMBER, Text = model.PLAN_CN_ROAD_NUMBER });
                        //new SelectList(list, "Value", "Text")

                        model.lstRoadNumber = list;
                    }
                    else
                    {
                        JsonResult a = GetRoadNumberByRoadRoute(model.PLAN_RD_ROUTE, model.MAST_BLOCK_CODE, model.PLAN_CN_ROAD_CODE);
                        ViewData["RoadNumber"] = a.Data;
                    }
                    model.lstRoadNumThroughFrom = model.RoadFrom.ToList();
                    model.lstRoadNumThroughTo = model.RoadFrom.ToList();

                    ViewBag.RoadCategory = objDAL.GetRoadCategory(model.MAST_ER_ROAD_CODE);

                    model.lstPreviousBlock = new SelectList(objDAL.GetBlocksByDistrictCode(PMGSYSession.Current.DistrictCode), "MAST_BLOCK_CODE", "MAST_BLOCK_NAME").ToList();
                    model.lstPreviousBlockRoadNo = new SelectList(objDAL.GetPreviousBlockByBlockCode(model.MAST_BLOCK_CODE), "PLAN_CN_ROAD_CODE", "PLAN_CN_ROAD_NUMBER").ToList();
                    //ViewData["RoadNumber"] = new SelectList(lstRoadNumber, "Value", "Text");
                    model.lstHabitationFrom = new SelectList(objDAL.GetHabitationsByBlockCode(model.MAST_BLOCK_CODE), "Value", "Text").ToList();
                    model.lstHabitationTo = new SelectList(objDAL.GetHabitationsByBlockCode(model.MAST_BLOCK_CODE), "Value", "Text").ToList();
                    model.lstNextBlock = new SelectList(objDAL.GetBlocksByDistrictCode(PMGSYSession.Current.DistrictCode), "MAST_BLOCK_CODE", "MAST_BLOCK_NAME").ToList();
                    model.lstNextBlockRoadNo = new SelectList(objDAL.GetPreviousBlockByBlockCode(model.MAST_BLOCK_CODE), "PLAN_CN_ROAD_CODE", "PLAN_CN_ROAD_NUMBER").ToList();

                    model.isPMGSY3Finalized = objDAL.CheckPMGSY3FinalizedTRMRLDAL(model.MAST_BLOCK_CODE);

                    return PartialView("CoreNetworksPmgsy3Layout", model);
                }
                return PartialView("CoreNetworksPmgsy3Layout", new CoreNetworkViewModelPMGSY3());
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.EditCoreNetworksPMGSY3()");
                return Json(new { success = false, message = "Error occurred while processing your request" });
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Audit]
        public ActionResult EditCoreNetworksPMGSY3(CoreNetworkViewModelPMGSY3 model)
        {
            bool status = false;
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            try
            {
                if (ModelState.IsValid)
                {
                    if (objDAL.EditCoreNetworksPMGSY3DAL(model, ref message))
                    {
                        message = message == string.Empty ? "TR/MRL road details updated successfully." : message;

                        status = true;
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        message = message == string.Empty ? "Error occurred while processing your request." : message;
                        ModelState.AddModelError("", message);
                        return Json(new { success = status, message = new CommonFunctions().FormatErrorMessage(ModelState) }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.EditCoreNetworksPMGSY3()");
                message = message == string.Empty ? "Error occurred while processing your request." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// deletes the particular core network details
        /// </summary>
        /// <param name="parameter">Encrypted id</param>
        /// <param name="hash">Encrypted id</param>
        /// <param name="key">Encrypted id</param>
        /// <returns>status of delete operation </returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Audit]
        public ActionResult DeleteCoreNetworkPMGSY3(String parameter, String hash, String key)
        {
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptedParameters.Count() > 0)
                {
                    if (!objDAL.DeleteCoreNetworksPMGSY3DAL(Convert.ToInt32(decryptedParameters["NetworkCode"].ToString())))
                    {
                        ModelState.AddModelError(String.Empty, "TR/MRL road details are in use and can not be deleted.");
                        message = "TR/MRL road details are in use and can not be deleted.";

                        return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                message = "TR/MRL road details deleted successfully.";

                return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.DeleteCoreNetworkPMGSY3");

                return Json(new { success = false, message = "TR/MRL road details can not be deleted" }, JsonRequestBehavior.AllowGet);
            }
        }



        //Inter District habitation mapping
        #region Inter District habitation Mapping

        [HttpPost]
        public ActionResult PopulateRoadsbyBlockCode(int blockCode, int districtCode)
        {
            try
            {
                CoreNetworkDAL objDAL = new CoreNetworkDAL();
                CommonFunctions objCommonFunctions = new CommonFunctions();

                List<SelectListItem> listRoads = new List<SelectListItem>();
                listRoads = objDAL.GetRoadsByBlockCode(blockCode, districtCode);

                return Json(listRoads);
            }
            catch
            {
                return Json(new { string.Empty });
            }
        }







        [HttpPost]
        public ActionResult PopulateBlocksbyDistrictCode(int districtCode)
        {
            try
            {
                CommonFunctions objCommonFunctions = new CommonFunctions();
                //int stateCode = Convert.ToInt32(Request.Params["stateCode"]);
                List<SelectListItem> lstBlocks = new List<SelectListItem>();
                lstBlocks = objCommonFunctions.PopulateBlocks(districtCode, false);
                lstBlocks.RemoveAt(0);
                lstBlocks.Insert(0, new SelectListItem { Value = "0", Text = "--Select Block--" });
                return Json(lstBlocks);
            }
            catch
            {
                return Json(new { string.Empty });
            }
        }




        [HttpGet]
        [Audit]
        public ActionResult ListHabitationsInterDistrictHabitationPMGSY3(String parameter, String hash, String key)
        {
            HabitationDetailsViewModelPMGSY3 model = new HabitationDetailsViewModelPMGSY3();
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            CommonFunctions objCommon = new CommonFunctions();
            Dictionary<string, string> decryptedParameters = null;
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                int networkCode = Convert.ToInt32(decryptedParameters["NetworkCode"]);
                model.UnlockFlag = decryptedParameters["UnlockFlag"];
                model.EncryptedRoadCode = networkCode.ToString();
                ViewBag.RoadNumber = db.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == networkCode).Select(m => m.PLAN_CN_ROAD_NUMBER).FirstOrDefault();


                int stateCodeValue = dbContext.PLAN_ROAD.Where(x => x.PLAN_CN_ROAD_CODE == networkCode).Select(x => x.MAST_STATE_CODE).FirstOrDefault();



                model.lstDistrict = new List<SelectListItem>();

                model.lstDistrict = objDAL.GetDistrictsByStateCodePMGSY3(stateCodeValue);
                model.lstDistrict.Insert(0, new SelectListItem() { Text = "--Select District--", Value = "-1" });

                model.lstBlock = new List<SelectListItem>();
                model.lstBlock.Insert(0, new SelectListItem() { Text = "Select Block", Value = "-1" });

                model.lstRoads = new List<SelectListItem>();
                model.lstRoads.Insert(0, new SelectListItem() { Text = "Select Road", Value = "-1" });

                return PartialView("ListHabitationsInterDistrictHabitationPMGSY3", model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.ListHabitationsPMGSY3()");
                return Json(new { success = false, message = "Error occurred while processing the request" });
            }
        }


        [HttpGet]
        [Audit]
        public ActionResult GetHabitationListToMapInterDistrictHabitationPMGSY3(int? page, int? rows, string sidx, string sord)
        {
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            try
            {
                int roadCode = 0, erRoadCode = 0;
                string habDirect = string.Empty;
                long totalRecords = 0;


                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["habCode"]))
                {
                    roadCode = Convert.ToInt32(Request.Params["habCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["erRoadCode"]))
                {
                    erRoadCode = Convert.ToInt32(Request.Params["erRoadCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["habDirect"]))
                {
                    habDirect = Request.Params["habDirect"];
                }

                var jsonData = new
                {
                    rows = objDAL.GetHabitationListToMapPMGSY3DAL_InterDistricthabitation(roadCode, habDirect, erRoadCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,

                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.GetHabitationListToMapPMGSY3()");
                return null;
            }
        }




        #endregion








        ///Habitation Mapping
        [HttpGet]
        [Audit]
        public ActionResult ListHabitationsPMGSY3(String parameter, String hash, String key)
        {
            HabitationDetailsViewModelPMGSY3 model = new HabitationDetailsViewModelPMGSY3();
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            CommonFunctions objCommon = new CommonFunctions();
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                int networkCode = Convert.ToInt32(decryptedParameters["NetworkCode"]);
                model.UnlockFlag = decryptedParameters["UnlockFlag"];
                model.EncryptedRoadCode = networkCode.ToString();
                ViewBag.RoadNumber = db.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == networkCode).Select(m => m.PLAN_CN_ROAD_NUMBER).FirstOrDefault();

                //ViewData["Habitations"] = objDAL.GetHabitationCodeList(networkCode);

                model.lstRoads = objDAL.GetRoadsByCNCodePMGSY3(networkCode);

                return PartialView("ListHabitationsPMGSY3", model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.ListHabitationsPMGSY3()");
                return Json(new { success = false, message = "Error occurred while processing the request" });
            }
        }

        [HttpGet]
        [Audit]
        public ActionResult GetHabitationListToMapPMGSY3(int? page, int? rows, string sidx, string sord)
        {
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            try
            {
                int roadCode = 0, erRoadCode = 0;
                string habDirect = string.Empty;
                long totalRecords = 0;
                bool isHabFinalized = false;

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["habCode"]))
                {
                    roadCode = Convert.ToInt32(Request.Params["habCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["erRoadCode"]))
                {
                    erRoadCode = Convert.ToInt32(Request.Params["erRoadCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["habDirect"]))
                {
                    habDirect = Request.Params["habDirect"];
                }

                var jsonData = new
                {
                    rows = objDAL.GetHabitationListToMapPMGSY3DAL(roadCode, habDirect, erRoadCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, out isHabFinalized),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                    isHabFinalized = isHabFinalized
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.GetHabitationListToMapPMGSY3()");
                return null;
            }
        }

        [HttpGet]
        [Audit]
        public ActionResult GetHabitationListPMGSY3(int? page, int? rows, string sidx, string sord)
        {
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            int habCode = 0;

            long totalRecords = 0;
            string flag = string.Empty;
            bool isHabFinalized = false;
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["habCode"]))
                {
                    habCode = Convert.ToInt32(Request.Params["habCode"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["Flag"]))
                {
                    flag = Request.Params["Flag"];
                }

                var jsonData = new
                {
                    rows = objDAL.GetHabitationListPMGSY3DAL(habCode, flag, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, out isHabFinalized),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                    isHabFinalized = isHabFinalized
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.GetHabitationListPMGSY3()");
                return null;
            }
        }

        /// <summary>
        /// add habitation to core network
        /// </summary>
        /// <param name="habModel">contains the details of Habitation</param>
        /// <returns>response message along with status</returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Audit]
        public ActionResult MapHabitationsToNetworkPMGSY3(FormCollection mappedCollection)
        {
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
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
                    message = "Habitations added successfully.";
                    status = true;
                }
                else
                {
                    status = false;
                    message = "Habitations not added.";
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.MapHabitationsToNetworkPMGSY3()");
                message = "Error occurred while processing the request.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// deletes the Habitation details
        /// </summary>
        /// <param name="hash">Encrypted id</param>
        /// <param name="parameter">Encrypted id</param>
        /// <param name="key">Encrypted id</param>
        /// <param name="roadCode">road id</param>
        /// <returns>response message along with status</returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Audit]
        public ActionResult DeleteMapHabitationPMGSY3(String hash, String parameter, String key, string roadCode)
        {
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            Dictionary<string, string> decryptedParameters = null;
            int coreNetworkCode = Convert.ToInt32(roadCode);
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptedParameters.Count() > 0)
                {
                    if (!objDAL.DeleteMapHabitationPMGSY3DAL(Convert.ToInt32(decryptedParameters["HabCode"].ToString()), decryptedParameters["Flag"].ToString(), coreNetworkCode))
                    {
                        ModelState.AddModelError(String.Empty, "Habitation not deleted.");
                        return Json(new { success = false, message = "Habitation can not be deleted" }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { success = true, message = "Habitation deleted successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.DeleteMapHabitationPMGSY3()");
                return Json(new { success = false, message = "Habitation can not be deleted" }, JsonRequestBehavior.AllowGet);
            }
        }

        //Map DRRP to CN
        [HttpGet]
        public ActionResult MapOtherCandidateRoadViewPMGSY3(String parameter, String hash, String key)
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                CoreNetworkDAL objDAL = new CoreNetworkDAL();
                CandidateRoadViewModel model = new CandidateRoadViewModel();
                if (!(String.IsNullOrEmpty(parameter)) && !(String.IsNullOrEmpty(hash)) && !(String.IsNullOrEmpty(key)))
                {
                    Dictionary<string, string> decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                    if (decryptedParameters != null)
                    {
                        model.CNCode = Convert.ToInt32(decryptedParameters["NetworkCode"]);
                    }
                }
                model.LockStatus = objDAL.GetLockStatusOfCandidateRoad(model.CNCode);
                //model.lstBlocks = objCommon.PopulateBlocks(PMGSYSession.Current.DistrictCode);
                //model.lstBlocks = new SelectList(objDAL.GetBlocksByDistrictCode(PMGSYSession.Current.DistrictCode), "MAST_BLOCK_CODE", "MAST_BLOCK_NAME").ToList();
                model.lstBlocks = objDAL.GetDRRPBlocksByDistrictCode(PMGSYSession.Current.DistrictCode);
                model.lstBlocks.Insert(0, new SelectListItem { Value = "0", Text = "Select Block" });
                model.lstRoadCategory = new SelectList(objDAL.GetAllRoadCategories().Where(m => m.MAST_ROAD_SHORT_DESC == "RR(ODR)" || m.MAST_ROAD_SHORT_DESC == "RR(VR)" || m.MAST_ROAD_SHORT_DESC == "RR(TRACK)").ToList(), "MAST_ROAD_CAT_CODE", "MAST_ROAD_CAT_NAME").ToList();
                model.lstRoadCategory.Insert(0, new SelectListItem { Value = "0", Text = "- Select Road Category -" });
                model.lstDRRP.Insert(0, new SelectListItem { Value = "0", Text = "- Select Road Name -" });

                List<string> lst = objDAL.GetCNDetailsPMGSY3DAL(model.CNCode);
                if (lst != null)
                {
                    ViewBag.RoadName = lst[0].Trim();
                    ViewBag.TotLength = lst[1].Trim();
                    ViewBag.BalLength = lst[2].Trim();
                }

                return PartialView(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.MapOtherCandidateRoadViewPMGSY3()");
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        [HttpGet]
        public JsonResult GetMappedCandidateRoadListPMGSY3(int? page, int? rows, string sidx, string sord)
        {
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            long totalRecords = 0;
            int candidateRoadCode = 0;
            string IsFinalized = String.Empty;
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["RoadCode"]))
                {
                    candidateRoadCode = Convert.ToInt32(Request.Params["RoadCode"]);
                }

                var jsonData = new
                {
                    rows = objDAL.ListCandidateRoadsPMGSY3DAL(candidateRoadCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, out IsFinalized),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                    IsFinalized = IsFinalized
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.GetMappedCandidateRoadList()");
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MapCandidateRoadPMGSY3(CandidateRoadViewModel model)
        {
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            try
            {
                if (ModelState.IsValid)
                {
                    if (objDAL.MapCandidateRoadPMGSY3DAL(model, ref message))
                    {
                        message = message == string.Empty ? "DRRP details mapped successfully." : message;
                        return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        message = message == string.Empty ? "Error occurred while processing your request." : message;
                        ModelState.AddModelError("", message);
                        return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.MapCandidateRoadPMGSY3()");
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteMappedDRRPDetailsPMGSY3(String parameter, String hash, String key)
        {
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            int DRRPCode = 0;
            int CNCode = 0;
            try
            {
                if (!(String.IsNullOrEmpty(parameter)) && !(String.IsNullOrEmpty(hash)) && !(String.IsNullOrEmpty(key)))
                {
                    Dictionary<string, string> decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                    if (decryptedParameters != null)
                    {
                        DRRPCode = Convert.ToInt32(decryptedParameters["DRRPCode"]);
                        CNCode = Convert.ToInt32(decryptedParameters["CNCode"]);
                    }
                }

                if (!(objDAL.DeleteMappedDRRPDetailsPMGSY3DAL(DRRPCode, CNCode)))
                {
                    return Json(new { success = false });
                }
                else
                {
                    return Json(new { success = true });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.DeleteMappedDRRPDetailsPMGSY3()");
                return Json(new { success = false });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FinalizeMappedDRRPDetailsPMGSY3(String id)
        {
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            int CNCode = 0;
            string message = string.Empty;
            try
            {
                if (!String.IsNullOrEmpty(id))
                {
                    CNCode = Convert.ToInt32(id);
                    if (objDAL.FinalizeMappedDRRPDetailsPMGSY3DAL(CNCode, ref message))
                    {
                        return Json(new { success = true, message = message });
                    }
                    else
                    {
                        return Json(new { success = false, message = message });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "TR/MRL Road details are not present." });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.FinalizeMappedDRRPDetailsPMGSY3()");
                return Json(new { success = false, message = "Error on DRRP details finalize." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeFinalizeMappedDRRPDetailsPMGSY3(String id)
        {
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            int CNCode = 0;
            try
            {
                if (!String.IsNullOrEmpty(id))
                {
                    CNCode = Convert.ToInt32(id);
                    if (objDAL.DeFinalizeMappedDRRPDetailsPMGSY3DAL(CNCode))
                    {
                        return Json(new { success = true });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Error occurred while processing your request." });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "TR/MRL Road details are not present." });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.DeFinalizeMappedDRRPDetailsPMGSY3");
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        [HttpGet]
        [Audit]
        public ActionResult GetCoreNetworkChecksPMGSY3()
        {
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            try
            {
                int PLAN_CN_ROAD_CODE = Convert.ToInt32(Request.Params["PLAN_CN_ROAD_CODE"]);
                string status = objDAL.GetCoreNetworkChecksPMGSY3DAL(PLAN_CN_ROAD_CODE);
                if (status == string.Empty)
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { Success = false, ErrorMessage = status }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.GetCoreNetworkChecksPMGSY3()");
                return Json(new { Success = false, message = "Error occurred while processing the request." }, JsonRequestBehavior.AllowGet);
            }
        }

        #region added by abhinav
        [HttpPost]
        public ActionResult CheckBlockFinalization()
        {
            try
            {
                var blockcode = Convert.ToInt32(Request.Params["BlockCode"]);
                var Blckname = Request.Params["BlockName"];
                PMGSYEntities context = new PMGSYEntities();
                bool isBlockFinalized = false;

                isBlockFinalized = context.MAST_CN_BLOCK_PMGSY3_FINALIZE.Where(x => x.MAST_BLOCK_CODE == blockcode).Any();

                if (isBlockFinalized)
                {
                    return Json(new { status = isBlockFinalized, ErrorMsg = Blckname + " Block is already Finalized." }, JsonRequestBehavior.DenyGet);
                }
                else
                {
                    return Json(new { status = isBlockFinalized, ErrorMsg = "" }, JsonRequestBehavior.DenyGet);
                }


            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork/CheckBlockFinalization");
                return null;
            }
        }

        #endregion

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FinalizeCNHabitationDetailsPMGSY3(string id)
        {
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            int CNCode = 0;
            try
            {
                if (!(String.IsNullOrEmpty(id)))
                {
                    CNCode = Convert.ToInt32(id);
                    if (objDAL.FinalizeCNHabitationDetailsPMGSY3DAL(CNCode))
                    {
                        return Json(new { success = true, message = "Habitations finalized successfully!!" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Error occurred while finalizing Habitations." });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "TR/MRL Road details are not present." });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.FinalizeCNHabitationDetailsPMGSY3");
                return Json(new { success = false, message = "Error occurred while Habitations Finalize." });
            }
        }

        #endregion

        #region PMGSY3 CN Finalize BLOCK/DISTRICT
        [HttpGet]
        public ActionResult FinalizeMRLPMGSY3Layout()
        {
            FinalizeMRLPMGSY3ViewModel model = new FinalizeMRLPMGSY3ViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                model.lstDistricts = comm.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                model.lstDistricts.Find(x => x.Value == "-1").Text = "Select District";

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.FinalizeMRLPMGSY3Layout()");
                return null;
            }
        }

        [HttpGet]
        [Audit]
        public ActionResult GetBlockListMRLPMGSY3(int? page, int? rows, string sidx, string sord)
        {
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
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
                    rows = objDAL.GetBlockListMRLPMGSY3DAL(districtCode, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords, Request.Params["filters"]
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
                ErrorLog.LogError(ex, "CoreNetwork.GetBlockListMRLPMGSY3()");
                return null;
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult FinalizeMRLBlock(String parameter, String hash, String key)
        {
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            string message = string.Empty;
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                int blockCode = Convert.ToInt32(decryptedParameters["BlockCode"]);

                if (decryptedParameters.Count() > 0)
                {
                    bool status = objDAL.FinalizeMRLBlockPMGSY3DAL(blockCode, ref message);
                    return Json(new { success = status, message = message.Trim() });
                }
                else
                {
                    return Json(new { success = false, message = "Invalid request" });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.FinalizeMRLBlock()");
                return Json(new { success = false, message = "Error occurred while processing the request." });
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult FinalizeMRLDistrict(int districtCode)
        {
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            string message = string.Empty;
            try
            {
                bool status = objDAL.FinalizeMRLDistrictPMGSY3DAL(districtCode, ref message);
                return Json(new { success = status, message = message.Trim() });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.FinalizeMRLDistrict()");
                return Json(new { success = false, message = "Error occurred while processing the request." });
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult DeFinalizeMRLBlock(String parameter, String hash, String key)
        {
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            string message = string.Empty;
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                int blockCode = Convert.ToInt32(decryptedParameters["BlockCode"]);

                if (decryptedParameters.Count() > 0)
                {
                    bool status = objDAL.DeFinalizeMRLBlockPMGSY3DAL(blockCode, ref message);
                    return Json(new { success = status, message = message.Trim() });
                }
                else
                {
                    return Json(new { success = false, message = "Invalid request" });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.DeFinalizeMRLBlock()");
                return Json(new { success = false, message = "Error occurred while processing the request." });
            }
        }
        #endregion

        #region PCI Entry
        [HttpGet]
        public ActionResult GetPCIForPmgsyIIIRoad()
        {
            try
            {
                PCIEntryViewModel viewmodel = new PCIEntryViewModel();
                CommonFunctions commonFunctions = new CommonFunctions();

                viewmodel.FyearList = commonFunctions.PopulateFinancialYear(true, false).AsEnumerable<SelectListItem>().ToList<SelectListItem>();
                viewmodel.FyearList.RemoveAll(x => Convert.ToInt32(x.Value) < 2019 && Convert.ToInt32(x.Value) != 0);


                //IEnumerable x = commonFunctions.PopulateFinancialYear(true, true).AsEnumerable<SelectListItem>().ToList<SelectListItem>().OrderByDescending(t => t.Text);

                //var type = x.GetType();
                viewmodel.RoadtypeList = new List<SelectListItem>();
                viewmodel.RoadtypeList.Add(new SelectListItem { Text = "Candidate Roads", Value = "C", Selected = true });
                viewmodel.BlockList = commonFunctions.PopulateBlocks(PMGSYSession.Current.DistrictCode, false);
                return View(viewmodel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork/GetPCIForPmgsyIIIRoad");
                return null;
            }
        }

        [HttpPost]
        public JsonResult GetPmgsyRoadList(FormCollection formCollection)
        {
            try
            {
                if (PMGSYSession.Current == null)
                {
                    Response.Redirect("/Login/Login");
                }

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                int IMS_YEAR = Convert.ToInt32(Request.Params["IMS_YEAR"]);
                int IMS_BLOCK_ID = Convert.ToInt32(Request.Params["Block"]);

                int totalRecords = 0;
                CoreNetworkDAL objDAL = new CoreNetworkDAL();

                var jsonData = new
                {
                    rows = objDAL.GetPmgsyRoadsDAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, PMGSYSession.Current.StateCode
                    , PMGSYSession.Current.DistrictCode, PMGSYSession.Current.AdminNdCode, IMS_YEAR, IMS_BLOCK_ID),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork/GetPmgsyRoadList");
                return null;
            }
        }

        [HttpPost]
        public JsonResult GetCNRoadList(FormCollection formCollection)
        {
            try
            {
                if (PMGSYSession.Current == null)
                {
                    Response.Redirect("/Login/Login");
                }

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 29-Apr-2014 end

                int IMS_BLOCK_ID = Convert.ToInt32(Request.Params["Block"]);
                int IMS_YEAR = Convert.ToInt32(Request.Params["IMSYEAR"]);
                int totalRecords = 0;
                CoreNetworkDAL objDAL = new CoreNetworkDAL();

                var jsonData = new
                {
                    rows = objDAL.GetCNRoadsDAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, PMGSYSession.Current.StateCode, PMGSYSession.Current.DistrictCode, PMGSYSession.Current.AdminNdCode, IMS_BLOCK_ID, IMS_YEAR),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork/GetCNRoadList");
                return null;
            }
        }

        [HttpGet]
        public ActionResult AddPciForCNRoad(String parameter, String hash, String key)
        {
            try
            {
                int PLAN_CN_ROAD_CODE = 0, ER_ROAD_CODE = 0;

                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        PLAN_CN_ROAD_CODE = Convert.ToInt32(urlSplitParams[0]);
                        ER_ROAD_CODE = Convert.ToInt32(urlSplitParams[1]);
                    }
                }
                PMGSYEntities dbContext = new PMGSYEntities();
                PLAN_ROAD plan_road = dbContext.PLAN_ROAD.Where(x => x.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE && x.MAST_ER_ROAD_CODE == ER_ROAD_CODE).FirstOrDefault();

                MASTER_EXISTING_ROADS MASTER_EXISTING_ROADS = dbContext.MASTER_EXISTING_ROADS.Find(ER_ROAD_CODE);

                PLAN_ROAD_MRL_PMGSY3 PLAN_ROAD_MRL_PMGSY3 = dbContext.PLAN_ROAD_MRL_PMGSY3.Where(x => x.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE && x.MAST_ER_ROAD_CODE == ER_ROAD_CODE).FirstOrDefault();

                CommonFunctions objCommonFunction = new CommonFunctions();

                PCIIndexViewModel pciIndexViewModel = new PCIIndexViewModel();

                //var PCIEntryForPlanroad = dbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(x => x.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE).ToList().LastOrDefault();
                var PCIEntryForPlanroad = dbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(x => x.MAST_ER_ROAD_CODE == ER_ROAD_CODE && x.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE).ToList().LastOrDefault();


                pciIndexViewModel.SURFACES = objCommonFunction.PopulateSurfaceType();
                pciIndexViewModel.ENC_PLAN_CN_ROAD_CODE = URLEncrypt.EncryptParameters(new string[] { PLAN_CN_ROAD_CODE.ToString().Trim() });
                pciIndexViewModel.ENC_ER_ROAD_CODE = URLEncrypt.EncryptParameters(new string[] { ER_ROAD_CODE.ToString().Trim() });

                pciIndexViewModel.EncERCodePlanCode = URLEncrypt.EncryptParameters(new string[] { PLAN_CN_ROAD_CODE.ToString() + "$" + ER_ROAD_CODE.ToString() });

                pciIndexViewModel.ER_ROAD_CODE = ER_ROAD_CODE;

                var MRLEntry = dbContext.PLAN_ROAD_MRL_PMGSY3.Where(x => x.MAST_ER_ROAD_CODE == ER_ROAD_CODE && x.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE).FirstOrDefault();

                if (PCIEntryForPlanroad != null)
                    pciIndexViewModel.MANE_STR_CHAIN = PCIEntryForPlanroad.MANE_END_CHAIN;

                else if (MRLEntry != null)
                {
                    pciIndexViewModel.MANE_STR_CHAIN = Convert.ToDecimal(MRLEntry.PLAN_RD_FROM_CHAINAGE);
                }

                else
                    pciIndexViewModel.MANE_STR_CHAIN = plan_road.PLAN_RD_FROM_CHAINAGE.HasValue ? plan_road.PLAN_RD_FROM_CHAINAGE.Value : 0;

                //pciIndexViewModel.RoadName = plan_road.PLAN_RD_NAME;
                pciIndexViewModel.RoadName = MASTER_EXISTING_ROADS.MAST_ER_ROAD_NAME;

                if (plan_road != null)
                    pciIndexViewModel.erRoadName = plan_road.MASTER_EXISTING_ROADS.MAST_ER_ROAD_NAME.Trim();
                //pciIndexViewModel.RoadLength = plan_road.PLAN_RD_TOTAL_LEN.HasValue ? plan_road.PLAN_RD_TOTAL_LEN.Value : (plan_road.PLAN_RD_LENGTH == null ? 0 : Convert.ToDecimal(plan_road.PLAN_RD_LENGTH));

                if (PLAN_ROAD_MRL_PMGSY3 != null)
                    pciIndexViewModel.RoadLength = Convert.ToDecimal(PLAN_ROAD_MRL_PMGSY3.PLAN_RD_LENGTH);

                else
                    pciIndexViewModel.RoadLength = Convert.ToDecimal(plan_road.PLAN_RD_LENGTH);


                if (DateTime.Now.Month <= 3)
                {
                    List<SelectListItem> lstYears = objCommonFunction.PopulateUpToYear(true, false);
                    lstYears.RemoveAt(1);
                    pciIndexViewModel.YEARS = lstYears;

                }
                else
                {
                    pciIndexViewModel.YEARS = objCommonFunction.PopulateUpToYear(true, false);


                }

                Int32 BlockCode = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE).Select(m => m.MAST_BLOCK_CODE).FirstOrDefault();

                if (dbContext.MAST_PCI_BLOCK_PMGSY3_FINALIZE.Any(m => m.MAST_BLOCK_CODE == BlockCode && m.IS_FINALIZED == "Y"))
                {

                    pciIndexViewModel.isBlockFinalizedForPCI = true;
                }
                else
                {
                    pciIndexViewModel.isBlockFinalizedForPCI = false;

                }



                pciIndexViewModel.YEARS.RemoveAll(x => Convert.ToInt32(x.Value) < 2019 && Convert.ToInt32(x.Value) != 0);

                #region To check wheather DRRP is finalized

                pciIndexViewModel.isfinalizedEntry = false;
                pciIndexViewModel.isLengthComplete = false;
                var Entries = dbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(x => x.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE && x.MAST_ER_ROAD_CODE == ER_ROAD_CODE).Select(y => y.IS_FINALIZED).ToList();
                foreach (var item in Entries)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        pciIndexViewModel.isfinalizedEntry = item.Equals("Y");
                        if (pciIndexViewModel.isfinalizedEntry)
                            break;
                    }

                }

                #endregion

                #region To check full length entry has been done

                var StartPCIEntry = dbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(x => x.MAST_ER_ROAD_CODE == ER_ROAD_CODE && x.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE).ToList().FirstOrDefault();
                var EndPCIEntry = dbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(x => x.MAST_ER_ROAD_CODE == ER_ROAD_CODE && x.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE).ToList().LastOrDefault();
                if (StartPCIEntry != null && EndPCIEntry != null)
                {
                    var Difference = EndPCIEntry.MANE_END_CHAIN - StartPCIEntry.MANE_STR_CHAIN;
                    if (Difference == pciIndexViewModel.RoadLength)
                    {
                        pciIndexViewModel.isLengthComplete = true;
                    }
                }
                #endregion

                return View("AddPciForCNRoad", pciIndexViewModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork/AddPciForCNRoad");
                return null;
            }
        }


        [Audit]
        public JsonResult GetCNRoadLengthDetailsDetailsForVerification()
        {
            int PLAN_CN_ROAD_CODE = 0;
            string[] EncKey = Request.Params["ENC_PLAN_CN_ROAD_CODE"].Split('/');
            int MANE_IMS_YEAR = Convert.ToInt32(Request.Params["MANE_IMS_YEAR"]);
            int ER_ROAD_CODE = Convert.ToInt32(Request.Params["ER_ROAD_CODE"]);

            PMGSYEntities dbContext = new PMGSYEntities();

            if (EncKey.Length == 3)
            {
                if (!String.IsNullOrEmpty(EncKey[0]) && !String.IsNullOrEmpty(EncKey[1]) && !String.IsNullOrEmpty(EncKey[2]))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { EncKey[0], EncKey[1], EncKey[2] });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        PLAN_CN_ROAD_CODE = Convert.ToInt32(urlSplitParams[0]);
                    }
                }
            }
            else
            {
                return Json(new { Success = false, ErrorMessage = "Error Occured while processing your Request." });
            }


            // Check if block is finalized or not. table omms.MAST_PCI_BLOCK_PMGSY3_FINALIZE

            //Int32 BlockCode = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE).Select(m => m.MAST_BLOCK_CODE).FirstOrDefault();

            //if (dbContext.MAST_PCI_BLOCK_PMGSY3_FINALIZE.Any(m => m.MAST_BLOCK_CODE == BlockCode && m.IS_FINALIZED == "Y"))
            //{

            //    return Json(new { Success = false, ErrorMessage = "PCI entry is not allowed for any year as the overall Block is finalized under PCI." });
            //}



            PLAN_ROAD plan_road = dbContext.PLAN_ROAD.Where(x => x.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE && x.MAST_ER_ROAD_CODE == ER_ROAD_CODE && x.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme).FirstOrDefault();

            var PCIEntryForPlanroad = dbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(x => x.MAST_ER_ROAD_CODE == ER_ROAD_CODE && x.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE && x.MANE_PCI_YEAR == MANE_IMS_YEAR).ToList().LastOrDefault();

            var MRLEntry = dbContext.PLAN_ROAD_MRL_PMGSY3.Where(x => x.MAST_ER_ROAD_CODE == ER_ROAD_CODE && x.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE).FirstOrDefault();




            if (PCIEntryForPlanroad != null)
            {
                return Json(new { Success = true, MANE_ST_CHAIN = PCIEntryForPlanroad.MANE_END_CHAIN });

                // pciIndexViewModel.MANE_STR_CHAIN = PCIEntryForPlanroad.MANE_END_CHAIN;
            }
            else if (MRLEntry != null)
            {
                return Json(new { Success = true, MANE_ST_CHAIN = Convert.ToDecimal(MRLEntry.PLAN_RD_FROM_CHAINAGE) });
                //  pciIndexViewModel.MANE_STR_CHAIN = Convert.ToDecimal(MRLEntry.PLAN_RD_FROM_CHAINAGE);
            }

            else
            {
                if (plan_road.PLAN_RD_FROM_CHAINAGE.HasValue)
                {
                    return Json(new { Success = true, MANE_ST_CHAIN = plan_road.PLAN_RD_FROM_CHAINAGE.Value });
                }
                else
                {
                    return Json(new { Success = true, MANE_ST_CHAIN = 0 });
                }
                //  pciIndexViewModel.MANE_STR_CHAIN = plan_road.PLAN_RD_FROM_CHAINAGE.HasValue ? plan_road.PLAN_RD_FROM_CHAINAGE.Value : 0;
            }





            //string status = GetCNRoadDetailsHere(PLAN_CN_ROAD_CODE, MANE_IMS_YEAR, ER_ROAD_CODE);

            //if (status == "-111")
            //{
            //    return Json(new { Success = false, ErrorMessage = "All Entries of PCI For this Year has been done." });
            //}
            //if (status == "-999")
            //{
            //    return Json(new { Success = false, ErrorMessage = "Error occured while processing your request." });
            //}
            //else
            //{
            //    return Json(new { Success = true, MANE_END_CHAIN = status });
            //}
        }

        [Audit]
        public JsonResult CheckIfDRRPIsFinalizedOrNot()
        {
            int PLAN_CN_ROAD_CODE = 0;
            string[] EncKey = Request.Params["ENC_PLAN_CN_ROAD_CODE"].Split('/');
            int MANE_IMS_YEAR = Convert.ToInt32(Request.Params["MANE_IMS_YEAR"]);
            int ER_ROAD_CODE = Convert.ToInt32(Request.Params["ER_ROAD_CODE"]);

            PMGSYEntities dbContext = new PMGSYEntities();


            if (EncKey.Length == 3)
            {
                if (!String.IsNullOrEmpty(EncKey[0]) && !String.IsNullOrEmpty(EncKey[1]) && !String.IsNullOrEmpty(EncKey[2]))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { EncKey[0], EncKey[1], EncKey[2] });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        PLAN_CN_ROAD_CODE = Convert.ToInt32(urlSplitParams[0]);
                    }
                }
            }
            else
            {
                return Json(new { Success = false, ErrorMessage = "Error Occured while processing your Request." });
            }

            #region Check if Length is completed or Not
            decimal RoadLength = 0.0M;

            bool isfinalizedEntry = false;
            bool isLengthComplete = false;

            PLAN_ROAD_MRL_PMGSY3 PLAN_ROAD_MRL_PMGSY3 = dbContext.PLAN_ROAD_MRL_PMGSY3.Where(x => x.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE && x.MAST_ER_ROAD_CODE == ER_ROAD_CODE).FirstOrDefault();
            PLAN_ROAD plan_road = dbContext.PLAN_ROAD.Where(x => x.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE && x.MAST_ER_ROAD_CODE == ER_ROAD_CODE && x.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme).FirstOrDefault();


            if (PLAN_ROAD_MRL_PMGSY3 != null)
                RoadLength = Convert.ToDecimal(PLAN_ROAD_MRL_PMGSY3.PLAN_RD_LENGTH);

            else
                RoadLength = Convert.ToDecimal(plan_road.PLAN_RD_LENGTH);

            var StartPCIEntry = dbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(x => x.MAST_ER_ROAD_CODE == ER_ROAD_CODE && x.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE && x.MANE_PCI_YEAR == MANE_IMS_YEAR).ToList().FirstOrDefault();
            var EndPCIEntry = dbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(x => x.MAST_ER_ROAD_CODE == ER_ROAD_CODE && x.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE && x.MANE_PCI_YEAR == MANE_IMS_YEAR).ToList().LastOrDefault();



            //using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
            //{
            //    sw.WriteLine("Date :" + DateTime.Now.ToString());
            //    sw.WriteLine("CoreNetworkController().CheckIfDRRPIsFinalizedOrNot()");
            //    sw.WriteLine("StartPCIEntry = " + StartPCIEntry);
            //    sw.WriteLine("EndPCIEntry = " + EndPCIEntry);
            //    //sw.WriteLine("Exception : " + new CommonFunctions().FormatErrorMessage(modelstate));
            //    sw.WriteLine("---------------------------------------------------------------------------------------");
            //    sw.Close();
            //}



            //    Check if block is finalized or not. table omms.MAST_PCI_BLOCK_PMGSY3_FINALIZE

            //Int32 BlockCode = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE).Select(m => m.MAST_BLOCK_CODE).FirstOrDefault();

            //if (dbContext.MAST_PCI_BLOCK_PMGSY3_FINALIZE.Any(m => m.MAST_BLOCK_CODE == BlockCode && m.IS_FINALIZED == "Y"))
            //{

            //     return Json(new { Success = true, IS_FINALIZED = isfinalizedEntry, IS_LEN_COMPLETED = isLengthComplete });
            //    //return Json(new { Success = false, ErrorMessage = "PCI entry is not allowed for any year as the overall Block is finalized under PCI." });
            //}



            if (StartPCIEntry != null && EndPCIEntry != null)
            {
                var Difference = EndPCIEntry.MANE_END_CHAIN - StartPCIEntry.MANE_STR_CHAIN;

                if (Difference == RoadLength)
                {
                    isLengthComplete = true;
                }
            }

            #endregion


            var Entries = dbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(x => x.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE && x.MAST_ER_ROAD_CODE == ER_ROAD_CODE && x.MANE_PCI_YEAR == MANE_IMS_YEAR).Select(y => y.IS_FINALIZED).ToList();




            //using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
            //{
            //    foreach (var item in Entries)
            //    {
            //        sw.WriteLine("Date :" + DateTime.Now.ToString());
            //        sw.WriteLine("CoreNetworkController().CheckIfDRRPIsFinalizedOrNot()");
            //        sw.WriteLine("Entries = " + Entries);

            //        //sw.WriteLine("Exception : " + new CommonFunctions().FormatErrorMessage(modelstate));
            //        sw.WriteLine("---------------------------------------------------------------------------------------");
            //        sw.Close();
            //    }
            //}





            foreach (var item in Entries)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    if (item.Equals("Y"))
                    {
                        isfinalizedEntry = true;
                        return Json(new { Success = true, IS_FINALIZED = isfinalizedEntry, IS_LEN_COMPLETED = isLengthComplete });
                        //break;
                    }

                }

            }

            return Json(new { Success = true, IS_FINALIZED = isfinalizedEntry, IS_LEN_COMPLETED = isLengthComplete });
        }

        public string GetCNRoadDetailsHere(int PLAN_CN_ROAD_CODE, int MANE_IMS_YEAR, int ER_ROAD_CODE)
        {
            try
            {
                PMGSYEntities DbContext = new PMGSYEntities();
                if (DbContext.MANE_CN_PCI_INDEX.Where(c => c.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE && c.MANE_PCI_YEAR == MANE_IMS_YEAR).Any())
                {
                    decimal? Road_Length = DbContext.PLAN_ROAD.Where(a => a.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE).Select(a => a.PLAN_RD_LENGTH).First();

                    decimal Max_End_Chain = (
                                                from c in DbContext.MANE_CN_PCI_INDEX
                                                where
                                                    c.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE &&
                                                    c.MANE_PCI_YEAR == MANE_IMS_YEAR
                                                select
                                                    c.MANE_END_CHAIN
                                            ).Max();

                    if (Max_End_Chain == Road_Length)
                    {
                        return "-111";
                    }
                    else
                    {
                        return Max_End_Chain.ToString();
                    }
                }
                else
                {
                    decimal? startChainage = DbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE).Select(m => m.PLAN_RD_FROM_CHAINAGE).FirstOrDefault();
                    if (startChainage == null)
                    {
                        return "0";
                    }
                    else
                    {
                        return startChainage.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return "-999";
            }
            finally
            {
                //System.Data.Entity.DbContext.Dispose();
            }
        }







        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SavePciForCNRoad(PCIIndexViewModel pciIndexViewModel)
        {
            try
            {
                PMGSYEntities dbContext = new PMGSYEntities();
                if (ModelState.IsValid)
                {
                    int PLAN_CN_ROAD_CODE = 0;
                    string[] EncKey = pciIndexViewModel.ENC_PLAN_CN_ROAD_CODE.Split('/');

                    if (EncKey.Length == 3)
                    {
                        if (!String.IsNullOrEmpty(EncKey[0]) && !String.IsNullOrEmpty(EncKey[1]) && !String.IsNullOrEmpty(EncKey[2]))
                        {
                            String[] urlParams = URLEncrypt.DecryptParameters(new String[] { EncKey[0], EncKey[1], EncKey[2] });
                            if (urlParams.Length >= 1)
                            {
                                String[] urlSplitParams = urlParams[0].Split('$');
                                PLAN_CN_ROAD_CODE = Convert.ToInt32(urlSplitParams[0]);
                            }
                        }
                    }
                    else
                    {
                        return Json(new { Success = false, ErrorMessage = "Error Occured while processing your Request." });
                    }
                    var ChainageDiffrence = pciIndexViewModel.MANE_END_CHAIN - pciIndexViewModel.MANE_STR_CHAIN;
                    if (ChainageDiffrence > 1 && ChainageDiffrence != -1)
                    {
                        ModelState.AddModelError("MANE_END_CHAIN", "The difference between end chainage and start chainage should be 1.");
                        return Json(new { Success = false, ErrorMessage = "The difference between end chainage and start chainage should be 1." });
                    }



                    // Check if block is finalized or not. table omms.MAST_PCI_BLOCK_PMGSY3_FINALIZE

                    Int32 BlockCode = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE).Select(m => m.MAST_BLOCK_CODE).FirstOrDefault();

                    if (dbContext.MAST_PCI_BLOCK_PMGSY3_FINALIZE.Any(m => m.MAST_BLOCK_CODE == BlockCode && m.IS_FINALIZED == "Y"))
                    {
                        return Json(new { Success = false, ErrorMessage = "PCI entry is not allowed for any year as the overall Block is finalized under PCI." });
                    }





                    #region To check full length entry has been done






                    var StartPCIEntry = dbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(x => x.MAST_ER_ROAD_CODE == pciIndexViewModel.ER_ROAD_CODE && x.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE && x.MANE_PCI_YEAR == pciIndexViewModel.MANE_PCI_YEAR).ToList().FirstOrDefault();
                    var EndPCIEntry = dbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(x => x.MAST_ER_ROAD_CODE == pciIndexViewModel.ER_ROAD_CODE && x.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE && x.MANE_PCI_YEAR == pciIndexViewModel.MANE_PCI_YEAR).ToList().LastOrDefault();

                    if (StartPCIEntry != null && EndPCIEntry != null)
                    {
                        var EntryDiff = EndPCIEntry.MANE_END_CHAIN - StartPCIEntry.MANE_STR_CHAIN;
                        var ModelDiff = pciIndexViewModel.MANE_END_CHAIN - pciIndexViewModel.MANE_STR_CHAIN;
                        var sum = EntryDiff + ModelDiff;
                        if (sum > pciIndexViewModel.RoadLength)
                        {
                            return Json(new { Success = false, ErrorMessage = "PCI Entry can not be greater than the Road Length." });
                        }
                    }

                    #endregion

                    pciIndexViewModel.PLAN_CN_ROAD_CODE = PLAN_CN_ROAD_CODE;

                    CoreNetworkDAL objDAL = new CoreNetworkDAL();
                    string status = objDAL.SavePciForCNRoadDAL(pciIndexViewModel);

                    if (status == string.Empty)
                    {
                        return Json(new { Success = true });
                    }
                    else
                    {
                        return Json(new { Success = false, ErrorMessage = status });
                    }
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork/SavePciForCNRoad");
                return null;
            }
        }

        [HttpPost]
        public JsonResult GetPCIListForCNRoad(FormCollection formCollection)
        {
            try
            {
                if (PMGSYSession.Current == null)
                {
                    Response.Redirect("/Login/Login");
                }

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                string urlparam = Request.Params["PLAN_CN_ROAD_CODE"].Split('$')[0];
                int ER_ROAD_CODE = Convert.ToInt32(Request.Params["PLAN_CN_ROAD_CODE"].Split('$')[1]);
                string[] EncKey = urlparam.Split('/');
                int PLAN_CN_ROAD_CODE = 0;

                if (EncKey.Length == 3)
                {
                    if (!String.IsNullOrEmpty(EncKey[0]) && !String.IsNullOrEmpty(EncKey[1]) && !String.IsNullOrEmpty(EncKey[2]))
                    {
                        String[] urlParams = URLEncrypt.DecryptParameters(new String[] { EncKey[0], EncKey[1], EncKey[2] });
                        if (urlParams.Length >= 1)
                        {
                            String[] urlSplitParams = urlParams[0].Split('$');
                            PLAN_CN_ROAD_CODE = Convert.ToInt32(urlSplitParams[0]);
                        }
                    }
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = "Error Occured while processing your Request." });
                }

                int totalRecords = 0;
                CoreNetworkDAL objDAL = new CoreNetworkDAL();
                var jsonData = new
                {
                    rows = objDAL.GetPCIListForCNRoadDAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, PLAN_CN_ROAD_CODE, ER_ROAD_CODE),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork/GetPCIListForCNRoad");
                return null;
            }

        }

        [HttpPost]
        public ActionResult GetPhotoUploadView(string parameter, string hash, string key)
        {
            try
            {
                PMGSYEntities dbcontext = new PMGSYEntities();
                PCIPhotoViewModel viewmodel = new PCIPhotoViewModel();
                Dictionary<string, string> decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                int PCIid = Convert.ToInt32(decryptedParameters["PCIID"].ToString());
                var PCIEntryDetails = dbcontext.MANE_CN_PCI_INDEX_PMGSY3.Where(x => x.PCI_ID == PCIid).FirstOrDefault();
                var PhotoGraphEntryCount = dbcontext.MANE_PCI_IMAGE_MAPPING_PMGSY3.Where(x => x.PCI_ID == PCIid).Count();

                #region if entry is finalized

                bool isFinalized = false;
                //var EntryToCheck = dbcontext.MANE_CN_PCI_INDEX_PMGSY3.Where(x => x.PCI_ID == PCIid).FirstOrDefault();
                if (PCIEntryDetails != null)
                {
                    if (PCIEntryDetails.IS_FINALIZED != null)
                        isFinalized = PCIEntryDetails.IS_FINALIZED.Equals("Y");
                }

                #endregion

                viewmodel.PCIid = parameter + "/" + hash + "/" + key;
                viewmodel.PLAN_CN_ROAD_CODE = PCIEntryDetails.PLAN_CN_ROAD_CODE;
                if (isFinalized)
                {
                    viewmodel.NumberofFiles = 2;
                }
                else
                {
                    viewmodel.NumberofFiles = PhotoGraphEntryCount;
                }

                return View(viewmodel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork/GetPhotoUploadView");
                return null;
            }
        }

        public ActionResult SavePhotograph(PCIPhotoViewModel formmodel)
        {
            try
            {
                CommonFunctions objCommonFunc = new CommonFunctions();

                String parameter = formmodel.PCIid.Split('/')[0];
                String hash = formmodel.PCIid.Split('/')[1];
                String key = formmodel.PCIid.Split('/')[2];

                Dictionary<string, string> decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                int pciid = Convert.ToInt32(decryptedParameters["PCIID"].ToString());

                foreach (string file in Request.Files)
                {
                    string status = ValidatePhoto(Request.Files[0].ContentLength, Path.GetExtension(Request.Files[0].FileName));
                    if (status != string.Empty)
                    {
                        formmodel.ErrorMessage = status;
                        return Json(new { message = "Photograph format is not valid", success = false });
                    }
                }
                bool isFileSaved = false;
                HttpPostedFileBase FileBase = null;
                CoreNetworkDAL objDAL = new CoreNetworkDAL();

                for (int i = 0; i < Request.Files.Count; i++)
                {
                    FileBase = Request.Files[i];
                    var filename = FileBase.FileName;

                    //var remark = Request.Params["InspPdfDescription[]"];
                    //if (remark == null || string.IsNullOrEmpty(remark) || string.IsNullOrWhiteSpace(remark))
                    //{
                    //    return Json(new { success = isFileSaved, message = "Please Enter Remark" }, JsonRequestBehavior.DenyGet);
                    //}
                    string remark = Request.Params["InspPdfDescription[]"];
                    Regex regex = new Regex(@"^[a-zA-Z0-9 ,.()-]+$");
                    if (!regex.IsMatch(remark))
                    {
                        return Json(new { success = isFileSaved, message = "Only alphabets and numbers allowed in remark" }, JsonRequestBehavior.DenyGet);
                    }

                    isFileSaved = objDAL.SavePhotoGraphDAL(pciid, filename, FileBase, Request.Params["InspPdfDescription[]"]);
                }

                formmodel.NumberofImages = 0;

                var serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;

                if (isFileSaved)
                    return Json(new { success = isFileSaved, message = "Photograph uploaded successfully" }, JsonRequestBehavior.DenyGet);
                else
                    return Json(new { success = isFileSaved, message = "Photograph not uploaded" }, JsonRequestBehavior.DenyGet);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork/SavePhotograph");
                return null;
            }
        }

        public string ValidatePhoto(int FileSize, string FileExtension)
        {
            try
            {
                if (!(FileExtension.ToUpper().Equals(".JPEG") || FileExtension.ToUpper().Equals(".JPG")))
                {
                    return "Photograph not in correct format";
                }
                if (FileSize > Convert.ToInt32(ConfigurationManager.AppSettings["PROPOSAL_PDF_FILE_MAX_SIZE"]))
                {
                    return "File Size Exceed the Maximum File Limit";
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork/ValidatePhoto");
                return null;
            }
        }


        [HttpPost]
        public JsonResult ListImageFiles(FormCollection formCollection)
        {

            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var param = Request.Params["PCIIDParam"];
                String parameter = param.Split('/')[0];
                String hash = param.Split('/')[1];
                String key = param.Split('/')[2];

                Dictionary<string, string> decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                int pciid = Convert.ToInt32(decryptedParameters["PCIID"].ToString());


                int totalRecords;
                var jsonData = new
                {
                    rows = objDAL.GetImageFilesListDAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, pciid),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork/ListImageFiles");
                return null;
            }
        }

        [HttpPost]
        public JsonResult UpdateImageRemarkDetails(FormCollection formCollection)
        {
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            try
            {
                string arrKey = formCollection["id"];


                Regex regex = new Regex(@"^[a-zA-Z0-9 ,.()-]+$");
                if (regex.IsMatch(formCollection["Description"]) && formCollection["Description"].Trim().Length != 0)
                {
                    //fileuploadViewModel.PdfDescription = formCollection["Description"];
                }
                else
                {
                    return Json("Invalid PDF Description, Only Alphabets,Numbers and [,.()-] are allowed");
                }

                //string status = qualityBAL.UpdateMultipleInspPDFDetailsBAL(fileuploadViewModel);

                //if (status == string.Empty)
                //  return Json(true);
                //else
                return Json("There is an error occurred while processing your request.");
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork/UpdateImageRemarkDetails");
                return Json("There is an error occurred while processing your request.");
            }
        }

        [HttpPost]
        public JsonResult DeleteMultipleInspFileDetails(string id)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            try
            {

                var param = Request.Params["Fileid"];
                String parameter = param.Split('/')[0];
                String hash = param.Split('/')[1];
                String key = param.Split('/')[2];

                Dictionary<string, string> decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                int fileid = Convert.ToInt32(decryptedParameters["FileID"].ToString());

                var EntryDetails = dbContext.MANE_PCI_IMAGE_MAPPING_PMGSY3.Where(x => x.FILE_ID == fileid).FirstOrDefault();
                int PCIid = EntryDetails.PCI_ID;
                string Filename = EntryDetails.FILE_NAME;
                bool isEntryDeleted = false;
                using (TransactionScope ts = new TransactionScope())
                {
                    if (EntryDetails != null)
                    {
                        dbContext.MANE_PCI_IMAGE_MAPPING_PMGSY3.Remove(EntryDetails);
                        dbContext.SaveChanges();
                        ts.Complete();
                        isEntryDeleted = true;
                    }
                }
                var count = dbContext.MANE_PCI_IMAGE_MAPPING_PMGSY3.Where(x => x.PCI_ID == PCIid).Count();

                if (isEntryDeleted)
                {
                    if (System.IO.File.Exists(Path.Combine(ConfigurationManager.AppSettings["PCI_INDEX_CHAINAGE_PHOTO"], Filename)))
                    {
                        System.IO.File.Delete(Path.Combine(ConfigurationManager.AppSettings["PCI_INDEX_CHAINAGE_PHOTO"], Filename));
                        return Json(new { Success = true, Message = "Photograph deleted successfully.", photocount = count });
                    }
                }
                return Json(new { Success = false, ErrorMessage = "There is an error while processing your request." });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork/DeleteMultipleInspFileDetails");

                return Json(new { Success = false, ErrorMessage = "There is an error while processing your request." });
            }
        }

        [HttpPost]
        public JsonResult DeletePciForCNRoad()
        {
            PMGSYEntities dbcontext = new PMGSYEntities();
            try
            {
                string Data = string.Empty;
                string[] EncKey = Request.Params["Data"].Split('/');
                String[] urlSplitParams = { };
                if (EncKey.Length == 3)
                {
                    if (!String.IsNullOrEmpty(EncKey[0]) && !String.IsNullOrEmpty(EncKey[1]) && !String.IsNullOrEmpty(EncKey[2]))
                    {
                        String[] urlParams = URLEncrypt.DecryptParameters(new String[] { EncKey[0], EncKey[1], EncKey[2] });
                        if (urlParams.Length >= 1)
                        {
                            urlSplitParams = urlParams[0].Split('$');
                        }
                    }
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = "Error Occured while processing your Request." });
                }

                var param = Request.Params["Data"];
                String parameter = param.Split('/')[0];
                String hash = param.Split('/')[1];
                String key = param.Split('/')[2];
                Dictionary<string, string> decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                int PCIid = Convert.ToInt32(decryptedParameters["PCIID"].ToString());

                CoreNetworkDAL objDAL = new CoreNetworkDAL();

                var Entry = dbcontext.MANE_CN_PCI_INDEX_PMGSY3.Where(x => x.PCI_ID == PCIid).FirstOrDefault();
                var EntryAgainstPhoto = dbcontext.MANE_PCI_IMAGE_MAPPING_PMGSY3.Where(x => x.PCI_ID == Entry.PCI_ID).FirstOrDefault();

                if (Entry != null)
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        if (EntryAgainstPhoto != null)
                            dbcontext.MANE_PCI_IMAGE_MAPPING_PMGSY3.Remove(EntryAgainstPhoto);

                        dbcontext.MANE_CN_PCI_INDEX_PMGSY3.Remove(Entry);

                        dbcontext.SaveChanges();

                        ts.Complete();
                        return Json(new { Success = true });
                    }
                }
                return Json(new { Success = false });

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork/DeletePciForCNRoad");
                return null;
            }
        }

        [HttpPost]
        public ActionResult FinalizePCIRoadDetails(int SelectedYear)
        {
            try
            {
                PMGSYEntities dbcontext = new PMGSYEntities();
                var PLAN_ROAD_CODE = Convert.ToInt32(Request.Params["Data"]);

                var PLAN_ROAD_MRL_PMGSY3 = dbcontext.PLAN_ROAD_MRL_PMGSY3.Where(x => x.PLAN_CN_ROAD_CODE == PLAN_ROAD_CODE).Select(z => z.MAST_ER_ROAD_CODE).ToList();
                var PLAN_ROAD = dbcontext.PLAN_ROAD.Where(y => y.PLAN_CN_ROAD_CODE == PLAN_ROAD_CODE).Select(z => z.MAST_ER_ROAD_CODE).ToList();

               // var ImageEntry = dbcontext.MANE_PCI_IMAGE_MAPPING_PMGSY3.Where(x => x.MANE_CN_PCI_INDEX_PMGSY3.PLAN_CN_ROAD_CODE == PLAN_ROAD_CODE && x.MANE_CN_PCI_INDEX_PMGSY3.MANE_PCI_YEAR == SelectedYear).ToList();

                //var MANE_CN_PCI_INDEX_PMGSY3 = dbcontext.MANE_CN_PCI_INDEX_PMGSY3.Where(x => x.PLAN_CN_ROAD_CODE == PLAN_ROAD_CODE && x.MANE_PCI_YEAR == SelectedYear).ToList();

                var UnionListToCheck = PLAN_ROAD_MRL_PMGSY3.Union(PLAN_ROAD);

                // Check that PCI is entered against ALL TR / MRL Roads
                foreach (var ERCode in UnionListToCheck)
                {
                    if (dbcontext.MANE_CN_PCI_INDEX_PMGSY3.Any(x => x.PLAN_CN_ROAD_CODE == PLAN_ROAD_CODE && x.MANE_PCI_YEAR == SelectedYear && x.MAST_ER_ROAD_CODE == ERCode))
                    {
                    }
                    else
                    {
                        return Json(new { success = false, ErrorMessage = "PCI is not entered against all TR / MRL Roads " + "for Year " + SelectedYear + "-" + (SelectedYear + 1) }, JsonRequestBehavior.DenyGet);
                    
                    
                    }
                
                }


                #region To check if complete enrty has been done against length

                    var EntryToUpdate = dbcontext.MANE_CN_PCI_INDEX_PMGSY3.Where(x => x.PLAN_CN_ROAD_CODE == PLAN_ROAD_CODE && x.MANE_PCI_YEAR == SelectedYear).ToList();
                 

                    var EntryToUpdateERCode = EntryToUpdate.Select(obj => obj.MAST_ER_ROAD_CODE).ToList();

                    // Check if Roads are entered in omms.MANE_CN_PCI_INDEX_PMGSY3
                    foreach (var itemInUnionListToCheck in UnionListToCheck)
                    {
                        bool isEntryPresent = EntryToUpdateERCode.Contains(itemInUnionListToCheck);
                        if (!isEntryPresent)
                        {
                            return Json(new { success = false, ErrorMessage = "Could not finalize, PCI Entry against road " + "(" + itemInUnionListToCheck.ToString() + ") " + "is pending for Year " + SelectedYear + "-" + (SelectedYear + 1) }, JsonRequestBehavior.DenyGet);
                        }
                    }

                    // Check if Roads are already finalized in omms.MANE_CN_PCI_INDEX_PMGSY3
                    List<MANE_CN_PCI_INDEX_PMGSY3> EntryList = new List<MANE_CN_PCI_INDEX_PMGSY3>();
                    foreach (var item in EntryToUpdate)
                    {

                        if (!string.IsNullOrEmpty(item.IS_FINALIZED))
                        {
                            if (item.IS_FINALIZED.Equals("Y"))
                                return Json(new { success = false, ErrorMessage = "PCI Entry against the road has already been finalized  for Year " + SelectedYear + "-" + (SelectedYear + 1) }, JsonRequestBehavior.DenyGet);
                        }
                    }

                decimal RoadLength = 0;
                PLAN_ROAD RoadEntryPlanRoad = null;
                foreach (var item in UnionListToCheck)
                {
                    var start = dbcontext.MANE_CN_PCI_INDEX_PMGSY3.Where(x => x.PLAN_CN_ROAD_CODE == PLAN_ROAD_CODE && x.MAST_ER_ROAD_CODE == item && x.MANE_PCI_YEAR == SelectedYear).ToList().FirstOrDefault();
                    var end = dbcontext.MANE_CN_PCI_INDEX_PMGSY3.Where(x => x.PLAN_CN_ROAD_CODE == PLAN_ROAD_CODE && x.MAST_ER_ROAD_CODE == item && x.MANE_PCI_YEAR == SelectedYear).ToList().LastOrDefault();
                    if (start != null && end != null)
                    {
                        var RoadEntry = dbcontext.PLAN_ROAD_MRL_PMGSY3.Where(x => x.PLAN_CN_ROAD_CODE == PLAN_ROAD_CODE && x.MAST_ER_ROAD_CODE == item).FirstOrDefault();
                        if (RoadEntry == null)
                        {
                            RoadEntryPlanRoad = dbcontext.PLAN_ROAD.Where(x => x.PLAN_CN_ROAD_CODE == PLAN_ROAD_CODE && x.MAST_ER_ROAD_CODE == item).FirstOrDefault();
                        }
                        var diff = end.MANE_END_CHAIN - start.MANE_STR_CHAIN;

                        if (RoadEntry != null)
                        {
                            RoadLength = Convert.ToDecimal(RoadEntry.PLAN_RD_LENGTH);
                        }
                        else
                        {
                            //RoadLength = Convert.ToDecimal(RoadEntryPlanRoad.PLAN_RD_LENGTH);
                            //RoadLength = Convert.ToDecimal(RoadEntryPlanRoad.PLAN_RD_TOTAL_LEN);
                            RoadLength = (Convert.ToDecimal(RoadEntryPlanRoad.PLAN_RD_TO_CHAINAGE) - Convert.ToDecimal(RoadEntryPlanRoad.PLAN_RD_FROM_CHAINAGE));
                        }

                        if (diff < RoadLength)
                        {
                            return Json(new { success = false, ErrorMessage = "PCI Entry against the road " + "(" + item.ToString() + ") " + " is pending  for Year " + SelectedYear + "-" + (SelectedYear + 1) }, JsonRequestBehavior.DenyGet);
                        }
                    }


                }



                #endregion


                int outPut = CheckPCIFinalizationCondition(PLAN_ROAD_CODE, SelectedYear);

                if (outPut == 1)
                {
                    var EntryToUpdate1 = dbcontext.MANE_CN_PCI_INDEX_PMGSY3.Where(x => x.PLAN_CN_ROAD_CODE == PLAN_ROAD_CODE && x.MANE_PCI_YEAR == SelectedYear).ToList();
                    using (TransactionScope ts = new TransactionScope())
                    {
                        foreach (var itemToUpdate in EntryToUpdate1)
                        {
                            itemToUpdate.IS_FINALIZED = "Y";
                            itemToUpdate.FINALIZED_DATE = System.DateTime.Now;
                            dbcontext.Entry(itemToUpdate).State = System.Data.Entity.EntityState.Modified;
                        }
                        dbcontext.SaveChanges();
                        ts.Complete();
                        return Json(new { success = true, ErrorMessage = "PCI Entry against the road has been finalized for Year " + SelectedYear + "-" + (SelectedYear + 1) }, JsonRequestBehavior.DenyGet);
                    }
                }
                else
                {
                    return Json(new { success = false, ErrorMessage = "Image is not uploaded for any one of the Road in PCI Module  " + "for Year " + SelectedYear + "-" + (SelectedYear + 1) }, JsonRequestBehavior.DenyGet);
                
                }



                //if (PLAN_ROAD_CODE != 0)
                //{
                //    var EntryToUpdate = dbcontext.MANE_CN_PCI_INDEX_PMGSY3.Where(x => x.PLAN_CN_ROAD_CODE == PLAN_ROAD_CODE && x.MANE_PCI_YEAR == SelectedYear).ToList();
                //    // var ImageRequiredForPci=

                //    var EntryToUpdateERCode = EntryToUpdate.Select(obj => obj.MAST_ER_ROAD_CODE).ToList();

                //    foreach (var itemInUnionListToCheck in UnionListToCheck)
                //    {
                //        bool isEntryPresent = EntryToUpdateERCode.Contains(itemInUnionListToCheck);
                //        if (!isEntryPresent)
                //        {
                //            return Json(new { success = false, ErrorMessage = "Could not finalize, PCI Entry against road " + "(" + itemInUnionListToCheck.ToString() + ") " + "is pending for Year " + SelectedYear + "-" + (SelectedYear + 1) }, JsonRequestBehavior.DenyGet);
                //        }
                //    }

                //    List<MANE_CN_PCI_INDEX_PMGSY3> EntryList = new List<MANE_CN_PCI_INDEX_PMGSY3>();
                //    foreach (var item in EntryToUpdate)
                //    {

                //        if (!string.IsNullOrEmpty(item.IS_FINALIZED))
                //        {
                //            if (item.IS_FINALIZED.Equals("Y"))
                //                return Json(new { success = false, ErrorMessage = "PCI Entry against the road has already been finalized  for Year " + SelectedYear + "-" + (SelectedYear + 1) }, JsonRequestBehavior.DenyGet);
                //        }

                //        //var photorequired = dbcontext.USP_PCI_FINALIZATION_CONDITION(PLAN_ROAD_CODE, System.DateTime.Now.Year).FirstOrDefault();
                //        ////   if (photorequired.IMAGE_REQUIRED != 0)
                //        //{
                //        //    foreach (var itemimage in MANE_CN_PCI_INDEX_PMGSY3)
                //        //    {
                //        //        var imagecount = ImageEntry.Where(y => y.PCI_ID == itemimage.PCI_ID).Count();
                //        //        if (imagecount < 2)
                //        //        {
                //        //            return Json(new { success = false, ErrorMessage = "Could not finalize, As photograph is not uploaded against the PCI Entry of road " + itemimage.MAST_ER_ROAD_CODE.ToString() + " for Year " + SelectedYear + "-" + (SelectedYear + 1) }, JsonRequestBehavior.DenyGet);
                //        //        }

                //        //    }
                //        //}

                //        if (string.IsNullOrEmpty(item.IS_FINALIZED))
                //        {
                //            item.IS_FINALIZED = "Y";
                //            item.FINALIZED_DATE = System.DateTime.Now;
                //            EntryList.Add(item);
                //        }
                //    }



                //    EntryToUpdate.Clear();
                //    EntryToUpdate = EntryList;
                //    using (TransactionScope ts = new TransactionScope())
                //    {
                //        foreach (var itemToUpdate in EntryToUpdate)
                //        {
                //            dbcontext.Entry(itemToUpdate).State = System.Data.Entity.EntityState.Modified;
                //        }
                //        dbcontext.SaveChanges();
                //        ts.Complete();
                //        return Json(new { success = true, ErrorMessage = "PCI Entry against the road has been finalized for Year " + SelectedYear + "-" + (SelectedYear + 1) }, JsonRequestBehavior.DenyGet);
                //    }




                //}
                //else
                //{
                //    return Json(new { success = false, ErrorMessage = "PCI Entry against the road could not be finalized  for Year " + SelectedYear + "-" + (SelectedYear + 1) }, JsonRequestBehavior.DenyGet);
                //}
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork/FinalizePCIRoadDetails");
                //return null;
                return Json(new { success = false, ErrorMessage = "Error occured while finalizing PCI for Year " + SelectedYear + "-" + (SelectedYear + 1) }, JsonRequestBehavior.DenyGet);
            }
        }
             

        public int CheckPCIFinalizationCondition(int PlanCNRoadCode, int SelectedYear)
        {
            try
            {
                //string message = string.Empty;
                int ImageFinalized = 1;
                PMGSYEntities dbContext = new PMGSYEntities();

                List<Int32> ERCodeFromPlanRoad = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == PlanCNRoadCode && m.MASTER_EXISTING_ROADS.MAST_ROAD_CAT_CODE!=8).Select(m => m.MAST_ER_ROAD_CODE).Distinct().ToList<Int32>();
                List<Int32> ERCodeFromPlanRoadMRL = dbContext.PLAN_ROAD_MRL_PMGSY3.Where(m => m.PLAN_CN_ROAD_CODE == PlanCNRoadCode && m.MASTER_EXISTING_ROADS.MAST_ROAD_CAT_CODE != 8).Select(m => m.MAST_ER_ROAD_CODE).Distinct().ToList<Int32>();


                // Check Image only against below ER Codes.
                List<Int32> ERCodeList = ERCodeFromPlanRoad.Union(ERCodeFromPlanRoadMRL).ToList<Int32>(); // All Roads of TR / MRL are here.


                PLAN_ROAD planRoad = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == PlanCNRoadCode).FirstOrDefault();


                if (planRoad.PLAN_RD_TOTAL_LEN < 5)
                {
                    // Allow PCI to Finalize. Image is not mandatory.
                    ImageFinalized = 1;
                    return ImageFinalized;
                }
                else
                { // Border Road and Image Mandatory

                   // var ImageEntry = dbContext.MANE_PCI_IMAGE_MAPPING_PMGSY3.Where(x => x.MANE_CN_PCI_INDEX_PMGSY3.PLAN_CN_ROAD_CODE == PlanCNRoadCode && x.MANE_CN_PCI_INDEX_PMGSY3.MANE_PCI_YEAR == SelectedYear).ToList();

                    var PCICodes = dbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(x => x.PLAN_CN_ROAD_CODE == PlanCNRoadCode && x.MANE_PCI_YEAR == SelectedYear && ERCodeList.Contains(x.MAST_ER_ROAD_CODE)).Select(x => x.PCI_ID).ToList<Int32>();


                    foreach (Int32 PCI_ID in PCICodes)
                    {
                        if (dbContext.MANE_PCI_IMAGE_MAPPING_PMGSY3.Any(m => m.PCI_ID == PCI_ID))
                        {
                        }
                        else
                        {
                            ImageFinalized = 0;
                            return ImageFinalized;
                        }
                    }

                  
                }
                return ImageFinalized;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork/GetPCIForPmgsyIIIRoad");
                return 0;
            }

        }
        
        #endregion

        #region CUPL PCI Details Report

        [HttpGet]
        public ActionResult GetPCIDetailsReportLayout()
        {
            try
            {
                CUPLPCIReportModel viewmodel = new CUPLPCIReportModel();
                CommonFunctions commonFunctions = new CommonFunctions();
                //IEnumerable x = commonFunctions.PopulateFinancialYear(true, true).AsEnumerable<SelectListItem>().ToList<SelectListItem>().OrderByDescending(t => t.Text);
                viewmodel.StateList = commonFunctions.PopulateStates(true);
                viewmodel.DistrictList = new List<SelectListItem>();
                viewmodel.DistrictList.Add(new SelectListItem { Text = "Select District", Value = "0", Selected = true });

                viewmodel.BlockList = new List<SelectListItem>();
                viewmodel.BlockList.Add(new SelectListItem { Text = "Select Block", Value = "0", Selected = true });
                //var type = x.GetType();
                //viewmodel.RoadtypeList.Add(new SelectListItem { Text = "Candidate Roads", Value = "C", Selected = true });
                //viewmodel.BlockList = commonFunctions.PopulateBlocks(PMGSYSession.Current.DistrictCode, false);
                return View(viewmodel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork/GetPCIForPmgsyIIIRoad");
                return null;
            }
        }

        [HttpPost]
        public ActionResult GetPCIDetailsReportPOST(CUPLPCIReportModel viewmodel)
        {
            try
            {
                return View(viewmodel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork/GetPCIForPmgsyIIIRoad");
                return null;
            }
        }

        #endregion

        #region PCI Block & District Finalize.

        public ActionResult FinalizePCIPMGSY3Layout()
        {
            PCIFinalizationViewModel model = new PCIFinalizationViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                model.lstDistricts = comm.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                model.lstDistricts.Find(x => x.Value == "-1").Text = "Select District";

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwok.FinalizePCIPMGSY3Layout()");
                return null;
            }
        }

        [HttpGet]
        public ActionResult GetBlockListPMGSY3(int? page, int? rows, string sidx, string sord)
        {
            String searchParameters = String.Empty;
            long totalRecords;

            int districtCode = 0;
            bool isAllBlockFinalized = false;
            try
            {
                CoreNetworkDAL objDAL = new CoreNetworkDAL();
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
                ErrorLog.LogError(ex, "CoreNetwok.GetBlockListPMGSY3()");
                return null;
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult FinalizePCIBlock(String parameter, String hash, String key)
        {
            string message = string.Empty;
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                CoreNetworkDAL objDAL = new CoreNetworkDAL();
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                int blockCode = Convert.ToInt32(decryptedParameters["BlockCode"]);

                if (decryptedParameters.Count() > 0)
                {
                    bool status = objDAL.FinalizePCIBlockPMGSY3DAL(blockCode, ref message);
                    return Json(new { success = status, message = message.Trim() });
                }
                else
                {
                    return Json(new { success = false, message = "Invalid request" });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwok.FinalizeFacilityBlock()");
                return Json(new { success = false, message = "Error occurred while processing the request." });
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Audit]
        public JsonResult FinalizePCIDistrict(int districtCode)
        {
            string message = string.Empty;
            try
            {
                CoreNetworkDAL objDAL = new CoreNetworkDAL();
                bool status = objDAL.FinalizePCIDistrictPMGSY3DAL(districtCode, ref message);
                return Json(new { success = status, message = message.Trim() });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwok.FinalizeFacilityDistrict()");
                return Json(new { success = false, message = "Error occurred while processing the request." });
            }
        }


        #endregion

        #region PCI Definalization at ITNO

        [HttpGet]
        public ActionResult GetPCIForPmgsyIIIRoadITNO()
        {
            try
            {
                PCIEntryViewModel viewmodel = new PCIEntryViewModel();
                CommonFunctions commonFunctions = new CommonFunctions();
                viewmodel.FyearList = commonFunctions.PopulateFinancialYear(true, true).AsEnumerable<SelectListItem>().ToList<SelectListItem>();
                viewmodel.FyearList.RemoveAll(x => Convert.ToInt32(x.Value) < 2019 && Convert.ToInt32(x.Value) != 0);

                viewmodel.RoadtypeList = new List<SelectListItem>();
                viewmodel.RoadtypeList.Add(new SelectListItem { Text = "Candidate Roads", Value = "C", Selected = true });
                viewmodel.BlockList = commonFunctions.PopulateBlocks(PMGSYSession.Current.DistrictCode, false);
                viewmodel.DistrictList = commonFunctions.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                return View(viewmodel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork/GetPCIForPmgsyIIIRoad");
                return null;
            }
        }

        [HttpPost]
        public JsonResult GetPmgsyRoadListITNO(FormCollection formCollection)
        {
            try
            {
                if (PMGSYSession.Current == null)
                {
                    Response.Redirect("/Login/Login");
                }

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                int IMS_YEAR = Convert.ToInt32(Request.Params["IMS_YEAR"]);
                int IMS_BLOCK_ID = Convert.ToInt32(Request.Params["Block"]);

                int totalRecords = 0;
                CoreNetworkDAL objDAL = new CoreNetworkDAL();

                var jsonData = new
                {
                    rows = objDAL.GetPmgsyRoadsDAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, PMGSYSession.Current.StateCode
                    , PMGSYSession.Current.DistrictCode, PMGSYSession.Current.AdminNdCode, IMS_YEAR, IMS_BLOCK_ID),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork/GetPmgsyRoadList");
                return null;
            }
        }

        [HttpPost]
        public JsonResult GetCNRoadListITNO(FormCollection formCollection)
        {
            try
            {
                if (PMGSYSession.Current == null)
                {
                    Response.Redirect("/Login/Login");
                }

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 29-Apr-2014 end

                int IMS_BLOCK_ID = Convert.ToInt32(Request.Params["Block"]);
                int IMS_YEAR = Convert.ToInt32(Request.Params["IMSYEAR"]);
                int totalRecords = 0;
                CoreNetworkDAL objDAL = new CoreNetworkDAL();

                var jsonData = new
                {
                    rows = objDAL.GetCNRoadsDALITNO(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, PMGSYSession.Current.StateCode, Convert.ToInt32(Request.Params["DistrictCode"]), PMGSYSession.Current.AdminNdCode, IMS_BLOCK_ID, IMS_YEAR),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork/GetCNRoadList");
                return null;
            }
        }

        [HttpGet]
        public ActionResult AddPciForCNRoadITNO(String parameter, String hash, String key)
        {
            try
            {
                int PLAN_CN_ROAD_CODE = 0, ER_ROAD_CODE = 0;

                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        PLAN_CN_ROAD_CODE = Convert.ToInt32(urlSplitParams[0]);
                        ER_ROAD_CODE = Convert.ToInt32(urlSplitParams[1]);
                    }
                }
                PMGSYEntities dbContext = new PMGSYEntities();
                PLAN_ROAD plan_road = dbContext.PLAN_ROAD.Where(x => x.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE && x.MAST_ER_ROAD_CODE == ER_ROAD_CODE).FirstOrDefault();

                MASTER_EXISTING_ROADS MASTER_EXISTING_ROADS = dbContext.MASTER_EXISTING_ROADS.Find(ER_ROAD_CODE);

                PLAN_ROAD_MRL_PMGSY3 PLAN_ROAD_MRL_PMGSY3 = dbContext.PLAN_ROAD_MRL_PMGSY3.Where(x => x.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE && x.MAST_ER_ROAD_CODE == ER_ROAD_CODE).FirstOrDefault();

                CommonFunctions objCommonFunction = new CommonFunctions();

                PCIIndexViewModel pciIndexViewModel = new PCIIndexViewModel();

                //var PCIEntryForPlanroad = dbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(x => x.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE).ToList().LastOrDefault();
                var PCIEntryForPlanroad = dbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(x => x.MAST_ER_ROAD_CODE == ER_ROAD_CODE && x.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE).ToList().LastOrDefault();


                pciIndexViewModel.SURFACES = objCommonFunction.PopulateSurfaceType();
                pciIndexViewModel.ENC_PLAN_CN_ROAD_CODE = URLEncrypt.EncryptParameters(new string[] { PLAN_CN_ROAD_CODE.ToString().Trim() });
                pciIndexViewModel.ENC_ER_ROAD_CODE = URLEncrypt.EncryptParameters(new string[] { ER_ROAD_CODE.ToString().Trim() });

                pciIndexViewModel.EncERCodePlanCode = URLEncrypt.EncryptParameters(new string[] { PLAN_CN_ROAD_CODE.ToString() + "$" + ER_ROAD_CODE.ToString() });

                pciIndexViewModel.ER_ROAD_CODE = ER_ROAD_CODE;

                var MRLEntry = dbContext.PLAN_ROAD_MRL_PMGSY3.Where(x => x.MAST_ER_ROAD_CODE == ER_ROAD_CODE && x.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE).FirstOrDefault();

                if (PCIEntryForPlanroad != null)
                    pciIndexViewModel.MANE_STR_CHAIN = PCIEntryForPlanroad.MANE_END_CHAIN;

                else if (MRLEntry != null)
                {
                    pciIndexViewModel.MANE_STR_CHAIN = Convert.ToDecimal(MRLEntry.PLAN_RD_FROM_CHAINAGE);
                }

                else
                    pciIndexViewModel.MANE_STR_CHAIN = plan_road.PLAN_RD_FROM_CHAINAGE.HasValue ? plan_road.PLAN_RD_FROM_CHAINAGE.Value : 0;

                //pciIndexViewModel.RoadName = plan_road.PLAN_RD_NAME;
                pciIndexViewModel.RoadName = MASTER_EXISTING_ROADS.MAST_ER_ROAD_NAME;

                if (plan_road != null)
                    pciIndexViewModel.erRoadName = plan_road.MASTER_EXISTING_ROADS.MAST_ER_ROAD_NAME.Trim();
                //pciIndexViewModel.RoadLength = plan_road.PLAN_RD_TOTAL_LEN.HasValue ? plan_road.PLAN_RD_TOTAL_LEN.Value : (plan_road.PLAN_RD_LENGTH == null ? 0 : Convert.ToDecimal(plan_road.PLAN_RD_LENGTH));

                if (PLAN_ROAD_MRL_PMGSY3 != null)
                    pciIndexViewModel.RoadLength = Convert.ToDecimal(PLAN_ROAD_MRL_PMGSY3.PLAN_RD_LENGTH);

                else
                    pciIndexViewModel.RoadLength = Convert.ToDecimal(plan_road.PLAN_RD_LENGTH);


                if (DateTime.Now.Month <= 3)
                {
                    List<SelectListItem> lstYears = objCommonFunction.PopulateUpToYear(true, false);
                    lstYears.RemoveAt(1);
                    pciIndexViewModel.YEARS = lstYears;

                }
                else
                {
                    pciIndexViewModel.YEARS = objCommonFunction.PopulateUpToYear(true, false);
                }

                pciIndexViewModel.YEARS.RemoveAll(x => Convert.ToInt32(x.Value) < 2019 && Convert.ToInt32(x.Value) != 0);

                #region To check wheather DRRP is finalized

                pciIndexViewModel.isfinalizedEntry = false;
                pciIndexViewModel.isLengthComplete = false;
                var Entries = dbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(x => x.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE && x.MAST_ER_ROAD_CODE == ER_ROAD_CODE).Select(y => y.IS_FINALIZED).ToList();
                foreach (var item in Entries)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        pciIndexViewModel.isfinalizedEntry = item.Equals("Y");
                        if (pciIndexViewModel.isfinalizedEntry)
                            break;
                    }

                }

                #endregion

                #region To check full length entry has been done
                var StartPCIEntry = dbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(x => x.MAST_ER_ROAD_CODE == ER_ROAD_CODE && x.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE).ToList().FirstOrDefault();
                var EndPCIEntry = dbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(x => x.MAST_ER_ROAD_CODE == ER_ROAD_CODE && x.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE).ToList().LastOrDefault();
                if (StartPCIEntry != null && EndPCIEntry != null)
                {
                    var Difference = EndPCIEntry.MANE_END_CHAIN - StartPCIEntry.MANE_STR_CHAIN;
                    if (Difference == pciIndexViewModel.RoadLength)
                    {
                        pciIndexViewModel.isLengthComplete = true;
                    }
                }
                #endregion

                return View(pciIndexViewModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork/AddPciForCNRoad");
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SavePciForCNRoadITNO(PCIIndexViewModel pciIndexViewModel)
        {
            try
            {
                PMGSYEntities dbContext = new PMGSYEntities();
                if (ModelState.IsValid)
                {
                    int PLAN_CN_ROAD_CODE = 0;
                    string[] EncKey = pciIndexViewModel.ENC_PLAN_CN_ROAD_CODE.Split('/');

                    if (EncKey.Length == 3)
                    {
                        if (!String.IsNullOrEmpty(EncKey[0]) && !String.IsNullOrEmpty(EncKey[1]) && !String.IsNullOrEmpty(EncKey[2]))
                        {
                            String[] urlParams = URLEncrypt.DecryptParameters(new String[] { EncKey[0], EncKey[1], EncKey[2] });
                            if (urlParams.Length >= 1)
                            {
                                String[] urlSplitParams = urlParams[0].Split('$');
                                PLAN_CN_ROAD_CODE = Convert.ToInt32(urlSplitParams[0]);
                            }
                        }
                    }
                    else
                    {
                        return Json(new { Success = false, ErrorMessage = "Error Occured while processing your Request." });
                    }
                    var ChainageDiffrence = pciIndexViewModel.MANE_END_CHAIN - pciIndexViewModel.MANE_STR_CHAIN;
                    if (ChainageDiffrence > 1 && ChainageDiffrence != -1)
                    {
                        ModelState.AddModelError("MANE_END_CHAIN", "The difference between end chainage and start chainage should be 1.");
                        return Json(new { Success = false, ErrorMessage = "The difference between end chainage and start chainage should be 1." });
                    }


                    #region To check full length entry has been done

                    var StartPCIEntry = dbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(x => x.MAST_ER_ROAD_CODE == pciIndexViewModel.ER_ROAD_CODE && x.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE).ToList().FirstOrDefault();
                    var EndPCIEntry = dbContext.MANE_CN_PCI_INDEX_PMGSY3.Where(x => x.MAST_ER_ROAD_CODE == pciIndexViewModel.ER_ROAD_CODE && x.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE).ToList().LastOrDefault();

                    if (StartPCIEntry != null && EndPCIEntry != null)
                    {
                        var EntryDiff = EndPCIEntry.MANE_END_CHAIN - StartPCIEntry.MANE_STR_CHAIN;
                        var ModelDiff = pciIndexViewModel.MANE_END_CHAIN - pciIndexViewModel.MANE_STR_CHAIN;
                        var sum = EntryDiff + ModelDiff;
                        if (sum > pciIndexViewModel.RoadLength)
                        {
                            return Json(new { Success = false, ErrorMessage = "PCI Entry can not be greater than the Road Length." });
                        }
                    }

                    #endregion

                    pciIndexViewModel.PLAN_CN_ROAD_CODE = PLAN_CN_ROAD_CODE;

                    CoreNetworkDAL objDAL = new CoreNetworkDAL();
                    string status = objDAL.SavePciForCNRoadDAL(pciIndexViewModel);

                    if (status == string.Empty)
                    {
                        return Json(new { Success = true });
                    }
                    else
                    {
                        return Json(new { Success = false, ErrorMessage = status });
                    }
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork/SavePciForCNRoad");
                return null;
            }
        }

        [HttpPost]
        public JsonResult GetPCIListForCNRoadITNO(FormCollection formCollection)
        {
            try
            {
                if (PMGSYSession.Current == null)
                {
                    Response.Redirect("/Login/Login");
                }

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                string urlparam = Request.Params["PLAN_CN_ROAD_CODE"].Split('$')[0];
                int ER_ROAD_CODE = Convert.ToInt32(Request.Params["PLAN_CN_ROAD_CODE"].Split('$')[1]);
                string[] EncKey = urlparam.Split('/');
                int PLAN_CN_ROAD_CODE = 0;

                if (EncKey.Length == 3)
                {
                    if (!String.IsNullOrEmpty(EncKey[0]) && !String.IsNullOrEmpty(EncKey[1]) && !String.IsNullOrEmpty(EncKey[2]))
                    {
                        String[] urlParams = URLEncrypt.DecryptParameters(new String[] { EncKey[0], EncKey[1], EncKey[2] });
                        if (urlParams.Length >= 1)
                        {
                            String[] urlSplitParams = urlParams[0].Split('$');
                            PLAN_CN_ROAD_CODE = Convert.ToInt32(urlSplitParams[0]);
                        }
                    }
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = "Error Occured while processing your Request." });
                }

                int totalRecords = 0;
                CoreNetworkDAL objDAL = new CoreNetworkDAL();
                var jsonData = new
                {
                    rows = objDAL.GetPCIListForCNRoadDALITNO(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, PLAN_CN_ROAD_CODE, ER_ROAD_CODE),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork/GetPCIListForCNRoad");
                return null;
            }

        }

        [HttpPost]
        public ActionResult GetPhotoUploadViewITNO(string parameter, string hash, string key)
        {
            try
            {
                PMGSYEntities dbcontext = new PMGSYEntities();
                PCIPhotoViewModel viewmodel = new PCIPhotoViewModel();
                Dictionary<string, string> decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                int PCIid = Convert.ToInt32(decryptedParameters["PCIID"].ToString());
                var PCIEntryDetails = dbcontext.MANE_CN_PCI_INDEX_PMGSY3.Where(x => x.PCI_ID == PCIid).FirstOrDefault();
                var PhotoGraphEntryCount = dbcontext.MANE_PCI_IMAGE_MAPPING_PMGSY3.Where(x => x.PCI_ID == PCIid).Count();

                #region if entry is finalized

                bool isFinalized = false;
                //var EntryToCheck = dbcontext.MANE_CN_PCI_INDEX_PMGSY3.Where(x => x.PCI_ID == PCIid).FirstOrDefault();
                if (PCIEntryDetails != null)
                {
                    if (PCIEntryDetails.IS_FINALIZED != null)
                        isFinalized = PCIEntryDetails.IS_FINALIZED.Equals("Y");
                }

                #endregion

                viewmodel.PCIid = parameter + "/" + hash + "/" + key;
                viewmodel.PLAN_CN_ROAD_CODE = PCIEntryDetails.PLAN_CN_ROAD_CODE;
                if (isFinalized)
                {
                    viewmodel.NumberofFiles = 2;
                }
                else
                {
                    viewmodel.NumberofFiles = PhotoGraphEntryCount;
                }

                return View(viewmodel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork/GetPhotoUploadView");
                return null;
            }
        }

        public ActionResult SavePhotographITNO(PCIPhotoViewModel formmodel)
        {
            try
            {
                CommonFunctions objCommonFunc = new CommonFunctions();

                String parameter = formmodel.PCIid.Split('/')[0];
                String hash = formmodel.PCIid.Split('/')[1];
                String key = formmodel.PCIid.Split('/')[2];

                Dictionary<string, string> decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                int pciid = Convert.ToInt32(decryptedParameters["PCIID"].ToString());

                foreach (string file in Request.Files)
                {
                    string status = ValidatePhoto(Request.Files[0].ContentLength, Path.GetExtension(Request.Files[0].FileName));
                    if (status != string.Empty)
                    {
                        formmodel.ErrorMessage = status;
                        return Json(new { message = "Photograph format is not valid", success = false });
                    }
                }
                bool isFileSaved = false;
                HttpPostedFileBase FileBase = null;
                CoreNetworkDAL objDAL = new CoreNetworkDAL();

                for (int i = 0; i < Request.Files.Count; i++)
                {
                    FileBase = Request.Files[i];
                    var filename = FileBase.FileName;

                    //var remark = Request.Params["InspPdfDescription[]"];
                    //if (remark == null || string.IsNullOrEmpty(remark) || string.IsNullOrWhiteSpace(remark))
                    //{
                    //    return Json(new { success = isFileSaved, message = "Please Enter Remark" }, JsonRequestBehavior.DenyGet);
                    //}
                    string remark = Request.Params["InspPdfDescription[]"];
                    Regex regex = new Regex(@"^[a-zA-Z0-9 ,.()-]+$");
                    if (!regex.IsMatch(remark))
                    {
                        return Json(new { success = isFileSaved, message = "Only alphabets and numbers allowed in remark" }, JsonRequestBehavior.DenyGet);
                    }

                    isFileSaved = objDAL.SavePhotoGraphDAL(pciid, filename, FileBase, Request.Params["InspPdfDescription[]"]);
                }

                formmodel.NumberofImages = 0;

                var serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;

                if (isFileSaved)
                    return Json(new { success = isFileSaved, message = "Photograph uploaded successfully" }, JsonRequestBehavior.DenyGet);
                else
                    return Json(new { success = isFileSaved, message = "Photograph not uploaded" }, JsonRequestBehavior.DenyGet);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork/SavePhotograph");
                return null;
            }
        }

        public string ValidatePhotoITNO(int FileSize, string FileExtension)
        {
            try
            {
                if (!(FileExtension.ToUpper().Equals(".JPEG") || FileExtension.ToUpper().Equals(".JPG")))
                {
                    return "Photograph not in correct format";
                }
                if (FileSize > Convert.ToInt32(ConfigurationManager.AppSettings["PROPOSAL_PDF_FILE_MAX_SIZE"]))
                {
                    return "File Size Exceed the Maximum File Limit";
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork/ValidatePhoto");
                return null;
            }
        }


        [HttpPost]
        public JsonResult ListImageFilesITNO(FormCollection formCollection)
        {

            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                var param = Request.Params["PCIIDParam"];
                String parameter = param.Split('/')[0];
                String hash = param.Split('/')[1];
                String key = param.Split('/')[2];

                Dictionary<string, string> decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                int pciid = Convert.ToInt32(decryptedParameters["PCIID"].ToString());


                int totalRecords;
                var jsonData = new
                {
                    rows = objDAL.GetImageFilesListDALITNO(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, pciid),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork/ListImageFiles");
                return null;
            }
        }

        [HttpPost]
        public JsonResult UpdateImageRemarkDetailsITNO(FormCollection formCollection)
        {
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            try
            {
                string arrKey = formCollection["id"];


                Regex regex = new Regex(@"^[a-zA-Z0-9 ,.()-]+$");
                if (regex.IsMatch(formCollection["Description"]) && formCollection["Description"].Trim().Length != 0)
                {
                    //fileuploadViewModel.PdfDescription = formCollection["Description"];
                }
                else
                {
                    return Json("Invalid PDF Description, Only Alphabets,Numbers and [,.()-] are allowed");
                }

                //string status = qualityBAL.UpdateMultipleInspPDFDetailsBAL(fileuploadViewModel);

                //if (status == string.Empty)
                //  return Json(true);
                //else
                return Json("There is an error occurred while processing your request.");
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork/UpdateImageRemarkDetails");
                return Json("There is an error occurred while processing your request.");
            }
        }

        [HttpPost]
        public JsonResult DeleteMultipleInspFileDetailsITNO(string id)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            try
            {

                var param = Request.Params["Fileid"];
                String parameter = param.Split('/')[0];
                String hash = param.Split('/')[1];
                String key = param.Split('/')[2];

                Dictionary<string, string> decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                int fileid = Convert.ToInt32(decryptedParameters["FileID"].ToString());

                var EntryDetails = dbContext.MANE_PCI_IMAGE_MAPPING_PMGSY3.Where(x => x.FILE_ID == fileid).FirstOrDefault();
                int PCIid = EntryDetails.PCI_ID;
                string Filename = EntryDetails.FILE_NAME;
                bool isEntryDeleted = false;
                using (TransactionScope ts = new TransactionScope())
                {
                    if (EntryDetails != null)
                    {
                        dbContext.MANE_PCI_IMAGE_MAPPING_PMGSY3.Remove(EntryDetails);
                        dbContext.SaveChanges();
                        ts.Complete();
                        isEntryDeleted = true;
                    }
                }
                var count = dbContext.MANE_PCI_IMAGE_MAPPING_PMGSY3.Where(x => x.PCI_ID == PCIid).Count();

                if (isEntryDeleted)
                {
                    if (System.IO.File.Exists(Path.Combine(ConfigurationManager.AppSettings["PCI_INDEX_CHAINAGE_PHOTO"], Filename)))
                    {
                        System.IO.File.Delete(Path.Combine(ConfigurationManager.AppSettings["PCI_INDEX_CHAINAGE_PHOTO"], Filename));
                        return Json(new { Success = true, Message = "Photograph deleted successfully.", photocount = count });
                    }
                }
                return Json(new { Success = false, ErrorMessage = "There is an error while processing your request." });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork/DeleteMultipleInspFileDetails");

                return Json(new { Success = false, ErrorMessage = "There is an error while processing your request." });
            }
        }

        [HttpPost]
        public JsonResult DeletePciForCNRoadITNO()
        {
            PMGSYEntities dbcontext = new PMGSYEntities();
            try
            {
                string Data = string.Empty;
                string[] EncKey = Request.Params["Data"].Split('/');
                String[] urlSplitParams = { };
                if (EncKey.Length == 3)
                {
                    if (!String.IsNullOrEmpty(EncKey[0]) && !String.IsNullOrEmpty(EncKey[1]) && !String.IsNullOrEmpty(EncKey[2]))
                    {
                        String[] urlParams = URLEncrypt.DecryptParameters(new String[] { EncKey[0], EncKey[1], EncKey[2] });
                        if (urlParams.Length >= 1)
                        {
                            urlSplitParams = urlParams[0].Split('$');
                        }
                    }
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = "Error Occured while processing your Request." });
                }

                var param = Request.Params["Data"];
                String parameter = param.Split('/')[0];
                String hash = param.Split('/')[1];
                String key = param.Split('/')[2];
                Dictionary<string, string> decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                int PCIid = Convert.ToInt32(decryptedParameters["PCIID"].ToString());

                CoreNetworkDAL objDAL = new CoreNetworkDAL();

                var Entry = dbcontext.MANE_CN_PCI_INDEX_PMGSY3.Where(x => x.PCI_ID == PCIid).FirstOrDefault();
                var EntryAgainstPhoto = dbcontext.MANE_PCI_IMAGE_MAPPING_PMGSY3.Where(x => x.PCI_ID == Entry.PCI_ID).FirstOrDefault();

                if (Entry != null)
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        if (EntryAgainstPhoto != null)
                            dbcontext.MANE_PCI_IMAGE_MAPPING_PMGSY3.Remove(EntryAgainstPhoto);

                        dbcontext.MANE_CN_PCI_INDEX_PMGSY3.Remove(Entry);

                        dbcontext.SaveChanges();

                        ts.Complete();
                        return Json(new { Success = true });
                    }
                }
                return Json(new { Success = false });

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork/DeletePciForCNRoad");
                return null;
            }
        }

        [HttpPost]
        public ActionResult DeFinalizePCIRoadDetailsITNO()
        {
            try
            {
                PMGSYEntities dbcontext = new PMGSYEntities();
                var PLAN_ROAD_CODE = Convert.ToInt32(Request.Params["Data"]);

                var MANE_CN_PCI_INDEX_PMGSY3 = dbcontext.MANE_CN_PCI_INDEX_PMGSY3.Where(x => x.PLAN_CN_ROAD_CODE == PLAN_ROAD_CODE).ToList();
                List<MANE_CN_PCI_INDEX_PMGSY3> EntryList = new List<MANE_CN_PCI_INDEX_PMGSY3>();

                #region Check Block Finalization
                var planRoad = dbcontext.PLAN_ROAD.Where(z => z.PLAN_CN_ROAD_CODE == PLAN_ROAD_CODE).FirstOrDefault();
                if (dbcontext.MAST_PCI_DISTRICT_PMGSY3_FINALIZE.Where(x => x.MAST_DISTRICT_CODE == planRoad.MAST_DISTRICT_CODE).Any())
                {
                    return Json(new { success = true, ErrorMessage = "PCI District is finalized, cannot definalize PCI Entry for individual roads." }, JsonRequestBehavior.DenyGet);
                }
                if (dbcontext.MAST_PCI_BLOCK_PMGSY3_FINALIZE.Where(x => x.MAST_BLOCK_CODE == planRoad.MAST_BLOCK_CODE).Any())
                {
                    return Json(new { success = true, ErrorMessage = "PCI Block is finalized, cannot definalize PCI Entry for individual roads." }, JsonRequestBehavior.DenyGet);
                }
                #endregion

                if (PLAN_ROAD_CODE != 0)
                {
                    foreach (var item in MANE_CN_PCI_INDEX_PMGSY3)
                    {
                        if (!string.IsNullOrEmpty(item.IS_FINALIZED))
                        {
                            if (item.IS_FINALIZED.Equals("Y"))
                            {
                                item.IS_FINALIZED = null;
                                item.FINALIZED_DATE = null;
                                EntryList.Add(item);
                            }
                            else
                            {
                                return Json(new { success = true, ErrorMessage = "PCI Entry against the road has not been finalized yet." }, JsonRequestBehavior.DenyGet);
                            }
                        }

                        else
                        {
                            return Json(new { success = true, ErrorMessage = "PCI Entry against the road has not been finalized yet." }, JsonRequestBehavior.DenyGet);
                        }
                    }
                    using (TransactionScope ts = new TransactionScope())
                    {
                        foreach (var itemToUpdate in EntryList)
                        {
                            dbcontext.Entry(itemToUpdate).State = System.Data.Entity.EntityState.Modified;
                        }
                        dbcontext.SaveChanges();
                        ts.Complete();
                        return Json(new { success = true, ErrorMessage = "PCI Entry against the road has been Definalized." }, JsonRequestBehavior.DenyGet);
                    }

                }
                else
                {
                    return Json(new { success = false, ErrorMessage = "PCI Entry against the road could not be Definalized." }, JsonRequestBehavior.DenyGet);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork/FinalizePCIRoadDetails");
                return null;
            }
        }

        [HttpGet]
        public ActionResult PopulateBlockList()
        {
            try
            {
                CommonFunctions comm = new CommonFunctions();
                var BlockList = comm.PopulateBlocks(Convert.ToInt32(Request.Params["DistrictCode"]));
                BlockList.RemoveAll(x => x.Value == "0");
                BlockList.Insert(0, new SelectListItem { Text = "All District", Value = "0", Selected = true });
                return Json(BlockList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoads/PopulateBlockList");
                return null;
            }
        }

        #endregion

        #region CUPL PCI  New Report

        [HttpGet]
        public ActionResult GetPCIDetailsReportNewLayout()
        {
            try
            {
                CUPLPCIReportModel viewmodel = new CUPLPCIReportModel();
                CommonFunctions commonFunctions = new CommonFunctions();
                //IEnumerable x = commonFunctions.PopulateFinancialYear(true, true).AsEnumerable<SelectListItem>().ToList<SelectListItem>().OrderByDescending(t => t.Text);
                viewmodel.StateList = commonFunctions.PopulateStates(true);
                viewmodel.StateCode = PMGSYSession.Current.StateCode;
                viewmodel.statename = PMGSYSession.Current.StateName;

                if (PMGSYSession.Current.DistrictCode > 0)
                {
                    viewmodel.DistrictList = new List<SelectListItem>();
                    viewmodel.DistrictList.Add(new SelectListItem { Text = "Select District", Value = "0", Selected = true });
                    viewmodel.DistrictList.Add(new SelectListItem { Text = PMGSYSession.Current.DistrictName.Trim(), Value = Convert.ToString(PMGSYSession.Current.DistrictCode) });
                }
                else
                {
                    viewmodel.DistrictList = commonFunctions.PopulateDistrict(PMGSYSession.Current.StateCode);
                }

                viewmodel.BlockList = new List<SelectListItem>();
                viewmodel.BlockList.Add(new SelectListItem { Text = "Select Block", Value = "0", Selected = true });

                return View(viewmodel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork/GetPCIDetailsReportNewLayout");
                return null;
            }
        }

        //[HttpPost]
        //public ActionResult GetPCIDetailsReportNewPOST(CUPLPCIReportModel viewmodel)
        //{
        //    PMGSYEntities dbContext = new PMGSYEntities();
        //    try
        //    {
        //        short roleCode = dbContext.UM_User_Master.Where(m => m.UserID == PMGSYSession.Current.UserId).Select(m => m.DefaultRoleID).FirstOrDefault();
        //        ViewData["TRMRLUnlocked"] = dbContext.UDF_IMS_UNLOCK_STATUS(PMGSYSession.Current.StateCode, PMGSYSession.Current.DistrictCode, viewmodel.BlockID, 0, 0, 0, 0, 0, "CN", PMGSYSession.Current.PMGSYScheme, roleCode).Select(c => c.UNLOCK_COUNT).FirstOrDefault();

        //        return View(viewmodel);
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "CoreNetwork/GetPCIDetailsReportNewPOST");
        //        return null;
        //    }
        //}
        [HttpPost]
        public ActionResult GetPCIDetailsReportNewPOST(CUPLPCIReportModel viewmodel)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                short roleCode = dbContext.UM_User_Master.Where(m => m.UserID == PMGSYSession.Current.UserId).Select(m => m.DefaultRoleID).FirstOrDefault();
                ViewData["TRMRLUnlocked"] = dbContext.UDF_IMS_UNLOCK_STATUS(PMGSYSession.Current.StateCode, PMGSYSession.Current.DistrictCode, viewmodel.BlockID, 0, 0, 0, 0, 0, "CN", PMGSYSession.Current.PMGSYScheme, roleCode).Select(c => c.UNLOCK_COUNT).FirstOrDefault();

                ViewData["IsHabitationUnlocked"] = checkIsUnlocked(PMGSYSession.Current.StateCode, PMGSYSession.Current.DistrictCode, viewmodel.BlockID);


                return View(viewmodel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork/GetPCIDetailsReportNewPOST");
                return null;
            }
        }


        public ActionResult BlockDetailsNew(FormCollection frmCollection)
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            List<SelectListItem> list = objCommonFunctions.PopulateBlocksNew(Convert.ToInt32(frmCollection["StateCode"]), Convert.ToInt32(frmCollection["DistrictCode"]), true);
            list.Find(x => x.Value == "-1").Value = "0";
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Definalize TR/MRL at ITNO

        [HttpGet]
        public ActionResult ListCoreNetWorksPMGSY3ITNO()
        {
            List<MASTER_BLOCK> lst = new List<MASTER_BLOCK>();
            int block = 0;
            CommonFunctions objCommon = new CommonFunctions();
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            CNPMGSY3ViewModel model = new CNPMGSY3ViewModel();
            try
            {

                model.DistrictCode = PMGSYSession.Current.DistrictCode;

                model.PmgsyScheme = PMGSYSession.Current.PMGSYScheme;
                model.lstRouteType = objDAL.GetAllRoutes().ToList();

                model.lstRouteType.Remove(model.lstRouteType.Find(c => c.Value.Trim() == "L"));

                model.lstCategory = objDAL.GetCategoryForSearch().ToList();
                model.lstCategory.Insert(0, new SelectListItem() { Value = "0", Text = "-- All --" });

                List<SelectListItem> lstDefault = new List<SelectListItem>();
                lstDefault.Insert(0, new SelectListItem { Value = "0", Text = "All" });
                if (PMGSYSession.Current.RoleCode == 25 || PMGSY.Extensions.PMGSYSession.Current.RoleCode == 53 || PMGSYSession.Current.RoleCode == 65 || PMGSYSession.Current.RoleCode == 36)//Changes by SAMMED A. PATIL for mordviewuser
                {
                    model.lstStateCode = objCommon.PopulateStates(true);
                    model.lstDistrictCode = objCommon.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                    model.lstBlockCode = lstDefault;
                    model.lstBlockCode.Insert(0, new SelectListItem { Text = "Select Block", Value = "0", Selected = true });
                }
                else if (PMGSYSession.Current.RoleCode == 2)
                {
                    model.lstDistrictCode = objCommon.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                    model.lstBlockCode = lstDefault;
                }
                else
                {
                    model.lstBlockCode = new SelectList(objDAL.GetBlocksByDistrictCode(model.DistrictCode), "MAST_BLOCK_CODE", "MAST_BLOCK_NAME").ToList();
                    lst = objDAL.GetBlocksByDistrictCode(model.DistrictCode);

                    if (lst.Count > 0)
                    {
                        PMGSY.DAL.ExistingRoads.IExistingRoadsDAL objDRRPDAL = new PMGSY.DAL.ExistingRoads.ExistingRoadsDAL();
                        ViewData["isUnlocked"] = objDRRPDAL.CheckUnlockedDAL(lst.Select(x => x.MAST_BLOCK_CODE).First());
                    }
                }
                ViewData["IsLocked"] = objDAL.checkIsLocked(block);
                model.isPMGSY3Finalized = objDAL.CheckPMGSY3FinalizedDAL(model.DistrictCode);
                //model.isPMGSY3Finalized = objDAL.CheckPMGSY3FinalizedDAL(lst.Select(x => x.MAST_BLOCK_CODE).First());
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.ListCoreNetWorksPMGSY3");
                return View();
            }
        }

        [HttpGet]
        public ActionResult DetailsCoreNetworkPMGSY3ITNO(String parameter, String hash, String key)
        {
            Dictionary<string, string> parameters = null;
            CoreNetworkViewModel model = new CoreNetworkViewModel();
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            try
            {
                parameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (parameters != null)
                {
                    model = objBAL.GetCoreNetworkDetails(Convert.ToInt32(parameters["NetworkCode"]));
                    model.ROAD_CODE = db.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == model.MAST_ER_ROAD_CODE).Select(r => r.MAST_ER_ROAD_NUMBER).FirstOrDefault();
                    return PartialView("DetailsCoreNetworkPMGSY3", model);
                }
                else
                {
                    return PartialView("DetailsCoreNetworkPMGSY3", new CoreNetworkViewModel());
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ListCoreNetWorksPMGSY3()");
                return Json(new { success = false, message = "Error occurred while processing the request." });
            }
            finally
            {
                db.Dispose();
            }
        }

        [HttpGet]
        [Audit]
        public ActionResult GetCoreNetWorksListPMGSY3ITNO(int? page, int? rows, string sidx, string sord)
        {
            int blockCode = 0;
            int districtCode = 0;
            int stateCode = 0;
            int categoryCode = 0;
            long totalRecords = 0;
            string roadType = string.Empty;
            String searchParameters = String.Empty;
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string roadName = string.Empty;
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
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

                if (!string.IsNullOrEmpty(Request.Params["blockCode"]))
                {
                    blockCode = Convert.ToInt32(Request.Params["blockCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["categoryCode"]))
                {
                    categoryCode = Convert.ToInt32(Request.Params["categoryCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["roadType"]))
                {
                    roadType = Request.Params["roadType"];
                }

                if (!string.IsNullOrEmpty(Request.Params["PLAN_RD_NAME"]))
                {
                    roadName = Request.Params["PLAN_RD_NAME"];
                }
                stateCode = PMGSYSession.Current.StateCode;
                var jsonData = new
                {
                    rows = objDAL.GetCoreNetWorksListPMGSY3DALITNO(stateCode, districtCode, blockCode, roadType, categoryCode, roadName, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.GetCoreNetWorksListPMGSY3()");
                return null;
            }
        }

        [Audit]
        public ActionResult CoreNetworksPMGSY3LayoutITNO(/*int blockCode*/ string id)
        {
            CoreNetworkViewModelPMGSY3 model = new CoreNetworkViewModelPMGSY3();
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            bool flag = false;
            int stateCode = 0, districtCode = 0, blockCode = 0;

            string[] locationcodes = id.Split('$');
            try
            {
                stateCode = locationcodes[0] == "undefined" ? 0 : Convert.ToInt32(locationcodes[0]);
                districtCode = locationcodes[1] == "undefined" ? 0 : Convert.ToInt32(locationcodes[1]);

                model.MAST_STATE_CODE = stateCode > 0 ? stateCode : PMGSYSession.Current.StateCode;
                model.MAST_DISTRICT_CODE = stateCode > 0 ? districtCode : PMGSYSession.Current.DistrictCode;
                blockCode = Convert.ToInt32(locationcodes[2]);
                model.BLOCK_NAME = objDAL.GetBlockName(blockCode);
                model.MAST_BLOCK_CODE = blockCode;
                List<SelectListItem> lstRoadNumber = new List<SelectListItem>();
                lstRoadNumber.Add(new SelectListItem { Value = "A", Text = "--Select Road--", Selected = true });
                //ViewData["RoadFrom"] = model.Road;
                model.lstRoadFrom = new SelectList(lstRoadNumber, "Value", "Text").ToList();
                model.lstRoadTo = new SelectList(lstRoadNumber, "Value", "Text").ToList();

                model.lstRoadNumFrom = model.RoadNumFromList.ToList();
                model.lstRoadNumTo = model.RoadNumToList.ToList();

                model.lstCategory = model.RoadCategory.ToList();
                model.lstCategory.Remove(model.lstCategory.Find(x => x.Text.Equals("X")));
                model.lstCategory.Remove(model.lstCategory.Find(x => x.Text.Equals("Major District Road")));
                model.lstCategory.Remove(model.lstCategory.Find(x => x.Text.Equals("National Highway")));
                model.lstCategory.Remove(model.lstCategory.Find(x => x.Text.Equals("State Highway")));

                model.lstPreviousBlock = new SelectList(objDAL.GetBlocksByDistrictCode(PMGSYSession.Current.DistrictCode, blockCode), "MAST_BLOCK_CODE", "MAST_BLOCK_NAME").ToList();
                model.lstPreviousBlockRoadNo = new SelectList(lstRoadNumber, "Value", "Text").ToList();
                model.lstRoadNumber = new SelectList(lstRoadNumber, "Value", "Text").ToList();
                model.lstHabitationFrom = new SelectList(objDAL.GetHabitationsByBlockCode(blockCode), "Value", "Text").ToList();
                model.lstHabitationTo = new SelectList(objDAL.GetHabitationsByBlockCode(blockCode), "Value", "Text").ToList();
                model.lstNextBlock = new SelectList(objDAL.GetBlocksByDistrictCode(PMGSYSession.Current.DistrictCode, blockCode), "MAST_BLOCK_CODE", "MAST_BLOCK_NAME").ToList();
                model.lstNextBlockRoadNo = new SelectList(lstRoadNumber, "Value", "Text").ToList();


                List<SelectListItem> lstRoute = new List<SelectListItem>();
                lstRoute.Add(new SelectListItem { Value = "0", Text = "--Select Route--", Selected = true });
                lstRoute.Add(new SelectListItem { Value = "T", Text = "Through Route" });

                ///Changes by SAMMED A. PATIL on 30AUG2017 to allow Link Route in PMGSY2
                //lstRoute.Add(new SelectListItem { Value = "L", Text = "Link Route" });
                lstRoute.Add(new SelectListItem { Value = "M", Text = "Major Rural Link" });

                //return new SelectList(lstRoute,"Value","Text");
                //if (PMGSYSession.Current.PMGSYScheme == 1)
                //{
                //    flag = checkSchedule5(blockCode);
                //    if (flag)
                //    {
                //        lstRoute.Add(new SelectListItem { Value = "N", Text = "Missing Link" });
                //    }
                //}
                model.RouteType = lstRoute;
                return PartialView(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.CoreNetworksPMGSY3Layout()");
                return null;
            }
        }

        [HttpPost]
        [Audit]
        public JsonResult GetRoadNameByRoadCodePMGSY3ITNO(string roadName, string blockName)
        {
            try
            {
                int roadCode = Convert.ToInt32(roadName);
                //int blockCode = PMGSYSession.Current.blockCode;
                int blockCode = Convert.ToInt32(blockName);
                CoreNetworkDAL objDAL = new CoreNetworkDAL();
                List<SelectListItem> lstRoads = objDAL.GetRoadNamesByRoadCodePMGSY3DAL(roadCode, blockCode);
                lstRoads.Insert(0, new SelectListItem { Value = "0", Text = "-Select Road Name-" });
                return Json(lstRoads, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.GetRoadNameByRoadCodePMGSY3()");
                return null;
            }

        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Audit]
        public ActionResult AddCoreNetworksPMGSY3ITNO(CoreNetworkViewModelPMGSY3 model)
        {
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            bool status = false;
            try
            {
                if (ModelState.IsValid)
                {
                    model.MAST_DISTRICT_CODE = model.MAST_DISTRICT_CODE > 0 ? model.MAST_DISTRICT_CODE : PMGSYSession.Current.DistrictCode;
                    model.MAST_STATE_CODE = model.MAST_STATE_CODE > 0 ? model.MAST_STATE_CODE : PMGSYSession.Current.StateCode;

                    if (objDAL.AddCoreNetworksPMGSY3DAL(model, ref message))
                    {
                        message = message == string.Empty ? "TR/MRL road details added successfully." : message;

                        status = true;
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        message = message == string.Empty ? "Error occurred while processing the request." : message;
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.AddCoreNetworksPMGSY3()");
                return Json(new { success = false, message = "Error occurred while processing the request." });
            }
        }

        [HttpGet]
        public ActionResult EditCoreNetworksPMGSY3ITNO(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptParameters = null;
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            List<SelectListItem> lstRoadNumber = new List<SelectListItem>();
            bool flag = false;
            try
            {
                decryptParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptParameters.Count() > 0)
                {
                    CoreNetworkViewModelPMGSY3 model = objDAL.GetCoreNetworkDetailsPMGSY3(Convert.ToInt32(decryptParameters["NetworkCode"]));

                    if (decryptParameters.Count > 1)
                    {
                        model.LockUnlockFlag = decryptParameters["UnlockFlag"];
                    }

                    List<SelectListItem> lstRoute = new List<SelectListItem>();
                    lstRoute.Add(new SelectListItem { Value = "0", Text = "--Select Route--", Selected = true });
                    lstRoute.Add(new SelectListItem { Value = "T", Text = "Through Route" });

                    ///Changes by SAMMED A. PATIL on 30AUG2017 to allow Link Route in PMGSY2
                    ///changes made by abhinav pathak on 31oct2019
                    if (!(PMGSYSession.Current.PMGSYScheme == 4))
                        lstRoute.Add(new SelectListItem { Value = "L", Text = "Link Route" });

                    lstRoute.Add(new SelectListItem { Value = "M", Text = "Major Rural Link" });

                    model.RouteType = lstRoute;

                    model.FROM_TYPE = model.PLAN_RD_FROM_TYPE;
                    model.TO_TYPE = model.PLAN_RD_TO_TYPE;
                    model.RD_FROM = model.PLAN_RD_FROM;
                    model.RD_TO = model.PLAN_RD_TO;
                    model.ROAD_CODE = db.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == model.MAST_ER_ROAD_CODE).Select(r => r.MAST_ER_ROAD_NUMBER).FirstOrDefault();
                    model.BLOCK_NAME = objDAL.GetBlockName(model.MAST_BLOCK_CODE);//db.MASTER_BLOCK.Where(m => m.MAST_BLOCK_CODE == model.MAST_BLOCK_CODE).Select(b => b.MAST_BLOCK_NAME).FirstOrDefault();
                    if (model.PLAN_RD_NUM_FROM == null)
                    {
                        model.NUM_FROM = null;
                    }
                    else
                    {
                        model.NUM_FROM = model.PLAN_RD_NUM_FROM;
                    }

                    if (model.PLAN_RD_NUM_TO == null)
                    {
                        model.NUM_TO = null;
                    }
                    else
                    {
                        model.NUM_TO = model.PLAN_RD_NUM_TO;
                    }

                    if (model == null)
                    {
                        ModelState.AddModelError(String.Empty, "Core network details not exist.");
                        return PartialView("CoreNetworksPmgsy3Layout", new CoreNetworkViewModelPMGSY3());
                    }

                    model.lstRoadFrom = model.Road.ToList();
                    model.lstRoadTo = model.RoadTo.ToList();

                    switch (model.PLAN_RD_FROM_TYPE)
                    {
                        case "T":
                            model.lstRoadNumFrom = new SelectList(objDAL.GetRoadNumFromThroughEditList(model.PLAN_RD_FROM_TYPE, model.MAST_BLOCK_CODE, model.PLAN_CN_ROAD_CODE), "Value", "Text", model.PLAN_RD_NUM_FROM).ToList();
                            break;
                        case "L":
                            model.lstRoadNumFrom = new SelectList(objDAL.GetRoadNumFromThroughEditList(model.PLAN_RD_FROM_TYPE, model.MAST_BLOCK_CODE, model.PLAN_CN_ROAD_CODE), "Value", "Text", model.PLAN_RD_NUM_FROM).ToList();
                            break;
                        case "M":
                            model.lstRoadNumFrom = new SelectList(objDAL.GetRoadNumFromThroughEditList(model.PLAN_RD_FROM_TYPE, model.MAST_BLOCK_CODE, model.PLAN_CN_ROAD_CODE), "Value", "Text", model.PLAN_RD_NUM_FROM).ToList();
                            break;
                        case "H":
                            model.lstRoadNumFrom = model.RoadNumFromList.ToList();
                            break;
                        case "B":
                            model.lstRoadNumFrom = model.RoadNumFromList.ToList();
                            break;
                        default:
                            model.lstRoadNumFrom = new SelectList(objDAL.GetRoadNumFromByRoadFrom(model.PLAN_RD_FROM_TYPE, model.MAST_BLOCK_CODE), "MAST_ER_ROAD_CODE", "MAST_ER_ROAD_NAME", model.PLAN_RD_NUM_FROM).ToList();
                            break;
                    }

                    switch (model.PLAN_RD_TO_TYPE)
                    {
                        case "T":
                            model.lstRoadNumTo = new SelectList(objDAL.GetRoadNumFromThroughList(model.PLAN_RD_TO_TYPE, model.MAST_BLOCK_CODE), "PLAN_CN_ROAD_CODE", "PLAN_CN_ROAD_NUMBER", model.PLAN_RD_NUM_TO).ToList();
                            break;
                        case "L":
                            model.lstRoadNumTo = new SelectList(objDAL.GetRoadNumFromThroughList(model.PLAN_RD_TO_TYPE, model.MAST_BLOCK_CODE), "PLAN_CN_ROAD_CODE", "PLAN_CN_ROAD_NUMBER", model.PLAN_RD_NUM_TO).ToList();
                            break;
                        case "M":
                            model.lstRoadNumTo = new SelectList(objDAL.GetRoadNumFromThroughList(model.PLAN_RD_TO_TYPE, model.MAST_BLOCK_CODE), "PLAN_CN_ROAD_CODE", "PLAN_CN_ROAD_NUMBER", model.PLAN_RD_NUM_TO).ToList();
                            break;
                        case "H":
                            model.lstRoadNumTo = model.RoadNumToList.ToList();
                            break;
                        case "B":
                            model.lstRoadNumTo = model.RoadNumToList.ToList();
                            break;
                        default:
                            model.lstRoadNumTo = new SelectList(objDAL.GetRoadNumFromByRoadFrom(model.PLAN_RD_TO_TYPE, model.MAST_BLOCK_CODE), "MAST_ER_ROAD_CODE", "MAST_ER_ROAD_NAME", model.PLAN_RD_NUM_TO).ToList();
                            break;
                    }
                    if (model.PLAN_RD_ROUTE == "N")
                    {
                        List<string> lst = objDAL.MLRoadList(model.MAST_BLOCK_CODE);
                        List<SelectListItem> lstRoadNumberML = new List<SelectListItem>();
                        lstRoadNumberML.Insert(0, new SelectListItem { Value = "A", Text = "--Select Road No--" });
                        for (int i = 1; i <= 30; i++)
                        {
                            if (i < 10)
                            {
                                lstRoadNumberML.Add(new SelectListItem { Value = "ML0" + i.ToString(), Text = "ML0" + i.ToString() });
                            }
                            else
                            {
                                lstRoadNumberML.Add(new SelectListItem { Value = "ML" + i.ToString(), Text = "ML" + i.ToString() });
                            }
                        }

                        var list = (from item in lstRoadNumberML
                                    where !lst.Contains(item.Value)
                                    select new
                                    {
                                        item.Value,
                                        item.Text
                                    }).Distinct().ToList().Select(x => new SelectListItem
                                    {
                                        Text = x.Text,
                                        Value = x.Value
                                    }).ToList();

                        list.Add(new SelectListItem { Value = model.PLAN_CN_ROAD_NUMBER, Text = model.PLAN_CN_ROAD_NUMBER });
                        //list.Insert(list.Count, SelectListItem { Value = model.PLAN_CN_ROAD_NUMBER, Text = model.PLAN_CN_ROAD_NUMBER });
                        //new SelectList(list, "Value", "Text")

                        model.lstRoadNumber = list;
                    }
                    else
                    {
                        JsonResult a = GetRoadNumberByRoadRoute(model.PLAN_RD_ROUTE, model.MAST_BLOCK_CODE, model.PLAN_CN_ROAD_CODE);
                        ViewData["RoadNumber"] = a.Data;
                    }
                    model.lstRoadNumThroughFrom = model.RoadFrom.ToList();
                    model.lstRoadNumThroughTo = model.RoadFrom.ToList();

                    ViewBag.RoadCategory = objDAL.GetRoadCategory(model.MAST_ER_ROAD_CODE);

                    model.lstPreviousBlock = new SelectList(objDAL.GetBlocksByDistrictCode(PMGSYSession.Current.DistrictCode), "MAST_BLOCK_CODE", "MAST_BLOCK_NAME").ToList();
                    model.lstPreviousBlockRoadNo = new SelectList(objDAL.GetPreviousBlockByBlockCode(model.MAST_BLOCK_CODE), "PLAN_CN_ROAD_CODE", "PLAN_CN_ROAD_NUMBER").ToList();
                    //ViewData["RoadNumber"] = new SelectList(lstRoadNumber, "Value", "Text");
                    model.lstHabitationFrom = new SelectList(objDAL.GetHabitationsByBlockCode(model.MAST_BLOCK_CODE), "Value", "Text").ToList();
                    model.lstHabitationTo = new SelectList(objDAL.GetHabitationsByBlockCode(model.MAST_BLOCK_CODE), "Value", "Text").ToList();
                    model.lstNextBlock = new SelectList(objDAL.GetBlocksByDistrictCode(PMGSYSession.Current.DistrictCode), "MAST_BLOCK_CODE", "MAST_BLOCK_NAME").ToList();
                    model.lstNextBlockRoadNo = new SelectList(objDAL.GetPreviousBlockByBlockCode(model.MAST_BLOCK_CODE), "PLAN_CN_ROAD_CODE", "PLAN_CN_ROAD_NUMBER").ToList();
                    return PartialView("CoreNetworksPmgsy3Layout", model);
                }
                return PartialView("CoreNetworksPmgsy3Layout", new CoreNetworkViewModelPMGSY3());
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.EditCoreNetworksPMGSY3()");
                return Json(new { success = false, message = "Error occurred while processing your request" });
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Audit]
        public ActionResult EditCoreNetworksPMGSY3ITNO(CoreNetworkViewModelPMGSY3 model)
        {
            bool status = false;
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            try
            {
                if (ModelState.IsValid)
                {
                    if (objDAL.EditCoreNetworksPMGSY3DAL(model, ref message))
                    {
                        message = message == string.Empty ? "TR/MRL road details updated successfully." : message;

                        status = true;
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        message = message == string.Empty ? "Error occurred while processing your request." : message;
                        ModelState.AddModelError("", message);
                        return Json(new { success = status, message = new CommonFunctions().FormatErrorMessage(ModelState) }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.EditCoreNetworksPMGSY3()");
                message = message == string.Empty ? "Error occurred while processing your request." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// deletes the particular core network details
        /// </summary>
        /// <param name="parameter">Encrypted id</param>
        /// <param name="hash">Encrypted id</param>
        /// <param name="key">Encrypted id</param>
        /// <returns>status of delete operation </returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Audit]
        public ActionResult DeleteCoreNetworkPMGSY3ITNO(String parameter, String hash, String key)
        {
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptedParameters.Count() > 0)
                {
                    if (!objDAL.DeleteCoreNetworksPMGSY3DAL(Convert.ToInt32(decryptedParameters["NetworkCode"].ToString())))
                    {
                        ModelState.AddModelError(String.Empty, "TR/MRL road details are in use and can not be deleted.");
                        message = "TR/MRL road details are in use and can not be deleted.";

                        return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                message = "TR/MRL road details deleted successfully.";

                return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.DeleteCoreNetworkPMGSY3");

                return Json(new { success = false, message = "TR/MRL road details can not be deleted" }, JsonRequestBehavior.AllowGet);
            }
        }

        ///Habitation Mapping
        [HttpGet]
        [Audit]
        public ActionResult ListHabitationsPMGSY3ITNO(String parameter, String hash, String key)
        {
            HabitationDetailsViewModelPMGSY3 model = new HabitationDetailsViewModelPMGSY3();
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            CommonFunctions objCommon = new CommonFunctions();
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                int networkCode = Convert.ToInt32(decryptedParameters["NetworkCode"]);
                model.UnlockFlag = decryptedParameters["UnlockFlag"];
                model.EncryptedRoadCode = networkCode.ToString();
                ViewBag.RoadNumber = db.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == networkCode).Select(m => m.PLAN_CN_ROAD_NUMBER).FirstOrDefault();

                //ViewData["Habitations"] = objDAL.GetHabitationCodeList(networkCode);

                model.lstRoads = objDAL.GetRoadsByCNCodePMGSY3(networkCode);

                return PartialView("ListHabitationsPMGSY3", model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.ListHabitationsPMGSY3()");
                return Json(new { success = false, message = "Error occurred while processing the request" });
            }
        }

        [HttpGet]
        [Audit]
        public ActionResult GetHabitationListToMapPMGSY3ITNO(int? page, int? rows, string sidx, string sord)
        {
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            try
            {
                int roadCode = 0, erRoadCode = 0;
                string habDirect = string.Empty;
                long totalRecords = 0;
                bool isHabFinalized = false;
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["habCode"]))
                {
                    roadCode = Convert.ToInt32(Request.Params["habCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["erRoadCode"]))
                {
                    erRoadCode = Convert.ToInt32(Request.Params["erRoadCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["habDirect"]))
                {
                    habDirect = Request.Params["habDirect"];
                }

                var jsonData = new
                {
                    rows = objDAL.GetHabitationListToMapPMGSY3DAL(roadCode, habDirect, erRoadCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, out isHabFinalized),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                    isHabFinalized = isHabFinalized
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.GetHabitationListToMapPMGSY3()");
                return null;
            }
        }

        [HttpGet]
        [Audit]
        public ActionResult GetHabitationListPMGSY3ITNO(int? page, int? rows, string sidx, string sord)
        {
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            int habCode = 0;

            long totalRecords = 0;
            string flag = string.Empty;
            bool isHabFinalized = false;
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["habCode"]))
                {
                    habCode = Convert.ToInt32(Request.Params["habCode"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["Flag"]))
                {
                    flag = Request.Params["Flag"];
                }

                var jsonData = new
                {
                    rows = objDAL.GetHabitationListPMGSY3DAL(habCode, flag, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, out isHabFinalized),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                    isHabFinalized = isHabFinalized
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.GetHabitationListPMGSY3()");
                return null;
            }
        }

        /// <summary>
        /// add habitation to core network
        /// </summary>
        /// <param name="habModel">contains the details of Habitation</param>
        /// <returns>response message along with status</returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Audit]
        public ActionResult MapHabitationsToNetworkPMGSY3ITNO(FormCollection mappedCollection)
        {
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
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
                    message = "Habitations added successfully.";
                    status = true;
                }
                else
                {
                    status = false;
                    message = "Habitations not added.";
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.MapHabitationsToNetworkPMGSY3()");
                message = "Error occurred while processing the request.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// deletes the Habitation details
        /// </summary>
        /// <param name="hash">Encrypted id</param>
        /// <param name="parameter">Encrypted id</param>
        /// <param name="key">Encrypted id</param>
        /// <param name="roadCode">road id</param>
        /// <returns>response message along with status</returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Audit]
        public ActionResult DeleteMapHabitationPMGSY3ITNO(String hash, String parameter, String key, string roadCode)
        {
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            Dictionary<string, string> decryptedParameters = null;
            int coreNetworkCode = Convert.ToInt32(roadCode);
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptedParameters.Count() > 0)
                {
                    if (!objDAL.DeleteMapHabitationPMGSY3DAL(Convert.ToInt32(decryptedParameters["HabCode"].ToString()), decryptedParameters["Flag"].ToString(), coreNetworkCode))
                    {
                        ModelState.AddModelError(String.Empty, "Habitation not deleted.");
                        return Json(new { success = false, message = "Habitation can not be deleted" }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { success = true, message = "Habitation deleted successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.DeleteMapHabitationPMGSY3()");
                return Json(new { success = false, message = "Habitation can not be deleted" }, JsonRequestBehavior.AllowGet);
            }
        }

        //Map DRRP to CN
        [HttpGet]
        public ActionResult MapOtherCandidateRoadViewPMGSY3ITNO(String parameter, String hash, String key)
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                CoreNetworkDAL objDAL = new CoreNetworkDAL();
                CandidateRoadViewModel model = new CandidateRoadViewModel();
                if (!(String.IsNullOrEmpty(parameter)) && !(String.IsNullOrEmpty(hash)) && !(String.IsNullOrEmpty(key)))
                {
                    Dictionary<string, string> decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                    if (decryptedParameters != null)
                    {
                        model.CNCode = Convert.ToInt32(decryptedParameters["NetworkCode"]);
                    }
                }
                model.LockStatus = objDAL.GetLockStatusOfCandidateRoad(model.CNCode);
                //model.lstBlocks = objCommon.PopulateBlocks(PMGSYSession.Current.DistrictCode);
                model.lstBlocks = new SelectList(objDAL.GetBlocksByDistrictCode(PMGSYSession.Current.DistrictCode), "MAST_BLOCK_CODE", "MAST_BLOCK_NAME").ToList();
                model.lstBlocks.Insert(0, new SelectListItem { Value = "0", Text = "Select Block" });
                model.lstRoadCategory = new SelectList(objDAL.GetAllRoadCategories().Where(m => m.MAST_ROAD_SHORT_DESC == "RR(ODR)" || m.MAST_ROAD_SHORT_DESC == "RR(VR)" || m.MAST_ROAD_SHORT_DESC == "RR(TRACK)").ToList(), "MAST_ROAD_CAT_CODE", "MAST_ROAD_CAT_NAME").ToList();
                model.lstRoadCategory.Insert(0, new SelectListItem { Value = "0", Text = "- Select Road Category -" });
                model.lstDRRP.Insert(0, new SelectListItem { Value = "0", Text = "- Select Road Name -" });

                List<string> lst = objDAL.GetCNDetailsPMGSY3DAL(model.CNCode);
                if (lst != null)
                {
                    ViewBag.RoadName = lst[0].Trim();
                    ViewBag.TotLength = lst[1].Trim();
                    ViewBag.BalLength = lst[2].Trim();
                }

                return PartialView(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.MapOtherCandidateRoadViewPMGSY3()");
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        [HttpGet]
        public JsonResult GetMappedCandidateRoadListPMGSY3ITNO(int? page, int? rows, string sidx, string sord)
        {
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            long totalRecords = 0;
            int candidateRoadCode = 0;
            string IsFinalized = String.Empty;
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["RoadCode"]))
                {
                    candidateRoadCode = Convert.ToInt32(Request.Params["RoadCode"]);
                }

                var jsonData = new
                {
                    rows = objDAL.ListCandidateRoadsPMGSY3DALITNO(candidateRoadCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, out IsFinalized),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                    IsFinalized = IsFinalized
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.GetMappedCandidateRoadList()");
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MapCandidateRoadPMGSY3ITNO(CandidateRoadViewModel model)
        {
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            try
            {
                if (ModelState.IsValid)
                {
                    if (objDAL.MapCandidateRoadPMGSY3DAL(model, ref message))
                    {
                        message = message == string.Empty ? "DRRP details mapped successfully." : message;
                        return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        message = message == string.Empty ? "Error occurred while processing your request." : message;
                        ModelState.AddModelError("", message);
                        return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.MapCandidateRoadPMGSY3()");
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteMappedDRRPDetailsPMGSY3ITNO(String parameter, String hash, String key)
        {
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            int DRRPCode = 0;
            int CNCode = 0;
            try
            {
                if (!(String.IsNullOrEmpty(parameter)) && !(String.IsNullOrEmpty(hash)) && !(String.IsNullOrEmpty(key)))
                {
                    Dictionary<string, string> decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                    if (decryptedParameters != null)
                    {
                        DRRPCode = Convert.ToInt32(decryptedParameters["DRRPCode"]);
                        CNCode = Convert.ToInt32(decryptedParameters["CNCode"]);
                    }
                }

                if (!(objDAL.DeleteMappedDRRPDetailsPMGSY3DAL(DRRPCode, CNCode)))
                {
                    return Json(new { success = false });
                }
                else
                {
                    return Json(new { success = true });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.DeleteMappedDRRPDetailsPMGSY3()");
                return Json(new { success = false });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FinalizeMappedDRRPDetailsPMGSY3ITNO(String id)
        {
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            int CNCode = 0;
            string message = string.Empty;
            try
            {
                if (!String.IsNullOrEmpty(id))
                {
                    CNCode = Convert.ToInt32(id);
                    if (objDAL.FinalizeMappedDRRPDetailsPMGSY3DAL(CNCode, ref message))
                    {
                        return Json(new { success = true, message = message });
                    }
                    else
                    {
                        return Json(new { success = false, message = message });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "TR/MRL Road details are not present." });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.FinalizeMappedDRRPDetailsPMGSY3()");
                return Json(new { success = false, message = "Error on DRRP details finalize." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeFinalizeMappedDRRPDetailsPMGSY3ITNO(String id)
        {
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            int CNCode = 0;
            try
            {
                if (!String.IsNullOrEmpty(id))
                {
                    CNCode = Convert.ToInt32(id);
                    if (objDAL.DeFinalizeMappedDRRPDetailsPMGSY3DALITNO(CNCode))
                    {
                        return Json(new { success = true });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Error occurred while processing your request." });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "TR/MRL Road details are not present." });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.DeFinalizeMappedDRRPDetailsPMGSY3");
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        [HttpGet]
        [Audit]
        public ActionResult GetCoreNetworkChecksPMGSY3ITNO()
        {
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            try
            {
                int PLAN_CN_ROAD_CODE = Convert.ToInt32(Request.Params["PLAN_CN_ROAD_CODE"]);
                string status = objDAL.GetCoreNetworkChecksPMGSY3DAL(PLAN_CN_ROAD_CODE);
                if (status == string.Empty)
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { Success = false, ErrorMessage = status }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.GetCoreNetworkChecksPMGSY3()");
                return Json(new { Success = false, message = "Error occurred while processing the request." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult PopulateBlockListTRITNO()
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

        #region added by abhinav
        [HttpPost]
        public ActionResult CheckBlockFinalizationITNO()
        {
            try
            {
                var blockcode = Convert.ToInt32(Request.Params["BlockCode"]);
                var Blckname = Request.Params["BlockName"];

                var districtcode = Convert.ToInt32(Request.Params["DistrictCode"]);
                var districtname = Request.Params["DistrictName"];

                PMGSYEntities context = new PMGSYEntities();
                bool isBlockFinalized = false;
                bool isDistrictFinalized = false;

                isBlockFinalized = context.MAST_CN_BLOCK_PMGSY3_FINALIZE.Where(x => x.MAST_BLOCK_CODE == blockcode && x.IS_FINALIZED == "Y").Any();
                isDistrictFinalized = context.MAST_CN_DISTRICT_PMGSY3_FINALIZE.Where(x => x.MAST_DISTRICT_CODE == districtcode && x.IS_FINALIZED == "Y").Any();
                if (isDistrictFinalized)
                {
                    return Json(new { status = isBlockFinalized, ErrorMsg = districtname + " District is already Finalized. So, Roads cannot be definalized" }, JsonRequestBehavior.DenyGet);
                }

                if (isBlockFinalized)
                {
                    return Json(new { status = isBlockFinalized, ErrorMsg = Blckname + " Block is already Finalized. So, Roads cannot be definalized" }, JsonRequestBehavior.DenyGet);
                }
                else
                {
                    return Json(new { status = isBlockFinalized, ErrorMsg = "" }, JsonRequestBehavior.DenyGet);
                }


            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork/CheckBlockFinalization");
                return null;
            }
        }

        #endregion

        #endregion

        #region PMGSY3 mord TR/MRL blocks definalization
        [HttpGet]
        public ActionResult DeFinalizeMRLPMGSY3Layout()
        {
            FinalizeMRLPMGSY3ViewModel model = new FinalizeMRLPMGSY3ViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                if (PMGSYSession.Current.RoleCode == 25)
                {
                    model.statelist = comm.PopulateStates(false);
                }

                model.lstDistricts = new List<SelectListItem>();
                model.lstDistricts.Insert(0, new SelectListItem { Text = "Select District", Value = "-1", Selected = true });

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.FinalizeMRLPMGSY3Layout()");
                return null;
            }
        }

        [HttpGet]
        [Audit]
        public ActionResult GetBlockListMRLPMGSY3MORD(int? page, int? rows, string sidx, string sord)
        {
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            String searchParameters = String.Empty;
            long totalRecords;

            int districtCode = 0;
            bool isAllBlockFinalized = false;
            bool isDistrictFinalizedForTRMRL = false;
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
                    rows = objDAL.GetBlockListMRLPMGSY3DALMORD(districtCode, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords, Request.Params["filters"], ref isAllBlockFinalized, ref isDistrictFinalizedForTRMRL, Convert.ToInt32(Request.Params["statecode"])),
                    total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                    page = Convert.ToInt32(Request.Params["page"]),
                    records = totalRecords,
                    isAllBlockFinalized = isAllBlockFinalized,
                    isDistrictFinalizedForTRMRL = isDistrictFinalizedForTRMRL
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.GetBlockListMRLPMGSY3()");
                return null;
            }
        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult FinalizeMRLBlockMORD(String parameter, String hash, String key)
        {
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            string message = string.Empty;
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                int blockCode = Convert.ToInt32(decryptedParameters["BlockCode"]);

                if (decryptedParameters.Count() > 0)
                {
                    bool status = objDAL.FinalizeMRLBlockPMGSY3DAL(blockCode, ref message);
                    return Json(new { success = status, message = message.Trim() });
                }
                else
                {
                    return Json(new { success = false, message = "Invalid request" });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.FinalizeMRLBlock()");
                return Json(new { success = false, message = "Error occurred while processing the request." });
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult FinalizeMRLDistrictMORD(int districtCode)
        {
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            string message = string.Empty;
            try
            {
                bool status = objDAL.FinalizeMRLDistrictPMGSY3DAL(districtCode, ref message);
                return Json(new { success = status, message = message.Trim() });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.FinalizeMRLDistrict()");
                return Json(new { success = false, message = "Error occurred while processing the request." });
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult DeFinalizeMRLBlockMORD(String parameter, String hash, String key)
        {
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            string message = string.Empty;
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                int blockCode = Convert.ToInt32(decryptedParameters["BlockCode"]);

                if (decryptedParameters.Count() > 0)
                {
                    bool status = objDAL.DeFinalizeMRLBlockPMGSY3DAL(blockCode, ref message);
                    return Json(new { success = status, message = message.Trim() });
                }
                else
                {
                    return Json(new { success = false, message = "Invalid request" });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.DeFinalizeMRLBlock()");
                return Json(new { success = false, message = "Error occurred while processing the request." });
            }
        }

        [HttpGet]
        public ActionResult PopulateDistrictListMORD()
        {
            try
            {
                CommonFunctions comm = new CommonFunctions();
                var BlockList = comm.PopulateDistrict(Convert.ToInt32(Request.Params["StateCode"]));
                BlockList.RemoveAll(x => x.Value == "0");
                BlockList.Insert(0, new SelectListItem { Text = "Select District", Value = "0", Selected = true });
                return Json(BlockList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ExistingRoads/PopulateBlockList");
                return null;
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult DefinalizeMRLDistrictMORD(int districtCode)  //Added by Aditi on 4 June 2020
        {
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            string message = string.Empty;
            try
            {
                bool status = objDAL.DefinalizeMRLDistrictPMGSY3DAL(districtCode, ref message);
                return Json(new { success = status, message = message.Trim() });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.DefinalizeMRLDistrictMORD()");
                return Json(new { success = false, message = "Error occurred while processing the request." });
            }
        }

        #endregion

        #region CUPL PMGSY3 Storing
        [HttpGet]
        public ActionResult CUPLPMGSY3Layout()
        {
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            CUPLPMGSY3ViewModel model = new CUPLPMGSY3ViewModel();
            CommonFunctions comm = new CommonFunctions();
            int year = 0;
            try
            {
                if (PMGSYSession.Current.StateCode > 0 && PMGSYSession.Current.DistrictCode == 0)
                {
                    model.lstDistricts = comm.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                    model.lstDistricts.Find(x => x.Value == "-1").Text = "Select District";
                }
                if (PMGSYSession.Current.DistrictCode > 0)
                {
                    model.lstDistricts = new List<SelectListItem>();
                    model.lstDistricts.Insert(0, new SelectListItem { Text = PMGSYSession.Current.DistrictName.Trim(), Value = Convert.ToString(PMGSYSession.Current.DistrictCode) });
                }

                model.lstYears = comm.PopulateFinancialYear(false).ToList();
                int count = model.lstYears.IndexOf(model.lstYears.Find(c => c.Value == "2018"));
                model.lstYears.RemoveRange(model.lstYears.IndexOf(model.lstYears.Find(c => c.Value == "2018")), model.lstYears.Count - count);

                if (DateTime.Now.Month <= 3)
                {
                    model.lstYears.RemoveAt(model.lstYears.IndexOf(model.lstYears.Find(z => z.Value == DateTime.Now.Year.ToString())));
                    model.Year = DateTime.Now.Year - 1;
                }
                else
                {
                    model.Year = DateTime.Now.Year;
                }

                model.lstBatch = new List<SelectListItem>();
                model.lstBatch.Insert(0, new SelectListItem() { Text = "Batch 1", Value = "1" });
                model.lstBatch.Insert(1, new SelectListItem() { Text = "Batch 2", Value = "2" });
                model.lstBatch.Insert(2, new SelectListItem() { Text = "Batch 3", Value = "3" });
                model.lstBatch.Insert(3, new SelectListItem() { Text = "Batch 4", Value = "4" });
                model.lstBatch.Insert(4, new SelectListItem() { Text = "Batch 5", Value = "5" });
                model.lstBatch.Insert(5, new SelectListItem() { Text = "Batch 6", Value = "6" });

                //model.Batch = objDAL.PopulateCUPLBatch(PMGSYSession.Current.DistrictCode, model.blockCode, model.Year);
                //model.lstBatch.Insert(0, new SelectListItem() { Text = "Batch " + model.Batch.ToString(), Value = model.Batch.ToString() });
                //model.lstBatch = objDAL.PopulateCUPLBatch(PMGSYSession.Current.DistrictCode, model.blockCode, model.Year);
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.FinalizeCUPLPMGSY3Layout()");
                return null;
            }
        }

        [HttpGet]
        [Audit]
        public ActionResult GetBlockListCUPLPMGSY3(int? page, int? rows, string sidx, string sord)
        {
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            String searchParameters = String.Empty;
            long totalRecords;

            int districtCode = 0, Year = 0, Batch = 0;
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
                if (!string.IsNullOrEmpty(Request.Params["Year"]))
                {
                    Year = Convert.ToInt32(Request.Params["Year"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["Batch"]))
                {
                    Batch = Convert.ToInt32(Request.Params["Batch"]);
                }

                var jsonData = new
                {
                    rows = objDAL.GetBlockListCUPLPMGSY3DAL(districtCode, Year, Batch, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                    page = Convert.ToInt32(Request.Params["page"]),
                    records = totalRecords,
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.GetBlockListMRLPMGSY3()");
                return null;
            }
        }

        //[HttpPost]
        //public ActionResult CUPLPMGSY3Report(string parameter, string hash, string key)
        //{
        //    CUPLPMGSY3ViewModel model = new CUPLPMGSY3ViewModel();
        //    Dictionary<string, string> parameters = null;
        //    try
        //    {
        //        parameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
        //        if (parameters != null)
        //        {
        //            model.stateCode = PMGSYSession.Current.StateCode;
        //            model.stateName = PMGSYSession.Current.StateName;
        //            model.distCode = PMGSYSession.Current.DistrictCode;
        //            model.districtName = PMGSYSession.Current.DistrictName;
        //            model.blockCode = Convert.ToInt32(parameters["BlockCode"]);
        //            model.blockName = parameters["BlockName"];
        //        }
        //        return View(model);
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "CoreNetwork/CUPLPMGSY3Report");
        //        return null;
        //    }
        //}
        [HttpPost]
        public ActionResult CUPLPMGSY3Report(string parameter, string hash, string key)
        {
            CUPLPMGSY3ViewModel model = new CUPLPMGSY3ViewModel();
            Dictionary<string, string> parameters = null;
            try
            {
                parameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (parameters != null)
                {
                    model.stateCode = PMGSYSession.Current.StateCode;
                    model.stateName = PMGSYSession.Current.StateName;
                    model.distCode = PMGSYSession.Current.DistrictCode;
                    model.districtName = PMGSYSession.Current.DistrictName;
                    model.blockCode = Convert.ToInt32(parameters["BlockCode"]);
                    model.blockName = parameters["BlockName"];
                }
                ViewData["IsHabitationUnlocked"] = checkIsUnlocked(PMGSYSession.Current.StateCode, PMGSYSession.Current.DistrictCode, model.blockCode);

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork/CUPLPMGSY3Report");
                return null;
            }
        }


        [HttpGet]
        public ActionResult GenerateCUPLPMGSY3Layout(string parameter, string hash, string key)
        {
            CUPLPMGSY3ViewModel model = new CUPLPMGSY3ViewModel();
            CommonFunctions comm = new CommonFunctions();
            Dictionary<string, string> parameters = null;
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            try
            {
                if (!(String.IsNullOrEmpty(parameter)) && !(String.IsNullOrEmpty(hash)) && !(String.IsNullOrEmpty(key)))
                {
                    parameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                    if (parameters != null)
                    {
                        model.stateCode = PMGSYSession.Current.StateCode;
                        model.stateName = PMGSYSession.Current.StateName;
                        model.distCode = PMGSYSession.Current.DistrictCode;
                        model.districtCode = PMGSYSession.Current.DistrictCode;
                        model.districtName = PMGSYSession.Current.DistrictName;
                        if (parameters.Count() == 2)
                        {
                            if (!string.IsNullOrEmpty(parameters["BlockCode"]))
                            {
                                model.blockCode = Convert.ToInt32(parameters["BlockCode"]);
                            }
                            if (!string.IsNullOrEmpty(parameters["BlockName"]))
                            {
                                model.blockName = parameters["BlockName"];
                            }
                        }
                        if (!string.IsNullOrEmpty(Request.Params["Year"]))
                        {
                            model.Year = Convert.ToInt32(Request.Params["Year"]);
                        }
                        if (!string.IsNullOrEmpty(Request.Params["Batch"]))
                        {
                            model.Batch = Convert.ToInt32(Request.Params["Batch"]);
                        }
                    }
                }
                //model.lstDistricts = comm.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                //model.lstDistricts.Find(x => x.Value == "-1").Text = "Select District";

                model.lstDistricts = new List<SelectListItem>();
                model.lstDistricts.Insert(0, new SelectListItem { Text = PMGSYSession.Current.DistrictName.Trim(), Value = Convert.ToString(PMGSYSession.Current.DistrictCode) });

                model.lstYears = new List<SelectListItem>();
                model.lstYears.Insert(0, new SelectListItem() { Text = model.Year.ToString() + "-" + Convert.ToString(model.Year + 1), Value = model.Year.ToString() });

                //model.lstYears = comm.PopulateFinancialYear(true).ToList();

                /*int count = model.lstYears.IndexOf(model.lstYears.Find(c => c.Value == "2018"));
                model.lstYears.RemoveRange(model.lstYears.IndexOf(model.lstYears.Find(c => c.Value == "2018")), model.lstYears.Count - count);

                if (DateTime.Now.Month <= 3)
                {
                    model.lstYears.RemoveAt(model.lstYears.IndexOf(model.lstYears.Find(z => z.Value == DateTime.Now.Year.ToString())));
                    model.Year = DateTime.Now.Year - 1;
                }
                else
                {
                    model.Year = DateTime.Now.Year;
                }*/

                //model.lstBatch = comm.PopulateBatch();
                model.lstBatch = new List<SelectListItem>();

                // Uncommented 3 lines on 05 Feb 2021
                model.lstBatch.Insert(0, new SelectListItem() { Text = "Batch 1", Value = "1" });
                model.lstBatch.Insert(1, new SelectListItem() { Text = "Batch 2", Value = "2" });
                model.lstBatch.Insert(2, new SelectListItem() { Text = "Batch 3", Value = "3" });
                model.lstBatch.Insert(3, new SelectListItem() { Text = "Batch 4", Value = "4" });
                model.lstBatch.Insert(4, new SelectListItem() { Text = "Batch 5", Value = "5" });
                model.lstBatch.Insert(5, new SelectListItem() { Text = "Batch 6", Value = "6" });

                //model.Batch = objDAL.PopulateCUPLBatch(PMGSYSession.Current.DistrictCode, model.blockCode, model.Year);
                //model.lstBatch.Insert(0, new SelectListItem() { Text = "Batch " + model.Batch.ToString(), Value = model.Batch.ToString() });


                //Commented on 05 Feb 2021
                //   model.lstBatch = objDAL.PopulateCUPLBatch(PMGSYSession.Current.DistrictCode, model.blockCode, model.Year);

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.GenerateCUPLPMGSY3Layout()");
                return null;
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Audit]
        public ActionResult GenerateCUPLPMGSY3(CUPLPMGSY3ViewModel model)
        {
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            bool status = false;
            Dictionary<string, string> parameters = null;
            try
            {
                if (ModelState.IsValid)
                {
                    if (objDAL.GeneratePMGSY3DAL(model, ref message))
                    {
                        message = message == string.Empty ? "CUPL details generated successfully." : message;

                        status = true;
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        message = message == string.Empty ? "Error occurred while processing the request." : message;
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.AddCoreNetworksPMGSY3()");
                return Json(new { success = false, message = "Error occurred while processing the request." });
            }
        }

        [HttpPost]
        public ActionResult CUPLPMGSY3GeneratedReport(string parameter, string hash, string key)
        {
            CUPLPMGSY3ViewModel model = new CUPLPMGSY3ViewModel();
            Dictionary<string, string> parameters = null;
            try
            {
                parameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (parameters != null)
                {
                    model.stateCode = PMGSYSession.Current.StateCode;
                    model.stateName = PMGSYSession.Current.StateName;
                    model.distCode = PMGSYSession.Current.DistrictCode;
                    model.districtName = PMGSYSession.Current.DistrictName;
                    model.blockCode = Convert.ToInt32(parameters["BlockCode"]);
                    model.blockName = parameters["BlockName"];


                    // Added By Rohit on 22 Apr 2020
                    model.Batch = Convert.ToInt32(parameters["BatchCode"]);
                    model.Year = Convert.ToInt32(parameters["YearCode"]);
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork/CUPLPMGSY3Report");
                return null;
            }
        }

        [HttpGet]
        [Audit]
        public ActionResult GetCUPLPMGSY3List(int? page, int? rows, string sidx, string sord)
        {
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            String searchParameters = String.Empty;
            long totalRecords;
            int districtCode = 0, blockCode = 0;
            Dictionary<string, string> parameters = null;
            string[] arrparams = null;
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["urlparameter"]))
                {
                    arrparams = Request.Params["urlparameter"].Split('/');

                    if (!(String.IsNullOrEmpty(arrparams[0])) && !(String.IsNullOrEmpty(arrparams[1])) && !(String.IsNullOrEmpty(arrparams[2])))
                    {
                        parameters = URLEncrypt.DecryptParameters1(new string[] { arrparams[0], arrparams[1], arrparams[2] });
                        if (parameters != null)
                        {
                            if (parameters.Count() == 2)
                            {
                                if (!string.IsNullOrEmpty(parameters["BlockCode"]))
                                {
                                    blockCode = Convert.ToInt32(parameters["BlockCode"]);
                                }
                            }
                        }
                    }

                    if (PMGSYSession.Current.DistrictCode > 0)
                    {
                        districtCode = PMGSYSession.Current.DistrictCode;
                    }

                }
                var jsonData = new
                {
                    rows = objDAL.GetCUPLPMGSY3ListDAL(districtCode, blockCode, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                    page = Convert.ToInt32(Request.Params["page"]),
                    records = totalRecords,
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.GetBlockListMRLPMGSY3()");
                return null;
            }
        }


        //[ValidateAntiForgeryToken]
        [HttpPost]
        [Audit]
        public ActionResult CopyTRMRLExemptiontoBatch2(string parameter, string hash, string key)
        {
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            bool status = false;
            Dictionary<string, string> parameters = null;
            int block = 0, year = 0;
            try
            {
                parameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });

                if (parameters != null)
                {
                    block = Convert.ToInt32(parameters["block"]);
                    year = Convert.ToInt32(parameters["year"]);
                }

                if (ModelState.IsValid)
                {
                    if (objDAL.CopyTRMRLExemptiontoBatch2DAL(block, year, ref message))
                    {
                        message = message == string.Empty ? "TR/MRL Exemption details copied successfully." : message;

                        status = true;
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        message = message == string.Empty ? "Error occurred while processing the request." : message;
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.CopyTRMRLExemptiontoBatch2()");
                return Json(new { success = false, message = "Error occurred while processing the request." });
            }
        }
        #endregion


        #region Unlock Method
        // Added By Rohit On 13APR2020

        // Check if TR / MRL Habitations are Unlocked or Not 
        public bool checkIsUnlocked(int state, int district, int block)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            bool flag = false;
            try
            {
                // State 
                if (dbContext.IMS_UNLOCK_DETAILS.Where(x => x.MAST_PMGSY_SCHEME == 4 && x.IMS_UNLOCK_TABLE == "CH" && x.IMS_UNLOCK_LEVEL == "S" && x.IMS_UNLOCK_STATUS == "Y" && x.MAST_STATE_CODE == state && x.MAST_DISTRICT_CODE == null && x.MAST_BLOCK_CODE == null && (DateTime.Now >= x.IMS_UNLOCK_START_DATE && DateTime.Now <= x.IMS_UNLOCK_END_DATE)).Any())
                {
                    flag = true;
                    return flag;
                }// District
                else if (dbContext.IMS_UNLOCK_DETAILS.Where(x => x.MAST_PMGSY_SCHEME == 4 && x.IMS_UNLOCK_TABLE == "CH" && x.IMS_UNLOCK_LEVEL == "D" && x.IMS_UNLOCK_STATUS == "Y" && x.MAST_STATE_CODE == state && x.MAST_DISTRICT_CODE == district && x.MAST_BLOCK_CODE == null && (DateTime.Now >= x.IMS_UNLOCK_START_DATE && DateTime.Now <= x.IMS_UNLOCK_END_DATE)).Any())
                {
                    flag = true;
                    return flag;
                }// Block
                else if (dbContext.IMS_UNLOCK_DETAILS.Where(x => x.MAST_PMGSY_SCHEME == 4 && x.IMS_UNLOCK_TABLE == "CH" && x.IMS_UNLOCK_LEVEL == "B" && x.IMS_UNLOCK_STATUS == "Y" && x.MAST_STATE_CODE == state && x.MAST_DISTRICT_CODE == null && x.MAST_BLOCK_CODE == block && (DateTime.Now >= x.IMS_UNLOCK_START_DATE && DateTime.Now <= x.IMS_UNLOCK_END_DATE)).Any())
                {
                    flag = true;
                    return flag;
                }
                else
                {
                    flag = false;
                    return flag;
                }

                // flag = Convert.ToInt32(query) > 0 ? true : false;

                //  return flag;

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetworkDAL.checkIsUnlocked()");
                return false;
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


        #region Village Vibrant Programme // added by rohit for vibrant village prog on 18-07-2023

        /// <summary>
        /// Returns the list of core network details
        /// </summary>
        /// <returns>List View of Core Network details</returns>
        [Audit]
        public ActionResult ListCoreNetWorksVVP()
        {
            List<MASTER_BLOCK> lst = new List<MASTER_BLOCK>();
            int block = 0;
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                int districtCode = PMGSYSession.Current.DistrictCode;
                CoreNetworkDAL objDAL = new CoreNetworkDAL();
                ViewBag.Scheme = PMGSYSession.Current.PMGSYScheme;
                ViewData["Route"] = objDAL.GetAllRoutes();
                ViewData["Category"] = objDAL.GetCategoryForSearch();

                List<SelectListItem> lstDefault = new List<SelectListItem>();
                lstDefault.Insert(0, new SelectListItem { Value = "0", Text = "All" });
                if (PMGSYSession.Current.RoleCode == 25 || PMGSY.Extensions.PMGSYSession.Current.RoleCode == 53 || PMGSYSession.Current.RoleCode == 65)//Changes by SAMMED A. PATIL for mordviewuser
                {
                    ViewData["States"] = objCommon.PopulateStates(true);
                    ViewData["Districts"] = lstDefault;
                    ViewData["Blocks"] = lstDefault;
                }
                else if (PMGSYSession.Current.RoleCode == 2)
                {
                    ViewData["Districts"] = objCommon.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                    ViewData["Blocks"] = lstDefault;
                }
                else
                {
                    ViewData["Blocks"] = new SelectList(objDAL.GetBlocksByDistrictCode(districtCode), "MAST_BLOCK_CODE", "MAST_BLOCK_NAME");
                    lst = objDAL.GetBlocksByDistrictCode(districtCode);

                    if (lst.Count > 0)
                    {
                        PMGSY.DAL.ExistingRoads.IExistingRoadsDAL objDRRPDAL = new PMGSY.DAL.ExistingRoads.ExistingRoadsDAL();
                        ViewData["isUnlocked"] = objDRRPDAL.CheckUnlockedDAL(lst.Select(x => x.MAST_BLOCK_CODE).First());
                    }
                }
                //for (int i = 0; i < lst.Count; i++)
                //{
                //    if (i == 0)
                //    { 
                //        block = lst.
                //    }
                //}
                //block = lst.ElementAt(0);
                ViewData["IsLocked"] = objDAL.checkIsLocked(block);

                return View();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return View();
            }

        }


        /// <summary>
        /// returns the list of core network details for populating the grid
        /// </summary>
        /// <param name="networkCollection">form collection containing Core Network Grid Parameters</param>
        /// <returns>Json data containing list to populate grid</returns>
        [HttpPost]
        [Audit]
        public ActionResult GetCoreNetWorksListVVP(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                int blockCode = 0;
                int districtCode = 0;
                int stateCode = 0;
                int categoryCode = 0;
                long totalRecords = 0;
                string roadType = string.Empty;
                String searchParameters = String.Empty;
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                string roadName = string.Empty;
                int CNCode = 0;

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

                if (!string.IsNullOrEmpty(Request.Params["blockCode"]))
                {
                    blockCode = Convert.ToInt32(Request.Params["blockCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["categoryCode"]))
                {
                    categoryCode = Convert.ToInt32(Request.Params["categoryCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["roadType"]))
                {
                    roadType = Request.Params["roadType"];
                }

                if (!string.IsNullOrEmpty(Request.Params["PLAN_RD_NAME"]))
                {
                    roadName = Request.Params["PLAN_RD_NAME"];
                }

                if (!string.IsNullOrEmpty(Request.Params["CNCode"]))
                {
                    CNCode = Convert.ToInt32(Request.Params["CNCode"]);
                }

                var jsonData = new
                {
                    rows = objBAL.GetCoreNetWorksListVVP(stateCode, districtCode, blockCode, roadType, categoryCode, roadName, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, CNCode),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
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
        /// returns the core network add view
        /// </summary>
        /// <param name="blockCode">id of block</param>
        /// <returns>returns the add view of Core Network</returns>
        [Audit]
        public ActionResult AddEditCoreNetworksVVP(/*int blockCode*/ string id)
        {
            CoreNetworkViewModel model = new CoreNetworkViewModel();
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            bool flag = false;
            int stateCode = 0, districtCode = 0, blockCode = 0;

            string[] locationcodes = id.Split('$');
            try
            {
                stateCode = locationcodes[0] == "undefined" ? 0 : Convert.ToInt32(locationcodes[0]);
                districtCode = locationcodes[1] == "undefined" ? 0 : Convert.ToInt32(locationcodes[1]);

                model.MAST_STATE_CODE = stateCode > 0 ? stateCode : PMGSYSession.Current.StateCode;
                model.MAST_DISTRICT_CODE = stateCode > 0 ? districtCode : PMGSYSession.Current.DistrictCode;
                blockCode = Convert.ToInt32(locationcodes[2]);
                model.BLOCK_NAME = objDAL.GetBlockName(blockCode);
                model.MAST_BLOCK_CODE = blockCode;
                List<SelectListItem> lstRoadNumber = new List<SelectListItem>();
                lstRoadNumber.Add(new SelectListItem { Value = "A", Text = "--Select Road--", Selected = true });
                //ViewData["RoadFrom"] = model.Road;
                ViewData["RoadFrom"] = new SelectList(lstRoadNumber, "Value", "Text");
                ViewData["RoadTo"] = new SelectList(lstRoadNumber, "Value", "Text");
                //ViewData["RoadTo"] = model.RoadTo;
                ViewData["RoadNumFrom"] = model.RoadNumFromList;
                ViewData["RoadNumTo"] = model.RoadNumToList;
                ViewData["Category"] = model.RoadCategory;
                ViewData["PreviousBlock"] = new SelectList(objDAL.GetBlocksByDistrictCode(PMGSYSession.Current.DistrictCode, blockCode), "MAST_BLOCK_CODE", "MAST_BLOCK_NAME");
                ViewData["PreviousBlockRoadNo"] = new SelectList(lstRoadNumber, "Value", "Text");
                ViewData["RoadNumber"] = new SelectList(lstRoadNumber, "Value", "Text");
                ViewData["HabitationFrom"] = new SelectList(objDAL.GetHabitationsByBlockCode(blockCode), "Value", "Text");
                ViewData["HabitationTo"] = new SelectList(objDAL.GetHabitationsByBlockCode(blockCode), "Value", "Text");
                ViewData["NextBlock"] = new SelectList(objDAL.GetBlocksByDistrictCode(PMGSYSession.Current.DistrictCode, blockCode), "MAST_BLOCK_CODE", "MAST_BLOCK_NAME");
                ViewData["NextBlockRoadNo"] = new SelectList(lstRoadNumber, "Value", "Text");


                List<SelectListItem> lstRoute = new List<SelectListItem>();
                lstRoute.Add(new SelectListItem { Value = "0", Text = "--Select Route--", Selected = true });
                lstRoute.Add(new SelectListItem { Value = "T", Text = "Through Route" });
                if (PMGSYSession.Current.PMGSYScheme == 1)
                {
                    lstRoute.Add(new SelectListItem { Value = "L", Text = "Link Route" });
                }
                // added by rohit for vibrant village prog on 20-07-2023
                else if (PMGSYSession.Current.PMGSYScheme == 2 || PMGSYSession.Current.PMGSYScheme == 5)
                {
                    ///Changes by SAMMED A. PATIL on 30AUG2017 to allow Link Route in PMGSY2
                    lstRoute.Add(new SelectListItem { Value = "L", Text = "Link Route" });
                    lstRoute.Add(new SelectListItem { Value = "M", Text = "Major Rural Link" });
                }
                //return new SelectList(lstRoute,"Value","Text");
                if (PMGSYSession.Current.PMGSYScheme == 1)
                {
                    flag = checkSchedule5(blockCode);
                    if (flag)
                    {
                        lstRoute.Add(new SelectListItem { Value = "N", Text = "Missing Link" });
                    }
                }
                model.RouteType = lstRoute;
                return PartialView("AddEditCoreNetworksVVP", model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return PartialView();
            }
        }



        /// <summary>
        /// saves the inserted details of core network
        /// </summary>
        /// <param name="model">contains the data of Core Network details</param>
        /// <returns>response message along with status</returns>
        [HttpPost]
        [Audit]
        public ActionResult AddCoreNetworksVVP(CoreNetworkViewModel model)
        {
            bool status = false;
            try
            {
                if (ModelState.IsValid)
                {
                    model.MAST_DISTRICT_CODE = model.MAST_DISTRICT_CODE > 0 ? model.MAST_DISTRICT_CODE : PMGSYSession.Current.DistrictCode;
                    model.MAST_STATE_CODE = model.MAST_STATE_CODE > 0 ? model.MAST_STATE_CODE : PMGSYSession.Current.StateCode;

                    if (objBAL.AddCoreNetworksVVP(model, ref message))
                    {
                        if (PMGSYSession.Current.PMGSYScheme == 1)
                        {
                            message = message == string.Empty ? "Core network details added successfully." : message;
                        }
                        // added by rohit for vibrant village prog on 20-07-2023
                        else if (PMGSYSession.Current.PMGSYScheme == 2 || PMGSYSession.Current.PMGSYScheme == 5)
                        {
                            message = message == string.Empty ? "Candidate road details added successfully." : message;
                        }
                        status = true;
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        message = message == string.Empty ? "Error occurred while processing the request." : message;
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
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

                return Json(new { success = false, message = "Error occurred while processing the request." });
                //return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }




        /// <summary>
        /// for getting Road Name of particular Road Code
        /// </summary>
        /// <param name="roadName">road name associated with road code</param>
        /// <param name="blockName">block name</param>
        /// <returns>Road name associated with the particular block</returns>
        [HttpPost]
        [Audit]
        public JsonResult GetRoadNameByRoadCodeVVP(string roadName, string blockName)
        {
            try
            {
                int roadCode = Convert.ToInt32(roadName);
                //int blockCode = PMGSYSession.Current.blockCode;
                int blockCode = Convert.ToInt32(blockName);
                CoreNetworkDAL objDAL = new CoreNetworkDAL();
                List<SelectListItem> lstRoads = objDAL.GetRoadNamesByRoadCodePMGSY3DAL(roadCode, blockCode);
                lstRoads.Insert(0, new SelectListItem { Value = "0", Text = "-Select Road Name-" });
                return Json(lstRoads, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return null;
            }

        }


        /// <summary>
        /// returns the core network details for updation
        /// </summary>
        /// <param name="parameter">Encrypted id</param>
        /// <param name="hash">Encrypted id</param>
        /// <param name="key">Encrypted id</param>
        /// <returns>data of Core Network road for updation</returns>
        [Audit]
        public ActionResult EditCoreNetworksVVP(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptParameters = null;
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            List<SelectListItem> lstRoadNumber = new List<SelectListItem>();
            bool flag = false;
            try
            {
                decryptParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptParameters.Count() > 0)
                {
                    CoreNetworkViewModel model = objBAL.GetCoreNetworkDetails(Convert.ToInt32(decryptParameters["NetworkCode"]));

                    if (decryptParameters.Count > 1)
                    {
                        model.LockUnlockFlag = decryptParameters["UnlockFlag"];
                    }

                    List<SelectListItem> lstRoute = new List<SelectListItem>();
                    lstRoute.Add(new SelectListItem { Value = "0", Text = "--Select Route--", Selected = true });
                    lstRoute.Add(new SelectListItem { Value = "T", Text = "Through Route" });
                    if (PMGSYSession.Current.PMGSYScheme == 1)
                    {
                        lstRoute.Add(new SelectListItem { Value = "L", Text = "Link Route" });
                    }
                    // added by rohit for vibrant village prog on 20-07-2023
                    else if (PMGSYSession.Current.PMGSYScheme == 2 || PMGSYSession.Current.PMGSYScheme == 5)
                    {
                        ///Changes by SAMMED A. PATIL on 30AUG2017 to allow Link Route in PMGSY2
                        lstRoute.Add(new SelectListItem { Value = "L", Text = "Link Route" });
                        lstRoute.Add(new SelectListItem { Value = "M", Text = "Major Rural Link" });
                    }
                    //return new SelectList(lstRoute,"Value","Text");
                    if (PMGSYSession.Current.PMGSYScheme == 1)
                    {
                        flag = checkSchedule5(model.MAST_BLOCK_CODE);
                        if (flag)
                        {
                            lstRoute.Add(new SelectListItem { Value = "N", Text = "Missing Link" });
                        }
                    }
                    model.RouteType = lstRoute;

                    model.FROM_TYPE = model.PLAN_RD_FROM_TYPE;
                    model.TO_TYPE = model.PLAN_RD_TO_TYPE;
                    model.RD_FROM = model.PLAN_RD_FROM;
                    model.RD_TO = model.PLAN_RD_TO;
                    model.ROAD_CODE = db.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == model.MAST_ER_ROAD_CODE).Select(r => r.MAST_ER_ROAD_NUMBER).FirstOrDefault();
                    model.BLOCK_NAME = objDAL.GetBlockName(model.MAST_BLOCK_CODE);//db.MASTER_BLOCK.Where(m => m.MAST_BLOCK_CODE == model.MAST_BLOCK_CODE).Select(b => b.MAST_BLOCK_NAME).FirstOrDefault();
                    if (model.PLAN_RD_NUM_FROM == null)
                    {
                        model.NUM_FROM = null;
                    }
                    else
                    {
                        model.NUM_FROM = model.PLAN_RD_NUM_FROM;
                    }

                    if (model.PLAN_RD_NUM_TO == null)
                    {
                        model.NUM_TO = null;
                    }
                    else
                    {
                        model.NUM_TO = model.PLAN_RD_NUM_TO;
                    }

                    if (model == null)
                    {
                        ModelState.AddModelError(String.Empty, "Core network details not exist.");
                        return PartialView("AddEditCoreNetworksVVP", new CoreNetworkViewModel());
                    }

                    ViewData["RoadFrom"] = model.Road;
                    ViewData["RoadTo"] = model.RoadTo;

                    switch (model.PLAN_RD_FROM_TYPE)
                    {
                        case "T":
                            ViewData["RoadNumFrom"] = new SelectList(objDAL.GetRoadNumFromThroughEditList(model.PLAN_RD_FROM_TYPE, model.MAST_BLOCK_CODE, model.PLAN_CN_ROAD_CODE), "Value", "Text", model.PLAN_RD_NUM_FROM);
                            break;
                        case "L":
                            ViewData["RoadNumFrom"] = new SelectList(objDAL.GetRoadNumFromThroughEditList(model.PLAN_RD_FROM_TYPE, model.MAST_BLOCK_CODE, model.PLAN_CN_ROAD_CODE), "Value", "Text", model.PLAN_RD_NUM_FROM);
                            break;
                        case "M":
                            ViewData["RoadNumFrom"] = new SelectList(objDAL.GetRoadNumFromThroughEditList(model.PLAN_RD_FROM_TYPE, model.MAST_BLOCK_CODE, model.PLAN_CN_ROAD_CODE), "Value", "Text", model.PLAN_RD_NUM_FROM);
                            break;
                        case "H":
                            ViewData["RoadNumFrom"] = model.RoadNumFromList;
                            break;
                        case "B":
                            ViewData["RoadNumFrom"] = model.RoadNumFromList;
                            break;
                        default:
                            ViewData["RoadNumFrom"] = new SelectList(objDAL.GetRoadNumFromByRoadFrom(model.PLAN_RD_FROM_TYPE, model.MAST_BLOCK_CODE), "MAST_ER_ROAD_CODE", "MAST_ER_ROAD_NAME", model.PLAN_RD_NUM_FROM);
                            break;
                    }

                    switch (model.PLAN_RD_TO_TYPE)
                    {
                        case "T":
                            ViewData["RoadNumTo"] = new SelectList(objDAL.GetRoadNumFromThroughList(model.PLAN_RD_TO_TYPE, model.MAST_BLOCK_CODE), "PLAN_CN_ROAD_CODE", "PLAN_CN_ROAD_NUMBER", model.PLAN_RD_NUM_TO);
                            break;
                        case "L":
                            ViewData["RoadNumTo"] = new SelectList(objDAL.GetRoadNumFromThroughList(model.PLAN_RD_TO_TYPE, model.MAST_BLOCK_CODE), "PLAN_CN_ROAD_CODE", "PLAN_CN_ROAD_NUMBER", model.PLAN_RD_NUM_TO);
                            break;
                        case "M":
                            ViewData["RoadNumTo"] = new SelectList(objDAL.GetRoadNumFromThroughList(model.PLAN_RD_TO_TYPE, model.MAST_BLOCK_CODE), "PLAN_CN_ROAD_CODE", "PLAN_CN_ROAD_NUMBER", model.PLAN_RD_NUM_TO);
                            break;
                        case "H":
                            ViewData["RoadNumTo"] = model.RoadNumToList;
                            break;
                        case "B":
                            ViewData["RoadNumTo"] = model.RoadNumToList;
                            break;
                        default:
                            ViewData["RoadNumTo"] = new SelectList(objDAL.GetRoadNumFromByRoadFrom(model.PLAN_RD_TO_TYPE, model.MAST_BLOCK_CODE), "MAST_ER_ROAD_CODE", "MAST_ER_ROAD_NAME", model.PLAN_RD_NUM_TO);
                            break;
                    }
                    if (model.PLAN_RD_ROUTE == "N")
                    {
                        List<string> lst = objDAL.MLRoadList(model.MAST_BLOCK_CODE);
                        List<SelectListItem> lstRoadNumberML = new List<SelectListItem>();
                        lstRoadNumberML.Insert(0, new SelectListItem { Value = "A", Text = "--Select Road No--" });
                        for (int i = 1; i <= 30; i++)
                        {
                            if (i < 10)
                            {
                                lstRoadNumberML.Add(new SelectListItem { Value = "ML0" + i.ToString(), Text = "ML0" + i.ToString() });
                            }
                            else
                            {
                                lstRoadNumberML.Add(new SelectListItem { Value = "ML" + i.ToString(), Text = "ML" + i.ToString() });
                            }
                        }

                        var list = (from item in lstRoadNumberML
                                    where !lst.Contains(item.Value)
                                    select new
                                    {
                                        item.Value,
                                        item.Text
                                    }).Distinct().ToList().Select(x => new SelectListItem
                                    {
                                        Text = x.Text,
                                        Value = x.Value
                                    }).ToList();

                        list.Add(new SelectListItem { Value = model.PLAN_CN_ROAD_NUMBER, Text = model.PLAN_CN_ROAD_NUMBER });
                        //list.Insert(list.Count, SelectListItem { Value = model.PLAN_CN_ROAD_NUMBER, Text = model.PLAN_CN_ROAD_NUMBER });
                        //new SelectList(list, "Value", "Text")

                        ViewData["RoadNumber"] = list;
                    }
                    else
                    {
                        JsonResult a = GetRoadNumberByRoadRoute(model.PLAN_RD_ROUTE, model.MAST_BLOCK_CODE, model.PLAN_CN_ROAD_CODE);
                        ViewData["RoadNumber"] = a.Data;
                    }
                    ViewData["RoadNumThroughFrom"] = model.RoadFrom;
                    ViewData["RoadNumThroughTo"] = model.RoadFrom;
                    //ViewData["RoadNumThroughTo"] = new SelectList(lstRoadNumber, "Value", "Text");
                    ViewBag.RoadCategory = objDAL.GetRoadCategory(model.MAST_ER_ROAD_CODE);
                    ViewData["PreviousBlock"] = new SelectList(objDAL.GetBlocksByDistrictCode(PMGSYSession.Current.DistrictCode), "MAST_BLOCK_CODE", "MAST_BLOCK_NAME");
                    ViewData["PreviousBlockRoadNo"] = new SelectList(objDAL.GetPreviousBlockByBlockCode(model.MAST_BLOCK_CODE), "PLAN_CN_ROAD_CODE", "PLAN_CN_ROAD_NUMBER");
                    //ViewData["RoadNumber"] = new SelectList(lstRoadNumber, "Value", "Text");
                    ViewData["HabitationFrom"] = new SelectList(objDAL.GetHabitationsByBlockCode(model.MAST_BLOCK_CODE), "Value", "Text");
                    ViewData["HabitationTo"] = new SelectList(objDAL.GetHabitationsByBlockCode(model.MAST_BLOCK_CODE), "Value", "Text");
                    ViewData["NextBlock"] = new SelectList(objDAL.GetBlocksByDistrictCode(PMGSYSession.Current.DistrictCode), "MAST_BLOCK_CODE", "MAST_BLOCK_NAME");
                    ViewData["NextBlockRoadNo"] = new SelectList(objDAL.GetPreviousBlockByBlockCode(model.MAST_BLOCK_CODE), "PLAN_CN_ROAD_CODE", "PLAN_CN_ROAD_NUMBER");
                    return PartialView("AddEditCoreNetworksVVP", model);
                }
                return PartialView("AddEditCoreNetworksVVP", new CoreNetworkViewModel());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return Json(new { success = false });
            }
        }

        /// <summary>
        /// save the updated details of core network
        /// </summary>
        /// <param name="model">contains the updated Core Network details</param>
        /// <returns>response message along with status of operation</returns>
        [HttpPost]
        [Audit]
        public ActionResult EditCoreNetworksVVP(CoreNetworkViewModel model)
        {
            bool status = false;
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            try
            {
                if (ModelState.IsValid)
                {
                    if (objBAL.EditCoreNetworksVVP(model, ref message))
                    {
                        if (PMGSYSession.Current.PMGSYScheme == 1)
                        {
                            message = message == string.Empty ? "Core network details updated successfully." : message;
                        }
                        // added by rohit for vibrant village prog on 20-07-2023
                        else if (PMGSYSession.Current.PMGSYScheme == 2 || PMGSYSession.Current.PMGSYScheme == 5)
                        {
                            message = message == string.Empty ? "Candidate road details updated successfully." : message;
                        }
                        status = true;
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        message = message == string.Empty ? "Error occurred while processing your request." : message;
                        ModelState.AddModelError("", message);
                        return Json(new { success = status, message = new CommonFunctions().FormatErrorMessage(ModelState) }, JsonRequestBehavior.AllowGet);
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

                message = message == string.Empty ? "Error occurred while processing your request." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }



        /// <summary>
        /// returns the view for mapping the other candidate road
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public ActionResult MapOtherCandidateRoadViewVVP(String parameter, String hash, String key)
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                CoreNetworkDAL objDAL = new CoreNetworkDAL();
                CandidateRoadViewModel model = new CandidateRoadViewModel();
                if (!(String.IsNullOrEmpty(parameter)) && !(String.IsNullOrEmpty(hash)) && !(String.IsNullOrEmpty(key)))
                {
                    Dictionary<string, string> decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                    if (decryptedParameters != null)
                    {
                        model.CNCode = Convert.ToInt32(decryptedParameters["NetworkCode"]);
                    }
                }
                model.LockStatus = objDAL.GetLockStatusOfCandidateRoad(model.CNCode);
                //model.lstBlocks = objCommon.PopulateBlocks(PMGSYSession.Current.DistrictCode);
                model.lstBlocks = new SelectList(objDAL.GetBlocksByDistrictCode(PMGSYSession.Current.DistrictCode), "MAST_BLOCK_CODE", "MAST_BLOCK_NAME").ToList();
                model.lstBlocks.Insert(0, new SelectListItem { Value = "0", Text = "Select Block" });
                model.lstRoadCategory = new SelectList(objDAL.GetAllRoadCategories().Where(m => m.MAST_ROAD_SHORT_DESC == "RR(ODR)" || m.MAST_ROAD_SHORT_DESC == "RR(VR)" || m.MAST_ROAD_SHORT_DESC == "RR(TRACK)").ToList(), "MAST_ROAD_CAT_CODE", "MAST_ROAD_CAT_NAME").ToList();
                model.lstRoadCategory.Insert(0, new SelectListItem { Value = "0", Text = "- Select Road Category -" });
                model.lstDRRP.Insert(0, new SelectListItem { Value = "0", Text = "- Select Road Name -" });
                return PartialView(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MapOtherCandidateRoadViewVVP()");
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }


        [HttpPost]
        public ActionResult FinalizeMappedDRRPDetailsVVP(String id)
        {
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            int CNCode = 0;
            string message = string.Empty;
            try
            {
                if (!String.IsNullOrEmpty(id))
                {
                    CNCode = Convert.ToInt32(id);
                    if (objBAL.FinalizeMappedDRRPDetails(CNCode))
                    {
                        return Json(new { success = true });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Error occurred while processing your request." });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "TR/MRL Road details are not present." });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.FinalizeMappedDRRPDetailsVVP()");
                return Json(new { success = false, message = "Error on DRRP details finalize." });
            }
        }


        [HttpPost]
        public JsonResult DeleteMappedDRRPDetailsVVP(String parameter, String hash, String key)
        {
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            int DRRPCode = 0;
            int CNCode = 0;
            try
            {
                if (!(String.IsNullOrEmpty(parameter)) && !(String.IsNullOrEmpty(hash)) && !(String.IsNullOrEmpty(key)))
                {
                    Dictionary<string, string> decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                    if (decryptedParameters != null)
                    {
                        DRRPCode = Convert.ToInt32(decryptedParameters["DRRPCode"]);
                        CNCode = Convert.ToInt32(decryptedParameters["CNCode"]);
                    }
                }

                if (!(objDAL.DeleteMappedDRRPDetailsVVP(DRRPCode, CNCode)))
                {
                    return Json(new { success = false });
                }
                else
                {
                    return Json(new { success = true });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CoreNetwork.DeleteMappedDRRPDetailsPMGSY3()");
                return Json(new { success = false });
            }
        }


        /// <summary>
        /// returns the Details view of Core Network
        /// </summary>
        /// <param name="parameter">Encrypted id</param>
        /// <param name="hash">Encrypted id</param>
        /// <param name="key">Encrypted id</param>
        /// <returns>Details view of Core Network</returns>
        [Audit]
        public ActionResult DetailsCoreNetworkVVP(String parameter, String hash, String key)
        {
            Dictionary<string, string> parameters = null;
            CoreNetworkViewModel model = new CoreNetworkViewModel();
            CoreNetworkDAL objDAL = new CoreNetworkDAL();
            try
            {
                parameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (parameters != null)
                {
                    model = objBAL.GetCoreNetworkDetails(Convert.ToInt32(parameters["NetworkCode"]));
                    model.ROAD_CODE = db.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == model.MAST_ER_ROAD_CODE).Select(r => r.MAST_ER_ROAD_NUMBER).FirstOrDefault();
                    return PartialView("DetailsCoreNetworkVVP", model);
                }
                else
                {
                    return PartialView("DetailsCoreNetworkVVP", new CoreNetworkViewModel());
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return Json(new { success = false, message = "Error occurred while processing the request." });
                //return PartialView("DetailsCoreNetwork", new CoreNetworkViewModel());
            }
            finally
            {
                db.Dispose();
            }

        }


        /// <summary>
        /// list view of Habitations
        /// </summary>
        /// <param name="parameter">Encrypted id</param>
        /// <param name="hash">Encrypted id</param>
        /// <param name="key">Encrypted id</param>
        /// <returns>list view of Habitation details</returns>
        [Audit]
        public ActionResult ListHabitationsVVP(String parameter, String hash, String key)
        {
            try
            {
                HabitationDetailsViewModel model = new HabitationDetailsViewModel();
                CoreNetworkDAL objDAL = new CoreNetworkDAL();
                CommonFunctions objCommon = new CommonFunctions();
                Dictionary<string, string> decryptedParameters = null;
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                int networkCode = Convert.ToInt32(decryptedParameters["NetworkCode"]);
                model.UnlockFlag = decryptedParameters["UnlockFlag"];
                model.EncryptedRoadCode = networkCode.ToString();
                ViewBag.RoadNumber = db.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == networkCode).Select(m => m.PLAN_CN_ROAD_NUMBER).FirstOrDefault();

                ViewData["Habitations"] = objDAL.GetHabitationCodeList(networkCode);

                if (PMGSYSession.Current.PMGSYScheme == 1)
                {
                    ViewBag.Blocks = objCommon.PopulateBlocks(PMGSYSession.Current.DistrictCode, false);
                }
                if (PMGSYSession.Current.PMGSYScheme == 2 || PMGSYSession.Current.PMGSYScheme == 5)
                {
                    ViewBag.Roads = objDAL.GetRoadsByCNCodeCandidate(networkCode);
                }
                return PartialView("ListHabitationsVVP", model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occurred while processing the request" });
            }

        }

        /// <summary>
        /// returns the list of habitations mapped with the road
        /// </summary>
        /// <param name="mapCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetHabitationListToMapVVP(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                int roadCode = 0, erRoadCode = 0;
                int blockCode = 0;
                long totalRecords = 0;

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["habCode"]))
                {
                    roadCode = Convert.ToInt32(Request.Params["habCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["erRoadCode"]))
                {
                    erRoadCode = Convert.ToInt32(Request.Params["erRoadCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["blockCode"]))
                {
                    blockCode = Convert.ToInt32(Request.Params["blockCode"]);
                }

                var jsonData = new
                {
                    rows = objBAL.GetHabitationListToMapVVP(roadCode, blockCode, erRoadCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
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
        /// Post method for adding the mapping details of candidate road
        /// </summary>
        /// <param name="model">model containing the mapping details</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MapCandidateRoadVVP(CandidateRoadViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (objBAL.MapCandidateRoadVVP(model, ref message))
                    {
                        message = message == string.Empty ? "DRRP details mapped successfully." : message;
                        return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        message = message == string.Empty ? "Error occurred while processing your request." : message;
                        ModelState.AddModelError("", message);
                        return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MapCandidateRoadVVP()");
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }


        #endregion
    }

    public class HabitationList
    {
        public int count;
        public int MAST_HAB_CODE;
        public string MAST_HAB_NAME;
        public int MAST_HAB_TOT_POP;

        public HabitationList()
        {
            count = 0;
        }

    }
}
