/*----------------------------------------------------------------------------------------

 * Project Id:

 * Project Name:OMMASII

 * File Name: AgreementController.cs

 * Author : Koustubh Nakate

 * Creation Date :18/June/2013

 * Desc : This controller is used as get the request and send response as view, list for agreement screens.  
 * ---------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.BAL.Agreement;
using PMGSY.DAL.Agreement;
using PMGSY.Common;
using PMGSY.Extensions;
using PMGSY.Models.Common;
using PMGSY.Models.Agreement;
using PMGSY.Models;
using System.Configuration;
using System.IO;
namespace PMGSY.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class AgreementController : Controller
    {
        private PMGSYEntities dbContext = new PMGSYEntities();

        IAgreementBAL agreementBAL = new AgreementBAL();
        AgreementDAL agreementDAL = new AgreementDAL();
        CommonFunctions commonFunction = new CommonFunctions();
        int outParam = 0;
        Dictionary<string, string> decryptedParameters = null;
        String[] encryptedParameters = null;

        #region AGREEMENT_DATA_ENTRY

        string message = string.Empty;
        [Audit]
        public ActionResult AgreementAgainstRoad()
        {
            DAL.MasterDataEntryDAL masterDataEntryDAL = new DAL.MasterDataEntryDAL();
            try
            {
                TransactionParams transactionParams = new TransactionParams();
                transactionParams.STATE_CODE = PMGSYSession.Current.StateCode;
                transactionParams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                transactionParams.ISSearch = true;
                transactionParams.SANC_YEAR = (Int16)DateTime.Now.Year;

                ViewData["FinancialYearList"] = commonFunction.PopulateFinancialYear(true, true);
                ViewData["BlockList"] = new SelectList(masterDataEntryDAL.GetAllBlocksByDistrictCode(transactionParams.DISTRICT_CODE, true), "MAST_BLOCK_CODE", "MAST_BLOCK_NAME");
                ViewData["PackageList"] = new SelectList(commonFunction.GetPackages(Convert.ToInt32(transactionParams.SANC_YEAR), 0, true), "IMS_PACKAGE_ID", "IMS_PACKAGE_ID");//commonFunction.PopulatePackage(transactionParams);
                ViewData["ProposalTypeList"] = agreementDAL.GetProposalTypes();
                ViewData["BatchList"] = commonFunction.PopulateBatch(true);
                ViewData["CollaborationList"] = commonFunction.PopulateFundingAgency(true);
                ViewData["UpgradationList"] = commonFunction.PopulateNewUpgradeList(true);
                ///Changes for RCPLWE
                if (PMGSYSession.Current.RoleCode == 36 || PMGSYSession.Current.RoleCode == 47 || PMGSYSession.Current.RoleCode == 56)
                {
                    ViewData["DistrictList"] = commonFunction.PopulateDistrict(PMGSYSession.Current.StateCode);
                }
                ViewBag.EncryptedAgreementType = URLEncrypt.EncryptParameters(new string[] { "AgreementType=" + "C" });
                Session["EncryptedAgreementType"] = ViewBag.EncryptedAgreementType;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ViewData["FinancialYearList"] = null;
                ViewData["PackageList"] = null;
                ViewData["BlockList"] = null;
            }

            return View("AgreementAgainstRoad");

        }

        [Audit]
        public ActionResult AgreementAgainstOtherRoad()
        {
            DAL.MasterDataEntryDAL masterDataEntryDAL = new DAL.MasterDataEntryDAL();
            try
            {

                TransactionParams transactionParams = new TransactionParams();
                transactionParams.STATE_CODE = PMGSYSession.Current.StateCode;
                transactionParams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                transactionParams.ISSearch = true;
                transactionParams.SANC_YEAR = (Int16)DateTime.Now.Year;

                ViewData["FinancialYearList"] = commonFunction.PopulateFinancialYear(true, true);
                ViewData["BlockList"] = new SelectList(masterDataEntryDAL.GetAllBlocksByDistrictCode(transactionParams.DISTRICT_CODE, true), "MAST_BLOCK_CODE", "MAST_BLOCK_NAME");
                //ViewData["PackageList"] = commonFunction.PopulatePackage(transactionParams);
                ViewData["PackageList"] = new SelectList(commonFunction.GetPackages(Convert.ToInt32(transactionParams.SANC_YEAR), 0, true), "IMS_PACKAGE_ID", "IMS_PACKAGE_ID");
                ViewData["ProposalTypeList"] = agreementDAL.GetProposalTypes();
                //new filters added by Vikram as per suggested by Dev Sir
                ViewData["BatchList"] = commonFunction.PopulateBatch(true);
                ViewData["CollaborationList"] = commonFunction.PopulateFundingAgency(true);
                ViewData["UpgradationList"] = commonFunction.PopulateNewUpgradeList(true);
                //end of change
                ViewBag.EncryptedAgreementType = URLEncrypt.EncryptParameters(new string[] { "AgreementType=" + "O" });
                ///Changes for RCPLWE
                if (PMGSYSession.Current.RoleCode == 36 || PMGSYSession.Current.RoleCode == 47 || PMGSYSession.Current.RoleCode == 56)
                {
                    ViewData["DistrictList"] = commonFunction.PopulateDistrict(PMGSYSession.Current.StateCode);
                }
                Session["EncryptedAgreementType"] = ViewBag.EncryptedAgreementType;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                ViewData["FinancialYearList"] = null;
                ViewData["PackageList"] = null;
                ViewData["BlockList"] = null;
            }

            return View("AgreementAgainstRoad");

        }

        [Audit]
        public ActionResult AgreementAgainstSupplier()
        {
            DAL.MasterDataEntryDAL masterDataEntryDAL = new DAL.MasterDataEntryDAL();
            try
            {
                //only for temporary purpose 
                //PMGSYSession.Current.AdminNdCode = 4;
                //PMGSYSession.Current.StateCode = 5;
                //PMGSYSession.Current.DistrictCode = 382;

                ViewBag.EncryptedAgreementType = URLEncrypt.EncryptParameters(new string[] { "AgreementType=" + "S" });
                ViewBag.ConSupFlag = "S";

                Session["EncryptedAgreementType"] = ViewBag.EncryptedAgreementType;

                SelectList lstYears = commonFunction.PopulateFinancialYear(true, true);

                lstYears.First().Selected = true;
                ViewData["FinancialYearList"] = lstYears;//commonFunction.PopulateFinancialYear(true, true);

                ViewData["Status"] = agreementDAL.GetAgreementStatusList();


                return View("AgreementWithoutRoad");

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return View("AgreementWithoutRoad");
            }


        }

        [Audit]
        public ActionResult AgreementAgainstDPR()
        {
            DAL.MasterDataEntryDAL masterDataEntryDAL = new DAL.MasterDataEntryDAL();
            try
            {
                //only for temporary purpose 
                //PMGSYSession.Current.AdminNdCode = 4;
                //PMGSYSession.Current.StateCode = 5;
                //PMGSYSession.Current.DistrictCode = 382;

                ViewBag.EncryptedAgreementType = URLEncrypt.EncryptParameters(new string[] { "AgreementType=" + "D" });
                ViewBag.ConSupFlag = "D";

                Session["EncryptedAgreementType"] = ViewBag.EncryptedAgreementType;

                SelectList lstYears = commonFunction.PopulateFinancialYear(true, true);

                lstYears.First().Selected = true;

                ViewData["FinancialYearList"] = lstYears;//commonFunction.PopulateFinancialYear(true, true);

                ViewData["Status"] = agreementDAL.GetAgreementStatusList();

                return View("AgreementWithoutRoad");

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return View("AgreementWithoutRoad");
            }

        }

        [HttpPost]
        [Audit]
        public JsonResult GetPackagesByYearandBlock(string sanctionYear, string blockCode)
        {

            try
            {
                if (!int.TryParse(sanctionYear, out outParam))
                {
                    return Json(false);
                }
                if (!int.TryParse(blockCode, out outParam))
                {
                    return Json(false);
                }

                //TransactionParams transactionParams = new TransactionParams();

                //transactionParams.STATE_CODE = PMGSYSession.Current.StateCode;
                //transactionParams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                //transactionParams.ISSearch = true;
                //transactionParams.SANC_YEAR = Convert.ToInt16(sanctionYear.Trim());
                //transactionParams.BlockCode = Convert.ToInt16(blockCode.Trim());

                //return Json(commonFunction.PopulatePackage(transactionParams));

                return Json(new SelectList(commonFunction.GetPackages(Convert.ToInt32(sanctionYear), Convert.ToInt32(blockCode), true), "IMS_PACKAGE_ID", "IMS_PACKAGE_ID"));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return Json(false);
            }
        }//end function GetPackagesByYearandBlock

        [HttpPost]
        [Audit]
        public ActionResult GetProposedRoadList(int? page, int? rows, string sidx, string sord)
        {

            long totalRecords;
            int stateCode = PMGSYSession.Current.StateCode;
            int districtCode = PMGSYSession.Current.DistrictCode;
            int adminNDCode = PMGSYSession.Current.AdminNdCode;
            int sanctionedYear = 0;
            int blockCode = 0;
            string packageID = string.Empty;
            string proposalType = string.Empty;
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

                if (!string.IsNullOrEmpty(Request.Params["proposalType"]))
                {
                    proposalType = Request.Params["proposalType"].Trim();
                }
                else
                {
                    proposalType = "0";
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
                ///Change for RCPLWE
                if (PMGSYSession.Current.RoleCode == 36 || PMGSYSession.Current.RoleCode == 47 || PMGSYSession.Current.RoleCode == 56)
                {
                    if (!string.IsNullOrEmpty(Request.Params["districtCode"]))
                    {
                        districtCode = Convert.ToInt32(Request.Params["districtCode"].Trim());
                    }
                }


                var jsonData = new
                {
                    rows = agreementBAL.GetProposedRoadListBAL(false, stateCode, districtCode, blockCode, sanctionedYear, packageID, proposalType, adminNDCode, batch, collaboration, upgradationType, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
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
        public ActionResult GetProposedRoadListForITNO(int? page, int? rows, string sidx, string sord)
        {

            long totalRecords;
            int stateCode = PMGSYSession.Current.StateCode;
            int districtCode = PMGSYSession.Current.DistrictCode;
            int adminNDCode = PMGSYSession.Current.AdminNdCode;
            int sanctionedYear = 0;
            int blockCode = 0;
            string packageID = string.Empty;
            string proposalType = string.Empty;
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

                if (!string.IsNullOrEmpty(Request.Params["proposalType"]))
                {
                    proposalType = Request.Params["proposalType"].Trim();
                }
                else
                {
                    proposalType = "0";
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

                if (PMGSYSession.Current.RoleCode == 36 || PMGSYSession.Current.RoleCode == 47)
                {
                    if (!string.IsNullOrEmpty(Request.Params["districtCode"]))
                    {
                        districtCode = Convert.ToInt32(Request.Params["districtCode"].Trim());
                    }
                }


                var jsonData = new
                {
                    rows = agreementBAL.GetProposedRoadITNOListBAL(false, stateCode, districtCode, blockCode, sanctionedYear, packageID, proposalType, adminNDCode, batch, collaboration, upgradationType, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
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
        public ActionResult AgreementDetails(String parameter, String hash, String key)
        {
            AgreementDetails agreementDetails = new AgreementDetails();
            agreementDetails.AgreementType = true;
            agreementDetails.Mast_Con_Sup_Flag = "C";
            agreementDetails.EncryptedIMSPRRoadCode = parameter + "/" + hash + "/" + key;

            decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

            if (decryptedParameters.Count > 0)
            {
                int IMSPRRoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"].ToString());
                //DateTime? sanctionedDate = null;
                IMS_SANCTIONED_PROJECTS imsMaster = dbContext.IMS_SANCTIONED_PROJECTS.Where(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode).FirstOrDefault();
                agreementDetails.SanctionedDate = imsMaster.IMS_SANCTIONED_DATE == null ? null : Convert.ToDateTime(imsMaster.IMS_SANCTIONED_DATE).ToString("dd/MM/yyyy");
                                
                TEND_NIT_DETAILS tendNITDetails = dbContext.TEND_NIT_DETAILS.Where(NIT => NIT.IMS_PR_ROAD_CODE == IMSPRRoadCode).FirstOrDefault();
                if (tendNITDetails != null)
                {
                    agreementDetails.TEND_TENDER_AMOUNT = (tendNITDetails.TEND_EST_COST == null ? 0 : tendNITDetails.TEND_EST_COST) + (tendNITDetails.TEND_MAINT_COST == null ? 0 : tendNITDetails.TEND_MAINT_COST);

                }
                var PrCodes = dbContext.USP_CHECK_AGREEMENT_ELIGIBILITY().ToList();

                //if (PrCodes.Any(m => m.SANCTION_CODE == IMSPRRoadCode))
                //{  // Show Alert : Agreement Details can not be added as Proposal is freezed.
                //    agreementDetails.AgreementAllowOrNot = "N";
                //}
                //else 
                //{
                //    agreementDetails.AgreementAllowOrNot = "Y";
                
                //}

                agreementDetails.TEND_TENDER_AMOUNT = agreementDetails.TEND_TENDER_AMOUNT == null ? 0 : agreementDetails.TEND_TENDER_AMOUNT;
                if (PMGSYSession.Current.PMGSYScheme == 2)
                {
                    ///Changed by SAMMED A. PATIL on 17FEB2017 for UPKASGANJ issue
                    /*if (imsMaster.IMS_SHARE_PERCENT == 1)
                    {
                        agreementDetails.ProposalStateShare = "10";
                        agreementDetails.ProposalMordShare = "90";
                    }
                    else if (imsMaster.IMS_SHARE_PERCENT == 2)
                    {
                        agreementDetails.ProposalStateShare = "25";
                        agreementDetails.ProposalMordShare = "75";
                    }
                    if (imsMaster.IMS_PROPOSAL_TYPE == "P")
                    {
                        agreementDetails.ProposalStateCost = (imsMaster.IMS_SANCTIONED_RS_AMT == null ? 0 : imsMaster.IMS_SANCTIONED_RS_AMT) + (imsMaster.IMS_SANCTIONED_HS_AMT == null ? 0 : imsMaster.IMS_SANCTIONED_HS_AMT).Value;
                        agreementDetails.ProposalMordCost = ((imsMaster.IMS_SANCTIONED_PAV_AMT == null ? 0 : imsMaster.IMS_SANCTIONED_PAV_AMT) + (imsMaster.IMS_SANCTIONED_PW_AMT == null ? 0 : imsMaster.IMS_SANCTIONED_PW_AMT) + (imsMaster.IMS_SANCTIONED_OW_AMT == null ? 0 : imsMaster.IMS_SANCTIONED_OW_AMT) + (imsMaster.IMS_SANCTIONED_CD_AMT == null ? 0 : imsMaster.IMS_SANCTIONED_CD_AMT) + (imsMaster.IMS_SANCTIONED_FC_AMT == null ? 0 : imsMaster.IMS_SANCTIONED_FC_AMT) - (imsMaster.IMS_SANCTIONED_RS_AMT == null ? 0 : imsMaster.IMS_SANCTIONED_RS_AMT)).Value;
                    }
                    else if (imsMaster.IMS_PROPOSAL_TYPE == "L")
                    {
                        agreementDetails.ProposalStateCost = imsMaster.IMS_SANCTIONED_BS_AMT == null ? 0 : imsMaster.IMS_SANCTIONED_BS_AMT;
                        agreementDetails.ProposalMordCost = (imsMaster.IMS_SANCTIONED_BW_AMT == null ? 0 : imsMaster.IMS_SANCTIONED_BW_AMT);
                    }*/

                    if (imsMaster.IMS_SHARE_PERCENT_2015 == 1)
                    {
                        //<label>(75% / 25%)</label>
                        agreementDetails.ProposalStateShare = "25";
                        agreementDetails.ProposalMordShare = "75";

                    }
                    if (imsMaster.IMS_SHARE_PERCENT_2015 == 2)
                    {
                        //<label>(90% / 10%)</label>
                        agreementDetails.ProposalStateShare = "10";
                        agreementDetails.ProposalMordShare = "90";
                    }
                    if (imsMaster.IMS_SHARE_PERCENT_2015 == 3)
                    {
                        //<label>(60% / 40%)</label>
                        agreementDetails.ProposalStateShare = "40";
                        agreementDetails.ProposalMordShare = "60";
                    }
                    if (imsMaster.IMS_SHARE_PERCENT_2015 == 4)
                    {
                        //<label>(100% / 0%)</label>
                        agreementDetails.ProposalStateShare = "0";
                        agreementDetails.ProposalMordShare = "100";
                    }

                    if (imsMaster.IMS_PROPOSAL_TYPE == "P")
                    {
                        agreementDetails.ProposalStateCost = (Convert.ToDecimal(imsMaster.IMS_SANCTIONED_HS_AMT) + Convert.ToDecimal(imsMaster.IMS_STATE_SHARE_2015) + imsMaster.IMS_SANCTIONED_BS_AMT);
                        agreementDetails.ProposalMordCost = Convert.ToDecimal(imsMaster.IMS_MORD_SHARE_2015);
                    }
                    else if (imsMaster.IMS_PROPOSAL_TYPE == "L")
                    {
                        agreementDetails.ProposalStateCost = Convert.ToDecimal(imsMaster.IMS_STATE_SHARE_2015) + Convert.ToDecimal(imsMaster.IMS_HIGHER_SPECIFICATION_COST);
                        agreementDetails.ProposalMordCost = Convert.ToDecimal(imsMaster.IMS_MORD_SHARE_2015);
                    }
                }
            }

            agreementDetails.IncludeRoadAmount = true;

            return PartialView("AgreementDetails", agreementDetails);
        }

        [Audit]
        public ActionResult AddAgreementAgainstRoad(String parameter, String hash, String key)//String parameter, String hash, String key
        {
            string package = string.Empty;
            AgreementDetails agreementDetails = new AgreementDetails();
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                //encryptedParameters = id.Trim().ToString().Split('/');

                //decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });


                if (decryptedParameters.Count > 0)
                {

                    agreementDetails.EncryptedIMSPRRoadCode = parameter + '/' + hash + '/' + key;

                    //agreementDetails.EncryptedIMSPRRoadCode = encryptedParameters[0] + '/' + encryptedParameters[1] + '/' + encryptedParameters[2];
                    agreementDetails.AgreementType = true;
                    agreementDetails.Mast_Con_Sup_Flag = "C";

                    ViewBag.SanctionedYear = decryptedParameters["SanctionedYear"].ToString();
                    //ViewBag.RoadName = decryptedParameters["IMSRoadName"].ToString();
                    //ViewBag.Package = decryptedParameters["Package"].ToString();
                    ViewBag.RoadLength = decryptedParameters["RoadLength"].ToString().Replace("--", ".");
                    ViewBag.SanctionedDate = decryptedParameters["SanctionedDate"].ToString().Replace("--", "/");

                    int IMSPRRoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"].ToString());

                    //var PrCodes = dbContext.USP_CHECK_AGREEMENT_ELIGIBILITY().ToList();


                    // change on  15-07-2022
                    //var IMS_FREEZE_STATUS = from  s   in  dbContext.IMS_SANCTIONED_PROJECTS
                    //                        where s.IMS_PR_ROAD_CODE == IMSPRRoadCode
                    //                        select new
                    //                         { 
                    //                            s.IMS_FREEZE_STATUS 
                    //                         };

                    string IMS_FREEZE_STATUS = dbContext.IMS_SANCTIONED_PROJECTS.Where(s => s.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(s => s.IMS_FREEZE_STATUS).First();

                    // Qty =b.Qty ?? 0                    
                    agreementDetails.AgreementAllowOrNot = (IMS_FREEZE_STATUS.Equals("U") || IMS_FREEZE_STATUS == null) ? "Y" : "N";




                    //if (PrCodes.Any(m => m.SANCTION_CODE == IMSPRRoadCode))
                    //{  // Show Alert : Agreement Details can not be added as Proposal is freezed.
                    //    agreementDetails.AgreementAllowOrNot = "N";
                    //}
                    //else
                    //{
                    //    agreementDetails.AgreementAllowOrNot = "Y";

                    //}


                    string proposalType = decryptedParameters["ProposalType"].ToString();

                    if (proposalType.Equals("P"))
                    {
                        ViewBag.RoadName = dbContext.IMS_SANCTIONED_PROJECTS.Where(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(IMS => IMS.IMS_ROAD_NAME).FirstOrDefault();
                    }
                    else
                    {
                        ViewBag.RoadName = dbContext.IMS_SANCTIONED_PROJECTS.Where(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(IMS => IMS.IMS_BRIDGE_NAME).FirstOrDefault();
                    }

                    ViewBag.Package = dbContext.IMS_SANCTIONED_PROJECTS.Where(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(IMS => IMS.IMS_PACKAGE_ID).FirstOrDefault();

                    DateTime? sanctionedDate = dbContext.IMS_SANCTIONED_PROJECTS.Where(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(IMS => IMS.IMS_SANCTIONED_DATE).FirstOrDefault();

                    agreementDetails.SanctionedDate = sanctionedDate == null ? null : Convert.ToDateTime(sanctionedDate).ToString("dd/MM/yyyy");

                    TEND_NIT_DETAILS tendNITDetails = dbContext.TEND_NIT_DETAILS.Where(NIT => NIT.IMS_PR_ROAD_CODE == IMSPRRoadCode).FirstOrDefault();
                    if (tendNITDetails != null)
                    {
                        agreementDetails.TEND_TENDER_AMOUNT = (tendNITDetails.TEND_EST_COST == null ? 0 : tendNITDetails.TEND_EST_COST) + (tendNITDetails.TEND_MAINT_COST == null ? 0 : tendNITDetails.TEND_MAINT_COST);

                    }
                    agreementDetails.TEND_TENDER_AMOUNT = agreementDetails.TEND_TENDER_AMOUNT == null ? 0 : agreementDetails.TEND_TENDER_AMOUNT;

                    //check for if any active agreement present against road

                    //if (dbContext.TEND_AGREEMENT_DETAIL.Any(ad => ad.IMS_PR_ROAD_CODE == IMSPRRoadCode && ad.TEND_AGREEMENT_STATUS == "P"))
                    //{
                    //    ViewBag.isAgreementActive = true;
                    //}
                    //else
                    //{
                    //    ViewBag.isAgreementActive = false;
                    //}

                    ///Changed by SAMMED A. PATIL on 03 OCTOBER 2017 to check if Physical progress of Road is Complete
                    if (dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(x => x.IMS_ISCOMPLETED).FirstOrDefault() == "C")
                    {
                        ViewBag.isCompleted = true;
                    }
                    else
                    {
                        ViewBag.isCompleted = false;
                    }

                    return PartialView("AddAgreementAgainstRoad", agreementDetails);
                }
                return PartialView("AddAgreementAgainstRoad", agreementDetails);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return PartialView("AddAgreementAgainstRoad", agreementDetails);
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult GetAgreementMasterDetailsList(int? page, int? rows, string sidx, string sord)
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

                encryptedParameters = Request.Params["IMSPRRoadCode"].Split('/');

                if (encryptedParameters.Length == 3)
                {
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    IMSPRRoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"].ToString());
                }

                encryptedParameters = Request.Params["AgreementType"].Split('/');

                if (encryptedParameters.Length == 3)
                {
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    agreementType = decryptedParameters["AgreementType"].ToString();
                }

                var jsonData = new
                {
                    rows = agreementBAL.GetAgreementDetailsListBAL(IMSPRRoadCode, agreementType, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
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
        public ActionResult GetAgreementMasterDetailsListITNO(int? page, int? rows, string sidx, string sord)
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

                encryptedParameters = Request.Params["IMSPRRoadCode"].Split('/');

                if (encryptedParameters.Length == 3)
                {
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    IMSPRRoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"].ToString());
                }

                encryptedParameters = Request.Params["AgreementType"].Split('/');

                if (encryptedParameters.Length == 3)
                {
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    agreementType = decryptedParameters["AgreementType"].ToString();
                }

                var jsonData = new
                {
                    rows = agreementBAL.GetAgreementDetailsListITNOBAL(IMSPRRoadCode, agreementType, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
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
        public ActionResult ChangeTerminatedAgreementStatus(String parameter, String hash, String key)
        {
            String[] urlParams;
            int roadCode = 0;
            int agreementCode = 0;
            bool status = false;
            message = "Agreement status not changed.";
            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        agreementCode = Convert.ToInt32(urlSplitParams[0]);

                        //Taken for is Mord Unlocked Status
                        roadCode = Convert.ToInt32(urlSplitParams[1]);

                        if (agreementBAL.ChangeTerminatedAgreementStatusBAL(agreementCode, roadCode))
                        {
                            message = "Agreement status changed to 'In Progress'.";
                            status = true;
                        }
                    }
                }
                //decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                //if (decryptedParameters.Count > 0)
                //{
                //    agreementCode = Convert.ToInt32(decryptedParameters["TendAgreementCode"]);


                //    if (agreementBAL.ChangeTerminatedAgreementStatusBAL(agreementCode))
                //    {
                //        message = "Agreement status changed to 'In Progress'.";
                //        status = true;
                //    }

                //}

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                // message = message = "Agreement not finalized successfully.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult ChangeTerminatedAgreementMasterStatus(String parameter, String hash, String key)
        {
            String[] urlParams;
            int roadCode = 0;
            int agreementCode = 0;
            bool status = false;
            message = "Agreement status not changed.";
            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        agreementCode = Convert.ToInt32(urlSplitParams[0]);

                        //Taken for is Mord Unlocked Status
                        roadCode = Convert.ToInt32(urlSplitParams[1]);

                        if (agreementBAL.ChangeTerminatedAgreementMasterStatusBAL(agreementCode))
                        {
                            message = "Agreement status changed to 'In Progress'.";
                            status = true;
                        }
                    }
                }
                //decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                //if (decryptedParameters.Count > 0)
                //{
                //    agreementCode = Convert.ToInt32(decryptedParameters["TendAgreementCode"]);


                //    if (agreementBAL.ChangeTerminatedAgreementStatusBAL(agreementCode))
                //    {
                //        message = "Agreement status changed to 'In Progress'.";
                //        status = true;
                //    }

                //}

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                // message = message = "Agreement not finalized successfully.";
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult AddAgreementDetails(AgreementDetails details_agreement)
        {
            bool status = false;
            string proposalType = string.Empty;
            string agreementType = string.Empty;
            try
            {
                //ModelState.AddModelError(string.Empty, "test");
                //return PartialView("AgreementDetails", details_agreement);

                encryptedParameters = details_agreement.EncryptedIMSPRRoadCode.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    message = "Agreement details not added successfully.";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                proposalType = decryptedParameters["ProposalType"].ToString();

                encryptedParameters = null;
                encryptedParameters = details_agreement.EncryptedAgreementType_Add.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    message = "Agreement details not added successfully.";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                agreementType = decryptedParameters["AgreementType"].ToString();

                if (agreementType.Equals("O"))
                {
                    ModelState.Remove("TEND_TENDER_AMOUNT");
                    ModelState.Remove("TEND_AMOUNT_YEAR1");
                    ModelState.Remove("TEND_AMOUNT_YEAR2");
                    ModelState.Remove("TEND_AMOUNT_YEAR3");
                    ModelState.Remove("TEND_AMOUNT_YEAR4");
                    ModelState.Remove("TEND_AMOUNT_YEAR5");
                    ModelState.Remove("TEND_AMOUNT_YEAR6");
                }

                if (proposalType.Equals("L"))
                {
                    ModelState.Remove("TEND_AMOUNT_YEAR1");
                    ModelState.Remove("TEND_AMOUNT_YEAR2");
                    ModelState.Remove("TEND_AMOUNT_YEAR3");
                    ModelState.Remove("TEND_AMOUNT_YEAR4");
                    ModelState.Remove("TEND_AMOUNT_YEAR5");
                    ModelState.Remove("TEND_AMOUNT_YEAR6");
                }

                if (PMGSYSession.Current.PMGSYScheme == 2)
                {
                    details_agreement.TEND_HIGHER_SPEC_AMT = 0;
                }

                if (ModelState.IsValid)
                {
                    if (agreementBAL.SaveAgreementDetailsBAL(details_agreement, ref message))
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
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });

                    //return PartialView("AgreementDetails", details_agreement);
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


        [HttpPost]
        [Audit]
        public JsonResult GetAgreementNumbersByContractor(string contractorID, string agreementType)
        {

            try
            {
                encryptedParameters = agreementType.Split('/');

                if (encryptedParameters.Length == 3)
                {
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    agreementType = decryptedParameters["AgreementType"].ToString();
                }

                List<TEND_AGREEMENT_MASTER> agreementNumbersList = new List<TEND_AGREEMENT_MASTER>();

                AgreementDAL agreementDAL = new AgreementDAL();

                agreementNumbersList = agreementDAL.GetAgreementNumbers(Convert.ToInt32(contractorID), agreementType, false);

                return Json(new SelectList(agreementNumbersList, "TEND_AGREEMENT_CODE", "TEND_AGREEMENT_NUMBER"));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(false);
            }
        }//end function GetAgreementNumbersByContractor

        //function not in use due to requirement changes
        [HttpPost]
        [Audit]
        public JsonResult GetProposalWorksByIMSRoadCode(String parameter, String hash, String key)
        {
            try
            {
                string IMSRoadCode = parameter + "/" + hash + "/" + key;
                List<IMS_PROPOSAL_WORK> lstProposalWork = new List<IMS_PROPOSAL_WORK>();

                AgreementDAL agreementDAL = new AgreementDAL();

                lstProposalWork = agreementDAL.GetProposalWorks(IMSRoadCode, string.Empty, false, true);

                return Json(new SelectList(lstProposalWork, "IMS_WORK_CODE", "IMS_WORK_DESC"));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(false);
            }
        }//end function GetProposalWorksByIMSRoadCode


        [Audit]
        public ActionResult ExistingAgreement(String contractorID, String agreementCode, String IMSPRRoadCode)
        {

            ExistingAgreementDetails existingAgreement = new ExistingAgreementDetails();
            try
            {
                if (!string.IsNullOrEmpty(contractorID) && !string.IsNullOrEmpty(agreementCode))
                {

                    agreementCode = agreementCode.Trim();
                    contractorID = contractorID.Trim();

                    existingAgreement = agreementBAL.GetExistingAgreementDetailsBAL(Convert.ToInt32(contractorID), Convert.ToInt32(agreementCode));
                    existingAgreement.EncryptedIMSPRRoadCode_Existing = IMSPRRoadCode;

                    return PartialView("ExistingAgreement", existingAgreement);
                }
                return PartialView("ExistingAgreement", existingAgreement);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return PartialView("ExistingAgreement", existingAgreement);
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult AddExistingAgreementDetails(ExistingAgreementDetails details_agreement_existing)
        {
            bool status = false;
            string proposalType = string.Empty;
            try
            {

                encryptedParameters = details_agreement_existing.EncryptedIMSPRRoadCode_Existing.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    message = "Agreement details not added successfully.";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                proposalType = decryptedParameters["ProposalType"].ToString();

                if (proposalType.Equals("L"))
                {
                    ModelState.Remove("TEND_AMOUNT_YEAR1");
                    ModelState.Remove("TEND_AMOUNT_YEAR2");
                    ModelState.Remove("TEND_AMOUNT_YEAR3");
                    ModelState.Remove("TEND_AMOUNT_YEAR4");
                    ModelState.Remove("TEND_AMOUNT_YEAR5");
                }

                if (ModelState.IsValid)
                {

                    if (agreementBAL.SaveExistingAgreementDetailsBAL(details_agreement_existing, ref message))
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

                    message = "Agreement details not added successfully.";
                    //return PartialView("ExistingAgreement", details_agreement_existing);
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

        [HttpPost]
        [Audit]
        public ActionResult GetAgreementDetailsList_ByAgreementCode(int? page, int? rows, string sidx, string sord)
        {

            long totalRecords;
            int agreementCode = 0;
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

                //code added by Vikram on 06-07-2014
                if (!string.IsNullOrEmpty(Request.Params["ProposalCode"]))
                {
                    encryptedParameters = Request.Params["IMSPRRoadCode"].Split('/');


                    if (encryptedParameters.Length == 3)
                    {
                        decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                        IMSPRRoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"].ToString());
                    }
                }
                encryptedParameters = null;
                encryptedParameters = Request.Params["AgreementCode"].Split('/');

                if (encryptedParameters.Length == 3)
                {
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    agreementCode = Convert.ToInt32(decryptedParameters["TendAgreementCode"].ToString());
                }

                // agreementCode = Convert.ToInt32(Request.Params["AgreementCode"]);

                var jsonData = new
                {
                    rows = agreementBAL.GetAgreementDetailsListBAL_ByAgreementCode(agreementCode, IMSPRRoadCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
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
        public ActionResult GetAgreementDetailsListITNO_ByAgreementCode(int? page, int? rows, string sidx, string sord)
        {

            long totalRecords;
            int agreementCode = 0;
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

                //code added by Vikram on 06-07-2014
                if (!string.IsNullOrEmpty(Request.Params["ProposalCode"]))
                {
                    encryptedParameters = Request.Params["IMSPRRoadCode"].Split('/');


                    if (encryptedParameters.Length == 3)
                    {
                        decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                        IMSPRRoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"].ToString());
                    }
                }
                encryptedParameters = null;
                encryptedParameters = Request.Params["AgreementCode"].Split('/');

                if (encryptedParameters.Length == 3)
                {
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    agreementCode = Convert.ToInt32(decryptedParameters["TendAgreementCode"].ToString());
                }

                // agreementCode = Convert.ToInt32(Request.Params["AgreementCode"]);

                var jsonData = new
                {
                    rows = agreementBAL.GetAgreementDetailsListITNOBAL_ByAgreementCode(agreementCode, IMSPRRoadCode, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
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
        public ActionResult EditAgreementMasterDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptParameters = null;
            try
            {
                decryptParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                PMGSYEntities dbContext = new PMGSYEntities();
                if (decryptParameters.Count() > 0)
                {
                    AgreementDetails agreementDetails = agreementBAL.GetAgreementMasterDetailsBAL_ByAgreementCode(Convert.ToInt32(decryptParameters["TendAgreementCode"].ToString()));
                    int ProposalCode = Convert.ToInt32(decryptParameters["IMSPRRoadCode"].ToString());
                    IMS_SANCTIONED_PROJECTS imsMaster = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == ProposalCode).FirstOrDefault();
                    if (PMGSYSession.Current.PMGSYScheme == 2)
                    {
                        ///Changed by SAMMED A. PATIL on 17FEB2017 for UPKASGANJ issue
                        /*if (imsMaster.IMS_SHARE_PERCENT == 1)
                        {
                            agreementDetails.ProposalStateShare = "10";
                            agreementDetails.ProposalMordShare = "90";
                        }
                        else if (imsMaster.IMS_SHARE_PERCENT == 2)
                        {
                            agreementDetails.ProposalStateShare = "25";
                            agreementDetails.ProposalMordShare = "75";
                        }

                        if (imsMaster.IMS_PROPOSAL_TYPE == "P")
                        {
                            agreementDetails.ProposalStateCost = imsMaster.IMS_SANCTIONED_RS_AMT == null ? 0 : imsMaster.IMS_SANCTIONED_RS_AMT;
                            agreementDetails.ProposalMordCost = ((imsMaster.IMS_SANCTIONED_PAV_AMT == null ? 0 : imsMaster.IMS_SANCTIONED_PAV_AMT) + (imsMaster.IMS_SANCTIONED_PW_AMT == null ? 0 : imsMaster.IMS_SANCTIONED_PW_AMT) + (imsMaster.IMS_SANCTIONED_OW_AMT == null ? 0 : imsMaster.IMS_SANCTIONED_OW_AMT) + (imsMaster.IMS_SANCTIONED_CD_AMT == null ? 0 : imsMaster.IMS_SANCTIONED_CD_AMT) + (imsMaster.IMS_SANCTIONED_FC_AMT == null ? 0 : imsMaster.IMS_SANCTIONED_FC_AMT) - (imsMaster.IMS_SANCTIONED_RS_AMT == null ? 0 : imsMaster.IMS_SANCTIONED_RS_AMT)).Value;
                        }
                        else if (imsMaster.IMS_PROPOSAL_TYPE == "L")
                        {
                            agreementDetails.ProposalStateCost = imsMaster.IMS_SANCTIONED_BS_AMT == null ? 0 : imsMaster.IMS_SANCTIONED_BS_AMT;
                            agreementDetails.ProposalMordCost = (imsMaster.IMS_SANCTIONED_BW_AMT == null ? 0 : imsMaster.IMS_SANCTIONED_BW_AMT);
                        }*/

                        if (imsMaster.IMS_SHARE_PERCENT_2015 == 1)
                        {
                            //<label>(75% / 25%)</label>
                            agreementDetails.ProposalStateShare = "25";
                            agreementDetails.ProposalMordShare = "75";

                        }
                        if (imsMaster.IMS_SHARE_PERCENT_2015 == 2)
                        {
                            //<label>(90% / 10%)</label>
                            agreementDetails.ProposalStateShare = "10";
                            agreementDetails.ProposalMordShare = "90";
                        }
                        if (imsMaster.IMS_SHARE_PERCENT_2015 == 3)
                        {
                            //<label>(60% / 40%)</label>
                            agreementDetails.ProposalStateShare = "40";
                            agreementDetails.ProposalMordShare = "60";
                        }
                        if (imsMaster.IMS_SHARE_PERCENT_2015 == 4)
                        {
                            //<label>(100% / 0%)</label>
                            agreementDetails.ProposalStateShare = "0";
                            agreementDetails.ProposalMordShare = "100";
                        }

                        if (imsMaster.IMS_PROPOSAL_TYPE == "P")
                        {
                            agreementDetails.ProposalStateCost = (Convert.ToDecimal(imsMaster.IMS_SANCTIONED_HS_AMT) + Convert.ToDecimal(imsMaster.IMS_STATE_SHARE_2015) + imsMaster.IMS_SANCTIONED_BS_AMT);
                            agreementDetails.ProposalMordCost = Convert.ToDecimal(imsMaster.IMS_MORD_SHARE_2015);
                        }
                        else if (imsMaster.IMS_PROPOSAL_TYPE == "L")
                        {
                            agreementDetails.ProposalStateCost = Convert.ToDecimal(imsMaster.IMS_STATE_SHARE_2015) + Convert.ToDecimal(imsMaster.IMS_HIGHER_SPECIFICATION_COST);
                            agreementDetails.ProposalMordCost = Convert.ToDecimal(imsMaster.IMS_MORD_SHARE_2015);
                        }
                    }
                    if (agreementDetails == null)
                    {
                        ModelState.AddModelError(string.Empty, "Agreement details not exist.");
                        return PartialView("AgreementDetails", new AgreementDetails());
                    }

                    // Pankaj Sir 
                    //var PrCodes = dbContext.USP_CHECK_AGREEMENT_ELIGIBILITY().ToList();
                    //if (PrCodes.Any(m => m.SANCTION_CODE == ProposalCode))
                    //{  // Show Alert : Agreement Details can not be added as Proposal is freezed.
                    //    agreementDetails.AgreementAllowOrNot = "N";
                    //}
                    //else
                    //{
                    //    agreementDetails.AgreementAllowOrNot = "Y";

                    //}

                    // added on 15-07-2022
                    if (imsMaster.IMS_FREEZE_STATUS.Equals("U"))
                    {
                        agreementDetails.AgreementAllowOrNot = "Y";
                    }
                    else
                    {
                        agreementDetails.AgreementAllowOrNot = "N";
                    }


                    return PartialView("AgreementDetails", agreementDetails);
                }
                return PartialView("AgreementDetails", new AgreementDetails());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                ModelState.AddModelError(String.Empty, "Agreement details not exist.");
                return PartialView("AgreementDetails", new AgreementDetails());
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult EditAgreementMasterDetails(AgreementDetails master_agreement)
        {
            bool status = false;
            string agreementType = string.Empty;
            try
            {
                //ModelState.AddModelError(string.Empty, "trst");
                //return PartialView("AgreementDetails", master_agreement);                
                encryptedParameters = master_agreement.EncryptedAgreementType_Add.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    message = "Agreement details not updated successfully.";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                agreementType = decryptedParameters["AgreementType"].ToString();

                if (agreementType.Equals("O"))
                {
                    ModelState.Remove("TEND_TENDER_AMOUNT");
                }

                if (ModelState.IsValid)
                {

                    if ((PMGSYSession.Current.RoleCode == 36 || PMGSYSession.Current.RoleCode == 47 || PMGSYSession.Current.RoleCode == 56) ? agreementBAL.UpdateAgreementMasterDetailsITNOBAL(master_agreement, ref message) : agreementBAL.UpdateAgreementMasterDetailsBAL(master_agreement, ref message))
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
                    return PartialView("AgreementDetails", master_agreement);
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
        public ActionResult DeleteAgreementMasterDetails(String parameter, String hash, String key)
        {

            bool status = false;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {

                    if (agreementBAL.DeleteAgreementMasterDetailsBAL_ByAgreementCode(Convert.ToInt32(decryptedParameters["TendAgreementCode"].ToString()), ref message))
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
        public ActionResult EditAgreementDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptParameters = null;
            try
            {
                decryptParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptParameters.Count() > 0)
                {
                    ExistingAgreementDetails existingAgreementDetails = agreementBAL.GetAgreementDetailsBAL_ByAgreementID(Convert.ToInt32(decryptParameters["TendAgreementCode"].ToString()), Convert.ToInt32(decryptParameters["TendAgreementID"].ToString()));
                    if (existingAgreementDetails == null)
                    {
                        ModelState.AddModelError(string.Empty, "Agreement details not exist.");
                        return PartialView("ExistingAgreement", new ExistingAgreementDetails());
                    }

                    return PartialView("ExistingAgreement", existingAgreementDetails);
                }
                return PartialView("ExistingAgreement", new ExistingAgreementDetails());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                ModelState.AddModelError(String.Empty, "Agreement details not exist.");
                return PartialView("ExistingAgreement", new ExistingAgreementDetails());
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult EditAgreementDetails(ExistingAgreementDetails details_agreement)
        {
            bool status = false;
            string proposalType = string.Empty;
            string agreementType = string.Empty;

            try
            {
                encryptedParameters = details_agreement.EncryptedIMSPRRoadCode_Existing.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    message = "Agreement details not updated successfully.";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                proposalType = decryptedParameters["ProposalType"].ToString();

                //get agreement type
                encryptedParameters = null;
                encryptedParameters = Session["EncryptedAgreementType"].ToString().Split('/');

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                agreementType = decryptedParameters["AgreementType"].ToString();


                if (proposalType.Equals("L") || agreementType.Equals("O"))
                {
                    ModelState.Remove("TEND_AMOUNT_YEAR1");
                    ModelState.Remove("TEND_AMOUNT_YEAR2");
                    ModelState.Remove("TEND_AMOUNT_YEAR3");
                    ModelState.Remove("TEND_AMOUNT_YEAR4");
                    ModelState.Remove("TEND_AMOUNT_YEAR5");
                }

                if (ModelState.IsValid)
                {

                    //  IMasterDataEntryBAL masterDataEntryBAL = new MasterDataEntryBAL();

                    if ((PMGSYSession.Current.RoleCode == 36 || PMGSYSession.Current.RoleCode == 47 || PMGSYSession.Current.RoleCode == 56) ? agreementBAL.UpdateAgreementDetailsITNOBAL(details_agreement, ref message) : agreementBAL.UpdateAgreementDetailsBAL(details_agreement, ref message))
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
                    return PartialView("ExistingAgreement", details_agreement);
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
        public ActionResult DeleteAgreementDetails(String parameter, String hash, String key)
        {

            bool status = false;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {

                    if (agreementBAL.DeleteAgreementDetailsBAL_ByAgreementID(Convert.ToInt32(decryptedParameters["TendAgreementID"].ToString()), Convert.ToInt32(decryptedParameters["TendAgreementCode"].ToString()), ref message))
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

        [HttpPost]
        [Audit]
        public ActionResult FinalizeAgreement(String parameter, String hash, String key)
        {
            int agreementCode = 0;
            bool status = false;
            message = "Agreement not finalized successfully.";
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    agreementCode = Convert.ToInt32(decryptedParameters["TendAgreementCode"]);

                    if (agreementBAL.FinalizeAgreementBAL(agreementCode))
                    {
                        message = "Agreement finalized successfully.";
                        status = true;
                    }
                }
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "FinalizeAgreement()");
                // message = message = "Agreement not finalized successfully.";
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
                    if (agreementBAL.ChangeAgreementStatusToInCompleteBAL(incompleteReason, ref message))
                    {

                        message = message == string.Empty ? "Agreement status changed to 'Incomplete'." : message;
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


        [Audit]
        public ActionResult CheckSplitWorkFinalized(String parameter, String hash, String key) //String parameter, String hash, String key
        {

            bool isSplitWorkFinalized = false;

            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });

                //encryptedParameters = id.Trim().ToString().Split('/');

                //decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                if (decryptedParameters.Count() > 0)
                {
                    isSplitWorkFinalized = agreementBAL.CheckSplitWorkFinalizedBAL(Convert.ToInt32(decryptedParameters["IMSPRRoadCode"].ToString()));
                }
                return Json(new { isSplitWorkFinalized = isSplitWorkFinalized }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                //return Json(new { exist = exist }, JsonRequestBehavior.AllowGet);
                return Json(new { isSplitWorkFinalized = isSplitWorkFinalized }, JsonRequestBehavior.AllowGet);
            }
        }

        [Audit]
        public ActionResult CheckforActiveAgreement()
        {

            bool exist = false;
            string agreementType = string.Empty;
            int IMSPRRoadCode = 0;
            bool isAgreementAvailable = true;
            try
            {
                if (Request.Params["AgreementType"] == null || Request.Params["EncryptedIMSPRCode"] == null)
                {
                    return Json(new { exist = true }, JsonRequestBehavior.AllowGet);
                }

                encryptedParameters = Request.Params["AgreementType"].Split('/');

                if (encryptedParameters.Length == 3)
                {
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    agreementType = decryptedParameters["AgreementType"].ToString();
                }

                encryptedParameters = null;
                encryptedParameters = Request.Params["EncryptedIMSPRCode"].Split('/');

                if (encryptedParameters.Length == 3)
                {
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    IMSPRRoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"].ToString());
                }

                exist = agreementBAL.CheckforActiveAgreementBAL(IMSPRRoadCode, agreementType, ref isAgreementAvailable);

                return Json(new { exist = exist, isAgreementAvailable = isAgreementAvailable }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return Json(new { exist = exist }, JsonRequestBehavior.AllowGet);
            }
        }



        [Audit]
        public ActionResult ViewAgreementMasterDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptParameters = null;
            try
            {
                decryptParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptParameters.Count() > 0)
                {
                    AgreementDetails agreementDetails = agreementBAL.GetAgreementMasterDetailsBAL_ByAgreementCode(Convert.ToInt32(decryptParameters["TendAgreementCode"].ToString()), true);

                    ViewBag.ContractorName = dbContext.MASTER_CONTRACTOR.Where(c => c.MAST_CON_ID == agreementDetails.MAST_CON_ID).Select(c => c.MAST_CON_COMPANY_NAME).FirstOrDefault();
                    if (agreementDetails == null)
                    {
                        ModelState.AddModelError(string.Empty, "Agreement details not exist.");
                        return PartialView("ViewAgreementDetails", new AgreementDetails());
                    }

                    return PartialView("ViewAgreementDetails", agreementDetails);
                }
                return PartialView("ViewAgreementDetails", new AgreementDetails());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                ModelState.AddModelError(String.Empty, "Agreement details not exist.");
                return PartialView("ViewAgreementDetails", new AgreementDetails());
            }
        }


        public ActionResult SearchContractorByPan()
        {
            try
            {
                return PartialView();
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpPost]
        public ActionResult GetContractorsByPan(string stateCode)
        {
            try
            {
                int state = Convert.ToInt32(stateCode.Split('$')[0]);
                string panSearch = stateCode.Split('$')[1];
                string conSupFlag = stateCode.Split('$')[2];
                List<SelectListItem> lstContractors = new List<SelectListItem>();
                AgreementDAL objDAL = new AgreementDAL();
                lstContractors = objDAL.PopulateContractorsByPan(state, panSearch, conSupFlag);
                return Json(new SelectList(lstContractors, "Value", "Text"));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(false);
            }
        }

        /// <summary>
        /// returns the list of blocks according to the district
        /// </summary>
        /// <param name="districtCode"></param>
        /// <returns></returns>
        public ActionResult GetBlocksByDistricts(int districtCode)
        {
            try
            {
                return Json(commonFunction.PopulateBlocks(districtCode, true));
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// returns the list of districts according to the state
        /// </summary>
        /// <param name="districtCode"></param>
        /// <returns></returns>
        public ActionResult GetDistrictsByState(int stateCode)
        {
            try
            {
                return Json(commonFunction.PopulateDistrict(stateCode, true));
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        #region Agreement without Road
        [HttpPost]
        [Audit]
        public ActionResult GetAgreementMasterDetailsList_WithoutRoad(int? page, int? rows, string sidx, string sord)
        {

            long totalRecords;
            string agreementType = string.Empty;
            int agreementYear = 0;
            string status = string.Empty;

            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["agreementYear"]))
                {
                    agreementYear = Convert.ToInt32(Request.Params["agreementYear"]);
                }
                else
                {
                    return null;
                }

                if (!string.IsNullOrEmpty(Request.Params["status"]))
                {
                    status = Request.Params["status"].Trim();
                }
                else
                {
                    status = "0";
                }

                encryptedParameters = Request.Params["AgreementType"].Split('/');

                if (encryptedParameters.Length == 3)
                {
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    agreementType = decryptedParameters["AgreementType"].ToString();
                }

                var jsonData = new
                {
                    rows = agreementBAL.GetAgreementDetailsListBAL_WithoutRoad(agreementYear, status, agreementType, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
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
        public ActionResult AddAgreementWithoutRoad()
        {
            AgreementDetails agreementDetails = new AgreementDetails();
            agreementDetails.Mast_Con_Sup_Flag = "S";
            try
            {
                return PartialView("AddAgreementWithoutRoad", agreementDetails);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return PartialView("AddAgreementWithoutRoad", agreementDetails);
            }
        }

        [Audit]
        public ActionResult AddAgreementWithoutRoad_DPR()
        {
            AgreementDetails agreementDetails = new AgreementDetails();
            agreementDetails.Mast_Con_Sup_Flag = "D";
            try
            {

                return PartialView("AddAgreementWithoutRoad", agreementDetails);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return PartialView("AddAgreementWithoutRoad", agreementDetails);
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult AddAgreementDetails_WithoutRoad(AgreementDetails details_agreement)
        {
            bool status = false;
            try
            {
                //ModelState.AddModelError(string.Empty, "test");
                //return PartialView("AddAgreementWithoutRoad", details_agreement);

                ModelState.Remove("TEND_AMOUNT_YEAR1");
                ModelState.Remove("TEND_AMOUNT_YEAR2");
                ModelState.Remove("TEND_AMOUNT_YEAR3");
                ModelState.Remove("TEND_AMOUNT_YEAR4");
                ModelState.Remove("TEND_AMOUNT_YEAR5");
                ModelState.Remove("TEND_TENDER_AMOUNT");

                if (ModelState.IsValid)
                {

                    if (agreementBAL.SaveAgreementDetailsBAL_WithoutRoad(details_agreement, ref message))
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
                    return PartialView("AddAgreementWithoutRoad", details_agreement);
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
        public ActionResult EditAgreementMasterDetails_WithoutRoad(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptParameters = null;
            try
            {
                decryptParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptParameters.Count() > 0)
                {
                    AgreementDetails agreementDetails = agreementBAL.GetAgreementMasterDetailsBAL_ByAgreementCode(Convert.ToInt32(decryptParameters["TendAgreementCode"].ToString()));

                    if (agreementDetails == null)
                    {
                        ModelState.AddModelError(string.Empty, "Agreement details not exist.");
                        return PartialView("AddAgreementWithoutRoad", new AgreementDetails());
                    }

                    return PartialView("AddAgreementWithoutRoad", agreementDetails);
                }
                return PartialView("AddAgreementWithoutRoad", new AgreementDetails());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                ModelState.AddModelError(String.Empty, "Agreement details not exist.");
                return PartialView("AddAgreementWithoutRoad", new AgreementDetails());
            }
        }

        [HttpPost]
        [Audit]
        public ActionResult EditAgreementMasterDetails_WithoutRoad(AgreementDetails details_agreement)
        {
            bool status = false;
            try
            {
                //ModelState.AddModelError(string.Empty, "trst");           
                //return PartialView("AgreementDetails", master_agreement);

                ModelState.Remove("TEND_AMOUNT_YEAR1");
                ModelState.Remove("TEND_AMOUNT_YEAR2");
                ModelState.Remove("TEND_AMOUNT_YEAR3");
                ModelState.Remove("TEND_AMOUNT_YEAR4");
                ModelState.Remove("TEND_AMOUNT_YEAR5");
                ModelState.Remove("TEND_TENDER_AMOUNT");

                if (ModelState.IsValid)
                {
                    if (agreementBAL.UpdateAgreementMasterDetailsBAL_WithoutRoad(details_agreement, ref message))
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
                    return PartialView("AddAgreementWithoutRoad", details_agreement);
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
        public ActionResult ChangeAgreementStatusToComplete(String parameter, String hash, String key)
        {
            int agreementCode = 0;
            bool status = false;
            message = "Agreement status not changed.";
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    agreementCode = Convert.ToInt32(decryptedParameters["TendAgreementCode"]);


                    if (agreementBAL.ChangeAgreementStatusToCompleteBAL(agreementCode))
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
        public ActionResult ViewAgreementMasterDetails_WithoutRoad(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptParameters = null;
            try
            {
                decryptParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptParameters.Count() > 0)
                {
                    AgreementDetails agreementDetails = agreementBAL.GetAgreementMasterDetailsBAL_ByAgreementCode(Convert.ToInt32(decryptParameters["TendAgreementCode"].ToString()), true);

                    ViewBag.ContractorName = dbContext.MASTER_CONTRACTOR.Where(c => c.MAST_CON_ID == agreementDetails.MAST_CON_ID).Select(c => c.MAST_CON_COMPANY_NAME).FirstOrDefault();

                    if (agreementDetails == null)
                    {
                        ModelState.AddModelError(string.Empty, "Agreement details not exist.");
                        return PartialView("ViewAgreementWithoutRoad", new AgreementDetails());
                    }

                    return PartialView("ViewAgreementWithoutRoad", agreementDetails);
                }
                return PartialView("ViewAgreementWithoutRoad", new AgreementDetails());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError(String.Empty, "Agreement details not exist.");
                return PartialView("ViewAgreementWithoutRoad", new AgreementDetails());
            }
        }

        #endregion Agreement without Road

        #region commonFunction
        protected override void Dispose(bool disposing)
        {
            dbContext.Dispose();
            base.Dispose(disposing);
        }
        #endregion commonFunction

        #region PROPOSAL_RELATED_DETAILS

        [Audit]
        public ActionResult ViewProposalAgreementDetails(string id)
        {
            int proposalCode = 0;
            if (int.TryParse(id, out proposalCode))
            {
                string agreementType = dbContext.TEND_AGREEMENT_MASTER.Join(dbContext.TEND_AGREEMENT_DETAIL, tm => tm.TEND_AGREEMENT_CODE, td => td.TEND_AGREEMENT_CODE, (tm, td) => new { tm.TEND_AGREEMENT_TYPE }).Select(m => m.TEND_AGREEMENT_TYPE).ToString();
                ViewBag.AgreementType = agreementType;
                ViewBag.ProposalCode = proposalCode;
            }
            return PartialView();
        }

        [HttpPost]
        [Audit]
        public ActionResult GetProposalAgreementList(int? page, int? rows, string sidx, string sord)
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
                    rows = agreementBAL.GetProposalAgreementListBAL(IMSPRRoadCode, agreementType, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
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



        #endregion

        #region FINALIZE_AGREEMENT

        public ActionResult ListAgreementsToFinalize()
        {
            try
            {
                AgreementFilterViewModel model = new AgreementFilterViewModel();
                model.lstBlocks = commonFunction.PopulateBlocks(PMGSYSession.Current.DistrictCode, true);
                model.lstYears = commonFunction.PopulateFinancialYear(true, true).ToList();
                return View(model);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public ActionResult GetAgreementDetailsList(int? page, int? rows, string sidx, string sord)
        {
            long totalRecords;
            int stateCode = PMGSYSession.Current.StateCode;
            int districtCode = PMGSYSession.Current.DistrictCode;
            int adminNDCode = PMGSYSession.Current.AdminNdCode;
            int yearCode = 0;
            string package = string.Empty;
            string proposalType = string.Empty;
            string agreementStatus = string.Empty;
            string agreementType = string.Empty;
            string finalize = string.Empty;
            int blockCode = 0;
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["YearCode"]))
                {
                    yearCode = Convert.ToInt32(Request.Params["YearCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["BlockCode"]))
                {
                    blockCode = Convert.ToInt32(Request.Params["BlockCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["Package"]))
                {
                    package = Request.Params["Package"];
                }

                if (!string.IsNullOrEmpty(Request.Params["ProposalType"]))
                {
                    proposalType = Request.Params["ProposalType"];
                }

                if (!string.IsNullOrEmpty(Request.Params["AgreementStatus"]))
                {
                    agreementStatus = Request.Params["AgreementStatus"];
                }

                if (!string.IsNullOrEmpty(Request.Params["AgreementType"]))
                {
                    agreementType = Request.Params["AgreementType"];
                }

                if (!string.IsNullOrEmpty(Request.Params["Finalize"]))
                {
                    finalize = Request.Params["Finalize"];
                }


                var jsonData = new
                {
                    rows = agreementBAL.GetAgreementListBAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, yearCode, blockCode, package, proposalType, agreementStatus, finalize, agreementType, out totalRecords),
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
        public ActionResult DeFinalizeAgreement(String parameter, String hash, String key)
        {
            AgreementDAL objDAL = new AgreementDAL();
            int agreementCode = 0;
            bool status = false;
            message = "Agreement not definalized successfully.";
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    agreementCode = Convert.ToInt32(decryptedParameters["TendAgreementCode"]);
                    /// Added by SAMMED A. PATIL on 30JUNE2017 to restrict finalization of agreement if entry found in other tables
                    string validate = objDAL.ValidateFinalizeAgreementDAL(agreementCode);
                    if (validate != "")
                    {
                        message = validate;
                        status = true;
                    }
                    else
                    {
                        if (agreementBAL.DeFinalizeAgreementBAL(agreementCode))
                        {
                            message = "Agreement definalized successfully.";
                            status = true;
                        }
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
        }//end function FinalizeAgreement

        #endregion

        #region SPECIAL AGREEMENT

        /// <summary>
        /// returns the view for listing the proposals
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult SpecialAgreementAgainstRoad()
        {
            DAL.MasterDataEntryDAL masterDataEntryDAL = new DAL.MasterDataEntryDAL();
            try
            {
                TransactionParams transactionParams = new TransactionParams();
                transactionParams.STATE_CODE = PMGSYSession.Current.StateCode;
                transactionParams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                transactionParams.ISSearch = true;
                transactionParams.SANC_YEAR = (Int16)DateTime.Now.Year;

                ViewData["FinancialYearList"] = commonFunction.PopulateFinancialYear(true, true);
                ViewData["BlockList"] = new SelectList(masterDataEntryDAL.GetAllBlocksByDistrictCode(transactionParams.DISTRICT_CODE, true), "MAST_BLOCK_CODE", "MAST_BLOCK_NAME");
                ViewData["PackageList"] = new SelectList(commonFunction.GetPackages(Convert.ToInt32(transactionParams.SANC_YEAR), 0, true), "IMS_PACKAGE_ID", "IMS_PACKAGE_ID");//commonFunction.PopulatePackage(transactionParams);
                ViewData["ProposalTypeList"] = agreementDAL.GetProposalTypes();
                ViewData["BatchList"] = commonFunction.PopulateBatch(true);
                ViewData["CollaborationList"] = commonFunction.PopulateFundingAgency(true);
                ViewData["UpgradationList"] = commonFunction.PopulateNewUpgradeList(true);
                if (PMGSYSession.Current.RoleCode == 36 || PMGSYSession.Current.RoleCode == 47)
                {
                    ViewData["DistrictList"] = commonFunction.PopulateDistrict(PMGSYSession.Current.StateCode);
                }
                ViewBag.EncryptedAgreementType = URLEncrypt.EncryptParameters(new string[] { "AgreementType=" + "R" });
                Session["EncryptedAgreementType"] = ViewBag.EncryptedAgreementType;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ViewData["FinancialYearList"] = null;
                ViewData["PackageList"] = null;
                ViewData["BlockList"] = null;
            }

            return View("SpecialAgreementAgainstRoad");

        }

        /// <summary>
        /// returns the list of proposals
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetSpecialAgreementProposedRoadList(int? page, int? rows, string sidx, string sord)
        {

            long totalRecords;
            int stateCode = PMGSYSession.Current.StateCode;
            int districtCode = PMGSYSession.Current.DistrictCode;
            int adminNDCode = PMGSYSession.Current.AdminNdCode;
            int sanctionedYear = 0;
            int blockCode = 0;
            string packageID = string.Empty;
            string proposalType = string.Empty;
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

                if (!string.IsNullOrEmpty(Request.Params["proposalType"]))
                {
                    proposalType = Request.Params["proposalType"].Trim();
                }
                else
                {
                    proposalType = "0";
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

                if (PMGSYSession.Current.RoleCode == 36 || PMGSYSession.Current.RoleCode == 47)
                {
                    if (!string.IsNullOrEmpty(Request.Params["districtCode"]))
                    {
                        districtCode = Convert.ToInt32(Request.Params["districtCode"].Trim());
                    }
                }


                var jsonData = new
                {
                    rows = agreementBAL.GetSpecialAgreementProposedRoadListBAL(false, stateCode, districtCode, blockCode, sanctionedYear, packageID, proposalType, adminNDCode, batch, collaboration, upgradationType, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
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
        /// returns the view for adding the special agreement details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult SpecialAgreementDetails(String parameter, String hash, String key)
        {
            SpecialAgreementDetails agreementDetails = new SpecialAgreementDetails();
            agreementDetails.AgreementType = true;
            agreementDetails.Mast_Con_Sup_Flag = "R";
            agreementDetails.EncryptedIMSPRRoadCode = parameter + "/" + hash + "/" + key;



            decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

            if (decryptedParameters.Count > 0)
            {
                int IMSPRRoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"].ToString());
                //DateTime? sanctionedDate = null;
                IMS_SANCTIONED_PROJECTS imsMaster = dbContext.IMS_SANCTIONED_PROJECTS.Where(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode).FirstOrDefault();
                agreementDetails.SanctionedDate = imsMaster.IMS_SANCTIONED_DATE == null ? null : Convert.ToDateTime(imsMaster.IMS_SANCTIONED_DATE).ToString("dd/MM/yyyy");

                TEND_NIT_DETAILS tendNITDetails = dbContext.TEND_NIT_DETAILS.Where(NIT => NIT.IMS_PR_ROAD_CODE == IMSPRRoadCode).FirstOrDefault();
                if (tendNITDetails != null)
                {
                    agreementDetails.TEND_TENDER_AMOUNT = (tendNITDetails.TEND_EST_COST == null ? 0 : tendNITDetails.TEND_EST_COST) + (tendNITDetails.TEND_MAINT_COST == null ? 0 : tendNITDetails.TEND_MAINT_COST);

                }

                agreementDetails.TEND_TENDER_AMOUNT = agreementDetails.TEND_TENDER_AMOUNT == null ? 0 : agreementDetails.TEND_TENDER_AMOUNT;
                if (PMGSYSession.Current.PMGSYScheme == 2)
                {
                    if (imsMaster.IMS_SHARE_PERCENT == 1)
                    {
                        agreementDetails.ProposalStateShare = "10";
                        agreementDetails.ProposalMordShare = "90";
                    }
                    else if (imsMaster.IMS_SHARE_PERCENT == 2)
                    {
                        agreementDetails.ProposalStateShare = "25";
                        agreementDetails.ProposalMordShare = "75";
                    }

                    if (imsMaster.IMS_PROPOSAL_TYPE == "P")
                    {
                        agreementDetails.ProposalStateCost = (imsMaster.IMS_SANCTIONED_RS_AMT == null ? 0 : imsMaster.IMS_SANCTIONED_RS_AMT) + (imsMaster.IMS_SANCTIONED_HS_AMT == null ? 0 : imsMaster.IMS_SANCTIONED_HS_AMT).Value;
                        agreementDetails.ProposalMordCost = ((imsMaster.IMS_SANCTIONED_PAV_AMT == null ? 0 : imsMaster.IMS_SANCTIONED_PAV_AMT) + (imsMaster.IMS_SANCTIONED_PW_AMT == null ? 0 : imsMaster.IMS_SANCTIONED_PW_AMT) + (imsMaster.IMS_SANCTIONED_OW_AMT == null ? 0 : imsMaster.IMS_SANCTIONED_OW_AMT) + (imsMaster.IMS_SANCTIONED_CD_AMT == null ? 0 : imsMaster.IMS_SANCTIONED_CD_AMT) + (imsMaster.IMS_SANCTIONED_FC_AMT == null ? 0 : imsMaster.IMS_SANCTIONED_FC_AMT) - (imsMaster.IMS_SANCTIONED_RS_AMT == null ? 0 : imsMaster.IMS_SANCTIONED_RS_AMT)).Value;
                    }
                    else if (imsMaster.IMS_PROPOSAL_TYPE == "L")
                    {
                        agreementDetails.ProposalStateCost = imsMaster.IMS_SANCTIONED_BS_AMT == null ? 0 : imsMaster.IMS_SANCTIONED_BS_AMT;
                        agreementDetails.ProposalMordCost = (imsMaster.IMS_SANCTIONED_BW_AMT == null ? 0 : imsMaster.IMS_SANCTIONED_BW_AMT);
                    }
                }
            }

            return PartialView("SpecialAgreementDetails", agreementDetails);
        }

        /// <summary>
        /// returns the list of agreement entered for the current road
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        public ActionResult GetSpecialAgreementMasterDetailsList(int? page, int? rows, string sidx, string sord)
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

                encryptedParameters = Request.Params["IMSPRRoadCode"].Split('/');

                if (encryptedParameters.Length == 3)
                {
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    IMSPRRoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"].ToString());
                }

                encryptedParameters = Request.Params["AgreementType"].Split('/');

                if (encryptedParameters.Length == 3)
                {
                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    agreementType = decryptedParameters["AgreementType"].ToString();
                }

                var jsonData = new
                {
                    rows = agreementBAL.GetSpecialAgreementDetailsListBAL(IMSPRRoadCode, agreementType, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, out totalRecords),
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
        /// returns the view containing the fields required for obtaining the special agreement details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult AddSpecialAgreementAgainstRoad(String parameter, String hash, String key)
        {
            SpecialAgreementDetails agreementDetails = new SpecialAgreementDetails();
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    agreementDetails.EncryptedIMSPRRoadCode = parameter + '/' + hash + '/' + key;
                    agreementDetails.AgreementType = true;
                    agreementDetails.Mast_Con_Sup_Flag = "R";
                    ViewBag.SanctionedYear = decryptedParameters["SanctionedYear"].ToString();
                    ViewBag.Package = decryptedParameters["Package"].ToString();
                    ViewBag.RoadLength = decryptedParameters["RoadLength"].ToString().Replace("--", ".");
                    ViewBag.SanctionedDate = decryptedParameters["SanctionedDate"].ToString().Replace("--", "/");

                    int IMSPRRoadCode = Convert.ToInt32(decryptedParameters["IMSPRRoadCode"].ToString());

                    string proposalType = decryptedParameters["ProposalType"].ToString();

                    if (proposalType.Equals("P"))
                    {
                        ViewBag.RoadName = dbContext.IMS_SANCTIONED_PROJECTS.Where(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(IMS => IMS.IMS_ROAD_NAME).FirstOrDefault();
                    }
                    else
                    {
                        ViewBag.RoadName = dbContext.IMS_SANCTIONED_PROJECTS.Where(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(IMS => IMS.IMS_BRIDGE_NAME).FirstOrDefault();
                    }

                    DateTime? sanctionedDate = dbContext.IMS_SANCTIONED_PROJECTS.Where(IMS => IMS.IMS_PR_ROAD_CODE == IMSPRRoadCode).Select(IMS => IMS.IMS_SANCTIONED_DATE).FirstOrDefault();
                    agreementDetails.SanctionedDate = sanctionedDate == null ? null : Convert.ToDateTime(sanctionedDate).ToString("dd/MM/yyyy");

                    TEND_NIT_DETAILS tendNITDetails = dbContext.TEND_NIT_DETAILS.Where(NIT => NIT.IMS_PR_ROAD_CODE == IMSPRRoadCode).FirstOrDefault();
                    if (tendNITDetails != null)
                    {
                        agreementDetails.TEND_TENDER_AMOUNT = (tendNITDetails.TEND_EST_COST == null ? 0 : tendNITDetails.TEND_EST_COST) + (tendNITDetails.TEND_MAINT_COST == null ? 0 : tendNITDetails.TEND_MAINT_COST);

                    }
                    agreementDetails.TEND_TENDER_AMOUNT = agreementDetails.TEND_TENDER_AMOUNT == null ? 0 : agreementDetails.TEND_TENDER_AMOUNT;

                    return PartialView("AddSpecialAgreementAgainstRoad", agreementDetails);
                }
                return PartialView("AddSpecialAgreementAgainstRoad", agreementDetails);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                return PartialView("AddSpecialAgreementAgainstRoad", agreementDetails);
            }
        }

        /// <summary>
        /// saves the special agreement details
        /// </summary>
        /// <param name="details_agreement"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult AddSpecialAgreementDetails(SpecialAgreementDetails details_agreement)
        {
            bool status = false;
            string proposalType = string.Empty;
            string agreementType = string.Empty;
            try
            {
                encryptedParameters = details_agreement.EncryptedIMSPRRoadCode.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    message = "Agreement details not added successfully.";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                proposalType = decryptedParameters["ProposalType"].ToString();

                encryptedParameters = null;
                encryptedParameters = details_agreement.EncryptedAgreementType_Add.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    message = "Agreement details not added successfully.";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                agreementType = decryptedParameters["AgreementType"].ToString();

                if (agreementType.Equals("O"))
                {
                    ModelState.Remove("TEND_TENDER_AMOUNT");
                    ModelState.Remove("TEND_AMOUNT_YEAR1");
                    ModelState.Remove("TEND_AMOUNT_YEAR2");
                    ModelState.Remove("TEND_AMOUNT_YEAR3");
                    ModelState.Remove("TEND_AMOUNT_YEAR4");
                    ModelState.Remove("TEND_AMOUNT_YEAR5");
                    ModelState.Remove("TEND_AMOUNT_YEAR6");
                }

                if (proposalType.Equals("L"))
                {
                    ModelState.Remove("TEND_AMOUNT_YEAR1");
                    ModelState.Remove("TEND_AMOUNT_YEAR2");
                    ModelState.Remove("TEND_AMOUNT_YEAR3");
                    ModelState.Remove("TEND_AMOUNT_YEAR4");
                    ModelState.Remove("TEND_AMOUNT_YEAR5");
                    ModelState.Remove("TEND_AMOUNT_YEAR6");
                }

                if (PMGSYSession.Current.PMGSYScheme == 2)
                {
                    details_agreement.TEND_HIGHER_SPEC_AMT = 0;
                }

                if (ModelState.IsValid)
                {
                    if (agreementBAL.SaveSpecialAgreementDetailsBAL(details_agreement, ref message))
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
                    return PartialView("AgreementDetails", details_agreement);
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
        public ActionResult EditSpecialAgreementMasterDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptParameters = null;
            try
            {
                decryptParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                PMGSYEntities dbContext = new PMGSYEntities();
                if (decryptParameters.Count() > 0)
                {
                    SpecialAgreementDetails agreementDetails = agreementBAL.GetSpecialAgreementMasterDetailsBAL_ByAgreementCode(Convert.ToInt32(decryptParameters["TendAgreementCode"].ToString()));
                    int ProposalCode = Convert.ToInt32(decryptParameters["IMSPRRoadCode"].ToString());
                    IMS_SANCTIONED_PROJECTS imsMaster = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == ProposalCode).FirstOrDefault();
                    if (PMGSYSession.Current.PMGSYScheme == 2)
                    {
                        if (imsMaster.IMS_SHARE_PERCENT == 1)
                        {
                            agreementDetails.ProposalStateShare = "10";
                            agreementDetails.ProposalMordShare = "90";
                        }
                        else if (imsMaster.IMS_SHARE_PERCENT == 2)
                        {
                            agreementDetails.ProposalStateShare = "25";
                            agreementDetails.ProposalMordShare = "75";
                        }

                        if (imsMaster.IMS_PROPOSAL_TYPE == "P")
                        {
                            agreementDetails.ProposalStateCost = imsMaster.IMS_SANCTIONED_RS_AMT == null ? 0 : imsMaster.IMS_SANCTIONED_RS_AMT;
                            agreementDetails.ProposalMordCost = ((imsMaster.IMS_SANCTIONED_PAV_AMT == null ? 0 : imsMaster.IMS_SANCTIONED_PAV_AMT) + (imsMaster.IMS_SANCTIONED_PW_AMT == null ? 0 : imsMaster.IMS_SANCTIONED_PW_AMT) + (imsMaster.IMS_SANCTIONED_OW_AMT == null ? 0 : imsMaster.IMS_SANCTIONED_OW_AMT) + (imsMaster.IMS_SANCTIONED_CD_AMT == null ? 0 : imsMaster.IMS_SANCTIONED_CD_AMT) + (imsMaster.IMS_SANCTIONED_FC_AMT == null ? 0 : imsMaster.IMS_SANCTIONED_FC_AMT) - (imsMaster.IMS_SANCTIONED_RS_AMT == null ? 0 : imsMaster.IMS_SANCTIONED_RS_AMT)).Value;
                        }
                        else if (imsMaster.IMS_PROPOSAL_TYPE == "L")
                        {
                            agreementDetails.ProposalStateCost = imsMaster.IMS_SANCTIONED_BS_AMT == null ? 0 : imsMaster.IMS_SANCTIONED_BS_AMT;
                            agreementDetails.ProposalMordCost = (imsMaster.IMS_SANCTIONED_BW_AMT == null ? 0 : imsMaster.IMS_SANCTIONED_BW_AMT);
                        }
                    }
                    if (agreementDetails == null)
                    {
                        ModelState.AddModelError(string.Empty, "Agreement details not exist.");
                        return PartialView("SpecialAgreementDetails", new AgreementDetails());
                    }

                    return PartialView("SpecialAgreementDetails", agreementDetails);
                }
                return PartialView("SpecialAgreementDetails", new AgreementDetails());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                ModelState.AddModelError(String.Empty, "Agreement details not exist.");
                return PartialView("SpecialAgreementDetails", new AgreementDetails());
            }
        }

        /// <summary>
        /// updates the special agreement details
        /// </summary>
        /// <param name="master_agreement"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult EditSpecialAgreementMasterDetails(SpecialAgreementDetails master_agreement)
        {
            bool status = false;
            string agreementType = string.Empty;
            try
            {
                encryptedParameters = master_agreement.EncryptedAgreementType_Add.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    message = "Agreement details not updated successfully.";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                agreementType = decryptedParameters["AgreementType"].ToString();

                if (agreementType.Equals("O"))
                {
                    ModelState.Remove("TEND_TENDER_AMOUNT");
                }

                if (ModelState.IsValid)
                {
                    if (agreementBAL.UpdateSpecialAgreementMasterDetailsBAL(master_agreement, ref message))
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
                    return PartialView("SpecialAgreementDetails", master_agreement);
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

        /// <summary>
        /// updates the details for the proposal
        /// </summary>
        /// <param name="details_agreement"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult EditSpecialAgreementDetails(ExistingSpecialAgreementDetails details_agreement)
        {
            bool status = false;
            string proposalType = string.Empty;
            string agreementType = string.Empty;

            try
            {
                encryptedParameters = details_agreement.EncryptedIMSPRRoadCode_Existing.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    message = "Agreement details not updated successfully.";
                    return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                proposalType = decryptedParameters["ProposalType"].ToString();

                encryptedParameters = null;
                encryptedParameters = Session["EncryptedAgreementType"].ToString().Split('/');

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                agreementType = decryptedParameters["AgreementType"].ToString();


                if (proposalType.Equals("L") || agreementType.Equals("O"))
                {
                    ModelState.Remove("TEND_AMOUNT_YEAR1");
                    ModelState.Remove("TEND_AMOUNT_YEAR2");
                    ModelState.Remove("TEND_AMOUNT_YEAR3");
                    ModelState.Remove("TEND_AMOUNT_YEAR4");
                    ModelState.Remove("TEND_AMOUNT_YEAR5");
                }

                if (ModelState.IsValid)
                {
                    if (agreementBAL.UpdateSpecialAgreementDetailsBAL(details_agreement, ref message))
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
                    return PartialView("ExistingAgreement", details_agreement);
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

        /// <summary>
        /// returns the view for updating the details of proposal
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
                    ExistingSpecialAgreementDetails existingAgreementDetails = agreementBAL.GetSpecialAgreementDetailsBAL_ByAgreementID(Convert.ToInt32(decryptParameters["TendAgreementCode"].ToString()), Convert.ToInt32(decryptParameters["TendAgreementID"].ToString()));
                    if (existingAgreementDetails == null)
                    {
                        ModelState.AddModelError(string.Empty, "Agreement details not exist.");
                        return PartialView("ExistingSpecialAgreement", new ExistingAgreementDetails());
                    }

                    return PartialView("ExistingSpecialAgreement", existingAgreementDetails);
                }
                return PartialView("ExistingSpecialAgreement", new ExistingAgreementDetails());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);

                ModelState.AddModelError(String.Empty, "Agreement details not exist.");
                return PartialView("ExistingSpecialAgreement", new ExistingAgreementDetails());
            }
        }

        /// <summary>
        /// deletes the special agreement details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult DeleteSpecialAgreementDetails(String parameter, String hash, String key)
        {
            bool status = false;
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count() > 0)
                {

                    if (agreementBAL.DeleteAgreementDetailsBAL_ByAgreementID(Convert.ToInt32(decryptedParameters["TendAgreementID"].ToString()), Convert.ToInt32(decryptedParameters["TendAgreementCode"].ToString()), ref message))
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

        #endregion

        #region AGREEMENT_TYPE_UPDATION

        /// <summary>
        /// lists the agreements for status updation
        /// </summary>
        /// <returns></returns>
        public ActionResult ListAgreementsForStatusUpdation()
        {
            try
            {
                AgreementFilterViewModel model = new AgreementFilterViewModel();
                model.lstDistricts = commonFunction.PopulateDistrict(PMGSYSession.Current.StateCode, false);
                model.lstBlocks = commonFunction.PopulateBlocks(PMGSYSession.Current.DistrictCode, true);
                model.lstYears = commonFunction.PopulateFinancialYear(true, true).ToList();
                return View(model);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// returns the list of agreement which are incomplete
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <returns></returns>
        public ActionResult GetAgreementDetailsListForUpdation(int? page, int? rows, string sidx, string sord)
        {
            long totalRecords;
            int stateCode = PMGSYSession.Current.StateCode;
            int districtCode = PMGSYSession.Current.DistrictCode;
            int adminNDCode = PMGSYSession.Current.AdminNdCode;
            int yearCode = 0;
            string package = string.Empty;
            string proposalType = string.Empty;
            string agreementStatus = string.Empty;
            string agreementType = string.Empty;
            string finalize = string.Empty;
            int blockCode = 0;
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["YearCode"]))
                {
                    yearCode = Convert.ToInt32(Request.Params["YearCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["BlockCode"]))
                {
                    blockCode = Convert.ToInt32(Request.Params["BlockCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["Package"]))
                {
                    package = Request.Params["Package"];
                }

                if (!string.IsNullOrEmpty(Request.Params["ProposalType"]))
                {
                    proposalType = Request.Params["ProposalType"];
                }

                if (!string.IsNullOrEmpty(Request.Params["AgreementStatus"]))
                {
                    agreementStatus = Request.Params["AgreementStatus"];
                }

                if (!string.IsNullOrEmpty(Request.Params["AgreementType"]))
                {
                    agreementType = Request.Params["AgreementType"];
                }

                if (!string.IsNullOrEmpty(Request.Params["Finalize"]))
                {
                    finalize = Request.Params["Finalize"];
                }

                if (!string.IsNullOrEmpty(Request.Params["State"]))
                {
                    stateCode = Convert.ToInt32(Request.Params["State"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["District"]))
                {
                    districtCode = Convert.ToInt32(Request.Params["District"]);
                }


                var jsonData = new
                {
                    rows = agreementBAL.GetAgreementListForUpdationBAL(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, yearCode, blockCode, package, proposalType, agreementStatus, finalize, agreementType, stateCode, districtCode, out totalRecords),
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
        /// updates the agreement status to incomplete
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public ActionResult ChangeAgreementStatus(string parameter, string hash, string key)
        {
            int agreementId = 0;
            bool status = false;
            message = "Error occurred while processing your request.";
            try
            {
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { parameter, hash, key });

                if (decryptedParameters.Count > 0)
                {
                    agreementId = Convert.ToInt32(decryptedParameters["TendAgreementId"]);


                    if (agreementBAL.ChangeAgreementStatusBAL(agreementId))
                    {
                        message = "Agreement updated successfully.";
                        status = true;
                    }

                }

                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = status, message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region 01 June 2016
        public ActionResult GetContratorBankNameAccNoAndIFSCcode(string id)
        {
            try
            {
                int ContractorId = 0;
                IAgreementBAL agreementBAL = new AgreementBAL();

                if (!String.IsNullOrEmpty(id))
                {
                    ContractorId = Convert.ToInt32(id.Trim());
                }
                else
                {
                    return null;
                }

                MASTER_CONTRACTOR_BANK contractorBankDetails = agreementBAL.GetContratorBankAccNoAndIFSCcode(ContractorId);

                if (contractorBankDetails != null)
                {
                    string BankAccNumber = contractorBankDetails.MAST_ACCOUNT_NUMBER;
                    string BankIFSCCode = contractorBankDetails.MAST_IFSC_CODE;
                    string BankName = contractorBankDetails.MAST_BANK_NAME;


                    return Json(new { Success = true, BankAccNumber = BankAccNumber, BankIFSCCode = BankIFSCCode, BankName = BankName }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }

        }

        [Audit]
        public String GetContractorSupplierName(string id)
        {
            string name = string.Empty;
            try
            {

                TransactionParams objparams = new TransactionParams();
                int CintractorId = 0;
                if (!String.IsNullOrEmpty(id))
                {
                    CintractorId = Convert.ToInt32(id.Trim());
                    objparams.MAST_CONT_ID = CintractorId;
                }
                else
                {
                    throw new Exception("Exception while getting Contractor/Supplier name");
                }


                name = commonFunction.GetContractorSupplierName(objparams);
                return name;

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                string errorMsg = string.Format("Errors: {0}", ex.Message);
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return string.Empty;
            }


        }

        #endregion


        #region Bank Guarantee  created by pradip patil
        [HttpGet]
        public ActionResult ListAgreementsForBankGuarantee()
        {
            try
            {
                BankGuaranteeModel model = new BankGuaranteeModel();
                model.lstBlocks = commonFunction.PopulateBlocks(PMGSYSession.Current.DistrictCode, true);
                model.lstYears = commonFunction.PopulateFinancialYear(true, true).ToList();
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ListAgreementsForBankGuarantee");
                return null;
            }
        }

        [HttpPost]
        public ActionResult GetAgreementDetailsListForBG(int? page, int? rows, string sidx, string sord)
        {
            long totalRecords;
            int stateCode = PMGSYSession.Current.StateCode;
            int districtCode = PMGSYSession.Current.DistrictCode;
            int adminNDCode = PMGSYSession.Current.AdminNdCode;
            int yearCode = 0;
            string package = string.Empty;
            string proposalType = string.Empty;
            string agreementStatus = string.Empty;
            string agreementType = string.Empty;
            string finalize = string.Empty;
            int blockCode = 0;
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["YearCode"]))
                {
                    yearCode = Convert.ToInt32(Request.Params["YearCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["BlockCode"]))
                {
                    blockCode = Convert.ToInt32(Request.Params["BlockCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["Package"]))
                {
                    package = Request.Params["Package"];
                }

                if (!string.IsNullOrEmpty(Request.Params["ProposalType"]))
                {
                    proposalType = Request.Params["ProposalType"];
                }

                if (!string.IsNullOrEmpty(Request.Params["AgreementStatus"]))
                {
                    agreementStatus = Request.Params["AgreementStatus"];
                }

                if (!string.IsNullOrEmpty(Request.Params["AgreementType"]))
                {
                    agreementType = Request.Params["AgreementType"];
                }

                if (!string.IsNullOrEmpty(Request.Params["Finalize"]))
                {
                    finalize = Request.Params["Finalize"];
                }


                var jsonData = new
                {
                    rows = agreementBAL.GetAgreementListBALForBG(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, yearCode, blockCode, package, proposalType, agreementStatus, finalize, agreementType, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : (totalRecords % Convert.ToInt32(rows) == 0 ? totalRecords / Convert.ToInt32(rows) : totalRecords / Convert.ToInt32(rows) + 1),
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetAgreementDetailsListForBG()");
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpGet]
        public ActionResult AddBankGuarantee()
        {
            try
            {
                BankGuaranteeDetailsModel model = new BankGuaranteeDetailsModel();
                model.Operation = "A";
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AddBankGuarantee");
                return null;
            }

        }
        [HttpGet]
        public ActionResult EditbankGuarantee(string urlparameter)
        {

            try
            {
                string[] encParam = urlparameter.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encParam[0], encParam[1], encParam[2] });

                if (decryptedParameters.Count > 0)
                {
                    BankGuaranteeDetailsModel model = agreementBAL.GetBankGuaranteeObj(decryptedParameters["TendAgreementCode"].ToString());
                    if (model != null)
                        return View("AddBankGuarantee", model);

                }
                return null;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EditbankGuarantee");
                return null;
            }


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditBankGuaranteeDetails(BankGuaranteeDetailsModel model)
        {

            int agreementCode = 0;
            bool status = false;
            string isValidMsg = String.Empty;
            message = "Bank guarantee details not updated.";
            try
            {

                string[] encParam = model.AGREEMENT_CODE.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encParam[0], encParam[1], encParam[2] });

                String Temppath = ConfigurationManager.AppSettings["BANK_GAURANTEE_TEMP"].ToString();
                if (!Directory.Exists(Temppath))
                    Directory.CreateDirectory(Temppath);  //if Directory not created creat it;

                if (model.BGFile == null)
                {
                    ModelState.Remove("BGFile");
                }
                else
                {

                    HttpPostedFileBase postedBgFile = Request.Files[0];
                    int maxSize = 1024 * 1024 * 4;
                    if (!commonFunction.ValidateIsPdf(Temppath, Request))
                    {
                        return Json(new { success = false, file = false, message = "Invalid file. Please upload only pdf files." });
                    }
                    else if (!(postedBgFile.ContentType == "application/pdf"))
                    {
                        return Json(new { success = false, file = false, message = "Invalid file. Please upload only pdf files." });
                    }
                    else if (postedBgFile.FileName.Substring(postedBgFile.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "pdf")
                    {
                        return Json(new { success = false, file = false, message = "Invalid file. Please upload only pdf files." });
                    }

                    if (postedBgFile.ContentLength > maxSize)
                    {
                        return Json(new { success = false, file = false, message = "Invalid file size. Please upload file upto 4 MB." });
                    }

                }


                if (decryptedParameters.Count > 0)
                {
                    agreementCode = Convert.ToInt32(decryptedParameters["TendAgreementCode"]);
                    model.AGREEMENT_CODE = agreementCode.ToString();

                    if (ModelState.IsValid)
                    {
                        status = agreementBAL.EditBankGuaranteeDetails(model, out isValidMsg);

                        return Json(new { success = status, message = isValidMsg });
                    }

                }

                return Json(new { success = status, message = message });
            }
            catch (Exception ex)
            {

                ErrorLog.LogError(ex, "EditBankGuaranteeDetails");
                return Json(new { success = status, message = message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddBankGuaranteeDetails(BankGuaranteeDetailsModel model)
        {
            int agreementCode = 0;
            bool status = false;
            string isValidMsg = String.Empty;
            message = " Bank guarantee  details not added";
            commonFunction = new CommonFunctions();
            try
            {

                string[] encParam = model.AGREEMENT_CODE.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encParam[0], encParam[1], encParam[2] });
                String Temppath = ConfigurationManager.AppSettings["BANK_GAURANTEE_TEMP"].ToString();
                if (!Directory.Exists(Temppath))
                    Directory.CreateDirectory(Temppath);  //if Directory not created creat it;

                if (Request.Files.Count == 0)
                {
                    return Json(new { success = false, file = false, message = "No file selected. Please select file" });
                }
                else
                {
                    HttpPostedFileBase postedBgFile = Request.Files[0];
                    int maxSize = 1024 * 1024 * 4;
                    if (!commonFunction.ValidateIsPdf(Temppath, Request))
                    {
                        return Json(new { success = false, file = false, message = "Invalid file. Please upload only pdf files." });
                    }
                    //else if (!(postedBgFile.ContentType == "application/pdf"))
                    //{
                    //    return Json(new { success = false, file = false, message = "invalid file. Please upload only pdf files." });
                    //}
                    else if (postedBgFile.FileName.Substring(postedBgFile.FileName.LastIndexOf('.') + 1).Trim().ToLower() != "pdf")
                    {
                        return Json(new { success = false, file = false, message = "Invalid file. Please upload only pdf files." });
                    }

                    if (postedBgFile.ContentLength > maxSize)
                    {
                        return Json(new { success = false, file = false, message = "Invalid file size. Please upload file upto 4 MB." });
                    }

                }
                if (decryptedParameters.Count > 0)
                {
                    agreementCode = Convert.ToInt32(decryptedParameters["TendAgreementCode"]);
                    model.AGREEMENT_CODE = agreementCode.ToString();
                    TryValidateModel(model);
                    if (ModelState.IsValid)
                    {
                        status = agreementBAL.AddBankGuaranteeDetails(model, out isValidMsg);

                        return Json(new { success = status, message = isValidMsg });
                    }

                }

                return Json(new { success = status, message = message });
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AddBankGuaranteeDetails");
                return Json(new { success = status, message = message });
            }
        }

        public ActionResult ViewBankGuaranteeDetails(String parameter, String hash, String key)
        {
            Dictionary<string, string> decryptParameters = null;
            try
            {
                decryptParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
                if (decryptParameters.Count > 0)
                {
                    BankGuaranteeDetailsModel agreementDetails = agreementBAL.GetBankGuaranteeObj(decryptParameters["TendAgreementCode"].ToString());

                    return PartialView("ViewBankGuaranteeDetails", agreementDetails);
                }
                return PartialView("ViewBankGuaranteeDetails", new AgreementDetails());

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ViewBankGuaranteeDetails");
                return null;
            }
        }

        [HttpGet]
        public ActionResult ListAgrExprired()
        {
            commonFunction = new CommonFunctions();
            try
            {
                BankGuaranteeModel model = new BankGuaranteeModel();
                model.lstDistricts = commonFunction.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                model.lstDistricts.Where(s => s.Value == "-1").FirstOrDefault().Value = "0";
                model.lstDistricts.Where(s => s.Value == "0").FirstOrDefault().Text = "All District";
                model.lstBlocks = commonFunction.PopulateBlocks(PMGSYSession.Current.DistrictCode, true);
                model.lstYears = commonFunction.PopulateFinancialYear(true, true).ToList();
                return View("ExpriedBankGuarantee", model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ListAgrExprired");
                return null;
            }
        }


        [HttpPost]
        public ActionResult GetExpBankGuaranteeList(int? page, int? rows, string sidx, string sord)
        {
            long totalRecords;
            int stateCode = PMGSYSession.Current.StateCode;
            int districtCode = PMGSYSession.Current.DistrictCode == 0 ? Convert.ToInt32(Request.Params["District"]) : PMGSYSession.Current.DistrictCode;
            int adminNDCode = PMGSYSession.Current.AdminNdCode;
            int yearCode = 0;
            string package = string.Empty;
            string proposalType = string.Empty;
            string agreementStatus = string.Empty;
            string agreementType = string.Empty;
            string finalize = string.Empty;
            int blockCode = 0;
            String ActiveStatus = String.Empty;
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["ActiveStatus"]))
                {
                    ActiveStatus = Request.Params["ActiveStatus"].ToString();
                }

                if (!string.IsNullOrEmpty(Request.Params["YearCode"]))
                {
                    yearCode = Convert.ToInt32(Request.Params["YearCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["BlockCode"]))
                {
                    blockCode = Convert.ToInt32(Request.Params["BlockCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["Package"]))
                {
                    package = Request.Params["Package"];
                }

                if (!string.IsNullOrEmpty(Request.Params["ProposalType"]))
                {
                    proposalType = Request.Params["ProposalType"];
                }

                if (!string.IsNullOrEmpty(Request.Params["AgreementStatus"]))
                {
                    agreementStatus = Request.Params["AgreementStatus"];
                }

                if (!string.IsNullOrEmpty(Request.Params["AgreementType"]))
                {
                    agreementType = Request.Params["AgreementType"];
                }

                if (!string.IsNullOrEmpty(Request.Params["Finalize"]))
                {
                    finalize = Request.Params["Finalize"];
                }


                var jsonData = new
                {
                    rows = agreementBAL.GetExpBankGuaranteeList(Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, yearCode, blockCode, package, proposalType, agreementStatus, finalize, agreementType, districtCode, ActiveStatus, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : (totalRecords % Convert.ToInt32(rows) == 0 ? totalRecords / Convert.ToInt32(rows) : totalRecords / Convert.ToInt32(rows) + 1),
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetExpBankGuaranteeList()");
                return null;
            }
        }

        public JsonResult GetExpiredBankGuaranteeCount()
        {
            int recordCount = agreementBAL.GetExpiredBankGuaranteeCount();
            return Json(new { count = recordCount }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetAgreementDetailsListForContractor(String parameter, String hash, String key, int? page, int? rows, string sidx, string sord)
        {
            long totalRecords;
            int stateCode = PMGSYSession.Current.StateCode;
            int districtCode = PMGSYSession.Current.DistrictCode;
            int adminNDCode = PMGSYSession.Current.AdminNdCode;
            int yearCode = 0;
            string package = string.Empty;
            string proposalType = string.Empty;
            string agreementStatus = string.Empty;
            string agreementType = string.Empty;
            string finalize = string.Empty;
            int blockCode = 0;
            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["YearCode"]))
                {
                    yearCode = Convert.ToInt32(Request.Params["YearCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["BlockCode"]))
                {
                    blockCode = Convert.ToInt32(Request.Params["BlockCode"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["Package"]))
                {
                    package = Request.Params["Package"];
                }

                if (!string.IsNullOrEmpty(Request.Params["ProposalType"]))
                {
                    proposalType = Request.Params["ProposalType"];
                }

                if (!string.IsNullOrEmpty(Request.Params["AgreementStatus"]))
                {
                    agreementStatus = Request.Params["AgreementStatus"];
                }

                if (!string.IsNullOrEmpty(Request.Params["AgreementType"]))
                {
                    agreementType = Request.Params["AgreementType"];
                }

                if (!string.IsNullOrEmpty(Request.Params["Finalize"]))
                {
                    finalize = Request.Params["Finalize"];
                }


                var jsonData = new
                {
                    rows = agreementBAL.GetAgreementDetailsListForContractor(parameter, hash, key, Convert.ToInt32(page) - 1, Convert.ToInt32(rows), sidx, sord, yearCode, blockCode, package, proposalType, agreementStatus, finalize, agreementType, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(rows) ? 1 : (totalRecords % Convert.ToInt32(rows) == 0 ? totalRecords / Convert.ToInt32(rows) : totalRecords / Convert.ToInt32(rows) + 1),
                    page = Convert.ToInt32(page),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetAgreementDetailsListForContractor()");
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }


        public FilePathResult GetBankGuarantee(string id)
        {
            try
            {
                String FileName = String.Empty;
                string[] encParam = id.Split('/');
                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encParam[0], encParam[1], encParam[2] });
                if (decryptedParameters.Count > 0)
                {
                    FileName = decryptedParameters["BGFile"];
                }
                String Path = ConfigurationManager.AppSettings["BANK_GAURANTEE_MAIN"].ToString();

                var cd = new System.Net.Mime.ContentDisposition { FileName = FileName, Inline = false };

                Response.AppendHeader("Content-Disposition", cd.ToString());

                return File(Path + FileName, System.Net.Mime.MediaTypeNames.Application.Pdf);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetBankGuarantee()");
                return null;
            }

        }

        [HttpPost]
        public JsonResult PopulateBlocks()
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            int districtCode = Convert.ToInt32(Request.Params["districtCode"]);

            return Json(objCommonFunctions.PopulateBlocks(districtCode, true));
        }


        //public ActionResult ViewBankGuaranteeUploadDetails(String parameter, String hash, String key,int?rows,int?page, String sidx,String sord)
        //{
        //    Dictionary<string, string> decryptParameters = null;
        //    try
        //    {
        //        decryptParameters = URLEncrypt.DecryptParameters1(new string[] { parameter, hash, key });
        //        if (decryptParameters.Count > 0)
        //        {
        //            int TendAgrCode =Convert.ToInt32( decryptParameters["TendAgreementCode"].ToString());
        //            int TendBGCode = Convert.ToInt32(decryptParameters["TenBGCode"].ToString());

        //        }
        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLog.LogError(ex, "ViewBankGuaranteeUploadDetails");
        //        return null;
        //    }
        //}

        public ActionResult ActionIcon()
        {
            return View();
        }
        #endregion


    }
}
