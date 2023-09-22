/*----------------------------------------------------------------------------------------

 * Project Id:

 * Project Name:OMMAS2

 * File Name: MasterDataEntryBAL.cs

 * Author : Koustubh Nakate

 * Creation Date :06/Apr/2013

 * Desc : This class is used to call methods from data access layer class.  
 * ---------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.Models.MasterDataEntry;
using PMGSY.DAL;

namespace PMGSY.BAL
{
    public class MasterDataEntryBAL : IMasterDataEntryBAL
    {
        IMasterDataEntryDAL masterDataEntryDAL = new MasterDataEntryDAL();

        #region addition of facility


        public Array GetFacilityDetailsListBALDefinalize(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, List<string> ModelParam)
        {
            return masterDataEntryDAL.GetFacilityDetailsListDALDefinalize(page, rows, sidx, sord, out totalRecords, ModelParam);

        }


        public bool SaveFacilityDetailsBAL(CreateFacility createFacility, ref string message)
        {
            return masterDataEntryDAL.SaveFacilityDetailsDAL(createFacility, ref message);
        }

        public Array GetFacilityDetailsListBAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, List<string> ModelParam)
        {
            return masterDataEntryDAL.GetFacilityDetailsListDAL(page, rows, sidx, sord, out totalRecords, ModelParam);

        }

        public bool DeleteFacilityBAL(int id)
        {
            return masterDataEntryDAL.DeleteFacilityDAL(id);
        }

        public CreateFacility DisplayfacilityDetailsBAL(int facilityID)
        {
            return masterDataEntryDAL.DisplayfacilityDetailsDAL(facilityID);
        }

        public bool DeleteImageLatLong(int facilityID, string remark)
        {
            return masterDataEntryDAL.DeleteImageLatLongDAL(facilityID, remark);
        }
        #endregion

        public bool SaveStateDetailsBAL(MASTER_STATE master_state, ref string message)
        {
            // IMasterDataEntryDAL masterDataEntryDAL = new MasterDataEntryDAL();

            return masterDataEntryDAL.SaveStateDetailsDAL(master_state, ref message);
        }



        public MASTER_STATE GetStateDetailsBAL_ByStateCode(int stateCode)
        {
            //IMasterDataEntryDAL masterDataEntryDAL = new MasterDataEntryDAL();

            return masterDataEntryDAL.GetStateDetailsDAL_ByStateCode(stateCode);

        }


        public Array GetStateDetailsListBAL(bool isMap, int page, int rows, string sidx, string sord, out long totalRecords, int stateUT, int stateType)
        {
            // IMasterDataEntryDAL masterDataEntryDAL = new MasterDataEntryDAL();

            //  return masterDataEntryDAL.GetStateDetailsListDAL(int? page, int? rows, string sidx, string sord, out long totalRecords);

            return masterDataEntryDAL.GetStateDetailsListDAL(isMap, page, rows, sidx, sord, out totalRecords, stateUT, stateType);
        }



        public bool UpdateStateDetailsBAL(MASTER_STATE master_state, ref string message)
        {
            // IMasterDataEntryDAL masterDataEntryDAL = new MasterDataEntryDAL();

            return masterDataEntryDAL.UpdateStateDetailsDAL(master_state, ref message);
        }


        public bool DeleteStateDetailsBAL_ByStateCode(int stateCode, ref string message)
        {
            // IMasterDataEntryDAL masterDataEntryDAL = new MasterDataEntryDAL();

            return masterDataEntryDAL.DeleteStateDetailsDAL_ByStateCode(stateCode, ref message);
        }


        public bool SaveDistrictDetailsBAL(MASTER_DISTRICT master_district, ref string message)
        {
            //IMasterDataEntryDAL masterDataEntryDAL = new MasterDataEntryDAL();

            return masterDataEntryDAL.SaveDistrictDetailsDAL(master_district, ref message);
        }


        public Array GetDistrictDetailsListBAL(int agencyCode, int regionCode, int adminNdCode, bool isMap, MappingType mapping, int stateCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            // IMasterDataEntryDAL masterDataEntryDAL = new MasterDataEntryDAL();        

            return masterDataEntryDAL.GetDistrictDetailsListDAL(agencyCode,regionCode,adminNdCode, isMap, mapping, stateCode, page, rows, sidx, sord, out totalRecords);
        }


        public MASTER_DISTRICT GetDistrictDetailsBAL_ByDistrictCode(int districtCode)
        {
            // IMasterDataEntryDAL masterDataEntryDAL = new MasterDataEntryDAL();

            return masterDataEntryDAL.GetDistrictDetailsDAL_ByDistrictCode(districtCode);
        }


        public bool UpdateDistrictDetailsBAL(MASTER_DISTRICT master_district, ref string message)
        {
            // IMasterDataEntryDAL masterDataEntryDAL = new MasterDataEntryDAL();

            return masterDataEntryDAL.UpdateDistrictDetailsDAL(master_district, ref message);
        }


        public Array GetBlockDetailsListBAL(bool isMap, bool isMLA, int stateCode, int districtCode, int MLAConstituencyCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return masterDataEntryDAL.GetBlockDetailsListDAL(isMap, isMLA, stateCode, districtCode, MLAConstituencyCode, page, rows, sidx, sord, out totalRecords);
        }


        public bool SaveBlockDetailsBAL(BlockMaster master_block, ref string message)
        {
            return masterDataEntryDAL.SaveBlockDetailsDAL(master_block, ref message);
        }


        public BlockMaster GetBlockDetailsBAL_ByBlockCode(int blockCode)
        {
            return masterDataEntryDAL.GetBlockDetailsDAL_ByBlockCode(blockCode);
        }


        public bool UpdateBlockDetailsBAL(BlockMaster master_block, ref string message)
        {
            return masterDataEntryDAL.UpdateBlockDetailsDAL(master_block, ref message);
        }


        public Array GetVillageDetailsListBAL(int stateCode, int districtCode, int blockCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return masterDataEntryDAL.GetVillageDetailsListDAL(stateCode, districtCode, blockCode, page, rows, sidx, sord, out totalRecords);
        }


        public bool SaveVillageDetailsBAL(VillageMaster master_village, ref string message)
        {
            return masterDataEntryDAL.SaveVillageDetailsDAL(master_village, ref message);
        }


        public VillageMaster GetVillageDetailsBAL_ByVillageCode(int villageCode)
        {
            return masterDataEntryDAL.GetVillageDetailsDAL_ByVillageCode(villageCode);
        }


        public bool UpdateVillageDetailsBAL(VillageMaster master_village, ref string message)
        {
            return masterDataEntryDAL.UpdateVillageDetailsDAL(master_village, ref message);
        }


        public Array GetHabitationDetailsListBAL(bool isMap, int stateCode, int districtCode, int blockCode, string villageName, string habitationName, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return masterDataEntryDAL.GetHabitationDetailsListDAL(isMap, stateCode, districtCode, blockCode, villageName, habitationName, page, rows, sidx, sord, out totalRecords);
        }


        public bool SaveHabitationDetailsBAL(HabitationMaster master_habitations, ref string message)
        {
            return masterDataEntryDAL.SaveHabitationDetailsDAL(master_habitations, ref message);
        }


        public HabitationMaster GetHabitationDetailsBAL_ByHabitationCode(int habitationCode)
        {
            return masterDataEntryDAL.GetHabitationDetailsDAL_ByHabitationCode(habitationCode);
        }


        public bool UpdateHabitationDetailsBAL(HabitationMaster master_habitations, ref string message)
        {
            return masterDataEntryDAL.UpdateHabitationDetailsDAL(master_habitations, ref message);
        }


        public Array GetPanchayatDetailsListBAL(int stateCode, int districtCode, int blockCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return masterDataEntryDAL.GetPanchayatDetailsListDAL(stateCode, districtCode, blockCode, page, rows, sidx, sord, out totalRecords);
        }


        public bool SavePanchayatDetailsBAL(PanchayatMaster master_panchayat, ref string message)
        {
            return masterDataEntryDAL.SavePanchayatDetailsDAL(master_panchayat, ref message);
        }


        public PanchayatMaster GetPanchayatDetailsBAL_ByPanchayatCode(int panchayatCode)
        {
            return masterDataEntryDAL.GetPanchayatDetailsDAL_ByPanchayatCode(panchayatCode);
        }


        public bool UpdatePanchayatDetailsBAL(PanchayatMaster master_panchayat, ref string message)
        {
            return masterDataEntryDAL.UpdatePanchayatDetailsDAL(master_panchayat, ref message);
        }


        public Array GetMLAConstituencyDetailsListBAL(int stateCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return masterDataEntryDAL.GetMLAConstituencyDetailsListDAL(stateCode, page, rows, sidx, sord, out totalRecords);
        }


        public bool SaveMLAConstituencyDetailsBAL(MLAConstituency master_mlaconstituency, ref string message)
        {
            return masterDataEntryDAL.SaveMLAConstituencyDetailsDAL(master_mlaconstituency, ref message);
        }


        public MLAConstituency GetMLAConstituencyDetailsBAL_ByMLAConstituencyCode(int mlaConstituencyCode)
        {
            return masterDataEntryDAL.GetMLAConstituencyDetailsDAL_ByMLAConstituencyCode(mlaConstituencyCode);
        }


        public bool UpdateMLAConstituencyDetailsBAL(MLAConstituency master_mlaconstituency, ref string message)
        {
            return masterDataEntryDAL.UpdateMLAConstituencyDetailsDAL(master_mlaconstituency, ref message);
        }


        public Array GetMPConstituencyDetailsListBAL(int stateCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return masterDataEntryDAL.GetMPConstituencyDetailsListDAL(stateCode, page, rows, sidx, sord, out totalRecords);
        }


        public bool SaveMPConstituencyDetailsBAL(MPConstituency master_mpconstituency, ref string message)
        {
            return masterDataEntryDAL.SaveMPConstituencyDetailsDAL(master_mpconstituency, ref message);
        }


        public MPConstituency GetMPConstituencyDetailsBAL_ByMPConstituencyCode(int mpConstituencyCode)
        {
            return masterDataEntryDAL.GetMPConstituencyDetailsDAL_ByMPConstituencyCode(mpConstituencyCode);
        }


        public bool UpdateMPConstituencyDetailsBAL(MPConstituency master_mpconstituency, ref string message)
        {
            return masterDataEntryDAL.UpdateMPConstituencyDetailsDAL(master_mpconstituency, ref message);
        }


        public Array GetOtherHabitationDetailsListBAL(int habitationCode, string lockStatus, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return masterDataEntryDAL.GetOtherHabitationDetailsListDAL(habitationCode, lockStatus, page, rows, sidx, sord, out totalRecords);
        }


        public bool SaveOtherHabitationDetailsBAL(HabitationDetails details_habitations, ref string message)
        {
            return masterDataEntryDAL.SaveOtherHabitationDetailsDAL(details_habitations, ref message);
        }


        public HabitationDetails GetOtherHabitationDetailsBAL_ByHabitationCodeandYear(int habitationCode, short year)
        {
            return masterDataEntryDAL.GetOtherHabitationDetailsDAL_ByHabitationCodeandYear(habitationCode, year);
        }


        public bool UpdateOtherHabitationDetailsBAL(HabitationDetails details_habitations, ref string message)
        {
            return masterDataEntryDAL.UpdateOtherHabitationDetailsDAL(details_habitations, ref message);
        }


        public bool MapMLAConstituencyBlocksBAL(string encryptedMLAConstituencyCode, string encryptedBlockCodes)
        {
            return masterDataEntryDAL.MapMLAConstituencyBlocksDAL(encryptedMLAConstituencyCode, encryptedBlockCodes);
        }


        public bool MapMPConstituencyBlocksBAL(string encryptedMPConstituencyCode, string encryptedBlockCodes)
        {
            return masterDataEntryDAL.MapMPConstituencyBlocksDAL(encryptedMPConstituencyCode, encryptedBlockCodes);
        }


        public bool MapPanchayatHabitationsBAL(string encryptedPanchayatCode, string encryptedHabCodes)
        {
            return masterDataEntryDAL.MapPanchayatHabitationsDAL(encryptedPanchayatCode, encryptedHabCodes);
        }


        public bool ShiftDistrictBAL(string encryptedDistrictCodes, string newStateCode)
        {
            return masterDataEntryDAL.ShiftDistrictDAL(encryptedDistrictCodes, newStateCode);
        }


        public bool ShiftBlockBAL(string encryptedBlockCode, string newDistictCode)
        {
            return masterDataEntryDAL.ShiftBlockDAL(encryptedBlockCode, newDistictCode);
        }


        public bool ShiftVillageBAL(string encryptedVillageCode, string newBlockCode)
        {
            return masterDataEntryDAL.ShiftVillageDAL(encryptedVillageCode, newBlockCode);
        }


        public bool ShiftPanchayatBAL(string encryptedPanchayatCode, string newBlockCode)
        {
            return masterDataEntryDAL.ShiftPanchayatDAL(encryptedPanchayatCode, newBlockCode);
        }


        public bool DeleteDistrictDetailsBAL_ByDistrictCode(int districtCode, ref string message)
        {
            return masterDataEntryDAL.DeleteDistrictDetailsDAL_ByDistrictCode(districtCode, ref message);
        }


        public bool DeleteBlockDetailsBAL_ByBlockCode(int blockCode, ref string message)
        {
            return masterDataEntryDAL.DeleteBlockDetailsDAL_ByBlockCode(blockCode, ref message);
        }


        public bool DeleteVillageDetailsBAL_ByVillageCode(int villageCode, ref string message)
        {
            return masterDataEntryDAL.DeleteVillageDetailsDAL_ByVillageCode(villageCode, ref message);
        }


        public bool DeletePanchayatDetailsBAL_ByPanchayatCode(int panchayatCode, ref string message)
        {
            return masterDataEntryDAL.DeletePanchayatDetailsDAL_ByPanchayatCode(panchayatCode, ref message);
        }


        public bool DeleteHabitationDetailsBAL_ByHabitationCode(int habitationCode, ref string message)
        {
            return masterDataEntryDAL.DeleteHabitationDetailsDAL_ByHabitationCode(habitationCode, ref message);
        }


        public bool DeleteMLAConstituencyDetailsBAL_ByMLAConstituencyCode(int mlaConstituencyCode, ref string message)
        {
            return masterDataEntryDAL.DeleteMLAConstituencyDetailsDAL_ByMLAConstituencyCode(mlaConstituencyCode, ref message);
        }


        public bool DeleteMPConstituencyDetailsBAL_ByMPConstituencyCode(int mpConstituencyCode, ref string message)
        {
            return masterDataEntryDAL.DeleteMPConstituencyDetailsDAL_ByMPConstituencyCode(mpConstituencyCode, ref message);
        }


        public bool DeleteHabitationOtherDetailsBAL_ByHabCodeandYear(int habitationCode, short year, ref string message)
        {
            return masterDataEntryDAL.DeleteHabitationOtherDetailsDAL_ByHabCodeandYear(habitationCode, year, ref message);
        }


        public Array GetMappedBlockDetailsListBAL_MLA(int mlaConstituencyCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return masterDataEntryDAL.GetMappedBlockDetailsListDAL_MLA(mlaConstituencyCode, page, rows, sidx, sord, out totalRecords);
        }


        public Array GetMappedBlockDetailsListBAL_MP(int mpConstituencyCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return masterDataEntryDAL.GetMappedBlockDetailsListDAL_MP(mpConstituencyCode, page, rows, sidx, sord, out totalRecords);
        }


        public Array GetMappedHabitationDetailsListBAL_Panchayat(int panchayatCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return masterDataEntryDAL.GetMappedHabitationDetailsListDAL_Panchayat(panchayatCode, page, rows, sidx, sord, out totalRecords);
        }


        public bool FinalizeHabitationBAL(int habitationCode, ref string message)
        {
            return masterDataEntryDAL.FinalizeHabitationDAL(habitationCode, ref message);
        }


        public bool DeleteMappedHabitationBAL(int adminId)
        {
            return masterDataEntryDAL.DeleteMappedHabitationDAL(adminId);
        }


        public bool DeleteMappedMLABlockBAL(int blockId, ref string message)
        {
            return masterDataEntryDAL.DeleteMappedMLABlockDAL(blockId, ref message);
        }

        public bool DeleteMappedMPBlockBAL(int blockId)
        {
            return masterDataEntryDAL.DeleteMappedMPBlockDAL(blockId);
        }

        public bool FinalizeStateBAL(int stateCode)
        {
            return masterDataEntryDAL.FinalizeStateDAL(stateCode);
        }

        public bool FinalizeDistrictBAL(int districtCode)
        {
            return masterDataEntryDAL.FinalizeDistrictDAL(districtCode);
        }

        public bool FinalizeBlockBAL(int blockCode)
        {
            return masterDataEntryDAL.FinalizeBlockDAL(blockCode);
        }

        public bool FinalizeVillageBAL(int villageCode)
        {
            return masterDataEntryDAL.FinalizeVillageDAL(villageCode);
        }
        public bool FinalizePanchayatBAL(int panchayatCode, ref string message)
        {
            return masterDataEntryDAL.FinalizePanchayatDAL(panchayatCode, ref message);
        }
    }


    public interface IMasterDataEntryBAL
    {
        #region addition of facility

        Array GetFacilityDetailsListBALDefinalize(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, List<string> ModelParams);


        bool DeleteFacilityBAL(int id);

        Array GetFacilityDetailsListBAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, List<string> ModelParams);

        bool SaveFacilityDetailsBAL(CreateFacility createFacility, ref string message);

        CreateFacility DisplayfacilityDetailsBAL(int facilityID);

        bool DeleteImageLatLong(int facilityID, string remark);
        #endregion

        bool SaveStateDetailsBAL(MASTER_STATE master_state, ref string message);

        MASTER_STATE GetStateDetailsBAL_ByStateCode(int stateCode);

        Array GetStateDetailsListBAL(bool isMap, int page, int rows, string sidx, string sord, out long totalRecords, int stateUT, int stateType);

        bool UpdateStateDetailsBAL(MASTER_STATE master_state, ref string message);

        bool DeleteStateDetailsBAL_ByStateCode(int stateCode, ref string message);

        bool SaveDistrictDetailsBAL(MASTER_DISTRICT master_district, ref string message);

        Array GetDistrictDetailsListBAL(int agencyCode, int regionCode, int adminNdCode, bool isMap, MappingType mapping, int stateCode, int page, int rows, string sidx, string sord, out long totalRecords);

        MASTER_DISTRICT GetDistrictDetailsBAL_ByDistrictCode(int districtCode);

        bool UpdateDistrictDetailsBAL(MASTER_DISTRICT master_district, ref string message);

        Array GetBlockDetailsListBAL(bool isMap, bool isMLA, int stateCode, int districtCode, int MLAConstituencyCode, int page, int rows, string sidx, string sord, out long totalRecords);

        bool SaveBlockDetailsBAL(BlockMaster master_block, ref string message);


        BlockMaster GetBlockDetailsBAL_ByBlockCode(int blockCode);

        bool UpdateBlockDetailsBAL(BlockMaster master_block, ref string message);

        Array GetVillageDetailsListBAL(int stateCode, int districtCode, int blockCode, int page, int rows, string sidx, string sord, out long totalRecords);


        bool SaveVillageDetailsBAL(VillageMaster master_village, ref string message);

        VillageMaster GetVillageDetailsBAL_ByVillageCode(int villageCode);

        bool UpdateVillageDetailsBAL(VillageMaster master_village, ref string message);

        Array GetHabitationDetailsListBAL(bool isMap, int stateCode, int districtCode, int blockCode, string villageName, string habitationName, int page, int rows, string sidx, string sord, out long totalRecords);

        bool SaveHabitationDetailsBAL(HabitationMaster master_habitations, ref string message);

        HabitationMaster GetHabitationDetailsBAL_ByHabitationCode(int habitationCode);

        bool UpdateHabitationDetailsBAL(HabitationMaster master_habitations, ref string message);

        Array GetPanchayatDetailsListBAL(int stateCode, int districtCode, int blockCode, int page, int rows, string sidx, string sord, out long totalRecords);

        bool SavePanchayatDetailsBAL(PanchayatMaster master_panchayat, ref string message);

        PanchayatMaster GetPanchayatDetailsBAL_ByPanchayatCode(int panchayatCode);

        bool UpdatePanchayatDetailsBAL(PanchayatMaster master_panchayat, ref string message);

        Array GetMLAConstituencyDetailsListBAL(int stateCode, int page, int rows, string sidx, string sord, out long totalRecords);

        bool SaveMLAConstituencyDetailsBAL(MLAConstituency master_mlaconstituency, ref string message);

        MLAConstituency GetMLAConstituencyDetailsBAL_ByMLAConstituencyCode(int mlaConstituencyCode);

        bool UpdateMLAConstituencyDetailsBAL(MLAConstituency master_mlaconstituency, ref string message);

        Array GetMPConstituencyDetailsListBAL(int stateCode, int page, int rows, string sidx, string sord, out long totalRecords);

        bool SaveMPConstituencyDetailsBAL(MPConstituency master_mpconstituency, ref string message);

        MPConstituency GetMPConstituencyDetailsBAL_ByMPConstituencyCode(int mpConstituencyCode);

        bool UpdateMPConstituencyDetailsBAL(MPConstituency master_mpconstituency, ref string message);

        Array GetOtherHabitationDetailsListBAL(int habitationCode, string lockStatus, int page, int rows, string sidx, string sord, out long totalRecords);

        bool SaveOtherHabitationDetailsBAL(HabitationDetails details_habitations, ref string message);

        HabitationDetails GetOtherHabitationDetailsBAL_ByHabitationCodeandYear(int habitationCode, short year);

        bool UpdateOtherHabitationDetailsBAL(HabitationDetails details_habitations, ref string message);

        bool MapMLAConstituencyBlocksBAL(string encryptedMLAConstituencyCode, string encryptedBlockCodes);

        bool MapMPConstituencyBlocksBAL(string encryptedMPConstituencyCode, string encryptedBlockCodes);

        bool MapPanchayatHabitationsBAL(string encryptedPanchayatCode, string encryptedHabCodes);

        bool ShiftDistrictBAL(string encryptedDistrictCodes, string newStateCode);

        bool ShiftBlockBAL(string encryptedBlockCode, string newDistictCode);

        bool ShiftVillageBAL(string encryptedVillageCode, string newBlockCode);

        bool ShiftPanchayatBAL(string encryptedPanchayatCode, string newBlockCode);

        bool DeleteDistrictDetailsBAL_ByDistrictCode(int districtCode, ref string message);

        bool DeleteBlockDetailsBAL_ByBlockCode(int blockCode, ref string message);

        bool DeleteVillageDetailsBAL_ByVillageCode(int villageCode, ref string message);

        bool DeletePanchayatDetailsBAL_ByPanchayatCode(int panchayatCode, ref string message);

        bool DeleteHabitationDetailsBAL_ByHabitationCode(int habitationCode, ref string message);

        bool DeleteMLAConstituencyDetailsBAL_ByMLAConstituencyCode(int mlaConstituencyCode, ref string message);

        bool DeleteMPConstituencyDetailsBAL_ByMPConstituencyCode(int mpConstituencyCode, ref string message);

        bool DeleteHabitationOtherDetailsBAL_ByHabCodeandYear(int habitationCode, short year, ref string message);

        Array GetMappedBlockDetailsListBAL_MLA(int mlaConstituencyCode, int page, int rows, string sidx, string sord, out long totalRecords);

        Array GetMappedBlockDetailsListBAL_MP(int mpConstituencyCode, int page, int rows, string sidx, string sord, out long totalRecords);

        Array GetMappedHabitationDetailsListBAL_Panchayat(int panchayatCode, int page, int rows, string sidx, string sord, out long totalRecords);

        bool FinalizeHabitationBAL(int habitationCode, ref string message);

        bool DeleteMappedHabitationBAL(int adminId);

        bool DeleteMappedMLABlockBAL(int blockId, ref string message);

        bool DeleteMappedMPBlockBAL(int blockId);

        bool FinalizeStateBAL(int stateCode);

        bool FinalizeDistrictBAL(int districtCode);

        bool FinalizeBlockBAL(int blockCode);

        bool FinalizeVillageBAL(int villageCode);
        bool FinalizePanchayatBAL(int panchayatCode, ref string message);

    }
}