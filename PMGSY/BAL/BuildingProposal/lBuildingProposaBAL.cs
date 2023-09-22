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
    

    public interface IBuildingProposalBAL
    {
       
        Array GetBuildingProposalsBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int MAST_STATE_CODE, int MAST_DISTRICT_CODE, int IMS_YEAR, int MAST_BLOCK_CODE, int IMS_BATCH, int IMS_STREAMS, string ProposalType, int MAST_DPIU_CODE, string IMS_PROPOSAL_STATUS, string IMS_UPGRADE_CONNECT, byte scheme, int roleCode, string filters);
        Array GetMoRDBuildingProposalsBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int MAST_STATE_CODE, int MAST_DISTRICT_CODE, int IMS_YEAR, int MAST_BLOCK_CODE, int IMS_BATCH, int IMS_STREAMS, string ProposalType, int MAST_DPIU_CODE, string IMS_PROPOSAL_STATUS, string IMS_UPGRADE_CONNECT, byte scheme,  string filters);
        string CreateBuilding(BuildingProposalViewModel buildingViewModel);
        bool BuildingProposalDelete(int ims_pr_road_code, ref string message);
        bool BuildingProposalUpdate(BuildingProposalViewModel buildingViewModel, ref string message);
        BuildingProposalViewModel GetBuildingProposal(int id);
        bool PIUFinalizedBuildingProposal(int id);
        BuildingSanctionViewModel GetSanctionBuildingProposal(int id);
        bool BuildingProposalMoRDSacntionUpdate(BuildingSanctionViewModel buildingMoRDSacntionViewModel, ref string message);
        
       

      
    }
}