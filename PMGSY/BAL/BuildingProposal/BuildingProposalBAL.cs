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

using PMGSY.DAL.BuildingProposal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.Models;
using PMGSY.Models.BuildingProposal;

using PMGSY.Extensions;
using ImageResizer;
using System.IO;
using System.Drawing;
using System.Web.Mvc;
using System.Configuration;
//using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects;

namespace PMGSY.BAL.BuildingProposal
{
    public class BuildingProposalBAL : IBuildingProposalBAL
    {
       
        IBuildingProposalDAL buildingProposalDAL;
       
        public BuildingProposalBAL()
        {
            buildingProposalDAL = new BuildingProposalDAL();
         
        }
        
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
       
        public Array GetBuildingProposalsBAL(int page, int rows, string sidx, string sord, out int totalRecords, int state, int district, int syear, int block, int batch, int stream, string proptype, int eacode, string propstatus, string propconnect,byte scheme,int roleCode, string filters)
        {
            //var buildingProposalList = buildingProposalDAL.BuildingProposalList(state, district, block, syear, batch, agency, stream, proptype, propstatus, propconnect, "1", "1", "1");
            BuildingProposalViewModel buildingProposalViewModel = new BuildingProposalViewModel { 
                MAST_STATE_CODE=state,
                MAST_DISTRICT_CODE=district,
                MAST_BLOCK_CODE=block,
                IMS_YEAR=syear,
                IMS_BATCH=batch,
                IMS_COLLABORATION=stream,
                IMS_ISCOMPLETED=propstatus,
                IMS_UPGRADE_CONNECT=propconnect,
                PMGSYScheme = scheme,
                RoleCode=roleCode,
                MAST_DPIU_CODE=eacode
            
            };


            var buildingProposalList = buildingProposalDAL.BuildingProposalListPIU(buildingProposalViewModel);
            totalRecords = buildingProposalList.Count;

            var itemJson= buildingProposalList.Select(propDetails => new
            {
                id = propDetails.IMS_PR_ROAD_CODE.ToString(),
                cell = new[] {     
                                    //propDetails.DistrictName.ToString(),                            
                                    propDetails.MAST_BLOCK_NAME.ToString(),
                                    propDetails.IMS_PACKAGE_ID.ToString(),                                    
                                    propDetails.IMS_YEAR_FINANCIAL.ToString(),
                                    propDetails.IMS_ROAD_NAME.ToString(),                                    
                                    Math.Round(Convert.ToDecimal(propDetails.IMS_PAV_EST_COST),2).ToString(),
                                    propDetails.IMS_BATCH.ToString(),
                                   // propDetails.MAST_FUNDING_AGENCY_NAME,
                                   // propDetails.STA_SANCTIONED=="Y"?propDetails.STA_SANCTIONED_BY+"("+propDetails.STA_SANCTIONED_DATE+")":"No",
                                    propDetails.IMS_SANCTIONED=="Y"?propDetails.IMS_SANCTIONED_BY+"("+propDetails.IMS_SANCTIONED_DATE+")":"No",
                                   // propDetails.PTA_SANCTIONED=="Y"?propDetails.PTA_SANCTIONED_BY+"("+propDetails.PTA_SANCTIONED_DATE+")":"No",
                                    
                                   "<a href='#'  class='ui-icon ui-icon-zoomin ui-align-center' onClick='ShowBuildingDetails(" + propDetails.IMS_PR_ROAD_CODE + "); return false;'>Show Details</a>",                                    
                                    ( propDetails.IMS_ISCOMPLETED == "E" && propDetails.IMS_LOCK_STATUS.ToUpper() == "N" ) ? "<a href='#'  class='ui-icon ui-icon-unlocked ui-align-center' onClick='PIUFinalizeBuildingDetails(" + propDetails.IMS_PR_ROAD_CODE + "); return false;'>Finalize</a>": "<a href='#' class='ui-icon ui-icon-locked ui-align-center'>",                                    
                                   //(( propDetails.IMS_ISCOMPLETED == "D" || propDetails.IMS_ISCOMPLETED == "M" || propDetails.IMS_SANCTIONED=="Y")  && propDetails.IMS_LOCK_STATUS.ToUpper() == "Y" )?"<a href='#' class='ui-icon ui-icon-locked ui-align-center'>": "<a href='#'  class='ui-icon ui-icon-plusthick ui-align-center' onClick='BuildingUpload(" + propDetails.IMS_PR_ROAD_CODE +"); return false;'>Upload</a>",
                                   (( propDetails.IMS_ISCOMPLETED == "D" || propDetails.IMS_ISCOMPLETED == "M" || propDetails.IMS_SANCTIONED=="Y")  && propDetails.IMS_LOCK_STATUS.ToUpper() == "Y" )?"<a href='#' class='ui-icon ui-icon-locked ui-align-center'>": "<a href='#'  class='ui-icon ui-icon-pencil ui-align-center' onClick='BuildingUpdate(" + propDetails.IMS_PR_ROAD_CODE +"); return false;'>Edit</a>",
                                   (( propDetails.IMS_ISCOMPLETED == "D" || propDetails.IMS_ISCOMPLETED == "M" || propDetails.IMS_SANCTIONED=="Y")  && propDetails.IMS_LOCK_STATUS.ToUpper() == "Y" )?"<a href='#' class='ui-icon ui-icon-locked ui-align-center'>": "<a href='#' class='ui-icon ui-icon-trash ui-align-center' onclick='BuildingDelete(" + propDetails.IMS_PR_ROAD_CODE + "); return false;'>Delete</a>" 
                   }
            }).ToArray();

            return itemJson;
        }


        public Array GetMoRDBuildingProposalsBAL(int page, int rows, string sidx, string sord, out int totalRecords, int state, int district, int syear, int block, int batch, int stream, string proptype, int eacode, string propstatus, string propconnect, byte scheme,  string filters)
        {
            //var buildingProposalList = buildingProposalDAL.BuildingProposalList(state, district, block, syear, batch, agency, stream, proptype, propstatus, propconnect, "1", "1", "1");
            BuildingProposalViewModel buildingProposalViewModel = new BuildingProposalViewModel
            {
                MAST_STATE_CODE = state,
                MAST_DISTRICT_CODE = district,
                MAST_BLOCK_CODE = block,
                IMS_YEAR = syear,
                IMS_BATCH = batch,
                IMS_COLLABORATION = stream,
                IMS_SANCTIONED = propstatus,
                IMS_UPGRADE_CONNECT = propconnect,
                PMGSYScheme = scheme,               
                MAST_DPIU_CODE = eacode

            };


            var buildingProposalList = buildingProposalDAL.BuildingProposalListMRD(buildingProposalViewModel);
            totalRecords = buildingProposalList.Count;

            var itemJson = buildingProposalList.Select(propDetails => new
            {
                id = propDetails.IMS_PR_ROAD_CODE.ToString(),
                cell = new[] {     
                                    propDetails.DistrictName.ToString(),                            
                                    propDetails.MAST_BLOCK_NAME.ToString(),
                                    propDetails.IMS_PACKAGE_ID.ToString(),                                    
                                    propDetails.IMS_YEAR_FINANCIAL.ToString(),
                                    propDetails.IMS_ROAD_NAME.ToString(),                                    
                                    Math.Round(Convert.ToDecimal(propDetails.IMS_PAV_EST_COST),2).ToString(),
                                    propDetails.IMS_BATCH.ToString(),
                                    propDetails.IMS_SANCTIONED=="Y"?propDetails.IMS_SANCTIONED_BY+"("+propDetails.IMS_SANCTIONED_DATE+")":"No",
                                    "<a href='#'  class='ui-icon ui-icon-zoomin ui-align-center' onClick='ShowBuildingDetails(" + propDetails.IMS_PR_ROAD_CODE + "); return false;'>Show Details</a>"
                                    
                                   
                   }
            }).ToArray();

            return itemJson;
        }
       

        /*
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

        */
        /// <summary>
        /// Save the Road Proposal
        /// </summary>
        /// <param name="ims_sanctioned_projects"></param>
        /// <returns></returns>
        public string CreateBuilding(BuildingProposalViewModel buildingViewModel)
        {
         
            IMS_SANCTIONED_PROJECTS objProposal = new IMS_SANCTIONED_PROJECTS();
            try
            {
                objProposal.MAST_PMGSY_SCHEME = PMGSYSession.Current.PMGSYScheme;
                objProposal.IMS_PROPOSAL_TYPE = "B";
                objProposal.IMS_YEAR = buildingViewModel.IMS_YEAR;
                objProposal.IMS_BATCH = buildingViewModel.IMS_BATCH;
                // New Package or Exising Package
                if (buildingViewModel.IMS_EXISTING_PACKAGE.ToUpper() == "N")
                {
                    objProposal.IMS_PACKAGE_ID = buildingViewModel.PACKAGE_PREFIX + buildingViewModel.IMS_PACKAGE_ID;
                }
                else
                {
                    objProposal.IMS_PACKAGE_ID = buildingViewModel.EXISTING_IMS_PACKAGE_ID;
                }

                objProposal.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                objProposal.MAST_DISTRICT_CODE = PMGSYSession.Current.DistrictCode;
                objProposal.MAST_BLOCK_CODE = buildingViewModel.MAST_BLOCK_CODE;
                objProposal.MAST_DPIU_CODE = PMGSYSession.Current.AdminNdCode;
                objProposal.IMS_ROAD_NAME = buildingViewModel.IMS_ROAD_NAME;
                
                objProposal.IMS_COLLABORATION = buildingViewModel.IMS_COLLABORATION;
                
                
                objProposal.IMS_EXISTING_PACKAGE = buildingViewModel.IMS_EXISTING_PACKAGE;
                objProposal.IMS_PAV_EST_COST = buildingViewModel.IMS_PAV_EST_COST;
                

                objProposal.IMS_MAINTENANCE_YEAR1 = buildingViewModel.IMS_SANCTIONED_MAN_AMT1;
                objProposal.IMS_MAINTENANCE_YEAR2 = buildingViewModel.IMS_SANCTIONED_MAN_AMT2;
                objProposal.IMS_MAINTENANCE_YEAR3 = buildingViewModel.IMS_SANCTIONED_MAN_AMT3;
                objProposal.IMS_MAINTENANCE_YEAR4 = buildingViewModel.IMS_SANCTIONED_MAN_AMT4;
                objProposal.IMS_MAINTENANCE_YEAR5 = buildingViewModel.IMS_SANCTIONED_MAN_AMT5;

                objProposal.IMS_SANCTIONED_MAN_AMT1 = buildingViewModel.IMS_SANCTIONED_MAN_AMT1;
                objProposal.IMS_SANCTIONED_MAN_AMT2 = buildingViewModel.IMS_SANCTIONED_MAN_AMT2;
                objProposal.IMS_SANCTIONED_MAN_AMT3 = buildingViewModel.IMS_SANCTIONED_MAN_AMT3;
                objProposal.IMS_SANCTIONED_MAN_AMT4 = buildingViewModel.IMS_SANCTIONED_MAN_AMT4;
                objProposal.IMS_SANCTIONED_MAN_AMT5 = buildingViewModel.IMS_SANCTIONED_MAN_AMT5;
                objProposal.IMS_REMARKS = buildingViewModel.IMS_REMARKS;


                objProposal.IMS_UPGRADE_CONNECT = "N";

                objProposal.IMS_STREAMS = 1;
                objProposal.IMS_ZP_RESO_OBTAINED = "Y";
                objProposal.STA_SANCTIONED = "N";
                objProposal.IMS_DPR_STATUS = "N";
                objProposal.IMS_ROAD_FROM = " ";
                objProposal.IMS_ROAD_TO = " ";
                objProposal.IMS_PARTIAL_LEN = "F";
                objProposal.IMS_PAV_LENGTH = 0;
                objProposal.IMS_NO_OF_CDWORKS = 0;
                objProposal.IMS_CD_WORKS_EST_COST = 0;
                objProposal.IMS_PROTECTION_WORKS = 0;
                objProposal.IMS_OTHER_WORK_COST = 0;
                objProposal.IMS_STATE_SHARE = 0;
                objProposal.IMS_IS_STAGED = "C";
                objProposal.IMS_ISBENEFITTED_HABS = "Y";
                objProposal.IMS_SANCTIONED = "N";
                objProposal.PTA_SANCTIONED = "N";
                
                objProposal.IMS_SANCTIONED_PAV_AMT = 0;
                objProposal.IMS_SANCTIONED_CD_AMT = 0;
                objProposal.IMS_SANCTIONED_PW_AMT = 0;
                objProposal.IMS_SANCTIONED_OW_AMT = 0;
                objProposal.IMS_SANCTIONED_BS_AMT = 0;
                objProposal.IMS_SANCTIONED_RS_AMT = 0;
                objProposal.IMS_SANCTIONED_RENEWAL_AMT = 0;
                // For Execution
                objProposal.IMS_FINAL_PAYMENT_FLAG = "N";
                // newly entered Proposal
                objProposal.IMS_ISCOMPLETED = "E";
                // not locked
                objProposal.IMS_LOCK_STATUS = "N";
                // for Freezing 
                objProposal.IMS_FREEZE_STATUS = "U";
                objProposal.IMS_SHIFT_STATUS = "N";
                return buildingProposalDAL.SaveBuildingProposalDAL(objProposal);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return "An Error Occurred While Your Processing Request.";
            }
           
        }
       






        public bool BuildingProposalDelete(int ims_pr_road_code, ref string message)
        {
            return buildingProposalDAL.BuildingProposalDelete(ims_pr_road_code,ref message);
        }

        public bool BuildingProposalUpdate(BuildingProposalViewModel buildingViewModel, ref string message)
        {
            return buildingProposalDAL.BuildingProposalUpdate(buildingViewModel,ref message);
            
        }


        public BuildingProposalViewModel GetBuildingProposal(int id)
        {
            return buildingProposalDAL.GetBuildingProposal(id);;
        }


        public bool PIUFinalizedBuildingProposal(int id)
        {
            return buildingProposalDAL.PIUFinalizedBuildingProposal(id);
        }

        public BuildingSanctionViewModel GetSanctionBuildingProposal(int id)
        {
            
            var buildingProposal=buildingProposalDAL.GetBuildingProposal(id);
            BuildingSanctionViewModel buildingSanctionViewModel = new BuildingSanctionViewModel
            { 
                IMS_PR_ROAD_CODE=buildingProposal.IMS_PR_ROAD_CODE,
                IMS_ISCOMPLETED=buildingProposal.IMS_ISCOMPLETED,
                IMS_SANCTIONED=buildingProposal.IMS_SANCTIONED,
                IMS_PROG_REMARKS = buildingProposal.IMS_PROG_REMARKS,
                IMS_SANCTIONED_BY=buildingProposal.IMS_SANCTIONED_BY,
                IMS_SANCTIONED_DATE = buildingProposal.IMS_SANCTIONED == "Y" ? Convert.ToDateTime(buildingProposal.IMS_SANCTIONED_DATE).ToString("dd-MMM-yyyy") : DateTime.Now.ToString("dd-MMM-yyyy")

            };
            if (buildingSanctionViewModel.IMS_SANCTIONED_BY == string.Empty || buildingSanctionViewModel.IMS_SANCTIONED_BY == null)
            {
                buildingSanctionViewModel.IMS_SANCTIONED_BY = PMGSYSession.Current.UserName;
            }

            

            return buildingSanctionViewModel;
        }
        public bool BuildingProposalMoRDSacntionUpdate(BuildingSanctionViewModel buildingMoRDSacntionViewModel, ref string message)
        {
            return buildingProposalDAL.BuildingProposalMoRDSacntionUpdate(buildingMoRDSacntionViewModel, ref message);
        }
    }

}