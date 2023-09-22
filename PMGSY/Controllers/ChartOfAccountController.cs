using PMGSY.BAL.ChartOfAccount;
using PMGSY.Models;
using PMGSY.Models.Menu.ChartOfAccounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Extensions;

namespace PMGSY.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class ChartOfAccountController : Controller
    {

        private PMGSYEntities dbContext = null;

        public ChartOfAccountController()
        {
            dbContext = new PMGSYEntities();
        }

               [Audit]
        public ActionResult GetChartOfAccounts(string id)
        {
            if (!String.IsNullOrEmpty(id))
            {
                Session.Add("fundtype", id);
            }
            else {
                Session.Add("fundtype", "P");
            }
            
            return View();
        }



               [Audit]
        public JsonResult GetChartOfAccountsList(int? page, int? rows, string sidx, string sord)
        {
            //Adde By Abhishek kamble 30-Apr-2014 start  
            using (PMGSY.Common.CommonFunctions commonFunction = new PMGSY.Common.CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new PMGSY.Common.GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 30-Apr-2014 end
            string fundType = string.Empty;

            fundType = Request.Url.AbsolutePath.Split('/')[3].ToString();
            IChartOfAccountBAL objChartOfAccountsBAL = new ChartOfAccountBAL();

         //add in session
            
            //List<ACC_HEAD_MASTER> lstChartOfAccounts = new List<ACC_HEAD_MASTER>();
        
            
            // lstChartOfAccounts =( from item in dbContext.ACC_HEAD_MASTER
            //                       join a in dbContext.ACC_HEAD_MASTER on item.HEAD_ID equals a.PARENT_HEAD_ID
            //                       where item.IS_OPERATIONAL == true &&
            //                        item.FUND_TYPE == fundType
            //               orderby item.PARENT_HEAD_ID
            //                         select new {item.}).ToList<ACC_HEAD_MASTER>();

             List<ChartOfAccountsModel> listChartOfAccount = new List<ChartOfAccountsModel>();

             listChartOfAccount = objChartOfAccountsBAL.GetChartOfAccountByFundType(PMGSYSession.Current.FundType);
          
            //listChartOfAccount = (from c in lstChartOfAccounts, d in listChartOfAccount
            //                     select new
            //                            {
            //                                                c.HEAD_CODE_REF,
            //                                                c.HEAD_NAME,
            //                                                c.CREDIT_DEBIT,
            //                                                c.PARENT_HEAD_ID,
            //                                                c.HEAD_CODE, 
            //                                                c.OP_LVL_ID,
            //                                                PARENT_HEAD_Name = (from soh in lstChartOfAccounts
            //                                                where soh.HEAD_ID== c.PARENT_HEAD_ID
            //                                                select soh.HEAD_NAME)
            //                            })


             var data = (listChartOfAccount.Select(c => new
            {
                id = c.HEAD_ID,
                cell = new[]
                        {
                            c.HEAD_CODE_REF == null ? String.Empty:c.HEAD_CODE_REF.ToString(),
                            c.PARENT_HEAD_Name,
                            c.HEAD_NAME.ToString(),
                            c.CREDIT_DEBIT==null? String.Empty: c.CREDIT_DEBIT.ToString(),
                            //c.PARENT_HEAD_ID == null ?  String.Empty:c.PARENT_HEAD_ID.ToString().Trim(),
                            c.HEAD_CODE.Split('.')[0],
                            c.HEAD_CODE.ToString(),
                            c.EntryToBeMadeBy
                           
                            
                         
                                             
                        }
            })).ToArray();


            var jsonData = new
            {
                total = Math.Ceiling(Convert.ToDouble(listChartOfAccount.Count()) / Convert.ToDouble(rows == null ? 1 : rows)),
                page = page == null ? 0 : page,
                records = listChartOfAccount.Count(),
                rows = data
            };
           
         

            return Json(jsonData, JsonRequestBehavior.AllowGet);

           
        }

    }
}
