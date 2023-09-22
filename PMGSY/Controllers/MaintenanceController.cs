#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   MaintenanceController.cs      
        * Description   :   Action methods for Creating , Editing, Deleting PCI Index of PMGSY Roads and Core Network Roads.
        * Author        :   Shivkumar Deshmukh        
        * Creation Date :   18/June/2013
 **/
#endregion

using PMGSY.BAL.Maintenance;
using PMGSY.Common;
using PMGSY.Extensions;
using PMGSY.Models;
using PMGSY.Models.Maintenance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class MaintenanceController : Controller
    {
     

        #region Variable Declaration
        private PMGSYEntities dbContext = new PMGSYEntities();
        private IMaintenanceBAL objMaintenanceBAL = new MaintenanceBAL();
        #endregion

        #region PCI Region

        /// <summary>
        /// Get Method for Filter of Populating PMGSY, Core Network Road
        /// </summary>
        /// <returns></returns>     
        [HttpGet]
        [Audit]
        public ActionResult GetPCIForPmgsyRoad()
        {
            CommonFunctions commonFunctions = new CommonFunctions();
            ViewBag.Years = new SelectList( commonFunctions.PopulateFinancialYear(false, false), "Value", "Text", DateTime.Now.Year);
            ViewBag.RoadType = new SelectList(objMaintenanceBAL.PopulateRoadType(), "Value", "Text", "P");
            ViewBag.Blocks = new SelectList( commonFunctions.PopulateBlocks(PMGSYSession.Current.DistrictCode,true) , "Value", "Text");
            return View("ListPciDetails");
        }
        
        /// <summary>
        /// Method to List Pmgsy Road 
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult GetPmgsyRoadList(FormCollection formCollection)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }

            //Adde By Abhishek kamble 29-Apr-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 29-Apr-2014 end
            
            int IMS_YEAR = Convert.ToInt32(Request.Params["IMS_YEAR"]);
            int IMS_BLOCK_ID = Convert.ToInt32(Request.Params["Block"]);

            int totalRecords =0;

            var jsonData = new
            {
                rows = objMaintenanceBAL.GetPmgsyRoadsBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, PMGSYSession.Current.StateCode, PMGSYSession.Current.DistrictCode, PMGSYSession.Current.AdminNdCode,IMS_YEAR , IMS_BLOCK_ID),                
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                page = Convert.ToInt32(formCollection["page"]),
                records = totalRecords
            };
            return Json(jsonData);
        }

        /// <summary>
        /// Method to List Core Network Roads
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult GetCNRoadList(FormCollection formCollection)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }

            //Adde By Abhishek kamble 29-Apr-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 29-Apr-2014 end
           
            int IMS_BLOCK_ID = Convert.ToInt32(Request.Params["Block"]);

            int totalRecords = 0;

            var jsonData = new
            {
                rows = objMaintenanceBAL.GetCNRoadsBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, PMGSYSession.Current.StateCode, PMGSYSession.Current.DistrictCode, PMGSYSession.Current.AdminNdCode, IMS_BLOCK_ID),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                page = Convert.ToInt32(formCollection["page"]),
                records = totalRecords
            };
            return Json(jsonData);
        }

        /// <summary>
        /// Get the PCI list for PMGSY Roads
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult GetPCIListForPmgsyRoad(FormCollection formCollection)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }

            //Adde By Abhishek kamble 29-Apr-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 29-Apr-2014 end

            string[] EncKey = Request.Params["IMS_PR_ROAD_CODE"].Split('/');
            int IMS_PR_ROAD_CODE = 0;
            if (EncKey.Length == 3)
            {
                if (!String.IsNullOrEmpty(EncKey[0]) && !String.IsNullOrEmpty(EncKey[1]) && !String.IsNullOrEmpty(EncKey[2]))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { EncKey[0], EncKey[1], EncKey[2] });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        IMS_PR_ROAD_CODE = Convert.ToInt32(urlSplitParams[0]);
                    }
                }
            }
            else
            {
                return Json(new { Success = false, ErrorMessage = "Error Occured while processing your Request." });
            }       

            int totalRecords = 0;
            var jsonData = new
            {
                rows = objMaintenanceBAL.GetPCIListForPmgsyRoadBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, IMS_PR_ROAD_CODE),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                page = Convert.ToInt32(formCollection["page"]),
                records = totalRecords
            };
            return Json(jsonData);
        }

        /// <summary>
        /// Get the PCI List of CN Roads
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult GetPCIListForCNRoad(FormCollection formCollection)
        {
            if (PMGSYSession.Current == null)
            {
                Response.Redirect("/Login/Login");
            }

            //Adde By Abhishek kamble 29-Apr-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 29-Apr-2014 end

            string[] EncKey = Request.Params["PLAN_CN_ROAD_CODE"].Split('/');
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
            var jsonData = new
            {
                rows = objMaintenanceBAL.GetPCIListForCNRoadBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords,PLAN_CN_ROAD_CODE),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                page = Convert.ToInt32(formCollection["page"]),
                records = totalRecords
            };
            return Json(jsonData);
        }
        
        /// <summary>
        /// Get the Details of the Road on Selection of an Year
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult GetRoadLengthDetails()
        {
            int IMS_PR_ROAD_CODE = 0;
            string[] EncKey = Request.Params["ENC_IMS_PR_ROAD_CODE"].Split('/');
            int MANE_IMS_YEAR = Convert.ToInt32(Request.Params["MANE_IMS_YEAR"]);
            
            if (EncKey.Length == 3)
            {
                if (!String.IsNullOrEmpty(EncKey[0]) && !String.IsNullOrEmpty(EncKey[1]) && !String.IsNullOrEmpty(EncKey[2]))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { EncKey[0], EncKey[1], EncKey[2] });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        IMS_PR_ROAD_CODE = Convert.ToInt32(urlSplitParams[0]);
                    }
                }
            }
            else
            {
                return Json(new { Success = false, ErrorMessage = "Error Occured while processing your Request." });
            }

            dbContext = new PMGSYEntities();
            objMaintenanceBAL = new MaintenanceBAL();

            string status = objMaintenanceBAL.GetRoadDetailsBAL(IMS_PR_ROAD_CODE, MANE_IMS_YEAR);
            
            if (status == "-111")
            {
                return  Json(new { Success = false, ErrorMessage = "All Entries of PCI For this Year has been done."});
            }
            if (status == "-999")
            {
                return Json(new { Success = false, ErrorMessage = "Error occured while processing your request."  });
            }
            else
            {
                return Json(new { Success = true, MANE_END_CHAIN = status });
            }
        }
        
        /// <summary>
        /// Get the PCI Data Entry Form For PMGSY Roads
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult AddPciForPmgsyRoad(String parameter, String hash, String key)
        {
            int IMS_PR_ROAD_CODE = 0;
            if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
            {
                String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                if (urlParams.Length >= 1)
                {
                    String[] urlSplitParams = urlParams[0].Split('$');
                    IMS_PR_ROAD_CODE = Convert.ToInt32(urlSplitParams[0]);
                }
            }            
            dbContext = new PMGSYEntities();
            IMS_SANCTIONED_PROJECTS ims_sanctioned_project = dbContext.IMS_SANCTIONED_PROJECTS.Find(IMS_PR_ROAD_CODE);
            
            CommonFunctions objCommonFunction = new CommonFunctions();

            PciIndexViewModel pciIndexViewModel = new PciIndexViewModel();
            pciIndexViewModel.SURFACES = objCommonFunction.PopulateSurfaceType();
            pciIndexViewModel.ENC_IMS_PR_ROAD_CODE = URLEncrypt.EncryptParameters(new string[] { IMS_PR_ROAD_CODE.ToString().Trim() });
            pciIndexViewModel.RoadName = ims_sanctioned_project.IMS_ROAD_NAME;
            pciIndexViewModel.RoadLength = ims_sanctioned_project.IMS_PAV_LENGTH;

            int CompletionYear = (from d in dbContext.EXEC_ROADS_MONTHLY_STATUS where d.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE && d.EXEC_ISCOMPLETED == "C" select d.EXEC_PROG_YEAR).First();

            //Commented By Abhishek kamble 18-Mar-2014
            //pciIndexViewModel.YEARS = objCommonFunction.PopulateUpToYear(true, false, CompletionYear);

            //Added By Abhishek kamble 18-Mar-2014 start
            if (DateTime.Now.Month <= 3)
            {
                List<SelectListItem> lstYears = objCommonFunction.PopulateUpToYear(true, false, CompletionYear);
                lstYears.RemoveAt(1);
                pciIndexViewModel.YEARS = lstYears; 
                
            }
            else {
                pciIndexViewModel.YEARS = objCommonFunction.PopulateUpToYear(true, false, CompletionYear);            
            }
            //Added By Abhishek kamble 18-Mar-2014 end

            return View("AddPciForPmgsyRoad", pciIndexViewModel);
        }
        
        /// <summary>
        /// Save the PCI Index Information for PMGSY Roads
        /// </summary>
        /// <param name="pciIndexViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult SavePciForPmgsyRoad(PciIndexViewModel pciIndexViewModel)
        {
            if (ModelState.IsValid)
            {
                int IMS_PR_ROAD_CODE = 0;
                string [] EncKey = pciIndexViewModel.ENC_IMS_PR_ROAD_CODE.Split('/');

                if (EncKey.Length == 3)
                {
                    if (!String.IsNullOrEmpty(EncKey[0] ) && !String.IsNullOrEmpty(EncKey[1]) && !String.IsNullOrEmpty(EncKey[2]))
                    {
                        String[] urlParams = URLEncrypt.DecryptParameters(new String[] { EncKey[0], EncKey[1], EncKey[2] });
                        if (urlParams.Length >= 1)
                        {
                            String[] urlSplitParams = urlParams[0].Split('$');
                            IMS_PR_ROAD_CODE = Convert.ToInt32(urlSplitParams[0]);
                        }
                    }
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = "Error Occured while processing your Request." });
                }

                pciIndexViewModel.IMS_PR_ROAD_CODE = IMS_PR_ROAD_CODE;                 

                objMaintenanceBAL = new MaintenanceBAL();
                string status = objMaintenanceBAL.SavePciForPmgsyRoadBAL(pciIndexViewModel);
            
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
        
        /// <summary>
        /// Save the PCI For CN Roads
        /// </summary>
        /// <param name="pciIndexViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult SavePciForCNRoad(PciIndexViewModel pciIndexViewModel)
        {
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

                pciIndexViewModel.PLAN_CN_ROAD_CODE = PLAN_CN_ROAD_CODE;

                objMaintenanceBAL = new MaintenanceBAL();
                string status = objMaintenanceBAL.SavePciForCNRoadBAL(pciIndexViewModel);

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
        
        /// <summary>
        /// Delete the PCI for PMGSY Road
        /// </summary>
        /// <returns></returns>
        [Audit]
        public JsonResult DeletePciForPmgsyRoad()
        {
            string Data =string.Empty;
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
            objMaintenanceBAL= new MaintenanceBAL();
            
            string status = objMaintenanceBAL.DeletePciforPmgsyRoadBAL(Convert.ToInt32(urlSplitParams[0]), Convert.ToInt32(urlSplitParams[1]), Convert.ToInt32(urlSplitParams[2]));
           
            if (status == string.Empty)
            {
                return Json(new { Success = true });
            }
            else
            {
                return Json(new { Success = false, ErrorMessage = status });
            }
        }
        
        /// <summary>
        /// Delete the PCI For CN Road
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult DeletePciForCNRoad()
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
            objMaintenanceBAL = new MaintenanceBAL();

            string status = objMaintenanceBAL.DeletePciforCNRoadBAL(Convert.ToInt32(urlSplitParams[0]), Convert.ToInt32(urlSplitParams[1]), Convert.ToInt32(urlSplitParams[2]));

            if (status == string.Empty)
            {
                return Json(new { Success = true });
            }
            else
            {
                return Json(new { Success = false, ErrorMessage = status });
            }
        }

        /// <summary>
        /// Add PCI For CN Roads
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult AddPciForCNRoad(String parameter, String hash, String key)
        {
            int PLAN_CN_ROAD_CODE = 0;
            if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
            {
                String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                if (urlParams.Length >= 1)
                {
                    String[] urlSplitParams = urlParams[0].Split('$');
                    PLAN_CN_ROAD_CODE = Convert.ToInt32(urlSplitParams[0]);
                }
            }
            dbContext = new PMGSYEntities();
            PLAN_ROAD plan_road = dbContext.PLAN_ROAD.Find(PLAN_CN_ROAD_CODE);

            CommonFunctions objCommonFunction = new CommonFunctions();

            PciIndexViewModel pciIndexViewModel = new PciIndexViewModel();
            
            pciIndexViewModel.SURFACES = objCommonFunction.PopulateSurfaceType();
            pciIndexViewModel.ENC_PLAN_CN_ROAD_CODE = URLEncrypt.EncryptParameters(new string[] { PLAN_CN_ROAD_CODE.ToString().Trim() });
            pciIndexViewModel.MANE_STR_CHAIN = plan_road.PLAN_RD_FROM_CHAINAGE.HasValue ? plan_road.PLAN_RD_FROM_CHAINAGE.Value : 0;
            pciIndexViewModel.RoadName = plan_road.PLAN_RD_NAME;
            pciIndexViewModel.RoadLength = plan_road.PLAN_RD_TOTAL_LEN.HasValue ? plan_road.PLAN_RD_TOTAL_LEN.Value : (plan_road.PLAN_RD_LENGTH == null ? 0 : Convert.ToDecimal(plan_road.PLAN_RD_LENGTH)) ;
            //pciIndexViewModel.YEARS = objCommonFunction.PopulateUpToYear(true, false);

            //Commented By Abhishek kamble 18-Mar-2014
            //pciIndexViewModel.YEARS = objCommonFunction.PopulateUpToYear(true, false, CompletionYear);

            //Added By Abhishek kamble 18-Mar-2014 start
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
            //Added By Abhishek kamble 18-Mar-2014 end

            return View("AddPciForCNRoad", pciIndexViewModel);
        }

        /// <summary>
        /// Get the Details of the Core Network Road on Selection of an Year
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult GetCNRoadLengthDetails()
        {
            int PLAN_CN_ROAD_CODE = 0;
            string[] EncKey = Request.Params["ENC_PLAN_CN_ROAD_CODE"].Split('/');
            int MANE_IMS_YEAR = Convert.ToInt32(Request.Params["MANE_IMS_YEAR"]);

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

            dbContext = new PMGSYEntities();
            objMaintenanceBAL = new MaintenanceBAL();

            string status = objMaintenanceBAL.GetCNRoadDetailsBAL(PLAN_CN_ROAD_CODE, MANE_IMS_YEAR);

            if (status == "-111")
            {
                return Json(new { Success = false, ErrorMessage = "All Entries of PCI For this Year has been done." });
            }
            if (status == "-999")
            {
                return Json(new { Success = false, ErrorMessage = "Error occured while processing your request." });
            }
            else
            {
                return Json(new { Success = true, MANE_END_CHAIN = status });
            }
        }
        
        #endregion

        #region 
        /// <summary>
        /// This Method Disposes the DB Object in this class
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            dbContext.Dispose();
            base.Dispose(disposing);
        }
        #endregion
    }
}
