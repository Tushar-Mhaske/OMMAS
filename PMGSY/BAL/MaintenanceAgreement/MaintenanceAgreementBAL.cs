/*----------------------------------------------------------------------------------------

 * Project Id:

 * Project Name:OMMAS2

 * File Name: MaintenanceAgreementBAL.cs

 * Author : Koustubh Nakate

 * Creation Date :21/June/2013

 * Desc : This class is used to call methods from data access layer class.  
 * ---------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.DAL.MaintenanceAgreement;
using PMGSY.Models.MaintenanceAgreement;
using System.Web.Mvc;
using PMGSY.Models.Proposal;

namespace PMGSY.BAL.MaintenanceAgreement
{
    public class MaintenanceAgreementBAL:IMaintenanceAgreementBAL
    {
        IMaintenanceAgreementDAL maintenanceAgreementDAL = new MaintenanceAgreementDAL();

        #region MAINTENANCE_AGREEMENT

        public Array GetCompletedRoadListBAL(int stateCode, int districtCode, int blockCode, int sanctionedYear,string packageID, int adminNDCode,int batch,int collaboration,string upgradationType, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return maintenanceAgreementDAL.GetCompletedRoadListDAL(stateCode, districtCode, blockCode, sanctionedYear, packageID, adminNDCode, batch, collaboration, upgradationType, page, rows, sidx, sord, out totalRecords);
        }


        public Array GetAgreementDetailsListBAL_Proposal(int IMSPRRoadCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return maintenanceAgreementDAL.GetAgreementDetailsListDAL_Proposal(IMSPRRoadCode, page, rows, sidx, sord, out totalRecords);
        }

        public bool SaveAgreementDetailsBAL_Proposal(MaintenanceAgreementDetails details_agreement, ref string message)
        {
            return maintenanceAgreementDAL.SaveAgreementDetailsDAL_Proposal(details_agreement, ref message);
        }


        public bool GetExistingAgreementDetailsBAL(int IMSPRRoadCode, int IMSWorkCode, ref int contractorID, ref string agreementNumber, ref  string agreementDate, ref decimal? year1, ref decimal? year2, ref decimal? year3, ref decimal? year4, ref decimal? year5, ref string message)
        {
            return maintenanceAgreementDAL.GetExistingAgreementDetailsDAL(IMSPRRoadCode, IMSWorkCode, ref contractorID, ref agreementNumber, ref agreementDate, ref year1, ref year2, ref year3, ref year4, ref year5, ref message);
        }


        public MaintenanceAgreementDetails GetMaintenanceAgreementDetailsBAL(int IMSPRRoadCode, int PRContractCode, int ManeContractId, bool isView = false)
        {
            return maintenanceAgreementDAL.GetMaintenanceAgreementDetailsDAL(IMSPRRoadCode, PRContractCode,ManeContractId, isView);
        }


        public bool UpdateMaintenanceAgreementDetailsBAL_Proposal(MaintenanceAgreementDetails details_agreement, ref string message)
        {
            return maintenanceAgreementDAL.UpdateMaintenanceAgreementDetailsDAL_Proposal(details_agreement, ref message);
        }


        public bool FinalizeAgreementBAL(int IMSPRRoadCode, int PRContractCode, int ManeContractId)
        {
            return maintenanceAgreementDAL.FinalizeAgreementDAL(IMSPRRoadCode, PRContractCode,ManeContractId);
        }

        public bool DeFinalizeAgreementBAL(int IMSPRRoadCode, int PRContractCode, int ManeContractId)
        {
            return maintenanceAgreementDAL.DeFinalizeAgreementDAL(IMSPRRoadCode, PRContractCode,ManeContractId);
        }


        public bool ChangeAgreementStatusToInCompleteBAL(Models.Agreement.IncompleteReason incompleteReason, ref string message)
        {
            return maintenanceAgreementDAL.ChangeAgreementStatusToInCompleteDAL(incompleteReason, ref message);
        }


        public bool DeleteMaintenanceAgreementDetailsBAL(int IMSPRRoadCode, int PRContractCode, int ManeContractId, ref string message)
        {
            return maintenanceAgreementDAL.DeleteMaintenanceAgreementDetailsDAL(IMSPRRoadCode, PRContractCode,ManeContractId, ref message);
        }


        public bool CheckForExistingorNewContractorBAL(int IMSPRRoadCode, int IMSWorkCode)
        {
            return maintenanceAgreementDAL.CheckForExistingorNewContractorDAL(IMSPRRoadCode, IMSWorkCode);
        }


        public bool ChangeAgreementStatusToCompleteBAL(int IMSPRRoadCode, int PRContractCode)
        {
            return maintenanceAgreementDAL.ChangeAgreementStatusToCompleteDAL(IMSPRRoadCode, PRContractCode);
        }

        #endregion

        #region SPECIAL_MAINTENANCE_AGREEMENT

        public Array GetCompletedRoadForSpecialAgreementsListBAL(int stateCode, int districtCode, int blockCode, int sanctionedYear, string packageID, int adminNDCode, int batch, int collaboration, string upgradationType, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return maintenanceAgreementDAL.GetCompletedRoadForSpecialAgreementsListDAL(stateCode, districtCode, blockCode, sanctionedYear, packageID,adminNDCode,batch,collaboration,upgradationType,page,rows,sidx,sord,out totalRecords);
        }

        public Array GetSpecialAgreementDetailsListBAL(int IMSPRRoadCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return maintenanceAgreementDAL.GetSpecialAgreementDetailsListDAL(IMSPRRoadCode, page, rows, sidx, sord, out totalRecords);
        }

        public bool SaveSpecialAgreementDetailsBAL(MaintenanceAgreementDetails model,ref string message)
        {
            return maintenanceAgreementDAL.SaveSpecialAgreementDetailsDAL(model,ref message);
        }

        public bool UpdateSpecialAgreementDetailsBAL(MaintenanceAgreementDetails model, ref string message)
        {
            return maintenanceAgreementDAL.UpdateSpecialAgreementDetailsDAL(model, ref message);
        }

        #endregion

        #region PROPOSAL_RELATED_DETAILS

        public Array GetProposalAgreementListBAL(int IMSPRRoadCode, string agreementType, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return maintenanceAgreementDAL.GetProposalAgreementListDAL(IMSPRRoadCode, agreementType, page, rows, sidx, sord, out totalRecords);
        }

        public Array GetProposalFinancialListBAL(int IMSPRRoadCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return maintenanceAgreementDAL.GetProposalFinancialListDAL(IMSPRRoadCode, page, rows, sidx, sord, out totalRecords);
        }


        #endregion

        #region TECHNOLOGY


        public bool AddTechnologyDetailsBAL(MaintenanceTechnologyDetailsViewModel model, ref string message)
        {
            try
            {
                var objMantenanceAgreementDAL = new MaintenanceAgreementDAL();
                return objMantenanceAgreementDAL.AddTechnologyDetailsDAL(model, ref message);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error occurred while processing your request.";
                return false;
            }
        }

        public bool EditTechnologyDetailsBAL(MaintenanceTechnologyDetailsViewModel model, ref string message)
        {
            try
            {
                var objMaintenanceAgreementDAL = new MaintenanceAgreementDAL();
                return objMaintenanceAgreementDAL.EditTechnologyDetailsDAL(model, ref message);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error occurred while processing your request.";
                return false;
            }
        }

        public Array GetTechnologyDetailsListBAL(int? page, int? rows, string sidx, string sord, out long totalRecords, int proposalCode, int contractCode)
        {
            try
            {
                //IProposalDAL objDAL = new ProposalDAL();
                var objMaintenanceAgreementDAL = new MaintenanceAgreementDAL();
                return objMaintenanceAgreementDAL.GetTechnologyDetailsListDAL(page, rows, sidx, sord, out totalRecords, proposalCode, contractCode);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
        }

        public MaintenanceTechnologyDetailsViewModel GetTechnologyDetails(int proposalCode, int contractCode, int segmentCode)
        {
            try
            {
                var objMaintenanceAgreementDAL = new MaintenanceAgreementDAL();
                return objMaintenanceAgreementDAL.GetTechnologyDetails(proposalCode, contractCode, segmentCode);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return null;
            }
        }

        public bool DeleteTechnologyDetails(int proposalCode, int segmentCode, int contractCode)
        {
            try
            {
                var objMaintenanceAgreementDAL = new MaintenanceAgreementDAL();
                return objMaintenanceAgreementDAL.DeleteTechnologyDetails(proposalCode, segmentCode, contractCode);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
        }

        public decimal? GetTechnologyStartChainage(int proposalCode, int techCode, int layerCode)
        {
            try
            {

                var objMaintenanceAgreementDAL = new MaintenanceAgreementDAL();
                return objMaintenanceAgreementDAL.GetTechnologyStartChainage(proposalCode, techCode, layerCode);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return 0;
            }
        }

        #endregion

        #region Periodic Maintenance

        public Array GetPeriodicCompletedRoadListBAL(int stateCode, int districtCode, int blockCode, int sanctionedYear, string packageID, int adminNDCode, int batch, int collaboration, string upgradationType, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return maintenanceAgreementDAL.GetPeriodicCompletedRoadListDAL(stateCode, districtCode, blockCode, sanctionedYear, packageID, adminNDCode, batch, collaboration, upgradationType, page, rows, sidx, sord, out totalRecords);
        }
        public Boolean AddPeriodicMaintenanceBAL(AddPeriodicMaintenanceModel model, out String message)
        {
            return maintenanceAgreementDAL.AddPeriodicMaintenanceDAL(model, out message);
        }

        public AddPeriodicMaintenanceModel GetPeriodicMentainanceModelBAL(Int32 imsRoadCode)
        {
            return maintenanceAgreementDAL.GetPeriodicMentainanceModelDAL(imsRoadCode);
        }
        public Boolean EditPeriodicMaintenanceBAL(AddPeriodicMaintenanceModel model, out String message)
        {
            return maintenanceAgreementDAL.EditPeriodicMaintenanceDAL(model, out message);
        }
        public List<AddPeriodicMaintenanceModel> GetPariodicMaintenceViewListBAL(int imsRoadCode, double RoadLength)
        {
            return maintenanceAgreementDAL.GetPariodicMaintenceViewListDAL(imsRoadCode, RoadLength);
        }

        public Array ViewPeriodicmaintenanceListBAL(int RodeCode, int page, int rows, string sidx, string sord, out long totalRecords, decimal RoadLength, out String AddButton)
        {
            return maintenanceAgreementDAL.ViewPeriodicmaintenanceListDAL(RodeCode, page, rows, sidx, sord, out totalRecords, RoadLength, out AddButton);
        }
        #endregion

        #region Maintenance Proposal Re-packaging

        public Array GetMaintenanceProposalsForRepackaging(int? page, int? rows, string sidx, string sord, out long totalRecords, int year, int batch, int block, string package, int collaboration, string proposalType, string upgradationType)
        {
            return maintenanceAgreementDAL.GetMaintenanceProposalsForRepackaging(page, rows, sidx, sord, out totalRecords, year, batch, block, package, collaboration, proposalType, upgradationType);
        }
        public bool AddMaintenanceRepackagingDetails(PMGSY.Models.MaintenanceAgreement.RepackagingDetailsViewModel model)
        {
            return maintenanceAgreementDAL.AddMaintenanceRepackagingDetails(model);
        }


        #endregion

        #region Terminated Package Agreement

        public Array GetTerminatedPackageListBAL(int stateCode, int yearCode, int districtCode, int blockCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return maintenanceAgreementDAL.GetTerminatedPackageListDAL(stateCode, yearCode, districtCode, blockCode, page, rows, sidx, sord, out totalRecords);
        }

        public bool SaveContractorDetailsBAL(TerminatedAgreementDetails details_agreement, ref string message)
        {
            return maintenanceAgreementDAL.SaveContractorDetailsDAL(details_agreement, ref message);
        }

        #endregion
    }

    public interface IMaintenanceAgreementBAL
    {
        #region MAINTENANCE_AGREEMENT

        Array GetCompletedRoadListBAL(int stateCode, int districtCode, int blockCode, int sanctionedYear,string packageID, int adminNDCode,int batch,int collaboration,string upgradationType, int page, int rows, string sidx, string sord, out long totalRecords);

        Array GetAgreementDetailsListBAL_Proposal(int IMSPRRoadCode,int page, int rows, string sidx, string sord, out long totalRecords);

        bool SaveAgreementDetailsBAL_Proposal(MaintenanceAgreementDetails details_agreement, ref string message);

        bool GetExistingAgreementDetailsBAL(int IMSPRRoadCode, int IMSWorkCode, ref int contractorID, ref string agreementNumber, ref  string agreementDate, ref decimal? year1, ref decimal? year2, ref decimal? year3, ref decimal? year4, ref decimal? year5, ref string message);

        MaintenanceAgreementDetails GetMaintenanceAgreementDetailsBAL(int IMSPRRoadCode, int PRContractCode, int ManeContractId, bool isView = false);

        bool UpdateMaintenanceAgreementDetailsBAL_Proposal(MaintenanceAgreementDetails details_agreement, ref string message);

        bool FinalizeAgreementBAL(int IMSPRRoadCode, int PRContractCode,int ManeContractId);

        bool DeFinalizeAgreementBAL(int IMSPRRoadCode, int PRContractCode, int ManeContractId);

        bool ChangeAgreementStatusToInCompleteBAL(Models.Agreement.IncompleteReason incompleteReason, ref string message);

        bool DeleteMaintenanceAgreementDetailsBAL(int IMSPRRoadCode, int PRContractCode, int ManeContractId, ref string message);

        bool CheckForExistingorNewContractorBAL(int IMSPRRoadCode, int IMSWorkCode);

        bool ChangeAgreementStatusToCompleteBAL(int IMSPRRoadCode, int PRContractCode);

        #endregion

        #region SPECIAL_MAINTENANCE_AGREEMENT

        Array GetCompletedRoadForSpecialAgreementsListBAL(int stateCode, int districtCode, int blockCode, int sanctionedYear, string packageID, int adminNDCode, int batch, int collaboration, string upgradationType, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetSpecialAgreementDetailsListBAL(int IMSPRRoadCode, int page, int rows, string sidx,string sord, out long totalRecords);
        bool SaveSpecialAgreementDetailsBAL(MaintenanceAgreementDetails model,ref string message);
        bool UpdateSpecialAgreementDetailsBAL(MaintenanceAgreementDetails model, ref string message);

        #endregion

        #region PROPOSAL_RELATED_DETAILS

        Array GetProposalAgreementListBAL(int IMSPRRoadCode, string agreementType, int page, int rows, string sidx, string sord, out long totalRecords);

        Array GetProposalFinancialListBAL(int IMSPRRoadCode, int page, int rows, string sidx, string sord, out long totalRecords);

        #endregion

        #region Technology

        bool AddTechnologyDetailsBAL(MaintenanceTechnologyDetailsViewModel model, ref string message);
        Array GetTechnologyDetailsListBAL(int? page, int? rows, string sidx, string sord, out long totalRecords, int proposalCode, int contractCode);
        decimal? GetTechnologyStartChainage(int proposalCode, int techCode, int layerCode);
        bool DeleteTechnologyDetails(int proposalCode, int segmentCode, int contractCode);
        MaintenanceTechnologyDetailsViewModel GetTechnologyDetails(int proposalCode, int contractCode, int segmentCode);
        bool EditTechnologyDetailsBAL(MaintenanceTechnologyDetailsViewModel model, ref string message);
        
        #endregion

        #region Periodic Maintenance
        Array GetPeriodicCompletedRoadListBAL(int stateCode, int districtCode, int blockCode, int sanctionedYear, string packageID, int adminNDCode, int batch, int collaboration, string upgradationType, int page, int rows, string sidx, string sord, out long totalRecords);
        Boolean AddPeriodicMaintenanceBAL(AddPeriodicMaintenanceModel model, out String message);
        AddPeriodicMaintenanceModel GetPeriodicMentainanceModelBAL(Int32 imsRoadCode);
        Boolean EditPeriodicMaintenanceBAL(AddPeriodicMaintenanceModel model, out String message);
        List<AddPeriodicMaintenanceModel> GetPariodicMaintenceViewListBAL(int imsRoadCode, double RoadLength);
        Array ViewPeriodicmaintenanceListBAL(int RodeCode, int page, int rows, string sidx, string sord, out long totalRecords, decimal RoadLength, out String AddButton);
        #endregion

        #region Maintenance Work Repackaging

        Array GetMaintenanceProposalsForRepackaging(int? page, int? rows, string sidx, string sord, out long totalRecords, int year, int batch, int block, string package, int collaboration, string proposalType, string upgradationType);

        bool AddMaintenanceRepackagingDetails(PMGSY.Models.MaintenanceAgreement.RepackagingDetailsViewModel model);

        #endregion


        #region Terminated Package Agreement

        Array GetTerminatedPackageListBAL(int stateCode, int yearCode, int districtCode, int blockCode, int page, int rows, string sidx, string sord, out long totalRecords);
        bool SaveContractorDetailsBAL(TerminatedAgreementDetails details_agreement, ref string message);

        #endregion
    }
}