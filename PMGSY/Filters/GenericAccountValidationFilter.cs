using PMGSY.Common;
using PMGSY.Extensions;
using PMGSY.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PMGSY.Controllers
{
    //public enum AccountValidation
    //{
    //    BankDetails=1,
    //    ChequeBookDetails,
    //    OpeningBalanceDetails

    //}

    public class GenericAccountValidationFilter : ActionFilterAttribute
    {

        public string[] InputParameter { get; set; }

       
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext ctx = HttpContext.Current;
            CommonFunctions common = new CommonFunctions ();
            try
            {
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "GenericAccountValidation.OnActionExecuting Method test");
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }
                //check accounting related validation
                AccountValidationModel validationModel = new AccountValidationModel();
                
                 validationModel =common.GenericAccountingValidation(PMGSYSession.Current.FundType, PMGSYSession.Current.LevelId, PMGSYSession.Current.AdminNdCode, InputParameter);

                 if (!validationModel.OpeningBalanceFinalized || !validationModel.ChequeBookDetailsEntered || !validationModel.BankDetailsEntered|| !validationModel.AuthSign || !validationModel.SrrdaOBEntered)     //if false
                 {  
                     string parameters = validationModel.BankDetailsEntered + "$" + validationModel.ChequeBookDetailsEntered + "$" + validationModel.OpeningBalanceFinalized +"$"+ validationModel.AuthSign + "$" +validationModel.SrrdaOBEntered;
                     filterContext.HttpContext.Response.Redirect("/Accounts/GenericAccountValidation/" + parameters, false);
                 }
                 else {

                    // string parameters = !validationModel.BankDetailsEntered + "$" + !validationModel.ChequeBookDetailsEntered + "$" + !validationModel.OpeningBalanceFinalized;
                     //filterContext.HttpContext.Response.Redirect("/Accounts/GenericAccountValidation/" + parameters, false);
                  
                 }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GenericAccountValidationFilter.OnActionExecuting()");
                ctx.Response.Redirect("/Login/");
            }
        }
    }
}