using PMGSY.Models;
using PMGSY.Models.Menu.ChartOfAccounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.DAL.ChartOfAccount
{
    public class ChartOfAccountDAL : IChartOfAccountDAL
    {

         private PMGSYEntities dbContext = null;

       public ChartOfAccountDAL()
        { 
          dbContext = new PMGSYEntities();
        }

         public List<ChartOfAccountsModel> GetChartOfAccountByFundType(String fundType)
         {

             List<ChartOfAccountsModel> lstLevel = new List<ChartOfAccountsModel>();
             List<sp_get_chart_of_account_Result> List = null;
             try
             {

                 List = dbContext.sp_get_chart_of_account(fundType).ToList<sp_get_chart_of_account_Result>();



                 foreach (sp_get_chart_of_account_Result item in List)
                 {
                     ChartOfAccountsModel objListDTO = new ChartOfAccountsModel();
                     objListDTO.CREDIT_DEBIT =item.CREDIT_DEBIT;
                     objListDTO.HEAD_CODE = item.HEAD_CODE;
                     objListDTO.HEAD_CODE_REF = item.HEAD_CODE_REF;
                     objListDTO.HEAD_ID = item.HEAD_ID;
                     objListDTO.HEAD_NAME = item.HEAD_NAME;
                     objListDTO.OP_LVL_ID = item.OP_LVL_ID;
                     objListDTO.PARENT_HEAD_ID = item.PARENT_HEAD_ID;
                     objListDTO.PARENT_HEAD_Name = item.parent_name;
                     objListDTO.EntryToBeMadeBy = item.Level_Desc;


                     lstLevel.Add(objListDTO);
                 }

                 return lstLevel;
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



    }

    public interface IChartOfAccountDAL
    {
        List<ChartOfAccountsModel> GetChartOfAccountByFundType(String fundType);
    }


}