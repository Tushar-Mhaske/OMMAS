using PMGSY.BAL.Proposal;
using PMGSY.Common;
using PMGSY.DAL.Master;
using PMGSY.Extensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Models.Proposal;
using PMGSY.Models;
using System.Transactions;

namespace PMGSY.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class MRDProposalController : Controller
    {
        //
        // GET: /MRDProposal/
        PMGSYEntities dbContext;
        Dictionary<string, string> decryptedParameters = null;

        [HttpGet]
        public ActionResult SearchMrdDroppedLetter()
        {
            try
            {
                MasterDAL objDAL = new MasterDAL();

                //MrdDroppedViewModel model = new MrdDroppedViewModel();
                SearchDroppedViewModel model = new SearchDroppedViewModel();

                CommonFunctions commonFunctions = new CommonFunctions();
                //model.StateList  = new List<SelectListItem>();
                model.StateName = PMGSYSession.Current.StateCode == 0 ? "0" : PMGSYSession.Current.StateName.Trim();
                model.StateList = commonFunctions.PopulateStates(false);
                model.StateList.Insert(0, (new SelectListItem { Text = "All States", Value = "0" }));
                model.StateCode = PMGSYSession.Current.StateCode == 0 ? 0 : PMGSYSession.Current.StateCode;
                model.StateList.Find(x => x.Value == model.StateCode.ToString()).Selected = true;
                model.BatchList = commonFunctions.PopulateBatch(true);
                model.Mast_AgencyList = new List<SelectListItem>();
                if (model.StateCode > 0)
                {
                    model.Mast_AgencyList = commonFunctions.PopulateAgenciesByStateAndDepartmentwise(model.StateCode, PMGSYSession.Current.AdminNdCode, true);
                }
                else
                {
                    model.Mast_AgencyList.Insert(0, (new SelectListItem { Text = "All Agencies", Value = "0" }));
                }
                model.PhaseYear = DateTime.Now.Year;
                //PhaseYearList = new SelectList(commonFunctions.PopulateYears(false), "Value", "Text").ToList();
                //PhaseYearList.Insert(0, (new SelectListItem { Text = "All Year", Value = "0" }));
                model.PhaseYearList = new SelectList(commonFunctions.PopulateFinancialYear(true, true), "Value", "Text").ToList();
                model.COLLABORATIONS_List = commonFunctions.PopulateFundingAgency(true);

                return PartialView("SearchMrdDroppedLetter", model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(string.Empty);
            }
        }

        [HttpGet]
        public ActionResult ListMrdDroppedLetter()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetMrdClearanceList(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                String searchParameters = String.Empty;
                long totalRecords;
                int stateCode = 0;
                // int districtCode = 0;
                int agencyCode = 0;
                int year = 0;
                int batch = 0;
                int collaboration = 0;
                Dictionary<string, string> parameters = new Dictionary<string, string>();

                IProposalBAL1 objBAL = new MRDProposalBAL();
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

                if (!string.IsNullOrEmpty(Request.Params["agency"]))
                {
                    agencyCode = Convert.ToInt32(Request.Params["agency"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["year"]))
                {
                    year = Convert.ToInt32(Request.Params["year"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["batch"]))
                {
                    batch = Convert.ToInt32(Request.Params["batch"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["collaboration"]))
                {
                    collaboration = Convert.ToInt32(Request.Params["collaboration"]);
                }
                var jsonData = new
                {
                    rows = objBAL.ListMrdClearanceBAL(stateCode, year, batch, agencyCode, collaboration, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(string.Empty);
            }
        }

        [HttpPost]
        public ActionResult LoadMrdDroppedLetter(String parameter, String hash, String key)
        {
            MrdDroppedViewModel model = new MrdDroppedViewModel();
            model.EncryptedClearanceCode = parameter.Trim() + "/" + hash.Trim() + "/" + key.Trim();

            decryptedParameters = PMGSY.Common.URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
            if (decryptedParameters.Count > 0)
            {
                model.MRD_CLEARANCE_CODE = Convert.ToInt32(decryptedParameters["ClearanceCode"]);
            }

            return PartialView("LoadMrdDroppedLetter", model);
        }

        [HttpPost]
        public ActionResult GetMrdDroppedLetterList(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                String searchParameters = String.Empty;
                long totalRecords;
                int stateCode = 0;
                // int districtCode = 0;
                int agencyCode = 0;
                int year = 0;
                int batch = 0;
                int collaboration = 0;
                int clrId = 0;
                Dictionary<string, string> parameters = new Dictionary<string, string>();

                IProposalBAL1 objBAL = new MRDProposalBAL();
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

                if (!string.IsNullOrEmpty(Request.Params["agency"]))
                {
                    agencyCode = Convert.ToInt32(Request.Params["agency"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["year"]))
                {
                    year = Convert.ToInt32(Request.Params["year"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["batch"]))
                {
                    batch = Convert.ToInt32(Request.Params["batch"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["collaboration"]))
                {
                    collaboration = Convert.ToInt32(Request.Params["collaboration"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["clearanceCode"]))
                {
                    clrId = Convert.ToInt32(Request.Params["clearanceCode"]);
                }
                var jsonData = new
                {
                    rows = objBAL.ListMrdDroppedBAL(clrId, stateCode, year, batch, agencyCode, collaboration, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(string.Empty);
            }
        }

        [HttpGet]
        //[Audit]
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

                if (FileExtension == ".pdf")
                {
                    FullFileLogicalPath = Path.Combine(ConfigurationManager.AppSettings["DROPPED_PDF_FILE_UPLOAD_VIRTUAL_DIR_PATH"], FileName);
                    FullfilePhysicalPath = Path.Combine(ConfigurationManager.AppSettings["DROPPED_PDF_FILE_UPLOAD"], FileName);
                }
                else if (FileExtension == ".xls" || FileExtension == ".xlsx")
                {
                    FullFileLogicalPath = Path.Combine(ConfigurationManager.AppSettings["DROPPED_ROAD_EXCEL_FILE_UPLOAD_VIRTUAL_DIR_PATH"], FileName);
                    FullfilePhysicalPath = Path.Combine(ConfigurationManager.AppSettings["DROPPED_ROAD_EXCEL_FILE_UPLOAD"], FileName);
                }

                string name = Path.GetFileName(FileName);
                string ext = Path.GetExtension(FileName);

                string type = string.Empty;

                if (ext != null)
                {
                    switch (ext.ToLower())
                    {
                        case ".pdf":
                            type = "Application/pdf";
                            break;
                        case ".doc":
                        case ".docx":
                        case ".xls":
                        case ".xlsx":
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
                    return Json(new { Success = "false" }, JsonRequestBehavior.AllowGet);
                    //return null;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(string.Empty);
            }
        }


        //[HttpGet]
        //public ActionResult AddMrdDroppLetter(String parameter, String hash, String key)
        //{
        //    //objBAL = new MasterBAL();
        //    MrdDroppedViewModel model = new MrdDroppedViewModel();
        //    model.User_Action = "A";

        //    return PartialView("SaveDroppedLetters", model);
        //}

        [HttpPost]
        public ActionResult SaveDroppedLetters(String parameter, String hash, String key/*string id*/)
        {
            CommonFunctions comm = new CommonFunctions();
            MrdDroppedViewModel model = new MrdDroppedViewModel();
            dbContext = new PMGSYEntities();
            try
            {
                string stg = "";
                //int cId = Convert.ToInt32(id);

                model.EncryptedClearanceCode = parameter.Trim() + "/" + hash.Trim() + "/" + key.Trim();

                decryptedParameters = PMGSY.Common.URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                if (decryptedParameters.Count > 0)
                {
                    model.User_Action = Convert.ToString(Request.Params["User_Action"]).Trim();

                    int cId = Convert.ToInt32(decryptedParameters["ClearanceCode"]);

                    model.StateCode = Convert.ToInt32(dbContext.MRD_CLEARANCE_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Select(m => m.MASTER_STATE.MAST_STATE_CODE).FirstOrDefault());
                    model.Mast_Agency = Convert.ToInt32(dbContext.MRD_CLEARANCE_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Select(m => m.ADMIN_ND_CODE).FirstOrDefault());
                    model.MRD_CLEARANCE_DATE = comm.GetDateTimeToString(dbContext.MRD_CLEARANCE_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Select(m => m.MRD_CLEARANCE_DATE).FirstOrDefault());
                    //model.MRD_CLEARANCE_DATE = dbContext.MRD_DROPPED_LETTERS.Where(a => a.MRD_CLEARANCE_CODE == cId).Count() == 0 ? System.DateTime.Now.ToString("dd/MM/yyyy") : Convert.ToDateTime(dbContext.MRD_CLEARANCE_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Select(m => m.MRD_CLEARANCE_DATE).FirstOrDefault()).ToString("dd/MM/yyyy");
                    model.IMS_COLLABORATION = Convert.ToInt32(dbContext.MRD_CLEARANCE_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Select(m => m.MASTER_FUNDING_AGENCY.MAST_FUNDING_AGENCY_CODE).FirstOrDefault());
                    model.PhaseYear = Convert.ToInt32(dbContext.MRD_CLEARANCE_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Select(m => m.MAST_YEAR).FirstOrDefault());
                    model.Batch = Convert.ToInt32(dbContext.MRD_CLEARANCE_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Select(m => m.MAST_BATCH).FirstOrDefault());

                    model.StateName = dbContext.MRD_CLEARANCE_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Select(m => m.MASTER_STATE.MAST_STATE_NAME).FirstOrDefault();
                    model.Agency_Name = dbContext.MASTER_AGENCY.Where(m => m.MAST_AGENCY_CODE == model.Mast_Agency).Select(m => m.MAST_AGENCY_NAME).FirstOrDefault();
                    model.Collaboration_Name = dbContext.MRD_CLEARANCE_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Select(m => m.MASTER_FUNDING_AGENCY.MAST_FUNDING_AGENCY_NAME).FirstOrDefault();
                    model.PhaseYear = Convert.ToInt32(dbContext.MRD_CLEARANCE_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Select(m => m.MAST_YEAR).FirstOrDefault());
                    model.Batch_Name = "Batch " + dbContext.MRD_CLEARANCE_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Select(m => m.MAST_BATCH).FirstOrDefault();

                    model.StateList = new List<SelectListItem>();
                    model.StateList.Insert(0, (new SelectListItem { Text = model.StateName.Trim(), Value = Convert.ToString(model.StateCode).Trim() }));

                    model.Mast_AgencyList = new List<SelectListItem>();
                    model.Mast_AgencyList.Insert(0, (new SelectListItem { Text = model.Agency_Name.Trim(), Value = Convert.ToString(model.Mast_Agency).Trim() }));

                    model.COLLABORATIONS_List = new List<SelectListItem>();
                    model.COLLABORATIONS_List.Insert(0, (new SelectListItem { Text = model.Collaboration_Name.Trim(), Value = Convert.ToString(model.IMS_COLLABORATION).Trim() }));

                    model.PhaseYearList = new List<SelectListItem>();
                    model.PhaseYearList.Insert(0, (new SelectListItem { Text = Convert.ToString(model.PhaseYear).Trim() + "-" + Convert.ToString(model.PhaseYear + 1).Trim(), Value = Convert.ToString(model.PhaseYear).Trim() }));

                    model.BatchList = new List<SelectListItem>();
                    model.BatchList.Insert(0, (new SelectListItem { Text = model.Batch_Name.Trim(), Value = Convert.ToString(model.Batch).Trim() }));

                    model.clrDate = Convert.ToDateTime(dbContext.MRD_CLEARANCE_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Select(m => m.MRD_CLEARANCE_DATE).FirstOrDefault()).ToString("dd/MM/yyyy");

                    #region Hidden Clearance Details
                    model.hdnClrTotRoads = dbContext.MRD_CLEARANCE_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Select(m => m.MRD_TOTAL_ROADS).FirstOrDefault();
                    model.hdnClrTotBridge = dbContext.MRD_CLEARANCE_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Select(m => m.MRD_TOTAL_LSB).FirstOrDefault();
                    model.hdnClrRoadMrdShare = dbContext.MRD_CLEARANCE_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Select(m => m.MRD_ROAD_MORD_SHARE_AMT).FirstOrDefault();
                    model.hdnClrRoadStateShare = dbContext.MRD_CLEARANCE_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Select(m => m.MRD_ROAD_STATE_SHARE_AMT).FirstOrDefault();
                    //model.MRD_ROAD_TOTAL_AMT = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Select(m => m.MRD_ROAD_TOTAL_AMT).FirstOrDefault();
                    model.hdnClrBridgeMrdShare = dbContext.MRD_CLEARANCE_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Select(m => m.MRD_LSB_MORD_SHARE_AMT).FirstOrDefault();
                    model.hdnClrBridgeStateShare = dbContext.MRD_CLEARANCE_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Select(m => m.MRD_LSB_STATE_SHARE_AMT).FirstOrDefault();
                    //model.MRD_LSB_TOTAL_AMT = dbContext.MRD_CLEARANCE_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Select(m => m.MRD_LSB_TOTAL_AMT).FirstOrDefault();
                    //model.MRD_TOTAL_MORD_SHARE_AMT = dbContext.MRD_CLEARANCE_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Select(m => m.MRD_TOTAL_MORD_SHARE_AMT).FirstOrDefault();
                    //model.MRD_TOTAL_STATE_SHARE_AMT = dbContext.MRD_CLEARANCE_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Select(m => m.MRD_TOTAL_STATE_SHARE_AMT).FirstOrDefault();
                    //model.MRD_TOTAL_SANCTIONED_AMT = dbContext.MRD_CLEARANCE_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Select(m => m.MRD_TOTAL_SANCTIONED_AMT).FirstOrDefault();
                    model.hdnClrRoadLen = dbContext.MRD_CLEARANCE_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Select(m => m.MRD_TOTAL_ROAD_LENGTH).FirstOrDefault();
                    model.hdnClrBridgeLen = dbContext.MRD_CLEARANCE_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Select(m => m.MRD_TOTAL_LSB_LENGTH).FirstOrDefault();
                    model.hdnClrHAB1000 = dbContext.MRD_CLEARANCE_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Select(m => m.MRD_HAB_1000).FirstOrDefault();
                    model.hdnClrHAB500 = dbContext.MRD_CLEARANCE_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Select(m => m.MRD_HAB_500).FirstOrDefault();
                    model.hdnClrHAB250 = dbContext.MRD_CLEARANCE_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Select(m => m.MRD_HAB_250_ELIGIBLE).FirstOrDefault();
                    model.hdnClrHAB100 = dbContext.MRD_CLEARANCE_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Select(m => m.MRD_HAB_100_ELIGIBLE).FirstOrDefault();
                    #endregion

                    #region Hidden Dropped Details
                    model.hdnDropTotRoads = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Any() ? dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Select(m => m.MRD_TOTAL_ROADS).Sum() : 0;
                    model.hdnDropTotBridge = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Any() ? dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Select(m => m.MRD_TOTAL_LSB).Sum() : 0;
                    model.hdnDropRoadMrdShare = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Any() ? dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Select(m => m.MRD_ROAD_MORD_SHARE_AMT).Sum() : 0;
                    model.hdnDropRoadStateShare = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Any() ? dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Select(m => m.MRD_ROAD_STATE_SHARE_AMT).Sum() : 0;
                    //model.MRD_ROAD_TOTAL_AMT = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Select(m => m.MRD_ROAD_TOTAL_AMT).FirstOrDefault();
                    model.hdnDropBridgeMrdShare = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Any() ? dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Select(m => m.MRD_LSB_MORD_SHARE_AMT).Sum() : 0;
                    model.hdnDropBridgeStateShare = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Any() ? dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Select(m => m.MRD_LSB_STATE_SHARE_AMT).Sum() : 0;
                    //model.MRD_LSB_TOTAL_AMT = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Select(m => m.MRD_LSB_TOTAL_AMT).FirstOrDefault();
                    //model.MRD_TOTAL_MORD_SHARE_AMT = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Select(m => m.MRD_TOTAL_MORD_SHARE_AMT).FirstOrDefault();
                    //model.MRD_TOTAL_STATE_SHARE_AMT = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Select(m => m.MRD_TOTAL_STATE_SHARE_AMT).FirstOrDefault();
                    //model.MRD_TOTAL_SANCTIONED_AMT = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Select(m => m.MRD_TOTAL_SANCTIONED_AMT).FirstOrDefault();
                    model.hdnDropRoadLen = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Any() ? dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Select(m => m.MRD_TOTAL_ROAD_LENGTH).Sum() : 0;
                    model.hdnDropBridgeLen = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Any() ? dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Select(m => m.MRD_TOTAL_LSB_LENGTH).Sum() : 0;
                    model.hdnDropHAB1000 = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Any() ? dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Select(m => m.MRD_HAB_1000).Sum() : 0;
                    model.hdnDropHAB500 = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Any() ? dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Select(m => m.MRD_HAB_500).Sum() : 0;
                    model.hdnDropHAB250 = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Any() ? dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Select(m => m.MRD_HAB_250_ELIGIBLE).Sum() : 0;
                    model.hdnDropHAB100 = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Any() ? dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Select(m => m.MRD_HAB_100_ELIGIBLE).Sum() : 0;
                    #endregion
                    if (model.User_Action == "E")
                    {
                        int dropId = Convert.ToInt32(decryptedParameters["DroppedCode"]);

                        #region Hidden Dropped Details
                        model.hdnDropTotRoads = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Any() ? (dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Sum(m => m.MRD_TOTAL_ROADS) - dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_DROPPED_CODE == dropId).Select(m => m.MRD_TOTAL_ROADS).FirstOrDefault()) : 0;
                        model.hdnDropTotBridge = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Any() ? (dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Sum(m => m.MRD_TOTAL_LSB) - dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_DROPPED_CODE == dropId).Select(m => m.MRD_TOTAL_LSB).FirstOrDefault()) : 0;
                        model.hdnDropRoadMrdShare = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Any() ? (dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Sum(m => m.MRD_ROAD_MORD_SHARE_AMT) - dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_DROPPED_CODE == dropId).Select(m => m.MRD_ROAD_MORD_SHARE_AMT).FirstOrDefault()) : 0;
                        model.hdnDropRoadStateShare = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Any() ? (dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Sum(m => m.MRD_ROAD_STATE_SHARE_AMT) - dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_DROPPED_CODE == dropId).Select(m => m.MRD_ROAD_STATE_SHARE_AMT).FirstOrDefault()) : 0;
                        model.hdnDropBridgeMrdShare = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Any() ? (dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Sum(m => m.MRD_LSB_MORD_SHARE_AMT) - dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_DROPPED_CODE == dropId).Select(m => m.MRD_LSB_MORD_SHARE_AMT).FirstOrDefault()) : 0;
                        model.hdnDropBridgeStateShare = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Any() ? (dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Sum(m => m.MRD_LSB_STATE_SHARE_AMT) - dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_DROPPED_CODE == dropId).Select(m => m.MRD_LSB_STATE_SHARE_AMT).FirstOrDefault()) : 0;
                        model.hdnDropRoadLen = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Any() ? (dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Sum(m => m.MRD_TOTAL_ROAD_LENGTH) - dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_DROPPED_CODE == dropId).Select(m => m.MRD_TOTAL_ROAD_LENGTH).FirstOrDefault()) : 0;
                        model.hdnDropBridgeLen = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Any() ? (dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Sum(m => m.MRD_TOTAL_LSB_LENGTH) - dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_DROPPED_CODE == dropId).Select(m => m.MRD_TOTAL_LSB_LENGTH).FirstOrDefault()) : 0;
                        model.hdnDropHAB1000 = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Any() ? (dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Sum(m => m.MRD_HAB_1000) - dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_DROPPED_CODE == dropId).Select(m => m.MRD_HAB_1000).FirstOrDefault()) : 0;
                        model.hdnDropHAB500 = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Any() ? (dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Sum(m => m.MRD_HAB_500) - dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_DROPPED_CODE == dropId).Select(m => m.MRD_HAB_500).FirstOrDefault()) : 0;
                        model.hdnDropHAB250 = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Any() ? (dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Sum(m => m.MRD_HAB_250_ELIGIBLE) - dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_DROPPED_CODE == dropId).Select(m => m.MRD_HAB_250_ELIGIBLE).FirstOrDefault()) : 0;
                        model.hdnDropHAB100 = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Any() ? (dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Sum(m => m.MRD_HAB_100_ELIGIBLE) - dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_DROPPED_CODE == dropId).Select(m => m.MRD_HAB_100_ELIGIBLE).FirstOrDefault()) : 0;
                        #endregion

                        model.MRD_CLEARANCE_DATE = comm.GetDateTimeToString(dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_DROPPED_CODE == dropId).Select(m => m.MRD_DROPPED_DATE).FirstOrDefault());
                        model.MRD_CLEARANCE_NUMBER = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_DROPPED_CODE == dropId).Select(m => m.MRD_DROPPED_LETTER_NUMBER).FirstOrDefault();
                        model.MRD_TOTAL_ROADS = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_DROPPED_CODE == dropId).Select(m => m.MRD_TOTAL_ROADS).FirstOrDefault();
                        model.MRD_TOTAL_LSB = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_DROPPED_CODE == dropId).Select(m => m.MRD_TOTAL_LSB).FirstOrDefault();
                        model.MRD_ROAD_MORD_SHARE_AMT = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_DROPPED_CODE == dropId).Select(m => m.MRD_ROAD_MORD_SHARE_AMT).FirstOrDefault();
                        model.MRD_ROAD_STATE_SHARE_AMT = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_DROPPED_CODE == dropId).Select(m => m.MRD_ROAD_STATE_SHARE_AMT).FirstOrDefault();
                        model.MRD_ROAD_TOTAL_AMT = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_DROPPED_CODE == dropId).Select(m => m.MRD_ROAD_TOTAL_AMT).FirstOrDefault();
                        model.MRD_LSB_MORD_SHARE_AMT = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_DROPPED_CODE == dropId).Select(m => m.MRD_LSB_MORD_SHARE_AMT).FirstOrDefault();
                        model.MRD_LSB_STATE_SHARE_AMT = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_DROPPED_CODE == dropId).Select(m => m.MRD_LSB_STATE_SHARE_AMT).FirstOrDefault();
                        model.MRD_LSB_TOTAL_AMT = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_DROPPED_CODE == dropId).Select(m => m.MRD_LSB_TOTAL_AMT).FirstOrDefault();
                        model.MRD_TOTAL_MORD_SHARE_AMT = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_DROPPED_CODE == dropId).Select(m => m.MRD_TOTAL_MORD_SHARE_AMT).FirstOrDefault();
                        model.MRD_TOTAL_STATE_SHARE_AMT = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_DROPPED_CODE == dropId).Select(m => m.MRD_TOTAL_STATE_SHARE_AMT).FirstOrDefault();
                        model.MRD_TOTAL_SANCTIONED_AMT = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_DROPPED_CODE == dropId).Select(m => m.MRD_TOTAL_SANCTIONED_AMT).FirstOrDefault();
                        model.MRD_TOTAL_ROAD_LENGTH = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_DROPPED_CODE == dropId).Select(m => m.MRD_TOTAL_ROAD_LENGTH).FirstOrDefault();
                        model.MRD_TOTAL_LSB_LENGTH = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_DROPPED_CODE == dropId).Select(m => m.MRD_TOTAL_LSB_LENGTH).FirstOrDefault();
                        model.MRD_HAB_1000 = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_DROPPED_CODE == dropId).Select(m => m.MRD_HAB_1000).FirstOrDefault();
                        model.MRD_HAB_500 = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_DROPPED_CODE == dropId).Select(m => m.MRD_HAB_500).FirstOrDefault();
                        model.MRD_HAB_250_ELIGIBLE = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_DROPPED_CODE == dropId).Select(m => m.MRD_HAB_250_ELIGIBLE).FirstOrDefault();
                        model.MRD_HAB_100_ELIGIBLE = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_DROPPED_CODE == dropId).Select(m => m.MRD_HAB_100_ELIGIBLE).FirstOrDefault();
                        model.UPGRADE_CONNECT = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_DROPPED_CODE == dropId).Select(m => m.MRD_UPGRADE_CONNECT).FirstOrDefault();

                        model.MRD_DROPPED_REMARKS = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_DROPPED_CODE == dropId).Select(m => m.MRD_DROPPED_REMARKS).FirstOrDefault();

                        //model.hdnStage = model.STAGE_COMPLETE;
                    }
                    stg = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_CLEARANCE_CODE == cId).Select(m => m.MRD_STAGE_COMPLETE).FirstOrDefault();
                    model.STAGE_COMPLETE = stg == null ? "C" : stg;
                }
                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(string.Empty);
            }
        }

        [HttpPost]
        //public JsonResult AddDroppedLetters(PMGSY.Models.Proposal.MrdDroppedViewModel crMrdDroppedLetters)
        public JsonResult AddDroppedLetters(FormCollection frmCollection)
        {
            string xlname;
            IProposalBAL1 objBAL = new MRDProposalBAL();
            string message = String.Empty;
            bool status = false;
            int clrCode = 0;
            string[] p;
            dbContext = new PMGSYEntities();
            MrdDroppedViewModel model = new MrdDroppedViewModel();
            CommonFunctions objCommonFunc = new CommonFunctions();
            try
            {
                p = Request.Params["EncryptedClearanceCode"].Split('/');
                decryptedParameters = PMGSY.Common.URLEncrypt.DecryptParameters1(new String[] { p[0], p[1], p[2] });
                if (decryptedParameters.Count > 0)
                {
                    clrCode = Convert.ToInt32(decryptedParameters["ClearanceCode"]);
                }

                #region Save/Add Dropped Letter Details
                model.MRD_CLEARANCE_CODE = clrCode;
                model.User_Action = frmCollection["hdUserAction"].ToString();
                HttpPostedFileBase ClearancePdfFile = Request.Files["DroppedPdf"];
                HttpPostedFileBase RoadPdfFile = Request.Files["DroppedRoadPdf"];
                HttpPostedFileBase RoadExcelFile = Request.Files["DroppedRoadExcel"];
                string fileTypes = string.Empty;
                string[] arrfiletype = new string[5];
                bool fileExt = false;
                string filename = string.Empty;
                string fileExcelSaveExt = string.Empty;
                string filePdfSaveExt = string.Empty;
                string filePathClearancePdfFile = string.Empty;
                string filePathRoadPdfFile = string.Empty;
                string filePathRoadExcelFile = string.Empty;

                int fileSize = 0;
                string ext;

                #region File Validation
                if (ClearancePdfFile != null)
                {
                    fileTypes = ConfigurationManager.AppSettings["DROPPED_PDF_VALID_FORMAT"];
                    fileSize = Convert.ToInt32(ConfigurationManager.AppSettings["DROPPED_PDF_FILE_MAX_SIZE"]);

                    
                    FileInfo fileinfo = new FileInfo(Path.Combine(ConfigurationManager.AppSettings["DROPPED_PDF_FILE_UPLOAD"], ClearancePdfFile.FileName.Trim()));
                    ext = fileinfo.Extension;
                    ext = ext.Trim().Contains('.') ? ext.Trim().Remove(0, 1) : ext;
                    //if (fileTypes == ClearancePdfFile.FileName.Split('.')[1])
                    //if ((objCommonFunc.ValidateIsPdf(ConfigurationManager.AppSettings["DROPPED_PDF_FILE_UPLOAD"], Request)))
                    if(ext == fileTypes)
                    {
                        fileExt = true;
                        filePdfSaveExt = fileTypes;
                        filePathClearancePdfFile = ConfigurationManager.AppSettings["DROPPED_PDF_FILE_UPLOAD"];
                        model.MRD_CLEARANCE_PDF_FILE = ClearancePdfFile.FileName.Trim();
                    }

                    if (ClearancePdfFile.ContentLength > fileSize)
                    {
                        message = "Dropped Letter Pdf - File size cannot be greater than 4 MB.";
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }

                    if (fileExt == false)
                    {
                        message = "Dropped Letter Pdf - File type is not allowed.";
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    fileExt = false;
                }

                if (RoadPdfFile != null)
                {
                    fileTypes = ConfigurationManager.AppSettings["DROPPED_PDF_VALID_FORMAT"];
                    fileSize = Convert.ToInt32(ConfigurationManager.AppSettings["DROPPED_PDF_FILE_MAX_SIZE"]);

                    if (RoadPdfFile.ContentLength > fileSize)
                    {
                        message = "Road List Pdf - File size cannot be greater than 4 MB.";
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }

                    FileInfo fileinfo = new FileInfo(Path.Combine(ConfigurationManager.AppSettings["DROPPED_ROAD_PDF_FILE_UPLOAD"], RoadPdfFile.FileName.Trim()));
                    ext = fileinfo.Extension;
                    ext = ext.Trim().Contains('.') ? ext.Trim().Remove(0, 1) : ext;
                    //if (fileTypes == RoadPdfFile.FileName.Split('.')[1])
                    //if ((objCommonFunc.ValidateIsPdf(ConfigurationManager.AppSettings["DROPPED_ROAD_PDF_FILE_UPLOAD"], Request)))
                    if (ext == fileTypes)
                    {
                        fileExt = true;
                        filePdfSaveExt = fileTypes;
                        filePathRoadPdfFile = ConfigurationManager.AppSettings["DROPPED_ROAD_PDF_FILE_UPLOAD"];
                    }

                    if (fileExt == false)
                    {
                        message = "Road List Pdf - File type is not allowed.";
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    fileExt = false;
                }
                if (RoadExcelFile != null)
                {
                    fileSize = Convert.ToInt32(ConfigurationManager.AppSettings["DROPPED_ROAD_EXCEL_FILE_MAX_SIZE"]);
                    if (RoadExcelFile.ContentLength > fileSize)
                    {
                        message = "Road List Excel - File size cannot be greater than 4 MB.";
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }

                    fileTypes = ConfigurationManager.AppSettings["DROPPED_ROAD_EXCEL_VALID_FORMAT"];
                    arrfiletype = fileTypes.Split('$');
                    if (arrfiletype.Count() > 0)
                    {
                        arrfiletype[0] = arrfiletype[0].Trim().Contains('.') ? arrfiletype[0] : "." + arrfiletype[0];
                        arrfiletype[1] = arrfiletype[1].Trim().Contains('.') ? arrfiletype[1] : "." + arrfiletype[1];
                    }
                    foreach (var item in arrfiletype)
                    {
                        //if (item == RoadExcelFile.FileName.Split('.')[1])
                        if ((objCommonFunc.ValidateIsExcel(ConfigurationManager.AppSettings["DROPPED_ROAD_EXCEL_FILE_UPLOAD"], RoadExcelFile, item)))
                        {
                            fileExt = true;
                            fileExcelSaveExt = item;
                            fileExcelSaveExt = fileExcelSaveExt.Trim().Contains('.') ? fileExcelSaveExt.Trim().Remove(0,1) : fileExcelSaveExt;
                            filePathRoadExcelFile = ConfigurationManager.AppSettings["DROPPED_ROAD_EXCEL_FILE_UPLOAD"];
                            break;
                        }
                    }

                    if (fileExt == false)
                    {
                        message = "Road List Excel - File type is not allowed.";
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    fileExt = false;
                }
                #endregion

                #region Field Previous Value Validation
                if ((Convert.ToInt32(frmCollection["MRD_TOTAL_ROADS"]) + Convert.ToInt32(Request.Params["hdnDropTotRoads"])) > Convert.ToInt32(Request.Params["hdnClrTotRoads"]))
                {
                    message = "Total No. of Roads for Dropped Letter cannot be greater than Total No. of Roads for Clearance Letter";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
                if ((Convert.ToInt32(frmCollection["MRD_TOTAL_LSB"]) + Convert.ToInt32(Request.Params["hdnDropTotBridge"])) > Convert.ToInt32(Request.Params["hdnClrTotBridge"]))
                {
                    message = "Total No. of Bridges for Dropped Letter cannot be greater than Total No. of Bridges for Clearance Letter";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
                if ((Convert.ToDecimal(frmCollection["MRD_TOTAL_ROAD_LENGTH"]) + Convert.ToDecimal(Request.Params["hdnDropRoadLen"])) > Convert.ToDecimal(Request.Params["hdnClrRoadLen"]))
                {
                    message = "Total Road Length for Dropped Letter cannot be greater than Total Road Length of Bridges for Clearance Letter";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
                if ((Convert.ToDecimal(frmCollection["MRD_TOTAL_LSB_LENGTH"]) + Convert.ToDecimal(Request.Params["hdnDropBridgeLen"])) > Convert.ToDecimal(Request.Params["hdnClrBridgeLen"]))
                {
                    message = "Total Bridge Length for Dropped Letter cannot be greater than Total Bridge Length for Clearance Letter";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
                if ((Convert.ToDecimal(frmCollection["MRD_ROAD_MORD_SHARE_AMT"]) + Convert.ToDecimal(Request.Params["hdnDropRoadMrdShare"])) > Convert.ToDecimal(Request.Params["hdnClrRoadMrdShare"]))
                {
                    message = "Total Road MoRD Share for Dropped Letter cannot be greater than Total Road MoRD Share for Clearance Letter";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
                if ((Convert.ToDecimal(frmCollection["MRD_ROAD_STATE_SHARE_AMT"]) + Convert.ToDecimal(Request.Params["hdnDropRoadStateShare"])) > Convert.ToDecimal(Request.Params["hdnClrRoadStateShare"]))
                {
                    message = "Total Road State Share for Dropped Letter cannot be greater than Total Road State Share for Clearance Letter";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
                if ((Convert.ToDecimal(frmCollection["MRD_LSB_MORD_SHARE_AMT"]) + Convert.ToDecimal(Request.Params["hdnDropBridgeMrdShare"])) > Convert.ToDecimal(Request.Params["hdnClrBridgeMrdShare"]))
                {
                    message = "Total Bridge MoRD Share for Dropped Letter cannot be greater than Total Bridge MoRD Share for Clearance Letter";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
                if ((Convert.ToDecimal(frmCollection["MRD_LSB_STATE_SHARE_AMT"]) + Convert.ToDecimal(Request.Params["hdnDropBridgeStateShare"])) > Convert.ToDecimal(Request.Params["hdnClrBridgeStateShare"]))
                {
                    message = "Total Bridge State Share for Dropped Letter cannot be greater than Total Bridge State Share for Clearance Letter";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                if ((Convert.ToDecimal(frmCollection["MRD_HAB_1000"]) + Convert.ToDecimal(Request.Params["hdnDropHAB1000"])) > Convert.ToDecimal(Request.Params["hdnClrHAB1000"]))
                {
                    message = "Total Habs 1000 for Dropped Letter cannot be greater than Total Habs 1000 Roads for Clearance Letter";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
                if ((Convert.ToDecimal(frmCollection["MRD_HAB_500"]) + Convert.ToDecimal(Request.Params["hdnDropHAB500"])) > Convert.ToDecimal(Request.Params["hdnClrHAB500"]))
                {
                    message = "Total Habs 500 for Dropped Letter cannot be greater than Total Habs 500 Roads for Clearance Letter";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
                if ((Convert.ToDecimal(frmCollection["MRD_HAB_250_ELIGIBLE"]) + Convert.ToDecimal(Request.Params["hdnDropHAB250"])) > Convert.ToDecimal(Request.Params["hdnClrHAB250"]))
                {
                    message = "Total Habs 250 for Dropped Letter cannot be greater than Total Habs 250 Roads for Clearance Letter";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
                if ((Convert.ToDecimal(frmCollection["MRD_HAB_100_ELIGIBLE"]) + Convert.ToDecimal(Request.Params["hdnDropHAB100"])) > Convert.ToDecimal(Request.Params["hdnClrHAB100"]))
                {
                    message = "Total Habs 100 for Dropped Letter cannot be greater than Total Habs 100 Roads for Clearance Letter";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
                #endregion

                model.PhaseYear = Convert.ToInt32(frmCollection["PhaseYear"]);
                model.Batch = Convert.ToInt32(frmCollection["Batch"]); ;
                model.Mast_Agency = Convert.ToInt32(frmCollection["Mast_Agency"]); ;
                model.IMS_COLLABORATION = Convert.ToInt32(frmCollection["IMS_COLLABORATION"]);
                model.StateCode = Convert.ToInt32(frmCollection["Mast_State_Code"]) == 0 ? Convert.ToInt32(frmCollection["hdStateCode"]) : Convert.ToInt32(frmCollection["Mast_State_Code"]);
                model.MRD_CLEARANCE_DATE = frmCollection["MRD_CLEARANCE_DATE"];
                model.MRD_CLEARANCE_NUMBER = frmCollection["MRD_CLEARANCE_NUMBER"];
                model.MRD_TOTAL_ROADS = Convert.ToInt32(frmCollection["MRD_TOTAL_ROADS"]);
                model.MRD_TOTAL_LSB = Convert.ToInt32(frmCollection["MRD_TOTAL_LSB"]);
                model.MRD_ROAD_MORD_SHARE_AMT = Convert.ToDecimal(frmCollection["MRD_ROAD_MORD_SHARE_AMT"]);
                model.MRD_ROAD_STATE_SHARE_AMT = Convert.ToDecimal(frmCollection["MRD_ROAD_STATE_SHARE_AMT"]);
                model.MRD_ROAD_TOTAL_AMT = Convert.ToDecimal(frmCollection["MRD_ROAD_TOTAL_AMT"]);
                model.MRD_LSB_MORD_SHARE_AMT = Convert.ToDecimal(frmCollection["MRD_LSB_MORD_SHARE_AMT"]);
                model.MRD_LSB_STATE_SHARE_AMT = Convert.ToDecimal(frmCollection["MRD_LSB_STATE_SHARE_AMT"]);
                model.MRD_LSB_TOTAL_AMT = Convert.ToDecimal(frmCollection["MRD_LSB_TOTAL_AMT"]);
                model.MRD_TOTAL_MORD_SHARE_AMT = Convert.ToDecimal(frmCollection["MRD_TOTAL_MORD_SHARE_AMT"]);
                model.MRD_TOTAL_STATE_SHARE_AMT = Convert.ToDecimal(frmCollection["MRD_TOTAL_STATE_SHARE_AMT"]);
                model.MRD_TOTAL_SANCTIONED_AMT = Convert.ToDecimal(frmCollection["MRD_TOTAL_SANCTIONED_AMT"]);
                model.MRD_TOTAL_ROAD_LENGTH = Convert.ToDecimal(frmCollection["MRD_TOTAL_ROAD_LENGTH"]);
                model.MRD_TOTAL_LSB_LENGTH = Convert.ToDecimal(frmCollection["MRD_TOTAL_LSB_LENGTH"]);
                model.MRD_HAB_1000 = Convert.ToInt32(frmCollection["MRD_HAB_1000"]);
                model.MRD_HAB_500 = Convert.ToInt32(frmCollection["MRD_HAB_500"]);
                model.MRD_HAB_250_ELIGIBLE = Convert.ToInt32(frmCollection["MRD_HAB_250_ELIGIBLE"]);
                model.MRD_HAB_100_ELIGIBLE = Convert.ToInt32(frmCollection["MRD_HAB_100_ELIGIBLE"]);
                model.UPGRADE_CONNECT = Convert.ToString(frmCollection["UPGRADE_CONNECT"]);
                if (model.UPGRADE_CONNECT == "N")
                {
                    model.STAGE_COMPLETE = Convert.ToString(frmCollection["STAGE_COMPLETE"]);
                }
                model.MRD_DROPPED_REMARKS = Convert.ToString(frmCollection["MRD_DROPPED_REMARKS"]);
                if (ModelState.IsValid)
                {
                    //filename = Path.GetFileName(Request.Files["file"].FileName);
                    model.MRD_DROPPED_CODE = dbContext.MRD_DROPPED_LETTERS.Max(cp => (Int32?)cp.MRD_DROPPED_CODE) == null ? 1 : (Int32)dbContext.MRD_DROPPED_LETTERS.Max(cp => (Int32?)cp.MRD_DROPPED_CODE) + 1;

                    //if (Request.Params["User_Action"] == "A")
                    if (model.User_Action == "A")
                    {
                        if (ClearancePdfFile != null)
                        {
                            model.MRD_DROPPED_PDF_FILE = "DL_" + model.MRD_DROPPED_CODE + "." + filePdfSaveExt;
                        }
                        if (RoadPdfFile != null)
                        {
                            model.MRD_ROAD_PDF_FILE = "DR_" + model.MRD_DROPPED_CODE + "." + filePdfSaveExt;
                        }
                        if (RoadExcelFile != null)
                        {
                            model.MRD_ROAD_EXCEL_FILE = "DRX_" + model.MRD_DROPPED_CODE + "." + fileExcelSaveExt;
                        }

                        using (TransactionScope objScope = new TransactionScope())
                        {

                            if (objBAL.saveDroppedLettersBAL(model, ref message))
                            {
                                if (message == string.Empty)
                                {
                                    if (ClearancePdfFile != null)
                                    {
                                        Request.Files["DroppedPdf"].SaveAs(Path.Combine(filePathClearancePdfFile, model.MRD_DROPPED_PDF_FILE));
                                    }
                                    if (RoadPdfFile != null)
                                    {
                                        Request.Files["DroppedRoadPdf"].SaveAs(Path.Combine(filePathRoadPdfFile, model.MRD_ROAD_PDF_FILE));
                                    }
                                    if (RoadExcelFile != null)
                                    {
                                        Request.Files["DroppedRoadExcel"].SaveAs(Path.Combine(filePathRoadExcelFile, model.MRD_ROAD_EXCEL_FILE));
                                    }
                                }
                                objScope.Complete();
                                message = message == string.Empty ? "Dropped Letter  details saved successfully." : message;
                                status = true;
                                return Json(new { success = status, message = message == string.Empty ? "Dropped Letter details not saved" : message });
                            }
                            else
                            {
                                return Json(new { success = status, message = message == string.Empty ? "Dropped Letter details not saved" : message });
                            }
                        }
                    }
                    else
                    {
                        model.MRD_DROPPED_CODE = Convert.ToInt32(decryptedParameters["DroppedCode"]); ;// model.MRD_DROPPED_CODE - 1;
                        xlname = "";
                        xlname = dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_DROPPED_CODE == model.MRD_DROPPED_CODE).Select(m => m.MRD_ROAD_EXCEL_FILE).FirstOrDefault();

                        if (ClearancePdfFile != null)
                        {
                            model.MRD_DROPPED_PDF_FILE = "DL_" + model.MRD_DROPPED_CODE + "." + filePdfSaveExt;
                        }
                        if (RoadPdfFile != null)
                        {
                            model.MRD_ROAD_PDF_FILE = "DR_" + model.MRD_DROPPED_CODE + "." + filePdfSaveExt;
                        }
                        if (RoadExcelFile != null)
                        {
                            model.MRD_ROAD_EXCEL_FILE = "DRX_" + model.MRD_DROPPED_CODE + "." + fileExcelSaveExt;
                        }

                        using (TransactionScope objScope = new TransactionScope())
                        {

                            if (objBAL.editDroppedLettersBAL(model))
                            {
                                if (message == string.Empty)
                                {
                                    if (ClearancePdfFile != null)
                                    {
                                        System.IO.File.Delete(Path.Combine(filePathClearancePdfFile, model.MRD_DROPPED_PDF_FILE));
                                        Request.Files["DroppedPdf"].SaveAs(Path.Combine(filePathClearancePdfFile, model.MRD_DROPPED_PDF_FILE));
                                    }
                                    if (RoadPdfFile != null)
                                    {
                                        System.IO.File.Delete(Path.Combine(filePathRoadPdfFile, model.MRD_ROAD_PDF_FILE));
                                        Request.Files["DroppedRoadPdf"].SaveAs(Path.Combine(filePathRoadPdfFile, model.MRD_ROAD_PDF_FILE));
                                    }
                                    if (RoadExcelFile != null)
                                    {
                                        System.IO.File.Delete(Path.Combine(filePathRoadExcelFile, model.MRD_ROAD_EXCEL_FILE));
                                        if (xlname != null)
                                        {
                                            System.IO.File.Delete(Path.Combine(filePathRoadExcelFile, xlname));
                                        }
                                        Request.Files["DroppedRoadExcel"].SaveAs(Path.Combine(filePathRoadExcelFile, model.MRD_ROAD_EXCEL_FILE));
                                    }
                                }
                                objScope.Complete();
                                message = message == string.Empty ? "Dropped Letter  details updated successfully." : message;
                                status = true;
                                return Json(new { success = status, message = message == string.Empty ? "Dropped Letter details not updated" : message });
                            }
                            else
                            {
                                return Json(new { success = status, message = message == string.Empty ? "Dropped Letter details not updated" : message });
                            }
                        }

                    }
                }
                else
                {
                    return Json(new { success = status, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
                #endregion
                //return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                //return null;
                return Json(new { success = status, message = message == string.Empty ? "Dropped Letter details not saved" : message });
            }
        }

        [HttpPost]
        public ActionResult delMrdDroppedLetter(String parameter, String hash, String key)
        {
            dbContext = new PMGSYEntities();
            int drCode = 0;
            string[] p;
            string message = "";
            IProposalBAL1 objBAL = new MRDProposalBAL();
            bool status = false;
            MrdDroppedViewModel model = new MrdDroppedViewModel();

            HttpPostedFileBase ClearancePdfFile = Request.Files["DroppedPdf"];
            HttpPostedFileBase RoadPdfFile = Request.Files["DroppedRoadPdf"];
            HttpPostedFileBase RoadExcelFile = Request.Files["DroppedRoadExcel"];

            string filePathClearancePdfFile = ConfigurationManager.AppSettings["DROPPED_PDF_FILE_UPLOAD"];
            string filePathRoadPdfFile = ConfigurationManager.AppSettings["DROPPED_ROAD_PDF_FILE_UPLOAD"];
            string filePathRoadExcelFile = ConfigurationManager.AppSettings["DROPPED_ROAD_EXCEL_FILE_UPLOAD"];

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    drCode = Convert.ToInt32(decryptedParameters["DroppedCode"]);

                    model.MRD_DROPPED_PDF_FILE = Convert.ToString(dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_DROPPED_CODE == drCode).Select(m => m.MRD_DROPPED_PDF_FILE).FirstOrDefault());
                    model.MRD_ROAD_PDF_FILE = Convert.ToString(dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_DROPPED_CODE == drCode).Select(m => m.MRD_ROAD_PDF_FILE).FirstOrDefault());
                    model.MRD_ROAD_EXCEL_FILE = Convert.ToString(dbContext.MRD_DROPPED_LETTERS.Where(m => m.MRD_DROPPED_CODE == drCode).Select(m => m.MRD_ROAD_EXCEL_FILE).FirstOrDefault());

                    if (objBAL.deleteDroppedLettersBAL(drCode))
                    {

                        status = true;
                        if (model.MRD_DROPPED_PDF_FILE != "" && model.MRD_DROPPED_PDF_FILE != null)
                        {
                            System.IO.File.Delete(Path.Combine(filePathClearancePdfFile, model.MRD_DROPPED_PDF_FILE));
                        }
                        if (model.MRD_ROAD_PDF_FILE != "" && model.MRD_ROAD_PDF_FILE != null)
                        {
                            System.IO.File.Delete(Path.Combine(filePathRoadPdfFile, model.MRD_ROAD_PDF_FILE));
                        }
                        if (model.MRD_ROAD_EXCEL_FILE != "" && model.MRD_ROAD_EXCEL_FILE != null)
                        {
                            System.IO.File.Delete(Path.Combine(filePathRoadExcelFile, model.MRD_ROAD_EXCEL_FILE));
                        }

                        message = message == string.Empty ? "Dropped Letter  details deleted successfully." : message;

                        return Json(new { success = status, message = message == string.Empty ? "Dropped Letter details not deleted" : message });
                        //return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        message = "You can not delete this Dropped Letter details.";
                        return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                //}
                else
                {
                    message = "You can not delete this Dropped Letter details.";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "You can not delete this Dropped Letter details.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }

        }
    }
}
