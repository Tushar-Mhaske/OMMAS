using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.DAL.RevokeClosing;
using PMGSY.Models.RevokeClosing;
using System.Web.Mvc;

namespace PMGSY.BAL.RevokeClosing
{
    public class RevokeClosingBAL : IRevokeClosingBAL
    {
        RevokeClosingDAL objRevokeDAL = new RevokeClosingDAL();
        public String GetLastMonthClosed(Int32 AdminNdCode, String FundType, Int16 LevelID)
        {
            return objRevokeDAL.GetLastMonthClosed(AdminNdCode,FundType, LevelID);
        }

        public String GetRevokeStatus(RevokeClosingModel rcModel)
        {
            return objRevokeDAL.GetRevokeStatus(rcModel);
        }

        public string RevokeClosing(RevokeClosingModel rcModel)
        {
            return objRevokeDAL.RevokeClosing(rcModel);
        }

        #region FinalizeBalanceSheet
        public bool FinalizeBalanceSheet(FinalizeBalanceSheetModel model,ref String message)
        {
            return objRevokeDAL.FinalizeBalanceSheet(model,ref message);        
        }
        public int? GetOpeningBalanceYear()
        {
            return objRevokeDAL.GetOpeningBalanceYear();
        }
        public SelectList PopulateFinancialYear(int OByear)
        {
            return objRevokeDAL.PopulateFinancialYear(OByear);
        }

        public String GetFinalizedBalanceSheetDetails()
        {
            return objRevokeDAL.GetFinalizedBalanceSheetDetails();
        }

        #endregion FinalizeBalanceSheet

        #region DeFinalizeBalanceSheet
        public List<SelectListItem> GetDefinalizeBalSheetYear(int adminNdCode, String FundType)
        {
            return objRevokeDAL.GetDefinalizeBalSheetYear(adminNdCode, FundType);   
        }

        public bool DefinalizeBalanceSheet(DefinalizeBalanceSheetModel model)
        {
            return objRevokeDAL.DefinalizeBalanceSheet(model);
        }
        #endregion DeFinalizeBalanceSheet

    }

    public interface IRevokeClosingBAL
    {
       String GetLastMonthClosed(Int32 AdminNdCode, String FundType, Int16 LevelID);
       String GetRevokeStatus(RevokeClosingModel rcModel);
       String RevokeClosing(RevokeClosingModel rcModel);


       #region FinalizeBalanceSheet
       bool FinalizeBalanceSheet(FinalizeBalanceSheetModel model,ref String message);
       int? GetOpeningBalanceYear();
       SelectList PopulateFinancialYear(int OByear);
       String GetFinalizedBalanceSheetDetails();


       #endregion FinalizeBalanceSheet


        #region DeFinalizeBalanceSheet
        List<SelectListItem> GetDefinalizeBalSheetYear(int adminNdCode, String FundType);
        bool DefinalizeBalanceSheet(DefinalizeBalanceSheetModel model);
        #endregion DeFinalizeBalanceSheet
    }
}