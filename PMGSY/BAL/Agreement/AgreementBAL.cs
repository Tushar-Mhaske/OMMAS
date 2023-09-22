/*----------------------------------------------------------------------------------------

 * Project Id:

 * Project Name:OMMAS2

 * File Name: AgreementBAL.cs

 * Author : Koustubh Nakate

 * Creation Date :18/June/2013

 * Desc : This class is used to call methods from data access layer class.  
 * ---------------------------------------------------------------------------------------*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.DAL.Agreement;
using PMGSY.Models.Agreement;
using PMGSY.Models;

namespace PMGSY.BAL.Agreement
{
    public class AgreementBAL:IAgreementBAL
    {
        IAgreementDAL agreementDAL = new AgreementDAL();

        public bool ChangeTerminatedAgreementMasterStatusBAL(int agreementCode)
        {
            return agreementDAL.ChangeTerminatedAgreementMasterStatusDAL(agreementCode);
        }

        public bool ChangeTerminatedAgreementStatusBAL(int agreementCode, int roadCode)
        { 
            return agreementDAL.ChangeTerminatedAgreementStatusDAL(agreementCode, roadCode);
        }
        
        public Array GetProposedRoadListBAL(bool isSplitWork, int stateCode, int districtCode, int blockCode, int sanctionedYear, string packageID, string proposalType, int adminNDCode,int batch,int collaboration,string upgradationType, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return agreementDAL.GetProposedRoadListDAL(isSplitWork, stateCode, districtCode, blockCode, sanctionedYear, packageID, proposalType, adminNDCode, batch, collaboration, upgradationType, page, rows, sidx, sord, out totalRecords);
        }

        public Array GetProposedRoadITNOListBAL(bool isSplitWork, int stateCode, int districtCode, int blockCode, int sanctionedYear, string packageID, string proposalType, int adminNDCode, int batch, int collaboration, string upgradationType, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return agreementDAL.GetProposedRoadITNOListDAL(isSplitWork, stateCode, districtCode, blockCode, sanctionedYear, packageID, proposalType, adminNDCode, batch, collaboration, upgradationType, page, rows, sidx, sord, out totalRecords);
        }

        public Array GetAgreementDetailsListBAL(int IMSPRRoadCode, string agreementType, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return agreementDAL.GetAgreementDetailsListDAL(IMSPRRoadCode, agreementType, page, rows, sidx, sord, out totalRecords);
        }

        public Array GetAgreementDetailsListITNOBAL(int IMSPRRoadCode, string agreementType, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return agreementDAL.GetAgreementDetailsListITNODAL(IMSPRRoadCode, agreementType, page, rows, sidx, sord, out totalRecords);
        }


        public bool SaveAgreementDetailsBAL(AgreementDetails details_agreement, ref string message)
        {
            return agreementDAL.SaveAgreementDetailsDAL(details_agreement, ref message);
        }


        public ExistingAgreementDetails GetExistingAgreementDetailsBAL(int contractorID, int agreementCode)
        {
            return agreementDAL.GetExistingAgreementDetailsDAL(contractorID, agreementCode);
        }


        public bool SaveExistingAgreementDetailsBAL(ExistingAgreementDetails details_agreement_existing, ref string message)
        {
            return agreementDAL.SaveExistingAgreementDetailsDAL(details_agreement_existing, ref message);
        }


        public Array GetAgreementDetailsListBAL_ByAgreementCode(int agreementCode, int IMSPRRoadCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return agreementDAL.GetAgreementDetailsListDAL_ByAgreementCode(agreementCode, IMSPRRoadCode, page, rows, sidx, sord, out totalRecords);
        }

        public Array GetAgreementDetailsListITNOBAL_ByAgreementCode(int agreementCode, int IMSPRRoadCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return agreementDAL.GetAgreementDetailsListITNODAL_ByAgreementCode(agreementCode, IMSPRRoadCode, page, rows, sidx, sord, out totalRecords);
        }


        public AgreementDetails GetAgreementMasterDetailsBAL_ByAgreementCode(int agreementCode, bool isView = false)
        {
            return agreementDAL.GetAgreementMasterDetailsDAL_ByAgreementCode(agreementCode, isView);
        }


        public bool UpdateAgreementMasterDetailsBAL(AgreementDetails master_agreement, ref string message)
        {
            return agreementDAL.UpdateAgreementMasterDetailsDAL(master_agreement,ref message);
        }

        public bool UpdateAgreementMasterDetailsITNOBAL(AgreementDetails master_agreement, ref string message)
        {
            return agreementDAL.UpdateAgreementMasterDetailsITNODAL(master_agreement, ref message);
        }


        public bool DeleteAgreementMasterDetailsBAL_ByAgreementCode(int agreementCode, ref string message)
        {
            return agreementDAL.DeleteAgreementMasterDetailsDAL_ByAgreementCode(agreementCode, ref message);
        }


        public ExistingAgreementDetails GetAgreementDetailsBAL_ByAgreementID(int agreementCode, int agreementID)
        {
            return agreementDAL.GetAgreementDetailsDAL_ByAgreementID(agreementCode, agreementID);
        }


        public bool UpdateAgreementDetailsBAL(ExistingAgreementDetails details_agreement, ref string message)
        {
            return agreementDAL.UpdateAgreementDetailsDAL(details_agreement, ref message);
        }

        public bool UpdateAgreementDetailsITNOBAL(ExistingAgreementDetails details_agreement, ref string message)
        {
            return agreementDAL.UpdateAgreementDetailsITNODAL(details_agreement, ref message);
        }


        public bool DeleteAgreementDetailsBAL_ByAgreementID(int agreementID, int agreementCode, ref string message)
        {
            return agreementDAL.DeleteAgreementDetailsDAL_ByAgreementID(agreementID,agreementCode, ref message);
        }

        public bool ChangeAgreementStatusToInCompleteBAL(IncompleteReason incompleteReason, ref string message)
        {
            return agreementDAL.ChangeAgreementStatusToInCompleteDAL(incompleteReason, ref message);
        }

        public bool CheckSplitWorkFinalizedBAL(int IMSPRRoadCode)
        {
            return agreementDAL.CheckSplitWorkFinalizedDAL(IMSPRRoadCode);
        }

        public bool CheckforActiveAgreementBAL(int IMSPRRoadCode, string agreementType, ref bool isAgreementAvailable)
        {
            return agreementDAL.CheckforActiveAgreementDAL(IMSPRRoadCode, agreementType, ref isAgreementAvailable);
        }


        #region Agreement Without Road
        public Array GetAgreementDetailsListBAL_WithoutRoad(int agreementYear, string status, string agreementType, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return agreementDAL.GetAgreementDetailsListDAL_WithoutRoad(agreementYear, status, agreementType, page, rows, sidx, sord, out totalRecords);
        }


        public bool SaveAgreementDetailsBAL_WithoutRoad(AgreementDetails details_agreement, ref string message)
        {
            return agreementDAL.SaveAgreementDetailsDAL_WithoutRoad(details_agreement, ref message);
        }


        public bool UpdateAgreementMasterDetailsBAL_WithoutRoad(AgreementDetails details_agreement, ref string message)
        {
            return agreementDAL.UpdateAgreementMasterDetailsDAL_WithoutRoad(details_agreement, ref message);
        }


        public bool FinalizeAgreementBAL(int agreementCode)
        {
            return agreementDAL.FinalizeAgreementDAL(agreementCode);
        }

        public bool DeFinalizeAgreementBAL(int agreementCode)
        {
            return agreementDAL.DeFinalizeAgreementDAL(agreementCode);
        }

        public bool ChangeAgreementStatusToCompleteBAL(int agreementCode)
        {
            return agreementDAL.ChangeAgreementStatusToCompleteDAL(agreementCode);
        }

        #endregion Agreement Without Road

        #region PROPOSAL_RELATED_DETAILS

        public Array GetProposalAgreementListBAL(int IMSPRRoadCode, string agreementType, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return agreementDAL.GetProposalAgreementListDAL(IMSPRRoadCode, agreementType, page, rows, sidx, sord, out totalRecords);
        }
        

        #endregion

        #region FINALIZE_AGREEMENT

        public Array GetAgreementListBAL(int page, int rows, string sidx, string sord, int yearCode, int blockCode, string package, string proposalType, string agreementStatus, string finalize, string agreementType, out long totalRecords)
        {
            return agreementDAL.GetAgreementListDAL( page, rows, sidx, sord, yearCode,blockCode,package,proposalType,agreementStatus,finalize,agreementType,out totalRecords);
        }

        #endregion

        #region SPECIAL_AGREEMENT

        public Array GetSpecialAgreementProposedRoadListBAL(bool isSplitWork, int stateCode, int districtCode, int blockCode, int sanctionedYear, string packageID, string proposalType, int adminNDCode, int batch, int collaboration, string upgradationType, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return agreementDAL.GetSpecialAgreementProposedRoadListDAL(isSplitWork, stateCode, districtCode, blockCode, sanctionedYear, packageID, proposalType, adminNDCode, batch, collaboration, upgradationType, page, rows, sidx, sord, out totalRecords);
        }

        public bool SaveSpecialAgreementDetailsBAL(SpecialAgreementDetails details_agreement, ref string message)
        {
            return agreementDAL.SaveSpecialAgreementDetailsDAL(details_agreement, ref message);
        }

        public bool UpdateSpecialAgreementMasterDetailsBAL(SpecialAgreementDetails master_agreement, ref string message)
        {
            return agreementDAL.UpdateSpecialAgreementMasterDetailsDAL(master_agreement, ref message);
        }

        public Array GetSpecialAgreementDetailsListBAL(int IMSPRRoadCode, string agreementType, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return agreementDAL.GetSpecialAgreementDetailsListDAL(IMSPRRoadCode, agreementType, page, rows, sidx, sord, out totalRecords);
        }

        public SpecialAgreementDetails GetSpecialAgreementMasterDetailsBAL_ByAgreementCode(int agreementCode, bool isView = false)
        {
            return agreementDAL.GetSpecialAgreementMasterDetailsDAL_ByAgreementCode(agreementCode, isView);
        }

        public bool UpdateSpecialAgreementDetailsBAL(ExistingSpecialAgreementDetails details_agreement, ref string message)
        {
            return agreementDAL.UpdateSpecialAgreementDetailsDAL(details_agreement, ref message);
        }

        public ExistingSpecialAgreementDetails GetSpecialAgreementDetailsBAL_ByAgreementID(int agreementCode, int agreementID)
        {
            return agreementDAL.GetSpecialAgreementDetailsDAL_ByAgreementID(agreementCode, agreementID);
        }

        #endregion

        #region AGREEMENT_UPDATION

        public Array GetAgreementListForUpdationBAL(int page, int rows, string sidx, string sord, int yearCode, int blockCode, string package, string proposalType, string agreementStatus, string finalize, string agreementType,int state,int district, out long totalRecords)
        {
            return agreementDAL.GetAgreementListForUpdationDAL(page, rows, sidx, sord, yearCode, blockCode, package, proposalType, agreementStatus, finalize, agreementType,state,district, out totalRecords);
        }

        public bool ChangeAgreementStatusBAL(int agreementId)
        {
            return agreementDAL.ChangeAgreementStatusDAL(agreementId);
        }

        #endregion

        #region 01 June 2016

        public MASTER_CONTRACTOR_BANK GetContratorBankAccNoAndIFSCcode(int contractorId)
        {
            return agreementDAL.GetContratorBankAccNoAndIFSCcode(contractorId);
        }
        #endregion 

        #region BANK Guarantee

        public Array GetAgreementListBALForBG(int page, int rows, string sidx, string sord, int yearCode, int blockCode, string package, string proposalType, string agreementStatus, string finalize, string agreementType, out long totalRecords)
        {
            return agreementDAL.GetAgreementListBALForBG(page, rows, sidx, sord, yearCode, blockCode, package, proposalType, agreementStatus, finalize, agreementType, out totalRecords);
        }
        public Array GetAgreementDetailsListForContractor(String parameter, String hash, String key, int page, int rows, string sidx, string sord, int yearCode, int blockCode, string package, string proposalType, string agreementStatus, string finalize, string agreementType, out long totalRecords)
        {
            return agreementDAL.GetAgreementDetailsListForContractor(parameter, hash, key, page, rows, sidx, sord, yearCode, blockCode, package, proposalType, agreementStatus, finalize, agreementType, out totalRecords);
        }

        public bool AddBankGuaranteeDetails(BankGuaranteeDetailsModel model, out String IsValid)
        {
            return agreementDAL.AddBankGuaranteeDetails(model, out IsValid);
        }
        public BankGuaranteeDetailsModel GetBankGuaranteeObj(string AgreementCode)
        {
            return agreementDAL.GetBankGuaranteeObj(AgreementCode);
        }


        public bool EditBankGuaranteeDetails(BankGuaranteeDetailsModel model, out String isValidMsg)
        {
            return agreementDAL.EditBankGuaranteeDetails(model, out isValidMsg);
        }


        public Array GetExpBankGuaranteeList(int page, int rows, string sidx, string sord, int yearCode, int blockCode, string package, string proposalType, string agreementStatus, string finalize, string agreementType, int districtCode, String ActiveStatus, out long totalRecords)
        {
            return agreementDAL.GetExpBankGuaranteeList(page, rows, sidx, sord, yearCode, blockCode, package, proposalType, agreementStatus, finalize, agreementType, districtCode, ActiveStatus, out totalRecords);
        }

        public int GetExpiredBankGuaranteeCount()
        {
            return agreementDAL.GetExpiredBankGuaranteeCount();
        }
        #endregion
    }

    public interface IAgreementBAL
    {

        bool ChangeTerminatedAgreementMasterStatusBAL(int agreementCode);

        bool ChangeTerminatedAgreementStatusBAL(int agreementCode, int roadCode);

        Array GetProposedRoadListBAL(bool isSplitWork, int stateCode, int districtCode, int blockCode, int sanctionedYear, string packageID, string proposalType, int adminNDCode,int batch,int collaboration,string upgradationType, int page, int rows, string sidx, string sord, out long totalRecords);

        Array GetProposedRoadITNOListBAL(bool isSplitWork, int stateCode, int districtCode, int blockCode, int sanctionedYear, string packageID, string proposalType, int adminNDCode, int batch, int collaboration, string upgradationType, int page, int rows, string sidx, string sord, out long totalRecords);

        Array GetAgreementDetailsListBAL(int IMSPRRoadCode,string agreementType, int page, int rows, string sidx, string sord, out long totalRecords);

        Array GetAgreementDetailsListITNOBAL(int IMSPRRoadCode, string agreementType, int page, int rows, string sidx, string sord, out long totalRecords);

        bool SaveAgreementDetailsBAL(AgreementDetails details_agreement, ref string message);

        ExistingAgreementDetails GetExistingAgreementDetailsBAL(int contractorID, int agreementCode);

        bool SaveExistingAgreementDetailsBAL(ExistingAgreementDetails details_agreement_existing, ref string message);

        Array GetAgreementDetailsListBAL_ByAgreementCode(int agreementCode,int IMSPRRoadCode, int page, int rows, string sidx, string sord, out long totalRecords);

        Array GetAgreementDetailsListITNOBAL_ByAgreementCode(int agreementCode, int IMSPRRoadCode, int page, int rows, string sidx, string sord, out long totalRecords);

        AgreementDetails GetAgreementMasterDetailsBAL_ByAgreementCode(int agreementCode,bool isView=false);

        bool UpdateAgreementMasterDetailsBAL(AgreementDetails master_agreement, ref string message);

        bool UpdateAgreementMasterDetailsITNOBAL(AgreementDetails master_agreement, ref string message);

        bool DeleteAgreementMasterDetailsBAL_ByAgreementCode(int agreementCode, ref string message);

        ExistingAgreementDetails GetAgreementDetailsBAL_ByAgreementID(int agreementCode, int agreementID);

        bool UpdateAgreementDetailsBAL(ExistingAgreementDetails details_agreement, ref string message);

        bool DeleteAgreementDetailsBAL_ByAgreementID(int agreementID, int agreementCode, ref string message);

        Array GetAgreementDetailsListBAL_WithoutRoad(int agreementYear, string status, string agreementType, int page, int rows, string sidx, string sord, out long totalRecords);

        bool SaveAgreementDetailsBAL_WithoutRoad(AgreementDetails details_agreement, ref string message);

        bool UpdateAgreementMasterDetailsBAL_WithoutRoad(AgreementDetails details_agreement, ref string message);

        bool FinalizeAgreementBAL(int agreementCode);

        bool ChangeAgreementStatusToInCompleteBAL(IncompleteReason incompleteReason, ref string message);

        bool CheckSplitWorkFinalizedBAL(int IMSPRRoadCode);

        bool CheckforActiveAgreementBAL(int IMSPRRoadCode, string agreementType, ref bool isAgreementAvailable);

        bool ChangeAgreementStatusToCompleteBAL(int agreementCode);

        bool UpdateAgreementDetailsITNOBAL(ExistingAgreementDetails details_agreement, ref string message);

        #region PROPOSAL_RELATED_DETAILS

        Array GetProposalAgreementListBAL(int IMSPRRoadCode, string agreementType, int page, int rows, string sidx, string sord, out long totalRecords);

        #endregion

        #region FINALIZE_AGREEMENT

        Array GetAgreementListBAL(int page, int rows, string sidx, string sord, int yearCode, int blockCode, string package, string proposalType, string agreementStatus, string finalize,string agreementType, out long totalRecords);
        bool DeFinalizeAgreementBAL(int agreementCode);
        #endregion

        #region SPECIAL_AGREEMENT

        Array GetSpecialAgreementProposedRoadListBAL(bool isSplitWork, int stateCode, int districtCode, int blockCode, int sanctionedYear, string packageID, string proposalType, int adminNDCode, int batch, int collaboration, string upgradationType, int page, int rows, string sidx, string sord, out long totalRecords);
        bool SaveSpecialAgreementDetailsBAL(SpecialAgreementDetails details_agreement, ref string message);
        bool UpdateSpecialAgreementMasterDetailsBAL(SpecialAgreementDetails master_agreement, ref string message);
        Array GetSpecialAgreementDetailsListBAL(int IMSPRRoadCode, string agreementType, int page, int rows, string sidx, string sord, out long totalRecords);
        SpecialAgreementDetails GetSpecialAgreementMasterDetailsBAL_ByAgreementCode(int agreementCode, bool isView = false);
        bool UpdateSpecialAgreementDetailsBAL(ExistingSpecialAgreementDetails details_agreement, ref string message);
        ExistingSpecialAgreementDetails GetSpecialAgreementDetailsBAL_ByAgreementID(int agreementCode, int agreementID);

        #endregion

        #region AGREEMENT_UPDATION

        Array GetAgreementListForUpdationBAL(int page, int rows, string sidx, string sord, int yearCode, int blockCode, string package, string proposalType, string agreementStatus, string finalize, string agreementType,int state,int district, out long totalRecords);
        bool ChangeAgreementStatusBAL(int agreementId);

        #endregion

        #region  01 June 2016
        MASTER_CONTRACTOR_BANK GetContratorBankAccNoAndIFSCcode(int contractorId);
        #endregion

        #region BANK Guarantee
        Array GetAgreementListBALForBG(int page, int rows, string sidx, string sord, int yearCode, int blockCode, string package, string proposalType, string agreementStatus, string finalize, string agreementType, out long totalRecords);
        Array GetAgreementDetailsListForContractor(String parameter, String hash, String key, int page, int rows, string sidx, string sord, int yearCode, int blockCode, string package, string proposalType, string agreementStatus, string finalize, string agreementType, out long totalRecords);
        bool AddBankGuaranteeDetails(BankGuaranteeDetailsModel model, out String IsValid);
        bool EditBankGuaranteeDetails(BankGuaranteeDetailsModel model, out String isValidMsg);
        BankGuaranteeDetailsModel GetBankGuaranteeObj(string AgreementCode);
        Array GetExpBankGuaranteeList(int page, int rows, string sidx, string sord, int yearCode, int blockCode, string package, string proposalType, string agreementStatus, string finalize, string agreementType, int districtCode, String ActiveStatus, out long totalRecords);
        int GetExpiredBankGuaranteeCount();
        #endregion

    }
}