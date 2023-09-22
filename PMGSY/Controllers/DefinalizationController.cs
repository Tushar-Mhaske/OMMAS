using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Models.Common;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using PMGSY.Common;
using PMGSY.BAL.Definalization;
using PMGSY.Extensions;
using PMGSY.Models.Definalization;


namespace PMGSY.Controllers
{
     
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class DefinalizationController : Controller
    {
        CommonFunctions common = null;
        IDefinalizationBAL objFinalizeBAL = null;


        /// <summary>
        /// action to get the definalization page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult GetDefinalizationView()
        {
            TransactionParams objparams = new TransactionParams();
            common = new CommonFunctions();
            try
            {

                DefinalizationModel model = new DefinalizationModel();

                //commented by koustubh Nakate on 23/08/2013 for take fund type from session
                List<SelectListItem> GeneralList = new List<SelectListItem>();
                //GeneralList.Add(new SelectListItem { Text = "--Select--", Value = "0", Selected = true });
                //GeneralList.Add(new SelectListItem { Text = "Programme Fund", Value = "P" });
                //GeneralList.Add(new SelectListItem { Text = "Administration Fund", Value = "A"});
                //GeneralList.Add(new SelectListItem { Text = "Maintenance Fund", Value = "M" });

                //model.FUND_TYPE_LIST = GeneralList;

                GeneralList = null;

                GeneralList =new List<SelectListItem>();

                GeneralList.Add(new SelectListItem { Text = "--Select--", Value = "0", Selected = true });
                GeneralList.Add(new SelectListItem { Text = "Opening Balance", Value = "O"});
                GeneralList.Add(new SelectListItem { Text = "Receipt ", Value = "R" });
                GeneralList.Add(new SelectListItem { Text = "Payment", Value = "P"});
                GeneralList.Add(new SelectListItem { Text = "Transfer Entry Order", Value = "J" });

                model.VOUCHER_TYPE_LIST = GeneralList;

                List<SelectListItem> monthList = common.PopulateMonths(DateTime.Now.Month);
                model.MONTH_LIST = monthList;

                List<SelectListItem> yearList = common.PopulateYears(DateTime.Now.Year);
                model.YEAR_LIST = yearList;

               
                objparams.LVL_ID = PMGSYSession.Current.LevelId;
                objparams.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                objparams.DISTRICT_CODE = PMGSYSession.Current.DistrictCode;


                //Added by abhishek kamble 5Jan2015 start 
                /* Role 17 for Admin Added by rohit borse on 06-06-2022*/
                if (PMGSYSession.Current.RoleCode == 21 || PMGSYSession.Current.RoleCode == 66 || PMGSYSession.Current.RoleCode == 17)//AccMorduser //if condition
                {
                    CommonFunctions commomFuncObj = new CommonFunctions();
                    //populate SRRDA
                    List<SelectListItem> lstSRRDA = new List<SelectListItem>();
                    lstSRRDA = commomFuncObj.PopulateNodalAgencies();
                    // ViewBag.SRRDA = lstSRRDA;
                    model.SRRDA_LIST = lstSRRDA;

                    //Populate DPIU
                    List<SelectListItem> lstDPIU = new List<SelectListItem>();
                    lstDPIU.Insert(0, new SelectListItem { Text = "Select DPIU", Value = "0" });
                    model.DPIU_LIST = lstDPIU;
                }
                else 
                {
                    List<SelectListItem> DPIUList = common.PopulateDPIU(objparams);
                    model.DPIU_LIST = DPIUList;
                }
                //Added by abhishek kamble 5Jan2015 end 

                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                string errorMsg = string.Format("Errors: {0}", ex.Message);
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return Json(string.Empty);
            }
          
        }
        //Populate DPIU
        [Audit]
        public JsonResult PopulateDPIU(string id)
        {
            try
            {
                TransactionParams objParam = new TransactionParams();
                CommonFunctions objCommonFunction = new CommonFunctions();

                int AdminNdCode = Convert.ToInt32(id);
                objParam.ADMIN_ND_CODE = AdminNdCode;


                if (AdminNdCode == 0)
                {
                    List<SelectListItem> lstDpiu = new List<SelectListItem>();
                    lstDpiu.Insert(0, new SelectListItem { Text = "Select DPIU", Value = "0" });
                    return Json(lstDpiu, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    List<SelectListItem> lstDPIU = objCommonFunction.PopulateDPIU(objParam);
                    lstDPIU.RemoveAt(0);
                    lstDPIU.Insert(0, new SelectListItem { Text = "Select DPIU", Value = "0" });

                    return Json(new SelectList(lstDPIU, "Value", "Text"), JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(false);
            }
        }


       /// <summary>
       /// action to list the voucher list to definalize or delete
       /// </summary>
       /// <param name="page"></param>
       /// <param name="rows"></param>
       /// <param name="sidx"></param>
       /// <param name="sord"></param>
       /// <returns> list of the voucher</returns>
        [Audit]
        public JsonResult GetVoucherList(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                //Adde By Abhishek kamble 30-Apr-2014 start  
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 30-Apr-2014 end
                VoucherFilterModel objFilter = new VoucherFilterModel();

                objFinalizeBAL = new DefinalizationBAL();

                if (Request.Params["voucherType"] != "R" && Request.Params["voucherType"] != "P" && Request.Params["voucherType"] != "O" && Request.Params["voucherType"] != "J")
                {
                    throw new Exception("Invalid Voucher Type");
                }
                //commented by Koustubh Nakate on 23/08/2013 for get details for fund from session 
                //else if (Request.Params["fundType"] != "P" && Request.Params["fundType"] != "A" && Request.Params["fundType"]!="M")
                //{
                //    throw new Exception("Invalid Fund Type");
                //}
                else if (Convert.ToInt16(Request.Params["level"]) != 4 && Convert.ToInt16(Request.Params["level"]) != 5)
                {
                  throw new Exception("Invalid level");
                }
                else if(Convert.ToInt16(Request.Params["level"]) == 5 && Convert.ToInt32(Request.Params["DPIU"])==0 )
                {
                   throw new Exception("Invalid DPIU");
                }

                long totalRecords;
                objFilter.Month = Convert.ToInt16(Request.Params["months"]);
                objFilter.Year = Convert.ToInt16(Request.Params["year"]);

               
                objFilter.page = Convert.ToInt32(page) - 1;
                objFilter.rows = Convert.ToInt32(rows);
                objFilter.sidx = sidx.ToString();
                objFilter.sord = sord.ToString();

                objFilter.Bill_type = Request.Params["voucherType"];
                objFilter.AdminNdCode =Convert.ToInt32(Request.Params["DPIU"]);
                
                //changes by Koustubh Nakate on 23/08/2013 for get details for fund from session 
                //objFilter.FundType = Request.Params["fundType"];
                
                // for Admin added by rohit  borse on 06-06-2022
                if (PMGSYSession.Current.RoleCode == 17)
                {
                    objFilter.FundType = Request.Params["FundType"];
                }
                else
                {
                    objFilter.FundType = PMGSYSession.Current.FundType;
                }

                objFilter.LevelId =Convert.ToInt16( Request.Params["level"]);

                if (PMGSYSession.Current.RoleCode == 21)//Accmord user
                {
                    objFilter.SRRDAAdminNdCode = Convert.ToInt32(Request.Params["SRRDA"]);                    
                }
                else 
                {
                    objFilter.SRRDAAdminNdCode = PMGSYSession.Current.AdminNdCode;
                }

                var jsonData = new
                {
                    rows = objFinalizeBAL.ListVoucherListtDetails(objFilter, out totalRecords),
                    total = totalRecords <= objFilter.rows ? 1 : totalRecords / objFilter.rows + 1,
                    page = page,
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                string errorMsg = string.Format("Errors: {0}", ex.Message);
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return Json(string.Empty);
            }
        
        }


        /// <summary>
        /// action to definalize the voucher details 
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult DefinalizeVoucher(String parameter, String hash, String key)
        {
             try
            {
                    Int64 bill_Id = 0;

                    objFinalizeBAL = new DefinalizationBAL();

                    if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                    {
                        String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                        if (urlParams.Length >= 1)
                        {

                            bill_Id = Convert.ToInt64(urlParams[0]);

                        }
                    }
                    else
                    {
                        throw new Exception("Error while getting voucher details..");
                    }

                    string result = objFinalizeBAL.DefinalizeVoucher(bill_Id);

                    return Json(new
                    {
                        result = result
                    });

            }
             catch (Exception ex)
             {
                 Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                 string errorMsg = string.Format("Errors: {0}", ex.Message);
                 Response.TrySkipIisCustomErrors = true;
                 Response.StatusCode = 500;
                 Response.Write(errorMsg);
                 return Json(string.Empty);
             }

        }

         /// <summary>
         /// Action to check if asset payment is done against the voucher to be finalized
         /// </summary>
         /// <param name="parameter"></param>
         /// <param name="hash"></param>
         /// <param name="key"></param>
         /// <returns></returns>
        [Audit]
        public String CheckIfAssetPayment(String parameter, String hash, String key)
        {
            try
            {
                Int64 bill_Id = 0;
                string result = string.Empty;
                objFinalizeBAL = new DefinalizationBAL();

                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {

                        bill_Id = Convert.ToInt64(urlParams[0]);
                        result = objFinalizeBAL.CheckIfAssetPayment(bill_Id);
                        return result;
                    }
                    else {
                        throw new Exception("Error while getting voucher details..");
                    }
                }
                else
                {
                    throw new Exception("Error while gettingvoucher details..");
                }
          
          }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                string errorMsg = string.Format("Errors: {0}", ex.Message);
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return (string.Empty);
            }

        }


        /// <summary>
        /// action to delete the voucher details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult DeleteVoucher(String parameter, String hash, String key)
        { try
            {
                Int64 bill_Id = 0;
               
                objFinalizeBAL = new DefinalizationBAL();

                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {

                        bill_Id = Convert.ToInt64(urlParams[0]);

                    }
                }
                else
                {
                    throw new Exception("Error while getting master payment details..");
                }

                string result = objFinalizeBAL.DeleteVoucher(bill_Id);

                
                return Json(new {
                    result = result
                });

             }
        catch (Exception ex)
        {
            //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
            ErrorLog.LogError(ex, "Definalization.DeleteVoucher()");
            string errorMsg = string.Format("Errors: {0}", ex.Message);
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return Json(string.Empty);
            }
        }


        /// <summary>
        /// function to get voucher transaction details
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult GetTransactionDetails(int? page, int? rows, string sidx, string sord, String parameter, String hash, String key)
        {

            try
            {
                //Adde By Abhishek kamble 30-Apr-2014 start  
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 30-Apr-2014 end
                long totalRecords;
                Int64 bill_Id = 0;
               
                VoucherFilterModel objFilter = new VoucherFilterModel();
                objFinalizeBAL = new DefinalizationBAL();

                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        bill_Id = Convert.ToInt64(urlParams[0]);
                    }
                }
                else
                {
                    throw new Exception("Error while getting tranasction details..");
                }


                objFilter.page = Convert.ToInt32(page) - 1;
                objFilter.rows = Convert.ToInt32(rows);
                objFilter.sidx = sidx.ToString();
                objFilter.sord = sord.ToString();
                objFilter.BillId = bill_Id;
                var jsonData = new
                {
                    rows = objFinalizeBAL.ListVoucherTransactionDetails(objFilter, out totalRecords),
                    total = totalRecords <= objFilter.rows ? 1 : totalRecords / objFilter.rows + 1,
                    page = objFilter.page,
                    records = totalRecords
                };
                return Json(jsonData);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                string errorMsg = string.Format("Errors: {0}", ex.Message);
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return Json(string.Empty);
            }

        }


    }
}
