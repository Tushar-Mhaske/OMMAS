using PMGSY.DAL.ChartOfAccount;
using PMGSY.Models.Menu.ChartOfAccounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.BAL.ChartOfAccount
{
    public class ChartOfAccountBAL : IChartOfAccountBAL
    {
        IChartOfAccountDAL ChartOfAccountDAL = null;

        public List<ChartOfAccountsModel> GetChartOfAccountByFundType(String fundType)
        {
           
            try
            {
                ChartOfAccountDAL = new ChartOfAccountDAL();
                 return ChartOfAccountDAL.GetChartOfAccountByFundType(fundType);
            }

            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);            
                throw ex;
            }
        }



    }

    public interface IChartOfAccountBAL
    {
        List<ChartOfAccountsModel> GetChartOfAccountByFundType(String fundType);
    }
}