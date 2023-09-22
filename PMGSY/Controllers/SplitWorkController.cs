/*----------------------------------------------------------------------------------------

 * Project Id:

 * Project Name:OMMASII

 * File Name: SplitWorkController.cs

 * Author : Koustubh Nakate

 * Creation Date :01/July/2013

 * Desc : This controller is used as get the request and send response as view, list for split work screen.  
 * ---------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Models;
using PMGSY.Common;
using PMGSY.Models.Common;
using PMGSY.Extensions;
using PMGSY.Models.SplitWork;
using PMGSY.BAL.SplitWork;
using PMGSY.DAL.SplitWork;
using PMGSY.BAL.Agreement;

namespace PMGSY.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class SplitWorkController : Controller
    {

        private PMGSYEntities dbContext = new PMGSYEntities();
        public Dictionary<string, string> decryptedParameters = null;
        public String[] encryptedParameters = null;

        ISplitWorkBAL splitWorkBAL = new SplitWorkBAL();
        SplitWorkDAL splitWorkDAL = new SplitWorkDAL();
        CommonFunctions commonFunction = new CommonFunctions();

        string message = string.Empty;

        [Audit]
        public ActionResult SanctionedRoadDetails()
        {
            DAL.MasterDataEntryDAL masterDataEntryDAL = new DAL.MasterDataEntryDAL();
            try
            {
                //only for temporary purpose   

                //end only for temporary purpose 
                TransactionParams transactionParams = new TransactionParams();
                transactionParams.STATE_CODE = PMGSYSession.Current.StateCode;
                transactionParams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                transactionParams.ISSearch = true;
                transactionParams.SANC_YEAR = (Int16)DateTime.Now.Year;
                
                ViewData["FinancialYearList"] = commonFunction.PopulateFinancialYear(true, true);
                ViewData["BlockList"] = new SelectList(masterDataEntryDAL.GetAllBlocksByDistrictCode(transactionParams.DISTRICT_CODE, true), "MAST_BLOCK_CODE", "MAST_BLOCK_NAME");
              // ViewData["PackageList"] = commonFunction.PopulatePackage(transactionParams);
                ViewData["PackageList"] = new SelectList(commonFunction.GetPackages(Convert.ToInt32(transactionParams.SANC_YEAR), 0, true), "IMS_PACKAGE_ID", "IMS_PACKAGE_ID");
                //new filters added by Vikram as per suggested by Dev Sir
                ViewData["BatchList"] = commonFunction.PopulateBatch(true);
                ViewData["CollaborationList"] = commonFunction.PopulateFundingAgency(true);
                ViewData["UpgradationList"] = commonFunction.PopulateNewUpgradeList(true);
                //end of change
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ViewData["FinancialYearList"] = null;
                ViewData["PackageList"] = null;
                ViewData["BlockList"] = null;
            }

            return View("SanctionedRoadDetails");

        }

        [HttpPost]
        [Audit]
        public ActionResult GetProposedRoadList(int? page, int? rows, string sidx, string sord)
        {

            IAgreementBAL agreementBAL = new AgreementBAL();
            long totalRecords;
            int stateCode = PMGSYSession.Current.StateCode;
            int districtCode = PMGSYSession.Current.DistrictCode;
            int adminNDCode = PMGSYSession.Current.AdminNdCode;
            int sanctionedYear = 0;
            int blockCode = 0;
            string packageID = string.Empty;
            string proposalType = "P";
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

                //new filters added by Vikram 
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

                //end of change

                var jsonData = new
                {
                    rows = agreementBAL.GetProposedRoadListBAL(true, stateCode, districtCode, blockCode, sanctionedYear, packageID, proposalType, adminNDCode, batch, collaboration, upgradationType, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
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

        [Audit]
        public ActionResult SplitWorkDetails(String parameter, String hash, String key)
        {
            SplitWorkDetails splitWorkDetails = new SplitWorkDetails();
            splitWorkDAL = new SplitWorkDAL();
            
            decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });
            
            if (decryptedParameters.Count > 0)
            {
                splitWorkDetails.SanctionedCostDetails = splitWorkBAL.GetSanctionedCostDetailsBAL(Convert.ToInt32(decryptedParameters["IMSPRRoadCode"].ToString()), 0);
                if (PMGSYSession.Current.PMGSYScheme == 2)
                {
                    splitWorkDetails.SharePercent = (short)splitWorkDAL.GetSharePercent(Convert.ToInt32(decryptedParameters["IMSPRRoadCode"].ToString()));
                }
            }


            splitWorkDetails.EncryptedIMSPRRoadCode = parameter + "/" + hash + "/" + key;
            return PartialView("SplitWorkDetails", splitWorkDetails);
        }

        [Audit]
        public ActionResult AddSplitWorkDetails(String parameter, String hash, String key)
        {
            SplitWorkDetails splitWorkDetails = new SplitWorkDetails();
            try
            {

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {

                    splitWorkDetails.EncryptedIMSPRRoadCode = parameter + '/' + hash + '/' + key;

                    ViewBag.SanctionedYear = decryptedParameters["SanctionedYear"].ToString();
                    // ViewBag.RoadName = decryptedParameters["IMSRoadName"].ToString();
                    //ViewBag.Package = decryptedParameters["Package"].ToString();
                    //  ViewBag.RoadLength = decryptedParameters["RoadLength"].ToString();
                    ViewBag.RoadLength = decryptedParameters["RoadLength"].ToString().Replace("--", ".");

                    int IMSPRRoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"].ToString());
                    ViewBag.RoadName = dbContext.IMS_SANCTIONED_PROJECTS.Where(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(IMS => IMS.IMS_ROAD_NAME).FirstOrDefault();
                    ViewBag.Package = dbContext.IMS_SANCTIONED_PROJECTS.Where(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(IMS => IMS.IMS_PACKAGE_ID).FirstOrDefault();

                    int? splitCount=dbContext.IMS_PROPOSAL_SPLIT.Where(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(IMS => IMS.IMS_TOTAL_SPLIT).FirstOrDefault();
                    
                    ViewBag.SplitCount = splitCount == null ? "0" : splitCount.ToString();

                    return PartialView("AddSplitWorkDetails", splitWorkDetails);
                }
                return PartialView("AddSplitWorkDetails", splitWorkDetails);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "AddSplitWorkDetails()");
                return PartialView("AddSplitWorkDetails", splitWorkDetails);

            }
        }

        [HttpPost]
        [Audit]
        public ActionResult GetSplitWorkDetailsList(int? page, int? rows, string sidx, string sord)
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
                encryptedParameters = Request.Params["IMSPRRoadCode"].Split('/');

                if (encryptedParameters.Length == 3)
                {
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    IMSPRRoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"].ToString());
                }

                //string isFinalize=dbContext.IMS_PROPOSAL_SPLIT.Where(ps => ps.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(ps => ps.IMS_SPLIT_STATUS).FirstOrDefault();

                IMS_PROPOSAL_SPLIT proposalSplit = dbContext.IMS_PROPOSAL_SPLIT.Where(ps => ps.IMS_PR_ROAD_CODE == IMSPRRoadCode).FirstOrDefault();

                var jsonData = new
                {
                    rows = splitWorkBAL.GetSplitWorkDetailsListBAL(IMSPRRoadCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : (totalRecords % Convert.ToInt32(rows) == 0 ? totalRecords / Convert.ToInt32(rows) : totalRecords / Convert.ToInt32(rows) + 1),
                    page = Convert.ToInt32(page),
                    records = totalRecords,
                    userdata = new { status = proposalSplit == null ? "N" : proposalSplit.IMS_SPLIT_STATUS.ToUpper(), splitCount = proposalSplit == null ? 0 : proposalSplit.IMS_TOTAL_SPLIT }
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "GetSplitWorkDetailsList()");
                return null;
            }

        }

        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult AddSplitWorkDetails(SplitWorkDetails splitWorkDetails)
        {
            bool status = false;
            try
            {
                ///Changes for RCPLWE
                if (PMGSYSession.Current.PMGSYScheme == 2 || PMGSYSession.Current.PMGSYScheme == 3)
                {
                    ModelState.Remove("IMS_STATE_SHARE");
                }

                if (ModelState.IsValid)
                {

                    if (splitWorkBAL.SaveSplitWorkDetailsBAL(splitWorkDetails, ref message))
                    {
                        message = message == string.Empty ? "Split Work details saved successfully." : message;
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Split Work details not saved successfully." : message;
                    }

                }
                else
                {
                    string ermessage = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    //return PartialView("SplitWorkDetails", splitWorkDetails);
                    return Json(new { success = false, message = ermessage }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "AddSplitWorkDetails(SplitWorkDetails splitWorkDetails)");
                message = "Split Work details not saved successfully.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        [Audit]
        public ActionResult EditSplitWorkDetails(String parameter, String hash, String key)
        {
           // Dictionary<string, string> decryptParameters = null;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptedParameters.Count() > 0)
                {
                    SplitWorkDetails splitWorkDetails = splitWorkBAL.GetSplitWorkDetailsBAL(Convert.ToInt32(decryptedParameters["IMSPRRoadCode"].ToString()), Convert.ToInt32(decryptedParameters["IMSWorkCode"].ToString()));

                    if (splitWorkDetails == null)
                    {
                        ModelState.AddModelError(string.Empty, "Split Work details not exist.");
                        return PartialView("SplitWorkDetails", new SplitWorkDetails());
                    }

                    return PartialView("SplitWorkDetails", splitWorkDetails);
                }
                return PartialView("SplitWorkDetails", new SplitWorkDetails());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(string.Empty, "Split Work details not exist.");
                return PartialView("SplitWorkDetails", new SplitWorkDetails());
            }
        }

        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult EditSplitWorkDetails(SplitWorkDetails splitWorkDetails)
        {
            bool status = false;
            try
            {
                //ModelState.AddModelError(string.Empty, "trst");           
                //return PartialView("AgreementDetails", master_agreement);
                if (PMGSYSession.Current.PMGSYScheme == 2 || PMGSYSession.Current.PMGSYScheme == 3)
                {
                    ModelState.Remove("IMS_STATE_SHARE");
                }
              
                if (ModelState.IsValid)
                {
                    if (splitWorkBAL.UpdateSplitWorkDetailsBAL(splitWorkDetails, ref message))
                    {
                        message = message == string.Empty ? "Split Work details updated successfully." : message;
                        status = true;

                    }
                    else
                    {
                        //message = message == string.Empty ? "Split Work details not updated successfully." : message;
                        string ermessage = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                        return Json(new { success = false, message = ermessage }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return PartialView("SplitWorkDetails", splitWorkDetails);
                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "EditSplitWorkDetails(SplitWorkDetails splitWorkDetails)");
                message = message == string.Empty ? "Split Work details not updated successfully." : message;
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }

        }


        [HttpPost]
        [Audit]
        public ActionResult DeleteSplitWorkDetails(String parameter, String hash, String key)
        {

            bool status = false;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {

                    if (splitWorkBAL.DeleteSplitWorkDetailsBAL(Convert.ToInt32(decryptedParameters["IMSPRRoadCode"].ToString()), Convert.ToInt32(decryptedParameters["IMSWorkCode"].ToString()), ref message))
                    {
                        message = "Split Work details deleted successfully.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Split Work details not deleted successfully." : message;
                    }

                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                message = "Split Work details not deleted successfully.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "DeleteSplitWorkDetails()");
                message = "Split Work details not deleted successfully.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }


        [Audit]
        public ActionResult CheckAgreementExist(String parameter, String hash, String key)
        {
            bool exist = false;

            bool isAgreementExist = false;
            bool isSplitWorkExist = false;
            bool isSplitCountExist = false;

            try
            {
                 decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                 if (decryptedParameters.Count() > 0)
                 {
                     exist = splitWorkBAL.CheckAgreementExistBAL(Convert.ToInt32(decryptedParameters["IMSPRRoadCode"].ToString()), ref isAgreementExist, ref isSplitWorkExist, ref isSplitCountExist);
                 }
                 return Json(new { exist = exist, isAgreementExist = isAgreementExist, isSplitWorkExist = isSplitWorkExist, isSplitCountExist = isSplitCountExist }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                //return Json(new { exist = exist }, JsonRequestBehavior.AllowGet);
                return Json(new { exist = exist, isAgreementExist = isAgreementExist, isSplitWorkExist = isSplitWorkExist, isSplitCountExist = isSplitCountExist }, JsonRequestBehavior.AllowGet);
            }
        }



        [Audit]
        public ActionResult SplitCount(String parameter, String hash, String key)
        {
            SplitCount splitCount = new SplitCount();

            splitCount.EncryptedIMSPRRoadCode = parameter + "/" + hash + "/" + key;
            return PartialView("SplitCount", splitCount);
        }

        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult AddSplitCount(SplitCount splitCount)
        {
            bool status = false;
            try
            {

                if (ModelState.IsValid)
                {
                    if (splitWorkBAL.AddSplitCountBAL(splitCount, ref message))
                    {

                        message = message == string.Empty ? "Split Work count saved successfully" : message;
                        status = true;

                    }
                    else
                    {
                        message = message == string.Empty ? "Split Work count not saved successfully." : message;
                    }
                }
                else
                {
                    return PartialView("SplitCount", splitCount);
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Split Work count not saved successfully.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }



        [HttpPost]
        [Audit]
        public ActionResult FinalizedSplitWorkDetails(String parameter, String hash, String key)
        {

            bool status = false;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {

                    if (splitWorkBAL.FinalizedSplitWorkDetailsBAL(Convert.ToInt32(decryptedParameters["IMSPRRoadCode"].ToString()), ref message))
                    {
                        message = "Split Work details finalized.";
                        status = true;
                    }
                    else
                    {
                        message = message == string.Empty ? "Split Work details not finalized." : message;
                    }

                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                message = "Split Work details not finalized.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                message = "Split Work details not finalized.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        #region commonFunction
        protected override void Dispose(bool disposing)
        {
            dbContext.Dispose();
            base.Dispose(disposing);
        }
        #endregion commonFunction
    }
}
