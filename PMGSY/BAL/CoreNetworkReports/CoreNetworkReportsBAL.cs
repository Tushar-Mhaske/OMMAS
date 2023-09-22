using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.DAL.CoreNetworkReports;
namespace PMGSY.BAL.CoreNetworkReports
{
    public class CoreNetworkReportsBAL : ICoreNetworkReportsBAL
    {
        ICoreNetworkReportsDAL coreNetworkReportsDAL = new CoreNetworkReportsDAL();
        public Array CN1ReportListingBAL(string route, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.CN1ReportListingDAL(route, page, rows, sidx, sord, out totalRecords);
        }


        public Array CN1DistrictReportListingBAL(string route, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.CN1DistrictReportListingDAL(route, stateCode, page, rows, sidx, sord, out totalRecords);
        }


        public Array CN1BlockReportListingBAL(string route, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.CN1BlockReportListingDAL(route, stateCode, districtCode, page, rows, sidx, sord, out totalRecords);
        }


        public Array CN1FinalBlockReportListingBAL(int pop, int road, string route, int blockCode, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.CN1FinalBlockReportListingDAL(pop, road, route, blockCode, stateCode, districtCode, page, rows, sidx, sord, out totalRecords);
        }


        public Array CN2StateReportListingBAL(int pop, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.CN2StateReportListingDAL(pop, page, rows, sidx, sord, out totalRecords);
        }


        public Array CN2DistrictReportListingBAL(int pop, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.CN2DistrictReportListingDAL(pop, stateCode, page, rows, sidx, sord, out totalRecords);
        }


        public Array CN2BlockReportListingBAL(int pop, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.CN2BlockReportListingDAL(pop, stateCode, districtCode, page, rows, sidx, sord, out totalRecords);
        }


        public Array CN2FinalListingBAL(int pop, int road, string route, int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.CN2FinalListingDAL(pop,road,route, blockCode, districtCode, stateCode, page, rows, sidx, sord, out totalRecords);
        }


        public Array CN3StateReportListingBAL(int roadcategory, string route, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.CN3StateReportListingDAL(roadcategory, route, page, rows, sidx, sord, out totalRecords);
        }


        public Array CN3DistrictReportListingBAL(int roadcategory, string route, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.CN3DistrictReportListingDAL(roadcategory, route, stateCode, page, rows, sidx, sord, out totalRecords);
        }


        public Array CN3BlockReportListingBAL(int roadcategory, string route, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.CN3BlockReportListingDAL(roadcategory, route, stateCode, districtCode, page, rows, sidx, sord, out totalRecords);
        }


        public Array CN3FinalListingBAL(int pop, int roadcategory, string route, int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.CN3FinalListingDAL(pop, roadcategory, route, blockCode, districtCode, stateCode, page, rows, sidx, sord, out totalRecords);
        }


        public Array CN4StateReportListingBAL(int roadcategory, string route, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.CN4StateReportListingDAL(roadcategory, route, page, rows, sidx, sord, out totalRecords);
        }


        public Array CN4DistrictReportListingBAL(int roadcategory, string route, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.CN4DistrictReportListingDAL(roadcategory, route, stateCode, page, rows, sidx, sord, out totalRecords);
        }


        public Array CN4BlockReportListingBAL(int roadcategory, string route, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.CN4BlockReportListingDAL(roadcategory, route, stateCode, districtCode, page, rows, sidx, sord, out totalRecords);
        }


        public Array CN4FinalListingBAL(int pop, int roadcategory, string route, int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.CN4FinalListingDAL(pop, roadcategory, route, blockCode, districtCode, stateCode, page, rows, sidx, sord, out totalRecords);
        }

        public Array CN5StateReportListingBAL(int roadcategory, string route, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.CN5StateReportListingDAL(roadcategory, route, page, rows, sidx, sord, out totalRecords);
        }


        public Array CN5DistrictReportListingBAL(int roadcategory, string route, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.CN5DistrictReportListingDAL(roadcategory, route, stateCode, page, rows, sidx, sord, out totalRecords);
        }


        public Array CN5BlockReportListingBAL(int roadcategory, string route, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.CN5BlockReportListingDAL(roadcategory, route, stateCode, districtCode, page, rows, sidx, sord, out totalRecords);
        }


        public Array CN5FinalListingBAL(int pop, int roadcategory, string route, int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.CN5FinalListingDAL(pop, roadcategory, route, blockCode, districtCode, stateCode, page, rows, sidx, sord, out totalRecords);
        }
             

        public Array CN6StateReportListingBAL(string route, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.CN6StateReportListingDAL(route, page, rows, sidx, sord, out totalRecords);
        }


        public Array CN6DistrictReportListingBAL(string route, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.CN6DistrictReportListingDAL(route, stateCode, page, rows, sidx, sord, out totalRecords);
        }


        public Array CN6BlockReportListingBAL(string route, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.CN6BlockReportListingDAL(route, stateCode, districtCode, page, rows, sidx, sord, out totalRecords);
        }


        public Array CN6FinalListingBAL(string route, int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.CN6FinalListingDAL(route, blockCode, districtCode, stateCode, page, rows, sidx, sord, out totalRecords);
        }

        public Array CNCPLStateListingBAL(string route, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.CNCPLStateListingDAL(route, page, rows, sidx, sord, out totalRecords);
        }

        public Array CNCPLDistrictListingBAL(string route, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.CNCPLDistrictListingDAL(route, stateCode, page, rows, sidx, sord, out totalRecords);
        }

        public Array CNCPLBlockListingBAL(string route, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.CNCPLBlockListingDAL(route, districtCode, stateCode, page, rows, sidx, sord, out totalRecords);
        }

        public Array CNCPLFinalListingBAL(string route, int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.CNCPLFinalListingDAL(route, blockCode, districtCode, stateCode, page, rows, sidx, sord, out totalRecords);
        }

        public Array HWCNStateListingBAL(string route, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.HWCNStateListingDAL(route, page, rows, sidx, sord, out totalRecords);
        }

        public Array HWCNDistrictListingBAL(string route, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.HWCNDistrictListingDAL(route, stateCode, page, rows, sidx, sord, out totalRecords);
        }

        public Array HWCNBlockListingBAL(string route, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.HWCNBlockListingDAL(route, districtCode, stateCode, page, rows, sidx, sord, out totalRecords);
        }

        public Array HWCNFinalListingBAL(string route, int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.HWCNFinalListingDAL(route, blockCode, districtCode, stateCode, page, rows, sidx, sord, out totalRecords);
        }

        public Array RWCNStateListingBAL(string route, int road, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.RWCNStateListingDAL(route,road, page, rows, sidx, sord, out totalRecords);
        }

        public Array RWCNDistrictListingBAL(string route, int road, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.RWCNDistrictListingDAL(route, road, stateCode, page, rows, sidx, sord, out totalRecords);
        }

        public Array RWCNBlockListingBAL(string route, int road, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.RWCNBlockListingDAL(route, road, districtCode, stateCode, page, rows, sidx, sord, out totalRecords);
        }

        public Array RWCNFinalListingBAL(int pop, string route, int road, int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.RWCNFinalListingDAL(pop, route, road, blockCode, districtCode, stateCode, page, rows, sidx, sord, out totalRecords);
        }


        public Array PCIAbstractStateListingBAL(string route, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.PCIAbstractStateListingDAL(route, page, rows, sidx, sord, out totalRecords);
        }


        public Array PCIAbstractDistrictListingBAL(string route, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.PCIAbstractDistrictListingDAL(route, stateCode, page, rows, sidx, sord, out totalRecords);
        }


        public Array PCIAbstractBlockListingBAL(string route, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.PCIAbstractBlockListingDAL(route, districtCode, stateCode, page, rows, sidx, sord, out totalRecords);
        }


        public Array PCIAbstractFinalListingBAL(string route, int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.PCIAbstractFinalListingDAL(route, blockCode, districtCode, stateCode, page, rows, sidx, sord, out totalRecords);
        }

        public Array CUCPLStateListingBAL(string route, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.CUCPLStateListingDAL(route, page, rows, sidx, sord, out totalRecords);
        }

        public Array CUCPLDistrictListingBAL(string route, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.CUCPLDistrictListingDAL(route, stateCode, page, rows, sidx, sord, out totalRecords);
        }

        public Array CUCPLBlockListingBAL(string route, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.CUCPLBlockListingDAL(route, districtCode, stateCode, page, rows, sidx, sord, out totalRecords);
        }

        public Array CUCPLFinalListingBAL(string route, int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.CUCPLFinalListingDAL(route, blockCode, districtCode, stateCode, page, rows, sidx, sord, out totalRecords);
        }

        public Array CNR1StateReportListingBAL(int road, string route, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.CNR1StateReportListingDAL(road, route, page, rows, sidx, sord, out totalRecords);
        }


        public Array CNR1DistrictReportListingBAL(int road, string route, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.CNR1DistrictReportListingDAL(road, route, stateCode, page, rows, sidx, sord, out totalRecords);
        }


        public Array CNR1BlockReportListingBAL(int road, string route, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.CNR1BlockReportListingDAL(road, route, stateCode, districtCode, page, rows, sidx, sord, out totalRecords);
        }


        public Array CNR1FinalBlockReportListingBAL(int pop, int road, string route, int blockCode, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.CNR1FinalBlockReportListingDAL(pop, road, route, blockCode, stateCode, districtCode, page, rows, sidx, sord, out totalRecords);
        }


        public Array CNR2StateReportListingBAL(int road, string route, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.CNR2StateReportListingDAL(road, route, page, rows, sidx, sord, out totalRecords);
        }


        public Array CNR2DistrictReportListingBAL(int road, string route, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.CNR2DistrictReportListingDAL(road, route, stateCode, page, rows, sidx, sord, out totalRecords);
        }


        public Array CNR2BlockReportListingBAL(int road, string route, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.CNR2BlockReportListingDAL(road, route, stateCode, districtCode, page, rows, sidx, sord, out totalRecords);
        }


        public Array CNR2FinalBlockReportListingBAL(int road, string route, int blockCode, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.CNR2FinalBlockReportListingDAL(road, route, blockCode, stateCode, districtCode, page, rows, sidx, sord, out totalRecords);
        }



        public Array CNR3StateReportListingBAL(int road, string route, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.CNR3StateReportListingDAL(road, route, page, rows, sidx, sord, out totalRecords);
        }


        public Array CNR3DistrictReportListingBAL(int road, string route, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.CNR3DistrictReportListingDAL(road, route, stateCode, page, rows, sidx, sord, out totalRecords);
        }


        public Array CNR3BlockReportListingBAL(int road, string route, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.CNR3BlockReportListingDAL(road, route, stateCode, districtCode, page, rows, sidx, sord, out totalRecords);
        }


        public Array CNR3FinalBlockReportListingBAL(int road, string route, int blockCode, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.CNR3FinalBlockReportListingDAL(road, route, blockCode, stateCode, districtCode, page, rows, sidx, sord, out totalRecords);
        }


        public Array CNR4StateReportListingBAL(int road, string route, string length, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.CNR4StateReportListingDAL(road, route,length, page, rows, sidx, sord, out totalRecords);
        }


        public Array CNR4DistrictReportListingBAL(int road, string route, string length, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.CNR4DistrictReportListingDAL(road, route,length, stateCode, page, rows, sidx, sord, out totalRecords);
        }


        public Array CNR4BlockReportListingBAL(int road, string route, string length, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.CNR4BlockReportListingDAL(road, route,length, stateCode, districtCode, page, rows, sidx, sord, out totalRecords);
        }


        public Array CNR4FinalBlockReportListingBAL(int road, string route, string length, int blockCode, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.CNR4FinalBlockReportListingDAL(road, route,length, blockCode, stateCode, districtCode, page, rows, sidx, sord, out totalRecords);
        }



        public Array CNPriorityStateReportListingBAL(int priority, string route, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.CNPriorityStateReportListingDAL(priority, route, page, rows, sidx, sord, out totalRecords);
        }


        public Array CNPriorityDistrictReportListingBAL(int stateCode, int priority, string route, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.CNPriorityDistrictReportListingDAL(stateCode, priority, route, page, rows, sidx, sord, out totalRecords);
        }


        public Array CNPriorityBlockReportListingBAL(int stateCode, int districtCode, int priority, string route, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.CNPriorityBlockReportListingDAL(stateCode, districtCode, priority, route, page, rows, sidx, sord, out totalRecords);
        }


        public Array CNPriorityFinalBlockReportListingBAL(int blockCode, int districtCode, int stateCode, int priority, string route, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return coreNetworkReportsDAL.CNPriorityFinalBlockReportListingDAL(blockCode, districtCode, stateCode, priority, route, page, rows, sidx, sord, out totalRecords);
        }


    }

    public interface ICoreNetworkReportsBAL
    {
        //CN1
        Array CN1ReportListingBAL(string route, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CN1DistrictReportListingBAL(string route, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CN1BlockReportListingBAL(string route, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CN1FinalBlockReportListingBAL(int pop, int road, string route, int blockCode, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords);

        //CN2
        Array CN2StateReportListingBAL(int pop, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CN2DistrictReportListingBAL(int pop, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CN2BlockReportListingBAL(int pop, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CN2FinalListingBAL(int pop, int road, string route, int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);

        //CN3
        Array CN3StateReportListingBAL(int roadcategory, string route, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CN3DistrictReportListingBAL(int roadcategory, string route, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CN3BlockReportListingBAL(int roadcategory, string route, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CN3FinalListingBAL(int pop, int roadcategory, string route, int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);

        //CN4
        Array CN4StateReportListingBAL(int roadcategory, string route, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CN4DistrictReportListingBAL(int roadcategory, string route, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CN4BlockReportListingBAL(int roadcategory, string route, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CN4FinalListingBAL(int pop, int roadcategory, string route, int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);

        //CN5
        Array CN5StateReportListingBAL(int roadcategory, string route, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CN5DistrictReportListingBAL(int roadcategory, string route, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CN5BlockReportListingBAL(int roadcategory, string route, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CN5FinalListingBAL(int pop, int roadcategory, string route, int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);

        //CN6
        Array CN6StateReportListingBAL(string route, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CN6DistrictReportListingBAL(string route, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CN6BlockReportListingBAL(string route, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CN6FinalListingBAL(string route, int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);

        //CNCPL
        Array CNCPLStateListingBAL(string route, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CNCPLDistrictListingBAL(string route, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CNCPLBlockListingBAL(string route, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CNCPLFinalListingBAL(string route, int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);

        //HWCN
        Array HWCNBlockListingBAL(string route, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array HWCNDistrictListingBAL(string route, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array HWCNStateListingBAL(string route, int page, int rows, string sidx, string sord, out int totalRecords);
        Array HWCNFinalListingBAL(string route, int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);

        //RWCN
        Array RWCNBlockListingBAL(string route, int road, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array RWCNDistrictListingBAL(string route, int road, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array RWCNStateListingBAL(string route, int road, int page, int rows, string sidx, string sord, out int totalRecords);
        Array RWCNFinalListingBAL(int pop, string route, int road, int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);

        //PCI
        Array PCIAbstractStateListingBAL(string route, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PCIAbstractDistrictListingBAL(string route, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PCIAbstractBlockListingBAL(string route, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array PCIAbstractFinalListingBAL(string route, int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);

        //CUCPL
        Array CUCPLStateListingBAL(string route, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CUCPLDistrictListingBAL(string route, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CUCPLBlockListingBAL(string route, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CUCPLFinalListingBAL(string route, int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);

        //CNR1
        Array CNR1StateReportListingBAL(int road, string route, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CNR1DistrictReportListingBAL(int road, string route, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CNR1BlockReportListingBAL(int road, string route, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CNR1FinalBlockReportListingBAL(int pop, int road, string route, int blockCode, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords);


        //CNR2
        Array CNR2StateReportListingBAL(int road, string route, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CNR2DistrictReportListingBAL(int road, string route, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CNR2BlockReportListingBAL(int road, string route, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CNR2FinalBlockReportListingBAL(int road, string route, int blockCode, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords);


        //CNR3
        Array CNR3StateReportListingBAL(int road, string route, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CNR3DistrictReportListingBAL(int road, string route, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CNR3BlockReportListingBAL(int road, string route, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CNR3FinalBlockReportListingBAL(int road, string route, int blockCode, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords);

        //CNR4
        Array CNR4StateReportListingBAL(int road, string route,string length, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CNR4DistrictReportListingBAL(int road, string route, string length, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CNR4BlockReportListingBAL(int road, string route, string length, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CNR4FinalBlockReportListingBAL(int road, string route,  string length, int blockCode, int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords);

        //CNPriority
        Array CNPriorityStateReportListingBAL(int priority, string route, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CNPriorityDistrictReportListingBAL(int stateCode, int priority, string route, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CNPriorityBlockReportListingBAL(int stateCode, int districtCode, int priority, string route, int page, int rows, string sidx, string sord, out int totalRecords);
        Array CNPriorityFinalBlockReportListingBAL(int blockCode, int districtCode, int stateCode, int priority, string route, int page, int rows, string sidx, string sord, out int totalRecords);

    }
}