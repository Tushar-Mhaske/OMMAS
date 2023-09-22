
#region FileHeader
/*----------------------------------------------------------------------------------------
 * Controller Name: Maintainance Inspection Controller.
 * Path: PMGSY/Controller/MaintainanceInspectionController.
 * Created By:Ashish Markande
 * Creation Date:27/06/2013
 * Purpose:Add/Edit/Delete and to load grid view of maintainance inspection.
 * ---------------------------------------------------------------------------------------
 */
#endregion FileHeader

using PMGSY.BAL.MaintainanceInspection;
using PMGSY.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.DAL.MaintainanceInspection;
using PMGSY.Common;
using PMGSY.Models.Common;
using PMGSY.Extensions;
using PMGSY.Models.MaintainanceInspection;
using System.Configuration;
using System.Web.Script.Serialization;
using System.Text;
using ExifLib;
using System.IO;
using System.Text.RegularExpressions;
namespace PMGSY.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class MaintainanceInspectionController : Controller
    {

        MaintainanceInspectionBAL objBAL = new MaintainanceInspectionBAL();
        IMaintenanceInspectionDAL objDAL = new MaintainanceInspectionDAL();

        #region InspectionControllerActions
        //
        // GET: /MaintainanceInspection/

        private PMGSYEntities dbContext = new PMGSYEntities();
        Dictionary<string, string> decryptedParameters = null;
        String[] encryptedParameters = null;

        IMaintenanceInspectionBAL maintenanceInspectionBAL = new MaintainanceInspectionBAL();
        MaintainanceInspectionDAL maintenanceInspectionDAL = new MaintainanceInspectionDAL();
        CommonFunctions commonFunction = new CommonFunctions();

        string message = string.Empty;

        /// <summary>
        /// Method to load search view and listing of proposals.
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult ProposalMaintenanceInspection()
        {
            //DAL.MasterDataEntryDAL masterDataEntryDAL = new DAL.MasterDataEntryDAL();
            try
            {
                TransactionParams transactionParams = new TransactionParams();
                transactionParams.STATE_CODE = PMGSYSession.Current.StateCode;
                transactionParams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                transactionParams.ISSearch = true;
                transactionParams.SANC_YEAR = (Int16)DateTime.Now.Year;

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    ViewData["FinancialYearList"] = commonFunction.PopulateFinancialYear(true, true);
                    ViewData["BlockList"] = commonFunction.PopulateBlocks(PMGSYSession.Current.DistrictCode, true); //new SelectList(masterDataEntryDAL.GetAllBlocksByDistrictCode(transactionParams.DISTRICT_CODE, true), "MAST_BLOCK_CODE", "MAST_BLOCK_NAME");
                    ViewData["PackageList"] = new SelectList(commonFunction.GetPackages(Convert.ToInt32(transactionParams.SANC_YEAR), 0, true), "IMS_PACKAGE_ID", "IMS_PACKAGE_ID");
                    //new filters added by Vikram as per suggested by Dev Sir
                    ViewData["BatchList"] = commonFunction.PopulateBatch(true);
                    ViewData["CollaborationList"] = commonFunction.PopulateFundingAgency(true);
                    ViewData["UpgradationList"] = commonFunction.PopulateNewUpgradeList(true);
                    //end of change
                }

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                ViewData["FinancialYearList"] = null;
                ViewData["PackageList"] = null;
            }

            return View("ProposalMaintenanceInspection");

        }

        /// <summary>
        /// Method to get complete road list.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetCompletedRoadList(int? page, int? rows, string sidx, string sord)
        {

            long totalRecords;
            int stateCode = PMGSYSession.Current.StateCode;
            int districtCode = PMGSYSession.Current.DistrictCode;
            int adminNDCode = PMGSYSession.Current.AdminNdCode;
            int sanctionedYear = 0;
            int blockCode = 0;
            string packageID = string.Empty;
            int batch = 0;
            int collaboration = 0;
            string upgradationType = string.Empty;

            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["sanctionedYear"]))
                {
                    sanctionedYear = Convert.ToInt32(Request.Params["sanctionedYear"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["blockCode"]))
                {
                    blockCode = Convert.ToInt32(Request.Params["blockCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["packageID"]))
                {
                    packageID = Request.Params["packageID"].Trim();
                }
                else
                {
                    return null;
                }

                if (!string.IsNullOrEmpty(Request.Params["batch"]))
                {
                    batch = Convert.ToInt32(Request.Params["batch"].Trim());
                }

                if (!string.IsNullOrEmpty(Request.Params["collaboration"]))
                {
                    collaboration = Convert.ToInt32(Request.Params["collaboration"].Trim());
                }

                if (!string.IsNullOrEmpty(Request.Params["upgradationType"]))
                {
                    upgradationType = Request.Params["upgradationType"].Trim();
                }


                var jsonData = new
                {
                    rows = maintenanceInspectionBAL.GetCompletedRoadListBAL(stateCode, districtCode, blockCode, sanctionedYear, adminNDCode, packageID, batch, collaboration, upgradationType, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
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

        [Audit]
        public ActionResult AddInspection(String parameter, String hash, String key)
        {
            MaintainanceInspectionViewModel maintenenaceInspectionDetails = new MaintainanceInspectionViewModel();
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {

                    maintenenaceInspectionDetails.EncryptedIMSPRRoadCode = parameter + '/' + hash + '/' + key;
                    ViewBag.SanctionedYear = decryptedParameters["SanctionedYear"];
                    ViewBag.RoadName = decryptedParameters["IMSRoadName"].ToString();
                    ViewBag.Package = decryptedParameters["Package"].ToString();
                    // ViewData["AdminName"] = maintenanceInspectionDAL.GetAdminNdName();
                    maintenenaceInspectionDetails.IMS_PR_ROAD_CODE = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"]);
                    return PartialView("AddMaintenanceInspectionAgainstRoad", maintenenaceInspectionDetails);
                }
                return PartialView("AddMaintenanceInspectionAgainstRoad", maintenenaceInspectionDetails);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return PartialView("AddMaintenanceInspectionAgainstRoad", maintenenaceInspectionDetails);
            }
        }

        /// <summary>
        /// Method to load inspection view.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult AddInspectionView(string id)
        {

            encryptedParameters = id.ToString().Split('/');
            decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
            int prRoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"]);
            MaintainanceInspectionViewModel maintainanceView = maintenanceInspectionDAL.GetInspectionDetails(prRoadCode);
            maintainanceView.IMS_PR_ROAD_CODE = prRoadCode;
            ViewBag.SanctionedYear = maintainanceView.SactionedYear;
            ViewBag.RoadName = maintainanceView.RoadName;
            ViewBag.Package = maintainanceView.Package;
            ViewBag.Block = maintainanceView.BlockName;
            return PartialView("AddMaintenanceInspectionAgainstRoad", maintainanceView);
        }

        /// <summary>
        /// Method to load inspection data entry form.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult AddInspectionDetail(string id)
        {
            MaintainanceInspectionViewModel inspectionView = new MaintainanceInspectionViewModel();
            inspectionView.IMS_PR_ROAD_CODE = Convert.ToInt32(id);
            inspectionView.InspectionDate = maintenanceInspectionDAL.LastInspectionDate(Convert.ToInt32(id));
            inspectionView.StartDate = Convert.ToDateTime(maintenanceInspectionDAL.getInspectionDate(inspectionView.IMS_PR_ROAD_CODE)).ToString("dd/MM/yyyy");
            ViewData["AdminName"] = new SelectList(maintenanceInspectionDAL.GetAdminNdName("0"), "Value", "Text");
            ViewData["Designation"] = new SelectList(maintenanceInspectionDAL.PopulateDesignation(), "Value", "Text");
            inspectionView.statusFlag = "";

            return PartialView("AddInspectionDetail", inspectionView);
        }


        //public ActionResult ChangeInspectionStatus(string id)
        //{
        //    MaintainanceInspectionViewModel inspectionView = new MaintainanceInspectionViewModel();
        //    inspectionView.IMS_PR_ROAD_CODE = Convert.ToInt32(id);
        //    inspectionView.InspectionDate = maintenanceInspectionDAL.LastInspectionDate(Convert.ToInt32(id));
        //    inspectionView.StartDate = Convert.ToDateTime(maintenanceInspectionDAL.getInspectionDate(inspectionView.IMS_PR_ROAD_CODE)).ToString("dd/MM/yyyy");
        //    inspectionView.statusFlag = 1;
        //    ViewData["AdminName"] = new SelectList(maintenanceInspectionDAL.GetAdminNdName("0"), "Value", "Text");
        //    ViewData["Designation"] = new SelectList(maintenanceInspectionDAL.PopulateDesignation(), "Value", "Text");
        //    return PartialView("AddInspectionDetail", inspectionView);
        //}


        [HttpPost]
        [Audit]
        public ActionResult GetNodalOfficerName(string desigCode)
        {
            try
            {
                return Json(new SelectList(maintenanceInspectionDAL.GetAdminNdName(desigCode), "Value", "Text"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return Json(false);
            }
        }

        /// <summary>
        /// Method to load inspection details list.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetInspectionRoadList(int? page, int? rows, string sidx, string sord)
        {

            long totalRecords;
            int proposalCode = 0;

            try
            {

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["ImsPrRoadCode"]))
                {
                    proposalCode = Convert.ToInt32(Request.Params["ImsPrRoadCode"]);
                }

                var jsonData = new
                {
                    rows = maintenanceInspectionBAL.GetInspectionRoadList(proposalCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
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
        /// Method to save inspection details.
        /// </summary>
        /// <param name="inspectionModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult AddMaintainanceInspection(MaintainanceInspectionViewModel inspectionModel)
        {
            bool status = false;
            try
            {

                if (ModelState.IsValid)
                {

                    if (maintenanceInspectionBAL.SaveInspectionDetails(inspectionModel, ref message))
                    {
                        message = message == string.Empty ? "Inspection details added successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Inspection details not added successfully." : message;
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
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Inspection details not added successfully.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Method to get existing entry into details form.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult EditMaintainanceInspection(String parameter, String hash, String key)
        {
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                int prRoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"]);
                int inspectionCode = Convert.ToInt32(decryptedParameters["IMSInspectionCode"]);
                string flag = decryptedParameters["Flag"];

                if (decryptedParameters.Count > 0)
                {
                    MaintainanceInspectionViewModel maintainanceView = maintenanceInspectionBAL.GetMaintainanceInspection_ByRoadCode(prRoadCode, inspectionCode);

                    ViewBag.SanctionedYear = maintainanceView.SactionedYear;
                    ViewBag.RoadName = maintainanceView.RoadName;
                    ViewBag.Package = maintainanceView.Package;
                    ViewData["AdminName"] = new SelectList(maintenanceInspectionDAL.GetAdminNdName("0"), "Value", "Text");
                    ViewData["Designation"] = new SelectList(maintenanceInspectionDAL.PopulateDesignation(), "Value", "Text");
                    maintainanceView.StartDate = Convert.ToDateTime(maintenanceInspectionDAL.getInspectionDate(maintainanceView.IMS_PR_ROAD_CODE)).ToString("dd/MM/yyyy");
                    //ViewBag.AdminName = maintainanceView.MAST_OFFICER_CODE;
                    //ViewBag.Designation= maintainanceView.Designation;
                    maintainanceView.EncryptedPRRoadCode = prRoadCode.ToString();

                    if (flag == "true")
                    {
                        maintainanceView.statusFlag = "true";
                    }

                    //Added By Abhishek kamble start 18-Mar-2014
                    maintainanceView.statusFlag = "true";
                    //Added By Abhishek kamble end 18-Mar-2014


                    if (maintainanceView == null)
                    {
                        ModelState.AddModelError(string.Empty, "Inspection Details does not exists");
                        return PartialView("AddInspectionDetail", new MaintainanceInspectionViewModel());
                    }
                    return PartialView("AddInspectionDetail", maintainanceView);
                }
                return PartialView("AddInspectionDetail", new MaintainanceInspectionViewModel());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                ModelState.AddModelError(string.Empty, "Qualification details not Exist.");

                return PartialView("AddInspectionDetail", new MaintainanceInspectionViewModel());
            }

        }

        /// <summary>
        /// Method to update inspection details.
        /// </summary>
        /// <param name="inspectionModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult EditMaintainanceInspection(MaintainanceInspectionViewModel inspectionModel)
        {
            bool status = false;

            try
            {
                if (ModelState.IsValid)
                {
                    if (maintenanceInspectionBAL.EditInspectionDetails(inspectionModel, ref message))
                    {
                        message = message == string.Empty ? "Inspection details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Inspection details not updated." : message;
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
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Qualification details not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Method to delete inspection details.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult DeleteMaintainanceInspection(String parameter, String hash, String key)
        {
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                int prRoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"]);
                int inspectionCode = Convert.ToInt32(decryptedParameters["IMSInspectionCode"]);

                if (decryptedParameters.Count() > 0)
                {
                    if (!maintenanceInspectionBAL.DeleteInspectionDetails(prRoadCode, inspectionCode))
                    {
                        ModelState.AddModelError(string.Empty, "Inspection details not deleted.");
                        return Json(new { success = false, message = "You can not delete this inspection details." }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { success = false, message = "You can not delete this inspection details." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "You can not delete this inspection details." }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion InspectionControllerActions

        #region FINANCIAL_PROGRESS

        /// <summary>
        /// returns the basic view for showing the financial details.
        /// </summary>
        /// <param name="urlparameter"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult ListFinancialDetails(string urlparameter)
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                MaintainanceInspectionDAL objDAL = new MaintainanceInspectionDAL();
                MaintenanceProgressViewModel progressModel = new MaintenanceProgressViewModel();
                int proposalCode = 0;
                string progressType = string.Empty;
                String[] encryptedParameters = null;
                Dictionary<string, string> decryptedParameters = null;
                if (urlparameter != string.Empty)
                {
                    encryptedParameters = urlparameter.Split('/');
                    if (encryptedParameters.Length == 3)
                    {
                        decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    }
                }
                proposalCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);
                progressModel = objDAL.GetFinancialAddDetails(proposalCode);
                progressModel.ProposalCode = proposalCode;
                ViewData["Year"] = objCommon.PopulateYears(false);
                ViewData["Month"] = objCommon.PopulateMonths();
                ViewData["urlparameter"] = urlparameter;
                return View("ListFinancialDetails", progressModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return View("ListFinancialDetails");
            }
        }

        /// <summary>
        /// returns the list of Financial Progress
        /// </summary>
        /// <param name="progressCollection">grid parameters along with filter parameters</param>
        /// <returns>list for populating grid</returns>
        [HttpPost]
        [Audit]
        public ActionResult GetFinancialProgressList(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                long totalRecords = 0;
                int proposalCode = 0;
                string progressType = string.Empty;
                int contractCode = 0;
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!(string.IsNullOrEmpty(Request.Params["roadCode"])))
                {
                    proposalCode = Convert.ToInt32(Request.Params["roadCode"]);
                }

                if (!(string.IsNullOrEmpty(Request.Params["progressType"])))
                {
                    progressType = Request.Params["progressType"];
                }

                if (!(string.IsNullOrEmpty(Request.Params["contractCode"])))
                {
                    contractCode = Convert.ToInt32(Request.Params["contractCode"]);
                }

                var jsonData = new
                {
                    rows = objBAL.GetFinancialProgressList(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, proposalCode, progressType, contractCode),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
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
        /// returns the add view of Financial road details 
        /// </summary>
        /// <param name="id">encrypted id key</param>
        /// <returns></returns>
        [Audit]
        public ActionResult AddFinancialProgress(string id)
        {
            try
            {
                int proposalCode = 0;
                int contractCode = 0;
                if (!string.IsNullOrEmpty(id))
                {
                    string[] parameters = id.Split('$');
                    proposalCode = Convert.ToInt32(parameters[0]);
                    contractCode = Convert.ToInt32(parameters[1]);
                }
                MaintenanceProgressViewModel progressModel = new MaintenanceProgressViewModel();
                progressModel = objDAL.GetFinancialAddDetails(proposalCode, contractCode);
                progressModel.ProposalCode = proposalCode;
                progressModel.ProposalContractCode = contractCode;
                CommonFunctions objCommon = new CommonFunctions();
                progressModel.Operation = "A";
                ViewData["Year"] = objCommon.PopulateYears(true);
                ViewData["Month"] = objCommon.PopulateMonths();
                return View("AddFinancialProgress", progressModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        /// <summary>
        /// save the data of Financial details
        /// </summary>
        /// <param name="progreeModel">model containing the financial details</param>
        /// <returns>status along with message</returns>
        [HttpPost]
        [Audit]
        public ActionResult AddFinancialProgress(MaintenanceProgressViewModel progressModel)
        {
            string message = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    if (!(objBAL.AddFinancialProgress(progressModel, ref message)))
                    {
                        if (message == string.Empty)
                        {
                            return Json(new { success = false, message = "Financial details not added successfully." });
                        }
                        else
                        {
                            return Json(new { success = false, message = message });
                        }
                    }
                    else
                    {
                        return Json(new { success = true, message = message });
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
                return Json(new { success = false, message = "Financial details not added successfully." });
            }
        }

        /// <summary>
        /// return the financial details for updation
        /// </summary>
        /// <param name="parameter">encrypted id for updation</param>
        /// <param name="hash">encrypted id for updation</param>
        /// <param name="key">encrypted id for updation</param>
        /// <returns>return view along with model containing data</returns>
        [Audit]
        public ActionResult EditFinancialDetails(String parameter, String hash, String key)
        {
            try
            {
                Dictionary<string, string> decryptedParameters = null;
                CommonFunctions objCommon = new CommonFunctions();
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                int proposalCode = 0;   //for storing decrypted proposal code
                int monthCode = 0;  //for storing decrypted month code
                int yearCode = 0;   //for storing decrypted year code
                int contractCode = 0; //for storing type of progress
                if (decryptedParameters != null)
                {
                    proposalCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);
                    monthCode = Convert.ToInt32(decryptedParameters["Month"]);
                    yearCode = Convert.ToInt32(decryptedParameters["Year"]);
                    contractCode = Convert.ToInt32(decryptedParameters["ContractCode"]);
                }
                ViewData["Year"] = objCommon.PopulateYears(true);
                ViewData["Month"] = objCommon.PopulateMonths();
                MaintenanceProgressViewModel progressModel = objBAL.GetFinancialDetails(proposalCode, contractCode, yearCode, monthCode);
                progressModel.PreviousYear = 0;
                if (progressModel != null)
                {
                    return PartialView("AddFinancialProgress", progressModel);
                }
                else
                {
                    return Json(new { success = false, message = "Request can not be processed at this time." });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Request can not be processed at this time." });
            }
        }

        /// <summary>
        /// updates the financial details
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult EditFinancialProgress(MaintenanceProgressViewModel progressModel)
        {
            string message = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    if (!(objBAL.EditFinancialProgress(progressModel, ref message)))
                    {
                        if (message == string.Empty)
                        {
                            return Json(new { success = false, message = "Financial details not updated successfully." });
                        }
                        else
                        {
                            return Json(new { success = false, message = message });
                        }
                    }
                    else
                    {
                        return Json(new { success = true, message = message });
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
                return Json(new { success = false, message = "Financial details not updated successfully." });
            }
        }

        /// <summary>
        /// delete operation for Financial details
        /// </summary>
        /// <param name="parameter">encrypted id key</param>
        /// <param name="hash">encrypted id key</param>
        /// <param name="key">encrypted id key</param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult DeleteFinancialDetails(String parameter, String hash, String key)
        {
            string message = string.Empty;
            Dictionary<string, string> decryptedParameters = null;
            int proposalCode = 0;
            int monthCode = 0;
            int yearCode = 0;
            int contractCode = 0;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                proposalCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);
                yearCode = Convert.ToInt32(decryptedParameters["Year"]);
                monthCode = Convert.ToInt32(decryptedParameters["Month"]);
                contractCode = Convert.ToInt32(decryptedParameters["ContractCode"]);
                if ((objBAL.DeleteFinancialRoadDetails(proposalCode, contractCode, yearCode, monthCode, ref message)))
                {
                    if (message == string.Empty)
                    {
                        return Json(new { success = true, message = "Financial details deleted successfully." });
                    }
                    else
                    {
                        return Json(new { success = true, message = message });
                    }
                }
                else
                {
                    return Json(new { success = false, message = message });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error Occurred while processing your request." });
            }
        }

        /// <summary>
        /// validates the sanction cost associated with the road for validating the financial details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult CheckSanctionCost(string id)
        {
            IMaintenanceInspectionBAL objBAL = new MaintainanceInspectionBAL();
            try
            {
                string[] parameters = id.Split('$');
                int proposalCode = Convert.ToInt32(parameters[0]);
                decimal valueofWork = 0;
                decimal valueofPayment = 0;
                int contractCode = 0;
                if (parameters[1] != null)
                {
                    valueofWork = Convert.ToDecimal(parameters[1]);
                }
                if (parameters[1] != null)
                {
                    valueofPayment = Convert.ToDecimal(parameters[2]);
                }
                string operation = parameters[3];
                contractCode = Convert.ToInt32(parameters[4]);
                bool status = objBAL.CheckSanctionValue(proposalCode, valueofWork, valueofPayment, operation, contractCode);
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
        /// returns the maintenance details associated with the proposal to display it on view
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult GetMaintenanceDetails(string id)
        {
            CommonFunctions comm = new CommonFunctions();
            try
            {
                PMGSYEntities db = new PMGSYEntities();
                if (!string.IsNullOrEmpty(id))
                {
                    string[] parameters = id.Split('$');
                    int proposalCode = Convert.ToInt32(parameters[0]);
                    int contractCode = Convert.ToInt32(parameters[1]);
                    string date = string.Empty;
                    string aggCost = string.Empty;
                    string overallCost = string.Empty;
                    string roadName = string.Empty;
                    string roadLength = string.Empty;
                    List<MANE_IMS_CONTRACT> lstContracts = dbContext.MANE_IMS_CONTRACT.Where(m => m.IMS_PR_ROAD_CODE == proposalCode && m.MANE_CONTRACT_NUMBER == contractCode && m.MANE_CONTRACT_STATUS != "I").ToList();
                    decimal? costToAdd = 0;
                    if (lstContracts != null)
                    {
                        foreach (var item in lstContracts)
                        {
                            costToAdd = costToAdd + item.MANE_YEAR1_AMOUNT + item.MANE_YEAR2_AMOUNT + item.MANE_YEAR3_AMOUNT + item.MANE_YEAR4_AMOUNT + item.MANE_YEAR5_AMOUNT;

                            date = comm.GetDateTimeToString(item.MANE_AGREEMENT_DATE).Trim();
                            roadName = item.IMS_SANCTIONED_PROJECTS.IMS_ROAD_NAME.Trim();
                            roadLength = Convert.ToString(item.IMS_SANCTIONED_PROJECTS.IMS_PAV_LENGTH);
                        }
                    }

                    decimal? agreementCost = (dbContext.MANE_IMS_CONTRACT.Where(m => m.IMS_PR_ROAD_CODE == proposalCode && m.MANE_CONTRACT_STATUS == "I" && m.MANE_CONTRACT_NUMBER == contractCode).Sum(m => (Decimal?)m.MANE_VALUE_WORK_DONE) == null ? 0 : dbContext.MANE_IMS_CONTRACT.Where(m => m.IMS_PR_ROAD_CODE == proposalCode && m.MANE_CONTRACT_STATUS == "I" && m.MANE_CONTRACT_NUMBER == contractCode).Sum(m => (Decimal?)m.MANE_VALUE_WORK_DONE)) + costToAdd;
                    return Json(new { AllCost = agreementCost, Date = date, RoadName = roadName, RoadLength = roadLength });
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
        /// returns the maintenance no. of associated proposal / road
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult PopulateMaintenanceNo(string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    return Json(objDAL.PopulateMaintenanceNo(id));
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
        /// returns the list of agreements done on the road/proposal
        /// </summary>
        /// <param name="id"> proposal code</param>
        /// <returns></returns>
        [Audit]
        public JsonResult PopulateAgreements(string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    string[] parameters = id.Split('$');
                    int proposalCode = Convert.ToInt32(parameters[0]);
                    int maintenanceNo = Convert.ToInt32(parameters[1]);
                    List<SelectListItem> lstAgreements = new List<SelectListItem>();
                    var lstContracts = dbContext.MANE_IMS_CONTRACT.Where(m => m.IMS_PR_ROAD_CODE == proposalCode && m.MANE_CONTRACT_NUMBER == maintenanceNo).ToList();
                    if (lstContracts != null)
                    {
                        foreach (var item in lstContracts)
                        {
                            lstAgreements.Add(new SelectListItem { Value = item.MANE_PR_CONTRACT_CODE.ToString(), Text = item.MANE_AGREEMENT_NUMBER.ToString() });
                        }
                    }
                    return Json(lstAgreements);
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
        /// returns the JSON for populating the agreements done on the respective road.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetAgreementDetailsList(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                long totalRecords = 0;
                int proposalCode = 0;
                string progressType = string.Empty;
                int contractCode = 0;
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!(string.IsNullOrEmpty(Request.Params["roadCode"])))
                {
                    proposalCode = Convert.ToInt32(Request.Params["roadCode"]);
                }

                if (!(string.IsNullOrEmpty(Request.Params["progressType"])))
                {
                    progressType = Request.Params["progressType"];
                }

                if (!(string.IsNullOrEmpty(Request.Params["contractCode"])))
                {
                    contractCode = Convert.ToInt32(Request.Params["contractCode"]);
                }

                var jsonData = new
                {
                    rows = objBAL.GetAgreementDetailsList(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, proposalCode, progressType, contractCode),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
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

        #endregion

        #region commonFunction
        protected override void Dispose(bool disposing)
        {
            dbContext.Dispose();
            base.Dispose(disposing);
        }
        #endregion commonFunction

        [HttpGet]
        [Audit]
        public ActionResult FileUpload(String hash, String parameter, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            int coreNetworkCode = 0;
            FileUploadViewModel fileUploadViewModel = new FileUploadViewModel();
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptedParameters.Count() > 0)
                {
                    coreNetworkCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);
                    fileUploadViewModel.IMS_PR_ROAD_CODE = coreNetworkCode;
                    fileUploadViewModel.Urlparameter = URLEncrypt.EncryptParameters1(new string[] { "ProposalCode=" + fileUploadViewModel.IMS_PR_ROAD_CODE.ToString().Trim() });
                    return PartialView("FileUpload", fileUploadViewModel);
                }
                else
                {
                    return PartialView("FileUpload", new FileUploadViewModel());
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return PartialView("FileUpload", new FileUploadViewModel());
            }
        }

        [HttpGet]
        [Audit]
        public ActionResult ImageUpload(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                dbContext = new PMGSYEntities();

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                int imsPrRoadCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);

                if (decryptedParameters.Count() > 0)
                {
                    PMGSY.Models.MaintainanceInspection.FileUploadViewModel fileUploadViewModel = new PMGSY.Models.MaintainanceInspection.FileUploadViewModel();
                    fileUploadViewModel.IMS_PR_ROAD_CODE = imsPrRoadCode;
                    fileUploadViewModel.lstHeadItems = new List<SelectListItem>();

                    fileUploadViewModel.lstHeadItems = new SelectList(dbContext.MASTER_EXECUTION_ITEM.Where(m => m.MAST_HEAD_TYPE == "I").Select(m => new { Value = m.MAST_HEAD_CODE, Text = m.MAST_HEAD_DESC }).ToList(), "Value", "Text").ToList();
                    fileUploadViewModel.lstHeadItems.Insert(0, new SelectListItem { Value = "0", Text = "Select Stage" });

                    if (dbContext.MAINTENANCE_FILES.Where(a => a.IMS_PR_ROAD_CODE == fileUploadViewModel.IMS_PR_ROAD_CODE).Any())
                    {
                        fileUploadViewModel.NumberofFiles = dbContext.MAINTENANCE_FILES.Where(a => a.IMS_PR_ROAD_CODE == fileUploadViewModel.IMS_PR_ROAD_CODE && a.MAINTENANCE_FILE_TYPE == 0).Count();
                    }
                    else
                    {
                        fileUploadViewModel.NumberofFiles = 0;
                    }
                    return View("ImageUpload", fileUploadViewModel);
                }
                return View("ImageUpload", new PMGSY.Models.MaintainanceInspection.FileUploadViewModel());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return View("ImageUpload", new PMGSY.Models.MaintainanceInspection.FileUploadViewModel());
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        //list uploaded Images
        [Audit]
        public JsonResult ListFiles(FormCollection formCollection)
        {
            //Added By Abhishek kamble 29-Apr-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Added By Abhishek kamble 29-Apr-2014 start

            int IMS_PR_ROAD_CODE = Convert.ToInt32(Request["IMS_PR_ROAD_CODE"]);
            int totalRecords;
            var jsonData = new
            {
                rows = maintenanceInspectionBAL.GetFilesListBAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, IMS_PR_ROAD_CODE),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                page = Convert.ToInt32(formCollection["page"]),
                records = totalRecords
            };
            return Json(jsonData);
        }

        #region File Upload Operations
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        [Audit]
        public ActionResult Uploads(PMGSY.Models.MaintainanceInspection.FileUploadViewModel fileUploadViewModel)
        {
            //Added By Abhishek kamble 26-Apr-2014 to validate File Type
            CommonFunctions objCommonFunc = new CommonFunctions();
            //Array of File Types to Validate             
            String[] fileTypes = new String[] { "jpeg", String.Empty, String.Empty };
            if (!(objCommonFunc.IsValidImageFile(ConfigurationManager.AppSettings["MAINTENANCE_PRGRESS_FILE_UPLOAD"], Request, fileTypes)))
            {
                fileUploadViewModel.ErrorMessage = "File Type is Not Allowed.";
                //return View("ImageUpload", fileUploadViewModel.ErrorMessage);
                return Json(new { status = false, message = fileUploadViewModel.ErrorMessage });
            }

            var fileData = new List<PMGSY.Models.MaintainanceInspection.FileUploadViewModel>();
            int IMS_PR_ROAD_CODE = 0;
            if (fileUploadViewModel.IMS_PR_ROAD_CODE != 0)
            {
                IMS_PR_ROAD_CODE = fileUploadViewModel.IMS_PR_ROAD_CODE;
            }
            else
            {
                try
                {
                    IMS_PR_ROAD_CODE = Convert.ToInt32(Request["IMS_PR_ROAD_CODE"]);
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                    if (Request["IMS_PR_ROAD_CODE"].Contains(','))
                    {
                        IMS_PR_ROAD_CODE = Convert.ToInt32(Request["IMS_PR_ROAD_CODE"].Split(',')[0]);
                    }
                }
            }

            string errorMessage = string.Empty;
            string latitude = string.Empty;
            string longitude = string.Empty;
            HttpPostedFileBase file1 = Request.Files[0];
            foreach (string file in Request.Files)
            {

                UploadImageFile(Request, fileData, IMS_PR_ROAD_CODE);

                //bool check = ReadGeoPositions(file1, out errorMessage, out latitude, out longitude);
                //if (check)
                //{
                //    decimal lati = Convert.ToDecimal(latitude);
                //    decimal longi = Convert.ToDecimal(longitude);
                //    HttpPostedFileBase file2 = Request.Files[0];

                //}
                //else
                //{
                //    fileUploadViewModel.ErrorMessage = errorMessage;
                //    return View("ImageUpload", fileUploadViewModel.ErrorMessage);
                //}
            }

            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;

            var result = new ContentResult
            {
                Content = "{\"files\":" + serializer.Serialize(fileData) + "}",
            };
            return result;
        }

        [Audit]
        public void UploadImageFile(HttpRequestBase request, List<PMGSY.Models.MaintainanceInspection.FileUploadViewModel> statuses, int IMS_PR_ROAD_CODE)
        {
            try
            {
                dbContext = new PMGSYEntities();

                HttpRequestBase newRequest = request;
                String StorageRoot = ConfigurationManager.AppSettings["MAINTENANCE_PRGRESS_FILE_UPLOAD"];
                int MaxCount = 0;
                //decimal latitude = 0;decimal longitude= 0;
                string errorMessage = string.Empty;
                string lati = string.Empty;
                string longi = string.Empty;
                for (int i = 0; i < request.Files.Count; i++)
                {
                    HttpPostedFileBase file = request.Files[i];

                    //bool check = ReadGeoPositions(file, out errorMessage, out lati, out longi);

                    //if (check)
                    //{

                    //}

                    #region LAT_LONG_CALCULATION

                    Double[] latitudeRef = null;
                    Double[] longitudeRef = null;
                    StringBuilder strLat = new StringBuilder();
                    StringBuilder strLong = new StringBuilder();

                    using (ExifReader reader = new ExifReader(file.InputStream, true))
                    {
                        reader.GetTagValue<Double[]>(ExifTags.GPSLatitude, out latitudeRef);
                        reader.GetTagValue<Double[]>(ExifTags.GPSLongitude, out longitudeRef);

                        if (latitudeRef != null && longitudeRef != null)
                        {
                            //return Json(new { success = true, latitude = latitude, longitude = longitude });

                            for (int value = 0; value < latitudeRef.Count(); value++)
                            {
                                if (value == 0)
                                {
                                    strLat.Append(latitudeRef[value].ToString() + ".");
                                }
                                else
                                {
                                    if (latitudeRef[value].ToString().Contains("."))
                                    {
                                        strLat.Append(latitudeRef[value].ToString().Replace(".", string.Empty).ToString());
                                    }
                                    else
                                    {
                                        strLat.Append(latitudeRef[value].ToString());
                                    }

                                }
                            }


                            for (int value = 0; value < longitudeRef.Count(); value++)
                            {
                                if (value == 0)
                                {
                                    strLong.Append(longitudeRef[value].ToString() + ".");
                                }
                                else
                                {
                                    if (longitudeRef[value].ToString().Contains("."))
                                    {
                                        strLong.Append(longitudeRef[value].ToString().Replace(".", string.Empty).ToString());
                                    }
                                    else
                                    {
                                        strLong.Append(longitudeRef[value].ToString());
                                    }
                                }
                            }
                        }
                    }

                    #endregion



                    int contentLength = file.ContentLength;
                    var fileId = IMS_PR_ROAD_CODE;
                    if (dbContext.MAINTENANCE_FILES.Where(a => a.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Any())
                    {
                        //new change done on 26-08-2013
                        //MaxCount = dbContext.MAINTENANCE_FILES.Where(a => a.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Count();
                        MaxCount = dbContext.MAINTENANCE_FILES.Where(m => m.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).OrderByDescending(m => m.MAINTENANCE_FILE_ID).Select(m => m.MAINTENANCE_FILE_ID).FirstOrDefault();
                    }
                    MaxCount++;

                    var fileName = IMS_PR_ROAD_CODE + "-" + MaxCount + Path.GetExtension(request.Files[i].FileName).ToString();
                    var fullPath = Path.Combine(StorageRoot, fileName);

                    var ThumbnailPath = Path.Combine(StorageRoot, "thumbnails");
                    var FullThumbnailPath = Path.Combine(ThumbnailPath, fileName);

                    statuses.Add(new PMGSY.Models.MaintainanceInspection.FileUploadViewModel()
                    {
                        url = fullPath,
                        thumbnail_url = fullPath,
                        name = fileName,
                        type = file.ContentType,
                        size = file.ContentLength,
                        delete_url = "",
                        delete_type = "DELETE",
                        file_type = 0,
                        Image_Description = request.Params["remark[]"],
                        IMS_PR_ROAD_CODE = IMS_PR_ROAD_CODE,
                        status = "M",
                        HeadItem = Convert.ToInt32(request.Params["stage[]"]),
                        //Latitude = latitude,
                        //Longitude = longitude
                        //Latitude = Convert.ToDecimal(strLat.ToString()),
                        //Longitude = Convert.ToDecimal(strLong.ToString())
                    });
                    string status = maintenanceInspectionBAL.AddFileUploadDetailsBAL(statuses);
                    if (status == string.Empty)
                    {
                        maintenanceInspectionBAL.CompressImage(newRequest.Files[0], fullPath, FullThumbnailPath);
                        //file.SaveAs(fullPath);
                    }
                    else
                    {
                        // show an error over here
                    }
                }
                //return Json(new { status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                throw ex;
                //return Json(new { status=false},JsonRequestBehavior.AllowGet);
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        [HttpGet]
        [Audit]
        public ActionResult DownloadFile(String parameter, String hash, String key)
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

            if (FileExtension == ".jpg" || FileExtension == ".jpeg" || FileExtension == ".bmp" || FileExtension == ".png" || FileExtension == ".gif" || FileExtension == ".tiff" || FileExtension == ".wmv")
            {
                FullFileLogicalPath = Path.Combine(ConfigurationManager.AppSettings["MAINTENANCE_PRGRESS_FILE_UPLOAD_VIRTUAL_DIR_PATH"], FileName);
                FullfilePhysicalPath = Path.Combine(ConfigurationManager.AppSettings["MAINTENANCE_PRGRESS_FILE_UPLOAD"], FileName);
            }

            string name = Path.GetFileName(FileName);
            string ext = Path.GetExtension(FileName);

            string type = string.Empty;

            if (ext != null)
            {
                switch (ext.ToLower())
                {
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
                return Json(new { Success = "false" });
            }
        }

        [HttpPost]
        [Audit]
        public JsonResult UpdateImageDetails(FormCollection formCollection)
        {

            string[] arrKey = formCollection["id"].Split('$');
            PMGSY.Models.MaintainanceInspection.FileUploadViewModel fileuploadViewModel = new PMGSY.Models.MaintainanceInspection.FileUploadViewModel();
            fileuploadViewModel.IMS_PR_ROAD_CODE = Convert.ToInt32(arrKey[1]);
            fileuploadViewModel.MAINTENANCE_FILE_ID = Convert.ToInt32(arrKey[0]);

            Regex regex = new Regex(@"^[a-zA-Z0-9]+$");
            if (regex.IsMatch(formCollection["Description"]))
            {
                fileuploadViewModel.Image_Description = formCollection["Description"];
            }
            else
            {
                return Json("Invalid Image Description, Only Alphabets and Numbers are allowed");
            }

            string status = maintenanceInspectionBAL.UpdateImageDetailsBAL(fileuploadViewModel);

            if (status == string.Empty)
                return Json(true);
            else
                return Json("There is an error occured while processing your request.");
        }

        [HttpPost]
        [Audit]
        public JsonResult DeleteFileDetails(string id)
        {

            String PhysicalPath = string.Empty;
            String ThumbnailPath = string.Empty;
            string MAINTENANCE_FILE_NAME = Request.Params["IMS_FILE_NAME"];
            PhysicalPath = ConfigurationManager.AppSettings["MAINTENANCE_PRGRESS_FILE_UPLOAD"];
            ThumbnailPath = Path.Combine(Path.Combine(PhysicalPath, "thumbnails"), MAINTENANCE_FILE_NAME);

            string[] arrParam = Request.Params["IMS_PR_ROAD_CODE"].Split('$');

            int MAINTENANCE_FILE_ID = Convert.ToInt32(arrParam[0]);
            int IMS_PR_ROAD_CODE = Convert.ToInt32(arrParam[1]);

            PhysicalPath = Path.Combine(PhysicalPath, MAINTENANCE_FILE_NAME);

            if (!System.IO.File.Exists(PhysicalPath) && !System.IO.File.Exists(ThumbnailPath))
            {
                //return Json(new { Success = false, ErrorMessage = "file Not Found." });
            }

            string status = maintenanceInspectionBAL.DeleteFileDetails(MAINTENANCE_FILE_ID, IMS_PR_ROAD_CODE, MAINTENANCE_FILE_NAME);

            if (status == string.Empty)
            {
                try
                {
                    System.IO.File.Delete(PhysicalPath);
                    System.IO.File.Delete(ThumbnailPath);
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                    return Json(new { Success = true, ErrorMessage = ex.Message });
                }
                return Json(new { Success = true, ErrorMessage = "There is an error while processing your request." });
            }
            else
            {
                return Json(new { Success = false, ErrorMessage = status });
            }
        }
        #endregion

        #region READ_GEOPOSITIONS_IMAGE

        /// <summary>
        /// reads the image to find the Geo locations (lattitude and longitude)
        /// </summary>
        /// <returns></returns>
        //[HttpPost]
        private bool ReadGeoPositions(HttpPostedFileBase file, out string errorMessage, out string latitude, out string longitude)
        {
            try
            {
                Double[] latitudeRef = null;
                Double[] longitudeRef = null;
                //HttpPostedFileBase file = formCollection["files"];
                using (ExifReader reader = new ExifReader(file.InputStream))
                {
                    reader.GetTagValue<Double[]>(ExifTags.GPSLatitude, out latitudeRef);
                    reader.GetTagValue<Double[]>(ExifTags.GPSLongitude, out longitudeRef);

                    if (latitudeRef != null && longitudeRef != null)
                    {
                        //return Json(new { success = true, latitude = latitude, longitude = longitude });
                        StringBuilder strLat = new StringBuilder();
                        for (int value = 0; value < latitudeRef.Count(); value++)
                        {
                            if (value == 0)
                            {
                                strLat.Append(latitudeRef[value].ToString() + ".");
                            }
                            else
                            {
                                if (latitudeRef[value].ToString().Contains("."))
                                {
                                    strLat.Append(latitudeRef[value].ToString().Replace(".", string.Empty).ToString());
                                }
                                else
                                {
                                    strLat.Append(latitudeRef[value].ToString());
                                }

                            }
                        }

                        StringBuilder strLong = new StringBuilder();
                        for (int value = 0; value < longitudeRef.Count(); value++)
                        {
                            if (value == 0)
                            {
                                strLong.Append(longitudeRef[value].ToString() + ".");
                            }
                            else
                            {
                                if (longitudeRef[value].ToString().Contains("."))
                                {
                                    strLong.Append(longitudeRef[value].ToString().Replace(".", string.Empty).ToString());
                                }
                                else
                                {
                                    strLong.Append(longitudeRef[value].ToString());
                                }
                            }
                        }

                        errorMessage = string.Empty;
                        latitude = strLat.ToString();
                        longitude = strLong.ToString();
                        return true;
                    }
                    else
                    {
                        errorMessage = "Image does not contain the Geo location information.";
                        //return Json(new { success = false, message = "Image does not contain the Geo location information." });
                        latitude = latitudeRef.ToString();
                        longitude = longitudeRef.ToString();
                        return false;
                    }

                }
            }
            catch (Exception)
            {
                errorMessage = "Please select file to upload";
                latitude = string.Empty;
                longitude = string.Empty;
                return false;
                //return Json(new { success = false , message = "Error occurred while processing your request."});
            }
        }


        [HttpPost]
        public ActionResult ReadGeoPositions(FormCollection formCollection)
        {
            try
            {
                Double[] latitudeRef = null;
                Double[] longitudeRef = null;

                Stream stream = Request.InputStream;

                if (stream != null)
                {
                    using (ExifReader reader = new ExifReader(stream))
                    {
                        reader.GetTagValue<Double[]>(ExifTags.GPSLatitude, out latitudeRef);
                        reader.GetTagValue<Double[]>(ExifTags.GPSLongitude, out longitudeRef);

                        if (latitudeRef != null && longitudeRef != null)
                        {
                            StringBuilder strLatitude = new StringBuilder();
                            for (int item = 0; item < latitudeRef.Count(); item++)
                            {
                                if (item == 0)
                                {
                                    strLatitude.Append(latitudeRef[item].ToString() + (char)176 + " ");
                                }
                                if (item == 1)
                                {
                                    strLatitude.Append(latitudeRef[item].ToString() + "' ");
                                }
                                if (item == 2)
                                {
                                    strLatitude.Append(Math.Round(latitudeRef[item], 1).ToString());
                                }
                            }

                            StringBuilder strLongitude = new StringBuilder();
                            for (int item = 0; item < longitudeRef.Count(); item++)
                            {
                                if (item == 0)
                                {
                                    strLongitude.Append(longitudeRef[item].ToString() + (char)176 + " ");
                                }
                                if (item == 1)
                                {
                                    strLongitude.Append(longitudeRef[item].ToString() + "' ");
                                }
                                if (item == 2)
                                {
                                    strLongitude.Append(Math.Round(longitudeRef[item], 1).ToString());
                                }
                                //strLongitude.Append(item.ToString());
                            }

                            return Json(new { success = true, latitude = strLatitude.ToString(), longitude = strLongitude.ToString() });
                        }
                        else
                        {
                            return Json(new { success = false, message = "Image does not contain the Geo location information." });
                        }

                    }
                }
                else
                {
                    return Json(new { success = false, message = "Please select image to upload." });
                }

                //return Json(new { success = true, latitude = latitudeRef.ToString(), longitude = longitudeRef.ToString() });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        #endregion

        #region Photo Uploads


        [HttpGet]
        [Audit]
        public ActionResult FileUploadProgress(String hash, String parameter, String key)
        {
            Dictionary<string, string> decryptedParameters = null;
            int coreNetworkCode = 0;
            FileUploadViewModelProgress fileUploadViewModel = new FileUploadViewModelProgress();
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptedParameters.Count() > 0)
                {
                    coreNetworkCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);
                    fileUploadViewModel.IMS_PR_ROAD_CODE = coreNetworkCode;
                    fileUploadViewModel.Urlparameter = URLEncrypt.EncryptParameters1(new string[] { "ProposalCode=" + fileUploadViewModel.IMS_PR_ROAD_CODE.ToString().Trim() });
                    return PartialView("FileUploadProgress", fileUploadViewModel);
                }
                else
                {
                    return PartialView("FileUploadProgress", new FileUploadViewModel());
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return PartialView("FileUploadProgress", new FileUploadViewModel());
            }
        }

        // 1
        [HttpGet]
        [Audit]
        public ActionResult ImageUploadProgress(String parameter, String hash, String key)
        {
            dbContext = new PMGSYEntities();
            PMGSY.Models.MaintainanceInspection.FileUploadViewModelProgress fileUploadViewModel = new PMGSY.Models.MaintainanceInspection.FileUploadViewModelProgress();
            try
            {
                //String[] urlSplitParams = id.Split('$');

                //  Int32 PRRoadCode = Convert.ToInt32(urlSplitParams[0]);

                Dictionary<string, string> decryptedParameters = null;
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                int PRRoadCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);

                fileUploadViewModel.WorkName = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == PRRoadCode).Select(m => m.IMS_ROAD_NAME).FirstOrDefault();
                fileUploadViewModel.PavLength = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == PRRoadCode).Select(m => m.IMS_PAV_LENGTH).FirstOrDefault();

                int ScheduleCode = 0;
                int ObsId = 0;

                fileUploadViewModel.ADMIN_SCHEDULE_CODE = ScheduleCode;
                fileUploadViewModel.IMS_PR_ROAD_CODE = PRRoadCode;
                fileUploadViewModel.QM_OBSERVATION_ID = ObsId;

                if (ObsId > 0)
                {
                    if (dbContext.QUALITY_QM_INSPECTION_FILE.Where(a => a.QM_OBSERVATION_ID == ObsId).Any())
                    {
                        fileUploadViewModel.NumberofFiles = dbContext.QUALITY_QM_INSPECTION_FILE.Where(a => a.QM_OBSERVATION_ID == ObsId).Count();
                    }
                    else
                    {
                        fileUploadViewModel.NumberofFiles = 0;
                    }
                }
                else
                {
                    if (dbContext.QUALITY_QM_INSPECTION_FILE.Where(a => a.ADMIN_SCHEDULE_CODE == fileUploadViewModel.ADMIN_SCHEDULE_CODE && a.IMS_PR_ROAD_CODE == fileUploadViewModel.IMS_PR_ROAD_CODE).Any())
                    {
                        fileUploadViewModel.NumberofFiles = dbContext.QUALITY_QM_INSPECTION_FILE.Where(a => a.ADMIN_SCHEDULE_CODE == fileUploadViewModel.ADMIN_SCHEDULE_CODE && a.IMS_PR_ROAD_CODE == fileUploadViewModel.IMS_PR_ROAD_CODE).Count();
                    }
                    else
                    {
                        fileUploadViewModel.NumberofFiles = 0;
                    }
                }


                return View("ImageUploadProgress", fileUploadViewModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MaintainanceInspection().ImageUploadProgress()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        [Audit]
        public ActionResult UploadsProgress(PMGSY.Models.MaintainanceInspection.FileUploadViewModelProgress fileUploadViewModel)
        {

            try
            {
                CommonFunctions objCommonFunc = new CommonFunctions();
                //Array of File Types to Validate             
                String[] fileTypes = new String[] { "jpeg", String.Empty, String.Empty };
                String StorageRoot = string.Empty;
                StorageRoot = System.Configuration.ConfigurationManager.AppSettings["MANE_PROGRESS_FILE_UPLOAD"];

                decimal? startChainage = fileUploadViewModel.Startchainage;
                decimal? endChainage = fileUploadViewModel.Endchainage;


                if (!(objCommonFunc.IsValidImageFile(StorageRoot, Request, fileTypes)))
                {
                    fileUploadViewModel.ErrorMessage = "File Type is Not Allowed.";
                    return View("ImageUploadProgress", fileUploadViewModel.ErrorMessage);
                }


                foreach (string file in Request.Files)
                {
                    string status = ValidateImageFileofProgress(Request.Files[0].ContentLength, System.IO.Path.GetExtension(Request.Files[0].FileName));
                    if (status != string.Empty)
                    {
                        fileUploadViewModel.ErrorMessage = status;
                        return View("ImageUploadProgress", fileUploadViewModel.ErrorMessage);
                    }
                }

                var fileData = new List<PMGSY.Models.MaintainanceInspection.FileUploadViewModelProgress>();

                //
                int IMS_PR_ROAD_CODE = 0;
                int ADMIN_SCHEDULE_CODE = 0;
                int QM_OBSERVATION_ID = 0;
                if (fileUploadViewModel.IMS_PR_ROAD_CODE != 0 && fileUploadViewModel.ADMIN_SCHEDULE_CODE != 0 && fileUploadViewModel.QM_OBSERVATION_ID != 0)
                {
                    ADMIN_SCHEDULE_CODE = fileUploadViewModel.ADMIN_SCHEDULE_CODE;
                    IMS_PR_ROAD_CODE = fileUploadViewModel.IMS_PR_ROAD_CODE;
                    QM_OBSERVATION_ID = fileUploadViewModel.QM_OBSERVATION_ID;
                }
                else
                {
                    try
                    {
                        //  ADMIN_SCHEDULE_CODE = Convert.ToInt32(Request["ADMIN_SCHEDULE_CODE"]);
                        IMS_PR_ROAD_CODE = fileUploadViewModel.IMS_PR_ROAD_CODE; //Convert.ToInt32(Request["IMS_PR_ROAD_CODE"]);
                        //   QM_OBSERVATION_ID = Convert.ToInt32(Request["QM_OBSERVATION_ID"]);
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.LogError(ex, "MaintainanceInspection().UploadsProgress()");

                        if (Request["IMS_PR_ROAD_CODE"].Contains(','))
                        {
                            IMS_PR_ROAD_CODE = Convert.ToInt32(Request["IMS_PR_ROAD_CODE"].Split(',')[0]);
                        }
                        if (Request["ADMIN_SCHEDULE_CODE"].Contains(','))
                        {
                            IMS_PR_ROAD_CODE = Convert.ToInt32(Request["ADMIN_SCHEDULE_CODE"].Split(',')[0]);
                        }
                    }
                }//

                foreach (string file in Request.Files)
                {
                    UploadImageFileProgress(Request, fileData, IMS_PR_ROAD_CODE, startChainage, endChainage, fileUploadViewModel);
                }
                if (!string.IsNullOrEmpty(fileUploadViewModel.ErrorMessage))
                {
                    // fileUploadViewModel.ErrorMessage = status;
                    //ModelState.AddModelError("IMS_PR_ROAD_CODE", fileUploadViewModel.ErrorMessage);
                    //return View("ImageUploadProgress", fileUploadViewModel.ErrorMessage);
                    return Json(new { success = "false", message = fileUploadViewModel.ErrorMessage });
                }

                var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;

                var result = new ContentResult
                {
                    Content = "{\"files\":" + serializer.Serialize(fileData) + "}",
                };
                return result;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MaintainanceInspection().Uploads()");
                return null;
            }
        }

        [Audit]
        public void UploadImageFileProgress(HttpRequestBase request, List<PMGSY.Models.MaintainanceInspection.FileUploadViewModelProgress> statuses, int IMS_PR_ROAD_CODE, decimal? startChainage, decimal? endChainage, PMGSY.Models.MaintainanceInspection.FileUploadViewModelProgress fileUploadViewModel)
        {
            dbContext = new PMGSYEntities();
            MaintainanceInspectionBAL maintenanceAgreementBAL = new MaintainanceInspectionBAL();
            String StorageRoot = string.Empty;
            int MaxFileID = 0;
            try
            {
                for (int i = 0; i < request.Files.Count; i++)
                {
                    HttpPostedFileBase file = request.Files[i];

                    if (dbContext.MANE_IMS_PROGRESS_FILES.Count() == 0)
                    {
                        MaxFileID = 0;
                    }
                    else
                    {
                        MaxFileID = (from c in dbContext.MANE_IMS_PROGRESS_FILES select (Int32?)c.MANE_IMS_FILE_ID ?? 0).Max();
                    }

                    Int32 countOfFilesUploaded = 0;
                    if (IMS_PR_ROAD_CODE > 0)
                    {
                        countOfFilesUploaded = (from c in dbContext.MANE_IMS_PROGRESS_FILES
                                                where c.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE
                                                select c).Count();
                    }
                    else
                    {
                        countOfFilesUploaded = (from c in dbContext.MANE_IMS_PROGRESS_FILES
                                                where c.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE
                                                select c).Count();
                    }


                    if (countOfFilesUploaded == 0)
                    {
                        countOfFilesUploaded = 1;
                    }
                    else
                    {
                        countOfFilesUploaded = countOfFilesUploaded + 1;
                    }

                    var fileId = MaxFileID + 1;


                    //var fileNameWithoutExt = Path.GetFileNameWithoutExtension(request.Files[i].FileName).ToString();
                    var fileName = fileId + "_" + IMS_PR_ROAD_CODE + "_" + countOfFilesUploaded + System.IO.Path.GetExtension(request.Files[i].FileName).ToString();

                    ///Path to upload files for NQM/SQM/CQC/SQC

                    StorageRoot = System.Configuration.ConfigurationManager.AppSettings["MANE_PROGRESS_FILE_UPLOAD"];


                    var fullPath = System.IO.Path.Combine(StorageRoot, fileName);

                    var ThumbnailPath = System.IO.Path.Combine(StorageRoot, "thumbnails");
                    var FullThumbnailPath = System.IO.Path.Combine(ThumbnailPath, fileName);

                    if (!(System.IO.Directory.Exists(ThumbnailPath)))
                    {
                        System.IO.Directory.CreateDirectory(ThumbnailPath);
                    }

                    statuses.Add(new PMGSY.Models.MaintainanceInspection.FileUploadViewModelProgress()
                    {
                        url = fullPath,
                        thumbnail_url = fullPath,
                        name = fileName,
                        type = file.ContentType,
                        size = file.ContentLength,
                        delete_url = "",
                        delete_type = "DELETE",
                        Image_Description = request.Params["remark[]"],  //.Split(',')[i]
                        IMS_PR_ROAD_CODE = IMS_PR_ROAD_CODE,
                        Startchainage = Convert.ToDecimal(request.Params["chainageValue1[]"]),//startChainage,
                        Endchainage = Convert.ToDecimal(request.Params["chainageValue2[]"])//endChainage

                    });

                    string status = maintenanceAgreementBAL.AddFileUploadDetailsBAL(statuses);

                    if (status == string.Empty)
                    {
                        new PMGSY.BAL.Proposal.ProposalBAL().CompressImage(request.Files[0], fullPath, FullThumbnailPath);
                    }
                    else
                    {

                        fileUploadViewModel.ErrorMessage = status;
                        // return View("ImageUploadProgress", fileUploadViewModel.ErrorMessage);
                        //   return View("ImageUploadProgress", fileUploadViewModel.ErrorMessage);
                        // show an error over here
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MaintainanceInspection().UploadImageFile()");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        //2 
        [Audit]
        public JsonResult ListFilesProgress(FormCollection formCollection)
        {
            dbContext = new PMGSYEntities();
            MaintainanceInspectionBAL maintenanceAgreementBAL = new MaintainanceInspectionBAL();
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


                Int32 IMS_PR_ROAD_CODE = Convert.ToInt32(Request.Params["IMS_PR_ROAD_CODE"]);



                int totalRecords;
                var jsonData = new
                {
                    rows = maintenanceAgreementBAL.GetFilesListBALProgress(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, IMS_PR_ROAD_CODE),

                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MaintainanceInspection().ListFiles()");
                return Json(string.Empty);
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        [HttpGet]
        public ActionResult DownloadFileUploadedByPIUForProgress(String parameter, String hash, String key)
        {
            string FileName = string.Empty;
            string FullFileLogicalPathName = string.Empty;
            string FullfilePhysicalPathName = string.Empty;
            string FullFileLogicalPathId = string.Empty;
            string FullfilePhysicalPathId = string.Empty;
            string FileExtension = string.Empty;
            Int32 FileID = 0;
            dbContext = new PMGSYEntities();
            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        FileName = (urlSplitParams[0]);
                        FileID = Convert.ToInt32(urlSplitParams[1]);
                    }
                }
                FileExtension = System.IO.Path.GetExtension(FileName).ToLower();

                if (FileExtension == ".jpg" || FileExtension == ".jpeg" || FileExtension == ".bmp" || FileExtension == ".png" || FileExtension == ".gif" || FileExtension == ".tiff")
                {
                    FullFileLogicalPathName = System.IO.Path.Combine(System.Configuration.ConfigurationManager.AppSettings["MANE_PROGRESS_FILE_UPLOAD_VIRTUAL_DIR_PATH"], FileName);
                    FullfilePhysicalPathName = System.IO.Path.Combine(System.Configuration.ConfigurationManager.AppSettings["MANE_PROGRESS_FILE_UPLOAD"], FileName);

                }

                string name = System.IO.Path.GetFileName(FileName);
                string ext = System.IO.Path.GetExtension(FileName);

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

                if (System.IO.File.Exists(FullfilePhysicalPathName))
                {
                    return File(FullfilePhysicalPathName, type, DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + FileExtension);
                }
                else if (System.IO.File.Exists(FullfilePhysicalPathId))
                {
                    return File(FullfilePhysicalPathId, type, DateTime.Now.ToShortDateString() + "_" + new Random().Next(1000000000) + FileExtension);
                }
                else
                {
                    return Json(new { Error = "File Not Exist" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MaintenaneInspectionController.DownloadFileUploadedByPIUForProgress()");
                return Json(new { Success = "false" }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }


        [HttpPost]
        public JsonResult DeleteProgressByPIU(String parameter, String hash, String key)
        {
            String PhysicalPath = string.Empty;
            String ThumbnailPath = string.Empty;
            dbContext = new PMGSYEntities();
            MaintainanceInspectionBAL maintenanceAgreementBAL = new MaintainanceInspectionBAL();

            try
            {
                String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });

                String[] urlSplitParams = urlParams[0].Split('$');
                string QM_FILE_NAME = (urlSplitParams[0]);
                int FILE_ID = Convert.ToInt32(urlSplitParams[1]);

                PhysicalPath = System.Configuration.ConfigurationManager.AppSettings["MANE_PROGRESS_FILE_UPLOAD"];
                ThumbnailPath = System.IO.Path.Combine(System.IO.Path.Combine(PhysicalPath, "thumbnails"), QM_FILE_NAME);
                PhysicalPath = System.IO.Path.Combine(PhysicalPath, QM_FILE_NAME);
                if (!System.IO.File.Exists(PhysicalPath) && !System.IO.File.Exists(ThumbnailPath))
                {
                    return Json(new { Success = false, ErrorMessage = "File Not Found." });
                }
                string status = maintenanceAgreementBAL.DeleteFileDetailsProgress(FILE_ID);
                if (status == string.Empty)
                {
                    try
                    {
                        System.IO.File.Delete(PhysicalPath);
                        System.IO.File.Delete(ThumbnailPath);
                    }
                    catch (Exception ex)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                        return Json(new { Success = true, ErrorMessage = ex.Message });
                    }
                    return Json(new { Success = true, ErrorMessage = "Error Ocurred While Processing Your Request." });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = status });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MaintenaneInspectionController.DeleteProgressByPIU()");
                return Json(new { Success = false, ErrorMessage = "Error Ocurred While Processing Your Request." });
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }


        public string ValidateImageFileofProgress(int FileSize, string FileExtension)
        {
            try
            {
                string ValidExtensions = System.Configuration.ConfigurationManager.AppSettings["MANE_PROGRESS_VALID_FORMAT"];
                string[] arrValidFormats = ValidExtensions.Split('$');


                if (!arrValidFormats.Contains(FileExtension.ToLower()))
                {
                    return "File is not Valid Image File";
                }
                if (FileSize > Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["MANE_PROGRESS_FILE_MAX_SIZE"]))
                {
                    return "File Size Exceed the Maximum File Limit";
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MaintainanceInspection().ValidateImageFileofProgress()");
                return null;
            }
        }


        #endregion
    }
}
