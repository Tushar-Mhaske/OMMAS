using PMGSY.DAL.ProposalReports;
using PMGSY.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.BAL.ProposalReports
{
    public class ProposalReportsBAL : IProposalReportsBAL
    {
        //MRDProposal Report
        IProposalReportsDAL proposalReportsDAL = new ProposalReportsDAL();

        public PMGSY.Models.ProposalReports.MRDProposalModel GetMRDProposalHabCovgBAL(PMGSY.Models.ProposalReports.MRDProposalModel objParam)
        {
            return proposalReportsDAL.GetMRDProposalHabCovgDAL(objParam);
        }

        public List<PMGSY.Models.ProposalReports.RoadCBRListing> GetMRDProposalRoadCBRDetailsBAL(int roadCode)
        {
            return proposalReportsDAL.GetMRDProposalRoadCBRDetailsDAL(roadCode);
        }

        public PMGSY.Models.ProposalReports.MRDProposalModel GetMRDProposalBAL(PMGSY.Models.ProposalReports.MRDProposalModel objParam)
        {
            return proposalReportsDAL.GetMRDProposalDAL(objParam);
        }

        public List<PMGSY.Models.ProposalReports.BridgeListing> GetMRDProposalBridgeDetailsBAL(int roadCode)
        {
            return proposalReportsDAL.GetMRDProposalBridgeDetailsDAL(roadCode);
        }

        public List<PMGSY.Models.ProposalReports.BridgeCostListing> GetMRDProposalBridgeCostDetailsBAL(int roadCode)
        {
            return proposalReportsDAL.GetMRDProposalBridgeCostDetailsDAL(roadCode);
        }

        public List<PMGSY.Models.ProposalReports.BridgeEstCostListing> GetMRDProposalBridgeEstCostDetailsBAL(int roadCode)
        {
            return proposalReportsDAL.GetMRDProposalBridgeEstCostDetailsDAL(roadCode);
        }


        public Array MPR1StateReportListingBAL(int year, int month, int collaboration, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.MPR1StateListingDAL(year, month, collaboration, page, rows, sidx, sord, out totalRecords);
        }

        public Array MPR1DistrictReportListingBAL(int stateCode, int year, int month, int collaboration, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.MPR1DistrictListingDAL(stateCode, year, month, collaboration, page, rows, sidx, sord, out totalRecords);
        }

        public Array MPR1BlockReportListingBAL(int stateCode, int districtCode, int year, int month, int collaboration, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.MPR1BlockListingDAL(districtCode, stateCode, year, month, collaboration, page, rows, sidx, sord, out totalRecords);
        }

        public Array MPR1FinalListingBAL(int blockCode, int districtCode, int stateCode, int year, int month, int collaboration, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.MPR1FinalListingDAL(blockCode, districtCode, stateCode, year, month, collaboration, page, rows, sidx, sord, out totalRecords);
        }

        public Array MPR2StateReportListingBAL(int month, int year, int collaboration, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.MPR2StateListingDAL(month, year, collaboration, page, rows, sidx, sord, out totalRecords);
        }

        public Array MPR2DistrictReportListingBAL(int month, int year, int collaboration, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.MPR2DistrictListingDAL(month, year, collaboration, stateCode, page, rows, sidx, sord, out totalRecords);
        }

        public Array MPR2BlockReportListingBAL(int month, int year, int collaboration, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.MPR2BlockListingDAL(month, year, collaboration, districtCode, stateCode, page, rows, sidx, sord, out totalRecords);
        }

        public Array MPR2FinalListingBAL(int month, int year, int collaboration, int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.MPR2FinalListingDAL(month, year, collaboration, blockCode, districtCode, stateCode, page, rows, sidx, sord, out totalRecords);
        }

        public Array HY1StateReportListingBAL(int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.HY1StateListingDAL(page, rows, sidx, sord, out totalRecords);
        }

        public Array HY1DistrictReportListingBAL(int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.HY1DistrictListingDAL(stateCode, page, rows, sidx, sord, out totalRecords);
        }

        public Array HY2StateReportListingBAL(int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.HY2StateListingDAL(page, rows, sidx, sord, out totalRecords);
        }

        public Array HY2DistrictReportListingBAL(int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.HY2DistrictListingDAL(stateCode, page, rows, sidx, sord, out totalRecords);
        }

        public Array PropListStateListingBAL(int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.PropListStateListingDAL(year, batch, collaboration, status, page, rows, sidx, sord, out totalRecords);
        }

        public Array PropListDistrictListingBAL(int stateCode, int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.PropListDistrictListingDAL(stateCode, year, batch, collaboration, status, page, rows, sidx, sord, out totalRecords);
        }

        public Array PropListBlockListingBAL(int districtCode, int stateCode, int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.PropListBlockListingDAL(districtCode, stateCode, year, batch, collaboration, status, page, rows, sidx, sord, out totalRecords);
        }

        public Array PropListFinalListingBAL(int blockCode, int districtCode, int stateCode, int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.PropListFinalListingDAL(blockCode, districtCode, stateCode, year, batch, collaboration, status, page, rows, sidx, sord, out totalRecords);
        }



        public Array PropSanctionStateListingBAL(int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.PropSanctionStateListingDAL(year, batch, collaboration, status, page, rows, sidx, sord, out totalRecords);
        }

        public Array PropSanctionDistrictListingBAL(int stateCode, int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.PropSanctionDistrictListingDAL(stateCode, year, batch, collaboration, status, page, rows, sidx, sord, out totalRecords);
        }

        public Array PropSanctionBlockListingBAL(int districtCode, int stateCode, int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.PropSanctionBlockListingDAL(districtCode, stateCode, year, batch, collaboration, status, page, rows, sidx, sord, out totalRecords);
        }

        public Array PropSanctionFinalListingBAL(int blockCode, int districtCode, int stateCode, int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.PropSanctionFinalListingDAL(blockCode, districtCode, stateCode, year, batch, collaboration, status, page, rows, sidx, sord, out totalRecords);
        }


        public Array PropSanctionLengthStateListingBAL(int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.PropSanctionLengthStateListingDAL(year, batch, collaboration, status, page, rows, sidx, sord, out totalRecords);
        }

        public Array PropSanctionLengthDistrictListingBAL(int stateCode, int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.PropSanctionLengthDistrictListingDAL(stateCode, year, batch, collaboration, status, page, rows, sidx, sord, out totalRecords);
        }

        public Array PropSanctionLengthBlockListingBAL(int districtCode, int stateCode, int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.PropSanctionLengthBlockListingDAL(districtCode, stateCode, year, batch, collaboration, status, page, rows, sidx, sord, out totalRecords);
        }

        public Array PropSanctionLengthFinalListingBAL(int blockCode, int districtCode, int stateCode, int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.PropSanctionLengthFinalListingDAL(blockCode, districtCode, stateCode, year, batch, collaboration, status, page, rows, sidx, sord, out totalRecords);
        }


        public Array PropEMCStateListingBAL(int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.PropEMCStateListingDAL(year, batch, collaboration, status, page, rows, sidx, sord, out totalRecords);
        }

        public Array PropEMCDistrictListingBAL(int stateCode, int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.PropEMCDistrictListingDAL(stateCode, year, batch, collaboration, status, page, rows, sidx, sord, out totalRecords);
        }

        public Array PropEMCBlockListingBAL(int districtCode, int stateCode, int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.PropEMCBlockListingDAL(districtCode, stateCode, year, batch, collaboration, status, page, rows, sidx, sord, out totalRecords);
        }

        public Array PropEMCFinalListingBAL(int blockCode, int districtCode, int stateCode, int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.PropEMCFinalListingDAL(blockCode, districtCode, stateCode, year, batch, collaboration, status, page, rows, sidx, sord, out totalRecords);
        }


        public Array PropScrutinyListingBAL(int stateCode, string type, int agencyId, int year, int batch, int scheme, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.PropScrutinyListingDAL(stateCode, type, agencyId, year, batch, scheme, page, rows, sidx, sord, out totalRecords);
        }
        public Array PropScrutinyTASDListingBAL(int stateCode, int districtCode, string type, int agencyId, int year, int batch, int scheme, string taName, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.PropScrutinyTASDListingDAL(stateCode, districtCode, type, agencyId, year, batch, scheme, taName, page, rows, sidx, sord, out totalRecords);
        }
        public Array PendingWorksListingBAL(int stateCode, string reason, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.PendingWorksListingDAL(stateCode, reason, page, rows, sidx, sord, out totalRecords);
        }
        public Array PropAnalysisDataGapListingBAL(int stateCode, string type, string scrutinity, string sanction, string report, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.PropAnalysisDataGapListingDAL(stateCode, type, scrutinity, sanction, report, page, rows, sidx, sord, out totalRecords);
        }
        public Array PropAnalysisDetailListingBAL(int stateCode, int year, int batch, string type, string scrutinity, string sanction, string report, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.PropAnalysisDetailListingDAL(stateCode, year, batch, type, scrutinity, sanction, report, page, rows, sidx, sord, out totalRecords);
        }
        public Array PropAnalysisHabitationListingBAL(int roadCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.PropAnalysisHabitationListingDAL(roadCode, page, rows, sidx, sord, out totalRecords);

        }

        public Array PropAnalysisTrafficListingBAL(int roadCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.PropAnalysisTrafficListingDAL(roadCode, page, rows, sidx, sord, out totalRecords);
        }

        public Array PropAnalysisCBRListingBAL(int roadCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.PropAnalysisCBRListingDAL(roadCode, page, rows, sidx, sord, out totalRecords);
        }

        #region Prposal DataGap 
        public Array PropNotMappedStateListingBAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, string sanctioned, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.PropNotMappedStateListingDAL(stateCode, districtCode, blockCode,yearCode,batchCode,collaborationCode,agencyCode,staStatusCode,mrdStatusCode, sanctioned, page, rows, sidx, sord, out totalRecords);
        }

        public Array PropNotMappedDistrictListingBAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, string sanctioned, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.PropNotMappedDistrictListingDAL(stateCode, districtCode, blockCode, yearCode, batchCode, collaborationCode, agencyCode, staStatusCode, mrdStatusCode, sanctioned, page, rows, sidx, sord, out totalRecords);
        }
        public Array PropNotMappedDetailsListingBAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, string sanctioned, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.PropNotMappedDetailsListingDAL(stateCode, districtCode, blockCode, yearCode, batchCode, collaborationCode, agencyCode, staStatusCode, mrdStatusCode, sanctioned, page, rows, sidx, sord, out totalRecords);
        }

        public Array PropNotMappedPhaseViewListingBAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, string sanctioned, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.PropNotMappedPhaseViewListingDAL(stateCode, districtCode, blockCode, yearCode, batchCode, collaborationCode, agencyCode, staStatusCode, mrdStatusCode, sanctioned, page, rows, sidx, sord, out totalRecords);
        }

        public Array PropNumberBaseCNDistrictListingBAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.PropNumberBaseCNDistrictListingDAL(stateCode, districtCode, blockCode, yearCode, batchCode, collaborationCode, agencyCode, staStatusCode, mrdStatusCode, page, rows, sidx, sord, out totalRecords);
        }

        public Array PropNumberBaseCNRoadDetailsListingBAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, string road, string report, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.PropNumberBaseCNRoadDetailsListingDAL(stateCode, districtCode, blockCode, yearCode, batchCode, collaborationCode, agencyCode, staStatusCode, mrdStatusCode, road, report, page, rows, sidx, sord, out totalRecords);
        }

        public Array PropMultipleDistrictListingBAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, string sanctioned, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.PropMultipleDistrictListingDAL(stateCode, districtCode, blockCode,yearCode,batchCode,collaborationCode,agencyCode,staStatusCode,mrdStatusCode, sanctioned, page, rows, sidx, sord, out totalRecords);
        }
        public Array PropMultipleDetailsListingBAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, string roadNumber, string sanctioned, string report, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.PropMultipleDetailsListingDAL(stateCode, districtCode, blockCode, yearCode, batchCode, collaborationCode, agencyCode, staStatusCode, mrdStatusCode, roadNumber, sanctioned, report, page, rows, sidx, sord, out totalRecords);
        }

        public Array PropSingleHabStateListingBAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, int population, string sanctioned, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.PropSingleHabStateListingDAL(stateCode, districtCode, blockCode, yearCode, batchCode, collaborationCode, agencyCode, staStatusCode, mrdStatusCode, population, sanctioned, page, rows, sidx, sord, out totalRecords);
        }
        public Array PropSingleHabDistrictListingBAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, int population, string sanctioned, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.PropSingleHabDistrictListingDAL(stateCode, districtCode, blockCode, yearCode, batchCode, collaborationCode, agencyCode, staStatusCode, mrdStatusCode, population, sanctioned, page, rows, sidx, sord, out totalRecords);
        }
        public Array PropSingleHabDetailsListingBAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, int population, string sanctioned, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.PropSingleHabDetailsListingDAL(stateCode, districtCode, blockCode,yearCode,batchCode,collaborationCode,agencyCode,staStatusCode,mrdStatusCode, population, sanctioned, page, rows, sidx, sord, out totalRecords);
        }

        public Array PropZeroMaintStateListingBAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, string constructionType, string sanctioned, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.PropZeroMaintStateListingDAL(stateCode, districtCode, blockCode, yearCode, batchCode, collaborationCode, agencyCode, staStatusCode, mrdStatusCode, constructionType, sanctioned, page, rows, sidx, sord, out totalRecords);
        }
        public Array PropZeroMaintDistrictListingBAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, string constructionType, string sanctioned, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.PropZeroMaintDistrictListingDAL(stateCode, districtCode, blockCode, yearCode, batchCode, collaborationCode, agencyCode, staStatusCode, mrdStatusCode, constructionType, sanctioned, page, rows, sidx, sord, out totalRecords);
        }
        public Array PropZeroMaintDetailsListingBAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, string constructionType, string sanctioned, string report, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.PropZeroMaintDetailsListingDAL(stateCode, districtCode, blockCode, yearCode, batchCode, collaborationCode, agencyCode, staStatusCode, mrdStatusCode, constructionType, sanctioned, report, page, rows, sidx, sord, out totalRecords);
        }


        public Array PropCarriageWidthStateListingBAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, int carriageWidth, string sanctioned, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.PropCarriageWidthStateListingDAL(stateCode, districtCode, blockCode, yearCode, batchCode, collaborationCode, agencyCode, staStatusCode, mrdStatusCode, carriageWidth, sanctioned, page, rows, sidx, sord, out totalRecords);
        }
        public Array PropCarriageWidthDistrictListingBAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, int carriageWidth, string sanctioned, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.PropCarriageWidthDistrictListingDAL(stateCode, districtCode, blockCode, yearCode, batchCode, collaborationCode, agencyCode, staStatusCode, mrdStatusCode, carriageWidth, sanctioned, page, rows, sidx, sord, out totalRecords);
        }
        public Array PropCarriageWidthDetailsListingBAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, int carriageWidth, string sanctioned, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.PropCarriageWidthDetailsListingDAL(stateCode, districtCode, blockCode, yearCode, batchCode, collaborationCode, agencyCode, staStatusCode, mrdStatusCode, carriageWidth, sanctioned, page, rows, sidx, sord, out totalRecords);
        }

        public Array PropVariationLengthListingBAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, string sanctioned, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.PropVariationLengthListingDAL(stateCode, districtCode, blockCode, yearCode, batchCode, collaborationCode, agencyCode, staStatusCode, mrdStatusCode, sanctioned, page, rows, sidx, sord, out totalRecords);
        }
        public Array PropMisclassificationListingBAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.PropMisclassificationListingDAL(stateCode, districtCode, blockCode, yearCode, batchCode, collaborationCode, agencyCode, staStatusCode, mrdStatusCode, page, rows, sidx, sord, out totalRecords);
        }
        public Array PropMisclassificationDetailsListingBAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, string type, string report, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.PropMisclassificationDetailsListingDAL(stateCode, districtCode, blockCode, yearCode, batchCode, collaborationCode, agencyCode, staStatusCode, mrdStatusCode, type, report, page, rows, sidx, sord, out totalRecords);
        }
#endregion

        public Array PCIAbstractAnalysisListingBAL(int stateCode, int districtCode, int blockCode, string flag, string routeType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.PCIAbstractAnalysisListingDAL(stateCode, districtCode, blockCode, flag, routeType, page, rows, sidx, sord, out totalRecords);

        }

        public List<USP_CN_PCI_ANALYSIS_Result> GetPCIAnalysisChartBAL(int stateCode, int districtCode, int blockCode, string flag, string routeType)
        {
            return proposalReportsDAL.GetPCIAnalysisChartDAL(stateCode, districtCode, blockCode, flag, routeType);
        }
        public Array PropAssetValueListingBAL(int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.PropAssetValueListingDAL(page, rows, sidx, sord, out totalRecords);
        }
        public Array ExecutionFinancialProgressListingBAL(int stateCode, int districtCode, int blockCode, int year, int batch, int collaboration, string type, string progress, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.ExecutionFinancialProgressListingDAL(stateCode, districtCode, blockCode, year, batch, collaboration, type, progress, page, rows, sidx, sord, out totalRecords);

        }
        public Array MaintenanceFinancialProgressListingBAL(int stateCode, int districtCode, int blockCode, int year, int batch, int collaboration, string type, string progress, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.MaintenanceFinancialProgressListingDAL(stateCode, districtCode, blockCode, year, batch, collaboration, type, progress, page, rows, sidx, sord, out totalRecords);
        }
        public Array MaintenanceAgreementListingBAL(int stateCode, int districtCode, int blockCode, int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.MaintenanceAgreementListingDAL(stateCode, districtCode, blockCode, year, batch, collaboration, status, page, rows, sidx, sord, out totalRecords);
        }
        public Array MaintenanceInspectionListingBAL(int stateCode, int districtCode, int blockCode, int year, int batch, int collaboration, string type, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.MaintenanceInspectionListingDAL(stateCode, districtCode, blockCode, year, batch, collaboration, type, page, rows, sidx, sord, out totalRecords);
        }
        public Array FundSanctionReleaseListingBAL(int stateCode, int year, int collaboration, int agency, string fund, string type, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.FundSanctionReleaseListingDAL(stateCode, year, collaboration, agency, fund, type, page, rows, sidx, sord, out totalRecords);
        }
        public Array TeanderAgreementListingBAL(int stateCode, int districtCode, int blockCode, int year, int batch, int collaboration, int conId, string package, string status, string agreement, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return proposalReportsDAL.TeanderAgreementListingDAL(stateCode, districtCode, blockCode, year, batch, collaboration, conId, package, status, agreement, page, rows, sidx, sord, out totalRecords);
        }

    }

    public interface IProposalReportsBAL
    {

        PMGSY.Models.ProposalReports.MRDProposalModel GetMRDProposalHabCovgBAL(PMGSY.Models.ProposalReports.MRDProposalModel objParam);

        //MRDProposal Report
        PMGSY.Models.ProposalReports.MRDProposalModel GetMRDProposalBAL(PMGSY.Models.ProposalReports.MRDProposalModel objParam);

        List<PMGSY.Models.ProposalReports.RoadCBRListing> GetMRDProposalRoadCBRDetailsBAL(int roadCode);

        List<PMGSY.Models.ProposalReports.BridgeListing> GetMRDProposalBridgeDetailsBAL(int roadCode);

        List<PMGSY.Models.ProposalReports.BridgeCostListing> GetMRDProposalBridgeCostDetailsBAL(int roadCode);

        List<PMGSY.Models.ProposalReports.BridgeEstCostListing> GetMRDProposalBridgeEstCostDetailsBAL(int roadCode);

        // MPR 1
        Array MPR1StateReportListingBAL(int year, int month, int collaboration, int page, int rows, string sidx, string sord, out int totalRecords);
        Array MPR1DistrictReportListingBAL(int stateCode, int year, int month, int collaboration, int page, int rows, string sidx, string sord, out int totalRecords);
        Array MPR1BlockReportListingBAL(int stateCode, int districtCode, int year, int month, int collaboration, int page, int rows, string sidx, string sord, out int totalRecords);
        Array MPR1FinalListingBAL(int blockCode, int districtCode, int stateCode, int year, int month, int collaboration, int page, int rows, string sidx, string sord, out int totalRecords);

        // MPR 2
        Array MPR2StateReportListingBAL(int month, int year, int collaboration, int page, int rows, string sidx, string sord, out int totalRecords);
        Array MPR2DistrictReportListingBAL(int month, int year, int collaboration, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array MPR2BlockReportListingBAL(int month, int year, int collaboration, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array MPR2FinalListingBAL(int month, int year, int collaboration, int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);

        // HY 1
        Array HY1StateReportListingBAL(int page, int rows, string sidx, string sord, out int totalRecords);
        Array HY1DistrictReportListingBAL(int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);

        // HY 2
        Array HY2StateReportListingBAL(int page, int rows, string sidx, string sord, out int totalRecords);
        Array HY2DistrictReportListingBAL(int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);

        // Prop List
        Array PropListStateListingBAL(int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropListDistrictListingBAL(int stateCode, int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropListBlockListingBAL(int districtCode, int stateCode, int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropListFinalListingBAL(int blockCode, int districtCode, int stateCode, int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords);

        // Prop Sanction List
        Array PropSanctionStateListingBAL(int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropSanctionDistrictListingBAL(int stateCode, int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropSanctionBlockListingBAL(int districtCode, int stateCode, int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropSanctionFinalListingBAL(int blockCode, int districtCode, int stateCode, int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords);

        // Prop Sanction Length
        Array PropSanctionLengthStateListingBAL(int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropSanctionLengthDistrictListingBAL(int stateCode, int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropSanctionLengthBlockListingBAL(int districtCode, int stateCode, int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropSanctionLengthFinalListingBAL(int blockCode, int districtCode, int stateCode, int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords);

        // Prop Estimated Maintanance Cost
        Array PropEMCStateListingBAL(int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropEMCDistrictListingBAL(int stateCode, int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropEMCBlockListingBAL(int districtCode, int stateCode, int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropEMCFinalListingBAL(int blockCode, int districtCode, int stateCode, int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords);

        // Prop Scrutiny 
        Array PropScrutinyListingBAL(int stateCode, string type, int agencyId, int year, int batch, int scheme, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropScrutinyTASDListingBAL(int stateCode, int districtCode, string type, int agencyId, int year, int batch, int scheme, string taName, int page, int rows, string sidx, string sord, out int totalRecords);

        //Pending Works
        Array PendingWorksListingBAL(int stateCode, string reason, int page, int rows, string sidx, string sord, out int totalRecords);

        // Proposal Analysis Details 
        Array PropAnalysisDataGapListingBAL(int stateCode, string type, string scrutinity, string sanction, string report, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropAnalysisDetailListingBAL(int stateCode, int year, int batch, string type, string scrutinity, string sanction, string report, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropAnalysisHabitationListingBAL(int roadCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropAnalysisTrafficListingBAL(int roadCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropAnalysisCBRListingBAL(int roadCode, int page, int rows, string sidx, string sord, out int totalRecords);


        //Proposal DataGap Details
        #region Proposal Datagap Details
        Array PropNotMappedStateListingBAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, string sanctioned, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropNotMappedDistrictListingBAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, string sanctioned, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropNotMappedDetailsListingBAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, string sanctioned, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropNotMappedPhaseViewListingBAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, string sanctioned, int page, int rows, string sidx, string sord, out int totalRecords);

        Array PropNumberBaseCNDistrictListingBAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropNumberBaseCNRoadDetailsListingBAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, string road, string report, int page, int rows, string sidx, string sord, out int totalRecords);

        Array PropMultipleDistrictListingBAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, string sanctioned, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropMultipleDetailsListingBAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, string roadNumber, string sanctioned, string report, int page, int rows, string sidx, string sord, out int totalRecords);

        Array PropSingleHabStateListingBAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, int population, string sanctioned, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropSingleHabDistrictListingBAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, int population, string sanctioned, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropSingleHabDetailsListingBAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, int population, string sanctioned, int page, int rows, string sidx, string sord, out int totalRecords);

        Array PropZeroMaintStateListingBAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, string constructionType, string sanctioned, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropZeroMaintDistrictListingBAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, string constructionType, string sanctioned, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropZeroMaintDetailsListingBAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, string constructionType, string sanctioned, string report, int page, int rows, string sidx, string sord, out int totalRecords);

        Array PropCarriageWidthStateListingBAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, int carriageWidth, string sanctioned, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropCarriageWidthDistrictListingBAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, int carriageWidth, string sanctioned, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropCarriageWidthDetailsListingBAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, int carriageWidth, string sanctioned, int page, int rows, string sidx, string sord, out int totalRecords);

        Array PropVariationLengthListingBAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, string sanctioned, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropMisclassificationListingBAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PropMisclassificationDetailsListingBAL(int stateCode, int districtCode, int blockCode, int yearCode, int batchCode, int collaborationCode, int agencyCode, string staStatusCode, string mrdStatusCode, string type, string report, int page, int rows, string sidx, string sord, out int totalRecords);
        #endregion
        Array PCIAbstractAnalysisListingBAL(int stateCode, int districtCode, int blockCode, string flag, string routeType, int page, int rows, string sidx, string sord, out int totalRecords);
        List<USP_CN_PCI_ANALYSIS_Result> GetPCIAnalysisChartBAL(int stateCode, int districtCode, int blockCode, string flag, string routeType);

        Array PropAssetValueListingBAL(int page, int rows, string sidx, string sord, out int totalRecords);
        Array ExecutionFinancialProgressListingBAL(int stateCode, int districtCode, int blockCode, int year, int batch, int collaboration, string type, string progress, int page, int rows, string sidx, string sord, out int totalRecords);
        Array MaintenanceFinancialProgressListingBAL(int stateCode, int districtCode, int blockCode, int year, int batch, int collaboration, string type, string progress, int page, int rows, string sidx, string sord, out int totalRecords);
        Array MaintenanceAgreementListingBAL(int stateCode, int districtCode, int blockCode, int year, int batch, int collaboration, string status, int page, int rows, string sidx, string sord, out int totalRecords);
        Array MaintenanceInspectionListingBAL(int stateCode, int districtCode, int blockCode, int year, int batch, int collaboration, string type, int page, int rows, string sidx, string sord, out int totalRecords);
        Array FundSanctionReleaseListingBAL(int stateCode, int year, int collaboration, int agency, string fund, string type, int page, int rows, string sidx, string sord, out int totalRecords);
        Array TeanderAgreementListingBAL(int stateCode, int districtCode, int blockCode, int year, int batch, int collaboration, int conId, string package, string status, string agreement, int page, int rows, string sidx, string sord, out int totalRecords);


    }
}