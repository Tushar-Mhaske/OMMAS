using PMGSY.Common;
using PMGSY.Extensions;
using PMGSY.Models;
using PMGSY.Models.Authorization;
using PMGSY.Models.Common;
using PMGSY.Models.PaymentModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.DAL.Authorization
{
    public class AuthorizationDAL : IAuthorizationDAL
    {
        PMGSYEntities dbContext = null;
        CommonFunctions commonFuncObj = null;

        public Array AuthorizationRequestList(AuthorizationFilter objFilter, out long totalRecords)
        {
            try
            {
                dbContext = new PMGSYEntities();


                List<SP_ACC_BAR_LIST_DETAILS_Result> lstAuthRequestMaster = dbContext.SP_ACC_BAR_LIST_DETAILS(objFilter.FundType, objFilter.AdminNdCode, objFilter.Month, objFilter.Year).ToList<SP_ACC_BAR_LIST_DETAILS_Result>();
                commonFuncObj = new CommonFunctions();
                //foreach (var item in lstAuthRequestMaster)
                //{
                //    if(commonFuncObj.GetStringToDateTime(item.Auth_Date)
                //}

                /*  List<AuthorizationRequestListModel> lstAuthRequestMaster = null;
                  lstAuthRequestMaster = (
                                         from am in dbContext.ACC_AUTH_REQUEST_MASTER
                                        join ad in dbContext.ACC_AUTH_REQUEST_DETAILS on am.AUTH_ID equals ad.AUTH_ID
                                        from sp in dbContext.IMS_SANCTIONED_PROJECTS.Where(m=>m.IMS_PR_ROAD_CODE == ad.IMS_PR_ROAD_CODE).DefaultIfEmpty()
                                        from bd in dbContext.ACC_BILL_DETAILS.Where(m => m.ACC_BILL_MASTER.BILL_FINALIZED == "Y" && m.IMS_PR_ROAD_CODE == sp.IMS_PR_ROAD_CODE && m.MAST_CON_ID == am.MAST_CON_ID && m.CREDIT_DEBIT == "D").DefaultIfEmpty() // equals bd.IMS_PR_ROAD_CODE 
                                        from cb in dbContext.MASTER_CONTRACTOR_BANK.Where(m=>m.MAST_CON_ID == am.MAST_CON_ID && m.MAST_ACCOUNT_STATUS == "A").DefaultIfEmpty()                                      
                                      
                                        where
                                               am.ADMIN_ND_CODE == objFilter.AdminNdCode &&
                                               (objFilter.Month == 0 ? 1 : am.AUTH_MONTH) == (objFilter.Month == 0 ? 1 : objFilter.Month) && 
                                               (objFilter.Year == 0 ? 1 : am.AUTH_YEAR) == (objFilter.Year == 0 ? 1 : objFilter.Year) &&
                                               am.AUTH_FINALIZED == "Y" && am.CURRENT_AUTH_STATUS == "F"
                                       group bd.AMOUNT 
                                       by new {
                                              am.AUTH_ID,
                                              am.AUTH_NO,
                                              am.AUTH_DATE,
                                              ad.TEND_AGREEMENT_MASTER.TEND_AGREEMENT_NUMBER,
                                              sp.IMS_PACKAGE_ID,
                                              sp.IMS_ROAD_NAME,
                                              am.MASTER_CONTRACTOR.MAST_CON_COMPANY_NAME,
                                              cb.MAST_BANK_NAME,
                                              cb.MAST_ACCOUNT_NUMBER,
                                              sp.IMS_SANCTIONED_CD_AMT, sp.IMS_SANCTIONED_OW_AMT, sp.IMS_SANCTIONED_PAV_AMT, sp.IMS_SANCTIONED_BW_AMT, sp.IMS_SANCTIONED_BS_AMT, sp.IMS_SANCTIONED_RS_AMT,                                           
                                              am.GROSS_AMOUNT
                                       } into grp
                                        select new AuthorizationRequestListModel {
                                            AuthId = grp.Key.AUTH_ID,
                                            AuthorizationNumber = grp.Key.AUTH_NO,
                                            AuthorizationDate = grp.Key.AUTH_DATE,
                                            AgreementCode = grp.Key.TEND_AGREEMENT_NUMBER,
                                            Package = grp.Key.IMS_PACKAGE_ID,
                                            RoadName = grp.Key.IMS_ROAD_NAME,
                                            ContractorName = grp.Key.MAST_CON_COMPANY_NAME,
                                            BankName = grp.Key.MAST_BANK_NAME,
                                            BankAccountNo = grp.Key.MAST_ACCOUNT_NUMBER,
                                            SanctionedAmount = (grp.Key.IMS_SANCTIONED_CD_AMT==null ? 0 : grp.Key.IMS_SANCTIONED_CD_AMT) + (grp.Key.IMS_SANCTIONED_OW_AMT == null ? 0 : grp.Key.IMS_SANCTIONED_OW_AMT) + (grp.Key.IMS_SANCTIONED_PAV_AMT ==null ? 0 : grp.Key.IMS_SANCTIONED_PAV_AMT) +
                                            (grp.Key.IMS_SANCTIONED_BW_AMT == null ? 0 : grp.Key.IMS_SANCTIONED_BW_AMT) + (grp.Key.IMS_SANCTIONED_BS_AMT == null ? 0 : grp.Key.IMS_SANCTIONED_BS_AMT) + (grp.Key.IMS_SANCTIONED_RS_AMT == null ? 0 : grp.Key.IMS_SANCTIONED_RS_AMT),
                                            ExpenditureAmount = grp.Sum(),
                                            PayableAmount = grp.Key.GROSS_AMOUNT
                                        }).ToList<AuthorizationRequestListModel>();                                        
                */
                totalRecords = lstAuthRequestMaster.Count();

                if (objFilter.sidx.Trim() != string.Empty)
                {
                    if (objFilter.sord.ToString() == "asc")
                    {
                        switch (objFilter.sidx)
                        {
                            case "AuthNumber":
                                lstAuthRequestMaster = lstAuthRequestMaster.OrderBy(x => x.Auth_No).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<SP_ACC_BAR_LIST_DETAILS_Result>();
                                break;
                            case "AuthDate":
                                lstAuthRequestMaster = lstAuthRequestMaster.OrderBy(x => x.Auth_Date).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<SP_ACC_BAR_LIST_DETAILS_Result>();
                                break;
                            case "AggNumber":
                                lstAuthRequestMaster = lstAuthRequestMaster.OrderBy(x => x.Agreement_Number).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<SP_ACC_BAR_LIST_DETAILS_Result>();
                                break;
                            case "Package":
                                lstAuthRequestMaster = lstAuthRequestMaster.OrderBy(x => x.Package).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<SP_ACC_BAR_LIST_DETAILS_Result>();
                                break;
                            case "RoadName":
                                lstAuthRequestMaster = lstAuthRequestMaster.OrderBy(x => x.Work_Name).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<SP_ACC_BAR_LIST_DETAILS_Result>();
                                break;
                            case "PayeeName":
                                lstAuthRequestMaster = lstAuthRequestMaster.OrderBy(x => x.Payee_Name).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<SP_ACC_BAR_LIST_DETAILS_Result>();
                                break;
                            default:
                                lstAuthRequestMaster = lstAuthRequestMaster.OrderBy(x => x.Auth_Date).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<SP_ACC_BAR_LIST_DETAILS_Result>();
                                break;
                        }

                    }
                    else
                    {
                        switch (objFilter.sidx)
                        {
                            case "AuthNumber":
                                lstAuthRequestMaster = lstAuthRequestMaster.OrderByDescending(x => x.Auth_No).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<SP_ACC_BAR_LIST_DETAILS_Result>();
                                break;
                            case "AuthDate":
                                lstAuthRequestMaster = lstAuthRequestMaster.OrderByDescending(x => x.Auth_Date).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<SP_ACC_BAR_LIST_DETAILS_Result>();
                                break;
                            case "AggNumber":
                                lstAuthRequestMaster = lstAuthRequestMaster.OrderByDescending(x => x.Agreement_Number).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<SP_ACC_BAR_LIST_DETAILS_Result>();
                                break;
                            case "Package":
                                lstAuthRequestMaster = lstAuthRequestMaster.OrderByDescending(x => x.Package).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<SP_ACC_BAR_LIST_DETAILS_Result>();
                                break;
                            case "RoadName":
                                lstAuthRequestMaster = lstAuthRequestMaster.OrderByDescending(x => x.Work_Name).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<SP_ACC_BAR_LIST_DETAILS_Result>();
                                break;
                            case "PayeeName":
                                lstAuthRequestMaster = lstAuthRequestMaster.OrderByDescending(x => x.Payee_Name).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<SP_ACC_BAR_LIST_DETAILS_Result>();
                                break;
                            default:
                                lstAuthRequestMaster = lstAuthRequestMaster.OrderByDescending(x => x.Auth_Date).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<SP_ACC_BAR_LIST_DETAILS_Result>();
                                break;
                        }
                    }
                }
                else
                {
                    lstAuthRequestMaster = lstAuthRequestMaster.OrderByDescending(x => x.Auth_No).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<SP_ACC_BAR_LIST_DETAILS_Result>();
                }

                /* foreach (SP_ACC_BAR_LIST_DETAILS_Result item in lstAuthRequestMaster)
                 {
                     if (item.EncAuthId == null)
                     {
                         String encAuthId = URLEncrypt.EncryptParameters(new string[] { item.AuthId.ToString().Trim() });

                         foreach (SP_ACC_BAR_LIST_DETAILS_Result itr in lstAuthRequestMaster)
                         {
                             if (item.AuthId == itr.AuthId && itr.EncAuthId == null)
                             {
                                 itr.EncAuthId = encAuthId;
                             }
                         }
                     }
                     //if(item.EncAuthId == null && lstAuthRequestMaster.Where(m=>m.AuthId == item.AuthId && m.EncAuthId == null).Any())
                     //{
                     //    item.EncAuthId = URLEncrypt.EncryptParameters(new string[] { item.AuthId.ToString().Trim() });
                     //    lstAuthRequestMaster.Where(m => m.AuthId == item.AuthId && m.EncAuthId == null).Select(m => m.EncAuthId = item.EncAuthId);
                     //}
                     //lstAuthRequestMaster.Where(m => m.AuthId == item.AuthId && m.EncAuthId == null).Select(m => m.EncAuthId = item.EncAuthId);
                 }
                 */
                return lstAuthRequestMaster.Select(item => new
                {

                    id = URLEncrypt.EncryptParameters(new string[] { item.Auth_ID.ToString().Trim() }),
                    cell = new[] {                         
                                    
                                    item.Auth_No,
                                    item.Auth_Date,
                                    item.Txn_Name.ToString(),
                                    item.Agreement_Number == null ? "0" : item.Agreement_Number.ToString(),
                                    item.Package,
                                    item.Work_Name,
                                    item.Payee_Name,
                                    item.Bank_Name ,
                                    item.Account_No,
                                    item.Sanctioned_Amt == null ? "0" : item.Sanctioned_Amt.ToString(),
                                    item.Expn_Amt == null ? "0" : item.Expn_Amt.ToString(),
                                    item.Payable_Amt.ToString()
                   }
                }).ToArray();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
           
                totalRecords = 0;
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public String AddRequestTrackingDetails(ListAutorizationRequestModel model)
        {
            string fundType = PMGSYSession.Current.FundType;
            int adminNdCode = PMGSYSession.Current.AdminNdCode;
            short levelID = PMGSYSession.Current.LevelId;

            using (var scope = new TransactionScope())
            {
                try
                {
                    dbContext = new PMGSYEntities();
                    List<Int64> authIds = new List<Int64>();
                    model.AUTH_STATUS = model.REQUEST_ID_LIST.Split('_')[0];

                    if (model.REQUEST_ID_LIST.Split('_')[1] == "Y")
                    {
                        authIds = dbContext.ACC_AUTH_REQUEST_MASTER.Where(m => m.ADMIN_ND_CODE == model.ADMIN_ND_CODE && (model.AUTH_MONTH == 0 ? 1 : m.AUTH_MONTH) == (model.AUTH_MONTH == 0 ? 1 : model.AUTH_MONTH) &&
                                             (model.AUTH_YEAR == 0 ? 1 : m.AUTH_YEAR) == (model.AUTH_YEAR == 0 ? 1 : model.AUTH_YEAR) &&
                                             m.AUTH_FINALIZED == "Y" && m.CURRENT_AUTH_STATUS == "F").Select(m => m.AUTH_ID).Distinct().ToList<Int64>();
                    }
                    else
                    {
                        String[] strRequestDetails = model.REQUEST_ID_LIST.Split('_')[2].Split(',');
                        String[] id = new String[1];
                        foreach (String item in strRequestDetails)
                        {
                            id = URLEncrypt.DecryptParameters(new string[] { item.Split('/')[0], item.Split('/')[1], item.Split('/')[2] });
                            authIds.Add(Convert.ToInt64(id[0]));
                        }
                        authIds = authIds.Distinct().ToList<Int64>();
                    }

                    commonFuncObj = new CommonFunctions();
                    foreach (Int64 item in authIds)
                    {

                        // Change done by shyam for validation on date
                        // Check for reject/approve bank authorization date
                        // Reject/Approve Date should be greater than or equal to authorization date
                        DateTime authDate = (dbContext.ACC_AUTH_REQUEST_MASTER.Where(c => c.AUTH_ID == item).Select(c => c.AUTH_DATE).FirstOrDefault());
                        DateTime Date_Of_Operation = commonFuncObj.GetStringToDateTime(model.DATE_OF_OPERATION);
                        if (authDate > Date_Of_Operation)
                        {
                            return "Date of Operation should be greater or equal to Bank Authorization Date";
                        }



                        ACC_AUTH_REQUEST_TRACKING request_tracking = new ACC_AUTH_REQUEST_TRACKING();
                        request_tracking.REQUEST_ID = dbContext.ACC_AUTH_REQUEST_TRACKING.Max(m => m.REQUEST_ID) + 1;
                        request_tracking.AUTH_ID = item;
                        request_tracking.AUTH_STATUS = model.AUTH_STATUS;
                        request_tracking.DATE_OF_OPERATION = Date_Of_Operation;
                        request_tracking.REMARKS = model.REMARKS;

                        //added by Abhishek kamble 29-nov-2013
                        request_tracking.USERID = PMGSYSession.Current.UserId;
                        request_tracking.IPADD=HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                        dbContext.ACC_AUTH_REQUEST_TRACKING.Add(request_tracking);
                        dbContext.SaveChanges();
                        ACC_AUTH_REQUEST_MASTER request_master = new ACC_AUTH_REQUEST_MASTER();
                        request_master = dbContext.ACC_AUTH_REQUEST_MASTER.Find(request_tracking.AUTH_ID);
                        request_master.CURRENT_AUTH_STATUS = model.AUTH_STATUS;

                        //added by Abhishek kamble 29-nov-2013
                        request_master.USERID = PMGSYSession.Current.UserId;
                        request_master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                        dbContext.Entry(request_master).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();

                        //added by Koustubh Nakate on 21/08/2013 to save notification in notification details table 
                        var result = dbContext.USP_ACC_INSERT_ALERT_DETAILS(adminNdCode, fundType, "A", levelID, item,PMGSYSession.Current.UserId,HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]);
                    }
                }
                catch (TransactionException ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);

                    string status = ex.Message;
                    return String.Empty;
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                       return ex.Message;
                }
              
                finally
                {
                    scope.Complete();
                    dbContext.Dispose();
                }
            }
            return String.Empty;
        }

        public ACC_AUTH_REQUEST_MASTER GetAuthorizationRequestMaster(Int64 authId)
        {
            try
            {
                dbContext = new PMGSYEntities();
                return dbContext.ACC_AUTH_REQUEST_MASTER.Where(m => m.AUTH_ID == authId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
               
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public String AddReceiptDetails(ReceiptDetailsModel receiptModel, ref string message)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    dbContext = new PMGSYEntities();
                    commonFuncObj = new CommonFunctions();
                    short year = (Int16)commonFuncObj.GetStringToDateTime(receiptModel.BILL_DATE).Year;
                    short month = (Int16)commonFuncObj.GetStringToDateTime(receiptModel.BILL_DATE).Month;

                    ////added by koustubh nakate on 22/07/2013 for check unique receipt number
                    //if (dbContext.ACC_BILL_MASTER.Any(bm => bm.BILL_NO.ToUpper() == receiptModel.BILL_NO.ToUpper() && bm.BILL_YEAR == year && bm.BILL_MONTH == month && bm.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode))
                    //{
                    //    message = "Receipt Number in selected month and year is already exist.";
                    //    return string.Empty;
                    //}


                    ACC_BILL_MASTER acc_bill_master = new ACC_BILL_MASTER();
                    ACC_AUTH_REQUEST_MASTER request_master = new ACC_AUTH_REQUEST_MASTER();
                    request_master = dbContext.ACC_AUTH_REQUEST_MASTER.Where(m => m.AUTH_ID == receiptModel.AUTH_ID).FirstOrDefault();
                    acc_bill_master.BILL_ID = dbContext.ACC_BILL_MASTER.Any() ? dbContext.ACC_BILL_MASTER.Max(m => m.BILL_ID) + 1 : 1;
                    acc_bill_master.BILL_NO = receiptModel.BILL_NO;
                    acc_bill_master.BILL_DATE = commonFuncObj.GetStringToDateTime(receiptModel.BILL_DATE);
                    acc_bill_master.BILL_MONTH = Convert.ToInt16(acc_bill_master.BILL_DATE.Month);
                    acc_bill_master.BILL_YEAR = Convert.ToInt16(acc_bill_master.BILL_DATE.Year);
                    acc_bill_master.TXN_ID = 12;
                    acc_bill_master.CHQ_AMOUNT = request_master.CHQ_AMOUNT;
                    acc_bill_master.GROSS_AMOUNT = request_master.CHQ_AMOUNT;
                    acc_bill_master.PAYEE_NAME = request_master.MASTER_CONTRACTOR.MAST_CON_COMPANY_NAME;
                    acc_bill_master.CHQ_EPAY = "B";
                    acc_bill_master.BILL_FINALIZED = "Y";
                    acc_bill_master.BILL_TYPE = "R";
                    acc_bill_master.FUND_TYPE = request_master.FUND_TYPE;
                    acc_bill_master.LVL_ID = (byte)PMGSYSession.Current.LevelId;
                    acc_bill_master.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                    acc_bill_master.MAST_CON_ID = request_master.MAST_CON_ID;

                    //added by Abhishek kamble 29-nov-2013
                    acc_bill_master.USERID = PMGSYSession.Current.UserId;
                    acc_bill_master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];


                    //Added By Abhishek kamble to set Chq_No to Auth Number for refference. 3 Sep 2014
                    acc_bill_master.CHQ_NO = request_master.AUTH_NO;

                    dbContext.ACC_BILL_MASTER.Add(acc_bill_master);
                    dbContext.SaveChanges();

                    ACC_BILL_DETAILS credit_acc_bill_details = new ACC_BILL_DETAILS();
                    credit_acc_bill_details.BILL_ID = acc_bill_master.BILL_ID;
                    credit_acc_bill_details.TXN_NO = 1;
                    credit_acc_bill_details.TXN_ID = 13;
                    credit_acc_bill_details.HEAD_ID = dbContext.ACC_TXN_HEAD_MAPPING.Where(m => m.TXN_ID == credit_acc_bill_details.TXN_ID && m.CREDIT_DEBIT == "C" && m.CASH_CHQ == "Q").Select(m => m.HEAD_ID).FirstOrDefault();
                    credit_acc_bill_details.AMOUNT = acc_bill_master.GROSS_AMOUNT;
                    credit_acc_bill_details.CREDIT_DEBIT = "C";
                    credit_acc_bill_details.CASH_CHQ = "Q";
                    credit_acc_bill_details.NARRATION = dbContext.ACC_MASTER_TXN.Where(m => m.TXN_ID == credit_acc_bill_details.TXN_ID).Select(m => m.TXN_DESC).FirstOrDefault();

                    //added by Abhishek kamble 29-nov-2013
                    credit_acc_bill_details.USERID = PMGSYSession.Current.UserId;
                    credit_acc_bill_details.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    dbContext.ACC_BILL_DETAILS.Add(credit_acc_bill_details);
                    dbContext.SaveChanges();

                    ACC_BILL_DETAILS debit_acc_bill_details = new ACC_BILL_DETAILS();
                    debit_acc_bill_details.BILL_ID = acc_bill_master.BILL_ID;
                    debit_acc_bill_details.TXN_NO = 2;
                    debit_acc_bill_details.TXN_ID = 13;
                    debit_acc_bill_details.HEAD_ID = dbContext.ACC_TXN_HEAD_MAPPING.Where(m => m.TXN_ID == debit_acc_bill_details.TXN_ID && m.CREDIT_DEBIT == "D" && m.CASH_CHQ == "Q").Select(m => m.HEAD_ID).FirstOrDefault();
                    debit_acc_bill_details.AMOUNT = acc_bill_master.GROSS_AMOUNT;
                    debit_acc_bill_details.CREDIT_DEBIT = "D";
                    debit_acc_bill_details.CASH_CHQ = "Q";
                    debit_acc_bill_details.NARRATION = credit_acc_bill_details.NARRATION;

                    //added by Abhishek kamble 29-nov-2013
                    debit_acc_bill_details.USERID = PMGSYSession.Current.UserId;
                    debit_acc_bill_details.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    dbContext.ACC_BILL_DETAILS.Add(debit_acc_bill_details);
                    dbContext.SaveChanges();

                    request_master.CURRENT_AUTH_STATUS = "R";
                    dbContext.Entry(request_master).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    ACC_AUTH_REQUEST_TRACKING request_tracking = new ACC_AUTH_REQUEST_TRACKING();
                    request_tracking.REQUEST_ID = dbContext.ACC_AUTH_REQUEST_TRACKING.Any() ? dbContext.ACC_AUTH_REQUEST_TRACKING.Max(m => m.REQUEST_ID) + 1 : 1;
                    request_tracking.AUTH_ID = request_master.AUTH_ID;
                    request_tracking.AUTH_STATUS = "R";
                    request_tracking.DATE_OF_OPERATION = acc_bill_master.BILL_DATE;
                    request_tracking.REMARKS = "Receipt Entered";
                    request_tracking.BILL_ID = acc_bill_master.BILL_ID;

                    //added by Abhishek kamble 29-nov-2013
                    request_tracking.USERID = PMGSYSession.Current.UserId;
                    request_tracking.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];


                    dbContext.ACC_AUTH_REQUEST_TRACKING.Add(request_tracking);
                    dbContext.SaveChanges();

                    return acc_bill_master.BILL_ID.ToString();
                }
                catch (TransactionException ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                   
                    string status = ex.Message;
                    return String.Empty;
                }
                catch (Exception Ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                    string status = Ex.Message;
                    return String.Empty;
                }
                finally
                {
                    scope.Complete();
                    dbContext.Dispose();
                }
            }
        }

        public String AddPaymentDetails(PMGSY.Models.Authorization.PaymentDetailsModel paymentModel)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    dbContext = new PMGSYEntities();
                    commonFuncObj = new CommonFunctions();
                    ACC_BILL_MASTER acc_bill_master = new ACC_BILL_MASTER();
                    ACC_AUTH_REQUEST_MASTER request_master = new ACC_AUTH_REQUEST_MASTER();
                    request_master = dbContext.ACC_AUTH_REQUEST_MASTER.Where(m => m.AUTH_ID == paymentModel.AUTH_ID).FirstOrDefault();
                    acc_bill_master.BILL_ID = dbContext.ACC_BILL_MASTER.Any() ? dbContext.ACC_BILL_MASTER.Max(m => m.BILL_ID) + 1 : 1;
                    acc_bill_master.BILL_NO = paymentModel.BILL_NO;
                    acc_bill_master.BILL_DATE = commonFuncObj.GetStringToDateTime(paymentModel.BILL_DATE);
                    acc_bill_master.BILL_MONTH = Convert.ToInt16(acc_bill_master.BILL_DATE.Month);
                    acc_bill_master.BILL_YEAR = Convert.ToInt16(acc_bill_master.BILL_DATE.Year);
                    acc_bill_master.TXN_ID = request_master.TXN_ID;
                    acc_bill_master.CHQ_AMOUNT = request_master.CHQ_AMOUNT;
                    acc_bill_master.CASH_AMOUNT = request_master.CASH_AMOUNT;
                    acc_bill_master.GROSS_AMOUNT = request_master.GROSS_AMOUNT;
                    acc_bill_master.PAYEE_NAME = request_master.MASTER_CONTRACTOR.MAST_CON_COMPANY_NAME;
                    acc_bill_master.CHQ_EPAY = "B";
                    acc_bill_master.BILL_FINALIZED = "Y";
                    acc_bill_master.BILL_TYPE = "P";
                    acc_bill_master.FUND_TYPE = request_master.FUND_TYPE;
                    acc_bill_master.LVL_ID = (byte)PMGSYSession.Current.LevelId;
                    acc_bill_master.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                    acc_bill_master.MAST_CON_ID = request_master.MAST_CON_ID;

                    //added by Abhishek kamble 29-nov-2013
                    acc_bill_master.USERID = PMGSYSession.Current.UserId;
                    acc_bill_master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    //Added By Abhishek kamble to set Chq_No to Auth Number for refference. 3 Sep 2014
                    acc_bill_master.CHQ_NO = request_master.AUTH_NO;   

                    dbContext.ACC_BILL_MASTER.Add(acc_bill_master);
                    dbContext.SaveChanges();

                    List<ACC_AUTH_REQUEST_DETAILS> request_details = new List<ACC_AUTH_REQUEST_DETAILS>();

                    request_details = dbContext.ACC_AUTH_REQUEST_DETAILS.Where(m => m.AUTH_ID == paymentModel.AUTH_ID).OrderBy(m => m.TXN_NO).ToList<ACC_AUTH_REQUEST_DETAILS>();
                    foreach (ACC_AUTH_REQUEST_DETAILS item in request_details)
                    {
                        ACC_BILL_DETAILS acc_bill_details = new ACC_BILL_DETAILS();
                        acc_bill_details.BILL_ID = acc_bill_master.BILL_ID;
                        acc_bill_details.TXN_NO = item.TXN_NO;
                        acc_bill_details.TXN_ID = item.TXN_ID;
                        acc_bill_details.HEAD_ID = item.HEAD_ID;
                        acc_bill_details.AMOUNT = item.AMOUNT;
                        acc_bill_details.CREDIT_DEBIT = item.CREDIT_DEBIT;
                        acc_bill_details.CASH_CHQ = item.CASH_CHQ;
                        acc_bill_details.NARRATION = item.NARRATION;
                        acc_bill_details.MAST_CON_ID = acc_bill_master.MAST_CON_ID;
                        acc_bill_details.IMS_PR_ROAD_CODE = item.IMS_PR_ROAD_CODE;
                        acc_bill_details.IMS_AGREEMENT_CODE = item.IMS_AGREEMENT_CODE;

                        //Added By Abhishek kamble to set FINAL_PAYMENT and MAS_FA_CODE for refference. 4 Sep 2014                        
                        acc_bill_details.FINAL_PAYMENT = item.FINAL_PAYMENT;
                        acc_bill_details.MAS_FA_CODE = item.MAS_FA_CODE;

                        //added by Abhishek kamble 29-nov-2013
                        acc_bill_details.USERID = PMGSYSession.Current.UserId;
                        acc_bill_details.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                        dbContext.ACC_BILL_DETAILS.Add(acc_bill_details);
                        dbContext.SaveChanges();
                    }

                    request_master.CURRENT_AUTH_STATUS = "P";

                    //added by Abhishek kamble 29-nov-2013
                    request_master.USERID = PMGSYSession.Current.UserId;
                    request_master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    dbContext.Entry(request_master).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    ACC_AUTH_REQUEST_TRACKING request_tracking = new ACC_AUTH_REQUEST_TRACKING();
                    request_tracking.REQUEST_ID = dbContext.ACC_AUTH_REQUEST_TRACKING.Any() ? dbContext.ACC_AUTH_REQUEST_TRACKING.Max(m => m.REQUEST_ID) + 1 : 1;
                    request_tracking.AUTH_ID = request_master.AUTH_ID;
                    request_tracking.AUTH_STATUS = "P";
                    request_tracking.DATE_OF_OPERATION = acc_bill_master.BILL_DATE;
                    request_tracking.REMARKS = "Payment Entered";
                    request_tracking.BILL_ID = acc_bill_master.BILL_ID;

                    //added by Abhishek kamble 29-nov-2013
                    request_tracking.USERID = PMGSYSession.Current.UserId;
                    request_tracking.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    dbContext.ACC_AUTH_REQUEST_TRACKING.Add(request_tracking);
                    dbContext.SaveChanges();

                    //Added By Abhishek kamble to Save Data in ACC_CHEQUE_ISSUED Table 24-sep-2014 start

                    ACC_CHEQUES_ISSUED chequeIssued = new ACC_CHEQUES_ISSUED();
                    chequeIssued.BILL_ID = acc_bill_master.BILL_ID;
                    chequeIssued.BANK_CODE = dbContext.ACC_BANK_DETAILS.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.ParentNDCode && x.FUND_TYPE == PMGSYSession.Current.FundType && x.BANK_ACC_STATUS == true && x.ACCOUNT_TYPE == "S").Select(z => z.BANK_CODE).FirstOrDefault();                    
                    chequeIssued.IS_CHQ_ENCASHED_NA = false;
                    chequeIssued.NA_BILL_ID = null;
                    chequeIssued.IS_CHQ_RECONCILE_BANK = false;
                    chequeIssued.CHQ_RECONCILE_DATE = null;
                    chequeIssued.CHQ_RECONCILE_REMARKS = null;
                    chequeIssued.CHEQUE_STATUS = "N";
                    chequeIssued.USERID = PMGSYSession.Current.UserId;
                    chequeIssued.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    dbContext.ACC_CHEQUES_ISSUED.Add(chequeIssued);
                    dbContext.SaveChanges();

                    //Added By Abhishek kamble to Save Data in ACC_CHEQUE_ISSUED Table 24-sep-2014 start
                    return String.Empty;
                }
                catch (TransactionException ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                   
                    return ex.Message;
                }
                catch (Exception Ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                    throw Ex;
                }
                finally
                {
                    scope.Complete();
                    dbContext.Dispose();
                }
            }
        }

        public ACC_BILL_MASTER GetBillMaster(Int64 billId)
        {
            try
            {
                dbContext = new PMGSYEntities();
                return dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == billId).FirstOrDefault();
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

        public ACC_AUTH_REQUEST_TRACKING GetAuthorizationTrackingDetails(Int64 authId, String authStatus)
        {
            try
            {
                dbContext = new PMGSYEntities();
                return dbContext.ACC_AUTH_REQUEST_TRACKING.Where(m => m.AUTH_ID == authId && m.AUTH_STATUS == authStatus).FirstOrDefault();
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

        /// <summary>
        /// function to list the authorization master details
        /// </summary>
        /// <param name="objFilter"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array ListAuthorizationRequestDetails(PaymentFilterModel objFilter, out long totalRecords)
        {
            CommonFunctions commomFuncObj = null;
            try
            {
                dbContext = new PMGSYEntities();

                commomFuncObj = new CommonFunctions();
                DateTime fromDate = DateTime.Now;
                DateTime toDate = DateTime.Now;

                if (objFilter.FromDate != String.Empty && objFilter.FromDate != null)
                {
                    fromDate = commomFuncObj.GetStringToDateTime(objFilter.FromDate);
                }

                if (objFilter.ToDate != String.Empty && objFilter.ToDate != null)
                {
                    toDate = commomFuncObj.GetStringToDateTime(objFilter.ToDate);
                }


                var query = from m in dbContext.ACC_AUTH_REQUEST_MASTER
                            where
                                     m.ADMIN_ND_CODE == objFilter.AdminNdCode
                                    && m.LVL_ID == objFilter.LevelId
                                    && m.FUND_TYPE == objFilter.FundType

                                   // && (objFilter.FromDate != null && !objFilter.FilterMode.Equals("view")) ? m.AUTH_DATE >= fromDate : 1 == 1
                                && ((objFilter.FromDate != null && !objFilter.FilterMode.Equals("view")) ? m.AUTH_DATE : DateTime.Now) >= ((objFilter.FromDate != null && !objFilter.FilterMode.Equals("view")) ? fromDate : DateTime.Now)
                                //  && (objFilter.ToDate != null && !objFilter.FilterMode.Equals("view")) ? m.AUTH_DATE <= toDate : 1 == 1
                                  && ((objFilter.ToDate != null && !objFilter.FilterMode.Equals("view")) ? m.AUTH_DATE : DateTime.Now) <= ((objFilter.ToDate != null && !objFilter.FilterMode.Equals("view")) ? toDate : DateTime.Now)

                                    //  && (objFilter.TransId != 0 && !objFilter.FilterMode.Equals("view")) ? m.TXN_ID == objFilter.TransId : 1 == 1
                                    && ((objFilter.TransId != 0 && !objFilter.FilterMode.Equals("view")) ? m.TXN_ID : 1) == ((objFilter.TransId != 0 && !objFilter.FilterMode.Equals("view")) ? objFilter.TransId : 1)
                                // && (objFilter.FilterMode.Equals("view")) ? m.AUTH_MONTH == objFilter.Month : 1 == 1
                                     && ((objFilter.FilterMode.Equals("view")) ? m.AUTH_MONTH : 1) == ((objFilter.FilterMode.Equals("view")) ? objFilter.Month : 1)
                                     && ((objFilter.FilterMode.Equals("view")) ? m.AUTH_YEAR : 1) == ((objFilter.FilterMode.Equals("view")) ? objFilter.Year : 1)
                            //&& (objFilter.FilterMode.Equals("view")) ? m.AUTH_YEAR == objFilter.Year : 1 == 1

                            select m;

                if (objFilter.AuthorizationStatus != String.Empty && !objFilter.FilterMode.Equals("view"))
                {
                    string[] AuthStatusArray = objFilter.AuthorizationStatus.Split('$');

                    query = query.Where(c => AuthStatusArray.Contains(c.CURRENT_AUTH_STATUS));

                }


                totalRecords = query.Count();

                if (objFilter.sidx.Trim() != string.Empty)
                {
                    if (objFilter.sord.ToString() == "asc")
                    {
                        switch (objFilter.sidx)
                        {
                            case "Auth_Number":
                                query = query.OrderBy(x => x.AUTH_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows));
                                break;
                            case "Auth_date":
                                query = query.OrderBy(x => x.AUTH_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows));
                                break;

                            case "CashAmount":
                                query = query.OrderBy(x => x.CASH_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows));
                                break;
                            case "ChequeAmount":
                                query = query.OrderBy(x => x.CHQ_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows));
                                break;
                            case "GrossAmount":
                                query = query.OrderBy(x => x.GROSS_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows));
                                break;

                            default:
                                query = query.OrderBy(x => x.AUTH_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows));
                                break;
                        }

                    }
                    else
                    {
                        switch (objFilter.sidx)
                        {
                            case "Auth_Number":
                                query = query.OrderByDescending(x => x.AUTH_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows));
                                break;
                            case "Auth_date":
                                query = query.OrderByDescending(x => x.AUTH_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows));
                                break;

                            case "CashAmount":
                                query = query.OrderByDescending(x => x.CASH_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows));
                                break;
                            case "ChequeAmount":
                                query = query.OrderByDescending(x => x.CHQ_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows));
                                break;
                            case "GrossAmount":
                                query = query.OrderByDescending(x => x.GROSS_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows));
                                break;

                            default:
                                query = query.OrderByDescending(x => x.AUTH_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows));
                                break;
                        }
                    }
                }
                else
                {
                    query = query.OrderByDescending(x => x.AUTH_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows));
                }

                var result = query.Select(m => new
                {

                    m.AUTH_ID,
                    m.AUTH_NO,
                    m.AUTH_MONTH,
                    m.AUTH_YEAR,
                    m.AUTH_DATE,
                    m.TXN_ID,
                    m.CASH_AMOUNT,
                    m.CHQ_AMOUNT,
                    m.GROSS_AMOUNT,
                    m.MAST_CON_ID,
                    m.AUTH_FINALIZED,
                    m.FUND_TYPE,
                    m.ADMIN_ND_CODE,
                    m.LVL_ID,
                    m.CURRENT_AUTH_STATUS,
                    m.ACC_MASTER_TXN.TXN_DESC

                }).ToArray();

                return result.Select(item => new
                {
                    id = URLEncrypt.EncryptParameters(new string[] { item.AUTH_ID.ToString().Trim() }),
                    cell = new[] {                         
                                    
                                        item.AUTH_NO.ToString(),
                                        commomFuncObj.GetDateTimeToString(item.AUTH_DATE),
                                        item.TXN_DESC.ToString(),
                                        item.CHQ_AMOUNT.ToString(),
                                        item.CASH_AMOUNT.ToString(),
                                        item.GROSS_AMOUNT.ToString(),
                                        GetAuthStatusInWords(item.CURRENT_AUTH_STATUS ==null ? " " : item.CURRENT_AUTH_STATUS),
                                        item.AUTH_FINALIZED=="N"? "<center><a href='#' class='ui-icon ui-icon-pencil' onclick='EditAuthorization(\"" +URLEncrypt.EncryptParameters(new string[] { item.AUTH_ID.ToString().Trim()})+ "\");return false;'>Edit</a></center>" : ""     ,
                                        item.AUTH_FINALIZED=="N"? "<center><a href='#' class='ui-icon ui-icon-trash' onclick='DeleteAuthorization(\"" +URLEncrypt.EncryptParameters(new string[] { item.AUTH_ID.ToString().Trim()})+ "\");return false;'>Delete</a></center>" : ""     ,
                                        item.AUTH_FINALIZED=="N" && CanAuthFinalize(item.AUTH_ID) ? "<center><a href='#' class='ui-icon ui-icon-unlocked' onclick='FinalizeAuthorization(\"" +URLEncrypt.EncryptParameters(new string[] { item.AUTH_ID.ToString().Trim()})+ "\");return false;'>Lock</a></center>" : item.AUTH_FINALIZED=="Y" ?  "<center><a href='#' class='ui-icon ui-icon-search' onclick='ViewAuthorization(\"" +URLEncrypt.EncryptParameters(new string[] { item.AUTH_ID.ToString().Trim()})+ "\");return false;'>View</a></center>":string.Empty,
                                       
                                        
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


        /// <summary>
        /// function to get auth status based on auth id
        /// </summary>
        /// <param name="auth_Status"></param>
        /// <returns></returns>
        public string GetAuthStatusInWords(String auth_Status)
        {
            String Status = string.Empty;

            switch (auth_Status)
            {

                case "F":
                    Status = "Forwarding request to SRRDA(EO)";
                    break;
                case "A":
                    Status = "Approval By Empowered Officer";
                    break;
                case "C":
                    Status = "Cancellation of request by Empowered Officer";
                    break;
                case "R":
                    Status = "Receipt of Authorization by PIU";
                    break;
                case "P":
                    Status = "Payment by PIU";
                    break;
                default:
                    Status = "";
                    break;
            }

            return Status;
        }



        /// <summary>
        /// DAL function to list authorization master details on data entry page
        /// </summary>
        /// <param name="objFilter"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array ListAuthorizationMasterDetails(PaymentFilterModel objFilter, out long totalRecords)
        {
            CommonFunctions commomFuncObj = null;
            try
            {
                dbContext = new PMGSYEntities();

                commomFuncObj = new CommonFunctions();



                var query = (from m in dbContext.ACC_AUTH_REQUEST_MASTER

                             where
                                    m.ADMIN_ND_CODE == objFilter.AdminNdCode
                                    && m.LVL_ID == objFilter.LevelId
                                    && m.FUND_TYPE == objFilter.FundType
                                    && m.AUTH_ID == objFilter.BillId



                             select new
                             {
                                 m.AUTH_ID,
                                 m.AUTH_NO,
                                 m.AUTH_MONTH,
                                 m.AUTH_YEAR,
                                 m.AUTH_DATE,
                                 m.TXN_ID,
                                 m.CASH_AMOUNT,
                                 m.CHQ_AMOUNT,
                                 m.GROSS_AMOUNT,
                                 m.MAST_CON_ID,
                                 m.AUTH_FINALIZED,
                                 m.FUND_TYPE,
                                 m.ADMIN_ND_CODE,
                                 m.LVL_ID,
                                 m.ACC_MASTER_TXN.TXN_DESC,

                             });





                totalRecords = query.Count();


                var result = query.Select(m => new
                {

                    m.AUTH_ID,
                    m.AUTH_NO,
                    m.AUTH_MONTH,
                    m.AUTH_YEAR,
                    m.AUTH_DATE,
                    m.TXN_ID,
                    m.CASH_AMOUNT,
                    m.CHQ_AMOUNT,
                    m.GROSS_AMOUNT,
                    m.MAST_CON_ID,
                    m.AUTH_FINALIZED,
                    m.FUND_TYPE,
                    m.ADMIN_ND_CODE,
                    m.LVL_ID,
                    m.TXN_DESC

                }).ToArray();

                return result.Select(item => new
                {
                    id = URLEncrypt.EncryptParameters(new string[] { item.AUTH_ID.ToString().Trim() }),
                    cell = new[] {                         
                                    
                                        item.AUTH_NO.ToString(),
                                        commomFuncObj.GetDateTimeToString(item.AUTH_DATE),
                                        item.TXN_DESC.ToString(),
                                        item.MAST_CON_ID !=0 ? dbContext.MASTER_CONTRACTOR.Where(c => c.MAST_CON_ID == item.MAST_CON_ID).Select(d => d.MAST_CON_COMPANY_NAME).FirstOrDefault() :String.Empty,
                                        item.CHQ_AMOUNT.ToString(),
                                        item.CASH_AMOUNT.ToString(),
                                        item.GROSS_AMOUNT.ToString(),
                                                            
                                        item.AUTH_FINALIZED=="N"? "<center><a href='#' class='ui-icon ui-icon-pencil' onclick='EditAuthorization(\"" +URLEncrypt.EncryptParameters(new string[] { item.AUTH_ID.ToString().Trim()})+ "\");return false;'>Edit</a></center>" : "<center><a href='#' class='ui-icon ui-icon-locked' onclick='ViewMasterAuthorization(\"" +URLEncrypt.EncryptParameters(new string[] { item.AUTH_ID.ToString().Trim()})+ "\");return false;'>View Authorization Details</a></center>"     ,
                                        item.AUTH_FINALIZED=="N"? "<center><a href='#' class='ui-icon ui-icon-trash' onclick='DeleteAuthorization(\"" +URLEncrypt.EncryptParameters(new string[] { item.AUTH_ID.ToString().Trim()})+ "\");return false;'>Delete</a></center>" : ""     ,
                                        
                                        
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
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }



        /// <summary>
        /// DAL function to Add and edit the authorization master request page
        /// </summary>
        /// <param name="model"> authorization model to add edit</param>
        /// <param name="operation"> A for Add E for Edit</param>
        /// <param name="Auth_Id"> In case of Edit its auth id of the auth request to edit</param>
        /// <returns></returns>
        public Int64 AddEditMasterAuthorizationDetails(AuthorizationRequestMasterModel model, string operationType, Int64 Auth_Id)
        {
            CommonFunctions objCommon = new CommonFunctions();
            dbContext = new PMGSYEntities();
            try
            {
                using (var scope = new TransactionScope())
                {

                    ACC_AUTH_REQUEST_MASTER ModelToAdd = new ACC_AUTH_REQUEST_MASTER();

                    //check if authorization number already exist
                    string auth_No = String.Empty;

                    auth_No = dbContext.ACC_AUTH_REQUEST_MASTER.Where(c => c.AUTH_MONTH == model.AUTH_MONTH
                         && c.AUTH_YEAR == model.AUTH_YEAR && c.FUND_TYPE == model.FUND_TYPE
                         && c.LVL_ID == c.LVL_ID &&
                         c.ADMIN_ND_CODE == c.ADMIN_ND_CODE
                         && !operationType.Equals("A") ? c.AUTH_ID != Auth_Id : 1 == 1
                         ).Select(x => x.AUTH_NO).FirstOrDefault();

                    if (auth_No != null && auth_No != string.Empty)
                    {
                        if (auth_No == model.AUTH_NO)
                        {
                            return Auth_Id = -2;
                        }
                    }

                    /*
                    //check if bank details is entered for the selected fund
                    int? parentNdCode = dbContext.ADMIN_DEPARTMENT.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Select(x => x.MAST_PARENT_ND_CODE).First();
                    short  bankCode = dbContext.ACC_BANK_DETAILS.Where(c => c.ADMIN_ND_CODE == parentNdCode && c.BANK_ACC_STATUS == true && c.FUND_TYPE == PMGSYSession.Current.FundType).Select(x => x.BANK_CODE).FirstOrDefault();

                    if (bankCode == 0)
                    {
                        return Auth_Id = -3;
                    }

                    */
                    Int64 maxBillId = 0;
                    if (operationType.Equals("A"))
                    {
                        if (dbContext.ACC_AUTH_REQUEST_MASTER.Any())
                        {
                            maxBillId = dbContext.ACC_AUTH_REQUEST_MASTER.Max(c => c.AUTH_ID);

                        }

                        maxBillId = maxBillId + 1;

                        ModelToAdd.AUTH_ID = maxBillId;

                    }
                    else
                    {

                        ModelToAdd.AUTH_ID = Auth_Id;
                    }

                    ModelToAdd.AUTH_MONTH = (byte)model.AUTH_MONTH; //find out in controller
                    ModelToAdd.AUTH_YEAR = model.AUTH_YEAR; //find out in controller
                    ModelToAdd.AUTH_DATE = objCommon.GetStringToDateTime(model.AUTH_DATE);
                    ModelToAdd.TXN_ID = Convert.ToInt16(model.TXN_ID.Split('$')[0]);

                    ModelToAdd.AUTH_NO = model.AUTH_NO;
                    ModelToAdd.CHQ_AMOUNT = model.CHEQUE_AMOUNT;
                    ModelToAdd.CASH_AMOUNT = model.CASH_AMOUNT;
                    ModelToAdd.GROSS_AMOUNT = model.GROSS_AMOUNT;

                    ModelToAdd.AUTH_FINALIZED = "N";
                    ModelToAdd.FUND_TYPE = model.FUND_TYPE;
                    ModelToAdd.ADMIN_ND_CODE = model.ADMIN_ND_CODE;
                    ModelToAdd.LVL_ID = (byte)model.LVL_ID;
                    ModelToAdd.MAST_CON_ID = model.MAST_CON_ID;

                    //added by Abhishek kamble 29-nov-2013
                    ModelToAdd.USERID = PMGSYSession.Current.UserId;
                    ModelToAdd.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    if (operationType.Equals("A"))
                    {

                        dbContext.ACC_AUTH_REQUEST_MASTER.Add(ModelToAdd);
                        dbContext.SaveChanges();
                        scope.Complete();
                        return maxBillId;
                    }
                    else
                    {
                        ACC_AUTH_REQUEST_MASTER old_model = new ACC_AUTH_REQUEST_MASTER();
                        old_model = dbContext.ACC_AUTH_REQUEST_MASTER.Where(x => x.AUTH_ID == Auth_Id).FirstOrDefault();
                        dbContext.Entry(old_model).CurrentValues.SetValues(ModelToAdd);


                        dbContext.SaveChanges();
                        scope.Complete();
                        return Auth_Id;

                    }

                }

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error while adding /updating  Authorization master details. ");

            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }


        /// <summary>
        /// DAL function to get the Authorization number
        /// </summary>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <param name="StateCode"></param>
        /// <param name="AdminNdCode"></param>
        /// <returns> authorization number</returns>
        public string GetAuthorizationNumber(short month, short year, int stateCode, int adminNdCode)
        {
            CommonFunctions objCommon = new CommonFunctions();

            try
            {
                String financialYear = objCommon.getFinancialYear(month, year);

                dbContext = new PMGSYEntities();

                String EpayNumber = "BAR/" + dbContext.MASTER_STATE.Where(x => x.MAST_STATE_CODE == stateCode).Select(y => y.MAST_STATE_SHORT_CODE).First().ToString()
                   + "/" + dbContext.ADMIN_DEPARTMENT.Where(x => x.ADMIN_ND_CODE == adminNdCode).Select(y => y.ADMIN_EPAY_DPIU_CODE).First().ToString() + "/" +
                     financialYear + "/" + month.ToString() + "/";

                int maxEpayVoucherCount = 0;

                if (dbContext.ACC_AUTH_REQUEST_MASTER.Where(t => t.ADMIN_ND_CODE == adminNdCode).Any())
                {
                    maxEpayVoucherCount = dbContext.ACC_AUTH_REQUEST_MASTER.Where(t => t.AUTH_MONTH == month && t.AUTH_YEAR == year && t.ADMIN_ND_CODE == adminNdCode).Count();

                    maxEpayVoucherCount = maxEpayVoucherCount + 1;

                }
                else
                {
                    maxEpayVoucherCount = maxEpayVoucherCount + 1;
                }


                EpayNumber = EpayNumber + maxEpayVoucherCount.ToString();

                return EpayNumber;


            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error while getting is Authorization Number");
            }
            finally
            {
                dbContext.Dispose();
            }

        }


        /// <summary>
        /// DAL function to get the authorization balances
        /// </summary>
        /// <param name="authID"></param>
        /// <returns></returns>
        public AmountCalculationModel GetAuthorizationAmountBalance(Int64 authID)
        {
            PMGSYEntities localDbContext = new PMGSYEntities();
            CommonFunctions common = new CommonFunctions();
            try
            {

                AmountCalculationModel amountModel = new AmountCalculationModel();


                ACC_AUTH_REQUEST_MASTER authMaster = new ACC_AUTH_REQUEST_MASTER();
                authMaster = localDbContext.ACC_AUTH_REQUEST_MASTER.Where(x => x.AUTH_ID == authID).First();

                amountModel.TotalAmtToEnterCachAmount = authMaster.CASH_AMOUNT;
                amountModel.TotalAmtToEnterChqAmount = authMaster.CHQ_AMOUNT;
                amountModel.TotalAmtToEnterDedAmount = authMaster.CASH_AMOUNT;
                amountModel.TotalAmtToEnterGrossAmount = authMaster.CASH_AMOUNT + authMaster.CHQ_AMOUNT;

                if (localDbContext.ACC_AUTH_REQUEST_MASTER.Any(x => x.AUTH_ID == authID))
                {

                    var query = (from master in localDbContext.ACC_AUTH_REQUEST_MASTER
                                 join details in localDbContext.ACC_AUTH_REQUEST_DETAILS
                                 on master.AUTH_ID equals details.AUTH_ID
                                 where master.AUTH_ID == authID && details.CREDIT_DEBIT == "D"

                                 select new
                                 {
                                     master.CHQ_AMOUNT,
                                     master.CASH_AMOUNT,
                                     master.AUTH_FINALIZED,
                                     details.CASH_CHQ,
                                     details.AMOUNT,
                                     master.TXN_ID
                                 });



                    foreach (var item in query)
                    {
                        amountModel.TotalAmtToEnterCachAmount = item.CASH_AMOUNT;
                        amountModel.TotalAmtToEnterChqAmount = item.CHQ_AMOUNT;
                        amountModel.TotalAmtToEnterDedAmount = item.CASH_AMOUNT;
                        amountModel.TotalAmtToEnterGrossAmount = (amountModel.TotalAmtToEnterCachAmount + amountModel.TotalAmtToEnterChqAmount);

                        if (item.CASH_CHQ.Equals("C"))
                        {
                            amountModel.TotalAmtEnteredCachAmount = amountModel.TotalAmtEnteredCachAmount + item.AMOUNT;
                            // amountModel.DiffCachAmount = amountModel.TotalAmtToEnterCachAmount - amountModel.TotalAmtEnteredCachAmount;
                        }

                        if (item.CASH_CHQ.Equals("Q"))
                        {
                            amountModel.TotalAmtEnteredChqAmount = amountModel.TotalAmtEnteredChqAmount + item.AMOUNT;
                            // amountModel.DiffChqAmount = amountModel.TotalAmtToEnterChqAmount - amountModel.TotalAmtEnteredChqAmount;
                        }

                        if (item.CASH_CHQ.Equals("D"))
                        {
                            amountModel.TotalAmtEnteredDedAmount = amountModel.TotalAmtEnteredDedAmount + item.AMOUNT;

                        }


                        // amountModel.TotalAmtEnteredGrossAmount = amountModel.TotalAmtEnteredGrossAmount + (amountModel.TotalAmtEnteredCachAmount + amountModel.TotalAmtEnteredChqAmount);
                        amountModel.VoucherFinalized = item.AUTH_FINALIZED;

                    }

                    amountModel.TotalAmtEnteredGrossAmount = (amountModel.TotalAmtEnteredCachAmount + amountModel.TotalAmtEnteredChqAmount);
                    amountModel.DiffGrossAmount = amountModel.TotalAmtToEnterGrossAmount - amountModel.TotalAmtEnteredGrossAmount;
                    amountModel.DiffDedAmount = amountModel.TotalAmtToEnterCachAmount - amountModel.TotalAmtEnteredDedAmount;
                    amountModel.DiffChqAmount = amountModel.TotalAmtToEnterChqAmount - amountModel.TotalAmtEnteredChqAmount;
                    amountModel.DiffCachAmount = amountModel.TotalAmtToEnterCachAmount - amountModel.TotalAmtEnteredCachAmount;

                }
                else
                {
                    amountModel.TotalAmtToEnterCachAmount = authMaster.CASH_AMOUNT;
                    amountModel.TotalAmtToEnterChqAmount = authMaster.CHQ_AMOUNT;
                    amountModel.TotalAmtToEnterGrossAmount = (amountModel.TotalAmtToEnterCachAmount + amountModel.TotalAmtToEnterChqAmount);
                    amountModel.DiffCachAmount = 0;
                    amountModel.DiffChqAmount = 0;
                    amountModel.DiffGrossAmount = amountModel.TotalAmtToEnterGrossAmount;
                    amountModel.DiffDedAmount = 0;
                    amountModel.TotalAmtToEnterDedAmount = authMaster.CHQ_AMOUNT;
                }

                return amountModel;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);

                throw new Exception("Error while calculating Authorization amount balances.");
            }
            finally
            {
                localDbContext.Dispose();
            }

        }

        /// <summary>
        /// function to delete the authorization request
        /// </summary>
        /// <param name="auth_id"></param>
        /// <returns> -1 if allredy finalized</returns>
        public String DeleteAuthorizationRequest(long auth_id)
        {
            dbContext = new PMGSYEntities();

            try
            {
                using (var scope = new TransactionScope())
                {

                    //get the master details
                    ACC_AUTH_REQUEST_MASTER con = dbContext.ACC_AUTH_REQUEST_MASTER.SingleOrDefault(p => p.AUTH_ID == auth_id);

                    //cheque if finalized
                    if (con.AUTH_FINALIZED == "Y")
                    {
                        return "-1"; //return status error
                    }

                    //added by Abhishek kamble 29-nov-2013                                                                                                
                    ACC_AUTH_REQUEST_DETAILS accAuthRequestDetails = dbContext.ACC_AUTH_REQUEST_DETAILS.Where(m => m.AUTH_ID == auth_id).FirstOrDefault();
                    if (accAuthRequestDetails != null)
                    {
                        accAuthRequestDetails.USERID = PMGSYSession.Current.UserId;
                        accAuthRequestDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        dbContext.Entry(accAuthRequestDetails).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }

                    //delete the cheque issued entry if allredy exist
                    dbContext.Database.ExecuteSqlCommand
                        ("DELETE [omms].ACC_AUTH_REQUEST_DETAILS Where AUTH_ID = {0}", auth_id);


                    //added by Abhishek kamble 29-nov-2013                                                                                                
                    ACC_AUTH_REQUEST_TRACKING accAuthRequestTracking = dbContext.ACC_AUTH_REQUEST_TRACKING.Where(m => m.AUTH_ID == auth_id).FirstOrDefault();
                    if (accAuthRequestTracking != null)
                    {
                        accAuthRequestTracking.USERID = PMGSYSession.Current.UserId;
                        accAuthRequestTracking.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        dbContext.Entry(accAuthRequestTracking).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }

                    //added by koustubh nakate on 21/08/2013 to delete details from ACC_NOTIFICATION_DETAILS     
                    dbContext.Database.ExecuteSqlCommand
                       ("DELETE [omms].ACC_AUTH_REQUEST_TRACKING Where AUTH_ID = {0}", auth_id);

                    //added by Abhishek kamble 29-nov-2013                                                                                                
                    //ACC_NOTIFICATION_DETAILS accNotificationDetails = dbContext.ACC_NOTIFICATION_DETAILS.Where(m => m.INITIATION_BILL_ID == auth_id && m.NOTIFICATION_ID==11 ||m.NOTIFICATION_ID==12||m.NOTIFICATION_ID==13).FirstOrDefault();
                    //if (accAuthRequestTracking != null)
                    //{
                    //    accAuthRequestTracking.USERID = PMGSYSession.Current.UserId;
                    //    accAuthRequestTracking.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    //    dbContext.Entry(accAuthRequestTracking).State = System.Data.Entity.EntityState.Modified;
                    //    dbContext.SaveChanges();
                    //}

                    //added by Abhishek kamble 29-nov-2013                                                                                                
                    dbContext.Database.ExecuteSqlCommand("UPDATE [omms].ACC_NOTIFICATION_DETAILS SET USERID={0}, IPADD={1} Where INITIATION_BILL_ID = {2} AND NOTIFICATION_ID IN ({3},{4},{5})",PMGSYSession.Current.UserId,HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], auth_id, 11, 12, 13);

                    dbContext.Database.ExecuteSqlCommand
                       ("DELETE [omms].ACC_NOTIFICATION_DETAILS Where INITIATION_BILL_ID = {0} AND NOTIFICATION_ID IN ({1},{2},{3})", auth_id, 11, 12, 13);

                 
                    //delete the master table entry
                    dbContext.ACC_AUTH_REQUEST_MASTER.Remove(con);

                    dbContext.SaveChanges();

                    scope.Complete();
                    return "1";
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);

                throw new Exception("Error while deleting aithorization requests");
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }

        }

        /// <summary>
        /// function to check whether authorization finalized
        /// </summary>
        /// <param name="auth_id"></param>
        public bool CanAuthFinalize(long auth_id)
        {

            try
            {

                AmountCalculationModel model = new AmountCalculationModel();

                model = GetAuthorizationAmountBalance(auth_id);

                if (model.DiffCachAmount == 0 && model.DiffChqAmount == 0 && model.DiffDedAmount == 0 && model.DiffGrossAmount == 0)
                {
                    return true;
                }
                else return false;

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error while deleting aithorization requests");
            }
            finally
            {

            }
        }

        /// <summary>
        /// function to get authorization master details
        /// </summary>
        /// <param name="auth_id"></param>
        /// <returns></returns>
        public ACC_AUTH_REQUEST_MASTER GetMasterAuthorizationDetails(long auth_id)
        {
            dbContext = new PMGSYEntities();
            try
            {
                return dbContext.ACC_AUTH_REQUEST_MASTER.Where(c => c.AUTH_ID == auth_id).First();

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error while geting authorization master details");
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }

        }



        /// <summary>
        /// function to get authorizationtransaction details
        /// </summary>
        /// <param name="auth_id"></param>
        /// <returns></returns>
        public ACC_AUTH_REQUEST_DETAILS GetAuthTransactionDetails(long auth_id, int transId)
        {
            dbContext = new PMGSYEntities();
            try
            {
                return dbContext.ACC_AUTH_REQUEST_DETAILS.Where(c => c.AUTH_ID == auth_id && c.TXN_NO == transId).First();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error while geting authorization transaction details");
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        /// <summary>
        /// function to finalize the authorization details
        /// </summary>
        /// <param name="auth_id"></param>
        /// <returns></returns>
        public String FinlizeAuthorization(long auth_id)
        {

            try
            {
                string fundType = PMGSYSession.Current.FundType;
                int adminNdCode = PMGSYSession.Current.AdminNdCode;
                short levelID = PMGSYSession.Current.LevelId;

                dbContext = new PMGSYEntities();

                using (var scope = new TransactionScope())
                {

                    if (CanAuthFinalize(auth_id))
                    {
                        //if all diffrence amount is 0 then finalize authorization

                        long requestId = 0;
                        if (dbContext.ACC_AUTH_REQUEST_TRACKING.Any())
                        {
                            requestId = dbContext.ACC_AUTH_REQUEST_TRACKING.Max(c => c.REQUEST_ID);
                        }

                        requestId = requestId + 1;

                        ACC_AUTH_REQUEST_TRACKING modelToAdd = new ACC_AUTH_REQUEST_TRACKING();
                        modelToAdd.REQUEST_ID = requestId;
                        modelToAdd.AUTH_ID = auth_id;
                        modelToAdd.AUTH_STATUS = "F";
                        modelToAdd.REMARKS = "forwarding request to PIU";
                        modelToAdd.DATE_OF_OPERATION = DateTime.Now;

                        //Added By Abhishek Kamble 29-nov-2013
                        modelToAdd.USERID = PMGSYSession.Current.UserId;
                        modelToAdd.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]; 

                        dbContext.ACC_AUTH_REQUEST_TRACKING.Add(modelToAdd);

                        ACC_AUTH_REQUEST_MASTER ACC_AUTH_REQUEST_MASTER = dbContext.ACC_AUTH_REQUEST_MASTER.Find(auth_id);
                        ACC_AUTH_REQUEST_MASTER.AUTH_FINALIZED = "Y";
                        ACC_AUTH_REQUEST_MASTER.CURRENT_AUTH_STATUS = "F";

                        //Added By Abhishek Kamble 29-nov-2013
                        ACC_AUTH_REQUEST_MASTER.USERID = PMGSYSession.Current.UserId;
                        ACC_AUTH_REQUEST_MASTER.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]; 

                        dbContext.Entry(ACC_AUTH_REQUEST_MASTER).State = System.Data.Entity.EntityState.Modified; 

                        dbContext.SaveChanges();

                        //added by Koustubh Nakate on 21/08/2013 to save notification in notification details table 
                        var result = dbContext.USP_ACC_INSERT_ALERT_DETAILS(adminNdCode, fundType, "A", levelID, auth_id,PMGSYSession.Current.UserId,HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]);

                    }
                    else
                    {
                        return "-1";
                    }

                    scope.Complete();
                    return "1";

                }

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error while finalizing authorization requests");
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }

        }


        /// <summary>
        /// function to list the transaction details
        /// </summary>
        /// <param name="objFilter"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array ListAuthDeductionDetailsForDataEntry(PaymentFilterModel objFilter, out long totalRecords)
        {

            CommonFunctions commomFuncObj = null;
            commomFuncObj = new CommonFunctions();
            List<transactionList> lstTransactions = new List<transactionList>();

            try
            {

                dbContext = new PMGSYEntities();
                TransactionParams objParam = new TransactionParams();
                TransactionParams objparams = new TransactionParams();
                ACC_SCREEN_DESIGN_PARAM_MASTER obj1 = new ACC_SCREEN_DESIGN_PARAM_MASTER();
                objparams.TXN_ID = dbContext.ACC_AUTH_REQUEST_MASTER.Where(x => x.AUTH_ID == objFilter.BillId).Select(c => c.TXN_ID).First();
                obj1 = commomFuncObj.getMasterDesignParam(objparams);


                var query = (from master in dbContext.ACC_AUTH_REQUEST_MASTER
                             join details in dbContext.ACC_AUTH_REQUEST_DETAILS
                             on master.AUTH_ID equals details.AUTH_ID
                             where master.AUTH_ID == objFilter.BillId
                             && details.CREDIT_DEBIT == "D"
                             select new
                             {
                                 master.AUTH_FINALIZED,
                                 details.AUTH_ID,
                                 details.ACC_MASTER_HEAD.HEAD_CODE,
                                 details.HEAD_ID,
                                 details.TXN_ID,
                                 details.CASH_CHQ,
                                 details.AMOUNT,
                                 details.NARRATION,
                                 details.TXN_NO,
                                 details.IMS_AGREEMENT_CODE,
                                 details.IMS_PR_ROAD_CODE,
                                 master.MAST_CON_ID,
                                 master.FUND_TYPE

                             });

                // query = query.OrderBy(c => c.CASH_CHQ == "Q").ThenBy(t => t.CASH_CHQ == "C").ThenBy(t => t.CASH_CHQ == "D");

                Int16 i = 0;
                foreach (var item in query)
                {
                    i++;
                    transactionList obj = new transactionList();
                    obj.SERIAL_No = i;
                    obj.BILL_FINALIZED = item.AUTH_FINALIZED;
                    obj.AMOUNT = item.AMOUNT;
                    obj.BILL_ID = item.AUTH_ID;
                    obj.CASH_CHQ = item.CASH_CHQ;
                    obj.HEAD_ID_Narration = dbContext.ACC_MASTER_TXN.Where(x => x.TXN_ID == item.TXN_ID).Select(h => h.TXN_DESC).FirstOrDefault();
                    // obj.HEAD_ID = item.HEAD_CODE.ToString();
                    obj.paymentType = obj.CASH_CHQ == "C" ? "Payment" : item.CASH_CHQ == "Q" ? "Payment" : "Deduction";
                    obj.HEAD_ID = obj.paymentType.Equals("Payment") ? item.HEAD_CODE.ToString() :
                            (dbContext.ACC_MASTER_HEAD.Where(t => t.HEAD_ID == (dbContext.ACC_AUTH_REQUEST_DETAILS.Where(c => c.AUTH_ID == item.AUTH_ID && c.TXN_NO == (item.TXN_NO - 1) && c.CREDIT_DEBIT.Equals("C")).Select(c => c.HEAD_ID).FirstOrDefault()))).Select(d => d.HEAD_CODE).FirstOrDefault().ToString();

                    obj.NARRATION = item.NARRATION;
                    obj.TXN_NO = item.TXN_NO;
                    obj.RODE_CODE = dbContext.IMS_SANCTIONED_PROJECTS.Where(c => c.IMS_PR_ROAD_CODE == (item.IMS_PR_ROAD_CODE.HasValue ? item.IMS_PR_ROAD_CODE.Value : -1)).Select(y => y.IMS_ROAD_NAME).FirstOrDefault();
                    //obj.AGREEMENT_CODE = dbContext.TEND_AGREEMENT_MASTER.Where(c => c.TEND_AGREEMENT_CODE == (item.IMS_AGREEMENT_CODE.HasValue ? item.IMS_AGREEMENT_CODE.Value : -1)).Select(y => y.TEND_AGREEMENT_NUMBER).FirstOrDefault();
                    //old
                    //obj.AGREEMENT_CODE = item.FUND_TYPE == "M" ? (dbContext.MANE_IMS_CONTRACT.Where(c => c.MANE_PR_CONTRACT_CODE == (item.IMS_AGREEMENT_CODE.HasValue ? item.IMS_AGREEMENT_CODE.Value : -1)).Select(x => x.MANE_AGREEMENT_NUMBER).FirstOrDefault()) : (dbContext.TEND_AGREEMENT_MASTER.Where(c => c.TEND_AGREEMENT_CODE == (item.IMS_AGREEMENT_CODE.HasValue ? item.IMS_AGREEMENT_CODE.Value : -1)).Select(y => y.TEND_AGREEMENT_NUMBER).FirstOrDefault());
                    //Modified by Abhishek kamble to get Agr Number using MANE_CONTRACTOR_ID 
                    obj.AGREEMENT_CODE = item.FUND_TYPE == "M" ? (dbContext.MANE_IMS_CONTRACT.Where(c => c.MANE_CONTRACT_ID == (item.IMS_AGREEMENT_CODE.HasValue ? item.IMS_AGREEMENT_CODE.Value : -1)).Select(x => x.MANE_AGREEMENT_NUMBER).FirstOrDefault()) : (dbContext.TEND_AGREEMENT_MASTER.Where(c => c.TEND_AGREEMENT_CODE == (item.IMS_AGREEMENT_CODE.HasValue ? item.IMS_AGREEMENT_CODE.Value : -1)).Select(y => y.TEND_AGREEMENT_NUMBER).FirstOrDefault());

                    obj.ONLY_DEDUCTION = obj1.DED_REQ == "B" ? "Y" : "N";


                    if (item.MAST_CON_ID != 0)
                    {
                        objParam.MAST_CONT_ID = item.MAST_CON_ID == null ? 0 : item.MAST_CON_ID.Value;
                        obj.CONTRACTORNAME = dbContext.MASTER_CONTRACTOR.Where(c => c.MAST_CON_ID == item.MAST_CON_ID).Select(d => d.MAST_CON_COMPANY_NAME).FirstOrDefault();
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
                    id = URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() + "$" + item.TXN_NO + "$" + item.CASH_CHQ }),
                    cell = new[] {                         
                                    
                                        //item.BILL_ID.ToString(),
                                        item.CASH_CHQ=="C"? "Payment" : item.CASH_CHQ=="Q"? "Payment" : "Deduction",
                                        item.SERIAL_No.ToString(),
                                        item.HEAD_ID.ToString(),
                                        item.HEAD_ID_Narration,
                                        item.CONTRACTORNAME,
                                        item.AGREEMENT_CODE,
                                        item.RODE_CODE,
                                        item.CASH_CHQ !="Q"? "Cash":"Cheque",
                                        item.AMOUNT.ToString(),
                                        item.NARRATION.ToString(),
                                        item.BILL_FINALIZED=="N" && (item.CASH_CHQ=="Q" ||item.CASH_CHQ=="D" ) ?  "<center><a href='#' class='ui-icon ui-icon-pencil' onclick='EditTransactionPayment(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()  +"$"+ item.TXN_NO+"$"+ item.CASH_CHQ})+ "\");return false;'>Edit</a></center>" :( (item.CASH_CHQ=="Q" || item.CASH_CHQ=="D")?  "<center><a href='#' class='ui-icon ui-icon-locked' onclick='ViewTransactionPayment(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()  +"$"+ item.TXN_NO+"$"+ item.CASH_CHQ})+ "\");return false;'>View</a></center>": string.Empty),    
                                        item.BILL_FINALIZED=="N" && (item.CASH_CHQ=="Q" ||item.CASH_CHQ=="D")? "<center><a href='#' class='ui-icon ui-icon-trash' onclick='DeleteTransactionPayment(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()  +"$"+ item.TXN_NO+"$"+ item.CASH_CHQ})+ "\");return false;'>Delete</a></center>" : ""                            
                  
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
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }


        }


        /// <summary>
        /// function to add edit the authorization transaction and deduction details
        /// </summary>
        /// <param name="model"></param>
        /// <param name="operationType"></param>
        /// <param name="Bill_id"></param>
        /// <param name="AddorEdit"></param>
        /// <param name="tranNumber"></param>
        /// <returns></returns>
        public Boolean AddEditTransactionDeductionPaymentDetails(AuthorizationRequestDetailsModel model, string operationType, Int64 Bill_id, String AddorEdit, Int16 tranNumber)
        {
            try
            {
                dbContext = new PMGSYEntities();
                ACC_AUTH_REQUEST_DETAILS ModelToAdd = null;
                Int16 maxTxnNo = 0;
                ACC_AUTH_REQUEST_DETAILS PaymentTransaction = new ACC_AUTH_REQUEST_DETAILS();

                string masterChequeCash = string.Empty;

                masterChequeCash = "Q";

                ///if operation is edit get the transaction details from database for credit entry
                if (AddorEdit.Equals("E"))
                {
                    PaymentTransaction = dbContext.ACC_AUTH_REQUEST_DETAILS.Where(t => t.AUTH_ID == Bill_id && t.TXN_NO == (tranNumber - 1)).FirstOrDefault();
                }

                //if simple transaction
                if (operationType.Equals("T"))
                {
                    using (var scope = new TransactionScope())
                    {

                        //if operation is ADD
                        if (AddorEdit.Equals("A"))
                        {
                            if (dbContext.ACC_AUTH_REQUEST_DETAILS.Where(t => t.AUTH_ID == Bill_id).Any())
                            {
                                maxTxnNo = dbContext.ACC_AUTH_REQUEST_DETAILS.Where(t => t.AUTH_ID == Bill_id).Max(c => c.TXN_NO);
                            }

                            //creating credit entry Cheque payment
                            maxTxnNo = Convert.ToInt16(maxTxnNo + 1);


                        }
                        else
                        {

                            maxTxnNo = PaymentTransaction.TXN_NO;
                            model.CASH_CHQ = PaymentTransaction.CASH_CHQ;


                        }

                        ModelToAdd = new ACC_AUTH_REQUEST_DETAILS();

                        Int16 TXN_ID_Model = Convert.ToInt16(model.HEAD_ID_P.ToString().Split('$')[0]);

                        //String CashChq = Convert.ToString(model.HEAD_ID_P.ToString().Split('$')[1]); 

                        //get the credit head
                        //for this credit head use the cash/cheque from master table 

                        ModelToAdd.HEAD_ID = (from item in dbContext.ACC_TXN_HEAD_MAPPING
                                              where item.TXN_ID == TXN_ID_Model && item.CASH_CHQ.Contains(masterChequeCash) && item.CREDIT_DEBIT == "C"
                                              select item.HEAD_ID).FirstOrDefault();

                        ModelToAdd.AUTH_ID = Bill_id;

                        //find transaction number only if operation is add

                        ModelToAdd.TXN_NO = maxTxnNo;
                        ModelToAdd.TXN_ID = TXN_ID_Model;
                        ModelToAdd.AMOUNT = model.AMOUNT_Q.HasValue ? model.AMOUNT_Q.Value : 0;
                        ModelToAdd.CREDIT_DEBIT = "C";
                        ModelToAdd.CASH_CHQ = model.CASH_CHQ;
                        ModelToAdd.NARRATION = model.NARRATION_P;


                        ModelToAdd.IMS_PR_ROAD_CODE = model.IMS_PR_ROAD_CODE;
                        ModelToAdd.IMS_AGREEMENT_CODE = model.IMS_AGREEMENT_CODE;
                        ModelToAdd.MAS_FA_CODE = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == (model.IMS_PR_ROAD_CODE.HasValue ? model.IMS_PR_ROAD_CODE.Value : -1)).Select(v => v.IMS_COLLABORATION).FirstOrDefault();
                        ModelToAdd.FINAL_PAYMENT = model.FINAL_PAYMENT;

                        //Added By Abhishek Kamble 29-nov-2013
                        ModelToAdd.USERID = PMGSYSession.Current.UserId;
                        ModelToAdd.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]; 

                        //creating debit cheque payment entry

                        ACC_AUTH_REQUEST_DETAILS ModelToAddDebit = new ACC_AUTH_REQUEST_DETAILS();

                        maxTxnNo = Convert.ToInt16(maxTxnNo + 1);

                        //get the debit head
                        ModelToAddDebit.HEAD_ID = (from item in dbContext.ACC_TXN_HEAD_MAPPING
                                                   where item.TXN_ID == TXN_ID_Model && item.CASH_CHQ.Contains(masterChequeCash) && item.CREDIT_DEBIT == "D"
                                                   select item.HEAD_ID).FirstOrDefault();

                        ModelToAddDebit.AUTH_ID = Bill_id;
                        ModelToAddDebit.TXN_NO = maxTxnNo;
                        ModelToAddDebit.TXN_ID = TXN_ID_Model;
                        ModelToAddDebit.AMOUNT = model.AMOUNT_Q.HasValue ? model.AMOUNT_Q.Value : 0;
                        ModelToAddDebit.CREDIT_DEBIT = "D";
                        ModelToAddDebit.CASH_CHQ = model.CASH_CHQ;
                        ModelToAddDebit.NARRATION = model.NARRATION_P;
                        ModelToAddDebit.IMS_PR_ROAD_CODE = model.IMS_PR_ROAD_CODE;
                        ModelToAddDebit.IMS_AGREEMENT_CODE = model.IMS_AGREEMENT_CODE;
                        ModelToAddDebit.MAS_FA_CODE = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == (model.IMS_PR_ROAD_CODE.HasValue ? model.IMS_PR_ROAD_CODE.Value : -1)).Select(v => v.IMS_COLLABORATION).FirstOrDefault();
                        ModelToAddDebit.FINAL_PAYMENT = model.FINAL_PAYMENT;

                        //Added By Abhishek Kamble 29-nov-2013
                        ModelToAddDebit.USERID = PMGSYSession.Current.UserId;
                        ModelToAddDebit.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]; 


                        //add both entries if add operation
                        if (AddorEdit.Equals("A"))
                        {
                            dbContext.ACC_AUTH_REQUEST_DETAILS.Add(ModelToAdd);
                            dbContext.ACC_AUTH_REQUEST_DETAILS.Add(ModelToAddDebit);
                        }
                        else
                        {
                            //make its state as modified

                            ACC_AUTH_REQUEST_DETAILS olddetails = new ACC_AUTH_REQUEST_DETAILS();
                            olddetails = dbContext.ACC_AUTH_REQUEST_DETAILS.Where(x => x.AUTH_ID == Bill_id && x.TXN_NO == (tranNumber - 1)).FirstOrDefault();
                            dbContext.Entry(olddetails).CurrentValues.SetValues(ModelToAdd);

                            olddetails = null;
                            olddetails = dbContext.ACC_AUTH_REQUEST_DETAILS.Where(x => x.AUTH_ID == Bill_id && x.TXN_NO == (tranNumber)).FirstOrDefault();
                            dbContext.Entry(olddetails).CurrentValues.SetValues(ModelToAddDebit);


                        }


                        #region cash amount data entry
                        {
                            //if  cash amount is not equal to 0
                            if (dbContext.ACC_AUTH_REQUEST_MASTER.Where(t => t.AUTH_ID == Bill_id).Select(b => b.CASH_AMOUNT).First() != 0
                                && dbContext.ACC_AUTH_REQUEST_MASTER.Where(t => t.AUTH_ID == Bill_id).Select(b => b.CHQ_AMOUNT).First() != 0)
                            {

                                //create credit entry for cash
                                ACC_AUTH_REQUEST_DETAILS CashModelCredit = new ACC_AUTH_REQUEST_DETAILS();

                                maxTxnNo = Convert.ToInt16(maxTxnNo + 1);

                                //get the debit head
                                CashModelCredit.HEAD_ID = (from item in dbContext.ACC_TXN_HEAD_MAPPING
                                                           where item.TXN_ID == TXN_ID_Model && item.CASH_CHQ.Trim().Equals("C") && item.CREDIT_DEBIT == "C"
                                                           select item.HEAD_ID).FirstOrDefault();

                                CashModelCredit.AUTH_ID = Bill_id;
                                CashModelCredit.TXN_NO = maxTxnNo;
                                CashModelCredit.TXN_ID = TXN_ID_Model;
                                CashModelCredit.AMOUNT = model.AMOUNT_C.HasValue ? model.AMOUNT_C.Value : 0;
                                CashModelCredit.CREDIT_DEBIT = "C";
                                CashModelCredit.CASH_CHQ = "C";
                                CashModelCredit.NARRATION = model.NARRATION_P;
                                CashModelCredit.IMS_PR_ROAD_CODE = model.IMS_PR_ROAD_CODE;
                                CashModelCredit.IMS_AGREEMENT_CODE = model.IMS_AGREEMENT_CODE;
                                CashModelCredit.MAS_FA_CODE = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == (model.IMS_PR_ROAD_CODE.HasValue ? model.IMS_PR_ROAD_CODE.Value : -1)).Select(v => v.IMS_COLLABORATION).FirstOrDefault();
                                CashModelCredit.FINAL_PAYMENT = model.FINAL_PAYMENT;

                                //Added By Abhishek Kamble 29-nov-2013
                                CashModelCredit.USERID = PMGSYSession.Current.UserId;
                                CashModelCredit.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]; 



                                //create Debit Entry for cash

                                ACC_AUTH_REQUEST_DETAILS CashModelDebit = new ACC_AUTH_REQUEST_DETAILS();

                                maxTxnNo = Convert.ToInt16(maxTxnNo + 1);

                                //get the debit head
                                CashModelDebit.HEAD_ID = (from item in dbContext.ACC_TXN_HEAD_MAPPING
                                                          where item.TXN_ID == TXN_ID_Model && item.CASH_CHQ.Contains(masterChequeCash) && item.CREDIT_DEBIT == "D"
                                                          select item.HEAD_ID).FirstOrDefault();

                                CashModelDebit.AUTH_ID = Bill_id;
                                CashModelDebit.TXN_NO = maxTxnNo;
                                CashModelDebit.TXN_ID = TXN_ID_Model;
                                CashModelDebit.AMOUNT = model.AMOUNT_C.HasValue ? model.AMOUNT_C.Value : 0;
                                CashModelDebit.CREDIT_DEBIT = "D";
                                CashModelDebit.CASH_CHQ = "C";
                                CashModelDebit.NARRATION = model.NARRATION_P;

                                CashModelDebit.IMS_PR_ROAD_CODE = model.IMS_PR_ROAD_CODE;
                                CashModelDebit.IMS_AGREEMENT_CODE = model.IMS_AGREEMENT_CODE;
                                CashModelDebit.MAS_FA_CODE = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == (model.IMS_PR_ROAD_CODE.HasValue ? model.IMS_PR_ROAD_CODE.Value : -1)).Select(v => v.IMS_COLLABORATION).FirstOrDefault();
                                CashModelDebit.FINAL_PAYMENT = model.FINAL_PAYMENT;

                                //Added By Abhishek Kamble 29-nov-2013
                                CashModelDebit.USERID = PMGSYSession.Current.UserId;
                                CashModelDebit.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]; 


                                //add the credit debit entries if add operation
                                if (AddorEdit.Equals("A"))
                                {
                                    dbContext.ACC_AUTH_REQUEST_DETAILS.Add(CashModelCredit);
                                    dbContext.ACC_AUTH_REQUEST_DETAILS.Add(CashModelDebit);
                                }
                                else
                                {

                                    ACC_AUTH_REQUEST_DETAILS olddetails = new ACC_AUTH_REQUEST_DETAILS();
                                    olddetails = dbContext.ACC_AUTH_REQUEST_DETAILS.Where(x => x.AUTH_ID == Bill_id && x.TXN_NO == CashModelCredit.TXN_NO).FirstOrDefault();
                                    dbContext.Entry(olddetails).CurrentValues.SetValues(CashModelCredit);

                                    olddetails = null;
                                    olddetails = dbContext.ACC_AUTH_REQUEST_DETAILS.Where(x => x.AUTH_ID == Bill_id && x.TXN_NO == (CashModelDebit.TXN_NO)).FirstOrDefault();
                                    dbContext.Entry(olddetails).CurrentValues.SetValues(CashModelDebit);



                                }
                            }

                        }
                        #endregion cash amount data entry

                        dbContext.SaveChanges();
                        scope.Complete();
                        return true;

                    }

                }
                else if (operationType.Equals("D")) //if entry to be entered is deduction entry
                {
                    using (var scope = new TransactionScope())
                    {
                        if (AddorEdit.Equals("A"))
                        {
                            if (dbContext.ACC_AUTH_REQUEST_DETAILS.Where(t => t.AUTH_ID == Bill_id).Any())
                            {
                                maxTxnNo = dbContext.ACC_AUTH_REQUEST_DETAILS.Where(t => t.AUTH_ID == Bill_id).Max(c => c.TXN_NO);
                            }
                            maxTxnNo = Convert.ToInt16(maxTxnNo + 1);


                        }
                        else
                        {
                            maxTxnNo = PaymentTransaction.TXN_NO; ;
                        }

                        //get the payment transaction so that we can get the contractor/supplier id && AgreementCode 
                        //Note that IMS_PR_ROAD_CODE is not required for deduction entry


                        //create credit entry for deduction
                        ACC_AUTH_REQUEST_DETAILS DeductionModelCredit = new ACC_AUTH_REQUEST_DETAILS();


                        Int16 TXN_ID_Model = Convert.ToInt16(model.HEAD_ID_D.ToString().Split('$')[0]);
                        // String CashChq = Convert.ToString(model.HEAD_ID_D.ToString().Split('$')[1]);

                        //get the debit head
                        DeductionModelCredit.HEAD_ID = (from item in dbContext.ACC_TXN_HEAD_MAPPING
                                                        where item.TXN_ID == TXN_ID_Model && item.CASH_CHQ.Contains("C") && item.CREDIT_DEBIT == "C"
                                                        select item.HEAD_ID).FirstOrDefault();

                        DeductionModelCredit.AUTH_ID = Bill_id;
                        DeductionModelCredit.TXN_NO = maxTxnNo;
                        DeductionModelCredit.TXN_ID = TXN_ID_Model;
                        DeductionModelCredit.AMOUNT = model.AMOUNT_D.HasValue ? model.AMOUNT_D.Value : 0;
                        DeductionModelCredit.CREDIT_DEBIT = "C";
                        DeductionModelCredit.CASH_CHQ = "D";
                        DeductionModelCredit.NARRATION = model.NARRATION_D;

                        //DeductionModelCredit.IMS_PR_ROAD_CODE = model.IMS_PR_ROAD_CODE;
                        DeductionModelCredit.IMS_AGREEMENT_CODE = model.IMS_AGREEMENT_CODE_DED;
                        // DeductionModelCredit.MAS_FA_CODE = model.MAS_FA_CODE;
                        // DeductionModelCredit.FINAL_PAYMENT = model.FINAL_PAYMENT;


                        //Added By Abhishek Kamble 29-nov-2013
                        DeductionModelCredit.USERID = PMGSYSession.Current.UserId;
                        DeductionModelCredit.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]; 


                        //create debit entry for deduction
                        ACC_AUTH_REQUEST_DETAILS DeductionModelDebit = new ACC_AUTH_REQUEST_DETAILS();

                        maxTxnNo = Convert.ToInt16(maxTxnNo + 1);

                        //get the debit head
                        DeductionModelDebit.HEAD_ID = (from item in dbContext.ACC_TXN_HEAD_MAPPING
                                                       where item.TXN_ID == TXN_ID_Model && item.CASH_CHQ.Contains("C") && item.CREDIT_DEBIT == "D"
                                                       select item.HEAD_ID).FirstOrDefault();

                        DeductionModelDebit.AUTH_ID = Bill_id;
                        DeductionModelDebit.TXN_NO = maxTxnNo;
                        DeductionModelDebit.TXN_ID = TXN_ID_Model;
                        DeductionModelDebit.AMOUNT = model.AMOUNT_D.HasValue ? model.AMOUNT_D.Value : 0;
                        DeductionModelDebit.CREDIT_DEBIT = "D";
                        DeductionModelDebit.CASH_CHQ = "D";
                        DeductionModelDebit.NARRATION = model.NARRATION_D;

                        //DeductionModelDebit.IMS_PR_ROAD_CODE = model.IMS_PR_ROAD_CODE;
                        DeductionModelDebit.IMS_AGREEMENT_CODE = model.IMS_AGREEMENT_CODE_DED;
                        // DeductionModelDebit.MAS_FA_CODE = model.MAS_FA_CODE;
                        // DeductionModelDebit.FINAL_PAYMENT = model.FINAL_PAYMENT;


                        //Added By Abhishek Kamble 29-nov-2013
                        DeductionModelDebit.USERID = PMGSYSession.Current.UserId;
                        DeductionModelDebit.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]; 


                        if (AddorEdit.Equals("A"))
                        {
                            //add the row
                            dbContext.ACC_AUTH_REQUEST_DETAILS.Add(DeductionModelCredit);
                            dbContext.ACC_AUTH_REQUEST_DETAILS.Add(DeductionModelDebit);
                            dbContext.SaveChanges();
                            scope.Complete();
                            return true;
                        }
                        else
                        {

                            ACC_AUTH_REQUEST_DETAILS olddetails = new ACC_AUTH_REQUEST_DETAILS();
                            olddetails = dbContext.ACC_AUTH_REQUEST_DETAILS.Where(x => x.AUTH_ID == Bill_id && x.TXN_NO == DeductionModelCredit.TXN_NO).FirstOrDefault();
                            dbContext.Entry(olddetails).CurrentValues.SetValues(DeductionModelCredit);

                            olddetails = null;
                            olddetails = dbContext.ACC_AUTH_REQUEST_DETAILS.Where(x => x.AUTH_ID == Bill_id && x.TXN_NO == (DeductionModelDebit.TXN_NO)).FirstOrDefault();
                            dbContext.Entry(olddetails).CurrentValues.SetValues(DeductionModelDebit);


                            dbContext.SaveChanges();
                            scope.Complete();
                            return true;
                        }
                    }

                }
                else
                {

                    return false; // error operation types has to be transaction(cheque and cash) and deduction
                }



            }
            catch (DbEntityValidationException e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e, HttpContext.Current);

                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error while adding/editing transaction entry.");

            }
            finally
            {
                dbContext.Dispose();
            }
        }


        /// <summary>
        /// function to delete the transaction details
        /// </summary>
        /// <param name="master_Bill_Id">Bill_Id of the transaction to be deleted </param>
        /// <param name="tranNumber">debit transaction number </param>
        /// <param name="paymentDeduction"> payment entry or deduction entry</param>
        /// <returns> 1 if succesfull ;-1 if already finalized</returns>
        public Int32 DeleteAuthorizationTransDetails(Int64 master_Bill_Id, Int32 tranNumber, String paymentDeduction)
        {
            try
            {

                dbContext = new PMGSYEntities();
                using (var scope = new TransactionScope())
                {
                    //get the master

                    ACC_AUTH_REQUEST_MASTER con = dbContext.ACC_AUTH_REQUEST_MASTER.SingleOrDefault(p => p.AUTH_ID == master_Bill_Id);

                    Int16? TXN_ID = dbContext.ACC_AUTH_REQUEST_DETAILS.Where(p => p.AUTH_ID == master_Bill_Id && p.TXN_NO == tranNumber).Select(x => x.TXN_ID).FirstOrDefault();

                    //cheque if finalized
                    if (con.AUTH_FINALIZED == "Y")
                    {
                        return -1; //return status error
                    }

                    if (paymentDeduction.Equals("D"))
                    {
                        //get the transaction details
                        ACC_AUTH_REQUEST_DETAILS creditRow = dbContext.ACC_AUTH_REQUEST_DETAILS.SingleOrDefault(p => p.AUTH_ID == master_Bill_Id && p.TXN_NO == (tranNumber - 1) && p.TXN_ID == TXN_ID);
                        ACC_AUTH_REQUEST_DETAILS debitRow = dbContext.ACC_AUTH_REQUEST_DETAILS.SingleOrDefault(p => p.AUTH_ID == master_Bill_Id && p.TXN_NO == tranNumber && p.TXN_ID == TXN_ID);

                        //delete the details table entry
                        if (creditRow != null)
                        {                           
                            //Added By Abhishek Kamble 29-nov-2013
                            creditRow.USERID = PMGSYSession.Current.UserId;
                            creditRow.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            dbContext.Entry(creditRow).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();

                            dbContext.ACC_AUTH_REQUEST_DETAILS.Remove(creditRow);
                        }
                        if (debitRow != null)
                        {
                            //Added By Abhishek Kamble 29-nov-2013
                            debitRow.USERID = PMGSYSession.Current.UserId;
                            debitRow.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            dbContext.Entry(debitRow).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();

                            dbContext.ACC_AUTH_REQUEST_DETAILS.Remove(debitRow);
                        }
                        dbContext.SaveChanges();
                        scope.Complete();

                        return 1;
                    }
                    else
                    {

                        //get the transaction details with CHQ as Q
                        ACC_AUTH_REQUEST_DETAILS creditRowChequePayment = dbContext.ACC_AUTH_REQUEST_DETAILS.SingleOrDefault(p => p.AUTH_ID == master_Bill_Id && p.TXN_NO == (tranNumber - 1) && p.TXN_ID == TXN_ID);
                        ACC_AUTH_REQUEST_DETAILS debitRowChequePayment = dbContext.ACC_AUTH_REQUEST_DETAILS.SingleOrDefault(p => p.AUTH_ID == master_Bill_Id && p.TXN_NO == tranNumber && p.TXN_ID == TXN_ID);

                        ACC_AUTH_REQUEST_DETAILS creditRowCashPayment = new ACC_AUTH_REQUEST_DETAILS();
                        ACC_AUTH_REQUEST_DETAILS debitRowCashPayment = new ACC_AUTH_REQUEST_DETAILS();

                        //get the associated  cash transaction details  only if its cheque with deduction
                        if (con.CASH_AMOUNT != 0 && con.CHQ_AMOUNT != 0)
                        {
                            creditRowCashPayment = dbContext.ACC_AUTH_REQUEST_DETAILS.SingleOrDefault(p => p.AUTH_ID == master_Bill_Id && p.TXN_NO == (tranNumber + 1) && p.TXN_ID == TXN_ID);
                            debitRowCashPayment = dbContext.ACC_AUTH_REQUEST_DETAILS.SingleOrDefault(p => p.AUTH_ID == master_Bill_Id && p.TXN_NO == (tranNumber + 2) && p.TXN_ID == TXN_ID);
                        }
                        //delete the details table entry
                        if (creditRowChequePayment != null)
                        {

                            //Added By Abhishek Kamble 29-nov-2013
                            creditRowChequePayment.USERID = PMGSYSession.Current.UserId;
                            creditRowChequePayment.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            dbContext.Entry(creditRowChequePayment).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();

                            dbContext.ACC_AUTH_REQUEST_DETAILS.Remove(creditRowChequePayment);
                        }
                        if (debitRowChequePayment != null)
                        {

                            //Added By Abhishek Kamble 29-nov-2013
                            debitRowChequePayment.USERID = PMGSYSession.Current.UserId;
                            debitRowChequePayment.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            dbContext.Entry(debitRowChequePayment).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();

                            dbContext.ACC_AUTH_REQUEST_DETAILS.Remove(debitRowChequePayment);
                        }

                        //delete only if cheque trabsaction with deduction
                        if (con.CASH_AMOUNT != 0)
                        {
                            if (creditRowCashPayment != null)
                            {

                                //Added By Abhishek Kamble 29-nov-2013
                                creditRowCashPayment.USERID = PMGSYSession.Current.UserId;
                                creditRowCashPayment.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                                dbContext.Entry(creditRowCashPayment).State = System.Data.Entity.EntityState.Modified;
                                dbContext.SaveChanges();

                                dbContext.ACC_AUTH_REQUEST_DETAILS.Remove(creditRowCashPayment);
                            }
                            if (debitRowCashPayment != null)
                            {
                                //Added By Abhishek Kamble 29-nov-2013
                                debitRowCashPayment.USERID = PMGSYSession.Current.UserId;
                                debitRowCashPayment.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                                dbContext.Entry(debitRowCashPayment).State = System.Data.Entity.EntityState.Modified;
                                dbContext.SaveChanges();

                                dbContext.ACC_AUTH_REQUEST_DETAILS.Remove(debitRowCashPayment);
                            }

                        }
                        dbContext.SaveChanges();
                        scope.Complete();

                        return 1;

                    }
                }

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error while deleting authorization transaction  details..");

            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }

        }


        /// <summary>
        /// function to get the aggrement number for deduction entry in authorization 
        /// </summary>
        /// <param name="bill_id"></param>
        /// <returns></returns>
        public String GetAgreemntNumberForAuthorization(Int64 bill_id)
        {
            try
            {
                dbContext = new PMGSYEntities();

                return (dbContext.ACC_AUTH_REQUEST_DETAILS.Where(x => x.AUTH_ID == bill_id && x.CREDIT_DEBIT == "D").Select(y => y.IMS_AGREEMENT_CODE).FirstOrDefault().ToString());

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error while getting agreement details");
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }


        }

        /// <summary>
        /// function to get finalized status of authorization
        /// </summary>
        /// <param name="BILL_ID"></param>
        /// <param name="roadID"></param>
        /// <returns></returns>
        public List<SelectListItem> GetFinalPaymentDetails(Int64 BILL_ID, Int32 roadID)
        {
            try
            {
                dbContext = new PMGSYEntities();

                List<SelectListItem> options = new List<SelectListItem>();

                ////get the Contractor details  from bill master
                //Int32? contractorId = dbContext.ACC_AUTH_REQUEST_MASTER.Where(x => x.AUTH_ID == BILL_ID).Select(c => c.MAST_CON_ID).First();

                ////get the agreement details from bill details
                //Int32? agreementId =
                //                  (from master in dbContext.ACC_AUTH_REQUEST_MASTER
                //                   join details in dbContext.ACC_AUTH_REQUEST_DETAILS
                //                   on master.AUTH_ID equals details.AUTH_ID
                //                   where master.AUTH_ID == BILL_ID
                //                   && master.MAST_CON_ID == contractorId
                //                   select details.IMS_AGREEMENT_CODE).FirstOrDefault();


                ////for contracor and agreemnt final payment is given

                //var query = (from master in dbContext.ACC_AUTH_REQUEST_MASTER
                //             join details in dbContext.ACC_AUTH_REQUEST_DETAILS
                //                 on master.AUTH_ID equals details.AUTH_ID
                //             where
                //             master.MAST_CON_ID == contractorId
                //             && details.IMS_AGREEMENT_CODE == agreementId
                //             && details.IMS_PR_ROAD_CODE == roadID &&
                //             master.AUTH_DATE <= (dbContext.ACC_AUTH_REQUEST_MASTER.Where(m=>m.AUTH_ID == BILL_ID).Select(m=>m.AUTH_DATE).FirstOrDefault())
                //             select new
                //             {
                //                 details.FINAL_PAYMENT

                //             });

                DateTime authDate = (dbContext.ACC_AUTH_REQUEST_MASTER.Where(m => m.AUTH_ID == BILL_ID).Select(m => m.AUTH_DATE).FirstOrDefault());

                var isValid = (from bm in dbContext.ACC_BILL_MASTER
                               join bd in dbContext.ACC_BILL_DETAILS
                               on bm.BILL_ID equals bd.BILL_ID
                               where
                               bd.IMS_PR_ROAD_CODE == roadID &&
                               bm.BILL_DATE <= authDate
                               select new
                               {
                                   bd.FINAL_PAYMENT
                               }).Distinct().ToList();

                //foreach (var item in query)
                //{
                //    if (item.FINAL_PAYMENT.HasValue)
                //    {
                //        if (item.FINAL_PAYMENT.Value)
                //        {
                //            options.Add(new SelectListItem { Selected = true, Text = "Yes", Value = "true" });
                //            break;
                //        }

                //    }
                //}

                if (isValid != null)
                {
                    if (isValid.Any(m => m.FINAL_PAYMENT == true))
                    {
                        options.Add(new SelectListItem { Selected = true, Text = "Yes", Value = "true" });
                    }
                    else
                    {
                        options.Add(new SelectListItem { Text = "Yes", Value = "true" });
                        options.Add(new SelectListItem { Selected = true, Text = "No", Value = "false" });
                    }
                }
                else
                {
                    options.Add(new SelectListItem { Text = "Yes", Value = "true" });
                    options.Add(new SelectListItem { Selected = true, Text = "No", Value = "false" });
                }


                //if no final payment is found 
                if (options.Count == 0)
                {

                    options.Add(new SelectListItem { Text = "Yes", Value = "true" });
                    options.Add(new SelectListItem { Selected = true, Text = "No", Value = "false" });
                }


                return options;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error while getting is final authorization details");
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }

        }

        /// <summary>
        /// DAL function to check whether authorization request is enabled 
        /// </summary>
        /// <param name="adminNdCode"></param>
        /// <returns></returns>
        public bool CheckIfBankRequestEnabled(long adminNdCode)
        {
            try
            {
                dbContext = new PMGSYEntities();

                string bankAuthAllowed = string.Empty;

                bankAuthAllowed = dbContext.ADMIN_DEPARTMENT.Where(c => c.ADMIN_ND_CODE == adminNdCode).Select(c => c.ADMIN_BANK_AUTH_ENABLED).First();

                if (bankAuthAllowed.Equals("Y"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error while checking if authorization is enabled");
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }

        }

        //new method added by Vikram on 29-08-2013
        public bool ValidateDPIUBankAuthDAL(int adminCode)
        {
            dbContext = new PMGSYEntities();
            try
            {
                if (dbContext.ADMIN_DEPARTMENT.Any(m => m.ADMIN_BANK_AUTH_ENABLED == "Y" && m.ADMIN_ND_CODE == adminCode))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }


    }

    public interface IAuthorizationDAL
    {
        Array AuthorizationRequestList(AuthorizationFilter objFilter, out long totalRecords);
        String AddRequestTrackingDetails(ListAutorizationRequestModel model);
        ACC_AUTH_REQUEST_MASTER GetAuthorizationRequestMaster(Int64 authId);
        String AddReceiptDetails(ReceiptDetailsModel receiptModel, ref string message);
        String AddPaymentDetails(PMGSY.Models.Authorization.PaymentDetailsModel paymentModel);
        ACC_BILL_MASTER GetBillMaster(Int64 billId);
        ACC_AUTH_REQUEST_TRACKING GetAuthorizationTrackingDetails(Int64 authId, String authStatus);
        # region authorization request

        Array ListAuthorizationRequestDetails(PaymentFilterModel objFilter, out long totalRecords);
        Int64 AddEditMasterAuthorizationDetails(AuthorizationRequestMasterModel model, string operation, Int64 Auth_Id);
        string GetAuthorizationNumber(short month, short year, int StateCode, int AdminNdCode);
        AmountCalculationModel GetAuthorizationAmountBalance(Int64 authID);
        String DeleteAuthorizationRequest(long auth_id);
        bool CanAuthFinalize(long auth_id);
        ACC_AUTH_REQUEST_MASTER GetMasterAuthorizationDetails(long auth_id);
        ACC_AUTH_REQUEST_DETAILS GetAuthTransactionDetails(long auth_id, int transId);
        String FinlizeAuthorization(long auth_id);
        Array ListAuthDeductionDetailsForDataEntry(PaymentFilterModel objFilter, out long totalRecords);
        Boolean AddEditTransactionDeductionPaymentDetails(AuthorizationRequestDetailsModel model, string operationType, Int64 Bill_id, String AddorEdit, Int16 tranNumber);
        Int32 DeleteAuthorizationTransDetails(Int64 master_Bill_Id, Int32 tranNumber, String paymentDeduction);
        String GetAgreemntNumberForAuthorization(Int64 bill_id);
        List<SelectListItem> GetFinalPaymentDetails(Int64 BILL_ID, Int32 roadID);
        Array ListAuthorizationMasterDetails(PaymentFilterModel objFilter, out long totalRecords);
        bool CheckIfBankRequestEnabled(long adminNdCode);
        # endregion  authorization request

        bool ValidateDPIUBankAuthDAL(int adminCode);
    }
}