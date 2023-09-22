using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.Models;
using PMGSY.Areas.ProposalSSRSReports.DAL;

namespace PMGSY.Areas.ProposalSSRSReports.BAL
{
    public class ProposalSSRSBAL : IProposalSSRSReportsBAL
    {
        IProposalSSRSReportsDAL proposalReportsDAL = new ProposalSSRSDAL();

        public Array PCIPropAnalysisListingBAL(int stateCode, int districtCode, int blockCode, string flag, string routeType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.PCIPropAnalysisListingDAL(stateCode, districtCode, blockCode, flag, routeType, page, rows, sidx, sord, out totalRecords);

        }

        public List<USP_CN_PROP_PCI_ANALYSIS_Result> GetPropPCIAnalysisChartBAL(int stateCode, int districtCode, int blockCode, string flag, string routeType)
        {
            return proposalReportsDAL.GetPropPCIAnalysisChartDAL(stateCode, districtCode, blockCode, flag, routeType);
        }
    }
    
    public interface IProposalSSRSReportsBAL
    {
        Array PCIPropAnalysisListingBAL(int stateCode, int districtCode, int blockCode, string flag, string routeType, int page, int rows, string sidx, string sord, out int totalRecords);
        List<USP_CN_PROP_PCI_ANALYSIS_Result> GetPropPCIAnalysisChartBAL(int stateCode, int districtCode, int blockCode, string flag, string routeType);
    }
}