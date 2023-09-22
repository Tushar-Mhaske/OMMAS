/*----------------------------------------------------------------------------------------

 * Project Id:

 * Project Name:OMMAS2

 * File Name: MaintenanceAgreementController.cs

 * Author : Koustubh Nakate

 * Creation Date :21/June/2013

 * Desc : This controller is used as get the request and send response as view, list for maintenance agreement screens.  
 * ---------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.BAL.MaintenanceAgreement;
using PMGSY.DAL.MaintenanceAgreement;
using PMGSY.Common;
using PMGSY.Extensions;
using PMGSY.Models;
using PMGSY.Models.Common;
using PMGSY.Models.MaintenanceAgreement;
using PMGSY.Models.Agreement;
using PMGSY.Models.Proposal;
using PMGSY.Areas.AccountReports.Models;
using PMGSY.WebServices.eMarg.Model;

namespace PMGSY.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class MaintenanceAgreementController : Controller
    {
        private PMGSYEntities dbContext = new PMGSYEntities();
        Dictionary<string, string> decryptedParameters = null;
        String[] encryptedParameters = null;

        IMaintenanceAgreementBAL maintenanceAgreementBAL = new MaintenanceAgreementBAL();
        MaintenanceAgreementDAL maintenanceAgreementDAL = new MaintenanceAgreementDAL();
        CommonFunctions commonFunction = new CommonFunctions();

        string message = string.Empty;


        #region MAINTENANCE_AGREEMENT


        [Audit]
        public ActionResult ProposalMaintenanceAgreement()
        {
            DAL.MasterDataEntryDAL masterDataEntryDAL = new DAL.MasterDataEntryDAL();
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
                    ViewData["BlockList"] = new SelectList(masterDataEntryDAL.GetAllBlocksByDistrictCode(transactionParams.DISTRICT_CODE, true), "MAST_BLOCK_CODE", "MAST_BLOCK_NAME");
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
                ViewData["BlockList"] = null;
            }

            return View("ProposalMaintenanceAgreement");

        }

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
                else
                {
                    return null;
                }
                if (!string.IsNullOrEmpty(Request.Params["blockCode"]))
                {
                    blockCode = Convert.ToInt32(Request.Params["blockCode"]);
                }
                else
                {
                    return null;
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
                    rows = maintenanceAgreementBAL.GetCompletedRoadListBAL(stateCode, districtCode, blockCode, sanctionedYear, packageID, adminNDCode, batch, collaboration, upgradationType, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : (totalRecords % Convert.ToInt32(rows) == 0 ? totalRecords / Convert.ToInt32(rows) : totalRecords / Convert.ToInt32(rows) + 1),//totalRecords / Convert.ToInt32(rows) + 1,
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
        public ActionResult MaintenanceAgreementDetails(String parameter, String hash, String key)
        {
            string roadLsb = string.Empty;
            int IMSPRRoadCode = 0;
            DateTime? constructionComDate = null;
            MaintenanceAgreementDetails maintenenaceAgreementDetails = new MaintenanceAgreementDetails();
            maintenenaceAgreementDetails.IsNewContractor = false;
            maintenenaceAgreementDetails.EncryptedIMSPRRoadCode = parameter + "/" + hash + "/" + key;

            decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

            IMSPRRoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"].ToString());

            roadLsb = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(x => x.IMS_PROPOSAL_TYPE).FirstOrDefault();
            if (roadLsb == "P")
            {
                EXEC_ROADS_MONTHLY_STATUS executionMaster = dbContext.EXEC_ROADS_MONTHLY_STATUS.Where(EXE => EXE.IMS_PR_ROAD_CODE == IMSPRRoadCode && EXE.EXEC_ISCOMPLETED == "C").FirstOrDefault();
                constructionComDate = executionMaster.EXEC_COMPLETION_DATE;
                maintenenaceAgreementDetails.CompletionMonth = executionMaster.EXEC_PROG_MONTH;
                maintenenaceAgreementDetails.CompletionYear = executionMaster.EXEC_PROG_YEAR;
                maintenenaceAgreementDetails.MANE_CONSTR_COMP_DATE = constructionComDate == null ? null : Convert.ToDateTime(constructionComDate).ToString("dd/MM/yyyy");
            }
            else
            {
                EXEC_LSB_MONTHLY_STATUS executionMaster = dbContext.EXEC_LSB_MONTHLY_STATUS.Where(EXE => EXE.IMS_PR_ROAD_CODE == IMSPRRoadCode && EXE.EXEC_ISCOMPLETED == "C").FirstOrDefault();
                constructionComDate = executionMaster.EXEC_COMPLETION_DATE;
                maintenenaceAgreementDetails.CompletionMonth = executionMaster.EXEC_PROG_MONTH;
                maintenenaceAgreementDetails.CompletionYear = executionMaster.EXEC_PROG_YEAR;
                maintenenaceAgreementDetails.MANE_CONSTR_COMP_DATE = constructionComDate == null ? null : Convert.ToDateTime(constructionComDate).ToString("dd/MM/yyyy");
            }

            ViewBag.IsNewEntry = CheckAlreadyAgreementDone(IMSPRRoadCode);

            return PartialView("MaintenanceAgreementDetails", maintenenaceAgreementDetails);
        }

        [Audit]
        private bool CheckAlreadyAgreementDone(int IMSPRRoadCode)
        {
            try
            {
                int? splitCount = dbContext.IMS_PROPOSAL_SPLIT.Where(ps => ps.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(ps => ps.IMS_TOTAL_SPLIT).FirstOrDefault();

                if (splitCount == null || splitCount == 0)
                {
                    if (dbContext.MANE_IMS_CONTRACT.Any(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode && IMS.MANE_AGREEMENT_TYPE == "R"))
                    {
                        return false;
                    }
                }
                else
                {
                    if (splitCount == dbContext.MANE_IMS_CONTRACT.Where(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(IMS => IMS.IMS_WORK_CODE).Distinct().Count())
                    {
                        return false;
                    }

                }

                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return true;
            }
        }
        [Audit]
        public ActionResult AddMaintenanceAgreementAgainstRoad(String parameter, String hash, String key)
        {
            MaintenanceAgreementDetails maintenenaceAgreementDetails = new MaintenanceAgreementDetails();
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    dbContext = new PMGSYEntities();

                    maintenenaceAgreementDetails.EncryptedIMSPRRoadCode = parameter + '/' + hash + '/' + key;
                    maintenenaceAgreementDetails.IsNewContractor = false;

                    ViewBag.SanctionedYear = decryptedParameters["SanctionedYear"].ToString();
                    //ViewBag.RoadName = decryptedParameters["IMSRoadName"].ToString();
                    ViewBag.Package = decryptedParameters["Package"].ToString().Replace("--", "/");
                    ViewBag.RoadLength = decryptedParameters["RoadLength"].ToString().Replace("--", ".");
                    string imsPackage = decryptedParameters["Package"].ToString().Replace("--", "/");
                    int prRoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"]);
                    ViewBag.RoadName = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PACKAGE_ID == imsPackage && x.IMS_PR_ROAD_CODE == prRoadCode).Select(s => s.IMS_ROAD_NAME).FirstOrDefault();

                    return PartialView("AddMaintenanceAgreementAgainstRoad", maintenenaceAgreementDetails);
                }
                return PartialView("AddMaintenanceAgreementAgainstRoad", maintenenaceAgreementDetails);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return PartialView("AddMaintenanceAgreementAgainstRoad", maintenenaceAgreementDetails);
            }
        }


      

        [HttpPost]
        [Audit]
        public ActionResult GetAgreementDetailsList_Proposal(int? page, int? rows, string sidx, string sord)
        {

            long totalRecords;
            int IMSPRRoadCode = 0;


            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                //if (!string.IsNullOrEmpty(Request.Params["ProposalCode"]))
                //{
                //    IMSPRRoadCode = Convert.ToInt32(Request.Params["ProposalCode"]);
                //}

                if (!string.IsNullOrEmpty(Request.Params["IMSPRRoadCode"]))
                {
                    encryptedParameters = Request.Params["IMSPRRoadCode"].Split('/');
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                    IMSPRRoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"]);
                }

                var jsonData = new
                {
                    rows = maintenanceAgreementBAL.GetAgreementDetailsListBAL_Proposal(IMSPRRoadCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : (totalRecords % Convert.ToInt32(rows) == 0 ? totalRecords / Convert.ToInt32(rows) : totalRecords / Convert.ToInt32(rows) + 1),
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


        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult AddAgreementDetails_Proposal(MaintenanceAgreementDetails details_agreement)
        {
            bool status = false;
            try
            {

                if (details_agreement.ProposalWorks.Count() == 1)
                {
                    ModelState.Remove("IMS_WORK_CODE");
                }

                if (ModelState.IsValid)
                {

                    if (maintenanceAgreementBAL.SaveAgreementDetailsBAL_Proposal(details_agreement, ref message))
                    {
                        message = message == string.Empty ? "Agreement details added successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Agreement details not added successfully." : message;
                    }

                }
                else
                {
                    return PartialView("MaintenanceAgreementDetails", details_agreement);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Agreement details not added successfully.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        [Audit]
        public ActionResult GetExistingAgreementDetails()
        {

            int IMSPRRoadCode = 0;
            int IMSWorkCode = 0;
            string agreementDetails = string.Empty;
            string agreementNumber = string.Empty;
            string agreementDate = string.Empty;
            int contractorID = 0;
            decimal? year1 = 0;
            decimal? year2 = 0;
            decimal? year3 = 0;
            decimal? year4 = 0;
            decimal? year5 = 0;
            //List<string> contractorList = new List<string>(); 
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["IMSPRRoadCode"]) && !string.IsNullOrEmpty(Request.Params["IMSWorkCode"]))
                {
                    encryptedParameters = Request.Params["IMSPRRoadCode"].Split('/');

                    if (encryptedParameters.Length == 3)
                    {
                        decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                        IMSPRRoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"].ToString());
                    }
                    IMSWorkCode = Convert.ToInt32(Request.Params["IMSWorkCode"].ToString());
                }
                else
                {
                    return Json(new { success = false, contractorID = 0, agreementNumber = string.Empty, agreementDate = string.Empty, year1 = string.Empty, year2 = string.Empty, year3 = string.Empty, year4 = string.Empty, year5 = string.Empty, message = message }, JsonRequestBehavior.AllowGet);
                }

                if (maintenanceAgreementBAL.GetExistingAgreementDetailsBAL(IMSPRRoadCode, IMSWorkCode, ref contractorID, ref agreementNumber, ref agreementDate, ref year1, ref year2, ref year3, ref year4, ref year5, ref message))
                {
                    return Json(new { success = true, contractorID = contractorID, agreementNumber = agreementNumber, agreementDate = agreementDate, year1 = year1, year2 = year2, year3 = year3, year4 = year4, year5 = year5, message = message }, JsonRequestBehavior.AllowGet);
                }
                else
                {

                    return Json(new { success = false, contractorID = 0, agreementNumber = string.Empty, agreementDate = string.Empty, year1 = string.Empty, year2 = string.Empty, year3 = string.Empty, year4 = string.Empty, year5 = string.Empty, message = message }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, contractorID = 0, agreementNumber = string.Empty, agreementDate = string.Empty, year1 = string.Empty, year2 = string.Empty, year3 = string.Empty, year4 = string.Empty, year5 = string.Empty, message = message }, JsonRequestBehavior.AllowGet);
            }
        }//end function GetExistingAgreementDetails


        [HttpPost]
        [Audit]
        public JsonResult GetExistingContractorsByIMSPRRoadCode(string PRRoadCode)
        {
            int IMSPRRoadCode = 0;
            try
            {
                encryptedParameters = PRRoadCode.Split('/');

                if (encryptedParameters.Length == 3)
                {
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    IMSPRRoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"].ToString());
                }


                //int stateCode = PMGSY.Extensions.PMGSYSession.Current.StateCode;
                List<MASTER_CONTRACTOR> contractorList = new List<MASTER_CONTRACTOR>();

                //PMGSY.DAL.Agreement.AgreementDAL agreementDAL = new PMGSY.DAL.Agreement.AgreementDAL();

                //contractorList = agreementDAL.GetAllContractor(stateCode, "C", false);

                contractorList = maintenanceAgreementDAL.GetExistingContractor(IMSPRRoadCode);

                return Json(new SelectList(contractorList, "MAST_CON_ID", "MAST_CON_COMPANY_NAME"));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(false);
            }
        }//end function GetExistingContractorsByIMSPRRoadCode


        [HttpPost]
        [Audit]
        public JsonResult GetContractors()
        {
            try
            {

                int stateCode = PMGSY.Extensions.PMGSYSession.Current.StateCode;
                List<MASTER_CONTRACTOR> contractorList = new List<MASTER_CONTRACTOR>();

                PMGSY.DAL.Agreement.AgreementDAL agreementDAL = new PMGSY.DAL.Agreement.AgreementDAL();

                contractorList = agreementDAL.GetAllContractor(stateCode, "C", false);

                return Json(new SelectList(contractorList, "MAST_CON_ID", "MAST_CON_COMPANY_NAME"));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(false);
            }
        }//end function GetContractors


        [Audit]
        public ActionResult EditMaintenancetAgreementDetails_Proposal(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptParameters = null;
            try
            {
                decryptParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptParameters.Count() > 0)
                {
                    MaintenanceAgreementDetails agreementDetails = maintenanceAgreementBAL.GetMaintenanceAgreementDetailsBAL(Convert.ToInt32(decryptParameters["IMSPRRoadCode"].ToString()), Convert.ToInt32(decryptParameters["PRContractCode"].ToString()), Convert.ToInt32(decryptParameters["ManeContractId"].ToString()));

                    if (agreementDetails == null)
                    {
                        ModelState.AddModelError(string.Empty, "Agreement details not exist.");
                        return PartialView("MaintenanceAgreementDetails", new MaintenanceAgreementDetails());
                    }

                    return PartialView("MaintenanceAgreementDetails", agreementDetails);
                }
                return PartialView("MaintenanceAgreementDetails", new MaintenanceAgreementDetails());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(String.Empty, "Agreement details not exist.");
                return PartialView("MaintenanceAgreementDetails", new MaintenanceAgreementDetails());
            }
        }

        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult EditMaintenancetAgreementDetails_Proposal(MaintenanceAgreementDetails details_agreement)
        {
            bool status = false;
            try
            {
                //ModelState.AddModelError(string.Empty, "trst");           
                //return PartialView("AgreementDetails", master_agreement);

                if (details_agreement.ProposalWorks.Count() == 1)
                {
                    ModelState.Remove("IMS_WORK_CODE");
                }

                if (ModelState.IsValid)
                {
                    if (maintenanceAgreementBAL.UpdateMaintenanceAgreementDetailsBAL_Proposal(details_agreement, ref message))
                    {
                        message = message == string.Empty ? "Agreement details updated successfully." : message;
                        status = true;

                    }
                    else
                    {
                        message = message == string.Empty ? "Agreement details not updated successfully." : message;
                    }
                }
                else
                {
                    return PartialView("MaintenanceAgreementDetails", details_agreement);
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Agreement details not updated successfully." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult FinalizeAgreement(String parameter, String hash, String key)
        {
            int IMSPRRoadCode = 0;
            int PRContractCode = 0;
            int ManeContractId = 0;
            bool status = false;
            message = "Agreement not finalized successfully.";
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    IMSPRRoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"]);
                    PRContractCode = Convert.ToInt32(decryptedParameters["PRContractCode"]);
                    ManeContractId = Convert.ToInt32(decryptedParameters["ManeContractId"]);

                    if (maintenanceAgreementBAL.FinalizeAgreementBAL(IMSPRRoadCode, PRContractCode, ManeContractId))
                    {
                        message = "Agreement finalized successfully.";
                        status = true;
                    }
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                //message = message = "Agreement not finalized successfully.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }//end function FinalizeAgreement

        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult DeFinalizeAgreement(String parameter, String hash, String key)
        {
            int IMSPRRoadCode = 0;
            int PRContractCode = 0;
            bool status = false;
            int ManeContractId = 0;
            message = "Agreement not definalized successfully.";
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    IMSPRRoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"]);
                    PRContractCode = Convert.ToInt32(decryptedParameters["PRContractCode"]);
                    ManeContractId = Convert.ToInt32(decryptedParameters["ManeContractId"]);

                    if (maintenanceAgreementBAL.DeFinalizeAgreementBAL(IMSPRRoadCode, PRContractCode, ManeContractId))
                    {
                        message = "Agreement definalized successfully.";
                        status = true;
                    }
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                //message = message = "Agreement not finalized successfully.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }//end function FinalizeAgreement

        [Audit]
        public ActionResult IncompleteReason(String parameter, String hash, String key)
        {
            IncompleteReason incompleteReason = new IncompleteReason();

            incompleteReason.EncryptedTendAgreementCode_IncompleteReason = parameter + "/" + hash + "/" + key;
            return PartialView("IncompleteReason", incompleteReason);
        }

        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeAgreementStatusToInComplete(IncompleteReason incompleteReason)
        {
            bool status = false;
            try
            {

                if (ModelState.IsValid)
                {
                    if (maintenanceAgreementBAL.ChangeAgreementStatusToInCompleteBAL(incompleteReason, ref message))
                    {

                        message = message == string.Empty ? "Agreement status changed to 'Terminate'." : message;
                        status = true;

                    }
                    else
                    {
                        message = message == string.Empty ? "Agreement status not changed." : message;
                    }
                }
                else
                {
                    return PartialView("IncompleteReason", incompleteReason);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Agreement status not changed.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteMaintenanceAgreementDetails(String parameter, String hash, String key)
        {

            bool status = false;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {

                    if (maintenanceAgreementBAL.DeleteMaintenanceAgreementDetailsBAL(Convert.ToInt32(decryptedParameters["IMSPRRoadCode"].ToString()), Convert.ToInt32(decryptedParameters["PRContractCode"].ToString()), Convert.ToInt32(decryptedParameters["ManeContractId"].ToString()), ref message))
                    {
                        message = "Agreement details deleted successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Agreement details not deleted successfully." : message;
                    }

                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                message = "Agreement details not deleted successfully.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Agreement details not deleted successfully.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }
        [Audit]
        public ActionResult CheckForExistingorNewContractor()
        {

            int IMSPRRoadCode = 0;
            int IMSWorkCode = 0;

            try
            {
                if (!string.IsNullOrEmpty(Request.Params["IMSPRRoadCode"]) && !string.IsNullOrEmpty(Request.Params["IMSWorkCode"]))
                {
                    encryptedParameters = Request.Params["IMSPRRoadCode"].Split('/');

                    if (encryptedParameters.Length == 3)
                    {
                        decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                        IMSPRRoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"].ToString());
                    }
                    IMSWorkCode = Convert.ToInt32(Request.Params["IMSWorkCode"].ToString());
                }
                else
                {
                    return Json(new { success = false }, JsonRequestBehavior.AllowGet);
                }

                if (maintenanceAgreementBAL.CheckForExistingorNewContractorBAL(IMSPRRoadCode, IMSWorkCode))
                {
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {

                    return Json(new { success = false }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }//end function CheckForExistingorNewContractor

        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeAgreementStatusToComplete(String parameter, String hash, String key)
        {
            int IMSPRRoadCode = 0;
            int PRContractCode = 0;
            bool status = false;
            message = "Agreement status not changed.";
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    IMSPRRoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"]);
                    PRContractCode = Convert.ToInt32(decryptedParameters["PRContractCode"]);


                    if (maintenanceAgreementBAL.ChangeAgreementStatusToCompleteBAL(IMSPRRoadCode, PRContractCode))
                    {
                        message = "Agreement status changed to 'Complete'.";
                        status = true;
                    }

                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                // message = message = "Agreement not finalized successfully.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [Audit]
        public ActionResult ViewMaintenancetAgreementDetails_Proposal(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptParameters = null;
            try
            {
                decryptParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptParameters.Count() > 0)
                {
                    MaintenanceAgreementDetails agreementDetails = maintenanceAgreementBAL.GetMaintenanceAgreementDetailsBAL(Convert.ToInt32(decryptParameters["IMSPRRoadCode"].ToString()), Convert.ToInt32(decryptParameters["PRContractCode"].ToString()), Convert.ToInt32(decryptParameters["ManeContractId"].ToString()), true);

                    ViewBag.ContractorName = dbContext.MASTER_CONTRACTOR.Where(c => c.MAST_CON_ID == agreementDetails.MAST_CON_ID).Select(c => c.MAST_CON_COMPANY_NAME).FirstOrDefault();

                    ViewBag.WorkName = dbContext.IMS_PROPOSAL_WORK.Where(c => c.IMS_WORK_CODE == agreementDetails.IMS_WORK_CODE).Select(c => c.IMS_WORK_DESC).FirstOrDefault();

                    if (agreementDetails == null)
                    {
                        ModelState.AddModelError(string.Empty, "Agreement details not exist.");
                        return PartialView("ViewMaintenanceAgreementDetails", new MaintenanceAgreementDetails());
                    }

                    return PartialView("ViewMaintenanceAgreementDetails", agreementDetails);
                }
                return PartialView("ViewMaintenanceAgreementDetails", new MaintenanceAgreementDetails());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(String.Empty, "Agreement details not exist.");
                return PartialView("ViewMaintenanceAgreementDetails", new MaintenanceAgreementDetails());
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


        #region PROPOSAL_RELATED_DETAILS

        [Audit]
        public ActionResult ViewProposalMaintenanceDetails(string id)
        {
            int proposalCode = 0;
            if (int.TryParse(id, out proposalCode))
            {
                //ViewBag.ProposalCode = URLEncrypt.EncryptParameters(new string[] { "IMSPRRoadCode=" + proposalCode.ToString() });
                ViewBag.ProposalCode = proposalCode;
            }
            return PartialView();
        }

        [HttpPost]
        [Audit]
        public ActionResult GetMaintenanceAgreementList(int? page, int? rows, string sidx, string sord)
        {

            long totalRecords;
            int stateCode = PMGSYSession.Current.StateCode;
            int districtCode = PMGSYSession.Current.DistrictCode;
            int adminNDCode = PMGSYSession.Current.AdminNdCode;
            int IMSPRRoadCode = 0;
            string agreementType = string.Empty;
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["ProposalCode"]))
                {
                    IMSPRRoadCode = Convert.ToInt32(Request.Params["ProposalCode"]);
                }

                var jsonData = new
                {
                    rows = maintenanceAgreementBAL.GetProposalAgreementListBAL(IMSPRRoadCode, agreementType, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : (totalRecords % Convert.ToInt32(rows) == 0 ? totalRecords / Convert.ToInt32(rows) : totalRecords / Convert.ToInt32(rows) + 1),
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

        [HttpPost]
        [Audit]
        public ActionResult GetProposalFinancialList(int? page, int? rows, string sidx, string sord)
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

                if (!(string.IsNullOrEmpty(Request.Params["ProposalCode"])))
                {
                    proposalCode = Convert.ToInt32(Request.Params["ProposalCode"]);
                }

                //if (!(string.IsNullOrEmpty(Request.Params["progressType"])))
                //{
                //    progressType = Request.Params["progressType"];
                //}

                //if (!(string.IsNullOrEmpty(Request.Params["contractCode"])))
                //{
                //    contractCode = Convert.ToInt32(Request.Params["contractCode"]);
                //}

                var jsonData = new
                {
                    rows = maintenanceAgreementBAL.GetProposalFinancialListBAL(proposalCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
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


        #region SPECIAL_AGREEMENTS

        /// <summary>
        /// returns the list of proposals for special maintenance ageements
        /// </summary>
        /// <returns></returns>
        public ActionResult ListSpecialAgreeementProposals()
        {
            try
            {
                DAL.MasterDataEntryDAL masterDataEntryDAL = new DAL.MasterDataEntryDAL();
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
                        ViewData["BlockList"] = new SelectList(masterDataEntryDAL.GetAllBlocksByDistrictCode(transactionParams.DISTRICT_CODE, true), "MAST_BLOCK_CODE", "MAST_BLOCK_NAME");
                        ViewData["PackageList"] = new SelectList(commonFunction.GetPackages(Convert.ToInt32(transactionParams.SANC_YEAR), 0, true), "IMS_PACKAGE_ID", "IMS_PACKAGE_ID");
                        ViewData["BatchList"] = commonFunction.PopulateBatch(true);
                        ViewData["CollaborationList"] = commonFunction.PopulateFundingAgency(true);
                        ViewData["UpgradationList"] = commonFunction.PopulateNewUpgradeList(true);
                    }
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                    ViewData["FinancialYearList"] = null;
                    ViewData["PackageList"] = null;
                    ViewData["BlockList"] = null;
                }

                return View();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// returns the list of proposals for adding the special maintenence agreements
        /// </summary>
        /// <returns></returns>
        [Audit]
        [HttpPost]
        public ActionResult GetCompletedRoadListForSpecialAgreements(int? page, int? rows, string sidx, string sord)
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
                else
                {
                    return null;
                }
                if (!string.IsNullOrEmpty(Request.Params["blockCode"]))
                {
                    blockCode = Convert.ToInt32(Request.Params["blockCode"]);
                }
                else
                {
                    return null;
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
                    rows = maintenanceAgreementBAL.GetCompletedRoadForSpecialAgreementsListBAL(stateCode, districtCode, blockCode, sanctionedYear, packageID, adminNDCode, batch, collaboration, upgradationType, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : (totalRecords % Convert.ToInt32(rows) == 0 ? totalRecords / Convert.ToInt32(rows) : totalRecords / Convert.ToInt32(rows) + 1),//totalRecords / Convert.ToInt32(rows) + 1,
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
        /// returns the view for adding the special maintenance agreement details
        /// </summary>
        /// <returns></returns>
        [Audit]                                 //Changed to post on 22-07-2022
        [HttpPost]
        public ActionResult AddSpecialAgreementAgainstRoad(String parameter, String hash, String key)
        {
            MaintenanceAgreementDetails maintenenaceAgreementDetails = new MaintenanceAgreementDetails();
            try
            {

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {

                    maintenenaceAgreementDetails.EncryptedIMSPRRoadCode = parameter + '/' + hash + '/' + key;
                    maintenenaceAgreementDetails.IsNewContractor = true;

                    int RoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"]);
                    IMS_SANCTIONED_PROJECTS roadData = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == RoadCode).FirstOrDefault();

                    //ViewBag.SanctionedYear = decryptedParameters["SanctionedYear"].ToString();
                    ViewBag.SanctionedYear = roadData.IMS_YEAR.ToString() + "-" + (roadData.IMS_YEAR + 1).ToString();
                    //ViewBag.RoadName = decryptedParameters["IMSRoadName"].ToString();
                    ViewBag.RoadName = roadData.IMS_ROAD_NAME.ToString();
                    //ViewBag.Package = decryptedParameters["Package"].ToString();
                    ViewBag.Package = roadData.IMS_PACKAGE_ID.ToString();
                    //ViewBag.RoadLength = decryptedParameters["RoadLength"].ToString().Replace("--", ".");
                    ViewBag.RoadLength = roadData.IMS_PAV_LENGTH.ToString();

                    return PartialView(maintenenaceAgreementDetails);
                }
                return PartialView(maintenenaceAgreementDetails);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return PartialView(maintenenaceAgreementDetails);
            }
        }

        /// <summary>
        /// view for adding the special maintenance agreement details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult SpecialAgreementDetails(String parameter, String hash, String key)
        {
            try
            {
                int IMSPRRoadCode = 0;
                DateTime? constructionComDate = null;
                MaintenanceAgreementDetails maintenenaceAgreementDetails = new MaintenanceAgreementDetails();
                maintenenaceAgreementDetails.IsNewContractor = false;
                maintenenaceAgreementDetails.EncryptedIMSPRRoadCode = parameter + "/" + hash + "/" + key;

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                IMSPRRoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"].ToString());
                int proposalCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"]);
                string proposalType = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == proposalCode).Select(m => m.IMS_PROPOSAL_TYPE).FirstOrDefault();
                EXEC_ROADS_MONTHLY_STATUS executionMaster = dbContext.EXEC_ROADS_MONTHLY_STATUS.Where(EXE => EXE.IMS_PR_ROAD_CODE == IMSPRRoadCode && EXE.EXEC_ISCOMPLETED == "C").FirstOrDefault();
                ViewBag.IsExecutionCompleted = proposalType == "P" ? (dbContext.EXEC_ROADS_MONTHLY_STATUS.Any(m => m.IMS_PR_ROAD_CODE == proposalCode && m.EXEC_ISCOMPLETED == "C") ? "Y" : "N") : (dbContext.EXEC_LSB_MONTHLY_STATUS.Any(m => m.IMS_PR_ROAD_CODE == proposalCode && m.EXEC_ISCOMPLETED == "C") ? "Y" : "N");
                if (ViewBag.IsExecutionCompleted == "N")
                {
                    maintenenaceAgreementDetails.MANE_CONSTR_COMP_DATE = new CommonFunctions().GetDateTimeToString(DateTime.Now);
                }
                if (executionMaster != null)
                {
                    constructionComDate = executionMaster.EXEC_COMPLETION_DATE;
                    maintenenaceAgreementDetails.CompletionMonth = executionMaster.EXEC_PROG_MONTH;
                    maintenenaceAgreementDetails.CompletionYear = executionMaster.EXEC_PROG_YEAR;
                    maintenenaceAgreementDetails.MANE_CONSTR_COMP_DATE = constructionComDate == null ? null : Convert.ToDateTime(constructionComDate).ToString("dd/MM/yyyy");
                }
                else
                {
                    constructionComDate = null;
                    maintenenaceAgreementDetails.CompletionMonth = 0;
                    maintenenaceAgreementDetails.CompletionYear = 0;
                    maintenenaceAgreementDetails.MANE_CONSTR_COMP_DATE = constructionComDate == null ? null : Convert.ToDateTime(constructionComDate).ToString("dd/MM/yyyy");
                }
                maintenenaceAgreementDetails.AgreementType = "S";
                ViewBag.IsNewEntry = false;
                return PartialView(maintenenaceAgreementDetails);
            }
            catch (Exception)
            {
                return null;
            }

        }

        /// <summary>
        /// returns the list of special agreements 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetSpecialAgreementDetailsList_Proposal(int? page, int? rows, string sidx, string sord)
        {
            long totalRecords;
            int IMSPRRoadCode = 0;
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["IMSPRRoadCode"]))
                {
                    encryptedParameters = Request.Params["IMSPRRoadCode"].Split('/');
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                    IMSPRRoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"]);
                }

                var jsonData = new
                {
                    rows = maintenanceAgreementBAL.GetSpecialAgreementDetailsListBAL(IMSPRRoadCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : (totalRecords % Convert.ToInt32(rows) == 0 ? totalRecords / Convert.ToInt32(rows) : totalRecords / Convert.ToInt32(rows) + 1),
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
        /// saves the details of special agreements
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSpecialAgreement(MaintenanceAgreementDetails model)
        {
            bool status = false;
            try
            {
                ModelState.Remove("MANE_YEAR2_AMOUNT");
                ModelState.Remove("MANE_YEAR3_AMOUNT");
                ModelState.Remove("MANE_YEAR4_AMOUNT");
                ModelState.Remove("MANE_YEAR5_AMOUNT");
                ModelState.Remove("MANE_YEAR6_AMOUNT");
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { model.EncryptedIMSPRRoadCode.Split('/')[0], model.EncryptedIMSPRRoadCode.Split('/')[1], model.EncryptedIMSPRRoadCode.Split('/')[2] });
                int proposalCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"]);
                if (!dbContext.EXEC_ROADS_MONTHLY_STATUS.Any(m => m.IMS_PR_ROAD_CODE == proposalCode && m.EXEC_ISCOMPLETED == "C"))
                {
                    model.MANE_CONSTR_COMP_DATE = new CommonFunctions().GetDateTimeToString(DateTime.Now);
                    ModelState.Remove("MANE_CONSTR_COMP_DATE");
                }
                if (model.ProposalWorks.Count() == 1)
                {
                    ModelState.Remove("IMS_WORK_CODE");
                }

                if (ModelState.IsValid)
                {
                    if (maintenanceAgreementBAL.SaveSpecialAgreementDetailsBAL(model, ref message))
                    {
                        message = message == string.Empty ? "Agreement details added successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Agreement details not added successfully." : message;
                    }
                }
                else
                {
                    return PartialView(model);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Agreement details not added successfully.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// returns the view for updating the special agreement details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult EditSpecialAgreementDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptParameters = null;
            try
            {
                decryptParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptParameters.Count() > 0)
                {
                    MaintenanceAgreementDetails agreementDetails = maintenanceAgreementBAL.GetMaintenanceAgreementDetailsBAL(Convert.ToInt32(decryptParameters["IMSPRRoadCode"].ToString()), Convert.ToInt32(decryptParameters["PRContractCode"].ToString()), Convert.ToInt32(decryptParameters["ManeContractId"].ToString()));
                    int proposalCode = Convert.ToInt32(decryptParameters["IMSPRRoadCode"]);
                    string proposalType = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == proposalCode).Select(m => m.IMS_PROPOSAL_TYPE).FirstOrDefault();
                    ViewBag.IsExecutionCompleted = proposalType == "P" ? (dbContext.EXEC_ROADS_MONTHLY_STATUS.Any(m => m.IMS_PR_ROAD_CODE == proposalCode && m.EXEC_ISCOMPLETED == "C") ? "Y" : "N") : (dbContext.EXEC_LSB_MONTHLY_STATUS.Any(m => m.IMS_PR_ROAD_CODE == proposalCode && m.EXEC_ISCOMPLETED == "C") ? "Y" : "N");
                    if (ViewBag.IsExecutionCompleted == "N")
                    {
                        agreementDetails.MANE_CONSTR_COMP_DATE = new CommonFunctions().GetDateTimeToString(DateTime.Now);
                    }
                    if (agreementDetails == null)
                    {
                        ModelState.AddModelError(string.Empty, "Agreement details not exist.");
                        return PartialView("MaintenanceAgreementDetails", new MaintenanceAgreementDetails());
                    }

                    return PartialView("SpecialAgreementDetails", agreementDetails);
                }
                return PartialView("SpecialAgreementDetails", new MaintenanceAgreementDetails());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(String.Empty, "Agreement details not exist.");
                return PartialView("SpecialAgreementDetails", new MaintenanceAgreementDetails());
            }
        }


        /// <summary>
        /// updates the details of special agreement details
        /// </summary>
        /// <param name="details_agreement"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult EditSpecialAgreementDetails(MaintenanceAgreementDetails details_agreement)
        {
            bool status = false;
            try
            {
                ModelState.Remove("MANE_YEAR2_AMOUNT");
                ModelState.Remove("MANE_YEAR3_AMOUNT");
                ModelState.Remove("MANE_YEAR4_AMOUNT");
                ModelState.Remove("MANE_YEAR5_AMOUNT");
                ModelState.Remove("MANE_YEAR6_AMOUNT");

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { details_agreement.EncryptedIMSPRRoadCode.Split('/')[0], details_agreement.EncryptedIMSPRRoadCode.Split('/')[1], details_agreement.EncryptedIMSPRRoadCode.Split('/')[2] });
                int proposalCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"]);
                if (!dbContext.EXEC_ROADS_MONTHLY_STATUS.Any(m => m.IMS_PR_ROAD_CODE == proposalCode && m.EXEC_ISCOMPLETED == "C"))
                {
                    details_agreement.MANE_CONSTR_COMP_DATE = new CommonFunctions().GetDateTimeToString(DateTime.Now);
                    ModelState.Remove("MANE_CONSTR_COMP_DATE");
                }

                if (details_agreement.ProposalWorks.Count() == 1)
                {
                    ModelState.Remove("IMS_WORK_CODE");
                }

                if (ModelState.IsValid)
                {
                    if (maintenanceAgreementBAL.UpdateSpecialAgreementDetailsBAL(details_agreement, ref message))
                    {
                        message = message == string.Empty ? "Agreement details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Agreement details not updated successfully." : message;
                    }
                }
                else
                {
                    return PartialView("SpecialAgreementDetails", details_agreement);
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = message == string.Empty ? "Agreement details not updated successfully." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        [Audit]
        public ActionResult ViewSpecialAgreementDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptParameters = null;
            try
            {
                decryptParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptParameters.Count() > 0)
                {
                    MaintenanceAgreementDetails agreementDetails = maintenanceAgreementBAL.GetMaintenanceAgreementDetailsBAL(Convert.ToInt32(decryptParameters["IMSPRRoadCode"].ToString()), Convert.ToInt32(decryptParameters["PRContractCode"].ToString()), Convert.ToInt32(decryptParameters["ManeContractId"].ToString()), true);

                    ViewBag.ContractorName = dbContext.MASTER_CONTRACTOR.Where(c => c.MAST_CON_ID == agreementDetails.MAST_CON_ID).Select(c => c.MAST_CON_COMPANY_NAME).FirstOrDefault();

                    ViewBag.WorkName = dbContext.IMS_PROPOSAL_WORK.Where(c => c.IMS_WORK_CODE == agreementDetails.IMS_WORK_CODE).Select(c => c.IMS_WORK_DESC).FirstOrDefault();

                    if (agreementDetails == null)
                    {
                        ModelState.AddModelError(string.Empty, "Agreement details not exist.");
                        return PartialView("ViewSpecialAgreementDetails", new MaintenanceAgreementDetails());
                    }

                    return PartialView("ViewSpecialAgreementDetails", agreementDetails);
                }
                return PartialView("ViewSpecialAgreementDetails", new MaintenanceAgreementDetails());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(String.Empty, "Agreement details not exist.");
                return PartialView("ViewSpecialAgreementDetails", new MaintenanceAgreementDetails());
            }
        }

        #endregion


        #region TECHNOLOGY

        /// <summary>
        /// returns the view for listing the technology details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult ListTechnologyDetails(int RoadCode, int ContractCode)
        {

            try
            {

                CommonFunctions objCommon = new CommonFunctions();
                PMGSY.DAL.MaintainanceInspection.MaintainanceInspectionDAL objDAL = new PMGSY.DAL.MaintainanceInspection.MaintainanceInspectionDAL();
                PMGSY.Models.MaintainanceInspection.MaintenanceProgressViewModel progressModel = new PMGSY.Models.MaintainanceInspection.MaintenanceProgressViewModel();


                progressModel = objDAL.GetFinancialAddDetails(RoadCode, ContractCode);


                ViewBag.ProposalCode = RoadCode;
                ViewBag.ContractCode = ContractCode;
                ViewBag.BlockName = progressModel.BlockName;
                ViewBag.Package = progressModel.Package;
                ViewBag.RoadName = progressModel.RoadName;
                ViewBag.AgreementDate = progressModel.AgreementDate;
                ViewBag.OverallCost = progressModel.OverallCost;
                ViewBag.SanctionLength = progressModel.SanctionLength;

                return PartialView("ListTechnologyDetails");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        /// <summary>
        /// get method for add view of Technology details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [Audit]
        public ActionResult AddTechnologyDetails(int RoadCode, int ContractCode)
        {

            MaintenanceTechnologyDetailsViewModel model = new MaintenanceTechnologyDetailsViewModel();
            CommonFunctions objCommon = new CommonFunctions();

            try
            {
                PMGSY.Models.MaintainanceInspection.MaintenanceProgressViewModel progressModel = new PMGSY.Models.MaintainanceInspection.MaintenanceProgressViewModel();
                PMGSY.DAL.MaintainanceInspection.MaintainanceInspectionDAL objDAL = new PMGSY.DAL.MaintainanceInspection.MaintainanceInspectionDAL();

                progressModel = objDAL.GetFinancialAddDetails(RoadCode, ContractCode);



                model.IMS_PR_SANCTIONED_LENGTH = (decimal)progressModel.SanctionLength;

                model.IMS_PR_ROAD_CODE = RoadCode;
                model.MANE_CONTRACT_CODE = ContractCode;

                model.ListLayers = objCommon.PopulateRoadExecutionItems();
                model.ListTechnology = objCommon.PopulateTechnologyItems();
                model.Operation = "A";
                return PartialView("AddTechnologyDetails", model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        /// <summary>
        /// post method for adding the Technology details
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult AddTechnologyDetails(MaintenanceTechnologyDetailsViewModel model)
        {
            try
            {
                string message = string.Empty;
                maintenanceAgreementBAL = new MaintenanceAgreementBAL();
                //  objProposalBAL = new PMGSY.BAL.Proposal.ProposalBAL();
                if (ModelState.IsValid)
                {
                    bool status = maintenanceAgreementBAL.AddTechnologyDetailsBAL(model, ref message);
                    if (status)
                        return Json(new { success = true, message = message });
                    else
                        return Json(new { success = false, message = message });
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        /// <summary>
        /// returns data of particular technology details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult GetTechnologyDetailsForEdit(String parameter, String hash, String key)
        {
            maintenanceAgreementBAL = new MaintenanceAgreementBAL();
            //objProposalBAL = new PMGSY.BAL.Proposal.ProposalBAL();
            Dictionary<string, string> decryptedParameters = null;
            //MainenanceTechnologyDetailsViewModel model = new PMGSY.Models.Proposal.TechnologyDetailsViewModel();
            MaintenanceTechnologyDetailsViewModel model = new MaintenanceTechnologyDetailsViewModel();
            CommonFunctions objCommom = new CommonFunctions();
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                model = maintenanceAgreementBAL.GetTechnologyDetails(Convert.ToInt32(decryptedParameters["ProposalCode"]), Convert.ToInt32(decryptedParameters["ContractCode"]), Convert.ToInt32(decryptedParameters["SegmentCode"]));
                model.ListLayers = objCommom.PopulateRoadExecutionItems();
                model.ListTechnology = objCommom.PopulateTechnologyItems();

                PMGSY.Models.MaintainanceInspection.MaintenanceProgressViewModel progressModel = new PMGSY.Models.MaintainanceInspection.MaintenanceProgressViewModel();
                PMGSY.DAL.MaintainanceInspection.MaintainanceInspectionDAL objDAL = new PMGSY.DAL.MaintainanceInspection.MaintainanceInspectionDAL();

                progressModel = objDAL.GetFinancialAddDetails(Convert.ToInt32(decryptedParameters["ProposalCode"]), Convert.ToInt32(decryptedParameters["ContractCode"]));



                model.IMS_PR_SANCTIONED_LENGTH = (decimal)progressModel.SanctionLength;


                if (model != null)
                {
                    return PartialView("AddTechnologyDetails", model);
                }
                else
                {
                    return Json(new { success = false, message = "Error occurred while processing your request." });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }


        /// <summary>
        /// post method for updating the Technology details
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult EditTechnologyDetails(MaintenanceTechnologyDetailsViewModel model)
        {
            try
            {
                //MaintenanceTechnologyDetailsViewModel model = new MaintenanceTechnologyDetailsViewModel();
                string message = string.Empty;
                maintenanceAgreementBAL = new MaintenanceAgreementBAL();
                if (ModelState.IsValid)
                {
                    bool status = maintenanceAgreementBAL.EditTechnologyDetailsBAL(model, ref message);
                    if (status)
                        return Json(new { success = true, message = message });
                    else
                        return Json(new { success = false, message = message });
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }

        /// <summary>
        /// deletes the technology details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult DeleteTechnologyDetails(String parameter, String hash, String key)
        {

            maintenanceAgreementBAL = new MaintenanceAgreementBAL();
            Dictionary<string, string> decryptedParameters = null;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                bool status = maintenanceAgreementBAL.DeleteTechnologyDetails(Convert.ToInt32(decryptedParameters["ProposalCode"]), Convert.ToInt32(decryptedParameters["SegmentCode"]), Convert.ToInt32(decryptedParameters["ContractCode"]));
                if (status == true)
                {
                    return Json(new { success = true, message = "Technology Details deleted successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Error occurred while processing your request." });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }
        }


        /// <summary>
        /// returns the list of Technology details
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetTechnologyDetailsList(int RoadCode, int contractCode, int? page, int? rows, string sidx, string sord)
        {
            //Adde By Abhishek kamble 30-Apr-2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 30-Apr-2014 end
            //objProposalBAL = new PMGSY.BAL.Proposal.ProposalBAL();
            maintenanceAgreementBAL = new MaintenanceAgreementBAL();
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





                var jsonData = new
                {
                    rows = maintenanceAgreementBAL.GetTechnologyDetailsListBAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, RoadCode, contractCode),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "Error occurred while processing your request." });
            }

        }

        public JsonResult GetStartChainage(String id)
        {
            int proposalCode = 0;
            int techCode = 0;
            int layerCode = 0;
            maintenanceAgreementBAL = new MaintenanceAgreementBAL();
            try
            {
                if (!String.IsNullOrEmpty(id))
                {
                    if (id.Split('$').Count() == 3)
                    {
                        proposalCode = Convert.ToInt32(id.Split('$')[0]);
                        techCode = Convert.ToInt32(id.Split('$')[1]);
                        layerCode = Convert.ToInt32(id.Split('$')[2]);
                    }
                }
                decimal? endChainage = maintenanceAgreementBAL.GetTechnologyStartChainage(proposalCode, techCode, layerCode);
                // decimal? endChainage = objProposalBAL.GetTechnologyStartChainage(proposalCode, techCode, layerCode);
                return Json(new { Success = true, StartChainage = endChainage });

            }
            catch (Exception)
            {
                return Json(null);
            }
        }

        #endregion

        #region PERIODIC MAINTENANCE [25-04-2017]

        [HttpGet]
        public ViewResult PeriodicMaintenceLayout()
        {
            DAL.MasterDataEntryDAL masterDataEntryDAL = new DAL.MasterDataEntryDAL();
            try
            {
                TransactionParams transactionParams = new TransactionParams();
                transactionParams.STATE_CODE = PMGSYSession.Current.StateCode;
                transactionParams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                transactionParams.ISSearch = true;
                transactionParams.SANC_YEAR = (Int16)DateTime.Now.Year;

                PeriodicMaintenanceModel model = new PeriodicMaintenanceModel();
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    model.YearList = commonFunction.PopulateFinancialYear(true, true).ToList<SelectListItem>();

                    model.BlockList = new SelectList(masterDataEntryDAL.GetAllBlocksByDistrictCode(transactionParams.DISTRICT_CODE, true), "MAST_BLOCK_CODE", "MAST_BLOCK_NAME").ToList<SelectListItem>();
                    model.PackageList = new SelectList(commonFunction.GetPackages(Convert.ToInt32(transactionParams.SANC_YEAR), 0, true), "IMS_PACKAGE_ID", "IMS_PACKAGE_ID").ToList<SelectListItem>();

                    model.BatchList = commonFunction.PopulateBatch(true);
                    model.FundingAgencyList = commonFunction.PopulateFundingAgency(true);
                    model.NewUpgradationList = commonFunction.PopulateNewUpgradeList(true);
                    //end of change
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PeriodicMaintenceLayout()");
                return null;
            }
        }

        [HttpPost]
        public ActionResult GetPeriodicCompletedRoadList(int? page, int? rows, string sidx, string sord)
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
                else
                {
                    return null;
                }
                if (!string.IsNullOrEmpty(Request.Params["blockCode"]))
                {
                    blockCode = Convert.ToInt32(Request.Params["blockCode"]);
                }
                else
                {
                    return null;
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
                    rows = maintenanceAgreementBAL.GetPeriodicCompletedRoadListBAL(stateCode, districtCode, blockCode, sanctionedYear, packageID, adminNDCode, batch, collaboration, upgradationType, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : (totalRecords % Convert.ToInt32(rows) == 0 ? totalRecords / Convert.ToInt32(rows) : totalRecords / Convert.ToInt32(rows) + 1),//totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };
                return Json(jsonData);

            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "GetPeriodicCompletedRoadList()");
                return null;
            }

        }

        [HttpGet]
        public ViewResult AddPeriodicMaintenance(String parameter, String hash, String key)
        {
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    dbContext = new PMGSYEntities();
                    AddPeriodicMaintenanceModel model = new AddPeriodicMaintenanceModel();

                    // var EncryptedIMSPRRoadCode = parameter + '/' + hash + '/' + key;

                    //string SanctionYear = decryptedParameters["SanctionedYear"];
                    string RoadLength = decryptedParameters["RoadLength"].ToString();
                    //string imsPackage = decryptedParameters["Package"];
                    int prRoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"].ToString());
                    model.IsSecondPeriodic = "N";
                    //if (decryptedParameters.Count > 4)
                    //{
                    //    String MainType = decryptedParameters["mainType"];
                    //    model.IsSecondPeriodic = MainType;
                    //}

                    //String RoadCodeLength=Convert.ToString( TempData["Roadcode"]);
                    //decimal RoadLength = Convert.ToDecimal(RoadCodeLength.Split(',')[1]);
                    //int prRoadCode = Convert.ToInt32(RoadCodeLength.Split(',')[0]);
                    //   String RoadName = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PACKAGE_ID == imsPackage && x.IMS_PR_ROAD_CODE == prRoadCode).Select(s => s.IMS_ROAD_NAME).FirstOrDefault();


                    model.ImdRoadCode = prRoadCode;
                    List<SelectListItem> Techlst = new SelectList(commonFunction.PopulateTechnologyItems(), "MAST_TECH_CODE", "MAST_TECH_NAME").ToList<SelectListItem>();
                    //Tempory only for two technology
                    model.TechnologyList.Add(Techlst.Where(t => t.Value == "54").FirstOrDefault());/// Added by SAMMED A. PATIL on 26DEC2017 as per requirment from Pankaj Sir
                    model.TechnologyList.Add(Techlst.Where(t => t.Value == "3").FirstOrDefault());
                    model.TechnologyList.Add(Techlst.Where(t => t.Value == "14").FirstOrDefault());

                    model.TechnologyList.Insert(0, new SelectListItem { Selected = true, Value = "0", Text = "Select Technology" });
                    model.RenewalTypeList = maintenanceAgreementDAL.GetRenewalTypeList();
                    model.RenewalTypeList.Insert(0, new SelectListItem { Selected = true, Value = "0", Text = "Select Renewal type" });
                    model.MANE_MAIN_YEAR_LIST = maintenanceAgreementDAL.GetYearList(prRoadCode, false);
                    //model.PerFormanceYearList = new List<SelectListItem> { new SelectListItem { Text = DateTime.Now.Year - 1 + "-" + DateTime.Now.Year, Value = DateTime.Now.Year - 1 + "" },
                    //                                                         new SelectListItem {Text=DateTime.Now.Year +"-"+(DateTime.Now.Year+1),Value=DateTime.Now.Year +"" } };
                    for (int year = 2016; year <= DateTime.Now.Year; year++)
                    {
                        model.PerFormanceYearList.Add(new SelectListItem { Text = year + "-" + (year + 1), Value = year + "" });
                    }
                    if (DateTime.Now.Month <= 3)
                    {
                        int count = model.MANE_MAIN_YEAR_LIST.IndexOf(model.MANE_MAIN_YEAR_LIST.Find(c => c.Value == DateTime.Now.Year.ToString()));
                        //model.PerFormanceYearList.RemoveRange(model.PerFormanceYearList.IndexOf(model.PerFormanceYearList.Find(c => c.Value == DateTime.Now.Year.ToString())), model.PerFormanceYearList.Count - count);

                        model.MANE_MAIN_YEAR_LIST.RemoveAt(count);

                        count = model.PerFormanceYearList.IndexOf(model.PerFormanceYearList.Find(c => c.Value == DateTime.Now.Year.ToString()));
                        model.PerFormanceYearList.RemoveAt(count);
                    }
                    model.PerFormanceYearList.Insert(0, new SelectListItem { Value = "0", Text = "Select Year", Selected = true });
                    model.MANE_MAIN_MONTH_LIST = maintenanceAgreementDAL.getMonthList();
                    model.Operation = "A";
                    model.IsPerformaceIncentive = "N";
                    model.Iscompleted = "N";
                    model.Length = Convert.ToDecimal(RoadLength);

                    model.intMaxValue = int.MaxValue;
                    return View("AddPeriodicMaintenance", model);
                }
            }
            catch (Exception ex)
            {
                // Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "AddPeriodicMaintenance()");
                return null;
            }
            return null;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddPeriodicMaintenance(AddPeriodicMaintenanceModel model)
        {
            message = "Error occured while processing ypur request";

            Boolean Status = false;
            try
            {
                Status = maintenanceAgreementBAL.AddPeriodicMaintenanceBAL(model, out message);

                if (Status)
                    return Json(new { success = Status, message = message });
                return Json(new { success = Status, message = message });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AddPeriodicMaintenance()");
                return null;
            }
        }

        [HttpGet]
        public ViewResult EditPeriodicMaintenance(String parameter, String hash, String key)
        {
            decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
            try
            {


                string RoadLength = decryptedParameters["RoadLength"].ToString();


                int prRoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"].ToString());


                AddPeriodicMaintenanceModel model = maintenanceAgreementBAL.GetPeriodicMentainanceModelBAL(prRoadCode);

                if (decryptedParameters.Count > 4)
                {
                    String MainType = decryptedParameters["mainType"];
                    model.IsSecondPeriodic = MainType;
                }
                model.ImdRoadCode = prRoadCode;
                List<SelectListItem> Techlst = new SelectList(commonFunction.PopulateTechnologyItems(), "MAST_TECH_CODE", "MAST_TECH_NAME").ToList<SelectListItem>();
                //Tempory only for two technology
                model.TechnologyList.Add(Techlst.Where(t => t.Value == "54").FirstOrDefault());/// Added by SAMMED A. PATIL on 26DEC2017 as per requirment from Pankaj Sir
                model.TechnologyList.Add(Techlst.Where(t => t.Value == "3").FirstOrDefault());
                model.TechnologyList.Add(Techlst.Where(t => t.Value == "14").FirstOrDefault());

                model.TechnologyList.Insert(0, new SelectListItem { Selected = true, Value = "0", Text = "Select Technology" });
                model.RenewalTypeList = maintenanceAgreementDAL.GetRenewalTypeList();
                model.RenewalTypeList.Insert(0, new SelectListItem { Selected = true, Value = "0", Text = "Select Renewal type" });
                model.MANE_MAIN_YEAR_LIST = maintenanceAgreementDAL.GetYearList(prRoadCode, false);
                //model.PerFormanceYearList = new List<SelectListItem> { new SelectListItem { Text = DateTime.Now.Year - 1 + "-" + DateTime.Now.Year, Value = DateTime.Now.Year - 1 + "" },
                //                                                         new SelectListItem {Text=DateTime.Now.Year +"-"+(DateTime.Now.Year+1),Value=DateTime.Now.Year +"" } };
                for (int year = 2016; year <= DateTime.Now.Year; year++)
                {
                    model.PerFormanceYearList.Add(new SelectListItem { Text = year + "-" + (year + 1), Value = year + "" });
                }

                model.PerFormanceYearList.Insert(0, new SelectListItem { Value = "0", Text = "Select Year", Selected = true });
                model.MANE_MAIN_MONTH_LIST = maintenanceAgreementDAL.getMonthList();

                model.Operation = "U";
                model.Length = Convert.ToDecimal(RoadLength);

                model.intMaxValue = int.MaxValue;

                return View("AddPeriodicMaintenance", model);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EditPeriodicMaintenanceGET()");
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult EditPeriodicMaintenance(AddPeriodicMaintenanceModel model)
        {
            message = "Error occured while processing ypur request";

            Boolean Status = false;
            try
            {
                Status = maintenanceAgreementBAL.EditPeriodicMaintenanceBAL(model, out message);

                if (Status)
                    return Json(new { success = Status, message = message });
                return Json(new { success = Status, message = message });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EditPeriodicMaintenancePOST()");
                return null;
            }
        }

        [HttpGet]
        public ViewResult ViewPeriodicMaintenance(String parameter, String hash, String key)
        {
            try
            {

                string RoadLength = String.Empty;
                int imsRoadCode = 0;
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                if (decryptedParameters.Count > 0)
                {
                    imsRoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"].ToString());
                    RoadLength = decryptedParameters["RoadLength"].ToString().Replace("--", ".");
                }
                // Tuple<String, String, String, String> CurrentTuple = maintenanceAgreementDAL.GetCurrentRoadInfo(imsRoadCode);
                //ViewData["CurrentRoadTuple"] = CurrentTuple;
                ViewData["RoadCode"] = imsRoadCode;
                ViewData["RoadLength"] = RoadLength;

                TempData["RoadLength"] = RoadLength;

                //List<AddPeriodicMaintenanceModel> lstPeriodicMaintence = maintenanceAgreementBAL.GetPariodicMaintenceViewListBAL(imsRoadCode,Convert.ToDouble( RoadLength));

                return View();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ViewPeriodicMaintenance()");
                return null;
            }

        }


        [HttpPost]
        public JsonResult ViewPeriodicMaintenanceList(String RoadCode, String Roadlength, int? page, int? rows, string sidx, string sord)
        {
            long totalRecords;
            try
            {
                String AddButton = String.Empty;
                decimal RoadLength = Convert.ToDecimal(Roadlength);
                var jsonData = new
                {
                    rows = maintenanceAgreementBAL.ViewPeriodicmaintenanceListBAL(Convert.ToInt32(RoadCode), Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, RoadLength, out AddButton),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : (totalRecords % Convert.ToInt32(rows) == 0 ? totalRecords / Convert.ToInt32(rows) : totalRecords / Convert.ToInt32(rows) + 1),//totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                    addbutton = AddButton
                };
                return Json(jsonData);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetPeriodicCompletedRoadList()");
                return null;
            }

        }

        [HttpPost]
        public JsonResult DeletePeriodicMaintenanceDetails(String parameter, String hash, String key)
        {
            message = "Error occured while processing your request";
            try
            {
                Int32 imsRoadCode = 0;
                int ManeId = 0;
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
                if (decryptedParameters.Count > 0)
                {
                    imsRoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"].ToString());
                    ManeId = Convert.ToInt32(decryptedParameters["ManeId"].ToString());

                }
                Boolean Status = maintenanceAgreementDAL.DeletePeriodicMantenanceDetails(imsRoadCode, ManeId, out message);
                if (Status)
                    return Json(new { success = Status, message = message });
                return Json(new { success = Status, message = message });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DeletePeriodicMaintenanceDetails()");
                return Json(new { message = message });
            }

        }
        #endregion

        #region Renewal Agreement Added by SAMMED A. PATIL on 29JAN2018
        /// <summary>
        /// returns the list of proposals for renewal ageements
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ListRenewalAgreeementProposals()
        {
            try
            {
                DAL.MasterDataEntryDAL masterDataEntryDAL = new DAL.MasterDataEntryDAL();
                
                TransactionParams transactionParams = new TransactionParams();
                transactionParams.STATE_CODE = PMGSYSession.Current.StateCode;
                transactionParams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                transactionParams.ISSearch = true;
                transactionParams.SANC_YEAR = (Int16)DateTime.Now.Year;

                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    ViewData["FinancialYearList"] = commonFunction.PopulateFinancialYear(true, true);
                    ViewData["BlockList"] = new SelectList(masterDataEntryDAL.GetAllBlocksByDistrictCode(transactionParams.DISTRICT_CODE, true), "MAST_BLOCK_CODE", "MAST_BLOCK_NAME");
                    ViewData["PackageList"] = new SelectList(commonFunction.GetPackages(Convert.ToInt32(transactionParams.SANC_YEAR), 0, true), "IMS_PACKAGE_ID", "IMS_PACKAGE_ID");
                    ViewData["BatchList"] = commonFunction.PopulateBatch(true);
                    ViewData["CollaborationList"] = commonFunction.PopulateFundingAgency(true);
                    ViewData["UpgradationList"] = commonFunction.PopulateNewUpgradeList(true);
                }
                return View();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MaintenanceAgreement.ListRenewalAgreeementProposals()");
                return null;
            }
        }

        /// <summary>
        /// returns the list of proposals for adding the special maintenence agreements
        /// </summary>
        /// <returns></returns>
        [Audit]
        [HttpPost]
        public ActionResult GetCompletedRoadListForRenewalAgreements(int? page, int? rows, string sidx, string sord)
        {
            MaintenanceAgreementDAL objDAL = new MaintenanceAgreementDAL();
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
                else
                {
                    return null;
                }
                if (!string.IsNullOrEmpty(Request.Params["blockCode"]))
                {
                    blockCode = Convert.ToInt32(Request.Params["blockCode"]);
                }
                else
                {
                    return null;
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
                    rows = objDAL.GetCompletedRoadForRenewalAgreementsListDAL(stateCode, districtCode, blockCode, sanctionedYear, packageID, adminNDCode, batch, collaboration, upgradationType, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : (totalRecords % Convert.ToInt32(rows) == 0 ? totalRecords / Convert.ToInt32(rows) : totalRecords / Convert.ToInt32(rows) + 1),//totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MaintenanceAgreement.GetCompletedRoadListForRenewalAgreements()");
                return null;
            }
        }

        /// <summary>
        /// returns the view for adding the special maintenance agreement details
        /// </summary>
        /// <returns></returns>
        [Audit]
        [HttpGet]
        public ActionResult AddRenewalAgreementAgainstRoad(String parameter, String hash, String key)
        {
            int prRoadCode = 0;
            MaintenanceAgreementDAL objDAL = new MaintenanceAgreementDAL();
            //MaintenanceAgreementDetails maintenenaceAgreementDetails = new MaintenanceAgreementDetails();
            RenewalMaintenanceViewModel maintenenaceAgreementDetails = new RenewalMaintenanceViewModel();
            try
            {

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {

                    maintenenaceAgreementDetails.EncryptedIMSPRRoadCode = parameter + '/' + hash + '/' + key;
                    maintenenaceAgreementDetails.IsNewContractor = true;

                    ViewBag.SanctionedYear = decryptedParameters["SanctionedYear"].ToString();
                    //ViewBag.RoadName = decryptedParameters["IMSRoadName"].ToString();

                    prRoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"]);

                    ViewBag.RoadName = objDAL.GetRoadName(prRoadCode);
                    ViewBag.Package = decryptedParameters["Package"].ToString();
                    ViewBag.RoadLength = decryptedParameters["RoadLength"].ToString().Replace("--", ".");

                    return PartialView(maintenenaceAgreementDetails);
                }
                return PartialView(maintenenaceAgreementDetails);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MaintenanceAgreement.AddRenewalAgreementAgainstRoad()");
                return PartialView(maintenenaceAgreementDetails);
            }
        }

        /// <summary>
        /// view for adding the special maintenance agreement details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [Audit]
        [HttpGet]
        public ActionResult RenewalAgreementDetails(String parameter, String hash, String key)
        {
            try
            {
                int IMSPRRoadCode = 0;
                DateTime? constructionComDate = null;
                //MaintenanceAgreementDetails maintenenaceAgreementDetails = new MaintenanceAgreementDetails();
                RenewalMaintenanceViewModel maintenenaceAgreementDetails = new RenewalMaintenanceViewModel();
                maintenenaceAgreementDetails.IsNewContractor = false;
                maintenenaceAgreementDetails.EncryptedIMSPRRoadCode = parameter + "/" + hash + "/" + key;

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                IMSPRRoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"].ToString());
                int proposalCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"]);
                string proposalType = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == proposalCode).Select(m => m.IMS_PROPOSAL_TYPE).FirstOrDefault();
                EXEC_ROADS_MONTHLY_STATUS executionMaster = dbContext.EXEC_ROADS_MONTHLY_STATUS.Where(EXE => EXE.IMS_PR_ROAD_CODE == IMSPRRoadCode && EXE.EXEC_ISCOMPLETED == "C").FirstOrDefault();
                ViewBag.IsExecutionCompleted = proposalType == "P" ? (dbContext.EXEC_ROADS_MONTHLY_STATUS.Any(m => m.IMS_PR_ROAD_CODE == proposalCode && m.EXEC_ISCOMPLETED == "C") ? "Y" : "N") : (dbContext.EXEC_LSB_MONTHLY_STATUS.Any(m => m.IMS_PR_ROAD_CODE == proposalCode && m.EXEC_ISCOMPLETED == "C") ? "Y" : "N");
                if (ViewBag.IsExecutionCompleted == "N")
                {
                    maintenenaceAgreementDetails.MANE_CONSTR_COMP_DATE = new CommonFunctions().GetDateTimeToString(DateTime.Now);
                }
                if (executionMaster != null)
                {
                    constructionComDate = executionMaster.EXEC_COMPLETION_DATE;
                    maintenenaceAgreementDetails.CompletionMonth = executionMaster.EXEC_PROG_MONTH;
                    maintenenaceAgreementDetails.CompletionYear = executionMaster.EXEC_PROG_YEAR;
                    maintenenaceAgreementDetails.MANE_CONSTR_COMP_DATE = constructionComDate == null ? null : Convert.ToDateTime(constructionComDate).ToString("dd/MM/yyyy");
                }
                else
                {
                    constructionComDate = null;
                    maintenenaceAgreementDetails.CompletionMonth = 0;
                    maintenenaceAgreementDetails.CompletionYear = 0;
                    maintenenaceAgreementDetails.MANE_CONSTR_COMP_DATE = constructionComDate == null ? null : Convert.ToDateTime(constructionComDate).ToString("dd/MM/yyyy");
                }
                maintenenaceAgreementDetails.AgreementType = "T";
                ViewBag.IsNewEntry = false;
                return PartialView(maintenenaceAgreementDetails);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MaintenanceAgreement.RenewalAgreementDetails()");
                return null;
            }

        }

        /// <summary>
        /// returns the list of special agreements 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult GetRenewalAgreementDetailsList_Proposal(int? page, int? rows, string sidx, string sord)
        {
            MaintenanceAgreementDAL objDAL = new MaintenanceAgreementDAL();
            long totalRecords;
            int IMSPRRoadCode = 0;
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["IMSPRRoadCode"]))
                {
                    encryptedParameters = Request.Params["IMSPRRoadCode"].Split('/');
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                    IMSPRRoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"]);
                }

                var jsonData = new
                {
                    rows = objDAL.GetRenewalAgreementDetailsListDAL(IMSPRRoadCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : (totalRecords % Convert.ToInt32(rows) == 0 ? totalRecords / Convert.ToInt32(rows) : totalRecords / Convert.ToInt32(rows) + 1),
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "MaintenanceAgreement.GetRenewalAgreementDetailsList_Proposal()");
                return null;
            }

        }

        /// <summary>
        /// saves the details of special agreements
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddRenewalAgreement(RenewalMaintenanceViewModel model)
        {
            MaintenanceAgreementDAL objDAL = new MaintenanceAgreementDAL();
            bool status = false;
            try
            {
                //ModelState.Remove("MANE_YEAR2_AMOUNT");
                //ModelState.Remove("MANE_YEAR3_AMOUNT");
                //ModelState.Remove("MANE_YEAR4_AMOUNT");
                //ModelState.Remove("MANE_YEAR5_AMOUNT");
                ModelState.Remove("MANE_YEAR6_AMOUNT");
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { model.EncryptedIMSPRRoadCode.Split('/')[0], model.EncryptedIMSPRRoadCode.Split('/')[1], model.EncryptedIMSPRRoadCode.Split('/')[2] });
                int proposalCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"]);
                if (!dbContext.EXEC_ROADS_MONTHLY_STATUS.Any(m => m.IMS_PR_ROAD_CODE == proposalCode && m.EXEC_ISCOMPLETED == "C"))
                {
                    model.MANE_CONSTR_COMP_DATE = new CommonFunctions().GetDateTimeToString(DateTime.Now);
                    ModelState.Remove("MANE_CONSTR_COMP_DATE");
                }
                if (model.ProposalWorks.Count() == 1)
                {
                    ModelState.Remove("IMS_WORK_CODE");
                }

                if (ModelState.IsValid)
                {
                    if (objDAL.SaveRenewalAgreementDetailsDAL(model, ref message))
                    {
                        message = message == string.Empty ? "Renewal Agreement details added successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Renewal Agreement details not added successfully." : message;
                    }
                }
                else
                {
                    return PartialView(model);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MaintenanceAgreement.AddRenewalAgreement()");
                message = "Renewal Agreement details not added successfully.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// returns the view for updating the special agreement details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult EditRenewalAgreementDetails(String parameter, String hash, String key)
        {
            MaintenanceAgreementDAL maintenanceAgreementDAL = new MaintenanceAgreementDAL();
            Dictionary<string, string> decryptParameters = null;
            try
            {
                decryptParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptParameters.Count() > 0)
                {
                    RenewalMaintenanceViewModel agreementDetails = maintenanceAgreementDAL.GetRenewalAgreementDetailsDAL(Convert.ToInt32(decryptParameters["IMSPRRoadCode"].ToString()), Convert.ToInt32(decryptParameters["PRContractCode"].ToString()), Convert.ToInt32(decryptParameters["ManeContractId"].ToString()));
                    int proposalCode = Convert.ToInt32(decryptParameters["IMSPRRoadCode"]);
                    string proposalType = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == proposalCode).Select(m => m.IMS_PROPOSAL_TYPE).FirstOrDefault();
                    ViewBag.IsExecutionCompleted = proposalType == "P" ? (dbContext.EXEC_ROADS_MONTHLY_STATUS.Any(m => m.IMS_PR_ROAD_CODE == proposalCode && m.EXEC_ISCOMPLETED == "C") ? "Y" : "N") : (dbContext.EXEC_LSB_MONTHLY_STATUS.Any(m => m.IMS_PR_ROAD_CODE == proposalCode && m.EXEC_ISCOMPLETED == "C") ? "Y" : "N");
                    if (ViewBag.IsExecutionCompleted == "N")
                    {
                        agreementDetails.MANE_CONSTR_COMP_DATE = new CommonFunctions().GetDateTimeToString(DateTime.Now);
                    }


                    if (agreementDetails == null)
                    {
                        ModelState.AddModelError(string.Empty, "Agreement details not exist.");
                        return PartialView("MaintenanceAgreementDetails", new MaintenanceAgreementDetails());
                    }

                    return PartialView("RenewalAgreementDetails", agreementDetails);
                }
                return PartialView("RenewalAgreementDetails", new RenewalMaintenanceViewModel());
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Maintenance");
                ModelState.AddModelError(String.Empty, "Agreement details not exist.");
                return PartialView("SpecialAgreementDetails", new RenewalMaintenanceViewModel());
            }
        }

        /// <summary>
        /// updates the details of special agreement details
        /// </summary>
        /// <param name="details_agreement"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult EditRenewalAgreementDetails(RenewalMaintenanceViewModel details_agreement)
        {
            MaintenanceAgreementDAL maintenanceAgreementDAL = new MaintenanceAgreementDAL();
            bool status = false;
            try
            {
                //ModelState.Remove("MANE_YEAR2_AMOUNT");
                //ModelState.Remove("MANE_YEAR3_AMOUNT");
                //ModelState.Remove("MANE_YEAR4_AMOUNT");
                //ModelState.Remove("MANE_YEAR5_AMOUNT");
                //ModelState.Remove("MANE_YEAR6_AMOUNT");

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { details_agreement.EncryptedIMSPRRoadCode.Split('/')[0], details_agreement.EncryptedIMSPRRoadCode.Split('/')[1], details_agreement.EncryptedIMSPRRoadCode.Split('/')[2] });
                int proposalCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"]);
                if (!dbContext.EXEC_ROADS_MONTHLY_STATUS.Any(m => m.IMS_PR_ROAD_CODE == proposalCode && m.EXEC_ISCOMPLETED == "C"))
                {
                    details_agreement.MANE_CONSTR_COMP_DATE = new CommonFunctions().GetDateTimeToString(DateTime.Now);
                    ModelState.Remove("MANE_CONSTR_COMP_DATE");
                }

                if (details_agreement.ProposalWorks.Count() == 1)
                {
                    ModelState.Remove("IMS_WORK_CODE");
                }

                if (ModelState.IsValid)
                {
                    if (maintenanceAgreementDAL.UpdateRenewalAgreementDetailsDAL(details_agreement, ref message))
                    {
                        message = message == string.Empty ? "Agreement details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Agreement details not updated successfully." : message;
                    }
                }
                else
                {
                    return PartialView("RenewalAgreementDetails", details_agreement);
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Maintenance.EditRenewalAgreementDetails()");
                message = message == string.Empty ? "Agreement details not updated successfully." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [Audit]
        public ActionResult ViewRenewalAgreementDetails(String parameter, String hash, String key)
        {
            MaintenanceAgreementDAL maintenanceAgreementDAL = new MaintenanceAgreementDAL();
            Dictionary<string, string> decryptParameters = null;
            try
            {
                decryptParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptParameters.Count() > 0)
                {
                    RenewalMaintenanceViewModel agreementDetails = maintenanceAgreementDAL.GetRenewalAgreementDetailsDAL(Convert.ToInt32(decryptParameters["IMSPRRoadCode"].ToString()), Convert.ToInt32(decryptParameters["PRContractCode"].ToString()), Convert.ToInt32(decryptParameters["ManeContractId"].ToString()), true);

                    agreementDetails.ContractorName = dbContext.MASTER_CONTRACTOR.Where(c => c.MAST_CON_ID == agreementDetails.MAST_CON_ID).Select(c => c.MAST_CON_COMPANY_NAME).FirstOrDefault();

                    agreementDetails.WorkName = dbContext.IMS_PROPOSAL_WORK.Where(c => c.IMS_WORK_CODE == agreementDetails.IMS_WORK_CODE).Select(c => c.IMS_WORK_DESC).FirstOrDefault();

                    if (agreementDetails == null)
                    {
                        ModelState.AddModelError(string.Empty, "Agreement details not exist.");
                        return PartialView("ViewRenewalAgreementDetails", new RenewalMaintenanceViewModel());
                    }

                    return PartialView("ViewRenewalAgreementDetails", agreementDetails);
                }
                return PartialView("ViewRenewalAgreementDetails", new RenewalMaintenanceViewModel());
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Maintenance.ViewRenewalAgreementDetails()");
                ModelState.AddModelError(String.Empty, "Agreement details not exist.");
                return PartialView("ViewRenewalAgreementDetails", new RenewalMaintenanceViewModel());
            }
        }
        #endregion

        #region Maintenance Work Repackaging

        /// <summary>
        /// view for listing the proposals for repackaging
        /// </summary>
        /// <returns></returns>
        public ActionResult ListMaintenanceProposalForRepackaging()
        {
            try
            {


                CommonFunctions objCommon = new CommonFunctions();
                RepackageDetailsViewModel model = new RepackageDetailsViewModel();
                model.lstBatchs = objCommon.PopulateBatch();
                model.lstBatchs.Find(x => x.Value == "0").Text = "All Batches";
                model.lstYears = new SelectList(objCommon.PopulateFinancialYear(true, true).ToList(), "Value", "Text").ToList();
                model.lstBlocks = objCommon.PopulateBlocks(PMGSYSession.Current.DistrictCode, true);
                model.lstCollaborations = objCommon.PopulateFundingAgency(true);
                List<SelectListItem> lstPackage = new List<SelectListItem>();
                lstPackage.Insert(0, new SelectListItem { Value = "0", Text = "All Packages" });
                model.lstPackages = lstPackage;
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ListMaintenanceProposalForRepackaging()");
                return null;
            }
        }

        /// <summary>
        /// returns the json for listing proposal details
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetMaintenanceProposalListForRepackaging(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 30-Apr-2014 end
                MaintenanceAgreementBAL objMaintenanceBAL = new MaintenanceAgreementBAL();
                long totalRecords = 0;
                int year = 0;
                int block = 0;
                int batch = 0;
                string package = string.Empty;
                int collaboration = 0;
                string proposalType = string.Empty;
                string upgradationType = string.Empty;

                if (!(string.IsNullOrEmpty(Request.Params["YearCode"])))
                {
                    year = Convert.ToInt32(Request.Params["YearCode"]);
                }

                if (!(string.IsNullOrEmpty(Request.Params["BatchCode"])))
                {
                    batch = Convert.ToInt32(Request.Params["BatchCode"]);
                }

                if (!(string.IsNullOrEmpty(Request.Params["BlockCode"])))
                {
                    block = Convert.ToInt32(Request.Params["BlockCode"]);
                }

                if (!(string.IsNullOrEmpty(Request.Params["Package"])))
                {
                    package = Request.Params["Package"];
                }

                if (!(string.IsNullOrEmpty(Request.Params["Collaboration"])))
                {
                    collaboration = Convert.ToInt32(Request.Params["Collaboration"]);
                }

                if (!(string.IsNullOrEmpty(Request.Params["ProposalType"])))
                {
                    proposalType = Request.Params["ProposalType"];
                }

                if (!(string.IsNullOrEmpty(Request.Params["UpgradationType"])))
                {
                    upgradationType = Request.Params["UpgradationType"];
                }

                var jsonData = new
                {
                    rows = objMaintenanceBAL.GetMaintenanceProposalsForRepackaging(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, year, batch, block, package, collaboration, proposalType, upgradationType),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                return jsonResult;

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetMaintenanceProposalListForRepackaging()");
                return null;
            }
        }

        /// <summary>
        /// returns the view for Repackaging
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public ActionResult AddMaintenanceRepackagingDetails()
        {
            Dictionary<string, string> decryptedParameters = null;
            CommonFunctions objCommon = new CommonFunctions();
            MaintenanceAgreementDAL objDAL = new MaintenanceAgreementDAL();
            try
            {
                string id = string.Empty;
                if (!String.IsNullOrEmpty(Request.Params["ProposalCode"]))
                {
                    id = Request.Params["ProposalCode"];
                }
                string[] encryptedParams = id.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedParams[0], encryptedParams[1], encryptedParams[2] });
                int proposalCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);
                PMGSY.Models.MaintenanceAgreement.RepackagingDetailsViewModel model = new PMGSY.Models.MaintenanceAgreement.RepackagingDetailsViewModel();
                model = objDAL.GetMaintenanceRepackagingDetails(proposalCode);
                
                model.EncProposalCode = URLEncrypt.EncryptParameters(new string[] { "ProposalCode=" + decryptedParameters["ProposalCode"] });
                return PartialView(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AddMaintenanceRepackagingDetails()");
                return null;
            }
        }

        /// <summary>
        /// post method for saving the repackaging details
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMaintenanceRepackagingDetails(PMGSY.Models.MaintenanceAgreement.RepackagingDetailsViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    MaintenanceAgreementBAL objBAL = new MaintenanceAgreementBAL();
                    bool status = false;
                    //if (model.StartChainage < model.EndChainage)
                    //{
                    status = objBAL.AddMaintenanceRepackagingDetails(model);
                    if (status == true)
                    {
                        return Json(new { success = true, message = "Repackaging details added successfully." });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Error occurred while processing your request." });
                    }
                    //}
                    //else
                    //{
                    //    return Json(new { success = false, message = "End Chainage should be greater than Start Chainage." });
                    //}

                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AddMaintenanceRepackagingDetails(RepackagingDetailsViewModel model)");
                return Json(new { success = false });
            }
        }

        /// <summary>
        /// populates the maintenance packages based on block , year and batch
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PopulateMaintenancePackagesForRepackaging(string id)
        {
            MaintenanceAgreementDAL objDAL = new MaintenanceAgreementDAL();
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    string[] arrParam = id.Split('$');
                    return Json(objDAL.PopulateMaintenancePackagesForRepackaging(Convert.ToInt32(arrParam[0]), Convert.ToInt32(arrParam[1]), Convert.ToInt32(arrParam[2])));
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PopulatePackagesForRepackaging()");
                return null;
            }
        }
        #endregion

        #region EMARG CORRECTION
        //[Audit]
        //public ActionResult EmargRoadList()
        //{
        //    BalanceSheet balanceSheet = new BalanceSheet();

        //    DAL.MasterDataEntryDAL masterDataEntryDAL = new DAL.MasterDataEntryDAL();
        //    try
        //    {
        //        TransactionParams transactionParams = new TransactionParams();
        //        transactionParams.STATE_CODE = PMGSYSession.Current.StateCode;
        //        transactionParams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
        //        transactionParams.ISSearch = true;
        //        transactionParams.SANC_YEAR = (Int16)DateTime.Now.Year;

        //        using (CommonFunctions commonFunction = new CommonFunctions())
        //        {
        //            ViewData["FinancialYearList"] = commonFunction.PopulateFinancialYear(true, true);
        //            ViewData["BlockList"] = new SelectList(masterDataEntryDAL.GetAllBlocksByDistrictCode(transactionParams.DISTRICT_CODE, true), "MAST_BLOCK_CODE", "MAST_BLOCK_NAME");
        //            ViewData["PackageList"] = new SelectList(commonFunction.GetPackages(Convert.ToInt32(transactionParams.SANC_YEAR), 0, true), "IMS_PACKAGE_ID", "IMS_PACKAGE_ID");
        //            //new filters added by Vikram as per suggested by Dev Sir
        //            ViewData["BatchList"] = commonFunction.PopulateBatch(true);
        //            ViewData["CollaborationList"] = commonFunction.PopulateFundingAgency(true);
        //            ViewData["UpgradationList"] = commonFunction.PopulateNewUpgradeList(true);
        //            //end of change
        //        }

        //        CommonFunctions commonFunctions = new CommonFunctions();

        //        //populate DPIU
        //        List<SelectListItem> lstDpiu = new List<SelectListItem>();
        //        lstDpiu.Insert(0, (new SelectListItem { Text = "Select DPIU", Value = "0" }));
        //        balanceSheet.DPIUList = lstDpiu;
        //        //balanceSheet.NodalAgencyList = commonFunctions.PopulateNodalAgencies();//populate SRRDA
        //        //balanceSheet.NodalAgencyList = commonFunctions.PopulateNodalAgencies(PMGSYSession.Current.StateCode);
        //        //balanceSheet.StateAdminCode = PMGSYSession.Current.AdminNdCode;

        //        //populate SRRDA
        //        if (PMGSYSession.Current.LevelId == 6)// for mrd2
        //        {
        //            balanceSheet.NodalAgencyList = commonFunctions.PopulateNodalAgencies();
        //        }
        //        else if (PMGSYSession.Current.LevelId == 4)//chg
        //        {

        //            balanceSheet.NodalAgencyList = commonFunctions.PopulateNodalAgencies(PMGSYSession.Current.StateCode);
        //            balanceSheet.StateAdminCode = PMGSYSession.Current.AdminNdCode;
        //        }
        //        else if (PMGSYSession.Current.LevelId == 5)//cgbalod
        //        {
        //            balanceSheet.NodalAgencyList = commonFunctions.PopulateNodalAgencies(PMGSYSession.Current.StateCode);
        //            balanceSheet.StateAdminCode = PMGSYSession.Current.ParentNDCode.Value;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "MaintenanceAgreementController().EmargRoadList");
        //        ViewData["FinancialYearList"] = null;
        //        ViewData["PackageList"] = null;
        //        ViewData["BlockList"] = null;
        //    }

        //    return View("EmargRoadList", balanceSheet);

        //}

        [Audit]
        public ActionResult EmargRoadList()
        {
            BalanceSheet balanceSheet = new BalanceSheet();

            DAL.MasterDataEntryDAL masterDataEntryDAL = new DAL.MasterDataEntryDAL();
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
                    ViewData["BlockList"] = new SelectList(masterDataEntryDAL.GetAllBlocksByDistrictCode(transactionParams.DISTRICT_CODE, true), "MAST_BLOCK_CODE", "MAST_BLOCK_NAME");
                    ViewData["PackageList"] = new SelectList(commonFunction.GetPackages(Convert.ToInt32(transactionParams.SANC_YEAR), 0, true), "IMS_PACKAGE_ID", "IMS_PACKAGE_ID");
                    //new filters added by Vikram as per suggested by Dev Sir
                    ViewData["BatchList"] = commonFunction.PopulateBatch(true);
                    ViewData["CollaborationList"] = commonFunction.PopulateFundingAgency(true);
                    ViewData["UpgradationList"] = commonFunction.PopulateNewUpgradeList(true);
                    //end of change

                }

                CommonFunctions commonFunctions = new CommonFunctions();

                //populate DPIU
                List<SelectListItem> lstDpiu = new List<SelectListItem>();
                lstDpiu.Insert(0, (new SelectListItem { Text = "Select DPIU", Value = "0" }));
                balanceSheet.DPIUList = lstDpiu;
                //balanceSheet.NodalAgencyList = commonFunctions.PopulateNodalAgencies();//populate SRRDA
                //balanceSheet.NodalAgencyList = commonFunctions.PopulateNodalAgencies(PMGSYSession.Current.StateCode);
                //balanceSheet.StateAdminCode = PMGSYSession.Current.AdminNdCode;

                //populate SRRDA
                if (PMGSYSession.Current.LevelId == 6)// for mrd2
                {
                    balanceSheet.NodalAgencyList = commonFunctions.PopulateNodalAgencies();
                    balanceSheet.MaintTypeList = commonFunctions.PopulateMaintenanceType();
                }
                else if (PMGSYSession.Current.LevelId == 4)//chg
                {

                    balanceSheet.NodalAgencyList = commonFunctions.PopulateNodalAgencies(PMGSYSession.Current.StateCode);
                    balanceSheet.StateAdminCode = PMGSYSession.Current.AdminNdCode;
                    balanceSheet.MaintTypeList = commonFunctions.PopulateMaintenanceType();

                }
                else if (PMGSYSession.Current.LevelId == 5)//cgbalod
                {
                    balanceSheet.NodalAgencyList = commonFunctions.PopulateNodalAgencies(PMGSYSession.Current.StateCode);
                    balanceSheet.StateAdminCode = PMGSYSession.Current.ParentNDCode.Value;
                    balanceSheet.MaintTypeList = commonFunctions.PopulateMaintenanceType();

                }

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MaintenanceAgreementController().EmargRoadListDLP");
                ViewData["FinancialYearList"] = null;
                ViewData["PackageList"] = null;
                ViewData["BlockList"] = null;
            }

            return View("EmargRoadList", balanceSheet);

        }

        [HttpPost]
        public JsonResult GetEmargFinalList(FormCollection formCollection)
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

                int totalRecords = 0;

                MaintenanceAgreementDAL maintenanceAgreementDAL = new MaintenanceAgreementDAL();
                var jsonData = new
                {
                    rows = maintenanceAgreementDAL.EmargDAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, Convert.ToInt32(formCollection["PIUCode"])),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MaintenananceAgreementController().GetEmargFinalList()");
                return null;
            }
        }



        [HttpGet]
        public ActionResult EditEmarg(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptParameters = null;
            try
            {
                CommonFunctions com = new CommonFunctions();
                MaintenanceAgreementDAL maintenanceAgreementDAL = new MaintenanceAgreementDAL();
                decryptParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                int count = decryptParameters.Count();
                if (count > 0)
                {
                    EmargCorrectionRoadDetails model = maintenanceAgreementDAL.GetEmargDetailsAgainstRoadCode(Convert.ToInt32(decryptParameters["EmargID"].ToString()));

                    if (model == null)
                    {
                        bool status = false;
                        message = message == string.Empty ? "Check Maintenance Agreement Finalization, Check Core Network / Candidate Road for Sanctioned Road, Check Contractor's PAN Details Then Proceed with these Correction Details." : message;
                        //return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

                        ModelState.AddModelError(String.Empty, "Details not exist for selected Road.");
                        return null;

                        //return PartialView("EditEmarg", new EmargCorrectionRoadDetails());
                    }

                    model.TrafficeTypeList = new List<SelectListItem>();
                    model.TrafficeTypeList = com.PopulateTrafficType();

                    model.CarriageWidthList = new List<SelectListItem>();
                    model.CarriageWidthList = com.PopulateCarriageWidth();

                    return PartialView("EditEmarg", model);
                }
                return PartialView("EditEmarg", new EmargCorrectionRoadDetails());
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MaintenanceAgreementController().EditEmarg");
                ModelState.AddModelError(String.Empty, "Details not exist.");
                return PartialView("EditEmarg", new EmargCorrectionRoadDetails());
            }
        }




        [HttpGet]
        public ActionResult ViewEditEmarg(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptParameters = null;
            try
            {
                CommonFunctions com = new CommonFunctions();
                MaintenanceAgreementDAL maintenanceAgreementDAL = new MaintenanceAgreementDAL();
                decryptParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                PMGSYEntities dbContext = new PMGSYEntities();
                int count = decryptParameters.Count();
                if (count > 0)
                {
                    EmargCorrectionRoadDetails model = maintenanceAgreementDAL.GetEmargDetailsAgainstRoadCode(Convert.ToInt32(decryptParameters["EmargID"].ToString()));

                    if (model == null)
                    {
                        bool status = false;
                        message = message == string.Empty ? "Details are not available for selected Road." : message;
                        //return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

                        ModelState.AddModelError(String.Empty, "Details not exist for selected Road.");
                        return null;

                        //return PartialView("EditEmarg", new EmargCorrectionRoadDetails());
                    }

                    model.TrafficeTypeCodeText = dbContext.MASTER_TRAFFIC_TYPE.Where(m => m.MAST_TRAFFIC_CODE == model.TrafficeTypeCode).Select(m => m.MAST_TRAFFIC_NAME).FirstOrDefault();
                    model.CarriageWidthCodeText = dbContext.MASTER_CARRIAGE.Where(m => m.MAST_CARRIAGE_CODE == model.CarriageWidthCode).Select(m => m.MAST_CARRIAGE_WIDTH).FirstOrDefault();


                    model.TrafficeTypeList = new List<SelectListItem>();
                    model.TrafficeTypeList = com.PopulateTrafficType();

                    model.CarriageWidthList = new List<SelectListItem>();
                    model.CarriageWidthList = com.PopulateCarriageWidth();

                    return PartialView("ViewEditEmarg", model);
                }
                return PartialView("ViewEditEmarg", new EmargCorrectionRoadDetails());
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MaintenanceAgreementController().ViewEditEmarg");
                ModelState.AddModelError(String.Empty, "Details not exist.");
                return PartialView("ViewEditEmarg", new EmargCorrectionRoadDetails());
            }
        }



        [HttpPost]
        public ActionResult UpdateEmargDetails(EmargCorrectionRoadDetails model)
        {
            bool status = false;

            try
            {
                if (ModelState.IsValid)
                {
                    MaintenanceAgreementDAL maintenanceAgreementDAL = new MaintenanceAgreementDAL();
                    if (maintenanceAgreementDAL.UpdateEmargDAL(model, ref message))
                    {
                        message = message == string.Empty ? "Details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Details not updated.." : message;
                    }
                }
                else
                {
                    return PartialView("EditEmarg", model);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MaintenanceAgreementController().UpdateEmargDetails");
                message = message == string.Empty ? "Details are not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        #region POST DLP

        [HttpPost]
        public JsonResult GetEmargDLPFinalList(FormCollection formCollection)
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

                int totalRecords = 0;

                MaintenanceAgreementDAL maintenanceAgreementDAL = new MaintenanceAgreementDAL();
                var jsonData = new
                {
                    rows = maintenanceAgreementDAL.EmargDLPDAL(Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords, Convert.ToInt32(formCollection["PIUCode"]), Convert.ToInt32(formCollection["MaintTypeCode"])),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MaintenananceAgreementController().GetEmargDLPFinalList()");
                return null;
            }
        }

        [HttpGet]
        public ActionResult EditEmargPostDLP(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptParameters = null;
            try
            {
                CommonFunctions com = new CommonFunctions();
                MaintenanceAgreementDAL maintenanceAgreementDAL = new MaintenanceAgreementDAL();
                decryptParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                int count = decryptParameters.Count();
                if (count > 0)
                {
                    EmargCorrectionRoadDetailsPostDLP model = maintenanceAgreementDAL.GetEmargDetailsAgainstRoadCodePostDLP(Convert.ToInt32(decryptParameters["EmargID"].ToString()));

                    if (model == null)
                    {
                        bool status = false;
                        message = message == string.Empty ? "Check Maintenance Agreement Finalization, Check Core Network / Candidate Road for Sanctioned Road, Check Contractor's PAN Details Then Proceed with these Correction Details." : message;
                        //return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

                        ModelState.AddModelError(String.Empty, "Details not exist for selected Road.");
                        return null;

                        //return PartialView("EditEmarg", new EmargCorrectionRoadDetails());
                    }

                    model.TrafficeTypeList = new List<SelectListItem>();
                    model.TrafficeTypeList = com.PopulateTrafficType();

                    model.CarriageWidthList = new List<SelectListItem>();
                    model.CarriageWidthList = com.PopulateCarriageWidth();

                    return PartialView("EditEmargPostDLP", model);
                }
                return PartialView("EditEmargPostDLP", new EmargCorrectionRoadDetailsPostDLP());
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MaintenanceAgreementController().EditEmargPostDLP");
                ModelState.AddModelError(String.Empty, "Details not exist.");
                return PartialView("EditEmargPostDLP", new EmargCorrectionRoadDetailsPostDLP());
            }
        }

        [HttpPost]
        public ActionResult UpdateEmargDetailsPostDLP(EmargCorrectionRoadDetailsPostDLP model)
        {
            bool status = false;

            try
            {


                if (ModelState.IsValid)
                {
                    MaintenanceAgreementDAL maintenanceAgreementDAL = new MaintenanceAgreementDAL();
                    if (maintenanceAgreementDAL.UpdateEmargPostDLPDAL(model, ref message))
                    {
                        message = message == string.Empty ? "Details updated successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Details not updated.." : message;
                    }
                }
                else
                {
                    return PartialView("EditEmargPostDLP", model);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MaintenanceAgreementController().UpdateEmargDetailsPostDLP");
                message = message == string.Empty ? "Details are not updated." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult FinalizeRoadAfterEmargCorrectionPostDLP(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptParameters = null;
            bool status = false;
            message = "Road Details not finalized.";
            try
            {
                MaintenanceAgreementDAL maintenanceAgreementDAL = new MaintenanceAgreementDAL();
                decryptParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                int count = decryptParameters.Count();


                if (count > 0)
                {
                    int EmargID = Convert.ToInt32(decryptParameters["EmargID"].ToString());

                    if (maintenanceAgreementDAL.FinalizeEmargRoadPostDLPDAL(EmargID, out message))
                    {
                        message = "Road Details finalized successfully.";
                        status = true;
                    }
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MaintenanceAgreementController().FinalizeRoadAfterEmargCorrectionPostDLP");
                return Json(new { success = false, message = "Road Details are not finalized." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult PushToEmargDetailsPostDLP()
        {
            // Dictionary<string, string> decryptParameters = null;
            bool status = false;
            message = "Package Details not sent to Emarg.";
            try
            {
                MaintenanceAgreementDAL maintenanceAgreementDAL = new MaintenanceAgreementDAL();

                var EmargId = Convert.ToString(Request.Params["Data"]);

                if (maintenanceAgreementDAL.PushPackageToEmargPostDLP(Convert.ToInt32(EmargId), out message))
                {
                    message = "Package Details pushed to Emarg Successfully.";
                    status = true;
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MaintenanceAgreementController().PushToEmargDetailsPostDLP");
                return Json(new { success = false, message = "Package Details not sent to Emarg." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult ViewEditEmargPostDLP(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptParameters = null;
            try
            {
                CommonFunctions com = new CommonFunctions();
                MaintenanceAgreementDAL maintenanceAgreementDAL = new MaintenanceAgreementDAL();
                decryptParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                PMGSYEntities dbContext = new PMGSYEntities();
                int count = decryptParameters.Count();
                if (count > 0)
                {
                    EmargCorrectionRoadDetailsPostDLP model = maintenanceAgreementDAL.GetEmargDetailsAgainstRoadCodePostDLP(Convert.ToInt32(decryptParameters["EmargID"].ToString()));

                    if (model == null)
                    {
                        bool status = false;
                        message = message == string.Empty ? "Details are not available for selected Road." : message;
                        //return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

                        ModelState.AddModelError(String.Empty, "Details not exist for selected Road.");
                        return null;

                        //return PartialView("EditEmarg", new EmargCorrectionRoadDetails());
                    }

                    model.TrafficeTypeCodeText = dbContext.MASTER_TRAFFIC_TYPE.Where(m => m.MAST_TRAFFIC_CODE == model.TrafficeTypeCode).Select(m => m.MAST_TRAFFIC_NAME).FirstOrDefault();
                    model.CarriageWidthCodeText = dbContext.MASTER_CARRIAGE.Where(m => m.MAST_CARRIAGE_CODE == model.CarriageWidthCode).Select(m => m.MAST_CARRIAGE_WIDTH).FirstOrDefault();


                    model.TrafficeTypeList = new List<SelectListItem>();
                    model.TrafficeTypeList = com.PopulateTrafficType();

                    model.CarriageWidthList = new List<SelectListItem>();
                    model.CarriageWidthList = com.PopulateCarriageWidth();

                    return PartialView("ViewEditEmargPostDLP", model);
                }
                return PartialView("ViewEditEmargPostDLP", new EmargCorrectionRoadDetailsPostDLP());
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MaintenanceAgreementController().ViewEditEmargPostDLP");
                ModelState.AddModelError(String.Empty, "Details not exist.");
                return PartialView("ViewEditEmargPostDLP", new EmargCorrectionRoadDetailsPostDLP());
            }
        }

        #endregion

        [HttpPost]
        [Audit]
        public ActionResult FinalizeRoadAfterEmargCorrection(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptParameters = null;
            bool status = false;
            message = "Road Details not finalized.";
            try
            {
                MaintenanceAgreementDAL maintenanceAgreementDAL = new MaintenanceAgreementDAL();
                decryptParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                int count = decryptParameters.Count();


                if (count > 0)
                {
                    int EmargID = Convert.ToInt32(decryptParameters["EmargID"].ToString());

                    if (maintenanceAgreementDAL.FinalizeEmargRoadDAL(EmargID, out message))
                    {
                        message = "Road Details finalized successfully.";
                        status = true;
                    }
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MaintenanceAgreementController().FinalizeRoadAfterEmargCorrection");
                return Json(new { success = false, message = "Road Details are not finalized." }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        [Audit]
        public ActionResult FinalizePackageEmargCorrection()
        {
            bool status = false;
            message = "Package Details not finalized. All roads in this package must be finalized first to finalize this package.";
            try
            {
                MaintenanceAgreementDAL maintenanceAgreementDAL = new MaintenanceAgreementDAL();

                var PackageID = Convert.ToString(Request.Params["Data"]);

                if (maintenanceAgreementDAL.PackageFinalizeEmargRoadDAL(PackageID, out message))
                {
                    message = "Package Details finalized successfully.";
                    status = true;
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MaintenanceAgreementController().FinalizeRoadAfterEmargCorrection");
                return Json(new { success = false, message = "Package Details are not finalized." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult DeleteRoadBeforePackageFinalization(String parameter, String hash, String key)
        {
            MaintenanceAgreementDAL maintenanceAgreementDAL = new MaintenanceAgreementDAL();
            string message = string.Empty;
            Dictionary<string, string> decryptedParameters = null;
            int EmargID = 0;
            string progressType = string.Empty;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                EmargID = Convert.ToInt32(decryptedParameters["EmargID"]);

                //progressType = decryptedParameters["Type"];
                if ((maintenanceAgreementDAL.DeleteRoadDetailsBeforePackageFinalizationDAL(EmargID, ref message)))
                {
                    if (message == string.Empty)
                    {
                        return Json(new { success = true, message = "Road details deleted successfully." });
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


        [HttpPost]
        [Audit]
        public ActionResult PushToEmargDetails()
        {
            // Dictionary<string, string> decryptParameters = null;
            bool status = false;
            message = "Package Details not sent to Emarg.";
            try
            {
                MaintenanceAgreementDAL maintenanceAgreementDAL = new MaintenanceAgreementDAL();

                var PackageID = Convert.ToString(Request.Params["Data"]);

                if (maintenanceAgreementDAL.PushPackageToEmarg(PackageID, out message))
                {
                    message = "Package Details pushed to Emarg Successfully.";
                    status = true;
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MaintenanceAgreementController().PushToEmargDetails");
                return Json(new { success = false, message = "Package Details not sent to Emarg." }, JsonRequestBehavior.AllowGet);
            }
        }



        #endregion


        #region Emarg Work Repackaging

        public ActionResult DistrictDetailsHere(FormCollection frmCollection)
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            List<SelectListItem> list = objCommonFunctions.PopulateDistrict(Convert.ToInt32(frmCollection["StateCode"]), false);
          //  list.Find(x => x.Value == "-1").Value = "0";
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// view for listing the proposals for repackaging
        /// </summary>
        /// <returns></returns>
        public ActionResult ListEmargMaintenanceProposalForRepackaging()
        {
            try
            {


                CommonFunctions objCommon = new CommonFunctions();
                RepackageDetailsViewModel model = new RepackageDetailsViewModel();


                CommonFunctions commonFunctions = new CommonFunctions();

                //if (PMGSYSession.Current.RoleCode == 25)
                //{
                //    model.StateList = commonFunctions.PopulateStates(true);
                //}
                //if (PMGSYSession.Current.RoleCode == 22)
                //{
                //    model.mast_stat = PMGSYSession.Current.StateCode;
                //}
                model.StateName = PMGSYSession.Current.StateCode == 0 ? "0" : PMGSYSession.Current.StateName.Trim();

                model.DistName = PMGSYSession.Current.DistrictCode == 0 ? "0" : PMGSYSession.Current.DistrictName.Trim();

                model.Mast_State_Code = PMGSYSession.Current.StateCode;

                model.Mast_District_Code = PMGSYSession.Current.DistrictCode;
                model.Block = PMGSYSession.Current.DistrictCode; // This is Dist Code




                model.StateList = commonFunctions.PopulateStates(true);


                model.StateList.Find(x => x.Value == model.StateCode.ToString()).Selected = true;

                model.lstBlocks = new List<SelectListItem>();
                if (model.StateCode == 0 && PMGSYSession.Current.RoleCode != 2)
                {
                    model.lstBlocks.Insert(0, (new SelectListItem { Text = "All Districts", Value = "0", Selected = true }));
                }
                else
                {
                    model.lstBlocks = commonFunctions.PopulateDistrictForSRRDA(PMGSYSession.Current.StateCode, true);

                    // objCommon.PopulateDistrict(PMGSYSession.Current.StateCode, false);
                }


                model.lstBatchs = objCommon.PopulateBatch();
                model.lstBatchs.Find(x => x.Value == "0").Text = "All Batches";


                model.lstYears = new SelectList(objCommon.PopulateFinancialYear(true, true).ToList(), "Value", "Text").ToList();

                //if (PMGSYSession.Current.RoleCode == 22)
                //{
                //    model.lstBlocks = objCommon.PopulateDistrict(PMGSYSession.Current.StateCode, false);
                //}


                //if (PMGSYSession.Current.RoleCode == 22)
                //{
                //    model.Block = PMGSYSession.Current.DistrictCode;
                //}

                model.lstCollaborations = objCommon.PopulateFundingAgency(true);

                List<SelectListItem> lstPackage = new List<SelectListItem>();
                lstPackage.Insert(0, new SelectListItem { Value = "0", Text = "All Packages" });
                model.lstPackages = lstPackage;
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ListEmargMaintenanceProposalForRepackaging()");
                return null;
            }
        }

        /// <summary>
        /// returns the json for listing proposal details
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetEmargMaintenanceProposalListForRepackaging(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
             
                MaintenanceAgreementDAL maintenanceAgreementDAL = new MaintenanceAgreementDAL();
                long totalRecords = 0;
                int StateCode = 0;
                int block = 0;
                int batch = 0;
                string package = string.Empty;
                int collaboration = 0;
                string proposalType = string.Empty;
                string upgradationType = string.Empty;

                if (!(string.IsNullOrEmpty(Request.Params["StateCode"])))
                {
                    StateCode = Convert.ToInt32(Request.Params["StateCode"]);
                }

                if (!(string.IsNullOrEmpty(Request.Params["BatchCode"])))
                {
                    batch = Convert.ToInt32(Request.Params["BatchCode"]);
                }

                if (!(string.IsNullOrEmpty(Request.Params["BlockCode"])))
                {
                    block = Convert.ToInt32(Request.Params["BlockCode"]);
                }

                if (!(string.IsNullOrEmpty(Request.Params["Package"])))
                {
                    package = Request.Params["Package"];
                }

                if (!(string.IsNullOrEmpty(Request.Params["Collaboration"])))
                {
                    collaboration = Convert.ToInt32(Request.Params["Collaboration"]);
                }

                if (!(string.IsNullOrEmpty(Request.Params["ProposalType"])))
                {
                    proposalType = Request.Params["ProposalType"];
                }

                if (!(string.IsNullOrEmpty(Request.Params["UpgradationType"])))
                {
                    upgradationType = Request.Params["UpgradationType"];
                }

                var jsonData = new
                {
                    rows = maintenanceAgreementDAL.GetEmargMaintenanceProposalsForRepackaging(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords, StateCode, batch, block, package, collaboration, proposalType, upgradationType),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                return jsonResult;

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetEmargMaintenanceProposalListForRepackaging()");
                return null;
            }
        }

        /// <summary>
        /// returns the view for Repackaging
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public ActionResult AddEmargMaintenanceRepackagingDetails()
        {
            Dictionary<string, string> decryptedParameters = null;
            CommonFunctions objCommon = new CommonFunctions();
            MaintenanceAgreementDAL objDAL = new MaintenanceAgreementDAL();
            try
            {
                string id = string.Empty;
                if (!String.IsNullOrEmpty(Request.Params["ProposalCode"]))
                {
                    id = Request.Params["ProposalCode"];
                }
                string[] encryptedParams = id.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedParams[0], encryptedParams[1], encryptedParams[2] });
                int proposalCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);
                PMGSY.Models.MaintenanceAgreement.EmargRepackage model = new PMGSY.Models.MaintenanceAgreement.EmargRepackage();
                model = objDAL.GetEmargMaintenanceRepackagingDetails(proposalCode);

                model.EncProposalCode = URLEncrypt.EncryptParameters(new string[] { "ProposalCode=" + decryptedParameters["ProposalCode"] });
                return PartialView(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AddEmargMaintenanceRepackagingDetails()");
                return null;
            }
        }

        /// <summary>
        /// post method for saving the repackaging details
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddEmargMaintenanceRepackagingDetails(PMGSY.Models.MaintenanceAgreement.EmargRepackage model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    MaintenanceAgreementDAL maintenanceAgreementDAL = new MaintenanceAgreementDAL();

                    bool status = false;
                    string message = "Repackaging details not finalized.";
                    status = maintenanceAgreementDAL.AddEmargMaintenanceRepackagingDetails(model, out message);
                    if (status == true)
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
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AddEmargMaintenanceRepackagingDetails(EmargRepackage model)");
                return Json(new { success = false });
            }
        }

        /// <summary>
        /// populates the maintenance packages based on block , year and batch
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PopulateEmargMaintenancePackagesForRepackaging(string id)
        {
            MaintenanceAgreementDAL objDAL = new MaintenanceAgreementDAL();
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    string[] arrParam = id.Split('$');
                    return Json(objDAL.PopulateEmargMaintenancePackagesForRepackagingDAL(Convert.ToInt32(arrParam[0]), Convert.ToInt32(arrParam[1]), Convert.ToInt32(arrParam[2])));
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PopulateEmargMaintenancePackagesForRepackaging()");
                return null;
            }
        }


        [HttpPost]
        public ActionResult FinalizeRepakageForEmarg(String parameter, String hash, String key)
        {
            bool status = false;
            message = "Repackaged Road details are not finalized.";
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {

                    MaintenanceAgreementDAL maintenanceAgreementDAL = new MaintenanceAgreementDAL();

                    Int32 ProposalCode = Convert.ToInt32(decryptedParameters["ProposalCode"]);


                    if (maintenanceAgreementDAL.FinalizeEmargRepackageRoadDAL(ProposalCode, out message))
                    {
                        message = "Repackaged Road details are finalized Successfully.";
                        status = true;
                    }
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                //message = message = "Agreement not finalized successfully.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult FinalizePackageEmargForRepackaing()
        {
            bool status = false;
            message = "Package Details not finalized. All roads in this package must be finalized first to finalize this package.";
            try
            {
                MaintenanceAgreementDAL maintenanceAgreementDAL = new MaintenanceAgreementDAL();

                var PackageID = Convert.ToString(Request.Params["Data"]);

                if (maintenanceAgreementDAL.PackageFinalizeEmargRepackageRoadDAL(PackageID, out message))
                {
                    message = "Package Details finalized successfully.";
                    status = true;
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MaintenanceAgreementController().FinalizePackageEmargForRepackaing");
                return Json(new { success = false, message = "Package Details are not finalized." }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion


        #region Emarg Work Repackage at ITNO

        [Audit]
        public ActionResult MordEmargRoadList()
        {
            DAL.MasterDataEntryDAL masterDataEntryDAL = new DAL.MasterDataEntryDAL();
            try
            {
                PMGSYEntities dbContext = new PMGSYEntities();
                MordListEmarg model = new MordListEmarg();
                CommonFunctions commonFunctions = new CommonFunctions();
                model.StateList = commonFunctions.PopulateStates(true);
                model.StateName = PMGSYSession.Current.StateName;

                model.Mast_State_Code = PMGSYSession.Current.StateCode;

                model.StateCode = PMGSYSession.Current.StateCode;
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MaintenanceAgreementController().MordEmargRoadList");

                return null;
            }


        }


        [Audit]
        public ActionResult MordEmargRoadListTwo(int stateCode)
        {
            DAL.MasterDataEntryDAL masterDataEntryDAL = new DAL.MasterDataEntryDAL();
            try
            {
                PMGSYEntities dbContext = new PMGSYEntities();

                MordListEmarg model = new MordListEmarg();

                CommonFunctions commonFunctions = new CommonFunctions();

                model.StateList = commonFunctions.PopulateStates(true);
                model.StateCode = stateCode;

                return View(model);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MaintenanceAgreementController().MordEmargRoadListTwo");

                return null;
            }
        }




        [HttpPost]
        public JsonResult MordGetEmargFinalList(FormCollection formCollection)
        {
            var JsonSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            JsonSerializer.MaxJsonLength = Int32.MaxValue;
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

             //   Int32 StateCode = Convert.ToInt32(formCollection["IMS_STATE"]);
                string DistListFinal = formCollection["DistListID"];

                int totalRecords = 0;

                MaintenanceAgreementDAL maintenanceAgreementDAL = new MaintenanceAgreementDAL();
                var jsonData = new
                {
                    rows = maintenanceAgreementDAL.MordEmargDAL(DistListFinal, Convert.ToInt32(formCollection["page"]) - 1, Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : totalRecords / Convert.ToInt32(formCollection["rows"]) + 1,
                    page = Convert.ToInt32(formCollection["page"]),
                    records = totalRecords
                };

                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "MaintenananceAgreementController().GetEmargFinalList()");
                return null;
            }
        }



        #endregion

        // Created on 12-04-2022 by Srishti Tyagi
        // Created to add the details of Emarg data into MANE_IMS_CONTRACT table 

        #region Terminated Package Agreements 

        public ActionResult TerminatedPackageAgreement()
        {
            try
            {
                PMGSYEntities dbContext = new PMGSYEntities();
                CommonFunctions objCommon = new CommonFunctions();

                TerminatedPackageModel packageDetails = new TerminatedPackageModel();

                packageDetails.StateName = PMGSYSession.Current.StateName;
                packageDetails.stateCode = PMGSYSession.Current.StateCode;
                packageDetails.DistrictName = PMGSYSession.Current.DistrictName;
                packageDetails.districtCode = PMGSYSession.Current.DistrictCode;
                packageDetails.lstBlock = new List<SelectListItem>();
                packageDetails.lstBlock = objCommon.PopulateBlocks(PMGSYSession.Current.DistrictCode, false);
                packageDetails.Years = new List<SelectListItem>();
                packageDetails.Years = PopulateYear(0, true, true);

                return View(packageDetails);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "TerminatedPackageAgreement()");
                return null;
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

        [Audit]
        public ActionResult GetTerminatedPackageList(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                int stateCode = 0;
                int yearCode = 0;
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

                var jsonData = new
                {
                    rows = maintenanceAgreementBAL.GetTerminatedPackageListBAL(stateCode, yearCode, districtCode, blockCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : totalRecords / Convert.ToInt32(rows) + 1,
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetTerminatedPackageList()");
                return null;
            }
        }

        [Audit]
        public ActionResult AddContractorDetails(String parameter, String hash, String key)
        {
            MaintenanceAgreementDetails maintenenaceAgreementDetails = new MaintenanceAgreementDetails();
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });


                if (decryptedParameters.Count > 0)
                {
                    dbContext = new PMGSYEntities();

                    maintenenaceAgreementDetails.EncryptedIMSPRRoadCode = parameter + '/' + hash + '/' + key;
                    maintenenaceAgreementDetails.IsNewContractor = false;

                    ViewBag.SanctionedYear = decryptedParameters["SanctionedYear"].ToString();
                    ViewBag.Package = decryptedParameters["Package"].ToString().Replace("--", "/");
                    ViewBag.RoadLength = decryptedParameters["RoadLength"].ToString().Replace("--", ".");
                    string imsPackage = decryptedParameters["Package"].ToString().Replace("--", "/");
                    int prRoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"]);

                    ViewBag.RoadName = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PACKAGE_ID == imsPackage && x.IMS_PR_ROAD_CODE == prRoadCode).Select(s => s.IMS_ROAD_NAME).FirstOrDefault();

                    return PartialView("AddContractorDetails", maintenenaceAgreementDetails);
                }
                return PartialView("AddContractorDetails", maintenenaceAgreementDetails);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return PartialView("AddContractorDetails", maintenenaceAgreementDetails);
            }
        }

        [Audit]
        public ActionResult MaintenanceContractorDetails(String parameter, String hash, String key)
        {
            string roadLsb = string.Empty;
            int IMSPRRoadCode = 0;
            int conCount = 0;
            int maxConId = 0;
            string conPan;
            string fname, mname, lname;
            System.DateTime constructionDate;
            TerminatedAgreementDetails maintenenaceAgreementDetails = new TerminatedAgreementDetails();
            EmargRoadWiseBalanceWorks maintenenaceDetails = new EmargRoadWiseBalanceWorks();
            maintenenaceAgreementDetails.EncryptedIMSPRRoadCode = parameter + "/" + hash + "/" + key;

            decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

            IMSPRRoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"].ToString());

            maintenenaceDetails = dbContext.EMARG_ROAD_WISE_BALANCE_WORK_PACKAGE_DETAILS.Where(x => x.ROAD_CODE == IMSPRRoadCode).Select(x => new EmargRoadWiseBalanceWorks()
            {
                CONTRACTOR_PAN = x.CONTRACTOR_PAN,
                RECORD_ID = x.RECORD_ID,
                AGREEMENT_NUMBER = x.AGREEMENT_NO,
                AGREEMENT_DATE = x.AGREEMENT_DATE,
                MAINTENANCE_START_DATE = x.MAINTENANCE_START_DATE,
                MAINTENANCE_END_DATE = x.MAINTENANCE_END_DATE
            }).FirstOrDefault();

            conPan = dbContext.MASTER_CONTRACTOR.Where(x => x.MAST_CON_PAN == maintenenaceDetails.CONTRACTOR_PAN).Select(y => y.MAST_CON_PAN).FirstOrDefault();

            if (conPan == null)
            {
                maintenenaceAgreementDetails.ContractorPAN = null;
                maintenenaceAgreementDetails.ContractorName = null;

                ViewBag.PanNumber = maintenenaceDetails.CONTRACTOR_PAN;
            }
            else
            {
                maintenenaceAgreementDetails.ContractorPAN = conPan;

                conCount = dbContext.MASTER_CONTRACTOR.Where(x => x.MAST_CON_PAN == maintenenaceDetails.CONTRACTOR_PAN).Count();

                if (conCount > 1)
                {
                    maxConId = dbContext.MASTER_CONTRACTOR.Where(x => x.MAST_CON_PAN == maintenenaceDetails.CONTRACTOR_PAN).Max(x => x.MAST_CON_ID);

                    fname = dbContext.MASTER_CONTRACTOR.Where(x => x.MAST_CON_ID == maxConId).Select(y => y.MAST_CON_FNAME).FirstOrDefault();
                    mname = dbContext.MASTER_CONTRACTOR.Where(x => x.MAST_CON_ID == maxConId).Select(y => y.MAST_CON_MNAME == null ? "" : y.MAST_CON_MNAME).FirstOrDefault();
                    lname = dbContext.MASTER_CONTRACTOR.Where(x => x.MAST_CON_ID == maxConId).Select(y => y.MAST_CON_LNAME == null ? "" : y.MAST_CON_LNAME).FirstOrDefault();

                    maintenenaceAgreementDetails.ContractorName = fname + " " + mname + " " + lname;
                }
                else
                {
                    fname = dbContext.MASTER_CONTRACTOR.Where(x => x.MAST_CON_PAN == maintenenaceDetails.CONTRACTOR_PAN).Select(y => y.MAST_CON_FNAME).FirstOrDefault();
                    mname = dbContext.MASTER_CONTRACTOR.Where(x => x.MAST_CON_PAN == maintenenaceDetails.CONTRACTOR_PAN).Select(y => y.MAST_CON_MNAME == null ? "" : y.MAST_CON_MNAME).FirstOrDefault();
                    lname = dbContext.MASTER_CONTRACTOR.Where(x => x.MAST_CON_PAN == maintenenaceDetails.CONTRACTOR_PAN).Select(y => y.MAST_CON_LNAME == null ? "" : y.MAST_CON_LNAME).FirstOrDefault();

                    maintenenaceAgreementDetails.ContractorName = fname + " " + mname + " " + lname;
                }
            }

            constructionDate = (DateTime)dbContext.EXEC_ROADS_MONTHLY_STATUS.Where(x => x.IMS_PR_ROAD_CODE == IMSPRRoadCode && x.EXEC_ISCOMPLETED == "C").Select(y => y.EXEC_COMPLETION_DATE).FirstOrDefault();
            maintenenaceAgreementDetails.MANE_CONSTR_COMP_DATE = constructionDate.ToShortDateString();

            maintenenaceAgreementDetails.MANE_AGREEMENT_NUMBER = maintenenaceDetails.AGREEMENT_NUMBER;
            maintenenaceAgreementDetails.MANE_AGREEMENT_DATE = maintenenaceDetails.AGREEMENT_DATE.ToShortDateString();
            maintenenaceAgreementDetails.MANE_MAINTENANCE_START_DATE = maintenenaceDetails.MAINTENANCE_START_DATE.ToShortDateString();
            maintenenaceAgreementDetails.MANE_MAINTENANCE_END_DATE = maintenenaceDetails.MAINTENANCE_END_DATE.ToShortDateString();
            maintenenaceAgreementDetails.recordId = maintenenaceDetails.RECORD_ID;

            return PartialView("FormToFillDetails", maintenenaceAgreementDetails);
        }

        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult SaveContractorDetails(TerminatedAgreementDetails details_agreement)
        {
            bool status = false;
            try
            {
                if (ModelState.IsValid)
                {

                    if (maintenanceAgreementBAL.SaveContractorDetailsBAL(details_agreement, ref message))
                    {
                        message = message == string.Empty ? "Details added successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Details not added successfully." : message;
                    }

                }
                else
                {
                    return PartialView("FormToFillDetails", details_agreement);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Details not added successfully.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [Audit]
        public ActionResult ViewContractorDetails(String parameter, String hash, String key)
        {
            MANE_IMS_CONTRACT model = null;
            TerminatedAgreementDetails maintenenaceAgreementDetails = new TerminatedAgreementDetails();

            string fname, mname, lname;

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });


                if (decryptedParameters.Count > 0)
                {
                    dbContext = new PMGSYEntities();

                    int prRoadCode = Convert.ToInt32(decryptedParameters["imsRoadID"]);
                    string agreementNo = decryptedParameters["AggNo"];
                    string panNumber = decryptedParameters["PanNumber"];

                    ViewBag.SanctionedYear = decryptedParameters["SanctionedYear"].ToString();
                    ViewBag.Package = decryptedParameters["Package"].ToString().Replace("--", "/");
                    ViewBag.RoadLength = decryptedParameters["RoadLength"].ToString().Replace("--", ".");
                    string imsPackage = decryptedParameters["Package"].ToString().Replace("--", "/");

                    ViewBag.RoadName = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PACKAGE_ID == imsPackage && x.IMS_PR_ROAD_CODE == prRoadCode).Select(s => s.IMS_ROAD_NAME).FirstOrDefault();

                    model = dbContext.MANE_IMS_CONTRACT.Where(x => x.IMS_PR_ROAD_CODE == prRoadCode && x.MANE_AGREEMENT_NUMBER == agreementNo).FirstOrDefault();

                    fname = dbContext.MASTER_CONTRACTOR.Where(x => x.MAST_CON_PAN == panNumber).Select(y => y.MAST_CON_FNAME).FirstOrDefault();
                    mname = dbContext.MASTER_CONTRACTOR.Where(x => x.MAST_CON_PAN == panNumber).Select(y => y.MAST_CON_MNAME == null ? "" : y.MAST_CON_MNAME).FirstOrDefault();
                    lname = dbContext.MASTER_CONTRACTOR.Where(x => x.MAST_CON_PAN == panNumber).Select(y => y.MAST_CON_LNAME == null ? "" : y.MAST_CON_LNAME).FirstOrDefault();

                    maintenenaceAgreementDetails.ContractorName = fname + " " + mname + " " + lname;
                    maintenenaceAgreementDetails.ContractorPAN = panNumber;
                    maintenenaceAgreementDetails.MANE_AGREEMENT_NUMBER = model.MANE_AGREEMENT_NUMBER;
                    maintenenaceAgreementDetails.MANE_AGREEMENT_DATE = model.MANE_AGREEMENT_DATE.ToShortDateString();
                    maintenenaceAgreementDetails.MANE_MAINTENANCE_START_DATE = model.MANE_MAINTENANCE_START_DATE.ToShortDateString();
                    maintenenaceAgreementDetails.MANE_MAINTENANCE_END_DATE = model.MANE_MAINTENANCE_END_DATE.ToString() == null ? "--" : model.MANE_MAINTENANCE_END_DATE.ToString().TrimEnd();
                    maintenenaceAgreementDetails.MANE_CONSTR_COMP_DATE = model.MANE_CONSTR_COMP_DATE.ToShortDateString();
                    maintenenaceAgreementDetails.MANE_YEAR1_AMOUNT = model.MANE_YEAR1_AMOUNT;
                    maintenenaceAgreementDetails.MANE_YEAR2_AMOUNT = model.MANE_YEAR2_AMOUNT;
                    maintenenaceAgreementDetails.MANE_YEAR3_AMOUNT = model.MANE_YEAR3_AMOUNT;
                    maintenenaceAgreementDetails.MANE_YEAR4_AMOUNT = model.MANE_YEAR4_AMOUNT;
                    maintenenaceAgreementDetails.MANE_YEAR5_AMOUNT = model.MANE_YEAR5_AMOUNT;
                    maintenenaceAgreementDetails.MANE_HANDOVER_DATE = model.MANE_HANDOVER_DATE.ToString() == null ? "--" : model.MANE_HANDOVER_DATE.ToString();
                    maintenenaceAgreementDetails.MANE_HANDOVER_TO = model.MANE_HANDOVER_TO == null || model.MANE_HANDOVER_TO == string.Empty ? "--" : model.MANE_HANDOVER_TO;

                    ViewBag.MANE_YEAR1_AMOUNT = model.MANE_YEAR1_AMOUNT;
                    ViewBag.MANE_YEAR2_AMOUNT = model.MANE_YEAR2_AMOUNT;
                    ViewBag.MANE_YEAR3_AMOUNT = model.MANE_YEAR3_AMOUNT;
                    ViewBag.MANE_YEAR4_AMOUNT = model.MANE_YEAR4_AMOUNT;
                    ViewBag.MANE_YEAR5_AMOUNT = model.MANE_YEAR5_AMOUNT;

                    return PartialView("ViewContractorDetails", maintenenaceAgreementDetails);
                }
                return PartialView("ViewContractorDetails", maintenenaceAgreementDetails);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return PartialView("ViewContractorDetails", maintenenaceAgreementDetails);
            }
        }

        public ActionResult FinalizeDetails(String parameter, String hash, String key)
        {
            try
            {
                PMGSYEntities dbContext = new PMGSYEntities();
                EMARG_ROAD_WISE_BALANCE_WORK_PACKAGE_DETAILS model = new EMARG_ROAD_WISE_BALANCE_WORK_PACKAGE_DETAILS();
                MANE_IMS_CONTRACT contractModel = new MANE_IMS_CONTRACT();

                string message = string.Empty;
                bool status = false;
                int recordId = 0;

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    recordId = Convert.ToInt32(decryptedParameters["recordId"]);
                }

                model = dbContext.EMARG_ROAD_WISE_BALANCE_WORK_PACKAGE_DETAILS.Where(x => x.RECORD_ID == recordId).FirstOrDefault<EMARG_ROAD_WISE_BALANCE_WORK_PACKAGE_DETAILS>();
                model.DATA_FLAG = "Y";
                dbContext.Entry(model).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                contractModel = dbContext.MANE_IMS_CONTRACT.Where(x => model.MANE_PR_CONTRACT_CODE == x.MANE_PR_CONTRACT_CODE && model.IMS_ROAD_CODE == x.IMS_PR_ROAD_CODE).FirstOrDefault<MANE_IMS_CONTRACT>();
                contractModel.MANE_CONTRACT_FINALIZED = "Y";
                contractModel.MANE_LOCK_STATUS = "Y";
                dbContext.Entry(contractModel).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                message = "Data finalized successfully.";
                status = true;

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "FinalizeDetails()");
                return null;
            }

        }

        public ActionResult DeleteDetails(String parameter, String hash, String key)
        {
            try
            {
                PMGSYEntities dbContext = new PMGSYEntities();
                EMARG_ROAD_WISE_BALANCE_WORK_PACKAGE_DETAILS model = new EMARG_ROAD_WISE_BALANCE_WORK_PACKAGE_DETAILS();
                //  MANE_IMS_CONTRACT contractModel = new MANE_IMS_CONTRACT();

                string message = string.Empty;
                bool status = false;
                int manePrContractCode = 0;
                int imsPrRodeCode = 0;

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    manePrContractCode = Convert.ToInt32(decryptedParameters["ManePrContractCode"]);
                    imsPrRodeCode = Convert.ToInt32(decryptedParameters["RoadCode"]);
                }

                model = dbContext.EMARG_ROAD_WISE_BALANCE_WORK_PACKAGE_DETAILS.Where(x => x.IMS_ROAD_CODE == imsPrRodeCode && x.MANE_PR_CONTRACT_CODE == manePrContractCode).FirstOrDefault<EMARG_ROAD_WISE_BALANCE_WORK_PACKAGE_DETAILS>();
                model.DATA_FLAG = null;
                model.MANE_PR_CONTRACT_CODE = null;
                model.IMS_ROAD_CODE = null;
                dbContext.Entry(model).State = System.Data.Entity.EntityState.Modified;

                dbContext.MANE_IMS_CONTRACT.Remove(dbContext.MANE_IMS_CONTRACT.Where(x => x.IMS_PR_ROAD_CODE == imsPrRodeCode && x.MANE_PR_CONTRACT_CODE == manePrContractCode).FirstOrDefault<MANE_IMS_CONTRACT>());
                dbContext.SaveChanges();

                message = "Data deleted successfully.";
                status = true;

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DeleteDetails()");
                return null;
            }

        }

        #endregion



        public ActionResult WorkStatusReport()
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                WorkStatusReport modelView = new WorkStatusReport();

                modelView.lstStates = objCommon.PopulateStates(true);

                modelView.lstDistricts = new List<SelectListItem>();
                modelView.lstDistricts.Insert(0, new SelectListItem() { Text = "Select District", Value = "0" });

                modelView.lstScheme = new List<SelectListItem>();
                modelView.lstScheme.Insert(0, new SelectListItem() { Text = "All Scheme", Value = "0" });
                modelView.lstScheme.Insert(1, new SelectListItem() { Text = "PMGSY1", Value = "1" });
                modelView.lstScheme.Insert(2, new SelectListItem() { Text = "PMGSY2", Value = "2" });
                modelView.lstScheme.Insert(3, new SelectListItem() { Text = "RCPLWE", Value = "3" });
                modelView.lstScheme.Insert(4, new SelectListItem() { Text = "PMGSY3", Value = "4" });

                return View(modelView);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "WorkStatusReport()");
                return null;
            }
        }

        public ActionResult PopulateAllDistrictsbyStateCode(int stateCode)
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                List<SelectListItem> lstDistrict = new List<SelectListItem>();
                lstDistrict = objCommon.PopulateDistrict(stateCode, true);
                return Json(lstDistrict);
            }
            catch
            {
                return Json(new { string.Empty });
            }
        }

        public ActionResult ViewWorkStatusReport()
        {

            if (!string.IsNullOrEmpty(Request.Params["stateCode"]))
            {
                ViewBag.stateCode = Convert.ToInt32(Request.Params["stateCode"]);
            }

            if (!string.IsNullOrEmpty(Request.Params["districtCode"]))
            {
                ViewBag.districtCode = (Convert.ToInt32(Request.Params["districtCode"])) == -1 ? 0 : Convert.ToInt32(Request.Params["districtCode"]);
            }

            if (!string.IsNullOrEmpty(Request.Params["Scheme"]))
            {
                ViewBag.Scheme = Convert.ToInt32(Request.Params["Scheme"]);
            }
            return View();
        }

    }
}
