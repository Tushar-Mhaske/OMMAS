using PMGSY.Areas.Accountant.Models;
using PMGSY.DAL.Accountant;
//using PMGSY.Models.UserManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//added by Pradip Patil 30-12-2016 
namespace PMGSY.BAL.Accountant
{
    public class AccountantBAL
    {
        public Array UserList(int? page, int? rows, string sidx, string sord, out long totalRecords, string filters,int Role, int State, int PIU)
        {       
            AccountantDAL acDAL = new AccountantDAL();
            try
            {
                return acDAL.UserList(page, rows, sidx, sord, out totalRecords, filters,Role,State,PIU);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}