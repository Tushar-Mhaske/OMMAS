using PMGSY.Common;
using PMGSY.Extensions;
using PMGSY.Models;
using PMGSY.Models.OnlineFundRequest;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.DAL.OnlineFundProcess
{
    public class OnlineFundProcessDAL: IOnlineFundProcessDAL
    {
        private PMGSYEntities dbContext = null;

        private CommonFunctions objCommon = new CommonFunctions();

        /// <summary>
        /// returns the array of requests
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="State"></param>
        /// <param name="Year"></param>
        /// <param name="Batch"></param>
        /// <param name="Collaboration"></param>
        /// <param name="Agency"></param>
        /// <param name="Scheme"></param>
        /// <returns></returns>
        public Array GetListOfOnlineFundRequestsDAL(int page, int rows, string sidx, string sord, out long totalRecords, int State, int Year, int Batch, int Collaboration, int Agency, int Scheme)
        {
            dbContext = new PMGSYEntities();
            try
            {
                //list of PIUs which are under the agency provided by selecting the filter.
                var lstPIUs = dbContext.ADMIN_DEPARTMENT.Where(m => m.MAST_PARENT_ND_CODE == PMGSYSession.Current.AdminNdCode && m.ADMIN_ND_ACTIVE == "Y" && m.MAST_ND_TYPE == "D").Select(m => m.ADMIN_ND_CODE).ToList();

                int parentNDCode = dbContext.ADMIN_DEPARTMENT.Where(m => m.MAST_AGENCY_CODE == Agency && m.MAST_STATE_CODE == State && m.MAST_ND_TYPE == "S").Select(m => m.ADMIN_ND_CODE).FirstOrDefault();

                var lstDetails = (from item in dbContext.OFP_REQUEST_MASTER
                                  where
                                  (State <= 0 ? 1 :item.MAST_STATE_CODE) == (State <= 0 ? 1 : State) &&
                                  (Year <= 0 ? 1 : item.MAST_YEAR) == ( Year <= 0 ? 1 : Year) &&
                                  (Batch <= 0 ? 1 : item.IMS_BATCH) == ( Batch <= 0 ? 1 : Batch) &&
                                  (Collaboration <= 0 ? 1 : item.IMS_COLLABORATION) == (Collaboration <= 0 ? 1 : Collaboration) &&
                                  (Agency <= 0 ? 1 : item.ADMIN_ND_CODE) == (parentNDCode <= 0 ? 1 : parentNDCode) &&
                                  item.MAST_PMGSY_SCHEME == Scheme
                                  select new
                                  {
                                      item.MASTER_STATE.MAST_STATE_NAME,
                                      item.MASTER_YEAR.MAST_YEAR_TEXT,
                                      item.MASTER_BATCH.MAST_BATCH_NAME,
                                      item.ADMIN_DEPARTMENT.ADMIN_ND_NAME,
                                      item.FILE_DATE,
                                      item.FILE_NO,
                                      item.MAST_PMGSY_SCHEME,
                                      item.REQUEST_AMOUNT,
                                      item.REQUEST_ID,
                                      item.REQUEST_FINALIZE,
                                      item.MAST_YEAR,
                                      item.MAST_STATE_CODE,
                                      item.IMS_BATCH,
                                      item.IMS_COLLABORATION,
                                      item.ADMIN_ND_CODE,
                                      item.REQUEST_INSTALLMENT
                                  }).OrderByDescending(m=>m.REQUEST_ID).Distinct();

                totalRecords = lstDetails.Count();

                if (sidx.Trim() != string.Empty)
                {
                    lstDetails = lstDetails.OrderByDescending(m => m.FILE_NO).Skip(page * rows).Take(rows);
                }
                else
                {
                    lstDetails = lstDetails.OrderBy(m => m.FILE_NO).Skip(page * rows).Take(rows);
                }

                var jsonResult = lstDetails.Select(requestDetails => new
                {
                    requestDetails.ADMIN_ND_NAME,
                    requestDetails.FILE_DATE,
                    requestDetails.FILE_NO,
                    requestDetails.MAST_BATCH_NAME,
                    requestDetails.MAST_PMGSY_SCHEME,
                    requestDetails.MAST_STATE_NAME,
                    requestDetails.MAST_YEAR_TEXT,
                    requestDetails.REQUEST_AMOUNT,
                    requestDetails.REQUEST_ID,
                    requestDetails.REQUEST_FINALIZE,
                    requestDetails.MAST_YEAR,
                    requestDetails.IMS_BATCH,
                    requestDetails.IMS_COLLABORATION,
                    requestDetails.MAST_STATE_CODE,
                    requestDetails.ADMIN_ND_CODE,
                    requestDetails.REQUEST_INSTALLMENT

                }).ToArray();

                return jsonResult.Select(result => new
                {
                    cell = new[] 
                    {
                        result.MAST_STATE_NAME.ToString(),
                        result.MAST_YEAR_TEXT.ToString(),
                        result.MAST_BATCH_NAME.ToString(),
                        result.ADMIN_ND_NAME.ToString(),
                        result.MAST_PMGSY_SCHEME == 1 ? "PMGSY I" : "PMGSY II",
                        result.REQUEST_INSTALLMENT.ToString(),
                        dbContext.IMS_SANCTIONED_PROJECTS.Where(m=> m.ADMIN_DEPARTMENT.MAST_PARENT_ND_CODE == parentNDCode && m.IMS_YEAR == result.MAST_YEAR && m.IMS_BATCH == result.IMS_BATCH && m.MAST_STATE_CODE == result.MAST_STATE_CODE && m.MAST_PMGSY_SCHEME == result.MAST_PMGSY_SCHEME && m.IMS_PROPOSAL_TYPE == "P" && m.IMS_SANCTIONED == "Y").Count().ToString(),
                        dbContext.IMS_SANCTIONED_PROJECTS.Where(m=> lstPIUs.Contains(m.MAST_DPIU_CODE) && m.IMS_YEAR == result.MAST_YEAR && m.IMS_BATCH == result.IMS_BATCH && m.MAST_STATE_CODE == result.MAST_STATE_CODE && m.MAST_PMGSY_SCHEME == result.MAST_PMGSY_SCHEME && m.IMS_PROPOSAL_TYPE == "L" && m.IMS_SANCTIONED == "Y").Count().ToString(),
                        (
                            from item in dbContext.IMS_SANCTIONED_PROJECTS
                            where 
                            item.IMS_BATCH == result.IMS_BATCH &&
                            item.IMS_YEAR == result.MAST_YEAR &&
                            item.IMS_COLLABORATION == result.IMS_COLLABORATION &&
                            //item.MAST_STATE_CODE == result.MAST_STATE_CODE &&
                            item.MAST_PMGSY_SCHEME == result.MAST_PMGSY_SCHEME &&
                            lstPIUs.Contains(item.MAST_DPIU_CODE)
                            group item by 1 into lstGroup
                            select new
                            {
                                SanctionAmount = (((Scheme == 1 ? lstGroup.Sum(m=>m.IMS_SANCTIONED_PAV_AMT) + lstGroup.Sum(m=>m.IMS_SANCTIONED_PW_AMT) + lstGroup.Sum(m=>m.IMS_SANCTIONED_OW_AMT) + lstGroup.Sum(m=>m.IMS_SANCTIONED_CD_AMT) + lstGroup.Sum(m=>m.IMS_SANCTIONED_BW_AMT) : lstGroup.Sum(m=>m.IMS_SANCTIONED_PAV_AMT) + lstGroup.Sum(m=>m.IMS_SANCTIONED_PW_AMT) + lstGroup.Sum(m=>m.IMS_SANCTIONED_OW_AMT) + lstGroup.Sum(m=>m.IMS_SANCTIONED_CD_AMT) + lstGroup.Sum(m=>m.IMS_SANCTIONED_FC_AMT)) == null? 0 : (Scheme == 1 ? lstGroup.Sum(m=>m.IMS_SANCTIONED_PAV_AMT) + lstGroup.Sum(m=>m.IMS_SANCTIONED_PW_AMT) + lstGroup.Sum(m=>m.IMS_SANCTIONED_OW_AMT) + lstGroup.Sum(m=>m.IMS_SANCTIONED_CD_AMT) + lstGroup.Sum(m=>m.IMS_SANCTIONED_BW_AMT) : lstGroup.Sum(m=>m.IMS_SANCTIONED_PAV_AMT) + lstGroup.Sum(m=>m.IMS_SANCTIONED_PW_AMT) + lstGroup.Sum(m=>m.IMS_SANCTIONED_OW_AMT) + lstGroup.Sum(m=>m.IMS_SANCTIONED_CD_AMT) + lstGroup.Sum(m=>m.IMS_SANCTIONED_FC_AMT))) + (lstGroup.Sum(m=>m.IMS_SANCTIONED_RS_AMT) + lstGroup.Sum(m=>m.IMS_SANCTIONED_BS_AMT))) / 100
                            }
                        ).Select(m=>m.SanctionAmount).FirstOrDefault().ToString(),
                        result.FILE_NO == null ? "" : result.FILE_NO.ToString(),
                        result.FILE_DATE.HasValue ? objCommon.GetDateTimeToString(result.FILE_DATE.Value) : "",
                        result.REQUEST_AMOUNT== null? "": result.REQUEST_AMOUNT.ToString(),
                        //result.REQUEST_FINALIZE == "N" ? "<a href='#' title='Click here to upload document details' class='ui-icon ui-icon-plusthick ui-align-center' onClick=UploadDetails('" + URLEncrypt.EncryptParameters(new string[]{ result.REQUEST_ID.ToString().Trim()}) +"'); return false;'>View</a>" : "<span class='ui-icon ui-icon-locked ui-align-center'></span>",
                        result.REQUEST_FINALIZE == "Y" ? "<span class='ui-icon ui-icon-locked ui-align-center'></span>" : "<a href='#' title='Click here to upload document details' class='ui-icon ui-icon-plusthick ui-align-center' onClick=UploadDetails('" + URLEncrypt.EncryptParameters(new string[]{ result.REQUEST_ID.ToString().Trim()}) +"'); return false;'>View</a>",
                        "<a href='#' title='Click here to view proposal details' class='ui-icon ui-icon-zoomin ui-align-center' onClick=ViewProposalDetails('" + URLEncrypt.EncryptParameters(new string[]{ result.REQUEST_ID.ToString().Trim()}) +"'); return false;'>View</a>",
                        result.REQUEST_FINALIZE == "Y" ? "<span class='ui-icon ui-icon-locked ui-align-center'></span>" : "<a href='#' title='Click here to finalize request details' class='ui-icon ui-icon-unlocked ui-align-center' onClick=FinalizeRequestDetails('" + URLEncrypt.EncryptParameters(new string[]{ result.REQUEST_ID.ToString().Trim()}) +"'); return false;'>Finalize</a>",
                        "<a href='#' title='Click here to view request observation' class='ui-icon ui-icon-zoomin ui-align-center' onClick=ViewRequestObservation('" + URLEncrypt.EncryptParameters(new string[]{ result.REQUEST_ID.ToString().Trim()}) +"'); return false;'>View</a>",
                        "<a href='#' title='Click here to view request details' class='ui-icon ui-icon-zoomin ui-align-center' onClick=ViewRequestDetails('" + URLEncrypt.EncryptParameters(new string[]{ result.REQUEST_ID.ToString().Trim()}) +"'); return false;'>View</a>",
                        dbContext.OFP_CONDITION_IMPOSED.Any(m=>m.REQUEST_ID == result.REQUEST_ID && m.CONDITION_STATUS == "N") ? "<a href='#' title='Click here to reply on conditions imposed' class='ui-icon ui-icon-plusthick ui-align-center' onClick=AddConditionReply('" + URLEncrypt.EncryptParameters(new string[]{ result.REQUEST_ID.ToString().Trim()}) +"'); return false;'>View</a>" : "<span>-</span>",
                        result.REQUEST_FINALIZE == "N" ? "<a href='#' title='Click here to delete request details' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteRequestDetails('" + URLEncrypt.EncryptParameters(new string[]{ result.REQUEST_ID.ToString().Trim()}) +"'); return false;'>View</a>" : "<span class='ui-icon ui-icon-locked ui-align-center'></span>",
                        (dbContext.OFP_REQUEST_APPROVAL.Any(m=>m.REQUEST_ID == result.REQUEST_ID && m.APPROVAL_STATUS == "R") && !dbContext.OFP_REQUEST_MASTER.Any(r=>r.OLD_REQUEST_ID == result.REQUEST_ID)) ? "<a href='#' title='Click here to regenerate request details' class='ui-icon ui-icon-plusthick ui-align-center' onClick=RegenerateRequestDetails('" + URLEncrypt.EncryptParameters(new string[]{ result.REQUEST_ID.ToString().Trim()}) +"'); return false;'>View</a>" : "<span>-</span>",
                    }

                }).ToArray();
            }
            catch (Exception)
            {
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

        /// <summary>
        /// saves the details of online fund request.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool AddOnlineFundRequestDAL(OnlineFundRequestViewModel model,ref string message)
        {
            dbContext = new PMGSYEntities();
            try
            {
                OFP_REQUEST_MASTER requestDetails = new OFP_REQUEST_MASTER();
                if (dbContext.OFP_REQUEST_MASTER.Any())
                {
                    requestDetails.REQUEST_ID = dbContext.OFP_REQUEST_MASTER.Max(m => m.REQUEST_ID) + 1;
                }
                else
                {
                    requestDetails.REQUEST_ID = 1;
                }

                if (dbContext.OFP_REQUEST_MASTER.Any(m => m.MAST_STATE_CODE == model.MAST_STATE_CODE && m.MAST_YEAR == model.YEAR && m.IMS_COLLABORATION == model.COLLABORATION && m.MAST_PMGSY_SCHEME == model.PMGSY_SCHEME && m.IMS_BATCH == model.BATCH))
                {
                    requestDetails.REQUEST_INSTALLMENT = dbContext.OFP_REQUEST_MASTER.Where(m => m.MAST_STATE_CODE == model.MAST_STATE_CODE && m.MAST_YEAR == model.YEAR && m.IMS_COLLABORATION == model.COLLABORATION && m.MAST_PMGSY_SCHEME == model.PMGSY_SCHEME && m.IMS_BATCH == model.BATCH).Max(m => m.REQUEST_INSTALLMENT) + 1;
                }
                else
                {
                    requestDetails.REQUEST_INSTALLMENT = 1;
                }

                requestDetails.ADMIN_ND_CODE = PMGSYSession.Current.AdminNdCode;
                requestDetails.APPROVAL_CONDITION_IMPOSSED = null;
                requestDetails.ELIGIBLE_FOR_NEXT_REQUEST = "N";
                requestDetails.IMS_BATCH = model.BATCH;
                requestDetails.IMS_COLLABORATION = model.COLLABORATION;
                requestDetails.MAST_FUND_TYPE = "P";
                requestDetails.MAST_PMGSY_SCHEME = model.PMGSY_SCHEME;
                requestDetails.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                requestDetails.MAST_YEAR = model.YEAR;
                requestDetails.OFP_CONDITION_IMPOSED = null;
                requestDetails.OFP_REQUEST_APPROVAL = null;
                requestDetails.PREVIOUS_CONDITION_IMPOSSED = null;
                requestDetails.RELEASE_AMOUNT = null;
                requestDetails.RELEASE_DATE = null;
                requestDetails.REQUEST_INITIATE_DATE = DateTime.Now;
                requestDetails.REQUEST_AMOUNT = model.AMOUNT;
                requestDetails.REQUEST_FINALIZE = "N";
                requestDetails.REQUEST_FINALIZE_DATE = null;
                requestDetails.REQUEST_REMARKS = model.REMARKS;
                requestDetails.FILE_NO = null;
                requestDetails.FILE_DATE = null;
                requestDetails.USERID = PMGSYSession.Current.UserId;
                requestDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                dbContext.OFP_REQUEST_MASTER.Add(requestDetails);
                dbContext.SaveChanges();
                message = "";
                return true;
            }
            catch (DbEntityValidationException ex)
            {
                message = "Error occurred while processing your request.";
                return false;
            }
            catch (Exception)
            {
                message = "Error occurred while processing your request.";
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
        /// updates the details of online fund request.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool UpdateOnlineFundRequestDAL(OnlineFundRequestViewModel model, ref string message)
        {
            dbContext = new PMGSYEntities();
            try
            {
                OFP_REQUEST_MASTER requestDetails = dbContext.OFP_REQUEST_MASTER.Find(model.REQUEST_ID);
                if (requestDetails != null)
                {
                    requestDetails.ADMIN_ND_CODE = model.ADMIN_ND_CODE;
                    requestDetails.APPROVAL_CONDITION_IMPOSSED = null;
                    requestDetails.ELIGIBLE_FOR_NEXT_REQUEST = null;
                    requestDetails.IMS_BATCH = model.BATCH;
                    requestDetails.IMS_COLLABORATION = model.COLLABORATION;
                    requestDetails.MAST_FUND_TYPE = "P";
                    requestDetails.MAST_PMGSY_SCHEME = model.PMGSY_SCHEME;
                    requestDetails.MAST_STATE_CODE = PMGSYSession.Current.StateCode;
                    requestDetails.MAST_YEAR = model.YEAR;
                    requestDetails.OFP_CONDITION_IMPOSED = null;
                    requestDetails.OFP_REQUEST_APPROVAL = null;
                    requestDetails.PREVIOUS_CONDITION_IMPOSSED = null;
                    requestDetails.RELEASE_AMOUNT = null;
                    requestDetails.RELEASE_DATE = null;
                    requestDetails.REQUEST_AMOUNT = model.AMOUNT;
                    requestDetails.REQUEST_FINALIZE = "N";
                    requestDetails.REQUEST_FINALIZE_DATE = null;
                    requestDetails.REQUEST_INSTALLMENT = model.INSTALLMENT;
                    requestDetails.REQUEST_REMARKS = model.REMARKS;
                    requestDetails.USERID = PMGSYSession.Current.UserId;
                    requestDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    dbContext.Entry(requestDetails).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    message = "";
                    return true;
                }
                else
                {
                    message = "Error occurred while processing your request.";
                    return false;
                }
            }
            catch (Exception)
            {
                message = "Error occurred while processing your request.";
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
        /// returns the details of fund request for updation.
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public OnlineFundRequestViewModel GetOnlineFundRequestDetailsDAL(int requestId)
        {
            dbContext = new PMGSYEntities();
            OnlineFundRequestViewModel model = new OnlineFundRequestViewModel();
            try
            {
                OFP_REQUEST_MASTER requestDetails = dbContext.OFP_REQUEST_MASTER.Find(requestId);
                if (requestDetails != null)
                {
                    model.ADMIN_ND_CODE = requestDetails.ADMIN_ND_CODE;
                    model.AMOUNT = requestDetails.REQUEST_AMOUNT;
                    model.APPROVAL_CONDITION_IMPOSSED = requestDetails.APPROVAL_CONDITION_IMPOSSED;
                    model.BATCH = requestDetails.IMS_BATCH;
                    model.COLLABORATION = requestDetails.IMS_COLLABORATION;
                    model.ELIGIBLE_FOR_NEXT_REQUEST = requestDetails.ELIGIBLE_FOR_NEXT_REQUEST;
                    model.FILE_DATE = requestDetails.FILE_DATE.HasValue ? objCommon.GetDateTimeToString(requestDetails.FILE_DATE.Value) : "";
                    model.FILE_NO = requestDetails.FILE_NO;
                    model.FINALIZE = requestDetails.REQUEST_FINALIZE;
                    model.FINALIZE_DATE = requestDetails.REQUEST_FINALIZE_DATE.HasValue ? objCommon.GetDateTimeToString(requestDetails.REQUEST_FINALIZE_DATE.Value) : "";
                    model.FUND_TYPE = requestDetails.MAST_FUND_TYPE;
                    model.INSTALLMENT = requestDetails.REQUEST_INSTALLMENT;
                    model.MAST_STATE_CODE = requestDetails.MAST_STATE_CODE;
                    model.PMGSY_SCHEME = requestDetails.MAST_PMGSY_SCHEME;
                    model.PREVIOUS_CONDITION_IMPOSSED = requestDetails.PREVIOUS_CONDITION_IMPOSSED;
                    model.RELEASE_AMOUNT = requestDetails.RELEASE_AMOUNT;
                    model.RELEASE_DATE = requestDetails.RELEASE_DATE.HasValue ?objCommon.GetDateTimeToString(requestDetails.RELEASE_DATE.Value) : "";
                    model.REMARKS = requestDetails.REQUEST_REMARKS;
                    model.YEAR = requestDetails.MAST_YEAR;
                    model.StateName = requestDetails.MASTER_STATE.MAST_STATE_NAME;
                    model.BatchName = requestDetails.MASTER_BATCH.MAST_BATCH_NAME;
                    model.YearName = requestDetails.MASTER_YEAR.MAST_YEAR_TEXT;
                    model.CollaborationName = requestDetails.MASTER_STREAMS.MAST_STREAM_NAME;
                    model.RELEASE_UO_NO = requestDetails.RELEASE_UO_NO;
                    model.RELEASE_AMOUNT = requestDetails.RELEASE_AMOUNT;
                    model.SANCTION_AMOUNT = requestDetails.SANCTION_AMOUNT;
                    model.AgencyName = requestDetails.ADMIN_DEPARTMENT.ADMIN_ND_NAME;
                    return model;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
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
        /// deletes the entry of online fund request.
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public bool DeleteOnlineFundRequestDAL(int requestId)
        {
            dbContext = new PMGSYEntities();
            try
            {
                OFP_REQUEST_MASTER requestDetails = dbContext.OFP_REQUEST_MASTER.Find(requestId);
                if (requestDetails != null)
                {
                    requestDetails.USERID = PMGSYSession.Current.UserId;
                    requestDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    dbContext.Entry(requestDetails).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();


                    dbContext.OFP_REQUEST_MASTER.Remove(requestDetails);
                    dbContext.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
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
        /// finalizes the request details.
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public bool FinalizeRequestDetailsDAL(int requestId,out string message)
        {
            dbContext = new PMGSYEntities();
            try
            {
                if (!dbContext.OFP_REQUEST_DOCUMENT_MAPPING.Any(m => m.REQUEST_ID == requestId))
                {
                    message = "Please upload necessary files in order to finalize the request.";
                    return false;
                }

                OFP_REQUEST_MASTER requestDetails = dbContext.OFP_REQUEST_MASTER.Find(requestId);
                if (requestDetails != null)
                {
                    requestDetails.REQUEST_FINALIZE = "Y";
                    requestDetails.REQUEST_FINALIZE_DATE = DateTime.Now;
                    requestDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    requestDetails.USERID = PMGSYSession.Current.UserId;
                    dbContext.Entry(requestDetails).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    message = "";
                    return true;
                }
                else
                {
                    message = "Error occurred while processing your request.";
                    return false;
                }
            }
            catch (Exception)
            {
                message = "Error occurred while processing your request.";
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
        /// returns the list of proposals
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public Array GetProposalListDAL(int page, int rows, string sidx, string sord, out long totalRecords, int requestId)
        {
            dbContext = new PMGSYEntities();

            try
            {
                OFP_REQUEST_MASTER requestDetails = dbContext.OFP_REQUEST_MASTER.Find(requestId);
                if (requestDetails != null)
                {
                    var lstProposalDetails = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.MAST_STATE_CODE == requestDetails.MAST_STATE_CODE && m.IMS_YEAR == requestDetails.MAST_YEAR && m.IMS_BATCH == requestDetails.IMS_BATCH && m.IMS_COLLABORATION == requestDetails.IMS_COLLABORATION && m.ADMIN_DEPARTMENT.MAST_PARENT_ND_CODE == requestDetails.ADMIN_ND_CODE && m.MAST_PMGSY_SCHEME == requestDetails.MAST_PMGSY_SCHEME).ToList();

                    totalRecords = lstProposalDetails.Count();

                    if (!String.IsNullOrEmpty(sidx))
                    {
                        if (sord == "asc")
                        {
                            lstProposalDetails = lstProposalDetails.OrderBy(m => m.IMS_ROAD_NAME).Skip(page * rows).Take(rows).ToList();
                        }
                        else
                        {
                            lstProposalDetails = lstProposalDetails.OrderByDescending(m => m.IMS_ROAD_NAME).Skip(page * rows).Take(rows).ToList();
                        }
                    }
                    else
                    {
                        lstProposalDetails = lstProposalDetails.OrderBy(m => m.IMS_ROAD_NAME).Skip(page * rows).Take(rows).ToList();
                    }

                    var jsonResult = lstProposalDetails.Select(m => new
                    {
                        m.MASTER_DISTRICT.MAST_DISTRICT_NAME,
                        m.MASTER_BLOCK.MAST_BLOCK_NAME,
                        m.IMS_PACKAGE_ID,
                        m.IMS_YEAR,
                        m.IMS_ROAD_NAME,
                        m.IMS_BRIDGE_NAME,
                        m.IMS_PAV_LENGTH,
                        m.IMS_BRIDGE_LENGTH,
                        m.IMS_SANCTIONED_BS_AMT,
                        m.IMS_SANCTIONED_BW_AMT,
                        m.IMS_SANCTIONED_CD_AMT,
                        m.IMS_SANCTIONED_FC_AMT,
                        m.IMS_SANCTIONED_HS_AMT,
                        m.IMS_SANCTIONED_PAV_AMT,
                        m.IMS_SANCTIONED_OW_AMT,
                        m.IMS_SANCTIONED_PW_AMT,
                        m.IMS_SANCTIONED_RS_AMT,
                        m.IMS_SANCTIONED_MAN_AMT1,
                        m.IMS_SANCTIONED_MAN_AMT2,
                        m.IMS_SANCTIONED_MAN_AMT3,
                        m.IMS_SANCTIONED_MAN_AMT4,
                        m.IMS_SANCTIONED_MAN_AMT5,
                        m.IMS_SANCTIONED_RENEWAL_AMT,
                        m.IMS_PROPOSAL_TYPE

                    }).ToArray();

                    return jsonResult.Select(result => new
                    {
                        cell = new[] 
                        {
                            result.MAST_DISTRICT_NAME == null ? "" : result.MAST_DISTRICT_NAME.ToString(),
                            result.MAST_BLOCK_NAME == null ? "": result.MAST_BLOCK_NAME.ToString(),
                            result.IMS_PROPOSAL_TYPE == "P" ? "Road" : "Bridge",
                            result.IMS_PACKAGE_ID.ToString(),
                            (result.IMS_YEAR + " - " + (result.IMS_YEAR + 1)).ToString(),
                            result.IMS_PROPOSAL_TYPE == "P" ? result.IMS_ROAD_NAME.ToString() : result.IMS_BRIDGE_NAME.ToString(),
                            result.IMS_PROPOSAL_TYPE == "P" ? result.IMS_PAV_LENGTH.ToString() : result.IMS_BRIDGE_LENGTH.ToString(),
                            result.IMS_PROPOSAL_TYPE == "P" ? 
                            ((result.IMS_SANCTIONED_CD_AMT == null ? 0 : result.IMS_SANCTIONED_CD_AMT) + (result.IMS_SANCTIONED_PW_AMT == null ? 0 : result.IMS_SANCTIONED_PW_AMT) + (result.IMS_SANCTIONED_PAV_AMT == null ? 0 : result.IMS_SANCTIONED_PAV_AMT) + (result.IMS_SANCTIONED_OW_AMT == null ? 0 : result.IMS_SANCTIONED_OW_AMT) + (result.IMS_SANCTIONED_FC_AMT == null ? 0 : result.IMS_SANCTIONED_FC_AMT)).ToString()
                            :((result.IMS_SANCTIONED_BS_AMT == null ? 0 : result.IMS_SANCTIONED_BS_AMT) + (result.IMS_SANCTIONED_BW_AMT == null ? 0 : result.IMS_SANCTIONED_BW_AMT)).ToString(),
                            ((result.IMS_SANCTIONED_MAN_AMT1 == null ? 0 : result.IMS_SANCTIONED_MAN_AMT1) + (result.IMS_SANCTIONED_MAN_AMT2 == null ? 0 : result.IMS_SANCTIONED_MAN_AMT2) + (result.IMS_SANCTIONED_MAN_AMT3 == null ? 0 : result.IMS_SANCTIONED_MAN_AMT3) + (result.IMS_SANCTIONED_MAN_AMT4 == null ? 0 : result.IMS_SANCTIONED_MAN_AMT4) + (result.IMS_SANCTIONED_MAN_AMT5 == null ? 0 : result.IMS_SANCTIONED_MAN_AMT5) + (result.IMS_SANCTIONED_RENEWAL_AMT == null ? 0 : result.IMS_SANCTIONED_RENEWAL_AMT)).ToString()
                        }
                    }).ToArray();
                }
                else
                {
                    totalRecords = 0;
                    return null;
                }
            }
            catch (Exception)
            {
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

        /// <summary>
        /// returns the flag for checking the document needed
        /// </summary>
        /// <returns></returns>
        public string GetDocumentNeeded(string id)
        {
            dbContext = new PMGSYEntities();

            try
            {
                string[] decryptedParameters = URLEncrypt.DecryptParameters(new string[] { id.Split('/')[0], id.Split('/')[1], id.Split('/')[2] });
                
                int requestCode =  Convert.ToInt32(decryptedParameters[0]);
                List<SelectListItem> lstDocuments = new List<SelectListItem>();
                OFP_REQUEST_MASTER requestDetails = dbContext.OFP_REQUEST_MASTER.Find(requestCode);
                int year = DateTime.Now.Year;
                if ((requestDetails.RELEASE_DATE.HasValue ? requestDetails.RELEASE_DATE.Value : DateTime.Now) < objCommon.GetStringToDateTime("31/10/" + year))
                {
                    if (dbContext.OFP_REQUEST_DOCUMENT_MAPPING.Any(m => m.REQUEST_ID == requestCode))
                    {
                        lstDocuments = new SelectList(dbContext.OFP_DOCUMENT_MASTER.Where(m => m.DOCUMENT_BEFORE_31OCT == "Y" && !(dbContext.OFP_REQUEST_DOCUMENT_MAPPING.Where(d => d.REQUEST_ID == requestCode).Select(d => d.DOCUMENT_ID)).Contains(m.DOCUMENT_ID)), "DOCUMENT_ID", "DOCUMENT_NAME").ToList();
                    }
                    else
                    {
                        lstDocuments = new SelectList(dbContext.OFP_DOCUMENT_MASTER.Where(m => m.DOCUMENT_BEFORE_31OCT == "Y").ToList(), "DOCUMENT_ID", "DOCUMENT_NAME").ToList();
                    }
                }
                else
                {
                    if (dbContext.OFP_REQUEST_DOCUMENT_MAPPING.Any(m => m.REQUEST_ID == requestCode))
                    {
                        lstDocuments = new SelectList(dbContext.OFP_DOCUMENT_MASTER.Where(m => !(dbContext.OFP_REQUEST_DOCUMENT_MAPPING.Where(d => d.REQUEST_ID == requestCode).Select(d => d.DOCUMENT_ID)).Contains(m.DOCUMENT_ID)).ToList(), "DOCUMENT_ID", "DOCUMENT_NAME").ToList();
                    }
                    else
                    {
                        lstDocuments = new SelectList(dbContext.OFP_DOCUMENT_MASTER.ToList(), "DOCUMENT_ID", "DOCUMENT_NAME").ToList();
                    }
                }

                if (lstDocuments.Count() == 0)
                {
                    return "N";
                }
                else
                {
                    return "N";
                }
            }
            catch (Exception)
            {
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
        /// returns the list of documents according to the request.
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public List<SelectListItem> GetDocumentListByRequestId(string requestId)
        {
            dbContext = new PMGSYEntities();
            string[] decryptedParams = null;
            List<SelectListItem> lstDocuments = new List<SelectListItem>();
            try
            {
                decryptedParams = URLEncrypt.DecryptParameters(new string[] { requestId.Split('/')[0], requestId.Split('/')[1], requestId.Split('/')[2] });
                int requestCode = Convert.ToInt32(decryptedParams[0]);
                OFP_REQUEST_MASTER requestDetails = dbContext.OFP_REQUEST_MASTER.Find(requestCode);

                if(requestDetails.REQUEST_INITIATE_DATE.Month < 11 && requestDetails.REQUEST_INITIATE_DATE.Month > 3)
                {
                    if (dbContext.OFP_REQUEST_DOCUMENT_MAPPING.Any(m => m.REQUEST_ID == requestCode))
                    {
                        lstDocuments = new SelectList(dbContext.OFP_DOCUMENT_MASTER.Where(m => m.DOCUMENT_BEFORE_31OCT == "Y" && !(dbContext.OFP_REQUEST_DOCUMENT_MAPPING.Where(d => d.REQUEST_ID == requestCode).Select(d => d.DOCUMENT_ID)).Contains(m.DOCUMENT_ID)), "DOCUMENT_ID", "DOCUMENT_NAME").ToList();
                    }
                    else
                    {
                        lstDocuments = new SelectList(dbContext.OFP_DOCUMENT_MASTER.Where(m => m.DOCUMENT_BEFORE_31OCT == "Y" ).ToList(), "DOCUMENT_ID", "DOCUMENT_NAME").ToList();
                    }
                }
                else
                {
                    if (dbContext.OFP_REQUEST_DOCUMENT_MAPPING.Any(m => m.REQUEST_ID == requestCode))
                    {
                        lstDocuments = new SelectList(dbContext.OFP_DOCUMENT_MASTER.Where(m => !(dbContext.OFP_REQUEST_DOCUMENT_MAPPING.Where(d => d.REQUEST_ID == requestCode).Select(d => d.DOCUMENT_ID)).Contains(m.DOCUMENT_ID)).ToList(), "DOCUMENT_ID", "DOCUMENT_NAME").ToList();
                    }
                    else
                    {
                        lstDocuments = new SelectList(dbContext.OFP_DOCUMENT_MASTER.ToList(), "DOCUMENT_ID", "DOCUMENT_NAME").ToList();
                    }
                }
               
                return lstDocuments;
            }
            catch (Exception)
            {
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
        /// populates the forward request dropdown
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public List<SelectListItem> PopulateForwardRequestUsers(string requestId)
        {
            dbContext = new PMGSYEntities();
            string[] decryptedParameters = null;
            if (!String.IsNullOrEmpty(requestId.Split('/')[0]) && !String.IsNullOrEmpty(requestId.Split('/')[1]) && !String.IsNullOrEmpty(requestId.Split('/')[2]))
            {
                decryptedParameters = URLEncrypt.DecryptParameters(new string[] { requestId.Split('/')[0], requestId.Split('/')[1], requestId.Split('/')[2] });
            }
            int requestCode = Convert.ToInt32(decryptedParameters[0]);

            List<SelectListItem> lstUsers = new List<SelectListItem>();
            lstUsers.Insert(0, new SelectListItem { Value = "0" , Text = "Select"});
            try
            {
                switch (PMGSYSession.Current.RoleCode)
                {
                    case 25:
                        lstUsers.Insert(1, new SelectListItem { Value = "51", Text = "Section Officer" });
                        break;
                    case 51:
                        if (dbContext.OFP_REQUEST_APPROVAL.Any(m => m.REQUEST_ID == requestCode && m.APPROVAL_STATUS == "C" && m.REQUEST_FORWADED_FROM == 57))
                        {
                            //lstUsers.Insert(1, new SelectListItem { Value = "51", Text = "Section Officer" });
                            // forward to state
                        }
                        else
                        {
                            lstUsers.Insert(1, new SelectListItem { Value = "57", Text = "Under Secretary" });
                        }
                        break;
                    case 57:
                        lstUsers.Insert(1, new SelectListItem { Value = "58", Text = "Director" });
                        break;
                    case 58:
                        if (dbContext.OFP_REQUEST_APPROVAL.Any(m => m.REQUEST_ID == requestCode && m.APPROVAL_STATUS == "C" && m.REQUEST_FORWADED_FROM == 57))
                        {
                            //lstUsers.Insert(1, new SelectListItem { Value = "51", Text = "Section Officer" });
                            // forward to state
                            lstUsers.Insert(1, new SelectListItem { Value = "59", Text = "Joint Secretary" });
                        }
                        //else
                        //{
                        //    lstUsers.Insert(1, new SelectListItem { Value = "59", Text = "Joint Secretary" });
                        //}
                        break;
                    case 59:
                        lstUsers.Insert(1, new SelectListItem { Value = "60", Text = "Section Officer (IFD)" });
                        lstUsers.Insert(2, new SelectListItem { Value = "61", Text = "Under Secretary (IFD)" });
                        lstUsers.Insert(3, new SelectListItem { Value = "62", Text = "Director (IFD)" });
                        break;
                    case 60:
                        if (dbContext.OFP_REQUEST_APPROVAL.Any(m => m.REQUEST_ID == requestCode && m.APPROVAL_STATUS == "C" && m.REQUEST_FORWADED_FROM == 63))
                        {
                            lstUsers.Insert(1, new SelectListItem { Value = "51", Text = "Section Officer" });
                        }
                        else
                        {
                            lstUsers.Insert(1, new SelectListItem { Value = "61", Text = "Under Secretary (IFD)" });
                            lstUsers.Insert(2, new SelectListItem { Value = "62", Text = "Director (IFD)" });//no
                        }
                        break;
                    case 61:
                        if (dbContext.OFP_REQUEST_APPROVAL.Any(m => m.REQUEST_ID == requestCode && m.APPROVAL_STATUS == "C" && m.REQUEST_FORWADED_FROM == 63))
                        {
                            lstUsers.Insert(1, new SelectListItem { Value = "60", Text = "Section Officer (IFD)" });
                        }
                        else
                        {
                            lstUsers.Insert(1, new SelectListItem { Value = "62", Text = "Director (IFD)" });
                        }
                        break;
                    case 62:
                        if (dbContext.OFP_REQUEST_APPROVAL.Any(m => m.REQUEST_ID == requestCode && m.APPROVAL_STATUS == "C" && m.REQUEST_FORWADED_FROM == 63))
                        {
                            lstUsers.Insert(1, new SelectListItem { Value = "60", Text = "Section Officer (IFD)" });//no
                            lstUsers.Insert(2, new SelectListItem { Value = "61", Text = "Under Secretary (IFD)" });
                        }
                        else
                        {
                            lstUsers.Insert(1, new SelectListItem { Value = "63", Text = "AS & FA (IFD)" });
                        }
                        break;
                    case 63:
                        lstUsers.Insert(1, new SelectListItem { Value = "60", Text = "Section Officer (IFD)" });
                        lstUsers.Insert(2, new SelectListItem { Value = "61", Text = "Under Secretary (IFD)" });
                        lstUsers.Insert(3, new SelectListItem { Value = "62", Text = "Director (IFD)" });
                        break;
                    case 64:
                        lstUsers.Insert(1, new SelectListItem { Value = "57", Text = "Under Secretary" });
                        break;
                    default:
                        break;
                }

                return lstUsers;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// returns the file no. of a particular request
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public string GetRequestFileNumber(string requestId)
        {
            string[] decryptedParams = null;
            dbContext = new PMGSYEntities();
            try
            {
                decryptedParams = URLEncrypt.DecryptParameters(new string[] { requestId.Split('/')[0], requestId.Split('/')[1], requestId.Split('/')[2] });
                int requestCode = Convert.ToInt32(decryptedParams[0]);
                return dbContext.OFP_REQUEST_MASTER.Where(m => m.REQUEST_ID == requestCode).Select(m => m.FILE_NO).FirstOrDefault();
            }
            catch (Exception)
            {
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
        /// saves the entry of observatio in database
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddObservationDetailsDAL(RequestApprovalViewModel model)
        {
            dbContext = new PMGSYEntities();
            string[] decryptedParams = null;
            try
            {
                decryptedParams = URLEncrypt.DecryptParameters(new string[] { model.REQUEST_ID.Split('/')[0], model.REQUEST_ID.Split('/')[1], model.REQUEST_ID.Split('/')[2] });
                int requestCode = Convert.ToInt32(decryptedParams[0]);

                using (TransactionScope ts = new TransactionScope())
                {
                    OFP_REQUEST_MASTER requestDetails = dbContext.OFP_REQUEST_MASTER.Find(requestCode);

                    if (requestDetails != null)
                    {
                        requestDetails.FILE_NO = model.FILE_NO;
                        requestDetails.FILE_DATE = DateTime.Now;
                        requestDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        requestDetails.USERID = PMGSYSession.Current.UserId;
                        dbContext.Entry(requestDetails).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        return false;
                    }

                    OFP_REQUEST_APPROVAL approvalDetails = new OFP_REQUEST_APPROVAL();

                    if (dbContext.OFP_REQUEST_APPROVAL.Any())
                    {
                        approvalDetails.REQUEST_APPROVAL_ID = dbContext.OFP_REQUEST_APPROVAL.Max(m => m.REQUEST_APPROVAL_ID) + 1;
                    }
                    else
                    {
                        approvalDetails.REQUEST_APPROVAL_ID = 1;
                    }

                    if (PMGSYSession.Current.RoleCode == 51 && model.APPROVAL_STATUS == "A")
                    {
                        approvalDetails.APPROVAL_STATUS = "C";
                    }
                    else if (dbContext.OFP_REQUEST_APPROVAL.Any(m => m.REQUEST_ID == requestCode && m.APPROVAL_STATUS == "C"))
                    {
                        approvalDetails.APPROVAL_STATUS = "C";
                    }
                    else
                    {
                        approvalDetails.APPROVAL_STATUS = model.APPROVAL_STATUS;
                    }
                    approvalDetails.APPROVAL_DATE = DateTime.Now;
                    approvalDetails.REMARKS = model.REMARKS;
                    if (approvalDetails.APPROVAL_STATUS == "R")
                    {
                        approvalDetails.REJECT_LETTER_NAME = model.RejectLetterName;
                    }
                    approvalDetails.REQUEST_FORWADED_FROM = PMGSYSession.Current.RoleCode;
                    approvalDetails.REQUEST_FORWADED_TO = model.APPROVAL_STATUS == "R" ? null : (model.REQUEST_FORWADED_TO == 0 ? null : model.REQUEST_FORWADED_TO);
                    approvalDetails.REQUEST_ID = requestCode;
                    approvalDetails.CONDITION_IMPOSED = model.CONDITION_IMPOSED;
                    approvalDetails.RECOMMENDATION = null;
                    approvalDetails.USERID = PMGSYSession.Current.UserId;
                    approvalDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    dbContext.OFP_REQUEST_APPROVAL.Add(approvalDetails);
                    dbContext.SaveChanges();

                    if (model.CONDITION_IMPOSED == "Y")
                    {
                        OFP_CONDITION_IMPOSED conditionDetails = new OFP_CONDITION_IMPOSED();
                        if (dbContext.OFP_CONDITION_IMPOSED.Any())
                        {
                            conditionDetails.CONDITION_IMPOSED_ID = dbContext.OFP_CONDITION_IMPOSED.Max(m => m.CONDITION_IMPOSED_ID) + 1;
                        }
                        else
                        {
                            conditionDetails.CONDITION_IMPOSED_ID = 1;
                        }
                        conditionDetails.CONDITION_IMPOSED_BY = PMGSYSession.Current.RoleCode;
                        conditionDetails.CONDITION_IMPOSED_DATE = DateTime.Now;
                        conditionDetails.CONDITION_ID = model.ConditionCode;
                        conditionDetails.CONDITION_STATUS = "N";
                        conditionDetails.CONDITION_REPLY = null;
                        conditionDetails.REQUEST_ID = requestCode;
                        dbContext.OFP_CONDITION_IMPOSED.Add(conditionDetails);
                        dbContext.SaveChanges();
                    }

                    ts.Complete();
                    return true;
                }
            }
            catch (Exception)
            {
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
        /// saves the file and file details in database
        /// </summary>
        /// <param name="lstModels"></param>
        /// <returns></returns>
        public bool AddDocumentDetailsDAL(List<DocumentUploadViewModel> lstModels)
        {
            dbContext = new PMGSYEntities();
            string[] decryptedParams = null;
            try
            {
                var modelToFindRequestId = lstModels.Select(m=>m.EncryptedRequestId).FirstOrDefault();
                decryptedParams = URLEncrypt.DecryptParameters(new string[] { modelToFindRequestId.Split('/')[0], modelToFindRequestId.Split('/')[1], modelToFindRequestId.Split('/')[2] });
                int requestCode = Convert.ToInt32(decryptedParams[0]);
                string filePath = string.Empty;
                string fileName = string.Empty;
                using (TransactionScope ts = new TransactionScope())
                {
                    foreach (var item in lstModels)
                    {

                        fileName = dbContext.OFP_REQUEST_MASTER.Where(m=>m.REQUEST_ID == requestCode).Select(m=>m.MASTER_STATE.MAST_STATE_SHORT_CODE).FirstOrDefault() + "_" + requestCode + "_" + dbContext.OFP_DOCUMENT_MASTER.Where(m => m.DOCUMENT_ID == item.DOCUMENT_ID).Select(m => m.DOCUMENT_NAME).FirstOrDefault().Replace(' ','_');

                        OFP_REQUEST_DOCUMENT_MAPPING mappingDetails = new OFP_REQUEST_DOCUMENT_MAPPING();
                        if (dbContext.OFP_REQUEST_DOCUMENT_MAPPING.Any())
                        {
                            mappingDetails.REQUEST_DOCUMENT_ID = dbContext.OFP_REQUEST_DOCUMENT_MAPPING.Max(m => m.REQUEST_DOCUMENT_ID) + 1;
                        }
                        else
                        {
                            mappingDetails.REQUEST_DOCUMENT_ID = 1;
                        }

                        mappingDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        mappingDetails.USERID = PMGSYSession.Current.UserId;
                        mappingDetails.DOCUMENT_FINALIZE = "N";
                        mappingDetails.DOCUMENT_GENERATION_DATE = DateTime.Now;
                        mappingDetails.DOCUMENT_ID = item.DOCUMENT_ID;
                        mappingDetails.REMARKS = item.REMARKS;
                        mappingDetails.REQUEST_ID = requestCode;
                        mappingDetails.UPLOAD_FILE_NAME = fileName + "." + item.fileInfo.FileName.Split('.')[1];
                        dbContext.OFP_REQUEST_DOCUMENT_MAPPING.Add(mappingDetails);
                        dbContext.SaveChanges();

                        switch (item.DOCUMENT_ID)
                        {
                            case 1:
                                filePath = ConfigurationManager.AppSettings["OFP_FILE_UPLOAD_BALANCE_SHEET"];
                                break;
                            case 2:
                                filePath = ConfigurationManager.AppSettings["OFP_FILE_UPLOAD_MPR"];
                                break;
                            case 3:
                                filePath = ConfigurationManager.AppSettings["OFP_FILE_UPLOAD_SANCTION_LETTER"];
                                break;
                            case 4:
                                filePath = ConfigurationManager.AppSettings["OFP_FILE_UPLOAD_BANKER_CERTIFICATE_OB"];
                                break;
                            case 5:
                                filePath = ConfigurationManager.AppSettings["OFP_FILE_UPLOAD_UTILIZATION_CERTIFICATE"];
                                break;
                            case 6:
                                filePath = ConfigurationManager.AppSettings["OFP_FILE_UPLOAD_BANK_RECONCILITATION_OB"];
                                break;
                            case 7:
                                filePath = ConfigurationManager.AppSettings["OFP_FILE_UPLOAD_BANKER_CERTIFICATE_CB"];
                                break;
                            case 8:
                                filePath = ConfigurationManager.AppSettings["OFP_FILE_UPLOAD_BANKER_CERTIFICATE_CB"];
                                break;
                            default:
                                break;
                        }
                        
                        item.fileInfo.SaveAs(Path.Combine(filePath, fileName + "." + item.fileInfo.FileName.Split('.')[1]));
                    }
                    ts.Complete();
                    return true;
                }
            }
            catch (Exception)
            {
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
        /// returns the list of documents uploaded
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public Array GetListOfDocumentsUploadedDAL(int page, int rows, string sidx, string sord, out long totalRecords, int requestId)
        {
            dbContext = new PMGSYEntities();

            try
            {
                var lstDocuments = dbContext.OFP_REQUEST_DOCUMENT_MAPPING.Where(m => m.REQUEST_ID == requestId).ToList();

                totalRecords = lstDocuments.Count();

                if (!String.IsNullOrEmpty(sidx))
                {
                    if (sord == "asc")
                    {
                        lstDocuments = lstDocuments.OrderBy(m => m.UPLOAD_FILE_NAME).Skip(page * rows).Take(rows).ToList();
                    }
                    else
                    {
                        lstDocuments = lstDocuments.OrderByDescending(m => m.UPLOAD_FILE_NAME).Skip(page * rows).Take(rows).ToList();
                    }
                }
                else
                {
                    lstDocuments = lstDocuments.OrderBy(m => m.UPLOAD_FILE_NAME).Skip(page * rows).Take(rows).ToList();
                }

                var jsonResult = lstDocuments.Select(m => new
                {
                    m.DOCUMENT_ID,
                    m.DOCUMENT_GENERATION_DATE,
                    m.UPLOAD_FILE_NAME,
                    m.REQUEST_ID,
                    m.REQUEST_DOCUMENT_ID,
                    m.REMARKS,
                    
                }).ToArray();

                return jsonResult.Select(result => new
                {
                    cell = new[] 
                        {
                            dbContext.OFP_DOCUMENT_MASTER.Where(m=>m.DOCUMENT_ID == result.DOCUMENT_ID).Select(m=>m.DOCUMENT_DECSRIPTION).FirstOrDefault(),
                            result.UPLOAD_FILE_NAME == null ? "": result.UPLOAD_FILE_NAME.ToString(),
                            objCommon.GetDateTimeToString(result.DOCUMENT_GENERATION_DATE),
                            result.REMARKS == null ? "" : result.REMARKS.ToString(),
                            "<a href='#' title='Click here to download document details' class='ui-icon ui-icon-arrowthickstop-1-s ui-align-center' onClick=Download('" + URLEncrypt.EncryptParameters(new string[]{ result.REQUEST_DOCUMENT_ID.ToString().Trim() + "$" + result.DOCUMENT_ID.ToString().Trim() + "$" + result.UPLOAD_FILE_NAME.ToString().Trim()}) +"'); return false;'>Download</a>",
                            "<a href='#' title='Click here to delete document details' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteDocument('" + URLEncrypt.EncryptParameters(new string[]{ result.REQUEST_DOCUMENT_ID.ToString().Trim() + "$" + result.DOCUMENT_ID.ToString().Trim() + "$" + result.UPLOAD_FILE_NAME.ToString().Trim()}) +"'); return false;'>Delete Document</a>",
                        }
                }).ToArray();

            }
            catch (Exception)
            {
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

        /// <summary>
        /// returns the list of observations done
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public Array GetListofObservationDetailsDAL(int page, int rows, string sidx, string sord, out long totalRecords, int requestId)
        {
            dbContext = new PMGSYEntities();

            try
            {
                var lstDocuments = dbContext.OFP_REQUEST_APPROVAL.Where(m => m.REQUEST_ID == requestId).ToList();

                totalRecords = lstDocuments.Count();

                if (!String.IsNullOrEmpty(sidx))
                {
                    if (sord == "asc")
                    {
                        lstDocuments = lstDocuments.OrderBy(m => m.UM_User_Master.DefaultRoleID).Skip(page * rows).Take(rows).ToList();
                    }
                    else
                    {
                        lstDocuments = lstDocuments.OrderByDescending(m => m.UM_User_Master.DefaultRoleID).Skip(page * rows).Take(rows).ToList();
                    }
                }
                else
                {
                    lstDocuments = lstDocuments.OrderBy(m => m.UM_User_Master.DefaultRoleID).Skip(page * rows).Take(rows).ToList();
                }

                var jsonResult = lstDocuments.Select(m => new
                {
                    m.REQUEST_APPROVAL_ID,
                    m.REQUEST_FORWADED_FROM,
                    m.REQUEST_FORWADED_TO,
                    m.OFP_REQUEST_MASTER.FILE_NO,
                    m.REMARKS,
                    m.RECOMMENDATION,
                    m.APPROVAL_STATUS,
                    m.APPROVAL_DATE,
                    m.REJECT_LETTER_NAME

                }).ToArray();

                return jsonResult.Select(result => new
                {
                    cell = new[] 
                        {
                            dbContext.UM_Role_Master.Where(m=>m.RoleID == result.REQUEST_FORWADED_FROM).Select(m=>m.RoleName).FirstOrDefault(),
                            dbContext.UM_Role_Master.Where(m=>m.RoleID == result.REQUEST_FORWADED_TO).Select(m=>m.RoleName).FirstOrDefault() == null ? "-" :dbContext.UM_Role_Master.Where(m=>m.RoleID == result.REQUEST_FORWADED_TO).Select(m=>m.RoleName).FirstOrDefault(),
                            result.FILE_NO == null? "-": result.FILE_NO.ToString(),
                            result.APPROVAL_DATE.ToString("dd/MM/yyyy"),
                            result.REMARKS == null ? "" : result.REMARKS.ToString(),
                            result.APPROVAL_STATUS == "A" ? "Yes" : "-",
                            result.APPROVAL_STATUS == "R" ? "Yes" : "-",
                            result.APPROVAL_STATUS == "F" ? "Yes" : "-",
                            result.APPROVAL_STATUS == "R" ? result.REJECT_LETTER_NAME.ToString() : "",
                            result.APPROVAL_STATUS == "R" ? "<a href='#' title='Click here to download document details' class='ui-icon ui-icon-arrowthickstop-1-s ui-align-center' onClick=DownloadRejectLetter('" + URLEncrypt.EncryptParameters(new string[]{ result.REQUEST_APPROVAL_ID.ToString().Trim() + "$" + result.REJECT_LETTER_NAME.ToString().Trim()}) +"'); return false;'>Download</a>" : "",
                            
                        }
                }).ToArray();

            }
            catch (Exception)
            {
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

        /// <summary>
        /// returns whether the observation is added by user or not
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public string IsObservationDone(string requestId)
        {
            //Role has changed 06/12/2017

            dbContext = new PMGSYEntities();
            string[] decryptedParams = null;
            List<int> lowerRoleIdForApprovedRequests = new List<int>();
            try
            {
                decryptedParams = URLEncrypt.DecryptParameters(new string[] { requestId.Split('/')[0], requestId.Split('/')[1], requestId.Split('/')[2] });
                int requestCode = Convert.ToInt32(decryptedParams[0]);

                switch (PMGSYSession.Current.RoleCode)
                {
                    case 63:
                        lowerRoleIdForApprovedRequests.Add(60);
                        lowerRoleIdForApprovedRequests.Add(61);
                        lowerRoleIdForApprovedRequests.Add(62);
                        break;
                    case 62:
                        lowerRoleIdForApprovedRequests.Add(60);
                        lowerRoleIdForApprovedRequests.Add(61);
                        break;
                    case 61:
                        lowerRoleIdForApprovedRequests.Add(60);
                        break;
                    case 60:
                        lowerRoleIdForApprovedRequests.Add(51);
                        break;
                    default:
                        break;
                }

                if (dbContext.OFP_REQUEST_APPROVAL.Any(m => m.REQUEST_ID == requestCode && m.REQUEST_FORWADED_FROM == PMGSYSession.Current.RoleCode))
                {
                    return "Y";
                }

                if (dbContext.OFP_REQUEST_APPROVAL.Where(m=>m.REQUEST_ID == requestCode).OrderByDescending(m => m.REQUEST_APPROVAL_ID).Select(m => m.REQUEST_FORWADED_TO).FirstOrDefault() == PMGSYSession.Current.RoleCode)
                {
                    return "N";
                }
                //else if (PMGSYSession.Current.RoleCode == 52 && dbContext.OFP_REQUEST_APPROVAL.Where(m => m.REQUEST_ID == requestCode).OrderByDescending(m => m.REQUEST_APPROVAL_ID).Select(m => m.REQUEST_FORWADED_FROM).FirstOrDefault() != PMGSYSession.Current.RoleCode)
                else if (PMGSYSession.Current.RoleCode == 25 && dbContext.OFP_REQUEST_APPROVAL.Where(m => m.REQUEST_ID == requestCode).OrderByDescending(m => m.REQUEST_APPROVAL_ID).Select(m => m.REQUEST_FORWADED_FROM).FirstOrDefault() != PMGSYSession.Current.RoleCode)
                {
                    return "N";
                }
                else if (PMGSYSession.Current.RoleCode == 51) //changed 06/12/2017
                {
                    return "N";
                }
                else
                {
                    return "Y";
                }

                if (dbContext.OFP_REQUEST_APPROVAL.Any(m => m.REQUEST_ID == requestCode && m.REQUEST_FORWADED_FROM == 63 && m.APPROVAL_STATUS == "A" && m.REQUEST_FORWADED_TO == PMGSYSession.Current.RoleCode))
                {
                    return "N";
                }

                if (dbContext.OFP_REQUEST_APPROVAL.Where(m => m.REQUEST_ID == requestCode).OrderByDescending(m => m.REQUEST_APPROVAL_ID).Select(m => m.REQUEST_FORWADED_TO).FirstOrDefault() == PMGSYSession.Current.RoleCode)
                {
                    return "N";
                }

                if (dbContext.OFP_REQUEST_APPROVAL.Where(m => m.REQUEST_ID == requestCode && (m.APPROVAL_STATUS == "A" || m.APPROVAL_STATUS == "F") && dbContext.OFP_REQUEST_APPROVAL.Any(a=>a.REQUEST_ID == requestCode && m.APPROVAL_STATUS == "A" && a.REQUEST_FORWADED_FROM == 63)).OrderByDescending(m => m.REQUEST_APPROVAL_ID).Select(m => m.REQUEST_FORWADED_TO).FirstOrDefault() == PMGSYSession.Current.RoleCode)
                {
                    return "N";
                }

                

                if (dbContext.OFP_REQUEST_APPROVAL.Where(m => m.REQUEST_ID == requestCode).OrderByDescending(m=>m.REQUEST_APPROVAL_ID).Select(m=>m.REQUEST_FORWADED_FROM).FirstOrDefault() == PMGSYSession.Current.RoleCode)
                {
                    return "Y";
                }
                else if (dbContext.OFP_REQUEST_APPROVAL.Any(m => m.REQUEST_ID == requestCode && m.REQUEST_FORWADED_FROM == PMGSYSession.Current.RoleCode))
                {
                    return "Y";
                }
                if (dbContext.OFP_REQUEST_APPROVAL.Any(m => m.REQUEST_ID == requestCode && m.REQUEST_FORWADED_TO == PMGSYSession.Current.RoleCode) && dbContext.OFP_REQUEST_APPROVAL.Any(m => m.REQUEST_ID == requestCode && m.REQUEST_FORWADED_FROM != PMGSYSession.Current.RoleCode))
                {
                    return "N";
                }
                else if (dbContext.OFP_REQUEST_APPROVAL.Where(m => m.REQUEST_ID == requestCode && m.APPROVAL_STATUS != "R").OrderByDescending(m => m.REQUEST_APPROVAL_ID).Select(m => m.REQUEST_FORWADED_TO).FirstOrDefault() == PMGSYSession.Current.RoleCode)
                {
                    return "N";
                }
                else if (PMGSYSession.Current.RoleCode == 51 && dbContext.OFP_REQUEST_APPROVAL.Where(m => m.REQUEST_ID == requestCode).OrderByDescending(m => m.REQUEST_APPROVAL_ID).Select(m => m.REQUEST_FORWADED_FROM).FirstOrDefault() == PMGSYSession.Current.RoleCode)
                {
                    return "Y";
                }
                else if (PMGSYSession.Current.RoleCode == 51) //changed 06/12/2017
                {
                    return "N";
                }
                else
                {
                    return "Y";
                }
            }
            catch (Exception)
            {
                return "N";
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
        /// returns the total details of proposal for the state
        /// </summary>
        /// <param name="year"></param>
        /// <param name="batch"></param>
        /// <param name="collaboration"></param>
        /// <param name="scheme"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public bool GetTotalDetails(int year, int batch, int collaboration, int scheme, out TotalFundDetails details)
        {
            dbContext = new PMGSYEntities();

            try
            {
                var lstPIUs = dbContext.ADMIN_DEPARTMENT.Where(m=>m.MAST_PARENT_ND_CODE == PMGSYSession.Current.AdminNdCode && m.ADMIN_ND_ACTIVE == "Y" && m.MAST_ND_TYPE == "D").Select(m=>m.ADMIN_ND_CODE).ToList();

                var lstProposalDetails = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => lstPIUs.Contains(m.MAST_DPIU_CODE) && m.IMS_YEAR == year && m.IMS_BATCH == batch && m.IMS_COLLABORATION == collaboration && m.MAST_PMGSY_SCHEME == scheme).ToList();

                TotalFundDetails detailsToSend = new TotalFundDetails();

                detailsToSend.totalBridges = lstProposalDetails.Where(m => m.IMS_PROPOSAL_TYPE == "L").Count();
                detailsToSend.totalRoads = lstProposalDetails.Where(m => m.IMS_PROPOSAL_TYPE == "P").Count();
                detailsToSend.totalPavementLength = lstProposalDetails.Where(m => m.IMS_PROPOSAL_TYPE == "P").Sum(m => m.IMS_PAV_LENGTH);
                detailsToSend.totalBridgeLength = lstProposalDetails.Where(m => m.IMS_PROPOSAL_TYPE == "L").Sum(m => m.IMS_BRIDGE_LENGTH);
                detailsToSend.totalStateCost = Math.Round((lstProposalDetails.Sum(m => m.IMS_SANCTIONED_RS_AMT) + lstProposalDetails.Sum(m => m.IMS_SANCTIONED_BS_AMT)) / 100 ,2);
                detailsToSend.totalMordCost = Math.Round((scheme == 1 ? (lstProposalDetails.Sum(m => m.IMS_SANCTIONED_PAV_AMT) + lstProposalDetails.Sum(m => m.IMS_SANCTIONED_PW_AMT) + lstProposalDetails.Sum(m => m.IMS_SANCTIONED_OW_AMT) + lstProposalDetails.Sum(m => m.IMS_SANCTIONED_CD_AMT) + lstProposalDetails.Sum(m => m.IMS_SANCTIONED_BW_AMT)) : lstProposalDetails.Sum(m => m.IMS_SANCTIONED_PW_AMT) + lstProposalDetails.Sum(m => m.IMS_SANCTIONED_OW_AMT) + lstProposalDetails.Sum(m => m.IMS_SANCTIONED_CD_AMT) + lstProposalDetails.Sum(m => m.IMS_SANCTIONED_BW_AMT) + lstProposalDetails.Sum(m => m.IMS_SANCTIONED_FC_AMT) - lstProposalDetails.Sum(m => m.IMS_SANCTIONED_RS_AMT)).Value / 100,2);
                detailsToSend.totalSanctionCost = Math.Round((detailsToSend.totalMordCost + detailsToSend.totalStateCost).Value,2);
                detailsToSend.totalMaintenanceCost = Math.Round((scheme == 1 ? (lstProposalDetails.Sum(m => m.IMS_SANCTIONED_MAN_AMT1) + lstProposalDetails.Sum(m => m.IMS_SANCTIONED_MAN_AMT2) + lstProposalDetails.Sum(m => m.IMS_SANCTIONED_MAN_AMT3) + lstProposalDetails.Sum(m => m.IMS_SANCTIONED_MAN_AMT4) + lstProposalDetails.Sum(m => m.IMS_SANCTIONED_MAN_AMT5)) : (lstProposalDetails.Sum(m => m.IMS_SANCTIONED_MAN_AMT1) + lstProposalDetails.Sum(m => m.IMS_SANCTIONED_MAN_AMT2) + lstProposalDetails.Sum(m => m.IMS_SANCTIONED_MAN_AMT3) + lstProposalDetails.Sum(m => m.IMS_SANCTIONED_MAN_AMT4) + lstProposalDetails.Sum(m => m.IMS_SANCTIONED_MAN_AMT5) + lstProposalDetails.Sum(m => m.IMS_SANCTIONED_RENEWAL_AMT))).Value / 100,2);
                details = detailsToSend;
                return true;
            }
            catch (Exception)
            {
                details = null;
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
        /// returns the total of proposal details.
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public TotalFundDetails GetTotalDetailsByRequestId(int requestId)
        {
            dbContext = new PMGSYEntities();

            try
            {
                OFP_REQUEST_MASTER requestDetails = dbContext.OFP_REQUEST_MASTER.Find(requestId);

                var lstPIUs = dbContext.ADMIN_DEPARTMENT.Where(m => m.MAST_PARENT_ND_CODE == requestDetails.ADMIN_ND_CODE && m.ADMIN_ND_ACTIVE == "Y" && m.MAST_ND_TYPE == "D").Select(m => m.ADMIN_ND_CODE).ToList();

                var lstProposalDetails = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => lstPIUs.Contains(m.MAST_DPIU_CODE) && m.IMS_YEAR == requestDetails.MAST_YEAR && m.IMS_BATCH == requestDetails.IMS_BATCH && m.IMS_COLLABORATION == requestDetails.IMS_COLLABORATION && m.MAST_PMGSY_SCHEME == requestDetails.MAST_PMGSY_SCHEME).ToList();

                TotalFundDetails detailsToSend = new TotalFundDetails();

                detailsToSend.totalBridges = lstProposalDetails.Where(m => m.IMS_PROPOSAL_TYPE == "L").Count();
                detailsToSend.totalRoads = lstProposalDetails.Where(m => m.IMS_PROPOSAL_TYPE == "P").Count();
                detailsToSend.totalPavementLength = lstProposalDetails.Where(m => m.IMS_PROPOSAL_TYPE == "P").Sum(m => m.IMS_PAV_LENGTH);
                detailsToSend.totalBridgeLength = lstProposalDetails.Where(m => m.IMS_PROPOSAL_TYPE == "L").Sum(m => m.IMS_BRIDGE_LENGTH);
                detailsToSend.totalStateCost = Math.Round((lstProposalDetails.Sum(m => m.IMS_SANCTIONED_RS_AMT) + lstProposalDetails.Sum(m => m.IMS_SANCTIONED_BS_AMT)) / 100,2);
                detailsToSend.totalMordCost = Math.Round(((requestDetails.MAST_PMGSY_SCHEME == 1 ? (lstProposalDetails.Sum(m => m.IMS_SANCTIONED_PAV_AMT) + lstProposalDetails.Sum(m => m.IMS_SANCTIONED_PW_AMT) + lstProposalDetails.Sum(m => m.IMS_SANCTIONED_OW_AMT) + lstProposalDetails.Sum(m => m.IMS_SANCTIONED_CD_AMT) + lstProposalDetails.Sum(m => m.IMS_SANCTIONED_BW_AMT)) : lstProposalDetails.Sum(m => m.IMS_SANCTIONED_PW_AMT) + lstProposalDetails.Sum(m => m.IMS_SANCTIONED_OW_AMT) + lstProposalDetails.Sum(m => m.IMS_SANCTIONED_CD_AMT) + lstProposalDetails.Sum(m => m.IMS_SANCTIONED_BW_AMT) +lstProposalDetails.Sum(m => m.IMS_SANCTIONED_FC_AMT) - lstProposalDetails.Sum(m => m.IMS_SANCTIONED_RS_AMT)).Value / 100),2);
                detailsToSend.totalSanctionCost = detailsToSend.totalMordCost + detailsToSend.totalStateCost;
                detailsToSend.totalMaintenanceCost = Math.Round(((requestDetails.MAST_PMGSY_SCHEME == 1 ? (lstProposalDetails.Sum(m => m.IMS_SANCTIONED_MAN_AMT1) + lstProposalDetails.Sum(m => m.IMS_SANCTIONED_MAN_AMT2) + lstProposalDetails.Sum(m => m.IMS_SANCTIONED_MAN_AMT3) + lstProposalDetails.Sum(m => m.IMS_SANCTIONED_MAN_AMT4) + lstProposalDetails.Sum(m => m.IMS_SANCTIONED_MAN_AMT5)) : (lstProposalDetails.Sum(m => m.IMS_SANCTIONED_MAN_AMT1) + lstProposalDetails.Sum(m => m.IMS_SANCTIONED_MAN_AMT2) + lstProposalDetails.Sum(m => m.IMS_SANCTIONED_MAN_AMT3) + lstProposalDetails.Sum(m => m.IMS_SANCTIONED_MAN_AMT4) + lstProposalDetails.Sum(m => m.IMS_SANCTIONED_MAN_AMT5) + lstProposalDetails.Sum(m => m.IMS_SANCTIONED_RENEWAL_AMT))).Value / 100),2);

                return detailsToSend;
            }
            catch (Exception)
            {
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
        /// returns the list of requests for adding the observations to the particular user
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array GetActionRequiredRequestListDAL(int page, int rows, string sidx, string sord, out long totalRecords)
        {
            dbContext = new PMGSYEntities();

            try
            {
                List<int> lstUpperRoles = dbContext.OFP_ROLE_REQUEST_FORWARD_MAPPING.Where(m => m.ROLE_ID == PMGSYSession.Current.RoleCode).Select(m => m.FORWARD_ROLE_ID).ToList();

                var lstDetails = (from item in dbContext.OFP_REQUEST_MASTER
                                  join approval in dbContext.OFP_REQUEST_APPROVAL on item.REQUEST_ID equals approval.REQUEST_ID into details
                                  from d in details.DefaultIfEmpty() 
                                  where
                                  item.REQUEST_FINALIZE == "Y" &&
                                  //PMGSYSession.Current.RoleCode == 52 ? (item.REQUEST_FINALIZE  == "Y" &&  !dbContext.OFP_REQUEST_APPROVAL.Any(m=>m.REQUEST_ID == d.REQUEST_ID)) : (dbContext.OFP_REQUEST_APPROVAL.Where(m=>m.REQUEST_ID == d.REQUEST_ID).OrderByDescending(m=>m.REQUEST_APPROVAL_ID).Select(m=>m.REQUEST_FORWADED_TO).FirstOrDefault() == PMGSYSession.Current.RoleCode)
                                  (PMGSYSession.Current.RoleCode == 57 || PMGSYSession.Current.RoleCode == 58) 
                                    ? (item.REQUEST_FINALIZE == "Y" && !dbContext.OFP_REQUEST_APPROVAL.Any(m => m.REQUEST_ID == d.REQUEST_ID)) 
                                    : (1 == 1)//(dbContext.OFP_REQUEST_APPROVAL.Where(m => m.REQUEST_ID == d.REQUEST_ID).OrderByDescending(m => m.REQUEST_APPROVAL_ID).Select(m => m.REQUEST_FORWADED_TO).FirstOrDefault() == PMGSYSession.Current.RoleCode)
                                  select new
                                  {
                                      item.MASTER_STATE.MAST_STATE_NAME,
                                      item.MASTER_YEAR.MAST_YEAR_TEXT,
                                      item.MASTER_BATCH.MAST_BATCH_NAME,
                                      item.ADMIN_DEPARTMENT.ADMIN_ND_NAME,
                                      item.FILE_DATE,
                                      item.FILE_NO,
                                      item.MAST_PMGSY_SCHEME,
                                      item.REQUEST_AMOUNT,
                                      item.REQUEST_ID,
                                      item.REQUEST_FINALIZE,
                                      item.MAST_YEAR,
                                      item.MAST_STATE_CODE,
                                      item.IMS_BATCH,
                                      item.IMS_COLLABORATION,
                                      item.ADMIN_ND_CODE,
                                      item.REQUEST_INSTALLMENT
                                  }).Distinct();



                totalRecords = lstDetails.Count();

                if (sidx.Trim() != string.Empty)
                {
                    lstDetails = lstDetails.OrderByDescending(m => m.FILE_NO).Skip(page * rows).Take(rows);
                }
                else
                {
                    lstDetails = lstDetails.OrderBy(m => m.FILE_NO).Skip(page * rows).Take(rows);
                }

                var jsonResult = lstDetails.Select(requestDetails => new
                {
                    requestDetails.ADMIN_ND_NAME,
                    requestDetails.FILE_DATE,
                    requestDetails.FILE_NO,
                    requestDetails.MAST_BATCH_NAME,
                    requestDetails.MAST_PMGSY_SCHEME,
                    requestDetails.MAST_STATE_NAME,
                    requestDetails.MAST_YEAR_TEXT,
                    requestDetails.REQUEST_AMOUNT,
                    requestDetails.REQUEST_ID,
                    requestDetails.REQUEST_FINALIZE,
                    requestDetails.MAST_YEAR,
                    requestDetails.IMS_BATCH,
                    requestDetails.IMS_COLLABORATION,
                    requestDetails.MAST_STATE_CODE,
                    requestDetails.ADMIN_ND_CODE,
                    requestDetails.REQUEST_INSTALLMENT

                }).ToArray();

                return jsonResult.Select(result => new
                {
                    cell = new[] 
                    {
                        result.MAST_STATE_NAME.ToString(),
                        result.MAST_YEAR_TEXT.ToString(),
                        result.MAST_BATCH_NAME.ToString(),
                        result.ADMIN_ND_NAME.ToString(),
                        result.MAST_PMGSY_SCHEME == 1 ? "PMGSY I" : "PMGSY II",
                        result.REQUEST_INSTALLMENT.ToString(),
                        dbContext.IMS_SANCTIONED_PROJECTS.Where(m=> m.ADMIN_DEPARTMENT.MAST_PARENT_ND_CODE == result.ADMIN_ND_CODE && m.IMS_YEAR == result.MAST_YEAR && m.IMS_BATCH == result.IMS_BATCH && m.MAST_STATE_CODE == result.MAST_STATE_CODE && m.MAST_PMGSY_SCHEME == result.MAST_PMGSY_SCHEME && m.IMS_PROPOSAL_TYPE == "P" && m.IMS_SANCTIONED == "Y").Count().ToString(),
                        dbContext.IMS_SANCTIONED_PROJECTS.Where(m=> m.ADMIN_DEPARTMENT.MAST_PARENT_ND_CODE == result.ADMIN_ND_CODE && m.IMS_YEAR == result.MAST_YEAR && m.IMS_BATCH == result.IMS_BATCH && m.MAST_STATE_CODE == result.MAST_STATE_CODE && m.MAST_PMGSY_SCHEME == result.MAST_PMGSY_SCHEME && m.IMS_PROPOSAL_TYPE == "L" && m.IMS_SANCTIONED == "Y").Count().ToString(),
                        (
                            from item in dbContext.IMS_SANCTIONED_PROJECTS
                            where 
                            item.IMS_BATCH == result.IMS_BATCH &&
                            item.IMS_YEAR == result.MAST_YEAR &&
                            item.IMS_COLLABORATION == result.IMS_COLLABORATION &&
                            item.MAST_STATE_CODE == result.MAST_STATE_CODE &&
                            item.MAST_PMGSY_SCHEME == result.MAST_PMGSY_SCHEME
                            group item by 1 into lstGroup
                            select new
                            {
                                SanctionAmount = result.MAST_PMGSY_SCHEME == 1 ? lstGroup.Sum(m=>m.IMS_SANCTIONED_PAV_AMT) + lstGroup.Sum(m=>m.IMS_SANCTIONED_PW_AMT) + lstGroup.Sum(m=>m.IMS_SANCTIONED_OW_AMT) + lstGroup.Sum(m=>m.IMS_SANCTIONED_CD_AMT) + lstGroup.Sum(m=>m.IMS_SANCTIONED_BW_AMT) : lstGroup.Sum(m=>m.IMS_SANCTIONED_PAV_AMT) + lstGroup.Sum(m=>m.IMS_SANCTIONED_PW_AMT) + lstGroup.Sum(m=>m.IMS_SANCTIONED_OW_AMT) + lstGroup.Sum(m=>m.IMS_SANCTIONED_CD_AMT) + lstGroup.Sum(m=>m.IMS_SANCTIONED_FC_AMT)
                            }
                        ).Select(m=>m.SanctionAmount).FirstOrDefault().ToString(),
                        result.FILE_NO == null ? "" : result.FILE_NO.ToString(),
                        result.FILE_DATE.HasValue ? objCommon.GetDateTimeToString(result.FILE_DATE.Value) : "",
                        result.REQUEST_AMOUNT== null? "": result.REQUEST_AMOUNT.ToString(),
                        //"<a href='#' title='Click here to upload document details' class='ui-icon ui-icon-plusthick ui-align-center' onClick=UploadDetails('" + URLEncrypt.EncryptParameters(new string[]{ result.REQUEST_ID.ToString().Trim()}) +"'); return false;'>View</a>",
                        //"<a href='#' title='Click here to view proposal details' class='ui-icon ui-icon-zoomin ui-align-center' onClick=ViewProposalDetails('" + URLEncrypt.EncryptParameters(new string[]{ result.REQUEST_ID.ToString().Trim()}) +"'); return false;'>View</a>",
                        "<a href='#' title='Click here to view request observation' class='ui-icon ui-icon-zoomin ui-align-center' onClick=AddObservationDetails('" + URLEncrypt.EncryptParameters(new string[]{ result.REQUEST_ID.ToString().Trim()}) +"'); return false;'>View</a>"
                    }
                }).ToArray();
            }
            catch (Exception)
            {
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

        /// <summary>
        /// returns the list of requests which are in progress
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array GetInProgressRequestListDAL(int page, int rows, string sidx, string sord, out long totalRecords)
        {
            dbContext = new PMGSYEntities();

            try
            {
                dbContext = new PMGSYEntities();

                try
                {
                    List<int> lstUpperRoles = dbContext.OFP_ROLE_REQUEST_FORWARD_MAPPING.Where(m => m.ROLE_ID == PMGSYSession.Current.RoleCode).Select(m => m.FORWARD_ROLE_ID).ToList();

                    var lstDetails = (from item in dbContext.OFP_REQUEST_MASTER
                                      join approval in dbContext.OFP_REQUEST_APPROVAL on item.REQUEST_ID equals approval.REQUEST_ID
                                      where
                                      item.REQUEST_FINALIZE == "Y" &&
                                      lstUpperRoles.Contains(approval.REQUEST_FORWADED_TO.Value)
                                      select new
                                      {
                                          item.MASTER_STATE.MAST_STATE_NAME,
                                          item.MASTER_YEAR.MAST_YEAR_TEXT,
                                          item.MASTER_BATCH.MAST_BATCH_NAME,
                                          item.ADMIN_DEPARTMENT.ADMIN_ND_NAME,
                                          item.FILE_DATE,
                                          item.FILE_NO,
                                          item.MAST_PMGSY_SCHEME,
                                          item.REQUEST_AMOUNT,
                                          item.REQUEST_ID,
                                          item.REQUEST_FINALIZE,
                                          item.MAST_YEAR,
                                          item.MAST_STATE_CODE,
                                          item.IMS_BATCH,
                                          item.IMS_COLLABORATION,
                                          item.ADMIN_ND_CODE,
                                          item.REQUEST_INSTALLMENT
                                      }).Distinct();



                    totalRecords = lstDetails.Count();

                    if (sidx.Trim() != string.Empty)
                    {
                        lstDetails = lstDetails.OrderByDescending(m => m.FILE_NO).Skip(page * rows).Take(rows);
                    }
                    else
                    {
                        lstDetails = lstDetails.OrderBy(m => m.FILE_NO).Skip(page * rows).Take(rows);
                    }

                    var jsonResult = lstDetails.Select(requestDetails => new
                    {
                        requestDetails.ADMIN_ND_NAME,
                        requestDetails.FILE_DATE,
                        requestDetails.FILE_NO,
                        requestDetails.MAST_BATCH_NAME,
                        requestDetails.MAST_PMGSY_SCHEME,
                        requestDetails.MAST_STATE_NAME,
                        requestDetails.MAST_YEAR_TEXT,
                        requestDetails.REQUEST_AMOUNT,
                        requestDetails.REQUEST_ID,
                        requestDetails.REQUEST_FINALIZE,
                        requestDetails.MAST_YEAR,
                        requestDetails.IMS_BATCH,
                        requestDetails.IMS_COLLABORATION,
                        requestDetails.MAST_STATE_CODE,
                        requestDetails.ADMIN_ND_CODE,
                        requestDetails.REQUEST_INSTALLMENT

                    }).ToArray();

                    return jsonResult.Select(result => new
                    {
                        cell = new[] 
                    {
                        result.MAST_STATE_NAME.ToString(),
                        result.MAST_YEAR_TEXT.ToString(),
                        result.MAST_BATCH_NAME.ToString(),
                        result.ADMIN_ND_NAME.ToString(),
                        result.MAST_PMGSY_SCHEME == 1 ? "PMGSY I" : "PMGSY II",
                        result.REQUEST_INSTALLMENT.ToString(),
                        dbContext.IMS_SANCTIONED_PROJECTS.Where(m=> m.ADMIN_DEPARTMENT.MAST_PARENT_ND_CODE == result.ADMIN_ND_CODE && m.IMS_YEAR == result.MAST_YEAR && m.IMS_BATCH == result.IMS_BATCH && m.MAST_STATE_CODE == result.MAST_STATE_CODE && m.MAST_PMGSY_SCHEME == result.MAST_PMGSY_SCHEME && m.IMS_PROPOSAL_TYPE == "P" && m.IMS_SANCTIONED == "Y").Count().ToString(),
                        dbContext.IMS_SANCTIONED_PROJECTS.Where(m=> dbContext.ADMIN_DEPARTMENT.Where(a=>a.MAST_PARENT_ND_CODE == result.ADMIN_ND_CODE && a.MAST_ND_TYPE == "D").Select(a=>a.ADMIN_ND_CODE).Contains(m.MAST_DPIU_CODE) && m.IMS_YEAR == result.MAST_YEAR && m.IMS_BATCH == result.IMS_BATCH && m.MAST_STATE_CODE == result.MAST_STATE_CODE && m.MAST_PMGSY_SCHEME == result.MAST_PMGSY_SCHEME && m.IMS_PROPOSAL_TYPE == "L" && m.IMS_SANCTIONED == "Y").Count().ToString(),
                        (
                            from item in dbContext.IMS_SANCTIONED_PROJECTS
                            where 
                            item.IMS_BATCH == result.IMS_BATCH &&
                            item.IMS_YEAR == result.MAST_YEAR &&
                            item.IMS_COLLABORATION == result.IMS_COLLABORATION &&
                            item.MAST_STATE_CODE == result.MAST_STATE_CODE &&
                            item.MAST_PMGSY_SCHEME == result.MAST_PMGSY_SCHEME
                            group item by 1 into lstGroup
                            select new
                            {
                                SanctionAmount = result.MAST_PMGSY_SCHEME == 1 ? lstGroup.Sum(m=>m.IMS_SANCTIONED_PAV_AMT) + lstGroup.Sum(m=>m.IMS_SANCTIONED_PW_AMT) + lstGroup.Sum(m=>m.IMS_SANCTIONED_OW_AMT) + lstGroup.Sum(m=>m.IMS_SANCTIONED_CD_AMT) + lstGroup.Sum(m=>m.IMS_SANCTIONED_BW_AMT) : lstGroup.Sum(m=>m.IMS_SANCTIONED_PAV_AMT) + lstGroup.Sum(m=>m.IMS_SANCTIONED_PW_AMT) + lstGroup.Sum(m=>m.IMS_SANCTIONED_OW_AMT) + lstGroup.Sum(m=>m.IMS_SANCTIONED_CD_AMT) + lstGroup.Sum(m=>m.IMS_SANCTIONED_FC_AMT)
                            }
                        ).Select(m=>m.SanctionAmount).FirstOrDefault().ToString(),
                        result.FILE_NO == null ? "" : result.FILE_NO.ToString(),
                        result.FILE_DATE.HasValue ? objCommon.GetDateTimeToString(result.FILE_DATE.Value) : "",
                        result.REQUEST_AMOUNT== null? "": result.REQUEST_AMOUNT.ToString(),
                        "<a href='#' title='Click here to view request details' class='ui-icon ui-icon-zoomin ui-align-center' onClick=ViewRequestDetails('" + URLEncrypt.EncryptParameters(new string[]{ result.REQUEST_ID.ToString().Trim()}) +"'); return false;'>View</a>",
                    }



                    }).ToArray();
                }
                catch (Exception)
                {
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
            catch (Exception)
            {
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

        /// <summary>
        /// returns the list of completed requests
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array GetCompletedRequestListDAL(int page, int rows, string sidx, string sord, out long totalRecords)
        {
            dbContext = new PMGSYEntities();

            try
            {

                List<int> lstUpperRoles = dbContext.OFP_ROLE_REQUEST_FORWARD_MAPPING.Where(m => m.ROLE_ID == PMGSYSession.Current.RoleCode).Select(m => m.FORWARD_ROLE_ID).ToList();

                var lstDetails = (from item in dbContext.OFP_REQUEST_MASTER
                                  join approval in dbContext.OFP_REQUEST_APPROVAL on item.REQUEST_ID equals approval.REQUEST_ID
                                  where
                                  item.REQUEST_FINALIZE == "Y" &&
                                  approval.APPROVAL_STATUS == "A" &&
                                  approval.UM_User_Master.DefaultRoleID == 59
                                  select new
                                  {
                                      item.MASTER_STATE.MAST_STATE_NAME,
                                      item.MASTER_YEAR.MAST_YEAR_TEXT,
                                      item.MASTER_BATCH.MAST_BATCH_NAME,
                                      item.ADMIN_DEPARTMENT.ADMIN_ND_NAME,
                                      item.FILE_DATE,
                                      item.FILE_NO,
                                      item.MAST_PMGSY_SCHEME,
                                      item.REQUEST_AMOUNT,
                                      item.REQUEST_ID,
                                      item.REQUEST_FINALIZE,
                                      item.MAST_YEAR,
                                      item.MAST_STATE_CODE,
                                      item.IMS_BATCH,
                                      item.IMS_COLLABORATION,
                                      item.ADMIN_ND_CODE,
                                      item.REQUEST_INSTALLMENT
                                  }).Distinct();



                totalRecords = lstDetails.Count();

                if (sidx.Trim() != string.Empty)
                {
                    lstDetails = lstDetails.OrderByDescending(m => m.FILE_NO).Skip(page * rows).Take(rows);
                }
                else
                {
                    lstDetails = lstDetails.OrderBy(m => m.FILE_NO).Skip(page * rows).Take(rows);
                }

                var jsonResult = lstDetails.Select(requestDetails => new
                {
                    requestDetails.ADMIN_ND_NAME,
                    requestDetails.FILE_DATE,
                    requestDetails.FILE_NO,
                    requestDetails.MAST_BATCH_NAME,
                    requestDetails.MAST_PMGSY_SCHEME,
                    requestDetails.MAST_STATE_NAME,
                    requestDetails.MAST_YEAR_TEXT,
                    requestDetails.REQUEST_AMOUNT,
                    requestDetails.REQUEST_ID,
                    requestDetails.REQUEST_FINALIZE,
                    requestDetails.MAST_YEAR,
                    requestDetails.IMS_BATCH,
                    requestDetails.IMS_COLLABORATION,
                    requestDetails.MAST_STATE_CODE,
                    requestDetails.ADMIN_ND_CODE,
                    requestDetails.REQUEST_INSTALLMENT

                }).ToArray();

                return jsonResult.Select(result => new
                {
                    cell = new[] 
                    {
                        result.MAST_STATE_NAME.ToString(),
                        result.MAST_YEAR_TEXT.ToString(),
                        result.MAST_BATCH_NAME.ToString(),
                        result.ADMIN_ND_NAME.ToString(),
                        result.MAST_PMGSY_SCHEME == 1 ? "PMGSY I" : "PMGSY II",
                        result.REQUEST_INSTALLMENT.ToString(),
                        dbContext.IMS_SANCTIONED_PROJECTS.Where(m=> m.ADMIN_DEPARTMENT.MAST_PARENT_ND_CODE == result.ADMIN_ND_CODE && m.IMS_YEAR == result.MAST_YEAR && m.IMS_BATCH == result.IMS_BATCH && m.MAST_STATE_CODE == result.MAST_STATE_CODE && m.MAST_PMGSY_SCHEME == result.MAST_PMGSY_SCHEME && m.IMS_PROPOSAL_TYPE == "P" && m.IMS_SANCTIONED == "Y").Count().ToString(),
                        dbContext.IMS_SANCTIONED_PROJECTS.Where(m=> m.ADMIN_DEPARTMENT.MAST_PARENT_ND_CODE == result.ADMIN_ND_CODE && m.IMS_YEAR == result.MAST_YEAR && m.IMS_BATCH == result.IMS_BATCH && m.MAST_STATE_CODE == result.MAST_STATE_CODE && m.MAST_PMGSY_SCHEME == result.MAST_PMGSY_SCHEME && m.IMS_PROPOSAL_TYPE == "L" && m.IMS_SANCTIONED == "Y").Count().ToString(),
                        (
                            from item in dbContext.IMS_SANCTIONED_PROJECTS
                            where 
                            item.IMS_BATCH == result.IMS_BATCH &&
                            item.IMS_YEAR == result.MAST_YEAR &&
                            item.IMS_COLLABORATION == result.IMS_COLLABORATION &&
                            item.MAST_STATE_CODE == result.MAST_STATE_CODE &&
                            item.MAST_PMGSY_SCHEME == result.MAST_PMGSY_SCHEME
                            group item by 1 into lstGroup
                            select new
                            {
                                SanctionAmount = result.MAST_PMGSY_SCHEME == 1 ? lstGroup.Sum(m=>m.IMS_SANCTIONED_PAV_AMT) + lstGroup.Sum(m=>m.IMS_SANCTIONED_PW_AMT) + lstGroup.Sum(m=>m.IMS_SANCTIONED_OW_AMT) + lstGroup.Sum(m=>m.IMS_SANCTIONED_CD_AMT) + lstGroup.Sum(m=>m.IMS_SANCTIONED_BW_AMT) : lstGroup.Sum(m=>m.IMS_SANCTIONED_PAV_AMT) + lstGroup.Sum(m=>m.IMS_SANCTIONED_PW_AMT) + lstGroup.Sum(m=>m.IMS_SANCTIONED_OW_AMT) + lstGroup.Sum(m=>m.IMS_SANCTIONED_CD_AMT) + lstGroup.Sum(m=>m.IMS_SANCTIONED_FC_AMT)
                            }
                        ).Select(m=>m.SanctionAmount).FirstOrDefault().ToString(),
                        result.FILE_NO == null ? "" : result.FILE_NO.ToString(),
                        result.FILE_DATE.HasValue ? objCommon.GetDateTimeToString(result.FILE_DATE.Value) : "",
                        result.REQUEST_AMOUNT== null? "": result.REQUEST_AMOUNT.ToString(),
                        "<a href='#' title='Click here to view request details' class='ui-icon ui-icon-zoomin ui-align-center' onClick=ViewRequestDetails('" + URLEncrypt.EncryptParameters(new string[]{ result.REQUEST_ID.ToString().Trim()}) +"'); return false;'>View</a>",
                    }



                }).ToArray();
            }
            catch (Exception)
            {
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

        /// <summary>
        /// returns the list of approved requests
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array GetApprovedRequestListDAL(int page, int rows, string sidx, string sord, out long totalRecords)
        {
            dbContext = new PMGSYEntities();

            try
            {
                var lstDetails = (from item in dbContext.OFP_REQUEST_MASTER
                                  join approval in dbContext.OFP_REQUEST_APPROVAL on item.REQUEST_ID equals approval.REQUEST_ID
                                  where
                                  //dbContext.OFP_REQUEST_APPROVAL.Any(m=>m.REQUEST_ID == item.REQUEST_ID && m.APPROVAL_STATUS == "C" && m.REQUEST_FORWADED_FROM == 59) &&
                                  dbContext.OFP_REQUEST_APPROVAL.Any(m=>m.REQUEST_ID == item.REQUEST_ID && m.APPROVAL_STATUS == "C") &&
                                  PMGSYSession.Current.RoleCode == 51 ? (1==1) : item.RELEASE_UO_NO != null
                                  select new
                                  {
                                      item.MASTER_STATE.MAST_STATE_NAME,
                                      item.MASTER_YEAR.MAST_YEAR_TEXT,
                                      item.MASTER_BATCH.MAST_BATCH_NAME,
                                      item.ADMIN_DEPARTMENT.ADMIN_ND_NAME,
                                      item.FILE_DATE,
                                      item.FILE_NO,
                                      item.MAST_PMGSY_SCHEME,
                                      item.REQUEST_AMOUNT,
                                      item.REQUEST_ID,
                                      item.REQUEST_FINALIZE,
                                      item.MAST_YEAR,
                                      item.MAST_STATE_CODE,
                                      item.IMS_BATCH,
                                      item.IMS_COLLABORATION,
                                      item.ADMIN_ND_CODE,
                                      item.REQUEST_INSTALLMENT
                                  }).Distinct();



                totalRecords = lstDetails.Count();

                if (sidx.Trim() != string.Empty)
                {
                    lstDetails = lstDetails.OrderByDescending(m => m.FILE_NO).Skip(page * rows).Take(rows);
                }
                else
                {
                    lstDetails = lstDetails.OrderBy(m => m.FILE_NO).Skip(page * rows).Take(rows);
                }

                var jsonResult = lstDetails.Select(requestDetails => new
                {
                    requestDetails.ADMIN_ND_NAME,
                    requestDetails.FILE_DATE,
                    requestDetails.FILE_NO,
                    requestDetails.MAST_BATCH_NAME,
                    requestDetails.MAST_PMGSY_SCHEME,
                    requestDetails.MAST_STATE_NAME,
                    requestDetails.MAST_YEAR_TEXT,
                    requestDetails.REQUEST_AMOUNT,
                    requestDetails.REQUEST_ID,
                    requestDetails.REQUEST_FINALIZE,
                    requestDetails.MAST_YEAR,
                    requestDetails.IMS_BATCH,
                    requestDetails.IMS_COLLABORATION,
                    requestDetails.MAST_STATE_CODE,
                    requestDetails.ADMIN_ND_CODE,
                    requestDetails.REQUEST_INSTALLMENT

                }).ToArray();

                return jsonResult.Select(result => new
                {
                    cell = new[] 
                    {
                        result.MAST_STATE_NAME.ToString(),
                        result.MAST_YEAR_TEXT.ToString(),
                        result.MAST_BATCH_NAME.ToString(),
                        result.ADMIN_ND_NAME.ToString(),
                        result.MAST_PMGSY_SCHEME == 1 ? "PMGSY I" : "PMGSY II",
                        result.REQUEST_INSTALLMENT.ToString(),
                        dbContext.IMS_SANCTIONED_PROJECTS.Where(m=> m.ADMIN_DEPARTMENT.MAST_PARENT_ND_CODE == result.ADMIN_ND_CODE && m.IMS_YEAR == result.MAST_YEAR && m.IMS_BATCH == result.IMS_BATCH && m.MAST_STATE_CODE == result.MAST_STATE_CODE && m.MAST_PMGSY_SCHEME == result.MAST_PMGSY_SCHEME && m.IMS_PROPOSAL_TYPE == "P" && m.IMS_SANCTIONED == "Y").Count().ToString(),
                        dbContext.IMS_SANCTIONED_PROJECTS.Where(m=> m.ADMIN_DEPARTMENT.MAST_PARENT_ND_CODE == result.ADMIN_ND_CODE && m.IMS_YEAR == result.MAST_YEAR && m.IMS_BATCH == result.IMS_BATCH && m.MAST_STATE_CODE == result.MAST_STATE_CODE && m.MAST_PMGSY_SCHEME == result.MAST_PMGSY_SCHEME && m.IMS_PROPOSAL_TYPE == "L" && m.IMS_SANCTIONED == "Y").Count().ToString(),
                        (
                            from item in dbContext.IMS_SANCTIONED_PROJECTS
                            where 
                            item.IMS_BATCH == result.IMS_BATCH &&
                            item.IMS_YEAR == result.MAST_YEAR &&
                            item.IMS_COLLABORATION == result.IMS_COLLABORATION &&
                            item.MAST_STATE_CODE == result.MAST_STATE_CODE &&
                            item.MAST_PMGSY_SCHEME == result.MAST_PMGSY_SCHEME
                            group item by 1 into lstGroup
                            select new
                            {
                                SanctionAmount = result.MAST_PMGSY_SCHEME == 1 ? lstGroup.Sum(m=>m.IMS_SANCTIONED_PAV_AMT) + lstGroup.Sum(m=>m.IMS_SANCTIONED_PW_AMT) + lstGroup.Sum(m=>m.IMS_SANCTIONED_OW_AMT) + lstGroup.Sum(m=>m.IMS_SANCTIONED_CD_AMT) + lstGroup.Sum(m=>m.IMS_SANCTIONED_BW_AMT) : lstGroup.Sum(m=>m.IMS_SANCTIONED_PAV_AMT) + lstGroup.Sum(m=>m.IMS_SANCTIONED_PW_AMT) + lstGroup.Sum(m=>m.IMS_SANCTIONED_OW_AMT) + lstGroup.Sum(m=>m.IMS_SANCTIONED_CD_AMT) + lstGroup.Sum(m=>m.IMS_SANCTIONED_FC_AMT)
                            }
                        ).Select(m=>m.SanctionAmount).FirstOrDefault().ToString(),
                        result.FILE_NO == null ? "" : result.FILE_NO.ToString(),
                        result.FILE_DATE.HasValue ? objCommon.GetDateTimeToString(result.FILE_DATE.Value) : "",
                        result.REQUEST_AMOUNT== null? "": result.REQUEST_AMOUNT.ToString(),
                        "<a href='#' title='Click here to add observation details' class='ui-icon ui-icon-plusthick ui-align-center' onClick=AddRequestUODetails('" + URLEncrypt.EncryptParameters(new string[]{ result.REQUEST_ID.ToString().Trim()}) +"'); return false;'>View</a>",
                    }



                }).ToArray();
            }
            catch (Exception)
            {
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

        /// <summary>
        /// returns the list of Conditions
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> PopulateConditions()
        {
            dbContext = new PMGSYEntities();

            try
            {
                return new SelectList(dbContext.OFP_CONDITION_MASTER.ToList(), "CONDITION_ID", "CONDITION_DESCRIPTION").ToList();
            }
            catch (Exception)
            {
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
        /// returns true or false depending on whether the condition is imposed or not
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsConditionImposedDAL(string id)
        {
            dbContext = new PMGSYEntities();
            
            int AdminCode = 0;
            int Year = 0;
            int Batch = 0;
            int Collaboration = 0;
            int Scheme = 0;
            try
            {
                if (String.IsNullOrEmpty(id))
                {
                    return false;
                }
                else
                {
                    Year = Convert.ToInt32(id.Split('$')[0]);
                    Batch = Convert.ToInt32(id.Split('$')[1]);
                    Collaboration = Convert.ToInt32(id.Split('$')[2]);
                    Scheme = Convert.ToInt32(id.Split('$')[3]);
                    AdminCode = PMGSYSession.Current.AdminNdCode;

                    if (dbContext.OFP_REQUEST_MASTER.Any(m => m.MAST_STATE_CODE == PMGSYSession.Current.StateCode && m.MAST_YEAR == Year && m.IMS_BATCH == Batch && m.IMS_COLLABORATION == Collaboration && m.MAST_PMGSY_SCHEME == Scheme && m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode))
                    {
                        OFP_REQUEST_MASTER requestMaster = dbContext.OFP_REQUEST_MASTER.Where(m => m.MAST_STATE_CODE == PMGSYSession.Current.StateCode && m.MAST_YEAR == Year && m.IMS_BATCH == Batch && m.IMS_COLLABORATION == Collaboration && m.MAST_PMGSY_SCHEME == Scheme && m.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).OrderByDescending(m => m.REQUEST_ID).FirstOrDefault();
                        if (dbContext.OFP_CONDITION_IMPOSED.Any(m => m.REQUEST_ID == requestMaster.REQUEST_ID && m.CONDITION_STATUS == "N"))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch (Exception)
            {
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
        /// saves the details of condition reply
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddConditionReplyDAL(ConditionReplyViewModel model)
        {
            dbContext = new PMGSYEntities();

            try
            {
                OFP_CONDITION_IMPOSED conditionDetails = dbContext.OFP_CONDITION_IMPOSED.Find(model.REQUEST_ID);
                if (conditionDetails != null)
                {
                    conditionDetails.CONDITION_STATUS = "C";
                    conditionDetails.CONDITION_REPLY = model.CONDITION_REPLY;
                    dbContext.Entry(conditionDetails).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
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
        /// returns the request details
        /// </summary>
        /// <returns></returns>
        public List<string> GetRequestDetails()
        {
            dbContext = new PMGSYEntities();

            try
            {
                var lstDetails = (from requestDetails in dbContext.OFP_REQUEST_MASTER
                                  join requestArroval in dbContext.OFP_REQUEST_APPROVAL on requestDetails.REQUEST_ID equals requestArroval.REQUEST_ID
                                  where 
                                  PMGSYSession.Current.RoleCode == 2 ? (requestDetails.MAST_STATE_CODE == PMGSYSession.Current.StateCode) : (1 == 1) &&
                                  !(requestArroval.REQUEST_FORWADED_FROM == 59 && requestArroval.APPROVAL_STATUS == "A")
                                  orderby requestArroval.REQUEST_APPROVAL_ID
                                  select new 
                                  {
                                      requestDetails.MASTER_STATE.MAST_STATE_NAME,
                                      requestDetails.ADMIN_DEPARTMENT.ADMIN_ND_NAME,
                                      requestDetails.MASTER_YEAR.MAST_YEAR_TEXT,
                                      requestDetails.MASTER_BATCH.MAST_BATCH_NAME,
                                      requestDetails.MASTER_STREAMS.MAST_STREAM_NAME,
                                      requestDetails.REQUEST_INSTALLMENT,
                                      requestDetails.MAST_PMGSY_SCHEME,
                                      requestArroval.APPROVAL_STATUS,
                                      ForwardToRole = dbContext.UM_Role_Master.Where(m=>m.RoleID == requestArroval.REQUEST_FORWADED_TO.Value).Select(a=>a.RoleName).FirstOrDefault(),
                                      ForwardFromRole = dbContext.UM_Role_Master.Where(m=>m.RoleID == requestArroval.REQUEST_FORWADED_FROM.Value).Select(a=>a.RoleName).FirstOrDefault()
                                  }).Take(20);

                List<string> lstData = new List<string>();

                foreach (var item in lstDetails)
                {
                    lstData.Add("Request of State " + item.MAST_STATE_NAME + " with Agency " + item.ADMIN_ND_NAME + " of Year : " + item.MAST_YEAR_TEXT + ", Batch : " + item.MAST_BATCH_NAME + ", Collaboration : " + item.MAST_STREAM_NAME + ", Installment No. :" + item.REQUEST_INSTALLMENT + ", PMGSY Scheme :" + item.MAST_PMGSY_SCHEME + " has been " + (item.APPROVAL_STATUS == "A" ? "Approved and Forwarded to " : (item.APPROVAL_STATUS == "F" ? "Forwarded to " : "Rejected by ")) + (item.APPROVAL_STATUS == "A" ? item.ForwardToRole : (item.APPROVAL_STATUS == "F" ? item.ForwardToRole : item.ForwardFromRole)));
                }

                return lstData;
            }
            catch (Exception)
            {
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
        /// returns the list of batches according to the year and state
        /// </summary>
        /// <param name="yearCode"></param>
        /// <returns></returns>
        public List<SelectListItem> PopulateBatchByYear(int yearCode)
        {
            List<SelectListItem> lstBatches = new List<SelectListItem>();
            dbContext = new PMGSYEntities();
            try
            {
                var lstBatch = (
                                    from item in dbContext.IMS_SANCTIONED_PROJECTS
                                    where item.MAST_STATE_CODE == PMGSYSession.Current.StateCode &&
                                    item.IMS_YEAR == yearCode
                                    select new
                                    {
                                        BATCH_CODE = item.IMS_BATCH,
                                        BATCH_NAME = item.IMS_BATCH
                                    }
                                ).Distinct().ToList();

                lstBatches.Insert(0, new SelectListItem { Value = "0" , Text = "Select Batch"});

                foreach (var item in lstBatch)
                {
                    lstBatches.Add(new SelectListItem { Value = item.BATCH_CODE.ToString() , Text = "Batch " + item.BATCH_NAME.ToString()});
                }

                return lstBatches;
            }
            catch (Exception)
            {
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
        /// reenter the old request details with new request id
        /// </summary>
        /// <returns></returns>
        public bool RegenerateRequestDetails(int requestId)
        {
            dbContext = new PMGSYEntities();

            try
            {
                OFP_REQUEST_MASTER oldRequestDetails = dbContext.OFP_REQUEST_MASTER.Find(requestId);
                OFP_REQUEST_MASTER newRequestDetails = new OFP_REQUEST_MASTER();
                if (oldRequestDetails != null)
                {
                    newRequestDetails.ADMIN_ND_CODE = oldRequestDetails.ADMIN_ND_CODE;
                    newRequestDetails.APPROVAL_CONDITION_IMPOSSED = null;
                    newRequestDetails.ELIGIBLE_FOR_NEXT_REQUEST = "N";
                    newRequestDetails.IMS_BATCH = oldRequestDetails.IMS_BATCH;
                    newRequestDetails.IMS_COLLABORATION = oldRequestDetails.IMS_COLLABORATION;
                    newRequestDetails.MAST_FUND_TYPE = "P";
                    newRequestDetails.MAST_PMGSY_SCHEME = oldRequestDetails.MAST_PMGSY_SCHEME;
                    newRequestDetails.MAST_STATE_CODE = oldRequestDetails.MAST_STATE_CODE;
                    newRequestDetails.MAST_YEAR = oldRequestDetails.MAST_YEAR;
                    newRequestDetails.OFP_CONDITION_IMPOSED = null;
                    newRequestDetails.OFP_REQUEST_APPROVAL = null;
                    newRequestDetails.PREVIOUS_CONDITION_IMPOSSED = null;
                    newRequestDetails.RELEASE_AMOUNT = null;
                    newRequestDetails.RELEASE_DATE = null;
                    newRequestDetails.REQUEST_AMOUNT = oldRequestDetails.REQUEST_AMOUNT;
                    newRequestDetails.REQUEST_FINALIZE = "N";
                    newRequestDetails.REQUEST_FINALIZE_DATE = null;
                    newRequestDetails.REQUEST_INITIATE_DATE = DateTime.Now;
                    newRequestDetails.OLD_REQUEST_ID = oldRequestDetails.REQUEST_ID;
                    newRequestDetails.REQUEST_REMARKS = oldRequestDetails.REQUEST_REMARKS;
                    newRequestDetails.FILE_NO = null;
                    newRequestDetails.FILE_DATE = null;


                    dbContext.OFP_REQUEST_MASTER.Add(newRequestDetails);
                    dbContext.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
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
        /// saves the observation details and uo number for the request.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddUODetailsDAL(OnlineFundRequestViewModel model)
        {
            string[] decryptedParameters = null;
            int requestCode = 0;
            dbContext = new PMGSYEntities();
            try
            {
                if (!String.IsNullOrEmpty(model.EncRequestId.Split('/')[0]) && !String.IsNullOrEmpty(model.EncRequestId.Split('/')[1]) && !String.IsNullOrEmpty(model.EncRequestId.Split('/')[2]))
                {
                    decryptedParameters = URLEncrypt.DecryptParameters(new string[] { model.EncRequestId.Split('/')[0], model.EncRequestId.Split('/')[1], model.EncRequestId.Split('/')[2] });
                }

                requestCode = Convert.ToInt32(decryptedParameters[0]);

                OFP_REQUEST_MASTER requestDetails = dbContext.OFP_REQUEST_MASTER.Find(requestCode);

                if (requestDetails != null)
                {
                    requestDetails.RELEASE_UO_NO = model.RELEASE_UO_NO;
                    requestDetails.RELEASE_DATE = objCommon.GetStringToDateTime(model.RELEASE_DATE);
                    requestDetails.RELEASE_AMOUNT = model.RELEASE_AMOUNT;
                    requestDetails.SANCTION_AMOUNT = model.SANCTION_AMOUNT;
                    requestDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    requestDetails.USERID = PMGSYSession.Current.UserId;
                    dbContext.Entry(requestDetails).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
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
        /// returns whether the release no is generated or not
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public string IsReleaseNoGenerated(int requestId)
        {
            dbContext = new PMGSYEntities();

            try
            {
                if (dbContext.OFP_REQUEST_MASTER.Where(m => m.REQUEST_ID == requestId).Select(m => m.RELEASE_UO_NO).FirstOrDefault() != null)
                {
                    return "Y";
                }
                else
                {
                    return "N";
                }
            }
            catch (Exception)
            {
                return "N";
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
        /// deletes the entry of document details
        /// </summary>
        /// <param name="requestDocumentId"></param>
        /// <returns></returns>
        public bool DeleteDocumentDetailsDAL(int requestDocumentId)
        {
            dbContext = new PMGSYEntities();

            try
            {
                OFP_REQUEST_DOCUMENT_MAPPING documentDetails = dbContext.OFP_REQUEST_DOCUMENT_MAPPING.Find(requestDocumentId);
                if (documentDetails != null)
                {
                    documentDetails.USERID = PMGSYSession.Current.UserId;
                    documentDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    dbContext.Entry(documentDetails).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    dbContext.OFP_REQUEST_DOCUMENT_MAPPING.Remove(documentDetails);
                    dbContext.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
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
        /// returns the list of conditions imposed on the requests
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="requestCode"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array GetConditionImposedListDAL(int page, int rows, string sidx, string sord, int requestCode, out long totalRecords)
        {
            dbContext = new PMGSYEntities();

            try
            {
                var lstDocuments = dbContext.OFP_CONDITION_IMPOSED.Where(m => m.REQUEST_ID == requestCode).ToList();

                totalRecords = lstDocuments.Count();

                if (!String.IsNullOrEmpty(sidx))
                {
                    if (sord == "asc")
                    {
                        lstDocuments = lstDocuments.OrderBy(m => m.UM_User_Master.DefaultRoleID).Skip(page * rows).Take(rows).ToList();
                    }
                    else
                    {
                        lstDocuments = lstDocuments.OrderByDescending(m => m.UM_User_Master.DefaultRoleID).Skip(page * rows).Take(rows).ToList();
                    }
                }
                else
                {
                    lstDocuments = lstDocuments.OrderBy(m => m.UM_User_Master.DefaultRoleID).Skip(page * rows).Take(rows).ToList();
                }

                var jsonResult = lstDocuments.Select(m => new
                {
                    m.CONDITION_IMPOSED_BY,
                    m.CONDITION_IMPOSED_DATE,
                    m.CONDITION_ID,
                }).ToArray();

                return jsonResult.Select(result => new
                {
                    cell = new[] 
                        {
                            dbContext.UM_Role_Master.Where(m=>m.RoleID == result.CONDITION_IMPOSED_BY).Select(m=>m.RoleName).FirstOrDefault(),
                            result.CONDITION_IMPOSED_DATE.Value.ToString("dd/MM/yyyy"),
                            dbContext.OFP_CONDITION_MASTER.Where(m=>m.CONDITION_ID == result.CONDITION_ID).Select(m=>m.CONDITION_DESCRIPTION).FirstOrDefault().ToString()
                        }
                }).ToArray();
            }
            catch (Exception)
            {
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

        /// <summary>
        /// returns the file name for the reject letter uploaded
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public string GetRejectLetterFileName(string requestId)
        {
            dbContext = new PMGSYEntities();
            string[] decryptedParameters = null;
            int requestCode = 0;
            try
            {
                if (!String.IsNullOrEmpty(requestId.Split('/')[0]) && !String.IsNullOrEmpty(requestId.Split('/')[1]) && !String.IsNullOrEmpty(requestId.Split('/')[2]))
                {
                    decryptedParameters = URLEncrypt.DecryptParameters(new string[] { requestId.Split('/')[0], requestId.Split('/')[1], requestId.Split('/')[2] });
                }

                requestCode = Convert.ToInt32(decryptedParameters[0]);

                string fileName = dbContext.OFP_REQUEST_MASTER.Where(m => m.REQUEST_ID == requestCode).Select(m => m.MASTER_STATE.MAST_STATE_SHORT_CODE).FirstOrDefault() + "_" + requestCode + "_RL".Replace(' ', '_');
                return fileName;
            }
            catch (Exception)
            {
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

    }

    public interface IOnlineFundProcessDAL
    {
        Array GetListOfOnlineFundRequestsDAL(int page, int rows, string sidx, string sord, out long totalRecords, int State, int Year, int Batch, int Collaboration, int Agency, int Scheme);
        bool AddOnlineFundRequestDAL(OnlineFundRequestViewModel model,ref string message);
        bool UpdateOnlineFundRequestDAL(OnlineFundRequestViewModel model, ref string message);
        bool DeleteOnlineFundRequestDAL(int requestId);
        OnlineFundRequestViewModel GetOnlineFundRequestDetailsDAL(int requestId);
        bool FinalizeRequestDetailsDAL(int requestId,out string message);
        Array GetProposalListDAL(int page, int rows, string sidx, string sord, out long totalRecords, int requestId);
        bool AddObservationDetailsDAL(RequestApprovalViewModel model);
        bool AddDocumentDetailsDAL(List<DocumentUploadViewModel> lstModels);
        Array GetListOfDocumentsUploadedDAL(int page, int rows, string sidx, string sord, out long totalRecords, int requestId);
        Array GetListofObservationDetailsDAL(int page, int rows, string sidx, string sord, out long totalRecords, int requestId);
        Array GetActionRequiredRequestListDAL(int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetInProgressRequestListDAL(int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetCompletedRequestListDAL(int page, int rows, string sidx, string sord, out long totalRecords);
        bool AddConditionReplyDAL(ConditionReplyViewModel model);
        bool AddUODetailsDAL(OnlineFundRequestViewModel model);
        Array GetApprovedRequestListDAL(int page, int rows, string sidx, string sord, out long totalRecords);
        bool DeleteDocumentDetailsDAL(int requestDocumentId);
        Array GetConditionImposedListDAL(int page, int rows, string sidx, string sord, int requestCode, out long totalRecords);
    }
}