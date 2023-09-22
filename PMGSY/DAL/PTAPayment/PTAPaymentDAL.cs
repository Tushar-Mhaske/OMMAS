using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.Models;
using PMGSY.Extensions;
using System.Web.Mvc;
using System.Data.Entity.Core.Objects;
using PMGSY.Models.PTAPayment;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using PMGSY.Common;
using System.Transactions;
using System.Data.Entity.Core;

namespace PMGSY.DAL.PTAPayment
{
    public class PTAPaymentDAL : IPTAPaymentDAL
    {
        Models.PMGSYEntities dbContext;
        public Array GetPTAPaymentListDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int MAST_STATE_CODE, int IMS_YEAR, int IMS_BATCH, int IMS_STREAMS, string ProposalType, decimal TOT_HON_MIN, int PMGSY_SCHEME, out PTAPaymentTotalModel model)
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            PTAPaymentViewModel ptaItem;
            try
            {
                dbContext = new Models.PMGSYEntities();
                var ListPtaPayment = dbContext.IMS_PTA_GET_PAYMENT(MAST_STATE_CODE, IMS_YEAR, IMS_BATCH, IMS_STREAMS, ProposalType, PMGSY_SCHEME).ToList();
                List<PTAPaymentViewModel> objListPtaPayment = new List<PTAPaymentViewModel>();
                PTAPaymentTotalModel modelTotal = new PTAPaymentTotalModel();
                /// Second List Created to Calculate Sum and Avoid Exception
                decimal? TOTAL_SCRUTNIZED_AMOUNT = ListPtaPayment.Sum(a => a.SANCTION_AMOUNT);
                foreach (IMS_PTA_GET_PAYMENT_Result item in ListPtaPayment)
                {
                    ptaItem = new PTAPaymentViewModel();
                    ptaItem.MAST_STATE_NAME = item.MAST_STATE_NAME;
                    ptaItem.IMS_YEAR = item.IMS_YEAR;
                    ptaItem.SANCTION_AMOUNT = Math.Round(Convert.ToDecimal(item.SANCTION_AMOUNT), 2);
                    ptaItem.PER_TOT_VALUE = Math.Round(Convert.ToDecimal(((item.SANCTION_AMOUNT * 100) / TOTAL_SCRUTNIZED_AMOUNT)), 2);
                    ptaItem.PTA_SANCTIONED_BY = item.PTA_SANCTIONED_BY.ToString();
                    ptaItem.INSTITUTE_NAME = item.INSTITUTE_NAME;
                    ptaItem.TOTAL_SCRUTNIZED_AMOUNT = TOTAL_SCRUTNIZED_AMOUNT;
                    ptaItem.HON_AMOUNT = Math.Round(TOT_HON_MIN * ptaItem.PER_TOT_VALUE, 2);
                    ptaItem.PMGSY_SCHEME = PMGSY_SCHEME;
                    ptaItem.PTA_SERVICE_TAX_NO = item.SERVICE_TAX_NO;
                    objListPtaPayment.Add(ptaItem);
                }

                if (objListPtaPayment != null)
                {
                    modelTotal.TOTAL_HON_AMOUNT = objListPtaPayment.Sum(m => m.HON_AMOUNT);
                    modelTotal.TOTAL_PER_TOT_VALUE = objListPtaPayment.Sum(m => m.PER_TOT_VALUE);
                    modelTotal.TOTAL_SANCTION_AMOUNT = objListPtaPayment.Sum(m => m.SANCTION_AMOUNT).HasValue ? objListPtaPayment.Sum(m => m.SANCTION_AMOUNT).Value : 0;

                    modelTotal.DIS_TOTAL_HON_AMOUNT = objCommonFunctions._IndianFormatAmount(modelTotal.TOTAL_HON_AMOUNT.ToString());
                    modelTotal.DIS_TOTAL_PER_TOT_VALUE = objCommonFunctions._IndianFormatAmount(modelTotal.TOTAL_PER_TOT_VALUE.ToString());
                    modelTotal.DIS_TOTAL_SANCTION_AMOUNT = objCommonFunctions._IndianFormatAmount(modelTotal.TOTAL_SANCTION_AMOUNT.ToString());


                }

                model = modelTotal;
                totalRecords = objListPtaPayment.Count();
                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        objListPtaPayment = objListPtaPayment.OrderBy(x => x.PTA_SANCTIONED_BY).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                    else
                    {
                        objListPtaPayment = objListPtaPayment.OrderByDescending(x => x.PTA_SANCTIONED_BY).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                }
                return objListPtaPayment.Select(ptaListItem => new
                {
                    cell = new[] {                                                                                 
                            ptaListItem.INSTITUTE_NAME,  
                            ptaListItem.PTA_SERVICE_TAX_NO,
                            objCommonFunctions._IndianFormatAmount(ptaListItem.SANCTION_AMOUNT==null?"0":ptaListItem.SANCTION_AMOUNT.ToString()),
                             objCommonFunctions._IndianFormatAmount(ptaListItem.PER_TOT_VALUE.ToString()),
                             objCommonFunctions._IndianFormatAmount(ptaListItem.HON_AMOUNT.ToString()),                            
                            "<a href='#'  class='ui-icon ui-icon-zoomin ui-align-center' onClick='ListInvoiceDetails(\"" + MAST_STATE_CODE + "\",\"" + IMS_YEAR + "\",\""  + IMS_BATCH + "\",\"" + IMS_STREAMS + "\",\"" + ProposalType + "\",\"" + ptaListItem.PTA_SANCTIONED_BY + "\",\"" + ptaListItem.INSTITUTE_NAME  + "\",\"" + ptaListItem.HON_AMOUNT+ "\",\"" + PMGSY_SCHEME + "\"); return false;'>Show Details</a>",                                    
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

        public Array GetPTAInvoiceListDAL(int page, int rows, string sidx, string sord, out int totalRecords, int MAST_STATE_CODE, Int16 IMS_YEAR, int IMS_BATCH, int IMS_STREAMS, string IMS_PROPOSAL_TYPE, string PTA_SANCTIONED_BY, string PTA_INSTITUTE_NAME, decimal HON_AMOUNT, int PMGSY_SCHEME, out PTAPaymentTotalViewModel model)
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            PTAPaymentTotalViewModel modelTotal = new PTAPaymentTotalViewModel();
            try
            {
                dbContext = new Models.PMGSYEntities();

                decimal BalanceAmount = 0;
                var ListPtaInvoiceModel = (from a in dbContext.IMS_GENERATED_INVOICE_PTA
                                           where
                                               a.MAST_STATE_CODE == MAST_STATE_CODE &&
                                               a.IMS_YEAR == IMS_YEAR &&
                                               a.IMS_BATCH == IMS_BATCH &&
                                               a.IMS_STREAM == IMS_STREAMS &&
                                               // a.IMS_PROPOSAL_TYPE == (IMS_PROPOSAL_TYPE == "A" ? "%" : IMS_PROPOSAL_TYPE) &&
                                               (IMS_PROPOSAL_TYPE == "A" ? "%" : a.IMS_PROPOSAL_TYPE) == (IMS_PROPOSAL_TYPE == "A" ? "%" : IMS_PROPOSAL_TYPE) &&
                                               a.PTA_SANCTIONED_BY == PTA_SANCTIONED_BY &&
                                               a.MAST_PMGSY_SCHEME == PMGSY_SCHEME
                                           select
                                           a).ToList();

                if (dbContext.IMS_GENERATED_INVOICE_PTA.Where(
                                                            a => a.MAST_STATE_CODE == MAST_STATE_CODE &&
                                                            a.IMS_YEAR == IMS_YEAR &&
                                                            a.IMS_BATCH == IMS_BATCH &&
                                                            a.IMS_STREAM == IMS_STREAMS &&
                                                                //a.IMS_PROPOSAL_TYPE == (IMS_PROPOSAL_TYPE == "A" ? "%" : IMS_PROPOSAL_TYPE) &&
                                                            (IMS_PROPOSAL_TYPE == "A" ? "%" : a.IMS_PROPOSAL_TYPE) == (IMS_PROPOSAL_TYPE == "A" ? "%" : IMS_PROPOSAL_TYPE) &&
                                                            a.PTA_SANCTIONED_BY == PTA_SANCTIONED_BY &&
                                                            a.MAST_PMGSY_SCHEME == PMGSY_SCHEME
                                                         ).Any())
                {
                    BalanceAmount = BalanceAmount - dbContext.IMS_GENERATED_INVOICE_PTA.Where(
                                                            a => a.MAST_STATE_CODE == MAST_STATE_CODE &&
                                                            a.IMS_YEAR == IMS_YEAR &&
                                                            a.IMS_BATCH == IMS_BATCH &&
                                                            a.IMS_STREAM == IMS_STREAMS &&
                                                            a.MAST_PMGSY_SCHEME == PMGSY_SCHEME &&
                                                                // a.IMS_PROPOSAL_TYPE == (IMS_PROPOSAL_TYPE == "A" ? "%" : IMS_PROPOSAL_TYPE) &&
                                                           (IMS_PROPOSAL_TYPE == "A" ? "%" : a.IMS_PROPOSAL_TYPE) == (IMS_PROPOSAL_TYPE == "A" ? "%" : IMS_PROPOSAL_TYPE) &&
                                                            a.PTA_SANCTIONED_BY == PTA_SANCTIONED_BY).Sum(a => a.TOTAL_AMOUNT);
                }
                else
                {
                    BalanceAmount = HON_AMOUNT;
                }
                totalRecords = ListPtaInvoiceModel.Count();
                if (ListPtaInvoiceModel != null)
                {
                    modelTotal.TOTAL_HONORARIUM_AMOUNT = ListPtaInvoiceModel.Sum(m => m.HONORARIUM_AMOUNT);
                    modelTotal.TOTAL_PENALTY_AMOUNT = ListPtaInvoiceModel.Sum(m => m.PENALTY_AMOUNT);
                    modelTotal.TOTAL_TDS_AMOUNT = ListPtaInvoiceModel.Sum(m => m.TDS_AMOUNT);
                    modelTotal.TOTAL_SC_AMOUNT = ListPtaInvoiceModel.Sum(m => m.SC_AMOUNT);
                    modelTotal.TOTAL_AMOUNT = ListPtaInvoiceModel.Sum(m => m.TOTAL_AMOUNT);
                    modelTotal.TOTAL_SERVICE_TAX_AMOUNT = ListPtaInvoiceModel.Sum(m => m.SERVICE_TAX_AMOUNT);

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
                        ListPtaInvoiceModel = ListPtaInvoiceModel.OrderBy(x => x.PTA_SANCTIONED_BY).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                    else
                    {
                        ListPtaInvoiceModel = ListPtaInvoiceModel.OrderByDescending(x => x.PTA_SANCTIONED_BY).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                }
                return ListPtaInvoiceModel.Select(ptaListItem => new
                {
                    cell = new[] {                                                                                                             
                            ptaListItem.SAS_ABBREVATION.ToString(), 
                            objCommonFunctions._IndianFormatAmount(ptaListItem.HONORARIUM_AMOUNT.ToString()),                    
                            ptaListItem.INVOICE_FILE_NO == null?"":ptaListItem.INVOICE_FILE_NO.ToString(),                           
                            objCommonFunctions._IndianFormatAmount(ptaListItem.PENALTY_AMOUNT.ToString()),                    
                            objCommonFunctions._IndianFormatAmount(ptaListItem.TDS_AMOUNT.ToString()),                       
                            objCommonFunctions._IndianFormatAmount(ptaListItem.SC_AMOUNT.ToString()),
                            objCommonFunctions._IndianFormatAmount(ptaListItem.SERVICE_TAX_AMOUNT.ToString()),
                            objCommonFunctions._IndianFormatAmount(Math.Round(ptaListItem.TOTAL_AMOUNT,MidpointRounding.AwayFromZero).ToString("0.00")),
                            ptaListItem.GENERATION_DATE.ToString("dd-MMM-yyyy"),
                            "<a href='#'  class='ui-icon ui-icon-zoomin ui-align-center' onClick='ShowDetails(\"" + ptaListItem.IMS_INVOICE_ID + "\"); return false;'>Show Details</a>",
                            "<a href='#'  class='ui-icon ui-icon-trash ui-align-center' onClick='DeleteDetails(\"" + ptaListItem.IMS_INVOICE_ID + "\"); return false;'>Delete</a>",        
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

        public string AddPtaInvoiceDetailsDAL(IMS_GENERATED_INVOICE_PTA ims_generated_invoice)
        {
            try
            {
                dbContext = new Models.PMGSYEntities();
                Int64? MaxID;
                if (!dbContext.IMS_GENERATED_INVOICE_PTA.Any())
                {
                    MaxID = 0;
                }
                else
                {
                    MaxID = (from c in dbContext.IMS_GENERATED_INVOICE_PTA select (Int32?)c.IMS_INVOICE_ID ?? 0).Max();
                }
                ++MaxID;
                ims_generated_invoice.IMS_INVOICE_ID = Convert.ToInt64(MaxID);

                //added by abhishek kamble 28-nov-2013
                ims_generated_invoice.USERID = PMGSYSession.Current.UserId;
                ims_generated_invoice.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                //ims_generated_invoice.MAST_PMGSY_SCHEME = PMGSYSession.Current.PMGSYScheme; //new change done by Deepak  on 9 June 2014
                dbContext.IMS_GENERATED_INVOICE_PTA.Add(ims_generated_invoice);
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


        public PTAInvoiceViewModel PtaPaymentReportDAL(int invoiceId)
        {
            dbContext = new Models.PMGSYEntities();
            PTAInvoiceViewModel ptaInvoiceModel = new PTAInvoiceViewModel();
            CommonFunctions objCommonFunctions = new CommonFunctions();
            try
            {
                var PtaInvoiceDetails = (from a in dbContext.IMS_GENERATED_INVOICE_PTA
                                         where a.IMS_INVOICE_ID == invoiceId
                                         select a).FirstOrDefault();

                if (PtaInvoiceDetails != null)
                {

                    ptaInvoiceModel.TOTAL_AMOUNT = Math.Round(PtaInvoiceDetails.TOTAL_AMOUNT, MidpointRounding.AwayFromZero);
                    ptaInvoiceModel.TOTAL_AMOUNT_IND_FORMAT = objCommonFunctions._IndianFormatAmount(Math.Round(PtaInvoiceDetails.TOTAL_AMOUNT, MidpointRounding.AwayFromZero).ToString());
                    ptaInvoiceModel.TOTAL_AMOUNT_WORDS = new AmountToWord().RupeesToWord(Math.Round(ptaInvoiceModel.TOTAL_AMOUNT, 2).ToString("0.00"));

                    ptaInvoiceModel.HONORARIUM_AMOUNT = PtaInvoiceDetails.HONORARIUM_AMOUNT;
                    ptaInvoiceModel.HONORARIUM_AMOUNT_IND_FORMAT = objCommonFunctions._IndianFormatAmount(ptaInvoiceModel.HONORARIUM_AMOUNT.ToString());

                    ptaInvoiceModel.PTA_SANCTIONED_BY = PtaInvoiceDetails.PTA_SANCTIONED_BY;
                    ptaInvoiceModel.SAS_ABBREVATION = PtaInvoiceDetails.SAS_ABBREVATION;

                    ptaInvoiceModel.PENALTY_AMOUNT = PtaInvoiceDetails.PENALTY_AMOUNT;
                    ptaInvoiceModel.PENALTY_AMOUNT_IND_FORMAT = objCommonFunctions._IndianFormatAmount(PtaInvoiceDetails.PENALTY_AMOUNT.ToString());

                    ptaInvoiceModel.TDS_AMOUNT = PtaInvoiceDetails.TDS_AMOUNT;
                    ptaInvoiceModel.TDS_AMOUNT_IND_FORMAT = objCommonFunctions._IndianFormatAmount(PtaInvoiceDetails.TDS_AMOUNT.ToString());

                    ptaInvoiceModel.SC_AMOUNT = PtaInvoiceDetails.SC_AMOUNT;
                    ptaInvoiceModel.SC_AMOUNT_IND_FORMAT = objCommonFunctions._IndianFormatAmount(PtaInvoiceDetails.SC_AMOUNT.ToString());

                    ptaInvoiceModel.INVOICE_GEN_DATE = PtaInvoiceDetails.GENERATION_DATE.ToString("dd-MMM-yyyy");
                    ptaInvoiceModel.Invoice_Generate_DATE = PtaInvoiceDetails.GENERATION_DATE.ToString("dd/MM/yyyy");
                    ptaInvoiceModel.INVOICE_FILE_NO = PtaInvoiceDetails.INVOICE_FILE_NO;
                    return ptaInvoiceModel;
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

        #region PTA Payment
        public Array GetPTAPaymenInoviceListDAL(int page, int rows, string sidx, string sord, out int totalRecords, int MAST_STATE_CODE, int IMS_YEAR, int IMS_BATCH, int IMS_STREAMS, string IMS_PROPOSAL_TYPE, int Scheme)
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            PTAPaymentTotalViewModel modelTotal = new PTAPaymentTotalViewModel();
            try
            {
                dbContext = new Models.PMGSYEntities();

                decimal BalanceAmount = 0;
                var ListPtaInvoiceModel = (from a in dbContext.IMS_GENERATED_INVOICE_PTA
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


                totalRecords = ListPtaInvoiceModel.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        ListPtaInvoiceModel = ListPtaInvoiceModel.OrderBy(x => x.PTA_SANCTIONED_BY).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                    else
                    {
                        ListPtaInvoiceModel = ListPtaInvoiceModel.OrderByDescending(x => x.PTA_SANCTIONED_BY).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }
                }
                return ListPtaInvoiceModel.Select(ptaListItem => new
                {
                    cell = new[] {                                                                                                             
                            ptaListItem.SAS_ABBREVATION.ToString(), 
                            objCommonFunctions._IndianFormatAmount(ptaListItem.HONORARIUM_AMOUNT.ToString()),                    
                            ptaListItem.INVOICE_FILE_NO == null?"":ptaListItem.INVOICE_FILE_NO.ToString(),                           
                            objCommonFunctions._IndianFormatAmount(ptaListItem.PENALTY_AMOUNT.ToString()),                    
                            objCommonFunctions._IndianFormatAmount(ptaListItem.TDS_AMOUNT.ToString()),                       
                            objCommonFunctions._IndianFormatAmount(ptaListItem.SC_AMOUNT.ToString()),                    
                            objCommonFunctions._IndianFormatAmount(Math.Round(ptaListItem.TOTAL_AMOUNT,MidpointRounding.AwayFromZero).ToString("0.00")),
                            ptaListItem.GENERATION_DATE.ToString("dd-MMM-yyyy"),
                            "<a href='#'  class='ui-icon ui-icon-zoomin ui-align-center' onClick='AddPtaPaymentDetail(\"" + URLEncrypt.EncryptParameters1(new string[]{"IMS_INVOICE_ID="+ptaListItem.IMS_INVOICE_ID.ToString().Trim()}) + "\"); return false;'>View Payment Details</a>",                                    
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

        public Array GetPTAPaymentListDAL(int invoiceCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                var lstPayment = (from payment in dbContext.IMS_PTA_PAYMENTS
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
                                    "<center><a href='#' class='ui-icon ui-icon-pencil' onclick='EditPTAPayment(\"" + URLEncrypt.EncryptParameters1(new string[] { "IMS_INVOICE_ID=" + m.IMS_INVOICE_ID.ToString().Trim(), "IMS_PAYMENT_ID =" + m.IMS_PAYMENT_ID.ToString().Replace("/", "") })+ "\"); return false;'>Edit</a></center>"
                                    :"<center><table><tr><td style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>",
                        m.IMS_PAYMENT_FINALIZE=="N"?
                        "<center><a href='#' class='ui-icon ui-icon-trash' onclick='DeletePTAPayment(\"" + URLEncrypt.EncryptParameters1(new string[] { "IMS_INVOICE_ID=" + m.IMS_INVOICE_ID.ToString().Trim(), "IMS_PAYMENT_ID =" + m.IMS_PAYMENT_ID.ToString().Replace("/", "") }) + "\"); return false;'>Delete</a></center>"
                        :"<center><table><tr><td style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>",
                          m.IMS_PAYMENT_FINALIZE=="N"?
                        "<center><a href='#' class='ui-icon ui-icon-unlocked' onclick='FinalizePTAPayment(\"" + URLEncrypt.EncryptParameters1(new string[] { "IMS_INVOICE_ID=" + m.IMS_INVOICE_ID.ToString().Trim(), "IMS_PAYMENT_ID =" + m.IMS_PAYMENT_ID.ToString().Replace("/", "") }) + "\"); return false;'>Finalize</a></center>"
                        :"<center><table><tr><td style='border:none'><span class='ui-icon ui-icon-locked' title='Locked' ></span></td></tr></table></center>",
                          m.IMS_PAYMENT_FINALIZE=="Y"?
                        "<center><a href='#' class='ui-icon ui-icon-unlocked' onclick='DeFinalizePTAPayment(\"" + URLEncrypt.EncryptParameters1(new string[] { "IMS_INVOICE_ID=" + m.IMS_INVOICE_ID.ToString().Trim(), "IMS_PAYMENT_ID =" + m.IMS_PAYMENT_ID.ToString().Replace("/", "") }) + "\"); return false;'>DeFinalize</a></center>"
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
        /// Add PTA Payment details
        /// </summary>
        /// <param name="ptaPaymentViewModel"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool AddPTAPaymentDetailsDAL(PTAPayemntInvoiceModel ptaPaymentViewModel, ref string message)
        {
            try
            {

                IMS_PTA_PAYMENTS imsPTAPaymentModel = new IMS_PTA_PAYMENTS();
                imsPTAPaymentModel.IMS_PAYMENT_ID = getMaxPtaPaymentCode();
                imsPTAPaymentModel.IMS_INVOICE_ID = ptaPaymentViewModel.IMS_INVOICE_CODE;
                imsPTAPaymentModel.IMS_NEFT_CHEQUE_NUMBER = ptaPaymentViewModel.IMS_NEFT_CHEQUE_NUMBER;
                imsPTAPaymentModel.IMS_NEFT_CHEQUE_PAYMENT = ptaPaymentViewModel.Payment_Type;
                imsPTAPaymentModel.IMS_PAYMENT_DATE = DateTime.ParseExact(ptaPaymentViewModel.IMS_Payment_DATE.ToString(), "dd/MM/yyyy", null);
                imsPTAPaymentModel.IMS_ENTRY_DATE = DateTime.Now;
                imsPTAPaymentModel.IMS_PAYMENT_FINALIZE = "N";
                imsPTAPaymentModel.USERID = PMGSYSession.Current.UserId;
                imsPTAPaymentModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                dbContext = new PMGSYEntities();
                dbContext.IMS_PTA_PAYMENTS.Add(imsPTAPaymentModel);
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
        /// Calculate Max PTAPayment.
        /// </summary>
        /// <returns></returns>
        public int getMaxPtaPaymentCode()
        {
            try
            {
                dbContext = new PMGSYEntities();

                int maxCode = 1;

                if (dbContext.IMS_PTA_PAYMENTS.Any())
                {
                    maxCode = dbContext.IMS_PTA_PAYMENTS.Max(s => s.IMS_PAYMENT_ID) + 1;
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
        public PTAPayemntInvoiceModel EditPTAPaymentDetailsDAL(int PayemntCode, int IMSInvoiceCode)
        {

            try
            {
                dbContext = new PMGSYEntities();
                PTAPayemntInvoiceModel ptaPaymentViewtModel = new PTAPayemntInvoiceModel();
                IMS_PTA_PAYMENTS ptaPaymentModel = dbContext.IMS_PTA_PAYMENTS.Where(m => m.IMS_PAYMENT_ID == PayemntCode && m.IMS_INVOICE_ID == IMSInvoiceCode).FirstOrDefault();
                PTAInvoiceViewModel ptaInvViewModel = PtaPaymentReportDAL(IMSInvoiceCode);

                if (ptaPaymentModel != null)
                {
                    ptaPaymentViewtModel.EncryptedIMS_Payment_CODE = URLEncrypt.EncryptParameters1(new string[] { "IMS_INVOICE_ID=" + IMSInvoiceCode.ToString().Trim(), "IMS_PAYMENT_ID =" + PayemntCode.ToString().Replace("/", "") });
                    ptaPaymentViewtModel.EncryptedIMS_Invoice_Code = URLEncrypt.EncryptParameters1(new string[] { "IMS_INVOICE_ID=" + IMSInvoiceCode.ToString().Trim() });
                    ptaPaymentViewtModel.IMS_NEFT_CHEQUE_NUMBER = ptaPaymentModel.IMS_NEFT_CHEQUE_NUMBER;
                    ptaPaymentViewtModel.IMS_Payment_DATE = ptaPaymentModel.IMS_PAYMENT_DATE.ToString("dd/MM/yyyy");
                    ptaPaymentViewtModel.Invoice_Generate_DATE = ptaInvViewModel.Invoice_Generate_DATE;
                    
                    ptaPaymentViewtModel.Payment_Type = ptaPaymentModel.IMS_NEFT_CHEQUE_PAYMENT;
                }
                return ptaPaymentViewtModel;
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
        /// Update PTA Payment details.
        /// </summary>
        /// <param name="ptaPaymentViewModel"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool UpdatePTAPaymentDetailsDAL(PTAPayemntInvoiceModel ptaPaymentViewModel, ref string message)
        {

            try
            {
                Dictionary<string, string> decryptedParameters = null;
                string[] encryptedParameter = null;
                dbContext = new PMGSYEntities();
                int IMS_Payment_Code = 0;
                int IMS_Invoice_Code = 0;

                encryptedParameter = ptaPaymentViewModel.EncryptedIMS_Payment_CODE.Split('/');

                if (!(encryptedParameter.Length == 3))
                {
                    return false;
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new string[] { encryptedParameter[0], encryptedParameter[1], encryptedParameter[2] });
                IMS_Invoice_Code = Convert.ToInt32(decryptedParameters["IMS_INVOICE_ID"].ToString());
                IMS_Payment_Code = Convert.ToInt32(decryptedParameters["IMS_PAYMENT_ID"].ToString());

                IMS_PTA_PAYMENTS imsPTAPaymentOrignalModel = dbContext.IMS_PTA_PAYMENTS.Where(m => m.IMS_PAYMENT_ID == IMS_Payment_Code && m.IMS_INVOICE_ID == IMS_Invoice_Code).FirstOrDefault();

                if (imsPTAPaymentOrignalModel == null)
                {
                    return false;
                }
                imsPTAPaymentOrignalModel.IMS_NEFT_CHEQUE_NUMBER = ptaPaymentViewModel.IMS_NEFT_CHEQUE_NUMBER;
                imsPTAPaymentOrignalModel.IMS_NEFT_CHEQUE_PAYMENT = ptaPaymentViewModel.Payment_Type;
                imsPTAPaymentOrignalModel.IMS_PAYMENT_DATE = DateTime.ParseExact(ptaPaymentViewModel.IMS_Payment_DATE, "dd/MM/yyyy", null);
                imsPTAPaymentOrignalModel.IMS_ENTRY_DATE = DateTime.Now;
                imsPTAPaymentOrignalModel.USERID = PMGSYSession.Current.UserId;
                imsPTAPaymentOrignalModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                dbContext.Entry(imsPTAPaymentOrignalModel).State = System.Data.Entity.EntityState.Modified;
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
        /// Delete PTAPayment details.
        /// </summary>
        /// <param name="paymentCode"></param>
        /// <param name="imsInvoiceCode"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool DeletePTAPaymentDetailsDAL(int paymentCode, int imsInvoiceCode, ref string message)
        {

            try
            {
                dbContext = new PMGSYEntities();

                IMS_PTA_PAYMENTS ptaPaymentModel = dbContext.IMS_PTA_PAYMENTS.Where(m => m.IMS_PAYMENT_ID == paymentCode && m.IMS_INVOICE_ID == imsInvoiceCode).FirstOrDefault();
                if (ptaPaymentModel == null)
                {
                    message = "Record not exist for delete.";
                    return false;
                }

                ptaPaymentModel.USERID = PMGSYSession.Current.UserId;
                ptaPaymentModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                dbContext.Entry(ptaPaymentModel).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                dbContext.IMS_PTA_PAYMENTS.Remove(ptaPaymentModel);
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
        /// Update PTA Payment details.
        /// </summary>
        /// <param name="ptaPaymentViewModel"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool FinalizePTAPaymentDetailsDAL(int paymentCode, int imsInvoiceCode, ref string message)
        {

            try
            {             
               
                dbContext = new PMGSYEntities();
                IMS_PTA_PAYMENTS ptaPaymentModel = dbContext.IMS_PTA_PAYMENTS.Where(m => m.IMS_PAYMENT_ID == paymentCode && m.IMS_INVOICE_ID == imsInvoiceCode).FirstOrDefault();

                if (ptaPaymentModel == null)
                {
                    return false;
                }
                ptaPaymentModel.IMS_PAYMENT_FINALIZE = "Y";
                ptaPaymentModel.USERID = PMGSYSession.Current.UserId;
                ptaPaymentModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                dbContext.Entry(ptaPaymentModel).State = System.Data.Entity.EntityState.Modified;
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
    public interface IPTAPaymentDAL
    {
        Array GetPTAPaymentListDAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int MAST_STATE_CODE, int IMS_YEAR, int IMS_BATCH, int IMS_STREAMS, string ProposalType, decimal TOT_HON_MIN, int PMGSY_SCHEME, out PTAPaymentTotalModel model);
        Array GetPTAInvoiceListDAL(int page, int rows, string sidx, string sord, out int totalRecords, int MAST_STATE_CODE, Int16 IMS_YEAR, int IMS_BATCH, int IMS_STREAMS, string IMS_PROPOSAL_TYPE, string PTA_SANCTIONED_BY, string PTA_INSTITUTE_NAME, decimal HON_AMOUNT, int PMGSY_SCHEME, out PTAPaymentTotalViewModel model);
        string AddPtaInvoiceDetailsDAL(IMS_GENERATED_INVOICE_PTA ims_generated_invoice);
        PTAInvoiceViewModel PtaPaymentReportDAL(int invoiceId);
        #region PTA Payment
        Array GetPTAPaymenInoviceListDAL(int page, int rows, string sidx, string sord, out int totalRecords, int MAST_STATE_CODE, int IMS_YEAR, int IMS_BATCH, int IMS_STREAMS, string IMS_PROPOSAL_TYPE, int Scheme);
        Array GetPTAPaymentListDAL(int invoiceCode, int page, int rows, string sidx, string sord, out long totalRecords);
        bool AddPTAPaymentDetailsDAL(PTAPayemntInvoiceModel ptaPaymentViewModel, ref string message);
        PTAPayemntInvoiceModel EditPTAPaymentDetailsDAL(int PayemntCode, int IMSInvoiceCode);
        bool UpdatePTAPaymentDetailsDAL(PTAPayemntInvoiceModel ptaPaymentViewModel, ref string message);
        bool DeletePTAPaymentDetailsDAL(int paymentCode, int imsInvoiceCode, ref string message);
        bool FinalizePTAPaymentDetailsDAL(int paymentCode, int imsInvoiceCode, ref string message);
        #endregion
    }
}