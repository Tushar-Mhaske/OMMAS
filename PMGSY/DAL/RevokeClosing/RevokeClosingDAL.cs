using PMGSY.Extensions;
using PMGSY.Models;
//using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects;
using PMGSY.Models.RevokeClosing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.DAL.RevokeClosing
{
    public class RevokeClosingDAL : IRevokeClosingDAL
    {
        private PMGSYEntities dbContext = null;
         
        public String GetLastMonthClosed(Int32 AdminNdCode, String FundType, Int16 LevelID)
        {
            try
            {
                dbContext = new PMGSYEntities();
                Int32 month = 0;
                Int32 year = 0;
                if(dbContext.ACC_RPT_MONTHWISE_SUMMARY.Where(m => m.ADMIN_ND_CODE == AdminNdCode && m.LVL_ID == LevelID && m.FUND_TYPE == FundType).Any())
                {
                    year = dbContext.ACC_RPT_MONTHWISE_SUMMARY.Where(m => m.ADMIN_ND_CODE == AdminNdCode && m.LVL_ID == LevelID && m.FUND_TYPE == FundType).Max(m => m.ACC_YEAR);
                }
                if (year != 0)
                {
                    month = dbContext.ACC_RPT_MONTHWISE_SUMMARY.Where(m => m.ADMIN_ND_CODE == AdminNdCode && m.LVL_ID == LevelID && m.FUND_TYPE == FundType && m.ACC_YEAR == year).Max(m => m.ACC_MONTH);
                }
                if (month == 0 && year == 0)
                {
                    return "";
                }
                else
                {
                    return dbContext.MASTER_MONTH.Where(m=>m.MAST_MONTH_CODE == month).Select(m=>m.MAST_MONTH_FULL_NAME).FirstOrDefault()+" "+year;
                }
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw Ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public String GetRevokeStatus(RevokeClosingModel rcModel)
        {
            try
            {
                dbContext = new PMGSYEntities();
                Int32 ? month = 0;
                Int32 ? year = 0;
               
                 year = dbContext.ACC_RPT_MONTHWISE_SUMMARY.Where(m => m.ADMIN_ND_CODE == rcModel.AdminNdCode && m.LVL_ID == rcModel.LevelID && m.FUND_TYPE == rcModel.FundType).Max(m => (Int32?)m.ACC_YEAR);

                 if (year == 0 || !year.HasValue)
                 {
                     return "Months not closed to use this functionality";
                 }
                 

                if (year != 0 && year.HasValue)
                {
                   
                    month = dbContext.ACC_RPT_MONTHWISE_SUMMARY.Where(m => m.ADMIN_ND_CODE == rcModel.AdminNdCode && m.LVL_ID == rcModel.LevelID && m.FUND_TYPE == rcModel.FundType && m.ACC_YEAR == year).Max(m => m.ACC_MONTH);
                }

                if (rcModel.LevelID != 4)
                {
                    Int32? adminNdCode = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == rcModel.AdminNdCode).Select(m => m.MAST_PARENT_ND_CODE).FirstOrDefault();

                    if (dbContext.ACC_RPT_MONTHWISE_SUMMARY.Where(m => m.ADMIN_ND_CODE == adminNdCode && m.LVL_ID == 4 && m.FUND_TYPE == rcModel.FundType && m.ACC_MONTH == month && m.ACC_YEAR == year).Any())
                    {
                        return "Please Revoke Monthly Closing till " + dbContext.MASTER_MONTH.Where(m => m.MAST_MONTH_CODE == month).Select(m => m.MAST_MONTH_FULL_NAME).FirstOrDefault() + " " + year + " at State Level";
                    }
                }               

                if (rcModel.Month == month && rcModel.Year == year)
                {
                    return "";
                }
                else
                {
                    return "Start Revoking from " + dbContext.MASTER_MONTH.Where(m => m.MAST_MONTH_CODE == month).Select(m => m.MAST_MONTH_FULL_NAME).FirstOrDefault() + " " + year;
                }      
                
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw Ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public String RevokeClosing(RevokeClosingModel rcModel)
        {
            try
            {
                using (dbContext = new PMGSYEntities())
                {
                    string result = string.Empty;
                    if (rcModel.OwnDPIUFlag.Equals("O"))
                    {
                        result = dbContext.USP_ACC_REVOKE_MONTHLY_CLOSING_DETAILS(rcModel.OwnDPIUFlag, rcModel.SRRDA_CODE, PMGSYSession.Current.FundType, rcModel.durationFlag, rcModel.StartMonth, rcModel.StartYear, rcModel.ToMonth, rcModel.ToYear, PMGSYSession.Current.UserId, HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"],rcModel.Remark).FirstOrDefault();
                    }
                    else
                    {
                        result = dbContext.USP_ACC_REVOKE_MONTHLY_CLOSING_DETAILS(rcModel.OwnDPIUFlag, rcModel.DPIU_CODE, PMGSYSession.Current.FundType, rcModel.durationFlag, rcModel.StartMonth, rcModel.StartYear, rcModel.ToMonth, rcModel.ToYear, PMGSYSession.Current.UserId, HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"],rcModel.Remark).FirstOrDefault();
                    }
                    
                    if (result != null)
                    {
                        result = result.Equals("1") ? "Success" : result;
                        return result;
                    }
                    else
                    {
                        return "Success";
                    }
                }
                
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                return "Error occured during account revoking";
                throw Ex;            
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        #region FinalizeBalanceSheet

        public bool FinalizeBalanceSheet(FinalizeBalanceSheetModel model,ref string message)
        {
            dbContext = new PMGSYEntities();
            PMGSYEntities dbContext1 = new PMGSYEntities();
            PMGSY.Common.CommonFunctions common=new PMGSY.Common.CommonFunctions();
            try
            {   

                if(dbContext.ACC_RPT_MONTHWISE_SUMMARY_REVOKE_FINALIZE.Where(m=>m.ADMIN_ND_CODE==PMGSYSession.Current.AdminNdCode && m.ACC_YEAR==(model.Year+1) && m.FUND_TYPE==PMGSYSession.Current.FundType && m.FLAG_RF=="F").Any())
                {
                    message = "Balance Sheet is Already FInalize.";
                    return false;
                }

                int? OBYear = GetOpeningBalanceYear();

                if (OBYear == model.Year)
                {

                    //validation for Audit date 
                    //int result = DateTime.Compare(issuedate, expireddate);
                    //if (result < 0)
                    //    Console.WriteLine("issue date is less than expired date");
                    //else if (result == 0)
                    //    Console.WriteLine("Both dates are same");
                    //else if (result > 0)
                    //    Console.WriteLine("issue date is greater than expired date");
                    if((DateTime.Compare( Convert.ToDateTime("31/03/"+(model.Year).ToString()),common.GetStringToDateTime(model.AuditDate)))>=0)
                    {
                        message = "Balance sheet cannot be finalized,Audit date must be in selected financial year only.";
                        return false;
                    }    

                    if (dbContext1.USP_ACC_FINALIZE_ACCOUNT(PMGSYSession.Current.AdminNdCode, model.Year, PMGSYSession.Current.FundType, PMGSYSession.Current.UserId, HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], common.GetStringToDateTime(model.AuditDate)).FirstOrDefault() == 1)
                    {
                        return true;
                    }
                    else {
                        message = "Balance sheet cannot be finalized , SRRDA has not closed the month.";
                        return false;
                    }
                }
                else {
                    //validate prev year 
                    if (!(dbContext1.ACC_RPT_MONTHWISE_SUMMARY_REVOKE_FINALIZE.Where(m => m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && m.ACC_YEAR == (model.Year) && m.FUND_TYPE == PMGSYSession.Current.FundType && m.FLAG_RF == "F").Any()))
                    {
                        message = "Please Finalize Previous Year Balance Sheet.";
                        return false;
                    }

                    //validation for Audit date 
                    //if (common.GetStringToDateTime(model.AuditDate) <= Convert.ToDateTime("31/03/" + model.Year.ToString()))
                    if ((DateTime.Compare(Convert.ToDateTime("31/03/" + (model.Year).ToString()) ,common.GetStringToDateTime(model.AuditDate))) >= 0)
                    {
                        message = "Balance sheet cannot be finalized,Audit date must be in selected financial year only.";
                        return false;
                    }

                    if (dbContext1.USP_ACC_FINALIZE_ACCOUNT(PMGSYSession.Current.AdminNdCode, model.Year, PMGSYSession.Current.FundType, PMGSYSession.Current.UserId, HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], common.GetStringToDateTime(model.AuditDate)).FirstOrDefault() == 1)
                    {
                        return true;
                    }
                    else
                    {
                        message = "Balance sheet cannot be finalized , SRRDA has not closed the month.";
                        return false;
                    }
                }
              //  return true;
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                message = "Balance Sheet is Not Finalize.";
                return false;
            }
            finally {
                if(dbContext!=null)
                    dbContext1.Dispose();            
            }
        }

        public int? GetOpeningBalanceYear()
        {
            dbContext = new PMGSYEntities();
            try
            {
                return dbContext.ACC_BILL_MASTER.Where(m => m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && m.BILL_TYPE == "O" && m.BILL_FINALIZED == "Y" && m.FUND_TYPE == PMGSYSession.Current.FundType && m.LVL_ID == 4).Select(s => s.BILL_YEAR).FirstOrDefault();
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                return null;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }        
        }


        public String GetFinalizedBalanceSheetDetails()
        {
            dbContext = new PMGSYEntities();            
            try
            {
                if (dbContext.ACC_RPT_MONTHWISE_SUMMARY_REVOKE_FINALIZE.Where(m => m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && m.FUND_TYPE == PMGSYSession.Current.FundType && m.FLAG_RF == "F").Any())
                {
                    return dbContext.ACC_RPT_MONTHWISE_SUMMARY_REVOKE_FINALIZE.Where(m => m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && m.FUND_TYPE == PMGSYSession.Current.FundType && m.FLAG_RF == "F").Max(s => s.ACC_YEAR).ToString();
                }
                else {
                    return String.Empty;
                }               
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                return String.Empty;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }    
        }             

       public SelectList PopulateFinancialYear(int OByear)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<MASTER_YEAR> lstYears = dbContext.MASTER_YEAR.Where(y => y.MAST_YEAR_CODE >= OByear && y.MAST_YEAR_CODE <= System.DateTime.Now.Year).OrderByDescending(y => y.MAST_YEAR_CODE).ToList<MASTER_YEAR>();


                if (dbContext.ACC_RPT_MONTHWISE_SUMMARY_REVOKE_FINALIZE.Where(m => m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && m.FUND_TYPE == PMGSYSession.Current.FundType && m.FLAG_RF == "F").Any())
                {
                    List<int> lstYear = dbContext.ACC_RPT_MONTHWISE_SUMMARY_REVOKE_FINALIZE.Where(m => m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && m.FUND_TYPE == PMGSYSession.Current.FundType && m.FLAG_RF == "F").Select(s => s.ACC_YEAR - 1).ToList<int>();

                    //Get FinYEars
                    //List<MASTER_YEAR> FinalizedYears = (from year in dbContext.MASTER_YEAR
                    //                                    join finYear in dbContext.ACC_RPT_MONTHWISE_SUMMARY_REVOKE_FINALIZE
                    //                                    on year.MAST_YEAR_CODE equals finYear.ACC_YEAR
                    //                                    where
                    //                                       finYear.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode &&
                    //                                       finYear.FUND_TYPE == PMGSYSession.Current.FundType &&
                    //                                       finYear.FLAG_RF == "F"
                    //                                    select new {
                    //                                        MAST_YEAR_CODE = year.MAST_YEAR_CODE - 1,
                    //                                        MAST_YEAR_TEXT = (year.MAST_YEAR_CODE) + "-" + (year.MAST_YEAR_CODE - 1)
                    //                                    }
                    //                   ).ToList<MASTER_YEAR>();

                    foreach (var year in lstYear)
                    {
                        MASTER_YEAR yearModel = dbContext.MASTER_YEAR.Where(m => m.MAST_YEAR_CODE == year).FirstOrDefault();
                        lstYears.Remove(yearModel);                      
                    }   

                  //lstYears.RemoveAll
                }

                 


                lstYears.Insert(0, new MASTER_YEAR() { MAST_YEAR_CODE = 0, MAST_YEAR_TEXT = "Select Year " });

                return new SelectList(lstYears, "MAST_YEAR_CODE", "MAST_YEAR_TEXT", DateTime.Now.Year);
            }
            catch
            {
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        #endregion FinalizeBalanceSheet


        #region DeFinalizeBalanceSheet

       public List<SelectListItem> GetDefinalizeBalSheetYear(int adminNdCode,String FundType)
       {
           dbContext=new PMGSYEntities();
           try
           {
               List<SelectListItem> lstYears = new List<SelectListItem>();
               if (dbContext.ACC_RPT_MONTHWISE_SUMMARY_REVOKE_FINALIZE.Where(m => m.ADMIN_ND_CODE == adminNdCode && m.FUND_TYPE == FundType && m.FLAG_RF == "F").Any())
               {
                   int Year = dbContext.ACC_RPT_MONTHWISE_SUMMARY_REVOKE_FINALIZE.Where(m => m.ADMIN_ND_CODE == adminNdCode && m.FUND_TYPE == FundType && m.FLAG_RF == "F").Max(s => s.ACC_YEAR);
                   String YearText = (Year - 1) + "-" + (Year);
                   lstYears.Add(new SelectListItem { Text = YearText, Value = Year.ToString() });
               }
               lstYears.Insert(0, new SelectListItem { Text = "Select Year", Value = "0" });

               return lstYears;
           }
           catch (Exception)
           {
               return null;
           }
           finally {
               dbContext.Dispose();
           }
       }

       public bool DefinalizeBalanceSheet(DefinalizeBalanceSheetModel model)
       {

           dbContext = new PMGSYEntities();
           try
           {
               if (dbContext.ACC_RPT_MONTHWISE_SUMMARY_REVOKE_FINALIZE.Where(m => m.ADMIN_ND_CODE == model.AdminNdCode && m.FUND_TYPE == model.FundType && m.FLAG_RF == "F" && m.ACC_YEAR == model.Year).Any())
               {

                   ACC_RPT_MONTHWISE_SUMMARY_REVOKE_FINALIZE RevokeModel = dbContext.ACC_RPT_MONTHWISE_SUMMARY_REVOKE_FINALIZE.Where(m => m.ADMIN_ND_CODE == model.AdminNdCode && m.FUND_TYPE == model.FundType && m.FLAG_RF == "F" && m.ACC_YEAR == model.Year).FirstOrDefault();

                   dbContext.ACC_RPT_MONTHWISE_SUMMARY_REVOKE_FINALIZE.Remove(RevokeModel);
                   dbContext.SaveChanges();

                   return true;
               }
               else {
                   return false;
               }
           }
           catch (Exception)
           {
               return false;                     
           }
           finally {
               dbContext.Dispose();
           }

       }

        #endregion DeFinalizeBalanceSheet

    }

    public interface IRevokeClosingDAL
    {
        String GetLastMonthClosed(Int32 AdminNdCode, String FundType, Int16 LevelID);
        String GetRevokeStatus(RevokeClosingModel rcModel);
        String RevokeClosing(RevokeClosingModel rcModel);


        #region FinalizeBalanceSheet                               
            bool FinalizeBalanceSheet(FinalizeBalanceSheetModel model,ref string message);
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