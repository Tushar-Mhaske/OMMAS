#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   LSBProposalBAL.cs
        * Description   :   BAL Methods to call DAL methods of  Creating , Editing, Deleting LSB Proposal and Related Screens of LSB Proposals Details
        * Author        :   Shyam Yadav
        * Modified By   :   Shivkumar Deshmukh        
        * Creation Date :   20-05-2013
**/
#endregion

using PMGSY.DAL.Proposal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.Models;
using PMGSY.Models.Proposal;
using PMGSY.Extensions;
using PMGSY.Common;

namespace PMGSY.BAL.Proposal
{
    public class LSBProposalBAL : ILSBProposalBAL
    {
        ILSBProposalDAL objLSBProposalDAL = new LSBProposalDAL();
        private PMGSYEntities db = new PMGSYEntities();

        #region LSB Proposal

        /// <summary>
        /// Get LSB Proposal Listing
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="MAST_STATE_CODE"></param>
        /// <param name="MAST_DISTRICT_CODE"></param>
        /// <param name="IMS_YEAR"></param>
        /// <param name="MAST_BLOCK_CODE"></param>
        /// <param name="IMS_BATCH"></param>
        /// <param name="IMS_STREAMS"></param>
        /// <param name="ProposalType"></param>
        /// <returns></returns>
        public Array GetLSBProposalBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int MAST_STATE_CODE, int MAST_DISTRICT_CODE, int IMS_YEAR, int MAST_BLOCK_CODE, int IMS_BATCH, int IMS_STREAMS, String ProposalType, string IMS_PROPOSAL_STATUS, string IMS_UPGRADE_CONNECT, out ProposalColumnsTotal model)
        {
            return objLSBProposalDAL.GetLSBProposalsDAL(page, rows, sidx, sord, out totalRecords, MAST_STATE_CODE, MAST_DISTRICT_CODE, IMS_YEAR, MAST_BLOCK_CODE, IMS_BATCH, IMS_STREAMS, ProposalType, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT, out model);
        }

        /// <summary>
        /// List Road Proposals for State
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="MAST_STATE_CODE"></param>
        /// <param name="MAST_DISTRICT_CODE"></param>
        /// <param name="IMS_YEAR"></param>
        /// <param name="IMS_BATCH"></param>
        /// <param name="IMS_STREAMS"></param>
        /// <param name="ProposalType"></param>
        /// <param name="Filters"></param>
        /// <returns></returns>
        public Array GetLSBProposalsForSRRDABAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int MAST_STATE_CODE, int MAST_DISTRICT_CODE, int IMS_YEAR, int IMS_BATCH, int IMS_STREAMS, String ProposalType, string IMS_PROPOSAL_STATUS, string IMS_UPGRADE_CONNECT, int MAST_BLOCK_CODE, out ProposalColumnsTotal model)
        {
            return objLSBProposalDAL.GetLSBProposalsForSRRDADAL(page, rows, sidx, sord, out totalRecords, MAST_STATE_CODE, MAST_DISTRICT_CODE, IMS_YEAR, IMS_BATCH, IMS_STREAMS, ProposalType, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT, MAST_BLOCK_CODE, out model);
        }

        /// <summary>
        /// Create LSB Proposal
        /// </summary>
        /// <param name="ims_sanctioned_projects"></param>
        /// <returns></returns>
        public string SaveLSBProposalBAL(PMGSY.Models.Proposal.LSBViewModel ims_sanctioned_projects)
        {
            IMS_SANCTIONED_PROJECTS objProposal = new IMS_SANCTIONED_PROJECTS();

            try
            {
                objProposal.MAST_PMGSY_SCHEME = PMGSYSession.Current.PMGSYScheme;//(PMGSYSession.Current.PMGSYScheme == 3 ? (byte)1 : PMGSYSession.Current.PMGSYScheme);///Changed for RCPLWE
                objProposal.IMS_PROPOSAL_TYPE = "L";
                objProposal.IMS_YEAR = ims_sanctioned_projects.IMS_YEAR;
                objProposal.IMS_BATCH = ims_sanctioned_projects.IMS_BATCH;
                objProposal.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                objProposal.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                objProposal.MAST_DPIU_CODE = PMGSYSession.Current.AdminNdCode;
                objProposal.MAST_BLOCK_CODE = ims_sanctioned_projects.MAST_BLOCK_CODE;
                //FUNDING AGENCY
                objProposal.IMS_COLLABORATION = ims_sanctioned_projects.IMS_COLLABORATION;
                objProposal.IMS_STREAMS = ims_sanctioned_projects.IMS_STREAMS;

                objProposal.IMS_EXISTING_PACKAGE = ims_sanctioned_projects.IMS_EXISTING_PACKAGE;
                // New Package or Exising Package
                if (ims_sanctioned_projects.IMS_EXISTING_PACKAGE.ToUpper() == "N")
                {
                    objProposal.IMS_PACKAGE_ID = ims_sanctioned_projects.PACKAGE_PREFIX + ims_sanctioned_projects.IMS_PACKAGE_ID;
                }
                else
                {
                    objProposal.IMS_PACKAGE_ID = ims_sanctioned_projects.EXISTING_IMS_PACKAGE_ID;
                }


                if (ims_sanctioned_projects.isExistingRoad.ToUpper().Equals("U"))
                {
                    //Existing Road

                    var query = (from isp in db.IMS_SANCTIONED_PROJECTS
                                 where isp.IMS_PR_ROAD_CODE == ims_sanctioned_projects.IMS_STAGED_ROAD_ID
                                 select isp).First();

                    objProposal.IMS_ROAD_NAME = ims_sanctioned_projects.IMS_ROAD_NAME;
                    objProposal.IMS_ROAD_FROM = query.IMS_ROAD_FROM;
                    objProposal.IMS_ROAD_TO = query.IMS_ROAD_TO;

                    //objProposal.IMS_UPGRADE_CONNECT = "U";
                    ///Changed by SAMMED PATIL on 27 October 2016 as per instructions from Srinivas Sir.
                    objProposal.IMS_UPGRADE_CONNECT = query.IMS_UPGRADE_CONNECT;
                }
                else
                {
                    objProposal.IMS_ROAD_NAME = ims_sanctioned_projects.IMS_ROAD_NAME;
                    objProposal.IMS_ROAD_FROM = ims_sanctioned_projects.IMS_ROAD_FROM;
                    objProposal.IMS_ROAD_TO = ims_sanctioned_projects.IMS_ROAD_TO;

                    objProposal.IMS_UPGRADE_CONNECT = "N";

                    // Link/Through Route Name
                    objProposal.PLAN_CN_ROAD_CODE = ims_sanctioned_projects.PLAN_CN_ROAD_CODE;
                }
                
                objProposal.IMS_PARTIAL_LEN = "F";
                objProposal.IMS_PAV_LENGTH = 0;
                objProposal.IMS_PAV_EST_COST = 0;
                objProposal.IMS_NO_OF_CDWORKS = 0;
                objProposal.IMS_CD_WORKS_EST_COST = 0;
                objProposal.IMS_PROTECTION_WORKS = 0;
                objProposal.IMS_OTHER_WORK_COST = 0;
                objProposal.IMS_STATE_SHARE = 0;


                // PMGSY Scheme-II
                objProposal.IMS_IS_HIGHER_SPECIFICATION = ims_sanctioned_projects.IMS_IS_HIGHER_SPECIFICATION;
                if (objProposal.IMS_IS_HIGHER_SPECIFICATION != null && ims_sanctioned_projects.IMS_IS_HIGHER_SPECIFICATION.Equals("Y"))
                {
                    objProposal.IMS_HIGHER_SPECIFICATION_COST = ims_sanctioned_projects.IMS_HIGHER_SPECIFICATION_COST;
                    objProposal.IMS_SANCTIONED_HS_AMT = ims_sanctioned_projects.IMS_HIGHER_SPECIFICATION_COST;
                }
                else
                {
                    objProposal.IMS_HIGHER_SPECIFICATION_COST = null;
                }

                objProposal.IMS_SHARE_PERCENT = ims_sanctioned_projects.IMS_SHARE_PERCENT ; //ims_sanctioned_projects.IMS_SHARE_PERCENT; Added on 12 March 2021
                

                objProposal.IMS_ZP_RESO_OBTAINED = "N";
                objProposal.IMS_IS_STAGED = "C";
                objProposal.IMS_ISBENEFITTED_HABS = "N";
                objProposal.IMS_MAINTENANCE_YEAR1 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT1;
                objProposal.IMS_MAINTENANCE_YEAR2 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT2;
                objProposal.IMS_MAINTENANCE_YEAR3 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT3;
                objProposal.IMS_MAINTENANCE_YEAR4 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT4;
                objProposal.IMS_MAINTENANCE_YEAR5 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT5;

                //PMGSY Scheme-II
                objProposal.IMS_RENEWAL_COST = ims_sanctioned_projects.IMS_RENEWAL_COST;

                objProposal.IMS_DPR_STATUS = "N";
                objProposal.STA_SANCTIONED = "N";
                objProposal.IMS_SANCTIONED = "N";

                objProposal.IMS_SANCTIONED_PAV_AMT = 0;
                objProposal.IMS_SANCTIONED_CD_AMT = 0;
                objProposal.IMS_SANCTIONED_PW_AMT = 0;
                objProposal.IMS_SANCTIONED_OW_AMT = 0;
                objProposal.IMS_SANCTIONED_BW_AMT = 0;
                objProposal.IMS_SANCTIONED_MAN_AMT1 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT1;
                objProposal.IMS_SANCTIONED_MAN_AMT2 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT2;
                objProposal.IMS_SANCTIONED_MAN_AMT3 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT3;
                objProposal.IMS_SANCTIONED_MAN_AMT4 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT4;
                objProposal.IMS_SANCTIONED_MAN_AMT5 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT5;

                //PMGSY Scheme-II
                objProposal.IMS_SANCTIONED_RENEWAL_AMT = ims_sanctioned_projects.IMS_RENEWAL_COST;

                objProposal.IMS_FINAL_PAYMENT_FLAG = "N";
                objProposal.IMS_ISCOMPLETED = "E";
                objProposal.IMS_LOCK_STATUS = "N";
                objProposal.IMS_FREEZE_STATUS = "U";
                objProposal.IMS_SHIFT_STATUS = "N";
                objProposal.PTA_SANCTIONED = "N";

                objProposal.IMS_STAGED_YEAR = ims_sanctioned_projects.IMS_STAGED_YEAR;
                objProposal.IMS_STAGED_PACKAGE_ID = ims_sanctioned_projects.IMS_STAGED_PACKAGE_ID;
                objProposal.IMS_STAGED_ROAD_ID = ims_sanctioned_projects.IMS_STAGED_ROAD_ID;
                objProposal.IMS_NO_OF_BRIDGEWRKS = 1; //default
                objProposal.IMS_BRIDGE_NAME = ims_sanctioned_projects.IMS_BRIDGE_NAME;
                objProposal.IMS_BRIDGE_LENGTH = ims_sanctioned_projects.IMS_BRIDGE_LENGTH;
                objProposal.IMS_BRIDGE_WORKS_EST_COST = ims_sanctioned_projects.IMS_BRIDGE_WORKS_EST_COST;
                objProposal.IMS_BRIDGE_EST_COST_STATE = ims_sanctioned_projects.IMS_BRIDGE_EST_COST_STATE;
                objProposal.IMS_REMARKS = ims_sanctioned_projects.IMS_REMARKS;

                objProposal.IMS_SANCTIONED_BW_AMT = ims_sanctioned_projects.IMS_BRIDGE_WORKS_EST_COST;
                objProposal.IMS_SANCTIONED_BS_AMT = ims_sanctioned_projects.IMS_BRIDGE_EST_COST_STATE;

                objProposal.IMS_SHARE_PERCENT_2015 = ims_sanctioned_projects.IMS_SHARE_PERCENT_2015;
                objProposal.IMS_STATE_SHARE_2015 = ims_sanctioned_projects.IMS_STATE_SHARE_2015;
                objProposal.IMS_MORD_SHARE_2015 = ims_sanctioned_projects.IMS_MORD_SHARE_2015;

                return objLSBProposalDAL.SaveLSBProposalDAL(objProposal);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                ErrorLog.LogError(ex, "SaveLSBProposalBAL");
                return "An Error Occurred While Processing Your Request.";
            }
            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// Update LSB Details
        /// </summary>
        /// <param name="ims_sanctioned_projects"></param>
        /// <returns></returns>
        public string UpdateLSBProposalBAL(PMGSY.Models.Proposal.LSBViewModel ims_sanctioned_projects)
        {
            IMS_SANCTIONED_PROJECTS objProposal = new IMS_SANCTIONED_PROJECTS();
            try
            {
                //Primary Key
                objProposal.IMS_PR_ROAD_CODE = ims_sanctioned_projects.IMS_PR_ROAD_CODE;

                objProposal.MAST_PMGSY_SCHEME = PMGSYSession.Current.PMGSYScheme;
                objProposal.IMS_PROPOSAL_TYPE = "L";
                objProposal.IMS_YEAR = ims_sanctioned_projects.IMS_YEAR;
                objProposal.IMS_BATCH = ims_sanctioned_projects.IMS_BATCH;
                objProposal.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                objProposal.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                objProposal.MAST_DPIU_CODE = PMGSYSession.Current.AdminNdCode;
                objProposal.MAST_BLOCK_CODE = ims_sanctioned_projects.MAST_BLOCK_CODE;
                //FUNDING AGENCY
                objProposal.IMS_COLLABORATION = ims_sanctioned_projects.IMS_COLLABORATION;
                objProposal.IMS_STREAMS = ims_sanctioned_projects.IMS_STREAMS;
                objProposal.IMS_EXISTING_PACKAGE = ims_sanctioned_projects.IMS_EXISTING_PACKAGE;


                // New Package or Exising Package
                if (ims_sanctioned_projects.IMS_EXISTING_PACKAGE.ToUpper() == "N")
                {
                    objProposal.IMS_PACKAGE_ID = ims_sanctioned_projects.PACKAGE_PREFIX + ims_sanctioned_projects.IMS_PACKAGE_ID;
                }
                else
                {
                    objProposal.IMS_PACKAGE_ID = ims_sanctioned_projects.EXISTING_IMS_PACKAGE_ID;
                }


                if (ims_sanctioned_projects.isExistingRoad.ToUpper().Equals("U"))
                {
                    //Existing Road

                    var query = (from isp in db.IMS_SANCTIONED_PROJECTS
                                 where isp.IMS_PR_ROAD_CODE == ims_sanctioned_projects.IMS_STAGED_ROAD_ID
                                 select isp).First();

                    objProposal.IMS_ROAD_NAME = ims_sanctioned_projects.IMS_ROAD_NAME;
                    objProposal.IMS_ROAD_FROM = query.IMS_ROAD_FROM;
                    objProposal.IMS_ROAD_TO = query.IMS_ROAD_TO;

                    //objProposal.IMS_UPGRADE_CONNECT = "U";
                    ///Changed by SAMMED PATIL on 27 October 2016 as per instructions from Srinivas Sir.
                    objProposal.IMS_UPGRADE_CONNECT = query.IMS_UPGRADE_CONNECT;
                }
                else
                {
                    objProposal.IMS_ROAD_NAME = ims_sanctioned_projects.IMS_ROAD_NAME;
                    objProposal.IMS_ROAD_FROM = ims_sanctioned_projects.IMS_ROAD_FROM;
                    objProposal.IMS_ROAD_TO = ims_sanctioned_projects.IMS_ROAD_TO;

                    objProposal.IMS_UPGRADE_CONNECT = "N";

                    // Link/Through Route Name
                    objProposal.PLAN_CN_ROAD_CODE = ims_sanctioned_projects.PLAN_CN_ROAD_CODE;
                }

                objProposal.IMS_PARTIAL_LEN = "F";
                objProposal.IMS_PAV_LENGTH = 0;
                objProposal.IMS_PAV_EST_COST = 0;
                objProposal.IMS_NO_OF_CDWORKS = 0;
                objProposal.IMS_CD_WORKS_EST_COST = 0;
                objProposal.IMS_PROTECTION_WORKS = 0;
                objProposal.IMS_OTHER_WORK_COST = 0;
                objProposal.IMS_STATE_SHARE = 0;


                //PMGSY Scheme-II
                objProposal.IMS_IS_HIGHER_SPECIFICATION = ims_sanctioned_projects.IMS_IS_HIGHER_SPECIFICATION;
                if (objProposal.IMS_IS_HIGHER_SPECIFICATION != null && ims_sanctioned_projects.IMS_IS_HIGHER_SPECIFICATION.Equals("Y"))
                {
                    objProposal.IMS_HIGHER_SPECIFICATION_COST = ims_sanctioned_projects.IMS_HIGHER_SPECIFICATION_COST;
                    objProposal.IMS_SANCTIONED_HS_AMT = ims_sanctioned_projects.IMS_HIGHER_SPECIFICATION_COST;
                }
                else
                {
                    objProposal.IMS_HIGHER_SPECIFICATION_COST = null;
                }
                objProposal.IMS_SHARE_PERCENT = ims_sanctioned_projects.IMS_SHARE_PERCENT;


                objProposal.IMS_ZP_RESO_OBTAINED = "N";
                objProposal.IMS_IS_STAGED = "C";
                objProposal.IMS_ISBENEFITTED_HABS = "N";
                objProposal.IMS_MAINTENANCE_YEAR1 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT1;
                objProposal.IMS_MAINTENANCE_YEAR2 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT2;
                objProposal.IMS_MAINTENANCE_YEAR3 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT3;
                objProposal.IMS_MAINTENANCE_YEAR4 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT4;
                objProposal.IMS_MAINTENANCE_YEAR5 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT5;

                //PMGSY Scheme-II
                objProposal.IMS_RENEWAL_COST = ims_sanctioned_projects.IMS_RENEWAL_COST;

                objProposal.IMS_DPR_STATUS = "N";

                objProposal.IMS_SANCTIONED_PAV_AMT = 0;
                objProposal.IMS_SANCTIONED_CD_AMT = 0;
                objProposal.IMS_SANCTIONED_PW_AMT = 0;
                objProposal.IMS_SANCTIONED_OW_AMT = 0;
                objProposal.IMS_SANCTIONED_BW_AMT = 0;
                objProposal.IMS_SANCTIONED_MAN_AMT1 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT1;
                objProposal.IMS_SANCTIONED_MAN_AMT2 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT2;
                objProposal.IMS_SANCTIONED_MAN_AMT3 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT3;
                objProposal.IMS_SANCTIONED_MAN_AMT4 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT4;
                objProposal.IMS_SANCTIONED_MAN_AMT5 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT5;

                //PMGSY Scheme-II
                objProposal.IMS_SANCTIONED_RENEWAL_AMT = ims_sanctioned_projects.IMS_RENEWAL_COST;

                objProposal.IMS_FINAL_PAYMENT_FLAG = "N";

                objProposal.IMS_LOCK_STATUS = "N";
                objProposal.IMS_FREEZE_STATUS = "U";
                objProposal.IMS_SHIFT_STATUS = "N";
                objProposal.PTA_SANCTIONED = "N";

                objProposal.IMS_STAGED_YEAR = ims_sanctioned_projects.IMS_STAGED_YEAR;
                objProposal.IMS_STAGED_PACKAGE_ID = ims_sanctioned_projects.IMS_STAGED_PACKAGE_ID;
                objProposal.IMS_STAGED_ROAD_ID = ims_sanctioned_projects.IMS_STAGED_ROAD_ID;
                objProposal.IMS_NO_OF_BRIDGEWRKS = 1; //default
                objProposal.IMS_BRIDGE_NAME = ims_sanctioned_projects.IMS_BRIDGE_NAME;
                objProposal.IMS_BRIDGE_LENGTH = ims_sanctioned_projects.IMS_BRIDGE_LENGTH;

                objProposal.IMS_BRIDGE_WORKS_EST_COST = ims_sanctioned_projects.IMS_BRIDGE_WORKS_EST_COST;
                objProposal.IMS_BRIDGE_EST_COST_STATE = ims_sanctioned_projects.IMS_BRIDGE_EST_COST_STATE;



                objProposal.IMS_REMARKS = ims_sanctioned_projects.IMS_REMARKS;

                objProposal.IMS_SANCTIONED_BW_AMT = ims_sanctioned_projects.IMS_BRIDGE_WORKS_EST_COST;
                objProposal.IMS_SANCTIONED_BS_AMT = ims_sanctioned_projects.IMS_BRIDGE_EST_COST_STATE;

                //Flags set on Edit before DPIU Finaliation as wel as reset after Mord Reconsider
                PMGSYEntities dbContext1 = new PMGSYEntities();
                IMS_SANCTIONED_PROJECTS objProposal1 = new IMS_SANCTIONED_PROJECTS();
                objProposal1 = dbContext1.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == ims_sanctioned_projects.IMS_PR_ROAD_CODE).FirstOrDefault();

                if (!dbContext1.IMS_SANCTIONED_PROJECTS.Any(m => m.STA_SANCTIONED.Equals("Y") && m.IMS_PR_ROAD_CODE == ims_sanctioned_projects.IMS_PR_ROAD_CODE))
                {
                    objProposal.IMS_ISCOMPLETED = "E";
                    objProposal.STA_SANCTIONED = "N";
                    objProposal.IMS_SANCTIONED = "N";
                    objProposal.STA_SANCTIONED_BY = null;
                    objProposal.STA_SANCTIONED_DATE = null;
                    objProposal.IMS_STA_REMARKS = null;
                    objProposal.IMS_SANCTIONED_BY = null;
                    objProposal.IMS_SANCTIONED_DATE = null;
                }
                else
                { // Mentioned Below Fields can be avoided to be updated when Proposal is STA Sanctioned and Unlocked by Mord.
                    objProposal.IMS_ISCOMPLETED = objProposal1.IMS_ISCOMPLETED;
                    objProposal.STA_SANCTIONED = objProposal1.STA_SANCTIONED;
                    objProposal.IMS_SANCTIONED = objProposal1.IMS_SANCTIONED;
                    objProposal.STA_SANCTIONED_BY = objProposal1.STA_SANCTIONED_BY;
                    objProposal.STA_SANCTIONED_DATE = objProposal1.STA_SANCTIONED_DATE;
                    objProposal.IMS_STA_REMARKS = objProposal1.IMS_STA_REMARKS;
                    objProposal.IMS_SANCTIONED_BY = objProposal1.IMS_SANCTIONED_BY;
                    objProposal.IMS_SANCTIONED_DATE = objProposal1.IMS_SANCTIONED_DATE;
                    objProposal.IMS_YEAR = objProposal1.IMS_YEAR;
                    objProposal.IMS_BATCH = objProposal1.IMS_BATCH;
                   

                    objProposal.IMS_FINAL_PAYMENT_FLAG = objProposal1.IMS_FINAL_PAYMENT_FLAG;
                    objProposal.IMS_LOCK_STATUS = objProposal1.IMS_LOCK_STATUS;
                    objProposal.IMS_FREEZE_STATUS = objProposal1.IMS_FREEZE_STATUS;
                    objProposal.IMS_SHIFT_STATUS = objProposal1.IMS_SHIFT_STATUS;
                    objProposal.PTA_SANCTIONED = objProposal1.PTA_SANCTIONED;
                    objProposal.IMS_DPR_STATUS = objProposal1.IMS_DPR_STATUS;
                    objProposal.IMS_ZP_RESO_OBTAINED = objProposal1.IMS_ZP_RESO_OBTAINED;
                    objProposal.IMS_IS_STAGED = objProposal1.IMS_IS_STAGED;
                    objProposal.IMS_ISBENEFITTED_HABS = objProposal1.IMS_ISBENEFITTED_HABS;
                    objProposal.IMS_SANCTIONED_PAV_AMT = objProposal1.IMS_SANCTIONED_PAV_AMT;
                    objProposal.IMS_SANCTIONED_CD_AMT = objProposal1.IMS_SANCTIONED_CD_AMT;
                    objProposal.IMS_SANCTIONED_PW_AMT = objProposal1.IMS_SANCTIONED_PW_AMT;
                    objProposal.IMS_SANCTIONED_OW_AMT = objProposal1.IMS_SANCTIONED_OW_AMT;
                   // objProposal.IMS_SANCTIONED_BW_AMT = objProposal1.IMS_SANCTIONED_BW_AMT;
                    objProposal.IMS_PARTIAL_LEN = objProposal1.IMS_PARTIAL_LEN;
                    objProposal.IMS_PAV_LENGTH = objProposal1.IMS_PAV_LENGTH;
                    objProposal.IMS_PAV_EST_COST = objProposal1.IMS_PAV_EST_COST;
                    objProposal.IMS_NO_OF_CDWORKS = objProposal1.IMS_NO_OF_CDWORKS;
                    objProposal.IMS_CD_WORKS_EST_COST = objProposal1.IMS_CD_WORKS_EST_COST;
                    objProposal.IMS_PROTECTION_WORKS = objProposal1.IMS_PROTECTION_WORKS;
                    objProposal.IMS_OTHER_WORK_COST = objProposal1.IMS_OTHER_WORK_COST;
                    objProposal.IMS_STATE_SHARE = objProposal1.IMS_STATE_SHARE;
                }

                objProposal.IMS_SHARE_PERCENT_2015 = ims_sanctioned_projects.IMS_SHARE_PERCENT_2015;
                objProposal.IMS_STATE_SHARE_2015 = ims_sanctioned_projects.IMS_STATE_SHARE_2015;
                objProposal.IMS_MORD_SHARE_2015 = ims_sanctioned_projects.IMS_MORD_SHARE_2015;

                return objLSBProposalDAL.UpdateLSBProposalDAL(objProposal);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                ErrorLog.LogError(ex, "UpdateLSBProposalBAL");
                return "An Error Occurred While Your Processing Request.";
            }
            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// Delete the LSB Proposal
        /// Check if Proposal can be deleted else return an error Message        
        /// </summary>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        /// <returns></returns>
        public string DeleteLSBProposalBAL(int IMS_PR_ROAD_CODE)
        {
            try
            {
                db = new PMGSYEntities();
                if (db.IMS_LSB_BRIDGE_COMPONENT_DETAIL.Where(c => c.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Count() > 0)
                    return "Bridge Component Details are added against Proposal, Proposal can not be deleted.";
                if (db.IMS_LSB_BRIDGE_DETAIL.Where(c => c.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Count() > 0)
                    return "Bridge Other Details are added against Proposal, Proposal can not be deleted.";
                if (db.IMS_PROPOSAL_FILES.Where(c => c.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Any())
                    return "Files are uploaded against Proposal, Proposal can not be deleted.";
                //new condition added by Vikram as suggested by Dev Sir on 02 July 2014 -- if the current road which is to be deleted is present in other proposal road (as upgradation or staged construction then dont delete the road.)
                if (db.IMS_SANCTIONED_PROJECTS.Any(c => c.IMS_STAGED_ROAD_ID == IMS_PR_ROAD_CODE))
                    return "Stage II / LSB proposal is present against this Proposal, so Proposal can not be deleted.";
                else
                    return objLSBProposalDAL.DeleteLSBProposalDAL(IMS_PR_ROAD_CODE);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("IMS_PR_ROAD_CODE :" + IMS_PR_ROAD_CODE);
                    sw.WriteLine("======================================================================");
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "DeleteLSBConfirmedBAL()");
                    if (ex != null)
                        sw.WriteLine("Exception : " + ex.ToString());
                    if (ex.InnerException != null)
                        sw.WriteLine("innerException : " + ex.InnerException.ToString());
                    if (ex.InnerException.InnerException != null)
                        sw.WriteLine("ex.InnerException.InnerException : " + ex.InnerException.InnerException.ToString());
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }
                return "An Error Occurred While  Processing Your Request.";
            }
            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// This Function Validates the Lengths in Other Details
        /// </summary>
        /// <param name="lsbOthDetailsModel"></param>
        /// <returns></returns>
        public string ValidateOtherDetails(LSBOthDetailsModel lsbOthDetailsModel)
        {
            if (lsbOthDetailsModel.IMS_HIGHEST_FLOOD_LEVEL > lsbOthDetailsModel.IMS_ROAD_TYPE_LEVEL)
            {
                return ("Highest Flood level(HFL) should be less than Road Top level(RTL)");
            }
            if (lsbOthDetailsModel.IMS_ORDINARY_FLOOD_LEVEL > lsbOthDetailsModel.IMS_HIGHEST_FLOOD_LEVEL)
            {
                return ("Ordinary Flood level(OFL) should be less than Highest Flood level(HFL)");
            }

            if (lsbOthDetailsModel.IMS_AVERAGE_GROUND_LEVEL > lsbOthDetailsModel.IMS_ORDINARY_FLOOD_LEVEL)
            {
                return ("Average Ground level(AGL) should be less than Ordinary Flood level(OFL)");
            }

            if (lsbOthDetailsModel.IMS_NALA_BED_LEVEL > lsbOthDetailsModel.IMS_AVERAGE_GROUND_LEVEL)
            {
                return ("Nala Bed level(NBL) should be less than Average Ground level(AGL)");
            }

            if (lsbOthDetailsModel.IMS_FOUNDATION_LEVEL > lsbOthDetailsModel.IMS_NALA_BED_LEVEL)
            {
                return ("Foundation level(FL) should be less than Nala Bed level(NBL)");
            }
            return string.Empty;
        }

        /// <summary>
        /// Save LSB Other Details
        /// </summary>
        /// <param name="lsbOthDetailsModel"></param>
        /// <returns></returns>
        public string LSBOtherDetailsBAL(LSBOthDetailsModel lsbOthDetailsModel)
        {
            decimal totalEstCost = 0;
            try
            {
                db = new PMGSYEntities();
                var ims_sanctioned_projects = db.IMS_SANCTIONED_PROJECTS.Find(lsbOthDetailsModel.IMS_PR_ROAD_CODE);

                string Status = ValidateOtherDetails(lsbOthDetailsModel);
                if (Status != string.Empty)
                {
                    return Status;
                }

                /// Change by SAMMED PATIL according to Pankaj Sirs requirement
                if (PMGSYSession.Current.PMGSYScheme == 1 || PMGSYSession.Current.PMGSYScheme == 3)///Changes for RCPLWE
                {
                    if (ims_sanctioned_projects.IMS_YEAR < 2015)
                    {
                        totalEstCost = (Convert.ToDecimal(ims_sanctioned_projects.IMS_HIGHER_SPECIFICATION_COST) + Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_BW_AMT) + Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_BS_AMT));

                    }
                    else if (ims_sanctioned_projects.IMS_YEAR >= 2015)
                    {
                        totalEstCost = (Convert.ToDecimal(ims_sanctioned_projects.IMS_HIGHER_SPECIFICATION_COST) + Convert.ToDecimal(ims_sanctioned_projects.IMS_MORD_SHARE_2015) + Convert.ToDecimal(ims_sanctioned_projects.IMS_STATE_SHARE_2015) + Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_BS_AMT));
                    }
                }
                // Added by Srishti
                else if (PMGSYSession.Current.PMGSYScheme == 2 || PMGSYSession.Current.PMGSYScheme == 4 || PMGSYSession.Current.PMGSYScheme == 5)//PMGSY3
                {
                    if (ims_sanctioned_projects.IMS_YEAR < 2015)
                    {
                        totalEstCost = (Convert.ToDecimal(ims_sanctioned_projects.IMS_HIGHER_SPECIFICATION_COST) + Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_BW_AMT) + Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_BS_AMT));
                    }
                    else if (ims_sanctioned_projects.IMS_YEAR >= 2015)
                    {
                        totalEstCost = (Convert.ToDecimal(ims_sanctioned_projects.IMS_HIGHER_SPECIFICATION_COST) + Convert.ToDecimal(ims_sanctioned_projects.IMS_MORD_SHARE_2015) + Convert.ToDecimal(ims_sanctioned_projects.IMS_STATE_SHARE_2015));
                    }
                }

                if (lsbOthDetailsModel.TotalEstimatedCost > totalEstCost)
                {
                    return "Total Estimated Cost should be less than the Sum of State Cost and MoRD Share.";
                }

                //if (lsbOthDetailsModel.TotalEstimatedCost > (ims_sanctioned_projects.IMS_BRIDGE_WORKS_EST_COST + ims_sanctioned_projects.IMS_BRIDGE_EST_COST_STATE))
                //{
                //    return "Total Estimated Cost should be less than the Sum of State Cost and MoRD Share.";
                //}




                IMS_LSB_BRIDGE_DETAIL ims_bridge_detail = new IMS_LSB_BRIDGE_DETAIL();
                ims_bridge_detail.IMS_PR_ROAD_CODE = lsbOthDetailsModel.IMS_PR_ROAD_CODE;
                ims_bridge_detail.IMS_ROAD_TYPE_LEVEL = lsbOthDetailsModel.IMS_ROAD_TYPE_LEVEL;
                ims_bridge_detail.IMS_AVERAGE_GROUND_LEVEL = lsbOthDetailsModel.IMS_AVERAGE_GROUND_LEVEL;
                ims_bridge_detail.IMS_NALA_BED_LEVEL = lsbOthDetailsModel.IMS_NALA_BED_LEVEL;
                ims_bridge_detail.IMS_HIGHEST_FLOOD_LEVEL = lsbOthDetailsModel.IMS_HIGHEST_FLOOD_LEVEL;
                ims_bridge_detail.IMS_ORDINARY_FLOOD_LEVEL = lsbOthDetailsModel.IMS_ORDINARY_FLOOD_LEVEL;
                ims_bridge_detail.IMS_FOUNDATION_LEVEL = lsbOthDetailsModel.IMS_FOUNDATION_LEVEL;
                ims_bridge_detail.IMS_HGT_BIRDGE_NBL = lsbOthDetailsModel.IMS_HGT_BIRDGE_NBL;
                ims_bridge_detail.IMS_HGT_BRIDGE_FL = lsbOthDetailsModel.IMS_HGT_BRIDGE_FL;
                ims_bridge_detail.IMS_BRG_SUBMERSIBLE = lsbOthDetailsModel.IMS_BRG_SUBMERSIBLE;
                ims_bridge_detail.IMS_BRG_BOX_CULVERT = lsbOthDetailsModel.IMS_BRG_BOX_CULVERT;
                ims_bridge_detail.IMS_BRG_RCC_ABUMENT = lsbOthDetailsModel.IMS_BRG_RCC_ABUMENT;
                ims_bridge_detail.IMS_BRG_HLB = lsbOthDetailsModel.IMS_BRG_HLB;
                ims_bridge_detail.IMS_SC_FD_CODE = lsbOthDetailsModel.IMS_SC_FD_CODE;
                ims_bridge_detail.IMS_BEARING_CAPACITY = lsbOthDetailsModel.IMS_BEARING_CAPACITY;
                ims_bridge_detail.IMS_ARG_TOT_SPANS = lsbOthDetailsModel.IMS_ARG_TOT_SPANS;
                ims_bridge_detail.IMS_NO_OF_VENTS = lsbOthDetailsModel.IMS_NO_OF_VENTS;
                ims_bridge_detail.IMS_SPAN_VENT = lsbOthDetailsModel.IMS_SPAN_VENT;
                ims_bridge_detail.IMS_SCOUR_DEPTH = lsbOthDetailsModel.IMS_SCOUR_DEPTH;
                ims_bridge_detail.IMS_WIDTH_OF_BRIDGE = lsbOthDetailsModel.IMS_WIDTH_OF_BRIDGE;
                ims_bridge_detail.IMS_APPROACH_COST = lsbOthDetailsModel.IMS_APPROACH_COST;
                ims_bridge_detail.IMS_BRGD_STRUCTURE_COST = lsbOthDetailsModel.IMS_BRGD_STRUCTURE_COST;
                ims_bridge_detail.IMS_STRUCTURE_COST = lsbOthDetailsModel.IMS_STRUCTURE_COST;
                ims_bridge_detail.IMS_BRGD_OTHER_COST = lsbOthDetailsModel.IMS_BRGD_OTHER_COST;
                ims_bridge_detail.IMS_APPROACH_PER_MTR = lsbOthDetailsModel.IMS_APPROACH_PER_MTR;
                ims_bridge_detail.IMS_BRGD_STRUCTURE_PER_MTR = lsbOthDetailsModel.IMS_BRGD_STRUCTURE_PER_MTR;
                ims_bridge_detail.IMS_STRUCTURE_PER_MTR = lsbOthDetailsModel.IMS_STRUCTURE_PER_MTR;
                ims_bridge_detail.IMS_BRGD_OTHER_PER_MTR = lsbOthDetailsModel.IMS_BRGD_OTHER_PER_MTR;


                return objLSBProposalDAL.LSBOtherDetailsDAL(ims_bridge_detail, lsbOthDetailsModel.OPERATION);
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                return "An Error Occurred While Your Processing Request.";
            }
            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// Delete the LSB Other Details
        /// </summary>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        /// <returns></returns>
        public string DeleteLSBOthDetailsBAL(int IMS_PR_ROAD_CODE)
        {
            try
            {
                return objLSBProposalDAL.DeleteLSBOthDetailsBAL(IMS_PR_ROAD_CODE);
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                return "An Error Occurred While Your Processing Request.";
            }
            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// Save Component Details
        /// </summary>
        /// <param name="lsbComponentModel"></param>
        /// <returns></returns>
        public string LSBComponentDetailsBAL(LSBComponentModel lsbComponentModel)
        {
            IMS_LSB_BRIDGE_COMPONENT_DETAIL ims_component_detail = new IMS_LSB_BRIDGE_COMPONENT_DETAIL();
            ims_component_detail.IMS_PR_ROAD_CODE = lsbComponentModel.IMS_PR_ROAD_CODE;
            ims_component_detail.IMS_COMPONENT_CODE = lsbComponentModel.IMS_COMPONENT_CODE;
            ims_component_detail.IMS_QUANTITY = lsbComponentModel.IMS_QUANTITY;
            ims_component_detail.IMS_TOTAL_COST = Convert.ToDecimal(lsbComponentModel.IMS_TOTAL_COST);
            ims_component_detail.IMS_GRADE_CONCRETE = lsbComponentModel.IMS_GRADE_CONCRETE;


            return objLSBProposalDAL.LSBComponentDetailsDAL(ims_component_detail, lsbComponentModel.OPERATION);
        }

        /// <summary>
        /// Array of Component Details
        /// </summary>
        /// <param name="roadId"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array LSBComponentList(int roadId, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                return objLSBProposalDAL.LSBComponentList(roadId, page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);

                throw Ex;
            }
        }

        /// <summary>
        /// Delete the Road Proposal
        /// Check if Proposal can be deleted else return an error Message        
        /// </summary>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        /// <returns></returns>
        public string DeleteLSBComponentBAL(int IMS_PR_ROAD_CODE, int IMS_COMPONENT_CODE)
        {
            try
            {
                db = new PMGSYEntities();
                return objLSBProposalDAL.DeleteLSBComponentDAL(IMS_PR_ROAD_CODE, IMS_COMPONENT_CODE);
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                return "An Error Occurred While Your Processing Request.";
            }
            finally
            {
                db.Dispose();
            }
        }


        /// <summary>
        /// DPIU Finalize LSB Proposal
        /// </summary>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        /// <returns></returns>
        public string DPIUFinalizeProposalBAL(int IMS_PR_ROAD_CODE)
        {
            return objLSBProposalDAL.DPIUFinalizeProposalDAL(IMS_PR_ROAD_CODE);
        }

        /// <summary>
        /// Check for LSB Details are Entered or Not 
        /// </summary>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        /// <returns></returns>
        public string GetLSBProposalChecksBAL(int IMS_PR_ROAD_CODE)
        {
            try
            {
                db = new PMGSYEntities();

                // Check whether habitations are added 
                if (!db.IMS_BENEFITED_HABS.Where(a => a.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Any())
                {
                    return "Habitation Details are not added against Proposal, Please Add Habitation Details.";
                }
                // Check for CBR Details Entered 
                if (!db.IMS_CBR_VALUE.Where(c => c.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Any())
                {
                    return "CBR Value Details are not added against Proposal, Please add CBR Value Details";
                }
                // check for Traffic Intensity 
                if (!db.IMS_TRAFFIC_INTENSITY.Where(c => c.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Any())
                {
                    return "Traffice Intensity Details are not added against Proposal, Please Add Traffic Intensity Details.";
                }

                // Check whether habitations are Finalized 
                String IMS_IS_COMLPLETED = String.Empty;
                IMS_IS_COMLPLETED = db.IMS_SANCTIONED_PROJECTS.Where(a => a.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(c => c.IMS_ISCOMPLETED).First().ToUpper();
                if (IMS_IS_COMLPLETED != "H")
                {
                    return "Habitations are not Finalized, Please Finalize Habitation to Finalize Proposal.";
                }

                // Check for CBR Details 
                decimal EnterdCBRValues = 0;
                decimal PavementLength = db.IMS_SANCTIONED_PROJECTS.Where(a => a.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(c => c.IMS_PAV_LENGTH).First();

                if (db.IMS_CBR_VALUE.Where(c => c.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Any())
                {
                    EnterdCBRValues = db.IMS_CBR_VALUE.Where(c => c.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).AsEnumerable().Sum(c => c.IMS_END_CHAIN - c.IMS_STR_CHAIN);

                    if (EnterdCBRValues < PavementLength)
                    {
                        return "Please Enter the Complete CBR Details \nTotal Segment Length in CBR Details :" + EnterdCBRValues + "\nPavement Length : " + PavementLength;
                    }
                }

                return string.Empty;
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                return "An Error Occurred While Your Processing Request.";
            }
            finally
            {
                db.Dispose();
            }
        }

        #endregion

        #region STA Mord
        /// <summary>
        /// Lists the LSB Proposal
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="MAST_STATE_CODE"></param>
        /// <param name="IMS_YEAR"></param>
        /// <param name="MAST_DISTRICT_ID"></param>
        /// <param name="IMS_BATCH"></param>
        /// <param name="IMS_STREAMS"></param>
        /// <param name="IMS_PROPOSAL_TYPE"></param>
        /// <param name="IMS_PROPOSAL_STATUS"></param>
        /// <returns></returns>
        public Array GetSTALSBProposalsBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int MAST_STATE_CODE, int IMS_YEAR, int MAST_DISTRICT_ID, int IMS_BATCH, int IMS_STREAMS, string IMS_PROPOSAL_TYPE, string IMS_PROPOSAL_STATUS)
        {
            return objLSBProposalDAL.GetSTALSBProposalsDAL(page, rows, sidx, sord, out totalRecords, MAST_STATE_CODE, IMS_YEAR, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAMS, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS);
        }

        /// <summary>
        /// STA Scrtinize Method
        /// </summary>
        /// <param name="staSanctionViewModel"></param>
        /// <param name="ProposalStatus"></param>
        /// <returns></returns>
        public string StaFinalizeLSBProposalBAL(StaLSBSanctionViewModel staSanctionViewModel, String ProposalStatus)
        {
            return objLSBProposalDAL.STAFinalizeLSBProposalDAL(staSanctionViewModel, ProposalStatus);
        }

        /// <summary>
        /// Enlists the MoRD Propsoals
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="IMS_YEAR"></param>
        /// <param name="MAST_STATE_ID"></param>
        /// <param name="MAST_DISTRICT_ID"></param>
        /// <param name="IMS_BATCH"></param>
        /// <param name="IMS_STREAMS"></param>
        /// <param name="IMS_PROPOSAL_TYPE"></param>
        /// <param name="IMS_PROPOSAL_STATUS"></param>
        /// <returns></returns>
        public Array GetMordLSBProposalsBAL(int page, int rows, string sidx, string sord, out int totalRecords, int IMS_YEAR, int MAST_STATE_ID, int MAST_DISTRICT_ID, int IMS_BATCH, int IMS_STREAMS, string IMS_PROPOSAL_TYPE, string IMS_PROPOSAL_STATUS, int IMS_AGENCY, string IMS_UPGRADE_CONNECT, out ProposalColumnsTotal totalColModel)
        {
            if (PMGSYSession.Current.RoleCode == 36)
            {
                IMS_PROPOSAL_STATUS = "Y";
            }

            return objLSBProposalDAL.GetMordLSBProposalsDAL(page, rows, sidx, sord, out totalRecords, IMS_YEAR, MAST_STATE_ID, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAMS, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_AGENCY, IMS_UPGRADE_CONNECT, out totalColModel);
        }


        /// <summary>
        /// Update Details of LSB Propsoal by Mord
        /// </summary>
        /// <param name="mordSanctionViewModel"></param>
        /// <returns></returns>
        public string UpdateMordLSBSanctionDetailsBAL(MordLSBSanctionViewModel mordSanctionViewModel)
        {
            if (mordSanctionViewModel.IMS_SANCTIONED == "R" || mordSanctionViewModel.IMS_SANCTIONED == "D")
            {
                if (mordSanctionViewModel.IMS_REASON == null || mordSanctionViewModel.IMS_REASON == 0)
                {
                    return "   Please Select Reason.";
                }
            }

            return objLSBProposalDAL.UpdateMordLSBSanctionDetailsDAL(mordSanctionViewModel);
        }

        #endregion

        #region PTA

        /// <summary>
        /// PTA Scrtinize Method
        /// </summary>
        /// <param name="ptaSanctionViewModel"></param>
        /// <param name="ProposalStatus"></param>
        /// <returns></returns>
        public string PtaFinalizeLSBProposalBAL(PtaLSBSanctionViewModel ptaSanctionViewModel, String ProposalStatus)
        {
            return objLSBProposalDAL.PTAFinalizeLSBProposalDAL(ptaSanctionViewModel, ProposalStatus);
        }

        #endregion


        /// <summary>
        /// Update the Unlocked Proposal
        /// </summary>
        /// <param name="ims_sanctioned_projects"></param>
        /// <returns></returns>
        public string UpdateUnlockedProposalBAL(UnlockLSBViewModel ims_sanctioned_projects)
        {
            return objLSBProposalDAL.UpdateUnlockedProposalDAL(ims_sanctioned_projects);
        }


        public MordLSBSanctionViewModel GetBulkMordDetailBAL(string IMS_PR_ROAD_CODES)
        {
            return objLSBProposalDAL.GetBulkMordDetailDAL(IMS_PR_ROAD_CODES);
        }
    }


    public interface ILSBProposalBAL
    {
        #region LSB Proposal
        Array GetLSBProposalsForSRRDABAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int MAST_STATE_CODE, int MAST_DISTRICT_CODE, int IMS_YEAR, int IMS_BATCH, int IMS_STREAMS, String ProposalType, string IMS_PROPOSAL_STATUS, string IMS_UPGRADE_CONNECT, int MAST_BLOCK_CODE, out ProposalColumnsTotal model);
        string SaveLSBProposalBAL(PMGSY.Models.Proposal.LSBViewModel ims_sanctioned_projects);
        string UpdateLSBProposalBAL(PMGSY.Models.Proposal.LSBViewModel ims_sanctioned_projects);
        string DeleteLSBProposalBAL(int IMS_PR_ROAD_CODE);
        string LSBOtherDetailsBAL(LSBOthDetailsModel lsbOthDetailsModel);
        string LSBComponentDetailsBAL(LSBComponentModel lsbComponentModel);
        Array LSBComponentList(int roadId, int? page, int? rows, string sidx, string sord, out long totalRecords);
        string DeleteLSBComponentBAL(int IMS_PR_ROAD_CODE, int IMS_COMPONENT_CODE);
        string UpdateUnlockedProposalBAL(UnlockLSBViewModel ims_sanctioned_projects);

        MordLSBSanctionViewModel GetBulkMordDetailBAL(string IMS_PR_ROAD_CODES);
        string PtaFinalizeLSBProposalBAL(PtaLSBSanctionViewModel ptaSanctionViewModel, String ProposalStatus);
        #endregion
    }

}