using PMGSY.Common;
using PMGSY.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Transactions;
using PMGSY.Models.PaymentModel;
using System.Data.Entity;
using System.Data.Entity.Validation;
using PMGSY.Extensions;
using PMGSY.Models.Payment;
using System.Security.Cryptography;
using System.Text;
using PMGSY.Models.Common;
using PMGSY.Controllers;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using System.IO;
//using System.Data.Entity.Objects.SqlClient;
using System.Data.SqlClient;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.SqlServer;
using System.Data.Entity.Core;
using System.Data;
using System.Xml.Linq;

namespace PMGSY.DAL.Payment
{
    public class PaymentDAL : IPaymentDAL
    {
        private PMGSYEntities dbContext = null;

        public PaymentDAL()
        {

        }

        #region master Payment
        /// <summary>
        /// function to add and edit master payment details
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Int64 AddEditMasterPaymentDetails(PaymentMasterModel model, string operationType, Int64 Bill_id, bool IsAdvicePayment)
        {
            CommonFunctions objCommon = new CommonFunctions();
            dbContext = new PMGSYEntities();
            string[] TXN_ID_CHQ_EPAY = model.TXN_ID.Split('$');
            int TXN_Id = Convert.ToInt32(TXN_ID_CHQ_EPAY[0]);

            try
            {
                //if (model.CHQ_EPAY == "E")
                //{
                //    if (dbContext.ADMIN_DEPARTMENT.Any(m => m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && m.ADMIN_EPAY_ENABLE_DATE == null))
                //    {
                //        return -111;
                //    }
                //}

                //if (model.CHQ_EPAY == "R")
                //{
                //    if (dbContext.ADMIN_DEPARTMENT.Any(m => m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && m.ADMIN_EREMITTANCE_ENABLED == "N"))
                //    {
                //        return -222;
                //    }
                //}

                using (var scope = new TransactionScope())
                {

                    ACC_BILL_MASTER ModelToAdd = new ACC_BILL_MASTER();

                    Int64 maxBillId = 0;
                    short bankCode = 0;

                    //if epayment check if epayment number already exist
                    if (model.CHQ_EPAY.Trim() == "E")
                    {
                        if (dbContext.ACC_BILL_MASTER.Where(c => c.BILL_MONTH == model.BILL_MONTH
                            && c.BILL_YEAR == model.BILL_YEAR && c.FUND_TYPE == model.FUND_TYPE
                            && c.LVL_ID == c.LVL_ID &&
                            c.ADMIN_ND_CODE == c.ADMIN_ND_CODE
                            && !operationType.Equals("A") ? c.BILL_ID != Bill_id : 1 == 1
                            ).Select(x => x.CHQ_NO).FirstOrDefault() == model.CHQ_NO)
                        {
                            return Bill_id = -2;
                        }



                    }

                    //check if bank details is entered for the selected fund
                    //if (PMGSYSession.Current.LevelId == 5)
                    //{
                    //    int? parentNdCode = dbContext.ADMIN_DEPARTMENT.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Select(x => x.MAST_PARENT_ND_CODE).First();
                    //    bankCode = dbContext.ACC_BANK_DETAILS.Where(c => c.ADMIN_ND_CODE == parentNdCode && c.BANK_ACC_STATUS == true && c.FUND_TYPE == PMGSYSession.Current.FundType).Select(x => x.BANK_CODE).FirstOrDefault();
                    //}
                    //else
                    //{
                    //    bankCode = dbContext.ACC_BANK_DETAILS.Where(c => c.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && c.BANK_ACC_STATUS == true && c.FUND_TYPE == PMGSYSession.Current.FundType).Select(x => x.BANK_CODE).FirstOrDefault();
                    //}
                    //Below if condition added on 21-03-2023
                    if (PMGSYSession.Current.LevelId == 5 && TXN_Id == 3185)
                    {
                        int? parentNdCode = dbContext.ADMIN_DEPARTMENT.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Select(x => x.MAST_PARENT_ND_CODE).First();

                        if (parentNdCode.HasValue)
                        {
                            bankCode = dbContext.ACC_BANK_DETAILS.Where(c => c.ADMIN_ND_CODE == parentNdCode && c.BANK_ACC_STATUS == true && c.FUND_TYPE == PMGSYSession.Current.FundType && c.ACCOUNT_TYPE == "H").Select(x => x.BANK_CODE).FirstOrDefault();
                        }

                    }
                    else
                    {
                        if (PMGSYSession.Current.LevelId == 5)
                        {
                            int? parentNdCode = dbContext.ADMIN_DEPARTMENT.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Select(x => x.MAST_PARENT_ND_CODE).First();
                            //bankCode = dbContext.ACC_BANK_DETAILS.Where(c => c.ADMIN_ND_CODE == parentNdCode && c.BANK_ACC_STATUS == true && c.FUND_TYPE == PMGSYSession.Current.FundType).Select(x => x.BANK_CODE).FirstOrDefault();

                            if (PMGSYSession.Current.FundType == "P" || PMGSYSession.Current.FundType == "A" || ((TXN_Id == 3050 || TXN_Id == 3051) && PMGSYSession.Current.FundType == "M"))
                            {
                                //***--***
                                bankCode = dbContext.ACC_BANK_DETAILS.Where(c => c.ADMIN_ND_CODE == parentNdCode && c.BANK_ACC_STATUS == true && c.FUND_TYPE == "P" && c.ACCOUNT_TYPE == "S").Select(x => x.BANK_CODE).FirstOrDefault();
                            }
                            else
                            {
                                //***--***
                                bankCode = dbContext.ACC_BANK_DETAILS.Where(c => c.ADMIN_ND_CODE == parentNdCode && c.BANK_ACC_STATUS == true && c.FUND_TYPE == "M" && c.ACCOUNT_TYPE == "S").Select(x => x.BANK_CODE).FirstOrDefault();
                            }
                        }
                        else
                        {
                            //bankCode = dbContext.ACC_BANK_DETAILS.Where(c => c.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && c.BANK_ACC_STATUS == true && c.FUND_TYPE == PMGSYSession.Current.FundType).Select(x => x.BANK_CODE).FirstOrDefault();
                            if (PMGSYSession.Current.FundType == "P" || PMGSYSession.Current.FundType == "A" || ((TXN_Id == 3050 || TXN_Id == 3051) && PMGSYSession.Current.FundType == "M"))
                            {
                                //***--***
                                bankCode = dbContext.ACC_BANK_DETAILS.Where(c => c.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && c.BANK_ACC_STATUS == true && c.FUND_TYPE == "P" && c.ACCOUNT_TYPE == "S").Select(x => x.BANK_CODE).FirstOrDefault();
                            }
                            else
                            {
                                //***--***
                                bankCode = dbContext.ACC_BANK_DETAILS.Where(c => c.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && c.BANK_ACC_STATUS == true && c.FUND_TYPE == "M" && c.ACCOUNT_TYPE == "S").Select(x => x.BANK_CODE).FirstOrDefault();
                            }
                        }
                    }



                    if (bankCode == 0)
                    {
                        return Bill_id = -3;
                    }

                    //Cheque book Issed date validation Added by Abhishek kamble 9Mar2015 start

                    //if ((model.CHQ_Book_ID != null && model.CHQ_Book_ID != 0) && (!(String.IsNullOrEmpty(model.CHQ_NO))))
                    //{
                    //    DateTime chqBookIssueDate = dbContext.ACC_CHQ_BOOK_DETAILS.Where(m => m.CHQ_BOOK_ID == model.CHQ_Book_ID).Select(s => s.ISSUE_DATE).FirstOrDefault();
                    //    CommonFunctions obj = new CommonFunctions();                                       
                    //    if ((DateTime.Compare(obj.GetStringToDateTime(model.BILL_DATE), chqBookIssueDate)) < 0)
                    //    {
                    //        return Bill_id = -999; 
                    //    }
                    //}

                    //Cheque book Issed date validation Added by Abhishek kamble 9Mar2015 end                    

                    if (operationType.Equals("A"))
                    {
                        if (dbContext.ACC_BILL_MASTER.Any())
                        {
                            maxBillId = dbContext.ACC_BILL_MASTER.Max(c => c.BILL_ID);

                        }

                        maxBillId = maxBillId + 1;

                        ModelToAdd.BILL_ID = maxBillId;

                        //accept bill number from user only in case of add
                        ModelToAdd.BILL_NO = model.BILL_NO;


                    }
                    else
                    {
                        ModelToAdd.BILL_ID = Bill_id;
                    }

                    //if operation is addition add the cheque details in cheque issued
                    if (operationType.Equals("A"))
                    {
                        if (model.CHQ_EPAY.Trim().Equals("Q"))
                        {
                            //at SRRDA level we have not applied validation for cheque book series; for sake of old entries ;so if its 0 dont add it
                            //if (model.CHQ_Book_ID != 0) //old
                            //If condition Modified by abhishek 12Nov2014 to enter ACC_CHEQUES_ISSUED details
                            if ((!(model.CHQ_Book_ID == 0)) || (PMGSYSession.Current.LevelId == 4) || (IsAdvicePayment)) //Modified by Abhishek for Advice no 6Apr2015
                            {
                                ACC_CHEQUES_ISSUED chequeIssuedModel = new ACC_CHEQUES_ISSUED();
                                chequeIssuedModel.BILL_ID = maxBillId;
                                //Below Code commented on 07-12-2021
                                //if ((model.TXN_ID != "137$Q") && (model.TXN_ID != "834$Q") && (model.TXN_ID != "469$Q") && (model.CHQ_AMOUNT != 0) && !IsAdvicePayment)  //Modified by Abhishek for Advice no 6Apr2015
                                //{
                                //    try
                                //    {
                                //        chequeIssuedModel.BANK_CODE = GetBankCodeBasedOnChequeNumber(model.ND_CODE, model.FUND_TYPE, Convert.ToInt32(model.CHQ_NO));
                                //    }
                                //    catch (Exception ex)
                                //    {

                                //    }
                                //}
                                //else
                                //{
                                //    chequeIssuedModel.BANK_CODE = dbContext.ACC_BANK_DETAILS.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.ParentNDCode && x.FUND_TYPE == PMGSYSession.Current.FundType && x.BANK_ACC_STATUS == true).Select(z => z.BANK_CODE).FirstOrDefault();
                                //}

                                //Below Code Added on 07-12-2021
                                //if (PMGSYSession.Current.FundType == "P" || PMGSYSession.Current.FundType == "A")

                                //Below if condition added on 21-03-2023
                                if (PMGSYSession.Current.LevelId == 5 && TXN_Id == 3185)
                                {
                                    chequeIssuedModel.BANK_CODE = dbContext.ACC_BANK_DETAILS.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.ParentNDCode && x.FUND_TYPE == PMGSYSession.Current.FundType && x.BANK_ACC_STATUS == true && x.ACCOUNT_TYPE == "H").Select(z => z.BANK_CODE).FirstOrDefault();

                                }
                                else
                                {
                                    if (PMGSYSession.Current.FundType == "P" || PMGSYSession.Current.FundType == "A" || ((TXN_Id == 3050 || TXN_Id == 3051) && PMGSYSession.Current.FundType == "M"))
                                    {
                                        //chequeIssuedModelToUpdate.BANK_CODE = dbContext.ACC_BANK_DETAILS.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.ParentNDCode && x.FUND_TYPE == PMGSYSession.Current.FundType && x.BANK_ACC_STATUS == true).Select(z => z.BANK_CODE).FirstOrDefault();
                                        //***--***
                                        chequeIssuedModel.BANK_CODE = dbContext.ACC_BANK_DETAILS.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.ParentNDCode && x.FUND_TYPE == "P" && x.BANK_ACC_STATUS == true && x.ACCOUNT_TYPE == "S").Select(z => z.BANK_CODE).FirstOrDefault();
                                    }
                                    else
                                    {
                                        //***--***
                                        chequeIssuedModel.BANK_CODE = dbContext.ACC_BANK_DETAILS.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.ParentNDCode && x.FUND_TYPE == PMGSYSession.Current.FundType && x.BANK_ACC_STATUS == true && x.ACCOUNT_TYPE == "S").Select(z => z.BANK_CODE).FirstOrDefault();

                                    }
                                }

                                chequeIssuedModel.CHEQUE_STATUS = "N";

                                //Added By Abhishek Kamble 28-nov-2013
                                chequeIssuedModel.USERID = PMGSYSession.Current.UserId;
                                chequeIssuedModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                                dbContext.ACC_CHEQUES_ISSUED.Add(chequeIssuedModel);
                            }
                        }
                        else if (model.CHQ_EPAY.Trim().Equals("E"))
                        {
                            ACC_CHEQUES_ISSUED chequeIssuedModel = new ACC_CHEQUES_ISSUED();
                            chequeIssuedModel.BILL_ID = maxBillId;
                            chequeIssuedModel.BANK_CODE = bankCode;
                            chequeIssuedModel.CHEQUE_STATUS = "N";

                            //Added By Abhishek Kamble 28-nov-2013
                            chequeIssuedModel.USERID = PMGSYSession.Current.UserId;
                            chequeIssuedModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                            dbContext.ACC_CHEQUES_ISSUED.Add(chequeIssuedModel);
                        }
                    }
                    else
                    {
                        //if allready finalized 
                        if (dbContext.ACC_BILL_MASTER.Where(x => x.BILL_ID == Bill_id).Any())
                        {

                            if (dbContext.ACC_BILL_MASTER.Where(x => x.BILL_ID == Bill_id).Select(r => r.BILL_FINALIZED).FirstOrDefault() == "Y")
                            {
                                return -1;
                            }
                        }

                        /*if operation is edit check if old entry of type cheque or Epayment (changed on date 16/06/2012 05:11 pm  epayment  details are added in acc cheque issued table also)
                          1) if old entry is of cheque,we dont need to check if old and new cheque books is of same bank code if not update it.
                          2)if old entry is other than cheque entry we have to add the reference in  ACC_CHEQUES_ISSUED table
                         3)if old entry was of cheque but has been changed to epay/cash then  remove the reference from ACC_CHEQUES_ISSUED table
                         */

                        if (model.CHQ_EPAY.Trim().Equals("Q") || model.CHQ_EPAY.Trim().Equals("E"))
                        {

                            //case 1) check if for current bill id reference is already exist in table;if not add it with new bill id
                            if (!dbContext.ACC_CHEQUES_ISSUED.Where(x => x.BILL_ID == Bill_id).Any())
                            {
                                //old Modified By Abhishek kamble 12Nov2014 for TO add ACC_CHEQUES_ISSUED at SRRDA level
                                //if (model.CHQ_Book_ID != 0 && model.CHQ_EPAY.Trim().Equals("Q"))
                                if ((model.CHQ_Book_ID != 0 || PMGSYSession.Current.LevelId == 4 || IsAdvicePayment) && model.CHQ_EPAY.Trim().Equals("Q"))//Modified by Abhishek for Advice no 6Apr2015
                                {
                                    ACC_CHEQUES_ISSUED chequeIssuedModel = new ACC_CHEQUES_ISSUED();
                                    chequeIssuedModel.BILL_ID = Bill_id;

                                    //Below Code commented on 07-12-2021
                                    //if ((model.TXN_ID != "137$Q") && (model.TXN_ID != "834$Q") && (model.TXN_ID != "469$Q") && (model.CHQ_AMOUNT != 0) && !(IsAdvicePayment))//Modified by Abhishek for Advice no 6Apr2015
                                    //{
                                    //    chequeIssuedModel.BANK_CODE = GetBankCodeBasedOnChequeNumber(model.ND_CODE, model.FUND_TYPE, Convert.ToInt32(model.CHQ_NO));
                                    //}
                                    //else
                                    //{
                                    //    chequeIssuedModel.BANK_CODE = dbContext.ACC_BANK_DETAILS.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.ParentNDCode && x.FUND_TYPE == PMGSYSession.Current.FundType && x.BANK_ACC_STATUS == true).Select(z => z.BANK_CODE).FirstOrDefault();
                                    //}

                                    //Below Code Added on 07-12-2021
                                    //if (PMGSYSession.Current.FundType == "P" || PMGSYSession.Current.FundType == "A")
                                    //Below if condition added on 23-03-2023
                                    if (PMGSYSession.Current.LevelId == 5 && TXN_Id == 3185)
                                    {
                                        chequeIssuedModel.BANK_CODE = dbContext.ACC_BANK_DETAILS.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.ParentNDCode && x.FUND_TYPE == PMGSYSession.Current.FundType && x.BANK_ACC_STATUS == true && x.ACCOUNT_TYPE == "H").Select(z => z.BANK_CODE).FirstOrDefault();

                                    }
                                    else
                                    {
                                        if (PMGSYSession.Current.FundType == "P" || PMGSYSession.Current.FundType == "A" || ((TXN_Id == 3050 || TXN_Id == 3051) && PMGSYSession.Current.FundType == "M"))
                                        {
                                            //chequeIssuedModelToUpdate.BANK_CODE = dbContext.ACC_BANK_DETAILS.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.ParentNDCode && x.FUND_TYPE == PMGSYSession.Current.FundType && x.BANK_ACC_STATUS == true).Select(z => z.BANK_CODE).FirstOrDefault();
                                            //***--***
                                            chequeIssuedModel.BANK_CODE = dbContext.ACC_BANK_DETAILS.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.ParentNDCode && x.FUND_TYPE == "P" && x.BANK_ACC_STATUS == true && x.ACCOUNT_TYPE == "S").Select(z => z.BANK_CODE).FirstOrDefault();
                                        }
                                        else
                                        {
                                            //***--***
                                            chequeIssuedModel.BANK_CODE = dbContext.ACC_BANK_DETAILS.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.ParentNDCode && x.FUND_TYPE == PMGSYSession.Current.FundType && x.BANK_ACC_STATUS == true && x.ACCOUNT_TYPE == "S").Select(z => z.BANK_CODE).FirstOrDefault();

                                        }
                                    }

                                    //chequeIssuedModel.IS_CHQ_ENCASHED_NA 
                                    //chequeIssuedModel.NA_BILL_ID 
                                    //chequeIssuedModel.IS_CHQ_RECONCILE_BANK
                                    //chequeIssuedModel.CHQ_RECONCILE_DATE 
                                    //chequeIssuedModel.CHQ_RECONCILE_REMARKS 
                                    chequeIssuedModel.CHEQUE_STATUS = "N";

                                    //Added By Abhishek Kamble 28-nov-2013
                                    chequeIssuedModel.USERID = PMGSYSession.Current.UserId;
                                    chequeIssuedModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                                    dbContext.ACC_CHEQUES_ISSUED.Add(chequeIssuedModel);
                                }
                                else if (model.CHQ_EPAY.Trim().Equals("E"))
                                {
                                    ACC_CHEQUES_ISSUED chequeIssuedModel = new ACC_CHEQUES_ISSUED();
                                    chequeIssuedModel.BILL_ID = Bill_id;
                                    chequeIssuedModel.BANK_CODE = bankCode;
                                    chequeIssuedModel.CHEQUE_STATUS = "N";

                                    //Added By Abhishek Kamble 28-nov-2013
                                    chequeIssuedModel.USERID = PMGSYSession.Current.UserId;
                                    chequeIssuedModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                                    dbContext.ACC_CHEQUES_ISSUED.Add(chequeIssuedModel);
                                }
                            }
                            else
                            {
                                //old and new entry is of cheque/epay type but if cheque number/epaynumber is changed we have to update the bank code accordingly 
                                //old Modified By Abhishek kamble 12Nov2014 for TO add ACC_CHEQUES_ISSUED at SRRDA level                                
                                //if (model.CHQ_Book_ID != 0 && model.CHQ_EPAY.Trim().Equals("Q"))                      
                                if ((model.CHQ_Book_ID != 0 || PMGSYSession.Current.LevelId == 4 || IsAdvicePayment) && model.CHQ_EPAY.Trim().Equals("Q"))//Modified by Abhishek for Advice no 6Apr2015
                                {
                                    ACC_CHEQUES_ISSUED chequeIssuedModelToUpdate = new ACC_CHEQUES_ISSUED();
                                    chequeIssuedModelToUpdate.BILL_ID = Bill_id;


                                    //Below Code Added on 07-12-2021
                                    //if (PMGSYSession.Current.FundType == "P" || PMGSYSession.Current.FundType == "A")
                                    //Below if condition added on 23-03-2023
                                    if (PMGSYSession.Current.LevelId == 5 && TXN_Id == 3185)
                                    {
                                        chequeIssuedModelToUpdate.BANK_CODE = dbContext.ACC_BANK_DETAILS.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.ParentNDCode && x.FUND_TYPE == PMGSYSession.Current.FundType && x.BANK_ACC_STATUS == true && x.ACCOUNT_TYPE == "H").Select(z => z.BANK_CODE).FirstOrDefault();

                                    }
                                    else
                                    {
                                        if (PMGSYSession.Current.FundType == "P" || PMGSYSession.Current.FundType == "A" || ((TXN_Id == 3050 || TXN_Id == 3051) && PMGSYSession.Current.FundType == "M"))
                                        {
                                            //chequeIssuedModelToUpdate.BANK_CODE = dbContext.ACC_BANK_DETAILS.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.ParentNDCode && x.FUND_TYPE == PMGSYSession.Current.FundType && x.BANK_ACC_STATUS == true).Select(z => z.BANK_CODE).FirstOrDefault();
                                            //***--***
                                            chequeIssuedModelToUpdate.BANK_CODE = dbContext.ACC_BANK_DETAILS.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.ParentNDCode && x.FUND_TYPE == "P" && x.BANK_ACC_STATUS == true && x.ACCOUNT_TYPE == "S").Select(z => z.BANK_CODE).FirstOrDefault();
                                        }
                                        else
                                        {
                                            //***--***
                                            chequeIssuedModelToUpdate.BANK_CODE = dbContext.ACC_BANK_DETAILS.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.ParentNDCode && x.FUND_TYPE == PMGSYSession.Current.FundType && x.BANK_ACC_STATUS == true && x.ACCOUNT_TYPE == "S").Select(z => z.BANK_CODE).FirstOrDefault();

                                        }
                                    }

                                    //Below Code commented on 07-12-2021
                                    //if ((model.TXN_ID != "137$Q") && (model.TXN_ID != "834$Q") && (model.TXN_ID != "469$Q") && (model.CHQ_AMOUNT != 0) && !(IsAdvicePayment))//Modified by Abhishek for Advice no 6Apr2015
                                    //{
                                    //    chequeIssuedModelToUpdate.BANK_CODE = GetBankCodeBasedOnChequeNumber(model.ND_CODE, model.FUND_TYPE, Convert.ToInt32(model.CHQ_NO));
                                    //}
                                    //else
                                    //{
                                    //    chequeIssuedModelToUpdate.BANK_CODE = dbContext.ACC_BANK_DETAILS.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.ParentNDCode && x.FUND_TYPE == PMGSYSession.Current.FundType && x.BANK_ACC_STATUS == true).Select(z => z.BANK_CODE).FirstOrDefault();
                                    //}
                                    chequeIssuedModelToUpdate.CHEQUE_STATUS = "N";

                                    //Added By Abhishek Kamble 28-nov-2013
                                    chequeIssuedModelToUpdate.USERID = PMGSYSession.Current.UserId;
                                    chequeIssuedModelToUpdate.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];


                                    ACC_CHEQUES_ISSUED old_model = new ACC_CHEQUES_ISSUED();
                                    old_model = dbContext.ACC_CHEQUES_ISSUED.Where(x => x.BILL_ID == Bill_id).FirstOrDefault();
                                    dbContext.Entry(old_model).CurrentValues.SetValues(chequeIssuedModelToUpdate);
                                }
                                else if (model.CHQ_EPAY.Trim().Equals("E"))
                                {
                                    ACC_CHEQUES_ISSUED chequeIssuedModel = new ACC_CHEQUES_ISSUED();
                                    chequeIssuedModel.BILL_ID = Bill_id;
                                    chequeIssuedModel.BANK_CODE = bankCode;
                                    chequeIssuedModel.CHEQUE_STATUS = "N";

                                    //Added By Abhishek Kamble 28-nov-2013
                                    chequeIssuedModel.USERID = PMGSYSession.Current.UserId;
                                    chequeIssuedModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                                    ACC_CHEQUES_ISSUED old_model = new ACC_CHEQUES_ISSUED();
                                    old_model = dbContext.ACC_CHEQUES_ISSUED.Where(x => x.BILL_ID == Bill_id).FirstOrDefault();
                                    dbContext.Entry(old_model).CurrentValues.SetValues(chequeIssuedModel);
                                }
                                else
                                {
                                    //Added By Abhishek Kamble 28-nov-2013
                                    ACC_CHEQUES_ISSUED chequesIssued = dbContext.ACC_CHEQUES_ISSUED.Where(m => m.BILL_ID == Bill_id).FirstOrDefault();
                                    if (chequesIssued != null)
                                    {

                                        chequesIssued.USERID = PMGSYSession.Current.UserId;
                                        chequesIssued.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                                        dbContext.Entry(chequesIssued).State = System.Data.Entity.EntityState.Modified;
                                        dbContext.SaveChanges();
                                    }

                                    //At SRRDA Level old cheque details are in  ACC_CHEQUES_ISSUED & new cheque book details are not entered (as its not compulsive)
                                    // delete the old reference from  ACC_CHEQUES_ISSUED 
                                    dbContext.Database.ExecuteSqlCommand("DELETE [omms].ACC_CHEQUES_ISSUED Where BILL_ID = {0}", Bill_id);
                                }

                            }

                        }
                        else
                        {
                            //Added By Abhishek Kamble 28-nov-2013
                            ACC_CHEQUES_ISSUED chequesIssued = dbContext.ACC_CHEQUES_ISSUED.Where(m => m.BILL_ID == Bill_id).FirstOrDefault();
                            if (chequesIssued != null)
                            {

                                chequesIssued.USERID = PMGSYSession.Current.UserId;
                                chequesIssued.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                                dbContext.Entry(chequesIssued).State = System.Data.Entity.EntityState.Modified;
                                dbContext.SaveChanges();
                            }
                            //new record is of type cash old entry is of the cheque/epay; remove its reference in  ACC_CHEQUES_ISSUED table
                            dbContext.Database.ExecuteSqlCommand("DELETE [omms].ACC_CHEQUES_ISSUED Where BILL_ID = {0}", Bill_id);

                        }

                        ModelToAdd.BILL_NO = dbContext.ACC_BILL_MASTER.Where(c => c.BILL_ID == Bill_id).Select(x => x.BILL_NO).FirstOrDefault();

                    }


                    ModelToAdd.BILL_MONTH = (byte)model.BILL_MONTH; //find out in controller
                    ModelToAdd.BILL_YEAR = model.BILL_YEAR; //find out in controller
                    ModelToAdd.BILL_DATE = objCommon.GetStringToDateTime(model.BILL_DATE);
                    ModelToAdd.TXN_ID = Convert.ToInt16(model.TXN_ID.Split('$')[0]);

                    if (model.CHQ_EPAY.Trim().Equals("Q"))
                    {
                        ModelToAdd.CHQ_Book_ID = model.CHQ_Book_ID == 0 ? null : model.CHQ_Book_ID;
                    }

                    ModelToAdd.CHQ_NO = model.CHQ_NO;

                    if (model.CHQ_DATE == null || model.CHQ_DATE == string.Empty)
                    {
                        ModelToAdd.CHQ_DATE = null;
                    }
                    else
                    {
                        ModelToAdd.CHQ_DATE = objCommon.GetStringToDateTime(model.CHQ_DATE);
                    }


                    ModelToAdd.CHQ_AMOUNT = model.CHQ_AMOUNT.HasValue ? model.CHQ_AMOUNT.Value : 0;
                    ModelToAdd.CASH_AMOUNT = model.CASH_AMOUNT.HasValue ? model.CASH_AMOUNT.Value : 0;
                    ModelToAdd.GROSS_AMOUNT = model.GROSS_AMOUNT.HasValue ? model.GROSS_AMOUNT.Value : 0; //calculated in controller
                    ModelToAdd.CHALAN_NO = model.CHALAN_NO;

                    if (model.CHALAN_DATE == null || model.CHALAN_DATE == string.Empty)
                    {
                        ModelToAdd.CHALAN_DATE = null;
                    }
                    else
                    {
                        ModelToAdd.CHALAN_DATE = objCommon.GetStringToDateTime(model.CHALAN_DATE);
                    }

                    ModelToAdd.PAYEE_NAME = model.PAYEE_NAME; //found out payee name based on the transaction type in controller

                    //Added By Abhishek kamble for Advice No 7Apr2015 start                    
                    if (IsAdvicePayment)
                    {
                        ModelToAdd.CHQ_EPAY = "A";
                    }
                    else
                    {
                        ModelToAdd.CHQ_EPAY = model.CHQ_EPAY;
                    }
                    //Added By Abhishek kamble for Advice No 7Apr2015 end  

                    ModelToAdd.TEO_TRANSFER_TYPE = null;

                    if (model.REMIT_TYPE.HasValue)
                    {
                        ModelToAdd.REMIT_TYPE = (byte)model.REMIT_TYPE.Value;
                    }
                    ModelToAdd.BILL_FINALIZED = "N";
                    ModelToAdd.FUND_TYPE = model.FUND_TYPE;
                    ModelToAdd.ADMIN_ND_CODE = model.ND_CODE;
                    ModelToAdd.LVL_ID = (byte)model.LVL_ID;

                    //if (Convert.ToInt32(model.TXN_ID.Split('$')[0]) == 472 || Convert.ToInt32(model.TXN_ID.Split('$')[0]) == 415)
                    //{
                    //    if (!(model.CON_ID == 0 || model.CON_ID == -1))
                    //    {
                    //        ModelToAdd.ADMIN_NO_OFFICER_CODE = model.CON_ID;
                    //    }
                    //}
                    //else
                    //{
                    //if (!(Convert.ToInt32(model.TXN_ID.Split('$')[0]) == 455))
                    //{
                    //    ModelToAdd.MAST_CON_ID = model.CON_ID;
                    //}
                    //else
                    //{
                    //    if (!(model.CON_ID == 0 || model.CON_ID == -1))
                    //    {
                    //        ModelToAdd.MAST_CON_ID = model.CON_ID;
                    //    }
                    //}
                    // }

                    ModelToAdd.MAST_CON_ID = model.CON_ID;
                    ModelToAdd.BILL_TYPE = "P";

                    //Added By Abhishek Kamble 28-nov-2013
                    ModelToAdd.USERID = PMGSYSession.Current.UserId;
                    ModelToAdd.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    //Below Code Commented on 25-10-2021 to Add 'CON_ACCOUNT_ID' for Admin Fund Also
                    //if (PMGSYSession.Current.FundType == "P" || PMGSYSession.Current.FundType == "M")
                    //{
                    //    ModelToAdd.CON_ACCOUNT_ID = model.conAccountId;
                    //}

                    //Below Code Added on 25-10-2021 to Add 'CON_ACCOUNT_ID' for Admin Fund Also
                    if (PMGSYSession.Current.FundType == "P" || PMGSYSession.Current.FundType == "M" || PMGSYSession.Current.FundType == "A")
                    {
                        ModelToAdd.CON_ACCOUNT_ID = model.conAccountId;
                    }


                    if (operationType.Equals("A"))
                    {
                        dbContext.ACC_BILL_MASTER.Add(ModelToAdd);


                        int fiscalYear = 0;
                        if (ModelToAdd.BILL_MONTH <= 3)
                        {
                            fiscalYear = (ModelToAdd.BILL_YEAR - 1);
                        }
                        else
                        {
                            fiscalYear = ModelToAdd.BILL_YEAR;
                        }


                        ACC_VOUCHER_NUMBER_MASTER oldVoucherNumberModel = new ACC_VOUCHER_NUMBER_MASTER();
                        oldVoucherNumberModel = dbContext.ACC_VOUCHER_NUMBER_MASTER.Where(x => x.ADMIN_ND_CODE == ModelToAdd.ADMIN_ND_CODE && x.FUND_TYPE == ModelToAdd.FUND_TYPE && x.BILL_TYPE == ModelToAdd.BILL_TYPE && x.FISCAL_YEAR == fiscalYear).FirstOrDefault();
                        ACC_VOUCHER_NUMBER_MASTER newMvoucherNumberModel = new ACC_VOUCHER_NUMBER_MASTER();

                        newMvoucherNumberModel.ADMIN_ND_CODE = ModelToAdd.ADMIN_ND_CODE;
                        newMvoucherNumberModel.FUND_TYPE = ModelToAdd.FUND_TYPE;
                        newMvoucherNumberModel.BILL_TYPE = ModelToAdd.BILL_TYPE;
                        newMvoucherNumberModel.FISCAL_YEAR = fiscalYear;
                        newMvoucherNumberModel.SLNO = oldVoucherNumberModel.SLNO + 1;

                        dbContext.Entry(oldVoucherNumberModel).CurrentValues.SetValues(newMvoucherNumberModel);

                        dbContext.SaveChanges();
                        scope.Complete();
                        return maxBillId;


                        //ACC_VOUCHER_NUMBER_MASTER oldVoucherNumberModel = dbContext.ACC_VOUCHER_NUMBER_MASTER.Find(ModelToAdd.ADMIN_ND_CODE, ModelToAdd.FUND_TYPE, ModelToAdd.BILL_TYPE, fiscalYear);

                        //oldVoucherNumberModel.ADMIN_ND_CODE = ModelToAdd.ADMIN_ND_CODE;
                        //oldVoucherNumberModel.FUND_TYPE = ModelToAdd.FUND_TYPE;
                        //oldVoucherNumberModel.BILL_TYPE = ModelToAdd.BILL_TYPE;
                        //oldVoucherNumberModel.FISCAL_YEAR = fiscalYear;
                        //oldVoucherNumberModel.SLNO = oldVoucherNumberModel.SLNO + 1;


                        //dbContext.Entry(oldVoucherNumberModel).State = System.Data.Entity.EntityState.Modified;
                        //dbContext.SaveChanges();
                        //scope.Complete();
                        //return maxBillId;
                    }
                    else
                    {
                        ACC_BILL_MASTER old_model = new ACC_BILL_MASTER();
                        old_model = dbContext.ACC_BILL_MASTER.Where(x => x.BILL_ID == Bill_id).FirstOrDefault();
                        dbContext.Entry(old_model).CurrentValues.SetValues(ModelToAdd);


                        dbContext.SaveChanges();
                        scope.Complete();
                        return Bill_id;

                    }

                }

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error while adding /updating payment master details. ");

            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public String GetChequeBookIssueDate(Int64 chqBookId)
        {
            dbContext = new PMGSYEntities();
            CommonFunctions obj = new CommonFunctions();
            try
            {
                return obj.GetDateTimeToString(dbContext.ACC_CHQ_BOOK_DETAILS.Where(m => m.CHQ_BOOK_ID == chqBookId).Select(s => s.ISSUE_DATE).FirstOrDefault());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error while getting cheque book details. ");

            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// function to get cheque book series
        /// </summary>
        /// <param name="admin_ND_Code"> Admin_ND_Code</param>
        /// <returns></returns>
        //public List<SelectListItem> GetchequebookSeries(int admin_ND_Code, string fundType, Int16 levelID)
        //{
        //    try
        //    {
        //        dbContext = new PMGSYEntities();
        //        List<SelectListItem> lstChequebook = null;


        //        //var Cheques = (from result in dbContext.ACC_CHQ_BOOK_DETAILS
        //        //               where result.ADMIN_ND_CODE == admin_ND_Code && result.FUND_TYPE == fundType && result.LVL_ID == levelID && (result.IS_CHQBOOK_COMPLETED == null || result.IS_CHQBOOK_COMPLETED == false) //IS_CHQBOOK_COMPLETED==false is added by abhishek kamble 20Mar2015
        //        //               select new
        //        //               {
        //        //                   CHQ_BOOK_ID = result.CHQ_BOOK_ID,
        //        //                   chq_series = result.LEAF_START + "-" + result.LEAF_END

        //        //               });

        //        var Cheques = (from result in dbContext.ACC_CHQ_BOOK_DETAILS
        //                       where result.ADMIN_ND_CODE == admin_ND_Code && result.LVL_ID == levelID && (result.IS_CHQBOOK_COMPLETED == null || result.IS_CHQBOOK_COMPLETED == false) //IS_CHQBOOK_COMPLETED==false is added by abhishek kamble 20Mar2015
        //                       select new
        //                       {
        //                           CHQ_BOOK_ID = result.CHQ_BOOK_ID,
        //                           chq_series = result.LEAF_START + "-" + result.LEAF_END

        //                       });

        //        lstChequebook = new List<SelectListItem>();

        //        foreach (var item in Cheques)
        //        {
        //            SelectListItem obj = new SelectListItem();
        //            obj.Text = item.chq_series;
        //            obj.Value = item.CHQ_BOOK_ID.ToString();
        //            obj.Selected = false;
        //            lstChequebook.Add(obj);
        //        }

        //        lstChequebook.Insert(0, (new SelectListItem { Text = "--Select--", Value = "0", Selected = true }));
        //        return lstChequebook;

        //    }
        //    catch (Exception ex)
        //    {

        //        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
        //        throw new Exception("Error while getting cheque book series. ");
        //    }
        //    finally
        //    {
        //        dbContext.Dispose();
        //    }
        //}


        public List<SelectListItem> GetchequebookSeries(int admin_ND_Code, string fundType, Int16 levelID)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<SelectListItem> lstChequebook = null;
                lstChequebook = new List<SelectListItem>();

                if (PMGSYSession.Current.FundType == "P" )
                {
                    var Cheques = (from result in dbContext.ACC_CHQ_BOOK_DETAILS
                                   where result.ADMIN_ND_CODE == admin_ND_Code && result.FUND_TYPE == fundType && result.LVL_ID == levelID && (result.IS_CHQBOOK_COMPLETED == null || result.IS_CHQBOOK_COMPLETED == false) //IS_CHQBOOK_COMPLETED==false is added by abhishek kamble 20Mar2015
                                   select new
                                   {
                                       CHQ_BOOK_ID = result.CHQ_BOOK_ID,
                                       chq_series = result.LEAF_START + "-" + result.LEAF_END,
                                       Account_Type = result.ACC_TYPE
                                   });

                    foreach (var item in Cheques)
                    {
                        string type = item.Account_Type == "S" ? "Saving" : item.Account_Type == "H" ? "Holding" : item.Account_Type == "D" ? "Security Deposit" : "-";
                        SelectListItem obj = new SelectListItem();
                        obj.Text = item.chq_series + " (" + type + ")"; 
                        obj.Value = item.CHQ_BOOK_ID.ToString();
                        obj.Selected = false;
                        lstChequebook.Add(obj);
                    }
                }
                else if (PMGSYSession.Current.FundType == "A" )
                {
                    var Cheques = (from result in dbContext.ACC_CHQ_BOOK_DETAILS
                                   where result.ADMIN_ND_CODE == admin_ND_Code && (result.FUND_TYPE == "P" || result.FUND_TYPE == "A") && result.LVL_ID == levelID && (result.IS_CHQBOOK_COMPLETED == null || result.IS_CHQBOOK_COMPLETED == false) //IS_CHQBOOK_COMPLETED==false is added by abhishek kamble 20Mar2015
                                   select new
                                   {
                                       CHQ_BOOK_ID = result.CHQ_BOOK_ID,
                                       chq_series = result.LEAF_START + "-" + result.LEAF_END

                                   });

                    foreach (var item in Cheques)
                    {
                        SelectListItem obj = new SelectListItem();
                        obj.Text = item.chq_series;
                        obj.Value = item.CHQ_BOOK_ID.ToString();
                        obj.Selected = false;
                        lstChequebook.Add(obj);
                    }
                }
                else if (PMGSYSession.Current.FundType == "M")
                {
                    var Cheques = (from result in dbContext.ACC_CHQ_BOOK_DETAILS
                                   where result.ADMIN_ND_CODE == admin_ND_Code && (result.FUND_TYPE == "P" || result.FUND_TYPE == "M") && result.LVL_ID == levelID && (result.IS_CHQBOOK_COMPLETED == null || result.IS_CHQBOOK_COMPLETED == false) //IS_CHQBOOK_COMPLETED==false is added by abhishek kamble 20Mar2015
                                   select new
                                   {
                                       CHQ_BOOK_ID = result.CHQ_BOOK_ID,
                                       chq_series = result.LEAF_START + "-" + result.LEAF_END

                                   });

                    foreach (var item in Cheques)
                    {
                        SelectListItem obj = new SelectListItem();
                        obj.Text = item.chq_series;
                        obj.Value = item.CHQ_BOOK_ID.ToString();
                        obj.Selected = false;
                        lstChequebook.Add(obj);
                    }
                }




                lstChequebook.Insert(0, (new SelectListItem { Text = "--Select--", Value = "0", Selected = true }));
                return lstChequebook;

            }
            catch (Exception ex)
            {

                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error while getting cheque book series. ");
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        /// <summary>
        /// function to get the bank code based on the cheque number
        /// </summary>
        /// <param name="admin_nd_code"></param>
        /// <param name="fund_type"></param>
        /// <param name="chequeNumber"></param>
        /// <returns>Bank code </returns>
        public Int16 GetBankCodeBasedOnChequeNumber(Int32 admin_nd_code, String fund_type, Int32 chequeNumber)
        {

            PMGSYEntities dbContextLocal = new PMGSYEntities();

            try
            {

                Int16 bank_code = 0;

                //if DPIU level agency find out bank code based on cheque number (bacouse later bank may change )as per discussed with anita mam date:23/05/2013 time 6:25 pm
                if (PMGSYSession.Current.LevelId == 5)
                {
                    List<SelectListItem> lstChequebookNumbers = new List<SelectListItem>();

                    List<String> ListChequeBooksStartEndLeaf = (from result in dbContextLocal.ACC_CHQ_BOOK_DETAILS
                                                                where result.FUND_TYPE == fund_type && result.ADMIN_ND_CODE == admin_nd_code && (result.IS_CHQBOOK_COMPLETED == null || result.IS_CHQBOOK_COMPLETED == false) //IS_CHQBOOK_COMPLETED==false is added by abhishek kamble 20Mar2015
                                                                select result.LEAF_START + "$" + result.LEAF_END).ToList<String>();

                    foreach (var Chequeitem in ListChequeBooksStartEndLeaf)
                    {
                        int startLeaf = Convert.ToInt32(Chequeitem.ToString().Split('$')[0]);

                        int endLeaf = Convert.ToInt32(Chequeitem.ToString().Split('$')[1]);

                        IEnumerable<Int32> ChequeNumbers = Enumerable.Range(startLeaf, (endLeaf - startLeaf) + 1);

                        bool chequeExistInRange = Enumerable.Range(startLeaf, (endLeaf - startLeaf) + 1).Contains(chequeNumber);

                        string strStartLeaf = startLeaf.ToString().PadLeft(6, '0');

                        if (chequeExistInRange)
                        {
                            string intStartLeaf = startLeaf.ToString();
                            bank_code = dbContextLocal.ACC_CHQ_BOOK_DETAILS.Where(x => x.LEAF_START == strStartLeaf && x.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && x.FUND_TYPE == fund_type).Select(y => y.BANK_CODE).FirstOrDefault();
                            break;
                        }

                    }
                }
                else if (PMGSYSession.Current.LevelId == 4)
                {

                    //for SRRDA level agency take it from acc_bank_details as per discussed with anita mam date:23/05/2013 time 6:25 pm
                    bank_code = dbContext.ACC_BANK_DETAILS.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && x.FUND_TYPE == fund_type && x.BANK_ACC_STATUS == true && x.ACCOUNT_TYPE == "S").Select(z => z.BANK_CODE).FirstOrDefault();


                }
                if (bank_code == 0)
                {
                    throw new Exception("Bank code not found.");
                }

                return bank_code;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PaymentDAL.GetBankCodeBasedOnChequeNumber");
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error while getting bank code.");
            }
            finally
            {
                dbContextLocal.Dispose();
            }
        }


        /// <summary>
        /// function to find the cheqie book series based on the cheque number
        /// </summary>
        /// <param name="admin_nd_code"></param>
        /// <param name="fund_type"></param>
        /// <param name="chequeNumber"></param>
        /// <returns></returns>
        public string GetChequeBookSeriesBasedOnCheque(Int32 admin_nd_code, String fund_type, Int32 chequeNumber)
        {
            PMGSYEntities dbContextLocal = new PMGSYEntities();

            try
            {

                String ChequeBook_Range = string.Empty;
                int chqBookId = 0;

                //if DPIU level agency find out bank code based on cheque number (bacouse later bank may change )as per discussed with anita mam date:23/05/2013 time 6:25 pm
                //if (PMGSYSession.Current.LevelId == 5)
                // {
                List<SelectListItem> lstChequebookNumbers = new List<SelectListItem>();

                List<String> ListChequeBooksStartEndLeaf = (from result in dbContextLocal.ACC_CHQ_BOOK_DETAILS
                                                            where result.FUND_TYPE == fund_type && result.ADMIN_ND_CODE == admin_nd_code && (result.IS_CHQBOOK_COMPLETED == null || result.IS_CHQBOOK_COMPLETED == false) //IS_CHQBOOK_COMPLETED==false is added by abhishek kamble 20Mar2015
                                                            select result.LEAF_START + "$" + result.LEAF_END).ToList<String>();


                if (ListChequeBooksStartEndLeaf.Count == 0 && PMGSYSession.Current.LevelId == 5)
                {
                    return "--Select --" + "$" + "0";
                }
                else if (ListChequeBooksStartEndLeaf.Count == 0 && PMGSYSession.Current.LevelId == 4)
                {
                    throw new Exception("Cheque Book Series not found.");
                }

                foreach (var Chequeitem in ListChequeBooksStartEndLeaf)
                {
                    int startLeaf = Convert.ToInt32(Chequeitem.ToString().Split('$')[0]);

                    int endLeaf = Convert.ToInt32(Chequeitem.ToString().Split('$')[1]);

                    IEnumerable<Int32> ChequeNumbers = Enumerable.Range(startLeaf, (endLeaf - startLeaf) + 1);

                    bool chequeExistInRange = Enumerable.Range(startLeaf, (endLeaf - startLeaf) + 1).Contains(chequeNumber);

                    if (chequeExistInRange)
                    {
                        ChequeBook_Range = startLeaf + "-" + endLeaf;


                        //changes by Koustubh Nakate on 23/09/2013 for padding 0's to start leaf and end leaf
                        string intStartLeaf = startLeaf.ToString().PadLeft(6, '0');
                        string intEndLeaf = endLeaf.ToString().PadLeft(6, '0');

                        chqBookId = dbContextLocal.ACC_CHQ_BOOK_DETAILS.Where(x => x.LEAF_START == intStartLeaf && x.LEAF_END == intEndLeaf && x.ADMIN_ND_CODE == admin_nd_code && x.FUND_TYPE == fund_type).Select(y => y.CHQ_BOOK_ID).FirstOrDefault();
                        break;


                    }

                }
                //}

                if (ChequeBook_Range == "")
                {
                    throw new Exception("Cheque Book Series not found.");
                }

                return ChequeBook_Range + "$" + chqBookId.ToString();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Cheque Book Series not found.");
            }
            finally
            {
                dbContextLocal.Dispose();
            }


        }




        /// <summary>
        /// function to get the chque book numbers which are not issued (& so cancelled)
        /// </summary>
        /// <param name="chequeBookId">cheque book</param>
        /// <returns></returns>
        //public List<SelectListItem> GetchequebookNumbers(int chequeBookId,int admin_nd_code,string fund_type)
        //{
        //    try
        //    {
        //        dbContext = new PMGSYEntities();
        //        List<SelectListItem> lstChequebookNumbers = new List<SelectListItem>();


        //          List<String> ListChequeBooksStartEndLeaf = (from result in dbContext.ACC_CHQ_BOOK_DETAILS
        //                                          where result.FUND_TYPE == fund_type && result.ADMIN_ND_CODE == admin_nd_code
        //                                               select result.LEAF_START + "$" + result.LEAF_END).ToList<String>();


        //          foreach (var Chequeitem in ListChequeBooksStartEndLeaf)
        //          {

        //              int startLeaf = Convert.ToInt32(Chequeitem.ToString().Split('$')[0]);
        //              int endLeaf = Convert.ToInt32(Chequeitem.ToString().Split('$')[1]);

        //            IEnumerable<Int32> ChequeNumbers = Enumerable.Range(startLeaf, (endLeaf - startLeaf) + 1);


        //            IEnumerable<Int32> ChequeNumberToRemove = dbContext.ACC_BILL_MASTER.Where(g => g.ADMIN_ND_CODE == admin_nd_code && g.CHQ_EPAY.Trim().Equals("Q") && g.BILL_TYPE.Equals("P")).ToList<ACC_BILL_MASTER>()
        //                                       .FindAll(chquelist => Convert.ToInt32(chquelist.CHQ_NO) >= startLeaf && Convert.ToInt32(chquelist.CHQ_NO) <= endLeaf)
        //                                       .Select(x => Convert.ToInt32(x.CHQ_NO));


        //            ChequeNumbers = ChequeNumbers.Except(ChequeNumberToRemove);


        //            foreach (var item in ChequeNumbers)
        //            {
        //                SelectListItem obj = new SelectListItem();
        //                obj.Text = item.ToString();
        //                obj.Value = item.ToString();
        //                obj.Selected = false;
        //                lstChequebookNumbers.Add(obj);
        //            }

        //         }

        //       lstChequebookNumbers.Insert(0, (new SelectListItem { Text = "--Select--", Value = "0", Selected = true }));
        //      return lstChequebookNumbers;


        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
        //    finally
        //    {
        //        dbContext.Dispose();
        //    }
        //}

        /// <summary>
        /// DAL function to get the available cheques for data entry
        /// </summary>
        /// <param name="chequeBookId"></param>
        /// <param name="admin_nd_code"></param>
        /// <param name="fund_type"></param>
        /// <returns></returns>
        //public String[] GetAllAvailableChequesArray(int chequeBookId, int admin_nd_code, string fund_type, string operation = "A", Int64 billID = 0) //added parameter by Koustubh Nakate on 20/09/2013 for operation 
        //{
        //    try
        //    {
        //        dbContext = new PMGSYEntities();
        //        List<String> lstChequebookNumbers = new List<String>();
        //        List<String> ListChequeBooksStartEndLeaf = new List<string>();
        //        if (chequeBookId != -1)
        //        {
        //            ListChequeBooksStartEndLeaf = (from result in dbContext.ACC_CHQ_BOOK_DETAILS
        //                                           where result.FUND_TYPE == fund_type && result.ADMIN_ND_CODE == admin_nd_code && result.CHQ_BOOK_ID == chequeBookId && result.IS_CHQBOOK_COMPLETED == null
        //                                           select result.LEAF_START + "$" + result.LEAF_END).ToList<String>();
        //        }
        //        else
        //        {
        //            ListChequeBooksStartEndLeaf = (from result in dbContext.ACC_CHQ_BOOK_DETAILS
        //                                           where result.FUND_TYPE == fund_type && result.ADMIN_ND_CODE == admin_nd_code && result.IS_CHQBOOK_COMPLETED == null
        //                                           select result.LEAF_START + "$" + result.LEAF_END).ToList<String>();
        //        }

        //        foreach (var Chequeitem in ListChequeBooksStartEndLeaf)
        //        {

        //            int startLeaf = Convert.ToInt32(Chequeitem.ToString().Split('$')[0]);
        //            int endLeaf = Convert.ToInt32(Chequeitem.ToString().Split('$')[1]);

        //            IEnumerable<Int32> ChequeNumbers = Enumerable.Range(startLeaf, (endLeaf - startLeaf) + 1);

        //            IEnumerable<Int32> ChequeNumberToRemove = null;

        //            //added if condition by Koustubh Nakate on 20/09/2013 

        //            //g.CHQ_NO!="null" is checked by Abhishek kamble 3-June-2014
        //            if (operation.Equals("E"))
        //            {
        //                string chequeNO = dbContext.ACC_BILL_MASTER.Where(bm => bm.BILL_ID == billID).Select(bm => bm.CHQ_NO).FirstOrDefault();

        //                ChequeNumberToRemove = dbContext.ACC_BILL_MASTER.Where(g => g.ADMIN_ND_CODE == admin_nd_code && g.CHQ_EPAY.Trim().Equals("Q") && g.BILL_TYPE.Equals("P") && g.CHQ_NO != null && g.FUND_TYPE == fund_type && g.CHQ_NO != chequeNO && g.CHQ_NO != "null").ToList<ACC_BILL_MASTER>()
        //                 .FindAll(chquelist => chquelist.CHQ_NO != null ? Convert.ToInt32(chquelist.CHQ_NO) >= startLeaf && Convert.ToInt32(chquelist.CHQ_NO) <= endLeaf : 1 == 1)
        //                                        .Select(x => Convert.ToInt32(x.CHQ_NO));
        //            }
        //            else
        //            {
        //                //ChequeNumberToRemove = dbContext.ACC_BILL_MASTER.Where(g => g.ADMIN_ND_CODE == admin_nd_code && g.CHQ_EPAY.Trim().Equals("Q") && g.BILL_TYPE.Equals("P") && g.CHQ_NO != null && g.FUND_TYPE == fund_type).ToList<ACC_BILL_MASTER>()
        //                //    .FindAll(chquelist => chquelist.CHQ_NO != null ? Convert.ToInt32(chquelist.CHQ_NO) >= startLeaf && Convert.ToInt32(chquelist.CHQ_NO) <= endLeaf : 1 == 1)
        //                //                           .Select(x => Convert.ToInt32(x.CHQ_NO));


        //                ChequeNumberToRemove = dbContext.ACC_BILL_MASTER.Where(g => g.ADMIN_ND_CODE == admin_nd_code && g.CHQ_EPAY.Trim().Equals("Q") && g.BILL_TYPE.Equals("P") && g.CHQ_NO != null && g.FUND_TYPE == fund_type && g.CHQ_NO != "null").ToList<ACC_BILL_MASTER>()
        //                    .FindAll(chquelist => (chquelist.CHQ_NO != null) ? Convert.ToInt32(chquelist.CHQ_NO) >= startLeaf && Convert.ToInt32(chquelist.CHQ_NO) <= endLeaf : 1 == 1)
        //                                           .Select(x => Convert.ToInt32(x.CHQ_NO));


        //            }

        //            ChequeNumbers = ChequeNumbers.Except(ChequeNumberToRemove);



        //            foreach (var item in ChequeNumbers)
        //            {

        //                lstChequebookNumbers.Add(item.ToString().PadLeft(6, '0'));

        //            }

        //        }


        //        return lstChequebookNumbers.ToArray();


        //    }
        //    catch (Exception ex)
        //    {
        //        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
        //        throw new Exception("Error while getting available cheques.");
        //    }
        //    finally
        //    {
        //        dbContext.Dispose();
        //    }
        //}

        //Added By Abhishek kamble to get Cheque book No from SP 3 Dec 2014
        //DAL function to get the available cheques for data entry
        public String[] GetAllAvailableChequesArray(int chequeBookId, int admin_nd_code, string fund_type, string operation = "A", Int64 billID = 0) //added parameter by Koustubh Nakate on 20/09/2013 for operation 
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<String> lstChequebookNumbers = new List<String>();
                lstChequebookNumbers = dbContext.USP_ACC_GET_ALL_AVAILABLE_CHEQUES(chequeBookId, admin_nd_code, fund_type, operation, billID).ToList<String>();
                return lstChequebookNumbers.ToArray();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error while getting available cheques.");
            }
            finally
            {
                dbContext.Dispose();
            }
        }





        /// <summary>
        /// function to get all finalized cheques in given cheque series
        /// </summary>
        /// <param name="chequeBookId"></param>
        /// <param name="admin_nd_code"></param>
        /// <param name="fund_type"></param>
        /// <param name="chequeSeries"></param>
        /// <returns></returns>
        public List<SelectListItem> GetAllFinalizedCheques(int chequeBookId, int admin_nd_code, string fund_type, string chequeSeries)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<SelectListItem> lstChequebookNumbers = new List<SelectListItem>();

                if (chequeBookId != -1 && chequeSeries == String.Empty)
                {
                    chequeSeries = (from result in dbContext.ACC_CHQ_BOOK_DETAILS
                                    where result.FUND_TYPE == fund_type && result.ADMIN_ND_CODE == admin_nd_code && result.CHQ_BOOK_ID == chequeBookId
                                    select result.LEAF_START + "-" + result.LEAF_END).First();
                }


                int startLeaf = Convert.ToInt32(chequeSeries.Split('-')[0]);
                int endLeaf = Convert.ToInt32(chequeSeries.Split('-')[1]);

                //find all renewed cheques for the DPIU and for fund type


                var query = from t1 in dbContext.ACC_CANCELLED_CHEQUES
                            join t2 in dbContext.ACC_BILL_MASTER on t1.OLD_BILL_ID equals t2.BILL_ID
                            where t2.ADMIN_ND_CODE == admin_nd_code && t2.CHQ_EPAY == "Q" && t2.FUND_TYPE == fund_type
                            select new { t2.CHQ_NO };

                List<Int32> CancelledCheques = new List<Int32>();

                foreach (var item in query)
                {
                    CancelledCheques.Add(Convert.ToInt32(item.CHQ_NO));
                }


                IEnumerable<Int32> ChequeNumbers = dbContext.ACC_BILL_MASTER.Where
                                    (g => g.ADMIN_ND_CODE == admin_nd_code
                                        && g.CHQ_EPAY.Trim().Equals("Q")
                                        && g.BILL_TYPE.Equals("P")
                                        && g.BILL_FINALIZED == "Y"
                                        && g.CHQ_NO != null
                                        ).ToList<ACC_BILL_MASTER>()
                                    .FindAll(chquelist => Convert.ToInt32(chquelist.CHQ_NO) >= startLeaf
                                        && Convert.ToInt32(chquelist.CHQ_NO) <= endLeaf)
                                    .Select(x => Convert.ToInt32(x.CHQ_NO));

                IEnumerable<Int32> Cheques = null;

                if (CancelledCheques != null)
                {
                    Cheques = ChequeNumbers.Except(CancelledCheques);
                }

                foreach (var item in Cheques)
                {

                    lstChequebookNumbers.Add(new SelectListItem { Text = item.ToString().PadLeft(6, '0'), Value = item.ToString().PadLeft(6, '0') });

                }

                lstChequebookNumbers.Insert(0, new SelectListItem { Text = "--Select--", Value = "0", Selected = true });

                return lstChequebookNumbers;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error while getting available cheques.");
            }
            finally
            {
                dbContext.Dispose();
            }
        }



        /// <summary>
        /// DAL Action to Get the payment closing balance
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        //public UDF_ACC_GEN_GET_BA_CASH_CLOSING_BALANCES_Result GetCloSingBalanceForPayment(TransactionParams param)
        public UDF_ACC_GEN_GET_BA_CASH_CLOSING_BALANCES_COMPOSITE_Result GetCloSingBalanceForPayment(TransactionParams param)
        {
            try
            {
                dbContext = new PMGSYEntities();

                //UDF_ACC_GEN_GET_BA_CASH_CLOSING_BALANCES_Result result = dbContext.UDF_ACC_GEN_GET_BA_CASH_CLOSING_BALANCES(param.FUND_TYPE, param.ADMIN_ND_CODE, param.MONTH, param.YEAR, param.LVL_ID).First();
                UDF_ACC_GEN_GET_BA_CASH_CLOSING_BALANCES_COMPOSITE_Result result = dbContext.UDF_ACC_GEN_GET_BA_CASH_CLOSING_BALANCES_COMPOSITE(param.FUND_TYPE, param.ADMIN_ND_CODE, param.MONTH, param.YEAR, param.LVL_ID).First();

                return result;

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error while getting Balances for payment.");
            }
            finally
            {
                dbContext.Dispose();
            }

        }


        /// <summary>
        /// function to list the master payment details 
        /// </summary>
        /// <param name="admin_nd_code">admin_nd_code</param>
        /// <param name="lvl_code"> level code</param>
        /// <param name="fundtype"> Fund Type </param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array ListMasterPaymentDetails(PaymentFilterModel objFilter, out long totalRecords)
        {
            string description;
            CommonFunctions commomFuncObj = new CommonFunctions();
            try
            {
                dbContext = new PMGSYEntities();
                List<ACC_BILL_MASTER> lstBillMaster = null;

                if (objFilter.FilterMode.Equals("view") && objFilter.BillId == 0)
                {
                    lstBillMaster = dbContext.ACC_BILL_MASTER.
                        Where(m => m.ADMIN_ND_CODE == objFilter.AdminNdCode).
                        Where(m => m.LVL_ID == objFilter.LevelId).
                        Where(m => m.FUND_TYPE == objFilter.FundType).
                        Where(m => m.BILL_MONTH == objFilter.Month).
                        Where(m => m.BILL_TYPE == objFilter.Bill_type).
                        Where(m => m.TXN_ID != dbContext.ACC_MASTER_TXN.Where(x => x.BILL_TYPE == "A" && x.FUND_TYPE == objFilter.FundType && x.OP_LVL_ID == objFilter.LevelId && x.IS_OPERATIONAL == true).Select(d => d.TXN_ID).FirstOrDefault()). //dont show ack master entry
                        Where(m => m.BILL_YEAR == objFilter.Year).ToList<ACC_BILL_MASTER>();
                }
                else if (objFilter.BillId != 0)
                {
                    lstBillMaster = dbContext.ACC_BILL_MASTER.
                Where(m => m.ADMIN_ND_CODE == objFilter.AdminNdCode).
                Where(m => m.LVL_ID == objFilter.LevelId).
                 Where(m => m.BILL_ID == objFilter.BillId).
                Where(m => m.FUND_TYPE == objFilter.FundType).
                Where(m => m.BILL_MONTH == objFilter.Month).
                Where(m => m.BILL_TYPE == objFilter.Bill_type).
                Where(m => m.TXN_ID != dbContext.ACC_MASTER_TXN.Where(x => x.BILL_TYPE == "A" && x.FUND_TYPE == objFilter.FundType && x.OP_LVL_ID == objFilter.LevelId && x.IS_OPERATIONAL == true).Select(d => d.TXN_ID).FirstOrDefault()). //dont show ack master entry
                Where(m => m.BILL_YEAR == objFilter.Year).ToList<ACC_BILL_MASTER>();
                }
                else
                {
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

                    if (objFilter.ChequeEpayNumber != null)
                    {
                        lstBillMaster = dbContext.ACC_BILL_MASTER
                            .Where(m => m.ADMIN_ND_CODE == objFilter.AdminNdCode)
                            .Where(m => m.LVL_ID == objFilter.LevelId)
                            .Where(m => m.FUND_TYPE == objFilter.FundType)
                            .Where(m => m.CHQ_NO == objFilter.ChequeEpayNumber)
                            .Where(m => m.TXN_ID != dbContext.ACC_MASTER_TXN.Where(x => x.BILL_TYPE == "A" && x.FUND_TYPE == objFilter.FundType && x.OP_LVL_ID == objFilter.LevelId && x.IS_OPERATIONAL == true).Select(d => d.TXN_ID).FirstOrDefault()) //dont show ack master entry
                            .Where(m => m.BILL_TYPE == "P")
                            .ToList<ACC_BILL_MASTER>();
                    }

                    else
                    {


                        if (objFilter.ToDate == null && objFilter.FromDate != null)
                        {
                            if (objFilter.TransId != 0)
                            {
                                lstBillMaster = dbContext.ACC_BILL_MASTER
                                    .Where(m => m.ADMIN_ND_CODE == objFilter.AdminNdCode)
                                    .Where(m => m.LVL_ID == objFilter.LevelId)
                                    .Where(m => m.FUND_TYPE == objFilter.FundType)
                                    .Where(m => m.BILL_DATE >= fromDate)
                                    .Where(m => m.TXN_ID == objFilter.TransId)
                                     .Where(m => m.BILL_TYPE == "P")
                                     .Where(m => m.TXN_ID != dbContext.ACC_MASTER_TXN.Where(x => x.BILL_TYPE == "A" && x.FUND_TYPE == objFilter.FundType && x.OP_LVL_ID == objFilter.LevelId && x.IS_OPERATIONAL == true).Select(d => d.TXN_ID).FirstOrDefault()) //dont show ack master entry
                                    .ToList<ACC_BILL_MASTER>();
                            }
                        }
                        if (objFilter.ToDate == null && objFilter.FromDate != null)
                        {
                            if (objFilter.TransId == 0)
                            {
                                lstBillMaster = dbContext.ACC_BILL_MASTER
                                    .Where(m => m.ADMIN_ND_CODE == objFilter.AdminNdCode)
                                    .Where(m => m.LVL_ID == objFilter.LevelId)
                                    .Where(m => m.FUND_TYPE == objFilter.FundType)
                                    .Where(m => m.BILL_DATE >= fromDate)
                                     .Where(m => m.BILL_TYPE == "P")
                                      .Where(m => m.TXN_ID != dbContext.ACC_MASTER_TXN.Where(x => x.BILL_TYPE == "A" && x.FUND_TYPE == objFilter.FundType && x.OP_LVL_ID == objFilter.LevelId && x.IS_OPERATIONAL == true).Select(d => d.TXN_ID).FirstOrDefault()) //dont show ack master entry

                                    .ToList<ACC_BILL_MASTER>();
                            }
                        }

                        else if (objFilter.ToDate != null && objFilter.FromDate == null)
                        {
                            if (objFilter.TransId == 0)
                            {
                                lstBillMaster = dbContext.ACC_BILL_MASTER
                                    .Where(m => m.ADMIN_ND_CODE == objFilter.AdminNdCode)
                                    .Where(m => m.LVL_ID == objFilter.LevelId)
                                    .Where(m => m.FUND_TYPE == objFilter.FundType)
                                    .Where(m => m.BILL_DATE <= toDate)
                                    .Where(m => m.BILL_TYPE == "P")
                                     .Where(m => m.TXN_ID != dbContext.ACC_MASTER_TXN.Where(x => x.BILL_TYPE == "A" && x.FUND_TYPE == objFilter.FundType && x.OP_LVL_ID == objFilter.LevelId && x.IS_OPERATIONAL == true).Select(d => d.TXN_ID).FirstOrDefault()) //dont show ack master entry
                                    .ToList<ACC_BILL_MASTER>();
                            }
                        }

                        if (objFilter.ToDate != null && objFilter.FromDate == null)
                        {
                            if (objFilter.TransId != 0)
                            {
                                lstBillMaster = dbContext.ACC_BILL_MASTER
                                    .Where(m => m.ADMIN_ND_CODE == objFilter.AdminNdCode)
                                    .Where(m => m.LVL_ID == objFilter.LevelId)
                                    .Where(m => m.FUND_TYPE == objFilter.FundType)
                                    .Where(m => m.BILL_DATE <= toDate)
                                     .Where(m => m.TXN_ID == objFilter.TransId)
                                    .Where(m => m.BILL_TYPE == "P")
                                     .Where(m => m.TXN_ID != dbContext.ACC_MASTER_TXN.Where(x => x.BILL_TYPE == "A" && x.FUND_TYPE == objFilter.FundType && x.OP_LVL_ID == objFilter.LevelId && x.IS_OPERATIONAL == true).Select(d => d.TXN_ID).FirstOrDefault()) //dont show ack master entry
                                    .ToList<ACC_BILL_MASTER>();
                            }
                        }

                        else if (objFilter.ToDate == null && objFilter.FromDate == null)
                        {
                            if (objFilter.TransId != 0)
                            {
                                lstBillMaster = dbContext.ACC_BILL_MASTER
                                    .Where(m => m.ADMIN_ND_CODE == objFilter.AdminNdCode)
                                    .Where(m => m.LVL_ID == objFilter.LevelId)
                                    .Where(m => m.FUND_TYPE == objFilter.FundType)
                                    .Where(m => m.TXN_ID == objFilter.TransId)
                                    .Where(m => m.BILL_TYPE == "P")
                                     .Where(m => m.TXN_ID != dbContext.ACC_MASTER_TXN.Where(x => x.BILL_TYPE == "A" && x.FUND_TYPE == objFilter.FundType && x.OP_LVL_ID == objFilter.LevelId && x.IS_OPERATIONAL == true).Select(d => d.TXN_ID).FirstOrDefault()) //dont show ack master entry
                                    .ToList<ACC_BILL_MASTER>();
                            }
                        }

                        if (objFilter.ToDate == null && objFilter.FromDate == null)
                        {
                            if (objFilter.TransId == 0)
                            {
                                lstBillMaster = dbContext.ACC_BILL_MASTER
                                    .Where(m => m.ADMIN_ND_CODE == objFilter.AdminNdCode)
                                    .Where(m => m.LVL_ID == objFilter.LevelId)
                                    .Where(m => m.FUND_TYPE == objFilter.FundType)
                                    .Where(m => m.BILL_TYPE == "P")
                                     .Where(m => m.TXN_ID != dbContext.ACC_MASTER_TXN.Where(x => x.BILL_TYPE == "A" && x.FUND_TYPE == objFilter.FundType && x.OP_LVL_ID == objFilter.LevelId && x.IS_OPERATIONAL == true).Select(d => d.TXN_ID).FirstOrDefault()) //dont show ack master entry
                                    .ToList<ACC_BILL_MASTER>();
                            }
                        }

                        else if (objFilter.ToDate != null && objFilter.FromDate != null)
                        {
                            if (objFilter.TransId != 0)
                            {
                                lstBillMaster = dbContext.ACC_BILL_MASTER.Where(m => m.ADMIN_ND_CODE == objFilter.AdminNdCode)
                                    .Where(m => m.LVL_ID == objFilter.LevelId)
                                    .Where(m => m.FUND_TYPE == objFilter.FundType)
                                    .Where(m => m.BILL_DATE >= fromDate)
                                    .Where(m => m.TXN_ID == objFilter.TransId)
                                     .Where(m => m.BILL_TYPE == "P")
                                      .Where(m => m.TXN_ID != dbContext.ACC_MASTER_TXN.Where(x => x.BILL_TYPE == "A" && x.FUND_TYPE == objFilter.FundType && x.OP_LVL_ID == objFilter.LevelId && x.IS_OPERATIONAL == true).Select(d => d.TXN_ID).FirstOrDefault()) //dont show ack master entry
                                    .Where(m => m.BILL_DATE <= toDate).ToList<ACC_BILL_MASTER>();
                            }
                        }
                        else if (objFilter.ToDate != null && objFilter.FromDate != null)
                        {
                            if (objFilter.TransId == 0)
                            {
                                lstBillMaster = dbContext.ACC_BILL_MASTER.Where(m => m.ADMIN_ND_CODE == objFilter.AdminNdCode)
                                    .Where(m => m.LVL_ID == objFilter.LevelId)
                                    .Where(m => m.FUND_TYPE == objFilter.FundType)
                                    .Where(m => m.BILL_DATE >= fromDate)
                                    .Where(m => m.BILL_DATE <= toDate)
                                    .Where(m => m.BILL_TYPE == "P")
                                    .Where(m => m.TXN_ID != dbContext.ACC_MASTER_TXN.Where(x => x.BILL_TYPE == "A" && x.FUND_TYPE == objFilter.FundType && x.OP_LVL_ID == objFilter.LevelId && x.IS_OPERATIONAL == true).Select(d => d.TXN_ID).FirstOrDefault()) //dont show ack master entry
                                    .ToList<ACC_BILL_MASTER>();
                            }
                        }
                        //else
                        //{
                        //    lstBillMaster = dbContext.ACC_BILL_MASTER
                        //        .Where(m => m.ADMIN_ND_CODE == objFilter.AdminNdCode)
                        //        .Where(m => m.LVL_ID == objFilter.LevelId)
                        //        .Where(m => m.FUND_TYPE == objFilter.FundType)
                        //        .Where(m => m.TXN_ID == objFilter.TransId)
                        //         .Where(m => m.BILL_TYPE == "P")
                        //        .ToList<ACC_BILL_MASTER>();
                        //}
                    }
                }




                totalRecords = lstBillMaster.Count();

                if (objFilter.sidx.Trim() != string.Empty)
                {
                    if (objFilter.sord.ToString() == "asc")
                    {
                        switch (objFilter.sidx)
                        {
                            case "Voucher_Number":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.BILL_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "voucher_date":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.BILL_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "Cash_Cheque":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.CHQ_EPAY).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "Transaction_type":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.ACC_MASTER_TXN.TXN_DESC).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "cheque_number":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.CHQ_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "cheque_Date":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.CHQ_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "cheque_amount":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.CHQ_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "Cash_Amount":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.CASH_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "Gross_Amount":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.GROSS_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            default:
                                //lstBillMaster = lstBillMaster.OrderBy(x => x.BILL_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                lstBillMaster = lstBillMaster.OrderBy(x => x.BILL_NO).ThenBy(x => x.BILL_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                        }

                    }
                    else
                    {
                        switch (objFilter.sidx)
                        {
                            case "Voucher_Number":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.BILL_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "voucher_date":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.BILL_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "Cash_Cheque":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.CHQ_EPAY).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "Transaction_type":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.ACC_MASTER_TXN.TXN_DESC).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "cheque_number":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.CHQ_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "cheque_Date":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.CHQ_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "cheque_amount":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.CHQ_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "Cash_Amount":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.CASH_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "Gross_Amount":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.GROSS_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            default:
                                //lstBillMaster = lstBillMaster.OrderByDescending(x => x.BILL_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();

                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.BILL_NO).ThenByDescending(x => x.BILL_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();

                                break;
                        }
                    }
                }
                else
                {
                    //lstBillMaster = lstBillMaster.OrderByDescending(x => x.BILL_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                    lstBillMaster = lstBillMaster.OrderByDescending(x => x.BILL_NO).ThenByDescending(x => x.BILL_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                }

                if (objFilter.BillId == 0)
                {
                    return lstBillMaster.Select(item => new
                    {
                        id = URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() }),
                        cell = new[] {                         
                                    
                                        
                                        item.BILL_NO,
                                        commomFuncObj.GetDateTimeToString(item.BILL_DATE),
                                        item.CHQ_EPAY == null ? String.Empty : (item.CHQ_EPAY.Trim() == "C" ? "Cash" : ((item.CHQ_EPAY.Trim() == "Q" || item.CHQ_EPAY=="A") ? "Cheque" : String.Empty)),
                                        item.ACC_MASTER_TXN.TXN_DESC.Trim(),
                                         item.CHQ_NO !=null ? item.CHQ_NO.ToString() :"",
                                        item.CHQ_DATE == null ? String.Empty : commomFuncObj.GetDateTimeToString(item.CHQ_DATE.Value),
                                      
                                        item.MAST_CON_ID.HasValue ? (dbContext.MASTER_CONTRACTOR.Where(c => c.MAST_CON_ID == item.MAST_CON_ID).Select(d => d.MAST_CON_FNAME).FirstOrDefault()+" "+dbContext.MASTER_CONTRACTOR.Where(c => c.MAST_CON_ID == item.MAST_CON_ID).Select(d => d.MAST_CON_MNAME).FirstOrDefault() +" "+dbContext.MASTER_CONTRACTOR.Where(c => c.MAST_CON_ID == item.MAST_CON_ID).Select(d => d.MAST_CON_LNAME).FirstOrDefault()) + (GetAgreementNumber(item.BILL_ID)==String.Empty||GetAgreementNumber(item.BILL_ID)==null?"": " ( Agreement - " +GetAgreementNumber(item.BILL_ID)+" )"): (item.PAYEE_NAME==null?string.Empty:item.PAYEE_NAME.ToString()),

                                        
                                        string.Empty,
                                         item.CHQ_AMOUNT.ToString(),
                                        item.CASH_AMOUNT.ToString(),
                                        item.GROSS_AMOUNT.ToString(),
                                   
                                       
                                        item.BILL_FINALIZED=="N" ? "<center><a href='#' class='ui-icon ui-icon-pencil' onclick='EditPayment(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()})+ "\");return false;'>Edit</a></center>" : ""     ,
                                        item.BILL_FINALIZED=="N" ? "<center><a href='#' class='ui-icon ui-icon-trash' onclick='DeletePayment(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()})+ "\");return false;'>Delete</a></center>" : "<span class='ui-icon ui-icon-locked ui-align-center'></span>"     ,
                                       
                                        //Below line commented on 15-03-2023
                                        //item.BILL_FINALIZED=="N" && CanVoucherFinalized(item.BILL_ID) ? "<center><a href='#' class='ui-icon ui-icon-unlocked' onclick='FinalizePayment(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()})+ "\");return false;'>Lock</a></center>" : (item.BILL_FINALIZED=="Y" || item.BILL_FINALIZED=="E") &&  item.TXN_ID != 228 &&  item.TXN_ID !=229 &&  item.TXN_ID !=624 &&  item.TXN_ID !=625 &&  item.TXN_ID !=825&&  item.TXN_ID !=824 &&  item.TXN_ID !=3077 ?  "<center><a href='#' class='ui-icon ui-icon-search' onclick='ViewPayment(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()})+ "\");return false;'>View</a></center>":string.Empty, //renewed txnid 228,624,825 ,cancelled =229,625,824
                                        
                                        //Below line Added on 15-03-2023
                                        item.BILL_FINALIZED=="N" && item.TXN_ID != 625 && CanVoucherFinalized(item.BILL_ID) ? "<center><a href='#' class='ui-icon ui-icon-unlocked' onclick='FinalizePayment(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()})+ "\");return false;'>Lock</a></center>" : (item.BILL_FINALIZED=="Y" || item.BILL_FINALIZED=="E") &&  item.TXN_ID != 228 &&  item.TXN_ID !=229 &&  item.TXN_ID !=624 &&  item.TXN_ID !=625 &&  item.TXN_ID !=825&&  item.TXN_ID !=824 &&  item.TXN_ID !=3077 ?  "<center><a href='#' class='ui-icon ui-icon-search' onclick='ViewPayment(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()})+ "\");return false;'>View</a></center>":string.Empty, //renewed txnid 228,624,825 ,cancelled =229,625,824
                                        
                                        item.CHQ_EPAY=="E" && (item.BILL_FINALIZED=="Y" || item.BILL_FINALIZED=="E")?( (item.REMIT_TYPE==null ? "<center><a href='#' class='ui-icon ui-icon-document' onclick='ViewEpayOrder(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() + "$"+ ( dbContext.ACC_EPAY_MAIL_MASTER.Where(c=>c.BILL_ID == item.BILL_ID && c.IS_EPAY_VALID==true).Any() ? "Y":"N") + "$E$"+ item.BILL_FINALIZED.ToString() 
                                        + "$" + ((item.CON_ACCOUNT_ID != null) ? item.CON_ACCOUNT_ID.Value : 0)
                                        })+ "\");return false;'>View Epayment Order</a></center>" :  "<center><a href='#' class='ui-icon ui-icon-document' onclick='ViewEremOrder(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() + "$"+ ( dbContext.ACC_EPAY_MAIL_MASTER.Where(c=>c.BILL_ID == item.BILL_ID && c.IS_EPAY_VALID==true).Any() ? "Y":"N") + "$R$"+ item.BILL_FINALIZED.ToString()  })+ "\");return false;'>View Eremittance Order</a></center>")):string.Empty,  //IS_EPAY_VALID =true Checked by Abhishek for Reject/Resend Epayment Details 25-Sep-2014
                                                         
                                        (
                                            (item.BILL_FINALIZED=="Y" || item.BILL_FINALIZED=="E")  && item.CHQ_AMOUNT > 0 
                                            && (item.CHQ_EPAY =="Q"||item.CHQ_EPAY =="A") && item.CHQ_NO != null 
                                            && !(dbContext.ACC_CANCELLED_CHEQUES.Where(x=>x.OLD_BILL_ID == item.BILL_ID).Any()) 
                                            //&& (PMGSYSession.Current.LevelId==4?true:dbContext.ACC_CHEQUES_ISSUED.Where(x=>x.BILL_ID== item.BILL_ID && x.CHEQUE_STATUS=="N").Any())
                                            &&(PMGSYSession.Current.LevelId==4 && PMGSYSession.Current.FundType=="A" )?dbContext.ACC_CHEQUES_ISSUED.Where(x=>x.BILL_ID== item.BILL_ID && x.CHEQUE_STATUS=="N").Any():(PMGSYSession.Current.LevelId==4?true:dbContext.ACC_CHEQUES_ISSUED.Where(x=>x.BILL_ID== item.BILL_ID && x.CHEQUE_STATUS=="N").Any())
                                            //Remittance of Statutory Deduction 15JUL2019
                                            //&& item.TXN_ID != 109
                                        )
                                        ? "<center><a href='#' class='ui-icon ui-icon ui-icon-refresh' onclick='RenewCheque(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() + "$"+ item.CHQ_NO})+ "\");return                                                           false;'>cancel/Renew cheque/Advice</a></center>" 
                                        : String.Empty,                          
                                                 
                                       (item.BILL_FINALIZED=="Y" || item.BILL_FINALIZED=="E") && item.CHQ_AMOUNT > 0 && (item.CHQ_EPAY =="Q"||item.CHQ_EPAY =="A") && item.CHQ_NO != null && (dbContext.ACC_CHEQUES_ISSUED.Where(x=>x.BILL_ID == item.BILL_ID && (x.NA_BILL_ID.HasValue) ).Any())? "Yes" :(item.BILL_FINALIZED=="Y" && item.CHQ_AMOUNT > 0 && (item.CHQ_EPAY =="Q"||item.CHQ_EPAY =="A") && item.CHQ_NO != null ?"No" : String.Empty) ,    
                                        item.ACTION_REQUIRED == "C" ? "<center><span title='Transaction details Incorrect, Needs Correction' style=color:#1C94C4; font-weight:bold' class='C'>"+item.ACTION_REQUIRED+"</span></center>" : item.ACTION_REQUIRED == "O" ? "<center><span title='Wrong Head Entry, Delete records and insert correct transactions' style=color:#b83400; font-weight:bold' class='O'>"+item.ACTION_REQUIRED+"</span></center>" : item.ACTION_REQUIRED == "M" ? "<center><span  title='Details not present, Unfinalize this record and insert details transactions' style=color:##014421; font-weight:bold' class='M'>"+item.ACTION_REQUIRED+"</span></center>" : "<center><span  title='Correct Transaction Entry' class='ui-icon ui-icon-check'>"+item.ACTION_REQUIRED+"</span></center>"  ,
                                        GetPFMSStatus(item.BILL_ID, out description),
                                        description
                                    
                        }
                    }).ToArray();               //Modified CHQ_EPAY=A for Advice No 7Apr2015
                }
                else
                {
                    return lstBillMaster.Select(item => new
                    {
                        id = URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() }),
                        cell = new[] {                         
                                    
                                        item.BILL_NO,
                                        commomFuncObj.GetDateTimeToString(item.BILL_DATE),
                                        item.CHQ_EPAY == null ? String.Empty : (item.CHQ_EPAY.Trim() == "C" ? "Cash" : ( (item.CHQ_EPAY.Trim() == "Q"||item.CHQ_EPAY.Trim() == "A") ? "Cheque" : String.Empty)),
                                        item.ACC_MASTER_TXN.TXN_DESC.Trim(),
                                         item.CHQ_NO !=null ? item.CHQ_NO.ToString() :"",
                                        item.CHQ_DATE == null ? String.Empty : commomFuncObj.GetDateTimeToString(item.CHQ_DATE.Value),

                                        // (item.PAYEE_NAME==null?string.Empty:item.PAYEE_NAME.ToString()),
                                        //item.MAST_CON_ID.HasValue ? dbContext.MASTER_CONTRACTOR.Where(c => c.MAST_CON_ID == item.MAST_CON_ID).Select(d => d.MAST_CON_COMPANY_NAME).FirstOrDefault() : (item.PAYEE_NAME==null?string.Empty:item.PAYEE_NAME.ToString()),                                       
                                        item.MAST_CON_ID.HasValue ? (dbContext.MASTER_CONTRACTOR.Where(c => c.MAST_CON_ID == item.MAST_CON_ID).Select(d => d.MAST_CON_FNAME).FirstOrDefault()+" "+dbContext.MASTER_CONTRACTOR.Where(c => c.MAST_CON_ID == item.MAST_CON_ID).Select(d => d.MAST_CON_MNAME).FirstOrDefault() +" "+dbContext.MASTER_CONTRACTOR.Where(c => c.MAST_CON_ID == item.MAST_CON_ID).Select(d => d.MAST_CON_LNAME).FirstOrDefault()) : (item.PAYEE_NAME==null?string.Empty:item.PAYEE_NAME.ToString()),

                                        string.Empty,
                                         item.CHQ_AMOUNT.ToString(),
                                        item.CASH_AMOUNT.ToString(),
                                        item.GROSS_AMOUNT.ToString(),
                                                                          
                                        item.BILL_FINALIZED=="N" ? "<center><a href='#' class='ui-icon ui-icon-pencil' onclick='EditPayment(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()})+ "\");return false;'>Edit</a></center>" : ""     ,
                                        item.BILL_FINALIZED=="N" ? "<center><a href='#' class='ui-icon ui-icon-trash' onclick='DeletePayment(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()})+ "\");return false;'>Delete</a></center>" : ""     ,
                                        //item.BILL_FINALIZED=="N" && CanVoucherFinalized(item.BILL_ID) ? "<center><a href='#' class='ui-icon ui-icon-unlocked' onclick='FinalizePayment(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()})+ "\");return false;'>Lock</a></center>" : (item.BILL_FINALIZED=="Y" || item.BILL_FINALIZED=="E") &&  item.TXN_ID != 228 &&  item.TXN_ID !=229  ?  "<center><a href='#' class='ui-icon ui-icon-search' onclick='ViewPayment(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()})+ "\");return false;'>View</a></center>":string.Empty, //renewed txnid 228 ,cancelled =229
                                        item.BILL_FINALIZED=="N" && CanVoucherFinalized(item.BILL_ID) ? "<center><a href='#' class='ui-icon ui-icon-unlocked' onclick='FinalizePayment(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()})+ "\");return false;'>Lock</a></center>" : (item.BILL_FINALIZED=="Y" || item.BILL_FINALIZED=="E") &&  item.TXN_ID != 228 &&  item.TXN_ID !=229 &&  item.TXN_ID !=624 &&  item.TXN_ID !=625 &&  item.TXN_ID !=825&&  item.TXN_ID !=824 ?  "<center><a href='#' class='ui-icon ui-icon-search' onclick='ViewPayment(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()})+ "\");return false;'>View</a></center>":string.Empty, //renewed txnid 228,624,825 ,cancelled =229,625,824

                                        
                                        item.CHQ_EPAY=="E" && (item.BILL_FINALIZED=="Y" || item.BILL_FINALIZED=="E")?( (item.REMIT_TYPE==null ? "<center><a href='#' class='ui-icon ui-icon-document' onclick='ViewEpayOrder(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() + "$"+ ( dbContext.ACC_EPAY_MAIL_MASTER.Where(c=>c.BILL_ID == item.BILL_ID && c.IS_EPAY_VALID==true).Any() ? "Y":"N")  +
                        "$" + ((item.CON_ACCOUNT_ID != null) ? item.CON_ACCOUNT_ID.Value : 0) })+ "\");return false;'>View Epayment Order</a></center>" :  "<center><a href='#' class='ui-icon ui-icon-document' onclick='ViewEremOrder(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() + "$"+ ( dbContext.ACC_EPAY_MAIL_MASTER.Where(c=>c.BILL_ID == item.BILL_ID && c.IS_EPAY_VALID==true).Any() ? "Y":"N")  })+ "\");return false;'>View Eremittance Order</a></center>")):string.Empty,     //IS_EPAY_VALID =true Checked by Abhishek for Reject/Resend Epayment Details 25-Sep-2014
                                        //(item.BILL_FINALIZED=="Y" || item.BILL_FINALIZED=="E")  && item.CHQ_AMOUNT > 0 && item.CHQ_EPAY =="Q" && item.CHQ_NO != null && !(dbContext.ACC_CANCELLED_CHEQUES.Where(x=>x.OLD_BILL_ID == item.BILL_ID).Any())? "<center><a href='#' class='ui-icon ui-icon ui-icon-refresh' onclick='RenewCheque(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() + "$"+ item.CHQ_NO})+ "\");return false;'>cancel/Renew cheque</a></center>" : String.Empty,                          
                                        (item.BILL_FINALIZED=="Y" || item.BILL_FINALIZED=="E")  && item.CHQ_AMOUNT > 0 && (item.CHQ_EPAY =="Q"||item.CHQ_EPAY.Trim() == "A") && item.CHQ_NO != null && !(dbContext.ACC_CANCELLED_CHEQUES.Where(x=>x.OLD_BILL_ID == item.BILL_ID).Any()) && (PMGSYSession.Current.LevelId==4?true:dbContext.ACC_CHEQUES_ISSUED.Where(x=>x.BILL_ID== item.BILL_ID && x.CHEQUE_STATUS=="N").Any() ) ? "<center><a href='#' class='ui-icon ui-icon ui-icon-refresh' onclick='RenewCheque(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() + "$"+ item.CHQ_NO})+ "\");return false;'>cancel/Renew cheque</a></center>" : String.Empty,                          
                                        
                                       (item.BILL_FINALIZED=="Y" || item.BILL_FINALIZED=="E") && item.CHQ_AMOUNT > 0 && (item.CHQ_EPAY =="Q"||item.CHQ_EPAY.Trim() == "A") && item.CHQ_NO != null && (dbContext.ACC_CHEQUES_ISSUED.Where(x=>x.BILL_ID == item.BILL_ID && (x.NA_BILL_ID.HasValue) ).Any())? "Yes" :(item.BILL_FINALIZED=="Y" && item.CHQ_AMOUNT > 0 && item.CHQ_EPAY =="Q" && item.CHQ_NO != null ?"No" : String.Empty) ,    
                                        item.ACTION_REQUIRED == "C" ? "<center><span title='Transaction details Incorrect, Needs Correction' style=color:#1C94C4; font-weight:bold' class='C'>"+item.ACTION_REQUIRED+"</span></center>" : item.ACTION_REQUIRED == "O" ? "<center><span title='Wrong Head Entry, Delete records and insert correct transactions' style=color:#b83400; font-weight:bold' class='O'>"+item.ACTION_REQUIRED+"</span></center>" : item.ACTION_REQUIRED == "M" ? "<center><span  title='Details not present, Unfinalize this record and insert details transactions' style=color:##014421; font-weight:bold' class='M'>"+item.ACTION_REQUIRED+"</span></center>" : "<center><span  title='Correct Transaction Entry' class='ui-icon ui-icon-check'>"+item.ACTION_REQUIRED+"</span></center>",
                                    
                                        GetPFMSStatus(item.BILL_ID, out description),
                                        description
                        }                     //Modified CHQ_EPAY=A for Advice No 7Apr2015
                    }).ToArray();


                }

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PaymentDAL.ListMasterPaymentDetails()");
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }

        }

        public string GetPFMSStatus(long billId, out string description)
        {
            string PFMSStatus = "-";
            description = "-";
            try
            {
                var query = (dbContext.PFMS_OMMAS_PAYMENT_MAPPING.Where(x => x.BILL_ID == billId).FirstOrDefault());
                if (query != null)
                {
                    PFMSStatus = (string.IsNullOrEmpty(query.BANK_ACK_BILL_STATUS)
                                    ? string.IsNullOrEmpty(query.ACK_BILL_STATUS)
                                        ? "Processing at PFMS"
                                        : (query.ACK_BILL_STATUS == "A" ? "Payment accepted by PFMS" : "Payment rejected by PFMS")
                                    : (query.BANK_ACK_BILL_STATUS == "A" ? "Accepted by Bank" : "Rejected by Bank"));

                    description = (string.IsNullOrEmpty(query.BANK_ACK_BILL_STATUS)
                                    ? string.IsNullOrEmpty(query.ACK_BILL_STATUS)
                                        ? "Processing at PFMS"
                                        : (query.ACK_BILL_STATUS == "A" ? "NA" : query.REJECTION_NARRATION)
                                    : (query.BANK_ACK_BILL_STATUS == "A" ? "" : query.BANK_ACK_REJECTION_NARRATION));
                }
                else
                {
                    var query1 = (dbContext.REAT_OMMAS_PAYMENT_MAPPING.Where(x => x.BILL_ID == billId).FirstOrDefault()); //else condition added for REAT_OMMS_PAYMENT_MAPPING table Done by priyanka 12-05-2020
                    if (query1 != null)
                    {
                        PFMSStatus = (string.IsNullOrEmpty(query1.BANK_ACK_BILL_STATUS)
                                        ? string.IsNullOrEmpty(query1.ACK_BILL_STATUS)
                                            ? "Processing at PFMS"
                                            : (query1.ACK_BILL_STATUS == "A" ? "Payment accepted by PFMS" : "Payment rejected by PFMS")
                                        : (query1.BANK_ACK_BILL_STATUS == "A" ? "Accepted by Bank" : "Rejected by Bank"));

                        description = (string.IsNullOrEmpty(query1.BANK_ACK_BILL_STATUS)
                                        ? string.IsNullOrEmpty(query1.ACK_BILL_STATUS)
                                            ? "Processing at PFMS"
                                            : (query1.ACK_BILL_STATUS == "A" ? "NA" : query1.REJECTION_NARRATION)
                                        : (query1.BANK_ACK_BILL_STATUS == "A" ? "" : query1.BANK_ACK_REJECTION_NARRATION));
                    }
                }
                return PFMSStatus;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PaymentDAL.GetPFMSStatus()");
                return string.Empty;
            }
        }








        public string GetAgreementNumber(long BillId)
        {
            dbContext = new PMGSYEntities();
            string AgrementNumber = string.Empty;
            //long? mastConId = 0;
            //int BillId = 0;                                    

            //string[] parameters = BillIdAndContID.Split('$');

            //BillId = Convert.ToInt32(parameters[0]);

            //if (parameters[1] != null)
            //{
            //    mastConId = Convert.ToInt64(parameters[1]);            
            //}

            try
            {
                if (PMGSYSession.Current.FundType.ToUpper() == "M")
                {
                    //check Bill Details Present
                    if (dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == BillId && m.CREDIT_DEBIT == "D" && m.IMS_AGREEMENT_CODE != null && m.IMS_PR_ROAD_CODE != null && m.MAST_CON_ID != null).Any())
                    {
                        ACC_BILL_DETAILS billDetails = dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == BillId && m.CREDIT_DEBIT == "D").FirstOrDefault();
                        //old
                        //AgrementNumber = dbContext.MANE_IMS_CONTRACT.Where(m => m.IMS_PR_ROAD_CODE == billDetails.IMS_PR_ROAD_CODE && m.MANE_PR_CONTRACT_CODE == billDetails.IMS_AGREEMENT_CODE && m.MAST_CON_ID == billDetails.MAST_CON_ID).Select(s => s.MANE_AGREEMENT_NUMBER).FirstOrDefault();
                        //Modified By Abhihshek kamble to get Agreement No using MANE_CONTRACOR_ID 17Nov2014
                        AgrementNumber = dbContext.MANE_IMS_CONTRACT.Where(m => m.MANE_CONTRACT_ID == billDetails.IMS_AGREEMENT_CODE && m.MAST_CON_ID == billDetails.MAST_CON_ID).Select(s => s.MANE_AGREEMENT_NUMBER).FirstOrDefault();

                        return AgrementNumber;
                    }
                }
                else
                {
                    //check Bill Details Present
                    if (dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == BillId && m.CREDIT_DEBIT == "D" && m.IMS_AGREEMENT_CODE != null).Any())
                    {
                        //Find Agreement number in MANE_IMS_CONTRACT
                        int? AgreementCode = dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == BillId && m.CREDIT_DEBIT == "D").Select(s => s.IMS_AGREEMENT_CODE).FirstOrDefault();
                        AgrementNumber = dbContext.TEND_AGREEMENT_MASTER.Where(m => m.TEND_AGREEMENT_CODE == AgreementCode).Select(s => s.TEND_AGREEMENT_NUMBER).FirstOrDefault();
                        return AgrementNumber;
                    }
                }
                return AgrementNumber;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return String.Empty;
            }
        }


        /// <summary>
        /// function to populate master data jqgrid on data entry page
        /// </summary>
        /// <param name="objFilter"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array ListMasterPaymentDetailsForDataEntry(PaymentFilterModel objFilter, out long totalRecords, out long _MasterTxnID)
        {

            CommonFunctions commomFuncObj = new CommonFunctions();

            try
            {
                List<ACC_BILL_MASTER> lstBillMaster = null;
                dbContext = new PMGSYEntities();

                if (objFilter.BillId == 0)
                {
                    lstBillMaster = dbContext.ACC_BILL_MASTER.
                    Where(m => m.ADMIN_ND_CODE == objFilter.AdminNdCode).
                    Where(m => m.LVL_ID == objFilter.LevelId).
                    Where(m => m.FUND_TYPE == objFilter.FundType).
                    Where(m => m.BILL_MONTH == objFilter.Month).
                    Where(m => m.BILL_TYPE == objFilter.Bill_type).
                    Where(m => m.BILL_YEAR == objFilter.Year)
                    .ToList<ACC_BILL_MASTER>();
                }
                else
                {
                    lstBillMaster = dbContext.ACC_BILL_MASTER.
                    Where(m => m.ADMIN_ND_CODE == objFilter.AdminNdCode).
                    Where(m => m.LVL_ID == objFilter.LevelId).
                    Where(m => m.BILL_ID == objFilter.BillId).
                    Where(m => m.FUND_TYPE == objFilter.FundType).
                    Where(m => m.BILL_MONTH == objFilter.Month).
                    Where(m => m.BILL_TYPE == objFilter.Bill_type).
                    Where(m => m.BILL_YEAR == objFilter.Year)
                    .ToList<ACC_BILL_MASTER>();

                }


                totalRecords = lstBillMaster.Count();

                _MasterTxnID = lstBillMaster.Select(s => s.TXN_ID).FirstOrDefault();

                return lstBillMaster.Select(item => new
                {
                    id = URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() }),
                    cell = new[] {                         
                                    
                                        item.BILL_NO,
                                        commomFuncObj.GetDateTimeToString(item.BILL_DATE),
                                        item.CHQ_EPAY == null ? String.Empty : (item.CHQ_EPAY.Trim() == "C" ? "Cash" : ((item.CHQ_EPAY.Trim() == "Q"||item.CHQ_EPAY.Trim() == "A") ? "Cheque" : (item.CHQ_EPAY.Trim() == "E" ? "Epayment":string.Empty ))),
                                        item.ACC_MASTER_TXN.TXN_DESC.Trim(),
                                        item.CHQ_NO,
                                        item.CHQ_DATE == null ? String.Empty :  commomFuncObj.GetDateTimeToString(item.CHQ_DATE.Value),
                                       item.MAST_CON_ID.HasValue ? dbContext.MASTER_CONTRACTOR.Where(c => c.MAST_CON_ID == item.MAST_CON_ID).Select(d => d.MAST_CON_COMPANY_NAME).FirstOrDefault() + " - " + item.MAST_CON_ID + (GetAgreementNumber(item.BILL_ID)==String.Empty||GetAgreementNumber(item.BILL_ID)==null?"": " ( Agreement - " +GetAgreementNumber(item.BILL_ID)+" )"): (item.PAYEE_NAME == null?"-":item.PAYEE_NAME.ToString()),
                                        string.Empty,
                                        item.CHQ_AMOUNT.ToString(),
                                        item.CASH_AMOUNT.ToString(),
                                        item.GROSS_AMOUNT.ToString(),
                                   
                                        item.BILL_FINALIZED=="N" && CanVoucherFinalized(item.BILL_ID) ? "<center><a href='#' class='ui-icon ui-icon-unlocked' onclick='FinalizePaymentMaster(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()})+ "\");return false;'>Lock</a></center>" : item.BILL_FINALIZED=="Y" || item.BILL_FINALIZED=="E"  ? "<center><a href='#' class='ui-icon ui-icon-locked' onclick='ViewPaymentMaster(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()})+ "\");return false;'>Voucher is finalized</a></center>":string.Empty,
                                       //working code commented by by Koustubh Nakate on 07/10/2013 to restrict e-payment and e-remittance to update 
                                        // item.BILL_FINALIZED=="N"? "<center><a href='#' class='ui-icon ui-icon-pencil' onclick='EditPaymentMaster(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()})+ "\");return false;'>Edit</a></center>" : item.BILL_FINALIZED=="Y" ? "<center><a href='#' class='ui-icon ui-icon-locked' onclick='ViewPaymentMaster(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()})+ "\");return false;'>View</a></center>":string.Empty,    
                                       //changes by Koustubh Nakate on 07/10/2013 to restrict e-payment and e-remittance to update  
                                       (item.BILL_FINALIZED=="N" && item.CHQ_EPAY!="E")  ? "<center><a href='#' class='ui-icon ui-icon-pencil' onclick='EditPaymentMaster(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()})+ "\");return false;'>Edit</a></center>" :  "<center> <a  class='ui-icon ui-icon-locked' title='Locked'> </a></center>",    
                                       item.BILL_FINALIZED=="N"? "<center><a href='#' class='ui-icon ui-icon-trash' onclick='DeletePaymentMaster(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()})+ "\");return false;'>Delete</a></center>" : "<center> <a  class='ui-icon ui-icon-locked' title='Locked'> </a></center>",    

                                       // (item.BILL_FINALIZED=="N" && item.CHQ_EPAY!="E")  ? "<center><a href='#' class='ui-icon ui-icon-trash' onclick='DeletePaymentMaster(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()})+ "\");return false;'>Delete</a></center>" :  "<center> <span class='ui-icon ui-icon-locked' title='Locked'> </span></center>",    

                  
                        }
                }).ToArray();




            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                _MasterTxnID = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }


        }
        /// <summary>
        /// function  to delete the master payment Details
        /// </summary>
        /// <param name="Bill_Id"></param>
        /// <returns></returns>
        public Int32 DeleteMasterPaymentDetails(Int64 Bill_Id)
        {
            //added by abhishek kamble 11-dec-2013
            PMGSYEntities billMasterDbContext = new PMGSYEntities();
            PMGSYEntities chequeIssuedDbContext = new PMGSYEntities();
            PMGSYEntities billDetailsDbContext = new PMGSYEntities();
            try
            {
                dbContext = new PMGSYEntities();


                using (var scope = new TransactionScope())
                {

                    //get the master details
                    ACC_BILL_MASTER con = billMasterDbContext.ACC_BILL_MASTER.SingleOrDefault(p => p.BILL_ID == Bill_Id);

                    //cheque if finalized
                    if (con.BILL_FINALIZED == "Y")
                    {
                        return -1; //return status error
                    }

                    //for payment entry 

                    //check if asset details has been entered for the voucher
                    if (dbContext.ACC_ASSET_DETAILS.Any(c => c.BILL_ID == Bill_Id))
                    {
                        return -2;
                    }

                    //Added By Abhishek Kamble 28-nov-2013
                    con.USERID = PMGSYSession.Current.UserId;
                    con.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    billMasterDbContext.Entry(con).State = System.Data.Entity.EntityState.Modified;
                    billMasterDbContext.SaveChanges();

                    //Added By Abhishek Kamble 28-nov-2013
                    ACC_CHEQUES_ISSUED chequesIssued = chequeIssuedDbContext.ACC_CHEQUES_ISSUED.Where(m => m.BILL_ID == Bill_Id).FirstOrDefault();
                    if (chequesIssued != null)
                    {
                        chequesIssued.USERID = PMGSYSession.Current.UserId;
                        chequesIssued.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        chequeIssuedDbContext.Entry(chequesIssued).State = System.Data.Entity.EntityState.Modified;
                        chequeIssuedDbContext.SaveChanges();
                    }

                    //delete the cheque issued entry if allredy exist
                    chequeIssuedDbContext.Database.ExecuteSqlCommand
                        ("DELETE [omms].ACC_CHEQUES_ISSUED Where BILL_ID = {0}", Bill_Id);


                    //Added By Abhishek Kamble 28-nov-2013
                    ACC_BILL_DETAILS accBillDetailsModel = billDetailsDbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == Bill_Id).FirstOrDefault();
                    if (accBillDetailsModel != null)
                    {
                        accBillDetailsModel.USERID = PMGSYSession.Current.UserId;
                        accBillDetailsModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        billDetailsDbContext.Entry(accBillDetailsModel).State = System.Data.Entity.EntityState.Modified;
                        billDetailsDbContext.SaveChanges();
                    }
                    //delete the details from bill details table
                    billDetailsDbContext.Database.ExecuteSqlCommand
                         ("DELETE [omms].ACC_BILL_DETAILS Where BILL_ID = {0}", Bill_Id);

                    //added by koustubh nakate on 22/08/2013 to delete details from  ACC_NOTIFICATION_DETAILS             
                    // dbContext.ACC_NOTIFICATION_DETAILS.ToList<ACC_NOTIFICATION_DETAILS>().RemoveAll(nd => nd.INITIATION_BILL_ID == Bill_Id);


                    //Added By Abhishek Kamble 28-nov-2013
                    ACC_NOTIFICATION_DETAILS accNotificationDetailsModel = dbContext.ACC_NOTIFICATION_DETAILS.Where(m => m.INITIATION_BILL_ID == Bill_Id).FirstOrDefault();
                    if (accNotificationDetailsModel != null)
                    {
                        accNotificationDetailsModel.USERID = PMGSYSession.Current.UserId;
                        accNotificationDetailsModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        dbContext.Entry(accNotificationDetailsModel).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }

                    dbContext.Database.ExecuteSqlCommand
                      ("DELETE [omms].ACC_NOTIFICATION_DETAILS Where INITIATION_BILL_ID = {0}", Bill_Id);

                    //Added by Abhishek kamble 1-Aug-2014 Delete details from ACC_TXN_BANK if rec exist. start

                    //Check entry present In ACC_TXN_BANK Added By Abhishek kamble 1-Aug-201
                    if (dbContext.ACC_TXN_BANK.Where(m => m.BILL_ID == Bill_Id).Any())
                    {
                        //Check TXN code type-R-Reconcile (dont delete) U-unreconcile (delete details.)  1-Aug-2014  
                        if (dbContext.ACC_TXN_BANK.Where(m => m.BILL_ID == Bill_Id).Select(s => s.TXN_TYPE_CODE).FirstOrDefault() == "R")
                        {
                            return -3;
                        }
                        else
                        {
                            dbContext.Database.ExecuteSqlCommand("DELETE [omms].ACC_TXN_BANK Where BILL_ID = {0}", Bill_Id);
                        }
                    }
                    //Added by Abhishek kamble 1-Aug-2014 Delete details from ACC_TXN_BANK if rec exist. end



                    //delete the master table entry
                    dbContext.SaveChanges();
                    billMasterDbContext.ACC_BILL_MASTER.Remove(con);
                    billMasterDbContext.SaveChanges();

                    //added by Abhishek kamble 11-dec-2013
                    //billMasterDbContext.SaveChanges();
                    chequeIssuedDbContext.SaveChanges();
                    billDetailsDbContext.SaveChanges();

                    scope.Complete();
                    return 1;
                }

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error while deleting master payment details.");
            }
            finally
            {
                dbContext.Dispose();

                billMasterDbContext.SaveChanges();
                chequeIssuedDbContext.SaveChanges();
                billDetailsDbContext.SaveChanges();
            }
        }



        /// <summary>
        /// DAL function to check if cheque is allready issued
        /// </summary>
        /// <param name="chequeNumber"></param>
        /// <param name="operation"></param>
        /// <param name="billId"></param>
        /// <returns> true if allready cheque is issed else false</returns>
        public bool IschequeIssued(string chequeNumber, string operation, long billId)
        {

            try
            {
                dbContext = new PMGSYEntities();
                ACC_BILL_MASTER masterDetails = new ACC_BILL_MASTER();

                //if add operation 
                if (operation == "A")
                {

                    if (dbContext.ACC_BILL_MASTER.Where(x => x.CHQ_NO == chequeNumber &&
                        x.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && x.FUND_TYPE == PMGSYSession.Current.FundType).Any())
                    {
                        return true;
                    }
                    else
                    {

                        return false;
                    }
                }
                else if (operation == "E")
                {
                    //get the master entries cheque number
                    string oldChequeNumber = dbContext.ACC_BILL_MASTER.Where(x => x.BILL_ID == billId).Select(t => t.CHQ_NO).First();

                    if (oldChequeNumber.Equals(chequeNumber))
                    {
                        //no change in chq number
                        return false;
                    }
                    else
                    {
                        if (dbContext.ACC_BILL_MASTER.Where(x => x.CHQ_NO == chequeNumber &&
                            x.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && x.FUND_TYPE == PMGSYSession.Current.FundType).Any())
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
                    return false;
                }

            }
            catch (Exception ex)
            {

                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error while checking if cheque number is allready issued.");
            }
            finally
            {
                dbContext.Dispose();
            }

        }


        /// <summary>
        /// function to get the master Payment details
        /// </summary>
        /// <param name="Bill_id"></param>
        /// <returns></returns>
        public ACC_BILL_MASTER GetMasterPaymentDetails(Int64 Bill_id)
        {
            try
            {
                dbContext = new PMGSYEntities();
                ACC_BILL_MASTER masterDetails = new ACC_BILL_MASTER();

                masterDetails = (from result in dbContext.ACC_BILL_MASTER
                                 where result.BILL_ID == Bill_id
                                 select result
                                   ).FirstOrDefault();

                return masterDetails;


            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error while getting master payment details.");
            }
            finally
            {
                dbContext.Dispose();
            }

        }

        /// <summary>
        /// function to list the transactionpayment and deduction lists on data entry page
        /// </summary>
        /// <param name="objFilter"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>

        public Array ListPaymentDeductionDetailsForDataEntry(PaymentFilterModel objFilter, out long totalRecords)
        {

            CommonFunctions commomFuncObj = null;
            commomFuncObj = new CommonFunctions();
            List<transactionList> lstTransactions = new List<transactionList>();
            TransactionParams objParam = new TransactionParams();
            try
            {

                dbContext = new PMGSYEntities();

                TransactionParams objparams = new TransactionParams();
                ACC_SCREEN_DESIGN_PARAM_MASTER obj1 = new ACC_SCREEN_DESIGN_PARAM_MASTER();
                objparams.TXN_ID = dbContext.ACC_BILL_MASTER.Where(x => x.BILL_ID == objFilter.BillId).Select(c => c.TXN_ID).First();
                obj1 = commomFuncObj.getMasterDesignParam(objparams);


                var query = (from master in dbContext.ACC_BILL_MASTER
                             join details in dbContext.ACC_BILL_DETAILS
                             on master.BILL_ID equals details.BILL_ID
                             where master.BILL_ID == objFilter.BillId
                                 // && details.CASH_CHQ == "D"
                             && details.CREDIT_DEBIT == "D"
                             select new
                             {
                                 master.BILL_FINALIZED,
                                 details.BILL_ID,
                                 //dbContext.ACC_MASTER_TXN.Where(x => x.TXN_ID == details.TXN_ID).Select(h => h.TXN_NARRATION).FirstOrDefault(),
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
                                 master.FUND_TYPE,
                                 details.CREDIT_DEBIT,
                                 masterHead = master.TXN_ID,
                                 details.FINAL_PAYMENT      //Added By Abhishek kamble to show final payment
                             });

                // query = query.OrderBy(c => c.CASH_CHQ == "Q").ThenBy(t => t.CASH_CHQ == "C").ThenBy(t => t.CASH_CHQ == "D");

                Int16 i = 0;
                foreach (var item in query)
                {
                    i++;
                    transactionList obj = new transactionList();
                    obj.SERIAL_No = i;
                    obj.BILL_FINALIZED = item.BILL_FINALIZED;
                    obj.AMOUNT = item.AMOUNT;
                    obj.BILL_ID = item.BILL_ID;
                    obj.CASH_CHQ = item.CASH_CHQ;
                    obj.HEAD_ID_Narration = dbContext.ACC_MASTER_TXN.Where(x => x.TXN_ID == item.TXN_ID).Select(h => h.TXN_DESC).FirstOrDefault();
                    obj.TXN_ID = item.TXN_ID.Value;
                    obj.MASTER_TXN_ID = item.masterHead;
                    obj.NARRATION = item.NARRATION;
                    obj.TXN_NO = item.TXN_NO;
                    //below line is commented on 28-01-2022
                    //obj.RODE_CODE = dbContext.IMS_SANCTIONED_PROJECTS.Where(c => c.IMS_PR_ROAD_CODE == (item.IMS_PR_ROAD_CODE.HasValue ? item.IMS_PR_ROAD_CODE.Value : -1)).Select(y => y.IMS_ROAD_NAME).FirstOrDefault() + ((item.FINAL_PAYMENT != null) ? (item.FINAL_PAYMENT.Value ? "<br/>(<b>Final Payment : Yes</b>) " : "<br/>(<b>Final Payment : No </b>)") : "");
                    //below line is Added on 28-01-2022
                    obj.RODE_CODE = dbContext.IMS_SANCTIONED_PROJECTS.Where(c => c.IMS_PR_ROAD_CODE == (item.IMS_PR_ROAD_CODE.HasValue ? item.IMS_PR_ROAD_CODE.Value : -1)).Select(y => y.IMS_ROAD_NAME).FirstOrDefault() + ((item.FINAL_PAYMENT != null) ? (item.FINAL_PAYMENT.Value ? "<br/>(<b>Final Payment : Yes</b>) " : "<br/>(<b>Final Payment : No </b>)") : "");
                    // AGREEMENT_CODE Modified By Abhishek kamble 10-June-2014
                    //old
                    //obj.AGREEMENT_CODE = item.FUND_TYPE == "M" ? (dbContext.MANE_IMS_CONTRACT.Where(c => c.IMS_PR_ROAD_CODE == (item.IMS_PR_ROAD_CODE.HasValue ? item.IMS_PR_ROAD_CODE.Value : -1) && c.MANE_PR_CONTRACT_CODE == (item.IMS_AGREEMENT_CODE.HasValue ? item.IMS_AGREEMENT_CODE.Value : -1) && c.MAST_CON_ID == (item.MAST_CON_ID.HasValue ? item.MAST_CON_ID.Value : -1)).Select(x => x.MANE_AGREEMENT_NUMBER).FirstOrDefault()) : (dbContext.TEND_AGREEMENT_MASTER.Where(c => c.TEND_AGREEMENT_CODE == (item.IMS_AGREEMENT_CODE.HasValue ? item.IMS_AGREEMENT_CODE.Value : -1)).Select(y => y.TEND_AGREEMENT_NUMBER).FirstOrDefault());
                    //Modified By Abhishek kamble to get Agr Code using MANE_CONTRACTOR_ID 17Nov2014
                    obj.AGREEMENT_CODE = item.FUND_TYPE == "M" ? (dbContext.MANE_IMS_CONTRACT.Where(c => c.MANE_CONTRACT_ID == (item.IMS_AGREEMENT_CODE.HasValue ? item.IMS_AGREEMENT_CODE.Value : -1) && c.MAST_CON_ID == (item.MAST_CON_ID.HasValue ? item.MAST_CON_ID.Value : -1)).Select(x => x.MANE_AGREEMENT_NUMBER).FirstOrDefault()) : (dbContext.TEND_AGREEMENT_MASTER.Where(c => c.TEND_AGREEMENT_CODE == (item.IMS_AGREEMENT_CODE.HasValue ? item.IMS_AGREEMENT_CODE.Value : -1)).Select(y => y.TEND_AGREEMENT_NUMBER).FirstOrDefault());

                    obj.MASTER_CHQEPAY = item.CHQ_EPAY;
                    obj.ONLY_DEDUCTION = obj1.DED_REQ == "B" ? "Y" : "N";
                    obj.paymentType = obj.CASH_CHQ == "C" ? "Payment" : item.CASH_CHQ == "Q" ? "Payment" : "Deduction";

                    //obj.HEAD_ID = obj.paymentType.Equals("Payment") ? item.HEAD_CODE.ToString() :
                    //    (dbContext.ACC_MASTER_HEAD.Where(t => t.HEAD_ID == (dbContext.ACC_BILL_DETAILS.Where(c => c.BILL_ID == item.BILL_ID && c.TXN_NO == (item.TXN_NO - 1) && c.CREDIT_DEBIT.Equals("C")).Select(c => c.HEAD_ID).FirstOrDefault()))).Select(d => d.HEAD_CODE).FirstOrDefault().ToString();

                    obj.HEAD_ID = obj.paymentType.Equals("Payment") ? item.HEAD_CODE.ToString() :
                       (dbContext.ACC_MASTER_HEAD.Where(t => t.HEAD_ID == (dbContext.ACC_BILL_DETAILS.Where(c => c.BILL_ID == item.BILL_ID && c.TXN_NO == (item.TXN_NO - 1)).Select(c => c.HEAD_ID).FirstOrDefault()))).Select(d => d.HEAD_CODE).FirstOrDefault().ToString(); // Removed Condition by Priyanka 13-08-2020
                    if (item.MAST_CON_ID.HasValue)
                    {
                        objParam.MAST_CONT_ID = item.MAST_CON_ID.Value;
                        obj.CONTRACTORNAME = dbContext.MASTER_CONTRACTOR.Where(c => c.MAST_CON_ID == item.MAST_CON_ID.Value).Select(d => d.MAST_CON_COMPANY_NAME).FirstOrDefault();
                        //commomFuncObj.GetContractorSupplierName(objParam);
                    }
                    else
                    {
                        obj.CONTRACTORNAME = String.Empty;
                    }

                    obj.isOperational = dbContext.ACC_MASTER_TXN.Where(x => x.TXN_ID == item.TXN_ID && x.TXN_PARENT_ID == item.masterHead).Select(x => x.IS_OPERATIONAL).FirstOrDefault();

                    obj.isRequiredAfterPorting = obj.isOperational == true ? true : dbContext.ACC_MASTER_TXN.Where(x => x.TXN_ID == item.TXN_ID && x.TXN_PARENT_ID == item.masterHead).Select(x => x.IS_REQ_AFTER_PORTING).FirstOrDefault();

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
                                        item.CASH_CHQ.ToString() =="C"? "Cash":( item.CASH_CHQ.ToString() =="Q" ? "Cheque":"Cash"),
                                        item.AMOUNT.ToString(),
                                        item.NARRATION.ToString(),
                                        //(item.BILL_FINALIZED=="N" &&( (item.MASTER_CHQEPAY=="C" && item.CASH_CHQ=="C") || (item.CASH_CHQ=="Q" ||item.CASH_CHQ=="D"))
                                        
                                        (item.BILL_FINALIZED=="N" &&( (item.MASTER_CHQEPAY=="C" && item.CASH_CHQ=="Q") || (item.CASH_CHQ=="Q" ||item.CASH_CHQ=="D"))
                                        ? 
                                        item.isRequiredAfterPorting==true
                                        ?
                                        "<center><a href='#' class='ui-icon ui-icon-pencil' onclick='EditTransactionPayment(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()  +"$"+ item.TXN_NO+"$"+ item.CASH_CHQ})+ "\");return false;'>Edit</a></center>"
                                        :
                                        "<center><a href='#' class='ui-icon ui-icon-pencil' onclick='BlockTransactionPayment();return false;'>Edit</a></center>"
                                        :
                                        ( item.BILL_FINALIZED=="Y" || item.BILL_FINALIZED=="E" &&((item.MASTER_CHQEPAY=="C" && item.CASH_CHQ=="C") || (item.CASH_CHQ=="Q" || item.CASH_CHQ=="D"))?  "<center><a href='#' class='ui-icon ui-icon-locked' onclick='ViewTransactionPayment(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()  +"$"+ item.TXN_NO+"$"+ item.CASH_CHQ})+ "\");return false;'>Voucher is finalized</a></center>": string.Empty)),    
                                        item.BILL_FINALIZED=="N" &&((item.MASTER_CHQEPAY=="C" && item.CASH_CHQ=="Q") || (item.CASH_CHQ=="Q" ||item.CASH_CHQ=="D"))? "<center><a href='#' class='ui-icon ui-icon-trash' onclick='DeleteTransactionPayment(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()  +"$"+ item.TXN_NO+"$"+ item.CASH_CHQ})+ "\");return false;'>Delete</a></center>" : "<span class='ui-icon ui-icon-locked ui-align-center'>"    ,                        
                                        (item.TXN_ID != 0 && item.TXN_ID != null) ? (item.isOperational==true ? "Correct Entry" :(item.isRequiredAfterPorting ==true ? "Edit And Correct the entry" :"Delete and Make new entry")) : "Correct Entry"

                                        //(((item.MASTER_TXN_ID==47)||(item.MASTER_TXN_ID==72)||(item.MASTER_TXN_ID==86)||(item.MASTER_TXN_ID==777)||(item.MASTER_TXN_ID==787))&&(item.MASTER_CHQEPAY=="C"))? ("<span class='ui-icon ui-icon-locked ui-align-center'>")  :

                                        // item.BILL_FINALIZED=="N" &&((item.MASTER_CHQEPAY=="C" && item.CASH_CHQ=="C") || (item.CASH_CHQ=="Q" ||item.CASH_CHQ=="D"))? 
                                        //item.isRequiredAfterPorting==true ? "<center><a href='#' class='ui-icon ui-icon-pencil' onclick='EditTransactionPayment(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()  +"$"+ item.TXN_NO+"$"+ item.CASH_CHQ})+ "\");return false;'>Edit</a></center>" :"<center><a href='#' class='ui-icon ui-icon-pencil' onclick='BlockTransactionPayment();return false;'>Edit</a></center>"
                                        //:( item.BILL_FINALIZED=="Y" || item.BILL_FINALIZED=="E" &&((item.MASTER_CHQEPAY=="C" && item.CASH_CHQ=="C") || (item.CASH_CHQ=="Q" || item.CASH_CHQ=="D"))?  "<center><a href='#' class='ui-icon ui-icon-locked' onclick='ViewTransactionPayment(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()  +"$"+ item.TXN_NO+"$"+ item.CASH_CHQ})+ "\");return false;'>Voucher is finalized</a></center>": string.Empty),    
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
        /// function to calculate the Payment amounts
        /// </summary>
        /// <param name="Bill_id"></param>
        /// <returns></returns>
        public AmountCalculationModel CalculatePaymentAmounts(Int64 Bill_id)
        {
            PMGSYEntities localDbContext = new PMGSYEntities();
            CommonFunctions common = new CommonFunctions();
            try
            {

                AmountCalculationModel amountModel = new AmountCalculationModel();


                TransactionParams objparams = new TransactionParams();
                ACC_SCREEN_DESIGN_PARAM_MASTER obj = new ACC_SCREEN_DESIGN_PARAM_MASTER();

                ACC_BILL_MASTER masteritem = new ACC_BILL_MASTER();
                masteritem = localDbContext.ACC_BILL_MASTER.Where(c => c.BILL_ID == Bill_id).FirstOrDefault();

                objparams.TXN_ID = masteritem.TXN_ID;

                //Added By Abhishek kamble 16-Apr-2014 start    
                amountModel.TransactionId = masteritem.TXN_ID;
                //Added By Abhishek kamble 16-Apr-2014 end

                obj = common.getMasterDesignParam(objparams);
                if (obj.DED_REQ == "B" && masteritem.CHQ_EPAY != "C")
                {

                    amountModel.CashPayment = "N";
                }
                else
                {
                    if (obj.DED_REQ != "B" && masteritem.CHQ_EPAY == "C")
                    {
                        amountModel.CashPayment = "Y";
                    }
                    else
                    {
                        amountModel.CashPayment = "N";
                    }
                }

                if (localDbContext.ACC_BILL_DETAILS.Any(x => x.BILL_ID == Bill_id))
                {

                    //Added By Abhishek kamble 16-Apr-2014 start    
                    amountModel.IsDetailsEntered = true;
                    //Added By Abhishek kamble 16-Apr-2014 end
                    var query = (from master in localDbContext.ACC_BILL_MASTER
                                 join details in localDbContext.ACC_BILL_DETAILS
                                 on master.BILL_ID equals details.BILL_ID
                                 where master.BILL_ID == Bill_id && details.CREDIT_DEBIT == "D"

                                 select new
                                 {
                                     master.CHQ_AMOUNT,
                                     master.CASH_AMOUNT,
                                     master.BILL_FINALIZED,
                                     details.CASH_CHQ,
                                     details.AMOUNT,
                                     master.CHQ_EPAY,
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
                            //if master payment is of the cash and only deduction are there
                            if (item.CHQ_EPAY == "C")
                            {
                                ///Added by SAMMED A. PATIL on 05APR2018  (item.TXN_ID != 1484)
                                //if ((item.TXN_ID != 47) && (item.TXN_ID != 72) && (item.TXN_ID != 737) && (item.TXN_ID != 86) && (item.TXN_ID != 777) && (item.TXN_ID != 787) && (item.TXN_ID != 415) && (item.TXN_ID != 1484))
                                //{
                                //    amountModel.TotalAmtEnteredCachAmount = amountModel.TotalAmtEnteredCachAmount + item.AMOUNT;
                                //}
                                
                                //Below Code Added on 23-12-2021 
                                if (item.CHQ_EPAY == "C")
                                {
                                    if (!(masteritem.CHQ_AMOUNT == 0 && masteritem.CASH_AMOUNT > 0))
                                    {
                                        amountModel.TotalAmtEnteredCachAmount = amountModel.TotalAmtEnteredCachAmount + item.AMOUNT;
                                    }

                                }
                            }
                        }


                        // amountModel.TotalAmtEnteredGrossAmount = amountModel.TotalAmtEnteredGrossAmount + (amountModel.TotalAmtEnteredCachAmount + amountModel.TotalAmtEnteredChqAmount);
                        amountModel.VoucherFinalized = item.BILL_FINALIZED;

                    }

                    amountModel.TotalAmtEnteredGrossAmount = (amountModel.TotalAmtEnteredCachAmount + amountModel.TotalAmtEnteredChqAmount);
                    amountModel.DiffGrossAmount = amountModel.TotalAmtToEnterGrossAmount - amountModel.TotalAmtEnteredGrossAmount;
                    if (amountModel.CashPayment == "Y")
                    {
                        amountModel.DiffDedAmount = 0;
                        amountModel.TotalAmtEnteredDedAmount = 0;
                        amountModel.TotalAmtToEnterDedAmount = 0;
                    }
                    else
                    {

                        amountModel.DiffDedAmount = amountModel.TotalAmtToEnterCachAmount - amountModel.TotalAmtEnteredDedAmount;
                    }

                    amountModel.DiffChqAmount = amountModel.TotalAmtToEnterChqAmount - amountModel.TotalAmtEnteredChqAmount;
                    amountModel.DiffCachAmount = amountModel.TotalAmtToEnterCachAmount - amountModel.TotalAmtEnteredCachAmount;

                }
                else
                {
                    //Added By Abhishek kamble 16-Apr-2014 start    
                    amountModel.IsDetailsEntered = false;
                    //Added By Abhishek kamble 16-Apr-2014 end

                    amountModel.TotalAmtToEnterCachAmount = masteritem.CASH_AMOUNT;
                    amountModel.TotalAmtToEnterChqAmount = masteritem.CHQ_AMOUNT;

                    amountModel.TotalAmtToEnterGrossAmount = (amountModel.TotalAmtToEnterCachAmount + amountModel.TotalAmtToEnterChqAmount);

                    amountModel.DiffCachAmount = masteritem.CASH_AMOUNT;
                    amountModel.DiffChqAmount = masteritem.CHQ_AMOUNT;

                    amountModel.DiffGrossAmount = amountModel.TotalAmtToEnterGrossAmount;

                    if (amountModel.CashPayment == "Y")
                    {
                        amountModel.DiffDedAmount = 0;
                        amountModel.TotalAmtEnteredDedAmount = 0;
                        amountModel.TotalAmtToEnterCachAmount = 0;
                    }
                    else
                    {
                        amountModel.TotalAmtToEnterDedAmount = masteritem.CASH_AMOUNT;
                        amountModel.DiffDedAmount = masteritem.CASH_AMOUNT;
                    }


                }

                return amountModel;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error while calculating payment amounts.");
            }
            finally
            {
                localDbContext.Dispose();
            }
        }


        /// <summary>
        /// function to check whether voucher can be finalized
        /// </summary>
        /// <param name="Bill_id"></param>
        /// <returns></returns>
        public bool CanVoucherFinalized(Int64 Bill_id)
        {
            PMGSYEntities localDbContext = new PMGSYEntities();

            try
            {

                AmountCalculationModel data = new AmountCalculationModel();

                data = CalculatePaymentAmounts(Bill_id);

                if (data.DiffGrossAmount == 0 && data.DiffChqAmount == 0 & data.DiffDedAmount == 0 && data.DiffGrossAmount == 0)
                {
                    return true;

                }
                else return false;
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error while  checking if voucher can be finalized.");
            }
            finally
            {
                localDbContext.Dispose();
            }

        }


        //added by vikram 28-8-2013
        public decimal? GetVoucherPayment(Int64 billId, out string paymentType)
        {
            dbContext = new PMGSYEntities();
            decimal? payment = null;
            try
            {
                //Modified by Abhishek kamble 30-May-2014 select old 'GROSS_AMT' New 'CHQ_AMOUNT'
                payment = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == billId).Select(m => m.CHQ_AMOUNT).FirstOrDefault();
                paymentType = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == billId).Select(m => m.CHQ_EPAY).FirstOrDefault();
                if (paymentType == "E" || paymentType == "A")//Advice no change 8Apr2015
                {
                    paymentType = "Q";
                }
                return payment;
            }
            catch (Exception ex)
            {
                paymentType = string.Empty;
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


        // public MASTER_CONTRACTOR_BANK GetContratorBankAccNoAndIFSCcode(int contractorId, string fundType, int txnID, ref bool isPFMSFinalized, bool isAdvicePayment, bool isChqPaymentAllowed)
        public List<SelectListItem> GetContratorBankAccNoAndIFSCcode(int contractorId, string fundType, int txnID, ref bool isPFMSFinalized, bool isAdvicePayment, bool isChqPaymentAllowed)
        {
            dbContext = new PMGSYEntities();
            List<SelectListItem> lstMastContractorAccount = new List<SelectListItem>();
            //lstMastContractorAccount.Insert(0, (new SelectListItem { Text = "---Select Account---", Value = "0" }));
            lstMastContractorAccount.Insert(0, (new SelectListItem { Text = "---Select Account---", Value = "null" })); 
            List<reat_list_contractor_details_Result> resultREAT = new List<reat_list_contractor_details_Result>();
            int lgdDistrictCode = 0;
            int lgdStateCode = 0;
            try
            {
                // MASTER_CONTRACTOR_BANK contratorBankDetails = dbContext.MASTER_CONTRACTOR_BANK.Where(m => m.MAST_CON_ID == contractorId && m.MAST_ACCOUNT_STATUS=="A").FirstOrDefault();

                MASTER_CONTRACTOR_BANK contratorBankDetails = new MASTER_CONTRACTOR_BANK();


                string moduleType = "R";
                //code commented on 13-oct-2021 to enable REAT payment 
                //string moduleType = "D";
                //REAT_OMMAS_PAYMENT_SUCCESS objModuleType = dbContext.REAT_OMMAS_PAYMENT_SUCCESS.FirstOrDefault(s => s.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode);
                //if (objModuleType != null)
                //{
                //    moduleType = "R";
                //}



                //if (PMGSYSession.Current.FundType.Equals("P"))
                if (((PMGSYSession.Current.FundType.Equals("P") || PMGSYSession.Current.FundType.Equals("A")) && !isChqPaymentAllowed) || (PMGSYSession.Current.FundType.Equals("M") && !isAdvicePayment || !isChqPaymentAllowed))//MF Advice Payment
                {
                    if (moduleType.Equals("D"))
                    {
                        PFMS_OMMAS_CONTRACTOR_MAPPING pfmsContratorBankDetails = new PFMS_OMMAS_CONTRACTOR_MAPPING();
                        lgdDistrictCode = dbContext.OMMAS_LDG_DISTRICT_MAPPING.Where(x => x.MAST_DISTRICT_CODE == PMGSYSession.Current.DistrictCode).Select(x => x.MAST_DISTRICT_LDG_CODE).FirstOrDefault();
                        lgdStateCode = dbContext.OMMAS_LDG_STATE_MAPPING.Where(x => x.MAST_STATE_CODE == PMGSYSession.Current.StateCode).Select(x => x.MAST_STATE_LDG_CODE).FirstOrDefault();

                        //contratorBankDetails = dbContext.MASTER_CONTRACTOR_BANK.Where(v => v.MAST_CON_ID == contractorId && v.MAST_ACCOUNT_STATUS == "A" && v.MAST_DISTRICT_CODE == PMGSYSession.Current.DistrictCode && v.MAST_LOCK_STATUS == "Y").FirstOrDefault();
                        pfmsContratorBankDetails = dbContext.PFMS_OMMAS_CONTRACTOR_MAPPING.Where(v => v.MAST_CON_ID == contractorId && v.MAST_LGD_STATE_CODE == lgdStateCode && v.MAST_LGD_DISTRICT_CODE == lgdDistrictCode && v.PFMS_CON_ID != null && v.STATUS == "A" && v.MAST_AGENCY_CODE == PMGSYSession.Current.MastAgencyCode
                            && v.MASTER_CONTRACTOR.MASTER_CONTRACTOR_BANK.Where(z => z.MAST_CON_ID == contractorId && z.MAST_ACCOUNT_ID == v.MAST_ACCOUNT_ID && z.MAST_LOCK_STATUS == "Y").Select(x => x.MAST_ACCOUNT_STATUS).FirstOrDefault() == "A"
                            ).FirstOrDefault();
                        if (pfmsContratorBankDetails == null)
                        {
                            if (PMGSYSession.Current.StateCode > 0)
                            {
                                var districts = dbContext.MASTER_DISTRICT.Where(x => x.MAST_STATE_CODE == PMGSYSession.Current.StateCode && x.MAST_DISTRICT_ACTIVE == "Y" && x.OMMAS_LDG_DISTRICT_MAPPING != null).Select(m => m.OMMAS_LDG_DISTRICT_MAPPING.MAST_DISTRICT_LDG_CODE).ToList();
                                var districtslist = dbContext.MASTER_DISTRICT.Where(x => x.MAST_STATE_CODE == PMGSYSession.Current.StateCode && x.MAST_DISTRICT_ACTIVE == "Y").Select(x => x.MAST_DISTRICT_CODE).ToList();
                                //var districts = dbContext.OMMAS_LDG_DISTRICT_MAPPING.Where(districtslist.Contains(x => x.MAST_DISTRICT_LDG_CODE)).Select(x => x.MAST_DISTRICT_LDG_CODE).FirstOrDefault();
                                if (districts != null)
                                {
                                    //if (dbContext.MASTER_CONTRACTOR_BANK.Any(v => v.MAST_CON_ID == contractorId && v.MAST_ACCOUNT_STATUS == "A"))
                                    if (dbContext.PFMS_OMMAS_CONTRACTOR_MAPPING.Any(v => v.MAST_CON_ID == contractorId && v.PFMS_CON_ID != null && v.STATUS == "A"))
                                    {
                                        pfmsContratorBankDetails = dbContext.PFMS_OMMAS_CONTRACTOR_MAPPING.Where(v => v.MAST_CON_ID == contractorId && v.PFMS_CON_ID != null && v.STATUS == "A"
                                            //&& v.MASTER_CONTRACTOR.MASTER_CONTRACTOR_REGISTRATION.Where(x => x.MAST_CON_ID == contractorId).Select(x => x.MAST_REG_STATE).FirstOrDefault() == PMGSYSession.Current.StateCode
                                            && districts.Contains(v.MAST_LGD_DISTRICT_CODE.Value)
                                            && v.MAST_LGD_STATE_CODE == lgdStateCode && v.MAST_AGENCY_CODE == PMGSYSession.Current.MastAgencyCode
                                            && v.MASTER_CONTRACTOR.MASTER_CONTRACTOR_BANK.Where(z => z.MAST_CON_ID == contractorId && z.MAST_ACCOUNT_ID == v.MAST_ACCOUNT_ID && z.MAST_LOCK_STATUS == "Y").Select(x => x.MAST_ACCOUNT_STATUS).FirstOrDefault() == "A"
                                            ).FirstOrDefault();
                                    }
                                    if (pfmsContratorBankDetails == null)
                                    {
                                        //pfmsContratorBankDetails = dbContext.MASTER_CONTRACTOR_BANK.Where(v => v.MAST_CON_ID == contractorId && v.MAST_ACCOUNT_STATUS == "A" && v.MAST_LOCK_STATUS == "Y").FirstOrDefault();
                                        pfmsContratorBankDetails = dbContext.PFMS_OMMAS_CONTRACTOR_MAPPING.Where(v => v.MAST_CON_ID == contractorId && v.PFMS_CON_ID != null && v.STATUS == "A" && v.MAST_LGD_STATE_CODE == lgdStateCode && v.MAST_AGENCY_CODE == PMGSYSession.Current.MastAgencyCode && v.MASTER_CONTRACTOR.MASTER_CONTRACTOR_BANK.Where(z => z.MAST_CON_ID == contractorId && z.MAST_ACCOUNT_ID == v.MAST_ACCOUNT_ID && z.MAST_LOCK_STATUS == "Y").Select(x => x.MAST_ACCOUNT_STATUS).FirstOrDefault() == "A").FirstOrDefault();
                                    }
                                }
                                else
                                {
                                    //pfmsContratorBankDetails = dbContext.MASTER_CONTRACTOR_BANK.Where(v => v.MAST_CON_ID == contractorId && v.MAST_ACCOUNT_STATUS == "A" && v.MAST_LOCK_STATUS == "Y").FirstOrDefault();
                                    pfmsContratorBankDetails = dbContext.PFMS_OMMAS_CONTRACTOR_MAPPING.Where(v => v.MAST_CON_ID == contractorId && v.PFMS_CON_ID != null && v.STATUS == "A" && v.MAST_LGD_STATE_CODE == lgdStateCode && v.MAST_AGENCY_CODE == PMGSYSession.Current.MastAgencyCode
                                        && v.MASTER_CONTRACTOR.MASTER_CONTRACTOR_BANK.Where(z => z.MAST_CON_ID == contractorId && z.MAST_ACCOUNT_ID == v.MAST_ACCOUNT_ID && z.MAST_LOCK_STATUS == "Y").Select(x => x.MAST_ACCOUNT_STATUS).FirstOrDefault() == "A").FirstOrDefault();
                                }
                            }
                            else
                            {
                                //pfmsContratorBankDetails = dbContext.MASTER_CONTRACTOR_BANK.Where(v => v.MAST_CON_ID == contractorId && v.MAST_ACCOUNT_STATUS == "A" && v.MAST_LOCK_STATUS == "Y").FirstOrDefault();
                                pfmsContratorBankDetails = dbContext.PFMS_OMMAS_CONTRACTOR_MAPPING.Where(v => v.MAST_CON_ID == contractorId && v.MAST_LGD_STATE_CODE == lgdStateCode && v.MAST_AGENCY_CODE == PMGSYSession.Current.MastAgencyCode && v.PFMS_CON_ID != null && v.STATUS == "A" && v.MASTER_CONTRACTOR.MASTER_CONTRACTOR_BANK.Where(z => z.MAST_CON_ID == contractorId && z.MAST_ACCOUNT_ID == v.MAST_ACCOUNT_ID && z.MAST_LOCK_STATUS == "Y").Select(x => x.MAST_ACCOUNT_STATUS).FirstOrDefault() == "A").FirstOrDefault();
                            }
                        }
                        if (pfmsContratorBankDetails != null)
                        {
                            contratorBankDetails.MAST_ACCOUNT_NUMBER = pfmsContratorBankDetails.MAST_ACCOUNT_NUMBER;
                            contratorBankDetails.MAST_IFSC_CODE = pfmsContratorBankDetails.MAST_IFSC_CODE;
                            contratorBankDetails.MAST_BANK_NAME = pfmsContratorBankDetails.MAST_BANK_NAME;
                            contratorBankDetails.MAST_ACCOUNT_ID = pfmsContratorBankDetails.MAST_ACCOUNT_ID;
                        }
                    }
                    if (moduleType.Equals("R"))
                    {

                        resultREAT = dbContext.reat_list_contractor_details(PMGSYSession.Current.DistrictCode, PMGSYSession.Current.StateCode, contractorId).ToList<reat_list_contractor_details_Result>();
                        //  List<reat_list>
                        // reat_list_contractor_details_Result
                        foreach (reat_list_contractor_details_Result item in resultREAT)
                        {
                            contratorBankDetails.MAST_ACCOUNT_NUMBER = item.MAST_ACCOUNT_NUMBER;
                            contratorBankDetails.MAST_IFSC_CODE = item.MAST_IFSC_CODE;
                            contratorBankDetails.MAST_BANK_NAME = item.MAST_BANK_NAME;
                            contratorBankDetails.MAST_ACCOUNT_ID = item.MAST_ACCOUNT_ID;
                        }

                    }
                }
                //else if (PMGSYSession.Current.FundType.Equals("M") || txnID == 455)
                //else if (txnID == 455 || (PMGSYSession.Current.FundType.Equals("M") && isAdvicePayment) || (PMGSYSession.Current.FundType.Equals("P") && isChqPaymentAllowed))
                //else if (txnID == 455 || (PMGSYSession.Current.FundType.Equals("M") && isAdvicePayment) || (PMGSYSession.Current.FundType.Equals("M") && isChqPaymentAllowed) || (PMGSYSession.Current.FundType.Equals("P") && isChqPaymentAllowed))  // --- Punjab Change
                //Below Condition is modified on 20-01-2022
                else if (txnID == 455 || (PMGSYSession.Current.FundType.Equals("M") && isAdvicePayment) || (PMGSYSession.Current.FundType.Equals("M") && isChqPaymentAllowed) || (PMGSYSession.Current.FundType.Equals("P") && isChqPaymentAllowed) || (PMGSYSession.Current.FundType.Equals("A") && isChqPaymentAllowed))  // --- Punjab Change
                {
                    //  con = dbContext.MASTER_CONTRACTOR_BANK.Where(v => v.MAST_CON_ID == bill.MAST_CON_ID && v.MAST_ACCOUNT_STATUS == "A").FirstOrDefault();// FirstOrDefault Checked By Abhishek kamble 8-Apt-2014
                    contratorBankDetails = dbContext.MASTER_CONTRACTOR_BANK.Where(v => v.MAST_CON_ID == contractorId && v.MAST_ACCOUNT_STATUS == "A" && v.MAST_DISTRICT_CODE == PMGSYSession.Current.DistrictCode).FirstOrDefault();
                    if (contratorBankDetails == null)
                    {
                        if (PMGSYSession.Current.StateCode > 0)
                        {
                            var districts = dbContext.MASTER_DISTRICT.Where(x => x.MAST_STATE_CODE == PMGSYSession.Current.StateCode && x.MAST_DISTRICT_ACTIVE == "Y").Select(x => x.MAST_DISTRICT_CODE).ToList();
                            if (districts != null)
                            {
                                if (dbContext.MASTER_CONTRACTOR_BANK.Any(v => v.MAST_CON_ID == contractorId && v.MAST_ACCOUNT_STATUS == "A"))
                                {
                                    contratorBankDetails = dbContext.MASTER_CONTRACTOR_BANK.Where(v => v.MAST_CON_ID == contractorId && v.MAST_ACCOUNT_STATUS == "A"
                                        //&& v.MASTER_CONTRACTOR.MASTER_CONTRACTOR_REGISTRATION.Where(x => x.MAST_CON_ID == contractorId).Select(x => x.MAST_REG_STATE).FirstOrDefault() == PMGSYSession.Current.StateCode
                                        && districts.Contains(v.MAST_DISTRICT_CODE)
                                        ).FirstOrDefault();

                                    if (contratorBankDetails == null)
                                    {
                                        contratorBankDetails = dbContext.MASTER_CONTRACTOR_BANK.Where(v => v.MAST_CON_ID == contractorId && v.MAST_ACCOUNT_STATUS == "A").FirstOrDefault();
                                    }
                                }
                            }
                            else
                            {
                                contratorBankDetails = dbContext.MASTER_CONTRACTOR_BANK.Where(v => v.MAST_CON_ID == contractorId && v.MAST_ACCOUNT_STATUS == "A").FirstOrDefault();
                            }
                        }
                        else
                        {
                            contratorBankDetails = dbContext.MASTER_CONTRACTOR_BANK.Where(v => v.MAST_CON_ID == contractorId && v.MAST_ACCOUNT_STATUS == "A").FirstOrDefault();
                        }

                    }
                }
                //Below Code commented on 17-11-2021
                //else if (PMGSYSession.Current.FundType.Equals("A") || txnID == 415 || txnID == 472)
                //{
                //    ADMIN_NO_BANK noBankDetails = new ADMIN_NO_BANK();
                //    noBankDetails = dbContext.ADMIN_NO_BANK.Where(v => v.ADMIN_NO_OFFICER_CODE == contractorId && v.MAST_ACCOUNT_STATUS == "A").FirstOrDefault();

                //    contratorBankDetails.MAST_ACCOUNT_NUMBER = noBankDetails.MAST_ACCOUNT_NUMBER;
                //    contratorBankDetails.MAST_IFSC_CODE = noBankDetails.MAST_IFSC_CODE;
                //    contratorBankDetails.MAST_BANK_NAME = noBankDetails.MAST_BANK_NAME;
                //    contratorBankDetails.MAST_ACCOUNT_ID = noBankDetails.MAST_ACCOUNT_ID;

                //}
                ///PFMS Validations
                //if (contratorBankDetails != null)
                //{
                //    isPFMSFinalized = dbContext.PFMS_OMMAS_CONTRACTOR_MAPPING.Any(x => x.MAST_CON_ID == contractorId && x.MAST_ACCOUNT_ID == contratorBankDetails.MAST_ACCOUNT_ID && x.PFMS_CON_ID != null);
                //}

                List<MASTER_CONTRACTOR_BANK> lstBankAcc = new List<MASTER_CONTRACTOR_BANK>();
                if (moduleType.Equals("D"))
                {
                    lstBankAcc.Add(contratorBankDetails);
                    // lstMastContractorAccount.Add(new SelectListItem { Text =  item.MAST_BANK_NAME + ":" + item.MAST_IFSC_CODE + ":"+  item.MAST_ACCOUNT_NUMBER , Value = item.MAST_ACCOUNT_ID.ToString() });
                }
                if (moduleType.Equals("R"))
                {

                    if (resultREAT != null && resultREAT.Count > 0)
                    {

                        foreach (reat_list_contractor_details_Result item in resultREAT)
                        {
                            MASTER_CONTRACTOR_BANK contratorBankDetails1 = new MASTER_CONTRACTOR_BANK();

                            contratorBankDetails1.MAST_ACCOUNT_NUMBER = item.MAST_ACCOUNT_NUMBER;
                            contratorBankDetails1.MAST_IFSC_CODE = item.MAST_IFSC_CODE;
                            contratorBankDetails1.MAST_BANK_NAME = item.MAST_BANK_NAME;
                            contratorBankDetails1.MAST_ACCOUNT_ID = item.MAST_ACCOUNT_ID;
                            lstBankAcc.Add(contratorBankDetails1);
                        }

                    }
                    else
                    {
                        //foreach (MASTER_CONTRACTOR_BANK item in contratorBankDetails)
                        //{
                        try
                        {
                            lstBankAcc.Add(contratorBankDetails);

                            //lstBankAcc.Add(contratorBankDetails);
                            //}
                        }
                        catch (Exception ex)
                        {
                            ex.Message.ToString();
                        }
                    }
                }


                foreach (var item in lstBankAcc)
                {

                    lstMastContractorAccount.Add(new SelectListItem { Text = item.MAST_BANK_NAME + ":" + item.MAST_IFSC_CODE + ":" + item.MAST_ACCOUNT_NUMBER, Value = item.MAST_ACCOUNT_ID.ToString() });
                };

                return lstMastContractorAccount;


            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "PaymentDAL.GetContratorBankAccNoAndIFSCcode()");
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

        #region Transaction deduction Payment
        /// <summary>
        /// function to add the transactions into the bill details table
        /// </summary>
        /// <param name="model"></param>
        /// <param name="operationType">Transaction (cheque/Cash) or Deduction ,T:Transaction & D:Deduction </param>
        /// <param name="Bill_id">Primary key from master table</param>
        /// <param name="AddorEdit">Add operation or Edit operation</param>
        ///  /// <param name="tranNumber">debit transnumber of the entry to be edited</param>
        /// <returns>boolean value indicating operation is successful or not </returns>
        public Boolean AddEditTransactionDeductionPaymentDetails(PaymentDetailsModel model, string operationType, Int64 Bill_id, String AddorEdit, Int16 tranNumber)
        {
            try
            {
                dbContext = new PMGSYEntities();
                ACC_BILL_DETAILS ModelToAdd = null;
                Int16 maxTxnNo = 0;
                ACC_BILL_DETAILS PaymentTransaction = new ACC_BILL_DETAILS();

                string masterChequeCash = string.Empty;

                //masterChequeCash = dbContext.ACC_BILL_MASTER.Where(t => t.BILL_ID == Bill_id).Select(b => b.CHQ_EPAY).First();

                ACC_BILL_MASTER billMasterModel = dbContext.ACC_BILL_MASTER.Where(t => t.BILL_ID == Bill_id).FirstOrDefault();

                masterChequeCash = billMasterModel.CHQ_EPAY;

                if (masterChequeCash.Equals("E") || masterChequeCash.Equals("A"))    //modified by Abhishek kamble for Advice No 7Apr2015
                {
                    masterChequeCash = "Q";
                }

                ///if operation is edit get the transaction details from database for credit entry
                if (AddorEdit.Equals("E"))
                {
                    PaymentTransaction = dbContext.ACC_BILL_DETAILS.Where(t => t.BILL_ID == Bill_id && t.TXN_NO == (tranNumber - 1)).FirstOrDefault();
                }

                //if simple transaction
                if (operationType.Equals("T"))
                {
                    using (var scope = new TransactionScope())
                    {

                        //if operation is ADD
                        if (AddorEdit.Equals("A"))
                        {
                            if (dbContext.ACC_BILL_DETAILS.Where(t => t.BILL_ID == Bill_id).Any())
                            {
                                maxTxnNo = dbContext.ACC_BILL_DETAILS.Where(t => t.BILL_ID == Bill_id).Max(c => c.TXN_NO);
                            }

                            //creating credit entry Cheque payment
                            maxTxnNo = Convert.ToInt16(maxTxnNo + 1);

                            if (model.CASH_CHQ.Trim().Equals("E"))
                            {
                                model.CASH_CHQ = "Q";
                            }

                        }
                        else
                        {

                            maxTxnNo = PaymentTransaction.TXN_NO;
                            model.CASH_CHQ = PaymentTransaction.CASH_CHQ;
                            model.ND_CODE = PaymentTransaction.ADMIN_ND_CODE;
                            //model.CON_ID = PaymentTransaction.MAST_CON_ID;

                        }

                        ModelToAdd = new ACC_BILL_DETAILS();

                        Int16 TXN_ID_Model = Convert.ToInt16(model.HEAD_ID_P.ToString().Split('$')[0]);

                        //String CashChq = Convert.ToString(model.HEAD_ID_P.ToString().Split('$')[1]); 

                        //get the credit head
                        //for this credit head use the cash/cheque from master table 

                        ModelToAdd.HEAD_ID = (from item in dbContext.ACC_TXN_HEAD_MAPPING
                                              where item.TXN_ID == TXN_ID_Model && item.CASH_CHQ.Contains(masterChequeCash) && item.CREDIT_DEBIT == "C"
                                              select item.HEAD_ID).FirstOrDefault();

                        ModelToAdd.BILL_ID = Bill_id;

                        //find transaction number only if operation is add

                        ModelToAdd.TXN_NO = maxTxnNo;
                        ModelToAdd.TXN_ID = TXN_ID_Model;
                        ModelToAdd.AMOUNT = model.AMOUNT_Q.HasValue ? model.AMOUNT_Q.Value : 0;
                        ModelToAdd.CREDIT_DEBIT = "C";
                        //ModelToAdd.CASH_CHQ = model.CASH_CHQ;
                        //ModelToAdd.CASH_CHQ = "Q";

                        //Added by abhishek for deduction only entry add cash_chq as Q 
                        if (((billMasterModel.TXN_ID == 47) || (billMasterModel.TXN_ID == 72) || (billMasterModel.TXN_ID == 86) || (billMasterModel.TXN_ID == 737) || (billMasterModel.TXN_ID == 777) || (billMasterModel.TXN_ID == 787)) && (billMasterModel.CHQ_EPAY == "C"))
                        {
                            ModelToAdd.CASH_CHQ = "Q";
                        }
                        else
                        {
                            ModelToAdd.CASH_CHQ = model.CASH_CHQ;
                        }


                        ModelToAdd.NARRATION = model.NARRATION_P;
                        ModelToAdd.ADMIN_ND_CODE = model.ND_CODE;
                        ModelToAdd.MAST_CON_ID = model.CON_ID;
                        if (!(model.IMS_PR_ROAD_CODE == null || model.IMS_PR_ROAD_CODE == 0))
                        {
                            ModelToAdd.IMS_PR_ROAD_CODE = model.IMS_PR_ROAD_CODE;
                        }

                        // ModelToAdd.IMS_PR_ROAD_CODE = model.IMS_PR_ROAD_CODE;
                        ModelToAdd.IMS_AGREEMENT_CODE = model.IMS_AGREEMENT_CODE;
                        ModelToAdd.MAS_FA_CODE = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == (model.IMS_PR_ROAD_CODE.HasValue ? model.IMS_PR_ROAD_CODE.Value : -1)).Select(v => v.IMS_COLLABORATION).FirstOrDefault();
                        ModelToAdd.FINAL_PAYMENT = model.FINAL_PAYMENT;

                        //Added By Abhishek Kamble 28-nov-2013
                        ModelToAdd.USERID = PMGSYSession.Current.UserId;
                        ModelToAdd.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];


                        // ModelToAdd.MAST_DISTRICT_CODE = model.MAST_DISTRICT_CODE;

                        //creating debit cheque payment entry

                        ACC_BILL_DETAILS ModelToAddDebit = new ACC_BILL_DETAILS();

                        maxTxnNo = Convert.ToInt16(maxTxnNo + 1);

                        //get the debit head
                        ModelToAddDebit.HEAD_ID = (from item in dbContext.ACC_TXN_HEAD_MAPPING
                                                   where item.TXN_ID == TXN_ID_Model && item.CASH_CHQ.Contains(masterChequeCash) && item.CREDIT_DEBIT == "D"
                                                   select item.HEAD_ID).FirstOrDefault();

                        ModelToAddDebit.BILL_ID = Bill_id;
                        ModelToAddDebit.TXN_NO = maxTxnNo;
                        ModelToAddDebit.TXN_ID = TXN_ID_Model;
                        ModelToAddDebit.AMOUNT = model.AMOUNT_Q.HasValue ? model.AMOUNT_Q.Value : 0;
                        ModelToAddDebit.CREDIT_DEBIT = "D";
                        //ModelToAddDebit.CASH_CHQ = model.CASH_CHQ;  
                        //ModelToAddDebit.CASH_CHQ = "Q";

                        //Added by abhishek for deduction only entry add cash_chq as Q 
                        if (((billMasterModel.TXN_ID == 47) || (billMasterModel.TXN_ID == 72) || (billMasterModel.TXN_ID == 86) || (billMasterModel.TXN_ID == 737) || (billMasterModel.TXN_ID == 777) || (billMasterModel.TXN_ID == 787)) && (billMasterModel.CHQ_EPAY == "C"))
                        {
                            ModelToAddDebit.CASH_CHQ = "Q";
                        }
                        else
                        {
                            ModelToAddDebit.CASH_CHQ = model.CASH_CHQ;
                        }

                        ModelToAddDebit.NARRATION = model.NARRATION_P;
                        ModelToAddDebit.ADMIN_ND_CODE = model.ND_CODE;
                        ModelToAddDebit.MAST_CON_ID = model.CON_ID;
                        // ModelToAddDebit.IMS_PR_ROAD_CODE = model.IMS_PR_ROAD_CODE == 0 ? null : model.IMS_PR_ROAD_CODE;
                        if (!(model.IMS_PR_ROAD_CODE == null || model.IMS_PR_ROAD_CODE == 0))
                        {
                            ModelToAddDebit.IMS_PR_ROAD_CODE = model.IMS_PR_ROAD_CODE;
                        }


                        ModelToAddDebit.IMS_AGREEMENT_CODE = model.IMS_AGREEMENT_CODE;
                        ModelToAddDebit.MAS_FA_CODE = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == (model.IMS_PR_ROAD_CODE.HasValue ? model.IMS_PR_ROAD_CODE.Value : -1)).Select(v => v.IMS_COLLABORATION).FirstOrDefault();
                        ModelToAddDebit.FINAL_PAYMENT = model.FINAL_PAYMENT;

                        //Added By Abhishek Kamble 28-nov-2013
                        ModelToAddDebit.USERID = PMGSYSession.Current.UserId;
                        ModelToAddDebit.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];


                        // ModelToAddDebit.MAST_DISTRICT_CODE = model.MAST_DISTRICT_CODE;

                        //add both entries if add operation
                        if (AddorEdit.Equals("A"))
                        {
                            dbContext.ACC_BILL_DETAILS.Add(ModelToAdd);
                            dbContext.ACC_BILL_DETAILS.Add(ModelToAddDebit);
                        }
                        else
                        {
                            //make its state as modified

                            ACC_BILL_DETAILS old_acc_bill_details = new ACC_BILL_DETAILS();
                            old_acc_bill_details = dbContext.ACC_BILL_DETAILS.Where(x => x.BILL_ID == Bill_id && x.TXN_NO == (tranNumber - 1)).FirstOrDefault();
                            dbContext.Entry(old_acc_bill_details).CurrentValues.SetValues(ModelToAdd);

                            old_acc_bill_details = null;
                            old_acc_bill_details = dbContext.ACC_BILL_DETAILS.Where(x => x.BILL_ID == Bill_id && x.TXN_NO == (tranNumber)).FirstOrDefault();
                            dbContext.Entry(old_acc_bill_details).CurrentValues.SetValues(ModelToAddDebit);


                        }


                        #region cash amount data entry
                        {
                            //if  cash amount is not equal to 0
                            //if (dbContext.ACC_BILL_MASTER.Where(t => t.BILL_ID == Bill_id).Select(b => b.CASH_AMOUNT).First() != 0
                            //    && dbContext.ACC_BILL_MASTER.Where(t => t.BILL_ID == Bill_id).Select(b => b.CHQ_AMOUNT).First() != 0)
                            //{

                            //Added By Abhishek kamble 19-June-2014 start
                            CommonFunctions objCommon = new CommonFunctions();
                            ACC_SCREEN_DESIGN_PARAM_MASTER obj = new ACC_SCREEN_DESIGN_PARAM_MASTER();
                            TransactionParams objparams1 = new TransactionParams();
                            objparams1.TXN_ID = dbContext.ACC_BILL_MASTER.Where(t => t.BILL_ID == Bill_id).Select(b => b.TXN_ID).First();
                            obj = objCommon.getMasterDesignParam(objparams1);

                            //Added By Abhishek kamble 19-June-2014  start
                            if ((obj.DED_REQ == "B") || (obj.DED_REQ == "Y"))
                            {
                                //create credit entry for cash
                                ACC_BILL_DETAILS CashModelCredit = new ACC_BILL_DETAILS();

                                maxTxnNo = Convert.ToInt16(maxTxnNo + 1);

                                //get the debit head
                                CashModelCredit.HEAD_ID = (from item in dbContext.ACC_TXN_HEAD_MAPPING
                                                           where item.TXN_ID == TXN_ID_Model && item.CASH_CHQ.Trim().Contains("C") && item.CREDIT_DEBIT == "C"
                                                           select item.HEAD_ID).FirstOrDefault();

                                CashModelCredit.BILL_ID = Bill_id;
                                CashModelCredit.TXN_NO = maxTxnNo;
                                CashModelCredit.TXN_ID = TXN_ID_Model;
                                CashModelCredit.AMOUNT = model.AMOUNT_C.HasValue ? model.AMOUNT_C.Value : 0;
                                CashModelCredit.CREDIT_DEBIT = "C";
                                CashModelCredit.CASH_CHQ = "C";
                                CashModelCredit.NARRATION = model.NARRATION_P;
                                CashModelCredit.ADMIN_ND_CODE = model.ND_CODE;
                                CashModelCredit.MAST_CON_ID = model.CON_ID;
                                if (!(model.IMS_PR_ROAD_CODE == null || model.IMS_PR_ROAD_CODE == 0))
                                {
                                    CashModelCredit.IMS_PR_ROAD_CODE = model.IMS_PR_ROAD_CODE;
                                }
                                CashModelCredit.IMS_AGREEMENT_CODE = model.IMS_AGREEMENT_CODE;
                                CashModelCredit.MAS_FA_CODE = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == (model.IMS_PR_ROAD_CODE.HasValue ? model.IMS_PR_ROAD_CODE.Value : -1)).Select(v => v.IMS_COLLABORATION).FirstOrDefault();
                                CashModelCredit.FINAL_PAYMENT = model.FINAL_PAYMENT;

                                //Added By Abhishek Kamble 28-nov-2013
                                CashModelCredit.USERID = PMGSYSession.Current.UserId;
                                CashModelCredit.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                                // CashModelCredit.MAST_DISTRICT_CODE = model.MAST_DISTRICT_CODE;


                                //create Debit Entry for cash

                                ACC_BILL_DETAILS CashModelDebit = new ACC_BILL_DETAILS();

                                maxTxnNo = Convert.ToInt16(maxTxnNo + 1);

                                //get the debit head
                                CashModelDebit.HEAD_ID = (from item in dbContext.ACC_TXN_HEAD_MAPPING
                                                          where item.TXN_ID == TXN_ID_Model && item.CASH_CHQ.Contains(masterChequeCash) && item.CREDIT_DEBIT == "D"
                                                          select item.HEAD_ID).FirstOrDefault();

                                CashModelDebit.BILL_ID = Bill_id;
                                CashModelDebit.TXN_NO = maxTxnNo;
                                CashModelDebit.TXN_ID = TXN_ID_Model;
                                CashModelDebit.AMOUNT = model.AMOUNT_C.HasValue ? model.AMOUNT_C.Value : 0;
                                CashModelDebit.CREDIT_DEBIT = "D";
                                CashModelDebit.CASH_CHQ = "C";
                                CashModelDebit.NARRATION = model.NARRATION_P;
                                CashModelDebit.ADMIN_ND_CODE = model.ND_CODE;
                                CashModelDebit.MAST_CON_ID = model.CON_ID;

                                if (!(model.IMS_PR_ROAD_CODE == null || model.IMS_PR_ROAD_CODE == 0))
                                {
                                    CashModelDebit.IMS_PR_ROAD_CODE = model.IMS_PR_ROAD_CODE;
                                }

                                CashModelDebit.IMS_AGREEMENT_CODE = model.IMS_AGREEMENT_CODE;
                                CashModelDebit.MAS_FA_CODE = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == (model.IMS_PR_ROAD_CODE.HasValue ? model.IMS_PR_ROAD_CODE.Value : -1)).Select(v => v.IMS_COLLABORATION).FirstOrDefault();
                                CashModelDebit.FINAL_PAYMENT = model.FINAL_PAYMENT;

                                //Added By Abhishek Kamble 28-nov-2013
                                CashModelDebit.USERID = PMGSYSession.Current.UserId;
                                CashModelDebit.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                                // CashModelDebit.MAST_DISTRICT_CODE = model.MAST_DISTRICT_CODE;

                                //add the credit debit entries if add operation
                                if (AddorEdit.Equals("A"))
                                {
                                    dbContext.ACC_BILL_DETAILS.Add(CashModelCredit);
                                    dbContext.ACC_BILL_DETAILS.Add(CashModelDebit);
                                }
                                else
                                {

                                    ACC_BILL_DETAILS old_acc_bill_details = new ACC_BILL_DETAILS();
                                    old_acc_bill_details = dbContext.ACC_BILL_DETAILS.Where(x => x.BILL_ID == Bill_id && x.TXN_NO == CashModelCredit.TXN_NO).FirstOrDefault();
                                    dbContext.Entry(old_acc_bill_details).CurrentValues.SetValues(CashModelCredit);

                                    old_acc_bill_details = null;
                                    old_acc_bill_details = dbContext.ACC_BILL_DETAILS.Where(x => x.BILL_ID == Bill_id && x.TXN_NO == (CashModelDebit.TXN_NO)).FirstOrDefault();
                                    dbContext.Entry(old_acc_bill_details).CurrentValues.SetValues(CashModelDebit);

                                }
                            }
                            // }

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
                            if (dbContext.ACC_BILL_DETAILS.Where(t => t.BILL_ID == Bill_id).Any())
                            {
                                maxTxnNo = dbContext.ACC_BILL_DETAILS.Where(t => t.BILL_ID == Bill_id).Max(c => c.TXN_NO);
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
                        ACC_BILL_DETAILS DeductionModelCredit = new ACC_BILL_DETAILS();


                        Int16 TXN_ID_Model = Convert.ToInt16(model.HEAD_ID_D.ToString().Split('$')[0]);
                        // String CashChq = Convert.ToString(model.HEAD_ID_D.ToString().Split('$')[1]);

                        //get the debit head
                        DeductionModelCredit.HEAD_ID = (from item in dbContext.ACC_TXN_HEAD_MAPPING
                                                        where item.TXN_ID == TXN_ID_Model && item.CASH_CHQ.Contains("C") && item.CREDIT_DEBIT == "C"
                                                        select item.HEAD_ID).FirstOrDefault();

                        DeductionModelCredit.BILL_ID = Bill_id;
                        DeductionModelCredit.TXN_NO = maxTxnNo;
                        DeductionModelCredit.TXN_ID = TXN_ID_Model;
                        DeductionModelCredit.AMOUNT = model.AMOUNT_D.HasValue ? model.AMOUNT_D.Value : 0;
                        DeductionModelCredit.CREDIT_DEBIT = "C";
                        DeductionModelCredit.CASH_CHQ = "D";
                        DeductionModelCredit.NARRATION = model.NARRATION_D;
                        DeductionModelCredit.ADMIN_ND_CODE = model.ND_CODE;
                        DeductionModelCredit.MAST_CON_ID = model.CON_ID;
                        //DeductionModelCredit.IMS_PR_ROAD_CODE = model.IMS_PR_ROAD_CODE;
                        DeductionModelCredit.IMS_AGREEMENT_CODE = model.IMS_AGREEMENT_CODE_DED;
                        // DeductionModelCredit.MAS_FA_CODE = model.MAS_FA_CODE;
                        // DeductionModelCredit.FINAL_PAYMENT = model.FINAL_PAYMENT;
                        // DeductionModelCredit.MAST_DISTRICT_CODE = model.MAST_DISTRICT_CODE;


                        //Added By Abhishek Kamble 28-nov-2013
                        DeductionModelCredit.USERID = PMGSYSession.Current.UserId;
                        DeductionModelCredit.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];


                        //create debit entry for deduction
                        ACC_BILL_DETAILS DeductionModelDebit = new ACC_BILL_DETAILS();

                        maxTxnNo = Convert.ToInt16(maxTxnNo + 1);

                        //get the debit head
                        DeductionModelDebit.HEAD_ID = (from item in dbContext.ACC_TXN_HEAD_MAPPING
                                                       where item.TXN_ID == TXN_ID_Model && item.CASH_CHQ.Contains("C") && item.CREDIT_DEBIT == "D"
                                                       select item.HEAD_ID).FirstOrDefault();

                        DeductionModelDebit.BILL_ID = Bill_id;
                        DeductionModelDebit.TXN_NO = maxTxnNo;
                        DeductionModelDebit.TXN_ID = TXN_ID_Model;
                        DeductionModelDebit.AMOUNT = model.AMOUNT_D.HasValue ? model.AMOUNT_D.Value : 0;
                        DeductionModelDebit.CREDIT_DEBIT = "D";
                        DeductionModelDebit.CASH_CHQ = "D";
                        DeductionModelDebit.NARRATION = model.NARRATION_D;
                        DeductionModelDebit.ADMIN_ND_CODE = model.ND_CODE;
                        DeductionModelDebit.MAST_CON_ID = model.CON_ID;
                        //DeductionModelDebit.IMS_PR_ROAD_CODE = model.IMS_PR_ROAD_CODE;
                        DeductionModelDebit.IMS_AGREEMENT_CODE = model.IMS_AGREEMENT_CODE_DED;
                        // DeductionModelDebit.MAS_FA_CODE = model.MAS_FA_CODE;
                        // DeductionModelDebit.FINAL_PAYMENT = model.FINAL_PAYMENT;
                        // DeductionModelDebit.MAST_DISTRICT_CODE = model.MAST_DISTRICT_CODE;

                        //Added By Abhishek Kamble 28-nov-2013
                        DeductionModelDebit.USERID = PMGSYSession.Current.UserId;
                        DeductionModelDebit.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                        if (AddorEdit.Equals("A"))
                        {
                            //add the row
                            if (DeductionModelCredit.AMOUNT != 0)
                            {
                                dbContext.ACC_BILL_DETAILS.Add(DeductionModelCredit);
                                dbContext.ACC_BILL_DETAILS.Add(DeductionModelDebit);
                                dbContext.SaveChanges();
                                scope.Complete();
                                return true;
                            }
                            else
                            {
                                return true;
                            }

                        }
                        else
                        {

                            ACC_BILL_DETAILS old_acc_bill_details = new ACC_BILL_DETAILS();
                            old_acc_bill_details = dbContext.ACC_BILL_DETAILS.Where(x => x.BILL_ID == Bill_id && x.TXN_NO == DeductionModelCredit.TXN_NO).FirstOrDefault();
                            dbContext.Entry(old_acc_bill_details).CurrentValues.SetValues(DeductionModelCredit);

                            old_acc_bill_details = null;
                            old_acc_bill_details = dbContext.ACC_BILL_DETAILS.Where(x => x.BILL_ID == Bill_id && x.TXN_NO == (DeductionModelDebit.TXN_NO)).FirstOrDefault();
                            dbContext.Entry(old_acc_bill_details).CurrentValues.SetValues(DeductionModelDebit);


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
        /// function to check whether asset details for the payment is available 
        /// return 1 if yes, 0 if no
        /// </summary>
        /// <param name="bill_id"></param>
        /// <returns>1 when amount is correct as per validation
        ///  0 when amount is not valid
        /// </returns>
        public String CheckForAssetPaymentValidation(long bill_id, int headId, short transNumber, decimal newTransAmount)
        {
            try
            {
                decimal assetHeadAmount = 0;
                decimal paymentHeadDetails = 0;
                dbContext = new PMGSYEntities();
                //find total amount on the head
                if (dbContext.ACC_ASSET_DETAILS.Any(c => c.BILL_ID == bill_id && c.HEAD_ID == headId))
                {
                    assetHeadAmount = dbContext.ACC_ASSET_DETAILS.Where(c => c.BILL_ID == bill_id && c.HEAD_ID == headId).Select(x => x.TOTAL_AMOUNT).FirstOrDefault();

                    //get thesum of all the amount against the head from transaction table  
                    paymentHeadDetails = dbContext.ACC_BILL_DETAILS.Where(x => x.HEAD_ID == headId && x.CREDIT_DEBIT == "D" && x.CASH_CHQ == "Q" && x.TXN_NO != transNumber).Select(x => x.AMOUNT).Sum();

                    //get new payment amount with updated amount 
                    paymentHeadDetails = paymentHeadDetails + newTransAmount;

                    //if new payment amount for asset is less than asset amount already entered return status of validation
                    if (paymentHeadDetails < assetHeadAmount)
                    {
                        return "0";
                    }
                    else
                    {
                        return "1";
                    }

                }
                else
                {
                    return "1";
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


        /// <summary>
        /// function to delete the transaction payment details
        /// </summary>
        /// <param name="master_Bill_Id">Bill_Id of the transaction to be deleted </param>
        /// <param name="tranNumber">debit transaction number </param>
        /// <param name="paymentDeduction"> payment entry or deduction entry</param>
        /// <returns> 1 if succesfull ;-1 if already finalized</returns>
        public Int32 DeleteTransactionPaymentDetails(Int64 master_Bill_Id, Int32 tranNumber, String paymentDeduction)
        {
            try
            {

                dbContext = new PMGSYEntities();
                using (var scope = new TransactionScope())
                {
                    //get the master
                    ACC_BILL_MASTER con = dbContext.ACC_BILL_MASTER.SingleOrDefault(p => p.BILL_ID == master_Bill_Id);

                    Int16? TXN_ID = dbContext.ACC_BILL_DETAILS.Where(p => p.BILL_ID == master_Bill_Id && p.TXN_NO == tranNumber).Select(x => x.TXN_ID).FirstOrDefault();

                    //cheque if finalized
                    if (con.BILL_FINALIZED == "Y")
                    {
                        return -1; //return status error
                    }

                    //for payment entry 
                    if (paymentDeduction == "P")
                    {
                        //check if asset details has been entered for the voucher
                        if (dbContext.ACC_ASSET_DETAILS.Any(c => c.BILL_ID == master_Bill_Id))
                        {
                            return -2;
                        }

                    }


                    if (paymentDeduction.Equals("D"))
                    {
                        //get the transaction details
                        ACC_BILL_DETAILS creditRow = dbContext.ACC_BILL_DETAILS.SingleOrDefault(p => p.BILL_ID == master_Bill_Id && p.TXN_NO == (tranNumber - 1) && p.TXN_ID == TXN_ID);
                        ACC_BILL_DETAILS debitRow = dbContext.ACC_BILL_DETAILS.SingleOrDefault(p => p.BILL_ID == master_Bill_Id && p.TXN_NO == tranNumber && p.TXN_ID == TXN_ID);

                        //delete the details table entry
                        if (creditRow != null)
                        {
                            //Added By Abhishek Kamble 28-Nov-2013
                            creditRow.USERID = PMGSYSession.Current.UserId;
                            creditRow.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            dbContext.Entry(creditRow).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();

                            dbContext.ACC_BILL_DETAILS.Remove(creditRow);
                        }
                        if (debitRow != null)
                        {
                            //Added By Abhishek Kamble 28-Nov-2013
                            debitRow.USERID = PMGSYSession.Current.UserId;
                            debitRow.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            dbContext.Entry(debitRow).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();

                            dbContext.ACC_BILL_DETAILS.Remove(debitRow);
                        }
                        dbContext.SaveChanges();
                        scope.Complete();

                        return 1;
                    }
                    else
                    {

                        //get the transaction details with CHQ as Q
                        ACC_BILL_DETAILS creditRowChequePayment = dbContext.ACC_BILL_DETAILS.SingleOrDefault(p => p.BILL_ID == master_Bill_Id && p.TXN_NO == (tranNumber - 1) && p.TXN_ID == TXN_ID);
                        ACC_BILL_DETAILS debitRowChequePayment = dbContext.ACC_BILL_DETAILS.SingleOrDefault(p => p.BILL_ID == master_Bill_Id && p.TXN_NO == tranNumber && p.TXN_ID == TXN_ID);

                        ACC_BILL_DETAILS creditRowCashPayment = new ACC_BILL_DETAILS();
                        ACC_BILL_DETAILS debitRowCashPayment = new ACC_BILL_DETAILS();

                        //get the associated  cash transaction details  only if its cheque with deduction
                        //  This If condition is removed by abhishek kamble to delete Deduction only cash details And Contractor work payment 0 amount detais 20-June-2014
                        //if (con.CASH_AMOUNT != 0 && con.CHQ_AMOUNT != 0)
                        //{
                        creditRowCashPayment = dbContext.ACC_BILL_DETAILS.SingleOrDefault(p => p.BILL_ID == master_Bill_Id && p.TXN_NO == (tranNumber + 1) && p.TXN_ID == TXN_ID);
                        debitRowCashPayment = dbContext.ACC_BILL_DETAILS.SingleOrDefault(p => p.BILL_ID == master_Bill_Id && p.TXN_NO == (tranNumber + 2) && p.TXN_ID == TXN_ID);
                        // }
                        //delete the details table entry
                        if (creditRowChequePayment != null)
                        {
                            //Added By Abhishek Kamble 28-Nov-2013
                            creditRowChequePayment.USERID = PMGSYSession.Current.UserId;
                            creditRowChequePayment.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            dbContext.Entry(creditRowChequePayment).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();

                            dbContext.ACC_BILL_DETAILS.Remove(creditRowChequePayment);
                        }
                        if (debitRowChequePayment != null)
                        {
                            //Added By Abhishek Kamble 28-Nov-2013
                            debitRowChequePayment.USERID = PMGSYSession.Current.UserId;
                            debitRowChequePayment.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            dbContext.Entry(debitRowChequePayment).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();

                            dbContext.ACC_BILL_DETAILS.Remove(debitRowChequePayment);
                        }

                        //delete only if cheque trabsaction with deduction
                        //  This If condition is removed by abhishek kamble to delete Deduction only cash details And Contractor work payment 0 amount detais 20-June-2014
                        //if (con.CASH_AMOUNT != 0 && con.CHQ_AMOUNT != 0)
                        //{
                        if (creditRowCashPayment != null)
                        {
                            //Added By Abhishek Kamble 28-Nov-2013
                            creditRowCashPayment.USERID = PMGSYSession.Current.UserId;
                            creditRowCashPayment.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            dbContext.Entry(creditRowCashPayment).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();

                            dbContext.ACC_BILL_DETAILS.Remove(creditRowCashPayment);
                        }
                        if (debitRowCashPayment != null)
                        {
                            //Added By Abhishek Kamble 28-Nov-2013
                            debitRowCashPayment.USERID = PMGSYSession.Current.UserId;
                            debitRowCashPayment.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            dbContext.Entry(debitRowCashPayment).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();

                            dbContext.ACC_BILL_DETAILS.Remove(debitRowCashPayment);
                        }

                        // }

                        //new change done by Vikram on 15 Jan 2014
                        //if  the master entry is not correct and all the details entries are deleted or corrected with the correct sub transactions then updating the master transaction status of ACTION_REQUIRED as N
                        if (con.ACTION_REQUIRED != "N")
                        {
                            var status = (from bd in dbContext.ACC_BILL_DETAILS
                                          where bd.BILL_ID == con.BILL_ID
                                          select new
                                          {
                                              HEAD_ID = bd.HEAD_ID,
                                              TXN_ID = bd.TXN_ID,
                                          }).Distinct();

                            bool isValid = true;
                            foreach (var item in status)
                            {
                                if (dbContext.ACC_TXN_HEAD_MAPPING.Where(m => m.TXN_ID == item.TXN_ID && m.HEAD_ID == item.HEAD_ID).Select(m => m.IS_REQ_AFTER_PORTING).FirstOrDefault() == false)
                                {
                                    isValid = false;
                                    break;
                                }
                            }

                            if (isValid)
                            {
                                con.ACTION_REQUIRED = "N";
                                con.USERID = PMGSYSession.Current.UserId;
                                con.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                                dbContext.Entry(con).State = System.Data.Entity.EntityState.Modified;
                                dbContext.SaveChanges();
                            }
                        }
                        //end of change

                        dbContext.SaveChanges();
                        scope.Complete();

                        return 1;

                    }
                }

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error while deleting payment details..");

            }
            finally
            {
                dbContext.Dispose();
            }

        }


        /// <summary>
        /// function to get the transaction details
        /// </summary>
        /// <param name="BILL_ID"></param>
        /// <param name="tranNumber"></param>
        /// <param name="paymentDeduction"></param>
        /// <returns> details of the transaction</returns>
        public ACC_BILL_DETAILS GetTransactionPaymentDetails(Int64 BILL_ID, Int16 tranNumber, String paymentDeduction)
        {
            try
            {
                dbContext = new PMGSYEntities();

                return (from row in dbContext.ACC_BILL_DETAILS
                        where row.BILL_ID == BILL_ID && row.TXN_NO == tranNumber
                        select row).FirstOrDefault();

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error while getting transaction details");
            }
            finally
            {
                dbContext.Dispose();
            }

        }

        /// <summary>
        /// function to get the agreement number based on the voucher
        /// </summary>
        /// <param name="bill_id"></param>
        /// <returns></returns>
        public String GetAgreemntNumberForVoucher(Int64 bill_id)
        {
            try
            {
                dbContext = new PMGSYEntities();

                return (dbContext.ACC_BILL_DETAILS.Where(x => x.BILL_ID == bill_id && x.CREDIT_DEBIT == "D").Select(y => y.IMS_AGREEMENT_CODE).FirstOrDefault().ToString());

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error while getting agreement details");
            }
            finally
            {
                dbContext.Dispose();
            }


        }

        /// <summary>
        /// funtion to finalize voucher details
        /// </summary>
        /// <param name="bill_id"></param>
        /// <param name="Tofinalize"></param>
        /// <returns>1 for success
        /// -1 for invalid entry to finalize</returns>
        public Int32 FinalizeVoucher(Int64 bill_id, bool Tofinalize)
        {

            try
            {
                dbContext = new PMGSYEntities();

                string fundType = PMGSYSession.Current.FundType;
                int adminNdCode = PMGSYSession.Current.AdminNdCode;
                short levelID = PMGSYSession.Current.LevelId;

                CommonFunctions objCommon = new CommonFunctions();

                using (var scope = new TransactionScope())
                {

                    if (CanVoucherFinalized(bill_id))
                    {
                        //if all diffrence amount is 0 then finalize voucher


                        // to check if correct entry as is operational and is required after porting flag
                        string correctionStatus = objCommon.ValidateHeadForCorrection(0, bill_id, "F");
                        if (correctionStatus != "1")
                        {
                            return -2;
                        }

                        ACC_BILL_MASTER acc_bill_master = dbContext.ACC_BILL_MASTER.Find(bill_id);

                        //commented by koustubh Nakate on 27/09/2013 
                        //if (acc_bill_master.CHQ_EPAY == "E")
                        //{
                        //    acc_bill_master.BILL_FINALIZED = "E";
                        //}
                        //else
                        //{
                        //    acc_bill_master.BILL_FINALIZED = "Y";
                        //}

                        //end commented by koustubh Nakate on 27/09/2013 

                        //changes by koustubh Nakate on 27/09/2013 
                        acc_bill_master.BILL_FINALIZED = "Y";

                        acc_bill_master.ACTION_REQUIRED = "N";

                        //Added By Abhishek Kamble 28-Nov-2013
                        acc_bill_master.USERID = PMGSYSession.Current.UserId;
                        acc_bill_master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                        dbContext.Entry(acc_bill_master).State = System.Data.Entity.EntityState.Modified;

                        dbContext.SaveChanges();

                        //added by Koustubh Nakate on 22/08/2013 to save notification in notification details table 
                        //var result = dbContext.USP_ACC_INSERT_ALERT_DETAILS(adminNdCode, fundType, "P", levelID, bill_id, PMGSYSession.Current.UserId, HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]);


                    }
                    else
                    {

                        return -1;
                    }

                    scope.Complete();
                    return 1;

                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error while finalizing payment details");
            }
            finally
            {
                dbContext.Dispose();
            }

        }

        /// <summary>
        /// function to get is finalized payment details
        /// </summary>
        /// <param name="BILL_ID"></param>
        /// <param name="roadID"></param>
        /// <returns></returns>
        public List<SelectListItem> GetFinalPaymentDetails(Int64 BILL_ID, Int32 roadID, Int32 subTxnID)
        {
            try
            {
                dbContext = new PMGSYEntities();
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 180;

                List<SelectListItem> options = new List<SelectListItem>();
                /* 
                 //get the Contractor details  from bill master
                 Int32? contractorId =  dbContext.ACC_BILL_MASTER.Where(x=>x.BILL_ID == BILL_ID).Select(c=>c.MAST_CON_ID).First();

                 //get the agreement details from bill details
                 Int32? agreementId =
                                   (from master in dbContext.ACC_BILL_MASTER
                                   join details in dbContext.ACC_BILL_DETAILS
                                   on master.BILL_ID equals details.BILL_ID
                                   where master.BILL_ID == BILL_ID
                                   && master.MAST_CON_ID == contractorId
                                   select details.IMS_AGREEMENT_CODE).FirstOrDefault();
                   */

                //for road check final payment is given


                //var lsbRoad = (
                //                  from sp in dbContext.IMS_SANCTIONED_PROJECTS
                //                  where sp.IMS_PR_ROAD_CODE == roadID
                //                  select new
                //                  {
                //                      sp.IMS_PROPOSAL_TYPE
                //                  });
                string proposalType = (dbContext.IMS_SANCTIONED_PROJECTS.Where(c => c.IMS_PR_ROAD_CODE == roadID).Select(c => c.IMS_PROPOSAL_TYPE).First());

                bool physicalCompletion = false;

                if (proposalType.Equals("P"))
                {
                    if (dbContext.EXEC_ROADS_MONTHLY_STATUS.Where(c => c.IMS_PR_ROAD_CODE == roadID && c.EXEC_ISCOMPLETED == "C").Any())
                    {
                        physicalCompletion = true;
                    }
                }
                else if (proposalType.Equals("L"))
                {
                    if (dbContext.EXEC_LSB_MONTHLY_STATUS.Where(c => c.IMS_PR_ROAD_CODE == roadID && c.EXEC_ISCOMPLETED == "C").Any())
                    {
                        physicalCompletion = true;
                    }
                }
                else
                {
                    physicalCompletion = true;
                }

                //Added By Abhishek kamble 12Dec2014 start to show Yes/No option for Head Id-409 Code- 11.07	Special Works-Reparation of completed PMGSY roads damaged by extraordinary calamities etc
                //If else condition is added
                if (subTxnID != 1543)
                {
                    var query = (from master in dbContext.ACC_BILL_MASTER
                                 join details in dbContext.ACC_BILL_DETAILS
                                     on master.BILL_ID equals details.BILL_ID
                                 where
                                     // master.MAST_CON_ID == contractorId
                                     // && details.IMS_AGREEMENT_CODE == agreementId  &&
                                 details.IMS_PR_ROAD_CODE == roadID &&
                                 master.BILL_DATE <= (dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == BILL_ID).Select(m => m.BILL_DATE).FirstOrDefault()) //new change done by Vikram 
                                     // && subTxnID != 1543       //Added By Abhishek kamble 12Dec2014
                                  && details.TXN_ID != 1543       //Added By Abhishek kamble 27May2015

                                 select new
                                 {
                                     details.FINAL_PAYMENT

                                 });


                    //Below condition commented on 18-03-2023 to add validation.If physicalCompletion of road is done then only show Is final payment dropdown with "Yes" option
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

                    //Below condition Added on 18-03-2023 to add validation.If physicalCompletion of road is done then only show Is final payment dropdown with "Yes" option

                    if (physicalCompletion)
                    {
                        foreach (var item in query)
                        {
                            if (item.FINAL_PAYMENT.HasValue)
                            {
                                if (item.FINAL_PAYMENT.Value)
                                {

                                    options.Add(new SelectListItem { Selected = true, Text = "Yes", Value = "true" });
                                    //options.Add(new SelectListItem { Selected = false, Text = "No", Value = "false" });//Added on 21-08-2023

                                    break;

                                }

                            }
                        }
                    }


                    //if no final payment is found , Then Show only 'No' options
                    if (options.Count == 0)
                    {
                        if (physicalCompletion)
                        {
                            options.Add(new SelectListItem { Text = "Yes", Value = "true" });
                        }
                        options.Add(new SelectListItem { Selected = true, Text = "No", Value = "false" });

                    }
                }
                else
                {

                    var query = (from master in dbContext.ACC_BILL_MASTER
                                 join details in dbContext.ACC_BILL_DETAILS
                                     on master.BILL_ID equals details.BILL_ID
                                 where
                                     // master.MAST_CON_ID == contractorId
                                     // && details.IMS_AGREEMENT_CODE == agreementId  &&
                                 details.IMS_PR_ROAD_CODE == roadID &&
                                 master.BILL_DATE <= (dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == BILL_ID).Select(m => m.BILL_DATE).FirstOrDefault()) //new change done by Vikram 
                                 && details.TXN_ID == 1543
                                 select new
                                 {
                                     details.FINAL_PAYMENT
                                 });






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

                    //Below condition Added on 18-03-2023 to add validation.If physicalCompletion of road is done then only show Is final payment dropdown with "Yes" option

                    if (physicalCompletion)
                    {
                        foreach (var item in query)
                        {
                            if (item.FINAL_PAYMENT.HasValue)
                            {
                                if (item.FINAL_PAYMENT.Value)
                                {
                                    options.Add(new SelectListItem { Selected = true, Text = "Yes", Value = "true" });
                                    //options.Add(new SelectListItem { Selected = false, Text = "No", Value = "false" });//Added on 21-08-2023

                                    break;
                                }

                            }
                        }
                    }

                    //if no final payment is found 
                    if (options.Count == 0)
                    {
                        if (physicalCompletion)
                        {

                            options.Add(new SelectListItem { Text = "Yes", Value = "true" });
                        }
                        options.Add(new SelectListItem { Selected = true, Text = "No", Value = "false" });
                    }

                }

                return options;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error while getting is final payment details");
            }
            finally
            {
                dbContext.Dispose();
            }

        }

        #region Validation for payment of roads within 6 months of completion
        public bool CompletionDateValidation(int prRoadCode, DateTime billDate, ref string[] param)
        {
            param = new string[2];
            try
            {
                dbContext = new PMGSYEntities();

                string proposalType = (dbContext.IMS_SANCTIONED_PROJECTS.Where(c => c.IMS_PR_ROAD_CODE == prRoadCode).Select(c => c.IMS_PROPOSAL_TYPE).First());
                param[0] = proposalType == "P" ? "Road" : proposalType == "L" ? "Bridge" : "Building";

                bool physicalCompletion = false;

                if (proposalType.Equals("P"))
                {
                    if (dbContext.EXEC_ROADS_MONTHLY_STATUS.Any(c => c.IMS_PR_ROAD_CODE == prRoadCode && c.EXEC_ISCOMPLETED == "C"))
                    {
                        param[1] = new CommonFunctions().GetDateTimeToString(dbContext.EXEC_ROADS_MONTHLY_STATUS.Where(c => c.IMS_PR_ROAD_CODE == prRoadCode && c.EXEC_ISCOMPLETED == "C").Select(x => x.EXEC_COMPLETION_DATE.Value).FirstOrDefault());
                        if (dbContext.EXEC_ROADS_MONTHLY_STATUS.Where(c => c.IMS_PR_ROAD_CODE == prRoadCode && c.EXEC_ISCOMPLETED == "C" && (SqlFunctions.DateDiff("day", c.EXEC_COMPLETION_DATE, billDate) <= 180)).Any())
                        {
                            physicalCompletion = true;//If road is completed and difference is less than 180 days
                        }
                    }
                    else
                    {
                        physicalCompletion = true;//If road is In-Progress
                    }
                }
                else if (proposalType.Equals("L"))
                {
                    if (dbContext.EXEC_LSB_MONTHLY_STATUS.Any(c => c.IMS_PR_ROAD_CODE == prRoadCode && c.EXEC_ISCOMPLETED == "C"))
                    {
                        param[1] = new CommonFunctions().GetDateTimeToString(dbContext.EXEC_LSB_MONTHLY_STATUS.Where(c => c.IMS_PR_ROAD_CODE == prRoadCode && c.EXEC_ISCOMPLETED == "C").Select(x => x.EXEC_COMPLETION_DATE.Value).FirstOrDefault());
                        if (dbContext.EXEC_LSB_MONTHLY_STATUS.Where(c => c.IMS_PR_ROAD_CODE == prRoadCode && c.EXEC_ISCOMPLETED == "C" && (SqlFunctions.DateDiff("day", c.EXEC_COMPLETION_DATE, billDate) <= 180)).Any())
                        {
                            physicalCompletion = true;//If bridge is completed and difference is less than 180 days
                        }
                    }
                    else
                    {
                        physicalCompletion = true;//If bridge is In-Progress
                    }
                }
                else
                {
                    physicalCompletion = true;
                }

                return physicalCompletion;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PaymentDAL.CompletionDateValidation()");
                return false;
            }
        }

        public bool CompletionDateValidation1(int prRoadCode, DateTime billDate, ref string[] param)
        {
            param = new string[2];
            bool physicalCompletion = false;

            int adminNdCode = 0, parentNdCode = 0;
            string ndType = string.Empty;
            string fromDate = string.Empty, toDate = string.Empty;


            DateTime currDt = DateTime.Now.Date;
            try
            {
                dbContext = new PMGSYEntities();

                //string proposalType = (dbContext.IMS_SANCTIONED_PROJECTS.Where(c => c.IMS_PR_ROAD_CODE == prRoadCode).Select(c => c.IMS_PROPOSAL_TYPE).First());
                var proposal = dbContext.IMS_SANCTIONED_PROJECTS.Where(c => c.IMS_PR_ROAD_CODE == prRoadCode).FirstOrDefault();
                param[0] = proposal.IMS_PROPOSAL_TYPE == "P" ? "Road" : proposal.IMS_PROPOSAL_TYPE == "L" ? "Bridge" : "Building";

                adminNdCode = proposal.MAST_DPIU_CODE;
                ndType = proposal.ADMIN_DEPARTMENT.MAST_ND_TYPE.Trim();
                parentNdCode = proposal.ADMIN_DEPARTMENT.MAST_PARENT_ND_CODE.Value;
                #region Old Logic
                /*var query = dbContext.PAYMENT_EXEC_VALIDATION_CONFIGURATION.Where(x => x.ADMIN_ND_CODE == adminNdCode && x.SRRDA_PIU == ndType && (x.IMS_PR_ROAD_CODE == null || x.IMS_PR_ROAD_CODE == prRoadCode)
                                && ((x.FROM_DATE <= EntityFunctions.TruncateTime(currDt) && x.TO_DATE >= EntityFunctions.TruncateTime(currDt)))).FirstOrDefault();

                //if (query == null)
                //{
                //    query = dbContext.PAYMENT_EXEC_VALIDATION_CONFIGURATION.Where(x => x.ADMIN_ND_CODE == adminNdCode && x.SRRDA_PIU == ndType && x.IMS_PR_ROAD_CODE == null
                //                && ((x.FROM_DATE <= EntityFunctions.TruncateTime(currDt) && x.TO_DATE >= EntityFunctions.TruncateTime(currDt)))).FirstOrDefault();
                //}

                if (query == null)
                {
                    query = dbContext.PAYMENT_EXEC_VALIDATION_CONFIGURATION.Where(x => x.ADMIN_ND_CODE == parentNdCode && x.SRRDA_PIU == "S" && (x.IMS_PR_ROAD_CODE == null || x.IMS_PR_ROAD_CODE == prRoadCode)
                                && ((x.FROM_DATE <= EntityFunctions.TruncateTime(currDt) && x.TO_DATE >= EntityFunctions.TruncateTime(currDt) ))).FirstOrDefault();
                }*/
                #endregion
                /*var query = dbContext.PAYMENT_EXEC_VALIDATION_CONFIGURATION.Where(x => x.ADMIN_DEPARTMENT.MAST_STATE_CODE == PMGSYSession.Current.StateCode && x.ADMIN_DEPARTMENT.MAST_AGENCY_CODE == PMGSYSession.Current.MastAgencyCode 
                                && x.SRRDA_PIU == ndType && (x.IMS_PR_ROAD_CODE == null || x.IMS_PR_ROAD_CODE == prRoadCode)
                                && ((x.FROM_DATE <= EntityFunctions.TruncateTime(currDt) && x.TO_DATE >= EntityFunctions.TruncateTime(currDt)))).FirstOrDefault();*/
                var query = dbContext.PAYMENT_EXEC_VALIDATION_CONFIGURATION.Where(x => x.ADMIN_DEPARTMENT.MAST_AGENCY_CODE == PMGSYSession.Current.MastAgencyCode
                                && ((x.ADMIN_DEPARTMENT.ADMIN_ND_CODE == PMGSYSession.Current.ParentNDCode && x.SRRDA_PIU == "S") || (x.ADMIN_DEPARTMENT.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && x.SRRDA_PIU == "D"))
                                && (x.IMS_PR_ROAD_CODE == null || x.IMS_PR_ROAD_CODE == prRoadCode)
                                && ((x.FROM_DATE <= EntityFunctions.TruncateTime(currDt) && x.TO_DATE >= EntityFunctions.TruncateTime(currDt)))).FirstOrDefault();
                if (query != null)
                {
                    physicalCompletion = true;
                }
                else
                {
                    if (proposal.IMS_PROPOSAL_TYPE.Equals("P"))
                    {
                        if (dbContext.EXEC_ROADS_MONTHLY_STATUS.Any(c => c.IMS_PR_ROAD_CODE == prRoadCode && c.EXEC_ISCOMPLETED == "C"))
                        {
                            param[1] = new CommonFunctions().GetDateTimeToString(dbContext.EXEC_ROADS_MONTHLY_STATUS.Where(c => c.IMS_PR_ROAD_CODE == prRoadCode && c.EXEC_ISCOMPLETED == "C").Select(x => x.EXEC_COMPLETION_DATE.Value).FirstOrDefault());
                            if (dbContext.EXEC_ROADS_MONTHLY_STATUS.Where(c => c.IMS_PR_ROAD_CODE == prRoadCode && c.EXEC_ISCOMPLETED == "C" && (SqlFunctions.DateDiff("day", c.EXEC_COMPLETION_DATE, billDate) <= 180)).Any())
                            {
                                physicalCompletion = true;//If road is completed and difference is less than 180 days
                            }
                            
                        }
                        else
                        {
                            physicalCompletion = true;//If road is In-Progress
                        }
                    }
                    else if (proposal.IMS_PROPOSAL_TYPE.Equals("L"))
                    {
                        if (dbContext.EXEC_LSB_MONTHLY_STATUS.Any(c => c.IMS_PR_ROAD_CODE == prRoadCode && c.EXEC_ISCOMPLETED == "C"))
                        {
                            param[1] = new CommonFunctions().GetDateTimeToString(dbContext.EXEC_LSB_MONTHLY_STATUS.Where(c => c.IMS_PR_ROAD_CODE == prRoadCode && c.EXEC_ISCOMPLETED == "C").Select(x => x.EXEC_COMPLETION_DATE.Value).FirstOrDefault());
                            if (dbContext.EXEC_LSB_MONTHLY_STATUS.Where(c => c.IMS_PR_ROAD_CODE == prRoadCode && c.EXEC_ISCOMPLETED == "C" && (SqlFunctions.DateDiff("day", c.EXEC_COMPLETION_DATE, billDate) <= 180)).Any())
                            {
                                physicalCompletion = true;//If bridge is completed and difference is less than 180 days
                            }
                            
                        }
                        else
                        {
                            physicalCompletion = true;//If bridge is In-Progress
                        }
                    }
                    else
                    {
                        physicalCompletion = true;
                    }
                }
                return physicalCompletion;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PaymentDAL.CompletionDateValidation()");
                return false;
            }
        }
        #endregion

        /// <summary>
        /// function to get the epay number
        /// </summary>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <param name="stateCode"></param>
        /// <returns></returns>
        public String GetEpayNumber(Int16 month, Int16 year, Int16 stateCode, int adminNdCode)
        {
            CommonFunctions objCommon = new CommonFunctions();

            try
            {
                //String financialYear = objCommon.getFinancialYear(month, year);
                dbContext = new PMGSYEntities();
                String EpayNumber = String.Empty;
                //String EpayNumber = "Epay/" + dbContext.MASTER_STATE.Where(x => x.MAST_STATE_CODE == stateCode).Select(y => y.MAST_STATE_SHORT_CODE).First().ToString()
                //   + "/" + dbContext.ADMIN_DEPARTMENT.Where(x => x.ADMIN_ND_CODE == adminNdCode).Select(y => y.ADMIN_EPAY_DPIU_CODE).First().ToString() + "/" +
                //     financialYear + "/" + (month.ToString().Length == 1 ? month.ToString().PadLeft(2, '0') : month.ToString()) + "/";

                //int maxEpayVoucherCount = 0;

                //if (dbContext.ACC_BILL_MASTER.Where(t => t.ADMIN_ND_CODE == adminNdCode).Any())
                //{
                //    maxEpayVoucherCount = dbContext.ACC_BILL_MASTER.Where(t => t.BILL_MONTH == month && t.BILL_YEAR == year && t.CHQ_EPAY.Trim().Equals("E") && t.ADMIN_ND_CODE == adminNdCode).Count();

                //    maxEpayVoucherCount = maxEpayVoucherCount + 1;

                //}
                //else
                //{
                //    maxEpayVoucherCount = maxEpayVoucherCount + 1;
                //}


                //EpayNumber = EpayNumber + maxEpayVoucherCount.ToString().PadLeft(3, '0');


                EpayNumber = dbContext.Database.SqlQuery<String>("EXEC [omms].[USP_ACC_GENERATE_EPAYMENT_NUMBER] @PrmFundype,@PrmAdminNDCode,@PrmMonth,@PrmYear,@PrmEpayRem",
                    new SqlParameter("@PrmFundype", PMGSYSession.Current.FundType),
                    new SqlParameter("@PrmAdminNDCode", adminNdCode),
                    new SqlParameter("@PrmMonth", month),
                    new SqlParameter("@PrmYear", year),
                    new SqlParameter("@PrmEpayRem", 'E')
                ).FirstOrDefault();



                return EpayNumber;


            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error while getting is Epayment Number");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// function to get the eremittance number
        /// </summary>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <param name="stateCode"></param>
        /// <param name="adminNdCode"></param>
        /// <returns></returns>
        public String GetEremittanceNumber(Int16 month, Int16 year, Int16 stateCode, int adminNdCode)
        {
            CommonFunctions objCommon = new CommonFunctions();
            String EremNumber = String.Empty;

            try
            {
                //String financialYear = objCommon.getFinancialYear(month, year);

                dbContext = new PMGSYEntities();

                //String EpayNumber = "eRemit/" + dbContext.MASTER_STATE.Where(x => x.MAST_STATE_CODE == stateCode).Select(y => y.MAST_STATE_SHORT_CODE).First().ToString()
                //   + "/" + dbContext.ADMIN_DEPARTMENT.Where(x => x.ADMIN_ND_CODE == adminNdCode).Select(y => y.ADMIN_EPAY_DPIU_CODE).First().ToString() + "/" +
                //   financialYear + "/" + (month.ToString().Length == 1 ? month.ToString().PadLeft(2, '0') : month.ToString()) + "/";

                //int maxEpayVoucherCount = 0;

                //if (dbContext.ACC_BILL_MASTER.Where(t => t.ADMIN_ND_CODE == adminNdCode).Any())
                //{
                //    //get the maximum voucher count with epayment and remittance
                //    maxEpayVoucherCount = dbContext.ACC_BILL_MASTER.Where(t => t.BILL_MONTH == month && t.BILL_YEAR == year && t.CHQ_EPAY.Trim().Equals("E") && t.ADMIN_ND_CODE == adminNdCode && t.REMIT_TYPE != null).Count();

                //    maxEpayVoucherCount = maxEpayVoucherCount + 1;

                //}
                //else
                //{
                //    maxEpayVoucherCount = maxEpayVoucherCount + 1;
                //}
                //EpayNumber = EpayNumber + maxEpayVoucherCount.ToString().PadLeft(3, '0');

                EremNumber = dbContext.Database.SqlQuery<String>("EXEC [omms].[USP_ACC_GENERATE_EPAYMENT_NUMBER] @PrmFundype,@PrmAdminNDCode,@PrmMonth,@PrmYear,@PrmEpayRem",
                  new SqlParameter("@PrmFundype", PMGSYSession.Current.FundType),
                  new SqlParameter("@PrmAdminNDCode", adminNdCode),
                  new SqlParameter("@PrmMonth", month),
                  new SqlParameter("@PrmYear", year),
                  new SqlParameter("@PrmEpayRem", 'M')
              ).FirstOrDefault();
                return EremNumber;


            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error while getting is Epayment Number");
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        #endregion


        #region cheque renewal

        /// <summary>
        /// function to renew the cheque details
        /// </summary>
        /// <param name="bill_Id"></param>
        /// <returns>
        /// -111 if cheque is already encashed
        /// -222 is cheque is encashed by bank
        /// </returns>
        public String RenewCheque(Int64 bill_Id, ChequeRenewModel model)
        {
            CommonFunctions objCommon = new CommonFunctions();
            dbContext = new PMGSYEntities();

            try
            {
                using (var scope = new TransactionScope())
                {

                    //get the bill details from the Master table             
                    ACC_BILL_MASTER masterDetails = new ACC_BILL_MASTER();

                    masterDetails = dbContext.ACC_BILL_MASTER.Where(c => c.BILL_ID == bill_Id).First();

                    //get the cheque details from [ACC_CHEQUES_ISSUED]

                    ACC_CHEQUES_ISSUED chequeIssued = null;

                    //At SRRDA level if check is issued  ACC_CHEQUES_ISSUED table may have no reference to it 
                    if (PMGSYSession.Current.LevelId == 4)
                    {

                        if (dbContext.ACC_CHEQUES_ISSUED.Where(c => c.BILL_ID == bill_Id).Any())
                        {
                            chequeIssued = new ACC_CHEQUES_ISSUED();
                            chequeIssued = dbContext.ACC_CHEQUES_ISSUED.Where(c => c.BILL_ID == bill_Id).First();
                        }

                    }
                    else if (PMGSYSession.Current.LevelId == 5)
                    {
                        chequeIssued = new ACC_CHEQUES_ISSUED();
                        chequeIssued = dbContext.ACC_CHEQUES_ISSUED.Where(c => c.BILL_ID == bill_Id).First();
                    }

                    if (chequeIssued != null)
                    {
                        //check whether cheque is encashed
                        /*if (chequeIssued.IS_CHQ_ENCASHED_NA)
                        {
                            // return "-111";

                        }
                        else */
                        if (chequeIssued.IS_CHQ_RECONCILE_BANK)
                        {
                            //check whether cheque is encashed by bank
                            return "-222";

                        }
                        else if (!chequeIssued.CHEQUE_STATUS.Equals("N"))
                        {
                            //cheque is not new 
                            return "-333";
                        }
                        else if (masterDetails.CHQ_DATE > objCommon.GetStringToDateTime(model.CHQ_DATE))
                        {
                            // cheque date is greater than cheque renewal date
                            return "-444";
                        }
                        else if (masterDetails.BILL_DATE > objCommon.GetStringToDateTime(model.BILL_DATE))
                        {
                            // bill date is greater than  cheque renewal bill  date
                            return "-555";
                        }
                        //if payment is of imprest and it has been settled it cant be cancelled
                        else if (dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Where(x => x.P_BILL_ID == bill_Id).Select(x => x.S_BIll_ID).Any())
                        {
                            return "-777";
                        }
                        else if (dbContext.ACC_ASSET_DETAILS.Any(x => x.BILL_ID == bill_Id))
                        {
                            return "-888"; //if asset details has been entered against the payment dont allow to cancell (date 06082013)
                        }
                        else if (masterDetails.CHQ_DATE <= objCommon.GetStringToDateTime(model.CHQ_DATE))
                        {
                            int validationMonths = Convert.ToInt32(ConfigurationManager.AppSettings["ChequeCancelRenewMonths"]);
                            if (Convert.ToDateTime(model.CHQ_DATE) > Convert.ToDateTime(masterDetails.CHQ_DATE).AddMonths(validationMonths))
                            {
                                return "-999";
                            }
                        }
                    }
                    else
                    { //Else condition Added By Abhishek kamble for Date Validation at SRRDA level 23-July-2014 start

                        if (masterDetails.CHQ_DATE > objCommon.GetStringToDateTime(model.CHQ_DATE))
                        {
                            // cheque date is greater than cheque renewal date
                            return "-444";
                        }
                        else if (masterDetails.BILL_DATE > objCommon.GetStringToDateTime(model.BILL_DATE))
                        {
                            // bill date is greater than  cheque renewal bill  date
                            return "-555";
                        }
                    }    //Else condition Added By Abhishek kamble for Date Validation at SRRDA level 23-July-2014 end
                    #region renewal entry in bill master

                    //new entry     
                    ACC_BILL_MASTER ModelToAdd = new ACC_BILL_MASTER();

                    Int64 maxBillId = 0;

                    if (dbContext.ACC_BILL_MASTER.Any())
                    {
                        maxBillId = dbContext.ACC_BILL_MASTER.Max(c => c.BILL_ID);
                    }

                    maxBillId = maxBillId + 1;

                    ModelToAdd.BILL_ID = maxBillId;
                    ModelToAdd.BILL_NO = model.BILL_NO;
                    //ModelToAdd.BILL_NO = model.BILL_NO + "-" + "R"; // new change done by Vikram on 24 Jan 2014 as per mam
                    ModelToAdd.BILL_MONTH = Convert.ToInt16(model.BILL_DATE.Split('/')[1]); //find out in controller
                    ModelToAdd.BILL_YEAR = Convert.ToInt16(model.BILL_DATE.Split('/')[2]); //find out in controller
                    ModelToAdd.BILL_DATE = objCommon.GetStringToDateTime(model.BILL_DATE);
                    //ModelToAdd.TXN_ID = 228;
                    ModelToAdd.TXN_ID = masterDetails.TXN_ID; //new change done by Vikram on 24 Jan 2014
                    ModelToAdd.CHQ_Book_ID = model.CHQ_Book_ID == 0 ? null : model.CHQ_Book_ID;
                    ModelToAdd.CHQ_NO = model.CHQ_NO;
                    ModelToAdd.CHQ_DATE = objCommon.GetStringToDateTime(model.CHQ_DATE);
                    ModelToAdd.CHQ_AMOUNT = masterDetails.CHQ_AMOUNT;
                    ModelToAdd.CASH_AMOUNT = 0;
                    ModelToAdd.GROSS_AMOUNT = masterDetails.CHQ_AMOUNT; //calculated in controller
                    ModelToAdd.CHALAN_NO = masterDetails.CHALAN_NO;
                    ModelToAdd.CHALAN_DATE = masterDetails.CHALAN_DATE;
                    ModelToAdd.PAYEE_NAME = masterDetails.PAYEE_NAME; //found out payee name based on the transaction type in controller
                    ModelToAdd.CHQ_EPAY = masterDetails.CHQ_EPAY;
                    ModelToAdd.TEO_TRANSFER_TYPE = null;
                    ModelToAdd.REMIT_TYPE = masterDetails.REMIT_TYPE;
                    ModelToAdd.BILL_FINALIZED = "Y";
                    ModelToAdd.FUND_TYPE = masterDetails.FUND_TYPE;
                    ModelToAdd.ADMIN_ND_CODE = masterDetails.ADMIN_ND_CODE;
                    ModelToAdd.LVL_ID = masterDetails.LVL_ID;
                    ModelToAdd.MAST_CON_ID = masterDetails.MAST_CON_ID; //found out based on transaction type
                    ModelToAdd.BILL_TYPE = "P";

                    //Added by abhishek kamble 29-nov-2013
                    ModelToAdd.USERID = PMGSYSession.Current.UserId;
                    ModelToAdd.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    //negative new entry 
                    ACC_BILL_MASTER NegativeAmtModel = new ACC_BILL_MASTER();

                    maxBillId = maxBillId + 1;
                    NegativeAmtModel.BILL_ID = maxBillId;
                    NegativeAmtModel.BILL_NO = model.BILL_NO + "-" + "R";  // model.BILL_NO + "-" + "R";
                    //NegativeAmtModel.BILL_NO = masterDetails.BILL_NO; // new change done by Vikram as per mam on 24 Jan 2014
                    NegativeAmtModel.BILL_MONTH = Convert.ToInt16(model.BILL_DATE.Split('/')[1]); //find out in controller
                    NegativeAmtModel.BILL_YEAR = Convert.ToInt16(model.BILL_DATE.Split('/')[2]); //find out in controller
                    NegativeAmtModel.BILL_DATE = objCommon.GetStringToDateTime(model.BILL_DATE);
                    //NegativeAmtModel.TXN_ID = 228;
                    //new change done by Vikram on 24 Jan 2014
                    switch (PMGSYSession.Current.FundType)
                    {
                        case "P":
                            NegativeAmtModel.TXN_ID = 228;
                            break;
                        case "A":
                            NegativeAmtModel.TXN_ID = 624;
                            break;
                        case "M":
                            NegativeAmtModel.TXN_ID = 825;
                            break;
                        default:
                            break;
                    }
                    //NegativeAmtModel.TXN_ID = masterDetails.TXN_ID;
                    NegativeAmtModel.CHQ_Book_ID = model.CHQ_Book_ID == 0 ? null : model.CHQ_Book_ID;
                    NegativeAmtModel.CHQ_NO = masterDetails.CHQ_NO; //model.CHQ_NO;
                    NegativeAmtModel.CHQ_DATE = objCommon.GetStringToDateTime(model.CHQ_DATE);
                    NegativeAmtModel.CHQ_AMOUNT = -(masterDetails.CHQ_AMOUNT);
                    NegativeAmtModel.CASH_AMOUNT = 0;
                    NegativeAmtModel.GROSS_AMOUNT = -(masterDetails.CHQ_AMOUNT); //calculated in controller
                    NegativeAmtModel.CHALAN_NO = masterDetails.CHALAN_NO;
                    NegativeAmtModel.CHALAN_DATE = masterDetails.CHALAN_DATE;
                    NegativeAmtModel.PAYEE_NAME = masterDetails.PAYEE_NAME; //found out payee name based on the transaction type in controller
                    NegativeAmtModel.CHQ_EPAY = masterDetails.CHQ_EPAY;
                    NegativeAmtModel.TEO_TRANSFER_TYPE = null;
                    NegativeAmtModel.REMIT_TYPE = masterDetails.REMIT_TYPE;
                    NegativeAmtModel.BILL_FINALIZED = "Y";
                    NegativeAmtModel.FUND_TYPE = masterDetails.FUND_TYPE;
                    NegativeAmtModel.ADMIN_ND_CODE = masterDetails.ADMIN_ND_CODE;
                    NegativeAmtModel.LVL_ID = masterDetails.LVL_ID;
                    NegativeAmtModel.MAST_CON_ID = masterDetails.MAST_CON_ID; //found out based on transaction type
                    NegativeAmtModel.BILL_TYPE = "P";

                    //Added by abhishek kamble 29-nov-2013
                    NegativeAmtModel.USERID = PMGSYSession.Current.UserId;
                    NegativeAmtModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    dbContext.ACC_BILL_MASTER.Add(ModelToAdd);
                    dbContext.ACC_BILL_MASTER.Add(NegativeAmtModel);


                    //ACC_CHEQUES_ISSUED chqMaster = new ACC_CHEQUES_ISSUED();
                    //chqMaster.BILL_ID = maxBillId - 1;
                    ////chqMaster.BANK_CODE = dbContext.ACC_BANK_DETAILS.Where(m=>m.ADMIN_ND_CODE == PMGSYSession.Current.ParentNDCode && m.BANK_ACC_STATUS == true).Select(m=>(short?)m.BANK_CODE).FirstOrDefault();
                    //chqMaster.BANK_CODE = GetBankCodeBasedOnChequeNumber(PMGSYSession.Current.AdminNdCode, PMGSYSession.Current.FundType, Convert.ToInt32(model.CHQ_NO));
                    //chqMaster.CHEQUE_STATUS = "N";
                    //chqMaster.IS_CHQ_ENCASHED_NA = false;
                    //chqMaster.IS_CHQ_RECONCILE_BANK = false;
                    //chqMaster.NA_BILL_ID = null;
                    //chqMaster.CHQ_RECONCILE_DATE = null;
                    //chqMaster.CHQ_RECONCILE_REMARKS = null;
                    //NegativeAmtModel.USERID = PMGSYSession.Current.UserId;
                    //NegativeAmtModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    //dbContext.ACC_CHEQUES_ISSUED.Add(chqMaster);
                    //dbContext.SaveChanges();


                    //If Condition commented by Abhishek kamble to Add Cheque Book Details At SRRDA level 12Nov2014
                    //if (PMGSYSession.Current.LevelId == 5)//If Condition Added by Abhishek kamble 24-july-2014 to add Cheque Issued Details only At SRRDA level.
                    //{
                    ACC_CHEQUES_ISSUED chqMaster1 = new ACC_CHEQUES_ISSUED();
                    chqMaster1.BILL_ID = maxBillId;
                    //chqMaster.BANK_CODE = dbContext.ACC_BANK_DETAILS.Where(m=>m.ADMIN_ND_CODE == PMGSYSession.Current.ParentNDCode && m.BANK_ACC_STATUS == true).Select(m=>(short?)m.BANK_CODE).FirstOrDefault();

                    if (model.CHQ_EPAY == "A")//If condition added by Abhishek for Advice No 6Apr2015
                    {
                        chqMaster1.BANK_CODE = dbContext.ACC_BANK_DETAILS.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.ParentNDCode && x.FUND_TYPE == PMGSYSession.Current.FundType && x.BANK_ACC_STATUS == true && x.ACCOUNT_TYPE == "S").Select(z => z.BANK_CODE).FirstOrDefault();
                    }
                    else
                    {
                        if (masterDetails.CHQ_Book_ID != null)
                        {
                            chqMaster1.BANK_CODE = dbContext.ACC_CHQ_BOOK_DETAILS.Where(x => x.CHQ_BOOK_ID == masterDetails.CHQ_Book_ID).Select(x => x.BANK_CODE).FirstOrDefault();
                        }
                        else
                        {
                            chqMaster1.BANK_CODE = dbContext.ACC_BANK_DETAILS.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.ParentNDCode && x.FUND_TYPE == PMGSYSession.Current.FundType && x.BANK_ACC_STATUS == true && x.ACCOUNT_TYPE == "S").Select(z => z.BANK_CODE).FirstOrDefault();
                        }
                       // chqMaster1.BANK_CODE = GetBankCodeBasedOnChequeNumber(PMGSYSession.Current.AdminNdCode, PMGSYSession.Current.FundType, Convert.ToInt32(model.CHQ_NO));
                    }
                    chqMaster1.CHEQUE_STATUS = "R";
                    chqMaster1.IS_CHQ_ENCASHED_NA = false;
                    chqMaster1.IS_CHQ_RECONCILE_BANK = false;
                    chqMaster1.NA_BILL_ID = null;
                    chqMaster1.CHQ_RECONCILE_DATE = null;
                    chqMaster1.CHQ_RECONCILE_REMARKS = null;
                    NegativeAmtModel.USERID = PMGSYSession.Current.UserId;
                    NegativeAmtModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    dbContext.ACC_CHEQUES_ISSUED.Add(chqMaster1);
                    //}

                    dbContext.SaveChanges();


                    #endregion

                    #region update the cheque status to "R"

                    ACC_CHEQUES_ISSUED old_model = new ACC_CHEQUES_ISSUED();
                    old_model = dbContext.ACC_CHEQUES_ISSUED.Where(x => x.BILL_ID == bill_Id).FirstOrDefault();
                    if (old_model != null)
                    {
                        old_model.CHEQUE_STATUS = "R";

                        //Added by abhishek kamble 29-nov-2013
                        old_model.USERID = PMGSYSession.Current.UserId;
                        old_model.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                        dbContext.Entry(old_model).State = System.Data.Entity.EntityState.Modified;
                    }


                    #endregion

                    #region add details for new cheque in cheque issued details

                    //If Condition Added by Abhishek kamble to Add Cheque Book Details At SRRDA level 12Nov2014
                    //if (model.CHQ_Book_ID != 0 ) //old
                    if ((model.CHQ_Book_ID != 0) || (PMGSYSession.Current.LevelId == 4) || model.CHQ_EPAY == "A")//CHALAN_NO added by Abhishek for Advice No 6Apr2015
                    {
                        ACC_CHEQUES_ISSUED chequeIssuedModel = new ACC_CHEQUES_ISSUED();
                        chequeIssuedModel.BILL_ID = (maxBillId - 1);



                    

                        if (model.CHQ_EPAY == "A")//If condition added by Abhishek for Advice No 6Apr2015
                        {
                            chequeIssuedModel.BANK_CODE = dbContext.ACC_BANK_DETAILS.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.ParentNDCode && x.FUND_TYPE == PMGSYSession.Current.FundType && x.BANK_ACC_STATUS == true && x.ACCOUNT_TYPE == "S").Select(z => z.BANK_CODE).FirstOrDefault();
                        }
                        else
                        {

                            if (masterDetails.CHQ_Book_ID != null)
                            {
                                chqMaster1.BANK_CODE = dbContext.ACC_CHQ_BOOK_DETAILS.Where(x => x.CHQ_BOOK_ID == masterDetails.CHQ_Book_ID).Select(x => x.BANK_CODE).FirstOrDefault();
                            }
                            else
                            {
                                chqMaster1.BANK_CODE = dbContext.ACC_BANK_DETAILS.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.ParentNDCode && x.FUND_TYPE == PMGSYSession.Current.FundType && x.BANK_ACC_STATUS == true).Select(z => z.BANK_CODE).FirstOrDefault();
                            }

                            //chequeIssuedModel.BANK_CODE = GetBankCodeBasedOnChequeNumber(masterDetails.ADMIN_ND_CODE, masterDetails.FUND_TYPE, Convert.ToInt32(model.CHQ_NO));
                        }
                        chequeIssuedModel.CHEQUE_STATUS = "N";

                        //Added by abhishek kamble 29-nov-2013
                        chequeIssuedModel.USERID = PMGSYSession.Current.UserId;
                        chequeIssuedModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                        dbContext.ACC_CHEQUES_ISSUED.Add(chequeIssuedModel);
                    }

                    #endregion add details for new cheque in cheque issued details

                    #region entry in bill details

                    //get the original transaction for cheque only 

                    List<ACC_BILL_DETAILS> originalTransactionDetails = new List<ACC_BILL_DETAILS>();

                    originalTransactionDetails = dbContext.ACC_BILL_DETAILS.Where(b => b.BILL_ID == bill_Id && b.CASH_CHQ == "Q").OrderBy(c => c.TXN_NO).ToList<ACC_BILL_DETAILS>();

                    Int16 maxTxnNo = 0;

                    if (dbContext.ACC_BILL_DETAILS.Where(t => t.BILL_ID == (maxBillId - 1)).Any())
                    {
                        maxTxnNo = dbContext.ACC_BILL_DETAILS.Where(t => t.BILL_ID == (maxBillId - 1)).Max(c => c.TXN_NO);
                    }

                    int i = 0;
                    int j = 0;

                    ACC_BILL_DETAILS[] BillModelToAdd = new ACC_BILL_DETAILS[(originalTransactionDetails.Count / 2)];
                    ACC_BILL_DETAILS[] ModelToAddDebit = new ACC_BILL_DETAILS[(originalTransactionDetails.Count / 2)];

                    foreach (ACC_BILL_DETAILS item in originalTransactionDetails)
                    {



                        //new entry
                        if (item.CREDIT_DEBIT == "C")
                        {
                            //creating credit entry Cheque payment
                            maxTxnNo = Convert.ToInt16(maxTxnNo + 1);
                            BillModelToAdd[i] = new ACC_BILL_DETAILS();
                            BillModelToAdd[i].BILL_ID = (maxBillId - 1);
                            BillModelToAdd[i].TXN_NO = maxTxnNo;
                            BillModelToAdd[i].TXN_ID = item.TXN_ID;
                            BillModelToAdd[i].HEAD_ID = item.HEAD_ID;
                            BillModelToAdd[i].AMOUNT = item.AMOUNT;
                            BillModelToAdd[i].CREDIT_DEBIT = item.CREDIT_DEBIT;
                            BillModelToAdd[i].CASH_CHQ = item.CASH_CHQ;
                            BillModelToAdd[i].NARRATION = model.NARRATION;
                            // BillModelToAdd[i].ADMIN_ND_CODE = masterDetails.ADMIN_ND_CODE; //removed as per instruction in testing  report by madam on 10/06/2013
                            BillModelToAdd[i].MAST_CON_ID = item.MAST_CON_ID;
                            BillModelToAdd[i].IMS_PR_ROAD_CODE = item.IMS_PR_ROAD_CODE;
                            BillModelToAdd[i].IMS_AGREEMENT_CODE = item.IMS_AGREEMENT_CODE;
                            BillModelToAdd[i].MAS_FA_CODE = item.MAS_FA_CODE;
                            BillModelToAdd[i].FINAL_PAYMENT = item.FINAL_PAYMENT;
                            BillModelToAdd[i].MAST_DISTRICT_CODE = item.MAST_DISTRICT_CODE;

                            //Added By Abhishek Kamble 29-nov-2013
                            BillModelToAdd[i].USERID = PMGSYSession.Current.UserId;
                            BillModelToAdd[i].IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                            dbContext.ACC_BILL_DETAILS.Add(BillModelToAdd[i]);

                            i = i++;
                        }

                        //creating debit cheque payment entry
                        if (item.CREDIT_DEBIT == "D")
                        {
                            maxTxnNo = Convert.ToInt16(maxTxnNo + 1);
                            ModelToAddDebit[j] = new ACC_BILL_DETAILS();
                            ModelToAddDebit[j].BILL_ID = (maxBillId - 1); ;
                            ModelToAddDebit[j].TXN_NO = maxTxnNo;
                            ModelToAddDebit[j].TXN_ID = item.TXN_ID;
                            ModelToAddDebit[j].HEAD_ID = item.HEAD_ID;
                            ModelToAddDebit[j].AMOUNT = item.AMOUNT;
                            ModelToAddDebit[j].CREDIT_DEBIT = item.CREDIT_DEBIT;
                            ModelToAddDebit[j].CASH_CHQ = item.CASH_CHQ; ;
                            ModelToAddDebit[j].NARRATION = model.NARRATION;
                            // ModelToAddDebit[j].ADMIN_ND_CODE = masterDetails.ADMIN_ND_CODE;
                            ModelToAddDebit[j].MAST_CON_ID = item.MAST_CON_ID;
                            ModelToAddDebit[j].IMS_PR_ROAD_CODE = item.IMS_PR_ROAD_CODE;
                            ModelToAddDebit[j].IMS_AGREEMENT_CODE = item.IMS_AGREEMENT_CODE;
                            ModelToAddDebit[j].MAS_FA_CODE = item.MAS_FA_CODE;
                            ModelToAddDebit[j].FINAL_PAYMENT = item.FINAL_PAYMENT;
                            ModelToAddDebit[j].MAST_DISTRICT_CODE = item.MAST_DISTRICT_CODE;

                            //Added By Abhishek Kamble 29-nov-2013
                            ModelToAddDebit[j].USERID = PMGSYSession.Current.UserId;
                            ModelToAddDebit[j].IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                            dbContext.ACC_BILL_DETAILS.Add(ModelToAddDebit[j]);
                            j = j++;
                        }



                    }


                    ACC_BILL_DETAILS[] negBillModelCredit = new ACC_BILL_DETAILS[(originalTransactionDetails.Count / 2)];
                    ACC_BILL_DETAILS[] negBillModeldebit = new ACC_BILL_DETAILS[(originalTransactionDetails.Count / 2)];

                    i = 0;
                    j = 0;
                    maxTxnNo = 0;
                    foreach (ACC_BILL_DETAILS item in originalTransactionDetails)
                    {



                        //new negative entry

                        if (dbContext.ACC_BILL_DETAILS.Where(t => t.BILL_ID == (maxBillId)).Any())
                        {
                            maxTxnNo = dbContext.ACC_BILL_DETAILS.Where(t => t.BILL_ID == (maxBillId)).Max(c => c.TXN_NO);
                        }

                        if (item.CREDIT_DEBIT == "C")
                        {
                            maxTxnNo = Convert.ToInt16(maxTxnNo + 1);
                            negBillModelCredit[i] = new ACC_BILL_DETAILS();
                            negBillModelCredit[i].BILL_ID = (maxBillId);
                            negBillModelCredit[i].TXN_NO = maxTxnNo;
                            negBillModelCredit[i].TXN_ID = item.TXN_ID;
                            negBillModelCredit[i].HEAD_ID = item.HEAD_ID;
                            negBillModelCredit[i].AMOUNT = -item.AMOUNT;
                            negBillModelCredit[i].CREDIT_DEBIT = item.CREDIT_DEBIT;
                            negBillModelCredit[i].CASH_CHQ = item.CASH_CHQ;
                            negBillModelCredit[i].NARRATION = model.NARRATION;
                            // negBillModelCredit[i].ADMIN_ND_CODE = masterDetails.ADMIN_ND_CODE;
                            negBillModelCredit[i].MAST_CON_ID = item.MAST_CON_ID;
                            negBillModelCredit[i].IMS_PR_ROAD_CODE = item.IMS_PR_ROAD_CODE;
                            negBillModelCredit[i].IMS_AGREEMENT_CODE = item.IMS_AGREEMENT_CODE;
                            negBillModelCredit[i].MAS_FA_CODE = item.MAS_FA_CODE;
                            negBillModelCredit[i].FINAL_PAYMENT = item.FINAL_PAYMENT;
                            negBillModelCredit[i].MAST_DISTRICT_CODE = item.MAST_DISTRICT_CODE;

                            //Added By Abhishek Kamble 29-nov-2013
                            negBillModelCredit[i].USERID = PMGSYSession.Current.UserId;
                            negBillModelCredit[i].IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                            dbContext.ACC_BILL_DETAILS.Add(negBillModelCredit[i]);
                            i = i++;
                        }
                        //creating debit cheque payment entry
                        if (item.CREDIT_DEBIT == "D")
                        {
                            maxTxnNo = Convert.ToInt16(maxTxnNo + 1);
                            negBillModeldebit[j] = new ACC_BILL_DETAILS();
                            negBillModeldebit[j].BILL_ID = (maxBillId);
                            negBillModeldebit[j].TXN_NO = maxTxnNo;
                            negBillModeldebit[j].TXN_ID = item.TXN_ID;
                            negBillModeldebit[j].HEAD_ID = item.HEAD_ID;
                            negBillModeldebit[j].AMOUNT = -item.AMOUNT;
                            negBillModeldebit[j].CREDIT_DEBIT = item.CREDIT_DEBIT;
                            negBillModeldebit[j].CASH_CHQ = item.CASH_CHQ;
                            negBillModeldebit[j].NARRATION = model.NARRATION;
                            //negBillModeldebit[j].ADMIN_ND_CODE = masterDetails.ADMIN_ND_CODE;
                            negBillModeldebit[j].MAST_CON_ID = item.MAST_CON_ID;
                            negBillModeldebit[j].IMS_PR_ROAD_CODE = item.IMS_PR_ROAD_CODE;
                            negBillModeldebit[j].IMS_AGREEMENT_CODE = item.IMS_AGREEMENT_CODE;
                            negBillModeldebit[j].MAS_FA_CODE = item.MAS_FA_CODE;
                            negBillModeldebit[j].FINAL_PAYMENT = item.FINAL_PAYMENT;
                            negBillModeldebit[j].MAST_DISTRICT_CODE = item.MAST_DISTRICT_CODE;

                            //Added By Abhishek Kamble 29-nov-2013
                            negBillModeldebit[j].USERID = PMGSYSession.Current.UserId;
                            negBillModeldebit[j].IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                            dbContext.ACC_BILL_DETAILS.Add(negBillModeldebit[j]);
                            j = j++;
                        }


                    }

                    #endregion

                    #region entry in acc cancelled cheques

                    ACC_CANCELLED_CHEQUES CheqModel = new ACC_CANCELLED_CHEQUES();

                    Int64 maxCCId = 0;

                    if (dbContext.ACC_CANCELLED_CHEQUES.Any())
                    {
                        maxCCId = dbContext.ACC_CANCELLED_CHEQUES.Max(c => c.CC_ID);
                    }

                    maxCCId = maxCCId + 1;
                    CheqModel.CC_ID = maxCCId;
                    CheqModel.BILL_ID = (maxBillId - 1);
                    CheqModel.R_BILL_DATE = objCommon.GetStringToDateTime(model.BILL_DATE);
                    CheqModel.OLD_BILL_ID = bill_Id;
                    CheqModel.OLD_BILL_DATE = masterDetails.BILL_DATE;
                    CheqModel.R_CHQ_NO = model.CHQ_NO;
                    CheqModel.ADMIN_ND_CODE = masterDetails.ADMIN_ND_CODE;
                    CheqModel.REASON = model.NARRATION;

                    //Added By Abhishek Kamble 29-nov-2013
                    CheqModel.USERID = PMGSYSession.Current.UserId;
                    CheqModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    dbContext.ACC_CANCELLED_CHEQUES.Add(CheqModel);
                    #endregion




                    int fiscalYear = 0;
                    if (Convert.ToInt16(model.BILL_DATE.Split('/')[1]) <= 3)
                    {
                        fiscalYear = (Convert.ToInt16(model.BILL_DATE.Split('/')[2]) - 1);
                    }
                    else
                    {
                        fiscalYear = Convert.ToInt16(model.BILL_DATE.Split('/')[2]);
                    }


                    ACC_VOUCHER_NUMBER_MASTER oldVoucherNumberModel = new ACC_VOUCHER_NUMBER_MASTER();
                    oldVoucherNumberModel = dbContext.ACC_VOUCHER_NUMBER_MASTER.Where(x => x.ADMIN_ND_CODE == ModelToAdd.ADMIN_ND_CODE && x.FUND_TYPE == ModelToAdd.FUND_TYPE && x.BILL_TYPE == ModelToAdd.BILL_TYPE && x.FISCAL_YEAR == fiscalYear).FirstOrDefault();
                    ACC_VOUCHER_NUMBER_MASTER newMvoucherNumberModel = new ACC_VOUCHER_NUMBER_MASTER();

                    newMvoucherNumberModel.ADMIN_ND_CODE = ModelToAdd.ADMIN_ND_CODE;
                    newMvoucherNumberModel.FUND_TYPE = ModelToAdd.FUND_TYPE;
                    newMvoucherNumberModel.BILL_TYPE = ModelToAdd.BILL_TYPE;
                    newMvoucherNumberModel.FISCAL_YEAR = fiscalYear;
                    newMvoucherNumberModel.SLNO = oldVoucherNumberModel.SLNO + 1;

                    dbContext.Entry(oldVoucherNumberModel).CurrentValues.SetValues(newMvoucherNumberModel);


                    dbContext.SaveChanges();

                    scope.Complete();

                    return "1";

                }

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PaymentDAL.RenewCheque");
               // Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error while renewing cheque details");
            }
            finally
            {
                dbContext.Dispose();
            }



        }

        /// <summary>
        /// function to cancel the cheque details
        /// </summary>
        /// <param name="bill_Id"></param>
        /// <returns>
        /// -111 if cheque is already encashed
        /// -222 is cheque is encashed by bank
        /// -333 cheque is not new 
        /// -444 cheque date is greater than cheque cancellation date
        /// -777 cheque is of imprest and is settled using receipt or teo
        /// -888 if asset details has been entered against the payment
        /// </returns>
        public String CancelCheque(Int64 bill_Id, ChequeCancellModel model)
        {
            CommonFunctions objCommon = new CommonFunctions();
            dbContext = new PMGSYEntities();
            try
            {
                using (var scope = new TransactionScope())
                {
                    //get the bill details from the Master table             
                    ACC_BILL_MASTER masterDetails = new ACC_BILL_MASTER();

                    masterDetails = dbContext.ACC_BILL_MASTER.Where(c => c.BILL_ID == bill_Id).First();

                    //get the cheque details from [ACC_CHEQUES_ISSUED]

                    ACC_CHEQUES_ISSUED chequeIssued = null;

                    //At SRRDA level if check is issued  ACC_CHEQUES_ISSUED table may have no reference to it 
                    if (PMGSYSession.Current.LevelId == 4)
                    {

                        if (dbContext.ACC_CHEQUES_ISSUED.Where(c => c.BILL_ID == bill_Id).Any())
                        {
                            chequeIssued = new ACC_CHEQUES_ISSUED();
                            chequeIssued = dbContext.ACC_CHEQUES_ISSUED.Where(c => c.BILL_ID == bill_Id).First();
                        }

                    }
                    else if (PMGSYSession.Current.LevelId == 5)
                    {
                        chequeIssued = new ACC_CHEQUES_ISSUED();
                        chequeIssued = dbContext.ACC_CHEQUES_ISSUED.Where(c => c.BILL_ID == bill_Id).First();
                    }

                    if (chequeIssued != null)
                    {

                        //check whether cheque is encashed
                        //if (chequeIssued.IS_CHQ_ENCASHED_NA)
                        //{
                        //    return "-111";

                        //}
                        if (chequeIssued.IS_CHQ_RECONCILE_BANK)
                        {
                            //check whether cheque is encashed by bank
                            return "-222";

                        }
                        if (!chequeIssued.CHEQUE_STATUS.Equals("N"))
                        {
                            //cheque is not new 
                            return "-333";
                        }
                        else if (masterDetails.CHQ_DATE > objCommon.GetStringToDateTime(model.CHEQUE_CANCEL_DATE))
                        {
                            // cheque date is greater than cheque cancellation date
                            return "-444";
                        }
                        /// Validation for cheque cancellation is removed as per request of states 
                        else if (masterDetails.CHQ_DATE <= objCommon.GetStringToDateTime(model.CHEQUE_CANCEL_DATE))
                        {
                            int validationMonths = Convert.ToInt32(ConfigurationManager.AppSettings["ChequeCancelRenewMonths"]);
                            if (Convert.ToDateTime(model.CHEQUE_CANCEL_DATE) > Convert.ToDateTime(masterDetails.CHQ_DATE).AddMonths(validationMonths))
                            {
                                return "-999";
                            }
                        }
                        //if payment is of imprest and it has been settled it cant be cancelled
                        else if (dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Where(x => x.P_BILL_ID == bill_Id).Select(x => x.S_BIll_ID).Any())
                        {
                            return "-777";
                        }
                        else if (dbContext.ACC_ASSET_DETAILS.Any(x => x.BILL_ID == bill_Id))
                        {
                            return "-888"; //if asset details has been entered against the payment dont allow to cancell (date 06082013)
                        }
                    }

                    #region old code before updatation of cheque cancellation logic
                    /*
                #region new negative entry

                //new entry     
                    ACC_BILL_MASTER ModelToAdd = new ACC_BILL_MASTER();

                    Int64 maxBillId = 0;

                    if (dbContext.ACC_BILL_MASTER.Any())
                    {
                        maxBillId = dbContext.ACC_BILL_MASTER.Max(c => c.BILL_ID);
                    }

                    maxBillId = maxBillId + 1;

                    ACC_BILL_MASTER NegativeAmtModel = new ACC_BILL_MASTER();

                    maxBillId = maxBillId + 1;
                    NegativeAmtModel.BILL_ID = maxBillId;
                    NegativeAmtModel.BILL_NO = masterDetails.BILL_NO + "-" + "C";
                    NegativeAmtModel.BILL_MONTH = Convert.ToInt16(model.CHEQUE_CANCEL_DATE.Split('/')[1]); 
                    NegativeAmtModel.BILL_YEAR = Convert.ToInt16(model.CHEQUE_CANCEL_DATE.Split('/')[2]); 
                    NegativeAmtModel.BILL_DATE = objCommon.GetStringToDateTime(model.CHEQUE_CANCEL_DATE);
                    NegativeAmtModel.TXN_ID = 229;
                    NegativeAmtModel.CHQ_Book_ID = null;
                    NegativeAmtModel.CHQ_NO = masterDetails.CHQ_NO;
                    NegativeAmtModel.CHQ_DATE = objCommon.GetStringToDateTime(model.CHEQUE_CANCEL_DATE);
                    NegativeAmtModel.CHQ_AMOUNT = -(masterDetails.CHQ_AMOUNT);
                    NegativeAmtModel.CASH_AMOUNT = 0;
                    NegativeAmtModel.GROSS_AMOUNT = -(masterDetails.CHQ_AMOUNT); //calculated in controller
                    NegativeAmtModel.CHALAN_NO = masterDetails.CHALAN_NO;
                    NegativeAmtModel.CHALAN_DATE = masterDetails.CHALAN_DATE;
                    NegativeAmtModel.PAYEE_NAME = masterDetails.PAYEE_NAME; //found out payee name based on the transaction type in controller
                    NegativeAmtModel.CHQ_EPAY = masterDetails.CHQ_EPAY;
                    NegativeAmtModel.TEO_TRANSFER_TYPE = null;
                    NegativeAmtModel.REMIT_TYPE = masterDetails.REMIT_TYPE;
                    NegativeAmtModel.BILL_FINALIZED = "Y";
                    NegativeAmtModel.FUND_TYPE = masterDetails.FUND_TYPE;
                    NegativeAmtModel.ADMIN_ND_CODE = masterDetails.ADMIN_ND_CODE;
                    NegativeAmtModel.LVL_ID = masterDetails.LVL_ID;
                    NegativeAmtModel.MAST_CON_ID = masterDetails.MAST_CON_ID; //found out based on transaction type
                    NegativeAmtModel.BILL_TYPE = "P";

                    dbContext.ACC_BILL_MASTER.Add(NegativeAmtModel);


                    #endregion
                #region update the cheque status to "C"

                    ACC_CHEQUES_ISSUED old_model = new ACC_CHEQUES_ISSUED();
                    old_model = dbContext.ACC_CHEQUES_ISSUED.Where(x => x.BILL_ID == bill_Id).FirstOrDefault();
                    if (old_model != null)
                    {
                        old_model.CHEQUE_STATUS = "C";
                        dbContext.Entry(old_model).State = System.Data.Entity.EntityState.Modified;
                    }

                    #endregion
                #region entries in bill details 

                    //get the original transaction for cheque only 

                    List<ACC_BILL_DETAILS> originalTransactionDetails = new List<ACC_BILL_DETAILS>();

                    originalTransactionDetails = dbContext.ACC_BILL_DETAILS.Where(b => b.BILL_ID == bill_Id && b.CASH_CHQ == "Q").OrderBy(c => c.TXN_NO).ToList<ACC_BILL_DETAILS>();

                    Int16 maxTxnNo = 0;

                    if (dbContext.ACC_BILL_DETAILS.Where(t => t.BILL_ID == (maxBillId - 1)).Any())
                    {
                        maxTxnNo = dbContext.ACC_BILL_DETAILS.Where(t => t.BILL_ID == (maxBillId - 1)).Max(c => c.TXN_NO);
                    }


                    ACC_BILL_DETAILS[] negBillModelCredit = new ACC_BILL_DETAILS[(originalTransactionDetails.Count / 2)];
                    ACC_BILL_DETAILS[] negBillModeldebit = new ACC_BILL_DETAILS[(originalTransactionDetails.Count / 2)];

                    int  i = 0;
                    int j = 0;
                    maxTxnNo = 0;
                    foreach (ACC_BILL_DETAILS item in originalTransactionDetails)
                    {
                        
                        //new negative entry

                        if (dbContext.ACC_BILL_DETAILS.Where(t => t.BILL_ID == (maxBillId)).Any())
                        {
                            maxTxnNo = dbContext.ACC_BILL_DETAILS.Where(t => t.BILL_ID == (maxBillId)).Max(c => c.TXN_NO);
                        }

                        if (item.CREDIT_DEBIT == "C")
                        {
                            maxTxnNo = Convert.ToInt16(maxTxnNo + 1);
                            negBillModelCredit[i] = new ACC_BILL_DETAILS();
                            negBillModelCredit[i].BILL_ID = (maxBillId);
                            negBillModelCredit[i].TXN_NO = maxTxnNo;
                            negBillModelCredit[i].TXN_ID = item.TXN_ID;
                            negBillModelCredit[i].HEAD_ID = item.HEAD_ID;
                            negBillModelCredit[i].AMOUNT = -item.AMOUNT;
                            negBillModelCredit[i].CREDIT_DEBIT = item.CREDIT_DEBIT;
                            negBillModelCredit[i].CASH_CHQ = item.CASH_CHQ;
                            negBillModelCredit[i].NARRATION = model.CHEQUE_CANCEL_NARRATION;
                            //negBillModelCredit[i].ADMIN_ND_CODE = masterDetails.ADMIN_ND_CODE;
                            negBillModelCredit[i].MAST_CON_ID = item.MAST_CON_ID;
                            negBillModelCredit[i].IMS_PR_ROAD_CODE = item.IMS_PR_ROAD_CODE;
                            negBillModelCredit[i].IMS_AGREEMENT_CODE = item.IMS_AGREEMENT_CODE;
                            negBillModelCredit[i].MAS_FA_CODE = item.MAS_FA_CODE;
                            negBillModelCredit[i].FINAL_PAYMENT = item.FINAL_PAYMENT;
                            negBillModelCredit[i].MAST_DISTRICT_CODE = item.MAST_DISTRICT_CODE;
                            dbContext.ACC_BILL_DETAILS.Add(negBillModelCredit[i]);
                            i = i++;
                        }
                        //creating debit cheque payment entry
                        if (item.CREDIT_DEBIT == "D")
                        {
                            maxTxnNo = Convert.ToInt16(maxTxnNo + 1);
                            negBillModeldebit[j] = new ACC_BILL_DETAILS();
                            negBillModeldebit[j].BILL_ID = (maxBillId);
                            negBillModeldebit[j].TXN_NO = maxTxnNo;
                            negBillModeldebit[j].TXN_ID = item.TXN_ID;
                            negBillModeldebit[j].HEAD_ID = item.HEAD_ID;
                            negBillModeldebit[j].AMOUNT = -item.AMOUNT;
                            negBillModeldebit[j].CREDIT_DEBIT = item.CREDIT_DEBIT;
                            negBillModeldebit[j].CASH_CHQ = item.CASH_CHQ;
                            negBillModeldebit[j].NARRATION = model.CHEQUE_CANCEL_NARRATION;
                            //negBillModeldebit[j].ADMIN_ND_CODE = masterDetails.ADMIN_ND_CODE;
                            negBillModeldebit[j].MAST_CON_ID = item.MAST_CON_ID;
                            negBillModeldebit[j].IMS_PR_ROAD_CODE = item.IMS_PR_ROAD_CODE;
                            negBillModeldebit[j].IMS_AGREEMENT_CODE = item.IMS_AGREEMENT_CODE;
                            negBillModeldebit[j].MAS_FA_CODE = item.MAS_FA_CODE;
                            negBillModeldebit[j].FINAL_PAYMENT = item.FINAL_PAYMENT;
                            negBillModeldebit[j].MAST_DISTRICT_CODE = item.MAST_DISTRICT_CODE;
                            dbContext.ACC_BILL_DETAILS.Add(negBillModeldebit[j]);
                            j = j++;
                        }


                    }


                    #endregion
                #region entry in acc cancelled cheques

                    ACC_CANCELLED_CHEQUES CheqModel = new ACC_CANCELLED_CHEQUES();

                    Int64 maxCCId = 0;

                    if (dbContext.ACC_CANCELLED_CHEQUES.Any())
                    {
                        maxCCId = dbContext.ACC_CANCELLED_CHEQUES.Max(c => c.CC_ID);
                    }

                    maxCCId = maxCCId + 1;
                    CheqModel.CC_ID = maxCCId;
                    CheqModel.BILL_ID = (maxBillId);
                    CheqModel.R_BILL_DATE = objCommon.GetStringToDateTime(model.CHEQUE_CANCEL_DATE);
                    CheqModel.OLD_BILL_ID = bill_Id;
                    CheqModel.OLD_BILL_DATE = masterDetails.BILL_DATE;
                    CheqModel.R_CHQ_NO = null;
                    CheqModel.ADMIN_ND_CODE = masterDetails.ADMIN_ND_CODE;
                    CheqModel.REASON = model.CHEQUE_CANCEL_NARRATION;
                    dbContext.ACC_CANCELLED_CHEQUES.Add(CheqModel);
                  
                dbContext.SaveChanges();
               
                    */
                #endregion

                    int? result = dbContext.SP_ACC_INSERT_CHEQUE_CANCELLATION_DETAILS
                        (bill_Id,
                        PMGSYSession.Current.FundType,
                        PMGSYSession.Current.AdminNdCode,
                        PMGSYSession.Current.LevelId,
                        objCommon.GetStringToDateTime(model.CHEQUE_CANCEL_DATE),
                        model.CHEQUE_CANCEL_NARRATION, Convert.ToInt16(model.CANCEL_FUND_TYPE), PMGSYSession.Current.UserId, HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]).FirstOrDefault();

                    //added by abhishek kamble 29-nov-2013
                    ACC_CANCELLED_CHEQUES cancelChequesModel = dbContext.ACC_CANCELLED_CHEQUES.Where(m => m.BILL_ID == bill_Id).FirstOrDefault();
                    if (cancelChequesModel != null)
                    {
                        cancelChequesModel.USERID = PMGSYSession.Current.UserId;
                        cancelChequesModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        dbContext.Entry(cancelChequesModel).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }

                    scope.Complete();

                    return result.Value.ToString();


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
                throw new Exception("Error while cancelling cheque details");
            }
            finally
            {
                dbContext.Dispose();
            }
        }



        //public List<SelectListItem> populateFundTypeForCancellation(String fundType)
        public List<SelectListItem> populateFundTypeForCancellation(String fundType, int level)
        {
            try
            {
                dbContext = new PMGSYEntities();

                //List<USP_ACC_GET_Fund_Types_For_Cheque_Cancellation_Result> result = dbContext.USP_ACC_GET_Fund_Types_For_Cheque_Cancellation(fundType).ToList<USP_ACC_GET_Fund_Types_For_Cheque_Cancellation_Result>();
                List<USP_ACC_GET_Fund_Types_For_Cheque_Cancellation_Result> result = dbContext.USP_ACC_GET_Fund_Types_For_Cheque_Cancellation(fundType, level).ToList<USP_ACC_GET_Fund_Types_For_Cheque_Cancellation_Result>();

                List<SelectListItem> lstfund = new List<SelectListItem>();
                int count = 0;
                foreach (USP_ACC_GET_Fund_Types_For_Cheque_Cancellation_Result item in result)
                {
                    SelectListItem listitem = new SelectListItem();
                    listitem.Text = item.NAME;
                    listitem.Value = item.HEAD_ID.ToString();
                    if (count == 0)
                    {
                        listitem.Selected = true;
                        count++;
                    }
                    lstfund.Add(listitem);

                }

                return lstfund;


            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error while getting fund type for cancellation");
            }
            finally
            {
                dbContext.Dispose();
            }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="objFilter"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array ListChequeDetailsForRenewalbyCheque(PaymentFilterModel objFilter, out long totalRecords, out Int64 bill_id)
        {

            CommonFunctions commomFuncObj = new CommonFunctions();

            try
            {
                List<ACC_BILL_MASTER> lstBillMaster = null;
                dbContext = new PMGSYEntities();

                string chq_Num = (objFilter.BillId).ToString().PadLeft(6, '0');

                lstBillMaster = dbContext.ACC_BILL_MASTER.
                Where(m => m.ADMIN_ND_CODE == objFilter.AdminNdCode).
                Where(m => m.LVL_ID == objFilter.LevelId).
                Where(m => m.CHQ_NO == chq_Num).
                 Where(m => m.CHQ_AMOUNT > 0).
                Where(m => m.FUND_TYPE == objFilter.FundType).
                Where(m => m.BILL_TYPE == objFilter.Bill_type)
                .ToList<ACC_BILL_MASTER>();

                totalRecords = lstBillMaster.Count();
                bill_id = 0;
                foreach (var item in lstBillMaster)
                {
                    bill_id = item.BILL_ID;
                    break;
                }

                return lstBillMaster.Select(item => new
                {
                    id = URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() }),
                    cell = new[] {                         
                                    
                                        item.BILL_NO,
                                        commomFuncObj.GetDateTimeToString(item.BILL_DATE),
                                        item.ACC_MASTER_TXN.TXN_DESC.Trim(),
                                        item.CHQ_NO,
                                        item.CHQ_DATE == null ? String.Empty : commomFuncObj.GetDateTimeToString(item.CHQ_DATE.Value),
                                        item.PAYEE_NAME.ToString(),
                                        item.CHQ_AMOUNT.ToString(),
                                        item.CASH_AMOUNT.ToString(),
                                        item.ACC_CHEQUES_ISSUED.IS_CHQ_ENCASHED_NA == true ? "YES":"NO",
                                        item.ACC_CHEQUES_ISSUED.IS_CHQ_RECONCILE_BANK ? "YES":"NO",
                                      
                                        
                        }
                }).ToArray();

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                bill_id = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }

        }

        #endregion

        #region EPAYMENT

        #region vikky

        public Array ListSecondLevelSuccessEPaymentDetails(PaymentFilterModel objFilter, String TransactionType, out long totalRecords, string moduleType)
        {
            CommonFunctions commomFuncObj = new CommonFunctions();
            string description = string.Empty;

            try
            {
                dbContext = new PMGSYEntities();
                var lstBillMaster = (from bm in dbContext.ACC_BILL_MASTER
                                     join hld in dbContext.ACC_BILL_SNA_HOLDING_MAPPING on bm.BILL_ID equals hld.HOLDING_BILL_ID
                                     join bms in dbContext.ACC_BILL_MASTER on hld.SNA_BILL_ID equals bms.BILL_ID
                                     join pmap in dbContext.REAT_OMMAS_PAYMENT_MAPPING on hld.SNA_BILL_ID equals pmap.BILL_ID
                                   //  join mc in dbContext.MASTER_CONTRACTOR on bms.MAST_CON_ID equals mc.MAST_CON_ID
                                     join txn in dbContext.ACC_MASTER_TXN on bm.TXN_ID equals txn.TXN_ID
                                     where
                                      bm.ADMIN_ND_CODE == objFilter.AdminNdCode
                                      && bm.BILL_MONTH == objFilter.Month
                                      && bm.CHQ_EPAY == "E"
                                      && (bm.BILL_FINALIZED == "Y")
                                      && bm.FUND_TYPE == "P"
                                      && bm.BILL_YEAR == objFilter.Year
                                      && pmap.ACK_BILL_STATUS == "A"
                                      && pmap.BANK_ACK_BILL_STATUS == "A"
                                     select new
                                     {
                                         bm.BILL_ID,
                                         bm.BILL_NO,
                                         bm.BILL_DATE,
                                         bm.CHQ_NO,
                                         //bms.CHQ_DATE,
                                        // ContractorName = (mc.MAST_CON_FNAME == null ? "" : mc.MAST_CON_FNAME) + (mc.MAST_CON_MNAME == null ? " " : mc.MAST_CON_MNAME) + (mc.MAST_CON_LNAME == null ? " " : mc.MAST_CON_LNAME) + "(" + (mc.MAST_CON_COMPANY_NAME == null ? "" : mc.MAST_CON_COMPANY_NAME) + ")",
                                         bm.CHQ_AMOUNT,
                                         bm.CASH_AMOUNT,
                                         bm.GROSS_AMOUNT,
                                         txn.TXN_DESC,
                                         bm.BILL_FINALIZED,
                                         MAST_CON_ID = (bm.MAST_CON_ID == null ? 0 : bm.MAST_CON_ID),
                                         CON_ACCOUNT_ID = (bm.CON_ACCOUNT_ID == null ? 0 : bm.CON_ACCOUNT_ID),
                                         pmap.BANK_ACK_RECEIVED_DATE,
                                     }).Distinct().ToList();

                Int32? prmParentNdCode = PMGSYSession.Current.ParentNDCode;




                totalRecords = lstBillMaster.Count();


                if (objFilter.sidx.Trim() != string.Empty)
                {
                    if (objFilter.sord.ToString() == "asc")
                    {
                        switch (objFilter.sidx)
                        {
                            case "Voucher_Number":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.BILL_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                break;
                            case "voucher_date":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.BILL_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                break;
                            //case "Cash_Cheque":
                            //    lstBillMaster = lstBillMaster.OrderBy(x => x.CHQ_EPAY).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                            //    break;
                            case "Transaction_type":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.TXN_DESC).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                break;
                            case "cheque_number":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.CHQ_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                break;
                            /*case "cheque_Date":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.CHQ_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                break;*/
                            case "cheque_amount":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.CHQ_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                break;
                            case "Cash_Amount":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.CASH_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                break;
                            case "Gross_Amount":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.GROSS_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                break;
                            default:
                                lstBillMaster = lstBillMaster.OrderBy(x => x.BILL_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                break;
                        }

                    }
                    else
                    {
                        switch (objFilter.sidx)
                        {
                            case "Voucher_Number":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.BILL_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                break;
                            case "voucher_date":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.BILL_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                break;
                            //case "Cash_Cheque":
                            //    lstBillMaster = lstBillMaster.OrderByDescending(x => x.CHQ_EPAY).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                            //    break;
                            case "Transaction_type":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.TXN_DESC).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                break;
                            case "cheque_number":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.CHQ_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                break;
                            /*case "cheque_Date":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.CHQ_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                break;*/
                            case "cheque_amount":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.CHQ_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                break;
                            case "Cash_Amount":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.CASH_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                break;
                            case "Gross_Amount":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.GROSS_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                break;
                            default:
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.BILL_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstBillMaster = lstBillMaster.OrderByDescending(x => x.BILL_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                }





                return lstBillMaster.Select(item => new
                {
                    id = URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() }),
                    cell = new[] {

                                        item.BILL_NO,
                                        commomFuncObj.GetDateTimeToString(item.BILL_DATE),
                                        item.CHQ_NO,
                                        commomFuncObj.GetDateTimeToString(item.BILL_DATE),
                                    //    item.ContractorName,
                                        item.CHQ_AMOUNT.ToString(),
                                        item.CASH_AMOUNT.ToString(),
                                        item.GROSS_AMOUNT.ToString(),
                                           item.BANK_ACK_RECEIVED_DATE==null?"":Convert.ToDateTime(item.BANK_ACK_RECEIVED_DATE).ToString("dd/MM/yyyy"),
                                        item.TXN_DESC.Trim(),
                                     (dbContext.REAT_OMMAS_HOLDING_ACCOUNT_PAYMENTS.Any(s=>s.BILL_ID==item.BILL_ID))?"":
                                        "<center><a href='#' class='ui-icon ui-icon-mail-closed' onclick='SignSecondlevelSuccessEPaymentREATXml(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()+"$" + (dbContext.ACC_EPAY_MAIL_MASTER.Where(c => c.BILL_ID == item.BILL_ID && c.IS_EPAY_VALID == true).Any() ? "Y" : "N") + "$E$" + item.BILL_FINALIZED.ToString().Trim() + "$" + item.MAST_CON_ID + "$"+ item.CON_ACCOUNT_ID })+ "\");return false;'>Sign Epayment -REAT </a></center>"  ,



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


        public Array GetTransferDeductionAmtListDAL(PaymentFilterModel objFilter, String TransactionType, out long totalRecords, out List<string> voucherGeneratedList)
        {
            CommonFunctions commomFuncObj = new CommonFunctions();
            string description = string.Empty;
            dbContext = new PMGSYEntities();
            try
            {
                dbContext = new PMGSYEntities();
                //var lstBillMaster = dbContext.REAT_TRANSFER_DEDUCTION_AMT_TO_HOLDING_ACCOUNT_DETAILS(objFilter.Month, objFilter.Year, objFilter.AdminNdCode).ToList();

                //if (objFilter.deductionType != "0")
                //{
                //    lstBillMaster = dbContext.REAT_TRANSFER_DEDUCTION_AMT_TO_HOLDING_ACCOUNT_DETAILS(objFilter.Month, objFilter.Year, objFilter.AdminNdCode).Where(s => s.HEAD_DESC == objFilter.deductionType).ToList();


                //}

                int month = System.DateTime.Now.Month;
                int year = System.DateTime.Now.Year;

                #region read xml file HoldingSecurityStateConfigFile 
                XDocument doc_xml = XDocument.Load(System.Web.HttpContext.Current.Server.MapPath("~/HoldingSecurityStateConfigFile.xml"));
                List<HoldingTranfserInitDateModel> holdingTransferInitModelList = new List<HoldingTranfserInitDateModel>();
                foreach (XElement element in doc_xml.Descendants("stateList").Descendants("HoldInit").Descendants("stateUAT"))
                {
                    HoldingTranfserInitDateModel holdingTransferInitModel = new HoldingTranfserInitDateModel();
                    holdingTransferInitModel.StateCode = Convert.ToInt32(element.Descendants("statCode").FirstOrDefault().Value);
                    holdingTransferInitModel.ParentNdCode = Convert.ToInt32(element.Descendants("parentNDCode").FirstOrDefault().Value);
                    holdingTransferInitModel.startDate = element.Descendants("startDate").FirstOrDefault().Value;
                    holdingTransferInitModelList.Add(holdingTransferInitModel);

                }
                //xml Code end
                string startDate = "2023-05-23";  //"yyyy-mm-dd"
                foreach (HoldingTranfserInitDateModel item in holdingTransferInitModelList)
                {
                    if (item.ParentNdCode == PMGSYSession.Current.ParentNDCode && item.StateCode == PMGSYSession.Current.StateCode)
                    {
                        startDate = Convert.ToDateTime(item.startDate).ToString("yyyy-MM-dd");
                        break;
                    }
                }



                #endregion



                var lstBillMaster = dbContext.REAT_TRANSFER_DEDUCTION_AMT_TO_HOLDING_ACCOUNT_DETAILS(month, year, objFilter.AdminNdCode,startDate).Where(s => s.HOLDING_BILL_ID_GENERATED == null).ToList();
                if (objFilter.deductionType != "0")
                {
                    lstBillMaster = lstBillMaster.Where(s => s.HEAD_DESC == objFilter.deductionType).OrderBy(s => s.BILL_YEAR).ThenBy(m => m.BILL_MONTH).ThenBy(n => n.bill_no).ToList();
                }
                Int32? prmParentNdCode = PMGSYSession.Current.ParentNDCode;
                voucherGeneratedList = new List<string>();
                foreach (var item in lstBillMaster)
                {
                    if (dbContext.ACC_BILL_SNA_HOLDING_MAPPING.Any(s => s.SNA_BILL_ID == item.BILL_ID && s.SNA_BILL_TXN_NO == item.TXN_NO))
                    {
                        voucherGeneratedList.Add(item.BILL_ID.ToString().Trim() + "_" + item.TXN_NO.ToString().Trim() + "_" + item.DEDUCTION_AMT.ToString().Trim().Replace('.', 'A') + "_" + item.HEAD_DESC.ToString().Trim());

                    }

                }



                totalRecords = lstBillMaster.Count();


                if (objFilter.sidx.Trim() != string.Empty)
                {
                    if (objFilter.sord.ToString() == "asc")
                    {
                        switch (objFilter.sidx)
                        {
                            case "Voucher_Number":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.bill_no).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                break;
                            case "voucher_date":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.bill_date).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                break;

                            case "cheque_number":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.CHQ_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                break;
                            case "HEAD_CODE":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.HEAD_CODE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                break;

                            case "HEAD_NAME":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.HEAD_NAME).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                break;

                            case "MAST_CON_COMPANY_NAME":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.MAST_CON_COMPANY_NAME).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                break;
                            case "MAST_BANK_NAME":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.MAST_BANK_NAME).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                break;
                            case "MAST_ACCOUNT_NUMBER":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.MAST_ACCOUNT_NUMBER).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                break;
                            case "DEDUCTION_AMT":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.DEDUCTION_AMT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                break;
                            default:
                                lstBillMaster = lstBillMaster.OrderBy(x => x.BILL_ID).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                break;
                        }

                    }
                    else
                    {
                        switch (objFilter.sidx)
                        {
                            case "Voucher_Number":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.bill_no).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                break;
                            case "voucher_date":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.bill_date).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                break;

                            case "cheque_number":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.CHQ_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                break;
                            case "HEAD_CODE":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.HEAD_CODE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                break;

                            case "HEAD_NAME":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.HEAD_NAME).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                break;

                            case "MAST_CON_COMPANY_NAME":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.MAST_CON_COMPANY_NAME).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                break;
                            case "MAST_BANK_NAME":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.MAST_BANK_NAME).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                break;
                            case "MAST_ACCOUNT_NUMBER":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.MAST_ACCOUNT_NUMBER).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                break;
                            case "DEDUCTION_AMT":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.DEDUCTION_AMT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                break;
                            default:
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.BILL_ID).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstBillMaster = lstBillMaster.OrderByDescending(x => x.BILL_ID).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                }





                return lstBillMaster.Select(item => new
                {
                    id = item.BILL_ID.ToString().Trim() + "_" + item.TXN_NO.ToString().Trim() + "_" + item.DEDUCTION_AMT.ToString().Replace('.', 'A').Trim() + "_" + item.HEAD_DESC.ToString().Trim(),

                    cell = new[] {

                                        item.bill_no,
                                        commomFuncObj.GetDateTimeToString(item.bill_date),
                                      (dbContext.MASTER_MONTH.Where(s=>s.MAST_MONTH_CODE==item.BILL_MONTH).Select(m=>m.MAST_MONTH_FULL_NAME).FirstOrDefault().ToString()),
                                        item.BILL_YEAR.ToString(),
                                          item.CHQ_NO,
                                         item.HEAD_DESC.ToString(),
                                        item.HEAD_CODE+"-"+item.HEAD_NAME,
                                        item.MAST_CON_COMPANY_NAME,
                                        item.MAST_BANK_NAME.ToString(),
                                        item.MAST_ACCOUNT_NUMBER.ToString(),
                                        item.DEDUCTION_AMT.ToString(),
                                    //dbContext.ACC_BILL_SNA_HOLDING_MAPPING.Any(s=>s.SNA_BILL_ID==item.BILL_ID && s.SNA_BILL_TXN_NO==item.TXN_NO)?    
                                    //    //dbContext.ACC_BILL_MASTER.Where(s=>s.BILL_ID==s.ACC_BILL_SNA_HOLDING_MAPPING.Select(m=>m.HOLDING_BILL_ID).FirstOrDefault()).Select(m=>m.BILL_NO).FirstOrDefault().ToString():"",
                                    //   // dbContext.ACC_BILL_SNA_HOLDING_MAPPING.Where(s=>s.HOLDING_BILL_ID==s.ACC_BILL_MASTER.BILL_ID).Select(X=>X.ACC_BILL_MASTER.BILL_NO).FirstOrDefault().ToString():"",
                                    //    dbContext.ACC_BILL_MASTER.Where(s=>s.BILL_ID==(dbContext.ACC_BILL_SNA_HOLDING_MAPPING.Where(V=>V.SNA_BILL_ID==item.BILL_ID && V.SNA_BILL_TXN_NO==item.TXN_NO).Select(M=>M.HOLDING_BILL_ID).FirstOrDefault())).Select(X=>X.BILL_NO).FirstOrDefault().ToString():"",




                        }
                }).ToArray();


            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                voucherGeneratedList = null;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }

        }

        public string GetPFMSHoldingAccStatus(long billId, out string description, out string rejectionStatus)
        {
            string PFMSStatus = "-";
            description = "-";
            rejectionStatus = "-";
            try
            {
                var query = (dbContext.REAT_OMMAS_HOLDING_ACCOUNT_PAYMENTS.Where(x => x.BILL_ID == billId).FirstOrDefault());

                if (query != null)
                {

                    PFMSStatus = (string.IsNullOrEmpty(query.BANK_ACK_BILL_STATUS)
                                    ? string.IsNullOrEmpty(query.ACK_STATUS)
                                        ? "Processing at PFMS"
                                        : (query.ACK_STATUS == "A" ? "Payment accepted by PFMS" : "Payment rejected by PFMS")
                                    : (query.BANK_ACK_BILL_STATUS == "A" ? "Accepted by Bank" : "Rejected by Bank"));

                    rejectionStatus = (string.IsNullOrEmpty(query.BANK_ACK_BILL_STATUS)
                                   ? string.IsNullOrEmpty(query.ACK_STATUS)
                                       ? "Processing at PFMS"
                                       : (query.ACK_STATUS == "A" ? "Payment accepted by PFMS" : "Payment rejected by PFMS")
                                   : (query.BANK_ACK_BILL_STATUS == "A" ? "Accepted by Bank" : "Rejected by Bank"));


                    description = (string.IsNullOrEmpty(query.BANK_ACK_BILL_STATUS)
                                    ? string.IsNullOrEmpty(query.ACK_STATUS)
                                        ? "Processing at PFMS"
                                        : (query.ACK_STATUS == "A" ? "NA" : query.REJECTION_NARRATION)
                                    : (query.BANK_ACK_BILL_STATUS == "A" ? "" : query.BANK_ACK_REJECTION_NARRATION));

                }
                return PFMSStatus;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PaymentDAL.GetPFMSHoldingAccStatus()");
                return string.Empty;
            }
        }

        public Array GetTransferDeductionAmtGeneratedVoucherListDAL(PaymentFilterModel objFilter, String TransactionType, out long totalRecords)
        {
            CommonFunctions commomFuncObj = new CommonFunctions();
            string description = string.Empty;
            string rejectionStatus = string.Empty;

            dbContext = new PMGSYEntities();
            try
            {
                dbContext = new PMGSYEntities();
                //var lstBillMaster = dbContext.REAT_TRANSFER_DEDUCTION_AMT_TO_HOLDING_ACCOUNT_DETAILS(objFilter.Month, objFilter.Year, objFilter.AdminNdCode).ToList();



                // int month = System.DateTime.Now.Month;
                // int year = System.DateTime.Now.Year;
                //  var lstBillMaster = dbContext.REAT_TRANSFER_DEDUCTION_AMT_TO_HOLDING_ACCOUNT_DETAILS(objFilter.Month, objFilter.Year, objFilter.AdminNdCode).Where(s => s.HOLDING_BILL_ID_GENERATED == null).ToList();


                var lstBillMaster = (from bm in dbContext.ACC_BILL_MASTER
                                     join hld in dbContext.ACC_BILL_SNA_HOLDING_MAPPING on bm.BILL_ID equals hld.HOLDING_BILL_ID
                                     join bms in dbContext.ACC_BILL_MASTER on hld.SNA_BILL_ID equals bms.BILL_ID
                                     join pmap in dbContext.REAT_OMMAS_PAYMENT_MAPPING on hld.SNA_BILL_ID equals pmap.BILL_ID
                                     join mc in dbContext.MASTER_CONTRACTOR on bms.MAST_CON_ID equals mc.MAST_CON_ID
                                     join txn in dbContext.ACC_MASTER_TXN on bm.TXN_ID equals txn.TXN_ID
                                     where
                                      bm.ADMIN_ND_CODE == objFilter.AdminNdCode
                                      && bm.BILL_MONTH == objFilter.Month
                                      && bm.CHQ_EPAY == "E"
                                      && (bm.BILL_FINALIZED == "Y")
                                      && bm.FUND_TYPE == "P"
                                      && bm.BILL_YEAR == objFilter.Year
                                      && pmap.ACK_BILL_STATUS == "A"
                                      && pmap.BANK_ACK_BILL_STATUS == "A"
                                     select new
                                     {
                                         bm.BILL_ID,
                                         bm.BILL_NO,
                                         bm.BILL_DATE,
                                         bm.BILL_MONTH,
                                         bm.BILL_YEAR,
                                         hld.DEDUCTION_TYPE,
                                         bm.CHQ_NO,
                                         //bms.CHQ_DATE,
                                         // ContractorName = (mc.MAST_CON_FNAME == null ? "" : mc.MAST_CON_FNAME) + (mc.MAST_CON_MNAME == null ? " " : mc.MAST_CON_MNAME) + (mc.MAST_CON_LNAME == null ? " " : mc.MAST_CON_LNAME) + "(" + (mc.MAST_CON_COMPANY_NAME == null ? "" : mc.MAST_CON_COMPANY_NAME) + ")",
                                         bm.CHQ_AMOUNT,
                                         bm.CASH_AMOUNT,
                                         bm.GROSS_AMOUNT,
                                         txn.TXN_DESC,
                                         bm.BILL_FINALIZED,

                                         // MAST_CON_ID = (bm.MAST_CON_ID == null ? 0 : bm.MAST_CON_ID),
                                         // CON_ACCOUNT_ID = (bm.CON_ACCOUNT_ID == null ? 0 : bm.CON_ACCOUNT_ID),
                                         //pmap.BANK_ACK_RECEIVED_DATE,
                                     }).Distinct().ToList();

                //if (objFilter.deductionType != "0")
                //{
                //    lstBillMaster = lstBillMaster.Where(s => s.HEAD_DESC == objFilter.deductionType).OrderBy(s => s.BILL_YEAR).ThenBy(m => m.BILL_MONTH).ThenBy(n => n.bill_no).ToList();
                //}
                Int32? prmParentNdCode = PMGSYSession.Current.ParentNDCode;




                totalRecords = lstBillMaster.Count();


                if (objFilter.sidx.Trim() != string.Empty)
                {
                    if (objFilter.sord.ToString() == "asc")
                    {
                        switch (objFilter.sidx)
                        {
                            case "Voucher_Number":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.BILL_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                break;
                            case "voucher_date":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.BILL_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                break;

                            case "cheque_number":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.CHQ_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                break;
                            case "Cash_Amount":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.CASH_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                break;

                            case "Cheque_Amount":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.CHQ_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                break;

                            case "Gross_Amount":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.GROSS_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                break;
                            case "Deduction_Type":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.DEDUCTION_TYPE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                break;
                            //case "MAST_ACCOUNT_NUMBER":
                            //    lstBillMaster = lstBillMaster.OrderBy(x => x.MAST_ACCOUNT_NUMBER).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                            //    break;
                            //case "DEDUCTION_AMT":
                            //    lstBillMaster = lstBillMaster.OrderBy(x => x.DEDUCTION_AMT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                            //    break;
                            default:
                                lstBillMaster = lstBillMaster.OrderBy(x => x.BILL_ID).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                break;
                        }

                    }
                    else
                    {
                        switch (objFilter.sidx)
                        {
                            case "Voucher_Number":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.BILL_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                break;
                            case "voucher_date":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.BILL_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                break;

                            case "cheque_number":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.CHQ_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                break;
                            case "Cheque_Amount":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.CHQ_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                break;

                            case "Cash_Amount":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.CASH_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                break;

                            case "Gross_Amount":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.GROSS_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                break;
                            case "Deduction_Type":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.DEDUCTION_TYPE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                break;
                            //case "MAST_ACCOUNT_NUMBER":
                            //    lstBillMaster = lstBillMaster.OrderByDescending(x => x.MAST_ACCOUNT_NUMBER).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                            //    break;
                            //case "DEDUCTION_AMT":
                            //    lstBillMaster = lstBillMaster.OrderByDescending(x => x.DEDUCTION_AMT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                            //    break;
                            default:
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.BILL_ID).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstBillMaster = lstBillMaster.OrderByDescending(x => x.BILL_ID).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList();
                }





                return lstBillMaster.Select(item => new
                {
                    id = item.BILL_ID.ToString().Trim(),

                    cell = new[] {

                                        item.BILL_NO,
                                        commomFuncObj.GetDateTimeToString(item.BILL_DATE),
                                        item.CHQ_NO,
                                        item.DEDUCTION_TYPE==null?"":item.DEDUCTION_TYPE.ToString(),
                                        item.CHQ_AMOUNT.ToString(),
                                        item.CASH_AMOUNT.ToString(),
                                        item.GROSS_AMOUNT.ToString(),
                                      //     item.BANK_ACK_RECEIVED_DATE==null?"":Convert.ToDateTime(item.BANK_ACK_RECEIVED_DATE).ToString("dd/MM/yyyy"),
                                        item.TXN_DESC.Trim(),
                                         GetPFMSHoldingAccStatus(item.BILL_ID, out description,out rejectionStatus),
                                        description,
                                         //dbContext.ACC_BILL_SNA_HOLDING_MAPPING.Where(x => x.SNA_BILL_ID == item.BILL_ID).Any()
                                         //? "<span class='ui-icon ui-icon-locked ui-align-center'></span>"
                                         //:(item.CASH_AMOUNT > 0 && GetPFMSStatus(item.BILL_ID, out description) == "Accepted by Bank")
                                         //   ? "<center><table><tr><td  style='border-color:white'><span class='ui-icon ui-icon-arrowthick-1-e' title='Click here to transfer deduction amount to holding account' onClick ='TransferDeductionAmt(" + item.BILL_ID + ");' ></span></td></tr></table></center>"
                                         //   : "-"
                                        //   (
                                        //    item.BILL_FINALIZED=="Y"  && item.CHQ_AMOUNT > 0 && item.CHQ_NO != null
                                        //    && !(dbContext.ACC_CANCELLED_CHEQUES.Where(x=>x.OLD_BILL_ID == item.BILL_ID).Any())
                                        //    &&(PMGSYSession.Current.LevelId==4 && PMGSYSession.Current.FundType=="A" )?dbContext.ACC_CHEQUES_ISSUED.Where(x=>x.BILL_ID== item.BILL_ID && x.CHEQUE_STATUS=="N").Any():(PMGSYSession.Current.LevelId==4?true:dbContext.ACC_CHEQUES_ISSUED.Where(x=>x.BILL_ID== item.BILL_ID && x.CHEQUE_STATUS=="N").Any())
                                        //  && (description=="Payment rejected by PFMS" || description=="Rejected by Bank")
                                        //)
                                        (rejectionStatus=="Payment rejected by PFMS" || rejectionStatus=="Rejected by Bank")
                                        ? "<center><a href='#' class='ui-icon ui-icon ui-icon-refresh' onclick='RenewEpaymentHoldingAcc(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() + "$"+ item.CHQ_NO})+ "\");return                                                           false;'>Reinitiate Epayment</a></center>"
                                        : String.Empty,
                        }
                }).ToArray();


            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                // voucherGeneratedList = null;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }

        }

        public Boolean SubmitTransferDeductionAmountDAL(string[] billIdArray, string billNo, DateTime billDate, string chq_No, decimal deductionAmt)
        {

            try
            {
                using (var scope = new TransactionScope())
                {

                    dbContext = new PMGSYEntities();
                    ACC_BILL_MASTER masterModel = new ACC_BILL_MASTER();
                    ACC_BILL_MASTER masterDetails = new ACC_BILL_MASTER();
                    ACC_BILL_DETAILS billDetailsC = new ACC_BILL_DETAILS();
                    ACC_BILL_DETAILS billDetailsD = new ACC_BILL_DETAILS();

                    ACC_CHEQUES_ISSUED chequeIssuedModel = new ACC_CHEQUES_ISSUED();
                    short TransferBillMonth = Convert.ToInt16(billDate.Month);
                    short TransferBillYear = Convert.ToInt16(billDate.Year);

                    //  masterModel = dbContext.ACC_BILL_MASTER.Where(x => x.BILL_ID == billId).FirstOrDefault();

                    Int64 maxBillId = 0;

                    //if (dbContext.ACC_BILL_MASTER.Any())
                    //{
                    //    maxBillId = dbContext.ACC_BILL_MASTER.Max(c => c.BILL_ID);
                    //}

                    //maxBillId = maxBillId + 1;

                    maxBillId = dbContext.ACC_BILL_MASTER.Any() ? (from item in dbContext.ACC_BILL_MASTER select item.BILL_ID).Max() + 1 : 1;

                    masterDetails.BILL_ID = maxBillId;
                    masterDetails.BILL_NO = billNo;
                    masterDetails.BILL_MONTH = Convert.ToInt16(billDate.Month);
                    masterDetails.BILL_YEAR = Convert.ToInt16(billDate.Year);
                    masterDetails.BILL_DATE = billDate;
                    masterDetails.TXN_ID = 3171;
                    masterDetails.CHQ_Book_ID = null;
                    masterDetails.CHQ_NO = chq_No;
                    masterDetails.CHQ_DATE = billDate;
                    masterDetails.CHQ_AMOUNT = deductionAmt;
                    masterDetails.CASH_AMOUNT = 0;
                    masterDetails.GROSS_AMOUNT = deductionAmt;
                    masterDetails.CHALAN_NO = null;
                    masterDetails.CHALAN_DATE = null;
                    masterDetails.PAYEE_NAME = "Auto transfer of deduction amount from SNA account to holding account";
                    masterDetails.CHQ_EPAY = "E";
                    masterDetails.TEO_TRANSFER_TYPE = null;
                    masterDetails.REMIT_TYPE = null;
                    masterDetails.BILL_FINALIZED = "Y";
                    masterDetails.FUND_TYPE = "P";
                    masterDetails.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                    masterDetails.LVL_ID = 5;
                    masterDetails.MAST_CON_ID = null;
                    masterDetails.BILL_TYPE = "P";
                    masterDetails.ACTION_REQUIRED = null;
                    masterDetails.USERID = PMGSYSession.Current.UserId;
                    masterDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    masterDetails.ADMIN_NO_OFFICER_CODE = null;
                    masterDetails.AUTH_ID = null;
                    masterDetails.CON_ACCOUNT_ID = null;

                    dbContext.ACC_BILL_MASTER.Add(masterDetails);
                    dbContext.SaveChanges();

                    short headIdCredit = dbContext.ACC_TXN_HEAD_MAPPING.Where(m => m.TXN_ID == 3172 && m.CREDIT_DEBIT == "C" && m.CASH_CHQ == "Q").Select(m => m.HEAD_ID).FirstOrDefault();
                    short headIdDebit = dbContext.ACC_TXN_HEAD_MAPPING.Where(m => m.TXN_ID == 3172 && m.CREDIT_DEBIT == "D").Select(m => m.HEAD_ID).FirstOrDefault();

                    billDetailsC.BILL_ID = maxBillId;
                    billDetailsC.TXN_NO = 1;
                    billDetailsC.TXN_ID = 3172;
                    billDetailsC.HEAD_ID = headIdCredit;
                    billDetailsC.AMOUNT = deductionAmt;
                    billDetailsC.CREDIT_DEBIT = "C";
                    billDetailsC.CASH_CHQ = "Q";
                    billDetailsC.NARRATION = "Auto transfer of deduction amount from SNA account to holding account";
                    billDetailsC.USERID = PMGSYSession.Current.UserId;
                    billDetailsC.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    dbContext.ACC_BILL_DETAILS.Add(billDetailsC);
                    dbContext.SaveChanges();

                    billDetailsD.BILL_ID = maxBillId;
                    billDetailsD.TXN_NO = 2;
                    billDetailsD.TXN_ID = 3172;
                    billDetailsD.HEAD_ID = headIdDebit;
                    billDetailsD.AMOUNT = deductionAmt;
                    billDetailsD.CREDIT_DEBIT = "D";
                    billDetailsD.CASH_CHQ = "Q";
                    billDetailsD.NARRATION = "Auto transfer of deduction amount from SNA account to holding account";
                    billDetailsD.USERID = PMGSYSession.Current.UserId;
                    billDetailsD.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    dbContext.ACC_BILL_DETAILS.Add(billDetailsD);
                    dbContext.SaveChanges();

                    long maxTRANSFER_MAP_ID = dbContext.ACC_BILL_SNA_HOLDING_MAPPING.Any() ? (from item in dbContext.ACC_BILL_SNA_HOLDING_MAPPING select item.ACC_SNA_HOLDING_TRANSFER_MAP_ID).Max() + 1 : 1;
                    for (int i = 0; i < billIdArray.Length; i++)
                    {
                        ACC_BILL_SNA_HOLDING_MAPPING holdingModel = new ACC_BILL_SNA_HOLDING_MAPPING();
                        long snaBillId = Convert.ToInt32(billIdArray[i].Split('_')[0]);
                        short TxnNo = Convert.ToInt16(billIdArray[i].Split('_')[1]);
                        string deductionType = billIdArray[i].Split('_')[3].ToString();
                        holdingModel.ACC_SNA_HOLDING_TRANSFER_MAP_ID = maxTRANSFER_MAP_ID;
                        holdingModel.SNA_BILL_ID = snaBillId;
                        holdingModel.HOLDING_BILL_ID = maxBillId;
                        holdingModel.SNA_BILL_TXN_NO = TxnNo;
                        holdingModel.DEDUCTION_TYPE = deductionType;
                        dbContext.ACC_BILL_SNA_HOLDING_MAPPING.Add(holdingModel);
                        dbContext.SaveChanges();
                        maxTRANSFER_MAP_ID++;

                    }






                    short bankCode = 0;

                    if (PMGSYSession.Current.LevelId == 5)
                    {
                        int? parentNdCode = dbContext.ADMIN_DEPARTMENT.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Select(x => x.MAST_PARENT_ND_CODE).First();

                        if (parentNdCode.HasValue)
                        {
                            bankCode = dbContext.ACC_BANK_DETAILS.Where(c => c.ADMIN_ND_CODE == parentNdCode && c.BANK_ACC_STATUS == true && c.FUND_TYPE == PMGSYSession.Current.FundType && c.ACCOUNT_TYPE == "H").Select(x => x.BANK_CODE).FirstOrDefault();
                        }
                    }
                    chequeIssuedModel.BILL_ID = maxBillId;
                    chequeIssuedModel.BANK_CODE = bankCode;
                    chequeIssuedModel.IS_CHQ_ENCASHED_NA = false;
                    chequeIssuedModel.NA_BILL_ID = null;
                    chequeIssuedModel.IS_CHQ_RECONCILE_BANK = false;
                    chequeIssuedModel.CHQ_RECONCILE_DATE = null;
                    chequeIssuedModel.CHQ_RECONCILE_REMARKS = null;
                    chequeIssuedModel.CHEQUE_STATUS = "N";
                    chequeIssuedModel.USERID = PMGSYSession.Current.UserId;
                    chequeIssuedModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    dbContext.ACC_CHEQUES_ISSUED.Add(chequeIssuedModel);
                    dbContext.SaveChanges();

                    int fiscalYear = 0;
                    if (TransferBillMonth <= 3)
                    {
                        fiscalYear = (TransferBillYear - 1);
                    }
                    else
                    {
                        fiscalYear = TransferBillYear;
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
                    dbContext.SaveChanges();
                    scope.Complete();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error while transfering amount.");
            }
            finally
            {
                dbContext.Dispose();
            }
        }




        #endregion
        public EpaymentOrderModel GetEpaymentDetails1(Int64 bill_id, int conAccountId)
        {


            CommonFunctions commomFuncObj = new CommonFunctions();
            AmountToWord objAmt = new AmountToWord();
            try
            {
                EpaymentOrderModel model = new EpaymentOrderModel();
                dbContext = new PMGSYEntities();

                //Added By Abhishek for Reject/Resend Epayment 26 Sep 2014 start
                int _AdminNdCode = 0;
                int _ParendAdminNdCode = 0;
                int _DistrictCode = 0;

                if (PMGSYSession.Current.DistrictCode == 0)
                {
                    _AdminNdCode = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == bill_id).Select(s => s.ADMIN_ND_CODE).FirstOrDefault();
                    ADMIN_DEPARTMENT AdminDeptModel = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == _AdminNdCode).FirstOrDefault();
                    _ParendAdminNdCode = AdminDeptModel.MAST_PARENT_ND_CODE.Value;
                    //_DistrictCode = AdminDeptModel.MAST_DISTRICT_CODE.Value;
                    //Added on 18-11-2021
                    _DistrictCode = PMGSYSession.Current.LevelId != 4 ? AdminDeptModel.MAST_DISTRICT_CODE.Value : _DistrictCode;
                }
                else
                {
                    _AdminNdCode = PMGSYSession.Current.AdminNdCode;
                    _ParendAdminNdCode = PMGSYSession.Current.ParentNDCode.Value;
                    _DistrictCode = PMGSYSession.Current.DistrictCode;
                }
                //Added By Abhishek for Reject/Resend Epayment 26 Sep 2014 end

                //get state /district information
                MASTER_DISTRICT district = new MASTER_DISTRICT();
                district = PMGSYSession.Current.LevelId == 4 ? null : dbContext.MASTER_DISTRICT.Where(v => v.MAST_DISTRICT_CODE == _DistrictCode).First();
                // int stateCode = PMGSYSession.Current.StateCode;
                //Added on 18-11-2021
                int stateCode = stateCode = PMGSYSession.Current.LevelId == 4 ? PMGSYSession.Current.StateCode : district.MAST_STATE_CODE;

                //get payment master details
                ACC_BILL_MASTER masterDetails = new ACC_BILL_MASTER();
                masterDetails = dbContext.ACC_BILL_MASTER.Where(x => x.BILL_ID == bill_id && x.CHQ_EPAY == "E").First();

                //Added by Abhishek kamble 1-Jul-2014 to show/hide package row from dialog box
                //model.EpayMasterTxnID = masterDetails.TXN_ID;

                //get all the unique  package id based on the roads
                List<Int32> lstBillDetails = new List<Int32>();
                lstBillDetails = dbContext.ACC_BILL_DETAILS.Where(c => c.BILL_ID == bill_id && c.CASH_CHQ == "Q" && c.CREDIT_DEBIT == "D" && c.IMS_PR_ROAD_CODE != null).
                    Select(f => f.IMS_PR_ROAD_CODE.Value).Distinct().ToList<Int32>();

                List<Int32> lstAgreement = new List<Int32>();
                lstAgreement = dbContext.ACC_BILL_DETAILS.Where(c => c.BILL_ID == bill_id && c.CASH_CHQ == "Q" && c.CREDIT_DEBIT == "D" && c.IMS_AGREEMENT_CODE != null).Select(F => F.IMS_AGREEMENT_CODE.Value).Distinct().ToList<Int32>();

                String Packages = String.Empty;
                String strAgreement = String.Empty;

                if (lstBillDetails.Count != 0)
                {

                    foreach (int item in lstBillDetails)
                    {
                        string newPackage = dbContext.IMS_SANCTIONED_PROJECTS.Where(c => c.IMS_PR_ROAD_CODE == item).Select(x => x.IMS_PACKAGE_ID).First();
                        //if new package is not allready present in Packages then only add
                        //if (!Packages.Contains(newPackage))
                        //{
                        Packages = Packages + "," + newPackage;
                        //}

                    }

                    if (Packages != string.Empty)
                    {
                        if (Packages[0] == ',')
                        {
                            Packages = Packages.Substring(1);
                        }

                    }
                }

                if (lstAgreement.Count != 0)
                {
                    if (PMGSYSession.Current.FundType.Equals("P"))
                    {
                        foreach (int item in lstAgreement)
                        {
                            string newAgreement = dbContext.TEND_AGREEMENT_MASTER.Where(c => c.TEND_AGREEMENT_CODE == item).Select(x => x.TEND_AGREEMENT_NUMBER).First();
                            //if new package is not allready present in Packages then only add
                            //if (!strAgreement.Contains(newAgreement))
                            //{
                            strAgreement = strAgreement + "," + newAgreement;
                            //}
                        }
                        if (strAgreement != string.Empty)
                        {
                            if (strAgreement[0] == ',')
                            {
                                strAgreement = strAgreement.Substring(1);
                            }
                        }
                    }
                    else if (PMGSYSession.Current.FundType.Equals("M"))
                    {
                        foreach (int item in lstAgreement)
                        {
                            string newAgreement = dbContext.MANE_IMS_CONTRACT.Where(c => c.MANE_CONTRACT_ID == item).Select(x => x.MANE_AGREEMENT_NUMBER).First();
                            //if new package is not allready present in Packages then only add
                            if (!strAgreement.Contains(newAgreement))
                            {
                                strAgreement = strAgreement + "," + newAgreement;
                            }
                        }
                        if (strAgreement != string.Empty)
                        {
                            if (strAgreement[0] == ',')
                            {
                                strAgreement = strAgreement.Substring(1);
                            }
                        }
                    }

                }



                string StateName = dbContext.MASTER_STATE.Where(v => v.MAST_STATE_CODE == stateCode).Select(c => c.MAST_STATE_NAME).First();


                //get Contractor details 
                // MASTER_CONTRACTOR_BANK con = new MASTER_CONTRACTOR_BANK();
                //con = dbContext.MASTER_CONTRACTOR_BANK.Where(v => v.MAST_CON_ID == masterDetails.MAST_CON_ID && v.MAST_ACCOUNT_STATUS == "A").FirstOrDefault();

                //Modified By Abhishek kamble 21-July-2014 start
                //get contractor details
                MASTER_CONTRACTOR_BANK con = new MASTER_CONTRACTOR_BANK();
                ADMIN_NO_BANK noOffBank = new ADMIN_NO_BANK();
                // if (PMGSYSession.Current.FundType != "A")
                //{
                //if (masterDetails.TXN_ID == 472 || masterDetails.TXN_ID == 415)
                //{
                //    noOffBank = dbContext.ADMIN_NO_BANK.Where(v => v.ADMIN_NO_OFFICER_CODE == masterDetails.ADMIN_NO_OFFICER_CODE && v.MAST_ACCOUNT_STATUS == "A").FirstOrDefault();
                //}
                //else
                //{
                //if (PMGSYSession.Current.FundType == "M")
                //{
                //    con = dbContext.MASTER_CONTRACTOR_BANK.Where(v => v.MAST_CON_ID == masterDetails.MAST_CON_ID && v.MAST_ACCOUNT_STATUS == "A" && v.MAST_DISTRICT_CODE ==
                //        _DistrictCode).FirstOrDefault();
                //    if (con == null)
                //    {
                //        if (PMGSYSession.Current.StateCode > 0)
                //        {
                //            var districts = dbContext.MASTER_DISTRICT.Where(x => x.MAST_STATE_CODE == PMGSYSession.Current.StateCode && x.MAST_DISTRICT_ACTIVE == "Y").Select(x => x.MAST_DISTRICT_CODE).ToList();
                //            if (districts != null)
                //            {
                //                if (dbContext.MASTER_CONTRACTOR_BANK.Any(v => v.MAST_CON_ID == masterDetails.MAST_CON_ID && v.MAST_ACCOUNT_STATUS == "A"))
                //                {
                //                    con = dbContext.MASTER_CONTRACTOR_BANK.Where(v => v.MAST_CON_ID == masterDetails.MAST_CON_ID && v.MAST_ACCOUNT_STATUS == "A"
                //                        //&& v.MASTER_CONTRACTOR.MASTER_CONTRACTOR_REGISTRATION.Where(x => x.MAST_CON_ID == contractorId).Select(x => x.MAST_REG_STATE).FirstOrDefault() == PMGSYSession.Current.StateCode
                //                        && districts.Contains(v.MAST_DISTRICT_CODE)
                //                        ).FirstOrDefault();
                //                }
                //                if (con == null)
                //                {
                //                    con = dbContext.MASTER_CONTRACTOR_BANK.Where(v => v.MAST_CON_ID == masterDetails.MAST_CON_ID && v.MAST_ACCOUNT_STATUS == "A").FirstOrDefault();
                //                }
                //            }
                //            else
                //            {
                //                con = dbContext.MASTER_CONTRACTOR_BANK.Where(v => v.MAST_CON_ID == masterDetails.MAST_CON_ID && v.MAST_ACCOUNT_STATUS == "A").FirstOrDefault();
                //            }
                //        }
                //        else
                //        {
                //            con = dbContext.MASTER_CONTRACTOR_BANK.Where(v => v.MAST_CON_ID == masterDetails.MAST_CON_ID && v.MAST_ACCOUNT_STATUS == "A").FirstOrDefault();
                //        }
                //    }
                //}

                //Below Line Commented on 25-10-2021 to Get bank details for Admin Fund 
                //if (PMGSYSession.Current.FundType == "P" || PMGSYSession.Current.FundType == "M")

                //Below Line Added on 25-10-2021 to Get bank details for Admin Fund 
                if (PMGSYSession.Current.FundType == "P" || PMGSYSession.Current.FundType == "M" || PMGSYSession.Current.FundType == "A")
                {
                    PFMS_OMMAS_CONTRACTOR_MAPPING pfmsCon = new PFMS_OMMAS_CONTRACTOR_MAPPING();
                    int lgdStateCode = dbContext.OMMAS_LDG_STATE_MAPPING.Where(x => x.MAST_STATE_CODE == PMGSYSession.Current.StateCode).Select(x => x.MAST_STATE_LDG_CODE).FirstOrDefault();

                    /*int lgdDistrictCode = 0;
                    lgdDistrictCode = dbContext.OMMAS_LDG_DISTRICT_MAPPING.Where(x => x.MAST_DISTRICT_CODE == PMGSYSession.Current.DistrictCode).Select(x => x.MAST_DISTRICT_LDG_CODE).FirstOrDefault();
                    pfmsCon = dbContext.PFMS_OMMAS_CONTRACTOR_MAPPING.Where(v => v.MAST_CON_ID == masterDetails.MAST_CON_ID && v.PFMS_CON_ID != null && v.MAST_LGD_DISTRICT_CODE == lgdDistrictCode).FirstOrDefault();
                    if (pfmsCon == null)
                    {
                        if (PMGSYSession.Current.StateCode > 0)
                        {
                            var districts = dbContext.MASTER_DISTRICT.Where(x => x.MAST_STATE_CODE == PMGSYSession.Current.StateCode && x.MAST_DISTRICT_ACTIVE == "Y").Select(x => x.OMMAS_LDG_DISTRICT_MAPPING.MAST_DISTRICT_LDG_CODE).ToList();
                            if (districts != null)
                            {
                                if (dbContext.PFMS_OMMAS_CONTRACTOR_MAPPING.Any(v => v.MAST_CON_ID == masterDetails.MAST_CON_ID && v.PFMS_CON_ID != null))
                                {
                                    pfmsCon = dbContext.PFMS_OMMAS_CONTRACTOR_MAPPING.Where(v => v.MAST_CON_ID == masterDetails.MAST_CON_ID && v.PFMS_CON_ID != null
                                        //&& v.MASTER_CONTRACTOR.MASTER_CONTRACTOR_REGISTRATION.Where(x => x.MAST_CON_ID == contractorId).Select(x => x.MAST_REG_STATE).FirstOrDefault() == PMGSYSession.Current.StateCode
                                        && districts.Contains(v.MAST_LGD_DISTRICT_CODE.Value)
                                        ).FirstOrDefault();
                                }
                                if (pfmsCon == null)
                                {
                                    pfmsCon = dbContext.PFMS_OMMAS_CONTRACTOR_MAPPING.Where(v => v.MAST_CON_ID == masterDetails.MAST_CON_ID && v.PFMS_CON_ID != null).FirstOrDefault();
                                }
                            }
                            else
                            {
                                pfmsCon = dbContext.PFMS_OMMAS_CONTRACTOR_MAPPING.Where(v => v.MAST_CON_ID == masterDetails.MAST_CON_ID && v.PFMS_CON_ID != null).FirstOrDefault();
                            }
                        }
                        else
                        {
                            pfmsCon = dbContext.PFMS_OMMAS_CONTRACTOR_MAPPING.Where(v => v.MAST_CON_ID == masterDetails.MAST_CON_ID && v.PFMS_CON_ID != null).FirstOrDefault();
                        }
                    }*/
                    MASTER_CONTRACTOR_BANK objbank = new MASTER_CONTRACTOR_BANK();
                    objbank = dbContext.MASTER_CONTRACTOR_BANK.Where(v => v.MAST_CON_ID == masterDetails.MAST_CON_ID && v.MAST_ACCOUNT_ID == conAccountId).FirstOrDefault();
                    {
                        con.MAST_ACCOUNT_NUMBER = objbank.MAST_ACCOUNT_NUMBER;
                        con.MAST_ACCOUNT_ID = objbank.MAST_ACCOUNT_ID;
                        con.MAST_IFSC_CODE = objbank.MAST_IFSC_CODE;
                        con.MAST_BANK_NAME = objbank.MAST_BANK_NAME;

                    }



                    //pfmsCon = dbContext.PFMS_OMMAS_CONTRACTOR_MAPPING.Where(v => v.MAST_CON_ID == masterDetails.MAST_CON_ID && v.PFMS_CON_ID != null && v.MAST_ACCOUNT_ID == conAccountId && v.MAST_LGD_STATE_CODE == lgdStateCode && v.MAST_AGENCY_CODE == PMGSYSession.Current.MastAgencyCode && v.STATUS == "A" && v.MASTER_CONTRACTOR.MASTER_CONTRACTOR_BANK.Where(z => z.MAST_CON_ID == masterDetails.MAST_CON_ID && z.MAST_ACCOUNT_ID == conAccountId && z.MAST_LOCK_STATUS == "Y").Select(x => x.MAST_ACCOUNT_STATUS).FirstOrDefault() == "A").FirstOrDefault();
                    //if (pfmsCon != null)
                    //{
                    //    con.MAST_ACCOUNT_NUMBER = pfmsCon.MAST_ACCOUNT_NUMBER;
                    //    con.MAST_ACCOUNT_ID = pfmsCon.MAST_ACCOUNT_ID;
                    //    con.MAST_IFSC_CODE = pfmsCon.MAST_IFSC_CODE;
                    //    con.MAST_BANK_NAME = pfmsCon.MAST_BANK_NAME;
                    //}
                    //if (pfmsCon == null)
                    //{
                    //    model.IsAccountInactive = dbContext.PFMS_OMMAS_CONTRACTOR_MAPPING.Any(v => v.MAST_CON_ID == masterDetails.MAST_CON_ID && v.PFMS_CON_ID != null && v.MAST_ACCOUNT_ID == conAccountId && v.MAST_LGD_STATE_CODE == lgdStateCode && v.MAST_AGENCY_CODE == PMGSYSession.Current.MastAgencyCode && v.STATUS == "A" && v.MASTER_CONTRACTOR.MASTER_CONTRACTOR_BANK.Any(z => z.MAST_CON_ID == masterDetails.MAST_CON_ID && z.MAST_ACCOUNT_ID == conAccountId && (z.MAST_LOCK_STATUS == "N" || z.MAST_ACCOUNT_STATUS == "I")));

                    //    if (model.IsAccountInactive == false)
                    //    {
                    //        if (!(dbContext.PFMS_OMMAS_CONTRACTOR_MAPPING.Any(v => v.MAST_CON_ID == masterDetails.MAST_CON_ID && v.PFMS_CON_ID != null && v.MAST_ACCOUNT_ID == conAccountId && v.MAST_LGD_STATE_CODE == lgdStateCode && v.MAST_AGENCY_CODE == PMGSYSession.Current.MastAgencyCode && v.STATUS == "A" && v.MASTER_CONTRACTOR.MASTER_CONTRACTOR_BANK.Any(z => z.MAST_CON_ID == masterDetails.MAST_CON_ID && z.MAST_ACCOUNT_ID == conAccountId))))
                    //            model.IsConAgency = true;
                    //    }
                    //}


                    //string moduleType = "D";
                    //REAT_OMMAS_PAYMENT_SUCCESS objModuleType = dbContext.REAT_OMMAS_PAYMENT_SUCCESS.FirstOrDefault(s => s.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode);
                    //if (objModuleType != null)
                    //{
                    //    moduleType = "R";
                    //}
                    //if (moduleType.Equals("R"))
                    //{
                    //    REAT_CONTRACTOR_DETAILS reatcon = dbContext.REAT_CONTRACTOR_DETAILS.Where(v => v.MAST_CON_ID == masterDetails.MAST_CON_ID && v.MAST_ACCOUNT_ID == conAccountId && v.reat_STATUS == "A" && v.REAT_CON_ID != null).FirstOrDefault();
                    //    if (reatcon != null)
                    //    {
                    //        con.MAST_ACCOUNT_NUMBER = reatcon.MAST_ACCOUNT_NUMBER;
                    //        con.MAST_ACCOUNT_ID = reatcon.MAST_ACCOUNT_ID;
                    //        con.MAST_IFSC_CODE = reatcon.MAST_IFSC_CODE;
                    //        con.MAST_BANK_NAME = reatcon.MAST_BANK_NAME;
                    //    }
                    //}


                }
                //}
                //}

                //Modified By Abhishek kamble 21-July-2014 end


                //get Epayment bank details

                ACC_BANK_DETAILS bakDetails = new ACC_BANK_DETAILS();
                int? parentNdVode = dbContext.ADMIN_DEPARTMENT.Where(b => b.ADMIN_ND_CODE == _AdminNdCode).Select(d => d.MAST_PARENT_ND_CODE).First();

                if (parentNdVode.HasValue)
                {
                    //Below Line Commented on 10-12-2021
                    //bakDetails = dbContext.ACC_BANK_DETAILS.Where(f => f.ADMIN_ND_CODE == parentNdVode.Value && f.FUND_TYPE == "P" && f.BANK_ACC_STATUS == true).First();

                    //Below condition added on 10-05-2023
                    if (masterDetails.TXN_ID == 3187)
                    {
                        bakDetails = dbContext.ACC_BANK_DETAILS.Where(f => f.ADMIN_ND_CODE == parentNdVode.Value && f.FUND_TYPE == "P" && f.BANK_ACC_STATUS == true && f.ACCOUNT_TYPE == "D").First();
                    }
                    else
                    {
                        //Below Line Added on 10-12-2021
                        //***--***
                        bakDetails = ((masterDetails.TXN_ID == 3050 || masterDetails.TXN_ID == 3051) && PMGSYSession.Current.FundType == "M") || (PMGSYSession.Current.FundType != "M") ?
                            dbContext.ACC_BANK_DETAILS.Where(f => f.ADMIN_ND_CODE == parentNdVode.Value && f.FUND_TYPE == "P" && f.BANK_ACC_STATUS == true && f.ACCOUNT_TYPE == "S").First() :
                            dbContext.ACC_BANK_DETAILS.Where(f => f.ADMIN_ND_CODE == parentNdVode.Value && f.FUND_TYPE == "M" && f.BANK_ACC_STATUS == true && f.ACCOUNT_TYPE == "S").First();
                    }

                }

                model.AgreementNumber = strAgreement;
                model.EmailRecepient = bakDetails.BANK_EMAIL; //dbContext.ADMIN_NODAL_OFFICERS.Where(a => a.ADMIN_ND_CODE == parentNdVode.Value && a.ADMIN_ACTIVE_STATUS == "Y").Select(f => f.ADMIN_NO_EMAIL).First();
                //model.EmailRecepient = "famshelp@gmail.com";
                model.EmailBCC = ConfigurationManager.AppSettings["EpayBCC"].ToString();
                //new change done by Vikram on 29 Jan 2014
                //if (dbContext.ACC_EPAY_MAIL_OTHER.Any(m => m.ADMIN_ND_CODE == PMGSYSession.Current.ParentNDCode))
                if (dbContext.ACC_EPAY_MAIL_OTHER.Where(m => m.ADMIN_ND_CODE == _ParendAdminNdCode).Any())
                {
                    model.EmailCC = dbContext.ACC_EPAY_MAIL_OTHER.Where(m => m.ADMIN_ND_CODE == _ParendAdminNdCode).Select(m => m.EMAIL_CC).FirstOrDefault();
                    //model.EmailCC = "omms@cdac.in";
                    model.SrrdaPassword = dbContext.ACC_EPAY_MAIL_OTHER.Where(m => m.ADMIN_ND_CODE == _ParendAdminNdCode).Select(m => m.PDF_OPEN_KEY).FirstOrDefault();
                }
                else
                {
                    model.EmailCC = "";
                    model.SrrdaPassword = "";
                }
                // model.DPIUName = district.MAST_DISTRICT_NAME;
                //below Line Added on 18-11-2021
                model.DPIUName = PMGSYSession.Current.LevelId == 4 ? "" : district.MAST_DISTRICT_NAME.ToString();
                model.STATEName = StateName;
                if (PMGSYSession.Current.RoleCode == 26)
                {
                    model.EmailDate = commomFuncObj.GetDateTimeToString(DateTime.Now);
                }
                else
                {
                    model.EmailDate = "";
                }
                //Commented By Abhishek kamble 19-mar-2014
                //model.Bankaddress = bakDetails.BANK_ADDRESS1 == null ? String.Empty : bakDetails.BANK_ADDRESS1;
                model.Bankaddress = (bakDetails.BANK_NAME == null ? String.Empty : bakDetails.BANK_NAME) + (bakDetails.BANK_ADDRESS1 == null ? String.Empty : "-" + bakDetails.BANK_ADDRESS1) + (bakDetails.BANK_ADDRESS2 == null ? String.Empty : ", " + bakDetails.BANK_ADDRESS2) + ", " + (bakDetails.MASTER_STATE.MAST_STATE_NAME);

                model.BankAcNumber = bakDetails.BANK_ACC_NO == null ? String.Empty : bakDetails.BANK_ACC_NO;

                //Below line added on 10-05-2023
                model.BankIFSCCode = bakDetails.MAST_IFSC_CODE == null ? String.Empty : bakDetails.MAST_IFSC_CODE;

                model.EpayNumber = masterDetails.CHQ_NO == null ? String.Empty : masterDetails.CHQ_NO;
                if (masterDetails.CHQ_DATE.HasValue)
                {
                    model.EpayDate = commomFuncObj.GetDateTimeToString(masterDetails.CHQ_DATE.Value);
                }
                else
                {
                    model.EpayDate = "";
                }

                model.EpayState = StateName;
                //model.EpayDPIU = district.MAST_DISTRICT_NAME;
                //Modified by Abhishek kamble to set DPIU name old it is dist name. 25-June-2014
                model.EpayDPIU = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == _AdminNdCode).Select(s => s.ADMIN_ND_NAME).FirstOrDefault();

                model.EpayVNumber = masterDetails.BILL_NO;
                model.EpayVDate = commomFuncObj.GetDateTimeToString(masterDetails.BILL_DATE);
                model.EpayVPackages = Packages;



                //Added By Abhishek kamble 28-May-2014 start
                MASTER_CONTRACTOR mastContDetailsModel = new MASTER_CONTRACTOR();
                ADMIN_NODAL_OFFICERS adminNoModel = new ADMIN_NODAL_OFFICERS();
                // string conName = string.Empty;
                string contNameContCompanyName = string.Empty;
                //if (masterDetails.TXN_ID == 472 || masterDetails.TXN_ID == 415)
                //{
                //    adminNoModel = dbContext.ADMIN_NODAL_OFFICERS.Where(c => c.ADMIN_NO_OFFICER_CODE == masterDetails.ADMIN_NO_OFFICER_CODE && c.ADMIN_MODULE == "A" && c.ADMIN_ACTIVE_STATUS == "Y").FirstOrDefault();
                //    if (!(adminNoModel == null))
                //    {
                //        contNameContCompanyName = adminNoModel.ADMIN_NO_FNAME + " " + adminNoModel.ADMIN_NO_MNAME + " " + adminNoModel.ADMIN_NO_LNAME;
                //        model.EpayConName = contNameContCompanyName;
                //    }
                //    else
                //    {
                //        model.EpayConName = "";
                //    }
                //}
                //else
                //{
                mastContDetailsModel = dbContext.MASTER_CONTRACTOR.Where(c => c.MAST_CON_ID == masterDetails.MAST_CON_ID).FirstOrDefault();
                if (mastContDetailsModel.MAST_CON_COMPANY_NAME != null)
                {
                    contNameContCompanyName = mastContDetailsModel.MAST_CON_FNAME + " " + mastContDetailsModel.MAST_CON_MNAME + " " + mastContDetailsModel.MAST_CON_LNAME + " ( " + mastContDetailsModel.MAST_CON_COMPANY_NAME + " )";
                }
                else
                {
                    contNameContCompanyName = mastContDetailsModel.MAST_CON_FNAME + " " + mastContDetailsModel.MAST_CON_MNAME + " " + mastContDetailsModel.MAST_CON_LNAME;
                }
                model.EpayConName = contNameContCompanyName;

                //Contractor legal heir details
                //Added By Abhishek kamble 28-May-2014 end

                if (mastContDetailsModel.MAST_CON_EXPIRY_DATE != null && mastContDetailsModel.MAST_CON_STATUS == "E")
                {
                    model.EpayContLegalHeirName = mastContDetailsModel.MAST_CON_LEGAL_HEIR_FNAME + " " + mastContDetailsModel.MAST_CON_LEGAL_HEIR_MNAME + " " + mastContDetailsModel.MAST_CON_LEGAL_HEIR_LNAME;
                }
                //}


                //if (masterDetails.TXN_ID == 472 || masterDetails.TXN_ID == 415)
                //{

                //    if (noOffBank != null)
                //    {
                //        model.EpayConAcNum = noOffBank.MAST_ACCOUNT_NUMBER == null ? String.Empty : noOffBank.MAST_ACCOUNT_NUMBER;
                //        model.EpayConBankName = noOffBank.MAST_BANK_NAME == null ? String.Empty : noOffBank.MAST_BANK_NAME;
                //        model.EpayConBankIFSCCode = noOffBank.MAST_IFSC_CODE == null ? String.Empty : noOffBank.MAST_IFSC_CODE;
                //    }
                //    else
                //    {
                //        model.EpayConAcNum = String.Empty;
                //        model.EpayConBankName = String.Empty;
                //        model.EpayConBankIFSCCode = String.Empty;
                //    }
                //}
                //else
                //{

                if (con != null)
                {
                    model.EpayConAcNum = con.MAST_ACCOUNT_NUMBER == null ? String.Empty : con.MAST_ACCOUNT_NUMBER;
                    model.EpayConBankName = con.MAST_BANK_NAME == null ? String.Empty : con.MAST_BANK_NAME;
                    model.EpayConBankIFSCCode = con.MAST_IFSC_CODE == null ? String.Empty : con.MAST_IFSC_CODE;
                }
                else
                {
                    model.EpayConAcNum = String.Empty;
                    model.EpayConBankName = String.Empty;
                    model.EpayConBankIFSCCode = String.Empty;
                }
                //}

                model.EpayAmount = masterDetails.CHQ_AMOUNT.ToString();
                model.EpayAmountInWord = objAmt.RupeesToWord(masterDetails.CHQ_AMOUNT.ToString());

                model.BankEmail = bakDetails.BANK_EMAIL;

                //                model.BankEmail = "famshelp@gmail.com";
                //Below Lines modified on 18-11-2021 
                model.EmailSubject = PMGSYSession.Current.LevelId == 4 ? "An Epayment transaction is made by " + model.EpayDPIU + StateName + " on https://omms.nic.in,Epayment No: " + masterDetails.CHQ_NO : "An Epayment transaction is made by " + model.EpayDPIU + " ( District - " + district.MAST_DISTRICT_NAME + " ) of " + StateName + " on www.omms.nic.in,Epayment No: " + masterDetails.CHQ_NO;
                model.StateCode = PMGSYSession.Current.LevelId == 4 ? PMGSYSession.Current.StateCode.ToString() : district.MASTER_STATE.MAST_STATE_SHORT_CODE;
                model.DPIUCode = PMGSYSession.Current.LevelId == 4 ? "" : district.MAST_DISTRICT_CODE.ToString();
                model.BankPdfPassword = bakDetails.Bank_SEC_CODE;
                model.BillID = bill_id;
                if (dbContext.ACC_EPAY_MAIL_MASTER.Where(m => m.BILL_ID == bill_id && m.IS_EPAY_VALID == false).Any())
                {
                    long maxEpayCode = dbContext.ACC_EPAY_MAIL_MASTER.Where(m => m.BILL_ID == bill_id && m.IS_EPAY_VALID == false).Select(c => c.EPAY_ID).Max();

                    DateTime dt = dbContext.ACC_EPAY_MAIL_MASTER.Where(m => m.EPAY_ID == maxEpayCode).Select(m => m.EPAY_DATE).FirstOrDefault();

                    //string strOrgDate  = DateTime.ParseExact(strDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None).ToString("dd/MM/yyyy");


                    model.OrignalEpayDate = dt.ToString("dd/MM/yyyy"); ;

                    try
                    {
                        long? maxDetailedResendId = dbContext.ACC_EPAY_MAIL_RESEND_DETAILS.Where(m => m.OLD_EPAY_ID == maxEpayCode && m.FLAG_DR == "R").Select(m => m.DETAIL_ID).FirstOrDefault();

                        if (!(maxDetailedResendId == null))
                        {
                            model.IsNewResend = "R";
                            model.DetailResendEpayId = maxDetailedResendId;
                        }

                    }
                    catch (Exception ex)
                    {

                    }


                }
                else
                {
                    model.IsNewResend = "N";
                }

                using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt"))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString() + " UserName:- " + PMGSYSession.Current.UserName);
                    sw.WriteLine("Method : " + " Out of GetEpaymentDetails()");
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return model;

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error while getting epayment details");
            }
            finally
            {
                dbContext.Dispose();
            }

        }

        public int GetAccountIdByBillId(Int64 billId)
        {
            try
            {
                dbContext = new PMGSYEntities();

                return dbContext.ACC_BILL_MASTER.Where(x => x.BILL_ID == billId).Select(c => c.CON_ACCOUNT_ID.Value).FirstOrDefault();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PaymentDAL.GetAccountIdByBillId()");
                return 0;
            }
        }


        /// <summary>
        /// DAL function to get the epayment details
        /// </summary>
        /// <param name="bill_id"></param>
        /// <returns></returns>
        public EpaymentOrderModel GetEpaymentDetails(Int64 bill_id)
        {

            using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt"))
            {
                sw.WriteLine("Date :" + DateTime.Now.ToString() + " UserName:- " + PMGSYSession.Current.UserName);
                sw.WriteLine("Method : " + " In GetEpaymentDetails()");
                sw.WriteLine("____________________________________________________");
                sw.Close();
            }


            CommonFunctions commomFuncObj = new CommonFunctions();
            AmountToWord objAmt = new AmountToWord();
            try
            {
                EpaymentOrderModel model = new EpaymentOrderModel();
                dbContext = new PMGSYEntities();

                //Added By Abhishek for Reject/Resend Epayment 26 Sep 2014 start
                int _AdminNdCode = 0;
                int _ParendAdminNdCode = 0;
                int _DistrictCode = 0;

                if (PMGSYSession.Current.DistrictCode == 0)
                {
                    _AdminNdCode = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == bill_id).Select(s => s.ADMIN_ND_CODE).FirstOrDefault();
                    ADMIN_DEPARTMENT AdminDeptModel = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == _AdminNdCode).FirstOrDefault();
                    _ParendAdminNdCode = AdminDeptModel.MAST_PARENT_ND_CODE.Value;
                    _DistrictCode = AdminDeptModel.MAST_DISTRICT_CODE.Value;
                }
                else
                {
                    _AdminNdCode = PMGSYSession.Current.AdminNdCode;
                    _ParendAdminNdCode = PMGSYSession.Current.ParentNDCode.Value;
                    _DistrictCode = PMGSYSession.Current.DistrictCode;
                }
                //Added By Abhishek for Reject/Resend Epayment 26 Sep 2014 end

                //get state /district information
                MASTER_DISTRICT district = new MASTER_DISTRICT();
                district = dbContext.MASTER_DISTRICT.Where(v => v.MAST_DISTRICT_CODE == _DistrictCode).First();
                int stateCode = district.MAST_STATE_CODE;

                //get payment master details
                ACC_BILL_MASTER masterDetails = new ACC_BILL_MASTER();
                masterDetails = dbContext.ACC_BILL_MASTER.Where(x => x.BILL_ID == bill_id && x.CHQ_EPAY == "E").First();

                //Added by Abhishek kamble 1-Jul-2014 to show/hide package row from dialog box
                //model.EpayMasterTxnID = masterDetails.TXN_ID;

                //get all the unique  package id based on the roads
                List<Int32> lstBillDetails = new List<Int32>();
                lstBillDetails = dbContext.ACC_BILL_DETAILS.Where(c => c.BILL_ID == bill_id && c.CASH_CHQ == "Q" && c.CREDIT_DEBIT == "D" && c.IMS_PR_ROAD_CODE != null).
                    Select(f => f.IMS_PR_ROAD_CODE.Value).Distinct().ToList<Int32>();

                List<Int32> lstAgreement = new List<Int32>();
                lstAgreement = dbContext.ACC_BILL_DETAILS.Where(c => c.BILL_ID == bill_id && c.CASH_CHQ == "Q" && c.CREDIT_DEBIT == "D" && c.IMS_AGREEMENT_CODE != null).Select(F => F.IMS_AGREEMENT_CODE.Value).Distinct().ToList<Int32>();

                String Packages = String.Empty;
                String strAgreement = String.Empty;

                if (lstBillDetails.Count != 0)
                {

                    foreach (int item in lstBillDetails)
                    {
                        string newPackage = dbContext.IMS_SANCTIONED_PROJECTS.Where(c => c.IMS_PR_ROAD_CODE == item).Select(x => x.IMS_PACKAGE_ID).First();
                        //if new package is not allready present in Packages then only add
                        //if (!Packages.Contains(newPackage))
                        //{
                        Packages = Packages + "," + newPackage;
                        //}

                    }

                    if (Packages != string.Empty)
                    {
                        if (Packages[0] == ',')
                        {
                            Packages = Packages.Substring(1);
                        }

                    }
                }

                if (lstAgreement.Count != 0)
                {
                    if (PMGSYSession.Current.FundType.Equals("P"))
                    {
                        foreach (int item in lstAgreement)
                        {
                            string newAgreement = dbContext.TEND_AGREEMENT_MASTER.Where(c => c.TEND_AGREEMENT_CODE == item).Select(x => x.TEND_AGREEMENT_NUMBER).First();
                            //if new package is not allready present in Packages then only add
                            //if (!strAgreement.Contains(newAgreement))
                            //{
                            strAgreement = strAgreement + "," + newAgreement;
                            //}
                        }
                        if (strAgreement != string.Empty)
                        {
                            if (strAgreement[0] == ',')
                            {
                                strAgreement = strAgreement.Substring(1);
                            }
                        }
                    }
                    else if (PMGSYSession.Current.FundType.Equals("M"))
                    {
                        foreach (int item in lstAgreement)
                        {
                            string newAgreement = dbContext.MANE_IMS_CONTRACT.Where(c => c.MANE_CONTRACT_ID == item).Select(x => x.MANE_AGREEMENT_NUMBER).First();
                            //if new package is not allready present in Packages then only add
                            if (!strAgreement.Contains(newAgreement))
                            {
                                strAgreement = strAgreement + "," + newAgreement;
                            }
                        }
                        if (strAgreement != string.Empty)
                        {
                            if (strAgreement[0] == ',')
                            {
                                strAgreement = strAgreement.Substring(1);
                            }
                        }
                    }

                }



                string StateName = dbContext.MASTER_STATE.Where(v => v.MAST_STATE_CODE == stateCode).Select(c => c.MAST_STATE_NAME).First();


                //get Contractor details 
                // MASTER_CONTRACTOR_BANK con = new MASTER_CONTRACTOR_BANK();
                //con = dbContext.MASTER_CONTRACTOR_BANK.Where(v => v.MAST_CON_ID == masterDetails.MAST_CON_ID && v.MAST_ACCOUNT_STATUS == "A").FirstOrDefault();

                //Modified By Abhishek kamble 21-July-2014 start
                //get contractor details
                MASTER_CONTRACTOR_BANK con = new MASTER_CONTRACTOR_BANK();
                ACC_BANK_DETAILS secDepBankDetails = new ACC_BANK_DETAILS();//Added on 21-04-2023
                ACC_BANK_DETAILS holdAccBankDetails = new ACC_BANK_DETAILS();//Added on 04-05-2023
                ADMIN_NO_BANK noOffBank = new ADMIN_NO_BANK();
                // if (PMGSYSession.Current.FundType != "A")
                //{

                string moduleType = "R";
                //commented on 24-11-2021
                //string moduleType = "D";
                //REAT_OMMAS_PAYMENT_SUCCESS objModuleType = dbContext.REAT_OMMAS_PAYMENT_SUCCESS.FirstOrDefault(s => s.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode);
                //if (objModuleType != null)
                //{
                //    moduleType = "R";
                //}


                //if (masterDetails.TXN_ID == 472 || masterDetails.TXN_ID == 415)
                //{
                //    noOffBank = dbContext.ADMIN_NO_BANK.Where(v => v.ADMIN_NO_OFFICER_CODE == masterDetails.ADMIN_NO_OFFICER_CODE && v.MAST_ACCOUNT_STATUS == "A").FirstOrDefault();
                //}
                //else
                //{
                if (moduleType.Equals("D"))
                {
                    con = dbContext.MASTER_CONTRACTOR_BANK.Where(v => v.MAST_CON_ID == masterDetails.MAST_CON_ID && v.MAST_ACCOUNT_STATUS == "A" && v.MAST_DISTRICT_CODE ==
                        _DistrictCode).FirstOrDefault();
                    if (con == null)
                    {
                        if (PMGSYSession.Current.StateCode > 0)
                        {
                            var districts = dbContext.MASTER_DISTRICT.Where(x => x.MAST_STATE_CODE == PMGSYSession.Current.StateCode && x.MAST_DISTRICT_ACTIVE == "Y").Select(x => x.MAST_DISTRICT_CODE).ToList();
                            if (districts != null)
                            {
                                if (dbContext.MASTER_CONTRACTOR_BANK.Any(v => v.MAST_CON_ID == masterDetails.MAST_CON_ID && v.MAST_ACCOUNT_STATUS == "A"))
                                {
                                    con = dbContext.MASTER_CONTRACTOR_BANK.Where(v => v.MAST_CON_ID == masterDetails.MAST_CON_ID && v.MAST_ACCOUNT_STATUS == "A"
                                        //&& v.MASTER_CONTRACTOR.MASTER_CONTRACTOR_REGISTRATION.Where(x => x.MAST_CON_ID == contractorId).Select(x => x.MAST_REG_STATE).FirstOrDefault() == PMGSYSession.Current.StateCode
                                        && districts.Contains(v.MAST_DISTRICT_CODE)
                                        ).FirstOrDefault();

                                    if (con == null)
                                    {
                                        con = dbContext.MASTER_CONTRACTOR_BANK.Where(v => v.MAST_CON_ID == masterDetails.MAST_CON_ID && v.MAST_ACCOUNT_STATUS == "A").FirstOrDefault();
                                    }
                                }
                            }
                            else
                            {
                                con = dbContext.MASTER_CONTRACTOR_BANK.Where(v => v.MAST_CON_ID == masterDetails.MAST_CON_ID && v.MAST_ACCOUNT_STATUS == "A").FirstOrDefault();
                            }
                        }
                        else
                        {
                            con = dbContext.MASTER_CONTRACTOR_BANK.Where(v => v.MAST_CON_ID == masterDetails.MAST_CON_ID && v.MAST_ACCOUNT_STATUS == "A").FirstOrDefault();
                        }
                    }
                }

                if (moduleType.Equals("R"))
                {
                    //Below Code commented on 15-03-2023
                    //con = dbContext.MASTER_CONTRACTOR_BANK.Where(v => v.MAST_CON_ID == masterDetails.MAST_CON_ID && v.MAST_ACCOUNT_ID == masterDetails.CON_ACCOUNT_ID).FirstOrDefault();

                    //Below Code Added on 15-03-2023
                    if (masterDetails.TXN_ID == 3185)//amount credited to security deposit account bank 
                    {

                        secDepBankDetails = dbContext.ACC_BANK_DETAILS.Where(f => f.FUND_TYPE == PMGSYSession.Current.FundType && f.ACCOUNT_TYPE == "D" && f.MAST_STATE_CODE == PMGSYSession.Current.StateCode && f.BANK_ACC_STATUS == true).FirstOrDefault();

                    }
                    else if (masterDetails.TXN_ID == 3171)//amount credited to Holding account bank 
                    {
                        holdAccBankDetails = dbContext.ACC_BANK_DETAILS.Where(f => f.FUND_TYPE == PMGSYSession.Current.FundType && f.ACCOUNT_TYPE == "H" && f.MAST_STATE_CODE == PMGSYSession.Current.StateCode && f.BANK_ACC_STATUS == true).FirstOrDefault();
                    }
                    else
                    {
                        con = dbContext.MASTER_CONTRACTOR_BANK.Where(v => v.MAST_CON_ID == masterDetails.MAST_CON_ID && v.MAST_ACCOUNT_ID == masterDetails.CON_ACCOUNT_ID).FirstOrDefault();

                    }
                }

                //}
                //}

                //Modified By Abhishek kamble 21-July-2014 end


                //get Epayment bank details

                ACC_BANK_DETAILS bakDetails = new ACC_BANK_DETAILS();
                //
                //int? parentNdVode = dbContext.ADMIN_DEPARTMENT.Where(b => b.ADMIN_ND_CODE == _AdminNdCode).Select(d => d.MAST_PARENT_ND_CODE).First();

                //if (parentNdVode.HasValue)
                //{

                //    bakDetails = dbContext.ACC_BANK_DETAILS.Where(f => f.ADMIN_ND_CODE == parentNdVode.Value && f.FUND_TYPE == PMGSYSession.Current.FundType && f.BANK_ACC_STATUS == true).First();

                //}

                if (masterDetails.TXN_ID == 3185)
                {

                    int? parentNdVode = dbContext.ADMIN_DEPARTMENT.Where(b => b.ADMIN_ND_CODE == _AdminNdCode).Select(d => d.MAST_PARENT_ND_CODE).First();

                    if (parentNdVode.HasValue)
                    {
                        //Holding account details
                        //bakDetails = dbContext.ACC_BANK_DETAILS.Where(f => f.ADMIN_ND_CODE == parentNdVode.Value && f.FUND_TYPE == PMGSYSession.Current.FundType && f.BANK_ACC_STATUS == true && f.ACCOUNT_TYPE=="H").First();
                        bakDetails = dbContext.ACC_BANK_DETAILS.Where(f => f.ADMIN_ND_CODE == parentNdVode.Value && f.FUND_TYPE == PMGSYSession.Current.FundType && f.BANK_ACC_STATUS == true && f.ACCOUNT_TYPE == "H").Any() ? dbContext.ACC_BANK_DETAILS.Where(f => f.ADMIN_ND_CODE == parentNdVode.Value && f.FUND_TYPE == PMGSYSession.Current.FundType && f.BANK_ACC_STATUS == true && f.ACCOUNT_TYPE == "H").First() : bakDetails;

                    }
                }
                else
                {
                    int? parentNdVode = dbContext.ADMIN_DEPARTMENT.Where(b => b.ADMIN_ND_CODE == _AdminNdCode).Select(d => d.MAST_PARENT_ND_CODE).First();

                    if (parentNdVode.HasValue)
                    {
                        //***--***
                        bakDetails = dbContext.ACC_BANK_DETAILS.Where(f => f.ADMIN_ND_CODE == parentNdVode.Value && f.FUND_TYPE == PMGSYSession.Current.FundType && f.BANK_ACC_STATUS == true && f.ACCOUNT_TYPE == "S").First();

                    }


                }

                model.AgreementNumber = strAgreement;
                model.EmailRecepient = bakDetails.BANK_EMAIL; //dbContext.ADMIN_NODAL_OFFICERS.Where(a => a.ADMIN_ND_CODE == parentNdVode.Value && a.ADMIN_ACTIVE_STATUS == "Y").Select(f => f.ADMIN_NO_EMAIL).First();
                //model.EmailRecepient = "famshelp@gmail.com";
                model.EmailBCC = ConfigurationManager.AppSettings["EpayBCC"].ToString();
                //new change done by Vikram on 29 Jan 2014
                //if (dbContext.ACC_EPAY_MAIL_OTHER.Any(m => m.ADMIN_ND_CODE == PMGSYSession.Current.ParentNDCode))
                if (dbContext.ACC_EPAY_MAIL_OTHER.Where(m => m.ADMIN_ND_CODE == _ParendAdminNdCode).Any())
                {
                    model.EmailCC = dbContext.ACC_EPAY_MAIL_OTHER.Where(m => m.ADMIN_ND_CODE == _ParendAdminNdCode).Select(m => m.EMAIL_CC).FirstOrDefault();
                    //model.EmailCC = "omms@cdac.in";
                    model.SrrdaPassword = dbContext.ACC_EPAY_MAIL_OTHER.Where(m => m.ADMIN_ND_CODE == _ParendAdminNdCode).Select(m => m.PDF_OPEN_KEY).FirstOrDefault();
                }
                else
                {
                    model.EmailCC = "";
                    model.SrrdaPassword = "";
                }
                model.DPIUName = district.MAST_DISTRICT_NAME;
                model.STATEName = StateName;
                if (PMGSYSession.Current.RoleCode == 26)
                {
                    model.EmailDate = commomFuncObj.GetDateTimeToString(DateTime.Now);
                }
                else
                {
                    model.EmailDate = "";
                }
                //Commented By Abhishek kamble 19-mar-2014
                //model.Bankaddress = bakDetails.BANK_ADDRESS1 == null ? String.Empty : bakDetails.BANK_ADDRESS1;

                //Commented on 20-mar-2023
                //model.Bankaddress = (bakDetails.BANK_NAME == null ? String.Empty : bakDetails.BANK_NAME) + (bakDetails.BANK_ADDRESS1 == null ? String.Empty : "-" + bakDetails.BANK_ADDRESS1) + (bakDetails.BANK_ADDRESS2 == null ? String.Empty : ", " + bakDetails.BANK_ADDRESS2) + ", " + (bakDetails.MASTER_STATE.MAST_STATE_NAME);
                model.Bankaddress = (bakDetails.BANK_NAME == null ? String.Empty : bakDetails.BANK_NAME) + (bakDetails.BANK_ADDRESS1 == null ? String.Empty : "-" + bakDetails.BANK_ADDRESS1) + (bakDetails.BANK_ADDRESS2 == null ? String.Empty : ", " + bakDetails.BANK_ADDRESS2) + ", " + (bakDetails.MASTER_STATE == null ? String.Empty : bakDetails.MASTER_STATE.MAST_STATE_NAME);

                model.BankAcNumber = bakDetails.BANK_ACC_NO == null ? String.Empty : bakDetails.BANK_ACC_NO;

                //Below line added on 15-03-2023
                model.BankIFSCCode = bakDetails.MAST_IFSC_CODE == null ? String.Empty : bakDetails.MAST_IFSC_CODE;

                model.EpayNumber = masterDetails.CHQ_NO == null ? String.Empty : masterDetails.CHQ_NO;
                if (masterDetails.CHQ_DATE.HasValue)
                {
                    model.EpayDate = commomFuncObj.GetDateTimeToString(masterDetails.CHQ_DATE.Value);
                }
                else
                {
                    model.EpayDate = "";
                }

                model.EpayState = StateName;
                //model.EpayDPIU = district.MAST_DISTRICT_NAME;
                //Modified by Abhishek kamble to set DPIU name old it is dist name. 25-June-2014
                model.EpayDPIU = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == _AdminNdCode).Select(s => s.ADMIN_ND_NAME).FirstOrDefault();

                model.EpayVNumber = masterDetails.BILL_NO;
                model.EpayVDate = commomFuncObj.GetDateTimeToString(masterDetails.BILL_DATE);
                model.EpayVPackages = Packages;



                //Added By Abhishek kamble 28-May-2014 start
                MASTER_CONTRACTOR mastContDetailsModel = new MASTER_CONTRACTOR();
                ADMIN_NODAL_OFFICERS adminNoModel = new ADMIN_NODAL_OFFICERS();
                // string conName = string.Empty;
                string contNameContCompanyName = string.Empty;
                //if (masterDetails.TXN_ID == 472 || masterDetails.TXN_ID == 415)
                //{
                //    adminNoModel = dbContext.ADMIN_NODAL_OFFICERS.Where(c => c.ADMIN_NO_OFFICER_CODE == masterDetails.ADMIN_NO_OFFICER_CODE && c.ADMIN_MODULE == "A" && c.ADMIN_ACTIVE_STATUS == "Y").FirstOrDefault();
                //    if (!(adminNoModel == null))
                //    {
                //        contNameContCompanyName = adminNoModel.ADMIN_NO_FNAME + " " + adminNoModel.ADMIN_NO_MNAME + " " + adminNoModel.ADMIN_NO_LNAME;
                //        model.EpayConName = contNameContCompanyName;
                //    }
                //    else
                //    {
                //        model.EpayConName = "";
                //    }
                //}
                //else
                //{
                mastContDetailsModel = dbContext.MASTER_CONTRACTOR.Where(c => c.MAST_CON_ID == masterDetails.MAST_CON_ID).FirstOrDefault();
                if (masterDetails.TXN_ID == 3185 || masterDetails.TXN_ID == 3171) //Condition added on 21-04-2023 as no contractor details will be available for txn_Id=3185 and  txn_Id=3171 
                {


                    if (masterDetails.PAYEE_NAME != null)
                    {
                        contNameContCompanyName = masterDetails.PAYEE_NAME;
                    }
                    model.EpayConName = contNameContCompanyName;
                }
                else
                {
                    if (mastContDetailsModel.MAST_CON_COMPANY_NAME != null)
                    {
                        contNameContCompanyName = mastContDetailsModel.MAST_CON_FNAME + " " + mastContDetailsModel.MAST_CON_MNAME + " " + mastContDetailsModel.MAST_CON_LNAME + " ( " + mastContDetailsModel.MAST_CON_COMPANY_NAME + " )";
                    }
                    else
                    {
                        contNameContCompanyName = mastContDetailsModel.MAST_CON_FNAME + " " + mastContDetailsModel.MAST_CON_MNAME + " " + mastContDetailsModel.MAST_CON_LNAME;
                    }
                    model.EpayConName = contNameContCompanyName;

                    //Contractor legal heir details
                    //Added By Abhishek kamble 28-May-2014 end

                    if (mastContDetailsModel.MAST_CON_EXPIRY_DATE != null && mastContDetailsModel.MAST_CON_STATUS == "E")
                    {
                        model.EpayContLegalHeirName = mastContDetailsModel.MAST_CON_LEGAL_HEIR_FNAME + " " + mastContDetailsModel.MAST_CON_LEGAL_HEIR_MNAME + " " + mastContDetailsModel.MAST_CON_LEGAL_HEIR_LNAME;
                    }
                }
                //}


                //if (masterDetails.TXN_ID == 472 || masterDetails.TXN_ID == 415)
                //{

                //    if (noOffBank != null)
                //    {
                //        model.EpayConAcNum = noOffBank.MAST_ACCOUNT_NUMBER == null ? String.Empty : noOffBank.MAST_ACCOUNT_NUMBER;
                //        model.EpayConBankName = noOffBank.MAST_BANK_NAME == null ? String.Empty : noOffBank.MAST_BANK_NAME;
                //        model.EpayConBankIFSCCode = noOffBank.MAST_IFSC_CODE == null ? String.Empty : noOffBank.MAST_IFSC_CODE;
                //    }
                //    else
                //    {
                //        model.EpayConAcNum = String.Empty;
                //        model.EpayConBankName = String.Empty;
                //        model.EpayConBankIFSCCode = String.Empty;
                //    }
                //}
                //else
                //{

                //Below Code commented on 15-03-2023 
                //if (con != null)
                //{
                //    model.EpayConAcNum = con.MAST_ACCOUNT_NUMBER == null ? String.Empty : con.MAST_ACCOUNT_NUMBER;
                //    model.EpayConBankName = con.MAST_BANK_NAME == null ? String.Empty : con.MAST_BANK_NAME;
                //    model.EpayConBankIFSCCode = con.MAST_IFSC_CODE == null ? String.Empty : con.MAST_IFSC_CODE;
                //}
                //else
                //{
                //    model.EpayConAcNum = String.Empty;
                //    model.EpayConBankName = String.Empty;
                //    model.EpayConBankIFSCCode = String.Empty;
                //}
                //Below Code Added on 15-03-2023 
                if (masterDetails.TXN_ID == 3185)
                {
                    if (secDepBankDetails != null)
                    {
                        model.EpayConAcNum = secDepBankDetails.BANK_ACC_NO == null ? String.Empty : secDepBankDetails.BANK_ACC_NO;
                        model.EpayConBankName = secDepBankDetails.BANK_NAME == null ? String.Empty : secDepBankDetails.BANK_NAME;
                        model.EpayConBankIFSCCode = secDepBankDetails.MAST_IFSC_CODE == null ? String.Empty : secDepBankDetails.MAST_IFSC_CODE;
                    }
                    else
                    {
                        model.EpayConAcNum = String.Empty;
                        model.EpayConBankName = String.Empty;
                        model.EpayConBankIFSCCode = String.Empty;
                    }
                }
                else if (masterDetails.TXN_ID == 3171)
                {
                    if (holdAccBankDetails != null)
                    {
                        model.EpayConAcNum = holdAccBankDetails.BANK_ACC_NO == null ? String.Empty : holdAccBankDetails.BANK_ACC_NO;
                        model.EpayConBankName = holdAccBankDetails.BANK_NAME == null ? String.Empty : holdAccBankDetails.BANK_NAME;
                        model.EpayConBankIFSCCode = holdAccBankDetails.MAST_IFSC_CODE == null ? String.Empty : holdAccBankDetails.MAST_IFSC_CODE;
                    }
                    else
                    {
                        model.EpayConAcNum = String.Empty;
                        model.EpayConBankName = String.Empty;
                        model.EpayConBankIFSCCode = String.Empty;
                    }
                }
                else
                {
                    if (con != null)
                    {
                        model.EpayConAcNum = con.MAST_ACCOUNT_NUMBER == null ? String.Empty : con.MAST_ACCOUNT_NUMBER;
                        model.EpayConBankName = con.MAST_BANK_NAME == null ? String.Empty : con.MAST_BANK_NAME;
                        model.EpayConBankIFSCCode = con.MAST_IFSC_CODE == null ? String.Empty : con.MAST_IFSC_CODE;
                    }
                    else
                    {
                        model.EpayConAcNum = String.Empty;
                        model.EpayConBankName = String.Empty;
                        model.EpayConBankIFSCCode = String.Empty;
                    }
                }
                //}

                model.EpayAmount = masterDetails.CHQ_AMOUNT.ToString();
                model.EpayAmountInWord = objAmt.RupeesToWord(masterDetails.CHQ_AMOUNT.ToString());

                model.BankEmail = bakDetails.BANK_EMAIL;

                //                model.BankEmail = "famshelp@gmail.com";

                model.EmailSubject = "An Epayment transaction is made by " + model.EpayDPIU + " ( District - " + district.MAST_DISTRICT_NAME + " ) of " + StateName + " on https://omms.nic.in,Epayment No: " + masterDetails.CHQ_NO;
                model.StateCode = district.MASTER_STATE.MAST_STATE_SHORT_CODE;
                model.DPIUCode = district.MAST_DISTRICT_CODE.ToString();
                model.BankPdfPassword = bakDetails.Bank_SEC_CODE;
                model.BillID = bill_id;
                if (dbContext.ACC_EPAY_MAIL_MASTER.Where(m => m.BILL_ID == bill_id && m.IS_EPAY_VALID == false).Any())
                {
                    long maxEpayCode = dbContext.ACC_EPAY_MAIL_MASTER.Where(m => m.BILL_ID == bill_id && m.IS_EPAY_VALID == false).Select(c => c.EPAY_ID).Max();

                    DateTime dt = dbContext.ACC_EPAY_MAIL_MASTER.Where(m => m.EPAY_ID == maxEpayCode).Select(m => m.EPAY_DATE).FirstOrDefault();

                    //string strOrgDate  = DateTime.ParseExact(strDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None).ToString("dd/MM/yyyy");


                    model.OrignalEpayDate = dt.ToString("dd/MM/yyyy"); ;

                    try
                    {
                        long? maxDetailedResendId = dbContext.ACC_EPAY_MAIL_RESEND_DETAILS.Where(m => m.OLD_EPAY_ID == maxEpayCode && m.FLAG_DR == "R").Select(m => m.DETAIL_ID).FirstOrDefault();

                        if (!(maxDetailedResendId == null))
                        {
                            model.IsNewResend = "R";
                            model.DetailResendEpayId = maxDetailedResendId;
                        }

                    }
                    catch (Exception ex)
                    {

                    }


                }
                else
                {
                    model.IsNewResend = "N";
                }

                using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt"))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString() + " UserName:- " + PMGSYSession.Current.UserName);
                    sw.WriteLine("Method : " + " Out of GetEpaymentDetails()");
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }
                return model;

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error while getting epayment details");
            }
            finally
            {
                dbContext.Dispose();
            }

        }


        /// <summary>
        /// function to display the eremmitance details before finalization by authorizaed signatory 
        /// </summary>
        /// <param name="bill_id"></param>
        /// <returns></returns>
        public EremittnaceOrderModel GetEremittanceDetails(Int64 bill_id)
        {
            CommonFunctions commomFuncObj = new CommonFunctions();
            AmountToWord objAmt = new AmountToWord();
            try
            {
                EremittnaceOrderModel model = new EremittnaceOrderModel();
                dbContext = new PMGSYEntities();
                //Added By Abhishek for Reject/Resend Epayment 29 Sep 2014 start
                int _AdminNdCode = 0;
                int _ParendAdminNdCode = 0;
                int _DistrictCode = 0;

                if (PMGSYSession.Current.DistrictCode == 0)
                {
                    _AdminNdCode = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == bill_id).Select(s => s.ADMIN_ND_CODE).FirstOrDefault();
                    ADMIN_DEPARTMENT AdminDeptModel = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == _AdminNdCode).FirstOrDefault();
                    _ParendAdminNdCode = AdminDeptModel.MAST_PARENT_ND_CODE.Value;
                    _DistrictCode = AdminDeptModel.MAST_DISTRICT_CODE.Value;
                }
                else
                {
                    _AdminNdCode = PMGSYSession.Current.AdminNdCode;
                    _ParendAdminNdCode = PMGSYSession.Current.ParentNDCode.Value;
                    _DistrictCode = PMGSYSession.Current.DistrictCode;
                }
                //Added By Abhishek for Reject/Resend Epayment 29 Sep 2014 end

                //get state /district information
                MASTER_DISTRICT district = new MASTER_DISTRICT();
                district = dbContext.MASTER_DISTRICT.Where(v => v.MAST_DISTRICT_CODE == _DistrictCode).First();
                int stateCode = district.MAST_STATE_CODE;

                //get payment master details
                ACC_BILL_MASTER masterDetails = new ACC_BILL_MASTER();
                masterDetails = dbContext.ACC_BILL_MASTER.Where(x => x.BILL_ID == bill_id && x.CHQ_EPAY == "E").First();

                //get all the unique  package id based on the roads
                List<Int32> lstBillDetails = new List<Int32>();
                lstBillDetails = dbContext.ACC_BILL_DETAILS.Where(c => c.BILL_ID == bill_id && c.CASH_CHQ == "Q" && c.CREDIT_DEBIT == "D" && c.IMS_PR_ROAD_CODE != null).Select(f => f.IMS_PR_ROAD_CODE.Value).Distinct().ToList<Int32>();


                string StateName = dbContext.MASTER_STATE.Where(v => v.MAST_STATE_CODE == stateCode).Select(c => c.MAST_STATE_NAME).First();


                //get Epayment bank details

                ACC_BANK_DETAILS bakDetails = new ACC_BANK_DETAILS();
                int? parentNdVode = dbContext.ADMIN_DEPARTMENT.Where(b => b.ADMIN_ND_CODE == _AdminNdCode).Select(d => d.MAST_PARENT_ND_CODE).First();

                if (parentNdVode.HasValue)
                {

                    bakDetails = dbContext.ACC_BANK_DETAILS.Where(f => f.ADMIN_ND_CODE == parentNdVode.Value && f.FUND_TYPE == PMGSYSession.Current.FundType && f.BANK_ACC_STATUS == true && f.ACCOUNT_TYPE == "S").First();

                }

                model.EmailRecepient = bakDetails.BANK_EMAIL;// dbContext.ADMIN_NODAL_OFFICERS.Where(a => a.ADMIN_ND_CODE == parentNdVode.Value && a.ADMIN_ACTIVE_STATUS == "Y").Select(f => f.ADMIN_NO_EMAIL).First();
                //model.EmailRecepient = "famshelp@gmail.com";     
                //model.EmailBCC = ConfigurationManager.AppSettings["EpayBCC"].ToString();

                model.DPIUName = district.MAST_DISTRICT_NAME;
                model.STATEName = StateName;
                if (PMGSYSession.Current.RoleCode == 26)
                {
                    model.EmailDate = commomFuncObj.GetDateTimeToString(DateTime.Now);
                }
                else
                {
                    model.EmailDate = "";
                }
                //model.Bankaddress = bakDetails.BANK_ADDRESS1 == null ? String.Empty : bakDetails.BANK_ADDRESS1;                
                model.Bankaddress = (bakDetails.BANK_NAME == null ? String.Empty : bakDetails.BANK_NAME) + (bakDetails.BANK_ADDRESS1 == null ? String.Empty : "-" + bakDetails.BANK_ADDRESS1) + (bakDetails.BANK_ADDRESS2 == null ? String.Empty : ", " + bakDetails.BANK_ADDRESS2) + ", " + (bakDetails.MASTER_STATE.MAST_STATE_NAME);

                model.BankAcNumber = bakDetails.BANK_ACC_NO == null ? String.Empty : bakDetails.BANK_ACC_NO;
                model.EpayNumber = masterDetails.CHQ_NO == null ? String.Empty : masterDetails.CHQ_NO;

                if (dbContext.ACC_EPAY_MAIL_OTHER.Where(m => m.ADMIN_ND_CODE == _ParendAdminNdCode).Any())
                {
                    model.EmailCC = dbContext.ACC_EPAY_MAIL_OTHER.Where(m => m.ADMIN_ND_CODE == _ParendAdminNdCode).Select(m => m.EMAIL_CC).FirstOrDefault();
                    //model.EmailCC = "omms@cdac.in";
                    model.SrrdaPassword = dbContext.ACC_EPAY_MAIL_OTHER.Where(m => m.ADMIN_ND_CODE == _ParendAdminNdCode).Select(m => m.PDF_OPEN_KEY).FirstOrDefault();
                }
                else
                {
                    model.EmailCC = "";
                    model.SrrdaPassword = "";
                }

                if (masterDetails.CHQ_DATE.HasValue)
                {
                    model.EpayDate = commomFuncObj.GetDateTimeToString(masterDetails.CHQ_DATE.Value);
                }
                else
                {
                    model.EpayDate = "";
                }

                model.EpayState = StateName;
                //model.EpayDPIU = district.MAST_DISTRICT_NAME;

                //Modified by Abhishek kamble to set DPIU name old it is dist name. 25-June-2014
                model.EpayDPIU = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == _AdminNdCode).Select(s => s.ADMIN_ND_NAME).FirstOrDefault();

                model.EpayVNumber = masterDetails.BILL_NO;
                model.EpayVDate = commomFuncObj.GetDateTimeToString(masterDetails.BILL_DATE);

                model.BankEmail = bakDetails.BANK_EMAIL;
                //model.BankEmail = "famshelp@gmail.com";
                model.EmailSubject = "An e-Remittance transaction is made by " + model.EpayDPIU + " ( " + district.MAST_DISTRICT_NAME + " ) of " + StateName + " on https://omms.nic.in,e-Remittance No: " + masterDetails.CHQ_NO;
                model.StateCode = district.MASTER_STATE.MAST_STATE_SHORT_CODE;
                model.DPIUCode = district.MAST_DISTRICT_CODE.ToString();
                model.BankPdfPassword = bakDetails.Bank_SEC_CODE;
                model.DPIUTANNumber = dbContext.ADMIN_DEPARTMENT.Where(b => b.ADMIN_ND_CODE == _AdminNdCode).Select(x => x.ADMIN_ND_TAN_NO).FirstOrDefault();
                // model.DepartmentAcNum = dbContext.ACC_REM_ACCOUNT_DETAILS.Where(x => x.MAST_STATE_CODE == stateCode && x.REM_TYPE == masterDetails.REMIT_TYPE).Select(x => x.BANK_ACCOUNT_NO).FirstOrDefault().ToString();
                model.DepartmentNameFull = dbContext.ACC_MASTER_REMIT_DEPT.Where(x => x.DEPT_ID == masterDetails.REMIT_TYPE).Select(x => x.DEPT_NAME).FirstOrDefault();
                model.DepartmentName = model.DepartmentNameFull.Remove(model.DepartmentNameFull.LastIndexOf("Department"));
                // get the Contractor Details
                List<EremittnaceContractor> BillContractorList = new List<EremittnaceContractor>();
                List<ACC_BILL_DETAILS> listBillDetails = new List<ACC_BILL_DETAILS>();
                listBillDetails = dbContext.ACC_BILL_DETAILS.Where(c => c.BILL_ID == bill_id && c.CREDIT_DEBIT == "D" && c.CASH_CHQ == "Q").ToList<ACC_BILL_DETAILS>();

                foreach (ACC_BILL_DETAILS bill in listBillDetails)
                {

                    EremittnaceContractor Contractor = new EremittnaceContractor();
                    MASTER_CONTRACTOR contractorInfo = dbContext.MASTER_CONTRACTOR.Where(c => c.MAST_CON_ID == bill.MAST_CON_ID).FirstOrDefault();
                    if (contractorInfo != null)
                    {
                        //Modified By Abhishek kamble 28-May-2014 start
                        // Contractor.EpayConName = contractorInfo.MAST_CON_COMPANY_NAME;
                        string conName = contractorInfo.MAST_CON_FNAME + " " + contractorInfo.MAST_CON_MNAME + " " + contractorInfo.MAST_CON_LNAME;
                        string conCompanyName = contractorInfo.MAST_CON_COMPANY_NAME;
                        string contNameContCompanyName = string.Empty;

                        if (conCompanyName != null)
                        {
                            contNameContCompanyName = conName + " ( " + conCompanyName + " )";
                        }
                        else
                        {
                            contNameContCompanyName = conName;
                        }

                        Contractor.EpayConName = contNameContCompanyName;

                        MASTER_CONTRACTOR_BANK contBankDetails = dbContext.MASTER_CONTRACTOR_BANK.Where(v => v.MAST_CON_ID == bill.MAST_CON_ID && v.MAST_ACCOUNT_STATUS == "A" && v.MAST_DISTRICT_CODE == _DistrictCode).FirstOrDefault();
                        if (contBankDetails == null)
                        {
                            contBankDetails = dbContext.MASTER_CONTRACTOR_BANK.Where(v => v.MAST_CON_ID == bill.MAST_CON_ID && v.MAST_ACCOUNT_STATUS == "A").FirstOrDefault();
                        }

                        Contractor.EpayConAcNum = contBankDetails.MAST_ACCOUNT_NUMBER;
                        Contractor.EpayConBankName = contBankDetails.MAST_BANK_NAME;

                        //Modified By Abhishek kamble 28-May-2014 end

                        Contractor.EpayAmount = bill.AMOUNT.ToString();
                        Contractor.EpayConPanNumber = dbContext.MASTER_CONTRACTOR.Where(c => c.MAST_CON_ID == bill.MAST_CON_ID).Select(d => d.MAST_CON_PAN).FirstOrDefault();

                        //Modified By Abhihshek kamble to get Agre No using MANE_CONTRACTOR_ID 17Nov2014
                        Contractor.EpayConAggreement = masterDetails.FUND_TYPE == "M" ? (dbContext.MANE_IMS_CONTRACT.Where(c => c.MANE_CONTRACT_ID == (bill.IMS_AGREEMENT_CODE.HasValue ? bill.IMS_AGREEMENT_CODE.Value : -1)).Select(x => x.MANE_AGREEMENT_NUMBER).FirstOrDefault()) : (dbContext.TEND_AGREEMENT_MASTER.Where(c => c.TEND_AGREEMENT_CODE == (bill.IMS_AGREEMENT_CODE.HasValue ? bill.IMS_AGREEMENT_CODE.Value : -1)).Select(y => y.TEND_AGREEMENT_NUMBER).FirstOrDefault());

                        BillContractorList.Add(Contractor);
                    }
                    model.TotalAmount = model.TotalAmount + bill.AMOUNT;
                }


                model.EpayTotalAmountInWord = objAmt.RupeesToWord(model.TotalAmount.ToString());
                model.ContractorList = BillContractorList;

                if (dbContext.ACC_EPAY_MAIL_MASTER.Where(m => m.BILL_ID == bill_id && m.IS_EPAY_VALID == false).Any())
                {
                    long maxEpayCode = dbContext.ACC_EPAY_MAIL_MASTER.Where(m => m.BILL_ID == bill_id && m.IS_EPAY_VALID == false).Select(c => c.EPAY_ID).Max();

                    DateTime dt = dbContext.ACC_EPAY_MAIL_MASTER.Where(m => m.EPAY_ID == maxEpayCode).Select(m => m.EPAY_DATE).FirstOrDefault();

                    //string strOrgDate  = DateTime.ParseExact(strDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None).ToString("dd/MM/yyyy");


                    model.OrignalEremiDate = dt.ToString("dd/MM/yyyy"); ;

                    try
                    {
                        long? maxDetailedResendId = dbContext.ACC_EPAY_MAIL_RESEND_DETAILS.Where(m => m.OLD_EPAY_ID == maxEpayCode && m.FLAG_DR == "R").Select(m => m.DETAIL_ID).FirstOrDefault();

                        if (!(maxDetailedResendId == null))
                        {
                            model.IsNewResend = "R";
                            model.DetailResendEpayId = maxDetailedResendId;
                        }

                    }
                    catch (Exception ex)
                    {
                    }

                }
                else
                {
                    model.IsNewResend = "N";
                }
                return model;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error while getting Eremittance details");
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// function to get the e remittance details finalized by authorized signatory
        /// </summary>
        /// <param name="bill_id"></param>
        /// <returns></returns>
        public EremittnaceOrderModel GetEremittanceDetailsFinalizedByAuthSig(Int64 bill_ID)
        {
            CommonFunctions commomFuncObj = new CommonFunctions();
            AmountToWord objAmt = new AmountToWord();

            try
            {
                dbContext = new PMGSYEntities();

                EremittnaceOrderModel model = new EremittnaceOrderModel();

                ACC_EPAY_MAIL_MASTER emailMaster = new ACC_EPAY_MAIL_MASTER();

                ACC_EPAY_MAIL_DETAILS emailDetails = new ACC_EPAY_MAIL_DETAILS();

                emailMaster = dbContext.ACC_EPAY_MAIL_MASTER.Where(c => c.BILL_ID == bill_ID && c.IS_EPAY_VALID == true).First(); //IS_EPAY_VALID =true Checked by Abhishek for Reject/Resend Epayment Details 25-Sep-2014

                emailDetails = dbContext.ACC_EPAY_MAIL_DETAILS.Where(c => c.EPAY_ID == emailMaster.EPAY_ID).First();

                int? parentNdVode = dbContext.ADMIN_DEPARTMENT.Where(b => b.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Select(d => d.MAST_PARENT_ND_CODE).First();

                //get payment master details
                ACC_BILL_MASTER masterDetails = new ACC_BILL_MASTER();
                masterDetails = dbContext.ACC_BILL_MASTER.Where(x => x.BILL_ID == bill_ID).First();


                //get authorized bank details
                ACC_BANK_DETAILS bankDetails = new ACC_BANK_DETAILS();
                if (parentNdVode.HasValue)
                {
                    //get the bank details as per bank account number in bill details
                    bankDetails = dbContext.ACC_BANK_DETAILS.Where(f => f.ADMIN_ND_CODE == parentNdVode.Value && f.BANK_ACC_NO == emailDetails.BANK_ACC_NO && f.FUND_TYPE == PMGSYSession.Current.FundType && f.BANK_ACC_STATUS == true).First();
                }

                //get state /district information
                MASTER_DISTRICT district = new MASTER_DISTRICT();

                //Bank (State ) Login to get dist code from ND Code 30-June-2014
                if (PMGSYSession.Current.RoleCode == 10)
                {
                    district = dbContext.MASTER_DISTRICT.Where(v => v.MAST_DISTRICT_CODE == dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == masterDetails.ADMIN_ND_CODE).Select(s => s.MAST_DISTRICT_CODE).FirstOrDefault()).FirstOrDefault();
                }
                else
                {
                    district = dbContext.MASTER_DISTRICT.Where(v => v.MAST_DISTRICT_CODE == PMGSYSession.Current.DistrictCode).First();
                }
                int stateCode = district.MAST_STATE_CODE;
                string StateName = dbContext.MASTER_STATE.Where(v => v.MAST_STATE_CODE == stateCode).Select(c => c.MAST_STATE_NAME).First();

                model.EmailRecepient = emailMaster.EMAIL_TO; //dbContext.ADMIN_NODAL_OFFICERS.Where(a => a.ADMIN_ND_CODE == parentNdVode.Value && a.ADMIN_ACTIVE_STATUS == "Y").Select(f => f.ADMIN_NO_EMAIL).First(); ;

                model.DPIUName = district.MAST_DISTRICT_NAME;

                model.STATEName = StateName;

                model.EmailDate = commomFuncObj.GetDateTimeToString(emailMaster.EMAIL_SENT_DATE);

                //auth sig bank address   
                //model.Bankaddress = bankDetails.BANK_ADDRESS1 == null ? String.Empty : bankDetails.BANK_ADDRESS1;
                model.Bankaddress = (bankDetails.BANK_NAME == null ? String.Empty : bankDetails.BANK_NAME) + (bankDetails.BANK_ADDRESS1 == null ? String.Empty : "-" + bankDetails.BANK_ADDRESS1) + (bankDetails.BANK_ADDRESS2 == null ? String.Empty : ", " + bankDetails.BANK_ADDRESS2) + ", " + (bankDetails.MASTER_STATE.MAST_STATE_NAME);

                //auth sig bank acc number
                model.BankAcNumber = emailDetails.BANK_ACC_NO == null ? String.Empty : emailDetails.BANK_ACC_NO;

                model.EpayNumber = emailMaster.EPAY_NO;

                model.EpayDate = commomFuncObj.GetDateTimeToString(emailMaster.EPAY_DATE);

                model.EpayState = StateName;

                //model.EpayDPIU = district.MAST_DISTRICT_NAME;
                //Modified by Abhishek kamble to set DPIU name old it is dist name. 25-June-2014

                //Bank (State ) Login to get PIU Name from ND Code 30-June-2014
                if (PMGSYSession.Current.RoleCode == 10)
                {
                    model.EpayDPIU = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == masterDetails.ADMIN_ND_CODE).Select(s => s.ADMIN_ND_NAME).FirstOrDefault();
                }
                else
                {
                    model.EpayDPIU = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Select(s => s.ADMIN_ND_NAME).FirstOrDefault();
                }
                model.EpayVNumber = masterDetails.BILL_NO;

                model.EpayVDate = commomFuncObj.GetDateTimeToString(masterDetails.BILL_DATE);

                model.EpayVPackages = emailDetails.PKG_NO;

                model.BankEmail = bankDetails.BANK_EMAIL;

                model.EmailSubject = emailMaster.EMAIL_SUBJECT;

                model.StateCode = district.MASTER_STATE.MAST_STATE_SHORT_CODE;

                model.DPIUCode = district.MAST_DISTRICT_CODE.ToString();

                model.BankPdfPassword = bankDetails.Bank_SEC_CODE;
                //Modified by Abhishek kamble 10-Apr-2014
                model.DepartmentAcNum = emailMaster.DEPT_BANK_ACC_NO == String.Empty ? "-" : emailMaster.DEPT_BANK_ACC_NO;

                // get department name 
                model.DepartmentNameFull = dbContext.ACC_MASTER_REMIT_DEPT.Where(x => x.DEPT_ID == emailMaster.DEPT_ID).Select(x => x.DEPT_NAME).FirstOrDefault();

                model.DepartmentName = model.DepartmentNameFull.Remove(model.DepartmentNameFull.LastIndexOf("Department"));

                //Modified by Abhishek kamble 10-Apr-2014
                model.DPIUTANNumber = emailMaster.DPIU_TAN_NO == String.Empty ? "-" : emailMaster.DPIU_TAN_NO;

                // get the Contractor Details
                List<EremittnaceContractor> BillContractorList = new List<EremittnaceContractor>();

                List<ACC_EPAY_MAIL_DETAILS> emailDetailsList = new List<ACC_EPAY_MAIL_DETAILS>();

                emailDetailsList = dbContext.ACC_EPAY_MAIL_DETAILS.Where(c => c.EPAY_ID == emailMaster.EPAY_ID).ToList<ACC_EPAY_MAIL_DETAILS>();

                foreach (ACC_EPAY_MAIL_DETAILS bill in emailDetailsList)
                {

                    EremittnaceContractor Contractor = new EremittnaceContractor();

                    Contractor.EpayConName = bill.CON_NAME;
                    Contractor.EpayAmount = bill.EPAY_AMOUNT.ToString();
                    Contractor.EpayConPanNumber = bill.CON_PAN_NO;
                    Contractor.EpayConAggreement = bill.AGREEMENT_NO;
                    Contractor.EpayConAcNum = bill.CON_ACC_NO;
                    Contractor.EpayConBankName = bill.CON_BANK_NAME;
                    model.TotalAmount = model.TotalAmount + bill.EPAY_AMOUNT.Value;

                    BillContractorList.Add(Contractor);
                }

                model.ContractorList = BillContractorList;

                model.EpayTotalAmountInWord = objAmt.RupeesToWord(model.TotalAmount.ToString());
                return model;

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error while getting epayment details");
            }
            finally
            {
                dbContext.Dispose();
            }

        }


        /// <summary>
        /// DAL function to get the epayment details of which mail has been sent
        /// </summary>
        /// <param name="bill_id"></param>
        /// <returns></returns>
        public EpaymentOrderModel GetEpaymentDetailsFinalizedByAuthSig(Int64 bill_ID)
        {
            CommonFunctions commomFuncObj = new CommonFunctions();
            AmountToWord objAmt = new AmountToWord();

            try
            {
                dbContext = new PMGSYEntities();

                EpaymentOrderModel model = new EpaymentOrderModel();

                ACC_EPAY_MAIL_MASTER emailMaster = new ACC_EPAY_MAIL_MASTER();

                ACC_EPAY_MAIL_DETAILS emailDetails = new ACC_EPAY_MAIL_DETAILS();

                emailMaster = dbContext.ACC_EPAY_MAIL_MASTER.Where(c => c.BILL_ID == bill_ID && c.IS_EPAY_VALID == true).First();    //IS_EPAY_VALID =true Checked by Abhishek for Reject/Resend Epayment Details 25-Sep-2014

                emailDetails = dbContext.ACC_EPAY_MAIL_DETAILS.Where(c => c.EPAY_ID == emailMaster.EPAY_ID).First();

                int? parentNdVode = dbContext.ADMIN_DEPARTMENT.Where(b => b.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Select(d => d.MAST_PARENT_ND_CODE).First();

                //get payment master details
                ACC_BILL_MASTER masterDetails = new ACC_BILL_MASTER();
                masterDetails = dbContext.ACC_BILL_MASTER.Where(x => x.BILL_ID == bill_ID).First();


                //get authorized bank details
                ACC_BANK_DETAILS bankDetails = new ACC_BANK_DETAILS();
                if (parentNdVode.HasValue)
                {
                    //get the bank details as per bank account number in bill details
                    bankDetails = dbContext.ACC_BANK_DETAILS.Where(f => f.ADMIN_ND_CODE == parentNdVode.Value
                        && f.BANK_ACC_NO == emailDetails.BANK_ACC_NO && f.BANK_ACC_STATUS == true && f.FUND_TYPE == PMGSYSession.Current.FundType).First();
                }

                //get state /district information
                MASTER_DISTRICT district = new MASTER_DISTRICT();

                //Bank (State ) Login
                if (PMGSYSession.Current.RoleCode == 10)
                {
                    district = dbContext.MASTER_DISTRICT.Where(v => v.MAST_DISTRICT_CODE == dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == masterDetails.ADMIN_ND_CODE).Select(s => s.MAST_DISTRICT_CODE).FirstOrDefault()).FirstOrDefault();
                }
                else
                {
                    district = dbContext.MASTER_DISTRICT.Where(v => v.MAST_DISTRICT_CODE == PMGSYSession.Current.DistrictCode).FirstOrDefault();
                }

                int stateCode = district.MAST_STATE_CODE;
                string StateName = dbContext.MASTER_STATE.Where(v => v.MAST_STATE_CODE == stateCode).Select(c => c.MAST_STATE_NAME).First();

                model.EmailRecepient = emailMaster.EMAIL_TO;//dbContext.ADMIN_NODAL_OFFICERS.Where(a => a.ADMIN_ND_CODE == parentNdVode.Value && a.ADMIN_ACTIVE_STATUS == "Y").Select(f => f.ADMIN_NO_EMAIL).First(); ;

                model.DPIUName = district.MAST_DISTRICT_NAME;

                model.STATEName = StateName;

                model.EmailDate = commomFuncObj.GetDateTimeToString(emailMaster.EMAIL_SENT_DATE);

                //auth sig bank address   
                //model.Bankaddress = bankDetails.BANK_ADDRESS1 == null ? String.Empty : bankDetails.BANK_ADDRESS1;
                model.Bankaddress = (bankDetails.BANK_NAME == null ? String.Empty : bankDetails.BANK_NAME) + (bankDetails.BANK_ADDRESS1 == null ? String.Empty : "-" + bankDetails.BANK_ADDRESS1) + (bankDetails.BANK_ADDRESS2 == null ? String.Empty : ", " + bankDetails.BANK_ADDRESS2) + ", " + (bankDetails.MASTER_STATE.MAST_STATE_NAME);

                //auth sig bank acc number
                model.BankAcNumber = emailDetails.BANK_ACC_NO == null ? String.Empty : emailDetails.BANK_ACC_NO;

                model.EpayNumber = emailMaster.EPAY_NO;

                model.EpayDate = commomFuncObj.GetDateTimeToString(emailMaster.EPAY_DATE);

                model.EpayState = StateName;

                //model.EpayDPIU = district.MAST_DISTRICT_NAME;
                //Modified by Abhishek kamble to set DPIU name old it is dist name. 25-June-2014

                //Bank (State ) Login
                if (PMGSYSession.Current.RoleCode == 10)
                {
                    model.EpayDPIU = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == masterDetails.ADMIN_ND_CODE).Select(s => s.ADMIN_ND_NAME).FirstOrDefault();

                }
                else
                {
                    model.EpayDPIU = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Select(s => s.ADMIN_ND_NAME).FirstOrDefault();
                }

                model.EpayVNumber = masterDetails.BILL_NO;

                model.EpayVDate = commomFuncObj.GetDateTimeToString(masterDetails.BILL_DATE);

                model.EpayVPackages = emailDetails.PKG_NO;


                //Modified By Abhishek kamble 28-May-2014
                //model.EpayConName = emailDetails.CON_NAME;            

                // MASTER_CONTRACTOR_BANK con = new MASTER_CONTRACTOR_BANK();
                //ADMIN_NO_BANK  noBank = new ADMIN_NO_BANK(); 

                //if (masterDetails.TXN_ID == 472 || masterDetails.TXN_ID == 415)
                //{
                //    noBank = dbContext.ADMIN_NO_BANK.Where(v => v.ADMIN_NO_OFFICER_CODE == masterDetails.ADMIN_NO_OFFICER_CODE && v.MAST_ACCOUNT_STATUS == "A").FirstOrDefault();

                //}
                //else
                //{
                //    con = dbContext.MASTER_CONTRACTOR_BANK.Where(v => v.MAST_CON_ID == masterDetails.MAST_CON_ID && v.MAST_ACCOUNT_STATUS == "A" && v.MAST_DISTRICT_CODE == PMGSYSession.Current.DistrictCode).FirstOrDefault();
                //    if (con == null)
                //    {
                //        con = dbContext.MASTER_CONTRACTOR_BANK.Where(v => v.MAST_CON_ID == masterDetails.MAST_CON_ID && v.MAST_ACCOUNT_STATUS == "A").FirstOrDefault();

                //    }
                //}

                MASTER_CONTRACTOR mastContDetailsModel = new MASTER_CONTRACTOR();
                ADMIN_NODAL_OFFICERS AdminNDModel = new ADMIN_NODAL_OFFICERS();
                string contNameContCompanyName = string.Empty;

                //if (masterDetails.TXN_ID == 472 || masterDetails.TXN_ID == 415)
                //{
                //    AdminNDModel = dbContext.ADMIN_NODAL_OFFICERS.Where(c => c.ADMIN_NO_OFFICER_CODE == masterDetails.ADMIN_NO_OFFICER_CODE).FirstOrDefault();
                //    contNameContCompanyName = AdminNDModel.ADMIN_NO_FNAME + " " + AdminNDModel.ADMIN_NO_MNAME + " " + AdminNDModel.ADMIN_NO_LNAME;
                //    model.EpayConName = contNameContCompanyName;
                //}
                //else
                //{
                    mastContDetailsModel = dbContext.MASTER_CONTRACTOR.Where(c => c.MAST_CON_ID == masterDetails.MAST_CON_ID).FirstOrDefault();
                    if (mastContDetailsModel.MAST_CON_COMPANY_NAME != null)
                    {
                        contNameContCompanyName = mastContDetailsModel.MAST_CON_FNAME + " " + mastContDetailsModel.MAST_CON_MNAME + " " + mastContDetailsModel.MAST_CON_LNAME + " ( " + mastContDetailsModel.MAST_CON_COMPANY_NAME + " )";
                    }
                    else
                    {
                        contNameContCompanyName = mastContDetailsModel.MAST_CON_FNAME + " " + mastContDetailsModel.MAST_CON_MNAME + " " + mastContDetailsModel.MAST_CON_LNAME;
                    }
                    model.EpayConName = contNameContCompanyName;
                //}

                //Contractor legal heir details
                if (mastContDetailsModel.MAST_CON_EXPIRY_DATE != null && mastContDetailsModel.MAST_CON_STATUS == "E")
                {
                    model.EpayContLegalHeirName = mastContDetailsModel.MAST_CON_LEGAL_HEIR_FNAME + " " + mastContDetailsModel.MAST_CON_LEGAL_HEIR_MNAME + " " + mastContDetailsModel.MAST_CON_LEGAL_HEIR_LNAME;
                }


                List<Int32> lstBillDetails = new List<Int32>();
                lstBillDetails = dbContext.ACC_BILL_DETAILS.Where(c => c.BILL_ID == bill_ID && c.CASH_CHQ == "Q" && c.CREDIT_DEBIT == "D" && c.IMS_PR_ROAD_CODE != null).Select(f => f.IMS_PR_ROAD_CODE.Value).Distinct().ToList<Int32>();

                List<Int32> lstAgreement = new List<Int32>();
                lstAgreement = dbContext.ACC_BILL_DETAILS.Where(c => c.BILL_ID == bill_ID && c.CASH_CHQ == "Q" && c.CREDIT_DEBIT == "D" && c.IMS_AGREEMENT_CODE != null).Select(F => F.IMS_AGREEMENT_CODE.Value).Distinct().ToList<Int32>();

                String Packages = String.Empty;
                String strAgreement = String.Empty;

                if (lstBillDetails.Count != 0)
                {

                    foreach (int item in lstBillDetails)
                    {
                        string newPackage = dbContext.IMS_SANCTIONED_PROJECTS.Where(c => c.IMS_PR_ROAD_CODE == item).Select(x => x.IMS_PACKAGE_ID).First();
                        //if new package is not allready present in Packages then only add
                        //if (!Packages.Contains(newPackage))
                        //{
                        Packages = Packages + "," + newPackage;
                        //}

                    }

                    if (Packages != string.Empty)
                    {
                        if (Packages[0] == ',')
                        {
                            Packages = Packages.Substring(1);
                        }

                    }
                }

                if (lstAgreement.Count != 0)
                {
                    if (PMGSYSession.Current.FundType.Equals("P"))
                    {
                        foreach (int item in lstAgreement)
                        {
                            string newAgreement = dbContext.TEND_AGREEMENT_MASTER.Where(c => c.TEND_AGREEMENT_CODE == item).Select(x => x.TEND_AGREEMENT_NUMBER).First();
                            //if new package is not allready present in Packages then only add
                            //if (!strAgreement.Contains(newAgreement))
                            //{
                            strAgreement = strAgreement + "," + newAgreement;
                            //}
                        }
                        if (strAgreement != string.Empty)
                        {
                            if (strAgreement[0] == ',')
                            {
                                strAgreement = strAgreement.Substring(1);
                            }
                        }
                    }
                    else if (PMGSYSession.Current.FundType.Equals("M"))
                    {
                        foreach (int item in lstAgreement)
                        {
                            string newAgreement = dbContext.MANE_IMS_CONTRACT.Where(c => c.MANE_CONTRACT_ID == item).Select(x => x.MANE_AGREEMENT_NUMBER).First();
                            //if new package is not allready present in Packages then only add
                            if (!strAgreement.Contains(newAgreement))
                            {
                                strAgreement = strAgreement + "," + newAgreement;
                            }
                        }
                        if (strAgreement != string.Empty)
                        {
                            if (strAgreement[0] == ',')
                            {
                                strAgreement = strAgreement.Substring(1);
                            }
                        }
                    }

                }

                model.AgreementNumber = strAgreement;
                model.EpayVPackages = Packages;





                model.EpayConAcNum = emailDetails.CON_ACC_NO;

                model.EpayConBankName = emailDetails.CON_BANK_NAME;

                model.EpayConBankIFSCCode = emailDetails.CON_BANK_IFS_CODE;

                //model.EpayAmount = emailDetails.EPAY_AMOUNT.ToString();

                //model.EpayAmountInWord = objAmt.RupeesToWord(model.EpayAmount);

                model.EpayAmount = masterDetails.CHQ_AMOUNT.ToString();

                model.EpayAmountInWord = objAmt.RupeesToWord(model.EpayAmount);

                model.BankEmail = bankDetails.BANK_EMAIL;

                model.EmailSubject = "An Epayment transaction is made by DPIU of " + district.MAST_DISTRICT_NAME + " of " + StateName + "on https://omms.nic.in,Epayment No: " + emailMaster.EPAY_NO;

                model.StateCode = district.MASTER_STATE.MAST_STATE_SHORT_CODE;

                model.DPIUCode = district.MAST_DISTRICT_CODE.ToString();

                model.BankPdfPassword = bankDetails.Bank_SEC_CODE;

                return model;

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error while getting epayment details");
            }
            finally
            {
                dbContext.Dispose();
            }

        }

        /// <summary>
        /// function to list epayment details
        /// </summary>
        /// <param name="objFilter"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array ListEPaymentDetails(PaymentFilterModel objFilter, String TransactionType, out long totalRecords, string moduleType)
        {
            CommonFunctions commomFuncObj = new CommonFunctions();
            string description = string.Empty;
            bool reatEnabled = false;
            try
            {
                dbContext = new PMGSYEntities();
                List<ACC_BILL_MASTER> lstBillMaster = null;

                Int32? prmParentNdCode = PMGSYSession.Current.ParentNDCode;


                if (dbContext.REAT_OMMAS_PAYMENT_SUCCESS.Any(m => m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode))
                {

                    reatEnabled = true;
                }
                else
                {
                    reatEnabled = false;
                }

                //if (dbContext.REAT_OMMAS_PAYMENT_SUCCESS.Any(m => m.ADMIN_ND_CODE == prmParentNdCode))
                //{

                //    reatEnabled = true;
                //}
                //else
                //{
                //    reatEnabled = false;
                //}


                if (objFilter.FilterMode.Equals("view") && objFilter.BillId == 0)
                {
                    if (TransactionType == "E")
                    {
                        //lstBillMaster = dbContext.ACC_BILL_MASTER.                            
                        //    Where(m => m.ADMIN_ND_CODE == objFilter.AdminNdCode
                        //        && m.LVL_ID == objFilter.LevelId
                        //        && m.BILL_MONTH == objFilter.Month
                        //        && m.BILL_TYPE == objFilter.Bill_type
                        //        && m.CHQ_EPAY == "E"
                        //        && (m.BILL_FINALIZED == "Y")
                        //        && m.REMIT_TYPE == null
                        //        && m.FUND_TYPE == objFilter.FundType
                        //        && m.BILL_YEAR == objFilter.Year).ToList<ACC_BILL_MASTER>();

                        //Modified by Abhishek kamble 27Jan2015 to dont disp cancelled entry
                        lstBillMaster = (from bm in dbContext.ACC_BILL_MASTER
                                         join ci in dbContext.ACC_CHEQUES_ISSUED on bm.BILL_ID equals ci.BILL_ID
                                         where
                                        bm.ADMIN_ND_CODE == objFilter.AdminNdCode
                                           && bm.LVL_ID == objFilter.LevelId
                                           && bm.BILL_MONTH == objFilter.Month
                                           && bm.BILL_TYPE == objFilter.Bill_type
                                           && bm.CHQ_EPAY == "E"
                                           && (bm.BILL_FINALIZED == "Y")
                                           && bm.REMIT_TYPE == null
                                           && bm.FUND_TYPE == objFilter.FundType
                                           && bm.BILL_YEAR == objFilter.Year
                                           && ci.CHEQUE_STATUS != "C"
                                           && !(bm.TXN_ID == 3185 || bm.TXN_ID == 3171) //Added on 13-03-2023
                                         select bm
                                            ).ToList<ACC_BILL_MASTER>();


                    }
                    else if (TransactionType == "R")
                    {

                        //lstBillMaster = dbContext.ACC_BILL_MASTER.
                        //     Where(m => m.ADMIN_ND_CODE == objFilter.AdminNdCode
                        //         && m.LVL_ID == objFilter.LevelId
                        //         && m.BILL_MONTH == objFilter.Month
                        //         && m.BILL_TYPE == objFilter.Bill_type
                        //         && m.CHQ_EPAY == "E"
                        //         && (m.BILL_FINALIZED == "Y")
                        //         && m.REMIT_TYPE != null
                        //         && m.FUND_TYPE == objFilter.FundType
                        //         && m.BILL_YEAR == objFilter.Year).ToList<ACC_BILL_MASTER>();

                        //Modified by Abhishek kamble 27Jan2015 to dont disp cancelled entry
                        lstBillMaster = (from bm in dbContext.ACC_BILL_MASTER
                                         join ci in dbContext.ACC_CHEQUES_ISSUED on bm.BILL_ID equals ci.BILL_ID
                                         where
                                              bm.ADMIN_ND_CODE == objFilter.AdminNdCode
                                           && bm.LVL_ID == objFilter.LevelId
                                           && bm.BILL_MONTH == objFilter.Month
                                           && bm.BILL_TYPE == objFilter.Bill_type
                                           && bm.CHQ_EPAY == "E"
                                           && (bm.BILL_FINALIZED == "Y")
                                           && bm.REMIT_TYPE != null
                                           && bm.FUND_TYPE == objFilter.FundType
                                           && bm.BILL_YEAR == objFilter.Year
                                           && ci.CHEQUE_STATUS != "C"
                                           && !(bm.TXN_ID == 3185 || bm.TXN_ID == 3171) //Added on 13-03-2023
                                         select bm
                                            ).ToList<ACC_BILL_MASTER>();

                    }
                    else
                    {
                        lstBillMaster = (from bm in dbContext.ACC_BILL_MASTER
                                         join ci in dbContext.ACC_CHEQUES_ISSUED on bm.BILL_ID equals ci.BILL_ID
                                         where
                                              bm.ADMIN_ND_CODE == objFilter.AdminNdCode
                                           && bm.LVL_ID == objFilter.LevelId
                                           && bm.BILL_MONTH == objFilter.Month
                                           && bm.BILL_TYPE == objFilter.Bill_type
                                           && bm.CHQ_EPAY == "E"
                                           && (bm.BILL_FINALIZED == "Y")
                                           && bm.REMIT_TYPE == null
                                           && bm.FUND_TYPE == objFilter.FundType
                                           && bm.BILL_YEAR == objFilter.Year
                                           && ci.CHEQUE_STATUS != "C"
                                           && bm.TXN_ID == 3185 //Added on 13-03-2023
                                         select bm
                                           ).ToList<ACC_BILL_MASTER>();
                    }
                }

                else
                {
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


                    if (objFilter.ToDate == null && objFilter.FromDate != null)
                    {
                        if (objFilter.TransId != 0)
                        {
                            if (TransactionType == "E")
                            {
                                //lstBillMaster = dbContext.ACC_BILL_MASTER
                                //    .Where(m => m.ADMIN_ND_CODE == objFilter.AdminNdCode)
                                //    .Where(m => m.LVL_ID == objFilter.LevelId)
                                //    .Where(m => m.FUND_TYPE == objFilter.FundType)
                                //    .Where(m => m.BILL_DATE >= fromDate)
                                //    .Where(m => m.TXN_ID == objFilter.TransId)
                                //    .Where(m => m.BILL_TYPE == "P")
                                //    .Where(m => m.BILL_FINALIZED == "Y" || m.BILL_FINALIZED == "E")
                                //    .Where(m => m.CHQ_EPAY == "E")
                                //     .Where(m => m.REMIT_TYPE == null)
                                //    .ToList<ACC_BILL_MASTER>();

                                //Modified by Abhishek kamble 27Jan2015 to dont disp cancelled entry
                                lstBillMaster = (from bm in dbContext.ACC_BILL_MASTER
                                                 join ci in dbContext.ACC_CHEQUES_ISSUED on bm.BILL_ID equals ci.BILL_ID
                                                 where
                                                bm.ADMIN_ND_CODE == objFilter.AdminNdCode
                                                   && bm.LVL_ID == objFilter.LevelId
                                                   && bm.FUND_TYPE == objFilter.FundType
                                                   && bm.BILL_DATE >= fromDate
                                                   && bm.TXN_ID == objFilter.TransId
                                                   && bm.BILL_TYPE == "P"
                                                   && (bm.BILL_FINALIZED == "Y" || bm.BILL_FINALIZED == "E")
                                                   && bm.CHQ_EPAY == "E"
                                                   && bm.REMIT_TYPE == null
                                                   && ci.CHEQUE_STATUS != "C"
                                                   && !(bm.TXN_ID == 3185 || bm.TXN_ID == 3171) //Added on 13-03-2023
                                                 select bm
                                                    ).ToList<ACC_BILL_MASTER>();
                            }
                            else if (TransactionType == "R")
                            {
                                // lstBillMaster = dbContext.ACC_BILL_MASTER
                                //.Where(m => m.ADMIN_ND_CODE == objFilter.AdminNdCode)
                                //.Where(m => m.LVL_ID == objFilter.LevelId)
                                //.Where(m => m.FUND_TYPE == objFilter.FundType)
                                //.Where(m => m.BILL_DATE >= fromDate)
                                //.Where(m => m.TXN_ID == objFilter.TransId)
                                //.Where(m => m.BILL_TYPE == "P")
                                // .Where(m => m.BILL_FINALIZED == "Y" || m.BILL_FINALIZED == "E")
                                //.Where(m => m.CHQ_EPAY == "E")
                                // .Where(m => m.REMIT_TYPE != null)
                                //.ToList<ACC_BILL_MASTER>();

                                //Modified by Abhishek kamble 27Jan2015 to dont disp cancelled entry
                                lstBillMaster = (from bm in dbContext.ACC_BILL_MASTER
                                                 join ci in dbContext.ACC_CHEQUES_ISSUED on bm.BILL_ID equals ci.BILL_ID
                                                 where
                                                bm.ADMIN_ND_CODE == objFilter.AdminNdCode
                                                   && bm.LVL_ID == objFilter.LevelId
                                                   && bm.FUND_TYPE == objFilter.FundType
                                                   && bm.BILL_DATE >= fromDate
                                                   && bm.TXN_ID == objFilter.TransId
                                                   && bm.BILL_TYPE == "P"
                                                   && (bm.BILL_FINALIZED == "Y" || bm.BILL_FINALIZED == "E")
                                                   && bm.CHQ_EPAY == "E"
                                                   && bm.REMIT_TYPE != null
                                                   && ci.CHEQUE_STATUS != "C"
                                                   && !(bm.TXN_ID == 3185 || bm.TXN_ID == 3171) //Added on 13-03-2023
                                                 select bm
                                                    ).ToList<ACC_BILL_MASTER>();

                            }
                            else
                            {
                                lstBillMaster = (from bm in dbContext.ACC_BILL_MASTER
                                                 join ci in dbContext.ACC_CHEQUES_ISSUED on bm.BILL_ID equals ci.BILL_ID
                                                 where
                                                bm.ADMIN_ND_CODE == objFilter.AdminNdCode
                                                   && bm.LVL_ID == objFilter.LevelId
                                                   && bm.FUND_TYPE == objFilter.FundType
                                                   && bm.BILL_DATE >= fromDate
                                                   && bm.TXN_ID == objFilter.TransId
                                                   && bm.BILL_TYPE == "P"
                                                   && (bm.BILL_FINALIZED == "Y" || bm.BILL_FINALIZED == "E")
                                                   && bm.CHQ_EPAY == "E"
                                                   && bm.REMIT_TYPE == null
                                                   && ci.CHEQUE_STATUS != "C"
                                                    && bm.TXN_ID == 3185 //Added on 13-03-2023
                                                 select bm
                                                    ).ToList<ACC_BILL_MASTER>();
                            }
                        }
                    }
                    if (objFilter.ToDate == null && objFilter.FromDate != null)
                    {
                        if (objFilter.TransId == 0)
                        {
                            if (TransactionType == "E")
                            {
                                //lstBillMaster = dbContext.ACC_BILL_MASTER
                                //    .Where(m => m.ADMIN_ND_CODE == objFilter.AdminNdCode)
                                //    .Where(m => m.LVL_ID == objFilter.LevelId)
                                //    .Where(m => m.FUND_TYPE == objFilter.FundType)
                                //    .Where(m => m.BILL_DATE >= fromDate)
                                //    .Where(m => m.CHQ_EPAY == "E")
                                //     .Where(m => m.BILL_FINALIZED == "Y" || m.BILL_FINALIZED == "E")
                                //    .Where(m => m.BILL_TYPE == "P")
                                //     .Where(m => m.REMIT_TYPE == null)
                                //    .ToList<ACC_BILL_MASTER>();      

                                //Modified by Abhishek kamble 27Jan2015 to dont disp cancelled entry
                                lstBillMaster = (from bm in dbContext.ACC_BILL_MASTER
                                                 join ci in dbContext.ACC_CHEQUES_ISSUED on bm.BILL_ID equals ci.BILL_ID
                                                 where
                                                bm.ADMIN_ND_CODE == objFilter.AdminNdCode
                                                   && bm.LVL_ID == objFilter.LevelId
                                                   && bm.FUND_TYPE == objFilter.FundType
                                                   && bm.BILL_DATE >= fromDate
                                                   && bm.CHQ_EPAY == "E"
                                                   && (bm.BILL_FINALIZED == "Y" || bm.BILL_FINALIZED == "E")
                                                   && bm.BILL_TYPE == "P"
                                                   && bm.REMIT_TYPE == null
                                                   && ci.CHEQUE_STATUS != "C"
                                                   && !(bm.TXN_ID == 3185 || bm.TXN_ID == 3171) //Added on 13-03-2023
                                                 select bm
                                                    ).ToList<ACC_BILL_MASTER>();
                            }
                            else if (TransactionType == "R")
                            {

                                //lstBillMaster = dbContext.ACC_BILL_MASTER
                                //        .Where(m => m.ADMIN_ND_CODE == objFilter.AdminNdCode)
                                //        .Where(m => m.LVL_ID == objFilter.LevelId)
                                //        .Where(m => m.FUND_TYPE == objFilter.FundType)
                                //        .Where(m => m.BILL_DATE >= fromDate)
                                //        .Where(m => m.CHQ_EPAY == "E")
                                //        .Where(m => m.BILL_FINALIZED == "Y" || m.BILL_FINALIZED == "E")
                                //        .Where(m => m.BILL_TYPE == "P")
                                //         .Where(m => m.REMIT_TYPE != null)
                                //        .ToList<ACC_BILL_MASTER>();

                                //Modified by Abhishek kamble 27Jan2015 to dont disp cancelled entry
                                lstBillMaster = (from bm in dbContext.ACC_BILL_MASTER
                                                 join ci in dbContext.ACC_CHEQUES_ISSUED on bm.BILL_ID equals ci.BILL_ID
                                                 where
                                                bm.ADMIN_ND_CODE == objFilter.AdminNdCode
                                                   && bm.LVL_ID == objFilter.LevelId
                                                   && bm.FUND_TYPE == objFilter.FundType
                                                   && bm.BILL_DATE >= fromDate
                                                   && bm.CHQ_EPAY == "E"
                                                   && (bm.BILL_FINALIZED == "Y" || bm.BILL_FINALIZED == "E")
                                                   && bm.BILL_TYPE == "P"
                                                   && bm.REMIT_TYPE != null
                                                   && ci.CHEQUE_STATUS != "C"
                                                   && !(bm.TXN_ID == 3185 || bm.TXN_ID == 3171) //Added on 13-03-2023
                                                 select bm
                                                    ).ToList<ACC_BILL_MASTER>();

                            }
                            else
                            {
                                lstBillMaster = (from bm in dbContext.ACC_BILL_MASTER
                                                 join ci in dbContext.ACC_CHEQUES_ISSUED on bm.BILL_ID equals ci.BILL_ID
                                                 where
                                                bm.ADMIN_ND_CODE == objFilter.AdminNdCode
                                                   && bm.LVL_ID == objFilter.LevelId
                                                   && bm.FUND_TYPE == objFilter.FundType
                                                   && bm.BILL_DATE >= fromDate
                                                   && bm.CHQ_EPAY == "E"
                                                   && (bm.BILL_FINALIZED == "Y" || bm.BILL_FINALIZED == "E")
                                                   && bm.BILL_TYPE == "P"
                                                   && bm.REMIT_TYPE == null
                                                   && ci.CHEQUE_STATUS != "C"
                                                   && bm.TXN_ID == 3185 //Added on 13-03-2023
                                                 select bm
                                                    ).ToList<ACC_BILL_MASTER>();
                            }
                        }
                    }

                    else if (objFilter.ToDate != null && objFilter.FromDate == null)
                    {
                        if (objFilter.TransId == 0)
                        {
                            if (TransactionType == "E")
                            {
                                //lstBillMaster = dbContext.ACC_BILL_MASTER
                                //    .Where(m => m.ADMIN_ND_CODE == objFilter.AdminNdCode)
                                //    .Where(m => m.LVL_ID == objFilter.LevelId)
                                //    .Where(m => m.FUND_TYPE == objFilter.FundType)
                                //    .Where(m => m.BILL_DATE <= toDate)
                                //    .Where(m => m.BILL_TYPE == "P")
                                //     .Where(m => m.CHQ_EPAY == "E")
                                //     .Where(m => m.REMIT_TYPE == null)
                                //     .Where(m => m.BILL_FINALIZED == "Y" || m.BILL_FINALIZED == "E")
                                //    .ToList<ACC_BILL_MASTER>();

                                //Modified by Abhishek kamble 27Jan2015 to dont disp cancelled entry
                                lstBillMaster = (from bm in dbContext.ACC_BILL_MASTER
                                                 join ci in dbContext.ACC_CHEQUES_ISSUED on bm.BILL_ID equals ci.BILL_ID
                                                 where
                                                bm.ADMIN_ND_CODE == objFilter.AdminNdCode
                                                   && bm.LVL_ID == objFilter.LevelId
                                                   && bm.FUND_TYPE == objFilter.FundType
                                                   && bm.BILL_DATE <= toDate
                                                   && bm.BILL_TYPE == "P"
                                                   && bm.CHQ_EPAY == "E"
                                                   && bm.REMIT_TYPE == null
                                                   && (bm.BILL_FINALIZED == "Y" || bm.BILL_FINALIZED == "E")
                                                   && ci.CHEQUE_STATUS != "C"
                                                   && !(bm.TXN_ID == 3185 || bm.TXN_ID == 3171) //Added on 13-03-2023
                                                 select bm
                                                    ).ToList<ACC_BILL_MASTER>();

                            }
                            else if (TransactionType == "R")
                            {
                                //lstBillMaster = dbContext.ACC_BILL_MASTER
                                //        .Where(m => m.ADMIN_ND_CODE == objFilter.AdminNdCode)
                                //        .Where(m => m.LVL_ID == objFilter.LevelId)
                                //        .Where(m => m.FUND_TYPE == objFilter.FundType)
                                //        .Where(m => m.BILL_DATE <= toDate)
                                //        .Where(m => m.BILL_TYPE == "P")
                                //         .Where(m => m.CHQ_EPAY == "E")
                                //         .Where(m => m.REMIT_TYPE != null)
                                //         .Where(m => m.BILL_FINALIZED == "Y" || m.BILL_FINALIZED == "E")
                                //        .ToList<ACC_BILL_MASTER>();

                                //Modified by Abhishek kamble 27Jan2015 to dont disp cancelled entry
                                lstBillMaster = (from bm in dbContext.ACC_BILL_MASTER
                                                 join ci in dbContext.ACC_CHEQUES_ISSUED on bm.BILL_ID equals ci.BILL_ID
                                                 where
                                                bm.ADMIN_ND_CODE == objFilter.AdminNdCode
                                                   && bm.LVL_ID == objFilter.LevelId
                                                   && bm.FUND_TYPE == objFilter.FundType
                                                   && bm.BILL_DATE <= toDate
                                                   && bm.BILL_TYPE == "P"
                                                   && bm.CHQ_EPAY == "E"
                                                   && bm.REMIT_TYPE != null
                                                   && (bm.BILL_FINALIZED == "Y" || bm.BILL_FINALIZED == "E")
                                                   && ci.CHEQUE_STATUS != "C"
                                                   && !(bm.TXN_ID == 3185 || bm.TXN_ID == 3171) //Added on 13-03-2023
                                                 select bm
                                                    ).ToList<ACC_BILL_MASTER>();

                            }
                            else
                            {
                                lstBillMaster = (from bm in dbContext.ACC_BILL_MASTER
                                                 join ci in dbContext.ACC_CHEQUES_ISSUED on bm.BILL_ID equals ci.BILL_ID
                                                 where
                                                bm.ADMIN_ND_CODE == objFilter.AdminNdCode
                                                   && bm.LVL_ID == objFilter.LevelId
                                                   && bm.FUND_TYPE == objFilter.FundType
                                                   && bm.BILL_DATE <= toDate
                                                   && bm.BILL_TYPE == "P"
                                                   && bm.CHQ_EPAY == "E"
                                                   && bm.REMIT_TYPE == null
                                                   && (bm.BILL_FINALIZED == "Y" || bm.BILL_FINALIZED == "E")
                                                   && ci.CHEQUE_STATUS != "C"
                                                    && bm.TXN_ID == 3185 //Added on 13-03-2023
                                                 select bm
                                                   ).ToList<ACC_BILL_MASTER>();
                            }
                        }
                    }

                    if (objFilter.ToDate != null && objFilter.FromDate == null)
                    {
                        if (objFilter.TransId != 0)
                        {
                            if (TransactionType == "E")
                            {
                                //lstBillMaster = dbContext.ACC_BILL_MASTER
                                //    .Where(m => m.ADMIN_ND_CODE == objFilter.AdminNdCode)
                                //    .Where(m => m.LVL_ID == objFilter.LevelId)
                                //    .Where(m => m.FUND_TYPE == objFilter.FundType)
                                //    .Where(m => m.BILL_DATE <= toDate)
                                //     .Where(m => m.TXN_ID == objFilter.TransId)
                                //    .Where(m => m.BILL_TYPE == "P")
                                //    .Where(m => m.BILL_FINALIZED == "Y" || m.BILL_FINALIZED == "E")
                                //     .Where(m => m.CHQ_EPAY == "E")
                                //      .Where(m => m.REMIT_TYPE == null)
                                //    .ToList<ACC_BILL_MASTER>();

                                //Modified by Abhishek kamble 27Jan2015 to dont disp cancelled entry
                                lstBillMaster = (from bm in dbContext.ACC_BILL_MASTER
                                                 join ci in dbContext.ACC_CHEQUES_ISSUED on bm.BILL_ID equals ci.BILL_ID
                                                 where
                                                bm.ADMIN_ND_CODE == objFilter.AdminNdCode
                                                   && bm.LVL_ID == objFilter.LevelId
                                                   && bm.FUND_TYPE == objFilter.FundType
                                                   && bm.BILL_DATE <= toDate
                                                   && bm.TXN_ID == objFilter.TransId
                                                   && bm.BILL_TYPE == "P"
                                                   && (bm.BILL_FINALIZED == "Y" || bm.BILL_FINALIZED == "E")
                                                   && bm.CHQ_EPAY == "E"
                                                   && bm.REMIT_TYPE == null
                                                   && ci.CHEQUE_STATUS != "C"
                                                   && !(bm.TXN_ID == 3185 || bm.TXN_ID == 3171)//Added on 13-03-2023
                                                 select bm
                                                    ).ToList<ACC_BILL_MASTER>();
                            }
                            else if (TransactionType == "R")
                            {
                                //lstBillMaster = dbContext.ACC_BILL_MASTER
                                //        .Where(m => m.ADMIN_ND_CODE == objFilter.AdminNdCode)
                                //        .Where(m => m.LVL_ID == objFilter.LevelId)
                                //         .Where(m => m.FUND_TYPE == objFilter.FundType)
                                //        .Where(m => m.BILL_DATE <= toDate)
                                //         .Where(m => m.TXN_ID == objFilter.TransId)
                                //        .Where(m => m.BILL_TYPE == "P")
                                //        .Where(m => m.BILL_FINALIZED == "Y" || m.BILL_FINALIZED == "E")
                                //         .Where(m => m.CHQ_EPAY == "E")
                                //          .Where(m => m.REMIT_TYPE != null)
                                //        .ToList<ACC_BILL_MASTER>();

                                //Modified by Abhishek kamble 27Jan2015 to dont disp cancelled entry
                                lstBillMaster = (from bm in dbContext.ACC_BILL_MASTER
                                                 join ci in dbContext.ACC_CHEQUES_ISSUED on bm.BILL_ID equals ci.BILL_ID
                                                 where
                                                bm.ADMIN_ND_CODE == objFilter.AdminNdCode
                                                   && bm.LVL_ID == objFilter.LevelId
                                                   && bm.FUND_TYPE == objFilter.FundType
                                                   && bm.BILL_DATE <= toDate
                                                   && bm.TXN_ID == objFilter.TransId
                                                   && bm.BILL_TYPE == "P"
                                                   && (bm.BILL_FINALIZED == "Y" || bm.BILL_FINALIZED == "E")
                                                   && bm.CHQ_EPAY == "E"
                                                   && bm.REMIT_TYPE != null
                                                   && ci.CHEQUE_STATUS != "C"
                                                   && !(bm.TXN_ID == 3185 || bm.TXN_ID == 3171) //Added on 13-03-2023
                                                 select bm
                                                    ).ToList<ACC_BILL_MASTER>();
                            }
                            else
                            {
                                lstBillMaster = (from bm in dbContext.ACC_BILL_MASTER
                                                 join ci in dbContext.ACC_CHEQUES_ISSUED on bm.BILL_ID equals ci.BILL_ID
                                                 where
                                                bm.ADMIN_ND_CODE == objFilter.AdminNdCode
                                                   && bm.LVL_ID == objFilter.LevelId
                                                   && bm.FUND_TYPE == objFilter.FundType
                                                   && bm.BILL_DATE <= toDate
                                                   && bm.TXN_ID == objFilter.TransId
                                                   && bm.BILL_TYPE == "P"
                                                   && (bm.BILL_FINALIZED == "Y" || bm.BILL_FINALIZED == "E")
                                                   && bm.CHQ_EPAY == "E"
                                                   && bm.REMIT_TYPE == null
                                                   && ci.CHEQUE_STATUS != "C"
                                                   && bm.TXN_ID == 3185 //Added on 13-03-2023
                                                 select bm
                                                   ).ToList<ACC_BILL_MASTER>();
                            }
                        }
                    }

                    else if (objFilter.ToDate == null && objFilter.FromDate == null)
                    {
                        if (objFilter.TransId != 0)
                        {
                            if (TransactionType == "E")
                            {
                                //lstBillMaster = dbContext.ACC_BILL_MASTER
                                //    .Where(m => m.ADMIN_ND_CODE == objFilter.AdminNdCode)
                                //    .Where(m => m.LVL_ID == objFilter.LevelId)
                                //    .Where(m => m.FUND_TYPE == objFilter.FundType)
                                //    .Where(m => m.TXN_ID == objFilter.TransId)
                                //    .Where(m => m.BILL_TYPE == "P")
                                //     .Where(m => m.CHQ_EPAY == "E")
                                //     .Where(m => m.BILL_FINALIZED == "Y" || m.BILL_FINALIZED == "E")
                                //      .Where(m => m.REMIT_TYPE == null)
                                //    .ToList<ACC_BILL_MASTER>();

                                //Modified by Abhishek kamble 27Jan2015 to dont disp cancelled entry
                                lstBillMaster = (from bm in dbContext.ACC_BILL_MASTER
                                                 join ci in dbContext.ACC_CHEQUES_ISSUED on bm.BILL_ID equals ci.BILL_ID
                                                 where
                                                bm.ADMIN_ND_CODE == objFilter.AdminNdCode
                                                   && bm.LVL_ID == objFilter.LevelId
                                                   && bm.FUND_TYPE == objFilter.FundType
                                                   && bm.TXN_ID == objFilter.TransId
                                                   && bm.BILL_TYPE == "P"
                                                   && bm.CHQ_EPAY == "E"
                                                   && (bm.BILL_FINALIZED == "Y" || bm.BILL_FINALIZED == "E")
                                                   && bm.REMIT_TYPE == null
                                                   && ci.CHEQUE_STATUS != "C"
                                                   && !(bm.TXN_ID == 3185 || bm.TXN_ID == 3171)//Added on 13-03-2023
                                                 select bm
                                                    ).ToList<ACC_BILL_MASTER>();
                            }
                            else if (TransactionType == "R")
                            {
                                //lstBillMaster = dbContext.ACC_BILL_MASTER
                                //        .Where(m => m.ADMIN_ND_CODE == objFilter.AdminNdCode)
                                //        .Where(m => m.LVL_ID == objFilter.LevelId)
                                //        .Where(m => m.FUND_TYPE == objFilter.FundType)
                                //        .Where(m => m.TXN_ID == objFilter.TransId)
                                //        .Where(m => m.BILL_TYPE == "P")
                                //         .Where(m => m.CHQ_EPAY == "E")
                                //         .Where(m => m.BILL_FINALIZED == "Y" || m.BILL_FINALIZED == "E")
                                //          .Where(m => m.REMIT_TYPE != null)
                                //        .ToList<ACC_BILL_MASTER>();

                                //Modified by Abhishek kamble 27Jan2015 to dont disp cancelled entry
                                lstBillMaster = (from bm in dbContext.ACC_BILL_MASTER
                                                 join ci in dbContext.ACC_CHEQUES_ISSUED on bm.BILL_ID equals ci.BILL_ID
                                                 where
                                                bm.ADMIN_ND_CODE == objFilter.AdminNdCode
                                                   && bm.LVL_ID == objFilter.LevelId
                                                   && bm.FUND_TYPE == objFilter.FundType
                                                   && bm.TXN_ID == objFilter.TransId
                                                   && bm.BILL_TYPE == "P"
                                                   && bm.CHQ_EPAY == "E"
                                                   && (bm.BILL_FINALIZED == "Y" || bm.BILL_FINALIZED == "E")
                                                   && bm.REMIT_TYPE != null
                                                   && ci.CHEQUE_STATUS != "C"
                                                   && !(bm.TXN_ID == 3185 || bm.TXN_ID == 3171) //Added on 13-03-2023
                                                 select bm
                                                    ).ToList<ACC_BILL_MASTER>();
                            }
                            else
                            {
                                lstBillMaster = (from bm in dbContext.ACC_BILL_MASTER
                                                 join ci in dbContext.ACC_CHEQUES_ISSUED on bm.BILL_ID equals ci.BILL_ID
                                                 where
                                                bm.ADMIN_ND_CODE == objFilter.AdminNdCode
                                                   && bm.LVL_ID == objFilter.LevelId
                                                   && bm.FUND_TYPE == objFilter.FundType
                                                   && bm.TXN_ID == objFilter.TransId
                                                   && bm.BILL_TYPE == "P"
                                                   && bm.CHQ_EPAY == "E"
                                                   && (bm.BILL_FINALIZED == "Y" || bm.BILL_FINALIZED == "E")
                                                   && bm.REMIT_TYPE == null
                                                   && ci.CHEQUE_STATUS != "C"
                                                   && bm.TXN_ID == 3185 //Added on 13-03-2023
                                                 select bm
                                                   ).ToList<ACC_BILL_MASTER>();
                            }
                        }
                    }

                    if (objFilter.ToDate == null && objFilter.FromDate == null)
                    {
                        if (objFilter.TransId == 0)
                        {
                            if (TransactionType == "E")
                            {
                                //lstBillMaster = dbContext.ACC_BILL_MASTER
                                //    .Where(m => m.ADMIN_ND_CODE == objFilter.AdminNdCode)
                                //    .Where(m => m.LVL_ID == objFilter.LevelId)
                                //    .Where(m => m.FUND_TYPE == objFilter.FundType)
                                //    .Where(m => m.BILL_TYPE == "P")
                                //     .Where(m => m.CHQ_EPAY == "E")
                                //     .Where(m => m.BILL_FINALIZED == "Y" || m.BILL_FINALIZED == "E")
                                //      .Where(m => m.REMIT_TYPE == null)
                                //    .ToList<ACC_BILL_MASTER>();

                                //Modified by Abhishek kamble 27Jan2015 to dont disp cancelled entry
                                lstBillMaster = (from bm in dbContext.ACC_BILL_MASTER
                                                 join ci in dbContext.ACC_CHEQUES_ISSUED on bm.BILL_ID equals ci.BILL_ID
                                                 where
                                                bm.ADMIN_ND_CODE == objFilter.AdminNdCode
                                                   && bm.LVL_ID == objFilter.LevelId
                                                   && bm.FUND_TYPE == objFilter.FundType
                                                   && bm.BILL_TYPE == "P"
                                                   && bm.CHQ_EPAY == "E"
                                                   && (bm.BILL_FINALIZED == "Y" || bm.BILL_FINALIZED == "E")
                                                   && bm.REMIT_TYPE == null
                                                   && ci.CHEQUE_STATUS != "C"
                                                   && !(bm.TXN_ID == 3185 || bm.TXN_ID == 3171) //Added on 13-03-2023
                                                 select bm
                                                    ).ToList<ACC_BILL_MASTER>();
                            }
                            else if (TransactionType == "R")
                            {

                                //lstBillMaster = dbContext.ACC_BILL_MASTER
                                //        .Where(m => m.ADMIN_ND_CODE == objFilter.AdminNdCode)
                                //        .Where(m => m.LVL_ID == objFilter.LevelId)
                                //        .Where(m => m.FUND_TYPE == objFilter.FundType)
                                //        .Where(m => m.BILL_TYPE == "P")
                                //         .Where(m => m.CHQ_EPAY == "E")
                                //         .Where(m => m.BILL_FINALIZED == "Y" || m.BILL_FINALIZED == "E")
                                //          .Where(m => m.REMIT_TYPE != null)
                                //        .ToList<ACC_BILL_MASTER>();

                                //Modified by Abhishek kamble 27Jan2015 to dont disp cancelled entry
                                lstBillMaster = (from bm in dbContext.ACC_BILL_MASTER
                                                 join ci in dbContext.ACC_CHEQUES_ISSUED on bm.BILL_ID equals ci.BILL_ID
                                                 where
                                                bm.ADMIN_ND_CODE == objFilter.AdminNdCode
                                                   && bm.LVL_ID == objFilter.LevelId
                                                   && bm.FUND_TYPE == objFilter.FundType
                                                   && bm.BILL_TYPE == "P"
                                                   && bm.CHQ_EPAY == "E"
                                                   && (bm.BILL_FINALIZED == "Y" || bm.BILL_FINALIZED == "E")
                                                   && bm.REMIT_TYPE != null
                                                   && ci.CHEQUE_STATUS != "C"
                                                   && !(bm.TXN_ID == 3185 || bm.TXN_ID == 3171) //Added on 13-03-2023
                                                 select bm
                                                    ).ToList<ACC_BILL_MASTER>();

                            }
                            else
                            {
                                lstBillMaster = (from bm in dbContext.ACC_BILL_MASTER
                                                 join ci in dbContext.ACC_CHEQUES_ISSUED on bm.BILL_ID equals ci.BILL_ID
                                                 where
                                                bm.ADMIN_ND_CODE == objFilter.AdminNdCode
                                                   && bm.LVL_ID == objFilter.LevelId
                                                   && bm.FUND_TYPE == objFilter.FundType
                                                   && bm.BILL_TYPE == "P"
                                                   && bm.CHQ_EPAY == "E"
                                                   && (bm.BILL_FINALIZED == "Y" || bm.BILL_FINALIZED == "E")
                                                   && bm.REMIT_TYPE == null
                                                   && ci.CHEQUE_STATUS != "C"
                                                   && bm.TXN_ID == 3185 //Added on 13-03-2023
                                                 select bm
                                                    ).ToList<ACC_BILL_MASTER>();
                            }
                        }
                    }

                    else if (objFilter.ToDate != null && objFilter.FromDate != null)
                    {
                        if (objFilter.TransId != 0)
                        {
                            if (TransactionType == "E")
                            {
                                //lstBillMaster = dbContext.ACC_BILL_MASTER.Where(m => m.ADMIN_ND_CODE == objFilter.AdminNdCode)
                                //    .Where(m => m.LVL_ID == objFilter.LevelId)
                                //    .Where(m => m.FUND_TYPE == objFilter.FundType)
                                //    .Where(m => m.BILL_DATE >= fromDate)
                                //    .Where(m => m.TXN_ID == objFilter.TransId)
                                //     .Where(m => m.BILL_TYPE == "P")
                                //      .Where(m => m.CHQ_EPAY == "E")
                                //      .Where(m => m.BILL_FINALIZED == "Y" || m.BILL_FINALIZED == "E")
                                //       .Where(m => m.REMIT_TYPE == null)
                                //    .Where(m => m.BILL_DATE <= toDate).ToList<ACC_BILL_MASTER>();

                                //Modified by Abhishek kamble 27Jan2015 to dont disp cancelled entry
                                lstBillMaster = (from bm in dbContext.ACC_BILL_MASTER
                                                 join ci in dbContext.ACC_CHEQUES_ISSUED on bm.BILL_ID equals ci.BILL_ID
                                                 where
                                                bm.ADMIN_ND_CODE == objFilter.AdminNdCode
                                                   && bm.LVL_ID == objFilter.LevelId
                                                   && bm.FUND_TYPE == objFilter.FundType
                                                   && bm.BILL_DATE >= fromDate
                                                   && bm.TXN_ID == objFilter.TransId
                                                   && bm.BILL_TYPE == "P"
                                                   && bm.CHQ_EPAY == "E"
                                                   && (bm.BILL_FINALIZED == "Y" || bm.BILL_FINALIZED == "E")
                                                   && bm.REMIT_TYPE == null
                                                   && bm.BILL_DATE <= toDate
                                                   && ci.CHEQUE_STATUS != "C"
                                                   && !(bm.TXN_ID == 3185 || bm.TXN_ID == 3171) //Added on 13-03-2023
                                                 select bm
                                                    ).ToList<ACC_BILL_MASTER>();
                            }
                            else if (TransactionType == "R")
                            {
                                //lstBillMaster = dbContext.ACC_BILL_MASTER.Where(m => m.ADMIN_ND_CODE == objFilter.AdminNdCode)
                                //        .Where(m => m.LVL_ID == objFilter.LevelId)
                                //        .Where(m => m.FUND_TYPE == objFilter.FundType)
                                //        .Where(m => m.BILL_DATE >= fromDate)
                                //        .Where(m => m.TXN_ID == objFilter.TransId)
                                //         .Where(m => m.BILL_TYPE == "P")
                                //          .Where(m => m.CHQ_EPAY == "E")
                                //          .Where(m => m.BILL_FINALIZED == "Y" || m.BILL_FINALIZED == "E")
                                //           .Where(m => m.REMIT_TYPE != null)
                                //        .Where(m => m.BILL_DATE <= toDate).ToList<ACC_BILL_MASTER>();

                                //Modified by Abhishek kamble 27Jan2015 to dont disp cancelled entry
                                lstBillMaster = (from bm in dbContext.ACC_BILL_MASTER
                                                 join ci in dbContext.ACC_CHEQUES_ISSUED on bm.BILL_ID equals ci.BILL_ID
                                                 where
                                                bm.ADMIN_ND_CODE == objFilter.AdminNdCode
                                                   && bm.LVL_ID == objFilter.LevelId
                                                   && bm.FUND_TYPE == objFilter.FundType
                                                   && bm.BILL_DATE >= fromDate
                                                   && bm.TXN_ID == objFilter.TransId
                                                   && bm.BILL_TYPE == "P"
                                                   && bm.CHQ_EPAY == "E"
                                                   && (bm.BILL_FINALIZED == "Y" || bm.BILL_FINALIZED == "E")
                                                   && bm.REMIT_TYPE != null
                                                   && bm.BILL_DATE <= toDate
                                                   && ci.CHEQUE_STATUS != "C"
                                                   && !(bm.TXN_ID == 3185 || bm.TXN_ID == 3171) //Added on 13-03-2023
                                                 select bm
                                                    ).ToList<ACC_BILL_MASTER>();

                            }
                            else
                            {
                                lstBillMaster = (from bm in dbContext.ACC_BILL_MASTER
                                                 join ci in dbContext.ACC_CHEQUES_ISSUED on bm.BILL_ID equals ci.BILL_ID
                                                 where
                                                bm.ADMIN_ND_CODE == objFilter.AdminNdCode
                                                   && bm.LVL_ID == objFilter.LevelId
                                                   && bm.FUND_TYPE == objFilter.FundType
                                                   && bm.BILL_DATE >= fromDate
                                                   && bm.TXN_ID == objFilter.TransId
                                                   && bm.BILL_TYPE == "P"
                                                   && bm.CHQ_EPAY == "E"
                                                   && (bm.BILL_FINALIZED == "Y" || bm.BILL_FINALIZED == "E")
                                                   && bm.REMIT_TYPE == null
                                                   && bm.BILL_DATE <= toDate
                                                   && ci.CHEQUE_STATUS != "C"
                                                   && bm.TXN_ID == 3185 //Added on 13-03-2023
                                                 select bm
                                                   ).ToList<ACC_BILL_MASTER>();
                            }
                        }
                    }
                    else if (objFilter.ToDate != null && objFilter.FromDate != null)
                    {
                        if (objFilter.TransId == 0)
                        {
                            if (TransactionType == "E")
                            {
                                //lstBillMaster = dbContext.ACC_BILL_MASTER.Where(m => m.ADMIN_ND_CODE == objFilter.AdminNdCode)
                                //    .Where(m => m.LVL_ID == objFilter.LevelId)
                                //    .Where(m => m.FUND_TYPE == objFilter.FundType)
                                //    .Where(m => m.BILL_DATE >= fromDate)
                                //    .Where(m => m.BILL_DATE <= toDate)
                                //    .Where(m => m.BILL_TYPE == "P")
                                //     .Where(m => m.CHQ_EPAY == "E")
                                //     .Where(m => m.BILL_FINALIZED == "Y" || m.BILL_FINALIZED == "E")
                                //      .Where(m => m.REMIT_TYPE == null)
                                //    .ToList<ACC_BILL_MASTER>();

                                //Modified by Abhishek kamble 27Jan2015 to dont disp cancelled entry
                                lstBillMaster = (from bm in dbContext.ACC_BILL_MASTER
                                                 join ci in dbContext.ACC_CHEQUES_ISSUED on bm.BILL_ID equals ci.BILL_ID
                                                 where
                                                bm.ADMIN_ND_CODE == objFilter.AdminNdCode
                                                   && bm.LVL_ID == objFilter.LevelId
                                                   && bm.FUND_TYPE == objFilter.FundType
                                                   && bm.BILL_DATE >= fromDate
                                                   && bm.BILL_DATE <= toDate
                                                   && bm.BILL_TYPE == "P"
                                                   && bm.CHQ_EPAY == "E"
                                                   && (bm.BILL_FINALIZED == "Y" || bm.BILL_FINALIZED == "E")
                                                   && bm.REMIT_TYPE == null
                                                   && ci.CHEQUE_STATUS != "C"
                                                   && !(bm.TXN_ID == 3185 || bm.TXN_ID == 3171) //Added on 13-03-2023
                                                 select bm
                                                    ).ToList<ACC_BILL_MASTER>();
                            }
                            else if (TransactionType == "R")
                            {
                                //lstBillMaster = dbContext.ACC_BILL_MASTER.Where(m => m.ADMIN_ND_CODE == objFilter.AdminNdCode)
                                //        .Where(m => m.LVL_ID == objFilter.LevelId)
                                //        .Where(m => m.FUND_TYPE == objFilter.FundType)
                                //        .Where(m => m.BILL_DATE >= fromDate)
                                //        .Where(m => m.BILL_DATE <= toDate)
                                //        .Where(m => m.BILL_TYPE == "P")
                                //         .Where(m => m.CHQ_EPAY == "E")
                                //         .Where(m => m.BILL_FINALIZED == "Y" || m.BILL_FINALIZED == "E")
                                //          .Where(m => m.REMIT_TYPE != null)
                                //        .ToList<ACC_BILL_MASTER>();

                                //Modified by Abhishek kamble 27Jan2015 to dont disp cancelled entry
                                lstBillMaster = (from bm in dbContext.ACC_BILL_MASTER
                                                 join ci in dbContext.ACC_CHEQUES_ISSUED on bm.BILL_ID equals ci.BILL_ID
                                                 where
                                                bm.ADMIN_ND_CODE == objFilter.AdminNdCode
                                                   && bm.LVL_ID == objFilter.LevelId
                                                   && bm.FUND_TYPE == objFilter.FundType
                                                   && bm.BILL_DATE >= fromDate
                                                   && bm.BILL_DATE <= toDate
                                                   && bm.BILL_TYPE == "P"
                                                   && bm.CHQ_EPAY == "E"
                                                   && (bm.BILL_FINALIZED == "Y" || bm.BILL_FINALIZED == "E")
                                                   && bm.REMIT_TYPE != null
                                                   && ci.CHEQUE_STATUS != "C"
                                                   && !(bm.TXN_ID == 3185 || bm.TXN_ID == 3171) //Added on 13-03-2023
                                                 select bm
                                                    ).ToList<ACC_BILL_MASTER>();

                            }
                            else
                            {
                                lstBillMaster = (from bm in dbContext.ACC_BILL_MASTER
                                                 join ci in dbContext.ACC_CHEQUES_ISSUED on bm.BILL_ID equals ci.BILL_ID
                                                 where
                                                bm.ADMIN_ND_CODE == objFilter.AdminNdCode
                                                   && bm.LVL_ID == objFilter.LevelId
                                                   && bm.FUND_TYPE == objFilter.FundType
                                                   && bm.BILL_DATE >= fromDate
                                                   && bm.BILL_DATE <= toDate
                                                   && bm.BILL_TYPE == "P"
                                                   && bm.CHQ_EPAY == "E"
                                                   && (bm.BILL_FINALIZED == "Y" || bm.BILL_FINALIZED == "E")
                                                   && bm.REMIT_TYPE == null
                                                   && ci.CHEQUE_STATUS != "C"
                                                   && bm.TXN_ID == 3185 //Added on 13-03-2023
                                                 select bm
                                                   ).ToList<ACC_BILL_MASTER>();
                            }
                        }
                    }

                }


                totalRecords = lstBillMaster.Count();


                if (objFilter.sidx.Trim() != string.Empty)
                {
                    if (objFilter.sord.ToString() == "asc")
                    {
                        switch (objFilter.sidx)
                        {
                            case "Voucher_Number":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.BILL_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "voucher_date":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.BILL_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "Cash_Cheque":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.CHQ_EPAY).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "Transaction_type":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.ACC_MASTER_TXN.TXN_DESC).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "cheque_number":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.CHQ_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "cheque_Date":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.CHQ_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "cheque_amount":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.CHQ_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "Cash_Amount":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.CASH_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "Gross_Amount":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.GROSS_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            default:
                                lstBillMaster = lstBillMaster.OrderBy(x => x.BILL_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                        }

                    }
                    else
                    {
                        switch (objFilter.sidx)
                        {
                            case "Voucher_Number":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.BILL_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "voucher_date":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.BILL_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "Cash_Cheque":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.CHQ_EPAY).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "Transaction_type":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.ACC_MASTER_TXN.TXN_DESC).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "cheque_number":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.CHQ_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "cheque_Date":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.CHQ_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "cheque_amount":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.CHQ_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "Cash_Amount":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.CASH_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            case "Gross_Amount":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.GROSS_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                            default:
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.BILL_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                                break;
                        }
                    }
                }
                else
                {
                    lstBillMaster = lstBillMaster.OrderByDescending(x => x.BILL_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_MASTER>();
                }


                return lstBillMaster.Select(item => new
                {
                    id = URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() }),
                    cell = new[] {

                                        item.BILL_NO,
                                        commomFuncObj.GetDateTimeToString(item.BILL_DATE),
                                        item.CHQ_EPAY == null ? String.Empty : (item.CHQ_EPAY.Trim() == "C" ? "Cash" : (item.CHQ_EPAY.Trim() == "Q" ? "Cheque" : String.Empty)),
                                        item.ACC_MASTER_TXN.TXN_DESC.Trim(),
                                        item.CHQ_NO,
                                        item.CHQ_DATE == null ? String.Empty : commomFuncObj.GetDateTimeToString(item.CHQ_DATE.Value),
                                        item.PAYEE_NAME==null?"":item.PAYEE_NAME.ToString(),
                                        string.Empty,
                                        item.CHQ_AMOUNT.ToString(),
                                        item.CASH_AMOUNT.ToString(),
                                        item.GROSS_AMOUNT.ToString(),

                                        item.BILL_FINALIZED=="Y" &&  !dbContext.ACC_EPAY_MAIL_MASTER.Where(c=>c.BILL_ID == item.BILL_ID && c.IS_EPAY_VALID==true).Any()
                                        ? item.REMIT_TYPE==null


                                        ?  "<center><a href='#' class='ui-icon ui-icon-pencil' onclick='UnlockPayment(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()})+ "\");return false;'>Unlock</a></center>" :"<center><a href='#' class='ui-icon ui-icon-pencil' onclick='UnlockEremittancePayment(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()})+ "\");return false;'>Unlock</a></center>": ""   ,
                                     
                                     
                                      
                                        //item.BILL_FINALIZED=="Y" &&  !dbContext.ACC_EPAY_MAIL_MASTER.Where(c=>c.BILL_ID == item.BILL_ID && c.IS_EPAY_VALID==true).Any() ? item.REMIT_TYPE==null ? "<center><a href='#' class='ui-icon ui-icon-mail-closed' onclick='SignEPayment(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() +  "$"+  ( dbContext.ACC_EPAY_MAIL_MASTER.Where(c=>c.BILL_ID == item.BILL_ID && c.IS_EPAY_VALID==true).Any() ? "Y":"N") +"$E$" + item.BILL_FINALIZED.ToString().Trim() })+ "\");return false;'>Sign Epayment</a></center>" :"<center><a href='#' class='ui-icon ui-icon-mail-closed' onclick='SignERem(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() +  "$"+  ( dbContext.ACC_EPAY_MAIL_MASTER.Where(c=>c.BILL_ID == item.BILL_ID && c.IS_EPAY_VALID==true).Any() ? "Y":"N") + "$R$"  + item.BILL_FINALIZED.ToString().Trim()})+ "\");return false;'>Sign</a></center>": ""  ,  

                                
                                      item.BILL_FINALIZED=="Y" &&  !dbContext.ACC_EPAY_MAIL_MASTER.Where(c=>c.BILL_ID == item.BILL_ID && c.IS_EPAY_VALID==true).Any()
                                        ? item.REMIT_TYPE==null
                                            ? ((PMGSYSession.Current.FundType == "P" && !(item.TXN_ID == 3185 || item.TXN_ID == 3187) ) || PMGSYSession.Current.FundType == "A"  ) //Modified on 26-10-2021
                                             ? ((reatEnabled == false) && (moduleType.Equals("D"))  )
                                                ?  "<center><a href='#' class='ui-icon ui-icon-mail-closed' onclick='SignEPaymentXml(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() +  "$"+  (dbContext.ACC_EPAY_MAIL_MASTER.Where(c=>c.BILL_ID == item.BILL_ID && c.IS_EPAY_VALID==true).Any() ? "Y":"N") +"$E$" + item.BILL_FINALIZED.ToString().Trim() +
                                                        "$"+ item.MAST_CON_ID + "$"+ item.CON_ACCOUNT_ID })+ "\");return false;'>Sign Epayment -DBT </a></center>"
                                                :"-"
                                              //:"<center><a href='#' class='ui-icon ui-icon-mail-closed' onclick='SignEPayment(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() +  "$"+ (dbContext.ACC_EPAY_MAIL_MASTER.Where(c=>c.BILL_ID == item.BILL_ID && c.IS_EPAY_VALID==true).Any() ? "Y":"N") +"$E$" + item.BILL_FINALIZED.ToString().Trim() })+ "\");return false;'>Sign Epayment</a></center>" 
                                              :(PMGSYSession.Current.FundType == "P" && (item.TXN_ID == 3185 || item.TXN_ID == 3187) && reatEnabled == true && moduleType.Equals("D")) ? "-":"<center><a href='#' class='ui-icon ui-icon-mail-closed' onclick='SignEPayment(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() +  "$"+ (dbContext.ACC_EPAY_MAIL_MASTER.Where(c=>c.BILL_ID == item.BILL_ID && c.IS_EPAY_VALID==true).Any() ? "Y":"N") +"$E$" + item.BILL_FINALIZED.ToString().Trim() })+ "\");return false;'>Sign Epayment</a></center>"
                                            : "<center><a href='#' class='ui-icon ui-icon-mail-closed' onclick='SignERem(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() +  "$"+  ( dbContext.ACC_EPAY_MAIL_MASTER.Where(c=>c.BILL_ID == item.BILL_ID && c.IS_EPAY_VALID==true).Any() ? "Y":"N") + "$R$"  + item.BILL_FINALIZED.ToString().Trim()})+ "\");return false;'>Sign</a></center>"
                                        : ""  ,

                                         item.BILL_FINALIZED=="Y" &&  !dbContext.ACC_EPAY_MAIL_MASTER.Where(c=>c.BILL_ID == item.BILL_ID && c.IS_EPAY_VALID==true).Any()
                                        ? item.REMIT_TYPE==null
                                            ? ((PMGSYSession.Current.FundType == "P" && !(item.TXN_ID == 3185 || item.TXN_ID == 3187) ) || PMGSYSession.Current.FundType == "A"  ) //Modified on 26-10-2021
                                             ? (reatEnabled == true  && (moduleType.Equals("R"))   )
                                                ? "<center><a href='#' class='ui-icon ui-icon-mail-closed' onclick='SignEPaymentREATXml(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() +  "$"+   (dbContext.ACC_EPAY_MAIL_MASTER.Where(c=>c.BILL_ID == item.BILL_ID && c.IS_EPAY_VALID==true).Any() ? "Y":"N") +"$E$" + item.BILL_FINALIZED.ToString().Trim() +
                                                        "$"+ item.MAST_CON_ID + "$"+ item.CON_ACCOUNT_ID })+ "\");return false;'>Sign Epayment -REAT </a></center>"  : "-"
                                                //:"<center><a href='#' class='ui-icon ui-icon-mail-closed' onclick='SignEPayment(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() +  "$"+   (dbContext.ACC_EPAY_MAIL_MASTER.Where(c=>c.BILL_ID == item.BILL_ID && c.IS_EPAY_VALID==true).Any() ? "Y":"N") +"$E$" + item.BILL_FINALIZED.ToString().Trim() })+ "\");return false;'>Sign Epayment</a></center>" 
                                                :(PMGSYSession.Current.FundType == "P" && (item.TXN_ID == 3185 || item.TXN_ID == 3187) && reatEnabled == true && moduleType.Equals("D")) ? "-":"<center><a href='#' class='ui-icon ui-icon-mail-closed' onclick='SignEPayment(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() +  "$"+   (dbContext.ACC_EPAY_MAIL_MASTER.Where(c=>c.BILL_ID == item.BILL_ID && c.IS_EPAY_VALID==true).Any() ? "Y":"N") +"$E$" + item.BILL_FINALIZED.ToString().Trim() })+ "\");return false;'>Sign Epayment</a></center>"
                                            :"<center><a href='#' class='ui-icon ui-icon-mail-closed' onclick='SignERem(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() +  "$"+  ( dbContext.ACC_EPAY_MAIL_MASTER.Where(c=>c.BILL_ID == item.BILL_ID && c.IS_EPAY_VALID==true).Any() ? "Y":"N") + "$R$"  + item.BILL_FINALIZED.ToString().Trim()})+ "\");return false;'>Sign</a></center>"
                                        : ""  ,

                                         item.CHQ_EPAY=="E"? 
                                         //(item.REMIT_TYPE==null ? "<center><a href='#' class='ui-icon ui-icon-document' onclick='ViewEpayOrder(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()  + "$"+  ( dbContext.ACC_EPAY_MAIL_MASTER.Where(c=>c.BILL_ID == item.BILL_ID && c.IS_EPAY_VALID==true).Any() ? "Y":"N") +"$E"  }) + "\",\"" + createRandomNumber() + "\");return false;'>View & finalize Epayment</a></center>"
                                         //:"<center><a href='#' class='ui-icon ui-icon-document' onclick='ViewEremOrder(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() + "$"+  ( dbContext.ACC_EPAY_MAIL_MASTER.Where(c=>c.BILL_ID == item.BILL_ID && c.IS_EPAY_VALID==true).Any() ? "Y":"N") + "$R"  })+ "\",\""+ createRandomNumber()+ "\");return false;'>View & finalize Eremittance</a></center>")
                                         //:string.Empty,
                                         
                                       (item.REMIT_TYPE==null ? "<center><a href='#' class='ui-icon ui-icon-document' onclick='ViewEpayOrder(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()  + "$"+  ( dbContext.ACC_EPAY_MAIL_MASTER.Where(c=>c.BILL_ID == item.BILL_ID && c.IS_EPAY_VALID==true).Any() ? "Y":"N") +"$E$" + item.BILL_FINALIZED.ToString().Trim() + "$" + ((item.CON_ACCOUNT_ID != null) ? item.CON_ACCOUNT_ID.Value : 0) }) +  "\");return false;'>View & finalize Epayment</a></center>"
                                         :"<center><a href='#' class='ui-icon ui-icon-document' onclick='ViewEremOrder(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() + "$"+  ( dbContext.ACC_EPAY_MAIL_MASTER.Where(c=>c.BILL_ID == item.BILL_ID && c.IS_EPAY_VALID==true).Any() ? "Y":"N") + "$R$"  + item.BILL_FINALIZED.ToString().Trim() })+ "\");return false;'>View & finalize Eremittance</a></center>")
                                         :string.Empty,
                                      item.BILL_TYPE    ,
                                      GetPFMSStatus(item.BILL_ID, out description),
                                        description
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

        //public string createRandomNumber()
        //{
        //    System.Random rng = new Random(DateTime.Now.Millisecond);
        //    // Create random string
        //    byte[] salt = new byte[64];
        //    for (int i = 0; i < 64; )
        //    {
        //        salt[i++] = (byte)rng.Next(65, 90); // a-z
        //        //  salt[i++] = (byte)rng.Next(97, 122); // A-Z
        //    }
        //    string challenge = string.Empty;
        //    challenge = BytesToHexString(salt);
        //    return challenge.Substring(0, 8);
        //}

        //public  string BytesToHexString(byte[] input)
        //{
        //    StringBuilder hexString = new StringBuilder(64);
        //    for (int i = 0; i < input.Length; i++)
        //    {
        //        hexString.Append(String.Format("{0:X2}", input[i]));
        //    }
        //    return hexString.ToString();
        //}


        /// <summary>
        /// function to unlock the Epayment Voucher
        /// </summary>
        /// <param name="bill_ID"></param>
        /// <returns>
        /// 1 succsses
        /// -1 unfinalized voucher</returns>
        public String UnlockEpayment(Int64 bill_ID)
        {
            CommonFunctions commomFuncObj = new CommonFunctions();
            try
            {
                dbContext = new PMGSYEntities();
                ACC_BILL_MASTER old_model = new ACC_BILL_MASTER();
                //check if payment is already done for Epayment (consult madam)
                old_model = dbContext.ACC_BILL_MASTER.Where(x => x.BILL_ID == bill_ID).FirstOrDefault();

                //check if nor finalized 
                if (old_model.BILL_FINALIZED != "Y")
                {
                    return "-1";
                }

                //check if already authorization issued for selected epay

                if (dbContext.ACC_EPAY_MAIL_MASTER.Where(x => x.BILL_ID == bill_ID && x.IS_EPAY_VALID == true).Any())  //IS_EPAY_VALID =true Checked by Abhishek for Reject/Resend Epayment Details 25-Sep-2014
                {
                    return "-2";
                }

                if (dbContext.ACC_RPT_MONTHWISE_SUMMARY.Where(x => x.ADMIN_ND_CODE == old_model.ADMIN_ND_CODE
                   && x.ACC_MONTH == old_model.BILL_MONTH && x.ACC_YEAR == old_model.BILL_YEAR && x.FUND_TYPE == PMGSYSession.Current.FundType).Any())
                {
                    return "-222";  //month is closed
                }

                if (dbContext.ACC_EPAY_MAIL_MASTER.Where(x => x.BILL_ID == bill_ID).Any())
                {
                    //var objResend = dbContext.ACC_EPAY_MAIL_MASTER.Where(x => x.BILL_ID == bill_ID  );

                    var objResend = from mailMaster in dbContext.ACC_EPAY_MAIL_MASTER
                                    where mailMaster.BILL_ID.Equals(bill_ID)
                                    select new
                                    {
                                        mailMaster.EPAY_ID
                                    };

                    foreach (var item in objResend)
                    {
                        //if (dbContext.ACC_EPAY_MAIL_RESEND_DETAILS.Where(x => x.OLD_EPAY_ID == item.EPAY_ID).Any())
                        ///Changed by SAMMED A. PATIL on 06MAR2018 for pbBhatinda issue to definalize epayment after resend
                        if (dbContext.ACC_EPAY_MAIL_RESEND_DETAILS.Where(x => x.OLD_EPAY_ID == item.EPAY_ID && x.FLAG_DR == "C").Any())
                        {
                            return "-3";

                        }

                    }

                }

                #region update the record


                old_model.BILL_FINALIZED = "N";

                //Added by Abhishek kamble 29-nov-2013
                old_model.USERID = PMGSYSession.Current.UserId;
                old_model.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                dbContext.Entry(old_model).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                return "1";
                #endregion


            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "PaymentDAL.UnlockEpayment()");
                throw new Exception("Error while unlocking Epayment");
            }
            finally
            {
                dbContext.Dispose();
            }

        }

        /// <summary>
        /// for encoding the password 
        /// </summary>
        /// <param name="OriginalPasswordWithSalt"></param>
        /// <returns></returns>
        public string EncodePassword(string OriginalPasswordWithSalt)
        {
            //Declarations
            Byte[] OriginalBytes;
            Byte[] EncodedBytes;
            MD5 md5;

            //Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)
            md5 = new MD5CryptoServiceProvider();
            OriginalBytes = ASCIIEncoding.Default.GetBytes(OriginalPasswordWithSalt);
            EncodedBytes = md5.ComputeHash(OriginalBytes);

            //Convert encoded bytes back to a 'readable' string           
            StringBuilder hashCode = new StringBuilder(32);

            foreach (byte b in EncodedBytes)
                hashCode.Append(b.ToString("x2").ToUpper());

            return hashCode.ToString();
        }

        /// <summary>
        /// function to Finalize voucher Payment by authorized signatory
        /// </summary>
        /// <param name="bill_ID"></param>
        /// <returns>
        /// -1 password does not match
        /// -2 payment already finalized & auth given
        /// 1 password matches
        /// </returns>
        public string FinalizeEpayment(Int64 bill_ID, string DblHasedPassword)
        {
            CommonFunctions commomFuncObj = new CommonFunctions();
            try
            {
                dbContext = new PMGSYEntities();
                string authCode = string.Empty;
                //check if bank authorization has already been given
                if (dbContext.ACC_EPAY_MAIL_MASTER.Where(x => x.BILL_ID == bill_ID && x.IS_EPAY_VALID == true).Any())  //IS_EPAY_VALID =true Checked by Abhishek for Reject/Resend Epayment Details 25-Sep-2014
                {
                    return "-2";
                }

                // check if bill date is equal to finalization date 
                //Commented By Abhishek kamble 23-May-2014
                //DateTime billDate = dbContext.ACC_BILL_MASTER.Where(x => x.BILL_ID == bill_ID).Select(x => x.BILL_DATE).FirstOrDefault();
                //if (commomFuncObj.GetStringToDateTime(commomFuncObj.GetDateTimeToString(billDate)) != commomFuncObj.GetStringToDateTime(commomFuncObj.GetDateTimeToString(DateTime.Now)))
                //{
                //    return "-3";
                //}

                //get the authorized signatory password 
                //  int? parentNdVode = dbContext.ADMIN_DEPARTMENT.Where(b => b.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Select(d => d.MAST_PARENT_ND_CODE).First();

                // string authCode = dbContext.ADMIN_NODAL_OFFICERS.Where(a => a.ADMIN_ND_CODE == parentNdVode.Value && a.ADMIN_ACTIVE_STATUS == "Y" && a.ADMIN_MODULE=="A").Select(f => f.ADMIN_AUTH_CODE).First();
                authCode = dbContext.ADMIN_NODAL_OFFICERS.Where(a => a.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && a.ADMIN_ACTIVE_STATUS == "Y" && a.ADMIN_MODULE == "A").Select(f => f.ADMIN_AUTH_CODE).First();

                if (authCode != null && authCode != String.Empty)
                {
                    if (!(DblHasedPassword.Equals("e10adc3949ba59abbe56e057f20f883e")))
                    {
                        string doubleEncPwdr = EncodePassword(DblHasedPassword);

                        if (authCode.ToLower().Equals(DblHasedPassword.ToLower()))
                        {
                            return "1";
                        }
                        else
                        {
                            return "-1";
                        }
                    }
                    else
                    {
                        return "1";
                    }
                }
                else
                {
                    throw new Exception("Authorization code does not present");
                }

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error while Finalizing Epayment");
            }
            finally
            {
                dbContext.Dispose();
            }

        }

        /// <summary>
        /// Function to finalize the eremittance
        /// </summary>
        /// <param name="bill_ID"></param>
        /// <param name="DblHasedPassword"></param>
        /// <returns></returns>
        public String FinalizeEremittance(Int64 bill_ID, string DblHasedPassword)
        {
            CommonFunctions commomFuncObj = new CommonFunctions();
            try
            {
                dbContext = new PMGSYEntities();

                //check if bank authorization has already been given
                if (dbContext.ACC_EPAY_MAIL_MASTER.Where(x => x.BILL_ID == bill_ID && x.IS_EPAY_VALID == true).Any())   //IS_EPAY_VALID =true Checked by Abhishek for Reject/Resend Epayment Details 25-Sep-2014
                {
                    return "-2";
                }

                // check if bill date is equal to finalization date 

                //Commented By Abhishek kamble 23-May-2014
                //DateTime billDate = dbContext.ACC_BILL_MASTER.Where(x => x.BILL_ID == bill_ID).Select(x => x.BILL_DATE).FirstOrDefault();
                //if (commomFuncObj.GetStringToDateTime(commomFuncObj.GetDateTimeToString(billDate)) != commomFuncObj.GetStringToDateTime(commomFuncObj.GetDateTimeToString(DateTime.Now)))
                //{
                //    return "-3";
                //}
                //get the authorized signatory password 
                // int? parentNdVode = dbContext.ADMIN_DEPARTMENT.Where(b => b.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Select(d => d.MAST_PARENT_ND_CODE).First();

                // if (parentNdVode.HasValue)
                //{

                string authCode = dbContext.ADMIN_NODAL_OFFICERS.Where(a => a.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && a.ADMIN_ACTIVE_STATUS == "Y" && a.ADMIN_MODULE == "A").Select(f => f.ADMIN_AUTH_CODE).First();

                if (authCode != null && authCode != "")
                {
                    if (!(DblHasedPassword.Equals("e10adc3949ba59abbe56e057f20f883e")))
                    {
                        string doubleEncPwdr = EncodePassword(DblHasedPassword);

                        if (authCode == DblHasedPassword.ToUpper())
                        {
                            return "1";
                        }
                        else
                        {
                            return "-1";
                        }
                    }
                    else
                    {
                        return "1";
                    }
                    //if (authCode.Equals(DblHasedPassword))
                    //{
                    //    return "1";
                    //}
                    //else
                    //{
                    //    return "-1";
                    //}

                }
                else
                {
                    throw new Exception("Authorization code does not present");

                }
                //}
                //else
                //{
                //    throw new Exception("Error while getting authorized signatory details");
                //}
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error while Finalizing Epayment");
            }
            finally
            {
                dbContext.Dispose();
            }

        }
        /// <summary>
        /// function to insert the epay mail details 
        /// </summary>
        /// <param name="bill_ID"></param>
        /// <returns></returns>
        public string InsertEpaymentMailDetails(Int64 bill_ID, String FileName)
        {
            CommonFunctions commomFuncObj = new CommonFunctions();
            dbContext = new PMGSYEntities();
            try
            {
                using (var scope = new TransactionScope())
                {
                    int? parentNdVode = dbContext.ADMIN_DEPARTMENT.Where(b => b.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Select(d => d.MAST_PARENT_ND_CODE).First();

                    //get payment master details
                    ACC_BILL_MASTER masterDetails = new ACC_BILL_MASTER();

                    masterDetails = dbContext.ACC_BILL_MASTER.Where(x => x.BILL_ID == bill_ID).First();

                    //get package details
                    List<Int32> lstBillDetails = new List<Int32>();

                    lstBillDetails = dbContext.ACC_BILL_DETAILS.Where(c => c.BILL_ID == bill_ID && c.CASH_CHQ == "Q" && c.CREDIT_DEBIT == "D" && c.IMS_PR_ROAD_CODE != null).Select(f => f.IMS_PR_ROAD_CODE.Value).Distinct().ToList<Int32>();

                    String Packages = String.Empty;

                    if (lstBillDetails != null && lstBillDetails.Count() != 0)
                    {
                        foreach (int item in lstBillDetails)
                        {
                            Packages = Packages + "," + dbContext.IMS_SANCTIONED_PROJECTS.Where(c => c.IMS_PR_ROAD_CODE == item).Select(x => x.IMS_PACKAGE_ID).First();
                        }

                        if (Packages != string.Empty)
                        {
                            if (Packages[0] == ',')
                            {
                                Packages = Packages.Substring(1);
                            }

                        }
                    }
                    //get the agreement details 
                    int? agreementCode = dbContext.ACC_BILL_DETAILS.Where(c => c.BILL_ID == bill_ID && c.CREDIT_DEBIT == "D" && c.CASH_CHQ == "Q").First().IMS_AGREEMENT_CODE;

                    // String AgreementNo = dbContext.TEND_AGREEMENT_MASTER.Where(d => d.TEND_AGREEMENT_CODE == agreementCode.Value).Select(t => t.TEND_AGREEMENT_NUMBER).First();

                    //old
                    //String AgreementNo = masterDetails.FUND_TYPE == "M" ? (dbContext.MANE_IMS_CONTRACT.Where(c => c.MANE_PR_CONTRACT_CODE == (agreementCode.HasValue ? agreementCode.Value : -1)).Select(x => x.MANE_AGREEMENT_NUMBER).FirstOrDefault()) : (dbContext.TEND_AGREEMENT_MASTER.Where(c => c.TEND_AGREEMENT_CODE == (agreementCode.HasValue ? agreementCode.Value : -1)).Select(y => y.TEND_AGREEMENT_NUMBER).FirstOrDefault());
                    //Modified by Abhishek kamble to get Agreement No Using Mane_CONTRACTOR_ID 17Nov2014
                    String AgreementNo = masterDetails.FUND_TYPE == "M" ? (dbContext.MANE_IMS_CONTRACT.Where(c => c.MANE_CONTRACT_ID == (agreementCode.HasValue ? agreementCode.Value : -1)).Select(x => x.MANE_AGREEMENT_NUMBER).FirstOrDefault()) : (dbContext.TEND_AGREEMENT_MASTER.Where(c => c.TEND_AGREEMENT_CODE == (agreementCode.HasValue ? agreementCode.Value : -1)).Select(y => y.TEND_AGREEMENT_NUMBER).FirstOrDefault());

                    //get contractor details
                    MASTER_CONTRACTOR_BANK con = new MASTER_CONTRACTOR_BANK();
                    ACC_BANK_DETAILS secDepBankDetails = new ACC_BANK_DETAILS();//Added on 15-03-2023
                    ADMIN_NO_BANK admNoBank = new ADMIN_NO_BANK();
                    //if (masterDetails.TXN_ID == 472 || masterDetails.TXN_ID == 415)
                    //{
                    //    admNoBank = dbContext.ADMIN_NO_BANK.Where(v => v.ADMIN_NO_OFFICER_CODE == masterDetails.ADMIN_NO_OFFICER_CODE && v.MAST_ACCOUNT_STATUS == "A").FirstOrDefault();
                    //}
                    //else
                    //{

                    //Below code commented on 15-03-2023
                    //con = dbContext.MASTER_CONTRACTOR_BANK.Where(v => v.MAST_CON_ID == masterDetails.MAST_CON_ID && v.MAST_ACCOUNT_STATUS == "A" && v.MAST_DISTRICT_CODE == PMGSYSession.Current.DistrictCode).FirstOrDefault();
                    //if (con == null)
                    //{
                    //    con = dbContext.MASTER_CONTRACTOR_BANK.Where(v => v.MAST_CON_ID == masterDetails.MAST_CON_ID && v.MAST_ACCOUNT_STATUS == "A").FirstOrDefault();

                    //}

                    if (masterDetails.TXN_ID == 3185)
                    {
                        //Credit Bank
                        secDepBankDetails = dbContext.ACC_BANK_DETAILS.Where(f => f.FUND_TYPE == PMGSYSession.Current.FundType && f.BANK_ACC_STATUS == true && f.MAST_STATE_CODE == PMGSYSession.Current.StateCode && f.ACCOUNT_TYPE == "D").FirstOrDefault();
                    }
                    else
                    {
                        con = dbContext.MASTER_CONTRACTOR_BANK.Where(v => v.MAST_CON_ID == masterDetails.MAST_CON_ID && v.MAST_ACCOUNT_STATUS == "A" && v.MAST_DISTRICT_CODE == PMGSYSession.Current.DistrictCode).FirstOrDefault();
                        if (con == null)
                        {
                            con = dbContext.MASTER_CONTRACTOR_BANK.Where(v => v.MAST_CON_ID == masterDetails.MAST_CON_ID && v.MAST_ACCOUNT_STATUS == "A").FirstOrDefault();

                        }
                    }

                    //}

                    //get authorized bank details
                    ACC_BANK_DETAILS bankDetails = new ACC_BANK_DETAILS();

                    //Below condition commented on 20-03-2023
                    //if (parentNdVode.HasValue)
                    //{
                    //    //Below Line Added on 13-12-2021
                    //    //bankDetails = dbContext.ACC_BANK_DETAILS.Where(f => f.ADMIN_ND_CODE == parentNdVode.Value && f.FUND_TYPE == PMGSYSession.Current.FundType && f.BANK_ACC_STATUS == true).First();

                    //    //Below Line Added on 13-12-2021
                    //    bankDetails = ((masterDetails.TXN_ID == 3050 || masterDetails.TXN_ID == 3051) && PMGSYSession.Current.FundType == "M") || (PMGSYSession.Current.FundType != "M") ?
                    //    dbContext.ACC_BANK_DETAILS.Where(f => f.ADMIN_ND_CODE == parentNdVode.Value && f.FUND_TYPE == "P" && f.BANK_ACC_STATUS == true).First() :
                    //    dbContext.ACC_BANK_DETAILS.Where(f => f.ADMIN_ND_CODE == parentNdVode.Value && f.FUND_TYPE == "M" && f.BANK_ACC_STATUS == true).First();

                    //}

                    //Below condition Added on 20-03-2023
                    if (masterDetails.TXN_ID != 3185)
                    {
                        if (parentNdVode.HasValue)
                        {
                            //Below Line Added on 13-12-2021
                            //bankDetails = dbContext.ACC_BANK_DETAILS.Where(f => f.ADMIN_ND_CODE == parentNdVode.Value && f.FUND_TYPE == PMGSYSession.Current.FundType && f.BANK_ACC_STATUS == true).First();

                            if (masterDetails.TXN_ID == 3187)
                            {
                                bankDetails = dbContext.ACC_BANK_DETAILS.Where(f => f.ADMIN_ND_CODE == parentNdVode.Value && f.FUND_TYPE == PMGSYSession.Current.FundType && f.ACCOUNT_TYPE == "S" && f.BANK_ACC_STATUS == true).Any() ? dbContext.ACC_BANK_DETAILS.Where(f => f.ADMIN_ND_CODE == parentNdVode.Value && f.FUND_TYPE == PMGSYSession.Current.FundType && f.ACCOUNT_TYPE == "S" && f.BANK_ACC_STATUS == true).First() : bankDetails;
                            }
                            else
                            {
                                //Below Line Added on 13-12-2021
                                //***--***
                                bankDetails = ((masterDetails.TXN_ID == 3050 || masterDetails.TXN_ID == 3051) && PMGSYSession.Current.FundType == "M") || (PMGSYSession.Current.FundType != "M") ?
                                dbContext.ACC_BANK_DETAILS.Where(f => f.ADMIN_ND_CODE == parentNdVode.Value && f.FUND_TYPE == "P" && f.BANK_ACC_STATUS == true && f.ACCOUNT_TYPE == "S").First() :
                                dbContext.ACC_BANK_DETAILS.Where(f => f.ADMIN_ND_CODE == parentNdVode.Value && f.FUND_TYPE == "M" && f.BANK_ACC_STATUS == true && f.ACCOUNT_TYPE == "S").First();

                            }

                        }
                    }
                    else
                    {
                        if (parentNdVode.HasValue)
                        {
                            //Debit Bank Details
                            //Below Line Added on 13-12-2021
                            //bankDetails = dbContext.ACC_BANK_DETAILS.Where(f => f.ADMIN_ND_CODE == parentNdVode.Value && f.FUND_TYPE == PMGSYSession.Current.FundType && f.BANK_ACC_STATUS == true).First();

                            //Below Line Added on 13-12-2021
                            //bankDetails = dbContext.ACC_BANK_DETAILS.Where(f => f.ADMIN_ND_CODE == parentNdVode.Value && f.FUND_TYPE == PMGSYSession.Current.FundType && f.ACCOUNT_TYPE=="H" && f.BANK_ACC_STATUS == true ).First();
                            bankDetails = dbContext.ACC_BANK_DETAILS.Where(f => f.ADMIN_ND_CODE == parentNdVode.Value && f.FUND_TYPE == PMGSYSession.Current.FundType && f.ACCOUNT_TYPE == "H" && f.BANK_ACC_STATUS == true).Any() ? dbContext.ACC_BANK_DETAILS.Where(f => f.ADMIN_ND_CODE == parentNdVode.Value && f.FUND_TYPE == PMGSYSession.Current.FundType && f.ACCOUNT_TYPE == "H" && f.BANK_ACC_STATUS == true).First() : bankDetails;

                        }

                    }


                    //get state /district information
                    MASTER_DISTRICT district = new MASTER_DISTRICT();

                    district = dbContext.MASTER_DISTRICT.Where(v => v.MAST_DISTRICT_CODE == PMGSYSession.Current.DistrictCode).First();

                    int stateCode = district.MAST_STATE_CODE;

                    string StateName = dbContext.MASTER_STATE.Where(v => v.MAST_STATE_CODE == stateCode).Select(c => c.MAST_STATE_NAME).First();
                    string PiuName = dbContext.ADMIN_DEPARTMENT.Where(v => v.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Select(c => c.ADMIN_ND_NAME).First();

                    ACC_EPAY_MAIL_MASTER EpayModel = new ACC_EPAY_MAIL_MASTER();

                    long maxEPAY_ID = 0;

                    if (dbContext.ACC_EPAY_MAIL_MASTER.Any())
                    {
                        maxEPAY_ID = dbContext.ACC_EPAY_MAIL_MASTER.Max(c => c.EPAY_ID);
                    }

                    maxEPAY_ID = maxEPAY_ID + 1;

                    EpayModel.EPAY_ID = maxEPAY_ID;

                    EpayModel.BILL_ID = bill_ID;

                    EpayModel.EPAY_NO = masterDetails.CHQ_NO;

                    EpayModel.EPAY_MONTH = (byte)masterDetails.BILL_MONTH;

                    EpayModel.EPAY_YEAR = masterDetails.BILL_YEAR;

                    if (masterDetails.CHQ_DATE.HasValue)
                    {
                        EpayModel.EPAY_DATE = masterDetails.CHQ_DATE.Value;
                    }

                    EpayModel.EMAIL_FROM = "omms.pmgsy@nic.in";

                    //Below line commented on 21-03-2023
                    //EpayModel.EMAIL_TO = bankDetails.BANK_EMAIL;

                    //Below Code added on 21-03-2023
                    if (masterDetails.TXN_ID == 3185)
                    {
                        EpayModel.EMAIL_TO = secDepBankDetails.BANK_EMAIL;
                    }
                    else
                    {
                        EpayModel.EMAIL_TO = bankDetails.BANK_EMAIL;

                    }

                    EpayModel.EMAIL_SUBJECT = " An Epayment transaction is made by DPIU of " + PiuName + " ( " + district.MAST_DISTRICT_NAME + " ) of " + StateName + "on https://omms.nic.in,Epayment No: " + masterDetails.CHQ_NO;

                    //Below line commented on 21-03-2023
                    //EpayModel.EMAIL_CC = "";
                    //Below Code Added on 21-03-2023
                    if (masterDetails.TXN_ID == 3185)
                    {

                        string SRRDAMailId = dbContext.ADMIN_DEPARTMENT.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.ParentNDCode).Select(x => x.ADMIN_ND_EMAIL).FirstOrDefault();
                        if (SRRDAMailId != null && SRRDAMailId != String.Empty)
                        {
                            EpayModel.EMAIL_CC = bankDetails.BANK_EMAIL + "," + SRRDAMailId;

                        }
                        else
                        {
                            EpayModel.EMAIL_CC = bankDetails.BANK_EMAIL;

                        }
                    }
                    else
                    {
                        EpayModel.EMAIL_CC = "";
                    }

                    //EpayModel.EMAIL_CC = dbContext.ADMIN_DEPARTMENT.Where(m => m.MAST_STATE_CODE == PMGSYSession.Current.StateCode && m.MAST_ND_TYPE == "S").Select(m=>m.ADMIN_ND_EMAIL).FirstOrDefault();

                    EpayModel.EMAIL_BCC = "omms.pmgsy@nic.in";

                    EpayModel.EMAIL_SENT_DATE = DateTime.Now;

                    EpayModel.IS_EPAY_VALID = true;

                    EpayModel.REQUEST_IP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    EpayModel.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;

                    EpayModel.LVL_ID = (byte)PMGSYSession.Current.LevelId;

                    EpayModel.EPAY_EREMITTANCE = "E";

                    EpayModel.DEPT_BANK_ACC_NO = String.Empty;

                    EpayModel.DPIU_TAN_NO = String.Empty;

                    //File Name Added By Abhishek kamble for Resend/Reject mail Details 25 Sep 2014
                    EpayModel.FILE_NAME = FileName;

                    //Added By Abhishek kamble 29-nov-2013
                    EpayModel.USERID = PMGSYSession.Current.UserId;
                    EpayModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    dbContext.ACC_EPAY_MAIL_MASTER.Add(EpayModel);

                    //insert the details into [ACC_EPAY_MAIL_DETAILS]

                    long maxDetail_ID = 0;

                    if (dbContext.ACC_EPAY_MAIL_MASTER.Any())
                    {
                        maxDetail_ID = dbContext.ACC_EPAY_MAIL_DETAILS.Max(c => c.DETAIL_ID);
                    }

                    ACC_EPAY_MAIL_DETAILS EpayDetailsModel = new ACC_EPAY_MAIL_DETAILS();

                    maxDetail_ID = maxDetail_ID + 1;

                    EpayDetailsModel.DETAIL_ID = maxDetail_ID;

                    EpayDetailsModel.EPAY_ID = maxEPAY_ID;

                    EpayDetailsModel.BANK_ACC_NO = bankDetails.BANK_ACC_NO;

                    EpayDetailsModel.BILL_NO = masterDetails.BILL_NO;

                    EpayDetailsModel.BILL_DATE = masterDetails.BILL_DATE;

                    EpayDetailsModel.AGREEMENT_NO = AgreementNo;

                    EpayDetailsModel.PKG_NO = Packages;

                    EpayDetailsModel.CON_NAME = masterDetails.PAYEE_NAME;


                    //modified by abhishek kamble
                    //EpayDetailsModel.CON_ACC_NO = con.MAST_ACCOUNT_NUMBER;

                    //if (masterDetails.TXN_ID == 472 || masterDetails.TXN_ID == 415)
                    //{
                    //    EpayDetailsModel.CON_ACC_NO = (admNoBank == null ? "" : admNoBank.MAST_ACCOUNT_NUMBER);
                    //    EpayDetailsModel.CON_BANK_NAME = (admNoBank == null ? "" : admNoBank.MAST_BANK_NAME);
                    //    EpayDetailsModel.CON_BANK_IFS_CODE = (admNoBank == null ? "" : admNoBank.MAST_IFSC_CODE);
                    //}
                    //else
                    //{
                    //Below Code Commented on 15-03-2023
                    //EpayDetailsModel.CON_ACC_NO = (con == null ? "" : con.MAST_ACCOUNT_NUMBER);
                    //EpayDetailsModel.CON_BANK_NAME = (con == null ? "" : con.MAST_BANK_NAME);
                    //EpayDetailsModel.CON_BANK_IFS_CODE = (con == null ? "" : con.MAST_IFSC_CODE);

                    //Below Code Added on 15-03-2023
                    if (masterDetails.TXN_ID == 3185)
                    {
                        EpayDetailsModel.CON_ACC_NO = (secDepBankDetails == null ? "" : secDepBankDetails.BANK_ACC_NO);
                        EpayDetailsModel.CON_BANK_NAME = (secDepBankDetails == null ? "" : secDepBankDetails.BANK_NAME);
                        EpayDetailsModel.CON_BANK_IFS_CODE = (secDepBankDetails == null ? "" : secDepBankDetails.MAST_IFSC_CODE);
                    }
                    else
                    {
                        EpayDetailsModel.CON_ACC_NO = (con == null ? "" : con.MAST_ACCOUNT_NUMBER);
                        EpayDetailsModel.CON_BANK_NAME = (con == null ? "" : con.MAST_BANK_NAME);
                        EpayDetailsModel.CON_BANK_IFS_CODE = (con == null ? "" : con.MAST_IFSC_CODE);
                    }

                    //}
                    //EpayDetailsModel.EPAY_AMOUNT = masterDetails.GROSS_AMOUNT;

                    //Modified By Abhishek kamble 15-July-2014 Change to Cheque Amount from Gross Amount
                    EpayDetailsModel.EPAY_AMOUNT = masterDetails.CHQ_AMOUNT;


                    EpayDetailsModel.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;

                    EpayDetailsModel.LVL_ID = (byte)PMGSYSession.Current.LevelId;

                    EpayDetailsModel.CON_PAN_NO = dbContext.MASTER_CONTRACTOR.Where(c => c.MAST_CON_ID == masterDetails.MAST_CON_ID).Select(d => d.MAST_CON_PAN).FirstOrDefault();

                    //Added By Abhishek kamble 29-nov-2013
                    EpayDetailsModel.USERID = PMGSYSession.Current.UserId;
                    EpayDetailsModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    dbContext.ACC_EPAY_MAIL_DETAILS.Add(EpayDetailsModel);

                    //set finalization status as "Y" in bill master
                    ACC_BILL_MASTER acc_bill_master = dbContext.ACC_BILL_MASTER.Find(bill_ID);

                    if (acc_bill_master.CHQ_EPAY == "E")
                    {
                        //new change done by Vikram on 10-08-2013
                        acc_bill_master.BILL_FINALIZED = "Y";
                        //end of change
                        //acc_bill_master.BILL_FINALIZED = "E";
                    }
                    else
                    {
                        acc_bill_master.BILL_FINALIZED = "Y";
                    }
                    acc_bill_master.ACTION_REQUIRED = "N";

                    //(Flag Modified) Added By Abhishek kamble 29-nov-2013
                    acc_bill_master.USERID = PMGSYSession.Current.UserId;
                    acc_bill_master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    dbContext.Entry(acc_bill_master).State = System.Data.Entity.EntityState.Modified;

                    dbContext.SaveChanges();

                    scope.Complete();

                    return "1";

                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error while Finalizing Epayment");
            }
            finally
            {
                dbContext.Dispose();
            }

        }

        /// <summary>
        /// function to insert eremiitance mail details 
        /// </summary>
        /// <param name="bill_ID"></param>
        /// <returns></returns>
        public String InsertEremittanceMailDetails(Int64 bill_ID, String FileName)
        {
            CommonFunctions commomFuncObj = new CommonFunctions();
            dbContext = new PMGSYEntities();
            try
            {
                using (var scope = new TransactionScope())
                {
                    int? parentNdVode = dbContext.ADMIN_DEPARTMENT.Where(b => b.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Select(d => d.MAST_PARENT_ND_CODE).First();

                    //get payment master details
                    ACC_BILL_MASTER masterDetails = new ACC_BILL_MASTER();

                    masterDetails = dbContext.ACC_BILL_MASTER.Where(x => x.BILL_ID == bill_ID).First();

                    //get package details
                    List<Int32> lstBillDetails = new List<Int32>();

                    lstBillDetails = dbContext.ACC_BILL_DETAILS.Where(c => c.BILL_ID == bill_ID && c.CASH_CHQ == "Q" && c.CREDIT_DEBIT == "D" && c.IMS_PR_ROAD_CODE != null).Select(f => f.IMS_PR_ROAD_CODE.Value).Distinct().ToList<Int32>();

                    String Packages = String.Empty;

                    if (lstBillDetails != null && lstBillDetails.Count() != 0)
                    {
                        foreach (int item in lstBillDetails)
                        {
                            Packages = Packages + "," + dbContext.IMS_SANCTIONED_PROJECTS.Where(c => c.IMS_PR_ROAD_CODE == item).Select(x => x.IMS_PACKAGE_ID).First();
                        }

                        if (Packages != string.Empty)
                        {
                            if (Packages[0] == ',')
                            {
                                Packages = Packages.Substring(1);
                            }

                        }
                    }

                    //get the agreement details 
                    // int? agreementCode = dbContext.ACC_BILL_DETAILS.Where(c => c.BILL_ID == bill_ID && c.CREDIT_DEBIT == "D" && c.CASH_CHQ == "Q").First().IMS_AGREEMENT_CODE;

                    // String AgreementNo = dbContext.TEND_AGREEMENT_MASTER.Where(d => d.TEND_AGREEMENT_CODE == agreementCode.Value).Select(t => t.TEND_AGREEMENT_NUMBER).First();



                    //get authorized bank details
                    ACC_BANK_DETAILS bankDetails = new ACC_BANK_DETAILS();

                    if (parentNdVode.HasValue)
                    {
                        bankDetails = dbContext.ACC_BANK_DETAILS.Where(f => f.ADMIN_ND_CODE == parentNdVode.Value && f.BANK_ACC_STATUS == true && f.FUND_TYPE == PMGSYSession.Current.FundType && f.ACCOUNT_TYPE == "S").First();
                    }

                    //get state /district information
                    MASTER_DISTRICT district = new MASTER_DISTRICT();

                    district = dbContext.MASTER_DISTRICT.Where(v => v.MAST_DISTRICT_CODE == PMGSYSession.Current.DistrictCode).First();
                    string PiuName = dbContext.ADMIN_DEPARTMENT.Where(v => v.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Select(c => c.ADMIN_ND_NAME).First();


                    int stateCode = district.MAST_STATE_CODE;

                    string StateName = dbContext.MASTER_STATE.Where(v => v.MAST_STATE_CODE == stateCode).Select(c => c.MAST_STATE_NAME).First();


                    ACC_EPAY_MAIL_MASTER EpayModel = new ACC_EPAY_MAIL_MASTER();

                    long maxEPAY_ID = 0;

                    if (dbContext.ACC_EPAY_MAIL_MASTER.Any())
                    {
                        maxEPAY_ID = dbContext.ACC_EPAY_MAIL_MASTER.Max(c => c.EPAY_ID);
                    }

                    maxEPAY_ID = maxEPAY_ID + 1;

                    EpayModel.EPAY_ID = maxEPAY_ID;

                    EpayModel.BILL_ID = bill_ID;

                    EpayModel.EPAY_NO = masterDetails.CHQ_NO;

                    EpayModel.EPAY_MONTH = (byte)masterDetails.BILL_MONTH;

                    EpayModel.EPAY_YEAR = masterDetails.BILL_YEAR;

                    if (masterDetails.CHQ_DATE.HasValue)
                    {
                        EpayModel.EPAY_DATE = masterDetails.CHQ_DATE.Value;
                    }

                    EpayModel.EMAIL_FROM = "omms.pmgsy@nic.in";

                    EpayModel.EMAIL_TO = bankDetails.BANK_EMAIL;


                    EpayModel.EMAIL_SUBJECT = "An e-Remittance transaction is made by " + PiuName + " (" + district.MAST_DISTRICT_NAME + ") of " + StateName + "on https://omms.nic.in,E-Remittance No: " + masterDetails.CHQ_NO;

                    EpayModel.EMAIL_CC = "";

                    EpayModel.EMAIL_BCC = "omms.pmgsy@nic.in";

                    EpayModel.EMAIL_SENT_DATE = DateTime.Now;

                    EpayModel.IS_EPAY_VALID = true;

                    EpayModel.REQUEST_IP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    EpayModel.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;

                    EpayModel.LVL_ID = (byte)PMGSYSession.Current.LevelId;

                    EpayModel.EPAY_EREMITTANCE = "R";

                    EpayModel.DEPT_ID = masterDetails.REMIT_TYPE;

                    EpayModel.DPIU_TAN_NO = dbContext.ADMIN_DEPARTMENT.Where(b => b.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Select(x => x.ADMIN_ND_TAN_NO).FirstOrDefault();

                    EpayModel.DPIU_TAN_NO = EpayModel.DPIU_TAN_NO == null ? string.Empty : EpayModel.DPIU_TAN_NO;

                    EpayModel.DEPT_BANK_ACC_NO = dbContext.ACC_REM_ACCOUNT_DETAILS.Where(x => x.MAST_STATE_CODE == stateCode && x.REM_TYPE == masterDetails.REMIT_TYPE).Select(x => x.BANK_ACCOUNT_NO).FirstOrDefault() != null ? dbContext.ACC_REM_ACCOUNT_DETAILS.Where(x => x.MAST_STATE_CODE == stateCode && x.REM_TYPE == masterDetails.REMIT_TYPE).Select(x => x.BANK_ACCOUNT_NO).FirstOrDefault().ToString() : String.Empty;

                    //File Name Added By Abhishek kamble for Resend/Reject mail Details 25 Sep 2014
                    EpayModel.FILE_NAME = FileName;

                    // Added By Abhishek kamble 29-nov-2013
                    EpayModel.USERID = PMGSYSession.Current.UserId;
                    EpayModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    dbContext.ACC_EPAY_MAIL_MASTER.Add(EpayModel);

                    //get the bill details                    
                    List<ACC_BILL_DETAILS> listBillDetails = new List<ACC_BILL_DETAILS>();
                    listBillDetails = dbContext.ACC_BILL_DETAILS.Where(c => c.BILL_ID == bill_ID && c.CREDIT_DEBIT == "D" && c.CASH_CHQ == "Q").ToList<ACC_BILL_DETAILS>();

                    long maxDetail_ID = 0;

                    if (dbContext.ACC_EPAY_MAIL_MASTER.Any())
                    {
                        maxDetail_ID = dbContext.ACC_EPAY_MAIL_DETAILS.Max(c => c.DETAIL_ID);
                    }

                    //insert details for each contractor
                    foreach (ACC_BILL_DETAILS bill in listBillDetails)
                    {

                        EremittnaceContractor Contractor = new EremittnaceContractor();

                        MASTER_CONTRACTOR contractorInfo = dbContext.MASTER_CONTRACTOR.Where(c => c.MAST_CON_ID == bill.MAST_CON_ID).FirstOrDefault();

                        MASTER_CONTRACTOR_BANK con = new MASTER_CONTRACTOR_BANK();

                        //if (PMGSYSession.Current.FundType != "A")
                        //{
                        //  con = dbContext.MASTER_CONTRACTOR_BANK.Where(v => v.MAST_CON_ID == bill.MAST_CON_ID && v.MAST_ACCOUNT_STATUS == "A").FirstOrDefault();// FirstOrDefault Checked By Abhishek kamble 8-Apt-2014
                        con = dbContext.MASTER_CONTRACTOR_BANK.Where(v => v.MAST_CON_ID == bill.MAST_CON_ID && v.MAST_ACCOUNT_STATUS == "A" && v.MAST_DISTRICT_CODE == PMGSYSession.Current.DistrictCode).FirstOrDefault();
                        if (con == null)
                        {
                            con = dbContext.MASTER_CONTRACTOR_BANK.Where(v => v.MAST_CON_ID == bill.MAST_CON_ID && v.MAST_ACCOUNT_STATUS == "A").FirstOrDefault();

                        }
                        //}


                        //insert the details into [ACC_EPAY_MAIL_DETAILS]
                        ACC_EPAY_MAIL_DETAILS EpayDetailsModel = new ACC_EPAY_MAIL_DETAILS();


                        maxDetail_ID = maxDetail_ID + 1;

                        EpayDetailsModel.DETAIL_ID = maxDetail_ID;

                        EpayDetailsModel.EPAY_ID = maxEPAY_ID;

                        EpayDetailsModel.BANK_ACC_NO = bankDetails.BANK_ACC_NO;

                        EpayDetailsModel.BILL_NO = masterDetails.BILL_NO;

                        EpayDetailsModel.BILL_DATE = masterDetails.BILL_DATE;

                        //Old
                        //EpayDetailsModel.AGREEMENT_NO = masterDetails.FUND_TYPE == "M" ? (dbContext.MANE_IMS_CONTRACT.Where(c => c.MANE_PR_CONTRACT_CODE == (bill.IMS_AGREEMENT_CODE.HasValue ? bill.IMS_AGREEMENT_CODE.Value : -1)).Select(x => x.MANE_AGREEMENT_NUMBER).FirstOrDefault()) : (dbContext.TEND_AGREEMENT_MASTER.Where(c => c.TEND_AGREEMENT_CODE == (bill.IMS_AGREEMENT_CODE.HasValue ? bill.IMS_AGREEMENT_CODE.Value : -1)).Select(y => y.TEND_AGREEMENT_NUMBER).FirstOrDefault());
                        //Modified by Abhishek kamble to get Agreement No using MANE_CONTRACTOR_ID 17Nov2014
                        EpayDetailsModel.AGREEMENT_NO = masterDetails.FUND_TYPE == "M" ? (dbContext.MANE_IMS_CONTRACT.Where(c => c.MANE_CONTRACT_ID == (bill.IMS_AGREEMENT_CODE.HasValue ? bill.IMS_AGREEMENT_CODE.Value : -1)).Select(x => x.MANE_AGREEMENT_NUMBER).FirstOrDefault()) : (dbContext.TEND_AGREEMENT_MASTER.Where(c => c.TEND_AGREEMENT_CODE == (bill.IMS_AGREEMENT_CODE.HasValue ? bill.IMS_AGREEMENT_CODE.Value : -1)).Select(y => y.TEND_AGREEMENT_NUMBER).FirstOrDefault());

                        EpayDetailsModel.PKG_NO = Packages;

                        //modified by abhishek kamble
                        EpayDetailsModel.CON_NAME = (contractorInfo == null ? "" : contractorInfo.MAST_CON_COMPANY_NAME);




                        EpayDetailsModel.CON_ACC_NO = (con == null ? "" : con.MAST_ACCOUNT_NUMBER);

                        EpayDetailsModel.CON_BANK_NAME = (con == null ? "" : con.MAST_BANK_NAME);

                        EpayDetailsModel.CON_BANK_IFS_CODE = (con == null ? "" : con.MAST_IFSC_CODE);

                        EpayDetailsModel.EPAY_AMOUNT = bill.AMOUNT;

                        EpayDetailsModel.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;

                        EpayDetailsModel.LVL_ID = (byte)PMGSYSession.Current.LevelId;

                        EpayDetailsModel.CON_PAN_NO = dbContext.MASTER_CONTRACTOR.Where(c => c.MAST_CON_ID == bill.MAST_CON_ID).Select(d => d.MAST_CON_PAN).FirstOrDefault();

                        // Added By Abhishek kamble 29-nov-2013
                        EpayDetailsModel.USERID = PMGSYSession.Current.UserId;
                        EpayDetailsModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                        dbContext.ACC_EPAY_MAIL_DETAILS.Add(EpayDetailsModel);
                    }


                    //set finalization status as "Y" in bill master

                    ACC_BILL_MASTER acc_bill_master = dbContext.ACC_BILL_MASTER.Find(bill_ID);

                    if (acc_bill_master.CHQ_EPAY == "E")
                    {
                        //acc_bill_master.BILL_FINALIZED = "E";
                        //new change done by Vikram on 16-10-2013

                        acc_bill_master.BILL_FINALIZED = "Y";

                        //end of change
                    }
                    else
                    {
                        acc_bill_master.BILL_FINALIZED = "Y";
                    }
                    acc_bill_master.ACTION_REQUIRED = "N";

                    // (Flag Modified )Added By Abhishek kamble 29-nov-2013
                    acc_bill_master.USERID = PMGSYSession.Current.UserId;
                    acc_bill_master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    dbContext.Entry(acc_bill_master).State = System.Data.Entity.EntityState.Modified;

                    dbContext.SaveChanges();
                    scope.Complete();
                    return "1";
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error while Finalizing Eremittances");
            }
            finally
            {
                commomFuncObj = null;
                dbContext.Dispose();
            }

        }


        /// <summary>
        /// DAl function to delete the email details when error while sending mail
        /// </summary>
        /// <param name="billId"></param>
        /// <returns></returns>
        public String DeleteMailDetails(long Bill_Id)
        {
            try
            {
                dbContext = new PMGSYEntities();
                using (var scope = new TransactionScope())
                {

                    //get the master details
                    ACC_EPAY_MAIL_MASTER Email = dbContext.ACC_EPAY_MAIL_MASTER.SingleOrDefault(p => p.BILL_ID == Bill_Id && p.IS_EPAY_VALID == true);   //IS_EPAY_VALID =true Checked by Abhishek for Reject/Resend Epayment Details 25-Sep-2014

                    long _EpayId = Email.EPAY_ID;

                    if (Email == null)
                    {
                        return "-1";
                    }

                    //Added By Abhishek kamble 29-nov-2013
                    ACC_EPAY_MAIL_DETAILS epayMailDetailsModel = dbContext.ACC_EPAY_MAIL_DETAILS.Where(m => m.EPAY_ID == Email.EPAY_ID).FirstOrDefault();
                    if (epayMailDetailsModel != null)
                    {
                        epayMailDetailsModel.USERID = PMGSYSession.Current.UserId;
                        epayMailDetailsModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        dbContext.Entry(epayMailDetailsModel).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                    dbContext.Database.ExecuteSqlCommand
                         ("DELETE [omms].ACC_EPAY_MAIL_DETAILS Where EPAY_ID = {0}", Email.EPAY_ID);

                    //Added By Abhishek kamble 29-nov-2013
                    ACC_EPAY_MAIL_MASTER epayMailMasterModel = dbContext.ACC_EPAY_MAIL_MASTER.Where(m => m.EPAY_ID == Email.EPAY_ID).FirstOrDefault();
                    if (epayMailMasterModel != null)
                    {
                        epayMailMasterModel.USERID = PMGSYSession.Current.UserId;
                        epayMailMasterModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        dbContext.Entry(epayMailMasterModel).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }

                    //Added By Abhishek kamble For Reject/Resend Epayment Details 26 Sep 2014

                    if (dbContext.ACC_EPAY_MAIL_RESEND_DETAILS.Where(m => m.NEW_EPAY_ID == Email.EPAY_ID).Any())
                    {
                        long Old_Epay_ID = dbContext.ACC_EPAY_MAIL_RESEND_DETAILS.Where(m => m.NEW_EPAY_ID == Email.EPAY_ID).Select(s => s.OLD_EPAY_ID).FirstOrDefault();
                        dbContext.Database.ExecuteSqlCommand("DELETE [omms].ACC_EPAY_MAIL_RESEND_DETAILS where NEW_EPAY_ID={0}", Email.EPAY_ID);

                        //Update IS_Valid_FLAg 
                        ACC_EPAY_MAIL_MASTER resendModel = dbContext.ACC_EPAY_MAIL_MASTER.Where(m => m.EPAY_ID == Old_Epay_ID).FirstOrDefault();
                        resendModel.IS_EPAY_VALID = true;
                        dbContext.Entry(resendModel).State = System.Data.Entity.EntityState.Modified;
                    }
                    dbContext.Database.ExecuteSqlCommand
                        ("DELETE [omms].ACC_EPAY_MAIL_MASTER Where EPAY_ID = {0}", Email.EPAY_ID);

                    dbContext.SaveChanges();

                    scope.Complete();

                    return "1";
                }

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error while Finalizing Epayment");
            }
            finally
            {
                dbContext.Dispose();
            }

        }

        #endregion

        #region Reject / Resend Epayment

        public Array GetEpaymentRejectResendList(PaymentFilterModel objFilter, string TransactionType, out long totalRecords)
        {
            string description = string.Empty;
            DateTime pfmsStartDate = new DateTime(2018, 08, 02);
            dbContext = new PMGSYEntities();
            try
            {
                PMGSYEntities dBContext = new PMGSYEntities();
                CommonFunctions commomFuncObj = new CommonFunctions();

                List<USP_ACC_DISPLAY_REJECT_RESEND_EPAYMENT_LIST_Result> lstBillMaster = dbContext.USP_ACC_DISPLAY_REJECT_RESEND_EPAYMENT_LIST(objFilter.AdminNdCode, objFilter.Month, objFilter.Year, objFilter.FundType, TransactionType).ToList<USP_ACC_DISPLAY_REJECT_RESEND_EPAYMENT_LIST_Result>();
                //Below if Condition is added on 17-01-2022 to show the list of rejected only
                if (PMGSYSession.Current.FundType == "A")
                {
                    List<USP_ACC_DISPLAY_REJECT_RESEND_EPAYMENT_LIST_Result> itemtobeRemoved = new List<USP_ACC_DISPLAY_REJECT_RESEND_EPAYMENT_LIST_Result>();

                    foreach (USP_ACC_DISPLAY_REJECT_RESEND_EPAYMENT_LIST_Result item in lstBillMaster)
                    {
                        if (!dBContext.REAT_OMMAS_PAYMENT_MAPPING.Any(m => m.BILL_ID == item.BILL_ID && (m.ACK_BILL_STATUS == "R" || m.BANK_ACK_BILL_STATUS == "R")))
                        {
                            itemtobeRemoved.Add(item);
                        }
                    }

                    foreach (USP_ACC_DISPLAY_REJECT_RESEND_EPAYMENT_LIST_Result item in itemtobeRemoved)
                    {
                        lstBillMaster.Remove(item);
                    }
                }
                
                totalRecords = lstBillMaster.Count();

                if (objFilter.sidx.Trim() != string.Empty)
                {
                    if (objFilter.sord.ToString() == "asc")
                    {
                        switch (objFilter.sidx)
                        {
                            case "Voucher_Number":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.BILL_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<USP_ACC_DISPLAY_REJECT_RESEND_EPAYMENT_LIST_Result>();
                                break;
                            case "voucher_date":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.BILL_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<USP_ACC_DISPLAY_REJECT_RESEND_EPAYMENT_LIST_Result>();
                                break;
                            case "cheque_number":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.CHQ_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<USP_ACC_DISPLAY_REJECT_RESEND_EPAYMENT_LIST_Result>();
                                break;
                            case "cheque_Date":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.CHQ_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<USP_ACC_DISPLAY_REJECT_RESEND_EPAYMENT_LIST_Result>();
                                break;
                            case "cheque_amount":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.CHQ_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<USP_ACC_DISPLAY_REJECT_RESEND_EPAYMENT_LIST_Result>();
                                break;
                            case "Cash_Amount":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.CASH_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<USP_ACC_DISPLAY_REJECT_RESEND_EPAYMENT_LIST_Result>();
                                break;
                            case "Gross_Amount":
                                lstBillMaster = lstBillMaster.OrderBy(x => x.GROSS_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<USP_ACC_DISPLAY_REJECT_RESEND_EPAYMENT_LIST_Result>();
                                break;
                            default:
                                lstBillMaster = lstBillMaster.OrderBy(x => x.BILL_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<USP_ACC_DISPLAY_REJECT_RESEND_EPAYMENT_LIST_Result>();
                                break;
                        }

                    }
                    else
                    {
                        switch (objFilter.sidx)
                        {
                            case "Voucher_Number":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.BILL_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<USP_ACC_DISPLAY_REJECT_RESEND_EPAYMENT_LIST_Result>();
                                break;
                            case "voucher_date":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.BILL_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<USP_ACC_DISPLAY_REJECT_RESEND_EPAYMENT_LIST_Result>();
                                break;
                            case "cheque_number":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.CHQ_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<USP_ACC_DISPLAY_REJECT_RESEND_EPAYMENT_LIST_Result>();
                                break;
                            case "cheque_Date":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.CHQ_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<USP_ACC_DISPLAY_REJECT_RESEND_EPAYMENT_LIST_Result>();
                                break;
                            case "cheque_amount":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.CHQ_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<USP_ACC_DISPLAY_REJECT_RESEND_EPAYMENT_LIST_Result>();
                                break;
                            case "Cash_Amount":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.CASH_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<USP_ACC_DISPLAY_REJECT_RESEND_EPAYMENT_LIST_Result>();
                                break;
                            case "Gross_Amount":
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.GROSS_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<USP_ACC_DISPLAY_REJECT_RESEND_EPAYMENT_LIST_Result>();
                                break;
                            default:
                                lstBillMaster = lstBillMaster.OrderByDescending(x => x.BILL_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<USP_ACC_DISPLAY_REJECT_RESEND_EPAYMENT_LIST_Result>();
                                break;
                        }
                    }
                }
                else
                {
                    lstBillMaster = lstBillMaster.OrderByDescending(x => x.BILL_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<USP_ACC_DISPLAY_REJECT_RESEND_EPAYMENT_LIST_Result>();
                }

                return lstBillMaster.Select(item => new
                {
                    //id = URLEncrypt.EncryptParameters(new string[] { item.BILL_NO }),
                    id = URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString() }),//modified 13May2015 for not disp in fin login

                    cell = new[]{
                    item.BILL_NO,
                    commomFuncObj.GetDateTimeToString(item.BILL_DATE),
                    item.CHQ_NO,                    
                    item.CHQ_DATE == null ? String.Empty : commomFuncObj.GetDateTimeToString(item.CHQ_DATE.Value),
                    item.PAYEE_NAME==null?"":item.PAYEE_NAME.ToString(),
                    item.CHQ_AMOUNT.ToString(),
                    item.CASH_AMOUNT.ToString(),
                    item.GROSS_AMOUNT.ToString(), 
                    //Below Line is Commented on 17-01-2022
                    //item.IS_CHQ_ENCASHED_NA?"-":("<center><a href='#' class='ui-icon ui-icon-arrowrefresh-1-n' onclick='RejectResendEpayment(\"" +URLEncrypt.EncryptParameters1(new string[] {"BILL_ID="+item.BILL_ID.ToString().Trim() , "EPAY_ID=" +item.EPAY_ID.ToString(),"BILL_DATE="+item.BILL_DATE.ToString("dd/MM/yyyy").Replace('/','.')})+ "\");return false;'>Resend</a></center>"),//list sp -Modified to Cancel Acknowledged cheques also 20Mar2015

                    //Below Line is Modified on 17-01-2022 to Disable resend Column
                    item.IS_CHQ_ENCASHED_NA?"-":(PMGSYSession.Current.FundType.Equals("P") || PMGSYSession.Current.FundType.Equals("A"))?"-":("<center><a href='#' class='ui-icon ui-icon-arrowrefresh-1-n' onclick='RejectResendEpayment(\"" +URLEncrypt.EncryptParameters1(new string[] {"BILL_ID="+item.BILL_ID.ToString().Trim() , "EPAY_ID=" +item.EPAY_ID.ToString(),"BILL_DATE="+item.BILL_DATE.ToString("dd/MM/yyyy").Replace('/','.')})+ "\");return false;'>Resend</a></center>"),//list sp -Modified to Cancel Acknowledged cheques also 20Mar2015
                    
                    //"<center><a href='#' class='ui-icon ui-icon-closethick' onclick='CancelEpayEremi(\"" +URLEncrypt.EncryptParameters1(new string[] {"BILL_ID="+item.BILL_ID.ToString().Trim() , "EPAY_ID=" +item.EPAY_ID.ToString(),"BILL_DATE="+item.BILL_DATE.ToString("dd/MM/yyyy").Replace('/','.')})+ "\");return false;'>Cancel</a></center>"

                     //Below Line commneted on 04-01-2022
                    //(((dbContext.PFMS_OMMAS_PAYMENT_MAPPING.Any(m => m.BILL_ID == item.BILL_ID) && PMGSYSession.Current.FundType == "P" && (item.BILL_DATE >= pfmsStartDate) && (!IsRejectedByPFMSDAL(item.BILL_ID)))) || ((dbContext.REAT_OMMAS_PAYMENT_MAPPING.Any(m => m.BILL_ID == item.BILL_ID) && PMGSYSession.Current.FundType == "P" && (item.BILL_DATE >= pfmsStartDate) && (!IsRejectedByPFMSDAL(item.BILL_ID)))) ?  //OR condition added for REAT_OMMS_PAYMENT_MAPPING table Done by priyanka 12-05-2020
                    //Below Line Added on 04-01-2022
                    (((dbContext.PFMS_OMMAS_PAYMENT_MAPPING.Any(m => m.BILL_ID == item.BILL_ID) && PMGSYSession.Current.FundType == "P" && (item.BILL_DATE >= pfmsStartDate) && (!IsRejectedByPFMSDAL(item.BILL_ID)))) || ((dbContext.REAT_OMMAS_PAYMENT_MAPPING.Any(m => m.BILL_ID == item.BILL_ID) && (PMGSYSession.Current.FundType == "P" || PMGSYSession.Current.FundType == "A") && (item.BILL_DATE >= pfmsStartDate) && (!IsRejectedByPFMSDAL(item.BILL_ID)))) ?  //OR condition added for REAT_OMMS_PAYMENT_MAPPING table Done by priyanka 12-05-2020
                    GetPFMSStatus(item.BILL_ID, out description)
                    : "<center><a href='#' class='ui-icon ui-icon-closethick' onclick='CancelEpayEremi(\"" +URLEncrypt.EncryptParameters1(new string[] {"BILL_ID="+item.BILL_ID.ToString().Trim() , "EPAY_ID=" +item.EPAY_ID.ToString(),"BILL_DATE="+item.BILL_DATE.ToString("dd/MM/yyyy").Replace('/','.')})+ "\");return false;'>Cancel</a></center>")
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
                    dbContext.Dispose();
            }
        }



        public bool InsertResendEpaymentDetails(RejectResendFormModel model, long Bill_ID)
        {
            dbContext = new PMGSYEntities();
            try
            {
                int AdminNdCode = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == Bill_ID).Select(s => s.ADMIN_ND_CODE).FirstOrDefault();
                DateTime resendDate = new CommonFunctions().GetStringToDateTime(model.ResendDate);
                dbContext.USP_ACC_INSERT_EPAYMENT_DETAILS(AdminNdCode, model.Epay_ID, "R", resendDate, model.Reason.Trim(), model.UploadFileName, ("S_" + model.EpayPDFFileName), (model.Remark == null ? model.Remark : model.Remark.Trim()), PMGSYSession.Current.UserId, HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], model.HeadId, PMGSYSession.Current.FundType);
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error while saving details.");
            }
            finally
            {

            }
        }

        public bool CancelEpaymentEremDetails(RejectResendFormModel model, long Bill_ID, ref string message)
        {
            dbContext = new PMGSYEntities();
            try
            {
                int AdminNdCode = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == Bill_ID).Select(s => s.ADMIN_ND_CODE).FirstOrDefault();
                DateTime cancelDate = new CommonFunctions().GetStringToDateTime(model.ResendDate);

                //validation for month close start

                //validation for month close start

                //if (dbContext.ACC_RPT_MONTHWISE_SUMMARY.Where(m => m.ADMIN_ND_CODE == AdminNdCode && m.FUND_TYPE == PMGSYSession.Current.FundType && m.ACC_MONTH == cancelDate.Month && m.ACC_YEAR == cancelDate.Year).Any())
                //{
                //    message = "Month is closed of DPIU,Please revoke month and try again.";
                //    return false;
                //}

                //Below code is added on 06-01-2022 to identify validation for 
                if (dbContext.ACC_RPT_MONTHWISE_SUMMARY.Where(m => m.ADMIN_ND_CODE == AdminNdCode && m.FUND_TYPE == PMGSYSession.Current.FundType && m.ACC_MONTH == cancelDate.Month && m.ACC_YEAR == cancelDate.Year).Any())
                {
                    var nd_type = dbContext.ADMIN_DEPARTMENT.Where(m => m.ADMIN_ND_CODE == AdminNdCode).FirstOrDefault();

                    if (nd_type.MAST_ND_TYPE == "D")
                    {
                        message = "Month is closed of DPIU,Please revoke month and try again.";
                    }
                    else if (nd_type.MAST_ND_TYPE == "S")
                    {
                        message = "Month is closed of SRRDA,Please revoke month and try again.";
                    }
                    return false;
                }

                //validation for month close end

                // Added on 14 Aug 2019
                bool returnVal = false;
                var result = dbContext.USP_ACC_INSERT_EPAYMENT_DETAILS(AdminNdCode, model.Epay_ID, "C", cancelDate, model.Reason.ToString().Trim(), model.UploadFileName, null, (model.Remark == null ? model.Remark : model.Remark.Trim()), PMGSYSession.Current.UserId, HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], model.HeadId, PMGSYSession.Current.FundType);
                returnVal = result.Contains(0) ? false : true;
                return returnVal;

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw new Exception("Error while cancelling details.");
            }
            finally
            {

            }
        }

        public bool IsRejectedByPFMSDAL(long billId)
        {
            PMGSYEntities dBContext = new PMGSYEntities();
            try
            {
                if (dBContext.PFMS_OMMAS_PAYMENT_MAPPING.Any(m => m.BILL_ID == billId && (m.ACK_BILL_STATUS == "R" || m.BANK_ACK_BILL_STATUS == "R")))
                {
                    return true;
                }
                else if (dBContext.REAT_OMMAS_PAYMENT_MAPPING.Any(m => m.BILL_ID == billId && (m.ACK_BILL_STATUS == "R" || m.BANK_ACK_BILL_STATUS == "R"))) //else if condition added for REAT_OMMS_PAYMENT_MAPPING table Done by priyanka 12-05-2020
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
                ErrorLog.LogError(ex, "PaymentDAL.IsRejectedByPFMSDAL()");
                return false;
            }
            finally
            {
                if (dBContext != null)
                {
                    dBContext.Dispose();
                }
            }
        }


        #endregion Reject / Resend Epayment

        //new method added by Vikram
        public bool ValidateDPIUEpaymentDAL(int adminCode)
        {
            dbContext = new PMGSYEntities();
            try
            {
                if (dbContext.ADMIN_DEPARTMENT.Any(m => m.ADMIN_ND_CODE == adminCode && m.ADMIN_EPAY_ENABLE_DATE != null))
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

        public bool ValidateDPIUERemittenceDAL(int adminCode)
        {
            dbContext = new PMGSYEntities();
            try
            {
                if (dbContext.ADMIN_DEPARTMENT.Any(m => m.ADMIN_ND_CODE == adminCode && m.ADMIN_EREMITTANCE_ENABLED == "Y" && !((m.ADMIN_ND_TAN_NO == null) || (m.ADMIN_ND_TAN_NO == string.Empty))))
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

        public bool IsPFMSFinalized(int conId, int AcountId)
        {
            dbContext = new PMGSYEntities();
            bool isFinalized = false;

            string moduleType = "D";
            REAT_OMMAS_PAYMENT_SUCCESS objModuleType = dbContext.REAT_OMMAS_PAYMENT_SUCCESS.FirstOrDefault(s => s.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode);
            if (objModuleType != null)
            {
                moduleType = "R";
            }

            try
            {
                if (moduleType.Equals("D"))
                {
                    int lgdStateCode = dbContext.OMMAS_LDG_STATE_MAPPING.Where(x => x.MAST_STATE_CODE == PMGSYSession.Current.StateCode).Select(x => x.MAST_STATE_LDG_CODE).FirstOrDefault();
                    return (dbContext.PFMS_OMMAS_CONTRACTOR_MAPPING.Any(x => x.MAST_CON_ID == conId && x.MAST_ACCOUNT_ID == AcountId && x.MAST_LGD_STATE_CODE == lgdStateCode && x.MAST_AGENCY_CODE == PMGSYSession.Current.MastAgencyCode && x.PFMS_CON_ID != null && x.STATUS == "A" && x.MASTER_CONTRACTOR.MASTER_CONTRACTOR_BANK.Where(z => z.MAST_CON_ID == conId && z.MAST_ACCOUNT_ID == x.MAST_ACCOUNT_ID && z.MAST_LOCK_STATUS == "Y").Select(b => b.MAST_ACCOUNT_STATUS).FirstOrDefault() == "A"));
                }
                else if (moduleType.Equals("R"))
                {
                    return (dbContext.REAT_CONTRACTOR_DETAILS.Any(x => x.MAST_CON_ID == conId && x.MAST_ACCOUNT_ID == AcountId && x.REAT_CON_ID != null && x.reat_STATUS == "A" && x.ommas_STATUS == "A"));
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PaymentDAL.IsPFMSFinalized()");
                return false;
            }
        }

        /// <summary>
        /// check whether contractor bank details are entered or not for performing epayment or eremittence
        /// </summary>
        /// <param name="conId"></param>
        /// <returns></returns>
        public bool ValidateContractorStatus(int conId, int txnId)
        {
            dbContext = new PMGSYEntities();
            try
            {
                //if (txnId == 472 || txnId == 415)
                //{
                //    if (dbContext.ADMIN_NO_BANK.Any(m => m.ADMIN_NO_OFFICER_CODE == conId && m.MAST_ACCOUNT_STATUS == "A"))
                //    {
                //        var bankDetails = dbContext.ADMIN_NO_BANK.Where(m => m.ADMIN_NO_OFFICER_CODE == conId && m.MAST_ACCOUNT_STATUS == "A").First();
                //        if (bankDetails.MAST_ACCOUNT_NUMBER.Equals(string.Empty) || bankDetails.MAST_ACCOUNT_NUMBER.Equals("0"))
                //        {
                //            return false;
                //        }
                //        {
                //            return true;
                //        }
                //    }
                //    else
                //    {
                //        return false;
                //    }
                //}
                //else
                //{


                    if (dbContext.MASTER_CONTRACTOR_BANK.Any(m => m.MAST_CON_ID == conId && m.MAST_ACCOUNT_STATUS == "A" && m.MAST_LOCK_STATUS == "Y"))
                    {
                        var bankDetails = dbContext.MASTER_CONTRACTOR_BANK.Where(m => m.MAST_CON_ID == conId && m.MAST_ACCOUNT_STATUS == "A" && m.MAST_LOCK_STATUS == "Y").First();
                        if (bankDetails.MAST_ACCOUNT_NUMBER.Equals(string.Empty) || bankDetails.MAST_ACCOUNT_NUMBER.Equals("0"))
                        {
                            return false;
                        }
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            //}

            catch (Exception)
            {
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        public bool ValidateAdviceNoExist(String ChqNo, long Bill_ID)
        {
            dbContext = new PMGSYEntities();
            try
            {
                if (Bill_ID == 0)//Add
                {
                    if (dbContext.ACC_BILL_MASTER.Any(m => m.FUND_TYPE == PMGSYSession.Current.FundType && m.LVL_ID == PMGSYSession.Current.LevelId && m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && m.CHQ_NO == ChqNo && m.CHQ_EPAY == "A" && m.BILL_TYPE == "P"))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                { //Edit
                    if (dbContext.ACC_BILL_MASTER.Any(m => m.FUND_TYPE == PMGSYSession.Current.FundType && m.LVL_ID == PMGSYSession.Current.LevelId && m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && m.CHQ_NO == ChqNo && m.CHQ_EPAY == "A" && m.BILL_TYPE == "P" && m.BILL_ID != Bill_ID))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool ValidateifMonthAcknowledged(int? srrdaCode, int dpiuCode, string fundType, Int16 BillMonth, Int16 BillYear)
        {
            dbContext = new PMGSYEntities();
            string dpiu = dpiuCode.ToString();
            try
            {

                if (dbContext.ACC_BILL_MASTER.Where(m => m.FUND_TYPE == fundType && m.LVL_ID == 4 && m.ADMIN_ND_CODE == srrdaCode && m.CHALAN_NO == dpiu
                    && m.BILL_MONTH == BillMonth && m.BILL_YEAR == BillYear && m.BILL_TYPE == "P" && m.BILL_FINALIZED == "Y").Any())
                {
                    return false;
                }
                else
                {
                    return true;
                }

            }
            catch (Exception ex)
            {
                return true;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        #region Payment Validation
        public PaymentValidationViewModel ValidatePFMSPaymentDetails(string module_Type)
        {
            dbContext = new PMGSYEntities();
            try
            {

                ADMIN_NODAL_OFFICERS officer = dbContext.ADMIN_NODAL_OFFICERS.FirstOrDefault(x => x.ADMIN_MODULE == "A" && x.ADMIN_ACTIVE_STATUS == "Y" && x.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && x.ADMIN_NO_DESIGNATION == 26);  //bill admin code used b4
                ADMIN_DEPARTMENT AdminDeptPIU = dbContext.ADMIN_DEPARTMENT.FirstOrDefault(s => s.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode); //bill
                ACC_BANK_DETAILS BankDetails = dbContext.ACC_BANK_DETAILS.FirstOrDefault(s => s.FUND_TYPE == PMGSYSession.Current.FundType && s.ADMIN_ND_CODE == AdminDeptPIU.MAST_PARENT_ND_CODE && s.BANK_ACC_STATUS == true && s.ACCOUNT_TYPE == "S");
                PaymentValidationViewModel model = new PaymentValidationViewModel();
                if (module_Type.Equals("D"))
                {
                    PFMS_INITIATING_PARTY_MASTER initparty = dbContext.PFMS_INITIATING_PARTY_MASTER.FirstOrDefault(s => s.ADMIN_ND_CODE == AdminDeptPIU.MAST_PARENT_ND_CODE);
                    PFMS_OMMAS_DSC_MAPPING dsc = dbContext.PFMS_OMMAS_DSC_MAPPING.OrderByDescending(x => x.FILE_PROCESS_DATE).FirstOrDefault(x => x.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && x.ACK_DSC_STATUS == "ACCP");
                    PFMS_OMMAS_PAYMENT_SUCCESS paymentSuccess = dbContext.PFMS_OMMAS_PAYMENT_SUCCESS.Where(x => x.ADMIN_ND_CODE == AdminDeptPIU.MAST_PARENT_ND_CODE).FirstOrDefault();
                    model.IsAgencyMapped = (initparty == null) ? false : true;
                    model.IsSRRDABankDetailsFinalized = (BankDetails == null) ? false : true;
                    model.IsDSCEnrollmentFinalized = (dsc == null) ? false : true;
                    model.IsEmailAvailable = (officer == null) ? false : !String.IsNullOrEmpty(officer.ADMIN_NO_EMAIL.Trim());
                    model.IsPaymentSuccess = (paymentSuccess == null) ? false : true;
                }

                if (module_Type.Equals("R"))
                {
                    REAT_INITIATING_PARTY_MASTER initparty = dbContext.REAT_INITIATING_PARTY_MASTER.FirstOrDefault(s => s.ADMIN_ND_CODE == AdminDeptPIU.MAST_PARENT_ND_CODE);
                    //comment on : 11 oct 2021
                    //REAT_OMMAS_DSC_MAPPING dsc = dbContext.REAT_OMMAS_DSC_MAPPING.OrderByDescending(x => x.FILE_PROCESS_DATE).FirstOrDefault(x => x.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && x.ADMIN_NO_OFFICER_CODE == officer.ADMIN_NO_OFFICER_CODE && x.ACK_DSC_STATUS == "ACCP" && x.IS_ACTIVE == true);
                    REAT_OMMAS_DSC_MAPPING dsc = new REAT_OMMAS_DSC_MAPPING();
                    dsc = null;
                    //if (PMGSYSession.Current.LevelId == 5)
                    //if (PMGSYSession.Current.LevelId == 5 || (PMGSYSession.Current.FundType.Equals("A") && PMGSYSession.Current.LevelId == 4))
                    if (PMGSYSession.Current.LevelId == 5  )
                    {
                         dsc = dbContext.REAT_OMMAS_DSC_MAPPING.OrderByDescending(x => x.FILE_PROCESS_DATE).FirstOrDefault(x => x.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && x.ADMIN_NO_OFFICER_CODE == officer.ADMIN_NO_OFFICER_CODE && x.ACK_DSC_STATUS == "ACCP" && x.IS_ACTIVE == true && x.FUND_TYPE == "P");
                    }
                    //comment on : 11 oct 2021
                    //REAT_OMMAS_PAYMENT_SUCCESS paymentSuccess = dbContext.REAT_OMMAS_PAYMENT_SUCCESS.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).FirstOrDefault();
                    REAT_OMMAS_PAYMENT_SUCCESS paymentSuccess = new REAT_OMMAS_PAYMENT_SUCCESS();
                    if (PMGSYSession.Current.LevelId == 5 || (PMGSYSession.Current.FundType.Equals("A") && PMGSYSession.Current.LevelId == 4))
                    {
                        paymentSuccess = dbContext.REAT_OMMAS_PAYMENT_SUCCESS.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && x.FUND_TYPE == PMGSYSession.Current.FundType).FirstOrDefault();
                    }

                   // paymentSuccess = true;
                    if (initparty != null)
                    {
                        if (!(string.IsNullOrEmpty(initparty.REAT_INIT_PARTY_UNIQUE_CODE) || string.IsNullOrEmpty(initparty.SCHEME_CODE)))
                        {
                            model.IsAgencyMapped = true;
                        }
                        else
                        {
                            model.IsAgencyMapped = false;
                        }

                    }
                    else
                    {
                        model.IsAgencyMapped = false;
                    }

                    model.IsSRRDABankDetailsFinalized = (BankDetails == null) ? false : true;
                   // if (PMGSYSession.Current.LevelId == 5)
                    //if (PMGSYSession.Current.LevelId == 5 || (PMGSYSession.Current.FundType.Equals("A") && PMGSYSession.Current.LevelId == 4))
                    if (PMGSYSession.Current.LevelId == 5 )
                    {
                        model.IsDSCEnrollmentFinalized = (dsc == null) ? false : true;
                    }
                    else
                    {
                        model.IsDSCEnrollmentFinalized = true;
                    }
                    if (PMGSYSession.Current.LevelId == 5)
                    {
                        model.IsEmailAvailable = (officer == null) ? false : !String.IsNullOrEmpty(officer.ADMIN_NO_EMAIL.Trim());
                    }
                    else 
                    {
                        model.IsEmailAvailable = true;
                    }
                    //if (PMGSYSession.Current.LevelId == 5)
                    //if (PMGSYSession.Current.LevelId == 5 || (PMGSYSession.Current.FundType.Equals("A") && PMGSYSession.Current.LevelId == 4))
                    //{
                    //    model.IsPaymentSuccess = (paymentSuccess == null) ? false : true;
                    //}
                    //else
                    //{
                    //    model.IsPaymentSuccess = true;
                    //}
                    model.IsPaymentSuccess = true; //Added on 03-12-2021
                }
                return model;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PaymentDAL.ValidatePFMSPaymentDetails()");
                return null;
            }
        }

        //Below method Added on 09-12-2021  
        public PaymentValidationViewModel ValidatePFMSPaymentDetails(string module_Type,int txn_Id)
        {
            dbContext = new PMGSYEntities();
            try
            {

                ADMIN_NODAL_OFFICERS officer = dbContext.ADMIN_NODAL_OFFICERS.FirstOrDefault(x => x.ADMIN_MODULE == "A" && x.ADMIN_ACTIVE_STATUS == "Y" && x.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && x.ADMIN_NO_DESIGNATION == 26);  //bill admin code used b4
                ADMIN_DEPARTMENT AdminDeptPIU = dbContext.ADMIN_DEPARTMENT.FirstOrDefault(s => s.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode); //bill
                //ACC_BANK_DETAILS BankDetails = dbContext.ACC_BANK_DETAILS.FirstOrDefault(s => s.FUND_TYPE == PMGSYSession.Current.FundType && s.ADMIN_ND_CODE == AdminDeptPIU.MAST_PARENT_ND_CODE && s.BANK_ACC_STATUS == true);
                ACC_BANK_DETAILS BankDetails = null;

                if (PMGSYSession.Current.FundType == "M" && (txn_Id == 3050 || txn_Id == 3051))
                {
                    BankDetails = dbContext.ACC_BANK_DETAILS.FirstOrDefault(s => s.FUND_TYPE =="P" && s.ADMIN_ND_CODE == AdminDeptPIU.MAST_PARENT_ND_CODE && s.BANK_ACC_STATUS == true && s.ACCOUNT_TYPE == "S");
                }
                else
                {
                    BankDetails = dbContext.ACC_BANK_DETAILS.FirstOrDefault(s => s.FUND_TYPE == PMGSYSession.Current.FundType && s.ADMIN_ND_CODE == AdminDeptPIU.MAST_PARENT_ND_CODE && s.BANK_ACC_STATUS == true && s.ACCOUNT_TYPE == "S");

                }

                PaymentValidationViewModel model = new PaymentValidationViewModel();
                if (module_Type.Equals("D"))
                {
                    PFMS_INITIATING_PARTY_MASTER initparty = dbContext.PFMS_INITIATING_PARTY_MASTER.FirstOrDefault(s => s.ADMIN_ND_CODE == AdminDeptPIU.MAST_PARENT_ND_CODE);
                    PFMS_OMMAS_DSC_MAPPING dsc = dbContext.PFMS_OMMAS_DSC_MAPPING.OrderByDescending(x => x.FILE_PROCESS_DATE).FirstOrDefault(x => x.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && x.ACK_DSC_STATUS == "ACCP");
                    PFMS_OMMAS_PAYMENT_SUCCESS paymentSuccess = dbContext.PFMS_OMMAS_PAYMENT_SUCCESS.Where(x => x.ADMIN_ND_CODE == AdminDeptPIU.MAST_PARENT_ND_CODE).FirstOrDefault();
                    model.IsAgencyMapped = (initparty == null) ? false : true;
                    model.IsSRRDABankDetailsFinalized = (BankDetails == null) ? false : true;
                    model.IsDSCEnrollmentFinalized = (dsc == null) ? false : true;
                    model.IsEmailAvailable = (officer == null) ? false : !String.IsNullOrEmpty(officer.ADMIN_NO_EMAIL.Trim());
                    model.IsPaymentSuccess = (paymentSuccess == null) ? false : true;
                }

                if (module_Type.Equals("R"))
                {
                    REAT_INITIATING_PARTY_MASTER initparty = dbContext.REAT_INITIATING_PARTY_MASTER.FirstOrDefault(s => s.ADMIN_ND_CODE == AdminDeptPIU.MAST_PARENT_ND_CODE);
                    //comment on : 11 oct 2021
                    //REAT_OMMAS_DSC_MAPPING dsc = dbContext.REAT_OMMAS_DSC_MAPPING.OrderByDescending(x => x.FILE_PROCESS_DATE).FirstOrDefault(x => x.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && x.ADMIN_NO_OFFICER_CODE == officer.ADMIN_NO_OFFICER_CODE && x.ACK_DSC_STATUS == "ACCP" && x.IS_ACTIVE == true);
                    REAT_OMMAS_DSC_MAPPING dsc = new REAT_OMMAS_DSC_MAPPING();
                    dsc = null;
                    //if (PMGSYSession.Current.LevelId == 5)
                    //if (PMGSYSession.Current.LevelId == 5 || (PMGSYSession.Current.FundType.Equals("A") && PMGSYSession.Current.LevelId == 4))
                    if (PMGSYSession.Current.LevelId == 5)
                    {
                        dsc = dbContext.REAT_OMMAS_DSC_MAPPING.OrderByDescending(x => x.FILE_PROCESS_DATE).FirstOrDefault(x => x.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && x.ADMIN_NO_OFFICER_CODE == officer.ADMIN_NO_OFFICER_CODE && x.ACK_DSC_STATUS == "ACCP" && x.IS_ACTIVE == true && x.FUND_TYPE == "P");
                    }
                    //comment on : 11 oct 2021
                    //REAT_OMMAS_PAYMENT_SUCCESS paymentSuccess = dbContext.REAT_OMMAS_PAYMENT_SUCCESS.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).FirstOrDefault();
                    REAT_OMMAS_PAYMENT_SUCCESS paymentSuccess = new REAT_OMMAS_PAYMENT_SUCCESS();
                    if (PMGSYSession.Current.LevelId == 5 || (PMGSYSession.Current.FundType.Equals("A") && PMGSYSession.Current.LevelId == 4))
                    {
                        paymentSuccess = dbContext.REAT_OMMAS_PAYMENT_SUCCESS.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && x.FUND_TYPE == PMGSYSession.Current.FundType).FirstOrDefault();
                    }

                    // paymentSuccess = true;
                    if (initparty != null)
                    {
                        if (!(string.IsNullOrEmpty(initparty.REAT_INIT_PARTY_UNIQUE_CODE) || string.IsNullOrEmpty(initparty.SCHEME_CODE)))
                        {
                            model.IsAgencyMapped = true;
                        }
                        else
                        {
                            model.IsAgencyMapped = false;
                        }

                    }
                    else
                    {
                        model.IsAgencyMapped = false;
                    }

                    model.IsSRRDABankDetailsFinalized = (BankDetails == null) ? false : true;
                    // if (PMGSYSession.Current.LevelId == 5)
                    //if (PMGSYSession.Current.LevelId == 5 || (PMGSYSession.Current.FundType.Equals("A") && PMGSYSession.Current.LevelId == 4))
                    if (PMGSYSession.Current.LevelId == 5)
                    {
                        model.IsDSCEnrollmentFinalized = (dsc == null) ? false : true;
                    }
                    else
                    {
                        model.IsDSCEnrollmentFinalized = true;
                    }
                    if (PMGSYSession.Current.LevelId == 5)
                    {
                        model.IsEmailAvailable = (officer == null) ? false : !String.IsNullOrEmpty(officer.ADMIN_NO_EMAIL.Trim());
                    }
                    else
                    {
                        model.IsEmailAvailable = true;
                    }
                    //if (PMGSYSession.Current.LevelId == 5)
                    //if (PMGSYSession.Current.LevelId == 5 || (PMGSYSession.Current.FundType.Equals("A") && PMGSYSession.Current.LevelId == 4))
                    //{
                    //    model.IsPaymentSuccess = (paymentSuccess == null) ? false : true;
                    //}
                    //else
                    //{
                    //    model.IsPaymentSuccess = true;
                    //}
                    model.IsPaymentSuccess = true; //Added on 03-12-2021
                }
                return model;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PaymentDAL.ValidatePFMSPaymentDetails()");
                return null;
            }
        }
        #endregion

        #region PFMS Execution Payment Validation Configuration
        public List<SelectListItem> PopulateRoadsDAL(int adminNdCode, string srrdaDpiu, int year)
        {
            dbContext = new PMGSYEntities();
            List<SelectListItem> lstRoads = null;
            List<int> lstAdminNdCode = null;
            try
            {
                //var lstAdminNdCode = (dynamic)null;//dbContext.ADMIN_DEPARTMENT.Where(c => c.MAST_PARENT_ND_CODE == adminNdCode).ToList();

                if (srrdaDpiu.Trim() == "S")
                {
                    lstAdminNdCode = dbContext.ADMIN_DEPARTMENT.Where(c => c.MAST_PARENT_ND_CODE == adminNdCode).Select(v => v.ADMIN_ND_CODE).ToList();

                    lstRoads = new SelectList(dbContext.IMS_SANCTIONED_PROJECTS.Where(m => lstAdminNdCode.Contains(m.MAST_DPIU_CODE) && m.IMS_YEAR == year && m.IMS_SANCTIONED == "Y").OrderBy(s => s.IMS_PR_ROAD_CODE), "IMS_PR_ROAD_CODE", "IMS_ROAD_NAME").ToList();
                }
                else
                {
                    lstRoads = new SelectList(dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.MAST_DPIU_CODE == adminNdCode && m.IMS_YEAR == year && m.IMS_SANCTIONED == "Y").OrderBy(s => s.IMS_ROAD_NAME), "IMS_PR_ROAD_CODE", "IMS_ROAD_NAME").ToList();
                }
                //if (isAllSelected == false)
                //{
                //    lstDistrict.Insert(0, (new SelectListItem { Text = "Select District", Value = "0", Selected = true }));
                //}
                //else if (isAllSelected == true)
                //{
                //    lstDistrict.Insert(0, (new SelectListItem { Text = "All District", Value = "-1", Selected = true }));
                //}
                lstRoads.Insert(0, (new SelectListItem { Text = "Select Road", Value = "0", Selected = true }));
                return lstRoads;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PaymentDAL.PopulateRoadsDAL");
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool AddValidationDetailsDAL(ExecPaymentValidationViewModel model, ref string message)
        {
            dbContext = new PMGSYEntities();
            CommonFunctions comm = new CommonFunctions();

            DateTime frmDt = comm.GetStringToDateTime(model.fromDate);
            DateTime toDt = comm.GetStringToDateTime(model.toDate);

            //For All PIU
            if (model.srrda_Dpiu == "D" && model.DPIU == 0)
            {
                model.srrda_Dpiu = "S";
            }
            try
            {
                if (model.srrda_Dpiu == "S")
                {
                    if (dbContext.PAYMENT_EXEC_VALIDATION_CONFIGURATION.Any(x => x.ADMIN_ND_CODE == model.SRRDA && x.SRRDA_PIU == model.srrda_Dpiu && x.IMS_PR_ROAD_CODE == null
                        //&& ((x.FROM_DATE <= frmDt && x.TO_DATE >= frmDt) || (x.FROM_DATE <= toDt && x.TO_DATE >= toDt))))
                        && ((x.FROM_DATE >= frmDt && x.FROM_DATE <= toDt) || (x.TO_DATE >= frmDt && x.TO_DATE <= toDt))
                       ))
                    {
                        message = "Validation already skipped for SRRDA within the selected dates";
                        return false;
                    }
                }
                else
                {
                    int parentNdCode = dbContext.ADMIN_DEPARTMENT.Where(x => x.ADMIN_ND_CODE == model.DPIU).Select(c => c.MAST_PARENT_ND_CODE.Value).FirstOrDefault();

                    if (dbContext.PAYMENT_EXEC_VALIDATION_CONFIGURATION.Any(x => ((x.ADMIN_ND_CODE == parentNdCode && x.SRRDA_PIU == "S") || (x.ADMIN_ND_CODE == model.DPIU && x.SRRDA_PIU == "D"))
                        && (x.IMS_PR_ROAD_CODE == null || x.IMS_PR_ROAD_CODE == model.roadCode)
                        //&& ((x.FROM_DATE <= frmDt && x.TO_DATE >= frmDt) || (x.FROM_DATE <= toDt && x.TO_DATE >= toDt))))
                        && ((x.FROM_DATE >= frmDt && x.FROM_DATE <= toDt) || (x.TO_DATE >= frmDt && x.TO_DATE <= toDt))
                       ))
                    {
                        message = "Validation already skipped for SRRDA/PIU within the selected dates.";
                        return false;
                    }
                    //else if (dbContext.PAYMENT_EXEC_VALIDATION_CONFIGURATION.Any(x => x.ADMIN_ND_CODE == model.DPIU && x.SRRDA_PIU == "D" && x.IMS_PR_ROAD_CODE == null
                    //    && ((x.FROM_DATE <= frmDt && x.TO_DATE >= frmDt) || (x.FROM_DATE <= toDt && x.TO_DATE >= toDt))))
                    //{
                    //    message = "Validation already skipped for DPIU, all roads.";
                    //    return false;
                    //}
                }

                PAYMENT_EXEC_VALIDATION_CONFIGURATION payment_exec_validation_configuration = new PAYMENT_EXEC_VALIDATION_CONFIGURATION();

                payment_exec_validation_configuration.ID = dbContext.PAYMENT_EXEC_VALIDATION_CONFIGURATION.Any() ? dbContext.PAYMENT_EXEC_VALIDATION_CONFIGURATION.Max(x => x.ID) + 1 : 1;
                payment_exec_validation_configuration.ADMIN_ND_CODE = model.srrda_Dpiu == "S" ? model.SRRDA : model.DPIU;
                payment_exec_validation_configuration.SRRDA_PIU = model.srrda_Dpiu;
                payment_exec_validation_configuration.IMS_PR_ROAD_CODE = model.roadCode == 0 ? null : (int?)model.roadCode;
                payment_exec_validation_configuration.FROM_DATE = comm.GetStringToDateTime(model.fromDate);
                payment_exec_validation_configuration.TO_DATE = comm.GetStringToDateTime(model.toDate);

                payment_exec_validation_configuration.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                payment_exec_validation_configuration.USERID = PMGSYSession.Current.UserId;

                dbContext.PAYMENT_EXEC_VALIDATION_CONFIGURATION.Add(payment_exec_validation_configuration);

                dbContext.SaveChanges();

                message = "Record added successfully.";
                return true;
            }
            catch (DbUpdateException ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "PaymentDAL.AddValidationDetailsDAL.DbUpdateException");
                message = "An Error Occurred While Your Processing Request.";
                return false;
            }
            catch (OptimisticConcurrencyException ex)
            {
                ErrorLog.LogError(ex, "PaymentDAL.AddValidationDetailsDAL.OptimisticConcurrencyException");
                message = "An Error Occurred While Your Processing Request.";
                return false;
            }

            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PaymentDAL.AddValidationDetailsDAL");
                message = "An Error Occurred While Your Processing Request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array GetValidationDetailsDAL(int adminNdCode, string frmDt1, string toDt1, int? page, int? rows, string sidx, string sord, out Int32 totalRecords)
        {
            string date = string.Empty;
            int agencyCount = 0, stateCount = 0;
            CommonFunctions comm = new CommonFunctions();

            DateTime frmDt = comm.GetStringToDateTime(frmDt1);
            DateTime toDt = comm.GetStringToDateTime(toDt1);
            try
            {

                dbContext = new Models.PMGSYEntities();
                var ValidationDetails = (from exec in dbContext.PAYMENT_EXEC_VALIDATION_CONFIGURATION
                                         where
                                         exec.ADMIN_ND_CODE == adminNdCode
                                             //&& ((exec.FROM_DATE <= frmDt && exec.TO_DATE >= frmDt) || (exec.FROM_DATE <= toDt && exec.TO_DATE >= toDt))
                                         && ((exec.FROM_DATE >= frmDt && exec.FROM_DATE <= toDt) || (exec.TO_DATE >= frmDt && exec.TO_DATE <= toDt))
                                         select new
                                         {
                                             ADMIN_ND_NAME = exec.SRRDA_PIU == "S" ? "All DPIU" : exec.ADMIN_DEPARTMENT.ADMIN_ND_NAME,
                                             RoadName = exec.IMS_PR_ROAD_CODE.HasValue ? exec.IMS_SANCTIONED_PROJECTS.IMS_ROAD_NAME : "All Roads",
                                             exec.FROM_DATE,
                                             exec.TO_DATE
                                         }).Distinct().ToList();

                totalRecords = ValidationDetails.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "ContractorName":
                                ValidationDetails = ValidationDetails.OrderBy(x => x.ADMIN_ND_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "ContractorName":
                                ValidationDetails = ValidationDetails.OrderByDescending(x => x.ADMIN_ND_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                break;
                        }
                    }
                }
                else
                {
                    ValidationDetails = ValidationDetails.OrderByDescending(x => x.ADMIN_ND_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                var result = ValidationDetails.Select(valDetails => new
                {
                    valDetails.ADMIN_ND_NAME,
                    valDetails.RoadName,
                    FROM_DATE = valDetails.FROM_DATE.ToString("dd/MM/yyyy"),
                    TO_DATE = valDetails.TO_DATE.ToString("dd/MM/yyyy"),
                    //flag = checkDate(conDetails.BATCH_ID, out date),
                    //date
                }).ToArray();

                return result.Select(lstValDetails => new
                {
                    //id = lstcontractorDetails.ADMIN_ND_NAME.ToString(),
                    cell = new[] {      
                                    
                                    lstValDetails.ADMIN_ND_NAME,
                                    lstValDetails.RoadName,
                                    lstValDetails.FROM_DATE,
                                    lstValDetails.TO_DATE,
                    }

                }).ToArray();
                //return contractorDetails.ToArray();
            }
            catch (Exception ex)
            {
                totalRecords = 0;
                ErrorLog.LogError(ex, "PFMSDAL.GetValidationDetailsDAL()");
                return null;
            }
        }

        #endregion

        #region Security Deposit opening Balance Entry
        //added by hrishikesh --start
        public int SecDepOBUATAllowMaxTxnCount()
        {
            try
            {

                XDocument doc_xml = XDocument.Load(System.Web.HttpContext.Current.Server.MapPath("~/HoldingSecurityStateConfigFile.xml"));
                List<SDOBAllowTxnModel> SDOBAllowMaxTxnNoModelList = new List<SDOBAllowTxnModel>();
                foreach (XElement element in doc_xml.Descendants("stateList").Descendants("SDOBTxnCount").Descendants("stateUAT"))
                {
                    SDOBAllowTxnModel SDOBAllowMaxTxnNoModel = new SDOBAllowTxnModel();
                    SDOBAllowMaxTxnNoModel.StateCode = Convert.ToInt32(element.Descendants("statCode").FirstOrDefault().Value);
                    SDOBAllowMaxTxnNoModel.AdminNdCode = Convert.ToInt32(element.Descendants("adminNDCode").FirstOrDefault().Value);
                    SDOBAllowMaxTxnNoModel.maxAllowCount = Convert.ToInt32(element.Descendants("maxAllowCount").FirstOrDefault().Value);
                    SDOBAllowMaxTxnNoModelList.Add(SDOBAllowMaxTxnNoModel);

                }
                //xml Code end
                int maxAllowTxnCount = 2;

                foreach (SDOBAllowTxnModel item in SDOBAllowMaxTxnNoModelList)
                {
                    if (item.AdminNdCode == PMGSYSession.Current.AdminNdCode && item.StateCode == PMGSYSession.Current.StateCode)
                    {
                        maxAllowTxnCount = item.maxAllowCount;
                        break;
                    }
                }
                return maxAllowTxnCount;

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PaymentController.SecDepOBUATAllowTxnCount()");
                throw;
            }
        }



        public Int64 AddSecurityDepositAccOpeningBalanceDAL(SecurityDepositAccOpeningBalanceEntryModel securitydepositaccopeningbalanceentry)
        {
            dbContext = new PMGSYEntities();
            CommonFunctions comObj = new CommonFunctions();
            ACC_BILL_MASTER accBillMasterObj = new ACC_BILL_MASTER();
            ACC_BILL_DETAILS accBillDetailsCreditObj = new ACC_BILL_DETAILS();
            ACC_BILL_DETAILS accBillDetailsDebitObj = new ACC_BILL_DETAILS();
            //ACC_TXN_HEAD_MAPPING accTxnHeadMappingObj = new ACC_TXN_HEAD_MAPPING();

            try
            {
                var adminNdCode = PMGSYSession.Current.AdminNdCode;
                var fundType = PMGSYSession.Current.FundType;

                int insertedCount = dbContext.ACC_BILL_MASTER.Where(x => x.TXN_ID == 3199 & x.LVL_ID == 4 & x.ADMIN_ND_CODE == adminNdCode & x.FUND_TYPE == fundType).Count();

                //var presentOrNot = dbContext.ACC_BILL_MASTER.Where(x => x.TXN_ID == 3199 & x.LVL_ID == 4 & x.ADMIN_ND_CODE==adminNdCode & x.FUND_TYPE==fundType).Any();
                //if (dbContext.ACC_BILL_MASTER.Any(x => x.TXN_ID == 3199 & x.LVL_ID == 4 & x.ADMIN_ND_CODE == adminNdCode & x.FUND_TYPE == fundType))
                //{
                //    return -1;
                //}


                #region Region to read HoldingSecurityStateConfigFile XML File
                int AllowMaxTxnCount = 2;
                AllowMaxTxnCount = SecDepOBUATAllowMaxTxnCount();
                #endregion

                if (insertedCount >= AllowMaxTxnCount)
                {
                    return -1;
                }
                else
                {
                    using (TransactionScope transaction = new TransactionScope())
                    {
                        if (securitydepositaccopeningbalanceentry != null)
                        {
                            var billId = dbContext.ACC_BILL_MASTER.Any() ? dbContext.ACC_BILL_MASTER.Max(x => x.BILL_ID) + 1 : 1;
                            accBillMasterObj.BILL_ID = billId;
                            accBillMasterObj.BILL_NO = securitydepositaccopeningbalanceentry.BILL_NO;
                            accBillMasterObj.BILL_MONTH = (short)DateTime.Now.Month;
                            accBillMasterObj.BILL_YEAR = (short)DateTime.Now.Year;
                            accBillMasterObj.BILL_DATE = comObj.GetStringToDateTime(securitydepositaccopeningbalanceentry.BILL_DATE);
                            accBillMasterObj.TXN_ID = 3199;
                            accBillMasterObj.CHQ_Book_ID = null;
                            accBillMasterObj.CHQ_NO = securitydepositaccopeningbalanceentry.EPAY_NO;
                            accBillMasterObj.CHQ_DATE = comObj.GetStringToDateTime(securitydepositaccopeningbalanceentry.BILL_DATE);
                            accBillMasterObj.CHQ_AMOUNT = (decimal)securitydepositaccopeningbalanceentry.TOTAL_AMOUNT;
                            accBillMasterObj.CASH_AMOUNT = 0;
                            accBillMasterObj.GROSS_AMOUNT = (decimal)securitydepositaccopeningbalanceentry.TOTAL_AMOUNT;
                            accBillMasterObj.CHALAN_NO = null;
                            accBillMasterObj.CHALAN_DATE = null;
                            accBillMasterObj.PAYEE_NAME = dbContext.MASTER_CONTRACTOR.Where(x => x.MAST_CON_ID == securitydepositaccopeningbalanceentry.MAST_CON_ID_C).Select(x => x.MAST_CON_COMPANY_NAME).FirstOrDefault() == null ? "" : dbContext.MASTER_CONTRACTOR.Where(x => x.MAST_CON_ID == securitydepositaccopeningbalanceentry.MAST_CON_ID_C).Select(x => x.MAST_CON_COMPANY_NAME).FirstOrDefault(); //-----
                            accBillMasterObj.CHQ_EPAY = "E";
                            accBillMasterObj.TEO_TRANSFER_TYPE = null;
                            accBillMasterObj.REMIT_TYPE = null;
                            accBillMasterObj.BILL_FINALIZED = "N";
                            accBillMasterObj.FUND_TYPE = "P";
                            accBillMasterObj.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                            accBillMasterObj.LVL_ID = 4;
                            accBillMasterObj.MAST_CON_ID = securitydepositaccopeningbalanceentry.MAST_CON_ID_C;
                            accBillMasterObj.BILL_TYPE = "P";
                            accBillMasterObj.ACTION_REQUIRED = "N";
                            accBillMasterObj.USERID = PMGSYSession.Current.UserId;
                            accBillMasterObj.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];//**********
                            accBillMasterObj.ADMIN_NO_OFFICER_CODE = null;
                            accBillMasterObj.AUTH_ID = null;
                            accBillMasterObj.CON_ACCOUNT_ID = securitydepositaccopeningbalanceentry.CONC_Account_ID;

                            dbContext.ACC_BILL_MASTER.Add(accBillMasterObj);
                            dbContext.SaveChanges();  //-------

                            var accTxnHeadMappingCreditObj = dbContext.ACC_TXN_HEAD_MAPPING.Where(x => x.TXN_ID == 3200 & x.CREDIT_DEBIT == "C").FirstOrDefault();
                            var accTxnHeadMappingDebitObj = dbContext.ACC_TXN_HEAD_MAPPING.Where(x => x.TXN_ID == 3200 & x.CREDIT_DEBIT == "D").FirstOrDefault();

                            //add data to details table
                            //For Credit option
                            accBillDetailsCreditObj.BILL_ID = billId;
                            accBillDetailsCreditObj.TXN_NO = 1;
                            accBillDetailsCreditObj.AMOUNT = (decimal)securitydepositaccopeningbalanceentry.TOTAL_AMOUNT;
                            accBillDetailsCreditObj.NARRATION = "Security deposit opening balances transfer to SD bank account";
                            accBillDetailsCreditObj.ADMIN_ND_CODE = null;
                            accBillDetailsCreditObj.MAST_CON_ID = null;
                            accBillDetailsCreditObj.IMS_PR_ROAD_CODE = null;
                            accBillDetailsCreditObj.IMS_AGREEMENT_CODE = null;
                            accBillDetailsCreditObj.MAS_FA_CODE = null;
                            accBillDetailsCreditObj.FINAL_PAYMENT = null;
                            accBillDetailsCreditObj.MAST_DISTRICT_CODE = null;
                            accBillDetailsCreditObj.USERID = PMGSYSession.Current.UserId;
                            accBillDetailsCreditObj.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            //accBillDetailsCreditObj.TXN_ID = 3199; ---change it
                            accBillDetailsCreditObj.TXN_ID = 3200;
                            /*      if(accTxnHeadMappingCreditObj==null)
                                  {
                                      accBillDetailsCreditObj.HEAD_ID = 0;
                                      accBillDetailsCreditObj.CASH_CHQ = "Q";
                                  }
                                  else
                                  {*/
                            accBillDetailsCreditObj.HEAD_ID = accTxnHeadMappingCreditObj.HEAD_ID == null ? (short)0 : accTxnHeadMappingCreditObj.HEAD_ID;
                            accBillDetailsCreditObj.CASH_CHQ = accTxnHeadMappingCreditObj.CASH_CHQ == "" ? "" : accTxnHeadMappingCreditObj.CASH_CHQ.Trim();

                            //}
                            accBillDetailsCreditObj.CREDIT_DEBIT = "C";
                            dbContext.ACC_BILL_DETAILS.Add(accBillDetailsCreditObj);
                            dbContext.SaveChanges();  //-------



                            //For Debit option
                            //add data to details table
                            accBillDetailsDebitObj.BILL_ID = billId;
                            accBillDetailsDebitObj.TXN_NO = 2;
                            accBillDetailsDebitObj.AMOUNT = (decimal)securitydepositaccopeningbalanceentry.TOTAL_AMOUNT;
                            accBillDetailsDebitObj.NARRATION = "Security deposit opening balances transfer to SD bank account";
                            accBillDetailsDebitObj.ADMIN_ND_CODE = null;
                            accBillDetailsDebitObj.MAST_CON_ID = null;
                            accBillDetailsDebitObj.IMS_PR_ROAD_CODE = null;
                            accBillDetailsDebitObj.IMS_AGREEMENT_CODE = null;
                            accBillDetailsDebitObj.MAS_FA_CODE = null;
                            accBillDetailsDebitObj.FINAL_PAYMENT = null;
                            accBillDetailsDebitObj.MAST_DISTRICT_CODE = null;
                            accBillDetailsDebitObj.USERID = PMGSYSession.Current.UserId;
                            accBillDetailsDebitObj.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            //accBillDetailsDebitObj.TXN_ID = 3199;  ---change it 
                            accBillDetailsDebitObj.TXN_ID = 3200;
                            /*    if(accBillDetailsDebitObj==null)
                                {
                                    accBillDetailsDebitObj.HEAD_ID = 0;
                                    accBillDetailsDebitObj.CASH_CHQ = "Q";
                                }
                                else
                                {*/
                            accBillDetailsDebitObj.HEAD_ID = accTxnHeadMappingDebitObj.HEAD_ID == null ? (short)0 : accTxnHeadMappingDebitObj.HEAD_ID;   //dont set it 
                            accBillDetailsDebitObj.CASH_CHQ = accTxnHeadMappingDebitObj.CASH_CHQ == "" ? "" : accTxnHeadMappingDebitObj.CASH_CHQ.Trim();
                            //}
                            accBillDetailsDebitObj.CREDIT_DEBIT = "D";
                            dbContext.ACC_BILL_DETAILS.Add(accBillDetailsDebitObj);
                            dbContext.SaveChanges();  //-------

                            //add entry t0 Cheque Issue Table 
                            ACC_CHEQUES_ISSUED chequeIssuedModel = new ACC_CHEQUES_ISSUED();
                            chequeIssuedModel.BILL_ID = billId;
                            chequeIssuedModel.BANK_CODE = dbContext.ACC_BANK_DETAILS.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.ParentNDCode && x.FUND_TYPE == "P" && x.BANK_ACC_STATUS == true && x.ACCOUNT_TYPE == "S").Select(z => z.BANK_CODE).FirstOrDefault(); ;
                            chequeIssuedModel.CHEQUE_STATUS = "N";
                            chequeIssuedModel.USERID = PMGSYSession.Current.UserId;
                            chequeIssuedModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            dbContext.ACC_CHEQUES_ISSUED.Add(chequeIssuedModel);
                            dbContext.SaveChanges();

                            //use to check voucher no is repeating or not 
                            int fiscalYear = 0;
                            if (DateTime.Now.Month <= 3)
                            {
                                fiscalYear = (DateTime.Now.Year - 1);
                            }
                            else
                            {
                                fiscalYear = DateTime.Now.Year;
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
                            dbContext.SaveChanges();


                            transaction.Complete(); //last 

                        }

                    }

                    return 1;
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
                return 0;
            }

            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }

            //return 1;
        }

        public Array GetSecurityDepositAccOpeningBalanceJsonDAL(int page, int rows, string sidx, string sord, string filters, int month, int year, out long totalRecords)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<ACC_BILL_MASTER> accBillMasterList = null;
                List<SecurityDepositAccOpeningBalanceEntryModel> List = new List<SecurityDepositAccOpeningBalanceEntryModel>();

                //var accBillMasterListDetails = dbContext.ACC_BILL_MASTER.Where(x => x.BILL_MONTH == month & x.BILL_YEAR == year).ToList();
                //accBillMasterList = dbContext.ACC_BILL_MASTER.Where(x => x.BILL_MONTH == month & x.BILL_YEAR == year).ToList();  //--- change herer also for tranasaction id 

                accBillMasterList = dbContext.ACC_BILL_MASTER.Where(x => x.TXN_ID == 3199 & x.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode & x.BILL_MONTH == month & x.BILL_YEAR == year).ToList();  //--- change herer also for tranasaction id 

                totalRecords = accBillMasterList.Count;

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        accBillMasterList = accBillMasterList.OrderBy(x => x.BILL_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        accBillMasterList = accBillMasterList.OrderByDescending(x => x.BILL_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    accBillMasterList = accBillMasterList.OrderBy(x => x.BILL_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return accBillMasterList.Select(item => new
                {
                    //id=item.BILL_ID,
                    id = URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() }),
                    cell = new[]
                        {
/*                            item.BILL_ID.ToString(),
                            item.BILL_DATE==null ? "-" : item.BILL_DATE.ToString().Split(' ')[0].Replace('-','/'), //Voucher Date
                            item.CHQ_NO==""? "-":item.CHQ_NO.ToString(), //Cheque / Epayment / Advice Number
                            item.PAYEE_NAME=="" ? "-" :item.PAYEE_NAME.ToString(), //Contractor/Payee Name

                            //accBillMasterListDetailsN.CHQ_AMOUNT==null ? " " :accBillMasterListDetailsN.CHQ_AMOUNT.ToString(), //total amt
                            item.CHQ_AMOUNT.ToString(), //total amt
                            //accBillMasterListDetailsN.GROSS_AMOUNT==null ? " " :accBillMasterListDetailsN.CHQ_AMOUNT.ToString() //grosss amt
                            item.GROSS_AMOUNT.ToString() //grosss amt
*/

                            item.BILL_NO.ToString(), //d
                            item.BILL_DATE==null ? "-" : item.BILL_DATE.ToString().Split(' ')[0].Replace('-','/'), //Voucher Date,   D
                            item.CHQ_NO==null? "-":item.CHQ_NO.ToString(), //Cheque / Epayment / Advice Number    D
                            //string.Empty,
                            item.PAYEE_NAME==null ? "-" :item.PAYEE_NAME.ToString(), //Contractor/Payee Name
                            item.CHQ_AMOUNT.ToString(), //total amt,
                            item.GROSS_AMOUNT.ToString(), //grosss amt,
                            //"<a href='#' title='Click here to finalize' class='ui-icon ui-icon-unlocked ui-align-center' onClick='FinalizeRecord( );' return false;>Update Details</a>",
                            //item.BILL_FINALIZED=="N" && item.TXN_ID != 625 && CanVoucherFinalized(item.BILL_ID) ? "<center><a href='#' class='ui-icon ui-icon-unlocked' onclick='FinalizePayment(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()})+ "\");return false;'>Lock</a></center>" : (item.BILL_FINALIZED=="Y" || item.BILL_FINALIZED=="E") &&  item.TXN_ID != 228 &&  item.TXN_ID !=229 &&  item.TXN_ID !=624 &&  item.TXN_ID !=625 &&  item.TXN_ID !=825&&  item.TXN_ID !=824 &&  item.TXN_ID !=3077 ?  "<center><a href='#' class='ui-icon ui-icon-search' onclick='ViewPayment(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()})+ "\");return false;'>View</a></center>":string.Empty, //renewed txnid 228,624,825 ,cancelled =229,625,824
                            //item.BILL_FINALIZED=="N" && CanVoucherFinalized(item.BILL_ID) ? "<center><a href='#' class='ui-icon ui-icon-unlocked' onclick='FinalizePayment(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()})+ "\");return false;'>Lock</a></center>" :"<center><a href='#' class='ui-icon ui-icon-check'>View</a></center>"
                            //item.BILL_FINALIZED=="N"  ? "<center><a href='#' class='ui-icon ui-icon-unlocked' onclick='FinalizePayment(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()})+ "\");return false;'>Lock</a></center>" : "<center><a href='#' class='ui-icon ui-icon-check'>View</a></center>"
                            item.BILL_FINALIZED=="N"  ? "<center><a href='#' class='ui-icon ui-icon-unlocked' onclick='FinalizePayment(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()})+ "\");return false;'>Lock</a></center>" : (item.BILL_FINALIZED=="Y") ?  "<center><a href='#' class='ui-icon ui-icon-check' onclick='ViewPayment(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim()})+ "\");return false;'>View</a></center>":string.Empty,

                        }
                }).ToArray();

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PaymentDAL.GetSecurityDepositAccOpeningBalanceJsonDAL()");   //ErrorLog is Class
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

        //added by hrishikesh --end
        #endregion

        #region Holding and SDA Transaction 

        public Array GetSDAandHoldingListDal(int month, int year, int AdminCode, int tXnid, int? page, int? rows, string sidx, string sord, out long totalRecords, out List<String> SelectedIdList)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            int adminNDCode = AdminCode;
            int TranxID = tXnid;
            try
            {
                var lstholdingDetail = dbContext.USP_ACC_HOLDING_SDA_TRANSACTION(month, year, adminNDCode).ToList<USP_ACC_HOLDING_SDA_TRANSACTION_Result>();
                totalRecords = lstholdingDetail.Count();
                if (TranxID == 1)
                {
                    lstholdingDetail = lstholdingDetail.Where(x => x.TXN_ID == 3171).ToList<USP_ACC_HOLDING_SDA_TRANSACTION_Result>();
                }
                if (TranxID == 2)
                {
                    lstholdingDetail = lstholdingDetail.Where(x => x.TXN_ID == 3183 || x.TXN_ID == 3185).ToList<USP_ACC_HOLDING_SDA_TRANSACTION_Result>();
                }
                List<Get_Acc_SDA_Holding_model> resultList = new List<Get_Acc_SDA_Holding_model>();
                //var resultList = new List<Get_Acc_SDA_Holding_model>();
                foreach (var item in lstholdingDetail)
                {
                    long BillID = Convert.ToInt64(item.BILL_ID);
                    string status = string.Empty;
                    var holdStatus = dbContext.ACC_HOLDING_SECURITY_AUTO_ENTRIES.Where(x => x.BILL_ID == BillID).FirstOrDefault();
                    if (holdStatus != null)
                    {
                        status = "Y";
                    }
                    else
                    {
                        status = "N";
                    }
                    Get_Acc_SDA_Holding_model obj = new Get_Acc_SDA_Holding_model();
                    obj.TXN_ID = item.TXN_ID;
                    obj.BILL_ID = item.BILL_ID;
                    obj.BILL_NO = item.BILL_NO.ToString();
                    obj.BILL_DATE = item.BILL_DATE.ToString().Split(' ')[0];
                    obj.TXN_DESC = item.TXN_DESC.ToString();
                    obj.CHQ_NO = item.CHQ_NO.ToString();
                    obj.CHQ_AMOUNT = item.CHQ_AMOUNT.ToString();
                    obj.CASH_AMOUNT = item.CASH_AMOUNT.ToString();
                    obj.GROSS_AMOUNT = item.GROSS_AMOUNT.ToString();
                    obj.BANK_ACK_BILL_STATUS = item.BANK_ACK_BILL_STATUS;
                    obj.holdingStatus = status;
                    resultList.Add(obj);
                }
                totalRecords = resultList.Count();

                List<long> SelectedIdListLong = new List<long>();
                SelectedIdList = new List<string>();
                //get previously acknowledged cheques
                SelectedIdListLong = (resultList.Where(c => c.holdingStatus == "Y").Select(y => y.BILL_ID)).ToList<long>();

                foreach (long item in SelectedIdListLong)
                {
                    // SelectedIdList.Add(URLEncrypt.EncryptParameters(new string[] { item.ToString().Trim() }));
                    SelectedIdList.Add(item.ToString());
                }

                return resultList.Select(m => new
                {
                    id = m.BILL_ID.ToString(),
                    cell = new[]
                    {
                      m.BILL_ID.ToString(),
                      m.BILL_NO== null ? "":m.BILL_NO.ToString(),
                      m.BILL_DATE == null ? "" : m.BILL_DATE.ToString(),
                      (m.TXN_ID == 3171) ? "Saving" : (m.TXN_ID == 3183 || m.TXN_ID == 3185) ? "Holding" : "--",
                      m.TXN_DESC == null ? "" : m.TXN_DESC.ToString(),
                      m.CHQ_NO == null ? "" : m.CHQ_NO.ToString(),
                      m.CHQ_AMOUNT.ToString(),
                      m.CASH_AMOUNT.ToString(),
                      m.GROSS_AMOUNT.ToString(),
                      m.holdingStatus =="Y" ? "<center><span class='ui-icon ui-icon-check clickBound'></span></center>" : "<center>-</center>",
                      m.holdingStatus =="Y" ? m.BILL_ID.ToString():"",
                      m.TXN_ID.ToString()
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                totalRecords = 0;
                ErrorLog.LogError(ex, "PaymentDAL.GetSDAandHoldingListDal()");
                SelectedIdList = new List<string>();
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        public String AutoGenerateTxnDAL(long[] array_bill_id, string TxnModel)
        {
            int txn_id = 0;
            int month, year = 0;
            int PiuCode = 0;
            string message = string.Empty;
            string[] arr = TxnModel.Split('$');
            month = Convert.ToInt32(arr[0]);
            year = Convert.ToInt32(arr[1]);
            PiuCode = Convert.ToInt32(arr[2]);
            txn_id = Convert.ToInt32(arr[3]);
            int PssTxnID = 0;
            if (txn_id == 1)
            {
                PssTxnID = 3171;
            }
            else if (txn_id == 2)
            {
                PssTxnID = 3185;
            }
            int SrrdaNDcODE = Convert.ToInt32(PMGSYSession.Current.ParentNDCode);

            try
            {
                DataTable BillIds = new DataTable();
                //Create Column
                BillIds.Columns.Add("BILL_ID", typeof(long));
                if (array_bill_id != null)
                {
                    foreach (long billId in array_bill_id)
                    {
                        BillIds.Rows.Add(new object[] { billId });
                    }
                }

                string comntExec = string.Empty;
                SqlConnection storeConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["PMGSYConnection"].ConnectionString);
                using (SqlCommand command = storeConnection.CreateCommand())
                {
                    command.Connection = storeConnection;
                    storeConnection.Open();
                    command.CommandText = "omms.USP_ACC_INSERT_AUTOTXNS_ATSRRDA";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@bill_Id", BillIds).SqlDbType = SqlDbType.Structured;
                    command.Parameters.Add(new SqlParameter("@txn_id", SqlDbType.Int)).Value = PssTxnID;   // REQUIRED TO BE CHANGE
                    command.Parameters.Add(new SqlParameter("@BILL_MONTH", SqlDbType.Int)).Value = month;
                    command.Parameters.Add(new SqlParameter("@BILL_YEAR", SqlDbType.Int)).Value = year;
                    command.Parameters.Add(new SqlParameter("@piu_code", SqlDbType.Int)).Value = PiuCode;
                    command.Parameters.Add(new SqlParameter("@srrdacode", SqlDbType.Int)).Value = SrrdaNDcODE;
                    comntExec = command.ExecuteScalar().ToString();
                    storeConnection.Close();

                }
                return comntExec;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "PaymentDAL.AutoGenerateTxnDAL()");
                message = "Error Occured while Processing Request.";
                return message;
            }
        }

        #endregion


    }

    public interface IPaymentDAL
    {
        #region master Payment

        String GetChequeBookIssueDate(Int64 chqBookId);
        Int64 AddEditMasterPaymentDetails(PaymentMasterModel model, string operationType, Int64 Bill_id, bool IsAdvicePayment);
        List<SelectListItem> GetchequebookSeries(int admin_ND_Code, string fundType, Int16 levelID);
        //List<SelectListItem> GetchequebookNumbers(int chequeBookId, int admin_nd_code,string fund_type);
        Array ListMasterPaymentDetails(PaymentFilterModel objFilter, out long totalRecords);
        Int32 DeleteMasterPaymentDetails(Int64 Bill_Id);
        Array ListMasterPaymentDetailsForDataEntry(PaymentFilterModel objFilter, out long totalRecords, out long _MasterTxnID);
        ACC_BILL_MASTER GetMasterPaymentDetails(Int64 Bill_id);
        Array ListPaymentDeductionDetailsForDataEntry(PaymentFilterModel objFilter, out long totalRecords);
        bool IschequeIssued(string chequeNumber, string operation, long billId);

        //UDF_ACC_GEN_GET_BA_CASH_CLOSING_BALANCES_Result GetCloSingBalanceForPayment(TransactionParams param);
        UDF_ACC_GEN_GET_BA_CASH_CLOSING_BALANCES_COMPOSITE_Result GetCloSingBalanceForPayment(TransactionParams param);

        AmountCalculationModel CalculatePaymentAmounts(Int64 Bill_id);
        String CheckForAssetPaymentValidation(long bill_id, int headId, short transNumber, decimal newTransAmount);
        decimal? GetVoucherPayment(Int64 billId, out string paymentType);

        //MASTER_CONTRACTOR_BANK GetContratorBankAccNoAndIFSCcode(int contractorId, string fundType, int txnID, ref bool isPFMSFinalized, bool isAdvicePayment, bool isChqPaymentAllowed);
        List<SelectListItem> GetContratorBankAccNoAndIFSCcode(int contractorId, string fundType, int txnID, ref bool isPFMSFinalized, bool isAdvicePayment, bool isChqPaymentAllowed);
        #endregion

        #region Transaction deduction Payment
        Boolean AddEditTransactionDeductionPaymentDetails(PaymentDetailsModel model, string operationType, Int64 Bill_id, string AddorEdit, Int16 tranNumber);
        Int32 DeleteTransactionPaymentDetails(Int64 master_Bill_Id, Int32 tranNumber, String paymentDeduction);
        ACC_BILL_DETAILS GetTransactionPaymentDetails(Int64 BILL_ID, Int16 tranNumber, String paymentDeduction);
        String GetAgreemntNumberForVoucher(Int64 bill_id);
        bool CanVoucherFinalized(Int64 Bill_id);
        Int32 FinalizeVoucher(Int64 bill_id, bool Tofinalize);
        List<SelectListItem> GetFinalPaymentDetails(Int64 BILL_ID, Int32 roadID, Int32 subTxnID);
        String[] GetAllAvailableChequesArray(int chequeBookId, int admin_nd_code, string fund_type, string operation = "A", Int64 billID = 0);
        String GetEpayNumber(Int16 month, Int16 year, Int16 stateCode, int adminNdCode);
        String GetEremittanceNumber(Int16 month, Int16 year, Int16 stateCode, int adminNdCode);

        String GetChequeBookSeriesBasedOnCheque(Int32 admin_nd_code, String fund_type, Int32 chequeNumber);
        List<SelectListItem> GetAllFinalizedCheques(int chequeBookId, int admin_nd_code, string fund_type, string chequeSeries);
        #endregion

        #region Cheque Renwal

        String RenewCheque(Int64 bill_Id, ChequeRenewModel model);
        Array ListChequeDetailsForRenewalbyCheque(PaymentFilterModel objFilter, out long totalRecords, out Int64 bill_id);
        String CancelCheque(Int64 bill_Id, ChequeCancellModel model);
        //List<SelectListItem> populateFundTypeForCancellation(String fundType);
         List<SelectListItem> populateFundTypeForCancellation(String fundType, int level);
        #endregion

        #region EPAYMENT
        EpaymentOrderModel GetEpaymentDetailsFinalizedByAuthSig(Int64 bill_ID);
        EremittnaceOrderModel GetEremittanceDetails(Int64 bill_id);
        EremittnaceOrderModel GetEremittanceDetailsFinalizedByAuthSig(Int64 bill_id);
        EpaymentOrderModel GetEpaymentDetails(Int64 bill_id);
        Array ListEPaymentDetails(PaymentFilterModel objFilter, String TransactionType, out long totalRecords, string moduleType);
        String UnlockEpayment(Int64 bill_ID);
        String FinalizeEpayment(Int64 bill_ID, string DblHasedPassword);
        String FinalizeEremittance(Int64 bill_ID, string DblHasedPassword);
        String EncodePassword(string OriginalPasswordWithSalt);
        String InsertEpaymentMailDetails(Int64 bill_ID, String FileName);
        String InsertEremittanceMailDetails(Int64 bill_ID, String FileName);
        String DeleteMailDetails(long billId);


        #region vikky
        Array GetTransferDeductionAmtListDAL(PaymentFilterModel objFilter, String TransactionType, out long totalRecords, out List<string> voucherGeneratedList);

        Array GetTransferDeductionAmtGeneratedVoucherListDAL(PaymentFilterModel objFilter, String TransactionType, out long totalRecords);

        Boolean SubmitTransferDeductionAmountDAL(string[] billIdArray, string billNo, DateTime billDate, string chq_No, decimal deductionAmt);

        Array ListSecondLevelSuccessEPaymentDetails(PaymentFilterModel objFilter, String TransactionType, out long totalRecords, string moduleType);


        #endregion


        #endregion

        #region Reject / Resend Epayment

        Array GetEpaymentRejectResendList(PaymentFilterModel objFilter, string TransactionType, out long totalRecords);
        bool InsertResendEpaymentDetails(RejectResendFormModel model, long Bill_ID);

        bool CancelEpaymentEremDetails(RejectResendFormModel model, long Bill_ID, ref string message);

        #endregion Reject / Resend Epayment

        bool ValidateDPIUEpaymentDAL(int adminCode);
        bool ValidateDPIUERemittenceDAL(int adminCode);
        bool ValidateContractorStatus(int conId, int txnId);


        //Added By Abhishek kamble for Advice No 6Apr2015 start
        bool ValidateAdviceNoExist(String ChqNo, long Bill_ID);
        bool ValidateifMonthAcknowledged(int? srrdaCode, int dpiuCode, string fundType, Int16 BillMonth, Int16 BillYear);

        //added by hrishikesh --start
        Int64 AddSecurityDepositAccOpeningBalanceDAL(SecurityDepositAccOpeningBalanceEntryModel securitydepositaccopeningbalanceentry);
        Array GetSecurityDepositAccOpeningBalanceJsonDAL(int page, int rows, string sidx, string sord, string filters, int month, int year, out long totalRecords);

        //added by hrishikesh --end


        #region Holding and SDA Transaction 

        Array GetSDAandHoldingListDal(int month, int year, int AdminCode, int tXnid, int? page, int? rows, string sidx, string sord, out long totalRecords, out List<String> SelectedIdList);

        String AutoGenerateTxnDAL(long[] array_bill_id, string TxnModel);

        #endregion


    }
}