using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using PMGSY.Extensions;
using PMGSY.Models;
using PMGSY.Models.AccountMaster;
using PMGSY.Common;
                  
namespace PMGSY.DAL.AccountMaster
{
    public class AccountMasterDAL:IAccountMasterDAL
    {    
        #region AccountMasterHead

        //populate Fund Type
        public List<SelectListItem> PopulateFundType()
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                List<SelectListItem> lstFundTypes = new List<SelectListItem>();
                if (PMGSYSession.Current.FundType == null)
                {
                    lstFundTypes = new SelectList(dbContext.ACC_MASTER_FUND_TYPE, "Fund_Type", "Fund_Type_Desc").ToList();
                }
                else {
                    lstFundTypes = new SelectList(dbContext.ACC_MASTER_FUND_TYPE.Where(m=>m.Fund_Type==PMGSYSession.Current.FundType), "Fund_Type", "Fund_Type_Desc").ToList();                
                }
                lstFundTypes.Insert(0, new SelectListItem { Text="-- Select --",Value=""});
                return lstFundTypes;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally {
                dbContext.Dispose();
            }
        }

        //Populate Head Category
        public List<SelectListItem> PopulateHeadCategory()
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                List<SelectListItem> lstHeadCategory = new List<SelectListItem>();
                lstHeadCategory = new SelectList(dbContext.ACC_MASTER_HEAD_CATEGORY.Where(m=>m.IS_VALID==true), "HEAD_CATEGORY_ID", "HEAD_CATEGORY_NAME").ToList();
                lstHeadCategory.Insert(0, new SelectListItem { Text = "-- Select --", Value = "" });
                return lstHeadCategory;                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally {
                dbContext.Dispose();
            }
        }


        //Populate Parent Head
        public List<SelectListItem> PopulateParentHead(String FundType)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                //List<SelectListItem> lstHeadCategory = new List<SelectListItem>();
                //lstHeadCategory = new SelectList(dbContext.ACC_MASTER_HEAD.Where(m => m.PARENT_HEAD_ID == null && m.FUND_TYPE==FundType), "HEAD_ID", "HEAD_NAME").ToList();

                //List<ACC_MASTER_HEAD> lstMasterHead=dbContext.ACC_MASTER_HEAD.Where(m => m.PARENT_HEAD_ID == null).ToList<ACC_MASTER_HEAD>();
                //foreach (var item in lstMasterHead)
                //{
                //    lstHeadCategory.Add(new SelectListItem { Text=item.HEAD_NAME.ToString(),Value=item.HEAD_ID.ToString()});
                //}
                //lstHeadCategory.Insert(0, new SelectListItem { Text = "-- Select --", Value = "" });

                //return lstHeadCategory;

                List<SelectListItem> lstParentHead = new List<SelectListItem>();
                //lstHeadCategory = new SelectList(dbContext.ACC_MASTER_HEAD.Where(m => m.PARENT_HEAD_ID == null && m.FUND_TYPE == FundType), "HEAD_ID", "HEAD_NAME").ToList();

                List<ACC_MASTER_HEAD> lstMasterHead = dbContext.ACC_MASTER_HEAD.Where(m => m.PARENT_HEAD_ID == null && m.FUND_TYPE == FundType).ToList<ACC_MASTER_HEAD>();
                foreach (var item in lstMasterHead)
                {
                    lstParentHead.Add(new SelectListItem { Text = item.HEAD_NAME.ToString(), Value = item.HEAD_ID.ToString() });
                }
                lstParentHead.Insert(0, new SelectListItem { Text = "-- Select --", Value = "" });

                return lstParentHead;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public List<SelectListItem> PopulateOperationalLevel()
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                List<SelectListItem> lstLevel = new List<SelectListItem>();
                lstLevel.Add(new SelectListItem { Text = "Select Level", Value = "0" });
                lstLevel.Add(new SelectListItem { Text="SRRDA",Value="1"});
                lstLevel.Add(new SelectListItem { Text = "DPIU", Value = "2" });
                lstLevel.Add(new SelectListItem { Text = "Both", Value = "3" });

                return lstLevel;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally {
                dbContext.Dispose();
            }
        }
                            

        //List Master Head Details
        public Array ListMasterHeadDetails(int? page,int?rows,string sidx,String sord,out long totalRecords,string FundType)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                var MasterHeadList = dbContext.ACC_MASTER_HEAD.Where(m=>m.FUND_TYPE==FundType).ToList();
                                                      
                totalRecords = MasterHeadList.Count();

                if (sidx.Trim() != String.Empty)
                {
                    if (sord.ToString() == "asc")
                    {

                        switch (sidx)
                        {
                            case "HEAD_CODE":
                                MasterHeadList = MasterHeadList.OrderBy(o => o.HEAD_CODE).ToList();
                                break;
                            case "HEAD_NAME":
                                MasterHeadList = MasterHeadList.OrderBy(o => o.HEAD_NAME).ToList();
                                break;
                            case "FUND_TYPE":
                                MasterHeadList = MasterHeadList.OrderBy(o => o.FUND_TYPE).ToList();
                                break;
                            case "CREDIT_DEBIT":
                                MasterHeadList = MasterHeadList.OrderBy(o => o.CREDIT_DEBIT).ToList();
                                break;
                            case "OP_LVL_ID":
                                MasterHeadList = MasterHeadList.OrderBy(o => o.OP_LVL_ID).ToList();
                                break;
                            case "IS_OPERATIONAL":
                                MasterHeadList = MasterHeadList.OrderBy(o => o.IS_OPERATIONAL).ToList();
                                break;
                            case "HEAD_CATEGORY_ID":
                                MasterHeadList = MasterHeadList.OrderBy(o => o.HEAD_CATEGORY_ID).ToList();
                                break;
                            default:
                                MasterHeadList = MasterHeadList.OrderBy(o => o.HEAD_CODE).ToList();
                                break;
                        }

                    }
                    else {   
                        switch (sidx)
                        {
                            case "HEAD_CODE":
                                MasterHeadList = MasterHeadList.OrderByDescending(o => o.HEAD_CODE).ToList();
                                break;
                            case "HEAD_NAME":
                                MasterHeadList = MasterHeadList.OrderByDescending(o => o.HEAD_NAME).ToList();
                                break;
                            case "FUND_TYPE":
                                MasterHeadList = MasterHeadList.OrderByDescending(o => o.FUND_TYPE).ToList();
                                break;
                            case "CREDIT_DEBIT":
                                MasterHeadList = MasterHeadList.OrderByDescending(o => o.CREDIT_DEBIT).ToList();
                                break;
                            case "OP_LVL_ID":
                                MasterHeadList = MasterHeadList.OrderByDescending(o => o.OP_LVL_ID).ToList();
                                break;
                            case "IS_OPERATIONAL":
                                MasterHeadList = MasterHeadList.OrderByDescending(o => o.IS_OPERATIONAL).ToList();
                                break;
                            case "HEAD_CATEGORY_ID":
                                MasterHeadList = MasterHeadList.OrderByDescending(o => o.HEAD_CATEGORY_ID).ToList();
                                break;
                            default:
                                MasterHeadList = MasterHeadList.OrderByDescending(o => o.HEAD_CODE).ToList();
                                break;
                        }                         
                    }
                }
                else {
                    MasterHeadList = MasterHeadList.OrderBy(o => o.HEAD_ID).ToList();                                 
                }
                return MasterHeadList.Select(item => new
                {
                    id = item.HEAD_CODE,
                    cell = new[]{
                            item.PARENT_HEAD_ID==null?"<b>"+item.HEAD_CODE+"</b>":item.HEAD_CODE,
                            item.PARENT_HEAD_ID==null?"<b>"+item.HEAD_NAME+"</b>":item.HEAD_NAME,
                            item.PARENT_HEAD_ID==null?"<b>"+ (item.FUND_TYPE=="P"?"Program Fund" : (item.FUND_TYPE=="A"?"Administrative Expenses Fund":"Maintenance Fund"))+"<b>":((item.FUND_TYPE=="P"?"Program Fund" : (item.FUND_TYPE=="A"?"Administrative Expenses Fund":"Maintenance Fund"))),
                            (item.CREDIT_DEBIT=="C"?"Credit": (item.CREDIT_DEBIT=="D"?"Debit":"")),
                            (item.OP_LVL_ID==1?"SRRDA":(item.OP_LVL_ID==2?"DPIU": item.OP_LVL_ID==3? "Both":"")),
                            item.PARENT_HEAD_ID==null?"<b>"+  (item.IS_OPERATIONAL==true?"Yes":"No")+"</b>":(item.IS_OPERATIONAL==true?"Yes":"No"),  
                            (item.HEAD_CATEGORY_ID==null?"": dbContext.ACC_MASTER_HEAD_CATEGORY.Where(m=>m.HEAD_CATEGORY_ID==item.HEAD_CATEGORY_ID).Select(s=>s.HEAD_CATEGORY_NAME).FirstOrDefault()) ,                            
                            URLEncrypt.EncryptParameters1(new string[]{"HEAD_ID="+item.HEAD_ID.ToString().Trim()}),
                            
                            //URLEncrypt.EncryptParameters1(new string[]{"HEAD_ID="+item.HEAD_ID.ToString().Trim()}),

                            item.PARENT_HEAD_ID==null?(dbContext.ACC_MASTER_HEAD.Where(m=>m.PARENT_HEAD_ID==item.HEAD_ID).Any()?String.Empty: URLEncrypt.EncryptParameters1(new string[]{"HEAD_ID="+item.HEAD_ID.ToString().Trim()})):URLEncrypt.EncryptParameters1(new string[]{"HEAD_ID="+item.HEAD_ID.ToString().Trim()}),

                            }
                }).ToArray();


            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                throw ex;
            }
            finally {
                dbContext.Dispose();            
            }        
        }

        //save details
        public bool AddMasterHeadDetails(MasterHeadViewModel headViewModel,ref String message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {

                //Parent Head Validation
                if (headViewModel.ParentSubHead.Equals("P"))
                {
                    if (headViewModel.HEAD_CODE.Contains('.'))
                    {
                        message = "Invalid Head Code,Decimal Numbers are not allowed.";
                        return false;
                    }                    
                }
                else {
                    if (!(headViewModel.HEAD_CODE.Contains('.')))
                    {
                        message = "Invalid Head Code.Only Decimal Numbers are allowed";
                        return false;
                    }
                }

                if (headViewModel.ParentSubHead.Equals("S"))
                {
                    if (!(headViewModel.HEAD_CODE.StartsWith(GetParentHeadCode(headViewModel.PARENT_HEAD_ID.Value)) ))
                    {
                        message = "Please enter correct head code.";
                        return false;
                    }
                }

                var result=dbContext.USP_ACC_ADD_EDIT_MASTER_HEAD_DETAILS(0, headViewModel.HEAD_CODE, headViewModel.HEAD_CODE, headViewModel.HEAD_NAME.Trim(), headViewModel.PARENT_HEAD_ID, headViewModel.FUND_TYPE, headViewModel.CREDIT_DEBIT, headViewModel.OP_LVL_ID, headViewModel.IS_OPERATIONAL, headViewModel.HEAD_CATEGORY_ID, "A");

                if (result.FirstOrDefault() == 1)
                {
                    message = "Head details is added successfully.";
                    return true;
                }
                else {
                    message = "An Error occured while adding Head Master details.";
                    return false;  
                }                
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error occurred while processing the request.";
                return false;
            }
            finally {               
                dbContext.Dispose();
            }
        }
                  
        //update details
        public bool EditMasterHeadDetails(MasterHeadViewModel headViewModel, ref String message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try 
            {

               string[] encryptedParameters = null;
               Dictionary<string, string> decryptedParameters = null;
               short HeadId = 0;

               encryptedParameters = headViewModel.EncryptedHeadID.Split('/');

               if (!(encryptedParameters.Length == 3))
               {
                   return false;
               }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                HeadId = Convert.ToInt16(decryptedParameters["HEAD_ID"].ToString());

                var result = dbContext.USP_ACC_ADD_EDIT_MASTER_HEAD_DETAILS(HeadId, headViewModel.HEAD_CODE, headViewModel.HEAD_CODE, headViewModel.HEAD_NAME.Trim(), headViewModel.PARENT_HEAD_ID, headViewModel.FUND_TYPE, headViewModel.CREDIT_DEBIT, headViewModel.OP_LVL_ID, headViewModel.IS_OPERATIONAL, headViewModel.HEAD_CATEGORY_ID, "U");

                if (result.FirstOrDefault() == 1)
                {
                    message = "Head details is Updated successfully.";
                    return true;
                }
                else
                {
                    message = "An Error occured while Updating Head Master details.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error occurred while processing the request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        //Delete details
        public bool DeleteMasterHeadDetails(short HeadId, ref String message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {

                if (dbContext.ACC_MASTER_HEAD.Where(m => m.PARENT_HEAD_ID == HeadId).Any())
                {
                    message = "Parent Head details can't be deleted as Sub Head details are Present.";
                    return false;                
                }

                ACC_MASTER_HEAD masterHead = dbContext.ACC_MASTER_HEAD.Where(m => m.HEAD_ID == HeadId).FirstOrDefault();
                
                if (masterHead != null)
                {
                    dbContext.ACC_MASTER_HEAD.Remove(masterHead);
                    dbContext.SaveChanges();
                    message = "Head details deleted successfully.";
                    return true;
                }
                else {
                    //message = "An Error occured while deleting Head Master details.";
                    message = "Head details are in use, can not delete head details.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error occurred while processing the request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
                         
        //GetHead Details
        public MasterHeadViewModel GetHeadDetails(short HeadId)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                ACC_MASTER_HEAD masterHeadModel = dbContext.ACC_MASTER_HEAD.Where(m => m.HEAD_ID == HeadId).FirstOrDefault();
                MasterHeadViewModel HeadViewModel = new MasterHeadViewModel();

                if (masterHeadModel != null)
                {        
                    HeadViewModel.EncryptedHeadID =  URLEncrypt.EncryptParameters1(new string[]{"HEAD_ID="+masterHeadModel.HEAD_ID.ToString().Trim()});
                    HeadViewModel.HEAD_CODE = masterHeadModel.HEAD_CODE;
                    HeadViewModel.HEAD_NAME = masterHeadModel.HEAD_NAME;
                    HeadViewModel.FUND_TYPE = masterHeadModel.FUND_TYPE;
                    HeadViewModel.CREDIT_DEBIT = masterHeadModel.CREDIT_DEBIT;
                    HeadViewModel.OP_LVL_ID = masterHeadModel.OP_LVL_ID;
                    HeadViewModel.IS_OPERATIONAL = masterHeadModel.IS_OPERATIONAL;
                    HeadViewModel.HEAD_CATEGORY_ID = masterHeadModel.HEAD_CATEGORY_ID;
                    HeadViewModel.PARENT_HEAD_ID = masterHeadModel.PARENT_HEAD_ID;
                    HeadViewModel.IsParentHead = masterHeadModel.PARENT_HEAD_ID == null ? true : false;
                    HeadViewModel.ParentSubHead = masterHeadModel.PARENT_HEAD_ID == null ? "P" : "S";
                }

                return HeadViewModel;

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);                
                return null;
            }
            finally {
                dbContext.Dispose();
            }
        
        }

        public String GetParentHeadCode(short ParentHeadID)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                return dbContext.ACC_MASTER_HEAD.Where(m => m.HEAD_ID == ParentHeadID).Select(s => s.HEAD_CODE).FirstOrDefault();
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

        #endregion AccountMasteHead


        #region AccountMasterTransaction

        //List Master Transaction Details
        public Array ListMasterTransactionDetails(int? page, int? rows, string sidx, String sord, out long totalRecords, string FundType, bool IsSearch, short ParentTxn, int Level, string CashCheque, string BillType,bool? IsOperational)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {

                List<ACC_MASTER_TXN> MasterTransactionList = new List<ACC_MASTER_TXN>();


                if (IsSearch)
                {
                    MasterTransactionList = dbContext.ACC_MASTER_TXN.Where(m => m.FUND_TYPE == FundType).ToList();

                    if (ParentTxn != 0)
                    {
                        List<ACC_MASTER_TXN> parentTransactionList = new List<ACC_MASTER_TXN>();
                        List<ACC_MASTER_TXN> subTransactionList = new List<ACC_MASTER_TXN>();
                        parentTransactionList = MasterTransactionList.Where(m=>m.TXN_ID == ParentTxn).ToList();
                        subTransactionList = MasterTransactionList.Where(m =>m.TXN_PARENT_ID == ParentTxn).ToList();
                        MasterTransactionList = parentTransactionList.Union(subTransactionList).ToList();
                    }

                    if (Level != 0)
                    {
                        MasterTransactionList = MasterTransactionList.Where(m =>  m.OP_LVL_ID == Level).ToList();                    
                    }

                    if (!(String.IsNullOrEmpty(CashCheque)))
                    {
                        MasterTransactionList = MasterTransactionList.Where(m => m.CASH_CHQ != null && m.CASH_CHQ.Trim() == CashCheque).ToList();
                    }

                    if (!(String.IsNullOrEmpty(BillType)))
                    {
                        MasterTransactionList = MasterTransactionList.Where(m => m.BILL_TYPE == BillType).ToList();
                    }
                    if (IsOperational!=null)
                    {
                        MasterTransactionList = MasterTransactionList.Where(m => m.IS_OPERATIONAL == IsOperational).ToList();
                    }
                }
                else
                {
                    MasterTransactionList = dbContext.ACC_MASTER_TXN.Where(m => m.FUND_TYPE == FundType).ToList();
                }
                totalRecords = MasterTransactionList.Count();

                //if (sidx.Trim() != String.Empty)
                //{
                //    if (sord.ToString() == "asc")
                //    {

                //        switch (sidx)
                //        {
                //            case "CASH_CHQ":
                //                MasterTransactionList = MasterTransactionList.OrderBy(o => o.CASH_CHQ).ToList();
                //                break;
                //            case "BILL_TYPE":
                //                MasterTransactionList = MasterTransactionList.OrderBy(o => o.BILL_TYPE).ToList();
                //                break;
                //            case "TXN_DESC":
                //                MasterTransactionList = MasterTransactionList.OrderBy(o => o.TXN_DESC).ToList();
                //                break;
                //            case "TXN_NARRATION":
                //                MasterTransactionList = MasterTransactionList.OrderBy(o => o.TXN_NARRATION).ToList();
                //                break;
                //            case "IS_OPERATIONAL":
                //                MasterTransactionList = MasterTransactionList.OrderBy(o => o.IS_OPERATIONAL).ToList();
                //                break;
                //            case "OP_LVL_ID":
                //                MasterTransactionList = MasterTransactionList.OrderBy(o => o.OP_LVL_ID).ToList();
                //                break;
                //            case "IS_REQ_AFTER_PORTING":
                //                MasterTransactionList = MasterTransactionList.OrderBy(o => o.IS_REQ_AFTER_PORTING).ToList();
                //                break;
                //            default:
                //                MasterTransactionList = MasterTransactionList.OrderBy(o => o.TXN_ID).ToList();
                //                break;
                //        }

                //    }
                //    else
                //    {
                //        switch (sidx)
                //        {
                //            case "CASH_CHQ":
                //                MasterTransactionList = MasterTransactionList.OrderByDescending(o => o.CASH_CHQ).ToList();
                //                break;
                //            case "BILL_TYPE":
                //                MasterTransactionList = MasterTransactionList.OrderByDescending(o => o.BILL_TYPE).ToList();
                //                break;
                //            case "TXN_DESC":
                //                MasterTransactionList = MasterTransactionList.OrderByDescending(o => o.TXN_DESC).ToList();
                //                break;
                //            case "TXN_NARRATION":
                //                MasterTransactionList = MasterTransactionList.OrderByDescending(o => o.TXN_NARRATION).ToList();
                //                break;
                //            case "IS_OPERATIONAL":
                //                MasterTransactionList = MasterTransactionList.OrderByDescending(o => o.IS_OPERATIONAL).ToList();
                //                break;
                //            case "OP_LVL_ID":
                //                MasterTransactionList = MasterTransactionList.OrderByDescending(o => o.OP_LVL_ID).ToList();
                //                break;
                //            case "IS_REQ_AFTER_PORTING":
                //                MasterTransactionList = MasterTransactionList.OrderByDescending(o => o.IS_REQ_AFTER_PORTING).ToList();
                //                break;
                //            default:
                //                MasterTransactionList = MasterTransactionList.OrderByDescending(o => o.TXN_ID).ToList();
                //                break;
                //        }
                //    }
                //}
                //else
                //{
                //    MasterTransactionList = MasterTransactionList.OrderBy(o => o.TXN_ID).ToList();                    
                //}
                return MasterTransactionList.Select(item => new
                {
                    id = (item.TXN_PARENT_ID==null? item.TXN_ID.ToString():item.TXN_PARENT_ID.ToString()),
                    cell = new[]{
                            item.TXN_PARENT_ID==null? item.TXN_ID.ToString():item.TXN_PARENT_ID.ToString(),
                            item.TXN_PARENT_ID==null?"<b>"+item.TXN_DESC+"</b>":item.TXN_DESC,
                            item.TXN_NARRATION,
                            
                            item.TXN_PARENT_ID==null?"<b>"+(item.CASH_CHQ==null?"":(item.CASH_CHQ.Trim()=="C"?"Cash":(item.CASH_CHQ.Trim()=="Q"?"Cheque": (item.CASH_CHQ.Trim()=="CQ"?"Cash Cheque":(item.CASH_CHQ.Trim()=="D"?"Deduction":"")))))+"</b>": (item.CASH_CHQ==null?"":(item.CASH_CHQ.Trim()=="C"?"Cash":(item.CASH_CHQ.Trim()=="Q"?"Cheque": (item.CASH_CHQ.Trim()=="CQ"?"Cash Cheque":(item.CASH_CHQ.Trim()=="D"?"Deduction":""))))),                                                        
//                          item.TXN_PARENT_ID==null?"<b>"+item.ACC_MASTER_BILL_TYPE.BILL_DESC+"</b>":item.ACC_MASTER_BILL_TYPE.BILL_DESC,                            
                            item.TXN_PARENT_ID==null?"<b>"+dbContext.ACC_MASTER_BILL_TYPE.Where(s=>s.BILL_TYPE==item.BILL_TYPE.Trim()).Select(s=>s.BILL_DESC).FirstOrDefault()+"</b>":dbContext.ACC_MASTER_BILL_TYPE.Where(s=>s.BILL_TYPE==item.BILL_TYPE.Trim()).Select(s=>s.BILL_DESC).FirstOrDefault(),  
                            item.TXN_PARENT_ID==null?"<b>"+ (item.FUND_TYPE=="P"?"Program Fund" : (item.FUND_TYPE=="A"?"Administrative Expenses Fund":"Maintenance Fund"))+"<b>":((item.FUND_TYPE=="P"?"Program Fund" : (item.FUND_TYPE=="A"?"Administrative Expenses Fund":"Maintenance Fund"))),
                            item.TXN_PARENT_ID==null?"<b>"+  (item.IS_OPERATIONAL==true?"Yes":"No")+"</b>":(item.IS_OPERATIONAL==true?"Yes":"No"),                              
                            item.TXN_PARENT_ID==null?"<b>"+(item.OP_LVL_ID==4?"SRRDA":(item.OP_LVL_ID==5?"DPIU":"-" )) +"</b>": (item.OP_LVL_ID==4?"SRRDA":(item.OP_LVL_ID==5?"DPIU":"-" )),
                            item.TXN_PARENT_ID==null?"<b>"+  (item.IS_REQ_AFTER_PORTING==true?"Yes":"No")+"</b>":(item.IS_REQ_AFTER_PORTING==true?"Yes":"No"),
                            

                            URLEncrypt.EncryptParameters1(new string[]{"TXN_ID="+item.TXN_ID.ToString().Trim()}),
                            //URLEncrypt.EncryptParameters1(new string[]{"TXN_ID="+item.TXN_ID.ToString().Trim()}),                            

                            item.TXN_PARENT_ID==null? (dbContext.ACC_MASTER_TXN.Where(m=>m.TXN_PARENT_ID==item.TXN_ID).Any()?String.Empty: URLEncrypt.EncryptParameters1(new string[]{"TXN_ID="+item.TXN_ID.ToString().Trim()})) :URLEncrypt.EncryptParameters1(new string[]{"TXN_ID="+item.TXN_ID.ToString().Trim()}),

                            item.TXN_PARENT_ID==null?"":(dbContext.ACC_MASTER_TXN.Where(m=>m.TXN_ID==item.TXN_PARENT_ID && m.TXN_PARENT_ID==null).Any()?"<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon ui-icon-check' title='Correct Transaction' ></span></td></tr></table></center>" : "<center><table><tr><td  style='border:none;cursor:pointer'><span class='ui-icon 	ui-icon-closethick' title='Invalid Transaction' ></span></td></tr></table></center>")
  
                           
                            }
                }).OrderBy(o=>o.id).ToArray();
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

        //save details
        public bool AddMasterTransactionDetails(MasterTransactionViewModel txnViewModel, ref String message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {

                //var result = dbContext.USP_ACC_ADD(0, headViewModel.HEAD_CODE, headViewModel.HEAD_CODE, headViewModel.HEAD_NAME, headViewModel.PARENT_HEAD_ID, headViewModel.FUND_TYPE, headViewModel.CREDIT_DEBIT, headViewModel.OP_LVL_ID, headViewModel.IS_OPERATIONAL, headViewModel.HEAD_CATEGORY_ID, "A");
                var result = dbContext.USP_ACC_ADD_EDIT_MASTER_TRANSACTION_DETAILS(0, txnViewModel.TXN_PARENT_ID, txnViewModel.CASH_CHQ, txnViewModel.BILL_TYPE, txnViewModel.TXN_DESC.Trim(), (txnViewModel.TXN_NARRATION == null ? txnViewModel.TXN_NARRATION : txnViewModel.TXN_NARRATION.Trim()), txnViewModel.FUND_TYPE, txnViewModel.IS_OPERATIONAL, txnViewModel.OP_LVL_ID, txnViewModel.IS_REQ_AFTER_PORTING, "A");

                if (result.FirstOrDefault() == 1)
                {
                    message = "Transaction details is added successfully.";
                    return true;
                }
                else
                {
                    message = "An Error occured while adding Transaction details.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error occurred while processing the request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        //update details
        public bool EditMasterTransactionDetails(MasterTransactionViewModel txnViewModel, ref String message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {

                string[] encryptedParameters = null;
                Dictionary<string, string> decryptedParameters = null;
                short TxnId = 0;

                encryptedParameters = txnViewModel.EncryptedTxnID.Split('/');

                if (!(encryptedParameters.Length == 3))
                {
                    return false;
                }

                decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                TxnId = Convert.ToInt16(decryptedParameters["TXN_ID"].ToString());

                var result = dbContext.USP_ACC_ADD_EDIT_MASTER_TRANSACTION_DETAILS(TxnId, txnViewModel.TXN_PARENT_ID, txnViewModel.CASH_CHQ, txnViewModel.BILL_TYPE, txnViewModel.TXN_DESC.Trim(), (txnViewModel.TXN_NARRATION == null ? txnViewModel.TXN_NARRATION : txnViewModel.TXN_NARRATION.Trim()), txnViewModel.FUND_TYPE, txnViewModel.IS_OPERATIONAL, txnViewModel.OP_LVL_ID, txnViewModel.IS_REQ_AFTER_PORTING, "U");


                if (result.FirstOrDefault() == 1)
                {
                    message = "Transaction details is Updated successfully.";
                    return true;
                }
                else
                {
                    message = "An Error occured while Updating Transaction details.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error occurred while processing the request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        //Delete details
        public bool DeleteMasterTransactionDetails(short TxnId, ref String message)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {

                //if (dbContext.ACC_MASTER_HEAD.Where(m => m.PARENT_HEAD_ID == HeadId).Any())
                //{
                //    message = "Parent Head details can't be deleted as Sub Head details are Present.";
                //    return false;
                //}

                ACC_MASTER_TXN masterTxn = dbContext.ACC_MASTER_TXN.Where(m => m.TXN_ID == TxnId).FirstOrDefault();

                if (masterTxn != null)
                {
                    dbContext.ACC_MASTER_TXN.Remove(masterTxn);
                    dbContext.SaveChanges();
                    message = "Transaction details deleted successfully.";
                    return true;
                }
                else
                {
                    //message = "An Error occured while deleting Transaction details.";
                    message = "Transaction are in use, can not delete Transaction details."; 
                    return false;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                message = "Error occurred while processing the request.";
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        //GetTransaction Details
        public MasterTransactionViewModel GetTransactionDetails(short TxnId)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                ACC_MASTER_TXN masterTxnModel = dbContext.ACC_MASTER_TXN.Where(m => m.TXN_ID == TxnId).FirstOrDefault();
                MasterTransactionViewModel TxnViewModel = new MasterTransactionViewModel();

                if (masterTxnModel != null)
                {
                    TxnViewModel.EncryptedTxnID = URLEncrypt.EncryptParameters1(new string[] { "TXN_ID=" + masterTxnModel.TXN_ID.ToString().Trim() });
                    TxnViewModel.TXN_PARENT_ID = masterTxnModel.TXN_PARENT_ID;
                    TxnViewModel.CASH_CHQ = masterTxnModel.CASH_CHQ==null?"": masterTxnModel.CASH_CHQ.Trim();
                    TxnViewModel.BILL_TYPE = masterTxnModel.BILL_TYPE;
                    TxnViewModel.TXN_DESC = masterTxnModel.TXN_DESC;
                    TxnViewModel.TXN_NARRATION = masterTxnModel.TXN_NARRATION;
                    TxnViewModel.FUND_TYPE = masterTxnModel.FUND_TYPE;
                    TxnViewModel.IS_OPERATIONAL = masterTxnModel.IS_OPERATIONAL;
                    TxnViewModel.OP_LVL_ID = masterTxnModel.OP_LVL_ID;
                    TxnViewModel.IS_REQ_AFTER_PORTING = masterTxnModel.IS_REQ_AFTER_PORTING;
                    TxnViewModel.IsParentTransaction = masterTxnModel.TXN_PARENT_ID == null ? true : false;
                    TxnViewModel.ParentSubTransaction = masterTxnModel.TXN_PARENT_ID== null ? "P" : "S";
                }

                return TxnViewModel;

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
           

        //Populate Parent Transaction
        public List<SelectListItem> PopulateParentTransaction(String FundType)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                List<SelectListItem> lstParentTransaction = new List<SelectListItem>();
                List<ACC_MASTER_TXN> lstMasterTxn = dbContext.ACC_MASTER_TXN.Where(m => m.TXN_PARENT_ID == null && m.FUND_TYPE == FundType).OrderBy(o=>o.TXN_DESC).ToList<ACC_MASTER_TXN>();
                foreach (var item in lstMasterTxn)
                {
                    lstParentTransaction.Add(new SelectListItem { Text = item.TXN_DESC.ToString(), Value = item.TXN_ID.ToString() });
                }                
                lstParentTransaction.Insert(0, new SelectListItem { Text = "-- Select --", Value = "" });

                return lstParentTransaction;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        //Populate Cash Cheque
        public List<SelectListItem> PopulateCashCheque()
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                List<SelectListItem> lstCashCheque = new List<SelectListItem>();

                lstCashCheque.Add(new SelectListItem { Text = "Cash", Value = "C" });
                lstCashCheque.Add(new SelectListItem { Text = "Cheque", Value = "Q" });
                lstCashCheque.Add(new SelectListItem { Text = "Cash Cheque", Value = "CQ" });
                lstCashCheque.Add(new SelectListItem { Text = "Deduction", Value = "D" });                
                lstCashCheque.Insert(0, new SelectListItem { Text = "-- Select --", Value = "" });

                return lstCashCheque;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        //Populate BILL Type
        public List<SelectListItem> PopulateBillType()
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                List<SelectListItem> lstBillType = new List<SelectListItem>();
                List<ACC_MASTER_BILL_TYPE> masterBillType= dbContext.ACC_MASTER_BILL_TYPE.OrderBy(o=>o.BILL_DESC).ToList<ACC_MASTER_BILL_TYPE>();
                foreach (var item in masterBillType)
                {
                    lstBillType.Add(new SelectListItem { Text = item.BILL_DESC.ToString(), Value = item.BILL_TYPE.ToString() });
                }
                lstBillType.Insert(0, new SelectListItem { Text = "-- Select --", Value = "" });

                return lstBillType;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        //Populate Cash Cheque
        public List<SelectListItem> PopulateLevel()
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                List<SelectListItem> lstLevel= new List<SelectListItem>();

                lstLevel.Add(new SelectListItem { Text = "DPIU", Value = "5" });
                lstLevel.Add(new SelectListItem { Text = "SRRDA", Value = "4" });      
                lstLevel = lstLevel.OrderBy(o => o.Value).ToList();

                lstLevel.Insert(0, new SelectListItem { Text = "-- Select --", Value = "" });

                return lstLevel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public ACC_MASTER_TXN GetParentTransactionDetails(short ParentTxnID)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                return dbContext.ACC_MASTER_TXN.Where(m => m.TXN_ID == ParentTxnID).FirstOrDefault();
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


        #endregion 

    }


    public interface IAccountMasterDAL
    {
        #region AccountMasterHead

        List<SelectListItem> PopulateFundType();
        List<SelectListItem> PopulateHeadCategory();
        List<SelectListItem> PopulateParentHead(String FundType);
        List<SelectListItem> PopulateOperationalLevel();

        Array ListMasterHeadDetails(int? page, int? rows, string sidx, String sord, out long totalRecords, string FundType);
        bool AddMasterHeadDetails(MasterHeadViewModel headViewModel, ref String message);
        bool EditMasterHeadDetails(MasterHeadViewModel headViewModel, ref String message);
        bool DeleteMasterHeadDetails(short HeadId, ref String message);
        MasterHeadViewModel GetHeadDetails(short HeadId);
        String GetParentHeadCode(short ParentHeadID);

        #endregion

        #region AccountMasterTransaction

        List<SelectListItem> PopulateParentTransaction(String FundType);
        List<SelectListItem> PopulateCashCheque();
        List<SelectListItem> PopulateBillType();
        List<SelectListItem> PopulateLevel();

        Array ListMasterTransactionDetails(int? page, int? rows, string sidx, String sord, out long totalRecords, string FundType, bool IsSearch, short ParentTxn, int Level, string CashCheque, string BillType, bool? IsOperational);
        bool AddMasterTransactionDetails(MasterTransactionViewModel txnViewModel, ref String message);
        bool EditMasterTransactionDetails(MasterTransactionViewModel txnViewModel, ref String message);
        bool DeleteMasterTransactionDetails(short TxnId, ref String message);
        MasterTransactionViewModel GetTransactionDetails(short TxnId);

        ACC_MASTER_TXN GetParentTransactionDetails(short ParentTxnID);
        #endregion


    }

}
