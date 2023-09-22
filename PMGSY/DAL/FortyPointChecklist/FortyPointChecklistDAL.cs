
/*----------------------------------------------------------------------------------------
 * Project Id    :

 * Project Name  : OMMAS-II

 * File Name     : FortyPointChecklistDAL.cs

 * Author        : Abhishek Kamble.
 
 * Creation Date : 20/Nov/2013

 * Desc          : This class is used as data access layer to perform Save,Edit,Delete and listing of Forty Point checklist Details.  
 
 * ---------------------------------------------------------------------------------------*/

using PMGSY.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Models.FortyPointCheckList;
using PMGSY.Extensions;
using System.Data.Entity;
using PMGSY.Common;
using System.Data.Entity.Infrastructure;
using System.Transactions;
using System.Data.Entity.Core;


namespace PMGSY.DAL.FortyPointChecklist
{
    public class FortyPointChecklistDAL:IFortyPointChecklistDAL
    {
        PMGSYEntities dbContext = null;

        #region Employment Information
            
            /// <summary>
        /// Get Data To display on the grid
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="stateCode"></param>
        /// <param name="adminCode"></param>
        /// <returns></returns>
            public Array EmploymentInformationDetailsDAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int adminCode)
            {
                dbContext = new PMGSYEntities();
                try
                {

                    List<USP_FPCL_EMPLOYMENT_INFORMATION_Result> employmentList = new List<USP_FPCL_EMPLOYMENT_INFORMATION_Result>();
                    employmentList = dbContext.USP_FPCL_EMPLOYMENT_INFORMATION(stateCode, adminCode).ToList<USP_FPCL_EMPLOYMENT_INFORMATION_Result>();

                    totalRecords = employmentList.Count();
                    if (sidx.Trim() != string.Empty)
                    {
                        if (sord.ToString().Trim() == "asc")
                        {
                            employmentList = employmentList.OrderBy(x => x.TEND_EMPLOYMENT_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                        }
                        else
                        {
                            employmentList = employmentList.OrderByDescending(x => x.TEND_EMPLOYMENT_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                        }
                    }
                    else
                    {
                        employmentList = employmentList.OrderBy(x => x.TEND_EMPLOYMENT_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }

                    return employmentList.Select(employmentDetails => new
                    {
                        id = employmentDetails.TEND_EMPLOYMENT_ID,
                        cell = new string[]{
                                 employmentDetails.MAST_QUALIFICATION_NAME.ToString(),
                                 employmentDetails.TEND_EMPLOYMENT_NUMBER.ToString(),
                                 URLEncrypt.EncryptParameters1(new string[] { "employmentId=" + employmentDetails.TEND_EMPLOYMENT_ID.ToString().Trim()}),
                                 URLEncrypt.EncryptParameters1(new string[] { "employmentId=" + employmentDetails.TEND_EMPLOYMENT_ID.ToString().Trim()})                             
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
            /// Populate Qualification
            /// </summary>
            /// <returns></returns>
            public List<SelectListItem> PopulateQualification()
            {
                try
                {
                    dbContext = new PMGSYEntities();
                    List<SelectListItem> lstQualification = new SelectList(dbContext.MASTER_QUALIFICATION, "MAST_QUALIFICATION_CODE", "MAST_QUALIFICATION_NAME").ToList();
                    lstQualification.Insert(0, (new SelectListItem { Text = "Select Qualification", Value = "0", Selected = true }));
                    return lstQualification;
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    return null;
                }
                finally {
                    if (dbContext != null)
                    {
                        dbContext.Dispose();
                    }
                }
            }

            /// <summary>
            /// Save Employment Details
            /// </summary>
            /// <param name="employmentViewModel"></param>
            /// <param name="message"></param>
            /// <returns></returns>        
            public bool AddEmploymentDetails(EmploymentInformationViewModel employmentViewModel,ref string message)
            {           
                try
                {       
                    //validation start
                    dbContext = new PMGSYEntities();
                    if(dbContext.MASTER_TEND_EMPLOYMENT.Where(m=>m.MAST_STATE_CODE==PMGSYSession.Current.StateCode && m.ADMIN_ND_CODE==PMGSYSession.Current.AdminNdCode && m.TEND_QUALIFICATION_ID==employmentViewModel.TEND_QUALIFICATION_ID).Any())
                    {
                        message = "Employment information details already exist.";
                        return false;
                    }                 
                    //validation start

                    MASTER_TEND_EMPLOYMENT masterTendEmployment = new MASTER_TEND_EMPLOYMENT();
                    masterTendEmployment.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                    masterTendEmployment.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                    masterTendEmployment.TEND_EMPLOYMENT_ID = GetMaxEmploymentID();
                    masterTendEmployment.MAST_CHECKLIST_POINTID = 18;
                    masterTendEmployment.TEND_QUALIFICATION_ID = employmentViewModel.TEND_QUALIFICATION_ID;
                    masterTendEmployment.TEND_EMPLOYMENT_NUMBER = employmentViewModel.TEND_EMPLOYMENT_NUMBER;
                    masterTendEmployment.USERID = PMGSYSession.Current.UserId;
                    masterTendEmployment.IPADD=HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    dbContext = new PMGSYEntities();
                    dbContext.MASTER_TEND_EMPLOYMENT.Add(masterTendEmployment);
                    dbContext.SaveChanges();                    
                    return true;
                }
                catch (OptimisticConcurrencyException ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    message = "An Error Occured While proccessing your request";
                    return false;
                }
                catch (UpdateException ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    message = "An Error Occured While proccessing your request";
                    return false;
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    message = "An error occured while processing your request.";
                    return false;
                }
                finally {
                    if (dbContext != null)
                    {
                        dbContext.Dispose();
                    }                
                }
            }
            
            /// <summary>
            /// Update Employment Details
            /// </summary>
            /// <param name="employmentViewModel"></param>
            /// <param name="message"></param>
            /// <returns></returns>
            public bool EditEmploymentDetails(EmploymentInformationViewModel employmentViewModel, ref string message)
            {

                try
                {                           
                    dbContext = new PMGSYEntities();
            
                    int employmentID;
                    Dictionary<string,string> decryptedParameters=null;
                    string [] encryptedParameters=null;
                    
                    encryptedParameters=employmentViewModel.EncryptedEmploymentId.Split('/');

                    if(!(encryptedParameters.Length==3))
                    {
                        return false;                    
                    }                

                    decryptedParameters=URLEncrypt.DecryptParameters1(new String[]{encryptedParameters[0],encryptedParameters[1],encryptedParameters[2]});
                    employmentID=Convert.ToInt32(decryptedParameters["employmentId"].ToString());

                     //validation start
                    if (dbContext.MASTER_TEND_EMPLOYMENT.Where(m => m.MAST_STATE_CODE == PMGSYSession.Current.StateCode && m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && m.TEND_QUALIFICATION_ID == employmentViewModel.TEND_QUALIFICATION_ID && m.TEND_EMPLOYMENT_ID!=employmentID).Any())
                    {
                        message = "Employment information details already exist.";
                        return false;
                    }
                    //validation start
                    
                    MASTER_TEND_EMPLOYMENT masterEmployment = dbContext.MASTER_TEND_EMPLOYMENT.Where(m=>m.MAST_STATE_CODE==PMGSYSession.Current.StateCode && m.ADMIN_ND_CODE==PMGSYSession.Current.AdminNdCode && m.TEND_EMPLOYMENT_ID==employmentID).FirstOrDefault();

                    masterEmployment.TEND_QUALIFICATION_ID = employmentViewModel.TEND_QUALIFICATION_ID;
                    masterEmployment.TEND_EMPLOYMENT_NUMBER = employmentViewModel.TEND_EMPLOYMENT_NUMBER;

                    masterEmployment.USERID = PMGSYSession.Current.UserId;
                    masterEmployment.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    dbContext.Entry(masterEmployment).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    message = "Employment Information Details updated successfully.";
                    return true;
                                        
                }
                catch (OptimisticConcurrencyException ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    message = "An Error Occured While proccessing your request";
                    return false;
                }
                catch (UpdateException ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    message = "An Error Occured While proccessing your request";
                    return false;
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    message = "An Error Occured While proccessing your request";
                    return false;
                }
                finally {                   
                    if (dbContext != null)
                    {
                        dbContext.Dispose();
                    }
                }
            }

            /// <summary>
            /// Get Employment Information Details
            /// </summary>
            /// <param name="employmentId"></param>
            /// <returns></returns>
            public EmploymentInformationViewModel GetEmploymentInformationDetails(int employmentId)
            {
                try
                {
                    dbContext = new PMGSYEntities();
                    EmploymentInformationViewModel employmentViewModel = new EmploymentInformationViewModel();

                    MASTER_TEND_EMPLOYMENT masterEmploymentModel = dbContext.MASTER_TEND_EMPLOYMENT.Where(m => m.MAST_STATE_CODE == PMGSYSession.Current.StateCode && m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && m.TEND_EMPLOYMENT_ID == employmentId).FirstOrDefault();

                    if (masterEmploymentModel != null)
                    {
                        employmentViewModel.EncryptedEmploymentId = URLEncrypt.EncryptParameters1(new string[] { "employmentId=" + employmentId.ToString().Trim()});
                        employmentViewModel.TEND_QUALIFICATION_ID = masterEmploymentModel.TEND_QUALIFICATION_ID;
                        employmentViewModel.TEND_EMPLOYMENT_NUMBER = masterEmploymentModel.TEND_EMPLOYMENT_NUMBER;
                    }
                    return employmentViewModel;
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    return null;
                }
                finally {                   
                    if (dbContext != null)
                    {
                        dbContext.Dispose();
                    }                       
                }
            }

            /// <summary>
            /// Delete Employment Information Details
            /// </summary>
            /// <param name="employmentId"></param>
            /// <param name="message"></param>
            /// <returns></returns>
            public bool DeleteEmploymentInformationDetails(int employmentId, ref string message)
            {

                using(TransactionScope ts=new TransactionScope()){

                    try
                    {
                        dbContext = new PMGSYEntities();

                        MASTER_TEND_EMPLOYMENT masterEmploymentModel = dbContext.MASTER_TEND_EMPLOYMENT.Where(m => m.MAST_STATE_CODE == PMGSYSession.Current.StateCode && m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && m.TEND_EMPLOYMENT_ID == employmentId).FirstOrDefault();

                        if(masterEmploymentModel==null)
                        {
                            message = "An error occured while processing your request.";
                            return false;
                        }


                        masterEmploymentModel.USERID = PMGSYSession.Current.UserId;
                        masterEmploymentModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        dbContext.Entry(masterEmploymentModel).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();

                        dbContext.MASTER_TEND_EMPLOYMENT.Remove(masterEmploymentModel);
                        dbContext.SaveChanges();

                        ts.Complete();
                        return true;
                    }
                    catch (DbUpdateException ex)
                    {
                        ts.Dispose();
                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                        message = "An error occured while processing your request.";
                        return false;
                    }
                    catch (Exception ex)
                    {
                        ts.Dispose();
                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                        message = "An error occured while processing your request.";
                        return false;
                    }
                    finally {
                        if (dbContext != null)
                        {
                            dbContext.Dispose();
                        }                
                    }
                }
            }           

            /// <summary>
            /// Get Max Employment ID
            /// </summary>
            /// <returns></returns>
            public int GetMaxEmploymentID()
            {
                dbContext = new PMGSYEntities();
                int maxId = 0;
                try
                {
                    if (dbContext.MASTER_TEND_EMPLOYMENT.Where(m => m.MAST_STATE_CODE == PMGSYSession.Current.StateCode && m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Any())
                    {
                        maxId = dbContext.MASTER_TEND_EMPLOYMENT.Where(m => m.MAST_STATE_CODE == PMGSYSession.Current.StateCode && m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Max(s => s.TEND_EMPLOYMENT_ID)+1;
                    }
                    else{
                        maxId=1;
                    }
                    return maxId;
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    return 0;
                }
                finally { 
                    if(dbContext!=null)
                    {
                        dbContext.Dispose();                    
                    }
                }
            }        

        #endregion Employment Information

        #region Tender Cost Information
               
            /// <summary>
            /// Get Tender Cost Information Details.
            /// </summary>
            /// <param name="page"></param>
            /// <param name="rows"></param>
            /// <param name="sidx"></param>
            /// <param name="sord"></param>
            /// <param name="totalRecords"></param>
            /// <param name="stateCode"></param>
            /// <param name="adminCode"></param>
            /// <returns></returns>
            public Array TenderCostInformationDetailsDAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int adminCode)
            {
                dbContext = new PMGSYEntities();
                try
                {
                    var tenderCostInformationList = dbContext.TEND_TENDER_PRICE.Where(m => m.MAST_STATE_CODE == stateCode && m.ADMIN_ND_CODE == adminCode).ToList();
                    
                    totalRecords = tenderCostInformationList.Count();
                    
                    if (sidx.Trim() != string.Empty)
                    {
                        if (sord.ToString().Trim() == "asc")
                        {
                            switch (sidx)
                            {
                                case "TEND_WORKS_COSTING_FROM":
                                    tenderCostInformationList = tenderCostInformationList.OrderBy(x => x.TEND_WORKS_COSTING_FROM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                    break;
                                case "TEND_WORKS_COSTING_TO":
                                    tenderCostInformationList = tenderCostInformationList.OrderBy(x => x.TEND_WORKS_COSTING_TO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                    break;
                                case "TEND_SALE_PRICE":
                                    tenderCostInformationList = tenderCostInformationList.OrderBy(x => x.TEND_SALE_PRICE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                    break;
                                default:
                                    tenderCostInformationList = tenderCostInformationList.OrderBy(x => x.TEND_PRICE_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                    break;
                            }                            
                        }
                        else
                        {
                            switch (sidx)
                            {
                                case "TEND_WORKS_COSTING_FROM":
                                    tenderCostInformationList = tenderCostInformationList.OrderByDescending(x => x.TEND_WORKS_COSTING_FROM).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                    break;
                                case "TEND_WORKS_COSTING_TO":
                                    tenderCostInformationList = tenderCostInformationList.OrderByDescending(x => x.TEND_WORKS_COSTING_TO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                    break;
                                case "TEND_SALE_PRICE":
                                    tenderCostInformationList = tenderCostInformationList.OrderByDescending(x => x.TEND_SALE_PRICE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                    break;
                                default:
                                    tenderCostInformationList = tenderCostInformationList.OrderByDescending(x => x.TEND_PRICE_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                    break;
                            }
                        }
                    }
                    else
                    {
                        tenderCostInformationList = tenderCostInformationList.OrderByDescending(x => x.TEND_PRICE_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }

                    return tenderCostInformationList.Select(TenderCostInformationDetails => new
                    {
                        cell = new string[]{
                                TenderCostInformationDetails.TEND_WORKS_COSTING_FROM.ToString(),
                                TenderCostInformationDetails.TEND_WORKS_COSTING_TO.ToString(),
                                TenderCostInformationDetails.TEND_SALE_PRICE,
                                 URLEncrypt.EncryptParameters1(new string[] { "priceId=" + TenderCostInformationDetails.TEND_PRICE_ID.ToString().Trim()}),
                                 URLEncrypt.EncryptParameters1(new string[] { "priceId=" + TenderCostInformationDetails.TEND_PRICE_ID.ToString().Trim()})                             
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
            /// Save Tender Cost Information Details
            /// </summary>
            /// <param name="tenderCostInformationViewModel"></param>
            /// <param name="message"></param>
            /// <returns></returns>                
            public bool AddTenderCostInformationDetails(TenderCostInformationViewModel tenderCostInformationViewModel, ref string message)
            {
                try
                {
                    TEND_TENDER_PRICE tenderPriceModel = new TEND_TENDER_PRICE();

                    //validation for work costing from & work costing to start
                    dbContext = new PMGSYEntities();
                    if (dbContext.TEND_TENDER_PRICE.Where(m => m.MAST_STATE_CODE == PMGSYSession.Current.StateCode && m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && m.TEND_WORKS_COSTING_FROM == tenderCostInformationViewModel.TEND_WORKS_COSTING_FROM && m.TEND_WORKS_COSTING_TO == tenderCostInformationViewModel.TEND_WORKS_COSTING_TO).Any())
                    {
                        message = "Tender cost details for this cost is already exist.";
                        return false;
                    }
                    else {
                        List<TEND_TENDER_PRICE> lstPriceMaster = dbContext.TEND_TENDER_PRICE.Where(m => m.MAST_STATE_CODE == PMGSYSession.Current.StateCode && m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).ToList();

                        foreach (var item in lstPriceMaster)
                        {
                            if (item.TEND_WORKS_COSTING_FROM < tenderCostInformationViewModel.TEND_WORKS_COSTING_FROM && tenderCostInformationViewModel.TEND_WORKS_COSTING_FROM < item.TEND_WORKS_COSTING_TO)
                            {
                                message = "Entered work costing from or work costing to is already exist.";
                                return false;
                            }

                            if (item.TEND_WORKS_COSTING_FROM < tenderCostInformationViewModel.TEND_WORKS_COSTING_TO && tenderCostInformationViewModel.TEND_WORKS_COSTING_TO < item.TEND_WORKS_COSTING_TO)
                            {
                                message = "Entered work costing from or work costing to is already exist.";
                                return false;
                            }
                            if (tenderCostInformationViewModel.TEND_WORKS_COSTING_FROM < item.TEND_WORKS_COSTING_FROM && item.TEND_WORKS_COSTING_FROM < tenderCostInformationViewModel.TEND_WORKS_COSTING_TO)
                            {
                                message = "Entered work costing from or work costing to is already exist.";
                                return false;
                            }

                            if (tenderCostInformationViewModel.TEND_WORKS_COSTING_FROM < item.TEND_WORKS_COSTING_TO && item.TEND_WORKS_COSTING_TO < tenderCostInformationViewModel.TEND_WORKS_COSTING_TO)
                            {
                                message = "Entered work costing from or work costing to is already exist.";
                                return false;
                            }  
                        }
                    }

                    //validation for work costing from & work costing to end

                    tenderPriceModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                    tenderPriceModel.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                    tenderPriceModel.TEND_PRICE_ID = GetMaxPriceID();
                    tenderPriceModel.MAST_CHECKLIST_POINTID = 11;
                    tenderPriceModel.TEND_WORKS_COSTING_FROM = tenderCostInformationViewModel.TEND_WORKS_COSTING_FROM;
                    tenderPriceModel.TEND_WORKS_COSTING_TO = tenderCostInformationViewModel.TEND_WORKS_COSTING_TO;
                    tenderPriceModel.TEND_SALE_PRICE = tenderCostInformationViewModel.TEND_SALE_PRICE;

                    tenderPriceModel.USERID = PMGSYSession.Current.UserId;
                    tenderPriceModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    dbContext = new PMGSYEntities();
                    dbContext.TEND_TENDER_PRICE.Add(tenderPriceModel);
                    dbContext.SaveChanges();
                    return true;
                }
                catch (OptimisticConcurrencyException ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    message = "An Error Occured While proccessing your request";
                    return false;
                }
                catch (UpdateException ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    message = "An Error Occured While proccessing your request";
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
           ///  Update Tender Cost Information Details
           /// </summary>
           /// <param name="tenderCostInformationViewModel"></param>
           /// <param name="message"></param>
           /// <returns></returns>
            public bool EditCostInformationDetails(TenderCostInformationViewModel tenderCostInformationViewModel, ref string message)
            {

                try
                {
                    dbContext = new PMGSYEntities();

                    int priceID;
                    Dictionary<string, string> decryptedParameters = null;
                    string[] encryptedParameters = null;
                    encryptedParameters = tenderCostInformationViewModel.EncryptedTenderPriceId.Split('/');

                    if (!(encryptedParameters.Length == 3))
                    {
                        return false;
                    }

                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    priceID = Convert.ToInt32(decryptedParameters["priceId"].ToString());

                    //validation for work costing from & work costing to end

                    if (dbContext.TEND_TENDER_PRICE.Where(m => m.MAST_STATE_CODE == PMGSYSession.Current.StateCode && m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && m.TEND_WORKS_COSTING_FROM == tenderCostInformationViewModel.TEND_WORKS_COSTING_FROM && m.TEND_WORKS_COSTING_TO == tenderCostInformationViewModel.TEND_WORKS_COSTING_TO && m.TEND_PRICE_ID!=priceID).Any())
                    {
                        message = "Tender cost details for this cost are already exist.";
                        return false;
                    }
                    else
                    {
                        List<TEND_TENDER_PRICE> lstPriceMaster = dbContext.TEND_TENDER_PRICE.Where(m => m.MAST_STATE_CODE == PMGSYSession.Current.StateCode && m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && m.TEND_PRICE_ID!=priceID).ToList();

                        foreach (var item in lstPriceMaster)
                        {
                            if (item.TEND_WORKS_COSTING_FROM < tenderCostInformationViewModel.TEND_WORKS_COSTING_FROM && tenderCostInformationViewModel.TEND_WORKS_COSTING_FROM < item.TEND_WORKS_COSTING_TO)
                            {
                                message = "Entered work costing from or work costing to is already exist.";
                                return false;
                            }

                            if (item.TEND_WORKS_COSTING_FROM < tenderCostInformationViewModel.TEND_WORKS_COSTING_TO && tenderCostInformationViewModel.TEND_WORKS_COSTING_TO < item.TEND_WORKS_COSTING_TO)
                            {
                                message = "Entered work costing from or work costing to is already exist.";
                                return false;
                            }
                            if (tenderCostInformationViewModel.TEND_WORKS_COSTING_FROM < item.TEND_WORKS_COSTING_FROM && item.TEND_WORKS_COSTING_FROM < tenderCostInformationViewModel.TEND_WORKS_COSTING_TO)
                            {
                                message = "Entered work costing from or work costing to is already exist.";
                                return false;
                            }

                            if (tenderCostInformationViewModel.TEND_WORKS_COSTING_FROM < item.TEND_WORKS_COSTING_TO && item.TEND_WORKS_COSTING_TO < tenderCostInformationViewModel.TEND_WORKS_COSTING_TO)
                            {
                                message = "Entered work costing from or work costing to is already exist.";
                                return false;
                            }
                        }
                    }

                    //validation for work costing from & work costing to end

                    TEND_TENDER_PRICE tenderPriceModel = dbContext.TEND_TENDER_PRICE.Where(m => m.MAST_STATE_CODE == PMGSYSession.Current.StateCode && m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && m.TEND_PRICE_ID == priceID).FirstOrDefault();

                    tenderPriceModel.TEND_WORKS_COSTING_FROM = tenderCostInformationViewModel.TEND_WORKS_COSTING_FROM;
                    tenderPriceModel.TEND_WORKS_COSTING_TO = tenderCostInformationViewModel.TEND_WORKS_COSTING_TO;
                    tenderPriceModel.TEND_SALE_PRICE = tenderCostInformationViewModel.TEND_SALE_PRICE;

                    tenderPriceModel.USERID = PMGSYSession.Current.UserId;
                    tenderPriceModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    dbContext.Entry(tenderPriceModel).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    message = "Tender Cost Details updated successfully.";
                    return true;

                }
                catch (OptimisticConcurrencyException ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    message = "An Error Occured While proccessing your request";
                    return false;
                }
                catch (UpdateException ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    message = "An Error Occured While proccessing your request";
                    return false;
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    message = "An Error Occured While proccessing your request";
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
            /// Get Tender Cost Information Details
            /// </summary>
            /// <param name="priceId"></param>
            /// <returns></returns>
            public TenderCostInformationViewModel GetTenderCostInformationDetails(int priceId)
            {
                try
                {
                    dbContext = new PMGSYEntities();
                    TenderCostInformationViewModel tenderCostInformationViewModel = new TenderCostInformationViewModel();
                    TEND_TENDER_PRICE tenderPriceModel = dbContext.TEND_TENDER_PRICE.Where(m => m.MAST_STATE_CODE == PMGSYSession.Current.StateCode && m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && m.TEND_PRICE_ID == priceId).FirstOrDefault();

                    if (tenderPriceModel != null)
                    {
                        tenderCostInformationViewModel.EncryptedTenderPriceId = URLEncrypt.EncryptParameters1(new string[] { "priceId=" + priceId.ToString().Trim() });
                        tenderCostInformationViewModel.TEND_WORKS_COSTING_FROM = tenderPriceModel.TEND_WORKS_COSTING_FROM;
                        tenderCostInformationViewModel.TEND_WORKS_COSTING_TO = tenderPriceModel.TEND_WORKS_COSTING_TO;
                        tenderCostInformationViewModel.TEND_SALE_PRICE = tenderPriceModel.TEND_SALE_PRICE;
                    }
                    return tenderCostInformationViewModel;
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
            /// Delete Tender Cost Information Details
            /// </summary>
            /// <param name="priceId"></param>
            /// <param name="message"></param>
            /// <returns></returns>
            public bool DeleteTenderCostInformationDetails(int priceId, ref string message)
            {
                using(TransactionScope ts=new TransactionScope())
                {                                               
                    try
                    {
                        dbContext = new PMGSYEntities();

                        TEND_TENDER_PRICE tenderPriceModel = dbContext.TEND_TENDER_PRICE.Where(m => m.MAST_STATE_CODE == PMGSYSession.Current.StateCode && m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && m.TEND_PRICE_ID == priceId).FirstOrDefault();

                        if (tenderPriceModel== null)
                        {
                            message = "An error occured while processing your request.";
                            return false;
                        }

                        tenderPriceModel.USERID = PMGSYSession.Current.UserId;
                        tenderPriceModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        dbContext.Entry(tenderPriceModel).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();

                        dbContext.TEND_TENDER_PRICE.Remove(tenderPriceModel);
                        dbContext.SaveChanges();

                        ts.Complete();
                        return true;
                    }
                    catch (DbUpdateException ex)
                    {
                        ts.Dispose();
                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                        message = "An error occured while processing your request.";
                        return false;
                    }
                    catch (Exception ex)
                    {
                        ts.Dispose();
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
            }

            /// <summary>
            /// Get Max Employment ID
            /// </summary>
            /// <returns></returns>
            public int GetMaxPriceID()
            {
                dbContext = new PMGSYEntities();
                int maxId = 0;
                try
                {
                    if (dbContext.TEND_TENDER_PRICE.Where(m => m.MAST_STATE_CODE == PMGSYSession.Current.StateCode && m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Any())
                    {
                        maxId = dbContext.TEND_TENDER_PRICE.Where(m => m.MAST_STATE_CODE == PMGSYSession.Current.StateCode && m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Max(s => s.TEND_PRICE_ID) + 1;
                    }
                    else
                    {
                        maxId = 1;
                    }
                    return maxId;
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
            }                    

        #endregion Tender Cost Information

        #region Tender Equipment

            /// <summary>
           ///  Get Tender Equipment Details.
           /// </summary>
           /// <param name="page"></param>
           /// <param name="rows"></param>
           /// <param name="sidx"></param>
           /// <param name="sord"></param>
           /// <param name="totalRecords"></param>
           /// <param name="stateCode"></param>
           /// <param name="adminCode"></param>
           /// <param name="equipmentFlag"></param>
           /// <returns></returns>
            public Array ListTenderEquipmentDetails(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int adminCode)
            {
                dbContext = new PMGSYEntities();
                try
                {
                    var tenderEquipmentList = dbContext.MASTER_TEND_EQUIPMENT.Where(m => m.MAST_STATE_CODE == stateCode && m.ADMIN_ND_CODE == adminCode).OrderBy(o=>o.TEND_EQUIPMENT_FLAG).ToList();

                    totalRecords = tenderEquipmentList.Count();


                    if (sidx.Trim() != string.Empty)
                    {
                        if (sord.ToString().Trim() == "asc")
                        {
                            switch (sidx)
                            {

                                case "TEND_EQUIPMENT_FLAG":
                                    tenderEquipmentList = tenderEquipmentList.OrderBy(x => x.TEND_EQUIPMENT_FLAG).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                    break;
                                case "TEND_EQUIPMENT_TYPE":
                                    tenderEquipmentList = tenderEquipmentList.OrderBy(x => x.TEND_EQUIPMENT_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                    break;
                                case "TEND_EQUIPMENT_NUMBERS":
                                    tenderEquipmentList = tenderEquipmentList.OrderBy(x => x.TEND_EQUIPMENT_NUMBERS).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                    break;
                                default:
                                    tenderEquipmentList = tenderEquipmentList.OrderBy(x => x.TEND_EQUIPMENT_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                    break;
                            }
                        }
                        else
                        {
                            switch (sidx)
                            {
                                case "TEND_EQUIPMENT_FLAG":
                                    tenderEquipmentList = tenderEquipmentList.OrderByDescending(x => x.TEND_EQUIPMENT_FLAG).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                    break;
                                case "TEND_EQUIPMENT_TYPE":
                                    tenderEquipmentList = tenderEquipmentList.OrderByDescending(x => x.TEND_EQUIPMENT_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                    break;
                                case "TEND_EQUIPMENT_NUMBERS":
                                    tenderEquipmentList = tenderEquipmentList.OrderByDescending(x => x.TEND_EQUIPMENT_NUMBERS).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                    break;
                                default:
                                    tenderEquipmentList = tenderEquipmentList.OrderByDescending(x => x.TEND_EQUIPMENT_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                    break;
                            }
                        }
                    }
                    else
                    {
                        tenderEquipmentList = tenderEquipmentList.OrderByDescending(x => x.TEND_EQUIPMENT_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }


                    return tenderEquipmentList.Select(TenderEquipmentDetails => new
                    {
                        cell = new string[]{
                                TenderEquipmentDetails.TEND_EQUIPMENT_FLAG=="C"?"Construction":"Lab",
                                TenderEquipmentDetails.TEND_EQUIPMENT_TYPE,
                                TenderEquipmentDetails.TEND_EQUIPMENT_NUMBERS.ToString(),
                                URLEncrypt.EncryptParameters1(new string[] { "equipmentId=" + TenderEquipmentDetails.TEND_EQUIPMENT_ID.ToString().Trim()}),
                                URLEncrypt.EncryptParameters1(new string[] { "equipmentId=" + TenderEquipmentDetails.TEND_EQUIPMENT_ID.ToString().Trim()})                             
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
            /// Save Tender Equipment Details
            /// </summary>
            /// <param name="tenderequipmentViewModel"></param>
            /// <param name="message"></param>
            /// <returns></returns>               
            public bool AddTenderEquipmentDetails(TenderEquipmentViewModel tenderequipmentViewModel, ref string message)
            {
                try
                {
                    //validation start

                    dbContext = new PMGSYEntities();
                    if(dbContext.MASTER_TEND_EQUIPMENT.Where(m=>m.MAST_STATE_CODE==PMGSYSession.Current.StateCode && m.ADMIN_ND_CODE==PMGSYSession.Current.AdminNdCode && m.TEND_EQUIPMENT_FLAG==tenderequipmentViewModel.TEND_EQUIPMENT_FLAG && m.TEND_EQUIPMENT_TYPE==tenderequipmentViewModel.TEND_EQUIPMENT_TYPE).Any())
                    {

                        if(tenderequipmentViewModel.TEND_EQUIPMENT_FLAG=="C")
                        {
                            message = "Construction Equipment details are already exist.";
                        }
                        else if (tenderequipmentViewModel.TEND_EQUIPMENT_FLAG == "L")
                        {
                            message = "Lab Equipment details are already exist.";
                        }
                        
                        return false;
                    }                           
                    //validation end

                    MASTER_TEND_EQUIPMENT tenderEquipmentModel = new MASTER_TEND_EQUIPMENT();

                    tenderEquipmentModel.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                    tenderEquipmentModel.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                    tenderEquipmentModel.TEND_EQUIPMENT_ID = GetMaxEquipmentID();
                    tenderEquipmentModel.TEND_CHECKLIST_POINTID = 17;
                    tenderEquipmentModel.TEND_EQUIPMENT_TYPE = tenderequipmentViewModel.TEND_EQUIPMENT_TYPE;
                    tenderEquipmentModel.TEND_EQUIPMENT_NUMBERS = tenderequipmentViewModel.TEND_EQUIPMENT_NUMBERS;
                    tenderEquipmentModel.TEND_EQUIPMENT_FLAG = tenderequipmentViewModel.TEND_EQUIPMENT_FLAG;
                    dbContext = new PMGSYEntities();

                    tenderEquipmentModel.USERID = PMGSYSession.Current.UserId;
                    tenderEquipmentModel.IPADD=HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    dbContext.MASTER_TEND_EQUIPMENT.Add(tenderEquipmentModel);
                    dbContext.SaveChanges();
                    return true;
                }
                catch (OptimisticConcurrencyException ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    message = "An Error Occured While proccessing your request";
                    return false;
                }
                catch (UpdateException ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    message = "An Error Occured While proccessing your request";
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
            /// Update Tender Equipment Details
            /// </summary>
            /// <param name="tenderEquipmentViewModel"></param>
            /// <param name="message"></param>
            /// <returns></returns>
            public bool EditTenderEquipmentDetails(TenderEquipmentViewModel tenderEquipmentViewModel, ref string message)
            {
                try
                {
                    dbContext = new PMGSYEntities();

                    int EquipmentID;
                    Dictionary<string, string> decryptedParameters = null;
                    string[] encryptedParameters = null;
                    encryptedParameters = tenderEquipmentViewModel.EncryptedEquipmentCode.Split('/');

                    if (!(encryptedParameters.Length == 3))
                    {
                        return false;
                    }

                    decryptedParameters = URLEncrypt.DecryptParameters1(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                    EquipmentID = Convert.ToInt32(decryptedParameters["equipmentId"].ToString());

                    //validation end
                    if (dbContext.MASTER_TEND_EQUIPMENT.Where(m => m.MAST_STATE_CODE == PMGSYSession.Current.StateCode && m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && m.TEND_EQUIPMENT_FLAG == tenderEquipmentViewModel.TEND_EQUIPMENT_FLAG && m.TEND_EQUIPMENT_TYPE == tenderEquipmentViewModel.TEND_EQUIPMENT_TYPE && m.TEND_EQUIPMENT_ID!=EquipmentID).Any())
                    {
                        if (tenderEquipmentViewModel.TEND_EQUIPMENT_FLAG == "C")
                        {
                            message = "Construction Equipment details are already exist.";
                        }
                        else if (tenderEquipmentViewModel.TEND_EQUIPMENT_FLAG == "L")
                        {
                            message = "Lab Equipment details are already exist.";
                        }                    
                        return false;
                    }
                    //validation end
                    
                    MASTER_TEND_EQUIPMENT tenderEquipmentModel = dbContext.MASTER_TEND_EQUIPMENT.Where(m => m.MAST_STATE_CODE == PMGSYSession.Current.StateCode && m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && m.TEND_EQUIPMENT_ID == EquipmentID).FirstOrDefault();

                    tenderEquipmentModel.TEND_EQUIPMENT_TYPE = tenderEquipmentViewModel.TEND_EQUIPMENT_TYPE;
                    tenderEquipmentModel.TEND_EQUIPMENT_NUMBERS = tenderEquipmentViewModel.TEND_EQUIPMENT_NUMBERS;
                    tenderEquipmentModel.USERID = PMGSYSession.Current.UserId;
                    tenderEquipmentModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    dbContext.Entry(tenderEquipmentModel).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    message = "Equipment Details updated successfully.";
                    return true;

                }
                catch (OptimisticConcurrencyException ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    message = "An Error Occured While proccessing your request";
                    return false;
                }
                catch (UpdateException ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    message = "An Error Occured While proccessing your request";
                    return false;
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    message = "An Error Occured While proccessing your request";
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
            /// Get Tender Equipment Details
            /// </summary>
            /// <param name="equipmentId"></param>
            /// <returns></returns>
            public TenderEquipmentViewModel GetTenderEquipmentDetails(int equipmentId)
            {
                try
                {
                    dbContext = new PMGSYEntities();
                    TenderEquipmentViewModel tenderEquipmentViewModel = new TenderEquipmentViewModel();
                    MASTER_TEND_EQUIPMENT tenderEquipmentModel = dbContext.MASTER_TEND_EQUIPMENT.Where(m => m.MAST_STATE_CODE == PMGSYSession.Current.StateCode && m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && m.TEND_EQUIPMENT_ID ==equipmentId).FirstOrDefault();

                    if (tenderEquipmentModel != null)
                    {
                        tenderEquipmentViewModel.EncryptedEquipmentCode= URLEncrypt.EncryptParameters1(new string[] { "equipmentId=" + equipmentId.ToString().Trim() });
                        tenderEquipmentViewModel.TEND_EQUIPMENT_TYPE = tenderEquipmentModel.TEND_EQUIPMENT_TYPE;
                        tenderEquipmentViewModel.TEND_EQUIPMENT_NUMBERS = tenderEquipmentModel.TEND_EQUIPMENT_NUMBERS;
                        tenderEquipmentViewModel.TEND_EQUIPMENT_FLAG = tenderEquipmentModel.TEND_EQUIPMENT_FLAG;
                    }
                    return tenderEquipmentViewModel;
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
            /// Delete Tender Equipment Details
           /// </summary>
           /// <param name="equipmentId"></param>
           /// <param name="message"></param>
           /// <returns></returns>
            public bool DeleteTenderEquipmentDetails(int equipmentId, ref string message)
            {
                using(TransactionScope ts=new TransactionScope())
                {

                try
                {
                    dbContext = new PMGSYEntities();

                    MASTER_TEND_EQUIPMENT tenderEquipmentModel = dbContext.MASTER_TEND_EQUIPMENT.Where(m => m.MAST_STATE_CODE == PMGSYSession.Current.StateCode && m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && m.TEND_EQUIPMENT_ID == equipmentId).FirstOrDefault();

                    if (tenderEquipmentModel == null)
                    {
                        message = "An error occured while processing your request.";
                        return false;
                    }
                    tenderEquipmentModel.USERID = PMGSYSession.Current.UserId;
                    tenderEquipmentModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    dbContext.Entry(tenderEquipmentModel).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    dbContext.MASTER_TEND_EQUIPMENT.Remove(tenderEquipmentModel);
                    dbContext.SaveChanges();

                    ts.Complete();
                    return true;
                }
                catch (DbUpdateException ex)
                {
                    ts.Dispose();
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    message = "An error occured while processing your request.";
                    return false;
                }
                catch (Exception ex)
                {
                    ts.Dispose();
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
                }//end of transaction scope
            }

            /// <summary>
            /// Get Max Employment ID
            /// </summary>
            /// <returns></returns>
            public int GetMaxEquipmentID()
            {
                dbContext = new PMGSYEntities();
                int maxId = 0;
                try
                {
                    if (dbContext.MASTER_TEND_EQUIPMENT.Where(m => m.MAST_STATE_CODE == PMGSYSession.Current.StateCode && m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Any())
                    {
                        maxId = dbContext.MASTER_TEND_EQUIPMENT.Where(m => m.MAST_STATE_CODE == PMGSYSession.Current.StateCode && m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).Max(s => s.TEND_EQUIPMENT_ID) + 1;
                    }
                    else
                    {
                        maxId = 1;
                    }
                    return maxId;
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
            }


            public List<SelectListItem> PopulateEquipmentType()
            {
                List<SelectListItem> lstEquipments = new List<SelectListItem>();
                lstEquipments.Add(new SelectListItem { Text = "Construction", Value = "C" });
                lstEquipments.Add(new SelectListItem { Text = "Lab", Value = "L" });
                return lstEquipments;
            }
                

                     
        #endregion Tender Equipment    

        #region Forty Point Check List

            /// <summary>
            /// Get Forty Point Checklist data
            /// </summary>
            /// <param name="page"></param>
            /// <param name="rows"></param>
            /// <param name="sidx"></param>
            /// <param name="sord"></param>
            /// <param name="totalRecords"></param>
            /// <param name="stateCode"></param>
            /// <param name="adminNdCode"></param>
            /// <returns></returns>
            public Array ListFortyPointCheckListDetails(int? page,int? rows,string sidx,string sord,out Int32 totalRecords, int stateCode,int adminNdCode)
            {
                dbContext = new PMGSYEntities();
                try
                {
                    var FortyPointcheckList = (from masterCheckListPoint in dbContext.MASTER_CHECKLIST_POINTS
                                               join tendCheckListDetails in dbContext.TEND_CHECKLIST_DETAIL
                                               on masterCheckListPoint.MAST_CHECKLIST_POINTID equals tendCheckListDetails.MAST_CHECKLIST_POINTID into CheckListdata
                                               from data in CheckListdata.DefaultIfEmpty()
                                               where 
                                                   (data.MAST_STATE_CODE == null?1:data.MAST_STATE_CODE)==(data.MAST_STATE_CODE==null?1:stateCode) &&
                                                   (data.ADMIN_ND_CODE == null?1:data.ADMIN_ND_CODE)==(data.MAST_STATE_CODE==null?1:adminNdCode) &&                                                   
                                                   masterCheckListPoint.MAST_CHECKLIST_ACTIVE=="Y"
                                               select new{
                                                masterCheckListPoint.MAST_CHECKLIST_POINTID,
                                                masterCheckListPoint.MAST_CHECKLIST_ISSUES,
                                                data.MAST_ACTION_TAKEN
                                               }
                                            ).ToList();
    
                    totalRecords = FortyPointcheckList.Count();

                    return FortyPointcheckList.Select(checkListDetails => new
                    {
                        id = checkListDetails.MAST_CHECKLIST_POINTID,
                        cell = new string[]{
                        checkListDetails.MAST_CHECKLIST_POINTID.ToString(),
                        checkListDetails.MAST_CHECKLIST_ISSUES,
                        checkListDetails.MAST_ACTION_TAKEN,
                        string.Empty,
                       dbContext.TEND_CHECKLIST_DETAIL.Where(m=>m.MAST_STATE_CODE==PMGSYSession.Current.StateCode && m.ADMIN_ND_CODE==PMGSYSession.Current.AdminNdCode && m.MAST_CHECKLIST_POINTID==checkListDetails.MAST_CHECKLIST_POINTID).Any()?
                       URLEncrypt.EncryptParameters1(new string[]{"checkListId="+checkListDetails.MAST_CHECKLIST_POINTID.ToString().Trim()}):string.Empty,
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
            /// Add/ Edit Forty Point Checklist Details
            /// </summary>
            /// <param name="fortyPointCheckListViewModel"></param>
            /// <param name="message"></param>
            /// <returns></returns>
            public bool AddEditFortyPointCheckList(FortyPointCheckListViewModel fortyPointCheckListViewModel, ref string message)
            {
                try
                {
                    dbContext = new PMGSYEntities();
                    TEND_CHECKLIST_DETAIL checkListDetailsModel = new TEND_CHECKLIST_DETAIL();                    
                    
                    if(dbContext.TEND_CHECKLIST_DETAIL.Where(m=>m.MAST_STATE_CODE==PMGSYSession.Current.StateCode && m.ADMIN_ND_CODE==PMGSYSession.Current.AdminNdCode && m.MAST_CHECKLIST_POINTID==fortyPointCheckListViewModel.MAST_CHECKLIST_POINTID).Any())
                    {
                        checkListDetailsModel = dbContext.TEND_CHECKLIST_DETAIL.Where(m => m.MAST_STATE_CODE == PMGSYSession.Current.StateCode && m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && m.MAST_CHECKLIST_POINTID == fortyPointCheckListViewModel.MAST_CHECKLIST_POINTID).FirstOrDefault();
                        checkListDetailsModel.MAST_ACTION_TAKEN = fortyPointCheckListViewModel.MAST_ACTION_TAKEN;

                        checkListDetailsModel.USERID=PMGSYSession.Current.UserId;
                        checkListDetailsModel.IPADD=HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];                                               
                        
                        dbContext.Entry(checkListDetailsModel).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                        message = "Reply For Forty Point Check List details updated successfully.";
                    
                    }else{
                        checkListDetailsModel.MAST_STATE_CODE = fortyPointCheckListViewModel.MAST_STATE_CODE;
                        checkListDetailsModel.ADMIN_ND_CODE = fortyPointCheckListViewModel.ADMIN_ND_CODE;
                        checkListDetailsModel.ADMIN_ND_CODE = fortyPointCheckListViewModel.ADMIN_ND_CODE;
                        checkListDetailsModel.MAST_CHECKLIST_POINTID = fortyPointCheckListViewModel.MAST_CHECKLIST_POINTID;
                        checkListDetailsModel.MAST_ACTION_TAKEN = fortyPointCheckListViewModel.MAST_ACTION_TAKEN;

                        checkListDetailsModel.USERID = PMGSYSession.Current.UserId;
                        checkListDetailsModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];                                               
                        
                        dbContext.TEND_CHECKLIST_DETAIL.Add(checkListDetailsModel);
                        dbContext.SaveChanges();
                        message = "Reply For Forty Point Check List details added successfully.";
                    }
                    
                    return true;
                }
                catch (OptimisticConcurrencyException ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    message = "An Error Occured While proccessing your request";
                    return false;
                }
                catch (UpdateException ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    message = "An Error Occured While proccessing your request";
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
            /// Delete Check list point details
            /// </summary>
            /// <param name="checkListPointId"></param>
            /// <param name="message"></param>
            /// <returns></returns>
            public bool DeleteFortyPointCheckListDetails(int checkListPointId, ref string message)
            {
                try
                {
                    dbContext = new PMGSYEntities();

                    TEND_CHECKLIST_DETAIL checkListModel = dbContext.TEND_CHECKLIST_DETAIL.Where(m => m.MAST_STATE_CODE == PMGSYSession.Current.StateCode && m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && m.MAST_CHECKLIST_POINTID== checkListPointId).FirstOrDefault();

                    if (checkListModel == null)
                    {
                        message = "Check List Point details not deleted bcause details are not exist.";
                        return false;
                    }                       
                    checkListModel.USERID = PMGSYSession.Current.UserId;
                    checkListModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    dbContext.Entry(checkListModel).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    dbContext.TEND_CHECKLIST_DETAIL.Remove(checkListModel);
                    dbContext.SaveChanges();
                    message = "Check List Point details deleted successfully.";
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


            public Array ListConstructionLabEquipmentDetails(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int adminCode)
            {
                dbContext = new PMGSYEntities();
                try
                {
                    var tenderEquipmentList = dbContext.MASTER_TEND_EQUIPMENT.Where(m => m.MAST_STATE_CODE == stateCode && m.ADMIN_ND_CODE == adminCode).ToList();

                    totalRecords = tenderEquipmentList.Count();

                    if (sidx.Trim() != string.Empty)
                    {
                        if (sord.ToString().Trim() == "asc")
                        {
                            switch (sidx)
                            {
                                case "TEND_EQUIPMENT_TYPE":
                                    tenderEquipmentList = tenderEquipmentList.OrderBy(x => x.TEND_EQUIPMENT_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                    break;
                                case "TEND_EQUIPMENT_NUMBERS":
                                    tenderEquipmentList = tenderEquipmentList.OrderBy(x => x.TEND_EQUIPMENT_NUMBERS).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                    break;
                                default:
                                    tenderEquipmentList = tenderEquipmentList.OrderBy(x => x.TEND_EQUIPMENT_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                    break;
                            }
                        }
                        else
                        {
                            switch (sidx)
                            {
                                case "TEND_EQUIPMENT_TYPE":
                                    tenderEquipmentList = tenderEquipmentList.OrderByDescending(x => x.TEND_EQUIPMENT_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                    break;
                                case "TEND_EQUIPMENT_NUMBERS":
                                    tenderEquipmentList = tenderEquipmentList.OrderByDescending(x => x.TEND_EQUIPMENT_NUMBERS).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                    break;
                                default:
                                    tenderEquipmentList = tenderEquipmentList.OrderByDescending(x => x.TEND_EQUIPMENT_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                                    break;
                            }
                        }
                    }
                    else
                    {
                        tenderEquipmentList = tenderEquipmentList.OrderByDescending(x => x.TEND_EQUIPMENT_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }

                    return tenderEquipmentList.Select(TenderEquipmentDetails => new
                    {
                        cell = new string[]{
                                TenderEquipmentDetails.TEND_EQUIPMENT_TYPE,
                                TenderEquipmentDetails.TEND_EQUIPMENT_NUMBERS.ToString(),
                                URLEncrypt.EncryptParameters1(new string[] { "equipmentId=" + TenderEquipmentDetails.TEND_EQUIPMENT_ID.ToString().Trim()}),
                                URLEncrypt.EncryptParameters1(new string[] { "equipmentId=" + TenderEquipmentDetails.TEND_EQUIPMENT_ID.ToString().Trim()})                             
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

        #endregion Forty Point Check List
    }
    public interface IFortyPointChecklistDAL
    {
        #region Employment Information
        
            Array EmploymentInformationDetailsDAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int adminCode);
            List<SelectListItem> PopulateQualification();
            bool AddEmploymentDetails(EmploymentInformationViewModel employmentViewModel, ref string message);
            bool EditEmploymentDetails(EmploymentInformationViewModel employmentViewModel, ref string message);
            EmploymentInformationViewModel GetEmploymentInformationDetails(int employmentId);                  
            bool DeleteEmploymentInformationDetails(int employmentId, ref string message);                     

        #endregion Employment Information

        #region Tender Cost Information

            Array TenderCostInformationDetailsDAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int adminCode);
            bool AddTenderCostInformationDetails(TenderCostInformationViewModel tenderCostInformationViewModel, ref string message);
            bool EditCostInformationDetails(TenderCostInformationViewModel tenderCostInformationViewModel, ref string message);
            TenderCostInformationViewModel GetTenderCostInformationDetails(int priceId);
            bool DeleteTenderCostInformationDetails(int priceId, ref string message);

        #endregion Tender Cost Information

        #region Tender Equipment

            Array ListTenderEquipmentDetails(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int adminCode);
            bool AddTenderEquipmentDetails(TenderEquipmentViewModel tenderequipmentViewModel, ref string message);
            bool EditTenderEquipmentDetails(TenderEquipmentViewModel tenderEquipmentViewModel, ref string message);
            TenderEquipmentViewModel GetTenderEquipmentDetails(int equipmentId);
            bool DeleteTenderEquipmentDetails(int equipmentId, ref string message);
            List<SelectListItem> PopulateEquipmentType();
        #endregion Tender Equipment       

        #region Forty Point Check List

            Array ListFortyPointCheckListDetails(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int adminNdCode);
            Array ListConstructionLabEquipmentDetails(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int adminCode);
            bool AddEditFortyPointCheckList(FortyPointCheckListViewModel fortyPointCheckListViewModel, ref string message);
            bool DeleteFortyPointCheckListDetails(int checkListPointId, ref string message);
                   
        #endregion Forty Point Check List
    }
}