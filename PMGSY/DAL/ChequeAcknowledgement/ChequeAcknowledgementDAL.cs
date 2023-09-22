/* 
     *  Name : ChequeAcknowledgementDAL.cs
     *  Path : ~DAL/ChequeAcknowledgement\ChequeAcknowledgementDAL.cs
     *  Description : ChequeAcknowledgementDAL.cs is Data access layer file for Cheuque Acknowledgment module.

                      
     *  Functions / Procedures Called : 
            
               
                                               

     * classes used 
        
        * ChequeAcknowledgementDAL
                                                   
 
     *  Author : Amol Jadhav (PE, e-gov)
     *  Company : C-DAC,E-GOV
     *  Dates of creation : 16/06/2013  
    
*/


using PMGSY.Common;
using PMGSY.Models;
using PMGSY.Models.ChequeAcknowledgement;
using System;
using System.Collections.Generic;
using System.Linq;
using PMGSY.Extensions;
using System.Web;
using System.Transactions;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data;

namespace PMGSY.DAL.ChequeAcknowledgement
{
    public class ChequeAcknowledgementDAL : IChequeAcknowledgementDAL
    {
        

        PMGSYEntities dbContext = null;

        public ChequeAcknowledgementDAL()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objFilter"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array ListChequeForAcknowledgment(PaymentFilterModel objFilter, out long totalRecords, out List<String> SelectedIdList, ref string SrrdaBillId)
        {
            CommonFunctions commomFuncObj = null;
            try
            {
                dbContext = new PMGSYEntities();

                commomFuncObj = new CommonFunctions();

                //Get SRRDA_BILL_ID 16-july-2014 start
                long na_bill_id = 0;

                string ndCode = objFilter.AdminNdCode.ToString();

                string bID = (from bm in dbContext.ACC_BILL_MASTER
                              where bm.BILL_MONTH == objFilter.Month
                                  && bm.BILL_YEAR == objFilter.Year
                                  && bm.BILL_TYPE == "P"
                                  && bm.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode
                                  // Commented and added by Srishti on 09-03-2023
                                  //&& (bm.TXN_ID == 212 || bm.TXN_ID == 628 || bm.TXN_ID == 704)
                                  && ((PMGSYSession.Current.FundType == "P" &&

                                        (objFilter.Account_Type == "1" && bm.TXN_ID == 212) ||
                                        (objFilter.Account_Type == "2" && bm.TXN_ID == 3193) ||
                                        (objFilter.Account_Type == "3" && bm.TXN_ID == 3194)) ||
                                      (PMGSYSession.Current.FundType == "A" && bm.TXN_ID == 628) ||
                                      (PMGSYSession.Current.FundType == "M" && bm.TXN_ID == 704))
                                  && bm.CHALAN_NO == ndCode
                                  && bm.FUND_TYPE == PMGSYSession.Current.FundType
                              select bm.BILL_ID).Distinct().FirstOrDefault().ToString();



                if (bID != null && bID != "0" && bID != String.Empty)
                {
                    SrrdaBillId = URLEncrypt.EncryptParameters(new string[] { bID });
                    //na_bill_id used in get only acknowledged chq records.
                    na_bill_id = Convert.ToInt64(bID);
                }


                //Get SRRDA_BILL_ID 16-july-2014 start end



                int TxnIdCancel = 0;
                int TxnIdEmargCancel = 0;//Added on 14-07-2023 for Emarg Cheque Cancellation
                int TxnIdRenew = 0;
                switch (PMGSYSession.Current.FundType)
                {
                    case "A":
                        TxnIdCancel = 229;
                        TxnIdRenew = 624;
                        break;
                    case "P":
                        TxnIdCancel = 625;
                        TxnIdRenew = 228;
                        break;
                    case "M":
                        TxnIdCancel = 824;
                        TxnIdEmargCancel = 3116;//Added on 14-07-2023 for Emarg Cheque Cancellation
                        TxnIdRenew = 825;
                        break;
                    default:
                        break;
                }



                var query = (from master in dbContext.ACC_BILL_MASTER
                             join details in dbContext.ACC_CHEQUES_ISSUED
                             on master.BILL_ID equals details.BILL_ID into c
                             from chq in c.DefaultIfEmpty()
                             where
                                    master.ADMIN_ND_CODE == objFilter.AdminNdCode
                                    && master.BILL_TYPE == objFilter.Bill_type
                                    //&& (master.CHQ_EPAY == "Q" || master.CHQ_EPAY == "E")
                                    && (master.CHQ_EPAY == "Q" || master.CHQ_EPAY == "E" || master.CHQ_EPAY == "B" || master.CHQ_EPAY == "A") // new change done by Vikram on 28 Jan 2014 as old data bank authorization payment should be ackowledged.         //Change by Abhishek for Advice no 7Apr2015
                                    && master.FUND_TYPE == objFilter.FundType
                                    //&& details.CHEQUE_STATUS == "N"       Commented By Abhishek kamble 9-jan-2014 for Chq Ack testing
                                    && master.BILL_MONTH == objFilter.Month
                                    && master.BILL_YEAR == objFilter.Year
                                    //&& master.GROSS_AMOUNT > 0
                                    //      && master.TXN_ID != 229 && //not cancelled            Commented By Abhishek kamble 9-jan-2014 for Chq Ack testing
                                    //    !dbContext.ACC_CANCELLED_CHEQUES.Where(g => g.OLD_BILL_ID.Value == master.BILL_ID).Any()      Commented By Abhishek kamble 9-jan-2014 for Chq Ack testing
                                    //&& !dbContext.ACC_CANCELLED_CHEQUES.Where(g => g.OLD_BILL_ID.Value == master.BILL_ID).Any()      //Uncommented By Abhishek kamble 23-Feb-2015 Canceled chq/Epay not Availabled for Ack
                                    //   && ((master.TXN_ID == 229 || master.TXN_ID == 625 || master.TXN_ID == 824) ? (dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == dbContext.ACC_CANCELLED_CHEQUES.Where(x => x.BILL_ID == master.BILL_ID).Select(s => s.OLD_BILL_ID).FirstOrDefault()).Select(s => s.CHQ_EPAY).FirstOrDefault() == "E" ? false : true) : true)////Abhishek kamble 20-Mar-2015 Canceled Epay not Availabled for Ack
                                    && master.BILL_FINALIZED == "Y"
                                    && master.CHQ_NO != null //Modified By Abhishek kamble to Skip Authorization Surrender Transaction for Ack 21-July-2014
                                                             //Added by Srishti on 06 - 03 - 2023
                                    && ((objFilter.Account_Type == "1" && master.TXN_ID != 3173 && master.TXN_ID != 3185 && master.TXN_ID != 3187) ||
                                        (objFilter.Account_Type == "2" && (master.TXN_ID == 3173 || master.TXN_ID == 3185)) ||
                                        (objFilter.Account_Type == "3" && master.TXN_ID == 3187))
                             select new
                             {
                                 master.BILL_DATE,
                                 master.CHQ_DATE,
                                 master.CHQ_NO,
                                 master.PAYEE_NAME,
                                 master.CHQ_AMOUNT,
                                 master.BILL_ID,
                                 master.ACC_MASTER_TXN.TXN_DESC,
                                 master.ACC_CHEQUES_ISSUED.NA_BILL_ID,
                                 IS_CHQ_ENCASHED_NA = master.ACC_CHEQUES_ISSUED.IS_CHQ_ENCASHED_NA == null ? false : (master.ACC_CHEQUES_ISSUED.IS_CHQ_ENCASHED_NA == true ? true : false),
                                 CHEQUE_STATUS = master.ACC_CHEQUES_ISSUED.CHEQUE_STATUS == null ? "N" : master.ACC_CHEQUES_ISSUED.CHEQUE_STATUS,
                                 //new change done by Vikram on 13 Jan 2014
                                 master.TXN_ID
                                 //end of change
                                 //dbContext.ACC_CHEQUES_ISSUED.Where(m=>m.BILL_ID == master.BILL_ID).Select(m=>m.NA_BILL_ID).FirstOrDefault(),//  details.NA_BILL_ID,
                                 //dbContext.ACC_CHEQUES_ISSUED.Where(m=>m.BILL_ID == master.BILL_ID).Select(m=>m.IS_CHQ_ENCASHED_NA).FirstOrDefault(),//details.IS_CHQ_ENCASHED_NA
                                 //chq.NA_BILL_ID,
                                 //chq.IS_CHQ_ENCASHED_NA,
                                 //chq.CHEQUE_STATUS                                 
                             });


                //Added By Abhishek kamble to get only Acknowledged cheques records for Unacknowledgement. 26-Aug-2014  start
                if (objFilter.AckUnackFlag == "U")
                {
                    query = query.Where(m => m.NA_BILL_ID == na_bill_id);
                }
                //Added By Abhishek kamble to get only Acknowledged cheques records for Unacknowledgement. 26-Aug-2014  end


                totalRecords = query.Count();

                List<long> SelectedIdListLong = new List<long>();
                SelectedIdList = new List<string>();
                //get previously acknowledged cheques
                SelectedIdListLong = (query.Where(c => c.IS_CHQ_ENCASHED_NA == true).Select(y => y.BILL_ID)).ToList<long>();

                foreach (long item in SelectedIdListLong)
                {
                    // SelectedIdList.Add(URLEncrypt.EncryptParameters(new string[] { item.ToString().Trim() }));
                    SelectedIdList.Add(item.ToString());
                }

                query = query.OrderBy(x => x.BILL_DATE);

                var result = query.Select(master => new
                {
                    master.BILL_DATE,
                    master.CHQ_DATE,
                    master.CHQ_NO,
                    master.PAYEE_NAME,
                    master.CHQ_AMOUNT,
                    master.BILL_ID,
                    master.TXN_DESC,
                    master.NA_BILL_ID,
                    master.IS_CHQ_ENCASHED_NA,
                    master.CHEQUE_STATUS,
                    master.TXN_ID


                }).ToArray();

                return result.Select(item => new
                {
                    // id = URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() }),
                    id = item.BILL_ID,
                    cell = new[] {


                                        commomFuncObj.GetDateTimeToString(item.BILL_DATE),
                                        item.CHQ_DATE.HasValue ?commomFuncObj.GetDateTimeToString(item.CHQ_DATE.Value):"",
                                        item.TXN_DESC.ToString(),
                                        //item.CHQ_NO.ToString(), // new change done by Vikram on 28 Jan 2014
                                        // Added by Srishti on 08-03-2023
                                        (item.TXN_ID == 3173 || item.TXN_ID == 3185) ? "Holding" : (item.TXN_ID == 3187) ? "Security Deposit Account" : "SNA",
                                        item.CHQ_NO == null?"-":item.CHQ_NO.ToString(),
                                        item.PAYEE_NAME==null?"-":item.PAYEE_NAME.ToString(),
                                        //item.CHEQUE_STATUS == "X" && item.TXN_ID == TxnIdCancel?"-"+item.CHQ_AMOUNT.ToString():(item.CHEQUE_STATUS == "R" && item.TXN_ID == TxnIdRenew?"-"+item.CHQ_AMOUNT.ToString():item.CHQ_AMOUNT.ToString()),
                                       // item.CHEQUE_STATUS == "N" || item.CHEQUE_STATUS == "R"? item.CHQ_AMOUNT.ToString():"-"+item.CHQ_AMOUNT.ToString(),
                                        
                                        //Commented on 14-07-2023 for Emarg Cheque Cancellation
                                        //item.TXN_ID == TxnIdCancel ? "-"+ item.CHQ_AMOUNT.ToString():item.CHQ_AMOUNT.ToString(),
                                        
                                        //Added on 14-07-2023 for Emarg Cheque Cancellation (negative amount Entry)
                                        (item.TXN_ID == TxnIdCancel) || (item.TXN_ID == TxnIdEmargCancel) ? "-"+ item.CHQ_AMOUNT.ToString():item.CHQ_AMOUNT.ToString(),

                                        item.IS_CHQ_ENCASHED_NA.ToString(),
                                        item.NA_BILL_ID.HasValue ?URLEncrypt.EncryptParameters(new string[] { item.NA_BILL_ID.ToString() }):"",
                                        URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString() })
                        }
                }).ToArray();

            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                totalRecords = 0;
                SelectedIdList = new List<string>();
                //SrrdaBillId = "";
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// DAL function to acknowledgemtn of cheques
        /// </summary>
        /// <param name="model">Model to save </param>
        /// <param name="array_bill_id"> array of bill ids of the cheques which are selected for acknowledgement</param>
        /// <param name="finalize">To finalize or not </param>
        /// <returns> Acknowledgment status: 1 for success -111:for already finalized ack voucher</returns>
        public String AcknowledgeCheues(CheckAckModel model, long[] array_bill_id, bool finalize)
        {
            CommonFunctions objCommon = new CommonFunctions();
            dbContext = new PMGSYEntities();
            short month = 0;
            short year = 0;
            short day = 0;
            try
            {

                //Added By Abhishek kamble 17June2015
                if (dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Select(s => s.MAST_ND_TYPE).FirstOrDefault() == "D")
                {
                    return "-777";
                    //Response.Redirect("/Login/Login/");
                }


                //using (var scope = new TransactionScope())
                //{

                //Old code Before SP commented by Abhishek kamble 22-Aug-2014 start
                // decimal amountToAdd =0;  //sum of cheque amount newly added 
                // decimal amountToRemove =0; //sum of cheque amount which are removed
                ACC_BILL_MASTER ModelToAdd = null;
                //Old code Before SP commented by Abhishek kamble 22-Aug-2014 end


                //string VoucherNo = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == model.NA_BILL_ID && m.BILL_YEAR == model.BILL_YEAR_VOUCHER && m.BILL_MONTH == model.BILL_MONTH_VOUCHER && m.FUND_TYPE == PMGSYSession.Current.FundType && m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && m.BILL_FINALIZED != "Y").Select(m => m.BILL_NO).FirstOrDefault();
                //if (VoucherNo != model.BILL_NO && VoucherNo != null)
                //{
                //    return VoucherNo;
                //}

                // //check previous month cheque is ack or not?
                // //Change by ashish markande
                //short billMonth = 0;
                //short billYear = 0000;
                //if (model.BILL_MONTH_VOUCHER == 1)
                //{
                //    billMonth = 12;
                //    billYear = Convert.ToInt16(model.BILL_YEAR_VOUCHER - 1);
                //}
                //else
                //{
                //    billMonth = Convert.ToInt16(model.BILL_MONTH_VOUCHER - 1);
                //    billYear = model.BILL_YEAR_VOUCHER;
                //}

                //List<long> previousBillId = dbContext.ACC_BILL_MASTER.Where(m => m.ADMIN_ND_CODE == model.DPIU_CODE && m.BILL_MONTH == billMonth && m.BILL_YEAR == billYear && m.FUND_TYPE == PMGSYSession.Current.FundType && m.BILL_TYPE == "P" && m.CHQ_EPAY == "Q").Select(m=>m.BILL_ID).ToList();
                //ModelToAdd = new ACC_BILL_MASTER();
                //ACC_CHEQUES_ISSUED objChequeIssued = new ACC_CHEQUES_ISSUED();
                //foreach (long item in previousBillId)
                //{

                //   objChequeIssued = dbContext.ACC_CHEQUES_ISSUED.Where(m => m.BILL_ID == item).FirstOrDefault();

                //   ModelToAdd = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == objChequeIssued.NA_BILL_ID).FirstOrDefault();
                //  if (ModelToAdd == null)
                //  {
                //       return "-555"; //last month cheque is not acknowledge
                //   }
                //  else if (objChequeIssued.IS_CHQ_ENCASHED_NA == false && objChequeIssued.NA_BILL_ID == null && ModelToAdd.BILL_FINALIZED == "N")
                //   {
                //       return "-555"; //last month cheque is not acknowledge
                //   }
                //string progFund=string.Empty;
                //string adminFund=string.Empty;
                //string mainFund=string.Empty;

                //switch(PMGSYSession.Current.FundType)
                //{
                //    case "P":
                //        progFund="P";
                //        break;
                //    case "A":
                //        adminFund="A";
                //            break;
                //    case "M":
                //        mainFund="M";
                //        break;
                //}
                //var billIdCount=(from bm in dbContext.ACC_BILL_MASTER
                //                 join bd in dbContext.ACC_BILL_DETAILS 
                //                 on bm.BILL_ID equals bd.BILL_ID
                //                 where bm.ADMIN_ND_CODE==PMGSYSession.Current.AdminNdCode
                //                 && bd.ADMIN_ND_CODE==model.DPIU_CODE
                //                 && bm.BILL_MONTH==billMonth && bm.BILL_YEAR==billYear
                //                 && bm.FUND_TYPE==progFund?bm.TXN_ID==212:(bm.FUND_TYPE==adminFund?bm.TXN_ID==628:bm.TXN_ID==704)
                //                 && bm.FUND_TYPE==PMGSYSession.Current.FundType && bm.BILL_FINALIZED=="Y"
                //                 select new 
                //                 {
                //                     bm.BILL_ID
                //                 }).Count();

                //if(billIdCount==0)
                //{
                //    return "-555";
                //}                   


                //check if allready finalized
                if (model.NA_BILL_ID != 0)
                {
                    ModelToAdd = new ACC_BILL_MASTER();

                    ModelToAdd = dbContext.ACC_BILL_MASTER.Where(c => c.BILL_ID == model.NA_BILL_ID).First();

                    if (ModelToAdd.BILL_FINALIZED == "Y")
                    {

                        //already finalized unfinalize the voucher first !!!
                        return "-111";

                    }

                }

                day = Convert.ToInt16(model.BILL_DATE.Split('/')[0]);
                month = Convert.ToInt16(model.BILL_DATE.Split('/')[1]);
                year = Convert.ToInt16(model.BILL_DATE.Split('/')[2]);

                if (!dbContext.ACC_RPT_MONTHWISE_SUMMARY.Where(x => x.ADMIN_ND_CODE == model.DPIU_CODE &&
                   x.ACC_MONTH == month && x.ACC_YEAR == year && x.FUND_TYPE == PMGSYSession.Current.FundType).Any())
                {
                    return "-222";  //month is not closed
                }

                DateTime endOfMonth = new DateTime(year, month, DateTime.DaysInMonth(year, month));

                if (day != endOfMonth.Day)
                {
                    return "-333";  //not last day of the month
                }


                //Old code Before SP commented by Abhishek kamble 22-Aug-2014 start
                //if cheques allready acknowledged but not finalized
                //if (model.NA_BILL_ID != 0)
                //{

                //    //check if previosly acknowledged cheques
                //    if (dbContext.ACC_CHEQUES_ISSUED.Where(x => array_bill_id.Contains(x.BILL_ID)).Any())
                //    {


                //        //Step 2) get the bill details records which are deleted from previous batch

                //        List<long> AckchequesBillId = new List<long>() ;

                //        //already acknowledged bill id
                //        AckchequesBillId = dbContext.ACC_CHEQUES_ISSUED.Where(c => c.NA_BILL_ID == model.NA_BILL_ID && c.IS_CHQ_ENCASHED_NA == true).Select(t => t.BILL_ID).ToList<long>();

                //        //keep copy 
                //        List<long> dupAckchequesBillId = new List<long>(AckchequesBillId);

                //        //remove the common cheques they are allready acknowledged
                //        AckchequesBillId.RemoveAll(c => array_bill_id.Contains(c));


                //        //remove the common from new cheques also
                //        array_bill_id = array_bill_id.Except(dupAckchequesBillId).ToArray<long>();

                //        //find out deleted ack cheques of previous batch in new batch 
                //        List<long> billIdToRemove = AckchequesBillId.Where(p => !array_bill_id.Contains(p)).ToList<long>();

                //        /// get the bill details records which are added in new batch

                //        List<long> billIdToAdd = array_bill_id.Where(p => !dupAckchequesBillId.Contains(p)).ToList<long>();

                //        // array of elements to add
                //        array_bill_id = billIdToAdd.ToArray<long>();


                //        //remove the deselected cheques which are already acknowledged
                //        if (billIdToRemove.Count != 0) 
                //        { 

                //            ACC_BILL_DETAILS[] ackBilldetailsCredit = new ACC_BILL_DETAILS[billIdToRemove.Count];
                //            ACC_BILL_DETAILS[] ackBilldetailsDebit = new ACC_BILL_DETAILS[billIdToRemove.Count];
                //            ACC_CHEQUES_ISSUED[] old_model = new ACC_CHEQUES_ISSUED[billIdToRemove.Count];
                //            int k = 0;

                //            List<short> DeletedBillTransID = new List<short>();

                //            foreach(long bill in billIdToRemove )
                //            {


                //                //get the details of the cheque to be deleted form acknowledged bill entry
                //                ACC_BILL_MASTER billDetails = new ACC_BILL_MASTER();
                //                billDetails = dbContext.ACC_BILL_MASTER.SingleOrDefault(p => p.BILL_ID == bill);

                //                amountToRemove +=billDetails.CHQ_AMOUNT ;

                //               //get the possible occurence of it in bill_details

                //                //find trns Id of debit of max trns id of the record with same amount and not allready seleced for deletion
                //                short maxTrnsIdDebit =0;

                //                maxTrnsIdDebit = dbContext.ACC_BILL_DETAILS.Where(p => p.BILL_ID == model.NA_BILL_ID && p.AMOUNT == billDetails.CHQ_AMOUNT && p.CREDIT_DEBIT == "D" && !DeletedBillTransID.Contains(p.TXN_NO))
                //                    .Select(x=>x.TXN_NO ).Max();

                //                //get debit and credit rows
                //                ackBilldetailsCredit[k] = new ACC_BILL_DETAILS();
                //                ackBilldetailsDebit[k] = new ACC_BILL_DETAILS();

                //                ackBilldetailsCredit[k] = dbContext.ACC_BILL_DETAILS.SingleOrDefault(p => p.BILL_ID == model.NA_BILL_ID && p.TXN_NO == maxTrnsIdDebit - 1 && p.CREDIT_DEBIT == "C");
                //                ackBilldetailsDebit[k] = dbContext.ACC_BILL_DETAILS.SingleOrDefault(p => p.BILL_ID == model.NA_BILL_ID && p.TXN_NO == maxTrnsIdDebit && p.CREDIT_DEBIT == "D");

                //                //delete it 
                //                dbContext.ACC_BILL_DETAILS.Remove(ackBilldetailsCredit[k]);
                //                dbContext.ACC_BILL_DETAILS.Remove(ackBilldetailsDebit[k]);

                //                // update their status as not acknowleged

                //                #region update the cheque IS_CHQ_ENCASHED_NA status to "false"

                //                 old_model[k] = new ACC_CHEQUES_ISSUED();
                //                 old_model[k] = dbContext.ACC_CHEQUES_ISSUED.Where(x => x.BILL_ID == bill).FirstOrDefault();
                //                 if (old_model[k] != null)
                //                {
                //                    old_model[k].IS_CHQ_ENCASHED_NA = false;
                //                    old_model[k].NA_BILL_ID = null;
                //                    dbContext.Entry(old_model[k]).State = System.Data.Entity.EntityState.Modified;
                //                }

                //                #endregion

                //                k++;

                //                DeletedBillTransID.Add(maxTrnsIdDebit);

                //              }

                //       }

                //    }

                //}
                //Old code Before SP commented by Abhishek kamble 22-Aug-2014 end

                //Added By Ashish Markande
                //long SealectedBillId = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_NO == model.BILL_NO && m.FUND_TYPE == PMGSYSession.Current.FundType && m.LVL_ID == PMGSYSession.Current.LevelId && m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && m.BILL_TYPE == "P").Select(m => m.BILL_ID).FirstOrDefault();
                //if (SealectedBillId != 0 && model.NA_BILL_ID==0)
                //{
                //    ModelToAdd = new ACC_BILL_MASTER();
                //    ModelToAdd = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == SealectedBillId).FirstOrDefault();
                //    ModelToAdd.CHQ_AMOUNT = dbContext.ACC_BILL_MASTER.Where(m => array_bill_id.Contains(m.BILL_ID)).Select(m => m.CHQ_AMOUNT).Sum();
                //    ModelToAdd.GROSS_AMOUNT = ModelToAdd.CHQ_AMOUNT;
                //    dbContext.Entry(ModelToAdd).State = System.Data.Entity.EntityState.Modified;

                //}                   


                //Old code Before SP commented by Abhishek kamble 22-Aug-2014 start
                //      Int64 maxBillId = 0;

                //     //if not already added in master table add it

                //     if (model.NA_BILL_ID == 0)
                //     {

                //         # region bill master entry
                //         //new entry     
                //         ModelToAdd = new ACC_BILL_MASTER();


                //         if (dbContext.ACC_BILL_MASTER.Any())
                //         {
                //             maxBillId = dbContext.ACC_BILL_MASTER.Max(c => c.BILL_ID);
                //         }

                //         maxBillId = maxBillId + 1;

                //         ModelToAdd.BILL_ID = maxBillId;
                //         ModelToAdd.BILL_NO = model.BILL_NO;
                //         ModelToAdd.BILL_MONTH = month;
                //         ModelToAdd.BILL_YEAR = year;
                //         ModelToAdd.BILL_DATE = objCommon.GetStringToDateTime(model.BILL_DATE);
                //         ModelToAdd.TXN_ID = dbContext.ACC_MASTER_TXN.Where(x => x.BILL_TYPE == "A" && x.FUND_TYPE == PMGSYSession.Current.FundType && x.OP_LVL_ID == PMGSYSession.Current.LevelId && x.IS_OPERATIONAL == true).Select(d => d.TXN_ID).First();
                //         ModelToAdd.CHQ_Book_ID = null;
                //         ModelToAdd.CHQ_NO = String.Empty;
                //         ModelToAdd.CHQ_DATE = null;
                //         //if (billMaster.TXN_ID == 229 || billMaster.TXN_ID == 625 || billMaster.TXN_ID == 824)
                //         //{
                //         ModelToAdd.CHQ_AMOUNT = (from o in dbContext.ACC_BILL_MASTER 
                //                                      where array_bill_id.Contains(o.BILL_ID) 
                //                                      select 
                //                                      (
                //                                          (o.TXN_ID == 229 ||o.TXN_ID == 625||o.TXN_ID == 824)? 0 - o.CHQ_AMOUNT:o.CHQ_AMOUNT
                //                                      )).Sum();


                //         ModelToAdd.CASH_AMOUNT = 0;
                //         ModelToAdd.GROSS_AMOUNT = ModelToAdd.CHQ_AMOUNT; //calculated in controller
                //         //old Code 
                //         //ModelToAdd.CHALAN_NO = null;

                //         //New Code Added By Abhishek kamble to Add DPIU Code in BILL_MASTER table to get details from DPIU code 16-July-2014
                //         ModelToAdd.CHALAN_NO = model.DPIU_CODE.ToString();

                //         ModelToAdd.CHALAN_DATE = null;
                //         ModelToAdd.PAYEE_NAME = null; ;
                //         ModelToAdd.CHQ_EPAY = "Q";
                //         ModelToAdd.TEO_TRANSFER_TYPE = null;
                //         ModelToAdd.REMIT_TYPE = null;
                //         ModelToAdd.BILL_FINALIZED = finalize ? "Y" : "N";
                //         ModelToAdd.FUND_TYPE = PMGSYSession.Current.FundType;
                //         ModelToAdd.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                //         ModelToAdd.LVL_ID = (byte)PMGSYSession.Current.LevelId;
                //         ModelToAdd.MAST_CON_ID = null;
                //         ModelToAdd.BILL_TYPE = "P";

                //         dbContext.ACC_BILL_MASTER.Add(ModelToAdd);
                //         #endregion
                //     }

                //     else
                //     {
                //         //get the details od the master details allready  entered
                //         ModelToAdd = dbContext.ACC_BILL_MASTER.Where(c => c.BILL_ID == model.NA_BILL_ID).First();
                //     }

                // ACC_BILL_MASTER billMaster = null;
                // //if their are elements to add
                // if (array_bill_id.Count() > 0)
                // {
                //     //enter the bill details 
                //     # region bill details entry

                //     ACC_BILL_DETAILS[] ModelToAddCredit = new ACC_BILL_DETAILS[array_bill_id.Length];
                //     ACC_BILL_DETAILS[] ModelToAddDebit = new ACC_BILL_DETAILS[array_bill_id.Length];

                //     short maxTxnNo = 0;

                //     if (dbContext.ACC_BILL_DETAILS.Where(t => t.BILL_ID == (ModelToAdd.BILL_ID)).Any())
                //     {
                //         maxTxnNo = dbContext.ACC_BILL_DETAILS.Where(t => t.BILL_ID == (ModelToAdd.BILL_ID)).Max(c => c.TXN_NO);
                //     }

                //     int i = 0, j = 0;



                //     foreach (long item in array_bill_id)
                //     {
                //         billMaster = new ACC_BILL_MASTER();

                //         billMaster = dbContext.ACC_BILL_MASTER.Where(x => x.BILL_ID == item).First();

                //         if (billMaster.TXN_ID == 229 || billMaster.TXN_ID == 625 || billMaster.TXN_ID == 824)
                //         {



                //             amountToAdd = amountToAdd - billMaster.CHQ_AMOUNT;
                //         }
                //         else
                //         {
                //             amountToAdd = amountToAdd + billMaster.CHQ_AMOUNT;
                //         }
                //         ModelToAddCredit[i] = new ACC_BILL_DETAILS();

                //         //creating credit entry Cheque payment
                //         maxTxnNo = Convert.ToInt16(maxTxnNo + 1);
                //         ModelToAddCredit[i].BILL_ID = ModelToAdd.BILL_ID;
                //         ModelToAddCredit[i].TXN_NO = maxTxnNo;
                //         ModelToAddCredit[i].TXN_ID = null;
                //         ModelToAddCredit[i].HEAD_ID = (from item1 in dbContext.ACC_TXN_HEAD_MAPPING
                //                                        where item1.TXN_ID == ModelToAdd.TXN_ID && item1.CASH_CHQ.Contains("Q") && item1.CREDIT_DEBIT == "C"
                //                                        select item1.HEAD_ID).FirstOrDefault();
                //         if (billMaster.TXN_ID == 229 || billMaster.TXN_ID == 625 || billMaster.TXN_ID == 824)
                //         {

                //             ModelToAddCredit[i].AMOUNT = dbContext.ACC_BILL_DETAILS.Where(x => x.BILL_ID == item && x.CREDIT_DEBIT == "D").Sum(x => x.AMOUNT);
                //             ModelToAddCredit[i].AMOUNT = 0 - ModelToAddCredit[i].AMOUNT;
                //         }
                //         else
                //         {
                //             ModelToAddCredit[i].AMOUNT = dbContext.ACC_BILL_DETAILS.Where(x => x.BILL_ID == item && x.CREDIT_DEBIT == "D" && x.CASH_CHQ == "Q").Sum(x => x.AMOUNT);
                //         }
                //         ModelToAddCredit[i].CREDIT_DEBIT = "C";
                //         ModelToAddCredit[i].CASH_CHQ = "Q";
                //         ModelToAddCredit[i].NARRATION = "Cheque No:" + billMaster.CHQ_NO + " encashed";
                //         ModelToAddCredit[i].ADMIN_ND_CODE = billMaster.ADMIN_ND_CODE;
                //         ModelToAddCredit[i].MAST_CON_ID = null;
                //         ModelToAddCredit[i].IMS_PR_ROAD_CODE = null;
                //         ModelToAddCredit[i].IMS_AGREEMENT_CODE = null;
                //         ModelToAddCredit[i].MAS_FA_CODE = null;
                //         ModelToAddCredit[i].FINAL_PAYMENT = null;
                //         ModelToAddCredit[i].MAST_DISTRICT_CODE = null;

                //         dbContext.ACC_BILL_DETAILS.Add(ModelToAddCredit[i]);

                //         i = i++;

                //         //creating debit entry Cheque payment

                //         ModelToAddDebit[j] = new ACC_BILL_DETAILS();

                //         maxTxnNo = Convert.ToInt16(maxTxnNo + 1);
                //         ModelToAddDebit[j].BILL_ID = ModelToAdd.BILL_ID;
                //         ModelToAddDebit[j].TXN_NO = maxTxnNo;
                //         ModelToAddDebit[j].TXN_ID = null;
                //         ModelToAddDebit[j].HEAD_ID = (from item1 in dbContext.ACC_TXN_HEAD_MAPPING
                //                                       where item1.TXN_ID == ModelToAdd.TXN_ID && item1.CASH_CHQ.Contains("Q") && item1.CREDIT_DEBIT == "D"
                //                                       select item1.HEAD_ID).FirstOrDefault();
                //         ModelToAddDebit[j].AMOUNT = ModelToAddCredit[i].AMOUNT;
                //         ModelToAddDebit[j].CREDIT_DEBIT = "D";
                //         ModelToAddDebit[j].CASH_CHQ = "Q";
                //         ModelToAddDebit[j].NARRATION = "Cheque No:" + billMaster.CHQ_NO + " encashed";
                //         ModelToAddDebit[j].ADMIN_ND_CODE = billMaster.ADMIN_ND_CODE;
                //         ModelToAddDebit[j].MAST_CON_ID = null;
                //         ModelToAddDebit[j].IMS_PR_ROAD_CODE = null;
                //         ModelToAddDebit[j].IMS_AGREEMENT_CODE = null;
                //         ModelToAddDebit[j].MAS_FA_CODE = null;
                //         ModelToAddDebit[j].FINAL_PAYMENT = null;
                //         ModelToAddDebit[j].MAST_DISTRICT_CODE = null;

                //         dbContext.ACC_BILL_DETAILS.Add(ModelToAddDebit[j]);

                //         j = j++;

                //         #region update the cheque IS_CHQ_ENCASHED_NA status to "True"

                //         ACC_CHEQUES_ISSUED old_model = new ACC_CHEQUES_ISSUED();
                //         old_model = dbContext.ACC_CHEQUES_ISSUED.Where(x => x.BILL_ID == item).FirstOrDefault();
                //         if (old_model != null)
                //         {
                //             old_model.IS_CHQ_ENCASHED_NA = true;
                //             old_model.NA_BILL_ID = ModelToAdd.BILL_ID;
                //             dbContext.Entry(old_model).State = System.Data.Entity.EntityState.Modified;
                //         }
                //         #endregion

                //     }
                //     #endregion

                // }

                // //#region Add Details to acc_map_ack_dpiu_details

                // //if (model.NA_BILL_ID == null)//To add the ack cheque details only once
                // //{
                // //    ACC_MAP_ACK_DPIU_DETAILS ackChequeDetailsModel = new ACC_MAP_ACK_DPIU_DETAILS();
                // //    ackChequeDetailsModel.NA_BILL_ID = ModelToAdd.BILL_ID;
                // //    ackChequeDetailsModel.Bill_Ack_Year = year;
                // //    ackChequeDetailsModel.Bill_Ack_Month = month;
                // //    ackChequeDetailsModel.Admin_DPIU_Code = billMaster.ADMIN_ND_CODE;
                // //    dbContext.ACC_MAP_ACK_DPIU_DETAILS.Add(ackChequeDetailsModel);
                // //}                 

                // //#endregion

                ////if master bill entry already entered update its amount as per new bill detail voucher
                // if (model.NA_BILL_ID != 0)
                // {
                //     #region update the cheque Amount & gross amount in bill master

                //     ACC_BILL_MASTER billMasterOld = new ACC_BILL_MASTER();
                //     billMasterOld = dbContext.ACC_BILL_MASTER.Where(x => x.BILL_ID == model.NA_BILL_ID).FirstOrDefault();
                //     if (billMasterOld != null)
                //     {
                //        decimal  masterAmount = billMasterOld.GROSS_AMOUNT;

                //         billMasterOld.CHQ_AMOUNT = masterAmount + (amountToAdd ) - (amountToRemove);
                //         billMasterOld.GROSS_AMOUNT = billMasterOld.CHQ_AMOUNT;
                //         billMasterOld.BILL_FINALIZED = finalize ? "Y" : "N";
                //         billMasterOld.BILL_DATE = objCommon.GetStringToDateTime(model.BILL_DATE);
                //         billMasterOld.BILL_NO = model.BILL_NO;
                //         dbContext.Entry(billMasterOld).State = System.Data.Entity.EntityState.Modified;
                //     }
                //     #endregion
                // }

                //Old code Before SP commented by Abhishek kamble 22-Aug-2014 end


                //New code Using SP Added by Abhishek kamble 22-Aug-2014 start
                //Create object of data table.                             
                DataTable AckBillIds = new DataTable();
                //Create Column
                AckBillIds.Columns.Add("Bill_id_Ack", typeof(long));
                if (array_bill_id != null)
                {
                    foreach (long billId in array_bill_id)
                    {
                        AckBillIds.Rows.Add(new object[] { billId });
                    }
                }

                //SP -call  USP_ACC_ACK_UNACK_CHEQUE_DETAILS 
                //var Status = dbContext.Database.SqlQuery<string>("EXEC omms.USP_ACC_ACK_UNACK_CHEQUE_DETAILS @SRRDANDCODE,@DPIUCODE,@Month,@Year,@FUNDTYPE,@FLAGACK,@FLAGAFIN,@DTVOUCHER,@VOUCHERNO,@NA_BILL_ID,@Billids,@PrmUserid,PrmUserIP",
                //                 new SqlParameter("SRRDANDCODE", PMGSYSession.Current.AdminNdCode),
                //                 new SqlParameter("DPIUCODE", model.DPIU_CODE),
                //                 new SqlParameter("Month", model.BILL_MONTH_VOUCHER),
                //                 new SqlParameter("Year", model.BILL_YEAR_VOUCHER),
                //                 new SqlParameter("FUNDTYPE", PMGSYSession.Current.FundType),
                //                 new SqlParameter("FLAGACK", "A"),
                //                 new SqlParameter("FLAGAFIN", finalize),
                //                 new SqlParameter("DTVOUCHER", model.BILL_DATE),
                //                 new SqlParameter("VOUCHERNO", model.BILL_NO),
                //                 new SqlParameter("NA_BILL_ID", model.NA_BILL_ID),
                //                 new SqlParameter("Billids", AckBillIds),
                //                 new SqlParameter("PrmUserid", PMGSYSession.Current.UserId),
                //                 new SqlParameter("PrmUserIP", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"])
                //                 );

                string status = String.Empty;
                SqlConnection storeConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["PMGSYConnection"].ConnectionString);
                using (SqlCommand command = storeConnection.CreateCommand())
                {
                    command.Connection = storeConnection;
                    storeConnection.Open();
                    command.CommandText = "omms.USP_ACC_ACK_UNACK_CHEQUE_DETAILS";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@SRRDANDCODE", SqlDbType.Int)).Value = PMGSYSession.Current.AdminNdCode;
                    command.Parameters.Add(new SqlParameter("@DPIUCODE", SqlDbType.Int)).Value = model.DPIU_CODE;
                    command.Parameters.Add(new SqlParameter("@Month", SqlDbType.Int)).Value = model.BILL_MONTH_VOUCHER;
                    command.Parameters.Add(new SqlParameter("@Year", SqlDbType.Int)).Value = model.BILL_YEAR_VOUCHER;
                    command.Parameters.Add(new SqlParameter("@FUNDTYPE", SqlDbType.Char)).Value = PMGSYSession.Current.FundType;
                    command.Parameters.Add(new SqlParameter("@FLAGACK", SqlDbType.Char)).Value = "A";
                    command.Parameters.Add(new SqlParameter("@FLAGAFIN", SqlDbType.Char)).Value = (finalize == true ? "Y" : "N");
                    command.Parameters.Add(new SqlParameter("@DTVOUCHER", SqlDbType.DateTime)).Value = model.BILL_DATE;
                    command.Parameters.Add(new SqlParameter("@VOUCHERNO", SqlDbType.VarChar)).Value = model.BILL_NO;
                    command.Parameters.Add(new SqlParameter("@NA_BILL_ID", SqlDbType.BigInt)).Value = model.NA_BILL_ID;
                    command.Parameters.AddWithValue("@Billids", AckBillIds).SqlDbType = SqlDbType.Structured;
                    command.Parameters.Add(new SqlParameter("@PrmUserid", SqlDbType.BigInt)).Value = PMGSYSession.Current.UserId;
                    command.Parameters.Add(new SqlParameter("@PrmUserIP", SqlDbType.VarChar)).Value = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    // Added by Srishti on 08-03-2023  
                    command.Parameters.Add(new SqlParameter("@AccType", SqlDbType.VarChar)).Value = model.ACC_TYPE;
                    status = command.ExecuteScalar().ToString();
                    storeConnection.Close();
                }



                if (model.NA_BILL_ID == 0)
                {
                    int fiscalYear = 0;
                    if (model.BILL_MONTH_VOUCHER <= 3)
                    {
                        fiscalYear = (model.BILL_YEAR_VOUCHER - 1);
                    }
                    else
                    {
                        fiscalYear = model.BILL_YEAR_VOUCHER;
                    }


                    ACC_VOUCHER_NUMBER_MASTER oldVoucherNumberModel = new ACC_VOUCHER_NUMBER_MASTER();
                    oldVoucherNumberModel = dbContext.ACC_VOUCHER_NUMBER_MASTER.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && x.FUND_TYPE == PMGSYSession.Current.FundType && x.BILL_TYPE == "P" && x.FISCAL_YEAR == fiscalYear).FirstOrDefault();
                    ACC_VOUCHER_NUMBER_MASTER newMvoucherNumberModel = new ACC_VOUCHER_NUMBER_MASTER();

                    newMvoucherNumberModel.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                    newMvoucherNumberModel.FUND_TYPE = PMGSYSession.Current.FundType;
                    newMvoucherNumberModel.BILL_TYPE = "P";
                    newMvoucherNumberModel.FISCAL_YEAR = fiscalYear;
                    newMvoucherNumberModel.SLNO = oldVoucherNumberModel.SLNO + 1;

                    dbContext.Entry(oldVoucherNumberModel).CurrentValues.SetValues(newMvoucherNumberModel);
                }
                dbContext.SaveChanges();



                // dbContext.SaveChanges();    
                //    scope.Complete();      

                if (status == "-111")//Chqs fina/unfina successfully.
                {
                    return "1";
                }
                else
                {//Error Occured While fina/unfina records
                    return "-999";
                }
                //New code Using SP Added by Abhishek kamble 22-Aug-2014 end


            }

            //  }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error while cheque acknowledgement");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// BAL method to get ack details
        /// </summary>
        /// <param name="bill_Id"> bill id of the acknowledgement entry</param>
        /// <returns>bill number and date & finalization status </returns>
        public String GetAcknowledgedCheuesDetails(long bill_Id)
        {
            CommonFunctions common = new CommonFunctions();
            dbContext = new PMGSYEntities();
            try 
            { 
               ACC_BILL_MASTER masterDetails =new ACC_BILL_MASTER ();

               if (dbContext.ACC_BILL_MASTER.Any(m => m.BILL_ID == bill_Id))
               {
                   masterDetails = dbContext.ACC_BILL_MASTER.Where(c => c.BILL_ID == bill_Id).FirstOrDefault();
                   return common.GetDateTimeToString(masterDetails.BILL_DATE) + "$" + masterDetails.BILL_NO + "$" + masterDetails.BILL_FINALIZED;
               }
               else
               {
                   return "";
               }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error while getting cheque acknowledgement details");
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
               
            }
        
        }

        public Array ListVoucherDetailsDAL(int? page, int? rows, string sidx, string sord, out long totalRecords, CheckAckSelectionModel model)
        {
            dbContext = new PMGSYEntities();
            CommonFunctions objCommon = new CommonFunctions();
            try
            {
                var lstVoucherDetails = dbContext.SP_ACC_Display_Vouchers_Acknowledged_details(PMGSYSession.Current.FundType, PMGSYSession.Current.AdminNdCode, model.BILL_MONTH, model.BILL_YEAR, model.DPIU, model.ACCOUNT_TYPE).ToList();//.Where(m=>m.Amount > 0).ToList();, model.ACCOUNT_TYPE

                totalRecords = lstVoucherDetails.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "BILL_NO":
                                lstVoucherDetails = lstVoucherDetails.OrderBy(m => m.BILL_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "M_Year":
                                lstVoucherDetails = lstVoucherDetails.OrderBy(m => m.M_Year).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "Bill_Date":
                                lstVoucherDetails = lstVoucherDetails.OrderBy(m => m.Bill_Date).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "ADMIN_ND_NAME":
                                lstVoucherDetails = lstVoucherDetails.OrderBy(m => m.ADMIN_ND_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "Amount":
                                lstVoucherDetails = lstVoucherDetails.OrderBy(m => m.Amount).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstVoucherDetails = lstVoucherDetails.OrderBy(m => m.ADMIN_ND_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "BILL_NO":
                                lstVoucherDetails = lstVoucherDetails.OrderByDescending(m => m.BILL_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "M_Year":
                                lstVoucherDetails = lstVoucherDetails.OrderByDescending(m => m.M_Year).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "Bill_Date":
                                lstVoucherDetails = lstVoucherDetails.OrderByDescending(m => m.Bill_Date).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "ADMIN_ND_NAME":
                                lstVoucherDetails = lstVoucherDetails.OrderByDescending(m => m.ADMIN_ND_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "Amount":
                                lstVoucherDetails = lstVoucherDetails.OrderByDescending(m => m.Amount).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstVoucherDetails = lstVoucherDetails.OrderByDescending(m => m.ADMIN_ND_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstVoucherDetails = lstVoucherDetails.OrderBy(m => m.ADMIN_ND_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                var gridData = lstVoucherDetails.Select(lstVoucher =>
                    new
                    {
                        lstVoucher.BILL_NO,
                        lstVoucher.Bill_Date,
                        lstVoucher.M_Year,
                        lstVoucher.ADMIN_ND_NAME,
                        lstVoucher.Amount,
                        lstVoucher.Finalized,
                        lstVoucher.Bill_id,
                        lstVoucher.TXN_ID
                    }).ToArray();

                return gridData.Select(m => new
                {
                    cell = new[]
                    {
                        m.BILL_NO.ToString(),
                        m.Bill_Date.ToString(),
                        m.M_Year.ToString(),
                        // Added by Srishti on 08-03-2023
                        (m.TXN_ID == 212) ? "SNA" : (m.TXN_ID == 3193) ? "Holding" : (m.TXN_ID == 3194) ? "Security Deposit Account" : "--",
                        m.ADMIN_ND_NAME.ToString(),
                        m.Amount.ToString(),
                        m.Finalized=="Y"?"<span class='ui-icon ui-icon-locked ui-align-center' style='cursor:default'></span>":"<span style='cursor:default' class='ui-icon ui-icon-unlocked ui-align-center'></span>",
                        m.Finalized=="Y"?"<a href='#' title='Click here to add View Acknowledged Voucher Details' class='ui-icon ui-icon-zoomin ui-align-center' onClick=ViewVoucherDetails('" +URLEncrypt.EncryptParameters1(new string[]{"BillId="+m.Bill_id.ToString().Trim()})+"'); return false;'>View Voucher Details</a>":"<center><table><tr><td style='border-color:white'> <a href='#' title='Click here to Acknowledged/Finalize Voucher Details' class='ui-icon ui-icon-circle-plus ui-align-center' onClick=AddVoucherDetails('" +URLEncrypt.EncryptParameters1(new string[]{"BillId="+m.Bill_id.ToString().Trim()})+"'); return false;'>Add Acknowledged Voucher Details</a></td>"+"<td style='border-color:white'> <a href='#' title='Click here to UnAcknowledge Voucher Details' class='ui-icon ui-icon-circle-minus ui-align-center' onClick=UnAcknowledgeVoucherDetails('" +URLEncrypt.EncryptParameters1(new string[]{"BillId="+m.Bill_id.ToString().Trim()})+"'); return false;'> UnAcknowledged Voucher Details</a></td>"+"<td style='border-color:white'><a href='#' title='Click here to Delete Acknowledged Voucher Details' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteChqAckVoucherDetails('" +URLEncrypt.EncryptParameters1(new string[]{"BillId="+m.Bill_id.ToString().Trim()})+"'); return false;'>Delete Acknowledged Voucher Details</a></td><tr></table></center>"
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error while getting cheque acknowledgement details");
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }

            }
        }


        //added by Ashish Markande on 21/08/2013
        public bool checkMonthClose(CheckAckSelectionModel model,ref string message)
        {
            dbContext = new PMGSYEntities();
            int DPIUCode = Convert.ToInt16(model.DPIU);
            int accMonth = Convert.ToInt16(model.BILL_MONTH);
            int accYear = Convert.ToInt16(model.BILL_YEAR);
            //string fundType = PMGSYSession.Current.FundType;
            String dpiuName = String.Empty;
            try
            {

                dpiuName = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == model.DPIU).Select(m => m.ADMIN_ND_NAME).FirstOrDefault();
                if (dbContext.ACC_RPT_MONTHWISE_SUMMARY.Where(m => m.ADMIN_ND_CODE == DPIUCode && m.ACC_MONTH == accMonth && m.ACC_YEAR == accYear && m.FUND_TYPE == model.fundType).Any())
                {                    
                    return true;
                }
                else
                {
                    message = dpiuName +" has not closed the month hence cheque can not be acknowledge.";
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

        public CheckAckSelectionModel GetSelectionDetails(int billId)
        {
            dbContext = new PMGSYEntities();
            CheckAckSelectionModel model = new CheckAckSelectionModel();
            try
            {
                
               //Old Code 16-July-2014
                //if ( dbContext.ACC_CHEQUES_ISSUED.Where(m => m.NA_BILL_ID == billId).Any())
                //{
                //    long? billID = dbContext.ACC_CHEQUES_ISSUED.Where(m => m.NA_BILL_ID == billId).Select(m => m.BILL_ID).First();
                //    ACC_BILL_MASTER billMaster = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == billID).FirstOrDefault();
                    
                //    if (billMaster != null)
                //    {
                //        model.BILL_MONTH = billMaster.BILL_MONTH;
                //        model.BILL_YEAR = billMaster.BILL_YEAR;
                //        model.DPIU = Convert.ToInt16(billMaster.ADMIN_ND_CODE);
                //    }
                //}
                //else
                //{
                //    long ackBillId = dbContext.ACC_CHEQUES_ISSUED.Where(m => m.BILL_ID == billId).Select(m=>m.BILL_ID).FirstOrDefault();
                //    ACC_BILL_MASTER billMaster = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == ackBillId).FirstOrDefault();
                    
                //    if (billMaster!= null)
                //    {
                //        model.BILL_MONTH = billMaster.BILL_MONTH;
                //        model.BILL_YEAR = billMaster.BILL_YEAR;
                //        model.DPIU = Convert.ToInt16(billMaster.ADMIN_ND_CODE);
                //    }                    
                //}
                //return model;
                //New Code 16-July-2014 to get bill details from DPIU code
                if (dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == billId).Any())
                {
                    ACC_BILL_MASTER billMaster = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == billId).FirstOrDefault();
                    model.BILL_MONTH = billMaster.BILL_MONTH;
                    model.BILL_YEAR = billMaster.BILL_YEAR;
                    model.DPIU = Convert.ToInt16(billMaster.CHALAN_NO);//DPIU CODE
                }
                return model;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error while getting cheque acknowledgement details");
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
               
            }
        }

        public String ValidatePreviousCheques(CheckAckSelectionModel model, ref string message)
        {
            dbContext = new PMGSYEntities();
            try
            {
                if (model.BILL_MONTH == 1)
                {
                    model.BILL_MONTH = 12;
                    model.BILL_YEAR = (short)(model.BILL_YEAR - 1);
                }
                else
                {
                    model.BILL_MONTH = (short)(model.BILL_MONTH - 1);
                    //model.BILL_YEAR = (short)(model.BILL_YEAR - 1);
                }
                if (dbContext.ACC_BILL_MASTER.Any(m => m.BILL_MONTH == model.BILL_MONTH && m.BILL_YEAR == model.BILL_YEAR && m.BILL_TYPE == "P" && m.ADMIN_ND_CODE == model.DPIU && m.BILL_FINALIZED == "Y"))
                {
                    var billID =  dbContext.ACC_BILL_MASTER.Where(m => m.BILL_MONTH == model.BILL_MONTH && m.BILL_YEAR == model.BILL_YEAR && m.BILL_TYPE == "P" && m.ADMIN_ND_CODE == model.DPIU).Select(m => m.BILL_ID);
                    var chkBillId = (from item in dbContext.ACC_CHEQUES_ISSUED
                                      where 
                                      billID.Contains(item.BILL_ID)
                                      select item.NA_BILL_ID).Distinct();
                    if(chkBillId.Contains(null))
                    {
                        return "222";
                    }

                    long? billCode = (chkBillId.OrderBy(m=>m.Value).First());
                    
                    if (dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == billCode).Any(m => m.BILL_FINALIZED == "N"))
                    {
                        return "222";
                    }
                }
                return "111";
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
               return "333";
                throw new Exception("Error while getting cheque acknowledgement details");
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
               
            }
        }

        //Added By Ashish Markande To unacknowledge cheques.
        public string UnauthrizeVoucher(long[] array_bill_id, CheckAckModel model)
        {
            PMGSYEntities dbContext = new PMGSYEntities();

            #region Begin Of Transaction
            //using (TransactionScope objTransaction = new TransactionScope())
            //{
            try
            {
                //Added By Abhishek kamble 17June2015
                if (dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Select(s => s.MAST_ND_TYPE).FirstOrDefault() == "D")
                {
                    return "-777";
                }

                //int BillId = 0;                         

                //foreach (int item in array_bill_id)
                //{
                //    BillId = item;
                //    ACC_CHEQUES_ISSUED objChequeIssued = dbContext.ACC_CHEQUES_ISSUED.Where(m => m.BILL_ID == BillId).FirstOrDefault();
                //    objChequeIssued.IS_CHQ_ENCASHED_NA = false;
                //    objChequeIssued.NA_BILL_ID = null;
                //    dbContext.Entry(objChequeIssued).State = System.Data.Entity.EntityState.Modified;
                //    dbContext.SaveChanges();          

                //}             
                //return true;
                short month = 0;
                short year = 0;
                short day = 0;

                //long? NaBillId = 0;
                //long BillId = 0;
                //decimal amountToRemove = 0;
                //decimal totalGrossAmount = 0;

                ACC_CHEQUES_ISSUED objChequeIssued = dbContext.ACC_CHEQUES_ISSUED.Where(m => array_bill_id.Contains(m.BILL_ID)).FirstOrDefault();

                //objChequeIssued==null check by Abhishek kamble 25-Aug-2014
                if ((objChequeIssued == null) || (objChequeIssued.IS_CHQ_ENCASHED_NA == false && objChequeIssued.NA_BILL_ID == null))
                {
                    return "-444";//Already unacknowledge
                }



                ACC_BILL_MASTER objBillMaster;
                //check if allready finalized
                if (model.NA_BILL_ID != 0)
                {
                    objBillMaster = new ACC_BILL_MASTER();

                    objBillMaster = dbContext.ACC_BILL_MASTER.Where(c => c.BILL_ID == model.NA_BILL_ID).First();

                    if (objBillMaster.BILL_FINALIZED == "Y")
                    {

                        //already finalized unfinalize the voucher first !!!
                        return "-111";

                    }
                }

                day = Convert.ToInt16(model.BILL_DATE.Split('/')[0]);
                month = Convert.ToInt16(model.BILL_DATE.Split('/')[1]);
                year = Convert.ToInt16(model.BILL_DATE.Split('/')[2]);

                if (!dbContext.ACC_RPT_MONTHWISE_SUMMARY.Where(x => x.ADMIN_ND_CODE == model.DPIU_CODE &&
                   x.ACC_MONTH == month && x.ACC_YEAR == year && x.FUND_TYPE == PMGSYSession.Current.FundType).Any())
                {
                    return "-222";  //month is not closed
                }

                DateTime endOfMonth = new DateTime(year, month, DateTime.DaysInMonth(year, month));

                if (day != endOfMonth.Day)
                {
                    return "-333";  //not last day of the month
                }

                //Old code Before SP commented by Abhishek kamble 23-Aug-2014 start                    
                //ACC_BILL_DETAILS[] ackBilldetailsCredit = new ACC_BILL_DETAILS[array_bill_id.Length];
                //ACC_BILL_DETAILS[] ackBilldetailsDebit = new ACC_BILL_DETAILS[array_bill_id.Length];
                //ACC_CHEQUES_ISSUED[] old_model = new ACC_CHEQUES_ISSUED[array_bill_id.Length];
                //ACC_BILL_MASTER billDetails = new ACC_BILL_MASTER();
                //int k = 0;
                //int i = 0, j = 0;
                //billDetails.TXN_ID = dbContext.ACC_MASTER_TXN.Where(x => x.BILL_TYPE == "A" && x.FUND_TYPE == PMGSYSession.Current.FundType && x.OP_LVL_ID == PMGSYSession.Current.LevelId && x.IS_OPERATIONAL == true).Select(d => d.TXN_ID).First();


                //foreach (var item in array_bill_id)
                //{
                //    var MasterData = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == item).FirstOrDefault();

                //    NaBillId = dbContext.ACC_CHEQUES_ISSUED.Where(m => m.BILL_ID == item).Select(m => m.NA_BILL_ID).FirstOrDefault();
                //    BillId = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == NaBillId).Select(m => m.BILL_ID).FirstOrDefault();
                //    amountToRemove = 0;
                //    if (MasterData.TXN_ID == 229 || MasterData.TXN_ID == 625 || MasterData.TXN_ID == 824)
                //    {
                //        amountToRemove = amountToRemove - MasterData.CHQ_AMOUNT;
                //    }
                //    else
                //    {
                //        amountToRemove = amountToRemove + MasterData.CHQ_AMOUNT;
                //    }



                //    billDetails = dbContext.ACC_BILL_MASTER.SingleOrDefault(p => p.BILL_ID == BillId);
                //    totalGrossAmount = billDetails.GROSS_AMOUNT;
                //    //amountToRemove = MasterData.CHQ_AMOUNT;

                //    totalGrossAmount = totalGrossAmount - amountToRemove;

                //    billDetails.CHQ_AMOUNT = totalGrossAmount;
                //    billDetails.GROSS_AMOUNT = totalGrossAmount;
                //    //  amountToRemove = totalGrossAmount;
                //    dbContext.Entry(billDetails).State = System.Data.Entity.EntityState.Modified;
                //    dbContext.SaveChanges();


                //    short maxTrnsNo = 0;
                //    if (dbContext.ACC_BILL_DETAILS.Where(t => t.BILL_ID == (billDetails.BILL_ID)).Any())
                //    {
                //        maxTrnsNo = dbContext.ACC_BILL_DETAILS.Where(t => t.BILL_ID == (billDetails.BILL_ID)).Max(c => c.TXN_NO);
                //    }

                //    maxTrnsNo = Convert.ToInt16(maxTrnsNo + 1);

                //    //get debit and credit rows
                //    ackBilldetailsCredit[k] = new ACC_BILL_DETAILS();
                //    ackBilldetailsDebit[k] = new ACC_BILL_DETAILS();

                //    ackBilldetailsCredit[k].TXN_NO = maxTrnsNo;
                //    ackBilldetailsCredit[k].BILL_ID = BillId;
                //    ackBilldetailsCredit[k].HEAD_ID = (from item1 in dbContext.ACC_TXN_HEAD_MAPPING
                //                                       where item1.TXN_ID == billDetails.TXN_ID && item1.CASH_CHQ.Contains("Q") && item1.CREDIT_DEBIT == "C"
                //                                       select item1.HEAD_ID).FirstOrDefault();
                //    if (MasterData.TXN_ID == 229 || MasterData.TXN_ID == 625 || MasterData.TXN_ID == 824)
                //    {
                //        //Commented by Abhishek as in bill details for Un-Ack. of chq Amount wrong amt was entering. hence taken from bill Master Table.31-July-2014 
                //        // ackBilldetailsCredit[k].AMOUNT = (dbContext.ACC_BILL_DETAILS.Where(x => x.BILL_ID == item && x.CREDIT_DEBIT == "D").Sum(x => x.AMOUNT));
                //        ackBilldetailsCredit[k].AMOUNT = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == item).Select(s => s.CHQ_AMOUNT).FirstOrDefault();
                //    }
                //    else
                //    {
                //        //Commented by Abhishek as in bill details for Un-Ack. of chq Amount wrong amt was entering. hence taken from bill Master Table.31-July-2014 
                //        //ackBilldetailsCredit[k].AMOUNT = -(dbContext.ACC_BILL_DETAILS.Where(x => x.BILL_ID == item && x.CREDIT_DEBIT == "D").Sum(x => x.AMOUNT));
                //        ackBilldetailsCredit[k].AMOUNT = -(dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == item).Select(s => s.CHQ_AMOUNT).FirstOrDefault());
                //    }
                //    ackBilldetailsCredit[k].ADMIN_ND_CODE = MasterData.ADMIN_ND_CODE;
                //    ackBilldetailsCredit[k].CREDIT_DEBIT = "C";
                //    ackBilldetailsCredit[k].CASH_CHQ = "Q";
                //    ackBilldetailsCredit[k].NARRATION = "Cheque No:" + dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == MasterData.BILL_ID).Select(m => m.CHQ_NO).FirstOrDefault() + "is unacknowledge";
                //    ackBilldetailsCredit[k].MAST_CON_ID = null;
                //    ackBilldetailsCredit[k].IMS_PR_ROAD_CODE = null;
                //    ackBilldetailsCredit[k].IMS_AGREEMENT_CODE = null;
                //    ackBilldetailsCredit[k].MAS_FA_CODE = null;
                //    ackBilldetailsCredit[k].FINAL_PAYMENT = null;
                //    ackBilldetailsCredit[k].MAST_DISTRICT_CODE = null;

                //    dbContext.ACC_BILL_DETAILS.Add(ackBilldetailsCredit[i]);

                //    //i = i++;
                //    i++;
                //    maxTrnsNo = Convert.ToInt16(maxTrnsNo + 1);
                //    ackBilldetailsDebit[k].TXN_NO = maxTrnsNo;
                //    ackBilldetailsDebit[k].BILL_ID = ackBilldetailsCredit[k].BILL_ID;
                //    ackBilldetailsDebit[k].HEAD_ID = (from item1 in dbContext.ACC_TXN_HEAD_MAPPING
                //                                      where item1.TXN_ID == billDetails.TXN_ID && item1.CASH_CHQ.Contains("Q") && item1.CREDIT_DEBIT == "D"
                //                                      select item1.HEAD_ID).FirstOrDefault();
                //    ackBilldetailsDebit[k].AMOUNT = ackBilldetailsCredit[k].AMOUNT;
                //    ackBilldetailsDebit[k].ADMIN_ND_CODE = ackBilldetailsCredit[k].ADMIN_ND_CODE;
                //    ackBilldetailsDebit[k].CREDIT_DEBIT = "D";
                //    ackBilldetailsDebit[k].CASH_CHQ = "Q";
                //    ackBilldetailsDebit[k].NARRATION = ackBilldetailsCredit[k].NARRATION;
                //    ackBilldetailsDebit[k].MAST_CON_ID = null;
                //    ackBilldetailsDebit[k].IMS_PR_ROAD_CODE = null;
                //    ackBilldetailsDebit[k].IMS_AGREEMENT_CODE = null;
                //    ackBilldetailsDebit[k].MAS_FA_CODE = null;
                //    ackBilldetailsDebit[k].FINAL_PAYMENT = null;
                //    ackBilldetailsDebit[k].MAST_DISTRICT_CODE = null;

                //    dbContext.ACC_BILL_DETAILS.Add(ackBilldetailsDebit[j]);

                //    //j = j++;
                //    j++;
                //    // update their status as not acknowleged

                //    #region update the cheque IS_CHQ_ENCASHED_NA status to "false"

                //    old_model[k] = new ACC_CHEQUES_ISSUED();
                //    old_model[k] = dbContext.ACC_CHEQUES_ISSUED.Where(x => x.BILL_ID == item).FirstOrDefault();
                //    if (old_model[k] != null)
                //    {
                //        old_model[k].IS_CHQ_ENCASHED_NA = false;
                //        old_model[k].NA_BILL_ID = null;
                //        dbContext.Entry(old_model[k]).State = System.Data.Entity.EntityState.Modified;
                //        dbContext.SaveChanges();
                //    }
                //    #endregion

                //    k++;

                //}
                //Old code Before SP commented by Abhishek kamble 23-Aug-2014 end                    

                //New code Using SP Added by Abhishek kamble 23-Aug-2014 start
                //Create object of data table.                             
                DataTable AckBillIds = new DataTable();
                //Create Column
                AckBillIds.Columns.Add("Bill_id_Ack", typeof(long));
                foreach (long billId in array_bill_id)
                {
                    AckBillIds.Rows.Add(new object[] { billId });
                }
                string status = String.Empty;
                SqlConnection storeConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["PMGSYConnection"].ConnectionString);
                using (SqlCommand command = storeConnection.CreateCommand())
                {
                    command.Connection = storeConnection;
                    storeConnection.Open();
                    command.CommandText = "omms.USP_ACC_ACK_UNACK_CHEQUE_DETAILS";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@SRRDANDCODE", SqlDbType.Int)).Value = PMGSYSession.Current.AdminNdCode;
                    command.Parameters.Add(new SqlParameter("@DPIUCODE", SqlDbType.Int)).Value = model.DPIU_CODE;
                    command.Parameters.Add(new SqlParameter("@Month", SqlDbType.Int)).Value = model.BILL_MONTH_VOUCHER;
                    command.Parameters.Add(new SqlParameter("@Year", SqlDbType.Int)).Value = model.BILL_YEAR_VOUCHER;
                    command.Parameters.Add(new SqlParameter("@FUNDTYPE", SqlDbType.Char)).Value = PMGSYSession.Current.FundType;
                    command.Parameters.Add(new SqlParameter("@FLAGACK", SqlDbType.Char)).Value = "U";
                    command.Parameters.Add(new SqlParameter("@FLAGAFIN", SqlDbType.Char)).Value = "N";
                    command.Parameters.Add(new SqlParameter("@DTVOUCHER", SqlDbType.DateTime)).Value = model.BILL_DATE;
                    command.Parameters.Add(new SqlParameter("@VOUCHERNO", SqlDbType.VarChar)).Value = model.BILL_NO;
                    command.Parameters.Add(new SqlParameter("@NA_BILL_ID", SqlDbType.BigInt)).Value = model.NA_BILL_ID;
                    command.Parameters.AddWithValue("@Billids", AckBillIds).SqlDbType = SqlDbType.Structured;
                    command.Parameters.Add(new SqlParameter("@PrmUserid", SqlDbType.BigInt)).Value = PMGSYSession.Current.UserId;
                    command.Parameters.Add(new SqlParameter("@PrmUserIP", SqlDbType.VarChar)).Value = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    // Added by Srishti on 08-03-2023  
                    command.Parameters.Add(new SqlParameter("@AccType", SqlDbType.VarChar)).Value = model.ACC_TYPE;
                    status = command.ExecuteScalar().ToString();
                    storeConnection.Close();
                }

                dbContext.SaveChanges();

                if (status == "-111")
                {
                    return "2";
                }
                else
                {
                    return "-999";
                }

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                //objTransaction.Dispose();
                throw new Exception("Error while unacknowleding the check");
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
            // }
            #endregion
            //     }    txn scope
        }

        public bool CheckDuplicateVoucher(Int16 month, Int16 year, Int32 adminNdCode, Int16 levelId, String fundType, String billType, String billNo,string finalise)
        {
            dbContext = new PMGSYEntities();

            try
            {
                return dbContext.ACC_BILL_MASTER.Where(m => m.ADMIN_ND_CODE == adminNdCode && m.BILL_MONTH == month && m.BILL_YEAR == year && m.LVL_ID == levelId && m.FUND_TYPE == fundType && m.BILL_NO == billNo && m.BILL_FINALIZED == finalise).Any();

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

        //Added By Abhishek kamble to delete cheque Ack Details.
        public bool DeleteChequeAckVocherDetails(int BIllMonth, int BIllYear, int AdminNdCode, Int64 BillID,ref string message)
        {
                PMGSYEntities dbContext = new PMGSYEntities();
                try
                {
                    dbContext.USP_ACC_DELETE_CHEQUE_ACKNOWLEDGEMENT_DETAILS(PMGSYSession.Current.AdminNdCode, BIllMonth, BIllYear, BillID);
                    dbContext.SaveChanges();                
                    message = "Cheque Acknowledgement details deleted successfully.";                        
                    return true;                    
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

        public bool checkSRRDAMonthCloseDAL(int month, int year,ref string message)
        {
            dbContext = new PMGSYEntities();
            try
            {
                if (dbContext.ACC_RPT_MONTHWISE_SUMMARY.Where(m => m.ADMIN_ND_CODE == PMGSYSession.Current.ParentNDCode && m.ACC_MONTH == month && m.ACC_YEAR == year && m.FUND_TYPE == PMGSYSession.Current.FundType).Any())
                {
                    message = "SRRDA has closed month and hence cannot acknowledge cheques.";
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                message = "Error occurred while cheque acknowledgement.";
                ErrorLog.LogError(ex, "checkSRRDAMonthCloseDAL");
                return false;
            }
        }
    }
   
    interface IChequeAcknowledgementDAL
    {

        Array ListChequeForAcknowledgment(PaymentFilterModel objFilter, out long totalRecords, out  List<String> SelectedIdList,ref string SrrdaBillId);
        String AcknowledgeCheues(CheckAckModel model, long[] array_bill_id, bool finalize);
        String GetAcknowledgedCheuesDetails(long bill_Id);
        Array ListVoucherDetailsDAL(int? page, int? rows, string sidx, string sord, out long totalRecords, CheckAckSelectionModel model);
        CheckAckSelectionModel GetSelectionDetails(int billId);
        String ValidatePreviousCheques(CheckAckSelectionModel model, ref string message);
        bool checkMonthClose(CheckAckSelectionModel model, ref string message);
        string UnauthrizeVoucher(long[] array_bill_id,CheckAckModel model);
        bool DeleteChequeAckVocherDetails(int BIllMonth, int BIllYear, int AdminNdCode, Int64 BillID,ref string message);
    }
}