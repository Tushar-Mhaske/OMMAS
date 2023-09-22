using PMGSY.Areas.ProposalSSRSReports.Models;
using PMGSY.Common;
using PMGSY.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Models;
using PMGSY.Models.ProposalReports;
using PMGSY.BAL.ProposalReports;
using PMGSY.Controllers;

namespace PMGSY.Areas.ProposalSSRSReports.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class ProposalSSRSReportsController : Controller
    {
        //
        // GET: /ProposalSSRSReports/ProposalSSRSReports/

        public ActionResult Index()
        {
            return View();
        }


        #region COMPLETION_PLAN

        public ActionResult CompletionPlanReportLayout()
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                CompletionPlanReportModel model = new CompletionPlanReportModel();
                //if (PMGSYSession.Current.RoleCode == 2)
                //{
                //    model.State = PMGSYSession.Current.StateCode;
                //}
                model.State = 0;
                model.StateName = "All States";

                model.StateList = objCommon.PopulateStates();
                model.YearList = new SelectList(objCommon.PopulateFinancialYear(false, true).ToList(), "Value", "Text").ToList();
                return View(model);
            }
            catch (Exception)
            {
                return null;
            }
        }


        [HttpPost]
        public ActionResult CompletionPlanReport(CompletionPlanReportModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return PartialView(model);
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        #region PHYSICAL_PROGRESS

        public ActionResult PhysicalProgressReportLayout()
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                PhysicalProgressReportModel model = new PhysicalProgressReportModel();
                if (PMGSYSession.Current.RoleCode == 2)
                {
                    model.StateCode = PMGSYSession.Current.StateCode;
                    model.StateName = PMGSYSession.Current.StateName;
                    model.DistrictList = objCommon.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                    model.DistrictList.ElementAt(0).Value = "0";
                    model.BlockList = objCommon.PopulateBlocks(0, true);
                    model.BlockList.ElementAt(0).Value = "0";
                }
                else if (PMGSYSession.Current.RoleCode == 22)
                {
                    model.StateCode = PMGSYSession.Current.StateCode;
                    model.StateName = PMGSYSession.Current.StateName;
                    model.DistrictCode = PMGSYSession.Current.DistrictCode;
                    model.DistrictName = PMGSYSession.Current.DistrictName;
                    model.BlockList = objCommon.PopulateBlocks(PMGSYSession.Current.DistrictCode, true);
                    model.BlockList.ElementAt(0).Value = "0";
                }
                else
                {
                    model.StateList = objCommon.PopulateStates();
                    model.DistrictList = objCommon.PopulateDistrict(0, true);
                    model.DistrictList.ElementAt(0).Value = "0";
                    model.BlockList = objCommon.PopulateBlocks(0, true);
                    model.BlockList.ElementAt(0).Value = "0";
                }

                model.BatchList = objCommon.PopulateBatch(true);
                model.FundingAgencyList = objCommon.PopulateFundingAgency(true);
                model.FundingAgencyList.ElementAt(0).Value = "0";
                model.PhaseYearList = new SelectList(objCommon.PopulateFinancialYear(false, true).ToList(), "Value", "Text").ToList();
                model.PhaseYearList.Find(x => x.Value == "0").Text = "All Years";
                return View(model);
            }
            catch (Exception)
            {
                return null;
            }
        }


        [HttpPost]
        public ActionResult PhysicalProgressReport(PhysicalProgressReportModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return PartialView(model);
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        #region DRRP_ROAD_WISE

        public ActionResult DRRPRoadwiseReportLayout()
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                DRRPRoadwiseReportModel model = new DRRPRoadwiseReportModel();
                if (PMGSYSession.Current.RoleCode == 2)
                {
                    model.StateCode = PMGSYSession.Current.StateCode;
                    model.StateName = PMGSYSession.Current.StateName;
                    model.DistrictList = objCommon.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                    model.DistrictList.ElementAt(0).Value = "0";
                    model.BlockList = objCommon.PopulateBlocks(PMGSYSession.Current.DistrictCode, true);
                    model.BlockList.ElementAt(0).Value = "0";
                }
                else if (PMGSYSession.Current.RoleCode == 22)
                {
                    model.StateCode = PMGSYSession.Current.StateCode;
                    model.StateName = PMGSYSession.Current.StateName;
                    model.DistrictCode = PMGSYSession.Current.DistrictCode;
                    model.DistrictName = PMGSYSession.Current.DistrictName;
                    model.BlockList = objCommon.PopulateBlocks(PMGSYSession.Current.DistrictCode, true);
                    model.BlockList.ElementAt(0).Value = "0";
                }
                else
                {
                    model.StateList = objCommon.PopulateStates();
                    model.DistrictList = objCommon.PopulateDistrict(0, true);
                    model.DistrictList.ElementAt(0).Value = "0";
                    model.BlockList = objCommon.PopulateBlocks(0, true);
                    model.BlockList.ElementAt(0).Value = "0";
                }
                model.CategoryList = objCommon.PopulateDRRPWhetherTypes(true);
                model.CategoryList.Insert(0, new SelectListItem { Text = "All", Value = "%", Selected = true });
                model.SoilTypeList = objCommon.PopulateSoilTypes(true);
                model.TerrainTypeList = objCommon.PopulateTerrainTypes(true);
                return View(model);
            }
            catch (Exception)
            {
                return null;
            }
        }


        [HttpPost]
        public ActionResult DRRPRoadwiseReport(DRRPRoadwiseReportModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return PartialView(model);
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        #region SDCP: DPIU Wise Cumulative Position -- Done
        [HttpGet]
        public ActionResult DPIUWiseCumulativePositionReportLayout()
        {
            DPIUWiseCumulativePositionModel objDPIUWiseCumulativePositionModel = new DPIUWiseCumulativePositionModel();
            return View(objDPIUWiseCumulativePositionModel);
        }

        [HttpPost]
        public ActionResult DPIUWiseCumulativePositionReport(DPIUWiseCumulativePositionModel objDPIUWiseCumulativePositionModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    objDPIUWiseCumulativePositionModel.LevelCode = 1;
                    objDPIUWiseCumulativePositionModel.Mast_State_Code = objDPIUWiseCumulativePositionModel.StateCode > 0 ? objDPIUWiseCumulativePositionModel.StateCode : objDPIUWiseCumulativePositionModel.Mast_State_Code;
                    objDPIUWiseCumulativePositionModel.Mast_District_Code = objDPIUWiseCumulativePositionModel.DistrictCode > 0 ? objDPIUWiseCumulativePositionModel.DistrictCode : objDPIUWiseCumulativePositionModel.Mast_District_Code;
                    //  objDPIUWiseCumulativePositionModel.Mast_Block_Code = objDPIUWiseCumulativePositionModel.BlockCode > 0 ? objDPIUWiseCumulativePositionModel.BlockCode : objDPIUWiseCumulativePositionModel.Mast_Block_Code;

                    return View(objDPIUWiseCumulativePositionModel);
                }
                else
                {
                    return View("DPIUWiseCumulativePositionReportLayout", objDPIUWiseCumulativePositionModel);
                }
            }
            catch
            {
                return View(objDPIUWiseCumulativePositionModel);
            }

        }

        #endregion

        #region SRP: District - Wise Rating  -- Done
        [HttpGet]
        public ActionResult DistrictWiseRatingReportLayout()
        {
            DistrictWiseRatingModel objDistrictWiseRatingModel = new DistrictWiseRatingModel();
            return View(objDistrictWiseRatingModel);
        }

        [HttpPost]
        public ActionResult DistrictWiseRatingReport(DistrictWiseRatingModel objDistrictWiseRatingModel)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    objDistrictWiseRatingModel.LevelCode = objDistrictWiseRatingModel.RoadWise == true ? 4 : objDistrictWiseRatingModel.BlockCode > 0 ? 3 : objDistrictWiseRatingModel.DistrictCode > 0 ? 2 : 1;
                    objDistrictWiseRatingModel.Mast_State_Code = objDistrictWiseRatingModel.StateCode > 0 ? objDistrictWiseRatingModel.StateCode : objDistrictWiseRatingModel.Mast_State_Code;
                    objDistrictWiseRatingModel.Mast_District_Code = objDistrictWiseRatingModel.DistrictCode > 0 ? objDistrictWiseRatingModel.DistrictCode : objDistrictWiseRatingModel.Mast_District_Code;
                    objDistrictWiseRatingModel.Mast_Block_Code = objDistrictWiseRatingModel.BlockCode > 0 ? objDistrictWiseRatingModel.BlockCode : objDistrictWiseRatingModel.Mast_Block_Code;

                    return View(objDistrictWiseRatingModel);
                }
                else
                {
                    return View("DistrictWiseRatingReportLayout", objDistrictWiseRatingModel);
                }
            }
            catch
            {
                return View(objDistrictWiseRatingModel);
            }

        }

        #endregion

        #region Fund Release -- Done
        [HttpGet]
        public ActionResult FundReleaseReportLayout()
        {
            FundReleaseModel objFundReleaseModel = new FundReleaseModel();
            return View(objFundReleaseModel);
        }

        [HttpPost]
        public ActionResult FundReleaseReport(FundReleaseModel objFundReleaseModel)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    objFundReleaseModel.Mast_State_Code = objFundReleaseModel.StateCode > 0 ? objFundReleaseModel.StateCode : objFundReleaseModel.Mast_State_Code;
                    return View(objFundReleaseModel);
                }
                else
                {
                    return View("FundReleaseReportLayout", objFundReleaseModel);
                }
            }
            catch
            {
                return View(objFundReleaseModel);
            }

        }

        #endregion

        #region Fund Allcation -- Done
        [HttpGet]
        public ActionResult FundAllocationReportLayout()
        {
            FundAllocationModel objFundAllocationModel = new FundAllocationModel();
            return View(objFundAllocationModel);
        }

        [HttpPost]
        public ActionResult FundAllocationReport(FundAllocationModel objFundAllocationModel)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    objFundAllocationModel.Mast_State_Code = objFundAllocationModel.StateCode > 0 ? objFundAllocationModel.StateCode : objFundAllocationModel.Mast_State_Code;
                    return View(objFundAllocationModel);
                }
                else
                {
                    return View("FundAllocationReportLayout", objFundAllocationModel);
                }
            }
            catch
            {
                return View(objFundAllocationModel);
            }

        }

        #endregion

        #region Agreement (PMGSY)  -- Done
        [HttpGet]
        public ActionResult AgreementReportLayout()
        {
            AgreementModel objAgreementModel = new AgreementModel();
            return View(objAgreementModel);
        }

        [HttpPost]
        public ActionResult AgreementReport(AgreementModel objAgreementModel)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    objAgreementModel.LevelCode = objAgreementModel.RoadWise == true ? 4 : objAgreementModel.BlockCode > 0 ? 3 : objAgreementModel.DistrictCode > 0 ? 2 : 1;
                    objAgreementModel.Mast_State_Code = objAgreementModel.StateCode > 0 ? objAgreementModel.StateCode : objAgreementModel.Mast_State_Code;
                    objAgreementModel.Mast_District_Code = objAgreementModel.DistrictCode > 0 ? objAgreementModel.DistrictCode : objAgreementModel.Mast_District_Code;
                    objAgreementModel.Mast_Block_Code = objAgreementModel.BlockCode > 0 ? objAgreementModel.BlockCode : objAgreementModel.Mast_Block_Code;

                    return View(objAgreementModel);
                }
                else
                {
                    return View("AgreementReportLayout", objAgreementModel);
                }
            }
            catch
            {
                return View(objAgreementModel);
            }

        }

        #endregion


        #region Proposal List

        [HttpGet]
        public ActionResult ProposalListLayout()
        {
            ProposalListViewModel objProp = new ProposalListViewModel();
            CommonFunctions objCommonFunctions = new CommonFunctions();
            PMGSY.DAL.ProposalReports.ProposalReportsDAL objDAL = new PMGSY.DAL.ProposalReports.ProposalReportsDAL();
            try
            {
                int stateCode = PMGSYSession.Current.StateCode;

                List<SelectListItem> ddYear = objCommonFunctions.PopulateFinancialYear(true, true).ToList();
                List<SelectListItem> ddFundingAgency = objDAL.GetFundingAgencyList();
                List<SelectListItem> ddState = objCommonFunctions.PopulateStates(false);
                SelectListItem all = new SelectListItem
                {
                    Selected = true,
                    Text = "All",
                    Value = "0"
                };
                ddYear.RemoveAt(0);
                ddYear.Insert(0, all);
                ddState.Insert(0, all);

                if (stateCode > 0)  //if state login
                {
                    ddState.Find(x => x.Value == stateCode.ToString()).Selected = true;
                }

                List<SelectListItem> ddProposalStatus = new List<SelectListItem>();
                SelectListItem item = new SelectListItem();

                item = new SelectListItem();
                item.Text = "Sanction Pending";
                item.Value = "N";
                ddProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Dropped Proposal";
                item.Value = "D";
                ddProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Sanctioned Proposals";
                item.Value = "Y";
                ddProposalStatus.Add(item);

                item = new SelectListItem();
                // item.Text = "Un-Sanctioned Proposals";
                item.Text = "Proposal Not Sanctioned";
                item.Value = "U";
                ddProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Reconsider Proposals";
                item.Value = "R";
                ddProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "All";
                item.Value = "%";
                item.Selected = true;
                ddProposalStatus.Add(item);

                List<SelectListItem> ddBatch = objCommonFunctions.PopulateBatch();
                ddBatch.RemoveAt(0);
                ddBatch.Insert(0, all);

                objProp.YearList = new SelectList(ddYear, "Value", "Text").ToList();
                objProp.BatchList = new SelectList(ddBatch, "Value", "Text").ToList();
                objProp.StatusList = new SelectList(ddProposalStatus, "Value", "Text").ToList();
                objProp.CollabList = new SelectList(ddFundingAgency, "Value", "Text").ToList();

                //ViewData["YEAR"] = ddYear;
                //ViewData["BATCH"] = ddBatch;
                //ViewData["AGENCY"] = ddFundingAgency;
                //ViewData["STATUS"] = ddProposalStatus;

                //ViewData["STATE"] = ddState;

                objProp.State = PMGSYSession.Current.StateCode;
                objProp.District = PMGSYSession.Current.DistrictCode;

                objProp.Level = PMGSYSession.Current.RoleCode == 25 ? 1 : PMGSYSession.Current.RoleCode == 2 ? 2 : 3;

                if (PMGSYSession.Current.StateCode == 0)
                {
                    //objProp.Level = 1;
                    objProp.StateName = "All States";
                }
                else
                {
                    //objProp.Level = 2;
                    objProp.StateName = PMGSYSession.Current.StateName;
                }

                if (PMGSYSession.Current.DistrictCode == 0)
                {
                    //objProp.Level = 3;
                    objProp.DistrictName = "All Districts";
                }
                else
                {
                    objProp.DistrictName = PMGSYSession.Current.DistrictName;
                }

                //if (PMGSYSession.Current.blockCode == 0)
                //{
                objProp.BlockName = "All Blocks";
                //}
                //else
                //{
                //    objProp.DistrictName = PMGSYSession.Current.DistrictName;
                //}

                return View(objProp);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPost]
        public ActionResult ProposalListReport(ProposalListViewModel objProp)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    //objProp.State_Code = objProp.StateCode > 0 ? objProp.StateCode : objProp.Mast_State_Code;
                    objProp.Status = objProp.Status == "A" ? "%" : objProp.Status;
                    return View(objProp);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return View("ProposalListLayout", objProp);
                }
            }
            catch
            {
                return View(objProp);
            }

        }

        #endregion

        #region CC BT

        [HttpGet]
        public ActionResult CCBTLengthLayout()
        {
            ProposalListViewModel objProp = new ProposalListViewModel();
            CommonFunctions objCommonFunctions = new CommonFunctions();
            PMGSY.DAL.ProposalReports.ProposalReportsDAL objDAL = new PMGSY.DAL.ProposalReports.ProposalReportsDAL();
            try
            {
                int stateCode = PMGSYSession.Current.StateCode;

                List<SelectListItem> ddYear = objCommonFunctions.PopulateFinancialYear(true, true).ToList();
                List<SelectListItem> ddFundingAgency = objDAL.GetFundingAgencyList();
                List<SelectListItem> ddState = objCommonFunctions.PopulateStates(false);
                SelectListItem all = new SelectListItem
                {
                    Selected = true,
                    Text = "All",
                    Value = "0"
                };
                ddYear.RemoveAt(0);
                ddYear.Insert(0, all);
                ddState.Insert(0, all);

                if (stateCode > 0)  //if state login
                {
                    ddState.Find(x => x.Value == stateCode.ToString()).Selected = true;
                }

                List<SelectListItem> ddProposalStatus = new List<SelectListItem>();
                SelectListItem item = new SelectListItem();

                item = new SelectListItem();
                item.Text = "Sanction Pending";
                item.Value = "N";
                ddProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Dropped Proposal";
                item.Value = "D";
                ddProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Sanctioned Proposals";
                item.Value = "Y";
                ddProposalStatus.Add(item);

                item = new SelectListItem();
                // item.Text = "Un-Sanctioned Proposals";
                item.Text = "Proposal Not Sanctioned";
                item.Value = "U";
                ddProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Reconsider Proposals";
                item.Value = "R";
                ddProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "All";
                item.Value = "A";
                item.Selected = true;
                ddProposalStatus.Add(item);


                List<SelectListItem> ddBatch = objCommonFunctions.PopulateBatch();
                ddBatch.RemoveAt(0);
                ddBatch.Insert(0, all);

                objProp.YearList = new SelectList(ddYear, "Value", "Text").ToList();
                objProp.BatchList = new SelectList(ddBatch, "Value", "Text").ToList();
                objProp.StatusList = new SelectList(ddProposalStatus, "Value", "Text").ToList();
                objProp.CollabList = new SelectList(ddFundingAgency, "Value", "Text").ToList();

                //ViewData["YEAR"] = ddYear;
                //ViewData["BATCH"] = ddBatch;
                //ViewData["AGENCY"] = ddFundingAgency;
                //ViewData["STATUS"] = ddProposalStatus;

                //ViewData["STATE"] = ddState;

                objProp.State = PMGSYSession.Current.StateCode;
                objProp.District = PMGSYSession.Current.DistrictCode;

                objProp.Level = PMGSYSession.Current.RoleCode == 25 ? 1 : PMGSYSession.Current.RoleCode == 2 ? 2 : 3;

                if (PMGSYSession.Current.StateCode == 0)
                {
                    objProp.StateName = "All States";
                }
                else
                {
                    objProp.StateName = PMGSYSession.Current.StateName;
                }

                if (PMGSYSession.Current.DistrictCode == 0)
                {
                    objProp.DistrictName = "All Districts";
                }
                else
                {
                    objProp.DistrictName = PMGSYSession.Current.DistrictName;
                }

                //if (PMGSYSession.Current.blockCode == 0)
                //{
                objProp.BlockName = "All Blocks";
                //}
                //else
                //{
                //    objProp.DistrictName = PMGSYSession.Current.DistrictName;
                //}

                return View(objProp);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPost]
        public ActionResult CCBTLengthReport(ProposalListViewModel objProp)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    //objProp.State_Code = objProp.StateCode > 0 ? objProp.StateCode : objProp.Mast_State_Code;
                    objProp.Status = objProp.Status == "A" ? "%" : objProp.Status;
                    return View(objProp);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return View("CCBTLengthLayout", objProp);
                }
            }
            catch
            {
                return View(objProp);
            }

        }

        #endregion

        #region Proposal Scrutiny

        [HttpGet]
        public ActionResult ProposalScrutinyLayout()
        {
            ProposalScrutinyViewModel objProp = new ProposalScrutinyViewModel();
            CommonFunctions objCommonFunctions = new CommonFunctions();
            //ProposalReportsModel ProposaleportsViewModel = new ProposalReportsModel();
            PMGSY.DAL.ProposalReports.ProposalReportsDAL objDAL = new PMGSY.DAL.ProposalReports.ProposalReportsDAL();
            try
            {
                int stateCode = PMGSYSession.Current.StateCode;

                List<SelectListItem> ddYear = objCommonFunctions.PopulateFinancialYear(true, true).ToList();
                List<SelectListItem> ddScheme = objDAL.GetFundingAgencyList();
                List<SelectListItem> ddState = objCommonFunctions.PopulateStates(false);
                SelectListItem all = new SelectListItem
                {
                    Selected = true,
                    Text = "All",
                    Value = "0"
                };
                ddYear.RemoveAt(0);
                ddYear.Insert(0, all);
                ddState.Insert(0, all);
                if (stateCode > 0)  //if state login
                {
                    ddState.Find(x => x.Value == stateCode.ToString()).Selected = true;
                }


                SelectListItem sta = new SelectListItem
                {
                    Selected = true,
                    Text = "STA",
                    Value = "S"
                };
                SelectListItem pta = new SelectListItem
                {

                    Text = "PTA",
                    Value = "P"
                };

                List<SelectListItem> ddtype = new List<SelectListItem>();
                ddtype.Add(sta);
                ddtype.Add(pta);
                List<SelectListItem> ddAgency = new List<SelectListItem>();
                ddAgency.Add(all);
                List<SelectListItem> ddBatch = objCommonFunctions.PopulateBatch();
                ddBatch.RemoveAt(0);
                ddBatch.Insert(0, all);

                objProp.TechTypeList = new SelectList(ddtype, "Value", "Text").ToList();
                objProp.AgencyList = new SelectList(ddAgency, "Value", "Text").ToList();
                objProp.YearList = new SelectList(ddYear, "Value", "Text").ToList();
                objProp.BatchList = new SelectList(ddBatch, "Value", "Text").ToList();
                objProp.CollabList = new SelectList(ddScheme, "Value", "Text").ToList();
                objProp.StateList = new SelectList(ddState, "Value", "Text").ToList();

                //ViewData["TYPE"] = ddtype;
                //ViewData["AGENCY"] = ddAgency;
                //ViewData["YEAR"] = ddYear;
                //ViewData["BATCH"] = ddBatch;
                //ViewData["SCHEME"] = ddScheme;
                //ViewData["STATE"] = ddState;

                objProp.State = PMGSYSession.Current.StateCode;
                objProp.District = PMGSYSession.Current.DistrictCode;

                //objProp.Agency = 0;
                //return View(ProposaleportsViewModel);

                return View(objProp);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult ProposalScrutinyReport(ProposalScrutinyViewModel objProp)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    objProp.State = PMGSYSession.Current.StateCode > 0 ? PMGSYSession.Current.StateCode : objProp.State;
                    objProp.District = PMGSYSession.Current.DistrictCode > 0 ? PMGSYSession.Current.DistrictCode : objProp.District;

                    objProp.TAName = "%";
                    objProp.DistrictName = PMGSYSession.Current.DistrictCode == 0 ? "All Districts" : PMGSYSession.Current.DistrictName;
                    return View(objProp);
                }
                else
                {
                    return View("ProposalScrutinyLayout", objProp);
                }
            }
            catch
            {
                return View(objProp);
            }

        }

        [HttpPost]
        public JsonResult GetTechAgencyName_ByAgencyType(string type, string stateCode)
        {
            ProposalScrutinyViewModel objProp = new ProposalScrutinyViewModel();
            try
            {
                List<SelectListItem> lstAgency = new List<SelectListItem>();
                PMGSY.DAL.ProposalReports.ProposalReportsDAL objDAL = new PMGSY.DAL.ProposalReports.ProposalReportsDAL();
                int StateCode = Convert.ToInt32(stateCode);
                //lstAgency = objDAL.GetTechAgencyName_ByAgencyType(StateCode, type.Trim());
                objProp.AgencyList = objDAL.GetTechAgencyName_ByAgencyType(StateCode, type.Trim());

                //classTypeList.Insert(0, new SelectListItem { Value = "0", Text = "--All--" });
                return Json(new SelectList(objProp.AgencyList, "Value", "Text"));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(false);
            }
        }

        #endregion

        #region Sanction Proposal

        [HttpGet]
        public ActionResult SanctionProposalLayout()
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            ProposalListViewModel objProp = new ProposalListViewModel();
            PMGSY.DAL.ProposalReports.ProposalReportsDAL objDAL = new PMGSY.DAL.ProposalReports.ProposalReportsDAL();
            try
            {
                int stateCode = PMGSYSession.Current.StateCode;

                List<SelectListItem> ddYear = objCommonFunctions.PopulateFinancialYear(true, true).ToList();
                List<SelectListItem> ddFundingAgency = objDAL.GetFundingAgencyList();
                List<SelectListItem> ddState = objCommonFunctions.PopulateStates(false);
                SelectListItem all = new SelectListItem
                {
                    Selected = true,
                    Text = "All",
                    Value = "0"
                };
                ddYear.RemoveAt(0);
                ddYear.Insert(0, all);
                ddState.Insert(0, all);
                if (stateCode > 0)  //if state login
                {
                    ddState.Find(x => x.Value == stateCode.ToString()).Selected = true;
                }


                List<SelectListItem> ddProposalStatus = new List<SelectListItem>();
                SelectListItem item = new SelectListItem();

                item = new SelectListItem();
                item.Text = "Pending Proposals";
                item.Value = "N";
                ddProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Sanctioned Proposals";
                item.Value = "Y";
                ddProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Un-Sanctioned Proposals";
                item.Value = "U";
                ddProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Recommended Proposals";
                item.Value = "R";
                ddProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Droped Proposal";
                item.Value = "D";
                ddProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "All";
                item.Value = "A";
                item.Selected = true;
                ddProposalStatus.Add(item);


                List<SelectListItem> ddBatch = objCommonFunctions.PopulateBatch();
                ddBatch.RemoveAt(0);
                ddBatch.Insert(0, all);

                objProp.YearList = new SelectList(ddYear, "Value", "Text").ToList();
                objProp.BatchList = new SelectList(ddBatch, "Value", "Text").ToList();
                objProp.CollabList = new SelectList(ddFundingAgency, "Value", "Text").ToList();
                objProp.StatusList = new SelectList(ddProposalStatus, "Value", "Text").ToList();

                //ViewData["YEAR"] = ddYear;
                //ViewData["BATCH"] = ddBatch;
                //ViewData["AGENCY"] = ddFundingAgency;
                //ViewData["STATUS"] = ddProposalStatus;

                //ViewData["STATE"] = ddState;
                objProp.State = PMGSYSession.Current.StateCode;
                objProp.District = PMGSYSession.Current.DistrictCode;
                return View(objProp);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult SanctionProposalReport(ProposalListViewModel objProp)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    //objProp.State_Code = objProp.StateCode > 0 ? objProp.StateCode : objProp.Mast_State_Code;
                    objProp.Level = PMGSYSession.Current.RoleCode == 25 ? 1 : PMGSYSession.Current.RoleCode == 2 ? 2 : 3;
                    objProp.Status = "%";

                    objProp.StateName = PMGSYSession.Current.StateCode == 0 ? "All States" : PMGSYSession.Current.StateName;
                    objProp.DistrictName = PMGSYSession.Current.DistrictCode == 0 ? "All Districts" : PMGSYSession.Current.DistrictName;

                    objProp.BlockName = "All Blocks";

                    return View(objProp);
                }
                else
                {
                    return View("SanctionProposalLayout", objProp);
                }
            }
            catch
            {
                return View(objProp);
            }

        }

        #endregion

        #region Proposal Analysis

        [HttpGet]
        public ActionResult ProposalAnalysisLayout()
        {
            ProposalAnalysisViewModel objProp = new ProposalAnalysisViewModel();
            CommonFunctions objCommonFunctions = new CommonFunctions();
            try
            {
                int stateCode = PMGSYSession.Current.StateCode;

                List<SelectListItem> ddState = objCommonFunctions.PopulateStates(false);
                //SelectListItem allState = new SelectListItem
                //{
                //    Selected = true,
                //    Text = "All",
                //    Value = "0"
                //};
                ddState.Find(x => x.Value == 1.ToString()).Selected = true;
                if (stateCode > 0)  //if state login
                {
                    ddState.Find(x => x.Value == stateCode.ToString()).Selected = true;
                }

                SelectListItem all = new SelectListItem
                {
                    Selected = true,
                    Text = "All",
                    Value = "%"
                };

                SelectListItem pending = new SelectListItem
                {
                    Text = "Pending",
                    Value = "N"
                };
                SelectListItem complete = new SelectListItem
                {
                    Text = "Complete",
                    Value = "Y"
                };


                SelectListItem yes = new SelectListItem
                {
                    Text = "YES",
                    Value = "Y"
                };
                SelectListItem no = new SelectListItem
                {
                    Text = "NO",
                    Value = "N"
                };
                List<SelectListItem> ddscrutiny = new List<SelectListItem>();
                ddscrutiny.Add(all);
                ddscrutiny.Add(pending);
                ddscrutiny.Add(complete);

                List<SelectListItem> lstProposal = new List<SelectListItem>();
                //lstProposal.Insert(0, new SelectListItem { Value = "0", Text = "Both" });
                lstProposal.Insert(0, new SelectListItem { Value = "P", Text = "Road" });
                lstProposal.Insert(1, new SelectListItem { Value = "L", Text = "Bridge" });

                //ViewData["PROPOSAL"] = lstProposal;

                List<SelectListItem> ddsanction = new List<SelectListItem>();
                ddsanction.Add(all);
                ddsanction.Add(yes);
                ddsanction.Add(no);

                objProp.StateList = new SelectList(ddState, "Value", "Text").ToList();
                objProp.ScrutinyList = new SelectList(ddscrutiny, "Value", "Text").ToList();
                objProp.SanctionedList = new SelectList(ddsanction, "Value", "Text").ToList();
                objProp.ProposalList = new SelectList(lstProposal, "Value", "Text").ToList();

                //ViewData["STATE"] = ddState;
                //ViewData["SCRUTINY"] = ddscrutiny;
                //ViewData["SANCTION"] = ddsanction;


                objProp.State = PMGSYSession.Current.StateCode;
                //objProp.District = PMGSYSession.Current.DistrictCode;
                return View(objProp);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult ProposalAnalysisReport(ProposalAnalysisViewModel objProp)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    objProp.State = PMGSYSession.Current.StateCode > 0 ? PMGSYSession.Current.StateCode : objProp.State;

                    //objProp.Status = objProp.Status == "A" ? "%" : objProp.Status;
                    return View(objProp);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return View("ProposalAnalysisLayout", objProp);
                }
            }
            catch
            {
                return View(objProp);
            }

        }

        #endregion

        #region Pending Works Details

        [HttpGet]
        public ActionResult PendingWorksLayout()
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            PendingWorksViewModel objProp = new PendingWorksViewModel();
            try
            {
                int stateCode = PMGSYSession.Current.StateCode;

                List<SelectListItem> ddState = objCommonFunctions.PopulateStates(false);
                SelectListItem allState = new SelectListItem
                {
                    Selected = true,
                    Text = "All",
                    Value = "0"
                };
                ddState.Insert(0, allState);
                if (stateCode > 0)  //if state login
                {
                    ddState.Find(x => x.Value == stateCode.ToString()).Selected = true;
                }

                SelectListItem all = new SelectListItem
                {
                    Selected = true,
                    Text = "All",
                    Value = "%"
                };

                SelectListItem landAcquisition = new SelectListItem
                {
                    Text = "Land Acquisition",
                    Value = "A"
                };
                SelectListItem legalCase = new SelectListItem
                {
                    Text = "Legal Case",
                    Value = "L"
                };
                SelectListItem forestClearence = new SelectListItem
                {
                    Text = "Forest Clearance",
                    Value = "F"
                };

                List<SelectListItem> ddreason = new List<SelectListItem>();
                ddreason.Add(all);
                ddreason.Add(landAcquisition);
                ddreason.Add(legalCase);
                ddreason.Add(forestClearence);

                objProp.StateList = new SelectList(ddState, "Value", "Text").ToList();
                objProp.ReasonList = new SelectList(ddreason, "Value", "Text").ToList();

                //ViewData["REASON"] = ddreason;
                //ViewData["STATE"] = ddState;

                objProp.State = PMGSYSession.Current.StateCode;
                //objProp.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                return View(objProp);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult PendingWorksReport(PendingWorksViewModel objProp)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //objProp.Status = objProp.Status == "A" ? "%" : objProp.Status;
                    if (PMGSYSession.Current.StateCode > 0)
                    {
                        objProp.StateName = PMGSYSession.Current.StateName;
                    }
                    return View(objProp);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return View("PendingWorksLayout", objProp);
                }
            }
            catch
            {
                return View(objProp);
            }

        }
        #endregion

        #region PCI Analysis
        [HttpGet]
        public ActionResult PCIAnalysisLayout()
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            PCIAnalysisViewModel objProp = new PCIAnalysisViewModel();
            try
            {

                objProp.State = PMGSYSession.Current.StateCode;
                objProp.District = PMGSYSession.Current.DistrictCode;

                objProp.StateName = PMGSYSession.Current.StateCode == 0 ? "All States" : PMGSYSession.Current.StateName;
                objProp.DistName = PMGSYSession.Current.DistrictCode == 0 ? "All Districts" : PMGSYSession.Current.DistrictName;
                objProp.BlockName = "All Blocks";

                if (PMGSYSession.Current.LevelId == 6)
                {
                    objProp.flag = "S";
                }
                else if (PMGSYSession.Current.LevelId == 4)
                {
                    objProp.flag = "D";
                }
                else if (PMGSYSession.Current.LevelId == 5)
                {
                    objProp.flag = "B";
                }
                //objProp.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                SelectListItem all = new SelectListItem
                {
                    Selected = true,
                    Text = "All",
                    Value = "%"
                };

                SelectListItem link = new SelectListItem
                {
                    Text = "Link",
                    Value = "L"
                };

                SelectListItem through = new SelectListItem
                {
                    Text = "Through",
                    Value = "T"
                };

                List<SelectListItem> route = new List<SelectListItem>();
                route.Add(all);
                route.Add(link);
                route.Add(through);

                objProp.RouteList = new SelectList(route, "Value", "Text").ToList();

                //ViewData["Route"] = route;
                return View(objProp);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult PCIAnalysisReport(PCIAnalysisViewModel objProp)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //objProp.Status = objProp.Status == "A" ? "%" : objProp.Status;
                    return View(objProp);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return View("PCIAnalysisLayout", objProp);
                }
            }
            catch
            {
                return View(objProp);
            }

        }
        #endregion

        #region Execution Financial Progress Details

        [HttpGet]
        public ActionResult ExecutionFinancialProgressLayout()
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            ExecutionFinancialProgressViewModel objProp = new ExecutionFinancialProgressViewModel();
            PMGSY.DAL.ProposalReports.ProposalReportsDAL objDAL = new PMGSY.DAL.ProposalReports.ProposalReportsDAL();
            try
            {
                int stateCode = PMGSYSession.Current.StateCode;

                List<SelectListItem> ddState = objCommonFunctions.PopulateStates(false);
                List<SelectListItem> ddYear = objCommonFunctions.PopulateFinancialYear(true, true).ToList();
                SelectListItem all = new SelectListItem
                {
                    Selected = true,
                    Text = "All",
                    Value = "0"
                };
                ddYear.RemoveAt(0);
                ddYear.Insert(0, all);

                //SelectListItem selectState = new SelectListItem
                //{
                //    Selected = true,
                //    Text = "Select State",
                //    Value = "0"
                //};
                // ddState.Insert(0, selectState);
                ddState.Find(x => x.Value == "1").Selected = true;

                if (stateCode > 0)  //if state login
                {
                    ddState.Find(x => x.Value == stateCode.ToString()).Selected = true;
                }

                List<SelectListItem> ddcollaboration = objDAL.GetFundingAgencyList();
                // ddcollaboration.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));

                List<SelectListItem> ddDistrict = new List<SelectListItem>();
                ddDistrict.Insert(0, (new SelectListItem { Text = "All District", Value = "0", Selected = true }));

                List<SelectListItem> ddBlock = new List<SelectListItem>();
                ddBlock.Insert(0, (new SelectListItem { Text = "All Block", Value = "0", Selected = true }));

                List<SelectListItem> ddBatch = objCommonFunctions.PopulateBatch();
                ddBatch.RemoveAt(0);
                ddBatch.Insert(0, (new SelectListItem { Text = "All Batches", Value = "0", Selected = true }));

                List<SelectListItem> ddPropType = new List<SelectListItem>();
                ddPropType.Insert(0, (new SelectListItem { Text = "All", Value = "%", Selected = true }));
                ddPropType.Insert(1, (new SelectListItem { Text = "Road", Value = "P" }));
                ddPropType.Insert(2, (new SelectListItem { Text = "Bridge", Value = "L" }));

                objProp.StateList = new SelectList(ddState, "Value", "Text").ToList();
                objProp.DistrictList = new SelectList(ddDistrict, "Value", "Text").ToList();
                objProp.BlockList = new SelectList(ddBlock, "Value", "Text").ToList();
                objProp.YearList = new SelectList(ddYear, "Value", "Text").ToList();
                objProp.BatchList = new SelectList(ddBatch, "Value", "Text").ToList();
                objProp.TypeList = new SelectList(ddPropType, "Value", "Text").ToList();
                objProp.CollabList = new SelectList(ddcollaboration, "Value", "Text").ToList();

                //ViewData["COLLABORATION"] = ddcollaboration;
                //ViewData["YEAR"] = ddYear;
                //ViewData["BATCH"] = ddBatch;
                //ViewData["STATE"] = ddState;
                //ViewData["DISTRICT"] = ddDistrict;
                //ViewData["BLOCK"] = ddBlock;
                //ViewData["TYPE"] = ddPropType;


                objProp.State = PMGSYSession.Current.StateCode;
                objProp.District = PMGSYSession.Current.DistrictCode;
                objProp.Progress = "R";

                return View(objProp);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult ExecutionFinancialProgressReport(ExecutionFinancialProgressViewModel objProp)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //objProp.Status = objProp.Status == "A" ? "%" : objProp.Status;
                    objProp.State = PMGSYSession.Current.StateCode > 0 ? PMGSYSession.Current.StateCode : objProp.State;
                    objProp.District = PMGSYSession.Current.DistrictCode > 0 ? PMGSYSession.Current.DistrictCode : objProp.District;
                    return View(objProp);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    //return View("ExecutionFinancialProgressLayout", objProp);
                    return Json(new { message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return View(objProp);
            }
        }
        #endregion

        #region Maintenance Financial Progress Details

        public ActionResult MaintenanceFinancialProgressLayout()
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            ExecutionFinancialProgressViewModel objProp = new ExecutionFinancialProgressViewModel();
            PMGSY.DAL.ProposalReports.ProposalReportsDAL objDAL = new PMGSY.DAL.ProposalReports.ProposalReportsDAL();
            try
            {
                int stateCode = PMGSYSession.Current.StateCode;

                List<SelectListItem> ddState = objCommonFunctions.PopulateStates(false);
                List<SelectListItem> ddYear = objCommonFunctions.PopulateFinancialYear(true, true).ToList();
                SelectListItem all = new SelectListItem
                {
                    Selected = true,
                    Text = "All",
                    Value = "0"
                };
                ddYear.RemoveAt(0);
                ddYear.Insert(0, all);

                //SelectListItem selectState = new SelectListItem
                //{
                //    Selected = true,
                //    Text = "Select State",
                //    Value = "0"
                //};
                // ddState.Insert(0, selectState);
                ddState.Find(x => x.Value == "1").Selected = true;

                if (stateCode > 0)  //if state login
                {
                    ddState.Find(x => x.Value == stateCode.ToString()).Selected = true;
                }



                List<SelectListItem> ddcollaboration = objDAL.GetFundingAgencyList();
                // ddcollaboration.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));

                List<SelectListItem> ddDistrict = new List<SelectListItem>();
                ddDistrict.Insert(0, (new SelectListItem { Text = "All District", Value = "0", Selected = true }));

                List<SelectListItem> ddBlock = new List<SelectListItem>();
                ddBlock.Insert(0, (new SelectListItem { Text = "All Block", Value = "0", Selected = true }));

                List<SelectListItem> ddBatch = objCommonFunctions.PopulateBatch();
                ddBatch.RemoveAt(0);
                ddBatch.Insert(0, (new SelectListItem { Text = "All Batches", Value = "0", Selected = true }));

                List<SelectListItem> ddPropType = new List<SelectListItem>();
                ddPropType.Insert(0, (new SelectListItem { Text = "All", Value = "%", Selected = true }));
                ddPropType.Insert(1, (new SelectListItem { Text = "Road", Value = "P" }));
                ddPropType.Insert(2, (new SelectListItem { Text = "Bridge", Value = "L" }));


                objProp.StateList = new SelectList(ddState, "Value", "Text").ToList();
                objProp.DistrictList = new SelectList(ddDistrict, "Value", "Text").ToList();
                objProp.BlockList = new SelectList(ddBlock, "Value", "Text").ToList();
                objProp.YearList = new SelectList(ddYear, "Value", "Text").ToList();
                objProp.BatchList = new SelectList(ddBatch, "Value", "Text").ToList();
                objProp.TypeList = new SelectList(ddPropType, "Value", "Text").ToList();
                objProp.CollabList = new SelectList(ddcollaboration, "Value", "Text").ToList();

                //ViewData["COLLABORATION"] = ddcollaboration;
                //ViewData["YEAR"] = ddYear;
                //ViewData["BATCH"] = ddBatch;
                //ViewData["STATE"] = ddState;
                //ViewData["DISTRICT"] = ddDistrict;
                //ViewData["BLOCK"] = ddBlock;
                //ViewData["TYPE"] = ddPropType;

                objProp.State = PMGSYSession.Current.StateCode;
                objProp.District = PMGSYSession.Current.DistrictCode;
                objProp.Progress = "M";

                return View(objProp);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult MaintenanceFinancialProgressReport(ExecutionFinancialProgressViewModel objProp)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    objProp.State = PMGSYSession.Current.StateCode > 0 ? PMGSYSession.Current.StateCode : objProp.State;
                    objProp.District = PMGSYSession.Current.DistrictCode > 0 ? PMGSYSession.Current.DistrictCode : objProp.District;
                    //objProp.Status = objProp.Status == "A" ? "%" : objProp.Status;
                    return View(objProp);
                }
                else
                {
                    return View("MaintenanceFinancialProgressLayout", objProp);
                }
            }
            catch
            {
                string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                return View(objProp);
            }

        }
        #endregion

        #region Maintenance Agreement Details

        public ActionResult MaintenanceAgreementLayout()
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            MaintenanceAgreementViewModel objProp = new MaintenanceAgreementViewModel();
            PMGSY.DAL.ProposalReports.ProposalReportsDAL objDAL = new PMGSY.DAL.ProposalReports.ProposalReportsDAL();
            try
            {
                int stateCode = PMGSYSession.Current.StateCode;

                List<SelectListItem> ddState = objCommonFunctions.PopulateStates(false);
                List<SelectListItem> ddYear = objCommonFunctions.PopulateFinancialYear(true, true).ToList();
                SelectListItem all = new SelectListItem
                {
                    Selected = true,
                    Text = "All",
                    Value = "0"
                };
                ddYear.RemoveAt(0);
                ddYear.Insert(0, all);

                //SelectListItem selectState = new SelectListItem
                //{
                //    Selected = true,
                //    Text = "Select State",
                //    Value = "0"
                //};
                // ddState.Insert(0, selectState);
                ddState.Find(x => x.Value == "1").Selected = true;

                if (stateCode > 0)  //if state login
                {
                    ddState.Find(x => x.Value == stateCode.ToString()).Selected = true;
                }



                List<SelectListItem> ddcollaboration = objDAL.GetFundingAgencyList();
                //ddcollaboration.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));

                List<SelectListItem> ddDistrict = new List<SelectListItem>();
                ddDistrict.Insert(0, (new SelectListItem { Text = "All District", Value = "0", Selected = true }));

                List<SelectListItem> ddBlock = new List<SelectListItem>();
                ddBlock.Insert(0, (new SelectListItem { Text = "All Block", Value = "0", Selected = true }));

                List<SelectListItem> ddBatch = objCommonFunctions.PopulateBatch();
                ddBatch.RemoveAt(0);
                ddBatch.Insert(0, (new SelectListItem { Text = "All Batches", Value = "0", Selected = true }));

                List<SelectListItem> ddProposalStatus = new List<SelectListItem>();
                SelectListItem item = new SelectListItem();

                item = new SelectListItem();
                item.Text = "Sanction Pending";
                item.Value = "N";
                ddProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Dropped Proposal";
                item.Value = "D";
                ddProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Sanctioned Proposals";
                item.Value = "Y";
                ddProposalStatus.Add(item);

                item = new SelectListItem();
                // item.Text = "Un-Sanctioned Proposals";
                item.Text = "Proposal Not Sanctioned";
                item.Value = "U";
                ddProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "Reconsider Proposals";
                item.Value = "R";
                ddProposalStatus.Add(item);

                item = new SelectListItem();
                item.Text = "All";
                item.Value = "%";
                item.Selected = true;
                ddProposalStatus.Add(item);

                objProp.StateList = new SelectList(ddState, "Value", "Text").ToList();
                objProp.DistrictList = new SelectList(ddDistrict, "Value", "Text").ToList();
                objProp.BlockList = new SelectList(ddBlock, "Value", "Text").ToList();
                objProp.YearList = new SelectList(ddYear, "Value", "Text").ToList();
                objProp.BatchList = new SelectList(ddBatch, "Value", "Text").ToList();
                objProp.StatusList = new SelectList(ddProposalStatus, "Value", "Text").ToList();
                objProp.CollabList = new SelectList(ddcollaboration, "Value", "Text").ToList();

                //ViewData["COLLABORATION"] = ddcollaboration;
                //ViewData["YEAR"] = ddYear;
                //ViewData["BATCH"] = ddBatch;
                //ViewData["STATE"] = ddState;
                //ViewData["DISTRICT"] = ddDistrict;
                //ViewData["BLOCK"] = ddBlock;
                //ViewData["STATUS"] = ddProposalStatus;

                objProp.State = PMGSYSession.Current.StateCode;
                objProp.District = PMGSYSession.Current.DistrictCode;

                return View(objProp);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult MaintenanceAgreementReport(MaintenanceAgreementViewModel objProp)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    objProp.State = PMGSYSession.Current.StateCode > 0 ? PMGSYSession.Current.StateCode : objProp.State;
                    objProp.District = PMGSYSession.Current.DistrictCode > 0 ? PMGSYSession.Current.DistrictCode : objProp.District;
                    objProp.Status = "%";
                    //objProp.Status = objProp.Status == "A" ? "%" : objProp.Status;
                    return View(objProp);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return View("MaintenanceAgreementLayout", objProp);
                }
            }
            catch
            {
                return View(objProp);
            }

        }
        #endregion

        #region Maintenance Inspection Details

        public ActionResult MaintenanceInspectionLayout()
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();

            ExecutionFinancialProgressViewModel objProp = new ExecutionFinancialProgressViewModel();

            PMGSY.DAL.ProposalReports.ProposalReportsDAL objDAL = new PMGSY.DAL.ProposalReports.ProposalReportsDAL();
            try
            {
                int stateCode = PMGSYSession.Current.StateCode;

                List<SelectListItem> ddState = objCommonFunctions.PopulateStates(false);
                List<SelectListItem> ddYear = objCommonFunctions.PopulateFinancialYear(true, true).ToList();
                SelectListItem all = new SelectListItem
                {
                    Selected = true,
                    Text = "All",
                    Value = "0"
                };
                ddYear.RemoveAt(0);
                ddYear.Insert(0, all);

                //SelectListItem selectState = new SelectListItem
                //{
                //    Selected = true,
                //    Text = "Select State",
                //    Value = "0"
                //};
                // ddState.Insert(0, selectState);
                ddState.Find(x => x.Value == "1").Selected = true;

                if (stateCode > 0)  //if state login
                {
                    ddState.Find(x => x.Value == stateCode.ToString()).Selected = true;
                }



                List<SelectListItem> ddcollaboration = objDAL.GetFundingAgencyList();
                // ddcollaboration.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));

                List<SelectListItem> ddDistrict = new List<SelectListItem>();
                ddDistrict.Insert(0, (new SelectListItem { Text = "All District", Value = "0", Selected = true }));

                List<SelectListItem> ddBlock = new List<SelectListItem>();
                ddBlock.Insert(0, (new SelectListItem { Text = "All Block", Value = "0", Selected = true }));

                List<SelectListItem> ddBatch = objCommonFunctions.PopulateBatch();
                ddBatch.RemoveAt(0);
                ddBatch.Insert(0, (new SelectListItem { Text = "All Batches", Value = "0", Selected = true }));

                List<SelectListItem> ddPropType = new List<SelectListItem>();
                ddPropType.Insert(0, (new SelectListItem { Text = "All", Value = "%", Selected = true }));
                ddPropType.Insert(1, (new SelectListItem { Text = "Road", Value = "P" }));
                ddPropType.Insert(2, (new SelectListItem { Text = "Bridge", Value = "L" }));

                objProp.StateList = new SelectList(ddState, "Value", "Text").ToList();
                objProp.DistrictList = new SelectList(ddDistrict, "Value", "Text").ToList();
                objProp.BlockList = new SelectList(ddBlock, "Value", "Text").ToList();
                objProp.YearList = new SelectList(ddYear, "Value", "Text").ToList();
                objProp.BatchList = new SelectList(ddBatch, "Value", "Text").ToList();
                objProp.TypeList = new SelectList(ddPropType, "Value", "Text").ToList();
                objProp.CollabList = new SelectList(ddcollaboration, "Value", "Text").ToList();

                //ViewData["COLLABORATION"] = ddcollaboration;
                //ViewData["YEAR"] = ddYear;
                //ViewData["BATCH"] = ddBatch;
                //ViewData["STATE"] = ddState;
                //ViewData["DISTRICT"] = ddDistrict;
                //ViewData["BLOCK"] = ddBlock;
                //ViewData["TYPE"] = ddPropType;

                objProp.State = PMGSYSession.Current.StateCode;
                objProp.District = PMGSYSession.Current.DistrictCode;

                //objProp.Status = "%";

                return View(objProp);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult MaintenanceInspectionReport(ExecutionFinancialProgressViewModel objProp)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    objProp.State = PMGSYSession.Current.StateCode > 0 ? PMGSYSession.Current.StateCode : objProp.State;
                    objProp.District = PMGSYSession.Current.DistrictCode > 0 ? PMGSYSession.Current.DistrictCode : objProp.District;
                    //objProp.Status = objProp.Status == "A" ? "%" : objProp.Status;
                    return View(objProp);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return View("MaintenanceInspectionLayout", objProp);
                }
            }
            catch
            {
                return View(objProp);
            }

        }
        #endregion

        #region PCI Analysis
        [HttpGet]
        public ActionResult PCIPropAnalysisLayout()
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            PCIAnalysisViewModel objProp = new PCIAnalysisViewModel();
            try
            {

                objProp.State = PMGSYSession.Current.StateCode;
                objProp.District = PMGSYSession.Current.DistrictCode;

                objProp.StateName = PMGSYSession.Current.StateCode == 0 ? "All States" : PMGSYSession.Current.StateName;
                objProp.DistName = PMGSYSession.Current.DistrictCode == 0 ? "All Districts" : PMGSYSession.Current.DistrictName;
                objProp.BlockName = "All Blocks";

                if (PMGSYSession.Current.LevelId == 6)
                {
                    objProp.flag = "S";
                }
                else if (PMGSYSession.Current.LevelId == 4)
                {
                    objProp.flag = "D";
                }
                else if (PMGSYSession.Current.LevelId == 5)
                {
                    objProp.flag = "B";
                }
                //objProp.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                SelectListItem all = new SelectListItem
                {
                    Selected = true,
                    Text = "All",
                    Value = "%"
                };

                SelectListItem link = new SelectListItem
                {
                    Text = "Link",
                    Value = "L"
                };

                SelectListItem through = new SelectListItem
                {
                    Text = "Through",
                    Value = "T"
                };

                List<SelectListItem> route = new List<SelectListItem>();
                route.Add(all);
                route.Add(link);
                route.Add(through);

                objProp.RouteList = new SelectList(route, "Value", "Text").ToList();

                //ViewData["Route"] = route;
                return View(objProp);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult PCIPropAnalysisReport(PCIAnalysisViewModel objProp)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //objProp.Status = objProp.Status == "A" ? "%" : objProp.Status;
                    return View(objProp);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return View("PCIAnalysisLayout", objProp);
                }
            }
            catch
            {
                return View(objProp);
            }

        }
        #endregion

        #region Technology Report
        [HttpGet]
        public ActionResult TechnologyLayout()
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            CommonFunctions objCommonFunctions = new CommonFunctions();
            TechnologyViewModel objProp = new TechnologyViewModel();
            PMGSY.DAL.ProposalReports.ProposalReportsDAL objDAL = new PMGSY.DAL.ProposalReports.ProposalReportsDAL();
            try
            {
                objProp.Mast_State_Code = PMGSYSession.Current.StateCode;
                objProp.Mast_District_Code = PMGSYSession.Current.DistrictCode;

                //int stateCode = PMGSYSession.Current.StateCode==0?1:PMGSYSession.Current.StateCode;
                int stateCode = PMGSYSession.Current.StateCode;
                int districtCode = 0;

                List<SelectListItem> ddState = objCommonFunctions.PopulateStates(false);
                List<SelectListItem> ddYear = objCommonFunctions.PopulateFinancialYear(true, true).ToList();
                SelectListItem all = new SelectListItem
                {
                    Selected = true,
                    Text = "All",
                    Value = "0"
                };
                ddYear.RemoveAt(0);
                ddYear.Insert(0, all);

                // ddState.Find(x => x.Value == "1").Selected = true;
                ddState.Insert(0, (new SelectListItem { Text = "All States", Value = "0", Selected = true }));
                if (stateCode > 0)  //if state login
                {
                    objProp.State = stateCode;
                    ddState.Find(x => x.Value == stateCode.ToString()).Selected = true;
                }

                List<SelectListItem> ddcollaboration = objDAL.GetFundingAgencyList();
                //ddcollaboration.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));

                List<SelectListItem> ddDistrict = new List<SelectListItem>();

                List<SelectListItem> ddBlock = new List<SelectListItem>();
                if (stateCode == 0)
                {
                    ddDistrict.Insert(0, (new SelectListItem { Text = "All Districts", Value = "0", Selected = true }));
                }
                else
                {
                    ddDistrict = objCommonFunctions.PopulateDistrict(stateCode, true);
                    districtCode = PMGSYSession.Current.DistrictCode == 0 ? 0 : PMGSYSession.Current.DistrictCode;
                    ddDistrict.Find(x => x.Value == "-1").Value = "0";
                    ddDistrict.Find(x => x.Value == districtCode.ToString()).Selected = true;

                }

                if (districtCode == 0)
                {
                    ddBlock.Insert(0, (new SelectListItem { Text = "All Blocks", Value = "0", Selected = true }));
                }
                else
                {
                    ddBlock = objCommonFunctions.PopulateBlocks(districtCode, true);
                    ddBlock.Find(x => x.Value == "-1").Value = "0";
                    //BlockCode = PMGSYSession.Current.BlockCode == 0 ? 0 : PMGSYSession.Current.BlockCode;
                    //BlockList.Find(x => x.Value == BlockCode.ToString()).Selected = true;
                }

                List<SelectListItem> ddBatch = objCommonFunctions.PopulateBatch();
                ddBatch.RemoveAt(0);
                ddBatch.Insert(0, (new SelectListItem { Text = "All Batches", Value = "0", Selected = true }));

                List<SelectListItem> lstMRDStatus = new List<SelectListItem>();
                lstMRDStatus.Insert(0, new SelectListItem { Value = "%", Text = "All" });
                lstMRDStatus.Insert(1, new SelectListItem { Value = "Y", Text = "Yes" });
                lstMRDStatus.Insert(2, new SelectListItem { Value = "N", Text = "No" });
                objProp.MRDStatusList = new SelectList(lstMRDStatus, "Value", "Text").ToList();

                List<SelectListItem> lstSTAStatus = new List<SelectListItem>();
                lstSTAStatus.Insert(0, new SelectListItem { Value = "%", Text = "All" });
                lstSTAStatus.Insert(1, new SelectListItem { Value = "Y", Text = "Yes" });
                lstSTAStatus.Insert(2, new SelectListItem { Value = "N", Text = "No" });
                objProp.STAStatusList = new SelectList(lstSTAStatus, "Value", "Text").ToList();

                List<SelectListItem> lstConnectivity = new List<SelectListItem>();
                lstConnectivity.Insert(0, new SelectListItem { Value = "%", Text = "All" });
                lstConnectivity.Insert(1, new SelectListItem { Value = "N", Text = "New Connectivity" });
                lstConnectivity.Insert(2, new SelectListItem { Value = "U", Text = "Upgradation" });
                objProp.ConnectivityList = new SelectList(lstConnectivity, "Value", "Text").ToList();

                List<SelectListItem> lstTechType = new List<SelectListItem>();
                lstTechType.Insert(0, new SelectListItem { Value = "%", Text = "All" });
                lstTechType.Insert(1, new SelectListItem { Value = "A", Text = "Accredited" });
                lstTechType.Insert(2, new SelectListItem { Value = "N", Text = "Non Accredited" });
                objProp.AggregatedTypeList = new SelectList(lstTechType, "Value", "Text").ToList();

                List<SelectListItem> lstReportType = new List<SelectListItem>();
                lstReportType.Insert(0, new SelectListItem { Value = "S", Text = "Statewise" });
                lstReportType.Insert(1, new SelectListItem { Value = "T", Text = "Technologywise" });
                objProp.ReportTypeList = new SelectList(lstReportType, "Value", "Text").ToList();

                objProp.StateList = new SelectList(ddState, "Value", "Text").ToList();
                objProp.DistrictList = new SelectList(ddDistrict, "Value", "Text").ToList();
                objProp.BlockList = new SelectList(ddBlock, "Value", "Text").ToList();
                objProp.YearList = new SelectList(ddYear, "Value", "Text").ToList();
                objProp.BatchList = new SelectList(ddBatch, "Value", "Text").ToList();
                objProp.CollabList = new SelectList(ddcollaboration, "Value", "Text").ToList();

                #region
                List<SelectListItem> lstTech = new List<SelectListItem>();

                var list = (from ag in dbContext.MASTER_TECHNOLOGY
                            select new
                            {
                                MAST_TECH_CODE = ag.MAST_TECH_CODE,
                                MAST_TECH_NAME = ag.MAST_TECH_NAME
                            }).Distinct().OrderBy(m => m.MAST_TECH_CODE).ToList();


                lstTech = new SelectList(list.ToList(), "MAST_TECH_CODE", "MAST_TECH_NAME").ToList();
                lstTech.Insert(0, new SelectListItem { Text = "All", Value = "0" });
                objProp.TechList = new SelectList(lstTech, "Value", "Text").ToList();
                #endregion
                //ViewData["COLLABORATION"] = ddcollaboration;
                //ViewData["YEAR"] = ddYear;
                //ViewData["BATCH"] = ddBatch;
                //ViewData["STATE"] = ddState;
                //ViewData["DISTRICT"] = ddDistrict;
                //ViewData["BLOCK"] = ddBlock;
                //ViewData["STATUS"] = ddProposalStatus;

                objProp.State = PMGSYSession.Current.StateCode;
                objProp.District = PMGSYSession.Current.DistrictCode;
                return View(objProp);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
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
        public ActionResult TechnologyReport(TechnologyViewModel objProp)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    objProp.State = PMGSYSession.Current.StateCode > 0 ? PMGSYSession.Current.StateCode : objProp.State;
                    objProp.District = PMGSYSession.Current.DistrictCode > 0 ? PMGSYSession.Current.DistrictCode : objProp.District;
                    //objProp.Status = objProp.Status == "A" ? "%" : objProp.Status;
                    objProp.YearName = objProp.Year == 0 ? "All Years" : Convert.ToString(objProp.Year) + "-" + Convert.ToString(objProp.Year + 1);

                    return View(objProp);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    //return View("TechnologyLayout", objProp);
                    return Json(new { message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return View(objProp);
            }

        }
        #endregion

        #region Fund Sanction Release  Details

        public ActionResult FundSanctionReleaseLayout()
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            FundSanctionReleaseViewModel objProp = new FundSanctionReleaseViewModel();
            PMGSY.DAL.ProposalReports.ProposalReportsDAL objDAL = new PMGSY.DAL.ProposalReports.ProposalReportsDAL();
            try
            {
                int stateCode = PMGSYSession.Current.StateCode;

                List<SelectListItem> ddState = objCommonFunctions.PopulateStates(false);
                List<SelectListItem> ddYear = objCommonFunctions.PopulateFinancialYear(true, true).ToList();
                SelectListItem all = new SelectListItem
                {
                    Selected = true,
                    Text = "All",
                    Value = "0"
                };
                ddYear.RemoveAt(0);
                ddYear.Insert(0, all);

                //SelectListItem selectState = new SelectListItem
                //{
                //    Selected = true,
                //    Text = "Select State",
                //    Value = "0"
                //};
                // ddState.Insert(0, selectState);
                ddState.Find(x => x.Value == "1").Selected = true;

                if (stateCode > 0)  //if state login
                {
                    ddState.Find(x => x.Value == stateCode.ToString()).Selected = true;
                }



                List<SelectListItem> ddcollaboration = objDAL.GetFundingAgencyList();
                // ddcollaboration.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));


                List<SelectListItem> ddAgency = objDAL.GetAgencyList();
                ddAgency.RemoveAt(0);
                ddAgency.Insert(0, (new SelectListItem { Text = "All Agency", Value = "0", Selected = true }));

                List<SelectListItem> ddFUNDTYPE = new List<SelectListItem>();
                ddFUNDTYPE.Insert(0, (new SelectListItem { Text = "All", Value = "%", Selected = true }));
                ddFUNDTYPE.Insert(1, (new SelectListItem { Text = "Program Fund", Value = "P" }));
                ddFUNDTYPE.Insert(2, (new SelectListItem { Text = "Administrative Fund", Value = "A" }));
                ddFUNDTYPE.Insert(3, (new SelectListItem { Text = "Maintenance Fund", Value = "M" }));

                List<SelectListItem> ddReleaseTYPE = new List<SelectListItem>();
                ddReleaseTYPE.Insert(0, (new SelectListItem { Text = "All", Value = "%", Selected = true }));
                ddReleaseTYPE.Insert(1, (new SelectListItem { Text = "Central Government", Value = "C" }));
                ddReleaseTYPE.Insert(2, (new SelectListItem { Text = "State Government", Value = "S" }));


                ViewData["COLLABORATION"] = ddcollaboration;
                ViewData["YEAR"] = ddYear;
                ViewData["AGENCY"] = ddAgency;
                ViewData["STATE"] = ddState;
                ViewData["FUNDTYPE"] = ddFUNDTYPE;
                ViewData["RELEASETYPE"] = ddReleaseTYPE;

                objProp.State = PMGSYSession.Current.StateCode;
                objProp.District = PMGSYSession.Current.DistrictCode;

                return View(objProp);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public ActionResult FundSanctionReleaseReport(FundSanctionReleaseViewModel objProp)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //objProp.Status = objProp.Status == "A" ? "%" : objProp.Status;
                    return View(objProp);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return View("FundSanctionReleaseLayout", objProp);
                }
            }
            catch
            {
                return View(objProp);
            }

        }
        #endregion

        #region PCI Proposal Analysis Details

        public ActionResult PCIProposalAnalysisDetails()
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            PropAnalysisViewModel ProposaleportsViewModel = new PropAnalysisViewModel();
            try
            {

                ProposaleportsViewModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                ProposaleportsViewModel.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                SelectListItem all = new SelectListItem
                {
                    Selected = true,
                    Text = "All",
                    Value = "%"
                };

                SelectListItem link = new SelectListItem
                {
                    Text = "Link",
                    Value = "L"
                };

                SelectListItem through = new SelectListItem
                {
                    Text = "Through",
                    Value = "T"
                };

                List<SelectListItem> route = new List<SelectListItem>();
                route.Add(all);
                route.Add(link);
                route.Add(through);
                ViewData["Route"] = route;
                return View(ProposaleportsViewModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }


        /// <summary>
        /// PCI Abstract StateWise Listing
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PCIProposalAnalysisListing(FormCollection formCollection)
        {
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                //PMGSY.BAL.ProposalReports.ProposalReportsBAL proposalReportsBAL = new PMGSY.BAL.ProposalReports.ProposalReportsBAL();
                PMGSY.Areas.ProposalSSRSReports.BAL.ProposalSSRSBAL proposalReportsBAL = new PMGSY.Areas.ProposalSSRSReports.BAL.ProposalSSRSBAL();
                int stateCode = Convert.ToInt32(formCollection["StateCode"]) == 0 ? PMGSYSession.Current.StateCode : Convert.ToInt32(formCollection["StateCode"]);
                int districtCode = Convert.ToInt32(formCollection["DistrictCode"]) == 0 ? PMGSYSession.Current.DistrictCode : Convert.ToInt32(formCollection["DistrictCode"]);
                int blockCode = Convert.ToInt32(formCollection["BlockCode"]);
                string flag = formCollection["Flag"];
                string routType = formCollection["RouteType"];

                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                int totalRecords;

                var jsonData = new
                {
                    rows = proposalReportsBAL.PCIPropAnalysisListingBAL(stateCode, districtCode, blockCode, flag, routType, page, rows, sidx, sord, out totalRecords),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }


        /// <summary>
        /// PCI Abstract StateWise Chart Listing
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult PCIProposalAnalysisChartListing(FormCollection formCollection)
        {

            try
            {
                //PMGSY.BAL.ProposalReports.ProposalReportsBAL proposalReportsBAL = new PMGSY.BAL.ProposalReports.ProposalReportsBAL();
                PMGSY.Areas.ProposalSSRSReports.BAL.ProposalSSRSBAL proposalReportsBAL = new PMGSY.Areas.ProposalSSRSReports.BAL.ProposalSSRSBAL();
                int stateCode = Convert.ToInt32(formCollection["StateCode"]) == 0 ? PMGSYSession.Current.StateCode : Convert.ToInt32(formCollection["StateCode"]);
                int districtCode = Convert.ToInt32(formCollection["DistrictCode"]) == 0 ? PMGSYSession.Current.DistrictCode : Convert.ToInt32(formCollection["DistrictCode"]);
                int blockCode = Convert.ToInt32(formCollection["BlockCode"]);
                string flag = formCollection["Flag"];
                string routType = formCollection["RouteType"];

                List<USP_CN_PROP_PCI_ANALYSIS_Result> list = proposalReportsBAL.GetPropPCIAnalysisChartBAL(stateCode, districtCode, blockCode, flag, routType);
                List<charModel> lstChart = new List<charModel>();

                decimal totalLen = 0;
                //decimal totalLen1 = 0;
                //decimal totalLen2 = 0; 
                //decimal totalAmt3 = 0;
                //decimal totalAmt4 = 0;
                //decimal totalAmt5 = 0;
                decimal pciNoTotal = 0;
                totalLen = list.Sum(m => (Decimal)m.PCI_TOTAL);
                string status = string.Empty;


                //totalLen1 = list.Sum(x => x.LEN_ONE.HasValue ? x.LEN_ONE.Value : 0);
                //totalLen2 = list.Sum(x => x.LEN_TWO.HasValue ? x.LEN_ONE.Value : 0);
                //totalAmt3 = list.Sum(x => x.LEN_THREE.HasValue ? x.LEN_ONE.Value : 0);
                //totalAmt4 = list.Sum(x => x.LEN_FOUR.HasValue ? x.LEN_ONE.Value : 0);
                //totalAmt5 = list.Sum(x => x.LEN_FIVE.HasValue ? x.LEN_ONE.Value : 0);

                if (totalLen > 0)
                {
                    for (int i = 1; i <= 5; ++i)
                    {
                        switch (i)
                        {
                            case 1:
                                pciNoTotal = list.Sum(x => x.LEN_ONE.HasValue ? x.LEN_ONE.Value : 0);
                                status = "Very Poor";
                                break;
                            case 2:
                                pciNoTotal = list.Sum(x => x.LEN_TWO.HasValue ? x.LEN_TWO.Value : 0);
                                status = "Poor";
                                break;
                            case 3:
                                pciNoTotal = list.Sum(x => x.LEN_THREE.HasValue ? x.LEN_THREE.Value : 0);
                                status = "Fair";
                                break;
                            case 4:
                                pciNoTotal = list.Sum(x => x.LEN_FOUR.HasValue ? x.LEN_FOUR.Value : 0);
                                status = "Good";
                                break;
                            case 5:
                                pciNoTotal = list.Sum(x => x.LEN_FIVE.HasValue ? x.LEN_FIVE.Value : 0);
                                status = "Very Good";
                                break;
                            default:
                                break;
                        }

                        var lenPercent = (pciNoTotal / totalLen) * 100;

                        charModel chart = new charModel();
                        //chart.x = (lenPercent < 10 ? "Very Poor: " + lenPercent : (lenPercent < 15 ? "Poor :" + lenPercent : (lenPercent < 20 ? "Good :" + lenPercent : "Very Good :" + lenPercent)));
                        chart.x = status.ToString() + ": " + lenPercent.ToString();
                        chart.y = pciNoTotal.ToString();
                        chart.z = i.ToString();
                        lstChart.Add(chart);
                    }
                }
                return new JsonResult
                {
                    Data = lstChart
                };

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        #endregion

        #region Common function
        public ActionResult DistrictDetails(FormCollection frmCollection)
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            List<SelectListItem> list = objCommonFunctions.PopulateDistrict(Convert.ToInt32(frmCollection["StateCode"]), true);
            list.Find(x => x.Value == "-1").Value = "0";
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BlockDetails(FormCollection frmCollection)
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            List<SelectListItem> list = objCommonFunctions.PopulateBlocks(Convert.ToInt32(frmCollection["DistrictCode"]), true);
            list.Find(x => x.Value == "-1").Value = "0";
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DistrictSelectDetails(FormCollection frmCollection)
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            List<SelectListItem> list = objCommonFunctions.PopulateDistrict(Convert.ToInt32(frmCollection["StateCode"]), false);
            // list.Find(x => x.Value == "-1").Value = "0";
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BlockSelectDetails(FormCollection frmCollection)
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            List<SelectListItem> list = objCommonFunctions.PopulateBlocks(Convert.ToInt32(frmCollection["DistrictCode"]), false);
            //list.Find(x => x.Value == "-1").Value = "0";
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AllDistrictDetails(FormCollection frmCollection)
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            try
            {
                objCommonFunctions = new CommonFunctions();
                List<SelectListItem> list = objCommonFunctions.PopulateDistrict(Convert.ToInt32(frmCollection["StateCode"]), true);
                // list.Find(x => x.Value == "-1").Value = "0";
                SelectListItem all = new SelectListItem
                {
                    Selected = true,
                    Text = "All District",
                    Value = "0"
                };
                list.RemoveAt(0);
                list.Insert(0, all);
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        public ActionResult AllBlockDetails(FormCollection frmCollection)
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            try
            {
                objCommonFunctions = new CommonFunctions();
                List<SelectListItem> list = objCommonFunctions.PopulateBlocks(Convert.ToInt32(frmCollection["DistrictCode"]), true);
                // list.Find(x => x.Value == "-1").Value = "0";
                SelectListItem all = new SelectListItem
                {
                    Selected = true,
                    Text = "All Block",
                    Value = "0"
                };
                list.RemoveAt(0);
                list.Insert(0, all);
                return Json(list, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        [HttpPost]
        public JsonResult PopulateCollaborations()
        {
            try
            {
                CommonFunctions objCommonFunctions = new CommonFunctions();
                int stateCode = Convert.ToInt32(Request.Params["stateCode"]);

                return Json(PopulateCollaborationsStateWise(stateCode, true));
            }
            catch
            {
                return Json(new { string.Empty });
            }
        }

        public List<SelectListItem> PopulateCollaborationsStateWise(int stateCode, bool isAllSelected = false)
        {
            List<SelectListItem> lstCollaborations = new List<SelectListItem>();
            SelectListItem item;
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                dbContext = new PMGSYEntities();

                if (isAllSelected == false)
                {
                    lstCollaborations.Insert(0, (new SelectListItem { Text = "Select Collaboration", Value = "-1", Selected = true }));
                }
                else if (isAllSelected == true)
                {
                    lstCollaborations.Insert(0, (new SelectListItem { Text = "All Collaborations", Value = "0", Selected = true }));
                }

                var query = (from ms in dbContext.MASTER_FUNDING_AGENCY
                             join isp in dbContext.IMS_SANCTIONED_PROJECTS on ms.MAST_FUNDING_AGENCY_CODE equals isp.IMS_COLLABORATION
                             where isp.MAST_STATE_CODE == stateCode
                             select new
                             {
                                 Text = ms.MAST_FUNDING_AGENCY_NAME,
                                 Value = ms.MAST_FUNDING_AGENCY_CODE
                             }).Distinct().OrderBy(c => c.Text).ToList();

                foreach (var data in query)
                {
                    item = new SelectListItem();
                    item.Text = data.Text;
                    item.Value = data.Value.ToString();
                    lstCollaborations.Add(item);
                }



                return lstCollaborations;
            }
            catch
            {
                return lstCollaborations;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion


        #region Wrongly Mapped habitations
        public ActionResult WronglyMappedHabitationsLayout()
        {
            CommonFunctions commonFunctions = new CommonFunctions();
            WronglyMappedHabitaions qmDetails = new WronglyMappedHabitaions();

            if (PMGSYSession.Current.RoleCode == 2)
            {
                qmDetails.lstStates = new List<SelectListItem>();


                qmDetails.lstStates.Insert(0, (new SelectListItem { Text = PMGSYSession.Current.StateName, Value = PMGSYSession.Current.StateCode.ToString(), Selected = true }));
            }
            else
            {
                qmDetails.lstStates = commonFunctions.PopulateStates(false);
              //  qmDetails.lstStates.Insert(0, (new SelectListItem { Text = "Select State", Value = "0", Selected = true }));
            }
            return View(qmDetails);
        }
        [HttpPost]
        public ActionResult WronglyMappedHabitationsReport(WronglyMappedHabitaions model)
        {
            try
            {
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ProposalSSRSReportsController.WronglyMappedHabitationsReport()");
                return null;
            }
           
        }
        #endregion

        #region Proposal without Uploads

        public ActionResult ProposalsWithoutUploadsReportLayout()
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                ProposalsWithoutUploads model = new ProposalsWithoutUploads();
                if (PMGSYSession.Current.StateCode > 0)
                {
                    model.State = PMGSYSession.Current.StateCode;
                    model.StateName = PMGSYSession.Current.StateName;
                    model.lstDistrict = objCommon.PopulateDistrict(PMGSYSession.Current.StateCode, false);
                    model.lstDistrict.Find(x => x.Value == "0").Text = "All Districts";
                    model.DistrictName = PMGSYSession.Current.DistrictName;
                }
                else if (PMGSYSession.Current.DistrictCode > 0)
                {
                    model.State = PMGSYSession.Current.StateCode;
                    model.StateName = PMGSYSession.Current.StateName;
                    model.District = PMGSYSession.Current.DistrictCode;
                    model.DistrictName = PMGSYSession.Current.DistrictName;
                }
                else if (PMGSYSession.Current.StateCode == 0)
                {
                    model.lstStates = objCommon.PopulateStates(true);
                    model.lstStates.Find(x => x.Value == "0").Value = "-1";
                    model.lstDistrict = objCommon.PopulateDistrict(PMGSYSession.Current.StateCode, false);
                    model.lstDistrict.Find(x => x.Value == "0").Text = "All Districts";
                }
                model.YearList = objCommon.PopulateFinancialYear(true, false).ToList();
                model.YearList.Find(x => x.Value == "0").Value = "-1";

                model.BatchList = objCommon.PopulateBatch();
                model.BatchList.Find(x => x.Value == "0").Text = "All Batches";

                model.TypeList = new List<SelectListItem>();
                SelectListItem item = new SelectListItem();

                item = new SelectListItem();
                item.Text = "Images";
                item.Value = "I";
                model.TypeList.Add(item);

                item = new SelectListItem();
                item.Text = "C Proforma";
                item.Value = "C";
                model.TypeList.Add(item);

                return View(model);
            }
            catch (Exception)
            {
                return null;
            }
        }


        [HttpPost]
        public ActionResult ProposalsWithoutUploadsReport(ProposalsWithoutUploads model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.State = PMGSYSession.Current.StateCode > 0 ? PMGSYSession.Current.StateCode : model.State;
                    model.District = PMGSYSession.Current.DistrictCode > 0 ? PMGSYSession.Current.DistrictCode : model.District;
                    model.PIUCode = PMGSYSession.Current.DistrictCode > 0 ? PMGSYSession.Current.AdminNdCode : 0;
                    return PartialView(model);
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public ActionResult DistrictByStateCode(FormCollection frmCollection)
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            List<SelectListItem> list = objCommonFunctions.PopulateDistrict(Convert.ToInt32(frmCollection["StateCode"]), false);
            list.RemoveAt(0);
            //list.Find(x => x.Value == "0").Value = "-1";
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region WorksSanctionedCompletedLayout
        [HttpGet]
        public ActionResult WorksSanctionedCompletedLayout()
        {
            CommonFunctions commonFunctions = new CommonFunctions();
            try
            {
                WorksSanctionedCompletedViewModel model = new WorksSanctionedCompletedViewModel();

                model.YearList = commonFunctions.PopulateYears(System.DateTime.Now.Year, false);
                model.MonthList = commonFunctions.PopulateMonths(System.DateTime.Now.Month, false);
                model.Month = DateTime.Now.Month;
                model.Year = DateTime.Now.Year;
                return View(model);
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult WorksSanctionedCompletedReport(WorksSanctionedCompletedViewModel model)
        {
            try
            {
                return View(model);
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region DRRP_ROAD

        public ActionResult DRRPRoadReportLayout()
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                DRRPRoadwiseReportModel model = new DRRPRoadwiseReportModel();
                if (PMGSYSession.Current.RoleCode == 2)
                {
                    model.StateCode = PMGSYSession.Current.StateCode;
                    model.StateName = PMGSYSession.Current.StateName;
                    model.DistrictList = objCommon.PopulateDistrict(PMGSYSession.Current.StateCode, true);
                    model.DistrictList.ElementAt(0).Value = "0";
                }
                else if (PMGSYSession.Current.RoleCode == 22)
                {
                    model.StateCode = PMGSYSession.Current.StateCode;
                    model.StateName = PMGSYSession.Current.StateName;
                    model.DistrictCode = PMGSYSession.Current.DistrictCode;
                    model.DistrictName = PMGSYSession.Current.DistrictName;
                }
                else
                {
                    model.StateList = objCommon.PopulateStates();
                    model.DistrictList = objCommon.PopulateDistrict(0, true);
                    model.DistrictList.ElementAt(0).Value = "0";
                    model.StateList.ElementAt(0).Text = "All States";
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DRRPRoadReportLayout");
                return null;
            }
        }


        [HttpPost]
        public ActionResult DRRPRoadReport(DRRPRoadwiseReportModel model)
        {
            ModelState.Remove("CategoryCode");
            try
            {
                if (ModelState.IsValid)
                {
                    PMGSYEntities dbcontext = new PMGSYEntities();

                    model.StateName = model.StateCode == 0 ? "All States" : dbcontext.MASTER_STATE.Where(x => x.MAST_STATE_CODE == model.StateCode).First().MAST_STATE_NAME;
                    model.DistrictName = model.DistrictCode == 0 ? "All Districts" : dbcontext.MASTER_DISTRICT.Where(x => x.MAST_DISTRICT_CODE == model.DistrictCode).First().MAST_DISTRICT_NAME;
                    model.BlockName = model.BlockCode == 0 ? "All Blocks" : dbcontext.MASTER_BLOCK.Where(x => x.MAST_BLOCK_CODE == model.BlockCode).First().MAST_BLOCK_NAME;

                    return PartialView(model);
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DRRPRoadReport");
                return null;
            }
        }

        #endregion

        #region Download Sanctioned Road List
        public ActionResult SanctionedRoadListLayout()
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                SanctionedRoadListViewModel model = new SanctionedRoadListViewModel();
                if (PMGSYSession.Current.StateCode > 0)
                {
                    model.stateCode = PMGSYSession.Current.StateCode;
                    model.lstState = new List<SelectListItem>();
                    model.lstState.Insert(0, new SelectListItem() { Text = PMGSYSession.Current.StateName.Trim(), Value = PMGSYSession.Current.StateCode.ToString() });
                }
                else
                {
                    model.lstState = objCommon.PopulateStates(false);
                    model.lstState.Find(x => x.Value == "0").Value = "-1";
                    model.lstState.Find(x => x.Value == "-1").Text = "Select State";
                }

                model.lstPmgsyScheme = new List<SelectListItem>();
                model.lstPmgsyScheme.Insert(0, new SelectListItem() { Text = "All", Value = "0" });
                model.lstPmgsyScheme.Insert(0, new SelectListItem() { Text = "Pmgsy-I", Value = "1" });
                model.lstPmgsyScheme.Insert(0, new SelectListItem() { Text = "Pmgsy-II", Value = "2" });
                model.lstPmgsyScheme.Insert(0, new SelectListItem() { Text = "RCPLWE", Value = "3" });

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Areas.ProposalSSRS.SanctionedRoadListLayout");
                return null;
            }
        }


        [HttpPost]
        public ActionResult SanctionedRoadListReport(SanctionedRoadListViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    PMGSYEntities dbcontext = new PMGSYEntities();

                    //model.StateName = model.StateCode == 0 ? "All States" : dbcontext.MASTER_STATE.Where(x => x.MAST_STATE_CODE == model.StateCode).First().MAST_STATE_NAME;
                    //model.DistrictName = model.DistrictCode == 0 ? "All Districts" : dbcontext.MASTER_DISTRICT.Where(x => x.MAST_DISTRICT_CODE == model.DistrictCode).First().MAST_DISTRICT_NAME;
                    //model.BlockName = model.BlockCode == 0 ? "All Blocks" : dbcontext.MASTER_BLOCK.Where(x => x.MAST_BLOCK_CODE == model.BlockCode).First().MAST_BLOCK_NAME;

                    return PartialView(model);
                }
                else
                {
                    return Json(new { success = false, message = new CommonFunctions().FormatErrorMessage(ModelState) });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Areas.ProposalSSRS.SanctionedRoadListReport");
                return null;
            }
        }
        #endregion

        #region Agreement Abstract Report
        [HttpGet]
        public ActionResult AgreementAbstractLayout()
        {
            AgreementAbstractViewModel model = new AgreementAbstractViewModel();
            CommonFunctions comm = new CommonFunctions();
            try
            {
                model.lstMonth = comm.PopulateMonths(true);
                model.lstYear = comm.PopulateYears(true);
                model.lstPmgsyScheme = new List<SelectListItem>();
                model.lstPmgsyScheme.Insert(0, new SelectListItem() { Value = "0", Text = "All" });
                model.lstPmgsyScheme.Insert(1, new SelectListItem() { Value = "1", Text = "PMGSY-I" });
                model.lstPmgsyScheme.Insert(2, new SelectListItem() { Value = "2", Text = "PMGSY-II" });
                model.lstPmgsyScheme.Insert(3, new SelectListItem() { Value = "3", Text = "RCPLWE" });

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PFMSReports.BeneficiaryDetailsLayout()");
                return Json(new { message = "Error occured while processing your request" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult AgreementAbstractReport(AgreementAbstractViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return View(model);
                }
                else
                {
                    string message = ModelState.Values.Where(x => x.Errors.Count == 1).FirstOrDefault().Errors[0].ErrorMessage;
                    return Json(new { message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PFMSReports.BeneficiaryDetailsReport()");
                return Json(new { message = "Error occured while processing your request" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}
