using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.Models;
using PMGSY.Common;
using System.Data.Entity.Validation;
using PMGSY.Models.ChequeBook;
using PMGSY.Extensions;
using System.Data.Entity;
using System.Configuration;
using System.Data.Entity.Core;

namespace PMGSY.DAL.ChequeBook
{
    public class ChequeBookDAL : IChequeBookDAL
    {
        PMGSYEntities dbContext = null;

        public Array ChequeBookList(int? page, int? rows, string sidx, string sord, string search, out long totalRecords, int AdminNdCode, int LevelId)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<ACC_CHQ_BOOK_DETAILS> lstChequeBookDetails = null;
                Int16 month = Convert.ToInt16(search.Split('$')[0]);
                Int16 year = Convert.ToInt16(search.Split('$')[1]);
                Int32 chequeNo = Convert.ToInt32(search.Split('$')[2]);
                //Int32 admin_nd_code = PMGSYSession.Current.AdminNdCode;
                Int32 admin_nd_code = AdminNdCode;
                String fund_type = PMGSYSession.Current.FundType;
                //Int16 level_id = PMGSYSession.Current.LevelId;
                int level_id = LevelId;
                if (month != 0 && year != 0 && chequeNo != 0)
                {
                    lstChequeBookDetails = dbContext.ACC_CHQ_BOOK_DETAILS.Where(m => m.ADMIN_ND_CODE == admin_nd_code && m.LVL_ID == level_id && m.FUND_TYPE == fund_type && m.ISSUE_DATE.Month == month && m.ISSUE_DATE.Year == year).ToList<ACC_CHQ_BOOK_DETAILS>().FindAll(m => Convert.ToInt32(m.LEAF_START) <= chequeNo && Convert.ToInt32(m.LEAF_END) >= chequeNo);
                }
                else
                {
                    if (month == 0 && year != 0 && chequeNo == 0)
                    {
                        //&& (m.IS_CHQBOOK_COMPLETED == null || m.IS_CHQBOOK_COMPLETED == false)
                        lstChequeBookDetails = dbContext.ACC_CHQ_BOOK_DETAILS.Where(m => m.ADMIN_ND_CODE == admin_nd_code && m.LVL_ID == level_id && m.FUND_TYPE == fund_type && m.ISSUE_DATE.Year == year).ToList<ACC_CHQ_BOOK_DETAILS>();
                    }
                    else if (year == 0 && month != 0 && chequeNo == 0)
                    {
                        //&& (m.IS_CHQBOOK_COMPLETED == null || m.IS_CHQBOOK_COMPLETED == false)
                        lstChequeBookDetails = dbContext.ACC_CHQ_BOOK_DETAILS.Where(m => m.ADMIN_ND_CODE == admin_nd_code && m.LVL_ID == level_id && m.FUND_TYPE == fund_type && m.ISSUE_DATE.Month == month).ToList<ACC_CHQ_BOOK_DETAILS>();
                    }
                    else if (year == 0 && month == 0 && chequeNo != 0)
                    {
                        lstChequeBookDetails = dbContext.ACC_CHQ_BOOK_DETAILS.Where(m => m.ADMIN_ND_CODE == admin_nd_code && m.LVL_ID == level_id && m.FUND_TYPE == fund_type).ToList<ACC_CHQ_BOOK_DETAILS>().FindAll(m => Convert.ToInt32(m.LEAF_START) <= chequeNo && Convert.ToInt32(m.LEAF_END) >= chequeNo);
                    }
                    else if (year == 0 && month != 0 && chequeNo != 0)
                    {
                        lstChequeBookDetails = dbContext.ACC_CHQ_BOOK_DETAILS.Where(m => m.ADMIN_ND_CODE == admin_nd_code && m.LVL_ID == level_id && m.FUND_TYPE == fund_type && m.ISSUE_DATE.Month == month).ToList<ACC_CHQ_BOOK_DETAILS>().FindAll(m => Convert.ToInt32(m.LEAF_START) <= chequeNo && Convert.ToInt32(m.LEAF_END) >= chequeNo);
                    }
                    else if (year != 0 && month == 0 && chequeNo != 0)
                    {
                        lstChequeBookDetails = dbContext.ACC_CHQ_BOOK_DETAILS.Where(m => m.ADMIN_ND_CODE == admin_nd_code && m.LVL_ID == level_id && m.FUND_TYPE == fund_type && m.ISSUE_DATE.Year == year).ToList<ACC_CHQ_BOOK_DETAILS>().FindAll(m => Convert.ToInt32(m.LEAF_START) <= chequeNo && Convert.ToInt32(m.LEAF_END) >= chequeNo);
                    }
                    else if (year != 0 && month != 0 && chequeNo == 0)
                    {
                        lstChequeBookDetails = dbContext.ACC_CHQ_BOOK_DETAILS.Where(m => m.ADMIN_ND_CODE == admin_nd_code && m.LVL_ID == level_id && m.FUND_TYPE == fund_type && m.ISSUE_DATE.Month == month && m.ISSUE_DATE.Year == year).ToList<ACC_CHQ_BOOK_DETAILS>();
                    }
                    else
                    {
                        lstChequeBookDetails = dbContext.ACC_CHQ_BOOK_DETAILS.Where(m => m.ADMIN_ND_CODE == admin_nd_code && m.LVL_ID == level_id && m.FUND_TYPE == fund_type).ToList<ACC_CHQ_BOOK_DETAILS>();
                    }
                }
                // Take ADMIN_ND_CODE from Seesion



                totalRecords = lstChequeBookDetails.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "ChqRecDate":
                                lstChequeBookDetails = lstChequeBookDetails.OrderBy(x => x.ISSUE_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList<ACC_CHQ_BOOK_DETAILS>();
                                break;
                            case "LeafStart":
                                lstChequeBookDetails = lstChequeBookDetails.OrderBy(x => x.LEAF_START).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList<ACC_CHQ_BOOK_DETAILS>();
                                break;
                            case "LeafEnd":
                                lstChequeBookDetails = lstChequeBookDetails.OrderBy(x => x.LEAF_END).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList<ACC_CHQ_BOOK_DETAILS>();
                                break;
                            default:
                                lstChequeBookDetails = lstChequeBookDetails.OrderBy(x => x.ISSUE_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList<ACC_CHQ_BOOK_DETAILS>();
                                break;
                        }

                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "ChqRecDate":
                                lstChequeBookDetails = lstChequeBookDetails.OrderByDescending(x => x.ISSUE_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList<ACC_CHQ_BOOK_DETAILS>();
                                break;
                            case "LeafStart":
                                lstChequeBookDetails = lstChequeBookDetails.OrderByDescending(x => x.LEAF_START).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList<ACC_CHQ_BOOK_DETAILS>();
                                break;
                            case "LeafEnd":
                                lstChequeBookDetails = lstChequeBookDetails.OrderByDescending(x => x.LEAF_END).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList<ACC_CHQ_BOOK_DETAILS>();
                                break;
                            default:
                                lstChequeBookDetails = lstChequeBookDetails.OrderByDescending(x => x.ISSUE_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList<ACC_CHQ_BOOK_DETAILS>();
                                break;
                        }
                    }
                }
                else
                {
                    lstChequeBookDetails = lstChequeBookDetails.OrderByDescending(x => x.ISSUE_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList<ACC_CHQ_BOOK_DETAILS>();
                }


                return lstChequeBookDetails.Select(item => new
                {
                    id = item.CHQ_BOOK_ID,
                    cell = new[] {                         
                                    Convert.ToDateTime(item.ISSUE_DATE).ToString("dd/MM/yyyy"),
                                    item.LEAF_START.Trim(),
                                    item.LEAF_END.Trim(),
                                    //"<center><a href='#' class='ui-icon ui-icon-pencil' onclick='EditChequeBook(\"" +URLEncrypt.EncryptParameters(new string[] { item.CHQ_BOOK_ID.ToString().Trim()})+ "\");return false;'>Edit</a></center>",
                                    //"<center><a href='#' class='ui-icon ui-icon-trash' onclick='DeleteChequeBook(\"" +URLEncrypt.EncryptParameters(new string[] { item.CHQ_BOOK_ID.ToString().Trim()})+ "\");return false;'>Delete</a></center>"                          
                                  item.IS_CHQBOOK_COMPLETED == true ? "<span title='Chque book is issued'>-</span>" :  dbContext.ACC_BILL_MASTER.Where(m=>m.CHQ_Book_ID==item.CHQ_BOOK_ID && m.FUND_TYPE==PMGSYSession.Current.FundType &&m.ADMIN_ND_CODE==AdminNdCode && m.LVL_ID==LevelId ).Any()?"<span title='Chque book is issued'>-</span>": "<center><a href='#' class='ui-icon ui-icon-pencil' onclick='EditChequeBook(\"" +URLEncrypt.EncryptParameters(new string[] { item.CHQ_BOOK_ID.ToString().Trim()})+ "\");return false;'>Edit</a></center>",
                                 item.IS_CHQBOOK_COMPLETED == true ? "<span title='Chque book is issued'>-</span>": dbContext.ACC_BILL_MASTER.Where(m=>m.CHQ_Book_ID==item.CHQ_BOOK_ID && m.FUND_TYPE==PMGSYSession.Current.FundType &&m.ADMIN_ND_CODE==AdminNdCode && m.LVL_ID==LevelId).Any()?"<span title='Chque book is issued'>-</span>": "<center><a href='#' class='ui-icon ui-icon-trash' onclick='DeleteChequeBook(\"" +URLEncrypt.EncryptParameters(new string[] { item.CHQ_BOOK_ID.ToString().Trim()})+ "\");return false;'>Delete</a></center>"                          
                             
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

        public String DeleteChequeBook(int chequeBookId)
        {
            try
            {
                dbContext = new PMGSYEntities();
                ACC_CHQ_BOOK_DETAILS cheque_details = dbContext.ACC_CHQ_BOOK_DETAILS.Where(m => m.CHQ_BOOK_ID == chequeBookId).FirstOrDefault();

                //&& m.ACC_CHEQUES_ISSUED.BILL_ID == m.BILL_ID
                if (dbContext.ACC_BILL_MASTER.Where(m => m.ADMIN_ND_CODE == cheque_details.ADMIN_ND_CODE && m.FUND_TYPE == PMGSYSession.Current.FundType && m.CHQ_Book_ID == cheque_details.CHQ_BOOK_ID).Any())
                {
                    return "Cannot Delete. Cheques Issued from the Cheque Book";
                }
                else
                {
                    ACC_CHQ_BOOK_DETAILS acc_chq_book_details = dbContext.ACC_CHQ_BOOK_DETAILS.Find(chequeBookId);

                    //Added by abhishek kamble 28-nov-2013
                    acc_chq_book_details.USERID = PMGSYSession.Current.UserId;
                    acc_chq_book_details.IPADD=HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    dbContext.Entry(acc_chq_book_details).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    dbContext.ACC_CHQ_BOOK_DETAILS.Remove(acc_chq_book_details);
                    dbContext.SaveChanges();
                    return String.Empty;
                }
            }
            catch (DbEntityValidationException entityException)
            {
                throw entityException;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw ex;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
               
            }

        }

        public String ValidateAddEditChequeBookDetails(ChequeBookViewModel chequeBookViewModel)
        {
            try
            {
                string validationMessage = String.Empty;
                dbContext = new PMGSYEntities();

                //validate Start and End leaf must be less than 100 leaf only Added by Abhishek 14Jan2014 start

                int MaxChequeBookLeafCount=Convert.ToInt32(ConfigurationManager.AppSettings["MaxChequeBookLeafCount"]);

                if ((Convert.ToInt64(chequeBookViewModel.LEAF_END) - Convert.ToInt64(chequeBookViewModel.LEAF_START)) > MaxChequeBookLeafCount)
                {
                    validationMessage = "Only "+ MaxChequeBookLeafCount+" Cheque book leaf are allowed. Please check Start leafs and End Leaf.";
                    return validationMessage;
                }

                //validate Start and End leaf must be less than 100 leaf only Added by Abhishek 14Jan2014 end


                List<ACC_CHQ_BOOK_DETAILS> lstChqBookModel = dbContext.ACC_CHQ_BOOK_DETAILS.Where(m => m.ADMIN_ND_CODE == chequeBookViewModel.ADMIN_ND_CODE && m.LVL_ID == chequeBookViewModel.LVL_ID && m.FUND_TYPE == chequeBookViewModel.FUND_TYPE && m.CHQ_BOOK_ID != chequeBookViewModel.CHQ_BOOK_ID).ToList<ACC_CHQ_BOOK_DETAILS>();
                foreach (ACC_CHQ_BOOK_DETAILS itemItr in lstChqBookModel)
                {
                    if (itemItr.CHQ_BOOK_ID != chequeBookViewModel.CHQ_BOOK_ID)
                    {
                        if (Convert.ToInt32(chequeBookViewModel.LEAF_START) <= Convert.ToInt32(itemItr.LEAF_END) && Convert.ToInt32(chequeBookViewModel.LEAF_START) >= Convert.ToInt32(itemItr.LEAF_START))
                        {
                            validationMessage = "Start Leaf already Exists in " + itemItr.LEAF_START + "-" + itemItr.LEAF_END + " Cheque Range";
                            break;
                        }
                        else
                        {
                            validationMessage = "";
                        }
                        if (validationMessage == "")
                        {
                            if (Convert.ToInt32(chequeBookViewModel.LEAF_END) <= Convert.ToInt32(itemItr.LEAF_END) && Convert.ToInt32(chequeBookViewModel.LEAF_END) >= Convert.ToInt32(itemItr.LEAF_START))
                            {
                                validationMessage = "End Leaf already Exists in " + itemItr.LEAF_START + "-" + itemItr.LEAF_END + " Cheque Range";
                                break;
                            }
                            else
                            {
                                validationMessage = "";
                            }
                        }

                        //Added By Abhishek kamble 27-Oct-2014 start

                        if (validationMessage == "")
                        {

                            if (Convert.ToInt32(itemItr.LEAF_START) >= Convert.ToInt32(chequeBookViewModel.LEAF_START) && Convert.ToInt32(itemItr.LEAF_END) <= Convert.ToInt32(chequeBookViewModel.LEAF_END))
                            {
                                validationMessage = "Start Leaf / End Leaf already Exists in " + itemItr.LEAF_START + "-" + itemItr.LEAF_END + " Cheque Range";
                                break;
                            }
                            else {
                                validationMessage = "";
                            }

                        }

                        //Added By Abhishek kamble 27-Oct-2014 end
                    }
                }
                return validationMessage;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw ex;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
               
            }
        }

        public bool ADDCBDAL(ChequeBookDetailsViewModel model, ref string message)
        {
            try
            {
                dbContext = new PMGSYEntities();
                CommonFunctions commomFuncObj = new CommonFunctions();
                ACC_BANK_DETAILS bank_detail = new ACC_BANK_DETAILS();
                ACC_CHQ_BOOK_DETAILS accCB = new ACC_CHQ_BOOK_DETAILS();

                if (PMGSYSession.Current.FundType != "P")
                    model.BANK_ACC_TYPE = "S";

                if (PMGSYSession.Current.LevelId == 4)
                {//  SSRDA Level
                    string returnValue1 = dbContext.USP_ACC_VALIDATE_BANK_DETAILS(PMGSYSession.Current.AdminNdCode, PMGSYSession.Current.FundType, model.BANK_ACC_TYPE).ToList().FirstOrDefault();
                    if (returnValue1 != "True")
                    {
                        message = returnValue1;
                        return false;
                    }
                    bank_detail.BANK_CODE = dbContext.ACC_BANK_DETAILS.Where(m => m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && m.BANK_ACC_STATUS == true && m.FUND_TYPE == PMGSYSession.Current.FundType && m.ACCOUNT_TYPE == "S").Select(m => m.BANK_CODE).FirstOrDefault();
                }
                else
                {// DPIU Level
                    string returnValue2 = dbContext.USP_ACC_VALIDATE_BANK_DETAILS(PMGSYSession.Current.ParentNDCode, PMGSYSession.Current.FundType, model.BANK_ACC_TYPE).ToList().FirstOrDefault();
                    if (returnValue2 != "True")
                    {
                        message = returnValue2;
                        return false;
                    }
                    bank_detail.BANK_CODE = dbContext.ACC_BANK_DETAILS.Where(m => m.ADMIN_ND_CODE == PMGSYSession.Current.ParentNDCode && m.BANK_ACC_STATUS == true && m.FUND_TYPE == PMGSYSession.Current.FundType && m.ACCOUNT_TYPE == "S").Select(m => m.BANK_CODE).FirstOrDefault();
                }

                if (PMGSYSession.Current.LevelId == 4)
                {   //chequeBookViewModel.LVL_ID = 5;
                    if (String.IsNullOrEmpty(model.IsSRRDADpiu))
                    {
                        model.LVL_ID = 5;
                    }
                    else
                    {
                        if (model.IsSRRDADpiu.Equals("S")) //SRRDA
                        {
                            model.LVL_ID = PMGSYSession.Current.LevelId;
                            model.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                        }
                        else if (model.IsSRRDADpiu.Equals("D"))//DPIU
                        {
                            model.LVL_ID = 5;
                        }
                    }
                }
                else
                {
                    model.LVL_ID = (byte)PMGSYSession.Current.LevelId;
                    model.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                }

                string returnValue = dbContext.USP_ACC_VALIDATE_CHEQUEBOOK_DETAILS(model.ADMIN_ND_CODE, PMGSYSession.Current.FundType, model.ISSUE_DATE, model.LEAF_START, model.LEAF_END, model.LVL_ID, model.BANK_ACC_TYPE).ToList().FirstOrDefault();
                if (returnValue != "True")
                {
                    message = returnValue;
                    return false;
                }
                //accCB.CHQ_BOOK_ID = dbContext.ACC_CHQ_BOOK_DETAILS.Any() ? dbContext.ACC_CHQ_BOOK_DETAILS.Max(m => m.CHQ_BOOK_ID) + 1 : 1;
                accCB.LEAF_START = model.LEAF_START.PadLeft(6, '0');
                accCB.LEAF_END = model.LEAF_END.PadLeft(6, '0');
                accCB.FUND_TYPE = PMGSYSession.Current.FundType;
                accCB.ISSUE_DATE = commomFuncObj.GetStringToDateTime(model.ISSUE_DATE);
                accCB.BANK_CODE = bank_detail.BANK_CODE;
                accCB.ADMIN_ND_CODE = model.ADMIN_ND_CODE;
                accCB.LVL_ID = model.LVL_ID;
                accCB.ACC_TYPE = model.BANK_ACC_TYPE;
                accCB.IS_CHQBOOK_COMPLETED = null;
                accCB.USERID = PMGSYSession.Current.UserId;
                //dbContext = new PMGSYEntities();
                //dbContext.ACC_CHQ_BOOK_DETAILS.Add(accCB);
                //dbContext.SaveChanges();
                string returnValue3 = dbContext.USP_ACC_INSERT_CHQ_BOOK_DETAILS(0, accCB.LEAF_START, accCB.LEAF_END, accCB.FUND_TYPE, accCB.ISSUE_DATE, accCB.BANK_CODE, accCB.ADMIN_ND_CODE, accCB.LVL_ID, accCB.USERID, accCB.IPADD, accCB.ACC_TYPE).ToList().FirstOrDefault();
                message = returnValue3;
                return true;
            }

            catch (OptimisticConcurrencyException ex)
            {
                ErrorLog.LogError(ex, "ADDCBDAL()");
                return false;
            }
            catch (UpdateException ex)
            {
                ErrorLog.LogError(ex, "ADDCBDAL()");
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "ADDCBDAL()");
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

        public Array CBDAL(int? page, int? rows, string sidx, string sord, string search, out long totalRecords, int AdminNdCode, int LevelId, string AccChqType)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<ACC_CHQ_BOOK_DETAILS> lstChequeBookDetails = null;
                IEnumerable<USP_ACC_GET_CHQ_BOOK_LIST_Result> itemList;
                Int16? month = Convert.ToInt16(search.Split('$')[0]);
                Int16? year = Convert.ToInt16(search.Split('$')[1]);
                Int32? chequeNo = Convert.ToInt32(search.Split('$')[2]);
                Int32 admin_nd_code = AdminNdCode;
                String fund_type = PMGSYSession.Current.FundType;
                int level_id = LevelId;

                itemList = dbContext.USP_ACC_GET_CHQ_BOOK_LIST(admin_nd_code, fund_type, level_id, (month == 0 ? null : month), (year == 0 ? null : year), (chequeNo == 0 ? null : chequeNo)).ToList<USP_ACC_GET_CHQ_BOOK_LIST_Result>();

                if (AccChqType != null && AccChqType != "A")
                {
                    itemList = itemList.Where(x => x.ACC_TYPE == AccChqType).ToList<USP_ACC_GET_CHQ_BOOK_LIST_Result>();
                }

                totalRecords = itemList.Count();
                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "ISSUE_DATE":
                                itemList = itemList.OrderBy(x => x.ISSUE_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList<USP_ACC_GET_CHQ_BOOK_LIST_Result>();
                                break;
                            case "LEAF_START":
                                itemList = itemList.OrderBy(x => x.LEAF_START).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList<USP_ACC_GET_CHQ_BOOK_LIST_Result>();
                                break;
                            case "LEAF_END":
                                itemList = itemList.OrderBy(x => x.LEAF_END).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList<USP_ACC_GET_CHQ_BOOK_LIST_Result>();
                                break;
                            default:
                                itemList = itemList.OrderBy(x => x.ISSUE_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList<USP_ACC_GET_CHQ_BOOK_LIST_Result>();
                                break;
                        }

                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "ISSUE_DATE":
                                itemList = itemList.OrderByDescending(x => x.ISSUE_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList<USP_ACC_GET_CHQ_BOOK_LIST_Result>();
                                break;
                            case "LEAF_START":
                                itemList = itemList.OrderByDescending(x => x.LEAF_START).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList<USP_ACC_GET_CHQ_BOOK_LIST_Result>();
                                break;
                            case "LEAF_END":
                                itemList = itemList.OrderByDescending(x => x.LEAF_END).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList<USP_ACC_GET_CHQ_BOOK_LIST_Result>();
                                break;
                            default:
                                itemList = itemList.OrderByDescending(x => x.ISSUE_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList<USP_ACC_GET_CHQ_BOOK_LIST_Result>();
                                break;
                        }
                    }
                }
                else
                {
                    itemList = itemList.OrderByDescending(x => x.ISSUE_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList<USP_ACC_GET_CHQ_BOOK_LIST_Result>();
                }
                return itemList.Select(item => new
                {
                    cell = new[]{   
                                    item.ISSUE_DATE.ToString(),
                                    item.ACC_TYPE =="S"?"Saving":item.ACC_TYPE =="H" ? "Holding":item.ACC_TYPE =="D"?"Security Deposit":"-",
                                    item.LEAF_START.Trim(),
                                    item.LEAF_END.Trim(),
                                    item.IS_CHQBOOK_COMPLETED == true ? "<span title='Chque book is issued'>-</span>" :  dbContext.ACC_BILL_MASTER.Where(m=>m.CHQ_Book_ID==item.CHQ_BOOK_ID && m.FUND_TYPE==PMGSYSession.Current.FundType &&m.ADMIN_ND_CODE==AdminNdCode && m.LVL_ID==LevelId ).Any()?"<span title='Chque book is issued'>-</span>": "<center><a href='#' class='ui-icon ui-icon-pencil' onclick='EditChequeBook(\"" +URLEncrypt.EncryptParameters(new string[] { item.CHQ_BOOK_ID.ToString().Trim()})+ "\");return false;'>Edit</a></center>",
                                    item.IS_CHQBOOK_COMPLETED == true ? "<span title='Chque book is issued'>-</span>": dbContext.ACC_BILL_MASTER.Where(m=>m.CHQ_Book_ID==item.CHQ_BOOK_ID && m.FUND_TYPE==PMGSYSession.Current.FundType &&m.ADMIN_ND_CODE==AdminNdCode && m.LVL_ID==LevelId).Any()?"<span title='Chque book is issued'>-</span>": "<center><a href='#' class='ui-icon ui-icon-trash' onclick='DeleteChequeBook(\"" +URLEncrypt.EncryptParameters(new string[] { item.CHQ_BOOK_ID.ToString().Trim()})+ "\");return false;'>Delete</a></center>" 
                            }
                }).ToArray();

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "CBDAL()");
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

        public String DeleteChequeBookDetails(int chequeBookId)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                dbContext = new PMGSYEntities();
                ACC_CHQ_BOOK_DETAILS cheque_details = dbContext.ACC_CHQ_BOOK_DETAILS.Where(m => m.CHQ_BOOK_ID == chequeBookId).FirstOrDefault();
                string returnValue = dbContext.USP_ACC_DELETE_CHEQUEBOOK_DETAILS(cheque_details.ADMIN_ND_CODE, PMGSYSession.Current.FundType, cheque_details.CHQ_BOOK_ID).ToList().FirstOrDefault();
                return returnValue;
            }
            catch (DbEntityValidationException entityException)
            {
                throw entityException;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DeleteChequeBookDetails()");
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
    }

    public interface IChequeBookDAL
    {
        Array ChequeBookList(int? page, int? rows, string sidx, string sord, string search, out long totalRecords, int AdminNdCode, int LevelId);
        String DeleteChequeBook(int chequeBookId);
        String ValidateAddEditChequeBookDetails(ChequeBookViewModel chequeBookViewModel);


        // New Cheque Book Details Screen[Rohit]
        bool ADDCBDAL(ChequeBookDetailsViewModel model, ref string message);
        Array CBDAL(int? page, int? rows, string sidx, string sord, string search, out long totalRecords, int AdminNdCode, int LevelId, string AccChqType);
        String DeleteChequeBookDetails(int chequeBookId);
        // New Cheque Book Details Screen
    }
}