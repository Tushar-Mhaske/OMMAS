using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Data.Entity;
using Newtonsoft.Json;
using System.ComponentModel;
using PMGSY.WebServices.eMarg.Model;
using PMGSY.Models;
using System.Data.Entity;
using PMGSY.Common;
using System.Data.Entity.Validation;
using System.Transactions;
using PMGSY.Extensions;
using PMGSY.Controllers;
using System.IO;
namespace PMGSY.WebServices.eMarg
{
    public class eMargDAL
    {
        PMGSYEntities dbcontext = null;

        public List<EMARG_ROAD_DETAILS> GetRoadDetails()
        {

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            //string URL = "https://emarg.gov.in/emargrest/rest/roaddetail/getdetail?User_ID=cdcweb&Password=gis@cdc098";
            //Uri uri = new Uri(URL);
            //WebClient wc = new WebClient();

            //string URL = "https://emarg.gov.in/emargrest/rest/roaddetail/getdetail/";
            string URL = "https://emarg.gov.in/emargrest/rest/datadetail/getdatacorrectionrecord";
            Uri uri = new Uri(URL);
            WebClient wc = new WebClient();

            //string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("cdcweb" + ":" + "gis@cdc098"));
            //wc.Headers[HttpRequestHeader.Authorization] = "Basic" + credentials;
            string token = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes("cdcweb" + ":" + "gis@cdc098"));
            // var token = (ASCIIEncoding.ASCII.GetBytes("cdcweb" + ":" + "gis@cdc098"));
            wc.Headers.Add("Authorization", "Basic " + token);
            // JSON.requ
            string results;



            PMGSY.WebServices.eMarg.Model.Roaddata abc = new Roaddata();


            //string json = JsonConvert.SerializeObject(results);
            results = wc.DownloadString(uri);

            //Console.Write(results);
            List<EMARG_ROAD_DETAILS> UserList = JsonConvert.DeserializeObject<List<EMARG_ROAD_DETAILS>>(results);
            return UserList;


        }


        public void saveData(List<RoadDetails> UserList)
        {
            //try
            //{

            //EMARG_ROAD_DETAILS sa = new EMARG_ROAD_DETAILS();
            foreach (var data in UserList)
            {

                using (dbcontext = new PMGSYEntities())
                {
                    using (TransactionScope tscaope = new TransactionScope())
                    {
                        EMARG_ROAD_DETAILS sa = new EMARG_ROAD_DETAILS();


                        try
                        {
                            sa.MAST_STATE_NAME = data.state;
                            sa.MAST_DISTRICT_NAME = data.district;
                            sa.PACKAGE_NO = data.packageNo;

                            //if (Equals(data.contractor, "false") || Equals(data.contractor, "NA") || Equals(data.contractor, "true"))
                            //{
                            //    sa.CONTRACTOR = null;
                            //}
                            //else
                            //{
                            sa.CONTRACTOR = data.contractor;
                            //}
                            //if (Equals(data.pan, "false") || Equals(data.pan, "NA") || Equals(data.pan, "true"))
                            //{
                            //    sa.PAN = null;
                            //}
                            //else
                            //{
                            sa.PAN = data.pan;
                            //}

                            //if (Equals(data.agreementNo, "false") || Equals(data.agreementNo, "NA") || Equals(data.agreementNo, "true"))
                            //{
                            //    sa.AGREEMENT_NO = null;
                            //}

                            //else
                            //{
                            sa.AGREEMENT_NO = data.agreementNo;
                            //}

                            //if (Equals(data.agreementDate, "false") || Equals(data.agreementDate, "NA") || Equals(data.agreementDate, "true"))
                            //{
                            //    sa.AGREEMENT_DATE = null;
                            //}

                            //else
                            //{
                            sa.AGREEMENT_DATE = data.agreementDate;
                            //}

                            sa.ROAD_CODE = data.roadCode;



                            //if (Equals(data.roadName, "false") || Equals(data.roadName, "NA") || Equals(data.roadName, "true"))
                            //{
                            //    sa.ROAD_NAME = null;
                            //}
                            //else
                            //{
                            sa.ROAD_NAME = data.roadName;

                            //}

                            //if (Equals(data.completionDate, "false") || Equals(data.completionDate, "NA") || Equals(data.completionDate, "true"))
                            //{
                            //    sa.COMPLETION_DATE = null;
                            //}

                            //else
                            //{
                            sa.COMPLETION_DATE = data.completionDate;
                            //}

                            //if (Equals(data.sanctionedLength, "false") || Equals(data.sanctionedLength, "NA") || Equals(data.sanctionedLength, "true"))
                            //{
                            //    sa.SANCTIONED_LENGTH = null;
                            //}

                            //else
                            //{
                            sa.SANCTIONED_LENGTH = data.sanctionedLength;
                            //}


                            //if (Equals(data.completedLength, "false") || Equals(data.completedLength, "NA") || Equals(data.completedLength, "true"))
                            //{
                            //    sa.COMPLETED_LENGTH = null;
                            //}

                            //else
                            //{
                            sa.COMPLETED_LENGTH = data.completedLength;
                            //}


                            //if (Equals(data.carriageWidth, "false") || Equals(data.carriageWidth, "NA") || Equals(data.carriageWidth, "true"))
                            //{
                            //    sa.CARRIAGE_WIDTH = null;
                            //}

                            //else
                            //{
                            sa.CARRIAGE_WIDTH = data.carriageWidth;
                            //}


                            //if (Equals(data.trafficDensity, "false") || Equals(data.trafficDensity, "NA") || Equals(data.trafficDensity, "true"))
                            //{
                            //    sa.TRAFFIC_DENSITY = null;
                            //}

                            //else
                            //{
                            sa.TRAFFIC_DENSITY = data.trafficDensity;
                            //}

                            //if (data.roadCode == 145728)
                            //{
                            //    sa.REMARKS = data.remarks.Trim();
                            //}

                            sa.REMARKS = data.remarks.Trim();




                            dbcontext.EMARG_ROAD_DETAILS.Add(sa);
                            dbcontext.SaveChanges();
                            tscaope.Complete();
                            //try
                            //{
                            //    dbcontext.SaveChanges();
                            //}

                            //catch(Exception ex) {
                            //    ErrorLog.EmargErrors(ex, sa);
                            //}
                        }
                        catch (DbEntityValidationException e)
                        {
                            //ErrorLog.EmargErrors( sa);
                            string errorLogPath = System.Configuration.ConfigurationSettings.AppSettings["EmargErrorPath"];
                            using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine(errorLogPath + "\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                            {
                                sw.WriteLine("---------------------------------------------------------------------------------------");
                                sw.WriteLine("Date :" + DateTime.Now.ToString());
                                sw.WriteLine("STATE : " + sa.MAST_STATE_NAME, sa.MAST_DISTRICT_NAME, sa.PACKAGE_NO, sa.ROAD_CODE);
                                sw.WriteLine("DISTRICT : " + sa.MAST_DISTRICT_NAME);
                                sw.WriteLine("PACKAGE NO : " + sa.PACKAGE_NO);
                                sw.WriteLine("ROAD CODE : " + sa.ROAD_CODE);
                                if (PMGSY.Extensions.PMGSYSession.Current != null)
                                {
                                    sw.WriteLine("UserName : " + PMGSY.Extensions.PMGSYSession.Current.UserName);
                                }
                                sw.WriteLine("---------------------------------------------------------------------------------------");
                                sw.Close();
                            }
                            continue;
                        }
                        catch (Exception ex)
                        {
                            //ErrorLog.EmargErrors( sa);
                            string errorLogPath = System.Configuration.ConfigurationSettings.AppSettings["EmargErrorPath"];
                            using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine(errorLogPath + "\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                            {
                                sw.WriteLine("---------------------------------------------------------------------------------------");
                                sw.WriteLine("Date :" + DateTime.Now.ToString());
                                sw.WriteLine("STATE : " + sa.MAST_STATE_NAME, sa.MAST_DISTRICT_NAME, sa.PACKAGE_NO, sa.ROAD_CODE);
                                sw.WriteLine("DISTRICT : " + sa.MAST_DISTRICT_NAME);
                                sw.WriteLine("PACKAGE NO : " + sa.PACKAGE_NO);
                                sw.WriteLine("ROAD CODE : " + sa.ROAD_CODE);
                                if (PMGSY.Extensions.PMGSYSession.Current != null)
                                {
                                    sw.WriteLine("UserName : " + PMGSY.Extensions.PMGSYSession.Current.UserName);
                                }
                                sw.WriteLine("---------------------------------------------------------------------------------------");
                                sw.Close();
                            }
                            continue;
                        }
                    }
                }

            }



            //  }
            //catch (Exception ex)
            //{
            //    ErrorLog.LogError(ex, "eMargDAL.saveData");
            //}
        }


        //public void SaveRoadDetails(int MAST_EMARG_ROAD_ID,
        // string MAST_STATE_NAME ,  string MAST_DISTRICT_NAME ,  string PACKAGE_NO ,string CONTRACTOR , string PAN ,string AGREEMENT_NO , Nullable<System.DateTime> AGREEMENT_DATE , int ROAD_CODE ,string ROAD_NAME , Nullable<System.DateTime> COMPLETION_DATE ,  double SANCTIONED_LENGTH , double COMPLETED_LENGTH , double CARRIAGE_WIDTH ,double TRAFFIC_DENSITY ,string REMARKS)
        //{

        // //   ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        // //   //string URL = "https://emarg.gov.in/emargrest/rest/roaddetail/getdetail?User_ID=cdcweb&Password=gis@cdc098";
        // //   //Uri uri = new Uri(URL);
        // //   //WebClient wc = new WebClient();

        // //   //string URL = "https://emarg.gov.in/emargrest/rest/roaddetail/getdetail/";
        // //   string URL = "https://emarg.gov.in/emargrest/rest/datadetail/getdatacorrectionrecord";
        // //   Uri uri = new Uri(URL);
        // //   WebClient wc = new WebClient();

        // //   //string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("cdcweb" + ":" + "gis@cdc098"));
        // //   //wc.Headers[HttpRequestHeader.Authorization] = "Basic" + credentials;
        // //   string token = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes("cdcweb" + ":" + "gis@cdc098"));
        // //   // var token = (ASCIIEncoding.ASCII.GetBytes("cdcweb" + ":" + "gis@cdc098"));
        // //   wc.Headers.Add("Authorization", "Basic " + token);
        // //   // JSON.requ
        // //   string results;



        // //   PMGSY.WebServices.eMarg.Model.Roaddata abc = new Roaddata();


        // //   //string json = JsonConvert.SerializeObject(results);
        // //   results = wc.DownloadString(uri);

        // //   //Console.Write(results);
        // ////   List<Roaddata> UserList = JsonConvert.DeserializeObject<List<Roaddata>>(results);

        //    dbcontext = new PMGSYEntities();

        //    EMARG_ROAD_DETAILS ABC = new EMARG_ROAD_DETAILS();
        //    ABC.AGREEMENT_NO="ABCD_EFGH_IJKL";

        //    ABC.AGREEMENT_DATE = DateTime.ParseExact("05-29-2015 05:50", "MM-dd-yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture); ;//05/29/2015 05:50';
        //    ABC.CARRIAGE_WIDTH = 3.2;

        //    ABC.COMPLETED_LENGTH = 4;

        //    ABC.COMPLETION_DATE = DateTime.ParseExact("05-29-2015 05:50", "MM-dd-yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture); ;//05/29/2015 05:50';
        //    ABC.CONTRACTOR="SACHIN";
        //    ABC.MAST_DISTRICT_NAME = "aroli";
        //    ABC.MAST_STATE_NAME = "UTTAR PRADESH";
        //    ABC.PACKAGE_NO = "AP12DND";
        //    ABC.PAN = "SADDDDFZS";
        //    ABC.REMARKS = "TESTING DATA";
        //    ABC.ROAD_CODE = 4343;
        //    ABC.ROAD_NAME = "DHOLIA TO SATIYA";
        //    ABC.SANCTIONED_LENGTH = 22;
        //    ABC.TRAFFIC_DENSITY = 32;
        //    dbcontext.EMARG_ROAD_DETAILS.Add(ABC);



        //}

        public int getemargpaymentdetailsDAL(List<EmargPaymentModel> PaymentList)
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
                        using (dbcontext = new PMGSYEntities())
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

                        string errorLogPath = System.Configuration.ConfigurationSettings.AppSettings["EmargErrorPath"];
                        if (!Directory.Exists(errorLogPath))
                        {
                            Directory.CreateDirectory(errorLogPath);
                        }
                        using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine(errorLogPath + "\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                        {
                            sw.WriteLine("---------------------------------------------------------------------------------------");
                            sw.WriteLine("Date :" + DateTime.Now.ToString());
                            if (PMGSY.Extensions.PMGSYSession.Current != null)
                            {
                                // sw.WriteLine("UserName : " + PMGSY.Extensions.PMGSYSession.Current.UserName);
                                sw.WriteLine("IPADD : " + HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]);
                            }
                            sw.WriteLine("---------------------------------------------------------------------------------------");
                            sw.Close();
                            return 0;
                        }
                    }
                    finally
                    {
                        tscaope.Dispose();
                    }
                }
        }
    }
}