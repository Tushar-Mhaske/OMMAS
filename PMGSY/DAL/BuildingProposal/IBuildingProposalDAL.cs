using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.Models;
using PMGSY.Models.BuildingProposal;

namespace PMGSY.DAL.BuildingProposal
{
    public interface IBuildingProposalDAL
    {
        string SaveBuildingProposalDAL(IMS_SANCTIONED_PROJECTS objProposal);
        List<BuildingProposalViewModel> BuildingProposalListMRD(BuildingProposalViewModel buildingProposalViewModel);
        List<BuildingProposalViewModel> BuildingProposalListPIU(BuildingProposalViewModel bm);
        bool BuildingProposalDelete(int ims_pr_road_code, ref string message);
        bool BuildingProposalUpdate(BuildingProposalViewModel buildingProposalViewModel, ref string message);
        BuildingProposalViewModel GetBuildingProposal(int id);
        
        bool PIUFinalizedBuildingProposal(int id);
        bool BuildingProposalMoRDSacntionUpdate(BuildingSanctionViewModel buildingMoRDSacntionViewModel, ref string message);
    }

}