#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   ProposalBAL.cs
        * Description   :   BAL Methods to call DAL methods of  Creating , Editing, Deleting Road Proposal and Related Screens of Road Proposals Habitation Details
                            Traffic Intensity , CBR Index and File Upload   
        * Author        :   Shivkumar Deshmukh        
        * Creation Date :   18/June/2013
        * Modified By   :   Shyam Yadav
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
using ImageResizer;
using System.IO;
using System.Drawing;
using System.Web.Mvc;
using System.Configuration;
//using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects;
using PMGSY.Common;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Data.Entity.Core;

namespace PMGSY.BAL.Proposal
{
    public class ProposalBAL : IProposalBAL
    {
        #region Variable Declaration
        IProposalDAL objProposalDAL = new ProposalDAL();
        private PMGSYEntities db = new PMGSYEntities();
        #endregion

        public bool checkIsPaymentBAL(int prRoadCode)
        {
            return objProposalDAL.checkIsPaymentDAL(prRoadCode);
        }

        #region Road Proposal Data Entry

        /// <summary>
        /// Lists the Road Proposal
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
        /// <param name="MAST_DPIU_CODE"></param>
        /// <returns></returns>
        public Array GetProposalsBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int MAST_STATE_CODE, int MAST_DISTRICT_CODE, int IMS_YEAR, int MAST_BLOCK_CODE, int IMS_BATCH, int IMS_STREAMS, String ProposalType, int MAST_DPIU_CODE, string IMS_PROPOSAL_STATUS, string IMS_UPGRADE_CONNECT, string Filters, out ProposalColumnsTotal colTotal)
        {
            return objProposalDAL.GetProposalsDAL(page, rows, sidx, sord, out totalRecords, MAST_STATE_CODE, MAST_DISTRICT_CODE, IMS_YEAR, MAST_BLOCK_CODE, IMS_BATCH, IMS_STREAMS, ProposalType, MAST_DPIU_CODE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT, Filters, out colTotal);
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
        /// <param name="MAST_DPIU_CODE"></param>
        /// <param name="Filters"></param>
        /// <returns></returns>
        public Array GetProposalsForSRRDABAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int MAST_STATE_CODE, int MAST_DISTRICT_CODE, int IMS_YEAR, int IMS_BATCH, int IMS_STREAMS, String ProposalType, string IMS_PROPOSAL_STATUS, string IMS_UPGRADE_CONNECT, int MAST_BLOCK_CODE, string Filters, out ProposalColumnsTotal totalColModel)
        {
            return objProposalDAL.GetProposalsForSRRDADAL(page, rows, sidx, sord, out totalRecords, MAST_STATE_CODE, MAST_DISTRICT_CODE, IMS_YEAR, IMS_BATCH, IMS_STREAMS, ProposalType, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT, MAST_BLOCK_CODE, Filters, out totalColModel);
        }


        /// <summary>
        /// Save the Road Proposal
        /// </summary>
        /// <param name="ims_sanctioned_projects"></param>
        /// <returns></returns>
        public string SaveRoadProposalBAL(PMGSY.Models.Proposal.ProposalViewModel ims_sanctioned_projects)
        {
            IMS_SANCTIONED_PROJECTS objProposal = new IMS_SANCTIONED_PROJECTS();
            try
            {
                objProposal.MAST_PMGSY_SCHEME = PMGSYSession.Current.PMGSYScheme;// == 3 ? (byte)1 : PMGSYSession.Current.PMGSYScheme;///Changed for RCPLWE
                objProposal.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                //objProposal.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;

                // Changes by Saurabh Start here.. 
                if (PMGSYSession.Current.DistrictCode == 0)
                {
                    return "Invalid District Code";
                }
                else
                {
                    objProposal.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                }
                // Changes by Saurabh End Here.

                objProposal.MAST_DPIU_CODE = PMGSYSession.Current.AdminNdCode;

                objProposal.IMS_UPGRADE_CONNECT = ims_sanctioned_projects.IMS_UPGRADE_CONNECT;

                // Upgradation Proposal
                if (ims_sanctioned_projects.IMS_UPGRADE_CONNECT.ToUpper() == "U")
                {
                    objProposal.MAST_EXISTING_SURFACE_CODE = ims_sanctioned_projects.MAST_EXISTING_SURFACE_CODE;

                    // is Habitations Benefited
                    if (ims_sanctioned_projects.IMS_ISBENEFITTED_HABS.ToUpper() == "N")
                    {
                        objProposal.IMS_ISBENEFITTED_HABS = ims_sanctioned_projects.IMS_ISBENEFITTED_HABS;
                        objProposal.IMS_HABS_REASON = ims_sanctioned_projects.IMS_HABS_REASON;
                    }
                    else
                    {
                        objProposal.IMS_ISBENEFITTED_HABS = ims_sanctioned_projects.IMS_ISBENEFITTED_HABS;
                    }
                }
                else
                {
                    objProposal.MAST_EXISTING_SURFACE_CODE = null;
                    objProposal.IMS_ISBENEFITTED_HABS = "Y";
                }

                // Existing Package or New Package
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

                // Staged Proposal or Complete Proposal
                if (ims_sanctioned_projects.IMS_IS_STAGED != null && ims_sanctioned_projects.IMS_IS_STAGED.ToUpper() == "S")
                {
                    objProposal.IMS_IS_STAGED = "S";
                    //Stage I Proposal
                    if (ims_sanctioned_projects.IMS_STAGE_PHASE == "1")
                    {
                        objProposal.IMS_STAGE_PHASE = "S1";
                    }
                    if (ims_sanctioned_projects.IMS_STAGE_PHASE == "2")
                    {
                        objProposal.IMS_STAGE_PHASE = "S2";

                        //IMS_YEAR_Staged
                        var data = (from c in db.IMS_SANCTIONED_PROJECTS
                                    where
                                        c.IMS_PACKAGE_ID == ims_sanctioned_projects.Stage_2_Package_ID
                                        &&
                                        c.IMS_YEAR == ims_sanctioned_projects.Stage_2_Year
                                        &&
                                        c.PLAN_CN_ROAD_CODE == ims_sanctioned_projects.PLAN_CN_ROAD_CODE
                                        && c.IMS_STAGED_ROAD_ID == ims_sanctioned_projects.IMS_STAGED_ROAD_ID
                                    select new
                                    {
                                        Package_ID = ims_sanctioned_projects.Stage_2_Package_ID,
                                        Year = ims_sanctioned_projects.Stage_2_Year,
                                        Road_ID = c.IMS_PR_ROAD_CODE
                                    }).ToList();
                        // Self Referencing Columns
                        foreach (var RowData in data)
                        {
                            objProposal.IMS_STAGED_YEAR = RowData.Year;
                            objProposal.IMS_STAGED_PACKAGE_ID = RowData.Package_ID.ToString();
                            objProposal.IMS_STAGED_ROAD_ID = RowData.Road_ID;
                        }

                        //validation for checking the total length of stage 2 proposals should not exceed the stage 1 proposal

                        objProposal.IMS_STAGED_ROAD_ID = ims_sanctioned_projects.IMS_STAGED_ROAD_ID;
                        //if (db.IMS_SANCTIONED_PROJECTS.Any(m => m.IMS_STAGED_ROAD_ID == objProposal.IMS_STAGED_ROAD_ID && m.IMS_STAGE_PHASE == "S2"))
                        if (db.IMS_SANCTIONED_PROJECTS.Any(m => m.IMS_STAGED_ROAD_ID == ims_sanctioned_projects.IMS_STAGED_ROAD_ID && m.IMS_STAGE_PHASE == "S2"))
                        {
                            //decimal sumOfPavLength = db.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_STAGED_ROAD_ID == objProposal.IMS_STAGED_ROAD_ID && m.IMS_STAGE_PHASE == "S2").Sum(m => m.IMS_PAV_LENGTH) == null ? 0 : db.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_STAGED_ROAD_ID == objProposal.IMS_STAGED_ROAD_ID && m.IMS_STAGE_PHASE == "S2").Sum(m => m.IMS_PAV_LENGTH);

                            decimal sumOfPavLength = db.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_STAGED_ROAD_ID == ims_sanctioned_projects.IMS_STAGED_ROAD_ID && m.IMS_STAGE_PHASE == "S2").Sum(m => m.IMS_PAV_LENGTH) == null ? 0 : db.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_STAGED_ROAD_ID == ims_sanctioned_projects.IMS_STAGED_ROAD_ID && m.IMS_STAGE_PHASE == "S2").Sum(m => m.IMS_PAV_LENGTH);

                            sumOfPavLength += ims_sanctioned_projects.IMS_PAV_LENGTH.Value;

                            if (sumOfPavLength > (db.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == objProposal.IMS_STAGED_ROAD_ID.Value).Select(m => m.IMS_PAV_LENGTH).FirstOrDefault()))
                            {
                                //return "Sum of Pavement Length of Stage 2 Proposals is exceeding the Pavement Length of Stage 1 Proposal.";
                                var stage2Roads = db.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_STAGED_ROAD_ID == ims_sanctioned_projects.IMS_STAGED_ROAD_ID && m.IMS_STAGE_PHASE == "S2").Select(x => new { imsYear = x.IMS_YEAR, imsPackage = x.IMS_PACKAGE_ID, blockName = x.MASTER_BLOCK.MAST_BLOCK_NAME }).ToList();

                                string stage2Year = string.Join(",", stage2Roads[0].imsYear);
                                string stage2Package = string.Join(",", stage2Roads[0].imsPackage);
                                string stage2Block = string.Join(",", stage2Roads[0].blockName);

                                return "Sum of Pavement Length of Stage 2 Proposals is exceeding the Pavement Length of Stage 1 Proposal.<br/> Package=[" + stage2Package + "]     Year=[" + stage2Year + "]     Block=[" + stage2Block + "]";
                            }

                        }

                        //validation for checking whether the Proposal length is exceeding the core network lenth.

                        decimal? totalCnLength = 0;

                        if (PMGSYSession.Current.PMGSYScheme == 1)
                        {
                            totalCnLength = (from c in db.PLAN_ROAD
                                             where c.PLAN_CN_ROAD_CODE == ims_sanctioned_projects.PLAN_CN_ROAD_CODE
                                             select c.PLAN_RD_LENGTH).First();
                        }
                        else if (PMGSYSession.Current.PMGSYScheme == 2 || PMGSYSession.Current.PMGSYScheme == 3)///Changes for RCPLWE
                        {
                            // For scheme 3 - RCPLWE Road can be combination of roads so,
                            // For scheme 2 - Candidate Road can be combination of roads so,
                            // Length is considered as Total Length i.e. PLAN_RD_TOTAL_LEN
                            totalCnLength = (from c in db.PLAN_ROAD
                                             where c.PLAN_CN_ROAD_CODE == ims_sanctioned_projects.PLAN_CN_ROAD_CODE
                                             select c.PLAN_RD_TOTAL_LEN).First();
                        }

                        decimal? ProposedLength = (from c in db.IMS_SANCTIONED_PROJECTS
                                                   where c.PLAN_CN_ROAD_CODE == ims_sanctioned_projects.PLAN_CN_ROAD_CODE
                                                   && c.IMS_PROPOSAL_TYPE == "P"
                                                   && c.IMS_SANCTIONED != "D"
                                                   && ((c.IMS_STAGE_PHASE == null ? "1" : c.IMS_STAGE_PHASE) != (c.IMS_STAGE_PHASE == null ? "2" : "S2"))    //condition added by shyam
                                                   select (decimal?)c.IMS_PAV_LENGTH).Sum();

                        decimal? remainingLength = Convert.ToDecimal(totalCnLength) - Convert.ToDecimal(ProposedLength);

                        if (ims_sanctioned_projects.IMS_PAV_LENGTH > (totalCnLength + (totalCnLength * Convert.ToDecimal(0.5))))
                        {
                            return "variation in Proposed Length can be upto 50 % CN Length";
                        }
                    }
                }
                else // Complete Proposal
                {
                    objProposal.IMS_IS_STAGED = "C";
                }

                objProposal.IMS_YEAR = ims_sanctioned_projects.IMS_YEAR;
                objProposal.IMS_BATCH = ims_sanctioned_projects.IMS_BATCH;
                objProposal.MAST_BLOCK_CODE = ims_sanctioned_projects.MAST_BLOCK_CODE;

                // FUNDING AGENCY
                objProposal.IMS_COLLABORATION = ims_sanctioned_projects.IMS_COLLABORATION;
                // STREAM
                objProposal.IMS_STREAMS = ims_sanctioned_projects.IMS_STREAMS;
                // Link/Through Route Name
                objProposal.PLAN_CN_ROAD_CODE = ims_sanctioned_projects.PLAN_CN_ROAD_CODE != null ? Convert.ToInt32(ims_sanctioned_projects.PLAN_CN_ROAD_CODE) : 0;

                objProposal.IMS_PARTIAL_LEN = ims_sanctioned_projects.IMS_PARTIAL_LEN;


                objProposal.IMS_ROAD_FROM = ims_sanctioned_projects.IMS_ROAD_FROM;
                objProposal.IMS_ROAD_TO = ims_sanctioned_projects.IMS_ROAD_TO;
                //objProposal.IMS_ROAD_NAME = ims_sanctioned_projects.IMS_ROAD_FROM + " " + ims_sanctioned_projects.IMS_ROAD_TO;

                ///Changed by SAMMED A. PATIL for RCPLWE
                if (objProposal.IMS_COLLABORATION == 5)
                {
                    objProposal.IMS_ROAD_NAME = db.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == ims_sanctioned_projects.PLAN_CN_ROAD_CODE).Select(m => m.PLAN_RD_NAME).FirstOrDefault();
                }
                else
                {
                    objProposal.IMS_ROAD_NAME = db.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == ims_sanctioned_projects.PLAN_CN_ROAD_CODE).Select(m => m.PLAN_CN_ROAD_NUMBER).FirstOrDefault() + "-" + db.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == ims_sanctioned_projects.PLAN_CN_ROAD_CODE).Select(m => m.PLAN_RD_NAME).FirstOrDefault();
                }

                objProposal.IMS_PAV_LENGTH = (ims_sanctioned_projects.IMS_PAV_LENGTH != null) ? Convert.ToDecimal(ims_sanctioned_projects.IMS_PAV_LENGTH) : 0;
                objProposal.IMS_NO_OF_CDWORKS = ims_sanctioned_projects.IMS_NO_OF_CDWORKS == null ? 0 : Convert.ToInt32(ims_sanctioned_projects.IMS_NO_OF_CDWORKS);
                objProposal.IMS_ZP_RESO_OBTAINED = ims_sanctioned_projects.IMS_ZP_RESO_OBTAINED;

                // All Costs Estimated ( For Reports Only, Don't Change them after Proposal has been Finalized by DPIU )
                objProposal.IMS_PAV_EST_COST = ims_sanctioned_projects.IMS_SANCTIONED_PAV_AMT;
                objProposal.IMS_CD_WORKS_EST_COST = ims_sanctioned_projects.IMS_SANCTIONED_CD_AMT;
                objProposal.IMS_PROTECTION_WORKS = ims_sanctioned_projects.IMS_SANCTIONED_PW_AMT;
                objProposal.IMS_OTHER_WORK_COST = ims_sanctioned_projects.IMS_SANCTIONED_OW_AMT;
                objProposal.IMS_STATE_SHARE = Convert.ToDecimal(ims_sanctioned_projects.IMS_STATE_SHARE);

                //PMGSY Scheme-II
                objProposal.IMS_IS_HIGHER_SPECIFICATION = ims_sanctioned_projects.IMS_IS_HIGHER_SPECIFICATION;
                if (objProposal.IMS_IS_HIGHER_SPECIFICATION != null && ims_sanctioned_projects.IMS_IS_HIGHER_SPECIFICATION.Equals("Y"))
                {
                    objProposal.IMS_HIGHER_SPECIFICATION_COST = ims_sanctioned_projects.IMS_HIGHER_SPECIFICATION_COST;
                }
                else
                {
                    objProposal.IMS_HIGHER_SPECIFICATION_COST = null;
                }
                objProposal.IMS_FURNITURE_COST = ims_sanctioned_projects.IMS_FURNITURE_COST;
                objProposal.IMS_SHARE_PERCENT = ims_sanctioned_projects.IMS_SHARE_PERCENT;

                objProposal.IMS_MAINTENANCE_YEAR1 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT1;
                objProposal.IMS_MAINTENANCE_YEAR2 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT2;
                objProposal.IMS_MAINTENANCE_YEAR3 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT3;
                objProposal.IMS_MAINTENANCE_YEAR4 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT4;
                objProposal.IMS_MAINTENANCE_YEAR5 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT5;

                //PMGSY Scheme-II
                objProposal.IMS_RENEWAL_COST = ims_sanctioned_projects.IMS_RENEWAL_COST;

                // All Costs Sanctioned ( Change this Costs iff Mord Changes Then) 
                objProposal.IMS_SANCTIONED_PAV_AMT = ims_sanctioned_projects.IMS_SANCTIONED_PAV_AMT;
                objProposal.IMS_SANCTIONED_CD_AMT = ims_sanctioned_projects.IMS_SANCTIONED_CD_AMT;
                objProposal.IMS_SANCTIONED_PW_AMT = ims_sanctioned_projects.IMS_SANCTIONED_PW_AMT;
                objProposal.IMS_SANCTIONED_OW_AMT = ims_sanctioned_projects.IMS_SANCTIONED_OW_AMT;
                objProposal.IMS_SANCTIONED_RS_AMT = Convert.ToDecimal(ims_sanctioned_projects.IMS_STATE_SHARE);
                //if (PMGSYSession.Current.PMGSYScheme == 1)
                //{
                //    objProposal.IMS_SANCTIONED_RS_AMT = Convert.ToDecimal(ims_sanctioned_projects.IMS_TOTAL_STATE_SHARE_2015);
                //}
                //else
                //{
                //    objProposal.IMS_SANCTIONED_RS_AMT = Convert.ToDecimal(ims_sanctioned_projects.IMS_STATE_SHARE);
                //}


                //PMGSY Scheme-II
                if (objProposal.IMS_IS_HIGHER_SPECIFICATION != null && ims_sanctioned_projects.IMS_IS_HIGHER_SPECIFICATION.Equals("Y"))
                {
                    objProposal.IMS_SANCTIONED_HS_AMT = ims_sanctioned_projects.IMS_HIGHER_SPECIFICATION_COST;
                }
                else
                {
                    objProposal.IMS_SANCTIONED_HS_AMT = null;
                }
                objProposal.IMS_SANCTIONED_FC_AMT = ims_sanctioned_projects.IMS_FURNITURE_COST;

                objProposal.IMS_SANCTIONED_MAN_AMT1 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT1;
                objProposal.IMS_SANCTIONED_MAN_AMT2 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT2;
                objProposal.IMS_SANCTIONED_MAN_AMT3 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT3;
                objProposal.IMS_SANCTIONED_MAN_AMT4 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT4;
                objProposal.IMS_SANCTIONED_MAN_AMT5 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT5;

                //PMGSY Scheme-II
                objProposal.IMS_SANCTIONED_RENEWAL_AMT = ims_sanctioned_projects.IMS_RENEWAL_COST;

                objProposal.MAST_MP_CONST_CODE = ims_sanctioned_projects.MAST_MP_CONST_CODE > 0 ? ims_sanctioned_projects.MAST_MP_CONST_CODE : null;
                objProposal.MAST_MLA_CONST_CODE = ims_sanctioned_projects.MAST_MLA_CONST_CODE > 0 ? ims_sanctioned_projects.MAST_MLA_CONST_CODE : null;

                // Carriage Width                
                objProposal.IMS_CARRIAGED_WIDTH = ims_sanctioned_projects.IMS_CARRIAGED_WIDTH;

                // Traffic Catagory
                objProposal.IMS_TRAFFIC_TYPE = ims_sanctioned_projects.IMS_TRAFFIC_TYPE == 0 ? null : ims_sanctioned_projects.IMS_TRAFFIC_TYPE;

                objProposal.IMS_PROPOSED_SURFACE = ims_sanctioned_projects.IMS_PROPOSED_SURFACE;
                objProposal.IMS_CC_LENGTH = ims_sanctioned_projects.IMS_CC_LENGTH;
                objProposal.IMS_BT_LENGTH = ims_sanctioned_projects.IMS_BT_LENGTH;
                objProposal.IMS_REMARKS = ims_sanctioned_projects.IMS_REMARKS;

                // Appear in case og Bridge Work
                objProposal.IMS_SANCTIONED_BW_AMT = 0;

                // not yet sanctioned by anyone
                objProposal.STA_SANCTIONED = "N";

                objProposal.IMS_SANCTIONED = "N";

                objProposal.IMS_STA_REMARKS = string.Empty;
                //Proposal Road 
                objProposal.IMS_PROPOSAL_TYPE = "P";

                // For Repackaging
                objProposal.IMS_DPR_STATUS = "N";

                // For Execution
                objProposal.IMS_FINAL_PAYMENT_FLAG = "N";

                // for Freezing 
                objProposal.IMS_FREEZE_STATUS = "U";

                // not locked
                objProposal.IMS_LOCK_STATUS = "N";

                // newly entered Proposal
                objProposal.IMS_ISCOMPLETED = "E";

                // Not Locked
                objProposal.IMS_LOCK_STATUS = "N";

                objProposal.IMS_PROG_REMARKS = string.Empty;
                objProposal.IMS_SHIFT_STATUS = "N";
                objProposal.PTA_SANCTIONED = "N";

                #region FUND_SHARING_RATIO_PMGSY_SCHEME_1

                objProposal.IMS_STATE_SHARE_2015 = ims_sanctioned_projects.IMS_STATE_SHARE_2015;
                objProposal.IMS_MORD_SHARE_2015 = ims_sanctioned_projects.IMS_MORD_SHARE_2015;
                objProposal.IMS_SHARE_PERCENT_2015 = ims_sanctioned_projects.IMS_SHARE_PERCENT_2015;

                #endregion

                return objProposalDAL.SaveRoadProposalDAL(objProposal);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "SaveRoadProposalBAL()");
                return "An Error Occurred While Your Processing Request.";
            }
            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// Update The Road Proposal
        /// </summary>
        /// <param name="ims_sanctioned_projects"></param>
        /// <returns></returns>
        public string UpdateRoadProposalBAL(PMGSY.Models.Proposal.ProposalViewModel ims_sanctioned_projects)
        {
            try
            {
                return objProposalDAL.UpdateRoadProposalDAL(ims_sanctioned_projects);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "UpdateRoadProposalBAL()");
                return "An Error Occurred While Your Processing Request.";
            }
            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// Delete the Road Proposal
        /// </summary>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        /// <returns></returns>
        public string DeleteRoadProposalBAL(int IMS_PR_ROAD_CODE)
        {
            try
            {
                db = new PMGSYEntities();

                // If Stage 2 Proposal, then allow to delete directly
                // First delete all dependent records, then delete Proposal
                if (db.IMS_SANCTIONED_PROJECTS.Where(c => c.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(c => c.IMS_STAGE_PHASE).First() == "S2")
                {
                    //new condition added by Vikram as suggested by Dev Sir on 02 July 2014 . if road is used as stage 1 road then it can not be deleted.
                    if (db.IMS_SANCTIONED_PROJECTS.Any(c => c.IMS_STAGED_ROAD_ID == IMS_PR_ROAD_CODE))
                    {
                        return "Staged construction is present against this Proposal, so Proposal can not be deleted.";
                    }

                    if (db.IMS_BENEFITED_HABS.Where(c => c.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Count() > 0)
                    {
                        db.Database.ExecuteSqlCommand("DELETE [omms].IMS_BENEFITED_HABS Where IMS_PR_ROAD_CODE = {0}", IMS_PR_ROAD_CODE);
                    }

                    if (db.IMS_TRAFFIC_INTENSITY.Where(c => c.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Count() > 0)
                    {
                        db.Database.ExecuteSqlCommand("DELETE [omms].IMS_TRAFFIC_INTENSITY Where IMS_PR_ROAD_CODE = {0}", IMS_PR_ROAD_CODE);
                    }

                    if (db.IMS_CBR_VALUE.Where(c => c.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Count() > 0)
                    {
                        db.Database.ExecuteSqlCommand("DELETE [omms].IMS_CBR_VALUE Where IMS_PR_ROAD_CODE = {0}", IMS_PR_ROAD_CODE);
                    }

                    return objProposalDAL.DeleteRoadProposalDAL(IMS_PR_ROAD_CODE);
                }
                else
                {
                    if (db.IMS_BENEFITED_HABS.Where(c => c.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Count() > 0)
                        return "Habitation Details are added against Proposal, Proposal can not be deleted.";
                    if (db.IMS_TRAFFIC_INTENSITY.Where(c => c.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Count() > 0)
                        return "Traffic Intensity Details are added against Proposal, Proposal can not be deleted.";
                    if (db.IMS_CBR_VALUE.Where(c => c.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Count() > 0)
                        return "CBR Values are added against Proposal, Proposal can not be deleted.";
                    if (db.IMS_SANCTIONED_PROJECTS.Where(c => c.IMS_STAGED_ROAD_ID == IMS_PR_ROAD_CODE && c.IMS_PROPOSAL_TYPE == "L").Count() > 0)
                        return "Bridge proposal is added against selected road Proposal, Proposal can not be deleted.";
                    else
                        return objProposalDAL.DeleteRoadProposalDAL(IMS_PR_ROAD_CODE);
                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "DeleteRoadProposalBAL()");
                return "An Error Occurred While Your Processing Request.";
            }
            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// DPIU Level Finalize Road Proposal
        /// </summary>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        /// <returns></returns>
        public string DPIUFinalizeProposalBAL(int IMS_PR_ROAD_CODE)
        {
            return objProposalDAL.DPIUFinalizeProposalDAL(IMS_PR_ROAD_CODE);
        }

        /// <summary>
        /// Check if Pavement Length is Valid 
        /// </summary>
        /// <param name="IMS_PAV_LEN"></param>
        /// <param name="IMS_CC_LEN"></param>
        /// <param name="IMS_BT_LEN"></param>
        /// <param name="IMS_UPGRADE_CONNECT"></param>
        /// <param name="DUP_IMS_PAV_LENGTH"></param>
        /// <param name="OperationType"></param>
        /// <returns></returns>
        public bool IsPavementLengthValidBAL(decimal IMS_PAV_LEN, decimal IMS_CC_LEN, decimal IMS_BT_LEN, string IMS_UPGRADE_CONNECT, decimal DUP_IMS_PAV_LENGTH, string OperationType, int IMS_STAGE_PHASE, decimal IMS_STAGE1_PAV_LENGTH)
        {
            db = new PMGSYEntities();
            try
            {
                string stateType = db.MASTER_STATE.Where(a => a.MAST_STATE_CODE == PMGSYSession.Current.StateCode).Select(a => a.MAST_STATE_TYPE).First();

                decimal RegularStagePercentageChangeForNew = 1.2M; // 20% ( 100% + 20% ie. 1.2) 
                decimal HillyStagePercentageChangeForNew = 1.3M; // 20%

                decimal RegularStagePercentageChangeForUpgradation = 1.1M; // 20% ( 100% + 20% ie. 1.2) 
                decimal HillyStagePercentageChangeForUpgradation = 1.2M;

                if (IMS_STAGE_PHASE == 2)
                {
                    // For Stage II, Length can be vary upto 10% (plus/minus) of Stage I Length
                    // But should not exceed 30% of Core Network length
                    // Here DUP_IMS_PAV_LENGTH is Core Network Remaining Length
                    // Conditions -     1. Should not Greater than 10% of Stage1 Length        
                    //                  2. Should not Less than 10% of Stage1 Length       
                    //                  3. Should not Greater than 30% of Core Network Length
                    if ((IMS_PAV_LEN > (IMS_STAGE1_PAV_LENGTH + (IMS_STAGE1_PAV_LENGTH * 0.1M))) ||
                            (IMS_PAV_LEN < (IMS_STAGE1_PAV_LENGTH - (IMS_STAGE1_PAV_LENGTH * 0.1M))) ||
                            (IMS_PAV_LEN > (DUP_IMS_PAV_LENGTH + (DUP_IMS_PAV_LENGTH * 0.3M))))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }

                }
                else if (IMS_UPGRADE_CONNECT == "N")
                {
                    if (stateType == "R" || stateType == "I")
                    {
                        if (IMS_PAV_LEN > (DUP_IMS_PAV_LENGTH + (DUP_IMS_PAV_LENGTH * RegularStagePercentageChangeForNew))) // 1.2
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else if (stateType == "H" || stateType == "N" || stateType == "X")
                    {
                        if (IMS_PAV_LEN > (DUP_IMS_PAV_LENGTH + (DUP_IMS_PAV_LENGTH * HillyStagePercentageChangeForNew))) // 1.3
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
                else if (IMS_UPGRADE_CONNECT == "U")
                {
                    if (stateType == "R" || stateType == "I")
                    {
                        if (IMS_PAV_LEN > (DUP_IMS_PAV_LENGTH + (DUP_IMS_PAV_LENGTH * RegularStagePercentageChangeForUpgradation))) // 1.1
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else if (stateType == "H" || stateType == "N" || stateType == "X")
                    {
                        if (IMS_PAV_LEN > (DUP_IMS_PAV_LENGTH + (DUP_IMS_PAV_LENGTH * HillyStagePercentageChangeForUpgradation))) // 1.2
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
                if (IMS_PAV_LEN == (IMS_BT_LEN + IMS_CC_LEN))
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "IsPavementLengthValidBAL()");
                return false;
            }
        }

        /// <summary>
        /// Used When user Edits the Road Proposal 
        /// This checks Wherther Bolck, Core Network Road Can be changed 
        /// </summary>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        /// <returns></returns>
        public string IsProposalEditedBAL(int IMS_PR_ROAD_CODE, bool IMS_TRAFFIC_TYPE)
        {
            try
            {
                db = new PMGSYEntities();

                //new change done by Vikram as if proposal is stage 2 and CBR Details are entered then check whether the new length matches with the cbr length if not prompt user to update the CBR Details
                if (db.IMS_SANCTIONED_PROJECTS.Any(m => m.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE && m.IMS_IS_STAGED == "S" && m.IMS_STAGE_PHASE == "S2"))
                {
                    if (db.IMS_CBR_VALUE.Any(m => m.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE))
                    {
                        decimal? cbrValue = db.IMS_CBR_VALUE.Where(m => m.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Sum(m => m.IMS_CBR_VALUE1);
                        if (cbrValue > db.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(m => m.IMS_PAV_LENGTH).FirstOrDefault())
                        {
                            return "S2";
                        }
                    }

                    if (db.IMS_TRAFFIC_INTENSITY.Where(c => c.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Count() > 0)
                    {
                        return "S2";
                    }
                }

                if (IMS_TRAFFIC_TYPE)
                {
                    if (db.IMS_TRAFFIC_INTENSITY.Where(c => c.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Count() > 0)
                        return "Traffic Intensity Details are added against Proposal, Delete Traffic Intensity Details.";

                    return string.Empty;
                }
                else
                {
                    if (!db.IMS_SANCTIONED_PROJECTS.Any(m => m.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE && m.IMS_STAGE_PHASE == "S2"))
                    {
                        if (db.IMS_BENEFITED_HABS.Where(c => c.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Count() > 0)
                            return "Habitation Details are added against Proposal, Delete the Habitataion Details.";
                    }
                    //commented by Vikram -- suggested by Dev Sir
                    //if (db.IMS_TRAFFIC_INTENSITY.Where(c => c.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Count() > 0)
                    //  return "Traffic Intensity Details are added against Proposal, Delete Traffic Intensity Details.";
                    if (db.IMS_CBR_VALUE.Where(c => c.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Count() > 0)
                        return "CBR Values are added against Proposal, Delete CBR Details.";

                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "IsProposalEditedBAL()");
                return "An Error Occurred While Your Processing Request.";
            }
            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// Check if Habitations are Benifitted Against it 
        /// </summary>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        /// <returns></returns>
        public string IsHabitationsBenifitted(int IMS_PR_ROAD_CODE)
        {
            try
            {
                db = new PMGSYEntities();

                if (db.IMS_BENEFITED_HABS.Where(c => c.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Count() > 0)
                    return "Habitation Details are added against Proposal, Delete the Habitataion Details.";

                return string.Empty;
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "IsHabitationsBenifitted()");
                return "An Error Occurred While Your Processing Request.";
            }
            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// Get the CN Road Details For Stage II Proposal
        /// </summary>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        /// <param name="PLAN_CN_ROAD_CODE"></param>
        /// <returns></returns>
        public string GetRoadDetailsForStageTwoProposalBAL(int IMS_PR_ROAD_CODE, int PLAN_CN_ROAD_CODE)
        {
            return objProposalDAL.GetRoadDetailsForStageTwoProposalDAL(IMS_PR_ROAD_CODE, PLAN_CN_ROAD_CODE);
        }

        /// <summary>
        /// Check if Proposal Can be Finalized or Return message for failure condition
        /// </summary>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        /// <returns></returns>
        public string GetProposalChecksBAL(int IMS_PR_ROAD_CODE, string IMS_LOCK_STATUS)
        {
            ProposalDAL objprDAL = new DAL.Proposal.ProposalDAL();
            bool flg = false;
            try
            {
                db = new PMGSYEntities();

                IMS_SANCTIONED_PROJECTS ims_sanctioned_project = db.IMS_SANCTIONED_PROJECTS.Find(IMS_PR_ROAD_CODE);

                flg = objprDAL.checkMLProposal(IMS_PR_ROAD_CODE);
                if (flg == false)
                {
                    if (ims_sanctioned_project.IMS_ISBENEFITTED_HABS == "Y")
                    {
                        ///Skip habitation mapping for RCPLWE Road change by SAMMED A. PATIL
                        if (!db.IMS_SANCTIONED_PROJECTS.Where(a => a.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE && a.PLAN_ROAD.MAST_PMGSY_SCHEME == 3).Any())
                        {
                            // Check whether habitations are added 
                            if (!db.IMS_BENEFITED_HABS.Where(a => a.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Any())
                            {
                                return "Habitation Details are not added against Proposal, Please Add Habitation Details.";
                            }
                        }
                    }
                }

                if ((ims_sanctioned_project.IMS_IS_STAGED.Equals("S") && ims_sanctioned_project.IMS_STAGE_PHASE.Equals("S2")) || (ims_sanctioned_project.IMS_IS_STAGED.Equals("C")))
                {
                    // Check for CBR Details Entered 
                    if (!db.IMS_CBR_VALUE.Where(c => c.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Any())
                    {
                        return "CBR Value Details are not added against Proposal, Please add CBR Value Details";
                    }

                    if (ims_sanctioned_project.IMS_TRAFFIC_TYPE != null)
                    {
                        // check for Traffic Intensity 
                        if (!db.IMS_TRAFFIC_INTENSITY.Where(c => c.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Any())
                        {
                            return "Traffice Intensity Details are not added against Proposal,\nPlease Add Traffic Intensity Details of Last One Year from Propsal Year.";
                        }
                    }
                }


                /// Check in case of Reconsider and Entry of Proposal

                // In Case of Mord Unlock Proposal

                //if ((ims_sanctioned_project.IMS_ISCOMPLETED == "E" || ims_sanctioned_project.IMS_SANCTIONED == "R") && ims_sanctioned_project.IMS_LOCK_STATUS == "N")
                if (flg == false)
                {
                    ///Skip habitation mapping for RCPLWE Road change by SAMMED A. PATIL
                    if (!db.IMS_SANCTIONED_PROJECTS.Where(a => a.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE && a.PLAN_ROAD.MAST_PMGSY_SCHEME == 3).Any())
                    {
                        if (ims_sanctioned_project.IMS_ISCOMPLETED == "E" || ims_sanctioned_project.IMS_SANCTIONED == "R")
                        {
                            if (ims_sanctioned_project.IMS_ISBENEFITTED_HABS == "Y")
                            {
                                /// Check whether habitations are Finalized                     
                                if (ims_sanctioned_project.IMS_ISCOMPLETED != "H" && ims_sanctioned_project.IMS_IS_STAGED == "C")
                                {
                                    return "Habitations are not Finalized, Please Finalize Habitation to Finalize Proposal.";
                                }
                                /// For Stage II Proposals Skip the Habitation Finalization Condition, Check Only For Stage I
                                else if (ims_sanctioned_project.IMS_ISCOMPLETED != "H" && ims_sanctioned_project.IMS_IS_STAGED == "S" && ims_sanctioned_project.IMS_STAGE_PHASE != "S2")
                                {
                                    return "Habitations are not Finalized, Please Finalize Habitation to Finalize Proposal.";
                                }
                            }
                        }
                    }
                }

                /// Incase of Unlocked Proposal Apply Habitations Validations

                // In Case of Mord Unlock Proposal
                //else if ( ims_sanctioned_project.IMS_SANCTIONED == "Y" && ims_sanctioned_project.IMS_LOCK_STATUS == "N")
                else if (IMS_LOCK_STATUS == "M")
                {
                    HabitationViewModel habitationViewModel = new HabitationViewModel();
                    habitationViewModel.IMS_PR_ROAD_CODE = ims_sanctioned_project.IMS_PR_ROAD_CODE;
                    habitationViewModel.MAST_STATE_TYPE = (from c in db.MASTER_STATE where c.MAST_STATE_CODE == ims_sanctioned_project.MAST_STATE_CODE select c.MAST_STATE_TYPE).First().ToUpper();
                    habitationViewModel.MAST_IAP_DISTRICT = (from c in db.MASTER_DISTRICT where c.MAST_DISTRICT_CODE == ims_sanctioned_project.MAST_DISTRICT_CODE select c.MAST_IAP_DISTRICT).First().ToUpper();
                    habitationViewModel.MAST_IS_TRIBAL = (from c in db.MASTER_BLOCK where c.MAST_BLOCK_CODE == ims_sanctioned_project.MAST_BLOCK_CODE select c.MAST_IS_TRIBAL).First().ToUpper();
                    string status = CheckHabitationsForUnlock(habitationViewModel);
                    if (status != string.Empty)
                    {
                        return status;
                    }
                }

                if ((ims_sanctioned_project.IMS_IS_STAGED.Equals("S") && ims_sanctioned_project.IMS_STAGE_PHASE.Equals("S2")) || (ims_sanctioned_project.IMS_IS_STAGED.Equals("C")))
                {
                    if (ims_sanctioned_project.IMS_TRAFFIC_TYPE != null)
                    {
                        // Check for Traffic Intensity Details for Last Two Years are entered.
                        int IMS_YEAR = db.IMS_SANCTIONED_PROJECTS.Where(c => c.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(c => c.IMS_YEAR).First();

                        if (!db.IMS_TRAFFIC_INTENSITY.Where(c => c.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE && c.IMS_TI_YEAR == (IMS_YEAR - 1)).Any())
                        {
                            return "Traffic Intensity Details of the Year " + (IMS_YEAR - 1) + "-" + IMS_YEAR + " are not Added. \n Add Traffic Intensity Details to Finalize Proposal.";
                        }
                        //else if (!db.IMS_TRAFFIC_INTENSITY.Where(c => c.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE && c.IMS_TI_YEAR == (IMS_YEAR - 2)).Any())
                        //{
                        //    return "Traffic Intensity Details of the Year " + (IMS_YEAR - 2) + "-" + (IMS_YEAR - 1) + " are not Added. \n Add Traffic Intensity Details to Finalize Proposal.";
                        //}
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
                }

                ///PMGSY3
                if (PMGSYSession.Current.PMGSYScheme == 4)
                {
                    //Temp check for RJ to restrict finalisation of Proposals
                    //if (PMGSYSession.Current.StateCode == 29 && PMGSYSession.Current.DistrictCode != 556)
                    //{
                    //    return "Proposal cannot be finalised, Please contact NRIDA.";
                    //}
                    //if (!db.IMS_PROPOSAL_FILES.Where(z => z.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE && z.ISPF_UPLOAD_BY == "D" && (z.ISPF_TYPE == "C" || z.ISPF_TYPE == "I")).Any())
                    //{
                    //    return "Either C Proforma or Image not uploaded against the Proposal.";
                    //}
                    if (!db.IMS_PROPOSAL_FILES.Where(z => z.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE && z.ISPF_UPLOAD_BY == "D" && z.ISPF_TYPE == "C").Any())
                    {
                        return "C Proforma not uploaded against the Proposal.";
                    }
                    if (!db.IMS_PROPOSAL_FILES.Where(z => z.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE && z.ISPF_UPLOAD_BY == "D" && z.ISPF_TYPE == "I").Any())
                    {
                        return "Image not uploaded against the Proposal.";
                    }
                    if (db.IMS_PROPOSAL_FILES.Where(z => z.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE && z.ISPF_UPLOAD_BY == "D" && z.ISPF_TYPE == "I").Count() <10 )
                    {
                        return "Please upload at least 10 images to finalize the Proposal.";
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "GetProposalChecksBAL()");
                return "An Error Occurred While Your Processing Request.";
            }
            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// Checks if Proposal Can be Deleted
        /// </summary>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        /// <returns></returns>
        public string IsProposalDeletedBAL(int IMS_PR_ROAD_CODE)
        {
            try
            {
                db = new PMGSYEntities();

                // If Stage 2 Proposal, then allow to delete directly, else check for dependent records.
                if (db.IMS_SANCTIONED_PROJECTS.Where(c => c.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(c => c.IMS_STAGE_PHASE).First() != "S2")
                {
                    // check for habitation details
                    if (db.IMS_BENEFITED_HABS.Where(a => a.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Any())
                    {
                        return "Habitation Details are added against Proposal, Please Delete Habitation Details.";
                    }

                    // Check for CBR Details Entered 
                    if (db.IMS_CBR_VALUE.Where(c => c.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Any())
                    {
                        return "CBR Value Details are added against Proposal, Please Delete CBR Value Details.";
                    }

                    // check for Traffic Intensity 
                    if (db.IMS_TRAFFIC_INTENSITY.Where(c => c.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Any())
                    {
                        return "Traffic Intensity Details are added against Proposal,\nPlease Delete Traffic Intensity Details.";
                    }
                }

                // check if any file is uploaded against this proposal 
                if (db.IMS_PROPOSAL_FILES.Where(c => c.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Any())
                {
                    return "Files are Uploaded against Proposal,\nPlease Delete Files to Delete Proposal.";
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "IsProposalDeletedBAL()");
                return "An Error Occurred While Your Processing Request.";
            }
            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// Only Used When Editing the Proposal
        /// </summary>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        /// <param name="PLAN_CN_ROAD_CODE"></param>
        /// <returns></returns>
        public string GetRoadDetailsForUpdate(int IMS_PR_ROAD_CODE, int PLAN_CN_ROAD_CODE)
        {
            return objProposalDAL.GetRoadDetailsForUpdateDAL(IMS_PR_ROAD_CODE, PLAN_CN_ROAD_CODE);
        }

        /// <summary>
        /// This Function checks if the Proposal Length type i.e. partial length/ Full length is Correct.
        /// </summary>
        /// <param name="PLAN_CN_ROAD_CODE"></param>
        /// <param name="IMS_PAV_LEN"></param>
        /// <returns></returns>
        public string isProposalLengthTypeValid(int PLAN_CN_ROAD_CODE, decimal IMS_PAV_LEN, int IMS_PR_ROAD_CODE, string ProposalLengthType, string IMS_STAGE_PHASE)
        {
            try
            {


                db = new PMGSYEntities();

                #region  Validation On Saving New Proposal / On Updating Existing Proposal

                decimal? ActualRoadLength = 0;

                if (PMGSYSession.Current.PMGSYScheme == 1)
                {

                    ActualRoadLength = db.PLAN_ROAD.Where(a => a.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE).Select(a => a.PLAN_RD_LENGTH).First();
                }
                else
                {
                    ActualRoadLength = db.PLAN_ROAD.Where(a => a.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE).Select(a => a.PLAN_RD_TOTAL_LEN).First();
                }

                if (ProposalLengthType == "F")
                {// Full Length
                    decimal? ModifiedCNLength = ActualRoadLength + (Convert.ToDecimal(ActualRoadLength) * 0.5M);

                    if (IMS_PAV_LEN >= ModifiedCNLength)
                    {
                        return "Proposed Length can not be more than 50 Percent of Core Network Length.";
                    }
                }
                else
                { // Partial Length

                    if (IMS_PAV_LEN <= ActualRoadLength)
                    {
                        // Continue further

                    }
                    else
                    {
                        return "Proposed Length can not be more than Core Network Length.";
                    }

                }
                #endregion

                /// For Stage II Skip the validation
                if (IMS_STAGE_PHASE == "S2" || IMS_STAGE_PHASE == "2")
                {
                    return string.Empty;
                }

                /// New Proposal
                if (IMS_PR_ROAD_CODE == 0)
                {


                    // Full Length Proposal
                    if (ProposalLengthType == "F")
                    {



                        /// Check if Proposal is Made on that road 
                        if (db.IMS_SANCTIONED_PROJECTS.Where(a => a.MAST_STATE_CODE == PMGSY.Extensions.PMGSYSession.Current.StateCode
                                                            && a.MAST_DISTRICT_CODE == PMGSY.Extensions.PMGSYSession.Current.DistrictCode
                                                            && a.IMS_PROPOSAL_TYPE == "P"
                                                            && a.IMS_SANCTIONED != "D"
                                                            && a.IMS_YEAR > 1950 // condition added by Vikram suggested by Dev Sir
                                                            && a.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE).Any())
                        {
                            return "Proposal Already Made on the Road, Please Select Proposal Length as a Partial";
                        }
                        else
                        {
                            if (IMS_PAV_LEN >= ActualRoadLength)
                            {
                                return string.Empty;
                            }
                            else
                            {
                                return "Pavement Length is Less than Road Length, Please Select Proposal Length as a Partial";
                            }
                        }
                    }
                    /// Partial Length Proposal
                    else
                    {
                        /// Check if Proposal is Made on that road 
                        if (db.IMS_SANCTIONED_PROJECTS.Where(a => a.MAST_STATE_CODE == PMGSY.Extensions.PMGSYSession.Current.StateCode
                                                            && a.MAST_DISTRICT_CODE == PMGSY.Extensions.PMGSYSession.Current.DistrictCode
                                                            && a.IMS_PROPOSAL_TYPE == "P"
                                                            && a.IMS_SANCTIONED != "D"
                                                            && a.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE).Any())
                        {

                            if (IMS_PAV_LEN == ActualRoadLength)
                            {
                                return "Pavement Length and CN Length are Same, Please Select Proposal Length as a Full";
                            }
                            else
                            {
                                return string.Empty;
                            }
                        }
                        else
                        {
                            if (IMS_PAV_LEN <= ActualRoadLength)
                            {
                                return string.Empty;
                            }
                            else
                            {
                                return "Pavement Length is Less Than CN Length, Please Select Proposal Length as a Partial";
                            }

                            if (IMS_PAV_LEN >= ActualRoadLength)
                            {
                                return string.Empty;
                            }
                            else
                            {
                                return "Pavement Length is Less Than CN Length, Please Select Proposal Length as a Excess";
                            }
                        }
                    }
                }
                /// Incase of Updatation
                else if (IMS_PR_ROAD_CODE != 0)
                {






                    ///Skip validation for Proposals before 2012 16AUG2019
                    if (db.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(c => c.IMS_YEAR).FirstOrDefault() < 2012)
                    {
                        return string.Empty;
                    }
                    // Full Length Proposal
                    if (ProposalLengthType == "F")
                    {
                        /// Check if Proposal is Made on that road 
                        if (db.IMS_SANCTIONED_PROJECTS.Where(a => a.MAST_STATE_CODE == PMGSY.Extensions.PMGSYSession.Current.StateCode
                                                            && a.MAST_DISTRICT_CODE == PMGSY.Extensions.PMGSYSession.Current.DistrictCode
                                                            && a.IMS_PR_ROAD_CODE != IMS_PR_ROAD_CODE
                                                            && a.IMS_PROPOSAL_TYPE == "P"
                                                            && a.IMS_SANCTIONED != "D"
                                                            && a.IMS_YEAR > 1950 // condition added by Vikram suggested by Dev Sir
                                                            && a.PLAN_CN_ROAD_CODE == PLAN_CN_ROAD_CODE).Any())
                        {
                            return "Proposal Already Made on the Road, Please Select Proposal Length as a Partial";
                        }
                        else
                        {
                            // Full Length Proposal and Length is equal to Actual Length of Road
                            if (IMS_PAV_LEN >= ActualRoadLength)
                            {
                                return string.Empty;
                            }
                            else
                            {
                                return "Pavement Length is Less than Road Length, Please Select Proposal Length as a Partial";
                            }
                        }
                    }
                    /// Partial Length
                    else
                    {
                        if (IMS_PAV_LEN > ActualRoadLength)
                        {
                            return "Pavement Length is greater than CN Length, Please Select Proposal Length as a Full/Excess Length";
                        }
                        else
                        {
                            return string.Empty;
                        }
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "isProposalLengthTypeValid()");
                return "Error Occured while getting Road Length";
            }
            finally
            {
                db.Dispose();
            }
        }

        #endregion

        #region Habitataion and Cluster

        /// <summary>
        /// List the Habitations
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        /// <returns></returns>
        public Array GetHabitationListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int IMS_PR_ROAD_CODE)
        {
            return objProposalDAL.GetHabitationListDAL(page, rows, sidx, sord, out totalRecords, IMS_PR_ROAD_CODE);
        }

        /// <summary>
        /// Method to Create the Cluster
        /// </summary>
        /// <param name="clusterArray"></param>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        /// <returns></returns>
        public int CreateClusterBAL(string[] clusterArray, int IMS_PR_ROAD_CODE)
        {

            return objProposalDAL.CreateClusterDAL(clusterArray, IMS_PR_ROAD_CODE);
        }

        /// <summary>
        /// Update the Cluster
        /// </summary>
        /// <param name="HabitationArray"></param>
        /// <param name="ClusterArray"></param>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        /// <returns></returns>
        public string UpdateClusterBAL(string[] HabitationArray, string[] ClusterArray, int IMS_PR_ROAD_CODE)
        {
            int[] Clusters = Array.ConvertAll(ClusterArray, s => int.Parse(s));
            int[] Habitations = Array.ConvertAll(HabitationArray, s => int.Parse(s));
            return objProposalDAL.UpdateClusterDAL(Habitations, Clusters, IMS_PR_ROAD_CODE);
        }

        /// <summary>
        /// UnMap the Habitation
        /// </summary>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        /// <param name="IMS_HAB_CODE"></param>
        /// <returns></returns>
        public int UnMapHabitationBAL(string IMS_PR_ROAD_CODE, string IMS_HAB_CODE)
        {
            return objProposalDAL.UnMapHabitationDAL(Convert.ToInt32(IMS_PR_ROAD_CODE), Convert.ToInt32(IMS_HAB_CODE));
        }

        /// <summary>
        /// UnMap the Habitation Cluster
        /// </summary>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        /// <param name="IMS_HAB_CODE"></param>
        /// <returns></returns>
        public int UnMapHabitationClusterBAL(string IMS_PR_ROAD_CODE, string IMS_HAB_CODE, string IMS_CLUSTER_CODE)
        {
            return objProposalDAL.UnMapHabitationClusterDAL(Convert.ToInt32(IMS_PR_ROAD_CODE), Convert.ToInt32(IMS_HAB_CODE), Convert.ToInt32(IMS_CLUSTER_CODE));
        }

        /// <summary>
        /// Map the habitation Details to Road Proposal
        /// </summary>
        /// <param name="habModel"></param>
        /// <returns></returns>
        public string AddHabitationDetailBAL(HabitationViewModel habModel)
        {
            return objProposalDAL.AddHabitationDetailsDAL(habModel);
        }

        /// <summary>
        /// Finalize the Habitation Details
        /// </summary>
        /// <param name="habitationViewModel"></param>
        /// <returns></returns>
        public string FinalizeHabitationBAL(HabitationViewModel habitationViewModel)
        {

            int requiredPopulation = 500;

            if (habitationViewModel.MAST_STATE_TYPE.ToUpper() == "N" || habitationViewModel.MAST_STATE_TYPE.ToUpper() == "H" || habitationViewModel.MAST_STATE_TYPE.ToUpper() == "X" || habitationViewModel.MAST_STATE_TYPE.ToUpper() == "D" ||
                habitationViewModel.MAST_IAP_DISTRICT == "Y" ||
                habitationViewModel.MAST_BLOCK_SCHEDULE5.ToUpper() == "Y" || habitationViewModel.MAST_IAP_BLOCK.ToUpper() == "Y" || habitationViewModel.MAST_BLOCK_IS_DESERT.ToUpper() == "Y" || habitationViewModel.MAST_IS_TRIBAL == "Y") // IAP District condition added by Vikram suggested by Dev Sir on 18 June 2014  and IS_TRIBAL is added as suggested by Srinivas sir on 03 Feb 2015
            {
                requiredPopulation = 250;
            }

            if (habitationViewModel.MAST_IAP_BLOCK == "Y") // For Deep IAP Block
            {
                requiredPopulation = 100;
            }

            

            /// Condition added by shyam on 04092015, as per Pankaj Kumar's direction
            /// If Habitation is GP or Tourist Place, then population should be greater than 0
            var habDetails = (
                                from c in db.IMS_BENEFITED_HABS
                                join d in db.MASTER_HABITATIONS_DETAILS
                                    on c.MAST_HAB_CODE equals d.MAST_HAB_CODE
                                where c.IMS_PR_ROAD_CODE == habitationViewModel.IMS_PR_ROAD_CODE
                                select d
                            ).ToList();

            foreach (var item in habDetails)
            {
                if (db.MASTER_HABITATIONS_DETAILS.Where(c => c.MAST_HAB_CODE == item.MAST_HAB_CODE && (c.MAST_PANCHAYAT_HQ.Equals("Y") || c.MAST_TOURIST_PLACE.Equals("Y"))).Any())
                {
                    requiredPopulation = 1;
                }
            }
            /////////////////////////////////////////////////////////////////////////////

            var ClusterWisePopulationList = (
                                                from c in db.IMS_BENEFITED_HABS
                                                join d in db.MASTER_HABITATIONS_DETAILS
                                                    on c.MAST_HAB_CODE equals d.MAST_HAB_CODE
                                                where c.IMS_PR_ROAD_CODE == habitationViewModel.IMS_PR_ROAD_CODE
                                                group new { c, d } by c.MAST_CLUSTER_CODE into ClusterGroup
                                                select new
                                                {
                                                    cluster = ClusterGroup.Key == null ? 0 : ClusterGroup.Key,
                                                    population = ClusterGroup.Sum(p => p.d.MAST_HAB_TOT_POP),
                                                    name = ClusterGroup.Sum(p => p.d.MAST_HAB_CODE),
                                                    code = ClusterGroup.Select(p => p.d.MAST_HAB_CODE).FirstOrDefault() //added by shyam on 28012015
                                                }
                                            );

            // Condition added by shyam on 28012015, as per srinivas sir's direction for schedule5 Habitation
            // If One of the habitation in cluster is Schedule5 then condtion of population will be 250
            foreach (var data in ClusterWisePopulationList)
            {
                if (db.MASTER_HABITATIONS.Where(c => c.MAST_HAB_CODE == data.code && c.MAST_SCHEDULE5.Equals("Y")).Any())
                {
                    requiredPopulation = 250;
                }
                else if (db.MASTER_HABITATIONS_DETAILS.Where(c => c.MAST_HAB_CODE == data.code && (c.MAST_PANCHAYAT_HQ.Equals("Y") || c.MAST_TOURIST_PLACE.Equals("Y"))).Any())
                {
                    requiredPopulation = 1; /// Condition added by shyam on 04092015, as per Pankaj Kumar's direction
                    /// If Habitation is GP or Tourist Place, then population should be greater than 0
                }

                if (habitationViewModel.MAST_IAP_BLOCK == "Y") // For Deep IAP Block
                {
                    requiredPopulation = 100;
                }
            }

            if (PMGSYSession.Current.PMGSYScheme == 5)
                requiredPopulation = 1;


            foreach (var data in ClusterWisePopulationList)
            {
                if (data.cluster != 0 && data.population < requiredPopulation)
                {
                    return "Population of Cluster " + data.cluster + " is less than " + requiredPopulation + " .\nPopulation of a Cluster should not be Less than " + requiredPopulation;
                }
                else if (data.cluster == 0 && data.population < requiredPopulation)
                {
                    return "Population of Habitation " + " must be Greator than " + requiredPopulation + ". if it is not in a Cluster, \nInclude Habitataion in cluster to finalize.";
                }
            }

            return objProposalDAL.FinalizeHabitationDAL(habitationViewModel);
        }

        /// <summary>
        /// Map the habitation Details to Road Proposal
        /// </summary>
        /// <param name="habModel"></param>
        /// <returns></returns>
        public string AddHabitationClusterDetailBAL(HabitationClusterViewModel habModel)
        {
            return objProposalDAL.AddHabitationClusterDetailDAL(habModel);
        }

        #endregion

        #region Traffic Intensity

        /// <summary>
        /// Populate the List of Traffic Intensity List 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        /// <returns></returns>
        public Array GetTrafficListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int IMS_PR_ROAD_CODE)
        {
            return objProposalDAL.GetTrafficListDAL(page, rows, sidx, sord, out totalRecords, IMS_PR_ROAD_CODE);
        }

        /// <summary>
        /// Saves the Data of Traffic Intensity
        /// </summary>
        /// <param name="traffic_intensity"></param>
        /// <returns></returns>
        public int SaveTrafficIntesityBAL(TrafficViewModel traffic_intensity)
        {
            try
            {
                db = new PMGSYEntities();
                IMS_TRAFFIC_INTENSITY ims_traffic_intensity = new IMS_TRAFFIC_INTENSITY();
                ims_traffic_intensity.IMS_PR_ROAD_CODE = traffic_intensity.IMS_PR_ROAD_CODE;
                ims_traffic_intensity.IMS_COMM_TI = Convert.ToInt32(traffic_intensity.IMS_COMM_TI);
                ims_traffic_intensity.IMS_TI_YEAR = traffic_intensity.IMS_TI_YEAR;
                ims_traffic_intensity.IMS_TOTAL_TI = Convert.ToInt32(traffic_intensity.IMS_TOTAL_TI);

                return objProposalDAL.SaveTrafficIntesityDAL(ims_traffic_intensity);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "SaveTrafficIntesityBAL()");
                return -1;
            }
            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        ///  Update the Traffic Intensity Details
        /// </summary>
        /// <param name="traffic_intensity"></param>
        /// <returns></returns>
        public int UpdateTrafficIntesityBAL(TrafficViewModel traffic_intensity)
        {
            try
            {
                db = new PMGSYEntities();
                IMS_TRAFFIC_INTENSITY ims_traffic_intensity = new IMS_TRAFFIC_INTENSITY();
                ims_traffic_intensity.IMS_PR_ROAD_CODE = traffic_intensity.IMS_PR_ROAD_CODE;
                ims_traffic_intensity.IMS_COMM_TI = Convert.ToInt32(traffic_intensity.IMS_COMM_TI);
                ims_traffic_intensity.IMS_TI_YEAR = traffic_intensity.IMS_TI_YEAR;
                ims_traffic_intensity.IMS_TOTAL_TI = Convert.ToInt32(traffic_intensity.IMS_TOTAL_TI);

                return objProposalDAL.UpdateTrafficIntesityDAL(ims_traffic_intensity);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "UpdateTrafficIntesityBAL()");
                return -1;
            }
            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// Delete the Traffic Intensity Details
        /// </summary>
        /// <param name="traffic_intensity"></param>
        /// <returns></returns>
        public int DeleteTrafficIntensityDetailsBAL(TrafficViewModel traffic_intensity)
        {
            try
            {
                db = new PMGSYEntities();
                IMS_TRAFFIC_INTENSITY ims_traffic_intensity = new IMS_TRAFFIC_INTENSITY();
                ims_traffic_intensity.IMS_PR_ROAD_CODE = traffic_intensity.IMS_PR_ROAD_CODE;
                ims_traffic_intensity.IMS_COMM_TI = Convert.ToInt32(traffic_intensity.IMS_COMM_TI);
                ims_traffic_intensity.IMS_TI_YEAR = traffic_intensity.IMS_TI_YEAR;
                ims_traffic_intensity.IMS_TOTAL_TI = Convert.ToInt32(traffic_intensity.IMS_TOTAL_TI);

                return objProposalDAL.DeleteTrafficIntensityDetailsDAL(ims_traffic_intensity);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "DeleteTrafficIntensityDetailsBAL()");
                return -1;
            }
            finally
            {
                db.Dispose();
            }
        }

        #endregion

        #region CBR Details

        /// <summary>
        ///  Save the CBR Value
        /// </summary>
        /// <param name="CBRModel"></param>
        /// <returns></returns>
        public string SaveCBRValueBAL(CBRViewModel CBRModel)
        {
            try
            {
                db = new PMGSYEntities();
                IMS_CBR_VALUE ims_cbr_value = new IMS_CBR_VALUE();
                Int32 MaxID = 0;
                if (db.IMS_CBR_VALUE.Where(c => c.IMS_PR_ROAD_CODE == CBRModel.IMS_PR_ROAD_CODE).Any())
                {
                    MaxID = (from c in db.IMS_CBR_VALUE where c.IMS_PR_ROAD_CODE == CBRModel.IMS_PR_ROAD_CODE select c.IMS_SEGMENT_NO).Max();
                }
                ++MaxID;
                ims_cbr_value.IMS_PR_ROAD_CODE = CBRModel.IMS_PR_ROAD_CODE;
                ims_cbr_value.IMS_SEGMENT_NO = Convert.ToInt32(MaxID);
                ims_cbr_value.IMS_STR_CHAIN = Convert.ToDecimal(CBRModel.IMS_STR_CHAIN);
                ims_cbr_value.IMS_END_CHAIN = Convert.ToDecimal(CBRModel.IMS_END_CHAIN);
                ims_cbr_value.IMS_CBR_VALUE1 = Convert.ToDecimal(CBRModel.IMS_CBR_VALUE1);

                if (db.IMS_CBR_VALUE.Where(c => c.IMS_PR_ROAD_CODE == CBRModel.IMS_PR_ROAD_CODE).Any())
                    CBRModel.CBRLenghEntered = db.IMS_CBR_VALUE.Where(c => c.IMS_PR_ROAD_CODE == CBRModel.IMS_PR_ROAD_CODE).AsEnumerable().Sum(c => c.IMS_END_CHAIN - c.IMS_STR_CHAIN);
                CBRModel.Remaining_Length = CBRModel.IMS_PAV_LENGTH - CBRModel.CBRLenghEntered;

                if (CBRModel.IMS_STR_CHAIN > CBRModel.IMS_END_CHAIN)
                {
                    return "Start Chainage must be less than End Chainage";
                }

                if ((CBRModel.Remaining_Length < (CBRModel.IMS_END_CHAIN - CBRModel.IMS_STR_CHAIN)))
                {
                    return "Remaining Length Exceeds than Segment Length. \nPlease Recheck Start,End Chainage";
                }

                return objProposalDAL.SaveCBRValueDAL(ims_cbr_value);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "SaveCBRValueBAL()");
                return "An Error Occurred While Your Processing Request.";
            }
            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// Update the CBR Details
        /// </summary>
        /// <param name="CBRModel"></param>
        /// <returns></returns>
        public string UpdateCBRValueBAL(CBRViewModel CBRModel)
        {
            try
            {
                db = new PMGSYEntities();
                IMS_CBR_VALUE ims_cbr_value = new IMS_CBR_VALUE();

                ims_cbr_value.IMS_PR_ROAD_CODE = CBRModel.IMS_PR_ROAD_CODE;
                ims_cbr_value.IMS_SEGMENT_NO = CBRModel.IMS_SEGMENT_NO;
                ims_cbr_value.IMS_STR_CHAIN = Convert.ToDecimal(CBRModel.IMS_STR_CHAIN);
                ims_cbr_value.IMS_END_CHAIN = Convert.ToDecimal(CBRModel.IMS_END_CHAIN);
                ims_cbr_value.IMS_CBR_VALUE1 = Convert.ToDecimal(CBRModel.IMS_CBR_VALUE1);

                if (db.IMS_CBR_VALUE.Where(c => c.IMS_PR_ROAD_CODE == CBRModel.IMS_PR_ROAD_CODE).Any())
                    CBRModel.CBRLenghEntered = db.IMS_CBR_VALUE.Where(c => c.IMS_PR_ROAD_CODE == CBRModel.IMS_PR_ROAD_CODE && c.IMS_SEGMENT_NO != CBRModel.IMS_SEGMENT_NO).AsEnumerable().Sum(c => c.IMS_END_CHAIN - c.IMS_STR_CHAIN);
                CBRModel.Remaining_Length = CBRModel.IMS_PAV_LENGTH - CBRModel.CBRLenghEntered;

                if (CBRModel.IMS_STR_CHAIN > CBRModel.IMS_END_CHAIN)
                {
                    return "Start Chainage must be less than End Chainage";
                }

                if ((CBRModel.Remaining_Length < (CBRModel.IMS_END_CHAIN - CBRModel.IMS_STR_CHAIN)))
                {
                    return "Segment length exceeds than the remaining length. \nPlease Recheck Start,End Chainage";
                }
                return objProposalDAL.UpdateCBRVAlueDAL(ims_cbr_value);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "UpdateCBRValueBAL()");
                return "An Error Occurred While Your Processing Request.";
            }
            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        ///  Delete the CBR Details
        /// </summary>
        /// <param name="CBRModel"></param>
        /// <returns></returns>
        public string DeleteCBRValueBAL(CBRViewModel CBRModel)
        {
            try
            {
                db = new PMGSYEntities();
                IMS_CBR_VALUE ims_cbr_value = new IMS_CBR_VALUE();

                ims_cbr_value.IMS_PR_ROAD_CODE = CBRModel.IMS_PR_ROAD_CODE;
                ims_cbr_value.IMS_SEGMENT_NO = CBRModel.IMS_SEGMENT_NO;
                ims_cbr_value.IMS_STR_CHAIN = Convert.ToDecimal(CBRModel.IMS_STR_CHAIN);
                ims_cbr_value.IMS_END_CHAIN = Convert.ToDecimal(CBRModel.IMS_END_CHAIN);
                ims_cbr_value.IMS_CBR_VALUE1 = Convert.ToDecimal(CBRModel.IMS_CBR_VALUE1);

                return objProposalDAL.DeleteCBRValueDAL(ims_cbr_value);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "DeleteCBRValueBAL()");
                return "An Error Occurred While Your Processing Request.";
            }
            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// Save the CBR Value and Get the Remaining Length
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        /// <returns></returns>
        public Array GetCBRListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int IMS_PR_ROAD_CODE)
        {
            return objProposalDAL.GetCBRListDAL(page, rows, sidx, sord, out totalRecords, IMS_PR_ROAD_CODE);
        }

        #endregion

        #region Upload File Details

        /// <summary>
        ///  Lists the Files
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        /// <returns></returns>
        public Array GetFilesListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int IMS_PR_ROAD_CODE)
        {
            return objProposalDAL.GetFilesListDAL(page, rows, sidx, sord, out totalRecords, IMS_PR_ROAD_CODE);
        }

        /// <summary>
        /// Get the PDF Files List
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        /// <returns></returns>
        public Array GetPDFFilesListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int IMS_PR_ROAD_CODE)
        {
            return objProposalDAL.GetPDFFilesListDAL(page, rows, sidx, sord, out totalRecords, IMS_PR_ROAD_CODE);
        }

        /// <summary>
        /// Get the PDF Files List
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        /// <returns></returns>
        public Array GetJIFilesListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int IMS_PR_ROAD_CODE)
        {
            return objProposalDAL.GetJIFilesListDAL(page, rows, sidx, sord, out totalRecords, IMS_PR_ROAD_CODE);
        }

        /// <summary>
        /// Get the PDF Files List
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        /// <returns></returns>
        public Array GetSTASRRDAPDFFilesListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int IMS_PR_ROAD_CODE)
        {
            return objProposalDAL.GetSTASRRDAPDFFilesListDAL(page, rows, sidx, sord, out totalRecords, IMS_PR_ROAD_CODE);
        }

        /// <summary>
        /// Add File Upload Details
        /// </summary>
        /// <param name="lstFileUploadViewModel"></param>
        /// <param name="ISPF_TYPE"></param>
        /// <returns></returns>
        public string AddFileUploadDetailsBAL(List<FileUploadViewModel> lstFileUploadViewModel, string ISPF_TYPE)
        {
            List<IMS_PROPOSAL_FILES> lst_ims_proposal_files = new List<IMS_PROPOSAL_FILES>();

            // Image Upload
            if (ISPF_TYPE.ToUpper() == "I")
            {

                foreach (FileUploadViewModel model in lstFileUploadViewModel)
                {
                    lst_ims_proposal_files.Add(
                        new IMS_PROPOSAL_FILES()
                        {
                            IMS_PR_ROAD_CODE = model.IMS_PR_ROAD_CODE,
                            SPF_UPLOAD_DATE = DateTime.Now,
                            IMS_FILE_NAME = model.name,
                            CHAINAGE = model.chainage,
                            ISPF_FILE_REMARK = model.Image_Description,
                            ISPF_IS_ACTIVE = "Y",
                            ISPF_TYPE = "I",
                            ISPF_UPLOAD_BY = (PMGSYSession.Current.RoleCode == 3 ? "T" : (PMGSYSession.Current.RoleCode == 2 || PMGSYSession.Current.RoleCode == 55) ? "S" : "D")
                        }
                   );
                }

            }
            // C Proforma PDF File Upload
            else if (ISPF_TYPE.ToUpper() == "C")
            {

                foreach (FileUploadViewModel model in lstFileUploadViewModel)
                {
                    lst_ims_proposal_files.Add(
                        new IMS_PROPOSAL_FILES()
                        {
                            IMS_PR_ROAD_CODE = model.IMS_PR_ROAD_CODE,
                            SPF_UPLOAD_DATE = DateTime.Now,
                            IMS_FILE_NAME = model.name,
                            CHAINAGE = null,
                            ISPF_FILE_REMARK = model.PdfDescription,
                            ISPF_IS_ACTIVE = "Y",
                            ISPF_TYPE = (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38 || PMGSYSession.Current.RoleCode == 54) ? "C" : "P",
                            ISPF_UPLOAD_BY = (PMGSYSession.Current.RoleCode == 3 ? "T" : (PMGSYSession.Current.RoleCode == 2 || PMGSYSession.Current.RoleCode == 55) ? "S" : PMGSYSession.Current.RoleCode == 15 ? "P" : "D")
                        }
                   );
                }
            }
            // JI PDF File Upload
            else if (ISPF_TYPE.ToUpper() == "J")
            {

                foreach (FileUploadViewModel model in lstFileUploadViewModel)
                {
                    lst_ims_proposal_files.Add(
                        new IMS_PROPOSAL_FILES()
                        {
                            IMS_PR_ROAD_CODE = model.IMS_PR_ROAD_CODE,
                            SPF_UPLOAD_DATE = DateTime.Now,
                            IMS_FILE_NAME = model.name,
                            CHAINAGE = null,
                            ISPF_FILE_REMARK = model.PdfDescription,
                            ISPF_IS_ACTIVE = "Y",
                            ISPF_TYPE = "J",//(PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38) ? "C" : "P",
                            ISPF_UPLOAD_BY = (PMGSYSession.Current.RoleCode == 3 ? "T" : (PMGSYSession.Current.RoleCode == 2 || PMGSYSession.Current.RoleCode == 55) ? "S" : PMGSYSession.Current.RoleCode == 15 ? "P" : "D")
                        }
                   );
                }
            }
            return objProposalDAL.AddFileUploadDetailsDAL(lst_ims_proposal_files);
        }

        /// <summary>
        /// Update the Image File Details
        /// </summary>
        /// <param name="fileuploadViewModel"></param>
        /// <returns></returns>
        public string UpdateImageDetailsBAL(FileUploadViewModel fileuploadViewModel)
        {
            IMS_PROPOSAL_FILES ims_proposal_files = new IMS_PROPOSAL_FILES();

            ims_proposal_files.IMS_FILE_ID = Convert.ToInt32(fileuploadViewModel.IMS_FILE_ID);
            ims_proposal_files.IMS_PR_ROAD_CODE = fileuploadViewModel.IMS_PR_ROAD_CODE;
            ims_proposal_files.ISPF_TYPE = "I";
            ims_proposal_files.CHAINAGE = fileuploadViewModel.chainage;
            ims_proposal_files.ISPF_FILE_REMARK = fileuploadViewModel.Image_Description;

            return objProposalDAL.UpdateImageDetailsDAL(ims_proposal_files);
        }

        /// <summary>
        ///  Delete File and File Details
        /// </summary>
        /// <param name="IMS_FILE_ID"></param>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        /// <param name="IMS_FILE_NAME"></param>
        /// <param name="ISPF_TYPE"></param>
        /// <returns></returns>
        public string DeleteFileDetails(int IMS_FILE_ID, int IMS_PR_ROAD_CODE, string IMS_FILE_NAME, string ISPF_TYPE)
        {
            IMS_PROPOSAL_FILES ims_proposal_files = db.IMS_PROPOSAL_FILES.Where(
                a => a.IMS_FILE_ID == IMS_FILE_ID &&
                a.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE &&
                a.ISPF_TYPE.ToUpper() == ISPF_TYPE &&
                    ///Changed by SAMMED A. PATIL for Delete SRRDA files
                    //a.ISPF_TYPE == (PMGSYSession.Current.RoleCode == 22 ? "C" : "P") &&
                a.IMS_FILE_NAME == IMS_FILE_NAME).FirstOrDefault();

            return objProposalDAL.DeleteFileDetailsDAL(ims_proposal_files);
        }

        /// <summary>
        /// Update the PDF File Details
        /// </summary>
        /// <param name="fileuploadViewModel"></param>
        /// <returns></returns>
        public string UpdateJIDetailsBAL(FileUploadViewModel fileuploadViewModel)
        {
            IMS_PROPOSAL_FILES ims_proposal_files = new IMS_PROPOSAL_FILES();

            ims_proposal_files.IMS_FILE_ID = Convert.ToInt32(fileuploadViewModel.IMS_FILE_ID);
            ims_proposal_files.IMS_PR_ROAD_CODE = fileuploadViewModel.IMS_PR_ROAD_CODE;
            ims_proposal_files.ISPF_TYPE = "J";//PMGSYSession.Current.RoleCode == 22 ? "C" : "P";
            ims_proposal_files.ISPF_FILE_REMARK = fileuploadViewModel.PdfDescription;

            return objProposalDAL.UpdatePDFDetailsDAL(ims_proposal_files);
        }

        /// <summary>
        /// Update the PDF File Details
        /// </summary>
        /// <param name="fileuploadViewModel"></param>
        /// <returns></returns>
        public string UpdatePDFDetailsBAL(FileUploadViewModel fileuploadViewModel)
        {
            IMS_PROPOSAL_FILES ims_proposal_files = new IMS_PROPOSAL_FILES();

            ims_proposal_files.IMS_FILE_ID = Convert.ToInt32(fileuploadViewModel.IMS_FILE_ID);
            ims_proposal_files.IMS_PR_ROAD_CODE = fileuploadViewModel.IMS_PR_ROAD_CODE;
            ims_proposal_files.ISPF_TYPE = PMGSYSession.Current.RoleCode == 22 ? "C" : "P";
            ims_proposal_files.ISPF_FILE_REMARK = fileuploadViewModel.PdfDescription;

            return objProposalDAL.UpdatePDFDetailsDAL(ims_proposal_files);
        }

        /// <summary>
        /// This Compresses Image and Creates the Thumbnail
        /// </summary>
        /// <param name="httpPostedFileBase"></param>
        /// <param name="DestinitionPath"></param>
        /// <param name="ThumbnailPath"></param>
        public void CompressImage(HttpPostedFileBase httpPostedFileBase, string DestinitionPath, string ThumbnailPath)
        {
            // For Thumbnail Image            
            ImageResizer.ImageJob ThumbnailJob = new ImageResizer.ImageJob(httpPostedFileBase, ThumbnailPath,
                new ImageResizer.ResizeSettings("width=100;height=75;format=jpg;mode=max"));

            ThumbnailJob.Build();

            HttpPostedFileBase ForResizeConditions = httpPostedFileBase;

            Image image = Image.FromStream(ForResizeConditions.InputStream);
            if (image.Height < 768 || image.Width < 1024)
            {
                httpPostedFileBase.InputStream.Seek(0, SeekOrigin.Begin);
                // For Original Image
                ImageResizer.ImageJob job = new ImageResizer.ImageJob(httpPostedFileBase, DestinitionPath,
                    new ImageResizer.ResizeSettings("width=" + image.Width + ";height=" + image.Height + ";format=jpg;mode=min"));

                job.Build();
            }
            else
            {
                httpPostedFileBase.InputStream.Seek(0, SeekOrigin.Begin);
                // For Original Image
                ImageResizer.ImageJob job = new ImageResizer.ImageJob(httpPostedFileBase, DestinitionPath,
                    new ImageResizer.ResizeSettings("width=1024;height=768;format=jpg;mode=max"));

                job.Build();
            }
        }


        /// <summary>
        /// Validates the PDF File
        /// </summary>
        /// <param name="FileSize"></param>
        /// <param name="FileExtension"></param>
        /// <returns></returns>
        public string ValidatePDFFile(int FileSize, string FileExtension)
        {
            if (FileExtension.ToUpper() != ".PDF")
            {
                return "File is not PDF File";
            }
            if (FileSize > Convert.ToInt32(ConfigurationManager.AppSettings["PROPOSAL_PDF_FILE_MAX_SIZE"]))
            {
                return "File Size Exceed the Maximum File Limit";
            }

            return string.Empty;
        }

        /// <summary>
        /// Validates the Image File
        /// </summary>
        /// <param name="FileSize"></param>
        /// <param name="FileExtension"></param>
        /// <returns></returns>
        public string ValidateImageFile(int FileSize, string FileExtension)
        {
            string ValidExtensions = ConfigurationManager.AppSettings["PROPOSAL_IMAGE_VALID_FORMAT"];
            string[] arrValidFormats = ValidExtensions.Split('$');


            if (!arrValidFormats.Contains(FileExtension.ToLower()))
            {
                return "File is not Valid Image File";
            }
            if (FileSize > Convert.ToInt32(ConfigurationManager.AppSettings["PROPOSAL_IMAGE_FILE_MAX_SIZE"]))
            {
                return "File Size Exceed the Maximum File Limit";
            }

            return string.Empty;
        }
        #endregion

        #region STA Road Proposal

        /// <summary>
        ///  Enlist the Road Proposals for STA Login    
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
        public Array GetSTAProposalsBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int MAST_STATE_CODE, int IMS_YEAR, int MAST_DISTRICT_ID, int IMS_BATCH, int IMS_STREAMS, string IMS_PROPOSAL_TYPE, string IMS_PROPOSAL_STATUS)
        {
            return objProposalDAL.GetSTAProposalsDAL(page, rows, sidx, sord, out totalRecords, MAST_STATE_CODE, IMS_YEAR, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAMS, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS);
        }

        /// <summary>
        /// STA Scrutinize the Road Proposal
        /// </summary>
        /// <param name="staSanctionViewModel"></param>
        /// <param name="ProposalStatus"></param>
        /// <returns></returns>
        public string StaFinalizeProposalBAL(StaSanctionViewModel staSanctionViewModel, String ProposalStatus)
        {
            return objProposalDAL.STAFinalizeProposalDAL(staSanctionViewModel, ProposalStatus);
        }

        #endregion

        #region PTA
        /// <summary>
        /// PTA Scrutinize the Road Proposal
        /// </summary>
        /// <param name="staSanctionViewModel"></param>
        /// <param name="ProposalStatus"></param>
        /// <returns></returns>
        public string PtaFinalizeProposalBAL(PtaSanctionViewModel ptaSanctionViewModel, String ProposalStatus)
        {
            return objProposalDAL.PTAFinalizeProposalDAL(ptaSanctionViewModel, ProposalStatus);
        }
        #endregion

        #region MoRD Road Proposal

        /// <summary>
        /// List the Road Proposals for MoRD Login
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
        public Array GetMordProposalsBAL(int page, int rows, string sidx, string sord, out int totalRecords, int IMS_YEAR, int MAST_STATE_ID, int MAST_DISTRICT_ID, int IMS_BATCH, int IMS_STREAMS, string IMS_PROPOSAL_TYPE, string IMS_PROPOSAL_STATUS, int IMS_AGENCY, string IMS_UPGRADE_CONNECT, out ProposalColumnsTotal totalColModel)
        {
            if (PMGSYSession.Current.RoleCode == 36)
            {
                IMS_PROPOSAL_STATUS = "Y";
            }
            return objProposalDAL.GetMordProposalsDAL(page, rows, sidx, sord, out totalRecords, IMS_YEAR, MAST_STATE_ID, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAMS, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_AGENCY, IMS_UPGRADE_CONNECT, out totalColModel);
        }

        /// <summary>
        ///  Update the Mord Sanction Details
        /// </summary>
        /// <param name="mordSanctionViewModel"></param>
        /// <returns></returns>
        public string UpdateMordSanctionDetailsBAL(MordSanctionViewModel mordSanctionViewModel)
        {
            if (mordSanctionViewModel.IMS_SANCTIONED == "R" || mordSanctionViewModel.IMS_SANCTIONED == "D")
            {
                if (mordSanctionViewModel.IMS_REASON == null || mordSanctionViewModel.IMS_REASON == 0)
                {
                    return " Please Select Reason.";
                }
            }
            return objProposalDAL.UpdateMordSanctionDetailsDAL(mordSanctionViewModel);
        }

        /// <summary>
        /// Get the Road Details
        /// </summary>
        /// <param name="PLAN_CN_ROAD_CODE"></param>
        /// <param name="isStageTwoProposal"></param>
        /// <returns></returns>
        public string GetRoadDetailsBAL(int PLAN_CN_ROAD_CODE, bool isStageTwoProposal, int IMS_PR_ROAD_CODE, int IMS_STAGED_ROAD_ID)
        {
            return objProposalDAL.GetRoadDetailsDAL(PLAN_CN_ROAD_CODE, isStageTwoProposal, IMS_PR_ROAD_CODE, IMS_STAGED_ROAD_ID);
        }

        /// <summary>
        /// Returns an Actions which can be performed by MORD 
        /// </summary>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        /// <returns></returns>
        public List<IMS_GET_ACTIONS_FOR_MORD_Result> GetMordActions(int IMS_PR_ROAD_CODE)
        {
            return db.IMS_GET_ACTIONS_FOR_MORD(IMS_PR_ROAD_CODE).ToList<IMS_GET_ACTIONS_FOR_MORD_Result>();
        }

        /// <summary>
        /// Bulk Sanction Details of Mord
        /// </summary>
        /// <param name="IMS_PR_ROAD_CODES"></param>
        /// <returns></returns>
        public MordSanctionViewModel GetBulkMordDetailBAL(string IMS_PR_ROAD_CODES)
        {
            return objProposalDAL.GetBulkMordDetailDAL(IMS_PR_ROAD_CODES);
        }

        /// <summary>
        /// Bulk Sanction 
        /// </summary>
        /// <param name="IMS_PR_ROAD_CODES"></param>
        /// <returns></returns>
        public string BulkMordDetailBAL(MordSanctionViewModel mordSanctionViewModel)
        {
            return objProposalDAL.BulkMordDetailDAL(mordSanctionViewModel);
        }

        #endregion

        #region Dropdown Population

        /// <summary>
        /// Populates the Existing Packages
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="BatchID"></param>
        /// <returns></returns>
        public List<SelectListItem> PopulateExistingPackage(int Year, int BatchID)
        {
            List<SelectListItem> lstPackage = new List<SelectListItem>();
            SelectListItem item = new SelectListItem();
            item.Text = "Select Package";
            item.Value = "0";
            item.Selected = true;
            lstPackage.Add(item);
            try
            {
                db = new PMGSYEntities();
                var query = (from c in db.IMS_SANCTIONED_PROJECTS
                             where
                                 c.MAST_STATE_CODE == PMGSYSession.Current.StateCode &&
                                 c.MAST_DISTRICT_CODE == PMGSYSession.Current.DistrictCode &&
                                 c.MAST_DPIU_CODE == PMGSYSession.Current.AdminNdCode &&
                                 c.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme &&     // Added by shyam on 27-02-2013
                                 c.IMS_YEAR == Year &&
                                 c.IMS_BATCH == BatchID
                             select new
                             {
                                 Value = c.IMS_PACKAGE_ID,
                                 Text = c.IMS_PACKAGE_ID
                             }).ToList().Distinct();

                foreach (var data in query)
                {
                    item = new SelectListItem();
                    item.Text = data.Text.ToString();
                    item.Value = data.Value.ToString();
                    lstPackage.Add(item);
                }
                return lstPackage;
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "PopulateExistingPackage()");
                return lstPackage;
            }
            finally
            {
                db.Dispose();
            }
        }

        #endregion

        #region UnlockedProposal

        /// <summary>
        /// Get the Road Proposal Details
        /// </summary>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        /// <returns></returns>
        public UnlockProposalViewModel GetPropsoalDetailsBAL(int IMS_PR_ROAD_CODE)
        {
            IMS_SANCTIONED_PROJECTS ims_sanctioned_projects = db.IMS_SANCTIONED_PROJECTS.Find(IMS_PR_ROAD_CODE);
            UnlockProposalViewModel objProposal = new UnlockProposalViewModel();
            try
            {
                objProposal.StateName = db.MASTER_STATE.Where(a => a.MAST_STATE_CODE == ims_sanctioned_projects.MAST_STATE_CODE).Select(a => a.MAST_STATE_NAME).First();
                objProposal.DistrictName = db.MASTER_DISTRICT.Where(a => a.MAST_DISTRICT_CODE == ims_sanctioned_projects.MAST_DISTRICT_CODE).Select(a => a.MAST_DISTRICT_NAME).First();
                objProposal.IMS_PR_ROAD_CODE = ims_sanctioned_projects.IMS_PR_ROAD_CODE;

                objProposal.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                objProposal.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                objProposal.MAST_BLOCK_NAME = ims_sanctioned_projects.MASTER_BLOCK.MAST_BLOCK_NAME;
                objProposal.MAST_STREAM_NAME = ims_sanctioned_projects.MASTER_STREAMS.MAST_STREAM_NAME;
                objProposal.IMS_STAGE_PHASE = ims_sanctioned_projects.IMS_STAGE_PHASE;
                objProposal.IMS_STAGED_YEAR = ims_sanctioned_projects.IMS_STAGED_YEAR;
                objProposal.IMS_STAGED_PACKAGE_ID = ims_sanctioned_projects.IMS_STAGED_PACKAGE_ID;
                objProposal.PLAN_RD_NAME = ims_sanctioned_projects.PLAN_CN_ROAD_CODE == null ? "-" : (ims_sanctioned_projects.PLAN_ROAD.PLAN_CN_ROAD_NUMBER + "-" + ims_sanctioned_projects.PLAN_ROAD.PLAN_RD_NAME);
                objProposal.IMS_UPGRADE_CONNECT = ims_sanctioned_projects.IMS_UPGRADE_CONNECT;

                if (ims_sanctioned_projects.IMS_UPGRADE_CONNECT == "U")
                {
                    if (ims_sanctioned_projects.MAST_EXISTING_SURFACE_CODE != null)
                    {
                        objProposal.MAST_EXISTING_SURFACE_NAME = ims_sanctioned_projects.MASTER_SURFACE.MAST_SURFACE_NAME;
                    }
                    objProposal.IMS_ISBENEFITTED_HABS = ims_sanctioned_projects.IMS_ISBENEFITTED_HABS;
                    if (ims_sanctioned_projects.IMS_ISBENEFITTED_HABS == "N")
                    {
                        objProposal.HABS_REASON_TEXT = db.MASTER_REASON.Where(a => a.MAST_REASON_CODE == ims_sanctioned_projects.IMS_HABS_REASON).Select(a => a.MAST_REASON_NAME).First();
                    }
                }

                objProposal.IMS_YEAR = ims_sanctioned_projects.IMS_YEAR;
                objProposal.IMS_BATCH = ims_sanctioned_projects.IMS_BATCH;
                objProposal.MAST_BLOCK_CODE = ims_sanctioned_projects.MAST_BLOCK_CODE;
                objProposal.IMS_PACKAGE_ID = ims_sanctioned_projects.IMS_PACKAGE_ID;
                objProposal.IMS_COLLABORATION = ims_sanctioned_projects.IMS_COLLABORATION;
                objProposal.IMS_STREAMS = ims_sanctioned_projects.IMS_STREAMS;
                objProposal.MAST_FUNDING_AGENCY_NAME = ims_sanctioned_projects.MASTER_FUNDING_AGENCY.MAST_FUNDING_AGENCY_NAME;
                objProposal.MAST_STREAM_NAME = ims_sanctioned_projects.MASTER_STREAMS.MAST_STREAM_NAME;

                objProposal.PLAN_CN_ROAD_CODE = ims_sanctioned_projects.PLAN_CN_ROAD_CODE;
                objProposal.IMS_PARTIAL_LEN = ims_sanctioned_projects.IMS_PARTIAL_LEN;// (ims_sanctioned_projects.IMS_PARTIAL_LEN.ToUpper() == "F" ? "Full Length" : "Partial Length");
                objProposal.IMS_ROAD_FROM = ims_sanctioned_projects.IMS_ROAD_FROM;
                objProposal.IMS_ROAD_TO = ims_sanctioned_projects.IMS_ROAD_TO;
                objProposal.IMS_ROAD_NAME = ims_sanctioned_projects.IMS_ROAD_NAME;
                objProposal.IMS_PAV_LENGTH = ims_sanctioned_projects.IMS_PAV_LENGTH;
                objProposal.IMS_NO_OF_CDWORKS = ims_sanctioned_projects.IMS_NO_OF_CDWORKS;

                // All Costs
                objProposal.IMS_SANCTIONED_PAV_AMT = ims_sanctioned_projects.IMS_SANCTIONED_PAV_AMT;
                objProposal.IMS_SANCTIONED_CD_AMT = ims_sanctioned_projects.IMS_SANCTIONED_CD_AMT;
                objProposal.IMS_SANCTIONED_PW_AMT = ims_sanctioned_projects.IMS_SANCTIONED_PW_AMT;
                objProposal.IMS_SANCTIONED_OW_AMT = ims_sanctioned_projects.IMS_SANCTIONED_OW_AMT;
                objProposal.IMS_SANCTIONED_RS_AMT = ims_sanctioned_projects.IMS_SANCTIONED_RS_AMT;
                objProposal.IMS_SANCTIONED_MAN_AMT1 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT1;
                objProposal.IMS_SANCTIONED_MAN_AMT2 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT2;
                objProposal.IMS_SANCTIONED_MAN_AMT3 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT3;
                objProposal.IMS_SANCTIONED_MAN_AMT4 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT4;
                objProposal.IMS_SANCTIONED_MAN_AMT5 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT5;

                if (PMGSYSession.Current.PMGSYScheme == 3)
                {
                    objProposal.IMS_RENEWAL_COST = ims_sanctioned_projects.IMS_SANCTIONED_RENEWAL_AMT;
                }

                if (PMGSYSession.Current.PMGSYScheme == 2)
                {
                    objProposal.IMS_RENEWAL_COST = ims_sanctioned_projects.IMS_SANCTIONED_RENEWAL_AMT;
                    objProposal.IMS_IS_HIGHER_SPECIFICATION = ims_sanctioned_projects.IMS_IS_HIGHER_SPECIFICATION;
                    objProposal.TotalCost = ims_sanctioned_projects.IMS_SANCTIONED_PAV_AMT + ims_sanctioned_projects.IMS_SANCTIONED_CD_AMT +
                                                        ims_sanctioned_projects.IMS_SANCTIONED_PW_AMT + ims_sanctioned_projects.IMS_SANCTIONED_OW_AMT +
                                                        Convert.ToDecimal(ims_sanctioned_projects.IMS_SANCTIONED_FC_AMT == null ? 0 : ims_sanctioned_projects.IMS_SANCTIONED_FC_AMT);

                    objProposal.IMS_FURNITURE_COST = ims_sanctioned_projects.IMS_FURNITURE_COST;
                    objProposal.IMS_HIGHER_SPECIFICATION_COST = ims_sanctioned_projects.IMS_HIGHER_SPECIFICATION_COST;
                    objProposal.IMS_SHARE_PERCENT = ims_sanctioned_projects.IMS_SHARE_PERCENT;

                    if (ims_sanctioned_projects.IMS_SHARE_PERCENT == 1)
                    {
                        objProposal.IMS_SANCTIONED_AMOUNT = (objProposal.TotalCost * 90) / 100;
                    }
                    else if (ims_sanctioned_projects.IMS_SHARE_PERCENT == 2)
                    {
                        objProposal.IMS_SANCTIONED_AMOUNT = (objProposal.TotalCost * 75) / 100;
                    }

                }

                objProposal.TotalMaintenanceCost = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT1 + ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT2 + ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT3 + ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT4 + ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT5;

                if (PMGSYSession.Current.PMGSYScheme == 2)
                {
                    objProposal.TotalMaintenanceCost += (ims_sanctioned_projects.IMS_SANCTIONED_RENEWAL_AMT.HasValue ? ims_sanctioned_projects.IMS_SANCTIONED_RENEWAL_AMT.Value : 0);
                }
                objProposal.IMS_ZP_RESO_OBTAINED = ims_sanctioned_projects.IMS_ZP_RESO_OBTAINED;
                objProposal.MAST_MP_CONST_NAME = ims_sanctioned_projects.MASTER_MP_CONSTITUENCY == null ? "NA" : ims_sanctioned_projects.MASTER_MP_CONSTITUENCY.MAST_MP_CONST_NAME;
                objProposal.MAST_MLA_CONST_NAME = ims_sanctioned_projects.MASTER_MLA_CONSTITUENCY == null ? "NA" : ims_sanctioned_projects.MASTER_MLA_CONSTITUENCY.MAST_MLA_CONST_NAME;

                objProposal.MAST_MP_CONST_CODE = ims_sanctioned_projects.MAST_MP_CONST_CODE;
                objProposal.MAST_MLA_CONST_CODE = ims_sanctioned_projects.MAST_MLA_CONST_CODE;

                objProposal.IMS_CARRIAGED_WIDTH = ims_sanctioned_projects.IMS_CARRIAGED_WIDTH;
                objProposal.Display_Carriaged_Width = ims_sanctioned_projects.IMS_CARRIAGED_WIDTH != null ? db.MASTER_CARRIAGE.Where(a => a.MAST_CARRIAGE_CODE == ims_sanctioned_projects.IMS_CARRIAGED_WIDTH).Select(a => a.MAST_CARRIAGE_WIDTH).First().ToString() : "NA";
                objProposal.IMS_TRAFFIC_TYPE = ims_sanctioned_projects.IMS_TRAFFIC_TYPE;
                if (ims_sanctioned_projects.MASTER_TRAFFIC_TYPE != null)
                {
                    objProposal.IMS_TRAFFIC_CATAGORY_NAME = ims_sanctioned_projects.MASTER_TRAFFIC_TYPE.MAST_TRAFFIC_NAME;
                }
                else
                {
                    objProposal.IMS_TRAFFIC_CATAGORY_NAME = "NA";
                }
                objProposal.IMS_PROPOSED_SURFACE = ims_sanctioned_projects.IMS_PROPOSED_SURFACE;

                objProposal.IMS_CC_LENGTH = ims_sanctioned_projects.IMS_CC_LENGTH;
                objProposal.IMS_BT_LENGTH = ims_sanctioned_projects.IMS_BT_LENGTH;
                objProposal.MAST_EXISTING_SURFACE_CODE = ims_sanctioned_projects.MAST_EXISTING_SURFACE_CODE;
                objProposal.IMS_REMARKS = ims_sanctioned_projects.IMS_REMARKS;
                objProposal.IMS_REASON = ims_sanctioned_projects.IMS_REASON;
                if (ims_sanctioned_projects.IMS_REASON != null)
                {
                    objProposal.Reason = db.MASTER_REASON.Where(a => a.MAST_REASON_CODE == ims_sanctioned_projects.IMS_REASON).Select(a => a.MAST_REASON_NAME).First();
                }
                objProposal.IMS_IS_STAGED = ims_sanctioned_projects.IMS_IS_STAGED;
                objProposal.IMS_ISCOMPLETED = ims_sanctioned_projects.IMS_ISCOMPLETED;
                objProposal.STA_SANCTIONED = ims_sanctioned_projects.STA_SANCTIONED;
                objProposal.STA_SANCTIONED_BY = ims_sanctioned_projects.STA_SANCTIONED_BY;
                if (ims_sanctioned_projects.STA_SANCTIONED_DATE != null && Convert.ToDateTime(ims_sanctioned_projects.STA_SANCTIONED_DATE).Year != 0)
                {
                    DateTime dateTime = new DateTime();
                    dateTime = Convert.ToDateTime(ims_sanctioned_projects.STA_SANCTIONED_DATE);
                    objProposal.STA_SANCTIONED_DATE = dateTime.ToString("dd-MMM-yyyy");
                }
                objProposal.MS_STA_REMARKS = ims_sanctioned_projects.IMS_STA_REMARKS;
                objProposal.IMS_SANCTIONED = ims_sanctioned_projects.IMS_SANCTIONED;
                objProposal.IMS_SANCTIONED_BY = ims_sanctioned_projects.IMS_SANCTIONED_BY;
                if (ims_sanctioned_projects.IMS_SANCTIONED_DATE != null && Convert.ToDateTime(ims_sanctioned_projects.IMS_SANCTIONED_DATE).Year != 0)
                {
                    DateTime dateTime = new DateTime();
                    dateTime = Convert.ToDateTime(ims_sanctioned_projects.IMS_SANCTIONED_DATE);
                    objProposal.IMS_SANCTIONED_DATE = dateTime.ToString("dd-MMM-yyyy");
                }
                objProposal.IMS_PROG_REMARKS = ims_sanctioned_projects.IMS_PROG_REMARKS;
                objProposal.IMS_LOCK_STATUS = ims_sanctioned_projects.IMS_LOCK_STATUS;
                objProposal.IMS_SHARE_PERCENT_2015 = ims_sanctioned_projects.IMS_SHARE_PERCENT_2015;
                objProposal.IMS_STATE_SHARE_2015 = ims_sanctioned_projects.IMS_STATE_SHARE_2015.HasValue ? ims_sanctioned_projects.IMS_STATE_SHARE_2015.Value : 0;
                objProposal.IMS_MORD_SHARE_2015 = ims_sanctioned_projects.IMS_MORD_SHARE_2015.HasValue ? ims_sanctioned_projects.IMS_MORD_SHARE_2015.Value : 0;
                objProposal.IMS_TOTAL_COST_2015 = objProposal.IMS_STATE_SHARE_2015 + objProposal.IMS_MORD_SHARE_2015;
                //objProposal.IMS_SHARE_PERCENT_2015 = ims_sanctioned_projects.IMS_SHARE_PERCENT_2015 == null ? ((byte)(db.MASTER_STATE_FUND_SHARING_MAPPING.Where(m => m.MAST_SHARE_CODE == PMGSYSession.Current.StateCode).Select(m => m.MAST_SHARE_CODE).FirstOrDefault() == 0 ? 3 : db.MASTER_STATE_FUND_SHARING_MAPPING.Where(m => m.MAST_SHARE_CODE == PMGSYSession.Current.StateCode).Select(m => m.MAST_SHARE_CODE).FirstOrDefault())) : ims_sanctioned_projects.IMS_SHARE_PERCENT_2015;


                if (objProposalDAL.IsProposalFinanciallyClosed(ims_sanctioned_projects.IMS_PR_ROAD_CODE) && ims_sanctioned_projects.IMS_SHARE_PERCENT_2015 == null)
                {
                    objProposal.IMS_SHARE_PERCENT_2015 = 4;
                }
                else
                {
                    objProposal.IMS_SHARE_PERCENT_2015 = ims_sanctioned_projects.IMS_SHARE_PERCENT_2015 == null ? ((byte)(db.MASTER_STATE_FUND_SHARING_MAPPING.Where(m => m.MAST_SHARE_CODE == PMGSYSession.Current.StateCode).Select(m => m.MAST_SHARE_CODE).FirstOrDefault() == 0 ? 3 : db.MASTER_STATE_FUND_SHARING_MAPPING.Where(m => m.MAST_SHARE_CODE == PMGSYSession.Current.StateCode).Select(m => m.MAST_SHARE_CODE).FirstOrDefault())) : ims_sanctioned_projects.IMS_SHARE_PERCENT_2015;
                }

                objProposal.Stage_2_Year = ims_sanctioned_projects.IMS_STAGED_YEAR;
                objProposal.Stage_2_Package_ID = ims_sanctioned_projects.IMS_STAGED_PACKAGE_ID;
                objProposal.IMS_STAGED_ROAD_ID = ims_sanctioned_projects.IMS_STAGED_ROAD_ID;

                objProposal.DUP_IMS_PAV_LENGTH = (from c in db.PLAN_ROAD
                                                  where c.PLAN_CN_ROAD_CODE == ims_sanctioned_projects.PLAN_CN_ROAD_CODE
                                                  select c.PLAN_RD_LENGTH).FirstOrDefault();

                objProposal.stateType = (from c in db.MASTER_STATE where c.MAST_STATE_CODE == PMGSYSession.Current.StateCode select c.MAST_STATE_TYPE).FirstOrDefault();
                //following property added by Vikram for providing the staged details to the districts which are IAP_DISTRICT
                objProposal.DistrictType = (from c in db.MASTER_DISTRICT where c.MAST_DISTRICT_CODE == PMGSYSession.Current.DistrictCode select c.MAST_IAP_DISTRICT).FirstOrDefault();

                return objProposal;
            }
            catch (DbUpdateException ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "ProposalBAL.GetPropsoalDetailsBAL(DbUpdateException ex)");
                return null;
            }
            catch (OptimisticConcurrencyException ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "ProposalBAL.GetPropsoalDetailsBAL(OptimisticConcurrencyException ex)");
                return null;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ProposalBAL.GetPropsoalDetailsBAL()");
                return null;
            }
        }

        /// <summary>
        /// Updates the Unlocked Road Proposal
        /// </summary>
        /// <param name="proposalViewModel"></param>
        /// <returns></returns>
        public string UpdateUnlockedProposalBAL(UnlockProposalViewModel proposalViewModel)
        {
            IProposalDAL objProposalDAL = new ProposalDAL();
            return objProposalDAL.UpdateUnlockedProposedDAL(proposalViewModel);
        }

        /// <summary>
        /// Dpiu Fianlize the Unlocked Proposals
        /// </summary>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        /// <returns></returns>
        public string DPIUFinalizeUnlockedProposalBAL(int IMS_PR_ROAD_CODE)
        {
            return objProposalDAL.DPIUFinalizeUnlockedProposalDAL(IMS_PR_ROAD_CODE);
        }

        /// <summary>
        ///  Called when we Lock the Unlocked Proposal
        /// </summary>
        /// <param name="habitationViewModel"></param>
        /// <returns></returns>
        public string CheckHabitationsForUnlock(HabitationViewModel habitationViewModel)
        {
            int requiredPopulation = 500;

            if (habitationViewModel.MAST_STATE_TYPE.ToUpper() == "N" || habitationViewModel.MAST_STATE_TYPE.ToUpper() == "H" || habitationViewModel.MAST_STATE_TYPE.ToUpper() == "X")
            {
                requiredPopulation = 250;
            }

            if (habitationViewModel.MAST_IAP_DISTRICT.ToUpper() == "Y")
            {
                requiredPopulation = 250;
            }

            if (habitationViewModel.MAST_IS_TRIBAL.ToUpper() == "Y")
            {
                requiredPopulation = 250;
            }

            var ClusterWisePopulationList = (
                                                from c in db.IMS_BENEFITED_HABS
                                                join d in db.MASTER_HABITATIONS_DETAILS
                                                    on c.MAST_HAB_CODE equals d.MAST_HAB_CODE
                                                where c.IMS_PR_ROAD_CODE == habitationViewModel.IMS_PR_ROAD_CODE
                                                group new { c, d } by c.MAST_CLUSTER_CODE into ClusterGroup
                                                select new
                                                {
                                                    cluster = ClusterGroup.Key == null ? 0 : ClusterGroup.Key,
                                                    population = ClusterGroup.Sum(p => p.d.MAST_HAB_TOT_POP),
                                                    name = ClusterGroup.Sum(p => p.d.MAST_HAB_CODE)
                                                }
                                            );

            foreach (var data in ClusterWisePopulationList)
            {
                if (data.cluster != 0 && data.population < requiredPopulation)
                {
                    return "Population of Cluster " + data.cluster + " is less than " + requiredPopulation + " .\nPopulation of a Cluster should not be Less than " + requiredPopulation;
                }
                else if (data.cluster == 0 && data.population < requiredPopulation)
                {
                    return "Population of Habitation " + " must be Greator than " + requiredPopulation + ". if it is not in a Cluster, \nInclude Habitataion in cluster to finalize.";
                }
            }
            return string.Empty;
        }

        #endregion

        #region REVISION
        // new change done by Vikram on 16-09-2013

        public RevisedCostLengthViewModel GetOldRevisedCostLengthBAL(int proposalCode)
        {
            try
            {
                IProposalDAL objDAL = new ProposalDAL();
                return objDAL.GetOldRevisedCostLengthDAL(proposalCode);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "GetOldRevisedCostLengthBAL()");
                return null;
            }
        }

        public Array GetRevisedCostLengthListBAL(int? page, int? rows, string sidx, string sord, out long totalRecords, int proposalCode)
        {
            try
            {
                IProposalDAL objDAL = new ProposalDAL();
                return objDAL.GetRevisedCostLengthListDAL(page, rows, sidx, sord, out totalRecords, proposalCode);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "GetRevisedCostLengthListBAL()");
                totalRecords = 0;
                return null;
            }
        }

        public Array GetRevisionBridgeListBAL(int? page, int? rows, string sidx, string sord, out long totalRecords, int proposalCode)
        {
            try
            {
                IProposalDAL objDAL = new ProposalDAL();
                return objDAL.GetRevisionBridgeListDAL(page, rows, sidx, sord, out totalRecords, proposalCode);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "GetRevisionBridgeListBAL()");
                totalRecords = 0;
                return null;
            }
        }

        public bool AddRevisedCostLengthBAL(RevisedCostLengthViewModel model, ref string message)
        {
            try
            {
                IProposalDAL objDAL = new ProposalDAL();
                return objDAL.AddRevisedCostLengthDAL(model, ref message);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "AddRevisedCostLengthBAL()");
                return false;
            }
        }


        public bool EditRevisionDetailsBAL(RevisedCostLengthViewModel model, ref string message)
        {
            try
            {
                IProposalDAL objDAL = new ProposalDAL();
                return objDAL.EditRevisionDetailsDAL(model, ref message);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "EditRevisionDetailsBAL()");
                return false;
            }
        }

        public RevisedCostLengthViewModel GetRevisionDetailsBAL(int proposalCode, int revisionCode)
        {
            try
            {
                objProposalDAL = new ProposalDAL();
                return objProposalDAL.GetRevisionDetailsDAL(proposalCode, revisionCode);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "GetRevisionDetailsBAL()");
                return null;
            }
        }

        #endregion

        #region TECHNOLOGY


        public bool AddTechnologyDetailsBAL(TechnologyDetailsViewModel model, ref string message)
        {
            try
            {
                objProposalDAL = new ProposalDAL();
                return objProposalDAL.AddTechnologyDetailsDAL(model, ref message);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AddTechnologyDetailsBAL()");
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error occurred while processing your request.";
                return false;
            }
        }

        public bool EditTechnologyDetailsBAL(TechnologyDetailsViewModel model, ref string message)
        {
            try
            {
                objProposalDAL = new ProposalDAL();
                return objProposalDAL.EditTechnologyDetailsDAL(model, ref message);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EditTechnologyDetailsBAL()");
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error occurred while processing your request.";
                return false;
            }
        }

        public Array GetTechnologyDetailsListBAL(int? page, int? rows, string sidx, string sord, out long totalRecords, int proposalCode)
        {
            try
            {
                IProposalDAL objDAL = new ProposalDAL();
                return objDAL.GetTechnologyDetailsListDAL(page, rows, sidx, sord, out totalRecords, proposalCode);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "GetTechnologyDetailsListBAL()");
                totalRecords = 0;
                return null;
            }
        }

        public TechnologyDetailsViewModel GetTechnologyDetails(int proposalCode, int segmentCode)
        {
            try
            {
                objProposalDAL = new ProposalDAL();
                return objProposalDAL.GetTechnologyDetails(proposalCode, segmentCode);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "GetTechnologyDetails()");
                return null;
            }
        }

        public bool DeleteTechnologyDetails(int proposalCode, int segmentCode)
        {
            try
            {
                objProposalDAL = new ProposalDAL();
                return objProposalDAL.DeleteTechnologyDetails(proposalCode, segmentCode);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DeleteTechnologyDetailsBAL()");
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
        }

        public decimal? GetTechnologyStartChainage(int proposalCode, int techCode, int layerCode)
        {
            try
            {
                objProposalDAL = new ProposalDAL();
                return objProposalDAL.GetTechnologyStartChainage(proposalCode, techCode, layerCode);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "GetTechnologyStartChainage()");
                return 0;
            }
        }

        #endregion

        #region Test Result Details

        /// <summary>
        /// Test Result Details
        /// </summary>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array TestResultDetails(int IMS_PR_ROAD_CODE, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            IProposalDAL objProposalDAL = new ProposalDAL();
            return objProposalDAL.TestResultDetails(IMS_PR_ROAD_CODE, page, rows, sidx, sord, out totalRecords);
        }

        /// <summary>
        /// Sample Details for Test Details.
        /// </summary>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array TestResultSampleDetails(int IMS_PR_ROAD_CODE, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            IProposalDAL objProposalDAL = new ProposalDAL();
            return objProposalDAL.TestResultSampleDetails(IMS_PR_ROAD_CODE, page, rows, sidx, sord, out totalRecords);
        }

        /// <summary>
        /// Add Test Result Details
        /// </summary>
        /// <param name="testResultViewModel"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool AddTestResultDetails(TestResultViewModel testResultViewModel, ref string message)
        {
            IProposalDAL objProposalDAL = new ProposalDAL();
            return objProposalDAL.AddTestResultDetails(testResultViewModel, ref message);
        }

        /// <summary>
        /// Get Test result details
        /// </summary>
        /// <param name="ResultCode"></param>
        /// <param name="ImsPrRoadCode"></param>
        /// <returns></returns>
        public TestResultViewModel EditTestResultDetails(int ResultCode, int ImsPrRoadCode)
        {
            IProposalDAL objProposalDAL = new ProposalDAL();
            return objProposalDAL.EditTestResultDetails(ResultCode, ImsPrRoadCode);
        }

        /// <summary>
        /// Update Test result details.
        /// </summary>
        /// <param name="testResultViewModel"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool UpdateTestResultDetails(TestResultViewModel testResultViewModel, ref string message)
        {
            IProposalDAL objProposalDAL = new ProposalDAL();
            return objProposalDAL.UpdateTestResultDetails(testResultViewModel, ref message);
        }

        /// <summary>
        /// Delete test result details.
        /// </summary>
        /// <param name="resultCode"></param>
        /// <param name="imsPrRoadCode"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool DeleteTestResultDetails(int resultCode, int imsPrRoadCode, ref string message)
        {
            IProposalDAL objProposalDAL = new ProposalDAL();
            return objProposalDAL.DeleteTestResultDetails(resultCode, imsPrRoadCode, ref message);
        }

        //login ITNO

        /// <summary>
        /// Get Road proposal List.
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
        /// <returns></returns>
        public Array GetItnoProposalsBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int MAST_STATE_CODE, int IMS_YEAR, int MAST_DISTRICT_ID, int IMS_BATCH, int IMS_STREAMS)
        {
            return objProposalDAL.GetItnoProposalsDAL(page, rows, sidx, sord, out totalRecords, MAST_STATE_CODE, IMS_YEAR, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAMS);
        }

        /// <summary>
        /// Get road details such as road name,Length, package.
        /// </summary>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        /// <returns></returns>
        public IMS_SANCTIONED_PROJECTS GetRoadDetails(int IMS_PR_ROAD_CODE)
        {
            return objProposalDAL.GetRoadDetails(IMS_PR_ROAD_CODE);
        }

        #endregion

        #region SANCTION_ORDER_GENERATION

        public Array GetProposalsForSanctionOrder(int? page, int? rows, string sidx, string sord, out long totalRecords, int stateCode, int year, int stream, int agency, int batch, int scheme, string proposalType, out bool IsSOGenerated)
        {
            return objProposalDAL.GetProposalsForSanctionOrder(page, rows, sidx, sord, out totalRecords, stateCode, year, stream, agency, batch, scheme, proposalType, out IsSOGenerated);
        }

        public Array GetSanctionOrderListBAL(int? page, int? rows, string sidx, string sord, out long totalRecords, int stateCode, int year, int stream, int agency, int batch, int scheme, string proposalType)
        {
            return objProposalDAL.GetSanctionOrderListDAL(page, rows, sidx, sord, out totalRecords, stateCode, year, stream, agency, batch, scheme, proposalType);
        }

        public bool AddSanctionOrderBAL(SanctionOrderViewModel model, ref string message)
        {
            return objProposalDAL.AddSanctionOrderDAL(model, ref message);
        }

        #endregion

        #region REPACKAGING

        public Array GetProposalsForRepackaging(int? page, int? rows, string sidx, string sord, out long totalRecords, int year, int batch, int block, string package, int collaboration, string proposalType, string upgradationType)
        {
            return objProposalDAL.GetProposalsForRepackaging(page, rows, sidx, sord, out totalRecords, year, batch, block, package, collaboration, proposalType, upgradationType);
        }

        public bool AddRepackagingDetails(RepackagingDetailsViewModel model)
        {
            return objProposalDAL.AddRepackagingDetails(model);
        }

        #endregion

        #region DPR_LIST

        public Array GetDPRProposalListBAL(int stateCode, int districtCode, int blockCode, int year, int batch, int collaboStream, string proposalType, string proposalStatus, string packageId, string connectivity, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            return objProposalDAL.GetDPRProposalListDAL(stateCode, districtCode, blockCode, year, batch, collaboStream, proposalType, proposalStatus, packageId, connectivity, page, rows, sidx, sord, out totalRecords);
        }

        public bool DeleteDPRProposalBAL(int proposalCode)
        {
            return objProposalDAL.DeleteDPRProposalDAL(proposalCode);
        }

        #endregion

        #region OLD_PROPOSAL_UPDATE

        public Array GetProposalsForUpdateBAL(int? page, int? rows, string sidx, string sord, out long totalRecords, int stateCode, int districtCode, int IMS_YEAR, int MAST_BLOCK_ID, int IMS_BATCH, int IMS_STREAMS, string IMS_PROPOSAL_TYPE, int adminCode, string IMS_PROPOSAL_STATUS, string IMS_UPGRADE_CONNECT)
        {
            return objProposalDAL.GetProposalsForUpdateDAL(page, rows, sidx, sord, out totalRecords, stateCode, districtCode, IMS_YEAR, MAST_BLOCK_ID, IMS_BATCH, IMS_STREAMS, IMS_PROPOSAL_TYPE, adminCode, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT);
        }

        public Array GetProposalsForCNMappingBAL(int? page, int? rows, string sidx, string sord, out long totalRecords, int stateCode, int districtCode, int IMS_YEAR, int MAST_BLOCK_ID, int IMS_BATCH, int IMS_STREAMS, string IMS_PROPOSAL_TYPE, int adminCode, string IMS_PROPOSAL_STATUS, string IMS_UPGRADE_CONNECT)
        {
            return objProposalDAL.GetProposalsForCNMappingDAL(page, rows, sidx, sord, out totalRecords, stateCode, districtCode, IMS_YEAR, MAST_BLOCK_ID, IMS_BATCH, IMS_STREAMS, IMS_PROPOSAL_TYPE, adminCode, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT);
        }

        public bool UpdateProposalDetailsBAL(ProposalUpdateViewModel model, out string message)
        {
            return objProposalDAL.UpdateProposalDetailsDAL(model, out message);
        }

        public ProposalUpdateViewModel GetOldProposalDetailsBAL(int proposalCode)
        {
            return objProposalDAL.GetOldProposalDetailsDAL(proposalCode);
        }

        public bool ChangeCompleteProposalToStagedBAL(int proposalCode, out string message)
        {
            return objProposalDAL.ChangeCompleteProposalToStagedDAL(proposalCode, out message);
        }

        public bool ChangeStagedProposalToCompleteBAL(int proposalCode, out string message)
        {
            return objProposalDAL.ChangeStagedProposalToCompleteDAL(proposalCode, out message);
        }

        public bool MapCoreNetworkDetailsBAL(CoreNetworkMappingViewModel model)
        {
            return objProposalDAL.MapCoreNetworkDetailsDAL(model);
        }

        public bool ChangeStage1ProposalToStage2BAL(Stage1ToStage2ViewModel model)
        {
            return objProposalDAL.ChangeStage1ProposalToStage2DAL(model);
        }

        public bool ChangeCompleteProposalsToStage2BAL(Stage1ToStage2ViewModel model)
        {
            return objProposalDAL.ChangeCompleteProposalsToStage2DAL(model);
        }

        //public bool UpdateProposalPIUDetailsBAL(ProposalPIUUpdateViewModel model)
        //{
        //    return objProposalDAL.UpdateProposalPIUDetailsDAL(model);
        //}
        public string UpdateProposalPIUDetailsBAL(ProposalPIUUpdateViewModel_New model)
        {
            return objProposalDAL.UpdateProposalPIUDetailsDAL(model);
        }

        public bool UpdateProposalBlockDetailsBAL(ProposalUpdateBlockViewModel model)
        {
            return objProposalDAL.UpdateProposalDetailsBlockDAL(model);
        }

        #endregion

        #region CHANGING_CORE_NETWORK

        public bool UpdateCoreNetworkDetailsBAL(CoreNetworkUpdateViewModel model)
        {
            return objProposalDAL.UpdateCoreNetworkDetailsDAL(model);
        }

        public bool UpdateBlockDetailsBAL(ProposalBlockUpdateViewModel model)
        {
            return objProposalDAL.UpdateBlockDetailsDAL(model);
        }

        #endregion

        #region Proposal Additional Cost Detail
        public Array GetProposalAdditionalCostListBAL(int stateCode, int districtCode, int blockCode, int yearCode, string packageCode, string proposalCode, int batchCode, int streamCode, string upgradationType, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                return objProposalDAL.GetProposalAdditionalCostListDAL(stateCode, districtCode, blockCode, yearCode, packageCode, proposalCode, batchCode, streamCode, upgradationType, page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "GetProposalAdditionalCostListBAL()");
                totalRecords = 0;
                return null;
            }
        }
        public Array GetAdditionalCostListBAL(int roadCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return objProposalDAL.GetAdditionalCostListDAL(roadCode, page, rows, sidx, sord, out totalRecords);
        }
        public bool AddAdditionalCostDetailsBAL(ProposalAdditionalCostModel proposalAdditionalCostModel, ref string message)
        {
            return objProposalDAL.AddAdditionalCostDetailsDAL(proposalAdditionalCostModel, ref message);
        }
        public ProposalAdditionalCostModel EditAdditionalCostDetailsBAL(int transctionCode, int imsPrRoadCode)
        {
            return objProposalDAL.EditAdditionalCostDetailsDAL(transctionCode, imsPrRoadCode);
        }
        public bool UpdateAdditionalCostDetailsBAL(ProposalAdditionalCostModel proposalAdditionalCostModel, ref string message)
        {
            return objProposalDAL.UpdateAdditionalCostDetailsDAL(proposalAdditionalCostModel, ref message);
        }
        public bool DeleteAdditionalCostDetailsBAL(int transactionCode, int imsPrRoadCode, ref string message)
        {
            return objProposalDAL.DeleteAdditionalCostDetailsDAL(transactionCode, imsPrRoadCode, ref message);

        }
        #endregion

        #region  Upload Sanction Order MRD CLEARANCE LETTERS
        public bool AddMrdClearanceBAL(MrdClearenceViewModel mrdClearancanceViewModel, ref string message)
        {
            return objProposalDAL.AddMrdClearanceDAL(mrdClearancanceViewModel, ref message);
        }
        public Array ListMrdClearanceBAL(int stateCode, int year, int batch, int agencyCode, int collaboration, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return objProposalDAL.ListMrdClearanceDAL(stateCode, year, batch, agencyCode, collaboration, page, rows, sidx, sord, out totalRecords);
        }
        public Array ListMrdClearanceFileBAL(int clearanceCode, string clearanceStatus, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return objProposalDAL.ListMrdClearanceFileDAL(clearanceCode, clearanceStatus, page, rows, sidx, sord, out totalRecords);
        }
        public bool DeleteMrdClearanceBAL(int clearanceCode, ref string message)
        {
            return objProposalDAL.DeleteMrdClearanceDAL(clearanceCode, ref message);
        }
        public bool EditMrdClearanceBAL(MrdClearenceViewModel mrdClearanceViewModel, ref string message)
        {
            return objProposalDAL.EditMrdClearanceDAL(mrdClearanceViewModel, ref message);
        }
        public MrdClearenceViewModel GetMrdClearanceDetailsBAL(int clearanceCode)
        {
            return objProposalDAL.GetMrdClearanceDetailsDAL(clearanceCode);
        }

        public bool AddMrdClearanceRevisionBAL(MrdClearenceRevisionViewModel mrdClearanceRevisionViewModel, ref string message)
        {
            return objProposalDAL.AddMrdClearanceRevisionDAL(mrdClearanceRevisionViewModel, ref message);
        }
        public bool DeleteMrdClearanceRevisionBAL(int clearanceCode, ref string message)
        {
            return objProposalDAL.DeleteMrdClearanceRevisionDAL(clearanceCode, ref message);
        }
        public MrdClearenceRevisionViewModel GetMrdClearanceRevisionDetailsBAL(int clearanceCode, string action)
        {
            return objProposalDAL.GetMrdClearanceRevisionDetailsDAL(clearanceCode, action);
        }
        public bool EditMrdClearanceRevsionBAL(MrdClearenceRevisionViewModel mrdClearanceRevsionViewModel, ref string message)
        {
            return objProposalDAL.EditMrdClearanceRevsionDAL(mrdClearanceRevsionViewModel, ref message);
        }
        public bool EditDeleteMrdClearanceFileBAL(int clearanceCode, string fileType, string fileName, ref string message)
        {
            return objProposalDAL.EditDeleteMrdClearanceFileDAL(clearanceCode, fileType, fileName, ref message);
        }
        public Array ListMrdClearanceRevisionBAL(int clearanceCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return objProposalDAL.ListMrdClearanceRevisionDAL(clearanceCode, page, rows, sidx, sord, out totalRecords);
        }

        public Array ListOriginalMrdClearanceBAL(int clearanceCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return objProposalDAL.ListOriginalMrdClearanceDAL(clearanceCode, page, rows, sidx, sord, out totalRecords);
        }

        #endregion


        #region  Habitation Finalization

        public Array GetMordProposalsforHabFinalizationBAL(int page, int rows, string sidx, string sord, out int totalRecords, int IMS_YEAR, int MAST_STATE_ID, int MAST_DISTRICT_ID, int IMS_BATCH, int IMS_STREAMS, string IMS_PROPOSAL_TYPE, string IMS_PROPOSAL_STATUS, int IMS_AGENCY, string IMS_UPGRADE_CONNECT)
        {
            return objProposalDAL.GetMordProposalsforHabFinalizationDAL(page, rows, sidx, sord, out totalRecords, IMS_YEAR, MAST_STATE_ID, MAST_DISTRICT_ID, IMS_BATCH, IMS_STREAMS, IMS_PROPOSAL_TYPE, IMS_PROPOSAL_STATUS, IMS_AGENCY, IMS_UPGRADE_CONNECT);
        }


        public bool DefinalizeHabitationBAL(int proposalCode)
        {
            return objProposalDAL.DefinalizeHabitationDAL(proposalCode);

        }
        #endregion

        #region GEPNIC_INTEGRATION

        public Array GetGepnicProposals(int page, int rows, string sidx, string sord, out int totalRecords, int State, int District, int Year, int Block, string ProposalType, string Package)
        {
            return objProposalDAL.GetGepnicProposals(page, rows, sidx, sord, out totalRecords, State, District, Year, Block, ProposalType, Package);
        }

        #endregion

        #region Dropped Proposal[by Pradip Patil (10/04/2017)]
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
        /// <param name="MAST_DPIU_CODE"></param>
        /// <param name="Filters"></param>
        /// <returns></returns>
        public Array GetDroppingProposalsForSRRDABAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int MAST_STATE_CODE, int MAST_DISTRICT_CODE, int IMS_YEAR, int IMS_BATCH, int IMS_STREAMS, String ProposalType, string IMS_PROPOSAL_STATUS, string IMS_UPGRADE_CONNECT, int MAST_BLOCK_CODE, string Filters, out ProposalColumnsTotal totalColModel)
        {
            return objProposalDAL.GetDroppingProposalsForSRRDADAL(page, rows, sidx, sord, out totalRecords, MAST_STATE_CODE, MAST_DISTRICT_CODE, IMS_YEAR, IMS_BATCH, IMS_STREAMS, ProposalType, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT, MAST_BLOCK_CODE, Filters, out totalColModel);
        }

        public bool DropProposal(List<int> imsRoadCodeList, out String result)
        {
            return objProposalDAL.DropProposalDAL(imsRoadCodeList, out result);
        }

        public Array GetProposalsForDroppedOrder(int? page, int? rows, string sidx, string sord, out long totalRecords, int stateCode, int year, int stream, int batch, int scheme, string proposalType, out bool IsDOGenerated, string reqCode)
        {
            return objProposalDAL.GetProposalsForDroppedOrder(page, rows, sidx, sord, out totalRecords, stateCode, year, stream, batch, scheme, proposalType, out IsDOGenerated, reqCode);
        }

        public bool AddDropOrderBAL(DropOrderViewModel model, List<int> mrdselectedroadList, ref string message)
        {
            return objProposalDAL.AddDropOrderDAL(model, mrdselectedroadList, ref message);
        }

        //public bool AddDropOrderBAL(DropOrderViewModel model, string [] dropApproveArray, ref string message)
        //{
        //    return objProposalDAL.AddDropOrderDAL(model,dropApproveArray, ref message);
        //}

        public Array GetDropOrderListBAL(int? page, int? rows, string sidx, string sord, out long totalRecords, int stateCode, int year, int stream, int batch, int scheme, string proposalType, string Status)
        {
            return objProposalDAL.GetDropOrderListDAL(page, rows, sidx, sord, out totalRecords, stateCode, year, stream, batch, scheme, proposalType, Status);
        }

        public Array GetDetailDropOrderListBAL(int? page, int? rows, string sidx, string sord, out long totalRecords, int RequestCode, int scheme)
        {
            return objProposalDAL.GetDetailDropOrderListBAL(page, rows, sidx, sord, out totalRecords, RequestCode, scheme);
        }

        public bool AddDropProposalBAL(AddDropOrderViewModel model, out String result)
        {
            return objProposalDAL.AddDropProposalDAL(model, out result);
        }

        public Array ListDropppingWorksBAL(int stateCode, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            return objProposalDAL.ListDropppingWorksDAL(stateCode, page, rows, sidx, sord, out totalRecords);
        }

        public Array ListWorksForDropppingBAL(int reqCode, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            return objProposalDAL.ListWorksForDropppingDAL(reqCode, page, rows, sidx, sord, out totalRecords);
        }

        public bool AddDropRequestDetailsBAL(string[] dropDetails, string letterNo, out String result)
        {
            return objProposalDAL.AddDropRequestDetailsDAL(dropDetails, letterNo, out result);
        }
        #endregion

        #region Matrix Master
        public Array ListMatrixParametersDetailsBAL(int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                return objProposalDAL.ListMatrixParametersDetailsDAL(page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ListMatrixParametersDetailsBAL()");
                totalRecords = 0;
                return null;
            }
        }

        public Array ListMatrixParametersWeightageDetailsBAL(int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                return objProposalDAL.ListMatrixParametersWeightageDetailsDAL(page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ListMatrixParametersWeightageDetailsBAL()");
                totalRecords = 0;
                return null;
            }
        }

        public bool AddMatrixDetailsBAL(string[] MatrixParams, ref string message)
        {
            try
            {
                return objProposalDAL.AddMatrixDetailsDAL(MatrixParams, ref message);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "AddMatrixDetailsBAL()");
                message = "Error occurred while processing your request.";
                return false;
            }
        }

        public Boolean SaveDistrictMappinDetails(DistrictMappingModel model, out String message)
        {
            try
            {
                return objProposalDAL.SaveDistrictMappinDetailsDAL(model, out message);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "SaveDistrictMappinDetails()");
                message = "Error occurred while processing your request.";
                return false;
            }
        }

        public Array ListMappedDistrictDetailsBAL(int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                return objProposalDAL.ListMappedDistrictDetailsDAL(page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ListMappedDistrictDetailsBAL()");
                totalRecords = 0;
                return null;
            }
        }

        public bool DeleteMappedDistrictsBAL(int districtId, out string message)
        {
            try
            {
                return objProposalDAL.DeleteMappedDistrictsDAL(districtId, out message);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DeleteMappedDistrictsBAL()");
                message = "Error occurred while processing your request.";
                return false;
            }
        }
        #endregion

        #region PMGSY3
        public string SaveRoadProposalBALPMGSY3(PMGSY.Models.Proposal.ProposalViewModelPMGSY3 ims_sanctioned_projects)
        {
            ProposalDAL objDAL = new ProposalDAL();
            IMS_SANCTIONED_PROJECTS objProposal = new IMS_SANCTIONED_PROJECTS();
            try
            {
                objProposal.MAST_PMGSY_SCHEME = PMGSYSession.Current.PMGSYScheme;// == 3 ? (byte)1 : PMGSYSession.Current.PMGSYScheme;///Changed for RCPLWE
                objProposal.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                // objProposal.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                // Changes by Saurabh Start here..
                if (PMGSYSession.Current.DistrictCode == 0)
                {
                    return "Invalid District Code";
                }
                else
                {
                    objProposal.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                }
                // Changes by Saurabh End here..

                objProposal.MAST_DPIU_CODE = PMGSYSession.Current.AdminNdCode;

                objProposal.IMS_UPGRADE_CONNECT = ims_sanctioned_projects.IMS_UPGRADE_CONNECT;

                // Below 2 new fields are added on  18 March 2021
                objProposal.EXISTING_CARRIAGEWAY_WIDTH = ims_sanctioned_projects.EXISTING_CARRIAGEWAY_WIDTH;
                objProposal.EXISTING_CARRIAGEWAY_PUC = ims_sanctioned_projects.EXISTING_CARRIAGEWAY_PUC;


                // Upgradation Proposal
                if (ims_sanctioned_projects.IMS_UPGRADE_CONNECT.ToUpper() == "U")
                {
                    objProposal.MAST_EXISTING_SURFACE_CODE = ims_sanctioned_projects.MAST_EXISTING_SURFACE_CODE;

                    // is Habitations Benefited
                    if (ims_sanctioned_projects.IMS_ISBENEFITTED_HABS.ToUpper() == "N")
                    {
                        objProposal.IMS_ISBENEFITTED_HABS = ims_sanctioned_projects.IMS_ISBENEFITTED_HABS;
                        objProposal.IMS_HABS_REASON = ims_sanctioned_projects.IMS_HABS_REASON;
                    }
                    else
                    {
                        objProposal.IMS_ISBENEFITTED_HABS = ims_sanctioned_projects.IMS_ISBENEFITTED_HABS;
                    }
                }
                else
                {
                    objProposal.MAST_EXISTING_SURFACE_CODE = null;
                    objProposal.IMS_ISBENEFITTED_HABS = "Y";
                }

                // Existing Package or New Package
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

                // Staged Proposal or Complete Proposal
                if (ims_sanctioned_projects.IMS_IS_STAGED != null && ims_sanctioned_projects.IMS_IS_STAGED.ToUpper() == "S")
                {
                    objProposal.IMS_IS_STAGED = "S";
                    //Stage I Proposal
                    if (ims_sanctioned_projects.IMS_STAGE_PHASE == "1")
                    {
                        objProposal.IMS_STAGE_PHASE = "S1";
                    }
                    if (ims_sanctioned_projects.IMS_STAGE_PHASE == "2")
                    {
                        objProposal.IMS_STAGE_PHASE = "S2";

                        //IMS_YEAR_Staged
                        var data = (from c in db.IMS_SANCTIONED_PROJECTS
                                    where
                                        c.IMS_PACKAGE_ID == ims_sanctioned_projects.Stage_2_Package_ID
                                        &&
                                        c.IMS_YEAR == ims_sanctioned_projects.Stage_2_Year
                                        &&
                                        c.PLAN_CN_ROAD_CODE == ims_sanctioned_projects.PLAN_CN_ROAD_CODE
                                        && c.IMS_STAGED_ROAD_ID == ims_sanctioned_projects.IMS_STAGED_ROAD_ID
                                    select new
                                    {
                                        Package_ID = ims_sanctioned_projects.Stage_2_Package_ID,
                                        Year = ims_sanctioned_projects.Stage_2_Year,
                                        Road_ID = c.IMS_PR_ROAD_CODE
                                    }).ToList();
                        // Self Referencing Columns
                        foreach (var RowData in data)
                        {
                            objProposal.IMS_STAGED_YEAR = RowData.Year;
                            objProposal.IMS_STAGED_PACKAGE_ID = RowData.Package_ID.ToString();
                            objProposal.IMS_STAGED_ROAD_ID = RowData.Road_ID;
                        }

                        //validation for checking the total length of stage 2 proposals should not exceed the stage 1 proposal

                        objProposal.IMS_STAGED_ROAD_ID = ims_sanctioned_projects.IMS_STAGED_ROAD_ID;
                        //if (db.IMS_SANCTIONED_PROJECTS.Any(m => m.IMS_STAGED_ROAD_ID == objProposal.IMS_STAGED_ROAD_ID && m.IMS_STAGE_PHASE == "S2"))
                        if (db.IMS_SANCTIONED_PROJECTS.Any(m => m.IMS_STAGED_ROAD_ID == ims_sanctioned_projects.IMS_STAGED_ROAD_ID && m.IMS_STAGE_PHASE == "S2"))
                        {
                            //decimal sumOfPavLength = db.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_STAGED_ROAD_ID == objProposal.IMS_STAGED_ROAD_ID && m.IMS_STAGE_PHASE == "S2").Sum(m => m.IMS_PAV_LENGTH) == null ? 0 : db.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_STAGED_ROAD_ID == objProposal.IMS_STAGED_ROAD_ID && m.IMS_STAGE_PHASE == "S2").Sum(m => m.IMS_PAV_LENGTH);

                            decimal sumOfPavLength = db.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_STAGED_ROAD_ID == ims_sanctioned_projects.IMS_STAGED_ROAD_ID && m.IMS_STAGE_PHASE == "S2").Sum(m => m.IMS_PAV_LENGTH) == null ? 0 : db.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_STAGED_ROAD_ID == ims_sanctioned_projects.IMS_STAGED_ROAD_ID && m.IMS_STAGE_PHASE == "S2").Sum(m => m.IMS_PAV_LENGTH);

                            sumOfPavLength += ims_sanctioned_projects.IMS_PAV_LENGTH.Value;

                            if (sumOfPavLength > (db.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == objProposal.IMS_STAGED_ROAD_ID.Value).Select(m => m.IMS_PAV_LENGTH).FirstOrDefault()))
                            {
                                //return "Sum of Pavement Length of Stage 2 Proposals is exceeding the Pavement Length of Stage 1 Proposal.";
                                var stage2Roads = db.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_STAGED_ROAD_ID == ims_sanctioned_projects.IMS_STAGED_ROAD_ID && m.IMS_STAGE_PHASE == "S2").Select(x => new { imsYear = x.IMS_YEAR, imsPackage = x.IMS_PACKAGE_ID, blockName = x.MASTER_BLOCK.MAST_BLOCK_NAME }).ToList();

                                string stage2Year = string.Join(",", stage2Roads[0].imsYear);
                                string stage2Package = string.Join(",", stage2Roads[0].imsPackage);
                                string stage2Block = string.Join(",", stage2Roads[0].blockName);

                                return "Sum of Pavement Length of Stage 2 Proposals is exceeding the Pavement Length of Stage 1 Proposal.<br/> Package=[" + stage2Package + "]     Year=[" + stage2Year + "]     Block=[" + stage2Block + "]";
                            }

                        }

                        //validation for checking whether the Proposal length is exceeding the core network lenth.

                        decimal? totalCnLength = 0;

                        if (PMGSYSession.Current.PMGSYScheme == 1)
                        {
                            totalCnLength = (from c in db.PLAN_ROAD
                                             where c.PLAN_CN_ROAD_CODE == ims_sanctioned_projects.PLAN_CN_ROAD_CODE
                                             select c.PLAN_RD_LENGTH).First();
                        }
                        else if (PMGSYSession.Current.PMGSYScheme == 2 || PMGSYSession.Current.PMGSYScheme == 3)///Changes for RCPLWE
                        {
                            // For scheme 3 - RCPLWE Road can be combination of roads so,
                            // For scheme 2 - Candidate Road can be combination of roads so,
                            // Length is considered as Total Length i.e. PLAN_RD_TOTAL_LEN
                            totalCnLength = (from c in db.PLAN_ROAD
                                             where c.PLAN_CN_ROAD_CODE == ims_sanctioned_projects.PLAN_CN_ROAD_CODE
                                             select c.PLAN_RD_TOTAL_LEN).First();
                        }

                        decimal? ProposedLength = (from c in db.IMS_SANCTIONED_PROJECTS
                                                   where c.PLAN_CN_ROAD_CODE == ims_sanctioned_projects.PLAN_CN_ROAD_CODE
                                                   && c.IMS_PROPOSAL_TYPE == "P"
                                                   && c.IMS_SANCTIONED != "D"
                                                   && ((c.IMS_STAGE_PHASE == null ? "1" : c.IMS_STAGE_PHASE) != (c.IMS_STAGE_PHASE == null ? "2" : "S2"))    //condition added by shyam
                                                   select (decimal?)c.IMS_PAV_LENGTH).Sum();

                        decimal? remainingLength = Convert.ToDecimal(totalCnLength) - Convert.ToDecimal(ProposedLength);

                        if (ims_sanctioned_projects.IMS_PAV_LENGTH > (totalCnLength + (totalCnLength * Convert.ToDecimal(0.5))))
                        {
                            return "variation in Proposed Length can be upto 50 % CN Length";
                        }
                    }
                }
                else // Complete Proposal
                {
                    objProposal.IMS_IS_STAGED = "C";
                }

                objProposal.IMS_YEAR = ims_sanctioned_projects.IMS_YEAR;
                objProposal.IMS_BATCH = ims_sanctioned_projects.IMS_BATCH;
                objProposal.MAST_BLOCK_CODE = ims_sanctioned_projects.MAST_BLOCK_CODE;

                // FUNDING AGENCY
                objProposal.IMS_COLLABORATION = ims_sanctioned_projects.IMS_COLLABORATION;
                // STREAM
                objProposal.IMS_STREAMS = ims_sanctioned_projects.IMS_STREAMS;
                // Link/Through Route Name
                objProposal.PLAN_CN_ROAD_CODE = ims_sanctioned_projects.PLAN_CN_ROAD_CODE != null ? Convert.ToInt32(ims_sanctioned_projects.PLAN_CN_ROAD_CODE) : 0;

                objProposal.IMS_PARTIAL_LEN = ims_sanctioned_projects.IMS_PARTIAL_LEN;


                objProposal.IMS_ROAD_FROM = ims_sanctioned_projects.IMS_ROAD_FROM;
                objProposal.IMS_ROAD_TO = ims_sanctioned_projects.IMS_ROAD_TO;
                //objProposal.IMS_ROAD_NAME = ims_sanctioned_projects.IMS_ROAD_FROM + " " + ims_sanctioned_projects.IMS_ROAD_TO;

                ///Changed by SAMMED A. PATIL for RCPLWE
                if (objProposal.IMS_COLLABORATION == 5)
                {
                    objProposal.IMS_ROAD_NAME = db.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == ims_sanctioned_projects.PLAN_CN_ROAD_CODE).Select(m => m.PLAN_RD_NAME).FirstOrDefault();
                }
                else
                {
                    //objProposal.IMS_ROAD_NAME = db.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == ims_sanctioned_projects.PLAN_CN_ROAD_CODE).Select(m => m.PLAN_CN_ROAD_NUMBER).FirstOrDefault() + "-" + db.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == ims_sanctioned_projects.PLAN_CN_ROAD_CODE).Select(m => m.PLAN_RD_NAME).FirstOrDefault();
                    var planRoad = db.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == ims_sanctioned_projects.PLAN_CN_ROAD_CODE).FirstOrDefault();
                    if ((planRoad.PLAN_CN_ROAD_NUMBER + "-" + planRoad.PLAN_RD_NAME).Length > 200)
                    {
                        objProposal.IMS_ROAD_NAME = planRoad.PLAN_RD_NAME.Trim();
                    }
                    else
                    {
                        objProposal.IMS_ROAD_NAME = planRoad.PLAN_CN_ROAD_NUMBER + "-" + planRoad.PLAN_RD_NAME.Trim();
                    }
                }

                objProposal.IMS_PAV_LENGTH = (ims_sanctioned_projects.IMS_PAV_LENGTH != null) ? Convert.ToDecimal(ims_sanctioned_projects.IMS_PAV_LENGTH) : 0;
                objProposal.IMS_NO_OF_CDWORKS = ims_sanctioned_projects.IMS_NO_OF_CDWORKS == null ? 0 : Convert.ToInt32(ims_sanctioned_projects.IMS_NO_OF_CDWORKS);
                objProposal.IMS_ZP_RESO_OBTAINED = ims_sanctioned_projects.IMS_ZP_RESO_OBTAINED;

                // All Costs Estimated ( For Reports Only, Don't Change them after Proposal has been Finalized by DPIU )
                objProposal.IMS_PAV_EST_COST = ims_sanctioned_projects.IMS_SANCTIONED_PAV_AMT;
                objProposal.IMS_CD_WORKS_EST_COST = ims_sanctioned_projects.IMS_SANCTIONED_CD_AMT;
                objProposal.IMS_PROTECTION_WORKS = ims_sanctioned_projects.IMS_SANCTIONED_PW_AMT;
                objProposal.IMS_OTHER_WORK_COST = ims_sanctioned_projects.IMS_SANCTIONED_OW_AMT;
                objProposal.IMS_STATE_SHARE = Convert.ToDecimal(ims_sanctioned_projects.IMS_STATE_SHARE);

                //PMGSY Scheme-II
                objProposal.IMS_IS_HIGHER_SPECIFICATION = ims_sanctioned_projects.IMS_IS_HIGHER_SPECIFICATION;
                if (objProposal.IMS_IS_HIGHER_SPECIFICATION != null && ims_sanctioned_projects.IMS_IS_HIGHER_SPECIFICATION.Equals("Y"))
                {
                    objProposal.IMS_HIGHER_SPECIFICATION_COST = ims_sanctioned_projects.IMS_HIGHER_SPECIFICATION_COST;
                }
                else
                {
                    objProposal.IMS_HIGHER_SPECIFICATION_COST = null;
                }
                objProposal.IMS_FURNITURE_COST = ims_sanctioned_projects.IMS_FURNITURE_COST;
                objProposal.IMS_SHARE_PERCENT = ims_sanctioned_projects.IMS_SHARE_PERCENT;

                objProposal.IMS_MAINTENANCE_YEAR1 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT1;
                objProposal.IMS_MAINTENANCE_YEAR2 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT2;
                objProposal.IMS_MAINTENANCE_YEAR3 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT3;
                objProposal.IMS_MAINTENANCE_YEAR4 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT4;
                objProposal.IMS_MAINTENANCE_YEAR5 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT5;

                //PMGSY Scheme-II
                objProposal.IMS_RENEWAL_COST = ims_sanctioned_projects.IMS_RENEWAL_COST;

                // All Costs Sanctioned ( Change this Costs iff Mord Changes Then) 
                objProposal.IMS_SANCTIONED_PAV_AMT = ims_sanctioned_projects.IMS_SANCTIONED_PAV_AMT;
                objProposal.IMS_SANCTIONED_CD_AMT = ims_sanctioned_projects.IMS_SANCTIONED_CD_AMT;
                objProposal.IMS_SANCTIONED_PW_AMT = ims_sanctioned_projects.IMS_SANCTIONED_PW_AMT;
                objProposal.IMS_SANCTIONED_OW_AMT = ims_sanctioned_projects.IMS_SANCTIONED_OW_AMT;
                objProposal.IMS_SANCTIONED_RS_AMT = Convert.ToDecimal(ims_sanctioned_projects.IMS_STATE_SHARE);
                //if (PMGSYSession.Current.PMGSYScheme == 1)
                //{
                //    objProposal.IMS_SANCTIONED_RS_AMT = Convert.ToDecimal(ims_sanctioned_projects.IMS_TOTAL_STATE_SHARE_2015);
                //}
                //else
                //{
                //    objProposal.IMS_SANCTIONED_RS_AMT = Convert.ToDecimal(ims_sanctioned_projects.IMS_STATE_SHARE);
                //}


                //PMGSY Scheme-II
                if (objProposal.IMS_IS_HIGHER_SPECIFICATION != null && ims_sanctioned_projects.IMS_IS_HIGHER_SPECIFICATION.Equals("Y"))
                {
                    objProposal.IMS_SANCTIONED_HS_AMT = ims_sanctioned_projects.IMS_HIGHER_SPECIFICATION_COST;
                }
                else
                {
                    objProposal.IMS_SANCTIONED_HS_AMT = null;
                }
                objProposal.IMS_SANCTIONED_FC_AMT = ims_sanctioned_projects.IMS_FURNITURE_COST;

                objProposal.IMS_SANCTIONED_MAN_AMT1 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT1;
                objProposal.IMS_SANCTIONED_MAN_AMT2 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT2;
                objProposal.IMS_SANCTIONED_MAN_AMT3 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT3;
                objProposal.IMS_SANCTIONED_MAN_AMT4 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT4;
                objProposal.IMS_SANCTIONED_MAN_AMT5 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT5;

                //PMGSY Scheme-II
                objProposal.IMS_SANCTIONED_RENEWAL_AMT = ims_sanctioned_projects.IMS_RENEWAL_COST;

                objProposal.MAST_MP_CONST_CODE = ims_sanctioned_projects.MAST_MP_CONST_CODE > 0 ? ims_sanctioned_projects.MAST_MP_CONST_CODE : null;
                objProposal.MAST_MLA_CONST_CODE = ims_sanctioned_projects.MAST_MLA_CONST_CODE > 0 ? ims_sanctioned_projects.MAST_MLA_CONST_CODE : null;

                // Carriage Width                
                objProposal.IMS_CARRIAGED_WIDTH = ims_sanctioned_projects.IMS_CARRIAGED_WIDTH;

                // Traffic Catagory
                objProposal.IMS_TRAFFIC_TYPE = ims_sanctioned_projects.IMS_TRAFFIC_TYPE == 0 ? null : ims_sanctioned_projects.IMS_TRAFFIC_TYPE;

                objProposal.IMS_PROPOSED_SURFACE = ims_sanctioned_projects.IMS_PROPOSED_SURFACE;
                objProposal.IMS_CC_LENGTH = ims_sanctioned_projects.IMS_CC_LENGTH;
                objProposal.IMS_BT_LENGTH = ims_sanctioned_projects.IMS_BT_LENGTH;
                objProposal.IMS_REMARKS = ims_sanctioned_projects.IMS_REMARKS;

                // Appear in case og Bridge Work
                objProposal.IMS_SANCTIONED_BW_AMT = 0;

                // not yet sanctioned by anyone
                objProposal.STA_SANCTIONED = "N";

                objProposal.IMS_SANCTIONED = "N";

                objProposal.IMS_STA_REMARKS = string.Empty;
                //Proposal Road 
                objProposal.IMS_PROPOSAL_TYPE = "P";

                // For Repackaging
                objProposal.IMS_DPR_STATUS = "N";

                // For Execution
                objProposal.IMS_FINAL_PAYMENT_FLAG = "N";

                // for Freezing 
                objProposal.IMS_FREEZE_STATUS = "U";

                // not locked
                objProposal.IMS_LOCK_STATUS = "N";

                // newly entered Proposal
                objProposal.IMS_ISCOMPLETED = "E";

                // Not Locked
                objProposal.IMS_LOCK_STATUS = "N";

                objProposal.IMS_PROG_REMARKS = string.Empty;
                objProposal.IMS_SHIFT_STATUS = "N";
                objProposal.PTA_SANCTIONED = "N";

                #region FUND_SHARING_RATIO_PMGSY_SCHEME_1

                objProposal.IMS_STATE_SHARE_2015 = ims_sanctioned_projects.IMS_STATE_SHARE_2015;
                objProposal.IMS_MORD_SHARE_2015 = ims_sanctioned_projects.IMS_MORD_SHARE_2015;
                objProposal.IMS_SHARE_PERCENT_2015 = ims_sanctioned_projects.IMS_SHARE_PERCENT_2015;

                #endregion

                //PMGSY3
                if (PMGSYSession.Current.PMGSYScheme == 4)
                {
                    objProposal.IMS_RIDING_QUALITY_LENGTH = ims_sanctioned_projects.ImsRidingQualityLength;
                    objProposal.IMS_PUCCA_SIDE_DRAINS = ims_sanctioned_projects.ImsPuccaSideDrains;
                    objProposal.IMS_GST_COST = ims_sanctioned_projects.ImsGSTCost;

                    #region For Post DPL Maintenance Cost
                    //Added by Aditi on 6 August 2020
                    objProposal.IMS_MAINTENANCE_YEAR6 = ims_sanctioned_projects.IMS_MAINTENANCE_YEAR6;
                    objProposal.IMS_MAINTENANCE_YEAR7 = ims_sanctioned_projects.IMS_MAINTENANCE_YEAR7;
                    objProposal.IMS_MAINTENANCE_YEAR8 = ims_sanctioned_projects.IMS_MAINTENANCE_YEAR8;
                    objProposal.IMS_MAINTENANCE_YEAR9 = ims_sanctioned_projects.IMS_MAINTENANCE_YEAR9;
                    objProposal.IMS_MAINTENANCE_YEAR10 = ims_sanctioned_projects.IMS_MAINTENANCE_YEAR10;
                    #endregion
                    //Added by Aditi on 10 August 2020
                    objProposal.PUCCA_SIDE_DRAIN_LENGTH = ims_sanctioned_projects.PUCCA_SIDE_DRAIN_LENGTH;
                    objProposal.PROTECTION_LENGTH = ims_sanctioned_projects.PROTECTION_LENGTH;

                    #region Existing Surface Details
                    objProposal.SURFACE_BRICK_SOLLING = ims_sanctioned_projects.SURFACE_BRICK_SOLLING;
                    objProposal.SURFACE_BT = ims_sanctioned_projects.SURFACE_BT;
                    objProposal.SURFACE_CC = ims_sanctioned_projects.SURFACE_CC;
                    objProposal.SURFACE_GRAVEL = ims_sanctioned_projects.SURFACE_GRAVEL;
                    objProposal.SURFACE_MOORUM = ims_sanctioned_projects.SURFACE_MOORUM;
                    objProposal.SURFACE_TRACK = ims_sanctioned_projects.SURFACE_TRACK;
                    objProposal.SURFACE_WBM = ims_sanctioned_projects.SURFACE_WBM;

                    decimal TotalSurfaceLength = Convert.ToDecimal(objProposal.SURFACE_BRICK_SOLLING + objProposal.SURFACE_BT + objProposal.SURFACE_CC + objProposal.SURFACE_GRAVEL + objProposal.SURFACE_MOORUM + objProposal.SURFACE_TRACK + objProposal.SURFACE_WBM);

                    if (ims_sanctioned_projects.IMS_PAV_LENGTH != TotalSurfaceLength)
                    {
                        return "Sum of all the Existing Surface Lengths should be equal to the Pavement Length";
                    }
                    #endregion
                }

                return objDAL.SaveRoadProposalPMGSY3DAL(objProposal, ims_sanctioned_projects);
                //return objProposalDAL.SaveRoadProposalDAL(objProposal);
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "SaveRoadProposalBAL()");
                return "An Error Occurred While Your Processing Request.";
            }
            finally
            {
                db.Dispose();
            }
        }

        public string DeleteRoadProposalPMGSY3BAL(int IMS_PR_ROAD_CODE)
        {
            ProposalDAL objDAL = new ProposalDAL();
            try
            {
                db = new PMGSYEntities();

                // If Stage 2 Proposal, then allow to delete directly
                // First delete all dependent records, then delete Proposal
                if (db.IMS_SANCTIONED_PROJECTS.Where(c => c.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Select(c => c.IMS_STAGE_PHASE).First() == "S2")
                {
                    //new condition added by Vikram as suggested by Dev Sir on 02 July 2014 . if road is used as stage 1 road then it can not be deleted.
                    if (db.IMS_SANCTIONED_PROJECTS.Any(c => c.IMS_STAGED_ROAD_ID == IMS_PR_ROAD_CODE))
                    {
                        return "Staged construction is present against this Proposal, so Proposal can not be deleted.";
                    }

                    if (db.IMS_BENEFITED_HABS.Where(c => c.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Count() > 0)
                    {
                        db.Database.ExecuteSqlCommand("DELETE [omms].IMS_BENEFITED_HABS Where IMS_PR_ROAD_CODE = {0}", IMS_PR_ROAD_CODE);
                    }

                    if (db.IMS_TRAFFIC_INTENSITY.Where(c => c.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Count() > 0)
                    {
                        db.Database.ExecuteSqlCommand("DELETE [omms].IMS_TRAFFIC_INTENSITY Where IMS_PR_ROAD_CODE = {0}", IMS_PR_ROAD_CODE);
                    }

                    if (db.IMS_CBR_VALUE.Where(c => c.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Count() > 0)
                    {
                        db.Database.ExecuteSqlCommand("DELETE [omms].IMS_CBR_VALUE Where IMS_PR_ROAD_CODE = {0}", IMS_PR_ROAD_CODE);
                    }

                    return objDAL.DeleteRoadProposalPMGSY3DAL(IMS_PR_ROAD_CODE);
                }
                else
                {
                    if (db.IMS_BENEFITED_HABS.Where(c => c.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Count() > 0)
                        return "Habitation Details are added against Proposal, Proposal can not be deleted.";
                    if (db.IMS_TRAFFIC_INTENSITY.Where(c => c.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Count() > 0)
                        return "Traffic Intensity Details are added against Proposal, Proposal can not be deleted.";
                    if (db.IMS_CBR_VALUE.Where(c => c.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).Count() > 0)
                        return "CBR Values are added against Proposal, Proposal can not be deleted.";
                    if (db.IMS_SANCTIONED_PROJECTS.Where(c => c.IMS_STAGED_ROAD_ID == IMS_PR_ROAD_CODE && c.IMS_PROPOSAL_TYPE == "L").Count() > 0)
                        return "Bridge proposal is added against selected road Proposal, Proposal can not be deleted.";
                    else
                        return objDAL.DeleteRoadProposalPMGSY3DAL(IMS_PR_ROAD_CODE);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DeleteRoadProposalPMGSY3BAL()");
                return "An Error Occurred While Your Processing Request.";
            }
            finally
            {
                db.Dispose();
            }
        }
        #endregion

        #region Proposal Shifting
        public Array GetProposalsForITNOForShiftingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int MAST_STATE_CODE, int MAST_DISTRICT_CODE, int IMS_YEAR, int IMS_BATCH, int IMS_STREAMS, String ProposalType, string IMS_PROPOSAL_STATUS, string IMS_UPGRADE_CONNECT, int MAST_BLOCK_CODE, string Filters, out ProposalColumnsTotal totalColModel)
        {
            return objProposalDAL.GetProposalsForITNOForShiftingDAL(page, rows, sidx, sord, out totalRecords, MAST_STATE_CODE, MAST_DISTRICT_CODE, IMS_YEAR, IMS_BATCH, IMS_STREAMS, ProposalType, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT, MAST_BLOCK_CODE, Filters, out totalColModel);
        }


        public Array GetLSBBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int MAST_STATE_CODE, int MAST_DISTRICT_CODE, int IMS_YEAR, int IMS_BATCH, int IMS_STREAMS, String ProposalType, string IMS_PROPOSAL_STATUS, string IMS_UPGRADE_CONNECT, int MAST_BLOCK_CODE, out ProposalColumnsTotal model)
        {
            return objProposalDAL.GetLSBDAL(page, rows, sidx, sord, out totalRecords, MAST_STATE_CODE, MAST_DISTRICT_CODE, IMS_YEAR, IMS_BATCH, IMS_STREAMS, ProposalType, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT, MAST_BLOCK_CODE, out model);
        }
        #endregion

        #region Proposals for PMGSY Scheme 5 (Vibrant Village) - Srishti Tyagi 27/06/2023

        public Array GetProposalsVibrantVillageBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int MAST_STATE_CODE, int MAST_DISTRICT_CODE, int IMS_YEAR, int MAST_BLOCK_CODE, int IMS_BATCH, int IMS_STREAMS, String ProposalType, int MAST_DPIU_CODE, string IMS_PROPOSAL_STATUS, string IMS_UPGRADE_CONNECT, string Filters, out ProposalColumnsTotal colTotal)
        {
            return objProposalDAL.GetProposalsVibrantVillageDAL(page, rows, sidx, sord, out totalRecords, MAST_STATE_CODE, MAST_DISTRICT_CODE, IMS_YEAR, MAST_BLOCK_CODE, IMS_BATCH, IMS_STREAMS, ProposalType, MAST_DPIU_CODE, IMS_PROPOSAL_STATUS, IMS_UPGRADE_CONNECT, Filters, out colTotal);
        }

        public string SaveRoadProposalPMGSY5BAL(PMGSY.Models.Proposal.ProposalViewModel ims_sanctioned_projects)
        {
            IMS_SANCTIONED_PROJECTS objProposal = new IMS_SANCTIONED_PROJECTS();
            try
            {
                // Basic Details
                objProposal.MAST_PMGSY_SCHEME = PMGSYSession.Current.PMGSYScheme;
                objProposal.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                objProposal.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                objProposal.MAST_DPIU_CODE = PMGSYSession.Current.AdminNdCode;

                #region Construction Type

                // New Connectivity
                objProposal.IMS_UPGRADE_CONNECT = ims_sanctioned_projects.IMS_UPGRADE_CONNECT;

                // Complete Construction or Staged
                if (ims_sanctioned_projects.IMS_IS_STAGED != null && ims_sanctioned_projects.IMS_IS_STAGED.ToUpper() == "S")
                {
                    objProposal.IMS_IS_STAGED = "S";

                    //Stage I
                    if (ims_sanctioned_projects.IMS_STAGE_PHASE == "1")
                    {
                        objProposal.IMS_STAGE_PHASE = "S1";
                    }

                    // Stage II
                    if (ims_sanctioned_projects.IMS_STAGE_PHASE == "2")
                    {
                        objProposal.IMS_STAGE_PHASE = "S2";

                        //Stage II Year 
                        var data = (from c in db.IMS_SANCTIONED_PROJECTS
                                    where
                                        c.IMS_PACKAGE_ID == ims_sanctioned_projects.Stage_2_Package_ID
                                        && c.IMS_YEAR == ims_sanctioned_projects.Stage_2_Year
                                        && c.PLAN_CN_ROAD_CODE == ims_sanctioned_projects.PLAN_CN_ROAD_CODE
                                        && c.IMS_STAGED_ROAD_ID == ims_sanctioned_projects.IMS_STAGED_ROAD_ID
                                    select new
                                    {
                                        Package_ID = ims_sanctioned_projects.Stage_2_Package_ID,
                                        Year = ims_sanctioned_projects.Stage_2_Year,
                                        Road_ID = c.IMS_PR_ROAD_CODE
                                    }).ToList();
                        // Self Referencing Columns
                        foreach (var RowData in data)
                        {
                            objProposal.IMS_STAGED_YEAR = RowData.Year;
                            objProposal.IMS_STAGED_PACKAGE_ID = RowData.Package_ID.ToString();
                            objProposal.IMS_STAGED_ROAD_ID = RowData.Road_ID;
                        }

                        //validation for checking the total length of stage 2 proposals should not exceed the stage 1 proposal
                        objProposal.IMS_STAGED_ROAD_ID = ims_sanctioned_projects.IMS_STAGED_ROAD_ID;

                        if (db.IMS_SANCTIONED_PROJECTS.Any(m => m.IMS_STAGED_ROAD_ID == ims_sanctioned_projects.IMS_STAGED_ROAD_ID && m.IMS_STAGE_PHASE == "S2"))
                        {
                            decimal sumOfPavLength = db.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_STAGED_ROAD_ID == ims_sanctioned_projects.IMS_STAGED_ROAD_ID && m.IMS_STAGE_PHASE == "S2").Sum(m => m.IMS_PAV_LENGTH) == null ? 0 : db.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_STAGED_ROAD_ID == ims_sanctioned_projects.IMS_STAGED_ROAD_ID && m.IMS_STAGE_PHASE == "S2").Sum(m => m.IMS_PAV_LENGTH);

                            sumOfPavLength += ims_sanctioned_projects.IMS_PAV_LENGTH.Value;

                            if (sumOfPavLength > (db.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == objProposal.IMS_STAGED_ROAD_ID.Value).Select(m => m.IMS_PAV_LENGTH).FirstOrDefault()))
                            {
                                var stage2Roads = db.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_STAGED_ROAD_ID == ims_sanctioned_projects.IMS_STAGED_ROAD_ID && m.IMS_STAGE_PHASE == "S2").Select(x => new { imsYear = x.IMS_YEAR, imsPackage = x.IMS_PACKAGE_ID, blockName = x.MASTER_BLOCK.MAST_BLOCK_NAME }).ToList();

                                string stage2Year = string.Join(",", stage2Roads[0].imsYear);
                                string stage2Package = string.Join(",", stage2Roads[0].imsPackage);
                                string stage2Block = string.Join(",", stage2Roads[0].blockName);

                                return "Sum of Pavement Length of Stage 2 Proposals is exceeding the Pavement Length of Stage 1 Proposal.<br/> Package=[" + stage2Package + "]     Year=[" + stage2Year + "]     Block=[" + stage2Block + "]";
                            }
                        }

                        //validation for checking whether the Proposal length is exceeding the core network length.
                        decimal? totalCnLength = 0;

                        totalCnLength = (from c in db.PLAN_ROAD
                                         where c.PLAN_CN_ROAD_CODE == ims_sanctioned_projects.PLAN_CN_ROAD_CODE
                                         select c.PLAN_RD_TOTAL_LEN).First();

                        decimal? ProposedLength = (from c in db.IMS_SANCTIONED_PROJECTS
                                                   where c.PLAN_CN_ROAD_CODE == ims_sanctioned_projects.PLAN_CN_ROAD_CODE
                                                   && c.IMS_PROPOSAL_TYPE == "P"
                                                   && c.IMS_SANCTIONED != "D"
                                                   && ((c.IMS_STAGE_PHASE == null ? "1" : c.IMS_STAGE_PHASE) != (c.IMS_STAGE_PHASE == null ? "2" : "S2"))
                                                   select (decimal?)c.IMS_PAV_LENGTH).Sum();

                        decimal? remainingLength = Convert.ToDecimal(totalCnLength) - Convert.ToDecimal(ProposedLength);

                        if (ims_sanctioned_projects.IMS_PAV_LENGTH > (totalCnLength + (totalCnLength * Convert.ToDecimal(0.5))))
                        {
                            return "variation in Proposed Length can be upto 50 % CN Length";
                        }
                    }
                }
                else // Complete Construction
                {
                    objProposal.IMS_IS_STAGED = "C";
                }

                objProposal.MAST_EXISTING_SURFACE_CODE = ims_sanctioned_projects.MAST_EXISTING_SURFACE_CODE;
                objProposal.IMS_YEAR = ims_sanctioned_projects.IMS_YEAR;
                objProposal.IMS_BATCH = ims_sanctioned_projects.IMS_BATCH;
                objProposal.MAST_BLOCK_CODE = ims_sanctioned_projects.MAST_BLOCK_CODE;
                objProposal.IMS_COLLABORATION = ims_sanctioned_projects.IMS_COLLABORATION;
                objProposal.IMS_STREAMS = ims_sanctioned_projects.IMS_STREAMS;

                // New Package or Exising Package
                objProposal.IMS_EXISTING_PACKAGE = ims_sanctioned_projects.IMS_EXISTING_PACKAGE;

                if (ims_sanctioned_projects.IMS_EXISTING_PACKAGE.ToUpper() == "N")
                {
                    objProposal.IMS_PACKAGE_ID = ims_sanctioned_projects.PACKAGE_PREFIX + ims_sanctioned_projects.IMS_PACKAGE_ID;
                }
                else
                {
                    objProposal.IMS_PACKAGE_ID = ims_sanctioned_projects.EXISTING_IMS_PACKAGE_ID;
                }

                // TR/MRL
                objProposal.PLAN_CN_ROAD_CODE = ims_sanctioned_projects.PLAN_CN_ROAD_CODE != null ? Convert.ToInt32(ims_sanctioned_projects.PLAN_CN_ROAD_CODE) : 0;

                // Habitations Benefited
                if (ims_sanctioned_projects.IMS_ISBENEFITTED_HABS.ToUpper() == "N")
                {
                    objProposal.IMS_ISBENEFITTED_HABS = ims_sanctioned_projects.IMS_ISBENEFITTED_HABS;
                    objProposal.IMS_HABS_REASON = ims_sanctioned_projects.IMS_HABS_REASON;
                }
                else
                {
                    objProposal.IMS_ISBENEFITTED_HABS = ims_sanctioned_projects.IMS_ISBENEFITTED_HABS;
                }

                objProposal.IMS_ROAD_FROM = ims_sanctioned_projects.IMS_ROAD_FROM;
                objProposal.IMS_ROAD_TO = ims_sanctioned_projects.IMS_ROAD_TO;

                #endregion

                #region Existing Surface Details

                objProposal.SURFACE_BRICK_SOLLING = ims_sanctioned_projects.SURFACE_BRICK_SOLLING;
                objProposal.SURFACE_BT = ims_sanctioned_projects.SURFACE_BT;
                objProposal.SURFACE_CC = ims_sanctioned_projects.SURFACE_CC;
                objProposal.SURFACE_GRAVEL = ims_sanctioned_projects.SURFACE_GRAVEL;
                objProposal.SURFACE_MOORUM = ims_sanctioned_projects.SURFACE_MOORUM;
                objProposal.SURFACE_TRACK = ims_sanctioned_projects.SURFACE_TRACK;
                objProposal.SURFACE_WBM = ims_sanctioned_projects.SURFACE_WBM;

                decimal TotalSurfaceLength = Convert.ToDecimal(objProposal.SURFACE_BRICK_SOLLING + objProposal.SURFACE_BT + objProposal.SURFACE_CC + objProposal.SURFACE_GRAVEL + objProposal.SURFACE_MOORUM + objProposal.SURFACE_TRACK + objProposal.SURFACE_WBM);

                if (ims_sanctioned_projects.IMS_PAV_LENGTH != TotalSurfaceLength)
                {
                    return "Sum of all the Existing Surface Lengths should be equal to the Pavement Length";
                }

                #endregion

                #region Technical Details

                objProposal.IMS_CC_LENGTH = ims_sanctioned_projects.IMS_CC_LENGTH;
                objProposal.IMS_BT_LENGTH = ims_sanctioned_projects.IMS_BT_LENGTH;
                objProposal.IMS_IS_HIGHER_SPECIFICATION = ims_sanctioned_projects.IMS_IS_HIGHER_SPECIFICATION;

                if (objProposal.IMS_IS_HIGHER_SPECIFICATION != null && ims_sanctioned_projects.IMS_IS_HIGHER_SPECIFICATION.Equals("Y"))
                {
                    objProposal.IMS_HIGHER_SPECIFICATION_COST = ims_sanctioned_projects.IMS_HIGHER_SPECIFICATION_COST;
                }
                else
                {
                    objProposal.IMS_HIGHER_SPECIFICATION_COST = null;
                }

                objProposal.IMS_RIDING_QUALITY_LENGTH = ims_sanctioned_projects.ImsRidingQualityLength;

                #endregion

                #region Pavement Cost Details (Including GST)

                objProposal.IMS_PAV_EST_COST = ims_sanctioned_projects.IMS_SANCTIONED_PAV_AMT;
                objProposal.IMS_SANCTIONED_PAV_AMT = ims_sanctioned_projects.IMS_SANCTIONED_PAV_AMT;

                #endregion

                #region Other Cost Details (Including GST)

                objProposal.IMS_PUCCA_SIDE_DRAINS = ims_sanctioned_projects.ImsPuccaSideDrains;
                objProposal.PUCCA_SIDE_DRAIN_LENGTH = ims_sanctioned_projects.PUCCA_SIDE_DRAIN_LENGTH;
                objProposal.IMS_NO_OF_CDWORKS = ims_sanctioned_projects.IMS_NO_OF_CDWORKS == null ? 0 : Convert.ToInt32(ims_sanctioned_projects.IMS_NO_OF_CDWORKS);

                objProposal.IMS_CD_WORKS_EST_COST = ims_sanctioned_projects.IMS_SANCTIONED_CD_AMT;
                objProposal.IMS_SANCTIONED_CD_AMT = ims_sanctioned_projects.IMS_SANCTIONED_CD_AMT;

                objProposal.IMS_PROTECTION_WORKS = ims_sanctioned_projects.IMS_SANCTIONED_PW_AMT;
                objProposal.IMS_SANCTIONED_PW_AMT = ims_sanctioned_projects.IMS_SANCTIONED_PW_AMT;

                objProposal.PROTECTION_LENGTH = ims_sanctioned_projects.PROTECTION_LENGTH;

                objProposal.IMS_OTHER_WORK_COST = ims_sanctioned_projects.IMS_SANCTIONED_OW_AMT;
                objProposal.IMS_SANCTIONED_OW_AMT = ims_sanctioned_projects.IMS_SANCTIONED_OW_AMT;

                objProposal.IMS_FURNITURE_COST = ims_sanctioned_projects.IMS_FURNITURE_COST;
                objProposal.IMS_SANCTIONED_FC_AMT = ims_sanctioned_projects.IMS_FURNITURE_COST;

                #endregion

                #region Fund Sharing Ratio

                objProposal.IMS_SHARE_PERCENT_2015 = ims_sanctioned_projects.IMS_SHARE_PERCENT_2015;
                objProposal.IMS_STATE_SHARE_2015 = ims_sanctioned_projects.IMS_STATE_SHARE_2015;
                objProposal.IMS_MORD_SHARE_2015 = ims_sanctioned_projects.IMS_MORD_SHARE_2015;
                objProposal.IMS_SANCTIONED_RS_AMT = Convert.ToDecimal(ims_sanctioned_projects.IMS_TOTAL_STATE_SHARE_2015);   //???
                objProposal.IMS_ZP_RESO_OBTAINED = ims_sanctioned_projects.IMS_ZP_RESO_OBTAINED;
                objProposal.MAST_MP_CONST_CODE = ims_sanctioned_projects.MAST_MP_CONST_CODE > 0 ? ims_sanctioned_projects.MAST_MP_CONST_CODE : null;
                objProposal.IMS_GST_COST = ims_sanctioned_projects.ImsGSTCost;

                #endregion

                #region Existing Carriageway Details

                objProposal.EXISTING_CARRIAGEWAY_WIDTH = ims_sanctioned_projects.EXISTING_CARRIAGEWAY_WIDTH;
                objProposal.EXISTING_CARRIAGEWAY_PUC = ims_sanctioned_projects.EXISTING_CARRIAGEWAY_PUC;

                #endregion

                #region Design Details

                objProposal.IMS_CARRIAGED_WIDTH = ims_sanctioned_projects.IMS_CARRIAGED_WIDTH;
                objProposal.IMS_TRAFFIC_TYPE = ims_sanctioned_projects.IMS_TRAFFIC_TYPE == 0 ? null : ims_sanctioned_projects.IMS_TRAFFIC_TYPE;
                objProposal.IMS_PROPOSED_SURFACE = ims_sanctioned_projects.IMS_PROPOSED_SURFACE;

                #endregion

                #region Maintenance Cost

                objProposal.IMS_MAINTENANCE_YEAR1 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT1;
                objProposal.IMS_MAINTENANCE_YEAR2 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT2;
                objProposal.IMS_MAINTENANCE_YEAR3 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT3;
                objProposal.IMS_MAINTENANCE_YEAR4 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT4;
                objProposal.IMS_MAINTENANCE_YEAR5 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT5;
                objProposal.IMS_RENEWAL_COST = ims_sanctioned_projects.IMS_RENEWAL_COST;

                #endregion

                #region Post DLP Maintenance Cost

                objProposal.IMS_MAINTENANCE_YEAR6 = ims_sanctioned_projects.IMS_MAINTENANCE_YEAR6;
                objProposal.IMS_MAINTENANCE_YEAR7 = ims_sanctioned_projects.IMS_MAINTENANCE_YEAR7;
                objProposal.IMS_MAINTENANCE_YEAR8 = ims_sanctioned_projects.IMS_MAINTENANCE_YEAR8;
                objProposal.IMS_MAINTENANCE_YEAR9 = ims_sanctioned_projects.IMS_MAINTENANCE_YEAR9;
                objProposal.IMS_MAINTENANCE_YEAR10 = ims_sanctioned_projects.IMS_MAINTENANCE_YEAR10;
                objProposal.IMS_REMARKS = ims_sanctioned_projects.IMS_REMARKS;

                #endregion

                #region Common

                objProposal.IMS_PROPOSAL_TYPE = "P";

                if (objProposal.IMS_COLLABORATION == 5)
                {
                    objProposal.IMS_ROAD_NAME = db.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == ims_sanctioned_projects.PLAN_CN_ROAD_CODE).Select(m => m.PLAN_RD_NAME).FirstOrDefault();
                }
                else
                {
                    objProposal.IMS_ROAD_NAME = db.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == ims_sanctioned_projects.PLAN_CN_ROAD_CODE).Select(m => m.PLAN_CN_ROAD_NUMBER).FirstOrDefault() + "-" + db.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == ims_sanctioned_projects.PLAN_CN_ROAD_CODE).Select(m => m.PLAN_RD_NAME).FirstOrDefault();
                }

                objProposal.IMS_PARTIAL_LEN = ims_sanctioned_projects.IMS_PARTIAL_LEN;
                objProposal.IMS_PAV_LENGTH = (ims_sanctioned_projects.IMS_PAV_LENGTH != null) ? Convert.ToDecimal(ims_sanctioned_projects.IMS_PAV_LENGTH) : 0;
                objProposal.IMS_SHARE_PERCENT = ims_sanctioned_projects.IMS_SHARE_PERCENT;
                objProposal.IMS_STATE_SHARE = Convert.ToDecimal(ims_sanctioned_projects.IMS_STATE_SHARE);
                objProposal.MAST_MLA_CONST_CODE = ims_sanctioned_projects.MAST_MLA_CONST_CODE > 0 ? ims_sanctioned_projects.MAST_MLA_CONST_CODE : null; //???
                objProposal.IMS_DPR_STATUS = "N";
                objProposal.STA_SANCTIONED = "N";
                objProposal.IMS_STA_REMARKS = string.Empty;
                objProposal.PTA_SANCTIONED = "N";
                objProposal.IMS_SANCTIONED = "N";

                if (objProposal.IMS_IS_HIGHER_SPECIFICATION != null && ims_sanctioned_projects.IMS_IS_HIGHER_SPECIFICATION.Equals("Y"))
                {
                    objProposal.IMS_SANCTIONED_HS_AMT = ims_sanctioned_projects.IMS_HIGHER_SPECIFICATION_COST;
                }
                else
                {
                    objProposal.IMS_SANCTIONED_HS_AMT = null;
                }

                objProposal.IMS_SANCTIONED_BW_AMT = 0;
                objProposal.IMS_SANCTIONED_MAN_AMT1 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT1;
                objProposal.IMS_SANCTIONED_MAN_AMT2 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT2;
                objProposal.IMS_SANCTIONED_MAN_AMT3 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT3;
                objProposal.IMS_SANCTIONED_MAN_AMT4 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT4;
                objProposal.IMS_SANCTIONED_MAN_AMT5 = ims_sanctioned_projects.IMS_SANCTIONED_MAN_AMT5;
                objProposal.IMS_SANCTIONED_RENEWAL_AMT = ims_sanctioned_projects.IMS_RENEWAL_COST;
                objProposal.IMS_PROG_REMARKS = string.Empty;
                objProposal.IMS_FINAL_PAYMENT_FLAG = "N";
                objProposal.IMS_ISCOMPLETED = "E";
                objProposal.IMS_LOCK_STATUS = "N";
                objProposal.IMS_FREEZE_STATUS = "U";
                objProposal.IMS_SHIFT_STATUS = "N";

                #endregion

                return objProposalDAL.SaveRoadProposalPMGSY5DAL(objProposal, ims_sanctioned_projects);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "SaveRoadProposalPMGSY5BAL()");
                return "An Error Occurred While Your Processing Request.";
            }
            finally
            {
                db.Dispose();
            }
        }

        #endregion
    }

    public interface IProposalBAL
    {
        bool checkIsPaymentBAL(int prRoadCode);

        #region Road Proposal Data Entry
        Array GetProposalsBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int MAST_STATE_CODE, int MAST_DISTRICT_CODE, int IMS_YEAR, int MAST_BLOCK_CODE, int IMS_BATCH, int IMS_STREAMS, String ProposalType, int MAST_DPIU_CODE, string IMS_PROPOSAL_STATUS, string IMS_UPGRADE_CONNECT, string filters, out ProposalColumnsTotal colTotal);
        Array GetProposalsForSRRDABAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int MAST_STATE_CODE, int MAST_DISTRICT_CODE, int IMS_YEAR, int IMS_BATCH, int IMS_STREAMS, String ProposalType, string IMS_PROPOSAL_STATUS, string IMS_UPGRADE_CONNECT, int MAST_BLOCK_CODE, string filters, out ProposalColumnsTotal totalColModel);
        string SaveRoadProposalBAL(PMGSY.Models.Proposal.ProposalViewModel ims_sanctioned_projects);
        string UpdateRoadProposalBAL(PMGSY.Models.Proposal.ProposalViewModel ims_sanctioned_projects);
        string DPIUFinalizeProposalBAL(int IMS_PR_ROAD_CODE);
        string IsProposalDeletedBAL(int IMS_PR_ROAD_CODE);
        string DeleteRoadProposalBAL(int IMS_PR_ROAD_CODE);
        string GetProposalChecksBAL(int IMS_PR_ROAD_CODE, string IMS_LOCK_STATUS);
        string GetRoadDetailsBAL(int PLAN_CN_ROAD_CODE, bool isStageTwoProposal, int IMS_PR_ROAD_CODE, int IMS_STAGED_ROAD_ID);
        bool IsPavementLengthValidBAL(decimal IMS_PAV_LEN, decimal IMS_CC_LEN, decimal IMS_BT_LEN, string IMS_UPGRADE_CONNECT, decimal DUP_IMS_PAV_LENGTH, string OperationType, int IMS_STAGE_PHASE, decimal IMS_STAGE1_PAV_LENGTH);
        string IsProposalEditedBAL(int IMS_PR_ROAD_CODE, bool IMS_TRAFFIC_TYPE);
        string GetRoadDetailsForUpdate(int IMS_PR_ROAD_CODE, int PLAN_CN_ROAD_CODE);
        string GetRoadDetailsForStageTwoProposalBAL(int IMS_PR_ROAD_CODE, int PLAN_CN_ROAD_CODE);
        List<SelectListItem> PopulateExistingPackage(int Year, int BatchID);
        string IsHabitationsBenifitted(int IMS_PR_ROAD_CODE);
        string isProposalLengthTypeValid(int PLAN_CN_ROAD_CODE, decimal IMS_PAV_LEN, int IMS_PR_ROAD_CODE, string ProposalLengthType, string IMS_STAGE_PHASE);
        #endregion

        #region UnlockedProposal
        UnlockProposalViewModel GetPropsoalDetailsBAL(int IMS_PR_ROAD_CODE);
        string UpdateUnlockedProposalBAL(UnlockProposalViewModel proposalViewModel);
        string DPIUFinalizeUnlockedProposalBAL(int IMS_PR_ROAD_CODE);
        #endregion

        #region Habitataion and Cluster
        Array GetHabitationListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int IMS_PR_ROAD_CODE);
        string AddHabitationDetailBAL(HabitationViewModel habModel);
        int CreateClusterBAL(string[] clusterArray, int IMS_PR_ROAD_CODE);
        string UpdateClusterBAL(string[] HabitationArray, string[] ClusterArray, int IMS_PR_ROAD_CODE);
        int UnMapHabitationBAL(string IMS_PR_ROAD_CODE, string IMS_HAB_CODE);
        string FinalizeHabitationBAL(HabitationViewModel habitationViewModel);
        string AddHabitationClusterDetailBAL(HabitationClusterViewModel habModel);
        int UnMapHabitationClusterBAL(string IMS_PR_ROAD_CODE, string IMS_HAB_CODE, string IMS_CLUSTER_CODE);
        #endregion

        #region Traffic Intensity
        Array GetTrafficListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int IMS_PR_ROAD_CODE);
        int SaveTrafficIntesityBAL(TrafficViewModel traffic_intensity);
        int UpdateTrafficIntesityBAL(TrafficViewModel ims_traffic_intensity);
        int DeleteTrafficIntensityDetailsBAL(TrafficViewModel ims_traffic_intensity);
        #endregion

        #region CBR Details
        Array GetCBRListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int IMS_PR_ROAD_CODE);
        string SaveCBRValueBAL(CBRViewModel CBRModel);
        string UpdateCBRValueBAL(CBRViewModel CBRModel);
        string DeleteCBRValueBAL(CBRViewModel CBRModel);
        #endregion

        #region Upload File Details
        Array GetFilesListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int IMS_PR_ROAD_CODE);
        string AddFileUploadDetailsBAL(List<FileUploadViewModel> lstFileUploadViewModel, string ISPF_TYPE);
        string DeleteFileDetails(int IMS_FILE_ID, int IMS_PR_ROAD_CODE, string IMS_FILE_NAME, string ISPF_TYPE);
        Array GetPDFFilesListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int IMS_PR_ROAD_CODE);
        Array GetSTASRRDAPDFFilesListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int IMS_PR_ROAD_CODE);
        string UpdateImageDetailsBAL(FileUploadViewModel fileuploadViewModel);
        string UpdatePDFDetailsBAL(FileUploadViewModel fileuploadViewModel);
        string ValidatePDFFile(int FileSize, string FileExtension);


        Array GetJIFilesListBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int IMS_PR_ROAD_CODE);
        string UpdateJIDetailsBAL(FileUploadViewModel fileuploadViewModel);
        #endregion

        #region STA Region
        Array GetSTAProposalsBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int MAST_STATE_CODE, int IMS_YEAR, int MAST_DISTRICT_ID, int IMS_BATCH, int IMS_STREAMS, string IMS_PROPOSAL_TYPE, string IMS_PROPOSAL_STATUS);
        string StaFinalizeProposalBAL(StaSanctionViewModel staSanctionViewModel, String ProposalStatus);
        #endregion

        #region PTA
        string PtaFinalizeProposalBAL(PtaSanctionViewModel ptaSanctionViewModel, String ProposalStatus);
        #endregion

        #region Mord Region
        Array GetMordProposalsBAL(int page, int rows, string sidx, string sord, out int totalRecords, int IMS_YEAR, int MAST_STATE_ID, int MAST_DISTRICT_ID, int IMS_BATCH, int IMS_STREAMS, string IMS_PROPOSAL_TYPE, string IMS_PROPOSAL_STATUS, int IMS_AGENCY, string IMS_UPGRADE_CONNECT, out ProposalColumnsTotal totalColModel);
        string UpdateMordSanctionDetailsBAL(MordSanctionViewModel mordSanctionViewModel);
        List<IMS_GET_ACTIONS_FOR_MORD_Result> GetMordActions(int IMS_PR_ROAD_CODE);
        MordSanctionViewModel GetBulkMordDetailBAL(string IMS_PR_ROAD_CODES);
        string BulkMordDetailBAL(MordSanctionViewModel mordSanctionViewModel);
        #endregion

        #region  Habitation Finalization
        Array GetMordProposalsforHabFinalizationBAL(int page, int rows, string sidx, string sord, out int totalRecords, int IMS_YEAR, int MAST_STATE_ID, int MAST_DISTRICT_ID, int IMS_BATCH, int IMS_STREAMS, string IMS_PROPOSAL_TYPE, string IMS_PROPOSAL_STATUS, int IMS_AGENCY, string IMS_UPGRADE_CONNECT);
        bool DefinalizeHabitationBAL(int proposalCode);
        #endregion

        #region REVISION


        RevisedCostLengthViewModel GetOldRevisedCostLengthBAL(int proposalCode);
        Array GetRevisedCostLengthListBAL(int? page, int? rows, string sidx, string sord, out long totalRecords, int proposalCode);
        bool AddRevisedCostLengthBAL(RevisedCostLengthViewModel model, ref string message);
        RevisedCostLengthViewModel GetRevisionDetailsBAL(int proposalCode, int revisionCode);
        Array GetRevisionBridgeListBAL(int? page, int? rows, string sidx, string sord, out long totalRecords, int proposalCode);
        bool EditRevisionDetailsBAL(RevisedCostLengthViewModel model, ref string message);


        #endregion

        #region TECHNOLOGY

        bool AddTechnologyDetailsBAL(TechnologyDetailsViewModel model, ref string message);
        bool EditTechnologyDetailsBAL(TechnologyDetailsViewModel model, ref string message);
        Array GetTechnologyDetailsListBAL(int? page, int? rows, string sidx, string sord, out long totalRecords, int proposalCode);
        TechnologyDetailsViewModel GetTechnologyDetails(int proposalCode, int segmentCode);
        bool DeleteTechnologyDetails(int proposalCode, int segmentCode);
        decimal? GetTechnologyStartChainage(int proposalCode, int techCode, int layerCode);

        #endregion

        #region Test Result Details

        //login STA
        Array TestResultDetails(int IMS_PR_ROAD_CODE, int? page, int? rows, string sidx, string sord, out long totalRecords);

        Array TestResultSampleDetails(int IMS_PR_ROAD_CODE, int? page, int? rows, string sidx, string sord, out long totalRecords);

        bool AddTestResultDetails(TestResultViewModel testResultViewModel, ref string message);

        TestResultViewModel EditTestResultDetails(int ResultCode, int ImsPrRoadCode);

        bool UpdateTestResultDetails(TestResultViewModel testResultViewModel, ref string message);

        bool DeleteTestResultDetails(int resultCode, int imsPrRoadCode, ref string message);


        //login ITNO

        Array GetItnoProposalsBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int MAST_STATE_CODE, int IMS_YEAR, int MAST_DISTRICT_ID, int IMS_BATCH, int IMS_STREAMS);

        IMS_SANCTIONED_PROJECTS GetRoadDetails(int IMS_PR_ROAD_CODE);
        #endregion

        #region SANCTION_ORDER_GENERATION

        Array GetProposalsForSanctionOrder(int? page, int? rows, string sidx, string sord, out long totalRecords, int stateCode, int year, int stream, int agency, int batch, int scheme, string proposalType, out bool IsSOGenerated);

        bool AddSanctionOrderBAL(SanctionOrderViewModel model, ref string message);

        Array GetSanctionOrderListBAL(int? page, int? rows, string sidx, string sord, out long totalRecords, int stateCode, int year, int stream, int agency, int batch, int scheme, string proposalType);

        #endregion

        #region REPACKAGING

        Array GetProposalsForRepackaging(int? page, int? rows, string sidx, string sord, out long totalRecords, int year, int batch, int block, string package, int collaboration, string proposalType, string upgradationType);
        bool AddRepackagingDetails(RepackagingDetailsViewModel model);

        #endregion

        #region DPR_LIST

        Array GetDPRProposalListBAL(int stateCode, int districtCode, int blockCode, int year, int batch, int collaboStream, string proposalType, string proposalStatus, string packageId, string connectivity, int? page, int? rows, string sidx, string sord, out long totalRecords);
        bool DeleteDPRProposalBAL(int proposalCode);
        #endregion

        #region OLD_PROPOSAL_UPDATE

        Array GetProposalsForUpdateBAL(int? page, int? rows, string sidx, string sord, out long totalRecords, int stateCode, int districtCode, int IMS_YEAR, int MAST_BLOCK_ID, int IMS_BATCH, int IMS_STREAMS, string IMS_PROPOSAL_TYPE, int adminCode, string IMS_PROPOSAL_STATUS, string IMS_UPGRADE_CONNECT);
        Array GetProposalsForCNMappingBAL(int? page, int? rows, string sidx, string sord, out long totalRecords, int stateCode, int districtCode, int IMS_YEAR, int MAST_BLOCK_ID, int IMS_BATCH, int IMS_STREAMS, string IMS_PROPOSAL_TYPE, int adminCode, string IMS_PROPOSAL_STATUS, string IMS_UPGRADE_CONNECT);
        bool UpdateProposalDetailsBAL(ProposalUpdateViewModel model, out string message);
        ProposalUpdateViewModel GetOldProposalDetailsBAL(int proposalCode);
        bool ChangeCompleteProposalToStagedBAL(int proposalCode, out string message);
        bool ChangeStagedProposalToCompleteBAL(int proposalCode, out string message);
        bool MapCoreNetworkDetailsBAL(CoreNetworkMappingViewModel model);
        bool ChangeStage1ProposalToStage2BAL(Stage1ToStage2ViewModel model);
        bool ChangeCompleteProposalsToStage2BAL(Stage1ToStage2ViewModel model);
        //bool UpdateProposalPIUDetailsBAL(ProposalPIUUpdateViewModel model);
        string UpdateProposalPIUDetailsBAL(ProposalPIUUpdateViewModel_New model);
        bool UpdateProposalBlockDetailsBAL(ProposalUpdateBlockViewModel model);

        #endregion

        #region CHANGING_CORE_NETWORK

        bool UpdateCoreNetworkDetailsBAL(CoreNetworkUpdateViewModel model);

        bool UpdateBlockDetailsBAL(ProposalBlockUpdateViewModel model);

        #endregion

        #region Proposal Additional Cost
        Array GetProposalAdditionalCostListBAL(int stateCode, int districtCode, int blockCode, int yearCode, string packageCode, string proposalCode, int batchCode, int streamCode, string upgradationType, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetAdditionalCostListBAL(int roadCode, int page, int rows, string sidx, string sord, out long totalRecords);
        bool AddAdditionalCostDetailsBAL(ProposalAdditionalCostModel proposalAdditionalCostModel, ref string message);
        ProposalAdditionalCostModel EditAdditionalCostDetailsBAL(int TransctionCode, int ImsPrRoadCode);
        bool UpdateAdditionalCostDetailsBAL(ProposalAdditionalCostModel proposalAdditionalCostModel, ref string message);
        bool DeleteAdditionalCostDetailsBAL(int transactionCode, int imsPrRoadCode, ref string message);
        #endregion

        #region  Upload Sanction Order MRD CLEARANCE LETTERS
        bool AddMrdClearanceBAL(MrdClearenceViewModel mrdClearancanceViewModel, ref string message);
        Array ListMrdClearanceBAL(int stateCode, int year, int batch, int agencyCode, int collaboration, int page, int rows, string sidx, string sord, out long totalRecords);
        Array ListMrdClearanceFileBAL(int cleranceCode, string clearanceStatus, int page, int rows, string sidx, string sord, out long totalRecords);
        Array ListMrdClearanceRevisionBAL(int clearanceCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array ListOriginalMrdClearanceBAL(int clearanceCode, int page, int rows, string sidx, string sord, out long totalRecords);
        bool DeleteMrdClearanceBAL(int clearanceCode, ref string message);
        bool EditMrdClearanceBAL(MrdClearenceViewModel mrdClearancanceViewModel, ref string message);
        MrdClearenceViewModel GetMrdClearanceDetailsBAL(int clearanceCode);

        bool AddMrdClearanceRevisionBAL(MrdClearenceRevisionViewModel mrdClearanceRevisionViewModel, ref string message);
        bool DeleteMrdClearanceRevisionBAL(int clearanceCode, ref string message);
        MrdClearenceRevisionViewModel GetMrdClearanceRevisionDetailsBAL(int clearanceCode, string action);
        bool EditMrdClearanceRevsionBAL(MrdClearenceRevisionViewModel mrdClearanceRevsionViewModel, ref string message);
        bool EditDeleteMrdClearanceFileBAL(int clearanceCode, string fileType, string fileName, ref string message);

        #endregion

        #region GEPNIC_INTEGRATION

        Array GetGepnicProposals(int page, int rows, string sidx, string sord, out int totalRecords, int State, int District, int Year, int Block, string ProposalType, string Package);

        #endregion

        #region Dropped Proposal[by Pradip Patil (10/04/2017)]

        Array GetDroppingProposalsForSRRDABAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int MAST_STATE_CODE, int MAST_DISTRICT_CODE, int IMS_YEAR, int IMS_BATCH, int IMS_STREAMS, String ProposalType, string IMS_PROPOSAL_STATUS, string IMS_UPGRADE_CONNECT, int MAST_BLOCK_CODE, string filters, out ProposalColumnsTotal totalColModel);

        bool DropProposal(List<int> imsRoadCodeList, out String result);

        Array GetProposalsForDroppedOrder(int? page, int? rows, string sidx, string sord, out long totalRecords, int stateCode, int year, int stream, int batch, int scheme, string proposalType, out bool IsDOGenerated, string reqCode);

        bool AddDropOrderBAL(DropOrderViewModel model, List<int> mrdselectedroadList, ref string message);

        // bool AddDropOrderBAL(DropOrderViewModel model, string[] dropApproveArray, ref string message);

        Array GetDropOrderListBAL(int? page, int? rows, string sidx, string sord, out long totalRecords, int stateCode, int year, int stream, int batch, int scheme, string proposalType, String Status);

        Array GetDetailDropOrderListBAL(int? page, int? rows, string sidx, string sord, out long totalRecords, int RequestCode, int scheme);

        bool AddDropProposalBAL(AddDropOrderViewModel model, out String result);

        Array ListDropppingWorksBAL(int stateCode, int? page, int? rows, string sidx, string sord, out long totalRecords);
        Array ListWorksForDropppingBAL(int reqCode, int? page, int? rows, string sidx, string sord, out long totalRecords);
        bool AddDropRequestDetailsBAL(string[] dropDetails, string letterNo, out String result);
        #endregion

        #region Matrix Master
        Array ListMatrixParametersDetailsBAL(int? page, int? rows, string sidx, string sord, out long totalRecords);
        Array ListMatrixParametersWeightageDetailsBAL(int? page, int? rows, string sidx, string sord, out long totalRecords);
        bool AddMatrixDetailsBAL(string[] MatrixParams, ref string message);
        Boolean SaveDistrictMappinDetails(DistrictMappingModel model, out String message);
        Array ListMappedDistrictDetailsBAL(int? page, int? rows, string sidx, string sord, out long totalRecords);
        bool DeleteMappedDistrictsBAL(int districtId, out string message);
        #endregion

        #region Proposal Shifting
        Array GetProposalsForITNOForShiftingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int MAST_STATE_CODE, int MAST_DISTRICT_CODE, int IMS_YEAR, int IMS_BATCH, int IMS_STREAMS, String ProposalType, string IMS_PROPOSAL_STATUS, string IMS_UPGRADE_CONNECT, int MAST_BLOCK_CODE, string filters, out ProposalColumnsTotal totalColModel);

        Array GetLSBBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int MAST_STATE_CODE, int MAST_DISTRICT_CODE, int IMS_YEAR, int IMS_BATCH, int IMS_STREAMS, String ProposalType, string IMS_PROPOSAL_STATUS, string IMS_UPGRADE_CONNECT, int MAST_BLOCK_CODE, out ProposalColumnsTotal model);
        #endregion

        #region Proposals for PMGSY Scheme 5 (Vibrant Village) - Srishti Tyagi 27/06/2023

        Array GetProposalsVibrantVillageBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int MAST_STATE_CODE, int MAST_DISTRICT_CODE, int IMS_YEAR, int MAST_BLOCK_CODE, int IMS_BATCH, int IMS_STREAMS, String ProposalType, int MAST_DPIU_CODE, string IMS_PROPOSAL_STATUS, string IMS_UPGRADE_CONNECT, string filters, out ProposalColumnsTotal colTotal);

        #endregion
    }
}