using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.DAL.ExistingRoadsReports;

namespace PMGSY.BAL.ExistingRoadsReports
{
    public class ExistingRoadsReportsBAL : IExistingRoadsReportsBAL
    {
        IExistingRoadsReportsDAL existingRoadsReportsDAL = new ExistingRoadsReportsDAL();
        public Array ERR1StateReportListingBAL(int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return existingRoadsReportsDAL.ERR1StateReportListingDAL(page, rows, sidx, sord, out totalRecords);
        }
        public Array ERR1DistrictReportListingBAL(int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return existingRoadsReportsDAL.ERR1DistrictReportListingDAL(stateCode, page, rows, sidx, sord, out totalRecords);
        }


        public Array ERR1BlockReportListingBAL(int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return existingRoadsReportsDAL.ERR1BlockReportListingDAL(stateCode, districtCode, page, rows, sidx, sord, out totalRecords);
        }


        public Array ERR1FinalListingBAL(int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return existingRoadsReportsDAL.ERR1FinalListingDAL(blockCode, districtCode, stateCode, page, rows, sidx, sord, out totalRecords);
        }


        public Array ERR2StateReportListingBAL(int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return existingRoadsReportsDAL.ERR2StateReportListingDAL(page, rows, sidx, sord, out totalRecords);
        }


        public Array ERR2DistrictReportListingBAL(int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return existingRoadsReportsDAL.ERR2DistrictReportListingDAL(stateCode, page, rows, sidx, sord, out totalRecords);
        }


        public Array ERR2BlockReportListingBAL(int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return existingRoadsReportsDAL.ERR2BlockReportListingDAL(stateCode, districtCode, page, rows, sidx, sord, out totalRecords);
        }


        public Array ERR2FinalListingBAL(int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return existingRoadsReportsDAL.ERR2FinalListingDAL(blockCode, districtCode, stateCode, page, rows, sidx, sord, out totalRecords);
        }


        public Array ERR3StateReportListingBAL(int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return existingRoadsReportsDAL.ERR3StateReportListingDAL(page, rows, sidx, sord, out totalRecords);
        }


        public Array ERR3DistrictReportListingBAL(int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return existingRoadsReportsDAL.ERR3DistrictReportListingDAL(stateCode, page, rows, sidx, sord, out totalRecords);
        }


        public Array ERR3BlockReportListingBAL(int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return existingRoadsReportsDAL.ERR3BlockReportListingDAL(stateCode, districtCode, page, rows, sidx, sord, out totalRecords);
        }


        public Array ERR3FinalListingBAL(int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return existingRoadsReportsDAL.ERR3FinalListingDAL(blockCode, districtCode, stateCode, page, rows, sidx, sord, out totalRecords);
        }

        public Array ERR4StateReportListingBAL(int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return existingRoadsReportsDAL.ERR4StateReportListingDAL(page, rows, sidx, sord, out totalRecords);
        }


        public Array ERR4DistrictReportListingBAL(int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return existingRoadsReportsDAL.ERR4DistrictReportListingDAL(stateCode, page, rows, sidx, sord, out totalRecords);
        }


        public Array ERR4BlockReportListingBAL(int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return existingRoadsReportsDAL.ERR4BlockReportListingDAL(stateCode, districtCode, page, rows, sidx, sord, out totalRecords);
        }


        public Array ERR4FinalListingBAL(int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return existingRoadsReportsDAL.ERR4FinalListingDAL(blockCode, districtCode, stateCode, page, rows, sidx, sord, out totalRecords);
        }

        public Array ERR5StateReportListingBAL(int page, int rows, string sidx, string sord, out int totalRecords, int cbrValue)
        {
            return existingRoadsReportsDAL.ERR5StateReportListingDAL(page, rows, sidx, sord, out totalRecords,cbrValue);
        }


        public Array ERR5DistrictReportListingBAL(int stateCode, int page, int rows, string sidx, string sord, out int totalRecords, int cbrValue)
        {
            return existingRoadsReportsDAL.ERR5DistrictReportListingDAL(stateCode, page, rows, sidx, sord, out totalRecords,cbrValue);
        }


        public Array ERR5BlockReportListingBAL(int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords, int cbrValue)
        {
            return existingRoadsReportsDAL.ERR5BlockReportListingDAL(stateCode, districtCode, page, rows, sidx, sord, out totalRecords,cbrValue);
        }


        public Array ERR5FinalListingBAL(int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords, int cbrValue)
        {
            return existingRoadsReportsDAL.ERR5FinalListingDAL(blockCode, districtCode, stateCode, page, rows, sidx, sord, out totalRecords,cbrValue);
        }
        public Array ERR6StateReportListingBAL(int page, int rows, string sidx, string sord, out int totalRecords, string roadType)
        {
            return existingRoadsReportsDAL.ERR6StateReportListingDAL(page, rows, sidx, sord, out totalRecords,roadType);
        }


        public Array ERR6DistrictReportListingBAL(int stateCode, int page, int rows, string sidx, string sord, out int totalRecords, string roadType)
        {
            return existingRoadsReportsDAL.ERR6DistrictReportListingDAL(stateCode, page, rows, sidx, sord, out totalRecords,roadType);
        }


        public Array ERR6BlockReportListingBAL(int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords, string roadType)
        {
            return existingRoadsReportsDAL.ERR6BlockReportListingDAL(stateCode, districtCode, page, rows, sidx, sord, out totalRecords,roadType);
        }


        public Array ERR6FinalListingBAL(int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords, string roadType)
        {
            return existingRoadsReportsDAL.ERR6FinalListingDAL(blockCode, districtCode, stateCode, page, rows, sidx, sord, out totalRecords,roadType);
        }

        public Array ERR7StateReportListingBAL(int terrainType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return existingRoadsReportsDAL.ERR7StateReportListingDAL(terrainType,page, rows, sidx, sord, out totalRecords);
        }


        public Array ERR7DistrictReportListingBAL(int stateCode, int terrainType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return existingRoadsReportsDAL.ERR7DistrictReportListingDAL(stateCode,terrainType, page, rows, sidx, sord, out totalRecords);
        }


        public Array ERR7BlockReportListingBAL(int stateCode, int districtCode, int terrainType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return existingRoadsReportsDAL.ERR7BlockReportListingDAL(stateCode, districtCode,terrainType, page, rows, sidx, sord, out totalRecords);
        }


        public Array ERR7FinalListingBAL(int blockCode, int districtCode, int stateCode, int terrainType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return existingRoadsReportsDAL.ERR7FinalListingDAL(blockCode, districtCode, stateCode,terrainType, page, rows, sidx, sord, out totalRecords);
        }
        public Array ERR8StateReportListingBAL(int soilType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return existingRoadsReportsDAL.ERR8StateReportListingDAL(soilType,page, rows, sidx, sord, out totalRecords);
        }


        public Array ERR8DistrictReportListingBAL(int stateCode, int soilType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return existingRoadsReportsDAL.ERR8DistrictReportListingDAL(stateCode,soilType, page, rows, sidx, sord, out totalRecords);
        }


        public Array ERR8BlockReportListingBAL(int stateCode, int districtCode, int soilType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return existingRoadsReportsDAL.ERR8BlockReportListingDAL(stateCode, districtCode,soilType, page, rows, sidx, sord, out totalRecords);
        }


        public Array ERR8FinalListingBAL(int blockCode, int districtCode, int stateCode, int soilType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return existingRoadsReportsDAL.ERR8FinalListingDAL(blockCode, districtCode, stateCode,soilType, page, rows, sidx, sord, out totalRecords);
        }

    }

    public interface IExistingRoadsReportsBAL
    {
           //ERR1
        Array ERR1StateReportListingBAL(int page, int rows, string sidx, string sord, out int totalRecords);
        Array ERR1DistrictReportListingBAL(int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array ERR1BlockReportListingBAL(int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array ERR1FinalListingBAL(int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);
       
        Array ERR2StateReportListingBAL(int page, int rows, string sidx, string sord, out int totalRecords);
        Array ERR2DistrictReportListingBAL(int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array ERR2BlockReportListingBAL(int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array ERR2FinalListingBAL(int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);


        Array ERR3StateReportListingBAL(int page, int rows, string sidx, string sord, out int totalRecords);
        Array ERR3DistrictReportListingBAL(int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array ERR3BlockReportListingBAL(int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array ERR3FinalListingBAL(int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);

        Array ERR4StateReportListingBAL(int page, int rows, string sidx, string sord, out int totalRecords);
        Array ERR4DistrictReportListingBAL(int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array ERR4BlockReportListingBAL(int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords);
        Array ERR4FinalListingBAL(int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords);

        Array ERR5StateReportListingBAL(int page, int rows, string sidx, string sord, out int totalRecords, int cbrValue);
        Array ERR5DistrictReportListingBAL(int stateCode, int page, int rows, string sidx, string sord, out int totalRecords, int cbrValue);
        Array ERR5BlockReportListingBAL(int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords, int cbrValue);
        Array ERR5FinalListingBAL(int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords, int cbrValue);

        Array ERR6StateReportListingBAL(int page, int rows, string sidx, string sord, out int totalRecords, string roadType);
        Array ERR6DistrictReportListingBAL(int stateCode, int page, int rows, string sidx, string sord, out int totalRecords, string roadType);
        Array ERR6BlockReportListingBAL(int stateCode, int districtCode, int page, int rows, string sidx, string sord, out int totalRecords, string roadType);
        Array ERR6FinalListingBAL(int blockCode, int districtCode, int stateCode, int page, int rows, string sidx, string sord, out int totalRecords, string roadType);

        Array ERR7StateReportListingBAL(int terrainType, int page, int rows, string sidx, string sord, out int totalRecords);
        Array ERR7DistrictReportListingBAL(int stateCode, int terrainType, int page, int rows, string sidx, string sord, out int totalRecords);
        Array ERR7BlockReportListingBAL(int stateCode, int districtCode, int terrainType, int page, int rows, string sidx, string sord, out int totalRecords);
        Array ERR7FinalListingBAL(int blockCode, int districtCode, int stateCode, int terrainType, int page, int rows, string sidx, string sord, out int totalRecords);

        Array ERR8StateReportListingBAL(int soilType, int page, int rows, string sidx, string sord, out int totalRecords);
        Array ERR8DistrictReportListingBAL(int stateCode, int soilType, int page, int rows, string sidx, string sord, out int totalRecords);
        Array ERR8BlockReportListingBAL(int stateCode, int districtCode, int soilType, int page, int rows, string sidx, string sord, out int totalRecords);
        Array ERR8FinalListingBAL(int blockCode, int districtCode, int stateCode, int soilType, int page, int rows, string sidx, string sord, out int totalRecords);
  

    }
}