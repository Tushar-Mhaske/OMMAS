#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   LSBProposalDAL.cs
        * Description   :   Data Methods for Creating , Editing, Deleting LSB Proposal and Related Screens of LSB Proposals.
        * Author        :   Shyam Yadav
        * Modified By   :   Shivkumar Deshmukh        
        * Creation Date :   20-05-2013
**/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.Models;
using PMGSY.Common;
using System.Data.Entity.Validation;
using System.Web.Mvc;
using System.Text;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Data.Entity;
using PMGSY.Models.Proposal;
using PMGSY.Extensions;
using System.Data.Entity.Core;

namespace PMGSY.DAL.Proposal
{
    public class LSBProposalDAL : ILSBProposalDAL
    {
        Models.PMGSYEntities dbContext;

        #region LSB Proposal

        /// <summary>
        /// Listing of LSB Proposals according to filters selscted by user.
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
        public Array GetLSBProposalsDAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int MAST_STATE_CODE, int MAST_DISTRICT_CODE, int IMS_YEAR, int MAST_BLOCK_CODE, int IMS_BATCH, int IMS_STREAMS, String ProposalType, string IMS_PROPOSAL_STATUS, string IMS_UPGRADE_CONNECT, out ProposalColumnsTotal model)
        {
            dbContext = new Models.PMGSYEntities();

            try
            {
                List<pr_get_proposals_Result> itemList = new List<pr_get_proposals_Result>();

                string PTAStatus = string.Empty;
                string STAStatus = string.Empty;
                string MRDStatus = string.Empty;
                string Status = string.Empty;

                string level = IMS_PROPOSAL_STATUS.Substring(0, 1);
                string flag = IMS_PROPOSAL_STATUS.Substring(1);

                switch (level)
                {
                    case "S":
                        switch (flag)
                        {
                            case "Y":
                                PTAStatus = "%";
                                STAStatus = "Y";
                                MRDStatus = "%";
                                Status = "%";
                                break;
                            case "N":
                                PTAStatus = "N";
                                STAStatus = "N";
                                MRDStatus = "N";
                                Status = "D";
                                break;
                            case "U":
                                PTAStatus = "%";
                                STAStatus = "U";
                                MRDStatus = "%";
                                Status = "%";
                                break;
                        }
                        break;
                    case "P":
                        switch (flag)
                        {
                            case "Y":
                                PTAStatus = "Y";
                                STAStatus = "%";
                                MRDStatus = "%";
                                Status = "%";
                                break;
                            case "N":
                                PTAStatus = "N";
                                STAStatus = "Y";
                                MRDStatus = "N";
                                Status = "S";
                                break;
                            case "U":
                                PTAStatus = "U";
                                STAStatus = "%";
                                MRDStatus = "%";
                                Status = "%";
                                break;
                        }
                        break;
                    case "M":
                        switch (flag)
                        {
                            case "Y":
                                PTAStatus = "%";
                                STAStatus = "%";
                                MRDStatus = "Y";
                                Status = "%";
                                break;
                            case "N":
                                PTAStatus = "%";
                                STAStatus = "Y";
                                MRDStatus = "N";
                                Status = "%";
                                break;
                            case "U":
                                PTAStatus = "%";
                                STAStatus = "%";
                                MRDStatus = "U";
                                Status = "%";
                                break;
                            case "R":
                                PTAStatus = "%";
                                STAStatus = "%";
                                MRDStatus = "R";
                                Status = "%";
                                break;
                            case "D":
                                PTAStatus = "%";
                                STAStatus = "%";
                                MRDStatus = "D";
                                Status = "%";
                                break;
                        }
                        break;
                    case "D":
                        switch (flag)
                        {
                            case "E":
                                PTAStatus = "%";
                                STAStatus = "%";
                                MRDStatus = "%";
                                Status = "E";
                                break;
                            case "H":
                                PTAStatus = "%";
                                STAStatus = "%";
                                MRDStatus = "%";
                                Status = "H";
                                break;
                            case "D":
                                PTAStatus = "%";
                                STAStatus = "%";
                                MRDStatus = "%";
                                Status = "D";
                                break;
                        }
                        break;
                    default:
                        PTAStatus = "%";
                        STAStatus = "%";
                        MRDStatus = "%";
                        Status = "%";
                        break;

                }

                short roleCode = dbContext.UM_User_Master.Where(m => m.UserID == PMGSYSession.Current.UserId).Select(m => m.DefaultRoleID).FirstOrDefault();
                //var listProposals = dbContext.USP_GET_PROPOSAL_LIST(4, MAST_STATE_CODE, (PMGSYSession.Current.DistrictCode < 0 ? 0 : PMGSYSession.Current.DistrictCode), (MAST_BLOCK_CODE <= 0 ? 0 : MAST_BLOCK_CODE), (IMS_YEAR < 0 ? 0 : IMS_YEAR), (IMS_BATCH < 0 ? 0 : IMS_BATCH), (IMS_STREAMS < 0 ? 0 : IMS_STREAMS), 0, PMGSYSession.Current.AdminNdCode, 0, "%", "%", Status, PTAStatus, STAStatus, MRDStatus, "L", (IMS_UPGRADE_CONNECT == "0" ? "%" : IMS_UPGRADE_CONNECT), (PMGSYSession.Current.PMGSYScheme == 3 ? (byte)1 : PMGSYSession.Current.PMGSYScheme), (short)PMGSYSession.Current.RoleCode).ToList();
                var listProposals = dbContext.USP_GET_PROPOSAL_LIST(4, MAST_STATE_CODE, (PMGSYSession.Current.DistrictCode < 0 ? 0 : PMGSYSession.Current.DistrictCode), (MAST_BLOCK_CODE <= 0 ? 0 : MAST_BLOCK_CODE), (IMS_YEAR < 0 ? 0 : IMS_YEAR), (IMS_BATCH < 0 ? 0 : IMS_BATCH), (IMS_STREAMS < 0 ? 0 : IMS_STREAMS), 0, PMGSYSession.Current.AdminNdCode, 0, "%", "%", Status, PTAStatus, STAStatus, MRDStatus, "L", (IMS_UPGRADE_CONNECT == "0" ? "%" : IMS_UPGRADE_CONNECT), PMGSYSession.Current.PMGSYScheme, roleCode).ToList();
                IQueryable<USP_GET_PROPOSAL_LIST_Result> query = listProposals.AsQueryable<USP_GET_PROPOSAL_LIST_Result>();
                totalRecords = listProposals.Count();

                ProposalColumnsTotal colTotal = new ProposalColumnsTotal();
                colTotal.TOT_COST = listProposals.Sum(m => m.TOTAL_COST).HasValue ? listProposals.Sum(m => m.TOTAL_COST).Value : 0;//listProposals.Sum(m => m.ROAD_AMT).HasValue ? listProposals.Sum(m => m.ROAD_AMT).Value : 0;
                colTotal.TOT_HAB1000 = listProposals.Sum(m => m.POP1000);
                colTotal.TOT_HAB250 = listProposals.Sum(m => m.POP250);
                colTotal.TOT_HAB499 = listProposals.Sum(m => m.POP499);
                colTotal.TOT_HAB999 = listProposals.Sum(m => m.POP999);
                colTotal.TOT_HABS = listProposals.Sum(m => m.TOTAL_HABS).HasValue ? listProposals.Sum(m => m.TOTAL_HABS).Value : 0;
                colTotal.TOT_HIGHER_SPEC = listProposals.Sum(m => m.HIGHER_SPECS);
                colTotal.TOT_MANE_COST = listProposals.Sum(m => m.MAINT_AMT).HasValue ? listProposals.Sum(m => m.MAINT_AMT).Value : 0;
                colTotal.TOT_MORD_COST = listProposals.Sum(m => m.BRIDGE_AMT).Value;
                colTotal.TOT_PAV_LENGTH = listProposals.Sum(m => m.ROAD_LENGTH).HasValue ? listProposals.Sum(m => m.ROAD_LENGTH).Value : 0;
                colTotal.TOT_RENEWAL_COST = listProposals.Sum(m => m.RENEWAL_AMT);
                colTotal.TOT_STATE_COST = listProposals.Sum(m => m.BRIDGE_STATE).HasValue ? (decimal)listProposals.Sum(m => m.BRIDGE_STATE) : 0;

                colTotal.STATE_SHARE_COST = listProposals.Sum(m => m.STATE_SHARE_2015);
                colTotal.MORD_SHARE_COST = listProposals.Sum(m => m.MORD_SHARE_2015);
                colTotal.TOTAL_STATE_SHARE = listProposals.Sum(m => m.TOTAL_STATE_SHARE);
                colTotal.TOTAL_SHARE_COST = listProposals.Sum(m => m.TOTAL_SHARE_COST);
                model = colTotal;

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        query = query.OrderBy(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                    }
                    else
                    {
                        query = query.OrderByDescending(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                    }
                }
                else
                {
                    query = query.OrderBy(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                return query.Select(propDetails => new
                {
                    id = propDetails.IMS_PR_ROAD_CODE.ToString(),
                    cell = new[] {                         
                                    propDetails.MAST_BLOCK_NAME.Trim(),
                                    propDetails.IMS_PACKAGE_ID,
                                    propDetails.IMS_ROAD_NAME == null ? "NA" :  propDetails.IMS_ROAD_NAME,
                                    propDetails.ROAD_NAME == null ? "NA" : propDetails.ROAD_NAME,
                                    propDetails.ROAD_LENGTH.ToString(),
                                    Math.Round(Convert.ToDecimal(propDetails.BRIDGE_STATE),2).ToString(),
                                    Math.Round(Convert.ToDecimal(propDetails.HIGHER_SPECS),2).ToString(),
                                    Math.Round(Convert.ToDecimal(propDetails.BRIDGE_AMT),2).ToString(),
                                    //Added By Pankaj To display Higher Specification Amount 
                                    //Math.Round(Convert.ToDecimal(propDetails.BRIDGE_AMT + propDetails.BRIDGE_STATE),2).ToString(),
                                    Math.Round(Convert.ToDecimal(propDetails.TOTAL_COST),2).ToString(),
                                    Math.Round(Convert.ToDecimal(propDetails.MAINT_AMT),2).ToString(),
                                    Math.Round(Convert.ToDecimal(propDetails.RENEWAL_AMT),2).ToString(),
                                    propDetails.FUND_SHARING_RATIO.ToString(),
                                    Math.Round(Convert.ToDecimal(propDetails.STATE_SHARE_2015),2).ToString(),
                                    Math.Round(Convert.ToDecimal(propDetails.MORD_SHARE_2015),2).ToString(),
                                    Math.Round(Convert.ToDecimal(propDetails.TOTAL_STATE_SHARE),2).ToString(),
                                    Math.Round(Convert.ToDecimal(propDetails.TOTAL_SHARE_COST),2).ToString(),
                                    ( propDetails.IMS_LOCK_STATUS == "M" ) 
                                        ? "<a href='#' title='Click here to add component details' class='ui-icon ui-icon-unlocked ui-align-center' onClick='EditComponentDetails(\""  + propDetails.IMS_PR_ROAD_CODE.ToString().Trim()  +"\"); return false;'>Edit</a>" 
                                        : (propDetails.IMS_LOCK_STATUS.ToUpper() == "N" || propDetails.IMS_SANCTIONED.ToUpper() == "R") 
                                            ?  ((dbContext.IMS_LSB_BRIDGE_COMPONENT_DETAIL.Where(a=> a.IMS_PR_ROAD_CODE == propDetails.IMS_PR_ROAD_CODE).Any()) 
                                                ?  "<a href='#' title='Click here to add component details' class='ui-icon ui-icon-note ui-align-center' onClick='EditComponentDetails(\""  
                                                :  "<a href='#' title='Click here to add component details' class='ui-icon ui-icon-plusthick ui-align-center' onClick='EditComponentDetails(\""  )+ propDetails.IMS_PR_ROAD_CODE.ToString().Trim()  +"\"); return false;'>Edit</a>"
                                            :"<a href='#' class='ui-icon ui-icon-locked ui-align-center'>",


                                    ( propDetails.IMS_LOCK_STATUS == "M" ) 
                                        ? "<a href='#' title='Click here to add other details' class='ui-icon ui-icon-unlocked ui-align-center' onClick='EditLSBOtherDetails(\"" + propDetails.IMS_PR_ROAD_CODE.ToString().Trim()  +"\"); return false;'>Edit</a>" 
                                        : (propDetails.IMS_LOCK_STATUS.ToUpper() == "N" || propDetails.IMS_SANCTIONED.ToUpper() == "R") 
                                            ? ((dbContext.IMS_LSB_BRIDGE_DETAIL.Where(a=> a.IMS_PR_ROAD_CODE == propDetails.IMS_PR_ROAD_CODE).Any()) 
                                                ? "<a href='#' title='Click here to add other details' class='ui-icon ui-icon-note ui-align-center' onClick='EditLSBOtherDetails(\"" 
                                                : "<a href='#' title='Click here to add other details' class='ui-icon ui-icon-plusthick ui-align-center' onClick='EditLSBOtherDetails(\"" ) + propDetails.IMS_PR_ROAD_CODE.ToString().Trim()  +"\"); return false;'>Edit</a>" 
                                            : "<a href='#' class='ui-icon ui-icon-locked ui-align-center'>",

                                            "<a href='#' class='ui-icon ui-icon-plusthick ui-align-center'  onclick='UploadFile(\"" + propDetails.IMS_PR_ROAD_CODE.ToString().Trim() + "\"); return false;'>Upload</a>" ,

                                    //( propDetails.IMS_LOCK_STATUS == "M" ) 
                                    //    ?  "<a href='#' class='ui-icon ui-icon-unlocked ui-align-center'  onclick='UploadFile(\"" + propDetails.IMS_PR_ROAD_CODE.ToString().Trim() + "\"); return false;'>Upload</a>"  
                                    //    :  ( propDetails.IMS_LOCK_STATUS.ToUpper() == "N" || propDetails.IMS_SANCTIONED.ToUpper() == "R")  
                                    //        ? ((dbContext.IMS_PROPOSAL_FILES.Where(a=> a.IMS_PR_ROAD_CODE == propDetails.IMS_PR_ROAD_CODE).Any()) 
                                    //            ? "<a href='#' class='ui-icon ui-icon-note ui-align-center'  onclick='UploadFile(\"" 
                                    //            : "<a href='#' class='ui-icon ui-icon-plusthick ui-align-center'  onclick='UploadFile(\"") + propDetails.IMS_PR_ROAD_CODE.ToString().Trim() + "\"); return false;'>Upload</a>" 
                                    //        : "<a href='#' class='ui-icon ui-icon-locked ui-align-center'>",


                                    "<a href='#'title='Click here to view the proposal details' class='ui-icon ui-icon-zoomin ui-align-center' onClick='ShowLSBDetails(\"" + propDetails.IMS_PR_ROAD_CODE.ToString().Trim() + "$" + propDetails.IMS_LOCK_STATUS  + "\"); return false;'>Show Details</a>",



                                    //( propDetails.IMS_LOCK_STATUS == "M" ) 
                                    //    ? "<a href='#' title='Click here to edit the proposal' class='ui-icon ui-icon-unlocked ui-align-center' onClick='EditUnlockedLSBDetails(\"" + propDetails.IMS_PR_ROAD_CODE.ToString().Trim()  +"\"); return false;'>Edit</a>" 
                                    //    :  (propDetails.IMS_LOCK_STATUS.ToUpper() == "N" || propDetails.IMS_SANCTIONED.ToUpper() == "R") 
                                    //        ? "<a href='#' title='Click here to edit the proposal' class='ui-icon ui-icon-pencil ui-align-center' onClick='EditLSBDetails(\"" + propDetails.IMS_PR_ROAD_CODE.ToString().Trim()  +"\"); return false;'>Edit</a>"
                                    //        :"<a href='#' class='ui-icon ui-icon-locked ui-align-center'>",

                                     ( propDetails.IMS_LOCK_STATUS == "M" ) 
                                        ? "<a href='#' title='Click here to edit the proposal' class='ui-icon ui-icon-unlocked ui-align-center' onClick='EditLSBDetails(\"" + propDetails.IMS_PR_ROAD_CODE.ToString().Trim()  +"\"); return false;'>Edit</a>" 
                                        :  (propDetails.IMS_LOCK_STATUS.ToUpper() == "N" || propDetails.IMS_SANCTIONED.ToUpper() == "R") 
                                            ? "<a href='#' title='Click here to edit the proposal' class='ui-icon ui-icon-pencil ui-align-center' onClick='EditLSBDetails(\"" + propDetails.IMS_PR_ROAD_CODE.ToString().Trim()  +"\"); return false;'>Edit</a>"
                                            :"<a href='#' class='ui-icon ui-icon-locked ui-align-center'>",



                                    ( propDetails.IMS_ISCOMPLETED == "E" && propDetails.IMS_LOCK_STATUS.ToUpper() == "N") 
                                        ? "<a href='#' title='Click here to delete the proposal' class='ui-icon ui-icon-trash ui-align-center' onClick='DeleteLSBDetails(\"" + propDetails.IMS_PR_ROAD_CODE.ToString().Trim()  +"\"); return false;'>Delete</a>"
                                        :"<a href='#' class='ui-icon ui-icon-locked ui-align-center'>"                                    
                   }
                }).ToArray();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                model = null;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        /// <summary>
        /// LSB Proposals for SRRDA 
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
        /// <returns></returns>
        public Array GetLSBProposalsForSRRDADAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int MAST_STATE_CODE, int MAST_DISTRICT_CODE, int IMS_YEAR, int IMS_BATCH, int IMS_STREAMS, String ProposalType, string IMS_PROPOSAL_STATUS, string IMS_UPGRADE_CONNECT, int MAST_BLOCK_CODE, out ProposalColumnsTotal model)
        {
            dbContext = new Models.PMGSYEntities();

            try
            {
                dbContext = new Models.PMGSYEntities();
                List<USP_GET_PROPOSAL_LIST_Result> itemList = new List<USP_GET_PROPOSAL_LIST_Result>();

                string PTAStatus = string.Empty;
                string STAStatus = string.Empty;
                string MRDStatus = string.Empty;
                string Status = string.Empty;

                string level = IMS_PROPOSAL_STATUS.Substring(0, 1);
                string flag = IMS_PROPOSAL_STATUS.Substring(1);


                switch (level)
                {
                    case "S":
                        switch (flag)
                        {
                            case "Y":
                                PTAStatus = "%";
                                STAStatus = "Y";
                                MRDStatus = "%";
                                Status = "%";
                                break;
                            case "N":
                                PTAStatus = "%";
                                STAStatus = "N";
                                MRDStatus = "%";
                                Status = "D";
                                break;
                            case "U":
                                PTAStatus = "%";
                                STAStatus = "U";
                                MRDStatus = "%";
                                Status = "%";
                                break;
                        }
                        break;
                    case "P":
                        switch (flag)
                        {
                            case "Y":
                                PTAStatus = "Y";
                                STAStatus = "%";
                                MRDStatus = "%";
                                Status = "%";
                                break;
                            case "N":
                                PTAStatus = "Y";
                                STAStatus = "Y";
                                MRDStatus = "%";
                                Status = "%";
                                break;
                            case "U":
                                PTAStatus = "U";
                                STAStatus = "%";
                                MRDStatus = "%";
                                Status = "%";
                                break;
                        }
                        break;
                    case "M":
                        switch (flag)
                        {
                            case "Y":
                                PTAStatus = "%";
                                STAStatus = "%";
                                MRDStatus = "Y";
                                Status = "%";
                                break;
                            case "N":
                                PTAStatus = "%";
                                STAStatus = "Y";
                                MRDStatus = "N";
                                Status = "%";
                                break;
                            case "U":
                                PTAStatus = "%";
                                STAStatus = "%";
                                MRDStatus = "U";
                                Status = "%";
                                break;
                            case "R":
                                PTAStatus = "%";
                                STAStatus = "%";
                                MRDStatus = "R";
                                Status = "%";
                                break;
                            case "D":
                                PTAStatus = "%";
                                STAStatus = "%";
                                MRDStatus = "D";
                                Status = "%";
                                break;
                        }
                        break;
                    case "D":
                        switch (flag)
                        {
                            case "E":
                                PTAStatus = "%";
                                STAStatus = "%";
                                MRDStatus = "%";
                                Status = "E";
                                break;
                            case "H":
                                PTAStatus = "%";
                                STAStatus = "%";
                                MRDStatus = "%";
                                Status = "H";
                                break;
                            case "D":
                                PTAStatus = "%";
                                STAStatus = "%";
                                MRDStatus = "%";
                                Status = "D";
                                break;
                        }
                        break;
                    default:
                        PTAStatus = "%";
                        STAStatus = "%";
                        MRDStatus = "%";
                        Status = "%";
                        break;

                }

                //itemList = dbContext.pr_get_proposals_for_srrda(MAST_STATE_CODE, MAST_DISTRICT_CODE, PMGSYSession.Current.AdminNdCode, IMS_YEAR, IMS_BATCH, IMS_STREAMS, "L",(IMS_UPGRADE_CONNECT == "0" ? "%" : IMS_UPGRADE_CONNECT),STAStatus,PTAStatus,MRDStatus,Status, PMGSYSession.Current.PMGSYScheme).ToList<pr_get_proposals_for_srrda_Result>();
                //totalRecords = itemList.Count();
                int agencyCode = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Select(m => m.MAST_AGENCY_CODE).FirstOrDefault();
                ///Changes for RCPLWE
                //itemList = dbContext.USP_GET_PROPOSAL_LIST(2, MAST_STATE_CODE, (MAST_DISTRICT_CODE <= 0 ? 0 : MAST_DISTRICT_CODE), (MAST_BLOCK_CODE <= 0 ? 0 : MAST_BLOCK_CODE), (IMS_YEAR <= 0 ? 0 : IMS_YEAR), (IMS_BATCH <= 0 ? 0 : IMS_BATCH), (IMS_STREAMS <= 0 ? 0 : IMS_STREAMS), (PMGSYSession.Current.PMGSYScheme == 3 ? 0 : agencyCode), 0, 0, "%", "%", Status, PTAStatus, STAStatus, MRDStatus, "L", (IMS_UPGRADE_CONNECT == "0" ? "%" : IMS_UPGRADE_CONNECT), (PMGSYSession.Current.PMGSYScheme == 3 ? (byte)1 : PMGSYSession.Current.PMGSYScheme), (short)PMGSYSession.Current.RoleCode).ToList<USP_GET_PROPOSAL_LIST_Result>();

                itemList = dbContext.USP_GET_PROPOSAL_LIST(2, MAST_STATE_CODE, (MAST_DISTRICT_CODE <= 0 ? 0 : MAST_DISTRICT_CODE), (MAST_BLOCK_CODE <= 0 ? 0 : MAST_BLOCK_CODE), (IMS_YEAR <= 0 ? 0 : IMS_YEAR), (IMS_BATCH <= 0 ? 0 : IMS_BATCH), (IMS_STREAMS <= 0 ? 0 : IMS_STREAMS), agencyCode, 0, 0, "%", "%", Status, PTAStatus, STAStatus, MRDStatus, "L", (IMS_UPGRADE_CONNECT == "0" ? "%" : IMS_UPGRADE_CONNECT), PMGSYSession.Current.PMGSYScheme, (short)PMGSYSession.Current.RoleCode).ToList<USP_GET_PROPOSAL_LIST_Result>();

                totalRecords = itemList.Count();

                ProposalColumnsTotal colTotal = new ProposalColumnsTotal();
                colTotal.TOT_COST = itemList.Sum(m => m.TOTAL_COST).HasValue ? itemList.Sum(m => m.TOTAL_COST).Value : 0; //itemList.Sum(m => m.ROAD_AMT).HasValue ? itemList.Sum(m => m.ROAD_AMT).Value : 0;
                colTotal.TOT_HAB1000 = itemList.Sum(m => m.POP1000);
                colTotal.TOT_HAB250 = itemList.Sum(m => m.POP250);
                colTotal.TOT_HAB499 = itemList.Sum(m => m.POP499);
                colTotal.TOT_HAB999 = itemList.Sum(m => m.POP999);
                colTotal.TOT_HABS = itemList.Sum(m => m.TOTAL_HABS).HasValue ? itemList.Sum(m => m.TOTAL_HABS).Value : 0;
                colTotal.TOT_HIGHER_SPEC = itemList.Sum(m => m.HIGHER_SPECS);
                colTotal.TOT_MANE_COST = itemList.Sum(m => m.MAINT_AMT).HasValue ? itemList.Sum(m => m.MAINT_AMT).Value : 0;
                colTotal.TOT_MORD_COST = itemList.Sum(m => m.BRIDGE_AMT).Value;
                colTotal.TOT_PAV_LENGTH = itemList.Sum(m => m.ROAD_LENGTH).HasValue ? itemList.Sum(m => m.ROAD_LENGTH).Value : 0;
                colTotal.TOT_RENEWAL_COST = itemList.Sum(m => m.RENEWAL_AMT);
                colTotal.TOT_STATE_COST = itemList.Sum(m => m.BRIDGE_STATE).HasValue ? (decimal)itemList.Sum(m => m.BRIDGE_STATE) : 0;//itemList.Sum(m => m.BRIDGE_STATE);

                colTotal.STATE_SHARE_COST = itemList.Sum(m => m.STATE_SHARE_2015);
                colTotal.MORD_SHARE_COST = itemList.Sum(m => m.MORD_SHARE_2015);
                colTotal.TOTAL_STATE_SHARE = itemList.Sum(m => m.TOTAL_STATE_SHARE);
                colTotal.TOTAL_SHARE_COST = itemList.Sum(m => m.TOTAL_SHARE_COST);

                model = colTotal;

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        itemList = itemList.OrderBy(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        itemList = itemList.OrderByDescending(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    itemList = itemList.OrderBy(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                itemList = itemList.OrderByDescending(x => x.MAST_STATE_NAME).ToList();

                return itemList.Select(propDetails => new
                {
                    id = propDetails.IMS_PR_ROAD_CODE.ToString(),
                    cell = new[] {    
                                    propDetails.MAST_DISTRICT_NAME.Trim(),
                                    propDetails.MAST_BLOCK_NAME.Trim(),
                                    propDetails.IMS_PACKAGE_ID,
                                    propDetails.IMS_ROAD_NAME == null ? "NA" :  propDetails.IMS_ROAD_NAME,
                                    propDetails.ROAD_NAME == null ? "NA" : propDetails.ROAD_NAME,
                                    propDetails.ROAD_LENGTH.ToString(),
                                    Math.Round(Convert.ToDecimal(propDetails.BRIDGE_STATE),2).ToString(),
                                    Math.Round(Convert.ToDecimal(propDetails.HIGHER_SPECS),2).ToString(),
                                    Math.Round(Convert.ToDecimal(propDetails.BRIDGE_AMT),2).ToString(),
                                    //Math.Round(Convert.ToDecimal(propDetails.BRIDGE_AMT + propDetails.BRIDGE_STATE),2).ToString(),
                                    Math.Round(Convert.ToDecimal(propDetails.TOTAL_COST),2).ToString(),
                                    //Added By Pankaj To display Higher Specification Amount 
                                    Math.Round(Convert.ToDecimal(propDetails.MAINT_AMT),2).ToString(),

                                    // added by rohit borse on 22-08-2022
                                    Math.Round(Convert.ToDecimal(propDetails.RENEWAL_AMT),2).ToString(),

                                    propDetails.FUND_SHARING_RATIO.ToString(),
                                    Math.Round(Convert.ToDecimal(propDetails.STATE_SHARE_2015),2).ToString(),
                                    Math.Round(Convert.ToDecimal(propDetails.MORD_SHARE_2015),2).ToString(),
                                    Math.Round(Convert.ToDecimal(propDetails.TOTAL_STATE_SHARE),2).ToString(),
                                    Math.Round(Convert.ToDecimal(propDetails.TOTAL_SHARE_COST),2).ToString(),
                                     "<a href='#'title='Click here to view the proposal details' class='ui-icon ui-icon-zoomin ui-align-center' onClick='ShowLSBDetails(\"" + propDetails.IMS_PR_ROAD_CODE.ToString().Trim() + "$" + propDetails.IMS_LOCK_STATUS  + "\"); return false;'>Show Details</a>",
                                     "<a href='#' class='ui-icon ui-icon-plusthick ui-align-center' onclick='UploadFile(\"" + propDetails.IMS_PR_ROAD_CODE.ToString().Trim() + "\"); return false;'>Upload</a>" 
                   }
                }).ToArray();

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                model = null;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        /// <summary>
        /// Save details for LSB Proposal
        /// </summary>
        /// <param name="objProposal"></param>
        /// <returns></returns>
        public string SaveLSBProposalDAL(IMS_SANCTIONED_PROJECTS objProposal)
        {
            dbContext = new Models.PMGSYEntities();
            try
            {
                Int32? MaxID;
                if (dbContext.IMS_SANCTIONED_PROJECTS.Count() == 0)
                {
                    MaxID = 0;
                }
                else
                {
                    MaxID = (from c in dbContext.IMS_SANCTIONED_PROJECTS select (Int32?)c.IMS_PR_ROAD_CODE ?? 0).Max();
                }

                objProposal.IMS_PR_ROAD_CODE = Convert.ToInt32(MaxID) + 1;
                objProposal.USERID = PMGSYSession.Current.UserId;
                objProposal.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                objProposal.IMS_UPGRADE_CONNECT = PMGSYSession.Current.PMGSYScheme == 2 ? "U" : objProposal.IMS_UPGRADE_CONNECT;
                dbContext.IMS_SANCTIONED_PROJECTS.Add(objProposal);
                dbContext.SaveChanges();

                return string.Empty;
            }
            catch (DbUpdateException ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "SaveLSBProposalDAL.DbUpdateException");
                return ("An Error Occurred While Processing Your Request.");
            }
            catch (OptimisticConcurrencyException ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "SaveLSBProposalDAL.OptimisticConcurrencyException");
                return ("An Error Occurred While Processing Your Request.");
            }
            catch (DbEntityValidationException e)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(e, HttpContext.Current);
                ErrorLog.LogError(e, "SaveLSBProposalDAL.DbEntityValidationException");
                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        return (eve.ValidationErrors.ToString());

                    }
                }
                return ("Error Occurred While Processing Request.");
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "SaveLSBProposalDAL");
                return ("Error Occurred While Processing Request.");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// Update Details in Database for LSB
        /// </summary>
        /// <param name="ims_sanctioned_projects"></param>
        /// <returns></returns>
        public string UpdateLSBProposalDAL(IMS_SANCTIONED_PROJECTS objProposal)
        {
            dbContext = new PMGSYEntities();
            try
            {
                objProposal.USERID = PMGSYSession.Current.UserId;
                objProposal.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                dbContext.Entry(objProposal).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                return string.Empty;
            }
            catch (DbUpdateException ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "UpdateLSBProposalDAL.DbUpdateException");
                return ("An Error Occurred While Processing Your Request.");
            }
            catch (OptimisticConcurrencyException ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "UpdateLSBProposalDAL.OptimisticConcurrencyException");
                return ("An Error Occurred While Processing Your Request.");
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "UpdateLSBProposalDAL");
                return ("Error Occurred While Processing Request.");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// Delete LSB Proposals
        /// </summary>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        /// <returns></returns>
        public string DeleteLSBProposalDAL(int IMS_PR_ROAD_CODE)
        {
            try
            {
                dbContext = new PMGSYEntities();

                /*Set Timeout to infinity*/
                var adapter = (IObjectContextAdapter)dbContext;
                var objectContext = adapter.ObjectContext;
                objectContext.CommandTimeout = 0;

                IMS_SANCTIONED_PROJECTS ims_sanctioned_projects = dbContext.IMS_SANCTIONED_PROJECTS.Find(IMS_PR_ROAD_CODE);
                ims_sanctioned_projects.USERID = PMGSYSession.Current.UserId;
                ims_sanctioned_projects.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                dbContext.Entry(ims_sanctioned_projects).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                ///Changed by SAMMED A. PATIL on 22JUNE2017 to delete record from IMS_UNLOCK_DETAILS if record is not sanctioned
                if (ims_sanctioned_projects.IMS_SANCTIONED == "N")
                {
                    var ims_unlock_details = dbContext.IMS_UNLOCK_DETAILS.Where(x => x.IMS_PR_ROAD_CODE == IMS_PR_ROAD_CODE).ToList();
                    if (ims_unlock_details != null)
                    {
                        foreach (var item in ims_unlock_details)
                        {
                            dbContext.IMS_UNLOCK_DETAILS.Remove(item);
                            dbContext.SaveChanges();
                        }
                    }
                }
                dbContext.IMS_SANCTIONED_PROJECTS.Remove(ims_sanctioned_projects);
                dbContext.SaveChanges();
                return string.Empty;
            }
            catch (DbUpdateException ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("IMS_PR_ROAD_CODE :" + IMS_PR_ROAD_CODE);
                    sw.WriteLine("======================================================================");
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "DeleteLSBProposalDAL() : DbUpdateException");
                    if (ex != null)
                        sw.WriteLine("Exception : " + ex.ToString());
                    if (ex.InnerException != null)
                        sw.WriteLine("innerException : " + ex.InnerException.ToString());
                    if (ex.InnerException.InnerException != null)
                        sw.WriteLine("ex.InnerException.InnerException : " + ex.InnerException.InnerException.ToString());
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }
                return ("An Error Occurred While Processing Your Request.");
            }
            catch (OptimisticConcurrencyException ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("IMS_PR_ROAD_CODE :" + IMS_PR_ROAD_CODE);
                    sw.WriteLine("======================================================================");
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "DeleteLSBProposalDAL() : OptimisticConcurrencyException");
                    if (ex != null)
                        sw.WriteLine("Exception : " + ex.ToString());
                    if (ex.InnerException != null)
                        sw.WriteLine("innerException : " + ex.InnerException.ToString());
                    if (ex.InnerException.InnerException != null)
                        sw.WriteLine("ex.InnerException.InnerException : " + ex.InnerException.InnerException.ToString());
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }
                return ("An Error Occurred While Processing Your Request.");
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("IMS_PR_ROAD_CODE :" + IMS_PR_ROAD_CODE);
                    sw.WriteLine("======================================================================");
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "DeleteLSBProposalDAL() : Exception");
                    if (ex != null)
                        sw.WriteLine("Exception : " + ex.ToString());
                    if (ex.InnerException != null)
                        sw.WriteLine("innerException : " + ex.InnerException.ToString());
                    if (ex.InnerException.InnerException != null)
                        sw.WriteLine("ex.InnerException.InnerException : " + ex.InnerException.InnerException.ToString());
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }
                return ("Error Occurred While Processing Request.");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// Update or Add Other details for LSB
        /// </summary>
        /// <param name="lsb_bridge_detail"></param>
        /// <param name="Operation"></param>
        /// <returns></returns>
        public string LSBOtherDetailsDAL(IMS_LSB_BRIDGE_DETAIL lsb_bridge_detail, string Operation)
        {
            dbContext = new PMGSYEntities();
            try
            {
                if (Operation.Equals("C"))
                {
                    lsb_bridge_detail.USERID = PMGSYSession.Current.UserId;
                    lsb_bridge_detail.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    dbContext.IMS_LSB_BRIDGE_DETAIL.Add(lsb_bridge_detail);
                    dbContext.SaveChanges();
                    return string.Empty;
                }
                else if (Operation.Equals("U"))
                {
                    lsb_bridge_detail.USERID = PMGSYSession.Current.UserId;
                    lsb_bridge_detail.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    dbContext.Entry(lsb_bridge_detail).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    return string.Empty;
                }

                return ("An Error Occurred While Processing Your Request.");
            }
            catch (DbUpdateException dbeux)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(dbeux, HttpContext.Current);
                return ("An Error Occurred While Processing Your Request.");
            }
            catch (OptimisticConcurrencyException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return ("An Error Occurred While Processing Your Request.");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return ("Error Occurred While Processing Request.");
            }

            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// Delete LSB Proposals
        /// </summary>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        /// <returns></returns>
        public string DeleteLSBOthDetailsBAL(int IMS_PR_ROAD_CODE)
        {
            try
            {
                dbContext = new PMGSYEntities();
                IMS_LSB_BRIDGE_DETAIL ims_bridge_details = dbContext.IMS_LSB_BRIDGE_DETAIL.Find(IMS_PR_ROAD_CODE);
                if (ims_bridge_details == null)
                {
                    return ("No other details available for current LSB Proposal.");
                }

                ims_bridge_details.USERID = PMGSYSession.Current.UserId;
                ims_bridge_details.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                dbContext.Entry(ims_bridge_details).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();


                dbContext.IMS_LSB_BRIDGE_DETAIL.Remove(ims_bridge_details);
                dbContext.SaveChanges();
                return string.Empty;
            }
            catch (DbUpdateException ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "DeleteLSBOthDetailsDAL.DbUpdateException");
                return ("An Error Occurred While Processing Your Request.");
            }
            catch (OptimisticConcurrencyException ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "DeleteLSBOthDetailsDAL.OptimisticConcurrencyException");
                return ("An Error Occurred While Processing Your Request.");
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "DeleteLSBOthDetailsDAL");
                return ("Error Occurred While Processing Request.");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// Array of all assigned Component details for LSB
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
                using (var dbContext = new PMGSYEntities())
                {
                    var componentList = (from bcd in dbContext.IMS_LSB_BRIDGE_COMPONENT_DETAIL
                                         join mct in dbContext.MASTER_COMPONENT_TYPE on bcd.IMS_COMPONENT_CODE equals mct.MAST_COMPONENT_CODE
                                         where bcd.IMS_PR_ROAD_CODE == roadId
                                         select new
                                         {
                                             bcd.IMS_PR_ROAD_CODE,
                                             mct.MAST_COMPONENT_CODE,
                                             mct.MAST_COMPONENT_NAME,
                                             bcd.IMS_QUANTITY,
                                             bcd.IMS_TOTAL_COST,
                                             bcd.IMS_GRADE_CONCRETE
                                         }).ToList();


                    totalRecords = componentList.Count();

                    if (sidx.Trim() != string.Empty)
                    {
                        if (sord.ToString() == "asc")
                        {
                            componentList = componentList.OrderBy(x => x.MAST_COMPONENT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                        }
                        else
                        {
                            componentList = componentList.OrderByDescending(x => x.MAST_COMPONENT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                        }
                    }
                    else
                    {
                        componentList = componentList.OrderBy(x => x.MAST_COMPONENT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }

                    return componentList.Select(model => new
                    {
                        id = model.MAST_COMPONENT_CODE,
                        cell = new[] {
                                        model.IMS_PR_ROAD_CODE.ToString(),
                                        model.MAST_COMPONENT_NAME,
                                        model.IMS_QUANTITY.ToString(),
                                        model.IMS_TOTAL_COST.ToString(),
                                        model.IMS_GRADE_CONCRETE.ToString(),
                                        "<a href='#' title='Click here to edit the component details' class='ui-icon ui-icon-pencil ui-align-center' onClick='EditCurrentLSBComponent(" + model.IMS_PR_ROAD_CODE.ToString().Trim() + "," + model.MAST_COMPONENT_CODE + "); return false;'>Edit</a>",
                                        "<a href='#' title='Click here to delete the component details' class='ui-icon ui-icon-trash ui-align-center' onClick='DeleteCurrentLSBComponent(" + model.IMS_PR_ROAD_CODE.ToString().Trim()  + "," + model.MAST_COMPONENT_CODE + "); return false;'>Delete</a>"
                                }
                    }).ToArray();
                }
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw Ex;
            }
        }

        /// <summary>
        /// Save or update LSB Component Details Entry
        /// </summary>
        /// <param name="lsb_component_detail"></param>
        /// <param name="Operation"></param>
        /// <returns></returns>
        public string LSBComponentDetailsDAL(IMS_LSB_BRIDGE_COMPONENT_DETAIL lsb_component_detail, string Operation)
        {
            dbContext = new PMGSYEntities();
            try
            {
                if (Operation.Equals("C"))
                {
                    dbContext.IMS_LSB_BRIDGE_COMPONENT_DETAIL.Add(lsb_component_detail);
                    dbContext.SaveChanges();
                    return string.Empty;
                }
                else if (Operation.Equals("U"))
                {
                    dbContext.Entry(lsb_component_detail).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    return string.Empty;
                }

                return ("An Error Occurred While Your Processing Request.");
            }
            catch (DbUpdateException dbeux)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(dbeux, HttpContext.Current);
                return ("An Error Occurred While Your Processing Request.");
            }
            catch (OptimisticConcurrencyException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return ("An Error Occurred While Your Processing Request.");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return ("Error Occurred While Processing Request.");
            }

            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// Delete LSB Component Details
        /// </summary>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        /// <returns></returns>
        public string DeleteLSBComponentDAL(int IMS_PR_ROAD_CODE, int IMS_COMPONENT_CODE)
        {
            try
            {
                dbContext = new PMGSYEntities();
                IMS_LSB_BRIDGE_COMPONENT_DETAIL ims_component_detail = dbContext.IMS_LSB_BRIDGE_COMPONENT_DETAIL.Find(IMS_PR_ROAD_CODE, IMS_COMPONENT_CODE);
                ims_component_detail.USERID = PMGSYSession.Current.UserId;
                ims_component_detail.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                dbContext.Entry(ims_component_detail).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                dbContext.IMS_LSB_BRIDGE_COMPONENT_DETAIL.Remove(ims_component_detail);
                dbContext.SaveChanges();
                return string.Empty;
            }
            catch (DbUpdateException dbex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(dbex, HttpContext.Current);
                return ("An Error Occurred While Your Processing Request.");
            }
            catch (OptimisticConcurrencyException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return ("An Error Occurred While Your Processing Request.");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return ("Error Occurred While Processing Request.");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// Update flag as D (for DPIU finalize )
        /// </summary>
        /// <param name="IMS_PR_ROAD_CODE"></param>
        /// <returns></returns>
        public string DPIUFinalizeProposalDAL(int IMS_PR_ROAD_CODE)
        {
            try
            {
                dbContext = new PMGSYEntities();
                IMS_SANCTIONED_PROJECTS ims_sanctioned_project = dbContext.IMS_SANCTIONED_PROJECTS.Find(IMS_PR_ROAD_CODE);
                if (ims_sanctioned_project.IMS_ISCOMPLETED == "E" || ims_sanctioned_project.IMS_ISCOMPLETED == "H")
                {
                    ims_sanctioned_project.IMS_ISCOMPLETED = "D";
                }
                ims_sanctioned_project.IMS_LOCK_STATUS = "Y";
                ims_sanctioned_project.USERID = PMGSYSession.Current.UserId;
                ims_sanctioned_project.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                dbContext.Entry(ims_sanctioned_project).State = System.Data.Entity.EntityState.Modified;

                dbContext.SaveChanges();
                return string.Empty;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return "An Error Occurred While Your Processing Request.";
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        #endregion

        #region STA Mord

        /// <summary>
        /// Populate List of LSB Proposals for STA and PTA
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        public Array GetSTALSBProposalsDAL(int page, int rows, string sidx, string sord, out int totalRecords, int MAST_STATE_CODE, int IMS_YEAR, int MAST_DISTRICT_ID, int IMS_BATCH, int IMS_STREAMS, string IMS_PROPOSAL_TYPE, string IMS_PROPOSAL_STATUS)
        {
            try
            {
                dbContext = new Models.PMGSYEntities();
                List<IMS_SANCTIONED_PROJECTS> listProposals;

                //------- block of code added by shyam to ensure that only proposals under mapped districts should be populated.
                Int32 taCode = dbContext.ADMIN_TECHNICAL_AGENCY.Where(a => a.ADMIN_USER_ID == PMGSYSession.Current.UserId).Select(a => a.ADMIN_TA_CODE).FirstOrDefault();

                var taMappedDistricts = dbContext.ADMIN_TA_STATE.Where(x => x.ADMIN_TA_CODE == taCode && x.MAST_IS_ACTIVE == "Y" && x.MAST_IS_FINALIZED == "Y").Select(p => p.MAST_DISTRICT_CODE).ToList();
                //-----------------------------------------------------

                if (IMS_PROPOSAL_STATUS == "Y")
                {
                    listProposals = (PMGSYSession.Current.RoleCode == 3 ? (from c in dbContext.IMS_SANCTIONED_PROJECTS
                                                                           where
                                                                               c.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme &&  //added by shyam on 23 Oct 2013

                                                                               // populate proposals of mapped districts only for particular STA &
                                                                               // in case of common districts for STAs, if already scrutinized proposals then it should be available to STA who scrutinized those proposals.
                                                                               taMappedDistricts.Contains(c.MAST_DISTRICT_CODE) &&
                                                                               c.STA_SANCTIONED_BY == PMGSYSession.Current.UserName &&


                                                                               c.MAST_STATE_CODE == MAST_STATE_CODE &&
                                                                               c.IMS_PROPOSAL_TYPE == "L" &&

                                                                               //c.IMS_ISCOMPLETED != "E" && c.IMS_ISCOMPLETED != "H" && //commneted by Vikram as suggested by Dev Sir
                                                                               c.STA_SANCTIONED == "Y" &&

                                                                               (IMS_YEAR > 0 ? c.IMS_YEAR : 1) == (IMS_YEAR > 0 ? IMS_YEAR : 1) &&
                                                                               (MAST_DISTRICT_ID > 0 ? c.MAST_DISTRICT_CODE : 1) == (MAST_DISTRICT_ID > 0 ? MAST_DISTRICT_ID : 1) &&
                                                                               (IMS_BATCH > 0 ? c.IMS_BATCH : 1) == (IMS_BATCH > 0 ? IMS_BATCH : 1) &&
                                                                               (IMS_STREAMS > 0 ? c.IMS_COLLABORATION : 1) == (IMS_STREAMS > 0 ? IMS_STREAMS : 1)
                                                                           select c)
                                        .OrderByDescending(c => c.IMS_PR_ROAD_CODE).ToList<IMS_SANCTIONED_PROJECTS>() : (from c in dbContext.IMS_SANCTIONED_PROJECTS
                                                                                                                         join a in dbContext.ADMIN_TECHNICAL_AGENCY
                                                                                                                             on c.PTA_SANCTIONED_BY equals a.ADMIN_TA_CODE
                                                                                                                         where
                                                                                                                             c.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme &&  //added by shyam on 23 Oct 2013
                                                                                                                             a.ADMIN_USER_ID == PMGSYSession.Current.UserId &&

                                                                                                                             c.MAST_STATE_CODE == MAST_STATE_CODE &&
                                                                                                                             c.IMS_PROPOSAL_TYPE == "L" &&

                                                                                                                             c.STA_SANCTIONED == "Y" &&
                                                                                                                             c.PTA_SANCTIONED == "Y" &&

                                                                                                                             (IMS_YEAR > 0 ? c.IMS_YEAR : 1) == (IMS_YEAR > 0 ? IMS_YEAR : 1) &&
                                                                                                                             (MAST_DISTRICT_ID > 0 ? c.MAST_DISTRICT_CODE : 1) == (MAST_DISTRICT_ID > 0 ? MAST_DISTRICT_ID : 1) &&
                                                                                                                             (IMS_BATCH > 0 ? c.IMS_BATCH : 1) == (IMS_BATCH > 0 ? IMS_BATCH : 1) &&
                                                                                                                             (IMS_STREAMS > 0 ? c.IMS_COLLABORATION : 1) == (IMS_STREAMS > 0 ? IMS_STREAMS : 1)
                                                                                                                         select c)
                                                                                                                                 .OrderByDescending(c => c.IMS_PR_ROAD_CODE).ToList<IMS_SANCTIONED_PROJECTS>());
                }
                else if (IMS_PROPOSAL_STATUS == "U") //Un-Scrutinized
                {
                    listProposals = (PMGSYSession.Current.RoleCode == 3 ? (from c in dbContext.IMS_SANCTIONED_PROJECTS
                                                                           where
                                                                               c.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme &&  //added by shyam on 23 Oct 2013

                                                                               // populate proposals of mapped districts only for particular STA
                                                                               taMappedDistricts.Contains(c.MAST_DISTRICT_CODE) &&
                                                                               c.STA_SANCTIONED_BY == PMGSYSession.Current.UserName &&
                                                                               c.MAST_STATE_CODE == MAST_STATE_CODE &&
                                                                               c.IMS_PROPOSAL_TYPE == "L" &&

                                                                               //(c.IMS_ISCOMPLETED == "D" || c.IMS_ISCOMPLETED == "S") && //commneted by Vikram as suggested by Dev Sir
                                                                               c.STA_SANCTIONED == "U" &&

                                                                               (IMS_YEAR > 0 ? c.IMS_YEAR : 1) == (IMS_YEAR > 0 ? IMS_YEAR : 1) &&
                                                                               (MAST_DISTRICT_ID > 0 ? c.MAST_DISTRICT_CODE : 1) == (MAST_DISTRICT_ID > 0 ? MAST_DISTRICT_ID : 1) &&
                                                                               (IMS_BATCH > 0 ? c.IMS_BATCH : 1) == (IMS_BATCH > 0 ? IMS_BATCH : 1) &&
                                                                               (IMS_STREAMS > 0 ? c.IMS_COLLABORATION : 1) == (IMS_STREAMS > 0 ? IMS_STREAMS : 1)
                                                                           select c).OrderByDescending(c => c.IMS_PR_ROAD_CODE).ToList<IMS_SANCTIONED_PROJECTS>() : (from c in dbContext.IMS_SANCTIONED_PROJECTS
                                                                                                                                                                     where
                                                                                                                                                                         c.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme &&  //added by shyam on 23 Oct 2013

                                                                                                                                                                         c.MAST_STATE_CODE == MAST_STATE_CODE &&
                                                                                                                                                                         c.IMS_PROPOSAL_TYPE == "L" &&

                                                                                                                                                                         c.STA_SANCTIONED == "Y" &&
                                                                                                                                                                         c.PTA_SANCTIONED == "U" &&

                                                                                                                                                                         (IMS_YEAR > 0 ? c.IMS_YEAR : 1) == (IMS_YEAR > 0 ? IMS_YEAR : 1) &&
                                                                                                                                                                         (MAST_DISTRICT_ID > 0 ? c.MAST_DISTRICT_CODE : 1) == (MAST_DISTRICT_ID > 0 ? MAST_DISTRICT_ID : 1) &&
                                                                                                                                                                         (IMS_BATCH > 0 ? c.IMS_BATCH : 1) == (IMS_BATCH > 0 ? IMS_BATCH : 1) &&
                                                                                                                                                                         (IMS_STREAMS > 0 ? c.IMS_COLLABORATION : 1) == (IMS_STREAMS > 0 ? IMS_STREAMS : 1)
                                                                                                                                                                     select c).OrderByDescending(c => c.IMS_PR_ROAD_CODE).ToList<IMS_SANCTIONED_PROJECTS>());
                }
                else if (IMS_PROPOSAL_STATUS == "N")  //Pending
                {
                    listProposals = (PMGSYSession.Current.RoleCode == 3 ? (from c in dbContext.IMS_SANCTIONED_PROJECTS
                                                                           where
                                                                               c.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme &&  //added by shyam on 23 Oct 2013

                                                                               // populate proposals of mapped districts only for particular STA
                                                                               taMappedDistricts.Contains(c.MAST_DISTRICT_CODE) &&

                                                                               c.MAST_STATE_CODE == MAST_STATE_CODE &&
                                                                               c.IMS_PROPOSAL_TYPE == "L" &&

                                                                               (c.IMS_ISCOMPLETED == "D" || c.IMS_ISCOMPLETED == "S") &&
                                                                               c.STA_SANCTIONED == "N" &&

                                                                               (IMS_YEAR > 0 ? c.IMS_YEAR : 1) == (IMS_YEAR > 0 ? IMS_YEAR : 1) &&
                                                                               (MAST_DISTRICT_ID > 0 ? c.MAST_DISTRICT_CODE : 1) == (MAST_DISTRICT_ID > 0 ? MAST_DISTRICT_ID : 1) &&
                                                                               (IMS_BATCH > 0 ? c.IMS_BATCH : 1) == (IMS_BATCH > 0 ? IMS_BATCH : 1) &&
                                                                               (IMS_STREAMS > 0 ? c.IMS_COLLABORATION : 1) == (IMS_STREAMS > 0 ? IMS_STREAMS : 1)
                                                                           select c).OrderByDescending(c => c.IMS_PR_ROAD_CODE).ToList<IMS_SANCTIONED_PROJECTS>() : (from c in dbContext.IMS_SANCTIONED_PROJECTS
                                                                                                                                                                     where
                                                                                                                                                                         c.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme &&  //added by shyam on 23 Oct 2013

                                                                                                                                                                         c.MAST_STATE_CODE == MAST_STATE_CODE &&
                                                                                                                                                                         c.IMS_PROPOSAL_TYPE == "L" &&

                                                                                                                                                                         c.STA_SANCTIONED == "Y" &&
                                                                                                                                                                         c.PTA_SANCTIONED == "N" &&

                                                                                                                                                                         (IMS_YEAR > 0 ? c.IMS_YEAR : 1) == (IMS_YEAR > 0 ? IMS_YEAR : 1) &&
                                                                                                                                                                         (MAST_DISTRICT_ID > 0 ? c.MAST_DISTRICT_CODE : 1) == (MAST_DISTRICT_ID > 0 ? MAST_DISTRICT_ID : 1) &&
                                                                                                                                                                         (IMS_BATCH > 0 ? c.IMS_BATCH : 1) == (IMS_BATCH > 0 ? IMS_BATCH : 1) &&
                                                                                                                                                                         (IMS_STREAMS > 0 ? c.IMS_COLLABORATION : 1) == (IMS_STREAMS > 0 ? IMS_STREAMS : 1)
                                                                                                                                                                     select c).OrderByDescending(c => c.IMS_PR_ROAD_CODE).ToList<IMS_SANCTIONED_PROJECTS>());
                }
                else
                {
                    listProposals = (PMGSYSession.Current.RoleCode == 3 ? (from c in dbContext.IMS_SANCTIONED_PROJECTS
                                                                           where
                                                                               c.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme &&  //added by shyam on 23 Oct 2013

                                                                               // populate proposals of mapped districts only for particular STA
                                                                               taMappedDistricts.Contains(c.MAST_DISTRICT_CODE) &&

                                                                               c.MAST_STATE_CODE == MAST_STATE_CODE &&
                                                                               c.IMS_PROPOSAL_TYPE == "L" &&

                                                                               c.IMS_ISCOMPLETED != "E" &&

                                                                               (IMS_YEAR > 0 ? c.IMS_YEAR : 1) == (IMS_YEAR > 0 ? IMS_YEAR : 1) &&
                                                                               (MAST_DISTRICT_ID > 0 ? c.MAST_DISTRICT_CODE : 1) == (MAST_DISTRICT_ID > 0 ? MAST_DISTRICT_ID : 1) &&
                                                                               (IMS_BATCH > 0 ? c.IMS_BATCH : 1) == (IMS_BATCH > 0 ? IMS_BATCH : 1) &&
                                                                               (IMS_STREAMS > 0 ? c.IMS_COLLABORATION : 1) == (IMS_STREAMS > 0 ? IMS_STREAMS : 1)
                                                                           select c).OrderByDescending(c => c.IMS_PR_ROAD_CODE).ToList<IMS_SANCTIONED_PROJECTS>() : (from c in dbContext.IMS_SANCTIONED_PROJECTS
                                                                                                                                                                     where
                                                                                                                                                                         c.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme &&  //added by shyam on 23 Oct 2013

                                                                                                                                                                         c.MAST_STATE_CODE == MAST_STATE_CODE &&
                                                                                                                                                                         c.IMS_PROPOSAL_TYPE == "L" &&

                                                                                                                                                                         c.STA_SANCTIONED == "Y" &&

                                                                                                                                                                         (IMS_YEAR > 0 ? c.IMS_YEAR : 1) == (IMS_YEAR > 0 ? IMS_YEAR : 1) &&
                                                                                                                                                                         (MAST_DISTRICT_ID > 0 ? c.MAST_DISTRICT_CODE : 1) == (MAST_DISTRICT_ID > 0 ? MAST_DISTRICT_ID : 1) &&
                                                                                                                                                                         (IMS_BATCH > 0 ? c.IMS_BATCH : 1) == (IMS_BATCH > 0 ? IMS_BATCH : 1) &&
                                                                                                                                                                         (IMS_STREAMS > 0 ? c.IMS_COLLABORATION : 1) == (IMS_STREAMS > 0 ? IMS_STREAMS : 1)
                                                                                                                                                                     select c).OrderByDescending(c => c.IMS_PR_ROAD_CODE).ToList<IMS_SANCTIONED_PROJECTS>());
                }

                IQueryable<IMS_SANCTIONED_PROJECTS> query = listProposals.AsQueryable<IMS_SANCTIONED_PROJECTS>();
                totalRecords = listProposals.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        query = query.OrderBy(x => x.MASTER_STATE.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                    }
                    else
                    {
                        query = query.OrderByDescending(x => x.MASTER_STATE.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                    }
                }
                else
                {
                    query = query.OrderBy(x => x.MASTER_STATE.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }
                if (PMGSYSession.Current.RoleCode == 3)
                {
                    return query.Select(propDetails => new
                    {
                        id = propDetails.IMS_PR_ROAD_CODE.ToString(),
                        cell = new[] {  
                                    propDetails.MASTER_BLOCK.MAST_BLOCK_NAME.Trim(),
                                    propDetails.IMS_PACKAGE_ID,
                                    propDetails.IMS_ROAD_NAME == null ? "NA" :  propDetails.IMS_ROAD_NAME,
                                    propDetails.IMS_BRIDGE_NAME == null ? "NA" : propDetails.IMS_BRIDGE_NAME,
                                    propDetails.IMS_BRIDGE_LENGTH.ToString(),
                                    propDetails.IMS_BRIDGE_EST_COST_STATE.ToString(),
                                    propDetails.IMS_BRIDGE_WORKS_EST_COST.ToString(),
                                    "<a href='#'  class='ui-icon ui-icon-zoomin ui-align-center' onClick='ShowLSBDetails(\"" + propDetails.IMS_PR_ROAD_CODE.ToString().Trim() +  "$" + propDetails.IMS_LOCK_STATUS  +"\"); return false;'>Show Details</a>",
                                    "<a href='#' class='ui-icon ui-icon-plusthick ui-align-center' onclick='UploadFile(\"" + propDetails.IMS_PR_ROAD_CODE.ToString().Trim() + "\"); return false;'>Upload</a>" 
                    }
                    }).ToArray();
                }
                else
                {
                    return query.Select(propDetails => new
                    {
                        id = propDetails.IMS_PR_ROAD_CODE.ToString(),
                        cell = new[] {  
                                    propDetails.MASTER_BLOCK.MAST_BLOCK_NAME.Trim(),
                                    propDetails.IMS_PACKAGE_ID,
                                    propDetails.IMS_ROAD_NAME == null ? "NA" :  propDetails.IMS_ROAD_NAME,
                                    propDetails.IMS_BRIDGE_NAME == null ? "NA" : propDetails.IMS_BRIDGE_NAME,
                                    propDetails.IMS_BRIDGE_LENGTH.ToString(),
                                    propDetails.IMS_BRIDGE_EST_COST_STATE.ToString(),
                                    propDetails.IMS_BRIDGE_WORKS_EST_COST.ToString(),
                                    "<a href='#'  class='ui-icon ui-icon-zoomin ui-align-center' onClick='ShowLSBDetails(\"" + propDetails.IMS_PR_ROAD_CODE.ToString().Trim() +  "$" + propDetails.IMS_LOCK_STATUS  +"\"); return false;'>Show Details</a>",
                                    "<a href='#' class='ui-icon ui-icon-plusthick ui-align-center' onclick='UploadFile(\"" + propDetails.IMS_PR_ROAD_CODE.ToString().Trim() + "\"); return false;'>Upload</a>" 
                    }
                    }).ToArray();
                }

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// Populate List of LSB Proposals for MoRD 
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        public Array GetMordLSBProposalsDAL(int page, int rows, string sidx, string sord, out int totalRecords, int IMS_YEAR, int MAST_STATE_CODE, int MAST_DISTRICT_ID, int IMS_BATCH, int IMS_STREAMS, string IMS_PROPOSAL_TYPE, string IMS_PROPOSAL_STATUS, int IMS_AGENCY, string IMS_UPGRADE_CONNECT, out ProposalColumnsTotal totalColModel)
        {
            try
            {
                dbContext = new Models.PMGSYEntities();
                #region
                //List<IMS_SANCTIONED_PROJECTS> listProposals;
                //if (IMS_PROPOSAL_STATUS == "Y")
                //{
                //    listProposals = (from c in dbContext.IMS_SANCTIONED_PROJECTS
                //                        where
                //                            c.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme &&  //added by shyam on 23 Oct 2013
                //                            c.MAST_STATE_CODE == MAST_STATE_CODE &&
                //                            (MAST_DISTRICT_ID > 0 ? c.MAST_DISTRICT_CODE : 1) == (MAST_DISTRICT_ID > 0 ? MAST_DISTRICT_ID : 1) &&
                //                            c.IMS_PROPOSAL_TYPE == "L" &&
                //                            c.STA_SANCTIONED == "Y" &&
                //                            c.IMS_SANCTIONED == "Y" &&
                //                            (IMS_YEAR > 0 ? c.IMS_YEAR : 1) == (IMS_YEAR > 0 ? IMS_YEAR : 1) &&
                //                            (IMS_BATCH > 0 ? c.IMS_BATCH : 1) == (IMS_BATCH > 0 ? IMS_BATCH : 1) &&
                //                            (IMS_STREAMS > 0 ? c.IMS_STREAMS : 1) == (IMS_STREAMS > 0 ? IMS_STREAMS : 1)
                //                        select c)
                //                        .OrderByDescending(c => c.IMS_PR_ROAD_CODE).ToList<IMS_SANCTIONED_PROJECTS>();
                //}
                //else if (IMS_PROPOSAL_STATUS == "N")
                //{
                //    listProposals = (from c in dbContext.IMS_SANCTIONED_PROJECTS
                //                        where
                //                            c.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme &&  //added by shyam on 23 Oct 2013
                //                            c.MAST_STATE_CODE == MAST_STATE_CODE &&
                //                            (MAST_DISTRICT_ID > 0 ? c.MAST_DISTRICT_CODE : 1) == (MAST_DISTRICT_ID > 0 ? MAST_DISTRICT_ID : 1) &&
                //                            c.IMS_PROPOSAL_TYPE == "L" &&
                //                            c.STA_SANCTIONED == "Y" &&
                //                            (c.IMS_SANCTIONED == "N" || c.IMS_SANCTIONED == "U") &&
                //                            (IMS_YEAR > 0 ? c.IMS_YEAR : 1) == (IMS_YEAR > 0 ? IMS_YEAR : 1) &&
                //                            (IMS_BATCH > 0 ? c.IMS_BATCH : 1) == (IMS_BATCH > 0 ? IMS_BATCH : 1) &&
                //                            (IMS_STREAMS > 0 ? c.IMS_STREAMS : 1) == (IMS_STREAMS > 0 ? IMS_STREAMS : 1) 
                //                        select c).OrderByDescending(c => c.IMS_PR_ROAD_CODE).ToList<IMS_SANCTIONED_PROJECTS>();
                //}
                //else if (IMS_PROPOSAL_STATUS == "A")
                //{
                //    listProposals = (from c in dbContext.IMS_SANCTIONED_PROJECTS
                //                        where
                //                            c.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme &&  //added by shyam on 23 Oct 2013
                //                            c.MAST_STATE_CODE == MAST_STATE_CODE &&
                //                            (MAST_DISTRICT_ID > 0 ? c.MAST_DISTRICT_CODE : 1) == (MAST_DISTRICT_ID > 0 ? MAST_DISTRICT_ID : 1) &&
                //                            c.IMS_PROPOSAL_TYPE == "L" &&
                //                            c.STA_SANCTIONED == "Y" &&
                //                            (IMS_YEAR > 0 ? c.IMS_YEAR : 1) == (IMS_YEAR > 0 ? IMS_YEAR : 1) &&                                           
                //                            (MAST_DISTRICT_ID > 0 ? c.MAST_DISTRICT_CODE : 1) == (MAST_DISTRICT_ID > 0 ? MAST_DISTRICT_ID : 1) &&
                //                            (IMS_BATCH > 0 ? c.IMS_BATCH : 1) == (IMS_BATCH > 0 ? IMS_BATCH : 1) &&
                //                            (IMS_STREAMS > 0 ? c.IMS_STREAMS : 1) == (IMS_STREAMS > 0 ? IMS_STREAMS : 1)

                //                        select c).OrderByDescending(c => c.IMS_PR_ROAD_CODE).ToList<IMS_SANCTIONED_PROJECTS>();
                //}
                //else if (IMS_PROPOSAL_STATUS == "S")
                //{
                //    listProposals = (from c in dbContext.IMS_SANCTIONED_PROJECTS
                //                     where
                //                         c.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme &&  //added by shyam on 23 Oct 2013
                //                         c.MAST_STATE_CODE == MAST_STATE_CODE &&
                //                         c.IMS_PROPOSAL_TYPE == "L" &&
                //                         c.STA_SANCTIONED == "N" &&
                //                         c.IMS_SANCTIONED == "N" &&
                //                         (IMS_YEAR > 0 ? c.IMS_YEAR : 1) == (IMS_YEAR > 0 ? IMS_YEAR : 1) &&
                //                         //(IMS_PROPOSAL_STATUS != "A" ? c.STA_SANCTIONED : "1") == (IMS_PROPOSAL_STATUS != "A" ? IMS_PROPOSAL_STATUS : "1") &&
                //                         (MAST_DISTRICT_ID > 0 ? c.MAST_DISTRICT_CODE : 1) == (MAST_DISTRICT_ID > 0 ? MAST_DISTRICT_ID : 1) &&
                //                         (IMS_BATCH > 0 ? c.IMS_BATCH : 1) == (IMS_BATCH > 0 ? IMS_BATCH : 1) &&
                //                         (IMS_STREAMS > 0 ? c.IMS_STREAMS : 1) == (IMS_STREAMS > 0 ? IMS_STREAMS : 1)

                //                     select c).OrderByDescending(c => c.IMS_PR_ROAD_CODE).ToList<IMS_SANCTIONED_PROJECTS>();
                //}
                //else
                //{
                //    listProposals = (from c in dbContext.IMS_SANCTIONED_PROJECTS
                //                        where
                //                            c.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme &&  //added by shyam on 23 Oct 2013
                //                            c.MAST_STATE_CODE == MAST_STATE_CODE &&
                //                            (MAST_DISTRICT_ID > 0 ? c.MAST_DISTRICT_CODE : 1) == (MAST_DISTRICT_ID > 0 ? MAST_DISTRICT_ID : 1) &&
                //                            (IMS_YEAR > 0 ? c.IMS_YEAR : 1) == (IMS_YEAR > 0 ? IMS_YEAR : 1) &&
                //                            (IMS_BATCH > 0 ? c.IMS_BATCH : 1) == (IMS_BATCH > 0 ? IMS_BATCH : 1) &&
                //                            (IMS_STREAMS > 0 ? c.IMS_STREAMS : 1) == (IMS_STREAMS > 0 ? IMS_STREAMS : 1) &&
                //                            c.IMS_PROPOSAL_TYPE == "L" &&
                //                            c.STA_SANCTIONED == "Y" &&
                //                            (c.IMS_SANCTIONED == IMS_PROPOSAL_STATUS)
                //                        select c).OrderByDescending(c => c.IMS_PR_ROAD_CODE).ToList<IMS_SANCTIONED_PROJECTS>();
                //}

                //IQueryable<IMS_SANCTIONED_PROJECTS> query = listProposals.AsQueryable<IMS_SANCTIONED_PROJECTS>();
                //totalRecords = listProposals.Count();

                //if (sidx.Trim() != string.Empty)
                //{
                //    if (sord.ToString() == "asc")
                //    {
                //        query = query.OrderBy(x => x.MASTER_STATE.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                //    }
                //    else
                //    {
                //        query = query.OrderByDescending(x => x.MASTER_STATE.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                //    }
                //}
                //else
                //{
                //    query = query.OrderBy(x => x.MASTER_STATE.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                //}
                //return query.Select(propDetails => new
                //{
                //    id = propDetails.IMS_PR_ROAD_CODE.ToString(),
                //    cell = new[] {         
                //                    propDetails.MASTER_DISTRICT.MAST_DISTRICT_NAME.Trim(),                            
                //                    propDetails.MASTER_BLOCK.MAST_BLOCK_NAME.Trim(),
                //                    propDetails.IMS_PACKAGE_ID,
                //                    propDetails.IMS_ROAD_NAME == null ? "NA" :  propDetails.IMS_ROAD_NAME,
                //                    propDetails.IMS_BRIDGE_NAME == null ? "NA" : propDetails.IMS_BRIDGE_NAME,
                //                    propDetails.IMS_BRIDGE_LENGTH.ToString(),
                //                    propDetails.IMS_BRIDGE_EST_COST_STATE.ToString(),
                //                    propDetails.IMS_BRIDGE_WORKS_EST_COST.ToString(),
                //                    "<a href='#'  class='ui-icon ui-icon-zoomin ui-align-center' onClick='ShowLSBDetails(\"" + propDetails.IMS_PR_ROAD_CODE.ToString().Trim() + "$" + propDetails.IMS_LOCK_STATUS  + "\"); return false;'>Show Details</a>"
                //    }
                //}).ToArray();
                #endregion

                string MRDStatus = string.Empty;
                string STAStatus = string.Empty;

                switch (IMS_PROPOSAL_STATUS)
                {
                    case "N":
                        MRDStatus = "N";
                        STAStatus = "Y";
                        break;
                    case "Y":
                        MRDStatus = "Y";
                        STAStatus = "%";
                        break;
                    case "U":
                        MRDStatus = "U";
                        STAStatus = "%";
                        break;
                    case "R":
                        MRDStatus = "R";
                        STAStatus = "%";
                        break;
                    case "D":
                        MRDStatus = "D";
                        STAStatus = "%";
                        break;
                    case "S":
                        MRDStatus = "N";
                        STAStatus = "N";
                        break;
                    case "A":
                        MRDStatus = "%";
                        STAStatus = "Y";
                        break;
                    default:
                        MRDStatus = "%";
                        STAStatus = "%";
                        break;
                }

                if (PMGSYSession.Current.RoleCode == 36 || PMGSYSession.Current.RoleCode == 47)
                {
                    var queryAgency = (from ma in dbContext.MASTER_AGENCY
                                       join md in dbContext.ADMIN_DEPARTMENT on ma.MAST_AGENCY_CODE equals md.MAST_AGENCY_CODE
                                       where md.MAST_STATE_CODE == PMGSYSession.Current.StateCode &&
                                       md.MAST_ND_TYPE == "S" &&
                                       md.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode
                                       select new
                                       {
                                           ma.MAST_AGENCY_CODE
                                       }).FirstOrDefault();
                    IMS_AGENCY = queryAgency == null ? 0 : queryAgency.MAST_AGENCY_CODE;
                }
                var listProposals = dbContext.USP_GET_PROPOSAL_LIST(1, MAST_STATE_CODE, (MAST_DISTRICT_ID < 0 ? 0 : MAST_DISTRICT_ID), 0, (IMS_YEAR < 0 ? 0 : IMS_YEAR), (IMS_BATCH < 0 ? 0 : IMS_BATCH), (IMS_STREAMS < 0 ? 0 : IMS_STREAMS), IMS_AGENCY, 0, 0, "%", "%", "%", "%", STAStatus, MRDStatus, "L", ((IMS_UPGRADE_CONNECT == "0" || IMS_UPGRADE_CONNECT == null) ? "%" : IMS_UPGRADE_CONNECT), PMGSYSession.Current.PMGSYScheme, (short)PMGSYSession.Current.RoleCode).ToList();
                IQueryable<USP_GET_PROPOSAL_LIST_Result> query = listProposals.AsQueryable<USP_GET_PROPOSAL_LIST_Result>();

                ProposalColumnsTotal colTotal = new ProposalColumnsTotal();
                colTotal.TOT_COST = listProposals.Sum(m => m.TOTAL_COST).HasValue ? listProposals.Sum(m => m.TOTAL_COST).Value : 0; ;//listProposals.Sum(m => m.ROAD_AMT).HasValue ? listProposals.Sum(m => m.ROAD_AMT).Value : 0;
                colTotal.TOT_HAB1000 = listProposals.Sum(m => m.POP1000);
                colTotal.TOT_HAB250 = listProposals.Sum(m => m.POP250);
                colTotal.TOT_HAB499 = listProposals.Sum(m => m.POP499);
                colTotal.TOT_HAB999 = listProposals.Sum(m => m.POP999);
                colTotal.TOT_HABS = listProposals.Sum(m => m.TOTAL_HABS).HasValue ? listProposals.Sum(m => m.TOTAL_HABS).Value : 0;
                colTotal.TOT_HIGHER_SPEC = listProposals.Sum(m => m.HIGHER_SPECS);
                colTotal.TOT_MANE_COST = listProposals.Sum(m => m.MAINT_AMT).HasValue ? listProposals.Sum(m => m.MAINT_AMT).Value : 0;
                colTotal.TOT_MORD_COST = listProposals.Sum(m => m.BRIDGE_AMT).Value;
                colTotal.TOT_PAV_LENGTH = listProposals.Sum(m => m.ROAD_LENGTH).HasValue ? listProposals.Sum(m => m.ROAD_LENGTH).Value : 0;
                colTotal.TOT_RENEWAL_COST = listProposals.Sum(m => m.RENEWAL_AMT);
                colTotal.TOT_STATE_COST = listProposals.Sum(m => m.BRIDGE_STATE).HasValue ? (decimal)listProposals.Sum(m => m.BRIDGE_STATE) : 0;

                colTotal.STATE_SHARE_COST = listProposals.Sum(m => m.STATE_SHARE_2015);
                colTotal.MORD_SHARE_COST = listProposals.Sum(m => m.MORD_SHARE_2015);
                colTotal.TOTAL_STATE_SHARE = listProposals.Sum(m => m.TOTAL_STATE_SHARE);
                colTotal.TOTAL_SHARE_COST = listProposals.Sum(m => m.TOTAL_SHARE_COST);

                totalColModel = colTotal;


                totalRecords = listProposals.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        query = query.OrderBy(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                    }
                    else
                    {
                        query = query.OrderByDescending(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                    }
                }
                else
                {
                    query = query.OrderBy(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }
                return query.Select(propDetails => new
                {
                    id = propDetails.IMS_PR_ROAD_CODE.ToString(),
                    cell = new[] {         
                                    propDetails.MAST_DISTRICT_NAME.Trim(),                            
                                    propDetails.MAST_BLOCK_NAME.Trim(),
                                    propDetails.IMS_PACKAGE_ID.ToString(),             
                                    propDetails.IMS_YEAR.ToString(),
                                    propDetails.IMS_ROAD_NAME.ToString(),
                                    propDetails.ROAD_NAME.ToString(),
                                    propDetails.ROAD_LENGTH.ToString() ,
                                    Math.Round(Convert.ToDecimal(propDetails.BRIDGE_AMT),2).ToString(),
                                    Math.Round(Convert.ToDecimal(propDetails.BRIDGE_STATE),2).ToString(),
                                    Math.Round(Convert.ToDecimal(propDetails.HIGHER_SPECS),2).ToString(),
                                    //Math.Round(Convert.ToDecimal(propDetails.BRIDGE_AMT + propDetails.BRIDGE_STATE),2).ToString(),
                                    Math.Round(Convert.ToDecimal(propDetails.TOTAL_COST),2).ToString(),
                                    Math.Round(Convert.ToDecimal(propDetails.MAINT_AMT),2).ToString(),
                                    Math.Round(Convert.ToDecimal(propDetails.RENEWAL_AMT),2).ToString(),

                                    propDetails.FUND_SHARING_RATIO.ToString(),
                                    Math.Round(Convert.ToDecimal(propDetails.STATE_SHARE_2015),2).ToString(),
                                    Math.Round(Convert.ToDecimal(propDetails.MORD_SHARE_2015),2).ToString(),
                                    Math.Round(Convert.ToDecimal(propDetails.TOTAL_STATE_SHARE),2).ToString(),
                                    Math.Round(Convert.ToDecimal(propDetails.TOTAL_SHARE_COST),2).ToString(),
                                    propDetails.STA_SCRUTINY.ToString(),
                                    propDetails.PTA_SCRUTINY.ToString(),
                                    propDetails.PROPOSAL_STATUS.ToString(),
                                    "<a href='#'  class='ui-icon ui-icon-zoomin ui-align-center' onClick='ShowLSBDetails(\"" + propDetails.IMS_PR_ROAD_CODE.ToString().Trim() + "$" + propDetails.IMS_LOCK_STATUS + "\"); return false;'>Show Details</a>"
                    }
                }).ToArray();

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                totalColModel = null;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// Finalization (i.e. Scrutinize) of LSB Proposal at STA Level
        /// </summary>
        /// <param name="staSanctionViewModel"></param>
        /// <returns></returns>
        public string STAFinalizeLSBProposalDAL(StaLSBSanctionViewModel staSanctionViewModel, string ProposalStatus)
        {
            try
            {
                dbContext = new Models.PMGSYEntities();
                IMS_SANCTIONED_PROJECTS ims_Sanctioned_Project = dbContext.IMS_SANCTIONED_PROJECTS.Find(staSanctionViewModel.IMS_PR_ROAD_CODE);

                ims_Sanctioned_Project.STA_SANCTIONED = ProposalStatus;         //Scrutinized or Unscrutinized
                ims_Sanctioned_Project.STA_SANCTIONED_BY = PMGSYSession.Current.UserName;

                if (ims_Sanctioned_Project.IMS_ISCOMPLETED == "D")
                {
                    ims_Sanctioned_Project.IMS_ISCOMPLETED = "S";                   // S is for STA Sanctioned
                }

                if (ProposalStatus == "Y")
                {
                    ims_Sanctioned_Project.IMS_STA_REMARKS = staSanctionViewModel.MS_STA_REMARKS.Trim();
                    ims_Sanctioned_Project.STA_SANCTIONED_DATE = Convert.ToDateTime(staSanctionViewModel.STA_SANCTIONED_DATE);
                }
                else if (ProposalStatus == "U")
                {
                    ims_Sanctioned_Project.IMS_STA_REMARKS = staSanctionViewModel.MS_STA_UnScrutinised_REMARKS.Trim();
                    ims_Sanctioned_Project.STA_SANCTIONED_DATE = Convert.ToDateTime(staSanctionViewModel.STA_UNSCRUTINY_DATE);
                }

                ims_Sanctioned_Project.USERID = PMGSYSession.Current.UserId;
                ims_Sanctioned_Project.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                dbContext.Entry(ims_Sanctioned_Project).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                return string.Empty;
            }
            catch (DbUpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return ("An Error Occurred While Processing YourRequest.");
            }
            catch (OptimisticConcurrencyException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return ("An Error Occurred While Processing Your Request.");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return ("Error Occurred While Processing Request.");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// Update Sanction cost details for particular LSB Prposal at MoRD Level
        /// </summary>
        /// <param name="mordSanctionViewModel"></param>
        /// <returns></returns>
        public string UpdateMordLSBSanctionDetailsDAL(MordLSBSanctionViewModel mordSanctionViewModel)
        {
            try
            {
                dbContext = new PMGSYEntities();
                IMS_SANCTIONED_PROJECTS ims_Sanctioned_Project = dbContext.IMS_SANCTIONED_PROJECTS.Find(mordSanctionViewModel.IMS_PR_ROAD_CODE);
                ims_Sanctioned_Project.IMS_SANCTIONED_BY = mordSanctionViewModel.IMS_SANCTIONED_BY;

                /// Update Costs Only If Proposal is Sanctioned
                if (mordSanctionViewModel.IMS_SANCTIONED.Equals("Y"))
                {
                    ims_Sanctioned_Project.IMS_SANCTIONED_BW_AMT = mordSanctionViewModel.IMS_SANCTIONED_BW_AMT;
                    ims_Sanctioned_Project.IMS_SANCTIONED_BS_AMT = mordSanctionViewModel.IMS_SANCTIONED_BS_AMT;

                    //PMGSY Scheme2
                    mordSanctionViewModel.IMS_SANCTIONED_HS_AMT = mordSanctionViewModel.IMS_SANCTIONED_HS_AMT;
                }

                if (ims_Sanctioned_Project.IMS_ISCOMPLETED == "S" || ims_Sanctioned_Project.IMS_ISCOMPLETED == "P")
                {
                    ims_Sanctioned_Project.IMS_ISCOMPLETED = "M";
                }

                //ims_Sanctioned_Project.IMS_SANCTIONED_DATE = DateTime.Now;
                ims_Sanctioned_Project.IMS_SANCTIONED_DATE = Convert.ToDateTime(mordSanctionViewModel.IMS_SANCTIONED_DATE);

                ims_Sanctioned_Project.IMS_PROG_REMARKS = mordSanctionViewModel.IMS_PROG_REMARKS;

                ims_Sanctioned_Project.IMS_SANCTIONED = mordSanctionViewModel.IMS_SANCTIONED;

                // For Reconsider Action, take a reason
                if (ims_Sanctioned_Project.IMS_SANCTIONED == "R")
                {
                    // Unlock the Proposal
                    ims_Sanctioned_Project.IMS_LOCK_STATUS = "N";
                    ims_Sanctioned_Project.IMS_REASON = mordSanctionViewModel.IMS_REASON;
                }
                // For Drop take a Action
                else if (ims_Sanctioned_Project.IMS_SANCTIONED == "D")
                {
                    ims_Sanctioned_Project.IMS_LOCK_STATUS = "Y";
                    ims_Sanctioned_Project.IMS_REASON = mordSanctionViewModel.IMS_REASON;
                }
                else
                {
                    ims_Sanctioned_Project.IMS_LOCK_STATUS = "Y";
                    ims_Sanctioned_Project.IMS_REASON = null;
                }



                ims_Sanctioned_Project.USERID = PMGSYSession.Current.UserId;
                ims_Sanctioned_Project.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                dbContext.Entry(ims_Sanctioned_Project).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                return string.Empty;
            }
            catch (DbUpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return ("An Error Occurred While Processing Your Request.");
            }
            catch (OptimisticConcurrencyException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return ("An Error Occurred While Processing Your Request.");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return ("Error Occurred While Processing Request.");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        #endregion

        #region PTA

        /// <summary>
        /// Finalization (i.e. Scrutinize) of LSB Proposal at PTA Level
        /// </summary>
        /// <param name="staSanctionViewModel"></param>
        /// <returns></returns>
        public string PTAFinalizeLSBProposalDAL(PtaLSBSanctionViewModel ptaSanctionViewModel, string ProposalStatus)
        {
            try
            {
                dbContext = new Models.PMGSYEntities();
                IMS_SANCTIONED_PROJECTS ims_Sanctioned_Project = dbContext.IMS_SANCTIONED_PROJECTS.Find(ptaSanctionViewModel.IMS_PR_ROAD_CODE);

                ims_Sanctioned_Project.PTA_SANCTIONED = ProposalStatus;         //Scrutinized or Unscrutinized
                ims_Sanctioned_Project.PTA_SANCTIONED_BY = dbContext.ADMIN_TECHNICAL_AGENCY.Where(b => b.ADMIN_USER_ID == PMGSYSession.Current.UserId).Select(a => a.ADMIN_TA_CODE).FirstOrDefault();
                ims_Sanctioned_Project.PTA_SANCTIONED_DATE = Convert.ToDateTime(ptaSanctionViewModel.PTA_SANCTIONED_DATE);

                if (ProposalStatus == "Y")
                {
                    ims_Sanctioned_Project.IMS_PTA_REMARKS = ptaSanctionViewModel.MS_PTA_REMARKS.Trim();

                }
                else if (ProposalStatus == "U")
                {
                    ims_Sanctioned_Project.IMS_PTA_REMARKS = ptaSanctionViewModel.MS_PTA_UnScrutinised_REMARKS.Trim();
                }
                ims_Sanctioned_Project.USERID = PMGSYSession.Current.UserId;
                ims_Sanctioned_Project.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                dbContext.Entry(ims_Sanctioned_Project).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                return string.Empty;
            }
            catch (DbUpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return ("An Error Occurred While Your Processing Request.");
            }
            catch (OptimisticConcurrencyException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return ("An Error Occurred While Your Processing Request.");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return ("Error Occurred While Processing Request.");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        #endregion

        #region Unlock LSB Proposal

        /// <summary>
        /// Update the Unlocked Proposal
        /// </summary>
        /// <param name="unlockLsbViewModel"></param>
        /// <returns></returns>
        public string UpdateUnlockedProposalDAL(UnlockLSBViewModel unlockLsbViewModel)
        {
            dbContext = new PMGSYEntities();
            try
            {
                IMS_SANCTIONED_PROJECTS objProposal = new IMS_SANCTIONED_PROJECTS();
                objProposal = dbContext.IMS_SANCTIONED_PROJECTS.Find(unlockLsbViewModel.IMS_PR_ROAD_CODE);

                //objProposal.IMS_BATCH = unlockLsbViewModel.IMS_BATCH;
                objProposal.IMS_COLLABORATION = unlockLsbViewModel.IMS_COLLABORATION;
                if (unlockLsbViewModel.isPaymentDone == false)
                {
                    objProposal.IMS_STREAMS = unlockLsbViewModel.IMS_STREAMS;
                }

                //below code replaced as per suggestion from  pankaj sir
 
                //objProposal.IMS_ROAD_NAME = unlockLsbViewModel.IMS_ROAD_NAME == null ? string.Empty : unlockLsbViewModel.IMS_ROAD_NAME;
                //objProposal.IMS_ROAD_FROM = unlockLsbViewModel.IMS_ROAD_FROM == null ? string.Empty : unlockLsbViewModel.IMS_ROAD_FROM;
                //objProposal.IMS_ROAD_TO = unlockLsbViewModel.IMS_ROAD_TO == null ? string.Empty : unlockLsbViewModel.IMS_ROAD_TO;

                // done by as per suggestion from Pankanj sir start
                if (unlockLsbViewModel.IMS_ROAD_NAME != null)
                    objProposal.IMS_ROAD_NAME = unlockLsbViewModel.IMS_ROAD_NAME;

                if (unlockLsbViewModel.IMS_ROAD_FROM != null)
                    objProposal.IMS_ROAD_FROM = unlockLsbViewModel.IMS_ROAD_FROM;

                if (unlockLsbViewModel.IMS_ROAD_TO != null)
                    objProposal.IMS_ROAD_TO = unlockLsbViewModel.IMS_ROAD_TO;

                // done by as per suggestion from Pankanj sir end

                objProposal.IMS_BRIDGE_NAME = unlockLsbViewModel.IMS_BRIDGE_NAME;
                objProposal.IMS_BRIDGE_LENGTH = unlockLsbViewModel.IMS_BRIDGE_LENGTH;


                // Below two lines are uncommented on 09 DEC 2020 as per suggestion by Pankaj Sir to update costs. (Mail : VZNM- RCPLWE 20​20-21 Bridge Proposa​ls-Reg​ Dated 11-24-2020 07:02 PM by  	OMMAS Help <ommashelp2018@gmail.com>)
                objProposal.IMS_SANCTIONED_BS_AMT = unlockLsbViewModel.IMS_SANCTIONED_BS_AMT;
                objProposal.IMS_SANCTIONED_BW_AMT = unlockLsbViewModel.IMS_SANCTIONED_BW_AMT; 

                


                int Scheme = PMGSYSession.Current.PMGSYScheme;
                if (Scheme == 1)
                {
                    objProposal.IMS_SANCTIONED_BW_AMT = unlockLsbViewModel.IMS_SANCTIONED_BW_AMT;
                }

                objProposal.IMS_SANCTIONED_MAN_AMT1 = unlockLsbViewModel.IMS_SANCTIONED_MAN_AMT1;
                objProposal.IMS_SANCTIONED_MAN_AMT2 = unlockLsbViewModel.IMS_SANCTIONED_MAN_AMT2;
                objProposal.IMS_SANCTIONED_MAN_AMT3 = unlockLsbViewModel.IMS_SANCTIONED_MAN_AMT3;
                objProposal.IMS_SANCTIONED_MAN_AMT4 = unlockLsbViewModel.IMS_SANCTIONED_MAN_AMT4;
                objProposal.IMS_SANCTIONED_MAN_AMT5 = unlockLsbViewModel.IMS_SANCTIONED_MAN_AMT5;

                // PMGSY Scheme-II
                objProposal.IMS_IS_HIGHER_SPECIFICATION = unlockLsbViewModel.IMS_IS_HIGHER_SPECIFICATION;
                if (objProposal.IMS_IS_HIGHER_SPECIFICATION != null && unlockLsbViewModel.IMS_IS_HIGHER_SPECIFICATION.Equals("Y"))
                {
                    objProposal.IMS_HIGHER_SPECIFICATION_COST = unlockLsbViewModel.IMS_HIGHER_SPECIFICATION_COST;
                    objProposal.IMS_SANCTIONED_HS_AMT = unlockLsbViewModel.IMS_HIGHER_SPECIFICATION_COST;
                }
                else
                {
                    objProposal.IMS_HIGHER_SPECIFICATION_COST = null;
                    objProposal.IMS_SANCTIONED_HS_AMT = null;
                }

                if (PMGSYSession.Current.PMGSYScheme == 2)
                {
                    objProposal.IMS_RENEWAL_COST = unlockLsbViewModel.IMS_RENEWAL_COST == null ? 0 : unlockLsbViewModel.IMS_RENEWAL_COST.Value;
                }

                #region FUND_SHARING_RATIO_PMGSY_SCHEME_1

                objProposal.IMS_STATE_SHARE_2015 = unlockLsbViewModel.IMS_STATE_SHARE_2015;
                objProposal.IMS_MORD_SHARE_2015 = unlockLsbViewModel.IMS_MORD_SHARE_2015;
                objProposal.IMS_SHARE_PERCENT_2015 = unlockLsbViewModel.IMS_SHARE_PERCENT_2015;

                #endregion

                objProposal.IMS_SHARE_PERCENT = unlockLsbViewModel.IMS_SHARE_PERCENT;
                objProposal.USERID = PMGSYSession.Current.UserId;
                objProposal.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                dbContext.Entry(objProposal).State = System.Data.Entity.EntityState.Modified;

                // Unlock By M - Mord Unlock
                //if (dbContext.IMS_LOCK_DETAILS.Where(a => a.IMS_PR_ROAD_CODE == unlockLsbViewModel.IMS_PR_ROAD_CODE && a.IMS_UNLOCK_BY == "M").Any())
                //{
                //    IMS_LOCK_DETAILS ims_lock_details = dbContext.IMS_LOCK_DETAILS.Where(a => a.IMS_PR_ROAD_CODE == unlockLsbViewModel.IMS_PR_ROAD_CODE
                //                                        && a.IMS_TRANSACTION_NO == (dbContext.IMS_LOCK_DETAILS.Where(b => b.IMS_PR_ROAD_CODE == unlockLsbViewModel.IMS_PR_ROAD_CODE && a.IMS_UNLOCK_BY == "M").Max(b => b.IMS_TRANSACTION_NO))
                //                                        ).First();
                //    ims_lock_details.IMS_DATA_FINALIZED = "Y";
                //    // M - Manual Unlock
                //    ims_lock_details.IMS_UNLOCK_BY = "M";
                //    dbContext.Entry(ims_lock_details).State = System.Data.Entity.EntityState.Modified;
                //}

                dbContext.SaveChanges();
                return string.Empty;
            }
            catch (DbUpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return ("An Error Occurred While Your Processing Request.");
            }
            catch (OptimisticConcurrencyException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return ("An Error Occurred While Your Processing Request.");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return ("Error Occurred While Processing Request.");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// DAL for Getting Bulk Sanction Details
        /// </summary>
        /// <param name="IMS_PR_ROAD_CODES"></param>
        /// <returns></returns>
        public MordLSBSanctionViewModel GetBulkMordDetailDAL(string IMS_PR_ROAD_CODES)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<int> lstRoadCodes = IMS_PR_ROAD_CODES.Split(',').Select(x => int.Parse(x)).ToArray().ToList<int>();

                MordLSBSanctionViewModel ObjMordSanctionViewModel = new MordLSBSanctionViewModel();
                ObjMordSanctionViewModel.IS_SANCTIONABLE = true;
                ObjMordSanctionViewModel.IS_EXECUTION_STARTED = "N";

                return ObjMordSanctionViewModel;
            }
            catch (Exception)
            {
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

        #endregion
    }

    public interface ILSBProposalDAL
    {
        #region LSB Proposal
        string SaveLSBProposalDAL(IMS_SANCTIONED_PROJECTS objProposal);
        Array GetLSBProposalsDAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int MAST_STATE_CODE, int MAST_DISTRICT_CODE, int IMS_YEAR, int MAST_BLOCK_CODE, int IMS_BATCH, int IMS_STREAMS, String ProposalType, string IMS_PROPOSAL_STATUS, string IMS_UPGRADE_CONNECT, out ProposalColumnsTotal model);
        Array GetLSBProposalsForSRRDADAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int MAST_STATE_CODE, int MAST_DISTRICT_CODE, int IMS_YEAR, int IMS_BATCH, int IMS_STREAMS, String ProposalType, string IMS_PROPOSAL_STATUS, string IMS_UPGRADE_CONNECT, int MAST_BLOCK_CODE, out ProposalColumnsTotal model);
        string UpdateLSBProposalDAL(IMS_SANCTIONED_PROJECTS ims_sanctioned_projects);
        string DeleteLSBProposalDAL(int IMS_PR_ROAD_CODE);
        string LSBOtherDetailsDAL(IMS_LSB_BRIDGE_DETAIL lsb_bridge_detail, string Operation);
        string DeleteLSBOthDetailsBAL(int IMS_PR_ROAD_CODE);
        string LSBComponentDetailsDAL(IMS_LSB_BRIDGE_COMPONENT_DETAIL lsb_component_detail, string Operation);
        Array LSBComponentList(int roadId, int? page, int? rows, string sidx, string sord, out long totalRecords);
        string DeleteLSBComponentDAL(int IMS_PR_ROAD_CODE, int IMS_COMPONENT_CODE);
        string DPIUFinalizeProposalDAL(int IMS_PR_ROAD_CODE);
        #endregion

        #region STA Mord

        Array GetSTALSBProposalsDAL(int page, int rows, string sidx, string sord, out int totalRecords, int MAST_STATE_CODE, int IMS_YEAR, int MAST_DISTRICT_ID, int IMS_BATCH, int IMS_STREAMS, string IMS_PROPOSAL_TYPE, string IMS_PROPOSAL_STATUS);
        string STAFinalizeLSBProposalDAL(StaLSBSanctionViewModel staSanctionViewModel, String ProposalStatus);
        Array GetMordLSBProposalsDAL(int page, int rows, string sidx, string sord, out int totalRecords, int IMS_YEAR, int MAST_STATE_ID, int MAST_DISTRICT_ID, int IMS_BATCH, int IMS_STREAMS, string IMS_PROPOSAL_TYPE, string IMS_PROPOSAL_STATUS, int IMS_AGENCY, string IMS_UPGRADE_CONNECT, out ProposalColumnsTotal totalColModel);
        string UpdateMordLSBSanctionDetailsDAL(MordLSBSanctionViewModel mordSanctionViewModel);
        MordLSBSanctionViewModel GetBulkMordDetailDAL(string IMS_PR_ROAD_CODES);

        #endregion

        #region PTA
        string PTAFinalizeLSBProposalDAL(PtaLSBSanctionViewModel ptaSanctionViewModel, string ProposalStatus);
        #endregion

        #region Unlock LSB Proposal
        string UpdateUnlockedProposalDAL(UnlockLSBViewModel ims_sanctioned_projects);
        #endregion
    }
}
