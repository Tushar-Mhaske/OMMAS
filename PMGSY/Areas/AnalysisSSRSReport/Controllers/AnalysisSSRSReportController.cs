using PMGSY.Areas.AnalysisSSRSReport.Models;
using PMGSY.Common;
using PMGSY.Controllers;
using PMGSY.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.AnalysisSSRSReport.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class AnalysisSSRSReportController : Controller
    {
        //
        // GET: /AnalysisSSRSReport/AnalysisSSRS/
        CommonFunctions commonFunctions;

        public ActionResult Index()
        {
            return View();
        }

        #region 1 Analysis of Data From Proposals -- Done
        [HttpGet]
        public ActionResult AnalysisDataProposalReportLayout()
        {
            AnalysisDataProposalModel objAnalysisDataProposalModel = new AnalysisDataProposalModel();
            return View(objAnalysisDataProposalModel);
        }

        [HttpPost]
        public ActionResult AnalysisDataProposalReport(AnalysisDataProposalModel objAnalysisDataProposalModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    objAnalysisDataProposalModel.LevelCode = objAnalysisDataProposalModel.RoadWise == true ? 4 : objAnalysisDataProposalModel.BlockCode > 0 ? 3 : objAnalysisDataProposalModel.DistrictCode > 0 ? 2 : 1;
                    objAnalysisDataProposalModel.Mast_State_Code = objAnalysisDataProposalModel.StateCode > 0 ? objAnalysisDataProposalModel.StateCode : objAnalysisDataProposalModel.Mast_State_Code;
                    objAnalysisDataProposalModel.Mast_District_Code = objAnalysisDataProposalModel.DistrictCode > 0 ? objAnalysisDataProposalModel.DistrictCode : objAnalysisDataProposalModel.Mast_District_Code;
                    objAnalysisDataProposalModel.Mast_Block_Code = objAnalysisDataProposalModel.BlockCode > 0 ? objAnalysisDataProposalModel.BlockCode : objAnalysisDataProposalModel.Mast_Block_Code;

                    return View(objAnalysisDataProposalModel);
                }
                else
                {
                    return View("AnalysisDataProposalReportLayout", objAnalysisDataProposalModel);
                }
            }
            catch
            {
                return View(objAnalysisDataProposalModel);
            }

        }
        #endregion


        #region 2 Analysis for Proposal --Done
        [HttpGet]
        public ActionResult AnalysisProposalReportLayout()
        {
            AnalysisProposalModel objAnalysisProposalModel = new AnalysisProposalModel();
            objAnalysisProposalModel.ReportName = "G";
            objAnalysisProposalModel.RoadWise = false;
            return View(objAnalysisProposalModel);
        }

        [HttpPost]
        public ActionResult AnalysisProposalReport(AnalysisProposalModel objAnalysisProposalModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    objAnalysisProposalModel.LevelCode = objAnalysisProposalModel.RoadWise == true ? 4 : objAnalysisProposalModel.BlockCode > 0 ? 3 : objAnalysisProposalModel.DistrictCode > 0 ? 2 : 1;
                    objAnalysisProposalModel.Mast_State_Code = objAnalysisProposalModel.StateCode > 0 ? objAnalysisProposalModel.StateCode : objAnalysisProposalModel.Mast_State_Code;
                    objAnalysisProposalModel.Mast_District_Code = objAnalysisProposalModel.DistrictCode > 0 ? objAnalysisProposalModel.DistrictCode : objAnalysisProposalModel.Mast_District_Code;
                    objAnalysisProposalModel.Mast_Block_Code = objAnalysisProposalModel.BlockCode > 0 ? objAnalysisProposalModel.BlockCode : objAnalysisProposalModel.Mast_Block_Code;

                    return View(objAnalysisProposalModel);
                }
                else
                {
                    return View("AnalysisProposalReportLayout", objAnalysisProposalModel);
                }
            }
            catch
            {
                return View(objAnalysisProposalModel);
            }

        }
        #endregion

        #region 3 Subbgrade Soil Pattern -- Done
        [HttpGet]
        public ActionResult AnalysisSubgradeSoilPatternReportLayout()
        {
            AnalysisSubgradeSoilModel objSubgradeSoilModel = new AnalysisSubgradeSoilModel();
            commonFunctions = new CommonFunctions();
            if (!(string.IsNullOrEmpty(Request.Params["SubReport"])))
            {
                ViewBag.ReportType = Request.Params["SubReport"];
            }
            if (!(string.IsNullOrEmpty(Request.Params["State"])))
            {
                objSubgradeSoilModel.StateCode = Convert.ToInt32(Request.Params["State"]);
            }
            if (!(string.IsNullOrEmpty(Request.Params["District"])))
            {
                if (objSubgradeSoilModel.StateCode > 0)
                {
                    objSubgradeSoilModel.DistrictList = new List<SelectListItem>();
                    objSubgradeSoilModel.DistrictList = commonFunctions.PopulateDistrict(objSubgradeSoilModel.StateCode, true);
                    objSubgradeSoilModel.DistrictList.Find(x => x.Value == "-1").Value = "0";
                }
                objSubgradeSoilModel.DistrictCode = Convert.ToInt32(Request.Params["District"]);
            }
            if (!(string.IsNullOrEmpty(Request.Params["Block"])))
            {
                if (objSubgradeSoilModel.DistrictCode > 0)
                {
                    objSubgradeSoilModel.BlockList = new List<SelectListItem>();
                    objSubgradeSoilModel.BlockList = commonFunctions.PopulateBlocks(objSubgradeSoilModel.DistrictCode, true);
                    objSubgradeSoilModel.BlockList.Find(x => x.Value == "-1").Value = "0";

                }
                objSubgradeSoilModel.BlockCode = Convert.ToInt32(Request.Params["Block"]);
            }
            if (!(string.IsNullOrEmpty(Request.Params["Collaboration"])))
            {
                objSubgradeSoilModel.FundingAgency = Convert.ToInt32(Request.Params["Collaboration"]);
            }
            if (!(string.IsNullOrEmpty(Request.Params["Year"])))
            {
                objSubgradeSoilModel.FromYear = Convert.ToInt32(Request.Params["Year"]);
                objSubgradeSoilModel.ToYear = Convert.ToInt32(Request.Params["Year"]);
            }
            if (!(string.IsNullOrEmpty(Request.Params["Batch"])))
            {
                objSubgradeSoilModel.Batch = Convert.ToInt32(Request.Params["Batch"]);
            }
            if (!(string.IsNullOrEmpty(Request.Params["Status"])))
            {
                objSubgradeSoilModel.Status = Request.Params["Status"].ToString();
            }
            return View(objSubgradeSoilModel);
        }

        [HttpPost]
        public ActionResult AnalysisSubgradeSoilPatternReport(AnalysisSubgradeSoilModel objSubgradeSoilModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    objSubgradeSoilModel.LevelCode = objSubgradeSoilModel.RoadWise == true ? 4 : objSubgradeSoilModel.BlockCode > 0 ? 3 : objSubgradeSoilModel.DistrictCode > 0 ? 2 : 1;
                    objSubgradeSoilModel.Mast_State_Code = objSubgradeSoilModel.StateCode > 0 ? objSubgradeSoilModel.StateCode : objSubgradeSoilModel.Mast_State_Code;
                    objSubgradeSoilModel.Mast_District_Code = objSubgradeSoilModel.DistrictCode > 0 ? objSubgradeSoilModel.DistrictCode : objSubgradeSoilModel.Mast_District_Code;
                    objSubgradeSoilModel.Mast_Block_Code = objSubgradeSoilModel.BlockCode > 0 ? objSubgradeSoilModel.BlockCode : objSubgradeSoilModel.Mast_Block_Code;

                    return View(objSubgradeSoilModel);
                }
                else
                {
                    return View("AnalysisSubgradeSoilPatternReportLayout", objSubgradeSoilModel);
                }
            }
            catch
            {
                return View(objSubgradeSoilModel);
            }

        }
        #endregion

        #region 4 Average Pavement Cost --Done
        [HttpGet]
        public ActionResult AveragePavementCostReportLayout()
        {
            AveragePavmentCostModel objAveragePavmentCostModel = new AveragePavmentCostModel();
            commonFunctions = new CommonFunctions();
            if (!(string.IsNullOrEmpty(Request.Params["SubReport"])))
            {
                ViewBag.ReportType = Request.Params["SubReport"];
            }
            if (!(string.IsNullOrEmpty(Request.Params["State"])))
            {
                objAveragePavmentCostModel.StateCode = Convert.ToInt32(Request.Params["State"]);
            }
            if (!(string.IsNullOrEmpty(Request.Params["District"])))
            {
                if (objAveragePavmentCostModel.StateCode > 0)
                {
                    objAveragePavmentCostModel.DistrictList = new List<SelectListItem>();
                    objAveragePavmentCostModel.DistrictList = commonFunctions.PopulateDistrict(objAveragePavmentCostModel.StateCode, true);
                    objAveragePavmentCostModel.DistrictList.Find(x => x.Value == "-1").Value = "0";

                }
                objAveragePavmentCostModel.DistrictCode = Convert.ToInt32(Request.Params["District"]);
            }
            if (!(string.IsNullOrEmpty(Request.Params["Block"])))
            {
                if (objAveragePavmentCostModel.DistrictCode > 0)
                {
                    objAveragePavmentCostModel.BlockList = new List<SelectListItem>();
                    objAveragePavmentCostModel.BlockList = commonFunctions.PopulateBlocks(objAveragePavmentCostModel.DistrictCode, true);
                    objAveragePavmentCostModel.BlockList.Find(x => x.Value == "-1").Value = "0";

                }
                objAveragePavmentCostModel.BlockCode = Convert.ToInt32(Request.Params["Block"]);
            }
            if (!(string.IsNullOrEmpty(Request.Params["Collaboration"])))
            {
                objAveragePavmentCostModel.FundingAgency = Convert.ToInt32(Request.Params["Collaboration"]);
            }
            if (!(string.IsNullOrEmpty(Request.Params["Year"])))
            {
                objAveragePavmentCostModel.FromYear = Convert.ToInt32(Request.Params["Year"]);
                objAveragePavmentCostModel.ToYear = Convert.ToInt32(Request.Params["Year"]);
            }
            if (!(string.IsNullOrEmpty(Request.Params["Batch"])))
            {
                objAveragePavmentCostModel.Batch = Convert.ToInt32(Request.Params["Batch"]);
            }
            if (!(string.IsNullOrEmpty(Request.Params["Status"])))
            {
                objAveragePavmentCostModel.Status = Request.Params["Status"].ToString();
            }
     
            return View(objAveragePavmentCostModel);
        }

        [HttpPost]
        public ActionResult AveragePavementCostReport(AveragePavmentCostModel objAveragePavmentCostModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    objAveragePavmentCostModel.LevelCode = objAveragePavmentCostModel.RoadWise == true ? 4 : objAveragePavmentCostModel.BlockCode > 0 ? 3 : objAveragePavmentCostModel.DistrictCode > 0 ? 2 : 1;
                    objAveragePavmentCostModel.Mast_State_Code = objAveragePavmentCostModel.StateCode > 0 ? objAveragePavmentCostModel.StateCode : objAveragePavmentCostModel.Mast_State_Code;
                    objAveragePavmentCostModel.Mast_District_Code = objAveragePavmentCostModel.DistrictCode > 0 ? objAveragePavmentCostModel.DistrictCode : objAveragePavmentCostModel.Mast_District_Code;
                    objAveragePavmentCostModel.Mast_Block_Code = objAveragePavmentCostModel.BlockCode > 0 ? objAveragePavmentCostModel.BlockCode : objAveragePavmentCostModel.Mast_Block_Code;

                    return View(objAveragePavmentCostModel);
                }
                else
                {
                    return View("AveragePavementCostReportLayout", objAveragePavmentCostModel);
                }
            }
            catch
            {
                return View(objAveragePavmentCostModel);
            }

        }
        #endregion

        #region 5 Construction Cost Pattern --Done
        [HttpGet]
        public ActionResult ConstructionCostPatternReportLayout()
        {
            ConstructionCostPatternModel objConstructionCostPatternModel = new ConstructionCostPatternModel();
            commonFunctions = new CommonFunctions();
            if (!(string.IsNullOrEmpty(Request.Params["SubReport"])))
            {
                ViewBag.ReportType = Request.Params["SubReport"];
            }
            if (!(string.IsNullOrEmpty(Request.Params["State"])))
            {
                objConstructionCostPatternModel.StateCode = Convert.ToInt32(Request.Params["State"]);
            }
            if (!(string.IsNullOrEmpty(Request.Params["District"])))
            {
                if (objConstructionCostPatternModel.StateCode > 0)
                {
                    objConstructionCostPatternModel.DistrictList = new List<SelectListItem>();
                    objConstructionCostPatternModel.DistrictList = commonFunctions.PopulateDistrict(objConstructionCostPatternModel.StateCode, true);
                    objConstructionCostPatternModel.DistrictList.Find(x => x.Value == "-1").Value = "0";

                }
                objConstructionCostPatternModel.DistrictCode = Convert.ToInt32(Request.Params["District"]);
            }
            if (!(string.IsNullOrEmpty(Request.Params["Block"])))
            {
                if (objConstructionCostPatternModel.DistrictCode > 0)
                {
                    objConstructionCostPatternModel.BlockList = new List<SelectListItem>();
                    objConstructionCostPatternModel.BlockList = commonFunctions.PopulateBlocks(objConstructionCostPatternModel.DistrictCode, true);
                    objConstructionCostPatternModel.BlockList.Find(x => x.Value == "-1").Value = "0";

                }
                objConstructionCostPatternModel.BlockCode = Convert.ToInt32(Request.Params["Block"]);
            }
            if (!(string.IsNullOrEmpty(Request.Params["Collaboration"])))
            {
                objConstructionCostPatternModel.FundingAgency = Convert.ToInt32(Request.Params["Collaboration"]);
            }
            if (!(string.IsNullOrEmpty(Request.Params["Year"])))
            {
                objConstructionCostPatternModel.FromYear = Convert.ToInt32(Request.Params["Year"]);
                objConstructionCostPatternModel.ToYear = Convert.ToInt32(Request.Params["Year"]);

            }
            if (!(string.IsNullOrEmpty(Request.Params["Batch"])))
            {
                objConstructionCostPatternModel.Batch = Convert.ToInt32(Request.Params["Batch"]);
            }
            if (!(string.IsNullOrEmpty(Request.Params["Status"])))
            {
                objConstructionCostPatternModel.Status = Request.Params["Status"].ToString();
            }
            return View(objConstructionCostPatternModel);
        }

        [HttpPost]
        public ActionResult ConstructionCostPatternReport(ConstructionCostPatternModel objConstructionCostPatternModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    objConstructionCostPatternModel.LevelCode = objConstructionCostPatternModel.RoadWise == true ? 4 : objConstructionCostPatternModel.BlockCode > 0 ? 3 : objConstructionCostPatternModel.DistrictCode > 0 ? 2 : 1;
                    objConstructionCostPatternModel.Mast_State_Code = objConstructionCostPatternModel.StateCode > 0 ? objConstructionCostPatternModel.StateCode : objConstructionCostPatternModel.Mast_State_Code;
                    objConstructionCostPatternModel.Mast_District_Code = objConstructionCostPatternModel.DistrictCode > 0 ? objConstructionCostPatternModel.DistrictCode : objConstructionCostPatternModel.Mast_District_Code;
                    objConstructionCostPatternModel.Mast_Block_Code = objConstructionCostPatternModel.BlockCode > 0 ? objConstructionCostPatternModel.BlockCode : objConstructionCostPatternModel.Mast_Block_Code;

                    return View(objConstructionCostPatternModel);
                }
                else
                {
                    return View("ConstructionCostPatternReportLayout", objConstructionCostPatternModel);
                }
            }
            catch
            {
                return View(objConstructionCostPatternModel);
            }

        }
        #endregion


        #region 6 Analysis for Average Length -- Done
        [HttpGet]
        public ActionResult AnalysisAverageLengthReportLayout()
        {
            AnalysisAverageLengthModel objAnalysisAverageLengthModel = new AnalysisAverageLengthModel();
            commonFunctions = new CommonFunctions();
            if (!(string.IsNullOrEmpty(Request.Params["SubReport"])))
            {
                ViewBag.ReportType = Request.Params["SubReport"];
            }
            if (!(string.IsNullOrEmpty(Request.Params["State"])))
            {
                objAnalysisAverageLengthModel.StateCode = Convert.ToInt32(Request.Params["State"]);
            }
            if (!(string.IsNullOrEmpty(Request.Params["District"])))
            {
                if (objAnalysisAverageLengthModel.StateCode > 0)
                {
                    objAnalysisAverageLengthModel.DistrictList = new List<SelectListItem>();
                    objAnalysisAverageLengthModel.DistrictList = commonFunctions.PopulateDistrict(objAnalysisAverageLengthModel.StateCode, true);
                    objAnalysisAverageLengthModel.DistrictList.Find(x => x.Value == "-1").Value = "0";
                }
                objAnalysisAverageLengthModel.DistrictCode = Convert.ToInt32(Request.Params["District"]);
            }
            if (!(string.IsNullOrEmpty(Request.Params["Block"])))
            {
                if (objAnalysisAverageLengthModel.DistrictCode > 0)
                {
                    objAnalysisAverageLengthModel.BlockList = new List<SelectListItem>();
                    objAnalysisAverageLengthModel.BlockList = commonFunctions.PopulateBlocks(objAnalysisAverageLengthModel.DistrictCode, true);
                    objAnalysisAverageLengthModel.BlockList.Find(x => x.Value == "-1").Value = "0";

                }
                objAnalysisAverageLengthModel.BlockCode = Convert.ToInt32(Request.Params["Block"]);
            }
            if (!(string.IsNullOrEmpty(Request.Params["Collaboration"])))
            {
                objAnalysisAverageLengthModel.FundingAgency = Convert.ToInt32(Request.Params["Collaboration"]);
            }
            if (!(string.IsNullOrEmpty(Request.Params["Year"])))
            {
                objAnalysisAverageLengthModel.PhaseYear = Convert.ToInt32(Request.Params["Year"]);
            }
            if (!(string.IsNullOrEmpty(Request.Params["Batch"])))
            {
                objAnalysisAverageLengthModel.Batch = Convert.ToInt32(Request.Params["Batch"]);
            }
            if (!(string.IsNullOrEmpty(Request.Params["Status"])))
            {
                objAnalysisAverageLengthModel.Status = Request.Params["Status"].ToString();
            }
            return View(objAnalysisAverageLengthModel);
        }

        [HttpPost]
        public ActionResult AnalysisAverageLengthReport(AnalysisAverageLengthModel objAnalysisAverageLengthModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    objAnalysisAverageLengthModel.LevelCode = objAnalysisAverageLengthModel.RoadWise == true ? 4 : objAnalysisAverageLengthModel.BlockCode > 0 ? 3 : objAnalysisAverageLengthModel.DistrictCode > 0 ? 2 : 1;
                    objAnalysisAverageLengthModel.Mast_State_Code = objAnalysisAverageLengthModel.StateCode > 0 ? objAnalysisAverageLengthModel.StateCode : objAnalysisAverageLengthModel.Mast_State_Code;
                    objAnalysisAverageLengthModel.Mast_District_Code = objAnalysisAverageLengthModel.DistrictCode > 0 ? objAnalysisAverageLengthModel.DistrictCode : objAnalysisAverageLengthModel.Mast_District_Code;
                    objAnalysisAverageLengthModel.Mast_Block_Code = objAnalysisAverageLengthModel.BlockCode > 0 ? objAnalysisAverageLengthModel.BlockCode : objAnalysisAverageLengthModel.Mast_Block_Code;

                    return View(objAnalysisAverageLengthModel);
                }
                else
                {
                    return View("AnalysisAverageLengthReportLayout", objAnalysisAverageLengthModel);
                }
            }
            catch
            {
                return View(objAnalysisAverageLengthModel);
            }

        }
        #endregion

        #region 7 Correlation between Population and AADT -- Done
        [HttpGet]
        public ActionResult CorrelationBetPopAADTReportLayout()
        {
            CorrelationBetPopAADTModel objCorrelationBetPopAADTModel = new CorrelationBetPopAADTModel();
            return View(objCorrelationBetPopAADTModel);
        }

        [HttpPost]
        public ActionResult CorrelationBetPopAADTReport(CorrelationBetPopAADTModel objCorrelationBetPopAADTModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    objCorrelationBetPopAADTModel.LevelCode = objCorrelationBetPopAADTModel.RoadWise == true ? 4 : objCorrelationBetPopAADTModel.BlockCode > 0 ? 3 : objCorrelationBetPopAADTModel.DistrictCode > 0 ? 2 : 1;
                    objCorrelationBetPopAADTModel.Mast_State_Code = objCorrelationBetPopAADTModel.StateCode > 0 ? objCorrelationBetPopAADTModel.StateCode : objCorrelationBetPopAADTModel.Mast_State_Code;
                    objCorrelationBetPopAADTModel.Mast_District_Code = objCorrelationBetPopAADTModel.DistrictCode > 0 ? objCorrelationBetPopAADTModel.DistrictCode : objCorrelationBetPopAADTModel.Mast_District_Code;
                    objCorrelationBetPopAADTModel.Mast_Block_Code = objCorrelationBetPopAADTModel.BlockCode > 0 ? objCorrelationBetPopAADTModel.BlockCode : objCorrelationBetPopAADTModel.Mast_Block_Code;

                    return View(objCorrelationBetPopAADTModel);
                }
                else
                {
                    return View("CorrelationBetPopAADTReportLayout", objCorrelationBetPopAADTModel);
                }
            }
            catch
            {
                return View(objCorrelationBetPopAADTModel);
            }

        }
        #endregion



        #region 8 Roads for Upgradation Maintenance -- Done
        [HttpGet]
        public ActionResult RoadUpgradeMaintReportLayout()
        {
            RodsUpgradeMaintModel objSubgradeSoilModel = new RodsUpgradeMaintModel();
            return View(objSubgradeSoilModel);
        }

        [HttpPost]
        public ActionResult RoadUpgradeMaintReport(RodsUpgradeMaintModel objRodsUpgradeMaintModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    objRodsUpgradeMaintModel.LevelCode = objRodsUpgradeMaintModel.RoadWise == true ? 4 : objRodsUpgradeMaintModel.BlockCode > 0 ? 3 : objRodsUpgradeMaintModel.DistrictCode > 0 ? 2 : 1;
                    objRodsUpgradeMaintModel.Mast_State_Code = objRodsUpgradeMaintModel.StateCode > 0 ? objRodsUpgradeMaintModel.StateCode : objRodsUpgradeMaintModel.Mast_State_Code;
                    objRodsUpgradeMaintModel.Mast_District_Code = objRodsUpgradeMaintModel.DistrictCode > 0 ? objRodsUpgradeMaintModel.DistrictCode : objRodsUpgradeMaintModel.Mast_District_Code;
                    objRodsUpgradeMaintModel.Mast_Block_Code = objRodsUpgradeMaintModel.BlockCode > 0 ? objRodsUpgradeMaintModel.BlockCode : objRodsUpgradeMaintModel.Mast_Block_Code;

                    return View(objRodsUpgradeMaintModel);
                }
                else
                {
                    return View("RoadUpgradeMaintReportLayout", objRodsUpgradeMaintModel);
                }
            }
            catch
            {
                return View(objRodsUpgradeMaintModel);
            }

        }

        #endregion



        #region 9 Pavement Condition of Through Routes --Done
        [HttpGet]
        public ActionResult PavementConditionThroughRouteReportLayout()
        {
            PavementConditionThroughRouteModel objPavementConditionThroughRouteModel = new PavementConditionThroughRouteModel();
            return View(objPavementConditionThroughRouteModel);
        }

        [HttpPost]
        public ActionResult PavementConditionThroughRouteReport(PavementConditionThroughRouteModel objPavementConditionThroughRouteModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    objPavementConditionThroughRouteModel.LevelCode = objPavementConditionThroughRouteModel.RoadWise == true ? 4 : objPavementConditionThroughRouteModel.BlockCode > 0 ? 3 : objPavementConditionThroughRouteModel.DistrictCode > 0 ? 2 : 1;
                    objPavementConditionThroughRouteModel.Mast_State_Code = objPavementConditionThroughRouteModel.StateCode > 0 ? objPavementConditionThroughRouteModel.StateCode : objPavementConditionThroughRouteModel.Mast_State_Code;
                    objPavementConditionThroughRouteModel.Mast_District_Code = objPavementConditionThroughRouteModel.DistrictCode > 0 ? objPavementConditionThroughRouteModel.DistrictCode : objPavementConditionThroughRouteModel.Mast_District_Code;
                    objPavementConditionThroughRouteModel.Mast_Block_Code = objPavementConditionThroughRouteModel.BlockCode > 0 ? objPavementConditionThroughRouteModel.BlockCode : objPavementConditionThroughRouteModel.Mast_Block_Code;

                    return View(objPavementConditionThroughRouteModel);
                }
                else
                {
                    return View("PavementConditionThroughRouteReportLayout", objPavementConditionThroughRouteModel);
                }
            }
            catch
            {
                return View(objPavementConditionThroughRouteModel);
            }

        }

        #endregion


        #region Fin Sta Locallisation Link Report Analysis For Proposal Status Report
        [HttpGet]
        public ActionResult FinStaAnalysisProposalReportLayout()
        {
            FinStaAnalysisProposalModel objFinStaAnalysisProposalModel = new FinStaAnalysisProposalModel();
            commonFunctions = new CommonFunctions();
            if (!(string.IsNullOrEmpty(Request.Params["SubReport"])))
            {
                ViewBag.ReportType = Request.Params["SubReport"];
            }

            objFinStaAnalysisProposalModel.Mast_State_Code = (Request.Params["State"] == null ? 0 : Convert.ToInt32(Request.Params["State"]));
            objFinStaAnalysisProposalModel.Mast_District_Code = (Request.Params["District"] == null ? 0 : Convert.ToInt32(Request.Params["District"]));
            objFinStaAnalysisProposalModel.Mast_Block_Code = (Request.Params["Block"] == null ? 0 : Convert.ToInt32(Request.Params["Block"]));
            objFinStaAnalysisProposalModel.FundingAgency = (Request.Params["Collaboration"] == null ? 0 : Convert.ToInt32(Request.Params["Collaboration"]));
            objFinStaAnalysisProposalModel.PhaseYear = (Request.Params["Year"] == null ? 0 : Convert.ToInt32(Request.Params["Year"]));
            objFinStaAnalysisProposalModel.Proposal = Request.Params["Proposal"] == null ? "%" : Request.Params["Proposal"];
            objFinStaAnalysisProposalModel.Type = Request.Params["Type"] == null ? "%" : Request.Params["Type"];
            objFinStaAnalysisProposalModel.Status = Request.Params["Status"] == null ? "%" : Request.Params["Status"];
            objFinStaAnalysisProposalModel.Drop = Request.Params["Drop"] == null ? "%" : Request.Params["Drop"];
            objFinStaAnalysisProposalModel.ReportType = Request.Params["ReportType"] == null ? "%" : Request.Params["ReportType"];
            objFinStaAnalysisProposalModel.Report = Request.Params["Rpt"] == null ? "%" : Request.Params["Rpt"];
            objFinStaAnalysisProposalModel.Progress = Request.Params["Progress"] == null ? "%" : Request.Params["Progress"];
            objFinStaAnalysisProposalModel.Sanctioned = Request.Params["Sanction"] == null ? "%" : Request.Params["Sanction"];
            objFinStaAnalysisProposalModel.Agency = (Request.Params["Agency"] == null ? 0 : Convert.ToInt32(Request.Params["Agency"]));
            objFinStaAnalysisProposalModel.Batch = (Request.Params["Batch"] == null ? 0 : Convert.ToInt32(Request.Params["Batch"]));
            objFinStaAnalysisProposalModel.Population = (Request.Params["Pop"] == null ? 0 : Convert.ToInt32(Request.Params["Pop"])); 

            return View(objFinStaAnalysisProposalModel);
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
        #endregion
    }
}
