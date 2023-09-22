using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.DAL.AccountMaster;
using PMGSY.Models.AccountMaster;
using PMGSY.Models;

namespace PMGSY.BAL.AccountMaster
{
    public class AccountMasterBAL :IAccountMasterBAL
    {
        IAccountMasterDAL objDAL = new AccountMasterDAL();


        #region AccountMaster

        //public List<SelectListItem> PopulateFundType()
        //{
        //    return objDAL.PopulateFundType();
        //}
        //public List<SelectListItem> PopulateHeadCategory()
        //{
        //    return objDAL.PopulateHeadCategory();
        //}
        //public List<SelectListItem> PopulateParentHead()
        //{
        //    return objDAL.PopulateParentHead();
        //}

        //public List<SelectListItem> PopulateOperationalLevel()
        //{
        //    return objDAL.PopulateOperationalLevel();
        //}

        public Array ListMasterHeadDetails(int? page, int? rows, string sidx, String sord, out long totalRecords, string FundType)
        { 
            return objDAL.ListMasterHeadDetails(page,rows,sidx,sord,out totalRecords,FundType);        
        }

        public bool AddMasterHeadDetails(MasterHeadViewModel headViewModel, ref String message)
        {
            return objDAL.AddMasterHeadDetails(headViewModel, ref message);            
        }

        public bool EditMasterHeadDetails(MasterHeadViewModel headViewModel, ref String message)
        {
            return objDAL.EditMasterHeadDetails(headViewModel, ref message);
        }

        public bool DeleteMasterHeadDetails(short HeadId, ref String message)
        {
            return objDAL.DeleteMasterHeadDetails(HeadId, ref message);        
        }

        public MasterHeadViewModel GetHeadDetails(short HeadId)
        {
            return objDAL.GetHeadDetails(HeadId);
        }

        public String GetParentHeadCode(short ParentHeadID)
        {
            return objDAL.GetParentHeadCode(ParentHeadID);        
        }

        #endregion AccountMaster


        #region AccountMasterTransaction

        public Array ListMasterTransactionDetails(int? page, int? rows, string sidx, String sord, out long totalRecords, string FundType, bool IsSearch, short ParentTxn, int Level, string CashCheque, string BillType, Boolean? IsOperational)
        {
            return objDAL.ListMasterTransactionDetails(page, rows, sidx, sord, out totalRecords, FundType, IsSearch, ParentTxn, Level, CashCheque, BillType, IsOperational);        
        }

        public bool AddMasterTransactionDetails(MasterTransactionViewModel txnViewModel, ref String message)
        {
            return objDAL.AddMasterTransactionDetails(txnViewModel, ref message);                    
        }

        public bool EditMasterTransactionDetails(MasterTransactionViewModel txnViewModel, ref String message)
        {
            return objDAL.EditMasterTransactionDetails(txnViewModel, ref message);                    
        }

        public bool DeleteMasterTransactionDetails(short TxnId, ref String message)
        {
            return objDAL.DeleteMasterTransactionDetails(TxnId, ref message);                    
        }

        public MasterTransactionViewModel GetTransactionDetails(short TxnId)
        {
            return objDAL.GetTransactionDetails(TxnId);                    
        }

        public ACC_MASTER_TXN GetParentTransactionDetails(short ParentTxnID)
        {
            return objDAL.GetParentTransactionDetails(ParentTxnID);                            
        }

        #endregion

    }

    public interface IAccountMasterBAL
    {                                             
        #region AccountMaster                     
            //List<SelectListItem> PopulateFundType();
            //List<SelectListItem> PopulateHeadCategory();
            //List<SelectListItem> PopulateParentHead();
            //List<SelectListItem> PopulateOperationalLevel();               
            Array ListMasterHeadDetails(int? page, int? rows, string sidx, String sord, out long totalRecords, string FundType);
            bool AddMasterHeadDetails(MasterHeadViewModel headViewModel, ref String message);
            bool EditMasterHeadDetails(MasterHeadViewModel headViewModel, ref String message);
            bool DeleteMasterHeadDetails(short HeadId, ref String message);
            MasterHeadViewModel GetHeadDetails(short HeadId);
            String GetParentHeadCode(short ParentHeadID);

        #endregion


        #region AccountMasterTransaction

        Array ListMasterTransactionDetails(int? page, int? rows, string sidx, String sord, out long totalRecords, string FundType, bool IsSearch, short ParentTxn, int Level, string CashCheque, string BillType, bool? IsOperational);
        bool AddMasterTransactionDetails(MasterTransactionViewModel txnViewModel, ref String message);
        bool EditMasterTransactionDetails(MasterTransactionViewModel txnViewModel, ref String message);
        bool DeleteMasterTransactionDetails(short TxnId, ref String message);
        MasterTransactionViewModel GetTransactionDetails(short TxnId);
        ACC_MASTER_TXN GetParentTransactionDetails(short ParentTxnID);

        #endregion

    }
}