#region HEADER
/*
 * Project Id:

 * Project Name:OMMAS-II

 * File Name: LockUnlockDAL.cs

 * Author : Vikram Nandanwar

 * Creation Date :05/June/2013

 * Desc : This class is used as data access layer to perform Save,Edit,Delete and listing of Lock Unlock screens.  
*/

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.Models;
using PMGSY.Models.LockUnlock;
using PMGSY.Common;
using System.Data.Entity;

using System.Transactions;
using System.Web.Mvc;
using PMGSY.Extensions;
using System.Text;
using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using System.IO;
using Mvc.Mailer;
using System.Configuration;
using System.Data.Entity.Infrastructure;

namespace PMGSY.DAL.LockUnlock
{
    public class LockUnlockDAL : ILockUnlockDAL
    {

        PMGSYEntities dbContext;
        PMGSYEntities dbContextImsSanctionedProjects;
        PMGSYEntities dbContextImsFreezeDetails;

        public Array GetProposalsDAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int IMS_YEAR, int IMS_MAST_STATE_CODE, int IMS_BATCH, int Scheme)
        {
            try
            {
                dbContext = new Models.PMGSYEntities();
                List<IMS_SANCTIONED_PROJECTS> listProposals = (from c in dbContext.IMS_SANCTIONED_PROJECTS
                                                               where
                                                 ((IMS_MAST_STATE_CODE == 0 ? 1 : c.MAST_STATE_CODE) == (IMS_MAST_STATE_CODE == 0 ? 1 : IMS_MAST_STATE_CODE)) &&
                                                ((IMS_YEAR == 0 ? 1 : c.IMS_YEAR) == (IMS_YEAR == 0 ? 1 : IMS_YEAR)) &&
                                                ((IMS_BATCH == 0 ? 1 : c.IMS_BATCH) == (IMS_BATCH == 0 ? 1 : IMS_BATCH)) &&
                                                //c.MAST_PMGSY_SCHEME == Scheme &&
                                                c.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme &&
                                                                   //c.IMS_SANCTIONED == "Y"
                                                                   ///Added by SAMMED A. PATIL on 25JULY2017 for freezing Sanctioned as well as scrutinized proposals 
                                                (c.IMS_SANCTIONED == "Y" || c.STA_SANCTIONED == "Y")
                                                               select c)
                                                               .OrderByDescending(c => c.IMS_PR_ROAD_CODE).ToList<IMS_SANCTIONED_PROJECTS>();

                IQueryable<IMS_SANCTIONED_PROJECTS> query = listProposals.AsQueryable<IMS_SANCTIONED_PROJECTS>();
                totalRecords = listProposals.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "District":
                                query = query.OrderBy(x => x.MASTER_DISTRICT.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;

                            case "Block":
                                query = query.OrderBy(x => x.MASTER_BLOCK.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "PackageNumber":
                                query = query.OrderBy(x => x.IMS_PACKAGE_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "RoadName":
                                query = query.OrderBy(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "Length":
                                query = query.OrderBy(x => x.IMS_PAV_LENGTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "Sanctioned":
                                query = query.OrderBy(x => x.IMS_SANCTIONED).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }

                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "District":
                                query = query.OrderByDescending(x => x.MASTER_DISTRICT.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;

                            case "Block":
                                query = query.OrderByDescending(x => x.MASTER_BLOCK.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "PackageNumber":
                                query = query.OrderByDescending(x => x.IMS_PACKAGE_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "RoadName":
                                query = query.OrderByDescending(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "Length":
                                query = query.OrderByDescending(x => x.IMS_PAV_LENGTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "Sanctioned":
                                query = query.OrderByDescending(x => x.IMS_SANCTIONED).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                }
                else
                {
                    query = query.OrderBy(x => x.MASTER_STATE.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                return query.Select(propDetails => new
                {
                    id = propDetails.IMS_PR_ROAD_CODE.ToString(),
                    IMS_FREEZE_STATUS = propDetails.IMS_FREEZE_STATUS.ToString(),

                    cell = new[] {                         
                                    propDetails.MASTER_DISTRICT.MAST_DISTRICT_NAME==null?"NA":propDetails.MASTER_DISTRICT.MAST_DISTRICT_NAME,
                                    propDetails.MASTER_BLOCK.MAST_BLOCK_NAME==null?"NA":propDetails.MASTER_BLOCK.MAST_BLOCK_NAME,
                                    propDetails.IMS_PACKAGE_ID==null?"NA":propDetails.IMS_PACKAGE_ID,
                                    propDetails.IMS_ROAD_NAME == null ? "NA" :  propDetails.IMS_ROAD_NAME,
                                    propDetails.IMS_PAV_LENGTH== null ? "NA" :  propDetails.IMS_PAV_LENGTH.ToString(),
                                    propDetails.IMS_SANCTIONED==null?"NA":propDetails.IMS_SANCTIONED=="Y"?"Yes":"No",
                                    ///Changes by SAMMED A. PATIL on 25JULY2017 for Freeze STA Scrutinized Proposals/RCPLWE
                                    propDetails.IMS_FREEZE_STATUS == "F" ? "Freezed" : "Unfreezed"
                   }
                }).ToArray();
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "LockUnlock.GetProposalsDAL()");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool FreezeUnfreezeProposal(ProposalFilterLockUnlockViewModel LockUnlockViewModel, ref string message)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                try
                {
                    dbContextImsSanctionedProjects = new PMGSYEntities();
                    int IMS_YEAR = LockUnlockViewModel.IMS_YEAR;
                    int MAST_STATE_CODE = (int)LockUnlockViewModel.MAST_STATE_CODE;
                    int IMS_BATCH = LockUnlockViewModel.IMS_BATCH;
                    byte Scheme = Convert.ToByte(LockUnlockViewModel.PMGSYScheme);
                    //1st Transaction on IMS_FREEZE_DETAILS            
                    if (LockUnlockViewModel.FreezeStatus == "U")
                    {
                        dbContextImsSanctionedProjects.sp_UpdateImsFreezeStatus(IMS_YEAR, MAST_STATE_CODE, IMS_BATCH, "F", PMGSYSession.Current.UserId, HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], Scheme);
                    }
                    else
                    {
                        dbContextImsSanctionedProjects.sp_UpdateImsFreezeStatus(IMS_YEAR, MAST_STATE_CODE, IMS_BATCH, "U", PMGSYSession.Current.UserId, HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], Scheme);
                    }

                    //2nd Transaction on IMS_FREEZE_DETAILS                                     
                    dbContextImsFreezeDetails = new PMGSYEntities();

                    var ImsFreezeDetails = new IMS_FREEZE_DETAILS();
                    ImsFreezeDetails.MAST_STATE_CODE = Convert.ToInt32(LockUnlockViewModel.MAST_STATE_CODE);
                    ImsFreezeDetails.IMS_YEAR = LockUnlockViewModel.IMS_YEAR;
                    ImsFreezeDetails.IMS_BATCH = LockUnlockViewModel.IMS_BATCH;
                    ImsFreezeDetails.IMS_TRANSACTION_NO = (int)GetMaxImsTransactionNumber(LockUnlockViewModel);
                    ImsFreezeDetails.IMS_FREEZE_DATE = DateTime.Now;
                    ImsFreezeDetails.MAST_PMGSY_SCHEME = Convert.ToByte(LockUnlockViewModel.PMGSYScheme);
                    if (LockUnlockViewModel.FreezeStatus == "U")
                    {
                        ImsFreezeDetails.IMS_FREEZE_STATUS = "F";
                    }
                    else
                    {
                        ImsFreezeDetails.IMS_FREEZE_STATUS = "U";
                    }

                    //Modified by abhishek kamble 28-nov-2013
                    ImsFreezeDetails.USERID = PMGSYSession.Current.UserId;
                    ImsFreezeDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    dbContextImsFreezeDetails.IMS_FREEZE_DETAILS.Add(ImsFreezeDetails);
                    dbContextImsFreezeDetails.SaveChanges();
                    // If execution reaches here, it indicates the successfull completion of all save/update operation. hence comit the transaction.
                    ts.Complete();
                    return true;

                }//end Of Try
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);

                    // If any excption is caught, roll back the entire transaction and ends the transaction scope
                    ts.Dispose();
                    return false;
                }
                finally
                {
                    dbContextImsFreezeDetails.Dispose();
                    dbContextImsSanctionedProjects.Dispose();
                }//end of finally     

            }//end of transaction scope

        }//end of FreezeUnfreezeProposal

        public Array GetFreezeUnfreezeReportDetails(int stateCode, int batchCode, int yearCode, int schemeCode, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            dbContext = new PMGSYEntities();
            try
            {
                var lstDetails = (from item in dbContext.IMS_FREEZE_DETAILS
                                  where
                                  (stateCode == 0 ? 1 : item.MAST_STATE_CODE) == (stateCode == 0 ? 1 : stateCode) &&
                                  (batchCode == 0 ? 1 : item.IMS_BATCH) == (batchCode == 0 ? 1 : batchCode) &&
                                  (yearCode == 0 ? 1 : item.IMS_YEAR) == (yearCode == 0 ? 1 : yearCode) &&
                                  item.MAST_PMGSY_SCHEME == schemeCode
                                  select new
                                  {
                                      item.MASTER_STATE.MAST_STATE_NAME,
                                      item.MASTER_YEAR.MAST_YEAR_TEXT,
                                      item.MASTER_BATCH.MAST_BATCH_NAME,
                                      item.IMS_TRANSACTION_NO,
                                      item.MAST_PMGSY_SCHEME,
                                      FREEZE_STATUS = item.IMS_FREEZE_STATUS == "F" ? "Freeze" : "Unfreeze",
                                      item.IMS_FREEZE_DATE
                                  }).Distinct().ToList();

                totalRecords = lstDetails.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "MAST_STATE_NAME":
                                lstDetails = lstDetails.OrderBy(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_YEAR_TEXT":
                                lstDetails = lstDetails.OrderBy(x => x.MAST_YEAR_TEXT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_BATCH_NAME":
                                lstDetails = lstDetails.OrderBy(x => x.MAST_BATCH_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_TRANSACTION_NO":
                                lstDetails = lstDetails.OrderBy(x => x.IMS_TRANSACTION_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_PMGSY_SCHEME":
                                lstDetails = lstDetails.OrderBy(x => x.MAST_PMGSY_SCHEME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "FREEZE_STATUS":
                                lstDetails = lstDetails.OrderBy(x => x.FREEZE_STATUS).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_FREEZE_DATE":
                                lstDetails = lstDetails.OrderBy(x => x.IMS_FREEZE_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstDetails = lstDetails.OrderBy(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }

                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "MAST_STATE_NAME":
                                lstDetails = lstDetails.OrderByDescending(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_YEAR_TEXT":
                                lstDetails = lstDetails.OrderByDescending(x => x.MAST_YEAR_TEXT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_BATCH_NAME":
                                lstDetails = lstDetails.OrderByDescending(x => x.MAST_BATCH_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_TRANSACTION_NO":
                                lstDetails = lstDetails.OrderByDescending(x => x.IMS_TRANSACTION_NO).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_PMGSY_SCHEME":
                                lstDetails = lstDetails.OrderByDescending(x => x.MAST_PMGSY_SCHEME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "FREEZE_STATUS":
                                lstDetails = lstDetails.OrderByDescending(x => x.FREEZE_STATUS).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_FREEZE_DATE":
                                lstDetails = lstDetails.OrderByDescending(x => x.IMS_FREEZE_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstDetails = lstDetails.OrderByDescending(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstDetails = lstDetails.OrderBy(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                return lstDetails.Select(freezeDetails => new
                {
                    cell = new[] {                         
                                    freezeDetails.MAST_STATE_NAME==null?"NA":freezeDetails.MAST_STATE_NAME.ToString(),
                                    freezeDetails.MAST_YEAR_TEXT==null?"NA":freezeDetails.MAST_YEAR_TEXT.ToString(),
                                    freezeDetails.MAST_PMGSY_SCHEME.ToString(),
                                    freezeDetails.MAST_BATCH_NAME==null?"NA":freezeDetails.MAST_BATCH_NAME.ToString(),
                                    //freezeDetails.IMS_TRANSACTION_NO.ToString(),
                                    freezeDetails.IMS_FREEZE_DATE == null? "-" :freezeDetails.IMS_FREEZE_DATE.ToString("dd/MM/yyyy"),
                                    freezeDetails.FREEZE_STATUS==null?"-":freezeDetails.FREEZE_STATUS.ToString(),
                                    
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
                dbContext.Dispose();
            }
        }


        //calculate maximum transaction number
        public Int32? GetMaxImsTransactionNumber(ProposalFilterLockUnlockViewModel LockUnlockViewModel)
        {

            try
            {
                Int32? MaxID = 0;


                dbContext = new PMGSYEntities();

                if (!dbContext.IMS_FREEZE_DETAILS.Any())
                {
                    MaxID = 1;
                }
                else
                {
                    if (!dbContext.IMS_FREEZE_DETAILS.Where(m => m.IMS_YEAR == LockUnlockViewModel.IMS_YEAR && m.MAST_STATE_CODE == LockUnlockViewModel.MAST_STATE_CODE && m.IMS_BATCH == LockUnlockViewModel.IMS_BATCH && m.MAST_PMGSY_SCHEME == LockUnlockViewModel.PMGSYScheme).Any())
                    {
                        MaxID = 1;
                    }
                    else
                    {

                        MaxID = dbContext.IMS_FREEZE_DETAILS.Where(m => m.IMS_YEAR == LockUnlockViewModel.IMS_YEAR && m.MAST_STATE_CODE == LockUnlockViewModel.MAST_STATE_CODE && m.IMS_BATCH == LockUnlockViewModel.IMS_BATCH && m.MAST_PMGSY_SCHEME == LockUnlockViewModel.PMGSYScheme).Max(m => m.IMS_TRANSACTION_NO);

                        MaxID = MaxID + 1;
                    }
                }
                return MaxID;
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

        /// <summary>
        /// checks whether the sanction order is generated for this combination or not
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool IsSanctionOrderGenerated(int state, int batch, int year, int scheme)
        {
            dbContext = new PMGSYEntities();

            try
            {
                if (dbContext.IMS_SANCTIONED_PROJECTS_PDF.Any(m => m.IMS_BATCH == batch && m.IMS_YEAR == year && m.MAST_STATE_CODE == state && m.MAST_PMGSY_SCHEME == scheme))
                {
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
                dbContext.Dispose();
            }
        }

        #region LOCK_UNLOCK

        #region PROPOSAL

        /// <summary>
        /// returns data to populate grid
        /// </summary>
        /// <param name="yearCode"></param>
        /// <param name="stateCode"></param>
        /// <param name="districtCode"></param>
        /// <param name="batchCode"></param>
        /// <param name="packageCode"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <param name="lstIds"></param>
        /// <param name="ImsPrRoadCode"></param>
        /// <returns></returns>
        public Array GetProposalList(string propType, int yearCode, int stateCode, int districtCode, int blockCode, int batchCode, string packageCode, byte scheme, int collaboration, int roleCode, string type, int page, int rows, string sidx, string sord, out long totalRecords, out List<int> lstIds, out string ImsPrRoadCode)
        {
            totalRecords = 0;
            dbContext = new PMGSYEntities();
            try
            {
                //var lstProposalList = (IEnumerable<dynamic>)null;
                //list of proposal details
                //lstProposalList = (from item in dbContext.IMS_SANCTIONED_PROJECTS
                //                   join district in dbContext.MASTER_DISTRICT on item.MAST_DISTRICT_CODE equals district.MAST_DISTRICT_CODE
                //                   join state in dbContext.MASTER_STATE on item.MAST_STATE_CODE equals state.MAST_STATE_CODE
                //                   join block in dbContext.MASTER_BLOCK on item.MAST_BLOCK_CODE equals block.MAST_BLOCK_CODE
                //                   join year in dbContext.MASTER_YEAR on item.IMS_YEAR equals year.MAST_YEAR_CODE
                //                   where
                //                   (yearCode == 0 ? 1 : item.IMS_YEAR) == (yearCode == 0 ? 1 : yearCode) &&
                //                   (stateCode == 0 ? 1 : item.MAST_STATE_CODE) == (stateCode == 0 ? 1 : stateCode) &&
                //                   (districtCode == 0 ? 1 : item.MAST_DISTRICT_CODE) == (districtCode == 0 ? 1 : districtCode) &&
                //                   (batchCode == 0 ? 1 : item.IMS_BATCH) == (batchCode == 0 ? 1 : batchCode) &&
                //                   (packageCode == "0" ? "%" : item.IMS_PACKAGE_ID) == (packageCode == "0" ? "%" : packageCode) &&
                //                   (item.IMS_SANCTIONED == "Y")
                //                   select new
                //                   {
                //                       block.MAST_BLOCK_NAME,
                //                       item.IMS_PACKAGE_ID,
                //                       item.IMS_PR_ROAD_CODE,
                //                       item.IMS_ROAD_NAME,
                //                       year.MAST_YEAR_TEXT,
                //                       item.IMS_LOCK_STATUS,
                //                       item.IMS_PAV_LENGTH

                //                   }).Distinct();

                //if (packageCode == "0")
                //{
                //    packageCode = null;
                //}

                if (packageCode == "-1" || packageCode == "0")
                {
                    packageCode = "%";
                }

                //var lstProposalList = dbContext.USP_UNLOCK_RECORDS("PR", "R", stateCode, (districtCode <= 0 ? 0 : districtCode), (blockCode <= 0 ? 0 : blockCode), 0, 0, 0, yearCode, batchCode, type, packageCode, scheme, collaboration, roleCode).ToList();
                var lstProposalList = dbContext.USP_UNLOCK_RECORDS(propType, "R", stateCode, (districtCode <= 0 ? 0 : districtCode), (blockCode <= 0 ? 0 : blockCode), 0, 0, 0, yearCode, batchCode, type, packageCode, scheme, collaboration, roleCode).ToList();

                totalRecords = lstProposalList.Count();

                lstIds = new List<int>();

                ImsPrRoadCode = String.Empty;
                foreach (var item in lstProposalList)
                {
                    int id = item.LEVELCODE;
                    lstIds.Add(id);

                    if (ImsPrRoadCode == String.Empty)
                    {
                        ImsPrRoadCode = Convert.ToString(item.LEVELCODE);
                    }
                    else
                    {
                        ImsPrRoadCode = ImsPrRoadCode + "_" + item.LEVELCODE;
                    }

                }
                //sorting 
                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "LEVELNAME":
                                lstProposalList = lstProposalList.OrderBy(x => x.LEVELNAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "LEVELNUMBER":
                                lstProposalList = lstProposalList.OrderBy(x => x.LEVELNUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "YEARCODE":
                                lstProposalList = lstProposalList.OrderBy(x => x.YEARCODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "STARTDATE":
                                lstProposalList = lstProposalList.OrderBy(x => x.STARTDATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "ENDDATE":
                                lstProposalList = lstProposalList.OrderBy(x => x.ENDDATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            //case "IMS_LOCK_STATUS":
                            //    lstProposalList = lstProposalList.OrderBy(x => x.IMS_LOCK_STATUS).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                            //    break;
                            //case "IMS_PAV_LENGTH":
                            //    lstProposalList = lstProposalList.OrderBy(x => x.IMS_PAV_LENGTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                            //    break;
                            default:
                                lstProposalList = lstProposalList.OrderBy(x => x.LEVELNAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "LEVELNAME":
                                lstProposalList = lstProposalList.OrderByDescending(x => x.LEVELNAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "LEVELNUMBER":
                                lstProposalList = lstProposalList.OrderByDescending(x => x.LEVELNUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "YEARCODE":
                                lstProposalList = lstProposalList.OrderByDescending(x => x.YEARCODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "STARTDATE":
                                lstProposalList = lstProposalList.OrderByDescending(x => x.STARTDATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "ENDDATE":
                                lstProposalList = lstProposalList.OrderByDescending(x => x.ENDDATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            //case "IMS_LOCK_STATUS":
                            //    lstProposalList = lstProposalList.OrderBy(x => x.IMS_LOCK_STATUS).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                            //    break;
                            //case "IMS_PAV_LENGTH":
                            //    lstProposalList = lstProposalList.OrderBy(x => x.IMS_PAV_LENGTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                            //    break;
                            default:
                                lstProposalList = lstProposalList.OrderByDescending(x => x.LEVELNAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstProposalList = lstProposalList.OrderByDescending(x => x.LEVELNAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }


                var gridData = lstProposalList.Select(proposalDetails => new
                {
                    proposalDetails.LEVELCODE,
                    proposalDetails.LEVELNAME,
                    proposalDetails.LEVELNUMBER,
                    proposalDetails.YEARCODE,
                    proposalDetails.STARTDATE,
                    proposalDetails.ENDDATE,
                }).ToArray();


                //returning the griddata
                var result = gridData.Select(proposalDetails => new
                {

                    id = proposalDetails.LEVELCODE.ToString(),
                    //IMS_LOCK_STATUS = proposalDetails.IMS_LOCK_STATUS.ToString(),
                    cell = new[]{
                    
                        proposalDetails.LEVELNAME==null?string.Empty:proposalDetails.LEVELNAME.ToString(),
                        proposalDetails.YEARCODE==null?string.Empty:proposalDetails.YEARCODE.ToString(),
                        proposalDetails.LEVELNUMBER == null?string.Empty:proposalDetails.LEVELNUMBER.ToString(),
                        proposalDetails.STARTDATE==null?"-":Convert.ToDateTime(proposalDetails.STARTDATE).ToString("dd/MM/yyyy"),
                        proposalDetails.ENDDATE==null?"-":Convert.ToDateTime(proposalDetails.ENDDATE).ToString("dd/MM/yyyy"),
                        "<a href='#' title='Click to Lock Proposal Details' class='ui-icon ui-icon-search ui-align-center' onClick='ViewDetails(\"" + URLEncrypt.EncryptParameters(new string[]{proposalDetails.LEVELCODE.ToString().Trim()+"$"+"R"}) +"\"); return false;'></a>"
                        //proposalDetails.IMS_PR_ROAD_CODE == 0?string.Empty:proposalDetails.IMS_PR_ROAD_CODE.ToString(),
                        //proposalDetails.IMS_LOCK_STATUS=="N"?"<a href='#' title='Click to Lock Proposal Details' class='ui-icon ui-icon-locked ui-align-center' onClick='LockModuleUnit(\"" + URLEncrypt.EncryptParameters1(new string[]{"ProposalCode="+ proposalDetails.IMS_PR_ROAD_CODE.ToString().Trim() }) +"\"); return false;'></a>":"<a href='#' title='Click to Unlock Proposal Details' class='ui-icon ui-icon-unlocked ui-align-center' onClick='UnlockModuleUnit(\"" + URLEncrypt.EncryptParameters1(new string[]{"ProposalCode="+ proposalDetails.IMS_PR_ROAD_CODE.ToString().Trim() }) +"\"); return false;'></a>"
                    }
                }).ToArray();

                return result;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ImsPrRoadCode = String.Empty;
                lstIds = null;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array GetProposalLockList(int yearCode, int stateCode, int districtCode, int batchCode, string packageCode, int page, int rows, string sidx, string sord, out long totalRecords, out List<int> lstIds, out string ImsPrRoadCode)
        {
            dbContext = new PMGSYEntities();
            try
            {
                var lstProposalList = (from item in dbContext.IMS_SANCTIONED_PROJECTS
                                       join district in dbContext.MASTER_DISTRICT on item.MAST_DISTRICT_CODE equals district.MAST_DISTRICT_CODE
                                       join state in dbContext.MASTER_STATE on item.MAST_STATE_CODE equals state.MAST_STATE_CODE
                                       join block in dbContext.MASTER_BLOCK on item.MAST_BLOCK_CODE equals block.MAST_BLOCK_CODE
                                       join year in dbContext.MASTER_YEAR on item.IMS_YEAR equals year.MAST_YEAR_CODE
                                       join lockDetails in dbContext.IMS_LOCK_DETAILS on item.IMS_PR_ROAD_CODE equals lockDetails.IMS_PR_ROAD_CODE
                                       where
                                       (yearCode == 0 ? 1 : item.IMS_YEAR) == (yearCode == 0 ? 1 : yearCode) &&
                                       (stateCode == 0 ? 1 : item.MAST_STATE_CODE) == (stateCode == 0 ? 1 : stateCode) &&
                                       (districtCode == 0 ? 1 : item.MAST_DISTRICT_CODE) == (districtCode == 0 ? 1 : districtCode) &&
                                       (batchCode == 0 ? 1 : item.IMS_BATCH) == (batchCode == 0 ? 1 : batchCode) &&
                                       (packageCode == "0" ? "%" : item.IMS_PACKAGE_ID) == (packageCode == "0" ? "%" : packageCode) &&
                                       item.IMS_SANCTIONED == "Y" &&
                                       lockDetails.IMS_AUTOLOCK_DATE > System.DateTime.Now &&
                                       lockDetails.IMS_UNLOCK_BY == "M" &&
                                       item.IMS_LOCK_STATUS == "N"
                                       select new
                                       {
                                           block.MAST_BLOCK_NAME,
                                           item.IMS_PACKAGE_ID,
                                           item.IMS_PR_ROAD_CODE,
                                           item.IMS_ROAD_NAME,
                                           year.MAST_YEAR_TEXT,
                                           item.IMS_LOCK_STATUS,
                                           item.IMS_PAV_LENGTH
                                       }).Distinct();

                totalRecords = lstProposalList.Count();

                lstIds = new List<int>();

                ImsPrRoadCode = String.Empty;
                foreach (var item in lstProposalList)
                {
                    int id = item.IMS_PR_ROAD_CODE;
                    lstIds.Add(id);

                    if (ImsPrRoadCode == String.Empty)
                    {
                        ImsPrRoadCode = Convert.ToString(item.IMS_PR_ROAD_CODE);
                    }
                    else
                    {
                        ImsPrRoadCode = ImsPrRoadCode + "_" + item.IMS_PR_ROAD_CODE;
                    }

                }

                //sorting 
                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "MAST_BLOCK_NAME":
                                lstProposalList = lstProposalList.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "IMS_PACKAGE_ID":
                                lstProposalList = lstProposalList.OrderBy(x => x.IMS_PACKAGE_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "IMS_ROAD_NAME":
                                lstProposalList = lstProposalList.OrderBy(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "IMS_ROAD_CODE":
                                lstProposalList = lstProposalList.OrderBy(x => x.IMS_PR_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "IMS_YEAR":
                                lstProposalList = lstProposalList.OrderBy(x => x.MAST_YEAR_TEXT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "IMS_LOCK_STATUS":
                                lstProposalList = lstProposalList.OrderBy(x => x.IMS_LOCK_STATUS).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "IMS_PAV_LENGTH":
                                lstProposalList = lstProposalList.OrderBy(x => x.IMS_PAV_LENGTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "MAST_BLOCK_NAME":
                                lstProposalList = lstProposalList.OrderByDescending(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "IMS_PACKAGE_ID":
                                lstProposalList = lstProposalList.OrderByDescending(x => x.IMS_PACKAGE_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "IMS_ROAD_NAME":
                                lstProposalList = lstProposalList.OrderByDescending(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "IMS_ROAD_CODE":
                                lstProposalList = lstProposalList.OrderByDescending(x => x.IMS_PR_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "IMS_YEAR":
                                lstProposalList = lstProposalList.OrderByDescending(x => x.MAST_YEAR_TEXT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "IMS_LOCK_STATUS":
                                lstProposalList = lstProposalList.OrderByDescending(x => x.IMS_LOCK_STATUS).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "IMS_PAV_LENGTH":
                                lstProposalList = lstProposalList.OrderByDescending(x => x.IMS_PAV_LENGTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                break;
                        }
                    }
                }
                else
                {
                    lstProposalList = lstProposalList.OrderByDescending(x => x.IMS_PR_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }


                var gridData = lstProposalList.Select(proposalDetails => new
                {

                    proposalDetails.MAST_BLOCK_NAME,
                    proposalDetails.IMS_PACKAGE_ID,
                    proposalDetails.IMS_PR_ROAD_CODE,
                    proposalDetails.IMS_ROAD_NAME,
                    proposalDetails.MAST_YEAR_TEXT,
                    proposalDetails.IMS_LOCK_STATUS,
                    proposalDetails.IMS_PAV_LENGTH
                }).ToArray();


                //returning the griddata
                var result = gridData.Select(proposalDetails => new
                {

                    id = proposalDetails.IMS_PR_ROAD_CODE.ToString(),
                    IMS_LOCK_STATUS = proposalDetails.IMS_LOCK_STATUS.ToString(),
                    cell = new[]{
                    
                        proposalDetails.MAST_BLOCK_NAME==null?string.Empty:proposalDetails.MAST_BLOCK_NAME.ToString(),
                        proposalDetails.MAST_YEAR_TEXT == null?string.Empty:proposalDetails.MAST_YEAR_TEXT.ToString(),
                        proposalDetails.IMS_PACKAGE_ID==null?string.Empty:proposalDetails.IMS_PACKAGE_ID.ToString(),
                        proposalDetails.IMS_ROAD_NAME==null?string.Empty:proposalDetails.IMS_ROAD_NAME.ToString(),
                        proposalDetails.IMS_PAV_LENGTH.ToString(),
                        //proposalDetails.IMS_PR_ROAD_CODE == 0?string.Empty:proposalDetails.IMS_PR_ROAD_CODE.ToString(),
                        //proposalDetails.IMS_LOCK_STATUS=="N"?"<a href='#' title='Click to Lock Proposal Details' class='ui-icon ui-icon-locked ui-align-center' onClick='LockModuleUnit(\"" + URLEncrypt.EncryptParameters1(new string[]{"ProposalCode="+ proposalDetails.IMS_PR_ROAD_CODE.ToString().Trim() }) +"\"); return false;'></a>":"<a href='#' title='Click to Unlock Proposal Details' class='ui-icon ui-icon-unlocked ui-align-center' onClick='UnlockModuleUnit(\"" + URLEncrypt.EncryptParameters1(new string[]{"ProposalCode="+ proposalDetails.IMS_PR_ROAD_CODE.ToString().Trim() }) +"\"); return false;'></a>"
                    }
                }).ToArray();

                return result;

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                lstIds = null;
                totalRecords = 0;
                ImsPrRoadCode = string.Empty;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// return json to populate dropdown
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetAllStates(bool isState, int selectedValue = 0)
        {
            dbContext = new PMGSYEntities();
            try
            {
                if (isState)
                {
                    List<SelectListItem> lstStates = new SelectList(dbContext.MASTER_STATE.Where(m => m.MAST_STATE_CODE == selectedValue).ToList(), "MAST_STATE_CODE", "MAST_STATE_NAME", selectedValue).ToList();
                    return lstStates;
                }
                else
                {
                    List<SelectListItem> lstStates = new SelectList(dbContext.MASTER_STATE.ToList(), "MAST_STATE_CODE", "MAST_STATE_NAME", selectedValue).ToList();
                    return lstStates;
                }
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

        /// <summary>
        /// returns list of districts to populate district dropdown
        /// </summary>
        /// <param name="stateCode"></param>
        /// <returns></returns>
        public List<SelectListItem> GetAllDistrictsByStateCode(int stateCode)
        {
            dbContext = new PMGSYEntities();
            try
            {
                List<SelectListItem> lstDistricts = new SelectList(dbContext.MASTER_DISTRICT.Where(m => m.MAST_STATE_CODE == stateCode).ToList(), "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME").ToList();
                return lstDistricts;
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

        /// <summary>
        /// returns all batches for Batch dropdown
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetAllBatches()
        {
            dbContext = new PMGSYEntities();
            try
            {
                List<SelectListItem> lstBatches = new SelectList(dbContext.MASTER_BATCH.ToList(), "MAST_BATCH_CODE", "MAST_BATCH_NAME").ToList();
                return lstBatches;
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

        /// <summary>
        /// returns the list of packages for package dropdown
        /// </summary>
        /// <param name="yearCode"></param>
        /// <param name="stateCode"></param>
        /// <param name="districtCode"></param>
        /// <param name="isAllPackagesSelected"></param>
        /// <returns></returns>
        public List<SelectListItem> GetAllPackages(int yearCode, int stateCode, int districtCode, int batchCode, int blockCode, string type, bool isAllPackagesSelected = true)
        {

            List<SelectListItem> BlockList = new List<SelectListItem>();
            SelectListItem item;
            if (!isAllPackagesSelected)
            {
                item = new SelectListItem();
                item.Text = "Select Package";
                item.Value = "0";
                item.Selected = true;
                BlockList.Add(item);
            }
            else
            {
                item = new SelectListItem();
                item.Text = "All Packages";
                item.Value = "-1";
                item.Selected = true;
                BlockList.Add(item);
            }
            try
            {
                dbContext = new PMGSYEntities();
                var query = (from c in dbContext.IMS_SANCTIONED_PROJECTS
                             where
                             c.MAST_DISTRICT_CODE == districtCode &&
                             c.IMS_YEAR == yearCode &&
                             c.MAST_STATE_CODE == stateCode &&
                             c.IMS_BATCH == batchCode &&
                             c.MAST_BLOCK_CODE == blockCode &&
                             (PMGSYSession.Current.RoleCode == 36 ? ((c.IMS_ISCOMPLETED == "S" && c.STA_SANCTIONED == "U") ||
                                       (c.IMS_ISCOMPLETED == "D" && c.STA_SANCTIONED == "N")) : (type == "M" ? c.IMS_SANCTIONED == "Y" : c.STA_SANCTIONED == "Y"))
                             select new
                             {
                                 Text = c.IMS_PACKAGE_ID,
                                 Value = c.IMS_PACKAGE_ID
                             }).Distinct().ToList();

                foreach (var data in query)
                {
                    item = new SelectListItem();
                    item.Text = data.Text;
                    item.Value = data.Value.ToString();
                    BlockList.Add(item);
                }
                return BlockList;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);

                return BlockList;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// returns the list of packages by state
        /// </summary>
        /// <param name="stateCode"></param>
        /// <param name="yearCode"></param>
        /// <param name="isAllPackagesSelected"></param>
        /// <returns></returns>
        public List<SelectListItem> GetAllPackageByStateCode(int stateCode, int yearCode, bool isAllPackagesSelected = false)
        {

            List<SelectListItem> BlockList = new List<SelectListItem>();
            SelectListItem item;
            if (!isAllPackagesSelected)
            {
                item = new SelectListItem();
                item.Text = "Select Package";
                item.Value = "0";
                item.Selected = true;
                BlockList.Add(item);
            }
            else
            {
                item = new SelectListItem();
                item.Text = "All Packages";
                item.Value = "-1";
                item.Selected = true;
                BlockList.Add(item);
            }
            try
            {
                dbContext = new PMGSYEntities();
                var query = (from c in dbContext.IMS_SANCTIONED_PROJECTS
                             where
                             c.IMS_YEAR == yearCode &&
                             c.MAST_STATE_CODE == stateCode
                             select new
                             {
                                 Text = c.IMS_PACKAGE_ID,
                                 Value = c.IMS_PACKAGE_ID
                             }).Distinct().ToList();

                foreach (var data in query)
                {
                    item = new SelectListItem();
                    item.Text = data.Text;
                    item.Value = data.Value.ToString();
                    BlockList.Add(item);
                }

                //List<SelectListItem> blockList = new SelectList(query, "Value", "Text").ToList<SelectListItem>();
                //blockList.Add(item);
                return BlockList;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);

                return BlockList;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// returns year dropdown list
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetAllYears()
        {
            dbContext = new PMGSYEntities();
            CommonFunctions objCommon = new CommonFunctions();
            try
            {
                List<SelectListItem> lstYear = new SelectList(objCommon.PopulateFinancialYear(true, false).ToList(), "Value", "Text").ToList();
                return lstYear;
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

        /// <summary>
        /// returns the list of module dropdown
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetAllModules()
        {
            try
            {
                List<SelectListItem> lstModules = new List<SelectListItem>();
                lstModules.Add(new SelectListItem { Value = "0", Text = "--Select Module--" });
                if (PMGSYSession.Current.RoleCode == 36 || PMGSYSession.Current.RoleCode == 47 || PMGSYSession.Current.RoleCode == 56) //ITNO=36 and ITNOOA=47 and ITNORCPLWE=56 
                {
                    lstModules.Add(new SelectListItem { Value = "2", Text = "Proposal" });
                }
                else
                {
                    lstModules.Add(new SelectListItem { Value = "1", Text = "Habitation" });
                    lstModules.Add(new SelectListItem { Value = "2", Text = "Proposal" });
                    lstModules.Add(new SelectListItem { Value = "3", Text = "Existing Road" });
                    lstModules.Add(new SelectListItem { Value = "4", Text = "Core Network" });
                    lstModules.Add(new SelectListItem { Value = "5", Text = "Village" });
                    lstModules.Add(new SelectListItem { Value = "6", Text = "District" });
                    lstModules.Add(new SelectListItem { Value = "7", Text = "Block" });
                    lstModules.Add(new SelectListItem { Value = "8", Text = "Proposal Habitation" });
                    lstModules.Add(new SelectListItem { Value = "9", Text = "TR/MRL Habitations" });// Added By Rohit On 09 APR 2020
                    // Added By to unlock Proposal Technology Details
                    lstModules.Add(new SelectListItem { Value = "10", Text = "Proposal Technology" });
                    lstModules.Add(new SelectListItem { Value = "11", Text = "C Proforma PDF" });  // Added by Shreyas on 14-09-2023
                }
                return lstModules;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);

                return null;
            }
        }

        public List<SelectListItem> GetSubmoduleByModuleCode(int moduleCode)
        {
            try
            {
                List<SelectListItem> lstSubmodule = new List<SelectListItem>();
                switch (moduleCode)
                {
                    case 1:
                        lstSubmodule.Add(new SelectListItem { Value = "0", Text = "--Select Submodule--" });
                        lstSubmodule.Add(new SelectListItem { Value = "H", Text = "Habitations" });
                        lstSubmodule.Add(new SelectListItem { Value = "C", Text = "CBR" });
                        lstSubmodule.Add(new SelectListItem { Value = "BC", Text = "Bridge Component" });
                        lstSubmodule.Add(new SelectListItem { Value = "LB", Text = "LSB Bridge" });
                        lstSubmodule.Add(new SelectListItem { Value = "T", Text = "Traffic Intensity" });
                        return lstSubmodule;
                    case 2:
                        lstSubmodule.Add(new SelectListItem { Value = "0", Text = "--Select Submodule--" });
                        lstSubmodule.Add(new SelectListItem { Value = "H", Text = "Habitations" });
                        lstSubmodule.Add(new SelectListItem { Value = "C", Text = "CBR" });
                        lstSubmodule.Add(new SelectListItem { Value = "CW", Text = "CDWorks" });
                        lstSubmodule.Add(new SelectListItem { Value = "S", Text = "Surface Types" });
                        lstSubmodule.Add(new SelectListItem { Value = "T", Text = "Traffic Intensity" });
                        lstSubmodule.Add(new SelectListItem { Value = "R", Text = "Road Details" });
                        return lstSubmodule;
                    case 3:
                        lstSubmodule.Add(new SelectListItem { Value = "0", Text = "--Select Submodule--" });
                        lstSubmodule.Add(new SelectListItem { Value = "H", Text = "Habitations" });
                        lstSubmodule.Add(new SelectListItem { Value = "R", Text = "Road Details" });
                        lstSubmodule.Add(new SelectListItem { Value = "U", Text = "Uploaded Files" });
                        return lstSubmodule;
                    default:
                        lstSubmodule.Add(new SelectListItem { Value = "0", Text = "--Select Submodule--" });
                        return lstSubmodule;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);

                return null;
            }
        }

        /// <summary>
        /// returns the level dropdown values according to the module code
        /// </summary>
        /// <param name="moduleCode"></param>
        /// <returns></returns>
        public List<SelectListItem> GetModuleLevelByModuleCode(int moduleCode)
        {
            try
            {
                List<SelectListItem> lstLevel = new List<SelectListItem>();
                switch (moduleCode)
                {
                    case 1:
                        lstLevel.Add(new SelectListItem { Value = "0", Text = "--Select Level--" });
                        lstLevel.Add(new SelectListItem { Value = "S", Text = "State" });
                        lstLevel.Add(new SelectListItem { Value = "D", Text = "District" });
                        lstLevel.Add(new SelectListItem { Value = "B", Text = "Block" });
                        //lstLevel.Add(new SelectListItem { Value = "R", Text = "Road" });
                        lstLevel.Add(new SelectListItem { Value = "H", Text = "Habitation" });
                        lstLevel.Add(new SelectListItem { Value = "V", Text = "Village" });
                        return lstLevel;
                    case 2:
                        lstLevel.Add(new SelectListItem { Value = "0", Text = "--Select Level--" });
                        //if (PMGSYSession.Current.LevelId != 4)
                        //{
                        //    lstLevel.Add(new SelectListItem { Value = "S", Text = "State" });
                        //}
                        //lstLevel.Add(new SelectListItem { Value = "D", Text = "District" });
                        //lstLevel.Add(new SelectListItem { Value = "B", Text = "Block" });
                        lstLevel.Add(new SelectListItem { Value = "R", Text = "Road/LSB" });
                        if (PMGSYSession.Current.RoleCode == 25)
                        {
                            lstLevel.Add(new SelectListItem { Value = "Y", Text = "Year" });
                            lstLevel.Add(new SelectListItem { Value = "T", Text = "Batch" });
                        }
                        return lstLevel;
                    case 3:
                        lstLevel.Add(new SelectListItem { Value = "0", Text = "--Select Level--" });
                        lstLevel.Add(new SelectListItem { Value = "S", Text = "State" });
                        lstLevel.Add(new SelectListItem { Value = "D", Text = "District" });
                        lstLevel.Add(new SelectListItem { Value = "B", Text = "Block" });
                        lstLevel.Add(new SelectListItem { Value = "R", Text = "Road" });
                        return lstLevel;
                    case 4:
                        lstLevel.Add(new SelectListItem { Value = "0", Text = "--Select Level--" });
                        lstLevel.Add(new SelectListItem { Value = "S", Text = "State" });
                        lstLevel.Add(new SelectListItem { Value = "D", Text = "District" });
                        lstLevel.Add(new SelectListItem { Value = "B", Text = "Block" });
                        lstLevel.Add(new SelectListItem { Value = "R", Text = "Road" });
                        return lstLevel;
                    case 5:
                        lstLevel.Add(new SelectListItem { Value = "0", Text = "--Select Level--" });
                        lstLevel.Add(new SelectListItem { Value = "S", Text = "State" });
                        lstLevel.Add(new SelectListItem { Value = "D", Text = "District" });
                        lstLevel.Add(new SelectListItem { Value = "B", Text = "Block" });
                        lstLevel.Add(new SelectListItem { Value = "V", Text = "Village" });
                        return lstLevel;
                    case 6:
                        lstLevel.Add(new SelectListItem { Value = "0", Text = "--Select Level--" });
                        lstLevel.Add(new SelectListItem { Value = "S", Text = "State" });
                        lstLevel.Add(new SelectListItem { Value = "D", Text = "District" });
                        return lstLevel;
                    case 7:
                        lstLevel.Add(new SelectListItem { Value = "0", Text = "--Select Level--" });
                        lstLevel.Add(new SelectListItem { Value = "S", Text = "State" });
                        lstLevel.Add(new SelectListItem { Value = "D", Text = "District" });
                        lstLevel.Add(new SelectListItem { Value = "B", Text = "Block" });
                        return lstLevel;
                    case 8:
                        lstLevel.Add(new SelectListItem { Value = "0", Text = "--Select Level--" });
                        //if (PMGSYSession.Current.LevelId != 4)
                        //{
                        //    lstLevel.Add(new SelectListItem { Value = "S", Text = "State" });
                        //}
                        //lstLevel.Add(new SelectListItem { Value = "D", Text = "District" });
                        //lstLevel.Add(new SelectListItem { Value = "B", Text = "Block" });
                        lstLevel.Add(new SelectListItem { Value = "R", Text = "Road/LSB" });
                        if (PMGSYSession.Current.RoleCode == 25)
                        {
                            lstLevel.Add(new SelectListItem { Value = "Y", Text = "Year" });
                            lstLevel.Add(new SelectListItem { Value = "T", Text = "Batch" });
                        }
                        return lstLevel;

                    case 9:// TR MRL Habitations Unlock // Added By Rohit On 09 APR 2020
                        lstLevel.Add(new SelectListItem { Value = "0", Text = "--Select Level--" });
                        lstLevel.Add(new SelectListItem { Value = "S", Text = "State" });
                        lstLevel.Add(new SelectListItem { Value = "D", Text = "District" });
                        lstLevel.Add(new SelectListItem { Value = "B", Text = "Block" });
                        return lstLevel;

                    // Added By to unlock Proposal Technology Details
                    case 10:
                        lstLevel.Add(new SelectListItem { Value = "0", Text = "--Select Level--" });
                        lstLevel.Add(new SelectListItem { Value = "R", Text = "Road/LSB" });
                        if (PMGSYSession.Current.RoleCode == 25)
                        {
                            lstLevel.Add(new SelectListItem { Value = "Y", Text = "Year" });
                            lstLevel.Add(new SelectListItem { Value = "T", Text = "Batch" });
                        }
                        return lstLevel;

                    case 11:
                        lstLevel.Add(new SelectListItem { Value = "0", Text = "--Select Level--" });
                        lstLevel.Add(new SelectListItem { Value = "R", Text = "Road/LSB" });
                        if (PMGSYSession.Current.RoleCode == 25)
                        {
                            lstLevel.Add(new SelectListItem { Value = "Y", Text = "Year" });
                            lstLevel.Add(new SelectListItem { Value = "T", Text = "Batch" });
                        }
                        return lstLevel;

                    default:
                        lstLevel.Add(new SelectListItem { Value = "0", Text = "--Select Level--" });
                        return lstLevel;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);

                return null;
            }
        }

        /// <summary>
        /// saves the lock details
        /// </summary>
        /// <param name="lockModel"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool AddLockDetails(LockDetailsViewModel lockModel, ref string message)
        {
            CommonFunctions objCommon = new CommonFunctions();
            dbContext = new PMGSYEntities();

            using (TransactionScope ts = new TransactionScope())
            {
                try
                {
                    switch (lockModel.IMS_UNLOCK_TABLE)
                    {
                        case "IR":
                            foreach (int id in lockModel.DataID)
                            {
                                IMS_LOCK_DETAILS master = new IMS_LOCK_DETAILS();
                                master.IMS_TRANSACTION_NO = (from item in dbContext.IMS_LOCK_DETAILS select item.IMS_TRANSACTION_NO).Any() ? (from item in dbContext.IMS_LOCK_DETAILS select item.IMS_TRANSACTION_NO).Max() + 1 : 1;
                                master.IMS_AUTOLOCK_DATE = objCommon.GetStringToDateTime(lockModel.IMS_AUTOLOCK_DATE);
                                master.IMS_LOCK_STATUS = (lockModel.IMS_LOCK_STATUS == null ? "M" : lockModel.IMS_LOCK_STATUS);
                                master.IMS_PR_ROAD_CODE = Convert.ToInt32(id);
                                master.IMS_UNLOCK_BY = lockModel.IMS_UNLOCK_BY == null ? "M" : lockModel.IMS_UNLOCK_BY;
                                master.IMS_UNLOCK_DATE = objCommon.GetStringToDateTime(lockModel.IMS_UNLOCK_DATE);
                                master.IMS_UNLOCK_REMARKS = lockModel.IMS_UNLOCK_REMARKS;
                                master.IMS_UNLOCK_TABLE = lockModel.IMS_UNLOCK_TABLE;
                                master.MANE_CONTRACT_CODE = lockModel.MANE_CONTRACT_CODE;
                                master.MAST_ER_ROAD_CODE = lockModel.MAST_ER_ROAD_CODE;
                                master.PLAN_CN_ROAD_CODE = lockModel.PLAN_CN_ROAD_CODE;
                                master.TEND_AGREEMENT_CODE = lockModel.TEND_AGREEMENT_CODE;
                                master.TEND_NIT_NO = lockModel.TEND_NIT_NO;
                                master.IMS_DATA_FINALIZED = "N";
                                dbContext.IMS_LOCK_DETAILS.Add(master);
                                dbContext.SaveChanges();

                                if (PMGSYSession.Current.RoleCode == 2)
                                {
                                    IMS_SANCTIONED_PROJECTS proposalMaster = dbContext.IMS_SANCTIONED_PROJECTS.Find(id);
                                    proposalMaster.STA_SANCTIONED = "N";
                                    proposalMaster.STA_SANCTIONED_BY = null;
                                    proposalMaster.STA_SANCTIONED_DATE = null;
                                    proposalMaster.IMS_ISCOMPLETED = "E";
                                    proposalMaster.IMS_STA_REMARKS = null;

                                    //added by abhishek kamble 28-nov-2013
                                    proposalMaster.USERID = PMGSYSession.Current.UserId;
                                    proposalMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                                    dbContext.Entry(proposalMaster).State = System.Data.Entity.EntityState.Modified;
                                    dbContext.SaveChanges();
                                }
                            }
                            if (lockModel.IMS_PR_ROAD_CODE != null)
                            {
                                IMS_SANCTIONED_PROJECTS proposalDetails = dbContext.IMS_SANCTIONED_PROJECTS.Find(lockModel.IMS_PR_ROAD_CODE);
                                proposalDetails.IMS_LOCK_STATUS = (proposalDetails.IMS_LOCK_STATUS == "N" ? "Y" : "N");

                                //added by abhishek kamble 28-nov-2013
                                proposalDetails.USERID = PMGSYSession.Current.UserId;
                                proposalDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                                dbContext.SaveChanges();
                            }
                            break;
                        case "ER":
                            foreach (int id in lockModel.DataID)
                            {
                                IMS_LOCK_DETAILS master = new IMS_LOCK_DETAILS();
                                master.IMS_TRANSACTION_NO = (from item in dbContext.IMS_LOCK_DETAILS select item.IMS_TRANSACTION_NO).Any() ? (from item in dbContext.IMS_LOCK_DETAILS select item.IMS_TRANSACTION_NO).Max() + 1 : 1;
                                master.IMS_AUTOLOCK_DATE = objCommon.GetStringToDateTime(lockModel.IMS_AUTOLOCK_DATE);
                                master.IMS_LOCK_STATUS = (lockModel.IMS_LOCK_STATUS == null ? "M" : lockModel.IMS_LOCK_STATUS);
                                master.IMS_PR_ROAD_CODE = lockModel.IMS_PR_ROAD_CODE;
                                master.IMS_UNLOCK_BY = lockModel.IMS_UNLOCK_BY == null ? "M" : lockModel.IMS_UNLOCK_BY;
                                master.IMS_UNLOCK_DATE = objCommon.GetStringToDateTime(lockModel.IMS_UNLOCK_DATE);
                                master.IMS_UNLOCK_REMARKS = lockModel.IMS_UNLOCK_REMARKS;
                                master.IMS_UNLOCK_TABLE = lockModel.IMS_UNLOCK_TABLE;
                                master.MANE_CONTRACT_CODE = lockModel.MANE_CONTRACT_CODE;
                                master.MAST_ER_ROAD_CODE = Convert.ToInt32(id);
                                master.PLAN_CN_ROAD_CODE = lockModel.PLAN_CN_ROAD_CODE;
                                master.TEND_AGREEMENT_CODE = lockModel.TEND_AGREEMENT_CODE;
                                master.TEND_NIT_NO = lockModel.TEND_NIT_NO;
                                master.IMS_DATA_FINALIZED = "N";
                                dbContext.IMS_LOCK_DETAILS.Add(master);
                                dbContext.SaveChanges();
                            }
                            if (lockModel.MAST_ER_ROAD_CODE != null)
                            {
                                MASTER_EXISTING_ROADS existingDetails = dbContext.MASTER_EXISTING_ROADS.Find(lockModel.MAST_ER_ROAD_CODE);
                                existingDetails.MAST_LOCK_STATUS = (existingDetails.MAST_LOCK_STATUS == "N" ? "Y" : "N");

                                //added by abhishek kamble 28-nov-2013
                                existingDetails.USERID = PMGSYSession.Current.UserId;
                                existingDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                                dbContext.SaveChanges();
                            }
                            break;
                        case "PR":
                            foreach (int id in lockModel.DataID)
                            {
                                IMS_LOCK_DETAILS master = new IMS_LOCK_DETAILS();
                                master.IMS_TRANSACTION_NO = (from item in dbContext.IMS_LOCK_DETAILS select item.IMS_TRANSACTION_NO).Any() ? (from item in dbContext.IMS_LOCK_DETAILS select item.IMS_TRANSACTION_NO).Max() + 1 : 1;
                                master.IMS_AUTOLOCK_DATE = objCommon.GetStringToDateTime(lockModel.IMS_AUTOLOCK_DATE);
                                master.IMS_LOCK_STATUS = (lockModel.IMS_LOCK_STATUS == null ? "M" : lockModel.IMS_LOCK_STATUS);
                                master.IMS_PR_ROAD_CODE = lockModel.IMS_PR_ROAD_CODE;
                                master.IMS_UNLOCK_BY = lockModel.IMS_UNLOCK_BY == null ? "M" : lockModel.IMS_UNLOCK_BY;
                                master.IMS_UNLOCK_DATE = objCommon.GetStringToDateTime(lockModel.IMS_UNLOCK_DATE);
                                master.IMS_UNLOCK_REMARKS = lockModel.IMS_UNLOCK_REMARKS;
                                master.IMS_UNLOCK_TABLE = lockModel.IMS_UNLOCK_TABLE;
                                master.MANE_CONTRACT_CODE = lockModel.MANE_CONTRACT_CODE;
                                master.MAST_ER_ROAD_CODE = lockModel.MAST_ER_ROAD_CODE;
                                master.PLAN_CN_ROAD_CODE = Convert.ToInt32(id);
                                master.TEND_AGREEMENT_CODE = lockModel.TEND_AGREEMENT_CODE;
                                master.TEND_NIT_NO = lockModel.TEND_NIT_NO;
                                master.IMS_DATA_FINALIZED = "N";
                                dbContext.IMS_LOCK_DETAILS.Add(master);
                                dbContext.SaveChanges();
                            }
                            if (lockModel.PLAN_CN_ROAD_CODE != null)
                            {
                                PLAN_ROAD coreDetails = dbContext.PLAN_ROAD.Find(lockModel.PLAN_CN_ROAD_CODE);
                                coreDetails.PLAN_LOCK_STATUS = (coreDetails.PLAN_LOCK_STATUS == "N" ? "Y" : "N");

                                //added by abhishek kamble 28-nov-2013
                                coreDetails.USERID = PMGSYSession.Current.UserId;
                                coreDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                                dbContext.SaveChanges();
                            }
                            break;
                        case "AD":
                            if (lockModel.TEND_AGREEMENT_CODE != null)
                            {
                                TEND_AGREEMENT_MASTER agreementMaster = dbContext.TEND_AGREEMENT_MASTER.Find(lockModel.TEND_AGREEMENT_CODE);
                                agreementMaster.TEND_LOCK_STATUS = (agreementMaster.TEND_LOCK_STATUS == "N" ? "Y" : "N");

                                //added by abhishek kamble 28-nov-2013
                                agreementMaster.USERID = PMGSYSession.Current.UserId;
                                agreementMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                                dbContext.SaveChanges();
                            }
                            break;
                        case "NT":
                            if (lockModel.TEND_NIT_NO != null)
                            {
                                TEND_NIT_MASTER tenderMaster = dbContext.TEND_NIT_MASTER.Find(lockModel.TEND_NIT_NO);
                                tenderMaster.TEND_LOCK_STATUS = (tenderMaster.TEND_LOCK_STATUS == "N" ? "Y" : "N");

                                //added by abhishek kamble 28-nov-2013
                                tenderMaster.USERID = PMGSYSession.Current.UserId;
                                tenderMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                                dbContext.SaveChanges();
                            }
                            break;
                        case "AP":
                            if (lockModel.MANE_CONTRACT_CODE != null && lockModel.IMS_PR_ROAD_CODE != null)
                            {
                                MANE_IMS_CONTRACT proposalContract = dbContext.MANE_IMS_CONTRACT.Find(lockModel.IMS_PR_ROAD_CODE, lockModel.MANE_CONTRACT_CODE);
                                proposalContract.MANE_LOCK_STATUS = (proposalContract.MANE_LOCK_STATUS == "N" ? "Y" : "N");

                                //added by abhishek kamble 28-nov-2013
                                proposalContract.USERID = PMGSYSession.Current.UserId;
                                proposalContract.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                                dbContext.SaveChanges();
                            }
                            break;
                        case "AC":
                            if (lockModel.MANE_CONTRACT_CODE != null && lockModel.IMS_PR_ROAD_CODE != null)
                            {
                                MANE_CN_CONTRACT coreNetworkContract = dbContext.MANE_CN_CONTRACT.Find(lockModel.PLAN_CN_ROAD_CODE, lockModel.MANE_CONTRACT_CODE);
                                coreNetworkContract.MANE_LOCK_STATUS = (coreNetworkContract.MANE_LOCK_STATUS == "N" ? "Y" : "N");

                                //added by abhishek kamble 28-nov-2013
                                coreNetworkContract.USERID = PMGSYSession.Current.UserId;
                                coreNetworkContract.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                                dbContext.SaveChanges();
                            }
                            break;
                        default:
                            break;
                    }

                    int? yearCode = lockModel.YearCode;
                    int? stateCode = lockModel.StateCode;
                    string packageCode = null;
                    if (lockModel.PackageCode == "0")
                    {
                        packageCode = null;
                    }
                    else
                    {
                        packageCode = lockModel.PackageCode;
                    }
                    if (lockModel.BatchCode == null)
                    {
                        lockModel.BatchCode = 0;
                    }

                    if (lockModel.LockStatus == "L")
                    {
                        switch (lockModel.IMS_UNLOCK_TABLE)
                        {
                            case "IR":
                                dbContext.sp_UpdateLockUnlockStatus(lockModel.YearCode, lockModel.StateCode, lockModel.BatchCode, packageCode, lockModel.DistrictCode, "Y", 1, "R", lockModel.BlockCode, PMGSYSession.Current.UserId, HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]);
                                dbContext.SaveChanges();
                                break;
                            case "ER":
                                dbContext.sp_UpdateLockUnlockStatus(lockModel.YearCode, lockModel.StateCode, lockModel.BatchCode, packageCode, lockModel.DistrictCode, "Y", 2, "R", lockModel.BlockCode, PMGSYSession.Current.UserId, HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]);
                                dbContext.SaveChanges();
                                break;
                            case "PR":
                                dbContext.sp_UpdateLockUnlockStatus(lockModel.YearCode, lockModel.StateCode, lockModel.BatchCode, packageCode, lockModel.DistrictCode, "Y", 3, "R", lockModel.BlockCode, PMGSYSession.Current.UserId, HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]);
                                dbContext.SaveChanges();
                                break;
                            case "AD":
                                if (lockModel.DistrictCode == 0)
                                {
                                    dbContext.TEND_AGREEMENT_MASTER.Where(m => m.MAST_STATE_CODE == lockModel.StateCode && m.TEND_AGREEMENT_START_DATE.Year == lockModel.YearCode).ToList().ForEach(m => { m.TEND_LOCK_STATUS = "Y"; m.USERID = PMGSYSession.Current.UserId; m.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]; });
                                }

                                dbContext.TEND_AGREEMENT_MASTER.Where(m => m.MAST_STATE_CODE == lockModel.StateCode && m.MAST_DISTRICT_CODE == lockModel.DistrictCode && m.TEND_AGREEMENT_START_DATE.Year == lockModel.YearCode).ToList().ForEach(m => { m.TEND_LOCK_STATUS = "Y"; m.USERID = PMGSYSession.Current.UserId; m.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]; });
                                dbContext.SaveChanges();
                                break;
                            case "NT":
                                if (lockModel.DistrictCode == 0)
                                {
                                    dbContext.TEND_NIT_MASTER.Where(m => m.MAST_STATE_CODE == lockModel.StateCode && m.TEND_ISSUE_START_DATE.Value.Year == lockModel.YearCode).ToList().ForEach(m => { m.TEND_LOCK_STATUS = "Y"; m.USERID = PMGSYSession.Current.UserId; m.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]; });
                                }
                                dbContext.TEND_NIT_MASTER.Where(m => m.MAST_STATE_CODE == lockModel.StateCode && m.MAST_DISTRICT_CODE == lockModel.DistrictCode && m.TEND_ISSUE_START_DATE.Value.Year == lockModel.YearCode).ToList().ForEach(m => { m.TEND_LOCK_STATUS = "Y"; m.USERID = PMGSYSession.Current.UserId; m.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]; });
                                dbContext.SaveChanges();
                                break;
                            case "AP":
                                //dbContext.MANE_IMS_CONTRACT.Where(m => m.MAST_STATE_CODE == lockModel.StateCode && m.MAST_DISTRICT_CODE == lockModel.DistrictCode && m.TEND_ISSUE_START_DATE.Value.Year == lockModel.YearCode).ToList().ForEach(m => m.TEND_LOCK_STATUS = "Y");
                                //dbContext.SaveChanges();
                                break;
                            case "AC":
                                //dbContext.TEND_NIT_MASTER.Where(m => m.MAST_STATE_CODE == lockModel.StateCode && m.MAST_DISTRICT_CODE == lockModel.DistrictCode && m.TEND_ISSUE_START_DATE.Value.Year == lockModel.YearCode).ToList().ForEach(m => m.TEND_LOCK_STATUS = "Y");
                                //dbContext.SaveChanges();
                                break;
                            default:
                                break;
                        }
                    }
                    else if (lockModel.LockStatus == "U")
                    {
                        switch (lockModel.IMS_UNLOCK_TABLE)
                        {
                            case "IR":
                                //dbContext.sp_UpdateLockUnlockStatus(lockModel.YearCode, lockModel.StateCode, lockModel.BatchCode, packageCode, lockModel.DistrictCode, "N", 1, "R", lockModel.BlockCode);
                                //dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_YEAR == yearCode && m.MAST_STATE_CODE == stateCode).ToList().ForEach(m => m.IMS_LOCK_STATUS = "N");

                                var listProjects = (from item in dbContext.IMS_SANCTIONED_PROJECTS
                                                    where lockModel.DataID.Contains(item.IMS_PR_ROAD_CODE)
                                                    select item).ToList();
                                listProjects.ForEach(m => { m.IMS_LOCK_STATUS = "N"; m.USERID = PMGSYSession.Current.UserId; m.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]; });
                                dbContext.SaveChanges();
                                break;
                            case "ER":
                                //dbContext.sp_UpdateLockUnlockStatus(lockModel.YearCode, lockModel.StateCode, lockModel.BatchCode, packageCode, lockModel.DistrictCode, "N", 2, "R", lockModel.BlockCode);
                                //dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_STATE_CODE == stateCode && m.MAST_DISTRICT_CODE == lockModel.DistrictCode && m.MAST_BLOCK_CODE == lockModel.BlockCode).ToList().ForEach(m => m.MAST_LOCK_STATUS = "N");
                                var listExistingRoads = (from item in dbContext.MASTER_EXISTING_ROADS
                                                         where lockModel.DataID.Contains(item.MAST_ER_ROAD_CODE)
                                                         select item).ToList();
                                listExistingRoads.ForEach(m => { m.MAST_LOCK_STATUS = "N"; m.USERID = PMGSYSession.Current.UserId; m.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]; });
                                dbContext.SaveChanges();
                                break;
                            case "PR":
                                //dbContext.sp_UpdateLockUnlockStatus(lockModel.YearCode, lockModel.StateCode, lockModel.BatchCode, packageCode, lockModel.DistrictCode, "N", 3, "R", lockModel.BlockCode);
                                //dbContext.PLAN_ROAD.Where(m => m.MAST_STATE_CODE == lockModel.StateCode && m.MAST_DISTRICT_CODE == lockModel.DistrictCode && m.MAST_BLOCK_CODE == lockModel.BlockCode).ToList().ForEach(m => m.PLAN_LOCK_STATUS = "N");
                                var listCoreNetworks = (from item in dbContext.PLAN_ROAD
                                                        where lockModel.DataID.Contains(item.PLAN_CN_ROAD_CODE)
                                                        select item).ToList();
                                listCoreNetworks.ForEach(m => { m.PLAN_LOCK_STATUS = "N"; m.USERID = PMGSYSession.Current.UserId; m.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]; });
                                dbContext.SaveChanges();
                                break;
                            case "AD":
                                dbContext.TEND_AGREEMENT_MASTER.Where(m => m.MAST_STATE_CODE == lockModel.StateCode && m.MAST_DISTRICT_CODE == lockModel.DistrictCode && m.TEND_AGREEMENT_START_DATE.Year == lockModel.YearCode).ToList().ForEach(m => { m.TEND_LOCK_STATUS = "N"; m.USERID = PMGSYSession.Current.UserId; m.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]; });
                                dbContext.SaveChanges();
                                break;
                            case "NT":
                                dbContext.TEND_NIT_MASTER.Where(m => m.MAST_STATE_CODE == lockModel.StateCode && m.MAST_DISTRICT_CODE == lockModel.DistrictCode && m.TEND_ISSUE_START_DATE.Value.Year == lockModel.YearCode).ToList().ForEach(m => { m.TEND_LOCK_STATUS = "N"; m.USERID = PMGSYSession.Current.UserId; m.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]; });
                                dbContext.SaveChanges();
                                break;
                            case "AP":
                                break;
                            case "AC":
                                break;
                            default:
                                break;
                        }
                    }
                    ts.Complete();
                    return true;
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);

                    return false;
                }
            }
        }

        public bool LockUnlockProposal(ProposalFilterViewModel model, ref string message)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                try
                {
                    dbContext = new PMGSYEntities();

                    ts.Complete();
                    return true;
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);

                    return false;
                }
                finally
                {
                    dbContext.Dispose();
                }
            }
        }

        /// <summary>
        /// returns the list of blocks for Block dropdown
        /// </summary>
        /// <param name="districtCode"></param>
        /// <returns></returns>
        public List<SelectListItem> GetBlocksByDistrictCode(int districtCode)
        {
            dbContext = new PMGSYEntities();
            try
            {
                List<SelectListItem> lstBlocks = new SelectList(dbContext.MASTER_BLOCK.Where(m => m.MAST_DISTRICT_CODE == districtCode).OrderBy(m => m.MAST_BLOCK_CODE).ToList(), "MAST_BLOCK_CODE", "MAST_BLOCK_NAME").ToList();
                lstBlocks.Add(new SelectListItem { Value = "0", Text = "--Select Block--", Selected = true });
                return lstBlocks;
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

        /// <summary>
        /// changes the lock status of the ids with respect to the module 
        /// </summary>
        /// <param name="arrIDs">array of id for converting the status</param>
        /// <param name="module">represents the module to convert the status.</param>
        /// <returns>result as true or false</returns>
        public bool ChangeLockStatus(int[] arrIDs, string module)
        {
            dbContext = new PMGSYEntities();
            try
            {
                switch (module)
                {
                    case "Proposal":
                        var listProposal = (from item in dbContext.IMS_SANCTIONED_PROJECTS
                                            where arrIDs.Contains(item.IMS_PR_ROAD_CODE)
                                            select item).ToList();
                        listProposal.ForEach(m => { m.IMS_LOCK_STATUS = "Y"; m.USERID = PMGSYSession.Current.UserId; m.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]; });

                        var lstProposalLockList = (from item in dbContext.IMS_LOCK_DETAILS
                                                   where arrIDs.Contains(item.IMS_PR_ROAD_CODE.Value)
                                                   select item).OrderByDescending(m => m.IMS_TRANSACTION_NO).Distinct().ToList();
                        lstProposalLockList.ForEach(m => m.IMS_AUTOLOCK_DATE = System.DateTime.Now);

                        break;
                    case "Core Network":
                        var corenetworkList = (from item in dbContext.PLAN_ROAD
                                               where arrIDs.Contains(item.PLAN_CN_ROAD_CODE)
                                               select item).ToList();
                        corenetworkList.ForEach(m => { m.PLAN_LOCK_STATUS = "Y"; m.USERID = PMGSYSession.Current.UserId; m.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]; });

                        var coreNetWorkLockList = (from item in dbContext.IMS_LOCK_DETAILS
                                                   where arrIDs.Contains(item.PLAN_CN_ROAD_CODE.Value)
                                                   select item).OrderByDescending(m => m.IMS_TRANSACTION_NO).Distinct().ToList();
                        coreNetWorkLockList.ForEach(m => m.IMS_AUTOLOCK_DATE = System.DateTime.Now);
                        break;
                    case "Existing Roads":
                        var existingroadList = (from item in dbContext.MASTER_EXISTING_ROADS
                                                where arrIDs.Contains(item.MAST_ER_ROAD_CODE)
                                                select item).ToList();
                        existingroadList.ForEach(m => { m.MAST_LOCK_STATUS = "Y"; m.USERID = PMGSYSession.Current.UserId; m.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]; });

                        var existingRoadsList = (from item in dbContext.IMS_LOCK_DETAILS
                                                 where arrIDs.Contains(item.MAST_ER_ROAD_CODE.Value)
                                                 select item).OrderByDescending(m => m.IMS_TRANSACTION_NO).Distinct().ToList();
                        existingRoadsList.ForEach(m => m.IMS_AUTOLOCK_DATE = System.DateTime.Now);
                        break;
                    default:
                        break;

                }
                dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);

                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        #endregion

        #region CORE_NETWORK

        /// <summary>
        /// returns the grid data for core network
        /// </summary>
        /// <param name="stateCode"></param>
        /// <param name="districtCode"></param>
        /// <param name="blockCode"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array GetCoreNetworkList(int stateCode, int districtCode, int blockCode, byte scheme, int collaboration, int roleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            totalRecords = 0;
            dbContext = new PMGSYEntities();
            try
            {
                // list of core network details according to the filters
                //var listCorenetwork = (from item in dbContext.PLAN_ROAD
                //                       join state in dbContext.MASTER_STATE on item.MAST_STATE_CODE equals state.MAST_STATE_CODE
                //                       join district in dbContext.MASTER_DISTRICT on item.MAST_DISTRICT_CODE equals district.MAST_DISTRICT_CODE
                //                       join block in dbContext.MASTER_BLOCK on item.MAST_BLOCK_CODE equals block.MAST_BLOCK_CODE
                //                       where
                //                       (stateCode == 0 ? 1 : item.MAST_STATE_CODE) == (stateCode == 0 ? 1 : stateCode) &&
                //                       (districtCode == 0 ? 1 : item.MAST_DISTRICT_CODE) == (districtCode == 0 ? 1 : districtCode) &&
                //                       (blockCode == 0 ? 1 : item.MAST_BLOCK_CODE) == (blockCode == 0 ? 1 : blockCode) &&
                //                       item.PLAN_LOCK_STATUS == "Y"
                //                       select new
                //                       {
                //                           item.PLAN_CN_ROAD_CODE,
                //                           block.MAST_BLOCK_NAME,
                //                           item.PLAN_RD_NAME,
                //                           item.PLAN_CN_ROAD_NUMBER,
                //                           item.PLAN_LOCK_STATUS,
                //                           item.PLAN_RD_FROM_CHAINAGE,
                //                           item.PLAN_RD_TO_CHAINAGE
                //                       }).Distinct();

                var listCorenetwork = dbContext.USP_UNLOCK_RECORDS("CN", "R", stateCode, districtCode, blockCode, 0, 0, 0, 0, 0, "%", null, scheme, collaboration, roleCode).ToList();
                totalRecords = listCorenetwork.Count();


                //sorting on each column
                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "LEVELNAME":
                                listCorenetwork = listCorenetwork.OrderBy(x => x.LEVELNAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "LEVELNUMBER":
                                listCorenetwork = listCorenetwork.OrderBy(x => x.LEVELNUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "STARTDATE":
                                listCorenetwork = listCorenetwork.OrderBy(x => x.STARTDATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "ENDDATE":
                                listCorenetwork = listCorenetwork.OrderBy(x => x.ENDDATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            //case "PLAN_RD_TO_CHAINAGE":
                            //    listCorenetwork = listCorenetwork.OrderBy(x => x.PLAN_RD_TO_CHAINAGE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                            //    break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "LEVELNAME":
                                listCorenetwork = listCorenetwork.OrderBy(x => x.LEVELNAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "LEVELNUMBER":
                                listCorenetwork = listCorenetwork.OrderBy(x => x.LEVELNUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "STARTDATE":
                                listCorenetwork = listCorenetwork.OrderBy(x => x.STARTDATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "ENDDATE":
                                listCorenetwork = listCorenetwork.OrderBy(x => x.ENDDATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            //case "PLAN_RD_TO_CHAINAGE":
                            //    listCorenetwork = listCorenetwork.OrderByDescending(x => x.PLAN_RD_TO_CHAINAGE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                            //    break;
                            default:
                                break;
                        }
                    }
                }
                else
                {
                    listCorenetwork = listCorenetwork.OrderByDescending(x => x.LEVELNAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                var gridData = listCorenetwork.Select(roadDetails => new
                {
                    roadDetails.LEVELCODE,
                    roadDetails.LEVELNAME,
                    roadDetails.LEVELNUMBER,
                    roadDetails.STARTDATE,
                    roadDetails.ENDDATE,
                }).ToArray();


                return gridData.Select(roadDetails => new
                {
                    id = roadDetails.LEVELCODE.ToString(),
                    cell = new[]{
                    
                        roadDetails.LEVELNAME == null?string.Empty:roadDetails.LEVELNAME.ToString(),
                        roadDetails.LEVELNUMBER == null?string.Empty:roadDetails.LEVELNUMBER.ToString(),
                        roadDetails.STARTDATE == null?"-":Convert.ToDateTime(roadDetails.STARTDATE).ToString("dd/MM/yyyy"),
                        roadDetails.ENDDATE == null?"-":Convert.ToDateTime(roadDetails.ENDDATE).ToString("dd/MM/yyyy"),
                        "<a href='#' title='Click to View Core Network Details' class='ui-icon ui-icon-search ui-align-center' onClick='ViewDetails(\"" + URLEncrypt.EncryptParameters(new string[]{roadDetails.LEVELCODE.ToString().Trim()+"$"+"R"}) +"\"); return false;'></a>"
                        //URLEncrypt.EncryptParameters1(new string[]{"NetworkCode="+roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim()}),
                        //roadDetails.PLAN_LOCK_STATUS=="N"?"<a href='#' title='Click to Lock Core Network Details' class='ui-icon ui-icon-locked ui-align-center' onClick='LockModuleUnit(\"" + URLEncrypt.EncryptParameters1(new string[]{"RoadCode="+ roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim() }) +"\"); return false;'></a>":"<a href='#' title='Click to Unlock Core Network Details' class='ui-icon ui-icon-unlocked ui-align-center' onClick='UnlockModuleUnit(\"" + URLEncrypt.EncryptParameters1(new string[]{"RoadCode="+ roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim() }) +"\"); return false;'></a>"
                        
                    }
                }).ToArray();

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

        /// <summary>
        /// returns the list of Core networks with status as unlocked
        /// </summary>
        /// <param name="stateCode"></param>
        /// <param name="districtCode"></param>
        /// <param name="blockCode"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array GetCoreNetworkUnlockList(int stateCode, int districtCode, int blockCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            dbContext = new PMGSYEntities();
            try
            {
                // list of core network details according to the filters
                var listCorenetwork = (from item in dbContext.PLAN_ROAD
                                       join state in dbContext.MASTER_STATE on item.MAST_STATE_CODE equals state.MAST_STATE_CODE
                                       join district in dbContext.MASTER_DISTRICT on item.MAST_DISTRICT_CODE equals district.MAST_DISTRICT_CODE
                                       join block in dbContext.MASTER_BLOCK on item.MAST_BLOCK_CODE equals block.MAST_BLOCK_CODE
                                       join lockDetails in dbContext.IMS_LOCK_DETAILS on item.PLAN_CN_ROAD_CODE equals lockDetails.PLAN_CN_ROAD_CODE
                                       where
                                       (stateCode == 0 ? 1 : item.MAST_STATE_CODE) == (stateCode == 0 ? 1 : stateCode) &&
                                       (districtCode == 0 ? 1 : item.MAST_DISTRICT_CODE) == (districtCode == 0 ? 1 : districtCode) &&
                                       (blockCode == 0 ? 1 : item.MAST_BLOCK_CODE) == (blockCode == 0 ? 1 : blockCode) &&
                                       lockDetails.IMS_AUTOLOCK_DATE > System.DateTime.Now &&
                                       lockDetails.IMS_UNLOCK_BY == "M" &&
                                       item.PLAN_LOCK_STATUS == "N"
                                       select new
                                       {
                                           item.PLAN_CN_ROAD_CODE,
                                           block.MAST_BLOCK_NAME,
                                           item.PLAN_RD_NAME,
                                           item.PLAN_CN_ROAD_NUMBER,
                                           item.PLAN_LOCK_STATUS,
                                           item.PLAN_RD_FROM_CHAINAGE,
                                           item.PLAN_RD_TO_CHAINAGE
                                       }).Distinct();


                totalRecords = listCorenetwork.Count();


                //sorting on each column
                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "MAST_BLOCK_NAME":
                                listCorenetwork = listCorenetwork.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "PLAN_RD_NAME":
                                listCorenetwork = listCorenetwork.OrderBy(x => x.PLAN_RD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "PLAN_CN_ROAD_NUMBER":
                                listCorenetwork = listCorenetwork.OrderBy(x => x.PLAN_CN_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "PLAN_RD_FROM_CHAINAGE":
                                listCorenetwork = listCorenetwork.OrderBy(x => x.PLAN_RD_FROM_CHAINAGE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "PLAN_RD_TO_CHAINAGE":
                                listCorenetwork = listCorenetwork.OrderBy(x => x.PLAN_RD_TO_CHAINAGE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "MAST_BLOCK_NAME":
                                listCorenetwork = listCorenetwork.OrderByDescending(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "PLAN_RD_NAME":
                                listCorenetwork = listCorenetwork.OrderByDescending(x => x.PLAN_RD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "PLAN_CN_ROAD_NUMBER":
                                listCorenetwork = listCorenetwork.OrderByDescending(x => x.PLAN_CN_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "PLAN_RD_FROM_CHAINAGE":
                                listCorenetwork = listCorenetwork.OrderByDescending(x => x.PLAN_RD_FROM_CHAINAGE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "PLAN_RD_TO_CHAINAGE":
                                listCorenetwork = listCorenetwork.OrderByDescending(x => x.PLAN_RD_TO_CHAINAGE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                break;
                        }
                    }
                }
                else
                {
                    listCorenetwork = listCorenetwork.OrderByDescending(x => x.PLAN_RD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var gridData = listCorenetwork.Select(roadDetails => new
                {
                    roadDetails.PLAN_CN_ROAD_CODE,
                    roadDetails.MAST_BLOCK_NAME,
                    roadDetails.PLAN_RD_NAME,
                    roadDetails.PLAN_CN_ROAD_NUMBER,
                    roadDetails.PLAN_LOCK_STATUS,
                    roadDetails.PLAN_RD_FROM_CHAINAGE,
                    roadDetails.PLAN_RD_TO_CHAINAGE
                }).ToArray();


                return gridData.Select(roadDetails => new
                {
                    id = roadDetails.PLAN_CN_ROAD_CODE.ToString(),
                    cell = new[]{
                    
                        roadDetails.MAST_BLOCK_NAME == null?string.Empty:roadDetails.MAST_BLOCK_NAME.ToString(),
                        roadDetails.PLAN_RD_NAME == null?string.Empty:roadDetails.PLAN_RD_NAME.ToString(),
                        roadDetails.PLAN_CN_ROAD_NUMBER == null?string.Empty:roadDetails.PLAN_CN_ROAD_NUMBER.ToString(),
                        roadDetails.PLAN_RD_FROM_CHAINAGE.ToString(),
                        roadDetails.PLAN_RD_TO_CHAINAGE.ToString(),
                        //URLEncrypt.EncryptParameters1(new string[]{"NetworkCode="+roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim()}),
                        //roadDetails.PLAN_LOCK_STATUS=="N"?"<a href='#' title='Click to Lock Core Network Details' class='ui-icon ui-icon-locked ui-align-center' onClick='LockModuleUnit(\"" + URLEncrypt.EncryptParameters1(new string[]{"RoadCode="+ roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim() }) +"\"); return false;'></a>":"<a href='#' title='Click to Unlock Core Network Details' class='ui-icon ui-icon-unlocked ui-align-center' onClick='UnlockModuleUnit(\"" + URLEncrypt.EncryptParameters1(new string[]{"RoadCode="+ roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim() }) +"\"); return false;'></a>"
                    }
                }).ToArray();

            }
            catch
            {
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        #endregion

        #region EXISTING_ROAD

        /// <summary>
        /// returns the data for populating the grid of Existing Roads
        /// </summary>
        /// <param name="stateCode"></param>
        /// <param name="districtCode"></param>
        /// <param name="blockCode"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array GetExistingRoadList(int stateCode, int districtCode, int blockCode, byte scheme, int collaboration, int roleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            totalRecords = 0;
            dbContext = new PMGSYEntities();
            try
            {
                //var existingRoadList = (from item in dbContext.MASTER_EXISTING_ROADS
                //                        join state in dbContext.MASTER_STATE on item.MAST_STATE_CODE equals state.MAST_STATE_CODE
                //                        join block in dbContext.MASTER_BLOCK on item.MAST_BLOCK_CODE equals block.MAST_BLOCK_CODE
                //                        join district in dbContext.MASTER_DISTRICT on item.MAST_DISTRICT_CODE equals district.MAST_DISTRICT_CODE
                //                        where
                //                        (stateCode == 0 ? 1 : item.MAST_STATE_CODE) == (stateCode == 0 ? 1 : stateCode) &&
                //                        (districtCode == 0 ? 1 : item.MAST_DISTRICT_CODE) == (districtCode == 0 ? 1 : districtCode) &&
                //                        (blockCode == 0 ? 1 : item.MAST_BLOCK_CODE) == (blockCode == 0 ? 1 : blockCode) &&
                //                        item.MAST_LOCK_STATUS == "Y"
                //                        select new
                //                        {
                //                            item.MAST_ER_ROAD_CODE,
                //                            item.MAST_ER_ROAD_NAME,
                //                            item.MAST_ER_ROAD_NUMBER,
                //                            block.MAST_BLOCK_NAME,
                //                            item.MAST_LOCK_STATUS,
                //                            item.MAST_ER_ROAD_STR_CHAIN,
                //                            item.MAST_ER_ROAD_END_CHAIN
                //                        }).Distinct();

                var existingRoadList = dbContext.USP_UNLOCK_RECORDS("ER", "R", stateCode, districtCode, blockCode, 0, 0, 0, 0, 0, "%", null, scheme, collaboration, roleCode).ToList();

                totalRecords = existingRoadList.Count();

                //sorting on each column of grid 
                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "LEVELNAME":
                                existingRoadList = existingRoadList.OrderBy(x => x.LEVELNAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "LEVELNUMBER":
                                existingRoadList = existingRoadList.OrderBy(x => x.LEVELNUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "STARTDATE":
                                existingRoadList = existingRoadList.OrderBy(x => x.STARTDATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "ENDDATE":
                                existingRoadList = existingRoadList.OrderBy(x => x.ENDDATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            //case "PLAN_RD_TO_CHAINAGE":
                            //    existingRoadList = existingRoadList.OrderBy(x => x.PLAN_RD_TO_CHAINAGE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                            //    break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "LEVELNAME":
                                existingRoadList = existingRoadList.OrderBy(x => x.LEVELNAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "LEVELNUMBER":
                                existingRoadList = existingRoadList.OrderBy(x => x.LEVELNUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "STARTDATE":
                                existingRoadList = existingRoadList.OrderBy(x => x.STARTDATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "ENDDATE":
                                existingRoadList = existingRoadList.OrderBy(x => x.ENDDATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            //case "PLAN_RD_TO_CHAINAGE":
                            //    existingRoadList = existingRoadList.OrderByDescending(x => x.PLAN_RD_TO_CHAINAGE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                            //    break;
                            default:
                                break;
                        }
                    }
                }
                else
                {
                    existingRoadList = existingRoadList.OrderByDescending(x => x.LEVELNAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                var gridData = existingRoadList.Select(roadDetails => new
                {
                    roadDetails.LEVELCODE,
                    roadDetails.LEVELNAME,
                    roadDetails.LEVELNUMBER,
                    roadDetails.STARTDATE,
                    roadDetails.ENDDATE,
                }).ToArray();


                return gridData.Select(roadDetails => new
                {
                    id = roadDetails.LEVELCODE.ToString(),
                    cell = new[]{
                    
                        roadDetails.LEVELNAME == null?string.Empty:roadDetails.LEVELNAME.ToString(),
                        roadDetails.LEVELNUMBER == null?string.Empty:roadDetails.LEVELNUMBER.ToString(),
                        roadDetails.STARTDATE == null?"-":Convert.ToDateTime(roadDetails.STARTDATE).ToString("dd/MM/yyyy"),
                        roadDetails.ENDDATE == null?"-":Convert.ToDateTime(roadDetails.ENDDATE).ToString("dd/MM/yyyy"),
                        "<a href='#' title='Click to View Existing Road Details' class='ui-icon ui-icon-search ui-align-center' onClick='ViewDetails(\"" + URLEncrypt.EncryptParameters(new string[]{roadDetails.LEVELCODE.ToString().Trim()+"$"+"R"}) +"\"); return false;'></a>"
                        //URLEncrypt.EncryptParameters1(new string[]{"NetworkCode="+roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim()}),
                        //roadDetails.PLAN_LOCK_STATUS=="N"?"<a href='#' title='Click to Lock Core Network Details' class='ui-icon ui-icon-locked ui-align-center' onClick='LockModuleUnit(\"" + URLEncrypt.EncryptParameters1(new string[]{"RoadCode="+ roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim() }) +"\"); return false;'></a>":"<a href='#' title='Click to Unlock Core Network Details' class='ui-icon ui-icon-unlocked ui-align-center' onClick='UnlockModuleUnit(\"" + URLEncrypt.EncryptParameters1(new string[]{"RoadCode="+ roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim() }) +"\"); return false;'></a>"
                    }
                }).ToArray();
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

        public Array GetExistingRoadUnlockList(int stateCode, int districtCode, int blockCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            totalRecords = 0;
            dbContext = new PMGSYEntities();
            try
            {
                var existingRoadList = (from item in dbContext.MASTER_EXISTING_ROADS
                                        join state in dbContext.MASTER_STATE on item.MAST_STATE_CODE equals state.MAST_STATE_CODE
                                        join block in dbContext.MASTER_BLOCK on item.MAST_BLOCK_CODE equals block.MAST_BLOCK_CODE
                                        join district in dbContext.MASTER_DISTRICT on item.MAST_DISTRICT_CODE equals district.MAST_DISTRICT_CODE
                                        join lockDetails in dbContext.IMS_LOCK_DETAILS on item.MAST_ER_ROAD_CODE equals lockDetails.MAST_ER_ROAD_CODE
                                        where
                                        (stateCode == 0 ? 1 : item.MAST_STATE_CODE) == (stateCode == 0 ? 1 : stateCode) &&
                                        (districtCode == 0 ? 1 : item.MAST_DISTRICT_CODE) == (districtCode == 0 ? 1 : districtCode) &&
                                        (blockCode == 0 ? 1 : item.MAST_BLOCK_CODE) == (blockCode == 0 ? 1 : blockCode) &&
                                        lockDetails.IMS_AUTOLOCK_DATE > System.DateTime.Now &&
                                        lockDetails.IMS_UNLOCK_BY == "M" &&
                                        item.MAST_LOCK_STATUS == "N"

                                        select new
                                        {
                                            item.MAST_ER_ROAD_CODE,
                                            item.MAST_ER_ROAD_NAME,
                                            item.MAST_ER_ROAD_NUMBER,
                                            block.MAST_BLOCK_NAME,
                                            item.MAST_LOCK_STATUS,
                                            item.MAST_ER_ROAD_END_CHAIN,
                                            item.MAST_ER_ROAD_STR_CHAIN
                                        }).Distinct();

                totalRecords = existingRoadList.Count();

                //sorting on each column of grid 
                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "MAST_BLOCK_NAME":
                                existingRoadList = existingRoadList.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_ER_ROAD_NAME":
                                existingRoadList = existingRoadList.OrderBy(x => x.MAST_ER_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_ER_ROAD_NUMBER":
                                existingRoadList = existingRoadList.OrderBy(x => x.MAST_ER_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_ER_ROAD_STR_CHAIN":
                                existingRoadList = existingRoadList.OrderBy(x => x.MAST_ER_ROAD_STR_CHAIN).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_ER_ROAD_END_CHAIN":
                                existingRoadList = existingRoadList.OrderBy(x => x.MAST_ER_ROAD_END_CHAIN).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "MAST_BLOCK_NAME":
                                existingRoadList = existingRoadList.OrderByDescending(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_ER_ROAD_NAME":
                                existingRoadList = existingRoadList.OrderByDescending(x => x.MAST_ER_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_ER_ROAD_NUMBER":
                                existingRoadList = existingRoadList.OrderByDescending(x => x.MAST_ER_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_ER_ROAD_STR_CHAIN":
                                existingRoadList = existingRoadList.OrderByDescending(x => x.MAST_ER_ROAD_STR_CHAIN).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_ER_ROAD_END_CHAIN":
                                existingRoadList = existingRoadList.OrderByDescending(x => x.MAST_ER_ROAD_END_CHAIN).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                break;
                        }
                    }
                }
                else
                {
                    existingRoadList = existingRoadList.OrderByDescending(x => x.MAST_ER_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var gridData = existingRoadList.Select(roadDetails => new
                {
                    roadDetails.MAST_BLOCK_NAME,
                    roadDetails.MAST_ER_ROAD_CODE,
                    roadDetails.MAST_ER_ROAD_NAME,
                    roadDetails.MAST_ER_ROAD_NUMBER,
                    roadDetails.MAST_LOCK_STATUS,
                    roadDetails.MAST_ER_ROAD_STR_CHAIN,
                    roadDetails.MAST_ER_ROAD_END_CHAIN
                }).ToArray();

                return gridData.Select(roadDetails => new
                {
                    id = roadDetails.MAST_ER_ROAD_CODE.ToString(),
                    cell = new[]{

                    roadDetails.MAST_BLOCK_NAME == null?string.Empty:roadDetails.MAST_BLOCK_NAME.ToString(),
                    roadDetails.MAST_ER_ROAD_NAME == null?string.Empty:roadDetails.MAST_ER_ROAD_NAME.ToString(),
                    roadDetails.MAST_ER_ROAD_NUMBER == null?string.Empty:roadDetails.MAST_ER_ROAD_NUMBER.ToString(),
                    roadDetails.MAST_ER_ROAD_STR_CHAIN.ToString(),
                    roadDetails.MAST_ER_ROAD_END_CHAIN.ToString()
                    //URLEncrypt.EncryptParameters1(new string[]{"ExistingCode="+roadDetails.MAST_ER_ROAD_CODE.ToString().Trim()}),
                    //roadDetails.MAST_LOCK_STATUS == "N"?"<a href='#' title='Click to Lock Existing Road Details' class='ui-icon ui-icon-locked ui-align-center' onClick='LockModuleUnit(\"" + URLEncrypt.EncryptParameters1(new string[]{"ExistingCode="+ roadDetails.MAST_ER_ROAD_CODE.ToString().Trim() }) +"\"); return false;'></a>":"<a href='#' title='Click to Unlock Existing Road Details' class='ui-icon ui-icon-unlocked ui-align-center' onClick='UnlockModuleUnit(\"" + URLEncrypt.EncryptParameters1(new string[]{"ExistingCode="+ roadDetails.MAST_ER_ROAD_CODE.ToString().Trim() }) +"\"); return false;'></a>"
                }
                }).ToArray();
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

        #region TENDERING

        /// <summary>
        /// returns the data for populating the grid of tendering
        /// </summary>
        /// <param name="stateCode"></param>
        /// <param name="districtCode"></param>
        /// <param name="yearCode"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array GetTenderingList(int stateCode, int districtCode, int yearCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            totalRecords = 0;
            dbContext = new PMGSYEntities();
            try
            {
                var tenderingList = (from item in dbContext.TEND_NIT_MASTER
                                     join year in dbContext.MASTER_YEAR on item.TEND_ISSUE_START_DATE.Value.Year equals year.MAST_YEAR_CODE
                                     join district in dbContext.MASTER_DISTRICT on item.MAST_DISTRICT_CODE equals district.MAST_DISTRICT_CODE
                                     where
                                     (stateCode == 0 ? 1 : item.MAST_STATE_CODE) == (stateCode == 0 ? 1 : stateCode) &&
                                     (districtCode == 0 ? 1 : item.MAST_DISTRICT_CODE) == (districtCode == 0 ? 1 : districtCode) &&
                                     (yearCode == 0 ? 1 : (item.TEND_ISSUE_START_DATE == null ? 1 : item.TEND_ISSUE_START_DATE.Value.Year)) == (yearCode == 0 ? 1 : yearCode)
                                     select new
                                     {
                                         item.TEND_NIT_NUMBER,
                                         district.MAST_DISTRICT_NAME,
                                         item.TEND_NIT_CODE,
                                         year.MAST_YEAR_TEXT,
                                         item.TEND_LOCK_STATUS,
                                         item.TEND_ISSUE_START_DATE,
                                         item.TEND_ISSUE_END_DATE
                                     });

                totalRecords = tenderingList.Count();
                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "MAST_DISTRICT_NAME":
                                tenderingList = tenderingList.OrderBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "TEND_NIT_NUMBER":
                                tenderingList = tenderingList.OrderBy(x => x.TEND_NIT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_YEAR_TEXT":
                                tenderingList = tenderingList.OrderBy(x => x.MAST_YEAR_TEXT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "MAST_DISTRICT_NAME":
                                tenderingList = tenderingList.OrderByDescending(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "TEND_NIT_NUMBER":
                                tenderingList = tenderingList.OrderByDescending(x => x.TEND_NIT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_YEAR_TEXT":
                                tenderingList = tenderingList.OrderByDescending(x => x.MAST_YEAR_TEXT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                break;
                        }
                    }
                }
                else
                {
                    tenderingList = tenderingList.OrderByDescending(x => x.TEND_NIT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var gridData = tenderingList.Select(tenderingDetails => new
                {
                    tenderingDetails.MAST_DISTRICT_NAME,
                    tenderingDetails.TEND_NIT_NUMBER,
                    tenderingDetails.MAST_YEAR_TEXT,
                    tenderingDetails.TEND_LOCK_STATUS,
                    tenderingDetails.TEND_NIT_CODE,
                    tenderingDetails.TEND_ISSUE_START_DATE,
                    tenderingDetails.TEND_ISSUE_END_DATE
                }).ToArray();

                return gridData.Select(tenderingDetails => new
                {
                    id = tenderingDetails.TEND_NIT_CODE.ToString(),
                    cell = new[]
                    {
                        tenderingDetails.MAST_YEAR_TEXT==null?string.Empty:tenderingDetails.MAST_YEAR_TEXT.ToString(),
                        tenderingDetails.MAST_DISTRICT_NAME==null?string.Empty:tenderingDetails.MAST_DISTRICT_NAME.ToString(),
                        tenderingDetails.TEND_NIT_NUMBER==null?string.Empty:tenderingDetails.TEND_NIT_NUMBER.ToString(),
                        tenderingDetails.TEND_ISSUE_START_DATE == null?"-":Convert.ToDateTime(tenderingDetails.TEND_ISSUE_START_DATE).ToString("dd/MM/yyyy"),
                        tenderingDetails.TEND_ISSUE_END_DATE == null?"-":Convert.ToDateTime(tenderingDetails.TEND_ISSUE_END_DATE).ToString("dd/MM/yyyy"),
                        URLEncrypt.EncryptParameters1(new string[]{"TenderingCode="+tenderingDetails.TEND_NIT_CODE.ToString().Trim()}),
                        tenderingDetails.TEND_LOCK_STATUS == "N"?"<a href='#' title='Click to Lock Tendering Details' class='ui-icon ui-icon-locked ui-align-center' onClick='LockModuleUnit(\"" + URLEncrypt.EncryptParameters1(new string[]{"TenderingCode="+ tenderingDetails.TEND_NIT_CODE.ToString().Trim() }) +"\"); return false;'></a>":"<a href='#' title='Click to Unlock Tendering Details' class='ui-icon ui-icon-unlocked ui-align-center' onClick='UnlockModuleUnit(\"" + URLEncrypt.EncryptParameters1(new string[]{"TenderingCode="+ tenderingDetails.TEND_NIT_CODE.ToString().Trim() }) +"\"); return false;'></a>"
                    }
                }).ToArray();
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

        #region AGREEMENT

        /// <summary>
        /// returns the data for populating the grid of agreement
        /// </summary>
        /// <param name="stateCode"></param>
        /// <param name="districtCode"></param>
        /// <param name="yearCode"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array GetAgreementList(int stateCode, int districtCode, int yearCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            totalRecords = 0;
            dbContext = new PMGSYEntities();
            try
            {

                //list of details according to the filters
                var agreementList = (from item in dbContext.TEND_AGREEMENT_MASTER
                                     join year in dbContext.MASTER_YEAR on item.TEND_AGREEMENT_START_DATE.Year equals year.MAST_YEAR_CODE
                                     join district in dbContext.MASTER_DISTRICT on item.MAST_DISTRICT_CODE equals district.MAST_DISTRICT_CODE
                                     join contractor in dbContext.MASTER_CONTRACTOR on item.MAST_CON_ID equals contractor.MAST_CON_ID
                                     where
                                     (stateCode == 0 ? 1 : item.MAST_STATE_CODE) == (stateCode == 0 ? 1 : stateCode) &&
                                     (districtCode == 0 ? 1 : item.MAST_DISTRICT_CODE) == (districtCode == 0 ? 1 : districtCode) &&
                                     (yearCode == 0 ? 1 : item.TEND_AGREEMENT_START_DATE.Year) == (yearCode == 0 ? 1 : yearCode)
                                     select new
                                     {
                                         contractor.MAST_CON_FNAME,
                                         contractor.MAST_CON_MNAME,
                                         contractor.MAST_CON_LNAME,
                                         item.TEND_AGREEMENT_CODE,
                                         district.MAST_DISTRICT_NAME,
                                         item.TEND_AGREEMENT_NUMBER,
                                         item.TEND_AGREEMENT_TYPE,
                                         item.TEND_DATE_OF_AGREEMENT,
                                         year.MAST_YEAR_TEXT,
                                         item.TEND_LOCK_STATUS,
                                         item.TEND_AGREEMENT_AMOUNT
                                     });

                totalRecords = agreementList.Count();

                //sorting on individual column
                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "TEND_AGREEMENT_NUMBER":
                                agreementList = agreementList.OrderBy(x => x.TEND_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_CON_NAME":
                                agreementList = agreementList.OrderBy(x => x.MAST_CON_FNAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "TEND_AGREEMENT_TYPE":
                                agreementList = agreementList.OrderBy(x => x.TEND_AGREEMENT_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "TEND_DATE_OF_AGREEMENT":
                                agreementList = agreementList.OrderBy(x => x.TEND_DATE_OF_AGREEMENT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "TEND_AGREEMENT_AMOUNT":
                                agreementList = agreementList.OrderBy(x => x.TEND_AGREEMENT_AMOUNT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "TEND_AGREEMENT_NUMBER":
                                agreementList = agreementList.OrderByDescending(x => x.TEND_AGREEMENT_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_CON_NAME":
                                agreementList = agreementList.OrderByDescending(x => x.MAST_CON_FNAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "TEND_AGREEMENT_TYPE":
                                agreementList = agreementList.OrderByDescending(x => x.TEND_AGREEMENT_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "TEND_DATE_OF_AGREEMENT":
                                agreementList = agreementList.OrderByDescending(x => x.TEND_DATE_OF_AGREEMENT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "TEND_AGREEMENT_AMOUNT":
                                agreementList = agreementList.OrderByDescending(x => x.TEND_AGREEMENT_AMOUNT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                break;
                        }
                    }
                }
                else
                {
                    agreementList = agreementList.OrderByDescending(x => x.TEND_AGREEMENT_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var gridData = agreementList.Select(aggDetails => new
                {
                    aggDetails.MAST_CON_FNAME,
                    aggDetails.MAST_CON_LNAME,
                    aggDetails.MAST_CON_MNAME,
                    aggDetails.TEND_AGREEMENT_TYPE,
                    aggDetails.TEND_DATE_OF_AGREEMENT,
                    aggDetails.TEND_AGREEMENT_AMOUNT,
                    aggDetails.MAST_DISTRICT_NAME,
                    aggDetails.TEND_AGREEMENT_NUMBER,
                    aggDetails.MAST_YEAR_TEXT,
                    aggDetails.TEND_AGREEMENT_CODE,
                    aggDetails.TEND_LOCK_STATUS
                }).ToArray();


                //returning the grid data along with encrypted id
                return gridData.Select(roadDetails => new
                {
                    id = roadDetails.TEND_AGREEMENT_CODE.ToString(),
                    cell = new[]{
                    roadDetails.TEND_AGREEMENT_NUMBER==null?string.Empty:roadDetails.TEND_AGREEMENT_NUMBER.ToString(),
                    (roadDetails.MAST_CON_FNAME==null?string.Empty:roadDetails.MAST_CON_FNAME)+" "+(roadDetails.MAST_CON_MNAME==null?string.Empty:roadDetails.MAST_CON_MNAME.ToString())+" "+(roadDetails.MAST_CON_LNAME==null?string.Empty:roadDetails.MAST_CON_LNAME.ToString()),
                    roadDetails.TEND_AGREEMENT_TYPE == null?string.Empty:(roadDetails.TEND_AGREEMENT_TYPE=="C"?"Contractor":roadDetails.TEND_AGREEMENT_TYPE=="S"?"Supplier":(roadDetails.TEND_AGREEMENT_TYPE=="O"?"Other Road":(roadDetails.TEND_AGREEMENT_TYPE=="D"?"DPR":roadDetails.TEND_AGREEMENT_TYPE.ToString()))),
                    roadDetails.TEND_DATE_OF_AGREEMENT == null?"-":Convert.ToDateTime(roadDetails.TEND_DATE_OF_AGREEMENT).ToString("dd/MM/yyyy"),
                    roadDetails.TEND_AGREEMENT_AMOUNT == null?string.Empty:roadDetails.TEND_AGREEMENT_AMOUNT.ToString(),
                    URLEncrypt.EncryptParameters1(new string[]{"AgreementCode="+roadDetails.TEND_AGREEMENT_CODE.ToString().Trim()}),
                    roadDetails.TEND_LOCK_STATUS == "N"?"<a href='#' title='Click to Lock Agreement Details' class='ui-icon ui-icon-locked ui-align-center' onClick='LockModuleUnit(\"" + URLEncrypt.EncryptParameters1(new string[]{"AgreementCode="+ roadDetails.TEND_AGREEMENT_CODE.ToString().Trim() }) +"\"); return false;'></a>":"<a href='#' title='Click to Unlock Agreement Details' class='ui-icon ui-icon-unlocked ui-align-center' onClick='UnlockModuleUnit(\"" + URLEncrypt.EncryptParameters1(new string[]{"AgreementCode="+ roadDetails.TEND_AGREEMENT_CODE.ToString().Trim() }) +"\"); return false;'></a>"
                }
                }).ToArray();

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

        #region CONTRACT

        /// <summary>
        /// returns Core Network Contract list
        /// </summary>
        /// <param name="stateCode"></param>
        /// <param name="districtCode"></param>
        /// <param name="blockCode"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array GetManeCoreNetworkList(int stateCode, int districtCode, int blockCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            totalRecords = 0;
            dbContext = new PMGSYEntities();
            try
            {
                var contractList = (from item in dbContext.MANE_CN_CONTRACT
                                    join road in dbContext.PLAN_ROAD on item.PLAN_CN_ROAD_CODE equals road.PLAN_CN_ROAD_CODE
                                    join year in dbContext.MASTER_YEAR on item.MANE_AGREEMENT_DATE.Year equals year.MAST_YEAR_CODE
                                    join block in dbContext.MASTER_BLOCK on road.MAST_BLOCK_CODE equals block.MAST_BLOCK_CODE
                                    where
                                    (stateCode == 0 ? 1 : road.MAST_STATE_CODE) == (stateCode == 0 ? 1 : stateCode) &&
                                    (districtCode == 0 ? 1 : road.MAST_DISTRICT_CODE) == (districtCode == 0 ? 1 : districtCode) &&
                                    (blockCode == 0 ? 1 : road.MAST_BLOCK_CODE) == (blockCode == 0 ? 1 : blockCode)
                                    select new
                                    {
                                        item.MANE_CN_CONTRACT_CODE,
                                        item.PLAN_CN_ROAD_CODE,
                                        road.PLAN_RD_NAME,
                                        block.MAST_BLOCK_NAME,
                                        year.MAST_YEAR_TEXT,
                                        item.MANE_LOCK_STATUS
                                    });

                totalRecords = contractList.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "MAST_BLOCK_NAME":
                                contractList = contractList.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "PLAN_RD_NAME":
                                contractList = contractList.OrderBy(x => x.PLAN_RD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_YEAR_TEXT":
                                contractList = contractList.OrderBy(x => x.MAST_YEAR_TEXT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "MAST_BLOCK_NAME":
                                contractList = contractList.OrderByDescending(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "PLAN_RD_NAME":
                                contractList = contractList.OrderByDescending(x => x.PLAN_RD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_YEAR_TEXT":
                                contractList = contractList.OrderByDescending(x => x.MAST_YEAR_TEXT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                break;
                        }
                    }
                }
                else
                {
                    contractList = contractList.OrderByDescending(x => x.PLAN_RD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var gridData = contractList.Select(roadDetails => new
                {
                    roadDetails.MAST_BLOCK_NAME,
                    roadDetails.MANE_CN_CONTRACT_CODE,
                    roadDetails.MANE_LOCK_STATUS,
                    roadDetails.MAST_YEAR_TEXT,
                    roadDetails.PLAN_CN_ROAD_CODE,
                    roadDetails.PLAN_RD_NAME

                }).ToArray();

                return gridData.Select(roadDetails => new
                {
                    id = roadDetails.MANE_CN_CONTRACT_CODE.ToString() + ',' + roadDetails.PLAN_CN_ROAD_CODE.ToString(),
                    cell = new[]
                    {
                        roadDetails.MAST_BLOCK_NAME == null?string.Empty:roadDetails.MAST_BLOCK_NAME.ToString(),
                        roadDetails.PLAN_RD_NAME == null?string.Empty:roadDetails.PLAN_RD_NAME.ToString(),
                        roadDetails.MAST_YEAR_TEXT == null?string.Empty:roadDetails.MAST_YEAR_TEXT.ToString(),
                        URLEncrypt.EncryptParameters1(new string[]{"RoadCode="+roadDetails.PLAN_CN_ROAD_CODE.ToString(),"ContractCode="+roadDetails.MANE_CN_CONTRACT_CODE.ToString()}),
                        roadDetails.MANE_LOCK_STATUS == "N"?"<a href='#' title='Click to Lock Core Network Contract Details' class='ui-icon ui-icon-locked ui-align-center' onClick='LockCNContractUnit(\"" + URLEncrypt.EncryptParameters1(new string[]{"RoadCode="+ roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim(),"ContractCode="+roadDetails.MANE_CN_CONTRACT_CODE.ToString().Trim() }) +"\"); return false;'></a>":"<a href='#' title='Click to Unlock Core Network Contract Details' class='ui-icon ui-icon-unlocked ui-align-center' onClick='UnlockCNContractUnit(\"" + URLEncrypt.EncryptParameters1(new string[]{"RoadCode="+ roadDetails.PLAN_CN_ROAD_CODE.ToString().Trim(),"ContractCode="+roadDetails.MANE_CN_CONTRACT_CODE.ToString().Trim() }) +"\"); return false;'></a>"
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);


                throw;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// returns the list of proposal contract 
        /// </summary>
        /// <param name="yearCode"></param>
        /// <param name="stateCode"></param>
        /// <param name="districtCode"></param>
        /// <param name="batchCode"></param>
        /// <param name="packageCode"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array GetIMSContractList(int yearCode, int stateCode, int districtCode, int batchCode, string packageCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            dbContext = new PMGSYEntities();
            totalRecords = 0;
            try
            {
                var imsContractList = (from item in dbContext.MANE_IMS_CONTRACT
                                       join proposal in dbContext.IMS_SANCTIONED_PROJECTS on item.IMS_PR_ROAD_CODE equals proposal.IMS_PR_ROAD_CODE
                                       join year in dbContext.MASTER_YEAR on proposal.IMS_YEAR equals year.MAST_YEAR_CODE
                                       join block in dbContext.MASTER_BLOCK on proposal.MAST_BLOCK_CODE equals block.MAST_BLOCK_CODE
                                       where
                                       (yearCode == 0 ? 1 : proposal.IMS_YEAR) == (yearCode == 0 ? 1 : yearCode) &&
                                       (stateCode == 0 ? 1 : proposal.MAST_STATE_CODE) == (stateCode == 0 ? 1 : stateCode) &&
                                       (districtCode == 0 ? 1 : proposal.MAST_DISTRICT_CODE) == (districtCode == 0 ? 1 : districtCode) &&
                                       (batchCode == 0 ? 1 : proposal.IMS_BATCH) == (batchCode == 0 ? 1 : batchCode) &&
                                       (packageCode == "0" ? "%" : proposal.IMS_PACKAGE_ID) == (packageCode == "0" ? "%" : packageCode)
                                       select new
                                       {
                                           item.IMS_PR_ROAD_CODE,
                                           item.MANE_PR_CONTRACT_CODE,
                                           proposal.IMS_ROAD_NAME,
                                           proposal.IMS_PACKAGE_ID,
                                           year.MAST_YEAR_TEXT,
                                           block.MAST_BLOCK_NAME,
                                           proposal.IMS_BATCH,
                                           item.MANE_LOCK_STATUS
                                       });

                totalRecords = imsContractList.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "PLAN_RD_NAME":
                                imsContractList = imsContractList.OrderBy(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_BLOCK_NAME":
                                imsContractList = imsContractList.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_YEAR_TEXT":
                                imsContractList = imsContractList.OrderBy(x => x.MAST_YEAR_TEXT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "PLAN_RD_NAME":
                                imsContractList = imsContractList.OrderByDescending(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_BLOCK_NAME":
                                imsContractList = imsContractList.OrderByDescending(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "MAST_YEAR_TEXT":
                                imsContractList = imsContractList.OrderByDescending(x => x.MAST_YEAR_TEXT).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                break;
                        }
                    }
                }
                else
                {
                    imsContractList = imsContractList.OrderByDescending(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var gridData = imsContractList.Select(imsDetails => new
                {
                    imsDetails.IMS_PR_ROAD_CODE,
                    imsDetails.MANE_PR_CONTRACT_CODE,
                    imsDetails.IMS_ROAD_NAME,
                    imsDetails.MAST_YEAR_TEXT,
                    imsDetails.MAST_BLOCK_NAME,
                    imsDetails.MANE_LOCK_STATUS

                }).ToArray();

                return gridData.Select(imsDetails => new
                {
                    id = imsDetails.IMS_PR_ROAD_CODE.ToString() + "," + imsDetails.MANE_PR_CONTRACT_CODE.ToString(),
                    cell = new[]
                    {
                        imsDetails.MAST_BLOCK_NAME== null?string.Empty:imsDetails.MAST_BLOCK_NAME.ToString(),
                        imsDetails.IMS_ROAD_NAME == null?string.Empty:imsDetails.IMS_ROAD_NAME.ToString(),
                        imsDetails.MAST_YEAR_TEXT == null?string.Empty:imsDetails.MAST_YEAR_TEXT.ToString(),
                        URLEncrypt.EncryptParameters1(new string[]{"ProposalCode="+imsDetails.IMS_PR_ROAD_CODE.ToString().Trim(),"ContractCode="+imsDetails.MANE_PR_CONTRACT_CODE.ToString().Trim()}),
                        imsDetails.MANE_LOCK_STATUS == "N"?"<a href='#' title='Click to Lock IMS Contract Details' class='ui-icon ui-icon-locked ui-align-center' onClick='LockIMSContractUnit(\"" + URLEncrypt.EncryptParameters1(new string[]{"ProposalCode="+ imsDetails.IMS_PR_ROAD_CODE.ToString().Trim(),"ContractCode="+imsDetails.MANE_PR_CONTRACT_CODE.ToString().Trim() }) +"\"); return false;'></a>":"<a href='#' title='Click to Unlock Proposal Contract Details' class='ui-icon ui-icon-unlocked ui-align-center' onClick='UnlockIMSContractUnit(\"" + URLEncrypt.EncryptParameters1(new string[]{"ProposalCode="+ imsDetails.IMS_PR_ROAD_CODE.ToString().Trim(),"ContractCode="+imsDetails.MANE_PR_CONTRACT_CODE.ToString().Trim() }) +"\"); return false;'></a>"
                    }
                }).ToArray();

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


        #endregion

        #region UNLOCK


        public List<SelectListItem> GetVillagesByBlockCode(int blockCode)
        {
            dbContext = new PMGSYEntities();
            try
            {
                List<SelectListItem> lstVillages = new SelectList(dbContext.MASTER_VILLAGE.Where(m => m.MAST_BLOCK_CODE == blockCode).Distinct().ToList(), "MAST_VILLAGE_CODE", "MAST_VILLAGE_NAME").ToList();
                return lstVillages;
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

        public List<SelectListItem> GetHabsByVillageCode(int villageCode)
        {
            dbContext = new PMGSYEntities();
            try
            {
                List<SelectListItem> lstHabs = new SelectList(dbContext.MASTER_HABITATIONS.Where(m => m.MAST_VILLAGE_CODE == villageCode).ToList(), "MAST_HAB_CODE", "MAST_HAB_NAME").ToList();
                return lstHabs;
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

        public Array GetStateList(string moduleCode, byte scheme, int collaboration, int roleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            dbContext = new PMGSYEntities();
            try
            {
                //var lstStates = dbContext.MASTER_STATE.ToList();
                var lstStates = dbContext.USP_UNLOCK_RECORDS(moduleCode, "S", 0, 0, 0, 0, 0, 0, 0, 0, "%", null, scheme, collaboration, roleCode).ToList();

                totalRecords = lstStates.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "MAST_STATE_NAME":
                                lstStates = lstStates.OrderBy(x => x.LEVELNAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_UNLOCK_START_DATE":
                                lstStates = lstStates.OrderBy(x => x.STARTDATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_UNLOCK_END_DATE":
                                lstStates = lstStates.OrderBy(x => x.ENDDATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstStates = lstStates.OrderBy(x => x.LEVELNAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "MAST_STATE_NAME":
                                lstStates = lstStates.OrderByDescending(x => x.LEVELNAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_UNLOCK_START_DATE":
                                lstStates = lstStates.OrderByDescending(x => x.STARTDATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_UNLOCK_END_DATE":
                                lstStates = lstStates.OrderByDescending(x => x.ENDDATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstStates = lstStates.OrderByDescending(x => x.LEVELNAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstStates = lstStates.OrderBy(x => x.LEVELNAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                return lstStates.Select(propDetails => new
                {
                    id = propDetails.LEVELCODE.ToString(),
                    cell = new[]
                    {                         
                        propDetails.LEVELNAME ==null?string.Empty:propDetails.LEVELNAME.ToString(),
                        propDetails.STARTDATE ==null?"-":Convert.ToDateTime(propDetails.STARTDATE).ToString("dd/MM/yyyy"),
                        propDetails.ENDDATE ==null?"-":Convert.ToDateTime(propDetails.ENDDATE).ToString("dd/MM/yyyy"),
                        "<a href='#' title='Click to Lock Proposal Details' class='ui-icon ui-icon-search ui-align-center' onClick='ViewDetails(\"" + URLEncrypt.EncryptParameters(new string[]{propDetails.LEVELCODE.ToString().Trim()+"$"+"S"}) +"\"); return false;'></a>"
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

        public Array GetDistrictList(string moduleCode, int stateCode, byte scheme, int collaboration, int roleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            dbContext = new PMGSYEntities();
            try
            {
                //var lstStates = dbContext.MASTER_DISTRICT.Where(m=>m.MAST_STATE_CODE == stateCode).ToList();

                var lstStates = dbContext.USP_UNLOCK_RECORDS(moduleCode, "D", stateCode, 0, 0, 0, 0, 0, 0, 0, "%", null, scheme, collaboration, roleCode).ToList();




                totalRecords = lstStates.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "MAST_STATE_NAME":
                                lstStates = lstStates.OrderBy(x => x.LEVELNAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_UNLOCK_START_DATE":
                                lstStates = lstStates.OrderBy(x => x.STARTDATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_UNLOCK_END_DATE":
                                lstStates = lstStates.OrderBy(x => x.ENDDATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstStates = lstStates.OrderBy(x => x.LEVELNAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "MAST_DISTRICT_NAME":
                                lstStates = lstStates.OrderByDescending(x => x.LEVELNAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_UNLOCK_START_DATE":
                                lstStates = lstStates.OrderByDescending(x => x.STARTDATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_UNLOCK_END_DATE":
                                lstStates = lstStates.OrderByDescending(x => x.ENDDATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstStates = lstStates.OrderByDescending(x => x.LEVELNAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstStates = lstStates.OrderBy(x => x.LEVELNAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                return lstStates.Select(propDetails => new
                {
                    id = propDetails.LEVELCODE.ToString(),
                    cell = new[]
                    {                         
                        propDetails.LEVELNAME ==null?string.Empty:propDetails.LEVELNAME.ToString(),
                        propDetails.STARTDATE ==null?"-":Convert.ToDateTime(propDetails.STARTDATE).ToString("dd/MM/yyyy"),
                        propDetails.ENDDATE ==null?"-":Convert.ToDateTime(propDetails.ENDDATE).ToString("dd/MM/yyyy"),
                        "<a href='#' title='Click to Lock Proposal Details' class='ui-icon ui-icon-search ui-align-center' onClick='ViewDetails(\"" + URLEncrypt.EncryptParameters(new string[]{propDetails.LEVELCODE.ToString().Trim()+"$"+"D"}) +"\"); return false;'></a>"
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

        public Array GetYearsList(string moduleCode, int stateCode, int districtCode, int blockCode, byte scheme, int collaboration, int roleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            dbContext = new PMGSYEntities();
            try
            {
                var lstYears = dbContext.USP_UNLOCK_RECORDS(moduleCode, "Y", stateCode, (districtCode <= 0 ? 0 : districtCode), (blockCode <= 0 ? 0 : blockCode), 0, 0, 0, 0, 0, "%", null, scheme, collaboration, roleCode).ToList();

                totalRecords = lstYears.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "MAST_YEAR_NAME":
                                lstYears = lstYears.OrderBy(x => x.LEVELNAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_UNLOCK_START_DATE":
                                lstYears = lstYears.OrderBy(x => x.STARTDATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_UNLOCK_END_DATE":
                                lstYears = lstYears.OrderBy(x => x.ENDDATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstYears = lstYears.OrderBy(x => x.LEVELNAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "MAST_YEAR_NAME":
                                lstYears = lstYears.OrderByDescending(x => x.LEVELNAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_UNLOCK_START_DATE":
                                lstYears = lstYears.OrderByDescending(x => x.STARTDATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_UNLOCK_END_DATE":
                                lstYears = lstYears.OrderByDescending(x => x.ENDDATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstYears = lstYears.OrderByDescending(x => x.LEVELNAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstYears = lstYears.OrderBy(x => x.LEVELNAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }
                              
                return lstYears.Select(propDetails => new
                {
                    id = propDetails.LEVELCODE.ToString(),
                    cell = new[]
                    {                         
                        propDetails.LEVELNAME ==null?string.Empty:propDetails.LEVELNAME.ToString(),
                        propDetails.STARTDATE ==null?"-":Convert.ToDateTime(propDetails.STARTDATE).ToString("dd/MM/yyyy"),
                        propDetails.ENDDATE ==null?"-":Convert.ToDateTime(propDetails.ENDDATE).ToString("dd/MM/yyyy"),
                        "<a href='#' title='Click to Lock Proposal Details' class='ui-icon ui-icon-search ui-align-center' onClick='ViewDetails(\"" + URLEncrypt.EncryptParameters(new string[]{propDetails.LEVELCODE.ToString().Trim()+"$"+"Y"}) +"\"); return false;'></a>"
                    }
                }).ToArray();

            }
            catch (Exception ex)
            {
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString());
                    sw.WriteLine("Method : " + "GetYearsListDAL()");
                    if (ex != null)
                        sw.WriteLine("Exception : " + ex.ToString());
                    if (ex.InnerException != null)
                        sw.WriteLine("ex.InnerException : " + ex.InnerException.ToString());
                    if (ex.InnerException.InnerException != null)
                        sw.WriteLine("ex.InnerException.InnerException : " + ex.InnerException.InnerException.ToString());
                    sw.WriteLine("---------------------------------------------------------------------------------------");
                    sw.Close();
                }
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);

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


        public Array GetBatchesList(string moduleCode, int stateCode, int districtCode, int blockCode, int yearCode, byte scheme, int collaboration, int roleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            dbContext = new PMGSYEntities();
            try
            {
                var lstBatches = dbContext.USP_UNLOCK_RECORDS(moduleCode, "T", stateCode, (districtCode <= 0 ? 0 : districtCode), (blockCode <= 0 ? 0 : blockCode), 0, 0, 0, (yearCode <= 0 ? 0 : yearCode), 0, "%", null, scheme, collaboration, roleCode).ToList();

                totalRecords = lstBatches.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "MAST_BATCH_NAME":
                                lstBatches = lstBatches.OrderBy(x => x.LEVELNAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_UNLOCK_START_DATE":
                                lstBatches = lstBatches.OrderBy(x => x.STARTDATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_UNLOCK_END_DATE":
                                lstBatches = lstBatches.OrderBy(x => x.ENDDATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstBatches = lstBatches.OrderBy(x => x.LEVELNAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "MAST_BATCH_NAME":
                                lstBatches = lstBatches.OrderByDescending(x => x.LEVELNAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_UNLOCK_START_DATE":
                                lstBatches = lstBatches.OrderByDescending(x => x.STARTDATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_UNLOCK_END_DATE":
                                lstBatches = lstBatches.OrderByDescending(x => x.ENDDATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstBatches = lstBatches.OrderByDescending(x => x.LEVELNAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstBatches = lstBatches.OrderBy(x => x.LEVELNAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                // Added By to unlock Proposal Technology Details
                // Bug fixing: id + rowcount conditions added for disable all repeated year where unlocking start & end date is available
                int rowcount = 1;
                foreach (var item in lstBatches)
                {
                    if (item.STARTDATE != null)
                    {
                        item.LEVELCODE = Convert.ToInt32(item.LEVELCODE.ToString() + rowcount);
                        rowcount++;
                    }
                }

                return lstBatches.Select(propDetails => new
                {
                    id = propDetails.LEVELCODE.ToString(),
                    cell = new[]
                    {                         
                        propDetails.LEVELNAME ==null?string.Empty:propDetails.LEVELNAME.ToString(),
                        propDetails.STARTDATE ==null?"-":Convert.ToDateTime(propDetails.STARTDATE).ToString("dd/MM/yyyy"),
                        propDetails.ENDDATE ==null?"-":Convert.ToDateTime(propDetails.ENDDATE).ToString("dd/MM/yyyy"),
                        "<a href='#' title='Click to Lock Proposal Details' class='ui-icon ui-icon-search ui-align-center' onClick='ViewDetails(\"" + URLEncrypt.EncryptParameters(new string[]{propDetails.LEVELCODE.ToString().Trim()+"$"+"T"}) +"\"); return false;'></a>"
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


        public Array GetBlockList(string moduleCode, int stateCode, int districtCode, byte scheme, int collaboration, int roleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            dbContext = new PMGSYEntities();
            try
            {
                //var lstStates = dbContext.USP_UNLOCK_RECORDS(moduleCode, "B", 0, districtCode, 0, 0, 0, 0, 0, 0, "%", null, scheme, roleCode).ToList();
                var lstStates = dbContext.USP_UNLOCK_RECORDS(moduleCode, "B", stateCode, districtCode, 0, 0, 0, 0, 0, 0, "%", null, scheme, collaboration, roleCode).ToList();

                #region
                //var lstStates = dbContext.MASTER_BLOCK.Where(m => m.MAST_DISTRICT_CODE == districtCode).ToList();

                //var lstStates = (IEnumerable<dynamic>)null;

                //if (PMGSYSession.Current.RoleCode == 36)
                //{
                //    lstStates = (from item in dbContext.MASTER_BLOCK
                //                 join unlockDetails in dbContext.IMS_UNLOCK_DETAILS on item.MAST_BLOCK_CODE equals unlockDetails.MAST_BLOCK_CODE into details
                //                 from subDetails in details.DefaultIfEmpty()
                //                 where
                //                 item.MAST_DISTRICT_CODE == districtCode &&
                //                 (subDetails.IMS_UNLOCK_TABLE == null ? "%" : subDetails.IMS_UNLOCK_TABLE) == (subDetails.IMS_UNLOCK_TABLE == null ? "%":moduleCode) &&
                //                 (subDetails.MAST_PMGSY_SCHEME == null?1: subDetails.MAST_PMGSY_SCHEME) == (subDetails.MAST_PMGSY_SCHEME == null ? 1 :scheme )&&
                //                 (subDetails.IMS_UNLOCK_BY == null?"%":subDetails.IMS_UNLOCK_BY) == (subDetails.IMS_UNLOCK_BY == null?"%": "N" )&&
                //                 (subDetails.IMS_UNLOCK_STATUS==null?"%":subDetails.IMS_UNLOCK_STATUS )  == (subDetails.IMS_UNLOCK_STATUS == null?"%":"N") &&
                //                 ((subDetails.IMS_UNLOCK_END_DATE == null? DateTime.MinValue:subDetails.IMS_UNLOCK_END_DATE) >= System.DateTime.Now)
                //                 select new
                //                 {
                //                     LEVELCODE = item.MAST_BLOCK_CODE,
                //                     LEVELNAME = item.MAST_BLOCK_NAME,
                //                     STARTDATE = (DateTime?)subDetails.IMS_UNLOCK_START_DATE ,
                //                     ENDDATE = (DateTime?)subDetails.IMS_UNLOCK_END_DATE
                //                 }).Distinct().ToList();

                //}
                //else
                //{
                //    lstStates = dbContext.USP_UNLOCK_RECORDS(moduleCode, "B", 0, districtCode, 0, 0, 0, 0, 0, 0, null, scheme).ToList();
                //}
                #endregion

                totalRecords = lstStates.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "MAST_BLOCK_NAME":
                                lstStates = lstStates.OrderBy(x => x.LEVELNAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_UNLOCK_START_DATE":
                                lstStates = lstStates.OrderBy(x => x.STARTDATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_UNLOCK_END_DATE":
                                lstStates = lstStates.OrderBy(x => x.ENDDATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstStates = lstStates.OrderBy(x => x.LEVELNAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "MAST_BLOCK_NAME":
                                lstStates = lstStates.OrderByDescending(x => x.LEVELNAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_UNLOCK_START_DATE":
                                lstStates = lstStates.OrderByDescending(x => x.STARTDATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_UNLOCK_END_DATE":
                                lstStates = lstStates.OrderByDescending(x => x.ENDDATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstStates = lstStates.OrderByDescending(x => x.LEVELNAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstStates = lstStates.OrderBy(x => x.LEVELNAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                return lstStates.Select(propDetails => new
                {
                    id = propDetails.LEVELCODE.ToString(),
                    cell = new[]
                    {                         
                        propDetails.LEVELNAME ==null?string.Empty:propDetails.LEVELNAME.ToString(),
                        propDetails.STARTDATE ==null?"-":Convert.ToDateTime(propDetails.STARTDATE).ToString("dd/MM/yyyy"),
                        propDetails.ENDDATE ==null?"-":Convert.ToDateTime(propDetails.ENDDATE).ToString("dd/MM/yyyy"),
                        "<a href='#' title='Click to Lock Proposal Details' class='ui-icon ui-icon-search ui-align-center' onClick='ViewDetails(\"" + URLEncrypt.EncryptParameters(new string[]{propDetails.LEVELCODE.ToString().Trim()+"$"+"B"}) +"\"); return false;'></a>"
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

        public Array GetVillageList(string moduleCode, int blockCode, byte scheme, int collaboration, int roleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            dbContext = new PMGSYEntities();
            try
            {
                var lstStates = dbContext.USP_UNLOCK_RECORDS(moduleCode, "V", 0, 0, blockCode, 0, 0, 0, 0, 0, "%", null, scheme, collaboration, roleCode).ToList();

                totalRecords = lstStates.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "MAST_VILLAGE_NAME":
                                lstStates = lstStates.OrderBy(x => x.LEVELNAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_UNLOCK_START_DATE":
                                lstStates = lstStates.OrderBy(x => x.STARTDATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_UNLOCK_END_DATE":
                                lstStates = lstStates.OrderBy(x => x.ENDDATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstStates = lstStates.OrderBy(x => x.LEVELNAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "MAST_VILLAGE_NAME":
                                lstStates = lstStates.OrderByDescending(x => x.LEVELNAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_UNLOCK_START_DATE":
                                lstStates = lstStates.OrderByDescending(x => x.STARTDATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_UNLOCK_END_DATE":
                                lstStates = lstStates.OrderByDescending(x => x.ENDDATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstStates = lstStates.OrderByDescending(x => x.LEVELNAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstStates = lstStates.OrderBy(x => x.LEVELNAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                return lstStates.Select(propDetails => new
                {
                    id = propDetails.LEVELCODE.ToString(),
                    cell = new[]
                    {                         
                        propDetails.LEVELNAME ==null?string.Empty:propDetails.LEVELNAME.ToString(),
                        propDetails.STARTDATE ==null?"-":Convert.ToDateTime(propDetails.STARTDATE).ToString("dd/MM/yyyy"),
                        propDetails.ENDDATE ==null?"-":Convert.ToDateTime(propDetails.ENDDATE).ToString("dd/MM/yyyy"),
                        "<a href='#' title='Click to Lock Proposal Details' class='ui-icon ui-icon-search ui-align-center' onClick='ViewDetails(\"" + URLEncrypt.EncryptParameters(new string[]{propDetails.LEVELCODE.ToString().Trim()+"$"+"V"}) +"\"); return false;'></a>"
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

        public Array GetHabitationList(string moduleCode, int villageCode, byte scheme, int collaboration, int roleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            dbContext = new PMGSYEntities();
            try
            {
                //var lstStates = dbContext.MASTER_HABITATIONS.Where(m => m.MAST_VILLAGE_CODE== villageCode).ToList();

                var lstStates = dbContext.USP_UNLOCK_RECORDS(moduleCode, "H", 0, 0, 0, villageCode, 0, 0, 0, 0, "%", null, scheme, collaboration, roleCode).ToList();

                totalRecords = lstStates.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "MAST_HAB_NAME":
                                lstStates = lstStates.OrderBy(x => x.LEVELNAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_UNLOCK_START_DATE":
                                lstStates = lstStates.OrderBy(x => x.STARTDATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_UNLOCK_END_DATE":
                                lstStates = lstStates.OrderBy(x => x.ENDDATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstStates = lstStates.OrderBy(x => x.LEVELNAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "MAST_BLOCK_NAME":
                                lstStates = lstStates.OrderByDescending(x => x.LEVELNAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_UNLOCK_START_DATE":
                                lstStates = lstStates.OrderByDescending(x => x.STARTDATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_UNLOCK_END_DATE":
                                lstStates = lstStates.OrderByDescending(x => x.ENDDATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstStates = lstStates.OrderByDescending(x => x.LEVELNAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstStates = lstStates.OrderBy(x => x.LEVELNAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                return lstStates.Select(propDetails => new
                {
                    id = propDetails.LEVELCODE.ToString(),
                    cell = new[]
                    {                         
                        propDetails.LEVELNAME ==null?string.Empty:propDetails.LEVELNAME.ToString(),
                        propDetails.STARTDATE ==null?"-":Convert.ToDateTime(propDetails.STARTDATE).ToString("dd/MM/yyyy"),
                        propDetails.ENDDATE ==null?"-":Convert.ToDateTime(propDetails.ENDDATE).ToString("dd/MM/yyyy"),
                        "<a href='#' title='Click to Lock Proposal Details' class='ui-icon ui-icon-search ui-align-center' onClick='ViewDetails(\"" + URLEncrypt.EncryptParameters(new string[]{propDetails.LEVELCODE.ToString().Trim()+"$"+"H"}) +"\"); return false;'></a>"
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

        public bool AddUnlockDetails(UnlockDetailsViewModel model, ref string message)
        {
            dbContext = new PMGSYEntities();
            CommonFunctions objCommon = new CommonFunctions();
            int districtCode = 0;   //new change

            using (TransactionScope ts = new TransactionScope())
            {
                try
                {
                    var adapter = (IObjectContextAdapter)dbContext;
                    var objectContext = adapter.ObjectContext;
                    objectContext.CommandTimeout = 300;

                    // Added to unlock Proposal Technology Details
                    string validateMessage1 = string.Empty;
                    string validateMessage2 = string.Empty;

                    if ((model.UnlockTable == "PR") || (model.UnlockTable == "PT") || (model.UnlockTable == "CP"))//Added CP for C-Proforma PDF unlocking by Shreyas)
                    {
                        if (!(ValidationUnlock1(model, ref validateMessage1) && ValidationUnlock2(model, ref validateMessage2)))
                        {

                            message = validateMessage1 + " " + validateMessage2;
                            return false;
                        }
                    }

                    foreach (var item in model.dataID)
                    {
                        //bool statusLevel = CheckAlreadyActiveLevel();

                        IMS_UNLOCK_DETAILS unlockMaster = new IMS_UNLOCK_DETAILS();
                        if (dbContext.IMS_UNLOCK_DETAILS.Any())
                        {
                            unlockMaster.IMS_TRANSACTION_NO = dbContext.IMS_UNLOCK_DETAILS.Max(m => m.IMS_TRANSACTION_NO) + 1;
                        }
                        else
                        {
                            unlockMaster.IMS_TRANSACTION_NO = 1;
                        }

                        unlockMaster.IMS_UNLOCK_BY = model.UnlockBy;
                        if (model.UnlockStartDate != null)
                        {
                            unlockMaster.IMS_UNLOCK_START_DATE = objCommon.GetStringToDateTime(model.UnlockStartDate);
                        }
                        if (model.UnlockEndDate != null)
                        {
                            unlockMaster.IMS_UNLOCK_END_DATE = objCommon.GetStringToDateTime(model.UnlockEndDate);
                        }

                        if (model.UnlockTable.Equals("CH"))
                        {
                            if (unlockMaster.IMS_UNLOCK_START_DATE == unlockMaster.IMS_UNLOCK_END_DATE)
                            {
                                message = "Unlock Start Date and Unlock End Date can not be same.";
                                return false;
                            }
                        }
                        


                        if (model.UnlockBy == "N")
                        {
                            unlockMaster.IMS_UNLOCK_BY = "N";
                            unlockMaster.IMS_UNLOCK_STATUS = "N";
                        }

                        switch (model.UnlockLevel)
                        {
                            case "S":
                                unlockMaster.MAST_STATE_CODE = item;
                                break;
                            case "D":
                                unlockMaster.MAST_DISTRICT_CODE = item;
                                //new change done 
                                unlockMaster.MAST_STATE_CODE = dbContext.MASTER_DISTRICT.Where(m => m.MAST_DISTRICT_CODE == item).Select(m => m.MAST_STATE_CODE).FirstOrDefault();
                                //end of change
                                if (PMGSYSession.Current.RoleCode == 36)
                                {
                                    var lstProposals = (from pr in dbContext.IMS_SANCTIONED_PROJECTS
                                                        where
                                                        pr.MAST_DISTRICT_CODE == item &&
                                                        ((pr.IMS_ISCOMPLETED == "S" && pr.STA_SANCTIONED == "U") ||
                                                         (pr.IMS_ISCOMPLETED == "D" && pr.STA_SANCTIONED == "N"))
                                                        select pr
                                                         ).ToList();
                                    if (lstProposals.Count() > 0)
                                    {
                                        lstProposals.ForEach(m =>
                                        {
                                            m.STA_SANCTIONED = "N";
                                            m.STA_SANCTIONED_BY = null;
                                            m.STA_SANCTIONED_DATE = null;
                                            m.IMS_ISCOMPLETED = "E";
                                            m.IMS_LOCK_STATUS = "N";
                                            m.IMS_STA_REMARKS = null;
                                            m.USERID = PMGSYSession.Current.UserId;
                                            m.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                                        });
                                    }
                                    dbContext.SaveChanges();
                                }
                                break;
                            case "B":
                                unlockMaster.MAST_BLOCK_CODE = item;
                                //new change
                                districtCode = dbContext.MASTER_BLOCK.Where(m => m.MAST_BLOCK_CODE == item).Select(m => m.MAST_DISTRICT_CODE).FirstOrDefault();
                                unlockMaster.MAST_STATE_CODE = dbContext.MASTER_DISTRICT.Where(m => m.MAST_DISTRICT_CODE == districtCode).Select(m => m.MAST_STATE_CODE).FirstOrDefault();
                                if (PMGSYSession.Current.RoleCode == 36)
                                {
                                    var lstProposals = (from pr in dbContext.IMS_SANCTIONED_PROJECTS
                                                        where
                                                        pr.MAST_BLOCK_CODE == item &&
                                                        ((pr.IMS_ISCOMPLETED == "S" && pr.STA_SANCTIONED == "U") ||
                                                         (pr.IMS_ISCOMPLETED == "D" && pr.STA_SANCTIONED == "N"))
                                                        select pr
                                                         ).ToList();
                                    if (lstProposals.Count() > 0)
                                    {
                                        lstProposals.ForEach(m =>
                                        {
                                            m.STA_SANCTIONED = "N";
                                            m.STA_SANCTIONED_BY = null;
                                            m.STA_SANCTIONED_DATE = null;
                                            m.IMS_ISCOMPLETED = "E";
                                            m.IMS_LOCK_STATUS = "N";
                                            m.IMS_STA_REMARKS = null;
                                            m.USERID = PMGSYSession.Current.UserId;
                                            m.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                                        });
                                    }
                                }


                                //end of change
                                break;
                            case "V":
                                unlockMaster.MAST_VILLAGE_CODE = item;
                                unlockMaster.MAST_STATE_CODE = dbContext.MASTER_VILLAGE.Where(m => m.MAST_VILLAGE_CODE == item).Select(m => m.MASTER_BLOCK.MASTER_DISTRICT.MASTER_STATE.MAST_STATE_CODE).FirstOrDefault();
                                break;
                            case "H":
                                unlockMaster.MAST_HAB_CODE = item;
                                unlockMaster.MAST_STATE_CODE = dbContext.MASTER_HABITATIONS.Where(m => m.MAST_HAB_CODE == item).Select(m => m.MASTER_VILLAGE.MASTER_BLOCK.MASTER_DISTRICT.MASTER_STATE.MAST_STATE_CODE).FirstOrDefault();
                                break;
                            case "Y":
                                unlockMaster.MAST_STATE_CODE = model.StateCode;
                                unlockMaster.MAST_DISTRICT_CODE = (model.DistrictCode <= 0 ? null : model.DistrictCode);
                                unlockMaster.MAST_BLOCK_CODE = (model.BlockCode <= 0 ? null : model.BlockCode);
                                unlockMaster.IMS_YEAR = item;
                                break;
                            case "T":
                                unlockMaster.MAST_STATE_CODE = model.StateCode;
                                unlockMaster.MAST_DISTRICT_CODE = (model.DistrictCode <= 0 ? null : model.DistrictCode);
                                unlockMaster.MAST_BLOCK_CODE = (model.BlockCode <= 0 ? null : model.BlockCode);
                                unlockMaster.IMS_YEAR = (model.YearCode <= 0 ? null : model.YearCode);
                                unlockMaster.IMS_BATCH = item;
                                break;
                            case "R":
                                switch (model.UnlockTable)
                                {
                                    case "PR":
                                        unlockMaster.IMS_PR_ROAD_CODE = item;
                                        if (model.UnlockBy == "N")
                                        {
                                            IMS_SANCTIONED_PROJECTS proposalMaster = dbContext.IMS_SANCTIONED_PROJECTS.Find(item);
                                            proposalMaster.STA_SANCTIONED = "N";
                                            proposalMaster.STA_SANCTIONED_BY = null;
                                            proposalMaster.STA_SANCTIONED_DATE = null;
                                            proposalMaster.IMS_ISCOMPLETED = "E";
                                            proposalMaster.IMS_LOCK_STATUS = "N";
                                            proposalMaster.IMS_STA_REMARKS = null;

                                            //added by abhishek kamble 28-nov-2013
                                            proposalMaster.USERID = PMGSYSession.Current.UserId;
                                            proposalMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                                            dbContext.Entry(proposalMaster).State = System.Data.Entity.EntityState.Modified;
                                            dbContext.SaveChanges();

                                        }
                                        //new change
                                        unlockMaster.MAST_STATE_CODE = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == item).Select(m => m.MAST_STATE_CODE).FirstOrDefault();
                                        //end of change

                                        break;
                                    case "ER":
                                        unlockMaster.MAST_ER_ROAD_CODE = item;
                                        //new change
                                        unlockMaster.MAST_STATE_CODE = dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == item).Select(m => m.MAST_STATE_CODE).FirstOrDefault();
                                        //end of change
                                        break;
                                    case "CN":
                                        unlockMaster.PLAN_CN_ROAD_CODE = item;
                                        //new change
                                        unlockMaster.MAST_STATE_CODE = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == item).Select(m => m.MAST_STATE_CODE).FirstOrDefault();
                                        //end of change
                                        break;
                                    case "PH":
                                        unlockMaster.IMS_PR_ROAD_CODE = item;
                                        //new change
                                        unlockMaster.MAST_STATE_CODE = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == item).Select(m => m.MAST_STATE_CODE).FirstOrDefault();
                                        //end of change
                                        break;
                                    // Added By to unlock Proposal Technology Details
                                    case "PT":
                                        unlockMaster.IMS_PR_ROAD_CODE = item;
                                        unlockMaster.MAST_STATE_CODE = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == item).Select(m => m.MAST_STATE_CODE).FirstOrDefault();
                                        unlockMaster.MAST_DISTRICT_CODE = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == item).Select(m => m.MAST_DISTRICT_CODE).FirstOrDefault();     //model.DistrictCode <= 0 ? null : model.DistrictCode;
                                        unlockMaster.MAST_BLOCK_CODE = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == item).Select(m => m.MAST_BLOCK_CODE).FirstOrDefault();        //model.BlockCode <= 0 ? null : model.BlockCode;
                                        unlockMaster.IMS_BATCH = model.BatchCode;
                                        unlockMaster.IMS_YEAR = model.YearCode;
                                        unlockMaster.IMS_PROPOSAL_TYPE = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == item).Select(m => m.IMS_PROPOSAL_TYPE).First();

                                        break;
                                    //Added to unlock C-Performa PDF
                                    case "CP":
                                        unlockMaster.IMS_PR_ROAD_CODE = item;
                                        unlockMaster.MAST_STATE_CODE = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == item).Select(m => m.MAST_STATE_CODE).FirstOrDefault();
                                        unlockMaster.MAST_DISTRICT_CODE = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == item).Select(m => m.MAST_DISTRICT_CODE).FirstOrDefault();     //model.DistrictCode <= 0 ? null : model.DistrictCode;
                                        unlockMaster.MAST_BLOCK_CODE = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == item).Select(m => m.MAST_BLOCK_CODE).FirstOrDefault();        //model.BlockCode <= 0 ? null : model.BlockCode;
                                        unlockMaster.IMS_BATCH = model.BatchCode;
                                        unlockMaster.IMS_YEAR = model.YearCode;
                                        unlockMaster.IMS_PROPOSAL_TYPE = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == item).Select(m => m.IMS_PROPOSAL_TYPE).First();

                                        break;

                                    default:
                                        break;
                                }
                                break;
                            default:
                                break;
                        }
                        //bool status = CheckActiveStatus(model.UnlockTable, model.UnlockLevel, model.UnlockStartDate, model.UnlockEndDate, item,ref message,model.UnlockBy);
                        //bool status = CheckActiveLevel(model.UnlockTable,model.UnlockLevel,item);
                        bool status = ValidateUnlockLevel(item, model.UnlockLevel, model.UnlockTable);

                        if (model.UnlockBy == "N")
                        {
                            status = true;
                        }

                        dbContext = new PMGSYEntities();

                        if (status == true)
                        {
                            if (model.UnlockBy != "N")
                            {
                                unlockMaster.IMS_UNLOCK_STATUS = "Y";
                            }
                            else
                            {
                                unlockMaster.IMS_UNLOCK_STATUS = "N";
                            }
                        }
                        else
                        {
                            switch (model.UnlockLevel)
                            {
                                case "D":
                                    message = "Another level of this module is already unlocked";
                                    return false;
                                case "B":
                                    message = "Another level of this module is already unlocked";
                                    return false;
                                case "V":
                                    message = "Another level of this module is already unlocked";
                                    return false;
                                case "R":
                                    message = "Another level of this module is already unlocked";
                                    return false;
                                case "H":
                                    message = "Another level of this module is already unlocked";
                                    return false;
                                case "S":
                                    message = "Another level of this module is already unlocked.";
                                    return false;
                                default:
                                    break;
                            }
                            unlockMaster.IMS_UNLOCK_STATUS = "N";
                        }

                        dbContext = new PMGSYEntities();
                        unlockMaster.IMS_UNLOCK_REMARKS = model.UnlockRemarks;
                        unlockMaster.IMS_UNLOCK_LEVEL = model.UnlockLevel;
                        unlockMaster.IMS_UNLOCK_TABLE = model.UnlockTable;
                        unlockMaster.MAST_ROLE_CODE = (short)model.UnlockRoleCode;
                        //added by abhishek kamble 28-nov-2013
                        unlockMaster.USERID = PMGSYSession.Current.UserId;
                        unlockMaster.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                        unlockMaster.MAST_PMGSY_SCHEME = model.PMGSYScheme;
                        dbContext.IMS_UNLOCK_DETAILS.Add(unlockMaster);
                        dbContext.SaveChanges();
                    }
                    ///Changes by SAMMED A. PATIL on 03JULY2017 for Send Mail on unocking records.
                    sendMail(model);
                    ts.Complete();

                    message = "Unlock details added successfully.";
                    return true;
                }
                catch (Exception ex)
                {
                    //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    ErrorLog.LogError(ex, "AddUnlockDetailsDAL");
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
        }


        // Added By to unlock Proposal Technology Details

        /// <summary>
        /// Validation-1   The same work that can not be unlocked at more than one role(SRRDA/PIU)
        /// </summary>
        /// <param name="UnlockDetailsViewModel"></param>
        /// <out param name="validateMessage1"></param>
        /// <returns> true or false </returns>
        public bool ValidationUnlock1(UnlockDetailsViewModel model, ref string validateMessage1)
        {

            foreach (var item in model.dataID)
            {
                if (model.UnlockLevel == "R")
                {
                    var isUnlockedYearBatchwise = dbContext.IMS_UNLOCK_DETAILS.Where(x => x.IMS_UNLOCK_TABLE.Equals(model.UnlockTable.Trim()) && (x.IMS_UNLOCK_LEVEL == "R")
                                                                                    && x.MAST_PMGSY_SCHEME == model.PMGSYScheme && x.MAST_STATE_CODE == model.StateCode
                                                                                    && ((x.MAST_DISTRICT_CODE == null) ? (true == true) : x.MAST_DISTRICT_CODE == model.DistrictCode)
                                                                                    && ((x.IMS_YEAR == null) ? (true == true) : x.IMS_YEAR == model.YearCode) && x.IMS_UNLOCK_STATUS == "Y" && x.IMS_PR_ROAD_CODE == item).Any()

                                                                                    ? dbContext.IMS_UNLOCK_DETAILS.Where(x => x.IMS_UNLOCK_TABLE.Equals(model.UnlockTable.Trim()) && (x.IMS_UNLOCK_LEVEL == "R")
                                                                                      && x.MAST_PMGSY_SCHEME == model.PMGSYScheme && x.MAST_STATE_CODE == model.StateCode
                                                                                      && ((x.MAST_DISTRICT_CODE == null) ? (true == true) : x.MAST_DISTRICT_CODE == model.DistrictCode)
                                                                                      && ((x.IMS_YEAR == null) ? (true == true) : x.IMS_YEAR == model.YearCode) && x.IMS_UNLOCK_STATUS == "Y" && x.IMS_PR_ROAD_CODE == item).First()
                                                                                    : null;
                    if (isUnlockedYearBatchwise != null)
                    {
                        var rolename = isUnlockedYearBatchwise.MAST_ROLE_CODE != 0 ? dbContext.UM_Role_Master.Where(a => a.RoleID == isUnlockedYearBatchwise.MAST_ROLE_CODE).Select(a => a.RoleName).FirstOrDefault() : "All Roles";
                        string unlockedRoadName = isUnlockedYearBatchwise.IMS_PR_ROAD_CODE != null ? dbContext.IMS_SANCTIONED_PROJECTS.Where(a => a.IMS_PR_ROAD_CODE == isUnlockedYearBatchwise.IMS_PR_ROAD_CODE).Select(a => a.IMS_ROAD_NAME).First() : "";
                        validateMessage1 = "Road/LSB " + unlockedRoadName + " is already unlocked for " + rolename;
                        return false;
                    }
                }
                else if (model.UnlockLevel == "Y")
                {
                    var isUnlockedBatchRoadwise = dbContext.IMS_UNLOCK_DETAILS.Where(x => x.IMS_UNLOCK_TABLE.Equals(model.UnlockTable.Trim()) && (x.IMS_UNLOCK_LEVEL == "Y")
                                                                                    && x.MAST_PMGSY_SCHEME == model.PMGSYScheme && x.MAST_STATE_CODE == model.StateCode
                                                                                    && ((x.MAST_DISTRICT_CODE == null) ? (true == true) : x.MAST_DISTRICT_CODE == model.DistrictCode)
                                                                                    && ((x.IMS_YEAR == null) ? (true == true) : x.IMS_YEAR == item)
                                                                                    && x.IMS_UNLOCK_STATUS == "Y").Any()

                                                                                    ? dbContext.IMS_UNLOCK_DETAILS.Where(x => x.IMS_UNLOCK_TABLE.Equals(model.UnlockTable.Trim()) && (x.IMS_UNLOCK_LEVEL == "Y")
                                                                                      && x.MAST_PMGSY_SCHEME == model.PMGSYScheme && x.MAST_STATE_CODE == model.StateCode
                                                                                      && ((x.MAST_DISTRICT_CODE == null) ? (true == true) : x.MAST_DISTRICT_CODE == model.DistrictCode)
                                                                                      && ((x.IMS_YEAR == null) ? (true == true) : x.IMS_YEAR == item)
                                                                                      && x.IMS_UNLOCK_STATUS == "Y").First()
                                                                                    : null;
                    if (isUnlockedBatchRoadwise != null)
                    {
                        int? yearAlreadyUnlocked = isUnlockedBatchRoadwise.IMS_YEAR;

                        var rolename = isUnlockedBatchRoadwise.MAST_ROLE_CODE != 0 ? dbContext.UM_Role_Master.Where(a => a.RoleID == isUnlockedBatchRoadwise.MAST_ROLE_CODE).Select(a => a.RoleName).FirstOrDefault() : "All Roles";
                        validateMessage1 = "Year " + yearAlreadyUnlocked + " is already unlocked for " + rolename;
                        return false;
                    }
                }
                else if (model.UnlockLevel == "T")
                {
                    var isUnlockedYearRoadwise = dbContext.IMS_UNLOCK_DETAILS.Where(x => x.IMS_UNLOCK_TABLE.Equals(model.UnlockTable.Trim()) && (x.IMS_UNLOCK_LEVEL == "T")
                                                                                    && x.MAST_PMGSY_SCHEME == model.PMGSYScheme && x.MAST_STATE_CODE == model.StateCode
                                                                                    && ((x.MAST_DISTRICT_CODE == null) ? (true == true) : x.MAST_DISTRICT_CODE == model.DistrictCode)
                                                                                    && ((x.IMS_YEAR == null) ? (true == true) : x.IMS_YEAR == model.YearCode)
                                                                                    && x.IMS_BATCH == item && x.IMS_UNLOCK_STATUS == "Y").Any()

                                                                                    ? dbContext.IMS_UNLOCK_DETAILS.Where(x => x.IMS_UNLOCK_TABLE.Equals(model.UnlockTable.Trim()) && (x.IMS_UNLOCK_LEVEL == "T")
                                                                                      && x.MAST_PMGSY_SCHEME == model.PMGSYScheme && x.MAST_STATE_CODE == model.StateCode
                                                                                      && ((x.MAST_DISTRICT_CODE == null) ? (true == true) : x.MAST_DISTRICT_CODE == model.DistrictCode)
                                                                                      && ((x.IMS_YEAR == null) ? (true == true) : x.IMS_YEAR == model.YearCode)
                                                                                      && x.IMS_BATCH == item && x.IMS_UNLOCK_STATUS == "Y").First()
                                                                                    : null;
                    if (isUnlockedYearRoadwise != null)
                    {
                        var rolename = isUnlockedYearRoadwise.MAST_ROLE_CODE != 0 ? dbContext.UM_Role_Master.Where(a => a.RoleID == isUnlockedYearRoadwise.MAST_ROLE_CODE).Select(a => a.RoleName).FirstOrDefault() : "All Roles";
                        validateMessage1 = "Batch " + isUnlockedYearRoadwise.IMS_BATCH + " is already unlocked for " + rolename;
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Validation-2  
        /// a) Unlocking is done at  year level then it can not be unlocked at Road and batch level. 
        /// b) Unlocking is done at  batch level then it can not be unlocked at Road and Year level. 
        /// c) Unlocking is done at  road level then it can not be unlocked at Year and batch level.
        /// </summary>
        /// <param name="UnlockDetailsViewModel"></param>
        /// <out param name="validateMessage2"></param>
        /// <returns> true or false </returns>
        public bool ValidationUnlock2(UnlockDetailsViewModel model, ref string validateMessage2)
        {
            DateTime todayDate = DateTime.Now.Date;

            if (model.UnlockTable.Trim().Equals("PT"))
            {
                foreach (var item in model.dataID)
                {
                    if (model.UnlockLevel == "R")
                    {
                        var isUnlockedYearBatchwise = dbContext.IMS_UNLOCK_DETAILS.Where(x => x.IMS_UNLOCK_STATUS == "Y" && (x.IMS_UNLOCK_BY == "M")
                                                                                    && (x.IMS_UNLOCK_TABLE == "PT" || x.IMS_UNLOCK_TABLE == "PR")
                                                                                    && (todayDate >= x.IMS_UNLOCK_START_DATE && todayDate <= x.IMS_UNLOCK_END_DATE)
                                                                                    && (x.MAST_STATE_CODE == model.StateCode)
                                                                                    && (x.MAST_PMGSY_SCHEME == model.PMGSYScheme) && ((x.IMS_UNLOCK_LEVEL == "T") || (x.IMS_UNLOCK_LEVEL == "Y"))).Any()

                                                    ? dbContext.IMS_UNLOCK_DETAILS.Where(x => x.IMS_UNLOCK_STATUS == "Y" && (x.IMS_UNLOCK_BY == "M")
                                                                                    && (x.IMS_UNLOCK_TABLE == "PT" || x.IMS_UNLOCK_TABLE == "PR")
                                                                                    && (todayDate >= x.IMS_UNLOCK_START_DATE && todayDate <= x.IMS_UNLOCK_END_DATE)
                                                                                    && (x.MAST_STATE_CODE == model.StateCode)
                                                                                    && (x.MAST_PMGSY_SCHEME == model.PMGSYScheme) && ((x.IMS_UNLOCK_LEVEL == "T") || (x.IMS_UNLOCK_LEVEL == "Y"))).First()
                                                    : null;

                        if (isUnlockedYearBatchwise != null)
                        {
                            var rolename = isUnlockedYearBatchwise.MAST_ROLE_CODE != 0 ? dbContext.UM_Role_Master.Where(a => a.RoleID == isUnlockedYearBatchwise.MAST_ROLE_CODE).Select(a => a.RoleName).FirstOrDefault() : "All Roles";
                            string unlockedLevel = isUnlockedYearBatchwise.IMS_UNLOCK_LEVEL == "T" ? "Batch" : (isUnlockedYearBatchwise.IMS_UNLOCK_LEVEL == "Y") ? "Year" : "";
                            validateMessage2 = "Record can not be unlocked at Road level try at year/ batch level";                         //"Road/LSB is already unlocked for " + rolename +" on "+ unlockedLevel +" level";
                            return false;
                        }
                    }
                    else if (model.UnlockLevel == "Y")
                    {
                        var isUnlockedBatchRoadwise = dbContext.IMS_UNLOCK_DETAILS.Where(x => x.IMS_UNLOCK_STATUS.Equals("Y")
                                                                                     && x.IMS_UNLOCK_BY.Equals("M")
                                                                                     && ((x.IMS_UNLOCK_TABLE.Equals("PT")) || (x.IMS_UNLOCK_TABLE.Equals("PR")))
                                                                                     && (todayDate >= x.IMS_UNLOCK_START_DATE && todayDate <= x.IMS_UNLOCK_END_DATE)
                                                                                     && (x.MAST_STATE_CODE == model.StateCode)
                                                                                     && (x.MAST_PMGSY_SCHEME == model.PMGSYScheme)
                                                                                     && ((x.IMS_UNLOCK_LEVEL.Equals("R")) || (x.IMS_UNLOCK_LEVEL.Equals("T")))).Any()

                                                    ? dbContext.IMS_UNLOCK_DETAILS.Where(x => x.IMS_UNLOCK_STATUS.Equals("Y")
                                                                                     && x.IMS_UNLOCK_BY.Equals("M")
                                                                                     && ((x.IMS_UNLOCK_TABLE.Equals("PT")) || (x.IMS_UNLOCK_TABLE.Equals("PR")))
                                                                                     && (todayDate >= x.IMS_UNLOCK_START_DATE && todayDate <= x.IMS_UNLOCK_END_DATE)
                                                                                     && (x.MAST_STATE_CODE == model.StateCode)
                                                                                     && (x.MAST_PMGSY_SCHEME == model.PMGSYScheme)
                                                                                     && ((x.IMS_UNLOCK_LEVEL.Equals("R")) || (x.IMS_UNLOCK_LEVEL.Equals("T")))).First()
                                                    : null;

                        if (isUnlockedBatchRoadwise != null)
                        {
                            int? batchAlreadyUnlocked = isUnlockedBatchRoadwise.IMS_YEAR;

                            var rolename = isUnlockedBatchRoadwise.MAST_ROLE_CODE != 0 ? dbContext.UM_Role_Master.Where(a => a.RoleID == isUnlockedBatchRoadwise.MAST_ROLE_CODE).Select(a => a.RoleName).FirstOrDefault() : "All Roles";
                            string unlockedLevel = isUnlockedBatchRoadwise.IMS_UNLOCK_LEVEL == "T" ? "Batch" : (isUnlockedBatchRoadwise.IMS_UNLOCK_LEVEL == "R") ? "Road/LSB" : "";
                            validateMessage2 = "Record can not be unlocked at year level try at batch level or road level.";                          // Year is already unlocked for " + rolename + " on " + unlockedLevel + " level";
                            return false;
                        }
                    }
                    else if (model.UnlockLevel == "T")
                    {
                        var isUnlockedYearRoadwise = dbContext.IMS_UNLOCK_DETAILS.Where(x => x.IMS_UNLOCK_STATUS == "Y" && (x.IMS_UNLOCK_BY == "M")
                                                                                      && (x.IMS_UNLOCK_TABLE == "PT" || x.IMS_UNLOCK_TABLE == "PR")
                                                                                      && (todayDate >= x.IMS_UNLOCK_START_DATE && todayDate <= x.IMS_UNLOCK_END_DATE)
                                                                                      && (x.MAST_STATE_CODE == model.StateCode)
                                                                                      && (x.MAST_PMGSY_SCHEME == model.PMGSYScheme) && ((x.IMS_UNLOCK_LEVEL == "R") || (x.IMS_UNLOCK_LEVEL == "Y"))).Any()

                                                    ? dbContext.IMS_UNLOCK_DETAILS.Where(x => x.IMS_UNLOCK_STATUS == "Y" && x.IMS_UNLOCK_BY == "M"
                                                                                    && (x.IMS_UNLOCK_TABLE == "PT" || x.IMS_UNLOCK_TABLE == "PR")
                                                                                    && (todayDate >= x.IMS_UNLOCK_START_DATE && todayDate <= x.IMS_UNLOCK_END_DATE)
                                                                                    && x.MAST_STATE_CODE == model.StateCode
                                                                                    && x.MAST_PMGSY_SCHEME == model.PMGSYScheme && ((x.IMS_UNLOCK_LEVEL == "R") || (x.IMS_UNLOCK_LEVEL == "Y"))).First()
                                                    : null;

                        if (isUnlockedYearRoadwise != null)
                        {
                            var rolename = isUnlockedYearRoadwise.MAST_ROLE_CODE != 0 ? dbContext.UM_Role_Master.Where(a => a.RoleID == isUnlockedYearRoadwise.MAST_ROLE_CODE).Select(a => a.RoleName).FirstOrDefault() : "All Roles";
                            string unlockedLevel = isUnlockedYearRoadwise.IMS_UNLOCK_LEVEL == "R" ? "Road/LSB" : (isUnlockedYearRoadwise.IMS_UNLOCK_LEVEL == "Y") ? "Year" : "";
                            validateMessage2 = "Record can not be unlocked at batch level. Try at year level or road level.";                   //"Batch is already unlocked for " + rolename + " on " + unlockedLevel + " level";
                            return false;
                        }
                    }
                }
            }
            else if (model.UnlockTable.Trim().Equals("CP"))  //Added for C-Performa Unlock
            {
                foreach (var item in model.dataID)
                {
                    if (model.UnlockLevel == "R")
                    {
                        var isUnlockedYearBatchwise = dbContext.IMS_UNLOCK_DETAILS.Where(x => x.IMS_UNLOCK_STATUS == "Y" && (x.IMS_UNLOCK_BY == "M")
                                                                                    && (x.IMS_UNLOCK_TABLE == "PR" || x.IMS_UNLOCK_TABLE == "CP") //Added CP for C-Proforma PDF unlock by Shreyas
                                                                                    && (todayDate >= x.IMS_UNLOCK_START_DATE && todayDate <= x.IMS_UNLOCK_END_DATE)
                                                                                    && (x.MAST_STATE_CODE == model.StateCode)
                                                                                    && (x.MAST_PMGSY_SCHEME == model.PMGSYScheme) && ((x.IMS_UNLOCK_LEVEL == "T") || (x.IMS_UNLOCK_LEVEL == "Y"))).Any()

                                                    ? dbContext.IMS_UNLOCK_DETAILS.Where(x => x.IMS_UNLOCK_STATUS == "Y" && (x.IMS_UNLOCK_BY == "M")
                                                                                    && (x.IMS_UNLOCK_TABLE == "PR" || x.IMS_UNLOCK_TABLE == "CP") //Added CP for C-Proforma PDF unlock by Shreyas
                                                                                    && (todayDate >= x.IMS_UNLOCK_START_DATE && todayDate <= x.IMS_UNLOCK_END_DATE)
                                                                                    && (x.MAST_STATE_CODE == model.StateCode)
                                                                                    && (x.MAST_PMGSY_SCHEME == model.PMGSYScheme) && ((x.IMS_UNLOCK_LEVEL == "T") || (x.IMS_UNLOCK_LEVEL == "Y"))).First()
                                                    : null;

                        if (isUnlockedYearBatchwise != null)
                        {
                            var rolename = isUnlockedYearBatchwise.MAST_ROLE_CODE != 0 ? dbContext.UM_Role_Master.Where(a => a.RoleID == isUnlockedYearBatchwise.MAST_ROLE_CODE).Select(a => a.RoleName).FirstOrDefault() : "All Roles";
                            string unlockedLevel = isUnlockedYearBatchwise.IMS_UNLOCK_LEVEL == "T" ? "Batch" : (isUnlockedYearBatchwise.IMS_UNLOCK_LEVEL == "Y") ? "Year" : "";
                            validateMessage2 = "Record can not be unlocked at Road level try at year/ batch level";                         //"Road/LSB is already unlocked for " + rolename +" on "+ unlockedLevel +" level";
                            return false;
                        }
                    }
                    else if (model.UnlockLevel == "Y")
                    {
                        var isUnlockedBatchRoadwise = dbContext.IMS_UNLOCK_DETAILS.Where(x => x.IMS_UNLOCK_STATUS.Equals("Y")
                                                                                     && x.IMS_UNLOCK_BY.Equals("M")
                                                                                     && ((x.IMS_UNLOCK_TABLE.Equals("PR")) || (x.IMS_UNLOCK_TABLE.Equals("CP"))) //Added CP for C-Proforma PDF unlock by Shreyas
                                                                                     && (todayDate >= x.IMS_UNLOCK_START_DATE && todayDate <= x.IMS_UNLOCK_END_DATE)
                                                                                     && (x.MAST_STATE_CODE == model.StateCode)
                                                                                     && (x.MAST_PMGSY_SCHEME == model.PMGSYScheme)
                                                                                     && ((x.IMS_UNLOCK_LEVEL.Equals("R")) || (x.IMS_UNLOCK_LEVEL.Equals("T")))).Any()

                                                    ? dbContext.IMS_UNLOCK_DETAILS.Where(x => x.IMS_UNLOCK_STATUS.Equals("Y")
                                                                                     && x.IMS_UNLOCK_BY.Equals("M")
                                                                                     && ((x.IMS_UNLOCK_TABLE.Equals("PR")) || (x.IMS_UNLOCK_TABLE.Equals("CP"))) //Added CP for C-Proforma PDF unlock by Shreyas
                                                                                     && (todayDate >= x.IMS_UNLOCK_START_DATE && todayDate <= x.IMS_UNLOCK_END_DATE)
                                                                                     && (x.MAST_STATE_CODE == model.StateCode)
                                                                                     && (x.MAST_PMGSY_SCHEME == model.PMGSYScheme)
                                                                                     && ((x.IMS_UNLOCK_LEVEL.Equals("R")) || (x.IMS_UNLOCK_LEVEL.Equals("T")))).First()
                                                    : null;

                        if (isUnlockedBatchRoadwise != null)
                        {
                            int? batchAlreadyUnlocked = isUnlockedBatchRoadwise.IMS_YEAR;

                            var rolename = isUnlockedBatchRoadwise.MAST_ROLE_CODE != 0 ? dbContext.UM_Role_Master.Where(a => a.RoleID == isUnlockedBatchRoadwise.MAST_ROLE_CODE).Select(a => a.RoleName).FirstOrDefault() : "All Roles";
                            string unlockedLevel = isUnlockedBatchRoadwise.IMS_UNLOCK_LEVEL == "T" ? "Batch" : (isUnlockedBatchRoadwise.IMS_UNLOCK_LEVEL == "R") ? "Road/LSB" : "";
                            validateMessage2 = "Record can not be unlocked at year level try at batch level or road level.";                          // Year is already unlocked for " + rolename + " on " + unlockedLevel + " level";
                            return false;
                        }
                    }
                    else if (model.UnlockLevel == "T")
                    {
                        var isUnlockedYearRoadwise = dbContext.IMS_UNLOCK_DETAILS.Where(x => x.IMS_UNLOCK_STATUS == "Y" && (x.IMS_UNLOCK_BY == "M")
                                                                                      && (x.IMS_UNLOCK_TABLE == "PR" || x.IMS_UNLOCK_TABLE == "CP") //Added CP for C-Proforma PDF unlock by Shreyas
                                                                                      && (todayDate >= x.IMS_UNLOCK_START_DATE && todayDate <= x.IMS_UNLOCK_END_DATE)
                                                                                      && (x.MAST_STATE_CODE == model.StateCode)
                                                                                      && (x.MAST_PMGSY_SCHEME == model.PMGSYScheme) && ((x.IMS_UNLOCK_LEVEL == "R") || (x.IMS_UNLOCK_LEVEL == "Y"))).Any()

                                                    ? dbContext.IMS_UNLOCK_DETAILS.Where(x => x.IMS_UNLOCK_STATUS == "Y" && x.IMS_UNLOCK_BY == "M"
                                                                                    && (x.IMS_UNLOCK_TABLE == "PR" || x.IMS_UNLOCK_TABLE == "CP")  //Added CP for C-Proforma PDF unlock by Shreyas
                                                                                    && (todayDate >= x.IMS_UNLOCK_START_DATE && todayDate <= x.IMS_UNLOCK_END_DATE)
                                                                                    && x.MAST_STATE_CODE == model.StateCode
                                                                                    && x.MAST_PMGSY_SCHEME == model.PMGSYScheme && ((x.IMS_UNLOCK_LEVEL == "R") || (x.IMS_UNLOCK_LEVEL == "Y"))).First()
                                                    : null;

                        if (isUnlockedYearRoadwise != null)
                        {
                            var rolename = isUnlockedYearRoadwise.MAST_ROLE_CODE != 0 ? dbContext.UM_Role_Master.Where(a => a.RoleID == isUnlockedYearRoadwise.MAST_ROLE_CODE).Select(a => a.RoleName).FirstOrDefault() : "All Roles";
                            string unlockedLevel = isUnlockedYearRoadwise.IMS_UNLOCK_LEVEL == "R" ? "Road/LSB" : (isUnlockedYearRoadwise.IMS_UNLOCK_LEVEL == "Y") ? "Year" : "";
                            validateMessage2 = "Record can not be unlocked at batch level. Try at year level or road level.";                   //"Batch is already unlocked for " + rolename + " on " + unlockedLevel + " level";
                            return false;
                        }
                    }
                }
            }

            return true;


        }



        /// <summary>
        /// DAL method to create the Template of email along with the attachment
        /// </summary>
        /// <returns></returns>
        public bool sendMail(UnlockDetailsViewModel model)
        {
            //dbContext = new PMGSYEntities();
            int stateCode = 0, distCode = 0, blockCode = 0, villageCode = 0, habCode = 0, erCode = 0, cnCode = 0, prCode = 0, batch = 0, yearCode = 0;
            string stateName = string.Empty, distName = string.Empty, blockName = string.Empty, villageName = string.Empty, habName = string.Empty, erName = string.Empty, cnName = string.Empty, package = string.Empty, workName = string.Empty, unlockLevel = string.Empty, unlockModule = string.Empty, roleName = string.Empty, yearName = string.Empty, sanctionedBy = string.Empty, batchName = string.Empty, remarks = string.Empty;
            try
            {
                StringBuilder msgBody = new StringBuilder();

                msgBody.Append("Sir, <br/><br/>");
                msgBody.Append("Please find the details of the module/location/work which are unlocked. <br/><br/>");

                /*stateName = (model.StateCode != null && model.StateCode.HasValue) ? dbContext.MASTER_STATE.Where(x => x.MAST_STATE_CODE == model.StateCode.Value).Select(x => x.MAST_STATE_NAME).FirstOrDefault() : "-";
                distName = (model.DistrictCode != null && model.DistrictCode.HasValue) ? dbContext.MASTER_DISTRICT.Where(x => x.MAST_DISTRICT_CODE == model.DistrictCode.Value).Select(x => x.MAST_DISTRICT_NAME).FirstOrDefault() : "-";
                blockName = (model.BlockCode != null && model.BlockCode.HasValue) ? dbContext.MASTER_BLOCK.Where(x => x.MAST_BLOCK_CODE == model.BlockCode.Value).Select(x => x.MAST_BLOCK_NAME).FirstOrDefault() : "-";
                villageName = (model.BlockCode != null && model.BlockCode.HasValue) ? dbContext.MASTER_BLOCK.Where(x => x.MAST_BLOCK_CODE == model.BlockCode.Value).Select(x => x.MAST_BLOCK_NAME).FirstOrDefault() : "-";
                habName = (model.HabCode != null && model.HabCode.HasValue) ? dbContext.MASTER_HABITATIONS.Where(x => x.MAST_HAB_CODE == model.HabCode.Value).Select(x => x.MAST_HAB_NAME).FirstOrDefault() : "-";
                package = (model.ProposalCode != null && model.ProposalCode.HasValue) ? dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == model.ProposalCode.Value).Select(x => x.IMS_PACKAGE_ID).FirstOrDefault() : "-";
                workName = (model.ProposalCode != null && model.ProposalCode.HasValue) ? dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == model.ProposalCode.Value).Select(x => x.IMS_ROAD_NAME).FirstOrDefault() : "-";
                erName = (model.ExistingRoadCode != null && model.ExistingRoadCode.HasValue) ? dbContext.MASTER_EXISTING_ROADS.Where(x => x.MAST_ER_ROAD_CODE == model.ExistingRoadCode.Value).Select(x => x.MAST_ER_ROAD_NAME).FirstOrDefault() : "-";
                cnName = (model.CoreNetworkCode != null && model.CoreNetworkCode.HasValue) ? dbContext.PLAN_ROAD.Where(x => x.PLAN_CN_ROAD_CODE == model.CoreNetworkCode.Value).Select(x => x.PLAN_RD_NAME).FirstOrDefault() : "-";*/
                roleName = (model.UnlockRoleCode > 0) ? dbContext.UM_Role_Master.Where(x => x.RoleID == model.UnlockRoleCode).Select(x => x.RoleName).FirstOrDefault() : "All Roles";

                int i = 0;
                string htmlTableStart = "<table style=\"border-collapse:collapse; text-align:center;\" >";
                string htmlTableEnd = "</table>";
                string htmlHeaderRowStart = "<tr style =\"background-color:#BDD3EB; color:#555555;\">";
                string htmlHeaderRowEnd = "</tr>";
                string htmlTrStart = "<tr style =\" color:#555555;\">";
                string htmlTrEnd = "</tr>";
                string htmlTdStart = "<td style=\" border-color:#5c87b2; border-style:solid; border-width:thin; padding: 5px;\">";
                string htmlTdEnd = "</td>";

                switch (model.UnlockTable)
                {
                    case "DM"://District Master
                        msgBody.Append("<b>Module</b> : District Master <br/>");
                        if (model.PMGSYScheme == 3)
                        {
                            msgBody.Append("<b>Scheme</b> : RCPLWE <br/>");
                        }
                        else
                        {
                            msgBody.Append("<b>Scheme</b> : PMGSY-" + (model.PMGSYScheme == 4 ? 3 : model.PMGSYScheme) + " <br/>");
                        }
                        switch (model.UnlockLevel)
                        {
                            case "S":  /* State */
                                unlockLevel = "State";
                                msgBody.Append("<b>Level</b> : State <br/><br/>");
                                msgBody.Append("<b>Remarks</b> : " + model.UnlockRemarks + " <br/><br/>");

                                //msgBody.Append("Sr.No \t Level \t Scheme \t State \t Start Date \t End Date \t Unlocked By \t Role");
                                //msgBody.Append("<table border=1><tr><td>Sr.No</td><td>Level</td><td>Scheme</td><td>State</td><td>Start Date</td><td>End Date</td><td>Unlocked By</td><td>Role</td></tr> <br/>");
                                //msgBody.Append(String.Format("{0,-10} {1,-25} {2, -22} {3, -15} {4, -15} {5, -10}", "Sr.No", "State", "Start Date", "End Date", "Unlocked By", "Role <br/>").Replace(" ", "&nbsp;"));

                                msgBody.Append(htmlTableStart + htmlHeaderRowStart);
                                msgBody = msgBody.Append(htmlTdStart + "Sr.No" + htmlTdEnd).Append(htmlTdStart + "State" + htmlTdEnd).Append(htmlTdStart + "Start Date" + htmlTdEnd).Append(htmlTdStart + "End Date" + htmlTdEnd).Append(htmlTdStart + "Unlocked By" + htmlTdEnd).Append(htmlTdStart + "Role" + htmlTdEnd);
                                msgBody = msgBody.Append(htmlHeaderRowEnd);

                                i = 0;
                                foreach (var item in model.dataID)
                                {
                                    stateCode = model.dataID[i];
                                    stateName = dbContext.MASTER_STATE.Where(x => x.MAST_STATE_CODE == stateCode).Select(x => x.MAST_STATE_NAME).FirstOrDefault();
                                    i++;
                                    //msgBody.Append("<table border=1><tr><td>" + i + " </td><td> " + unlockLevel + " </td><td> Scheme" + model.PMGSYScheme + " </td><td> " + stateName + " </td><td> " + model.UnlockStartDate + " </td><td> " + model.UnlockEndDate + " </td><td> " + PMGSYSession.Current.UserName + " </td><td> " + roleName + " </td> <br/>");
                                    //msgBody.Append(string.Format(i + "  {0,-10}  " + unlockLevel + "  {1,-10}  Scheme" + model.PMGSYScheme + "  {2,-10}  " + stateName + "  {3,-10}  " + model.UnlockStartDate + "  {4,-10}  " + model.UnlockEndDate + "  {5,-10}  " + PMGSYSession.Current.UserName + "  {6,-10}  " + roleName + " {7,-10}  <br/><br/>"));
                                    //msgBody.Append(HttpContext.Current.Server.HtmlDecode(String.Format("{0,-14} {1,-33} {2, -18} {3, -15} {4, -20} {5, -10}", i, stateName, model.UnlockStartDate, model.UnlockEndDate, PMGSYSession.Current.UserName, roleName + " <br/>").Replace(" ", "&nbsp;")));
                                    // msgBody.Append(HttpContext.Current.Server.HtmlDecode(String.Format("{0,-10} {1,-25} {2, -22} {3, -15} {4, -15} {5, -10}", i, stateName, model.UnlockStartDate, model.UnlockEndDate, PMGSYSession.Current.UserName, roleName + " <br/>").Replace(" ", "&nbsp;")));

                                    msgBody = msgBody.Append(htmlTrStart);
                                    msgBody = msgBody.Append(htmlTdStart + i + htmlTdEnd).Append(htmlTdStart + stateName + htmlTdEnd).Append(htmlTdStart + model.UnlockStartDate + htmlTdEnd).Append(htmlTdStart + model.UnlockEndDate + htmlTdEnd).Append(htmlTdStart + PMGSYSession.Current.UserName + htmlTdEnd).Append(htmlTdStart + roleName + htmlTdEnd);
                                    msgBody = msgBody.Append(htmlTrEnd);
                                }
                                msgBody.Append(htmlTableEnd);
                                break;
                            case "D": /* District */
                                unlockLevel = "District";
                                msgBody.Append("<b>Level</b> : District <br/>");

                                i = 0;
                                foreach (var item in model.dataID)
                                {
                                    distCode = model.dataID[i];
                                    if (i == 0)
                                    {
                                        stateName = dbContext.MASTER_DISTRICT.Where(x => x.MAST_DISTRICT_CODE == distCode).Select(x => x.MASTER_STATE.MAST_STATE_NAME).FirstOrDefault();
                                        msgBody.Append("<b>State</b> : " + stateName + " <br/><br/>");
                                        msgBody.Append("<b>Remarks</b> : " + model.UnlockRemarks + " <br/><br/>");

                                        msgBody.Append(htmlTableStart + htmlHeaderRowStart);
                                        msgBody = msgBody.Append(htmlTdStart + "Sr.No" + htmlTdEnd).Append(htmlTdStart + "District" + htmlTdEnd).Append(htmlTdStart + "Start Date" + htmlTdEnd).Append(htmlTdStart + "End Date" + htmlTdEnd).Append(htmlTdStart + "Unlocked By" + htmlTdEnd).Append(htmlTdStart + "Role" + htmlTdEnd);
                                        msgBody = msgBody.Append(htmlHeaderRowEnd);
                                    }
                                    distName = dbContext.MASTER_DISTRICT.Where(x => x.MAST_DISTRICT_CODE == distCode).Select(x => x.MAST_DISTRICT_NAME).FirstOrDefault();
                                    i++;
                                    msgBody = msgBody.Append(htmlTrStart);
                                    msgBody = msgBody.Append(htmlTdStart + i + htmlTdEnd).Append(htmlTdStart + distName + htmlTdEnd).Append(htmlTdStart + model.UnlockStartDate + htmlTdEnd).Append(htmlTdStart + model.UnlockEndDate + htmlTdEnd).Append(htmlTdStart + PMGSYSession.Current.UserName + htmlTdEnd).Append(htmlTdStart + roleName + htmlTdEnd);
                                    msgBody = msgBody.Append(htmlTrEnd);
                                }
                                msgBody.Append(htmlTableEnd);
                                break;
                        }
                        break;
                    case "BM"://Block Master
                        msgBody.Append("<b>Module</b> : Block Master <br/>");
                        if (model.PMGSYScheme == 3)
                        {
                            msgBody.Append("<b>Scheme</b> : RCPLWE <br/>");
                        }
                        else
                        {
                            msgBody.Append("<b>Scheme</b> : PMGSY-" + (model.PMGSYScheme == 4 ? 3 : model.PMGSYScheme) + " <br/>");
                        }
                        switch (model.UnlockLevel)
                        {
                            case "S":
                                unlockLevel = "State";
                                msgBody.Append("<b>Level</b> : State <br/><br/>");
                                msgBody.Append("<b>Remarks</b> : " + model.UnlockRemarks + " <br/><br/>");
                                msgBody.Append(htmlTableStart + htmlHeaderRowStart);
                                msgBody = msgBody.Append(htmlTdStart + "Sr.No" + htmlTdEnd).Append(htmlTdStart + "State" + htmlTdEnd).Append(htmlTdStart + "Start Date" + htmlTdEnd).Append(htmlTdStart + "End Date" + htmlTdEnd).Append(htmlTdStart + "Unlocked By" + htmlTdEnd).Append(htmlTdStart + "Role" + htmlTdEnd);
                                msgBody = msgBody.Append(htmlHeaderRowEnd);

                                i = 0;
                                foreach (var item in model.dataID)
                                {
                                    stateCode = model.dataID[i];
                                    stateName = dbContext.MASTER_STATE.Where(x => x.MAST_STATE_CODE == stateCode).Select(x => x.MAST_STATE_NAME).FirstOrDefault();
                                    i++;
                                    msgBody = msgBody.Append(htmlTrStart);
                                    msgBody = msgBody.Append(htmlTdStart + i + htmlTdEnd).Append(htmlTdStart + stateName + htmlTdEnd).Append(htmlTdStart + model.UnlockStartDate + htmlTdEnd).Append(htmlTdStart + model.UnlockEndDate + htmlTdEnd).Append(htmlTdStart + PMGSYSession.Current.UserName + htmlTdEnd).Append(htmlTdStart + roleName + htmlTdEnd);
                                    msgBody = msgBody.Append(htmlTrEnd);
                                }
                                msgBody.Append(htmlTableEnd);
                                break;
                            case "D": /* District */
                                unlockLevel = "District";
                                msgBody.Append("<b>Level</b> : District <br/>");

                                i = 0;
                                foreach (var item in model.dataID)
                                {
                                    distCode = model.dataID[i];
                                    if (i == 0)
                                    {
                                        stateName = dbContext.MASTER_DISTRICT.Where(x => x.MAST_DISTRICT_CODE == distCode).Select(x => x.MASTER_STATE.MAST_STATE_NAME).FirstOrDefault();
                                        msgBody.Append("<b>State</b> : " + stateName + " <br/><br/>");
                                        msgBody.Append("<b>Remarks</b> : " + model.UnlockRemarks + " <br/><br/>");

                                        msgBody.Append(htmlTableStart + htmlHeaderRowStart);
                                        msgBody = msgBody.Append(htmlTdStart + "Sr.No" + htmlTdEnd).Append(htmlTdStart + "District" + htmlTdEnd).Append(htmlTdStart + "Start Date" + htmlTdEnd).Append(htmlTdStart + "End Date" + htmlTdEnd).Append(htmlTdStart + "Unlocked By" + htmlTdEnd).Append(htmlTdStart + "Role" + htmlTdEnd);
                                        msgBody = msgBody.Append(htmlHeaderRowEnd);
                                    }
                                    distName = dbContext.MASTER_DISTRICT.Where(x => x.MAST_DISTRICT_CODE == distCode).Select(x => x.MAST_DISTRICT_NAME).FirstOrDefault();
                                    i++;
                                    msgBody = msgBody.Append(htmlTrStart);
                                    msgBody = msgBody.Append(htmlTdStart + i + htmlTdEnd).Append(htmlTdStart + distName + htmlTdEnd).Append(htmlTdStart + model.UnlockStartDate + htmlTdEnd).Append(htmlTdStart + model.UnlockEndDate + htmlTdEnd).Append(htmlTdStart + PMGSYSession.Current.UserName + htmlTdEnd).Append(htmlTdStart + roleName + htmlTdEnd);
                                    msgBody = msgBody.Append(htmlTrEnd);
                                }
                                msgBody.Append(htmlTableEnd);
                                break;
                            case "B": /* Block */
                                unlockLevel = "Block";
                                msgBody.Append("<b>Level</b> : Block <br/>");

                                i = 0;
                                foreach (var item in model.dataID)
                                {
                                    blockCode = model.dataID[i];
                                    if (i == 0)
                                    {
                                        stateName = dbContext.MASTER_BLOCK.Where(x => x.MAST_BLOCK_CODE == blockCode).Select(x => x.MASTER_DISTRICT.MASTER_STATE.MAST_STATE_NAME).FirstOrDefault();
                                        msgBody.Append("<b>State</b> : " + stateName + " <br/>");

                                        distName = dbContext.MASTER_BLOCK.Where(x => x.MAST_BLOCK_CODE == blockCode).Select(x => x.MASTER_DISTRICT.MAST_DISTRICT_NAME).FirstOrDefault();
                                        msgBody.Append("<b>District</b> : " + distName + " <br/><br/>");
                                        msgBody.Append("<b>Remarks</b> : " + model.UnlockRemarks + " <br/><br/>");

                                        msgBody.Append(htmlTableStart + htmlHeaderRowStart);
                                        msgBody = msgBody.Append(htmlTdStart + "Sr.No" + htmlTdEnd).Append(htmlTdStart + "District" + htmlTdEnd).Append(htmlTdStart + "Start Date" + htmlTdEnd).Append(htmlTdStart + "End Date" + htmlTdEnd).Append(htmlTdStart + "Unlocked By" + htmlTdEnd).Append(htmlTdStart + "Role" + htmlTdEnd);
                                        msgBody = msgBody.Append(htmlHeaderRowEnd);
                                    }
                                    blockName = dbContext.MASTER_BLOCK.Where(x => x.MAST_BLOCK_CODE == blockCode).Select(x => x.MAST_BLOCK_NAME).FirstOrDefault();
                                    i++;
                                    msgBody = msgBody.Append(htmlTrStart);
                                    msgBody = msgBody.Append(htmlTdStart + i + htmlTdEnd).Append(htmlTdStart + blockName + htmlTdEnd).Append(htmlTdStart + model.UnlockStartDate + htmlTdEnd).Append(htmlTdStart + model.UnlockEndDate + htmlTdEnd).Append(htmlTdStart + PMGSYSession.Current.UserName + htmlTdEnd).Append(htmlTdStart + roleName + htmlTdEnd);
                                    msgBody = msgBody.Append(htmlTrEnd);
                                }
                                msgBody.Append(htmlTableEnd);
                                break;
                        }
                        unlockModule = "Block Master";
                        break;
                    case "VM"://Village Master
                        msgBody.Append("<b>Module</b> : Village Master <br/>");
                        if (model.PMGSYScheme == 3)
                        {
                            msgBody.Append("<b>Scheme</b> : RCPLWE <br/>");
                        }
                        else
                        {
                            msgBody.Append("<b>Scheme</b> : PMGSY-" + (model.PMGSYScheme == 4 ? 3 : model.PMGSYScheme) + " <br/>");
                        }
                        switch (model.UnlockLevel)
                        {
                            case "S":
                                unlockLevel = "State";
                                msgBody.Append("<b>Level</b> : State <br/><br/>");
                                msgBody.Append("<b>Remarks</b> : " + model.UnlockRemarks + " <br/><br/>");
                                msgBody.Append(htmlTableStart + htmlHeaderRowStart);
                                msgBody = msgBody.Append(htmlTdStart + "Sr.No" + htmlTdEnd).Append(htmlTdStart + "State" + htmlTdEnd).Append(htmlTdStart + "Start Date" + htmlTdEnd).Append(htmlTdStart + "End Date" + htmlTdEnd).Append(htmlTdStart + "Unlocked By" + htmlTdEnd).Append(htmlTdStart + "Role" + htmlTdEnd);
                                msgBody = msgBody.Append(htmlHeaderRowEnd);
                                i = 0;
                                foreach (var item in model.dataID)
                                {
                                    stateCode = model.dataID[i];
                                    stateName = dbContext.MASTER_STATE.Where(x => x.MAST_STATE_CODE == stateCode).Select(x => x.MAST_STATE_NAME).FirstOrDefault();
                                    i++;
                                    msgBody = msgBody.Append(htmlTrStart);
                                    msgBody = msgBody.Append(htmlTdStart + i + htmlTdEnd).Append(htmlTdStart + stateName + htmlTdEnd).Append(htmlTdStart + model.UnlockStartDate + htmlTdEnd).Append(htmlTdStart + model.UnlockEndDate + htmlTdEnd).Append(htmlTdStart + PMGSYSession.Current.UserName + htmlTdEnd).Append(htmlTdStart + roleName + htmlTdEnd);
                                    msgBody = msgBody.Append(htmlTrEnd);
                                }
                                msgBody.Append(htmlTableEnd);
                                break;
                            case "D": /* District */
                                unlockLevel = "District";
                                msgBody.Append("<b>Level</b> : District <br/>");

                                i = 0;
                                foreach (var item in model.dataID)
                                {
                                    distCode = model.dataID[i];
                                    if (i == 0)
                                    {
                                        stateName = dbContext.MASTER_DISTRICT.Where(x => x.MAST_DISTRICT_CODE == distCode).Select(x => x.MASTER_STATE.MAST_STATE_NAME).FirstOrDefault();
                                        msgBody.Append("<b>State</b> : " + stateName + " <br/><br/>");
                                        msgBody.Append("<b>Remarks</b> : " + model.UnlockRemarks + " <br/><br/>");

                                        msgBody.Append(htmlTableStart + htmlHeaderRowStart);
                                        msgBody = msgBody.Append(htmlTdStart + "Sr.No" + htmlTdEnd).Append(htmlTdStart + "District" + htmlTdEnd).Append(htmlTdStart + "Start Date" + htmlTdEnd).Append(htmlTdStart + "End Date" + htmlTdEnd).Append(htmlTdStart + "Unlocked By" + htmlTdEnd).Append(htmlTdStart + "Role" + htmlTdEnd);
                                        msgBody = msgBody.Append(htmlHeaderRowEnd);
                                    }
                                    distName = dbContext.MASTER_DISTRICT.Where(x => x.MAST_DISTRICT_CODE == distCode).Select(x => x.MAST_DISTRICT_NAME).FirstOrDefault();
                                    i++;
                                    msgBody = msgBody.Append(htmlTrStart);
                                    msgBody = msgBody.Append(htmlTdStart + i + htmlTdEnd).Append(htmlTdStart + distName + htmlTdEnd).Append(htmlTdStart + model.UnlockStartDate + htmlTdEnd).Append(htmlTdStart + model.UnlockEndDate + htmlTdEnd).Append(htmlTdStart + PMGSYSession.Current.UserName + htmlTdEnd).Append(htmlTdStart + roleName + htmlTdEnd);
                                    msgBody = msgBody.Append(htmlTrEnd);
                                }
                                msgBody.Append(htmlTableEnd);
                                break;
                            case "B": /* Block */
                                unlockLevel = "Block";
                                msgBody.Append("<b>Level</b> : Block <br/>");

                                i = 0;
                                foreach (var item in model.dataID)
                                {
                                    blockCode = model.dataID[i];
                                    if (i == 0)
                                    {
                                        stateName = dbContext.MASTER_BLOCK.Where(x => x.MAST_BLOCK_CODE == blockCode).Select(x => x.MASTER_DISTRICT.MASTER_STATE.MAST_STATE_NAME).FirstOrDefault();
                                        msgBody.Append("<b>State</b> : " + stateName + " <br/>");

                                        distName = dbContext.MASTER_BLOCK.Where(x => x.MAST_BLOCK_CODE == blockCode).Select(x => x.MASTER_DISTRICT.MAST_DISTRICT_NAME).FirstOrDefault();
                                        msgBody.Append("<b>District</b> : " + distName + " <br/><br/>");
                                        msgBody.Append("<b>Remarks</b> : " + model.UnlockRemarks + " <br/><br/>");

                                        msgBody.Append(htmlTableStart + htmlHeaderRowStart);
                                        msgBody = msgBody.Append(htmlTdStart + "Sr.No" + htmlTdEnd).Append(htmlTdStart + "Block" + htmlTdEnd).Append(htmlTdStart + "Start Date" + htmlTdEnd).Append(htmlTdStart + "End Date" + htmlTdEnd).Append(htmlTdStart + "Unlocked By" + htmlTdEnd).Append(htmlTdStart + "Role" + htmlTdEnd);
                                        msgBody = msgBody.Append(htmlHeaderRowEnd);
                                    }
                                    blockName = dbContext.MASTER_BLOCK.Where(x => x.MAST_BLOCK_CODE == blockCode).Select(x => x.MAST_BLOCK_NAME).FirstOrDefault();
                                    i++;
                                    msgBody = msgBody.Append(htmlTrStart);
                                    msgBody = msgBody.Append(htmlTdStart + i + htmlTdEnd).Append(htmlTdStart + blockName + htmlTdEnd).Append(htmlTdStart + model.UnlockStartDate + htmlTdEnd).Append(htmlTdStart + model.UnlockEndDate + htmlTdEnd).Append(htmlTdStart + PMGSYSession.Current.UserName + htmlTdEnd).Append(htmlTdStart + roleName + htmlTdEnd);
                                    msgBody = msgBody.Append(htmlTrEnd);
                                }
                                msgBody.Append(htmlTableEnd);
                                break;
                            case "V": /* inner B case code */
                                unlockLevel = "Village";
                                msgBody.Append("<b>Level</b> : Village <br/>");

                                i = 0;
                                foreach (var item in model.dataID)
                                {
                                    villageCode = model.dataID[i];
                                    if (i == 0)
                                    {
                                        stateName = dbContext.MASTER_VILLAGE.Where(x => x.MAST_VILLAGE_CODE == villageCode).Select(x => x.MASTER_BLOCK.MASTER_DISTRICT.MASTER_STATE.MAST_STATE_NAME).FirstOrDefault();
                                        msgBody.Append("<b>State</b> : " + stateName + " <br/>");

                                        distName = dbContext.MASTER_VILLAGE.Where(x => x.MAST_VILLAGE_CODE == villageCode).Select(x => x.MASTER_BLOCK.MASTER_DISTRICT.MAST_DISTRICT_NAME).FirstOrDefault();
                                        msgBody.Append("<b>District</b> : " + distName + " <br/>");

                                        blockName = dbContext.MASTER_VILLAGE.Where(x => x.MAST_VILLAGE_CODE == villageCode).Select(x => x.MASTER_BLOCK.MAST_BLOCK_NAME).FirstOrDefault();
                                        msgBody.Append("<b>Block</b> : " + blockName + " <br/><br/>");
                                        msgBody.Append("<b>Remarks</b> : " + model.UnlockRemarks + " <br/><br/>");

                                        msgBody.Append(htmlTableStart + htmlHeaderRowStart);
                                        msgBody = msgBody.Append(htmlTdStart + "Sr.No" + htmlTdEnd).Append(htmlTdStart + "Village" + htmlTdEnd).Append(htmlTdStart + "Start Date" + htmlTdEnd).Append(htmlTdStart + "End Date" + htmlTdEnd).Append(htmlTdStart + "Unlocked By" + htmlTdEnd).Append(htmlTdStart + "Role" + htmlTdEnd);
                                        msgBody = msgBody.Append(htmlHeaderRowEnd);
                                    }
                                    villageName = dbContext.MASTER_VILLAGE.Where(x => x.MAST_VILLAGE_CODE == villageCode).Select(x => x.MAST_VILLAGE_NAME).FirstOrDefault();
                                    i++;
                                    msgBody = msgBody.Append(htmlTrStart);
                                    msgBody = msgBody.Append(htmlTdStart + i + htmlTdEnd).Append(htmlTdStart + villageName + htmlTdEnd).Append(htmlTdStart + model.UnlockStartDate + htmlTdEnd).Append(htmlTdStart + model.UnlockEndDate + htmlTdEnd).Append(htmlTdStart + PMGSYSession.Current.UserName + htmlTdEnd).Append(htmlTdStart + roleName + htmlTdEnd);
                                    msgBody = msgBody.Append(htmlTrEnd);
                                }
                                msgBody.Append(htmlTableEnd);
                                break;
                        }
                        unlockModule = "Village Master";
                        break;
                    case "HM"://Habitation Master
                        msgBody.Append("<b>Module</b> : Habitation Master <br/>");
                        if (model.PMGSYScheme == 3)
                        {
                            msgBody.Append("<b>Scheme</b> : RCPLWE <br/>");
                        }
                        else
                        {
                            msgBody.Append("<b>Scheme</b> : PMGSY-" + (model.PMGSYScheme == 4 ? 3 : model.PMGSYScheme) + " <br/>");
                        }
                        switch (model.UnlockLevel)
                        {
                            case "S": /* State */
                                unlockLevel = "State";
                                msgBody.Append("<b>Level</b> : State <br/><br/>");
                                msgBody.Append("<b>Remarks</b> : " + model.UnlockRemarks + " <br/><br/>");
                                msgBody.Append(htmlTableStart + htmlHeaderRowStart);
                                msgBody = msgBody.Append(htmlTdStart + "Sr.No" + htmlTdEnd).Append(htmlTdStart + "State" + htmlTdEnd).Append(htmlTdStart + "Start Date" + htmlTdEnd).Append(htmlTdStart + "End Date" + htmlTdEnd).Append(htmlTdStart + "Unlocked By" + htmlTdEnd).Append(htmlTdStart + "Role" + htmlTdEnd);
                                msgBody = msgBody.Append(htmlHeaderRowEnd);
                                i = 0;
                                foreach (var item in model.dataID)
                                {
                                    stateCode = model.dataID[i];
                                    stateName = dbContext.MASTER_STATE.Where(x => x.MAST_STATE_CODE == stateCode).Select(x => x.MAST_STATE_NAME).FirstOrDefault();
                                    i++;
                                    msgBody = msgBody.Append(htmlTrStart);
                                    msgBody = msgBody.Append(htmlTdStart + i + htmlTdEnd).Append(htmlTdStart + stateName + htmlTdEnd).Append(htmlTdStart + model.UnlockStartDate + htmlTdEnd).Append(htmlTdStart + model.UnlockEndDate + htmlTdEnd).Append(htmlTdStart + PMGSYSession.Current.UserName + htmlTdEnd).Append(htmlTdStart + roleName + htmlTdEnd);
                                    msgBody = msgBody.Append(htmlTrEnd);
                                }
                                msgBody.Append(htmlTableEnd);
                                break;
                            case "D": /* District */
                                unlockLevel = "District";
                                msgBody.Append("<b>Level</b> : District <br/>");

                                i = 0;
                                foreach (var item in model.dataID)
                                {
                                    distCode = model.dataID[i];
                                    if (i == 0)
                                    {
                                        stateName = dbContext.MASTER_DISTRICT.Where(x => x.MAST_DISTRICT_CODE == distCode).Select(x => x.MASTER_STATE.MAST_STATE_NAME).FirstOrDefault();
                                        msgBody.Append("<b>State</b> : " + stateName + " <br/><br/>");
                                        msgBody.Append("<b>Remarks</b> : " + model.UnlockRemarks + " <br/><br/>");

                                        msgBody.Append(htmlTableStart + htmlHeaderRowStart);
                                        msgBody = msgBody.Append(htmlTdStart + "Sr.No" + htmlTdEnd).Append(htmlTdStart + "District" + htmlTdEnd).Append(htmlTdStart + "Start Date" + htmlTdEnd).Append(htmlTdStart + "End Date" + htmlTdEnd).Append(htmlTdStart + "Unlocked By" + htmlTdEnd).Append(htmlTdStart + "Role" + htmlTdEnd);
                                        msgBody = msgBody.Append(htmlHeaderRowEnd);
                                    }
                                    distName = dbContext.MASTER_DISTRICT.Where(x => x.MAST_DISTRICT_CODE == distCode).Select(x => x.MAST_DISTRICT_NAME).FirstOrDefault();
                                    i++;
                                    msgBody = msgBody.Append(htmlTrStart);
                                    msgBody = msgBody.Append(htmlTdStart + i + htmlTdEnd).Append(htmlTdStart + distName + htmlTdEnd).Append(htmlTdStart + model.UnlockStartDate + htmlTdEnd).Append(htmlTdStart + model.UnlockEndDate + htmlTdEnd).Append(htmlTdStart + PMGSYSession.Current.UserName + htmlTdEnd).Append(htmlTdStart + roleName + htmlTdEnd);
                                    msgBody = msgBody.Append(htmlTrEnd);
                                }
                                msgBody.Append(htmlTableEnd);
                                break;
                            case "B": /* Block */
                                unlockLevel = "Block";
                                msgBody.Append("<b>Level</b> : Block <br/>");

                                i = 0;
                                foreach (var item in model.dataID)
                                {
                                    blockCode = model.dataID[i];
                                    if (i == 0)
                                    {
                                        stateName = dbContext.MASTER_BLOCK.Where(x => x.MAST_BLOCK_CODE == blockCode).Select(x => x.MASTER_DISTRICT.MASTER_STATE.MAST_STATE_NAME).FirstOrDefault();
                                        msgBody.Append("<b>State</b> : " + stateName + " <br/>");

                                        distName = dbContext.MASTER_BLOCK.Where(x => x.MAST_BLOCK_CODE == blockCode).Select(x => x.MASTER_DISTRICT.MAST_DISTRICT_NAME).FirstOrDefault();
                                        msgBody.Append("<b>District</b> : " + distName + " <br/><br/>");
                                        msgBody.Append("<b>Remarks</b> : " + model.UnlockRemarks + " <br/><br/>");

                                        msgBody.Append(htmlTableStart + htmlHeaderRowStart);
                                        msgBody = msgBody.Append(htmlTdStart + "Sr.No" + htmlTdEnd).Append(htmlTdStart + "Block" + htmlTdEnd).Append(htmlTdStart + "Start Date" + htmlTdEnd).Append(htmlTdStart + "End Date" + htmlTdEnd).Append(htmlTdStart + "Unlocked By" + htmlTdEnd).Append(htmlTdStart + "Role" + htmlTdEnd);
                                        msgBody = msgBody.Append(htmlHeaderRowEnd);
                                    }
                                    blockName = dbContext.MASTER_BLOCK.Where(x => x.MAST_BLOCK_CODE == blockCode).Select(x => x.MAST_BLOCK_NAME).FirstOrDefault();
                                    i++;
                                    msgBody = msgBody.Append(htmlTrStart);
                                    msgBody = msgBody.Append(htmlTdStart + i + htmlTdEnd).Append(htmlTdStart + blockName + htmlTdEnd).Append(htmlTdStart + model.UnlockStartDate + htmlTdEnd).Append(htmlTdStart + model.UnlockEndDate + htmlTdEnd).Append(htmlTdStart + PMGSYSession.Current.UserName + htmlTdEnd).Append(htmlTdStart + roleName + htmlTdEnd);
                                    msgBody = msgBody.Append(htmlTrEnd);
                                }
                                msgBody.Append(htmlTableEnd);
                                break;
                            case "V": /* Village */
                                unlockLevel = "Village";
                                msgBody.Append("<b>Level</b> : Village <br/>");

                                i = 0;
                                foreach (var item in model.dataID)
                                {
                                    villageCode = model.dataID[i];
                                    if (i == 0)
                                    {
                                        stateName = dbContext.MASTER_VILLAGE.Where(x => x.MAST_VILLAGE_CODE == villageCode).Select(x => x.MASTER_BLOCK.MASTER_DISTRICT.MASTER_STATE.MAST_STATE_NAME).FirstOrDefault();
                                        msgBody.Append("<b>State</b> : " + stateName + " <br/>");

                                        distName = dbContext.MASTER_VILLAGE.Where(x => x.MAST_VILLAGE_CODE == villageCode).Select(x => x.MASTER_BLOCK.MASTER_DISTRICT.MAST_DISTRICT_NAME).FirstOrDefault();
                                        msgBody.Append("<b>District</b> : " + distName + " <br/>");

                                        blockName = dbContext.MASTER_VILLAGE.Where(x => x.MAST_VILLAGE_CODE == villageCode).Select(x => x.MASTER_BLOCK.MAST_BLOCK_NAME).FirstOrDefault();
                                        msgBody.Append("<b>Block</b> : " + blockName + " <br/><br/>");
                                        msgBody.Append("<b>Remarks</b> : " + model.UnlockRemarks + " <br/><br/>");

                                        msgBody.Append(htmlTableStart + htmlHeaderRowStart);
                                        msgBody = msgBody.Append(htmlTdStart + "Sr.No" + htmlTdEnd).Append(htmlTdStart + "Village" + htmlTdEnd).Append(htmlTdStart + "Start Date" + htmlTdEnd).Append(htmlTdStart + "End Date" + htmlTdEnd).Append(htmlTdStart + "Unlocked By" + htmlTdEnd).Append(htmlTdStart + "Role" + htmlTdEnd);
                                        msgBody = msgBody.Append(htmlHeaderRowEnd);
                                    }

                                    villageName = dbContext.MASTER_VILLAGE.Where(x => x.MAST_VILLAGE_CODE == villageCode).Select(x => x.MAST_VILLAGE_NAME).FirstOrDefault();
                                    i++;
                                    msgBody = msgBody.Append(htmlTrStart);
                                    msgBody = msgBody.Append(htmlTdStart + i + htmlTdEnd).Append(htmlTdStart + villageName + htmlTdEnd).Append(htmlTdStart + model.UnlockStartDate + htmlTdEnd).Append(htmlTdStart + model.UnlockEndDate + htmlTdEnd).Append(htmlTdStart + PMGSYSession.Current.UserName + htmlTdEnd).Append(htmlTdStart + roleName + htmlTdEnd);
                                    msgBody = msgBody.Append(htmlTrEnd);
                                }
                                msgBody.Append(htmlTableEnd);
                                break;
                            case "H": /* Habitation */
                                unlockLevel = "Habitation";
                                msgBody.Append("<b>Level</b> : Habitation <br/>");

                                i = 0;
                                foreach (var item in model.dataID)
                                {
                                    habCode = model.dataID[i];
                                    if (i == 0)
                                    {
                                        stateName = dbContext.MASTER_HABITATIONS.Where(x => x.MAST_HAB_CODE == habCode).Select(x => x.MASTER_VILLAGE.MASTER_BLOCK.MASTER_DISTRICT.MASTER_STATE.MAST_STATE_NAME).FirstOrDefault();
                                        msgBody.Append("<b>State</b> : " + stateName + " <br/>");

                                        distName = dbContext.MASTER_HABITATIONS.Where(x => x.MAST_HAB_CODE == habCode).Select(x => x.MASTER_VILLAGE.MASTER_BLOCK.MASTER_DISTRICT.MAST_DISTRICT_NAME).FirstOrDefault();
                                        msgBody.Append("<b>District</b> : " + distName + " <br/>");

                                        blockName = dbContext.MASTER_HABITATIONS.Where(x => x.MAST_HAB_CODE == habCode).Select(x => x.MASTER_VILLAGE.MASTER_BLOCK.MAST_BLOCK_NAME).FirstOrDefault();
                                        msgBody.Append("<b>Block</b> : " + blockName + " <br/>");

                                        villageName = dbContext.MASTER_HABITATIONS.Where(x => x.MAST_HAB_CODE == habCode).Select(x => x.MASTER_VILLAGE.MAST_VILLAGE_NAME).FirstOrDefault();
                                        msgBody.Append("<b>Village</b> : " + villageName + " <br/><br/>");
                                        msgBody.Append("<b>Remarks</b> : " + model.UnlockRemarks + " <br/><br/>");

                                        msgBody.Append(htmlTableStart + htmlHeaderRowStart);
                                        msgBody = msgBody.Append(htmlTdStart + "Sr.No" + htmlTdEnd).Append(htmlTdStart + "Habitation" + htmlTdEnd).Append(htmlTdStart + "Start Date" + htmlTdEnd).Append(htmlTdStart + "End Date" + htmlTdEnd).Append(htmlTdStart + "Unlocked By" + htmlTdEnd).Append(htmlTdStart + "Role" + htmlTdEnd);
                                        msgBody = msgBody.Append(htmlHeaderRowEnd);
                                    }
                                    habName = dbContext.MASTER_HABITATIONS.Where(x => x.MAST_HAB_CODE == habCode).Select(x => x.MAST_HAB_NAME).FirstOrDefault();
                                    i++;
                                    msgBody = msgBody.Append(htmlTrStart);
                                    msgBody = msgBody.Append(htmlTdStart + i + htmlTdEnd).Append(htmlTdStart + habName + htmlTdEnd).Append(htmlTdStart + model.UnlockStartDate + htmlTdEnd).Append(htmlTdStart + model.UnlockEndDate + htmlTdEnd).Append(htmlTdStart + PMGSYSession.Current.UserName + htmlTdEnd).Append(htmlTdStart + roleName + htmlTdEnd);
                                    msgBody = msgBody.Append(htmlTrEnd);
                                }
                                msgBody.Append(htmlTableEnd);
                                break;
                        }
                        unlockModule = "Habitation Master";
                        break;
                    case "ER"://Existing Roads
                        msgBody.Append("<b>Module</b> : Existing Roads <br/>");
                        if (model.PMGSYScheme == 3)
                        {
                            msgBody.Append("<b>Scheme</b> : RCPLWE <br/>");
                        }
                        else
                        {
                            msgBody.Append("<b>Scheme</b> : PMGSY-" + (model.PMGSYScheme == 4 ? 3 : model.PMGSYScheme) + " <br/>");
                        }
                        switch (model.UnlockLevel)
                        {
                            case "S": /* State */
                                unlockLevel = "State";
                                msgBody.Append("<b>Level</b> : State <br/><br/>");
                                msgBody.Append("<b>Remarks</b> : " + model.UnlockRemarks + " <br/><br/>");
                                msgBody.Append(htmlTableStart + htmlHeaderRowStart);
                                msgBody = msgBody.Append(htmlTdStart + "Sr.No" + htmlTdEnd).Append(htmlTdStart + "State" + htmlTdEnd).Append(htmlTdStart + "Start Date" + htmlTdEnd).Append(htmlTdStart + "End Date" + htmlTdEnd).Append(htmlTdStart + "Unlocked By" + htmlTdEnd).Append(htmlTdStart + "Role" + htmlTdEnd);
                                msgBody = msgBody.Append(htmlHeaderRowEnd);
                                i = 0;
                                foreach (var item in model.dataID)
                                {
                                    stateCode = model.dataID[i];
                                    stateName = dbContext.MASTER_STATE.Where(x => x.MAST_STATE_CODE == stateCode).Select(x => x.MAST_STATE_NAME).FirstOrDefault();
                                    i++;
                                    msgBody = msgBody.Append(htmlTrStart);
                                    msgBody = msgBody.Append(htmlTdStart + i + htmlTdEnd).Append(htmlTdStart + stateName + htmlTdEnd).Append(htmlTdStart + model.UnlockStartDate + htmlTdEnd).Append(htmlTdStart + model.UnlockEndDate + htmlTdEnd).Append(htmlTdStart + PMGSYSession.Current.UserName + htmlTdEnd).Append(htmlTdStart + roleName + htmlTdEnd);
                                    msgBody = msgBody.Append(htmlTrEnd);
                                }
                                msgBody.Append(htmlTableEnd);
                                break;
                            case "D": /* District */
                                unlockLevel = "District";
                                msgBody.Append("<b>Level</b> : District <br/>");

                                i = 0;
                                foreach (var item in model.dataID)
                                {
                                    distCode = model.dataID[i];
                                    if (i == 0)
                                    {
                                        stateName = dbContext.MASTER_DISTRICT.Where(x => x.MAST_DISTRICT_CODE == distCode).Select(x => x.MASTER_STATE.MAST_STATE_NAME).FirstOrDefault();
                                        msgBody.Append("<b>State</b> : " + stateName + " <br/><br/>");
                                        msgBody.Append("<b>Remarks</b> : " + model.UnlockRemarks + " <br/><br/>");

                                        msgBody.Append(htmlTableStart + htmlHeaderRowStart);
                                        msgBody = msgBody.Append(htmlTdStart + "Sr.No" + htmlTdEnd).Append(htmlTdStart + "District" + htmlTdEnd).Append(htmlTdStart + "Start Date" + htmlTdEnd).Append(htmlTdStart + "End Date" + htmlTdEnd).Append(htmlTdStart + "Unlocked By" + htmlTdEnd).Append(htmlTdStart + "Role" + htmlTdEnd);
                                        msgBody = msgBody.Append(htmlHeaderRowEnd);
                                    }
                                    distName = dbContext.MASTER_DISTRICT.Where(x => x.MAST_DISTRICT_CODE == distCode).Select(x => x.MAST_DISTRICT_NAME).FirstOrDefault();
                                    i++;
                                    msgBody = msgBody.Append(htmlTrStart);
                                    msgBody = msgBody.Append(htmlTdStart + i + htmlTdEnd).Append(htmlTdStart + distName + htmlTdEnd).Append(htmlTdStart + model.UnlockStartDate + htmlTdEnd).Append(htmlTdStart + model.UnlockEndDate + htmlTdEnd).Append(htmlTdStart + PMGSYSession.Current.UserName + htmlTdEnd).Append(htmlTdStart + roleName + htmlTdEnd);
                                    msgBody = msgBody.Append(htmlTrEnd);
                                }
                                msgBody.Append(htmlTableEnd);
                                break;
                            case "B": /* Block */
                                unlockLevel = "Block";
                                msgBody.Append("<b>Level</b> : Block <br/>");

                                i = 0;
                                foreach (var item in model.dataID)
                                {
                                    blockCode = model.dataID[i];
                                    if (i == 0)
                                    {
                                        stateName = dbContext.MASTER_BLOCK.Where(x => x.MAST_BLOCK_CODE == blockCode).Select(x => x.MASTER_DISTRICT.MASTER_STATE.MAST_STATE_NAME).FirstOrDefault();
                                        msgBody.Append("<b>State</b> : " + stateName + " <br/>");

                                        distName = dbContext.MASTER_BLOCK.Where(x => x.MAST_BLOCK_CODE == blockCode).Select(x => x.MASTER_DISTRICT.MAST_DISTRICT_NAME).FirstOrDefault();
                                        msgBody.Append("<b>District</b> : " + distName + " <br/><br/>");
                                        msgBody.Append("<b>Remarks</b> : " + model.UnlockRemarks + " <br/><br/>");

                                        msgBody.Append(htmlTableStart + htmlHeaderRowStart);
                                        msgBody = msgBody.Append(htmlTdStart + "Sr.No" + htmlTdEnd).Append(htmlTdStart + "Block" + htmlTdEnd).Append(htmlTdStart + "Start Date" + htmlTdEnd).Append(htmlTdStart + "End Date" + htmlTdEnd).Append(htmlTdStart + "Unlocked By" + htmlTdEnd).Append(htmlTdStart + "Role" + htmlTdEnd);
                                        msgBody = msgBody.Append(htmlHeaderRowEnd);
                                    }
                                    blockName = dbContext.MASTER_BLOCK.Where(x => x.MAST_BLOCK_CODE == blockCode).Select(x => x.MAST_BLOCK_NAME).FirstOrDefault();
                                    i++;
                                    msgBody = msgBody.Append(htmlTrStart);
                                    msgBody = msgBody.Append(htmlTdStart + i + htmlTdEnd).Append(htmlTdStart + blockName + htmlTdEnd).Append(htmlTdStart + model.UnlockStartDate + htmlTdEnd).Append(htmlTdStart + model.UnlockEndDate + htmlTdEnd).Append(htmlTdStart + PMGSYSession.Current.UserName + htmlTdEnd).Append(htmlTdStart + roleName + htmlTdEnd);
                                    msgBody = msgBody.Append(htmlTrEnd);
                                }
                                msgBody.Append(htmlTableEnd);
                                break;
                            case "R": /* Road */
                                unlockLevel = "Road";
                                msgBody.Append("<b>Level</b> : Road <br/>");

                                i = 0;
                                foreach (var item in model.dataID)
                                {
                                    erCode = model.dataID[i];
                                    if (i == 0)
                                    {
                                        stateName = dbContext.MASTER_EXISTING_ROADS.Where(x => x.MAST_ER_ROAD_CODE == erCode).Select(x => x.MASTER_BLOCK.MASTER_DISTRICT.MASTER_STATE.MAST_STATE_NAME).FirstOrDefault();
                                        msgBody.Append("<b>State</b> : " + stateName + " <br/>");

                                        distName = dbContext.MASTER_EXISTING_ROADS.Where(x => x.MAST_ER_ROAD_CODE == erCode).Select(x => x.MASTER_BLOCK.MASTER_DISTRICT.MAST_DISTRICT_NAME).FirstOrDefault();
                                        msgBody.Append("<b>District</b> : " + distName + " <br/>");

                                        blockName = dbContext.MASTER_EXISTING_ROADS.Where(x => x.MAST_ER_ROAD_CODE == erCode).Select(x => x.MASTER_BLOCK.MAST_BLOCK_NAME).FirstOrDefault();
                                        msgBody.Append("<b>Block</b> : " + blockName + " <br/><br/>");
                                        msgBody.Append("<b>Remarks</b> : " + model.UnlockRemarks + " <br/><br/>");

                                        msgBody.Append(htmlTableStart + htmlHeaderRowStart);
                                        msgBody = msgBody.Append(htmlTdStart + "Sr.No" + htmlTdEnd).Append(htmlTdStart + "Existing Road" + htmlTdEnd).Append(htmlTdStart + "Start Date" + htmlTdEnd).Append(htmlTdStart + "End Date" + htmlTdEnd).Append(htmlTdStart + "Unlocked By" + htmlTdEnd).Append(htmlTdStart + "Role" + htmlTdEnd);
                                        msgBody = msgBody.Append(htmlHeaderRowEnd);
                                    }

                                    erName = dbContext.MASTER_EXISTING_ROADS.Where(x => x.MAST_ER_ROAD_CODE == erCode).Select(x => x.MAST_ER_ROAD_NAME).FirstOrDefault();
                                    i++;
                                    msgBody = msgBody.Append(htmlTrStart);
                                    msgBody = msgBody.Append(htmlTdStart + i + htmlTdEnd).Append(htmlTdStart + erName + htmlTdEnd).Append(htmlTdStart + model.UnlockStartDate + htmlTdEnd).Append(htmlTdStart + model.UnlockEndDate + htmlTdEnd).Append(htmlTdStart + PMGSYSession.Current.UserName + htmlTdEnd).Append(htmlTdStart + roleName + htmlTdEnd);
                                    msgBody = msgBody.Append(htmlTrEnd);
                                }
                                msgBody.Append(htmlTableEnd);
                                break;
                        }
                        unlockModule = "Existing Roads";
                        break;
                    case "CN"://Core Network
                        msgBody.Append("<b>Module</b> : Core Network <br/>");
                        if (model.PMGSYScheme == 3)
                        {
                            msgBody.Append("<b>Scheme</b> : RCPLWE <br/>");
                        }
                        else
                        {
                            msgBody.Append("<b>Scheme</b> : PMGSY-" + (model.PMGSYScheme == 4 ? 3 : model.PMGSYScheme) + " <br/>");
                        }
                        switch (model.UnlockLevel)
                        {
                            case "S": /* State */
                                unlockLevel = "State";
                                msgBody.Append("<b>Level</b> : State <br/><br/>");
                                msgBody.Append("<b>Remarks</b> : " + model.UnlockRemarks + " <br/><br/>");
                                msgBody.Append(htmlTableStart + htmlHeaderRowStart);
                                msgBody = msgBody.Append(htmlTdStart + "Sr.No" + htmlTdEnd).Append(htmlTdStart + "State" + htmlTdEnd).Append(htmlTdStart + "Start Date" + htmlTdEnd).Append(htmlTdStart + "End Date" + htmlTdEnd).Append(htmlTdStart + "Unlocked By" + htmlTdEnd).Append(htmlTdStart + "Role" + htmlTdEnd);
                                msgBody = msgBody.Append(htmlHeaderRowEnd);
                                i = 0;
                                foreach (var item in model.dataID)
                                {
                                    stateCode = model.dataID[i];
                                    stateName = dbContext.MASTER_STATE.Where(x => x.MAST_STATE_CODE == stateCode).Select(x => x.MAST_STATE_NAME).FirstOrDefault();
                                    i++;
                                    msgBody = msgBody.Append(htmlTrStart);
                                    msgBody = msgBody.Append(htmlTdStart + i + htmlTdEnd).Append(htmlTdStart + stateName + htmlTdEnd).Append(htmlTdStart + model.UnlockStartDate + htmlTdEnd).Append(htmlTdStart + model.UnlockEndDate + htmlTdEnd).Append(htmlTdStart + PMGSYSession.Current.UserName + htmlTdEnd).Append(htmlTdStart + roleName + htmlTdEnd);
                                    msgBody = msgBody.Append(htmlTrEnd);
                                }
                                msgBody.Append(htmlTableEnd);
                                break;
                            case "D": /* District */
                                unlockLevel = "District";
                                msgBody.Append("<b>Level</b> : District <br/>");

                                i = 0;
                                foreach (var item in model.dataID)
                                {
                                    distCode = model.dataID[i];
                                    if (i == 0)
                                    {
                                        stateName = dbContext.MASTER_DISTRICT.Where(x => x.MAST_DISTRICT_CODE == distCode).Select(x => x.MASTER_STATE.MAST_STATE_NAME).FirstOrDefault();
                                        msgBody.Append("<b>State</b> : " + stateName + " <br/><br/>");
                                        msgBody.Append("<b>Remarks</b> : " + model.UnlockRemarks + " <br/><br/>");

                                        msgBody.Append(htmlTableStart + htmlHeaderRowStart);
                                        msgBody = msgBody.Append(htmlTdStart + "Sr.No" + htmlTdEnd).Append(htmlTdStart + "District" + htmlTdEnd).Append(htmlTdStart + "Start Date" + htmlTdEnd).Append(htmlTdStart + "End Date" + htmlTdEnd).Append(htmlTdStart + "Unlocked By" + htmlTdEnd).Append(htmlTdStart + "Role" + htmlTdEnd);
                                        msgBody = msgBody.Append(htmlHeaderRowEnd);
                                    }
                                    distName = dbContext.MASTER_DISTRICT.Where(x => x.MAST_DISTRICT_CODE == distCode).Select(x => x.MAST_DISTRICT_NAME).FirstOrDefault();
                                    i++;
                                    msgBody = msgBody.Append(htmlTrStart);
                                    msgBody = msgBody.Append(htmlTdStart + i + htmlTdEnd).Append(htmlTdStart + distName + htmlTdEnd).Append(htmlTdStart + model.UnlockStartDate + htmlTdEnd).Append(htmlTdStart + model.UnlockEndDate + htmlTdEnd).Append(htmlTdStart + PMGSYSession.Current.UserName + htmlTdEnd).Append(htmlTdStart + roleName + htmlTdEnd);
                                    msgBody = msgBody.Append(htmlTrEnd);
                                }
                                msgBody.Append(htmlTableEnd);
                                break;
                            case "B": /* Block */
                                unlockLevel = "Block";
                                msgBody.Append("<b>Level</b> : Block <br/>");

                                i = 0;
                                foreach (var item in model.dataID)
                                {
                                    blockCode = model.dataID[i];
                                    if (i == 0)
                                    {
                                        stateName = dbContext.MASTER_BLOCK.Where(x => x.MAST_BLOCK_CODE == blockCode).Select(x => x.MASTER_DISTRICT.MASTER_STATE.MAST_STATE_NAME).FirstOrDefault();
                                        msgBody.Append("<b>State</b> : " + stateName + " <br/>");

                                        distName = dbContext.MASTER_BLOCK.Where(x => x.MAST_BLOCK_CODE == blockCode).Select(x => x.MASTER_DISTRICT.MAST_DISTRICT_NAME).FirstOrDefault();
                                        msgBody.Append("<b>District</b> : " + distName + " <br/><br/>");
                                        msgBody.Append("<b>Remarks</b> : " + model.UnlockRemarks + " <br/><br/>");

                                        msgBody.Append(htmlTableStart + htmlHeaderRowStart);
                                        msgBody = msgBody.Append(htmlTdStart + "Sr.No" + htmlTdEnd).Append(htmlTdStart + "Block" + htmlTdEnd).Append(htmlTdStart + "Start Date" + htmlTdEnd).Append(htmlTdStart + "End Date" + htmlTdEnd).Append(htmlTdStart + "Unlocked By" + htmlTdEnd).Append(htmlTdStart + "Role" + htmlTdEnd);
                                        msgBody = msgBody.Append(htmlHeaderRowEnd);
                                    }
                                    blockName = dbContext.MASTER_BLOCK.Where(x => x.MAST_BLOCK_CODE == blockCode).Select(x => x.MAST_BLOCK_NAME).FirstOrDefault();
                                    i++;
                                    msgBody = msgBody.Append(htmlTrStart);
                                    msgBody = msgBody.Append(htmlTdStart + i + htmlTdEnd).Append(htmlTdStart + blockName + htmlTdEnd).Append(htmlTdStart + model.UnlockStartDate + htmlTdEnd).Append(htmlTdStart + model.UnlockEndDate + htmlTdEnd).Append(htmlTdStart + PMGSYSession.Current.UserName + htmlTdEnd).Append(htmlTdStart + roleName + htmlTdEnd);
                                    msgBody = msgBody.Append(htmlTrEnd);
                                }
                                msgBody.Append(htmlTableEnd);
                                break;
                            case "R": /* Road */
                                unlockLevel = "Road";
                                msgBody.Append("<b>Level</b> : Road <br/>");

                                i = 0;
                                foreach (var item in model.dataID)
                                {
                                    cnCode = model.dataID[i];
                                    if (i == 0)
                                    {
                                        stateName = dbContext.PLAN_ROAD.Where(x => x.PLAN_CN_ROAD_CODE == cnCode).Select(x => x.MASTER_BLOCK.MASTER_DISTRICT.MASTER_STATE.MAST_STATE_NAME).FirstOrDefault();
                                        msgBody.Append("<b>State</b> : " + stateName + " <br/>");

                                        distName = dbContext.PLAN_ROAD.Where(x => x.PLAN_CN_ROAD_CODE == cnCode).Select(x => x.MASTER_BLOCK.MASTER_DISTRICT.MAST_DISTRICT_NAME).FirstOrDefault();
                                        msgBody.Append("<b>District</b> : " + distName + " <br/>");

                                        blockName = dbContext.PLAN_ROAD.Where(x => x.PLAN_CN_ROAD_CODE == cnCode).Select(x => x.MASTER_BLOCK.MAST_BLOCK_NAME).FirstOrDefault();
                                        msgBody.Append("<b>Block</b> : " + blockName + " <br/><br/>");
                                        msgBody.Append("<b>Remarks</b> : " + model.UnlockRemarks + " <br/><br/>");

                                        msgBody.Append(htmlTableStart + htmlHeaderRowStart);
                                        msgBody = msgBody.Append(htmlTdStart + "Sr.No" + htmlTdEnd).Append(htmlTdStart + "Core Network Road" + htmlTdEnd).Append(htmlTdStart + "Start Date" + htmlTdEnd).Append(htmlTdStart + "End Date" + htmlTdEnd).Append(htmlTdStart + "Unlocked By" + htmlTdEnd).Append(htmlTdStart + "Role" + htmlTdEnd);
                                        msgBody = msgBody.Append(htmlHeaderRowEnd);
                                    }

                                    cnName = dbContext.PLAN_ROAD.Where(x => x.PLAN_CN_ROAD_CODE == cnCode).Select(x => x.PLAN_RD_NAME).FirstOrDefault();
                                    i++;
                                    msgBody = msgBody.Append(htmlTrStart);
                                    msgBody = msgBody.Append(htmlTdStart + i + htmlTdEnd).Append(htmlTdStart + cnName + htmlTdEnd).Append(htmlTdStart + model.UnlockStartDate + htmlTdEnd).Append(htmlTdStart + model.UnlockEndDate + htmlTdEnd).Append(htmlTdStart + PMGSYSession.Current.UserName + htmlTdEnd).Append(htmlTdStart + roleName + htmlTdEnd);
                                    msgBody = msgBody.Append(htmlTrEnd);
                                }
                                msgBody.Append(htmlTableEnd);
                                break;
                        }
                        unlockModule = "Core Network";
                        break;
                    case "PR"://Proposal
                        msgBody.Append("<b>Module</b> : Proposal <br/>");
                        if (model.PMGSYScheme == 3)
                        {
                            msgBody.Append("<b>Scheme</b> : RCPLWE <br/>");
                        }
                        else
                        {
                            msgBody.Append("<b>Scheme</b> : PMGSY-" + (model.PMGSYScheme == 4 ? 3 : model.PMGSYScheme) + " <br/>");
                        }
                        switch (model.UnlockLevel)
                        {
                            case "R":
                                unlockLevel = "Road";
                                msgBody.Append("<b>Level</b> : Road <br/>");

                                i = 0;
                                foreach (var item in model.dataID)
                                {
                                    prCode = model.dataID[i];
                                    if (i == 0)
                                    {
                                        stateName = model.StateCode > 0 ? dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == prCode).Select(x => x.MASTER_BLOCK.MASTER_DISTRICT.MASTER_STATE.MAST_STATE_NAME).FirstOrDefault() : "All States";
                                        msgBody.Append("<b>State</b> : " + stateName + " <br/>");

                                        distName = model.DistrictCode > 0 ? dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == prCode).Select(x => x.MASTER_BLOCK.MASTER_DISTRICT.MAST_DISTRICT_NAME).FirstOrDefault() : "All Districts";
                                        msgBody.Append("<b>District</b> : " + distName + " <br/>");

                                        blockName = model.BlockCode > 0 ? dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == prCode).Select(x => x.MASTER_BLOCK.MAST_BLOCK_NAME).FirstOrDefault() : "All Blocks";
                                        msgBody.Append("<b>Block</b> : " + blockName + " <br/>");

                                        var prop = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == prCode).FirstOrDefault();
                                        if (prop != null)
                                        {
                                            yearName = model.YearCode > 0 ? prop.IMS_YEAR + "-" + (prop.IMS_YEAR + 1) : "All Years";
                                            batchName = model.BatchCode > 0 ? "Batch-" + model.BatchCode : "All Batches";
                                            model.Package = (model.Package == "0" || model.Package == "-1") ? "All Packages" : prop.IMS_PACKAGE_ID;

                                            msgBody.Append("<b>Year</b> : " + yearName + " <br/>");
                                            msgBody.Append("<b>Batch</b> : " + batchName + " <br/>");
                                            msgBody.Append("<b>Package</b> : " + model.Package + " <br/>");
                                            //msgBody.Append("<b>Type</b> : " + (prop.IMS_ISCOMPLETED == "M" ? "MoRD Sanctioned" : "STA/PTA Scrutinized") + " <br/><br/>");
                                        }

                                        if (PMGSYSession.Current.RoleCode != 36 && PMGSYSession.Current.RoleCode != 47)
                                        {
                                            msgBody.Append("<b>Type</b> : " + (model.sanctionType == "M" ? "MoRD Sanctioned" : "STA/PTA Scrutinized") + " <br/><br/>");
                                        }
                                        msgBody.Append("<b>Remarks</b> : " + model.UnlockRemarks + " <br/><br/>");
                                        msgBody.Append(htmlTableStart + htmlHeaderRowStart);
                                        msgBody = msgBody.Append(htmlTdStart + "Sr.No" + htmlTdEnd).Append(htmlTdStart + "Proposal Road" + htmlTdEnd).Append(htmlTdStart + "Start Date" + htmlTdEnd).Append(htmlTdStart + "End Date" + htmlTdEnd).Append(htmlTdStart + "Unlocked By" + htmlTdEnd).Append(htmlTdStart + "Role" + htmlTdEnd);
                                        msgBody = msgBody.Append(htmlHeaderRowEnd);
                                    }

                                    workName = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == prCode).Select(x => x.IMS_ROAD_NAME).FirstOrDefault();
                                    i++;
                                    msgBody = msgBody.Append(htmlTrStart);
                                    msgBody = msgBody.Append(htmlTdStart + i + htmlTdEnd).Append(htmlTdStart + workName + htmlTdEnd).Append(htmlTdStart + model.UnlockStartDate + htmlTdEnd).Append(htmlTdStart + model.UnlockEndDate + htmlTdEnd).Append(htmlTdStart + PMGSYSession.Current.UserName + htmlTdEnd).Append(htmlTdStart + roleName + htmlTdEnd);
                                    msgBody = msgBody.Append(htmlTrEnd);
                                }
                                msgBody.Append(htmlTableEnd);
                                break;
                            case "Y": /* inner B case code */
                                unlockLevel = "Year";
                                msgBody.Append("<b>Level</b> : Year <br/>");

                                i = 0;
                                foreach (var item in model.dataID)
                                {
                                    yearCode = model.dataID[i];
                                    if (i == 0)
                                    {
                                        stateName = model.StateCode > 0 ? dbContext.MASTER_STATE.Where(x => x.MAST_STATE_CODE == model.StateCode).Select(x => x.MAST_STATE_NAME).FirstOrDefault() : "All States";
                                        msgBody.Append("<b>State</b> : " + stateName + " <br/>");

                                        distName = model.DistrictCode > 0 ? dbContext.MASTER_DISTRICT.Where(x => x.MAST_DISTRICT_CODE == model.DistrictCode).Select(x => x.MAST_DISTRICT_NAME).FirstOrDefault() : "All Districts";
                                        msgBody.Append("<b>District</b> : " + distName + " <br/>");

                                        blockName = model.BlockCode > 0 ? dbContext.MASTER_BLOCK.Where(x => x.MAST_BLOCK_CODE == model.BlockCode).Select(x => x.MAST_BLOCK_NAME).FirstOrDefault() : "All Blocks";
                                        msgBody.Append("<b>Block</b> : " + blockName + " <br/>");

                                        msgBody.Append("<b>Type</b> : " + (model.sanctionType == "M" ? "MoRD Sanctioned" : "STA/PTA Scrutinized") + " <br/><br/>");

                                        msgBody.Append("<b>Remarks</b> : " + model.UnlockRemarks + " <br/><br/>");
                                        msgBody.Append(htmlTableStart + htmlHeaderRowStart);
                                        msgBody = msgBody.Append(htmlTdStart + "Sr.No" + htmlTdEnd).Append(htmlTdStart + "Year" + htmlTdEnd).Append(htmlTdStart + "Start Date" + htmlTdEnd).Append(htmlTdStart + "End Date" + htmlTdEnd).Append(htmlTdStart + "Unlocked By" + htmlTdEnd).Append(htmlTdStart + "Role" + htmlTdEnd);
                                        msgBody = msgBody.Append(htmlHeaderRowEnd);
                                    }

                                    //workName = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == prCode).Select(x => x.IMS_ROAD_NAME).FirstOrDefault();
                                    i++;
                                    msgBody = msgBody.Append(htmlTrStart);
                                    msgBody = msgBody.Append(htmlTdStart + i + htmlTdEnd).Append(htmlTdStart + yearCode + "-" + (yearCode + 1) + htmlTdEnd).Append(htmlTdStart + model.UnlockStartDate + htmlTdEnd).Append(htmlTdStart + model.UnlockEndDate + htmlTdEnd).Append(htmlTdStart + PMGSYSession.Current.UserName + htmlTdEnd).Append(htmlTdStart + roleName + htmlTdEnd);
                                    msgBody = msgBody.Append(htmlTrEnd);
                                }
                                msgBody.Append(htmlTableEnd);
                                break;
                            case "T": /* inner B case code */
                                unlockLevel = "Batch";
                                msgBody.Append("<b>Level</b> : Batch <br/>");

                                i = 0;
                                foreach (var item in model.dataID)
                                {
                                    batch = model.dataID[i];
                                    if (i == 0)
                                    {
                                        stateName = model.StateCode > 0 ? dbContext.MASTER_STATE.Where(x => x.MAST_STATE_CODE == model.StateCode).Select(x => x.MAST_STATE_NAME).FirstOrDefault() : "All States";
                                        msgBody.Append("<b>State</b> : " + stateName + " <br/>");

                                        distName = model.DistrictCode > 0 ? dbContext.MASTER_DISTRICT.Where(x => x.MAST_DISTRICT_CODE == model.DistrictCode).Select(x => x.MAST_DISTRICT_NAME).FirstOrDefault() : "All Districts";
                                        msgBody.Append("<b>District</b> : " + distName + " <br/>");

                                        blockName = model.BlockCode > 0 ? dbContext.MASTER_BLOCK.Where(x => x.MAST_BLOCK_CODE == model.BlockCode).Select(x => x.MAST_BLOCK_NAME).FirstOrDefault() : "All Blocks";
                                        msgBody.Append("<b>Block</b> : " + blockName + " <br/>");

                                        //model.YearCode = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == prCode).Select(x => x.IMS_YEAR).FirstOrDefault();
                                        yearName = model.YearCode > 0 ? dbContext.MASTER_YEAR.Where(x => x.MAST_YEAR_CODE == model.YearCode).Select(x => x.MAST_YEAR_TEXT).FirstOrDefault() : "All Years";
                                        msgBody.Append("<b>Year</b> : " + yearName + " <br/>");

                                        msgBody.Append("<b>Type</b> : " + (model.sanctionType == "M" ? "MoRD Sanctioned" : "STA/PTA Scrutinized") + " <br/><br/>");
                                        msgBody.Append("<b>Remarks</b> : " + model.UnlockRemarks + " <br/><br/>");

                                        msgBody.Append(htmlTableStart + htmlHeaderRowStart);
                                        msgBody = msgBody.Append(htmlTdStart + "Sr.No" + htmlTdEnd).Append(htmlTdStart + "Batch" + htmlTdEnd).Append(htmlTdStart + "Start Date" + htmlTdEnd).Append(htmlTdStart + "End Date" + htmlTdEnd).Append(htmlTdStart + "Unlocked By" + htmlTdEnd).Append(htmlTdStart + "Role" + htmlTdEnd);
                                        msgBody = msgBody.Append(htmlHeaderRowEnd);
                                    }
                                    //workName = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == prCode).Select(x => x.IMS_ROAD_NAME).FirstOrDefault();

                                    i++;
                                    msgBody = msgBody.Append(htmlTrStart);
                                    msgBody = msgBody.Append(htmlTdStart + i + htmlTdEnd).Append(htmlTdStart + "Batch-" + batch + htmlTdEnd).Append(htmlTdStart + model.UnlockStartDate + htmlTdEnd).Append(htmlTdStart + model.UnlockEndDate + htmlTdEnd).Append(htmlTdStart + PMGSYSession.Current.UserName + htmlTdEnd).Append(htmlTdStart + roleName + htmlTdEnd);
                                    msgBody = msgBody.Append(htmlTrEnd);
                                }
                                msgBody.Append(htmlTableEnd);
                                break;
                        }
                        unlockModule = "Proposal";
                        break;
                    case "PH"://Proposal Habitation
                        msgBody.Append("<b>Module</b> : Proposal Habitation <br/>");
                        if (model.PMGSYScheme == 3)
                        {
                            msgBody.Append("<b>Scheme</b> : RCPLWE <br/>");
                        }
                        else
                        {
                            msgBody.Append("<b>Scheme</b> : PMGSY-" + (model.PMGSYScheme == 4 ? 3 : model.PMGSYScheme) + " <br/>");
                        }
                        switch (model.UnlockLevel)
                        {
                            case "R":
                                unlockLevel = "Road";
                                msgBody.Append("<b>Level</b> : Road <br/>");

                                i = 0;
                                foreach (var item in model.dataID)
                                {
                                    prCode = model.dataID[i];
                                    if (i == 0)
                                    {
                                        stateName = model.StateCode > 0 ? dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == prCode).Select(x => x.MASTER_BLOCK.MASTER_DISTRICT.MASTER_STATE.MAST_STATE_NAME).FirstOrDefault() : "All States";
                                        msgBody.Append("<b>State</b> : " + stateName + " <br/>");

                                        distName = model.DistrictCode > 0 ? dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == prCode).Select(x => x.MASTER_BLOCK.MASTER_DISTRICT.MAST_DISTRICT_NAME).FirstOrDefault() : "All Districts";
                                        msgBody.Append("<b>District</b> : " + distName + " <br/>");

                                        blockName = model.BlockCode > 0 ? dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == prCode).Select(x => x.MASTER_BLOCK.MAST_BLOCK_NAME).FirstOrDefault() : "All Blocks";
                                        msgBody.Append("<b>Block</b> : " + blockName + " <br/>");

                                        var prop = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == prCode).FirstOrDefault();
                                        if (prop != null)
                                        {
                                            yearName = model.YearCode > 0 ? prop.IMS_YEAR + "-" + (prop.IMS_YEAR + 1) : "All Years";
                                            batchName = model.BatchCode > 0 ? "Batch-" + model.BatchCode : "All Batches";
                                            model.Package = (model.Package == "0" || model.Package == "-1") ? "All Packages" : prop.IMS_PACKAGE_ID;

                                            msgBody.Append("<b>Year</b> : " + yearName + " <br/>");
                                            msgBody.Append("<b>Batch</b> : " + batchName + " <br/>");
                                            msgBody.Append("<b>Package</b> : " + model.Package + " <br/>");
                                            //msgBody.Append("<b>Type</b> : " + (prop.IMS_ISCOMPLETED == "M" ? "MoRD Sanctioned" : "STA/PTA Scrutinized") + " <br/><br/>");
                                        }

                                        if (PMGSYSession.Current.RoleCode != 36 && PMGSYSession.Current.RoleCode != 47)
                                        {
                                            msgBody.Append("<b>Type</b> : " + (model.sanctionType == "M" ? "MoRD Sanctioned" : "STA/PTA Scrutinized") + " <br/><br/>");
                                        }
                                        msgBody.Append("<b>Remarks</b> : " + model.UnlockRemarks + " <br/><br/>");
                                        msgBody.Append(htmlTableStart + htmlHeaderRowStart);
                                        msgBody = msgBody.Append(htmlTdStart + "Sr.No" + htmlTdEnd).Append(htmlTdStart + "Proposal Road" + htmlTdEnd).Append(htmlTdStart + "Start Date" + htmlTdEnd).Append(htmlTdStart + "End Date" + htmlTdEnd).Append(htmlTdStart + "Unlocked By" + htmlTdEnd).Append(htmlTdStart + "Role" + htmlTdEnd);
                                        msgBody = msgBody.Append(htmlHeaderRowEnd);
                                    }

                                    workName = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == prCode).Select(x => x.IMS_ROAD_NAME).FirstOrDefault();
                                    i++;
                                    msgBody = msgBody.Append(htmlTrStart);
                                    msgBody = msgBody.Append(htmlTdStart + i + htmlTdEnd).Append(htmlTdStart + workName + htmlTdEnd).Append(htmlTdStart + model.UnlockStartDate + htmlTdEnd).Append(htmlTdStart + model.UnlockEndDate + htmlTdEnd).Append(htmlTdStart + PMGSYSession.Current.UserName + htmlTdEnd).Append(htmlTdStart + roleName + htmlTdEnd);
                                    msgBody = msgBody.Append(htmlTrEnd);
                                }
                                msgBody.Append(htmlTableEnd);
                                break;
                            case "Y": /* inner B case code */
                                unlockLevel = "Year";
                                msgBody.Append("<b>Level</b> : Year <br/>");

                                i = 0;
                                foreach (var item in model.dataID)
                                {
                                    yearCode = model.dataID[i];
                                    if (i == 0)
                                    {
                                        stateName = model.StateCode > 0 ? dbContext.MASTER_STATE.Where(x => x.MAST_STATE_CODE == model.StateCode).Select(x => x.MAST_STATE_NAME).FirstOrDefault() : "All States";
                                        msgBody.Append("<b>State</b> : " + stateName + " <br/>");

                                        distName = model.DistrictCode > 0 ? dbContext.MASTER_DISTRICT.Where(x => x.MAST_DISTRICT_CODE == model.DistrictCode).Select(x => x.MAST_DISTRICT_NAME).FirstOrDefault() : "All Districts";
                                        msgBody.Append("<b>District</b> : " + distName + " <br/>");

                                        blockName = model.BlockCode > 0 ? dbContext.MASTER_BLOCK.Where(x => x.MAST_BLOCK_CODE == model.BlockCode).Select(x => x.MAST_BLOCK_NAME).FirstOrDefault() : "All Blocks";
                                        msgBody.Append("<b>Block</b> : " + blockName + " <br/>");

                                        msgBody.Append("<b>Type</b> : " + (model.sanctionType == "M" ? "MoRD Sanctioned" : "STA/PTA Scrutinized") + " <br/><br/>");

                                        msgBody.Append("<b>Remarks</b> : " + model.UnlockRemarks + " <br/><br/>");
                                        msgBody.Append(htmlTableStart + htmlHeaderRowStart);
                                        msgBody = msgBody.Append(htmlTdStart + "Sr.No" + htmlTdEnd).Append(htmlTdStart + "Year" + htmlTdEnd).Append(htmlTdStart + "Start Date" + htmlTdEnd).Append(htmlTdStart + "End Date" + htmlTdEnd).Append(htmlTdStart + "Unlocked By" + htmlTdEnd).Append(htmlTdStart + "Role" + htmlTdEnd);
                                        msgBody = msgBody.Append(htmlHeaderRowEnd);
                                    }

                                    //workName = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == prCode).Select(x => x.IMS_ROAD_NAME).FirstOrDefault();
                                    i++;
                                    msgBody = msgBody.Append(htmlTrStart);
                                    msgBody = msgBody.Append(htmlTdStart + i + htmlTdEnd).Append(htmlTdStart + yearCode + "-" + (yearCode + 1) + htmlTdEnd).Append(htmlTdStart + model.UnlockStartDate + htmlTdEnd).Append(htmlTdStart + model.UnlockEndDate + htmlTdEnd).Append(htmlTdStart + PMGSYSession.Current.UserName + htmlTdEnd).Append(htmlTdStart + roleName + htmlTdEnd);
                                    msgBody = msgBody.Append(htmlTrEnd);
                                }
                                msgBody.Append(htmlTableEnd);
                                break;
                            case "T": /* inner B case code */
                                unlockLevel = "Batch";
                                msgBody.Append("<b>Level</b> : Batch <br/>");

                                i = 0;
                                foreach (var item in model.dataID)
                                {
                                    batch = model.dataID[i];
                                    if (i == 0)
                                    {
                                        stateName = model.StateCode > 0 ? dbContext.MASTER_STATE.Where(x => x.MAST_STATE_CODE == model.StateCode).Select(x => x.MAST_STATE_NAME).FirstOrDefault() : "All States";
                                        msgBody.Append("<b>State</b> : " + stateName + " <br/>");

                                        distName = model.DistrictCode > 0 ? dbContext.MASTER_DISTRICT.Where(x => x.MAST_DISTRICT_CODE == model.DistrictCode).Select(x => x.MAST_DISTRICT_NAME).FirstOrDefault() : "All Districts";
                                        msgBody.Append("<b>District</b> : " + distName + " <br/>");

                                        blockName = model.BlockCode > 0 ? dbContext.MASTER_BLOCK.Where(x => x.MAST_BLOCK_CODE == model.BlockCode).Select(x => x.MAST_BLOCK_NAME).FirstOrDefault() : "All Blocks";
                                        msgBody.Append("<b>Block</b> : " + blockName + " <br/>");

                                        //model.YearCode = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == prCode).Select(x => x.IMS_YEAR).FirstOrDefault();
                                        yearName = model.YearCode > 0 ? dbContext.MASTER_YEAR.Where(x => x.MAST_YEAR_CODE == model.YearCode).Select(x => x.MAST_YEAR_TEXT).FirstOrDefault() : "All Years";
                                        msgBody.Append("<b>Year</b> : " + yearName + " <br/>");

                                        msgBody.Append("<b>Type</b> : " + (model.sanctionType == "M" ? "MoRD Sanctioned" : "STA/PTA Scrutinized") + " <br/><br/>");
                                        msgBody.Append("<b>Remarks</b> : " + model.UnlockRemarks + " <br/><br/>");

                                        msgBody.Append(htmlTableStart + htmlHeaderRowStart);
                                        msgBody = msgBody.Append(htmlTdStart + "Sr.No" + htmlTdEnd).Append(htmlTdStart + "Batch" + htmlTdEnd).Append(htmlTdStart + "Start Date" + htmlTdEnd).Append(htmlTdStart + "End Date" + htmlTdEnd).Append(htmlTdStart + "Unlocked By" + htmlTdEnd).Append(htmlTdStart + "Role" + htmlTdEnd);
                                        msgBody = msgBody.Append(htmlHeaderRowEnd);
                                    }
                                    //workName = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == prCode).Select(x => x.IMS_ROAD_NAME).FirstOrDefault();

                                    i++;
                                    msgBody = msgBody.Append(htmlTrStart);
                                    msgBody = msgBody.Append(htmlTdStart + i + htmlTdEnd).Append(htmlTdStart + "Batch-" + batch + htmlTdEnd).Append(htmlTdStart + model.UnlockStartDate + htmlTdEnd).Append(htmlTdStart + model.UnlockEndDate + htmlTdEnd).Append(htmlTdStart + PMGSYSession.Current.UserName + htmlTdEnd).Append(htmlTdStart + roleName + htmlTdEnd);
                                    msgBody = msgBody.Append(htmlTrEnd);
                                }
                                msgBody.Append(htmlTableEnd);
                                break;
                        }
                        unlockModule = "Proposal Habitation";
                        break;

                    case "PT"://Proposal Technology                                         // Added By to unlock Proposal Technology Details
                        msgBody.Append("<b>Module</b> : Proposal Technology <br/>");
                        if (model.PMGSYScheme == 3)
                        {
                            msgBody.Append("<b>Scheme</b> : RCPLWE <br/>");
                        }
                        else
                        {
                            msgBody.Append("<b>Scheme</b> : PMGSY-" + (model.PMGSYScheme == 4 ? 3 : model.PMGSYScheme) + " <br/>");
                        }
                        switch (model.UnlockLevel)
                        {
                            case "R":
                                unlockLevel = "Road";
                                msgBody.Append("<b>Level</b> : Road <br/>");

                                i = 0;
                                foreach (var item in model.dataID)
                                {
                                    prCode = model.dataID[i];
                                    if (i == 0)
                                    {
                                        stateName = model.StateCode > 0 ? dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == prCode).Select(x => x.MASTER_BLOCK.MASTER_DISTRICT.MASTER_STATE.MAST_STATE_NAME).FirstOrDefault() : "All States";
                                        msgBody.Append("<b>State</b> : " + stateName + " <br/>");

                                        distName = model.DistrictCode > 0 ? dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == prCode).Select(x => x.MASTER_BLOCK.MASTER_DISTRICT.MAST_DISTRICT_NAME).FirstOrDefault() : "All Districts";
                                        msgBody.Append("<b>District</b> : " + distName + " <br/>");

                                        blockName = model.BlockCode > 0 ? dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == prCode).Select(x => x.MASTER_BLOCK.MAST_BLOCK_NAME).FirstOrDefault() : "All Blocks";
                                        msgBody.Append("<b>Block</b> : " + blockName + " <br/>");

                                        var prop = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == prCode).FirstOrDefault();
                                        if (prop != null)
                                        {
                                            yearName = model.YearCode > 0 ? prop.IMS_YEAR + "-" + (prop.IMS_YEAR + 1) : "All Years";
                                            batchName = model.BatchCode > 0 ? "Batch-" + model.BatchCode : "All Batches";
                                            model.Package = (model.Package == "0" || model.Package == "-1") ? "All Packages" : prop.IMS_PACKAGE_ID;

                                            msgBody.Append("<b>Year</b> : " + yearName + " <br/>");
                                            msgBody.Append("<b>Batch</b> : " + batchName + " <br/>");
                                            msgBody.Append("<b>Package</b> : " + model.Package + " <br/>");
                                            //msgBody.Append("<b>Type</b> : " + (prop.IMS_ISCOMPLETED == "M" ? "MoRD Sanctioned" : "STA/PTA Scrutinized") + " <br/><br/>");
                                        }

                                        if (PMGSYSession.Current.RoleCode != 36 && PMGSYSession.Current.RoleCode != 47)
                                        {
                                            msgBody.Append("<b>Type</b> : " + (model.sanctionType == "M" ? "MoRD Sanctioned" : "STA/PTA Scrutinized") + " <br/><br/>");
                                        }
                                        msgBody.Append("<b>Remarks</b> : " + model.UnlockRemarks + " <br/><br/>");
                                        msgBody.Append(htmlTableStart + htmlHeaderRowStart);
                                        msgBody = msgBody.Append(htmlTdStart + "Sr.No" + htmlTdEnd).Append(htmlTdStart + "Proposal Road" + htmlTdEnd).Append(htmlTdStart + "Start Date" + htmlTdEnd).Append(htmlTdStart + "End Date" + htmlTdEnd).Append(htmlTdStart + "Unlocked By" + htmlTdEnd).Append(htmlTdStart + "Role" + htmlTdEnd);
                                        msgBody = msgBody.Append(htmlHeaderRowEnd);
                                    }

                                    workName = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == prCode).Select(x => x.IMS_ROAD_NAME).FirstOrDefault();
                                    i++;
                                    msgBody = msgBody.Append(htmlTrStart);
                                    msgBody = msgBody.Append(htmlTdStart + i + htmlTdEnd).Append(htmlTdStart + workName + htmlTdEnd).Append(htmlTdStart + model.UnlockStartDate + htmlTdEnd).Append(htmlTdStart + model.UnlockEndDate + htmlTdEnd).Append(htmlTdStart + PMGSYSession.Current.UserName + htmlTdEnd).Append(htmlTdStart + roleName + htmlTdEnd);
                                    msgBody = msgBody.Append(htmlTrEnd);
                                }
                                msgBody.Append(htmlTableEnd);
                                break;
                            case "Y": /* inner B case code */
                                unlockLevel = "Year";
                                msgBody.Append("<b>Level</b> : Year <br/>");

                                i = 0;
                                foreach (var item in model.dataID)
                                {
                                    yearCode = model.dataID[i];
                                    if (i == 0)
                                    {
                                        stateName = model.StateCode > 0 ? dbContext.MASTER_STATE.Where(x => x.MAST_STATE_CODE == model.StateCode).Select(x => x.MAST_STATE_NAME).FirstOrDefault() : "All States";
                                        msgBody.Append("<b>State</b> : " + stateName + " <br/>");

                                        distName = model.DistrictCode > 0 ? dbContext.MASTER_DISTRICT.Where(x => x.MAST_DISTRICT_CODE == model.DistrictCode).Select(x => x.MAST_DISTRICT_NAME).FirstOrDefault() : "All Districts";
                                        msgBody.Append("<b>District</b> : " + distName + " <br/>");

                                        blockName = model.BlockCode > 0 ? dbContext.MASTER_BLOCK.Where(x => x.MAST_BLOCK_CODE == model.BlockCode).Select(x => x.MAST_BLOCK_NAME).FirstOrDefault() : "All Blocks";
                                        msgBody.Append("<b>Block</b> : " + blockName + " <br/>");

                                        msgBody.Append("<b>Type</b> : " + (model.sanctionType == "M" ? "MoRD Sanctioned" : "STA/PTA Scrutinized") + " <br/><br/>");

                                        msgBody.Append("<b>Remarks</b> : " + model.UnlockRemarks + " <br/><br/>");
                                        msgBody.Append(htmlTableStart + htmlHeaderRowStart);
                                        msgBody = msgBody.Append(htmlTdStart + "Sr.No" + htmlTdEnd).Append(htmlTdStart + "Year" + htmlTdEnd).Append(htmlTdStart + "Start Date" + htmlTdEnd).Append(htmlTdStart + "End Date" + htmlTdEnd).Append(htmlTdStart + "Unlocked By" + htmlTdEnd).Append(htmlTdStart + "Role" + htmlTdEnd);
                                        msgBody = msgBody.Append(htmlHeaderRowEnd);
                                    }

                                    //workName = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == prCode).Select(x => x.IMS_ROAD_NAME).FirstOrDefault();
                                    i++;
                                    msgBody = msgBody.Append(htmlTrStart);
                                    msgBody = msgBody.Append(htmlTdStart + i + htmlTdEnd).Append(htmlTdStart + yearCode + "-" + (yearCode + 1) + htmlTdEnd).Append(htmlTdStart + model.UnlockStartDate + htmlTdEnd).Append(htmlTdStart + model.UnlockEndDate + htmlTdEnd).Append(htmlTdStart + PMGSYSession.Current.UserName + htmlTdEnd).Append(htmlTdStart + roleName + htmlTdEnd);
                                    msgBody = msgBody.Append(htmlTrEnd);
                                }
                                msgBody.Append(htmlTableEnd);
                                break;
                            case "T": /* inner B case code */
                                unlockLevel = "Batch";
                                msgBody.Append("<b>Level</b> : Batch <br/>");

                                i = 0;
                                foreach (var item in model.dataID)
                                {
                                    batch = model.dataID[i];
                                    if (i == 0)
                                    {
                                        stateName = model.StateCode > 0 ? dbContext.MASTER_STATE.Where(x => x.MAST_STATE_CODE == model.StateCode).Select(x => x.MAST_STATE_NAME).FirstOrDefault() : "All States";
                                        msgBody.Append("<b>State</b> : " + stateName + " <br/>");

                                        distName = model.DistrictCode > 0 ? dbContext.MASTER_DISTRICT.Where(x => x.MAST_DISTRICT_CODE == model.DistrictCode).Select(x => x.MAST_DISTRICT_NAME).FirstOrDefault() : "All Districts";
                                        msgBody.Append("<b>District</b> : " + distName + " <br/>");

                                        blockName = model.BlockCode > 0 ? dbContext.MASTER_BLOCK.Where(x => x.MAST_BLOCK_CODE == model.BlockCode).Select(x => x.MAST_BLOCK_NAME).FirstOrDefault() : "All Blocks";
                                        msgBody.Append("<b>Block</b> : " + blockName + " <br/>");

                                        //model.YearCode = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == prCode).Select(x => x.IMS_YEAR).FirstOrDefault();
                                        yearName = model.YearCode > 0 ? dbContext.MASTER_YEAR.Where(x => x.MAST_YEAR_CODE == model.YearCode).Select(x => x.MAST_YEAR_TEXT).FirstOrDefault() : "All Years";
                                        msgBody.Append("<b>Year</b> : " + yearName + " <br/>");

                                        msgBody.Append("<b>Type</b> : " + (model.sanctionType == "M" ? "MoRD Sanctioned" : "STA/PTA Scrutinized") + " <br/><br/>");
                                        msgBody.Append("<b>Remarks</b> : " + model.UnlockRemarks + " <br/><br/>");

                                        msgBody.Append(htmlTableStart + htmlHeaderRowStart);
                                        msgBody = msgBody.Append(htmlTdStart + "Sr.No" + htmlTdEnd).Append(htmlTdStart + "Batch" + htmlTdEnd).Append(htmlTdStart + "Start Date" + htmlTdEnd).Append(htmlTdStart + "End Date" + htmlTdEnd).Append(htmlTdStart + "Unlocked By" + htmlTdEnd).Append(htmlTdStart + "Role" + htmlTdEnd);
                                        msgBody = msgBody.Append(htmlHeaderRowEnd);
                                    }
                                    //workName = dbContext.IMS_SANCTIONED_PROJECTS.Where(x => x.IMS_PR_ROAD_CODE == prCode).Select(x => x.IMS_ROAD_NAME).FirstOrDefault();

                                    i++;
                                    msgBody = msgBody.Append(htmlTrStart);
                                    msgBody = msgBody.Append(htmlTdStart + i + htmlTdEnd).Append(htmlTdStart + "Batch-" + batch + htmlTdEnd).Append(htmlTdStart + model.UnlockStartDate + htmlTdEnd).Append(htmlTdStart + model.UnlockEndDate + htmlTdEnd).Append(htmlTdStart + PMGSYSession.Current.UserName + htmlTdEnd).Append(htmlTdStart + roleName + htmlTdEnd);
                                    msgBody = msgBody.Append(htmlTrEnd);
                                }
                                msgBody.Append(htmlTableEnd);
                                break;
                        }
                        unlockModule = "Proposal Technology";
                        break;

                        //default:
                        //    break;
                }

                msgBody.Append("<br/> This is for your kind information. <br/>");
                msgBody.Append("With Regards, <br/>");
                msgBody.Append("OMMAS Team. <br/><br/>");
                msgBody.Append("<b>Note:</b> This is a system generated mail. Please do not reply back to this email ID. <br/><br/>");
                msgBody.Append("<b><u>CONFIDENTIALITY INFORMATION AND DISCLAIMER</u></b> <br/>");
                msgBody.Append("This email message and its attachments may contain confidential, proprietary or legally privileged information and is intended solely for the use of the " +
                               " individual or entity to whom it is addressed. If you have erroneously received this message, please delete it immediately and notify the sender. If " +
                               " you are not the intended recipient of the email message you should not disseminate, distribute or copy this e-mail. E-mail transmission cannot be guaranteed " +
                               " to be secure or error-free as information could be intercepted, corrupted, lost, destroyed, incomplete or contain viruses and the OMMAS team " +
                               " accepts no liability for any damage caused by the limitations of the e-mail transmission.");

                // Added by Srishti on 14-03-2023
                ServicePointManager.ServerCertificateValidationCallback += (s, ce, ca, p) => true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                MailMessage msg = new MailMessage();
                //msg.Attachments.Add(new Attachment(stream, MediaTypeNames.Application.Octet));

                // live
                msg.From = new MailAddress("omms.pmgsy@nic.in");

                // local
                //msg.From = new MailAddress("user@cdac.in");

                if (!(model.UnlockLevel == "R" && model.UnlockTable == "PR" && (PMGSYSession.Current.RoleCode == 36 || PMGSYSession.Current.RoleCode == 47 || PMGSYSession.Current.RoleCode == 56)))
                {
                    // Live
                    msg.To.Add(ConfigurationManager.AppSettings["UnlockMailTo"].ToString());

                    // Local
                    //msg.To.Add(("user@cdac.in").ToString());
                }
                           
                msg.CC.Add(ConfigurationManager.AppSettings["UnlockMailCC1"].ToString());
                msg.CC.Add(ConfigurationManager.AppSettings["UnlockMailCC2"].ToString());

                msg.IsBodyHtml = true;
                msg.Body = msgBody.ToString();
                msg.Subject = "Ommas Unlock details";
                msg.Priority = MailPriority.High;

                SmtpClient client = new SmtpClient();

                // Added by Srishti
                string e_EuthHost = ConfigurationManager.AppSettings["e_EuthHost"];
                string e_EuthPort = ConfigurationManager.AppSettings["e_EuthPort"];
                string e_EuthMailUserName = ConfigurationManager.AppSettings["e_EuthMailUserName"];
                string e_EuthMailPassword = ConfigurationManager.AppSettings["e_EuthMailPassword"];

                client.Host = e_EuthHost;
                client.Port = Convert.ToInt32(e_EuthPort);
                client.UseDefaultCredentials = false;
                client.EnableSsl = true; // Change to true
                client.Credentials = new NetworkCredential(e_EuthMailUserName, e_EuthMailPassword);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;

                ///Live
                // Commented by Srishti
                //client.Host = "relay.nic.in";
                //client.Port = 25;
                //client.UseDefaultCredentials = true;
                //client.Credentials = new NetworkCredential("omms.pmgsy@nic.in", "Ommas@@321");

                ///Local
                //client.Host = "smtp.cdac.in"; //"smtp.gmail.in"; 
                //client.Port = 25;//587;
                //client.UseDefaultCredentials = true;
                //client.Credentials = new NetworkCredential("sammedp@cdac.in", "Sammed@123#");
                //client.Credentials = new NetworkCredential("rvoteadmin@cdac.in", "3UQpfwk7");
                //client.Credentials = new NetworkCredential("sammed.work", "sammedwork@123");
                //client.Credentials = new NetworkCredential("user@cdac.in", "Cdac@1234");

                //client.EnableSsl = true;

                client.Send(msg);

                return true;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "sendMail().DAL");
                return false;
            }
        }


        public bool CheckActiveStatus(string unlockTable, string unlockLevel, string unlockstartDate, string unlockendDate, int id, ref string message, string unlockBy)
        {
            dbContext = new PMGSYEntities();
            CommonFunctions objCommon = new CommonFunctions();
            DateTime startDate = objCommon.GetStringToDateTime(unlockstartDate);
            DateTime endDate = objCommon.GetStringToDateTime(unlockendDate);
            try
            {
                switch (unlockTable)
                {
                    case "PR":
                        IMS_SANCTIONED_PROJECTS imsMaster = dbContext.IMS_SANCTIONED_PROJECTS.Find(id);
                        switch (unlockLevel)
                        {
                            case "R":
                                List<IMS_UNLOCK_DETAILS> lstRoadLevel = dbContext.IMS_UNLOCK_DETAILS.Where(m => m.IMS_UNLOCK_START_DATE <= startDate && m.IMS_UNLOCK_END_DATE >= endDate && (m.IMS_UNLOCK_LEVEL == "D" || m.IMS_UNLOCK_LEVEL == "S" || m.IMS_UNLOCK_LEVEL == "B") && m.IMS_UNLOCK_TABLE == unlockTable && m.IMS_UNLOCK_STATUS == "Y" && m.IMS_UNLOCK_BY == unlockBy).ToList();
                                if (lstRoadLevel.Any(m => m.IMS_UNLOCK_LEVEL == "S" && m.MAST_STATE_CODE == imsMaster.MAST_STATE_CODE))
                                {
                                    message = "State of this road is already unlocked.";
                                    return false;
                                }
                                else if (lstRoadLevel.Any(m => m.IMS_UNLOCK_LEVEL == "D" && m.MAST_DISTRICT_CODE == imsMaster.MAST_DISTRICT_CODE))
                                {
                                    message = "District of this road is already unlocked.";
                                    return false;
                                }
                                else if (lstRoadLevel.Any(m => m.IMS_UNLOCK_LEVEL == "B" && m.MAST_BLOCK_CODE == imsMaster.MAST_BLOCK_CODE))
                                {
                                    message = "Block of this road is already unlocked.";
                                    return false;
                                }
                                else if (dbContext.IMS_UNLOCK_DETAILS.Any(m => m.IMS_UNLOCK_LEVEL == "R" && m.IMS_PR_ROAD_CODE == id && m.IMS_UNLOCK_START_DATE == startDate && m.IMS_UNLOCK_END_DATE == endDate && m.IMS_UNLOCK_STATUS == "Y" && m.IMS_UNLOCK_TABLE == unlockTable))
                                {
                                    message = "Road is already unlocked.";
                                    return false;
                                }
                                break;
                            case "B":
                                List<IMS_UNLOCK_DETAILS> lstBlockLevel = dbContext.IMS_UNLOCK_DETAILS.Where(m => m.IMS_UNLOCK_START_DATE <= startDate && m.IMS_UNLOCK_END_DATE >= endDate && (m.IMS_UNLOCK_LEVEL == "D" || m.IMS_UNLOCK_LEVEL == "S" || m.IMS_UNLOCK_LEVEL == "B") && m.IMS_UNLOCK_TABLE == unlockTable && m.IMS_UNLOCK_STATUS == "Y" && m.IMS_UNLOCK_BY == unlockBy).ToList();
                                int districtCode = dbContext.MASTER_BLOCK.Where(m => m.MAST_BLOCK_CODE == id).Select(m => m.MAST_DISTRICT_CODE).FirstOrDefault();
                                int stateCode = dbContext.MASTER_DISTRICT.Where(m => m.MAST_DISTRICT_CODE == districtCode).Select(m => m.MAST_STATE_CODE).FirstOrDefault();
                                if (lstBlockLevel.Any(m => m.IMS_UNLOCK_LEVEL == "S" && m.MAST_STATE_CODE == stateCode))
                                {
                                    message = "State of this block is already unlocked.";
                                    return false;
                                }
                                else if (lstBlockLevel.Any(m => m.IMS_UNLOCK_LEVEL == "D" && m.MAST_DISTRICT_CODE == districtCode))
                                {
                                    message = "District of this block is already unlocked.";
                                    return false;
                                }
                                else if (dbContext.IMS_UNLOCK_DETAILS.Any(m => m.IMS_UNLOCK_LEVEL == "B" && m.MAST_BLOCK_CODE == id && m.IMS_UNLOCK_START_DATE == startDate && m.IMS_UNLOCK_END_DATE == endDate && m.IMS_UNLOCK_STATUS == "Y" && m.IMS_UNLOCK_TABLE == unlockTable))
                                {
                                    message = "Block is already unlocked.";
                                    return false;
                                }
                                break;
                            case "D":
                                stateCode = dbContext.MASTER_DISTRICT.Where(m => m.MAST_DISTRICT_CODE == id).Select(m => m.MAST_STATE_CODE).FirstOrDefault();
                                List<IMS_UNLOCK_DETAILS> lstDistrictLevel = dbContext.IMS_UNLOCK_DETAILS.Where(m => m.IMS_UNLOCK_START_DATE <= startDate && m.IMS_UNLOCK_END_DATE >= endDate && (m.IMS_UNLOCK_LEVEL == "D" || m.IMS_UNLOCK_LEVEL == "S" || m.IMS_UNLOCK_LEVEL == "B") && m.IMS_UNLOCK_TABLE == unlockTable && m.IMS_UNLOCK_STATUS == "Y" && m.IMS_UNLOCK_BY == unlockBy).ToList();
                                if (lstDistrictLevel.Any(m => m.IMS_UNLOCK_LEVEL == "S" && m.MAST_STATE_CODE == stateCode))
                                {
                                    message = "State of this district is already unlocked.";
                                    return false;
                                }
                                else if (dbContext.IMS_UNLOCK_DETAILS.Any(m => m.IMS_UNLOCK_LEVEL == "D" && m.MAST_DISTRICT_CODE == id && m.IMS_UNLOCK_START_DATE == startDate && m.IMS_UNLOCK_END_DATE == endDate && m.IMS_UNLOCK_STATUS == "Y"))
                                {
                                    message = "The district is already unlocked.";
                                    return false;
                                }
                                break;
                            case "S":
                                if (dbContext.IMS_UNLOCK_DETAILS.Any(m => m.IMS_UNLOCK_LEVEL == "S" && m.MAST_STATE_CODE == id && m.IMS_UNLOCK_START_DATE == startDate && m.IMS_UNLOCK_END_DATE == endDate && m.IMS_UNLOCK_STATUS == "Y" && m.IMS_UNLOCK_TABLE == unlockTable))
                                {
                                    message = "State is already unlocked.";
                                    return false;
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    case "ER":

                        MASTER_EXISTING_ROADS existingRoad = dbContext.MASTER_EXISTING_ROADS.Find(id);
                        switch (unlockLevel)
                        {
                            case "R":
                                List<IMS_UNLOCK_DETAILS> lstRoadLevel = dbContext.IMS_UNLOCK_DETAILS.Where(m => m.IMS_UNLOCK_START_DATE <= startDate && m.IMS_UNLOCK_END_DATE >= endDate && (m.IMS_UNLOCK_LEVEL == "D" || m.IMS_UNLOCK_LEVEL == "S" || m.IMS_UNLOCK_LEVEL == "B") && m.IMS_UNLOCK_TABLE == unlockTable && m.IMS_UNLOCK_STATUS == "Y" && m.IMS_UNLOCK_BY == unlockBy).ToList();
                                if (lstRoadLevel.Any(m => m.IMS_UNLOCK_LEVEL == "S" && m.MAST_STATE_CODE == existingRoad.MAST_STATE_CODE))
                                {
                                    message = "State of this road is already unlocked.";
                                    return false;
                                }
                                else if (lstRoadLevel.Any(m => m.IMS_UNLOCK_LEVEL == "D" && m.MAST_DISTRICT_CODE == existingRoad.MAST_DISTRICT_CODE))
                                {
                                    message = "District of this road is already unlocked.";
                                    return false;
                                }
                                else if (lstRoadLevel.Any(m => m.IMS_UNLOCK_LEVEL == "B" && m.MAST_BLOCK_CODE == existingRoad.MAST_BLOCK_CODE))
                                {
                                    message = "Block of this road is already unlocked.";
                                    return false;
                                }
                                else if (dbContext.IMS_UNLOCK_DETAILS.Any(m => m.IMS_UNLOCK_LEVEL == "R" && m.MAST_ER_ROAD_CODE == id && m.IMS_UNLOCK_START_DATE == startDate && m.IMS_UNLOCK_END_DATE == endDate && m.IMS_UNLOCK_STATUS == "Y" && m.IMS_UNLOCK_TABLE == unlockTable))
                                {
                                    message = "Road is already unlocked.";
                                    return false;
                                }

                                break;
                            case "B":
                                List<IMS_UNLOCK_DETAILS> lstBlockLevel = dbContext.IMS_UNLOCK_DETAILS.Where(m => m.IMS_UNLOCK_START_DATE <= startDate && m.IMS_UNLOCK_END_DATE >= endDate && (m.IMS_UNLOCK_LEVEL == "D" || m.IMS_UNLOCK_LEVEL == "S" || m.IMS_UNLOCK_LEVEL == "B") && m.IMS_UNLOCK_TABLE == unlockTable && m.IMS_UNLOCK_STATUS == "Y" && m.IMS_UNLOCK_BY == unlockBy).ToList();
                                int districtCode = dbContext.MASTER_BLOCK.Where(m => m.MAST_BLOCK_CODE == id).Select(m => m.MAST_DISTRICT_CODE).FirstOrDefault();
                                int stateCode = dbContext.MASTER_DISTRICT.Where(m => m.MAST_DISTRICT_CODE == districtCode).Select(m => m.MAST_STATE_CODE).FirstOrDefault();
                                if (lstBlockLevel.Any(m => m.IMS_UNLOCK_LEVEL == "S" && m.MAST_STATE_CODE == stateCode))
                                {
                                    message = "State of this road is already unlocked.";
                                    return false;
                                }
                                else if (lstBlockLevel.Any(m => m.IMS_UNLOCK_LEVEL == "D" && m.MAST_DISTRICT_CODE == districtCode))
                                {
                                    message = "District of this block is already unlocked.";
                                    return false;
                                }
                                else if (dbContext.IMS_UNLOCK_DETAILS.Any(m => m.IMS_UNLOCK_LEVEL == "B" && m.MAST_BLOCK_CODE == id && m.IMS_UNLOCK_START_DATE == startDate && m.IMS_UNLOCK_END_DATE == endDate && m.IMS_UNLOCK_STATUS == "Y" && m.IMS_UNLOCK_TABLE == unlockTable))
                                {
                                    message = "Block is already unlocked.";
                                    return false;
                                }
                                break;
                            case "D":
                                List<IMS_UNLOCK_DETAILS> lstDistrictLevel = dbContext.IMS_UNLOCK_DETAILS.Where(m => m.IMS_UNLOCK_START_DATE <= startDate && m.IMS_UNLOCK_END_DATE >= endDate && (m.IMS_UNLOCK_LEVEL == "D" || m.IMS_UNLOCK_LEVEL == "S" || m.IMS_UNLOCK_LEVEL == "B") && m.IMS_UNLOCK_TABLE == unlockTable && m.IMS_UNLOCK_STATUS == "Y" && m.IMS_UNLOCK_BY == unlockBy).ToList();
                                stateCode = dbContext.MASTER_DISTRICT.Where(m => m.MAST_DISTRICT_CODE == id).Select(m => m.MAST_STATE_CODE).FirstOrDefault();
                                if (lstDistrictLevel.Any(m => m.IMS_UNLOCK_LEVEL == "S" && m.MAST_STATE_CODE == stateCode))
                                {
                                    message = "State of this district is already unlocked.";
                                    return false;
                                }
                                else if (dbContext.IMS_UNLOCK_DETAILS.Any(m => m.IMS_UNLOCK_LEVEL == "D" && m.MAST_DISTRICT_CODE == id && m.IMS_UNLOCK_START_DATE == startDate && m.IMS_UNLOCK_END_DATE == endDate && m.IMS_UNLOCK_STATUS == "Y"))
                                {
                                    message = "District is already unlocked.";
                                    return false;
                                }
                                break;
                            case "S":
                                if (dbContext.IMS_UNLOCK_DETAILS.Any(m => m.IMS_UNLOCK_LEVEL == "S" && m.MAST_STATE_CODE == id && m.IMS_UNLOCK_START_DATE == startDate && m.IMS_UNLOCK_END_DATE == endDate && m.IMS_UNLOCK_STATUS == "Y" && m.IMS_UNLOCK_BY == unlockBy && m.IMS_UNLOCK_TABLE == unlockTable))
                                {
                                    message = "State is already unlocked.";
                                    return false;
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    case "CN":
                        PLAN_ROAD corenetworkMaster = dbContext.PLAN_ROAD.Find(id);
                        switch (unlockLevel)
                        {
                            case "R":
                                List<IMS_UNLOCK_DETAILS> lstRoadLevel = dbContext.IMS_UNLOCK_DETAILS.Where(m => m.IMS_UNLOCK_START_DATE <= startDate && m.IMS_UNLOCK_END_DATE >= endDate && (m.IMS_UNLOCK_LEVEL == "D" || m.IMS_UNLOCK_LEVEL == "S" || m.IMS_UNLOCK_LEVEL == "B") && m.IMS_UNLOCK_TABLE == unlockTable && m.IMS_UNLOCK_STATUS == "Y" && m.IMS_UNLOCK_BY == unlockBy && m.IMS_UNLOCK_STATUS == "Y").ToList();
                                if (lstRoadLevel.Any(m => m.IMS_UNLOCK_LEVEL == "S" && m.MAST_STATE_CODE == corenetworkMaster.MAST_STATE_CODE))
                                {
                                    message = "State of this road is already unlocked.";
                                    return false;
                                }
                                else if (lstRoadLevel.Any(m => m.IMS_UNLOCK_LEVEL == "D" && m.MAST_DISTRICT_CODE == corenetworkMaster.MAST_DISTRICT_CODE))
                                {
                                    message = "District of this road is already unlocked.";
                                    return false;
                                }
                                else if (lstRoadLevel.Any(m => m.IMS_UNLOCK_LEVEL == "B" && m.MAST_BLOCK_CODE == corenetworkMaster.MAST_BLOCK_CODE))
                                {
                                    message = "Block of this road is already unlocked.";
                                    return false;
                                }
                                else if (dbContext.IMS_UNLOCK_DETAILS.Any(m => m.IMS_UNLOCK_LEVEL == "R" && m.PLAN_CN_ROAD_CODE == id && m.IMS_UNLOCK_START_DATE == startDate && m.IMS_UNLOCK_END_DATE == endDate && m.IMS_UNLOCK_STATUS == "Y" && m.IMS_UNLOCK_TABLE == unlockTable))
                                {
                                    message = "Road is already unlocked.";
                                    return false;
                                }
                                break;
                            case "B":
                                List<IMS_UNLOCK_DETAILS> lstBlockLevel = dbContext.IMS_UNLOCK_DETAILS.Where(m => m.IMS_UNLOCK_START_DATE <= startDate && m.IMS_UNLOCK_END_DATE >= endDate && (m.IMS_UNLOCK_LEVEL == "D" || m.IMS_UNLOCK_LEVEL == "S" || m.IMS_UNLOCK_LEVEL == "B") && m.IMS_UNLOCK_TABLE == unlockTable && m.IMS_UNLOCK_STATUS == "Y" && m.IMS_UNLOCK_BY == unlockBy).ToList();
                                int districtCode = dbContext.MASTER_BLOCK.Where(m => m.MAST_BLOCK_CODE == id).Select(m => m.MAST_DISTRICT_CODE).FirstOrDefault();
                                int stateCode = dbContext.MASTER_DISTRICT.Where(m => m.MAST_DISTRICT_CODE == districtCode).Select(m => m.MAST_STATE_CODE).FirstOrDefault();
                                if (lstBlockLevel.Any(m => m.IMS_UNLOCK_LEVEL == "S" && m.MAST_STATE_CODE == stateCode))
                                {
                                    message = "State of this block is already unlocked.";
                                    return false;
                                }
                                else if (lstBlockLevel.Any(m => m.IMS_UNLOCK_LEVEL == "D" && m.MAST_DISTRICT_CODE == districtCode))
                                {
                                    message = "District of this block is already unlocked.";
                                    return false;
                                }
                                else if (dbContext.IMS_UNLOCK_DETAILS.Any(m => m.IMS_UNLOCK_LEVEL == "B" && m.MAST_BLOCK_CODE == id && m.IMS_UNLOCK_START_DATE == startDate && m.IMS_UNLOCK_END_DATE == endDate && m.IMS_UNLOCK_STATUS == "Y" && m.IMS_UNLOCK_TABLE == unlockTable))
                                {
                                    message = "Block is already unlocked.";
                                    return false;
                                }
                                break;
                            case "D":
                                List<IMS_UNLOCK_DETAILS> lstDistrictLevel = dbContext.IMS_UNLOCK_DETAILS.Where(m => m.IMS_UNLOCK_START_DATE <= startDate && m.IMS_UNLOCK_END_DATE >= endDate && (m.IMS_UNLOCK_LEVEL == "D" || m.IMS_UNLOCK_LEVEL == "S" || m.IMS_UNLOCK_LEVEL == "B") && m.IMS_UNLOCK_TABLE == unlockTable && m.IMS_UNLOCK_STATUS == "Y" && m.IMS_UNLOCK_BY == unlockBy).ToList();
                                stateCode = dbContext.MASTER_DISTRICT.Where(m => m.MAST_DISTRICT_CODE == id).Select(m => m.MAST_STATE_CODE).FirstOrDefault();
                                if (lstDistrictLevel.Any(m => m.IMS_UNLOCK_LEVEL == "S" && m.MAST_STATE_CODE == stateCode))
                                {
                                    message = "State of this district is already unlocked.";
                                    return false;
                                }
                                else if (dbContext.IMS_UNLOCK_DETAILS.Any(m => m.IMS_UNLOCK_LEVEL == "D" && m.MAST_DISTRICT_CODE == id && m.IMS_UNLOCK_START_DATE == startDate && m.IMS_UNLOCK_END_DATE == endDate && m.IMS_UNLOCK_STATUS == "Y"))
                                {
                                    message = "District is already unlocked.";
                                    return false;
                                }
                                break;
                            case "S":
                                if (dbContext.IMS_UNLOCK_DETAILS.Any(m => m.IMS_UNLOCK_LEVEL == "S" && m.MAST_STATE_CODE == id && m.IMS_UNLOCK_START_DATE == startDate && m.IMS_UNLOCK_END_DATE == endDate && m.IMS_UNLOCK_STATUS == "Y" && m.IMS_UNLOCK_TABLE == unlockTable))
                                {
                                    message = "State is already unlocked.";
                                    return false;
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    case "HM":
                        MASTER_HABITATIONS habMaster = dbContext.MASTER_HABITATIONS.Find(id);

                        switch (unlockLevel)
                        {
                            case "H":
                                int blockCode = dbContext.MASTER_VILLAGE.Where(m => m.MAST_VILLAGE_CODE == habMaster.MAST_VILLAGE_CODE).Select(m => m.MAST_BLOCK_CODE).FirstOrDefault();
                                int district = dbContext.MASTER_BLOCK.Where(m => m.MAST_BLOCK_CODE == blockCode).Select(m => m.MAST_DISTRICT_CODE).FirstOrDefault();
                                int state = dbContext.MASTER_DISTRICT.Where(m => m.MAST_DISTRICT_CODE == district).Select(m => m.MAST_STATE_CODE).FirstOrDefault();
                                List<IMS_UNLOCK_DETAILS> lstHabLevel = dbContext.IMS_UNLOCK_DETAILS.Where(m => m.IMS_UNLOCK_START_DATE <= startDate && m.IMS_UNLOCK_END_DATE >= endDate && (m.IMS_UNLOCK_LEVEL == "D" || m.IMS_UNLOCK_LEVEL == "S" || m.IMS_UNLOCK_LEVEL == "B" || m.IMS_UNLOCK_LEVEL == "V") && m.IMS_UNLOCK_TABLE == unlockTable && m.IMS_UNLOCK_STATUS == "Y" && m.IMS_UNLOCK_BY == unlockBy).ToList();
                                if (lstHabLevel.Any(m => m.IMS_UNLOCK_LEVEL == "S" && m.MAST_STATE_CODE == state))
                                {
                                    message = "State of this habitation is already unlocked.";
                                    return false;
                                }
                                else if (lstHabLevel.Any(m => m.IMS_UNLOCK_LEVEL == "D" && m.MAST_DISTRICT_CODE == district))
                                {
                                    message = "District of this habitation is already unlocked.";
                                    return false;
                                }
                                else if (lstHabLevel.Any(m => m.IMS_UNLOCK_LEVEL == "B" && m.MAST_BLOCK_CODE == blockCode))
                                {
                                    message = "Block of this habitation is already unlocked.";
                                    return false;
                                }
                                else if (lstHabLevel.Any(m => m.IMS_UNLOCK_LEVEL == "V" && m.MAST_VILLAGE_CODE == habMaster.MAST_VILLAGE_CODE))
                                {
                                    message = "Village of this habitation is already unlocked.";
                                    return false;
                                }
                                else if (dbContext.IMS_UNLOCK_DETAILS.Any(m => m.IMS_UNLOCK_LEVEL == "H" && m.MAST_HAB_CODE == id && m.IMS_UNLOCK_START_DATE == startDate && m.IMS_UNLOCK_END_DATE == endDate && m.IMS_UNLOCK_STATUS == "Y" && m.IMS_UNLOCK_TABLE == unlockTable))
                                {
                                    message = "Habitation is already unlocked.";
                                    return false;
                                }
                                break;
                            case "V":
                                blockCode = dbContext.MASTER_VILLAGE.Where(m => m.MAST_VILLAGE_CODE == id).Select(m => m.MAST_BLOCK_CODE).FirstOrDefault();
                                district = dbContext.MASTER_BLOCK.Where(m => m.MAST_BLOCK_CODE == blockCode).Select(m => m.MAST_DISTRICT_CODE).FirstOrDefault();
                                state = dbContext.MASTER_DISTRICT.Where(m => m.MAST_DISTRICT_CODE == district).Select(m => m.MAST_STATE_CODE).FirstOrDefault();
                                List<IMS_UNLOCK_DETAILS> lstVillageLevel = dbContext.IMS_UNLOCK_DETAILS.Where(m => m.IMS_UNLOCK_START_DATE <= startDate && m.IMS_UNLOCK_END_DATE >= endDate && (m.IMS_UNLOCK_LEVEL == "D" || m.IMS_UNLOCK_LEVEL == "S" || m.IMS_UNLOCK_LEVEL == "B") && m.IMS_UNLOCK_TABLE == unlockTable && m.IMS_UNLOCK_STATUS == "Y" && m.IMS_UNLOCK_BY == unlockBy).ToList();
                                if (lstVillageLevel.Any(m => m.IMS_UNLOCK_LEVEL == "S" && m.MAST_STATE_CODE == state))
                                {
                                    message = "State of this village is already unlocked.";
                                    return false;
                                }
                                else if (lstVillageLevel.Any(m => m.IMS_UNLOCK_LEVEL == "D" && m.MAST_DISTRICT_CODE == district))
                                {
                                    message = "District of this village is already unlocked.";
                                    return false;
                                }
                                else if (lstVillageLevel.Any(m => m.IMS_UNLOCK_LEVEL == "B" && m.MAST_BLOCK_CODE == blockCode))
                                {
                                    message = "District of this road is already unlocked.";
                                    return false;
                                }
                                else if (dbContext.IMS_UNLOCK_DETAILS.Any(m => m.IMS_UNLOCK_LEVEL == "V" && m.MAST_VILLAGE_CODE == id && m.IMS_UNLOCK_START_DATE == startDate && m.IMS_UNLOCK_END_DATE == endDate && m.IMS_UNLOCK_STATUS == "Y" && m.IMS_UNLOCK_TABLE == unlockTable))
                                {
                                    message = "Village is already unlocked.";
                                    return false;
                                }
                                break;
                            case "B":
                                district = dbContext.MASTER_BLOCK.Where(m => m.MAST_BLOCK_CODE == id).Select(m => m.MAST_DISTRICT_CODE).FirstOrDefault();
                                state = dbContext.MASTER_DISTRICT.Where(m => m.MAST_DISTRICT_CODE == district).Select(m => m.MAST_STATE_CODE).FirstOrDefault();
                                List<IMS_UNLOCK_DETAILS> lstBlockLevel = dbContext.IMS_UNLOCK_DETAILS.Where(m => m.IMS_UNLOCK_START_DATE <= startDate && m.IMS_UNLOCK_END_DATE >= endDate && (m.IMS_UNLOCK_LEVEL == "D" || m.IMS_UNLOCK_LEVEL == "S" || m.IMS_UNLOCK_LEVEL == "B") && m.IMS_UNLOCK_TABLE == unlockTable && m.IMS_UNLOCK_STATUS == "Y" && m.IMS_UNLOCK_BY == unlockBy).ToList();
                                if (lstBlockLevel.Any(m => m.IMS_UNLOCK_LEVEL == "S" && m.MAST_STATE_CODE == state))
                                {
                                    message = "State of this block is already unlocked.";
                                    return false;
                                }
                                else if (lstBlockLevel.Any(m => m.IMS_UNLOCK_LEVEL == "D" && m.MAST_DISTRICT_CODE == district))
                                {
                                    message = "District of this block is already unlocked.";
                                    return false;
                                }
                                else if (dbContext.IMS_UNLOCK_DETAILS.Any(m => m.IMS_UNLOCK_LEVEL == "B" && m.MAST_BLOCK_CODE == id && m.IMS_UNLOCK_START_DATE == startDate && m.IMS_UNLOCK_END_DATE == endDate && m.IMS_UNLOCK_STATUS == "Y" && m.IMS_UNLOCK_TABLE == unlockTable))
                                {
                                    message = "Block is already unlocked.";
                                    return false;
                                }
                                break;
                            case "D":
                                state = dbContext.MASTER_DISTRICT.Where(m => m.MAST_DISTRICT_CODE == id).Select(m => m.MAST_STATE_CODE).FirstOrDefault();
                                List<IMS_UNLOCK_DETAILS> lstDistrictLevel = dbContext.IMS_UNLOCK_DETAILS.Where(m => m.IMS_UNLOCK_START_DATE <= startDate && m.IMS_UNLOCK_END_DATE >= endDate && (m.IMS_UNLOCK_LEVEL == "D" || m.IMS_UNLOCK_LEVEL == "S" || m.IMS_UNLOCK_LEVEL == "B") && m.IMS_UNLOCK_TABLE == unlockTable && m.IMS_UNLOCK_STATUS == "Y" && m.IMS_UNLOCK_BY == unlockBy).ToList();
                                if (lstDistrictLevel.Any(m => m.IMS_UNLOCK_LEVEL == "S" && m.MAST_STATE_CODE == state))
                                {
                                    message = "State of this district is already unlocked.";
                                    return false;
                                }
                                else if (dbContext.IMS_UNLOCK_DETAILS.Any(m => m.IMS_UNLOCK_LEVEL == "D" && m.MAST_DISTRICT_CODE == id && m.IMS_UNLOCK_START_DATE == startDate && m.IMS_UNLOCK_END_DATE == endDate && m.IMS_UNLOCK_STATUS == "Y"))
                                {
                                    message = "District is already unlocked.";
                                    return false;
                                }
                                break;
                            case "S":
                                if (dbContext.IMS_UNLOCK_DETAILS.Any(m => m.IMS_UNLOCK_LEVEL == "S" && m.MAST_STATE_CODE == id && m.IMS_UNLOCK_START_DATE == startDate && m.IMS_UNLOCK_END_DATE == endDate && m.IMS_UNLOCK_STATUS == "Y" && m.IMS_UNLOCK_BY == unlockBy && m.IMS_UNLOCK_TABLE == unlockTable))
                                {
                                    message = "State is already unlocked.";
                                    return false;
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
                return true;
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

        public bool CheckAlreadyActiveLevel(string unlockTable, string unlockLevel, string unlockstartDate, string unlockendDate, int id)
        {
            dbContext = new PMGSYEntities();
            CommonFunctions objCommon = new CommonFunctions();
            DateTime startDate = objCommon.GetStringToDateTime(unlockstartDate);
            DateTime endDate = objCommon.GetStringToDateTime(unlockendDate);
            try
            {
                switch (unlockTable)
                {
                    case "PR":
                        IMS_SANCTIONED_PROJECTS imsMaster = dbContext.IMS_SANCTIONED_PROJECTS.Find(id);
                        switch (unlockLevel)
                        {
                            case "R":
                                List<IMS_UNLOCK_DETAILS> lstRoadLevel = dbContext.IMS_UNLOCK_DETAILS.Where(m => m.IMS_UNLOCK_START_DATE <= startDate && m.IMS_UNLOCK_END_DATE >= endDate && (m.IMS_UNLOCK_LEVEL == "D" || m.IMS_UNLOCK_LEVEL == "S" || m.IMS_UNLOCK_LEVEL == "B") && m.IMS_UNLOCK_TABLE == unlockTable).ToList();
                                if (lstRoadLevel.Any(m => m.IMS_UNLOCK_LEVEL == "S" && m.MAST_STATE_CODE == imsMaster.MAST_STATE_CODE))
                                {
                                    return false;
                                }
                                else if (lstRoadLevel.Any(m => m.IMS_UNLOCK_LEVEL == "D" && m.MAST_DISTRICT_CODE == imsMaster.MAST_DISTRICT_CODE))
                                {
                                    return false;
                                }
                                else if (lstRoadLevel.Any(m => m.IMS_UNLOCK_LEVEL == "B" && m.MAST_BLOCK_CODE == imsMaster.MAST_BLOCK_CODE))
                                {
                                    return false;
                                }
                                break;
                            case "B":
                                List<IMS_UNLOCK_DETAILS> lstBlockLevel = dbContext.IMS_UNLOCK_DETAILS.Where(m => m.IMS_UNLOCK_START_DATE <= startDate && m.IMS_UNLOCK_END_DATE >= endDate && (m.IMS_UNLOCK_LEVEL == "D" || m.IMS_UNLOCK_LEVEL == "S" || m.IMS_UNLOCK_LEVEL == "B") && m.IMS_UNLOCK_TABLE == unlockTable).ToList();
                                int districtCode = dbContext.MASTER_BLOCK.Where(m => m.MAST_BLOCK_CODE == id).Select(m => m.MAST_DISTRICT_CODE).FirstOrDefault();
                                int stateCode = dbContext.MASTER_DISTRICT.Where(m => m.MAST_DISTRICT_CODE == districtCode).Select(m => m.MAST_STATE_CODE).FirstOrDefault();
                                if (lstBlockLevel.Any(m => m.IMS_UNLOCK_LEVEL == "S" && m.MAST_STATE_CODE == stateCode))
                                {
                                    return false;
                                }
                                else if (lstBlockLevel.Any(m => m.IMS_UNLOCK_LEVEL == "D" && m.MAST_DISTRICT_CODE == districtCode))
                                {
                                    return false;
                                }
                                break;
                            case "D":
                                stateCode = dbContext.MASTER_DISTRICT.Where(m => m.MAST_DISTRICT_CODE == id).Select(m => m.MAST_STATE_CODE).FirstOrDefault();
                                List<IMS_UNLOCK_DETAILS> lstDistrictLevel = dbContext.IMS_UNLOCK_DETAILS.Where(m => m.IMS_UNLOCK_START_DATE <= startDate && m.IMS_UNLOCK_END_DATE >= endDate && (m.IMS_UNLOCK_LEVEL == "D" || m.IMS_UNLOCK_LEVEL == "S" || m.IMS_UNLOCK_LEVEL == "B") && m.IMS_UNLOCK_TABLE == unlockTable).ToList();
                                if (lstDistrictLevel.Any(m => m.IMS_UNLOCK_LEVEL == "S" && m.MAST_STATE_CODE == stateCode))
                                {
                                    return false;
                                }
                                break;
                            case "S":
                                break;
                            default:
                                break;
                        }
                        break;
                    case "ER":

                        MASTER_EXISTING_ROADS existingRoad = dbContext.MASTER_EXISTING_ROADS.Find(id);
                        switch (unlockLevel)
                        {
                            case "R":
                                List<IMS_UNLOCK_DETAILS> lstRoadLevel = dbContext.IMS_UNLOCK_DETAILS.Where(m => m.IMS_UNLOCK_START_DATE <= startDate && m.IMS_UNLOCK_END_DATE >= endDate && (m.IMS_UNLOCK_LEVEL == "D" || m.IMS_UNLOCK_LEVEL == "S" || m.IMS_UNLOCK_LEVEL == "B") && m.IMS_UNLOCK_TABLE == unlockTable).ToList();
                                if (lstRoadLevel.Any(m => m.IMS_UNLOCK_LEVEL == "S" && m.MAST_STATE_CODE == existingRoad.MAST_STATE_CODE))
                                {
                                    return false;
                                }
                                else if (lstRoadLevel.Any(m => m.IMS_UNLOCK_LEVEL == "D" && m.MAST_DISTRICT_CODE == existingRoad.MAST_DISTRICT_CODE))
                                {
                                    return false;
                                }
                                else if (lstRoadLevel.Any(m => m.IMS_UNLOCK_LEVEL == "B" && m.MAST_BLOCK_CODE == existingRoad.MAST_BLOCK_CODE))
                                {
                                    return false;
                                }
                                break;
                            case "B":
                                List<IMS_UNLOCK_DETAILS> lstBlockLevel = dbContext.IMS_UNLOCK_DETAILS.Where(m => m.IMS_UNLOCK_START_DATE <= startDate && m.IMS_UNLOCK_END_DATE >= endDate && (m.IMS_UNLOCK_LEVEL == "D" || m.IMS_UNLOCK_LEVEL == "S" || m.IMS_UNLOCK_LEVEL == "B") && m.IMS_UNLOCK_TABLE == unlockTable).ToList();
                                int districtCode = dbContext.MASTER_BLOCK.Where(m => m.MAST_BLOCK_CODE == id).Select(m => m.MAST_DISTRICT_CODE).FirstOrDefault();
                                int stateCode = dbContext.MASTER_DISTRICT.Where(m => m.MAST_DISTRICT_CODE == districtCode).Select(m => m.MAST_STATE_CODE).FirstOrDefault();
                                if (lstBlockLevel.Any(m => m.IMS_UNLOCK_LEVEL == "S" && m.MAST_STATE_CODE == stateCode))
                                {
                                    return false;
                                }
                                else if (lstBlockLevel.Any(m => m.IMS_UNLOCK_LEVEL == "D" && m.MAST_DISTRICT_CODE == districtCode))
                                {
                                    return false;
                                }
                                break;
                            case "D":
                                List<IMS_UNLOCK_DETAILS> lstDistrictLevel = dbContext.IMS_UNLOCK_DETAILS.Where(m => m.IMS_UNLOCK_START_DATE <= startDate && m.IMS_UNLOCK_END_DATE >= endDate && (m.IMS_UNLOCK_LEVEL == "D" || m.IMS_UNLOCK_LEVEL == "S" || m.IMS_UNLOCK_LEVEL == "B") && m.IMS_UNLOCK_TABLE == unlockTable).ToList();
                                stateCode = dbContext.MASTER_DISTRICT.Where(m => m.MAST_DISTRICT_CODE == id).Select(m => m.MAST_STATE_CODE).FirstOrDefault();
                                if (lstDistrictLevel.Any(m => m.IMS_UNLOCK_LEVEL == "S" && m.MAST_STATE_CODE == stateCode))
                                {
                                    return false;
                                }
                                break;
                            case "S":
                                break;
                            default:
                                break;
                        }
                        break;
                    case "CN":
                        PLAN_ROAD corenetworkMaster = dbContext.PLAN_ROAD.Find(id);
                        switch (unlockLevel)
                        {
                            case "R":
                                List<IMS_UNLOCK_DETAILS> lstRoadLevel = dbContext.IMS_UNLOCK_DETAILS.Where(m => m.IMS_UNLOCK_START_DATE <= startDate && m.IMS_UNLOCK_END_DATE >= endDate && (m.IMS_UNLOCK_LEVEL == "D" || m.IMS_UNLOCK_LEVEL == "S" || m.IMS_UNLOCK_LEVEL == "B") && m.IMS_UNLOCK_TABLE == unlockTable).ToList();
                                if (lstRoadLevel.Any(m => m.IMS_UNLOCK_LEVEL == "S" && m.MAST_STATE_CODE == corenetworkMaster.MAST_STATE_CODE))
                                {
                                    return false;
                                }
                                else if (lstRoadLevel.Any(m => m.IMS_UNLOCK_LEVEL == "D" && m.MAST_DISTRICT_CODE == corenetworkMaster.MAST_DISTRICT_CODE))
                                {
                                    return false;
                                }
                                else if (lstRoadLevel.Any(m => m.IMS_UNLOCK_LEVEL == "B" && m.MAST_BLOCK_CODE == corenetworkMaster.MAST_BLOCK_CODE))
                                {
                                    return false;
                                }
                                break;
                            case "B":
                                List<IMS_UNLOCK_DETAILS> lstBlockLevel = dbContext.IMS_UNLOCK_DETAILS.Where(m => m.IMS_UNLOCK_START_DATE <= startDate && m.IMS_UNLOCK_END_DATE >= endDate && (m.IMS_UNLOCK_LEVEL == "D" || m.IMS_UNLOCK_LEVEL == "S" || m.IMS_UNLOCK_LEVEL == "B") && m.IMS_UNLOCK_TABLE == unlockTable).ToList();
                                int districtCode = dbContext.MASTER_BLOCK.Where(m => m.MAST_BLOCK_CODE == id).Select(m => m.MAST_DISTRICT_CODE).FirstOrDefault();
                                int stateCode = dbContext.MASTER_DISTRICT.Where(m => m.MAST_DISTRICT_CODE == districtCode).Select(m => m.MAST_STATE_CODE).FirstOrDefault();
                                if (lstBlockLevel.Any(m => m.IMS_UNLOCK_LEVEL == "S" && m.MAST_STATE_CODE == stateCode))
                                {
                                    return false;
                                }
                                else if (lstBlockLevel.Any(m => m.IMS_UNLOCK_LEVEL == "D" && m.MAST_DISTRICT_CODE == districtCode))
                                {
                                    return false;
                                }
                                break;
                            case "D":
                                List<IMS_UNLOCK_DETAILS> lstDistrictLevel = dbContext.IMS_UNLOCK_DETAILS.Where(m => m.IMS_UNLOCK_START_DATE <= startDate && m.IMS_UNLOCK_END_DATE >= endDate && (m.IMS_UNLOCK_LEVEL == "D" || m.IMS_UNLOCK_LEVEL == "S" || m.IMS_UNLOCK_LEVEL == "B") && m.IMS_UNLOCK_TABLE == unlockTable).ToList();
                                stateCode = dbContext.MASTER_DISTRICT.Where(m => m.MAST_DISTRICT_CODE == id).Select(m => m.MAST_STATE_CODE).FirstOrDefault();
                                if (lstDistrictLevel.Any(m => m.IMS_UNLOCK_LEVEL == "S" && m.MAST_STATE_CODE == stateCode))
                                {
                                    return false;
                                }
                                break;
                            case "S":
                                break;
                            default:
                                break;
                        }
                        break;
                    case "HM":
                        MASTER_HABITATIONS habMaster = dbContext.MASTER_HABITATIONS.Find(id);

                        switch (unlockLevel)
                        {
                            case "H":
                                int blockCode = dbContext.MASTER_VILLAGE.Where(m => m.MAST_VILLAGE_CODE == habMaster.MAST_VILLAGE_CODE).Select(m => m.MAST_BLOCK_CODE).FirstOrDefault();
                                int district = dbContext.MASTER_BLOCK.Where(m => m.MAST_BLOCK_CODE == blockCode).Select(m => m.MAST_DISTRICT_CODE).FirstOrDefault();
                                int state = dbContext.MASTER_DISTRICT.Where(m => m.MAST_DISTRICT_CODE == district).Select(m => m.MAST_STATE_CODE).FirstOrDefault();
                                List<IMS_UNLOCK_DETAILS> lstHabLevel = dbContext.IMS_UNLOCK_DETAILS.Where(m => m.IMS_UNLOCK_START_DATE <= startDate && m.IMS_UNLOCK_END_DATE >= endDate && (m.IMS_UNLOCK_LEVEL == "D" || m.IMS_UNLOCK_LEVEL == "S" || m.IMS_UNLOCK_LEVEL == "B" || m.IMS_UNLOCK_LEVEL == "V") && m.IMS_UNLOCK_TABLE == unlockTable).ToList();
                                if (lstHabLevel.Any(m => m.IMS_UNLOCK_LEVEL == "S" && m.MAST_STATE_CODE == state))
                                {
                                    return false;
                                }
                                else if (lstHabLevel.Any(m => m.IMS_UNLOCK_LEVEL == "D" && m.MAST_DISTRICT_CODE == district))
                                {
                                    return false;
                                }
                                else if (lstHabLevel.Any(m => m.IMS_UNLOCK_LEVEL == "B" && m.MAST_BLOCK_CODE == blockCode))
                                {
                                    return false;
                                }
                                else if (lstHabLevel.Any(m => m.IMS_UNLOCK_LEVEL == "V" && m.MAST_VILLAGE_CODE == habMaster.MAST_VILLAGE_CODE))
                                {
                                    return false;
                                }
                                break;
                            case "V":
                                blockCode = dbContext.MASTER_VILLAGE.Where(m => m.MAST_VILLAGE_CODE == id).Select(m => m.MAST_BLOCK_CODE).FirstOrDefault();
                                district = dbContext.MASTER_BLOCK.Where(m => m.MAST_BLOCK_CODE == blockCode).Select(m => m.MAST_DISTRICT_CODE).FirstOrDefault();
                                state = dbContext.MASTER_DISTRICT.Where(m => m.MAST_DISTRICT_CODE == district).Select(m => m.MAST_STATE_CODE).FirstOrDefault();
                                List<IMS_UNLOCK_DETAILS> lstVillageLevel = dbContext.IMS_UNLOCK_DETAILS.Where(m => m.IMS_UNLOCK_START_DATE <= startDate && m.IMS_UNLOCK_END_DATE >= endDate && (m.IMS_UNLOCK_LEVEL == "D" || m.IMS_UNLOCK_LEVEL == "S" || m.IMS_UNLOCK_LEVEL == "B") && m.IMS_UNLOCK_TABLE == unlockTable).ToList();
                                if (lstVillageLevel.Any(m => m.IMS_UNLOCK_LEVEL == "S" && m.MAST_STATE_CODE == state))
                                {
                                    return false;
                                }
                                else if (lstVillageLevel.Any(m => m.IMS_UNLOCK_LEVEL == "D" && m.MAST_DISTRICT_CODE == district))
                                {
                                    return false;
                                }
                                else if (lstVillageLevel.Any(m => m.IMS_UNLOCK_LEVEL == "B" && m.MAST_BLOCK_CODE == blockCode))
                                {
                                    return false;
                                }
                                break;
                            case "B":
                                district = dbContext.MASTER_BLOCK.Where(m => m.MAST_BLOCK_CODE == id).Select(m => m.MAST_DISTRICT_CODE).FirstOrDefault();
                                state = dbContext.MASTER_DISTRICT.Where(m => m.MAST_DISTRICT_CODE == district).Select(m => m.MAST_STATE_CODE).FirstOrDefault();
                                List<IMS_UNLOCK_DETAILS> lstBlockLevel = dbContext.IMS_UNLOCK_DETAILS.Where(m => m.IMS_UNLOCK_START_DATE <= startDate && m.IMS_UNLOCK_END_DATE >= endDate && (m.IMS_UNLOCK_LEVEL == "D" || m.IMS_UNLOCK_LEVEL == "S" || m.IMS_UNLOCK_LEVEL == "B") && m.IMS_UNLOCK_TABLE == unlockTable).ToList();
                                if (lstBlockLevel.Any(m => m.IMS_UNLOCK_LEVEL == "S" && m.MAST_STATE_CODE == state))
                                {
                                    return false;
                                }
                                else if (lstBlockLevel.Any(m => m.IMS_UNLOCK_LEVEL == "D" && m.MAST_DISTRICT_CODE == district))
                                {
                                    return false;
                                }
                                break;
                            case "D":
                                state = dbContext.MASTER_DISTRICT.Where(m => m.MAST_DISTRICT_CODE == id).Select(m => m.MAST_STATE_CODE).FirstOrDefault();
                                List<IMS_UNLOCK_DETAILS> lstDistrictLevel = dbContext.IMS_UNLOCK_DETAILS.Where(m => m.IMS_UNLOCK_START_DATE <= startDate && m.IMS_UNLOCK_END_DATE >= endDate && (m.IMS_UNLOCK_LEVEL == "D" || m.IMS_UNLOCK_LEVEL == "S" || m.IMS_UNLOCK_LEVEL == "B") && m.IMS_UNLOCK_TABLE == unlockTable).ToList();
                                if (lstDistrictLevel.Any(m => m.IMS_UNLOCK_LEVEL == "S" && m.MAST_STATE_CODE == state))
                                {
                                    return false;
                                }
                                break;
                            case "S":
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
                return true;
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

        public bool CheckActiveLevel(string unlockTable, string level, int item)
        {
            dbContext = new PMGSYEntities();
            try
            {
                switch (unlockTable)
                {
                    case "PR":
                        switch (level)
                        {
                            case "S":
                                if (dbContext.IMS_UNLOCK_DETAILS.Any(m => m.IMS_UNLOCK_TABLE == unlockTable && m.IMS_UNLOCK_END_DATE >= System.DateTime.Now && m.IMS_UNLOCK_STATUS == "Y" && (m.IMS_UNLOCK_LEVEL == "D" || m.IMS_UNLOCK_LEVEL == "B" || m.IMS_UNLOCK_LEVEL == "R")))
                                {
                                    return false;
                                }
                                break;
                            case "D":
                                if (dbContext.IMS_UNLOCK_DETAILS.Any(m => m.IMS_UNLOCK_TABLE == unlockTable && m.IMS_UNLOCK_END_DATE >= System.DateTime.Now && m.IMS_UNLOCK_STATUS == "Y" && (m.IMS_UNLOCK_LEVEL == "B" || m.IMS_UNLOCK_LEVEL == "R")))
                                {
                                    return false;
                                }
                                break;
                            case "B":
                                if (dbContext.IMS_UNLOCK_DETAILS.Any(m => m.IMS_UNLOCK_TABLE == unlockTable && m.IMS_UNLOCK_END_DATE >= System.DateTime.Now && m.IMS_UNLOCK_STATUS == "Y" && m.IMS_UNLOCK_LEVEL == "R"))
                                {
                                    return false;
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    case "ER":
                        switch (level)
                        {
                            case "S":
                                if (dbContext.IMS_UNLOCK_DETAILS.Any(m => m.IMS_UNLOCK_TABLE == unlockTable && m.IMS_UNLOCK_END_DATE >= System.DateTime.Now && m.IMS_UNLOCK_STATUS == "Y" && (m.IMS_UNLOCK_LEVEL == "D" || m.IMS_UNLOCK_LEVEL == "B" || m.IMS_UNLOCK_LEVEL == "R")))
                                {
                                    return false;
                                }
                                break;
                            case "D":
                                if (dbContext.IMS_UNLOCK_DETAILS.Any(m => m.IMS_UNLOCK_TABLE == unlockTable && m.IMS_UNLOCK_END_DATE >= System.DateTime.Now && m.IMS_UNLOCK_STATUS == "Y" && (m.IMS_UNLOCK_LEVEL == "B" || m.IMS_UNLOCK_LEVEL == "R")))
                                {
                                    return false;
                                }
                                break;
                            case "B":
                                if (dbContext.IMS_UNLOCK_DETAILS.Any(m => m.IMS_UNLOCK_TABLE == unlockTable && m.IMS_UNLOCK_END_DATE >= System.DateTime.Now && m.IMS_UNLOCK_STATUS == "Y" && m.IMS_UNLOCK_LEVEL == "R"))
                                {
                                    return false;
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    case "CN":
                        switch (level)
                        {
                            case "S":
                                if (dbContext.IMS_UNLOCK_DETAILS.Any(m => m.IMS_UNLOCK_TABLE == unlockTable && m.IMS_UNLOCK_END_DATE >= System.DateTime.Now && m.IMS_UNLOCK_STATUS == "Y" && (m.IMS_UNLOCK_LEVEL == "D" || m.IMS_UNLOCK_LEVEL == "B" || m.IMS_UNLOCK_LEVEL == "R")))
                                {
                                    return false;
                                }
                                break;
                            case "D":
                                if (dbContext.IMS_UNLOCK_DETAILS.Any(m => m.IMS_UNLOCK_TABLE == unlockTable && m.IMS_UNLOCK_END_DATE >= System.DateTime.Now && m.IMS_UNLOCK_STATUS == "Y" && (m.IMS_UNLOCK_LEVEL == "B" || m.IMS_UNLOCK_LEVEL == "R")))
                                {
                                    return false;
                                }
                                break;
                            case "B":
                                if (dbContext.IMS_UNLOCK_DETAILS.Any(m => m.IMS_UNLOCK_TABLE == unlockTable && m.IMS_UNLOCK_END_DATE >= System.DateTime.Now && m.IMS_UNLOCK_STATUS == "Y" && m.IMS_UNLOCK_LEVEL == "R"))
                                {
                                    return false;
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    case "HM":
                        switch (level)
                        {
                            case "S":
                                if (dbContext.IMS_UNLOCK_DETAILS.Any(m => m.IMS_UNLOCK_TABLE == unlockTable && m.IMS_UNLOCK_END_DATE >= System.DateTime.Now && m.IMS_UNLOCK_STATUS == "Y" && (m.IMS_UNLOCK_LEVEL == "D" || m.IMS_UNLOCK_LEVEL == "B" || m.IMS_UNLOCK_LEVEL == "V" || m.IMS_UNLOCK_LEVEL == "H")))
                                {
                                    return false;
                                }
                                break;
                            case "D":
                                if (dbContext.IMS_UNLOCK_DETAILS.Any(m => m.IMS_UNLOCK_TABLE == unlockTable && m.IMS_UNLOCK_END_DATE >= System.DateTime.Now && m.IMS_UNLOCK_STATUS == "Y" && (m.IMS_UNLOCK_LEVEL == "B" || m.IMS_UNLOCK_LEVEL == "H" || m.IMS_UNLOCK_LEVEL == "V")))
                                {
                                    return false;
                                }
                                break;
                            case "B":
                                if (dbContext.IMS_UNLOCK_DETAILS.Any(m => m.IMS_UNLOCK_TABLE == unlockTable && m.IMS_UNLOCK_END_DATE >= System.DateTime.Now && m.IMS_UNLOCK_STATUS == "Y" && (m.IMS_UNLOCK_LEVEL == "V" || m.IMS_UNLOCK_LEVEL == "H")))
                                {
                                    return false;
                                }
                                break;
                            case "V":
                                if (dbContext.IMS_UNLOCK_DETAILS.Any(m => m.IMS_UNLOCK_TABLE == unlockTable && m.IMS_UNLOCK_END_DATE >= System.DateTime.Now && m.IMS_UNLOCK_STATUS == "Y" && m.IMS_UNLOCK_LEVEL == "H"))
                                {
                                    return false;
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }


                if (dbContext.IMS_UNLOCK_DETAILS.Any(m => m.IMS_UNLOCK_TABLE == unlockTable && m.IMS_UNLOCK_END_DATE >= System.DateTime.Now && m.IMS_UNLOCK_STATUS == "Y" && m.IMS_UNLOCK_LEVEL != level))
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);

                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array GetITNOProposalList(int yearCode, int stateCode, int districtCode, int blockCode, int batchCode, string packageCode, byte scheme, string type, int page, int rows, string sidx, string sord, out long totalRecords, out List<int> lstIds, out string ImsPrRoadCode)
        {
            dbContext = new PMGSYEntities();
            try
            {
                //For In Clause in LINQ Contains
                //var lstParantNd = (dbContext.ADMIN_DEPARTMENT.Where(a => a.MAST_PARENT_ND_CODE == PMGSYSession.Current.AdminNdCode).Select(a => a.ADMIN_ND_CODE).ToList());

                ///Changes for RCPLWE unlocking at ITNO
                var lstStateNd = dbContext.ADMIN_DEPARTMENT.Where(x => x.MAST_STATE_CODE == PMGSYSession.Current.StateCode && x.MAST_ND_TYPE == "S").Select(x => x.ADMIN_ND_CODE).ToList();
                var lstParantNd = (dbContext.ADMIN_DEPARTMENT.Where(a => lstStateNd.Contains(a.MAST_PARENT_ND_CODE.Value)).Select(a => a.ADMIN_ND_CODE).ToList());

                var lstProposalList = (from item in dbContext.IMS_SANCTIONED_PROJECTS
                                       join state in dbContext.MASTER_STATE on item.MAST_STATE_CODE equals state.MAST_STATE_CODE
                                       join district in dbContext.MASTER_DISTRICT on item.MAST_DISTRICT_CODE equals district.MAST_DISTRICT_CODE
                                       join block in dbContext.MASTER_BLOCK on item.MAST_BLOCK_CODE equals block.MAST_BLOCK_CODE
                                       // join year in dbContext.MASTER_YEAR on item.IMS_YEAR equals year.MAST_YEAR_CODE
                                       where
                                       (yearCode == 0 ? 1 : item.IMS_YEAR) == (yearCode == 0 ? 1 : yearCode) &&
                                       (stateCode == 0 ? 1 : item.MAST_STATE_CODE) == (stateCode == 0 ? 1 : stateCode) &&
                                       (districtCode <= 0 ? 1 : item.MAST_DISTRICT_CODE) == (districtCode <= 0 ? 1 : districtCode) &&
                                       (batchCode == 0 ? 1 : item.IMS_BATCH) == (batchCode == 0 ? 1 : batchCode) &&
                                       ((packageCode == "-1" || packageCode == "0") ? "%" : item.IMS_PACKAGE_ID) == ((packageCode == "-1" || packageCode == "0") ? "%" : packageCode) &&
                                       (blockCode <= 0 ? 1 : item.MAST_BLOCK_CODE) == (blockCode <= 0 ? 1 : blockCode) &&
                                           //((item.IMS_ISCOMPLETED == "S" && item.STA_SANCTIONED == "U") ||
                                           //(item.IMS_ISCOMPLETED == "D" && item.STA_SANCTIONED == "N")) &&

                                       ///Changes by SAMMED A. PATIL on 20JULY2017 to display Type at ITNO login
                                       (type == "S" ? (item.IMS_ISCOMPLETED == "S" && item.STA_SANCTIONED == "U") : (item.IMS_ISCOMPLETED == "D" && item.STA_SANCTIONED == "N")) &&
                                       (lstParantNd.Contains(item.MAST_DPIU_CODE)) &&
                                       item.MAST_PMGSY_SCHEME == scheme
                                       select new
                                       {
                                           block.MAST_BLOCK_NAME,
                                           item.IMS_PACKAGE_ID,
                                           item.IMS_PR_ROAD_CODE,
                                           item.IMS_ROAD_NAME,
                                           item.IMS_YEAR,
                                           item.IMS_LOCK_STATUS,
                                           item.IMS_PAV_LENGTH

                                       }).Distinct();


                totalRecords = lstProposalList.Count();

                lstIds = new List<int>();

                ImsPrRoadCode = String.Empty;
                foreach (var item in lstProposalList)
                {
                    int id = item.IMS_PR_ROAD_CODE;
                    lstIds.Add(id);

                    if (ImsPrRoadCode == String.Empty)
                    {
                        ImsPrRoadCode = Convert.ToString(item.IMS_PR_ROAD_CODE);
                    }
                    else
                    {
                        ImsPrRoadCode = ImsPrRoadCode + "_" + item.IMS_PR_ROAD_CODE;
                    }

                }
                //sorting 
                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "MAST_BLOCK_NAME":
                                lstProposalList = lstProposalList.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "IMS_PACKAGE_ID":
                                lstProposalList = lstProposalList.OrderBy(x => x.IMS_PACKAGE_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "IMS_ROAD_NAME":
                                lstProposalList = lstProposalList.OrderBy(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "IMS_ROAD_CODE":
                                lstProposalList = lstProposalList.OrderBy(x => x.IMS_PR_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "YEARCODE":
                                lstProposalList = lstProposalList.OrderBy(x => x.IMS_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "IMS_LOCK_STATUS":
                                lstProposalList = lstProposalList.OrderBy(x => x.IMS_LOCK_STATUS).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "IMS_PAV_LENGTH":
                                lstProposalList = lstProposalList.OrderBy(x => x.IMS_PAV_LENGTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "MAST_BLOCK_NAME":
                                lstProposalList = lstProposalList.OrderByDescending(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "IMS_PACKAGE_ID":
                                lstProposalList = lstProposalList.OrderByDescending(x => x.IMS_PACKAGE_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "IMS_ROAD_NAME":
                                lstProposalList = lstProposalList.OrderByDescending(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "IMS_ROAD_CODE":
                                lstProposalList = lstProposalList.OrderByDescending(x => x.IMS_PR_ROAD_CODE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "YEARCODE":
                                lstProposalList = lstProposalList.OrderByDescending(x => x.IMS_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "IMS_LOCK_STATUS":
                                lstProposalList = lstProposalList.OrderByDescending(x => x.IMS_LOCK_STATUS).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            case "IMS_PAV_LENGTH":
                                lstProposalList = lstProposalList.OrderByDescending(x => x.IMS_PAV_LENGTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                            default:
                                lstProposalList = lstProposalList.OrderByDescending(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                                break;
                        }
                    }
                }
                else
                {
                    lstProposalList = lstProposalList.OrderByDescending(x => x.IMS_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }


                var gridData = lstProposalList.Select(proposalDetails => new
                {

                    proposalDetails.MAST_BLOCK_NAME,
                    proposalDetails.IMS_PACKAGE_ID,
                    proposalDetails.IMS_PR_ROAD_CODE,
                    proposalDetails.IMS_ROAD_NAME,
                    proposalDetails.IMS_YEAR,
                    proposalDetails.IMS_LOCK_STATUS,
                    proposalDetails.IMS_PAV_LENGTH
                }).ToArray();


                //returning the griddata
                var result = gridData.Select(proposalDetails => new
                {

                    id = proposalDetails.IMS_PR_ROAD_CODE.ToString(),
                    IMS_LOCK_STATUS = proposalDetails.IMS_LOCK_STATUS.ToString(),
                    cell = new[]{
                    
                        proposalDetails.MAST_BLOCK_NAME==null?string.Empty:proposalDetails.MAST_BLOCK_NAME.ToString(),
                        proposalDetails.IMS_ROAD_NAME==null?string.Empty:proposalDetails.IMS_ROAD_NAME.ToString(),
                        proposalDetails.IMS_YEAR == null?string.Empty:((proposalDetails.IMS_YEAR)+" - "+(proposalDetails.IMS_YEAR+1)).ToString(),
                        proposalDetails.IMS_PACKAGE_ID==null?string.Empty:proposalDetails.IMS_PACKAGE_ID.ToString(),
                        proposalDetails.IMS_PAV_LENGTH.ToString(), 
                        //proposalDetails.IMS_PR_ROAD_CODE == 0?string.Empty:proposalDetails.IMS_PR_ROAD_CODE.ToString(),
                        //proposalDetails.IMS_LOCK_STATUS=="N"?"<a href='#' title='Click to Lock Proposal Details' class='ui-icon ui-icon-locked ui-align-center' onClick='LockModuleUnit(\"" + URLEncrypt.EncryptParameters1(new string[]{"ProposalCode="+ proposalDetails.IMS_PR_ROAD_CODE.ToString().Trim() }) +"\"); return false;'></a>":"<a href='#' title='Click to Unlock Proposal Details' class='ui-icon ui-icon-unlocked ui-align-center' onClick='UnlockModuleUnit(\"" + URLEncrypt.EncryptParameters1(new string[]{"ProposalCode="+ proposalDetails.IMS_PR_ROAD_CODE.ToString().Trim() }) +"\"); return false;'></a>"
                    }
                }).ToArray();

                return result;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);

                ImsPrRoadCode = String.Empty;
                lstIds = null;
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public bool ValidateUnlockLevel(int id, string level, string unlockTable)
        {
            dbContext = new PMGSYEntities();
            try
            {
                List<int?> lstDistricts = null;
                List<int?> lstBlocks = null;
                List<int?> lstVillages = null;
                List<int?> lstRoads = null;
                var lstHabs = (IEnumerable<dynamic>)null;
                List<int?> districtCount = null;
                List<int?> blockCount = null;
                List<int?> roadCount = null;
                int? activeState = 0;
                int? unlockState = 0;


                //switch (imsMaster.IMS_UNLOCK_LEVEL)
                //{
                //    case "S":
                //        activeState = imsMaster.MAST_STATE_CODE;
                //        break;
                //    case "D":
                //        activeState = dbContext.MASTER_DISTRICT.Where(m => m.MAST_DISTRICT_CODE == imsMaster.MAST_DISTRICT_CODE).Select(m => m.MAST_STATE_CODE).FirstOrDefault();
                //        break;
                //    case "B":
                //        activeState = (from item in dbContext.MASTER_BLOCK
                //                       join district in dbContext.MASTER_DISTRICT
                //                       on item.MAST_DISTRICT_CODE equals district.MAST_DISTRICT_CODE
                //                       join state in dbContext.MASTER_STATE
                //                       on district.MAST_STATE_CODE equals state.MAST_STATE_CODE
                //                       where item.MAST_BLOCK_CODE == imsMaster.MAST_BLOCK_CODE
                //                       select state.MAST_STATE_CODE).FirstOrDefault();
                //        break;
                //    case "R":
                //        switch (imsMaster.IMS_UNLOCK_TABLE)
                //        {
                //            case "PR":
                //                activeState = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == imsMaster.IMS_PR_ROAD_CODE).Select(m=>m.MAST_STATE_CODE).FirstOrDefault();
                //                break;
                //            case "ER":
                //                activeState = dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == imsMaster.MAST_ER_ROAD_CODE).Select(m => m.MAST_STATE_CODE).FirstOrDefault();
                //                break;
                //            case "CN":
                //                activeState = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == imsMaster.PLAN_CN_ROAD_CODE).Select(m => m.MAST_STATE_CODE).FirstOrDefault();
                //                break;
                //            default:
                //                break;
                //        }
                //        break;
                //    default:
                //        break;
                //}

                switch (level)
                {
                    case "S":
                        unlockState = id;
                        break;
                    case "D":
                        unlockState = dbContext.MASTER_DISTRICT.Where(m => m.MAST_DISTRICT_CODE == id).Select(m => m.MAST_STATE_CODE).FirstOrDefault();
                        break;
                    case "B":
                        unlockState = (from item in dbContext.MASTER_BLOCK
                                       join district in dbContext.MASTER_DISTRICT
                                       on item.MAST_DISTRICT_CODE equals district.MAST_DISTRICT_CODE
                                       join state in dbContext.MASTER_STATE
                                       on district.MAST_STATE_CODE equals state.MAST_STATE_CODE
                                       where item.MAST_BLOCK_CODE == id
                                       select state.MAST_STATE_CODE).FirstOrDefault();
                        break;
                    case "R":
                        switch (unlockTable)
                        {
                            case "PR":
                                unlockState = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == id).Select(m => m.MAST_STATE_CODE).FirstOrDefault();
                                break;
                            case "ER":
                                unlockState = dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_ER_ROAD_CODE == id).Select(m => m.MAST_STATE_CODE).FirstOrDefault();
                                break;
                            case "CN":
                                unlockState = dbContext.PLAN_ROAD.Where(m => m.PLAN_CN_ROAD_CODE == id).Select(m => m.MAST_STATE_CODE).FirstOrDefault();
                                break;
                            case "PH":
                                unlockState = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.IMS_PR_ROAD_CODE == id).Select(m => m.MAST_STATE_CODE).FirstOrDefault();
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }

                IMS_UNLOCK_DETAILS imsMaster = dbContext.IMS_UNLOCK_DETAILS.Where(m => m.IMS_UNLOCK_TABLE == unlockTable && m.IMS_UNLOCK_END_DATE >= System.DateTime.Now && m.IMS_UNLOCK_STATUS == "Y" && m.MAST_STATE_CODE == unlockState && m.MAST_PMGSY_SCHEME == PMGSYSession.Current.PMGSYScheme && m.IMS_UNLOCK_LEVEL == level).OrderByDescending(m => m.IMS_TRANSACTION_NO).FirstOrDefault();
                if (imsMaster == null)
                {
                    return true;
                }
                else if (level == imsMaster.IMS_UNLOCK_LEVEL && unlockTable == imsMaster.IMS_UNLOCK_TABLE)
                {
                    return true;
                }
                else
                {
                    return false;
                }

                switch (level)
                {
                    case "S":
                        lstDistricts = dbContext.MASTER_DISTRICT.Where(m => m.MAST_STATE_CODE == id).Select(m => (Int32?)m.MAST_DISTRICT_CODE).ToList();
                        districtCount = dbContext.IMS_UNLOCK_DETAILS.Where(m => lstDistricts.Contains(m.MAST_DISTRICT_CODE) && m.IMS_UNLOCK_END_DATE >= System.DateTime.Now && m.IMS_UNLOCK_STATUS == "Y" && m.IMS_UNLOCK_TABLE == "PR").Select(m => m.MAST_DISTRICT_CODE).ToList();
                        if (districtCount.Count() > 0)
                        {
                            return false;
                        }
                        lstBlocks = dbContext.MASTER_BLOCK.Where(m => lstDistricts.Contains(m.MAST_DISTRICT_CODE)).Select(m => (Int32?)m.MAST_BLOCK_CODE).ToList();
                        blockCount = dbContext.IMS_UNLOCK_DETAILS.Where(m => lstBlocks.Contains(m.MAST_BLOCK_CODE) && m.IMS_UNLOCK_END_DATE >= System.DateTime.Now && m.IMS_UNLOCK_STATUS == "Y" && m.IMS_UNLOCK_TABLE == "PR").Select(m => m.MAST_BLOCK_CODE).ToList();
                        if (blockCount.Count() > 0)
                        {
                            return false;
                        }
                        lstRoads = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => lstBlocks.Contains(m.MAST_BLOCK_CODE)).Select(m => (Int32?)m.IMS_PR_ROAD_CODE).ToList();
                        roadCount = dbContext.IMS_UNLOCK_DETAILS.Where(m => lstRoads.Contains(m.IMS_PR_ROAD_CODE) && m.IMS_UNLOCK_END_DATE >= System.DateTime.Now && m.IMS_UNLOCK_STATUS == "Y" && m.IMS_UNLOCK_TABLE == "PR").Select(m => m.IMS_PR_ROAD_CODE).ToList();
                        if (roadCount.Count() > 0)
                        {
                            return false;
                        }
                        break;
                    case "D":

                        break;
                    default:
                        break;
                }

                switch (unlockTable)
                {
                    case "PR":
                        switch (level)
                        {
                            case "S":
                                lstDistricts = dbContext.MASTER_DISTRICT.Where(m => m.MAST_STATE_CODE == id).Select(m => (Int32?)m.MAST_DISTRICT_CODE).ToList();
                                districtCount = dbContext.IMS_UNLOCK_DETAILS.Where(m => lstDistricts.Contains(m.MAST_DISTRICT_CODE) && m.IMS_UNLOCK_END_DATE >= System.DateTime.Now && m.IMS_UNLOCK_STATUS == "Y" && m.IMS_UNLOCK_TABLE == "PR").Select(m => m.MAST_DISTRICT_CODE).ToList();
                                if (districtCount.Count() > 0)
                                {
                                    return false;
                                }
                                lstBlocks = dbContext.MASTER_BLOCK.Where(m => lstDistricts.Contains(m.MAST_DISTRICT_CODE)).Select(m => (Int32?)m.MAST_BLOCK_CODE).ToList();
                                blockCount = dbContext.IMS_UNLOCK_DETAILS.Where(m => lstBlocks.Contains(m.MAST_BLOCK_CODE) && m.IMS_UNLOCK_END_DATE >= System.DateTime.Now && m.IMS_UNLOCK_STATUS == "Y" && m.IMS_UNLOCK_TABLE == "PR").Select(m => m.MAST_BLOCK_CODE).ToList();
                                if (blockCount.Count() > 0)
                                {
                                    return false;
                                }
                                lstRoads = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => lstBlocks.Contains(m.MAST_BLOCK_CODE)).Select(m => (Int32?)m.IMS_PR_ROAD_CODE).ToList();
                                roadCount = dbContext.IMS_UNLOCK_DETAILS.Where(m => lstRoads.Contains(m.IMS_PR_ROAD_CODE) && m.IMS_UNLOCK_END_DATE >= System.DateTime.Now && m.IMS_UNLOCK_STATUS == "Y" && m.IMS_UNLOCK_TABLE == "PR").Select(m => m.IMS_PR_ROAD_CODE).ToList();
                                if (roadCount.Count() > 0)
                                {
                                    return false;
                                }
                                break;
                            case "D":
                                lstBlocks = dbContext.MASTER_BLOCK.Where(m => m.MAST_DISTRICT_CODE == id).Select(m => (Int32?)m.MAST_BLOCK_CODE).ToList();
                                blockCount = dbContext.IMS_UNLOCK_DETAILS.Where(m => lstBlocks.Contains(m.MAST_BLOCK_CODE) && m.IMS_UNLOCK_END_DATE >= System.DateTime.Now && m.IMS_UNLOCK_STATUS == "Y" && m.IMS_UNLOCK_TABLE == "PR").Select(m => m.MAST_BLOCK_CODE).ToList();
                                if (blockCount.Count() > 0)
                                {
                                    return false;
                                }
                                lstRoads = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => lstBlocks.Contains(m.MAST_BLOCK_CODE)).Select(m => (Int32?)m.IMS_PR_ROAD_CODE).ToList();
                                roadCount = dbContext.IMS_UNLOCK_DETAILS.Where(m => lstRoads.Contains(m.IMS_PR_ROAD_CODE) && m.IMS_UNLOCK_END_DATE >= System.DateTime.Now && m.IMS_UNLOCK_STATUS == "Y" && m.IMS_UNLOCK_TABLE == "PR").Select(m => m.IMS_PR_ROAD_CODE).ToList();
                                if (roadCount.Count() > 0)
                                {
                                    return false;
                                }
                                break;
                            case "B":
                                lstRoads = dbContext.IMS_SANCTIONED_PROJECTS.Where(m => m.MAST_BLOCK_CODE == id).Select(m => (Int32?)m.IMS_PR_ROAD_CODE).ToList();
                                roadCount = dbContext.IMS_UNLOCK_DETAILS.Where(m => lstRoads.Contains(m.IMS_PR_ROAD_CODE) && m.IMS_UNLOCK_END_DATE >= System.DateTime.Now && m.IMS_UNLOCK_STATUS == "Y" && m.IMS_UNLOCK_TABLE == "PR").Select(m => m.IMS_PR_ROAD_CODE).ToList();
                                if (roadCount.Count() > 0)
                                {
                                    return false;
                                }
                                break;
                            case "R":

                                break;
                            default:
                                break;
                        }
                        break;
                    case "CN":
                        switch (level)
                        {
                            case "S":
                                lstDistricts = dbContext.MASTER_DISTRICT.Where(m => m.MAST_STATE_CODE == id).Select(m => (Int32?)m.MAST_DISTRICT_CODE).ToList();
                                districtCount = dbContext.IMS_UNLOCK_DETAILS.Where(m => lstDistricts.Contains(m.MAST_DISTRICT_CODE) && m.IMS_UNLOCK_END_DATE >= System.DateTime.Now && m.IMS_UNLOCK_STATUS == "Y" && m.IMS_UNLOCK_TABLE == "CN").Select(m => (Int32?)m.MAST_DISTRICT_CODE).ToList();
                                if (districtCount.Count() > 0)
                                {
                                    return false;
                                }
                                lstBlocks = dbContext.MASTER_BLOCK.Where(m => lstDistricts.Contains(m.MAST_DISTRICT_CODE)).Select(m => (Int32?)m.MAST_BLOCK_CODE).ToList();
                                blockCount = dbContext.IMS_UNLOCK_DETAILS.Where(m => lstBlocks.Contains(m.MAST_BLOCK_CODE) && m.IMS_UNLOCK_END_DATE >= System.DateTime.Now && m.IMS_UNLOCK_STATUS == "Y" && m.IMS_UNLOCK_TABLE == "CN").Select(m => m.MAST_BLOCK_CODE).ToList();
                                if (blockCount.Count() > 0)
                                {
                                    return false;
                                }
                                lstRoads = dbContext.PLAN_ROAD.Where(m => lstBlocks.Contains(m.MAST_BLOCK_CODE)).Select(m => (Int32?)m.PLAN_CN_ROAD_CODE).ToList();
                                roadCount = dbContext.IMS_UNLOCK_DETAILS.Where(m => lstRoads.Contains(m.IMS_PR_ROAD_CODE) && m.IMS_UNLOCK_END_DATE >= System.DateTime.Now && m.IMS_UNLOCK_STATUS == "Y" && m.IMS_UNLOCK_TABLE == "CN").Select(m => (Int32?)m.IMS_PR_ROAD_CODE).ToList();
                                if (roadCount.Count() > 0)
                                {
                                    return false;
                                }
                                break;
                            case "D":
                                lstBlocks = dbContext.MASTER_BLOCK.Where(m => m.MAST_DISTRICT_CODE == id).Select(m => (Int32?)m.MAST_BLOCK_CODE).ToList();
                                var blocks = dbContext.IMS_UNLOCK_DETAILS.Where(m => lstBlocks.Contains(m.MAST_BLOCK_CODE) && m.IMS_UNLOCK_END_DATE >= System.DateTime.Now && m.IMS_UNLOCK_STATUS == "Y" && m.IMS_UNLOCK_TABLE == "CN").Select(m => (Int32?)m.MAST_BLOCK_CODE).Distinct().ToList();
                                if (blocks.Count() > 0)
                                {
                                    return false;
                                }
                                lstRoads = dbContext.PLAN_ROAD.Where(m => lstBlocks.Contains(m.MAST_BLOCK_CODE)).Select(m => (Int32?)m.PLAN_CN_ROAD_CODE).ToList();
                                roadCount = dbContext.IMS_UNLOCK_DETAILS.Where(m => lstRoads.Contains(m.PLAN_CN_ROAD_CODE) && m.IMS_UNLOCK_END_DATE >= System.DateTime.Now && m.IMS_UNLOCK_STATUS == "Y" && m.IMS_UNLOCK_TABLE == "CN").Select(m => (Int32?)m.PLAN_CN_ROAD_CODE).ToList();
                                if (roadCount.Count() > 0)
                                {
                                    return false;
                                }
                                break;
                            case "B":
                                lstRoads = dbContext.PLAN_ROAD.Where(m => m.MAST_BLOCK_CODE == id).Select(m => (Int32?)m.PLAN_CN_ROAD_CODE).ToList();
                                roadCount = dbContext.IMS_UNLOCK_DETAILS.Where(m => lstRoads.Contains(m.PLAN_CN_ROAD_CODE) && m.IMS_UNLOCK_END_DATE >= System.DateTime.Now && m.IMS_UNLOCK_STATUS == "Y" && m.IMS_UNLOCK_TABLE == "CN").Select(m => (Int32?)m.PLAN_CN_ROAD_CODE).ToList();
                                if (roadCount.Count() > 0)
                                {
                                    return false;
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    case "ER":
                        switch (level)
                        {
                            case "S":
                                lstDistricts = dbContext.MASTER_DISTRICT.Where(m => m.MAST_STATE_CODE == id).Select(m => (Int32?)m.MAST_DISTRICT_CODE).ToList();
                                districtCount = dbContext.IMS_UNLOCK_DETAILS.Where(m => lstDistricts.Contains(m.MAST_DISTRICT_CODE) && m.IMS_UNLOCK_END_DATE >= System.DateTime.Now && m.IMS_UNLOCK_STATUS == "Y" && m.IMS_UNLOCK_TABLE == "ER").Select(m => m.MAST_DISTRICT_CODE).ToList();
                                if (districtCount.Count() > 0)
                                {
                                    return false;
                                }
                                lstBlocks = dbContext.MASTER_BLOCK.Where(m => lstDistricts.Contains(m.MAST_DISTRICT_CODE)).Select(m => (Int32?)m.MAST_BLOCK_CODE).ToList();
                                blockCount = dbContext.IMS_UNLOCK_DETAILS.Where(m => lstBlocks.Contains(m.MAST_BLOCK_CODE) && m.IMS_UNLOCK_END_DATE >= System.DateTime.Now && m.IMS_UNLOCK_STATUS == "Y" && m.IMS_UNLOCK_TABLE == "ER").Select(m => m.MAST_BLOCK_CODE).ToList();
                                if (blockCount.Count() > 0)
                                {
                                    return false;
                                }
                                lstRoads = dbContext.MASTER_EXISTING_ROADS.Where(m => lstBlocks.Contains(m.MAST_BLOCK_CODE)).Select(m => (Int32?)m.MAST_ER_ROAD_CODE).ToList();
                                roadCount = dbContext.IMS_UNLOCK_DETAILS.Where(m => lstRoads.Contains(m.MAST_ER_ROAD_CODE) && m.IMS_UNLOCK_END_DATE >= System.DateTime.Now && m.IMS_UNLOCK_STATUS == "Y" && m.IMS_UNLOCK_TABLE == "ER").Select(m => m.MAST_ER_ROAD_CODE).ToList();
                                if (roadCount.Count() > 0)
                                {
                                    return false;
                                }
                                break;
                            case "D":
                                lstBlocks = dbContext.MASTER_BLOCK.Where(m => m.MAST_DISTRICT_CODE == id).Select(m => (Int32?)m.MAST_BLOCK_CODE).ToList();
                                blockCount = dbContext.IMS_UNLOCK_DETAILS.Where(m => lstBlocks.Contains(m.MAST_BLOCK_CODE) && m.IMS_UNLOCK_END_DATE >= System.DateTime.Now && m.IMS_UNLOCK_STATUS == "Y" && m.IMS_UNLOCK_TABLE == "ER").Select(m => m.MAST_BLOCK_CODE).ToList();
                                if (blockCount.Count() > 0)
                                {
                                    return false;
                                }
                                lstRoads = dbContext.MASTER_EXISTING_ROADS.Where(m => lstBlocks.Contains(m.MAST_BLOCK_CODE)).Select(m => (Int32?)m.MAST_ER_ROAD_CODE).ToList();
                                roadCount = dbContext.IMS_UNLOCK_DETAILS.Where(m => lstRoads.Contains(m.MAST_ER_ROAD_CODE) && m.IMS_UNLOCK_END_DATE >= System.DateTime.Now && m.IMS_UNLOCK_STATUS == "Y" && m.IMS_UNLOCK_TABLE == "ER").Select(m => (Int32?)m.MAST_ER_ROAD_CODE).ToList();
                                if (roadCount.Count() > 0)
                                {
                                    return false;
                                }
                                break;
                            case "B":
                                lstRoads = dbContext.MASTER_EXISTING_ROADS.Where(m => m.MAST_BLOCK_CODE == id).Select(m => (Int32?)m.MAST_ER_ROAD_CODE).ToList();
                                roadCount = dbContext.IMS_UNLOCK_DETAILS.Where(m => lstRoads.Contains(m.MAST_ER_ROAD_CODE) && m.IMS_UNLOCK_END_DATE >= System.DateTime.Now && m.IMS_UNLOCK_STATUS == "Y" && m.IMS_UNLOCK_TABLE == "ER").Select(m => (Int32?)m.MAST_ER_ROAD_CODE).ToList();
                                if (roadCount.Count() > 0)
                                {
                                    return false;
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    case "HM":

                        break;
                    default:
                        break;
                }
                return true;
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "ValidateUnlockLevel().DAL");
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

        public Array GetUnlockRecordListDAL(int stateCode, int moduleCode, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            using (dbContext = new PMGSYEntities())
            {
                try
                {
                    string unlockTable = string.Empty;
                    switch (moduleCode)
                    {
                        case 1:
                            unlockTable = "HM";
                            break;
                        case 2:
                            unlockTable = "PR";
                            break;
                        case 3:
                            unlockTable = "ER";
                            break;
                        case 4:
                            unlockTable = "CN";
                            break;
                        case 5:
                            unlockTable = "VM";
                            break;
                        case 11:                        //Added By Shreyas to unlock C-Proforma PDF
                            unlockTable = "CP";
                            break;
                        default:
                            break;
                    }

                    var _lstRecords = dbContext.IMS_UNLOCK_DETAILS.Where(m => m.MAST_STATE_CODE == stateCode && m.IMS_UNLOCK_STATUS == "Y" && m.IMS_UNLOCK_TABLE == unlockTable).ToList();

                    totalRecords = _lstRecords.Count();

                    if (sidx.Trim() != string.Empty)
                    {
                        if (sord.ToString() == "asc")
                        {
                            switch (sidx)
                            {
                                case "IMS_UNLOCK_TABLE":
                                    _lstRecords = _lstRecords.OrderBy(x => x.IMS_UNLOCK_TABLE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            switch (sidx)
                            {
                                case "IMS_PAV_LENGTH":
                                    _lstRecords = _lstRecords.OrderByDescending(x => x.IMS_UNLOCK_TABLE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                    break;
                                default:
                                    _lstRecords = _lstRecords.OrderByDescending(x => x.IMS_UNLOCK_TABLE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                    break;
                            }
                        }
                    }
                    else
                    {
                        _lstRecords = _lstRecords.OrderByDescending(x => x.IMS_UNLOCK_TABLE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }


                    //var gridData = _lstRecords.Select(m => new
                    //{
                    //    cell = new[]
                    //    {
                    //        m.IMS_UNLOCK_TABLE == "PR"?"Proposal":(m.IMS_UNLOCK_TABLE == "ER"?"Existing Road":(m.IMS_UNLOCK_TABLE == "CN"?"Core Network":(m.IMS_UNLOCK_TABLE == "HM"?"Habitation":"Village"))),
                    //        m.MAST_PMGSY_SCHEME == null?"-":m.MAST_PMGSY_SCHEME.ToString(),
                    //        m.IMS_UNLOCK_LEVEL == "S"?"State":(m.IMS_UNLOCK_LEVEL == "D"?"District":(m.IMS_UNLOCK_LEVEL == "B"?"Block":(m.IMS_UNLOCK_LEVEL == "V"?"Village":(m.IMS_UNLOCK_LEVEL=="H"?"Habitation":"Road")))),
                    //        m.IMS_UNLOCK_LEVEL == "S"?m.MASTER_STATE.MAST_STATE_NAME.ToString():(m.IMS_UNLOCK_LEVEL == "D"?m.MASTER_DISTRICT.MAST_DISTRICT_NAME.ToString():(m.IMS_UNLOCK_LEVEL == "B"?m.MASTER_BLOCK.MAST_BLOCK_NAME:m.IMS_UNLOCK_TABLE == "PR"?m.IMS_SANCTIONED_PROJECTS.IMS_ROAD_NAME.ToString():(m.IMS_UNLOCK_TABLE == "ER"?m.MASTER_EXISTING_ROADS.MAST_ER_ROAD_NAME.ToString():(m.IMS_UNLOCK_TABLE == "CN"?m.PLAN_ROAD.PLAN_RD_NAME.ToString():(m.IMS_UNLOCK_LEVEL == "H"?m.MASTER_HABITATIONS.MAST_HAB_NAME.ToString():m.MASTER_VILLAGE.MAST_VILLAGE_NAME.ToString()))))),
                    //        //m.IMS_UNLOCK_TABLE == "PR"?m.IMS_SANCTIONED_PROJECTS.IMS_ROAD_NAME.ToString():(m.IMS_UNLOCK_TABLE == "ER"?m.MASTER_EXISTING_ROADS.MAST_ER_ROAD_NAME.ToString():(m.IMS_UNLOCK_TABLE == "CN"?m.PLAN_ROAD.PLAN_RD_NAME.ToString():m.MASTER_HABITATIONS.MAST_HAB_NAME.ToString())),
                    //        m.IMS_UNLOCK_START_DATE.ToString("dd/MM/yyyy"),
                    //        m.IMS_UNLOCK_END_DATE.ToString("dd/MM/yyyy"),
                    //        m.IMS_UNLOCK_REMARKS.ToString(),
                    //    }
                    //}).ToArray();

                    var gridData = _lstRecords.Select(m => new  //modified to display C-Proforma PDF data
                    {
                        cell = new[]
                        {
                            m.IMS_UNLOCK_TABLE == "PR"?"Proposal":(m.IMS_UNLOCK_TABLE == "ER"?"Existing Road":(m.IMS_UNLOCK_TABLE == "CN"?"Core Network": (m.IMS_UNLOCK_TABLE == "CP"?"C-Proforma PDF": (m.IMS_UNLOCK_TABLE == "HM"?"Habitation":"Village")))),
                            m.MAST_PMGSY_SCHEME == 0 ?"-":m.MAST_PMGSY_SCHEME.ToString(),
                            m.IMS_UNLOCK_LEVEL == "S"?"State":(m.IMS_UNLOCK_LEVEL == "D"?"District":(m.IMS_UNLOCK_LEVEL == "B"?"Block":(m.IMS_UNLOCK_LEVEL == "V"?"Village":(m.IMS_UNLOCK_LEVEL=="H"?"Habitation":"Road")))),
                            //m.IMS_UNLOCK_LEVEL == "S"?m.MASTER_STATE.MAST_STATE_NAME.ToString():(m.IMS_UNLOCK_LEVEL == "D"?m.MASTER_DISTRICT.MAST_DISTRICT_NAME.ToString():(m.IMS_UNLOCK_LEVEL == "B"?m.MASTER_BLOCK.MAST_BLOCK_NAME:m.IMS_UNLOCK_TABLE == "PR"?m.IMS_SANCTIONED_PROJECTS.IMS_ROAD_NAME.ToString():(m.IMS_UNLOCK_TABLE == "ER"?m.MASTER_EXISTING_ROADS.MAST_ER_ROAD_NAME.ToString():(m.IMS_UNLOCK_TABLE == "CN"?m.PLAN_ROAD.PLAN_RD_NAME.ToString():(m.IMS_UNLOCK_LEVEL == "H"?m.MASTER_HABITATIONS.MAST_HAB_NAME.ToString():m.MASTER_VILLAGE.MAST_VILLAGE_NAME.ToString()))))),
                            m.IMS_UNLOCK_LEVEL == "S"?m.MASTER_STATE.MAST_STATE_NAME.ToString():(m.IMS_UNLOCK_LEVEL == "D"?m.MASTER_DISTRICT.MAST_DISTRICT_NAME.ToString():(m.IMS_UNLOCK_LEVEL == "B"?m.MASTER_BLOCK.MAST_BLOCK_NAME:m.IMS_UNLOCK_TABLE == "PR"?m.IMS_SANCTIONED_PROJECTS.IMS_ROAD_NAME.ToString():(m.IMS_UNLOCK_TABLE == "ER"?m.MASTER_EXISTING_ROADS.MAST_ER_ROAD_NAME.ToString():(m.IMS_UNLOCK_TABLE == "CN"?m.PLAN_ROAD.PLAN_RD_NAME.ToString():(m.IMS_UNLOCK_LEVEL == "H"?m.MASTER_HABITATIONS.MAST_HAB_NAME.ToString():(m.IMS_UNLOCK_LEVEL == "R"?m.IMS_SANCTIONED_PROJECTS.IMS_ROAD_NAME.ToString() : (m.IMS_UNLOCK_LEVEL == "Y"? m.MASTER_STATE.MAST_STATE_NAME.ToString()+" - "+m.IMS_YEAR.ToString() : (m.IMS_UNLOCK_LEVEL == "T"? m.IMS_YEAR.ToString()+" - Batch "+m.IMS_BATCH.ToString()  :m.MASTER_VILLAGE.MAST_VILLAGE_NAME.ToString())))))))),
                            //m.IMS_UNLOCK_TABLE == "PR"?m.IMS_SANCTIONED_PROJECTS.IMS_ROAD_NAME.ToString():(m.IMS_UNLOCK_TABLE == "ER"?m.MASTER_EXISTING_ROADS.MAST_ER_ROAD_NAME.ToString():(m.IMS_UNLOCK_TABLE == "CN"?m.PLAN_ROAD.PLAN_RD_NAME.ToString():m.MASTER_HABITATIONS.MAST_HAB_NAME.ToString())),
                            m.IMS_UNLOCK_START_DATE.ToString("dd/MM/yyyy"),
                            m.IMS_UNLOCK_END_DATE.ToString("dd/MM/yyyy"),
                            m.IMS_UNLOCK_REMARKS.ToString(),
                        }
                    }).ToArray();

                    return gridData;
                }
                catch (Exception ex)
                {
                    ErrorLog.LogError(ex, "GetUnlockRecordListDAL().DAL");
                    totalRecords = 0;
                    return null;
                }
            }
        }

        public Array GetProposalDetails(int levelCode, string levelName, int scheme, string type, string yearbatch, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            dbContext = new PMGSYEntities();
            try
            {
                int stateCode = 0;
                int districtCode = 0;
                int blockCode = 0;
                int yearCode = 0;
                string[] arrParams = null;

                //switch (levelName)
                //{
                //    case "S":
                //        stateCode = levelCode;
                //        break;
                //    case "D":
                //        districtCode = levelCode;
                //        break;
                //    case "B":
                //        blockCode = levelCode;
                //        break;
                //}
                if (!String.IsNullOrEmpty(yearbatch))
                {
                    arrParams = yearbatch.Split('$');
                    stateCode = Convert.ToInt32(arrParams[0]);
                    districtCode = Convert.ToInt32(arrParams[1]);
                    blockCode = Convert.ToInt32(arrParams[2]);
                    yearCode = Convert.ToInt32(arrParams[3]);
                }

                var lstPropDetails = (from item in dbContext.IMS_SANCTIONED_PROJECTS
                                      where
                                      (levelName == "S" ? item.MASTER_DISTRICT.MAST_STATE_CODE : (levelName == "D" ? item.MAST_DISTRICT_CODE : (levelName == "B" ? item.MAST_BLOCK_CODE : (levelName == "T" ? item.IMS_BATCH : (levelName == "Y" ? item.IMS_YEAR : item.IMS_PR_ROAD_CODE))))) == levelCode
                                      && item.MAST_PMGSY_SCHEME == scheme
                                      && (type == "M" ? item.IMS_SANCTIONED == "Y" : item.STA_SANCTIONED == "Y")
                                      && (((levelName == "T" || levelName == "Y") ? item.MAST_STATE_CODE : 1)) == ((levelName == "T" || levelName == "Y") ? (stateCode <= 0 ? 0 : stateCode) : 1)
                                      && (((levelName == "T" || levelName == "Y") ? (districtCode <= 0 ? 0 : item.MAST_DISTRICT_CODE) : 1)) == ((levelName == "T" || levelName == "Y") ? (districtCode <= 0 ? 0 : districtCode) : 1)
                                      && (((levelName == "T" || levelName == "Y") ? (blockCode <= 0 ? 0 : item.MAST_BLOCK_CODE) : 1)) == ((levelName == "T" || levelName == "Y") ? (blockCode <= 0 ? 0 : blockCode) : 1)
                                      && ((levelName == "T" ? (yearCode <= 0 ? 0 : item.IMS_YEAR) : 1)) == (levelName == "T" ? (yearCode <= 0 ? 0 : yearCode) : 1)

                                      select new
                                      {
                                          ROAD_NAME = item.IMS_PROPOSAL_TYPE == "P" ? item.IMS_ROAD_NAME : item.IMS_BRIDGE_NAME,
                                          item.IMS_PR_ROAD_CODE,
                                          item.MASTER_DISTRICT.MAST_DISTRICT_NAME,
                                          item.MASTER_BLOCK.MAST_BLOCK_NAME,
                                          item.IMS_YEAR,
                                          item.IMS_PACKAGE_ID,
                                          item.IMS_BATCH,
                                          ROAD_LENGTH = item.IMS_PROPOSAL_TYPE == "P" ? item.IMS_PAV_LENGTH : item.IMS_BRIDGE_LENGTH,
                                      }).Distinct().ToList();



                totalRecords = lstPropDetails.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "ROAD_NAME":
                                lstPropDetails = lstPropDetails.OrderBy(x => x.ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_DISTRICT_NAME":
                                lstPropDetails = lstPropDetails.OrderBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_BLOCK_NAME":
                                lstPropDetails = lstPropDetails.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_PACKAGE_ID":
                                lstPropDetails = lstPropDetails.OrderBy(x => x.IMS_PACKAGE_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_BATCH":
                                lstPropDetails = lstPropDetails.OrderBy(x => x.IMS_BATCH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_YEAR":
                                lstPropDetails = lstPropDetails.OrderBy(x => x.IMS_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstPropDetails = lstPropDetails.OrderBy(x => x.ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "ROAD_NAME":
                                lstPropDetails = lstPropDetails.OrderByDescending(x => x.ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_DISTRICT_NAME":
                                lstPropDetails = lstPropDetails.OrderByDescending(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_BLOCK_NAME":
                                lstPropDetails = lstPropDetails.OrderByDescending(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_PACKAGE_ID":
                                lstPropDetails = lstPropDetails.OrderByDescending(x => x.IMS_PACKAGE_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_BATCH":
                                lstPropDetails = lstPropDetails.OrderByDescending(x => x.IMS_BATCH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_YEAR":
                                lstPropDetails = lstPropDetails.OrderByDescending(x => x.IMS_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstPropDetails = lstPropDetails.OrderByDescending(x => x.ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstPropDetails = lstPropDetails.OrderByDescending(x => x.ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                var gridData = lstPropDetails.Select(m => new
                {
                    cell = new[]
                        {
                            m.ROAD_NAME == null?"":m.ROAD_NAME.ToString(),
                            m.MAST_BLOCK_NAME == null?"":m.MAST_BLOCK_NAME.ToString(),
                            m.MAST_DISTRICT_NAME == null?"":m.MAST_DISTRICT_NAME.ToString(),
                            m.IMS_PACKAGE_ID == null?"":m.IMS_PACKAGE_ID.ToString(),
                            (m.IMS_YEAR + " - " + (m.IMS_YEAR + 1)).ToString(),
                            m.IMS_BATCH == null?"":m.IMS_BATCH.ToString(),
                            m.ROAD_LENGTH == null?"":m.ROAD_LENGTH.ToString(),
                            //"<a href='#' title='Click to Lock Proposal Details' class='ui-icon ui-icon-search ui-align-center' onClick='ViewDetails(\"" + URLEncrypt.EncryptParameters(new string[]{propDetails.LEVELCODE.ToString().Trim()+"$"+"H"}) +"\"); return false;'></a>"
                        }
                }).ToArray();

                return gridData;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetProposalDetails().DAL");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array GetProposalDetailsForITNO(int levelCode, string levelName, int scheme, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            dbContext = new PMGSYEntities();
            try
            {
                int stateCode = 0;
                int districtCode = 0;
                int blockCode = 0;
                switch (levelName)
                {
                    case "S":
                        stateCode = levelCode;
                        break;
                    case "D":
                        districtCode = levelCode;
                        break;
                    case "B":
                        blockCode = levelCode;
                        break;
                }

                var lstPropDetails = (from item in dbContext.IMS_SANCTIONED_PROJECTS
                                      where
                                      (levelName == "S" ? item.MASTER_DISTRICT.MAST_STATE_CODE : (levelName == "D" ? item.MAST_DISTRICT_CODE : (levelName == "B" ? item.MAST_BLOCK_CODE : item.IMS_PR_ROAD_CODE))) == levelCode
                                      && item.MAST_PMGSY_SCHEME == scheme
                                      &&
                                      ((item.IMS_ISCOMPLETED == "S" && item.STA_SANCTIONED == "U") ||
                                       (item.IMS_ISCOMPLETED == "D" && item.STA_SANCTIONED == "N"))
                                      select new
                                      {
                                          ROAD_NAME = item.IMS_PROPOSAL_TYPE == "P" ? item.IMS_ROAD_NAME : item.IMS_BRIDGE_NAME,
                                          item.IMS_PR_ROAD_CODE,
                                          item.MASTER_DISTRICT.MAST_DISTRICT_NAME,
                                          item.MASTER_BLOCK.MAST_BLOCK_NAME,
                                          item.IMS_PACKAGE_ID,
                                          item.IMS_BATCH,
                                          ROAD_LENGTH = item.IMS_PROPOSAL_TYPE == "P" ? item.IMS_PAV_LENGTH : item.IMS_BRIDGE_LENGTH,
                                      }).Distinct().ToList();



                totalRecords = lstPropDetails.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "ROAD_NAME":
                                lstPropDetails = lstPropDetails.OrderBy(x => x.ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_DISTRICT_NAME":
                                lstPropDetails = lstPropDetails.OrderBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_BLOCK_NAME":
                                lstPropDetails = lstPropDetails.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_PACKAGE_ID":
                                lstPropDetails = lstPropDetails.OrderBy(x => x.IMS_PACKAGE_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_BATCH":
                                lstPropDetails = lstPropDetails.OrderBy(x => x.IMS_BATCH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstPropDetails = lstPropDetails.OrderBy(x => x.ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "ROAD_NAME":
                                lstPropDetails = lstPropDetails.OrderByDescending(x => x.ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_DISTRICT_NAME":
                                lstPropDetails = lstPropDetails.OrderByDescending(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_BLOCK_NAME":
                                lstPropDetails = lstPropDetails.OrderByDescending(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_PACKAGE_ID":
                                lstPropDetails = lstPropDetails.OrderByDescending(x => x.IMS_PACKAGE_ID).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_BATCH":
                                lstPropDetails = lstPropDetails.OrderByDescending(x => x.IMS_BATCH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstPropDetails = lstPropDetails.OrderByDescending(x => x.ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstPropDetails = lstPropDetails.OrderByDescending(x => x.ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                var gridData = lstPropDetails.Select(m => new
                {
                    cell = new[]
                        {
                            m.ROAD_NAME == null?"":m.ROAD_NAME.ToString(),
                            m.MAST_BLOCK_NAME == null?"":m.MAST_BLOCK_NAME.ToString(),
                            m.MAST_DISTRICT_NAME == null?"":m.MAST_DISTRICT_NAME.ToString(),
                            m.IMS_PACKAGE_ID == null?"":m.IMS_PACKAGE_ID.ToString(),
                            m.IMS_BATCH == null?"":m.IMS_BATCH.ToString(),
                            m.ROAD_LENGTH == null?"":m.ROAD_LENGTH.ToString(),
                            //"<a href='#' title='Click to Lock Proposal Details' class='ui-icon ui-icon-search ui-align-center' onClick='ViewDetails(\"" + URLEncrypt.EncryptParameters(new string[]{propDetails.LEVELCODE.ToString().Trim()+"$"+"H"}) +"\"); return false;'></a>"
                        }
                }).ToArray();

                return gridData;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "GetProposalDetailsForITNO");
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }

        }

        public Array GetDRRPDetails(int levelCode, string levelName, int scheme, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            dbContext = new PMGSYEntities();
            try
            {
                var lstDRRPDetails = (from item in dbContext.MASTER_EXISTING_ROADS
                                      where
                                      (levelName == "S" ? item.MASTER_DISTRICT.MAST_STATE_CODE : (levelName == "D" ? item.MAST_DISTRICT_CODE : (levelName == "B" ? item.MAST_BLOCK_CODE : item.MAST_ER_ROAD_CODE))) == levelCode
                                      && item.MAST_PMGSY_SCHEME == scheme
                                      select new
                                      {
                                          item.MAST_ER_ROAD_NAME,
                                          item.MAST_ER_ROAD_NUMBER,
                                          item.MASTER_DISTRICT.MAST_DISTRICT_NAME,
                                          item.MASTER_BLOCK.MAST_BLOCK_NAME,
                                          item.MAST_ER_ROAD_STR_CHAIN,
                                          item.MAST_ER_ROAD_END_CHAIN,
                                          item.MASTER_ROAD_CATEGORY.MAST_ROAD_CAT_NAME,
                                          item.MASTER_AGENCY.MAST_AGENCY_NAME
                                      }).Distinct().ToList();

                totalRecords = lstDRRPDetails.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "MAST_ER_ROAD_NAME":
                                lstDRRPDetails = lstDRRPDetails.OrderBy(x => x.MAST_ER_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_DISTRICT_NAME":
                                lstDRRPDetails = lstDRRPDetails.OrderBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_BLOCK_NAME":
                                lstDRRPDetails = lstDRRPDetails.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_NUMBER":
                                lstDRRPDetails = lstDRRPDetails.OrderBy(x => x.MAST_ER_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_AGENCY_NAME":
                                lstDRRPDetails = lstDRRPDetails.OrderBy(x => x.MAST_AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ROAD_CAT_NAME":
                                lstDRRPDetails = lstDRRPDetails.OrderBy(x => x.MAST_ROAD_CAT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstDRRPDetails = lstDRRPDetails.OrderBy(x => x.MAST_ER_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "MAST_ER_ROAD_NAME":
                                lstDRRPDetails = lstDRRPDetails.OrderByDescending(x => x.MAST_ER_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_DISTRICT_NAME":
                                lstDRRPDetails = lstDRRPDetails.OrderByDescending(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_BLOCK_NAME":
                                lstDRRPDetails = lstDRRPDetails.OrderByDescending(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ER_ROAD_NUMBER":
                                lstDRRPDetails = lstDRRPDetails.OrderByDescending(x => x.MAST_ER_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_AGENCY_NAME":
                                lstDRRPDetails = lstDRRPDetails.OrderByDescending(x => x.MAST_AGENCY_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_ROAD_CAT_NAME":
                                lstDRRPDetails = lstDRRPDetails.OrderByDescending(x => x.MAST_ROAD_CAT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstDRRPDetails = lstDRRPDetails.OrderByDescending(x => x.MAST_ER_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstDRRPDetails = lstDRRPDetails.OrderByDescending(x => x.MAST_ER_ROAD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                var gridData = lstDRRPDetails.Select(m => new
                {
                    cell = new[]
                        {
                            m.MAST_ER_ROAD_NAME == null?"":m.MAST_ER_ROAD_NAME.ToString(),
                            m.MAST_ER_ROAD_NUMBER == null?"":m.MAST_ER_ROAD_NUMBER.ToString(),
                            m.MAST_BLOCK_NAME == null?"":m.MAST_BLOCK_NAME.ToString(),
                            m.MAST_DISTRICT_NAME == null?"":m.MAST_DISTRICT_NAME.ToString(),
                            m.MAST_ROAD_CAT_NAME == null?"":m.MAST_ROAD_CAT_NAME.ToString(),
                            m.MAST_AGENCY_NAME == null?"":m.MAST_AGENCY_NAME.ToString(),
                        }
                }).ToArray();

                return gridData;
            }
            catch (Exception)
            {
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array GetCNDetails(int levelCode, string levelName, int scheme, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            dbContext = new PMGSYEntities();
            try
            {
                var lstCNDetails = (from item in dbContext.PLAN_ROAD
                                    where
                                    (levelName == "S" ? item.MASTER_DISTRICT.MAST_STATE_CODE : (levelName == "D" ? item.MAST_DISTRICT_CODE : (levelName == "B" ? item.MAST_BLOCK_CODE : item.PLAN_CN_ROAD_CODE))) == levelCode
                                    && item.MAST_PMGSY_SCHEME == scheme
                                    select new
                                    {
                                        item.PLAN_RD_NAME,
                                        item.PLAN_CN_ROAD_NUMBER,
                                        item.MASTER_DISTRICT.MAST_DISTRICT_NAME,
                                        item.MASTER_BLOCK.MAST_BLOCK_NAME,
                                        item.PLAN_RD_LENGTH,
                                        ROAD_TYPE = item.PLAN_RD_LENG == "P" ? "Partial" : "Full",
                                    }).Distinct().ToList();

                totalRecords = lstCNDetails.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "PLAN_RD_NAME":
                                lstCNDetails = lstCNDetails.OrderBy(x => x.PLAN_RD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_DISTRICT_NAME":
                                lstCNDetails = lstCNDetails.OrderBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_BLOCK_NAME":
                                lstCNDetails = lstCNDetails.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_CN_ROAD_NUMBER":
                                lstCNDetails = lstCNDetails.OrderBy(x => x.PLAN_CN_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_LENGTH":
                                lstCNDetails = lstCNDetails.OrderBy(x => x.PLAN_RD_LENGTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "ROAD_TYPE":
                                lstCNDetails = lstCNDetails.OrderBy(x => x.ROAD_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstCNDetails = lstCNDetails.OrderBy(x => x.PLAN_RD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "PLAN_RD_NAME":
                                lstCNDetails = lstCNDetails.OrderByDescending(x => x.PLAN_RD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_DISTRICT_NAME":
                                lstCNDetails = lstCNDetails.OrderByDescending(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_BLOCK_NAME":
                                lstCNDetails = lstCNDetails.OrderByDescending(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_CN_ROAD_NUMBER":
                                lstCNDetails = lstCNDetails.OrderByDescending(x => x.PLAN_CN_ROAD_NUMBER).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PLAN_RD_LENGTH":
                                lstCNDetails = lstCNDetails.OrderByDescending(x => x.PLAN_RD_LENGTH).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "ROAD_TYPE":
                                lstCNDetails = lstCNDetails.OrderByDescending(x => x.ROAD_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstCNDetails = lstCNDetails.OrderByDescending(x => x.PLAN_RD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstCNDetails = lstCNDetails.OrderByDescending(x => x.PLAN_RD_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                var gridData = lstCNDetails.Select(m => new
                {
                    cell = new[]
                        {
                            m.PLAN_RD_NAME == null?"":m.PLAN_RD_NAME.ToString(),
                            m.PLAN_CN_ROAD_NUMBER == null?"":m.PLAN_CN_ROAD_NUMBER.ToString(),
                            m.MAST_BLOCK_NAME == null?"":m.MAST_BLOCK_NAME.ToString(),
                            m.MAST_DISTRICT_NAME == null?"":m.MAST_DISTRICT_NAME.ToString(),
                            m.ROAD_TYPE == null?"":m.ROAD_TYPE.ToString(),
                            m.PLAN_RD_LENGTH == null?"":m.PLAN_RD_LENGTH.ToString(),
                        }
                }).ToArray();

                return gridData;

            }
            catch (Exception)
            {
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array GetHabDetails(int levelCode, string levelName, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            dbContext = new PMGSYEntities();
            try
            {
                var lstHabDetails = (from item in dbContext.MASTER_HABITATIONS
                                     join habDetails in dbContext.MASTER_HABITATIONS_DETAILS on item.MAST_HAB_CODE equals habDetails.MAST_HAB_CODE
                                     where
                                     (levelName == "S" ? item.MASTER_VILLAGE.MASTER_BLOCK.MASTER_DISTRICT.MASTER_STATE.MAST_STATE_CODE : (levelName == "D" ? item.MASTER_VILLAGE.MASTER_BLOCK.MASTER_DISTRICT.MAST_DISTRICT_CODE : (levelName == "B" ? item.MASTER_VILLAGE.MASTER_BLOCK.MAST_BLOCK_CODE : (levelName == "V" ? item.MAST_VILLAGE_CODE : item.MAST_HAB_CODE)))) == levelCode
                                     select new
                                     {
                                         item.MAST_HAB_NAME,
                                         item.MASTER_MLA_CONSTITUENCY.MAST_MLA_CONST_NAME,
                                         item.MASTER_MP_CONSTITUENCY.MAST_MP_CONST_NAME,
                                         IS_SCHEDULE5 = (item.MAST_SCHEDULE5 == "Y" ? "Yes" : "No"),
                                         HAB_STATUS = (item.MAST_HAB_STATUS == "U" ? "Unconnected" : "Connected"),
                                         habDetails.MAST_HAB_TOT_POP,
                                         habDetails.MAST_HAB_SCST_POP
                                     }).Distinct().ToList();

                totalRecords = lstHabDetails.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "MAST_HAB_NAME":
                                lstHabDetails = lstHabDetails.OrderBy(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_MLA_CONST_NAME":
                                lstHabDetails = lstHabDetails.OrderBy(x => x.MAST_MLA_CONST_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_MP_CONST_NAME":
                                lstHabDetails = lstHabDetails.OrderBy(x => x.MAST_MP_CONST_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IS_SCHEDULE5":
                                lstHabDetails = lstHabDetails.OrderBy(x => x.IS_SCHEDULE5).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "HAB_STATUS":
                                lstHabDetails = lstHabDetails.OrderBy(x => x.HAB_STATUS).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_HAB_TOT_POP":
                                lstHabDetails = lstHabDetails.OrderBy(x => x.MAST_HAB_TOT_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_HAB_SCST_POP":
                                lstHabDetails = lstHabDetails.OrderBy(x => x.MAST_HAB_SCST_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstHabDetails = lstHabDetails.OrderBy(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "MAST_HAB_NAME":
                                lstHabDetails = lstHabDetails.OrderByDescending(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_MLA_CONST_NAME":
                                lstHabDetails = lstHabDetails.OrderByDescending(x => x.MAST_MLA_CONST_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_MP_CONST_NAME":
                                lstHabDetails = lstHabDetails.OrderByDescending(x => x.MAST_MP_CONST_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IS_SCHEDULE5":
                                lstHabDetails = lstHabDetails.OrderByDescending(x => x.IS_SCHEDULE5).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "HAB_STATUS":
                                lstHabDetails = lstHabDetails.OrderByDescending(x => x.HAB_STATUS).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_HAB_TOT_POP":
                                lstHabDetails = lstHabDetails.OrderByDescending(x => x.MAST_HAB_TOT_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_HAB_SCST_POP":
                                lstHabDetails = lstHabDetails.OrderByDescending(x => x.MAST_HAB_SCST_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstHabDetails = lstHabDetails.OrderByDescending(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstHabDetails = lstHabDetails.OrderByDescending(x => x.MAST_HAB_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                var gridData = lstHabDetails.Select(m => new
                {
                    cell = new[]
                        {
                            m.MAST_HAB_NAME == null?"":m.MAST_HAB_NAME.ToString(),
                            m.MAST_MLA_CONST_NAME == null?"":m.MAST_MLA_CONST_NAME.ToString(),
                            m.MAST_MP_CONST_NAME== null?"":m.MAST_MP_CONST_NAME.ToString(),
                            m.IS_SCHEDULE5.ToString(),
                            m.HAB_STATUS.ToString(),
                            m.MAST_HAB_TOT_POP.ToString(),
                            m.MAST_HAB_SCST_POP.ToString()
                        }
                }).ToArray();

                return gridData;
            }
            catch (Exception)
            {
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array GetVillageDetails(int levelCode, string levelName, int page, int rows, string sidx, string sord, out long totalRecords)
        {
            dbContext = new PMGSYEntities();
            try
            {
                var lstVillageDetails = (from item in dbContext.MASTER_VILLAGE
                                         where
                                         (levelName == "S" ? item.MASTER_BLOCK.MASTER_DISTRICT.MASTER_STATE.MAST_STATE_CODE : (levelName == "D" ? item.MASTER_BLOCK.MASTER_DISTRICT.MAST_DISTRICT_CODE : item.MASTER_BLOCK.MAST_BLOCK_CODE)) == levelCode
                                         select new
                                         {
                                             item.MAST_VILLAGE_NAME,
                                             item.MASTER_BLOCK.MAST_BLOCK_NAME,
                                             item.MASTER_BLOCK.MASTER_DISTRICT.MAST_DISTRICT_NAME,
                                             IS_SCHEDULE5 = (item.MAST_SCHEDULE5 == "Y" ? "Yes" : "No"),
                                             item.MAST_VILLAGE_TOT_POP,
                                             item.MAST_VILLAGE_SCST_POP
                                         }).Distinct().ToList();

                totalRecords = lstVillageDetails.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "MAST_VILLAGE_NAME":
                                lstVillageDetails = lstVillageDetails.OrderBy(x => x.MAST_VILLAGE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_BLOCK_NAME":
                                lstVillageDetails = lstVillageDetails.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_DISTRICT_NAME":
                                lstVillageDetails = lstVillageDetails.OrderBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IS_SCHEDULE5":
                                lstVillageDetails = lstVillageDetails.OrderBy(x => x.IS_SCHEDULE5).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_VILLAGE_TOT_POP":
                                lstVillageDetails = lstVillageDetails.OrderBy(x => x.MAST_VILLAGE_TOT_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_VILLAGE_SCST_POP":
                                lstVillageDetails = lstVillageDetails.OrderBy(x => x.MAST_VILLAGE_SCST_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstVillageDetails = lstVillageDetails.OrderBy(x => x.MAST_VILLAGE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "MAST_VILLAGE_NAME":
                                lstVillageDetails = lstVillageDetails.OrderByDescending(x => x.MAST_VILLAGE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_BLOCK_NAME":
                                lstVillageDetails = lstVillageDetails.OrderByDescending(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_DISTRICT_NAME":
                                lstVillageDetails = lstVillageDetails.OrderByDescending(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IS_SCHEDULE5":
                                lstVillageDetails = lstVillageDetails.OrderByDescending(x => x.IS_SCHEDULE5).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_VILLAGE_TOT_POP":
                                lstVillageDetails = lstVillageDetails.OrderByDescending(x => x.MAST_VILLAGE_TOT_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_VILLAGE_SCST_POP":
                                lstVillageDetails = lstVillageDetails.OrderByDescending(x => x.MAST_VILLAGE_SCST_POP).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstVillageDetails = lstVillageDetails.OrderByDescending(x => x.MAST_VILLAGE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstVillageDetails = lstVillageDetails.OrderByDescending(x => x.MAST_VILLAGE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                var gridData = lstVillageDetails.Select(m => new
                {
                    cell = new[]
                        {
                            m.MAST_VILLAGE_NAME == null?"":m.MAST_VILLAGE_NAME.ToString(),
                            m.MAST_BLOCK_NAME == null?"":m.MAST_BLOCK_NAME.ToString(),
                            m.MAST_DISTRICT_NAME == null?"":m.MAST_DISTRICT_NAME.ToString(),
                            m.IS_SCHEDULE5.ToString(),
                            m.MAST_VILLAGE_TOT_POP.ToString(),
                            m.MAST_VILLAGE_SCST_POP.ToString()
                        }
                }).ToArray();

                return gridData;
            }
            catch (Exception)
            {
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Array GetUnlockReportList(int stateCode, int districtCode, int blockCode, int moduleCode, int schemeCode, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            dbContext = new PMGSYEntities();
            try
            {
                string unlockTable = string.Empty;

                switch (moduleCode)
                {
                    case 1:
                        unlockTable = "HM";
                        break;
                    case 2:
                        unlockTable = "PR";
                        break;
                    case 3:
                        unlockTable = "ER";
                        break;
                    case 4:
                        unlockTable = "CN";
                        break;
                    case 5:
                        unlockTable = "VM";
                        break;
                }

                var lstUnlockDetails = (from item in dbContext.IMS_UNLOCK_DETAILS
                                        where
                                        (stateCode == 0 ? 1 : item.MAST_STATE_CODE) == (stateCode == 0 ? 1 : stateCode) &&
                                        (districtCode <= 0 ? 1 : item.MAST_DISTRICT_CODE) == (districtCode <= 0 ? 1 : districtCode) &&
                                        (blockCode <= 0 ? 1 : item.MAST_BLOCK_CODE) == (blockCode <= 0 ? 1 : blockCode) &&
                                        (item.IMS_UNLOCK_TABLE == unlockTable) &&
                                        (item.MAST_PMGSY_SCHEME == schemeCode)
                                        select new
                                        {
                                            item.MASTER_STATE.MAST_STATE_NAME,
                                            item.MASTER_DISTRICT.MAST_DISTRICT_NAME,
                                            item.MASTER_BLOCK.MAST_BLOCK_NAME,
                                            UNLOCK_LEVEL = (item.IMS_UNLOCK_LEVEL == "S" ? "State" : (item.IMS_UNLOCK_LEVEL == "D" ? "District" : (item.IMS_UNLOCK_LEVEL == "B" ? "Block" : (item.IMS_UNLOCK_LEVEL == "R" ? "Road" : (item.IMS_UNLOCK_LEVEL == "V" ? "Village" : (item.IMS_UNLOCK_LEVEL == "T" ? "Batch" : (item.IMS_UNLOCK_LEVEL == "Y" ? "Year" : ""))))))),
                                            UNLOCK_DATA = (item.IMS_UNLOCK_LEVEL == "S" ? item.MASTER_STATE.MAST_STATE_NAME : (item.IMS_UNLOCK_LEVEL == "D" ? item.MASTER_DISTRICT.MAST_DISTRICT_NAME : (item.IMS_UNLOCK_LEVEL == "B" ? item.MASTER_BLOCK.MAST_BLOCK_NAME : (item.IMS_UNLOCK_LEVEL == "R" ? (item.IMS_UNLOCK_TABLE == "PR" ? item.IMS_SANCTIONED_PROJECTS.IMS_ROAD_NAME : (item.IMS_UNLOCK_TABLE == "ER" ? item.MASTER_EXISTING_ROADS.MAST_ER_ROAD_NAME : item.PLAN_ROAD.PLAN_RD_NAME)) : (item.IMS_UNLOCK_LEVEL == "V" ? item.MASTER_VILLAGE.MAST_VILLAGE_NAME : (item.IMS_UNLOCK_LEVEL == "H" ? item.MASTER_HABITATIONS.MAST_HAB_NAME : (item.IMS_UNLOCK_LEVEL == "T" ? item.MASTER_BATCH.MAST_BATCH_NAME : item.MASTER_YEAR.MAST_YEAR_TEXT))))))),
                                            //UNLOCK_DATA = (unlockTable == "HM"?item.MASTER_HABITATIONS.MAST_HAB_NAME:(unlockTable == "PR"?item.IMS_SANCTIONED_PROJECTS.IMS_ROAD_NAME:(unlockTable == "ER"?item.MASTER_EXISTING_ROADS.MAST_ER_ROAD_NAME:(unlockTable == "CN"?item.PLAN_ROAD.PLAN_RD_NAME:item.MASTER_VILLAGE.MAST_VILLAGE_NAME)))),
                                            item.IMS_UNLOCK_START_DATE,
                                            item.IMS_UNLOCK_END_DATE
                                        }).ToList();

                totalRecords = lstUnlockDetails.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "MAST_STATE_NAME":
                                lstUnlockDetails = lstUnlockDetails.OrderBy(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_DISTRICT_NAME":
                                lstUnlockDetails = lstUnlockDetails.OrderBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_BLOCK_NAME":
                                lstUnlockDetails = lstUnlockDetails.OrderBy(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "UNLOCK_LEVEL":
                                lstUnlockDetails = lstUnlockDetails.OrderBy(x => x.UNLOCK_LEVEL).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "UNLOCK_DATA":
                                lstUnlockDetails = lstUnlockDetails.OrderBy(x => x.UNLOCK_DATA).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_UNLOCK_START_DATE":
                                lstUnlockDetails = lstUnlockDetails.OrderBy(x => x.IMS_UNLOCK_START_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_UNLOCK_END_DATE":
                                lstUnlockDetails = lstUnlockDetails.OrderBy(x => x.IMS_UNLOCK_END_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstUnlockDetails = lstUnlockDetails.OrderBy(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }

                    }
                    else
                    {
                        switch (sidx)
                        {
                            case "MAST_STATE_NAME":
                                lstUnlockDetails = lstUnlockDetails.OrderByDescending(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_DISTRICT_NAME":
                                lstUnlockDetails = lstUnlockDetails.OrderByDescending(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "MAST_BLOCK_NAME":
                                lstUnlockDetails = lstUnlockDetails.OrderByDescending(x => x.MAST_BLOCK_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "UNLOCK_LEVEL":
                                lstUnlockDetails = lstUnlockDetails.OrderByDescending(x => x.UNLOCK_LEVEL).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "UNLOCK_DATA":
                                lstUnlockDetails = lstUnlockDetails.OrderByDescending(x => x.UNLOCK_DATA).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_UNLOCK_START_DATE":
                                lstUnlockDetails = lstUnlockDetails.OrderByDescending(x => x.IMS_UNLOCK_START_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "IMS_UNLOCK_END_DATE":
                                lstUnlockDetails = lstUnlockDetails.OrderByDescending(x => x.IMS_UNLOCK_END_DATE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            default:
                                lstUnlockDetails = lstUnlockDetails.OrderByDescending(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstUnlockDetails = lstUnlockDetails.OrderBy(x => x.MAST_STATE_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                }

                return lstUnlockDetails.Select(d => new
                {
                    cell = new[] {                         
                                    d.MAST_STATE_NAME==null?"-":d.MAST_STATE_NAME.ToString(),
                                    d.MAST_DISTRICT_NAME==null?"-":d.MAST_DISTRICT_NAME.ToString(),
                                    d.MAST_BLOCK_NAME==null?"-":d.MAST_BLOCK_NAME.ToString(),
                                    d.UNLOCK_LEVEL.ToString(),
                                    d.UNLOCK_DATA.ToString(),
                                    d.IMS_UNLOCK_START_DATE==null?"-":d.IMS_UNLOCK_START_DATE.ToString("dd/MM/yyyy"),
                                    d.IMS_UNLOCK_END_DATE == null? "-" :d.IMS_UNLOCK_END_DATE.ToString("dd/MM/yyyy")
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
                dbContext.Dispose();
            }
        }


        #endregion

        #region Common function
        public List<SelectListItem> GetAllDistrictsByAdminNDCode(int stateCode, int adminCode)
        {
            try
            {

                dbContext = new PMGSYEntities();

                int agencyCode = dbContext.ADMIN_DEPARTMENT.Where(x => x.ADMIN_ND_CODE == adminCode).Select(x => x.MAST_AGENCY_CODE).FirstOrDefault();
                string agencyType = dbContext.MASTER_AGENCY.Where(x => x.MAST_AGENCY_CODE == agencyCode).Select(x => x.MAST_AGENCY_TYPE).FirstOrDefault();

                if (agencyType != "O")
                {
                    var list = dbContext.MASTER_DISTRICT.Where(m => m.MAST_STATE_CODE == stateCode && m.MAST_DISTRICT_ACTIVE == "Y").OrderBy(a => a.MAST_DISTRICT_NAME).ToList();
                    return new SelectList(list.ToList(), "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME").ToList();
                }
                else
                {

                    var list = (from aad in dbContext.ADMIN_AGENCY_DISTRICT
                                join md in dbContext.MASTER_DISTRICT on aad.MAST_DISTRICT_CODE equals md.MAST_DISTRICT_CODE
                                where aad.ADMIN_ND_CODE == adminCode &&
                                       md.MAST_DISTRICT_ACTIVE == "Y"
                                select md).OrderBy(a => a.MAST_DISTRICT_NAME).ToList();



                    return new SelectList(list.ToList(), "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME").ToList();

                }


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
        #endregion
    }

    public interface ILockUnlockDAL
    {

        Array GetProposalList(string propType, int yearCode, int stateCode, int districtCode, int blockCode, int batchCode, string packageCode, byte scheme, int collaboration, int roleCode, string type, int page, int rows, string sidx, string sord, out long totalRecords, out List<int> lstIds, out string ImsPrRoadCode);
        Array GetProposalLockList(int yearCode, int stateCode, int districtCode, int batchCode, string packageCode, int page, int rows, string sidx, string sord, out long totalRecords, out List<int> lstIds, out string ImsPrRoadCode);
        Array GetFreezeUnfreezeReportDetails(int stateCode, int batchCode, int yearCode, int schemeCode, int? page, int? rows, string sidx, string sord, out long totalRecords);

        bool AddLockDetails(LockDetailsViewModel lockModel, ref string message);
        bool LockUnlockProposal(ProposalFilterViewModel model, ref string message);
        Array GetCoreNetworkList(int stateCode, int districtCode, int blockCode, byte scheme, int collaboration, int roleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetExistingRoadList(int stateCode, int districtCode, int blockCode, byte scheme, int collaboration, int roleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetTenderingList(int stateCode, int districtCode, int yearCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetAgreementList(int stateCode, int districtCode, int yearCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetManeCoreNetworkList(int stateCode, int districtCode, int blockCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetIMSContractList(int yearCode, int stateCode, int districtCode, int batchCode, string packageCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetProposalsDAL(int? page, int? rows, string sidx, string sord, out Int32 totalRecords, int IMS_YEAR, int IMS_MAST_STATE_CODE, int IMS_BATCH, int Scheme);
        bool FreezeUnfreezeProposal(ProposalFilterLockUnlockViewModel LockUnlockViewModel, ref string message);
        Int32? GetMaxImsTransactionNumber(ProposalFilterLockUnlockViewModel LockUnlockViewModel);
        Array GetCoreNetworkUnlockList(int stateCode, int districtCode, int blockCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetExistingRoadUnlockList(int stateCode, int districtCode, int blockCode, int page, int rows, string sidx, string sord, out long totalRecords);

        #region UNLOCK

        Array GetStateList(string moduleCode, byte scheme, int collaboration, int roleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetDistrictList(string moduleCode, int stateCode, byte scheme, int collaboration, int roleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetYearsList(string moduleCode, int stateCode, int districtCode, int blockCode, byte scheme, int collaboration, int roleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetBatchesList(string moduleCode, int stateCode, int districtCode, int blockCode, int yearCode, byte scheme, int collaboration, int roleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetBlockList(string moduleCode, int stateCode, int districtCode, byte scheme, int collaboration, int roleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetVillageList(string moduleCode, int blockCode, byte scheme, int collaboration, int roleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetHabitationList(string moduleCode, int villageCode, byte scheme, int collaboration, int roleCode, int page, int rows, string sidx, string sord, out long totalRecords);
        bool AddUnlockDetails(UnlockDetailsViewModel model, ref string message);
        Array GetITNOProposalList(int yearCode, int stateCode, int districtCode, int blockCode, int batchCode, string packageCode, byte scheme, string type, int page, int rows, string sidx, string sord, out long totalRecords, out List<int> lstIds, out string ImsPrRoadCode);
        Array GetUnlockRecordListDAL(int stateCode, int moduleCode, int page, int rows, string sidx, string sord, out long totalRecords);


        Array GetProposalDetails(int levelCode, string levelName, int scheme, string type, string yearbatch, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetProposalDetailsForITNO(int levelCode, string levelName, int scheme, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetDRRPDetails(int levelCode, string levelName, int scheme, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetCNDetails(int levelCode, string levelName, int scheme, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetHabDetails(int levelCode, string levelName, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetVillageDetails(int levelCode, string levelName, int page, int rows, string sidx, string sord, out long totalRecords);
        Array GetUnlockReportList(int stateCode, int districtCode, int blockCode, int moduleCode, int schemeCode, int? page, int? rows, string sidx, string sord, out long totalRecords);

        #endregion


    }

}