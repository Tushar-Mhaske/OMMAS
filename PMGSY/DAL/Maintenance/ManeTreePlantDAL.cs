using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PMGSY.Model.Maintenance;
using System.Data.Entity;
//using PMGSY.ViewModel.Maintenance;
using PMGSY.Models;
using System.Web;
using PMGSY.Extensions;

namespace PMGSY.DAL.Maintenance
{
    public class ManeTreePlantDAL : IManeTreePlantDAL
    {
        PMGSYEntities maneDbContext = new PMGSYEntities();

        public bool Add(ManeTreePlantModel treePlant)
        {
            try
            {
                var treePlantToAdd = new MANE_TREE_PLANT
                {
                    IMS_PR_ROAD_CODE = treePlant.IMS_PR_ROAD_CODE,
                    TREE_PLANT_MONTH = treePlant.TREE_PLANT_MONTH,
                    TREE_PLANT_YEAR = treePlant.TREE_PLANT_YEAR,
                    TREE_PLANT_NEW = treePlant.TREE_PLANT_NEW,
                    TREE_PLANT_OLD = treePlant.TREE_PLANT_OLD
                };

                maneDbContext.MANE_TREE_PLANT.Add(treePlantToAdd);
                maneDbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
            }
        }

        public string Delete(int id)
        {
            try
            {
                var treePlantToAdd = maneDbContext.MANE_TREE_PLANT.Find(id);
                maneDbContext.MANE_TREE_PLANT.Remove(treePlantToAdd);
                maneDbContext.SaveChanges();
                return "";
            }
            catch (Exception ex)
            {
                return "Error Occured on Tree Plant Deletion.";
            }
            finally
            {
            }
        }

        public void Edite(ManeTreePlantModel treePlant)
        {
            try
            {
                var treePlantToAdd = maneDbContext.MANE_TREE_PLANT.Find(treePlant.TREE_PLANT_ID);
                maneDbContext.Entry(treePlantToAdd).State =  System.Data.Entity.EntityState.Modified;
                maneDbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                //return null;
            }
            finally
            {
            }
        }

        public ManeTreePlantHeaderViewModel GetHeader(int id)
        {
            try
            {
                var treePlantToGet = maneDbContext.USP_TREEPLANT_HEADER(id).FirstOrDefault<USP_TREEPLANT_HEADER_Result>();
                var treePlant = new ManeTreePlantHeaderViewModel
                {
                    StateName = treePlantToGet.MAST_STATE_NAME,
                    DistrictName = treePlantToGet.MAST_DISTRICT_NAME,
                    BlockName = treePlantToGet.MAST_BLOCK_NAME,
                    RoadName = treePlantToGet.IMS_ROAD_NAME,
                    SanctionYear = treePlantToGet.IMS_YEAR,
                    Package = treePlantToGet.IMS_PACKAGE_ID,
                    verifyCount = maneDbContext.QUALITY_QM_OBSERVATION_TREES.Where(x => x.QUALITY_QM_OBSERVATION_MASTER.IMS_PR_ROAD_CODE == id).Count(),
                };
                return treePlant;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
            }
        }

        public ManeTreePlantModel Get(int id)
        {
            try
            {
                var treePlantToGet = maneDbContext.MANE_TREE_PLANT.Find(id);
                var treePlant = new ManeTreePlantModel
                {
                    IMS_PR_ROAD_CODE = treePlantToGet.IMS_PR_ROAD_CODE,
                    TREE_PLANT_MONTH = treePlantToGet.TREE_PLANT_MONTH,
                    TREE_PLANT_YEAR = treePlantToGet.TREE_PLANT_YEAR,
                    TREE_PLANT_NEW = treePlantToGet.TREE_PLANT_NEW,
                    TREE_PLANT_OLD = treePlantToGet.TREE_PLANT_OLD
                };
                return treePlant;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
            }
        }

        public List<ManeTreePlantModel> GetAll(int roadId)
        {
            try
            {
                var treePlantToList = maneDbContext.USP_TREEPLANT_LIST(roadId).ToList<USP_TREEPLANT_LIST_Result>();
                var treePlantList = (from treePlant in treePlantToList
                                     select new ManeTreePlantModel
                                         {
                                             TREE_PLANT_MONTH = treePlant.TREE_PLANT_MONTH,
                                             TREE_PLANT_MONTH_NAME = treePlant.TREE_PLANT_MONTH_NAME,
                                             TREE_PLANT_YEAR = treePlant.TREE_PLANT_YEAR,
                                             TREE_PLANT_NEW = treePlant.TREE_PLANT_NEW,
                                             TREE_PLANT_OLD = treePlant.TREE_PLANT_OLD,
                                             TREE_PLANT_ID = treePlant.TREE_PLANT_ID,
                                             SrNo = treePlant.SR_NO,
                                             verifyCount = maneDbContext.QUALITY_QM_OBSERVATION_TREES.Where(x => x.QUALITY_QM_OBSERVATION_MASTER.IMS_PR_ROAD_CODE == roadId).Count(),
                                             Verify = maneDbContext.QUALITY_QM_OBSERVATION_TREES.Where(x => x.QUALITY_QM_OBSERVATION_MASTER.IMS_PR_ROAD_CODE == roadId).Select(x => x.QM_TREES_VERIFIED).FirstOrDefault() == "Y" ? "Yes" : "No",
                                         }).ToList<ManeTreePlantModel>();
                return treePlantList;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
            }
        }

        public string TreePlantVerifyAddDAL(ManeTreePlantVerifyViewModel treePlant)
        {
            int scheduleCode = 0;
            PMGSY.Models.PMGSYEntities dbContext = new Models.PMGSYEntities();
            try
            {
                //scheduleCode = dbContext.QUALITY_QM_SCHEDULE_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == treePlant.roadCode && x.UM_User_Master.UserID == PMGSY.Extensions.PMGSYSession.Current.UserId).Select(x => x.QUALITY_QM_SCHEDULE.ADMIN_QM_CODE).FirstOrDefault();
                var treePlantToAdd = new QUALITY_QM_OBSERVATION_TREES
                {
                    QM_OBSERVATION_ID = treePlant.observationId,
                    QM_REMARKS = treePlant.Remarks,
                    QM_TREES_VERIFIED = treePlant.Verify,
                    USERID = PMGSYSession.Current.UserId,
                    IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]
                };
                maneDbContext.QUALITY_QM_OBSERVATION_TREES.Add(treePlantToAdd);
                maneDbContext.SaveChanges();
                return "";
            }
            catch (Exception ex)
            {
                return "Error occured on Tree Plant Verify";
            }
            finally
            {
            }
        }
    }
}
