using PMGSY.BAL.Payment;
using PMGSY.Common;
using PMGSY.Extensions;
using PMGSY.Models;
using PMGSY.Models.Common;
using PMGSY.Models.Definalization;
using PMGSY.Models.PaymentModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using System.Web;

namespace PMGSY.DAL.Definalization
{
    public class DefinalizationDAL : IDefinalizationDAL
    {
        PMGSYEntities dbContext = null;

        /// <summary>
        /// DAL function to list the voucher details in which
        /// cancelled /renewed cheques are not listed
        /// acknowledgement master entry is not listed 
        /// </summary>
        /// <param name="objFilter"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array ListVoucherListtDetails(VoucherFilterModel objFilter, out long totalRecords)
        {
            CommonFunctions commomFuncObj = null;

            try
            {
                dbContext = new PMGSYEntities();

                commomFuncObj = new CommonFunctions();

                List<ACC_BILL_MASTER> listBillMaster = new List<ACC_BILL_MASTER>();

                int ndCode = 0;
                //District
                if (objFilter.AdminNdCode != 0)
                {

                    listBillMaster = dbContext.ACC_BILL_MASTER.Where(
                                     c => c.BILL_MONTH == objFilter.Month &&
                                         c.BILL_YEAR == objFilter.Year &&
                                         c.FUND_TYPE == objFilter.FundType &&
                                         c.BILL_TYPE == objFilter.Bill_type &&
                                         (c.BILL_FINALIZED == "Y") &&
                                         // (c.BILL_FINALIZED == "Y" || c.BILL_FINALIZED == "E" )&& //added -02/4/2014 -- While Testing 
                                         // (c.CHQ_EPAY == null ? "" : c.CHQ_EPAY) != "X" &&//Added by ashish on 18/10/2013
                                         //c.CHQ_EPAY != "B" && //new change done by Vikram as suggested by Mam on 27 Jan 2014
                                         //                                         (c.CHQ_EPAY == null ? "" : c.CHQ_EPAY) != "B" &&//Modified by Abhishek  kamble for Null 9-Apr-2014
                                         //Commented By Abishek kamble to populate 0 amount entry for definalization 11-June-2014
                                         //c.GROSS_AMOUNT > 0 &&
                                         c.ADMIN_ND_CODE == objFilter.AdminNdCode &&
                                          (c.TXN_ID != 228 && c.TXN_ID != 624 && c.TXN_ID != 825) &&
                                          (c.TXN_ID != 229 && c.TXN_ID != 625 && c.TXN_ID != 824) &&
                                         // c.TXN_ID != 228 && //not renewed      //added -02/4/2014 -- While Testing 
                                         // c.TXN_ID != 229 && //not cancelled     //added -02/4/2014 -- While Testing 
                                         !dbContext.ACC_CANCELLED_CHEQUES.Where(g => g.OLD_BILL_ID.Value == c.BILL_ID).Any() &&
                                         // (c.ACC_CHEQUES_ISSUED.IS_CHQ_ENCASHED_NA ==null ||  c.ACC_CHEQUES_ISSUED.IS_CHQ_ENCASHED_NA == false) && //not acknowledged by SRRDA
                                         c.LVL_ID == objFilter.LevelId &&
                                         c.TXN_ID != dbContext.ACC_MASTER_TXN.Where(x => x.BILL_TYPE == "A" && x.FUND_TYPE == objFilter.FundType && x.OP_LVL_ID == objFilter.LevelId && x.IS_OPERATIONAL == true).Select(d => d.TXN_ID).FirstOrDefault() //dont show ack master entry

                                         ).ToList<ACC_BILL_MASTER>();

                    //remove the epayment/eremittances which are finalized by authorized signatory

                    List<ACC_BILL_MASTER> listEpayErem = new List<ACC_BILL_MASTER>();

                    listEpayErem = listBillMaster.Where(x => x.CHQ_EPAY == "E" && x.BILL_FINALIZED == "Y").ToList();

                    //new change done by Vikram on 07-10-2013
                    //remove TOB auto entries
                    List<ACC_BILL_MASTER> listTOBAutoEntry = new List<ACC_BILL_MASTER>();
                    if (objFilter.Bill_type == "J")
                    {
                        listTOBAutoEntry = listBillMaster.Where(m => m.CHQ_NO != null).ToList();
                    }
                    listBillMaster = listBillMaster.Except(listTOBAutoEntry).ToList();
                    //end of change


                    listBillMaster = listBillMaster.Except(listEpayErem).ToList();
                    List<ACC_BILL_MASTER> lstMaster = new List<ACC_BILL_MASTER>();
                    if (objFilter.Bill_type == "P" || objFilter.Bill_type == "R")
                    {

                        lstMaster = listBillMaster.Where(m => m.CHQ_EPAY != "X").ToList();
                        listBillMaster = lstMaster.ToList();
                    }

                }
                else
                { //state
                    //get state admin nd code

                    // int stateAdminNDCode = PMGSYSession.Current.AdminNdCode; //dbContext.ADMIN_DEPARTMENT.Where(c => c.MAST_STATE_CODE == PMGSYSession.Current.StateCode && c.MAST_ND_TYPE=="S").Select(x => x.ADMIN_ND_CODE).First();
                    int stateAdminNDCode = objFilter.SRRDAAdminNdCode;

                    listBillMaster = dbContext.ACC_BILL_MASTER.Where(
                                        c => c.BILL_MONTH == objFilter.Month &&
                                            c.BILL_YEAR == objFilter.Year &&
                                            c.FUND_TYPE == objFilter.FundType &&
                                            c.BILL_TYPE == objFilter.Bill_type &&
                                            c.BILL_FINALIZED == "Y" &&
                                            //(objFilter.Bill_type=="R" || objFilter.Bill_type=="P")?c.CHQ_EPAY !="X":objFilter.Bill_type=="J" &&//Added by ashish on 18/10/2013
                                            c.ADMIN_ND_CODE == stateAdminNDCode &&
                                            //Commented by Abhishek kamble to Popolate 0 Gross amount Ack Voucher for Definalization 8Jan2015
                                            //c.GROSS_AMOUNT > 0 &&
                                            c.TXN_ID != 228 && //not renewed
                                            c.TXN_ID != 229 && //not cancelled
                                            !dbContext.ACC_CANCELLED_CHEQUES.Where(g => g.OLD_BILL_ID.Value == c.BILL_ID).Any() &&
                                            // (c.ACC_CHEQUES_ISSUED.IS_CHQ_ENCASHED_NA ==null ||  c.ACC_CHEQUES_ISSUED.IS_CHQ_ENCASHED_NA == false) && //not acknowledged by SRRDA
                                            c.LVL_ID == objFilter.LevelId
                        //&& c.TXN_ID != dbContext.ACC_MASTER_TXN.Where(x => x.BILL_TYPE == "A" && x.FUND_TYPE == objFilter.FundType && x.OP_LVL_ID == objFilter.LevelId && x.IS_OPERATIONAL == true).Select(d => d.TXN_ID).FirstOrDefault()//dont show ack master entry

                                            ).ToList<ACC_BILL_MASTER>();

                    //new change done by Vikram on 07-10-2013
                    //remove TOB auto entries
                    List<ACC_BILL_MASTER> listTOBAutoEntry = new List<ACC_BILL_MASTER>();
                    if (objFilter.Bill_type == "J")
                    {
                        listTOBAutoEntry = listBillMaster.Where(m => m.CHQ_NO != null).ToList();
                    }
                    listBillMaster = listBillMaster.Except(listTOBAutoEntry).ToList();
                    //end of change

                    List<ACC_BILL_MASTER> lstMaster = new List<ACC_BILL_MASTER>();
                    if (objFilter.Bill_type == "P" || objFilter.Bill_type == "R")
                    {

                        lstMaster = listBillMaster.Where(m => m.CHQ_EPAY != "X").ToList();
                        listBillMaster = lstMaster.ToList();
                    }

                }

                totalRecords = listBillMaster.Count();

                if (objFilter.sidx.Trim() != string.Empty)
                {
                    if (objFilter.sord.ToString() == "asc")
                    {
                        switch (objFilter.sidx)
                        {
                            case "Bill_date":
                                listBillMaster = listBillMaster.OrderBy(x => x.BILL_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "Cheque_Date":
                                listBillMaster = listBillMaster.OrderBy(x => x.CHQ_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;

                            case "Cheque_Amount":
                                listBillMaster = listBillMaster.OrderBy(x => x.CHQ_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "Cash_Amount":
                                listBillMaster = listBillMaster.OrderBy(x => x.CASH_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "Gross_Amount":
                                listBillMaster = listBillMaster.OrderBy(x => x.GROSS_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;

                            case "ref_NO":
                                listBillMaster = listBillMaster.OrderBy(x => x.CHQ_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;

                            case "Voucher_Number":
                                listBillMaster = listBillMaster.OrderBy(x => x.BILL_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;

                            default:
                                listBillMaster = listBillMaster.OrderBy(x => x.BILL_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                        }

                    }
                    else
                    {
                        switch (objFilter.sidx)
                        {

                            case "Bill_date":
                                listBillMaster = listBillMaster.OrderByDescending(x => x.BILL_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "Cheque_Date":
                                listBillMaster = listBillMaster.OrderByDescending(x => x.CHQ_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;

                            case "Cheque_Amount":
                                listBillMaster = listBillMaster.OrderByDescending(x => x.CHQ_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "Cash_Amount":
                                listBillMaster = listBillMaster.OrderByDescending(x => x.CASH_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "Gross_Amount":
                                listBillMaster = listBillMaster.OrderByDescending(x => x.GROSS_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;

                            case "ref_NO":
                                listBillMaster = listBillMaster.OrderByDescending(x => x.CHQ_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;

                            case "Voucher_Number":
                                listBillMaster = listBillMaster.OrderByDescending(x => x.BILL_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;

                            default:
                                listBillMaster = listBillMaster.OrderByDescending(x => x.BILL_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                        }
                    }
                }
                else
                {
                    listBillMaster = listBillMaster.OrderByDescending(x => x.BILL_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                }



                return listBillMaster.Select(item => new
                {
                    id = URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() }),
                    cell = new[] {                         
                                    
                                        item.BILL_NO.ToString(),
                                        commomFuncObj.GetDateTimeToString(item.BILL_DATE),
                                        //item.ACC_MASTER_TXN.TXN_DESC.ToString(),
                                        //Modified by Abhishek kamble sugested by Sriniwas sir to dispaly PIU name for Ack vouchers 26-June-2014
                                        //item.ACC_MASTER_TXN.TXN_DESC.ToString()+ ((item.TXN_ID==212||item.TXN_ID==628||item.TXN_ID==704)? ("-"+ dbContext.ADMIN_DEPARTMENT.Where(c=>c.ADMIN_ND_CODE==dbContext.ACC_BILL_MASTER.Where(m=>m.BILL_ID==dbContext.ACC_CHEQUES_ISSUED.Where(a=>a.NA_BILL_ID==item.BILL_ID).Select(s=>s.BILL_ID).FirstOrDefault()).Select(b=>b.ADMIN_ND_CODE).FirstOrDefault()).Select(e=>e.ADMIN_ND_NAME).FirstOrDefault()):""),
                                        //Modified by Abhishek kamble to dispaly PIU name for Ack vouchers 28-June-2014                                        
                                        
                                        item.ACC_MASTER_TXN.TXN_DESC.ToString()+ ((item.TXN_ID==212||item.TXN_ID==628||item.TXN_ID==704)? (item.CHALAN_NO==null?"":( "-"+GetDPIUNameForAckList(item.CHALAN_NO))):""),

                                       // item.ACC_MASTER_TXN.TXN_DESC.ToString()+ ((item.TXN_ID==212||item.TXN_ID==628||item.TXN_ID==704)? ( item.CHALAN_NO==null?"": ("-"+  (from piuData in dbContext.ADMIN_DEPARTMENT where piuData.ADMIN_ND_CODE==ConvertStringToIntNdCode(item.CHALAN_NO) )  select  new {piuData.ADMIN_ND_NAME} ) )):""),
                                        
                                        
                                        item.CHQ_NO != null ?  item.CHQ_NO.ToString():"",
                                        item.CHQ_DATE.HasValue ?   commomFuncObj.GetDateTimeToString( item.CHQ_DATE.Value) :"",
                                        item.CHQ_AMOUNT.ToString(),
                                        item.CASH_AMOUNT.ToString(),
                                        item.GROSS_AMOUNT.ToString(),
                                        item.PAYEE_NAME,
                                        "<center><a href='#' class='ui-icon ui-icon-search' onclick='ViewDetails(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()})+ "\");return false;'>View Details</a></center>"  ,                                                                                                         //   ACC_CANCELLED_CHEQUES check condition added by Abhishek kamble to dont allow definalization of renewed vaocher 24-July-2014
                                       
                                        // Changes for ADMIN User by rohit borse on 06-06-2022
                                        PMGSYSession.Current.RoleCode == 17 ? "-" 
                                            //old Code before 5Jan2016
                                        : item.BILL_TYPE=="O"?(item.BILL_NO=="1"?"<center><a href='#' class='ui-icon ui-icon-pencil' onclick='DefinalizeVoucher(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()})+ "\");return false;'>Definalize</a></center>":"<center><span class='ui-icon ui-icon-locked'></span></center>"): ( dbContext.ACC_CANCELLED_CHEQUES.Where(m=>m.BILL_ID==item.BILL_ID).Any()?"<span title='Voucher can not be definalize because this is Renewed Cheque.'>-</span>":dbContext.EMARG_PAYMENT_DETAILS.Where(m=>m.OMMAS_BILL_ID==item.BILL_ID).Any()?"<span title='Voucher can not be definalized.'>-</span>":"<center><a href='#' class='ui-icon ui-icon-pencil' onclick='DefinalizeVoucher(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()})+ "\");return false;'>Definalize</a></center>" ),//Changes done by ashish markande on 21/10/2013
                                       //item.BILL_TYPE=="O"?(item.BILL_NO=="1"?"<center><a href='#' class='ui-icon ui-icon-trash' onclick='Delete(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()})+ "\");return false;'>Delete</a></center>" :"<center><span class='ui-icon ui-icon-locked'></span></center>"):"<center><a href='#' class='ui-icon ui-icon-trash' onclick='Delete(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()})+ "\");return false;'>Delete</a></center>" //Changes done by ashish markande on 21/10/2013
                                       item.BILL_TYPE=="O"?(item.BILL_NO=="1"?"<center><a href='#' class='ui-icon ui-icon-trash' onclick='Delete(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()})+ "\");return false;'>Delete</a></center>" :"<center><span class='ui-icon ui-icon-locked'></span></center>"):dbContext.EMARG_PAYMENT_DETAILS.Where(m=>m.OMMAS_BILL_ID==item.BILL_ID).Any()?"<span title='Voucher can not be deleted.'>-</span>":"<center><a href='#' class='ui-icon ui-icon-trash' onclick='Delete(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()})+ "\");return false;'>Delete</a></center>" //Changes done by ashish markande on 21/10/2013 //changes done by Priyanka 17-08-2020

                                       //////New Code 5jan2016 
                                       ////IsOperationAllowed(item.BILL_DATE)? (item.BILL_TYPE=="O"? (item.BILL_NO=="1"?"<center><a href='#' class='ui-icon ui-icon-pencil' onclick='DefinalizeVoucher(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()})+ "\");return false;'>Definalize</a></center>":"<center><span class='ui-icon ui-icon-locked'></span></center>"): ( dbContext.ACC_CANCELLED_CHEQUES.Where(m=>m.BILL_ID==item.BILL_ID).Any()?"<span title='Voucher can not be definalize because this is Renewed Cheque.'>-</span>":"<center><a href='#' class='ui-icon ui-icon-pencil' onclick='DefinalizeVoucher(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()})+ "\");return false;'>Definalize</a></center>" )):"<center><span title='Voucher can not be definalize because definalization time is elapsed.' class='ui-icon ui-icon-locked'></span></center>",
                                       ////IsOperationAllowed(item.BILL_DATE)?(item.BILL_TYPE=="O"?(item.BILL_NO=="1"?"<center><a href='#' class='ui-icon ui-icon-trash' onclick='Delete(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()})+ "\");return false;'>Delete</a></center>" :"<center><span class='ui-icon ui-icon-locked'></span></center>"):"<center><a href='#' class='ui-icon ui-icon-trash' onclick='Delete(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()})+ "\");return false;'>Delete</a></center>"):"<center><span title='Voucher can not be delete because delete time is elapsed.' class='ui-icon ui-icon-locked'></span></center>"                                                                                                                        
                        }
                }).ToArray();

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        //Added By Abhishek kamble 5Jan2016 for Definalize and delete operation restriction
        //public bool IsOperationAllowed(DateTime voucherBillDate)
        //{
        //    if (PMGSYSession.Current.RoleCode == 46)//Finance Role
        //    {
        //        //if voucher is in current month
        //        if (voucherBillDate.Month == DateTime.Now.Month && voucherBillDate.Year == DateTime.Now.Year)
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            int CurrentMonthMaxAllowedDays = 10;//Default
        //            if (!String.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["ACCOUNT_VOUCHER_DEFINALIZE_DELETE_CURRENT_MONTH_MAX_DAYS"]))
        //            {
        //                CurrentMonthMaxAllowedDays = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ACCOUNT_VOUCHER_DEFINALIZE_DELETE_CURRENT_MONTH_MAX_DAYS"]);
        //            }

        //            DateTime minAllowedDate = new DateTime(DateTime.Now.Date.AddMonths(-1).Year, DateTime.Now.Date.AddMonths(-1).Month, 1);
        //            DateTime maxAllowedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, CurrentMonthMaxAllowedDays);

        //            //if (voucherBillDate >= minAllowedDate && voucherBillDate <= maxAllowedDate)
        //            if (voucherBillDate >= minAllowedDate && DateTime.Now <= maxAllowedDate)
        //            {
        //                return true;
        //            }
        //            else
        //            {
        //                return false;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        return true;
        //    }
        //}

        public bool IsOperationAllowed(DateTime voucherBillDate)
        {
            if (PMGSYSession.Current.RoleCode == 46)//Finance Role
            {
                //if voucher is in current month
                if (voucherBillDate.Month == DateTime.Now.Month && voucherBillDate.Year == DateTime.Now.Year)
                {
                    return true;
                }
                else
                {
                    int CurrentMonthMaxAllowedDays = 10;//Default
                    if (!String.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["ACCOUNT_VOUCHER_DEFINALIZE_DELETE_CURRENT_MONTH_MAX_DAYS"]))
                    {
                        CurrentMonthMaxAllowedDays = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ACCOUNT_VOUCHER_DEFINALIZE_DELETE_CURRENT_MONTH_MAX_DAYS"]);
                    }

                    DateTime minAllowedDate = new DateTime(DateTime.Now.Date.AddMonths(-1).Year, DateTime.Now.Date.AddMonths(-1).Month, 1);
                    DateTime maxAllowedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, CurrentMonthMaxAllowedDays);

                    //if (voucherBillDate >= minAllowedDate && voucherBillDate <= maxAllowedDate)
                    if (voucherBillDate.Date >= minAllowedDate.Date && DateTime.Now.Date <= maxAllowedDate.Date)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return true;
            }
        }


        public string GetDPIUNameForAckList(string strndcode)
        {
            int ndcode = Convert.ToInt32(strndcode);
            string DpiuName = "";
            //dbContext = new PMGSYEntities();

            PMGSYEntities dbCon = new PMGSYEntities();

            try
            {
                DpiuName = dbCon.ADMIN_DEPARTMENT.Where(c => c.ADMIN_ND_CODE == ndcode).Select(e => e.ADMIN_ND_NAME).FirstOrDefault();
                return DpiuName;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return "";
            }
            finally
            {
                if (dbCon != null)
                {
                    dbCon.Dispose();
                }
            }
        }

        /// <summary>
        /// DAL function to return the transaction details
        /// </summary>
        /// <param name="objFilter"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array ListVoucherTransactionDetails(VoucherFilterModel objFilter, out long totalRecords)
        {

            CommonFunctions commomFuncObj = null;
            commomFuncObj = new CommonFunctions();
            TransactionParams objParam = new TransactionParams();
            List<transactionList> lstTransactions = new List<transactionList>();

            try
            {

                dbContext = new PMGSYEntities();
                var billType = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == objFilter.BillId).Select(m => m.BILL_TYPE).FirstOrDefault();

                if (billType == "O" || billType == "J")
                {
                    var queryOpening = (from master in dbContext.ACC_BILL_MASTER
                                        join details in dbContext.ACC_BILL_DETAILS
                                        on master.BILL_ID equals details.BILL_ID
                                        join headMaster in dbContext.ACC_MASTER_HEAD
                                        on details.HEAD_ID equals headMaster.HEAD_ID
                                        where master.BILL_ID == objFilter.BillId
                                            //  && master.BILL_TYPE == "O" ? (details.CREDIT_DEBIT == "C" || details.CREDIT_DEBIT == "D") : details.CREDIT_DEBIT == "D"
                                        && (details.CREDIT_DEBIT == "D" || details.CREDIT_DEBIT == "C")
                                        select new
                                        {
                                            master.BILL_FINALIZED,

                                            details.BILL_ID,

                                            details.ACC_MASTER_HEAD.HEAD_CODE,
                                            details.HEAD_ID,
                                            details.TXN_ID,
                                            details.CASH_CHQ,
                                            details.AMOUNT,
                                            details.NARRATION,
                                            details.TXN_NO,
                                            details.IMS_AGREEMENT_CODE,
                                            details.IMS_PR_ROAD_CODE,
                                            master.CHQ_EPAY,
                                            details.MAST_CON_ID,
                                            details.FINAL_PAYMENT,
                                            master.ADMIN_ND_CODE,
                                            headMaster.HEAD_NAME,
                                            details.CREDIT_DEBIT
                                        });
                    foreach (var item in queryOpening)
                    {

                        transactionList obj = new transactionList();

                        obj.BILL_FINALIZED = item.BILL_FINALIZED;
                        obj.AMOUNT = item.AMOUNT;
                        obj.BILL_ID = item.BILL_ID;
                        obj.CASH_CHQ = item.CASH_CHQ;
                        //obj.HEAD_ID_Narration = dbContext.ACC_MASTER_TXN.Where(x => x.TXN_ID == item.TXN_ID).Select(h => h.TXN_DESC).FirstOrDefault();
                        obj.HEAD_ID_Narration = item.HEAD_NAME + (item.CREDIT_DEBIT == "C" ? " (Credit)" : " (Debit)");
                        obj.HEAD_ID = item.HEAD_CODE.ToString();
                        obj.NARRATION = item.NARRATION;
                        obj.TXN_NO = item.TXN_NO;
                        obj.RODE_CODE = dbContext.IMS_SANCTIONED_PROJECTS.Where(c => c.IMS_PR_ROAD_CODE == (item.IMS_PR_ROAD_CODE.HasValue ? item.IMS_PR_ROAD_CODE.Value : -1)).Select(y => y.IMS_ROAD_NAME).FirstOrDefault();
                        obj.AGREEMENT_CODE = dbContext.TEND_AGREEMENT_MASTER.Where(c => c.TEND_AGREEMENT_CODE == (item.IMS_AGREEMENT_CODE.HasValue ? item.IMS_AGREEMENT_CODE.Value : -1)).Select(y => y.TEND_AGREEMENT_NUMBER).FirstOrDefault();
                        obj.MASTER_CHQEPAY = item.CHQ_EPAY;
                        obj.paymentType = obj.CASH_CHQ == "C" ? "Payment" : item.CASH_CHQ == "Q" ? "Payment" : "Deduction";
                        obj.FINALPAYMENT = item.FINAL_PAYMENT.ToString() == "Y" ? "YES" : "NO";
                        obj.PIUNAME = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == item.ADMIN_ND_CODE).Select(t => t.ADMIN_ND_NAME).First();

                        if (item.MAST_CON_ID.HasValue)
                        {
                            objParam.MAST_CONT_ID = item.MAST_CON_ID.Value;
                            obj.CONTRACTORNAME = commomFuncObj.GetContractorSupplierName(objParam);
                        }
                        else
                        {
                            obj.CONTRACTORNAME = String.Empty;
                        }


                        lstTransactions.Add(obj);

                    }

                    lstTransactions = lstTransactions.OrderByDescending(x => x.paymentType == "Payment").ToList();

                    totalRecords = lstTransactions.Count();
                    return lstTransactions.Select(item => new
                    {

                        cell = new[] {                         
                                   
                                                              
                                        
                                        item.HEAD_ID.ToString() + " | "+ item.HEAD_ID_Narration,
                                        item.AMOUNT.ToString(),
                                        item.NARRATION.ToString(),
                                        item.CONTRACTORNAME,
                                        item.AGREEMENT_CODE,
                                        item.RODE_CODE,
                                        item.PIUNAME,
                                        item.FINALPAYMENT,
                                        item.CASH_CHQ=="C"? "Payment" : item.CASH_CHQ=="Q"? "Payment" : "Deduction",
                                                    
                                       
                        }
                    }).ToArray();
                }
                else
                {
                    var queryOther = (from master in dbContext.ACC_BILL_MASTER
                                      join details in dbContext.ACC_BILL_DETAILS
                                      on master.BILL_ID equals details.BILL_ID
                                      join headMaster in dbContext.ACC_MASTER_HEAD
                                       on details.HEAD_ID equals headMaster.HEAD_ID
                                      where master.BILL_ID == objFilter.BillId
                                          //  && master.BILL_TYPE == "O" ? (details.CREDIT_DEBIT == "C" || details.CREDIT_DEBIT == "D") : details.CREDIT_DEBIT == "D"
                                      && //details.CREDIT_DEBIT == "D" &&
                                      (master.BILL_TYPE == "P" ? details.CREDIT_DEBIT == "D" : (master.BILL_TYPE == "R") ? details.CREDIT_DEBIT == "C" : 1 == 1)
                                      select new
                                      {
                                          master.BILL_FINALIZED,

                                          details.BILL_ID,

                                          details.ACC_MASTER_HEAD.HEAD_CODE,
                                          details.HEAD_ID,
                                          details.TXN_ID,
                                          details.CASH_CHQ,
                                          details.AMOUNT,
                                          details.NARRATION,
                                          details.TXN_NO,
                                          details.IMS_AGREEMENT_CODE,
                                          details.IMS_PR_ROAD_CODE,
                                          master.CHQ_EPAY,
                                          details.MAST_CON_ID,
                                          details.FINAL_PAYMENT,
                                          master.ADMIN_ND_CODE,
                                          headMaster.HEAD_NAME
                                      });
                    foreach (var item in queryOther)
                    {

                        transactionList obj = new transactionList();

                        obj.BILL_FINALIZED = item.BILL_FINALIZED;
                        obj.AMOUNT = item.AMOUNT;
                        obj.BILL_ID = item.BILL_ID;
                        obj.CASH_CHQ = item.CASH_CHQ;
                        //obj.HEAD_ID_Narration = dbContext.ACC_MASTER_TXN.Where(x => x.TXN_ID == item.TXN_ID).Select(h => h.TXN_DESC).FirstOrDefault();
                        obj.HEAD_ID_Narration = item.HEAD_NAME;
                        obj.HEAD_ID = item.HEAD_CODE.ToString();
                        obj.NARRATION = item.NARRATION;
                        obj.TXN_NO = item.TXN_NO;
                        obj.RODE_CODE = dbContext.IMS_SANCTIONED_PROJECTS.Where(c => c.IMS_PR_ROAD_CODE == (item.IMS_PR_ROAD_CODE.HasValue ? item.IMS_PR_ROAD_CODE.Value : -1)).Select(y => y.IMS_ROAD_NAME).FirstOrDefault();
                        obj.AGREEMENT_CODE = dbContext.TEND_AGREEMENT_MASTER.Where(c => c.TEND_AGREEMENT_CODE == (item.IMS_AGREEMENT_CODE.HasValue ? item.IMS_AGREEMENT_CODE.Value : -1)).Select(y => y.TEND_AGREEMENT_NUMBER).FirstOrDefault();
                        obj.MASTER_CHQEPAY = item.CHQ_EPAY;
                        obj.paymentType = obj.CASH_CHQ == "C" ? "Payment" : item.CASH_CHQ == "Q" ? "Payment" : "Deduction";
                        obj.FINALPAYMENT = item.FINAL_PAYMENT.ToString() == "Y" ? "YES" : "NO";
                        obj.PIUNAME = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == item.ADMIN_ND_CODE).Select(t => t.ADMIN_ND_NAME).First();

                        if (item.MAST_CON_ID.HasValue)
                        {
                            objParam.MAST_CONT_ID = item.MAST_CON_ID.Value;
                            obj.CONTRACTORNAME = commomFuncObj.GetContractorSupplierName(objParam);
                        }
                        else
                        {
                            obj.CONTRACTORNAME = String.Empty;
                        }


                        lstTransactions.Add(obj);

                    }

                    lstTransactions = lstTransactions.OrderByDescending(x => x.paymentType == "Payment").ToList();

                    totalRecords = lstTransactions.Count();
                    return lstTransactions.Select(item => new
                    {

                        cell = new[] {                         
                                   
                                                              
                                        
                                        item.HEAD_ID.ToString() + " | "+ item.HEAD_ID_Narration,
                                        item.AMOUNT.ToString(),
                                        item.NARRATION.ToString(),
                                        item.CONTRACTORNAME,
                                        item.AGREEMENT_CODE,
                                        item.RODE_CODE,
                                        item.PIUNAME,
                                        item.FINALPAYMENT,
                                        item.CASH_CHQ=="C"? "Payment" : item.CASH_CHQ=="Q"? "Payment" : "Deduction",
                                                    
                                       
                        }
                    }).ToArray();

                }
            }



                // query = query.OrderBy(c => c.CASH_CHQ == "Q").ThenBy(t => t.CASH_CHQ == "C").ThenBy(t => t.CASH_CHQ == "D");


            //    foreach (var item in query)
            //    {

            //        transactionList obj = new transactionList();

            //        obj.BILL_FINALIZED = item.BILL_FINALIZED;
            //        obj.AMOUNT = item.AMOUNT;
            //        obj.BILL_ID = item.BILL_ID;
            //        obj.CASH_CHQ = item.CASH_CHQ;
            //        obj.HEAD_ID_Narration = dbContext.ACC_MASTER_TXN.Where(x => x.TXN_ID == item.TXN_ID).Select(h => h.TXN_DESC).FirstOrDefault();
            //        obj.HEAD_ID = item.HEAD_CODE.ToString();
            //        obj.NARRATION = item.NARRATION;
            //        obj.TXN_NO = item.TXN_NO;
            //        obj.RODE_CODE = dbContext.IMS_SANCTIONED_PROJECTS.Where(c => c.IMS_PR_ROAD_CODE == (item.IMS_PR_ROAD_CODE.HasValue ? item.IMS_PR_ROAD_CODE.Value : -1)).Select(y => y.IMS_ROAD_NAME).FirstOrDefault();
            //        obj.AGREEMENT_CODE = dbContext.TEND_AGREEMENT_MASTER.Where(c => c.TEND_AGREEMENT_CODE == (item.IMS_AGREEMENT_CODE.HasValue ? item.IMS_AGREEMENT_CODE.Value : -1)).Select(y => y.TEND_AGREEMENT_NUMBER).FirstOrDefault();
            //        obj.MASTER_CHQEPAY = item.CHQ_EPAY;
            //        obj.paymentType = obj.CASH_CHQ == "C" ? "Payment" : item.CASH_CHQ == "Q" ? "Payment" : "Deduction";
            //        obj.FINALPAYMENT = item.FINAL_PAYMENT.ToString() =="Y" ? "YES": "NO";
            //        obj.PIUNAME = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == item.ADMIN_ND_CODE).Select(t => t.ADMIN_ND_NAME).First();

            //        if(item.MAST_CON_ID.HasValue)
            //        {
            //             objParam.MAST_CONT_ID = item.MAST_CON_ID.Value ;
            //             obj.CONTRACTORNAME =commomFuncObj.GetContractorSupplierName(objParam);
            //        }
            //        else {
            //         obj.CONTRACTORNAME =String.Empty;
            //        }


            //        lstTransactions.Add(obj);

            //    }

            //    lstTransactions = lstTransactions.OrderByDescending(x => x.paymentType == "Payment").ToList();

            //    totalRecords = lstTransactions.Count();
            //    return lstTransactions.Select(item => new
            //    {

            //        cell = new[] {                         



            //                            item.HEAD_ID.ToString() + " | "+ item.HEAD_ID_Narration,
            //                            item.AMOUNT.ToString(),
            //                            item.NARRATION.ToString(),
            //                            item.CONTRACTORNAME,
            //                            item.AGREEMENT_CODE,
            //                            item.RODE_CODE,
            //                            item.PIUNAME,
            //                            item.FINALPAYMENT,
            //                            item.CASH_CHQ=="C"? "Payment" : item.CASH_CHQ=="Q"? "Payment" : "Deduction",


            //            }
            //    }).ToArray();

            //}
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }


        }


        /// <summary>
        /// DAL function to  Definalize Voucher
        /// </summary>
        /// <param name="bill_id"> bill id of the voucher to definalize</param>
        /// <returns> Status of operation</returns>
        public String DefinalizeVoucher(long bill_id)
        {
            dbContext = new PMGSYEntities();
            try
            {
                string fundType = PMGSYSession.Current.FundType;
                int adminNdCode = PMGSYSession.Current.AdminNdCode;
                short levelID = PMGSYSession.Current.LevelId;

                String result = ValidateVoucher(bill_id);
                if (result == "1")
                {

                    //using (var scope = new TransactionScope())
                    {
                        //get the master details
                        ACC_BILL_MASTER master = dbContext.ACC_BILL_MASTER.SingleOrDefault(p => p.BILL_ID == bill_id);

                        if (adminNdCode == 0)
                        {
                            adminNdCode = master.ADMIN_ND_CODE;
                        }

                        long authId = dbContext.ACC_AUTH_REQUEST_TRACKING.Where(m => m.BILL_ID == bill_id).Select(m => m.AUTH_ID).FirstOrDefault();
                        //check if bill is of auhorization requests receipt or payment  
                        if (!string.IsNullOrEmpty(master.CHQ_EPAY) && master.CHQ_EPAY.Equals("B")) //addded condition by koustubh Nakate on 07/10/2013 
                        {

                            //if receipt delete its details
                            if (master.BILL_TYPE.Equals("R"))
                            {
                                ACC_AUTH_REQUEST_TRACKING receiptRecord = new ACC_AUTH_REQUEST_TRACKING();
                                ACC_AUTH_REQUEST_TRACKING paymentRecord = null;


                                receiptRecord = dbContext.ACC_AUTH_REQUEST_TRACKING.Single(x => x.BILL_ID == bill_id && x.AUTH_STATUS == "R");
                                if (dbContext.ACC_AUTH_REQUEST_TRACKING.Any(x => x.AUTH_ID == receiptRecord.AUTH_ID && x.AUTH_STATUS == "P"))
                                {
                                    paymentRecord = dbContext.ACC_AUTH_REQUEST_TRACKING.Single(x => x.AUTH_ID == receiptRecord.AUTH_ID && x.AUTH_STATUS == "P");
                                }

                                //updating the status of Auth master
                                //long authId = dbContext.ACC_AUTH_REQUEST_TRACKING.Where(m => m.BILL_ID == bill_id).Select(m => m.AUTH_ID).FirstOrDefault();


                                //get the  payment entries from bill master
                                ACC_BILL_MASTER paymentEntry = null;
                                if (paymentRecord != null)
                                {
                                    paymentEntry = dbContext.ACC_BILL_MASTER.SingleOrDefault(p => p.BILL_ID == paymentRecord.BILL_ID);
                                }

                                //remove from auth request tracking
                                dbContext.ACC_AUTH_REQUEST_TRACKING.Remove(receiptRecord);
                                if (paymentRecord != null)
                                {
                                    dbContext.ACC_AUTH_REQUEST_TRACKING.Remove(paymentRecord);
                                }


                                //remove from bill details
                                //delete the details from bill details table
                                if (paymentEntry != null)
                                {
                                    dbContext.Database.ExecuteSqlCommand("DELETE [omms].ACC_BILL_DETAILS Where BILL_ID = {0}", paymentEntry.BILL_ID); // delete the payment entry

                                    //remove from ACC_Cheque_Issued table 24sep2014  for Bank auth issued Payment entry details.
                                    dbContext.Database.ExecuteSqlCommand("DELETE [omms].ACC_CHEQUES_ISSUED where BILL_ID={0}", paymentEntry.BILL_ID);
                                }
                                dbContext.Database.ExecuteSqlCommand("DELETE [omms].ACC_BILL_DETAILS Where BILL_ID = {0}", master.BILL_ID); // delete the receipt entry


                                //remove from bill master
                                if (paymentEntry != null)
                                {
                                    dbContext.ACC_BILL_MASTER.Remove(paymentEntry);//payment
                                }
                                dbContext.ACC_BILL_MASTER.Remove(master); //receipt

                                if (authId != null)
                                {
                                    ACC_AUTH_REQUEST_MASTER authMaster = dbContext.ACC_AUTH_REQUEST_MASTER.Where(m => m.AUTH_ID == authId).FirstOrDefault();
                                    authMaster.CURRENT_AUTH_STATUS = "A";
                                    dbContext.Entry(authMaster).State = System.Data.Entity.EntityState.Modified;
                                    dbContext.SaveChanges();
                                }



                            }
                            else if (master.BILL_TYPE.Equals("P"))
                            {
                                ACC_AUTH_REQUEST_TRACKING paymentRecord = new ACC_AUTH_REQUEST_TRACKING();

                                paymentRecord = dbContext.ACC_AUTH_REQUEST_TRACKING.Single(x => x.BILL_ID == bill_id && x.AUTH_STATUS == "P");

                                //remove paymemnt from tracking details
                                dbContext.ACC_AUTH_REQUEST_TRACKING.Remove(paymentRecord);

                                //remove from bill details 
                                dbContext.Database.ExecuteSqlCommand("DELETE [omms].ACC_BILL_DETAILS Where BILL_ID = {0}", master.BILL_ID);


                                //remove from ACC_Cheque_Issued table 24sep2014  for Bank auth issued Payment entry details.
                                dbContext.Database.ExecuteSqlCommand("DELETE [omms].ACC_CHEQUES_ISSUED where BILL_ID={0}", master.BILL_ID);

                                //remove only payment entry from  master table
                                dbContext.ACC_BILL_MASTER.Remove(master); //payment

                                // updating the status of authorization if payment is deleted.
                                if (authId != null)
                                {
                                    ACC_AUTH_REQUEST_MASTER authMaster = dbContext.ACC_AUTH_REQUEST_MASTER.Where(m => m.AUTH_ID == authId).FirstOrDefault();
                                    authMaster.CURRENT_AUTH_STATUS = "R";
                                    dbContext.Entry(authMaster).State = System.Data.Entity.EntityState.Modified;
                                    dbContext.SaveChanges();
                                }
                            }

                        }
                        else
                        {
                            //Added By ashish markande ON 16/10/2013
                            if (master.BILL_TYPE == "O")
                            {
                                long assetId = bill_id;
                                long liabilitiesId = bill_id + 1;
                                long[] id = { assetId, liabilitiesId };
                                foreach (long item in id)
                                {
                                    master = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == item).FirstOrDefault();

                                    master.BILL_FINALIZED = "N";

                                    dbContext.Entry(master).State = System.Data.Entity.EntityState.Modified;

                                    dbContext.SaveChanges();
                                }
                            }
                            else
                            {

                                master.BILL_FINALIZED = "N";

                                dbContext.Entry(master).State = System.Data.Entity.EntityState.Modified;

                                dbContext.SaveChanges();
                            }

                            //added by Koustubh Nakate on 21/08/2013 to save notification in notification details table 
                            //dbContext.USP_ACC_INSERT_ALERT_DETAILS(adminNdCode, fundType, "U", levelID, bill_id, PMGSYSession.Current.UserId, HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]);

                            //new change done by Vikram on 07-10-2013
                            string bill = bill_id.ToString();
                            if (PMGSYSession.Current.FundType == "P")
                            {
                                List<ACC_BILL_MASTER> lstMaster = dbContext.ACC_BILL_MASTER.Where(m => m.CHQ_NO == bill && m.TXN_ID == 1196).ToList();
                                foreach (var item in lstMaster)
                                {
                                    /*  start of code by anita */
                                    
                                    ACC_BILL_MASTER master1 = dbContext.ACC_BILL_MASTER.SingleOrDefault(p => p.BILL_ID == item.BILL_ID);

                                    if (dbContext.ACC_RPT_MONTHWISE_SUMMARY.Where(x => x.ADMIN_ND_CODE == master1.ADMIN_ND_CODE
                        && x.ACC_MONTH == master1.BILL_MONTH && x.ACC_YEAR == master1.BILL_YEAR && x.FUND_TYPE == PMGSYSession.Current.FundType).Any())
                                    {
                                        return "-999";  // piu month is closed
                                    }
                                    /*  end of code by anita */
                                    else
                                    {

                                        dbContext.Database.ExecuteSqlCommand("DELETE [omms].ACC_BILL_DETAILS Where BILL_ID = {0}", item.BILL_ID);
                                        dbContext.ACC_BILL_MASTER.Remove(item);
                                        dbContext.SaveChanges();
                                    }


                                }
                            }
                            if (PMGSYSession.Current.FundType == "A")
                            {
                                List<ACC_BILL_MASTER> lstMaster = dbContext.ACC_BILL_MASTER.Where(m => m.CHQ_NO == bill && m.TXN_ID == 1197).ToList();
                                foreach (var item in lstMaster)
                                {
                                    ACC_BILL_MASTER master1 = dbContext.ACC_BILL_MASTER.SingleOrDefault(p => p.BILL_ID == item.BILL_ID);

                                    if (dbContext.ACC_RPT_MONTHWISE_SUMMARY.Where(x => x.ADMIN_ND_CODE == master1.ADMIN_ND_CODE
                        && x.ACC_MONTH == master1.BILL_MONTH && x.ACC_YEAR == master1.BILL_YEAR && x.FUND_TYPE == PMGSYSession.Current.FundType).Any())
                                    {
                                        return "-999";  // piu month is closed
                                    }
                                    else
                                    {
                                        dbContext.Database.ExecuteSqlCommand("DELETE [omms].ACC_BILL_DETAILS Where BILL_ID = {0}", item.BILL_ID);
                                        dbContext.ACC_BILL_MASTER.Remove(item);
                                        dbContext.SaveChanges();
                                    }
                                }
                            }
                            if (PMGSYSession.Current.FundType == "M")
                            {
                                List<ACC_BILL_MASTER> lstMaster = dbContext.ACC_BILL_MASTER.Where(m => m.CHQ_NO == bill && m.TXN_ID == 1198).ToList();
                                foreach (var item in lstMaster)
                                {
                                    ACC_BILL_MASTER master1 = dbContext.ACC_BILL_MASTER.SingleOrDefault(p => p.BILL_ID == item.BILL_ID);

                                    if (dbContext.ACC_RPT_MONTHWISE_SUMMARY.Where(x => x.ADMIN_ND_CODE == master1.ADMIN_ND_CODE
                        && x.ACC_MONTH == master1.BILL_MONTH && x.ACC_YEAR == master1.BILL_YEAR && x.FUND_TYPE == PMGSYSession.Current.FundType).Any())
                                    {
                                        return "-999";  // piu month is closed
                                    }
                                    else
                                    {
                                        dbContext.Database.ExecuteSqlCommand("DELETE [omms].ACC_BILL_DETAILS Where BILL_ID = {0}", item.BILL_ID);
                                        dbContext.ACC_BILL_MASTER.Remove(item);
                                        dbContext.SaveChanges();
                                    }
                                }
                            }

                            if (dbContext.ACC_NOTIFICATION_DETAILS.Any(m => m.INITIATION_BILL_ID == bill_id && m.FUND_TYPE == PMGSYSession.Current.FundType))
                            {
                                List<ACC_NOTIFICATION_DETAILS> lstDetailsMaster = dbContext.ACC_NOTIFICATION_DETAILS.Where(m => m.FUND_TYPE == PMGSYSession.Current.FundType && m.INITIATION_BILL_ID == bill_id && m.RECEIVER_ADMIN_ND_CODE != PMGSYSession.Current.AdminNdCode).ToList();
                                foreach (var item in lstDetailsMaster)
                                {
                                    dbContext.ACC_NOTIFICATION_DETAILS.Remove(item);
                                    dbContext.SaveChanges();
                                }
                            }

                            //end of change

                        }
                        dbContext.SaveChanges();



                        //scope.Complete();

                        return "1";
                    }

                }
                else
                {
                    return result;
                }


            }
            catch(DbUpdateException ex)
            {
                var sqlex = ex.InnerException.InnerException as SqlException;
                sqlex.Message.ToString();
                return "1";
            }
            catch (Exception ex)
            {
                return "1";
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                // throw new Exception("Error while definalizing the voucher");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// AL function to delete the voucher
        /// </summary>
        /// <param name="bill_id"> bill id of the voucher to definalize</param>
        /// <returns> Status of operation</returns>
        public String DeleteVoucher(long bill_id)
        {
            dbContext = new PMGSYEntities();
            // bool chequeRenewalDeleteStatus = true;
            try
            {
                String result = ValidateVoucher(bill_id);
                if (result == "1")
                {

                    using (var scope = new TransactionScope())
                    {
                        //get the master details
                        ACC_BILL_MASTER master = dbContext.ACC_BILL_MASTER.SingleOrDefault(p => p.BILL_ID == bill_id);

                        if (!string.IsNullOrEmpty(master.CHQ_EPAY) && master.CHQ_EPAY.Equals("B")) //addded condition by koustubh Nakate on 07/10/2013 
                        {
                            //get Auth Id
                            long? AuthID = dbContext.ACC_AUTH_REQUEST_TRACKING.Where(m => m.BILL_ID == bill_id).Select(s => s.AUTH_ID).FirstOrDefault();

                            //if receipt delete its details
                            if (master.BILL_TYPE.Equals("R"))
                            {
                                ACC_AUTH_REQUEST_TRACKING receiptRecord = new ACC_AUTH_REQUEST_TRACKING();
                                ACC_AUTH_REQUEST_TRACKING paymentRecord = new ACC_AUTH_REQUEST_TRACKING();
                                long? paymentBIllId = 0;

                                receiptRecord = dbContext.ACC_AUTH_REQUEST_TRACKING.Where(x => x.BILL_ID == bill_id && x.AUTH_STATUS == "R").FirstOrDefault();

                                paymentRecord = dbContext.ACC_AUTH_REQUEST_TRACKING.Where(x => x.AUTH_ID == receiptRecord.AUTH_ID && x.AUTH_STATUS == "P").FirstOrDefault();

                                //remove from auth request tracking
                                dbContext.ACC_AUTH_REQUEST_TRACKING.Remove(receiptRecord);
                                if (paymentRecord != null)
                                {
                                    paymentBIllId = paymentRecord.BILL_ID;
                                    dbContext.ACC_AUTH_REQUEST_TRACKING.Remove(paymentRecord);
                                }

                                //remove from bill details
                                //delete the details from bill details table
                                if (paymentBIllId != null && paymentBIllId != 0)
                                {
                                    dbContext.Database.ExecuteSqlCommand("DELETE [omms].ACC_BILL_DETAILS Where BILL_ID = {0}", paymentBIllId); // delete the payment entry
                                }
                                dbContext.Database.ExecuteSqlCommand("DELETE [omms].ACC_BILL_DETAILS Where BILL_ID = {0}", master.BILL_ID); // delete the receipt entry

                                //remove payment Master Records
                                if (paymentBIllId != null && paymentBIllId != 0)
                                {
                                    //get the  payment entries from bill master
                                    ACC_BILL_MASTER paymentEntry = dbContext.ACC_BILL_MASTER.SingleOrDefault(p => p.BILL_ID == paymentBIllId);
                                    dbContext.ACC_BILL_MASTER.Remove(paymentEntry);//payment
                                }
                                //remove from Receipt bill master
                                dbContext.ACC_BILL_MASTER.Remove(master); //receipt

                                //Change Auth Status to approved.
                                if (AuthID != null)
                                {
                                    ACC_AUTH_REQUEST_MASTER authMaster = dbContext.ACC_AUTH_REQUEST_MASTER.Where(m => m.AUTH_ID == AuthID).FirstOrDefault();
                                    authMaster.CURRENT_AUTH_STATUS = "A";
                                    dbContext.Entry(authMaster).State = System.Data.Entity.EntityState.Modified;
                                }

                            }
                            else if (master.BILL_TYPE.Equals("P"))
                            {
                                ACC_AUTH_REQUEST_TRACKING paymentRecord = new ACC_AUTH_REQUEST_TRACKING();

                                paymentRecord = dbContext.ACC_AUTH_REQUEST_TRACKING.Where(x => x.BILL_ID == bill_id && x.AUTH_STATUS == "P").FirstOrDefault();

                                //remove paymemnt from tracking details
                                dbContext.ACC_AUTH_REQUEST_TRACKING.Remove(paymentRecord);

                                //remove paymemnt details from ACC_BILL_DETAILS
                                dbContext.Database.ExecuteSqlCommand("DELETE [omms].ACC_BILL_DETAILS Where BILL_ID = {0}", bill_id); // delete the payment details entry

                                //remove payment entry only
                                dbContext.ACC_BILL_MASTER.Remove(master); //payment


                                //Change Auth Status to approved.
                                if (AuthID != null)
                                {
                                    ACC_AUTH_REQUEST_MASTER authMaster = dbContext.ACC_AUTH_REQUEST_MASTER.Where(m => m.AUTH_ID == AuthID).FirstOrDefault();

                                    //Check for receipt details 
                                    ACC_AUTH_REQUEST_TRACKING receiptRecord = dbContext.ACC_AUTH_REQUEST_TRACKING.Where(x => x.AUTH_ID == AuthID && x.AUTH_STATUS == "R").FirstOrDefault();

                                    if (receiptRecord != null)
                                    {
                                        authMaster.CURRENT_AUTH_STATUS = "R";
                                    }
                                    else
                                    {
                                        authMaster.CURRENT_AUTH_STATUS = "A";
                                    }
                                    dbContext.Entry(authMaster).State = System.Data.Entity.EntityState.Modified;
                                }

                            }
                        }
                        else
                        {

                            //new change done by Vikram on 07-10-2013
                            string bill = bill_id.ToString();
                            if (PMGSYSession.Current.FundType == "P")
                            {
                                ///Change for deleting TEO -auto entries
                                List<ACC_BILL_MASTER> lstMaster = dbContext.ACC_BILL_MASTER.Where(m => m.CHQ_NO == bill && (m.TXN_ID == 1196 || m.TXN_ID == 1216)).ToList();
                                foreach (var item in lstMaster)
                                {
                                    dbContext.Database.ExecuteSqlCommand("DELETE [omms].ACC_BILL_DETAILS Where BILL_ID = {0}", item.BILL_ID);
                                    dbContext.ACC_BILL_MASTER.Remove(item);
                                    dbContext.SaveChanges();
                                }
                            }
                            if (PMGSYSession.Current.FundType == "A")
                            {
                                List<ACC_BILL_MASTER> lstMaster = dbContext.ACC_BILL_MASTER.Where(m => m.CHQ_NO == bill && m.TXN_ID == 1197).ToList();
                                foreach (var item in lstMaster)
                                {
                                    dbContext.Database.ExecuteSqlCommand("DELETE [omms].ACC_BILL_DETAILS Where BILL_ID = {0}", item.BILL_ID);
                                    dbContext.ACC_BILL_MASTER.Remove(item);
                                    dbContext.SaveChanges();
                                }
                            }
                            if (PMGSYSession.Current.FundType == "M")
                            {
                                List<ACC_BILL_MASTER> lstMaster = dbContext.ACC_BILL_MASTER.Where(m => m.CHQ_NO == bill && m.TXN_ID == 1198).ToList();
                                foreach (var item in lstMaster)
                                {
                                    dbContext.Database.ExecuteSqlCommand("DELETE [omms].ACC_BILL_DETAILS Where BILL_ID = {0}", item.BILL_ID);
                                    dbContext.ACC_BILL_MASTER.Remove(item);
                                    dbContext.SaveChanges();
                                }
                            }

                            //end of change

                            //Changes done by ashish markande on 21/10/2013



                        }
                        if (master.BILL_TYPE == "O")
                        {
                            string billId = bill_id.ToString();
                            Int64 assetId = Convert.ToInt64(billId);
                            Int64 liabilitiesId = assetId + 1;
                            long[] arrAssetLib = { assetId, liabilitiesId };
                            foreach (long item in arrAssetLib)
                            {
                                master = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == item).FirstOrDefault();
                                dbContext.Database.ExecuteSqlCommand
                                ("DELETE [omms].ACC_CHEQUES_ISSUED Where BILL_ID = {0}", master.BILL_ID);

                                //delete the details from bill details table
                                dbContext.Database.ExecuteSqlCommand
                                ("DELETE [omms].ACC_BILL_DETAILS Where BILL_ID = {0}", master.BILL_ID);

                                //delete the master table entry
                                dbContext.ACC_BILL_MASTER.Remove(master);
                            }
                        }
                        else if (master.BILL_TYPE == "J")
                        {

                            //delete the cheque issued entry if allredy exist
                            dbContext.Database.ExecuteSqlCommand
                                ("DELETE [omms].ACC_CHEQUES_ISSUED Where BILL_ID = {0}", bill_id);

                            //delete the details from bill details table
                            dbContext.Database.ExecuteSqlCommand
                                 ("DELETE [omms].ACC_BILL_DETAILS Where BILL_ID = {0}", bill_id);

                            dbContext.Database.ExecuteSqlCommand
                                ("DELETE [omms].ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS Where S_BILL_ID = {0}", bill_id);

                            //delete the master table entry
                            dbContext.ACC_BILL_MASTER.Remove(master);

                        }//end of change                            
                        else
                        {

                            //If condition added by Abhishek kamble 24-July-2014 to Delete renewal of cheque voucher details
                            if (dbContext.ACC_CANCELLED_CHEQUES.Where(m => m.BILL_ID == bill_id).Any())
                            {
                                //cal SP to delete renewal voucher details
                                var status = dbContext.USP_ACC_DELETE_CHEQUE_RENEWAL_DETAILS(bill_id);

                                int? statusCode = status.Select(s => s.Value).FirstOrDefault();

                                if (statusCode == 0)
                                {
                                    return "-8080";
                                }
                                //chequeRenewalDeleteStatus = false;
                            }
                            else
                            {
                                //If condition added by abhishek kamble to modify ACC_CHEQUES_ISSUED table for IS_CHQ_ENCASHED_NA=0 and NA_BILL_IS=NULL for delete Ack voucher
                                //By Sriniwas Sir.
                                if ((master.TXN_ID == 212 || master.TXN_ID == 628 || master.TXN_ID == 704) && master.BILL_TYPE == "P")
                                {
                                    var AccChequesIssuedModel = dbContext.ACC_CHEQUES_ISSUED.Where(x => x.NA_BILL_ID == bill_id).ToList();
                                    AccChequesIssuedModel.ForEach(m => { m.NA_BILL_ID = null; m.IS_CHQ_ENCASHED_NA = false; });
                                }
                                else
                                {
                                    dbContext.Database.ExecuteSqlCommand
                                            ("DELETE [omms].ACC_CHEQUES_ISSUED Where BILL_ID = {0}", bill_id);
                                }
                                //delete the details from bill details table
                                dbContext.Database.ExecuteSqlCommand
                                     ("DELETE [omms].ACC_BILL_DETAILS Where BILL_ID = {0}", bill_id);

                                //Check entry present In ACC_TXN_BANK Added By Abhishek kamble 14-July-2014
                                if (dbContext.ACC_TXN_BANK.Where(m => m.BILL_ID == bill_id).Any())
                                {
                                    //Check TXN code type-R-Reconcile (dont delete) U-unreconcile (delete details.)  1-Aug-2014  
                                    if (dbContext.ACC_TXN_BANK.Where(m => m.BILL_ID == bill_id).Select(s => s.TXN_TYPE_CODE).FirstOrDefault() == "R")
                                    {
                                        return "-999";
                                    }
                                    else
                                    {
                                        dbContext.Database.ExecuteSqlCommand("DELETE [omms].ACC_TXN_BANK Where BILL_ID = {0}", bill_id);
                                    }
                                }
                                //delete the master table entry
                                dbContext.ACC_BILL_MASTER.Remove(master);
                                // dbContext.SaveChanges();
                            }
                        }

                        dbContext.SaveChanges();
                        scope.Complete();

                        return "1";
                    }

                }
                else
                {
                    return result;
                }



            }
            catch (DbUpdateException ex)
            {
                ErrorLog.LogError(ex, "DefinalizationDAL.DeleteVoucher(DbUpdateException ex)");
                return ("An Update Error Occurred While Processing Your Request.");
            }
            catch (OptimisticConcurrencyException ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "DefinalizationDAL.DeleteVoucher(OptimisticConcurrencyException ex)");
                return ("An Error Occurred While Processing Your Request.");
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "DefinalizationDAL.DeleteVoucher()");
                throw new Exception("Error while deleting the voucher");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// DAL function to validate voucher before deleting fianlizing it
        /// </summary>
        /// <param name="bill_id"></param>
        /// <returns></returns>
        public String ValidateVoucher(long bill_id)
        {
            PMGSYEntities loacalDbContext = new PMGSYEntities();
            try
            {


                //check if already acknowledged
                ACC_BILL_MASTER master = new ACC_BILL_MASTER();

                master = loacalDbContext.ACC_BILL_MASTER.SingleOrDefault(c => c.BILL_ID == bill_id);

                //cheque if finalized
                if (master.BILL_FINALIZED == "N")
                {
                    return "-555"; //return not finalized
                }
                else if (master.ACC_CHEQUES_ISSUED != null ? master.ACC_CHEQUES_ISSUED.IS_CHQ_ENCASHED_NA == true : 1 != 1)
                {
                    return "-111";  //acknowledged
                }
                else if (loacalDbContext.ACC_RPT_MONTHWISE_SUMMARY.Where(x => x.ADMIN_ND_CODE == master.ADMIN_ND_CODE
                    && x.ACC_MONTH == master.BILL_MONTH && x.ACC_YEAR == master.BILL_YEAR && x.FUND_TYPE == PMGSYSession.Current.FundType).Any())
                {
                    return "-222";  //month is closed
                }
                else if (master.BILL_TYPE == "O" && loacalDbContext.ACC_BILL_MASTER.Where(c => c.ADMIN_ND_CODE == master.ADMIN_ND_CODE && c.BILL_TYPE != "O" && c.FUND_TYPE == PMGSYSession.Current.FundType).Any())
                {
                    if (!(PMGSYSession.Current.RoleCode == 21))
                    {
                        return "-333";// receipt, payment, TEO voucher is entered 
                    }
                }
                else if (master.BILL_TYPE == "P" && master.CHQ_EPAY == "E" && loacalDbContext.ACC_EPAY_MAIL_MASTER.Where(c => c.BILL_ID == master.BILL_ID).Any())
                {
                    return "-444"; //Epayment is done

                }
                //check if imprest voucher entry and already settled dont allow its definalization
                else if (dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Where(x => x.P_BILL_ID == bill_id).Select(x => x.S_BIll_ID).Any())
                {
                    return "-123";
                }
                //if it is the part of cancellation entry dont allow definalization and deletion
                else if (dbContext.ACC_BILL_MASTER.Where(x => x.BILL_ID == bill_id && x.CHQ_EPAY == "X").Any())
                {
                    return "-1000";
                }
                else if (master.BILL_TYPE == "P" && dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Where(m => m.P_BILL_ID == bill_id).Select(m => m.S_BIll_ID).Any())
                {
                    return "-888"; //Settlement is already present
                }
                else if (master.BILL_TYPE == "R")
                { //check if balance is going to negative due to cancellation of this receipt

                    TransactionParams objparams = new TransactionParams();

                    //objparams.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;

                    //objparams.FUND_TYPE = PMGSYSession.Current.FundType;

                    //objparams.MONTH = master.BILL_MONTH;

                    //objparams.YEAR = Convert.ToInt16(master.BILL_YEAR);

                    //objparams.LVL_ID = PMGSYSession.Current.LevelId;


                    //change done by Vikram on 24 March 2014
                    //for both srrda and dpiu the parameters added were of srrda so the validation was failed.
                    //now the validation is applied according to the selected parameters ( the transaction params are selected from bill master of bill id passed to this function).
                    objparams.ADMIN_ND_CODE = master.ADMIN_ND_CODE;

                    objparams.FUND_TYPE = PMGSYSession.Current.FundType;

                    objparams.MONTH = master.BILL_MONTH;

                    objparams.YEAR = Convert.ToInt16(master.BILL_YEAR);

                    objparams.LVL_ID = master.LVL_ID;

                    //commented on 05-05-2023
                    //UDF_ACC_GEN_GET_BA_CASH_CLOSING_BALANCES_Result balance = new UDF_ACC_GEN_GET_BA_CASH_CLOSING_BALANCES_Result();
                    //Added on 05-05-2023
                    UDF_ACC_GEN_GET_BA_CASH_CLOSING_BALANCES_COMPOSITE_Result balance = new UDF_ACC_GEN_GET_BA_CASH_CLOSING_BALANCES_COMPOSITE_Result();

                    PaymentBAL objPaymentBAL = new PaymentBAL();

                    balance = objPaymentBAL.GetCloSingBalanceForPayment(objparams);

                    //Commented by Abhishek kamble 5-June-2014
                    //check Bank authorization
                    //if (master.CHQ_EPAY == "Q" && ( (balance.bank_auth - master.CHQ_AMOUNT) < 0) )
                    //{
                    //        return "-666"; //negative authorization balance
                    //} else if
                    if (master.CHQ_EPAY == "C" && ((balance.cash - master.CASH_AMOUNT) < 0))
                    {

                        return "-777"; //negative Cash balance
                    }
                    else
                    {
                        return "1";//valid
                    }

                   
                }
                else
                {
                    return "1"; //OK
                }
                return "1";
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error while deleting the voucher");
            }
            finally
            {
                loacalDbContext.Dispose();
            }


        }

        /// <summary>
        /// function to check whether asset details for the payment is available 
        /// return 1 if yes, 0 if no
        /// </summary>
        /// <param name="bill_id"></param>
        /// <returns></returns>
        public String CheckIfAssetPayment(long bill_id)
        {
            try
            {
                dbContext = new PMGSYEntities();

                if (dbContext.ACC_ASSET_DETAILS.Any(c => c.BILL_ID == bill_id))
                {
                    return "1";
                }
                else
                {
                    return "0";
                }

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error while getting the voucher details");
            }
            finally
            {
                dbContext.Dispose();
            }



        }


    }

    interface IDefinalizationDAL
    {
        Array ListVoucherListtDetails(VoucherFilterModel objFilter, out long totalRecords);
        String DefinalizeVoucher(long bill_id);
        String DeleteVoucher(long bill_id);
        String ValidateVoucher(long bill_id);
        Array ListVoucherTransactionDetails(VoucherFilterModel objFilter, out long totalRecords);
        String CheckIfAssetPayment(long bill_id);
    }
}