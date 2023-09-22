using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.DAL.IAPReports;
namespace PMGSY.BAL.IAPReports
{
    public class IAPReportsBAL : IIAPReportsBAL
    {
        IIAPReportsDAL iAPReportsDAL = new IAPReportsDAL();
        public Array IAPDistrictHabitationDetailsBAL(int statecode, int month, int year, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return iAPReportsDAL.IAPDistrictHabitationDetailsDAL(statecode,month,year,page, rows, sidx, sord, out totalRecords);
        }

        public Array IAPDistrictPhysicalProgressDetailsBAL(int statecode, int month, int year, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return iAPReportsDAL.IAPDistrictPhysicalProgressDetailsDAL(statecode, month, year, page, rows, sidx, sord, out totalRecords);
        }
        public Array IAPDistrictFinancialProgressDetailsBAL(int statecode, int month, int year, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return iAPReportsDAL.IAPDistrictFinancialProgressDetailsDAL(statecode, month, year, page, rows, sidx, sord, out totalRecords);
        }

        public Array IAPDistrictExpenditureDetailsBAL(int statecode, int year, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            return iAPReportsDAL.IAPDistrictExpenditureDetailsDAL(statecode,year, page, rows, sidx, sord, out totalRecords);
        }
   
    }
    public interface IIAPReportsBAL
    {

        Array IAPDistrictHabitationDetailsBAL(int statecode, int month, int year, int page, int rows, string sidx, string sord, out int totalRecords);
        Array IAPDistrictPhysicalProgressDetailsBAL(int statecode, int month, int year, int page, int rows, string sidx, string sord, out int totalRecords);
        Array IAPDistrictFinancialProgressDetailsBAL(int statecode, int month, int year, int page, int rows, string sidx, string sord, out int totalRecords);
        Array IAPDistrictExpenditureDetailsBAL(int statecode, int year, int page, int rows, string sidx, string sord, out int totalRecords);

    }
}