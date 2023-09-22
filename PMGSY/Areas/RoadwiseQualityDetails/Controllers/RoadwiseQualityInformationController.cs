using PMGSY.Areas.RoadwiseQualityDetails.DAL;
using PMGSY.Areas.RoadwiseQualityDetails.Models;
using PMGSY.Common;
using PMGSY.Controllers;
using PMGSY.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.RoadwiseQualityDetails.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class RoadwiseQualityInformationController : Controller
    {
        //
        // GET: /RoadwiseQualityDetails/RoadwiseQualityInformation/

        int outParam = 0;
        CommonFunctions objCommon = new CommonFunctions();
        RoadwiseQualityInformationDAL objDAL = new RoadwiseQualityInformationDAL();
        
        [Audit]
        [HttpGet]
        public ActionResult Index()
        {
            CommonFunctions objCommon = new CommonFunctions();

            List<SelectListItem> lstDistrict = new List<SelectListItem>();
            List<SelectListItem> lstBlock = new List<SelectListItem>();
            List<SelectListItem> lstRoads = new List<SelectListItem>();

            try
            {
                lstDistrict.Insert(0, new SelectListItem { Value = "0", Text = "Select District" });
                lstBlock.Insert(0, new SelectListItem { Value = "0", Text = "Select Block" });
                lstRoads.Insert(0, new SelectListItem { Value = "0", Text = "Select Road" });

                FilterDetailsViewModel objModel = new FilterDetailsViewModel();
                objModel.SearchType = "R"; /// default type Road
                objModel.State = 0;
                objModel.StateList = objCommon.PopulateStates(true);
                objModel.District = 0;
                objModel.DistrictList = lstDistrict;
                objModel.Block = 0;
                objModel.BlockList = lstBlock;
                objModel.RoadCode = 0;
                objModel.RoadList = lstRoads;

                return View(objModel);                
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RoadwiseQualityInformation.Index()");
                FilterDetailsViewModel objModelEx = new FilterDetailsViewModel();
                objModelEx.ErrorMessage = "Error occured while processing your request...";
                return View(objModelEx);
            }
        }


        /// <summary>
        /// returns the districts by state code
        /// </summary>
        /// <param name="stateCode"></param>
        /// <returns></returns>
        [Audit]
        [HttpPost]
        public JsonResult GetDistrictsByStateCode(string stateCode)
        {
            try
            {
                if (!int.TryParse(stateCode, out outParam))
                {
                    return Json(false);
                }
                
                List<SelectListItem> districtList = new List<SelectListItem>();
                               
                districtList = objDAL.GetDistrictsByState(Convert.ToInt32(stateCode.Trim()), false);

                return Json(districtList,JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)    
            {
                ErrorLog.LogError(ex, "RoadwiseQualityInformation.GetDistrictsByStateCode()");
                return null;
            }
        }

        /// <summary>
        /// returns the blocks by district code
        /// </summary>
        /// <param name="districtCode"></param>
        /// <returns></returns>
        [Audit]
        [HttpPost]
        public JsonResult GetBlocksByDistrictCode(string districtCode)
        {
            try
            {
                if (!int.TryParse(districtCode, out outParam))
                {
                    return Json(false);
                }
                List<SelectListItem> blockList = new List<SelectListItem>();

                blockList = objDAL.GetAllBlocksByDistrictCode(Convert.ToInt32(districtCode.Trim()), false);
                return Json(blockList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RoadwiseQualityInformation.GetBlocksByDistrictCode()");
                return null;
            }
        }

        /// <summary>
        /// returns the roads of block
        /// </summary>
        /// <param name="blockCode"></param>
        /// <returns></returns>
        [Audit]
        [HttpPost]
        public JsonResult GetAllRoadsByBlockCode(string blockCode)
        {
            try
            {
                if (!int.TryParse(blockCode, out outParam))
                {
                    return Json(false);
                }
                
                List<SelectListItem> roadList = new List<SelectListItem>();

                roadList = objDAL.GetAllRoadByBlockCode(Convert.ToInt32(blockCode.Trim()), false);                
                return Json(roadList);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RoadwiseQualityInformation.GetAllRoadsByBlockCode()");
                return null;
            }
        }

        [Audit]
        [HttpGet]
        public ActionResult GetProposalDetails(string planRoadCode) {
            try
            {
                PropsalDetailModel objModel = new PropsalDetailModel();
                objModel = objDAL.GetProposalDetailsDAL(Convert.ToInt32(planRoadCode));
                return View(objModel);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RoadwiseQualityInformation.GetAllRoadsByBlockCode()");
                PropsalDetailModel objModel = new PropsalDetailModel();
                objModel.ErrorMessage = "Error occured while processing your request...";
               return View(objModel);
            }
        }

        [Audit]
        [HttpGet]
        public ActionResult GetQualityDetails(int roadCode) 
        {
            try
            {
                QualityDetailsModel model = new QualityDetailsModel();
                model = objDAL.GetQualityDetailsDAL(roadCode);
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RoadwiseQualityInformation.GetQualityDetails()");
                QualityDetailsModel model = new QualityDetailsModel();
                model.ErrorMessage = "Error occured while processing your request...";
                return View(model);
            }

        }

        [Audit]
        [HttpGet]
        public ActionResult GetInspectionGradingDetails(string obsidqmtype) 
        {
            try
            {
                int observationId = 0;
                string qmType = String.Empty;

                if (obsidqmtype != null || obsidqmtype != string.Empty) { 
                    observationId = Convert.ToInt32(obsidqmtype.Split('$')[0]);
                    qmType = obsidqmtype.Split('$')[1];
                }
                
                if (observationId > 0 && !String.IsNullOrEmpty(qmType))
                {
                    ViewBag.InspectionDetails = objDAL.GetGradingDetails(observationId, qmType);
                    ViewBag.FileDetails = objDAL.GetFileDetails(observationId, qmType);
                    ViewBag.ObservationId = observationId;
                    ViewBag.IsMarkerAvailable = objDAL.GetMarkerDetails(observationId, qmType);
                }

                return View();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RoadwiseQualityInformation.GetQualityDetails()");

                return Json(new { status="error"});
            }
        }


        /// <summary>
        /// returns the start ,end longitude and latitude
        /// </summary>
        /// <returns></returns>
        public ActionResult GetStartEndLatLong()
        {
            try
            {
                int obsId = Convert.ToInt32(Request.Params["obsId"]);
                return Json(new { Success = true, Message = objDAL.GetStartEndLatLongBAL(obsId) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return null;
            }
        }



    }
}
