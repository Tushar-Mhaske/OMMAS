#region HEADER
/*
 * Project Id:

 * Project Name:OMMAS-II

 * File Name: LockUnlockBAL.cs

 * Author : Vikram Nandanwar
 
 * Creation Date :05/June/2013

 * Desc : This class is used as BAL to call methods present in the DAL for Save,Edit,Update,Delete and listing of Lock Unlock screens.  
 
 */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using PMGSY.DAL.LockUnlock;
using PMGSY.Models.LockUnlock;

namespace PMGSY.BAL.LockUnlock
{
    public class LockUnlockBAL : ILockUnlockBAL
    {
        ILockUnlockDAL objLockUnlockDAL = new LockUnlockDAL();

        public Array GetProposalsBAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int IMS_YEAR, int IMS_MAST_STATE_CODE, int IMS_BATCH, int Scheme)
        {
            return objLockUnlockDAL.GetProposalsDAL(page, rows, sidx, sord, out totalRecords, IMS_YEAR, IMS_MAST_STATE_CODE, IMS_BATCH, Scheme);
        }

        public bool FreezeUnfreezeProposal(ProposalFilterLockUnlockViewModel LockUnlockViewModel, ref string message)
        {
            return objLockUnlockDAL.FreezeUnfreezeProposal(LockUnlockViewModel, ref message);
        }

        public Array GetFreezeUnfreezeReportDetails(int stateCode, int batchCode, int yearCode, int schemeCode, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            return objLockUnlockDAL.GetFreezeUnfreezeReportDetails(stateCode, batchCode, yearCode, schemeCode, page, rows, sidx, sord, out totalRecords);
        }


        #region LOCK_UNLOCK

        public bool AddLockDetails(LockDetailsViewModel lockModel, ref string message)
        {
            return objLockUnlockDAL.AddLockDetails(lockModel, ref message);
        }

        public Array GetProposalList(string propType, int yearCode, int stateCode, int districtCode, int blockCode, int batchCode, string packageCode, byte scheme, int collaboration, int roleCode, string type, int page, int rows, string sidx, string sord, out long totalRecords, out List<int> lstIds, out string ImsPrRoadCode)
        {
            return objLockUnlockDAL.GetProposalList(propType, yearCode, stateCode, districtCode, blockCode, batchCode, packageCode, scheme, collaboration, roleCode, type, page, rows, sidx, sord, out totalRecords, out lstIds, out ImsPrRoadCode);
        }

        public Array GetProposalLockList(int yearCode, int stateCode, int districtCode, int batchCode, string packageCode, int page, int rows, string sidx, string sord, out long totalRecords, out List<int> lstIds, out string ImsPrRoadCode)
        {
            return objLockUnlockDAL.GetProposalLockList(yearCode, stateCode, districtCode, batchCode, packageCode, page, rows, sidx, sord, out totalRecords, out lstIds, out ImsPrRoadCode);
        }


        public bool LockUnlockProposal(ProposalFilterViewModel model, ref string message)
        {
            return objLockUnlockDAL.LockUnlockProposal(model, ref message);
        }

        public Array GetCoreNetworkList(int stateCode, int districtCode, int blockCode, byte scheme, int collaboration, int roleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return objLockUnlockDAL.GetCoreNetworkList(stateCode, districtCode, blockCode, scheme, collaboration, roleCode, page, rows, sidx, sord, out totalRecords);
        }

        public Array GetCoreNetworkUnlockList(int stateCode, int districtCode, int blockCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return objLockUnlockDAL.GetCoreNetworkUnlockList(stateCode, districtCode, blockCode, page, rows, sidx, sord, out totalRecords);
        }

        public Array GetExistingRoadList(int stateCode, int districtCode, int blockCode, byte scheme, int collaboration, int roleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return objLockUnlockDAL.GetExistingRoadList(stateCode, districtCode, blockCode, scheme, collaboration, roleCode, page, rows, sidx, sord, out totalRecords);
        }

        public Array GetExistingRoadUnlockList(int stateCode, int districtCode, int blockCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return objLockUnlockDAL.GetExistingRoadUnlockList(stateCode, districtCode, blockCode, page, rows, sidx, sord, out totalRecords);
        }

        public Array GetAgreementList(int stateCode, int districtCode, int yearCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return objLockUnlockDAL.GetAgreementList(stateCode, districtCode, yearCode, page, rows, sidx, sord, out totalRecords);
        }

        public Array GetTenderingList(int stateCode, int districtCode, int yearCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return objLockUnlockDAL.GetTenderingList(stateCode, districtCode, yearCode, page, rows, sidx, sord, out totalRecords);
        }

        public Array GetManeCoreNetworkList(int stateCode, int districtCode, int blockCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return objLockUnlockDAL.GetManeCoreNetworkList(stateCode, districtCode, blockCode, page, rows, sidx, sord, out totalRecords);
        }

        public Array GetIMSContractList(int yearCode, int stateCode, int districtCode, int batchCode, string packageCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return objLockUnlockDAL.GetIMSContractList(yearCode, stateCode, districtCode, batchCode, packageCode, page, rows, sidx, sord, out totalRecords);
        }

        // Unlock

        public Array GetStateList(string moduleCode, byte scheme, int collaboration, int roleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                return objLockUnlockDAL.GetStateList(moduleCode, scheme, collaboration, roleCode, page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception)
            {
                totalRecords = 0;
                return null;
            }
        }

        public Array GetDistrictList(string moduleCode, int stateCode, byte scheme, int collaboration, int roleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                return objLockUnlockDAL.GetDistrictList(moduleCode, stateCode, scheme, collaboration, roleCode, page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception)
            {
                totalRecords = 0;
                return null;
            }
        }

        public Array GetYearsList(string moduleCode, int stateCode, int districtCode, int blockCode, byte scheme, int collaboration, int roleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                return objLockUnlockDAL.GetYearsList(moduleCode, stateCode, districtCode, blockCode, scheme, collaboration, roleCode, page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception)
            {
                totalRecords = 0;
                return null;
            }
        }

        public Array GetBatchesList(string moduleCode, int stateCode, int districtCode, int blockCode, int yearCode, byte scheme, int collaboration, int roleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                return objLockUnlockDAL.GetBatchesList(moduleCode, stateCode, districtCode, blockCode, yearCode, scheme, collaboration, roleCode, page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception)
            {
                totalRecords = 0;
                return null;
            }
        }

        public Array GetBlockList(string moduleCode, int stateCode, int districtCode, byte scheme, int collaboration, int roleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                return objLockUnlockDAL.GetBlockList(moduleCode, stateCode, districtCode, scheme, collaboration, roleCode, page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception)
            {
                totalRecords = 0;
                return null;
            }
        }

        public Array GetVillageList(string moduleCode, int blockCode, byte scheme, int collaboration, int roleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                return objLockUnlockDAL.GetVillageList(moduleCode, blockCode, scheme, collaboration, roleCode, page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception)
            {
                totalRecords = 0;
                return null;
            }
        }

        public Array GetHabitationList(string moduleCode, int villageCode, byte scheme, int collaboration, int roleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                return objLockUnlockDAL.GetHabitationList(moduleCode, villageCode, scheme, collaboration, roleCode, page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception)
            {
                totalRecords = 0;
                return null;
            }
        }

        public bool AddUnlockDetails(UnlockDetailsViewModel model, ref string message)
        {
            try
            {
                return objLockUnlockDAL.AddUnlockDetails(model, ref message);
            }
            catch (Exception)
            {
                message = "Error occurred while processing your request.";
                return false;
            }
        }

        public Array GetITNOProposalList(int yearCode, int stateCode, int districtCode, int blockCode, int batchCode, string packageCode, byte scheme, string type, int page, int rows, string sidx, string sord, out long totalRecords, out List<int> lstIds, out string ImsPrRoadCode)
        {
            return objLockUnlockDAL.GetITNOProposalList(yearCode, stateCode, districtCode, blockCode, batchCode, packageCode, scheme, type, page, rows, sidx, sord, out totalRecords, out lstIds, out ImsPrRoadCode);
        }

        public Array GetUnlockRecordListBAL(int stateCode, int moduleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return objLockUnlockDAL.GetUnlockRecordListDAL(stateCode, moduleCode, page, rows, sidx, sord, out totalRecords);
        }


        public Array GetProposalDetails(int levelCode, string levelName, int scheme, string type, string yearbatch, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return objLockUnlockDAL.GetProposalDetails(levelCode, levelName, scheme, type, yearbatch, page, rows, sidx, sord, out totalRecords);
        }

        public Array GetProposalDetailsForITNO(int levelCode, string levelName, int scheme, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return objLockUnlockDAL.GetProposalDetailsForITNO(levelCode, levelName, scheme, page, rows, sidx, sord, out totalRecords);
        }

        public Array GetDRRPDetails(int levelCode, string levelName, int scheme, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return objLockUnlockDAL.GetDRRPDetails(levelCode, levelName, scheme, page, rows, sidx, sord, out totalRecords);
        }

        public Array GetCNDetails(int levelCode, string levelName, int scheme, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return objLockUnlockDAL.GetCNDetails(levelCode, levelName, scheme, page, rows, sidx, sord, out totalRecords);
        }

        public Array GetHabDetails(int levelCode, string levelName, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return objLockUnlockDAL.GetHabDetails(levelCode, levelName, page, rows, sidx, sord, out totalRecords);
        }

        public Array GetVillageDetails(int levelCode, string levelName, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return objLockUnlockDAL.GetVillageDetails(levelCode, levelName, page, rows, sidx, sord, out totalRecords);
        }

        public Array GetUnlockReportList(int stateCode, int districtCode, int blockCode, int moduleCode, int schemeCode, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            return objLockUnlockDAL.GetUnlockReportList(stateCode, districtCode, blockCode, moduleCode, schemeCode, page, rows, sidx, sord, out totalRecords);
        }

        #endregion
    }

    public interface ILockUnlockBAL
    {

        #region LockUnlock

        Array GetProposalsBAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int IMS_YEAR, int IMS_MAST_STATE_CODE, int IMS_BATCH, int Scheme);
        bool FreezeUnfreezeProposal(ProposalFilterLockUnlockViewModel LockUnlockViewModel, ref string message);
        Array GetFreezeUnfreezeReportDetails(int stateCode, int batchCode, int yearCode, int schemeCode, int? page, int? rows, string sidx, string sord, out long totalRecords);


        Array GetProposalList(string propType, int yearCode, int stateCode, int districtCode, int blockCode, int batchCode, string packageCode, byte scheme, int collaboration, int roleCode, string type, int page, int rows, string sidx, string sord, out long totalRecords, out List<int> lstIds, out string ImsPrRoadCode);

        Array GetProposalLockList(int yearCode, int stateCode, int districtCode, int batchCode, string packageCode, int page, int rows, string sidx, string sord, out long totalRecords, out List<int> lstIds, out string ImsPrRoadCode);

        bool AddLockDetails(LockDetailsViewModel lockModel, ref string message);

        bool LockUnlockProposal(ProposalFilterViewModel model, ref string message);

        Array GetCoreNetworkList(int stateCode, int districtCode, int blockCode, byte scheme, int collaboration, int roleCode, int page, int rows, string sidx, string sord, out long totalRecords);

        Array GetExistingRoadList(int stateCode, int districtCode, int blockCode, byte scheme, int collaboration, int roleCode, int page, int rows, string sidx, string sord, out long totalRecords);

        Array GetAgreementList(int stateCode, int districtCode, int yearCode, int page, int rows, string sidx, string sord, out long totalRecords);

        Array GetTenderingList(int stateCode, int districtCode, int yearCode, int page, int rows, string sidx, string sord, out long totalRecords);

        Array GetManeCoreNetworkList(int stateCode, int districtCode, int blockCode, int page, int rows, string sidx, string sord, out long totalRecords);

        Array GetIMSContractList(int yearCode, int stateCode, int districtCode, int batchCode, string packageCode, int page, int rows, string sidx, string sord, out long totalRecords);

        Array GetCoreNetworkUnlockList(int stateCode, int districtCode, int blockCode, int page, int rows, string sidx, string sord, out long totalRecords);

        Array GetExistingRoadUnlockList(int stateCode, int districtCode, int blockCode, int page, int rows, string sidx, string sord, out long totalRecords);

        #endregion LockUnlock

        #region UNLOCK

        Array GetStateList(string moduleCode, byte scheme, int collaboration, int roleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetDistrictList(string moduleCode, int stateCode, byte scheme, int collaboration, int roleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetYearsList(string moduleCode, int stateCode, int districtCode, int blockCode, byte scheme, int collaboration, int roleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetBatchesList(string moduleCode, int stateCode, int districtCode, int blockCode, int yearCode, byte scheme, int collaboration, int roleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetBlockList(string moduleCode, int stateCode, int districtCode, byte scheme, int collaboration, int roleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetVillageList(string moduleCode, int blockCode, byte scheme, int collaboration, int roleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetHabitationList(string moduleCode, int villageCode, byte scheme, int collaboration, int roleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        bool AddUnlockDetails(UnlockDetailsViewModel model, ref string message);
        Array GetITNOProposalList(int yearCode, int stateCode, int districtCode, int blockCode, int batchCode, string packageCode, byte scheme, string type, int page, int rows, string sidx, string sord, out long totalRecords, out List<int> lstIds, out string ImsPrRoadCode);
        Array GetUnlockRecordListBAL(int stateCode, int moduleCode, int page, int rows, string sidx, string sord, out long totalRecords);

        Array GetProposalDetails(int levelCode, string levelName, int scheme, string type, string yearbatch, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetProposalDetailsForITNO(int levelCode, string levelName, int scheme, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetDRRPDetails(int levelCode, string levelName, int scheme, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetCNDetails(int levelCode, string levelName, int scheme, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetHabDetails(int levelCode, string levelName, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetVillageDetails(int levelCode, string levelName, int page, int rows, string sidx, string sord, out long totalRecords);


        Array GetUnlockReportList(int stateCode, int districtCode, int blockCode, int moduleCode, int schemeCode, int? page, int? rows, string sidx, string sord, out long totalRecords);

        #endregion

    }
}