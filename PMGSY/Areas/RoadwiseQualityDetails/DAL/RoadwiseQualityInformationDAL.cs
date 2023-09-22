using PMGSY.Areas.RoadwiseQualityDetails.Models;
using PMGSY.Common;
using PMGSY.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.RoadwiseQualityDetails.DAL
{
    public class RoadwiseQualityInformationDAL
    {
        PMGSYEntities dbContext = null;

        public List<SelectListItem> GetDistrictsByState(int stateCode, bool isAllSelected=false, int selectedDistrictCode = 0)
        {
            dbContext = new PMGSYEntities();
            List<SelectListItem> ListDistrict = new List<SelectListItem>();
             try
            {
                ListDistrict = new SelectList(dbContext.MASTER_DISTRICT.Where(b => b.MAST_STATE_CODE == stateCode && b.MAST_DISTRICT_ACTIVE == "Y").OrderBy(b => b.MAST_DISTRICT_NAME), "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME", selectedDistrictCode).ToList();

                if (isAllSelected)
                {
                    ListDistrict.Insert(0, (new SelectListItem { Value = "0", Text = "All District", Selected = true }));                    
                }
                else
                {
                    ListDistrict.Insert(0, (new SelectListItem { Value = "0", Text = "Select District", Selected = true }));
                }           
               
                return ListDistrict;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RoadwiseQualityInformationDAL.GetDistrictsByState()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public List<SelectListItem> GetAllBlocksByDistrictCode(int DistrictCode, bool isAllSelected = false, Int32 selectedBlockCode = 0)
        {
            dbContext = new PMGSYEntities();

            List<SelectListItem> blockList = new List<SelectListItem>();
            try
            {
                blockList = new SelectList(dbContext.MASTER_BLOCK.Where(b => b.MAST_DISTRICT_CODE == DistrictCode && b.MAST_BLOCK_ACTIVE == "Y").OrderBy(b => b.MAST_BLOCK_NAME), "MAST_BLOCK_CODE", "MAST_BLOCK_NAME", selectedBlockCode).ToList();
                if (isAllSelected)
                {
                    blockList.Insert(0, (new SelectListItem { Value = "0", Text = "All Blocks", Selected = true }));
                }
                else
                {
                    blockList.Insert(0, (new SelectListItem { Value = "0", Text = "Select Block", Selected = true }));
                }
                return blockList;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RoadwiseQualityInformationDAL.GetAllBlocksByDistrictCode()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public List<SelectListItem> GetAllRoadByBlockCode(int blockCode, bool isAllSelected = false, int selectedRoadCode = 0)
        {
            dbContext = new PMGSYEntities();
            List<SelectListItem> roadList = new List<SelectListItem>();
            
            try
            {                   
                roadList = new SelectList(dbContext.PLAN_ROAD.Where(m=>m.MAST_BLOCK_CODE == blockCode).OrderBy(m=>m.PLAN_RD_NAME), "PLAN_CN_ROAD_CODE", "PLAN_RD_NAME", selectedRoadCode).ToList();

                if (isAllSelected)
                {                    
                    roadList.Insert(0, (new SelectListItem { Value = "0", Text = "All Roads", Selected = true }));
                }
                else
                {
                    roadList.Insert(0, (new SelectListItem { Value = "0", Text = "Select Road", Selected = true }));
                }

                return roadList;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RoadwiseQualityInformationDAL.GetAllRoadByBlockCode()");
                return null;
            }
            finally
            {
               dbContext.Dispose();
            }          
        }

        public PropsalDetailModel GetProposalDetailsDAL(int planRoadCode)
        {
            dbContext = new PMGSYEntities();
            PropsalDetailModel objProposalModel = new PropsalDetailModel();
            objProposalModel.lstIMSProposalDetails = new List<USP_CS_PROPOSAL_DETAILS_Result>();

            try
            {
                PLAN_ROAD _cnDetails = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == planRoadCode).FirstOrDefault();
                objProposalModel.lstIMSProposalDetails = dbContext.USP_CS_PROPOSAL_DETAILS(_cnDetails.MAST_STATE_CODE, _cnDetails.MAST_DISTRICT_CODE, _cnDetails.MAST_BLOCK_CODE, 0, 0, planRoadCode).Distinct().ToList();                
                return objProposalModel;
            }
            catch (Exception ex) {
                ErrorLog.LogError(ex, "RoadwiseQualityInformationDAL.GetProposalDetailsDAL()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public QualityDetailsModel GetQualityDetailsDAL(int roadCode)
        {
            dbContext = new PMGSYEntities();
            QualityDetailsModel objModel = new QualityDetailsModel();
            try
            {
                
                TEND_AGREEMENT_DETAIL aggDetails = null;
                if (dbContext.TEND_AGREEMENT_DETAIL.Any(m => m.IMS_PR_ROAD_CODE == roadCode))
                {
                    aggDetails = dbContext.TEND_AGREEMENT_DETAIL.Where(m => m.IMS_PR_ROAD_CODE == roadCode).FirstOrDefault();
                }

                if (aggDetails != null) {
                    objModel.QualityDetails = dbContext.USP_CITIZEN_POST_PROPOSAL_DETAILS(roadCode, "%").ToList();
                }                

                return objModel;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RoadwiseQualityInformationDAL.GetProposalDetailsDAL()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// returns the Quality monitoring details
        /// </summary>
        /// <param name="observationId"></param>
        /// <param name="qmType"></param>
        /// <returns></returns>
        public List<qm_observation_grading_detail_Result> GetGradingDetails(int observationId, string qmType)
        {
            dbContext = new PMGSYEntities();

            try
            {
                return dbContext.qm_observation_grading_detail(observationId, qmType).ToList();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RoadwiseQualityInformationDAL.GetGradingDetails()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// returns the list of image files along with description by observation id
        /// </summary>
        /// <param name="observationId"></param>
        /// <param name="qmType"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetFileDetails(int observationId, string qmType)
        {
            dbContext = new PMGSYEntities();
            try
            {
                Dictionary<string, string> lstPaths = new Dictionary<string, string>();
                var lstPathDetails = (from item in dbContext.QUALITY_QM_INSPECTION_FILE
                                      where item.QM_OBSERVATION_ID == observationId
                                      select
                                      new
                                      {
                                          item.QM_FILE_NAME,
                                          item.QM_FILE_DESCR
                                      }).ToList();

                foreach (var item in lstPathDetails)
                {
                    if (qmType == "I")
                    {
                        lstPaths.Add(
                       item.QM_FILE_NAME.Trim().Contains("$") != true ?
                            (Path.Combine(ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_NQM_VIRTUAL_DIR_PATH"], "thumbnails/" + item.QM_FILE_NAME.Trim()))
                       :
                       (Path.Combine(ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_NQM_VIRTUAL_DIR_PATH_NEW"], HttpUtility.UrlEncode(item.QM_FILE_NAME.Split('$')[0]) + "/" + "thumbnails/" + item.QM_FILE_NAME.Trim()))
                       , item.QM_FILE_DESCR);

                    }
                    else
                    {
                        lstPaths.Add(
                       item.QM_FILE_NAME.Trim().Contains("$") != true ?
                            (Path.Combine(ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_SQM_VIRTUAL_DIR_PATH"], "thumbnails/" + item.QM_FILE_NAME.Trim()))
                       :
                       (Path.Combine(ConfigurationManager.AppSettings["QUALITY_INSPECTION_FILE_UPLOAD_SQM_VIRTUAL_DIR_PATH_NEW"], HttpUtility.UrlEncode(item.QM_FILE_NAME.Split('$')[0]) + "/" + "thumbnails/" + item.QM_FILE_NAME.Trim()))
                       , item.QM_FILE_DESCR);
                    }
                }
                return lstPaths;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RoadwiseQualityInformationDAL.GetFileDetails()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
               

        /// <summary>
        /// check whether the start , end latitude and longitude are present or not
        /// </summary>
        /// <param name="observationId"></param>
        /// <param name="qmType"></param>
        /// <returns></returns>
        public string GetMarkerDetails(int observationId, string qmType)
        {
            dbContext = new PMGSYEntities();
            decimal value = (Decimal)0.00;
            try
            {
                if (dbContext.QUALITY_QM_OBSERVATION_MASTER.Any(m => m.QM_OBSERVATION_ID == observationId && (m.QM_START_LONGITUDE != value) && m.QM_END_LATITUDE != value && m.QM_END_LONGITUDE != value && m.QM_END_LONGITUDE != value))
                {
                    return "Y";
                }
                else
                {
                    return "N";
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RoadwiseQualityInformationDAL.GetMarkerDetails()");
                return "N";
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public string GetStartEndLatLongBAL(int obsId)
        {
            dbContext = new PMGSYEntities();
            try
            {
                var schDetails = (from qqom in dbContext.QUALITY_QM_OBSERVATION_MASTER
                                  where qqom.QM_OBSERVATION_ID == obsId
                                  select qqom).FirstOrDefault();


                return schDetails.QM_START_LATITUDE + "@" + schDetails.QM_START_LONGITUDE + "$$" + schDetails.QM_END_LATITUDE + "@" + schDetails.QM_END_LONGITUDE;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RoadwiseQualityInformationDAL.GetStartEndLatLongBAL()");
                return null;
            }
            finally {
                dbContext.Dispose();
            }
        }


    }
}