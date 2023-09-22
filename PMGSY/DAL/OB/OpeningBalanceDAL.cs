using PMGSY.Models.OB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.Models;
using PMGSY.Common;
using System.Transactions;
using PMGSY.Models.Receipts;
using System.Data.Entity;
using PMGSY.Models.Common;
using PMGSY.Extensions;
using System.Data.Entity.Core;

namespace PMGSY.DAL.OB
{
    public class OpeningBalanceDAL : IOpeningBalanceDAL
    {
        PMGSYEntities dbContext = null;
        CommonFunctions commonFuncObj = null;

        public String GetOBMasterIds(Int64 billId)
        {
            try
            {
                dbContext = new PMGSYEntities();
                ACC_BILL_MASTER acc_bill_master = new ACC_BILL_MASTER();
                acc_bill_master = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == billId).FirstOrDefault();
                ACC_BILL_MASTER next_acc_bill_master = new ACC_BILL_MASTER();
                next_acc_bill_master = dbContext.ACC_BILL_MASTER.Where(m => m.ADMIN_ND_CODE == acc_bill_master.ADMIN_ND_CODE && m.LVL_ID == acc_bill_master.LVL_ID && m.BILL_TYPE == acc_bill_master.BILL_TYPE && m.BILL_NO != acc_bill_master.BILL_NO && m.FUND_TYPE == PMGSYSession.Current.FundType).FirstOrDefault();
                return (acc_bill_master.BILL_NO == "1" ? acc_bill_master.BILL_ID + "$" + next_acc_bill_master.BILL_ID : next_acc_bill_master.BILL_ID + "$" + acc_bill_master.BILL_ID);
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw Ex;
            }
            finally
            {
                dbContext.Dispose();
            }

        }
        
        public OBMasterModel GetOBMasterById(String billId)
        {
            try
            {
                commonFuncObj = new CommonFunctions();
                Int64 billIdAsset = Convert.ToInt64(billId.Split('$')[0]);
                Int64 billIdLib = Convert.ToInt64(billId.Split('$')[1]);
                dbContext = new PMGSYEntities();
                ACC_BILL_MASTER acc_bill_master = new ACC_BILL_MASTER();
                acc_bill_master = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == billIdAsset).FirstOrDefault();
                OBMasterModel obMasterModel = new OBMasterModel();
                obMasterModel = CloneMasterObject(acc_bill_master);
                acc_bill_master = null;
                acc_bill_master = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == billIdLib).FirstOrDefault();
                obMasterModel.LIB_BILL_ID = acc_bill_master.BILL_ID;
                obMasterModel.LIB_BILL_NO = acc_bill_master.BILL_NO;
                obMasterModel.LIB_BILL_DATE = commonFuncObj.GetDateTimeToString(acc_bill_master.BILL_DATE);
                obMasterModel.LIB_GROSS_AMOUNT = acc_bill_master.GROSS_AMOUNT;
                return obMasterModel;
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw Ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public ACC_BILL_MASTER GetOBMaster(Int64 billId)
        {
            try
            {
                dbContext = new PMGSYEntities();
                ACC_BILL_MASTER acc_bill_master = new ACC_BILL_MASTER();
                acc_bill_master = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == billId).FirstOrDefault();
                return acc_bill_master;
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw Ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public OBDetailsModel GetOBDetailsByTransId(Int64 billId, Int16 transId)
        {
            try
            {
                dbContext = new PMGSYEntities();
                ACC_BILL_DETAILS acc_bill_details = new ACC_BILL_DETAILS();
                OBDetailsModel obDetailsModel = new OBDetailsModel();
                acc_bill_details = dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == billId && m.TXN_NO == transId).FirstOrDefault();
                obDetailsModel = CloneDetailsObject(acc_bill_details);
                return obDetailsModel;
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw Ex;
            }
            finally
            {
                dbContext.Dispose();
            }
 
        }

        public Int64 AddOBMaster(OBMasterModel obMasterModel)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    dbContext = new PMGSYEntities();
                    ACC_BILL_MASTER acc_bill_master = new ACC_BILL_MASTER();
                    obMasterModel.TXN_ID = dbContext.ACC_MASTER_TXN.Where(m=>m.BILL_TYPE == obMasterModel.BILL_TYPE && m.FUND_TYPE == obMasterModel.FUND_TYPE && m.OP_LVL_ID == obMasterModel.LVL_ID && m.TXN_PARENT_ID == null).Select(m => m.TXN_ID).FirstOrDefault();
                    obMasterModel.ASSET_BILL_ID = dbContext.ACC_BILL_MASTER.Any() ? dbContext.ACC_BILL_MASTER.Max(m => m.BILL_ID) + 1 : 1;
                    obMasterModel.ASSET_BILL_NO = "1";
                    acc_bill_master = CloneMasterModel(obMasterModel);
                    dbContext.ACC_BILL_MASTER.Add(acc_bill_master);
                    dbContext.SaveChanges();
                    acc_bill_master = null;
                    obMasterModel.ASSET_BILL_NO = "0";
                    obMasterModel.LIB_BILL_ID = dbContext.ACC_BILL_MASTER.Any() ? dbContext.ACC_BILL_MASTER.Max(m => m.BILL_ID) + 1 : 1;
                    obMasterModel.LIB_BILL_NO = "2";
                    acc_bill_master = CloneMasterModel(obMasterModel);
                    dbContext.ACC_BILL_MASTER.Add(acc_bill_master);
                    dbContext.SaveChanges();
                    scope.Complete();
                    return obMasterModel.ASSET_BILL_ID;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public String EditOBMaster(OBMasterModel obMasterModel)
        {
            try
            {
                dbContext = new PMGSYEntities();
                ACC_BILL_MASTER exist_acc_bill_master = new ACC_BILL_MASTER();
                ACC_BILL_MASTER acc_bill_master = new ACC_BILL_MASTER();
                exist_acc_bill_master = dbContext.ACC_BILL_MASTER.Find(obMasterModel.ASSET_BILL_ID);
                obMasterModel.FUND_TYPE = exist_acc_bill_master.FUND_TYPE;
                obMasterModel.LVL_ID = exist_acc_bill_master.LVL_ID;
                obMasterModel.ADMIN_ND_CODE = exist_acc_bill_master.ADMIN_ND_CODE;
                obMasterModel.BILL_TYPE = "O";
                obMasterModel.CHQ_EPAY = "O";
                obMasterModel.TXN_ID = exist_acc_bill_master.TXN_ID;
                obMasterModel.BILL_FINALIZED = exist_acc_bill_master.BILL_FINALIZED;
                obMasterModel.ASSET_BILL_NO = exist_acc_bill_master.BILL_NO;
                acc_bill_master = CloneMasterModel(obMasterModel);
                
                dbContext.Entry(exist_acc_bill_master).CurrentValues.SetValues(acc_bill_master);
                dbContext.SaveChanges();
                exist_acc_bill_master = null;
                exist_acc_bill_master = dbContext.ACC_BILL_MASTER.Find(obMasterModel.LIB_BILL_ID);
                
                obMasterModel.ASSET_BILL_NO = "0"; // For Edit Purpose
                obMasterModel.FUND_TYPE = exist_acc_bill_master.FUND_TYPE;
                obMasterModel.LVL_ID = exist_acc_bill_master.LVL_ID;
                obMasterModel.ADMIN_ND_CODE = exist_acc_bill_master.ADMIN_ND_CODE;
                obMasterModel.BILL_TYPE = "O";
                obMasterModel.CHQ_EPAY = "O";
                obMasterModel.TXN_ID = exist_acc_bill_master.TXN_ID;
                obMasterModel.BILL_FINALIZED = exist_acc_bill_master.BILL_FINALIZED;
                obMasterModel.LIB_BILL_NO = exist_acc_bill_master.BILL_NO;
                acc_bill_master = null;
                acc_bill_master = CloneMasterModel(obMasterModel);

                dbContext.Entry(exist_acc_bill_master).CurrentValues.SetValues(acc_bill_master);
                dbContext.SaveChanges();
                
                return "";
            }
            catch (EntityCommandExecutionException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return ex.Message;
            }
            catch (EntityException ex)
            {
                return ex.Message;
            }
            catch (Exception Ex)
            {
                return Ex.Message;
            }
            finally
            {
                dbContext.Dispose();
            }
 
        }

        public String DeleteOBMaster(String billId)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    dbContext = new PMGSYEntities();
                    Int64 billIdAsset = Convert.ToInt64(billId.Split('$')[0]);
                    Int64 billIdLib = Convert.ToInt64(billId.Split('$')[1]);

                    List<ACC_BILL_DETAILS> lstBillDetails = dbContext.ACC_BILL_DETAILS.Where(w => w.BILL_ID == billIdAsset).ToList<ACC_BILL_DETAILS>();
                    foreach (ACC_BILL_DETAILS item in lstBillDetails)
                    {
                        //added by abhishek kamble 28-nov-2013
                        item.USERID = PMGSYSession.Current.UserId;
                        item.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        dbContext.Entry(item).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();

                        dbContext.ACC_BILL_DETAILS.Remove(item);
                    }
                    dbContext.SaveChanges();

                    if (dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Any(m => m.P_BILL_ID == billIdAsset))
                    {
                        List<ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS> lstMapDetails = dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Where(m => m.P_BILL_ID == billIdAsset).ToList<ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS>();
                        if (lstMapDetails.Count() > 0)
                        {
                            foreach (var item in lstMapDetails)
                            {
                                //added by abhishek kamble 28-nov-2013
                                item.USERID = PMGSYSession.Current.UserId;
                                item.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                                dbContext.Entry(item).State = System.Data.Entity.EntityState.Modified;
                                dbContext.SaveChanges();

                                dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Remove(item);
                            }
                        }
                    }

                    dbContext.SaveChanges();

                    ACC_BILL_MASTER acc_bill_master = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == billIdAsset).FirstOrDefault();

                    //added by abhishek kamble 28-nov-2013
                    acc_bill_master.USERID = PMGSYSession.Current.UserId;
                    acc_bill_master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    dbContext.Entry(acc_bill_master).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    dbContext.ACC_BILL_MASTER.Remove(acc_bill_master);
                    dbContext.SaveChanges();

                    lstBillDetails = null;
                    acc_bill_master = null;

                    lstBillDetails = dbContext.ACC_BILL_DETAILS.Where(w => w.BILL_ID == billIdLib).ToList<ACC_BILL_DETAILS>();
                    foreach (ACC_BILL_DETAILS item in lstBillDetails)
                    {
                        //added by abhishek kamble 28-nov-2013
                        item.USERID = PMGSYSession.Current.UserId;
                        item.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        dbContext.Entry(item).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                        
                        dbContext.ACC_BILL_DETAILS.Remove(item);
                    }
                    dbContext.SaveChanges();

                    if (dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Any(m => m.P_BILL_ID == billIdLib))
                    {
                        List<ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS> lstMapDetails = dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Where(m => m.P_BILL_ID == billIdLib).ToList<ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS>();
                        if (lstMapDetails.Count() > 0)
                        {
                            foreach (var item in lstMapDetails)
                            {
                                //added by abhishek kamble 28-nov-2013
                                item.USERID = PMGSYSession.Current.UserId;
                                item.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                                dbContext.Entry(item).State = System.Data.Entity.EntityState.Modified;
                                dbContext.SaveChanges();

                                dbContext.ACC_MAP_IMPREST_PAYMENT_SETTLEMENT_DETAILS.Remove(item);
                            }
                        }
                    }

                    dbContext.SaveChanges();

                    acc_bill_master = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == billIdLib).FirstOrDefault();

                    //added by abhishek kamble 28-nov-2013
                    acc_bill_master.USERID = PMGSYSession.Current.UserId;
                    acc_bill_master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    dbContext.Entry(acc_bill_master).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    dbContext.ACC_BILL_MASTER.Remove(acc_bill_master);
                    dbContext.SaveChanges();
                }
                catch (TransactionException ex)
                {
                    return ex.Message;
                }
                finally
                {
                    scope.Complete();
                    dbContext.Dispose();
                }
            }
            return String.Empty;
 
        }

        public String AddOBDetails(OBDetailsModel obDetailsModel)
        {
            try
            {
                dbContext = new PMGSYEntities();
                commonFuncObj = new CommonFunctions();
                TransactionParams objParam = new TransactionParams();

                using (var scope = new TransactionScope())
                {
                    ACC_BILL_DETAILS acc_bill_details = new ACC_BILL_DETAILS();


                    obDetailsModel.HEAD_ID = (from item in dbContext.ACC_TXN_HEAD_MAPPING
                                                where item.TXN_ID == obDetailsModel.SUB_TXN_ID && item.CREDIT_DEBIT == obDetailsModel.CREDIT_DEBIT && item.CASH_CHQ == "O"
                                                select item.HEAD_ID).FirstOrDefault();

                    acc_bill_details = CloneDetailsModel(obDetailsModel);
                    
                    Int16? maxTxnId = dbContext.ACC_BILL_DETAILS.Where(item => item.BILL_ID == acc_bill_details.BILL_ID).Max(T => (Int16?)T.TXN_NO);
                    
                    if (maxTxnId == null)
                    {
                        acc_bill_details.TXN_NO = 1;
                    }
                    else
                    {
                        acc_bill_details.TXN_NO = Convert.ToInt16(maxTxnId + 1);
                    }

                   
                    objParam.TXN_ID = Convert.ToInt16(acc_bill_details.TXN_ID);
                    ACC_SCREEN_DESIGN_PARAM_DETAILS objDesignDetails = new ACC_SCREEN_DESIGN_PARAM_DETAILS();
                    objDesignDetails = commonFuncObj.getDetailsDesignParam(objParam);

                    if (objDesignDetails != null && objDesignDetails.CON_REQ == "Y")
                    {
                        acc_bill_details.MAST_CON_ID = obDetailsModel.MAST_CON_ID;
                    }
                    else if (objDesignDetails != null && objDesignDetails.SUPPLIER_REQ == "Y")
                    {
                        acc_bill_details.MAST_CON_ID = obDetailsModel.MAST_CON_ID;
                    }
                    else
                    {
                        acc_bill_details.MAST_CON_ID = null;
                    }

                    if (objDesignDetails != null && objDesignDetails.AGREEMENT_REQ == "Y")
                    {
                        acc_bill_details.IMS_AGREEMENT_CODE = obDetailsModel.IMS_AGREEMENT_CODE;
                    }
                    else
                    {
                        acc_bill_details.IMS_AGREEMENT_CODE = null;
                    }

                    if (objDesignDetails != null && objDesignDetails.PIU_REQ == "Y")
                    {
                        acc_bill_details.ADMIN_ND_CODE = obDetailsModel.ADMIN_ND_CODE;
                    }
                    else
                    {
                        acc_bill_details.ADMIN_ND_CODE = null;
                    }

                    if (objDesignDetails != null && objDesignDetails.ROAD_REQ == "Y")
                    {
                        acc_bill_details.IMS_PR_ROAD_CODE = obDetailsModel.IMS_PR_ROAD_CODE;
                        if (obDetailsModel.FINAL_PAYMENT == false)
                        {
                            acc_bill_details.FINAL_PAYMENT = null;
                        }
                        else
                        {
                            acc_bill_details.FINAL_PAYMENT = true;
                        }

                        acc_bill_details.MAS_FA_CODE = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == acc_bill_details.IMS_PR_ROAD_CODE).Select(m => m.IMS_COLLABORATION).FirstOrDefault();
                    }
                    else
                    {
                        acc_bill_details.IMS_PR_ROAD_CODE = null;
                        acc_bill_details.FINAL_PAYMENT = null;
                    }

                    dbContext.ACC_BILL_DETAILS.Add(acc_bill_details);
                    dbContext.SaveChanges();                    
                    scope.Complete();
                    return "";
                }
            }
            catch (EntityCommandExecutionException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return ex.Message;
            }
            catch (EntityException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return ex.Message;
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                return Ex.Message;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public String EditOBDetails(OBDetailsModel obDetailsModel)
        {
            try
            {
                dbContext = new PMGSYEntities();

                
                obDetailsModel.HEAD_ID = (from item in dbContext.ACC_TXN_HEAD_MAPPING
                                          where item.TXN_ID == obDetailsModel.SUB_TXN_ID && item.CREDIT_DEBIT == obDetailsModel.CREDIT_DEBIT && item.CASH_CHQ == "O"
                                          select item.HEAD_ID).FirstOrDefault();

                ACC_BILL_DETAILS acc_bill_details = CloneDetailsModel(obDetailsModel);

                ACC_BILL_DETAILS old_acc_bill_details = new ACC_BILL_DETAILS();
                old_acc_bill_details = dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == obDetailsModel.BILL_ID && m.TXN_NO == obDetailsModel.TXN_NO).FirstOrDefault();

                commonFuncObj = new CommonFunctions();
                TransactionParams objParam = new TransactionParams();
                objParam.TXN_ID = Convert.ToInt16(acc_bill_details.TXN_ID);
                ACC_SCREEN_DESIGN_PARAM_DETAILS objDesignDetails = new ACC_SCREEN_DESIGN_PARAM_DETAILS();
                objDesignDetails = commonFuncObj.getDetailsDesignParam(objParam);
                
                if (objDesignDetails != null && objDesignDetails.CON_REQ == "Y")
                {
                    acc_bill_details.MAST_CON_ID = obDetailsModel.MAST_CON_ID;
                }
                else if (objDesignDetails != null && objDesignDetails.SUPPLIER_REQ == "Y")
                {
                    acc_bill_details.MAST_CON_ID = obDetailsModel.MAST_CON_ID;
                }
                else
                {
                    acc_bill_details.MAST_CON_ID = null;
                }

                if (objDesignDetails != null && objDesignDetails.AGREEMENT_REQ == "Y")
                {
                    acc_bill_details.IMS_AGREEMENT_CODE = obDetailsModel.IMS_AGREEMENT_CODE;
                }
                else
                {
                    acc_bill_details.IMS_AGREEMENT_CODE = null;
                }

                if (objDesignDetails != null && objDesignDetails.PIU_REQ == "Y")
                {
                    acc_bill_details.ADMIN_ND_CODE = obDetailsModel.ADMIN_ND_CODE;
                }
                else
                {
                    acc_bill_details.ADMIN_ND_CODE = null;
                }
                if (objDesignDetails != null && objDesignDetails.ROAD_REQ == "Y")
                {
                    acc_bill_details.IMS_PR_ROAD_CODE = obDetailsModel.IMS_PR_ROAD_CODE;
                    if(obDetailsModel.FINAL_PAYMENT == false)
                    {
                        acc_bill_details.FINAL_PAYMENT = null;
                    }
                    else
                    {
                        acc_bill_details.FINAL_PAYMENT = true;
                    }
                    acc_bill_details.MAS_FA_CODE = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == acc_bill_details.IMS_PR_ROAD_CODE).Select(m => m.IMS_COLLABORATION).FirstOrDefault();
                }
                else
                {
                    acc_bill_details.IMS_PR_ROAD_CODE = null;
                    acc_bill_details.FINAL_PAYMENT = null;
                    acc_bill_details.MAS_FA_CODE = null;
                }

                dbContext.Entry(old_acc_bill_details).CurrentValues.SetValues(acc_bill_details);
                dbContext.SaveChanges();

                return "";
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw Ex;
            }
            finally
            {
                dbContext.Dispose();
            }
 
        }

        public String DeleteOBDetails(Int64 billId, Int16 transNo)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    dbContext = new PMGSYEntities();
                    ACC_BILL_DETAILS acc_bill_details = new ACC_BILL_DETAILS();
                    acc_bill_details = dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == billId && m.TXN_NO == transNo).FirstOrDefault();

                    //added by abhishek kamble 28-nov-2013
                    acc_bill_details.USERID = PMGSYSession.Current.UserId;
                    acc_bill_details.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    dbContext.Entry(acc_bill_details).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();               

                    dbContext.ACC_BILL_DETAILS.Remove(acc_bill_details);
                    dbContext.SaveChanges();
                }
                catch (TransactionException ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    return ex.Message;
                }
                catch (Exception Ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                    throw Ex;
                }
                finally
                {
                    scope.Complete();
                    dbContext.Dispose();
                }
            }
            return String.Empty;
 
        }

        public Array GetOBMasterList(ReceiptFilterModel objFilter, out long totalRecords)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<OBMasterList> lstMasterList = new List<OBMasterList>();

                lstMasterList = (from m in dbContext.ACC_BILL_MASTER
                              where
                              m.ADMIN_ND_CODE == objFilter.AdminNdCode && m.LVL_ID == objFilter.LevelId
                              && m.FUND_TYPE == objFilter.FundType && m.BILL_TYPE == objFilter.BillType
                              select new OBMasterList
                              {
                                  BILL_ID = m.BILL_ID,
                                  BILL_NO = m.BILL_NO,
                                  BILL_DATE = m.BILL_DATE,
                                  GROSS_AMOUNT = m.GROSS_AMOUNT,
                                  DETAILS_AMOUNT = m.ACC_BILL_DETAILS.Any(t => t.BILL_ID == m.BILL_ID) ? m.ACC_BILL_DETAILS.Where(t => t.BILL_ID == m.BILL_ID).Sum(t => t.AMOUNT) : 0,
                                  TXN_DESC = m.ACC_MASTER_TXN.TXN_DESC,
                                  BILL_FINALIZED = m.BILL_FINALIZED,
                                  BILL_MONTH = m.BILL_MONTH,
                                  BILL_YEAR = m.BILL_YEAR,
                                  ACTION_REQUIRED = m.ACTION_REQUIRED
                              }).Distinct().ToList<OBMasterList>();


                //added by koustubh Nakate on 11/10/2013 for grouping

                lstMasterList = lstMasterList.GroupBy(lst => lst.BILL_ID).Select(lst => lst.FirstOrDefault()).ToList<OBMasterList>();


                totalRecords = lstMasterList.Count();

                if (objFilter.sidx.Trim() != string.Empty)
                {
                    if (objFilter.sord.ToString() == "asc")
                    {
                        switch (objFilter.sidx)
                        {
                            case "OBType":
                                lstMasterList = lstMasterList.OrderBy(x => x.BILL_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<OBMasterList>();
                                break;
                            case "OBDate":
                                lstMasterList = lstMasterList.OrderBy(x => x.BILL_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<OBMasterList>();
                                break;
                            case "TransactionName":
                                lstMasterList = lstMasterList.OrderBy(x => x.TXN_DESC).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<OBMasterList>();
                                break;
                            case "GrossAmount":
                                lstMasterList = lstMasterList.OrderBy(x => x.GROSS_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<OBMasterList>();
                                break;
                            default:
                                lstMasterList = lstMasterList.OrderBy(x => x.BILL_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<OBMasterList>();
                                break;
                        }

                    }
                    else
                    {
                        switch (objFilter.sidx)
                        {
                            case "OBType":
                                lstMasterList = lstMasterList.OrderByDescending(x => x.BILL_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<OBMasterList>();
                                break;
                            case "OBDate":
                                lstMasterList = lstMasterList.OrderByDescending(x => x.BILL_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<OBMasterList>();
                                break;
                            case "TransactionName":
                                lstMasterList = lstMasterList.OrderByDescending(x => x.TXN_DESC).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<OBMasterList>();
                                break;
                            case "GrossAmount":
                                lstMasterList = lstMasterList.OrderByDescending(x => x.GROSS_AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<OBMasterList>();
                                break;
                            default:
                                lstMasterList = lstMasterList.OrderByDescending(x => x.BILL_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<OBMasterList>();
                                break;
                        }
                    }
                }
                else
                {
                    lstMasterList = lstMasterList.OrderByDescending(x => x.BILL_DATE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<OBMasterList>();
                }
                Int64 AssBillId = lstMasterList.Where(m => m.BILL_NO == "1").Select(m => m.BILL_ID).FirstOrDefault();
                Int64 LibBillId = lstMasterList.Where(m=>m.BILL_NO == "2").Select(m=>m.BILL_ID).FirstOrDefault();
                Decimal? AssetDetailsAmount = lstMasterList.Where(m => m.BILL_NO == "1").Select(m => m.DETAILS_AMOUNT).FirstOrDefault();
                Decimal? LibDetailsAmount = lstMasterList.Where(m => m.BILL_NO == "2").Select(m => m.DETAILS_AMOUNT).FirstOrDefault(); 
                return lstMasterList.Select(item => new
                {

                    id = URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() }),
                    cell = new[] {                         
                                    
                                    item.BILL_NO == "1" ? "Assets" : "Liabilities",
                                    item.TXN_DESC.Trim(),
                                    Convert.ToDateTime(item.BILL_DATE).ToString("dd/MM/yyyy"),
                                    item.GROSS_AMOUNT.ToString(),
                                    item.DETAILS_AMOUNT.ToString(),
                                    //item.GROSS_AMOUNT == item.DETAILS_AMOUNT ? (item.BILL_FINALIZED == "N" ? "<center><a href='#' class='ui-icon ui-icon-unlocked' onclick='LockOB(\"" +URLEncrypt.EncryptParameters(new string[] { AssBillId.ToString().Trim() +"_"+ LibBillId.ToString().Trim() })+ "\");return false;'>Lock</a></center>" : "<center><span class='ui-icon ui-icon-locked'></span></center>") : "<center><div style='width:50px;height:10px; background-color:#b83400; border:0.1em solid #b83400; text-align:left' class='ui-corner-all' title='Rs "+((item.GROSS_AMOUNT) - item.DETAILS_AMOUNT)+" remaining'><div style='width:"+(((item.DETAILS_AMOUNT/item.GROSS_AMOUNT)*100)/2)+"px;height:100%; background-color:#4eb305' class='ui-corner-left' title='Rs "+(item.DETAILS_AMOUNT)+" entered'></div></div></center>",
                                    (item.GROSS_AMOUNT == AssetDetailsAmount && item.GROSS_AMOUNT == LibDetailsAmount) ? (item.BILL_FINALIZED == "N" ? "<center><a href='#' class='ui-icon ui-icon-unlocked' onclick='LockOB(\"" +URLEncrypt.EncryptParameters(new string[] { AssBillId.ToString().Trim() +"_"+ LibBillId.ToString().Trim() })+ "\");return false;'>Lock</a></center>" : "<center><span class='ui-icon ui-icon-locked'></span></center>") : "<center><div style='width:50px;height:10px; background-color:#b83400; border:0.1em solid #b83400; text-align:left' class='ui-corner-all' title='Rs "+((item.GROSS_AMOUNT) - item.DETAILS_AMOUNT)+" remaining'><div style='width:"+ (item.GROSS_AMOUNT==0?0:(((item.DETAILS_AMOUNT/item.GROSS_AMOUNT)*100)/2)) +"px;height:100%; background-color:#4eb305' class='ui-corner-left' title='Rs "+(item.DETAILS_AMOUNT)+" entered'></div></div></center>",
                                    item.BILL_FINALIZED=="N"? "<center><a href='#' class='ui-icon ui-icon-circle-plus' onclick='AddOBDetails(\"" +URLEncrypt.EncryptParameters(new string[] { AssBillId.ToString().Trim() +"_"+ LibBillId.ToString().Trim() })+"\",\""+item.BILL_NO+ "\");return false;'>Add Transactions</a></center>" : "<center><a href='#' class='ui-icon ui-icon-search' onclick='ViewOBDetails(\"" +URLEncrypt.EncryptParameters(new string[] { AssBillId.ToString().Trim() +"_"+ LibBillId.ToString().Trim() })+ "\");return false;'>View OB Details</a></center>",
                                    item.BILL_FINALIZED=="N"? "<center><a href='#' class='ui-icon ui-icon-pencil' onclick='EditOB(\"" +URLEncrypt.EncryptParameters(new string[] { AssBillId.ToString().Trim() +"$"+ LibBillId.ToString().Trim() })+ "\");return false;'>Edit OB Master</a></center>" : "<center><span class='ui-icon ui-icon-locked'></span></center>",
                                    item.BILL_FINALIZED=="N"? "<center><a href='#' class='ui-icon ui-icon-trash' onclick='DeleteOB(\"" +URLEncrypt.EncryptParameters(new string[] { AssBillId.ToString().Trim() +"$"+ LibBillId.ToString().Trim() })+"\",\""+AssetDetailsAmount.ToString()+"$"+LibDetailsAmount.ToString()+"\");return false;'>Delete</a></center>" : "<center><span class='ui-icon ui-icon-locked'></span></center>",                                    
                                    item.ACTION_REQUIRED == "C" ? "<center><span title='Transaction details Incorrect, Needs Correction' style=color:#1C94C4; font-weight:bold' class='C'>"+item.ACTION_REQUIRED+"</span></center>" : item.ACTION_REQUIRED == "O" ? "<center><span title='Wrong Head Entry, Delete records and insert correct transactions' style=color:#b83400; font-weight:bold' class='O'>"+item.ACTION_REQUIRED+"</span></center>" : item.ACTION_REQUIRED == "M" ? "<center><span  title='Details not present, Unfinalize this record and insert details transactions' style=color:##014421; font-weight:bold' class='M'>"+item.ACTION_REQUIRED+"</span></center>" : "<center><span  title='Correct Transaction Entry' class='ui-icon ui-icon-check'>"+item.ACTION_REQUIRED+"</span></center>"                            

                   }
                }).ToArray();


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

        public Array GetOBDetailsList(ReceiptFilterModel objFilter, out long totalRecords, out decimal AssetTotal, out decimal LibTotal, out decimal GrossAmount, out string Finalized)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<ACC_BILL_DETAILS> lstBillDetails = null;
                lstBillDetails = dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == objFilter.AssetBillId || m.BILL_ID == objFilter.LibBillId).ToList<ACC_BILL_DETAILS>();

                if (lstBillDetails.Count() > 0)
                {
                    AssetTotal = lstBillDetails.Where(m => m.BILL_ID == objFilter.AssetBillId).Sum(m => m.AMOUNT);
                    LibTotal = lstBillDetails.Where(m => m.BILL_ID == objFilter.LibBillId).Sum(m => m.AMOUNT);
                }
                else
                {
                    AssetTotal = 0;
                    LibTotal = 0;
                }
                ACC_BILL_MASTER acc_bill_master = new ACC_BILL_MASTER();
                acc_bill_master = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == objFilter.AssetBillId).FirstOrDefault();
                string isFinalize = acc_bill_master.BILL_FINALIZED;
                Finalized = isFinalize;
                GrossAmount = acc_bill_master.GROSS_AMOUNT;

                totalRecords = lstBillDetails.Count();

                if (objFilter.sidx.Trim() != string.Empty)
                {
                    if (objFilter.sord.ToString() == "asc")
                    {
                        switch (objFilter.sidx)
                        {
                            case "CreditDebit":
                                lstBillDetails = lstBillDetails.OrderBy(x => x.CREDIT_DEBIT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_DETAILS>();
                                break;
                            case "HeadName":
                                lstBillDetails = lstBillDetails.OrderBy(x => x.ACC_MASTER_HEAD.HEAD_NAME).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_DETAILS>();
                                break;
                            case "Contractor":
                                lstBillDetails = lstBillDetails.OrderBy(x => x.MASTER_CONTRACTOR.MAST_CON_COMPANY_NAME).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_DETAILS>();
                                break;
                            case "Agreement":
                                lstBillDetails = lstBillDetails.OrderBy(x => x.IMS_AGREEMENT_CODE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_DETAILS>();
                                break;
                            case "DPIU":
                                lstBillDetails = lstBillDetails.OrderBy(x => x.ADMIN_ND_CODE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_DETAILS>();
                                break;
                           case "CAmount":
                                lstBillDetails = lstBillDetails.OrderBy(x => x.AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_DETAILS>();
                                break;
                            case "DAmount":
                                lstBillDetails = lstBillDetails.OrderBy(x => x.AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_DETAILS>();
                                break;
                            default:
                                lstBillDetails = lstBillDetails.OrderBy(x => x.TXN_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_DETAILS>();
                                break;
                        }
                    }
                    else
                    {
                        switch (objFilter.sidx)
                        {
                            case "CreditDebit":
                                lstBillDetails = lstBillDetails.OrderByDescending(x => x.CREDIT_DEBIT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_DETAILS>();
                                break;
                            case "HeadName":
                                lstBillDetails = lstBillDetails.OrderByDescending(x => x.ACC_MASTER_HEAD.HEAD_NAME).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_DETAILS>();
                                break;
                            case "Contractor":
                                lstBillDetails = lstBillDetails.OrderByDescending(x => x.MASTER_CONTRACTOR.MAST_CON_COMPANY_NAME).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_DETAILS>();
                                break;
                            //case "Agreement":
                            //        lstBillDetails = lstBillDetails.OrderByDescending(x => x.TEND_AGREEMENT_MASTER.TEND_AGREEMENT_NUMBER).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_DETAILS>();
                            case "DPIU":
                                lstBillDetails = lstBillDetails.OrderByDescending(x => x.ADMIN_ND_CODE).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_DETAILS>();
                                break;
                            case "CAmount":
                                lstBillDetails = lstBillDetails.OrderByDescending(x => x.AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_DETAILS>();
                                break;
                            case "DAmount":
                                lstBillDetails = lstBillDetails.OrderByDescending(x => x.AMOUNT).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_DETAILS>();
                                break;
                            default:
                                lstBillDetails = lstBillDetails.OrderByDescending(x => x.TXN_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_DETAILS>();
                                break;
                        }
                    }
                }
                else
                {
                    lstBillDetails = lstBillDetails.OrderByDescending(x => x.TXN_NO).Skip(Convert.ToInt32(objFilter.page * objFilter.rows)).Take(objFilter.rows).ToList<ACC_BILL_DETAILS>();
                }

                return lstBillDetails.Select(item => new
                {
                    id = URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() + "$" + item.TXN_NO.ToString().Trim() }),
                    cell = new[] {                         
                                    
                                    item.CREDIT_DEBIT == "D" ? "Assets" : "Liabilities",
                                    "<span class='ui-state-default' style='border:none'>"+item.ACC_MASTER_HEAD.HEAD_CODE.Trim()+"</span> "+item.ACC_MASTER_HEAD.HEAD_NAME.Trim(),
                                    item.MAST_CON_ID == null ? "-" : item.MASTER_CONTRACTOR.MAST_CON_COMPANY_NAME.ToString(),
                                    //Old
                                    //item.IMS_AGREEMENT_CODE == null ? "-" : PMGSYSession.Current.FundType.ToLower() == "m" ? dbContext.MANE_IMS_CONTRACT.Where(t=>t.MANE_PR_CONTRACT_CODE == item.IMS_AGREEMENT_CODE).Select(t=>t.MANE_AGREEMENT_NUMBER).FirstOrDefault() : dbContext.TEND_AGREEMENT_MASTER.Where(t=>t.TEND_AGREEMENT_CODE == item.IMS_AGREEMENT_CODE).Select(t=>t.TEND_AGREEMENT_NUMBER).FirstOrDefault(),
                                    //Modified By Abhishek kamble 17Nov2014 TO get Agr Code using MANE_CONTRACTOR_ID 
                                    item.IMS_AGREEMENT_CODE == null ? "-" : PMGSYSession.Current.FundType.ToLower() == "m" ? dbContext.MANE_IMS_CONTRACT.Where(t=>t.MANE_CONTRACT_ID == item.IMS_AGREEMENT_CODE).Select(t=>t.MANE_AGREEMENT_NUMBER).FirstOrDefault() : dbContext.TEND_AGREEMENT_MASTER.Where(t=>t.TEND_AGREEMENT_CODE == item.IMS_AGREEMENT_CODE).Select(t=>t.TEND_AGREEMENT_NUMBER).FirstOrDefault(),
                                    dbContext.IMS_SANCTIONED_PROJECTS.Where(m=>m.IMS_PR_ROAD_CODE==item.IMS_PR_ROAD_CODE).Select(s=>s.IMS_ROAD_NAME).FirstOrDefault(),                                    
                                    item.ADMIN_ND_CODE == null ? "-" : item.ADMIN_DEPARTMENT.ADMIN_ND_NAME,
                                    item.CREDIT_DEBIT == "D" ? item.AMOUNT.ToString() : "0.00",
                                    item.CREDIT_DEBIT == "C" ? item.AMOUNT.ToString() : "0.00",
                                    item.NARRATION,
                                    isFinalize == "N" ? "<center><a href='#' class='ui-icon ui-icon-pencil' onclick='EditOBDetails(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() + "$" + item.TXN_NO.ToString().Trim() })+"\");return false;'>Edit</a></center>" : "<center><span class='ui-icon ui-icon-locked'></span></center>",
                                    isFinalize == "N" ? "<center><a href='#' class='ui-icon ui-icon-trash' onclick='DeleteOBDetails(\"" +URLEncrypt.EncryptParameters(new string[] { item.BILL_ID.ToString().Trim() + "$" + item.TXN_NO.ToString().Trim() })+"\");return false;'>Delete</a></center>" : "<center><span class='ui-icon ui-icon-locked'></span></center>"  ,                                  
                                     (item.TXN_ID != 0 &&  item.TXN_ID != null) ? dbContext.ACC_MASTER_TXN.Where(x => x.TXN_ID == item.TXN_ID).Select(x => x.IS_OPERATIONAL).FirstOrDefault()==true ? "Correct Entry" :dbContext.ACC_MASTER_TXN.Where(x => x.TXN_ID == item.TXN_ID).Select(x => x.IS_REQ_AFTER_PORTING).FirstOrDefault() ==true ? "Edit And Correct the entry" :"Delete and Make new entry" : "Correct Entry"
                   }
                }).ToArray();

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                AssetTotal = 0;
                LibTotal = 0;
                GrossAmount = 0;
                Finalized = "N";
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public String FinalizeOB(Int64 assetBillId, Int64 libBillId)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    dbContext = new PMGSYEntities();
                    decimal? assAmount = dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == assetBillId).Sum(m => (decimal?)m.AMOUNT);
                    decimal? libAmount = dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == libBillId).Sum(m => (decimal?)m.AMOUNT);
                    decimal? grossAmount = dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == assetBillId).Sum(m => (decimal?)m.GROSS_AMOUNT);
                    assAmount = assAmount == null ? 0 : assAmount;
                    libAmount = libAmount == null ? 0 : libAmount;
                    grossAmount = grossAmount == null ? 0 : grossAmount;
                    
                    TransactionParams objParam = new TransactionParams();
                    objParam.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                    objParam.LVL_ID = PMGSYSession.Current.LevelId;
                    objParam.FUND_TYPE = PMGSYSession.Current.FundType;
                    // if receipt , payment and TEo details are present against unfinalize ob and for dates validations
                   
                    //Validation Commented by Abhishek kamble 17-Oct-2014 to finalize OB
                    //if(dbContext.ACC_BILL_MASTER.Where(m => m.ADMIN_ND_CODE == objParam.ADMIN_ND_CODE && m.LVL_ID == objParam.LVL_ID && m.FUND_TYPE == objParam.FUND_TYPE && m.BILL_TYPE != "O").Any())
                    //{
                    //    scope.Complete();
                    //    return "Cannot Finalize, Payment / Receipt / Transfer Entry Order Details Already Present.";
                    //}
                    //else
                    //{
                        if (grossAmount == assAmount && grossAmount == libAmount) // && grossAmount != 0)   commented by Vikram as per the 0 opening balance need to be finalized.
                        {
                            ACC_BILL_MASTER acc_bill_master = dbContext.ACC_BILL_MASTER.Find(assetBillId);
                            acc_bill_master.BILL_FINALIZED = "Y";
                            acc_bill_master.ACTION_REQUIRED = "N";
                            dbContext.Entry(acc_bill_master).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                            acc_bill_master = null;
                            acc_bill_master = dbContext.ACC_BILL_MASTER.Find(libBillId);
                            acc_bill_master.BILL_FINALIZED = "Y";
                            acc_bill_master.ACTION_REQUIRED = "N";

                            //added by abhishek kamble 28-nov-2013
                            acc_bill_master.USERID = PMGSYSession.Current.UserId;
                            acc_bill_master.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            
                            dbContext.Entry(acc_bill_master).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                            scope.Complete();
                            return String.Empty;
                        }
                        else
                        {
                            scope.Complete();
                            return "Cannot Finalize, Gross Amount and Details Amount Does not Match.";
                        }
                  //  }


                }
                catch (EntityCommandExecutionException ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    return ex.Message;
                }
                catch (EntityException ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    return ex.Message;
                }
                catch (Exception Ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                    return Ex.Message;
                }
                finally
                {
                    dbContext.Dispose();
                }
            }
        }

        public OBMasterModel CloneMasterObject(ACC_BILL_MASTER acc_bill_master)
        {
            try
            {
                commonFuncObj = new CommonFunctions();
                OBMasterModel obMasterModel = new OBMasterModel();
                if (acc_bill_master.BILL_NO == "1")
                {
                    obMasterModel.ASSET_BILL_DATE = commonFuncObj.GetDateTimeToString(acc_bill_master.BILL_DATE);
                    obMasterModel.ASSET_GROSS_AMOUNT = acc_bill_master.GROSS_AMOUNT;
                    obMasterModel.ASSET_BILL_ID = acc_bill_master.BILL_ID;
                    obMasterModel.ASSET_BILL_NO = acc_bill_master.BILL_NO;                    
                }
                else
                {
                    obMasterModel.LIB_BILL_DATE = commonFuncObj.GetDateTimeToString(acc_bill_master.BILL_DATE);
                    obMasterModel.LIB_GROSS_AMOUNT = acc_bill_master.GROSS_AMOUNT;
                    obMasterModel.LIB_BILL_ID = acc_bill_master.BILL_ID;
                    obMasterModel.LIB_BILL_NO = acc_bill_master.BILL_NO;                    
                }
                obMasterModel.TXN_ID = acc_bill_master.TXN_ID;
                obMasterModel.CHQ_EPAY = acc_bill_master.CHQ_EPAY;                
                obMasterModel.BILL_FINALIZED = acc_bill_master.BILL_FINALIZED;
                obMasterModel.ADMIN_ND_CODE = acc_bill_master.ADMIN_ND_CODE;
                obMasterModel.FUND_TYPE = acc_bill_master.FUND_TYPE;
                obMasterModel.LVL_ID = acc_bill_master.LVL_ID;
                obMasterModel.BILL_TYPE = acc_bill_master.BILL_TYPE;
                
                return obMasterModel;
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw Ex;
            }
        }

        public ACC_BILL_MASTER CloneMasterModel(OBMasterModel obMasterModel)
        {
            try
            {
                commonFuncObj = new CommonFunctions();
                ACC_BILL_MASTER acc_bill_master = new ACC_BILL_MASTER();

                if (obMasterModel.ASSET_BILL_NO == "1")
                {
                    acc_bill_master.BILL_ID = obMasterModel.ASSET_BILL_ID;
                    acc_bill_master.BILL_NO = obMasterModel.ASSET_BILL_NO;
                    acc_bill_master.BILL_DATE = commonFuncObj.GetStringToDateTime(obMasterModel.ASSET_BILL_DATE);
                    acc_bill_master.GROSS_AMOUNT = (decimal)obMasterModel.ASSET_GROSS_AMOUNT;
                }
                else
                {
                    acc_bill_master.BILL_ID = obMasterModel.LIB_BILL_ID;
                    acc_bill_master.BILL_NO = obMasterModel.LIB_BILL_NO;
                    acc_bill_master.BILL_DATE = commonFuncObj.GetStringToDateTime(obMasterModel.LIB_BILL_DATE);
                    acc_bill_master.GROSS_AMOUNT = (decimal)obMasterModel.LIB_GROSS_AMOUNT;
                }
                acc_bill_master.BILL_MONTH = (short)acc_bill_master.BILL_DATE.Month;
                acc_bill_master.BILL_YEAR = (short)acc_bill_master.BILL_DATE.Year;
                acc_bill_master.TXN_ID = obMasterModel.TXN_ID;
                acc_bill_master.CHQ_EPAY = obMasterModel.CHQ_EPAY;
                acc_bill_master.BILL_FINALIZED = obMasterModel.BILL_FINALIZED;
                acc_bill_master.ADMIN_ND_CODE = obMasterModel.ADMIN_ND_CODE;
                acc_bill_master.FUND_TYPE = obMasterModel.FUND_TYPE;
                acc_bill_master.LVL_ID = (byte)obMasterModel.LVL_ID;
                acc_bill_master.BILL_TYPE = obMasterModel.BILL_TYPE;

                //added by abhishek kamble 28-nov-2013
                acc_bill_master.USERID = PMGSYSession.Current.UserId;
                acc_bill_master.IPADD=HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                
                return acc_bill_master;
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw Ex;
            }            
        }

        public OBDetailsModel CloneDetailsObject(ACC_BILL_DETAILS acc_bill_details)
        {
            try
            {
                OBDetailsModel obDetailsModel = new OBDetailsModel();
                obDetailsModel.BILL_ID = acc_bill_details.BILL_ID;
                obDetailsModel.TXN_NO = acc_bill_details.TXN_NO;
                obDetailsModel.SUB_TXN_ID = Convert.ToInt16(acc_bill_details.TXN_ID);
                obDetailsModel.TXN_ID = dbContext.ACC_MASTER_TXN.Where(m => m.TXN_ID == obDetailsModel.SUB_TXN_ID).Select(m => (Int16)m.TXN_PARENT_ID).FirstOrDefault();
                obDetailsModel.HEAD_ID = acc_bill_details.HEAD_ID;
                obDetailsModel.ADMIN_ND_CODE = acc_bill_details.ADMIN_ND_CODE;
                obDetailsModel.MAST_CON_ID = acc_bill_details.MAST_CON_ID;
                obDetailsModel.IMS_AGREEMENT_CODE = acc_bill_details.IMS_AGREEMENT_CODE;
                obDetailsModel.IMS_PR_ROAD_CODE = acc_bill_details.IMS_PR_ROAD_CODE;
                if (obDetailsModel.IMS_PR_ROAD_CODE != null)
                {
                    obDetailsModel.FINAL_PAYMENT = acc_bill_details.FINAL_PAYMENT == null ? false : true;
                }
                obDetailsModel.AMOUNT = acc_bill_details.AMOUNT;
                obDetailsModel.NARRATION = acc_bill_details.NARRATION;
                obDetailsModel.CREDIT_DEBIT = acc_bill_details.CREDIT_DEBIT;

                commonFuncObj = new CommonFunctions();
                ACC_SCREEN_DESIGN_PARAM_DETAILS designparams = new ACC_SCREEN_DESIGN_PARAM_DETAILS();                
                TransactionParams objParam = new TransactionParams();
                objParam.TXN_ID = obDetailsModel.SUB_TXN_ID;
                designparams = commonFuncObj.getDetailsDesignParam(objParam);
                
                if (designparams != null && designparams.YEAR_REQ == "Y")
                {
                    obDetailsModel.SANC_YEAR = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == acc_bill_details.IMS_PR_ROAD_CODE).Select(m => m.IMS_YEAR).FirstOrDefault();
                }
                else
                {
                    obDetailsModel.SANC_YEAR = null;
                }
                if (designparams != null && designparams.PKG_REQ == "Y")
                {
                    obDetailsModel.IMS_PACKAGE_ID = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == acc_bill_details.IMS_PR_ROAD_CODE).Select(m => m.IMS_PACKAGE_ID).FirstOrDefault();
                }
                else
                {
                    obDetailsModel.IMS_PACKAGE_ID = null;
                }
                return obDetailsModel;
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw Ex;
            }
        }

        public ACC_BILL_DETAILS CloneDetailsModel(OBDetailsModel obDetailsModel)
        {
            try
            {
                ACC_BILL_DETAILS acc_bill_Details = new ACC_BILL_DETAILS();
                acc_bill_Details.BILL_ID = obDetailsModel.BILL_ID;
                acc_bill_Details.CASH_CHQ = obDetailsModel.CASH_CHQ;
                acc_bill_Details.AMOUNT = Convert.ToDecimal(obDetailsModel.AMOUNT);
                acc_bill_Details.NARRATION = obDetailsModel.NARRATION;
                acc_bill_Details.TXN_NO = obDetailsModel.TXN_NO;
                acc_bill_Details.TXN_ID = obDetailsModel.SUB_TXN_ID;
                acc_bill_Details.HEAD_ID = obDetailsModel.HEAD_ID;
                acc_bill_Details.CREDIT_DEBIT = obDetailsModel.CREDIT_DEBIT;
                acc_bill_Details.ADMIN_ND_CODE = obDetailsModel.ADMIN_ND_CODE;
                acc_bill_Details.MAST_CON_ID = obDetailsModel.MAST_CON_ID;
                acc_bill_Details.IMS_AGREEMENT_CODE = obDetailsModel.IMS_AGREEMENT_CODE;
                acc_bill_Details.IMS_PR_ROAD_CODE = obDetailsModel.IMS_PR_ROAD_CODE;
                if (acc_bill_Details.IMS_PR_ROAD_CODE != null)
                {
                    acc_bill_Details.FINAL_PAYMENT = obDetailsModel.FINAL_PAYMENT; 
                }

                //added by abhishek kamble 28-nov-2013
                acc_bill_Details.USERID = PMGSYSession.Current.UserId;
                acc_bill_Details.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                
                return acc_bill_Details;
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw Ex;
            }
        }

        public String GetOBStatus(TransactionParams objParam)
        {
            try
            {
                dbContext = new PMGSYEntities();
                String status = dbContext.ACC_BILL_MASTER.Where(m => m.ADMIN_ND_CODE == objParam.ADMIN_ND_CODE && m.LVL_ID == objParam.LVL_ID && m.FUND_TYPE == objParam.FUND_TYPE && m.BILL_TYPE == objParam.BILL_TYPE && m.BILL_NO == "1").Select(m => m.BILL_FINALIZED).FirstOrDefault();
                return status == null ? "N" : status;
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw Ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public String ValidateOBDetails(OBDetailsModel obDetailsModel)
        {
            return String.Empty;
        }

        public String GetAssetAmountDetails(TransactionParams objParam)
        {
            try
            {
                dbContext = new PMGSYEntities();
                ACC_BILL_MASTER acc_bill_master = new ACC_BILL_MASTER();
                acc_bill_master = dbContext.ACC_BILL_MASTER.Where(m => m.ADMIN_ND_CODE == objParam.ADMIN_ND_CODE && m.LVL_ID == objParam.LVL_ID && m.FUND_TYPE == objParam.FUND_TYPE && m.BILL_TYPE == objParam.BILL_TYPE && m.BILL_NO == objParam.BILL_NO).FirstOrDefault();
                if (acc_bill_master == null)
                {
                    return "0$0";
                }
                Decimal detailsAmount = 0;
                Decimal oldDetailsAmount = 0;
                if (dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == acc_bill_master.BILL_ID).Any())
                {
                    detailsAmount = dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == acc_bill_master.BILL_ID).Sum(m => m.AMOUNT);
                    //detailsAmount = detailsAmount == null ? 0 : detailsAmount;                    
                    if (objParam.TXN_NO > 0)
                    {
                        oldDetailsAmount = dbContext.ACC_BILL_DETAILS.Where(m => m.BILL_ID == acc_bill_master.BILL_ID && m.TXN_NO == objParam.TXN_NO).Select(m => m.AMOUNT).FirstOrDefault();
                        //if (oldDetailsAmount != null)
                        //{
                        //    detailsAmount = detailsAmount - oldDetailsAmount;
                        //}
                        detailsAmount = detailsAmount - oldDetailsAmount;
                    }
                }
                return acc_bill_master.GROSS_AMOUNT + "$" + detailsAmount;
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw Ex;
            }
            finally
            {
                dbContext.Dispose();
            }
 
        }

        public List<OBChart> GetAssetLiabilityDetails(TransactionParams objParam)
        {
            List<OBChart> lstParams = new List<OBChart>();
            try
            {
                dbContext = new PMGSYEntities();
                Int64 assetBillId = dbContext.ACC_BILL_MASTER.Where(m => m.ADMIN_ND_CODE == objParam.ADMIN_ND_CODE && m.LVL_ID == objParam.LVL_ID && m.FUND_TYPE == objParam.FUND_TYPE && m.BILL_TYPE == objParam.BILL_TYPE && m.BILL_NO == "1").Select(m => m.BILL_ID).FirstOrDefault();
                Int64 libBillId = dbContext.ACC_BILL_MASTER.Where(m => m.ADMIN_ND_CODE == objParam.ADMIN_ND_CODE && m.LVL_ID == objParam.LVL_ID && m.FUND_TYPE == objParam.FUND_TYPE && m.BILL_TYPE == objParam.BILL_TYPE && m.BILL_NO == "2").Select(m => m.BILL_ID).FirstOrDefault();

                //var assetlst = (from m in dbContext.ACC_BILL_MASTER
                //               join d in dbContext.ACC_BILL_DETAILS
                //               on m.BILL_ID equals d.BILL_ID
                //               where m.BILL_ID == assetBillId
                //               group new { m, d } by d.HEAD_ID into newItem
                //               select new
                //               {
                //                   headid = newItem.Key,
                //                   tname = dbContext.ACC_MASTER_HEAD.Where(m => m.HEAD_ID == newItem.Key).FirstOrDefault(),
                //                   Amount = newItem.Sum(m => m.d.AMOUNT)

                //               });

                var assetlst = (from item in dbContext.ACC_BILL_DETAILS
                                where item.BILL_ID == assetBillId
                                group new { item } by item.TXN_ID
                                into newItem
                                select new
                                {
                                    TransName = newItem.Where(m=>m.item.TXN_ID == newItem.Key.Value).FirstOrDefault(),
                                    Amount = newItem.Sum(m => m.item.AMOUNT)
                                });
  



                foreach (var item in assetlst)
                {
                    OBChart obChart = new OBChart();
                    obChart.Id = 1;
                    obChart.TransDesc = item.TransName.item.ACC_MASTER_TXN.TXN_DESC;
                    obChart.Amount = item.Amount;
                    lstParams.Add(obChart);
                }

                var liblst = (from item in dbContext.ACC_BILL_DETAILS
                              where item.BILL_ID == libBillId
                              group new { item } by item.TXN_ID
                              into newItem
                              select new
                              {
                                  TransName = newItem.Where(m => m.item.TXN_ID == newItem.Key.Value).FirstOrDefault(),
                                  Amount = newItem.Sum(m => m.item.AMOUNT)
                              });
  



                foreach (var item in liblst)
                {
                    OBChart obChart = new OBChart();
                    obChart.Id = 2;
                    obChart.TransDesc = item.TransName.item.ACC_MASTER_TXN.TXN_DESC;
                    obChart.Amount = item.Amount;
                    lstParams.Add(obChart);
                }

                return lstParams;
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw Ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public String GetAccountStatus(TransactionParams objParam)
        {
            try
            {
                dbContext = new PMGSYEntities();
                if (dbContext.ACC_BILL_MASTER.Where(m => m.ADMIN_ND_CODE == objParam.ADMIN_ND_CODE && m.LVL_ID == objParam.LVL_ID && m.FUND_TYPE == objParam.FUND_TYPE && m.BILL_TYPE != "O").Any())
                {
                    if (dbContext.ACC_BILL_MASTER.Where(m => m.ADMIN_ND_CODE == objParam.ADMIN_ND_CODE && m.LVL_ID == objParam.LVL_ID && m.FUND_TYPE == objParam.FUND_TYPE && m.BILL_TYPE == "O").Any())
                    {
                        return "N";
                    }
                    else if (dbContext.ADMIN_DEPARTMENT.Where(m => m.MAST_PARENT_ND_CODE == 1131).Any())
                    {
                        return "N";
                    }   
                    else
                    {
                        return "Y";
                    }
                }
                else
                {
                    return "N";
                }
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw Ex;
            }
            finally
            {
                dbContext.Dispose();
            }
 
        }

        public bool IsFinalPayment(int billId,int roadCode)
        {
            try
            {
                bool status = false;
                dbContext = new PMGSYEntities();
                var query = (from master in dbContext.ACC_BILL_MASTER
                             join details in dbContext.ACC_BILL_DETAILS
                                 on master.BILL_ID equals details.BILL_ID
                             where
                                 // master.MAST_CON_ID == contractorId
                                 // && details.IMS_AGREEMENT_CODE == agreementId  &&
                             details.IMS_PR_ROAD_CODE == roadCode &&
                             master.BILL_DATE <= (dbContext.ACC_BILL_MASTER.Where(m => m.BILL_ID == billId).Select(m => m.BILL_DATE).FirstOrDefault()) //new change done by Vikram 
                             select new
                             {
                                 details.FINAL_PAYMENT

                             });


                foreach (var item in query)
                {
                    if (item.FINAL_PAYMENT.HasValue)
                    {
                        if (item.FINAL_PAYMENT.Value)
                        {
                            status = true;
                            return true;
                        }
                    }
                }

                return status;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }

    public interface IOpeningBalanceDAL
    {
        String GetOBMasterIds(Int64 billId);
        OBMasterModel GetOBMasterById(String billId);
        ACC_BILL_MASTER GetOBMaster(Int64 billId);
        OBDetailsModel GetOBDetailsByTransId(Int64 billId, Int16 transId);
        Int64 AddOBMaster(OBMasterModel obMasterModel);
        String EditOBMaster(OBMasterModel obMasterModel);
        Array GetOBMasterList(ReceiptFilterModel objFilter, out long totalRecords);
        Array GetOBDetailsList(ReceiptFilterModel objFilter, out long totalRecords, out decimal AssetTotal, out decimal LibTotal, out decimal GrossAmount, out string Finalized);
        String DeleteOBMaster(String billId);
        String AddOBDetails(OBDetailsModel obDetailsModel);
        String EditOBDetails(OBDetailsModel obDetailsModel);
        String DeleteOBDetails(Int64 billId, Int16 transNo);
        String FinalizeOB(Int64 assetBillId, Int64 libBillId);
        String GetOBStatus(TransactionParams objParam);
        String ValidateOBDetails(OBDetailsModel obDetailsModel);
        String GetAssetAmountDetails(TransactionParams objParam);
        List<OBChart> GetAssetLiabilityDetails(TransactionParams objParam);
        String GetAccountStatus(TransactionParams objParam);
        bool IsFinalPayment(int billId, int roadCode);
    }
}