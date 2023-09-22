using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using PMGSY.Models;
using PMGSY.Extensions;
using System.Web.Mvc;
//using System.Data.Entity.Core.Objects;
using PMGSY.Models.STAPayment;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using PMGSY.Common;
using System.Transactions;
using System.Data.Entity.Core;

namespace PMGSY.DAL.STAPayment
{
    public class STAPaymentDAL : ISTAPaymentDAL
    {
        Models.PMGSYEntities dbContext;
        public Array GetSTAPaymentListDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int MAST_STATE_CODE, int IMS_YEAR, int IMS_BATCH, int IMS_STREAMS, string ProposalType, decimal TOT_HON_MIN, int PMGSY_SCHEME, out STAPaymentTotalModel model)
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            STAPaymentViewModel staItem;
            try
            {
                dbContext = new Models.PMGSYEntities();
                var ListStaPayment = dbContext.IMS_STA_GET_PAYMENT(MAST_STATE_CODE, IMS_YEAR, IMS_BATCH, IMS_STREAMS, ProposalType, PMGSY_SCHEME).ToList();
                List<STAPaymentViewModel> objListStaPayment = new List<STAPaymentViewModel>();
                STAPaymentTotalModel modelTotal = new STAPaymentTotalModel();
                /// Second List Created to Calculate Sum and Avoid Exception
                decimal? TOTAL_SCRUTNIZED_AMOUNT = ListStaPayment.Sum(a => a.SANCTION_AMOUNT);
                foreach (IMS_STA_GET_PAYMENT_Result item in ListStaPayment)
                {
                    staItem = new STAPaymentViewModel();
                    staItem.MAST_STATE_NAME = item.MAST_STATE_NAME;
                    staItem.IMS_YEAR = item.IMS_YEAR;
                    staItem.SANCTION_AMOUNT = Math.Round(Convert.ToDecimal(item.SANCTION_AMOUNT), 2);
                    staItem.PER_TOT_VALUE = Math.Round(Convert.ToDecimal(((item.SANCTION_AMOUNT * 100) / TOTAL_SCRUTNIZED_AMOUNT)), 2);
                    staItem.STA_SANCTIONED_BY = item.STA_SANCTIONED_BY;
                    staItem.INSTITUTE_NAME = item.INSTITUTE_NAME;
                    staItem.TOTAL_SCRUTNIZED_AMOUNT = TOTAL_SCRUTNIZED_AMOUNT;
                    staItem.HON_AMOUNT = Math.Round(TOT_HON_MIN * staItem.PER_TOT_VALUE, 2);
                    staItem.PMGSY_SCHEME = PMGSY_SCHEME;
                    staItem.STA_SERVICE_TAX_NO = item.SERVICE_TAX_NO;
                    objListStaPayment.Add(staItem);
                }

                if (objListStaPayment != null)
                {
                    modelTotal.TOTAL_HON_AMOUNT = objListStaPayment.Sum(m => m.HON_AMOUNT);
                    modelTotal.TOTAL_PER_TOT_VALUE = objListStaPayment.Sum(m => m.PER_TOT_VALUE);
                    modelTotal.TOTAL_SANCTION_AMOUNT = objListStaPayment.Sum(m => m.SANCTION_AMOUNT).HasValue ? objListStaPayment.Sum(m => m.SANCTION_AMOUNT).Value : 0;

                    modelTotal.DIS_TOTAL_HON_AMOUNT = objCommonFunctions._IndianFormatAmount(modelTotal.TOTAL_HON_AMOUNT.ToString());
                    modelTotal.DIS_TOTAL_PER_TOT_VALUE = objCommonFunctions._IndianFormatAmount(modelTotal.TOTAL_PER_TOT_VALUE.ToString());
                    modelTotal.DIS_TOTAL_SANCTION_AMOUNT = objCommonFunctions._IndianFormatAmount(modelTotal.TOTAL_SANCTION_AMOUNT.ToString());


                }

                model = modelTotal;
                totalRecords = objListStaPayment.Count();
                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        objListStaPayment = objListStaPayment.OrderBy(x => x.STA_SANCTIONED_BY).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                    else
                    {
                        objListStaPayment = objListStaPayment.OrderByDescending(x => x.STA_SANCTIONED_BY).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                }
                return objListStaPayment.Select(staListItem => new
                {
                    cell = new[] {                                                                                 
                            staListItem.INSTITUTE_NAME,  
                            staListItem.STA_SERVICE_TAX_NO,
                            objCommonFunctions._IndianFormatAmount(staListItem.SANCTION_AMOUNT==null?"0":staListItem.SANCTION_AMOUNT.ToString()),
                             objCommonFunctions._IndianFormatAmount(staListItem.PER_TOT_VALUE.ToString()),
                             objCommonFunctions._IndianFormatAmount(staListItem.HON_AMOUNT.ToString()),                            
                            "<a href='#'  class='ui-icon ui-icon-zoomin ui-align-center' onClick='ListInvoiceDetails(\"" + MAST_STATE_CODE + "\",\"" + IMS_YEAR + "\",\""  + IMS_BATCH + "\",\"" + IMS_STREAMS + "\",\"" + ProposalType + "\",\"" + staListItem.STA_SANCTIONED_BY + "\",\"" + staListItem.INSTITUTE_NAME  + "\",\"" + staListItem.HON_AMOUNT+ "\",\"" + PMGSY_SCHEME + "\"); return false;'>Show Details</a>",                                    
                   }
                }).ToArray();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                model = null;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array GetSTAInvoiceListDAL(int page, int rows, string sidx, string sord, out int totalRecords, int MAST_STATE_CODE, Int16 IMS_YEAR, int IMS_BATCH, int IMS_STREAMS, string IMS_PROPOSAL_TYPE, string STA_SANCTIONED_BY, string STA_INSTITUTE_NAME, decimal HON_AMOUNT, int PMGSY_SCHEME, out STAPaymentTotalViewModel model)
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            STAPaymentTotalViewModel modelTotal = new STAPaymentTotalViewModel();
            try
            {
                dbContext = new Models.PMGSYEntities();

                decimal BalanceAmount = 0;
                var ListStaInvoiceModel = (from a in dbContext.IMS_GENERATED_INVOICE
                                           where
                                               a.MAST_STATE_CODE == MAST_STATE_CODE &&
                                               a.IMS_YEAR == IMS_YEAR &&
                                               a.IMS_BATCH == IMS_BATCH &&
                                               a.IMS_STREAM == IMS_STREAMS &&
                                               // a.IMS_PROPOSAL_TYPE == (IMS_PROPOSAL_TYPE == "A" ? "%" : IMS_PROPOSAL_TYPE) &&
                                               (IMS_PROPOSAL_TYPE == "A" ? "%" : a.IMS_PROPOSAL_TYPE) == (IMS_PROPOSAL_TYPE == "A" ? "%" : IMS_PROPOSAL_TYPE) &&
                                               a.STA_SANCTIONED_BY == STA_SANCTIONED_BY &&
                                               a.MAST_PMGSY_SCHEME == PMGSY_SCHEME
                                           select
                                           a).ToList();

                if (dbContext.IMS_GENERATED_INVOICE.Where(
                                                            a => a.MAST_STATE_CODE == MAST_STATE_CODE &&
                                                            a.IMS_YEAR == IMS_YEAR &&
                                                            a.IMS_BATCH == IMS_BATCH &&
                                                            a.IMS_STREAM == IMS_STREAMS &&
                                                                //a.IMS_PROPOSAL_TYPE == (IMS_PROPOSAL_TYPE == "A" ? "%" : IMS_PROPOSAL_TYPE) &&
                                                            (IMS_PROPOSAL_TYPE == "A" ? "%" : a.IMS_PROPOSAL_TYPE) == (IMS_PROPOSAL_TYPE == "A" ? "%" : IMS_PROPOSAL_TYPE) &&
                                                            a.STA_SANCTIONED_BY == STA_SANCTIONED_BY &&
                                                            a.MAST_PMGSY_SCHEME == PMGSY_SCHEME
                                                         ).Any())
                {
                    BalanceAmount = BalanceAmount - dbContext.IMS_GENERATED_INVOICE.Where(
                                                            a => a.MAST_STATE_CODE == MAST_STATE_CODE &&
                                                            a.IMS_YEAR == IMS_YEAR &&
                                                            a.IMS_BATCH == IMS_BATCH &&
                                                            a.IMS_STREAM == IMS_STREAMS &&
                                                            a.MAST_PMGSY_SCHEME == PMGSY_SCHEME &&
                                                                // a.IMS_PROPOSAL_TYPE == (IMS_PROPOSAL_TYPE == "A" ? "%" : IMS_PROPOSAL_TYPE) &&
                                                           (IMS_PROPOSAL_TYPE == "A" ? "%" : a.IMS_PROPOSAL_TYPE) == (IMS_PROPOSAL_TYPE == "A" ? "%" : IMS_PROPOSAL_TYPE) &&
                                                            a.STA_SANCTIONED_BY == STA_SANCTIONED_BY).Sum(a => a.TOTAL_AMOUNT);
                }
                else
                {
                    BalanceAmount = HON_AMOUNT;
                }
                totalRecords = ListStaInvoiceModel.Count();
                if (ListStaInvoiceModel != null)
                {
                    modelTotal.TOTAL_HONORARIUM_AMOUNT = ListStaInvoiceModel.Sum(m => m.HONORARIUM_AMOUNT);
                    modelTotal.TOTAL_PENALTY_AMOUNT = ListStaInvoiceModel.Sum(m => m.PENALTY_AMOUNT);
                    modelTotal.TOTAL_TDS_AMOUNT = ListStaInvoiceModel.Sum(m => m.TDS_AMOUNT);
                    modelTotal.TOTAL_SC_AMOUNT = ListStaInvoiceModel.Sum(m => m.SC_AMOUNT);
                    modelTotal.TOTAL_AMOUNT = ListStaInvoiceModel.Sum(m => m.TOTAL_AMOUNT);
                    modelTotal.TOTAL_SERVICE_TAX_AMOUNT = ListStaInvoiceModel.Sum(m => m.SERVICE_TAX_AMOUNT);

                    modelTotal.DIS_TOTAL_HONORARIUM_AMOUNT = objCommonFunctions._IndianFormatAmount(modelTotal.TOTAL_HONORARIUM_AMOUNT.ToString());
                    modelTotal.DIS_TOTAL_PENALTY_AMOUNT = objCommonFunctions._IndianFormatAmount(modelTotal.TOTAL_PENALTY_AMOUNT.ToString());
                    modelTotal.DIS_TOTAL_TDS_AMOUNT = objCommonFunctions._IndianFormatAmount(modelTotal.TOTAL_TDS_AMOUNT.ToString());
                    modelTotal.DIS_TOTAL_SC_AMOUNT = objCommonFunctions._IndianFormatAmount(modelTotal.TOTAL_SC_AMOUNT.ToString());
                    modelTotal.DIS_TOTAL_SERVICE_TAX_AMOUNT = objCommonFunctions._IndianFormatAmount(modelTotal.TOTAL_SERVICE_TAX_AMOUNT.ToString());
                    modelTotal.DIS_TOTAL_AMOUNT = objCommonFunctions._IndianFormatAmount(Math.Round(modelTotal.TOTAL_AMOUNT, MidpointRounding.AwayFromZero).ToString("0.00")); // change on 18/07/2014 by deepak round up total
                }

                model = modelTotal;
                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        ListStaInvoiceModel = ListStaInvoiceModel.OrderBy(x => x.STA_SANCTIONED_BY).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                    else
                    {
                        ListStaInvoiceModel = ListStaInvoiceModel.OrderByDescending(x => x.STA_SANCTIONED_BY).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                }
                return ListStaInvoiceModel.Select(staListItem => new
                {
                    cell = new[] {                                                                                                             
                            staListItem.SAS_ABBREVATION.ToString(), 
                            objCommonFunctions._IndianFormatAmount(staListItem.HONORARIUM_AMOUNT.ToString()),                    
                            staListItem.INVOICE_FILE_NO == null?"":staListItem.INVOICE_FILE_NO.ToString(),                           
                            objCommonFunctions._IndianFormatAmount(staListItem.PENALTY_AMOUNT.ToString()),                    
                            objCommonFunctions._IndianFormatAmount(staListItem.TDS_AMOUNT.ToString()),                       
                            objCommonFunctions._IndianFormatAmount(staListItem.SC_AMOUNT.ToString()),
                            objCommonFunctions._IndianFormatAmount(staListItem.SERVICE_TAX_AMOUNT.ToString()),
                            objCommonFunctions._IndianFormatAmount(Math.Round(staListItem.TOTAL_AMOUNT,MidpointRounding.AwayFromZero).ToString("0.00")),
                            staListItem.GENERATION_DATE.ToString("dd-MMM-yyyy"),
                            "<a href='#'  class='ui-icon ui-icon-zoomin ui-align-center' onClick='ShowDetails(\"" + staListItem.IMS_INVOICE_ID + "\"); return false;'>Show Details</a>",
                            "<a href='#'  class='ui-icon ui-icon-trash ui-align-center' onClick='DeleteDetails(\"" + staListItem.IMS_INVOICE_ID + "\"); return false;'>Delete</a>",        
                            objCommonFunctions._IndianFormatAmount(BalanceAmount.ToString())                  

                   }
                }).ToArray();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                model = null;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public string AddStaInvoiceDetailsDAL(IMS_GENERATED_INVOICE ims_generated_invoice)
        {
            try
            {
                dbContext = new Models.PMGSYEntities();
                Int64? MaxID;
                if (!dbContext.IMS_GENERATED_INVOICE.Any())
                {
                    MaxID = 0;
                }
                else
                {
                    MaxID = (from c in dbContext.IMS_GENERATED_INVOICE select (Int32?)c.IMS_INVOICE_ID ?? 0).Max();
                }
                ++MaxID;
                ims_generated_invoice.IMS_INVOICE_ID = Convert.ToInt64(MaxID);

                //added by abhishek kamble 28-nov-2013
                ims_generated_invoice.USERID = PMGSYSession.Current.UserId;
                ims_generated_invoice.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                //ims_generated_invoice.MAST_PMGSY_SCHEME = PMGSYSession.Current.PMGSYScheme; //new change done by Deepak  on 9 June 2014
                dbContext.IMS_GENERATED_INVOICE.Add(ims_generated_invoice);
                dbContext.SaveChanges();
                return string.Empty;
            }
            catch (DbUpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return ("An Error Occurred While Processing Your Request.");
            }
            catch (OptimisticConcurrencyException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return ("An Error Occurred While Processing Your Request.");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return ("Error Occurred While Processing Request.");
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        public STAInvoiceViewModel StaPaymentReportDAL(int invoiceId)
        {
            dbContext = new Models.PMGSYEntities();
            STAInvoiceViewModel staInvoiceModel = new STAInvoiceViewModel();
            CommonFunctions objCommonFunctions = new CommonFunctions();
            try
            {
                var StaInvoiceDetails = (from a in dbContext.IMS_GENERATED_INVOICE
                                         where a.IMS_INVOICE_ID == invoiceId
                                         select a).FirstOrDefault();

                if (StaInvoiceDetails != null)
                {

                    staInvoiceModel.TOTAL_AMOUNT = Math.Round(StaInvoiceDetails.TOTAL_AMOUNT, MidpointRounding.AwayFromZero);
                    staInvoiceModel.TOTAL_AMOUNT_IND_FORMAT = objCommonFunctions._IndianFormatAmount(Math.Round(StaInvoiceDetails.TOTAL_AMOUNT, MidpointRounding.AwayFromZero).ToString());
                    staInvoiceModel.TOTAL_AMOUNT_WORDS = new AmountToWord().RupeesToWord(Math.Round(staInvoiceModel.TOTAL_AMOUNT, 2).ToString("0.00"));

                    staInvoiceModel.HONORARIUM_AMOUNT = StaInvoiceDetails.HONORARIUM_AMOUNT;
                    staInvoiceModel.HONORARIUM_AMOUNT_IND_FORMAT = objCommonFunctions._IndianFormatAmount(StaInvoiceDetails.HONORARIUM_AMOUNT.ToString());

                    staInvoiceModel.STA_SANCTIONED_BY = StaInvoiceDetails.STA_SANCTIONED_BY;
                    staInvoiceModel.SAS_ABBREVATION = StaInvoiceDetails.SAS_ABBREVATION;

                    staInvoiceModel.PENALTY_AMOUNT = StaInvoiceDetails.PENALTY_AMOUNT;
                    staInvoiceModel.PENALTY_AMOUNT_IND_FORMAT = objCommonFunctions._IndianFormatAmount(StaInvoiceDetails.PENALTY_AMOUNT.ToString());

                    staInvoiceModel.TDS_AMOUNT = StaInvoiceDetails.TDS_AMOUNT;
                    staInvoiceModel.TDS_AMOUNT_IND_FORMAT = objCommonFunctions._IndianFormatAmount(StaInvoiceDetails.TDS_AMOUNT.ToString());

                    staInvoiceModel.SC_AMOUNT = StaInvoiceDetails.SC_AMOUNT;
                    staInvoiceModel.SC_AMOUNT_IND_FORMAT = objCommonFunctions._IndianFormatAmount(StaInvoiceDetails.SC_AMOUNT.ToString());

                    staInvoiceModel.INVOICE_GEN_DATE = StaInvoiceDetails.GENERATION_DATE.ToString("dd-MMM-yyyy");
                    staInvoiceModel.Invoice_Generate_DATE = StaInvoiceDetails.GENERATION_DATE.ToString("dd/MM/yyyy");
                    staInvoiceModel.INVOICE_FILE_NO = StaInvoiceDetails.INVOICE_FILE_NO;
                    return staInvoiceModel;
                }
                else
                {
                    return null;
                }

            }
            catch (DbUpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return null;
            }
            catch (OptimisticConcurrencyException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return null;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }

        }

        #region STA Payment
        public Array GetSTAPaymenInoviceListDAL(int page, int rows, string sidx, string sord, out int totalRecords, int MAST_STATE_CODE, int IMS_YEAR, int IMS_BATCH, int IMS_STREAMS, string IMS_PROPOSAL_TYPE, int Scheme)
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            STAPaymentTotalViewModel modelTotal = new STAPaymentTotalViewModel();
            try
            {
                dbContext = new Models.PMGSYEntities();

                decimal BalanceAmount = 0;
                var ListStaInvoiceModel = (from a in dbContext.IMS_GENERATED_INVOICE
                                           where
                                               a.MAST_STATE_CODE == MAST_STATE_CODE &&
                                               a.IMS_YEAR == IMS_YEAR &&
                                               a.IMS_BATCH == IMS_BATCH &&
                                               a.IMS_STREAM == IMS_STREAMS &&
                                               //(IMS_PROPOSAL_TYPE == "0" ? "%" : a.IMS_PROPOSAL_TYPE) == (IMS_PROPOSAL_TYPE == "0" ? "%" : IMS_PROPOSAL_TYPE) &&
                                               a.IMS_PROPOSAL_TYPE==IMS_PROPOSAL_TYPE &&
                                               a.MAST_PMGSY_SCHEME == Scheme
                                           //&&
                                           // a.STA_SANCTIONED_BY == STA_SANCTIONED_BY
                                           select
                                           a).ToList();


                totalRecords = ListStaInvoiceModel.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        ListStaInvoiceModel = ListStaInvoiceModel.OrderBy(x => x.STA_SANCTIONED_BY).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                    else
                    {
                        ListStaInvoiceModel = ListStaInvoiceModel.OrderByDescending(x => x.STA_SANCTIONED_BY).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                }
                return ListStaInvoiceModel.Select(staListItem => new
                {
                    cell = new[] {                                                                                                             
                            staListItem.SAS_ABBREVATION.ToString(), 
                            objCommonFunctions._IndianFormatAmount(staListItem.HONORARIUM_AMOUNT.ToString()),                    
                            staListItem.INVOICE_FILE_NO == null?"":staListItem.INVOICE_FILE_NO.ToString(),                           
                            objCommonFunctions._IndianFormatAmount(staListItem.PENALTY_AMOUNT.ToString()),                    
                            objCommonFunctions._IndianFormatAmount(staListItem.TDS_AMOUNT.ToString()),                       
                            objCommonFunctions._IndianFormatAmount(staListItem.SC_AMOUNT.ToString()),                    
                            objCommonFunctions._IndianFormatAmount(Math.Round(staListItem.TOTAL_AMOUNT,MidpointRounding.AwayFromZero).ToString("0.00")),
                            staListItem.GENERATION_DATE.ToString("dd-MMM-yyyy"),
                            "<a href='#'  class='ui-icon ui-icon-zoomin ui-align-center' onClick='AddStaPaymentDetail(\"" + URLEncrypt.EncryptParameters1(new string[]{"IMS_INVOICE_ID="+staListItem.IMS_INVOICE_ID.ToString().Trim()}) + "\"); return false;'>View Payment Details</a>",                                    
                            objCommonFunctions._IndianFormatAmount(BalanceAmount.ToString())
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

        public Array GetSTAPaymentListDAL(int invoiceCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                var lstPayment = (from payment in dbContext.IMS_STA_PAYMENTS
                                  where
                                  payment.IMS_INVOICE_ID == invoiceCode

                                  select new
                                  {
                                      payment.IMS_PAYMENT_ID,
                                      payment.IMS_INVOICE_ID,
                                      payment.IMS_NEFT_CHEQUE_NUMBER,
                                      payment.IMS_NEFT_CHEQUE_PAYMENT,
                                      payment.IMS_PAYMENT_DATE,
                                      payment.IMS_ENTRY_DATE,
                                      payment.IMS_PAYMENT_FINALIZE
                                  }).Distinct();
                totalRecords = lstPayment.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {

                            case "IMS_NEFT_CHEQUE_PAYMENT":
                                lstPayment = lstPayment.OrderBy(m => m.IMS_NEFT_CHEQUE_PAYMENT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstPayment = lstPayment.OrderBy(m => m.IMS_PAYMENT_ID).ThenBy(x => x.IMS_NEFT_CHEQUE_PAYMENT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {

                            case "IMS_NEFT_CHEQUE_PAYMENT":
                                lstPayment = lstPayment.OrderByDescending(m => m.IMS_NEFT_CHEQUE_PAYMENT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstPayment = lstPayment.OrderByDescending(m => m.IMS_PAYMENT_ID).ThenBy(x => x.IMS_NEFT_CHEQUE_PAYMENT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                }
                else
                {
                    lstPayment = lstPayment.OrderByDescending(m => m.IMS_PAYMENT_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var result = lstPayment.Select(pay => new
                {
                    pay.IMS_PAYMENT_ID,
                    pay.IMS_INVOICE_ID,
                    pay.IMS_NEFT_CHEQUE_NUMBER,
                    pay.IMS_NEFT_CHEQUE_PAYMENT,
                    pay.IMS_PAYMENT_DATE,
                    pay.IMS_ENTRY_DATE,
                    pay.IMS_PAYMENT_FINALIZE

                }).ToArray();

                return result.Select(m => new
                {
                    id = URLEncrypt.EncryptParameters1(new string[] { "IMS_INVOICE_ID=" + m.IMS_INVOICE_ID.ToString().Trim(), "IMS_PAYMENT_ID =" + m.IMS_PAYMENT_ID.ToString().Replace("/", "") }),
                    cell = new[] 
                    {                      
                        m.IMS_NEFT_CHEQUE_PAYMENT=="N"?"NEFT":"Cheque",
                        m.IMS_NEFT_CHEQUE_NUMBER.ToString(),
                        m.IMS_PAYMENT_DATE==null?"":m.IMS_PAYMENT_DATE.ToString("dd/MM/yyyy"),
                        m.IMS_ENTRY_DATE==null?"":m.IMS_ENTRY_DATE.ToString("dd/MM/yyyy"),                      
                       m.IMS_PAYMENT_FINALIZE=="N"?
                                    "<center><a href='#' class='ui-icon ui-icon-pencil' onclick='EditSTAPayment(\"" + URLEncrypt.EncryptParameters1(new string[] { "IMS_INVOICE_ID=" + m.IMS_INVOICE_ID.ToString().Trim(), "IMS_PAYMENT_ID =" + m.IMS_PAYMENT_ID.ToString().Replace("/", "") })+ "\"); return false;'>Edit</a></center>"
                                    :"<center><table><tr><td style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>",
                        m.IMS_PAYMENT_FINALIZE=="N"?
                        "<center><a href='#' class='ui-icon ui-icon-trash' onclick='DeleteSTAPayment(\"" + URLEncrypt.EncryptParameters1(new string[] { "IMS_INVOICE_ID=" + m.IMS_INVOICE_ID.ToString().Trim(), "IMS_PAYMENT_ID =" + m.IMS_PAYMENT_ID.ToString().Replace("/", "") }) + "\"); return false;'>Delete</a></center>"
                        :"<center><table><tr><td style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>",
                          m.IMS_PAYMENT_FINALIZE=="N"?
                        "<center><a href='#' class='ui-icon ui-icon-unlocked' onclick='FinalizeSTAPayment(\"" + URLEncrypt.EncryptParameters1(new string[] { "IMS_INVOICE_ID=" + m.IMS_INVOICE_ID.ToString().Trim(), "IMS_PAYMENT_ID =" + m.IMS_PAYMENT_ID.ToString().Replace("/", "") }) + "\"); return false;'>Finalize</a></center>"
                        :"<center><table><tr><td style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>",
                          m.IMS_PAYMENT_FINALIZE=="Y"?
                        "<center><a href='#' class='ui-icon ui-icon-unlocked' onclick='DeFinalizeSTAPayment(\"" + URLEncrypt.EncryptParameters1(new string[] { "IMS_INVOICE_ID=" + m.IMS_INVOICE_ID.ToString().Trim(), "IMS_PAYMENT_ID =" + m.IMS_PAYMENT_ID.ToString().Replace("/", "") }) + "\"); return false;'>DeFinalize</a></center>"
                        :"<center><table><tr><td style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>",
                     }
                }).ToArray();
            }
            catch
            {
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        /// <summary>
        /// Add STA Payment details
        /// </summary>
        /// <param name="staPaymentViewModel"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool AddSTAPaymentDetailsDAL(STAPayemntInvoiceModel staPaymentViewModel, ref string message)
        {
            try
            {

                IMS_STA_PAYMENTS imsSTAPaymentModel = new IMS_STA_PAYMENTS();
                imsSTAPaymentModel.IMS_PAYMENT_ID = getMaxStaPaymentCode();
                imsSTAPaymentModel.IMS_INVOICE_ID = staPaymentViewModel.IMS_INVOICE_CODE;
                imsSTAPaymentModel.IMS_NEFT_CHEQUE_NUMBER = staPaymentViewModel.IMS_NEFT_CHEQUE_NUMBER;
                imsSTAPaymentModel.IMS_NEFT_CHEQUE_PAYMENT = staPaymentViewModel.Payment_Type;
                imsSTAPaymentModel.IMS_PAYMENT_DATE = DateTime.ParseExact(staPaymentViewModel.IMS_Payment_DATE, "dd/MM/yyyy", null); 
                imsSTAPaymentModel.IMS_ENTRY_DATE = DateTime.Now;
                imsSTAPaymentModel.IMS_PAYMENT_FINALIZE = "N";
                imsSTAPaymentModel.USERID = PMGSYSession.Current.UserId;
                imsSTAPaymentModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                dbContext = new PMGSYEntities();
                dbContext.IMS_STA_PAYMENTS.Add(imsSTAPaymentModel);
                dbContext.SaveChanges();

                return true;
            }
            catch (DbUpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "An error occured while processing your request.";
                return false;
            }
            catch (OptimisticConcurrencyException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "An error occured while processing your request.";
                return false;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "An error occured while processing your request.";
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

        /// <summary>
        /// Calculate Max STAPayment.
        /// </summary>
        /// <returns></returns>
        public int getMaxStaPaymentCode()
        {
            try
            {
                dbContext = new PMGSYEntities();

                int maxCode = 1;

                if (dbContext.IMS_STA_PAYMENTS.Any())
                {
                    maxCode = dbContext.IMS_STA_PAYMENTS.Max(s => s.IMS_PAYMENT_ID) + 1;
                    return maxCode;
                }
                else
                {
                    return maxCode;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return 1;
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
        /// Get Additional Cost Details.
        /// </summary>
        /// <param name="PayemntCode"></param>
        /// <param name="IMSInvoiceCode"></param>
        /// <returns></returns>
        public STAPayemntInvoiceModel EditSTAPaymentDetailsDAL(int PayemntCode, int IMSInvoiceCode)
        {

            try
            {
                dbContext = new PMGSYEntities();
                STAPayemntInvoiceModel staPaymentViewtModel = new STAPayemntInvoiceModel();
                IMS_STA_PAYMENTS staPaymentModel = dbContext.IMS_STA_PAYMENTS.Where(m => m.IMS_PAYMENT_ID == PayemntCode && m.IMS_INVOICE_ID == IMSInvoiceCode).FirstOrDefault();
                STAInvoiceViewModel staInvViewModel = StaPaymentReportDAL(IMSInvoiceCode);

                if (staPaymentModel != null)
                {
                    staPaymentViewtModel.EncryptedIMS_Payment_CODE = URLEncrypt.EncryptParameters1(new string[] { "IMS_INVOICE_ID=" + IMSInvoiceCode.ToString().Trim(), "IMS_PAYMENT_ID =" + PayemntCode.ToString().Replace("/", "") });
                    staPaymentViewtModel.EncryptedIMS_Invoice_Code = URLEncrypt.EncryptParameters1(new string[] { "IMS_INVOICE_ID=" + IMSInvoiceCode.ToString().Trim() });
                    staPaymentViewtModel.IMS_NEFT_CHEQUE_NUMBER = staPaymentModel.IMS_NEFT_CHEQUE_NUMBER;
                    staPaymentViewtModel.IMS_Payment_DATE = staPaymentModel.IMS_PAYMENT_DATE.ToString("dd/MM/yyyy");
                    staPaymentViewtModel.Invoice_Generate_DATE = staInvViewModel.Invoice_Generate_DATE;
                    
                    staPaymentViewtModel.Payment_Type = staPaymentModel.IMS_NEFT_CHEQUE_PAYMENT;
                }
                return staPaymentViewtModel;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
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
        /// Update STA Payment details.
        /// </summary>
        /// <param name="staPaymentViewModel"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool UpdateSTAPaymentDetailsDAL(STAPayemntInvoiceModel staPaymentViewModel, ref string message)
        {

            try
            {
                Dictionary<string, string> decryptedParameters = null;
                string[] encryptedParameter = null;
                dbContext = new PMGSYEntities();
                int IMS_Payment_Code = 0;
                int IMS_Invoice_Code = 0;

                encryptedParameter = staPaymentViewModel.EncryptedIMS_Payment_CODE.Split('/');

                if (!(encryptedParameter.Length == 3))
                {
                    return false;
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedParameter[0], encryptedParameter[1], encryptedParameter[2] });
                IMS_Invoice_Code = Convert.ToInt32(decryptedParameters["IMS_INVOICE_ID"].ToString());
                IMS_Payment_Code = Convert.ToInt32(decryptedParameters["IMS_PAYMENT_ID"].ToString());

                IMS_STA_PAYMENTS imsSTAPaymentOrignalModel = dbContext.IMS_STA_PAYMENTS.Where(m => m.IMS_PAYMENT_ID == IMS_Payment_Code && m.IMS_INVOICE_ID == IMS_Invoice_Code).FirstOrDefault();

                if (imsSTAPaymentOrignalModel == null)
                {
                    return false;
                }
                imsSTAPaymentOrignalModel.IMS_NEFT_CHEQUE_NUMBER = staPaymentViewModel.IMS_NEFT_CHEQUE_NUMBER;
                imsSTAPaymentOrignalModel.IMS_NEFT_CHEQUE_PAYMENT = staPaymentViewModel.Payment_Type;
                imsSTAPaymentOrignalModel.IMS_PAYMENT_DATE = DateTime.ParseExact(staPaymentViewModel.IMS_Payment_DATE, "dd/MM/yyyy", null);
                imsSTAPaymentOrignalModel.IMS_ENTRY_DATE = DateTime.Now;
                imsSTAPaymentOrignalModel.USERID = PMGSYSession.Current.UserId;
                imsSTAPaymentOrignalModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                dbContext.Entry(imsSTAPaymentOrignalModel).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                return true;

            }
            catch (DbUpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "An error ocuured while processing your request.";
                return false;
            }
            catch (OptimisticConcurrencyException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "An error ocuured while processing your request.";
                return false;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "An error ocuured while processing your request.";
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

        /// <summary>
        /// Delete STAPayment details.
        /// </summary>
        /// <param name="paymentCode"></param>
        /// <param name="imsInvoiceCode"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool DeleteSTAPaymentDetailsDAL(int paymentCode, int imsInvoiceCode, ref string message)
        {

            try
            {
                dbContext = new PMGSYEntities();

                IMS_STA_PAYMENTS staPaymentModel = dbContext.IMS_STA_PAYMENTS.Where(m => m.IMS_PAYMENT_ID == paymentCode && m.IMS_INVOICE_ID == imsInvoiceCode).FirstOrDefault();
                if (staPaymentModel == null)
                {
                    message = "Record not exist for delete.";
                    return false;
                }

                staPaymentModel.USERID = PMGSYSession.Current.UserId;
                staPaymentModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                dbContext.Entry(staPaymentModel).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                dbContext.IMS_STA_PAYMENTS.Remove(staPaymentModel);
                dbContext.SaveChanges();
                return true;
            }
            catch (DbUpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "An error occured while processing your request.";
                return false;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "An error occured while processing your request.";
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

        /// <summary>
        /// Update STA Payment details.
        /// </summary>
        /// <param name="staPaymentViewModel"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool FinalizeSTAPaymentDetailsDAL(int paymentCode, int imsInvoiceCode, ref string message)
        {

            try
            {             
               
                dbContext = new PMGSYEntities();
                IMS_STA_PAYMENTS staPaymentModel = dbContext.IMS_STA_PAYMENTS.Where(m => m.IMS_PAYMENT_ID == paymentCode && m.IMS_INVOICE_ID == imsInvoiceCode).FirstOrDefault();

                if (staPaymentModel == null)
                {
                    return false;
                }
                staPaymentModel.IMS_PAYMENT_FINALIZE = "Y";
                staPaymentModel.USERID = PMGSYSession.Current.UserId;
                staPaymentModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                dbContext.Entry(staPaymentModel).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                return true;

            }
            catch (DbUpdateException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "An error ocuured while processing your request.";
                return false;
            }
            catch (OptimisticConcurrencyException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "An error ocuured while processing your request.";
                return false;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "An error ocuured while processing your request.";
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
        #endregion
    }
    public interface ISTAPaymentDAL
    {
        Array GetSTAPaymentListDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int MAST_STATE_CODE, int IMS_YEAR, int IMS_BATCH, int IMS_STREAMS, string ProposalType, decimal TOT_HON_MIN, int PMGSY_SCHEME, out STAPaymentTotalModel model);
        Array GetSTAInvoiceListDAL(int page, int rows, string sidx, string sord, out int totalRecords, int MAST_STATE_CODE, Int16 IMS_YEAR, int IMS_BATCH, int IMS_STREAMS, string IMS_PROPOSAL_TYPE, string STA_SANCTIONED_BY, string STA_INSTITUTE_NAME, decimal HON_AMOUNT, int PMGSY_SCHEME, out STAPaymentTotalViewModel model);
        string AddStaInvoiceDetailsDAL(IMS_GENERATED_INVOICE ims_generated_invoice);
        STAInvoiceViewModel StaPaymentReportDAL(int invoiceId);
        #region STA Payment
        Array GetSTAPaymenInoviceListDAL(int page, int rows, string sidx, string sord, out int totalRecords, int MAST_STATE_CODE, int IMS_YEAR, int IMS_BATCH, int IMS_STREAMS, string IMS_PROPOSAL_TYPE, int Scheme);
        Array GetSTAPaymentListDAL(int invoiceCode, int page, int rows, string sidx, string sord, out long totalRecords);
        bool AddSTAPaymentDetailsDAL(STAPayemntInvoiceModel staPaymentViewModel, ref string message);
        STAPayemntInvoiceModel EditSTAPaymentDetailsDAL(int PayemntCode, int IMSInvoiceCode);
        bool UpdateSTAPaymentDetailsDAL(STAPayemntInvoiceModel staPaymentViewModel, ref string message);
        bool DeleteSTAPaymentDetailsDAL(int paymentCode, int imsInvoiceCode, ref string message);
        bool FinalizeSTAPaymentDetailsDAL(int paymentCode, int imsInvoiceCode, ref string message);
        #endregion
    }
}