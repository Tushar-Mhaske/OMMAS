using PMGSY.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.Common;

namespace PMGSY.OmmasDoRDIntegration
{
    public class OmmasDoRDIntegrationDAL
    {
        PMGSYEntities dbContext = null;

        #region Get LGD GetLGDHabitationsDAL()
        /// <summary>
        /// Get LGD Habitations
        /// </summary>
        /// <returns></returns>
        public List<OmmasDoRDIntegrationViewModel> GetLGDHabitationsDAL(int lgdStateCode, int lgdDistrictCode, int lgdBlockCode)
        {
            List<OmmasDoRDIntegrationViewModel> lstModel = new List<OmmasDoRDIntegrationViewModel>();
            dbContext = new PMGSYEntities();
            try
            {
                ((System.Data.Entity.Infrastructure.IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                //Get LGD Habitations
                var lgdHabitations = dbContext.USP_LDG_GET_HABITATION(lgdStateCode, lgdDistrictCode, lgdBlockCode).ToList();

                foreach (var item in lgdHabitations)
                {
                    OmmasDoRDIntegrationViewModel model = new OmmasDoRDIntegrationViewModel();

                    //model.ommasStateCode = item.MAST_STATE_CODE;
                    model.lgdStateCode = item.MAST_STATE_LDG_CODE;
                    model.stateName = item.MAST_STATE_NAME;

                    //model.ommasDistrictCode = item.MAST_DISTRICT_CODE;
                    model.lgdDistrictCode = item.MAST_DISTRICT_LDG_CODE;
                    model.districtName = item.MAST_DISTRICT_NAME;

                    //model.ommasBlockCode = item.MAST_BLOCK_CODE;
                    model.lgdBlockCode = item.MAST_BLOCK_LDG_CODE;
                    model.blockName = item.MAST_BLOCK_NAME;

                    model.habCode = item.MAST_HAB_CODE;
                    model.habName = item.MAST_HAB_NAME;
                    model.habTotPopulation = item.MAST_HAB_TOT_POP;

                    lstModel.Add(model);
                }

                return lstModel;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "OmmasDoRDIntegrationDAL.GetLGDHabitationsDAL()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion Get LGD GetLGDHabitationsDAL() Ends

        #region Get LGD GetLGDHabitationDetailsDAL()
        /// <summary>
        /// Get LGD Habitation Details
        /// </summary>
        /// <returns></returns>
        public List<OmmasDoRDHabitationDetailsViewModel> GetLGDHabitationDetailsDAL(int lgdStateCode, int lgdDistrictCode, int lgdBlockCode, int habCode, byte scheme)
        {
            List<OmmasDoRDHabitationDetailsViewModel> lstModel = new List<OmmasDoRDHabitationDetailsViewModel>();
            dbContext = new PMGSYEntities();
            try
            {
                ((System.Data.Entity.Infrastructure.IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                //Get LGD Habitation Details
                var lgdHabDetails = dbContext.USP_LDG_HAB_DEATAIL(lgdStateCode, lgdDistrictCode, lgdBlockCode, habCode, scheme).ToList();

                foreach (var item in lgdHabDetails)
                {
                    OmmasDoRDHabitationDetailsViewModel model = new OmmasDoRDHabitationDetailsViewModel();

                    model.roadCode = item.ROAD_ID;
                    model.habCode = item.HAB_CODE;

                    model.pmgsyScheme = item.SCHEME;

                    model.connectivity = item.CONNECTIVITY;
                    model.completionStage = item.Completion_Stage;
                    model.stage = item.STAGE;
                    model.piu = item.PIU;
                    model.sanctionYear = item.SANCTION_YEAR;

                    model.lgdStateCode = item.MAST_STATE_LDG_CODE ?? 0;
                    model.stateName = item.MAST_STATE_NAME;
                    model.lgdDistrictCode = item.MAST_DISTRICT_LDG_CODE ?? 0;
                    model.districtName = item.MAST_DISTRICT_NAME;
                    model.lgdBlockCode = item.MAST_BLOCK_LDG_CODE ?? 0;
                    model.blockName = item.MAST_BLOCK_NAME;

                    model.identification = item.IDENTIFICATION;
                    model.sanctionLength = item.SANCTION_LENGTH;
                    model.estimatedCost = item.ESTIMATED_COST ?? 0;
                    model.actualCost = item.ACTUAL_COST ?? 0;

                    model.BTType = item.BT_TYPE;
                    model.CCType = item.CC_TYPE ?? 0;
                    model.completedLength = item.COMPLETED_LENGTH ?? 0;

                    model.projectEndDate = string.IsNullOrEmpty(item.PROJECT_END_DATE) ? "NA" : item.PROJECT_END_DATE;

                    lstModel.Add(model);
                }

                return lstModel;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "OmmasDoRDIntegrationDAL.GetLGDHabitationDetailsDAL()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion Get LGD GetLGDHabitationDetailsDAL() Ends

        #region Get LGD GetSQCDetailsDAL()
        /// <summary>
        /// Get SQC Details
        /// </summary>
        /// <returns></returns>
        public List<OmmasDoRDSQCDetailsViewModel> GetSQCDetailsDAL(int lgdstateCode)
        {
            List<OmmasDoRDSQCDetailsViewModel> lstModel = new List<OmmasDoRDSQCDetailsViewModel>();
            dbContext = new PMGSYEntities();
            try
            {
                ((System.Data.Entity.Infrastructure.IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                //Get LGD Habitations
                var sqcDetails = dbContext.USP_SQC_DETAILS(lgdstateCode).ToList();

                foreach (var item in sqcDetails)
                {
                    OmmasDoRDSQCDetailsViewModel model = new OmmasDoRDSQCDetailsViewModel();

                    model.stateName = item.STATE_NAME;
                    model.sqcName = item.SQC;
                    model.designation = item.MAST_DESIG_NAME;
                    model.address = item.ADDRESS;
                    model.mobileNo = item.MOBILE;
                    model.email = item.EMAIL;

                    lstModel.Add(model);
                }

                return lstModel;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "OmmasDoRDIntegrationDAL.GetSQCDetailsDAL()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion Get LGD GetSQCDetailsDAL() Ends

        #region Get LGD GetHabitationsStatewiseDAL()
        /// <summary>
        /// Get LGD Habitations Statewise
        /// </summary>
        /// <returns></returns>
        public List<OmmasDoRDHabStatewiseViewModel> GetHabitationsStatewiseDAL(int lgdStateCode, int lgdDistrictCode, int lgdBlockCode, byte scheme)
        {
            List<OmmasDoRDHabStatewiseViewModel> lstModel = new List<OmmasDoRDHabStatewiseViewModel>();
            dbContext = new PMGSYEntities();
            try
            {
                ((System.Data.Entity.Infrastructure.IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                //Get LGD Habitations
                var habsStatewise = dbContext.USP_LDG_GET_HABITATION_STATEWISE(lgdStateCode, lgdDistrictCode, lgdBlockCode, scheme).ToList();

                foreach (var item in habsStatewise)
                {
                    OmmasDoRDHabStatewiseViewModel model = new OmmasDoRDHabStatewiseViewModel();

                    model.lgdStateCode = item.STATE_CODE;
                    model.stateName = item.STATE_NAME;

                    model.lgdDistrictCode = item.DISTRICT_CODE;
                    model.districtName = item.DISTRICT_NAME;

                    model.lgdBlockCode = item.BLOCK_CODE;
                    model.blockName = item.BLOCK_NAME;

                    model.habCode = item.HAB_CODE;
                    model.habName = item.HAB_NAME;

                    model.habPopulation = item.HAB_POPULATION;
                    model.habSCSTPopulation = item.HAB_SCST_POPULATION;

                    lstModel.Add(model);
                }

                return lstModel;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "OmmasDoRDIntegrationDAL.GetHabitationsStatewiseDAL()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion Get LGD GetHabitationsStatewiseDAL() Ends

        #region Get LGD GetHabitationsStateDAL()
        /// <summary>
        /// Get LGD Habitations State
        /// </summary>
        /// <returns></returns>
        public List<OmmasDoRDHabDetailsStateViewModel> GetHabitationsStateDAL(int lgdStateCode, int lgdDistrictCode, byte scheme)
        {
            List<OmmasDoRDHabDetailsStateViewModel> lstModel = new List<OmmasDoRDHabDetailsStateViewModel>();
            dbContext = new PMGSYEntities();
            try
            {
                ((System.Data.Entity.Infrastructure.IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                //Get LGD Habitations
                var habsStatewise = dbContext.USP_LDG_HAB_DEATAIL_STATE(lgdStateCode, lgdDistrictCode, scheme).ToList();

                foreach (var item in habsStatewise)
                {
                    OmmasDoRDHabDetailsStateViewModel model = new OmmasDoRDHabDetailsStateViewModel();

                    model.roadId = item.ROAD_ID;
                    model.habCode = item.HAB_CODE;
                    model.scheme = item.SCHEME;
                    model.connectivity = item.CONNECTIVITY;
                    model.completionStage = item.Completion_Stage;
                    model.stage = item.STAGE;
                    model.piu = item.PIU;
                    model.sanctionYear = item.SANCTION_YEAR;

                    model.lgdStateCode = Convert.ToInt32(item.MAST_STATE_LDG_CODE);
                    model.lgdDistrictCode = Convert.ToInt32(item.MAST_DISTRICT_LDG_CODE);
                    model.lgdBlockCode = Convert.ToInt32(item.MAST_BLOCK_LDG_CODE);

                    model.identification = item.IDENTIFICATION;
                    model.sanctionLength = item.SANCTION_LENGTH;
                    model.estimatedCost = Convert.ToDecimal(item.ESTIMATED_COST);
                    model.actualCost = item.ACTUAL_COST;
                    model.BTType = item.BT_TYPE;
                    model.CCType = Convert.ToDecimal(item.CC_TYPE);
                    model.completedLength = Convert.ToDecimal(item.COMPLETED_LENGTH);

                    model.projectEndDate = item.PROJECT_END_DATE;

                    lstModel.Add(model);
                }

                return lstModel;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "OmmasDoRDIntegrationDAL.GetHabitationsStateDAL()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion Get LGD GetHabitationsStatewiseDAL() Ends
    }
}