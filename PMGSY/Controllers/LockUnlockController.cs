#region HEADER
/*
* Project Id:

 * Project Name:OMMAS-II

 * File Name: LockUnlockController.cs

 * Author :  Vikram Nandanwar

 * Creation Date :05/June/2013

 * Desc : This class is used as controller  to perform Save,Edit,Update,Delete and listing of Lock Unlock screens.  
 */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Models.LockUnlock;
using PMGSY.Common;
using PMGSY.Extensions;
using PMGSY.BAL.LockUnlock;
using PMGSY.DAL.LockUnlock;
namespace PMGSY.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class LockUnlockController : Controller
    {

        public LockUnlockController()
        {
            PMGSYSession.Current.ModuleName = "Lock / Unlock";
        }

        ILockUnlockBAL objLockUnlockBAL = new LockUnlockBAL();
        LockUnlockDAL objDAL = new LockUnlockDAL();
        String message = string.Empty;


        #region FREEZE_UNFREEZE

        /// <summary>
        /// FreezeUnfreezeProposal() Action is used to show State,Year,Batch Filter and Proposal Grid
        /// </summary>
        /// <returns>
        /// </returns>
        [Audit]
        public ActionResult FreezeUnfreezeProposal()
        {
            ProposalFilterLockUnlockViewModel proposalFilterLockUnlockViewModel = new ProposalFilterLockUnlockViewModel();
            CommonFunctions objCommonFunctions = new CommonFunctions();

            proposalFilterLockUnlockViewModel.BATCHS = objCommonFunctions.PopulateBatch();
            proposalFilterLockUnlockViewModel.STATES = objCommonFunctions.PopulateStates();
            proposalFilterLockUnlockViewModel.Years = PopulateYear();

            proposalFilterLockUnlockViewModel.IMS_YEAR = DateTime.Now.Year;

            return View("FreezeUnfreezeProposal", proposalFilterLockUnlockViewModel);
        }

        /// <summary>
        /// FreezeProposal() Action is used to Freeze/Unfreeze Batch
        /// </summary>
        /// <param name="LockUnlockViewModel"></param>
        /// <returns>
        /// 1) if batch is successfully freeze/Unfreeze returns success message
        /// 2) if batch is not freeze/Unfreeze returns error message
        /// </returns>
        [HttpPost]
        [Audit]
        public ActionResult FreezeProposal(ProposalFilterLockUnlockViewModel LockUnlockViewModel)
        {
            LockUnlockViewModel.IMS_YEAR = Convert.ToInt32(LockUnlockViewModel.YearCode);
            LockUnlockViewModel.IMS_BATCH = Convert.ToInt32(LockUnlockViewModel.BatchCode);
            LockUnlockViewModel.MAST_STATE_CODE = Convert.ToInt32(LockUnlockViewModel.StateCode);

            bool status = false;
            try
            {
                LockUnlockViewModel.PMGSYScheme = PMGSYSession.Current.PMGSYScheme;
                if (ModelState.IsValid)
                {
                    if (objLockUnlockBAL.FreezeUnfreezeProposal(LockUnlockViewModel, ref message))
                    {
                        if (LockUnlockViewModel.FreezeStatus == "U")
                        {
                            message = message == string.Empty ? "Batch is Freezed successfully." : message;
                        }
                        else
                        {
                            message = message == string.Empty ? "Batch is Unfreezed successfully." : message;
                        }
                        status = true;
                    }
                    else
                    {
                        if (LockUnlockViewModel.FreezeStatus == "U")
                        {
                            message = message == string.Empty ? "Batch is not Freezed." : message;
                        }
                        else
                        {
                            message = message == string.Empty ? "Batch is not UnFreezed." : message;
                        }
                    }
                }
                else
                {
                    string messages = string.Join("; ", ModelState.Values
                                              .SelectMany(x => x.Errors)
                                              .Select(x => x.ErrorMessage));

                    return Json(new { success = status, message = messages }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "");
                if (LockUnlockViewModel.FreezeStatus == "U")
                {
                    message = "Batch is not Freezed.";
                }
                else
                {
                    message = "Batch is not UnFreezed.";
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// GetLockUnlockList() Action is used to show Proposal List on grid
        /// </summary>
        /// <returns>
        /// Proposal list in filtered by Year,State and Batch wise. and json data is return to jqGrid
        /// </returns>
        [Audit]
        public ActionResult GetLockUnlockList(int? page, int? rows, string sidx, string sord)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }
            int IMS_YEAR = Convert.ToInt32(Request.Params["IMS_YEAR"]);
            int IMS_BATCH = Convert.ToInt32(Request.Params["IMS_BATCH"]);
            int IMS_MAST_STATE_CODE = Convert.ToInt32(Request.Params["IMS_MAST_STATE_CODE"]);
            int Scheme = Convert.ToInt32(Request.Params["Scheme"]);
            int totalRecords;
            bool IsSOGenerated = false;
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters((new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"])))))
                {
                    return null;
                }
            }

            IsSOGenerated = objDAL.IsSanctionOrderGenerated(IMS_MAST_STATE_CODE, IMS_BATCH, IMS_YEAR, Scheme);

            var jsonData = new
            {
                rows = objLockUnlockBAL.GetProposalsBAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, IMS_YEAR, IMS_MAST_STATE_CODE, IMS_BATCH, Scheme),
                total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                page = Convert.ToInt32(page),
                records = totalRecords,
                IsSOGenerated = IsSOGenerated
            };
            return Json(jsonData);
        }

        /// <summary>
        /// PopulateYear() Action is used to Populate Years from 2000 to till date
        /// </summary>
        /// <returns>
        /// Return Year select list which is used to Populate Year Drop down
        /// </returns>
        public List<SelectListItem> PopulateYear()
        {
            List<SelectListItem> lstYears = new List<SelectListItem>();
            SelectListItem item = new SelectListItem();

            item.Text = "Select Year";
            item.Value = "0";
            //item.Selected=true;
            lstYears.Add(item);

            for (int i = 2000; i <= DateTime.Now.Year; ++i)
            {
                item = new SelectListItem();
                item.Text = i + " - " + (i + 1);
                item.Value = i.ToString();

                lstYears.Add(item);
            }

            return lstYears;
        }//year populate


        public ActionResult ListBatchFreezeView()
        {
            CommonFunctions objCommon = new CommonFunctions();
            List<SelectListItem> lstDemo = new List<SelectListItem>();
            lstDemo.Insert(0, new SelectListItem { Value = "0", Text = "Select Scheme" });
            lstDemo.Insert(1, new SelectListItem { Value = "1", Text = "PMGSY-1" });
            lstDemo.Insert(2, new SelectListItem { Value = "2", Text = "PMGSY-2" });
            ///Changes for RCPLWE
            lstDemo.Insert(3, new SelectListItem { Value = "3", Text = "RCPLWE" });

            FreezeUnfreezeReportModel model = new FreezeUnfreezeReportModel();
            model.lstScheme = lstDemo;
            model.BATCHS = objCommon.PopulateBatch();
            model.BATCHS.RemoveAt(0);
            model.BATCHS.Insert(0, new SelectListItem { Value = "0", Text = "All Batches" });
            model.Years = objCommon.PopulateYears(true);
            model.Years.RemoveAt(0);
            model.Years.Insert(0, new SelectListItem { Value = "0", Text = "All Years" });
            model.STATES = objCommon.PopulateStates(true);
            return View(model);
        }

        [HttpPost]
        public ActionResult GetFreezeUnfreezeReportDetails(int? page, int? rows, string sidx, string sord)
        {
            long totalRecords = 0;
            int stateCode = 0;
            int batchCode = 0;
            int yearCode = 0;
            int schemeCode = 0;
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!String.IsNullOrEmpty(Request.Params["StateCode"]))
                {
                    stateCode = Convert.ToInt32(Request.Params["StateCode"]);
                }

                if (!String.IsNullOrEmpty(Request.Params["BatchCode"]))
                {
                    batchCode = Convert.ToInt32(Request.Params["BatchCode"]);
                }

                if (!String.IsNullOrEmpty(Request.Params["YearCode"]))
                {
                    yearCode = Convert.ToInt32(Request.Params["YearCode"]);
                }

                if (!String.IsNullOrEmpty(Request.Params["PMGSYScheme"]))
                {
                    schemeCode = Convert.ToInt32(Request.Params["PMGSYScheme"]);
                }

                var jsonData = new
                {
                    rows = objLockUnlockBAL.GetFreezeUnfreezeReportDetails(stateCode, batchCode, yearCode, schemeCode, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                    page = Convert.ToInt32(Request.Params["page"]),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);


            }
            catch (Exception)
            {
                return null;
            }
        }


        #endregion


        #region LOCK_UNLOCK

        #region PROPOSAL

        //GET:
        [Audit]
        public ActionResult ListLockUnlockDetails()
        {
            List<SelectListItem> lstDemo = new List<SelectListItem>();
            lstDemo.Add(new SelectListItem { Value = "0", Text = "--Select--" });
            ViewData["Module"] = new SelectList(objDAL.GetAllModules(), "Value", "Text");
            ViewData["Submodule"] = new SelectList(lstDemo, "Value", "Text");
            ViewData["Level"] = new SelectList(lstDemo, "Value", "Text");
            return View("ListLockUnlockDetails");
        }

        [HttpPost]
        public JsonResult GetSubmoduleByModuleCode(int moduleCode)
        {
            try
            {
                List<SelectListItem> lstSubmodule = objDAL.GetSubmoduleByModuleCode(moduleCode);
                return Json(new SelectList(lstSubmodule, "Value", "Text"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        //GET:
        /// <summary>
        /// search view of proposal module 
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult SearchProposal()
        {
            ProposalFilterViewModel model = new ProposalFilterViewModel();
            model.State = PMGSYSession.Current.StateCode;
            model.District = PMGSYSession.Current.DistrictCode;
            List<SelectListItem> lstDemo = new List<SelectListItem>();
            lstDemo.Add(new SelectListItem { Value = "0", Text = "--Select--" });
            if (PMGSYSession.Current.RoleCode == 2)
            {
                ViewData["State"] = new SelectList(objDAL.GetAllStates(true, model.State), "Value", "Text", model.State);
                ViewData["District"] = new SelectList(objDAL.GetAllDistrictsByStateCode(PMGSYSession.Current.StateCode), "Value", "Text");
            }
            else
            {
                ViewData["State"] = new SelectList(objDAL.GetAllStates(false, model.State), "Value", "Text", model.State);
                ViewData["District"] = new SelectList(lstDemo, "Value", "Text");
            }
            ViewData["Year"] = new SelectList(objDAL.GetAllYears(), "Value", "Text");

            ViewData["Batch"] = new SelectList(objDAL.GetAllBatches(), "Value", "Text");
            ViewData["Package"] = new SelectList(lstDemo, "Value", "Text");
            return PartialView("SearchProposal", model);
        }

        /// <summary>
        /// Returns the List of Proposal details
        /// </summary>
        /// <param name="proposalCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetProposalList(int? page, int? rows, string sidx, string sord)
        {
            int yearCode = 0;
            int stateCode = 0;
            int districtCode = 0;
            int batchCode = 0;
            string packageCode = string.Empty;
            long totalRecords = 0;
            int blockCode = 0;
            byte scheme = 0;
            int roleCode = 0;
            string type = string.Empty;
            List<int> lstIds;
            string ImsPrRoadCode;
            int collaboration = 0;
            string propType = string.Empty;

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

            if (!string.IsNullOrEmpty(Request.Params["batchCode"]))
            {
                batchCode = Convert.ToInt32(Request.Params["batchCode"]);
            }

            if (!string.IsNullOrEmpty(Request.Params["packageCode"]))
            {
                packageCode = Request.Params["packageCode"];
            }

            if (!string.IsNullOrEmpty(Request.Params["scheme"]))
            {
                scheme = Convert.ToByte(Request.Params["scheme"]);
            }

            if (!string.IsNullOrEmpty(Request.Params["type"]))
            {
                type = (Request.Params["type"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["RoleCode"]))
            {
                roleCode = Convert.ToInt32(Request.Params["roleCode"]);
            }
            ///Changes for Collaboration
            if (!string.IsNullOrEmpty(Request.Params["collaboration"]))
            {
                collaboration = Convert.ToInt32(Request.Params["collaboration"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["proposalType"]))
            {


                // Added to unlock Proposal Technology Details
                // Old Code
                //propType = Request.Params["proposalType"].Trim() == "2" ? "PR" : "PH";

                //New Code // if prop type = 2 then PR else if propo type 10 then PT i.e.(Proposal Technology)
                //propType = Request.Params["proposalType"].Trim() == "2" ? "PR" : Request.Params["proposalType"].Trim() == "10" ? "PT" : "PH";

                // if prop type = 2 then PR else if propo type 10 then PT i.e.(Proposal Technology) else if propo type 11 then CP
                propType = Request.Params["proposalType"].Trim() == "2" ? "PR" : Request.Params["proposalType"].Trim() == "10" ? "PT" : Request.Params["proposalType"].Trim() == "11" ? "CP" : "PH";

            }

            var jsonData = new
            {
                rows = objLockUnlockBAL.GetProposalList(propType, yearCode, stateCode, districtCode, blockCode, batchCode, packageCode, scheme, collaboration, roleCode, type, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords, out lstIds, out ImsPrRoadCode),
                total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                page = Convert.ToInt32(Request.Params["page"]),
                records = totalRecords,
                ids = lstIds,
                RoadCodes = ImsPrRoadCode,

            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// returns the lock proposal list
        /// </summary>
        /// <param name="page">No. of pages</param>
        /// <param name="rows">No. of rows</param>
        /// <param name="sidx">sord column name</param>
        /// <param name="sord">sort order</param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetProposalUnLockList(int? page, int? rows, string sidx, string sord)
        {
            int yearCode = 0;
            int stateCode = 0;
            int districtCode = 0;
            int batchCode = 0;
            string packageCode = string.Empty;
            long totalRecords = 0;
            List<int> lstIds;
            string ImsPrRoadCode;

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

            if (!string.IsNullOrEmpty(Request.Params["stateCode"]))
            {
                stateCode = Convert.ToInt32(Request.Params["stateCode"]);
            }

            if (!string.IsNullOrEmpty(Request.Params["districtCode"]))
            {
                districtCode = Convert.ToInt32(Request.Params["districtCode"]);
            }

            if (!string.IsNullOrEmpty(Request.Params["batchCode"]))
            {
                batchCode = Convert.ToInt32(Request.Params["batchCode"]);
            }

            if (!string.IsNullOrEmpty(Request.Params["packageCode"]))
            {
                packageCode = Request.Params["packageCode"];
            }

            var jsonData = new
            {
                rows = objLockUnlockBAL.GetProposalLockList(yearCode, stateCode, districtCode, batchCode, packageCode, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords, out lstIds, out ImsPrRoadCode),
                total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                page = Convert.ToInt32(Request.Params["page"]),
                records = totalRecords,
                ids = lstIds,
                RoadCodes = ImsPrRoadCode,

            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// returns the districts by state
        /// </summary>
        /// <param name="stateCode"></param>
        /// <returns></returns>
        //[HttpPost]
        [Audit]
        public JsonResult GetDistrictsByStateCode(int stateCode)
        {
            CommonFunctions objCommon = new CommonFunctions();
            try
            {
                if (stateCode == 0)
                {

                    List<SelectListItem> lstDistricts = new List<SelectListItem>();
                    lstDistricts.Add(new SelectListItem { Value = "0", Text = "--Select District--" });
                    return Json(new SelectList(lstDistricts, "Value", "Text"), JsonRequestBehavior.AllowGet);


                }
                else
                {
                    if (PMGSYSession.Current.RoleCode != 47) //ITNOOA=47 RoleCode
                    {
                        List<SelectListItem> lstDistricts = objCommon.PopulateDistrict(stateCode, true);
                        return Json(new SelectList(lstDistricts, "Value", "Text"), JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        //List<SelectListItem> districtList = new List<SelectListItem>();

                        //LockUnlockDAL lockDAL = new LockUnlockDAL();

                        //districtList = lockDAL.GetAllDistrictsByAdminNDCode(stateCode, PMGSYSession.Current.AdminNdCode);
                        //districtList.Insert(0, new SelectListItem { Value = "0", Text = "--Select District--" });

                        //return Json(new SelectList(districtList, "Value", "Text"), JsonRequestBehavior.AllowGet);                      
                        List<SelectListItem> districtList = new List<SelectListItem>();
                        districtList = objCommon.GetAllDistrictsByAdminNDCode(stateCode, PMGSYSession.Current.AdminNdCode);
                        districtList.Insert(0, new SelectListItem { Value = "0", Text = "All District" });
                        return Json(new SelectList(districtList, "Value", "Text"), JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        //GET:
        /// <summary>
        /// returns the level dropdown values
        /// </summary>
        /// <param name="moduleCode"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult GetModuleLevelByModuleCode(int moduleCode)
        {
            try
            {
                List<SelectListItem> lstSubmodule = objDAL.GetModuleLevelByModuleCode(moduleCode);
                return Json(new SelectList(lstSubmodule, "Value", "Text"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        //GET:
        /// <summary>
        /// returns Lock Details View
        /// </summary>
        /// <param name="urlparameter"></param>
        /// <param name="moduleCode"></param>
        /// <param name="subModuleCode"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult AddLockDetails(string urlparameter, int moduleCode, string subModuleCode)
        {
            LockDetailsViewModel objModel = new LockDetailsViewModel();
            String[] encryptedParameters = null;
            Dictionary<string, string> decryptedParameters = null;

            if (urlparameter != string.Empty)
            {
                encryptedParameters = urlparameter.Split('/');

                if (encryptedParameters.Length == 3)
                {
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                }
            }

            switch (moduleCode)
            {
                case 1:
                    int proposalCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);
                    objModel.IMS_PR_ROAD_CODE = proposalCode;
                    objModel.IMS_UNLOCK_TABLE = "IR";
                    break;
                case 2:
                    int existingRoadCode = Convert.ToInt32(decryptedParameters["ExistingCode"]);
                    objModel.MAST_ER_ROAD_CODE = existingRoadCode;
                    objModel.IMS_UNLOCK_TABLE = "ER";
                    break;
                case 3:
                    int planRoadCode = Convert.ToInt32(decryptedParameters["RoadCode"]);
                    objModel.PLAN_CN_ROAD_CODE = planRoadCode;
                    objModel.IMS_UNLOCK_TABLE = "PR";
                    break;
                case 4:
                    int agreementCode = Convert.ToInt32(decryptedParameters["AgreementCode"]);
                    objModel.TEND_AGREEMENT_CODE = agreementCode;
                    objModel.IMS_UNLOCK_TABLE = "AD";
                    break;
                case 5:
                    int tenderingCode = Convert.ToInt32(decryptedParameters["TenderingCode"]);
                    objModel.TEND_NIT_NO = tenderingCode;
                    objModel.IMS_UNLOCK_TABLE = "NT";
                    break;
                case 6:
                    int proposalContractCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);
                    int contractCode = Convert.ToInt32(decryptedParameters["ContractCode"]);
                    objModel.IMS_PR_ROAD_CODE = proposalContractCode;
                    objModel.MANE_CONTRACT_CODE = contractCode;
                    objModel.IMS_UNLOCK_TABLE = "AP";
                    break;
                case 7:
                    int roadContractCode = Convert.ToInt32(decryptedParameters["RoadCode"]);
                    int contractRoadCode = Convert.ToInt32(decryptedParameters["ContractCode"]);
                    objModel.IMS_PR_ROAD_CODE = roadContractCode;
                    objModel.MANE_CONTRACT_CODE = contractRoadCode;
                    objModel.IMS_UNLOCK_TABLE = "AC";
                    break;
                default:
                    break;
            }

            switch (PMGSYSession.Current.RoleCode)
            {
                case 2:
                    objModel.IMS_UNLOCK_BY = "S";
                    break;
                case 25:
                    objModel.IMS_UNLOCK_BY = "M";
                    break;
                case 22:
                    objModel.IMS_UNLOCK_BY = "D";
                    break;
                case 8:
                    objModel.IMS_UNLOCK_BY = "N";
                    break;
            }

            return PartialView("AddLockDetails", objModel);
        }


        /// <summary>
        /// Add Lock details according to specific data
        /// </summary>
        /// <param name="lockModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult AddLockDetails(LockDetailsViewModel lockModel)
        {
            string message = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    if (objLockUnlockBAL.AddLockDetails(lockModel, ref message))
                    {
                        message = (message == string.Empty ? "Unlock details added successfully." : message);
                        return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        message = message == string.Empty ? "Error occurred while processing your request." : message;
                        return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    string messages = string.Join("; ", ModelState.Values
                                              .SelectMany(x => x.Errors)
                                              .Select(x => x.ErrorMessage));

                    return Json(new { success = false, message = messages }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Error occurred while processing your request." : message;
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        //GET:
        /// <summary>
        /// returns Lock Details View for Updating records in batch
        /// </summary>
        /// <param name="moduleCode"></param>
        /// <param name="subModuleCode"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult AddLockDetailsBatch(int moduleCode, string subModuleCode)
        {
            LockDetailsViewModel model = new LockDetailsViewModel();

            if (Request.Params["batchCode"] != string.Empty)
            {
                model.BatchCode = Convert.ToInt32(Request.Params["batchCode"]);
            }
            if (Request.Params["districtCode"] != string.Empty)
            {
                model.DistrictCode = Convert.ToInt32(Request.Params["districtCode"]);
            }
            if (Request.Params["blockCode"] != string.Empty)
            {
                model.BlockCode = Convert.ToInt32(Request.Params["blockCode"]);
            }
            model.YearCode = Convert.ToInt32(Request.Params["yearCode"]);
            model.StateCode = Convert.ToInt32(Request.Params["stateCode"]);
            model.PackageCode = Request.Params["packageCode"];
            model.LockStatus = Request.Params["lockStatus"];
            string ids = (Request.Params["ids[]"]);
            string[] dataIDs = (ids.Split(','));
            model.DataID = dataIDs.Select(m => int.Parse(m)).ToArray(); //dataIDs;

            switch (moduleCode)
            {
                case 1:
                    model.IMS_UNLOCK_TABLE = "IR";
                    break;
                case 2:
                    model.IMS_UNLOCK_TABLE = "ER";
                    break;
                case 3:
                    model.IMS_UNLOCK_TABLE = "PR";
                    break;
                case 4:
                    model.IMS_UNLOCK_TABLE = "AD";
                    break;
                case 5:
                    model.IMS_UNLOCK_TABLE = "NT";
                    break;
                case 6:
                    model.IMS_UNLOCK_TABLE = "AP";
                    break;
                case 7:
                    model.IMS_UNLOCK_TABLE = "AC";
                    break;
                default:
                    break;
            }

            switch (PMGSYSession.Current.RoleCode)
            {
                case 2:
                    model.IMS_UNLOCK_BY = "S";
                    break;
                case 25:
                    model.IMS_UNLOCK_BY = "M";
                    break;
                case 22:
                    model.IMS_UNLOCK_BY = "D";
                    break;
                case 8:
                    model.IMS_UNLOCK_BY = "I";
                    break;
            }

            return PartialView("AddLockDetails", model);
        }

        [HttpPost]
        [Audit]
        public ActionResult LockProposal()
        {
            string message = string.Empty;
            ProposalFilterViewModel model = new ProposalFilterViewModel();

            if (Request.Params["batchCode"] != string.Empty)
            {
                model.Batch = Convert.ToInt32(Request.Params["batchCode"]);
            }
            model.Year = Convert.ToInt32(Request.Params["yearCode"]);
            model.State = Convert.ToInt32(Request.Params["stateCode"]);
            if (Request.Params["districtCode"] != string.Empty)
            {
                model.District = Convert.ToInt32(Request.Params["districtCode"]);
            }
            model.Package = Request.Params["packageCode"];
            model.LockStatus = Request.Params["lockStatus"];
            try
            {
                if (ModelState.IsValid)
                {
                    if (objLockUnlockBAL.LockUnlockProposal(model, ref message))
                    {
                        if (model.LockStatus == "L")
                        {
                            message = message == string.Empty ? "Proposal Details Locked successfully." : message;
                        }
                        else
                        {
                            message = message == string.Empty ? "Proposal is Unlocked successfully." : message;
                        }
                    }
                    else
                    {
                        if (model.LockStatus == "U")
                        {
                            message = message == string.Empty ? "Proposal Details not locked successfully." : message;
                        }
                        else
                        {
                            message = message == string.Empty ? "Proposal is not unlocked successfully." : message;
                        }
                    }
                }
                else
                {
                    string messages = string.Join("; ", ModelState.Values
                                              .SelectMany(x => x.Errors)
                                              .Select(x => x.ErrorMessage));

                    return Json(new { success = false, message = messages }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                if (model.LockStatus == "U")
                {
                    message = "Proposal Details not Unlocked successfully";
                }
                else
                {
                    message = "Proposal Details not Locked successfully";
                }

                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult GetPackageByStateCode(int stateCode, int yearCode)
        {
            try
            {
                if (stateCode == 0)
                {
                    List<SelectListItem> lstPackages = new List<SelectListItem>();
                    lstPackages.Add(new SelectListItem { Value = "0", Text = "--Select Package--" });
                    return Json(new SelectList(lstPackages, "Value", "Text"), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    List<SelectListItem> lstPackages = objDAL.GetAllPackageByStateCode(stateCode, yearCode);
                    return Json(new SelectList(lstPackages, "Value", "Text"), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult GetAllPackageByDistrictCode(int stateCode, int yearCode, int districtCode, int batchCode, int blockCode, string type)
        {
            try
            {
                List<SelectListItem> lstPackages = objDAL.GetAllPackages(yearCode, stateCode, districtCode, batchCode, blockCode, type);
                return Json(new SelectList(lstPackages, "Value", "Text"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult ChangeLockStatus()
        {
            LockUnlockDAL objDAL = new LockUnlockDAL();
            try
            {
                int[] arrIDs = (Request.Params["ids[]"]).Split(',').Select(m => int.Parse(m)).ToArray();
                string module = Request.Params["module"];
                bool status = objDAL.ChangeLockStatus(arrIDs, module);
                if (status)
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


        #endregion

        #region CORE_NETWORK

        //GET:
        [Audit]
        public ActionResult SearchCoreNetwork()
        {
            List<SelectListItem> lstDemo = new List<SelectListItem>();
            lstDemo.Add(new SelectListItem { Value = "0", Text = "--Select--" });
            ViewData["State"] = new SelectList(objDAL.GetAllStates(false), "Value", "Text");
            ViewData["District"] = new SelectList(lstDemo, "Value", "Text");
            ViewData["Block"] = new SelectList(lstDemo, "Value", "Text");
            return PartialView("SearchCoreNetwork");
        }

        //GET:
        /// <summary>
        /// returns blocks by district code
        /// </summary>
        /// <param name="districtCode"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult GetBlocksByDistrictCode(int districtCode)
        {
            CommonFunctions objCommon = new CommonFunctions();

            try
            {
                List<SelectListItem> lstBlocks = objCommon.PopulateBlocks(districtCode, true);
                return Json(new SelectList(lstBlocks, "Value", "Text"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        //GET:
        /// <summary>
        /// returns list of Core Network details
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult GetCoreNetworkList(int? page, int? rows, string sidx, string sord)
        {
            int stateCode = 0;
            int districtCode = 0;
            int blockCode = 0;
            long totalRecords = 0;
            byte scheme = 0;
            int roleCode = 0;
            int collaboration = 0;

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

            if (!string.IsNullOrEmpty(Request.Params["scheme"]))
            {
                scheme = Convert.ToByte(Request.Params["scheme"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["RoleCode"]))
            {
                roleCode = Convert.ToByte(Request.Params["RoleCode"]);
            }
            ///Changes for Collaboration
            if (!string.IsNullOrEmpty(Request.Params["collaboration"]))
            {
                //collaboration = Convert.ToInt32(Request.Params["collaboration"]);
                collaboration = 0;
            }

            var jsonData = new
            {
                rows = objLockUnlockBAL.GetCoreNetworkList(stateCode, districtCode, blockCode, scheme, collaboration, roleCode, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords),
                total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                page = Convert.ToInt32(Request.Params["page"]),
                records = totalRecords,
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// returns list of Core Network Unlock details
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult GetCoreNetworkUnlockList(int? page, int? rows, string sidx, string sord)
        {
            int stateCode = 0;
            int districtCode = 0;
            int blockCode = 0;
            long totalRecords = 0;

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

            var jsonData = new
            {
                rows = objLockUnlockBAL.GetCoreNetworkUnlockList(stateCode, districtCode, blockCode, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords),
                total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                page = Convert.ToInt32(Request.Params["page"]),
                records = totalRecords,
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region EXISTING_ROAD

        //GET:
        [Audit]
        public ActionResult SearchExistingRoad()
        {
            List<SelectListItem> lstDemo = new List<SelectListItem>();

            int roleCode = PMGSYSession.Current.RoleCode;


            lstDemo.Add(new SelectListItem { Value = "0", Text = "--Select--" });
            ViewData["State"] = new SelectList(objDAL.GetAllStates(false), "Value", "Text");
            ViewData["District"] = new SelectList(lstDemo, "Value", "Text");
            ViewData["Block"] = new SelectList(lstDemo, "Value", "Text");
            return PartialView("SearchExistingRoad");
        }

        //GET:
        /// <summary>
        /// returns list of ExistingRoads
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult GetExistingRoadList(int? page, int? rows, string sidx, string sord)
        {
            int stateCode = 0;
            int districtCode = 0;
            int blockCode = 0;
            long totalRecords = 0;
            byte scheme = 0;
            int roleCode = 0;
            int collaboration = 0;

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

            if (!string.IsNullOrEmpty(Request.Params["scheme"]))
            {
                scheme = Convert.ToByte(Request.Params["scheme"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["RoleCode"]))
            {
                roleCode = Convert.ToByte(Request.Params["RoleCode"]);
            }
            ///Changes for Collaboration
            if (!string.IsNullOrEmpty(Request.Params["collaboration"]))
            {
                //collaboration = Convert.ToInt32(Request.Params["collaboration"]);
                collaboration = 0;
            }

            var jsonData = new
            {
                rows = objLockUnlockBAL.GetExistingRoadList(stateCode, districtCode, blockCode, scheme, collaboration, roleCode, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords),
                total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                page = Convert.ToInt32(Request.Params["page"]),
                records = totalRecords,
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// returns list of ExistingRoads
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult GetExistingRoadUnlockList(int? page, int? rows, string sidx, string sord)
        {
            int stateCode = 0;
            int districtCode = 0;
            int blockCode = 0;
            long totalRecords = 0;

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

            var jsonData = new
            {
                rows = objLockUnlockBAL.GetExistingRoadUnlockList(stateCode, districtCode, blockCode, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords),
                total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                page = Convert.ToInt32(Request.Params["page"]),
                records = totalRecords,
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region AGREEMENT

        //GET:
        [Audit]
        public ActionResult SearchAgreement()
        {
            List<SelectListItem> lstDemo = new List<SelectListItem>();
            lstDemo.Add(new SelectListItem { Value = "0", Text = "--Select--" });
            ViewData["State"] = new SelectList(objDAL.GetAllStates(false), "Value", "Text");
            ViewData["District"] = new SelectList(lstDemo, "Value", "Text");
            ViewData["Year"] = new SelectList(objDAL.GetAllYears(), "Value", "Text");
            return PartialView("SearchAgreement");
        }

        //GET:
        /// <summary>
        /// returns list of Agreement records
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        //public ActionResult GetAgreementList(FormCollection formCollection)
        [Audit]
        public ActionResult GetAgreementList(int? page, int? rows, string sidx, string sord)
        {
            int stateCode = 0;
            int districtCode = 0;
            int yearCode = 0;
            long totalRecords = 0;

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

            if (!string.IsNullOrEmpty(Request.Params["yearCode"]))
            {
                yearCode = Convert.ToInt32(Request.Params["yearCode"]);
            }

            var jsonData = new
            {
                rows = objLockUnlockBAL.GetAgreementList(stateCode, districtCode, yearCode, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords),
                total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                page = Convert.ToInt32(Request.Params["page"]),
                records = totalRecords,
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region TENDERING

        //GET:
        [Audit]
        public ActionResult SearchTendering()
        {
            List<SelectListItem> lstDemo = new List<SelectListItem>();
            lstDemo.Add(new SelectListItem { Value = "0", Text = "--Select--" });
            ViewData["State"] = new SelectList(objDAL.GetAllStates(false), "Value", "Text");
            ViewData["District"] = new SelectList(lstDemo, "Value", "Text");
            ViewData["Year"] = new SelectList(objDAL.GetAllYears(), "Value", "Text");
            return PartialView("SearchTendering");
        }

        //GET:
        /// <summary>
        /// returns list of tendering records
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        //public ActionResult GetTenderingList(FormCollection formCollection)
        [Audit]
        public ActionResult GetTenderingList(int? page, int? rows, string sidx, string sord)
        {
            int stateCode = 0;
            int districtCode = 0;
            int yearCode = 0;
            long totalRecords = 0;

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

            if (!string.IsNullOrEmpty(Request.Params["yearCode"]))
            {
                yearCode = Convert.ToInt32(Request.Params["yearCode"]);
            }

            var jsonData = new
            {
                rows = objLockUnlockBAL.GetTenderingList(stateCode, districtCode, yearCode, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords),
                total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                page = Convert.ToInt32(Request.Params["page"]),
                records = totalRecords,
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region CONTRACT

        //GET:
        /// <summary>
        /// returns Core Network maintanance list
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult GetManeCoreNetworkList(FormCollection formCollection)
        {
            int stateCode = 0;
            int districtCode = 0;
            int blockCode = 0;
            long totalRecords = 0;

            if (!string.IsNullOrEmpty(formCollection["stateCode"]))
            {
                stateCode = Convert.ToInt32(formCollection["stateCode"]);
            }

            if (!string.IsNullOrEmpty(formCollection["districtCode"]))
            {
                districtCode = Convert.ToInt32(formCollection["districtCode"]);
            }

            if (!string.IsNullOrEmpty(formCollection["blockCode"]))
            {
                blockCode = Convert.ToInt32(formCollection["blockCode"]);
            }

            var jsonData = new
            {
                rows = objLockUnlockBAL.GetManeCoreNetworkList(stateCode, districtCode, blockCode, Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                page = Convert.ToInt32(formCollection["page"]),
                records = totalRecords,
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        //GET:
        [Audit]
        public ActionResult SearchCNContract()
        {
            List<SelectListItem> lstDemo = new List<SelectListItem>();
            lstDemo.Add(new SelectListItem { Value = "0", Text = "--Select--" });
            ViewData["State"] = new SelectList(objDAL.GetAllStates(false), "Value", "Text");
            ViewData["District"] = new SelectList(lstDemo, "Value", "Text");
            ViewData["Block"] = new SelectList(lstDemo, "Value", "Text");
            return PartialView("SearchCNContract");
        }

        //GET:
        [Audit]
        public ActionResult GetIMSContractList(FormCollection proposalCollection)
        {
            int yearCode = 0;
            int stateCode = 0;
            int districtCode = 0;
            int batchCode = 0;
            string packageCode = string.Empty;
            long totalRecords = 0;

            if (!string.IsNullOrEmpty(proposalCollection["yearCode"]))
            {
                yearCode = Convert.ToInt32(proposalCollection["yearCode"]);
            }

            if (!string.IsNullOrEmpty(proposalCollection["stateCode"]))
            {
                stateCode = Convert.ToInt32(proposalCollection["stateCode"]);
            }

            if (!string.IsNullOrEmpty(proposalCollection["districtCode"]))
            {
                districtCode = Convert.ToInt32(proposalCollection["districtCode"]);
            }

            if (!string.IsNullOrEmpty(proposalCollection["batchCode"]))
            {
                batchCode = Convert.ToInt32(proposalCollection["batchCode"]);
            }

            if (!string.IsNullOrEmpty(proposalCollection["packageCode"]))
            {
                packageCode = proposalCollection["packageCode"];
            }

            var jsonData = new
            {
                rows = objLockUnlockBAL.GetIMSContractList(yearCode, stateCode, districtCode, batchCode, packageCode, Convert.ToInt32(proposalCollection["page"]) - 1, Convert.ToInt32(proposalCollection["rows"]), proposalCollection["sidx"], proposalCollection["sord"], out totalRecords),
                total = totalRecords <= Convert.ToInt32(proposalCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(proposalCollection["rows"]) + 1,
                page = Convert.ToInt32(proposalCollection["page"]),
                records = totalRecords,
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        //GET:
        [Audit]
        public ActionResult SearchIMSContract()
        {
            List<SelectListItem> lstDemo = new List<SelectListItem>();
            lstDemo.Add(new SelectListItem { Value = "0", Text = "--Select--" });
            ViewData["State"] = new SelectList(objDAL.GetAllStates(false), "Value", "Text");
            ViewData["Year"] = new SelectList(objDAL.GetAllYears(), "Value", "Text");
            ViewData["District"] = new SelectList(lstDemo, "Value", "Text");
            ViewData["Batch"] = new SelectList(objDAL.GetAllBatches(), "Value", "Text");
            ViewData["Package"] = new SelectList(lstDemo, "Value", "Text");
            return PartialView("SearchIMSContract");
        }

        #endregion

        #endregion

        #region UNLOCK

        [Audit]
        public ActionResult ListUnlockDetails()
        {
            CommonFunctions objCommon = new CommonFunctions();
            List<SelectListItem> lstDemo = new List<SelectListItem>();
            List<SelectListItem> lstRoles = new List<SelectListItem>();
            lstDemo.Add(new SelectListItem { Value = "0", Text = "--Select--" });
            ViewData["Module"] = new SelectList(objDAL.GetAllModules(), "Value", "Text");
            ViewData["FilterModule"] = new SelectList(objDAL.GetAllModules(), "Value", "Text");
            ViewData["Submodule"] = new SelectList(lstDemo, "Value", "Text");
            ViewData["Level"] = new SelectList(lstDemo, "Value", "Text");
            ViewBag.MAST_STATE_CODE = objCommon.PopulateStates(false);
            ViewBag.MAST_DISTRICT_CODE = lstDemo;
            ViewBag.MAST_BLOCK_CODE = lstDemo;
            ViewBag.MAST_VILLAGE_CODE = lstDemo;
            ViewBag.MAST_HAB_CODE = lstDemo;
            ViewData["State"] = objCommon.PopulateStates();

            lstRoles.Insert(0, new SelectListItem { Value = "0", Text = "All" });
            lstRoles.Insert(1, new SelectListItem { Value = "2", Text = "SRRDA" });
            lstRoles.Insert(2, new SelectListItem { Value = "37", Text = "SRRDA Other Agency" });
            lstRoles.Insert(3, new SelectListItem { Value = "22", Text = "PIU" });
            lstRoles.Insert(4, new SelectListItem { Value = "38", Text = "PIU Other Agency" });
            ViewData["Role"] = lstRoles;

            return View("ListUnlockDetails");
        }

        [Audit]
        public ActionResult LoadFilterDetails()
        {
            FilterDetailsViewModel model = new FilterDetailsViewModel();
            CommonFunctions objCommon = new CommonFunctions();
            try
            {
                List<SelectListItem> lstDemo = new List<SelectListItem>();
                lstDemo.Add(new SelectListItem { Value = "0", Text = "--Select--" });
                List<SelectListItem> lstSample = new List<SelectListItem>();
                lstSample.Add(new SelectListItem { Value = "0", Text = "--All--" });
                model.ddlBlock = lstSample;
                model.ddlDistrict = lstSample;
                model.ddlHabitation = lstSample;
                model.ddlVillage = lstSample;
                model.ddlState = objCommon.PopulateStates(true);
                model.ddlBatch = objCommon.PopulateBatch();
                model.ddlBatch.RemoveAt(0);
                model.ddlBatch.Insert(0, new SelectListItem { Value = "0", Text = "All" });
                model.ddlPackage = lstSample;
                //model.ddlYear = new SelectList(objCommon.PopulateFinancialYear(true, true).ToList(), "Value", "Text").ToList();

                //---------->> start Added By to unlock Proposal Technology Details
                model.ddlYear = new SelectList(objCommon.PopulateFinancialYear(true, false).ToList(), "Value", "Text").ToList();
                //---------->> end

                if (PMGSYSession.Current.RoleCode == 56)
                {
                    lstDemo.Insert(1, new SelectListItem { Value = "3", Text = "RCPLWE" });
                    lstDemo.Insert(2, new SelectListItem { Value = "4", Text = "PMGSY-3" });
                }
                else
                {
                    lstDemo.Insert(1, new SelectListItem { Value = "1", Text = "PMGSY-1" });
                    lstDemo.Insert(2, new SelectListItem { Value = "2", Text = "PMGSY-2" });
                    ///Changes for RCPLWE Unolock at ITNO/PMGSY3
                    lstDemo.Insert(3, new SelectListItem { Value = "3", Text = "RCPLWE" });
                    lstDemo.Insert(4, new SelectListItem { Value = "4", Text = "PMGSY-3" });
                    // Added by Srishti
                    lstDemo.Insert(5, new SelectListItem { Value = "5", Text = "Vibrant Village" });
                }
                model.lstSchemes = lstDemo;
                if (PMGSYSession.Current.RoleCode == 36 || PMGSYSession.Current.RoleCode == 47 || PMGSYSession.Current.RoleCode == 56)
                {
                    model.State = PMGSYSession.Current.StateCode;
                }
                model.lstProposalTypes = objCommon.PopulateProposalTypes();
                ///Changes for Collaboration
                model.lstCollaboration = objCommon.PopulateFundingAgency(true);
                model.lstCollaboration.Find(x => x.Value == "-1").Value = "0";

                return PartialView("LoadFilterDetails", model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return PartialView("LoadFilterDetails", new FilterDetailsViewModel());
            }
        }

        [Audit]
        [HttpPost]
        public ActionResult AddEditUnlockDetails(string modulecode, string levelcode, string scheme, string yearbatch, int roleCode)
        {
            UnlockDetailsViewModel objModel = new UnlockDetailsViewModel();
            string[] dataCodes = null;
            try
            {
                switch (Convert.ToInt32(modulecode))
                {
                    case 1:
                        objModel.UnlockTable = "HM";
                        break;
                    case 2:
                        objModel.UnlockTable = "PR";
                        break;
                    case 3:
                        objModel.UnlockTable = "ER";
                        break;
                    case 4:
                        objModel.UnlockTable = "CN";
                        break;
                    case 5:
                        objModel.UnlockTable = "VM";
                        break;
                    case 6:
                        objModel.UnlockTable = "DM";
                        break;
                    case 7:
                        objModel.UnlockTable = "BM";
                        break;
                    case 8:
                        objModel.UnlockTable = "PH";
                        break;

                    case 9: // Added By Rohit On 09 APR 2020
                        objModel.UnlockTable = "CH";
                        break;
                    case 10:                                // Added By to unlock Proposal Technology Details (Case 10)
                        objModel.UnlockTable = "PT";
                        break;
                    case 11:                                // Added By to unlock C-Performa PDF (Case 11) on 31-07-2023
                        objModel.UnlockTable = "CP";
                        break;
                    default:
                        break;
                }
                objModel.UnlockLevel = levelcode;
                objModel.UnlockRoleCode = roleCode;
                if (levelcode == "T")
                {
                    if (!String.IsNullOrEmpty(yearbatch))
                    {
                        string[] arrParams = yearbatch.Split('$');
                        objModel.StateCode = Convert.ToInt32(arrParams[0]);
                        objModel.DistrictCode = Convert.ToInt32(arrParams[1]);
                        objModel.BlockCode = Convert.ToInt32(arrParams[2]);
                        objModel.YearCode = Convert.ToInt32(arrParams[3]);
                    }
                }
                else if (levelcode == "Y")
                {
                    if (!String.IsNullOrEmpty(yearbatch))
                    {
                        string[] arrParams = yearbatch.Split('$');
                        objModel.StateCode = Convert.ToInt32(arrParams[0]);
                        objModel.DistrictCode = Convert.ToInt32(arrParams[1]);
                        objModel.BlockCode = Convert.ToInt32(arrParams[2]);
                    }
                }
                // Added By to unlock Proposal Technology Details // for road wise unlock save batch and package details in table
                else if (levelcode == "R")
                {
                    if (!String.IsNullOrEmpty(yearbatch))
                    {
                        string[] arrParams = yearbatch.Split('$');
                        objModel.StateCode = Convert.ToInt32(arrParams[0]);
                        objModel.DistrictCode = Convert.ToInt32(arrParams[1]);
                        objModel.BlockCode = Convert.ToInt32(arrParams[2]);
                        objModel.YearCode = Convert.ToInt32(arrParams[3]);
                        objModel.BatchCode = Convert.ToInt32(arrParams[4]);
                        objModel.Package = Convert.ToString(arrParams[5]);
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["unlockData[]"]))
                {
                    dataCodes = (Request.Params["unlockData[]"].Split(','));
                }

                objModel.dataID = dataCodes.Select(m => int.Parse(m)).ToArray(); //dataIDs;
                objModel.PMGSYScheme = Convert.ToByte(scheme);
                switch (PMGSYSession.Current.RoleCode)
                {
                    case 25:
                        objModel.UnlockBy = "M";
                        break;
                    case 36:
                        objModel.UnlockBy = "N";
                        objModel.UnlockStartDate = new CommonFunctions().GetDateTimeToString(DateTime.Now);
                        objModel.UnlockEndDate = new CommonFunctions().GetDateTimeToString(DateTime.Now);
                        break;
                    case 47:
                        objModel.UnlockBy = "N";
                        objModel.UnlockStartDate = new CommonFunctions().GetDateTimeToString(DateTime.Now);
                        objModel.UnlockEndDate = new CommonFunctions().GetDateTimeToString(DateTime.Now);
                        break;
                    case 56:
                        objModel.UnlockBy = "N";
                        objModel.UnlockStartDate = new CommonFunctions().GetDateTimeToString(DateTime.Now);
                        objModel.UnlockEndDate = new CommonFunctions().GetDateTimeToString(DateTime.Now);
                        break;
                }

                return PartialView("AddEditUnlockDetails", objModel);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "AddEditUnlockDetails()");
                return PartialView("AddEditUnlockDetails", new UnlockDetailsViewModel());
            }

        }

        [HttpPost]
        [Audit]
        public ActionResult AddUnlockDetails(UnlockDetailsViewModel model)
        {
            string message = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    if (objLockUnlockBAL.AddUnlockDetails(model, ref message))
                    {
                        message = (message == string.Empty ? "Unlock details added successfully." : message);
                        return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        message = message == string.Empty ? "Error occurred while processing your request." : message;
                        return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AddUnlockDetails(UnlockDetailsViewModel model)");
                message = message == string.Empty ? "Error occurred while processing your request." : message;
                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        [Audit]
        public JsonResult GetAllBlocksByDistrictCode(int districtCode)
        {
            CommonFunctions objCommon = new CommonFunctions();
            try
            {
                List<SelectListItem> lstBlocks = objCommon.PopulateBlocks(districtCode, true);
                return Json(new SelectList(lstBlocks, "Value", "Text"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [Audit]
        public JsonResult GetAllVillagesByBlockCode(int blockCode)
        {
            CommonFunctions objCommon = new CommonFunctions();
            try
            {
                List<SelectListItem> lstVillages = objDAL.GetVillagesByBlockCode(blockCode);
                return Json(new SelectList(lstVillages, "Value", "Text"), JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return null;
            }
        }

        [Audit]
        public JsonResult GetAllHabsByVillageCode(int villageCode)
        {
            try
            {
                List<SelectListItem> lstHabs = objDAL.GetHabsByVillageCode(villageCode);
                return Json(new SelectList(lstHabs, "Value", "Text"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [Audit]
        public ActionResult GetStateList(int? page, int? rows, string sidx, string sord)
        {
            long totalRecords = 0;
            int module = 0;
            byte scheme = 0;
            string moduleCode = string.Empty;
            int roleCode = 0;
            int collaboration = 0;
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["moduleCode"]))
                {
                    module = Convert.ToInt32(Request.Params["moduleCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["scheme"]))
                {
                    scheme = Convert.ToByte(Request.Params["scheme"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["RoleCode"]))
                {
                    roleCode = Convert.ToByte(Request.Params["RoleCode"]);
                }
                ///Changes for Collaboration
                if (!string.IsNullOrEmpty(Request.Params["collaboration"]))
                {
                    if (module == 2)
                    {
                        collaboration = Convert.ToInt32(Request.Params["collaboration"]);
                    }
                    else
                    {
                        collaboration = 0;
                    }
                }

                switch (module)
                {
                    case 1:
                        moduleCode = "HM";
                        break;
                    case 2:
                        moduleCode = "PR";
                        break;
                    case 3:
                        moduleCode = "ER";
                        break;
                    case 4:
                        moduleCode = "CN";
                        break;
                    case 5:
                        moduleCode = "VM";
                        break;
                    case 6:
                        moduleCode = "DM";
                        break;
                    case 7:
                        moduleCode = "BM";
                        break;

                    case 9: // Added By Rohit On 09 APR 2020
                         moduleCode = "CH";
                        break;

                    default:
                        break;
                }

                var jsonData = new
                {
                    rows = objLockUnlockBAL.GetStateList(moduleCode, scheme, collaboration, roleCode, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                    page = Convert.ToInt32(Request.Params["page"]),
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

        [Audit]
        public ActionResult GetDistrictList(int? page, int? rows, string sidx, string sord)
        {
            long totalRecords = 0;
            int stateCode = 0;
            string moduleCode = string.Empty;
            byte scheme = 0;
            int module = 0;
            int roleCode = 0;
            int collaboration = 0;

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

                if (!string.IsNullOrEmpty(Request.Params["moduleCode"]))
                {
                    module = Convert.ToInt32(Request.Params["moduleCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["scheme"]))
                {
                    scheme = Convert.ToByte(Request.Params["scheme"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["RoleCode"]))
                {
                    roleCode = Convert.ToByte(Request.Params["RoleCode"]);
                }
                ///Changes for Collaboration
                if (!string.IsNullOrEmpty(Request.Params["collaboration"]))
                {
                    if (module == 2)
                    {
                        collaboration = Convert.ToInt32(Request.Params["collaboration"]);
                    }
                    else
                    {
                        collaboration = 0;
                    }
                }

                switch (module)
                {
                    case 1:
                        moduleCode = "HM";
                        break;
                    case 2:
                        moduleCode = "PR";
                        break;
                    case 3:
                        moduleCode = "ER";
                        break;
                    case 4:
                        moduleCode = "CN";
                        break;
                    case 5:
                        moduleCode = "VM";
                        break;
                    case 6:
                        moduleCode = "DM";
                        break;
                    case 7:
                        moduleCode = "BM";
                        break;
                    case 9: // Added By Rohit On 09 APR 2020
                        moduleCode = "CH";
                        break;
                    default:
                        break;
                }
                var jsonData = new
                {
                    rows = objLockUnlockBAL.GetDistrictList(moduleCode, stateCode, scheme, collaboration, roleCode, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                    page = Convert.ToInt32(Request.Params["page"]),
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

        [Audit]
        public ActionResult GetYearsList(int? page, int? rows, string sidx, string sord)
        {
            long totalRecords = 0;
            int stateCode = 0;
            int districtCode = 0;
            int blockCode = 0;
            string moduleCode = string.Empty;
            byte scheme = 0;
            int module = 0;
            int roleCode = 0;
            int collaboration = 0;
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

                if (!string.IsNullOrEmpty(Request.Params["moduleCode"]))
                {
                    module = Convert.ToInt32(Request.Params["moduleCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["scheme"]))
                {
                    scheme = Convert.ToByte(Request.Params["scheme"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["RoleCode"]))
                {
                    roleCode = Convert.ToByte(Request.Params["RoleCode"]);
                }
                ///Changes for Collaboration
                if (!string.IsNullOrEmpty(Request.Params["collaboration"]))
                {
                    if (module == 2)
                    {
                        collaboration = Convert.ToInt32(Request.Params["collaboration"]);
                    }
                    else
                    {
                        collaboration = 0;
                    }
                }

                switch (module)
                {
                    case 1:
                        moduleCode = "HM";
                        break;
                    case 2:
                        moduleCode = "PR";
                        break;
                    case 3:
                        moduleCode = "ER";
                        break;
                    case 4:
                        moduleCode = "CN";
                        break;
                    case 5:
                        moduleCode = "VM";
                        break;
                    case 6:
                        moduleCode = "DM";
                        break;
                    case 7:
                        moduleCode = "BM";
                        break;
                    case 8:
                        moduleCode = "PH";
                        break;

                    case 9: // Added By Rohit On 09 APR 2020
                        moduleCode = "CH";
                        break;
                    case 10:                                // Added By to unlock Proposal Technology Details
                        moduleCode = "PT";
                        break;
                    case 11:                                // Added By Shreyas to unlock C-Proforma PDF
                        moduleCode = "CP";
                        break;
                    default:
                        break;
                }
                var jsonData = new
                {
                    rows = objLockUnlockBAL.GetYearsList(moduleCode, stateCode, districtCode, blockCode, scheme, collaboration, roleCode, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                    page = Convert.ToInt32(Request.Params["page"]),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "GetYears()");
                    if (ex != null)
                        sw.WriteLine("Exception : " + ex.ToString());
                    if (ex.InnerException != null)
                        sw.WriteLine("ex.InnerException : " + ex.InnerException.ToString());
                    if (ex.InnerException.InnerException != null)
                        sw.WriteLine("ex.InnerException.InnerException : " + ex.InnerException.InnerException.ToString());
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [Audit]
        public ActionResult GetBatchesList(int? page, int? rows, string sidx, string sord)
        {
            long totalRecords = 0;
            int stateCode = 0;
            int districtCode = 0;
            int blockCode = 0;
            int yearCode = 0;
            string moduleCode = string.Empty;
            byte scheme = 0;
            int module = 0;
            int roleCode = 0;
            int collaboration = 0;

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

                if (!string.IsNullOrEmpty(Request.Params["yearCode"]))
                {
                    yearCode = Convert.ToInt32(Request.Params["yearCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["moduleCode"]))
                {
                    module = Convert.ToInt32(Request.Params["moduleCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["scheme"]))
                {
                    scheme = Convert.ToByte(Request.Params["scheme"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["RoleCode"]))
                {
                    roleCode = Convert.ToByte(Request.Params["RoleCode"]);
                }
                ///Changes for Collaboration
                if (!string.IsNullOrEmpty(Request.Params["collaboration"]))
                {
                    if (module == 2)
                    {
                        collaboration = Convert.ToInt32(Request.Params["collaboration"]);
                    }
                    else
                    {
                        collaboration = 0;
                    }
                }

                switch (module)
                {
                    case 1:
                        moduleCode = "HM";
                        break;
                    case 2:
                        moduleCode = "PR";
                        break;
                    case 3:
                        moduleCode = "ER";
                        break;
                    case 4:
                        moduleCode = "CN";
                        break;
                    case 5:
                        moduleCode = "VM";
                        break;
                    case 6:
                        moduleCode = "DM";
                        break;
                    case 7:
                        moduleCode = "BM";
                        break;
                    case 8:
                        moduleCode = "PH";
                        break;

                    case 9: // Added By Rohit On 09 APR 2020
                        moduleCode = "CH";
                        break;
                    case 10:                                // Added to unlock Proposal Technology Details
                        moduleCode = "PT";
                        break;
                    case 11:                                // Added By Shreyas to unlock C-Proforma PDF
                        moduleCode = "CP";
                        break;
                    default:
                        break;
                }
                var jsonData = new
                {
                    rows = objLockUnlockBAL.GetBatchesList(moduleCode, stateCode, districtCode, blockCode, yearCode, scheme, collaboration, roleCode, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                    page = Convert.ToInt32(Request.Params["page"]),
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


        [Audit]
        public ActionResult GetBlockList(int? page, int? rows, string sidx, string sord)
        {
            long totalRecords = 0;
            int stateCode = 0, districtCode = 0;
            byte scheme = 0;
            string moduleCode = string.Empty;
            int module = 0;
            int roleCode = 0;
            int collaboration = 0;

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

                if (!string.IsNullOrEmpty(Request.Params["moduleCode"]))
                {
                    module = Convert.ToInt32(Request.Params["moduleCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["scheme"]))
                {
                    scheme = Convert.ToByte(Request.Params["scheme"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["RoleCode"]))
                {
                    roleCode = Convert.ToByte(Request.Params["RoleCode"]);
                }
                ///Changes for Collaboration
                if (!string.IsNullOrEmpty(Request.Params["collaboration"]))
                {
                    if (module == 2)
                    {
                        collaboration = Convert.ToInt32(Request.Params["collaboration"]);
                    }
                    else
                    {
                        collaboration = 0;
                    }
                }

                switch (module)
                {
                    case 1:
                        moduleCode = "HM";
                        break;
                    case 2:
                        moduleCode = "PR";
                        break;
                    case 3:
                        moduleCode = "ER";
                        break;
                    case 4:
                        moduleCode = "CN";
                        break;
                    case 5:
                        moduleCode = "VM";
                        break;
                    case 6:
                        moduleCode = "DM";
                        break;
                    case 7:
                        moduleCode = "BM";
                        break;

                    case 9: // Added By Rohit On 09 APR 2020
                        moduleCode = "CH";
                        break;
                    default:
                        break;
                }

                var jsonData = new
                {
                    rows = objLockUnlockBAL.GetBlockList(moduleCode, stateCode, districtCode, scheme, collaboration, roleCode, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                    page = Convert.ToInt32(Request.Params["page"]),
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

        [Audit]
        public ActionResult GetVillageList(int? page, int? rows, string sidx, string sord)
        {
            long totalRecords = 0;
            byte scheme = 0;
            int blockCode = 0;
            int module = 0;
            int roleCode = 0;
            string moduleCode = string.Empty;
            int collaboration = 0;

            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["blockCode"]))
                {
                    blockCode = Convert.ToInt32(Request.Params["blockCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["moduleCode"]))
                {
                    module = Convert.ToInt32(Request.Params["moduleCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["scheme"]))
                {
                    scheme = Convert.ToByte(Request.Params["scheme"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["RoleCode"]))
                {
                    roleCode = Convert.ToByte(Request.Params["RoleCode"]);
                }
                ///Changes for Collaboration
                if (!string.IsNullOrEmpty(Request.Params["collaboration"]))
                {
                    if (module == 2)
                    {
                        collaboration = Convert.ToInt32(Request.Params["collaboration"]);
                    }
                    else
                    {
                        collaboration = 0;
                    }
                }

                switch (module)
                {
                    case 1:
                        moduleCode = "HM";
                        break;
                    case 2:
                        moduleCode = "PR";
                        break;
                    case 3:
                        moduleCode = "ER";
                        break;
                    case 4:
                        moduleCode = "CN";
                        break;
                    case 5:
                        moduleCode = "VM";
                        break;

                    case 9: // Added By Rohit On 09 APR 2020
                        moduleCode = "CH";
                        break;

                    default:
                        break;
                }

                var jsonData = new
                {
                    rows = objLockUnlockBAL.GetVillageList(moduleCode, blockCode, scheme, collaboration, roleCode, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                    page = Convert.ToInt32(Request.Params["page"]),
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

        [Audit]
        public ActionResult GetHabitationList(int? page, int? rows, string sidx, string sord)
        {
            long totalRecords = 0;
            int villageCode = 0;
            byte scheme = 0;
            int roleCode = 0;
            string moduleCode = string.Empty;
            int module = 0;
            int collaboration = 0;

            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["villageCode"]))
                {
                    villageCode = Convert.ToInt32(Request.Params["villageCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["moduleCode"]))
                {
                    module = Convert.ToInt32(Request.Params["moduleCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["scheme"]))
                {
                    scheme = Convert.ToByte(Request.Params["scheme"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["RoleCode"]))
                {
                    roleCode = Convert.ToByte(Request.Params["RoleCode"]);
                }
                ///Changes for Collaboration
                if (!string.IsNullOrEmpty(Request.Params["collaboration"]))
                {
                    if (module == 2)
                    {
                        collaboration = Convert.ToInt32(Request.Params["collaboration"]);
                    }
                    else
                    {
                        collaboration = 0;
                    }
                }

                switch (module)
                {
                    case 1:
                        moduleCode = "HM";
                        break;
                    case 2:
                        moduleCode = "PR";
                        break;
                    case 3:
                        moduleCode = "ER";
                        break;
                    case 4:
                        moduleCode = "CN";
                        break;

                    case 9: // Added By Rohit On 09 APR 2020
                        moduleCode = "CH";
                        break;
                    default:
                        break;
                }

                var jsonData = new
                {
                    rows = objLockUnlockBAL.GetHabitationList(moduleCode, villageCode, scheme, collaboration, roleCode, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                    page = Convert.ToInt32(Request.Params["page"]),
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
        /// Returns the List of Proposal details
        /// </summary>
        /// <param name="proposalCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetITNOProposalList(int? page, int? rows, string sidx, string sord)
        {
            int yearCode = 0;
            int stateCode = 0;
            int districtCode = 0;
            int batchCode = 0;
            string packageCode = string.Empty;
            long totalRecords = 0;
            int blockCode = 0;
            byte scheme = 0;
            string type = string.Empty;
            List<int> lstIds;
            string ImsPrRoadCode;

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

            if (!string.IsNullOrEmpty(Request.Params["batchCode"]))
            {
                batchCode = Convert.ToInt32(Request.Params["batchCode"]);
            }

            if (!string.IsNullOrEmpty(Request.Params["packageCode"]))
            {
                packageCode = Request.Params["packageCode"];
            }

            if (!string.IsNullOrEmpty(Request.Params["scheme"]))
            {
                scheme = Convert.ToByte(Request.Params["scheme"]);
            }

            ///Changes by SAMMED A. PATIL on 20JULY2017 to display Type at ITNO login
            if (!string.IsNullOrEmpty(Request.Params["type"]))
            {
                type = Request.Params["type"].Trim();
            }

            var jsonData = new
            {
                rows = objLockUnlockBAL.GetITNOProposalList(yearCode, stateCode, districtCode, blockCode, batchCode, packageCode, scheme, type, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords, out lstIds, out ImsPrRoadCode),
                total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                page = Convert.ToInt32(Request.Params["page"]),
                records = totalRecords,
                ids = lstIds,
                RoadCodes = ImsPrRoadCode,

            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUnlockRecordList(int? page, int? rows, string sidx, string sord)
        {
            int _stateCode = 0;
            int _moduleCode = 0;
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

                if (!string.IsNullOrEmpty(Request.Params["StateCode"]))
                {
                    _stateCode = Convert.ToInt32(Request.Params["StateCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["ModuleCode"]))
                {
                    _moduleCode = Convert.ToInt32(Request.Params["ModuleCode"]);
                }

                var jsonData = new
                {
                    rows = objLockUnlockBAL.GetUnlockRecordListBAL(_stateCode, _moduleCode, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                    page = Convert.ToInt32(Request.Params["page"]),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return null;
            }
        }


        #region LIST_DATA

        public ActionResult GetProposalDetails(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                int levelCode = 0;
                String module = String.Empty;
                String levelName = String.Empty;
                long totalRecords;
                int scheme = 0;
                string type = string.Empty;
                string yearbatch = string.Empty;

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }


                if (!String.IsNullOrEmpty(Request.Params["param"]))
                {
                    String[] encryptedParams = Request.Params["param"].Split('/');
                    String[] decryptedParams = URLEncrypt.DecryptParameters(new String[] { encryptedParams[0], encryptedParams[1], encryptedParams[2] });
                    if (!String.IsNullOrEmpty(decryptedParams[0]))
                    {
                        levelCode = Convert.ToInt32(decryptedParams[0].Split('$')[0]);
                        levelName = (decryptedParams[0].Split('$')[1]);
                    }
                }

                if (!String.IsNullOrEmpty(Request.Params["module"]))
                {
                    module = Request.Params["module"];
                }

                if (!String.IsNullOrEmpty(Request.Params["scheme"]))
                {
                    scheme = Convert.ToInt32(Request.Params["scheme"]);
                }

                if (!String.IsNullOrEmpty(Request.Params["type"]))
                {
                    type = Request.Params["type"];
                }

                if (!String.IsNullOrEmpty(Request.Params["yearbatch"]))
                {
                    yearbatch = Request.Params["yearbatch"];
                }

                var jsonData = new
                {
                    rows = objLockUnlockBAL.GetProposalDetails(levelCode, levelName, scheme, type, yearbatch, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                    page = Convert.ToInt32(Request.Params["page"]),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(null);
            }
        }


        public ActionResult GetProposalDetailsForITNO(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                int levelCode = 0;
                String module = String.Empty;
                String levelName = String.Empty;
                long totalRecords;
                int scheme = 0;

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }


                if (!String.IsNullOrEmpty(Request.Params["param"]))
                {
                    String[] encryptedParams = Request.Params["param"].Split('/');
                    String[] decryptedParams = URLEncrypt.DecryptParameters(new String[] { encryptedParams[0], encryptedParams[1], encryptedParams[2] });
                    if (!String.IsNullOrEmpty(decryptedParams[0]))
                    {
                        levelCode = Convert.ToInt32(decryptedParams[0].Split('$')[0]);
                        levelName = (decryptedParams[0].Split('$')[1]);
                    }
                }

                if (!String.IsNullOrEmpty(Request.Params["module"]))
                {
                    module = Request.Params["module"];
                }

                if (!String.IsNullOrEmpty(Request.Params["scheme"]))
                {
                    scheme = Convert.ToInt32(Request.Params["scheme"]);
                }

                var jsonData = new
                {
                    rows = objLockUnlockBAL.GetProposalDetailsForITNO(levelCode, levelName, scheme, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                    page = Convert.ToInt32(Request.Params["page"]),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(null);
            }
        }


        public ActionResult GetCoreNetworkDetails(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                int levelCode = 0;
                String module = String.Empty;
                String levelName = String.Empty;
                int scheme = 0;
                long totalRecords;

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }


                if (!String.IsNullOrEmpty(Request.Params["param"]))
                {
                    String[] encryptedParams = Request.Params["param"].Split('/');
                    String[] decryptedParams = URLEncrypt.DecryptParameters(new String[] { encryptedParams[0], encryptedParams[1], encryptedParams[2] });
                    if (!String.IsNullOrEmpty(decryptedParams[0]))
                    {
                        levelCode = Convert.ToInt32(decryptedParams[0].Split('$')[0]);
                        levelName = (decryptedParams[0].Split('$')[1]);
                    }
                }

                if (!String.IsNullOrEmpty(Request.Params["module"]))
                {
                    module = Request.Params["module"];
                }

                if (!String.IsNullOrEmpty(Request.Params["scheme"]))
                {
                    scheme = Convert.ToInt32(Request.Params["scheme"]);
                }

                var jsonData = new
                {
                    rows = objLockUnlockBAL.GetCNDetails(levelCode, levelName, scheme, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                    page = Convert.ToInt32(Request.Params["page"]),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(null);
            }
        }


        public ActionResult GetDRRPDetails(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                int levelCode = 0;
                String module = String.Empty;
                String levelName = String.Empty;
                int scheme = 0;
                long totalRecords;

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }


                if (!String.IsNullOrEmpty(Request.Params["param"]))
                {
                    String[] encryptedParams = Request.Params["param"].Split('/');
                    String[] decryptedParams = URLEncrypt.DecryptParameters(new String[] { encryptedParams[0], encryptedParams[1], encryptedParams[2] });
                    if (!String.IsNullOrEmpty(decryptedParams[0]))
                    {
                        levelCode = Convert.ToInt32(decryptedParams[0].Split('$')[0]);
                        levelName = (decryptedParams[0].Split('$')[1]);
                    }
                }

                if (!String.IsNullOrEmpty(Request.Params["module"]))
                {
                    module = Request.Params["module"];
                }

                if (!String.IsNullOrEmpty(Request.Params["scheme"]))
                {
                    scheme = Convert.ToInt32(Request.Params["scheme"]);
                }

                var jsonData = new
                {
                    rows = objLockUnlockBAL.GetDRRPDetails(levelCode, levelName, scheme, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                    page = Convert.ToInt32(Request.Params["page"]),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(null);
            }
        }


        public ActionResult GetVillageDetails(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                int levelCode = 0;
                String module = String.Empty;
                String levelName = String.Empty;
                int scheme = 0;
                long totalRecords;

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }


                if (!String.IsNullOrEmpty(Request.Params["param"]))
                {
                    String[] encryptedParams = Request.Params["param"].Split('/');
                    String[] decryptedParams = URLEncrypt.DecryptParameters(new String[] { encryptedParams[0], encryptedParams[1], encryptedParams[2] });
                    if (!String.IsNullOrEmpty(decryptedParams[0]))
                    {
                        levelCode = Convert.ToInt32(decryptedParams[0].Split('$')[0]);
                        levelName = (decryptedParams[0].Split('$')[1]);
                    }
                }

                if (!String.IsNullOrEmpty(Request.Params["module"]))
                {
                    module = Request.Params["module"];
                }

                if (!String.IsNullOrEmpty(Request.Params["scheme"]))
                {
                    scheme = Convert.ToInt32(Request.Params["scheme"]);
                }


                var jsonData = new
                {
                    rows = objLockUnlockBAL.GetVillageDetails(levelCode, levelName, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                    page = Convert.ToInt32(Request.Params["page"]),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(null);
            }
        }

        public ActionResult GetHabitationDetails(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                int levelCode = 0;
                String module = String.Empty;
                String levelName = String.Empty;
                int scheme = 0;
                long totalRecords;

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }


                if (!String.IsNullOrEmpty(Request.Params["param"]))
                {
                    String[] encryptedParams = Request.Params["param"].Split('/');
                    String[] decryptedParams = URLEncrypt.DecryptParameters(new String[] { encryptedParams[0], encryptedParams[1], encryptedParams[2] });
                    if (!String.IsNullOrEmpty(decryptedParams[0]))
                    {
                        levelCode = Convert.ToInt32(decryptedParams[0].Split('$')[0]);
                        levelName = (decryptedParams[0].Split('$')[1]);
                    }
                }

                if (!String.IsNullOrEmpty(Request.Params["module"]))
                {
                    module = Request.Params["module"];
                }

                if (!String.IsNullOrEmpty(Request.Params["scheme"]))
                {
                    scheme = Convert.ToInt32(Request.Params["scheme"]);
                }

                var jsonData = new
                {
                    rows = objLockUnlockBAL.GetHabDetails(levelCode, levelName, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                    page = Convert.ToInt32(Request.Params["page"]),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(null);
            }
        }


        #endregion


        public ActionResult ViewLockUnlockReport()
        {
            List<SelectListItem> lstTitle = new List<SelectListItem>();
            lstTitle.Insert(0, new SelectListItem { Value = "0", Text = "All" });
            LockUnlockReportModel model = new LockUnlockReportModel();
            model.lstDistricts = lstTitle;
            model.lstBlocks = lstTitle;
            model.lstModules = objDAL.GetAllModules();
            List<SelectListItem> lstDemo = new List<SelectListItem>();
            lstDemo.Insert(0, new SelectListItem { Value = "0", Text = "Select Scheme" });
            if (PMGSYSession.Current.RoleCode == 56)
            {
                lstDemo.Insert(1, new SelectListItem { Value = "3", Text = "RCPLWE" });
                lstDemo.Insert(4, new SelectListItem { Value = "4", Text = "PMGSY-3" });
            }
            else
            {
                lstDemo.Insert(1, new SelectListItem { Value = "1", Text = "PMGSY-1" });
                lstDemo.Insert(2, new SelectListItem { Value = "2", Text = "PMGSY-2" });
                ///Changes for RCPLWE/PMGSY3
                lstDemo.Insert(3, new SelectListItem { Value = "3", Text = "RCPLWE" });
                lstDemo.Insert(4, new SelectListItem { Value = "4", Text = "PMGSY-3" });
            }
            model.lstSchemes = lstDemo;

            if (PMGSYSession.Current.RoleCode == 36 || PMGSYSession.Current.RoleCode == 47 || PMGSYSession.Current.RoleCode == 56)
            {
                model.StateCode = PMGSYSession.Current.StateCode;
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult GetUnlockReportList(int? page, int? rows, string sidx, string sord)
        {
            long totalRecords = 0;
            int stateCode = 0;
            int districtCode = 0;
            int blockCode = 0;
            int moduleCode = 0;
            int schemeCode = 0;
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!String.IsNullOrEmpty(Request.Params["StateCode"]))
                {
                    stateCode = Convert.ToInt32(Request.Params["StateCode"]);
                }

                if (!String.IsNullOrEmpty(Request.Params["BlockCode"]))
                {
                    blockCode = Convert.ToInt32(Request.Params["BlockCode"]);
                }

                if (!String.IsNullOrEmpty(Request.Params["DistrictCode"]))
                {
                    districtCode = Convert.ToInt32(Request.Params["DistrictCode"]);
                }

                if (!String.IsNullOrEmpty(Request.Params["ModuleCode"]))
                {
                    moduleCode = Convert.ToInt32(Request.Params["ModuleCode"]);
                }

                if (!String.IsNullOrEmpty(Request.Params["PMGSYScheme"]))
                {
                    schemeCode = Convert.ToInt32(Request.Params["PMGSYScheme"]);
                }

                var jsonData = new
                {
                    rows = objLockUnlockBAL.GetUnlockReportList(stateCode, districtCode, blockCode, moduleCode, schemeCode, Convert.ToInt32(Request.Params["page"]) - 1, Convert.ToInt32(Request.Params["rows"]), Request.Params["sidx"], Request.Params["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(Request.Params["rows"]) ? 1 : totalRecords / Convert.ToInt32(Request.Params["rows"]) + 1,
                    page = Convert.ToInt32(Request.Params["page"]),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);


            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion



    }
}

