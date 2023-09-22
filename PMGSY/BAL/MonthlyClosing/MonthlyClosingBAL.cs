using PMGSY.DAL.MonthlyClosing;
using PMGSY.Models;
using PMGSY.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace PMGSY.BAL.MonthlyClosing
{
     
    public class MonthlyClosingBAL  : IMonthlyClosingBAL
    {
        IMonthlyClosingDAL objMonthlyClosingDAL = null;

        /// <summary>
        /// Action method to get closing month and year 
        /// </summary>
        /// <param name="objparams"></param>
        /// <returns></returns>
        public string GetClosedMonthAndYear(TransactionParams objparams)
        {
            try
            {
                objMonthlyClosingDAL = new MonthlyClosingDAL();

                return objMonthlyClosingDAL.GetClosedMonthAndYear(objparams);
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
        
                throw new Exception("Error While getting monthly closing details.");
            
            }
        }

        /// <summary>
        /// BAL function to get the list of the DPIU which have not closed the month and year
        /// </summary>
        /// <param name="objFilter"></param>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array ListDPIUMonthNotClosed(PaymentFilterModel objFilter, short month, short year, out long totalRecords)
        {
            try
            {
                objMonthlyClosingDAL = new MonthlyClosingDAL();
                return objMonthlyClosingDAL.ListDPIUMonthNotClosed(objFilter, month, year,out totalRecords);
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                        throw new Exception("Error while getting DPIU  list...");
            }
        }


       public String CloseMonth(short fromMonth, short fromYear, short toMonth, short toYear, int adminNdCode, String FundType,short level_Id)
        {
            try
            {
                objMonthlyClosingDAL = new MonthlyClosingDAL();
                return objMonthlyClosingDAL.CloseMonth(fromMonth, fromYear, toMonth, toYear, adminNdCode, FundType,level_Id);
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw new Exception("Error while getting DPIU  list...");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objparams"></param>
        public String  GetAccountStartMonthandYear(TransactionParams objparams)
        {
           try
            {
                objMonthlyClosingDAL = new MonthlyClosingDAL();

                return objMonthlyClosingDAL.GetAccountStartMonthandYear(objparams);
            }
           catch (Exception Ex)
           {
               Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw new Exception("Error While getting data entry start month and year.");
            
            }
        }


        public string CheckAllPiuForMonthlyClosing(int adminNdCode, short monthToClose, short yearToClose, string fundType, short levelID)
        {
            try
            {
                objMonthlyClosingDAL = new MonthlyClosingDAL();

                return objMonthlyClosingDAL.CheckAllPiuForMonthlyClosing(adminNdCode,  monthToClose, yearToClose,  fundType, levelID);


            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                       throw new Exception("Error While checking if all DPIU has closed the month or not.");
            
            }
        }

        public List<USP_ACC_VERIFY_PIUS_CHEQUEACK_Result> CheckAllPiuForChequeAck(string FundType, int AdminNdCode, int FromMonth, int FromYear, int ToMonth, int ToYear, String SingleMultipleMonth)
        {
            try
            {
                objMonthlyClosingDAL = new MonthlyClosingDAL();
                return objMonthlyClosingDAL.CheckAllPiuForChequeAck( FundType,  AdminNdCode,  FromMonth,  FromYear,  ToMonth,  ToYear, SingleMultipleMonth);
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                return null;
            }
        }
    }
   
    interface IMonthlyClosingBAL
    {
        string GetClosedMonthAndYear(TransactionParams objparams);
       Array ListDPIUMonthNotClosed(PaymentFilterModel objFilter, short month, short year,out long totalRecords);
        String CloseMonth(short fromMonth, short fromYear, short toMonth, short toYear, int adminNdCode, String FundType,short level_Id);
        String GetAccountStartMonthandYear(TransactionParams objparams);
        string CheckAllPiuForMonthlyClosing(int adminNdCode, short monthToClose, short yearToClose, string fundType, short levelID);
        List<USP_ACC_VERIFY_PIUS_CHEQUEACK_Result> CheckAllPiuForChequeAck(string FundType, int AdminNdCode, int FromMonth, int FromYear, int ToMonth, int ToYear, String SingleMultipleMonth);
    }
}