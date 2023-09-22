#region File Header
/*
        * Project Id    :
        * Project Name  :   PMGSY
        * Name          :   EAuthorizationDAL.cs        
        * Description   :   EAuthorization DAL describing PIU AND EO Details
        * Author        :   Avinash
        * Creation Date :   15/November/2018
 **/
#endregion

using PMGSY.Common;
using PMGSY.Extensions;
using PMGSY.Models;
using PMGSY.Models.Common;
using PMGSY.Models.EAuthorization;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using System.Configuration;
using System.IO;


namespace PMGSY.DAL.EAuthorization
{
    public class EAuthorizationDAL : IEAuthorizationDAL
    {

        #region Properties
        PMGSYEntities dbContext = null;

        #endregion


        #region Insert/Delete/Update/Finalize EAuth Details/Master PIU
        #region Adding EAuthorization Master
        /// <summary>
        /// Method to Add Master Entry And Returning EAuthID
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        public EAuthorizationMasterModel AddEAuthorizationMasterDetails(EAuthorizationMasterModel model)
        {
            ACC_EAUTH_MASTER objEAuthMaster = null;
            Int32 Auth_ID = 0;
            CommonFunctions common = null;
            EAuthorizationMasterModel returnModel = null;
            try
            {
                common = new CommonFunctions();
                returnModel = new EAuthorizationMasterModel();
                using (TransactionScope scope = new TransactionScope())
                {
                    dbContext = new PMGSYEntities();
                    objEAuthMaster = new ACC_EAUTH_MASTER();
                    objEAuthMaster.EAUTH_ID = (dbContext.ACC_EAUTH_MASTER.Any() ? dbContext.ACC_EAUTH_MASTER.Max(X => X.EAUTH_ID) : 0) + 1;

                    string EAuthNo=GetAuthorizationNumber(model.BILL_MONTH, model.BILL_YEAR, PMGSYSession.Current.StateCode, model.AdminNDCode);
                    if (EAuthNo != model.EAUTHORIZATION_NO)
                    {
                        returnModel.status = false;
                        returnModel.StatusMessage = "EAuthorization Number is Invalid";
                    }
                    else
                    {

                        DateTime TodaysDate = DateTime.Now;
                        string EauthDate = common.GetDateTimeToString(TodaysDate);

                        if (EauthDate != model.EAUTHORIZATION_DATE)
                        {
                            returnModel.status = false;
                            returnModel.StatusMessage = "EAuthorization Date  is Invalid";

                        }
                        else
                        {
                            objEAuthMaster.EAUTH_NO = model.EAUTHORIZATION_NO;
                            objEAuthMaster.EAUTH_MONTH = model.BILL_MONTH;
                            objEAuthMaster.EAUTH_YEAR = model.BILL_YEAR;
                            objEAuthMaster.EAUTH_DATE = GetStringToDateTime(model.EAUTHORIZATION_DATE);
                            objEAuthMaster.TOTAL_AUTH_AMOUNT_REQ = 0;
                            objEAuthMaster.TOTAL_AUTH_APPROVED = 0;
                            objEAuthMaster.EAUTH_STATUS = "N";
                            objEAuthMaster.FUND_TYPE = model.FundType;
                            objEAuthMaster.ADMIN_ND_CODE = model.AdminNDCode;
                            objEAuthMaster.LVL_ID = model.LevelID;
                            objEAuthMaster.USERID = model.UserID;
                            string IpAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            objEAuthMaster.IPADD = IpAddress;
                            if (model.Operation.Equals("A"))
                            {

                                dbContext.ACC_EAUTH_MASTER.Add(objEAuthMaster);
                                dbContext.SaveChanges();
                                //Physical fin year
                                int decYear = 0;
                                if (model.BILL_MONTH <=3)
                                {
                                    decYear = model.BILL_YEAR - 1;
                                }
                                else
                                {

                                    decYear = model.BILL_YEAR; 
                                }
                                
                                ACC_VOUCHER_NUMBER_MASTER objVoucherMaster = new ACC_VOUCHER_NUMBER_MASTER();
                                objVoucherMaster = dbContext.ACC_VOUCHER_NUMBER_MASTER.Where(x => x.ADMIN_ND_CODE == model.AdminNDCode && x.FUND_TYPE == model.FundType && x.BILL_TYPE == "E" && x.FISCAL_YEAR == decYear).FirstOrDefault();
                                if (objVoucherMaster.SLNO == 0)
                                {

                                    objVoucherMaster.SLNO = 1;
                                }
                                else
                                {

                                    objVoucherMaster.SLNO = objVoucherMaster.SLNO + 1;
                                }
                                dbContext.Entry(objVoucherMaster).State = System.Data.Entity.EntityState.Modified;
                                dbContext.SaveChanges();
                                scope.Complete();
                            }
                            returnModel.Auth_Id = objEAuthMaster.EAUTH_ID;
                            returnModel.status = true;
                        }
                        

                    }
                 

                    return returnModel;

                }
            }


            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorizationDAL.AddEAuthorizationMasterDetails()");
                return null;
            }

        }
        #endregion

        #region Delete EAuthorization Master
        /// <summary>
        /// Delete EAuthorization Master Record
        /// </summary>
        /// <param name="EAuthID"></param>
        /// <returns></returns>
        public EAuthorizationRequestDetailsModel DeleteEAuthorizationMaster(Int64 EAuthID)
        {
            EAuthorizationRequestDetailsModel model = null;
            dbContext = new PMGSYEntities();
            ACC_EAUTH_MASTER con = null;

            try
            {
                using (var scope = new TransactionScope())
                {
                    model = new EAuthorizationRequestDetailsModel();
                    con = new ACC_EAUTH_MASTER();
                    con = dbContext.ACC_EAUTH_MASTER.Where(p => p.EAUTH_ID == EAuthID).FirstOrDefault();
                    if (con != null)
                    {
                        if (con.EAUTH_STATUS == "Y")
                        {
                            model.ErrMessage = "Eauthorization Details is Finalized So Details can't be Deleted";
                            model.status = false;
                        }
                        else
                        {
                            ACC_EAUTH_DETAILS accEAuthRequestDetails = dbContext.ACC_EAUTH_DETAILS.Where(m => m.EAUTH_ID == EAuthID).FirstOrDefault();

                            if (accEAuthRequestDetails != null)
                            {
                                //accEAuthRequestDetails.USERID = PMGSYSession.Current.UserId;
                                //accEAuthRequestDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                                dbContext.Entry(accEAuthRequestDetails).State = EntityState.Unchanged;
                                dbContext.Entry(con).State = EntityState.Unchanged;
                                dbContext.SaveChanges();
                            }


                            int count = dbContext.Database.ExecuteSqlCommand("DELETE FROM [omms].ACC_EAUTH_DETAILS Where EAUTH_ID = {0}", EAuthID);

                            dbContext.Database.ExecuteSqlCommand("DELETE FROM [omms].ACC_EAUTH_AUTHAMT_DETAILS Where EAUTH_ID = {0}", EAuthID);

                            var context = ((IObjectContextAdapter)dbContext).ObjectContext;
                            var refreshableObjects = (from entry in context.ObjectStateManager.GetObjectStateEntries(
                                                                       EntityState.Added
                                                                       | EntityState.Deleted
                                                                       | System.Data.Entity.EntityState.Modified
                                                                       | EntityState.Unchanged)
                                                      where entry.EntityKey != null
                                                      select entry.Entity).ToList();

                            context.Refresh(RefreshMode.StoreWins, refreshableObjects);



                            dbContext.ACC_EAUTH_MASTER.Remove(con);



                            //context.SaveChanges();

                            dbContext.SaveChanges();

                            scope.Complete();
                            model.status = true;
                        }
                    }

                    else
                    {
                        model.ErrMessage = "Error while deleting EAuthorization request Details";
                        model.status = false;
                    }
                    return model;

                }


            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorizationDAL.DeleteEAuthorizationMaster()");
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
        #endregion

        #region Add EAuthorization Details
        /// <summary>
        /// Edit Payment Transaction Details Form
        /// </summary>
        /// <param name="model"></param>
        /// <param name="operationType"></param>
        /// <param name="Bill_id"></param>
        /// <param name="AddorEdit"></param>
        /// <param name="tranNumber"></param>
        /// 
        /// <returns></returns>
        public Int32 AddPaymentTransactionDetails(EAuthorizationRequestDetailsModel model, string operationType, Int64 Bill_id, String AddorEdit, Int16 tranNumber)
        {
            ACC_EAUTH_AUTHAMT_DETAILS objAuthAmount = null;
            try
            {
                dbContext = new PMGSYEntities();
                ACC_EAUTH_DETAILS ModelToAdd = null;
                objAuthAmount = new ACC_EAUTH_AUTHAMT_DETAILS();
                Int16 maxTxnNo = 0;
                ACC_EAUTH_DETAILS PaymentTransaction = new ACC_EAUTH_DETAILS();

                if (operationType.Equals("T"))
                {
                    using (var scope = new TransactionScope())
                    {
                        if (AddorEdit.Equals("A"))
                        {
                            if (dbContext.ACC_EAUTH_DETAILS.Where(t => t.EAUTH_ID == Bill_id).Any())
                            {
                                maxTxnNo = dbContext.ACC_EAUTH_DETAILS.Where(t => t.EAUTH_ID == Bill_id).Max(c => c.EAUTH_TXN_NO);
                            }

                            maxTxnNo = Convert.ToInt16(maxTxnNo + 1);
                        }

                        ModelToAdd = new ACC_EAUTH_DETAILS();
                        ModelToAdd.EAUTH_ID = Convert.ToInt32(Bill_id);
                        ModelToAdd.EAUTH_TXN_NO = maxTxnNo;
                        ModelToAdd.AMOUNT = model.AMOUNT_Q.HasValue ? model.AMOUNT_Q.Value : 0;
                        ModelToAdd.MAST_CON_ID = Convert.ToInt32(model.MAST_CON_ID_C);
                        ModelToAdd.IMS_AGREEMENT_CODE = Convert.ToInt32(model.IMS_AGREEMENT_CODE_C);
                        ModelToAdd.APPROVAL_STAUS = "N";
                        ModelToAdd.USERID = PMGSYSession.Current.UserId;
                        ModelToAdd.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        ModelToAdd.IMS_PACKAGE_ID = model.IMS_SANCTION_PACKAGE;
                        ModelToAdd.TEND_AGREEMENT_AMOUNT = model.AGREEMENT_AMOUNT;
                        ModelToAdd.ALREADY_AUTH_AMOUNT = model.ALREADY_AUTHORISED_AMOUNT;
                        if (AddorEdit.Equals("A"))
                        {
                            dbContext.ACC_EAUTH_DETAILS.Add(ModelToAdd);
                        }
                        dbContext.SaveChanges();
                        scope.Complete();
                    }
                    return ModelToAdd.EAUTH_ID;
                }
                else
                {
                    return 0;
                }


            }

            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorizationDAL.AddPaymentTransactionDetails()");
                throw new Exception("Error while adding/editing transaction entry.");

            }
            finally
            {
                dbContext.Dispose();
            }

        }
        #endregion

        #region Update EAuthorization Details
        /// <summary>
        /// Update EAuthorization Details
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public EAuthorizationRequestDetailsModel UpdateEAuthorizationDetails(EAuthorizationRequestDetailsModel model)
        {
            ACC_EAUTH_DETAILS tlbmodel = null;
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    dbContext = new PMGSYEntities();
                    tlbmodel = new ACC_EAUTH_DETAILS();
                    tlbmodel = dbContext.ACC_EAUTH_DETAILS.Where(x => x.EAUTH_ID == model.EAUTH_ID && x.EAUTH_TXN_NO == model.TXN_NO).FirstOrDefault();
                    if (tlbmodel == null)
                    {
                        model.ErrMessage = "Error in Updating,Please try again";
                        model.status = false;
                        return model;
                    }
                    else
                    {

                        if (tlbmodel.IMS_PACKAGE_ID == model.IMS_SANCTION_PACKAGE)  //its ok
                        {

                        }
                        else
                        {
                            Int32 AggrementCode = Convert.ToInt32(model.IMS_AGREEMENT_CODE_C);
                            if (dbContext.ACC_EAUTH_DETAILS.Where(x => x.IMS_PACKAGE_ID == model.IMS_SANCTION_PACKAGE && x.IMS_AGREEMENT_CODE == AggrementCode && x.EAUTH_ID == model.EAUTH_ID).Any())
                            {
                                model.ErrMessage = "Details already entered against selected Package,Please select Another Package";
                                model.status = false;
                                return model;
                            }
                        }

                        decimal TotalAgreementAmt = Convert.ToDecimal(tlbmodel.TEND_AGREEMENT_AMOUNT); //A (In LAkhs )
                        decimal alreadyAuthAmt = Convert.ToDecimal(tlbmodel.ALREADY_AUTH_AMOUNT); //B
                        decimal RequestAmount = Convert.ToDecimal(model.AMOUNT_Q);

                        decimal TotalCompareAmt = RequestAmount + alreadyAuthAmt;
                        if (TotalAgreementAmt >= TotalCompareAmt)   //Total Aggrement Amount Should be Greater than or Equal to Agreemment Amt AND Request Amt
                        {
                            tlbmodel.AMOUNT = Convert.ToDecimal(model.AMOUNT_Q);
                        }
                        else
                        {
                            model.ErrMessage = "Sum of Authorization Request Amount and Amount Already Authorized Should be less than Agreement Amount";
                            model.status = false;
                            return model;
                        }

                        tlbmodel.MAST_CON_ID = Convert.ToInt32(model.MAST_CON_ID_C);
                        tlbmodel.IMS_AGREEMENT_CODE = Convert.ToInt32(model.IMS_AGREEMENT_CODE_C);
                        tlbmodel.IMS_PACKAGE_ID = model.IMS_SANCTION_PACKAGE;
                        if (tlbmodel != null)
                        {
                            dbContext.Entry(tlbmodel).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                            scope.Complete();
                            model.status = true;
                            return model;
                        }
                        else
                        {
                            model.ErrMessage = "Error in Updating,Please try again";
                            model.status = false;
                            return model;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorizationDAL.UpdateEAuthorizationDetails()");
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

        #endregion

        #region Delete EAuthorization Details
        /// <summary>
        /// Delete EAuthorization Details Against EAuthID and TxNo
        /// </summary>
        /// <param name="EAuthID"></param>
        /// <returns></returns>
        public EAuthorizationRequestDetailsModel DeleteEAuthorizationDetails(Int64 EAuthID, int TxNo)
        {
            ACC_EAUTH_DETAILS tlbmodel = null;
            EAuthorizationRequestDetailsModel model = null;
            ACC_EAUTH_AUTHAMT_DETAILS objAuthAmt = null;
            try
            {
                dbContext = new PMGSYEntities();
                tlbmodel = new ACC_EAUTH_DETAILS();
                model = new EAuthorizationRequestDetailsModel();
                objAuthAmt = new ACC_EAUTH_AUTHAMT_DETAILS();
                objAuthAmt = dbContext.ACC_EAUTH_AUTHAMT_DETAILS.Where(x => x.EAUTH_ID == EAuthID && x.EAUTH_TXN_NO == TxNo).FirstOrDefault();
                if (objAuthAmt != null)
                {
                    dbContext.ACC_EAUTH_AUTHAMT_DETAILS.Remove(objAuthAmt);
                }
                else
                {

                    model.ErrMessage = "Error in Deleting EAuthorization Details,Please try again";
                    model.status = false;

                }
                tlbmodel = dbContext.ACC_EAUTH_DETAILS.Where(x => x.EAUTH_ID == EAuthID && x.EAUTH_TXN_NO == TxNo).FirstOrDefault();
                if (tlbmodel != null)
                {
                    dbContext.ACC_EAUTH_DETAILS.Remove(tlbmodel);
                    dbContext.SaveChanges();
                    model.status = true;

                }
                else
                {
                    model.ErrMessage = "Error in Deleting EAuthorization Details,Please try again";
                    model.status = false;
                }

                int Count = dbContext.ACC_EAUTH_DETAILS.Where(x => x.EAUTH_ID == EAuthID).Count();
                model.count = Count;
                return model;

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorizationDAL.DeleteEAuthorizationDetails()");
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

        #endregion

        #region Finalize EAuthorization Details
        /// <summary>
        /// Finalize EAuthorization Details based on EAuthID
        /// </summary>
        /// <param name="EAuthID"></param>
        /// <returns></returns>
        public EAuthorizationRequestDetailsModel FinalizeEAuthorizationDetails(Int64 EAuthID)
        {
            EAuthorizationRequestDetailsModel model = null;
            ACC_EAUTH_MASTER tlbmodel = null;
            ACC_EAUTH_DETAILS objDetails = null;
            try
            {
                dbContext = new PMGSYEntities();
                model = new EAuthorizationRequestDetailsModel();
                tlbmodel = new ACC_EAUTH_MASTER();
                objDetails = new ACC_EAUTH_DETAILS();

                if (dbContext.ACC_EAUTH_DETAILS.Where(x => x.EAUTH_ID == EAuthID).Any())
                {
                    decimal TotalAmt = dbContext.ACC_EAUTH_DETAILS.Where(x => x.EAUTH_ID == EAuthID).Sum(x => x.AMOUNT);
                    if (TotalAmt == 0)
                    {

                        model.ErrMessage = "EAuthorization Details cannot be finalized,Request Amount should be greater than 0";
                        model.status = false;
                        return model;
                    }
                    else
                    {
                        tlbmodel = dbContext.ACC_EAUTH_MASTER.Where(x => x.EAUTH_ID == EAuthID).FirstOrDefault();
                        if (tlbmodel != null)
                        {
                            if (tlbmodel.EAUTH_STATUS == "N")
                            {

                                tlbmodel.EAUTH_STATUS = "Y";
                                tlbmodel.TOTAL_AUTH_AMOUNT_REQ = TotalAmt;
                                dbContext.Entry(tlbmodel).State = System.Data.Entity.EntityState.Modified;
                                if (tlbmodel != null)
                                {
                                    dbContext.SaveChanges();
                                    model.status = true;
                                }
                                else
                                {
                                    model.ErrMessage = "Error Occur while Finalizing EAuthorization Details,Please try Again";
                                    model.status = false;
                                }
                                return model;
                            }
                            else if (tlbmodel.EAUTH_STATUS == "Y")
                            {
                                model.ErrMessage = "EAuthorization Details is Already Finalized";
                                model.status = false;
                            }
                            else if (tlbmodel.EAUTH_STATUS == "A")
                            {
                                model.ErrMessage = "EAuthorization Details is Approved by SRRDA";
                                model.status = false;
                            }
                            else if (tlbmodel.EAUTH_STATUS == "R")
                            {
                                model.ErrMessage = "EAuthorization Details is Rejected by SRRDA";
                                model.status = false;
                            }
                            else if (tlbmodel.EAUTH_STATUS == "E")
                            {
                                model.ErrMessage = "Epayment Invoice is generated for EAuthorization Details";
                                model.status = false;
                            }

                            return model;
                        }
                        else
                        {
                            model.ErrMessage = "Error in Finalizing EAuthorization Details,Please try Again";
                            model.status = false;
                            return model;

                        }
                    }

                }
                else
                {
                    model.ErrMessage = "EAuthorization Details cannot be finalized,Request Amount should be greater than 0";
                    model.status = false;
                    return model;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorizationDAL.FinalizeEAuthorizationDetails()");
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
        #endregion

        #region Add New Authorization Entry
        public bool AddNewAuthorizationEntry(EAuthorizationAmountRequestModel model)
        {
            PMGSYEntities dbcontext = null;
            ACC_EAUTH_AUTHAMT_DETAILS objAddmodel = null;
            Int16 maxTxnNo = 0;
            try
            {
                objAddmodel = new ACC_EAUTH_AUTHAMT_DETAILS();
                dbcontext = new PMGSYEntities();
                if (dbcontext.ACC_EAUTH_AUTHAMT_DETAILS.Where(t => t.EAUTH_ID == model.EAuth_ID).Any())
                {
                    maxTxnNo = dbcontext.ACC_EAUTH_AUTHAMT_DETAILS.Where(t => t.EAUTH_ID == model.EAuth_ID).Max(c => c.EAUTH_TXN_NO);
                }
                maxTxnNo = Convert.ToInt16(maxTxnNo + 1);

                objAddmodel.DETAIL_ID = (dbcontext.ACC_EAUTH_AUTHAMT_DETAILS.Any() ? dbcontext.ACC_EAUTH_AUTHAMT_DETAILS.Max(X => X.DETAIL_ID) : 0) + 1;
                objAddmodel.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                objAddmodel.MAST_CON_ID = model.MAST_CONT_ID;
                objAddmodel.IMS_AGREEMENT_CODE = model.AGREEMENT_CODE;
                objAddmodel.IMS_PACKAGE_ID = model.IMS_SANCTION_PACKAGE;
                objAddmodel.AMOUNT_AUTHORIZED = Convert.ToDecimal(model.AUTHORIZATION_AMOUNT);
                objAddmodel.AUTHORIZATION_DATE = dbcontext.ACC_EAUTH_MASTER.Where(x => x.EAUTH_ID == model.EAuth_ID).Select(x => x.EAUTH_DATE).FirstOrDefault();
                objAddmodel.EAUTH_ID = Convert.ToInt32(model.EAuth_ID);
                objAddmodel.EAUTH_TXN_NO = maxTxnNo;
                objAddmodel.FUND_TYPE = PMGSYSession.Current.FundType;
                objAddmodel.ISACTIVE = true;
                objAddmodel.ENTRYDATE = DateTime.Now;
                objAddmodel.USERID = PMGSYSession.Current.UserId;
                objAddmodel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                dbcontext.ACC_EAUTH_AUTHAMT_DETAILS.Add(objAddmodel);
                dbcontext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {

                ErrorLog.LogError(ex, "EAuthorizationDAL.GetAddEAuthorizationLinkView()");
                return false;
            }
            finally
            {
                if (dbcontext != null)
                {
                    dbcontext.Dispose();
                }

            }
        }
        #endregion
        #endregion

        #region Proceed/Save EAuth Details SRRDA
        #region SRRDA Proceed Approve Reject Details
        /// <summary>
        /// Save Approval/Rejected Status
        /// </summary>
        /// <param name="ApproveArr"></param>
        /// <param name="RejectArr"></param>
        /// <param name="EAuthID"></param>
        /// <returns></returns>
        public EAuthorizationRequestDetailsModel ProceedForApproveRejectDetails(string ApproveArr, string RejectArr, Int64 EAuthID)
        {
            EAuthorizationRequestDetailsModel model = null;
            ACC_EAUTH_DETAILS objdetails = null;
            ACC_EAUTH_MASTER objMaster = null;
            try
            {

                model = new EAuthorizationRequestDetailsModel();
                dbContext = new PMGSYEntities();
                objdetails = new ACC_EAUTH_DETAILS();
                objMaster = new ACC_EAUTH_MASTER();
                Dictionary<int, string> dict = new Dictionary<int, string>();
                if (EAuthID == 0)
                {
                    model.status = false;
                    model.ErrMessage = "Error in Updating Status,please try again";
                    return model;
                }
                else
                {

                    objMaster = dbContext.ACC_EAUTH_MASTER.Where(x => x.EAUTH_ID == EAuthID).FirstOrDefault();

                    if (dbContext.ACC_EAUTH_DETAILS.Where(x => x.EAUTH_ID == EAuthID && x.APPROVAL_STAUS == "A").Any())
                    {

                        int Count = dbContext.ACC_EAUTH_DETAILS.Where(x => x.EAUTH_ID == EAuthID).Count();

                        model.status = true;
                        model.ErrMessage = "e-Authorization Details Already Saved,Please Proceed to Send Mail";
                        model.EAUTH_ID = EAuthID;
                        List<ACC_EAUTH_DETAILS> lst = new List<ACC_EAUTH_DETAILS>();
                        objMaster = dbContext.ACC_EAUTH_MASTER.Where(x => x.EAUTH_ID == EAuthID).FirstOrDefault();
                        model.EAUTH_NO = objMaster.EAUTH_NO;
                        return model;
                    }
                    else if (dbContext.ACC_EAUTH_DETAILS.Where(x => x.EAUTH_ID == EAuthID && x.APPROVAL_STAUS == "R").Any())
                    {

                        int Count = dbContext.ACC_EAUTH_DETAILS.Where(x => x.EAUTH_ID == EAuthID).Count();

                        model.status = true;
                        model.ErrMessage = "e-Authorization Details Already Saved,Please Proceed to Send Mail";
                        model.EAUTH_ID = EAuthID;
                        List<ACC_EAUTH_DETAILS> lst = new List<ACC_EAUTH_DETAILS>();
                        objMaster = dbContext.ACC_EAUTH_MASTER.Where(x => x.EAUTH_ID == EAuthID).FirstOrDefault();
                        model.EAUTH_NO = objMaster.EAUTH_NO;
                        return model;
                    }

                    string[] Approvalseperator = new string[] { "$%*~_~*%$" };   //Rejection Array Split by $%*~_~*%$
                    String[] ApprovalArray = ApproveArr.Split(Approvalseperator, StringSplitOptions.None);


                    foreach (var itemApprove in ApprovalArray)
                    {

                        string[] ApprovalIDseperator = new string[] { "$*_~-~_*$" };  //For Spliting ID AND REMARK
                        String[] ApproveArray1 = itemApprove.Split(ApprovalIDseperator, StringSplitOptions.None);

                        //String[] ApproveArray = itemApprove.Split('$');
                        if (!string.IsNullOrEmpty(ApproveArr))
                        {
                            int r = Convert.ToInt32(ApproveArray1[0]);
                            string str = string.Empty;
                            dict.Add(r, str);

                        }



                    }


                    string[] Rejectseperator = new string[] { "$%*~_~*%$" };   //Rejection Array Split by $%*~_~*%$
                    String[] RejectionArray = RejectArr.Split(Rejectseperator, StringSplitOptions.None);

                    foreach (var item in RejectionArray)
                    {
                        if (!String.IsNullOrEmpty(RejectArr))
                        {

                            string[] RejectIDseperator = new string[] { "$*_~-~_*$" };  //For Spliting ID AND REMARK
                            String[] RejectionArray1 = item.Split(RejectIDseperator, StringSplitOptions.None);
                            int r = Convert.ToInt32(RejectionArray1[0]);

                            if (RejectionArray1[1].Length >= 1)
                            {

                                if (RejectionArray1[1].Length > 255)
                                {
                                    model.status = false;
                                    model.ErrMessage = "Only 255 character are allowed in Remark ";
                                    return model;

                                }


                                //Regular Expression to handle remark as text
                                var pattern = @"([a-zA-Z.&])";
                                Regex rgx = new Regex(pattern);
                                Match match = rgx.Match(RejectionArray1[1]);
                                if (match.Success)
                                {

                                }
                                else
                                {
                                    model.status = false;
                                    model.ErrMessage = "Special Symbols are not allowed in Remark";
                                    return model;
                                }

                                //If Regular Expression Fails

                                var values = new[] { "~", "`", "!", "@", "#", "$", "%", "^", "*", "(", ")", "-", "_", "+", "=", "{", "}", "[", "]", "|", ";", ":", "'", "<", ">", ",", "/", "?" };
                                if (values.Any(RejectionArray1[1].Contains))
                                {
                                    model.status = false;
                                    model.ErrMessage = "Special Symbols are not allowed in Remark";
                                    return model;

                                }







                                // Match match = Regex.Match(RejectionArray1[1], regex, RegexOptions.IgnoreCase);




                            }



                            dict.Add(r, RejectionArray1[1]);




                        }



                    }

                    int counter = 0;
                    int saveChangeValue = 0;
                    int CountRows = 0;

                    CountRows = dbContext.ACC_EAUTH_DETAILS.Where(x => x.EAUTH_ID == EAuthID).Count();
                    if (CountRows == 0)
                    {
                        model.status = false;
                        model.ErrMessage = "Error while getting Saving EAuthorization Details";
                    }
                    else
                    {
                        foreach (KeyValuePair<int, string> entry in dict)
                        {
                            int TxNo = entry.Key;
                            string Remark = entry.Value;





                            if (dbContext.ACC_EAUTH_DETAILS.Where(x => x.EAUTH_ID == EAuthID && x.EAUTH_TXN_NO == TxNo).Any())
                            {
                                try
                                {
                                    using (TransactionScope scope = new TransactionScope())
                                    {
                                        objdetails = dbContext.ACC_EAUTH_DETAILS.Where(x => x.EAUTH_ID == EAuthID && x.EAUTH_TXN_NO == TxNo).FirstOrDefault();
                                        if (objdetails != null)
                                        {
                                            if (String.IsNullOrEmpty(Remark)) //Remark is Null means Approve
                                            {

                                                objdetails.APPROVAL_STAUS = "A";
                                                objdetails.APPROVAL_REMARKS = "Approved";
                                            }
                                            else  //Remark is not Null means Rejected
                                            {
                                                objdetails.APPROVAL_STAUS = "R";
                                                objdetails.APPROVAL_REMARKS = Remark;
                                            }

                                            if (objdetails != null)
                                            {

                                                dbContext.Entry(objdetails).State = System.Data.Entity.EntityState.Modified;
                                                saveChangeValue = dbContext.SaveChanges();
                                                scope.Complete();

                                            }//End of objdetails

                                            counter = counter + saveChangeValue;

                                        }//End of objdetails
                                    }//End of Transaction Scope
                                }
                                catch (Exception)
                                {

                                    throw new Exception("Error while getting Saving EAuthorization Details");

                                }
                            }
                        }//End of Dict For Each



                        if (CountRows == counter)  //check whether all Columns are SuccessFully Updated
                        {
                            model.status = true;
                            model.ErrMessage = "e-Authorization Details  Saved Successfully";
                            model.EAUTH_ID = EAuthID;
                            List<ACC_EAUTH_DETAILS> lst = new List<ACC_EAUTH_DETAILS>();
                            objMaster = dbContext.ACC_EAUTH_MASTER.Where(x => x.EAUTH_ID == EAuthID).FirstOrDefault();
                            model.EAUTH_NO = objMaster.EAUTH_NO;

                            if (objMaster != null)
                            {

                                int count = dbContext.ACC_EAUTH_DETAILS.Where(x => x.EAUTH_ID == EAuthID).Count();

                                //Do not Update Status in Master After Click on proceed....Status in EAuth_MASTER Should be change After Sending Mail Discuss with Mam
                                #region Update Approval Status in Master

                                //Update Status Rejected if All Are Rejected..in other Case Update Status After Sending Mail


                                int RejectedCount = dbContext.ACC_EAUTH_DETAILS.Where(x => x.APPROVAL_STAUS == "R" && x.EAUTH_ID == EAuthID).Count();
                                if (RejectedCount > 0)
                                {
                                    if (count == RejectedCount)
                                    {
                                        objMaster.EAUTH_STATUS = "R";
                                        objMaster.APPOVAL_DATE_SRRDA = DateTime.Now;

                                    }
                                }

                                //To Hide PDF in Case if All are Rejected...we need Status..R on js..
                                //if Total count == Rejected Count..means All are Rejected...retrun Status as Rejected

                                if (count == RejectedCount)
                                {
                                    model.ApprovalStatus = "R";
                                }

                                dbContext.Entry(objMaster).State = System.Data.Entity.EntityState.Modified;
                                dbContext.SaveChanges();

                                //int arrroval = dbContext.ACC_EAUTH_DETAILS.Where(x => x.APPROVAL_STAUS == "A" && x.EAUTH_ID == EAuthID).Count();
                                //if (arrroval == 0)
                                //{

                                //    objMaster.EAUTH_STATUS = "R";


                                //}
                                //else if (count == arrroval)
                                //{
                                //    objMaster.EAUTH_STATUS = "A";

                                //}
                                //else
                                //{
                                //    objMaster.EAUTH_STATUS = "P";

                                //}

                                //dbContext.Entry(objMaster).State = System.Data.Entity.EntityState.Modified;
                                //dbContext.SaveChanges();
                                #endregion

                                #region Update Total_EAuth_Approved in Master


                                if (!String.IsNullOrEmpty(RejectArr) && String.IsNullOrEmpty(ApproveArr))     //if Rejected Array is not null...i.e ..APPROVAL NULL.....DECIMAL VALUE OF AMOUNT AUTHORISED =0
                                {
                                    //Accepted Value is Zero
                                    objMaster.TOTAL_AUTH_APPROVED = 0;
                                }
                                else if ((String.IsNullOrEmpty(RejectArr)) && !String.IsNullOrEmpty(ApproveArr))  //if Rejected array is null.....ie All Accepted
                                {
                                    decimal TotalApproved = dbContext.ACC_EAUTH_DETAILS.Where(x => x.EAUTH_ID == EAuthID && x.APPROVAL_STAUS == "A").Sum(x => x.AMOUNT);
                                    objMaster.TOTAL_AUTH_APPROVED = TotalApproved;
                                }
                                else if ((!String.IsNullOrEmpty(RejectArr)) && !String.IsNullOrEmpty(ApproveArr))  //Both contain some value..
                                {
                                    decimal TotalApproved = dbContext.ACC_EAUTH_DETAILS.Where(x => x.EAUTH_ID == EAuthID && x.APPROVAL_STAUS == "A").Sum(x => x.AMOUNT);
                                    objMaster.TOTAL_AUTH_APPROVED = TotalApproved;
                                }

                                else if ((String.IsNullOrEmpty(RejectArr)) && String.IsNullOrEmpty(ApproveArr))  //Both contain some value..
                                {
                                    //decimal TotalApproved = dbContext.ACC_EAUTH_DETAILS.Where(x => x.EAUTH_ID == EAuthID && x.APPROVAL_STAUS == "A").Sum(x => x.AMOUNT);
                                    objMaster.TOTAL_AUTH_APPROVED = 0;
                                }



                                dbContext.Entry(objMaster).State = System.Data.Entity.EntityState.Modified;
                                dbContext.SaveChanges();

                                #endregion




                            }


                        }
                        else   //All Columns are not SuccessFully Updated
                        {
                            model.status = false;
                            model.ErrMessage = "Error occur while Saving,Please try Again";
                        }

                    }


                    return model;


                }

            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                string rs = "";
                foreach (var eve in e.EntityValidationErrors)
                {
                    rs = string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    Console.WriteLine(rs);

                    foreach (var ve in eve.ValidationErrors)
                    {
                        rs += "<br />" + string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw new Exception(rs);
            }

            catch (DbUpdateException ex)
            {
                throw ex;
            }


            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorizationDAL.ProceedForApproveRejectDetails()");
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
        #endregion

        #region SRRDA Save Notification Details After Sending Mail
        /// <summary>
        /// function to Save Notification After Sending Mail
        /// </summary>
        /// <param name="EncryptedEAuthID"></param>
        /// <returns></returns>
        public EAuthorizationRequestDetailsModel SaveNotificationDetailsAfterSendingMail(string EncryptedEAuthID)
        {
            ACC_NOTIFICATION_DETAILS objNotificationDetails = null;
            ACC_EAUTH_MASTER objMaster = null;
            Int64 EAuthID = 0;
            string parameter = string.Empty;
            string hash = string.Empty;
            string key = string.Empty;
            EAuthorizationRequestDetailsModel model = null;
            PMGSYEntities dbContext = null;

            try
            {

                objNotificationDetails = new ACC_NOTIFICATION_DETAILS();
                objMaster = new ACC_EAUTH_MASTER();
                model = new EAuthorizationRequestDetailsModel();
                dbContext = new PMGSYEntities();

                if (!String.IsNullOrEmpty(EncryptedEAuthID))
                {
                    String[] splitID = EncryptedEAuthID.Split('/');
                    parameter = splitID[0];
                    hash = splitID[1];
                    key = splitID[2];
                }
                else
                {
                    return model;
                }


                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        EAuthID = Convert.ToInt64(urlSplitParams[0]);

                    }
                }
                else
                {
                    return model;
                }


                using (TransactionScope scope = new TransactionScope())
                {
                    objNotificationDetails = new ACC_NOTIFICATION_DETAILS();
                    objMaster = dbContext.ACC_EAUTH_MASTER.Where(x => x.EAUTH_ID == EAuthID).FirstOrDefault();
                    objNotificationDetails.DETAIL_ID = (dbContext.ACC_NOTIFICATION_DETAILS.Any() ? dbContext.ACC_NOTIFICATION_DETAILS.Max(X => X.DETAIL_ID) : 0) + 1;
                    objNotificationDetails.INITIATION_BILL_ID = EAuthID;
                    objNotificationDetails.NOTIFICATION_ID = 15;
                    objNotificationDetails.INITIATOR_ADMIN_ND_CODE = objMaster.ADMIN_ND_CODE;  //EO Admin ND Code
                    objNotificationDetails.RECEIVER_ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;  //PIU Admin ND Code
                    string DPIUName = dbContext.ADMIN_DEPARTMENT.Where(x => x.ADMIN_ND_CODE == objMaster.ADMIN_ND_CODE).Select(x => x.ADMIN_ND_NAME).FirstOrDefault();
                    string str = "e-Authorization issued by SRRDA to DPIU of " + DPIUName + " " + "of Rs." + objMaster.TOTAL_AUTH_APPROVED + " " + "Lakhs." + "( EAuthNo - " + objMaster.EAUTH_NO + ")";
                    objNotificationDetails.REQUEST_NARRATION = str;
                    objNotificationDetails.RECEIVER_BILL_ID = null;
                    objNotificationDetails.FUND_TYPE = PMGSYSession.Current.FundType;
                    objNotificationDetails.DT_NOTIFICATION = DateTime.Now;
                    objNotificationDetails.USERID = PMGSYSession.Current.UserId;
                    objNotificationDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    dbContext.ACC_NOTIFICATION_DETAILS.Add(objNotificationDetails);
                    dbContext.SaveChanges();
                    scope.Complete();
                    model.status = true;

                }

                return model;
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                string rs = "";
                foreach (var eve in e.EntityValidationErrors)
                {
                    rs = string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    Console.WriteLine(rs);

                    foreach (var ve in eve.ValidationErrors)
                    {
                        rs += "<br />" + string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw new Exception(rs);
            }

            catch (DbUpdateException ex)
            {
                throw ex;
            }


            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorizationDAL.SaveNotificationDetailsAfterSendingMail()");
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
        #endregion
        #endregion

        #region Listing Grid:PIU

        #region List EAuthorization Master 1st View
        /// <summary>
        /// This Method Returns EAuthorization Master List
        /// </summary>
        /// <param name="objFilter"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array EAuthorizationRequestListView(EAuthorizationFilterModel objFilter, out long totalRecords)
        {
            CommonFunctions commomFuncObj = null;

            try
            {
                dbContext = new PMGSYEntities();
                commomFuncObj = new CommonFunctions();
                DateTime fromDate = DateTime.Now;
                DateTime toDate = DateTime.Now;
                List<ACC_EAUTH_MASTER> lstEAuthMaster = null;
                if (objFilter.FromDate != String.Empty && objFilter.FromDate != null)
                {
                    fromDate = commomFuncObj.GetStringToDateTime(objFilter.FromDate);
                }

                if (objFilter.ToDate != String.Empty && objFilter.ToDate != null)
                {
                    toDate = commomFuncObj.GetStringToDateTime(objFilter.ToDate);
                }

                if (objFilter.FilterMode.Equals("view"))
                {
                    lstEAuthMaster = dbContext.ACC_EAUTH_MASTER.
                     Where(m => m.ADMIN_ND_CODE == objFilter.AdminNdCode).
                     Where(m => m.LVL_ID == objFilter.LevelId).
                     Where(m => m.FUND_TYPE == objFilter.FundType).
                     Where(m => m.EAUTH_MONTH == objFilter.Month).
                     Where(m => m.EAUTH_YEAR == objFilter.Year).OrderByDescending(x => x.EAUTH_DATE).ToList<ACC_EAUTH_MASTER>();
                }
                totalRecords = lstEAuthMaster.Count();
                lstEAuthMaster = lstEAuthMaster.OrderByDescending(x => x.EAUTH_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).ToList<ACC_EAUTH_MASTER>();
                var result = lstEAuthMaster.Select(m => new
                {
                    m.EAUTH_ID,
                    m.EAUTH_NO,
                    m.EAUTH_MONTH,
                    m.EAUTH_YEAR,
                    m.EAUTH_DATE,
                    m.FUND_TYPE,
                    m.ADMIN_ND_CODE,
                    m.LVL_ID,
                    m.EAUTH_STATUS,

                }).ToArray();
                return result.Select(item => new
                {
                    id = URLEncrypt.EncryptParameters(new string[] { item.EAUTH_ID.ToString().Trim() }),
                    cell = new[] {                         
                                                item.EAUTH_NO.ToString(),
                                                commomFuncObj.GetDateTimeToString(item.EAUTH_DATE),
                                                GetAuthStatusInWords(item.EAUTH_STATUS ==null ? " " : item.EAUTH_STATUS),
                                                GetCalculatedEAuthorizationRequestAmount(item.EAUTH_ID),
                                                GetTotalAmountApproved(item.EAUTH_ID),
                                                GetApprovalDate(item.EAUTH_ID),
                                                GetMasterEditSymbol(item.EAUTH_ID),
                                                GetMasterDeleteSymbol(item.EAUTH_ID),
                                                GetMasterFinalizeSymbol(item.EAUTH_ID),
                                }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorizationDAL.EAuthorizationRequestListView()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }


        }
        #endregion

        #region Listing Aftering Adding Master Entry
        /// <summary>
        /// Listing Master Entry After Master Entry
        /// </summary>
        /// <param name="objFilter"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array ListEAuthorizationMasterDetails(EAuthorizationFilterModel objFilter, out long totalRecords)
        {
            CommonFunctions commomFuncObj = null;
            try
            {
                dbContext = new PMGSYEntities();
                commomFuncObj = new CommonFunctions();

                var query = (from m in dbContext.ACC_EAUTH_MASTER

                             where
                                    m.ADMIN_ND_CODE == objFilter.AdminNdCode
                                    && m.LVL_ID == objFilter.LevelId
                                    && m.FUND_TYPE == objFilter.FundType
                                    && m.EAUTH_ID == objFilter.BillId
                             select new
                             {
                                 m.EAUTH_ID,
                                 m.EAUTH_NO,
                                 m.EAUTH_MONTH,
                                 m.EAUTH_YEAR,
                                 m.EAUTH_DATE,
                                 m.EAUTH_STATUS,
                                 m.FUND_TYPE,
                                 m.ADMIN_ND_CODE,
                                 m.LVL_ID,
                             });

                totalRecords = query.Count();
                string LockEditSymbol = "<center><a href='#' class='ui-icon ui-icon-locked'>Edit EAuthorization Details</a></center>";
                var result = query.Select(m => new
                {

                    m.EAUTH_ID,
                    m.EAUTH_NO,
                    m.EAUTH_MONTH,
                    m.EAUTH_YEAR,
                    m.EAUTH_DATE,
                    m.EAUTH_STATUS,
                    m.FUND_TYPE,
                    m.ADMIN_ND_CODE,
                    m.LVL_ID,
                }).ToArray();
                return result.Select(item => new
                {
                    id = URLEncrypt.EncryptParameters(new string[] { item.EAUTH_ID.ToString().Trim() }),
                    cell = new[] {                         
                                    
                                        item.EAUTH_NO.ToString(),
                                        commomFuncObj.GetDateTimeToString(item.EAUTH_DATE),
                                        GetAuthStatusInWords(item.EAUTH_STATUS ==null ? " " : item.EAUTH_STATUS),
                                        GetCalculatedEAuthorizationRequestAmount(item.EAUTH_ID),
                                        GetTotalAmountApproved(item.EAUTH_ID),
                                        GetApprovalDate(item.EAUTH_ID),
                                        LockEditSymbol,
                                        GetMasterDeleteSymbol(item.EAUTH_ID),
                                        GetMasterFinalizeSymbol(item.EAUTH_ID),
                        }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorizationDAL.ListEAuthorizationMasterDetails()");
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

        #endregion

        #region Listing After Adding Details Entry
        /// <summary>
        ///List Details After Adding Detail Entry
        /// </summary>
        /// <param name="objFilter"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array GetPaymentDetailList(EAuthorizationFilterModel objFilter, out long totalRecords)
        {
            CommonFunctions commomFuncObj = null;
            try
            {
                dbContext = new PMGSYEntities();
                commomFuncObj = new CommonFunctions();
                var query = (from m in dbContext.ACC_EAUTH_DETAILS
                             where
                                     m.EAUTH_ID == objFilter.BillId
                             select new
                             {
                                 m.EAUTH_ID,
                                 m.EAUTH_TXN_NO,
                                 m.AMOUNT,
                                 m.MAST_CON_ID,
                                 m.IMS_AGREEMENT_CODE,
                             });

                totalRecords = query.Count();
                var result = query.Select(m => new
                {

                    m.EAUTH_ID,
                    m.EAUTH_TXN_NO,
                    m.AMOUNT,
                    m.MAST_CON_ID,
                    m.IMS_AGREEMENT_CODE,


                }).ToArray();

                return result.Select(item => new
                {
                    id = URLEncrypt.EncryptParameters(new string[] { item.EAUTH_ID.ToString().Trim() }),
                    cell = new[] {
                                        GetCompanyName(item.MAST_CON_ID==0?0:(item.MAST_CON_ID)),
                                        GetPayeeName(item.MAST_CON_ID==0?0:(item.MAST_CON_ID)),
                                        GetAgreementname(item.IMS_AGREEMENT_CODE==0?0:(item.IMS_AGREEMENT_CODE)),
                                        GetPackageNo(item.EAUTH_ID,item.EAUTH_TXN_NO),
                                        GetAgreementAmount(item.EAUTH_ID,item.EAUTH_TXN_NO),
                                        GetAlreadyAuthorisedAmount(item.EAUTH_ID,item.EAUTH_TXN_NO),
                                        item.AMOUNT.ToString(),
                                        GetEditSymbolForDetails(item.EAUTH_ID,item.EAUTH_TXN_NO),
                                        GetDeleteSymbolForDetails(item.EAUTH_ID,item.EAUTH_TXN_NO)
                                        }
                }).ToArray();

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorizationDAL.GetPaymentDetailList()");
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
        #endregion

        #endregion

        #region Listing Grid:SRRDA

        #region SRRDA Main View Data
        /// <summary>
        /// Returns SRRDA Data List 
        /// </summary>
        /// <param name="objFilter"></param>
        /// <param name="totalRecords"></param>
        /// 
        /// <returns></returns>
        public Array SRRDAeAuthorizationRequestListData(EAuthorizationFilterModel objFilter, out long totalRecords)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<ACC_EAUTH_MASTER> lstEAuthMaster = null;
                CommonFunctions commomFuncObj = new CommonFunctions();
                Int32 AdminNDCode = PMGSYSession.Current.AdminNdCode;

                if (!String.IsNullOrEmpty(objFilter.LoadStr)) //onLoad
                {
                    lstEAuthMaster = dbContext.ACC_EAUTH_MASTER.Where(x => x.ADMIN_DEPARTMENT.MAST_STATE_CODE == objFilter.StateCode && x.ADMIN_DEPARTMENT.MAST_PARENT_ND_CODE == objFilter.ParentNdCode).ToList().Where(v => v.EAUTH_STATUS == "Y" && v.FUND_TYPE == "P").OrderByDescending(x => x.EAUTH_DATE).ToList();
                }
                else //on view Click
                {
                    if ((objFilter.AdminNdCode == 0 || objFilter.AdminNdCode != 0) && (objFilter.Month != 0 || objFilter.Month == 0) && (objFilter.Year != 0 || objFilter.Year == 0) && objFilter.StatusID > 1)   //[DPIU=0] [Month=1]
                    {
                        string status = GetAuthStatusBasedOnStatusID(objFilter.StatusID);

                        if (objFilter.AdminNdCode == 0)  //ALL DPIU Selected
                        {
                            objFilter.ParentNdCode = Convert.ToInt32(PMGSYSession.Current.ParentNDCode);
                            objFilter.StateCode = PMGSYSession.Current.StateCode;
                            if (objFilter.Month == 0 && objFilter.Year == 0 && (objFilter.AdminNdCode == 0) && objFilter.StatusID > 1) //[DPIU]=0/1  [Status]=1 Month=0 Year=0
                            {
                                lstEAuthMaster = dbContext.ACC_EAUTH_MASTER.Where(m => m.FUND_TYPE == objFilter.FundType && (m.EAUTH_STATUS == status) && m.ADMIN_DEPARTMENT.MAST_STATE_CODE == objFilter.StateCode && m.ADMIN_DEPARTMENT.MAST_PARENT_ND_CODE == objFilter.ParentNdCode).OrderByDescending(x => x.EAUTH_DATE).ToList();

                            }
                            else if (objFilter.Month == 0 && objFilter.Year != 0 && (objFilter.AdminNdCode == 0) && objFilter.StatusID > 1)   //[DPIU]=0/1 [Status]=1 Month=0 Year=1
                            {
                                lstEAuthMaster = dbContext.ACC_EAUTH_MASTER.Where(m => m.FUND_TYPE == objFilter.FundType && m.EAUTH_YEAR == objFilter.Year && (m.EAUTH_STATUS == status) && m.ADMIN_DEPARTMENT.MAST_STATE_CODE == objFilter.StateCode && m.ADMIN_DEPARTMENT.MAST_PARENT_ND_CODE == objFilter.ParentNdCode).OrderByDescending(x => x.EAUTH_DATE).ToList();

                            }
                            else if (objFilter.Month != 0 && objFilter.Year == 0 && (objFilter.AdminNdCode == 0) && objFilter.StatusID > 1)   //[DPIU]=0/1 [Status]=1 Month=1 Year=0
                            {
                                lstEAuthMaster = dbContext.ACC_EAUTH_MASTER.Where(m => m.FUND_TYPE == objFilter.FundType && m.EAUTH_MONTH == objFilter.Month && (m.EAUTH_STATUS == status) && m.ADMIN_DEPARTMENT.MAST_STATE_CODE == objFilter.StateCode && m.ADMIN_DEPARTMENT.MAST_PARENT_ND_CODE == objFilter.ParentNdCode).OrderByDescending(x => x.EAUTH_DATE).ToList();

                            }
                            else if (objFilter.Month != 0 && objFilter.Year != 0 && (objFilter.AdminNdCode == 0) && objFilter.StatusID > 1)   //[DPIU]=0/1 [Status]=1 Month=1 Year=1
                            {
                                lstEAuthMaster = dbContext.ACC_EAUTH_MASTER.Where(m => m.FUND_TYPE == objFilter.FundType && m.EAUTH_MONTH == objFilter.Month && m.EAUTH_YEAR == objFilter.Year && (m.EAUTH_STATUS == status) && m.ADMIN_DEPARTMENT.MAST_STATE_CODE == objFilter.StateCode && m.ADMIN_DEPARTMENT.MAST_PARENT_ND_CODE == objFilter.ParentNdCode).OrderByDescending(x => x.EAUTH_DATE).ToList();

                            }
                        }
                        else  //When Specific DPIU Selected 
                        {

                            if (objFilter.Month == 0 && objFilter.Year == 0 && (objFilter.AdminNdCode != 0) && objFilter.StatusID > 1) //[DPIU]=0/1  [Status]=1 Month=0 Year=0
                            {
                                lstEAuthMaster = dbContext.ACC_EAUTH_MASTER.Where(m => m.FUND_TYPE == objFilter.FundType && (m.EAUTH_STATUS == status) && (m.ADMIN_ND_CODE == objFilter.AdminNdCode)).OrderByDescending(x => x.EAUTH_DATE).ToList();

                            }
                            else if (objFilter.Month == 0 && objFilter.Year != 0 && (objFilter.AdminNdCode != 0 && objFilter.StatusID > 1))   //[DPIU]=0/1 [Status]=1 Month=0 Year=1
                            {
                                lstEAuthMaster = dbContext.ACC_EAUTH_MASTER.Where(m => m.FUND_TYPE == objFilter.FundType && m.EAUTH_YEAR == objFilter.Year && (m.EAUTH_STATUS == status) && (m.ADMIN_ND_CODE == objFilter.AdminNdCode)).OrderByDescending(x => x.EAUTH_DATE).ToList();

                            }
                            else if (objFilter.Month != 0 && objFilter.Year == 0 && (objFilter.AdminNdCode != 0) && objFilter.StatusID > 1)   //[DPIU]=0/1 [Status]=1 Month=1 Year=0
                            {
                                lstEAuthMaster = dbContext.ACC_EAUTH_MASTER.Where(m => m.FUND_TYPE == objFilter.FundType && m.EAUTH_MONTH == objFilter.Month && (m.EAUTH_STATUS == status) && (m.ADMIN_ND_CODE == objFilter.AdminNdCode)).OrderByDescending(x => x.EAUTH_DATE).ToList();

                            }
                            else if (objFilter.Month != 0 && objFilter.Year != 0 && (objFilter.AdminNdCode != 0) && objFilter.StatusID > 1)   //[DPIU]=0/1 [Status]=1 Month=1 Year=1
                            {
                                lstEAuthMaster = dbContext.ACC_EAUTH_MASTER.Where(m => m.FUND_TYPE == objFilter.FundType && m.EAUTH_MONTH == objFilter.Month && m.EAUTH_YEAR == objFilter.Year && (m.EAUTH_STATUS == status) && (m.ADMIN_ND_CODE == objFilter.AdminNdCode)).OrderByDescending(x => x.EAUTH_DATE).ToList();

                            }

                        }



                    }

                }

                totalRecords = lstEAuthMaster.Count();


                //List to be Display in Ddescending order of EAUTH_DATE
                lstEAuthMaster = lstEAuthMaster.OrderByDescending(x => x.EAUTH_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).ToList<ACC_EAUTH_MASTER>();

                var result = lstEAuthMaster.Select(m => new
                {

                    m.EAUTH_ID,
                    m.EAUTH_NO,
                    m.EAUTH_MONTH,
                    m.EAUTH_YEAR,
                    m.EAUTH_DATE,
                    m.FUND_TYPE,
                    m.ADMIN_ND_CODE,
                    m.LVL_ID,
                    m.EAUTH_STATUS,


                }).ToArray();


                return result.Select(item => new
                {
                    id = URLEncrypt.EncryptParameters(new string[] { item.EAUTH_ID.ToString().Trim() }),
                    cell = new[] {                         

                                                GetDPIUName(item.EAUTH_ID),
                                                item.EAUTH_NO.ToString(),
                                                 commomFuncObj.GetDateTimeToString(item.EAUTH_DATE),
                                                GetCalculatedEAuthorizationRequestAmount(item.EAUTH_ID),
                                                GetAuthStatusInWords(item.EAUTH_STATUS ==null ? " " : item.EAUTH_STATUS),
                                                GetApprovalDate(item.EAUTH_ID),
                                                GetViewDetailsSymbol(item.EAUTH_ID),

                                }
                }).ToArray();


            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorizationDAL.DeleteEAuthorizationMaster()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }



        }
        #endregion

        #region SRRDA Details Grid
        /// <summary>
        /// DAL function to list authorization master details on data entry page(SRRDA Login)
        /// </summary>
        /// <param name="objFilter"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array GetSRRDAeAuthDetailListForApproval(EAuthorizationFilterModel objFilter, out long totalRecords)
        {
            CommonFunctions commomFuncObj = null;
            try
            {
                dbContext = new PMGSYEntities();
                commomFuncObj = new CommonFunctions();
                var query = (from m in dbContext.ACC_EAUTH_DETAILS
                             where
                                     m.EAUTH_ID == objFilter.BillId
                             select new
                             {
                                 m.EAUTH_ID,
                                 m.EAUTH_TXN_NO,
                                 m.AMOUNT,
                                 m.MAST_CON_ID,
                                 m.IMS_AGREEMENT_CODE,
                             });

                totalRecords = query.Count();


                var result = query.Select(m => new
                {

                    m.EAUTH_ID,
                    m.EAUTH_TXN_NO,
                    m.AMOUNT,
                    m.MAST_CON_ID,
                    m.IMS_AGREEMENT_CODE,


                }).ToArray();

                return result.Select(item => new
                {
                    id = URLEncrypt.EncryptParameters(new string[] { item.EAUTH_ID.ToString().Trim() }),
                    cell = new[] {
                                        GetCompanyName(item.MAST_CON_ID==0?0:(item.MAST_CON_ID)),
                                        GetPayeeName(item.MAST_CON_ID==0?0:(item.MAST_CON_ID)),
                                        GetAgreementname(item.IMS_AGREEMENT_CODE==0?0:(item.IMS_AGREEMENT_CODE)),
                                        GetPackageNo(item.EAUTH_ID,item.EAUTH_TXN_NO),
                                        GetAlreadyAuthorisedAmount(item.EAUTH_ID,item.EAUTH_TXN_NO),
                                        GetAgreementAmount(item.EAUTH_ID,item.EAUTH_TXN_NO),
                                        item.AMOUNT.ToString(),
                                        //GetFinalPaymentStatus(item.EAUTH_ID,item.EAUTH_TXN_NO),
                                        GetRadioButtonSymbol(item.EAUTH_TXN_NO,item.EAUTH_ID),


                                        }
                }).ToArray();

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorizationDAL.GetSRRDAeAuthDetailListForApproval()");
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

        #endregion
        #endregion

        /// <summary>
        ///Function to Get Bank Authorization Available in Case of E-Authorization Master Form
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public UDF_ACC_GEN_GET_BA_CASH_CLOSING_BALANCES_Result GetBankAuthorizationAvailable(TransactionParams param)
        {
            try
            {
                dbContext = new PMGSYEntities();
                UDF_ACC_GEN_GET_BA_CASH_CLOSING_BALANCES_Result result = dbContext.UDF_ACC_GEN_GET_BA_CASH_CLOSING_BALANCES(param.FUND_TYPE, param.ADMIN_ND_CODE, param.MONTH, param.YEAR, param.LVL_ID).First();
                return result;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorizationDAL.GetBankAuthorizationAvailable()");
                throw new Exception("Error while Bank Available Authorization.");
            }
            finally
            {
                dbContext.Dispose();
            }

        }

        #region Getting Authorization Number
        /// <summary>
        ///Get E-Auth No based on month/year/statecode/adminCode
        /// </summary>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <param name="stateCode"></param>
        /// <param name="adminNdCode"></param>
        /// <returns></returns>
        public string GetAuthorizationNumber(int month, int year, int stateCode, int adminNdCode)
        {
            CommonFunctions objCommon = null;
            try
            {
                objCommon = new CommonFunctions();
                dbContext = new PMGSYEntities();

                short Month = Convert.ToInt16(month);
                short Year = Convert.ToInt16(year);
                string FundType = PMGSYSession.Current.FundType;
                String financialYear = objCommon.getFinancialYear(Month, Year);

                String AuthorizationNumber = "e-Auth/" + dbContext.MASTER_STATE.Where(x => x.MAST_STATE_CODE == stateCode).Select(y => y.MAST_STATE_SHORT_CODE).First().ToString()
                   + "/" + dbContext.ADMIN_DEPARTMENT.Where(x => x.ADMIN_ND_CODE == adminNdCode).Select(y => y.ADMIN_EPAY_DPIU_CODE).First().ToString() + "/" +
                     financialYear + "/" + month.ToString() + "/";


                int maxEpayVoucherCount = 0;
                int decYear = 0;
                //Physical Fin year
                if (month <= 3)
                {
                    decYear = year - 1;
                }
                else
                {
                    decYear = Year;
                }


                if (!dbContext.ACC_VOUCHER_NUMBER_MASTER.Where(x => x.ADMIN_ND_CODE == adminNdCode && x.FUND_TYPE == FundType && x.BILL_TYPE == "E" && x.FISCAL_YEAR == decYear).Any())   //No Record in Acc_Voucher_Number_Master ..Insert SL_no=0
                {
                    string strVoucherNumber = objCommon.GetPaymentReceiptNumber(adminNdCode, FundType, "E", month, year);
                    string strVoucherCnt = strVoucherNumber.Split('$')[1].Trim();
                    maxEpayVoucherCount = Convert.ToInt16(strVoucherCnt); //1
                }
                else  //Record present in voucher Master Entry
                {
                    ACC_VOUCHER_NUMBER_MASTER oldVoucherNumberModel = new ACC_VOUCHER_NUMBER_MASTER();
                    oldVoucherNumberModel = dbContext.ACC_VOUCHER_NUMBER_MASTER.Where(x => x.ADMIN_ND_CODE == adminNdCode && x.FUND_TYPE == FundType && x.BILL_TYPE == "E" && x.FISCAL_YEAR == decYear).FirstOrDefault();
                    maxEpayVoucherCount = oldVoucherNumberModel.SLNO + 1;
                }


                //if (dbContext.ACC_EAUTH_MASTER.Where(t => t.ADMIN_ND_CODE == adminNdCode).Any())
                //{
                //    maxEpayVoucherCount = dbContext.ACC_EAUTH_MASTER.Where(t => t.EAUTH_MONTH == month && t.EAUTH_YEAR == year && t.ADMIN_ND_CODE == adminNdCode).Count();

                //    maxEpayVoucherCount = maxEpayVoucherCount + 1;

                //}
                //else
                //{
                //    maxEpayVoucherCount = maxEpayVoucherCount + 1;
                //}


                AuthorizationNumber = AuthorizationNumber + maxEpayVoucherCount.ToString();
                return AuthorizationNumber;
            }
            catch (Exception ex)
            {

                ErrorLog.LogError(ex, "EAuthorizationDAL.GetAuthorizationNumber()");
                return string.Empty;
            }


        }
        #endregion

        #region Getting MasterAuthorizationDetails
        /// <summary>
        /// function to get authorization master details Against EAuthID
        /// </summary>
        /// <param name="auth_id"></param>
        /// <returns></returns>
        public ACC_EAUTH_MASTER GetMasterAuthorizationDetails(long auth_id)
        {
            dbContext = new PMGSYEntities();
            try
            {
                return dbContext.ACC_EAUTH_MASTER.Where(c => c.EAUTH_ID == auth_id).First();

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorizationDAL.GetMasterAuthorizationDetails()");
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
        #endregion

        #region Setting PayeeName
        /// <summary>
        /// Returns Payee Name based on contractor
        /// </summary>
        /// <param name="objParam"></param>
        /// <returns></returns>
        public String SetPayeeName(TransactionParams objParam)
        {
            try
            {
                dbContext = new PMGSYEntities();
                MASTER_CONTRACTOR master_Contractor = new MASTER_CONTRACTOR();
                master_Contractor = (from con in dbContext.MASTER_CONTRACTOR
                                     where con.MAST_CON_ID == objParam.MAST_CONT_ID
                                     select con).FirstOrDefault();

                if (master_Contractor == null)
                {
                    return "-";
                }
                else
                {
                    return (master_Contractor.MAST_CON_FNAME != null ? master_Contractor.MAST_CON_FNAME.Trim() : string.Empty) + " " + (master_Contractor.MAST_CON_MNAME != null ? master_Contractor.MAST_CON_MNAME.Trim() : string.Empty) + " " + (master_Contractor.MAST_CON_LNAME != null ? master_Contractor.MAST_CON_LNAME.Trim() : string.Empty);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorizationDAL.SetPayeeName()");
                return string.Empty;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion


        /// <summary>
        /// Converts DateTime To String
        /// </summary>
        /// <param name="strDate"></param>
        /// <returns></returns>
        public static DateTime GetStringToDateTime(string strDate)
        {
            string[] formats = { "dd/MM/yyyy" };
            DateTime newDate;
            bool boolDate = DateTime.TryParse(DateTime.ParseExact(strDate, formats[0], null).ToString(), out newDate);
            if (boolDate)
            {
                return newDate;
            }
            else
            {

                throw new Exception("Invalid Date. Error in Parsing");
            }
        }

        #region Grid Column Values PIU/SRRDA

        #region GetFinalPaymentStatus
        /// <summary>
        ///Method returns Payment Status.either done or pending
        /// </summary>
        /// <param name="EAuthID"></param>
        /// <param name="TxNO"></param>
        /// <returns></returns>
        public String GetFinalPaymentStatus(Int32 EAuthID, Int16 TxNO)
        {
            ACC_EAUTH_DETAILS model = null;
            string symbol = string.Empty;
            bool status = false;
            try
            {
                dbContext = new PMGSYEntities();
                model = new ACC_EAUTH_DETAILS();
                model = dbContext.ACC_EAUTH_DETAILS.Where(x => x.EAUTH_ID == EAuthID && x.EAUTH_TXN_NO == TxNO).FirstOrDefault();
                if (model != null)
                {
                    status = true;
                    if (status)
                    {
                        symbol = "Y";

                    }
                    else
                    {
                        symbol = "N";
                    }
                }
                else
                {
                    symbol = "-";
                }


                return symbol;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorizationDAL.GetFinalPaymentStatus()");
                return string.Empty;
            }
            finally
            {
                dbContext.Dispose();
            }

        }
        #endregion

        #region GetPackageNo
        /// <summary>
        ///Get Package against EAuthID and TxNo
        /// </summary>
        /// <param name="EAuthID"></param>
        /// <param name="TxNO"></param>
        /// <returns></returns>
        public String GetPackageNo(Int32 EAuthID, Int16 TxNO)
        {
            ACC_EAUTH_DETAILS model = null;
            string symbol = string.Empty;
            try
            {
                dbContext = new PMGSYEntities();
                model = new ACC_EAUTH_DETAILS();
                model = dbContext.ACC_EAUTH_DETAILS.Where(x => x.EAUTH_ID == EAuthID && x.EAUTH_TXN_NO == TxNO).FirstOrDefault();
                if (model != null)
                {
                    symbol = model.IMS_PACKAGE_ID;
                }
                else
                {
                    symbol = "-";
                }
                return symbol;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorizationDAL.GetPackageNo()");
                return string.Empty;
            }
            finally
            {
                dbContext.Dispose();
            }



        }
        #endregion

        #region GET GetTotalAmountApproved
        /// <summary>
        ///This Method Returns Total Approved Amount by EO
        /// </summary>
        /// <param name="EAuthID"></param>
        /// <returns></returns>
        public String GetTotalAmountApproved(Int32 EAuthID)
        {
            string Amt = String.Empty;
            ACC_EAUTH_MASTER objMaster = null;
            try
            {
                dbContext = new PMGSYEntities();
                objMaster = new ACC_EAUTH_MASTER();
                objMaster = dbContext.ACC_EAUTH_MASTER.Where(x => x.EAUTH_ID == EAuthID).FirstOrDefault();
                if (objMaster != null)
                {
                    if (objMaster.TOTAL_AUTH_APPROVED == 0 || objMaster.TOTAL_AUTH_APPROVED == null)
                    {
                        Amt = "0";
                    }
                    else
                    {
                        decimal Approvedamt = Convert.ToDecimal(objMaster.TOTAL_AUTH_APPROVED);
                        Amt = Convert.ToString(Approvedamt);
                    }
                }
                else
                {
                    Amt = "0";
                }

                return Amt;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorizationDAL.GetTotalAmountApproved()");
                return string.Empty;
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

        #region GET GetApprovalDate
        /// <summary>
        ///This Method Returns Approval Date by EO
        /// </summary>
        /// <param name="EAuthID"></param>
        /// <returns></returns>
        public String GetApprovalDate(Int32 EAuthID)
        {
            string Approveddate = String.Empty;
            ACC_EAUTH_MASTER objMaster = null;
            try
            {
                dbContext = new PMGSYEntities();
                objMaster = new ACC_EAUTH_MASTER();
                objMaster = dbContext.ACC_EAUTH_MASTER.Where(x => x.EAUTH_ID == EAuthID).FirstOrDefault();
                if (objMaster != null)
                {
                    if (!objMaster.APPOVAL_DATE_SRRDA.HasValue)
                    {
                        Approveddate = "-";
                    }
                    else
                    {
                        DateTime approvedDate = Convert.ToDateTime(objMaster.APPOVAL_DATE_SRRDA);
                        Approveddate = approvedDate.ToString("dd/MM/yyyy");
                    }
                }
                else
                {
                    Approveddate = "-";
                }


                return Approveddate;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorizationDAL.GetApprovalDate()");
                return string.Empty;
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

        #region Get Calculated EAuthorizationRequest Amount
        /// <summary>
        /// Returns Authorized Amount based on EAuthID
        /// </summary>
        /// <param name="EAuthID"></param>
        /// <returns></returns>
        public string GetCalculatedEAuthorizationRequestAmount(Int64 EAuthID)
        {
            string Amt = string.Empty;
            try
            {
                dbContext = new PMGSYEntities();
                decimal TotalAmount = 0;

                if (dbContext.ACC_EAUTH_DETAILS.Where(x => x.EAUTH_ID == EAuthID).Any())
                {
                    TotalAmount = dbContext.ACC_EAUTH_DETAILS.Where(x => x.EAUTH_ID == EAuthID).Sum(c => c.AMOUNT);

                }
                else
                {
                    Amt = "0";
                    return Amt;
                }

                if (TotalAmount == 0)
                {
                    Amt = "0";
                }
                else
                {
                    Amt = Convert.ToString(TotalAmount);

                }
                return Amt;

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorizationDAL.GetCalculatedEAuthorizationRequestAmount()");
                return string.Empty;
            }
            finally
            {
                dbContext.Dispose();
            }

        }
        #endregion

        #region Master FINALIZE Details for Grid
        /// <summary>
        /// Get Finalize/View Symbol
        /// </summary>
        /// <param name="EAuthID"></param>
        /// <returns></returns>
        public String GetMasterFinalizeSymbol(Int64 EAuthID)
        {
            ACC_EAUTH_MASTER model = null;
            string symbol = string.Empty;
            try
            {
                dbContext = new PMGSYEntities();
                model = new ACC_EAUTH_MASTER();

                model = dbContext.ACC_EAUTH_MASTER.Where(x => x.EAUTH_ID == EAuthID).FirstOrDefault();

                if (dbContext.ACC_EAUTH_DETAILS.Where(x => x.EAUTH_ID == EAuthID).Any())//Entry in Detail Table
                {

                    if (model.EAUTH_STATUS == "Y")
                    {

                        //View After Finalized
                        string status = "<center><a href='#' class='ui-icon ui-icon-search' onclick='GetEAuthorizationDetailsViewAfterFinalize(\"" + URLEncrypt.EncryptParameters(new string[] { EAuthID.ToString().Trim() }) + "\");return false;'>View EAuthorization Details</a></center>";
                        //string LockFinalizeSymbol = "<center><a href='#' class='ui-icon ui-icon-locked'>Finalize EAuthorization Details</a></center>";
                        symbol = status;
                    }
                    else if (model.EAUTH_STATUS == "N")
                    {
                        symbol = "<center><a href='#' class='ui-icon ui-icon-unlocked' onclick='FinalizeEAuthorizationMaster(\"" + URLEncrypt.EncryptParameters(new string[] { EAuthID.ToString().Trim() }) + "\");return false;'>Finalize EAuthorization Details</a></center>";


                    }
                    else if (model.EAUTH_STATUS == "A")
                    {
                        //symbol = "<center><a href='#' class='ui-icon ui-icon-locked'>Finalize EAuthorization Details</a></center>";
                        symbol = "<center><a href='#' class='ui-icon ui-icon-search' onclick='GetEAuthorizationDetailsViewAfterFinalize(\"" + URLEncrypt.EncryptParameters(new string[] { EAuthID.ToString().Trim() }) + "\");return false;'>View EAuthorization Details</a></center>";
                    }
                    else if (model.EAUTH_STATUS == "R")
                    {
                        symbol = "<center><a href='#' class='ui-icon ui-icon-search' onclick='GetEAuthorizationDetailsViewAfterFinalize(\"" + URLEncrypt.EncryptParameters(new string[] { EAuthID.ToString().Trim() }) + "\");return false;'>View EAuthorization Details</a></center>";
                        //symbol = "<center><a href='#' class='ui-icon ui-icon-locked'>Finalize EAuthorization Details</a></center>";
                    }
                    else if (model.EAUTH_STATUS == "E")
                    {
                        symbol = "<center><a href='#' class='ui-icon ui-icon-search' onclick='GetEAuthorizationDetailsViewAfterFinalize(\"" + URLEncrypt.EncryptParameters(new string[] { EAuthID.ToString().Trim() }) + "\");return false;'>View EAuthorization Details</a></center>";
                    }
                    else if (model.EAUTH_STATUS == "P")
                    {
                        symbol = "<center><a href='#' class='ui-icon ui-icon-search' onclick='GetEAuthorizationDetailsViewAfterFinalize(\"" + URLEncrypt.EncryptParameters(new string[] { EAuthID.ToString().Trim() }) + "\");return false;'>View EAuthorization Details</a></center>";
                        //symbol = "-";
                    }
                    return symbol;


                }
                else  //No Entry in Detail Table
                {
                    symbol = "-";
                    return symbol;
                }

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorizationDAL.GetMasterFinalizeSymbol()");
                return string.Empty;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion

        #region Get PayeeName For Detail Grid
        /// <summary>
        /// Returns Payee Name
        /// </summary>
        /// <param name="MastConID"></param>
        /// <returns></returns>
        public String GetPayeeName(int MastConID)
        {
            try
            {
                dbContext = new PMGSYEntities();
                MASTER_CONTRACTOR master_Contractor = new MASTER_CONTRACTOR();
                master_Contractor = (from con in dbContext.MASTER_CONTRACTOR
                                     where con.MAST_CON_ID == MastConID
                                     select con).FirstOrDefault();

                if (master_Contractor == null)
                {
                    return "Contractor or supplier Name not Present";
                }
                else
                {
                    return (master_Contractor.MAST_CON_FNAME != null ? master_Contractor.MAST_CON_FNAME.Trim() : string.Empty) + " " + (master_Contractor.MAST_CON_MNAME != null ? master_Contractor.MAST_CON_MNAME.Trim() : string.Empty) + " " + (master_Contractor.MAST_CON_LNAME != null ? master_Contractor.MAST_CON_LNAME.Trim() : string.Empty);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorizationDAL.GetPayeeName()");
                return string.Empty;

            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion

        #region Get Company Name For Detail Grid
        /// <summary>
        /// Returns Company name based on Master Con ID
        /// </summary>
        /// <param name="MastConID"></param>
        /// <returns></returns>
        public String GetCompanyName(int MastConID)
        {
            try
            {
                dbContext = new PMGSYEntities();
                MASTER_CONTRACTOR master_Contractor = new MASTER_CONTRACTOR();
                string name = dbContext.MASTER_CONTRACTOR.Where(x => x.MAST_CON_ID == MastConID).Select(x => x.MAST_CON_COMPANY_NAME).FirstOrDefault();


                if (string.IsNullOrEmpty(name))
                {
                    return "Company Name not Present";
                }
                else
                {
                    return name;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorizationDAL.GetCompanyName()");
                return string.Empty;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion

        #region Get Agreement Name For Detail grid
        /// <summary>
        /// Get Aggrement Name Based on Aggrement code
        /// </summary>
        /// <param name="AgreementCode"></param>
        /// <returns></returns>
        public String GetAgreementname(int AgreementCode)
        {
            try
            {
                string name = string.Empty;
                dbContext = new PMGSYEntities();
                TEND_AGREEMENT_MASTER master = new TEND_AGREEMENT_MASTER();
                name = dbContext.TEND_AGREEMENT_MASTER.Where(x => x.TEND_AGREEMENT_CODE == AgreementCode).Select(x => x.TEND_AGREEMENT_NUMBER).FirstOrDefault();
                return name;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorizationDAL.GetAgreementname()");
                return string.Empty;
            }
            finally
            {
                dbContext.Dispose();
            }

        }
        #endregion

        #region EDIT Details for Grid
        /// <summary>
        /// Returns Edit Symbol for Detail Grid
        /// </summary>
        /// <param name="id"></param>
        /// <param name="TxNo"></param>
        /// 
        /// <returns></returns>
        public string GetEditSymbolForDetails(int id, int TxNo)
        {
            ACC_EAUTH_MASTER model = null;
            string symbol = string.Empty;
            try
            {
                dbContext = new PMGSYEntities();
                model = new ACC_EAUTH_MASTER();
                model = dbContext.ACC_EAUTH_MASTER.Where(x => x.EAUTH_ID == id).FirstOrDefault();
                if (model.EAUTH_STATUS == "Y")
                {
                    symbol = "-";
                }
                else if (model.EAUTH_STATUS == "N")
                {
                    symbol = "<center><a href='#' class='ui-icon ui-icon-pencil' onclick='EditEAuthorizationDetail(\"" + URLEncrypt.EncryptParameters(new string[] { id.ToString().Trim() + "$" + TxNo.ToString().Trim() }) + "\");return false;'>Edit</a></center>";
                }
                else if (model.EAUTH_STATUS == "A")
                {
                    symbol = "-";
                }
                else if (model.EAUTH_STATUS == "R")
                {
                    symbol = "-";
                }
                else if (model.EAUTH_STATUS == "E")
                {
                    symbol = "-";
                }


                return symbol;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorizationDAL.GetEditSymbolForDetails()");
                return string.Empty;
            }
            finally
            {
                dbContext.Dispose();
            }

        }
        #endregion

        #region Delete Details for Grid
        /// <summary>
        /// Returns Delete Symbol for Detail Grid
        /// </summary>
        /// <param name="id"></param>
        /// <param name="TxNo"></param>
        /// 
        /// <returns></returns>
        public string GetDeleteSymbolForDetails(int id, int TxNo)
        {
            ACC_EAUTH_MASTER model = null;
            string symbol = string.Empty;
            try
            {
                dbContext = new PMGSYEntities();
                model = new ACC_EAUTH_MASTER();
                model = dbContext.ACC_EAUTH_MASTER.Where(x => x.EAUTH_ID == id).FirstOrDefault();

                if (model.EAUTH_STATUS == "Y")
                {
                    symbol = "-";
                }
                else if (model.EAUTH_STATUS == "N")
                {
                    symbol = "<center><a href='#' class='ui-icon ui-icon-trash' onclick='DeleteEAuthorizationDetail(\"" + URLEncrypt.EncryptParameters(new string[] { id.ToString().Trim() + "$" + TxNo.ToString().Trim() }) + "\");return false;'>Delete</a></center>";
                }
                else if (model.EAUTH_STATUS == "A")
                {
                    symbol = "-";
                }
                else if (model.EAUTH_STATUS == "R")
                {
                    symbol = "-";
                }
                else if (model.EAUTH_STATUS == "E")
                {
                    symbol = "-";
                }

                return symbol;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorizationDAL.GetDeleteSymbolForDetails()");
                return string.Empty;
            }
            finally
            {
                dbContext.Dispose();
            }

        }
        #endregion

        #region Master Delete Details for Grid
        /// <summary>
        /// Returns Delete Symbol in Case of Master Grid
        /// </summary>
        /// <param name="EAuthID"></param>
        /// <returns></returns>
        public string GetMasterDeleteSymbol(Int64 EAuthID)
        {
            ACC_EAUTH_MASTER model = null;
            string symbol = string.Empty;
            try
            {
                dbContext = new PMGSYEntities();
                model = new ACC_EAUTH_MASTER();
                model = dbContext.ACC_EAUTH_MASTER.Where(x => x.EAUTH_ID == EAuthID).FirstOrDefault();

                if (model.EAUTH_STATUS == "Y")
                {
                    symbol = "-";
                }
                else if (model.EAUTH_STATUS == "N")
                {
                    symbol = "<center><a href='#' class='ui-icon ui-icon-trash' onclick='DeleteEAuthorizationMaster(\"" + URLEncrypt.EncryptParameters(new string[] { EAuthID.ToString().Trim() }) + "\");return false;'>Delete EAuthorization Details</a></center>";
                }
                else if (model.EAUTH_STATUS == "A")
                {
                    symbol = "-";
                }
                else if (model.EAUTH_STATUS == "R")
                {
                    symbol = "-";
                }
                else if (model.EAUTH_STATUS == "E")
                {
                    symbol = "-";
                }
                else if (model.EAUTH_STATUS == "P")
                {
                    symbol = "-";
                }
                else
                {
                    symbol = "-";


                }


                return symbol;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorizationDAL.GetMasterDeleteSymbol()");
                return string.Empty;
            }
            finally
            {
                dbContext.Dispose();
            }

        }

        #endregion

        #region Master EDIT Details for Grid
        /// <summary>
        /// Returns Edit Symbol in Case of Master Grid
        /// </summary>
        /// <param name="EAuthID"></param>
        /// <returns></returns>
        public string GetMasterEditSymbol(Int64 EAuthID)
        {
            ACC_EAUTH_MASTER model = null;
            string symbol = string.Empty;
            try
            {
                dbContext = new PMGSYEntities();
                model = new ACC_EAUTH_MASTER();

                model = dbContext.ACC_EAUTH_MASTER.Where(x => x.EAUTH_ID == EAuthID).FirstOrDefault();

                if (model.EAUTH_STATUS == "Y")
                {
                    symbol = "-";
                }
                else if (model.EAUTH_STATUS == "N")
                {
                    symbol = "<center><a href='#' class='ui-icon ui-icon-pencil' onclick='EditEAuthorizationMaster(\"" + URLEncrypt.EncryptParameters(new string[] { EAuthID.ToString().Trim() }) + "\");return false;'>Edit EAuthorization Details</a></center>";
                }
                else if (model.EAUTH_STATUS == "A")
                {
                    symbol = "-";
                }
                else if (model.EAUTH_STATUS == "R")
                {
                    symbol = "-";
                }
                else if (model.EAUTH_STATUS == "E")
                {
                    symbol = "-";
                }
                else if (model.EAUTH_STATUS == "P")
                {
                    symbol = "-";
                }
                else
                {
                    symbol = "-";


                }
                return symbol;





            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorizationDAL.GetMasterEditSymbol()");
                return string.Empty;
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

        #region GetAggrement Amount
        /// <summary>
        /// This Method Returns Aggrement Amount
        /// </summary>
        /// <param name="EauthID"></param>
        /// <param name="TxNo"></param>
        /// 
        /// <returns></returns>
        public string GetAgreementAmount(Int32 EauthID, Int16 TxNo)
        {
            dbContext = new PMGSYEntities();
            ACC_EAUTH_MASTER objMaster = null;
            ACC_EAUTH_DETAILS objDetail = null;
            string Amt = String.Empty;
            try
            {

                objMaster = new ACC_EAUTH_MASTER();
                objDetail = new ACC_EAUTH_DETAILS();

                if (dbContext.ACC_EAUTH_DETAILS.Where(x => x.EAUTH_ID == EauthID).Any())
                {
                    objMaster = dbContext.ACC_EAUTH_MASTER.Where(x => x.EAUTH_ID == EauthID).FirstOrDefault();
                    if (objMaster != null)
                    {

                        objDetail = dbContext.ACC_EAUTH_DETAILS.Where(x => x.EAUTH_ID == EauthID && x.EAUTH_TXN_NO == TxNo).FirstOrDefault();
                        if (objDetail != null)
                        {


                            Amt = Convert.ToString(objDetail.TEND_AGREEMENT_AMOUNT);
                            if (String.IsNullOrEmpty(Amt))
                            {
                                Amt = "0";
                            }
                            /*
                            var result = dbContext.UDF_ACC_EAUTH_AGR_AUTH_AMT(objMaster.ADMIN_ND_CODE, objDetail.MAST_CON_ID, objDetail.IMS_AGREEMENT_CODE, objMaster.FUND_TYPE);

                            if (result != null)
                            {
                                foreach (var item in result)
                                {

                                    if (item.TEND_AGREEMENT_AMOUNT == null)
                                    {
                                        tend_Aggrement_Amount = 0;
                                        Amt = Convert.ToString(tend_Aggrement_Amount);
                                    }
                                    else
                                    {
                                        tend_Aggrement_Amount = Convert.ToDecimal(item.TEND_AGREEMENT_AMOUNT);
                                        Amt = Convert.ToString(tend_Aggrement_Amount);
                                    }
                                }
                            }
                            else
                            {
                                Amt = "0";
                            }
                             * */



                        }
                        else
                        {
                            Amt = "0";
                        }


                    }
                    else
                    {
                        Amt = "0";
                    }


                }
                else
                {
                    Amt = "0";
                }
                return Amt;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorizationDAL.GetAgreementAmount()");
                return string.Empty;
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

        #region GetAlready Authorised Amount
        /// <summary>
        /// Get Already Authorised Amount
        /// </summary>
        /// <param name="EauthID"></param>
        /// <param name="TxNo"></param>
        /// <returns></returns>
        public string GetAlreadyAuthorisedAmount(Int32 EauthID, Int16 TxNo)
        {
            dbContext = new PMGSYEntities();
            ACC_EAUTH_MASTER objMaster = null;
            ACC_EAUTH_DETAILS objDetail = null;
            string Amt = String.Empty;
            try
            {

                objMaster = new ACC_EAUTH_MASTER();
                objDetail = new ACC_EAUTH_DETAILS();
                decimal Auth_Amount = 0;
                if (dbContext.ACC_EAUTH_DETAILS.Where(x => x.EAUTH_ID == EauthID).Any())
                {
                    objMaster = dbContext.ACC_EAUTH_MASTER.Where(x => x.EAUTH_ID == EauthID).FirstOrDefault();
                    if (objMaster != null)
                    {

                        objDetail = dbContext.ACC_EAUTH_DETAILS.Where(x => x.EAUTH_ID == EauthID && x.EAUTH_TXN_NO == TxNo).FirstOrDefault();
                        Amt = Convert.ToString(objDetail.ALREADY_AUTH_AMOUNT);
                        if (String.IsNullOrEmpty(Amt))
                        {
                            Amt = "0";
                        }
                        //if (objDetail != null)
                        //{
                        //    var result = dbContext.UDF_ACC_EAUTH_AGR_AUTH_AMT(objMaster.ADMIN_ND_CODE, objDetail.MAST_CON_ID, objDetail.IMS_AGREEMENT_CODE, objMaster.FUND_TYPE);

                        //    if (result != null)
                        //    {
                        //        foreach (var item in result)
                        //        {
                        //            if (item.AUTH_AMOUNT == null)
                        //            {
                        //                Auth_Amount = 0;
                        //                Amt = Convert.ToString(Auth_Amount);

                        //            }
                        //            else
                        //            {
                        //                Auth_Amount = Convert.ToDecimal(item.AUTH_AMOUNT);
                        //                Amt = Convert.ToString(Auth_Amount);

                        //            }
                        //        }

                        //    }
                        //    else
                        //    {
                        //        Amt = "0";
                        //    }

                        //}
                        //else
                        //{
                        //    Amt = "0";
                        //}


                    }
                    else
                    {
                        Amt = "0";
                    }


                }
                else
                {
                    Amt = "0";
                }
                return Amt;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorizationDAL.GetAlreadyAuthorisedAmount()");
                return string.Empty;
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

        #region Get Auth Status Based on StatusID
        /// <summary>
        /// function to get auth status based on auth id
        /// </summary>
        /// <param name="auth_Status"></param>
        /// <returns></returns>
        public string GetAuthStatusBasedOnStatusID(Int32 StatusID)
        {
            String Status = string.Empty;
            switch (StatusID)
            {

                case 2:
                    Status = "Y";
                    break;
                case 3:
                    Status = "A";
                    break;
                case 4:
                    Status = "P";
                    break;
                case 5:
                    Status = "R";
                    break;

                //case 6:
                //    Status = "E";
                //    break;


                default:
                    Status = "";
                    break;
            }

            return Status;
        }
        #endregion

        #region Get AuthStatus in Words
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
                case "N":
                    Status = "Draft";
                    break;
                case "Y":
                    Status = "Pending at SRRDA";
                    break;
                case "A":
                    Status = "Approved";
                    break;
                case "R":
                    Status = "Rejected";
                    break;
                case "P":
                    Status = "Partially Approved";
                    break;
                default:
                    Status = "";
                    break;
            }
            return Status;
        }
        #endregion

        #region SRRDA Get DPIU Name
        /// <summary>
        /// Returns DPIU Name
        /// </summary>
        /// <param name="EAuthID"></param>
        /// <returns></returns>
        public string GetDPIUName(Int64 EAuthID)
        {
            string name = string.Empty;
            try
            {
                dbContext = new PMGSYEntities();
                int AdminNDCode = dbContext.ACC_EAUTH_MASTER.Where(x => x.EAUTH_ID == EAuthID).Select(x => x.ADMIN_ND_CODE).FirstOrDefault();
                if (AdminNDCode != 0)
                {
                    string AdminName = dbContext.ADMIN_DEPARTMENT.Where(x => x.ADMIN_ND_CODE == AdminNDCode).Select(x => x.ADMIN_ND_NAME).FirstOrDefault();
                    if (String.IsNullOrEmpty(AdminName))
                    {
                        name = "-";
                        return name;

                    }
                    else
                    {
                        return AdminName;
                    }

                }
                else
                {
                    name = "-";
                    return name;

                }



            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorizationDAL.PopulateSTATUSForSRRDA()");
                return string.Empty;
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

        #region SRRDA View Details Symbol
        /// <summary>
        /// View Details Symbol to view Detail Grid
        /// </summary>
        /// <param name="EAuthID"></param>
        /// <returns></returns>
        public string GetViewDetailsSymbol(Int64 EAuthID)
        {

            string view = string.Empty;
            try
            {
                dbContext = new PMGSYEntities();
                string status = dbContext.ACC_EAUTH_MASTER.Where(x => x.EAUTH_ID == EAuthID).Select(x => x.EAUTH_STATUS).FirstOrDefault();
                if (String.IsNullOrEmpty(status))
                {
                    status = "-";
                    return status;
                }
                else
                {
                    status = "<center><a href='#' class='ui-icon ui-icon-search' onclick='ViewEAuthorizationDetailsForApproval(\"" + URLEncrypt.EncryptParameters(new string[] { EAuthID.ToString().Trim() }) + "\");return false;'>View EAuthorization Details</a></center>";

                    return status;
                }

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorizationDAL.GetViewDetailsSymbol()");
                return string.Empty;

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

        #region Radio Button Symbol
        /// <summary>
        /// Returns Radio Button Status
        /// </summary>
        /// <param name="EAUTH_TXN_NO"></param>
        /// <param name="EAuthID"></param>
        /// 
        /// <returns></returns>
        public string GetRadioButtonSymbol(Int64 EAUTH_TXN_NO, Int64 EAuthID)
        {
            string symbol = string.Empty;
            string ApprovalStatus = string.Empty;
            ACC_EAUTH_DETAILS model = null;
            ACC_EAUTH_MASTER objmaster = null;
            try
            {
                dbContext = new PMGSYEntities();
                model = new ACC_EAUTH_DETAILS();
                string EncTXN_NO = URLEncrypt.EncryptParameters(new string[] { EAUTH_TXN_NO.ToString().Trim() });
                string sEncryptedEAUTH_ID = URLEncrypt.EncryptParameters(new string[] { EAUTH_TXN_NO.ToString().Trim() });// change id here 
                objmaster = new ACC_EAUTH_MASTER();
                model = dbContext.ACC_EAUTH_DETAILS.Where(x => x.EAUTH_TXN_NO == EAUTH_TXN_NO && x.EAUTH_ID == EAuthID).FirstOrDefault();

                if (model.APPROVAL_STAUS == "N")  //Not Approved
                {
                    int level = PMGSYSession.Current.LevelId;
                    if (level == 5)
                    {

                        symbol = "Pending at SRRDA";
                    }
                    else
                    {
                        symbol = "<center><input type='radio' name='EAuthRemark_" + EAUTH_TXN_NO + "" + "' value='EAuth_A' onchange='RadioClick(\"" + EncTXN_NO + "\",\"" + EAUTH_TXN_NO + "\");'  id=" + EAUTH_TXN_NO + " " + "class='clsRemark' checked ><b>Approve</b>&nbsp; <input type='radio' name='EAuthRemark_" + EAUTH_TXN_NO + "" + "' value='EAuth_R' onchange='RadioClick1(\"" + EncTXN_NO + "\",\"" + EAUTH_TXN_NO + "\");'  id=" + EAUTH_TXN_NO + " " + "class='clsRemark'><b>Reject</b><br><textarea rows='1' cols='30' class='txtAreaRejectRemark'  onfocusout='validateRemark(\"" + EncTXN_NO + "\",\"" + EAUTH_TXN_NO + "\");' id='txtRemark_" + EAUTH_TXN_NO + "" + "' style='display:none' maxlength='250'></textarea></center>";
                    }



                    //<input type='text'  hidden='hidden' id='hidEncId_" + EAUTH_TXN_NO + "' value='" + sEncryptedEAUTH_ID + "'/>
                }
                else if (model.APPROVAL_STAUS == "A")
                {
                    symbol = "<center><input type='radio' name='EAuthRemark_" + EAUTH_TXN_NO + "" + "' value='EAuth_A' id=" + EAUTH_TXN_NO + " " + "class='clsRemark' checked ><b>Approved</b></center>";

                }
                else if (model.APPROVAL_STAUS == "R")
                {

                    //symbol = "<center><input type='radio' name='EAuthRemark_" + EAUTH_TXN_NO + "" + "' value='EAuth_R' id=" + EAUTH_TXN_NO + " " + "class='clsRemark' checked ><b>Reject</b><br/>" + "<span style='color:Red'>Remark:</span>" + model.APPROVAL_REMARKS + "</center>";

                    symbol = "<center><b>Rejected</b><br/>" + "<span style='color:Red'>Remark:</span>" + model.APPROVAL_REMARKS + "</center>";

                }
                else
                {

                    symbol = "-";
                }

                return symbol;

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorizationDAL.GetRadioButtonSymbol()");
                return string.Empty;
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

        #endregion

        #region GetDPIU list For EO
        /// <summary>
        /// Returns DPIU List 
        /// </summary>
        /// <param name="objParam"></param>
        /// <returns></returns>
        public List<SelectListItem> PopulateDPIUForSRRDA(TransactionParams objParam)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<SelectListItem> lstDPIU = null;
                if (objParam.DISTRICT_CODE == 0)
                {
                    lstDPIU = new SelectList(dbContext.ADMIN_DEPARTMENT.Where(m => m.MAST_PARENT_ND_CODE == objParam.ADMIN_ND_CODE && m.MAST_ND_TYPE == "D"), "ADMIN_ND_CODE", "ADMIN_ND_NAME", objParam.ADMIN_ND_CODE).ToList();
                }
                else
                {
                    lstDPIU = new SelectList(dbContext.ADMIN_DEPARTMENT.Where(m => m.MAST_DISTRICT_CODE == objParam.DISTRICT_CODE), "ADMIN_ND_CODE", "ADMIN_ND_NAME", objParam.ADMIN_ND_CODE).ToList();
                }
                if (dbContext.ACC_TEO_SCREEN_DESIGN_PARAM_DETAILS.Where(m => m.TXN_ID == objParam.TXN_ID).Select(m => m.DPIU_REQ).FirstOrDefault() == "N")
                {
                    lstDPIU.Insert(0, (new SelectListItem { Text = "All DPIU", Value = "0", Selected = true }));
                    return lstDPIU;
                }
                else if (lstDPIU == null || lstDPIU.Count() == 0)
                {
                    lstDPIU.Insert(0, (new SelectListItem { Text = "All DPIU", Value = "0", Selected = true }));
                    return lstDPIU;
                }
                lstDPIU = lstDPIU.OrderBy(m => m.Text).ToList();
                lstDPIU.Insert(0, (new SelectListItem { Text = "All DPIU", Value = "0" }));
                return lstDPIU;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorizationDAL.PopulateDPIUForSRRDA()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        #endregion

        #region GetSTATUS list For EO
        /// <summary>
        /// Returns Status list From MASTER_STATUS
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public List<SelectListItem> PopulateSTATUSForSRRDA()
        {
            List<SelectListItem> lstStatus = null;
            try
            {
                dbContext = new PMGSYEntities();
                lstStatus = new List<SelectListItem>();
                lstStatus = new SelectList(dbContext.MASTER_STATUS, "STATUS_ID", "STATUS_DESC").Skip(1).ToList();
                return lstStatus;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorizationDAL.PopulateSTATUSForSRRDA()");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        #endregion

        #region Populate New Already Authorised Amount
        /// <summary>
        /// This Method Returns already Authorised Amount
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public EAuthorizationAmountRequestModel CheckAlreadyAuthorisedAmount(EAuthorizationAmountRequestModel model)
        {
            PMGSYEntities dbContext = null;
            ACC_EAUTH_AUTHAMT_DETAILS obj = null;

            try
            {
                dbContext = new PMGSYEntities();
                obj = new ACC_EAUTH_AUTHAMT_DETAILS();

                //AdminNDCode...PAckage..Aggrement..Contractor
                obj = dbContext.ACC_EAUTH_AUTHAMT_DETAILS.Where(x => x.ADMIN_ND_CODE == model.ADMIN_ND_CODE && x.IMS_PACKAGE_ID == model.IMS_SANCTION_PACKAGE && x.IMS_AGREEMENT_CODE == model.AGREEMENT_CODE && x.MAST_CON_ID ==
model.MAST_CONT_ID && x.ISACTIVE == true).FirstOrDefault();
                if (obj != null)
                {
                    if (obj.AMOUNT_AUTHORIZED == 0)
                    {
                        model.AMOUNT_AUTHORIZED = 0;

                    }
                    else
                    {
                        model.AMOUNT_AUTHORIZED = obj.AMOUNT_AUTHORIZED;
                    }

                }

                return model;



            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorizationDAL.CheckAlreadyAuthorisedAmount()");
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
        #endregion

        #region  Get AddEAuthorizationLinkView
        public EAuthorizationAmountRequestModel GetAddEAuthorizationLinkView(EAuthorizationAmountRequestModel model)
        {
            PMGSYEntities dbcontext = null;
            EAuthorizationAmountRequestModel objEAuthorizationAmountRequestModel = null;
            string ContractorName = string.Empty;
            string AggrementNumber = string.Empty;
            string PackageName = string.Empty;
            CommonFunctions objCommon = null;
            try
            {
                dbcontext = new PMGSYEntities();
                objEAuthorizationAmountRequestModel = new EAuthorizationAmountRequestModel();
                objCommon = new CommonFunctions();
                if (model.MAST_CONT_ID != 0 && model.AGREEMENT_CODE != 0 && !String.IsNullOrEmpty(model.IMS_SANCTION_PACKAGE))
                {
                    objEAuthorizationAmountRequestModel.Contractor_Name = GetCompanyName(model.MAST_CONT_ID);
                    objEAuthorizationAmountRequestModel.Package_Number = model.IMS_SANCTION_PACKAGE;
                    objEAuthorizationAmountRequestModel.Aggrement_Number = GetAgreementname(model.AGREEMENT_CODE);
                    objEAuthorizationAmountRequestModel.MAST_CONT_ID = model.MAST_CONT_ID;
                    objEAuthorizationAmountRequestModel.AGREEMENT_CODE = model.AGREEMENT_CODE;
                    objEAuthorizationAmountRequestModel.IMS_SANCTION_PACKAGE = model.IMS_SANCTION_PACKAGE;
                }
                else
                {
                    objEAuthorizationAmountRequestModel.Contractor_Name = "-";
                    objEAuthorizationAmountRequestModel.Package_Number = "-";
                    objEAuthorizationAmountRequestModel.Aggrement_Number = "-";
                }
                DateTime Todaysdate = DateTime.Now;
                objEAuthorizationAmountRequestModel.DateAsOnNow = objCommon.GetDateTimeToString(Todaysdate);

                return objEAuthorizationAmountRequestModel;

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "EAuthorizationDAL.GetAddEAuthorizationLinkView()");
                return null;
            }
            finally
            {
                if (dbcontext != null)
                {
                    dbcontext.Dispose();
                }
            }


        }
        #endregion

    }
    public interface IEAuthorizationDAL
    {
        #region PIU
        EAuthorizationRequestDetailsModel DeleteEAuthorizationMaster(Int64 EAuthID);
        String GetMasterEditSymbol(Int64 EAuthID);
        String GetMasterDeleteSymbol(Int64 EAuthID);
        String GetMasterFinalizeSymbol(Int64 EAuthID);
        EAuthorizationMasterModel AddEAuthorizationMasterDetails(EAuthorizationMasterModel model);
        string GetAuthorizationNumber(int month, int year, int stateCode, int adminNdCode);
        ACC_EAUTH_MASTER GetMasterAuthorizationDetails(long auth_id);
        Array ListEAuthorizationMasterDetails(EAuthorizationFilterModel objFilter, out long totalRecords);
        Array GetPaymentDetailList(EAuthorizationFilterModel objFilter, out long totalRecords);
        Int32 AddPaymentTransactionDetails(EAuthorizationRequestDetailsModel model, string operationType, Int64 Bill_id, String AddorEdit, Int16 tranNumber);
        String GetDeleteSymbolForDetails(int id, int TxNo);
        String GetEditSymbolForDetails(int id, int TxNo);
        String SetPayeeName(TransactionParams objParam);
        String GetPayeeName(int MastConID);
        String GetCompanyName(int MastConID);
        String GetAgreementname(int AgreementCode);
        String GetPackageNo(Int32 EAuthID, Int16 TxNO);
        EAuthorizationRequestDetailsModel UpdateEAuthorizationDetails(EAuthorizationRequestDetailsModel model);
        EAuthorizationRequestDetailsModel DeleteEAuthorizationDetails(Int64 EAuthID, int TxNo);
        EAuthorizationRequestDetailsModel FinalizeEAuthorizationDetails(Int64 EAuthID);
        string GetCalculatedEAuthorizationRequestAmount(Int64 EAuthID);
        String GetTotalAmountApproved(Int32 EAuthID);
        String GetApprovalDate(Int32 EAuthID);
        String GetFinalPaymentStatus(Int32 EauthID, Int16 TxNo);
        UDF_ACC_GEN_GET_BA_CASH_CLOSING_BALANCES_Result GetBankAuthorizationAvailable(TransactionParams param);
        string GetAgreementAmount(Int32 EauthID, Int16 TxNo);
        string GetAlreadyAuthorisedAmount(Int32 EauthID, Int16 TxNo);
        EAuthorizationAmountRequestModel CheckAlreadyAuthorisedAmount(EAuthorizationAmountRequestModel model);
        EAuthorizationAmountRequestModel GetAddEAuthorizationLinkView(EAuthorizationAmountRequestModel model);
        bool AddNewAuthorizationEntry(EAuthorizationAmountRequestModel model);
        #endregion

        #region SRRDA
        Array EAuthorizationRequestListView(EAuthorizationFilterModel objFilter, out long totalRecords);
        Array SRRDAeAuthorizationRequestListData(EAuthorizationFilterModel objFilter, out long totalRecords);
        Array GetSRRDAeAuthDetailListForApproval(EAuthorizationFilterModel objFilter, out long totalRecords);
        string GetDPIUName(Int64 EAuthID);
        string GetViewDetailsSymbol(Int64 EAuthID);
        string GetRadioButtonSymbol(Int64 EAUTH_TXN_NO, Int64 EAuthID);
        List<SelectListItem> PopulateDPIUForSRRDA(TransactionParams objParam);
        EAuthorizationRequestDetailsModel ProceedForApproveRejectDetails(string ApproveArr, string RejectArr, Int64 EAuthID);
        List<SelectListItem> PopulateSTATUSForSRRDA();
        String GetAuthStatusBasedOnStatusID(Int32 StatusID);
        EAuthorizationRequestDetailsModel SaveNotificationDetailsAfterSendingMail(string EncryptedEAuthID);

        #endregion
    }

}