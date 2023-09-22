using PMGSY.Areas.EmargDataPull.Models;
using PMGSY.Common;
using PMGSY.Models;
using PMGSY.WebServices.eMarg.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;

namespace PMGSY.Areas.EmargDataPull.DAL
{
    public class EmargDAL
    {
        #region  First Level Ack
        public void SaveAcknowledmentData(List<EmargAck> list)
        {// First Level Service Details
            PMGSYEntities dbcontext = new PMGSYEntities();
            dbcontext.Configuration.AutoDetectChangesEnabled = false;

            try
            {
                foreach (var data in list)
                {
                    using (TransactionScope tscaope = new TransactionScope())
                    {
                        EMARG_ROAD_DETAILS sa = new EMARG_ROAD_DETAILS();

                        try
                        {

                            Int32? emargID = dbcontext.EMARG_COMPLETED_WORK_DETAILS_SERVICE.Where(m => m.EID == data.EID).Select(m => m.EMARG_ID).FirstOrDefault();
                            Int16 RejectionID = Convert.ToInt16(data.rejectCode);

                            Int32? RejectionCode = dbcontext.EMARG_REJECTION_MASTER.Where(m => m.REJECTION_ID == RejectionID).Select(m => m.REJECTION_ID).FirstOrDefault();

                            if (dbcontext.EMARG_ROAD_DETAILS.Where(m => m.EMARG_ID == emargID && m.EMARG_STATUS1 == null && sa.IS_DEACTIVATED == null).Any())
                            {
                                #region Flag Updations
                                sa = dbcontext.EMARG_ROAD_DETAILS.Where(m => m.EMARG_ID == emargID).FirstOrDefault();

                                if (data.successStatus.Equals("True") || data.successStatus.Equals("true"))
                                {// Accepted
                                    sa.EMARG_STATUS1 = "Y"; // Is Success Status is True then set it Y else N
                                }
                                else if (data.successStatus.Equals("False") || data.successStatus.Equals("false"))
                                {// Rejected 
                                    sa.EMARG_STATUS1 = "N"; // Is Success Status is True then set it Y else N
                                }

                                if (data.rejectCode.Equals("0"))
                                {
                                    // If 0 then keep EMARG_STATUS1_REJECTION_CODE as null.
                                }
                                else
                                {
                                    sa.EMARG_STATUS1_REJECTION_CODE = RejectionCode;
                                }
                                if (data.successStatus.Equals("False") || data.successStatus.Equals("false"))
                                {//Rejected
                                    sa.IS_DEACTIVATED = "Y";
                                }
                                sa.EMARG_STATUS1_ACK_DATE = Convert.ToDateTime(data.acknowledgementDate);
                                dbcontext.Entry(sa).State =  System.Data.Entity.EntityState.Modified;
                                dbcontext.SaveChanges();
                                #endregion

                                #region Reinsert above road details if First Level is Rejected. Dont check second level.

                                if (data.successStatus.Equals("False") || data.successStatus.Equals("false"))
                                {
                                    EMARG_ROAD_DETAILS model = dbcontext.EMARG_ROAD_DETAILS.Where(m => m.EMARG_ID == emargID).FirstOrDefault();

                                    EMARG_ROAD_DETAILS sa2 = new EMARG_ROAD_DETAILS();
                                    sa2.EMARG_ID = (dbcontext.EMARG_ROAD_DETAILS.Any() ? dbcontext.EMARG_ROAD_DETAILS.Max(m => m.EMARG_ID) + 1 : 1);
                                    sa2.MAST_STATE_NAME = model.MAST_STATE_NAME;
                                    sa2.MAST_DISTRICT_NAME = model.MAST_DISTRICT_NAME;
                                    sa2.PACKAGE_NO = model.PACKAGE_NO;
                                    sa2.CONTRACTOR = model.CONTRACTOR;
                                    sa2.PAN = model.PAN;
                                    sa2.AGREEMENT_NO = model.AGREEMENT_NO;
                                    sa2.AGREEMENT_DATE = model.AGREEMENT_DATE;
                                    sa2.ROAD_CODE = model.ROAD_CODE;
                                    sa2.ROAD_NAME = model.ROAD_NAME;
                                    sa2.COMPLETION_DATE = model.COMPLETION_DATE;
                                    sa2.SANCTIONED_LENGTH = model.SANCTIONED_LENGTH;
                                    sa2.COMPLETED_LENGTH = model.COMPLETED_LENGTH;
                                    sa2.CARRIAGE_WIDTH = model.CARRIAGE_WIDTH;
                                    sa2.TRAFFIC_DENSITY = model.TRAFFIC_DENSITY;
                                    sa2.REMARKS = null;
                                    sa2.EMARG_STATUS = "N";
                                    sa2.DATA_RECEIPT_DATE = System.DateTime.Now;
                                    sa2.OMMAS_REPUSHING_STATUS = null;
                                    sa2.DATE_OF_REPUSHING = null;
                                    sa2.REJECTION_REASON = "2";
                                    sa2.EMARG_STATUS1 = null;
                                    sa2.EMARG_STATUS1_REJECTION_CODE = null;
                                    sa2.EMARG_STATUS1_ACK_DATE = null;
                                    sa2.EMARG_STATUS2 = null;
                                    sa2.EMARG_STATUS2_REJECTION_CODE = null;
                                    sa2.EMARG_STATUS2_ACK_DATE = null;
                                    sa2.IS_DEACTIVATED = null;
                                    sa2.IS_REINSERTED = "Y";
                                    dbcontext.EMARG_ROAD_DETAILS.Add(sa2);
                                    dbcontext.SaveChanges();


                                    // Mark this Emarg Id Null in table  omms.MANE_EMARG_CONTRACT
                                    MANE_EMARG_CONTRACT contract = new MANE_EMARG_CONTRACT();
                                    contract = dbcontext.MANE_EMARG_CONTRACT.Where(m => m.EMARG_ID == emargID).FirstOrDefault();
                                    if (contract != null)
                                    {
                                        contract.IS_DEACTIVATED = "Y";
                                        dbcontext.Entry(contract).State = System.Data.Entity.EntityState.Modified;
                                        dbcontext.SaveChanges();
                                    }

                                }
                                #endregion

                            }

                            tscaope.Complete();
                        }
                        catch (Exception ex)
                        {

                            string errorLogPath = System.Configuration.ConfigurationSettings.AppSettings["EmargErrorPath"];
                            using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine(errorLogPath + "\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                            {
                                sw.WriteLine("---------------------------------------------------------------------------------------");
                                sw.WriteLine("Method Name : EmargDAL().SaveAcknowledmentData()");

                                if (PMGSY.Extensions.PMGSYSession.Current != null)
                                {
                                    sw.WriteLine("UserName : " + PMGSY.Extensions.PMGSYSession.Current.UserName);
                                }
                                sw.WriteLine("---------------------------------------------------------------------------------------");
                                sw.Close();
                            }
                            continue;
                        }
                        finally
                        {
                            // dbcontext.Configuration.AutoDetectChangesEnabled = true;
                            //  dbcontext.Dispose();
                        }
                    }



                }
            }
            catch (Exception ex)
            {
                string errorLogPath = System.Configuration.ConfigurationSettings.AppSettings["EmargErrorPath"];
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine(errorLogPath + "\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.WriteLine("Outer Catch :: Method Name : EmargDAL().SaveAcknowledmentData()");

                    if (PMGSY.Extensions.PMGSYSession.Current != null)
                    {
                        sw.WriteLine("UserName : " + PMGSY.Extensions.PMGSYSession.Current.UserName);
                    }
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }
            }
            finally
            {
                dbcontext.Configuration.AutoDetectChangesEnabled = true;
                dbcontext.Dispose();
            }


        }
        #endregion

        #region Second Level Data Service
        public void SaveData(List<RoadDetails> UserList)
        {// Second Level Service Details

            foreach (var data in UserList)
            {


                using (PMGSYEntities dbcontext = new PMGSYEntities())
                {
                    using (TransactionScope tscaope = new TransactionScope())
                    {
                        EMARG_ROAD_DETAILS sa = new EMARG_ROAD_DETAILS();

                        try
                        {

                            if (dbcontext.EMARG_ROAD_DETAILS.Where(m => m.ROAD_CODE == data.roadCode).Any())
                            {

                                //#region Updation of REJECTION_REASON flag to 2
                                //if (dbcontext.EMARG_ROAD_DETAILS.Where(m => m.ROAD_CODE == data.roadCode && m.REJECTION_REASON == "1").Any())
                                //{
                                //    Int32 emargID = dbcontext.EMARG_ROAD_DETAILS.Where(m => m.ROAD_CODE == data.roadCode && m.REJECTION_REASON == "1").Select(m=>m.EMARG_ID).FirstOrDefault();
                                //    sa = dbcontext.EMARG_ROAD_DETAILS.Where(m => m.EMARG_ID == emargID).FirstOrDefault();
                                //    sa.MAST_STATE_NAME = (data.state == null ? "NA" : data.state);
                                //    sa.MAST_DISTRICT_NAME = (data.district == null ? "NA" : data.district);
                                //    sa.PACKAGE_NO = (data.packageNo == null ? "NA" : data.packageNo.Trim());
                                //    sa.CONTRACTOR = data.contractor;
                                //    sa.PAN = data.pan;
                                //    sa.AGREEMENT_NO = data.agreementNo;
                                //    sa.AGREEMENT_DATE = data.agreementDate;
                                //    sa.ROAD_CODE = data.roadCode;
                                //    sa.ROAD_NAME = data.roadName;
                                //    sa.COMPLETION_DATE = data.completionDate;
                                //    sa.SANCTIONED_LENGTH = data.sanctionedLength;
                                //    sa.COMPLETED_LENGTH = data.completedLength;
                                //    sa.CARRIAGE_WIDTH = data.carriageWidth;
                                //    sa.TRAFFIC_DENSITY = data.trafficDensity;
                                //    sa.REJECTION_REASON = "2";
                                //    sa.DATA_RECEIPT_DATE = System.DateTime.Now;
                                //    dbcontext.Entry(sa).State = System.Data.Entity.EntityState.Modified;
                                //    dbcontext.SaveChanges();

                                //}
                                //#endregion

                                //#region Ack Logic : Flag Updation (EMARG_STATUS2 = "N",IS_DEACTIVATED = "Y" ) and Reinserting Road Details
                                //Int32? EmargID = dbcontext.EMARG_COMPLETED_WORK_DETAILS_SERVICE.Where(m => m.EID == data.EID).Select(m => m.EMARG_ID).FirstOrDefault();

                                //if (EmargID != null)
                                //{
                                //    if (dbcontext.EMARG_ROAD_DETAILS.Where(m => m.ROAD_CODE == data.roadCode && m.EMARG_ID == EmargID && (m.IS_DEACTIVATED==null || m.IS_DEACTIVATED!="Y")).Any())
                                //    {
                                //        // Update Status against existing Emarg ID
                                //        EMARG_ROAD_DETAILS sa1 = new EMARG_ROAD_DETAILS();
                                //        sa1 = dbcontext.EMARG_ROAD_DETAILS.Where(m => m.EMARG_ID == EmargID).FirstOrDefault();
                                //        sa1.EMARG_STATUS2 = "N";
                                //        sa1.EMARG_STATUS2_ACK_DATE = System.DateTime.Now;
                                //        sa1.EMARG_STATUS2_REJECTION_CODE = null;
                                //        sa1.IS_DEACTIVATED = "Y";
                                //        sa1.IS_REINSERTED = null;
                                //        dbcontext.Entry(sa1).State = System.Data.Entity.EntityState.Modified;
                                //        dbcontext.SaveChanges();

                                //        // Mark this Emarg Id Null in table  omms.MANE_EMARG_CONTRACT
                                //        MANE_EMARG_CONTRACT contract = new MANE_EMARG_CONTRACT();
                                //        contract = dbcontext.MANE_EMARG_CONTRACT.Where(m => m.EMARG_ID == EmargID).FirstOrDefault();
                                //        if (contract != null)
                                //        {
                                //            contract.IS_DEACTIVATED = "Y";
                                //            dbcontext.Entry(contract).State = System.Data.Entity.EntityState.Modified;
                                //            dbcontext.SaveChanges();
                                //        }



                                //        // Now here second level service inserts above rejcted record in the table with new Emarg ID.
                                //        EMARG_ROAD_DETAILS sa2 = new EMARG_ROAD_DETAILS();
                                //        sa2.EMARG_ID = (dbcontext.EMARG_ROAD_DETAILS.Any() ? dbcontext.EMARG_ROAD_DETAILS.Max(m => m.EMARG_ID) + 1 : 1);
                                //        sa2.MAST_STATE_NAME = (data.state == null ? "NA" : data.state);
                                //        sa2.MAST_DISTRICT_NAME = (data.district == null ? "NA" : data.district);
                                //        sa2.PACKAGE_NO = (data.packageNo == null ? "NA" : data.packageNo.Trim());
                                //        sa2.CONTRACTOR = data.contractor;
                                //        sa2.PAN = data.pan;
                                //        sa2.AGREEMENT_NO = data.agreementNo;
                                //        sa2.AGREEMENT_DATE = data.agreementDate;
                                //        sa2.ROAD_CODE = data.roadCode;
                                //        sa2.ROAD_NAME = data.roadName;
                                //        sa2.COMPLETION_DATE = data.completionDate;
                                //        sa2.SANCTIONED_LENGTH = data.sanctionedLength;
                                //        sa2.COMPLETED_LENGTH = data.completedLength;
                                //        sa2.CARRIAGE_WIDTH = data.carriageWidth;
                                //        sa2.TRAFFIC_DENSITY = data.trafficDensity;
                                //        sa2.REMARKS = null;

                                //        sa2.EMARG_STATUS = "N";
                                //        sa2.DATA_RECEIPT_DATE = System.DateTime.Now;
                                //        sa2.OMMAS_REPUSHING_STATUS = null;
                                //        sa2.DATE_OF_REPUSHING = null;
                                //        sa2.REJECTION_REASON = "2";

                                //        sa2.EMARG_STATUS1 = null;
                                //        sa2.EMARG_STATUS1_REJECTION_CODE = null;
                                //        sa2.EMARG_STATUS1_ACK_DATE = null;

                                //        sa2.EMARG_STATUS2 = null;
                                //        sa2.EMARG_STATUS2_REJECTION_CODE = null;
                                //        sa2.EMARG_STATUS2_ACK_DATE = null;

                                //        sa2.IS_DEACTIVATED = null;
                                //        sa2.IS_REINSERTED = "Y";

                                //        dbcontext.EMARG_ROAD_DETAILS.Add(sa2);
                                //        dbcontext.SaveChanges();

                                //    }
                                //}
                                //#endregion

                                //tscaope.Complete();

                            }
                            else
                            {
                                #region New Road Details Saving
                                sa.EMARG_ID = (dbcontext.EMARG_ROAD_DETAILS.Any() ? dbcontext.EMARG_ROAD_DETAILS.Max(m => m.EMARG_ID) + 1 : 1);
                                sa.MAST_STATE_NAME = (data.state == null ? "NA" : data.state);
                                sa.MAST_DISTRICT_NAME = (data.district == null ? "NA" : data.district);
                                sa.PACKAGE_NO = (data.packageNo == null ? "NA" : data.packageNo.Trim());
                                sa.CONTRACTOR = data.contractor;
                                sa.PAN = data.pan;
                                sa.AGREEMENT_NO = data.agreementNo;
                                sa.AGREEMENT_DATE = data.agreementDate;
                                sa.ROAD_CODE = data.roadCode;
                                sa.ROAD_NAME = data.roadName;
                                sa.COMPLETION_DATE = data.completionDate;
                                sa.SANCTIONED_LENGTH = data.sanctionedLength;
                                sa.COMPLETED_LENGTH = data.completedLength;
                                sa.CARRIAGE_WIDTH = data.carriageWidth;
                                sa.TRAFFIC_DENSITY = data.trafficDensity;
                                sa.REMARKS = null; // Above is commented because some Remarks fields are of junk data 
                                sa.EMARG_STATUS = "N";  /// This Status will be Y only if PIU will finalize the package Details
                                sa.DATA_RECEIPT_DATE = System.DateTime.Now;
                                sa.OMMAS_REPUSHING_STATUS = null;
                                sa.DATE_OF_REPUSHING = null;
                                sa.REJECTION_REASON = "2"; // PIU Rejected

                                sa.EMARG_STATUS1 = null;
                                sa.EMARG_STATUS1_REJECTION_CODE = null;
                                sa.EMARG_STATUS1_ACK_DATE = null;

                                sa.EMARG_STATUS2 = null;
                                sa.EMARG_STATUS2_REJECTION_CODE = null;
                                sa.EMARG_STATUS2_ACK_DATE = null;

                                sa.IS_DEACTIVATED = null;
                                sa.IS_REINSERTED = null;

                                dbcontext.EMARG_ROAD_DETAILS.Add(sa);
                                dbcontext.SaveChanges();
                                tscaope.Complete();
                                #endregion
                            }

                        }
                        catch (Exception ex)
                        {

                            string errorLogPath = System.Configuration.ConfigurationSettings.AppSettings["EmargErrorPath"];
                            using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine(errorLogPath + "\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                            {
                                sw.WriteLine("---------------------------------------------------------------------------------------");
                                sw.WriteLine("Date :" + DateTime.Now.ToString());
                                sw.WriteLine("STATE : " + sa.MAST_STATE_NAME, sa.MAST_DISTRICT_NAME, sa.PACKAGE_NO, sa.ROAD_CODE);
                                sw.WriteLine("DISTRICT : " + sa.MAST_DISTRICT_NAME);
                                sw.WriteLine("PACKAGE NO : " + sa.PACKAGE_NO);
                                sw.WriteLine("ROAD CODE : " + sa.ROAD_CODE);
                                sw.WriteLine("REMARKS  : " + sa.REMARKS);
                                if (PMGSY.Extensions.PMGSYSession.Current != null)
                                {
                                    sw.WriteLine("UserName : " + PMGSY.Extensions.PMGSYSession.Current.UserName);
                                }
                                sw.WriteLine("---------------------------------------------------------------------------------------");
                                sw.Close();
                            }
                            continue;
                        }
                        finally
                        {
                            dbcontext.Dispose();
                        }
                    }
                }


            }


        }
        #endregion

        #region Get Data in a table from First Level Serivice (Ack)
        public int GetDataInAtableFromAcknowledmentDetails(List<EmargAck> list)
        {
            PMGSYEntities dbcontext = new PMGSYEntities();
            try
            {
                int rowsaffected = 0;
                dbcontext.Configuration.AutoDetectChangesEnabled = false;
                int Ack_Id = dbcontext.EMARG_FIRST_LEVEL_ACK_SERVICE.Any() ? dbcontext.EMARG_FIRST_LEVEL_ACK_SERVICE.Max(m => m.EMARG_ACK_ID) + 1 : 1;
                foreach (var data in list)
                {
                    using (TransactionScope tscaope = new TransactionScope())
                    {                       
                        EMARG_FIRST_LEVEL_ACK_SERVICE sa = new EMARG_FIRST_LEVEL_ACK_SERVICE();
                        if (dbcontext.EMARG_FIRST_LEVEL_ACK_SERVICE.Where(m => m.EID == data.EID).Any())
                        {

                        }
                        else
                        {
                            sa.EMARG_ACK_ID = Ack_Id;
                            sa.EID = data.EID;
                            sa.PACKAGE_NO = data.packageNo;
                            sa.ACK_DATE = data.acknowledgementDate;
                            sa.SUCCESS_STATUS = data.successStatus;
                            sa.REJECT_STATUS = data.rejectStatus;
                            sa.REJECT_CODE = data.rejectCode;
                            sa.REJECT_REASON = data.rejectReason;
                            dbcontext.EMARG_FIRST_LEVEL_ACK_SERVICE.Add(sa);
                            Ack_Id++;
                            //    dbcontext.SaveChanges();
                           // rowsaffected++;
                            tscaope.Complete();         
                        }

                    }
                }

                using (TransactionScope scope = new TransactionScope())
                {
                    rowsaffected = dbcontext.SaveChanges();
                    dbcontext.Configuration.AutoDetectChangesEnabled = true;
                    scope.Complete();

                    using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                    {
                        sw.WriteLine("-Step 4-");
                        sw.WriteLine("Affected rows :" + rowsaffected);
                        sw.Close();

                    }
                }

                return rowsaffected;
            }
            catch (Exception ex)
            {
                dbcontext.Configuration.AutoDetectChangesEnabled = true;
                ErrorLog.LogError(ex, "EmargDAL().GetDataInAtableFromAcknowledmentDetails()");
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Catch Method= : " + ex.Message.ToString());

                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }
                throw;
            }
            finally
            {
                dbcontext.Configuration.AutoDetectChangesEnabled = true;
            }

        }
        #endregion

        #region Get Data in a table from Second Level Service
        public int SaveDataFromSecondLevelService(List<RoadDetails> UserList)
        {
            // Save Second Level Service Details in a single table
            int rowsAffected = 0;
            PMGSYEntities dbcontext = new PMGSYEntities();
            try
            {
                foreach (var data in UserList)
                {
                        using (TransactionScope tscaope = new TransactionScope())
                        {
                            EMARG_ROAD_CORRECTION_SERVICEII sa = new EMARG_ROAD_CORRECTION_SERVICEII();
                                if (dbcontext.EMARG_ROAD_CORRECTION_SERVICEII.Where(m => m.EID == data.EID).Any())
                                {

                                }
                                else
                                {
                                    #region New Road Details Saving
                                    sa.EID = data.EID; //(dbcontext.EMARG_ROAD_CORRECTION_SERVICEII.Any() ? dbcontext.EMARG_ROAD_CORRECTION_SERVICEII.Max(m => m.EID) + 1 : 1);
                                    sa.MAST_STATE_NAME = (data.state == null ? "NA" : data.state);
                                    sa.MAST_DISTRICT_NAME = (data.district == null ? "NA" : data.district);
                                    sa.PACKAGE_NO = (data.packageNo == null ? "NA" : data.packageNo.Trim());
                                    sa.CONTRACTOR = data.contractor;
                                    sa.PAN = data.pan;
                                    sa.AGREEMENT_NO = data.agreementNo;
                                    sa.AGREEMENT_DATE = data.agreementDate;
                                    sa.ROAD_CODE = data.roadCode;
                                    sa.ROAD_NAME = data.roadName;
                                    sa.COMPLETION_DATE = data.completionDate;
                                    sa.SANCTIONED_LENGTH = data.sanctionedLength;
                                    sa.COMPLETED_LENGTH = data.completedLength;
                                    sa.CARRIAGE_WIDTH = data.carriageWidth;
                                    sa.TRAFFIC_DENSITY = data.trafficDensity;
                                    sa.REMARKS = null;
                                    #region
                            //sa.EMARG_STATUS = "N";  
                            //sa.DATA_RECEIPT_DATE = System.DateTime.Now;
                            //sa.OMMAS_REPUSHING_STATUS = null;
                            //sa.DATE_OF_REPUSHING = null;
                            //sa.REJECTION_REASON = "2"; 

                            //sa.EMARG_STATUS1 = null;
                            //sa.EMARG_STATUS1_REJECTION_CODE = null;
                            //sa.EMARG_STATUS1_ACK_DATE = null;

                            //sa.EMARG_STATUS2 = null;
                            //sa.EMARG_STATUS2_REJECTION_CODE = null;
                            //sa.EMARG_STATUS2_ACK_DATE = null;

                            //sa.IS_DEACTIVATED = null;
                            //sa.IS_REINSERTED = null;
                            #endregion
                                    dbcontext.EMARG_ROAD_CORRECTION_SERVICEII.Add(sa);
                                     //rowsAffected++;
                                    //dbcontext.SaveChanges();
                                     tscaope.Complete();
                                    #endregion
                                }
                        }                   
                }
                using (TransactionScope scope = new TransactionScope())
                {
                    rowsAffected = dbcontext.SaveChanges();
                    dbcontext.Configuration.AutoDetectChangesEnabled = true;
                    scope.Complete();

                    using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                    {
                        sw.WriteLine("-Step 4-");
                        sw.WriteLine("Affected rows :" + rowsAffected);
                        sw.Close();

                    }
                }

                return rowsAffected;
            }
            catch(Exception ex)
            {
                string errorLogPath = System.Configuration.ConfigurationSettings.AppSettings["EmargErrorPath"];
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine(errorLogPath + "\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("-------------SaveDataFromSecondLevelService--------------------------------------------------------------------------");
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Exception : " + ex.Message.ToString());
                    if (PMGSY.Extensions.PMGSYSession.Current != null)
                    {
                        sw.WriteLine("UserName : " + PMGSY.Extensions.PMGSYSession.Current.UserName);
                    }
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }
                throw;
            }
            finally
            {
                dbcontext.Dispose();
            }
            


        }
        #endregion

        #region Emarg Payment


        public int EmargPaymentDAL(List<EmargPaymentPullModel> PaymentDetailsList)
        {

            List<EMARG_PAYMENT_MASTER> paymasterlist = new List<EMARG_PAYMENT_MASTER>();
            List<EMARG_PAYMENT_DETAILS> paydetailslist = new List<EMARG_PAYMENT_DETAILS>();

            List<EMARG_PAYMENT_RENEW_MASTER> payRenmasterlist = new List<EMARG_PAYMENT_RENEW_MASTER>();
            List<EMARG_PAYMENT_RENEW_DETAILS> payRendetailslist = new List<EMARG_PAYMENT_RENEW_DETAILS>();

            // List<ACC_BILL_MASTER> billmasterlist = new List<ACC_BILL_MASTER>();
            CommonFunctions common = new CommonFunctions();
            int rowsaffected = 0;
            int max_txn_no = 0;
            int count = 0;
            PMGSYEntities dbcontext = new PMGSYEntities();

            int i = 0;
            //using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\eMARGErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
            //{

            //    sw.WriteLine("Step 1-");
            //    sw.WriteLine("List Count :" + PaymentDetailsList.Count);
            //    sw.Close();

            //}

            //using (TransactionScope tscaope = new TransactionScope(TransactionScopeOption.Required,
            //                   new System.TimeSpan(0, 15, 0)))
            //{
            try
            {

                //var max_bill_ID = dbcontext.ACC_BILL_MASTER.Max(ac => (ac.BILL_ID));
                //// var bill_ID_emarg = dbcontext.EMARG_PAYMENT_MASTER.Any() ? dbcontext.EMARG_PAYMENT_MASTER.Max(ad => ad.BILL_ID):0;
                dbcontext.Configuration.AutoDetectChangesEnabled = false;
                foreach (var item in PaymentDetailsList)
                {

                    //var mast_con_ID = dbcontext.MANE_IMS_CONTRACT.Where(an => an.MANE_AGREEMENT_NUMBER == item.AGREEMENT_CODE).Min(ms => ms.MAST_CON_ID);
                    //var MonthYearVal = item.BILL_MONTH + (item.BILL_YEAR * 12);

                    if (item.VOUCHER_TYPE=="N")
                    {

                    

                        if (!dbcontext.EMARG_PAYMENT_MASTER.Any(b => b.BILL_ID == item.BILL_ID) && !paymasterlist.Any(s => s.BILL_ID == item.BILL_ID))
                        {
                            //// bill_ID_emarg++;
                            var payMaster = new EMARG_PAYMENT_MASTER
                            {

                                BILL_ID = item.BILL_ID,//bill_ID_emarg,
                                BILL_ID_PREVIOUS = item.BILL_ID_PREVIOUS,
                                //VOUCHER_NO = item.VOUCHER_NO,
                                VOUCHER_NO = item.EMARG_VOUCHER_NO,
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
                                AMOUNT = item.AMOUNT,
                                //RECEIVED_DATE_TIME = item.RECEIVED_DATE_TIME,
                                RECEIVED_DATE_TIME = DateTime.Now,
                                EMARG_DATE_TIME = item.EMARG_DATE_TIME,
                                EMARG_VOUCHER_NO = item.EMARG_VOUCHER_NO,
                                OMMAS_PACKAGE_NO = item.OMMAS_PACKAGE_NO,
                                EMARG_BILL_ID = item.BILL_ID,
                                VOUCHER_TYPE = item.VOUCHER_TYPE
                            };
                            i++;
                            paymasterlist.Add(payMaster);
                            dbcontext.EMARG_PAYMENT_MASTER.Add(payMaster);

                            //using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                            //{
                            //    sw.WriteLine("Date :" + DateTime.Now.ToString());
                            //    sw.WriteLine("After   dbcontext.EMARG_PAYMENT_MASTER.Add(payMaster)");

                            //    sw.WriteLine("---------------------------------------------------------------------------------------");
                            //    sw.Close();
                            //}


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

                        //using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\eMARGErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                        //{
                        //    sw.WriteLine("-Step 2-");
                        //    sw.WriteLine("Count :" + i.ToString() + " , Bill ID:" + item.BILL_ID + " ,Bill Date:" + item.BILL_DATE);
                        //    sw.Close();

                        //}


                        if (!dbcontext.EMARG_PAYMENT_DETAILS.Any(b => b.BILL_ID == item.BILL_ID && b.TXN_NO == item.TXN_NO) && !paydetailslist.Any(s => s.BILL_ID == item.BILL_ID && s.TXN_NO == item.TXN_NO))
                        {
                            var payDetails = new EMARG_PAYMENT_DETAILS
                            {
                                BILL_ID = item.BILL_ID,//bill_ID_emarg,
                                TXN_NO = item.TXN_NO,
                                HEAD_CODE_DEDUCTIONS = item.HEAD_CODE_DEDUCTIONS,
                                AMOUNT_DEDUCTIONS = item.AMOUNT_DEDUCTIONS,
                                EMARG_VOUCHER_NO = item.EMARG_VOUCHER_NO,
                                OMMAS_PACKAGE_NO = item.OMMAS_PACKAGE_NO,
                                EMARG_PACKAGE_NO = (item.EMARG_PACKAGE_NO == null ? "NA" : item.EMARG_PACKAGE_NO),
                                EMARG_DATE_TIME = item.EMARG_DATE_TIME

                            };



                            paydetailslist.Add(payDetails);
                            count++;
                            dbcontext.EMARG_PAYMENT_DETAILS.Add(payDetails);


                            //using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\eMARGErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                            //{
                            //    sw.WriteLine("-Step 3-");
                            //    sw.WriteLine("Count :" + i.ToString() + " , Bill ID:" + item.BILL_ID + " ,TXN No:" + item.TXN_NO);
                            //    sw.Close();

                            //}


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
                    else if(item.VOUCHER_TYPE == "R")
                    {
                        if (!dbcontext.EMARG_PAYMENT_RENEW_MASTER.Any(b => b.BILL_ID == item.BILL_ID) && !payRenmasterlist.Any(s => s.BILL_ID == item.BILL_ID))
                        {
                            //// bill_ID_emarg++;
                            var payRenMaster = new EMARG_PAYMENT_RENEW_MASTER
                            {

                                BILL_ID = item.BILL_ID,//bill_ID_emarg,
                                BILL_ID_PREVIOUS = item.BILL_ID_PREVIOUS,
                                //VOUCHER_NO = item.VOUCHER_NO,
                                VOUCHER_NO = item.EMARG_VOUCHER_NO,
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
                                AMOUNT = item.AMOUNT,
                                //RECEIVED_DATE_TIME = item.RECEIVED_DATE_TIME,
                                RECEIVED_DATE_TIME = DateTime.Now,
                                EMARG_DATE_TIME = item.EMARG_DATE_TIME,
                                EMARG_VOUCHER_NO = item.EMARG_VOUCHER_NO,
                                OMMAS_PACKAGE_NO = item.OMMAS_PACKAGE_NO,
                                EMARG_BILL_ID = item.BILL_ID,
                                VOUCHER_TYPE = item.VOUCHER_TYPE
                            };
                            i++;
                            payRenmasterlist.Add(payRenMaster);
                            dbcontext.EMARG_PAYMENT_RENEW_MASTER.Add(payRenMaster);

                            //using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                            //{
                            //    sw.WriteLine("Date :" + DateTime.Now.ToString());
                            //    sw.WriteLine("After   dbcontext.EMARG_PAYMENT_MASTER.Add(payMaster)");

                            //    sw.WriteLine("---------------------------------------------------------------------------------------");
                            //    sw.Close();
                            //}


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

                        //using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\eMARGErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                        //{
                        //    sw.WriteLine("-Step 2-");
                        //    sw.WriteLine("Count :" + i.ToString() + " , Bill ID:" + item.BILL_ID + " ,Bill Date:" + item.BILL_DATE);
                        //    sw.Close();

                        //}


                        if (!dbcontext.EMARG_PAYMENT_RENEW_DETAILS.Any(b => b.BILL_ID == item.BILL_ID && b.TXN_NO == item.TXN_NO) && !payRendetailslist.Any(s => s.BILL_ID == item.BILL_ID && s.TXN_NO == item.TXN_NO))
                        {
                            var payRenDetails = new EMARG_PAYMENT_RENEW_DETAILS
                            {
                                BILL_ID = item.BILL_ID,//bill_ID_emarg,
                                TXN_NO = item.TXN_NO,
                                HEAD_CODE_DEDUCTIONS = item.HEAD_CODE_DEDUCTIONS,
                                AMOUNT_DEDUCTIONS = item.AMOUNT_DEDUCTIONS,
                                EMARG_VOUCHER_NO = item.EMARG_VOUCHER_NO,
                                OMMAS_PACKAGE_NO = item.OMMAS_PACKAGE_NO,
                                EMARG_PACKAGE_NO = (item.EMARG_PACKAGE_NO == null ? "NA" : item.EMARG_PACKAGE_NO),
                                EMARG_DATE_TIME = item.EMARG_DATE_TIME

                            };



                            payRendetailslist.Add(payRenDetails);
                            count++;
                            dbcontext.EMARG_PAYMENT_RENEW_DETAILS.Add(payRenDetails);


                            //using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\eMARGErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                            //{
                            //    sw.WriteLine("-Step 3-");
                            //    sw.WriteLine("Count :" + i.ToString() + " , Bill ID:" + item.BILL_ID + " ,TXN No:" + item.TXN_NO);
                            //    sw.Close();

                            //}


                        }

                    }
                }

                using (TransactionScope scope = new TransactionScope())
                {
                  
                    //BulkInsert(ref dbcontext,paymasterlist);
                    //BulkInsert(ref dbcontext, paydetailslist);

                    //BulkInsert(ref dbcontext, payRenmasterlist);
                    //BulkInsert(ref dbcontext, payRendetailslist);

                    rowsaffected = dbcontext.SaveChanges();
                    dbcontext.Configuration.AutoDetectChangesEnabled = true;
                    scope.Complete();

                    string errorLogPath = System.Configuration.ConfigurationSettings.AppSettings["EmargErrorPath"];
                    using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine(errorLogPath+"\\eMARGErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                    {
                        sw.WriteLine("-Step 4-");
                        sw.WriteLine("Affected rows :" + rowsaffected);
                        sw.Close();

                    }
                }
                return count;


            }
            catch (DbEntityValidationException e)
            {
                string errorLogPath = System.Configuration.ConfigurationSettings.AppSettings["EmargErrorPath"];
                foreach (var eve in e.EntityValidationErrors)
                {
                    using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine(errorLogPath + "\\eMARGErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                    {
                        sw.WriteLine("Date :" + DateTime.Now.ToString());
                        sw.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        sw.WriteLine("---------------------------------------------------------------------------------------");
                        sw.Close();
                    }

                    foreach (var ve in eve.ValidationErrors)
                    {
                        using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine(errorLogPath + "\\eMARGErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                        {
                            sw.WriteLine("Date :" + DateTime.Now.ToString());
                            sw.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                            sw.WriteLine("---------------------------------------------------------------------------------------");
                            sw.Close();
                        }
                    }
                }
                throw;
            }


            catch (Exception ex)
            {
                string errorLogPath = System.Configuration.ConfigurationSettings.AppSettings["EmargErrorPath"];
                dbcontext.Configuration.AutoDetectChangesEnabled = true;
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine(errorLogPath + "\\eMARGErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Catch Block Before : in EmargDAL().EmargPaymentDAL()");
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "EmargDAL.cs - EmargPaymentDAL()");
                    sw.WriteLine("Exception : " + ex.ToString());
                    sw.WriteLine("Exception : " + ex.StackTrace);
                    if (ex.InnerException != null)
                        sw.WriteLine("Inner Exception : " + ex.InnerException.ToString());
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                // ErrorLog.LogError(ex, "EmargDAL.EmargPaymentDAL()");
                //string errorLogPath = System.Configuration.ConfigurationManager.AppSettings["OMMASErrorLogPath"];
                //if (!Directory.Exists(errorLogPath))
                //{
                //    Directory.CreateDirectory(errorLogPath);
                //}
                //using (StreamWriter sw = File.AppendText(Path.Combine(errorLogPath, "")))
                //{
                //    sw.WriteLine("Date :" + DateTime.Now.ToString());
                //    sw.WriteLine("Method : " + "EmargDAL.cs - EmargPaymentDAL()");
                //    sw.WriteLine("Exception : " + ex.ToString());
                //    sw.WriteLine("Exception : " + ex.StackTrace);
                //    if (ex.InnerException != null)
                //        sw.WriteLine("Inner Exception : " + ex.InnerException.ToString());
                //    sw.WriteLine("____________________________________________________");
                //    sw.Close();
                //}
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine(errorLogPath + "\\eMARGErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Catch Block After : in EmargDAL().EmargPaymentDAL()");

                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }
                return 0;
                throw ex;
            }
            finally
            {
                dbcontext.Configuration.AutoDetectChangesEnabled = true;
            }
            //    {
            //        tscaope.Dispose();
            //    }

            //}
        }

        public void BulkInsert<T>(ref PMGSYEntities db, List<T> entities) where T : class
        {
            using (var transaction = db.Database.BeginTransaction())
            {/// TransactionScope scope = new TransactionScope(
                try
                {
                    db.Set<T>().AddRange(entities);
                    db.SaveChanges();
                    transaction.Commit();

                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public int EmargPaymentNewDAL(List<EmargPaymentPullModel> PaymentDetailsList)
        {

            List<EMARG_PAYMENT_MASTER_NEW> paymasterlist = new List<EMARG_PAYMENT_MASTER_NEW>();
            List<EMARG_PAYMENT_DETAILS_NEW> paydetailslist = new List<EMARG_PAYMENT_DETAILS_NEW>();

            CommonFunctions common = new CommonFunctions();
            int rowsaffected = 0;
            int max_txn_no = 0;
            int count = 0;
            PMGSYEntities dbcontext = new PMGSYEntities();

            int i = 0;
            using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\eMARGErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
            {

                sw.WriteLine("Step 1-");
                sw.WriteLine("List Count :" + PaymentDetailsList.Count);
                sw.Close();

            }

            //using (TransactionScope tscaope = new TransactionScope(TransactionScopeOption.Required,
            //                   new System.TimeSpan(0, 15, 0)))
            //{
            try
            {

                //var max_bill_ID = dbcontext.ACC_BILL_MASTER.Max(ac => (ac.BILL_ID));
                //// var bill_ID_emarg = dbcontext.EMARG_PAYMENT_MASTER.Any() ? dbcontext.EMARG_PAYMENT_MASTER.Max(ad => ad.BILL_ID):0;
                dbcontext.Configuration.AutoDetectChangesEnabled = false;
                foreach (var item in PaymentDetailsList)
                {
                    if (item.BILL_YEAR == 2021) 
                    {
                        //var mast_con_ID = dbcontext.MANE_IMS_CONTRACT.Where(an => an.MANE_AGREEMENT_NUMBER == item.AGREEMENT_CODE).Min(ms => ms.MAST_CON_ID);

                        if (!dbcontext.EMARG_PAYMENT_MASTER_NEW.Any(b => b.BILL_ID == item.BILL_ID) && !paymasterlist.Any(s => s.BILL_ID == item.BILL_ID))
                        {
                            //// bill_ID_emarg++;
                            var payMaster = new EMARG_PAYMENT_MASTER_NEW
                            {

                                BILL_ID = item.BILL_ID,//bill_ID_emarg,
                                BILL_ID_TEMP = null,
                                BILL_ID_PREVIOUS = item.BILL_ID_PREVIOUS,
                                //VOUCHER_NO = item.VOUCHER_NO,  ---changed on 30-12-21
                                VOUCHER_NO = item.EMARG_VOUCHER_NO,
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
                                AMOUNT = item.AMOUNT,
                                IS_PORTED = item.IS_PORTED,
                                OMMAS_BILL_ID = item.OMMAS_BILL_ID,
                                //RECEIVED_DATE_TIME = item.RECEIVED_DATE_TIME,
                                RECEIVED_DATE_TIME = DateTime.Now,
                                RECEIVED_ACK_DATE_TIME = item.RECEIVED_ACK_DATE_TIME,
                                RECEIVED_STATUS = item.RECEIVED_STATUS,
                                EMARG_DATE_TIME = item.EMARG_DATE_TIME

                            };
                            i++;
                            paymasterlist.Add(payMaster);
                            dbcontext.EMARG_PAYMENT_MASTER_NEW.Add(payMaster);


                        }

                        //using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\eMARGErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                        //{
                        //    sw.WriteLine("-Step 2-");
                        //    sw.WriteLine("Count :" + i.ToString() + " , Bill ID:" + item.BILL_ID + " ,Bill Date:" + item.BILL_DATE);
                        //    sw.Close();

                        //}


                        if (!dbcontext.EMARG_PAYMENT_DETAILS_NEW.Any(b => b.BILL_ID == item.BILL_ID && b.TXN_NO == item.TXN_NO) && !paydetailslist.Any(s => s.BILL_ID == item.BILL_ID && s.TXN_NO == item.TXN_NO))
                        {
                            var payDetails = new EMARG_PAYMENT_DETAILS_NEW
                            {
                                BILL_ID = item.BILL_ID,//bill_ID_emarg,
                                TXN_NO = item.TXN_NO,
                                HEAD_CODE_DEDUCTIONS = item.HEAD_CODE_DEDUCTIONS,
                                AMOUNT_DEDUCTIONS = item.AMOUNT_DEDUCTIONS,
                                EMARG_VOUCHER_NO = item.EMARG_VOUCHER_NO,
                                OMMAS_PACKAGE_NO = item.OMMAS_PACKAGE_NO,
                                EMARG_PACKAGE_NO = (item.EMARG_PACKAGE_NO == null ? "NA" : item.EMARG_PACKAGE_NO),
                                EMARG_DATE_TIME = item.EMARG_DATE_TIME,
                                IS_PORTED = item.IS_PORTED,
                                OMMAS_BILL_ID = item.OMMAS_BILL_ID

                            };

                            paydetailslist.Add(payDetails);
                            count++;
                            dbcontext.EMARG_PAYMENT_DETAILS_NEW.Add(payDetails);


                            //using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\eMARGErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                            //{
                            //    sw.WriteLine("-Step 3-");
                            //    sw.WriteLine("Count :" + i.ToString() + " , Bill ID:" + item.BILL_ID + " ,TXN No:" + item.TXN_NO);
                            //    sw.Close();

                            //}


                        }

                    }

                    using (TransactionScope scope = new TransactionScope())
                    {
                        rowsaffected = dbcontext.SaveChanges();
                        dbcontext.Configuration.AutoDetectChangesEnabled = true;
                        scope.Complete();

                        using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\eMARGErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                        {
                            sw.WriteLine("-Step 4-");
                            sw.WriteLine("Affected rows :" + rowsaffected);
                            sw.Close();

                        }
                    }
                }
                return count;


            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\eMARGErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                    {
                        sw.WriteLine("Date :" + DateTime.Now.ToString());
                        sw.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        sw.WriteLine("---------------------------------------------------------------------------------------");
                        sw.Close();
                    }

                    foreach (var ve in eve.ValidationErrors)
                    {
                        using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\eMARGErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                        {
                            sw.WriteLine("Date :" + DateTime.Now.ToString());
                            sw.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                            sw.WriteLine("---------------------------------------------------------------------------------------");
                            sw.Close();
                        }
                    }
                }
                throw;
            }


            catch (Exception ex)
            {
                dbcontext.Configuration.AutoDetectChangesEnabled = true;
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\eMARGErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Catch Block Before : in EmargDAL().EmargPaymentNewDAL()");
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "EmargDAL.cs - EmargPaymentNewDAL()");
                    sw.WriteLine("Exception : " + ex.ToString());
                    sw.WriteLine("Exception : " + ex.StackTrace);
                    if (ex.InnerException != null)
                        sw.WriteLine("Inner Exception : " + ex.InnerException.ToString());
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\eMARGErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Catch Block After : in EmargDAL().EmargPaymentNewDAL()");

                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }
                return 0;
                throw ex;
            }
            finally
            {
                dbcontext.Configuration.AutoDetectChangesEnabled = true;
            }

        }
        #endregion

        //#region EMARG Balance Work Package Details Webservice

        //public void SaveRoadWiseBalanceWorkPackageDetails(List<EmargRoadWiseBalanceWorks> RoadList)
        //{
        //    foreach (var data in RoadList)
        //    {
        //        using (PMGSYEntities dbcontext = new PMGSYEntities())
        //        {
        //            using (TransactionScope tscaope = new TransactionScope())
        //            {
        //                EMARG_ROAD_WISE_BALANCE_WORK_PACKAGE_DETAILS sa = new EMARG_ROAD_WISE_BALANCE_WORK_PACKAGE_DETAILS();

        //                try
        //                {

        //                    if (dbcontext.EMARG_ROAD_WISE_BALANCE_WORK_PACKAGE_DETAILS.Where(m => m.RECORD_ID == data.RECORD_ID).Any())
        //                    {

        //                    }
        //                    else
        //                    {
        //                        #region New Road Details Saving
        //                        sa.RECORD_ID = data.RECORD_ID;
        //                        sa.STATE_CODE = data.STATE_CODE;
        //                        sa.STATE_NAME = data.STATE_NAME;
        //                        sa.DISTRICT_CODE = data.DISTRICT_CODE;
        //                        sa.DISTRICT_NAME = data.DISTRICT_NAME;
        //                        sa.BLOCK_CODE = data.BLOCK_CODE;
        //                        sa.BLOCK_NAME = data.BLOCK_NAME;
        //                        sa.PIU_CODE = data.PIU_CODE;
        //                        sa.PIU_NAME = data.PIU_NAME;
        //                        sa.PACKAGE_NO = data.PACKAGE_NO;
        //                        sa.EMARG_PACKAGE_NO = data.EMARG_PACKAGE_NO;
        //                        sa.ROAD_CODE = data.ROAD_CODE;
        //                        sa.ROAD_NAME = data.ROAD_NAME;
        //                        sa.CC_LENGTH = data.CC_LENGTH;
        //                        sa.BT_LENGTH = data.BT_LENGTH;
        //                        sa.ROAD_WIDTH = data.ROAD_WIDTH;
        //                        sa.WORK_ORDER_NO = data.WORK_ORDER_NO;
        //                        sa.WORK_ORDER_DATE = data.WORK_ORDER_DATE;
        //                        sa.AGREEMENT_NO = data.AGREEMENT_NO;
        //                        sa.AGREEMENT_DATE = data.AGREEMENT_DATE;
        //                        sa.CONTRACTOR_NAME = data.CONTRACTOR_NAME;
        //                        sa.CONTRACTOR_PAN = data.CONTRACTOR_PAN;

        //                        dbcontext.EMARG_ROAD_WISE_BALANCE_WORK_PACKAGE_DETAILS.Add(sa);
        //                        dbcontext.SaveChanges();
        //                        tscaope.Complete();
        //                        #endregion
        //                    }

        //                }
        //                catch (Exception ex)
        //                {

        //                    string errorLogPath = System.Configuration.ConfigurationSettings.AppSettings["EmargErrorPath"];
        //                    using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine(errorLogPath + "\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
        //                    {
        //                        sw.WriteLine("-------------SaveDataFromRoadWiseBalanceWorkPackageDetails--------------------------------------------------------------------------");
        //                        sw.WriteLine("Date :" + DateTime.Now.ToString());
        //                        sw.WriteLine("STATE : " + sa.STATE_NAME, sa.DISTRICT_NAME, sa.PACKAGE_NO, sa.ROAD_CODE);
        //                        sw.WriteLine("DISTRICT : " + sa.DISTRICT_NAME);
        //                        sw.WriteLine("PACKAGE NO : " + sa.PACKAGE_NO);
        //                        sw.WriteLine("ROAD CODE : " + sa.ROAD_CODE);
        //                        if (PMGSY.Extensions.PMGSYSession.Current != null)
        //                        {
        //                            sw.WriteLine("UserName : " + PMGSY.Extensions.PMGSYSession.Current.UserName);
        //                        }
        //                        sw.WriteLine("---------------------------------------------------------------------------------------");
        //                        sw.Close();
        //                    }
        //                    continue;
        //                }
        //                finally
        //                {
        //                    dbcontext.Dispose();
        //                }
        //            }
        //        }


        //    }


        //}
        //#endregion

        #region State Ranking KPI
        public void SaveStateRankingKPI(List<StateRankKPI> list)
        {// First Level Service Details
            PMGSYEntities dbcontext = new PMGSYEntities();
            dbcontext.Configuration.AutoDetectChangesEnabled = false;





            try
            {
                foreach (var data in list)
                {
                    using (TransactionScope tscaope = new TransactionScope())
                    {
                        //EMARG_ROAD_DETAILS sa = new EMARG_ROAD_DETAILS();
                        STATE_RANKING_KPI sa = new STATE_RANKING_KPI();

                        try
                        {

                            sa.KPI_ID = (dbcontext.STATE_RANKING_KPI.Any() ? dbcontext.STATE_RANKING_KPI.Max(m => m.KPI_ID) + 1 : 1);
                            sa.stateId = data.stateId;
                            sa.stateName = data.stateName;
                            sa.packageCount = data.packageCount;
                            sa.workableCount = data.workableCount;
                            sa.verifiedCount = data.verifiedCount;
                            sa.freezedCount = data.freezedCount;
                            sa.unfreezedCount = data.unfreezedCount;
                            sa.lockedCount = data.lockedCount;
                            sa.manualExpenditureCount = data.manualExpenditureCount;
                            sa.completedCount = data.completedCount;
                            sa.invalidCount = data.invalidCount;
                            sa.incorrectCount = data.incorrectCount;
                            sa.paymentStartedCount = data.paymentStartedCount;
                            sa.stateName = data.stateName;
                            sa.GENERATED_DATE = System.DateTime.Now;
                            sa.eligibleContractorCount = data.eligibleContractorCount;
                            sa.registeredContractorCount = data.registeredContractorCount;
                            sa.districtCount = data.districtCount;
                            sa.dscEnrolledUserCount = data.dscEnrolledUserCount;
                            dbcontext.STATE_RANKING_KPI.Add(sa);
                            dbcontext.SaveChanges();
                            tscaope.Complete();
                        }
                        catch (Exception ex)
                        {

                            string errorLogPath = System.Configuration.ConfigurationSettings.AppSettings["EmargErrorPath"];
                            using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine(errorLogPath + "\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                            {
                                sw.WriteLine("---------------------------------------------------------------------------------------");
                                sw.WriteLine("Method Name : EmargDAL().SaveStateRankingKPI()");

                                if (PMGSY.Extensions.PMGSYSession.Current != null)
                                {
                                    sw.WriteLine("UserName : " + PMGSY.Extensions.PMGSYSession.Current.UserName);
                                }
                                sw.WriteLine("---------------------------------------------------------------------------------------");
                                sw.Close();
                            }
                            continue;
                        }
                        finally
                        {
                            // dbcontext.Configuration.AutoDetectChangesEnabled = true;
                            //  dbcontext.Dispose();
                        }
                    }



                }
            }
            catch (Exception ex)
            {
                string errorLogPath = System.Configuration.ConfigurationSettings.AppSettings["EmargErrorPath"];
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine(errorLogPath + "\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.WriteLine("Outer Catch :: Method Name : EmargDAL().SaveStateRankingKPI()");

                    if (PMGSY.Extensions.PMGSYSession.Current != null)
                    {
                        sw.WriteLine("UserName : " + PMGSY.Extensions.PMGSYSession.Current.UserName);
                    }
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }
            }
            finally
            {
                dbcontext.Configuration.AutoDetectChangesEnabled = true;
                dbcontext.Dispose();
            }


        }
        #endregion


        // new  DAL METHOD FOR EMARG ROADWISE EXPENDITURE BALANCE WORK ON 05-01-2022

        #region EMARG RoadWise Expenditure Balance Work
        internal void SaveRoadWiseExpenditureBalanceWork(List<EMARG_ROADWISE_EXPENDITURE_BALANCEWORK> RoadList)
        {
            PMGSYEntities dbcontext = new PMGSYEntities();
            try
            {

                foreach (var data in RoadList)
                {
                    EMARG_ROADWISE_EXPENDITURE_BALANCE_WORK_MASTER emargmast = new EMARG_ROADWISE_EXPENDITURE_BALANCE_WORK_MASTER();
                    EMARG_ROADWISE_EXPENDITURE_BALANCE_WORK_DETAILS emargdet = new EMARG_ROADWISE_EXPENDITURE_BALANCE_WORK_DETAILS();
                    using (TransactionScope tscaope = new TransactionScope())
                    {

                        if (!dbcontext.EMARG_ROADWISE_EXPENDITURE_BALANCE_WORK_MASTER.Any(m => m.BILL_ID == data.BILL_ID))
                        {
                            #region Saving RoadWise Expenditure Balance Work

                            //var id = dbcontext.EMARG_ROADWISE_EXPENDITURE_BALANCE_WORK.DefaultIfEmpty().Max(x => x == null ? 0 : x.id);
                            //var id = dbcontext.EMARG_ROADWISE_EXPENDITURE_BALANCE_WORK.Count();     //int max = dbcontext.EMARG_ROADWISE_EXPENDITURE_BALANCE_WORK.Max(x => x.id as int?) ?? 0;
                            var id = 0;
                            if (!dbcontext.EMARG_ROADWISE_EXPENDITURE_BALANCE_WORK_MASTER.Any())
                            {
                                id = 0;
                            }
                            else
                            {
                                id = dbcontext.EMARG_ROADWISE_EXPENDITURE_BALANCE_WORK_MASTER.DefaultIfEmpty().Max(x => x.id);
                            }

                            emargmast.id = id + 1;
                            emargmast.BILL_ID = data.BILL_ID;
                            emargmast.BILL_ID_PREVIOUS = data.BILL_ID_PREVIOUS;
                            emargmast.EMARG_VOUCHER_NO = data.EMARG_VOUCHER_NO;
                            emargmast.VOUCHER_NO = data.VOUCHER_NO;
                            emargmast.BILL_MONTH = data.BILL_MONTH;
                            emargmast.BILL_YEAR = data.BILL_YEAR;
                            emargmast.BILL_DATE = data.BILL_DATE;
                            emargmast.CHQ_NO = data.CHQ_NO;
                            emargmast.CHQ_DATE = data.CHQ_DATE;
                            emargmast.CHQ_AMOUNT = data.CHQ_AMOUNT;
                            emargmast.CASH_AMOUNT = data.CASH_AMOUNT;
                            emargmast.GROSS_AMOUNT = data.GROSS_AMOUNT;
                            emargmast.PIU_CODE = data.PIU_CODE;
                            emargmast.MAST_CON_ID = data.MAST_CON_ID;
                            emargmast.NARRATION = data.NARRATION;

                            emargmast.ROAD_CODE = data.ROAD_CODE;
                            emargmast.AGREEMENT_CODE = data.AGREEMENT_CODE;
                            emargmast.FUND_TYPE = data.FUND_TYPE;
                            emargmast.PAYEE_NAME = data.PAYEE_NAME;
                            emargmast.ACCOUNT_NUMBER = data.ACCOUNT_NUMBER;
                            emargmast.IFSC_CODE = data.IFSC_CODE;
                            emargmast.EMARG_DATE_TIME = data.EMARG_DATE_TIME;
                            emargmast.HEAD_CODE = data.HEAD_CODE;
                            emargmast.AMOUNT = data.AMOUNT;
                            dbcontext.EMARG_ROADWISE_EXPENDITURE_BALANCE_WORK_MASTER.Add(emargmast);
                            //     dbcontext.SaveChanges();                               
                            #endregion
                        }

                        //if (!dbcontext.EMARG_ROADWISE_EXPENDITURE_BALANCE_WORK_DETAILS.Where(m => m.BILL_ID == data.BILL_ID && m.TXN_NO == data.TXN_NO).Any())
                        if (!dbcontext.EMARG_ROADWISE_EXPENDITURE_BALANCE_WORK_DETAILS.Any(m => m.BILL_ID == data.BILL_ID && m.TXN_NO == data.TXN_NO))
                        {
                            //}
                            //else
                            //{
                            #region Saving RoadWise Expenditure Balance Work
                            //var id = dbcontext.EMARG_ROADWISE_EXPENDITURE_BALANCE_WORK.DefaultIfEmpty().Max(x => x == null ? 0 : x.id);
                            //var id = dbcontext.EMARG_ROADWISE_EXPENDITURE_BALANCE_WORK.Count();   
                            //int max = dbcontext.EMARG_ROADWISE_EXPENDITURE_BALANCE_WORK.Max(x => x.id as int?) ?? 0;
                            var id = 0;
                            if (!dbcontext.EMARG_ROADWISE_EXPENDITURE_BALANCE_WORK_DETAILS.Any())
                            {
                                id = 0;
                            }
                            else
                            {
                                id = dbcontext.EMARG_ROADWISE_EXPENDITURE_BALANCE_WORK_DETAILS.DefaultIfEmpty().Max(x => x.id);
                            }
                            emargdet.id = id + 1;
                            emargdet.BILL_ID = data.BILL_ID;
                            emargdet.BILL_ID_PREVIOUS = data.BILL_ID_PREVIOUS;
                            emargdet.EMARG_VOUCHER_NO = data.EMARG_VOUCHER_NO;
                            emargdet.VOUCHER_NO = data.VOUCHER_NO;

                            emargdet.OMMAS_PACKAGE_NO = data.OMMAS_PACKAGE_NO;

                            emargdet.EMARG_DATE_TIME = data.EMARG_DATE_TIME;

                            emargdet.TXN_NO = data.TXN_NO;
                            emargdet.HEAD_CODE_DEDUCTIONS = data.HEAD_CODE_DEDUCTIONS;
                            emargdet.AMOUNT_DEDUCTIONS = data.AMOUNT_DEDUCTIONS;
                            emargdet.VOUCHER_TYPE = data.VOUCHER_TYPE;

                            dbcontext.EMARG_ROADWISE_EXPENDITURE_BALANCE_WORK_DETAILS.Add(emargdet);
                            //  dbcontext.SaveChanges();
                            //tscaope.Complete();
                            #endregion
                        }

                        dbcontext.SaveChanges();
                        tscaope.Complete();


                    }


                }
            }
            catch (Exception ex)
            {

                string errorLogPath = System.Configuration.ConfigurationSettings.AppSettings["EmargErrorPath"];
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine(errorLogPath + "\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("-------------SaveDataFromEMARG_ROADWISE_EXPENDITURE_BALANCE_WORK--------------------------------------------------------------------------");
                    sw.WriteLine("Date :" + DateTime.Now.ToString());

                    if (PMGSY.Extensions.PMGSYSession.Current != null)
                    {
                        sw.WriteLine("UserName : " + PMGSY.Extensions.PMGSYSession.Current.UserName);
                    }
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }
                // continue;
            }
            finally
            {
                dbcontext.Dispose();
            }

        } // end method
        #endregion

        //NEW FINAL DAL FOR LOCKED PACKAGE RECORD ON 05-01-2022

        #region EMARG Locked Package Record
        internal void SaveLockedPackageRecord(List<EMARG_LOCKED_PACKAGE_RECORD> RoadList)
        {
            foreach (var data in RoadList)
            {
                using (PMGSYEntities dbcontext = new PMGSYEntities())
                {
                    using (TransactionScope tscaope = new TransactionScope())
                    {
                        EMARG_LOCKED_PACKAGE_RECORD sa = new EMARG_LOCKED_PACKAGE_RECORD();

                        try
                        {
                            if (!dbcontext.EMARG_LOCKED_PACKAGE_RECORD.Any(m => m.EID == data.EID))
                            //if (dbcontext.EMARG_LOCKED_PACKAGE_RECORD.Where(m => m.agreementNo == data.agreementNo).Any())
                            //{

                            //}
                            //else
                            {
                                #region New Road Details Saving

                                //var id = dbcontext.EMARG_LOCKED_PACKAGE_RECORD.Max(x => x == null ? 0 : x.id);
                                var id = 0;
                                if (!dbcontext.EMARG_LOCKED_PACKAGE_RECORD.Any())
                                {
                                    id = 0;
                                }
                                else
                                {
                                    id = dbcontext.EMARG_LOCKED_PACKAGE_RECORD.DefaultIfEmpty().Max(x => x.id);
                                }

                                sa.id = id + 1;
                                sa.EID = data.EID;
                                sa.stateCode = data.stateCode;
                                sa.stateName = data.stateName;
                                sa.districtCode = data.districtCode;
                                sa.districtName = data.districtName;
                                sa.piuCode = data.piuCode;
                                sa.piuName = data.piuName;
                                sa.ommasPackageNo = data.ommasPackageNo;
                                sa.agreementNo = data.agreementNo;
                                sa.agreementDate = data.agreementDate;
                                sa.contractorName = data.contractorName;
                                sa.contractorPan = data.contractorPan;
                                sa.blockCode = data.blockCode;
                                sa.blockName = data.blockName;
                                sa.roadCode = data.roadCode;
                                sa.roadName = data.roadName;
                                sa.ccLength = data.ccLength;
                                sa.btLength = data.btLength;
                                sa.roadWidth = data.roadWidth;


                                dbcontext.EMARG_LOCKED_PACKAGE_RECORD.Add(sa);
                                dbcontext.SaveChanges();
                                tscaope.Complete();
                                #endregion
                            }

                        }
                        catch (Exception ex)
                        {

                            string errorLogPath = System.Configuration.ConfigurationSettings.AppSettings["EmargErrorPath"];
                            using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine(errorLogPath + "\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                            {
                                sw.WriteLine("-------------SaveDataFromEMARGLOCKED_PACKAGE_RECORD--------------------------------------------------------------------------");
                                //sw.WriteLine("Date :" + DateTime.Now.ToString());
                                //sw.WriteLine("STATE : " + sa.STATE_NAME, sa.DISTRICT_NAME, sa.PACKAGE_NO, sa.ROAD_CODE);
                                //sw.WriteLine("DISTRICT : " + sa.DISTRICT_NAME);
                                //sw.WriteLine("PACKAGE NO : " + sa.PACKAGE_NO);
                                //sw.WriteLine("ROAD CODE : " + sa.ROAD_CODE);
                                if (PMGSY.Extensions.PMGSYSession.Current != null)
                                {
                                    sw.WriteLine("UserName : " + PMGSY.Extensions.PMGSYSession.Current.UserName);
                                }
                                sw.WriteLine("---------------------------------------------------------------------------------------");
                                sw.Close();
                            }
                            continue;
                        }
                        finally
                        {
                            dbcontext.Dispose();
                        }
                    }
                }


            }
        }
        #endregion


        // UPDATED DAL FOR EMARG BALANCE WORK PACKAGE DETAILS ON 05-01-2022

        #region EMARG Balance Work Package Details Webservice

        public void SaveRoadWiseBalanceWorkPackageDetails(List<EmargRoadWiseBalanceWorks> RoadList)
        {
            foreach (var data in RoadList)
            {
                using (PMGSYEntities dbcontext = new PMGSYEntities())
                {
                    using (TransactionScope tscaope = new TransactionScope(TransactionScopeOption.Required, new System.TimeSpan(0, 15, 0)))
                    {
                        EMARG_ROAD_WISE_BALANCE_WORK_PACKAGE_DETAILS sa = new EMARG_ROAD_WISE_BALANCE_WORK_PACKAGE_DETAILS();

                        try
                        {

                            //if (dbcontext.EMARG_ROAD_WISE_BALANCE_WORK_PACKAGE_DETAILS.Where(m => m.RECORD_ID == data.recordId).Any())
                            //{

                            //}
                            //else
                            {
                                #region New Road Details Saving
                                sa.RECORD_ID = data.RECORD_ID;
                                sa.STATE_CODE = data.STATE_CODE;
                                //sa.STATE_NAME = data.stateName;
                                sa.DISTRICT_CODE = data.DISTRICT_CODE;
                                //sa.DISTRICT_NAME = data.districtName;
                                sa.BLOCK_CODE = data.BLOCK_CODE;
                                //sa.BLOCK_NAME = data.blockName;
                                sa.ROAD_CODE = data.ROAD_CODE;
                                sa.PIU_CODE = data.PIU_CODE;
                                //sa.PIU_NAME = data.piuName;
                                sa.PACKAGE_NO = data.PACKAGE_NUMBER;
                                sa.EMARG_PACKAGE_NO = data.E_MARG_PACKAGE_NUMBER;
                                //sa.ROAD_CODE = data.roadCode;
                                //sa.ROAD_NAME = data.roadName;
                                //sa.CC_LENGTH = data.ccLength;
                                //sa.BT_LENGTH = data.btLength;
                                //sa.ROAD_WIDTH = data.roadWidth;
                                //sa.WORK_ORDER_NO = data.workOrderNo;
                                //sa.WORK_ORDER_DATE = data.workOrderDate;
                                sa.CONTRACTOR_PAN = data.CONTRACTOR_PAN;
                                sa.CONTRACTOR_NAME = data.CONTRACTOR_NAME;
                                sa.AGREEMENT_NO = data.AGREEMENT_NUMBER;
                                sa.AGREEMENT_DATE = data.AGREEMENT_DATE;
                                sa.MAINTENANCE_START_DATE = data.MAINTENANCE_START_DATE;
                                sa.MAINTENANCE_END_DATE = data.MAINTENANCE_END_DATE;
                                //sa.CONTRACTOR_NAME = data.contractorName;
                                //sa.CONTRACTOR_PAN = data.contractorPan;

                                dbcontext.EMARG_ROAD_WISE_BALANCE_WORK_PACKAGE_DETAILS.Add(sa);
                                dbcontext.SaveChanges();
                                tscaope.Complete();
                                #endregion
                            }

                        }
                        catch (Exception ex)
                        {

                            string errorLogPath = System.Configuration.ConfigurationSettings.AppSettings["EmargErrorPath"];
                            using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine(errorLogPath + "\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                            {
                                sw.WriteLine("-------------SaveDataFromRoadWiseBalanceWorkPackageDetails--------------------------------------------------------------------------");
                                sw.WriteLine("Date :" + DateTime.Now.ToString());
                                sw.WriteLine("STATE Code: " + sa.STATE_CODE, sa.DISTRICT_CODE, sa.PACKAGE_NO, sa.ROAD_CODE);
                                sw.WriteLine("DISTRICT : " + sa.DISTRICT_CODE);
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
                        finally
                        {
                            dbcontext.Dispose();
                        }
                    }
                }


            }


        }
        #endregion

        //Added on 18-08-2022 by Shreyas
        #region EMARG KPI PM Awards Details
        internal int SaveKPIPMAwardsDetails(List<Emarg_KPI_PM_Awardsdetails> records, int userid, string IPADDRESS)
        {
            int rowsaffected = 0;
            int totalrowsaffected = 0;
            foreach (var data in records)
            {
                using (PMGSYEntities dbcontext = new PMGSYEntities())
                {
                    using (TransactionScope tscaope = new TransactionScope())
                    {
                        Emarg_KPI_PM_Awardsdetails sa = new Emarg_KPI_PM_Awardsdetails();

                        try
                        {

                            var curDate = DateTime.Now.Date;
                            if (!dbcontext.Emarg_KPI_PM_Awardsdetails.Any(m => m.MAST_DISTRICT_CODE == data.MAST_DISTRICT_CODE && DateTime.Compare(m.DATA_RECEIVEING_DATE, curDate) == 0))
                            {
                                var id = 0;
                                if (!dbcontext.Emarg_KPI_PM_Awardsdetails.Any())
                                {
                                    id = 0;
                                }
                                else
                                {
                                    id = dbcontext.Emarg_KPI_PM_Awardsdetails.DefaultIfEmpty().Max(x => x.ID);
                                }


                                sa.ID = id + 1;
                                sa.MAST_STATE_CODE = data.MAST_STATE_CODE;
                                sa.MAST_STATE_NAME = data.MAST_STATE_NAME;
                                sa.MAST_DISTRICT_CODE = data.MAST_DISTRICT_CODE;
                                sa.MAST_DISTRICT_NAME = data.MAST_DISTRICT_NAME;
                                sa.RI_REQUIRED = data.RI_REQUIRED;
                                sa.RI_COMPLETED = data.RI_COMPLETED;
                                sa.DATA_RECEIVEING_DATE = curDate;
                                sa.USERID = userid;
                                sa.IPADDRESS = IPADDRESS;

                                dbcontext.Emarg_KPI_PM_Awardsdetails.Add(sa);
                                rowsaffected = dbcontext.SaveChanges();
                                dbcontext.Configuration.AutoDetectChangesEnabled = true;
                                tscaope.Complete();
                                if (rowsaffected > 0)
                                {
                                    totalrowsaffected++;
                                }

                            }


                        }
                        catch (Exception ex)
                        {

                            string errorLogPath = System.Configuration.ConfigurationSettings.AppSettings["EmargErrorPath"];
                            using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine(errorLogPath + "\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                            {
                                sw.WriteLine("-------------SaveKPIPMAwardsDetails--------------------------------------------------------------------------");

                                if (PMGSY.Extensions.PMGSYSession.Current != null)
                                {
                                    sw.WriteLine("UserName : " + PMGSY.Extensions.PMGSYSession.Current.UserName);
                                }
                                sw.WriteLine("---------------------------------------------------------------------------------------");
                                sw.Close();
                            }
                            continue;
                        }
                        finally
                        {

                            dbcontext.Dispose();
                        }
                    }
                }


            }
            return totalrowsaffected;
        }
        #endregion

        //Added on 05-12-2022 by Bhushan
        #region Cancel Cheque Payment

        public void saveChequeCancellationDetails(List<EMARG_CHEQUE_CANCELLATION_DETAILS_MODEL> cancelChequeList)
        {

            foreach (var data in cancelChequeList)
            {
                PMGSYEntities dbcontext = new PMGSYEntities();

                EMARG_CHEQUE_CANCELLATION_DETAILS CancelChequeDetail = new EMARG_CHEQUE_CANCELLATION_DETAILS();

                using (TransactionScope tscaope = new TransactionScope())
                {
                    try
                    {

                        if (!dbcontext.EMARG_CHEQUE_CANCELLATION_DETAILS.Any(m => m.BILL_ID == data.BILL_ID && m.EMARG_VOUCHER_NO == data.EMARG_VOUCHER_NO && m.CHEQ_NO == data.CHEQUE_NO && m.ROAD_CODE == data.ROAD_CODE && m.SCROLL_GENERATION_TYPE == data.SCROLL_GENERATION_TYPE))
                        {
                            CancelChequeDetail.ID = dbcontext.EMARG_CHEQUE_CANCELLATION_DETAILS.Any() ? dbcontext.EMARG_CHEQUE_CANCELLATION_DETAILS.Max(x => x.ID) + 1 : 1;
                            CancelChequeDetail.BILL_ID = data.BILL_ID;
                            CancelChequeDetail.EMARG_VOUCHER_NO = data.EMARG_VOUCHER_NO;
                            CancelChequeDetail.CANCELLATION_MONTH = data.CANCELLATION_MONTH;
                            CancelChequeDetail.CANCELLATION_YEAR = data.CANCELLATION_YEAR;
                            CancelChequeDetail.CANCELLATION_DATE = data.CANCELLATION_DATE;
                            CancelChequeDetail.CHQ_AMOUNT = data.CHEQUE_AMOUNT;
                            CancelChequeDetail.CHEQ_NO = data.CHEQUE_NO;
                            CancelChequeDetail.PIU_CODE = data.PIU_CODE;
                            CancelChequeDetail.CANCELLATION_REMARKS = data.CANCELLATION_REMARKS;
                            CancelChequeDetail.ROAD_CODE = data.ROAD_CODE;
                            CancelChequeDetail.SCROLL_GENERATION_TYPE = data.SCROLL_GENERATION_TYPE;
                            dbcontext.EMARG_CHEQUE_CANCELLATION_DETAILS.Add(CancelChequeDetail);
                        }

                        dbcontext.SaveChanges();
                        tscaope.Complete();
                    }
                    catch (Exception ex)
                    {

                        string errorLogPath = System.Configuration.ConfigurationSettings.AppSettings["EmargErrorPath"];
                        using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine(errorLogPath + "\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                        {
                            sw.WriteLine("-------------SaveDataFrom saveChequeCancellationDetails--------------------------------------------------------------------------");
                            sw.WriteLine("Date :" + DateTime.Now.ToString());
                            sw.WriteLine("Exception " + ex);
                            if (PMGSY.Extensions.PMGSYSession.Current != null)
                            {
                                sw.WriteLine("UserName : " + PMGSY.Extensions.PMGSYSession.Current.UserName);
                            }
                            sw.WriteLine("---------------------------------------------------------------------------------------");
                            sw.Close();
                        }
                        continue;
                    }
                    finally
                    {
                        dbcontext.Dispose();
                    }
                }

            }

        }

        #endregion
    }

}