using PMGSY.Areas.EmargDataPush;
using PMGSY.Areas.EmargDataPush.Models;
using PMGSY.Areas.RESTAPIImplementation.Models;
using PMGSY.Common;
using PMGSY.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;


namespace PMGSY.Areas.RESTAPIImplementation.DAL
{
    public class EmargPushDetailsDAL
    {

        private static string ErrorLogBasePath = System.Configuration.ConfigurationSettings.AppSettings["EmargPushDetailsErrorPath"];
        private static string ErrorLogDirectory = ErrorLogBasePath + "\\" + "ErrorLog_" + DateTime.Now.Month + "_" + DateTime.Now.Year;
        private string ErrorLogFilePath = ErrorLogDirectory + "//ErrorLog_" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + ".txt";


        PMGSYEntities dbContext = null;
        #region Push Corrected Packages to Emarg

        public dynamic GetCorrectedEmargDetailsDAL(string packageId)
        {
            //List<RoadDetails> 
            // List<RoadDetails> lstModel = new List<RoadDetails>();
            //    List<EMARG_COMPLETED_WORK_DETAILS_SERVICE> lstModel = new List<EMARG_COMPLETED_WORK_DETAILS_SERVICE>();

            //RoadDetails model = new RoadDetails();
            dbContext = new PMGSYEntities();


            try
            {
                ((System.Data.Entity.Infrastructure.IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                //  EMARG_COMPLETED_WORK_DETAILS_SERVICE model = new EMARG_COMPLETED_WORK_DETAILS_SERVICE();

                var emargStats = dbContext.USP_PUSH_CORRECTED_EMARG_DETAILS(packageId).ToList();

                return emargStats;

            }
            catch (Exception ex)
            {
                if (!Directory.Exists(ErrorLogDirectory))
                {
                    Directory.CreateDirectory(ErrorLogDirectory);
                }
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "EmargPushDetailsDAL.cs - GetCorrectedEmargDetailsDAL()");
                    sw.WriteLine("Exception : " + ex.ToString());
                    sw.WriteLine("Exception : " + ex.StackTrace);
                    if (ex.InnerException != null)
                        sw.WriteLine("Inner Exception : " + ex.InnerException.ToString());
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion


        #region Push Corrected Packages to Emarg Post DLP
        public dynamic GetCorrectedEmargDetailsPostDLPDAL(int RoadCode)
        {
            //List<RoadDetails> 
            // List<RoadDetails> lstModel = new List<RoadDetails>();
            //    List<EMARG_COMPLETED_WORK_DETAILS_SERVICE> lstModel = new List<EMARG_COMPLETED_WORK_DETAILS_SERVICE>();

            //RoadDetails model = new RoadDetails();
            dbContext = new PMGSYEntities();


            try
            {
                ((System.Data.Entity.Infrastructure.IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                //  EMARG_COMPLETED_WORK_DETAILS_SERVICE model = new EMARG_COMPLETED_WORK_DETAILS_SERVICE();

                var emargStats = dbContext.USP_PUSH_CORRECTED_EMARG_DETAILS_POST_DLP(RoadCode).ToList();

                return emargStats;

            }
            catch (Exception ex)
            {
                if (!Directory.Exists(ErrorLogDirectory))
                {
                    Directory.CreateDirectory(ErrorLogDirectory);
                }
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "EmargPushDetailsDAL.cs - GetCorrectedEmargDetailsPostDLPDAL()");
                    sw.WriteLine("Exception : " + ex.ToString());
                    sw.WriteLine("Exception : " + ex.StackTrace);
                    if (ex.InnerException != null)
                        sw.WriteLine("Inner Exception : " + ex.InnerException.ToString());
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion

        #region Push PACKAGE PIU MAPPING to Emarg
        public dynamic GetPIUMappingEmargDetailsDAL()
        {

            dbContext = new PMGSYEntities();

            try
            {
                ((System.Data.Entity.Infrastructure.IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var emargStats = dbContext.USP_EMARG_PIU_MAPPING().ToList();

                return emargStats;

            }
            catch (Exception ex)
            {
                if (!Directory.Exists(ErrorLogDirectory))
                {
                    Directory.CreateDirectory(ErrorLogDirectory);
                }
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "EmargPushDetailsDAL.cs - GetPIUMappingEmargDetailsDAL()");
                    sw.WriteLine("Exception : " + ex.ToString());
                    sw.WriteLine("Exception : " + ex.StackTrace);
                    if (ex.InnerException != null)
                        sw.WriteLine("Inner Exception : " + ex.InnerException.ToString());
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion

        #region Push Acknowledgement of Second Level Service to Emarg
        public dynamic GetSecondLevelAckDAL(string packageId)
        {
            dbContext = new PMGSYEntities();

            try
            {
                ((System.Data.Entity.Infrastructure.IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var emargStats = dbContext.USP_PUSH_ACK_SECOND_LEVEL_SERVICE(packageId).ToList();
                return emargStats;

            }
            catch (Exception ex)
            {
                if (!Directory.Exists(ErrorLogDirectory))
                {
                    Directory.CreateDirectory(ErrorLogDirectory);
                }
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "EmargPushDetailsDAL.cs - GetSecondLevelAckDAL()");
                    sw.WriteLine("Exception : " + ex.ToString());
                    sw.WriteLine("Exception : " + ex.StackTrace);
                    if (ex.InnerException != null)
                        sw.WriteLine("Inner Exception : " + ex.InnerException.ToString());
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion

        #region Push Acknowledgement of First Level Service to Emarg
        public dynamic GetFirstLevelAckDAL(string packageId)
        {
            dbContext = new PMGSYEntities();

            try
            {
                ((System.Data.Entity.Infrastructure.IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var emargStats = dbContext.USP_PUSH_ACK_FIRST_LEVEL_SERVICE(packageId).ToList();
                return emargStats;

            }
            catch (Exception ex)
            {
                if (!Directory.Exists(ErrorLogDirectory))
                {
                    Directory.CreateDirectory(ErrorLogDirectory);
                }
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "EmargPushDetailsDAL.cs - GetFirstLevelAckDAL()");
                    sw.WriteLine("Exception : " + ex.ToString());
                    sw.WriteLine("Exception : " + ex.StackTrace);
                    if (ex.InnerException != null)
                        sw.WriteLine("Inner Exception : " + ex.InnerException.ToString());
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion


        #region Push PACKAGE Emarg PIU Details
        public dynamic GetEmargPIUDetailsDAL()
        {

            dbContext = new PMGSYEntities();

            try
            {
                ((System.Data.Entity.Infrastructure.IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                var emargStats = dbContext.USP_EMARG_PIU_DETAILS().ToList();

                return emargStats;

            }
            catch (Exception ex)
            {
                if (!Directory.Exists(ErrorLogDirectory))
                {
                    Directory.CreateDirectory(ErrorLogDirectory);
                }
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "EmargPushDetailsDAL.cs - GetEmargPIUDetailsDAL()");
                    sw.WriteLine("Exception : " + ex.ToString());
                    sw.WriteLine("Exception : " + ex.StackTrace);
                    if (ex.InnerException != null)
                        sw.WriteLine("Inner Exception : " + ex.InnerException.ToString());
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion

        #region Push GetPaymentAckDetails
        public dynamic GetPaymentAckDetailsDAL()
        {

            dbContext = new PMGSYEntities();

            try
            {
                ((System.Data.Entity.Infrastructure.IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;




                //var emargStats = dbContext.EMARG_PAYMENT_MASTER.Where(m => m.IS_PORTED == true && m.OMMAS_BILL_ID != null).Select(n => new { n.CHQ_NO, n.BILL_MONTH, n.BILL_YEAR, n.BILL_DATE, n.CHQ_AMOUNT, n.AGREEMENT_CODE, n.PAYEE_NAME, n.PIU_CODE, n.MAST_CON_ID, n.ROAD_CODE, n.ACCOUNT_NUMBER, n.IFSC_CODE }).ToList();

                // List<EmargPayment> emargStats = dbContext.EMARG_PAYMENT_MASTER.Where(m => m.IS_PORTED == true && m.OMMAS_BILL_ID != null).Select(n => new EmargPayment { CHQ_NO = n.CHQ_NO, BILL_MONTH = n.BILL_MONTH, BILL_YEAR = n.BILL_YEAR, BILL_DATE = n.BILL_DATE, CHQ_AMOUNT = n.CHQ_AMOUNT, AGREEMENT_CODE = n.AGREEMENT_CODE, PAYEE_NAME = n.PAYEE_NAME, PIU_CODE = n.PIU_CODE, MAST_CON_ID = n.MAST_CON_ID, ROAD_CODE = n.ROAD_CODE, ACCOUNT_NUMBER = n.ACCOUNT_NUMBER, IFSC_CODE = n.IFSC_CODE }).ToList();

                //List<EmargPayment> emargStats = dbContext.EMARG_PAYMENT_MASTER.Where(m => m.IS_PORTED == true && m.OMMAS_BILL_ID != null).Select(n => new EmargPayment { BILL_ID = (long)n.OMMAS_BILL_ID, EMARG_VOUCHER_NO = n.CHQ_NO, MESSAGE_DATE_TIME = (n.RECEIVED_DATE_TIME.HasValue ? n.RECEIVED_DATE_TIME : System.DateTime.Now), STATUS = "ACCP", RJCT_CODE = " ", REMARKS = " " }).ToList();
                //10-01-22
                // List<EmargPayment> emargStats = dbContext.EMARG_PAYMENT_MASTER.Where(m => m.IS_PORTED == true && m.OMMAS_BILL_ID != null).Select(n => new EmargPayment { BILL_ID = (long)n.OMMAS_BILL_ID, EMARG_VOUCHER_NO = n.EMARG_VOUCHER_NO, MESSAGE_DATE_TIME = (n.RECEIVED_DATE_TIME.HasValue ? n.RECEIVED_DATE_TIME : System.DateTime.Now), STATUS = "ACCP", RJCT_CODE = " ", REMARKS = " " }).ToList();

                // List<EmargPayment> emargStats = dbContext.EMARG_PAYMENT_MASTER.Where(m => m.IS_PORTED == true && m.OMMAS_BILL_ID != null && m.EMARG_BILL_ID != null).Select(n => new EmargPayment { BILL_ID = (long)n.EMARG_BILL_ID, EMARG_VOUCHER_NO = n.EMARG_VOUCHER_NO, MESSAGE_DATE_TIME = (n.RECEIVED_DATE_TIME.HasValue ? n.RECEIVED_DATE_TIME : System.DateTime.Now), STATUS = "ACCP", RJCT_CODE = " ", REMARKS = " " }).ToList();

                List<EmargPayment> emargStats = dbContext.EMARG_PAYMENT_MASTER.Where(m =>m.VOUCHER_TYPE == "N" && m.IS_PORTED == true && m.OMMAS_BILL_ID != null && m.EMARG_BILL_ID != null).Select(n => new EmargPayment { BILL_ID = (long)n.EMARG_BILL_ID, EMARG_VOUCHER_NO = n.EMARG_VOUCHER_NO, MESSAGE_DATE_TIME = (n.RECEIVED_DATE_TIME.HasValue ? n.RECEIVED_DATE_TIME : System.DateTime.Now), ROAD_CODE = n.ROAD_CODE, VOUCHER_TYPE= n.VOUCHER_TYPE, STATUS = "ACCP" }).ToList();


                return emargStats;

            }
            catch (Exception ex)
            {
                if (!Directory.Exists(ErrorLogDirectory))
                {
                    Directory.CreateDirectory(ErrorLogDirectory);
                }
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "EmargPushDetailsDAL.cs - GetEmargPAYMENTDetailsDAL()");
                    sw.WriteLine("Exception : " + ex.ToString());
                    sw.WriteLine("Exception : " + ex.StackTrace);
                    if (ex.InnerException != null)
                        sw.WriteLine("Inner Exception : " + ex.InnerException.ToString());
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion




        public dynamic AuthorizationDetailsDAL(string auth_type, DateTime? auth_date)
        {
            dbContext = new PMGSYEntities();


            try
            {

                dbContext.Configuration.LazyLoadingEnabled = false;
                var resultStats = dbContext.USP_EMARG_SRVC_PUSH_AUTHORIZATION_DETAILS(auth_type, auth_date.Value).ToList();


                return resultStats;



            }
            catch (Exception ex)
            {
                if (!Directory.Exists(ErrorLogDirectory))
                {
                    Directory.CreateDirectory(ErrorLogDirectory);
                }
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "EmargPushDetailsDAL.cs - GetCorrectedEmargDetailsDAL()");
                    sw.WriteLine("Exception : " + ex.ToString());
                    sw.WriteLine("Exception : " + ex.StackTrace);
                    if (ex.InnerException != null)
                        sw.WriteLine("Inner Exception : " + ex.InnerException.ToString());
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public int getemargpaymentdetailsDAL(List<PaymentEmargDetails> PaymentList)
        {

            List<EMARG_PAYMENT_MASTER> paymasterlist = new List<EMARG_PAYMENT_MASTER>();

            // List<ACC_BILL_MASTER> billmasterlist = new List<ACC_BILL_MASTER>();
            CommonFunctions common = new CommonFunctions();
            int rowsaffected = 0;
            int max_txn_no = 0;


            using (TransactionScope tscaope = new TransactionScope(TransactionScopeOption.Required,
                               new System.TimeSpan(0, 15, 0)))
            {
                try
                {
                    using (PMGSYEntities dbcontext = new PMGSYEntities())
                    {
                        //var max_bill_ID = dbcontext.ACC_BILL_MASTER.Max(ac => (ac.BILL_ID));
                        //// var bill_ID_emarg = dbcontext.EMARG_PAYMENT_MASTER.Any() ? dbcontext.EMARG_PAYMENT_MASTER.Max(ad => ad.BILL_ID):0;

                        foreach (var item in PaymentList)
                        {

                            //var mast_con_ID = dbcontext.MANE_IMS_CONTRACT.Where(an => an.MANE_AGREEMENT_NUMBER == item.AGREEMENT_CODE).Min(ms => ms.MAST_CON_ID);

                            if (!dbcontext.EMARG_PAYMENT_MASTER.Any(b => b.BILL_ID == item.BILL_ID) && !paymasterlist.Any(s => s.BILL_ID == item.BILL_ID))
                            {
                                //// bill_ID_emarg++;
                                var payMaster = new EMARG_PAYMENT_MASTER
                                {

                                    BILL_ID = item.BILL_ID,//bill_ID_emarg,
                                    BILL_ID_PREVIOUS = item.BILL_ID_PREVIOUS,
                                    VOUCHER_NO = item.VOUCHER_NO,
                                    BILL_MONTH = item.BILL_MONTH,
                                    BILL_YEAR = item.BILL_YEAR,
                                    BILL_DATE = item.BILL_DATE,
                                    CHQ_NO = item.CHQ_NO,
                                    CHQ_DATE = item.CHQ_DATE,
                                    CHQ_AMOUNT = item.CHQ_AMOUNT,
                                    CASH_AMOUNT = item.CASH_AMOUNT,
                                    GROSS_AMOUNT = item.GROSS_AMOUNT,
                                    PIU_CODE = item.PIU_CODE,
                                    MAST_CON_ID = item.MAST_CON_ID,
                                    NARRATION = item.NARRATION,
                                    ROAD_CODE = item.ROAD_CODE,
                                    AGREEMENT_CODE = item.AGREEMENT_CODE,
                                    FUND_TYPE = item.FUND_TYPE,
                                    PAYEE_NAME = item.PAYEE_NAME,
                                    ACCOUNT_NUMBER = item.ACCOUNT_NUMBER,
                                    IFSC_CODE = item.IFSC_CODE,
                                    HEAD_CODE = item.HEAD_CODE,
                                    AMOUNT = item.AMOUNT

                                };
                                paymasterlist.Add(payMaster);
                                dbcontext.EMARG_PAYMENT_MASTER.Add(payMaster);



                                //var voucher_No = common.GetPaymentReceiptNumber(item.PIU_CODE, item.FUND_TYPE, "V", item.BILL_MONTH, item.BILL_YEAR);
                                //max_bill_ID++;

                                //max_txn_no = 0;
                                //var billMaster = new ACC_BILL_MASTER
                                //{
                                //    BILL_ID = max_bill_ID,
                                //    // BILL_ID_PREVIOUS = item.BILL_ID_PREVIOUS,
                                //    BILL_NO = voucher_No,
                                //    BILL_MONTH = (short)item.BILL_MONTH,
                                //    BILL_YEAR = (short)item.BILL_YEAR,
                                //    BILL_DATE = item.BILL_DATE,
                                //    TXN_ID = 1,                            // ask logic to mam
                                //    CHQ_EPAY = "Q",
                                //    BILL_FINALIZED = "Y",
                                //    LVL_ID = 5,
                                //    BILL_TYPE = "P",
                                //    ACTION_REQUIRED = "N",
                                //    CHQ_NO = item.CHQ_NO,
                                //    CHQ_DATE = item.CHQ_DATE,
                                //    CHQ_AMOUNT = item.CHQ_AMOUNT,
                                //    CASH_AMOUNT = item.CASH_AMOUNT,
                                //    GROSS_AMOUNT = item.GROSS_AMOUNT,
                                //    ADMIN_ND_CODE = item.PIU_CODE,
                                //    MAST_CON_ID = mast_con_ID,
                                //    FUND_TYPE = item.FUND_TYPE,
                                //    PAYEE_NAME = item.PAYEE_NAME,
                                //    USERID = 3,//PMGSYSession.Current.UserId,
                                //    IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]
                                //    // ACCOUNT_NUMBER = item.ACCOUNT_NUMBER,             // CON_ACCOUNT_ID
                                //    //IFSC_CODE = item.IFSC_CODE,                      // CON_ACCOUNT_ID

                                //};
                                //billmasterlist.Add(billMaster);
                                //dbcontext.ACC_BILL_MASTER.Add(billMaster);
                            }

                            if (item.TXN_NO != 0)
                            {
                                var payDetails = new EMARG_PAYMENT_DETAILS
                                {
                                    BILL_ID = item.BILL_ID,//bill_ID_emarg,
                                    TXN_NO = item.TXN_NO,
                                    HEAD_CODE_DEDUCTIONS = item.HEAD_CODE_DEDUCTIONS,
                                    AMOUNT_DEDUCTIONS = item.AMOUNT_DEDUCTIONS,
                                    EMARG_VOUCHER_NO = item.EMARG_VOUCHER_NO,
                                    OMMAS_PACKAGE_NO = item.OMMAS_PACKAGE_NO,
                                    EMARG_PACKAGE_NO = item.EMARG_PACKAGE_NO,
                                    EMARG_DATE_TIME = item.EMARG_DATE_TIME

                                };
                                dbcontext.EMARG_PAYMENT_DETAILS.Add(payDetails);
                            }

                            //    var Head_ID = dbcontext.ACC_MASTER_HEAD.Where(id => id.HEAD_CODE == item.HEAD_CODE).Select(hc => hc.HEAD_ID).FirstOrDefault();
                            //    var mast_fa_code = dbcontext.IMS_SANCTIONED_PROJECTS.Where(pr => pr.IMS_PR_ROAD_CODE == item.ROAD_CODE).Select(c => c.IMS_COLLABORATION).FirstOrDefault();
                            //    var agg_code = dbcontext.MANE_IMS_CONTRACT.Where(an => an.MANE_AGREEMENT_NUMBER == item.AGREEMENT_CODE).Min(ms => ms.MANE_PR_CONTRACT_CODE);




                            //    max_txn_no++;
                            //    var billDetails = new ACC_BILL_DETAILS
                            //    {
                            //        BILL_ID = max_bill_ID,
                            //        TXN_NO = (short)max_txn_no,
                            //        TXN_ID = 1,
                            //        HEAD_ID = Head_ID,                         // convert head code to head_ID
                            //        AMOUNT = item.AMOUNT,                                // creaditdebit -D, cash chq - Q
                            //        CREDIT_DEBIT = "D",
                            //        CASH_CHQ = "Q",
                            //        NARRATION = item.NARRATION,
                            //        IMS_PR_ROAD_CODE = item.ROAD_CODE,
                            //        MAS_FA_CODE = mast_fa_code,
                            //        IMS_AGREEMENT_CODE = agg_code,
                            //        MAST_CON_ID = mast_con_ID,              //Logic
                            //        USERID = 3,//PMGSYSession.Current.UserId,
                            //        IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]

                            //        //EMARG_VOUCHER_NO = item.EMARG_VOUCHER_NO,  
                            //        //OMMAS_PACKAGE_NO = item.OMMAS_PACKAGE_NO,
                            //        //EMARG_PACKAGE_NO = item.EMARG_PACKAGE_NO,
                            //        //EMARG_DATE_TIME = item.EMARG_DATE_TIME

                            //    };
                            //    dbcontext.ACC_BILL_DETAILS.Add(billDetails);

                            //    var Head_ID_Deduction = dbcontext.ACC_MASTER_HEAD.Where(id => id.HEAD_CODE == item.HEAD_CODE_DEDUCTIONS).Select(hc => hc.HEAD_ID).FirstOrDefault();


                            //    max_txn_no++;
                            //    var billDetailsdeduction = new ACC_BILL_DETAILS
                            //  {
                            //      BILL_ID = max_bill_ID,
                            //      TXN_NO = (short)max_txn_no,
                            //      TXN_ID = 1,
                            //      HEAD_ID = Head_ID_Deduction,
                            //      AMOUNT = item.AMOUNT_DEDUCTIONS,
                            //      CREDIT_DEBIT = "D",
                            //      CASH_CHQ = "D",                  // creaditdebit - D, cash chq - D
                            //      NARRATION = item.NARRATION,
                            //      IMS_PR_ROAD_CODE = item.ROAD_CODE,
                            //      MAS_FA_CODE = mast_fa_code,
                            //      IMS_AGREEMENT_CODE = agg_code,
                            //      MAST_CON_ID = mast_con_ID,   //Logic
                            //      USERID = 3,//PMGSYSession.Current.UserId,
                            //      IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]
                            //      //EMARG_VOUCHER_NO = item.EMARG_VOUCHER_NO,  
                            //      //OMMAS_PACKAGE_NO = item.OMMAS_PACKAGE_NO,
                            //      //EMARG_PACKAGE_NO = item.EMARG_PACKAGE_NO,
                            //      //EMARG_DATE_TIME = item.EMARG_DATE_TIME

                            //  };

                            //    dbcontext.ACC_BILL_DETAILS.Add(billDetailsdeduction);
                        }

                        rowsaffected = dbcontext.SaveChanges();
                        tscaope.Complete();
                        return rowsaffected;

                    }


                }

                catch (Exception ex)
                {
                    if (!Directory.Exists(ErrorLogDirectory))
                    {
                        Directory.CreateDirectory(ErrorLogDirectory);
                    }
                    using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                    {
                        sw.WriteLine("Date :" + DateTime.Now.ToString());
                        sw.WriteLine("Method : " + "EmargPushDetailsDAL.cs - getemargpaymentdetailsDAL()");
                        sw.WriteLine("Exception : " + ex.ToString());
                        sw.WriteLine("Exception : " + ex.StackTrace);
                        if (ex.InnerException != null)
                            sw.WriteLine("Inner Exception : " + ex.InnerException.ToString());
                        sw.WriteLine("____________________________________________________");
                        sw.Close();
                    }
                    return 0;
                }
                finally
                {
                    tscaope.Dispose();
                }
            }
        }



        #region PIU MASTER
        public List<PIUMasterViewModel> PIUMasterDetailsDAL()
        {
            dbContext = new PMGSYEntities();


            try
            {

                dbContext.Configuration.LazyLoadingEnabled = false;
                var resultStats = (from ms in dbContext.MASTER_STATE
                                   join ad in dbContext.ADMIN_DEPARTMENT on ms.MAST_STATE_CODE equals ad.MAST_STATE_CODE
                                   join md in dbContext.MASTER_DISTRICT on ad.MAST_DISTRICT_CODE equals md.MAST_DISTRICT_CODE
                                   where
                                   ad.MAST_STATE_CODE == md.MAST_STATE_CODE
                                   && ms.MAST_STATE_CODE == md.MAST_STATE_CODE
                                   && ms.MAST_STATE_ACTIVE == "Y"
                                   && md.MAST_DISTRICT_ACTIVE == "Y"
                                   && ad.ADMIN_ND_ACTIVE == "Y"
                                   && ad.MAST_ND_TYPE == "D"
                                   select new PIUMasterViewModel
                                   {
                                       MAST_STATE_CODE = ms.MAST_STATE_CODE,
                                       MAST_STATE_NAME = ms.MAST_STATE_NAME,
                                       MAST_DISTRICT_CODE = md.MAST_DISTRICT_CODE,
                                       MAST_DISTRICT_NAME = md.MAST_DISTRICT_NAME,
                                       ADMIN_ND_CODE = ad.ADMIN_ND_CODE,
                                       MAST_PARENT_ND_CODE = ad.MAST_PARENT_ND_CODE,
                                       ADMIN_ND_NAME = ad.ADMIN_ND_NAME

                                   }).OrderBy(x => x.MAST_STATE_CODE).ThenBy(x => x.MAST_DISTRICT_CODE).ThenBy(x => x.ADMIN_ND_CODE).ToList();


                return resultStats;



            }
            catch (Exception ex)
            {
                if (!Directory.Exists(ErrorLogDirectory))
                {
                    Directory.CreateDirectory(ErrorLogDirectory);
                }
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "EmargPushDetailsDAL.cs - PIUMasterDetailsDAL()");
                    sw.WriteLine("Exception : " + ex.ToString());
                    sw.WriteLine("Exception : " + ex.StackTrace);
                    if (ex.InnerException != null)
                        sw.WriteLine("Inner Exception : " + ex.InnerException.ToString());
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        #endregion



        #region  Get Emarg package Data  : omms.USP_ROAD_PIU_DETAILS_EMARG


        public dynamic GetRoadPiuEmargDetailsDAL()
        {
            dbContext = new PMGSYEntities();

            try
            {
                ((System.Data.Entity.Infrastructure.IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;
                var emargStats = dbContext.USP_ROAD_PIU_DETAILS_EMARG().ToList();
                return emargStats;

            }
            catch (Exception ex)
            {
                if (!Directory.Exists(ErrorLogDirectory))
                {
                    Directory.CreateDirectory(ErrorLogDirectory);
                }
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "EmargPushDetailsDAL.cs - GetRoadPiuEmargDetailsDAL()");
                    sw.WriteLine("Exception : " + ex.ToString());
                    sw.WriteLine("Exception : " + ex.StackTrace);
                    if (ex.InnerException != null)
                        sw.WriteLine("Inner Exception : " + ex.InnerException.ToString());
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion



        #region STATE MASTER
        public List<STATEMasterViewModel> STATEMasterDetailsDAL()
        {
            dbContext = new PMGSYEntities();
            try
            {

                dbContext.Configuration.LazyLoadingEnabled = false;
                var resultStats = (from ms in dbContext.MASTER_STATE
                                   where
                                    ms.MAST_STATE_ACTIVE == "Y"
                                   select new STATEMasterViewModel
                                   {
                                       STATE_CODE = ms.MAST_STATE_CODE,
                                       STATE_NAME = ms.MAST_STATE_NAME,
                                   }).OrderBy(x => x.STATE_CODE).ToList();


                return resultStats;
            }
            catch (Exception ex)
            {
                if (!Directory.Exists(ErrorLogDirectory))
                {
                    Directory.CreateDirectory(ErrorLogDirectory);
                }
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "EmargPushDetailsDAL.cs - STATEMasterDetailsDAL()");
                    sw.WriteLine("Exception : " + ex.ToString());
                    sw.WriteLine("Exception : " + ex.StackTrace);
                    if (ex.InnerException != null)
                        sw.WriteLine("Inner Exception : " + ex.InnerException.ToString());
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion

        #region DISTRICT MASTER
        public List<DISTRICTMasterViewModel> DISTRICTMasterDetailsDAL()
        {
            dbContext = new PMGSYEntities();

            try
            {

                dbContext.Configuration.LazyLoadingEnabled = false;
                var resultStats = (from md in dbContext.MASTER_DISTRICT
                                   where
                                   md.MAST_DISTRICT_ACTIVE == "Y"
                                   select new DISTRICTMasterViewModel
                                   {
                                       STATE_CODE = md.MAST_STATE_CODE,
                                       DISTRICT_CODE = md.MAST_DISTRICT_CODE,
                                       DISTRICT_NAME = md.MAST_DISTRICT_NAME,

                                   }).OrderBy(x => x.STATE_CODE).ThenBy(x => x.DISTRICT_CODE).ToList();


                return resultStats;
            }
            catch (Exception ex)
            {
                if (!Directory.Exists(ErrorLogDirectory))
                {
                    Directory.CreateDirectory(ErrorLogDirectory);
                }
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "EmargPushDetailsDAL.cs - DISTRICTMasterDetailsDAL()");
                    sw.WriteLine("Exception : " + ex.ToString());
                    sw.WriteLine("Exception : " + ex.StackTrace);
                    if (ex.InnerException != null)
                        sw.WriteLine("Inner Exception : " + ex.InnerException.ToString());
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion

        #region BLOCK MASTER
        public List<BLOCKMasterViewModel> BLOCKMasterDetailsDAL()
        {
            dbContext = new PMGSYEntities();

            try
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                var resultStats = (from mb in dbContext.MASTER_BLOCK
                                   where
                                   mb.MAST_BLOCK_ACTIVE == "Y"
                                   select new BLOCKMasterViewModel
                                   {
                                       DISTRICT_CODE = mb.MAST_DISTRICT_CODE,
                                       BLOCK_CODE = mb.MAST_BLOCK_CODE,
                                       BLOCK_NAME = mb.MAST_BLOCK_NAME,

                                   }).OrderBy(x => x.DISTRICT_CODE).ThenBy(x => x.BLOCK_CODE).ToList();


                return resultStats;
            }
            catch (Exception ex)
            {
                if (!Directory.Exists(ErrorLogDirectory))
                {
                    Directory.CreateDirectory(ErrorLogDirectory);
                }
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "EmargPushDetailsDAL.cs - BLOCKMasterDetailsDAL()");
                    sw.WriteLine("Exception : " + ex.ToString());
                    sw.WriteLine("Exception : " + ex.StackTrace);
                    if (ex.InnerException != null)
                        sw.WriteLine("Inner Exception : " + ex.InnerException.ToString());
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion



        #region EMARG Acknowledgement

        public List<EmargPaymentMasterViewModel> GetEmargAckDAL(string[] VoucherArray)
        {
            dbContext = new PMGSYEntities();
            var resultList = new List<EmargPaymentMasterViewModel>();
            //List<long> OmmasBillIdList = new List<long>();
            //long[] OmmasBillIdArray = new long[] { };
            try
            {
                ((System.Data.Entity.Infrastructure.IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                for (int i = 0; i < VoucherArray.Length; i++)
                {
                    int Bill_No = Convert.ToInt32(VoucherArray[i].Trim());
                    EMARG_PAYMENT_MASTER Master = dbContext.EMARG_PAYMENT_MASTER.Where(x => x.BILL_ID == Bill_No).FirstOrDefault();
                    if (Master != null)
                    {
                        //OmmasBillIdList.Add((long)Master.OMMAS_BILL_ID);
                        resultList.Add(new EmargPaymentMasterViewModel
                        {
                            EMARG_VOUCHER_NO = Master.BILL_ID,
                            OMMAS_BILL_ID = Master.OMMAS_BILL_ID,
                        });
                        Master.RECEIVED_ACK_DATE_TIME = DateTime.Now;
                        dbContext.Entry(Master).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                }
                //OmmasBillIdArray = OmmasBillIdList.ToArray();
                return resultList;

            }
            catch (Exception ex)
            {
                if (!Directory.Exists(ErrorLogDirectory))
                {
                    Directory.CreateDirectory(ErrorLogDirectory);
                }
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "EmargPushDetailsDAL.cs - GetEmargAckDAL()");
                    sw.WriteLine("Exception : " + ex.ToString());
                    sw.WriteLine("Exception : " + ex.StackTrace);
                    if (ex.InnerException != null)
                        sw.WriteLine("Inner Exception : " + ex.InnerException.ToString());
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        #endregion


        #region Get DRRP Road Details --Added By Hrishikesh
        public dynamic GetDRRPRoadPullDAL(string state_code)
        {
            //List<RoadDetails> 
            // List<RoadDetails> lstModel = new List<RoadDetails>();
            //    List<EMARG_COMPLETED_WORK_DETAILS_SERVICE> lstModel = new List<EMARG_COMPLETED_WORK_DETAILS_SERVICE>();

            //RoadDetails model = new RoadDetails();
            dbContext = new PMGSYEntities();


            try
            {
                ((System.Data.Entity.Infrastructure.IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                //  EMARG_COMPLETED_WORK_DETAILS_SERVICE model = new EMARG_COMPLETED_WORK_DETAILS_SERVICE();
                int new_state_code = Convert.ToInt32(state_code);
                List<Models.MasterDRRPRoadDetailsModel> DRRPRoadDetailsList = dbContext.MASTER_DRRP_ROAD.Where(x => x.STATE_CODE == new_state_code).Select(x => new Models.MasterDRRPRoadDetailsModel
                {
                    ER_ROAD_CODE = x.ER_ROAD_CODE,
                    STATE_NAME = x.STATE_NAME,
                    DISTRICT_NAME = x.DISTRICT_NAME,
                    BLOCK_NAME = x.BLOCK_NAME,
                    ER_ROAD_NUMBER = x.ER_ROAD_NUMBER,
                    ROAD_CAT_NAME = x.ROAD_CAT_NAME,
                    ROAD_NAME = x.ROAD_NAME,
                    ROAD_STR_CHAINAGE = x.ROAD_STR_CHAINAGE,
                    ROAD_END_CHAINAGE = x.ROAD_END_CHAINAGE,
                    ROAD_C_WIDTH = x.ROAD_C_WIDTH,
                    ROAD_F_WIDTH = x.ROAD_F_WIDTH,
                    ROAD_L_WIDTH = x.ROAD_L_WIDTH,
                    ROAD_TYPE = x.ROAD_TYPE == "A" ? "All Weather" : x.ROAD_TYPE == "F" ? "Fair Weather" : "",
                    SOIL_TYPE_NAME = x.SOIL_TYPE_NAME,
                    TERRAIN_TYPE = x.TERRAIN_TYPE,
                    TERRAIN_NAME = x.TERRAIN_NAME,
                    DRRP_SCHEME = x.DRRP_SCHEME == 1 ? "PMGSY-I" : x.DRRP_SCHEME == 2 ? "PMGSY-II" : x.DRRP_SCHEME == 3 ? "RCPLWEA" : "PMGSY-III"
                }).ToList<Models.MasterDRRPRoadDetailsModel>();

                return DRRPRoadDetailsList;

            }
            catch (Exception ex)
            {
                if (!Directory.Exists(ErrorLogDirectory))
                {
                    Directory.CreateDirectory(ErrorLogDirectory);
                }
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "EmargPushDetailsDAL.cs - GetDRRPRoadPullDAL()");
                    sw.WriteLine("Exception : " + ex.ToString());
                    sw.WriteLine("Exception : " + ex.StackTrace);
                    if (ex.InnerException != null)
                        sw.WriteLine("Inner Exception : " + ex.InnerException.ToString());
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        #endregion





        #region Get SANCTION_DRRP_ROADS EMARG-- Added By Shreyas-----
        public dynamic GetGetSanctionDRRPRoadsDAL(int finalStateCode)
        {
            dbContext = new PMGSYEntities();

            try
            {
                ((System.Data.Entity.Infrastructure.IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 360;
                dbContext.Configuration.LazyLoadingEnabled = false;

                

                var emargStats = dbContext.USP_SANCTION_DRRP_ROADS(finalStateCode).ToList();

                return emargStats;

            }
            catch (Exception ex)
            {
                if (!Directory.Exists(ErrorLogDirectory))
                {
                    Directory.CreateDirectory(ErrorLogDirectory);
                }
                using (StreamWriter sw = File.AppendText(Path.Combine(ErrorLogFilePath, "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "EmargPushDetailsDAL.cs - GetGetSanctionDRRPRoadsDAL()");
                    sw.WriteLine("Exception : " + ex.ToString());
                    sw.WriteLine("Exception : " + ex.StackTrace);
                    if (ex.InnerException != null)
                        sw.WriteLine("Inner Exception : " + ex.InnerException.ToString());
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }


        }
        #endregion
    }
}