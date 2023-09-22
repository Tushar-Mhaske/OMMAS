
/*----------------------------------------------------------------------------------------
 * Project Id    :

 * Project Name  : OMMAS-II

 * File Name     : FortyPointChecklistBAL.cs

 * Author        : Abhishek Kamble.
 
 * Creation Date : 20/Nov/2013

 * Desc          : This class is used to call methods from data access layer class of Forty Point check list .  
 
 * ---------------------------------------------------------------------------------------*/

using PMGSY.DAL.FortyPointChecklist;
using PMGSY.Models.FortyPointCheckList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.BAL.FortyPointChecklist
{
    public class FortyPointChecklistBAL:IFortyPointChecklistBAL
    {
        FortyPointChecklistDAL fortyPointChecklistDALContext = new FortyPointChecklistDAL();

        #region Employment Information Details

            public Array EmploymentInformationDetailsBAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int adminCode)
            {
                return fortyPointChecklistDALContext.EmploymentInformationDetailsDAL(page, rows, sidx, sord, out  totalRecords, stateCode, adminCode);
            }

            public bool AddEmploymentDetails(EmploymentInformationViewModel employmentViewModel, ref string message)
            {
                return fortyPointChecklistDALContext.AddEmploymentDetails(employmentViewModel, ref message);
            }

            public bool EditEmploymentDetails(EmploymentInformationViewModel employmentViewModel, ref string message)
            {
                return fortyPointChecklistDALContext.EditEmploymentDetails(employmentViewModel, ref message);     
            }

            public EmploymentInformationViewModel GetEmploymentInformationDetails(int employmentId)
            {
                return fortyPointChecklistDALContext.GetEmploymentInformationDetails(employmentId);
            }

            public bool DeleteEmploymentInformationDetails(int employmentId, ref string message)
            {
                return fortyPointChecklistDALContext.DeleteEmploymentInformationDetails(employmentId,ref message);
            }

        #endregion Employment Information Details

        #region Tender Cost Information

            public Array TenderCostInformationDetailsDAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int adminCode)
            {
                return fortyPointChecklistDALContext.TenderCostInformationDetailsDAL(page, rows, sidx, sord, out totalRecords, stateCode, adminCode);
            }

            public bool AddTenderCostInformationDetails(TenderCostInformationViewModel tenderCostInformationViewModel, ref string message)
            {
                return fortyPointChecklistDALContext.AddTenderCostInformationDetails(tenderCostInformationViewModel, ref message);
            }

            public bool EditCostInformationDetails(TenderCostInformationViewModel tenderCostInformationViewModel, ref string message)
            {
                return fortyPointChecklistDALContext.EditCostInformationDetails(tenderCostInformationViewModel, ref message);    
            }

            public TenderCostInformationViewModel GetTenderCostInformationDetails(int priceId)
            {
                return fortyPointChecklistDALContext.GetTenderCostInformationDetails(priceId);                     
            }

            public bool DeleteTenderCostInformationDetails(int priceId, ref string message)
            {
                return fortyPointChecklistDALContext.DeleteTenderCostInformationDetails(priceId, ref message);
            }                                                                                          

        #endregion Tender Cost Information

        #region Tender Equipment

            public Array ListTenderEquipmentDetails(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int adminCode)
            {
                return fortyPointChecklistDALContext.ListTenderEquipmentDetails(page, rows, sidx, sord,out totalRecords, stateCode, adminCode);
            }

            public bool AddTenderEquipmentDetails(TenderEquipmentViewModel tenderequipmentViewModel, ref string message)
            {
                return fortyPointChecklistDALContext.AddTenderEquipmentDetails(tenderequipmentViewModel, ref message);    
            }

            public bool EditTenderEquipmentDetails(TenderEquipmentViewModel tenderEquipmentViewModel, ref string message)
            {
                return fortyPointChecklistDALContext.EditTenderEquipmentDetails(tenderEquipmentViewModel, ref message);    
            }

            public TenderEquipmentViewModel GetTenderEquipmentDetails(int equipmentId)
            {
                return fortyPointChecklistDALContext.GetTenderEquipmentDetails(equipmentId);    
            }

            public bool DeleteTenderEquipmentDetails(int equipmentId, ref string message)
            {
                return fortyPointChecklistDALContext.DeleteTenderEquipmentDetails(equipmentId, ref message);
            }

        #endregion Tender Equipment    

        #region Forty Point Check List

            public Array ListFortyPointCheckListDetails(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int adminNdCode)
            {
                return fortyPointChecklistDALContext.ListFortyPointCheckListDetails(page, rows, sidx, sord, out totalRecords, stateCode, adminNdCode);
            }

            public Array ListConstructionLabEquipmentDetails(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int adminCode)
            {
                return fortyPointChecklistDALContext.ListConstructionLabEquipmentDetails(page, rows, sidx, sord, out totalRecords, stateCode, adminCode);
            }

            public bool AddEditFortyPointCheckList(FortyPointCheckListViewModel fortyPointCheckListViewModel, ref string message)
            {
                return fortyPointChecklistDALContext.AddEditFortyPointCheckList(fortyPointCheckListViewModel, ref message);    
            }

            public bool DeleteFortyPointCheckListDetails(int checkListPointId, ref string message)
            {
                return fortyPointChecklistDALContext.DeleteFortyPointCheckListDetails(checkListPointId, ref message);
            }

        #endregion Forty Point Check List

    }
    public interface IFortyPointChecklistBAL
    {

        #region Employment Information Details

            Array EmploymentInformationDetailsBAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int adminCode);
            bool AddEmploymentDetails(EmploymentInformationViewModel employmentViewModel, ref string message);
            bool EditEmploymentDetails(EmploymentInformationViewModel employmentViewModel, ref string message);
            EmploymentInformationViewModel GetEmploymentInformationDetails(int employmentId);
            bool DeleteEmploymentInformationDetails(int employmentId, ref string message);

        #endregion Employment Information Details

        #region Tender Cost Information

            Array TenderCostInformationDetailsDAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int adminCode);
            bool AddTenderCostInformationDetails(TenderCostInformationViewModel tenderCostInformationViewModel, ref string message);
            bool EditCostInformationDetails(TenderCostInformationViewModel tenderCostInformationViewModel, ref string message);
            TenderCostInformationViewModel GetTenderCostInformationDetails(int priceId);
            bool DeleteTenderCostInformationDetails(int priceId, ref string message);

        #endregion Tender Cost Information

        #region Tender Equipment

            Array ListTenderEquipmentDetails(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int adminCode);
            bool AddTenderEquipmentDetails(TenderEquipmentViewModel tenderequipmentViewModel, ref string message);
            bool EditTenderEquipmentDetails(TenderEquipmentViewModel tenderEquipmentViewModel, ref string message);
            TenderEquipmentViewModel GetTenderEquipmentDetails(int equipmentId);
            bool DeleteTenderEquipmentDetails(int equipmentId, ref string message);

        #endregion Tender Equipment    

        #region Forty Point Check List

            Array ListFortyPointCheckListDetails(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int adminNdCode);
            Array ListConstructionLabEquipmentDetails(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int adminCode);
            bool AddEditFortyPointCheckList(FortyPointCheckListViewModel fortyPointCheckListViewModel, ref string message);
            bool DeleteFortyPointCheckListDetails(int checkListPointId, ref string message);

        #endregion Forty Point Check List

    }
}
